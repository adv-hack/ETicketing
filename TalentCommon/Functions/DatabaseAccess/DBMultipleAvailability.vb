Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Text
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Multiple stock enquires
'
'       Date                        Mar 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBMAV- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       29/05/07    /001    Ben     Re-write parms to make more extensible
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBMultipleAvailability
    Inherits DBAccess

    Private _dep As DEProductAlert
    Private _depa As DePNA
    Private _usage As Int16 = 0
    Private ParmItems As New Generic.List(Of String)

    Public Property Dep() As DEProductAlert
        Get
            Return _dep
        End Get
        Set(ByVal value As DEProductAlert)
            _dep = value
        End Set
    End Property
    Public Property Depa() As DePNA
        Get
            Return _depa
        End Get
        Set(ByVal value As DePNA)
            _depa = value
        End Set
    End Property
    Public Property Usage() As Int16
        Get
            Return _usage
        End Get
        Set(ByVal value As Int16)
            _usage = value
        End Set
    End Property

    Protected Overrides Function ValidateAgainstDatabaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        '   PARAM1  = (line no (4-int) & product (30-text) & Alt item (1) & Wharehouse(10-text) & SPARE (up to 100) ) by 20 thus 2000 char long
        '   PARAM2  = (Line no (4-int) & product (30-text) & Warehouse(10-text) & Quantity(4 - number) &
        '              line error (4-string) & SPARE (up to 100 ) by 80 thus 6000 char long (allows for multiple 
        '             warehouse for each item)
        '
        err = DataEntityUnPackSystem21()
        '
        If Not err.HasError Then
            Dim PARAMOUT As String = String.Empty
            Dim cmdSelect As iDB2Command = Nothing
            Dim dtrProducts As SqlDataReader = Nothing
            Dim paraminput As iDB2Parameter
            Dim paramoutput As iDB2Parameter
            Dim SQLString As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                      "/XSTOCKENQ(@PARAM1, @PARAM2) "
            '
            Dim dt As New DataTable
            With dt.Columns
                .Add("LineNo", GetType(Double))
                .Add("ProductNumber", GetType(String))
                .Add("WareHouse", GetType(String))
                .Add("Quantity", GetType(Double))
                .Add("QuantityOnOrder", GetType(String))
                .Add("Description", GetType(String))
                .Add("ErrorCode", GetType(String))
            End With
            ResultDataSet = New DataSet

            Dim loopCounter As Integer = 0, _
                loopAgain As Boolean = True

            Try

                If ParmItems.Count > 0 Then

                    cmdSelect = New iDB2Command(SQLString, conSystem21)

                    Do While loopAgain

                        loopAgain = False

                        With cmdSelect

                            paraminput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 2048)
                            paraminput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                              Utilities.FixStringLength(Settings.AccountNo1, 8) & _
                                              Utilities.FixStringLength(Settings.AccountNo2, 3)

                            Dim startCounter As Integer = 0, _
                                endCounter As Integer = 0

                            If ParmItems.Count > (60 * loopCounter) Then
                                startCounter = (60 * loopCounter)
                                endCounter = ((60 * loopCounter) + 60)
                            Else
                                startCounter = 0
                                endCounter = ParmItems.Count
                            End If

                            If endCounter > ParmItems.Count - 1 Then endCounter = ParmItems.Count - 1

                            For itemCounter As Integer = startCounter To endCounter
                                paraminput.Value += ParmItems(itemCounter)
                                If itemCounter = endCounter AndAlso ParmItems.Count - 1 > endCounter Then
                                    loopAgain = True
                                    loopCounter += 1
                                Else
                                    loopAgain = False
                                End If
                            Next

                            paraminput.Direction = ParameterDirection.Input
                            '
                            paramoutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 8192)
                            paramoutput.Value = Param2
                            paramoutput.Direction = ParameterDirection.InputOutput

                            .ExecuteNonQuery()

                            PARAMOUT = cmdSelect.Parameters(Param2).Value.ToString
                            If PARAMOUT.Substring(8191, 1) = "Y" Then
                                With err
                                    .ErrorMessage = PARAMOUT
                                    .ErrorNumber = PARAMOUT.Substring(8187, 4)
                                    .ErrorStatus = "Error checking stock for item - " & _
                                                Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", err.ErrorNumber)
                                    .HasError = True
                                End With
                            Else
                                '--------------------------------------------------------------------
                                ' Build the response DataSet
                                '
                                '
                                '--------------------------------------------------------------------
                                '          1         2         3         4         5         6
                                '0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890 
                                'nnnnDI813953                      PK        0045D001
                                '0123456789012                     45         
                                '--------------------------------------------------------------------
                                '
                                Dim iCounter As Integer = 0
                                Dim iPosition As Integer = 0
                                '    Dim sWork As String = PARAMOUT.Substring(2)
                                Dim sWork As String = PARAMOUT.Substring(0, 8000)
                                Dim dr As DataRow = Nothing

                                ' For iCounter = 0 To sWork.Length \ 44
                                For iCounter = 0 To 79
                                    iPosition = iCounter * 100
                                    If sWork.Substring(iPosition, 100).Trim > String.Empty Then
                                        dr = Nothing
                                        dr = dt.NewRow()
                                        ' dr("LineNo") = swork.Substring(iposition + xx)
                                        dr("LineNo") = sWork.Substring(iPosition, 4)
                                        dr("ProductNumber") = sWork.Substring(iPosition + 4, 30)
                                        dr("WareHouse") = sWork.Substring(iPosition + 34, 10).Trim
                                        If sWork.Substring(iPosition + 44, 7).Trim <> String.Empty Then
                                            dr("Quantity") = CType(sWork.Substring(iPosition + 44, 7), Double)
                                        Else
                                            dr("Quantity") = 0
                                        End If
                                        If sWork.Substring(iPosition + 51, 7).Trim <> String.Empty Then
                                            dr("QuantityOnOrder") = CType(sWork.Substring(iPosition + 51, 4), Double)
                                        Else
                                            dr("QuantityOnOrder") = 0
                                        End If
                                        dr("Description") = sWork.Substring(iPosition + 58, 30).Trim
                                        dr("ErrorCode") = sWork.Substring(iPosition + 88, 4).Trim
                                        dt.Rows.Add(dr)
                                    Else
                                        Exit For
                                    End If
                                Next
                                '--------------------------------------------------------------------
                            End If
                        End With
                    Loop

                    cmdSelect.Dispose()

                    ResultDataSet.Tables.Add(dt)
                End If

                '--------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access "
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8 & "[" & PARAMOUT & "]"
                    .ErrorNumber = "TACDBMAV-08"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------
        Return err
    End Function

    Private Function DataEntityUnPackSystem21() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------
        '   put the parameter generation in its own function as it is likely to be used  
        '   several times
        '   PARAM1  = (product (30-text) & Wharehouse(10-text) ) by 20 thus 800 char long
        '
        Dim iItem As Integer = 0
        Dim iItems As Integer = 0
        Dim sItems As StringBuilder = Nothing
        Dim sAltPart As String = String.Empty
        Dim dea As New DEAlerts                 ' Items
        '
        Try
            '-----------------------------------------------------------------------------------------
            Select Case Usage
                Case Is = 0
                    sItems = New StringBuilder
                    For iItems = 1 To Dep.CollDEAlerts.Count
                        dea = Dep.CollDEAlerts.Item(iItems)
                        With dea
                            If .ProductCode <> String.Empty Then
                                sItems.Append(Utilities.FixStringLength(iItems.ToString.PadLeft(4, "0"), 4))
                                sItems.Append(Utilities.FixStringLength(.ProductCode, 30))
                                sItems.Append(" ")
                                sItems.Append(Utilities.FixStringLength(.BranchID, 10))
                            Else
                                sItems.Append(Utilities.FixStringLength(iItems.ToString.PadLeft(4, "0"), 4))
                                sItems.Append(Utilities.FixStringLength(.ManufacturerPartNumber, 30))
                                sItems.Append("1")
                                sItems.Append(Utilities.FixStringLength(.BranchID, 10))
                            End If
                            ' Make parm length up to 100
                            If sItems.Length Mod 100 <> 0 Then
                                sItems.Append(Utilities.FixStringLength(" ", 100 - (sItems.Length Mod 100)))
                            End If

                            If iItems \ 20 = iItems / 20 Then
                                iItem += 1
                                ParmItems.Add(sItems.ToString)
                                sItems = New StringBuilder
                            End If
                        End With
                    Next iItems
                    If (sItems.ToString).Length > 39 Then
                        iItem += 1
                        ParmItems.Add(sItems.ToString)
                    End If

                Case Is = 1
                    Dim sProducts() As String = Depa.SKU.Split(",")
                    Dim sWarehouse() As String = Depa.Warehouse.Split(",")
                    Dim sWork As String = String.Empty
                    Dim iCounter As Integer = 0
                    Dim iPosition As Integer = 0
                    '
                    sItems = New StringBuilder
                    For iItems = 0 To sProducts.GetUpperBound(0)
                        '
                        sItems.Append(Utilities.FixStringLength((iItems + 1).ToString.PadLeft(4, "0"), 4))
                        sItems.Append(Utilities.FixStringLength(sProducts(iItems), 30))
                        sItems.Append(" ")
                        sItems.Append(Utilities.FixStringLength(sWarehouse(iItems), 10))

                        ' Make parm length up to 100
                        If sItems.Length Mod 100 <> 0 Then
                            sItems.Append(Utilities.FixStringLength(" ", 100 - (sItems.Length Mod 100)))
                        End If

                        '
                        If iItems > 19 And (iItems \ 20 = iItems / 20) Then
                            iItem += 1
                            ParmItems.Add(sItems.ToString)
                            sItems = New StringBuilder
                        End If
                    Next iItems
                    If (sItems.ToString).Length > 39 Then
                        iItem += 1
                        ParmItems.Add(sItems.ToString)
                    End If

            End Select
            '-----------------------------------------------------------------------------------------
        Catch ex As Exception
            Const strError17 As String = "Could not Unpack Data Entity "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError17
                err.ErrorNumber = "TACDBMAV-17"
                err.HasError = True
            End With

        End Try
        Return err
    End Function

    Private Function DataEntityUnPackChorus() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------
        Try
            Dim sProducts() As String = Depa.SKU.Split(",")
            'Dim sWarehouse() As String = Depa.Warehouse.Split(",")

            For Each item As String In sProducts
                ParmItems.Add(item)
            Next

            '-----------------------------------------------------------------------------------------
        Catch ex As Exception
            Const strError17 As String = "Could not Unpack Data Entity "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError17
                err.ErrorNumber = "TACDBMAV-18"
                err.HasError = True
            End With

        End Try
        Return err
    End Function


    Protected Overrides Function ValidateAgainstDatabaseChorus() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        '   PARAM1  = (line no (4-int) & product (30-text) & Alt item (1) & Wharehouse(10-text) & SPARE (up to 100) ) by 20 thus 2000 char long
        '   PARAM2  = (Line no (4-int) & product (30-text) & Warehouse(10-text) & Quantity(4 - number) &
        '              line error (4-string) & SPARE (up to 100 ) by 80 thus 6000 char long (allows for multiple 
        '             warehouse for each item)
        '
        err = DataEntityUnPackChorus()
        '
        If Not err.HasError Then
            Dim PARAMOUT As String = String.Empty
            Dim cmdSelect As iDB2Command = Nothing
            Dim dtrProducts As SqlDataReader = Nothing
            Dim paraminput As iDB2Parameter
            Dim paramoutput As iDB2Parameter
            Dim SQLString As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                      "/XSTOCKENQ(@PARAM1, @PARAM2) "
            '
            Dim dt As New DataTable
            With dt.Columns
                .Add("LineNo", GetType(Double))
                .Add("ProductNumber", GetType(String))
                .Add("WareHouse", GetType(String))
                .Add("Quantity", GetType(Decimal))
                .Add("QuantityOnOrder", GetType(String))
                .Add("Description", GetType(String))
                .Add("ErrorCode", GetType(String))
            End With
            Dim iItems As Integer = 0
            ResultDataSet = New DataSet

            Dim loopCounter As Integer = 0, _
                loopAgain As Boolean = True

            Try

                If ParmItems.Count > 0 Then
                    cmdSelect = New iDB2Command(SQLString, conChorus)

                    Do While loopAgain
                        loopAgain = False

                        With cmdSelect
                            '
                            paraminput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 2048)
                            paraminput.Value = Utilities.FixStringLength(Settings.AccountNo3, 20) & _
                                              Utilities.FixStringLength(Settings.AccountNo1, 15) & _
                                              Utilities.FixStringLength(Settings.AccountNo2, 15)

                            Dim startCounter As Integer = 0, _
                              endCounter As Integer = 0

                            If ParmItems.Count > (60 * loopCounter) Then
                                startCounter = (60 * loopCounter)
                                endCounter = ((60 * loopCounter) + 60)
                            Else
                                startCounter = 0
                                endCounter = ParmItems.Count
                            End If

                            If endCounter > ParmItems.Count - 1 Then endCounter = ParmItems.Count - 1

                            For itemCounter As Integer = startCounter To endCounter
                                paraminput.Value += Utilities.FixStringLength(ParmItems(itemCounter), 30)
                                If itemCounter = endCounter AndAlso ParmItems.Count - 1 > endCounter Then
                                    loopAgain = True
                                    loopCounter += 1
                                End If
                            Next

                            paraminput.Direction = ParameterDirection.Input
                            '
                            paramoutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 8192)
                            paramoutput.Value = Param2
                            paramoutput.Direction = ParameterDirection.InputOutput
                            .ExecuteNonQuery()
                            '--------------------------------------------------------------------
                            PARAMOUT = cmdSelect.Parameters(Param2).Value.ToString
                            If PARAMOUT.Substring(8191, 1) = "Y" Then
                                With err
                                    .ErrorMessage = PARAMOUT
                                    .ErrorNumber = PARAMOUT.Substring(8187, 4)
                                    .ErrorStatus = "Error checking stock for item - " & _
                                                Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", err.ErrorNumber)
                                    .HasError = True
                                End With
                            Else

                                Dim iCounter As Integer = 0
                                Dim iPosition As Integer = 0
                                Dim dr As DataRow = Nothing
                                'Max 60 Returned
                                For iCounter = 0 To 59
                                    iPosition = iCounter * 95
                                    If PARAMOUT.Substring(iPosition, 95).Trim > String.Empty Then
                                        dr = Nothing
                                        dr = dt.NewRow()
                                        dr("LineNo") = 0 'PARAMOUT.Substring(iPosition, ??).Trim
                                        dr("ProductNumber") = PARAMOUT.Substring(iPosition, 30)
                                        dr("WareHouse") = PARAMOUT.Substring(iPosition + 30, 20).Trim
                                        If PARAMOUT.Substring(iPosition + 50, 11).Trim <> String.Empty Then
                                            dr("Quantity") = CType(PARAMOUT.Substring(iPosition + 50, 8) & "." & PARAMOUT.Substring(iPosition + 50, 3), Decimal)
                                        Else
                                            dr("Quantity") = 0
                                        End If
                                        dr("QuantityOnOrder") = 0
                                        dr("Description") = PARAMOUT.Substring(iPosition + 61, 30).Trim
                                        dr("ErrorCode") = PARAMOUT.Substring(iPosition + 91, 4).Trim
                                        dt.Rows.Add(dr)
                                    Else
                                        Exit For
                                    End If
                                Next
                                '--------------------------------------------------------------------
                            End If
                        End With

                    Loop



                End If

                cmdSelect.Dispose()
                ResultDataSet.Tables.Add(dt)
                '--------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access "
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8 & "[" & PARAMOUT & "]"
                    .ErrorNumber = "TACDBMAV-09"
                    .HasError = True
                End With
            End Try
        End If
                '--------------------------------------------------------------------
                Return err
    End Function


End Class
