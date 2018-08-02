Imports IBM.Data.DB2.iSeries

<Serializable()> _
Public Class DBActivities
    Inherits DBAccess

#Region "Constants"

    Private Const CustomerActivitiesSearch As String = "CustomerActivitiesSearch"
    Private Const CustomerActivitiesRetrieval As String = "CustomerActivitiesRetrieval"
    Private Const ActivitiesQNARetrieval As String = "ActivitiesQNARetrieval"
    Private Const DeleteCustomerActivity As String = "DeleteCustomerActivity"
    Private Const AddCustomerActivity As String = "AddCustomerActivity"
    Private Const UpdateCustomerActivity As String = "UpdateCustomerActivity"
    Private Const CreateActivityComment As String = "CreateActivityComment"
    Private Const UpdateActivityComment As String = "UpdateActivityComment"
    Private Const DeleteActivityComment As String = "DeleteActivityComment"
    Private Const CreateActivityFileAttachment As String = "CreateActivityFileAttachment"
    Private Const DeleteActivityFileAttachment As String = "DeleteActivityFileAttachment"

#End Region

#Region "Class Level Fields"

    Private _cmdAdapter As iDB2DataAdapter = Nothing
    Private _cmd As iDB2Command = Nothing

#End Region

#Region "Public Properties"

    Public Property ActivitiesDataEntity As New DEActivities

#End Region

#Region "Protected Functions"

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = CustomerActivitiesSearch : err = AccessDatabaseCD040S1()

            Case Is = CustomerActivitiesRetrieval : err = AccessDatabaseCD040S()
            Case Is = DeleteCustomerActivity : err = AccessDatabaseCD040S()
            Case Is = AddCustomerActivity : err = AccessDatabaseCD040S()
            Case Is = UpdateCustomerActivity : err = AccessDatabaseCD040S()

            Case Is = ActivitiesQNARetrieval : err = AccessDatabaseQA002S()

            Case Is = CreateActivityComment : err = AccessDatabaseCD041S()
            Case Is = UpdateActivityComment : err = AccessDatabaseCD041S()
            Case Is = DeleteActivityComment : err = AccessDatabaseCD041S()

            Case Is = CreateActivityFileAttachment : err = AccessDatabaseCD042S()
            Case Is = DeleteActivityFileAttachment : err = AccessDatabaseCD042S()
        End Select

        Return err
    End Function

#End Region

#Region "Private Fuctions"

    ''' <summary>
    ''' Call Customer Activities header stored procedure with a given mode for read, delete, create and update and return a single customer activities header record
    ''' </summary>
    ''' <returns>error object with any issues</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseCD040S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("RecordsReturned", GetType(Integer))
        End With

        Try
            CallCD040S()
            If dtStatusResults.Rows.Count > 0 AndAlso dtStatusResults.Rows(0).Item(0).Equals("E") Then
                With err
                    .ErrorNumber = "TACDBActivities-CD040S"
                    .HasError = True
                End With
            End If
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBActivities-CD040S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build parameters and call stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallCD040S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL CD040S(@Error, @Source, @PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6, @PARAM7, @PARAM8, @PARAM9, @PARAM10, @PARAM11, @PARAM12, @PARAM13, @PARAM14, @PARAM15, @PARAM16, @PARAM17, @PARAM18, @PARAM19, @PARAM20)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pMode As New iDB2Parameter
        Dim pCustomerActivitiesHeaderID As iDB2Parameter
        Dim pCustomerNumber As iDB2Parameter
        Dim pTemplateID As iDB2Parameter
        Dim pSelectedTALENTUser As iDB2Parameter
        Dim pDate As iDB2Parameter
        Dim pSubject As iDB2Parameter
        Dim pStatus As iDB2Parameter
        Dim pQuestionID As iDB2Parameter
        Dim pQuestionText As iDB2Parameter
        Dim pAnswerText As iDB2Parameter
        Dim pRememberedAnswer As iDB2Parameter
        Dim pDelimeter As IDbDataParameter
        Dim pCommentID As IDbDataParameter
        Dim pCommentText As IDbDataParameter
        Dim pCommentEdited As IDbDataParameter
        Dim pFileAttachmentID As IDbDataParameter
        Dim pFileName As IDbDataParameter
        Dim pFileDescription As IDbDataParameter
        Dim pCurrentAgent As IDbDataParameter
        Dim pCAllId As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pSource = _cmd.Parameters.Add(Source, iDB2DbType.iDB2Char, 1)
        pMode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 1)
        pCustomerActivitiesHeaderID = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Decimal, 13)
        pCustomerNumber = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 12)
        pTemplateID = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Decimal, 13)
        pSelectedTALENTUser = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 10)
        pDate = _cmd.Parameters.Add(Param5, iDB2DbType.iDB2Decimal, 8)
        pSubject = _cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 60)
        pStatus = _cmd.Parameters.Add(Param7, iDB2DbType.iDB2Char, 50)
        pQuestionID = _cmd.Parameters.Add(Param8, iDB2DbType.iDB2VarChar, 1000)
        pQuestionText = _cmd.Parameters.Add(Param9, iDB2DbType.iDB2VarChar, 1000)
        pAnswerText = _cmd.Parameters.Add(Param10, iDB2DbType.iDB2VarChar, 1000)
        pRememberedAnswer = _cmd.Parameters.Add(Param11, iDB2DbType.iDB2Char, 1)
        pDelimeter = _cmd.Parameters.Add(Param12, iDB2DbType.iDB2Char, 1)
        pCommentID = _cmd.Parameters.Add(Param13, iDB2DbType.iDB2Decimal, 13)
        pCommentText = _cmd.Parameters.Add(Param14, iDB2DbType.iDB2VarChar, 32000)
        pCommentEdited = _cmd.Parameters.Add(Param15, iDB2DbType.iDB2Char, 1)
        pCommentEdited.Direction = ParameterDirection.Output
        pFileAttachmentID = _cmd.Parameters.Add(Param16, iDB2DbType.iDB2Decimal, 13)
        pFileName = _cmd.Parameters.Add(Param17, iDB2DbType.iDB2VarChar, 300)
        pFileDescription = _cmd.Parameters.Add(Param18, iDB2DbType.iDB2VarChar, 32000)
        pCurrentAgent = _cmd.Parameters.Add(Param19, iDB2DbType.iDB2Char, 10)
        pCallId = _cmd.Parameters.Add(Param20, iDB2DbType.iDB2Char, 1)

        pErrorCode.Value = String.Empty
        pSource.Value = ActivitiesDataEntity.Source
        pMode.Value = ActivitiesDataEntity.CustomerActivitiesHeaderMode
        pCustomerActivitiesHeaderID.Value = ActivitiesDataEntity.CustomerActivitiesHeaderID
        pCustomerNumber.Value = ActivitiesDataEntity.CustomerNumber
        pTemplateID.Value = ActivitiesDataEntity.TemplateID
        pSelectedTALENTUser.Value = ActivitiesDataEntity.ActivityUserName
        pDate.Value = ActivitiesDataEntity.ActivityDate
        pSubject.Value = ActivitiesDataEntity.ActivitySubject
        pStatus.Value = ActivitiesDataEntity.ActivityStatus
        pQuestionID.Value = ActivitiesDataEntity.ActivityQuestionIDArray
        pQuestionText.Value = ActivitiesDataEntity.ActivityQuestionTextArray
        pAnswerText.Value = ActivitiesDataEntity.ActivityAnswerTextArray
        pRememberedAnswer.Value = "N"
        pDelimeter.Value = ActivitiesDataEntity.ActivityTextDelimiter
        pCommentID.Value = ActivitiesDataEntity.ActivityCommentID
        pCommentText.Value = ActivitiesDataEntity.ActivityCommentText
        pFileAttachmentID.Value = ActivitiesDataEntity.ActivityFileAttachmentID
        pFileName.Value = ActivitiesDataEntity.ActivityFileName
        pFileDescription.Value = ActivitiesDataEntity.ActivityFileDescription
        If ActivitiesDataEntity.ActivityUserName IsNot Nothing AndAlso ActivitiesDataEntity.ActivityUserName.Equals(GlobalConstants.BACKEND_PWS_AGENT) Then
            pCurrentAgent.Value = GlobalConstants.BACKEND_PWS_AGENT
        Else
            pCurrentAgent.Value = _settings.AgentEntity.AgentUsername
        End If
        If Not ActivitiesDataEntity.ActivityCallId = 0 Then
            pCAllId.Value = "Y"
        Else
            pCAllId.Value = "N"
        End If
        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet)
        Utilities.ConvertISeriesTables(ResultDataSet)
        If ResultDataSet.Tables.Count = 4 Then
            ResultDataSet.Tables(1).TableName = "CustomerActivitiesHeader"
            ResultDataSet.Tables(2).TableName = "CustomerActivitiesComments"
            ResultDataSet.Tables(3).TableName = "CustomerActivitiesFileAttachments"
        End If

        Dim drStatus As DataRow = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).NewRow
        If CStr(_cmd.Parameters(ErrorCode).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(ErrorCode).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
            If pMode.Value = "R" Then
                If ResultDataSet.Tables("CustomerActivitiesComments").Rows(0)("ErrorCode").ToString().Trim().Length > 0 Then
                    drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
                    drStatus("ReturnCode") = ResultDataSet.Tables("CustomerActivitiesComments").Rows(0)("ErrorCode")
                End If
                ResultDataSet.Tables("CustomerActivitiesComments").Rows(0).Delete()
                ResultDataSet.Tables("CustomerActivitiesComments").AcceptChanges()
                If ResultDataSet.Tables("CustomerActivitiesFileAttachments").Rows(0)("ErrorCode").ToString().Trim().Length > 0 Then
                    drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
                    drStatus("ReturnCode") = ResultDataSet.Tables("CustomerActivitiesFileAttachments").Rows(0)("ErrorCode")
                End If
                ResultDataSet.Tables("CustomerActivitiesFileAttachments").Rows(0).Delete()
                ResultDataSet.Tables("CustomerActivitiesFileAttachments").AcceptChanges()
            End If
        End If
        ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Add(drStatus)
    End Sub

    ''' <summary>
    ''' Call Customer Activities header stored procedure for searching only and return list of customer activities header records
    ''' </summary>
    ''' <returns>error object with any issues</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseCD040S1() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("RecordsReturned", GetType(Integer))
        End With

        'Create the Customer Activities data table
        Dim dtCustomerActivitiesHeader As New DataTable("CustomerActivitiesHeader")
        With dtCustomerActivitiesHeader.Columns
            .Add("CustomerActivitiesHeaderID", GetType(Integer))
            .Add("CustomerNumber", GetType(String))
            .Add("TemplateID", GetType(Integer))
            .Add("ActivityUserName", GetType(String))
            .Add("ActivityDate", GetType(String))
            .Add("ActivitySubject", GetType(String))
            .Add("ActivityStatus", GetType(String))
            .Add("CustomerName", GetType(String))
            .Add("FormattedDate", GetType(String))
            .Add("DescriptiveUserName", GetType(String))
        End With
        ResultDataSet.Tables.Add(dtCustomerActivitiesHeader)

        Try
            CallCD040S1()
            getFormattedDate(dtCustomerActivitiesHeader)
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBActivities-CD040S1"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build parameters and call stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallCD040S1()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL CD040S1(@Error, @Source, @PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6, @PARAM7, @PARAM8, @PARAM9, @RecordCount)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pCustomerNumber As iDB2Parameter
        Dim pTemplateID As iDB2Parameter
        Dim pSelectedTALENTUser As iDB2Parameter
        Dim pDate As iDB2Parameter
        Dim pSubject As iDB2Parameter
        Dim pStatus As iDB2Parameter
        Dim pOrderBy As IDbDataParameter
        Dim pPageSize As IDbDataParameter
        Dim pStartNumber As IDbDataParameter
        Dim pDraw As IDbDataParameter
        Dim pRecordCount As IDbDataParameter

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pSource = _cmd.Parameters.Add(Source, iDB2DbType.iDB2Char, 1)
        pCustomerNumber = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 12)
        pTemplateID = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Decimal, 13)
        pSelectedTALENTUser = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10)
        pDate = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Decimal, 8)
        pSubject = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 60)
        pStatus = _cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 50)
        pOrderBy = _cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 30)
        pPageSize = _cmd.Parameters.Add(Param7, iDB2DbType.iDB2Decimal, 8)
        pStartNumber = _cmd.Parameters.Add(Param8, iDB2DbType.iDB2Decimal, 8)
        pDraw = _cmd.Parameters.Add(Param9, iDB2DbType.iDB2Decimal, 3)
        pRecordCount = _cmd.Parameters.Add(RecordCount, iDB2DbType.iDB2Decimal, 8)
        pRecordCount.Direction = ParameterDirection.InputOutput

        pErrorCode.Value = String.Empty
        pSource.Value = ActivitiesDataEntity.Source
        pCustomerNumber.Value = ActivitiesDataEntity.CustomerNumber
        pTemplateID.Value = ActivitiesDataEntity.TemplateID
        pSelectedTALENTUser.Value = ActivitiesDataEntity.ActivityUserName
        If ActivitiesDataEntity.ActivityDate IsNot Nothing Then pDate.Value = Utilities.DateToIseries8Format(CDate(ActivitiesDataEntity.ActivityDate))
        pSubject.Value = ActivitiesDataEntity.ActivitySubject
        pStatus.Value = ActivitiesDataEntity.ActivityStatus
        pOrderBy.Value = ActivitiesDataEntity.SortOrder
        pPageSize.Value = ActivitiesDataEntity.Length
        pStartNumber.Value = ActivitiesDataEntity.Start
        pDraw.Value = ActivitiesDataEntity.Draw
        pRecordCount.Value = 0

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "CustomerActivitiesHeader")

        Dim drStatus As DataRow = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).NewRow
        If CStr(_cmd.Parameters(ErrorCode).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(ErrorCode).Value).Trim
            drStatus("RecordsReturned") = CInt(_cmd.Parameters(RecordCount).Value)
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
            drStatus("RecordsReturned") = CInt(_cmd.Parameters(RecordCount).Value)
        End If
        ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Add(drStatus)
    End Sub

    ''' <summary>
    ''' Format the iSeries 8 char date YYYYMMDD to a date object and then to a string based on the global date format
    ''' </summary>
    ''' <param name="dtCustomerActivitiesHeader">The customer activities data table</param>
    ''' <remarks></remarks>
    Private Sub getFormattedDate(ByRef dtCustomerActivitiesHeader As DataTable)
        Dim activityDate As Date = Nothing
        Dim formattedDate As String = String.Empty
        For Each row As DataRow In dtCustomerActivitiesHeader.Rows
            activityDate = Utilities.ISeries8CharacterDate(row("ActivityDate"))
            formattedDate = activityDate.ToString(Settings.EcommerceModuleDefaultsValues.GlobalDateFormat)
            row("FormattedDate") = formattedDate
        Next
    End Sub

    ''' <summary>
    ''' Call Customer Activities QA002 questions and answers file for retrieving the question and answer data
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseQA002S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Customer Activities Details data table
        Dim dtCustomerActivitiesDetail As New DataTable("CustomerActivitiesDetail")
        With dtCustomerActivitiesDetail.Columns
            .Add("CustomerNumber", GetType(String))
            .Add("TemplateID", GetType(Integer))
            .Add("QuestionText", GetType(String))
            .Add("AnswerText", GetType(String))
            .Add("RememberedAnswer", GetType(String))
            .Add("QuestionID", GetType(Integer))
        End With
        ResultDataSet.Tables.Add(dtCustomerActivitiesDetail)

        Try
            CallQA002S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBActivities-QA002S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build parameters and call stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallQA002S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL QA002S(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6, @PARAM7, @PARAM8, @PARAM9, @PARAM10, @PARAM11)"
        _cmd.CommandType = CommandType.Text

        Dim pMode As New iDB2Parameter
        Dim pCustomerActivitiesHeaderID As iDB2Parameter
        Dim pCustomerNumber As iDB2Parameter
        Dim pTemplateID As iDB2Parameter
        Dim pQuestionID As iDB2Parameter
        Dim pSelectedTALENTUser As iDB2Parameter
        Dim pQuestionText As iDB2Parameter
        Dim pAnswerText As iDB2Parameter
        Dim pRememberedAnswer As iDB2Parameter
        Dim pDelimeter As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pErrorCode As iDB2Parameter

        pMode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 1)
        pSelectedTALENTUser = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 10)
        pCustomerActivitiesHeaderID = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Decimal, 13)
        pCustomerNumber = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 12)
        pTemplateID = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Decimal, 13)
        pQuestionID = _cmd.Parameters.Add(Param5, iDB2DbType.iDB2VarChar, 1000)
        pQuestionText = _cmd.Parameters.Add(Param6, iDB2DbType.iDB2VarChar, 1000)
        pAnswerText = _cmd.Parameters.Add(Param7, iDB2DbType.iDB2VarChar, 1000)
        pRememberedAnswer = _cmd.Parameters.Add(Param8, iDB2DbType.iDB2Char, 1)
        pDelimeter = _cmd.Parameters.Add(Param9, iDB2DbType.iDB2Char, 1)
        pSource = _cmd.Parameters.Add(Param10, iDB2DbType.iDB2Char, 1)
        pErrorCode = _cmd.Parameters.Add(Param11, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput

        pMode.Value = ActivitiesDataEntity.CustomerActivitiesHeaderMode
        pSelectedTALENTUser.Value = ActivitiesDataEntity.ActivityUserName
        pCustomerActivitiesHeaderID.Value = ActivitiesDataEntity.CustomerActivitiesHeaderID
        pCustomerNumber.Value = ActivitiesDataEntity.CustomerNumber
        pTemplateID.Value = ActivitiesDataEntity.TemplateID
        pQuestionText.Value = String.Empty
        pAnswerText.Value = String.Empty
        pRememberedAnswer.Value = String.Empty
        pSource.Value = ActivitiesDataEntity.Source
        pErrorCode.Value = String.Empty

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "CustomerActivitiesDetail")

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(Param11).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param11).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

    ''' <summary>
    ''' Call Customer Activities comments stored procedure to execute read, delete, update or create a comment
    ''' </summary>
    ''' <returns>An error object detailing any issues</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseCD041S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Customer Activities Comments data table
        Dim dtCustomerActivitiesComments As New DataTable("CustomerActivitiesComments")
        With dtCustomerActivitiesComments.Columns
            .Add("CommentID", GetType(Integer))
            .Add("AgentName", GetType(String))
            .Add("UpdatedDate", GetType(Integer))
            .Add("UpdatedTime", GetType(Integer))
            .Add("CommentText", GetType(String))
            .Add("CommentEdited", GetType(String))
        End With
        ResultDataSet.Tables.Add(dtCustomerActivitiesComments)

        Try
            CallCD041S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBActivities-CD041S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build parameters and call stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallCD041S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL CD041S(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pMode As New iDB2Parameter
        Dim pCustomerActivitiesHeaderID As iDB2Parameter
        Dim pCommentID As IDbDataParameter
        Dim pCurrentAgent As IDbDataParameter
        Dim pCommentText As IDbDataParameter

        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pSource = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pMode = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1)
        pCustomerActivitiesHeaderID = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Decimal, 13)
        pCommentID = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Decimal, 13)
        pCommentID.Direction = ParameterDirection.InputOutput
        pCurrentAgent = _cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 10)
        pCommentText = _cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 32000)

        pErrorCode.Value = String.Empty
        pSource.Value = ActivitiesDataEntity.Source
        pMode.Value = ActivitiesDataEntity.CustomerActivitiesCommentsMode
        pCustomerActivitiesHeaderID.Value = ActivitiesDataEntity.CustomerActivitiesHeaderID
        pCommentID.Value = ActivitiesDataEntity.ActivityCommentID
        pCurrentAgent.Value = _settings.AgentEntity.AgentUsername
        pCommentText.Value = ActivitiesDataEntity.ActivityCommentText

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "CustomerActivitiesComments")

        Dim drStatus As DataRow = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).NewRow
        If CStr(_cmd.Parameters(Param0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param0).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
            Dim commentId As Integer = 0
            If Integer.TryParse(_cmd.Parameters(Param4).Value, commentId) Then
                ActivitiesDataEntity.ActivityCommentID = commentId
            End If
        End If
        ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Add(drStatus)
    End Sub

    ''' <summary>
    ''' Call Customer Activities comments stored procedure to execute read, delete, update or create a comment
    ''' </summary>
    ''' <returns>An error object detailing any issues</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseCD042S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Customer Activities File Attachment data table
        Dim dtCustomerActivitiesFileAttachments As New DataTable("CustomerActivitiesFileAttachments")
        With dtCustomerActivitiesFileAttachments.Columns
            .Add("AttachmentID", GetType(Integer))
            .Add("AgentName", GetType(String))
            .Add("DateAdded", GetType(Integer))
            .Add("TimeAdded", GetType(Integer))
            .Add("FileName", GetType(String))
            .Add("Description", GetType(String))
        End With
        ResultDataSet.Tables.Add(dtCustomerActivitiesFileAttachments)

        Try
            CallCD042S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBActivities-CD042S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build parameters and call stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallCD042S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL CD042S(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6, @PARAM7)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pMode As New iDB2Parameter
        Dim pCustomerActivitiesHeaderID As iDB2Parameter
        Dim pFileAttachmentID As IDbDataParameter
        Dim pCurrentAgent As IDbDataParameter
        Dim pFileName As IDbDataParameter
        Dim pFileDescription As IDbDataParameter

        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pSource = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pMode = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1)
        pCustomerActivitiesHeaderID = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Decimal, 13)
        pFileAttachmentID = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Decimal, 13)
        pFileAttachmentID.Direction = ParameterDirection.InputOutput
        pCurrentAgent = _cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 10)
        pFileName = _cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 300)
        pFileDescription = _cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 32000)

        pErrorCode.Value = String.Empty
        pSource.Value = ActivitiesDataEntity.Source
        pMode.Value = ActivitiesDataEntity.CustomerActivitiesAttachmentsMode
        pCustomerActivitiesHeaderID.Value = ActivitiesDataEntity.CustomerActivitiesHeaderID
        pFileAttachmentID.Value = ActivitiesDataEntity.ActivityFileAttachmentID
        pCurrentAgent.Value = _settings.AgentEntity.AgentUsername
        pFileName.Value = ActivitiesDataEntity.ActivityFileName
        pFileDescription.Value = ActivitiesDataEntity.ActivityFileDescription

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "CustomerActivitiesFileAttachments")

        Dim drStatus As DataRow = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).NewRow
        If CStr(_cmd.Parameters(Param0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param0).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
            Dim fileAttachmentID As Integer = 0
            If Integer.TryParse(_cmd.Parameters(Param4).Value, fileAttachmentID) Then
                ActivitiesDataEntity.ActivityFileAttachmentID = fileAttachmentID
            End If
        End If
        ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Add(drStatus)
    End Sub

#End Region

End Class
