Imports Microsoft.VisualBasic
Imports System.Data
Imports System.IO
Imports System.Xml
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports Talent.Common
Imports GCheckout.Checkout
Imports GCheckout.Util
Imports GCheckout.OrderProcessing

Public Class GoogleCheckout

#Region "Class Level Members"

    'merchant private data constants
    'headerid, temporderid, loginid, username, totalitems, baskettotal, agentname
    Private Const MERC_PRIV_DATA_HEADER_ID As String = "headerid"
    Private Const MERC_PRIV_DATA_TEMP_ORDER_ID As String = "temporderid"
    Private Const MERC_PRIV_DATA_LOGIN_ID As String = "loginid"
    Private Const MERC_PRIV_DATA_USERNAME As String = "username"
    Private Const MERC_PRIV_DATA_TOTAL_ITEMS As String = "totalitems"
    Private Const MERC_PRIV_DATA_BASKET_TOTAL As String = "baskettotal"
    Private Const MERC_PRIV_DATA_AGENT_NAME As String = "agentname"

    'serial number status
    Private Const SERIAL_NUMBER_RECEIVED As String = "RECEIVED"
    Private Const SERIAL_NUMBER_ORDER_STATUS As String = "ORDER STATUS XML RECEIVED"
    Private Const SERIAL_NUMBER_PROCESS_START As String = "PROCESS STARTED"
    Private Const SERIAL_NUMBER_COMPLETED As String = "COMPLETED"
    Private Const SERIAL_NUMBER_FAILED As String = "FAILED"
    Private Const SERIAL_NUMBER_LOGGED As String = "LOGGGED"

    'cancel reason
    Private Const CANCEL_NEWORDER_AS_FBM As String = "CANCEL_NEWORDER_AS_FRONTEND_BASKET_MISSING"
    Private Const CANCEL_NEWORDER_AS_BBM As String = "CANCEL_NEWORDER_AS_BACKEND_BASKET_MISSING"
    Private Const CANCEL_AMTAUTH_AS_FBM As String = "CANCEL_AMTAUTH_AS_FRONTEND_BASKET_MISSING"
    Private Const CANCEL_AMTAUTH_AS_BBM As String = "CANCEL_AMTAUTH_AS_BACKEND_BASKET_MISSING"
    Private Const CANCEL_AMTAUTH_AS_RISK As String = "CANCEL_AMTAUTH_AS_RISKY_PAYMENT"
    Private Const CANCEL_AMTAUTH_AS_CASRF As String = "CANCEL_AMTAUTH_AS_CHARGEANDSHIPREQUEST_FAILED"
    Private Const CANCEL_AMTAUTH_AS_OC As String = "CANCEL_AMTAUTH_AS_ORDER_CANCELLED"
    Private Const CANCEL_AMTAUTH_AS_PAYDEC As String = "CANCEL_AMTAUTH_AS_PAYMENT_DECLINED"

    Private Const BACKEND_BASKET_VERIFY_MODE_U = "U" 'update the external order number to basket
    Private Const BACKEND_BASKET_VERIFY_MODE_V = "V"    'verify the basket existence
    'verify the basket existence before charging so backend will delay the monitor to clear the ticket
    Private Const BACKEND_BASKET_VERIFY_MODE_A = "A"

    'GCGERR - Google checkout gateway page error - error from this page
    'GCERR - Google checkout error - error may be from google
    Private _defaultsValue As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Private _wfr As WebFormResource = Nothing
    Private _businessUnit As String = String.Empty
    Private _partnerCode As String = String.Empty
    Private _languageCode As String = String.Empty
    Private _talentLogging As TalentLogging = Nothing
    Private _talentDataObj As TalentDataObjects = Nothing

    'Basket related members
    Private _basketTotal As Decimal = 0 'balance after any part payments, cashback (onaccount) etc.,

    'GC related members
    Private _merchantID As String = String.Empty
    Private _merchantKey As String = String.Empty
    Private _checkoutURL As String = String.Empty
    Private _retunURL As String = String.Empty
    Private _currency As String = String.Empty
    Private _apiVersion As String = String.Empty
    Private _cartExpirationMinutes As Integer = 10
    Private _environmentType As GCheckout.EnvironmentType = GCheckout.EnvironmentType.Sandbox
    Private _isTalentLogEnabled As Boolean = False
    Private _logNotificationXML As Boolean = False
    Private _riskInfoAllowedAVSResponses As String = String.Empty
    Private _riskInfoAllowedCVNResponses As String = String.Empty
    'Y - Full AVS match (address and postal code)
    'P - Partial AVS match (postal code only)
    'A - Partial AVS match (address only)
    'N - no AVS match
    'U - AVS not supported by issuer
    'M - CVN match
    'N - No CVN match
    'U - CVN not available
    'E - CVN error
#End Region

#Region "Constructor"

    Public Sub New(ByVal pageName As String)
        _wfr = New Talent.Common.WebFormResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _talentLogging = New Talent.Common.TalentLogging
        _talentLogging.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        _businessUnit = TalentCache.GetBusinessUnit()
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        With _wfr
            .BusinessUnit = _businessUnit
            .PageCode = pageName
            .PartnerCode = _partnerCode
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "GoogleCheckout.vb"
        End With
        _defaultsValue = (New Talent.eCommerce.ECommerceModuleDefaults()).GetDefaults
        _talentDataObj = New TalentDataObjects
        _talentDataObj.Settings = TEBUtilities.GetSettingsObject()
        AssignConfigurations()
    End Sub

#End Region

#Region "Public Methods"

    Public Sub ProcessGoogleCheckout()
        ProcessBasketCheckout()
    End Sub

    Public Sub ProcessGoogleNotifications()
        ProcessGoogleOrderProcessing()
    End Sub

#End Region

#Region "Private Methods"

    Private Sub AssignConfigurations()
        Try
            If TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_wfr.Attribute("IsSandbox")) Then
                'test environment
                _merchantID = TEBUtilities.CheckForDBNull_String(_wfr.Attribute("SandboxMerchantID"))
                _merchantKey = TEBUtilities.CheckForDBNull_String(_wfr.Attribute("SandboxMerchantKey"))
                _checkoutURL = TEBUtilities.CheckForDBNull_String(_wfr.Attribute("SandboxURLGoogleCheckout")).Trim
                _retunURL = TEBUtilities.CheckForDBNull_String(_wfr.Attribute("SandboxURLReturn")).Trim
                _environmentType = GCheckout.EnvironmentType.Sandbox

            Else
                'live environment
                _merchantID = TEBUtilities.CheckForDBNull_String(_wfr.Attribute("LiveMerchantID"))
                _merchantKey = TEBUtilities.CheckForDBNull_String(_wfr.Attribute("LiveMerchantKey"))
                _checkoutURL = TEBUtilities.CheckForDBNull_String(_wfr.Attribute("LiveURLGoogleCheckout")).Trim
                _retunURL = TEBUtilities.CheckForDBNull_String(_wfr.Attribute("LiveURLReturn")).Trim
                _environmentType = GCheckout.EnvironmentType.Production
            End If
            'common configurations
            _currency = TEBUtilities.CheckForDBNull_String(_wfr.Attribute("Currency")).Trim
            _apiVersion = TEBUtilities.CheckForDBNull_String(_wfr.Attribute("APIVersion")).Trim
            _riskInfoAllowedAVSResponses = TEBUtilities.CheckForDBNull_String(_wfr.Attribute("AllowedAVSResponses")).Trim
            _riskInfoAllowedCVNResponses = TEBUtilities.CheckForDBNull_String(_wfr.Attribute("AllowedCVNResponses")).Trim
            _isTalentLogEnabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfr.Attribute("IsTalentLogEnabled"))
            _logNotificationXML = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfr.Attribute("LogNotificationXML"))
            If TEBUtilities.CheckForDBNull_Int(_wfr.Attribute("CartExpirationMinutes")) > 0 Then
                _cartExpirationMinutes = TEBUtilities.CheckForDBNull_Int(_wfr.Attribute("CartExpirationMinutes"))
            End If
        Catch ex As Exception
            ProcessError("GCGERR-EX-CONFIG", ex)
        End Try
    End Sub

#Region "Google Checkout Private Methods"

    Private Sub InitialiseCheckout()
        'Session("ExternalGatewayPayType") = GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE
        'set this session in checkout page on button click and redirect the user to this page
        'check this session exists if not redirect the page to external gateway error page
        LoggingGoogleCheckoutProcess("001", "InitialiseCheckout")
        If HttpContext.Current.Session("ExternalGatewayPayType") Is Nothing Then
            'unauthorised page access
            ProcessError("GCGERR-UNAUTH")
        ElseIf HttpContext.Current.Session("ExternalGatewayPayType") IsNot Nothing AndAlso HttpContext.Current.Session("ExternalGatewayPayType") <> GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE Then
            'unauthorised page access
            ProcessError("GCGERR-UNAUTH")
        Else
            'authorised page access
            HttpContext.Current.Session.Remove("ExternalGatewayPayType")
        End If

        Try
            If HttpContext.Current.Profile.IsAnonymous Then
                ProcessError("GCGERR-ANONYMOUS")
            Else
                UpdateBasketAndOrderStatus("PAYMENT ATTEMPTED", "Google Checkout Payment Process Started; Basket Content Type : " & CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType, "", "")
                Dim referrerUrl As String = String.Empty
                Try
                    referrerUrl = HttpContext.Current.Request.UrlReferrer.AbsoluteUri
                Catch ex As Exception
                    referrerUrl = ex.Message
                End Try
                Try
                    _basketTotal = HttpContext.Current.Session("TicketingBasektSummaryTotalValue")
                Catch ex As Exception
                    ProcessError("GCGERR-EX-BASKETTOTAL", ex)
                End Try
                If _basketTotal <= 0 Then
                    ProcessError("GCGERR-BASKETZERO")
                End If
                _talentLogging.GeneralLog(_wfr.PageCode, "Page_Init", "Customer: " & CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID & ", Referrer URL: " & referrerUrl, "GoogleCheckoutLogging")
            End If
        Catch ex As Exception
            ProcessError("GCGERR-EX-PAGEINIT")
        End Try
    End Sub

    Private Sub ProcessBasketCheckout()
        InitialiseCheckout()
        Dim canRedirect As Boolean = False
        Dim googleRedirectURL As String = String.Empty
        Try
            LoggingGoogleCheckoutProcess("002", "ProcessBasketCheckout; " & _wfr.BusinessUnit & ";" & _wfr.PartnerCode & ";" & _wfr.PageCode & ";" & _wfr.KeyCode & ";" & _wfr.FrontEndConnectionString)
            Dim gcRequest As New CheckoutShoppingCartRequest(_merchantID, _merchantKey, _environmentType, _currency, _cartExpirationMinutes)
            PopulateMerchantPrivateData(gcRequest)
            Dim productDisplayName As String = TEBUtilities.CheckForDBNull_String(_wfr.Content("ProductNameInGoogle", _languageCode, True)).Trim
            Dim productDisplayDescription As String = TEBUtilities.CheckForDBNull_String(_wfr.Content("ProductDescriptionInGoogle", _languageCode, True)).Trim
            If String.IsNullOrWhiteSpace(productDisplayName) Then
                productDisplayName = "Tickets Name"
            End If
            If String.IsNullOrWhiteSpace(productDisplayDescription) Then
                productDisplayDescription = "Tickets Description"
            End If
            gcRequest.AddItem(productDisplayName, productDisplayDescription, _basketTotal, 1)
            Dim gcResponse As GCheckoutResponse = gcRequest.Send
            Dim commentDetails As String = String.Empty

            If gcResponse.IsGood Then
                'response is good update basket status
                commentDetails += "Redirect URL:" & gcResponse.RedirectUrl
                commentDetails += "Serial Number:" & gcResponse.SerialNumber
                commentDetails += "Response XML:" & gcResponse.ResponseXml
                If UpdateBasketAndOrderStatus("PAYMENT INTERMEDIATE 1", commentDetails, "", gcResponse.SerialNumber) Then
                    'mark backend basket as payment pending
                    MarkBackendBasketAsPayPending()
                    'before mark the frontend basket as processed clear the cache which has to be cleared in confirmation page
                    Try
                        TEBUtilities.ClearOtherCacheItems()
                    Catch ex As Exception

                    End Try
                    'mark frontend basket as processed
                    MarkFrontendBasketAsProcessed()
                    googleRedirectURL = gcResponse.RedirectUrl
                    canRedirect = True
                End If
            Else
                'error in xml update basket status as payment rejected
                'gcResponse.ErrorMessage
                commentDetails += "Error Message:" & gcResponse.ErrorMessage
                commentDetails += "Redirect URL:" & gcResponse.RedirectUrl
                commentDetails += "Serial Number:" & gcResponse.SerialNumber
                commentDetails += "Response XML:" & gcResponse.ResponseXml
                UpdateBasketAndOrderStatus("PAYMENT REJECTED", commentDetails, "", gcResponse.SerialNumber)
                ProcessError("GCERR-INVBASKET", commentDetails)
            End If
        Catch ex As Exception
            ProcessError("GCERR-PGC", ex) 'process google checkout exception
        End Try
        If canRedirect Then
            'everything is success now call google checkout redirect page
            LoggingGoogleCheckoutProcess("007", "ProcessBasketCheckout - Success")
            HttpContext.Current.Response.Redirect(googleRedirectURL)
        End If

    End Sub

    Private Sub PopulateMerchantPrivateData(ByRef gcRequest As CheckoutShoppingCartRequest)
        LoggingGoogleCheckoutProcess("003", "PopulateMerchantPrivateData Starts")
        'add merchant private data
        Dim merchantPrivateDataDoc As New System.Xml.XmlDocument()
        'basket header id
        Dim basketHeaderIdNode As System.Xml.XmlNode = merchantPrivateDataDoc.CreateElement(MERC_PRIV_DATA_HEADER_ID)
        basketHeaderIdNode.InnerText = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID.ToString
        gcRequest.AddMerchantPrivateDataNode(basketHeaderIdNode)
        'temp order id
        Dim basketTempOrderIdNode As System.Xml.XmlNode = merchantPrivateDataDoc.CreateElement(MERC_PRIV_DATA_TEMP_ORDER_ID)
        basketTempOrderIdNode.InnerText = CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID
        gcRequest.AddMerchantPrivateDataNode(basketTempOrderIdNode)
        'login id
        Dim basketLoginIdNode As System.Xml.XmlNode = merchantPrivateDataDoc.CreateElement(MERC_PRIV_DATA_LOGIN_ID)
        basketLoginIdNode.InnerText = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
        gcRequest.AddMerchantPrivateDataNode(basketLoginIdNode)
        'username
        Dim profileUsernameNode As System.Xml.XmlNode = merchantPrivateDataDoc.CreateElement(MERC_PRIV_DATA_USERNAME)
        profileUsernameNode.InnerText = HttpContext.Current.Profile.UserName
        gcRequest.AddMerchantPrivateDataNode(profileUsernameNode)
        'basket total items including fees
        Dim basketTotalItemsNode As System.Xml.XmlNode = merchantPrivateDataDoc.CreateElement(MERC_PRIV_DATA_TOTAL_ITEMS)
        basketTotalItemsNode.InnerText = CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems.Count
        gcRequest.AddMerchantPrivateDataNode(basketTotalItemsNode)
        'basket total after any part payments
        Dim basketTotalNode As System.Xml.XmlNode = merchantPrivateDataDoc.CreateElement(MERC_PRIV_DATA_BASKET_TOTAL)
        basketTotalNode.InnerText = _basketTotal
        gcRequest.AddMerchantPrivateDataNode(basketTotalNode)
        'agent name
        Dim agentNameNode As System.Xml.XmlNode = merchantPrivateDataDoc.CreateElement(MERC_PRIV_DATA_AGENT_NAME)
        agentNameNode.InnerText = TEBUtilities.GetOriginatingSource(HttpContext.Current.Session.Item("Agent"))
        gcRequest.AddMerchantPrivateDataNode(agentNameNode)
        LoggingGoogleCheckoutProcess("004", "PopulateMerchantPrivateData Ends - " & gcRequest.MerchantPrivateData)
    End Sub

    Private Function UpdateBasketAndOrderStatus(ByVal status As String, ByVal comment As String, ByVal externalOrderNumber As String, ByVal googleSerialNumber As String) As Boolean
        Dim isInserted As Boolean = False
        Dim loginId As String = HttpContext.Current.Profile.UserName
        If CType(HttpContext.Current.Profile, TalentProfile).User IsNot Nothing AndAlso CType(HttpContext.Current.Profile, TalentProfile).User.Details IsNot Nothing Then
            loginId = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
        End If
        Dim basketStatusEntity As New DEBasketStatus
        basketStatusEntity.BusinessUnit = _businessUnit
        basketStatusEntity.Partner = _partnerCode
        basketStatusEntity.LoginId = loginId
        basketStatusEntity.BasketHeaderId = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
        basketStatusEntity.TempOrderId = CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID
        basketStatusEntity.Status = Talent.Common.Utilities.GetOrderStatus(status)
        basketStatusEntity.Comment = comment
        basketStatusEntity.GoogleSerialNumber = googleSerialNumber
        basketStatusEntity.ExternalOrderNumber = externalOrderNumber
        LoggingGoogleCheckoutProcess("005", "UpdateBasketAndOrderStatus", basketStatusEntity)
        _talentDataObj.BasketSettings.TblBasketStatus.BasketStatusEntity = basketStatusEntity
        Dim affectedRows As Integer = _talentDataObj.BasketSettings.TblBasketStatus.Insert()
        If affectedRows > 0 Then
            isInserted = True
        End If
        Return isInserted
    End Function

    Private Sub MarkBackendBasketAsPayPending()
        LoggingGoogleCheckoutProcess("006", "MarkBackendBasketAsPayPending")
        HttpContext.Current.Session("ExternalGatewayPayType") = GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE
        Select Case CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType
            Case GlobalConstants.TICKETINGBASKETCONTENTTYPE
                Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
                ticketingGatewayFunctions.CheckoutExternal(False)
            Case GlobalConstants.MERCHANDISEBASKETCONTENTTYPE
            Case GlobalConstants.COMBINEDBASKETCONTENTTYPE
        End Select
    End Sub

    Private Sub MarkFrontendBasketAsProcessed()
        Try
            LoggingGoogleCheckoutProcess("007", "MarkFrontendBasketAsProcessed")
            Dim basket As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
            For i As Integer = 0 To i = 3
                Try
                    basket.Mark_As_Processed(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
                    Exit For
                Catch ex As Exception
                    If i = 3 Then
                        ProcessError("GCGERR-EX-MFBAP", ex) 'mark front end basket as processed
                    Else
                        System.Threading.Thread.Sleep(50)
                    End If
                End Try
            Next
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ProcessError(ByVal errorCode As String)
        Dim errorRedirectURL As String = String.Empty
        Select Case errorCode
            Case "GCGERR-ANONYMOUS" 'Profile is anonymous
                errorRedirectURL = "~/PagesPublic/Basket/Basket.aspx"
                LoggingErrorPayment(errorCode, "PROFILE IS ANONYMOUS")
                AssignErrSessionAndRedirect(errorCode, errorRedirectURL)
            Case "GCGERR-BASKETZERO" 'Basket total is zero
                errorRedirectURL = "~/PagesPublic/Basket/Basket.aspx"
                AssignErrSessionAndRedirect(errorCode, errorRedirectURL)
            Case "GCGERR-UNAUTH" 'unauthorised gateway page access
                LoggingErrorPayment(errorCode, "SESSION STAGE ISSUE")
                HttpContext.Current.Response.Redirect("~/PagesPublic/Error/ExternalGatewayError.aspx?gatewayname=GOOGLECHECKOUT")
        End Select
    End Sub

    Private Sub ProcessError(ByVal errorCode As String, ByVal errorMsg As String)
        Dim errorRedirectURL As String = String.Empty
        Select Case errorCode
            Case "GCERR-INVBASKET" 'gc response as invalid basket
                errorRedirectURL = "~/PagesPublic/Basket/Basket.aspx"
                LoggingErrorPayment(errorCode, errorMsg)
                AssignErrSessionAndRedirect(errorCode, errorRedirectURL)
        End Select
    End Sub

    Private Sub ProcessError(ByVal errorCode As String, ByVal ex As Exception)
        Dim errorRedirectURL As String = String.Empty
        Select Case errorCode
            Case "GCGERR-EX-PAGEINIT" 'exception in page initiation
                errorRedirectURL = "~/PagesPublic/Basket/Basket.aspx"
                LoggingErrorPayment(errorCode, ex.Message & ";" & ex.StackTrace)
                AssignErrSessionAndRedirect(errorCode, errorRedirectURL)
            Case "GCGERR-EX-CONFIG" 'exception in assigning configurations
                errorRedirectURL = "~/PagesPublic/Basket/Basket.aspx"
                LoggingErrorPayment(errorCode, ex.Message & ";" & ex.StackTrace)
                AssignErrSessionAndRedirect(errorCode, errorRedirectURL)
            Case "GCGERR-EX-BASKETTOTAL" 'exception while getting basket total from session
                errorRedirectURL = "~/PagesPublic/Basket/Basket.aspx"
                LoggingErrorPayment(errorCode, ex.Message & ";" & ex.StackTrace)
                AssignErrSessionAndRedirect(errorCode, errorRedirectURL)
            Case "GCGERR-EX-MFBAP" 'exception while marking frontend basket as processed
                LoggingErrorPayment(errorCode, ex.Message & ";" & ex.StackTrace)
                'as backend basket already marked for pay pending mark that also as pay failure
                ProcessFailurePayment(errorCode & "; " & ex.Message)
            Case "GCERR-PGC" 'exception while processing google checkout
                errorRedirectURL = "~/PagesPublic/Basket/Basket.aspx"
                LoggingErrorPayment(errorCode, ex.Message & ";" & ex.StackTrace)
                AssignErrSessionAndRedirect(errorCode, errorRedirectURL)
        End Select
    End Sub

    Private Sub ProcessFailurePayment(ByVal commentDetails As String)
        Dim redirectURL As String = String.Empty
        UpdateBasketAndOrderStatus("PAYMENT REJECTED", commentDetails, "", "")
        HttpContext.Current.Session("ExternalGatewayPayType") = GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE
        Dim errorCode As String = "GCERR"
        Dim errMsg As New Talent.Common.TalentErrorMessages(_languageCode, _
                                        TalentCache.GetBusinessUnitGroup, _
                                        TalentCache.GetPartner(HttpContext.Current.Profile), _
                                        ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
        HttpContext.Current.Session("GatewayErrorMessage") = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
        Talent.eCommerce.Utilities.GetCurrentPageName, errorCode).ERROR_MESSAGE

        Select Case Talent.eCommerce.Utilities.BasketContentTypeWithOverride
            Case "T", "C"
                redirectURL = "~/Redirect/TicketingGateway.aspx?name=GOOGLECHECKOUT&page=checkoutpaymentdetails.aspx&function=paymentexternalfailure"
                HttpContext.Current.Response.Redirect(redirectURL)
            Case Else
                redirectURL = "~/PagesLogin/Checkout/checkout.aspx"
                HttpContext.Current.Response.Redirect(redirectURL)
        End Select
    End Sub

    Private Sub AssignErrSessionAndRedirect(ByVal errorMessageCode As String, ByVal redirectURL As String)
        HttpContext.Current.Session("TalentErrorCode") = ""
        HttpContext.Current.Session("TicketingGatewayError") = errorMessageCode
        HttpContext.Current.Response.Redirect(redirectURL)
    End Sub

    Private Sub LoggingErrorPayment(ByVal errorCode As String, ByVal errorMsg As String)
        If _isTalentLogEnabled Then
            Dim loginId As String = ""
            If Not HttpContext.Current.Profile.IsAnonymous Then
                loginId = HttpContext.Current.Profile.UserName
            ElseIf CType(HttpContext.Current.Profile, TalentProfile).User IsNot Nothing AndAlso CType(HttpContext.Current.Profile, TalentProfile).User.Details IsNot Nothing Then
                loginId = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
            End If
            errorMsg = errorMsg & "; Partner:" & _partnerCode
            errorMsg = errorMsg & "; LoginID:" & loginId
            errorMsg = errorMsg & "; Requested Url: " & HttpContext.Current.Request.Url.AbsoluteUri
            If HttpContext.Current.Request.UrlReferrer IsNot Nothing Then
                errorMsg = errorMsg & "; UrlReferrer: " & HttpContext.Current.Request.UrlReferrer.AbsoluteUri
            End If

            Try
                errorMsg = errorMsg & "; Basket Header Id: " & CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID & "; Basket Total: " & _basketTotal & "; Basket Items Including Fees: " & CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems.Count
                errorMsg = errorMsg & "; Temp Order Id: " & CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID
            Catch ex As Exception
                errorMsg = errorMsg & ex.Message
            End Try
            Try
                _talentLogging.GeneralLog("GoogleCheckoutGateway.aspx", errorCode, errorMsg, "GoogleCheckoutErrorLog")
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub LoggingGoogleCheckoutProcess(ByVal stepNumber As String, ByVal logMessage As String, Optional ByVal basketStatusEntity As DEBasketStatus = Nothing)
        Try
            If _isTalentLogEnabled Then
                Try
                    If basketStatusEntity IsNot Nothing Then
                        logMessage = logMessage & ";" & basketStatusEntity.BasketConnectionString
                        logMessage = logMessage & ";" & basketStatusEntity.BasketHeaderId
                        logMessage = logMessage & ";" & basketStatusEntity.BusinessUnit
                        logMessage = logMessage & ";" & basketStatusEntity.Comment
                        logMessage = logMessage & ";" & basketStatusEntity.ExternalOrderNumber
                        logMessage = logMessage & ";" & basketStatusEntity.GoogleSerialNumber
                        logMessage = logMessage & ";" & basketStatusEntity.LoginId
                        logMessage = logMessage & ";" & basketStatusEntity.Partner
                        logMessage = logMessage & ";" & basketStatusEntity.Status
                        logMessage = logMessage & ";" & basketStatusEntity.TempOrderId
                    End If
                    logMessage = logMessage & "; Partner:" & _partnerCode
                    logMessage = logMessage & "; Basket Header Id: " & CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID & "; Basket Total: " & _basketTotal & "; Basket Items Including Fees: " & CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems.Count
                    logMessage = logMessage & "; Temp Order Id: " & CType(HttpContext.Current.Profile, TalentProfile).Basket.TempOrderID
                Catch ex As Exception
                    logMessage = logMessage & ex.Message
                End Try
                _talentLogging.GeneralLog("GoogleCheckout.vb", stepNumber, logMessage, "GoogleCheckoutProcessLog")
            End If
        Catch ex As Exception

        End Try
    End Sub

#End Region

#Region "Google Order Processing Private Methods"

    Private Sub ProcessGoogleOrderProcessing()

        Dim serialNumber As String = Nothing
        Dim requestInputStream As Stream = HttpContext.Current.Request.InputStream
        Dim requestStreamAsString As String = Nothing

        Using oneStreamReader As StreamReader = New StreamReader(requestInputStream)
            requestStreamAsString = oneStreamReader.ReadToEnd
        End Using
        LogNotificationProcess("001", "ProcessGoogleOrderProcessing - " & requestStreamAsString)
        Dim requestStreamAsParts As String() = requestStreamAsString.Split(New Char() {"="c})
        If (requestStreamAsParts.Length >= 2) Then
            serialNumber = requestStreamAsParts(1)
            InsertSerialNumberNotificationStatus(serialNumber, "serialnumber from google", SERIAL_NUMBER_RECEIVED)
        End If
        LogNotificationProcess("002", "ProcessGoogleOrderProcessing - " & serialNumber)
        'Call NotificationHistory Google Checkout API to retrieve the notification for the given serial number and process the notification
        ProcessNotification(serialNumber)

        'serialize the message to the output stream only if you could process the message.
        'Otherwise throw an http 500.

        Dim response As New GCheckout.AutoGen.NotificationAcknowledgment With { _
            .serialnumber = serialNumber _
        }
        HttpContext.Current.Response.Clear()
        HttpContext.Current.Response.BinaryWrite(EncodeHelper.Serialize(response))
    End Sub

    Private Sub ProcessNotification(ByVal serialNumber As String)

        'The next two statements set up a request and call google checkout for the details based on that serial number.
        Dim oneNotificationHistoryRequest As New NotificationHistoryRequest(_merchantID, _merchantKey, _environmentType, New NotificationHistorySerialNumber(serialNumber))
        Dim oneNotificationHistoryResponse As NotificationHistoryResponse = DirectCast(oneNotificationHistoryRequest.Send, NotificationHistoryResponse)

        'oneNotificationHistoryResponse.ResponseXml contains the complete response
        LogNotificationProcess("003", oneNotificationHistoryResponse.ResponseXml)
        InsertSerialNumberNotificationStatus(serialNumber, "xml related to serial number is received", SERIAL_NUMBER_ORDER_STATUS)
        If _logNotificationXML Then
            InsertSerialNumberNotificationStatus(serialNumber, oneNotificationHistoryResponse.ResponseXml, SERIAL_NUMBER_LOGGED)
        End If


        Dim oneNotification As Object
        For Each oneNotification In oneNotificationHistoryResponse.NotificationResponses
            If oneNotification.GetType.Equals(GetType(GCheckout.AutoGen.NewOrderNotification)) Then
                InsertSerialNumberNotificationStatus(serialNumber, "new-order-notification", SERIAL_NUMBER_PROCESS_START)
                Dim oneNewOrderNotification As GCheckout.AutoGen.NewOrderNotification = DirectCast(oneNotification, GCheckout.AutoGen.NewOrderNotification)
                If oneNewOrderNotification.serialnumber.Equals(serialNumber) Then
                    HandleNewOrderNotification(oneNewOrderNotification)
                End If
            ElseIf oneNotification.GetType.Equals(GetType(GCheckout.AutoGen.OrderStateChangeNotification)) Then
                Dim oneOrderStateChangeNotification As GCheckout.AutoGen.OrderStateChangeNotification = DirectCast(oneNotification, GCheckout.AutoGen.OrderStateChangeNotification)
                InsertSerialNumberNotificationStatus(serialNumber, "order-state-change-notification", SERIAL_NUMBER_PROCESS_START)
                If oneOrderStateChangeNotification.serialnumber.Equals(serialNumber) Then
                    HandleOrderStateChangeNotification(oneOrderStateChangeNotification)
                End If
            ElseIf oneNotification.GetType.Equals(GetType(GCheckout.AutoGen.RiskInformationNotification)) Then
                Dim oneRiskInformationNotification As GCheckout.AutoGen.RiskInformationNotification = DirectCast(oneNotification, GCheckout.AutoGen.RiskInformationNotification)
                InsertSerialNumberNotificationStatus(serialNumber, "risk-information-notification", SERIAL_NUMBER_PROCESS_START)
                If oneRiskInformationNotification.serialnumber.Equals(serialNumber) Then
                    HandleRiskInformationNotification(oneRiskInformationNotification)
                End If
            ElseIf oneNotification.GetType.Equals(GetType(GCheckout.AutoGen.AuthorizationAmountNotification)) Then
                Dim oneAuthorizationAmountNotification As GCheckout.AutoGen.AuthorizationAmountNotification = DirectCast(oneNotification, GCheckout.AutoGen.AuthorizationAmountNotification)
                InsertSerialNumberNotificationStatus(serialNumber, "authorization-amount-notification", SERIAL_NUMBER_PROCESS_START)
                If oneAuthorizationAmountNotification.serialnumber.Equals(serialNumber) Then
                    HandleAuthorizationAmountNotification(oneAuthorizationAmountNotification)
                End If
            ElseIf oneNotification.GetType.Equals(GetType(GCheckout.AutoGen.ChargeAmountNotification)) Then
                Dim oneChargeAmountNotification As GCheckout.AutoGen.ChargeAmountNotification = DirectCast(oneNotification, GCheckout.AutoGen.ChargeAmountNotification)
                InsertSerialNumberNotificationStatus(serialNumber, "charge-amount-notification", SERIAL_NUMBER_PROCESS_START)
                If oneChargeAmountNotification.serialnumber.Equals(serialNumber) Then
                    HandleChargeAmountNotification(oneChargeAmountNotification)
                End If
            Else
                InsertSerialNumberNotificationStatus(serialNumber, "notification type not handled by talent", SERIAL_NUMBER_FAILED)
                LogNotificationError("GCNHERR-NNH", serialNumber)
                Throw New ArgumentOutOfRangeException(String.Concat(New String() {"Unhandled Type [", oneNotification.GetType.ToString, "]!; serialNumber=[", serialNumber, "];"}))
            End If
            InsertSerialNumberNotificationStatus(serialNumber, "notification processed successfully", SERIAL_NUMBER_COMPLETED)
        Next
    End Sub

    Private Sub HandleNewOrderNotification(ByVal inputNewOrderNotification As GCheckout.AutoGen.NewOrderNotification)
        Dim hiddenMerchantPrivateData As String = inputNewOrderNotification.shoppingcart.merchantprivatedata.Any(0).InnerText
        'headerid, temporderid, loginid, username, totalitems, baskettotal
        Dim basketHeaderID As String = String.Empty, _
            tempOrderID As String = String.Empty, _
            loginID As String = String.Empty, _
            username As String = String.Empty, _
            totalItems As String = String.Empty, _
            basketTotal As String = String.Empty, _
            agentName As String = String.Empty

        If inputNewOrderNotification.shoppingcart.merchantprivatedata.Any(0).HasChildNodes Then
            For Each privateDataNode As XmlNode In inputNewOrderNotification.shoppingcart.merchantprivatedata.Any
                LogNotificationProcess("003B", "HandleNewOrderNotification - " & privateDataNode.Name & ";" & privateDataNode.InnerText & ";" & privateDataNode.LocalName)
                Select Case privateDataNode.Name
                    Case MERC_PRIV_DATA_HEADER_ID
                        basketHeaderID = privateDataNode.InnerText
                    Case MERC_PRIV_DATA_TEMP_ORDER_ID
                        tempOrderID = privateDataNode.InnerText
                    Case MERC_PRIV_DATA_LOGIN_ID
                        loginID = privateDataNode.InnerText
                    Case MERC_PRIV_DATA_USERNAME
                        username = privateDataNode.InnerText
                    Case MERC_PRIV_DATA_TOTAL_ITEMS
                        totalItems = privateDataNode.InnerText
                    Case MERC_PRIV_DATA_BASKET_TOTAL
                        basketTotal = privateDataNode.InnerText
                    Case MERC_PRIV_DATA_AGENT_NAME
                        agentName = privateDataNode.InnerText
                End Select
            Next
        End If

        For Each oneItem As GCheckout.AutoGen.Item In inputNewOrderNotification.shoppingcart.items
            ' TODO: Get MerchantItemId from shopping cart item (oneItem.merchantitemid) and process it
        Next
        Dim sbCommentDetails As New StringBuilder
        sbCommentDetails.Append("LoginID: " & loginID)
        sbCommentDetails.Append("; Username: " & username)
        sbCommentDetails.Append("; Total items: " & totalItems)
        sbCommentDetails.Append("; Payment amount: " & basketTotal)
        sbCommentDetails.Append("; Buyer Id: " & inputNewOrderNotification.buyerid.ToString)
        sbCommentDetails.Append("; Financial order state: " & inputNewOrderNotification.financialorderstate.ToString)
        sbCommentDetails.Append("; Fulfillment order state: " & inputNewOrderNotification.fulfillmentorderstate.ToString)
        sbCommentDetails.Append("; Cart Expires: " & inputNewOrderNotification.shoppingcart.cartexpiration.gooduntildate.ToString("dd/MM/yyyy HH:mm:ss"))
        sbCommentDetails.Append("; Order total in google: " & inputNewOrderNotification.ordertotal.Value.ToString)
        sbCommentDetails.Append("; Buyer marketing email allowed: " & inputNewOrderNotification.buyermarketingpreferences.emailallowed.ToString)
        LogNotificationProcess("004", "HandleNewOrderNotification - " & sbCommentDetails.ToString)
        'TODO: Add custom processing for this notification type
        Dim basketConnectionString As String = GetBasketSQLConnection(basketHeaderID, inputNewOrderNotification.googleordernumber)
        If String.IsNullOrWhiteSpace(basketConnectionString) Then
            'frontend basket is missing so call order cancel request
            HandleAndInitiateCancelOrderRequest(CANCEL_NEWORDER_AS_FBM, inputNewOrderNotification.serialnumber, basketHeaderID, inputNewOrderNotification.googleordernumber, loginID, basketConnectionString, tempOrderID)
        Else
            'basket found update google order number to backend basket
            Dim backendErrorDetails As String = String.Empty
            If AddAndVerifyBackendBasket(basketHeaderID, inputNewOrderNotification.googleordernumber, BACKEND_BASKET_VERIFY_MODE_U, backendErrorDetails) Then
                'basket found update the status to frontend basket
                sbCommentDetails.Append("; Backend error details: " & backendErrorDetails)

                Dim basketStatusEntity As New DEBasketStatus
                basketStatusEntity.LoginId = loginID
                basketStatusEntity.BasketConnectionString = basketConnectionString
                basketStatusEntity.GoogleSerialNumber = inputNewOrderNotification.serialnumber
                basketStatusEntity.ExternalOrderNumber = inputNewOrderNotification.googleordernumber
                basketStatusEntity.BusinessUnit = _businessUnit
                basketStatusEntity.Partner = _partnerCode
                basketStatusEntity.TempOrderId = tempOrderID
                basketStatusEntity.BasketHeaderId = basketHeaderID
                basketStatusEntity.Comment = sbCommentDetails.ToString
                LogNotificationProcess("007", "HandleNewOrderNotification - " & sbCommentDetails.ToString)
                If UpdateBasketAndOrderStatus(basketStatusEntity, "PAYMENT INTERMEDIATE 2") Then
                    'send new order email to user with cc to club
                    EmailToUserNewOrderNotification(loginID, inputNewOrderNotification, basketStatusEntity)
                End If
            Else
                'backend basket expired so call order cancel request
                HandleAndInitiateCancelOrderRequest(CANCEL_NEWORDER_AS_BBM, inputNewOrderNotification.serialnumber, basketHeaderID, inputNewOrderNotification.googleordernumber, loginID, basketConnectionString, tempOrderID)
            End If
        End If
    End Sub

    Private Function IsRiskyOrderPayment(ByVal AVS_Received As String, ByVal CVN_Received As String) As Boolean
        LogNotificationProcess("012", "IsRiskyOrderPayment - " & AVS_Received & ";" & CVN_Received)
        Dim isRisky As Boolean = True
        If AVS_Received.Length > 0 AndAlso CVN_Received.Length > 0 Then
            'Y - Full AVS match (address and postal code)   'P - Partial AVS match (postal code only)
            'A - Partial AVS match (address only)   'N - no AVS match
            'U - AVS not supported by issuer
            Dim avsValid As Boolean = False
            If _riskInfoAllowedAVSResponses.Length > 0 Then
                Dim expectedAVSResponses As String() = _riskInfoAllowedAVSResponses.Split(";")
                For avsArrIndex As Integer = 0 To expectedAVSResponses.Length - 1
                    If expectedAVSResponses(avsArrIndex).ToUpper = AVS_Received.ToUpper Then
                        avsValid = True
                        Exit For
                    End If
                Next
            Else
                avsValid = True
            End If

            'M - CVN match  'N - No CVN match   'U - CVN not available  'E - CVN error
            Dim cvnValid As Boolean = False
            If _riskInfoAllowedCVNResponses.Length > 0 Then
                Dim expectedCVNResponses As String() = _riskInfoAllowedCVNResponses.Split(";")
                For avsArrIndex As Integer = 0 To expectedCVNResponses.Length - 1
                    If expectedCVNResponses(avsArrIndex).ToUpper = CVN_Received.ToUpper Then
                        cvnValid = True
                        Exit For
                    End If
                Next
            Else
                cvnValid = True
            End If
            If avsValid AndAlso cvnValid Then
                isRisky = False
            End If
        End If
        Return isRisky
    End Function

    Private Sub HandleAuthorizationAmountNotification(ByVal inputAuthorizationAmountNotification As GCheckout.AutoGen.AuthorizationAmountNotification)
        'TODO: Add custom processing for this notification type
        LogNotificationProcess("010", "HandleAuthorizationAmountNotification")
        'get the merchant private data if exits
        'headerid, temporderid, loginid, username, totalitems, baskettotal
        Dim basketHeaderID As String = String.Empty, _
            tempOrderID As String = String.Empty, _
            loginID As String = String.Empty, _
            username As String = String.Empty, _
            totalItems As String = String.Empty, _
            basketTotal As String = String.Empty, _
            agentName As String = String.Empty

        If inputAuthorizationAmountNotification.ordersummary.shoppingcart.merchantprivatedata.Any(0).HasChildNodes Then
            For Each privateDataNode As XmlNode In inputAuthorizationAmountNotification.ordersummary.shoppingcart.merchantprivatedata.Any
                LogNotificationProcess("0010A", "HandleAuthorizationAmountNotification - " & privateDataNode.Name & ";" & privateDataNode.InnerText & ";" & privateDataNode.LocalName)
                Select Case privateDataNode.Name
                    Case MERC_PRIV_DATA_HEADER_ID
                        basketHeaderID = privateDataNode.InnerText
                    Case MERC_PRIV_DATA_TEMP_ORDER_ID
                        tempOrderID = privateDataNode.InnerText
                    Case MERC_PRIV_DATA_LOGIN_ID
                        loginID = privateDataNode.InnerText
                    Case MERC_PRIV_DATA_USERNAME
                        username = privateDataNode.InnerText
                    Case MERC_PRIV_DATA_TOTAL_ITEMS
                        totalItems = privateDataNode.InnerText
                    Case MERC_PRIV_DATA_BASKET_TOTAL
                        basketTotal = privateDataNode.InnerText
                    Case MERC_PRIV_DATA_AGENT_NAME
                        agentName = privateDataNode.InnerText
                End Select
            Next
        End If

        Dim basketConnectionString As String = String.Empty
        'update basket with status
        If UpdateBasketForAuthorizationStatus(basketConnectionString, inputAuthorizationAmountNotification, tempOrderID, basketHeaderID, loginID) Then
            If ((inputAuthorizationAmountNotification.ordersummary.financialorderstate = GCheckout.AutoGen.FinancialOrderState.CHARGEABLE)) Then
                'now you can charge it only when AVS CVN are matched with our requirement
                If Not IsRiskyOrderPayment(inputAuthorizationAmountNotification.avsresponse, inputAuthorizationAmountNotification.cvnresponse) Then
                    'basket found update google order number to backend basket
                    Dim backendErrorDetails As String = String.Empty
                    If AddAndVerifyBackendBasket(basketHeaderID, inputAuthorizationAmountNotification.googleordernumber, BACKEND_BASKET_VERIFY_MODE_A, backendErrorDetails) Then
                        'basket found update the status to frontend basket
                        Dim basketStatusEntity As New DEBasketStatus
                        basketStatusEntity.BasketConnectionString = basketConnectionString
                        basketStatusEntity.GoogleSerialNumber = inputAuthorizationAmountNotification.serialnumber
                        basketStatusEntity.ExternalOrderNumber = inputAuthorizationAmountNotification.googleordernumber
                        basketStatusEntity.BusinessUnit = _businessUnit
                        basketStatusEntity.Partner = _partnerCode
                        basketStatusEntity.TempOrderId = tempOrderID
                        basketStatusEntity.BasketHeaderId = basketHeaderID
                        basketStatusEntity.LoginId = loginID
                        basketStatusEntity.Comment = "Backend Basket Found Calling Charge and ship request"
                        LogNotificationProcess("013", "HandleAuthorizationAmountNotification - " & basketStatusEntity.Comment)
                        If UpdateBasketAndOrderStatus(basketStatusEntity, "PAYMENT INTERMEDIATE 5") Then
                            'Dim gcRequest As New CheckoutShoppingCartRequest(_merchantID, _merchantKey, _environmentType, _currency, _cartExpirationMinutes)
                            Dim chargeAndShipOrderRequestObj As New ChargeAndShipOrderRequest(_merchantID, _
                                                                           _merchantKey, _
                                                                           _environmentType, _
                                                                           inputAuthorizationAmountNotification.googleordernumber, _
                                                                           _currency, _
                                                                           inputAuthorizationAmountNotification.authorizationamount.Value)
                            Dim oneGCheckoutResponse As GCheckoutResponse = chargeAndShipOrderRequestObj.Send
                            If oneGCheckoutResponse.IsGood Then
                                Dim tempCharedStatus As String = oneGCheckoutResponse.SerialNumber
                                tempCharedStatus = tempCharedStatus & ";" & oneGCheckoutResponse.RedirectUrl
                                tempCharedStatus = tempCharedStatus & ";" & oneGCheckoutResponse.ResponseXml
                                basketStatusEntity.Comment = tempCharedStatus
                                UpdateBasketAndOrderStatus(basketStatusEntity, "PAYMENT ACCEPTED")
                                Dim returnErrorCode As String = ProcessBackendForPaymentSuccess(basketHeaderID, inputAuthorizationAmountNotification.googleordernumber, tempOrderID, loginID, agentName)
                                If returnErrorCode.Trim.Length > 0 Then
                                    'charged the customer but lost the backend basket
                                    'todo ck: we have to do refund and cancel or how
                                    basketStatusEntity.Comment = "Error Code From Backend:" & returnErrorCode
                                    UpdateBasketAndOrderStatus(basketStatusEntity, "ORDER FAILED")
                                    EmailPaymentSuccessOrderFailed(basketStatusEntity, inputAuthorizationAmountNotification)
                                Else
                                    'send new order email to user with cc to club
                                    Dim paymentRef As String = String.Empty
                                    If HttpContext.Current.Session("TalentPaymentReference") IsNot Nothing Then
                                        paymentRef = HttpContext.Current.Session("TalentPaymentReference").ToString
                                        basketStatusEntity.Comment = "Talent Payment Reference: " & paymentRef
                                    End If

                                    UpdateBasketAndOrderStatus(basketStatusEntity, "ORDER COMPLETE")
                                    EmailOrderConfirmation(paymentRef, inputAuthorizationAmountNotification, basketStatusEntity)
                                End If
                            Else
                                Dim tempCharedStatus As String = oneGCheckoutResponse.SerialNumber
                                tempCharedStatus = tempCharedStatus & ";" & oneGCheckoutResponse.RedirectUrl
                                tempCharedStatus = tempCharedStatus & ";" & oneGCheckoutResponse.ResponseXml
                                tempCharedStatus = tempCharedStatus & ";" & oneGCheckoutResponse.ResponseXml
                                tempCharedStatus = tempCharedStatus & ";" & oneGCheckoutResponse.ErrorMessage
                                basketStatusEntity.Comment = tempCharedStatus
                                If UpdateBasketAndOrderStatus(basketStatusEntity, "PAYMENT REJECTED") Then

                                End If
                                'charge and ship request failed so cancel the order
                                HandleAndInitiateCancelOrderRequest(CANCEL_AMTAUTH_AS_CASRF, inputAuthorizationAmountNotification.serialnumber, basketHeaderID, inputAuthorizationAmountNotification.googleordernumber, loginID, basketConnectionString, tempOrderID)
                            End If

                        End If
                    Else
                        'backend basket expired so call order cancel request
                        HandleAndInitiateCancelOrderRequest(CANCEL_AMTAUTH_AS_BBM, inputAuthorizationAmountNotification.serialnumber, basketHeaderID, inputAuthorizationAmountNotification.googleordernumber, loginID, basketConnectionString, tempOrderID)
                    End If
                Else
                    'avs and cvn response risky payment
                    HandleAndInitiateCancelOrderRequest(CANCEL_AMTAUTH_AS_RISK, inputAuthorizationAmountNotification.serialnumber, basketHeaderID, inputAuthorizationAmountNotification.googleordernumber, loginID, basketConnectionString, tempOrderID)
                End If
            ElseIf ((inputAuthorizationAmountNotification.ordersummary.financialorderstate = GCheckout.AutoGen.FinancialOrderState.PAYMENT_DECLINED)) Then
                'order payment is declined
                HandleAndInitiateCancelOrderRequest(CANCEL_AMTAUTH_AS_PAYDEC, inputAuthorizationAmountNotification.serialnumber, basketHeaderID, inputAuthorizationAmountNotification.googleordernumber, loginID, basketConnectionString, tempOrderID)
            ElseIf ((inputAuthorizationAmountNotification.ordersummary.financialorderstate = GCheckout.AutoGen.FinancialOrderState.CANCELLED)) OrElse _
                ((inputAuthorizationAmountNotification.ordersummary.financialorderstate = GCheckout.AutoGen.FinancialOrderState.CANCELLED_BY_GOOGLE)) Then
                'order is cancelled in google mark the status to tbl_basket_status
                'order is cancelled by seller, buyer or google
                HandleAndInitiateCancelOrderRequest(CANCEL_AMTAUTH_AS_OC, inputAuthorizationAmountNotification.serialnumber, basketHeaderID, inputAuthorizationAmountNotification.googleordernumber, loginID, basketConnectionString, tempOrderID)
            End If
        Else
            'order is cancelled as frontend basket missing - else for UpdateBasketForAuthorizationStatus 
            HandleAndInitiateCancelOrderRequest(CANCEL_AMTAUTH_AS_FBM, inputAuthorizationAmountNotification.serialnumber, basketHeaderID, inputAuthorizationAmountNotification.googleordernumber, loginID, basketConnectionString, tempOrderID)
        End If
    End Sub

    Private Sub HandleChargeAmountNotification(ByVal inputChargeAmountNotification As GCheckout.AutoGen.ChargeAmountNotification)
        'TODO: Add custom processing for this notification type
    End Sub

    Private Sub HandleOrderStateChangeNotification(ByVal notification As GCheckout.AutoGen.OrderStateChangeNotification)

        'Charge Order If Chargeable
        'If ((notification.previousfinancialorderstate = GCheckout.AutoGen.FinancialOrderState.REVIEWING) _
        '    AndAlso (notification.newfinancialorderstate = GCheckout.AutoGen.FinancialOrderState.CHARGEABLE)) Then

        '    Dim oneGCheckoutResponse As GCheckoutResponse = New ChargeOrderRequest(notification.googleordernumber).Send
        '    LogNotificationError("006", oneGCheckoutResponse.IsGood.ToString)
        'End If
        ''Update License If Charged
        'If ((notification.previousfinancialorderstate = GCheckout.AutoGen.FinancialOrderState.CHARGING) _
        '    AndAlso (notification.newfinancialorderstate = GCheckout.AutoGen.FinancialOrderState.CHARGED)) Then
        '    'TODO: For each shopping cart item received in the NewOrderNotification, authorize the license
        'End If

        'TODO: Add custom processing for this notification type
    End Sub

    Private Sub HandleRiskInformationNotification(ByVal notification As GCheckout.AutoGen.RiskInformationNotification)
        ' TODO: Add custom processing for this notification type
    End Sub

    Private Sub HandleAndInitiateCancelOrderRequest(ByVal cancelReasonCode As String, ByVal serialNumber As String, ByVal basketHeaderId As String, ByVal googleOrderNumber As String, ByVal loginId As String, ByVal basketConnectionString As String, ByVal tempOrderId As String)
        'on cancellation success send cancellation email to user with reason
        LogNotificationProcess("000", "HandleAndInitiateCancelOrderRequest - " & cancelReasonCode)
        Dim basketStatusEntity As New DEBasketStatus
        basketStatusEntity.BusinessUnit = _businessUnit
        basketStatusEntity.LoginId = loginId
        basketStatusEntity.GoogleSerialNumber = serialNumber
        basketStatusEntity.ExternalOrderNumber = googleOrderNumber
        basketStatusEntity.BasketHeaderId = basketHeaderId
        basketStatusEntity.TempOrderId = tempOrderId
        basketStatusEntity.BasketConnectionString = basketConnectionString
        basketStatusEntity.Comment = cancelReasonCode
        'update basket status
        UpdateBasketAndOrderStatus(basketStatusEntity, "PAYMENT REJECTED")
        Dim returnCodeFromBackendCall As String = String.Empty
        Select Case cancelReasonCode
            Case CANCEL_NEWORDER_AS_FBM
                'call backend payment failure
                returnCodeFromBackendCall = ProcessBackendForPaymentFailure(basketHeaderId, googleOrderNumber, tempOrderId, loginId)
                CallCancelOrderRequest(cancelReasonCode, basketStatusEntity, TEBUtilities.CheckForDBNull_String(_wfr.Content("Reason_FrontendBasketMissing", _languageCode, True)))
            Case CANCEL_NEWORDER_AS_BBM
                CallCancelOrderRequest(cancelReasonCode, basketStatusEntity, TEBUtilities.CheckForDBNull_String(_wfr.Content("Reason_BackendBasketExpired", _languageCode, True)))
            Case CANCEL_AMTAUTH_AS_FBM
                'call backend payment failure
                returnCodeFromBackendCall = ProcessBackendForPaymentFailure(basketHeaderId, googleOrderNumber, tempOrderId, loginId)
                CallCancelOrderRequest(cancelReasonCode, basketStatusEntity, TEBUtilities.CheckForDBNull_String(_wfr.Content("Reason_FrontendBasketMissing", _languageCode, True)))
            Case CANCEL_AMTAUTH_AS_BBM
                'update basket status
                CallCancelOrderRequest(cancelReasonCode, basketStatusEntity, TEBUtilities.CheckForDBNull_String(_wfr.Content("Reason_BackendBasketExpired", _languageCode, True)))
            Case CANCEL_AMTAUTH_AS_RISK
                'call backend payment failure
                returnCodeFromBackendCall = ProcessBackendForPaymentFailure(basketHeaderId, googleOrderNumber, tempOrderId, loginId)
                CallCancelOrderRequest(cancelReasonCode, basketStatusEntity, TEBUtilities.CheckForDBNull_String(_wfr.Content("Reason_PaymentRisky", _languageCode, True)))
            Case CANCEL_AMTAUTH_AS_CASRF
                'call backend payment failure
                CallCancelOrderRequest(cancelReasonCode, basketStatusEntity, TEBUtilities.CheckForDBNull_String(_wfr.Content("Reason_ChargeCustomerFailed", _languageCode, True)))
            Case CANCEL_AMTAUTH_AS_OC
                'call backend payment failure
                returnCodeFromBackendCall = ProcessBackendForPaymentFailure(basketHeaderId, googleOrderNumber, tempOrderId, loginId)
                'Ordered is already cancelled in google so no need to call cancelorderrequest
                'CallCancelOrderRequest(basketStatusEntity, TEBUtilities.CheckForDBNull_String(_wfr.Content("Reason_ChargeCustomerFailed", _languageCode, True)))
            Case CANCEL_AMTAUTH_AS_PAYDEC
                'call backend payment failure
                returnCodeFromBackendCall = ProcessBackendForPaymentFailure(basketHeaderId, googleOrderNumber, tempOrderId, loginId)
                CallCancelOrderRequest(cancelReasonCode, basketStatusEntity, TEBUtilities.CheckForDBNull_String(_wfr.Content("Reason_PaymentDeclined", _languageCode, True)))
        End Select
        If Not String.IsNullOrWhiteSpace(returnCodeFromBackendCall) Then
            basketStatusEntity.Comment = "ProcessBackendForPaymentFailure Error: " & returnCodeFromBackendCall
            UpdateBasketAndOrderStatus(basketStatusEntity, "PAYMENT REJECTED")
        End If
    End Sub

    Private Sub CallCancelOrderRequest(ByVal cancelReasonCode As String, ByVal basketStatusEntity As DEBasketStatus, ByVal reasonForCancel As String)
        LogNotificationProcess("000", "CallCancelOrderRequest - " & basketStatusEntity.ExternalOrderNumber & ";" & basketStatusEntity.GoogleSerialNumber & ";" & reasonForCancel)
        Try
            Dim cancelOrderRequestObj As New CancelOrderRequest(_merchantID, _merchantKey, _environmentType, basketStatusEntity.ExternalOrderNumber, reasonForCancel)
            Dim oneGCheckoutResponse As GCheckoutResponse = cancelOrderRequestObj.Send
            If Not oneGCheckoutResponse.IsGood Then
                Dim tempCharedStatus As String = "CancelOrderRequest Call Failed: " & oneGCheckoutResponse.SerialNumber
                tempCharedStatus = tempCharedStatus & ";" & oneGCheckoutResponse.RedirectUrl
                tempCharedStatus = tempCharedStatus & ";" & oneGCheckoutResponse.ResponseXml
                tempCharedStatus = tempCharedStatus & ";" & oneGCheckoutResponse.ResponseXml
                tempCharedStatus = tempCharedStatus & ";" & oneGCheckoutResponse.ErrorMessage
                basketStatusEntity.Comment = tempCharedStatus
                EmailToUserOrderCancellation(basketStatusEntity, cancelReasonCode, reasonForCancel)
                UpdateBasketAndOrderStatus(basketStatusEntity, "PAYMENT REJECTED")
            Else
                basketStatusEntity.Comment += oneGCheckoutResponse.IsGood.ToString
                basketStatusEntity.Comment += ";" & oneGCheckoutResponse.ResponseXml
                UpdateBasketAndOrderStatus(basketStatusEntity, "PAYMENT REJECTED")
            End If
            LogNotificationProcess("000", "CallCancelOrderRequest - " & basketStatusEntity.ExternalOrderNumber & ";" & basketStatusEntity.GoogleSerialNumber & ";" & oneGCheckoutResponse.IsGood.ToString)
        Catch ex As Exception
            basketStatusEntity.Comment = ex.Message
            UpdateBasketAndOrderStatus(basketStatusEntity, "PAYMENT REJECTED")
        End Try
    End Sub

    Private Function UpdateBasketForAuthorizationStatus(ByRef basketConnectionString As String, ByVal inputAuthorizationAmountNotification As GCheckout.AutoGen.AuthorizationAmountNotification, ByVal tempOrderId As String, ByVal basketHeaderID As String, ByVal loginID As String) As Boolean
        Dim isUpdated As Boolean = False
        Dim sbCommentDetails As New StringBuilder
        sbCommentDetails.Append(basketHeaderID & ";" & tempOrderId & ";" & loginID)
        sbCommentDetails.Append("; Buyer ID: " & inputAuthorizationAmountNotification.ordersummary.buyerid.ToString)
        sbCommentDetails.Append("; Financial order state: " & inputAuthorizationAmountNotification.ordersummary.financialorderstate.ToString)
        sbCommentDetails.Append("; Fulfillment order state: " & inputAuthorizationAmountNotification.ordersummary.fulfillmentorderstate.ToString)
        sbCommentDetails.Append("; Authorization Expires: " & inputAuthorizationAmountNotification.authorizationexpirationdate.ToString("dd/MM/yyyy HH:mm:ss"))
        sbCommentDetails.Append("; Order total in google: " & inputAuthorizationAmountNotification.ordersummary.ordertotal.Value.ToString)
        sbCommentDetails.Append("; AVS Response: " & inputAuthorizationAmountNotification.avsresponse)
        sbCommentDetails.Append("; CVN Response: " & inputAuthorizationAmountNotification.cvnresponse)
        LogNotificationProcess("011", "UpdateBasketForAuthorizationStatus - " & sbCommentDetails.ToString)
        basketConnectionString = GetBasketSQLConnection("", inputAuthorizationAmountNotification.googleordernumber)
        If Not String.IsNullOrWhiteSpace(basketConnectionString) Then
            Dim basketStatusEntity As New DEBasketStatus
            basketStatusEntity.BasketConnectionString = basketConnectionString
            basketStatusEntity.GoogleSerialNumber = inputAuthorizationAmountNotification.serialnumber
            basketStatusEntity.ExternalOrderNumber = inputAuthorizationAmountNotification.googleordernumber
            basketStatusEntity.BusinessUnit = _businessUnit
            basketStatusEntity.Partner = _partnerCode
            basketStatusEntity.TempOrderId = tempOrderId
            basketStatusEntity.BasketHeaderId = basketHeaderID
            basketStatusEntity.LoginId = loginID
            basketStatusEntity.Comment = sbCommentDetails.ToString
            If UpdateBasketAndOrderStatus(basketStatusEntity, "PAYMENT INTERMEDIATE 4") Then
                isUpdated = True
            End If
        End If
        If Not isUpdated OrElse String.IsNullOrWhiteSpace(basketConnectionString) Then
            isUpdated = False
            LogNotificationError("GCNHERR-FBM", "Front end basket is missing or status update failed")
        End If
        Return isUpdated
    End Function

    Private Function AddAndVerifyBackendBasket(ByVal basketHeaderID As String, ByVal googleOrderNumber As String, ByVal mode As String, ByRef errorDetails As String) As Boolean
        LogNotificationProcess("006", "AddAndVerifyBackendBasket - " & basketHeaderID & ";" & googleOrderNumber & ";" & mode & ";" & errorDetails)
        Dim isUpdated As Boolean = False
        Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
        Dim returnErrorCode As String = ticketingGatewayFunctions.VerifyAndUpdateExtOrderNumberToBasket(basketHeaderID, mode, googleOrderNumber)
        If Not String.IsNullOrWhiteSpace(returnErrorCode) Then
            isUpdated = False
        Else
            isUpdated = True
        End If
        If Not isUpdated Then
            LogNotificationError("GCNHERR-BB", returnErrorCode)
        End If
        Return isUpdated
    End Function

    Private Function InsertSerialNumberNotificationStatus(ByVal serialNumber As String, ByVal comments As String, ByVal status As String) As Boolean
        Dim isLogged As Boolean = False
        Dim affectedRows As Integer = _talentDataObj.BasketSettings.TblGoogleCheckoutSerialNumberStatus.Insert(serialNumber, comments, status)
        If affectedRows <= 0 Then
            'insert failed log it
            LogNotificationError("GCNHERR-SNO", serialNumber & ";" & comments & ";" & status)
        Else
            isLogged = True
            LogNotificationProcess("003a", "InsertSerialNumberNotificationStatus - " & serialNumber & ";" & comments & ";" & status)
        End If
        Return isLogged
    End Function

    Private Function GetBasketSQLConnection(ByVal basketHeaderID As String, ByVal googleOrderNumber As String) As String
        LogNotificationProcess("000", "GetBasketSQLConnection - " & googleOrderNumber & ";" & basketHeaderID)
        Dim basketConnectionString As String = String.Empty
        Dim dtSQLFarmConnString As DataTable = _talentDataObj.AppVariableSettings.TblDatabaseVersion.GetByDestinationDatabase("SQLFARM")
        If dtSQLFarmConnString IsNot Nothing AndAlso dtSQLFarmConnString.Rows.Count > 0 Then
            Dim dtBasketStatus As DataTable = Nothing
            For rowIndex As Integer = 0 To dtSQLFarmConnString.Rows.Count - 1
                basketConnectionString = TEBUtilities.CheckForDBNull_String(dtSQLFarmConnString.Rows(0)("CONNECTION_STRING")).Trim
                If basketConnectionString.Length > 0 Then
                    dtBasketStatus = _talentDataObj.BasketSettings.TblBasketStatus.GetByBasketHeaderID(basketConnectionString, basketHeaderID, googleOrderNumber)
                    If dtBasketStatus IsNot Nothing AndAlso dtBasketStatus.Rows.Count > 0 Then
                        Exit For
                    Else
                        basketConnectionString = String.Empty
                    End If
                End If
            Next
        End If
        If String.IsNullOrWhiteSpace(basketConnectionString) Then
            LogNotificationError("GCNHERR-BM", "Not able to find the frontend basket")
        End If
        LogNotificationProcess("005a", "GetBasketSQLConnection - " & basketConnectionString)
        Return basketConnectionString
    End Function

    Private Function UpdateBasketAndOrderStatus(ByVal basketStatusEntity As DEBasketStatus, ByVal orderStatus As String) As Boolean
        LogNotificationProcess("000", "UpdateBasketAndOrderStatus - " & basketStatusEntity.BasketHeaderId & ";" & basketStatusEntity.GoogleSerialNumber & ";" & basketStatusEntity.ExternalOrderNumber)
        Dim isInserted As Boolean = False
        If String.IsNullOrWhiteSpace(basketStatusEntity.LoginId) Then
            Dim loginId As String = HttpContext.Current.Profile.UserName
            If CType(HttpContext.Current.Profile, TalentProfile).User IsNot Nothing AndAlso CType(HttpContext.Current.Profile, TalentProfile).User.Details IsNot Nothing Then
                loginId = CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID
            End If
            basketStatusEntity.LoginId = loginId
        End If
        basketStatusEntity.Status = Talent.Common.Utilities.GetOrderStatus(orderStatus)
        _talentDataObj.BasketSettings.TblBasketStatus.BasketStatusEntity = basketStatusEntity
        Dim affectedRows As Integer = _talentDataObj.BasketSettings.TblBasketStatus.Insert()
        If affectedRows > 0 Then
            isInserted = True
        Else
            'failed to update basket status log it
            Dim errorMsg As String = String.Empty
            errorMsg += basketStatusEntity.BasketConnectionString
            errorMsg += "; " & basketStatusEntity.BusinessUnit
            errorMsg += "; " & basketStatusEntity.Partner
            errorMsg += "; " & basketStatusEntity.LoginId
            errorMsg += "; " & basketStatusEntity.BasketHeaderId
            errorMsg += "; " & basketStatusEntity.TempOrderId
            errorMsg += "; " & basketStatusEntity.Comment
            errorMsg += "; " & basketStatusEntity.ExternalOrderNumber
            errorMsg += "; " & basketStatusEntity.GoogleSerialNumber
            errorMsg += "; " & basketStatusEntity.Status
            LogNotificationError("GCNHERR-FB", errorMsg)
        End If
        Return isInserted
    End Function

    Private Function ProcessBackendForPaymentSuccess(ByVal basketHeaderId As String, ByVal googleOrderNumber As String, ByVal tempOrderId As String, ByVal loginId As String, ByVal agentName As String) As String
        LogNotificationProcess("006", "ProcessBackendForPaymentSuccess")
        Dim returnErrorCode As String = String.Empty
        HttpContext.Current.Session("ExternalGatewayPayType") = GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE
        HttpContext.Current.Session("ExtPaymentReferenceNo") = googleOrderNumber
        Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
        ticketingGatewayFunctions.CheckoutExternalSuccess(False, basketHeaderId, False, agentName)
        If HttpContext.Current.Session("TalentErrorCode") IsNot Nothing AndAlso HttpContext.Current.Session("TalentErrorCode").ToString.Trim.Length > 0 Then
            returnErrorCode = HttpContext.Current.Session("TalentErrorCode").ToString.Trim
        ElseIf HttpContext.Current.Session("TicketingGatewayError") IsNot Nothing AndAlso HttpContext.Current.Session("TicketingGatewayError").ToString.Trim.Length > 0 Then
            returnErrorCode = HttpContext.Current.Session("TicketingGatewayError").ToString.Trim
        End If
        If HttpContext.Current.Session("TalentPaymentReference") IsNot Nothing Then
            LogNotificationProcess("006", "ProcessBackendForPaymentSuccess - Talent Pay Ref :" & HttpContext.Current.Session("TalentPaymentReference").ToString)
        End If
        Return returnErrorCode
    End Function

    Private Function ProcessBackendForPaymentFailure(ByVal basketHeaderId As String, ByVal googleOrderNumber As String, ByVal tempOrderId As String, ByVal loginId As String) As String
        LogNotificationProcess("000", "ProcessBackendPaymentFailure")
        Dim returnErrorCode As String = String.Empty
        HttpContext.Current.Session("ExternalGatewayPayType") = GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE
        Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
        ticketingGatewayFunctions.CheckoutExternalFailure(False, basketHeaderId, False)
        If HttpContext.Current.Session("TalentErrorCode") IsNot Nothing AndAlso HttpContext.Current.Session("TalentErrorCode").ToString.Trim.Length > 0 Then
            returnErrorCode = HttpContext.Current.Session("TalentErrorCode").ToString.Trim
        ElseIf HttpContext.Current.Session("TicketingGatewayError") IsNot Nothing AndAlso HttpContext.Current.Session("TicketingGatewayError").ToString.Trim.Length > 0 Then
            returnErrorCode = HttpContext.Current.Session("TicketingGatewayError").ToString.Trim
        End If
        Return returnErrorCode
    End Function

    Private Sub LogNotificationError(ByVal errorCode As String, ByVal errorMsg As String)
        If _isTalentLogEnabled Then
            errorMsg = errorMsg & "; Requested Url: " & HttpContext.Current.Request.Url.AbsoluteUri
            If HttpContext.Current.Request.UrlReferrer IsNot Nothing Then
                errorMsg = errorMsg & "; UrlReferrer: " & HttpContext.Current.Request.UrlReferrer.AbsoluteUri
            End If
            _talentLogging.GeneralLog("GoogleCheckout.vb", errorCode, errorMsg, "GCNotificationErrorLog")
        End If
    End Sub

    Private Sub LogNotificationProcess(ByVal stepNumber As String, ByVal logMessage As String)
        If _isTalentLogEnabled Then
            _talentLogging.GeneralLog("GoogleCheckout.vb", stepNumber, logMessage, "GCNotificationProcessLog")
        End If
    End Sub

    Private Function TryGetTalentCustomerDetails(ByVal customerNumber As String, ByRef returnCode As String) As DataTable
        LogNotificationProcess("008", "TryGetTalentCustomerDetails - " & customerNumber)
        Dim dtCustomerDetails As DataTable = Nothing
        Dim settingsEntity As New DESettings
        Dim customerSettings As New DECustomer
        Dim customerTalent As New TalentCustomer
        Dim err As New ErrorObj
        settingsEntity.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settingsEntity.BusinessUnit = TalentCache.GetBusinessUnit()
        settingsEntity.StoredProcedureGroup = _defaultsValue.StoredProcedureGroup()
        settingsEntity.TicketingKioskMode = True
        settingsEntity.PerformWatchListCheck = False
        customerSettings.BasketID = CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID
        customerSettings.CustomerNumber = customerNumber
        customerSettings.Source = "W"
        customerTalent.Settings = settingsEntity
        Dim customerSettingsV11 As New DECustomerV11
        customerSettingsV11.DECustomersV1.Add(customerSettings)
        customerTalent.DeV11 = customerSettingsV11
        err = customerTalent.VerifyAndRetrieveCustomerDetails()
        If Not err.HasError _
                        AndAlso Not customerTalent.ResultDataSet Is Nothing _
                        AndAlso customerTalent.ResultDataSet.Tables(0).Rows.Count > 0 Then
            Dim dr As DataRow
            ' Has there been an internal error
            dr = customerTalent.ResultDataSet.Tables(0).Rows(0)
            If dr("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                returnCode = TEBUtilities.CheckForDBNull_String(dr("ReturnCode"))
            Else
                If customerTalent.ResultDataSet.Tables.Count > 1 _
                                                    AndAlso customerTalent.ResultDataSet.Tables(1) IsNot Nothing _
                                                    AndAlso customerTalent.ResultDataSet.Tables(1).Rows.Count > 0 Then
                    dtCustomerDetails = customerTalent.ResultDataSet.Tables(1)
                End If
            End If
        Else
            returnCode = err.ErrorNumber & ";" & err.ErrorMessage
        End If

        Return dtCustomerDetails
    End Function

    Private Sub EmailToUserNewOrderNotification(ByVal loginid As String, ByVal inputNewOrderNotification As GCheckout.AutoGen.NewOrderNotification, ByVal basketStatusEntity As DEBasketStatus)
        LogNotificationProcess("009", "EmailToUserNewOrderNotification")
        Dim returnErrorCode As String = String.Empty
        If Not String.IsNullOrWhiteSpace(loginid) Then
            Dim dtCustomerDetails As DataTable = TryGetTalentCustomerDetails(basketStatusEntity.LoginId, returnErrorCode)
            If dtCustomerDetails IsNot Nothing AndAlso dtCustomerDetails.Rows.Count > 0 Then
                Dim emailOrder As New Talent.eCommerce.Order_Email
                'emailOrder.SendTicketingConfirmationEmail(paymentRef)
                'basketStatusEntity.Comment = "ConfirmationEmail sent successfully;" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("CustomerNo")) & _
                '                        ";" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("EmailAddress")) & _
                '                        ";" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("ContactTitle")) & _
                '                        ";" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("ContactForename")) & _
                '                        ";" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("ContactSurname")) & _
                UpdateBasketAndOrderStatus(basketStatusEntity, "PAYMENT INTERMEDIATE 3")
            Else
                If returnErrorCode.Trim.Length <= 0 Then
                    returnErrorCode = "No Error from talent, but no customer details found"
                End If
            End If
        Else
            returnErrorCode = "customer number is empty"
        End If
        If returnErrorCode.Trim.Length > 0 Then
            basketStatusEntity.Comment = returnErrorCode
        End If
    End Sub

    Private Sub EmailToUserOrderCancellation(ByVal basketStatusEntity As DEBasketStatus, ByVal cancelReasonCode As String, ByVal reasonMessage As String)
        LogNotificationProcess("000", "EmailToUserOrderCancellation")
        Dim returnErrorCode As String = String.Empty
        If Not String.IsNullOrWhiteSpace(basketStatusEntity.LoginId) Then
            Dim dtCustomerDetails As DataTable = TryGetTalentCustomerDetails(basketStatusEntity.LoginId, returnErrorCode)
            If dtCustomerDetails IsNot Nothing AndAlso dtCustomerDetails.Rows.Count > 0 Then
                Dim emailOrder As New Talent.eCommerce.Order_Email
                'emailOrder.SendTicketingConfirmationEmail(paymentRef)
                'basketStatusEntity.Comment = "ConfirmationEmail sent successfully;" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("CustomerNo")) & _
                '                        ";" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("EmailAddress")) & _
                '                        ";" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("ContactTitle")) & _
                '                        ";" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("ContactForename")) & _
                '                        ";" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("ContactSurname")) & _
            Else
                If returnErrorCode.Trim.Length <= 0 Then
                    returnErrorCode = "No Error from talent, but no customer details found"
                End If
            End If
        Else
            returnErrorCode = "customer number is empty"
        End If
        If returnErrorCode.Trim.Length > 0 Then
            basketStatusEntity.Comment = returnErrorCode
        End If
    End Sub

    Private Sub EmailOrderConfirmation(ByVal paymentRef As String, ByVal inputAuthorizationAmountNotification As GCheckout.AutoGen.AuthorizationAmountNotification, ByVal basketStatusEntity As DEBasketStatus)
        LogNotificationProcess("008", "EmailOrderConfirmation")
        Try
            Dim returnErrorCode As String = String.Empty
            If Not String.IsNullOrWhiteSpace(paymentRef) Then
                Dim dtCustomerDetails As DataTable = TryGetTalentCustomerDetails(basketStatusEntity.LoginId, returnErrorCode)
                If dtCustomerDetails IsNot Nothing AndAlso dtCustomerDetails.Rows.Count > 0 Then
                    Dim emailOrder As New Talent.eCommerce.Order_Email
                    emailOrder.SendTicketingConfirmationEmail(paymentRef, dtCustomerDetails)
                    basketStatusEntity.Comment = "ConfirmationEmail sent successfully;" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("CustomerNo")).Trim & _
                                            ";" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("EmailAddress")).Trim & _
                                            ";" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("ContactTitle")).Trim & _
                                            ";" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("ContactForename")).Trim & _
                                            ";" & TEBUtilities.CheckForDBNull_String(dtCustomerDetails.Rows(0)("ContactSurname")).Trim & _
                    UpdateBasketAndOrderStatus(basketStatusEntity, "ORDER PROCESSED")
                Else
                    If returnErrorCode.Trim.Length <= 0 Then
                        returnErrorCode = "No Error from talent, but no customer details found"
                    End If
                End If
            Else
                returnErrorCode = "Talent payment reference is empty"
            End If
            If returnErrorCode.Trim.Length > 0 Then
                basketStatusEntity.Comment = returnErrorCode
                UpdateBasketAndOrderStatus(basketStatusEntity, "PROCESS FAILED")
            End If
        Catch ex As Exception
            basketStatusEntity.Comment = ex.Message
            UpdateBasketAndOrderStatus(basketStatusEntity, "PROCESS FAILED")
        End Try

    End Sub

    Private Sub EmailPaymentSuccessOrderFailed(ByVal basketStatusEntity As DEBasketStatus, ByVal inputAuthorizationAmountNotification As GCheckout.AutoGen.AuthorizationAmountNotification)
        LogNotificationProcess("009", "EmailPaymentSuccessOrderFailed")
    End Sub

#End Region

#End Region

    
End Class
