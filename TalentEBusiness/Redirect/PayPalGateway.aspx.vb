Imports Talent.eCommerce
Imports Talent.Common.CardProcessing.PayPal

Partial Class Redirect_PayPalGateway
    Inherits Base01

#Region "Class Level Fields"
    Private Const STAGE1 As String = "1"
    Private Const STAGE2 As String = "2"
    Private Const PPPaySuccessStatus As String = "COMPLETED"

    Private _modouleDefs As Talent.eCommerce.ECommerceModuleDefaults
    Private _defaultsValue As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Private wfr As Talent.Common.WebFormResource
    Private _languageCode As String = String.Empty
    Private _talentLogging As Talent.Common.TalentLogging

    'Flag that determines the PayPal environment (live or sandbox) 
    Private _isSandbox As Boolean = True

    Private _httpWebReqTimeOut As Integer = 5000
    Private _APIUsername As String = String.Empty
    Private _APIPassword As String = String.Empty
    Private _APISignature As String = String.Empty
    Private _subject As String = String.Empty
    Private _hostURLLive As String = String.Empty
    Private _hostURLSandbox As String = String.Empty
    Private _endURLLive As String = String.Empty
    Private _endURLSandbox As String = String.Empty
    Private _gatewayURL As String = String.Empty
    Private _returnURL As String = String.Empty
    Private _cancelURL As String = String.Empty
    Private _gatewayUrl_cancelURL As String = String.Empty
    Private _cancelMessageForUser As String = String.Empty
    Private _basketTotal As String = String.Empty
    Private _basketHeaderID As String = String.Empty
    Private _basketTotalItemsIncFees As String = String.Empty
    Private _requestFields As Generic.Dictionary(Of String, String) = Nothing
    Private _payPalFunction As String = String.Empty
#End Region

#Region "Protected Methods"


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Profile.IsAnonymous Then
            ProcessError("PPGERR-001")
        Else
            wfr = New Talent.Common.WebFormResource
            _languageCode = Talent.Common.Utilities.GetDefaultLanguage
            _talentLogging = New Talent.Common.TalentLogging
            With wfr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = ProfileHelper.GetPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "PayPalGateway.aspx"
            End With
            _modouleDefs = New Talent.eCommerce.ECommerceModuleDefaults()
            _defaultsValue = _modouleDefs.GetDefaults

            Dim referrerUrl As String = String.Empty
            Try
                referrerUrl = Request.UrlReferrer.AbsoluteUri
            Catch ex As Exception
                referrerUrl = ex.Message
            End Try
            _talentLogging.FrontEndConnectionString = wfr.FrontEndConnectionString
            _talentLogging.GeneralLog(wfr.PageCode, "Page_Init", "Customer: " & Profile.User.Details.LoginID & ", Referrer URL: " & referrerUrl, "PayPalLogging")
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Profile.IsAnonymous Then
            ProcessError("PPGERR-001")
        Else
            'Check whether paypal payment cancel call or payment process call
            If Not String.IsNullOrEmpty(Request.QueryString("function")) Then
                _payPalFunction = Request.QueryString("function")
                AssignConfigurations()
                setLineItemRequestFields()
                Select Case _payPalFunction
                    Case Is = "paymentexternalcheckout"
                        _returnURL = _gatewayURL & "?function=paymentexternalcheckout"
                        _cancelURL = _gatewayURL & "?function=paymentexternalcheckout&PayPalCancelCall=CANCEL"
                        If Not PayPalCancelCall() Then
                            ProcessPayPalPayment()
                        End If
                    Case Is = "paymentexternalrefund"
                        _returnURL = _gatewayURL & "?function=paymentexternalrefund"
                        ProcessPayPalRefund()
                End Select
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub AssignConfigurations()
        _talentLogging.GeneralLog(wfr.PageCode, "AssignConfigurations", "Customer: " & Profile.User.Details.LoginID, "PayPalLogging")
        _APIUsername = Utilities.CheckForDBNull_String(wfr.Attribute("UserName"))
        _APIPassword = Utilities.CheckForDBNull_String(wfr.Attribute("Password"))
        _APISignature = Utilities.CheckForDBNull_String(wfr.Attribute("Signature"))
        _subject = Utilities.CheckForDBNull_String(wfr.Attribute("Subject"))
        _isSandbox = Utilities.CheckForDBNull_Boolean_DefaultFalse(wfr.Attribute("IsSandbox"))
        _httpWebReqTimeOut = Utilities.CheckForDBNull_Int(wfr.Attribute("HttpWebRequestTimeOut"))
        _endURLLive = Utilities.CheckForDBNull_String(wfr.Attribute("EndURLLive"))
        _endURLSandbox = Utilities.CheckForDBNull_String(wfr.Attribute("EndURLSandbox"))
        _hostURLLive = Utilities.CheckForDBNull_String(wfr.Attribute("HostURLLive"))
        _hostURLSandbox = Utilities.CheckForDBNull_String(wfr.Attribute("HostURLSandbox"))
        _gatewayURL = Utilities.CheckForDBNull_String(wfr.Attribute("PayPalGatewayUrl"))
        _cancelMessageForUser = Utilities.CheckForDBNull_String(wfr.Content("CancelMessageForUser", _languageCode, True))

        _basketTotal = Profile.Basket.BasketSummary.TotalBasket.ToString("#####0.00")
        _basketHeaderID = Profile.Basket.Basket_Header_ID
        _basketTotalItemsIncFees = Profile.Basket.BasketItems.Count.ToString()

        _requestFields = New Generic.Dictionary(Of String, String)
        Dim requestFieldsAll As String = Utilities.CheckForDBNull_String(wfr.Attribute("RequestFields")).Trim().ToUpper()
        If requestFieldsAll.Length > 0 Then
            Dim charSeparators() As Char = {";"c}
            Dim arrRequestFields() As String = requestFieldsAll.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries)
            For fieldIndex As Integer = 0 To arrRequestFields.Length - 1
                _requestFields.Add(arrRequestFields(fieldIndex), Utilities.CheckForDBNull_String(wfr.Attribute(arrRequestFields(fieldIndex))).Trim())
            Next
        End If
        'PAYMENTREQUEST_n_DESC
        If Not String.IsNullOrWhiteSpace(wfr.Content("OrderSummaryDescription", _languageCode, True)) Then
            _requestFields.Add("PAYMENTREQUEST_0_DESC", wfr.Content("OrderSummaryDescription", _languageCode, True))
        End If
        'PAYMENTREQUEST_n_ITEMAMT
        _requestFields.Add("PAYMENTREQUEST_0_AMT", _basketTotal)
        _requestFields.Add("PAYMENTREQUEST_0_INVNUM", Profile.Basket.Temp_Order_Id)
        _requestFields.Add("MAXAMT", _basketTotal)
        _requestFields.Add("FIRSTNAME", Profile.User.Details.Forename)
        _requestFields.Add("LASTNAME", Profile.User.Details.Surname)
        _requestFields.Add("BUTTONSOURCE", wfr.Attribute("ButtonSourceValue"))
    End Sub

    ''' <summary>
    ''' Add the basket item details to the request fields. The PayPal items request shouldn't exceed 10 lines, so will stop at 10 items. 
    ''' L_PAYMENTREQUEST_0_NAME string format: [event name], [event venue], [venue location], [DD/MM/YY], [hh:mm], [first name] [last name]
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setLineItemRequestFields()
        Dim index As Integer = 0
        For Each item As TalentBasketItem In Profile.Basket.BasketItems
            Dim paymentRequestNameString As New StringBuilder
            Dim commaSpace As String = ", "
            If item.MODULE_ = GlobalConstants.BASKETMODULETICKETING Then
                If Utilities.IsTicketingFee(item.MODULE_, item.Product, item.FEE_CATEGORY) Then
                    paymentRequestNameString.Append(item.PRODUCT_DESCRIPTION1.Trim())
                Else
                    Dim productLocation As String = String.Empty
                    Dim productStadium As String = String.Empty
                    Dim productDate As String = String.Empty
                    Dim productTime As String = String.Empty
                    getProductSpecificDetails(item.Product, item.PRODUCT_TYPE_ACTUAL, productLocation, productStadium, productDate, productTime)
                    paymentRequestNameString.Append(item.PRODUCT_DESCRIPTION1.Trim())
                    If productLocation.Trim().Length > 0 Then
                        paymentRequestNameString.Append(commaSpace)
                        paymentRequestNameString.Append(productLocation.Trim())
                    End If
                    If productStadium.Trim().Length > 0 Then
                        paymentRequestNameString.Append(commaSpace)
                        paymentRequestNameString.Append(productStadium.Trim())
                    End If
                    If productDate.Trim().Length > 0 Then
                        paymentRequestNameString.Append(commaSpace)
                        paymentRequestNameString.Append(productDate.Trim())
                    End If
                    If productTime.Trim().Length > 0 Then
                        paymentRequestNameString.Append(commaSpace)
                        paymentRequestNameString.Append(productTime.Trim())
                    End If
                End If
            Else
                paymentRequestNameString.Append(item.PRODUCT_DESCRIPTION1.Trim())
            End If
            If Not Profile.IsAnonymous Then
                paymentRequestNameString.Append(commaSpace).Append(Profile.User.Details.Full_Name)
            End If
            _requestFields.Add("L_PAYMENTREQUEST_0_NAME" & index.ToString(), paymentRequestNameString.ToString())
            _requestFields.Add("L_PAYMENTREQUEST_0_AMT" & index.ToString(), item.Gross_Price.ToString("#####0.00"))
            _requestFields.Add("L_PAYMENTREQUEST_0_QTY" & index.ToString(), CInt(item.Quantity))
            _requestFields.Add("L_PAYMENTREQUEST_0_DESC" & index.ToString(), item.PRODUCT_DESCRIPTION2)
            _requestFields.Add("L_PAYMENTREQUEST_0_NUMBER" & index.ToString(), item.Product)
            index += 1
        Next
    End Sub

    Private Sub ProcessPayPalPayment()
        _talentLogging.GeneralLog(wfr.PageCode, "ProcessPayPal", "Customer: " & Profile.User.Details.LoginID, "PayPalLogging")

        'Decide Stages
        If String.IsNullOrWhiteSpace(Session("PayPalStage")) Then
            ProcessError("PPGERR-002")
        Else
            Dim PPToken As String = String.Empty
            Dim shipToName As String = String.Empty
            Dim shipToStreet As String = String.Empty
            Dim shipToStreet2 As String = String.Empty
            Dim shipToCity As String = String.Empty
            Dim shipToState As String = String.Empty
            Dim shipToZip As String = String.Empty
            Dim shipToCountryCode As String = String.Empty
            Dim noShippingFlag As String = String.Empty
            Dim addrOverride As String = String.Empty
            Dim payerID As String = String.Empty
            Dim returnMessage As String = String.Empty
            Dim payPalCaller As PayPal = Nothing
            Dim payPalErrorCode As String = String.Empty
            Try
                payPalCaller = New PayPal(_isSandbox, _httpWebReqTimeOut, _endURLLive, _endURLSandbox,
                                       _hostURLLive, _hostURLSandbox, _returnURL, _cancelURL)
                payPalCaller.SetCredentials(_APIUsername, _APIPassword, _APISignature, _subject)
                payPalCaller.RequestFields = _requestFields
            Catch ex As Exception
                ProcessError("PPGERR-003", ex)
            End Try
            Dim paypalStage As String = Session("PayPalStage").ToString()

            Select Case paypalStage
                Case STAGE1
                    Session("BasketValue") = _basketTotal
                    Dim isPayPalInitialised As Boolean = False
                    Try
                        setShippingValues(shipToName, shipToStreet, shipToStreet2, shipToCity, shipToState, shipToZip, shipToCountryCode, noShippingFlag, addrOverride)
                        isPayPalInitialised = payPalCaller.MarkExpressCheckout(_basketTotal, shipToName, shipToStreet, shipToStreet2, shipToCity, shipToState, shipToZip, shipToCountryCode, noShippingFlag, addrOverride, PPToken, returnMessage, payPalErrorCode)
                    Catch ex As Exception
                        ProcessError("PPGERR-004", ex)
                    End Try
                    If isPayPalInitialised Then
                        Try
                            If PPToken.Length > 0 Then
                                UpdatePaymentIntermediateStatus("PAYMENT INTERMEDIATE 1", PPToken, "Paypal Token : " & PPToken & "; Basket Header Id : " & _basketHeaderID & "; Basket Total : " & _basketTotal & "; Basket Items Including Fees : " & _basketTotalItemsIncFees)
                            Else
                                ProcessError("PPERR-001-A", "Received an empty paypal token")
                            End If
                        Catch ex As Exception
                            ProcessError("PPGERR-007", ex)
                        End Try
                        Session("PayPalStage") = "2"
                        Session("PPToken") = PPToken
                        Response.Redirect(returnMessage)
                    Else
                        ProcessError("PPERR-001", "Not a valid PayPal Payer Authentication : " & returnMessage, payPalErrorCode)
                    End If
                Case STAGE2
                    If ((String.IsNullOrWhiteSpace(Request.QueryString("token"))) OrElse (String.IsNullOrWhiteSpace(Request.QueryString("payerid")))) Then
                        ProcessError("PPERR-002", "Token or PayerId is missing. Not a valid session. Please try again.")
                    ElseIf Request.QueryString("token") <> Session("PPToken").ToString() Then
                        ProcessError("PPERR-003", "Tokens are not matching. Not a valid session. Please try again.")
                    Else
                        payerID = Request.QueryString("payerid")
                        PPToken = Session("PPToken")
                        Dim deCoder As New NVPCodec()
                        Dim confirmPay As Boolean = False
                        Dim doRedirect As Boolean = False
                        Try
                            UpdatePaymentIntermediateStatus("PAYMENT INTERMEDIATE 2", PPToken, "Call to ConfirmPayment Starts; Paypal Token : " & PPToken & "; Basket Header Id : " & _basketHeaderID & "; Basket Total : " & _basketTotal & "; Basket Items Including Fees : " & _basketTotalItemsIncFees)
                            confirmPay = payPalCaller.ConfirmPayment(Session("BasketValue").ToString(), PPToken, payerID, deCoder, returnMessage, doRedirect)
                            UpdatePaymentIntermediateStatus("PAYMENT INTERMEDIATE 3", PPToken, "Call to ConfirmPayment Ends; Paypal Token : " & PPToken & "; PayerId : " & payerID & "; Basket Header Id : " & _basketHeaderID)
                        Catch ex As Exception
                            confirmPay = False
                            ProcessError("PPGERR-005", ex)
                        End Try

                        If confirmPay Then
                            If deCoder("PAYMENTINFO_0_PAYMENTSTATUS").ToUpper() = PPPaySuccessStatus Then
                                Session("ExtPaymentReferenceNo") = deCoder("PAYMENTINFO_0_TRANSACTIONID")
                                Try
                                    UpdateExternalPaymentTokenInBasket("")
                                Catch ex As Exception

                                End Try
                                ProcessSuccessPayment("Payment Success : " & GetDecoderString(deCoder))
                            Else
                                ProcessFailurePayment("Payment Failure : " & GetDecoderString(deCoder), False)
                            End If
                        Else
                            If doRedirect Then
                                Response.Redirect(returnMessage)
                            Else
                                ProcessError("PPERR-004", returnMessage)
                            End If
                        End If
                    End If
                Case Else
                    ProcessError("PPGERR-006")
            End Select
        End If
    End Sub

    ''' <summary>
    ''' Process the refund of the PayPal transaction
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub ProcessPayPalRefund()
        _talentLogging.GeneralLog(wfr.PageCode, "ProcessPayPalRefund", "Customer: " & Profile.User.Details.LoginID, "PayPalLogging")
        Dim payPalCaller As PayPal = Nothing
        Dim payPalTransactionId As String = Profile.Basket.EXTERNAL_PAYMENT_TOKEN
        Dim paymentReference As String = CATHelper.GetCATPaymentReference()
        Dim priceListHeaderTableAdapter As New TalentBasketDatasetTableAdapters.tbl_price_list_headerTableAdapter
        Dim priceListHeaderTable As Data.DataTable = priceListHeaderTableAdapter.GetPriceListHeaderByPriceList(_defaultsValue.PriceList)
        Dim currenciesTableAdapter As New TalentApplicationVariablesTableAdapters.tbl_currencyTableAdapter
        Dim currenciesTable As Data.DataTable = currenciesTableAdapter.GetDataByCurrencyCode(priceListHeaderTable.Rows(0)("CURRENCY_CODE").ToString)
        Dim currencyCode As String = String.Empty
        If currenciesTable.Rows.Count > 0 AndAlso Talent.eCommerce.Utilities.CheckForDBNull_String(currenciesTable.Rows(0)("CURRENCY_CODE").ToString) <> String.Empty Then
            currencyCode = currenciesTable.Rows(0)("CURRENCY_CODE").ToString
        End If
        Dim deCoder As New NVPCodec()
        Dim returnMessage As String = String.Empty
        Dim hasTransactionRefunded As Boolean = False
        Dim payPalRefundedTransactionID As String = String.Empty

        Try
            payPalCaller = New PayPal(_isSandbox, _httpWebReqTimeOut, _endURLLive, _endURLSandbox, _hostURLLive, _hostURLSandbox, _returnURL, _cancelURL)
            payPalCaller.SetCredentials(_APIUsername, _APIPassword, _APISignature, _subject)
        Catch ex As Exception
            ProcessError("PPGERR-003", ex)
        End Try
        Try
            Session("BasketValue") = _basketTotal
            UpdatePaymentIntermediateStatus("ATTEMPT REFUND PAYMENT", payPalTransactionId, "Paypal Transaction ID : " & payPalTransactionId & "; Basket Header Id : " & _basketHeaderID & "; Basket Total : " & _basketTotal & "; Basket Items Including Fees : " & _basketTotalItemsIncFees)
            hasTransactionRefunded = payPalCaller.RefundTransaction(payPalTransactionId, _basketHeaderID, paymentReference, _basketTotal, currencyCode, deCoder, returnMessage, payPalRefundedTransactionID)
        Catch ex As Exception
            hasTransactionRefunded = False
            ProcessError("PPGERR-008", ex)
        End Try

        If hasTransactionRefunded Then
            UpdatePaymentIntermediateStatus("REFUND COMPLETED", payPalRefundedTransactionID, "Refund Transaction ID : " & payPalRefundedTransactionID & "; Basket Header Id : " & _basketHeaderID & "; Basket Total : " & _basketTotal & "; Basket Items Including Fees : " & _basketTotalItemsIncFees)
            Session("ExtPaymentReferenceNo") = payPalRefundedTransactionID
            UpdateExternalPaymentTokenInBasket(String.Empty)
            ProcessSuccessRefund(returnMessage)
        Else
            ProcessError("PPGERR-009", returnMessage)
            UpdatePaymentIntermediateStatus("REFUND FAILED", payPalRefundedTransactionID, "Refund Transaction ID : " & payPalRefundedTransactionID & "; Basket Header Id : " & _basketHeaderID & "; Basket Total : " & _basketTotal & "; Basket Items Including Fees : " & _basketTotalItemsIncFees)
            UpdateExternalPaymentTokenInBasket(String.Empty)
            ProcessFailureRefund(returnMessage)
        End If
    End Sub

    Private Sub ProcessSuccessPayment(ByVal PPResponse As String)
        Dim redirectURL As String = String.Empty
        Checkout.UpdateOrderStatusForExternalPay("PAYMENT ACCEPTED", PPResponse)
        Session("ExtPaymentAmount") = Session("BasketValue").ToString()
        Select Case Talent.eCommerce.Utilities.BasketContentTypeWithOverride
            Case "T", "C"
                redirectURL = "~/Redirect/TicketingGateway.aspx?page=checkoutpaymentdetails.aspx&function=paymentexternalsuccess"
                RemoveSessions()
                Response.Redirect(redirectURL)
            Case Else
                Dim order As New Order()
                order.ProcessMerchandiseInBackend(False, False, String.Empty, False)
                redirectURL = "~/PagesLogin/Checkout/CheckoutOrderConfirmation.aspx"
                redirectURL = redirectURL & "?PaymentRef=" & Session("ExtPaymentReferenceNo").ToString()
                redirectURL = redirectURL & "&paymentType=" & Session("ExternalGatewayPayType").ToString()
                redirectURL = redirectURL & "&paymentAmount=" & Session("BasketValue").ToString()
                RemoveSessions()
                Response.Redirect(redirectURL)
        End Select
    End Sub

    ''' <summary>
    ''' Process the failure of the request to make payment
    ''' </summary>
    ''' <param name="PPResponse">The formatted paypal response message for logging</param>
    ''' <param name="isCancelCall">Is this a cancellation request?</param>
    ''' <param name="payPalErrorCodeForDisplay">PayPal error code for a customer facing error. Eg. Address Details are incorrect</param>
    ''' <remarks></remarks>
    Private Sub ProcessFailurePayment(ByVal PPResponse As String, ByVal isCancelCall As Boolean, Optional ByVal payPalErrorCodeForDisplay As String = "")
        Dim redirectURL As String = String.Empty
        _talentLogging.GeneralLog("PayPalGateway.aspx", "ProcessFailurePayment", PPResponse, "PayPalErrorLog")
        Checkout.UpdateOrderStatusForExternalPay("PAYMENT REJECTED", PPResponse)
        RemoveSessions()
        If isCancelCall Then

        Else
            Dim errorCode As String = "PPERR"
            Dim errMsg As New Talent.Common.TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString())
            Dim getDefaultErrorMessage As Boolean = False
            If payPalErrorCodeForDisplay.Length > 0 Then
                Session("GatewayErrorMessage") = errMsg.GetErrorMessage("PayPalErrorCode", Talent.eCommerce.Utilities.GetCurrentPageName, payPalErrorCodeForDisplay, False).ERROR_MESSAGE
                If String.IsNullOrEmpty(Session("GatewayErrorMessage")) Then
                    getDefaultErrorMessage = True
                End If
            Else
                getDefaultErrorMessage = True
            End If
            If getDefaultErrorMessage Then
                Session("GatewayErrorMessage") = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, errorCode).ERROR_MESSAGE
            End If
        End If
        Select Case Talent.eCommerce.Utilities.BasketContentTypeWithOverride
            Case "T", "C"
                redirectURL = "~/Redirect/TicketingGateway.aspx?name=PAYPAL&page=checkoutpaymentdetails.aspx&function=paymentexternalfailure"
                Response.Redirect(redirectURL)
            Case Else
                redirectURL = "~/PagesLogin/Checkout/checkout.aspx"
                Response.Redirect(redirectURL)
        End Select
    End Sub

    Private Sub ProcessErrorPayment(ByVal errorCode As String, ByVal errorStatus As String)
        Dim loginId As String = ""
        Dim errorMsg As String = String.Empty
        If Not Profile.IsAnonymous Then
            loginId = Profile.UserName
        End If
        errorMsg = errorMsg & "Requested Url : " & HttpContext.Current.Request.Url.AbsoluteUri
        Try
            errorMsg = errorMsg & "; Basket Header Id : " & _basketHeaderID & "; Basket Total : " & _basketTotal & "; Basket Items Including Fees : " & _basketTotalItemsIncFees
            errorMsg = errorMsg & "; Temp Order Id : " & Profile.Basket.TempOrderID
        Catch ex As Exception
            errorMsg = errorMsg & ex.Message
        End Try
        Try
            Dim talentLogging As New Talent.Common.TalentLogging
            talentLogging.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            talentLogging.GeneralLog("PayPalGateway.aspx", errorCode, errorStatus & " : " & errorMsg, "PayPalErrorLog")
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Process the PayPal refund and forward onto the correct confirmation page
    ''' </summary>
    ''' <param name="PPResponse">The PayPal response string</param>
    ''' <remarks></remarks>
    Private Sub ProcessSuccessRefund(ByVal PPResponse As String)
        Dim redirectURL As String = String.Empty
        _talentLogging.GeneralLog("PayPalGateway.aspx", "ProcessSuccessRefund", PPResponse, "PayPalErrorLog")
        Checkout.UpdateOrderStatusForExternalPay("REFUND ACCEPTED", PPResponse)
        Session("ExtPaymentAmount") = Session("BasketValue").ToString()
        Select Case Talent.eCommerce.Utilities.BasketContentTypeWithOverride
            Case "T", "C"
                redirectURL = "~/Redirect/TicketingGateway.aspx?page=checkoutpaymentdetails.aspx&function=paymentexternalsuccess"
                RemoveSessions()
                Response.Redirect(redirectURL)
            Case Else
                Dim order As New Order()
                order.ProcessMerchandiseInBackend(False, False, String.Empty, False)
                redirectURL = "~/PagesLogin/Checkout/CheckoutOrderConfirmation.aspx"
                redirectURL = redirectURL & "?PaymentRef=" & Session("ExtPaymentReferenceNo").ToString()
                redirectURL = redirectURL & "&paymentType=" & Session("ExternalGatewayPayType").ToString()
                redirectURL = redirectURL & "&paymentAmount=" & Session("BasketValue").ToString()
                RemoveSessions()
                Response.Redirect(redirectURL)
        End Select
    End Sub

    ''' <summary>
    ''' Process the PayPal refund when it has failed and forward onto the correct page
    ''' </summary>
    ''' <param name="PPResponse">The PayPal response string</param>
    ''' <remarks></remarks>
    Private Sub ProcessFailureRefund(ByVal PPResponse As String)
        Dim redirectURL As String = String.Empty
        Dim errorCode As String = "PPERR"
        Dim errMsg As New Talent.Common.TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)

        _talentLogging.GeneralLog("PayPalGateway.aspx", "ProcessFailurePayment", PPResponse, "PayPalErrorLog")
        Checkout.UpdateOrderStatusForExternalPay("REFUND REJECTED", PPResponse)
        RemoveSessions()
        Session("GatewayErrorMessage") = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, errorCode).ERROR_MESSAGE

        Select Case Talent.eCommerce.Utilities.BasketContentTypeWithOverride
            Case "T", "C"
                redirectURL = "~/Redirect/TicketingGateway.aspx?name=PAYPAL&page=checkoutpaymentdetails.aspx&function=paymentexternalfailure"
                Response.Redirect(redirectURL)
            Case Else
                redirectURL = "~/PagesLogin/Checkout/checkout.aspx"
                Response.Redirect(redirectURL)
        End Select
    End Sub

    ''' <summary>
    ''' Process the error and call the handler method based on the exception type
    ''' </summary>
    ''' <param name="errorCode">The error code string</param>
    ''' <param name="ex">The exception object</param>
    ''' <remarks></remarks>
    Private Sub ProcessError(ByVal errorCode As String, ByVal ex As Exception)
        Select Case errorCode
            Case "PPGERR-003" 'PayPal API creation exception
                ProcessFailurePayment("PPGERR-003 : PayPal API initiate exception : " & ex.Message, False)
            Case "PPGERR-004" 'Token creation exception
                ProcessFailurePayment("PPGERR-004 : PayPal API Token Creation exception : " & ex.Message, False)
            Case "PPGERR-005" 'Confirm Payment exception
                ProcessFailurePayment("PPGERR-005 : PayPal API Pay Confirmation exception : " & ex.Message, False)
            Case "PPGERR-007" 'Basket header updated with payment token exception
                ProcessFailurePayment("PPGERR-007 : PayPal API Token to Basket Header Update exception : " & ex.Message, False)
            Case "PPGERR-008" 'Refund payment exception
                ProcessFailureRefund("PPGERR-008 : PayPal refund exception : " & ex.Message)
        End Select
    End Sub

    ''' <summary>
    ''' Process the error and call the handler method based on the error code
    ''' </summary>
    ''' <param name="errorCode">The error code string</param>
    ''' <param name="errorMessage">The error message text</param>
    ''' <remarks></remarks>
    Private Sub ProcessError(ByVal errorCode As String, ByVal errorMessage As String, Optional ByVal payPalErrorCode As String = "")
        Select Case errorCode
            Case "PPERR-001" 'PayPal error for Token creation
                ProcessFailurePayment("PPERR-001 : PayPal Error On Token Creation : " & errorMessage, False, payPalErrorCode)
            Case "PPERR-001-A" 'PayPal error for Token creation
                ProcessFailurePayment("PPERR-001-A : PayPal Error On Token Creation : " & errorMessage, False)
            Case "PPERR-002" 'Token and PayerId is missing in PayPal return
                ProcessFailurePayment("PPERR-002 : PayPal Querystring Missing : " & errorMessage, False)
            Case "PPERR-003" 'Token mismatch
                ProcessFailurePayment("PPERR-003 : PayPal Created and Received Token Mismatch : " & errorMessage, False)
            Case "PPERR-004" 'Confirm Payment error
                ProcessFailurePayment("PPERR-004 : PayPal Error on Pay Confirm : " & errorMessage, False)
            Case "PPERR-009" 'Refund Payment error
                ProcessFailureRefund("PPERR-009 : PayPal Error on refund : " & errorMessage)
        End Select
    End Sub

    Private Sub ProcessError(ByVal errorCode As String)
        Select Case errorCode
            Case "PPGERR-001" 'Profile is anonymous
                ProcessErrorPayment(errorCode, "PROFILE IS ANONYMOUS")
                Response.Redirect("~/PagesPublic/Login/login.aspx")
            Case "PPGERR-002" 'Logged in but tries to access gateway directly / PayPal Stage session is missing or not yet created
                ProcessErrorPayment(errorCode, "SESSION STAGE ISSUE")
                Response.Redirect("~/PagesPublic/Error/ExternalGatewayError.aspx?gatewayname=PAYPAL")
            Case "PPGERR-006" 'PayPal Stage (.Net) Session is not valid one
                'Redirect to new Error Page with Querystring of External Gateway Name
                ProcessErrorPayment(errorCode, "SESSION STAGE MISMATCH")
                Response.Redirect("~/PagesPublic/Error/ExternalGatewayError.aspx?gatewayname=PAYPAL")
        End Select
    End Sub

    Private Function GetDecoderString(ByVal deCoder As NVPCodec) As String
        Dim sbDeCoder As New StringBuilder
        sbDeCoder.Append("&TOKEN = " & deCoder("TOKEN"))
        sbDeCoder.Append("&TIMESTAMP = " & deCoder("TIMESTAMP"))
        sbDeCoder.Append("&ACK = " & deCoder("ACK"))
        sbDeCoder.Append("&PAYMENTINFO_0_TRANSACTIONID = " & deCoder("PAYMENTINFO_0_TRANSACTIONID"))
        sbDeCoder.Append("&PAYMENTINFO_0_TRANSACTIONTYPE = " & deCoder("PAYMENTINFO_0_TRANSACTIONTYPE"))
        sbDeCoder.Append("&PAYMENTINFO_0_PAYMENTTYPE = " & deCoder("PAYMENTINFO_0_PAYMENTTYPE"))
        sbDeCoder.Append("&PAYMENTINFO_0_ORDERTIME = " & deCoder("PAYMENTINFO_0_ORDERTIME"))
        sbDeCoder.Append("&PAYMENTINFO_0_AMT = " & deCoder("PAYMENTINFO_0_AMT"))
        sbDeCoder.Append("&PAYMENTINFO_0_CURRENCYCODE = " & deCoder("PAYMENTINFO_0_CURRENCYCODE"))
        sbDeCoder.Append("&PAYMENTINFO_0_EXCHANGERATE = " & deCoder("PAYMENTINFO_0_EXCHANGERATE"))
        sbDeCoder.Append("&PAYMENTINFO_0_PAYMENTSTATUS = " & deCoder("PAYMENTINFO_0_PAYMENTSTATUS"))
        sbDeCoder.Append("&PAYMENTINFO_0_PENDINGREASON = " & deCoder("PAYMENTINFO_0_PENDINGREASON"))
        sbDeCoder.Append("&PAYMENTINFO_0_REASONCODE = " & deCoder("PAYMENTINFO_0_REASONCODE"))
        sbDeCoder.Append("&PAYMENTINFO_0_PROTECTIONELIGIBILITY = " & deCoder("PAYMENTINFO_0_PROTECTIONELIGIBILITY"))
        sbDeCoder.Append("&PAYMENTINFO_0_ERRORCODE = " & deCoder("PAYMENTINFO_0_ERRORCODE"))
        sbDeCoder.Append("&PAYMENTINFO_0_ACK = " & deCoder("PAYMENTINFO_0_ACK"))
        Return sbDeCoder.ToString()
    End Function

    Private Sub RemoveSessions()
        Session.Remove("PPToken")
        Session.Remove("BasketValue")
        Session.Remove("ExternalGatewayURL")
        Session.Remove("PayPalStage")
    End Sub

    ''' <summary>
    ''' Return call from PayPal as user cancelled the payment process
    ''' </summary>
    Private Function PayPalCancelCall() As Boolean
        Dim isCancelCall As Boolean = False
        If (Not String.IsNullOrWhiteSpace(Request.QueryString("PayPalCancelCall"))) AndAlso (Request.QueryString("PayPalCancelCall") = "CANCEL") Then
            If (Not String.IsNullOrWhiteSpace(Session("PPToken"))) Then
                isCancelCall = True
                Session("GatewayErrorMessage") = _cancelMessageForUser
                Try
                    UpdateExternalPaymentTokenInBasket("")
                    Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
                Catch ex As Exception

                End Try
                ProcessFailurePayment("User Cancelled the PayPal Payment Process : Token ID : " & Session("PPToken").ToString(), isCancelCall)
            End If
        End If
        _talentLogging.GeneralLog(wfr.PageCode, "PayPalCancelCall", "Customer: " & Profile.User.Details.LoginID & ", isCancelCall=" & isCancelCall.ToString(), "PayPalLogging")
        Return isCancelCall
    End Function

    Private Sub UpdateExternalPaymentTokenInBasket(ByVal externalPayToken As String)
        Dim isUpdated As Boolean = False
        Dim basketHeader As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
        basketHeader.Update_External_Payment_Token(externalPayToken, Profile.Basket.Basket_Header_ID)
        basketHeader = Nothing
    End Sub

    Private Sub UpdatePaymentIntermediateStatus(ByVal statusText As String, ByVal externalPayToken As String, ByVal statusMessage As String)
        UpdateExternalPaymentTokenInBasket(externalPayToken)
        Checkout.UpdateOrderStatusForExternalPay(statusText, statusMessage)
    End Sub

    ''' <summary>
    ''' Setup the PayPal shipping details based on the fulfilment type the user has selected.
    ''' C - PayPal shipping details are the club address
    ''' P or R - PayPal shipping details are the customer address they have selected from the checkout page
    ''' 0, H, P - PayPal shipping details are not set since nothing is shipped.
    ''' </summary>
    ''' <param name="shipToName">Customer Name. 128 character limit</param>
    ''' <param name="shipToStreet">Address Line 1. 100 character limit</param>
    ''' <param name="shipToStreet2">Address Line 2. 100 character limit</param>
    ''' <param name="shipToCity">Address Line 3 (City). 40 character limit</param>
    ''' <param name="shipToState">Address Line 4 (County). 40 character limit</param>
    ''' <param name="shipToZip">Address PostCode. 20 character limit</param>
    ''' <param name="shipToCountryCode">Address Country Code, eg. UK, US, IT. 2 character limit</param>
    ''' <param name="noShippingFlag">
    ''' 0 — PayPal displays the shipping address on the PayPal pages.
    ''' 1 — PayPal does not display shipping address fields and removes shipping information from the transaction.
    ''' 2 — If you do not pass the shipping address, PayPal obtains it from the buyer's account profile.
    ''' </param>
    ''' <param name="addrOverride">
    ''' 0 — The PayPal pages should not display the shipping address.
    ''' 1 — The PayPal pages should display the shipping address.
    ''' </param>
    ''' <remarks></remarks>
    Private Sub setShippingValues(ByRef shipToName As String, ByRef shipToStreet As String, ByRef shipToStreet2 As String, ByRef shipToCity As String, ByRef shipToState As String, ByRef shipToZip As String, ByRef shipToCountryCode As String, ByRef noShippingFlag As String, ByRef addrOverride As String)
        Try
            Dim fulfilmentType As String = getFulfilmentTypeForEntireBasket()
            Select Case fulfilmentType
                Case Is = GlobalConstants.COLLECT_FULFILMENT
                    Dim ticketOfficeAddress() As String = wfr.Attribute("AddressForTicketCollection").Split(";")
                    shipToName = wfr.Attribute("S2SValue")
                    shipToStreet = ticketOfficeAddress(0)
                    shipToStreet2 = ticketOfficeAddress(1)
                    shipToCity = ticketOfficeAddress(2)
                    shipToState = ticketOfficeAddress(3)
                    shipToZip = ticketOfficeAddress(4)
                    shipToCountryCode = ticketOfficeAddress(5)
                    noShippingFlag = "0"
                    addrOverride = "1"
                Case Is = GlobalConstants.POST_FULFILMENT
                    Dim deliveryDetails As New Talent.Common.DEDeliveryDetails
                    If Session("StoredDeliveryAddress") IsNot Nothing Then
                        deliveryDetails = Session("StoredDeliveryAddress")
                        shipToName = Profile.User.Details.Full_Name
                        shipToStreet = deliveryDetails.Address2
                        shipToStreet2 = deliveryDetails.Address3
                        shipToCity = deliveryDetails.Address4
                        shipToState = deliveryDetails.Address5
                        shipToZip = deliveryDetails.Postcode
                        shipToCountryCode = deliveryDetails.CountryCode
                    Else
                        shipToName = Profile.User.Details.Full_Name
                        shipToStreet = Profile.User.Addresses(0).Address_Line_2
                        shipToStreet2 = Profile.User.Addresses(0).Address_Line_3
                        shipToCity = Profile.User.Addresses(0).Address_Line_4
                        shipToState = Profile.User.Addresses(0).Address_Line_5
                        shipToZip = Profile.User.Addresses(0).Post_Code
                        shipToCountryCode = Profile.User.Addresses(0).Country
                    End If
                    noShippingFlag = "0"
                    addrOverride = "1"
                Case Else
                    noShippingFlag = "1"
                    addrOverride = "0"
            End Select
        Catch ex As Exception
            ProcessError("PPGERR-004a", ex)
        End Try
    End Sub

    ''' <summary>
    ''' Retrieve the fulfilment type for the entire basket specific to PayPal.
    ''' If any of the items are post then the customer postage address needs sending to PayPal
    ''' If any of the items are collect and there are no post items then send the club collection address to PayPal
    ''' If none of the items are post or collect then no address details are to be sent to PayPal since nothing is specific to an address
    ''' </summary>
    ''' <returns>The highest priority fulfilment: 1- Post, 2- Collect, 3- anything else</returns>
    ''' <remarks></remarks>
    Private Function getFulfilmentTypeForEntireBasket() As String
        Dim fulfilmentType As String = String.Empty
        Dim basketHasPost As Boolean = False
        Dim basketHasCollect As Boolean = False
        Dim basketHasPrintOrUploadOrPAH As Boolean = False
        For Each item As TalentBasketItem In Profile.Basket.BasketItems
            If item.CURR_FULFIL_SLCTN = GlobalConstants.POST_FULFILMENT OrElse item.CURR_FULFIL_SLCTN = GlobalConstants.REG_POST_FULFILMENT Then
                basketHasPost = True
            ElseIf item.CURR_FULFIL_SLCTN = GlobalConstants.COLLECT_FULFILMENT Then
                basketHasCollect = True
            Else
                basketHasPrintOrUploadOrPAH = True
            End If
        Next
        If basketHasPost Then
            fulfilmentType = GlobalConstants.POST_FULFILMENT
        Else
            If basketHasCollect Then
                fulfilmentType = GlobalConstants.COLLECT_FULFILMENT
            Else
                If basketHasPrintOrUploadOrPAH Then
                    fulfilmentType = GlobalConstants.PRINT_FULFILMENT
                End If
            End If
        End If
        Return fulfilmentType
    End Function

    ''' <summary>
    ''' Get the product specific data for display in PayPal. Format the date and return any information that isn't populated.
    ''' </summary>
    ''' <param name="productCode">The current product code</param>
    ''' <param name="productType">The current product type</param>
    ''' <param name="productLocation">The product location set against the product</param>
    ''' <param name="productStadiumDescription">The product stadium description</param>
    ''' <param name="productDate">The product formatted date</param>
    ''' <param name="productTime">The product time</param>
    ''' <remarks></remarks>
    Private Sub getProductSpecificDetails(ByVal productCode As String, ByVal productType As String, ByRef productLocation As String, ByRef productStadiumDescription As String, ByRef productDate As String, ByRef productTime As String)
        Dim product As New Talent.Common.TalentProduct
        Dim de As New Talent.Common.DEProductDetails
        Dim settings As New Talent.Common.DESettings
        Dim err As New Talent.Common.ErrorObj
        settings = Utilities.GetSettingsObject()
        de.ProductType = productType
        de.PPSType = "*"
        de.Src = GlobalConstants.SOURCE
        product.Settings = settings
        product.De = de
        err = product.ProductList()

        If Not err.HasError AndAlso product.ResultDataSet IsNot Nothing AndAlso product.ResultDataSet.Tables("StatusResults") IsNot Nothing Then
            If product.ResultDataSet.Tables("ProductListResults").Rows.Count > 0 Then
                Dim dtProductList As New Data.DataTable
                dtProductList = product.ResultDataSet.Tables("ProductListResults")
                For Each row In dtProductList.Rows
                    If row("ProductCode").ToString().Trim() = productCode.Trim() Then
                        productLocation = row("location").ToString().Trim()
                        productStadiumDescription = row("ProductStadiumDescription").ToString().Trim()
                        If Not Utilities.CheckForDBNull_Boolean_DefaultFalse(row("HideDate")) Then
                            productDate = row("ProductMDTE08").ToString().Trim()
                            productDate = Utilities.GetFormattedDateAndTime(productDate, String.Empty, String.Empty, "dd/MM/yy", String.Empty)
                        End If
                        If Not Utilities.CheckForDBNull_Boolean_DefaultFalse(row("HideTime")) Then
                            productTime = row("ProductTime").ToString().Trim()
                        End If
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub

#End Region

#Region "Return Params From PayPal"
    'deCoder("TOKEN")
    'deCoder("SUCCESSPAGEREDIRECTREQUESTED")
    'deCoder("TIMESTAMP")
    'deCoder("CORRELATIONID")
    'deCoder("ACK")
    'deCoder("VERSION")
    'deCoder("BUILD")
    'deCoder("INSURANCEOPTIONSELECTED")
    'deCoder("SHIPPINGOPTIONISDEFAULT")
    'deCoder("PAYMENTINFO_0_TRANSACTIONID")
    'deCoder("PAYMENTINFO_0_TRANSACTIONTYPE")
    'deCoder("PAYMENTINFO_0_PAYMENTTYPE")
    'deCoder("PAYMENTINFO_0_ORDERTIME")
    'deCoder("PAYMENTINFO_0_AMT")
    'deCoder("PAYMENTINFO_0_FEEAMT")
    'deCoder("PAYMENTINFO_0_SETTLEAMT")
    'deCoder("PAYMENTINFO_0_TAXAMT")
    'deCoder("PAYMENTINFO_0_CURRENCYCODE")
    'deCoder("PAYMENTINFO_0_EXCHANGERATE")
    'deCoder("PAYMENTINFO_0_PAYMENTSTATUS")
    'deCoder("PAYMENTINFO_0_PENDINGREASON")
    'deCoder("PAYMENTINFO_0_REASONCODE")
    'deCoder("PAYMENTINFO_0_PROTECTIONELIGIBILITY")
    'deCoder("PAYMENTINFO_0_ERRORCODE")
    'deCoder("PAYMENTINFO_0_ACK")
#End Region

End Class
