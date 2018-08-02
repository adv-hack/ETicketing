
Partial Class UserControls_UpdateDetailsConfirmation_AddAutoMembership
    Inherits ControlBase

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private ucr As New Talent.Common.UserControlResource

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            If Not String.IsNullOrEmpty(Request.QueryString("addauto")) _
                AndAlso Request.QueryString("addauto").ToLower = "false" Then

                With ucr
                    .BusinessUnit = TalentCache.GetBusinessUnit
                    .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                    .PageCode = Talent.Common.Utilities.GetAllString 'GetCurrentPageName()
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    .KeyCode = "UpdateDetailsConfirmation_AddAutoMembership.ascx"
                End With

                With ucr

                    If Not Profile.User.Details.OWNS_AUTO_MEMBERSHIP _
                        AndAlso Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(.Attribute("AddMembership_ShowAddAutoMembership")) Then
                        Me.addMembershipCheck.Checked = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(.Attribute("AddMembership_CheckedByDefault"))
                        Me.titleLabel.Text = .Content("AddMembership_TitleLabel", _languageCode, True)
                        Me.contentLabel.Text = .Content("AddMembership_ContentLabel", _languageCode, True)
                        Me.addMembershipCheck.Text = .Content("AddMembership_CheckboxLabel", _languageCode, True)
                        Me.returnButton.Text = .Content("AddMembership_ReturnToHomepageButtonText", _languageCode, True)
                        Me.addButton.Text = .Content("AddMembership_AddAutoMembershipButtonText", _languageCode, True)
                    Else
                        Me.Visible = False
                    End If
                End With

            Else
                Me.Visible = False
            End If

        End If
    End Sub

    Protected Sub returnButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles returnButton.Click
        Response.Redirect(Talent.eCommerce.Utilities.GetSiteHomePage())
    End Sub

    Protected Sub addButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles addButton.Click
        If Me.addMembershipCheck.Checked Then
            Response.Redirect("~/Redirect/TicketingGateway.aspx?page=Registration.aspx&function=AddFreeMembership")
        End If
    End Sub
End Class
