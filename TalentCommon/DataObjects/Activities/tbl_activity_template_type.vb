Imports System.Data.SqlClient

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_activity_template_type
    ''' </summary>
    <Serializable()> _
    Public Class tbl_activity_template_type
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_activity_template_type"
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
        ''' Initializes a new instance of the <see cref="tbl_activity_template_type" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Get the activity template type records by the given business unit, based on use in the maintenance portal
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetByBUForMaintenance(ByVal businessUnit As String, Optional ByVal cacheing As Boolean = False, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetByBUForMaintenance")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim sqlStatement As String = String.Empty
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & businessUnit
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                sqlStatement = "SELECT [TEMPLATE_TYPE_ID], [NAME], [SAVE_TO_TALENT] FROM [tbl_activity_template_type] WHERE [BUSINESS_UNIT]=@BusinessUnit AND [HIDE_IN_MAINTENANCE] = 0"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

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
        ''' Gets the settings for a template type by name and business unit
        ''' </summary>
        ''' <param name="businessUnit">The given business unit</param>
        ''' <param name="typeName">Name of Template type</param>
        ''' <param name="cacheing">The cache property</param>
        ''' <param name="cacheTimeMinutes">The cache time in mins</param>
        ''' <returns>A data table of results</returns>
        ''' <remarks></remarks>
        Public Function GetSettingsByNameBUForMaintenance(ByVal businessUnit As String, ByVal typeName As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30)
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetSettingsByNameBUForMaintenance")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim sqlStatement As String = String.Empty
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & businessUnit & typeName
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                sqlStatement = "SELECT  [TEMPLATE_TYPE_ID], [NAME], [LOCAL_ROOT_DIRECTORY], [REMOTE_ROOT_DIRECTORY], [MAX_FILE_UPLOAD_SIZE], [ALLOWABLE_FILE_TYPES], [ACTIVE], [SAVE_TO_TALENT], [SAVE_DEFAULTS] FROM [tbl_activity_template_type] WHERE [BUSINESS_UNIT] = @BusinessUnit AND [NAME] = @Name AND [HIDE_IN_MAINTENANCE] = 0"
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Name", typeName))
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

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
