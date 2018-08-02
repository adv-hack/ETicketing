Imports System.Data
Imports System.Data.SqlClient

Partial Class UserControls_RegisterAccount
    Inherits ControlBase


    Protected _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Protected ucr As New Talent.Common.UserControlResource
    Protected mydefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        mydefaults = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults

        If mydefaults.ShowRegisterAccountOnLoginPage Then
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "RegisterAccount.ascx"
            End With
        Else
            Me.Visible = False
            registerButton.Enabled = False
        End If
    End Sub

    Protected Sub registerButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles registerButton.Click
        Response.Redirect(ucr.Attribute("RegisterURL"))
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            registerButton.Text = ucr.Content("RegisterButtonText", _languageCode, True)
            headerLabel.Text = ucr.Content("RegisterAccountTitleText", _languageCode, True)
            infoLabel.Text = ucr.Content("RegisterAccountInfoText", _languageCode, True)
        End If
    End Sub
End Class
