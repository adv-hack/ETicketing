Imports Talent.Common.DataObjects.TableObjects
Imports System.Text

Namespace DataObjects
    ''' <summary>
    ''' Class provides the functionality to access Talent Admin Database
    ''' </summary>
    <Serializable()> _
    Public Class TalentAdmin
        Inherits DBObjectBase
#Region "Class Level Fields"

        ''' <summary>
        ''' DESettings Instance
        ''' </summary>
        Private _settings As New DESettings

        Private _tblClientWebServers As tbl_client_web_servers

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "TalentAdmin"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="TalentAdmin" /> class.
        ''' </summary>
        ''' <param name="settings">DESettings Instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Properties"

        ''' <summary>
        ''' Create and Gets the tbl_page instance with DESettings
        ''' </summary>
        ''' <value></value>
        Public ReadOnly Property TblClientWebServers() As tbl_client_web_servers
            Get
                If (_tblClientWebServers Is Nothing) Then
                    _tblClientWebServers = New tbl_client_web_servers(_settings)
                End If
                Return _tblClientWebServers
            End Get
        End Property

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Get the host server name based on the given client details
        ''' </summary>
        ''' <param name="clientName">The given client name</param>
        ''' <param name="liveOrTest">Is this a live or test environment</param>
        ''' <param name="cacheing">Cache value, default true</param>
        ''' <param name="cacheTimeMinutes">Cache time, default 30 mins</param>
        ''' <returns>The server details table</returns>
        ''' <remarks></remarks>
        Public Function GetHostServerDetailsByClientName(ByVal clientName As String, ByVal liveOrTest As String, _
                                                         Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim err As New ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetHostServerDetailsByClientName")
            Try
                Dim sqlStatement As New StringBuilder

                sqlStatement.Append("SELECT * FROM tbl_servers WHERE SERVER_NAME IN (")
                sqlStatement.Append("SELECT SERVER_NAME FROM tbl_client_web_servers WHERE CLIENT_NAME=@ClientName AND LIVE_OR_TEST=@LiveOrTest")
                sqlStatement.Append(")")

                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & clientName & liveOrTest
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ClientName", clientName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LiveOrTest", liveOrTest))
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                err = talentSqlAccessDetail.TalentAdminAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return outputDataTable
        End Function

        ''' <summary>
        ''' Retrieve and format a list of connection strings for the client based on the databases setup
        ''' </summary>
        ''' <param name="clientName">The given client name</param>
        ''' <param name="liveOrTest">Is this a live or test environment</param>
        ''' <param name="cacheing">Cache value, default true</param>
        ''' <param name="cacheTimeMinutes">Cache time, default 30 mins</param>
        ''' <returns>Formatted list of connection strings</returns>
        ''' <remarks></remarks>
        Public Function GetConnectionStringListForMultiDB(ByVal clientName As String, ByVal liveOrTest As String, Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As List(Of String)
            Dim connectionStringTemplate As String = "Data Source={0};Initial Catalog={1}; User ID={2}; password={3};"
            Dim connectionStringList As New List(Of String)
            Dim outputDataTable As New DataTable
            Dim err As New ErrorObj
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim sqlStatement As New StringBuilder
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetConnectionStringListForMultiDB")
            Try
                sqlStatement.Append("SELECT DISTINCT [SQL_SERVER_NAME], [SQL_DATABASE_NAME], [SQL_USER], [SQL_PASSWORD] ")
                sqlStatement.Append("FROM [tbl_client_web_servers] ")
                sqlStatement.Append("WHERE [CLIENT_NAME] = @ClientName ")
                sqlStatement.Append("AND [LIVE_OR_TEST] = @LiveOrTest")

                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & clientName & liveOrTest
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ClientName", clientName))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LiveOrTest", liveOrTest))
                talentSqlAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

                'Execute
                err = talentSqlAccessDetail.TalentAdminAccess()
                If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                    outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                End If

                For Each row As DataRow In outputDataTable.Rows
                    Dim connectionString As String = String.Empty
                    connectionString = String.Format(connectionStringTemplate, row("SQL_SERVER_NAME"), row("SQL_DATABASE_NAME"), row("SQL_USER"), row("SQL_PASSWORD"))
                    connectionStringList.Add(connectionString)
                Next
            Catch ex As Exception
                Throw
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            Return connectionStringList
        End Function

#End Region

    End Class
End Namespace