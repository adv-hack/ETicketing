Imports Microsoft.VisualBasic
Imports TalentUtilities
Imports System.Data
Imports System.Xml

Public Class TalentBatchFunctions

    Private _parms As Generic.Dictionary(Of String, String)
    Public Property Parms() As Generic.Dictionary(Of String, String)
        Get
            Return _parms
        End Get
        Set(ByVal value As Generic.Dictionary(Of String, String))
            _parms = value
        End Set
    End Property

    Public Sub CreateWebSite()

        Try

            Dim siteFormat As String = Parms.Item("SiteFormat")
            Dim siteName As String = Parms.Item("SiteName")
            Dim siteType As String = Parms.Item("SiteType")
            Dim siteUrl As String = Parms.Item("SiteUrl")
            Dim upgradePath As String = Parms.Item("UpgradePath")
            Dim sslPath As String = Parms.Item("SslPath")
            Dim serverName As String = Parms.Item("ServerName")

            Dim defaults As New TalentDefaults

            Dim sIp As String = defaults.RetrieveNextEBServeId(siteName)

            Dim dRow As DataRow = defaults.WebServerProperties(serverName)

            'Zip the upgrade path up
            Dim zip As New Zip
            zip.ZipThisFolder(upgradePath, defaults.FtpLocalPath & siteName & ".zip")

            'Ftp the directory
            Dim ftpPath As String = "Ftp://" & dRow("IP_ADDRESS") & defaults.FtpRemotePath & siteName & ".zip"
            FTPPutFile(ftpPath, dRow("USER_NAME"), dRow("PASSWORD"), dRow("REMOTE_FTP_PATH") & siteName & ".zip")

            'Call the web service to create the web site on the remote server
            Dim myWSFunctions As New refWebSiteFunctions.WebSiteFunctions
            myWSFunctions.Url = "http://Admin." & serverName & "/Server/WebSite/CreateWebSite.asmx"
            Dim xDoc As XmlDocument = myWSFunctions.CreateWebSite(siteFormat, siteName, siteUrl, serverName, _
                                        dRow("DRIVE"), siteType, sslPath, "", "")

        Catch ex As Exception

        End Try

    End Sub

End Class
