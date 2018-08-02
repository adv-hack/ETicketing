Imports System.Text
Imports IBM.Data.DB2.iSeries
Imports Talent.Common
Imports Talent.Common.Utilities

<Serializable()> _
Public Class DBVanguard
    Inherits DBAccess

#Region "Constants"

    Private Const CLASSNAME As String = "DBVANGUARD"
    Private Const GetVanguardConfigurations As String = "GetVanguardConfigurations"
    Private _cmdAdapter As iDB2DataAdapter = Nothing
    Private _cmd As iDB2Command = Nothing

#End Region

#Region "Public Properties"

    Public Property TDataObjects As New TalentDataObjects()

#End Region

#Region "Protected Functions"

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj
        Select Case _settings.ModuleName
            Case Is = GetVanguardConfigurations : err = AccessDatabaseVG300S()
        End Select
        Return err
    End Function

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj
        Select Case _settings.ModuleName
            Case Is = GetVanguardConfigurations : err = AccessDatabaseSQL2005_VanguardConfiguration()
        End Select
        Return err
    End Function

#End Region

#Region "Private Functions"

    Private Function AccessDatabaseSQL2005_VanguardConfiguration()
        Dim err As New ErrorObj
        Dim dicVanguardConfiguration As New Dictionary(Of String, String)
        Dim dicVanguardPayAccount As New Dictionary(Of String, String)
        Dim dtStatusResults As New DataTable("ErrorStatus")
        Dim dtVanguardConfiguration As New DataTable("VanguardConfiguration")
        Dim dtPayAccountConfigurations As New DataTable("PayAccountConfigurations")
        Dim dRow As DataRow = Nothing
        Try
            ResultDataSet = New DataSet
            ResultDataSet.Tables.Add(dtStatusResults)
            With dtStatusResults.Columns
                .Add("ErrorOccurred", GetType(String))
                .Add("ReturnCode", GetType(String))
            End With
            ResultDataSet.Tables.Add(dtVanguardConfiguration)
            With dtVanguardConfiguration.Columns
                .Add("SystemID", GetType(String))
                .Add("SystemGUID", GetType(String))
                .Add("SystemPasscode", GetType(String))
                .Add("WebserviceURL", GetType(String))
            End With
            ResultDataSet.Tables.Add(dtPayAccountConfigurations)
            With dtPayAccountConfigurations.Columns
                .Add("RASMNum", GetType(String))
                .Add("RASMNumPwd", GetType(String))
                .Add("PWSMNum", GetType(String))
                .Add("PWSMNumPwd", GetType(String))
            End With

            dicVanguardConfiguration = TDataObjects.AppVariableSettings.TblDefaults.GetVanguardConfiguration(_settings.BusinessUnit, _settings.Language)
            dRow = Nothing
            dRow = dtVanguardConfiguration.NewRow
            For Each keyValueItem As KeyValuePair(Of String, String) In dicVanguardConfiguration
                dRow(keyValueItem.Key) = keyValueItem.Value
            Next
            dtVanguardConfiguration.Rows.Add(dRow)
            dicVanguardPayAccount = TDataObjects.AppVariableSettings.TblDefaults.GetVanguardPayAccountConfiguration(_settings.BusinessUnit, _settings.Language)
            dRow = dtPayAccountConfigurations.NewRow
            dRow = Nothing
            dRow = dtPayAccountConfigurations.NewRow
            For Each keyValueItem As KeyValuePair(Of String, String) In dicVanguardPayAccount
                dRow(keyValueItem.Key) = keyValueItem.Value
            Next
            dtPayAccountConfigurations.Rows.Add(dRow)
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBVanguard-SQL2005"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseSQL2005_VanguardConfiguration", strError, err, ex, LogTypeConstants.TCBMVANGUARDERRLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Function AccessDatabaseVG300S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            CallVG300S()
        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBVanguard-VG300S"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseVG300S", strError, err, ex, LogTypeConstants.TCBMVANGUARDERRLOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Sub CallVG300S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL VG300S(@PARAM0,@PARAM1)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pErrorCode.Value = String.Empty
        Dim pSource As iDB2Parameter
        pSource = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pSource.Direction = ParameterDirection.Input
        pSource.Value = String.Empty

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet)

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(Param0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param0).Value).Trim
        ElseIf ResultDataSet.Tables.Count < 3 Then

        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
            If ResultDataSet.Tables.Count > 2 Then
                ResultDataSet.Tables(1).TableName = "VanguardConfiguration"
                ResultDataSet.Tables(2).TableName = "PayAccountConfigurations"
            End If
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

#End Region

End Class
