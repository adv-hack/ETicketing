Imports System.Data.SqlClient

''' <summary>
''' Provides the functionality to manage data from the table tbl_activity_questions_with_answers
''' </summary>
<Serializable()> _
Public Class tbl_activity_questions_with_answers
    Inherits DBObjectBase

#Region "Class Level Fields"
    ''' <summary>
    ''' Instance of DESettings
    ''' </summary>
    Private _settings As New DESettings

    ''' <summary>
    ''' Class Name which is used in cache key construction
    ''' </summary>
    Const CACHEKEY_CLASSNAME As String = "tbl_activity_questions_with_answers"
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
    ''' Initializes a new instance of the <see cref="tbl_activity_questions_with_answers" /> class.
    ''' </summary>
    ''' <param name="settings">The DESettings instance</param>
    Sub New(ByVal settings As DESettings)
        _settings = settings
    End Sub
#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Get all the activity questions with answers records
    ''' </summary>
    ''' <param name="cacheing">The cache property</param>
    ''' <param name="cacheTimeMinutes">The cache time in mins</param>
    ''' <returns>A data table of records</returns>
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
            talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_activity_questions_with_answers]"

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
    ''' Delete an activity question record based on the given question ID
    ''' </summary>
    ''' <param name="questionID">The question ID</param>
    ''' <returns>The number of affected records</returns>
    ''' <remarks></remarks>
    Public Function DeleteByQuestionID(ByVal questionID As String) As Integer
        Dim output As Integer = 0
        Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "DeleteByQuestionID")
        Dim talentSqlAccessDetail As New TalentDataAccess
        Try
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM [tbl_activity_questions_with_answers] WHERE [QUESTION_ID]=@QuestionID"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QuestionID", questionID))

            'Execute
            Dim err As New ErrorObj
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                output = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
        Catch ex As Exception
            Throw
        Finally
            talentSqlAccessDetail = Nothing
        End Try

        'Return the results
        Return output
    End Function

    ''' <summary>
    ''' Add the relationship between the question ID and the answer ID
    ''' </summary>
    ''' <param name="questionID">The given question ID</param>
    ''' <param name="answerID">The given answer ID</param>
    ''' <param name="categoryID">The given category ID</param>
    ''' <returns>The number affected rows</returns>
    ''' <remarks></remarks>
    Public Function AddByQuestionIDAnswerID(ByVal questionID As Integer, ByVal answerID As Integer, ByVal categoryID As Integer) As Integer
        Dim output As New Integer
        Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "AddByQuestionIDAnswerID")
        Dim talentSqlAccessDetail As New TalentDataAccess
        Try
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = "INSERT INTO [tbl_activity_questions_with_answers] ([ANSWER_ID], [QUESTION_ID], [CATEGORY_ID]) VALUES (@AnswerID, @QuestionID, @CategoryID)"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@QuestionID", questionID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AnswerID", answerID))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CategoryID", categoryID))

            'Execute
            Dim err As New ErrorObj
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                output = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
        Catch ex As Exception
            Throw
        Finally
            talentSqlAccessDetail = Nothing
        End Try

        'Return the results
        Return output
    End Function

#End Region

End Class
