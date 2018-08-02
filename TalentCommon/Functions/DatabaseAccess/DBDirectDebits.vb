Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text

<Serializable()> _
Public Class DBDirectDebits
    Inherits DBAccess

    Public Property DirectDebitDetails As New DEDirectDebitDetails
    Public Property Company As String
    Public Property Customer As String
    Public Property conTalent As iDB2Connection

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj
        Select Case Settings.ModuleName
            Case Is = "RetrieveTalentDirectDebitDetails" : err = DBRetrieveTalentDirectDebitDetails()
            Case Is = "RetrieveTalentDirectDebitBalances" : err = DBRetrieveTalentDirectDebitBalances()
            Case Is = "RetrieveTalentDirectDebitOnAccount" : err = DBRetrieveTalentDirectDebitOnAccount()
            Case Is = "UpdateTalentDirectDebitDetails" : err = DBUpdateTalentDirectDebitDetails()
            Case Is = "ChangeTalentDirectDebitOnAccountStatus" : err = DBChangeTalentDirectDebitOnAccountStatus()
            Case Is = "DeleteTalentDirectDebitOnAccount" : err = DBDeleteTalentDirectDebitOnAccount()
        End Select
        Return err
    End Function

    Protected Function DBRetrieveTalentDirectDebitDetails() As ErrorObj
        Dim err As New ErrorObj
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtr As iDB2DataReader = Nothing
        Try
            Dim sqlselect As String = "select dnam20, sort20, acno20, SP0320 from cd020l1 where cono20 = @COMPANY and memb20 = @MEMBER"
            cmdSelect = New iDB2Command(sqlselect, conTALENTTKT)
            cmdSelect.Parameters.Add("@COMPANY", iDB2DbType.iDB2VarChar, 3).Value = Me.Company
            cmdSelect.Parameters.Add("@MEMBER", iDB2DbType.iDB2VarChar, 12).Value = Me.Customer
            dtr = cmdSelect.ExecuteReader

            While dtr.Read
                DirectDebitDetails.AccountName = dtr.Item("dnam20").ToString().Trim()
                DirectDebitDetails.SortCode1 = dtr.Item("sort20").ToString().Substring(0, 2).Trim()
                DirectDebitDetails.SortCode2 = dtr.Item("sort20").ToString().Substring(2, 2).Trim()
                DirectDebitDetails.SortCode3 = dtr.Item("sort20").ToString().Substring(4, 2).Trim()
                DirectDebitDetails.AccountNumber = dtr.Item("acno20").ToString().Trim()
                DirectDebitDetails.DDTreasurer = Utilities.convertToBool(dtr.Item("SP0320").ToString().Trim())
            End While

        Catch ex As Exception
            Const strError As String = "Error Retrieving Direct Debit Details"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBRtvDDDetails-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Protected Function DBRetrieveTalentDirectDebitBalances() As ErrorObj
        Dim err As New ErrorObj
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtr As iDB2DataReader = Nothing
        Dim AgentCompany As String = Me.company
        Dim dtDirectDebitBalances As New DataTable
        Dim sqlSelect As String
        ResultDataSet = New DataSet
        dtDirectDebitBalances.TableName = "DirectDebitBalances"
        dtDirectDebitBalances.Columns.Add("Reference", GetType(Decimal))
        dtDirectDebitBalances.Columns.Add("Batch", GetType(Decimal))
        dtDirectDebitBalances.Columns.Add("DateCreated", GetType(Decimal))
        dtDirectDebitBalances.Columns.Add("Balance", GetType(Decimal))
        dtDirectDebitBalances.Columns.Add("User", GetType(String))

        Try
            sqlSelect = "SELECT PYRF15,BREF15,TXDT15,PYMT15,USER15 FROM py015 WHERE CONO15 = @COMAPNY and ACTR15 = 'P' and MEMB15 = @MEMBER AND SRCE15 = ' '   ORDER BY PYRF15 DESC"
            cmdSelect = New iDB2Command(sqlSelect, conTALENTTKT)
            cmdSelect.Parameters.Add("@COMPANY", iDB2DbType.iDB2VarChar, 3).Value = Me.Company
            cmdSelect.Parameters.Add("@MEMBER", iDB2DbType.iDB2VarChar, 12).Value = Me.Customer

            dtr = cmdSelect.ExecuteReader
            While dtr.Read
                Dim dRow As DataRow = Nothing
                dRow = dtDirectDebitBalances.NewRow
                dRow.Item("Reference") = dtr.Item("PYRF15").ToString().Trim()
                dRow.Item("Batch") = dtr.Item("BREF15").ToString().Trim()
                dRow.Item("DateCreated") = dtr.Item("TXDT15").ToString().Trim()
                dRow.Item("Balance") = dtr.Item("PYMT15").ToString().Trim()
                dRow.Item("User") = dtr.Item("USER15").ToString().Trim()
                dtDirectDebitBalances.Rows.Add(dRow)
            End While
            ResultDataSet.Tables.Add(dtDirectDebitBalances)

        Catch ex As Exception
            Const strError As String = "Error Retrieving Direct Debit Balances"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBRtrDDBalances-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Protected Function DBRetrieveTalentDirectDebitOnAccount() As ErrorObj
        Dim err As New ErrorObj
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtr As iDB2DataReader = Nothing
        Dim AgentCompany As String = Me.Company
        Dim dtDirectDebitOnAccount As New DataTable
        ResultDataSet = New DataSet
        dtDirectDebitOnAccount.TableName = "DirectDebitOnAccount"
        dtDirectDebitOnAccount.Columns.Add("Date", GetType(String))
        dtDirectDebitOnAccount.Columns.Add("Reference", GetType(Decimal))
        dtDirectDebitOnAccount.Columns.Add("Product", GetType(String))
        dtDirectDebitOnAccount.Columns.Add("Description", GetType(String))
        dtDirectDebitOnAccount.Columns.Add("Value", GetType(Decimal))
        dtDirectDebitOnAccount.Columns.Add("Payref", GetType(Decimal))
        dtDirectDebitOnAccount.Columns.Add("Status", GetType(String))

        Try
            Dim strSelect As New StringBuilder
            If DirectDebitDetails.showPaidOnAcountRecords Then
                strSelect.Append("SELECT date11, idiD11, MTCD11, COMM20, valu11, PYRF11, ACTR11 FROM py011l5 left outer join MD020L6 on digits(IDID11) = TRRF20")
                strSelect.Append(" WHERE CONO11 = CONO20 and MTCD11 = mTCD20  AND MEMB11 = @MEMBER AND CONO11 = @COMPANY   ORDER BY UPDT11 desc, pyid11")
            Else
                strSelect.Append("SELECT date11, idiD11, MTCD11, COMM20, valu11, PYRF11, ACTR11 FROM py011l5 left outer join MD020L6 on digits(IDID11) = TRRF20")
                strSelect.Append(" WHERE CONO11 = CONO20 and MTCD11 = mTCD20  AND MEMB11 = @MEMBER AND CONO11 = @COMPANY and PYRF11 = 0  ORDER BY UPDT11 desc, pyid11")
            End If
            Dim sqlselect As String = strSelect.ToString
            cmdSelect = New iDB2Command(sqlselect, conTALENTTKT)
            cmdSelect.Parameters.Add("@COMPANY", iDB2DbType.iDB2VarChar, 3).Value = Me.Company
            cmdSelect.Parameters.Add("@MEMBER", iDB2DbType.iDB2VarChar, 12).Value = Me.Customer

            dtr = cmdSelect.ExecuteReader

            While dtr.Read
                Dim dRow As DataRow = Nothing
                dRow = dtDirectDebitOnAccount.NewRow
                dRow("Date") = dtr.Item("date11").ToString().Trim()
                dRow.Item("Reference") = dtr.Item("IDID11").ToString().Trim()
                dRow.Item("Product") = dtr.Item("MTCD11").ToString().Trim()
                dRow.Item("Description") = dtr.Item("COMM20").ToString().Trim()
                dRow.Item("Value") = dtr.Item("valu11").ToString().Trim()
                dRow.Item("Payref") = dtr.Item("PYRF11").ToString().Trim()
                dRow.Item("Status") = dtr.Item("ACTR11").ToString().Trim()
                dtDirectDebitOnAccount.Rows.Add(dRow)
            End While
            ResultDataSet.Tables.Add(dtDirectDebitOnAccount)
        Catch ex As Exception
            Const strError As String = "Error Retrieving Direct Debit On Account"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBRtrDDOnAccount-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Protected Function DBUpdateTalentDirectDebitDetails() As ErrorObj
        Dim err As New ErrorObj
        Try
            Dim cmdSELECT As iDB2Command = Nothing
            ResultDataSet = New DataSet
            Dim sLastRecord As String = "000"
            Dim sRecordTotal As String = "000"
            Dim bMoreRecords As Boolean = True
            Dim bError As Boolean = False
            Dim dRow As DataRow = Nothing
            Dim strProgram As String = "WS205R"
            Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1, @PARAM2)"
            Dim parmIO1 As iDB2Parameter
            Dim parmIO2 As iDB2Parameter
            Dim PARAMOUT1 As String = String.Empty
            Dim PARAMOUT2 As String = String.Empty

            'Set the connection string
            cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

            'Populate the parameters
            parmIO1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
            parmIO1.Value = buildWS205Parm("U")
            parmIO1.Direction = ParameterDirection.InputOutput
            parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5012)
            parmIO2.Value = String.Empty
            parmIO2.Direction = ParameterDirection.InputOutput

            cmdSELECT.ExecuteNonQuery()
            PARAMOUT1 = cmdSELECT.Parameters(Param1).Value.ToString
            PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString

            'Create the Status data table
            Dim DtStatusResults As New DataTable
            ResultDataSet.Tables.Add(DtStatusResults)
            With DtStatusResults.Columns
                .Add("ErrorOccurred", GetType(String))
                .Add("ReturnCode", GetType(String))
            End With
        Catch ex As Exception
            Dim strError As String = "Error Updating Direct Debit Details for member " & DirectDebitDetails.CustomerNumber
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBUpdateTalentDDDetails-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Protected Function DBChangeTalentDirectDebitOnAccountStatus() As ErrorObj
        Dim err As New ErrorObj
        Try
            Dim cmdSELECT As iDB2Command = Nothing
            ResultDataSet = New DataSet
            Dim bError As Boolean = False
            Dim dRow As DataRow = Nothing
            Dim strProgram As String = "WS205R"
            Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1, @PARAM2)"
            Dim parmIO1 As iDB2Parameter
            Dim parmIO2 As iDB2Parameter
            Dim PARAMOUT1 As String = String.Empty
            Dim PARAMOUT2 As String = String.Empty

            'Set the connection string
            cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

            'Populate the parameters
            parmIO1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
            parmIO1.Value = buildWS205Parm("S")
            parmIO1.Direction = ParameterDirection.InputOutput
            parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5012)
            parmIO2.Value = String.Empty
            parmIO2.Direction = ParameterDirection.InputOutput

            cmdSELECT.ExecuteNonQuery()
            PARAMOUT1 = cmdSELECT.Parameters(Param1).Value.ToString
            PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString

            'Create the Status data table
            Dim DtStatusResults As New DataTable("Status")
            ResultDataSet.Tables.Add(DtStatusResults)
            With DtStatusResults.Columns
                .Add("ErrorOccurred", GetType(String))
                .Add("ReturnCode", GetType(String))
            End With

            ' If any errors populate the status table 
            PARAMOUT1 = cmdSELECT.Parameters(Param1).Value.ToString
            dRow = DtStatusResults.NewRow
            If PARAMOUT1.Substring(1023, 1) = "E" Or PARAMOUT1.Substring(1021, 2).Trim <> String.Empty Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT1.Substring(1021, 2)
                bError = True
                DtStatusResults.Rows.Add(dRow)
            End If
        Catch ex As Exception
            Dim strError As String = "Error Updating Direct Debit On Account Status for member " & DirectDebitDetails.CustomerNumber
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBUpdateTalentDDOnAccount-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Protected Function DBDeleteTalentDirectDebitOnAccount() As ErrorObj
        Dim err As New ErrorObj
        Try
            Dim cmdSELECT As iDB2Command = Nothing
            ResultDataSet = New DataSet
            Dim bError As Boolean = False
            Dim dRow As DataRow = Nothing
            Dim strProgram As String = "WS205R"
            Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1, @PARAM2)"
            Dim parmIO1 As iDB2Parameter
            Dim parmIO2 As iDB2Parameter
            Dim PARAMOUT1 As String = String.Empty
            Dim PARAMOUT2 As String = String.Empty

            'Set the connection string
            cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

            'Populate the parameters
            parmIO1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
            parmIO1.Value = buildWS205Parm("D")
            parmIO1.Direction = ParameterDirection.InputOutput
            parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5012)
            parmIO2.Value = String.Empty
            parmIO2.Direction = ParameterDirection.InputOutput

            cmdSELECT.ExecuteNonQuery()
            PARAMOUT1 = cmdSELECT.Parameters(Param1).Value.ToString
            PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString

            'Create the Status data table
            Dim DtStatusResults As New DataTable("Status")
            ResultDataSet.Tables.Add(DtStatusResults)
            With DtStatusResults.Columns
                .Add("ErrorOccurred", GetType(String))
                .Add("ReturnCode", GetType(String))
            End With

            ' If any errors populate the status table 
            PARAMOUT1 = cmdSELECT.Parameters(Param1).Value.ToString
            dRow = DtStatusResults.NewRow
            If PARAMOUT1.Substring(1023, 1) = "E" Or PARAMOUT1.Substring(1021, 2).Trim <> String.Empty Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT1.Substring(1021, 2)
                bError = True
                DtStatusResults.Rows.Add(dRow)
            End If

        Catch ex As Exception
            Dim strError As String = "Error deleting Direct Debit On Account record for member " & DirectDebitDetails.CustomerNumber
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBDeleteTalentDDOnAccount-01"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function buildWS205Parm(ByVal mode As String) As String
        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(DirectDebitDetails.CustomerNumber, 12))
        myString.Append(Utilities.FixStringLength(mode, 1))
        myString.Append(Utilities.FixStringLength(DirectDebitDetails.AccountName, 40))
        myString.Append(Utilities.FixStringLength(DirectDebitDetails.AccountNumber, 8))
        myString.Append(Utilities.FixStringLength(DirectDebitDetails.SortCode1, 2))
        myString.Append(Utilities.FixStringLength(DirectDebitDetails.SortCode2, 2))
        myString.Append(Utilities.FixStringLength(DirectDebitDetails.SortCode3, 2))
        myString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(DirectDebitDetails.DDTreasurer), 1))
        myString.Append(Utilities.FixStringLength(DirectDebitDetails.Agent, 10))
        myString.Append(Utilities.PadLeadingZeros(DirectDebitDetails.OnAccountRef, 13))
        myString.Append(Utilities.FixStringLength("", 933))

        Return myString.ToString()
    End Function
End Class
