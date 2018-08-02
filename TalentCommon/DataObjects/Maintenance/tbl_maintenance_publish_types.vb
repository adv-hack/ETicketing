Imports System.Data.SqlClient

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data over tbl_maintenance_publish_types
    ''' </summary>
    <Serializable()> _
    Public Class tbl_maintenance_publish_types
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_maintenance_publish_types"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_maintenance_publish_types" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Functions"

        ''' <summary>
        ''' Retrieve the active records from the maintenance publish types table, ordered by sequence
        ''' </summary>
        ''' <param name="cacheing">Optional cache setting, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time setting, default 30 mins</param>
        ''' <returns>A datatable of results</returns>
        ''' <remarks></remarks>
        Public Function GetAll(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetMaintenancePublishTypes")
            Dim talentAdminAccessDetail As New TalentDataAccess
            Dim sqlStatement As String = String.Empty
            Try
                'Construct The Call
                talentAdminAccessDetail.Settings = _settings
                talentAdminAccessDetail.Settings.Cacheing = cacheing
                talentAdminAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentAdminAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentAdminAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                sqlStatement = "SELECT * FROM [tbl_maintenance_publish_types] WHERE [ACTIVE] = 1 ORDER BY [SEQUENCE], [NAME]"
                talentAdminAccessDetail.CommandElements.CommandText = sqlStatement

                'Execute
                Dim err As New ErrorObj
                err = talentAdminAccessDetail.TalentAdminAccess()
                If (Not (err.HasError)) And (Not (talentAdminAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentAdminAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentAdminAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function


        '''
        ''' <summary>
        ''' Retrieve the active records from the maintenance publish types table by BU and Partner, ordered by sequence
        ''' </summary>
        ''' <param name="cacheing">Optional cache setting, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time setting, default 30 mins</param>
        ''' <returns>A datatable of results</returns>
        ''' <remarks></remarks>
        Public Function GetAllByBUPartner(ByVal BU As String, ByVal Partner As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetMaintenancePublishTypesByBusinessUnitAndPartner" + BU)
            Dim talentAdminAccessDetail As New TalentDataAccess
            Dim sqlStatement As String = String.Empty
            Try
                'Construct The Call
                talentAdminAccessDetail.Settings = _settings
                talentAdminAccessDetail.Settings.Cacheing = False
                talentAdminAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentAdminAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentAdminAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                sqlStatement = "SELECT * FROM [tbl_maintenance_publish_types] WHERE [ACTIVE] = 1 and [BUSINESS_UNIT] = '" + BU + "' and [PARTNER_CODE] = '" + Partner + "' ORDER BY [SEQUENCE], [NAME]"
                talentAdminAccessDetail.CommandElements.CommandText = sqlStatement

                'Execute
                Dim err As New ErrorObj
                err = talentAdminAccessDetail.TalentAdminAccess()
                If (Not (err.HasError)) And (Not (talentAdminAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentAdminAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentAdminAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

        '''
        ''' <summary>
        ''' Retrieve the active records from the maintenance publish types table by BU and Partner, ordered by sequence
        ''' </summary>
        ''' <param name="cacheing">Optional cache setting, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time setting, default 30 mins</param>
        ''' <returns>A datatable of results</returns>
        ''' <remarks></remarks>
        Public Function UpdateLastUpdated(ByVal BU As String, ByVal Partner As String, ByVal sType As String, ByVal sLastUpdated As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As Integer
            Dim affectedRows As Integer = 0
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "UpdateMaintenancePublishTypesLastUpdatedByName" + BU)
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim sqlStatement As String = String.Empty
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
                sqlStatement = "UPDATE [tbl_maintenance_publish_types] SET [LAST_UPDATED] = '" + sLastUpdated + "' WHERE [NAME] = '" + sType + "' and [BUSINESS_UNIT] = '" + BU + "' and [PARTNER_CODE] = '" + Partner + "' "
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement

                'Execute
                Dim err As New ErrorObj
                err = talentSqlAccessDetail.SQLAccess
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    affectedRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return affectedRows
        End Function

#End Region

    End Class
End Namespace