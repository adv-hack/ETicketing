Imports TalentUtilities
Imports System.Threading

Partial Class UserControls_WebSiteDetails
    Inherits System.Web.UI.UserControl

    Private defaults As New TalentDefaults

    Protected Sub btnCreateWebSite_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreateWebSite.Click

        Try

            'Paopulate the paramaters
            Dim parms As New Generic.Dictionary(Of String, String)
            parms.Add("SiteFormat", ddlSiteFormat.SelectedItem.Text.ToString.Trim)
            parms.Add("SiteName", ddlSiteName.SelectedItem.ToString.Trim)
            parms.Add("SiteType", ddlSiteName.SelectedItem.ToString.Trim)
            parms.Add("SiteUrl", txtSiteUrl.Text)
            parms.Add("UpgradePath", txtUpgradePath.Text)
            parms.Add("SslPath", txtSslCertPath.Text)
            parms.Add("ServerName", ddlWebServer.SelectedItem.Text)

            'Create the web site in a new batch
            Dim batchFunc As New TalentBatchFunctions
            batchFunc.Parms = parms
            Dim NewThread As Thread = New Thread(AddressOf batchFunc.CreateWebSite)
            NewThread.Priority = ThreadPriority.Lowest
            NewThread.Start()

            lblCreateWebSite.Text = "Processing the request"

        Catch ex As Exception
            lblCreateWebSite.Text = "Error Calling Server Page"
        End Try

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then

            'Set the website type drop down list
            ddlSiteType.Items.Add(ListItemDef("Live", "Live"))
            ddlSiteType.Items.Add(ListItemDef("Test", "Test"))

            'Set the website type drop down list
            ddlSiteFormat.Items.Add(ListItemDef("Ticketing", "Ticketing"))

            'Load the remaining ddls
            LoadDDLs()

            'Set the required field messages
            rfvSiteUrl.ErrorMessage = "Site Url must be entered"
        End If

    End Sub

    Protected Sub ddlSiteFormat_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSiteFormat.SelectedIndexChanged
        LoadDDLs()
    End Sub

    Private Sub LoadDDLs()

        'Set the client data source
        ddlSiteName.Items.Clear()
        Select Case ddlSiteFormat.SelectedItem.Text
            Case Is = "Ticketing" : ddlSiteName.DataSource = defaults.TicketingClients
        End Select
        ddlSiteName.DataTextField = "CLIENT_NAME"
        ddlSiteName.DataValueField = "IP_ADDRESS"
        ddlSiteName.DataBind()

        'Set the web server drop down list
        ddlWebServer.Items.Clear()
        ddlWebServer.DataSource = defaults.WebServers(ddlSiteFormat.SelectedItem.Text, ddlSiteType.SelectedItem.Text)
        ddlWebServer.DataTextField = "SERVER_NAME"
        ddlWebServer.DataValueField = "IP_ADDRESS"
        ddlWebServer.DataBind()

    End Sub

    Protected Sub ddlSiteType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSiteType.SelectedIndexChanged
        LoadDDLs()
    End Sub
End Class
