Imports Talent.Common
Imports System.Data

Partial Class Navigation_AdhocGroupDetails
    Inherits System.Web.UI.Page
    Public wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Public objGroup As New NavigationDataSetTableAdapters.tbl_groupTableAdapter

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With wfrPage
            .BusinessUnit = "MAINTENANCE"
            .PageCode = String.Empty
            'added for testing should be removed once Talent.common is updated 
            .PartnerCode = "*ALL"
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "AdhocGroupDetails.aspx"
            .PageCode = "AdhocGroupDetails.aspx"
        End With
        setLabel()
    End Sub

    Public Sub setLabel()
        Dim liGroupHomeLink As New ListItem
        liGroupHomeLink.Value = "NavigationMaintenance.aspx?BU="
        liGroupHomeLink.Value += Request.QueryString("BU")
        liGroupHomeLink.Value += "&partner="
        liGroupHomeLink.Value += Request.QueryString("partner")
        liGroupHomeLink.Text = "Navigation Overview"
        navigationOptions.Items.Add(liGroupHomeLink)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request("groupName") Is Nothing Then
            Page.Title = "Group Navigation Maintenance - Add Adhoc Group"
        Else
            Page.Title = "Group Navigation Maintenance - Change Adhoc Group"
        End If
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
        master.HeaderText = "Group Navigation Maintenance"

        If IsPostBack = False Then

            If String.IsNullOrEmpty(Request.QueryString("BU")) Then
                Response.Redirect("../MaintenancePortal.aspx")
            End If

            If String.IsNullOrEmpty(Request.QueryString("Partner")) Then
                Response.Redirect("../MaintenancePortal.aspx")
            End If

            If Request.QueryString("Status") = "New" Then
                GroupNameTextBox.ReadOnly = False
                titleLabel.Text = "Add Group"
                btnDeleteGroup.Visible = False
            Else
                GroupNameTextBox.ReadOnly = True
                btnDeleteGroup.Visible = True
                fillData()
            End If

        End If
    End Sub

    Public Sub fillData()
        Dim dt As New DataTable
        dt = objGroup.GetDataById(Convert.ToInt32(Request.QueryString("GroupId")))
        If dt.Rows.Count = 0 Then
            ErrorLabel.Text = "This group does not exist."
        Else
            titleLabel.Text = "Change Group Details: " + dt.Rows(0).Item("GROUP_NAME")
            GroupNameTextBox.Text = dt.Rows(0).Item("GROUP_NAME").ToString
            Description1TextBox.Text = dt.Rows(0).Item("GROUP_DESCRIPTION_1").ToString
            Description2TextBox.Text = dt.Rows(0).Item("GROUP_DESCRIPTION_2").ToString
            Html1TextBox.Text = dt.Rows(0).Item("GROUP_HTML_1").ToString
            Html2TextBox.Text = dt.Rows(0).Item("GROUP_HTML_2").ToString
            Html3TextBox.Text = dt.Rows(0).Item("GROUP_HTML_3").ToString
            PageTitleTextBox.Text = dt.Rows(0).Item("GROUP_PAGE_TITLE").ToString
            MetaDescTextBox.Text = dt.Rows(0).Item("GROUP_META_DESCRIPTION").ToString
            MetaKeysTextBox.Text = dt.Rows(0).Item("GROUP_META_KEYWORDS").ToString
        End If
    End Sub

    Protected Sub ConfirmButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ConfirmButton.Click
        If Request.QueryString("status") = "New" Then
            Dim checkVal As Integer
            checkVal = objGroup.CheckExistsByGroupName(GroupNameTextBox.Text)
            If checkVal = 0 Then
                Try
                    Dim retValInsert As Integer
                    retValInsert = objGroup.InsertQuery(GroupNameTextBox.Text, Description1TextBox.Text, Description2TextBox.Text, Html1TextBox.Text, Html2TextBox.Text, _
                                                        Html3TextBox.Text, PageTitleTextBox.Text, MetaDescTextBox.Text, MetaKeysTextBox.Text, True)
                    If retValInsert > 0 Then
                        'ErrorLabel.Text = "Group added succesfully"
                        Response.Redirect("AdhocGroupMaintenance.aspx?BU=" + Request.QueryString("BU") + "&partner=" + Request.QueryString("partner") + "&status=added")
                    Else
                        ErrorLabel.Text = "Failed to add group"
                    End If
                Catch ex As Exception
                    ErrorLabel.Text = wfrPage.Content("UnScc", _languageCode, True)
                End Try
            Else
                ErrorLabel.Text = "Failed to add - group already exists"
            End If
        Else
            Try
                Dim retVal As Integer
                retVal = objGroup.UpdateByID(GroupNameTextBox.Text, Description1TextBox.Text, Description2TextBox.Text, Html1TextBox.Text, Html2TextBox.Text, _
                                                        Html3TextBox.Text, PageTitleTextBox.Text, MetaDescTextBox.Text, MetaKeysTextBox.Text, True, CInt(Request("GroupId")))
                If retVal = 1 Then
                    'ErrorLabel.Text = "Group updated successfully"
                    Response.Redirect("AdhocGroupMaintenance.aspx?BU=" + Request.QueryString("BU") + "&partner=" + Request.QueryString("partner") + "&status=updated")
                Else
                    ErrorLabel.Text = "Failed to update group"
                End If
            Catch ex As Exception
                ErrorLabel.Text = "Error when updating - " & ex.Message
            End Try
        End If
    End Sub

    Protected Sub ReturnToGroups_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ReturnToGroupsButton.Click
        Response.Redirect("AdhocGroupMaintenance.aspx?BU=" + Request.QueryString("BU") + "&partner=" + Request.QueryString("partner"))
    End Sub

    Protected Sub btnDeleteGroup_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteGroup.Click
        Try
            Dim retVal As Integer
            retVal = objGroup.Delete(GroupNameTextBox.Text)
            If retVal > 0 Then
                Response.Redirect("AdhocGroupMaintenance.aspx?BU=" + Request.QueryString("BU") + "&partner=" + Request.QueryString("partner") + "&status=deleted")
            Else
                ErrorLabel.Text = "Group Not Deleted"
            End If
        Catch ex As Exception
            ErrorLabel.Text = "Error Deleting Group - " + ex.Message
        End Try
    End Sub
End Class
