Imports IBM.Data.DB2.iSeries

<Serializable()> _
Public Class DBTalentDefaults
    Inherits DBAccess


#Region "Class Level Fields"

    Private Const CLASSNAME As String = "DBTALENTDEFAULTS"
    Const RetrieveTalentDefaults As String = "RetrieveTalentDefaults"
    Const RetrieveCountryDefinitions As String = "RetrieveCountryDefinitions"
    Const RetrieveCustomerTitles As String = "RetrieveCustomerTitles"
    Private _cmdAdapter As iDB2DataAdapter = Nothing
    Private _cmd As iDB2Command = Nothing

#End Region

#Region "Protected Functions"

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = RetrieveTalentDefaults : err = AccessDatabaseTD003S()
            Case Is = RetrieveCountryDefinitions : err = AccessDatabaseCW001S()
            Case Is = RetrieveCustomerTitles : err = AccessDatabasetl001S()
        End Select

        Return err
    End Function

#End Region

#Region "Private Fuctions"

    ''' <summary>
    ''' Retrieve basic talent defaults using the stored procedure TD003S
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseTD003S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim dtTalentDefaults As New DataTable("TalentDefaults")
        ResultDataSet.Tables.Add(dtTalentDefaults)

        Try
            CallTD003S()
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBTalentDefaults-TD003S"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseTD003S", strError, err, ex, LogTypeConstants.TCBMTALENTDEFAULTSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Call the Stored Procedure TD003S
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallTD003S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL TD003S(@PARAM0)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pErrorCode.Value = String.Empty

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "TalentDefaults")

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(Param0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param0).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

    Private Function AccessDatabaseCW001S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim dtTalentDefaults As New DataTable("CountryDefinitions")
        ResultDataSet.Tables.Add(dtTalentDefaults)

        Try
            CallCW001S()
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBTalentDefaults-CW001S"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseCW001S", strError, err, ex, LogTypeConstants.TCBMTALENTDEFAULTSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function
    Private Function AccessDatabaseTL001S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim dtTalentDefaults As New DataTable("CustomerTitles")
        ResultDataSet.Tables.Add(dtTalentDefaults)

        Try
            CallTL001S()
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBTalentDefaults-TL001S"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseTL001S", strError, err, ex, LogTypeConstants.TCBMTALENTDEFAULTSLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function
    Private Sub CallCW001S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL CW001S(@PARAM0)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pErrorCode.Value = String.Empty

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "CountryDefinitions")

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(Param0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param0).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub


    Private Sub CallTL001S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL TL001S(@PARAM0,@PARAM1)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pErrorCode.Value = String.Empty

        Dim pPWS As iDB2Parameter
        pPWS = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pPWS.Direction = ParameterDirection.Input
        pPWS.Value = String.Empty
        If Not Settings.IsAgent Then pPWS.Value = "Y"

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "CustomerTitles")

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(Param0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param0).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

#End Region

End Class
