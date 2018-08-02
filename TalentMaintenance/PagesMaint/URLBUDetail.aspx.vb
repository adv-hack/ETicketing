Imports System.Data
Imports System.Collections.Generic
Imports Talent.Common
Partial Class PagesMaint_URLBUDetail
    Inherits System.Web.UI.Page
    Private _businessUnit As String = Utilities.GetBusinessUnit()
    Private _partner As String = Utilities.GetDefaultPartner
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _wfrPage As New Talent.Common.WebFormResource
    Private Const STARTSYMBOL As String = "*"

#Region "Protected Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _wfrPage
            .BusinessUnit = "MAINTENANCE" 'TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            'added for testing should be removed once Talent.common is updated 
            .PartnerCode = "*ALL" 'TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "URLBUDetail.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = "URLBUDetail.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        pageinstructionsLabel.Text = _wfrPage.Content("pageinstructionsLabel", _languageCode, True)
        pagetitleLabel.Text = _wfrPage.Content("PageTitleUpdateEmailTemplate", _languageCode, True)

        If pagetitleLabel.Text = String.Empty Then pagetitleLabel.Text = "Business Unit URL Detail"
    End Sub
#End Region

End Class