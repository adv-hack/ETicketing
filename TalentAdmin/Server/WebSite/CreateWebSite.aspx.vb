
Partial Class _CreateWebSite
    Inherits System.Web.UI.Page

    Private iis As New TalentIISManager
    Private web As New TalentWebSite

    Private Enum AccessPermissionFlags1
        Read = 1
        Write = 2
        Execute = 4
        Source = 16
        Script = 512
        NoRemoteRead = 4096
        NoRemoteWrite = 1024
        NoRemoteExecute = 8192
        NoRemoteScript = 16384
        NoPhysicalDir = 32768
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Retrieve the parameters from the query string
        paramaters()

        If Not iis.Success Then

            'We need to send messages back
            iis.SendMessages = True

            'Create the relevant site
            Select Case web.SiteFormat
                Case Is = "Ticketing" : TicketingSite()
            End Select

        End If

    End Sub

    Public Sub TicketingSite()

        'Create Application Pool
        iis.ApplicationPoolName = web.SiteName
        iis.SetSiteProperty("PeriodicRestartTime", 0, "AppPool")
        iis.SetSiteProperty("PeriodicRestartSchedule", "03:30", "AppPool")
        iis.SetSiteProperty("IdleTimeout", 60, "AppPool")
        iis.SetSiteProperty("AppPoolQueueLength", 1000, "AppPool")
        iis.SetSiteProperty("CPULimit", 75000, "AppPool")
        iis.SetSiteProperty("RapidFailProtectionMaxCrashes", 25, "AppPool")
        'Later use 2 = stop; 1 = start
        'iis.SetSiteProperty("AppPoolCommand", 1, "AppPool")
        iis.CreateApplicationPool()

        'Create Web Site
        iis.SetSiteBinding("", "80", web.ServerName & "." & web.SiteName)
        iis.SetSiteBinding(web.IP1, "80", web.SiteUrl)
        iis.SetSiteBinding(web.IP2, "80", web.SiteUrl)
        iis.WebSiteName = web.SiteName
        iis.WebSitePath = web.Drive & ":\TalentEBusinessSuite\" & web.SiteName & "\" & web.SiteType & "\TalentEBusiness"
        iis.AspVersion = "2.0.50727"
        iis.SetSiteProperty("AppFriendlyName", web.SiteName, "Root")
        iis.SetSiteProperty("AllowKeepAlive", False, "Site")
        iis.SetSiteProperty("DefaultDoc", "default.aspx", "Site")
        iis.SetSiteProperty("EnableDirBrowsing", False, "Site")
        iis.SetSiteProperty("AccessFlags", AccessPermissionFlags1.Read + AccessPermissionFlags1.Script, "Site")
        Dim httpHeaders(2) As String
        httpHeaders(0) = "Pragma: no-cache"
        httpHeaders(1) = "Expires: 0"
        httpHeaders(2) = "Cache-Control"
        iis.SetSiteProperty("HttpCustomHeaders", httpHeaders, "Site", True)
        iis.SetSiteProperty("AppPoolId", web.SiteName, "Site")
        iis.SetSiteProperty("AuthFlags", "1", "Site")
        iis.SetSiteProperty("AuthNTLM", True, "Site")
        iis.CreateWebSite()

        'Create virtual directory for SupplyNet
        iis.VirtualDirectoryName = "SupplyNet"
        iis.SetSiteProperty("Path", web.Drive & ":\TalentEBusinessSuite\" & web.SiteName & "\" & web.SiteType & "\TalentSupplyNet", "VirDir")
        iis.SetSiteProperty("AppFriendlyName", "SupplyNet", "VirDir")
        iis.SetSiteProperty("AccessFlags", AccessPermissionFlags1.Read + AccessPermissionFlags1.Script, "VirDir")
        iis.SetSiteProperty("AccessRead", True, "VirDir")
        iis.SetSiteProperty("EnableDirBrowsing", False, "VirDir")
        iis.SetSiteProperty("AuthNTLM", True, "VirDir")
        iis.SetSiteProperty("AspEnableParentPaths", True, "VirDir")
        iis.CreateVirtualDirectory()

        'Create virtual directory for SupplyNet
        iis.VirtualDirectoryName = "Maintenance"
        iis.SetSiteProperty("Path", web.Drive & ":\TalentEBusinessSuite\" & web.SiteName & "\" & web.SiteType & "\TalentMaintenance", "VirDir")
        iis.SetSiteProperty("AppFriendlyName", "Maintenance", "VirDir")
        iis.SetSiteProperty("AccessFlags", AccessPermissionFlags1.Read + AccessPermissionFlags1.Script, "VirDir")
        iis.SetSiteProperty("AccessRead", True, "VirDir")
        iis.SetSiteProperty("EnableDirBrowsing", False, "VirDir")
        iis.SetSiteProperty("AuthNTLM", True, "VirDir")
        iis.SetSiteProperty("AspEnableParentPaths", True, "VirDir")
        iis.CreateVirtualDirectory()

        'Create virtual directory for Assets
        iis.VirtualDirectoryName = "Assets"
        iis.SetSiteProperty("Path", web.Drive & ":\TalentEBusinessSuite\" & web.SiteName & "\" & web.SiteType & "\TalentEBusiness\Assets", "VirDir")
        iis.SetSiteProperty("AppFriendlyName", "Assets", "VirDir")
        iis.SetSiteProperty("AccessFlags", AccessPermissionFlags1.Read, "VirDir")
        iis.SetSiteProperty("AccessRead", True, "VirDir")
        iis.SetSiteProperty("EnableDirBrowsing", False, "VirDir")
        iis.SetSiteProperty("AuthNTLM", True, "VirDir")
        iis.SetSiteProperty("AspEnableParentPaths", True, "VirDir")
        iis.VirDirCreate = False
        iis.SetSiteProperty("HttpCustomHeaders", "X-Powered-By: ASP.NET", "VirDir")
        iis.CreateVirtualDirectory()

        'Set Directory Properties For JavaScript
        iis.DirectoryName = "JavaScript"
        iis.SetSiteProperty("HttpCustomHeaders", "X-Powered-By: ASP.NET", "Dir")
        iis.SetDirectoryProperties()

        'Set Directory Properties For App_Themes
        iis.DirectoryName = "App_Themes"
        iis.SetSiteProperty("HttpCustomHeaders", "X-Powered-By: ASP.NET", "Dir")
        iis.SetDirectoryProperties()

        'Set Directory Properties For Flash
        iis.DirectoryName = "Flash"
        iis.SetSiteProperty("HttpCustomHeaders", "X-Powered-By: ASP.NET", "Dir")
        iis.SetDirectoryProperties()

        'Install the ssl certfificate
        If Not web.SslCertPath.Trim.Equals("") Then
            iis.SetSiteBinding(web.IP1, "443", "")
            iis.SetSiteBinding(web.IP2, "443", "")
            iis.SslPfxPath = web.SslCertPath
            iis.SslPassword = "csg"
            iis.InstallSSLCertificate()
        End If

        'Start the web site
        iis.StartWebSite()

        'Send the completion message
        If iis.Success Then
            iis.SendMessage("The website was successfully created.", False)
        Else
            iis.SendMessage("There was problems creating the web site.  Please check previously listed errors.", False)
        End If


    End Sub

    Public Sub Paramaters()

        'Retrieve the site name parameter
        If Not String.IsNullOrEmpty(Request("siteFormat")) Then
            web.SiteFormat = Request("siteFormat")
        Else
            iis.SendMessage("The web site format was missing in the request", True)
        End If

        'Retrieve the site name parameter
        If Not String.IsNullOrEmpty(Request("siteName")) Then
            web.SiteName = Request("siteName")
        Else
            iis.SendMessage("The web site name was missing in the request", True)
        End If

        'Retrieve the url parameter
        If Not String.IsNullOrEmpty(Request("siteUrl")) Then
            web.SiteUrl = Request("siteUrl")
        Else
            iis.SendMessage("The web site url was missing in the request", True)
        End If

        'Retrieve the server name parameters
        If Not String.IsNullOrEmpty(Request("serverName")) Then
            web.ServerName = Request("serverName")
        Else
            iis.SendMessage("The server name was missing in the request", True)
        End If

        'Retrieve the drive parameters
        If Not String.IsNullOrEmpty(Request("drive")) Then
            web.Drive = Request("drive")
        Else
            iis.SendMessage("The drive letter was missing in the request", True)
        End If

        'Retrieve the site type
        If Not String.IsNullOrEmpty(Request("siteType")) Then
            web.SiteType = Request("siteType")
        Else
            iis.SendMessage("The site type was missing in the request", True)
        End If

        'Retrieve the site type
        If Not String.IsNullOrEmpty(Request("sslCertPath")) Then
            web.SslCertPath = Request("sslCertPath")
        End If

        'Retrieve the ip address 1
        If Not String.IsNullOrEmpty(Request("IP1")) Then
            web.IP1 = Request("IP1")
        End If

        'Retrieve the ip address 2
        If Not String.IsNullOrEmpty(Request("IP2")) Then
            web.IP2 = Request("IP2")
        End If
    End Sub
End Class
