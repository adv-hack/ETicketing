Imports TEBUtilities = Talent.eCommerce.Utilities
Imports Talent.Common
Partial Class Redirect_GoogleCheckoutNotificationHandler
    Inherits Base01
#Region "Class Level Fields"

    Private _googleCheckout As GoogleCheckout = Nothing

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        HttpContext.Current.Session("ExternalGatewayPayType") = GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE
        _googleCheckout = New GoogleCheckout("GoogleCheckoutNotificationHandler.aspx")
        _googleCheckout.ProcessGoogleNotifications()

        'TryGetTalentCustomerDetails()
        'HttpContext.Current.Session("ExternalGatewayPayType") = GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE
        '_googleCheckout.MarkBackendBasketAsPayPending()
        'AddAndVerifyBackendBasket("U")
        'AddAndVerifyBackendBasket("V")
        'AddAndVerifyBackendBasket("A")
        'ProcessBackendForPaymentFailure()
    End Sub

    Private Sub InsertSerialNumberNotificationStatus()
        Response.Write("<br>")
        'Response.Write(_googleCheckout.InsertSerialNumberNotificationStatus("000456", "Testing - Insert Serial Number Notification Status", "TEST").ToString)
    End Sub

    Private Sub GetBasketSQLConnection()
        Response.Write("<br>")
        'Response.Write(_googleCheckout.GetBasketSQLConnection("123456789", ""))
        Response.Write("<br>")
        'Response.Write(_googleCheckout.GetBasketSQLConnection("", "123456789"))
    End Sub

    Private Sub AddAndVerifyBackendBasket(ByVal mode As String)
        Dim errorDetails As String = String.Empty
        Response.Write("<br>")
        'Response.Write(_googleCheckout.AddAndVerifyBackendBasket("10134496", "123456789012348", mode, errorDetails).ToString())
    End Sub

    Private Sub ProcessBackendForPaymentSuccess()
        '_googleCheckout.ProcessBackendForPaymentSuccess("10134496", "123456789012348", "UNITEDKINGDOM", "000000010740")
    End Sub

    Private Sub ProcessBackendForPaymentFailure()
        ' _googleCheckout.ProcessBackendForPaymentFailure("10134496", "123456789012348", "UNITEDKINGDOM", "000000010740")
    End Sub

    Private Sub ShowReasons()
        Dim _wfr As WebFormResource = Nothing
        Dim _businessUnit As String = String.Empty
        Dim _partnerCode As String = String.Empty
        Dim _languageCode As String = String.Empty
        Dim _talentLogging As TalentLogging = Nothing
        Dim _talentDataObj As TalentDataObjects = Nothing
        _wfr = New Talent.Common.WebFormResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _talentLogging = New Talent.Common.TalentLogging
        _talentLogging.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        _businessUnit = TalentCache.GetBusinessUnit()
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        With _wfr
            .BusinessUnit = _businessUnit
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = _partnerCode
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "GoogleCheckout.vb"
        End With
        Response.Write("<br>" & _wfr.BusinessUnit & _wfr.PageCode & _wfr.PartnerCode & _wfr.FrontEndConnectionString & _wfr.KeyCode)
        Response.Write("<br>" & "Reason_FrontendBasketMissing" & TEBUtilities.CheckForDBNull_String(_wfr.Content("Reason_FrontendBasketMissing", _languageCode, True)))
        Response.Write("<br>" & "Reason_BackendBasketExpired" & TEBUtilities.CheckForDBNull_String(_wfr.Content("Reason_BackendBasketExpired", _languageCode, True)))
        Response.Write("<br>" & "Reason_PaymentRisky" & TEBUtilities.CheckForDBNull_String(_wfr.Content("Reason_PaymentRisky", _languageCode, True)))
        Response.Write("<br>" & "Reason_ChargeCustomerFailed" & TEBUtilities.CheckForDBNull_String(_wfr.Content("Reason_ChargeCustomerFailed", _languageCode, True)))
        Response.Write("<br>" & "Reason_PaymentDeclined" & TEBUtilities.CheckForDBNull_String(_wfr.Content("Reason_PaymentDeclined", _languageCode, True)))

    End Sub

    Private Sub TryGetTalentCustomerDetails()
        Dim returncode As String = String.Empty
        '_googleCheckout.TryGetTalentCustomerDetails("000000010740", returncode)
    End Sub
#End Region
End Class
