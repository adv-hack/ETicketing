Imports TalentUtilities
Imports System.Threading

Partial Class UserControls_IseriesSetup
    Inherits System.Web.UI.UserControl

    Private defaults As New TalentDefaults

    Protected Sub btnCreateWebSite_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfigureISeries.Click

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

    End Sub

    Protected Sub ddlSiteType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSiteType.SelectedIndexChanged
        LoadDDLs()
    End Sub
End Class
