Imports System.Data.SqlClient
Imports Talent.Common.DataObjects.TableObjects
Imports System.Text
Imports Talent.Common

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access activity tables
    ''' </summary>
    <Serializable()> _
    Public Class Activities
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings
        Private _tblActivityQuestions As tbl_activity_questions
        Private _tblActivityQuestionsAnswerCategories As tbl_activity_questions_answer_categories
        Private _tblActivityQuestionsAnswers As tbl_activity_questions_answers
        Private _tblActivityQuestionsWithAnswers As tbl_activity_questions_with_answers
        Private _tblActivityTemplates As tbl_activity_templates
        Private _tblActivityTemplateType As tbl_activity_template_type
        Private _tblActivityTemplatesDetail As tbl_activity_templates_detail
        Private _tblActivityTemplatesDefaults As tbl_activity_templates_defaults

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "Activities"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="Products" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"
        
        ''' <summary>
        ''' Create and Gets the tbl_activity_questions instance with DESettings
        ''' </summary>
        ''' <value>tbl_activity_questions</value>
        Public ReadOnly Property TblActivityQuestions() As tbl_activity_questions
            Get
                If (_tblActivityQuestions Is Nothing) Then
                    _tblActivityQuestions = New tbl_activity_questions(_settings)
                End If
                Return _tblActivityQuestions
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_activity_questions_answer_categories instance with DESettings
        ''' </summary>
        ''' <value>tbl_activity_questions_answer_categories</value>
        Public ReadOnly Property TblActivityQuestionsAnswerCategories() As tbl_activity_questions_answer_categories
            Get
                If (_tblActivityQuestionsAnswerCategories Is Nothing) Then
                    _tblActivityQuestionsAnswerCategories = New tbl_activity_questions_answer_categories(_settings)
                End If
                Return _tblActivityQuestionsAnswerCategories
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_activity_questions_answers instance with DESettings
        ''' </summary>
        ''' <value>tbl_activity_questions_answers</value>
        Public ReadOnly Property TblActivityQuestionsAnswers() As tbl_activity_questions_answers
            Get
                If (_tblActivityQuestionsAnswers Is Nothing) Then
                    _tblActivityQuestionsAnswers = New tbl_activity_questions_answers(_settings)
                End If
                Return _tblActivityQuestionsAnswers
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_activity_questions_with_answers instance with DESettings
        ''' </summary>
        ''' <value>tbl_activity_questions_with_answers</value>
        Public ReadOnly Property TblActivityQuestionsWithAnswers() As tbl_activity_questions_with_answers
            Get
                If (_tblActivityQuestionsWithAnswers Is Nothing) Then
                    _tblActivityQuestionsWithAnswers = New tbl_activity_questions_with_answers(_settings)
                End If
                Return _tblActivityQuestionsWithAnswers
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_activity_templates instance with DESettings
        ''' </summary>
        ''' <value>tbl_activity_templates</value>
        Public ReadOnly Property TblActivityTemplates() As tbl_activity_templates
            Get
                If (_tblActivityTemplates Is Nothing) Then
                    _tblActivityTemplates = New tbl_activity_templates(_settings)
                End If
                Return _tblActivityTemplates
            End Get
        End Property


        ''' <summary>
        ''' Create and Gets the tbl_activity_template_type instance with DESettings
        ''' </summary>
        ''' <value>tbl_activity_template_type</value>
        Public ReadOnly Property TblActivityTemplateType() As tbl_activity_template_type
            Get
                If (_tblActivityTemplateType Is Nothing) Then
                    _tblActivityTemplateType = New tbl_activity_template_type(_settings)
                End If
                Return _tblActivityTemplateType
            End Get
        End Property

        ''' <summary>
        ''' Create and Gets the tbl_activity_templates_detail instance with DESettings
        ''' </summary>
        ''' <value>tbl_activity_templates_detail</value>
        Public ReadOnly Property TblActivityTemplatesDetail() As tbl_activity_templates_detail
            Get
                If (_tblActivityTemplatesDetail Is Nothing) Then
                    _tblActivityTemplatesDetail = New tbl_activity_templates_detail(_settings)
                End If
                Return _tblActivityTemplatesDetail
            End Get
        End Property

        Public ReadOnly Property TblActivityTemplatesDefaults() As tbl_activity_templates_defaults
            Get
                If (_tblActivityTemplatesDefaults Is Nothing) Then
                    _tblActivityTemplatesDefaults = New tbl_activity_templates_defaults(_settings)
                End If
                Return _tblActivityTemplatesDefaults
            End Get
        End Property
        
#End Region

#Region "Public Functions"

        ''' <summary>
        ''' Get a list of questions related to a particular template
        ''' </summary>
        ''' <param name="templateID">The given template ID</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetExistingActivityQuestionByTemplateID(ByVal templateID As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetExistingActivityQuestionByTemplateID")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplateID", templateID))
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT d.[SEQUENCE], a.[QUESTION_TEXT], d.[QUESTION_ID], a.[ANSWER_TYPE], a.[ALLOW_SELECT_OTHER_OPTION], a.[REGULAR_EXPRESSION], a.[MANDATORY], a.[HYPERLINK], a.[PRICE_BAND_LIST], a.[REMEMBERED_ANSWER], a.[ASK_QUESTION_PER_HOSPITALITY_BOOKING] ")
                sqlStatement.Append("FROM [tbl_activity_questions] a, [tbl_activity_templates_detail] d ")
                sqlStatement.Append("WITH(NOLOCK) ")
                sqlStatement.Append("WHERE a.[QUESTION_ID] = d.[QUESTION_ID] ")
                sqlStatement.Append("AND d.[TEMPLATE_ID] = @TemplateID ")
                sqlStatement.Append("ORDER BY d.[SEQUENCE] ASC")
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Deletes question and associates template relationship
        ''' </summary>
        ''' <param name="templateID">The given template ID</param>
        ''' <param name="questionID">The question ID</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeleteActivityQuestionTemplate(ByVal templateID As Integer, ByVal questionID As Integer) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            Dim sqlStatement As String = "DELETE FROM [tbl_activity_questions] WHERE [QUESTION_ID] = @QuestionID"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QuestionID", questionID))
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If

            If affectedRows > 0 Then
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                sqlStatement = "DELETE FROM [tbl_activity_templates_detail] WHERE [QUESTION_ID] = @QuestionID AND [TEMPLATE_ID]=@TemplateID"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QuestionID", questionID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplateID", templateID))
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

                'Execute
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                End If
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        ''' <summary>
        ''' Get a list of categories related to a question
        ''' </summary>
        ''' <param name="questionID">The given question ID</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetAnswerCategoriesByQuestionID(ByVal questionID As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAnswerCategoriesByQuestionID")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QuestionID", questionID))
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT a.[ANSWER_ID], c.[CATEGORY_ID], a.[QUESTION_ID], c.[CATEGORY_NAME] ")
                sqlStatement.Append("FROM [tbl_activity_questions_with_answers] a, [tbl_activity_questions_answers] d, [tbl_activity_questions_answer_categories] c ")
                sqlStatement.Append("WITH(NOLOCK) ")
                sqlStatement.Append("WHERE d.[ANSWER_ID] = a.[ANSWER_ID] ")
                sqlStatement.Append("AND d.[CATEGORY_ID] = c.[CATEGORY_ID] ")
                sqlStatement.Append("AND a.[QUESTION_ID] = @QuestionID ")
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Get the category ID and Name based on the given question ID
        ''' </summary>
        ''' <param name="questionID">The given question ID</param>
        ''' <param name="cacheing">The caching property</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetCategoryIDByQuestionID(ByVal questionID As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetCategoryIDByQuestionID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QuestionID", questionID))
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT DISTINCT d.[CATEGORY_ID], b.[CATEGORY_NAME] ")
                sqlStatement.Append("FROM  [tbl_activity_questions_with_answers] d, [tbl_activity_questions_answer_categories] b ")
                sqlStatement.Append("With (NOLOCK) ")
                sqlStatement.Append("WHERE d.[QUESTION_ID] = @QuestionID ")
                sqlStatement.Append("AND d.[CATEGORY_ID] = b.[CATEGORY_ID]")
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Get answer without category based on the given question ID
        ''' </summary>
        ''' <param name="questionID">The given question ID</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetAnswerWithoutCategoryByQuestionID(ByVal questionID As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAnswerWithoutCategoryByQuestionID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QuestionID", questionID))
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT DISTINCT d.[ANSWER_ID], d.[ANSWER_TEXT] ")
                sqlStatement.Append("FROM [tbl_activity_questions_with_answers] a, [tbl_activity_questions_answers] d ")
                sqlStatement.Append("WITH(NOLOCK) ")
                sqlStatement.Append("WHERE d.[ANSWER_ID] = a.[ANSWER_ID] ")
                sqlStatement.Append("AND a.[QUESTION_ID] = @QuestionID ")
                sqlStatement.Append("AND d.[CATEGORY_ID] IS NULL ")
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Get the answer details based on the given question ID
        ''' </summary>
        ''' <param name="questionID">The given question ID</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetAnswerByQuestionID(ByVal questionID As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAnswerWithoutCategoryByQuestionID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & questionID
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QuestionID", questionID))
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT DISTINCT d.[ANSWER_ID], d.[ANSWER_TEXT] ")
                sqlStatement.Append("FROM [tbl_activity_questions_with_answers] a, [tbl_activity_questions_answers] d ")
                sqlStatement.Append("WITH(NOLOCK) ")
                sqlStatement.Append("WHERE (d.[ANSWER_ID] = a.[ANSWER_ID] OR d.[CATEGORY_ID] = a.[CATEGORY_ID]) ")
                sqlStatement.Append("AND a.[QUESTION_ID] = @QuestionID ")
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

        ''' <summary>
        ''' ## THIS CAN BE REMOVED AS IT HAS NO REFERENCES AND SELECTS DATA OVER A TABLE THAT NO LONGER EXISTS ##
        ''' Get thw question details based on the business unit and template ID
        ''' </summary>
        ''' <param name="templateID">The given template ID</param>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetQuestionIDByTemplateID(ByVal templateID As String, ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetQuestionIDByTemplateID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = False
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplateID", templateID))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT a.[QUESTION_ID], d.[TEMPLATE_ID], d.[NAME] ")
                sqlStatement.Append("FROM [tbl_product_questions_templates] d, [tbl_product_questions_templates_detail] a ")
                sqlStatement.Append("WITH(NOLOCK) ")
                sqlStatement.Append("WHERE a.[TEMPLATE_ID] = d.[TEMPLATE_ID] ")
                sqlStatement.Append("AND d.[TEMPLATE_ID] = @TemplateID ")
                sqlStatement.Append("AND d.[BUSINESS_UNIT]=@BusinessUnit")
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Retrieve the activity Name and IDs for the activity drop down list shown on the activities page.
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="cacheing">Is this data cached</param>
        ''' <param name="cacheTimeMinutes">How long is it cached for in mins</param>
        ''' <returns>Activity name and ID datatable of results</returns>
        ''' <remarks></remarks>
        Public Function GetActivityTemplatesForActivityPage(ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetActivityTemplatesForActivityPage")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT a.[NAME], a.[TEMPLATE_ID] ")
                sqlStatement.Append("FROM [tbl_activity_templates] a, [tbl_activity_template_type] b ")
                sqlStatement.Append("WITH(NOLOCK) ")
                sqlStatement.Append("WHERE b.[TEMPLATE_TYPE_ID] = a.[TEMPLATE_TYPE] ")
                sqlStatement.Append("AND b.[HIDE_IN_ACTIVITIES_PAGE] = 0 ")
                sqlStatement.Append("AND a.[BUSINESS_UNIT] = @BusinessUnit")
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Get the activity template details (including type details) based on the given template ID and business unit
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="templateID">The given template ID</param>
        ''' <param name="cacheing">Is this data cached</param>
        ''' <param name="cacheTimeMinutes">How long is it cached for in mins</param>
        ''' <returns>Data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetActivityTemplatesTypeByBUTemplateID(ByVal businessUnit As String, ByVal templateID As Integer, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetActivityTemplatesTypeByBUTemplateID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplateID", templateID))
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT * ")
                sqlStatement.Append("FROM [tbl_activity_templates] a, [tbl_activity_template_type] b ")
                sqlStatement.Append("WITH(NOLOCK) ")
                sqlStatement.Append("WHERE b.[TEMPLATE_TYPE_ID] = a.[TEMPLATE_TYPE] ")
                sqlStatement.Append("AND b.[HIDE_IN_ACTIVITIES_PAGE] = 0 ")
                sqlStatement.Append("AND a.[BUSINESS_UNIT] = @BusinessUnit ")
                sqlStatement.Append("AND a.[TEMPLATE_ID] = @TemplateID")
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Get the activity template details (including type details) based on the given template ID
        ''' </summary>
        ''' <param name="templateID">The given template ID</param>
        ''' <param name="cacheing">Is this data cached</param>
        ''' <param name="cacheTimeMinutes">How long is it cached for in mins</param>
        ''' <returns>Data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetActivityTemplatesTypeByTemplateID(ByVal templateID As Integer, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetActivityTemplatesTypeByTemplateID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplateID", templateID))
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT * ")
                sqlStatement.Append("FROM [tbl_activity_templates] a, [tbl_activity_template_type] b ")
                sqlStatement.Append("WITH(NOLOCK) ")
                sqlStatement.Append("WHERE b.[TEMPLATE_TYPE_ID] = a.[TEMPLATE_TYPE] ")
                sqlStatement.Append("AND b.[HIDE_IN_ACTIVITIES_PAGE] = 0 ")
                sqlStatement.Append("AND a.[TEMPLATE_ID] = @TemplateID")
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function


        ''' <summary>
        ''' Get all of the activity status descriptions based on the given business unit irrespective of type
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="cacheing">Is this data cached</param>
        ''' <param name="cacheTimeMinutes">How long is it cached for in mins</param>
        ''' <returns>Activity Status data table</returns>
        ''' <remarks></remarks>
        Public Function GetActivityStatusByBusinessUnit(ByVal businessUnit As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetActivityStatusByBusinessUnit")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT [DESCRIPTION] FROM [tbl_activity_status_description] WHERE [BUSINESS_UNIT] = @BusinessUnit"

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Get all of the activity status descriptions based on the given business unit irrespective of type
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="templateId">The given activity template ID</param>
        ''' <param name="cacheing">Is this data cached</param>
        ''' <param name="cacheTimeMinutes">How long is it cached for in mins</param>
        ''' <returns>Activity Status data table</returns>
        ''' <remarks></remarks>
        Public Function GetActivityStatusByBUAndTemplateId(ByVal businessUnit As String, ByVal templateId As Integer, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetActivityStatusByBusinessUnitAndTemplateId")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@TemplateID", templateId))
                Dim sqlStatement As New StringBuilder
                sqlStatement.Append("SELECT d.[DESCRIPTION] ")
                sqlStatement.Append("FROM [tbl_activity_status] s, [tbl_activity_status_description] d, [tbl_activity_templates] t ")
                sqlStatement.Append("WHERE d.[BUSINESS_UNIT] = @BusinessUnit ")
                sqlStatement.Append("AND s.[STATUS_ID] = d.[STATUS_ID] ")
                sqlStatement.Append("AND t.[TEMPLATE_ID] = @TemplateID")
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

#End Region
        
    End Class
End Namespace