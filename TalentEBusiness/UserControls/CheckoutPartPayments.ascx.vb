Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Data

Partial Class UserControls_CheckoutPartPayments
    Inherits ControlBase

    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Public ReadOnly Property PartPaymentAmount() As Decimal
        Get
            Return txtPartPmt.Text
        End Get
    End Property



    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = TEBUtilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "CheckoutPartPayments.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If AgentProfile.IsAgent AndAlso Profile.Basket.BasketSummary.TotalBasket > 0 AndAlso Not CorporateExistsInBasket() Then

            Dim payingForPPS As Boolean = False

            payingForPPS = IsPayingForPPS()

            If Not payingForPPS Then
                'Only valid for tickets or when retail is integrated with ticketing
                If Profile.Basket.BasketContentType = GlobalConstants.TICKETINGBASKETCONTENTTYPE OrElse Talent.eCommerce.Utilities.IsTicketingDBforRetailOrders Then
                    Me.plhPartPmt.Visible = True
                    lblPartPmt.Text = _ucr.Content("PartPaymentLabel", _languageCode, True)
                End If
            Else
                Me.plhPartPmt.Visible = False
            End If
           
        Else
            Me.plhPartPmt.Visible = False
        End If

    End Sub

    Private Function IsPayingForPPS() As Boolean

        Dim payingForPPS As Boolean = False

        If Session("basketPPS1List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") Is Nothing Then
            payingForPPS = True
        Else
            If Session("basketPPS2List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") Is Nothing Then
                payingForPPS = True
            End If
        End If

        Return payingForPPS

    End Function

    Protected Sub SetValidators(ByVal sender As Object, ByVal e As EventArgs)
        Dim reg As RegularExpressionValidator = CType(sender, RegularExpressionValidator)
        reg.ValidationExpression = _ucr.Attribute("PartPaymentAmountRegularExpression")
        reg.ErrorMessage = _ucr.Content("PartPaymentErrorMessage", _languageCode, True)
    End Sub


    Protected Sub SetRequiredValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim rfv As RequiredFieldValidator = CType(sender, RequiredFieldValidator)
        rfv.Enabled = True
        rfv.ErrorMessage = _ucr.Content("PartPaymentMandatoryErrorMessage", _languageCode, True)
    End Sub

    Public Sub SetPartPaymentAmount()
        txtPartPmt.Text = Convert.ToDecimal(Profile.Basket.BasketSummary.TotalBasket).ToString("F2")
    End Sub

    Private Function CorporateExistsInBasket() As Boolean
        Dim CorporateInBasket As Boolean = False
        For Each item As TalentBasketItem In Profile.Basket.BasketItems
            If item.PRODUCT_TYPE = GlobalConstants.CORPORATEPRODUCTTYPE Then
                CorporateInBasket = True
                Exit For
            End If
        Next
        Return CorporateInBasket
    End Function

End Class
