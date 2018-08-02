Imports MaintenancePortalDataSetTableAdapters
Imports Talent.Common

Partial Class Navigation_AdhocGroupMaintenance
    Inherits System.Web.UI.Page
    Dim objBUTableAdapter As New BUSINESS_UNIT_TableAdapter
    Dim objPTableAdapter As New PARTNER_TableAdapter
    Public wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Dim objGroup1 As New NavigationDataSetTableAdapters.tbl_groupTableAdapter

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With wfrPage
            .BusinessUnit = "MAINTENANCE"
            .PageCode = String.Empty
            .PartnerCode = "*ALL"
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "AdhocGroupMaintenance.aspx"
            .PageCode = "AdhocGroupMaintenance.aspx"
        End With
        ObjectDataSource1.SelectMethod = "GetAdhocGroups"
        titleLabel.Text = "Maintain Adhoc Groups"
    End Sub

    Public Sub setLabel()
        'instructionsLabel.Text = wfrPage.Content("instructionsLabel", _languageCode, True)
        'pageLabel.Text = wfrPage.Content("selectPageLabel", _languageCode, True)
        'AddNewPageButton.Text = wfrPage.Content("AddNewPageButton", _languageCode, True)

        Dim liGroupHomeLink As New ListItem
        liGroupHomeLink.Value = "NavigationMaintenance.aspx?BU="
        liGroupHomeLink.Value += Request.QueryString("BU")
        liGroupHomeLink.Value += "&partner="
        liGroupHomeLink.Value += Request.QueryString("partner")
        liGroupHomeLink.Text = "Navigation Overview"
        navigationOptions.Items.Add(liGroupHomeLink)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Group Navigation Maintenance - Maintain Adhoc Groups"
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
        master.HeaderText = "Group Navigation Maintenance"
        If String.IsNullOrEmpty(Request.QueryString("BU")) Then
            Response.Redirect("../MaintenancePortal.aspx")
        End If

        If String.IsNullOrEmpty(Request.QueryString("partner")) Then
            Response.Redirect("../MaintenancePortal.aspx")
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("status")) Then
            Dim groupStatus As String
            groupStatus = Request.QueryString("status")
            Select Case groupStatus
                Case "added"
                    ErrorLabel.Text = "The group was successfully added"
                Case "updated"
                    ErrorLabel.Text = "The group was successfully updated"
                Case "deleted"
                    ErrorLabel.Text = "The group was successfully deleted"
                Case Else
                    ErrorLabel.Text = ""
            End Select
        End If

        If IsPostBack = False Then
            setLabel()
            fillPageDDL()
        End If

    End Sub

    Public Sub fillPageDDL()
        PageDDL.DataSource = objGroup1.GetAdhocGroups()
        PageDDL.DataTextField = "GROUP_NAME"
        PageDDL.DataValueField = "GROUP_ID"
        PageDDL.DataBind()
        PageDDL.Items.Insert(0, " -- ")
    End Sub

    Protected Sub AddNewPageButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AddNewPageButton.Click
        Response.Redirect("AdhocGroupDetails.aspx?Status=New&partner=" + Request.QueryString("partner") + "&BU=" + Request.QueryString("BU"))
    End Sub

    Protected Sub PageDDL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles PageDDL.SelectedIndexChanged
        Response.Redirect("AdhocGroupDetails.aspx?GroupId=" + PageDDL.SelectedValue + "&GroupName=" + PageDDL.SelectedItem.Text + "&partner=" + Request.QueryString("partner") + "&BU=" + Request.QueryString("BU"))
    End Sub

    Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelButton.Click
        Response.Redirect("NavigationMaintenance.aspx?partner=" + Request.QueryString("partner") + "&BU=" + Request.QueryString("BU"))
    End Sub

End Class
