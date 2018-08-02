
Partial Class UserControls_RecommendAFriend
    Inherits ControlBase

    Private ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        recommendLink.NavigateUrl = "~/PagesPublic/ProductBrowse/RecommendProduct.aspx?" & Request.QueryString.ToString
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "RecommendAFriend.ascx"
        End With
        recommendLink.Text = ucr.Content("LinkText", _languageCode, True)
    End Sub

End Class
