Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_activity_questions
    ''' </summary>
    <Serializable()> _
    Public Class tbl_activity_questions
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_activity_questions"
        ''' <summary>
        ''' to create only one instance of string when called by insert multiple
        ''' as it is a long string 
        ''' as well as to speed up the insert when called under transaction
        ''' </summary>
        Private ReadOnly _insertSQLStatement As String = String.Empty
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_activity_questions" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Retrieves all questions
        ''' </summary>
        ''' <param name="cacheing">Optional boolean to determine if cacheing is enabled, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time in mins value, default 30 mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetAll(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAll")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_activity_questions] ORDER BY [QUESTION_ID] DESC"

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
        ''' Retrieves top 30 questions
        ''' </summary>
        ''' <param name="cacheing">Optional boolean to determine if cacheing is enabled, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time in mins value, default 30 mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetTOP30(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetTOP30")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT TOP(30)* FROM [tbl_activity_questions]"

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
        ''' Retrieves question by Id
        ''' </summary>
        ''' <param name="questionID">Id of the required question</param>
        ''' <param name="cacheing">Optional boolean to determine if cacheing is enabled, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time in mins value, default 30 mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetActivityQuestionByID(ByVal questionID As Integer, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetActivityQuestionByID")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_activity_questions] where [QUESTION_ID]=@QuestionID"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QuestionID", questionID))

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
        ''' Updates all columns of a question of questionId
        ''' </summary>
        ''' <param name="questionId">The question to be updated</param>
        ''' <param name="questionText">Question text</param>
        ''' <param name="answerType">Answer type of question/ checkbox/TextField etc.</param>
        ''' <param name="allowSelectOtherOption">Option to handle specify option on pre defined answer type</param>
        ''' <param name="mandatory">Manditory field</param>
        ''' <param name="priceBandList">Price bands split with (,) commas</param>
        ''' <param name="regularExpression">Regular expression on question</param>
        ''' <param name="hyperlink">Hyperlink</param>
        ''' <param name="rememberedAnswer">Whether the answer is remembered</param>
        ''' <param name="askQuestionPerHospitalitybooking">Whether Question is to be asked per hospitality booking</param>
        ''' <returns>number of rows affected</returns>
        ''' <remarks></remarks>
        Public Function UpdateAll(ByVal questionId As Integer, ByVal questionText As String, ByVal answerType As Integer, ByVal allowSelectOtherOption As Boolean, ByVal mandatory As Boolean,
                                  ByVal priceBandList As String, ByVal regularExpression As String, ByVal hyperlink As String, ByVal rememberedAnswer As Boolean, ByVal askQuestionPerHospitalitybooking As Boolean) As Integer
            Dim output As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("UPDATE [tbl_activity_questions] SET [QUESTION_TEXT]=@QuestionText, [ANSWER_TYPE]=@AnswerType, [ALLOW_SELECT_OTHER_OPTION]=@AllowSelectOtherOption,  ")
            sqlStatement.Append("[MANDATORY]=@Mandatory, [PRICE_BAND_LIST]=@PriceBandList, [REGULAR_EXPRESSION]=@RegularExpression, [HYPERLINK]=@HyperlinK, [REMEMBERED_ANSWER]=@RememberedAnswer, [ASK_QUESTION_PER_HOSPITALITY_BOOKING]=@AskQuestionPerHospitalitybooking ")
            sqlStatement.Append("WHERE [QUESTION_ID] = @QuestionID")

            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QuestionID", questionId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QuestionText", questionText))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AnswerType", answerType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AllowSelectOtherOption", allowSelectOtherOption))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Mandatory", mandatory))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PriceBandList", priceBandList))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RegularExpression", regularExpression))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Hyperlink", hyperlink))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RememberedAnswer", rememberedAnswer))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AskQuestionPerHospitalitybooking", askQuestionPerHospitalitybooking))
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                output = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return output
        End Function

        ''' <summary>
        ''' WildCard text search
        ''' </summary>
        ''' <param name="categoryText">Search text</param>
        ''' <param name="cacheing">Optional boolean to determine if cacheing is enabled, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time in mins value, default 30 mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetByTextWildCard(ByVal categoryText As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByTextWildCard")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT TOP(30) * FROM [tbl_activity_questions] WHERE [QUESTION_TEXT] like @CategoryText"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CategoryText", "%" & categoryText & "%"))

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
        ''' WildCard text search
        ''' </summary>
        ''' <param name="categoryText">Search text</param>
        ''' <param name="cacheing">Optional boolean to determine if cacheing is enabled, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time in mins value, default 30 mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetByTextWildCardALL(ByVal categoryText As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByTextWildCardALL")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_activity_questions] WHERE [QUESTION_TEXT] like @CategoryText"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CategoryText", "%" & categoryText & "%"))

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
        ''' Insert a new question on all columns
        ''' </summary>
        ''' <param name="questionText">Question text</param>
        ''' <param name="answerType">Answer type of question/ checkbox/TextField etc.</param>
        ''' <param name="allowSelectOtherOption">Option to handle specify option on pre defined answer type</param>
        ''' <param name="mandatory">Manditory field</param>
        ''' <param name="priceBandList">Price bands split with (,) commas</param>
        ''' <param name="regularExpression">Regular expression on question</param>
        ''' <param name="hyperlink">Hyperlink</param>
        ''' <param name="rememberedAnswer">Whether or not this answer is to be remembered</param>
        ''' <param name="askQuestionPerHospitalitybooking">Whether Question is to be asked per hospitality booking</param>
        ''' <returns>number of rows affected</returns>
        ''' <remarks></remarks>
        Public Function InsertAll(ByVal questionText As String, ByVal answerType As Integer, ByVal allowSelectOtherOption As Boolean, ByVal mandatory As Boolean,
                                  ByVal priceBandList As String, ByVal regularExpression As String, ByVal hyperlink As String, ByVal rememberedAnswer As Boolean, ByVal askQuestionPerHospitalitybooking As Boolean) As Integer
            Dim output As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("INSERT INTO [tbl_activity_questions] ([QUESTION_TEXT], [ANSWER_TYPE], [ALLOW_SELECT_OTHER_OPTION],  ")
            sqlStatement.Append("[MANDATORY], [PRICE_BAND_LIST], [REGULAR_EXPRESSION], [HYPERLINK], [REMEMBERED_ANSWER], [ASK_QUESTION_PER_HOSPITALITY_BOOKING]) VALUES ")
            sqlStatement.Append("(@QuestionText, @AnswerType, @AllowSelectOtherOption, @Mandatory, @PriceBandList, @RegularExpression, @Hyperlink, @RememberedAnswer, @AskQuestionPerHospitalitybooking) ")
            sqlStatement.Append("SELECT SCOPE_IDENTITY()")

            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QuestionText", questionText))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AnswerType", answerType))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AllowSelectOtherOption", allowSelectOtherOption))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Mandatory", mandatory))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@PriceBandList", priceBandList))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RegularExpression", regularExpression))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Hyperlink", hyperlink))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@RememberedAnswer", rememberedAnswer))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AskQuestionPerHospitalitybooking", askQuestionPerHospitalitybooking))
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                output = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return output
        End Function

        ''' <summary>
        ''' Delete question by question Id
        ''' </summary>
        ''' <param name="ID">Question Id </param>
        ''' <returns>number of affect rows</returns>
        ''' <remarks></remarks>
        Public Function DeleteByID(ByVal ID As Integer) As Integer
            Dim output As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj

            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            Dim sqlStatement As String = "DELETE FROM [tbl_activity_questions] WHERE [QUESTION_ID] = @ID"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ID", ID))
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                output = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return output
        End Function

        ''' <summary>
        ''' Get question by question text
        ''' </summary>
        ''' <param name="questionText">Question text</param>
        ''' <param name="cacheing">Optional boolean to determine if cacheing is enabled, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time in mins value, default 30 mins</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetByQuestionText(ByVal questionText As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByQuestionText")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_activity_questions] WHERE [QUESTION_TEXT] = @QuestionText"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QuestionText", questionText))

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
