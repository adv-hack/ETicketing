Imports System.Data.SqlClient

Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data over tbl_maintenance_modules
    ''' </summary>
    <Serializable()> _
    Public Class tbl_maintenance_modules
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_maintenance_modules"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_maintenance_modules" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Functions"

        ''' <summary>
        ''' Retrieve the active records from the maintenance module table, ordered by sequence
        ''' </summary>
        ''' <param name="cacheing">Optional cache setting, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time setting, default 30 mins</param>
        ''' <returns>A datatable of results</returns>
        ''' <remarks></remarks>
        Public Function GetMaintenanceMenuOptions(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetMaintenanceMenuOptions")
            Dim talentAdminAccessDetail As New TalentDataAccess
            Dim sqlStatement As String = String.Empty
            Try
                'Construct The Call
                talentAdminAccessDetail.Settings = _settings
                talentAdminAccessDetail.Settings.Cacheing = cacheing
                talentAdminAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentAdminAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentAdminAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                sqlStatement = "SELECT * FROM [tbl_maintenance_modules] WHERE [ACTIVE] = 1 ORDER BY [SEQUENCE], [MODULE]"
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

#End Region

    End Class
End Namespace