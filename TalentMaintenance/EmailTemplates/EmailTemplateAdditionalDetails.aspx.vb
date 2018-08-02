
Partial Class EmailTemplates_EmailTemplateAdditionalDetails
    Inherits System.Web.UI.Page

#Region "Class Level Fields"
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _wfrPage As New Talent.Common.WebFormResource
    Private _businessUnit As String = "*ALL"
    Private _partner As String = "*ALL"
#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With (_wfrPage)
            .BusinessUnit = "MAINTENANCE" 'TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            'added for testing should be removed once Talent.common is updated 
            .PartnerCode = "*ALL" 'TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "emailTemplateAdditionalDetails.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = "emailTemplateAdditionalDetails.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        blErrorList.Items.Clear()
        pagetitleLabel.Text = _wfrPage.Content("PageTitleText", _languageCode, True)
        pageinstructionsLabel.Text = _wfrPage.Content("PageInstructionText", _languageCode, True)
        If Not String.IsNullOrWhiteSpace(Session("SavedemailTemplateName")) Then
            blErrorList.Items.Add(Session("SavedemailTemplateName").ToString() & " Saved Successfully")
            Session.Remove("SavedemailTemplateName")
        End If
    End Sub

#End Region

End Class
