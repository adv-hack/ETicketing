Imports Talent.eCommerce
Partial Class UserControls_PaymentExternal
    Inherits System.Web.UI.UserControl

    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _businessUnit As String = String.Empty
    Private _partner As String = String.Empty
    Private Const PAYPAL As String = "PAYPAL"

    Public Sub ProcessPayPalDirectly()
        ProcessPayPal()
    End Sub
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _businessUnit = TalentCache.GetBusinessUnit
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        With _ucr
            .BusinessUnit = _businessUnit
            .PartnerCode = _partner
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PaymentExternal.ascx"
        End With
    End Sub

    Protected Sub imgBtnPayPal_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgBtnPayPal.Click
        ProcessPayPal()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SetButtonImageUrl()
        Session.Remove("ExternalGatewayName")
        Session.Remove("PayPalStage")
        Session.Remove("ExternalGatewayURL")
        TitleLabel.Text = _ucr.Content("Title", _languageCode, True)
        AssignVisibility()
    End Sub

    Private Sub SetButtonImageUrl()
        With _ucr
            imgBtnPayPal.ImageUrl = Talent.eCommerce.Utilities.GetExternalGatewayImgURL("PAYPAL")
        End With
    End Sub

    Private Sub AssignVisibility()
        imgBtnPayPal.Visible = False
        imgBtnPayPal.Enabled = False
        If Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("PayPalEnabled")) Then
            imgBtnPayPal.Visible = True
            imgBtnPayPal.Enabled = True
        Else
            Me.Visible = False
        End If
    End Sub

    Private Sub ProcessPayPal()
        Dim redirectURL As String = String.Empty
        Session("ExternalGatewayName") = PAYPAL
        Session("PayPalStage") = "1"
        Dim basketContentType As String = Profile.Basket.BasketContentType
        Checkout.UpdateOrderStatusForExternalPay("PAYMENT ATTEMPTED", "PAYPAL Payment Process Started; Basket Content Type : " & basketContentType)
        Select Case basketContentType
            Case "T", "C"
                Session("ExternalGatewayURL") = "~/Redirect/PayPalGateway.aspx?page=checkoutpaymentdetails.aspx&function=paymentexternalcheckout"
                redirectURL = "~/Redirect/TicketingGateway.aspx?page=checkoutpaymentdetails.aspx&function=paymentexternalcheckout"
            Case Else
                redirectURL = "~/Redirect/PayPalGateway.aspx?page=checkoutpaymentdetails.aspx&function=paymentexternalcheckout"
        End Select
        Server.Transfer(redirectURL)
    End Sub
End Class
