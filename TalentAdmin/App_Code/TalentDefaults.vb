Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient

Public Class TalentDefaults

    Private _dtAllWebServers As DataTable
    Private _dtLiveTicketingWebServers As DataTable
    Private _dtTestTicketingWebServers As DataTable
    Private _dtTicketingClients As DataTable
    Private _ftpLocalPath As String = String.Empty
    Private _ftpRemotePath As String = String.Empty
    Private _dbAdminXmlPath As String = String.Empty

    Public ReadOnly Property AllWebServers() As DataTable
        Get
            'Is the web server table populated
            If _dtAllWebServers Is Nothing Then
                _dtAllWebServers = RetriveSQLData("AllWebServers", "select * from tbl_web_servers with (nolock) order by client_name")
            End If
            Return _dtAllWebServers
        End Get
    End Property

    Public ReadOnly Property LiveTicketingWebServers() As DataTable
        Get
            'Is the web server table populated
            If _dtLiveTicketingWebServers Is Nothing Then
                _dtLiveTicketingWebServers = RetriveSQLData("LiveTicketingWebServers", "select * from tbl_web_servers with (nolock) where ticketing_server = 'True' and live_server = 'True'")
            End If
            Return _dtLiveTicketingWebServers
        End Get
    End Property

    Public ReadOnly Property TestTicketingWebServers() As DataTable
        Get
            'Is the web server table populated
            If _dtTestTicketingWebServers Is Nothing Then
                _dtTestTicketingWebServers = RetriveSQLData("TestTicketingWebServers", "select * from tbl_web_servers with (nolock) where ticketing_server = 'True' and live_server = 'False'")
            End If
            Return _dtTestTicketingWebServers
        End Get
    End Property

    Public ReadOnly Property TicketingClients() As DataTable
        Get
            'Is the ticketing clients table populated
            If _dtTicketingClients Is Nothing Then
                _dtTicketingClients = RetriveSQLData("TicketingClients", "select * from tbl_client_backend_servers with (nolock) where ticketing_client = 'True'")
            End If
            Return _dtTicketingClients
        End Get
    End Property

    Public ReadOnly Property AllClients() As DataTable
        Get
            'Is the ticketing clients table populated
            If _dtTicketingClients Is Nothing Then
                _dtTicketingClients = RetriveSQLData("AllClients", "select * from tbl_client_backend_servers with (nolock) order by client_name ")
            End If
            Return _dtTicketingClients
        End Get
    End Property

    Public ReadOnly Property ClientsWithVersion(ByVal liveOrTest As String) As DataTable
        Get
            'Is the ticketing clients table populated
            If _dtTicketingClients Is Nothing Then
                _dtTicketingClients = RetriveSQLData("ClientsWithVersion_" & liveOrTest, "select * from tbl_client_backend_servers t1 inner join tbl_client_versions t2 with (nolock) on t1.client_name = t2.client_name where t2.live_or_test = '" & liveOrTest & "' order by t1.client_name ")
            End If
            Return _dtTicketingClients
        End Get
    End Property

    'Public ReadOnly Property ClientsWithVersion(ByVal liveOrTest As String) As DataTable
    '    Get
    '        'Is the ticketing clients table populated
    '        If _dtTicketingClients Is Nothing Then
    '            _dtTicketingClients = RetriveSQLData("ClientsWithVersion_" & liveOrTest, "select * from tbl_client_backend_servers t1 inner join tbl_client_backend_servers_link t2 with (nolock) on t1.client_name = t2.client_name where t2.live_or_test = '" & liveOrTest & "' order by t1.client_name ")
    '        End If
    '        Return _dtTicketingClients
    '    End Get
    'End Property

    Public ReadOnly Property AllClientServerLinks() As DataTable
        Get
            'Is the ticketing clients table populated
            If _dtTicketingClients Is Nothing Then
                _dtTicketingClients = RetriveSQLData("AllClientServerLinks", "select * from tbl_client_web_servers with (nolock) order by client_name, server_name ")
            End If
            Return _dtTicketingClients
        End Get
    End Property

    'Public ReadOnly Property SpecificClientServerLinks(ByVal clientName As String, ByVal liveOrTest As String) As DataTable
    '    Get
    '        'Is the ticketing clients table populated

    '        Dim selectStr As String = "select * " & _
    '                                        " from tbl_client_web_servers t1 " & _
    '                                        " inner join tbl_web_servers t2 with (nolock) " & _
    '                                        " on t1.server_name = t2.server_name " & _
    '                                        " inner join tbl_client_backend_servers t3 with (nolock) " & _
    '                                        " on t1.client_name = t3.client_name " & _
    '                                        " inner join tbl_client_versions t4 with (nolock) " & _
    '                                        " on t1.client_name = t4.client_name " & _
    '                                        " where t1.client_name = '" & clientName & "' " & _
    '                                        " and t1.LIVE_OR_TEST = '" & liveOrTest & "' " & _
    '                                        " and t4.LIVE_OR_TEST = '" & liveOrTest & "' " & _
    '                                        " order by t1.server_name "

    '        If _dtTicketingClients Is Nothing Then
    '            _dtTicketingClients = RetriveSQLData("SpecificClientServerLinks_" & clientName & "_" & liveOrTest, selectStr)
    '        End If
    '        Return _dtTicketingClients
    '    End Get
    'End Property

    Public ReadOnly Property SpecificClientServerLinks(ByVal clientName As String, ByVal liveOrTest As String) As DataTable
        Get
            'Is the ticketing clients table populated

            Dim selectStr As String = "select * " & _
                                            " from tbl_client_web_servers t1 " & _
                                            " inner join tbl_web_servers t2 with (nolock) " & _
                                            " on t1.server_name = t2.server_name " & _
                                            " inner join tbl_client_backend_servers t3 with (nolock) " & _
                                            " on t1.client_name = t3.client_name " & _
                                            " where t1.client_name = '" & clientName & "' " & _
                                            " and t1.LIVE_OR_TEST = '" & liveOrTest & "' " & _
                                            " order by t1.server_name "

            If _dtTicketingClients Is Nothing Then
                _dtTicketingClients = RetriveSQLData("SpecificClientServerLinks_" & clientName & "_" & liveOrTest, selectStr)
            End If
            Return _dtTicketingClients
        End Get
    End Property


    Public ReadOnly Property DBAdminXmlPath() As String
        Get
            If _dbAdminXmlPath.Equals(String.Empty) Then
                _dbAdminXmlPath = RetrieveAdminDefault("DBADMIN_XML_PATH")
            End If
            Return _dbAdminXmlPath
        End Get
    End Property

    Public ReadOnly Property FtpRemotePath() As String
        Get
            If _ftpRemotePath.Equals(String.Empty) Then
                _ftpRemotePath = RetrieveAdminDefault("FTP_REMOTE_PATH")
            End If
            Return _ftpRemotePath
        End Get
    End Property


    Public ReadOnly Property FtpLocalPath() As String
        Get
            If _ftpLocalPath.Equals(String.Empty) Then
                _ftpLocalPath = RetrieveAdminDefault("FTP_LOCAL_PATH")
            End If
            Return _ftpLocalPath
        End Get
    End Property

    Public Function WebServers(ByVal siteFormat As String, ByVal siteType As String) As DataTable

        Dim dt As New DataTable
        Select Case siteFormat
            Case Is = "Ticketing"
                If siteType = "Live" Then
                    dt = LiveTicketingWebServers
                Else
                    dt = TestTicketingWebServers
                End If
        End Select

        Return dt

    End Function

    Public Function WebServerProperties(ByVal serverName As String) As DataRow

        Dim dr As DataRow = AllWebServers.NewRow
        For Each dr In AllWebServers.Rows
            If dr.Item("SERVER_NAME").ToString.Trim.Equals(serverName) Then
                Exit For
            End If
        Next

        Return dr

    End Function

    Public Function RetrieveNextEBServeId(ByVal Client As String) As String

        Const sqlNextIpAddress As String = " select min(ip_address) as IP from tbl_ip_address with (nolock) where client = ' ' "

        Const sqlSetIpAddress As String = "  update tbl_ip_address set client = @Client where ip_address = @IPAddress "

        Dim cmd As New SqlClient.SqlCommand(sqlNextIpAddress, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
        Dim IP As String = String.Empty

        Try
            cmd.Connection.Open()

            Dim dtr As SqlDataReader = Nothing
            dtr = cmd.ExecuteReader()

            If dtr.HasRows Then
                dtr.Read()
                IP = dtr("IP")
                dtr.Close()
                With cmd.Parameters
                    .Clear()
                    .Add("@Client", SqlDbType.NVarChar).Value = Client
                    .Add("@IPAddress", SqlDbType.NVarChar).Value = IP
                End With
                cmd.CommandText = sqlSetIpAddress
                cmd.ExecuteNonQuery()
            End If

        Catch ex As Exception
        Finally
            cmd.Connection.Close()
            cmd.Dispose()
        End Try
        Return IP

    End Function

    Private Function RetrieveAdminDefault(ByVal defaultName As String) As String


        Dim defaultValue As String = String.Empty
        Dim cacheString As String = "AdminDefaults - " & defaultName

        If Not HttpContext.Current.Cache.Item(cacheString) Is Nothing Then
            'Populate the default from cache
            defaultValue = HttpContext.Current.Cache.Item(cacheString)
        Else

            'Retrieve the default from db
            Dim SelectStr As String = "select * from tbl_admin_defaults where default_name = '" & defaultName & "'"
            Dim cmd As New SqlCommand(SelectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Dim ta As New SqlDataAdapter(cmd)

            Try
                'Populate the web server table
                cmd.Connection.Open()
                Dim dtr As SqlDataReader = Nothing
                dtr = cmd.ExecuteReader()

                'Populate the return string
                If dtr.HasRows Then
                    dtr.Read()
                    defaultValue = dtr("VALUE")
                End If

                'Cache the results
                HttpContext.Current.Cache.Insert(cacheString, defaultValue, Nothing, _
                     System.DateTime.Now.AddMinutes(30), Caching.Cache.NoSlidingExpiration)

            Catch ex As Exception
            Finally
                cmd.Connection.Close()
                cmd.Dispose()
                ta.Dispose()
            End Try
        End If

        Return defaultValue

    End Function

    Private Function RetriveSQLData(ByVal cacheString As String, ByVal SQL As String) As DataTable

        Dim dt As New DataTable
        cacheString = "RetriveSQLData - " & cacheString
        If Not HttpContext.Current.Cache.Item(cacheString) Is Nothing Then
            'Populate from cache
            dt = HttpContext.Current.Cache.Item(cacheString)
        Else

            'Setup the sql command
            Dim cmd As New SqlCommand(SQL, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Dim ta As New SqlDataAdapter(cmd)

            Try

                'Populate the data table
                cmd.Connection.Open()
                dt = New DataTable
                ta.Fill(dt)

                'Cache the results
                HttpContext.Current.Cache.Insert(cacheString, dt, Nothing, _
                     System.DateTime.Now.AddMinutes(30), Caching.Cache.NoSlidingExpiration)

            Catch ex As Exception
            Finally
                cmd.Connection.Close()
                cmd.Dispose()
                ta.Dispose()
            End Try

        End If
        Return dt
    End Function

End Class
