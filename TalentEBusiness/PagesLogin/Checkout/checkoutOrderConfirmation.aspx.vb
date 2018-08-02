Imports Microsoft.VisualBasic
Imports System.Data
Imports Talent.eCommerce
Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Collections.Generic
Imports System.Linq

Partial Class PagesLogin_checkoutOrderConfirmation
    Inherits TalentBase01

    Public discountIF As Boolean
#Region "Class Level Fields"
    Private wfr As New Talent.Common.WebFormResource
    Private _businessUnit As String = String.Empty
    Private _partner As String = String.Empty
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _orderNeedsReview As Boolean = False

    Private _hsbcRequest As CardProcessing.HSBC.HSBCApiRequest
    Private _expiryDate As String = String.Empty
    Private _startDate As String = String.Empty
    Private _issueNumber As String = String.Empty
    Private _cv2Number As String = String.Empty
    Private _cardNumber As String = String.Empty

    Private _merchandiseOrderValue As Decimal = 0
    Private _merchandiseOrderReference As String = String.Empty
    Private _merchandiseBackOfficeOrdRef As String = String.Empty

    Dim defaults As New Talent.eCommerce.ECommerceModuleDefaults
    Dim def As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues

    Dim preSharedKey As String
#End Region

#Region "Public Methods"
    Public Function DiscountIFValues(ByVal intMode As Integer) As String
        'Provide the following order information
        Dim payment_reference As String = Request.QueryString("PaymentRef") 'Assign the order number
        Dim transaction_value As String = Request.QueryString("PaymentAmount") 'Assign the order amount
        Dim preshared_key As String = preSharedKey  'DiscountIF Preshared-Key
        'Generate the authenticity token
        Dim sReturnValue As String = ""

        Select Case intMode
            Case Is = 0 : sReturnValue = payment_reference
            Case Is = 1 : sReturnValue = transaction_value
            Case Is = 2 : sReturnValue = DiscountIFHash(payment_reference, transaction_value, preshared_key)
        End Select
        Return sReturnValue
    End Function

    Private Function DiscountIFHash(payment_reference As String, transaction_value As String, preshared_key As String) As String
        Dim sRet As String = ""

        Dim objMD5 As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim arrData() As Byte
        Dim arrHash() As Byte

        Dim sStringToHash = payment_reference + "-" + transaction_value + "-" + preshared_key

        arrData = Text.Encoding.UTF8.GetBytes(sStringToHash)
        arrHash = objMD5.ComputeHash(arrData)
        objMD5 = Nothing

        Dim strOutput As New System.Text.StringBuilder(arrHash.Length)
        For i As Integer = 0 To arrHash.Length - 1
            strOutput.Append(arrHash(i).ToString("X2"))
        Next

        sRet = strOutput.ToString().ToLower
        Return sRet
    End Function
#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Session("CheckoutExternalStarted") IsNot Nothing Then
            Session.Remove("CheckoutExternalStarted")
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        PayPalReferenceLabel.Visible = False
        _businessUnit = TalentCache.GetBusinessUnit()
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        def = defaults.GetDefaults
        Logging.WriteLog(Profile.UserName, "COC-001", Profile.UserName & " - Entering Checkout Confirmation", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
        With wfr
            .BusinessUnit = _businessUnit
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = _partner
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "CheckoutOrderConfirmation.aspx"
        End With

        ' Only show order confirmation scan if system default set to scan and if fulfilment method is at order level and is print now 
        uscDespatchProcess.Visible = False
        If ModuleDefaults.UseScanNowForPrintNowFulfilment AndAlso Profile.Basket.CAT_MODE <> GlobalConstants.CATMODE_CANCEL Then
            If Not String.IsNullOrEmpty(Request.QueryString("OrderLevelFulfilmentMethod")) AndAlso Request.QueryString("OrderLevelFulfilmentMethod") = "N" Then '  
                uscDespatchProcess.Visible = True
                uscDespatchProcess.PaymentReference = Request.QueryString("PaymentRef")
            End If
        End If

        If Not Page.IsPostBack Then
            If Session("ExternalGatewayPayType") IsNot Nothing AndAlso Session("ExtPaymentReferenceNo") IsNot Nothing Then
                ProcessExternalPayCheckout()
            End If
            Select Case Profile.Basket.BasketContentType
                Case "C"
                    ProcessMerchandiseCheckout()
                    ProcessTicketingCheckout()
                Case "T"
                    ProcessTicketingCheckout()
                Case "M"
                    ProcessMerchandiseCheckout()
            End Select
        End If

        Try
            Dim miniBasket As Control = Talent.eCommerce.Utilities.FindWebControl("MiniBasket1", Page.Controls)
            If Not miniBasket Is Nothing Then
                CallByName(miniBasket, "ReBindBasket", CallType.Method)
            End If
        Catch ex As Exception
        End Try

        Try
            If Not String.IsNullOrEmpty(Request.QueryString("PaymentRef")) AndAlso Not String.IsNullOrEmpty(Request.QueryString("paymentType")) Then
                If Request.QueryString("paymentType").ToString = GlobalConstants.DDPAYMENTTYPE Then
                    Me.DirectDebitSummary1.PaymentRef = Request.QueryString("PaymentRef").ToString.Trim
                ElseIf Request.QueryString("paymentType").ToString = GlobalConstants.CFPAYMENTTYPE Then
                    CreditFinanceSummary1.PaymentRef = Request.QueryString("PaymentRef").ToString.Trim
                Else
                    'if the DD schedule adjusted flag is set to true the schedule needs to be displayed even though the payment type might not be DD
                    If Not String.IsNullOrEmpty(Request.QueryString("DDScheduleAdjusted")) Then
                        If Request.QueryString("DDScheduleAdjusted") = "True" Then
                            Me.DirectDebitSummary1.PaymentRef = Request.QueryString("PaymentRef").ToString.Trim
                        End If
                    End If
                End If
            End If
            If Not String.IsNullOrEmpty(Request.QueryString("paymentType")) Then
                Checkout.StoreCurrentPaymentType(Request.QueryString("paymentType"))
            End If
            DisplayConfirmationMessage()
        Catch ex As Exception

        End Try

        Talent.eCommerce.Utilities.ClearOtherCacheItems()

        If AgentProfile.IsAgent Then
            If ModuleDefaults.NotificationsOnConfirmPage Then
                If Not Profile.IsAnonymous Then
                    NotificationOptions.Visible = True
                    If Not IsPostBack Then NotificationOptions.CATMode = Profile.Basket.CAT_MODE
                End If
                plhConfirm.Visible = True
                btnConfirm.Text = wfr.Content("ConfirmButtonText", _languageCode, True)
            Else
                NotificationOptions.Visible = False
                plhConfirm.Visible = False
            End If
        Else
            NotificationOptions.Visible = False
        End If
        discountIF = def.DiscountIF
        'hdnDiscountIFLink.Value = discountIF
        'If (discountIF) Then
        '    discountIFLink.Attributes.Add("style", "display:block")
        'End If
        preSharedKey = def.DiscountIFPSK
    End Sub

    Protected Sub Page_PreRender1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        SmartcardOrderDetails.Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(wfr.Attribute("SmartCardOrderDetailsVisible"))
    End Sub

    Protected Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        Dim defaults As New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
        def = defaults.GetDefaults

        Session("personalisationTransactions") = Nothing
        Session("CodeFromPromotionsBox") = Nothing
        Session("UserFromPromotionsBox") = Nothing
        Session("AllPrmotionCodesEnteredByUser") = Nothing
        Session("AllowManualIntervention") = Nothing
        Session("PartnerPromotionCode") = Nothing
        Session("ExternalGatewayPayType") = Nothing
        Session("PPS1PaymentComplete") = Nothing
        Session("PPS2PaymentComplete") = Nothing
        Session("basketPPS1List") = Nothing
        Session("basketPPS2List") = Nothing
        Session("VGPaidPPS1") = Nothing
        Session("VGPaidPPS2") = Nothing

        If Not Profile.IsAnonymous AndAlso Profile.User.Details IsNot Nothing Then
            Dim payment As New TalentPayment
            payment.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            payment.De.CustomerNumber = Profile.User.Details.LoginID
            payment.RetrieveMySavedCardsClearCache()
        End If

        If def.LOG_USER_OUT_AFTER_CHECKOUT AndAlso (NotificationOptions IsNot Nothing AndAlso Not NotificationOptions.Visible) Then
            FormsAuthentication.SignOut()
            Talent.eCommerce.Utilities.ClearAllSessions()
        End If

        If def.NOISE_IN_USE AndAlso def.NOISE_SESSION_EXPIRES_AFTER_CHECKOUT Then
            Dim noiseSettings As DESettings = Talent.eCommerce.Utilities.GetSettingsObject
            Dim myNoise As New TalentNoise(noiseSettings, _
                                            Session.SessionID, _
                                            Now, _
                                            Now.AddMinutes(-def.NOISE_THRESHOLD_MINUTES), _
                                            def.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES)
            myNoise.RemoveSpecificNoiseSession()
        End If

    End Sub
    Protected Sub ProcessExternalPayCheckout()
        If Session("ExternalGatewayPayType").ToString() = "PP" Then
            PayPalReferenceLabel.Visible = True
            PayPalReferenceLabel.Text = wfr.Content("PayPalReferenceText", _languageCode, True)
            PayPalReferenceLabel.Text = PayPalReferenceLabel.Text.Replace("[PAYPAL REFERENCE]", Session("ExtPaymentReferenceNo").ToString())
            PayPalReferenceLabel.Text = PayPalReferenceLabel.Text.Replace("[TOTAL_ORDER_VALUE]", _
            TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.CheckForDBNull_Decimal(Request.QueryString("PaymentAmount")), wfr.BusinessUnit, wfr.PartnerCode))
        Else
            PayPalReferenceLabel.Visible = False
        End If
        Session.Remove("ExternalGatewayPayType")
        Session.Remove("ExtPaymentReferenceNo")
        Session.Remove("ExtPaymentAmount")
    End Sub

    Protected Sub ProcessTicketingCheckout()
        If def.TicketingKioskMode Then
            Dim script As New HtmlGenericControl("SCRIPT")
            script.Attributes.Add("type", "text/javascript")
            script.InnerText = "window.external.KFWClose(" & def.KioskCompleteTimeout & ");"
            Me.Page.Header.Controls.Add(script)
        End If

        Logging.WriteLog(Profile.UserName, "COC-400", Profile.UserName & " - Ticketing Checkout", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))

        ' Set basket to processed
        '---------------------------------------
        Try
            Dim basket As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
            For i As Integer = 0 To i = 3
                Try
                    basket.Mark_As_Processed(Profile.Basket.Basket_Header_ID)
                    Exit For
                Catch ex As Exception
                    If i = 3 Then
                        Logging.WriteLog(Profile.UserName, "UCCCPL-020", ex.Message, "Failed to set basket as processed!", "", "", ProfileHelper.GetPageName)
                    Else
                        System.Threading.Thread.Sleep(50)
                    End If
                End Try
            Next
        Catch ex As Exception
        End Try
        If Not String.IsNullOrEmpty(Request.QueryString("paymentType")) AndAlso Request.QueryString("paymentType") <> GlobalConstants.CFPAYMENTTYPE Then
            Checkout.RemovePaymentDetailsFromSession()
        End If
        If Not String.IsNullOrEmpty(Request.QueryString("PaymentRef")) Then
            TDataObjects.BasketSettings.TblBasketHeader.UpdateTicketingPaymentReference(Profile.Basket.Basket_Header_ID, Request.QueryString("PaymentRef").ToString())
        End If
    End Sub

    Protected Sub ProcessMerchandiseCheckout()
        Dim checkout3dSecureError As Boolean
        Dim generatedWebOrderNo As String = String.Empty

        ' If payment details were saved from payment screen then extract here and clear session variable
        ' These will get passed down to backend if ticketing retail environment
        _cardNumber = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)
        _expiryDate = Checkout.RetrievePaymentItemFromSession("ExpiryDate", GlobalConstants.CHECKOUTASPXSTAGE)
        _startDate = Checkout.RetrievePaymentItemFromSession("StartDate", GlobalConstants.CHECKOUTASPXSTAGE)
        _cv2Number = Checkout.RetrievePaymentItemFromSession("CV2Number", GlobalConstants.CHECKOUTASPXSTAGE)
        _issueNumber = Checkout.RetrievePaymentItemFromSession("IssueNumber", GlobalConstants.CHECKOUTASPXSTAGE)
        Checkout.RemovePaymentDetailsFromSession()

        Logging.WriteLog(Profile.UserName, "COC-010", Profile.UserName & " - Not Ticketing Checkout", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
        'TicketingBasketDetails1.Visible = False
        'TicketingSummaryTotals1.Visible = False

        '----------------------------------------
        ' If HSBC then do payment processing here
        '----------------------------------------
        If def.PaymentCallBankAPI AndAlso def.PaymentGatewayType = "HSBC" Then
            Logging.WriteLog(Profile.UserName, "COC-020", Profile.UserName & " - HSBC Bank API Transaction", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
            '-----------------------------------------------
            ' Check not just here to complete a broken order
            '-----------------------------------------------
            If Session("HSBCRequest") Is Nothing Then
                Logging.WriteLog(Profile.UserName, "COC-030", Profile.UserName & " - No HSBC Session Variable", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))

                Dim myProfile As TalentProfile
                myProfile = CType(HttpContext.Current.Profile, TalentProfile)
                Dim tempOrderId As String = myProfile.Basket.TempOrderID

                Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter

                Dim dt As Data.DataTable = orders.Get_Header_By_Temp_Order_Id(tempOrderId)
                '-----------------------------------------------
                ' Just completing broken order - stay in section
                '-----------------------------------------------
                If dt.Rows.Count > 0 AndAlso dt.Rows(0)("STATUS") = Talent.Common.Utilities.GetOrderStatus("ORDER COMPLETE") Then
                    Logging.WriteLog(Profile.UserName, "COC-040", Profile.UserName & " - Payment OK", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                Else
                    '----------------------------------------
                    ' Got here without payment info - go back
                    '----------------------------------------
                    Logging.WriteLog(Profile.UserName, "COC-050", Profile.UserName & " - No Payment Data, Redirecting to Checkout...", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                    Response.Redirect("checkout.aspx")
                End If
            Else
                Logging.WriteLog(Profile.UserName, "COC-060", Profile.UserName & " - HSBC Session Variable Found", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                '----------------------------------------------------------------
                ' Check 3d Secure results. If failed then go back to payment page
                '----------------------------------------------------------------
                _hsbcRequest = CType(Session("HSBCRequest"), CardProcessing.HSBC.HSBCApiRequest)
                If def.PaymentProcess3dSecure Then
                    Logging.WriteLog(Profile.UserName, "COC-070", Profile.UserName & " - Calling 3D Secure", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                    checkout3dSecureError = check3dSecureHSBC()
                Else
                    checkout3dSecureError = False
                End If

                If checkout3dSecureError Then
                    Logging.WriteLog(Profile.UserName, "COC-080", Profile.UserName & " - 3D Secure Error, Redirecting to Checkout...", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                    Session("CardError") = True
                    Insert_Order_Status_Flow("PAYMENT REJECTED", _hsbcRequest.LogText)
                    Session("GatewayErrorMessage") = _hsbcRequest.LogText
                    Response.Redirect("checkout.aspx")
                Else
                    '-------------
                    ' Take payment
                    '-------------
                    Logging.WriteLog(Profile.UserName, "COC-090", Profile.UserName & " - No 3D Secure Error, Taking Payment...", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                    If def.PaymentHSBCPassWebOrderNo Then
                        generatedWebOrderNo = Checkout.GenNewOrderID()
                    End If
                    _hsbcRequest.HSBCOrderID = generatedWebOrderNo
                    _hsbcRequest.buildXml()

                    Session("CardError") = Nothing
                    Session("CardErrorAvs") = Nothing
                    _hsbcRequest.sendXml()
                    '----------------------
                    ' Check payment results
                    '----------------------
                    If _hsbcRequest.isApproved Then
                        Logging.WriteLog(Profile.UserName, "COC-100", Profile.UserName & " - HSBC Payment Taken, Setting amount charged", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                        Session("HSBCRequest") = Nothing
                        Try
                            Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                            Dim dt As Data.DataTable = orders.Get_UNPROCESSED_Order(TalentCache.GetBusinessUnit, Profile.UserName)
                            Dim total As Decimal = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_ORDER_VALUE"))
                            Dim promo As Decimal = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("PROMOTION_VALUE"))

                            If Not def.Call_Tax_WebService Then
                                total = total - promo
                            End If

                            orders.Set_Total_Amount_Charged(Talent.Common.Utilities.GetOrderStatus("ORDER COMPLETE"), _
                                                            Now, _
                                                            Talent.eCommerce.Utilities.RoundToValue(total, 0.01, False), _
                                                            Profile.Basket.TempOrderID, _
                                                            TalentCache.GetBusinessUnit, _
                                                            Profile.UserName)
                            Logging.WriteLog(Profile.UserName, "COC-101", Profile.UserName & " - Amount Charged = " & total.ToString, "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                        Catch ex As Exception
                            Logging.WriteLog(Profile.UserName, "UCCPTP-010", ex.Message, "Error setting total amount charged on order header - TempOrderID: " & Profile.Basket.TempOrderID, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "OrderSummary.ascx")
                        End Try

                        Insert_Order_Status_Flow("PAYMENT ACCEPTED", _hsbcRequest.LogText)
                        If FinaliseOrder(False, generatedWebOrderNo) Then
                            Insert_Order_Status_Flow("ORDER COMPLETE")
                        Else
                            Insert_Order_Status_Flow("ORDER FAILED", "Payment recieved but order creation failed")
                        End If

                        If _hsbcRequest.requiresReview Then
                            _orderNeedsReview = True
                        Else
                            _orderNeedsReview = False
                        End If
                        Session.Contents.Remove("paymentAttemptCounter")
                    Else
                        Logging.WriteLog(Profile.UserName, "COC-110", Profile.UserName & " - HSBC Payment Error, redirecting to Checkout...", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                        Session("CardError") = True
                        If _hsbcRequest.AvsOrCscRejected Then
                            Session("CardErrorAvs") = True
                        Else
                            Session("CardErrorAvs") = False
                        End If
                        Insert_Order_Status_Flow("PAYMENT REJECTED", _hsbcRequest.LogText + _hsbcRequest.overviewXML)
                        Session("GatewayErrorMessage") = _hsbcRequest.LogText & wfr.Content("HsbcErrorPostfix", _languageCode, True)
                        Response.Redirect("checkout.aspx")

                    End If
                    If _hsbcRequest.transactionFail Then
                        ' ----------------------------------------------------------------------------------
                        ' Transaction error, such as fail to connect to bank (This doesn't include declines)
                        ' ----------------------------------------------------------------------------------
                        Logging.WriteLog(Profile.UserName, "COC-120", Profile.UserName & " - HSBC Failed to connect to the Bank", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                        Session("CardError") = True
                        Insert_Order_Status_Flow("PAYMENT REJECTED", _hsbcRequest.LogText)
                        Session("GatewayErrorMessage") = _hsbcRequest.LogText
                        Response.Redirect("~/pagesSecure/checkoutPaymentDetails.aspx")
                    End If
                End If
            End If
        Else
            Logging.WriteLog(Profile.UserName, "COC-130", Profile.UserName & "Not CallBankAPI or Not Payment Gateway=HSBC", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
            If def.PaymentProcess3dSecure Then
                Logging.WriteLog(Profile.UserName, "COC-140", Profile.UserName & " - Processing 3D Secure", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                If Session("HSBCRequest") Is Nothing Then
                    Logging.WriteLog(Profile.UserName, "COC-150", Profile.UserName & " - No HSBC Session Variable Found", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                    Dim myProfile As TalentProfile
                    myProfile = CType(HttpContext.Current.Profile, TalentProfile)
                    Dim tempOrderId As String = myProfile.Basket.TempOrderID

                    Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter



                    Dim dt As Data.DataTable = orders.Get_Header_By_Temp_Order_Id(tempOrderId)
                    '-----------------------------------------------
                    ' Just completing broken order - stay in section
                    '-----------------------------------------------
                    If dt.Rows.Count > 0 Then
                        Dim status As String = TEBUtilities.CheckForDBNull_String(dt.Rows(0)("STATUS"))
                        If status = Talent.Common.Utilities.GetOrderStatus("ORDER PENDING") OrElse status = Talent.Common.Utilities.GetOrderStatus("ORDER COMPLETE") _
                            OrElse status = Talent.Common.Utilities.GetOrderStatus("ORDER PROCESSED") OrElse status = Talent.Common.Utilities.GetOrderStatus("PROCESS FAILED") Then
                            'The order will be processed or pending (100 or 95) or has been paid for but failed (70 or 90)
                            Logging.WriteLog(Profile.UserName, "COC-160", Profile.UserName & " - Payment already taken", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                        End If
                    Else
                        '----------------------------------------
                        ' Got here without payment info - go back
                        '----------------------------------------
                        Logging.WriteLog(Profile.UserName, "COC-170", Profile.UserName & " - No Payment Info, redirecting to Checkout...", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                        Response.Redirect("checkout.aspx")
                    End If
                Else
                    '----------------------------------------------------------------
                    ' Check 3d Secure results. If failed then go back to payment page
                    '----------------------------------------------------------------
                    _hsbcRequest = CType(Session("HSBCRequest"), CardProcessing.HSBC.HSBCApiRequest)
                    If def.PaymentProcess3dSecure Then
                        Logging.WriteLog(Profile.UserName, "COC-180", Profile.UserName & " - Checking 3D Secure", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                        checkout3dSecureError = check3dSecureHSBC()
                    Else
                        checkout3dSecureError = False
                    End If
                    If checkout3dSecureError Then
                        Logging.WriteLog(Profile.UserName, "COC-190", Profile.UserName & " - 3D Secure Failed, redirecting to Checkout...", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                        Session("CardError") = True
                        Insert_Order_Status_Flow("PAYMENT REJECTED", _hsbcRequest.LogText)
                        Session("GatewayErrorMessage") = _hsbcRequest.LogText
                        Response.Redirect("checkout.aspx")
                    Else
                        Logging.WriteLog(Profile.UserName, "COC-200", Profile.UserName & " - 3D Secure Passed, Finalising Order...", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))

                        Insert_Order_Status_Flow("PAYMENT ACCEPTED", "Dummy Bank Call, 3-d secure ON")
                        If FinaliseOrder(False, generatedWebOrderNo) Then
                            Logging.WriteLog(Profile.UserName, "COC-210", Profile.UserName & " - Order Finalised", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                            Insert_Order_Status_Flow("ORDER COMPLETE")
                        Else
                            Logging.WriteLog(Profile.UserName, "COC-220", Profile.UserName & " - Failed to Finalise Order - Front-End", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                            Insert_Order_Status_Flow("ORDER FAILED", "Payment recieved but order creation failed")
                        End If
                    End If
                End If
            End If
        End If

        ProcessMerchandiseInBackend(True, False, generatedWebOrderNo)

    End Sub

    Protected Sub ProcessInvoiceCheckout()
        Session.Remove("InvoiceRequest")
        Select Case Profile.Basket.BasketContentType
            Case "C"
                ProcessMerchandiseInBackend(False, True, String.Empty)
                ProcessTicketingCheckout()
            Case "T"
                ProcessTicketingCheckout()
            Case "M"
                ProcessMerchandiseInBackend(False, True, String.Empty)
        End Select
    End Sub

    Protected Sub ProcessMerchandiseInBackend(ByVal isOrderAlreadyFinalised As Boolean, ByVal isInvoicePayType As Boolean, ByVal generatedWebOrderNo As String)

        If Not isOrderAlreadyFinalised Then
            Logging.WriteLog(Profile.UserName, "PLCOC-200", Profile.UserName & " Invoice or External Gateway, Finalising Order...", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
            If FinaliseOrder(isInvoicePayType, generatedWebOrderNo) Then
                Logging.WriteLog(Profile.UserName, "PLCOC-210", Profile.UserName & " - Order Finalised", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                Insert_Order_Status_Flow("ORDER COMPLETE")
            Else
                Logging.WriteLog(Profile.UserName, "PLCOC-220", Profile.UserName & " - Failed to Finalise Order - Front-End", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                Insert_Order_Status_Flow("ORDER FAILED", "Payment recieved but order creation failed")
            End If
        End If

        'This session object is set in order.ProcessMerchandiseInBackend as it may have already happened earlier in the payment process and won't need to happen again
        If Session("BackendProcessInCheckout") Is Nothing Then
            Dim order As New Order
            order.ProcessMerchandiseInBackend(isOrderAlreadyFinalised, isInvoicePayType, generatedWebOrderNo, True)
            HttpContext.Current.Session("BackendProcessInCheckout") = Nothing
        End If

        Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
        Dim dt As Data.DataTable = orders.Get_Header_By_Temp_Order_Id(CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID)
        If dt.Rows.Count > 0 Then
            _merchandiseOrderValue = (Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_ORDER_VALUE")) - Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("PROMOTION_VALUE")))
            _merchandiseOrderReference = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0)("PROCESSED_ORDER_ID"))
            _merchandiseBackOfficeOrdRef = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0)("BACK_OFFICE_ORDER_ID"))
        End If


        '-----------------------------------------------------------------------------
        ' Set basket to processed
        ' 
        Try
            Dim basket As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
            For i As Integer = 0 To i = 3
                Try
                    basket.Mark_As_Processed(Profile.Basket.Basket_Header_ID)
                    Exit For
                Catch ex As Exception
                    If i = 3 Then
                        Logging.WriteLog(Profile.UserName, "UCCCPL-020", ex.Message, "Failed to set basket as processed!", "", "", ProfileHelper.GetPageName)
                    Else
                        System.Threading.Thread.Sleep(50)
                    End If
                End Try
            Next
        Catch ex As Exception
        End Try

        Session("BackendProcessInCheckout") = Nothing
    End Sub

    Protected Sub ProcessGiftMessage(ByVal tempOrderID As String, _
                                    ByVal webOrderID As String, _
                                    ByVal def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues)
        Dim isGiftMessage As Boolean = False
        Try
            If TestForGiftMessage(tempOrderID) Then
                isGiftMessage = True
                Dim giftMsgUCR As New Talent.Common.UserControlResource
                With giftMsgUCR
                    .BusinessUnit = TalentCache.GetBusinessUnit
                    .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                    .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    .KeyCode = "GiftMessage.ascx"
                End With
                Try
                    Dim htmlMsg As New HTMLGiftMessage(giftMsgUCR.Content("HTMLTitleText", _languageCode, True), _
                                                        giftMsgUCR.Content("HTMLNameIntroText", _languageCode, True), _
                                                        giftMsgUCR.Content("HTMLNameOutroText", _languageCode, True), _
                                                        giftMsgUCR.Content("HTMLFontSize", _languageCode, True), _
                                                        giftMsgUCR.Content("HTMLFont", _languageCode, True))

                    htmlMsg.writeHTMLMessage(Profile.Basket.TempOrderID)
                Catch ex As Exception
                End Try
                Try
                    Dim GiftEmail As New Talent.eCommerce.GiftMessageEmail(def.GiftMessageEmail_From, _
                                                                            def.GiftMessageEmail_To, _
                                                                            giftMsgUCR.Content("WarehouseEmailSubjectText", _languageCode, True), _
                                                                            giftMsgUCR.Content("WarehouseEmailBodyText", _languageCode, True), _
                                                                            webOrderID, _
                                                                            tempOrderID)
                    GiftEmail.SendMail()
                Catch ex As Exception
                End Try

            End If
        Catch ex As Exception

        End Try
        Try
            Dim ordersTA As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
            ordersTA.UpdateGiftMessageStatus(isGiftMessage, Profile.Basket.TempOrderID, TalentCache.GetBusinessUnit, Profile.UserName)
        Catch ex As Exception
        End Try
    End Sub

    Protected Function TestForGiftMessage(ByVal tempOrderId As String) As Boolean
        Dim exists As Boolean = False
        Dim SQLString As String = " SELECT * " & _
                  " FROM tbl_gift_message WITH (NOLOCK)  " & _
                  " WHERE TEMP_ORDER_ID = @TEMP_ORDER_ID " & _
                  " AND BUSINESS_UNIT = @BUSINESS_UNIT " & _
                  " AND PARTNER = @PARTNER "

        Dim cmd As New Data.SqlClient.SqlCommand(SQLString, New Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ConnectionString))
        Dim dr As Data.SqlClient.SqlDataReader = Nothing

        '--------------------
        ' Add the parameters
        '--------------------
        With cmd.Parameters
            .Clear()
            .Add("TEMP_ORDER_ID", Data.SqlDbType.NVarChar).Value = tempOrderId
            .Add("BUSINESS_UNIT", Data.SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
            .Add("PARTNER", Data.SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
        End With

        Try
            ' ------------------------------------------------------------
            ' Open connection to DB
            ' ------------------------------------------------------------
            cmd.Connection.Open()
            dr = cmd.ExecuteReader()

            If dr.HasRows Then
                exists = True
            End If
            dr.Close()
        Catch ex As Exception
        Finally
            cmd.Connection.Close()
        End Try

        Return exists
    End Function

    Protected Sub Insert_Order_Status_Flow(ByVal StatusName As String, Optional ByVal comment As String = "")
        Try
            Dim status As New TalentBasketDatasetTableAdapters.tbl_order_statusTableAdapter
            status.Insert_Order_Status_Flow(TalentCache.GetBusinessUnit, _
                                                Profile.Basket.TempOrderID, _
                                                Talent.Common.Utilities.GetOrderStatus(StatusName), _
                                                Now, _
                                                comment)
        Catch ex As Exception
        End Try
    End Sub

    Protected Function SendCreditExceededEmail(ByVal backOfficeOrderNo As String, _
                                                ByVal WebOrderNumber As String, _
                                                ByVal TotalOrderItemsValue As Decimal, _
                                                ByVal def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues) As Boolean

        Try
            Dim subject, body As String
            subject = wfr.Content("CreditLimitExceededEmailSubject", _languageCode, True)
            body = wfr.Content("CreditLimitExceededBodyText", _languageCode, True)

            body = body.Replace("<<LineBreak>>", vbCrLf)
            body = body.Replace("<<Username>>", Profile.UserName)
            body = body.Replace("<<UserNumber>>", Profile.User.Details.User_Number.ToString)
            body = body.Replace("<<OrderDate>>", Now.ToString)
            body = body.Replace("<<OrderValue>>", TotalOrderItemsValue.ToString)
            body = body.Replace("<<WebOrderID>>", WebOrderNumber.ToString)
            body = body.Replace("<<BackOfficeOrderNo>>", backOfficeOrderNo.ToString)
            body = body.Replace("<<AccountNumber>>", Profile.User.Details.Account_No_1.ToString)

            Talent.Common.Utilities.Email_Send(def.EmailFromAddress_OrderCreditLimitExceeded, _
                                                def.EmailToAddress_OrderCreditLimitExceeded, _
                                                subject, _
                                                body)
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Protected Function check3dSecureHSBC() As Boolean
        Dim checkout3dSecureError As Boolean = False

        _hsbcRequest.SecCardBrand = Request.Params("CardBrand")
        _hsbcRequest.SecCcpaClientID = Request.Params("CcpaClientId")
        _hsbcRequest.SecMd = Request.Params("MD")
        _hsbcRequest.SecPurchaseCurrency = Request.Params("PurchaseCurrency")
        _hsbcRequest.SecPurchaseAmountRaw = Request.Params("PurchaseAmountRaw")
        _hsbcRequest.SecXid = Request.Params("XID")
        _hsbcRequest.SecTransactionStatus = Request.Params("TransactionStatus")
        _hsbcRequest.SecCAVV = Request.Params("CAVV")
        _hsbcRequest.SecCaVVAlgorithm = Request.Params("CAVVAlgorithm")
        _hsbcRequest.SecECI = Request.Params("ECI")
        _hsbcRequest.SecPurchaseDate = Request.Params("PurchaseDate")
        _hsbcRequest.SecCcpaResultsCode = Request.Params("CcpaResultsCode")
        _hsbcRequest.SecIreqCode = Request.Params("IreqCode")
        _hsbcRequest.SecIreqVendorCode = Request.Params("IreqVendorCode")
        _hsbcRequest.SecIreqDetail = Request.Params("IreqDetail")

        '------------------------------------------------------
        ' Check 3d Secure settings and set additonal properties
        '------------------------------------------------------
        checkout3dSecureError = _hsbcRequest.Check3dSecure()
        '
        ' Final check - always error if basket doesn't match
        If (_hsbcRequest.SecMd.Trim <> Profile.Basket.TempOrderID.Trim) Then
            checkout3dSecureError = True
        End If

        ' LogWriter.LogBankCCommunication("Received 3d Secure details for basket " & Session("myBasket").ToString.Trim & ". Result:" & CcpaResultsCode & ". Error:" + checkout3dSecureError.ToString)
        Return checkout3dSecureError
    End Function

    Protected Function FinaliseOrder(ByVal isInvoicePayType As Boolean, ByVal generatedWebOrderNo As String) As Boolean
        Try
            Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
            Dim orderDets As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter

            Dim webOrderID As String = String.Empty
            If generatedWebOrderNo <> String.Empty Then
                webOrderID = generatedWebOrderNo
            Else
                webOrderID = Checkout.GenNewOrderID()
            End If
            For i As Integer = 1 To 3
                Try
                    orders.Complete_Order(webOrderID, Talent.Common.Utilities.GetOrderStatus("ORDER COMPLETE"), Now, Profile.Basket.TempOrderID)
                    orderDets.UpdateHeaderOrderId(webOrderID, Profile.Basket.TempOrderID)
                    If i > 1 Then Logging.WriteLog(Profile.UserName, _
                                                    "UCCPFO-030", _
                                                    String.Format("Finalise order failed initially but was completed at attempt {0}", i.ToString), _
                                                    String.Format("Order successfully finalised at attempt {0} - TempOrderID: {1} - WebOrderID: {2}", i.ToString, Profile.Basket.TempOrderID, webOrderID), _
                                                    TalentCache.GetBusinessUnit, _
                                                    TalentCache.GetPartner(Profile), _
                                                    ProfileHelper.GetPageName, _
                                                    "OrderSummary.ascx")
                    Exit For
                Catch ex As Exception
                    If i = 3 Then
                        Logging.WriteLog(Profile.UserName, "UCCPFO-020", ex.Message, String.Format("Error finalising order, attempt {0} - TempOrderID: {1}", i.ToString, Profile.Basket.TempOrderID), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "OrderSummary.ascx")
                    Else
                        System.Threading.Thread.Sleep(50)
                    End If
                End Try
            Next
            Try
                If isInvoicePayType Then
                    orders.SetPaymentType("INV", Profile.Basket.TempOrderID)
                    Dim dt As Data.DataTable = orders.Get_Header_By_Temp_Order_Id(Profile.Basket.TempOrderID)
                    Dim total As Decimal = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_ORDER_VALUE"))
                    Dim promo As Decimal = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(dt.Rows(0)("PROMOTION_VALUE"))
                    total = total - promo
                    orders.Set_Total_Amount_Charged(Talent.Common.Utilities.GetOrderStatus("ORDER COMPLETE"), _
                                                    Now, _
                                                    Talent.eCommerce.Utilities.RoundToValue(total, 0.01, False), _
                                                    Profile.Basket.TempOrderID, _
                                                    TalentCache.GetBusinessUnit, _
                                                    Profile.UserName)
                Else
                    orders.SetPaymentType("Credit Card", Profile.Basket.TempOrderID)
                End If
            Catch ex As Exception
                Logging.WriteLog(Profile.UserName, "COC-111", ex.Message, "Error setting payment type as cc or inv - TempOrderID: " & Profile.Basket.TempOrderID, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "")
            End Try

            UpdateTempOrderIDOnBasket()

        Catch ex As Exception
            Logging.WriteLog(Profile.UserName, "UCCPFO-010", ex.Message, "Error marking order header as finalised - TempOrderID: " & Profile.Basket.TempOrderID, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "OrderSummary.ascx")
            Return False
        End Try
        Return True
    End Function

    Protected Sub UpdateTempOrderIDOnBasket()
        Try
            Dim basket As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
            For i As Integer = 1 To 3
                Try
                    basket.Update_Temp_order_id(Profile.Basket.TempOrderID, Profile.Basket.Basket_Header_ID)
                    Exit For
                Catch ex As Exception
                    If i = 3 Then
                        Logging.WriteLog(Profile.UserName, "UCCPUB-010", ex.Message, String.Format("Error finalising basket, attempt {0} - TempOrderID: {1}", i.ToString, Profile.Basket.TempOrderID), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), ProfileHelper.GetPageName, "OrderSummary.ascx")
                    Else
                        System.Threading.Thread.Sleep(50)
                    End If
                End Try
            Next
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub btnConfirm_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirm.Click
        Dim err As New ErrorObj
        err = NotificationOptions.ConfirmNotificationOptions(Not uscDespatchProcess.Visible)
        If Not err.HasError Then
            If uscDespatchProcess.Visible Then err = uscDespatchProcess.FinishOrder()
            If Not err.HasError Then
                uscDespatchProcess.Visible = False
                NotificationOptions.Visible = False
                plhConfirm.Visible = False
                plhSuccessMessage.Visible = True
                blSuccessMessages.Items.Add(wfr.Content("SuccessMessage", _languageCode, True))
                btnNextSale.Text = wfr.Content("NextSaleButtonText", _languageCode, True)
                Confirmation_BasketDetails.Visible = False
            End If
        End If
    End Sub

    Protected Sub btnNextSale_Click(sender As Object, e As EventArgs) Handles btnNextSale.Click
        If def.LOG_USER_OUT_AFTER_CHECKOUT And Not NotificationOptions.Visible Then
            FormsAuthentication.SignOut()
            Talent.eCommerce.Utilities.ClearAllSessions()
        End If
        Response.Redirect(TEBUtilities.GetSiteHomePage())
    End Sub

#End Region

#Region "Private Methods"

    Private Sub DisplayConfirmationMessage()

        'The order number will be the ticketing payment reference when we are integrated to the ticketing database.
        Select Case Talent.eCommerce.Utilities.BasketContentTypeWithOverride
            Case "C"
                If Not String.IsNullOrEmpty(Request.QueryString("PaymentRef")) Then
                    DisplaySingleBasketPaymentConfirmation()
                End If
            Case "T"
                If Not String.IsNullOrEmpty(Request.QueryString("PaymentRef")) Then
                    If Talent.eCommerce.Utilities.CheckForDBNull_Decimal(GetValueFromQuerystring("PaymentAmount")) >= 0 Then
                        'Process Payment details
                        DisplayTicketingPaymentConfirmation()
                    Else
                        'Process Refund details
                        DisplayTicketingRefundConfirmation()
                    End If
                End If
            Case "M"
                DisplayMerchandisePaymentConfirmation()
        End Select
    End Sub

    Private Sub DisplayTicketingPaymentConfirmation()
        Dim payType As String = GetPayTypeString("paymentType")
        Dim payAmountString As String = GetValueFromQuerystring("PaymentAmount")
        Dim payAmount As Decimal = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(payAmountString)

        Dim confirmDetails As String = String.Empty
        Dim confirmMessage As String = String.Empty
        confirmDetails = wfr.Content("TicketingPaymentDetails", _languageCode, True)
        If Profile.IsAnonymous Then
            confirmMessage = wfr.Content("TicketingPaymentMessageForGenericSale", _languageCode, True)
        Else
            confirmMessage = wfr.Content("TicketingPaymentMessage", _languageCode, True)
        End If
        confirmDetails = confirmDetails.Replace("<<PAYMENT_TYPE>>", payType)
        confirmDetails = confirmDetails.Replace("<<PAYMENT_REFERENCE>>", GetValueFromQuerystring("PaymentRef"))
        If payAmountString.Length > 0 Then
            confirmDetails = confirmDetails.Replace("<<PAYMENT_AMOUNT>>", TDataObjects.PaymentSettings.FormatCurrency(payAmount, wfr.BusinessUnit, wfr.PartnerCode))
        Else
            confirmDetails = confirmDetails.Replace("<<PAYMENT_AMOUNT>>", "")
        End If
        ltlConfirmationDetails.Text = confirmDetails
        ltlConfirmationMessage.Text = confirmMessage
    End Sub

    Private Sub DisplayTicketingRefundConfirmation()
        Dim payType As String = GetPayTypeString("paymentType")
        Dim payAmountString As String = GetValueFromQuerystring("PaymentAmount")
        Dim payAmount As Decimal = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(payAmountString)
        Dim payRef As String = GetValueFromQuerystring("PaymentRef")
        Dim confirmDetails As String = String.Empty
        Dim confirmMessage As String = String.Empty
        confirmDetails = wfr.Content("TicketingRefundDetails", _languageCode, True)
        confirmMessage = wfr.Content("TicketingRefundMessage", _languageCode, True)
        confirmDetails = confirmDetails.Replace("<<PAYMENT_TYPE>>", payType)
        confirmDetails = confirmDetails.Replace("<<PAYMENT_REFERENCE>>", payRef)
        If payAmountString.Length > 0 Then
            confirmDetails = confirmDetails.Replace("<<PAYMENT_AMOUNT>>", TDataObjects.PaymentSettings.FormatCurrency(payAmount, wfr.BusinessUnit, wfr.PartnerCode))
        Else
            confirmDetails = confirmDetails.Replace("<<PAYMENT_AMOUNT>>", "")
        End If
        ltlConfirmationDetails.Text = confirmDetails
        ltlConfirmationMessage.Text = confirmMessage
    End Sub

    Private Sub DisplayMerchandisePaymentConfirmation()
        Dim payType As String = GetPayTypeString("paymentType")
        Dim confirmDetails As String = String.Empty
        Dim confirmMessage As String = String.Empty
        confirmDetails = wfr.Content("MerchandisePaymentDetails", _languageCode, True)
        confirmMessage = wfr.Content("MerchandisePaymentMessage", _languageCode, True)
        confirmDetails = confirmDetails.Replace("<<PAYMENT_TYPE>>", payType)
        confirmDetails = confirmDetails.Replace("<<ORDER_REFERENCE>>", _merchandiseOrderReference)
        confirmDetails = confirmDetails.Replace("<<PAYMENT_AMOUNT>>", TDataObjects.PaymentSettings.FormatCurrency(_merchandiseOrderValue, wfr.BusinessUnit, wfr.PartnerCode))
        If _merchandiseBackOfficeOrdRef.Length > 0 Then
            confirmDetails = confirmDetails.Replace("<<BACK_OFFICE_ORDER_ID>>", " - " & _merchandiseBackOfficeOrdRef)
        Else
            confirmDetails = confirmDetails.Replace("<<BACK_OFFICE_ORDER_ID>>", _merchandiseBackOfficeOrdRef)
        End If
        confirmDetails = confirmDetails.Replace("<<BACK_OFFICE_ORDER_ID>>", _merchandiseBackOfficeOrdRef)
        ltlConfirmationDetails.Text = confirmDetails
        ltlConfirmationMessage.Text = confirmMessage
    End Sub

    Private Sub DisplaySingleBasketPaymentConfirmation()
        Dim payType As String = GetPayTypeString("paymentType")
        Dim payAmountString As String = GetValueFromQuerystring("PaymentAmount")
        Dim payAmount As Decimal = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(payAmountString)
        Dim confirmDetails As String = String.Empty
        Dim confirmMessage As String = String.Empty
        confirmDetails = wfr.Content("SingleBasketPaymentDetails", _languageCode, True)
        confirmMessage = wfr.Content("SingleBasketPaymentMessage", _languageCode, True)
        confirmDetails = confirmDetails.Replace("<<PAYMENT_TYPE>>", payType)
        confirmDetails = confirmDetails.Replace("<<PAYMENT_REFERENCE>>", GetValueFromQuerystring("PaymentRef"))
        confirmDetails = confirmDetails.Replace("<<ORDER_REFERENCE>>", _merchandiseOrderReference)
        If _merchandiseBackOfficeOrdRef.Length > 0 Then
            confirmDetails = confirmDetails.Replace("<<BACK_OFFICE_ORDER_ID>>", " - " & _merchandiseBackOfficeOrdRef)
        Else
            confirmDetails = confirmDetails.Replace("<<BACK_OFFICE_ORDER_ID>>", _merchandiseBackOfficeOrdRef)
        End If
        If payAmountString.Length > 0 Then
            confirmDetails = confirmDetails.Replace("<<PAYMENT_AMOUNT>>", TDataObjects.PaymentSettings.FormatCurrency(payAmount, wfr.BusinessUnit, wfr.PartnerCode))
        Else
            confirmDetails = confirmDetails.Replace("<<PAYMENT_AMOUNT>>", "")
        End If
        ltlConfirmationDetails.Text = confirmDetails
        ltlConfirmationMessage.Text = confirmMessage
    End Sub

    Private Function GetValueFromQuerystring(ByVal queryStringKey As String) As String
        Dim queryStringValue As String = String.Empty
        If Not String.IsNullOrWhiteSpace(Request.QueryString(queryStringKey)) Then
            queryStringValue = Request.QueryString(queryStringKey).Trim
        End If
        Return queryStringValue
    End Function

    Private Function GetPayTypeString(ByVal queryStringKey As String) As String
        Dim payType As String = GetValueFromQuerystring(queryStringKey).ToUpper
        If payType.Length > 0 Then
            'there is a increase in minutes in caching, as it is base table 
            Dim dtPaymentType As DataTable = TDataObjects.PaymentSettings.TblPaymentTypeLang.GetByBUPartnerLangCode(_businessUnit, _partner, _languageCode, True, 90)
            For rowIndex As Integer = 0 To dtPaymentType.Rows.Count - 1
                If payType = Talent.eCommerce.Utilities.CheckForDBNull_String(dtPaymentType.Rows(rowIndex)("PAYMENT_TYPE_CODE")) Then
                    payType = Talent.eCommerce.Utilities.CheckForDBNull_String(dtPaymentType.Rows(rowIndex)("PAYMENT_TYPE_DESCRIPTION"))
                    Exit For
                End If
            Next
        End If
        Return payType
    End Function

#End Region

End Class
