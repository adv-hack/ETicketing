Imports Talent.Common
Imports Talent.eCommerce

Partial Class PagesPublic_error
    Inherits TalentBase01

    Private _wfr As WebFormResource = Nothing
    Private _languageCode As String = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _wfr = New WebFormResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "error.aspx"
            bodyLabel1.Text = .Content("bodyText", _languageCode, True)
        End With
        If Not Session("talentErrorObj") Is Nothing Then
            Dim errObj As Talent.Common.ErrorObj = CType(Session("talentErrorObj"), Talent.Common.ErrorObj)
            lblErrorCode.Text = _wfr.Content("errorCodeText", _languageCode, True) & errObj.ErrorNumber
            bodyLabel1.Text = errObj.ErrorMessage
            Session("talentErrorObj") = Nothing
        End If
        plhBodyLabel.Visible = (bodyLabel1.Text.Length > 0)
        plhError.Visible = (lblErrorCode.Text.Length > 0)
    End Sub

End Class
