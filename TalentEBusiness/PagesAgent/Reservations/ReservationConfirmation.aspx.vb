Imports System.Collections.Generic
Imports Talent.Common
Imports System.Data
Imports TEBUtilities = Talent.eCommerce.Utilities
Partial Class PagesAgent_Reservations_ReservationConfirmation
    Inherits TalentBase01
#Region "Private Variables"
    Private _languageCode As String
    Private _settings As DESettings = Nothing
    Private _businessUnit As String = Nothing
    Private _partnerCode As String = Nothing
    Private _wfr As New WebFormResource
#End Region
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _wfr = New Talent.Common.WebFormResource
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        _businessUnit = TalentCache.GetBusinessUnit()
        With _wfr
            .BusinessUnit = _businessUnit
            .PartnerCode = _partnerCode
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .PageCode = "ReservationConfirmation.aspx"
        End With
    End Sub
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Profile.IsAnonymous AndAlso AgentProfile.IsAgent AndAlso Not String.IsNullOrEmpty(GetValueFromQuerystring("ReservationRef")) Then
            btnNextSale.Text = _wfr.Content("NextSaleButtonText", _languageCode, True)
            DisplayReferenceConfirmation()
        Else
            Response.Redirect(TEBUtilities.GetSiteHomePage())
        End If

    End Sub
    Protected Sub btnNextSale_Click(sender As Object, e As EventArgs) Handles btnNextSale.Click
        If (Not Profile.IsAnonymous) OrElse (Page.User.Identity.IsAuthenticated) Then
            Profile.CustomerLogout(ModuleDefaults.REDIRECT_AFTER_AGENT_LOGOUT_URL)
        Else
            Response.Redirect(ModuleDefaults.REDIRECT_AFTER_AGENT_LOGOUT_URL)
        End If
    End Sub

    Private Sub DisplayReferenceConfirmation()
        Dim reservationReference As String = GetValueFromQuerystring("ReservationRef")
        Dim callId As String = GetValueFromQuerystring("CallId")
        Dim confirmDetails As String = String.Empty
        Dim confirmMessage As String = String.Empty
        Dim confirmHospitalityBookingDetails As String = String.Empty
        ltlConfirmationMessage.Text = _wfr.Content("ReservationMessage", _languageCode, True)
        confirmDetails = _wfr.Content("ReservationDetails", _languageCode, True)
        confirmDetails = confirmDetails.Replace("<<RESERVATION_REFERENCE>>", reservationReference)
        If Not String.IsNullOrEmpty(callId) Then
            confirmHospitalityBookingDetails = _wfr.Content("ReservationCallIdDetails", _languageCode, True)
            confirmHospitalityBookingDetails = confirmHospitalityBookingDetails.Replace("<<BOOKING_REFERENCE>>", callId)
            ltlConfirmationDetails.Text = confirmDetails & confirmHospitalityBookingDetails
        Else
            ltlConfirmationDetails.Text = confirmDetails
        End If
    End Sub
    Private Function GetValueFromQuerystring(ByVal queryStringKey As String) As String
        Dim queryStringValue As String = String.Empty
        If Not String.IsNullOrWhiteSpace(Request.QueryString(queryStringKey)) Then
            queryStringValue = Request.QueryString(queryStringKey).Trim
        End If
        Return queryStringValue
    End Function
End Class
