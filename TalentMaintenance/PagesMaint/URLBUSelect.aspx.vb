
Partial Class PagesMaint_URLBUSelect
    Inherits System.Web.UI.Page

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _wfrPage As New Talent.Common.WebFormResource
    Private _businessUnit As String = "*ALL"
    Private _partner As String = "*ALL"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With (_wfrPage)
            .BusinessUnit = "MAINTENANCE" 'TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            'added for testing should be removed once Talent.common is updated 
            .PartnerCode = "*ALL" 'TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "URLBUSelect.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = "URLBUSelect.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        blErrorList.Items.Clear()
        pagetitleLabel.Text = _wfrPage.Content("PageTitleText", _languageCode, True)
        pageinstructionsLabel.Text = _wfrPage.Content("PageInstructionText", _languageCode, True)

        If pagetitleLabel.Text = String.Empty Then pagetitleLabel.Text = "Select Business Unit URL"

        If Not String.IsNullOrWhiteSpace(Session("SavedURLBU")) Then
            blErrorList.Items.Add("'" + Session("SavedURLBU").ToString() & "' saved successfully")
            Session.Remove("SavedURLBU")
        End If

    End Sub
End Class
