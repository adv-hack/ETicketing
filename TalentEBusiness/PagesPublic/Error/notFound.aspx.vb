Imports Talent.eCommerce
Imports Talent.Common

Partial Class PagesPublic_notFound
    Inherits TalentBase01

    Private _wfr As WebFormResource = Nothing
    Private _languageCode As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _wfr = New WebFormResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        If Not Page.IsPostBack Then
            With _wfr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = ProfileHelper.GetPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "notFound.aspx"
                bodyLabel1.Text = .Content("bodyText", _languageCode, True)
            End With
            If Not Request("aspxerrorpath") Is Nothing Then
                ltlErrorPath.Text = Request("aspxerrorpath")
            End If
            plhErrorPath.Visible = (ltlErrorPath.Text.Length > 0)
            plhBodyLabel.Visible = (bodyLabel1.Text.Length > 0)
        End If
    End Sub

End Class
