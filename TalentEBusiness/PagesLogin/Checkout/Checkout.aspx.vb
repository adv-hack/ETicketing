Imports System.Data
Imports Talent.eCommerce
Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities
Imports System.Linq
Imports System.Collections.Generic

Partial Class PagesLogin_Checkout_Checkout
    Inherits TalentBase01

#Region "Constants"

    Const KEYCODE As String = "Checkout.aspx"
    Const AppendOthersTypeWith = "_Mandatory"

#End Region

#Region "Class Level Fields"

    Private _ucr As New Talent.Common.UserControlResource
    Private _currentPaymentMethod As String = String.Empty
    Private _singlePaymentMethod As Boolean = False
    Private _nextItemToOpen As Integer = 1
    Private _businessUnit As String = TalentCache.GetBusinessUnit
    Private _partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
    Private _wfrPage As New WebFormResource
    Private _languageCode As String = TCUtilities.GetDefaultLanguage
    Private _numberCounter As Integer = 0
    Private _ccTypeEnabled As Boolean = False
    Private _ddTypeEnabled As Boolean = False
    Private _cfTypeEnabled As Boolean = False
    Private _epTypeEnabled As Boolean = False
    Private _ppTypeEnabled As Boolean = False
    Private _csTypeEnabled As Boolean = False
    Private _oaTypeEnabled As Boolean = False
    Private _cpTypeEnabled As Boolean = False
    Private _zbTypeEnabled As Boolean = False
    Private _psTypeEnabled As Boolean = False
    Private _pdTypeEnabled As Boolean = False
    Private _gcTypeEnabled As Boolean = False
    Private _otTypeEnabled As Boolean = False
    Private _GiftCardEnabled As Boolean = False
    Private _invTypeEnabled As Boolean = False
    Private _errMsg As TalentErrorMessages
    Private _log As TalentLogging = TEBUtilities.TalentLogging
    Private _doLayoutFunctions As Boolean = False
    Private _payingForPPS As Boolean = False
    Private _PaymentAPILogText As String = String.Empty
    Private _avsOrCscRejected As Boolean = False
    Private _basketOverAllTotal As Decimal = 0
    Private _isBasketOverAllTotalNegative As Boolean = False
    Private _posIpAddress As String = String.Empty
    Private _posTcpPort As String = String.Empty
    Private _dtPaymentTypes As DataTable = Nothing
    Private _numberOfActivePaymentMethods As Integer = 0
    Private _TicketingGatewayFunctions As TicketingGatewayFunctions = Nothing
    Private _clubProductCodeForFinance As String = String.Empty
    Private _hideTAndCCheckbox As Boolean = True
    Private _TicketingCheckout As Boolean = False
    Private _basketContentType As String = String.Empty
    Private _MonthsAtAddress As String = String.Empty
    Private _Homestatus As String = String.Empty
    Private _EmploymentStatus As String = String.Empty
    Private _GrossIncome As String = String.Empty

#End Region

#Region "Public Properties"

    Public LoadingText As String = String.Empty
    Public CSSClassPPS1 As String = String.Empty
    Public CSSClassPPS2 As String = String.Empty

#End Region

#Region "Page Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = KEYCODE
        End With

        _basketContentType = Profile.Basket.BasketContentType
        If _basketContentType <> GlobalConstants.TICKETINGBASKETCONTENTTYPE Then Checkout.CheckBasketValidity()
        If Profile.Basket.IsEmpty OrElse Session("PartPaymentError") IsNot Nothing Then Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        _TicketingCheckout = Talent.eCommerce.Utilities.IsTicketingDBforRetailOrders()

        ResetPPSPayment(0)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Checkout.Clear3dDetails()
            Session("plhCCExternal") = Nothing
            ResetPPSPayment(1)
            Session("AutoEnrolPPS") = Nothing
            Session("PayPalStage") = Nothing
            Session("GoogleCheckoutStage") = Nothing
            Session("HSBCRequest") = Nothing
            Session("MarketingCampaign") = Nothing
            Session("TakeFinalPayment") = True
            Session("DeliveryDetails") = Nothing
            Session("deliveryAddressValidated") = Nothing
            Session("deliveryAddressRestoreRequired") = Nothing
            If Session("TalentErrorCode") IsNot Nothing AndAlso Session("TicketingGatewayError") IsNot Nothing Then
                Session("StoredDeliveryAddress") = Nothing
            End If
            Session("StoredTicketingDeliveryAddress") = Nothing
            Session("StoredRetailDeliveryAddress") = Nothing
            Session("TemplateIDs") = Nothing
            Session("customerPresent") = Nothing
            registerAcordionMenuScript(0)
            csvIsOthersTextMandatory.Enabled = False
            Checkout.StoreCurrentPaymentType(String.Empty)
        End If
        _hideTAndCCheckbox = Not TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowTAndCCheckbox"))
        _TicketingGatewayFunctions = New TicketingGatewayFunctions
        _errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), _wfrPage.FrontEndConnectionString)
        blErrorMessages.Items.Clear()
        ltlSuccessMessages.Text = String.Empty
        plhSelectPaymentMethod.Visible = True
        _singlePaymentMethod = False
        determineWhatOptionsEnabled()
        Session("MarketingCampaign") = uscMarketingCampaigns.MarketingCampaign
        If uscBasketFees.Visible Then Session("GiftAidSelected") = uscBasketFees.GiftAidSelected
        If uscBasketFees.Visible Then
            uscBasketFees.Visible = (String.IsNullOrWhiteSpace(Profile.Basket.CAT_MODE) OrElse (Not (Profile.Basket.CAT_MODE = GlobalConstants.CATMODE_CANCEL)))
        End If
        If plhAutoEnrolPPS.Visible Then Session("AutoEnrolPPS") = chkAutoEnrolPPS.Checked
        LoadingText = _wfrPage.Content("LoadingText", _languageCode, True)
        Dim scmMainScriptManager As ScriptManager = Master.FindControl("scmMainScriptManager")
        If scmMainScriptManager IsNot Nothing Then
            scmMainScriptManager.AsyncPostBackTimeout = TEBUtilities.CheckForDBNull_Int(_wfrPage.Attribute("CheckoutUpdatePanelScriptTimeoutValue"))
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Try
            'Check the error session objects for any error codes
            If Session("TalentErrorCode") IsNot Nothing Then
                Dim talentErrorMsg As TalentErrorMessage = _errMsg.GetErrorMessage(Session("TalentErrorCode"))
                Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(talentErrorMsg.ERROR_MESSAGE)
                If listItemObject Is Nothing Then blErrorMessages.Items.Add(talentErrorMsg.ERROR_MESSAGE)
                Session("TalentErrorCode") = Nothing
            End If
            If Session("TicketingGatewayError") IsNot Nothing Then
                Dim talentErrorMsg As TalentErrorMessage = _errMsg.GetErrorMessage(Session("TicketingGatewayError"))
                Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(talentErrorMsg.ERROR_MESSAGE)
                If listItemObject Is Nothing Then blErrorMessages.Items.Add(talentErrorMsg.ERROR_MESSAGE)
                Session("TicketingGatewayError") = Nothing
            End If
            If Session("GatewayErrorMessage") IsNot Nothing Then
                If Not String.IsNullOrWhiteSpace(Session("GatewayErrorMessage")) Then
                    Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(Session("GatewayErrorMessage").ToString())
                    If listItemObject Is Nothing Then blErrorMessages.Items.Add(Session("GatewayErrorMessage").ToString())
                    Session.Remove("GatewayErrorMessage")
                End If
            End If
            If Session("TicketingPromotionsSuccess") IsNot Nothing Then
                ltlSuccessMessages.Text = Session("TicketingPromotionsSuccess").ToString()
                Session("TicketingPromotionsSuccess") = Nothing
            End If

            If Session("TicketingPromotionsWarning") IsNot Nothing Then
                ltlWarningMessage.Text = Session("TicketingPromotionsWarning").ToString()
                Session("TicketingPromotionsWarning") = Nothing
            End If

            Dim basketSummaryHasError As Boolean = IsBasketSummaryHasError()

            'Potentially we may have a post back from a user control (such as cashback), if so we need to run the same payment and layout functions
            'which is the same functions that are triggered when we select a payment method.
            If Not _doLayoutFunctions AndAlso canDoLayoutFunction() Then
                _doLayoutFunctions = True
            ElseIf basketSummaryHasError AndAlso _numberOfActivePaymentMethods = 1 Then
                _doLayoutFunctions = True
            End If

            ' Only allowed to continue if Delivery Address has been validated, go into this if statement if the user has changed something on the delivery address
            ' as we need to return them back to the delivery address form and click continue again.
            If plhDeliveryAddress.Visible AndAlso uscDeliveryAddress IsNot Nothing AndAlso Not IsDeliveryAddressValid(False) Then
                _nextItemToOpen = 0
                registerAcordionMenuScript(_nextItemToOpen)
                _doLayoutFunctions = False
            End If

            If Page.IsPostBack AndAlso _doLayoutFunctions Then
                setCurrentPaymentMethod()
                _payingForPPS = isPayingForPPS()
                paymentSelected(_payingForPPS, (uscPaymentDetails.InstallmentsPostBack OrElse uscPaymentDetails.CardTypeSelectionChanged OrElse uscPaymentCardType.CardTypeSelectionChanged))
                isTicketingPromotionsOptionActive()
            End If

            'Show any error/success messages that have been added to the list items
            If blErrorMessages.Items.Count > 0 Then
                plhErrorList.Visible = True
            Else
                plhErrorList.Visible = False
            End If

            If ltlSuccessMessages.Text.Length > 0 Then
                plhSuccessMessage.Visible = True
            Else
                plhSuccessMessage.Visible = False
            End If

            If ltlWarningMessage.Text.Length > 0 Then
                plhWarningMessage.Visible = True
            Else
                plhWarningMessage.Visible = False
            End If

            'Disable certain payment types when part payment is enabled
            InvalidPartPaymentPayTypes()
        Catch ex As Exception
            blErrorMessages.Items.Add(_errMsg.GetErrorMessage("XX").ERROR_MESSAGE)
            _log.ExceptionLog(KEYCODE & "-Page_PreRender", ex)
        End Try
    End Sub

#End Region

#Region "Button Click Protected Methods"

    Protected Sub btnPaymentOptionSelected_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPaymentOptionSelected.Click
        Try
            If isPageValid() Then
                If Not IsDeliveryAddressValid() Then Exit Sub
                uscEPurse.PaymentOptionSelected = True
                _payingForPPS = isPayingForPPS()
                setCurrentPaymentMethod()
                UpdatePaymentTypeInBasketHeader(True)
                paymentSelected(_payingForPPS)
            End If
        Catch ex As Exception
            blErrorMessages.Items.Add(_errMsg.GetErrorMessage("XX").ERROR_MESSAGE)
            _log.ExceptionLog(KEYCODE & "-btnPaymentOptionSelected_Click", ex)
        End Try
    End Sub

    Protected Sub btnConfirmDeliveryAddress_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmDeliveryAddress.Click
        _doLayoutFunctions = True
        If Page.IsValid Then
            Dim errorMessage As String = String.Empty
            errorMessage = uscDeliveryAddress.Confirm()
            If (Session("ddlSelectAddressChanged") IsNot Nothing) Then Session.Remove("ddlSelectAddressChanged")
            If errorMessage = String.Empty Then
                'todo ck leave the below commented lines this is required to fix the order table update only on this button click instead of country change or delivery selection option change
                'Dim deliveryDetails As New DEDeliveryDetails
                'deliveryDetails = Session("StoredDeliveryAddress")
                'Dim deliveryChargeEntity As DEDeliveryCharges.DEDeliveryCharge = Nothing
                'deliveryChargeEntity = CType(HttpContext.Current.Session("DeliveryChargeRetail"), DEDeliveryCharges.DEDeliveryCharge)
                If Profile.Basket.WebPrices.IsWebPricesModified Then
                    TDataObjects.BasketSettings.TblBasketDetail.UpdateRetailBasketItems(Profile.Basket.Basket_Header_ID, Profile.Basket.WebPrices)
                    Dim orderLines As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
                    orderLines.Empty_Order(Profile.Basket.TempOrderID)
                    Dim retailOrder As New Talent.eCommerce.Order
                    retailOrder.Add_OrderLines_To_Order(Profile.Basket.WebPrices, Profile.Basket.TempOrderID, Talent.eCommerce.Utilities.GetCurrencyCode)
                End If
                _nextItemToOpen = 0
                registerAcordionMenuScript(_nextItemToOpen, True)
                _doLayoutFunctions = False
            Else
                _nextItemToOpen = 0
                registerAcordionMenuScript(_nextItemToOpen, False)
                blErrorMessages.Items.Add(errorMessage)
            End If
        End If
    End Sub

    Protected Sub btnConfirmCCType_Click(sender As Object, e As EventArgs) Handles btnConfirmCCType.Click
        _doLayoutFunctions = True
        plhCCType.Visible = True
        plhCCExternal.Visible = True
        _payingForPPS = isPayingForPPS()
        setCurrentPaymentMethod()
        UpdatePaymentTypeInBasketHeader(True)
        paymentSelected(_payingForPPS, canRegisterAccordion:=False)
        Session("plhCCExternal") = True
        If uscPaymentCardType.StartPayment() Then
            _nextItemToOpen = 0
            registerAcordionMenuScript(_nextItemToOpen, True, True)
        Else
            Session("plhCCExternal") = False
            plhCCExternal.Visible = False
            paymentSelected(_payingForPPS)
            _nextItemToOpen = 1
            registerAcordionMenuScript(_nextItemToOpen)
        End If
    End Sub

    Protected Sub btnConfirmCCPayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmCCPayment.Click
        _doLayoutFunctions = True
        plhCreditCard.Visible = True
        csvCardTAndC.Validate()
        If csvCardTAndC.IsValid AndAlso isPageValid() AndAlso ProductsInStock() Then
            If Not IsDeliveryAddressValid() Then Exit Sub
            If uscPaymentDetails.ValidateUserInput() Then
                Session("customerPresent") = chkCustomerPresentCC.Checked
                Dim checkoutStage As String = GlobalConstants.CHECKOUTASPXSTAGE
                'Check the PPS List session object to see if we are saving the payment details or completing a sale
                If Session("basketPPS1List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") Is Nothing Then
                    checkoutStage = GlobalConstants.PPS1STAGE
                    If Session("PPS1PaymentComplete") Is Nothing Then
                        Session("PPS1PaymentComplete") = True
                    End If
                Else
                    If Session("basketPPS2List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") Is Nothing Then
                        checkoutStage = GlobalConstants.PPS2STAGE
                        If Session("PPS2PaymentComplete") Is Nothing Then
                            Session("PPS2PaymentComplete") = True
                        End If
                    End If
                End If

                If _payingForPPS = False AndAlso checkoutStage = GlobalConstants.CHECKOUTASPXSTAGE Then Session("TakeFinalPayment") = True

                If Profile.Basket.BasketContentType <> GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
                    Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                    orders.Set_CreditCardType(Now, uscPaymentDetails.CardTypeDDL.SelectedValue, Profile.Basket.Temp_Order_Id, TalentCache.GetBusinessUnit, Profile.UserName)
                    Session("ccForm") = uscPaymentDetails
                End If

                Dim installments As String = uscPaymentDetails.InstallmentsBox.Text
                If String.IsNullOrEmpty(installments) Then installments = uscPaymentDetails.InstallmentsDDL.SelectedValue
                Checkout.StoreCCDetails(uscPaymentDetails.CardTypeDDL.SelectedValue.ToString, _
                    uscPaymentDetails.CardNumberBox.Text, _
                    uscPaymentDetails.ExpiryMonthDDL.SelectedValue.ToString & uscPaymentDetails.ExpiryYearDDL.SelectedValue.ToString, _
                    uscPaymentDetails.StartMonthDDL.SelectedValue.ToString & uscPaymentDetails.StartYearDDL.SelectedValue.ToString, _
                    uscPaymentDetails.IssueNumberBox.Text, _
                    uscPaymentDetails.SecurityNumberBox.Text, _
                    uscPaymentDetails.CardHolderNameBox.Text, _
                    installments, "", "", "", "", checkoutStage, uscPaymentDetails.SaveTheseCardDetails)
                uscPaymentDetails.ResetCCForm()

                If checkoutStage = GlobalConstants.CHECKOUTASPXSTAGE Then
                    If BasketContentTypeOverride() = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                        If takePayment() Then
                            If Checkout.FinaliseOrder("CC") Then
                                Dim order As New Order()
                                order.ProcessMerchandiseInBackend(True, False, String.Empty, False)
                                Response.Redirect("~/PagesLogin/Checkout/CheckoutOrderConfirmation.aspx?paymentType=CC")
                            Else
                                Checkout.UpdateOrderStatus("ORDER FAILED")
                                blErrorMessages.Items.Add(_wfrPage.Content("ErrorFinalisingOrder", _languageCode, True))
                            End If
                        Else
                            Checkout.UpdateOrderStatus("PAYMENT REJECTED", _PaymentAPILogText)
                            If _avsOrCscRejected Then
                                blErrorMessages.Items.Add(_wfrPage.Content("ErrorTakingPaymentAvs", _languageCode, True))
                            Else
                                blErrorMessages.Items.Add(_wfrPage.Content("ErrorTakingPayment", _languageCode, True))
                            End If
                        End If
                    Else


                        If is3DSecureInUse() AndAlso canProcess3DSecure(False) Then
                            Response.Redirect("checkout3dSecure.aspx")
                        Else
                            If Session("TakeFinalPayment") = True Then

                                If AgentProfile.IsAgent Then
                                    Dim paymentAmount As Decimal = RetrievePaymentAmount()
                                    If paymentAmount = Profile.Basket.BasketSummary.TotalBasket Then
                                        _TicketingGatewayFunctions.CheckoutPayment()

                                    Else
                                        PartPayment(paymentAmount, False)
                                    End If
                                Else
                                    _TicketingGatewayFunctions.CheckoutPayment()
                                End If

                            End If
                        End If
                    End If
                Else
                    _doLayoutFunctions = False
                    determineWhatOptionsEnabled()
                    _nextItemToOpen = 0
                    registerAcordionMenuScript(_nextItemToOpen)
                End If
            Else
                For Each item As ListItem In uscPaymentDetails.ErrorMessages.Items
                    blErrorMessages.Items.Add(item)
                Next
                _nextItemToOpen = 0
                registerAcordionMenuScript(_nextItemToOpen)
            End If
        Else
            _nextItemToOpen = 0
            registerAcordionMenuScript(_nextItemToOpen)
        End If
    End Sub

    Protected Sub btnConfirmSavedCardPayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmSavedCardPayment.Click
        If (ModuleDefaults.PaymentGatewayType.ToUpper() = GlobalConstants.PAYMENTGATEWAY_VANGUARD) Then
            ConfirmSavedCardPayment_Vanguard()
        Else
            ConfirmSavedCardPayment()
        End If
    End Sub

    Protected Sub imgBtnConfirmPayPalPayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles imgBtnConfirmPayPalPayment.Click
        _doLayoutFunctions = True
        plhPayPal.Visible = True
        csvPayPalTAndC.Validate()
        If csvPayPalTAndC.IsValid AndAlso isPageValid() Then
            If Not IsDeliveryAddressValid() Then Exit Sub
            Session("ExternalGatewayPayType") = GlobalConstants.PAYPALPAYMENTTYPE
            Session("PayPalStage") = "1"
            Checkout.UpdateOrderStatusForExternalPay("PAYMENT ATTEMPTED", "PAYPAL Payment Process Started; Basket Content Type : " & Profile.Basket.BasketContentType)
            If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE OrElse Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                orders.Set_CreditCardType(Now, uscPaymentDetails.CardTypeDDL.SelectedValue, Profile.Basket.Temp_Order_Id, TalentCache.GetBusinessUnit, Profile.UserName)
            End If
            Select Case BasketContentTypeOverride()
                Case GlobalConstants.TICKETINGBASKETCONTENTTYPE
                    _TicketingGatewayFunctions.CheckoutExternal(True)
                Case GlobalConstants.MERCHANDISEBASKETCONTENTTYPE
                    Server.Transfer("~/Redirect/PayPalGateway.aspx?page=checkoutpaymentdetails.aspx&function=paymentexternalcheckout")
                Case GlobalConstants.COMBINEDBASKETCONTENTTYPE
                    _TicketingGatewayFunctions.CheckoutExternal(True)
            End Select
        Else
            _nextItemToOpen = 1
            registerAcordionMenuScript(_nextItemToOpen)
        End If
    End Sub

    Protected Sub imgBtnConfirmGoogleCheckoutPayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles imgBtnConfirmGoogleCheckoutPayment.Click
        _doLayoutFunctions = True
        plhGoogleCheckout.Visible = True
        csvGoogleCheckoutTAndC.Validate()
        If csvGoogleCheckoutTAndC.IsValid AndAlso isPageValid() Then
            If Not IsDeliveryAddressValid() Then Exit Sub
            Session("ExternalGatewayPayType") = GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE
            Session("GoogleCheckoutStage") = "1"
            Server.Transfer("~/Redirect/GoogleCheckoutGateway.aspx")
        Else
            _nextItemToOpen = 1
            registerAcordionMenuScript(_nextItemToOpen)
        End If
    End Sub

    Protected Sub btnConfirmDDPayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmDDPayment.Click
        _doLayoutFunctions = True
        plhDirectDebit.Visible = True
        csvDDTAndC.Validate()
        If csvDDTAndC.IsValid AndAlso isPageValid() Then
            If uscDirectDebit.ValidateUserInput() Then
                'If uscCheckoutTotals.PartPaymentsTotal = 0 Then
                If Profile.Basket.BasketSummary.TotalPartPayments = 0 Then
                    Dim checkoutStage As String = GlobalConstants.CHECKOUTASPXSTAGE
                    'Check the PPS List session object to see if we are saving the payment details or completing a sale
                    If Session("basketPPS1List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") Is Nothing Then
                        checkoutStage = GlobalConstants.PPS1STAGE
                    Else
                        If Session("basketPPS2List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") Is Nothing Then
                            checkoutStage = GlobalConstants.PPS2STAGE
                        End If
                    End If

                    Dim productForDirectDebit As String = hdfProductForDD.Value
                    Dim sortCode As New StringBuilder
                    sortCode.Append(uscDirectDebit.SortCode1Box.Text)
                    sortCode.Append(uscDirectDebit.SortCode2Box.Text)
                    sortCode.Append(uscDirectDebit.SortCode3Box.Text)
                    Checkout.StoreDDDetails(uscDirectDebit.AccountNameBox.Text, sortCode.ToString(), uscDirectDebit.AccountNumberBox.Text, _
                        uscDirectDebit.BankName, uscDirectDebit.PaymentDayDropDownList.SelectedValue, _
                        productForDirectDebit, checkoutStage)
                    plhDirectDebitWrapper.Visible = False
                    plhDirectDebitMandateWrapper.Visible = True
                    btnConfirmDDMandate.Text = _wfrPage.Content("ConfirmDDMandateButtonText", _languageCode, True)
                    btnCancelDDMandate.Text = _wfrPage.Content("CancelDDMandateButtonText", _languageCode, True)
                    uscDirectDebitMandate.PageCode = checkoutStage
                    uscDirectDebitMandate.Visible = True
                    uscBasketFees.DisableFormElements = True
                    txtPromotions.ReadOnly = True
                    btnPromotions.Enabled = False
                End If
            End If
        End If
        _nextItemToOpen = 1
        registerAcordionMenuScript(_nextItemToOpen)
    End Sub

    Protected Sub btnConfirmDDMandate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmDDMandate.Click
        Dim checkoutStage As String = GlobalConstants.CHECKOUTASPXSTAGE
        'Check the PPS List session object to see if we are saving the payment details or completing a sale
        If Session("basketPPS1List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") Is Nothing Then
            checkoutStage = GlobalConstants.PPS1STAGE
            If Session("PPS1PaymentComplete") Is Nothing Then
                Session("PPS1PaymentComplete") = True
            End If
        Else
            If Session("basketPPS2List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") Is Nothing Then
                checkoutStage = GlobalConstants.PPS2STAGE
                If Session("PPS2PaymentComplete") Is Nothing Then
                    Session("PPS2PaymentComplete") = True
                End If
            End If
        End If

        If checkoutStage = GlobalConstants.CHECKOUTASPXSTAGE Then
            _TicketingGatewayFunctions.CheckoutPayment()
        Else
            _doLayoutFunctions = False
            determineWhatOptionsEnabled()
            _nextItemToOpen = 0
            registerAcordionMenuScript(_nextItemToOpen)
            plhDirectDebitMandateWrapper.Visible = False
            plhDirectDebitWrapper.Visible = True
        End If
    End Sub

    Protected Sub btnCancelDDMandate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelDDMandate.Click
        _doLayoutFunctions = True
        If Session("basketPPS1List") IsNot Nothing Then
            _payingForPPS = True
        ElseIf Session("basketPPS2List") IsNot Nothing Then
            _payingForPPS = True
        End If
        plhDirectDebitWrapper.Visible = True
        plhDirectDebitMandateWrapper.Visible = False
        _nextItemToOpen = 1
        registerAcordionMenuScript(_nextItemToOpen)
    End Sub

    Protected Sub btnConfirmInvoicePayment_Click(sender As Object, e As EventArgs) Handles btnConfirmInvoicePayment.Click
        csvInvoice.Validate()
        If csvInvoice.IsValid() AndAlso isPageValid() Then
            If Not IsDeliveryAddressValid() Then Exit Sub
            Checkout.StoreINVDetails()
            _TicketingGatewayFunctions.CheckoutPayment()
        Else
            _nextItemToOpen = 1
        End If
        registerAcordionMenuScript(_nextItemToOpen)
    End Sub

    Protected Sub btnConfirmEPursePayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmEPursePayment.Click
        plhEPurse.Visible = True
        csvEPurseTAndC.Validate()
        If csvEPurseTAndC.IsValid AndAlso isPageValid() Then
            If Not IsDeliveryAddressValid() Then Exit Sub
            uscEPurse.BasketTotal = Profile.Basket.BasketSummary.TotalBasketWithoutPayTypeFee
            uscEPurse.BasketTotalWithFees = Profile.Basket.BasketSummary.TotalBasket

            Dim paymentAmount As Decimal = 0
            Dim PIN As String = String.Empty
            Dim isGiftCard As Boolean = False
            Dim giftCardNumber As String = String.Empty
            Try
                paymentAmount = CDec(uscEPurse.PaymentAmount)
            Catch ex As Exception
                paymentAmount = 0
            End Try

            isGiftCard = uscEPurse.IsGiftCard
            PIN = uscEPurse.PIN
            If paymentAmount = Profile.Basket.BasketSummary.TotalBasket Then
                If uscEPurse.ValidateUserInput() Then
                    Checkout.StoreEPDetails(uscEPurse.GiftCardNumber, uscEPurse.Card.Text, PIN, isGiftCard)
                    _TicketingGatewayFunctions.CheckoutPayment()
                End If
            Else
                If uscEPurse.ValidateUserInput() Then
                    Dim paymentSuccess As Boolean = uscEPurse.TakePartPayment()
                    ProcessPartPayment(paymentSuccess, paymentAmount)
                    uscBasketFees.DisableFormElements = True
                    txtPromotions.ReadOnly = True
                    btnPromotions.Enabled = False
                    _nextItemToOpen = 0
                End If
            End If
        Else
            _nextItemToOpen = 1
        End If
        registerAcordionMenuScript(_nextItemToOpen)
    End Sub

    Protected Sub btnConfirmCFPayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmCFPayment.Click
        Dim clubProductCodeForFinance As String = String.Empty
        _doLayoutFunctions = True
        plhCreditFinance.Visible = True
        csvCFTAndC.Validate()
        If csvCFTAndC.IsValid AndAlso isPageValid() Then
            If uscCreditFinance.ValidateUserInput() Then
                If Profile.Basket.BasketSummary.TotalPartPayments = 0 Then
                    Dim sortCode As New StringBuilder
                    sortCode.Append(uscCreditFinance.SortCode1Box.Text)
                    sortCode.Append(uscCreditFinance.SortCode2Box.Text)
                    sortCode.Append(uscCreditFinance.SortCode3Box.Text)

                    For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                        If tbi.FINANCE_CLUB_PRODUCT_ID <> String.Empty Then
                            clubProductCodeForFinance = Trim(tbi.FINANCE_CLUB_PRODUCT_ID)
                            Exit For
                        End If
                    Next
                    Checkout.StoreCFDetails(uscCreditFinance.AccountNameBox.Text, sortCode.ToString(), uscCreditFinance.AccountNumberBox.Text, _
                        uscCreditFinance.BankName, uscCreditFinance.InstallmentPlanDropDownList.SelectedValue, _
                        uscCreditFinance.NoOfYearsAtAddress, uscCreditFinance.AddressLine1.Text, uscCreditFinance.AddressLine2.Text, _
                        uscCreditFinance.AddressLine3.Text, uscCreditFinance.AddressLine4.Text, uscCreditFinance.AddressPostCode.Text, clubProductCodeForFinance, _
                    uscCreditFinance.MonthsAtAddress, uscCreditFinance.HomeStatus, uscCreditFinance.EmploymentStatus, uscCreditFinance.GrossIncome)


                    uscCreditFinanceMandate.Display = True
                    plhCreditFinanceWrapper.Visible = False
                    plhCreditFinanceMandateWrapper.Visible = True
                    btnConfirmCFMandate.Text = _wfrPage.Content("ConfirmCFMandateButtonText", _languageCode, True)
                    btnCancelCFMandate.Text = _wfrPage.Content("CancelCFMandateButtonText", _languageCode, True)
                    uscCreditFinance.Visible = True
                    uscBasketFees.DisableFormElements = True
                    txtPromotions.ReadOnly = True
                    btnPromotions.Enabled = False
                End If
            End If
        End If
        _nextItemToOpen = 1
        registerAcordionMenuScript(_nextItemToOpen)
    End Sub

    Protected Sub btnCancelCFMandate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelCFMandate.Click
        _doLayoutFunctions = True
        plhCreditFinanceWrapper.Visible = True
        plhCreditFinanceMandateWrapper.Visible = False
        registerAcordionMenuScript(_nextItemToOpen)
    End Sub

    Protected Sub btnConfirmCFMandate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmCFMandate.Click
        If isPageValid() Then
            _TicketingGatewayFunctions.CheckoutPayment()
        End If
    End Sub

    Protected Sub btnConfirmCashPayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmCashPayment.Click
        plhCash.Visible = True
        csvCashTAndC.Validate()
        If csvCashTAndC.IsValid() AndAlso isPageValid() Then
            If Not IsDeliveryAddressValid() Then Exit Sub

            Checkout.StoreCSDetails()

            If AgentProfile.IsAgent Then
                Dim paymentAmount As Decimal = RetrievePaymentAmount()
                If paymentAmount = Profile.Basket.BasketSummary.TotalBasket Then
                    _TicketingGatewayFunctions.CheckoutPayment()
                Else
                    PartPayment(paymentAmount, False)
                End If
            Else
                _TicketingGatewayFunctions.CheckoutPayment()
            End If
        Else
            _nextItemToOpen = 1
            registerAcordionMenuScript(_nextItemToOpen)
        End If
    End Sub

    Protected Sub btnConfirmOthersPayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmOthersPayment.Click
        plhOthers.Visible = True
        If Page.IsValid Then
            Checkout.StoreOTDetails(ddlOthersType.SelectedValue, Server.HtmlEncode(txtOthersConfigurableValue.Text))
            If AgentProfile.IsAgent Then
                Dim paymentAmount As Decimal = RetrievePaymentAmount()
                If paymentAmount = Profile.Basket.BasketSummary.TotalBasket Then
                    _TicketingGatewayFunctions.CheckoutPayment()
                Else
                    PartPayment(paymentAmount, False)
                End If
            Else
                _TicketingGatewayFunctions.CheckoutPayment()
            End If
        Else
            _nextItemToOpen = 1
            If plhMarketingCampaigns.Visible Then _nextItemToOpen += 1
            registerAcordionMenuScript(_nextItemToOpen)
        End If
    End Sub

    Protected Sub btnConfirmPDQPayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmPDQPayment.Click
        plhPDQ.Visible = True
        csvPDQTAndC.Validate()
        If csvPDQTAndC.IsValid AndAlso isPageValid() Then
            Checkout.StorePDDetails()
            If AgentProfile.IsAgent Then
                Dim paymentAmount As Decimal = RetrievePaymentAmount()
                If paymentAmount = Profile.Basket.BasketSummary.TotalBasket Then
                    _TicketingGatewayFunctions.CheckoutPayment()
                Else
                    PartPayment(paymentAmount, False)
                End If
            Else
                _TicketingGatewayFunctions.CheckoutPayment()
            End If
        Else
            _nextItemToOpen = 1
            registerAcordionMenuScript(_nextItemToOpen)
        End If
    End Sub

    Protected Sub btnConfirmOnAccountPayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmOnAccountPayment.Click
        plhOnAccount.Visible = True
        csvOnAccountTAndC.Validate()
        If csvOnAccountTAndC.IsValid AndAlso isPageValid() Then
            If Not IsDeliveryAddressValid() Then Exit Sub
            Checkout.StoreOADetails()
            _TicketingGatewayFunctions.CheckoutPayment()
        Else
            _nextItemToOpen = 1
            registerAcordionMenuScript(_nextItemToOpen)
        End If
    End Sub

    Protected Sub btnConfirmChipAndPinPayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmChipAndPinPayment.Click
        plhChipAndPin.Visible = True
        csvChipAndPin.Validate()
        If csvChipAndPin.IsValid() AndAlso isPageValid() Then
            If Not IsDeliveryAddressValid() Then Exit Sub
            Dim ipAddress As String = String.Empty
            Dim dtChipAndPinDevices As DataTable = TDataObjects.PaymentSettings.TblChipAndPinDevices.GetByBUPartner(_businessUnit, _partner)
            For Each row As DataRow In dtChipAndPinDevices.Rows
                If row("ID").ToString() = ddlChipAndPinDevices.SelectedValue Then
                    ipAddress = row("TERMINAL_DEVICE_IP_ADDRESS").ToString()
                    Exit For
                End If
            Next
            If ipAddress.Length > 0 Then
                Checkout.StoreCPDetails(ipAddress)
                Dim redirectUrl As String = String.Empty

                'Retrieve part payment amount when in agent mode
                Dim paymentAmount As Decimal
                If AgentProfile.IsAgent Then
                    paymentAmount = RetrievePaymentAmount()
                Else
                    paymentAmount = Profile.Basket.BasketSummary.TotalBasket
                End If

                If paymentAmount = Profile.Basket.BasketSummary.TotalBasket Then
                    _TicketingGatewayFunctions.CheckoutPayment(False, redirectUrl)
                    redirectUrl = ResolveUrl(redirectUrl)
                    If Not redirectUrl.Contains("checkout.aspx") Then
                        ScriptManager.RegisterStartupScript(updCheckout, Me.GetType(), "redirect", "window.location.href='" & redirectUrl & "';", True)
                    End If
                Else
                    PartPayment(paymentAmount, False)
                End If
            End If
        End If
        _nextItemToOpen = 0
        registerAcordionMenuScript(_nextItemToOpen)
    End Sub

    Protected Sub btnConfirmPointOfSalePayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmPointOfSalePayment.Click
        plhPointOfSale.Visible = True
        csvPointOfSale.Validate()
        If csvPointOfSale.IsValid AndAlso isPageValid() Then
            Dim ipAddress As String = String.Empty
            Dim tcpPort As String = String.Empty
            Dim dtPointOfSaleTerminals As DataTable = TDataObjects.PaymentSettings.TblPointOfSaleTerminals.GetByBUPartner(_businessUnit, _partner)
            For Each row As DataRow In dtPointOfSaleTerminals.Rows
                If row("ID").ToString() = ddlPointOfSaleTerminals.SelectedValue Then
                    ipAddress = row("POS_TERMINAL_IP_ADDRESS").ToString()
                    tcpPort = row("POS_TERMINAL_TCP_PORT").ToString()
                    Exit For
                End If
            Next
            If ipAddress.Length > 0 AndAlso tcpPort.Length > 0 Then
                Checkout.StorePSDetails(ipAddress, tcpPort)
                Dim redirectUrl As String = String.Empty
                _TicketingGatewayFunctions.CheckoutPayment(False, redirectUrl)
                redirectUrl = ResolveUrl(redirectUrl)
                If Not redirectUrl.Contains("checkout.aspx") Then
                    ScriptManager.RegisterStartupScript(updCheckout, Me.GetType(), "redirect", "window.location.href='" & redirectUrl & "';", True)
                End If
            End If
        End If
        _nextItemToOpen = 1
        registerAcordionMenuScript(_nextItemToOpen)
    End Sub

    Protected Sub btnZeroPricedBasketPayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnZeroPricedBasketPayment.Click
        plhZeroPricedBasket.Visible = True
        csvZeroPricedBasketTAndC.Validate()
        If csvZeroPricedBasketTAndC.IsValid AndAlso isPageValid() Then
            Checkout.StoreCSDetails() 'A zero priced basket is treated as a cash sale
            _TicketingGatewayFunctions.CheckoutPayment()
        Else
            _nextItemToOpen = 1
            registerAcordionMenuScript(_nextItemToOpen)
        End If
    End Sub

    Protected Sub btnTicketingPromotions_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPromotions.Click
        processPromotion()
    End Sub


#End Region

#Region "Other Protected Methods"

    Protected Sub ValidateTerms(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        If plhCreditCard.Visible AndAlso plhCreditCardTAndC.Visible Then e.IsValid = chkCardTAndC.Checked
        If plhSavedCard.Visible AndAlso plhSavedCardsTAndC.Visible Then e.IsValid = chkSavedCardTAndC.Checked
        If plhPayPal.Visible AndAlso plhPayPalTAndC.Visible Then e.IsValid = chkPayPalTAndC.Checked
        If plhDirectDebit.Visible AndAlso plhDirectDebitTAndC.Visible Then e.IsValid = chkDDTAndC.Checked
        If plhGoogleCheckout.Visible AndAlso plhGoogleCheckoutTAndC.Visible Then e.IsValid = chkGoogleCheckoutTAndC.Checked
        If plhEPurse.Visible AndAlso plhEPurseTAndC.Visible Then e.IsValid = chkEPurseTAndC.Checked
        If plhCreditFinance.Visible AndAlso plhCFTAndC.Visible Then e.IsValid = chkCFTAndC.Checked
        If plhCash.Visible AndAlso plhCashTAndC.Visible Then e.IsValid = chkCashTAndC.Checked
        If plhPDQ.Visible AndAlso plhPDQTAndC.Visible Then e.IsValid = chkPDQTAndC.Checked
        If plhInvoice.Visible AndAlso plhInvoiceTAndC.Visible Then e.IsValid = chkInvoiceTAndC.Checked
        If plhOnAccount.Visible AndAlso plhOnAccountTAndC.Visible Then e.IsValid = chkOnAccountTAndC.Checked
        If plhChipAndPin.Visible AndAlso plhChipAndPinTAndC.Visible Then e.IsValid = chkChipAndPinTAndC.Checked
        If plhPointOfSale.Visible AndAlso plhPointOfSaleTAndC.Visible Then e.IsValid = chkPointOfSaleTAndC.Checked
        If plhZeroPricedBasket.Visible AndAlso plhZeroPricedBasketTAndC.Visible Then e.IsValid = chkZeroPricedBasketTAndC.Checked
        If plhOthers.Visible AndAlso plhOthersTAndC.Visible Then e.IsValid = chkOthersTAndC.Checked
    End Sub

    Protected Sub ValidateOthersText(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        If plhOthers.Visible Then
            Dim isOthersTextMandatoryDictionary As New Dictionary(Of String, Boolean)
            Dim lic As ListItemCollection
            lic = GetOtherTypes(isOthersTextMandatoryDictionary)
            e.IsValid = True
            For Each Pair In isOthersTextMandatoryDictionary
                If Pair.Key = ddlOthersType.SelectedValue Then
                    If Pair.Value AndAlso String.IsNullOrEmpty(txtOthersConfigurableValue.Text) Then
                        e.IsValid = False
                    End If
                End If
            Next
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Check the system defaults to see which payment options are enabled and alter the display accordingly
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub determineWhatOptionsEnabled()
        Try
            resetPaymentOptions(canResetPayOptions())
            populatePaymentDefaults()
            Dim showNumbers As Boolean = False

            If isPPSPaymentOptionsInUse() Then
                plhPaymentMethodDirectDebit.Visible = isDirectDebitOptionActive(True)
                plhPaymentMethodPayPal.Visible = False
                plhPaymentMethodGoogleCheckout.Visible = False
                plhPaymentMethodCashback.Visible = False
                plhPaymentMethodEPurse.Visible = False
                plhPaymentMethodCash.Visible = False
                plhPaymentMethodPDQ.Visible = False
                plhPaymentMethodOnAccount.Visible = False
                plhPaymentMethodChipAndPin.Visible = False
                plhPaymentMethodPointOfSale.Visible = False
                plhPaymentMethodCreditFinance.Visible = False
                plhPaymentMethodZeroPricedBasket.Visible = False
                plhPaymentMethodInvoice.Visible = False
                plhAutoEnrolPPS.Visible = False
                plhCreditCardTAndC.Visible = False
                plhSavedCardsTAndC.Visible = False
                plhDirectDebitTAndC.Visible = False
                plhPaymentMethodOthers.Visible = False
                plhOthersTAndC.Visible = False
                Session("PPSPayment") = True
            Else
                Session("PPSPayment") = False
                showNumbers = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfrPage.Attribute("ShowNumbersOnPaymentSteps"))
                plhPaymentMethodDirectDebit.Visible = isDirectDebitOptionActive(False)
                plhPaymentMethodPayPal.Visible = isPayPalOptionActive()
                plhPaymentMethodGoogleCheckout.Visible = isGoogleCheckoutOptionActive()
                plhPaymentMethodCashback.Visible = (Not _isBasketOverAllTotalNegative AndAlso isCashbackOptionActive())
                plhPaymentMethodEPurse.Visible = isEPurseOptionActive()
                plhPaymentMethodCreditFinance.Visible = isCreditFinanceOptionActive()
                plhPaymentMethodCash.Visible = isCashOptionActive()
                plhPaymentMethodOthers.Visible = isOtherOptionActive()
                plhPaymentMethodPDQ.Visible = isPDQOptionActive()
                plhPaymentMethodOnAccount.Visible = isOnAccountOptionActive()
                plhPaymentMethodChipAndPin.Visible = isChipAndPinOptionActive()
                plhPaymentMethodPointOfSale.Visible = isPointOfSaleOptionActive()
                plhPaymentMethodZeroPricedBasket.Visible = isZeroPricedBasketOptionActive()
                plhPaymentMethodInvoice.Visible = isInvoiceOptionActive()
                plhAutoEnrolPPS.Visible = isAutoEnrolPPSOptionActive()
                plhCreditCardTAndC.Visible = True
                plhSavedCardsTAndC.Visible = True
                plhDirectDebitTAndC.Visible = True
            End If
            If Session("TakeFinalPayment") = False Then plhSelectPaymentMethod.Visible = True
            plhMarketingCampaigns.Visible = isMarketingCampaignsActive()
            If Not plhPaymentMethodZeroPricedBasket.Visible Then plhPaymentMethodCreditCard.Visible = isCreditCardOptionActive()
            If Not plhPaymentMethodZeroPricedBasket.Visible Then plhPaymentMethodSavedCard.Visible = isSavedCardOptionActive()
            plhSelectPaymentMethod.Visible = isSelectPaymentMethodActive()
            plhPromotions.Visible = isTicketingPromotionsOptionActive()

            plhDeliveryAddress.Visible = isDeliveryAddressActive()
            If Not IsPostBack Then IsDeliveryAddressValid(False)

            'Numbering on each option
            If showNumbers Then
                _numberCounter = TEBUtilities.CheckForDBNull_Int(_wfrPage.Content("StartingNumber", _languageCode, True))
                If plhMarketingCampaigns.Visible Then
                    ltlMarketingCampaignNumber.Text = _numberCounter.ToString()
                    _numberCounter += 1
                End If
                If plhAutoEnrolPPS.Visible Then
                    ltlAutoEnrolPPSNumber.Text = _numberCounter.ToString()
                    _numberCounter += 1
                End If
                If plhDeliveryAddress.Visible Then
                    ltlDeliveryAddressNumber.Text = _numberCounter.ToString()
                    _numberCounter += 1
                End If
                If plhSelectPaymentMethod.Visible Then
                    ltlPaymentOptionNumber.Text = _numberCounter.ToString()
                    _numberCounter += 1
                End If
                setCCControlsByPaymentGateway("determineWhatOptionsEnabled")
                If _numberCounter > 1 Then
                    ltlCreditCardNumber.Text = _numberCounter.ToString()
                    ltlSavedCardNumber.Text = _numberCounter.ToString()
                    ltlDirectDebitNumber.Text = _numberCounter.ToString()
                    ltlPayPalNumber.Text = _numberCounter.ToString()
                    ltlGoogleCheckoutNumber.Text = _numberCounter.ToString()
                    ltlCashbackNumber.Text = _numberCounter.ToString()
                    ltlEPurseNumber.Text = _numberCounter.ToString()
                    ltlCreditFinanceNumber.Text = _numberCounter.ToString()
                    ltlCashNumber.Text = _numberCounter.ToString()
                    ltlOthersNumber.Text = _numberCounter.ToString()
                    ltlOnAccountNumber.Text = _numberCounter.ToString()
                    ltlChipAndPinNumber.Text = _numberCounter.ToString()
                    ltlPointOfSaleNumber.Text = _numberCounter.ToString()
                    ltlZeroPricedBasketNumber.Text = _numberCounter.ToString()
                    ltlPDQNumber.Text = _numberCounter.ToString()
                    ltlInvoiceNumber.Text = _numberCounter.ToString()
                Else
                    showNumbers = False
                End If
            End If
            plhMarketingCampaignNumber.Visible = showNumbers
            plhDeliveryAddressNumber.Visible = showNumbers
            plhAutoEnrolPPSNumber.Visible = showNumbers
            plhPaymentOptionNumber.Visible = showNumbers
            plhCreditCardNumber.Visible = showNumbers
            plhSavedCardNumber.Visible = showNumbers
            plhPayPalNumber.Visible = showNumbers
            plhGoogleCheckoutNumber.Visible = showNumbers
            plhDirectDebitNumber.Visible = showNumbers
            plhCashbackNumber.Visible = showNumbers
            plhEPurseNumber.Visible = showNumbers
            plhCreditFinanceNumber.Visible = showNumbers
            plhCashNumber.Visible = showNumbers
            plhOthersNumber.Visible = showNumbers
            plhOnAccountNumber.Visible = showNumbers
            plhChipAndPinNumber.Visible = showNumbers
            plhPointOfSaleNumber.Visible = showNumbers
            plhZeroPricedBasketNumber.Visible = showNumbers
            plhPDQNumber.Visible = showNumbers
            plhInvoiceNumber.Visible = showNumbers
        Catch ex As Exception
            resetPaymentOptions(True)
            blErrorMessages.Items.Add(_errMsg.GetErrorMessage("XX").ERROR_MESSAGE)
            _log.ExceptionLog(KEYCODE & "-determineWhatOptionsEnabled", ex)
        End Try
    End Sub

    ''' <summary>
    ''' Checks if product is in stock for each merhandise item
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ProductsInStock() As Boolean
        'by pass stock check if ticketing type
        If Profile.Basket.BasketContentType = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then Return True
        Dim success As Boolean = True
        Dim productCode As String
        Dim quantity As Decimal
        For Each item In Profile.Basket.BasketItems
            If Not TEBUtilities.IsTicketingFee(item.MODULE_, item.Product, item.FEE_CATEGORY) AndAlso item.MODULE_.ToUpper <> "TICKETING" Then
                productCode = item.Product
                quantity = item.Quantity
                If Not String.IsNullOrEmpty(productCode) Then
                    If Stock.GetStockBalance(productCode) < quantity Then
                        Dim PSerrorString As String = _wfrPage.Content("ProductOutOfStock", _languageCode, True).Replace("<<ProductCode>>", item.Product)
                        PSerrorString = PSerrorString.Replace("<<ProductDescription>>", item.PRODUCT_DESCRIPTION1)
                        blErrorMessages.Items.Add(PSerrorString)
                        success = False
                    End If
                End If
            End If
        Next
        Return success
    End Function

    ''' <summary>
    ''' Populate class level default settings based on basket type, agent mode and setup in tbl_payment_type tables
    ''' IMPORTANT: Not every payment selection will have a default value, eg. cashback.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populatePaymentDefaults()
        Dim ticketingBasket As Boolean = False
        Dim merchandiseBasket As Boolean = False
        Dim combinedBasket As Boolean = False
        Select Case BasketContentTypeOverride()
            Case GlobalConstants.TICKETINGBASKETCONTENTTYPE
                ticketingBasket = True
            Case GlobalConstants.MERCHANDISEBASKETCONTENTTYPE
                merchandiseBasket = True
            Case GlobalConstants.COMBINEDBASKETCONTENTTYPE
                combinedBasket = True
        End Select

        _dtPaymentTypes = TDataObjects.PaymentSettings.TblPaymentTypeBu.GetByBasketTypeAndBU( _
            _wfrPage.BusinessUnit, ticketingBasket, merchandiseBasket, combinedBasket, AgentProfile.IsAgent(), isBasketTotalNegative, Profile.IsAnonymous, False)
        For Each row As DataRow In _dtPaymentTypes.Rows
            Select Case TEBUtilities.CheckForDBNull_String(row("PAYMENT_TYPE_CODE"))
                Case GlobalConstants.CCPAYMENTTYPE : _ccTypeEnabled = True
                Case GlobalConstants.DDPAYMENTTYPE : _ddTypeEnabled = True
                Case GlobalConstants.CFPAYMENTTYPE : _cfTypeEnabled = True
                Case GlobalConstants.EPURSEPAYMENTTYPE
                    If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(row("IS_GIFT_CARD")) Then
                        _GiftCardEnabled = True
                    Else
                        _epTypeEnabled = True
                    End If
                Case GlobalConstants.PAYPALPAYMENTTYPE : _ppTypeEnabled = True
                Case GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE : _gcTypeEnabled = True
                Case GlobalConstants.CSPAYMENTTYPE : _csTypeEnabled = True
                Case GlobalConstants.OAPAYMENTTYPE : _oaTypeEnabled = True
                Case GlobalConstants.CHIPANDPINPAYMENTTYPE : _cpTypeEnabled = True
                Case GlobalConstants.ZEROPRICEDBASKETPAYMENTTYPE : _zbTypeEnabled = True
                Case GlobalConstants.POINTOFSALEPAYMENTTYPE : _psTypeEnabled = True
                Case GlobalConstants.PDQPAYMENTTYPE : _pdTypeEnabled = True
                Case GlobalConstants.OTHERSPAYMENTTYPE : _otTypeEnabled = True
                Case GlobalConstants.INVPAYMENTTYPE : _invTypeEnabled = True
            End Select
        Next
    End Sub

    ''' <summary>
    ''' Get the list of other payment methods
    ''' </summary>
    ''' <param name="isOthersTextMandatoryDictionary">The text mandatory dictionary object</param>
    ''' <returns>The list object</returns>
    ''' <remarks></remarks>
    Private Function GetOtherTypes(ByRef isOthersTextMandatoryDictionary As Dictionary(Of String, Boolean)) As ListItemCollection
        Dim ticketingBasket As Boolean = False
        Dim merchandiseBasket As Boolean = False
        Dim combinedBasket As Boolean = False
        Dim returnDT As New DataTable
        Dim dRow As DataRow = Nothing
        Dim lic As New ListItemCollection
        Dim li As ListItem

        Select Case BasketContentTypeOverride()
            Case GlobalConstants.TICKETINGBASKETCONTENTTYPE : ticketingBasket = True
            Case GlobalConstants.MERCHANDISEBASKETCONTENTTYPE : merchandiseBasket = True
            Case GlobalConstants.COMBINEDBASKETCONTENTTYPE : combinedBasket = True
        End Select

        Dim dt As DataTable = TDataObjects.PaymentSettings.TblPaymentTypeBu.GetByBasketTypeAndBU(_wfrPage.BusinessUnit, ticketingBasket, merchandiseBasket, combinedBasket, AgentProfile.IsAgent(), isBasketTotalNegative, Profile.IsAnonymous, , , True)
        Dim paymentTypeCodeList = (From r In dt.AsEnumerable() Select r.Field(Of String)("PAYMENT_TYPE_CODE")).Distinct().ToList()
        Dim isTextMandatoryValue As Boolean
        Dim conversionResult As Boolean

        For Each item As String In paymentTypeCodeList
            li = New ListItem(_wfrPage.Content(item, _languageCode, True), item)
            lic.Add(li)
            isTextMandatoryValue = Boolean.TryParse(_wfrPage.Content(item & AppendOthersTypeWith, _languageCode, True), conversionResult)
            If Not conversionResult Then
                isTextMandatoryValue = conversionResult
            End If
            isOthersTextMandatoryDictionary.Add(item, isTextMandatoryValue)
        Next
        Return lic
    End Function

    ''' <summary>
    ''' Set _currentPaymentMethod string object based on the main payment option selected
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setCurrentPaymentMethod()
        resetPaymentOptions(False)
        If rdoCashback.Checked Then _currentPaymentMethod = GlobalConstants.CBPAYMENTTYPE
        If rdoCreditCard.Checked Then _currentPaymentMethod = GlobalConstants.CCPAYMENTTYPE
        If rdoDirectDebit.Checked Then _currentPaymentMethod = GlobalConstants.DDPAYMENTTYPE
        If rdoPayPal.Checked Then _currentPaymentMethod = GlobalConstants.PAYPALPAYMENTTYPE
        If rdoGoogleCheckout.Checked Then _currentPaymentMethod = GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE
        If rdoSavedCard.Checked Then _currentPaymentMethod = GlobalConstants.SAVEDCARDPAYMENTTYPE
        If rdoEPurse.Checked Then _currentPaymentMethod = GlobalConstants.EPURSEPAYMENTTYPE
        If rdoCreditFinance.Checked Then _currentPaymentMethod = GlobalConstants.CFPAYMENTTYPE
        If rdoCash.Checked Then _currentPaymentMethod = GlobalConstants.CSPAYMENTTYPE
        If rdoOthers.Checked Then _currentPaymentMethod = GlobalConstants.OTHERSPAYMENTTYPE
        If rdoOnAccount.Checked Then _currentPaymentMethod = GlobalConstants.OAPAYMENTTYPE
        If rdoChipAndPin.Checked Then _currentPaymentMethod = GlobalConstants.CHIPANDPINPAYMENTTYPE
        If rdoZeroPricedBasket.Checked Then _currentPaymentMethod = GlobalConstants.ZEROPRICEDBASKETPAYMENTTYPE
        If rdoPointOfSale.Checked Then _currentPaymentMethod = GlobalConstants.POINTOFSALEPAYMENTTYPE
        If rdoPDQ.Checked Then _currentPaymentMethod = GlobalConstants.PDQPAYMENTTYPE
        If rdoInvoice.Checked Then _currentPaymentMethod = GlobalConstants.INVPAYMENTTYPE
        If (Not Page.IsPostBack) AndAlso (Not uscBasketFees.BasketFeesUpdated) AndAlso (Not uscBasketSummary.BasketSummaryUpdated) Then
            UpdatePaymentTypeInBasketHeader(True)
        End If
    End Sub

    ''' <summary>
    ''' Validate the part payment input
    ''' </summary>
    ''' <param name="PaymentAmount">The payment amount entered in the part payment text box</param>
    ''' <returns>Whether the value is valid</returns>
    ''' <remarks></remarks>
    Private Function ValidatePartPaymentInput(ByVal PaymentAmount As Decimal) As Boolean
        If PaymentAmount > Profile.Basket.BasketSummary.TotalBasket Then
            blErrorMessages.Items.Add(_wfrPage.Content("PartPaymentExceedsBasketErrorMessage", _languageCode, True))
            Return False
        ElseIf PaymentAmount <= 0 Then
            blErrorMessages.Items.Add(_wfrPage.Content("PartPaymentInvalidAmountErrorMessage", _languageCode, True))
            Return False
        End If
        Return True
    End Function

    ''' <summary>
    ''' Process the part payment
    ''' </summary>
    ''' <param name="PaymentAmount">The payment amount entered in the part payment text box</param>
    ''' <param name="savedCard">Is the payment type a saved card</param>
    ''' <remarks></remarks>
    Private Sub PartPayment(ByVal paymentAmount As Decimal, ByVal savedCard As Boolean)

        If ValidatePartPaymentInput(paymentAmount) Then

            Dim paymentSuccess As Boolean = False
            _TicketingGatewayFunctions.CheckoutPayment(savedCard, "", paymentSuccess, paymentAmount)
            ProcessPartPayment(paymentSuccess, paymentAmount)
        End If
    End Sub

    ''' <summary>
    ''' Retrieve the part payment amount from the correct user control
    ''' </summary>
    ''' <remarks></remarks>
    Private Function RetrievePaymentAmount() As Decimal
        Dim paymentAmount As Decimal = 0
        Try
            If plhCreditCard.Visible Then paymentAmount = CType(uscCCPartPayment.FindControl("txtPartPmt"), TextBox).Text
            If plhSavedCard.Visible Then paymentAmount = CType(uscSCPartPayment.FindControl("txtPartPmt"), TextBox).Text
            If plhCash.Visible Then paymentAmount = CType(uscCashPartPayment.FindControl("txtPartPmt"), TextBox).Text
            If plhPDQ.Visible Then paymentAmount = CType(uscPDQPartPayment.FindControl("txtPartPmt"), TextBox).Text
            If plhChipAndPin.Visible Then paymentAmount = CType(uscChipAndPinPartPayment.FindControl("txtPartPmt"), TextBox).Text
            If plhOthers.Visible Then paymentAmount = CType(uscOthersPartPayment.FindControl("txtPartPmt"), TextBox).Text
        Catch ex As Exception
            paymentAmount = 0
        End Try
        Return paymentAmount
    End Function

    ''' <summary>
    ''' We have a payment method, setup the page based on the selected type
    ''' If there is only 1 payment option selected (CC) then don't reset the credit card form as it fails page validation
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub paymentSelected(ByVal payingForPPS As Boolean, Optional ByVal dontResetCCForm As Boolean = False, Optional ByVal canRegisterAccordion As Boolean = True)
        If _currentPaymentMethod = GlobalConstants.CBPAYMENTTYPE Then plhCashback.Visible = True
        If _currentPaymentMethod = GlobalConstants.CCPAYMENTTYPE Then setCCControlsByPaymentGateway("paymentSelected")
        If _currentPaymentMethod = GlobalConstants.DDPAYMENTTYPE Then plhDirectDebit.Visible = True
        If _currentPaymentMethod = GlobalConstants.PAYPALPAYMENTTYPE Then plhPayPal.Visible = True
        If _currentPaymentMethod = GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE Then plhGoogleCheckout.Visible = True
        If _currentPaymentMethod = GlobalConstants.SAVEDCARDPAYMENTTYPE Then plhSavedCard.Visible = True
        If _currentPaymentMethod = GlobalConstants.EPURSEPAYMENTTYPE Then plhEPurse.Visible = True
        If _currentPaymentMethod = GlobalConstants.CSPAYMENTTYPE Then plhCash.Visible = True
        If _currentPaymentMethod = GlobalConstants.OTHERSPAYMENTTYPE Then plhOthers.Visible = True
        If _currentPaymentMethod = GlobalConstants.OAPAYMENTTYPE Then plhOnAccount.Visible = True
        If _currentPaymentMethod = GlobalConstants.CFPAYMENTTYPE Then plhCreditFinance.Visible = True
        If _currentPaymentMethod = GlobalConstants.CHIPANDPINPAYMENTTYPE Then plhChipAndPin.Visible = True
        If _currentPaymentMethod = GlobalConstants.ZEROPRICEDBASKETPAYMENTTYPE Then plhZeroPricedBasket.Visible = True
        If _currentPaymentMethod = GlobalConstants.POINTOFSALEPAYMENTTYPE Then plhPointOfSale.Visible = True
        If _currentPaymentMethod = GlobalConstants.PDQPAYMENTTYPE Then plhPDQ.Visible = True
        If _currentPaymentMethod = GlobalConstants.INVPAYMENTTYPE Then plhInvoice.Visible = True
        Checkout.StoreCurrentPaymentType(String.Empty)

        Select Case _currentPaymentMethod
            Case GlobalConstants.CCPAYMENTTYPE
                rdoCreditCard.Checked = True
                ltlPaymentOption.Text = lblCreditCard.Text
                If AgentProfile.IsAgent Then
                    Dim txtcc As TextBox = Me.uscCCPartPayment.FindControl("txtPartPmt")
                    txtcc.Text = Convert.ToDecimal(Profile.Basket.BasketSummary.TotalBasket).ToString("F2")
                End If
                If Not dontResetCCForm Then uscPaymentDetails.ResetCCForm()
                uscPaymentDetails.HideSecurityNumber = payingForPPS
                uscPaymentCardDetails.HideSecurityNumber = payingForPPS

            Case GlobalConstants.CFPAYMENTTYPE
                rdoCreditFinance.Checked = True
                ltlPaymentOption.Text = lblCreditFinance.Text

                Checkout.StoreCurrentPaymentType(GlobalConstants.CFPAYMENTTYPE)
                If Profile.User.Details.DOB.Date.ToString.Equals("01/01/1900 00:00:00") Then
                    blErrorMessages.Items.Clear()
                    blErrorMessages.Items.Add(_errMsg.GetErrorMessage(TCUtilities.GetAllString, TEBUtilities.GetCurrentPageName, "needDoBToApplyForCreditFinance").ERROR_MESSAGE)
                Else
                    Dim maxDoBForFinance As Date = Now.AddYears(-ModuleDefaults.minageforcreditfinance)
                    If Profile.User.Details.DOB > maxDoBForFinance Then
                        blErrorMessages.Items.Clear()
                        blErrorMessages.Items.Add(_errMsg.GetErrorMessage(TCUtilities.GetAllString, TEBUtilities.GetCurrentPageName, "tooYoungToApplyForCreditFinance").ERROR_MESSAGE & ModuleDefaults.minageforcreditfinance.ToString)
                    Else
                        'If uscCheckoutTotals.PartPaymentsTotal > 0 Then
                        If Profile.Basket.BasketSummary.TotalPartPayments > 0 Then
                            blErrorMessages.Items.Clear()
                            blErrorMessages.Items.Add(_errMsg.GetErrorMessage(TCUtilities.GetAllString, TEBUtilities.GetCurrentPageName, "CannotPayWithCFWhenEPHasBeenUsed").ERROR_MESSAGE)
                            plhCreditFinance.Visible = False
                            rdoCreditFinance.Checked = False
                            _nextItemToOpen = 0
                        Else
                            validateBasketforCreditFinance()
                        End If
                    End If
                End If
                If blErrorMessages.Items.Count <> 0 Then
                    _currentPaymentMethod = String.Empty
                    rdoCreditFinance.Checked = False
                    pnlCreditFinanceWrapper.Visible = False
                    plhCreditFinance.Visible = False
                    ltlPaymentOption.Text = String.Empty
                End If

            Case GlobalConstants.CSPAYMENTTYPE
                rdoCash.Checked = True
                ltlPaymentOption.Text = lblCash.Text
                If AgentProfile.IsAgent Then
                    Dim txtcc As TextBox = Me.uscCashPartPayment.FindControl("txtPartPmt")
                    txtcc.Text = Convert.ToDecimal(Profile.Basket.BasketSummary.TotalBasket).ToString("F2")
                End If
                Checkout.StoreCurrentPaymentType(GlobalConstants.CSPAYMENTTYPE)

            Case GlobalConstants.OTHERSPAYMENTTYPE
                rdoOthers.Checked = True
                ltlPaymentOption.Text = lblOthers.Text
                If AgentProfile.IsAgent Then
                    Dim txtcc As TextBox = Me.uscOthersPartPayment.FindControl("txtPartPmt")
                    txtcc.Text = Convert.ToDecimal(Profile.Basket.BasketSummary.TotalBasket).ToString("F2")
                End If
                Checkout.StoreCurrentPaymentType(GlobalConstants.OTHERSPAYMENTTYPE)
                PopulateDropDownForOthers()

            Case GlobalConstants.PDQPAYMENTTYPE
                ltlPaymentOption.Text = lblPDQ.Text
                If AgentProfile.IsAgent Then
                    Dim txtcc As TextBox = Me.uscPDQPartPayment.FindControl("txtPartPmt")
                    txtcc.Text = Convert.ToDecimal(Profile.Basket.BasketSummary.TotalBasket).ToString("F2")
                End If
                Checkout.StoreCurrentPaymentType(GlobalConstants.PDQPAYMENTTYPE)

            Case GlobalConstants.OAPAYMENTTYPE
                rdoOnAccount.Checked = True
                ltlPaymentOption.Text = lblOnAccount.Text
                Checkout.StoreCurrentPaymentType(GlobalConstants.OAPAYMENTTYPE)

            Case GlobalConstants.ZEROPRICEDBASKETPAYMENTTYPE
                rdoZeroPricedBasket.Checked = True
                ltlPaymentOption.Text = lblZeroPricedBasket.Text
                Checkout.StoreCurrentPaymentType(GlobalConstants.ZEROPRICEDBASKETPAYMENTTYPE)

            Case GlobalConstants.CHIPANDPINPAYMENTTYPE
                rdoChipAndPin.Checked = True
                Dim dtChipAndPinDevices As DataTable = TDataObjects.PaymentSettings.TblChipAndPinDevices.GetByBUPartner(_businessUnit, _partner)
                Dim selectedValue As String = getSelectedValue(dtChipAndPinDevices)
                ddlChipAndPinDevices.DataSource = dtChipAndPinDevices
                ddlChipAndPinDevices.DataTextField = "TERMINAL_DEVICE_NAME"
                ddlChipAndPinDevices.DataValueField = "ID"
                ddlChipAndPinDevices.DataBind()
                If selectedValue.Length > 0 Then ddlChipAndPinDevices.SelectedValue = selectedValue
                ltlPaymentOption.Text = lblChipAndPin.Text
                If AgentProfile.IsAgent Then
                    Dim txtcc As TextBox = Me.uscChipAndPinPartPayment.FindControl("txtPartPmt")
                    txtcc.Text = Convert.ToDecimal(Profile.Basket.BasketSummary.TotalBasket).ToString("F2")
                End If
                Checkout.StoreCurrentPaymentType(GlobalConstants.CHIPANDPINPAYMENTTYPE)
                updProgressCheckout.Visible = False
                Dim scmMainScriptManager As ScriptManager = Master.FindControl("scmMainScriptManager")
                If scmMainScriptManager IsNot Nothing Then
                    scmMainScriptManager.AsyncPostBackTimeout = TEBUtilities.CheckForDBNull_Int(_wfrPage.Attribute("ChipAndPinTimeoutValue"))
                End If

            Case GlobalConstants.POINTOFSALEPAYMENTTYPE
                rdoPointOfSale.Checked = True
                Dim dtPointOfSaleTerminals As DataTable = TDataObjects.PaymentSettings.TblPointOfSaleTerminals.GetByBUPartner(_businessUnit, _partner)
                ddlPointOfSaleTerminals.DataSource = dtPointOfSaleTerminals
                ddlPointOfSaleTerminals.DataTextField = "POS_TERMINAL_NAME"
                ddlPointOfSaleTerminals.DataValueField = "ID"
                ddlPointOfSaleTerminals.DataBind()
                ltlPaymentOption.Text = lblPointOfSale.Text
                Checkout.StoreCurrentPaymentType(GlobalConstants.POINTOFSALEPAYMENTTYPE)
                updProgressCheckout.Visible = False
                Dim scmMainScriptManager As ScriptManager = Master.FindControl("scmMainScriptManager")
                If scmMainScriptManager IsNot Nothing Then
                    scmMainScriptManager.AsyncPostBackTimeout = TEBUtilities.CheckForDBNull_Int(_wfrPage.Attribute("PointOfSaleTimeoutValue"))
                End If

                ' Default the PoS terminal details passed in query string from PoS terminal 
                If Not Session("posip") Is Nothing Then _posIpAddress = Session("posip")
                If Not Session("posport") Is Nothing Then _posTcpPort = Session("posport")
                Dim boolfound As Boolean = False
                If _posIpAddress <> String.Empty And _posTcpPort <> String.Empty Then
                    For Each row As DataRow In dtPointOfSaleTerminals.Rows
                        If row("POS_TERMINAL_IP_ADDRESS").ToString() = _posIpAddress And row("POS_TERMINAL_TCP_PORT") = _posTcpPort Then
                            ddlPointOfSaleTerminals.SelectedValue = row("ID").ToString()
                            boolfound = True
                        Else
                            Dim item As ListItem = ddlPointOfSaleTerminals.Items.FindByText("POS_TERMINAL_NAME")
                            ddlPointOfSaleTerminals.Items.Remove(item)
                        End If
                    Next
                    If Not boolfound Then
                        blErrorMessages.Items.Clear()
                        blErrorMessages.Items.Add(_errMsg.GetErrorMessage(TCUtilities.GetAllString, TEBUtilities.GetCurrentPageName, "InvalidPoSTerminal").ERROR_MESSAGE + " (posip=" + _posIpAddress + ", posport=" + _posTcpPort + ")")
                        plhPointOfSale.Visible = False
                        _nextItemToOpen = 0
                    End If
                End If

            Case GlobalConstants.DDPAYMENTTYPE
                rdoDirectDebit.Checked = True
                ltlPaymentOption.Text = lblDirectDebit.Text
                uscDirectDebit.ResetForm()
                If uscDirectDebit.ErrorMessage.Length > 0 Then
                    plhErrorList.Visible = True
                    blErrorMessages.Items.Clear()
                    blErrorMessages.Items.Add(uscDirectDebit.ErrorMessage)
                End If
                If Profile.Basket.BasketSummary.TotalPartPayments > 0 Then
                    blErrorMessages.Items.Clear()
                    blErrorMessages.Items.Add(_errMsg.GetErrorMessage(TCUtilities.GetAllString, TEBUtilities.GetCurrentPageName, "CannotPayWithDDWhenEPHasBeenUsed").ERROR_MESSAGE)
                    plhDirectDebit.Visible = False
                    _nextItemToOpen = 0
                End If
                If payingForPPS Then
                    Checkout.StoreCurrentPaymentType(GlobalConstants.CSPAYMENTTYPE)
                Else
                    Checkout.StoreCurrentPaymentType(GlobalConstants.DDPAYMENTTYPE)
                End If

            Case GlobalConstants.CBPAYMENTTYPE
                rdoCashback.Checked = True
                ltlPaymentOption.Text = lblCashback.Text
                If uscApplyCashback.ErrorMessage.Length > 0 Then
                    plhErrorList.Visible = True
                    blErrorMessages.Items.Clear()
                    blErrorMessages.Items.Add(uscApplyCashback.ErrorMessage)
                    _nextItemToOpen = 0
                End If
                If uscApplyCashback.SuccessMessage.Length > 0 Then
                    plhSuccessMessage.Visible = True
                    ltlSuccessMessages.Text = uscApplyCashback.SuccessMessage
                    _nextItemToOpen = 0
                End If
                Checkout.StoreCurrentPaymentType(GlobalConstants.OAPAYMENTTYPE)

            Case GlobalConstants.PAYPALPAYMENTTYPE
                rdoPayPal.Checked = True
                ltlPaymentOption.Text = lblPayPal.Text
                Session.Remove("ExternalGatewayName")
                Session.Remove("PayPalStage")
                Session.Remove("ExternalGatewayURL")
                Session("ExternalGatewayURL") = getPayPalRedirectFunction()

            Case GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE
                rdoGoogleCheckout.Checked = True
                ltlPaymentOption.Text = lblGoogleCheckout.Text
                Session.Remove("ExternalGatewayName")
                Session.Remove("GoogleCheckoutStage")
                Session.Remove("ExternalGatewayURL")
                Session("ExternalGatewayURL") = "~/Redirect/GoogleCheckoutGateway.aspx?page=checkoutpaymentdetails.aspx&function=paymentexternalcheckout"

            Case GlobalConstants.SAVEDCARDPAYMENTTYPE
                rdoSavedCard.Checked = True
                ltlPaymentOption.Text = lblSavedCard.Text
                If AgentProfile.IsAgent Then
                    Dim txtcc As TextBox = Me.uscSCPartPayment.FindControl("txtPartPmt")
                    txtcc.Text = Convert.ToDecimal(Profile.Basket.BasketSummary.TotalBasket).ToString("F2")
                End If
                uscMySavedCards.ShowSecurityNumber = Not payingForPPS
                uscMySavedCards.ResetForm()

            Case GlobalConstants.EPURSEPAYMENTTYPE
                rdoEPurse.Checked = True
                ltlPaymentOption.Text = lblEPurse.Text
                If uscEPurse.ErrorList.Items.Count > 0 Then
                    plhErrorList.Visible = True
                    blErrorMessages.Items.Clear()
                    For Each item As ListItem In uscEPurse.ErrorList.Items
                        blErrorMessages.Items.Add(item)
                    Next
                End If
                If uscEPurse.SuccessList.Items.Count > 0 Then
                    plhSuccessMessage.Visible = True
                    For Each item As ListItem In uscEPurse.SuccessList.Items
                        ltlSuccessMessages.Text = item.Text
                    Next
                End If
                Checkout.StoreCurrentPaymentType(GlobalConstants.EPURSEPAYMENTTYPE)

            Case GlobalConstants.INVPAYMENTTYPE
                rdoInvoice.Checked = True
                ltlPaymentOption.Text = lblInvoice.Text
                Checkout.StoreCurrentPaymentType(GlobalConstants.INVPAYMENTTYPE)

            Case Else
                'No Payment Option Selected
                If _currentPaymentMethod.Length = 0 AndAlso Not uscBasketFees.BasketFeesUpdated AndAlso Not uscBasketSummary.BasketSummaryUpdated AndAlso Not uscBasketSummary.PartPaymentOptionsChanged Then
                    blErrorMessages.Items.Clear()
                    blErrorMessages.Items.Add(_wfrPage.Content("NoPaymentOptionSelected", _languageCode, True))
                End If
                _nextItemToOpen = 0
                Checkout.StoreCurrentPaymentType(String.Empty)

        End Select

        Dim basketMini As Object = Talent.eCommerce.Utilities.FindWebControl("MiniBasket1", Me.Page.Controls)
        If basketMini IsNot Nothing Then
            CallByName(basketMini, "ReBindBasket", CallType.Method)
        End If

        If Profile.Basket.BasketSummary.TotalPartPayments > 0 Then
            uscBasketFees.DisableFormElements = True
            txtPromotions.ReadOnly = True
            btnPromotions.Enabled = False
        Else
            For Each item As TalentBasketItem In Profile.Basket.BasketItems
                If item.Product = ModuleDefaults.CashBackFeeCode Then
                    uscBasketFees.DisableFormElements = True
                    txtPromotions.ReadOnly = True
                    btnPromotions.Enabled = False
                    Exit For
                End If
            Next
        End If

        If (Not _singlePaymentMethod) AndAlso canRegisterAccordion Then
            registerAcordionMenuScript(_nextItemToOpen)
        End If
    End Sub

    ''' <summary>
    ''' Popylate the drop down list for other payment types
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PopulateDropDownForOthers()
        Dim isOthersTextMandatoryDictionary As New Dictionary(Of String, Boolean)
        ddlOthersType.DataSource = GetOtherTypes(isOthersTextMandatoryDictionary)
        ddlOthersType.DataTextField = "Text"
        ddlOthersType.DataValueField = "Value"
        ddlOthersType.DataBind()
        PrepareClientSideScriptToValidateOthersMandatoryText(isOthersTextMandatoryDictionary)
    End Sub

    ''' <summary>
    ''' Prepare script for other payment type validation
    ''' </summary>
    ''' <param name="isOthersTextMandatoryDictionary"></param>
    ''' <remarks></remarks>
    Private Sub PrepareClientSideScriptToValidateOthersMandatoryText(ByVal isOthersTextMandatoryDictionary As Dictionary(Of String, Boolean))
        csvIsOthersTextMandatory.Enabled = True
        csvIsOthersTextMandatory.Display = ValidatorDisplay.None
        Dim StringForOthersValidation As String
        StringForOthersValidation = String.Empty
        For Each pair In isOthersTextMandatoryDictionary
            StringForOthersValidation = StringForOthersValidation & pair.Key & ":" & pair.Value & ";"
        Next
        hdnOthers.Value = StringForOthersValidation
        hdnOthersErrorMessage.Value = _wfrPage.Content("OthersPaymentTypeMandatoryErrorMessage", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Check all basket contents can be bought with finance and can be bought together 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub validateBasketforCreditFinance()
        Dim FirstFinanceClubProductID = String.Empty
        Dim OnlyAvailablePlan As String = String.Empty
        Dim prodDescription1 As String = String.Empty
        Dim prodDescription2 As String = String.Empty
        Dim messageText As String = String.Empty
        blErrorMessages.Items.Clear()


        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems

            If (Not TEBUtilities.IsTicketingFee(tbi.MODULE_, tbi.Product, tbi.FEE_CATEGORY)) AndAlso (tbi.PRODUCT_TYPE <> GlobalConstants.PPSPRODUCTTYPE) Then

                If FirstFinanceClubProductID = String.Empty Then
                    FirstFinanceClubProductID = Trim(tbi.FINANCE_CLUB_PRODUCT_ID)
                    uscCreditFinance.clubProductCodeForFinance = Trim(tbi.FINANCE_CLUB_PRODUCT_ID)
                    prodDescription1 = tbi.PRODUCT_DESCRIPTION1
                End If

                If OnlyAvailablePlan = String.Empty AndAlso tbi.FINANCE_PLAN_ID <> String.Empty Then
                    OnlyAvailablePlan = Trim(tbi.FINANCE_PLAN_ID)
                    prodDescription2 = tbi.PRODUCT_DESCRIPTION1
                    ' Setting this makes finance plan drop down show just this plan 
                    uscCreditFinance.OnlyAvailablePlan = OnlyAvailablePlan
                End If

                'All products must be available to buy with finance 
                If tbi.FINANCE_CLUB_PRODUCT_ID = String.Empty AndAlso tbi.Gross_Price <> 0 Then
                    messageText = _errMsg.GetErrorMessage(TCUtilities.GetAllString, TEBUtilities.GetCurrentPageName, "CannotUseCFforThisProduct").ERROR_MESSAGE
                    messageText = messageText.Replace("<<<description>>>", tbi.PRODUCT_DESCRIPTION1)
                    Exit For
                End If

                'Plan is optional. If not set product it can be bought using any plan but if set only this plan can be used to buy this product.
                If OnlyAvailablePlan <> String.Empty AndAlso tbi.FINANCE_PLAN_ID <> String.Empty AndAlso Trim(tbi.FINANCE_PLAN_ID) <> OnlyAvailablePlan Then
                    messageText = _errMsg.GetErrorMessage(TCUtilities.GetAllString, TEBUtilities.GetCurrentPageName, "CannotBuyTheseTogetherWithCF").ERROR_MESSAGE
                    messageText = messageText.Replace("<<<productdescription1>>>", prodDescription2)
                    messageText = messageText.Replace("<<<productdescription2>>>", tbi.PRODUCT_DESCRIPTION1)
                    Exit For
                End If

                ' All products must have the same finance club/product ID
                If Trim(tbi.FINANCE_CLUB_PRODUCT_ID) <> FirstFinanceClubProductID Then
                    messageText = _errMsg.GetErrorMessage(TCUtilities.GetAllString, TEBUtilities.GetCurrentPageName, "CannotBuyTheseTogetherWithCF").ERROR_MESSAGE
                    messageText = messageText.Replace("<<<productdescription1>>>", prodDescription1)
                    messageText = messageText.Replace("<<<productdescription2>>>", tbi.PRODUCT_DESCRIPTION1)
                    Exit For
                End If
            End If
        Next

        If messageText <> String.Empty Then
            blErrorMessages.Items.Add(messageText)
            plhCreditFinance.Visible = False
            rdoCreditFinance.Checked = False
        End If
    End Sub

    ''' <summary>
    ''' Reset the payment options step.
    ''' </summary>
    ''' <param name="resetRadioButtons">A boolean value to indicate that the radio buttons are reset</param>
    ''' <remarks></remarks>
    Private Sub resetPaymentOptions(ByVal resetRadioButtons As Boolean)
        plhCCType.Visible = False
        plhCCExternal.Visible = False
        plhCreditCard.Visible = False
        plhSavedCard.Visible = False
        plhPayPal.Visible = False
        plhGoogleCheckout.Visible = False
        plhCashback.Visible = False
        plhEPurse.Visible = False
        plhDirectDebit.Visible = False
        plhCreditFinance.Visible = False
        plhCash.Visible = False
        plhOthers.Visible = False
        plhOnAccount.Visible = False
        plhChipAndPin.Visible = False
        plhZeroPricedBasket.Visible = False
        plhPointOfSale.Visible = False
        plhInvoice.Visible = False
        plhPDQ.Visible = False
        If resetRadioButtons Then
            rdoCreditCard.Checked = False
            rdoSavedCard.Checked = False
            rdoPayPal.Checked = False
            rdoGoogleCheckout.Checked = False
            rdoCashback.Checked = False
            rdoEPurse.Checked = False
            rdoDirectDebit.Checked = False
            rdoCreditFinance.Checked = False
            rdoCash.Checked = False
            rdoOthers.Checked = False
            rdoOnAccount.Checked = False
            rdoChipAndPin.Checked = False
            rdoZeroPricedBasket.Checked = False
            rdoPointOfSale.Checked = False
            rdoPDQ.Checked = False
            rdoInvoice.Checked = False
        End If
    End Sub

    ''' <summary>
    ''' Setup the PPS/Season ticket payment flow box. Used to indicate which items have been paid for and how
    ''' </summary>
    ''' <param name="basketPPS1DescriptionList">The descriptive list of PPS1 items</param>
    ''' <param name="basketPPS2DescriptionList">The descriptive list of PPS2 items</param>
    ''' <param name="seasonTicketDescription">The descriptive name for the season ticket purchased</param>
    ''' <remarks></remarks>
    Private Sub setupPPSItemsPaidFor(ByVal basketPPS1DescriptionList As Collection, ByVal basketPPS1SeatDescriptionList As Collection, _
                                     ByVal basketPPS2DescriptionList As Collection, ByVal basketPPS2SeatDescriptionList As Collection, ByVal seasonTicketDescription As String)
        Const NOTPAIDCSSCLASS As String = "not-paid"
        Const PAIDCSSCLASS As String = "paid"

        If basketPPS1DescriptionList.Count > 0 Then
            plhPPSList.Visible = True
            plhPPSListPPS1.Visible = True
            ltlPPS1PaidUsingLabel.Text = _wfrPage.Content("PaidUsingLabel", _languageCode, True)
            blPPS1List.Items.Clear()
            Dim i As Integer = 1
            For Each item As String In basketPPS1DescriptionList
                Dim paymentOptionMessage As String = _wfrPage.Content("MultiplePPSPaymentOptionsMessage2", _languageCode, True)
                paymentOptionMessage = paymentOptionMessage.Replace("<<ProductDescription>>", item)
                paymentOptionMessage = paymentOptionMessage.Replace("<<SeatDetails>>", basketPPS1SeatDescriptionList(i))
                blPPS1List.Items.Add(paymentOptionMessage)
                i += 1
            Next
            If Session("PPS1PaymentComplete") = True Then
                CSSClassPPS1 = PAIDCSSCLASS
                Dim payType As String = Checkout.RetrievePaymentItemFromSession("PaymentType", GlobalConstants.PPS1STAGE)
                Select Case payType
                    Case GlobalConstants.CCPAYMENTTYPE : ltlPPS1PaidUsing.Text = _wfrPage.Content("CreditCardPaymentMethodLabel", _languageCode, True)
                    Case GlobalConstants.SAVEDCARDPAYMENTTYPE : ltlPPS1PaidUsing.Text = _wfrPage.Content("SavedCardPaymentMethodLabel", _languageCode, True)
                    Case GlobalConstants.DDPAYMENTTYPE : ltlPPS1PaidUsing.Text = _wfrPage.Content("DirectDebitPaymentMethodLabel", _languageCode, True)
                End Select
                plhResetPayments.Visible = True
            Else
                CSSClassPPS1 = NOTPAIDCSSCLASS
                ltlPPS1PaidUsing.Text = _wfrPage.Content("NotPaidForLabel", _languageCode, True)
            End If
        End If

        If basketPPS2DescriptionList.Count > 0 Then
            plhPPSList.Visible = True
            plhPPSListPPS2.Visible = True
            ltlPPS2PaidUsingLabel.Text = _wfrPage.Content("PaidUsingLabel", _languageCode, True)
            blPPS2List.Items.Clear()
            Dim i As Integer = 1
            For Each item As String In basketPPS2DescriptionList
                Dim paymentOptionMessage As String = _wfrPage.Content("MultiplePPSPaymentOptionsMessage2", _languageCode, True)
                paymentOptionMessage = paymentOptionMessage.Replace("<<ProductDescription>>", item)
                paymentOptionMessage = paymentOptionMessage.Replace("<<SeatDetails>>", basketPPS2SeatDescriptionList(i))
                blPPS2List.Items.Add(paymentOptionMessage)
                i += 1
            Next
            If Session("PPS2PaymentComplete") = True Then
                CSSClassPPS2 = PAIDCSSCLASS
                Dim payType As String = Checkout.RetrievePaymentItemFromSession("PaymentType", GlobalConstants.PPS2STAGE)
                Select Case payType
                    Case GlobalConstants.CCPAYMENTTYPE : ltlPPS2PaidUsing.Text = _wfrPage.Content("CreditCardPaymentMethodLabel", _languageCode, True)
                    Case GlobalConstants.SAVEDCARDPAYMENTTYPE : ltlPPS2PaidUsing.Text = _wfrPage.Content("SavedCardPaymentMethodLabel", _languageCode, True)
                    Case GlobalConstants.DDPAYMENTTYPE : ltlPPS2PaidUsing.Text = _wfrPage.Content("DirectDebitPaymentMethodLabel", _languageCode, True)
                End Select
                plhResetPayments.Visible = True
            Else
                CSSClassPPS2 = NOTPAIDCSSCLASS
                ltlPPS2PaidUsing.Text = _wfrPage.Content("NotPaidForLabel", _languageCode, True)
            End If
        End If

        If seasonTicketDescription.Length > 0 Then
            plhPPSListSeasonTicket.Visible = True
            ltlSeasonTicket.Text = seasonTicketDescription
            ltlSeasonTicketPaidUsingLabel.Text = _wfrPage.Content("PaidUsingLabel", _languageCode, True)
            ltlSeasonTicketPaidUsing.Text = _wfrPage.Content("NotPaidForLabel", _languageCode, True)
        End If

        If plhResetPayments.Visible Then ltlResetPayments.Text = _wfrPage.Content("ResetPaymentsButton", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Retrieve the saved card details from cache/TALENT WS060R call based on the selected card used for payment
    ''' </summary>
    ''' <param name="uniqueCardId">The unique ID in CD051 of the card</param>
    ''' <param name="cv2Number">The CV2 Number entered by the user</param>
    ''' <param name="cardNumber">A reference to the card number string</param>
    ''' <param name="expiryDate">A reference to the card expiry date string</param>
    ''' <param name="issueNumber">A reference to the card issue number string</param>
    ''' <param name="startDate">A reference to the card start date string</param>
    ''' <remarks></remarks>
    Private Sub getStoredCCDetailsFromCache(ByVal uniqueCardId As String, ByVal cv2Number As String, ByRef cardNumber As String, _
                                        ByRef expiryDate As String, ByRef startDate As String, ByRef issueNumber As String, ByRef vgTokenID As String, ByRef vgProcessingDB As String, ByRef vgTokenDate As String)
        Dim err As New ErrorObj
        Dim payment As New TalentPayment
        Dim dePayment As New DEPayments
        If ModuleDefaults.PaymentGatewayType = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
            dePayment.PaymentType = GlobalConstants.PAYMENTTYPE_VANGUARD
        End If
        dePayment.SessionId = Profile.Basket.Basket_Header_ID
        dePayment.CustomerNumber = Profile.UserName
        dePayment.Source = "W"
        payment.De = dePayment
        payment.Settings = TEBUtilities.GetSettingsObject()
        err = payment.RetrieveMySavedCards

        If Not err.HasError Then
            If payment.ResultDataSet.Tables.Count > 1 Then
                If payment.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    If String.IsNullOrWhiteSpace(payment.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString) Then
                        If payment.ResultDataSet.Tables("CardDetails").Rows.Count > 0 Then
                            For Each row As Data.DataRow In payment.ResultDataSet.Tables("CardDetails").Rows
                                If row("UniqueCardId").ToString.Equals(uniqueCardId) Then
                                    cardNumber = row("CardNumber").ToString
                                    expiryDate = row("ExpiryDate").ToString
                                    startDate = row("StartDate").ToString
                                    issueNumber = row("IssueNumber").ToString
                                    vgTokenID = row("VGTokenID").ToString
                                    vgProcessingDB = row("VGProcessingDB").ToString
                                    vgTokenDate = row("VGTokenDate").ToString
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Fire a javascript function to the browser so that the itemToOpen is opened in the accordion menu
    ''' </summary>
    ''' <param name="itemToOpen">The option that must be expanded in the accordion menu</param>
    ''' <param name="okToMoveToNextItem">Set to false if stay on the current option</param>
    ''' <remarks></remarks>
    Private Sub registerAcordionMenuScript(ByVal itemToOpen As Integer, Optional ByVal okToMoveToNextItem As Boolean = True, Optional ByVal canConsiderCCType As Boolean = True)
        If IsPostBack Then
            If plhMarketingCampaigns.Visible Then itemToOpen += 1
            If plhDeliveryAddress.Visible AndAlso okToMoveToNextItem Then
                If (Session("ddlSelectAddressChanged") Is Nothing) Or (Session("ddlSelectAddressChanged") IsNot Nothing AndAlso Session("ddlSelectAddressChanged") = False) Then
                    itemToOpen += 1
                End If
            End If
            If plhCCType.Visible AndAlso Session("plhCCExternal") IsNot Nothing AndAlso CType(Session("plhCCExternal"), Boolean) AndAlso canConsiderCCType Then itemToOpen += 1
        End If
        Dim javascriptString As New StringBuilder
        javascriptString.Append("$(function () {")
        javascriptString.Append("    $("".ebiz-checkout-accordion"").accordion({ icons: false, heightStyle: ""content"", header: ""div.header"" });")
        javascriptString.Append("    $("".ebiz-checkout-accordion"").accordion(""option"", ""active"", ")
        javascriptString.Append(itemToOpen)
        javascriptString.Append(");")
        javascriptString.Append("});")
        ScriptManager.RegisterStartupScript(btnPaymentOptionSelected, Me.GetType(), "AccordionMenu", javascriptString.ToString(), True)
    End Sub

    Private Sub setCCControlsByPaymentGateway(ByVal callingMethodName As String)
        Dim isVanguard As Boolean = (ModuleDefaults.PaymentGatewayType.ToUpper() = GlobalConstants.PAYMENTGATEWAY_VANGUARD)

        Select Case callingMethodName
            Case "IsDeliveryAddressValid"
                If isVanguard Then
                    If Session("basketPPS1List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") Is Nothing Then
                        If plhCCExternal.Visible Then plhCCExternal.Visible = False
                        plhCCType.Visible = False
                    ElseIf Session("basketPPS2List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") Is Nothing Then
                        If plhCCExternal.Visible Then plhCCExternal.Visible = False
                        plhCCType.Visible = False
                    Else
                        If plhCCType.Visible Then plhCCType.Visible = False
                    End If
                Else
                    If plhCreditCard.Visible Then plhCreditCard.Visible = False
                End If
            Case "paymentSelected"
                If isVanguard Then
                    If Session("basketPPS1List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") Is Nothing Then
                        plhCCType.Visible = False
                        plhCreditCard.Visible = False
                        plhCCExternal.Visible = True
                    ElseIf Session("basketPPS2List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") Is Nothing Then
                        plhCCType.Visible = False
                        plhCreditCard.Visible = False
                        plhCCExternal.Visible = True
                    Else
                        plhCCType.Visible = True
                        plhCreditCard.Visible = False
                        If plhCCType.Visible AndAlso Session("plhCCExternal") IsNot Nothing AndAlso CType(Session("plhCCExternal"), Boolean) Then
                            plhCCExternal.Visible = True
                        End If
                    End If
                Else
                    plhCCType.Visible = False
                    plhCCExternal.Visible = False
                    plhCreditCard.Visible = True
                End If
            Case "determineWhatOptionsEnabled"
                If isVanguard Then
                    If Session("basketPPS1List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") Is Nothing Then
                        ltlCCExternalNumber.Text = _numberCounter.ToString
                        _numberCounter += 1
                    ElseIf Session("basketPPS2List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") Is Nothing Then
                        ltlCCExternalNumber.Text = _numberCounter.ToString
                        _numberCounter += 1
                    Else
                        ltlCCTypeNumber.Text = _numberCounter.ToString
                        _numberCounter += 1
                        ltlCCExternalNumber.Text = _numberCounter.ToString()
                    End If
                Else
                    ltlCreditCardNumber.Text = _numberCounter.ToString
                End If
        End Select

    End Sub

    ''' <summary>
    ''' Common function to process the promotion entered into the promotion text box. Called from btnTicketingPromotions click or partner promotion is in use.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub processPromotion()
        _doLayoutFunctions = True
        Session("TicketingGatewayError") = Nothing
        Select Case ModuleDefaults.CheckoutPromotionType
            Case Is = GlobalConstants.TICKETING_PROMOTION_TYPE
                Dim promotions As TalentPromotions = New TalentPromotions()
                Dim _dePromotions As New DEPromotions(Profile.Basket.Basket_Header_ID, Nothing, "", "", Nothing, "", "", "", 0, txtPromotions.Text, 0, "", "", "", "", "", "")
                _dePromotions.BasketTotal = Profile.Basket.BasketSummary.TotalBasketWithoutPayTypeFee
                promotions.Dep = _dePromotions
                promotions.Settings = TEBUtilities.GetSettingsObject
                promotions.Settings.BusinessUnit = TalentCache.GetBusinessUnit
                promotions.Settings.StoredProcedureGroup = TEBUtilities.GetStoredProcedureGroup
                If Profile.IsAnonymous Then
                    promotions.Settings.LoginId = GlobalConstants.GENERIC_CUSTOMER_NUMBER
                Else
                    promotions.Settings.LoginId = Profile.User.Details.LoginID
                End If
                promotions.Settings.OriginatingSourceCode = GlobalConstants.SOURCE
                promotions.Settings.OriginatingSource = TEBUtilities.GetOriginatingSource(Session.Item("Agent"))
                Dim err As ErrorObj = promotions.ProcessPromotions()

                _doLayoutFunctions = False
                If err.HasError Then
                    Session("TicketingGatewayError") = err.ErrorNumber
                Else
                    If Not _TicketingGatewayFunctions.ProcessNewBasket(promotions.ResultDataSet) Then
                        Session("TicketingGatewayError") = "PNBERR"
                    Else
                        ltlSuccessMessages.Text = String.Empty
                        If promotions.ResultDataSet.Tables("PromotionStatus").Rows.Count > 0 Then
                            Dim successCode As String = promotions.ResultDataSet.Tables("PromotionStatus").Rows(0)("Successcode")
                            If successCode.Equals("P") Then
                                Session("TicketingPromotionsSuccess") = _wfrPage.Content("PromotionApplied", _languageCode, True)
                            ElseIf successCode.Equals("E") Then
                                Session("TicketingPromotionsSuccess") = _wfrPage.Content("VoucherAppliedE", _languageCode, True)
                            ElseIf successCode.Equals("X") Then
                                Session("TicketingPromotionsWarning") = _wfrPage.Content("VoucherAppliedX", _languageCode, True)
                            ElseIf successCode.Equals("G") Then
                                Session("TicketingPromotionsSuccess") = _wfrPage.Content("VoucherAppliedG", _languageCode, True)
                            End If
                        End If
                        If plhAutoEnrolPPS.Visible AndAlso chkAutoEnrolPPS.Checked AndAlso Profile.Basket.BasketSummary.TotalBasketWithoutPayTypeFee = 0 Then
                            Session("TicketingPromotionsSuccess") = _wfrPage.Content("CreditCardNoPayment", _languageCode, True)
                        End If
                        Response.Redirect(Request.Url.AbsoluteUri)
                    End If
                End If

            Case Is = GlobalConstants.RETAIL_PROMOTION_TYPE
                Try
                    Dim totals As Talent.Common.TalentWebPricing = Profile.Basket.WebPrices
                    Dim currentPromoCode As String = txtPromotions.Text.Trim()
                    If (currentPromoCode.Length > 0) Then
                        Session("UserFromPromotionsBox") = Profile.UserName.Trim().ToLower()
                        Session("CodeFromPromotionsBox") = txtPromotions.Text.Trim()
                        If (Session("AllPrmotionCodesEnteredByUser") IsNot Nothing) Then
                            Dim PromotionCodes As String()
                            Dim charSeparators() As Char = {";"c}
                            PromotionCodes = (CStr(Session("AllPrmotionCodesEnteredByUser"))).Split(charSeparators, System.StringSplitOptions.RemoveEmptyEntries)
                            Dim totalCodesIndex As Integer = (PromotionCodes.Length - 1)
                            Dim addCode As Boolean = True
                            For codeCount As Integer = totalCodesIndex To 0 Step -1
                                If UCase(currentPromoCode) = UCase(PromotionCodes(codeCount)) Then
                                    addCode = False
                                    Exit For
                                End If
                            Next
                            If addCode Then
                                Session("AllPrmotionCodesEnteredByUser") = Session("AllPrmotionCodesEnteredByUser") & ";" & currentPromoCode
                            End If
                        Else
                            Session("AllPrmotionCodesEnteredByUser") = currentPromoCode
                        End If
                    End If
                    Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
                Catch ex As Exception
                    Response.Redirect(Request.Url.AbsoluteUri)
                End Try
        End Select
        _nextItemToOpen = 0
        registerAcordionMenuScript(_nextItemToOpen)
    End Sub
    Private Sub ResetPPSPayment(ByVal resetMode As Integer)
        If resetMode = 0 Then 'page init
            If (Not Page.IsPostBack) AndAlso (Not String.IsNullOrWhiteSpace(Request.QueryString("clearpps"))) Then
                If Request.QueryString("clearpps") = "true" Then
                    Session("PPS1PaymentComplete") = Nothing
                    Session("PPS2PaymentComplete") = Nothing
                    Session("basketPPS1List") = Nothing
                    Session("basketPPS2List") = Nothing
                    Session("VGPaidPPS1") = Nothing
                    Session("VGPaidPPS2") = Nothing
                End If
            End If
        ElseIf resetMode = 1 Then 'page load
            Dim canReset As Boolean = True
            If Not Page.IsPostBack Then
                If ModuleDefaults.PaymentGatewayType = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
                    If Request.QueryString("_rvgpps") IsNot Nothing Then
                        If Request.QueryString("_rvgpps") = "pps1" OrElse Request.QueryString("_rvgpps") = "pps2" Then
                            If Session("VGPPSRedirect") IsNot Nothing AndAlso Session("VGPPSRedirect") = "STEP1" Then
                                Session("VGPPSRedirect") = "STEP2"
                                If Session("VGPaidPPS1") Is Nothing OrElse Session("VGPaidPPS1") <> True Then
                                    canReset = False
                                ElseIf Session("VGPaidPPS2") Is Nothing OrElse Session("VGPaidPPS2") <> True Then
                                    canReset = False
                                ElseIf Session("VGPaidPPS1") IsNot Nothing AndAlso Session("VGPaidPPS1") = True AndAlso Session("VGPaidPPS2") IsNot Nothing AndAlso Session("VGPaidPPS2") = True Then
                                    canReset = False
                                End If
                            End If
                        End If
                    End If
                Else
                    If Session("PPS1PaymentComplete") Is Nothing OrElse Session("PPS1PaymentComplete") <> True Then
                        canReset = False
                    ElseIf Session("PPS2PaymentComplete") Is Nothing OrElse Session("PPS2PaymentComplete") <> True Then
                        canReset = False
                    ElseIf Session("PPS1PaymentComplete") IsNot Nothing AndAlso Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") IsNot Nothing AndAlso Session("PPS2PaymentComplete") = True Then
                        canReset = False
                    End If
                End If
            Else
                canReset = False
            End If

            If canReset Then
                Session("ExternalGatewayPayType") = Nothing
                Session("PPS1PaymentComplete") = Nothing
                Session("PPS2PaymentComplete") = Nothing
                Session("basketPPS1List") = Nothing
                Session("basketPPS2List") = Nothing
                Session("VGPaidPPS1") = Nothing
                Session("VGPaidPPS2") = Nothing
            End If
        End If
    End Sub

    ''' <summary>
    ''' Confirm the saved card payment process
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ConfirmSavedCardPayment()
        _doLayoutFunctions = True
        plhSavedCard.Visible = True
        csvSavedCardTAndC.Validate()
        If csvSavedCardTAndC.IsValid AndAlso isPageValid() Then
            Dim checkoutStage As String = GlobalConstants.CHECKOUTASPXSTAGE
            Session("customerPresent") = chkCustomerPresentSC.Checked
            'Check the PPS List session object to see if we are saving the payment details or completing a sale
            If Session("basketPPS1List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") Is Nothing Then
                checkoutStage = GlobalConstants.PPS1STAGE
                If Session("PPS1PaymentComplete") Is Nothing Then
                    Session("PPS1PaymentComplete") = True
                End If
            Else
                If Session("basketPPS2List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") Is Nothing Then
                    checkoutStage = GlobalConstants.PPS2STAGE
                    If Session("PPS2PaymentComplete") Is Nothing Then
                        Session("PPS2PaymentComplete") = True
                    End If
                End If
            End If
            If _payingForPPS = False AndAlso checkoutStage = GlobalConstants.CHECKOUTASPXSTAGE Then Session("TakeFinalPayment") = True

            Dim cardNumber As String = String.Empty
            Dim startDate As String = String.Empty
            Dim expiryDate As String = String.Empty
            Dim issueNumber As String = String.Empty
            Dim vgTokenId As String = String.Empty
            Dim vgProcessingDB As String = String.Empty
            Dim vgTokenDate As String = String.Empty
            If is3DSecureInUse() Or checkoutStage <> GlobalConstants.CHECKOUTASPXSTAGE Then
                'Get the card details from TALENT if we need them for 3DSecure or PPS processing
                getStoredCCDetailsFromCache(uscMySavedCards.SelectedCard, uscMySavedCards.SecurityNumber, cardNumber, expiryDate, startDate, issueNumber, vgTokenId, vgProcessingDB, vgTokenDate)
            End If
            Checkout.StoreSCDetails(cardNumber, expiryDate, startDate, issueNumber, uscMySavedCards.SecurityNumber, _
                                    String.Empty, uscMySavedCards.SelectedCard, _
                                    vgTokenId, vgProcessingDB, vgTokenDate, String.Empty, _
                                    checkoutStage)


            If checkoutStage = GlobalConstants.CHECKOUTASPXSTAGE Then
                If is3DSecureInUse() AndAlso canProcess3DSecure(True) Then
                    Response.Redirect("checkout3dSecure.aspx")
                Else
                    If Session("TakeFinalPayment") = True Then
                        If AgentProfile.IsAgent Then
                            Dim paymentAmount As Decimal = RetrievePaymentAmount()
                            If paymentAmount = Profile.Basket.BasketSummary.TotalBasket Then
                                _TicketingGatewayFunctions.CheckoutPayment(True)
                            Else
                                PartPayment(paymentAmount, True)
                            End If
                        Else
                            _TicketingGatewayFunctions.CheckoutPayment(True)
                        End If
                    End If
                End If
            Else
                _doLayoutFunctions = False
                determineWhatOptionsEnabled()
                _nextItemToOpen = 0
                registerAcordionMenuScript(_nextItemToOpen)
            End If
        Else
            _nextItemToOpen = 1
            registerAcordionMenuScript(_nextItemToOpen)
        End If
    End Sub

    ''' <summary>
    ''' Confirm the saved card payment process using Vangaurd
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ConfirmSavedCardPayment_Vanguard()
        _doLayoutFunctions = True
        plhSavedCard.Visible = True
        csvSavedCardTAndC.Validate()
        If csvSavedCardTAndC.IsValid AndAlso isPageValid() Then
            Dim checkoutStage As String = GlobalConstants.CHECKOUTASPXSTAGE
            Dim checkoutStageVG As String = DEVanguard.CheckoutStages.CHECKOUT
            'Session("customerPresent") = chkCustomerPresentSC.Checked

            'Check the PPS List session object to see if we are saving the payment details or completing a sale
            If Session("basketPPS1List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") Is Nothing Then
                checkoutStage = GlobalConstants.PPS1STAGE
                checkoutStageVG = DEVanguard.CheckoutStages.PPS1
                If Session("PPS1PaymentComplete") Is Nothing Then
                    Session("PPS1PaymentComplete") = True
                End If
            Else
                If Session("basketPPS2List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") Is Nothing Then
                    checkoutStage = GlobalConstants.PPS2STAGE
                    checkoutStageVG = DEVanguard.CheckoutStages.PPS1
                    If Session("PPS2PaymentComplete") Is Nothing Then
                        Session("PPS2PaymentComplete") = True
                    End If
                End If
            End If
            If _payingForPPS = False AndAlso checkoutStage = GlobalConstants.CHECKOUTASPXSTAGE Then Session("TakeFinalPayment") = True

            Dim cardNumber As String = String.Empty
            Dim startDate As String = String.Empty
            Dim expiryDate As String = String.Empty
            Dim issueNumber As String = String.Empty
            Dim vgTokenId As String = String.Empty
            Dim vgProcessingDB As String = String.Empty
            Dim vgTokenDate As String = String.Empty

            Dim paymentAmount As Decimal = 0
            If Not IsNothing(Me.uscSCPartPayment.FindControl("txtPartPmt")) Then
                paymentAmount = DirectCast(Me.uscSCPartPayment.FindControl("txtPartPmt"), TextBox).Text
            Else
                paymentAmount = Profile.Basket.BasketSummary.TotalBasket
            End If


            'Get the card details from TALENT if we need them for 3DSecure or PPS processing
            getStoredCCDetailsFromCache(uscMySavedCards.SelectedCard, uscMySavedCards.SecurityNumber, cardNumber, expiryDate, startDate, issueNumber, vgTokenId, vgProcessingDB, vgTokenDate)
            TEBUtilities.TicketingCheckoutExternalStart(Page.IsPostBack, ModuleDefaults.PaymentGatewayType, paymentAmount < Profile.Basket.BasketSummary.TotalBasket)
            Dim cardTypeCode As String = TCUtilities.GetCardType(TEBUtilities.CheckForDBNull_String(cardNumber), ConfigurationManager.ConnectionStrings("SqlServer2005").ConnectionString)
            'start the vanguard process first
            Dim talVanguard As New TalentVanguard
            Dim err As New ErrorObj
            talVanguard.Settings = TEBUtilities.GetSettingsObject()
            talVanguard.VanguardAttributes.BasketHeaderID = Profile.Basket.Basket_Header_ID
            talVanguard.VanguardAttributes.TempOrderID = Profile.Basket.Temp_Order_Id
            talVanguard.VanguardAttributes.ProcessStep = DEVanguard.ProcessingStep.START_PAYMENT
            talVanguard.VanguardAttributes.PaymentType = GlobalConstants.SAVEDCARDPAYMENTTYPE
            talVanguard.VanguardAttributes.SessionID = Session.SessionID
            talVanguard.VanguardAttributes.CheckOutStage = checkoutStageVG
            talVanguard.VanguardAttributes.CardType = cardTypeCode
            talVanguard.VanguardAttributes.CaptureMethod = uscMySavedCards.CaptureMethod
            talVanguard.VanguardAttributes.SecurityNumber = uscMySavedCards.SecurityNumber

            Dim dicVanguardAttributes As Dictionary(Of String, String) = TDataObjects.AppVariableSettings.TblDefaults.GetVanguardAttributes(talVanguard.Settings.BusinessUnit, _languageCode)
            talVanguard = TEBUtilities.SetVanguardDefaultAttributes(dicVanguardAttributes, talVanguard)
            If Profile.Basket.BasketSummary.TotalBasket < 0 Then
                talVanguard.VanguardAttributes.ProcessIdentifier = DEVanguard.ProcessingIdentifier.CHARGEONLY
                talVanguard.VanguardAttributes.TxnType = DEVanguard.TransactionType.REFUND
            End If
            talVanguard.VanguardAttributes.PaymentAmount = paymentAmount
            talVanguard.VanguardAttributes.BasketAmount = Profile.Basket.BasketSummary.TotalBasket
            talVanguard.VanguardAttributes.ProcessingDB = vgProcessingDB
            'set saved card details
            With talVanguard.BasketPayEntity
                .ADDRESS_LINE_1 = ""
                .AGENT_NAME = ""
                .BASKET_HEADER_ID = talVanguard.VanguardAttributes.BasketHeaderID
                .CAPTUREMETHOD = talVanguard.VanguardAttributes.CaptureMethod
                .CARD_HOLDER_NAME = ""
                .CARD_TYPE = cardTypeCode
                .CARDNUMBER = cardNumber
                .CHECKOUT_STAGE = checkoutStageVG
                .PROCESSING_DB = vgProcessingDB
                .SESSION_GUID = ""
                .SESSION_PASSCODE = ""
                If Not String.IsNullOrWhiteSpace(expiryDate) Then
                    If expiryDate.Trim("0").Length > 0 AndAlso expiryDate.Length > 3 Then
                        .EXPIRYMONTH = expiryDate.Substring(0, 2)
                        .EXPIRYYEAR = expiryDate.Substring(2, 2)
                    End If
                End If
                If Not String.IsNullOrWhiteSpace(startDate) Then
                    If startDate.Trim("0").Length > 0 AndAlso startDate.Length > 3 Then
                        .STARTMONTH = startDate.Substring(0, 2)
                        .STARTYEAR = startDate.Substring(2, 2)
                    End If
                End If
                .PAYMENT_TYPE = talVanguard.VanguardAttributes.PaymentType
                .PAYMENT_AMOUNT = talVanguard.VanguardAttributes.PaymentAmount
                .BASKET_AMOUNT = talVanguard.VanguardAttributes.BasketAmount
                .TOKENID = vgTokenId
                .TXNType = talVanguard.VanguardAttributes.TxnType
                .SAVEDCARD_UNIQUEID = uscMySavedCards.SelectedCard
            End With
            err = talVanguard.ProcessVanguard()
            If (Not err.HasError) AndAlso talVanguard.VanguardAttributes IsNot Nothing Then
                Session("VanguardStage") = "STAGE1"
                TEBUtilities.SetVGAttributesSession(talVanguard.VanguardAttributes.BasketPaymentID, talVanguard.VanguardAttributes, talVanguard.BasketPayEntity)
                Response.Redirect(talVanguard.VanguardAttributes.ReturnURL)
            Else
                _nextItemToOpen = 1
                registerAcordionMenuScript(_nextItemToOpen)
            End If
        Else
            _nextItemToOpen = 1
            registerAcordionMenuScript(_nextItemToOpen)
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Handle basket summary errors
    ''' </summary>
    ''' <returns>Does the basket have an error</returns>
    ''' <remarks></remarks>
    Private Function IsBasketSummaryHasError() As Boolean
        Dim hasError As Boolean = False
        If Profile.Basket.BasketSummary.BasketErrorCode.Trim.Length > 0 Then
            Try
                If Profile.Basket.BasketSummary.BasketErrorCode = GlobalConstants.BSKTSMRY_ERR_CODE_BTC Then
                    If Request.Url.PathAndQuery.Contains("/PagesLogin/Checkout/Checkout.aspx") Then
                        Response.Redirect(Request.Url.AbsoluteUri)
                    End If
                Else
                    Dim talentErrorMsg As TalentErrorMessage = _errMsg.GetErrorMessage(Profile.Basket.BasketSummary.BasketErrorCode.Trim)
                    Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(talentErrorMsg.ERROR_MESSAGE)
                    If listItemObject Is Nothing Then blErrorMessages.Items.Add(talentErrorMsg.ERROR_MESSAGE)
                    hasError = True
                End If
            Catch ex As Exception
                'empty exception to catch threadabort
            End Try
        End If
        Return hasError
    End Function

    ''' <summary>
    ''' Validate the page fields
    ''' </summary>
    ''' <returns>True if the page is valid</returns>
    ''' <remarks></remarks>
    Private Function isPageValid() As Boolean
        Dim validPageToProcess As Boolean = True
        If validPageToProcess AndAlso (Not Page.IsValid) Then
            validPageToProcess = False
        End If
        If validPageToProcess AndAlso Profile.Basket.BasketSummary.BasketErrorCode.Trim.Length > 0 Then
            validPageToProcess = False
            Dim talentErrorMsg As TalentErrorMessage = _errMsg.GetErrorMessage(Profile.Basket.BasketSummary.BasketErrorCode.Trim)
            Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(talentErrorMsg.ERROR_MESSAGE)
            If listItemObject Is Nothing Then blErrorMessages.Items.Add(talentErrorMsg.ERROR_MESSAGE)
        End If
        Return validPageToProcess
    End Function

    ''' <summary>
    ''' Is the delivery address valid
    ''' Only allowed to continue if Delivery Address has been validated (as Delivery Address accordion option may be reopened, amended, and closed again without revalidation occuring)
    ''' </summary>
    ''' <returns>True if the delivery address is valid</returns>
    ''' <remarks></remarks>
    Private Function IsDeliveryAddressValid(Optional ByVal booleanShowErrorMessage As Boolean = True) As Boolean
        Dim ret As Boolean = True
        If plhDeliveryAddress.Visible AndAlso uscDeliveryAddress IsNot Nothing AndAlso Not uscDeliveryAddress.DeliveryAddressValidated Then

            ' Hide all subsequenent panels
            If plhSelectPaymentMethod.Visible Then plhSelectPaymentMethod.Visible = False
            setCCControlsByPaymentGateway("IsDeliveryAddressValid")
            If plhPaymentMethodPayPal.Visible Then plhPaymentMethodPayPal.Visible = False
            If plhPaymentMethodGoogleCheckout.Visible Then plhPaymentMethodGoogleCheckout.Visible = False
            If plhPaymentMethodEPurse.Visible Then plhPaymentMethodEPurse.Visible = False
            If plhPaymentMethodCash.Visible Then plhPaymentMethodCash.Visible = False
            If plhPaymentMethodInvoice.Visible Then plhPaymentMethodInvoice.Visible = False
            If plhPaymentMethodOnAccount.Visible Then plhPaymentMethodOnAccount.Visible = False
            If plhPaymentMethodChipAndPin.Visible Then plhPaymentMethodChipAndPin.Visible = False
            If booleanShowErrorMessage Then

                ' Move back to Delivery Address panel
                _nextItemToOpen = 0
                registerAcordionMenuScript(_nextItemToOpen, False)

                ' Display error message
                blErrorMessages.Items.Add(_wfrPage.Content("DeliveryAddressNotValidatedText", _languageCode, True))
                _currentPaymentMethod = String.Empty
            End If
            ret = False
        End If
        Return ret
    End Function

    ''' <summary>
    ''' Check to see if the layout needs to be rebound
    ''' </summary>
    ''' <returns>True if the layout function needs to occur</returns>
    ''' <remarks></remarks>
    Private Function canDoLayoutFunction() As Boolean
        Dim doLayout As Boolean = False
        Dim canResetExternalCCVisibility As Boolean = False
        If uscBasketFees.BasketFeesUpdated Then
            doLayout = True
            canResetExternalCCVisibility = True
        ElseIf uscApplyCashback.CashbackChanged Then
            doLayout = True
        ElseIf uscEPurse.EPurseOptionsChanged Then
            doLayout = True
        ElseIf uscBasketSummary.PartPaymentOptionsChanged Then
            doLayout = True
        ElseIf uscPaymentDetails.InstallmentsPostBack Then
            doLayout = True
        ElseIf uscPaymentDetails.CardTypeSelectionChanged Then
            doLayout = True
        ElseIf uscBasketSummary.BasketSummaryUpdated Then
            doLayout = True
            canResetExternalCCVisibility = True
        ElseIf uscMySavedCards.SavedCardSelectionChanged Then
            doLayout = True
        ElseIf uscPaymentCardType.CardTypeSelectionChanged Then
            doLayout = True
            canResetExternalCCVisibility = True
        End If
        If canResetExternalCCVisibility Then
            If Session("plhCCExternal") IsNot Nothing Then
                plhCCExternal.Visible = False
                Session("plhCCExternal") = Nothing
            End If
        End If
        Return doLayout
    End Function

    ''' <summary>
    ''' Determine if the basket total is negative
    ''' </summary>
    ''' <returns>True if the basket is negative</returns>
    ''' <remarks></remarks>
    Private Function isBasketTotalNegative() As Boolean
        If Profile.Basket.BasketSummary.TotalBasket < 0 AndAlso Profile.Basket.BasketSummary.BasketErrorCode.Trim = GlobalConstants.BSKTSMRY_ERR_CODE_NSB Then
            'if sale basket is negative consider it as positive to get the correct payment options
            _isBasketOverAllTotalNegative = Not (_basketOverAllTotal < 0)
        Else
            _basketOverAllTotal = Profile.Basket.BasketSummary.TotalBasket
            _isBasketOverAllTotalNegative = _basketOverAllTotal < 0
        End If
        Return _isBasketOverAllTotalNegative
    End Function

    ''' <summary>
    ''' Review the basket and determine if the PPS payment methods need to show
    ''' </summary>
    ''' <returns>A boolean to indicate whether we are using PPS</returns>
    ''' <remarks></remarks>
    Private Function isPPSPaymentOptionsInUse() As Boolean
        Dim ppsItemsInBasket As Boolean = False
        Dim basketPPS1CodeList As New Collection
        Dim basketPPS1DescriptionList As New Collection
        Dim basketPPS1SeatDescriptionList As New Collection
        Dim basketPPS2CodeList As New Collection
        Dim basketPPS2DescriptionList As New Collection
        Dim basketPPS2SeatDescriptionList As New Collection
        Dim seasonTicketDescription As String = String.Empty
        Dim seasonTicketProductCode As String = String.Empty

        blProductList.Items.Clear()
        plhPPSPaymentOptionsMessage.Visible = False
        If Profile.Basket.BasketContentType = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
            For Each basketItem As TalentBasketItem In Profile.Basket.BasketItems
                If basketItem.MODULE_ = GlobalConstants.BASKETMODULETICKETING AndAlso basketItem.PRODUCT_TYPE = GlobalConstants.PPSPRODUCTTYPE AndAlso basketItem.PRODUCT_SUB_TYPE = GlobalConstants.PPSTYPE1 Then
                    basketPPS1DescriptionList.Add(basketItem.PRODUCT_DESCRIPTION1.Trim)
                    basketPPS1SeatDescriptionList.Add(basketItem.SEAT.Trim)
                    basketPPS1CodeList.Add(basketItem.Product)
                ElseIf basketItem.MODULE_ = GlobalConstants.BASKETMODULETICKETING AndAlso basketItem.PRODUCT_TYPE = GlobalConstants.PPSPRODUCTTYPE AndAlso basketItem.PRODUCT_SUB_TYPE = GlobalConstants.PPSTYPE2 Then
                    basketPPS2DescriptionList.Add(basketItem.PRODUCT_DESCRIPTION1.Trim)
                    basketPPS2SeatDescriptionList.Add(basketItem.SEAT.Trim)
                    basketPPS2CodeList.Add(basketItem.Product)
                ElseIf basketItem.MODULE_ = GlobalConstants.BASKETMODULETICKETING AndAlso basketItem.PRODUCT_TYPE = GlobalConstants.SEASONTICKETPRODUCTTYPE Then
                    seasonTicketDescription = basketItem.PRODUCT_DESCRIPTION1.Trim
                    seasonTicketProductCode = basketItem.Product
                End If
            Next
        End If

        If basketPPS1CodeList.Count > 0 AndAlso Session("PPS1PaymentComplete") Is Nothing Then
            hdfProductForDD.Value = basketPPS1CodeList(1)
            ppsItemsInBasket = True
            plhPPSPaymentOptionsMessage.Visible = True
            If basketPPS1CodeList.Count = 1 Then
                blProductList.Visible = False
                ltlPPSPaymentOptionsMessage.Text = _wfrPage.Content("PPSPaymentOptionsMessage", _languageCode, True).Replace("<<ProductDescription>>", basketPPS1DescriptionList(1))
                ltlPPSPaymentOptionsMessage.Text = ltlPPSPaymentOptionsMessage.Text.Replace("<<SeatDetails>>", basketPPS1SeatDescriptionList(1))
            Else
                blProductList.Visible = True
                ltlPPSPaymentOptionsMessage.Text = _wfrPage.Content("MultiplePPSPaymentOptionsMessage1", _languageCode, True)
                Dim i As Integer = 1
                For Each item As String In basketPPS1DescriptionList
                    Dim paymentOptionsDescription As String = _wfrPage.Content("MultiplePPSPaymentOptionsMessage2", _languageCode, True)
                    paymentOptionsDescription = paymentOptionsDescription.Replace("<<ProductDescription>>", item)
                    paymentOptionsDescription = paymentOptionsDescription.Replace("<<SeatDetails>>", basketPPS1SeatDescriptionList(i))
                    blProductList.Items.Add(paymentOptionsDescription)
                    i += 1
                Next
            End If
            Session("basketPPS1List") = basketPPS1CodeList
        Else
            Session("PPS1PaymentComplete") = True
        End If

        If basketPPS2CodeList.Count > 0 Then
            If Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") Is Nothing Then
                hdfProductForDD.Value = basketPPS2CodeList(1)
                ppsItemsInBasket = True
                plhPPSPaymentOptionsMessage.Visible = True
                If basketPPS1CodeList.Count = 1 Then
                    blProductList.Visible = False
                    ltlPPSPaymentOptionsMessage.Text = _wfrPage.Content("PPSPaymentOptionsMessage", _languageCode, True).Replace("<<ProductDescription>>", basketPPS2DescriptionList(1))
                    ltlPPSPaymentOptionsMessage.Text = ltlPPSPaymentOptionsMessage.Text.Replace("<<SeatDetails>>", basketPPS2SeatDescriptionList(1))
                Else
                    blProductList.Visible = True
                    ltlPPSPaymentOptionsMessage.Text = _wfrPage.Content("MultiplePPSPaymentOptionsMessage1", _languageCode, True)
                    Dim i As Integer = 1
                    For Each item As String In basketPPS2DescriptionList
                        Dim paymentOptionsDescription As String = _wfrPage.Content("MultiplePPSPaymentOptionsMessage2", _languageCode, True)
                        paymentOptionsDescription = paymentOptionsDescription.Replace("<<ProductDescription>>", item)
                        paymentOptionsDescription = paymentOptionsDescription.Replace("<<SeatDetails>>", basketPPS2SeatDescriptionList(i))
                        blProductList.Items.Add(paymentOptionsDescription)
                        i += 1
                    Next
                End If
            End If
            Session("basketPPS2List") = basketPPS2CodeList
        Else
            Session("PPS2PaymentComplete") = True
        End If

        If seasonTicketDescription.Length > 0 Then
            If Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") = True Then
                plhPPSPaymentOptionsMessage.Visible = True
                blProductList.Visible = False
                ltlPPSPaymentOptionsMessage.Text = _wfrPage.Content("SeasonTicketPaymentOptionsMessage", _languageCode, True).Replace("<<ProductDescription>>", seasonTicketDescription)
            End If
            hdfProductForDD.Value = seasonTicketProductCode
        End If

        setupPPSItemsPaidFor(basketPPS1DescriptionList, basketPPS1SeatDescriptionList, basketPPS2DescriptionList, basketPPS2SeatDescriptionList, seasonTicketDescription)

        Return ppsItemsInBasket
    End Function

    ''' <summary>
    ''' Determine if we are paying for PPS at this point
    ''' </summary>
    ''' <returns>True or false depending on whether we are paying for PPS</returns>
    ''' <remarks></remarks>
    Private Function isPayingForPPS() As Boolean
        'During PPS payments we are just saving the payment details in session. This code checks to sess if we have provided payment details
        'for each stage. If the PPS have been paid for the final stage is available for completion to TALENT.
        'Because the browser reloads the page after each stage we need to perform this check and set "TakentFinalPayment" so we know the user
        'has selected a payment option first before completing. This prevents any "browser back" actions being performed and the payment
        'details from one of the PPS being used to complete with.

        Dim payingForPPS As Boolean = False
        Dim checkoutStage As String = GlobalConstants.CHECKOUTASPXSTAGE
        If Session("basketPPS1List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") Is Nothing Then
            checkoutStage = GlobalConstants.PPS1STAGE
            Session("TakeFinalPayment") = False
            payingForPPS = True
        Else
            If Session("basketPPS2List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") Is Nothing Then
                checkoutStage = GlobalConstants.PPS2STAGE
                Session("TakeFinalPayment") = False
                payingForPPS = True
            End If
        End If
        If checkoutStage = GlobalConstants.CHECKOUTASPXSTAGE Then
            payingForPPS = False
        End If
        Return payingForPPS
    End Function

    ''' <summary>
    ''' Determine if the Marketing Campaigns are in use. Setup option if in use.
    ''' </summary>
    ''' <returns>True if marketing campaigns are in use</returns>
    ''' <remarks></remarks>
    Private Function isMarketingCampaignsActive() As Boolean
        Dim marketingCampaignsActive As Boolean = False
        If Profile.Basket.CAT_MODE <> GlobalConstants.CATMODE_CANCEL AndAlso ModuleDefaults.MARKETING_CAMPAIGNS_ACTIVE AndAlso Profile.Basket.MARKETING_CAMPAIGN AndAlso uscMarketingCampaigns.Display Then
            marketingCampaignsActive = True
            ltlMarketingCampaignTitle.Text = _wfrPage.Content("MarketingCampaignTitle", _languageCode, True)
            If ltlMarketingCampaignTitle.Text.Length = 0 Then plhMarketingCampaignTitle.Visible = False
            ltlMarketingCampaignSubTitle.Text = _wfrPage.Content("MarketingCampaignSubTitle", _languageCode, True)
            If ltlMarketingCampaignSubTitle.Text.Length = 0 Then plhMarketingCampaignSubTitle.Visible = False
        End If
        Return marketingCampaignsActive
    End Function

    ''' <summary>
    ''' Determine if the Delivery Address capture form should be displayed. Setup option if required or popultae default details if Agent + PrintNow.
    ''' </summary>
    ''' <returns>True if Delivery Address form in use</returns>
    ''' <remarks></remarks>
    Private Function isDeliveryAddressActive() As Boolean
        Dim deliveryAddressActive As Boolean = False

        Select Case Profile.Basket.BasketContentType
            Case GlobalConstants.COMBINEDBASKETCONTENTTYPE
                deliveryAddressActive = True
            Case GlobalConstants.MERCHANDISEBASKETCONTENTTYPE
                deliveryAddressActive = True
            Case GlobalConstants.TICKETINGBASKETCONTENTTYPE
                If ModuleDefaults.AllowPublicTicketingDeliveryAddressSelection And containsPostOrRegisteredPost() Then
                    deliveryAddressActive = True
                End If
        End Select

        'If AgentProfile.IsAgent Then
        '    If Profile.IsAnonymous Then
        '        deliveryAddressActive = False
        '    End If
        'End If

        If deliveryAddressActive Then
            btnConfirmDeliveryAddress.Text = _wfrPage.Content("DeliveryAddressContinueButton", _languageCode, True)
            ltlDeliveryAddressSubTitle.Text = _wfrPage.Content("DeliveryAddressSubTitleText", _languageCode, True)
            plhDeliveryAddressSubTitle.Visible = (ltlDeliveryAddressSubTitle.Text.Length > 0)
            ltlDeliveryAddressTitle.Text = _wfrPage.Content("DeliveryAddressTitleText", _languageCode, True)
            plhDeliveryAddressTitle.Visible = (ltlDeliveryAddressTitle.Text.Length > 0)
            deliveryAddressActive = True
        Else

            ' Set Delivery Address as having been validated
            uscDeliveryAddress.DeliveryAddressValidated = True

            ' If not displaying the delivery address form but are an Agent and have tickets that are PrintNow then
            ' then always ensure that printing details are passed to the back-end via Session("DeliveryDetails") 
            If AgentProfile.IsAgent Then
                If containsPrint() Then
                    Dim deliveryDetails As New DEDeliveryDetails
                    deliveryDetails.PrintOption = SetPrintOption(AgentProfile.PrintAddressLabelDefault, AgentProfile.PrintTransactionReceiptDefault)
                    If (Not Profile.IsAnonymous) AndAlso (Profile.User IsNot Nothing) AndAlso (Profile.User.Details IsNot Nothing) Then
                        If deliveryDetails.PrintOption.Trim = "2" Or deliveryDetails.PrintOption.Trim = "4" Then
                            Try
                                deliveryDetails.ContactName = Profile.User.Details.Full_Name
                                Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
                                Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
                                If Profile.User.Details.Title.ToString.Trim <> "" AndAlso _
                                    Not CType(Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(def.ProfileFullNameWithTitle), Boolean) Then
                                    deliveryDetails.ContactName = Profile.User.Details.Title.ToString.Trim + " " + deliveryDetails.ContactName.ToString.Trim
                                End If
                                Dim enumer As Collections.Generic.Dictionary(Of String, TalentProfileAddress).KeyCollection.Enumerator
                                enumer = Profile.User.Addresses.Keys.GetEnumerator
                                Dim userAddress As TalentProfileAddress
                                While enumer.MoveNext
                                    userAddress = Profile.User.Addresses(enumer.Current)
                                    If String.IsNullOrEmpty(userAddress.Address_Line_1.Trim) Then
                                        deliveryDetails.Address1 = userAddress.Address_Line_2
                                        deliveryDetails.Address2 = userAddress.Address_Line_3
                                        deliveryDetails.Address3 = userAddress.Address_Line_4
                                        deliveryDetails.Address4 = userAddress.Address_Line_5
                                    Else
                                        deliveryDetails.Address1 = userAddress.Address_Line_1
                                        If userAddress.Address_Line_2.Trim = "" Then
                                            deliveryDetails.Address2 = userAddress.Address_Line_3
                                        Else
                                            If userAddress.Address_Line_3.Trim = "" Then
                                                deliveryDetails.Address2 = userAddress.Address_Line_2
                                            Else
                                                deliveryDetails.Address2 = userAddress.Address_Line_2 + ", " + userAddress.Address_Line_3
                                            End If
                                        End If
                                        deliveryDetails.Address3 = userAddress.Address_Line_4
                                        deliveryDetails.Address4 = userAddress.Address_Line_5
                                    End If
                                    deliveryDetails.Country = userAddress.Country
                                    deliveryDetails.Postcode = userAddress.Post_Code
                                    Exit While
                                End While
                            Catch ex As Exception
                                ' no address
                            End Try

                        End If
                    End If
                    Session("DeliveryDetails") = deliveryDetails
                End If
            End If
        End If
        Return deliveryAddressActive
    End Function

    ''' <summary>
    ''' Determine if the credit card option is in use. Setup option if in use.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isCreditCardOptionActive() As Boolean
        Dim creditCardOptionActive As Boolean = False
        If _hideTAndCCheckbox Then plhCreditCardTAndC.Visible = False
        If _ccTypeEnabled Then
            If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                _ccTypeEnabled = True
            Else
                _ccTypeEnabled = Profile.Basket.PAYMENT_OPTIONS.Contains(GlobalConstants.CCPAYMENTTYPE)
            End If
            If _ccTypeEnabled Then
                creditCardOptionActive = True
                lblCreditCard.Text = _wfrPage.Content("CreditCardPaymentMethodLabel", _languageCode, True)
                Dim isVanguard As Boolean = True
                'isVanguard = (ModuleDefaults.PaymentGatewayType.ToUpper() = "VANGUARD")
                If isVanguard Then
                    ltlCCTypeTitle.Text = _wfrPage.Content("CCTypeTitle", _languageCode, True)
                    ltlCCTypeSubTitle.Text = _wfrPage.Content("CCTypeSubTitle", _languageCode, True)
                    ltlCCTypeMessage.Text = _wfrPage.Content("CCTypeMessasge", _languageCode, True)
                    btnConfirmCCType.Text = _wfrPage.Content("CCTypeButtonText", _languageCode, True)
                    ltlCCExternalTitle.Text = _wfrPage.Content("CCExternalTitle", _languageCode, True)
                    ltlCCExternalSubTitle.Text = _wfrPage.Content("CCExternalSubTitle", _languageCode, True)
                    btnConfirmCCExternalPayment.Text = _wfrPage.Content("CCExternalButtonText", _languageCode, True)
                Else
                    btnConfirmCCPayment.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnConfirmCCPayment))
                End If
                btnConfirmCCPayment.Text = _wfrPage.Content("ConfirmSaleButtonText", _languageCode, True)
                ltlCreditCardTitle.Text = _wfrPage.Content("CreditCardTitle", _languageCode, True)
                If ltlCreditCardTitle.Text.Length = 0 Then plhCreditCardTitle.Visible = False
                ltlCreditCardSubTitle.Text = _wfrPage.Content("CreditCardSubTitle", _languageCode, True)
                If ltlCreditCardSubTitle.Text.Length = 0 Then plhCreditCardSubTitle.Visible = False
                csvCardTAndC.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
                lblCardTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
                ltlCreditCardMessage.Text = _wfrPage.Content("CreditCardMessage", _languageCode, True)
                If ltlCreditCardMessage.Text.Length = 0 Then plhCreditCardMessage.Visible = False
            End If
        End If
        If creditCardOptionActive AndAlso AgentProfile.IsAgent Then
            plhCustomerPresentCC.Visible = True
            lblCustomerPresentCC.Text = _wfrPage.Content("CustomerPresentText", _languageCode, True)
        End If
        Return creditCardOptionActive
    End Function

    ''' <summary>
    ''' Determine if the saved credit card option is in use. Setup option if in use.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isSavedCardOptionActive() As Boolean
        Dim savedCardOptionActive As Boolean = False
        If _hideTAndCCheckbox Then plhSavedCardsTAndC.Visible = False
        If _ccTypeEnabled AndAlso BasketContentTypeOverride() <> GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
            If ModuleDefaults.UseSaveMyCard Then
                If Profile.Basket.PAYMENT_OPTIONS.Contains(GlobalConstants.CCPAYMENTTYPE) OrElse
                    (Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE AndAlso _TicketingCheckout) Then
                    uscMySavedCards.RetrieveMySavedCards()
                    If uscMySavedCards.UserHasSavedCards Then
                        savedCardOptionActive = True
                        lblSavedCard.Text = _wfrPage.Content("SavedCardPaymentMethodLabel", _languageCode, True)
                        ltlSavedCardTitle.Text = _wfrPage.Content("SavedCardTitle", _languageCode, True)
                        If ltlSavedCardTitle.Text.Length = 0 Then plhSavedCardTitle.Visible = False
                        ltlSavedCardSubTitle.Text = _wfrPage.Content("SavedCardSubTitle", _languageCode, True)
                        If ltlSavedCardSubTitle.Text.Length = 0 Then plhSavedCardSubTitle.Visible = False
                        ltlSavedCardsLegend.Text = _wfrPage.Content("SavedCardLegend", _languageCode, True)
                        csvSavedCardTAndC.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
                        lblSavedCardTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
                        btnConfirmSavedCardPayment.Text = _wfrPage.Content("ConfirmSaleButtonText", _languageCode, True)
                        btnConfirmSavedCardPayment.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnConfirmSavedCardPayment))
                        ' ltlSCPartPmt.Text = _wfrPage.Content("PartPaymentLabel", _languageCode, True)
                    End If
                End If
            End If
        End If
        If savedCardOptionActive AndAlso AgentProfile.IsAgent Then
            plhCustomerPresentSC.Visible = (Not (ModuleDefaults.PaymentGatewayType = GlobalConstants.PAYMENTGATEWAY_VANGUARD))
            lblCustomerPresentSC.Text = _wfrPage.Content("CustomerPresentText", _languageCode, True)
        End If
        Return savedCardOptionActive
    End Function

    ''' <summary>
    ''' Determine if the paypal option is in use. Setup option if in use.
    ''' If the basket is being cancelled then the paypal option can only be used providing there is a transaction ID to cancel
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isPayPalOptionActive() As Boolean
        Dim payPalOptionActive As Boolean = False
        If _hideTAndCCheckbox Then plhPayPalTAndC.Visible = False
        If _ppTypeEnabled Then
            If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                payPalOptionActive = True
            Else
                payPalOptionActive = Profile.Basket.PAYMENT_OPTIONS.Contains(GlobalConstants.PAYPALPAYMENTTYPE)
            End If
            If CATHelper.IsBasketNotInCancelMode() = False Then
                If Profile.Basket.EXTERNAL_PAYMENT_TOKEN.Length > 0 Then
                    payPalOptionActive = True
                Else
                    payPalOptionActive = False
                End If
            End If
            If payPalOptionActive Then
                payPalOptionActive = True
                imgBtnConfirmPayPalPayment.ImageUrl = TEBUtilities.GetExternalGatewayImgURL(GlobalConstants.PAYPALPAYMENTTYPE)
                lblPayPal.Text = _wfrPage.Content("PayPalPaymentMethodLabel", _languageCode, True)
                ltlPayPalTitle.Text = _wfrPage.Content("PayPalTitle", _languageCode, True)
                If ltlPayPalTitle.Text.Length = 0 Then plhPayPalTitle.Visible = False
                ltlPayPalSubTitle.Text = _wfrPage.Content("PayPalSubTitle", _languageCode, True)
                If ltlPayPalSubTitle.Text.Length = 0 Then plhPayPalSubTitle.Visible = False
                ltlPayPalMessage.Text = _wfrPage.Content("PayPalMessage", _languageCode, True)
                If ltlPayPalMessage.Text.Length = 0 Then plhPayPalMessage.Visible = False
                ltlPayPalLegend.Text = _wfrPage.Content("PayPalLegend", _languageCode, True)
                lblPayPalTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
                csvPayPalTAndC.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
                hdfPayPalAccountID.Value = ModuleDefaults.PaymentPayPalAccountID
                hdfPayPalEnvironment.Value = ModuleDefaults.PaymentPayPalEnvironment
                ltlPayPalObjectFile.Text = "<script src=""//www.paypalobjects.com/api/checkout.js"" async></script>"
                ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "checkout-paypal.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("checkout-paypal.js", "/Module/Checkout/"), False)
            End If
        End If
        Return payPalOptionActive
    End Function

    ''' <summary>
    ''' Determine if the google checkout option is in use. Setup option if in use.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isGoogleCheckoutOptionActive() As Boolean
        Dim googleCheckoutOptionActive As Boolean = False
        If _hideTAndCCheckbox Then plhGoogleCheckoutTAndC.Visible = False
        If _gcTypeEnabled Then
            If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
                googleCheckoutOptionActive = True
            Else
                googleCheckoutOptionActive = Profile.Basket.PAYMENT_OPTIONS.Contains(GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE)
            End If
            If googleCheckoutOptionActive Then
                googleCheckoutOptionActive = True
                imgBtnConfirmGoogleCheckoutPayment.ImageUrl = TEBUtilities.GetExternalGatewayImgURL(GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE)
                lblGoogleCheckout.Text = _wfrPage.Content("GoogleCheckoutPaymentMethodLabel", _languageCode, True)
                ltlGoogleCheckoutTitle.Text = _wfrPage.Content("GoogleCheckoutTitle", _languageCode, True)
                If ltlGoogleCheckoutTitle.Text.Length = 0 Then plhGoogleCheckoutTitle.Visible = False
                ltlGoogleCheckoutSubTitle.Text = _wfrPage.Content("GoogleCheckoutSubTitle", _languageCode, True)
                If ltlGoogleCheckoutSubTitle.Text.Length = 0 Then plhGoogleCheckoutSubTitle.Visible = False
                ltlGoogleCheckoutMessage.Text = _wfrPage.Content("GoogleCheckoutMessage", _languageCode, True)
                If ltlGoogleCheckoutMessage.Text.Length > 0 Then
                    ltlGoogleCheckoutMessage.Text = ltlGoogleCheckoutMessage.Text.Replace("<<<TEMP_ORDER_ID>>>", Profile.Basket.TempOrderID)
                End If
                If ltlGoogleCheckoutMessage.Text.Length = 0 Then plhGoogleCheckoutMessage.Visible = False
                ltlGoogleCheckoutLegend.Text = _wfrPage.Content("GoogleCheckoutLegend", _languageCode, True)
                lblGoogleCheckoutTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
                csvGoogleCheckoutTAndC.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
            End If
        End If
        Return googleCheckoutOptionActive
    End Function

    ''' <summary>
    ''' Determine if the direct debit option is in use. Setup option if in use.
    ''' If paying for PPS don't check the basket header payment options field and instead check again tbl_payment_type_bu for PPS_TYPE = true
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <param name="ppsOption">Specify whether the option is for PPS payments or Season Ticket payments as a boolean</param>
    ''' <remarks></remarks>
    Private Function isDirectDebitOptionActive(ByVal ppsOption As Boolean) As Boolean
        Dim directDebitOptionActive As Boolean = False
        If _hideTAndCCheckbox Then plhDirectDebitTAndC.Visible = False
        If ppsOption Then
            If _ddTypeEnabled AndAlso Profile.Basket.BasketContentType = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
                directDebitOptionActive = False
                For Each row As DataRow In _dtPaymentTypes.Rows
                    If row("PAYMENT_TYPE_CODE") = GlobalConstants.DDPAYMENTTYPE Then
                        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(row("PPS_TYPE")) Then directDebitOptionActive = True
                        Exit For
                    End If
                Next
            End If
        Else
            If _ddTypeEnabled AndAlso Profile.Basket.BasketContentType = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
                directDebitOptionActive = Profile.Basket.PAYMENT_OPTIONS.Contains(GlobalConstants.DDPAYMENTTYPE)
            End If
        End If

        If directDebitOptionActive Then
            If ModuleDefaults.AllowMultipleProductsInDDCheckout Then
                directDebitOptionActive = True
            Else
                Dim numberOfSeasonTickets As Integer = 0
                For Each item As TalentBasketItem In Profile.Basket.BasketItems
                    If item.PRODUCT_TYPE = GlobalConstants.SEASONTICKETPRODUCTTYPE Then numberOfSeasonTickets += 1
                Next
                If numberOfSeasonTickets > 1 Then directDebitOptionActive = False
            End If
            If directDebitOptionActive Then
                lblDirectDebit.Text = _wfrPage.Content("DirectDebitPaymentMethodLabel", _languageCode, True)
                ltlDirectDebitTitle.Text = _wfrPage.Content("DirectDebitTitle", _languageCode, True)
                If ltlDirectDebitTitle.Text.Length = 0 Then plhDirectDebitTitle.Visible = False
                ltlDirectDebitSubTitle.Text = _wfrPage.Content("DirectDebitSubTitle", _languageCode, True)
                If ltlDirectDebitSubTitle.Text.Length = 0 Then plhDirectDebitSubTitle.Visible = False
                ltlDirectDebitH2.Text = _wfrPage.Content("DirectDebitH2", _languageCode, True)
                If ltlDirectDebitH2.Text.Length = 0 Then plhDirectDebitH2.Visible = False
                ltlDirectDebitMessage.Text = _wfrPage.Content("DirectDebitMessage", _languageCode, True)
                If ltlDirectDebitMessage.Text.Length = 0 Then plhDirectDebitMessage.Visible = False
                ltlDirectDebitLegend.Text = _wfrPage.Content("DirectDebitLegend", _languageCode, True)
                lblDDTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
                csvDDTAndC.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
                btnConfirmDDPayment.Text = _wfrPage.Content("ConfirmSaleButtonText", _languageCode, True)
                btnConfirmDDMandate.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnConfirmDDMandate))
            End If
        End If
        If Not directDebitOptionActive Then rdoDirectDebit.Checked = False 'Do this incase DD is available for PPS, but not Season tickets
        Return directDebitOptionActive
    End Function

    ''' <summary>
    ''' Determine if the cashback option is in use. Setup option if in use.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isCashbackOptionActive() As Boolean
        Dim cashbackOptionIsActive As Boolean = False
        If _hideTAndCCheckbox Then plhCashbackTAndC.Visible = False
        If (Not Profile.IsAnonymous) AndAlso BasketContentTypeOverride() = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
            Dim dtAdhocFees As New DataTable
            dtAdhocFees = TDataObjects.PaymentSettings.TblAdhocFees.GetByBUPartnerLang(_wfrPage.BusinessUnit, _wfrPage.PartnerCode, _languageCode)
            For Each row As DataRow In dtAdhocFees.Rows
                If row("FEE_CODE").ToString() = ModuleDefaults.CashBackFeeCode Then
                    cashbackOptionIsActive = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(row("FEE_IN_USE"))
                    Exit For
                End If
            Next

            If cashbackOptionIsActive Then
                Dim cashbackItemInBasket As Boolean = False
                For Each item As TalentBasketItem In Profile.Basket.BasketItems
                    If item.Product = ModuleDefaults.CashBackFeeCode Then
                        cashbackItemInBasket = True
                        ' davetodo - need to review this after F1 - do we need to stop if a Corporate hospitality in the basket ??  
                        ' ElseIf item.PRODUCT_TYPE = GlobalConstants.CORPORATEPRODUCTTYPE Then 'Disable option if a corporate item is in the basket
                        '    cashbackOptionIsActive = False
                        Exit For
                    End If
                Next
                If Not cashbackItemInBasket AndAlso cashbackOptionIsActive Then
                    If uscApplyCashback.RetrieveCashback > 0 Then
                        cashbackOptionIsActive = True
                    Else
                        cashbackOptionIsActive = False
                    End If
                End If
                If cashbackOptionIsActive Then
                    lblCashback.Text = _wfrPage.Content("CashbackPaymentMethodLabel", _languageCode, True)
                    ltlCashbackTitle.Text = _wfrPage.Content("CashbackTitle", _languageCode, True)
                    If ltlCashbackTitle.Text.Length = 0 Then plhCashbackTitle.Visible = False
                    ltlCashbackSubTitle.Text = _wfrPage.Content("CashbackSubTitle", _languageCode, True)
                    If ltlCashbackSubTitle.Text.Length = 0 Then plhCashbackSubTitle.Visible = False
                    ltlCashbackMessage.Text = _wfrPage.Content("CashbackMessage", _languageCode, True)
                    If ltlCashbackMessage.Text.Length = 0 Then plhCashbackMessage.Visible = False
                    ltlCashbackLegend.Text = _wfrPage.Content("CashbackLegend", _languageCode, True)
                    lblCashbackTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
                    csvCashbackTAndC.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
                End If
            End If
        End If
        Return cashbackOptionIsActive
    End Function

    ''' <summary>
    ''' Determine if the EPurse option is in use. Setup option if in use.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isEPurseOptionActive() As Boolean
        Dim ePurseOptionActive As Boolean = False
        If _hideTAndCCheckbox Then
            uscEPurse.ShowTAndCs = False
        Else
            uscEPurse.ShowTAndCs = True
        End If
        If _GiftCardEnabled Then
            Return isGiftCardActive()
        Else
            If _epTypeEnabled AndAlso BasketContentTypeOverride() = GlobalConstants.TICKETINGBASKETCONTENTTYPE AndAlso Not Profile.IsAnonymous Then
                _epTypeEnabled = Profile.Basket.PAYMENT_OPTIONS.Contains(GlobalConstants.EPURSEPAYMENTTYPE)
                If _epTypeEnabled Then
                    Dim err As New ErrorObj
                    Dim ts As New TalentSmartcard
                    Dim deSmartcard As New DESmartcard
                    Dim ucr As New UserControlResource
                    ucr.BusinessUnit = _wfrPage.BusinessUnit
                    ucr.PageCode = _wfrPage.PageCode
                    ucr.FrontEndConnectionString = _wfrPage.FrontEndConnectionString
                    ucr.KeyCode = "EPurse.ascx"

                    deSmartcard.CustomerNumber = Profile.User.Details.LoginID
                    ts.DE = deSmartcard
                    ts.Settings = TEBUtilities.GetSettingsObject()
                    ts.Settings.Cacheing = ucr.Attribute("Cacheing")
                    ts.Settings.CacheTimeMinutes = ucr.Attribute("CacheTimeMinutes")
                    err = ts.RetrieveSmartcardCards
                    If Not err.HasError AndAlso ts.ResultDataSet.Tables.Count = 2 AndAlso ts.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then
                        If ts.ResultDataSet.Tables("Smartcards").Rows.Count > 0 Then
                            ePurseOptionActive = True
                        End If
                    End If
                    If ePurseOptionActive Then
                        lblEPurse.Text = _wfrPage.Content("EPursePaymentMethodLabel", _languageCode, True)
                        ltlEPurseTitle.Text = _wfrPage.Content("EPurseTitle", _languageCode, True)
                        If ltlEPurseTitle.Text.Length = 0 Then plhEPurseTitle.Visible = False
                        ltlEPurseSubTitle.Text = _wfrPage.Content("EPurseSubTitle", _languageCode, True)
                        If ltlEPurseSubTitle.Text.Length = 0 Then plhEPurseSubTitle.Visible = False
                        ltlEPurseH2.Text = _wfrPage.Content("EPurseH2", _languageCode, True)
                        If ltlEPurseH2.Text.Length = 0 Then plhEPurseH2.Visible = False
                        ltlEPurseMessage.Text = _wfrPage.Content("EPurseMessage", _languageCode, True)
                        If ltlEPurseMessage.Text.Length = 0 Then plhEPurseMessage.Visible = False
                        ltlEPurseLegend.Text = _wfrPage.Content("EPurseLegend", _languageCode, True)
                        lblEPurseTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
                        csvEPurseTAndC.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
                        btnConfirmEPursePayment.Text = _wfrPage.Content("ConfirmSaleButtonText", _languageCode, True)
                        btnConfirmEPursePayment.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnConfirmEPursePayment))
                        lblEPPartPmt.Text = _wfrPage.Content("PartPaymentLabel", _languageCode, True)
                        uscEPurse.Display = True
                    End If
                End If
            End If
        End If
        Return ePurseOptionActive
    End Function

    ''' <summary>
    ''' Determine if the EPurse option is to be used for giftcards. Setup option if in use.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isGiftCardActive() As Boolean
        Dim GiftCardActive As Boolean = False
        If _GiftCardEnabled AndAlso BasketContentTypeOverride() = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
            Dim err As New ErrorObj
            Dim ts As New TalentSmartcard
            Dim deSmartcard As New DESmartcard
            Dim ucr As New UserControlResource
            ucr.BusinessUnit = _wfrPage.BusinessUnit
            ucr.PageCode = _wfrPage.PageCode
            ucr.FrontEndConnectionString = _wfrPage.FrontEndConnectionString
            ucr.KeyCode = "EPurse.ascx"
            GiftCardActive = True
            uscEPurse.IsGiftCard = True
            If GiftCardActive Then
                lblEPurse.Text = _wfrPage.Content("EPursePaymentMethodLabel", _languageCode, True)
                ltlEPurseTitle.Text = _wfrPage.Content("EPurseTitle", _languageCode, True)
                If ltlEPurseTitle.Text.Length = 0 Then plhEPurseTitle.Visible = False
                ltlEPurseSubTitle.Text = _wfrPage.Content("EPurseSubTitle", _languageCode, True)
                If ltlEPurseSubTitle.Text.Length = 0 Then plhEPurseSubTitle.Visible = False
                ltlEPurseH2.Text = _wfrPage.Content("EPurseH2", _languageCode, True)
                If ltlEPurseH2.Text.Length = 0 Then plhEPurseH2.Visible = False
                ltlEPurseMessage.Text = _wfrPage.Content("EPurseMessage", _languageCode, True)
                If ltlEPurseMessage.Text.Length = 0 Then plhEPurseMessage.Visible = False
                ltlEPurseLegend.Text = _wfrPage.Content("EPurseLegend", _languageCode, True)
                lblEPurseTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
                csvEPurseTAndC.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
                btnConfirmEPursePayment.Text = _wfrPage.Content("ConfirmSaleButtonText", _languageCode, True)
                btnConfirmEPursePayment.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnConfirmEPursePayment))
                lblEPPartPmt.Text = _wfrPage.Content("PartPaymentLabel", _languageCode, True)
                uscEPurse.Display = True
            End If
        End If
        Return GiftCardActive
    End Function

    ''' <summary>
    ''' Determine if the Credit Finance option is in use. Setup option if in use.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isCreditFinanceOptionActive() As Boolean
        Dim creditFinanceOptionActive As Boolean = False
        If _hideTAndCCheckbox Then plhCFTAndC.Visible = False
        If _cfTypeEnabled AndAlso Profile.Basket.BasketContentType = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
            _cfTypeEnabled = Profile.Basket.PAYMENT_OPTIONS.Contains(GlobalConstants.CFPAYMENTTYPE)
            If _cfTypeEnabled Then
                creditFinanceOptionActive = True
                lblCreditFinance.Text = _wfrPage.Content("CreditFinancePaymentMethodLabel", _languageCode, True)
                ltlCreditFinanceTitle.Text = _wfrPage.Content("CreditFinanceTitle", _languageCode, True)
                If ltlCreditFinanceTitle.Text.Length = 0 Then plhCreditFinanceTitle.Visible = False
                ltlCreditFinanceSubTitle.Text = _wfrPage.Content("CreditFinanceSubTitle", _languageCode, True)
                If ltlCreditFinanceSubTitle.Text.Length = 0 Then plhCreditFinanceSubTitle.Visible = False
                ltlCreditFinanceH2.Text = _wfrPage.Content("CreditFinanceH2", _languageCode, True)
                If ltlCreditFinanceH2.Text.Length = 0 Then plhCreditFinanceH2.Visible = False
                ltlCreditFinanceMessage.Text = _wfrPage.Content("CreditFinanceMessage", _languageCode, True)
                If ltlCreditFinanceMessage.Text.Length = 0 Then plhCreditFinanceMessage.Visible = False
                ltlCreditFinanceLegend.Text = _wfrPage.Content("CreditFinanceLegend", _languageCode, True)
                lblCFTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
                csvCFTAndC.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
                btnConfirmCFPayment.Text = _wfrPage.Content("ConfirmSaleButtonText", _languageCode, True)
                btnConfirmCFMandate.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnConfirmCFMandate))
            End If
        End If
        Return creditFinanceOptionActive
    End Function

    ''' <summary>
    ''' Determine if the Cash payment option is in use. Setup option if in use.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isCashOptionActive() As Boolean
        Dim cashOptionActive As Boolean = False
        If _hideTAndCCheckbox Then plhCashTAndC.Visible = False
        If _csTypeEnabled AndAlso AgentProfile.IsAgent AndAlso BasketContentTypeOverride() = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
            cashOptionActive = True
            lblCash.Text = _wfrPage.Content("CashPaymentMethodLabel", _languageCode, True)
            ltlCashTitle.Text = _wfrPage.Content("CashTitle", _languageCode, True)
            If ltlCashTitle.Text.Length = 0 Then plhCashTitle.Visible = False
            ltlCashSubTitle.Text = _wfrPage.Content("CashSubTitle", _languageCode, True)
            If ltlCashSubTitle.Text.Length = 0 Then plhCashSubTitle.Visible = False
            ltlCashMessage.Text = _wfrPage.Content("CashMessage", _languageCode, True)
            If ltlCashMessage.Text.Length = 0 Then plhCashMessage.Visible = False
            ltlCashLegend.Text = _wfrPage.Content("CashLegend", _languageCode, True)
            lblCashTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
            csvCashTAndC.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
            btnConfirmCashPayment.Text = _wfrPage.Content("ConfirmSaleButtonText", _languageCode, True)
            btnConfirmCashPayment.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnConfirmCashPayment))
        End If
        Return cashOptionActive
    End Function

    ''' <summary>
    ''' Determine if the Other payment types option is in use. Setup option if in use.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isOtherOptionActive() As Boolean
        Dim otherOptionActive As Boolean = False
        If _hideTAndCCheckbox Then plhOthersTAndC.Visible = False
        If _otTypeEnabled AndAlso BasketContentTypeOverride() = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
            otherOptionActive = True
            lblOthers.Text = _wfrPage.Content("OtherPaymentMethodLabel", _languageCode, True)
            ltlOthersTitle.Text = _wfrPage.Content("OthersTitle", _languageCode, True)
            If ltlOthersTitle.Text.Length = 0 Then plhOthersTitle.Visible = False
            ltlOthersSubTitle.Text = _wfrPage.Content("OthersSubTitle", _languageCode, True)
            If ltlOthersSubTitle.Text.Length = 0 Then plhOthersSubtitle.Visible = False
            ltlOthersMessage.Text = _wfrPage.Content("OthersMessage", _languageCode, True)
            If ltlOthersMessage.Text.Length = 0 Then plhOthersMessage.Visible = False
            ltlOthersLegend.Text = _wfrPage.Content("OthersLegend", _languageCode, True)
            lblOthersTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
            lblSelectOthersType.Text = _wfrPage.Content("SelectOthersType", _languageCode, True)
            lblOthersConfigurableValue.Text = _wfrPage.Content("OthersTextValue", _languageCode, True)
            csvOthersTAndC.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
            csvIsOthersTextMandatory.ErrorMessage = _wfrPage.Content("OthersPaymentTypeMandatoryErrorMessage", _languageCode, True)
            btnConfirmOthersPayment.Text = _wfrPage.Content("ConfirmSaleButtonText", _languageCode, True)
            btnConfirmOthersPayment.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnConfirmOthersPayment))
            chkOthersTAndC.Checked = False
        End If
        Return otherOptionActive
    End Function

    ''' <summary>
    ''' Determine if the On Account payment option is in use. Setup option if in use.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isOnAccountOptionActive() As Boolean
        Dim onAccountOptionActive As Boolean = False
        If _hideTAndCCheckbox Then plhOnAccountTAndC.Visible = False
        If _oaTypeEnabled AndAlso BasketContentTypeOverride() = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
            onAccountOptionActive = True
            lblOnAccount.Text = _wfrPage.Content("OnAccountPaymentMethodLabel", _languageCode, True)
            ltlOnAccountTitle.Text = _wfrPage.Content("OnAccountTitle", _languageCode, True)
            If ltlOnAccountTitle.Text.Length = 0 Then plhOnAccountTitle.Visible = False
            ltlOnAccountSubTitle.Text = _wfrPage.Content("OnAccountSubTitle", _languageCode, True)
            If ltlOnAccountSubTitle.Text.Length = 0 Then plhOnAccountSubTitle.Visible = False
            ltlOnAccountH2.Text = _wfrPage.Content("OnAccountH2", _languageCode, True)
            If ltlOnAccountH2.Text.Length = 0 Then plhOnAccountH2.Visible = False
            ltlOnAccountMessage.Text = _wfrPage.Content("OnAccountMessage", _languageCode, True)
            If ltlOnAccountMessage.Text.Length = 0 Then plhOnAccountMessage.Visible = False
            ltlOnAccountLegend.Text = _wfrPage.Content("OnAccountLegend", _languageCode, True)
            lblOnAccountTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
            csvOnAccountTAndC.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
            btnConfirmOnAccountPayment.Text = _wfrPage.Content("ConfirmSaleButtonText", _languageCode, True)
            btnConfirmOnAccountPayment.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnConfirmOnAccountPayment))
        End If
        Return onAccountOptionActive
    End Function

    ''' <summary>
    ''' Determine if the Chip and Pin payment option is in use. Setup option if in use.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isChipAndPinOptionActive() As Boolean
        Dim chipAndPinOptionActive As Boolean = False
        If _hideTAndCCheckbox Then plhChipAndPinTAndC.Visible = False
        If _cpTypeEnabled Then
            chipAndPinOptionActive = True
            lblChipAndPin.Text = _wfrPage.Content("ChipAndPinPaymentMethodLabel", _languageCode, True)
            ltlChipAndPinTitle.Text = _wfrPage.Content("ChipAndPinTitle", _languageCode, True)
            If ltlChipAndPinTitle.Text.Length = 0 Then plhChipAndPinTitle.Visible = False
            ltlChipAndPinTitle.Text = _wfrPage.Content("ChipAndPinSubTitle", _languageCode, True)
            If ltlChipAndPinTitle.Text.Length = 0 Then plhChipAndPinTitle.Visible = False
            ltlChipAndPinMessage.Text = _wfrPage.Content("ChipAndPinMessage", _languageCode, True)
            If ltlChipAndPinMessage.Text.Length = 0 Then plhChipAndPinMessage.Visible = False
            ltlChipAndPinLegend.Text = _wfrPage.Content("ChipAndPinLegend", _languageCode, True)
            lblChipAndPinDevices.Text = _wfrPage.Content("ChipAndPinDevicesLabel", _languageCode, True)
            lblChipAndPinTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
            csvChipAndPin.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
            btnConfirmChipAndPinPayment.Text = _wfrPage.Content("ConfirmSaleButtonText", _languageCode, True)
        End If
        Return chipAndPinOptionActive
    End Function

    ''' <summary>
    ''' Determine if the invoice payment option is in use. Setup option if in use.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isInvoiceOptionActive() As Boolean
        Dim invoiceOptionActive As Boolean = False
        If _invTypeEnabled Then
            If AgentProfile.IsAgent Then
                If AgentProfile.AgentPermissions.CanBuyWithInvoice Then
                    invoiceOptionActive = True
                Else
                    invoiceOptionActive = False
                End If
            Else
                If Profile.Basket.PAYMENT_OPTIONS.Contains(GlobalConstants.INVPAYMENTTYPE) Then
                    invoiceOptionActive = True
                End If
            End If
            lblInvoice.Text = _wfrPage.Content("InvoicePaymentMethodLabel", _languageCode, True)
            ltlInvoiceTitle.Text = _wfrPage.Content("InvoiceTitleLabel", _languageCode, True)
            If ltlInvoiceTitle.Text.Length = 0 Then plhInvoiceTitle.Visible = False
            ltlInvoiceSubTitle.Text = _wfrPage.Content("InvoiceSubTitleLabel", _languageCode, True)
            If ltlInvoiceSubTitle.Text.Length = 0 Then plhInvoiceSubTitle.Visible = False
            ltlInvoiceMessage.Text = _wfrPage.Content("InvoiceMessageLabel", _languageCode, True)
            If ltlInvoiceMessage.Text.Length = 0 Then plhInvoiceMessage.Visible = False
            ltlInvoiceLegend.Text = _wfrPage.Content("InvoiceLegend", _languageCode, True)
            lblInvoiceTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
            csvInvoice.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
            btnConfirmInvoicePayment.Text = _wfrPage.Content("ConfirmSaleButtonText", _languageCode, True)
            btnConfirmInvoicePayment.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnConfirmInvoicePayment))
        End If
        Return invoiceOptionActive
    End Function

    ''' <summary>
    ''' Determine if the Point Of Sale payment option is in use. Setup option if in use.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isPointOfSaleOptionActive() As Boolean
        Dim pointOfSaleOptionActive As Boolean = False
        If _hideTAndCCheckbox Then plhPointOfSaleTAndC.Visible = False
        If _psTypeEnabled Then
            pointOfSaleOptionActive = True
            lblPointOfSale.Text = _wfrPage.Content("PointOfSalePaymentMethodLabel", _languageCode, True)
            ltlPointOfSaleTitle.Text = _wfrPage.Content("PointOfSaleTitle", _languageCode, True)
            plhPointOfSaleTitle.Visible = (ltlPointOfSaleTitle.Text.Length > 0)
            ltlPointOfSaleSubTitle.Text = _wfrPage.Content("PointOfSaleSubTitle", _languageCode, True)
            plhPointOfSaleSubTitle.Visible = (ltlPointOfSaleSubTitle.Text.Length > 0)
            ltlPointOfSaleMessage.Text = _wfrPage.Content("PointOfSaleMessage", _languageCode, True)
            plhPointOfSaleMessage.Visible = (ltlPointOfSaleMessage.Text.Length > 0)
            ltlPointOfSaleLegend.Text = _wfrPage.Content("PointOfSaleLegend", _languageCode, True)
            lblPointOfSaleTerminals.Text = _wfrPage.Content("PointOfSaleTerminalsLabel", _languageCode, True)
            lblPointOfSaleTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
            csvPointOfSale.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
            btnConfirmPointOfSalePayment.Text = _wfrPage.Content("ConfirmSaleButtonText", _languageCode, True)
        End If
        Return pointOfSaleOptionActive
    End Function

    ''' <summary>
    ''' Setup the zero priced basket completion option.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isZeroPricedBasketOptionActive() As Boolean
        Dim zeroPricedBasketOptionActive As Boolean = False
        If _hideTAndCCheckbox Then plhZeroPricedBasketTAndC.Visible = False
        If _zbTypeEnabled AndAlso BasketContentTypeOverride() = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
            Dim basketTotal As Decimal = 0
            For Each basketItem As TalentBasketItem In Profile.Basket.BasketItems
                basketTotal += basketItem.Gross_Price
            Next
            If _basketOverAllTotal = 0 Then
                zeroPricedBasketOptionActive = True
                lblZeroPricedBasket.Text = _wfrPage.Content("ZeroPricedBasketPaymentMethodLabel", _languageCode, True)
                ltlZeroPricedBasketTitle.Text = _wfrPage.Content("ZeroPricedBasketTitle", _languageCode, True)
                If ltlZeroPricedBasketTitle.Text.Length = 0 Then plhZeroPricedBasketTitle.Visible = False
                ltlZeroPricedBasketSubTitle.Text = _wfrPage.Content("ZeroPricedBasketSubTitle", _languageCode, True)
                If ltlZeroPricedBasketSubTitle.Text.Length = 0 Then plhZeroPricedBasketSubTitle.Visible = False
                ltlZeroPricedBasketMessage.Text = _wfrPage.Content("ZeroPricedBasketMessage", _languageCode, True)
                If ltlZeroPricedBasketMessage.Text.Length = 0 Then plhZeroPricedBasketMessage.Visible = False
                ltlZeroPricedBasketLegend.Text = _wfrPage.Content("ZeroPricedBasketLegend", _languageCode, True)
                lblZeroPricedBasketTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
                csvZeroPricedBasketTAndC.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
                btnZeroPricedBasketPayment.Text = _wfrPage.Content("ConfirmSaleButtonText", _languageCode, True)
                btnZeroPricedBasketPayment.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnZeroPricedBasketPayment))

                'hide all other payment methods
                plhPaymentMethodCreditCard.Visible = False
                plhPaymentMethodSavedCard.Visible = False
                plhPaymentMethodDirectDebit.Visible = False
                plhPaymentMethodPayPal.Visible = False
                plhPaymentMethodGoogleCheckout.Visible = False
                plhPaymentMethodCashback.Visible = False
                plhPaymentMethodEPurse.Visible = False
                plhPaymentMethodCreditFinance.Visible = False
                plhPaymentMethodCash.Visible = False
                plhPaymentMethodOnAccount.Visible = False
                plhPaymentMethodChipAndPin.Visible = False
                plhPaymentMethodPDQ.Visible = False
                plhPaymentMethodPointOfSale.Visible = False
                plhPaymentMethodOthers.Visible = False
                plhPaymentMethodInvoice.Visible = False
            End If
        End If
        Return zeroPricedBasketOptionActive
    End Function

    ''' <summary>
    ''' Setup the PDQ payment option.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isPDQOptionActive() As Boolean
        Dim pdqOptionActive As Boolean = False
        If _hideTAndCCheckbox Then plhPDQTAndC.Visible = False
        If _pdTypeEnabled AndAlso BasketContentTypeOverride() = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
            pdqOptionActive = True
            lblPDQ.Text = _wfrPage.Content("PDQPaymentMethodLabel", _languageCode, True)
            ltlPDQTitle.Text = _wfrPage.Content("PDQTitle", _languageCode, True)
            If ltlPDQTitle.Text.Length = 0 Then plhPDQTitle.Visible = False
            ltlPDQSubTitle.Text = _wfrPage.Content("PDQSubTitle", _languageCode, True)
            If ltlPDQSubTitle.Text.Length = 0 Then plhPDQSubTitle.Visible = False
            ltlPDQH2.Text = _wfrPage.Content("PDQH2", _languageCode, True)
            If ltlPDQH2.Text.Length = 0 Then plhPDQH2.Visible = False
            ltlPDQMessage.Text = _wfrPage.Content("PDQMessage", _languageCode, True)
            If ltlPDQMessage.Text.Length = 0 Then plhPDQMessage.Visible = False
            ltlPDQLegend.Text = _wfrPage.Content("PDQLegend", _languageCode, True)
            lblPDQTAndC.Text = _wfrPage.Content("TAndCLabelText", _languageCode, True)
            csvPDQTAndC.ErrorMessage = _wfrPage.Content("TAndCErrorMessage", _languageCode, True)
            btnConfirmPDQPayment.Text = _wfrPage.Content("ConfirmSaleButtonText", _languageCode, True)
            btnConfirmPDQPayment.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnConfirmPDQPayment))
            'ltlPDQPartPmt.Text = _wfrPage.Content("PartPaymentLabel", _languageCode, True)
        End If
        Return pdqOptionActive
    End Function

    ''' <summary>
    ''' Determine if the multiple select payment methods box is in use. Setup option if in use.
    ''' </summary>
    ''' <returns>True if option is in use</returns>
    ''' <remarks></remarks>
    Private Function isSelectPaymentMethodActive() As Boolean
        Dim selectPaymentMethodActive As Boolean = False
        Dim availablePaymentMethods As New Collection
        If plhPaymentMethodCreditCard.Visible Then availablePaymentMethods.Add(GlobalConstants.CCPAYMENTTYPE)
        If plhPaymentMethodSavedCard.Visible Then availablePaymentMethods.Add(GlobalConstants.SAVEDCARDPAYMENTTYPE)
        If plhPaymentMethodDirectDebit.Visible Then availablePaymentMethods.Add(GlobalConstants.DDPAYMENTTYPE)
        If plhPaymentMethodCreditFinance.Visible Then availablePaymentMethods.Add(GlobalConstants.CFPAYMENTTYPE)
        If plhPaymentMethodPayPal.Visible Then availablePaymentMethods.Add(GlobalConstants.PAYPALPAYMENTTYPE)
        If plhPaymentMethodGoogleCheckout.Visible Then availablePaymentMethods.Add(GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE)
        If plhPaymentMethodCashback.Visible Then availablePaymentMethods.Add(GlobalConstants.CBPAYMENTTYPE)
        If plhPaymentMethodEPurse.Visible Then availablePaymentMethods.Add(GlobalConstants.EPURSEPAYMENTTYPE)
        If plhPaymentMethodCash.Visible Then availablePaymentMethods.Add(GlobalConstants.CSPAYMENTTYPE)
        If plhPaymentMethodInvoice.Visible Then availablePaymentMethods.Add(GlobalConstants.INVPAYMENTTYPE)
        If plhPaymentMethodOnAccount.Visible Then availablePaymentMethods.Add(GlobalConstants.OAPAYMENTTYPE)
        If plhPaymentMethodChipAndPin.Visible Then availablePaymentMethods.Add(GlobalConstants.CHIPANDPINPAYMENTTYPE)
        If plhPaymentMethodPointOfSale.Visible Then availablePaymentMethods.Add(GlobalConstants.POINTOFSALEPAYMENTTYPE)
        If plhPaymentMethodZeroPricedBasket.Visible Then availablePaymentMethods.Add(GlobalConstants.ZEROPRICEDBASKETPAYMENTTYPE)
        If plhPaymentMethodPDQ.Visible Then availablePaymentMethods.Add(GlobalConstants.PDQPAYMENTTYPE)
        If plhPaymentMethodOthers.Visible Then availablePaymentMethods.Add(GlobalConstants.OTHERSPAYMENTTYPE)
        If availablePaymentMethods.Count = 1 Then
            _currentPaymentMethod = availablePaymentMethods(1)
            _payingForPPS = isPayingForPPS()
            _singlePaymentMethod = True
            paymentSelected(_payingForPPS, True)
        ElseIf availablePaymentMethods.Count > 1 Then
            selectPaymentMethodActive = True
            ltlPaymentOptionTitle.Text = _wfrPage.Content("PaymentOptionsTitle", _languageCode, True)
            ltlPaymentOptionsMessage.Text = _wfrPage.Content("PaymentOptionsMessage", _languageCode, True)
            ltlPaymentOptionsLegend.Text = _wfrPage.Content("PaymentOptionsLegend", _languageCode, True)
            btnPaymentOptionSelected.Text = _wfrPage.Content("PaymentOptionContinueButton", _languageCode, True)
            If ltlPaymentOptionTitle.Text.Length = 0 Then plhPaymentOptionTitle.Visible = False
            If ltlPaymentOptionsMessage.Text.Length = 0 Then plhPaymentOptionsMessage.Visible = False
        End If
        _numberOfActivePaymentMethods = availablePaymentMethods.Count
        Return selectPaymentMethodActive
    End Function

    ''' <summary>
    ''' Returns a string that disables the confirm button and changes its text
    ''' </summary>
    ''' <param name="ConfirmButton">The button we are applying the change to</param>
    ''' <returns>The string we need to set on the onlick attribute</returns>
    ''' <remarks></remarks>
    Private Function getJSFunctionForConfirmButton(ByRef ConfirmButton As Button) As String
        Dim javascriptFunction As New StringBuilder()
        javascriptFunction.Append("if (typeof(Page_ClientValidate) == 'function') { ")
        javascriptFunction.Append("var oldPage_IsValid = Page_IsValid; var oldPage_BlockSubmit = Page_BlockSubmit;")
        javascriptFunction.Append("if (Page_ClientValidate('")
        javascriptFunction.Append(ConfirmButton.ValidationGroup)
        javascriptFunction.Append("') == false) {")
        javascriptFunction.Append(" Page_IsValid = oldPage_IsValid; Page_BlockSubmit = oldPage_BlockSubmit; return false; }} ")
        javascriptFunction.Append("this.value = '")
        javascriptFunction.Append(_wfrPage.Content("ProcessingText", _languageCode, True))
        javascriptFunction.Append("';")
        javascriptFunction.Append("this.disabled = true;")
        javascriptFunction.Append(Me.Page.ClientScript.GetPostBackEventReference(ConfirmButton, Nothing))
        javascriptFunction.Append(";")
        javascriptFunction.Append("return true;")
        Return javascriptFunction.ToString()
    End Function

    ''' <summary>
    ''' Determine if the basket is valid for 3D Secure and 3D Secure is active.
    ''' </summary>
    ''' <returns>A boolean to indicate 3DSecure is in use</returns>
    ''' <remarks></remarks>
    Private Function is3DSecureInUse() As Boolean

        Return Checkout.is3DSecureInUse(AgentProfile.IsAgent, BasketContentTypeOverride(), ModuleDefaults)

    End Function

    ''' <summary>
    ''' Check that the card type supports 3D secure.
    ''' If 3D Secure is active and the card type isn't supported (such as AMEX) don't process 3D Secure, but some 3D Secure values are stored and used during checkout.
    ''' </summary>
    ''' <returns>A boolean to indicate whether or not this card type supports 3D Secure</returns>
    ''' <remarks></remarks>
    Private Function canProcess3DSecure(ByVal isASavedCard As Boolean) As Boolean
        Dim process3DSecure As Boolean = True
        Dim cardType As String = String.Empty
        If isASavedCard Then
            Dim cardNumber As String = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)
            Dim maxInstallments As Integer = 0
            TCUtilities.CheckCardType(cardNumber, "SAVEDCARD", _wfrPage.FrontEndConnectionString, maxInstallments, cardType)
        Else
            cardType = Checkout.RetrievePaymentItemFromSession("CardType", GlobalConstants.CHECKOUTASPXSTAGE)
        End If
        If Not TDataObjects.PaymentSettings.TblCreditCard.CanCardTypeUse3DSecure(cardType) Then
            Checkout.Store3dDetails(ModuleDefaults.Payment3DSecureDefaultECI, String.Empty, String.Empty, String.Empty, String.Empty, _
                                    ModuleDefaults.Payment3DSecureDefaultATSData, String.Empty, String.Empty, String.Empty, String.Empty)
            process3DSecure = False
        End If
        Return process3DSecure
    End Function

    ''' <summary>
    ''' Determine if the auto enrol pps option is in use. Setup option if in use.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function isAutoEnrolPPSOptionActive() As Boolean
        Dim showPanel As Boolean = False
        If ModuleDefaults.AutoEnrolPPSOnPayment AndAlso Not Profile.IsAnonymous Then
            For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                If tbi.PRODUCT_TYPE = "M" AndAlso ModuleDefaults.EPurseTopUpProductCode <> tbi.Product Then
                    If Not isAHocRefundProduct(tbi.Product) AndAlso Not isDirectDebitRefundProduct(tbi.Product) Then
                        showPanel = True
                    End If
                    Exit For
                End If
            Next
            If showPanel Then
                lblAutoEnrolPPS.Text = _wfrPage.Content("AutoEnrolPPSCheckboxLabel", _languageCode, True)
                ltlAutoEnrolPPSTitle.Text = _wfrPage.Content("AutoEnrolPPSTitle", _languageCode, True)
                plhAutoEnrolPPS.Visible = (ltlAutoEnrolPPSTitle.Text.Length > 0)
                If Session("AutoEnrolPPS") IsNot Nothing Then
                    chkAutoEnrolPPS.Checked = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_wfrPage.Attribute("AutoEnrolPPSCheckboxDefault"))
                Else
                    chkAutoEnrolPPS.Checked = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(Session("AutoEnrolPPS"))
                End If
            End If
        End If
        Return showPanel
    End Function

    ''' <summary>
    ''' Determine if the given product code is an adhoc refund product
    ''' </summary>
    ''' <param name="productCode">The product code to check</param>
    ''' <returns>True if adhoc refund product</returns>
    ''' <remarks></remarks>
    Private Function isAHocRefundProduct(ByVal productCode As String) As Boolean
        Dim isAdHocRefund As Boolean = False
        Dim dsProductDetails As New DataSet
        Dim err As ErrorObj = Nothing
        Dim tp As New TalentProduct
        Dim deP As New DEProductDetails
        deP.ProductCode = productCode
        With tp
            .Settings = TEBUtilities.GetSettingsObject()
            tp.Settings.Cacheing = True
            .De = deP
        End With
        err = tp.ProductDetails
        dsProductDetails = tp.ResultDataSet
        If Not err.HasError AndAlso dsProductDetails IsNot Nothing AndAlso dsProductDetails.Tables.Count > 0 Then
            If dsProductDetails.Tables(0).Rows.Count > 0 Then
                If dsProductDetails.Tables(0).Rows(0)("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then
                    dsProductDetails = dsProductDetails
                    If dsProductDetails.Tables(2).Rows(0)("isAdHocREfund") Then
                        isAdHocRefund = True
                    End If
                End If
            End If
        End If
        Return isAdHocRefund
    End Function

    ''' <summary>
    ''' Determin if the given product code is a direct debit refund product
    ''' </summary>
    ''' <param name="productCode">The product code to check</param>
    ''' <returns>True if the product is a direct debit refund product</returns>
    ''' <remarks></remarks>
    Private Function isDirectDebitRefundProduct(ByVal productCode As String) As Boolean
        Dim isDirectDebitRefund As Boolean = False
        Dim dsProductDetails As New DataSet
        Dim err As ErrorObj = Nothing
        Dim tp As New TalentProduct
        Dim deP As New DEProductDetails
        deP.ProductCode = productCode
        With tp
            .Settings = TEBUtilities.GetSettingsObject()
            .Settings.Cacheing = True
            .De = deP
        End With
        err = tp.ProductDetails
        dsProductDetails = tp.ResultDataSet
        If Not err.HasError AndAlso dsProductDetails IsNot Nothing AndAlso dsProductDetails.Tables.Count > 0 Then
            If dsProductDetails.Tables(0).Rows.Count > 0 Then
                If dsProductDetails.Tables(0).Rows(0)("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then
                    dsProductDetails = dsProductDetails
                    If dsProductDetails.Tables(2).Rows(0)("isDDRefundProduct") Then
                        isDirectDebitRefund = True
                    End If
                End If

            End If
        End If
        Return isDirectDebitRefund
    End Function

    ''' <summary>
    ''' Determine if the ticketing promotions option is in use. Setup Option if in use.
    ''' </summary>
    ''' <returns>A boolean value to indicate if the option is active</returns>
    ''' <remarks></remarks>
    Private Function isTicketingPromotionsOptionActive() As Boolean
        Dim promotionsOptionActive As Boolean = False
        If (ModuleDefaults.CheckoutPromotionType.Length > 0 AndAlso Profile.Basket.CAT_MODE.Trim.Length = 0) Then
            If shortHandBasketContentTypeToPromoFullName(Profile.Basket.BasketContentType) = ModuleDefaults.CheckoutPromotionType Then
                lblPromotions.Text = _wfrPage.Content("PromotionsLabel", _languageCode, True)
                btnPromotions.Text = _wfrPage.Content("PromotionsButtonText", _languageCode, True)
                rfvPromotions.ErrorMessage = _wfrPage.Content("NoPromoCodeError", _languageCode, True)
                If Session("PartnerPromotionCode") IsNot Nothing Then
                    txtPromotions.Text = Session("PartnerPromotionCode")
                    Session("PartnerPromotionCode") = Nothing
                    processPromotion()
                End If
                promotionsOptionActive = True
                Dim cashbackItemInBasket As Boolean = False
                Dim promotionApplied As Boolean = TDataObjects.BasketSettings.TblBasketPromotionItems.DoesBasketHeaderIdHavePromotions(Profile.Basket.Basket_Header_ID, GlobalConstants.BASKETMODULETICKETING)
                For Each item As TalentBasketItem In Profile.Basket.BasketItems
                    If item.Product = ModuleDefaults.CashBackFeeCode Then
                        cashbackItemInBasket = True
                        Exit For
                    End If
                Next
                If Profile.Basket.BasketSummary.TotalPartPayments > 0 Or cashbackItemInBasket Then
                    txtPromotions.ReadOnly = True
                    btnPromotions.Enabled = False
                Else
                    txtPromotions.ReadOnly = False
                    btnPromotions.Enabled = True
                End If
            End If
        End If
        Return promotionsOptionActive
    End Function

    ''' <summary>
    ''' Returns the full name of a basket content type from the promotions full name.
    ''' Example: shortHandBasketContentTypeToFullName("M") = RETAIL
    ''' </summary>
    ''' <param name="shortHandBasketContentType">The short hand basket content type</param>
    ''' <returns>The checkout promotion type</returns>
    ''' <remarks></remarks>
    Private Function shortHandBasketContentTypeToPromoFullName(ByVal shortHandBasketContentType As String) As String
        Select Case shortHandBasketContentType
            Case Is = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE
                Return GlobalConstants.RETAIL_PROMOTION_TYPE
            Case Is = GlobalConstants.TICKETINGBASKETCONTENTTYPE
                Return GlobalConstants.TICKETING_PROMOTION_TYPE
            Case Is = GlobalConstants.COMBINEDBASKETCONTENTTYPE
                Return "NOSHOW"
        End Select
        Return String.Empty
    End Function

    ''' <summary>
    ''' Return the row ID from tbl_chip_and_pin devices where the user matches the agent session
    ''' </summary>
    ''' <param name="dtChipAndPinDevices">The chip and pin devices table</param>
    ''' <returns>A string value row ID</returns>
    ''' <remarks></remarks>
    Private Function getSelectedValue(ByVal dtChipAndPinDevices As DataTable) As String
        Dim selectedValue As String = String.Empty
        If Session.Item("Agent") IsNot Nothing Then
            Dim agent As String = Session.Item("Agent")
            For Each row As DataRow In dtChipAndPinDevices.Rows
                If TEBUtilities.CheckForDBNull_String(row("TERMINAL_DEVICE_USER")) = agent Then
                    selectedValue = row("ID")
                    Exit For
                End If
            Next
        End If
        Return selectedValue
    End Function

    ''' <summary>
    ''' Take payment routine for mixed and retail baskets
    ''' </summary>
    ''' <returns>A boolean value to indicate a success</returns>
    ''' <remarks></remarks>
    Private Function takePayment() As Boolean
        Dim paymentError As Boolean = False
        If ModuleDefaults.PaymentCallBankAPI OrElse (ModuleDefaults.PaymentProcess3dSecure AndAlso ModuleDefaults.Payment3DSecureProvider <> String.Empty) Then
            Checkout.UpdateOrderStatus("PAYMENT ATTEMPTED", "Card/Account ending: " & Checkout.RetrieveAccountEnd() & ". Attempt Payment.")
            paymentError = True
            Select Case ModuleDefaults.PaymentGatewayType
                Case Is = "COMMIDEA"
                    paymentError = ProcessCommideaTransaction()
                Case Is = "HSBC"
                    paymentError = ProcessHSBCTransaction()
                Case Is = "TICKETINGBACKEND"
                    paymentError = ProcessTicketingBackendTransaction()
                Case Is = GlobalConstants.ECENTRICGATEWAY
                    If is3DSecureInUse() AndAlso canProcess3DSecure(False) Then
                        Response.Redirect("checkout3dSecure.aspx")
                    Else
                        If Session("TakeFinalPayment") = True Then
                            _TicketingGatewayFunctions.CheckoutPayment(False, String.Empty, paymentError)
                        End If
                    End If
                Case Else
                    paymentError = ProcessCommideaTransaction()
            End Select
        Else
            Dim paymentAPILogText As String = "Card ending: " & Checkout.RetrieveAccountEnd() & ". Dummy Bank Call."
            Checkout.UpdateOrderStatus("PAYMENT ATTEMPTED", paymentAPILogText)
        End If
        Return Not paymentError
    End Function

    ''' <summary>
    ''' Process payment via CommIdea.
    ''' </summary>
    ''' <returns>A Boolean value to indicate an error</returns>
    ''' <remarks></remarks>
    Protected Function ProcessCommideaTransaction() As Boolean
        Dim paymentError As Boolean = True
        Dim CClient As CardProcessing.Commidea.CommideaClient
        Dim TRequest As CardProcessing.Commidea.TransactionRequest
        Dim SResponse As CardProcessing.Commidea.WebServices.StandardResponse = Nothing
        Dim strWebRef As String

        strWebRef = Profile.Basket.Basket_Header_ID
        CClient = New CardProcessing.Commidea.CommideaClient
        TRequest = New CardProcessing.Commidea.TransactionRequest
        TRequest.AccountID = ModuleDefaults.PaymentDetails1
        TRequest.AccountNumber = ModuleDefaults.PaymentDetails2
        TRequest.GUID = ModuleDefaults.PaymentDetails3
        TRequest.CSC = Checkout.RetrievePaymentItemFromSession("CV2Number", GlobalConstants.CHECKOUTASPXSTAGE)
        TRequest.CNP = True
        TRequest.ECom = True
        TRequest.ExpiryDate = Checkout.RetrievePaymentItemFromSession("ExpiryDate", GlobalConstants.CHECKOUTASPXSTAGE)
        TRequest.Issue = Checkout.RetrievePaymentItemFromSession("IssueNumber", GlobalConstants.CHECKOUTASPXSTAGE)
        TRequest.MerchantData = strWebRef
        TRequest.Pan = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)
        TRequest.Reference = strWebRef

        '-------------------------------------
        ' Get address details off order header
        '-------------------------------------
        Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
        Dim dt As Data.DataTable = orders.Get_UNPROCESSED_Order(TalentCache.GetBusinessUnit, Profile.UserName)
        If dt.Rows.Count > 0 Then
            Dim total As Decimal = TEBUtilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_ORDER_VALUE"))
            Dim promo As Decimal = TEBUtilities.CheckForDBNull_Decimal(dt.Rows(0)("PROMOTION_VALUE"))
            '--------------------------------------------
            ' Set AVS details - From registration address
            '--------------------------------------------
            Dim registrationAddress As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)
            If registrationAddress.Address_Line_1 Is Nothing AndAlso registrationAddress.Post_Code Is Nothing Then
                TRequest.AddressLine1 = String.Empty
                TRequest.PostCode = String.Empty
            Else
                TRequest.AddressLine1 = TEBUtilities.CheckForDBNull_String(registrationAddress.Address_Line_1)
                If TRequest.AddressLine1 = String.Empty Then
                    TRequest.AddressLine1 = TEBUtilities.CheckForDBNull_String(registrationAddress.Address_Line_2)
                End If
                TRequest.PostCode = TEBUtilities.CheckForDBNull_String(registrationAddress.Post_Code)
            End If
            TRequest.TxnValue = total
            If Not ModuleDefaults.Call_Tax_WebService Then
                TRequest.TxnValue = total - promo
            End If
        End If
        TRequest.RejectAdAvs = ModuleDefaults.PaymentRejectAddressAVS
        TRequest.RejectPcAvs = ModuleDefaults.PaymentRejectPostcodeAVS
        TRequest.RejectCsc = ModuleDefaults.PaymentRejectCSC
        TRequest.AllowPartialAvs = ModuleDefaults.PaymentAllowPartialAVS
        TRequest.Debug = ModuleDefaults.PaymentDebug

        If Checkout.RetrievePaymentItemFromSession("StartDate", GlobalConstants.CHECKOUTASPXSTAGE) <> "0000" Then
            TRequest.StartDate = Checkout.RetrievePaymentItemFromSession("StartDate", GlobalConstants.CHECKOUTASPXSTAGE)
        Else
            TRequest.StartDate = ""
        End If

        CClient.TransactionURL = ModuleDefaults.PaymentURL1
        CClient.ConfirmURL = ModuleDefaults.PaymentURL2
        '----------------------------------------------------------------
        ' 01=Purchase,02=Refund,04=Cash Advance,05=Purchase with cashback
        '----------------------------------------------------------------
        TRequest.TxnType = "01"
        _avsOrCscRejected = False
        _PaymentAPILogText = String.Empty

        Try
            SResponse = CClient.DoTransactionRequest(TRequest)
        Catch ex As Exception
            Dim errObj As New Talent.Common.ErrorObj
            ' Handle error codes
            Select Case CClient.ErrorNo
                ' Internal error, payment not taken
                Case Is = "COMMCCL-010"
                    errObj.ErrorMessage = _wfrPage.Content("payError1", _languageCode, True)

                    ' Internal error, payment may have been taken
                Case Is = "COMMCCL-020"
                    errObj.ErrorMessage &= CClient.ErrorText
                    errObj.ErrorMessage = _wfrPage.Content("payError1", _languageCode, True)
            End Select
            errObj.ErrorNumber = CClient.ErrorNo
            errObj.HasError = True
            errObj.ErrorStatus = ""
            Session("talentErrorObj") = errObj
            Checkout.UpdateOrderStatus("PAYMENT REJECTED", CClient.LogText)
            Response.Redirect("~/PagesPublic/error/error.aspx")
        End Try

        _PaymentAPILogText = CClient.LogText
        If CClient.Completed Then
            paymentError = False
        Else
            If CClient.AvsOrCscRejected Then
                _avsOrCscRejected = True
            End If
            paymentError = True
        End If

        Return paymentError

    End Function

    ''' <summary>
    ''' Process payment via HSBC.
    ''' </summary>
    ''' <returns>A boolean value to indicate an error as occurred</returns>
    ''' <remarks></remarks>
    Protected Function ProcessHSBCTransaction() As Boolean
        Dim paymentError As Boolean = True
        Dim HSBCRequest As New CardProcessing.HSBC.HSBCApiRequest
        Dim strWebRef As String
        _avsOrCscRejected = False
        strWebRef = Profile.Basket.Basket_Header_ID
        Dim clientID As String = ModuleDefaults.PaymentDetails3
        Dim clientAlias As String = ModuleDefaults.PaymentDetails4

        ' User
        HSBCRequest.user = ModuleDefaults.PaymentDetails1
        ' Password
        HSBCRequest.password = ModuleDefaults.PaymentDetails2
        ' Client
        HSBCRequest.clientId = ModuleDefaults.PaymentDetails3
        ' Client Alias
        HSBCRequest.clientAlias = ModuleDefaults.PaymentDetails4
        ' Processing Mode
        HSBCRequest.processingMode = ModuleDefaults.PaymentDetails5
        ' URL
        HSBCRequest.url = ModuleDefaults.PaymentURL1
        ' Debug
        HSBCRequest.debug = ModuleDefaults.PaymentDebug
        ' Reject address AVS
        HSBCRequest.RejectAdAvs = ModuleDefaults.PaymentRejectAddressAVS
        ' Reject postcode AVS
        HSBCRequest.RejectPcAvs = ModuleDefaults.PaymentRejectPostcodeAVS
        ' Reject CSC
        HSBCRequest.RejectCsc = ModuleDefaults.PaymentRejectCSC
        ' Unique transaction ID (this will get overwritten if 3d-secure)
        ' HSBCRequest.payerTxnId = strWebRef
        HSBCRequest.payerTxnId = String.Empty

        ' Check whether need to skip pre-auth and just do Auth (as per Simon Jersey)
        If ModuleDefaults.PaymentSkipPreAuth Then
            HSBCRequest.authType = "Auth"
        End If

        '---------------------------
        ' Set properties from screen
        '---------------------------
        ' Card Number
        HSBCRequest.ccNumber = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)
        ' Expiry
        Dim expiryDate As String = Checkout.RetrievePaymentItemFromSession("ExpiryDate", GlobalConstants.CHECKOUTASPXSTAGE)
        expiryDate = expiryDate.Substring(0, 2) & "/" & expiryDate.Substring(2, 2)
        HSBCRequest.ccExpiry = expiryDate
        ' Start Date (format MM/YY)
        If Checkout.RetrievePaymentItemFromSession("StartDate", GlobalConstants.CHECKOUTASPXSTAGE) <> "0000" Then
            Dim startDate As String = Checkout.RetrievePaymentItemFromSession("StartDate", GlobalConstants.CHECKOUTASPXSTAGE)
            startDate = startDate.Substring(0, 2) & "/" & startDate.Substring(2, 2)
            HSBCRequest.ccStartDate = startDate
        Else
            HSBCRequest.ccStartDate = String.Empty
        End If
        ' Issue Number
        HSBCRequest.issueNo = Checkout.RetrievePaymentItemFromSession("IssueNumber", GlobalConstants.CHECKOUTASPXSTAGE)
        ' Card security code
        HSBCRequest.csc = Checkout.RetrievePaymentItemFromSession("CV2Number", GlobalConstants.CHECKOUTASPXSTAGE)

        '-------------------------------------------
        ' Get del address and value off order header
        '-------------------------------------------
        Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
        Dim dt As Data.DataTable = orders.Get_UNPROCESSED_Order(TalentCache.GetBusinessUnit, Profile.UserName)

        If dt.Rows.Count > 0 Then
            Dim total As Decimal = TEBUtilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_ORDER_VALUE"))
            Dim promo As Decimal = TEBUtilities.CheckForDBNull_Decimal(dt.Rows(0)("PROMOTION_VALUE"))
            ' Amount (in pence) (Convert
            HSBCRequest.amountPence = (CInt((total) * 100)).ToString
            If Not ModuleDefaults.Call_Tax_WebService Then
                HSBCRequest.amountPence = (CInt((total - promo) * 100)).ToString
            End If
            ' Delivery address
            HSBCRequest.deliveryAddressLine1 = TEBUtilities.CheckForDBNull_String(dt.Rows(0)("ADDRESS_LINE_1"))
            If HSBCRequest.deliveryAddressLine1 = String.Empty Then
                HSBCRequest.deliveryAddressLine1 = TEBUtilities.CheckForDBNull_String(dt.Rows(0)("ADDRESS_LINE_2"))
            End If
            HSBCRequest.deliveryPostCode = TEBUtilities.CheckForDBNull_String(dt.Rows(0)("POSTCODE"))
        End If

        '--------------------------------------------
        ' Set AVS details - From registration address
        '--------------------------------------------
        Dim registrationAddress As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)
        HSBCRequest.addressLine1 = TEBUtilities.CheckForDBNull_String(registrationAddress.Address_Line_1)
        If HSBCRequest.addressLine1 = String.Empty Then
            HSBCRequest.addressLine1 = TEBUtilities.CheckForDBNull_String(registrationAddress.Address_Line_2)
        End If
        HSBCRequest.postCode = TEBUtilities.CheckForDBNull_String(registrationAddress.Post_Code)

        '-----------------------------------
        ' Set billing country as iso country
        '-----------------------------------
        Dim countries As New TalentApplicationVariablesTableAdapters.tbl_countryTableAdapter
        Dim dtCountries As Data.DataTable = countries.GetDataByCountryCode(Profile.User.Addresses(0).Country)
        If dtCountries.Rows.Count > 0 Then
            HSBCRequest.country = dtCountries.Rows(0)("ISO_CODE").ToString.Trim
        End If

        '----------------------------------------------
        ' Set Customer details - From registration dets
        '----------------------------------------------
        HSBCRequest.emailAddress = Profile.User.Details.Email
        HSBCRequest.firstName = Profile.User.Details.Forename
        HSBCRequest.lastName = Profile.User.Details.Surname
        '------------------------
        ' Set Customer IP address
        '------------------------
        HSBCRequest.IPAddress = Request.ServerVariables("REMOTE_ADDR")

        Dim priceListHeader As New TalentBasketDatasetTableAdapters.tbl_price_list_headerTableAdapter
        Dim dt3 As Data.DataTable = priceListHeader.Get_PriceList_Header_By_BU_Partner(TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
        If dt3.Rows.Count > 0 AndAlso TEBUtilities.CheckForDBNull_String(dt3.Rows(0)("CURRENCY_CODE").ToString) <> String.Empty Then
            '---------------------------------------
            ' Look up 3 Char code to get 3 digit ISO
            '---------------------------------------
            Dim currenciesTable As New TalentApplicationVariablesTableAdapters.tbl_currencyTableAdapter
            Dim dt4 As Data.DataTable = currenciesTable.GetDataByCurrencyCode(dt3.Rows(0)("CURRENCY_CODE").ToString)
            Dim curCode As String = String.Empty
            If dt4.Rows.Count > 0 AndAlso TEBUtilities.CheckForDBNull_String(dt4.Rows(0)("CURRENCY_CODE_1").ToString) <> String.Empty Then
                HSBCRequest.currency = dt4.Rows(0)("CURRENCY_CODE_1").ToString
                ' Check for overides at currency level
                If dt4.Rows(0)("CURRENCY_CODE_3").ToString.Trim <> String.Empty AndAlso dt4.Rows(0)("CURRENCY_CODE_4").ToString.Trim <> String.Empty Then
                    HSBCRequest.clientId = dt4.Rows(0)("CURRENCY_CODE_3").ToString.Trim
                    HSBCRequest.clientAlias = dt4.Rows(0)("CURRENCY_CODE_4").ToString.Trim
                End If
            Else
                HSBCRequest.currency = "826"
            End If

        Else
            ' try with *all
            dt3 = priceListHeader.Get_PriceList_Header_By_BU_Partner(TalentCache.GetBusinessUnit, "*ALL")
            '---------------------------------------
            ' Look up 3 Char code to get 3 digit ISO
            '---------------------------------------
            Dim currenciesTable As New TalentApplicationVariablesTableAdapters.tbl_currencyTableAdapter
            Dim dt4 As Data.DataTable = currenciesTable.GetDataByCurrencyCode(dt3.Rows(0)("CURRENCY_CODE").ToString)
            Dim curCode As String = String.Empty
            If dt4.Rows.Count > 0 AndAlso TEBUtilities.CheckForDBNull_String(dt4.Rows(0)("CURRENCY_CODE_1").ToString) <> String.Empty Then
                HSBCRequest.currency = dt4.Rows(0)("CURRENCY_CODE_1").ToString
                ' Check for overides at currency level
                If dt4.Rows(0)("CURRENCY_CODE_3").ToString.Trim <> String.Empty AndAlso dt4.Rows(0)("CURRENCY_CODE_4").ToString.Trim <> String.Empty Then
                    HSBCRequest.clientId = dt4.Rows(0)("CURRENCY_CODE_3").ToString.Trim
                    HSBCRequest.clientAlias = dt4.Rows(0)("CURRENCY_CODE_4").ToString.Trim
                End If
            Else
                HSBCRequest.currency = "826"
            End If
        End If
        '---------------------------------------------------------------
        ' Save dets for taking payment on CheckoutOrderConfirmation page
        '---------------------------------------------------------------
        Session("HSBCRequest") = HSBCRequest
        If ModuleDefaults.PaymentProcess3dSecure AndAlso ModuleDefaults.Payment3DSecureProvider <> String.Empty Then
            HttpContext.Current.Session("BackendProcessInCheckout") = Nothing
            Logging.WriteLog(Profile.UserName, "CPD-0030", "ProcessHSBCTransaction... Transfering to Checkout3dSecure", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), "Checkout")
            Response.Redirect("Checkout3dSecure.aspx")
        Else
            HttpContext.Current.Session("BackendProcessInCheckout") = Nothing
            Logging.WriteLog(Profile.UserName, "CPD-0020", "ProcessHSBCTransaction... Transfering to CheckoutOrderConfirmation", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), "Checkou")
            Response.Redirect("CheckoutOrderConfirmation.aspx")
        End If

        Return paymentError

    End Function

    ''' <summary>
    ''' Take payment via whatever is setup in the ticketing backend
    ''' </summary>
    ''' <returns>A boolean value to indicate an error as occurred</returns>
    ''' <remarks></remarks>
    Protected Function ProcessTicketingBackendTransaction() As Boolean
        Dim paymentError As Boolean = True
        Dim err As New ErrorObj
        Dim payment As New TalentPayment
        Dim dePayment As New DEPayments
        With dePayment
            .SessionId = Profile.Basket.Basket_Header_ID
            .CardNumber = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)
            .ExpiryDate = Checkout.RetrievePaymentItemFromSession("ExpiryDate", GlobalConstants.CHECKOUTASPXSTAGE)
            .StartDate = Checkout.RetrievePaymentItemFromSession("StartDate", GlobalConstants.CHECKOUTASPXSTAGE)
            .IssueNumber = Checkout.RetrievePaymentItemFromSession("IssueNumber", GlobalConstants.CHECKOUTASPXSTAGE)
            .CV2Number = Checkout.RetrievePaymentItemFromSession("CV2Number", GlobalConstants.CHECKOUTASPXSTAGE)
            .Source = "M"
            .PaymentType = GlobalConstants.CCPAYMENTTYPE
            .Amount = CType(Checkout.MerchandiseTotalFromOrderHeader, String)
            .CustomerNumber = Profile.User.Details.LoginID
        End With
        payment.De = dePayment
        payment.Settings = TEBUtilities.GetSettingsObject()
        payment.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        payment.Settings.StoredProcedureGroup = TEBUtilities.GetStoredProcedureGroup
        If Not String.IsNullOrEmpty(Session.Item("Agent")) Then
            payment.Settings.OriginatingSource = CStr(Session.Item("Agent"))
        End If
        If Not String.IsNullOrEmpty(Session.Item("AgentType")) Then
            payment.Settings.AgentType = CStr(Session.Item("AgentType"))
        End If
        _PaymentAPILogText = String.Empty
        err = payment.TakePaymentViaBackend

        If Not payment.ResultDataSet Is Nothing Then
            Dim errorTable As DataTable = Nothing
            Try
                errorTable = payment.ResultDataSet.Tables(0)
            Catch ex As Exception

            End Try
            If Not errorTable Is Nothing Then
                For Each row As Data.DataRow In errorTable.Rows
                    If Not row("ReturnCode").Equals(String.Empty) Then
                        _PaymentAPILogText = "Card ending: " & Checkout.RetrieveAccountEnd() & " - Return Code = " & row("ReturnCode").ToString
                        paymentError = True
                        Exit For
                    Else
                        _PaymentAPILogText = "Card ending: " & Checkout.RetrieveAccountEnd() & " - Return Code = ACCEPTED"
                        paymentError = False
                    End If
                Next
            End If
        End If
        Return paymentError
    End Function

    ''' <summary>
    ''' Updates the payment type in basket header.
    ''' </summary>
    ''' <param name="isToUpdatePayTypeInBsktHeader">if set to <c>true</c> [is to update pay type in BSKT header].</param><returns></returns>
    Private Function UpdatePaymentTypeInBasketHeader(ByVal isToUpdatePayTypeInBsktHeader As Boolean) As Boolean
        Dim isUpdated As Boolean = False
        If _payingForPPS Then
            If TDataObjects.BasketSettings.TblBasketDetail.DeleteAllPayTypeFees(Profile.Basket.Basket_Header_ID) > 0 Then
                isUpdated = True
            End If
        Else

            Dim talBasketSummary As New Talent.Common.TalentBasketSummary
            talBasketSummary.Settings = TEBUtilities.GetSettingsObject()
            talBasketSummary.LoginID = Profile.UserName
            talBasketSummary.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talBasketSummary.Settings.BusinessUnit)
            talBasketSummary.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talBasketSummary.Settings.BusinessUnit)
            isUpdated = talBasketSummary.UpdateBasketPayTypeOrCardType(Profile.Basket.Basket_Header_ID, _currentPaymentMethod, "", isToUpdatePayTypeInBsktHeader, True)
        End If
        Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
        Return isUpdated
    End Function

    ''' <summary>
    ''' Returns the print option based upon Print Address option and Print Receipt option
    ''' </summary>
    ''' <returns>1=Print Tickets only, 2=Print Address + Tickets, 3=Print Receipt + Tickets, 4=Print Address + Receipt + Tickets</returns>
    ''' <remarks></remarks>
    Private Function SetPrintOption(ByVal boolPrintAddress As Boolean, ByVal boolPrintReceipt As Boolean) As String
        Dim printOption As String = ""
        Dim basketContainsTicketItems As Boolean = containsTicketItem()
        If basketContainsTicketItems Then
            If boolPrintAddress AndAlso boolPrintReceipt Then
                printOption = "4"
            ElseIf Not boolPrintAddress AndAlso boolPrintReceipt Then
                printOption = "3"
            ElseIf boolPrintAddress AndAlso Not boolPrintReceipt Then
                printOption = "2"
            Else
                printOption = "1"
            End If
        End If
        Return printOption
    End Function

    ''' <summary>
    ''' Can the payment options be reset
    ''' </summary>
    ''' <returns>True if the payment options are all paid</returns>
    ''' <remarks></remarks>
    Private Function canResetPayOptions() As Boolean
        Dim isAllPaid As Boolean = False
        If Session("basketPPS1List") IsNot Nothing Then
            If Session("PPS1PaymentComplete") Is Nothing Then
                isAllPaid = False
            Else
                isAllPaid = True
            End If
        End If
        If Session("basketPPS2List") IsNot Nothing Then
            If Session("PPS2PaymentComplete") Is Nothing Then
                isAllPaid = False
            Else
                isAllPaid = True
            End If
        End If
        If isAllPaid AndAlso Session("PPSPayment") = True Then
            isAllPaid = True
        ElseIf isAllPaid AndAlso Session("PPSPayment") = False Then
            isAllPaid = False
        End If
        Return isAllPaid
    End Function

    ''' <summary>
    ''' Does the basket contain the registered post option
    ''' </summary>
    ''' <returns>True if the registered post option is available</returns>
    ''' <remarks></remarks>
    Private Function containsPostOrRegisteredPost() As Boolean
        Dim success As Boolean = False
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If (Not tbi.CURR_FULFIL_SLCTN Is Nothing AndAlso tbi.CURR_FULFIL_SLCTN.Trim = "P") Or (Not tbi.CURR_FULFIL_SLCTN Is Nothing AndAlso tbi.CURR_FULFIL_SLCTN.Trim = "R") Then
                success = True
                Exit For
            End If
        Next
        Return success
    End Function

    ''' <summary>
    ''' Does the basket contain the print option
    ''' </summary>
    ''' <returns>True if the basket contains the print option</returns>
    ''' <remarks></remarks>
    Private Function containsPrint() As Boolean
        Dim success As Boolean = False
        Dim printAlways As Boolean = AgentProfile.PrintAlways
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.CURR_FULFIL_SLCTN)) Then
                If (Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.FULFIL_OPT_PRINT))) AndAlso tbi.CURR_FULFIL_SLCTN.Trim = GlobalConstants.PRINT_FULFILMENT Then
                    success = True
                Else
                    If printAlways AndAlso _
                        ( _
                            (Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.FULFIL_OPT_COLL)) AndAlso tbi.CURR_FULFIL_SLCTN.Trim = GlobalConstants.COLLECT_FULFILMENT) _
                            OrElse (Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.FULFIL_OPT_POST)) AndAlso tbi.CURR_FULFIL_SLCTN.Trim = GlobalConstants.POST_FULFILMENT) _
                            OrElse (Not String.IsNullOrWhiteSpace(TEBUtilities.CheckForDBNull_String(tbi.FULFIL_OPT_REGPOST)) AndAlso tbi.CURR_FULFIL_SLCTN.Trim = GlobalConstants.REG_POST_FULFILMENT) _
                        ) Then
                        success = True

                    End If
                End If
                If success Then
                    Exit For
                End If
            End If
        Next
        Return success
    End Function

    ''' <summary>
    ''' Does the basket contain any retail items
    ''' </summary>
    ''' <returns>True if the basket contains</returns>
    ''' <remarks></remarks>
    Private Function containsRetailItem() As Boolean
        Dim success As Boolean = False
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If (Not tbi.PRODUCT_TYPE Is Nothing AndAlso tbi.PRODUCT_TYPE = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE) Then
                success = True
                Exit For
            End If
        Next
        Return success
    End Function

    ''' <summary>
    ''' Does the basket contain any ticketing items
    ''' </summary>
    ''' <returns>True if the basket contains ticketing items</returns>
    ''' <remarks></remarks>
    Private Function containsTicketItem() As Boolean
        Dim success As Boolean = False
        For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            If (Not tbi.PRODUCT_TYPE Is Nothing AndAlso tbi.PRODUCT_TYPE = GlobalConstants.TICKETINGBASKETCONTENTTYPE) Then
                success = True
                Exit For
            End If
        Next
        Return success
    End Function

    ''' <summary>
    ''' Hide the payment types that are not valid for part payments
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InvalidPartPaymentPayTypes()
        If Profile.Basket.BasketSummary.TotalPartPayments > 0 Then
            plhPaymentMethodDirectDebit.Visible = False
            plhPaymentMethodGoogleCheckout.Visible = False
            plhPaymentMethodCreditFinance.Visible = False
            plhPaymentMethodPayPal.Visible = False
            plhPaymentMethodInvoice.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Retrieve the basket content type if it's not set.
    ''' When orders are sent to the ticketing database we can support all of the  ticketing functions.  
    ''' There should be no difference so we will send the basket content back as ticket only
    ''' </summary>
    ''' <returns>Basket content type</returns>
    ''' <remarks></remarks>
    Private Function BasketContentTypeOverride() As String
        If String.IsNullOrWhiteSpace(_basketContentType) Then
            _basketContentType = Profile.Basket.BasketContentType
        End If
        If _TicketingCheckout Then
            Return GlobalConstants.TICKETINGBASKETCONTENTTYPE
        Else
            Return _basketContentType
        End If
    End Function

    ''' <summary>
    ''' Get the PayPalGateway url to use based on the current action of the basket
    ''' </summary>
    ''' <returns>Formatted redirection URL to use for the PayPalGatway page</returns>
    ''' <remarks></remarks>
    Private Function getPayPalRedirectFunction() As String
        Dim payPalFunction As String = String.Empty
        If CATHelper.IsBasketNotInCancelMode() Then
            payPalFunction = "~/Redirect/PayPalGateway.aspx?page=checkoutpaymentdetails.aspx&function=paymentexternalcheckout"
        Else
            payPalFunction = "~/Redirect/PayPalGateway.aspx?page=checkoutpaymentdetails.aspx&function=paymentexternalrefund"
        End If
        Return payPalFunction
    End Function

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Process the part payment and reset the payment options if successful
    ''' </summary>
    ''' <param name="paymentSuccess">The payment success flag</param>
    ''' <param name="paymentAmount">The payment ammount</param>
    ''' <remarks></remarks>
    Public Sub ProcessPartPayment(ByVal paymentSuccess As Boolean, ByVal paymentAmount As Decimal)
        uscBasketSummary.ReBindPartPayments()
        Dim canProcessBookingFees As Boolean = TEBUtilities.IsPartPayRequiresBookFeeProcess()
        Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True, canProcessBookingFees)

        'Was the payment a success
        If paymentSuccess Then

            'Send a success message
            Dim successMessage As String = _wfrPage.Content("SuccessfullyAppliedPartPayment", _languageCode, True)
            successMessage = successMessage.Replace("<<Amount>>", TDataObjects.PaymentSettings.FormatCurrency(paymentAmount, _wfrPage.BusinessUnit, _wfrPage.PartnerCode))
            successMessage = HttpUtility.HtmlDecode(successMessage)
            ltlSuccessMessages.Text = successMessage

            'Open the select payment accordion option
            _nextItemToOpen = 0
            resetPaymentOptions(True)
        End If
    End Sub

    ''' <summary>
    ''' Saved card web method - used when saving a card during payment with a new card
    ''' </summary>
    ''' <param name="basketPaymentID">The current basket header ID</param>
    ''' <param name="canSaveCard">Can save the card</param>
    ''' <returns>Success flag</returns>
    ''' <remarks></remarks>
    <System.Web.Services.WebMethod()> _
    Public Shared Function SetSaveCardFlag(ByVal basketPaymentID As String, ByVal canSaveCard As String) As String
        Dim flagSetSuccess As String = "false"
        Dim wfrPage As WebFormResource = New WebFormResource
        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = TEBUtilities.GetCurrentPageName()
            .PageCode = TEBUtilities.GetCurrentPageName()
        End With
        Dim vanguardAttributes As DEVanguard = Nothing
        Dim basketPayEntity As DEBasketPayment = Nothing
        Dim tempBasketPaymentID As Long = 0
        If TEBUtilities.TryGetVGAttributesSession(tempBasketPaymentID, vanguardAttributes, basketPayEntity) AndAlso basketPaymentID = tempBasketPaymentID Then
            Dim talVanguard As New TalentVanguard
            Dim err As New ErrorObj
            talVanguard.Settings = TEBUtilities.GetSettingsObject()
            talVanguard.VanguardAttributes = vanguardAttributes
            talVanguard.VanguardAttributes.BasketPaymentID = basketPaymentID
            talVanguard.VanguardAttributes.SaveThisCard = canSaveCard
            talVanguard.VanguardAttributes.ProcessStep = DEVanguard.ProcessingStep.SAVE_CARD_FLAG_UPDATE
            talVanguard.VanguardAttributes.SessionID = HttpContext.Current.Session.SessionID
            err = talVanguard.ProcessVanguard()
            If Not (err.HasError) Then
                flagSetSuccess = "true"
                TEBUtilities.SetVGAttributesSession(talVanguard.VanguardAttributes.BasketPaymentID, talVanguard.VanguardAttributes, talVanguard.BasketPayEntity)
            End If
        End If
        Return flagSetSuccess
    End Function

#End Region


End Class