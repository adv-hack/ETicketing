Imports Microsoft.VisualBasic
Imports System.Xml

Public Class TalentWebSite

    Private _siteFormat As String = String.Empty
    Private _siteName As String = String.Empty
    Private _siteUrl As String = String.Empty
    Private _serverName As String = String.Empty
    Private _drive As String = String.Empty
    Private _siteType As String = String.Empty
    Private _sslCertPath As String = String.Empty
    Private _IP1 As String = String.Empty
    Private _IP2 As String = String.Empty

    Public Property SiteFormat() As String
        Get
            Return _siteFormat
        End Get
        Set(ByVal value As String)
            _siteFormat = value
        End Set
    End Property

    Public Property SiteName() As String
        Get
            Return _siteName
        End Get
        Set(ByVal value As String)
            _siteName = value
        End Set
    End Property

    Public Property SiteUrl() As String
        Get
            Return _siteUrl
        End Get
        Set(ByVal value As String)
            _siteUrl = value
        End Set
    End Property

    Public Property ServerName() As String
        Get
            Return _serverName
        End Get
        Set(ByVal value As String)
            _serverName = value
        End Set
    End Property

    Public Property Drive() As String
        Get
            Return _drive
        End Get
        Set(ByVal value As String)
            _drive = value
        End Set
    End Property

    Public Property SiteType() As String
        Get
            Return _siteType
        End Get
        Set(ByVal value As String)
            _siteType = value
        End Set
    End Property

    Public Property SslCertPath() As String
        Get
            Return _sslCertPath
        End Get
        Set(ByVal value As String)
            _sslCertPath = value
        End Set
    End Property

    Public Property IP1() As String
        Get
            Return _IP1
        End Get
        Set(ByVal value As String)
            _IP1 = value
        End Set
    End Property

    Public Property IP2() As String
        Get
            Return _IP2
        End Get
        Set(ByVal value As String)
            _IP2 = value
        End Set
    End Property

    Private Enum AccessPermissionFlags
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

    Public Function CreateWebSite() As XmlDocument

        Dim xDoc As New XmlDocument

        'Create the relevant site
        Select Case SiteFormat
            Case Is = "Ticketing" : TicketingWebSite()
        End Select

        Return xDoc

    End Function

    Public Function TicketingWebSite() As XmlDocument

        Dim iis As New TalentIISManager
        Dim xDoc As New XmlDocument

        'Create Application Pool
        iis.ApplicationPoolName = SiteName
        iis.SetSiteProperty("PeriodicRestartTime", 0, "AppPool")
        iis.SetSiteProperty("PeriodicRestartSchedule", "03:30", "AppPool")
        iis.SetSiteProperty("IdleTimeout", 60, "AppPool")
        iis.SetSiteProperty("AppPoolQueueLength", 1000, "AppPool")
        iis.SetSiteProperty("CPULimit", 75000, "AppPool")
        iis.SetSiteProperty("RapidFailProtectionMaxCrashes", 25, "AppPool")
        iis.CreateApplicationPool()

        'Create Web Site
        iis.SetSiteBinding("", "80", ServerName & "." & SiteName)
        If Not IP1.Trim.Equals("") Then
            iis.SetSiteBinding(IP1, "80", SiteUrl)
        End If
        If Not IP2.Trim.Equals("") Then
            iis.SetSiteBinding(IP2, "80", SiteUrl)
        End If
        iis.WebSiteName = SiteName
        iis.WebSitePath = Drive & ":\TalentEBusinessSuite\" & SiteName & "\" & SiteType & "\TalentEBusiness"
        iis.AspVersion = "2.0.50727"
        iis.SetSiteProperty("AppFriendlyName", SiteName, "Root")
        iis.SetSiteProperty("AllowKeepAlive", False, "Site")
        iis.SetSiteProperty("DefaultDoc", "default.aspx", "Site")
        iis.SetSiteProperty("EnableDirBrowsing", False, "Site")
        iis.SetSiteProperty("AccessFlags", AccessPermissionFlags.Read + AccessPermissionFlags.Script, "Site")
        Dim httpHeaders(2) As String
        httpHeaders(0) = "Pragma: no-cache"
        httpHeaders(1) = "Expires: 0"
        httpHeaders(2) = "Cache-Control"
        iis.SetSiteProperty("HttpCustomHeaders", httpHeaders, "Site", True)
        iis.SetSiteProperty("AppPoolId", SiteName, "Site")
        iis.SetSiteProperty("AuthFlags", "1", "Site")
        iis.SetSiteProperty("AuthNTLM", True, "Site")
        iis.CreateWebSite()

        'Create virtual directory for SupplyNet
        iis.VirtualDirectoryName = "SupplyNet"
        iis.SetSiteProperty("Path", Drive & ":\TalentEBusinessSuite\" & SiteName & "\" & SiteType & "\TalentSupplyNet", "VirDir")
        iis.SetSiteProperty("AppFriendlyName", "SupplyNet", "VirDir")
        iis.SetSiteProperty("AccessFlags", AccessPermissionFlags.Read + AccessPermissionFlags.Script, "VirDir")
        iis.SetSiteProperty("AccessRead", True, "VirDir")
        iis.SetSiteProperty("EnableDirBrowsing", False, "VirDir")
        iis.SetSiteProperty("AuthNTLM", True, "VirDir")
        iis.SetSiteProperty("AspEnableParentPaths", True, "VirDir")
        iis.CreateVirtualDirectory()

        'Create virtual directory for SupplyNet
        iis.VirtualDirectoryName = "Maintenance"
        iis.SetSiteProperty("Path", Drive & ":\TalentEBusinessSuite\" & SiteName & "\" & SiteType & "\TalentMaintenance", "VirDir")
        iis.SetSiteProperty("AppFriendlyName", "Maintenance", "VirDir")
        iis.SetSiteProperty("AccessFlags", AccessPermissionFlags.Read + AccessPermissionFlags.Script, "VirDir")
        iis.SetSiteProperty("AccessRead", True, "VirDir")
        iis.SetSiteProperty("EnableDirBrowsing", False, "VirDir")
        iis.SetSiteProperty("AuthNTLM", True, "VirDir")
        iis.SetSiteProperty("AspEnableParentPaths", True, "VirDir")
        iis.CreateVirtualDirectory()

        'Create virtual directory for Assets
        iis.VirtualDirectoryName = "Assets"
        iis.SetSiteProperty("Path", Drive & ":\TalentEBusinessSuite\" & SiteName & "\" & SiteType & "\TalentEBusiness\Assets", "VirDir")
        iis.SetSiteProperty("AppFriendlyName", "Assets", "VirDir")
        iis.SetSiteProperty("AccessFlags", AccessPermissionFlags.Read, "VirDir")
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
        If Not SslCertPath.Trim.Equals("") Then
            iis.SetSiteBinding(IP1, "443", "")
            iis.SetSiteBinding(IP2, "443", "")
            iis.SslPfxPath = SslCertPath
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

        Return xDoc

    End Function

End Class
