Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml

<WebService(Namespace:="http://www.talent-sport.co.uk/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class WebSiteFunctions
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function StartWebsite(ByVal websiteName As String, _
                                    ByVal server As String, _
                                    ByVal startAppPool As Boolean, _
                                    ByVal startSite As Boolean, _
                                    ByVal iisWebsiteID As String) As XmlDocument

        Dim xDoc As New XmlDocument

        Dim xRoot As XmlNode = xDoc.CreateElement("StartWebsite")
        Dim xError As XmlNode = xDoc.CreateElement("HasError")
        Dim xErrorText As XmlNode = xDoc.CreateElement("ErrorText")
        Dim xSiteName As XmlNode = xDoc.CreateElement("WebsiteName")
        Dim xServerName As XmlNode = xDoc.CreateElement("ServerName")
        Dim xAppAttempt As XmlNode = xDoc.CreateElement("StartApplicationPoolAttempted")
        Dim xAppSuccess As XmlNode = xDoc.CreateElement("StartApplicationPoolSuccess")
        Dim xAppResp As XmlNode = xDoc.CreateElement("StartApplicationPoolResponse")
        Dim xSiteAttempt As XmlNode = xDoc.CreateElement("StartWebsiteAttempted")
        Dim xSiteSuccess As XmlNode = xDoc.CreateElement("StartWebsiteSuccess")
        Dim xSiteResp As XmlNode = xDoc.CreateElement("StartWebsiteResponse")

        xDoc.AppendChild(xRoot)
        With xRoot
            .AppendChild(xError)
            .AppendChild(xErrorText)
            .AppendChild(xSiteName)
            .AppendChild(xServerName)
            .AppendChild(xAppAttempt)
            .AppendChild(xAppSuccess)
            .AppendChild(xAppResp)
            .AppendChild(xSiteAttempt)
            .AppendChild(xSiteSuccess)
            .AppendChild(xSiteResp)
        End With

        xSiteName.InnerText = websiteName
        xServerName.InnerText = server

        Dim iis As New TalentIISManager

        If String.IsNullOrEmpty(iisWebsiteID) Then
            iis.SiteID = iis.GetWebsiteID(websiteName)
        Else
            iis.SiteID = iisWebsiteID
        End If
        iis.ApplicationPoolName = websiteName

        If Not String.IsNullOrEmpty(iis.SiteID) Then
            If startAppPool Then
                xAppAttempt.InnerText = "True"
                xAppResp.InnerText = iis.StartAppPool
                If xAppResp.InnerText = "Success" Then
                    xAppSuccess.InnerText = "True"
                Else
                    xAppSuccess.InnerText = "False"
                    xError.InnerText = "True"
                    xErrorText.InnerText = "Could not start application pool"
                End If
            Else
                xAppAttempt.InnerText = "False"
            End If
            If startSite AndAlso _
                (String.IsNullOrEmpty(xAppSuccess.InnerText) Or xAppSuccess.InnerText = "True") Then
                xSiteAttempt.InnerText = "True"
                xSiteResp.InnerText = iis.StartWebSite
                If xSiteResp.InnerText = "Success" Then
                    xSiteSuccess.InnerText = "True"
                Else
                    xSiteSuccess.InnerText = "False"
                    If Not xError.InnerText = "True" Then
                        xError.InnerText = "True"
                        xErrorText.InnerText = "Could not start website"
                    End If
                End If
            Else
                xSiteAttempt.InnerText = "False"
            End If
        Else
            xError.InnerText = "True"
            xErrorText.InnerText = "Could not find Site ID for website " & websiteName
        End If


        Return xDoc
    End Function

    <WebMethod()> _
       Public Function StopWebsite(ByVal websiteName As String, _
                                    ByVal server As String, _
                                    ByVal stopAppPool As Boolean, _
                                    ByVal stopSite As Boolean, _
                                    ByVal iisWebsiteID As String) As XmlDocument

        Dim xDoc As New XmlDocument

        Dim xRoot As XmlNode = xDoc.CreateElement("StopWebsite")
        Dim xError As XmlNode = xDoc.CreateElement("HasError")
        Dim xErrorText As XmlNode = xDoc.CreateElement("ErrorText")
        Dim xSiteName As XmlNode = xDoc.CreateElement("WebsiteName")
        Dim xServerName As XmlNode = xDoc.CreateElement("ServerName")
        Dim xAppAttempt As XmlNode = xDoc.CreateElement("StopApplicationPoolAttempted")
        Dim xAppSuccess As XmlNode = xDoc.CreateElement("StopApplicationPoolSuccess")
        Dim xAppResp As XmlNode = xDoc.CreateElement("StopApplicationPoolResponse")
        Dim xSiteAttempt As XmlNode = xDoc.CreateElement("StopWebsiteAttempted")
        Dim xSiteSuccess As XmlNode = xDoc.CreateElement("StopWebsiteSuccess")
        Dim xSiteResp As XmlNode = xDoc.CreateElement("StopWebsiteResponse")

        xDoc.AppendChild(xRoot)
        With xRoot
            .AppendChild(xError)
            .AppendChild(xErrorText)
            .AppendChild(xSiteName)
            .AppendChild(xServerName)
            .AppendChild(xAppAttempt)
            .AppendChild(xAppSuccess)
            .AppendChild(xAppResp)
            .AppendChild(xSiteAttempt)
            .AppendChild(xSiteSuccess)
            .AppendChild(xSiteResp)
        End With

        xSiteName.InnerText = websiteName
        xServerName.InnerText = server

        Dim iis As New TalentIISManager

        If String.IsNullOrEmpty(iisWebsiteID) Then
            iis.SiteID = iis.GetWebsiteID(websiteName)
        Else
            iis.SiteID = iisWebsiteID
        End If
        iis.ApplicationPoolName = websiteName

        If Not String.IsNullOrEmpty(iis.SiteID) Then
            If stopAppPool Then
                xAppAttempt.InnerText = "True"
                xAppResp.InnerText = iis.StopAppPool
                If xAppResp.InnerText = "Success" Then
                    xAppSuccess.InnerText = "True"
                Else
                    xAppSuccess.InnerText = "False"
                    xError.InnerText = "True"
                    xErrorText.InnerText = "Could not Stop application pool"
                End If
            Else
                xAppAttempt.InnerText = "False"
            End If
            If stopSite AndAlso _
                (String.IsNullOrEmpty(xAppSuccess.InnerText) Or xAppSuccess.InnerText = "True") Then
                xSiteAttempt.InnerText = "True"
                xSiteResp.InnerText = iis.StopWebSite
                If xSiteResp.InnerText = "Success" Then
                    xSiteSuccess.InnerText = "True"
                Else
                    xSiteSuccess.InnerText = "False"
                    If Not xError.InnerText = "True" Then
                        xError.InnerText = "True"
                        xErrorText.InnerText = "Could not Stop website"
                    End If
                End If
            Else
                xSiteAttempt.InnerText = "False"
            End If
        Else
            xError.InnerText = "True"
            xErrorText.InnerText = "Could not find Site ID for website " & websiteName
        End If


        Return xDoc
    End Function

    <WebMethod()> _
    Public Function CreateWebSite(ByVal siteFormat As String, _
                                    ByVal siteName As String, _
                                    ByVal siteUrl As String, _
                                    ByVal serverName As String, _
                                    ByVal drive As String, _
                                    ByVal siteType As String, _
                                    ByVal sslCertPath As String, _
                                    ByVal IP1 As String, _
                                    ByVal IP2 As String) As XmlDocument

        Dim xDoc As New XmlDocument

        'Populate the web site properties
        Dim web As New TalentWebSite
        web.SiteFormat = siteFormat
        web.SiteName = siteName
        web.SiteUrl = siteUrl
        web.ServerName = serverName
        web.Drive = drive
        web.SiteType = siteType
        web.SslCertPath = sslCertPath
        web.IP1 = IP1
        web.IP2 = IP2

        'Create the website
        'xDoc = web.CreateWebSite

        Return xDoc

    End Function

End Class
