Imports Talent.Common
Imports Talent.eCommerce

Partial Class PagesPublic_Error_SessionError
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
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            .KeyCode = "SessionError.aspx"
        End With
        If Request.QueryString("errortype") IsNot Nothing Then
            Dim errorType As String = Request.QueryString("errortype").ToUpper()
            plhSessionErrorType.Visible = True
            If errorType = "SAPOCIERROR" Then
                ltlSessionErrTypeMess.Text = _wfr.Content("SAPOCISessionErrorLinkText", _languageCode, True)
            End If
        End If
        If Request.QueryString("cookieless") IsNot Nothing Then
            plhSessionErrorType.Visible = True
            ltlSessionErrTypeMess.Text = ltlSessionErrTypeMess.Text & _wfr.Content("CookielessErrMessage", _languageCode, True)
        End If
        If Request.QueryString("clientscriptless") IsNot Nothing Then
            plhSessionErrorType.Visible = True
            ltlSessionErrTypeMess.Text = ltlSessionErrTypeMess.Text & _wfr.Content("ScriptlessErrMessage", _languageCode, True)
        End If
        plhSessionErrorType.Visible = (ltlSessionErrTypeMess.Text.Length > 0)
    End Sub
End Class