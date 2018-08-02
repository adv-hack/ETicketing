
Partial Class EmailTemplates_EmailTemplateDetail
    Inherits System.Web.UI.Page

#Region "Class Level Fields"
    Private _businessUnit As String = Utilities.GetBusinessUnit()
    Private _partner As String = Utilities.GetDefaultPartner
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _wfrPage As New Talent.Common.WebFormResource
#End Region

#Region "Constants"
    Private Const STARTSYMBOL As String = "*"
#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _wfrPage
            .BusinessUnit = "MAINTENANCE" 'TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            'added for testing should be removed once Talent.common is updated 
            .PartnerCode = "*ALL" 'TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "EmailTemplateDetail.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = "EmailTemplateDetail.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        pageinstructionsLabel.Text = _wfrPage.Content("pageinstructionsLabel", _languageCode, True)
        pagetitleLabel.Text = _wfrPage.Content("PageTitleUpdateEmailTemplate", _languageCode, True)
        If Not String.IsNullOrWhiteSpace(Session("ClonedEmailTemplateName")) Then
            blErrorList.Items.Add("'" + Session("ClonedEmailTemplateName").ToString() & "' " & _wfrPage.Content("lblCloned", _languageCode, True) & " successfully to '" + Session("ClonedNewEmailTemplateName").ToString + "'")
            Session.Remove("ClonedEmailTemplateName")
            Session.Remove("ClonedNewEmailTemplateName")
        End If
    End Sub
#End Region

End Class