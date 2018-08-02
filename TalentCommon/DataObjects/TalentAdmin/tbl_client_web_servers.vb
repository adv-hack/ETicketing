Imports System.Data.SqlClient
Imports System.Text


Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to manage data over tbl_client_web_servers
    ''' </summary>
    <Serializable()> _
    Public Class tbl_client_web_servers
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_client_web_servers"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_client_web_servers" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Functions"

        ''' <summary>
        ''' Retrieve the web server data based on the given client name and optional live or test value
        ''' </summary>
        ''' <param name="clientName">The given client name as a string</param>
        ''' <param name="liveOrTest">The live or test environment setting, optional, if blank both live and test are returned</param>
        ''' <param name="cacheing">Optional cache setting, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time setting, default 30 mins</param>
        ''' <returns>A datatable of results</returns>
        ''' <remarks></remarks>
        Public Function GetWebServerDataByClientName(ByVal clientName As String, Optional ByVal liveOrTest As String = "", _
                                                     Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetWebServerDataByClientName")
            Dim talentAdminAccessDetail As New TalentDataAccess
            Dim sqlStatement As String = String.Empty
            Try
                'Construct The Call
                talentAdminAccessDetail.Settings = _settings
                talentAdminAccessDetail.Settings.Cacheing = cacheing
                talentAdminAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentAdminAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentAdminAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentAdminAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ClientName", clientName))
                sqlStatement = "SELECT * FROM [tbl_client_web_servers] WHERE [CLIENT_NAME]=@ClientName"
                If Not String.IsNullOrWhiteSpace(liveOrTest) Then
                    talentAdminAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LiveOrTest", liveOrTest))
                    sqlStatement = sqlStatement & " AND [LIVE_OR_TEST]=@LiveOrTest"
                End If
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

        ''' <summary>
        ''' Gets the server details using the client name and optional live or test value.
        ''' </summary>
        ''' <param name="clientName">The given client name as a string</param>
        ''' <param name="liveOrTest">The live or test environment setting, optional, if blank both live and test are returned</param>
        ''' <param name="cacheing">Optional cache setting, default true</param>
        ''' <param name="cacheTimeMinutes">Optional cache time setting, default 30 mins</param>
        ''' <returns>A datatable of results</returns>
        ''' <remarks>This is a new function for Gift Vouchers utility</remarks>
        Public Function GetServerDetailsByClientName(ByVal clientName As String, Optional ByVal liveOrTest As String = "", _
                                                     Optional ByVal cacheing As Boolean = True, Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetServerDetailsByClientName")
            Dim talentAdminAccessDetail As New TalentDataAccess
            Dim sqlStatement As StringBuilder = New StringBuilder()
            Try
                'Construct The Call
                talentAdminAccessDetail.Settings = _settings
                talentAdminAccessDetail.Settings.Cacheing = cacheing
                talentAdminAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentAdminAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix
                talentAdminAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentAdminAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@ClientName", clientName))
                sqlStatement.Append("SELECT A.SERVER_NAME,A.IP_ADDRESS FROM tbl_servers A ")
                sqlStatement.Append("JOIN tbl_client_web_servers B ON A.SERVER_NAME = B.SERVER_NAME ")
                sqlStatement.Append("WHERE B.[CLIENT_NAME] = @ClientName")

                If Not String.IsNullOrWhiteSpace(liveOrTest) Then
                    talentAdminAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LiveOrTest", liveOrTest))
                    sqlStatement.Append(" AND B.[LIVE_OR_TEST] = @LiveOrTest")
                End If
                talentAdminAccessDetail.CommandElements.CommandText = sqlStatement.ToString()

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