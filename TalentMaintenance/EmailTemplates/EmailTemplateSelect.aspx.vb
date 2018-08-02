
Partial Class EmailTemplates_EmailTemplateSelect
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
            .KeyCode = "EmailTemplateSelect.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = "EmailTemplateSelect.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        blErrorList.Items.Clear()
        pagetitleLabel.Text = _wfrPage.Content("PageTitleText", _languageCode, True)
        pageinstructionsLabel.Text = _wfrPage.Content("PageInstructionText", _languageCode, True)

        If Not String.IsNullOrWhiteSpace(Session("SavedEmailTemplateName")) Then
            blErrorList.Items.Add("'" + Session("SavedEmailTemplateName").ToString() & "' saved successfully")
            Session.Remove("SavedEmailTemplateName")
        End If
        If Not String.IsNullOrWhiteSpace(Session("DeletedEmailTemplateName")) Then
            blErrorList.Items.Add("'" + Session("DeletedEmailTemplateName").ToString() & "' deleted successfully")
            Session.Remove("DeletedEmailTemplateName")
        End If
        If Not String.IsNullOrWhiteSpace(Session("NewActiveTemplateName")) Then
            blErrorList.Items.Add("'" + Session("NewActiveTemplateName").ToString() & "' is now an active template.")
            blErrorList.Items.Add("'" + Session("OldActiveTemplateName").ToString() & "' is no longer an active template.")
            Session.Remove("NewActiveTemplateName")
            Session.Remove("OldActiveTemplateName")
        End If
    End Sub

#End Region

End Class
