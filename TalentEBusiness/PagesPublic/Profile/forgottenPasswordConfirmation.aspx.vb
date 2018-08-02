Imports Talent.eCommerce

Partial Class PagesLogin_forgottenPasswordConfirmation
    Inherits TalentBase01

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Dim wfr As New Talent.Common.WebFormResource
        Dim languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
        With wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "forgottenPasswordConfirmation.aspx"
            MessageLabel.Text = .Content("MessageText", languageCode, True)
            plhMessage.Visible = (MessageLabel.Text.Length > 0)
        End With
    End Sub
End Class
