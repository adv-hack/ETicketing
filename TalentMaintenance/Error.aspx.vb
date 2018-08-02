Imports Talent.Common
Partial Class PagesMaint_Error
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
            .KeyCode = "Error.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = "Error.aspx" 'Talent.eCommerce.Utilities.GetCurrentPageName
        End With

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        titleLabel.Text = wfrPage.Content("titleLabel", _languageCode, True)
        instructionsLabel.Text = wfrPage.Content("instructionsLabel", _languageCode, True)

        Dim errorType As String = Request.QueryString("Type")
        If String.IsNullOrWhiteSpace(Request.QueryString("Type")) Then
            errorType = ""
        Else
            errorType = errorType.ToUpper()
        End If

        Select Case errorType
            Case "UNAUTH"
                titleLabel.Text = "Unauthorised Access"
                instructionsLabel.Text = "Sorry, you are not authorised to view this page"
        End Select
    End Sub
End Class
