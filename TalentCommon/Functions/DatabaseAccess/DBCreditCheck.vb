Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Credit checks
'
'       Date                        July 2007
'
'       Author                      Ben
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBCCHK- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------

Public Class DBCreditCheck
    Inherits DBAccess

    Private _deCredCheck As DECreditCheck
    Private _dtHeader As New DataTable
    Private _dtItem As New DataTable
    Private _paramTRAN As String

    Public Property deCredCheck() As DECreditCheck
        Get
            Return _deCredCheck
        End Get
        Set(ByVal value As DECreditCheck)
            _deCredCheck = value
        End Set
    End Property

    Public Property dtHeader() As DataTable
        Get
            Return _dtHeader
        End Get
        Set(ByVal value As DataTable)
            _dtHeader = value
        End Set
    End Property

    Public Property dtItem() As DataTable
        Get
            Return _dtItem
        End Get
        Set(ByVal value As DataTable)
            _dtItem = value
        End Set
    End Property

    Public Property ParamTRAN() As String
        Get
            Return _paramTRAN
        End Get
        Set(ByVal value As String)
            _paramTRAN = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then
            Dim PARAMOUT As String = String.Empty
            Dim cmdSelect As iDB2Command = Nothing
            Dim dtrProducts As SqlDataReader = Nothing
            Dim paraminput As iDB2Parameter
            Dim paramoutput As iDB2Parameter
            Dim SQLString As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                      "/CREDCHK(@PARAM1, @PARAM2)"

            CreateResultsTables()
            Try
                '-------------------------------------------------------------
                '   Execute
                '
                cmdSelect = New iDB2Command(SQLString, conSystem21)
                With cmdSelect
                    paraminput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                    paraminput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                        Utilities.FixStringLength(Settings.AccountNo1, 8) & _
                                        Utilities.FixStringLength(Settings.AccountNo2, 3)
                    paraminput.Direction = ParameterDirection.Input
                    paramoutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                    paramoutput.Value = Param2
                    paramoutput.Direction = ParameterDirection.InputOutput
                    .ExecuteNonQuery()
                    '--------------------------------------------------------------------
                    PARAMOUT = cmdSelect.Parameters(Param2).Value.ToString
                    If PARAMOUT.Substring(1023, 1) = "Y" Then
                        With err
                            .ErrorMessage = PARAMOUT
                            .ErrorNumber = PARAMOUT.Substring(1019, 4)
                            .ErrorStatus = "Error checking credit - " & _
                                        Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", err.ErrorNumber)
                            .HasError = True
                        End With
                    Else
                        ' Build the response DataSet
                        Dim CreditStatus As String = PARAMOUT.Substring(0, 1)

                        ResultDataSet = New DataSet
                        Dim newRow As DataRow = dtHeader.NewRow
                        newRow("AccountNumber") = Settings.AccountNo1
                        'newRow("CreditLimit") = 10
                        newRow("CreditStatus") = CreditStatus
                        dtHeader.Rows.Add(newRow)

                        Me.ResultDataSet.Tables.Add(Me.dtHeader)
                    End If
                    .Dispose()
                End With
                '--------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access "
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8 & "[" & PARAMOUT & "]"
                    .ErrorNumber = "TACDBCCHK-08"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------

        Return err
    End Function

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj

        CreateResultsTables()

        Dim newRow As DataRow = dtHeader.NewRow
        newRow("AccountNumber") = deCredCheck.AccountNumber
        'newRow("CreditLimit") = 10
        newRow("CreditStatus") = 2
        dtHeader.Rows.Add(newRow)

        Me.ResultDataSet.Tables.Add(Me.dtHeader)

        Return err
    End Function

    Protected Sub CreateResultsTables()
        Me.ResultDataSet = New DataSet("CreditCheckDataSet")
        Me.dtHeader = New DataTable("CreditCheckHeader")
        Me.dtItem = New DataTable("CreditCheckDetail")

        With dtHeader.Columns
            .Add(New DataColumn("AccountNumber", GetType(String)))
            .Add(New DataColumn("CreditStatus", GetType(Integer)))
        End With

    End Sub

    Protected Overrides Function AccessDataBaseChorus() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------
        If Not err.HasError Then
            Dim PARAMOUT As String = String.Empty
            Dim cmdSelect As iDB2Command = Nothing
            Dim dtrProducts As SqlDataReader = Nothing
            Dim paraminput As iDB2Parameter
            Dim paramoutput As iDB2Parameter
            Dim SQLString As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                      "/CREDCHK(@PARAM1, @PARAM2)"

            CreateResultsTables()
            Try
                '-------------------------------------------------------------
                '   Execute
                '
                cmdSelect = New iDB2Command(SQLString, conChorus)
                With cmdSelect
                    paraminput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                    paraminput.Value = Utilities.FixStringLength(Settings.AccountNo1, 6) & _
                                        Utilities.FixStringLength(Settings.AccountNo2, 3) & _
                                        Utilities.PadLeadingZeros(deCredCheck.TotalOrderValue.ToString("0.00").Replace(".", ""), 11)
                    paraminput.Direction = ParameterDirection.Input
                    paramoutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                    paramoutput.Value = Param2
                    paramoutput.Direction = ParameterDirection.InputOutput
                    .ExecuteNonQuery()
                    '--------------------------------------------------------------------
                    PARAMOUT = cmdSelect.Parameters(Param2).Value.ToString
                    If PARAMOUT.Substring(1023, 1) = "Y" Then
                        With err
                            .ErrorMessage = PARAMOUT
                            .ErrorNumber = PARAMOUT.Substring(1019, 4)
                            .ErrorStatus = "Error checking credit - " & _
                                        Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", err.ErrorNumber)
                            .HasError = True
                        End With
                    Else
                        ' Build the response DataSet
                        Dim CreditStatus As String = PARAMOUT.Substring(11, 1)

                        ResultDataSet = New DataSet
                        Dim newRow As DataRow = dtHeader.NewRow
                        newRow("AccountNumber") = Settings.AccountNo1
                        'newRow("CreditLimit") = 10
                        newRow("CreditStatus") = CreditStatus
                        dtHeader.Rows.Add(newRow)

                        Me.ResultDataSet.Tables.Add(Me.dtHeader)
                    End If
                    .Dispose()
                End With
                '--------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access "
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8 & "[" & PARAMOUT & "]"
                    .ErrorNumber = "TACDBCCHK-09"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------


        Return err
    End Function
End Class
