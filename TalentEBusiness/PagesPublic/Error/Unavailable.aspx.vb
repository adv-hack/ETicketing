Imports Talent.Common
Imports System.Data

Partial Class PagesPublic_Error_Unavailable
    Inherits TalentBase01

    Private _wfr As WebFormResource = Nothing
    Private _languageCode As String = Nothing
    Private _businessUnit As String = Nothing
    Private _partnerCode As String = Nothing
    Private _pageCode As String = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _wfr = New WebFormResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _businessUnit = TalentCache.GetBusinessUnit()
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        _pageCode = Talent.eCommerce.Utilities.GetCurrentPageName
        With _wfr
            .BusinessUnit = _businessUnit
            .PageCode = _pageCode
            .PartnerCode = _partnerCode
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "Unavailable.aspx"
        End With
        Dim unavailableErrorCode As String = String.Empty
        Dim unavailableReturnPage As String = String.Empty

        If (Session("UnavailableErrorCode") IsNot Nothing) AndAlso (Session("UnavailableErrorCode").ToString().Trim().Length > 0) Then
            unavailableErrorCode = Session("UnavailableErrorCode").ToString().Trim()
            Session("UnavailableErrorCode") = String.Empty
            Session.Remove("UnavailableErrorCode")
            unavailableReturnPage = Session("UnavailableReturnPage").ToString().Trim()
            Session("UnavailableReturnPage") = String.Empty
            Session.Remove("UnavailableReturnPage")
            Dim errMsg As Talent.Common.TalentErrorMessages
            errMsg = New Talent.Common.TalentErrorMessages(_languageCode, _businessUnit, _partnerCode, ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
            ltlUnavailableDescription.Text = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _pageCode, unavailableErrorCode).ERROR_MESSAGE
            If (unavailableReturnPage.Length > 0) Then
                hlnkUnavailableReturnLink.NavigateUrl = unavailableReturnPage
                hlnkUnavailableReturnLink.Text = _wfr.Content("ReturnToPreviousPage", _languageCode, True)
            End If
        End If

        plhReturnLink.Visible = (hlnkUnavailableReturnLink.Text.Length > 0)
        plhUnavailable.Visible = (ltlUnavailableDescription.Text.Length > 0)
    End Sub

End Class
