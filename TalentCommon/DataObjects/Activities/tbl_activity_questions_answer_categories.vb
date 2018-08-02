Imports System.Data.SqlClient

''' <summary>
''' Provides the functionality to manage data from the table tbl_activity_questions_answer_categories
''' </summary>
<Serializable()> _
Public Class tbl_activity_questions_answer_categories
    Inherits DBObjectBase

#Region "Class Level Fields"
    ''' <summary>
    ''' Instance of DESettings
    ''' </summary>
    Private _settings As New DESettings

    ''' <summary>
    ''' Class Name which is used in cache key construction
    ''' </summary>
    Const CACHEKEY_CLASSNAME As String = "tbl_activity_questions_answer_categories"
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
    ''' Initializes a new instance of the <see cref="tbl_activity_questions_answer_categories" /> class.
    ''' </summary>
    ''' <param name="settings">The DESettings instance</param>
    Sub New(ByVal settings As DESettings)
        _settings = settings
    End Sub
#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Get all the answer categories records
    ''' </summary>
    ''' <param name="cacheing">Caching property</param>
    ''' <param name="cacheTimeMinutes">Cache time</param>
    ''' <returns>Data table of answer categories</returns>
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
            talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_activity_questions_answer_categories]"

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
    ''' Get a particular answer category ID
    ''' </summary>
    ''' <param name="categoryId">The given category ID</param>
    ''' <param name="cacheing">The caching property</param>
    ''' <param name="cacheTimeMinutes">The cache time</param>
    ''' <returns>A data table with the given answer category id record</returns>
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
            talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_activity_questions_answer_categories] WHERE [CATEGORY_ID]=@CategoryID"
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
    ''' Insert a new answer category by the given category name
    ''' </summary>
    ''' <param name="categoryName">The category name</param>
    ''' <param name="givenTransaction">The given transaction</param>
    ''' <returns>The affected rows after the insert</returns>
    ''' <remarks></remarks>
    Public Function AddCategoryByName(ByVal categoryName As String, Optional ByVal givenTransaction As SqlTransaction = Nothing) As Integer
        Dim affectedRows As Integer = 0
        Dim talentSqlAccessDetail As New TalentDataAccess
        Dim err As New ErrorObj
        'Construct The Call
        talentSqlAccessDetail.Settings = _settings
        talentSqlAccessDetail.Settings.Cacheing = False
        talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
        Dim sqlStatement As String = "INSERT INTO [tbl_activity_questions_answer_categories] ([CATEGORY_NAME]) VALUES (@CategoryName)"
        talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
        talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CategoryName", categoryName))

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
    ''' Get the answer category records by the given category name
    ''' </summary>
    ''' <param name="categoryName">The given category name</param>
    ''' <param name="cacheing">The caching property</param>
    ''' <param name="cacheTimeMinutes">The cache time in mins</param>
    ''' <returns>The answer category records by the given category name</returns>
    ''' <remarks></remarks>
    Public Function GetByName(ByVal categoryName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
        Dim outputDataTable As New DataTable
        Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByName")
        Dim talentSqlAccessDetail As New TalentDataAccess
        Try
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = cacheing
            talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
            talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_activity_questions_answer_categories] WHERE [CATEGORY_NAME]=@CategoryName"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@CategoryName", categoryName))

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