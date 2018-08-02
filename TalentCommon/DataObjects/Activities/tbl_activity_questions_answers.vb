Imports System.Data.SqlClient

''' <summary>
''' Provides the functionality to manage data from the table tbl_activity_questions_answers
''' </summary>
<Serializable()> _
Public Class tbl_activity_questions_answers
    Inherits DBObjectBase

#Region "Class Level Fields"
    ''' <summary>
    ''' Instance of DESettings
    ''' </summary>
    Private _settings As New DESettings

    ''' <summary>
    ''' Class Name which is used in cache key construction
    ''' </summary>
    Const CACHEKEY_CLASSNAME As String = "tbl_activity_questions_answers"
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
    ''' Initializes a new instance of the <see cref="tbl_activity_questions_answers" /> class.
    ''' </summary>
    ''' <param name="settings">The DESettings instance</param>
    Sub New(ByVal settings As DESettings)
        _settings = settings
    End Sub
#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Get all the answers from the table
    ''' </summary>
    ''' <param name="cacheing">The caching property</param>
    ''' <param name="cacheTimeMinutes">The cache time in mins</param>
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
            talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_activity_questions_answers]"

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
    ''' Get the first 30 records from the table
    ''' </summary>
    ''' <param name="cacheing">The caching property</param>
    ''' <param name="cacheTimeMinutes">The cache time in mins</param>
    ''' <returns>A data table of records</returns>
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
            talentSqlAccessDetail.CommandElements.CommandText = "SELECT TOP(30) * FROM [tbl_activity_questions_answers]"

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
    ''' Get the answers based on a given category ID
    ''' </summary>
    ''' <param name="categoryId">The category ID</param>
    ''' <param name="cacheing">The cache property</param>
    ''' <param name="cacheTimeMinutes">The cache time in mins</param>
    ''' <returns>A data table of results based on the category ID</returns>
    ''' <remarks></remarks>
    Public Function GetByID(ByVal categoryId As Integer, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
        Dim outputDataTable As New DataTable
        Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByID")
        Dim talentSqlAccessDetail As New TalentDataAccess
        Try
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = cacheing
            talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
            talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            talentSqlAccessDetail.CommandElements.CommandText = "SELECT TOP(30) * FROM tbl_activity_questions_answers WHERE [CATEGORY_ID]=@CategoryID"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CategoryID", categoryId))

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
    ''' Get the top 30 records based on the answer text given
    ''' </summary>
    ''' <param name="answerText">The given answer text which is used with wildcards</param>
    ''' <param name="cacheing">The cache property</param>
    ''' <param name="cacheTimeMinutes">The cache time in mins</param>
    ''' <returns>A data table of records based on the given answer text string</returns>
    ''' <remarks></remarks>
    Public Function GetByTextWildCard(ByVal answerText As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
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
            talentSqlAccessDetail.CommandElements.CommandText = "SELECT TOP(30) * FROM [tbl_activity_questions_answers] WHERE [ANSWER_TEXT] LIKE @AnswerText"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AnswerText", "%" & answerText & "%"))

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
    ''' Add an answer with a given category ID. If the category ID is 0 the answer is inserted with a null category.
    ''' </summary>
    ''' <param name="answerText">The answer text</param>
    ''' <param name="categoryId">The category ID</param>
    ''' <param name="givenTransaction">The given transaction</param>
    ''' <returns>Affected rows</returns>
    ''' <remarks></remarks>
    Public Function AddAnswerByTextCategoryId(ByVal answerText As String, ByVal categoryId As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
        Dim affectedRows As Integer = 0
        Dim talentSqlAccessDetail As New TalentDataAccess
        Dim err As New ErrorObj
        'Construct The Call
        talentSqlAccessDetail.Settings = _settings
        talentSqlAccessDetail.Settings.Cacheing = False
        talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
        Dim sqlStatement As String
        If categoryId <> 0 Then
            sqlStatement = "INSERT INTO [tbl_activity_questions_answers] ([CATEGORY_ID], [ANSWER_TEXT]) VALUES (@CategoryId, @AnswerText)"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CategoryId", categoryId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AnswerText", answerText))
        Else
            sqlStatement = "INSERT INTO [tbl_activity_questions_answers] ([CATEGORY_ID], [ANSWER_TEXT]) VALUES (NULL, @AnswerText)"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AnswerText", answerText))
        End If
        'Execute
        If (givenTransaction Is Nothing) Then
            err = talentSqlAccessDetail.SQLAccess()
        Else
            err = talentSqlAccessDetail.SQLAccess(givenTransaction)
        End If
        If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
            affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
        End If
        talentSqlAccessDetail = Nothing

        'Return results
        Return affectedRows
    End Function

    ''' <summary>
    ''' Delete an answer record based on the given answer ID
    ''' </summary>
    ''' <param name="answerId">The given answer ID</param>
    ''' <param name="givenTransaction">The given transaction</param>
    ''' <returns>The number of affected rows</returns>
    ''' <remarks></remarks>
    Public Function DeleteByAnswerID(ByVal answerId As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
        Dim affectedRows As Integer = 0
        Dim talentSqlAccessDetail As New TalentDataAccess
        Dim err As New ErrorObj
        'Construct The Call
        talentSqlAccessDetail.Settings = _settings
        talentSqlAccessDetail.Settings.Cacheing = False
        talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
        Dim sqlStatement As String = "DELETE FROM [tbl_activity_questions_answers] WHERE [ANSWER_ID]=@AnswerId"
        talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AnswerId", answerId))
        'Execute
        If (givenTransaction Is Nothing) Then
            err = talentSqlAccessDetail.SQLAccess()
        Else
            err = talentSqlAccessDetail.SQLAccess(givenTransaction)
        End If
        If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
            affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
        End If
        talentSqlAccessDetail = Nothing

        'Return results
        Return affectedRows
    End Function

    ''' <summary>
    ''' Get all answers that are not in a category
    ''' </summary>
    ''' <param name="cacheing">The caching property</param>
    ''' <param name="cacheTimeMinutes">The cache time in mins</param>
    ''' <returns>A data table of records</returns>
    ''' <remarks></remarks>
    Public Function GetAllWithoutCategories(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
        Dim outputDataTable As New DataTable
        Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllWithoutCategories")
        Dim talentSqlAccessDetail As New TalentDataAccess
        Try
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = cacheing
            talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
            talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_activity_questions_answers] WHERE [CATEGORY_ID] IS NULL OR [CATEGORY_ID] = ''"

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
    ''' Get the answer ID based on the given category ID
    ''' </summary>
    ''' <param name="categoryId">The given category ID</param>
    ''' <param name="cacheing">The cache property</param>
    ''' <param name="cacheTimeMinutes">The cache time in mins</param>
    ''' <returns>A data table of records with only the answer ID</returns>
    ''' <remarks></remarks>
    Public Function GetAnswerIDByCategoryID(ByVal categoryId As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
        Dim outputDataTable As New DataTable
        Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetAllWithoutCategories")
        Dim talentSqlAccessDetail As New TalentDataAccess
        Try
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = cacheing
            talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
            talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            talentSqlAccessDetail.CommandElements.CommandText = "SELECT [ANSWER_ID] FROM [tbl_activity_questions_answers] WHERE [CATEGORY_ID] = @CategoryID"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CategoryID", categoryId))
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