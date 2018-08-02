Imports System.Data.SqlClient
Imports System.Text

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_querystring based on business functionality
    ''' </summary>
    <Serializable()>
    Public Class tbl_querystring
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_querystring"

#End Region

#Region "Constructors"

        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_querystring" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets all the carrier records from the table tbl_querystring
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing].</param>
        ''' <param name="cacheTimeMinutes">The cache time minutes.</param>
        ''' <returns>DataSet with records</returns>
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
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From tbl_querystring"

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
        ''' Gets the querystring records based on the given querystring.
        ''' </summary>
        ''' <param name="obfuscatedQuerystring">The carrier.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetActiveByObfuscatedQueryString(ByVal obfuscatedQuerystring As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetActiveByObfuscatedQueryString")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(obfuscatedQuerystring)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From tbl_querystring Where QUERYSTRING_OBFUSCATED=@obfuscatedQuerystring AND ACTIVE = 1"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@obfuscatedQuerystring", obfuscatedQuerystring))

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
        ''' Gets the querystring records based on the given querystring.
        ''' </summary>
        ''' <param name="description">The carrier.</param>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns>DataTable</returns>
        Public Function GetByDescription(ByVal description As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable

            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByDescription")
            Dim talentSqlAccessDetail As New TalentDataAccess

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & ToUpper(description)
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select * From tbl_querystring Where DESCRIPTION=@description"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@description", description))

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
        ''' Gets the specified carrier record from tbl_querystring by the given record id
        ''' </summary>
        ''' <param name="querystringId">The given record Id</param>
        ''' <param name="cacheing">Optional boolean value to enable caching, default true</param>
        ''' <param name="cacheTimeMinutes">Option integer value to represent cache time, default 30</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetQueryStringRecordById(ByVal querystringId As Integer, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetQueryStringRecordById")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "SELECT * FROM [tbl_querystring] WHERE QUERYSTRING_ID = @Id"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Id", querystringId))

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
        ''' Update the specified tbl_querystring record based on the given data and record id
        ''' </summary>
        ''' <param name="querystringId">The unique record id</param>
        ''' <param name="active">The boolean value to indicate whether or not this record is active</param>
        ''' <param name="description">The record description</param>
        ''' <param name="page">The page name</param>
        ''' <param name="querystringValue">The querystring string</param>
        ''' <param name="querystringObfuscated">The obfuscated querystring string</param>
        ''' <returns>Number of records affected</returns>
        ''' <remarks></remarks>
        Public Function UpdateQueryStringRecordById(ByVal querystringId As Integer, ByVal active As Boolean, ByVal description As String, ByVal page As String, ByVal querystringValue As String, ByVal querystringObfuscated As String) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("UPDATE [tbl_querystring] SET ACTIVE = @active, DESCRIPTION = @description, PAGE = @page, QUERYSTRING_VALUE = @querystringValue, QUERYSTRING_OBFUSCATED = @querystringObfuscated ")
            sqlStatement.Append("WHERE QUERYSTRING_ID = @querystringId ")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@active", active))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@description", description))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@page", page))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@querystringValue", querystringValue))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@querystringObfuscated", querystringObfuscated))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@querystringId", querystringId))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        ''' <summary>
        ''' Delete the record from tbl_querystring based on the given record id
        ''' </summary>
        ''' <param name="idToUseToDelete">The record id to delete</param>
        ''' <returns>The number of records affected</returns>
        ''' <remarks></remarks>
        Public Function Delete(ByVal idToUseToDelete As Long) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = String.Empty
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            talentSqlAccessDetail.CommandElements.CommandText = "DELETE FROM [tbl_querystring] WHERE QUERYSTRING_ID = @Id"
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Id", idToUseToDelete))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

        ''' 
        ''' <summary>
        ''' Insert the specified tbl_querystring record based on the given data
        ''' </summary>
        ''' <param name="querystringId">The unique record id</param>
        ''' <param name="active">The boolean value to indicate whether or not this record is active</param>
        ''' <param name="description">The record description</param>
        ''' <param name="page">The page name</param>
        ''' <param name="querystringValue">The querystring string</param>
        ''' <param name="querystringObfuscated">The obfuscated querystring string</param>
        ''' <returns>Number of records affected</returns>
        ''' <remarks></remarks>
        Public Function AddNewQueryStringRecord(ByVal active As Boolean, ByVal description As String, ByVal page As String, ByVal querystringValue As String, ByVal querystringObfuscated As String) As Integer
            Dim affectedRows As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As New StringBuilder
            sqlStatement.Append("INSERT INTO [tbl_querystring] (ACTIVE, DESCRIPTION, PAGE, QUERYSTRING_VALUE, QUERYSTRING_OBFUSCATED)")
            sqlStatement.Append("VALUES (@active, @description, @page, @querystringValue, @querystringObfuscated)")
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@active", active))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@description", description))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@page", page))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@querystringValue", querystringValue))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@querystringObfuscated", querystringObfuscated))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return affectedRows
        End Function

#End Region

    End Class

End Namespace