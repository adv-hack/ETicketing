Imports Talent.eCommerce
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Partial Class Redirect_VanguardGateway
    Inherits Base01

#Region "Class Level Fields"
    Private _moduleDefaultsValue As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Private _wfr As Talent.Common.WebFormResource
    Private _languageCode As String = String.Empty
    Private _talentLogging As Talent.Common.TalentLogging
    Private _expectedStageValue As String = String.Empty
    Private _currentProcessStage As String = String.Empty
    Private _isSessionMissing As Boolean = False
    Private _isJavaScriptRedirect As Boolean = False
#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        InitialisePage()
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ltlProcessingWaitMessage.Text = _wfr.Content("PleaseWaitText", _languageCode, True)
    End Sub

    Protected Sub Page_PreRenderComplete(sender As Object, e As EventArgs) Handles Me.PreRenderComplete
        _expectedStageValue = "STAGE1"
        If Not String.IsNullOrWhiteSpace(Request.QueryString("_uvgsv")) Then
            _expectedStageValue = "STAGE2"
        End If
        ProcessVanguard()
    End Sub

    Protected Sub Page_Unload(sender As Object, e As EventArgs) Handles Me.Unload

    End Sub
#End Region

#Region "Private Methods"
    Private Sub ProcessVanguard()
        Dim err As New ErrorObj
        Dim basketPaymentID As Long = 0
        Dim vgAttributes As DEVanguard = Nothing
        Dim basketPayEntity As DEBasketPayment = Nothing
        Try
            If CanProcessRequest() Then
                If TEBUtilities.TryGetVGAttributesSession(basketPaymentID, vgAttributes, basketPayEntity) OrElse IsNothing(vgAttributes) Then
                    Dim is3DSecureOn As Boolean = CanProcess3DSecure()
                    If vgAttributes.TxnType = DEVanguard.TransactionType.REFUND Then
                        is3DSecureOn = False
                    End If
                    If (_currentProcessStage = DEVanguard.CheckoutStages.PPS1) OrElse (_currentProcessStage = DEVanguard.CheckoutStages.PPS2) Then
                        is3DSecureOn = False
                        If is3DSecureOn Then
                            Process_3DSecureOn_PPS_Transaction(basketPaymentID, vgAttributes, basketPayEntity)
                        Else
                            Process_3DSecureOff_PPS_Transaction(basketPaymentID, vgAttributes, basketPayEntity)
                        End If
                    ElseIf _currentProcessStage = DEVanguard.CheckoutStages.CHECKOUT Then
                        Process_Checkout_Transaction(is3DSecureOn, basketPaymentID, vgAttributes, basketPayEntity)
                    ElseIf _currentProcessStage = DEVanguard.CheckoutStages.SAVEMYCARD Then
                        Process_3DSecureOff_SaveMyCard(basketPaymentID, vgAttributes, basketPayEntity)
                    ElseIf _currentProcessStage = DEVanguard.CheckoutStages.AMENDPPSCARD Then
                        Process_3DSecureOff_AmendPPSCard(basketPaymentID, vgAttributes, basketPayEntity)
                    End If
                Else
                    err.HasError = True
                    err.ErrorNumber = "VGGERR-SM"
                    err.ErrorMessage = "VG Attributes and related session are missing"
                    ProcessError(err)
                End If
            Else
                If _isSessionMissing Then
                    err.HasError = True
                    err.ErrorNumber = "VGGERR-SM"
                    err.ErrorMessage = "VG Attributes and related session are missing"
                Else
                    err.HasError = True
                    err.ErrorNumber = "VGGERR-002"
                    err.ErrorMessage = "Invalid Request"
                End If
                ProcessError(err)
            End If
        Catch ex As Threading.ThreadAbortException
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "VGGEX-001"
            err.ErrorMessage = ex.Message
            ProcessError(err, ex)
        End Try
    End Sub

    Private Sub InitialisePage()
        _wfr = New Talent.Common.WebFormResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _talentLogging = New Talent.Common.TalentLogging
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "VanguardGateway.aspx"
        End With
        _moduleDefaultsValue = (New Talent.eCommerce.ECommerceModuleDefaults()).GetDefaults

        Dim referrerUrl As String = String.Empty
        Dim loginid As String = String.Empty
        If Profile.IsAnonymous AndAlso Profile.User IsNot Nothing AndAlso Profile.User.Details IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(Profile.User.Details.LoginID) Then
            loginid = Profile.User.Details.LoginID
        End If
        Try
            referrerUrl = Request.UrlReferrer.AbsoluteUri
        Catch ex As Exception
            referrerUrl = ex.Message
        End Try
        _talentLogging.FrontEndConnectionString = _wfr.FrontEndConnectionString
        _talentLogging.GeneralLog(_wfr.PageCode, "Page_Init", "Customer: " & loginid & ", Referrer URL: " & referrerUrl, "VGLog")

    End Sub

    Private Function Process_3DSecureOff_SaveMyCard(ByVal basketPaymentID As Long, ByVal vgAttributes As DEVanguard, ByVal basketPayEntity As DEBasketPayment) As ErrorObj
        Dim err As New ErrorObj
        Dim talVanguard As New TalentVanguard
        talVanguard.Settings = TEBUtilities.GetSettingsObject()
        talVanguard.VanguardAttributes = SetOtherAttributes(False, vgAttributes)
        talVanguard.BasketPayEntity = basketPayEntity
        talVanguard.VanguardAttributes.BasketPaymentID = basketPaymentID
        talVanguard.VanguardAttributes.ProcessStep = DEVanguard.ProcessingStep.SAVE_MY_CARD
        talVanguard.VanguardAttributes.SessionID = Session.SessionID
        err = talVanguard.ProcessVanguard()
        If (Not err.HasError) AndAlso talVanguard.VanguardAttributes IsNot Nothing Then
            If talVanguard.VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.SAVEMYCARD Then
                ProcessSuccessSaveMyCard(talVanguard.VanguardAttributes, talVanguard.BasketPayEntity)
            End If
        Else
            err.ErrorMessage = err.ErrorNumber & ";" & err.ErrorMessage
            err.ErrorNumber = "VGPERR-SMC-001"
            ProcessError(err)
        End If
        Return err
    End Function

    Private Function Process_3DSecureOff_AmendPPSCard(ByVal basketPaymentID As Long, ByVal vgAttributes As DEVanguard, ByVal basketPayEntity As DEBasketPayment) As ErrorObj
        Dim err As New ErrorObj
        Dim talVanguard As New TalentVanguard
        talVanguard.Settings = TEBUtilities.GetSettingsObject()
        talVanguard.VanguardAttributes = SetOtherAttributes(False, vgAttributes)
        talVanguard.BasketPayEntity = basketPayEntity
        talVanguard.VanguardAttributes.BasketPaymentID = basketPaymentID
        talVanguard.VanguardAttributes.ProcessStep = DEVanguard.ProcessingStep.SAVE_AMENDPPS_CARD
        talVanguard.VanguardAttributes.SessionID = Session.SessionID
        err = talVanguard.ProcessVanguard()
        If (Not err.HasError) AndAlso talVanguard.VanguardAttributes IsNot Nothing Then
            If talVanguard.VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.AMENDPPSCARD Then
                ProcessSuccessAmendPPSCard(talVanguard.VanguardAttributes, talVanguard.BasketPayEntity)
            End If
        Else
            err.ErrorMessage = err.ErrorNumber & ";" & err.ErrorMessage
            err.ErrorNumber = "VGPERR-SMC-001"
            ProcessError(err)
        End If
        Return err
    End Function

    Private Sub Process_Checkout_Transaction(ByVal is3DSecureOn As Boolean, ByVal basketPaymentID As Long, ByVal vgAttributes As DEVanguard, ByVal basketPayEntity As DEBasketPayment)
        Dim err As New ErrorObj
        ' is it part payment 
        If vgAttributes.PaymentAmount <> "" AndAlso vgAttributes.PaymentAmount <> vgAttributes.BasketAmount Then
            If is3DSecureOn Then
                Process_3DSecureOn_Checkout_PartPayment_Transaction(basketPaymentID, vgAttributes, basketPayEntity)
            Else
                Process_3DSecureOff_Checkout_Partpayment_Transaction(basketPaymentID, vgAttributes, basketPayEntity)
            End If
        Else
            'final transaction or final full payment
            If is3DSecureOn Then
                Process_3DSecureOn_Checkout_Transaction(basketPaymentID, vgAttributes, basketPayEntity)
            Else
                Process_3DSecureOff_Checkout_Transaction(basketPaymentID, vgAttributes, basketPayEntity)
            End If
        End If
    End Sub

    Private Sub Process_3DSecureOff_Checkout_Transaction(ByVal basketPaymentID As Long, ByVal vgAttributes As DEVanguard, ByVal basketPayEntity As DEBasketPayment)
        Dim err As New ErrorObj
        Dim talVanguard As New TalentVanguard
        talVanguard.Settings = TEBUtilities.GetSettingsObject()
        talVanguard.VanguardAttributes = SetOtherAttributes(False, vgAttributes)
        talVanguard.BasketPayEntity = basketPayEntity
        talVanguard.VanguardAttributes.BasketPaymentID = basketPaymentID
        talVanguard.VanguardAttributes.ProcessStep = DEVanguard.ProcessingStep.SECURE3D_OFF_CHECKOUT_TRANSACTION
        talVanguard.VanguardAttributes.SessionID = Session.SessionID
        err = talVanguard.ProcessVanguard()
        If (Not err.HasError) AndAlso talVanguard.VanguardAttributes IsNot Nothing Then
            ProcessSuccessPayment(talVanguard.VanguardAttributes, talVanguard.BasketPayEntity)
        Else
            err.ErrorMessage = err.ErrorNumber & ";" & err.ErrorMessage
            err.ErrorNumber = "VGPERR-CHK-001"
            ProcessError(err)
        End If
    End Sub

    Private Sub Process_3DSecureOn_Checkout_Transaction(ByVal basketPaymentID As Long, ByVal vgAttributes As DEVanguard, ByVal basketPayEntity As DEBasketPayment)
        Dim isStepInPart2 As Boolean = False
        Dim err As New ErrorObj
        Dim talVanguard As New TalentVanguard
        talVanguard.Settings = TEBUtilities.GetSettingsObject()
        talVanguard.VanguardAttributes = SetOtherAttributes(True, vgAttributes)
        talVanguard.BasketPayEntity = basketPayEntity
        talVanguard.VanguardAttributes.BasketPaymentID = basketPaymentID
        If Session("VG_FROM_3DSECURE") IsNot Nothing AndAlso Session("VG_FROM_3DSECURE") = True Then
            talVanguard.VanguardAttributes.ProcessStep = DEVanguard.ProcessingStep.SECURE3D_ON_CHECKOUT_PART2
            _isJavaScriptRedirect = True
            Session("VG_FROM_3DSECURE") = Nothing
            isStepInPart2 = True
            If Not Request.Form.Item("PaRes") Is Nothing Then
                talVanguard.VanguardAttributes.PayerAuth.PareS = Request.Form.Item("PaRes")
            End If
        Else
            talVanguard.VanguardAttributes.ProcessStep = DEVanguard.ProcessingStep.SECURE3D_ON_CHECKOUT_PART1
        End If
        talVanguard.VanguardAttributes.SessionID = Session.SessionID
        err = talVanguard.ProcessVanguard()
        If (Not err.HasError) AndAlso talVanguard.VanguardAttributes IsNot Nothing Then
            If talVanguard.VanguardAttributes.Is3DTransactionCompleted Then
                ProcessSuccessPayment(talVanguard.VanguardAttributes, talVanguard.BasketPayEntity)
            Else
                If isStepInPart2 Then
                    'unhandled error occured
                    err.ErrorMessage = err.ErrorNumber & ";" & err.ErrorMessage
                    err.ErrorNumber = "VGPERR-3DCHK-002"
                    ProcessError(err)
                Else
                    'process 3d secure
                    Dim Is3DSecureSessionSet As Boolean = False
                    Is3DSecureSessionSet = TEBUtilities.SetVGAttributesSession(talVanguard.VanguardAttributes.BasketPaymentID, talVanguard.VanguardAttributes, talVanguard.BasketPayEntity)
                    Response.Redirect("~/PagesLogin/Checkout/checkout3dSecure.aspx")
                End If

            End If
        Else
            err.ErrorMessage = err.ErrorNumber & ";" & err.ErrorMessage
            err.ErrorNumber = "VGPERR-3DCHK-001"
            ProcessError(err)
        End If
    End Sub

    Private Function Process_3DSecureOn_PPS_Transaction(ByVal basketPaymentID As Long, ByVal vgAttributes As DEVanguard, ByVal basketPayEntity As DEBasketPayment) As ErrorObj
        Dim isStepInPart2 As Boolean = False
        Dim err As New ErrorObj
        Dim talVanguard As New TalentVanguard
        talVanguard.Settings = TEBUtilities.GetSettingsObject()
        talVanguard.VanguardAttributes = SetOtherAttributes(True, vgAttributes)
        talVanguard.BasketPayEntity = basketPayEntity
        talVanguard.VanguardAttributes.BasketPaymentID = basketPaymentID
        If Session("VG_FROM_3DSECURE") IsNot Nothing AndAlso Session("VG_FROM_3DSECURE") = True Then
            talVanguard.VanguardAttributes.ProcessStep = DEVanguard.ProcessingStep.SECURE3D_ON_PPS_TRANSACTION_PART2
            _isJavaScriptRedirect = True
            Session("VG_FROM_3DSECURE") = Nothing
            isStepInPart2 = True
            If Not Request.Form.Item("PaRes") Is Nothing Then
                talVanguard.VanguardAttributes.PayerAuth.PareS = Request.Form.Item("PaRes")
            End If
        Else
            talVanguard.VanguardAttributes.ProcessStep = DEVanguard.ProcessingStep.SECURE3D_ON_PPS_TRANSACTION_PART1
        End If
        talVanguard.VanguardAttributes.SessionID = Session.SessionID
        err = talVanguard.ProcessVanguard()
        If (Not err.HasError) AndAlso talVanguard.VanguardAttributes IsNot Nothing Then
            If Not isStepInPart2 Then
                Dim Is3DSecureSessionSet As Boolean = False
                Is3DSecureSessionSet = TEBUtilities.SetVGAttributesSession(talVanguard.VanguardAttributes.BasketPaymentID, talVanguard.VanguardAttributes, talVanguard.BasketPayEntity)
                Response.Redirect("~/PagesLogin/Checkout/checkout3dSecure.aspx")
            Else
                If talVanguard.VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.PPS1 Then
                    RedirectForPPS1(talVanguard.BasketPayEntity)
                ElseIf talVanguard.VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.PPS2 Then
                    RedirectForPPS2(talVanguard.BasketPayEntity)
                End If
            End If
        Else
            err.ErrorMessage = err.ErrorNumber & ";" & err.ErrorMessage
            err.ErrorNumber = "VGPERR-3DPPS1-001"
            ProcessError(err)
        End If
        Return err
    End Function

    Private Function Process_3DSecureOff_PPS_Transaction(ByVal basketPaymentID As Long, ByVal vgAttributes As DEVanguard, ByVal basketPayEntity As DEBasketPayment) As ErrorObj
        Dim err As New ErrorObj
        Dim talVanguard As New TalentVanguard
        talVanguard.Settings = TEBUtilities.GetSettingsObject()
        talVanguard.VanguardAttributes = SetOtherAttributes(False, vgAttributes)
        talVanguard.BasketPayEntity = basketPayEntity
        talVanguard.VanguardAttributes.BasketPaymentID = basketPaymentID
        talVanguard.VanguardAttributes.ProcessStep = DEVanguard.ProcessingStep.SECURE3D_OFF_PPS_TRANSACTION
        talVanguard.VanguardAttributes.SessionID = Session.SessionID
        err = talVanguard.ProcessVanguard()
        If (Not err.HasError) AndAlso talVanguard.VanguardAttributes IsNot Nothing Then
            If talVanguard.VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.PPS1 Then
                RedirectForPPS1(talVanguard.BasketPayEntity)
            ElseIf talVanguard.VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.PPS2 Then
                RedirectForPPS2(talVanguard.BasketPayEntity)
            End If
        Else
            err.ErrorMessage = err.ErrorNumber & ";" & err.ErrorMessage
            err.ErrorNumber = "VGPERR-PPS1-001"
            ProcessError(err)
        End If
        Return err
    End Function

    Private Sub Process_3DSecureOff_Checkout_Partpayment_Transaction(ByVal basketPaymentID As Long, ByVal vgAttributes As DEVanguard, ByVal basketPayEntity As DEBasketPayment)
        Dim err As New ErrorObj
        Dim talVanguard As New TalentVanguard
        talVanguard.Settings = TEBUtilities.GetSettingsObject()
        talVanguard.VanguardAttributes = SetOtherAttributes(False, vgAttributes)
        talVanguard.BasketPayEntity = basketPayEntity
        talVanguard.VanguardAttributes.BasketPaymentID = basketPaymentID
        talVanguard.VanguardAttributes.ProcessStep = DEVanguard.ProcessingStep.SECURE3D_OFF_CHECKOUT_PARTPAYMENT
        talVanguard.VanguardAttributes.SessionID = Session.SessionID
        err = talVanguard.ProcessVanguard()
        If (Not err.HasError) AndAlso talVanguard.VanguardAttributes IsNot Nothing Then
            ProcessSuccessPartPayment(talVanguard.VanguardAttributes, talVanguard.BasketPayEntity)
        Else
            err.ErrorMessage = err.ErrorNumber & ";" & err.ErrorMessage
            err.ErrorNumber = "VGPERR-PPT-002"
            ProcessError(err)
        End If
    End Sub

    Private Sub Process_3DSecureOn_Checkout_PartPayment_Transaction(ByVal basketPaymentID As Long, ByVal vgAttributes As DEVanguard, ByVal basketPayEntity As DEBasketPayment)
        Dim isStepInPart2 As Boolean = False
        Dim talVanguard As New TalentVanguard
        Dim err As New ErrorObj
        talVanguard.Settings = TEBUtilities.GetSettingsObject()
        talVanguard.VanguardAttributes = SetOtherAttributes(True, vgAttributes)
        talVanguard.BasketPayEntity = basketPayEntity
        talVanguard.VanguardAttributes.BasketPaymentID = basketPaymentID
        If Session("VG_FROM_3DSECURE") IsNot Nothing AndAlso Session("VG_FROM_3DSECURE") = True Then
            talVanguard.VanguardAttributes.ProcessStep = DEVanguard.ProcessingStep.SECURE3D_ON_CHECKOUT_PARTPAYMENT2
            _isJavaScriptRedirect = True
            Session("VG_FROM_3DSECURE") = Nothing
            isStepInPart2 = True
            If Not Request.Form.Item("PaRes") Is Nothing Then
                talVanguard.VanguardAttributes.PayerAuth.PareS = Request.Form.Item("PaRes")
            End If
        Else
            talVanguard.VanguardAttributes.ProcessStep = DEVanguard.ProcessingStep.SECURE3D_ON_CHECKOUT_PARTPAYMENT1
        End If
        talVanguard.VanguardAttributes.SessionID = Session.SessionID
        err = talVanguard.ProcessVanguard()
        If (Not err.HasError) AndAlso talVanguard.VanguardAttributes IsNot Nothing Then
            If talVanguard.VanguardAttributes.Is3DTransactionCompleted Then
                ProcessSuccessPartPayment(talVanguard.VanguardAttributes, talVanguard.BasketPayEntity)
            Else
                If isStepInPart2 Then
                    'unhandled error occured
                    err.ErrorMessage = err.ErrorNumber & ";" & err.ErrorMessage
                    err.ErrorNumber = "VGPERR-3DPPT-002"
                    ProcessError(err)
                Else
                    'process 3d secure
                    Dim Is3DSecureSessionSet As Boolean = False
                    Is3DSecureSessionSet = TEBUtilities.SetVGAttributesSession(talVanguard.VanguardAttributes.BasketPaymentID, talVanguard.VanguardAttributes, talVanguard.BasketPayEntity)
                    Response.Redirect("~/PagesLogin/Checkout/checkout3dSecure.aspx")
                End If
            End If
        Else
            err.ErrorMessage = err.ErrorNumber & ";" & err.ErrorMessage
            err.ErrorNumber = "VGPERR-3DPPT-001"
            ProcessError(err)
        End If
    End Sub

    Private Function SetOtherAttributes(ByVal is3DSecure As Boolean, ByVal vgAttributes As DEVanguard) As DEVanguard
        If vgAttributes IsNot Nothing Then
            vgAttributes = SetAddressDetails(vgAttributes)
            If is3DSecure Then
                vgAttributes = Set3dSecureAttributes(vgAttributes)
            End If
        End If
        Return vgAttributes
    End Function

    Private Function SetAddressDetails(ByVal vgAttributes As DEVanguard) As DEVanguard
        If Profile.IsAnonymous Then
            vgAttributes.AddressLine1 = ""
            vgAttributes.Postcode = ""
        Else
            Dim registrationAddress As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)
            vgAttributes.AddressLine1 = registrationAddress.Address_Line_2
            vgAttributes.Postcode = registrationAddress.Post_Code
        End If
        Return vgAttributes
    End Function

    Private Function Set3dSecureAttributes(ByVal vgAttributes As DEVanguard) As DEVanguard
        If _moduleDefaultsValue.Payment3DSecureProvider = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
            vgAttributes.MKAccountID = _moduleDefaultsValue.Payment3DSecureDetails1
            vgAttributes.MKAcquirerId = _moduleDefaultsValue.Payment3DSecureDetails2
            vgAttributes.MerchantName = _moduleDefaultsValue.Payment3DSecureDetails3
            vgAttributes.MerchantUrl = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.LastIndexOf("Redirect"))
            vgAttributes.VisaMerchantBankId = _moduleDefaultsValue.Payment3DSecureDetails4
            vgAttributes.VisaMerchantNumber = _moduleDefaultsValue.Payment3DSecureDetails5
            vgAttributes.VisaMerchantPassword = _moduleDefaultsValue.Payment3DSecureDetails6
            vgAttributes.MCMMerchantBankId = _moduleDefaultsValue.Payment3DSecureDetails7
            vgAttributes.MCMMerchantNumber = _moduleDefaultsValue.Payment3DSecureDetails8
            vgAttributes.MCMMerchantPassword = _moduleDefaultsValue.Payment3DSecureDetails9
        End If
        Return vgAttributes
    End Function

    Private Sub RedirectForPPS1(ByVal basketPayEntity As DEBasketPayment)
        SetPayDetailSession(basketPayEntity, GlobalConstants.PPS1STAGE)
        Session("VGPaidPPS1") = True
        Session("PPS1PaymentComplete") = True 'decides payment completed or not
        Session("VGPPSRedirect") = "STEP1"
        JSRedirect("PagesLogin/Checkout/checkout.aspx?_rvgpps=pps1")
    End Sub

    Private Sub RedirectForPPS2(ByVal basketPayEntity As DEBasketPayment)
        SetPayDetailSession(basketPayEntity, GlobalConstants.PPS2STAGE)
        Session("VGPaidPPS2") = True
        Session("PPS2PaymentComplete") = True 'decides payment completed or not
        Session("VGPPSRedirect") = "STEP1"
        JSRedirect("PagesLogin/Checkout/checkout.aspx?_rvgpps=pps2")
    End Sub

    Private Sub ProcessSuccessSaveMyCard(ByVal vgAttributes As DEVanguard, ByVal basketPayEntity As DEBasketPayment)
        If IsCardSavedInBackend(basketPayEntity) Then
            RemoveSessions()
            Session("VG_SAVEMYCARD") = True
            JSRedirect("PagesLogin/Profile/SaveMyCard.aspx?_vgsmc=true")
        Else
            Dim err As New ErrorObj
            err.ErrorNumber = "VGPERR-SMC-001"
            ProcessError(err)
        End If
    End Sub

    Private Sub ProcessSuccessAmendPPSCard(ByVal vgAttributes As DEVanguard, ByVal basketPayEntity As DEBasketPayment)
        Dim dePayments As DEPayments = GetPaymentEntity(basketPayEntity)
        Session("DEPaymentsFromExternal") = dePayments
        If Session("DEPaymentsFromExternal") IsNot Nothing Then
            RemoveSessions()
            Session("VG_AMENDPPSCARD") = True
            JSRedirect("PagesPublic/ProductBrowse/AmendPPSPayments.aspx?_vgapc=true")
        Else
            Dim err As New ErrorObj
            err.ErrorNumber = "VGPERR-APC-001"
            ProcessError(err)
        End If
    End Sub

    Private Sub ProcessSuccessPayment(ByVal vgAttributes As DEVanguard, ByVal basketPayEntity As DEBasketPayment)
        Dim redirectURL As New StringBuilder
        Session("ExtPaymentAmount") = basketPayEntity.PAYMENT_AMOUNT
        Session("ExtPaymentReferenceNo") = vgAttributes.TransactionID
        SetPayDetailSession(basketPayEntity, GlobalConstants.CHECKOUTASPXSTAGE)
        Select Case BasketContentTypeOverride()
            Case "T", "C"
                Dim ticketGatewayFunc As New TicketingGatewayFunctions
                RemoveSessions()
                ticketGatewayFunc.CheckoutExternalSuccess(False, Profile.Basket.Basket_Header_ID, True, String.Empty)
                If Session("TalentPaymentReference") IsNot Nothing Then
                    redirectURL.Append("PagesLogin/Checkout/CheckoutOrderConfirmation.aspx?")
                    redirectURL.Append("&PaymentRef=").Append(Session("TalentPaymentReference").ToString.Trim)
                    redirectURL.Append("&paymentType=").Append(Session("ExternalGatewayPayType").ToString())
                    redirectURL.Append("&paymentAmount=").Append(Session("ExtPaymentAmount"))
                Else
                    'looks like some error
                    If Session("TalentErrorCode") IsNot Nothing AndAlso Session("TalentErrorCode").ToString.Trim.Length > 0 Then
                        redirectURL.Append("PagesPublic/Basket/basket.aspx")
                    End If
                End If
                JSRedirect(redirectURL.ToString())
            Case Else
                Dim order As New Order()
                order.ProcessMerchandiseInBackend(False, False, String.Empty, False)
                redirectURL.Append("../../PagesLogin/Checkout/CheckoutOrderConfirmation.aspx")
                redirectURL.Append("?PaymentRef=")
                If order.MerchandiseOrderReference IsNot Nothing AndAlso order.MerchandiseOrderReference.Length > 0 Then
                    redirectURL.Append(order.MerchandiseOrderReference)
                Else
                    If Session("ExtPaymentRefereceNo") IsNot Nothing Then
                        redirectURL.Append(Session("ExtPaymentReferenceNo").ToString())
                    End If
                End If
                redirectURL.Append("&paymentType=").Append(Session("ExternalGatewayPayType").ToString())
                redirectURL.Append("&paymentAmount=").Append(basketPayEntity.PAYMENT_AMOUNT)
                RemoveSessions()
                JSRedirect(redirectURL.ToString())
        End Select
    End Sub

    Private Sub ProcessSuccessPartPayment(ByVal vgAttributes As DEVanguard, ByVal basketPayEntity As DEBasketPayment)
        Dim redirectURL As String = String.Empty
        Session("ExtPaymentAmount") = basketPayEntity.PAYMENT_AMOUNT
        Session("ExtPaymentReferenceNo") = vgAttributes.TransactionID
        SetPayDetailSession(basketPayEntity, GlobalConstants.CHECKOUTASPXSTAGE)
        Select Case BasketContentTypeOverride()
            Case "T", "C"
                Dim returnURL As String = String.Empty
                Dim ticketGatewayFunc As New TicketingGatewayFunctions
                RemoveSessions()
                ticketGatewayFunc.CheckoutExternalSuccess(False, Profile.Basket.Basket_Header_ID, True, String.Empty, basketPayEntity.PAYMENT_AMOUNT)
                If Session("TalentErrorCode") IsNot Nothing AndAlso Session("TalentErrorCode").ToString.Trim.Length > 0 Then
                    returnURL = "PagesPublic/Basket/basket.aspx"
                Else
                    returnURL = "PagesLogin/Checkout/checkout.aspx"
                End If
                JSRedirect(returnURL)
            Case Else
                Dim order As New Order()
                order.ProcessMerchandiseInBackend(False, False, String.Empty, False)
                redirectURL = "~/PagesLogin/Checkout/CheckoutOrderConfirmation.aspx"
                redirectURL = redirectURL & "?PaymentRef=" & Session("ExtPaymentReferenceNo").ToString()
                redirectURL = redirectURL & "&paymentType=" & Session("ExternalGatewayPayType").ToString()
                redirectURL = redirectURL & "&paymentAmount=" & basketPayEntity.PAYMENT_AMOUNT
                RemoveSessions()
                Response.Redirect(redirectURL)
        End Select
    End Sub

    Private Sub ProcessFailurePayment(ByVal errorCode As String)
        Dim redirectURL As String = String.Empty
        RemoveSessions()
        Dim errMsg As New Talent.Common.TalentErrorMessages(_languageCode, _
                                        TalentCache.GetBusinessUnitGroup, _
                                        TalentCache.GetPartner(Profile), _
                                        ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
        Session("GatewayErrorMessage") = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
        Talent.eCommerce.Utilities.GetCurrentPageName, errorCode).ERROR_MESSAGE
        Select Case Talent.eCommerce.Utilities.BasketContentTypeWithOverride
            Case GlobalConstants.TICKETINGBASKETCONTENTTYPE, GlobalConstants.COMBINEDBASKETCONTENTTYPE
                Dim ticketGatewayFunc As New TicketingGatewayFunctions
                ticketGatewayFunc.CheckoutExternalFailure(False, Profile.Basket.Basket_Header_ID, False)
        End Select
    End Sub

    Private Sub LogGeneralError(ByVal errorCode As String, ByVal errorMessage As String)
        Dim errorMsg As String = String.Empty
        Try
            errorMsg = errorMsg & "Referrer: " & Request.UrlReferrer.AbsoluteUri
        Catch ex As Exception
            errorMsg = errorMsg & "Referrer: " & ex.Message
        End Try
        Try
            errorMsg = errorMsg & "; Requested Url: " & HttpContext.Current.Request.Url.AbsoluteUri
            errorMsg = errorMsg & "; Login Id: " & Profile.UserName
            errorMsg = errorMsg & "; Basket Header Id: " & Profile.Basket.Basket_Header_ID
            errorMsg = errorMsg & "; Temp Order Id: " & Profile.Basket.TempOrderID
            Dim talentLogging As New Talent.Common.TalentLogging
            talentLogging.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            talentLogging.GeneralLog("VanguardGateway.aspx", errorCode, errorMessage & " : " & errorMsg, "VGErrorLog")
        Catch ex As Exception
            errorMsg = errorMsg & ex.Message
        End Try
    End Sub

    Private Sub ProcessError(ByVal err As ErrorObj, ByVal ex As Exception)
        Select Case err.ErrorNumber
            Case "VGGEX-001"
                'exception in processvanguard call
                ProcessFailurePayment("VGGERR")
                JSRedirect("PagesLogin/Checkout/checkout.aspx")
            Case Else

        End Select

    End Sub

    Private Sub ProcessError(ByVal err As ErrorObj)
        Select Case err.ErrorNumber
            Case "VGGERR-SM"
                'vanguard attributes session are missing
                ProcessFailurePayment("VGGERR")
                LogGeneralError(err.ErrorNumber, err.ErrorMessage)
                JSRedirect("PagesPublic/Basket/basket.aspx")
            Case "VGGERR-002"
                'invalid request
                ProcessFailurePayment("VGGERR")
                LogGeneralError(err.ErrorNumber, err.ErrorMessage)
                JSRedirect("PagesPublic/Basket/basket.aspx")
            Case "VGPERR-SMC-001"
                LogGeneralError(err.ErrorNumber, err.ErrorMessage)
                Session("VG_SAVEMYCARD") = False
                JSRedirect("PagesLogin/Profile/SaveMyCard.aspx?_vgsmc=false")
            Case Else
                ProcessFailurePayment("VGGERR")
                LogGeneralError(err.ErrorNumber, err.ErrorMessage)
                JSRedirect("PagesLogin/Checkout/checkout.aspx")
        End Select
    End Sub

    Private Sub RemoveSessions()
        Session.Remove("VanguardStage")
        If HttpContext.Current.Session("VG_BasketPaymentID") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(HttpContext.Current.Session("VG_BasketPaymentID")) Then
            'remove those session before instantiate new one
            If HttpContext.Current.Session("VG_VGAttributes_" & HttpContext.Current.Session("VG_BasketPaymentID").ToString()) IsNot Nothing Then
                HttpContext.Current.Session.Remove("VG_VGAttributes_" & HttpContext.Current.Session("VG_BasketPaymentID").ToString())
                HttpContext.Current.Session.Remove("VG_VGBasketPayment_" & HttpContext.Current.Session("VG_BasketPaymentID").ToString())
            End If
        End If
    End Sub

    Private Function CanProcessRequest() As Boolean
        Dim isValid As Boolean = False
        If IsValidSession() Then
            If IsValidRequest() Then
                isValid = True
            End If
        End If

        'isvalid is true if we came from 3dsecure
        If Not isValid Then
            If Session("VG_FROM_3DSECURE") IsNot Nothing AndAlso Convert.ToBoolean(Session("VG_FROM_3DSECURE")) = True Then
                Dim basketPaymentID As Long = 0
                Dim vgAttributes As DEVanguard = Nothing
                Dim basketPayEntity As DEBasketPayment = Nothing
                If TEBUtilities.TryGetVGAttributesSession(basketPaymentID, vgAttributes, basketPayEntity) Then
                    _currentProcessStage = vgAttributes.CheckOutStage
                End If
                'If this session is not nothing andalso it is true then we are going to process 3dsecure on transaction part 2
                isValid = True
            End If
        End If

        Return isValid
    End Function

    Private Function IsValidSession() As Boolean
        Dim isValid As Boolean = True
        If HttpContext.Current.Session Is Nothing Then
            isValid = False
        ElseIf Session("VanguardStage") Is Nothing Then
            _isSessionMissing = True
            isValid = False
        ElseIf Session("VanguardStage") <> _expectedStageValue Then
            isValid = False
        End If
        Return isValid
    End Function

    Private Function IsValidRequest() As Boolean
        Dim isValid As Boolean = True
        If String.IsNullOrWhiteSpace(Request.QueryString("_utcr")) Then
            isValid = False
        ElseIf String.IsNullOrWhiteSpace(Request.QueryString("_ubpcr")) Then
            isValid = False
        ElseIf String.IsNullOrWhiteSpace(Request.QueryString("_ubhcr")) Then
            isValid = False
        ElseIf String.IsNullOrWhiteSpace(Request.QueryString("_ucos")) Then
            isValid = False
        ElseIf String.IsNullOrWhiteSpace(Request.QueryString("_uspiii")) Then
            isValid = False
        ElseIf Request.QueryString("_ubpcr") <> Session("VG_BasketPaymentID") Then
            isValid = False
        ElseIf Request.QueryString("_uspiii") <> Session.SessionID Then
            isValid = False
        Else
            'all querystring exists now set the stage
            Dim basketPaymentID As Long = 0
            Dim vgAttributes As DEVanguard = Nothing
            Dim basketPayEntity As DEBasketPayment = Nothing
            If TEBUtilities.TryGetVGAttributesSession(basketPaymentID, vgAttributes, basketPayEntity) Then
                If Request.QueryString("_ucos") <> vgAttributes.CheckOutStage Then
                    isValid = False
                Else
                    _currentProcessStage = vgAttributes.CheckOutStage
                End If
            Else
                _isSessionMissing = True
                isValid = False
            End If
        End If
        Return isValid
    End Function

    Private Sub SetPayDetailSession(ByVal basketPayEntity As DEBasketPayment, ByVal payStage As String)
        If basketPayEntity.PAYMENT_TYPE = GlobalConstants.CCPAYMENTTYPE Then
            Checkout.StoreCCDetails(basketPayEntity.CARD_TYPE, _
                        basketPayEntity.CARDNUMBER, _
                        basketPayEntity.EXPIRYMONTH & basketPayEntity.EXPIRYYEAR, _
                        basketPayEntity.STARTMONTH & basketPayEntity.STARTYEAR, _
                        basketPayEntity.ISSUENUMBER, _
                        "", _
                        basketPayEntity.CARD_HOLDER_NAME, _
                        "0", basketPayEntity.TOKENID, basketPayEntity.PROCESSING_DB, basketPayEntity.EXPIRYMONTH & basketPayEntity.EXPIRYYEAR, basketPayEntity.TRANSACTION_ID, payStage, basketPayEntity.CAN_SAVE_CARD)

        ElseIf basketPayEntity.PAYMENT_TYPE = GlobalConstants.SAVEDCARDPAYMENTTYPE Then
            Checkout.StoreSCDetails(basketPayEntity.CARDNUMBER, basketPayEntity.EXPIRYMONTH & basketPayEntity.EXPIRYYEAR, basketPayEntity.STARTMONTH & basketPayEntity.STARTYEAR, _
                                    basketPayEntity.ISSUENUMBER, "", _
                                        String.Empty, basketPayEntity.SAVEDCARD_UNIQUEID, basketPayEntity.TOKENID, basketPayEntity.PROCESSING_DB, basketPayEntity.EXPIRYMONTH & basketPayEntity.EXPIRYYEAR, basketPayEntity.TRANSACTION_ID, payStage)
        End If
    End Sub

    Private Function BasketContentTypeOverride() As String
        Dim basketContentType As String = Profile.Basket.BasketContentType
        If Talent.eCommerce.Utilities.IsTicketingDBforRetailOrders() Then
            basketContentType = GlobalConstants.TICKETINGBASKETCONTENTTYPE
        End If
        Return basketContentType
    End Function

    Private Sub JSRedirect(ByVal returnURL As String)
        Dim currentUrl As String = TEBUtilities.FormatSSLOffloadedURL()
        returnURL = currentUrl.Substring(0, currentUrl.IndexOf("Redirect")) & returnURL
        If _isJavaScriptRedirect Then
            Dim jsScript As String = "<script language=JavaScript>"
            jsScript += " top.location = '" & returnURL & "';"
            jsScript += "</script>"
            Page.ClientScript.RegisterStartupScript(GetType(String), "redirectScript", jsScript)
        Else
            'Use standard method
            Response.Redirect(returnURL)
        End If
    End Sub

    Private Function GetPaymentEntity(ByVal baskeyPayEntity As DEBasketPayment) As DEPayments
        Dim dePayment As New DEPayments
        With dePayment
            .PaymentType = GlobalConstants.PAYMENTTYPE_VANGUARD
            .CardType = baskeyPayEntity.CARD_TYPE
            .CardNumber = baskeyPayEntity.CARDNUMBER
            .StartDate = TCUtilities.PadLeadingZeros(GetFormattedDate(0, baskeyPayEntity.STARTMONTH, baskeyPayEntity.STARTYEAR), 4)
            .ExpiryDate = GetFormattedDate(0, baskeyPayEntity.EXPIRYMONTH, baskeyPayEntity.EXPIRYYEAR)
            .TokenDate = GetFormattedDate(1, baskeyPayEntity.EXPIRYMONTH, baskeyPayEntity.EXPIRYYEAR)
            .TokenID = baskeyPayEntity.TOKENID
            .ProcessingDB = baskeyPayEntity.PROCESSING_DB
            .CustomerNumber = Profile.UserName
            .SessionId = Profile.Basket.Basket_Header_ID
            .Source = "W"
        End With
        Return dePayment
    End Function

    Private Function IsCardSavedInBackend(ByVal baskeyPayEntity As DEBasketPayment) As Boolean
        Dim err As New ErrorObj
        Dim payment As New TalentPayment
        Dim hasCardBeenSaved As Boolean = False
        payment.De = GetPaymentEntity(baskeyPayEntity)
        payment.Settings = TEBUtilities.GetSettingsObject()
        err = payment.SaveMyCard

        Try
            If Not err.HasError Then
                If payment.ResultDataSet.Tables.Count > 1 Then
                    If payment.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        If String.IsNullOrWhiteSpace(payment.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString) Then
                            hasCardBeenSaved = True
                            payment.RetrieveMySavedCardsClearCache()
                        End If
                    Else
                        hasCardBeenSaved = True
                        payment.RetrieveMySavedCardsClearCache()
                    End If
                End If
            End If
        Catch ex As Exception
            hasCardBeenSaved = False
        End Try

        Return hasCardBeenSaved
    End Function

    Private Function GetFormattedDate(ByVal mode As Integer, ByVal monthValue As String, ByVal yearValue As String) As String
        Dim formattedvalue As String = "0"
        Try
            If (Not String.IsNullOrWhiteSpace(monthValue)) AndAlso (Not String.IsNullOrWhiteSpace(yearValue)) Then
                If IsNumeric(monthValue) AndAlso IsNumeric(yearValue) Then
                    'Utilities.PadLeadingZeros(fromDate.Month, 2)
                    If mode = 0 Then
                        formattedvalue = TCUtilities.PadLeadingZeros(monthValue, 2) & TCUtilities.PadLeadingZeros(yearValue, 2)
                    ElseIf mode = 1 Then
                        Dim expirydate As Date = Nothing
                        If Now.Year > CInt("20" & yearValue) Then
                            expirydate = New Date(CInt("21" & yearValue), monthValue, System.DateTime.DaysInMonth("21" & yearValue, monthValue))
                        Else
                            expirydate = New Date(CInt("20" & yearValue), monthValue, System.DateTime.DaysInMonth("20" & yearValue, monthValue))
                        End If
                        formattedvalue = TCUtilities.DateToIseriesFormat(expirydate)
                    End If
                End If
            End If
        Catch ex As Exception
            formattedvalue = "0"
        End Try

        Return formattedvalue
    End Function

    Private Function CanProcess3DSecure() As Boolean
        Dim isOn As Boolean = False
        isOn = Checkout.is3DSecureInUse(TEBUtilities.IsAgent, BasketContentTypeOverride(), _moduleDefaultsValue)
        Return isOn
    End Function

#End Region

End Class
