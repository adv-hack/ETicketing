Imports Talent.Common
Partial Class PagesMaint_MaintenancePortal
    Inherits System.Web.UI.Page
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Public wfrPage As New Talent.Common.WebFormResource
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        With wfrPage
            .BusinessUnit = "MAINTENANCE" 'TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            'added for testing should be removed once Talent.common is updated 
            .PartnerCode = "*ALL" 'TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "MaintenancePortal.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = "MaintenancePortal.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
        End With

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        titleLabel.Text = wfrPage.Content("titleLabel", _languageCode, True)
        instructionsLabel.Text = wfrPage.Content("instructionsLabel", _languageCode, True)
    End Sub
End Class
