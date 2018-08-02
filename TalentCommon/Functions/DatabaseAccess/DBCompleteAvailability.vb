Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Text
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Complete stock enquires
'
'       Date                        June 2007
'
'       Author                      Ben
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBCAV- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBCompleteAvailability
    Inherits DBAccess

    Private _dep As DEProductAlert
    Private _depa As DePNA
    Private _dePNARequest As DEPNARequest
    Private _usage As Int16 = 0
    Private _parmITEM(500) As String

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
    Public Property DePNARequest() As DEPNARequest
        Get
            Return _dePNARequest
        End Get
        Set(ByVal value As DEPNARequest)
            _dePNARequest = value
        End Set
    End Property

    Protected Overrides Function ValidateAgainstDatabaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        '   PARAM1  = (company no (4-int) & Old key )
        '   PARAM2  = (Line no (4-int) & product (30-text) & Warehouse(10-text) & Quantity(4 - number) &
        '              line error (4-string) & SPARE (up to 100 ) by 80 thus 6000 char long (allows for multiple 
        '             warehouse for each item)
        '

        '
        If Not err.HasError Then
            Dim PARAMOUT As String = String.Empty
            Dim cmdSelect As iDB2Command = Nothing
            Dim dtrProducts As SqlDataReader = Nothing
            Dim paraminput As iDB2Parameter
            Dim paramoutput As iDB2Parameter
            Dim SQLString As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                      "/ALLSTKENQ(@PARAM1, @PARAM2) "
            Dim moreProducts As Boolean = True
            '
            Dim dt As New DataTable
            With dt.Columns
                .Add("LineNo", GetType(Double))
                .Add("ProductNumber", GetType(String))
                .Add("WareHouse", GetType(String))
                .Add("Quantity", GetType(Double))
                .Add("ErrorCode", GetType(String))
            End With
            Dim iItems As Integer = 0
            ResultDataSet = New DataSet
            Dim TXstore As String = "0"
            Dim lineCount As Integer = 1
            Dim warehouses(5) As String
            Dim warehouseString As String = String.Empty
            '-------------------------------------
            ' If multiple warehouses then unpack..
            '-------------------------------------
            Dim warehouseCount As Integer = 1
            For Each alert As DEAlerts In Dep.CollDEAlerts
                If alert.BranchID <> String.Empty Then
                    warehouses(warehouseCount) = alert.BranchID
                End If
                warehouseCount += 1
                If warehouseCount > 5 Then
                    Exit For
                End If
            Next

            warehouseString = Utilities.FixStringLength(warehouses(1), 10) & _
                                          Utilities.FixStringLength(warehouses(2), 10) & _
                                          Utilities.FixStringLength(warehouses(3), 10) & _
                                          Utilities.FixStringLength(warehouses(4), 10) & _
                                          Utilities.FixStringLength(warehouses(5), 10)
            '
            Try
                While moreProducts
                 
                    '------------------------------------------------------------------------
                    '   Execute
                    '
                    cmdSelect = New iDB2Command(SQLString, conSystem21)
                    With cmdSelect
                        '
                        paraminput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                        paraminput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                          warehouseString & _
                                        Utilities.FixStringLength(TXstore, 33)

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
                                moreProducts = False
                            End With
                        Else
                            '--------------------------------------------------------------------
                            ' Build the response DataSet
                            '
                            '--------------------------------------------------------------------
                            '          1         2         3         4         5         6
                            '0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890 
                            'nnnnDI813953                      PK        0045D001nnnnDI816301
                            '123456789                     123456789 1234
                            '--------------------------------------------------------------------
                            '
                            Dim iCounter As Integer
                            Dim iPosition As Integer = 0
                            Dim sWork As String = PARAMOUT.Substring(0, 8000)
                            Dim dr As DataRow = Nothing
                            Dim strQty As String = String.Empty

                            For iCounter = 0 To 99
                                iPosition = iCounter * 80
                                If sWork.Substring(iPosition, 30).Trim > String.Empty Then
                                    dr = Nothing
                                    dr = dt.NewRow()
                                    dr("LineNo") = lineCount
                                    dr("ProductNumber") = sWork.Substring(iPosition, 30)
                                    dr("Warehouse") = sWork.Substring(iPosition + 30, 10)
                                    If sWork.Substring(iPosition + 40, 13).Trim <> String.Empty Then
                                        ' Convert QTY (has 3 dec places)
                                        strQty = sWork.Substring(iPosition + 40, 13)
                                        strQty = strQty.Insert(10, ".")
                                        dr("Quantity") = CDec(strQty)
                                    Else
                                        dr("Quantity") = 0
                                    End If
                                    dr("ErrorCode") = String.Empty

                                    dt.Rows.Add(dr)
                                    lineCount += 1
                                Else
                                    Exit For
                                End If
                            Next
                            ' Check EOF flag
                            '  If PARAMOUT.Substring(8000, 1) <> "1" Then
                            If PARAMOUT.Substring(8153, 1) = "1" Then
                                moreProducts = False
                            Else
                                ' Set pointer
                                TXstore = PARAMOUT.Substring(8154, 33)
                            End If
                            ' 
                            '--------------------------------------------------------------------
                        End If
                        .Dispose()
                    End With
                End While
                ResultDataSet.Tables.Add(dt)
                '--------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access "
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8 & "[" & PARAMOUT & "]"
                    .ErrorNumber = "TACDBCAV-08"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------
        Return err
    End Function
End Class
