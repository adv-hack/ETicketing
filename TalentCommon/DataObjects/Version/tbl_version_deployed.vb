Imports System.Data.SqlClient
Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_flash_settings based on business functionality
    ''' </summary>
    <Serializable()> _
        Public Class tbl_version_deployed
        Inherits DBObjectBase
#Region "Class Level Fields"
        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings
        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_version_deployed"
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_flash_settings" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"
        ''' <summary>
        ''' Gets the all the Business Units.
        ''' </summary>
        ''' <param name="cacheing">if set to <c>true</c> [cacheing]. Default <c>true</c></param>
        ''' <param name="cacheTimeMinutes">The cache time minutes. Default <c>30</c></param>
        ''' <returns></returns>
        Public Function GetDeployVersion(Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetDeployVersion")
            Dim talentSqlAccessDetail As New TalentDataAccess
            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandText = "Select Top 1 * From TBL_VERSION_DEPLOYED ORDER BY [DATEUPDATED] DESC"
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
