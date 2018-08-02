Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.IO
Imports Talent.Common
Imports System.Data
Imports System.Data.SqlClient
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function.                    CSG Webservice class
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPWSWS- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Partial Public Class TalentWebService

        Private _webServiceName As String = String.Empty
        Private _responseName As String = String.Empty
        Private _transactionID As String = String.Empty

        '------------------------------------------------------------------------------------------
        Private Const resp As String = "Response"
        Private Const reqe As String = "Request"
        '
        Private Const AddLoyaltyPointsRequest As String = "AddLoyaltyPointsRequest"
        Private Const AddOrderTrackingRequest As String = "AddOrderTrackingRequest"
        Private Const AddTicketingItemsRequest As String = "AddTicketingItemsRequest"
        Private Const AddTicketingItemsReturnBasketRequest As String = "AddTicketingItemsReturnBasketRequest"
        Private Const AddTicketingReservedItemsRequest As String = "AddTicketingReservedItemsRequest"
        Private Const AmendBasketRequest As String = "AmendBasketRequest"
        Private Const AmendTicketingItemsRequest As String = "AmendTicketingItemsRequest"
        Private Const AmendTicketingItemsReturnBasketRequest As String = "AmendTicketingItemsReturnBasketRequest"
        Private Const AmendTemplateRequest As String = "AmendTemplateRequest"
        Private Const AvailabilityRequest As String = "AvailabilityRequest"
        Private Const CompletePNARequest As String = "CompletePNARequest"
        Private Const CreditNoteRequest As String = "CreditNoteRequest"
        Private Const CustRemittRequest As String = "CustomerRemittanceRequest"
        Private Const CustomerAddRequest As String = "CustomerAddRequest"
        Private Const CustomerRetrievalRequest As String = "CustomerRetrievalRequest"
        Private Const CustomerSearchRequest As String = "CustomerSearchRequest"
        Private Const CustomerAssociationsRequest As String = "CustomerAssociationsRequest"
        Private Const DispatchAdviceRequest As String = "DispatchAdviceRequest"
        Private Const DTypeOrderRequest As String = "DTypeOrderRequest"
        Private Const InvoiceRequest As String = "InvoiceRequest"
        Private Const GenerateTicketingBasketIDRequest As String = "GenerateTicketingBasketIDRequest"
        Private Const InvoiceStatusRequest As String = "InvoiceStatusRequest"
        Private Const MultiAvailabilityRequest As String = "MultiAvailabilityRequest"
        Private Const OrderChangeRequest As String = "OrderChangeRequest"
        Private Const OrderDetailRequest As String = "OrderDetailRequest"
        Private Const OrderDetailTicketingRequest As String = "OrderDetailTicketingRequest"
        Private Const OrderRequest As String = "OrderRequest"
        Private Const OrderStatus As String = "OrderStatus"
        Private Const OrderStatusRequest As String = "OrderStatusRequest"
        Private Const OrderTracking As String = "OrderTracking"
        Private Const OrderTrackingRequest As String = "OrderTrackingRequest"
        Private Const PaymentRequest As String = "PaymentRequest"
        Private Const PNARequest As String = "PNARequest"
        Private Const ProductAlertRequest As String = "ProductAlertRequest"
        Private Const ProductAlertOutboundRequest As String = "ProductAlertOutboundRequest"
        Private Const ProductListRequest As String = "ProductListRequest"
        Private Const ProductDetailsRequest As String = "ProductDetailsRequest"
        Private Const ProductPricingDetailsRequest As String = "ProductPricingDetailsRequest"
        Private Const ProductStadiumAvailabilityRequest As String = "ProductStadiumAvailabilityRequest"
        Private Const ProductSeatAvailabilityRequest As String = "ProductSeatAvailabilityRequest"
        Private Const ProductSeatNumbersRequest As String = "ProductSeatNumbersRequest"
        Private Const RefundPaymentRequest As String = "RefundPaymentRequest"
        Private Const RemoveTicketingItemsRequest As String = "RemoveTicketingItemsRequest"
        Private Const RemoveTicketingItemsReturnBasketRequest As String = "RemoveTicketingItemsReturnBasketRequest"
        Private Const RemoveExpiredTicketingBasketsRequest As String = "RemoveExpiredTicketingBasketsRequest"
        Private Const RetrieveTicketingItemsRequest As String = "RetrieveTicketingItemsRequest"
        Private Const RMAStatusRequest As String = "RMAStatusRequest"
        Private Const PurchaseOrderRequest As String = "PurchaseOrderRequest"
        Private Const SupplierInvoiceRequest As String = "SupplierInvoiceRequest"
        Private Const SupplierInvoiceAcceptedRequest As String = "SupplierInvoiceAcceptedRequest"
        Private Const SuppRemittRequest As String = "SupplierRemittanceRequest"
        Private Const UploadTemplatesRequest As String = "UploadTemplatesRequest"
        Private Const XmlLoadRequest As String = "XmlLoadRequest"
        Private Const TicketBookerRequest As String = "TicketBookerRequest"
        Private Const RemoveExpiredNoiseSessionsRequest As String = "RemoveExpiredNoiseSessionsRequest"
        Private Const RetrieveEligibleTicketingCustomersRequest As String = "RetrieveEligibleTicketingCustomersRequest"
        Private Const ProductNavigationLoadRequest As String = "ProductNavigationLoadRequest"
        Private Const ProductLoadRequest As String = "ProductLoadRequest"
        Private Const ProductOptionsLoadRequest As String = "ProductOptionsLoadRequest"
        Private Const ProductRelationsLoadRequest As String = "ProductRelationsLoadRequest"
        Private Const SupplierOrderAcknowledgementRequest As String = "SupplierOrderAcknowledgementRequest"
        Private Const ReceiveProfileManagerTransactionsRequest As String = "ReceiveProfileManagerTransactionsRequest"
        Private Const VerifyPasswordRequest As String = "VerifyPasswordRequest"
        Private Const RetrievePasswordRequest As String = "RetrievePasswordRequest"
        Private Const CustomerUpdateRequest As String = "CustomerUpdateRequest"
        Private Const SeasonTicketRenewalsRequest As String = "SeasonTicketRenewalsRequest"
        Private Const RetrievePromotionsRequest As String = "RetrievePromotionsRequest"
        Private Const AddCustomerAssociationsRequest As String = "AddCustomerAssociationsRequest"
        Private Const DeleteCustomerAssociationsRequest As String = "DeleteCustomerAssociationsRequest"
        Private Const SeasonTicketSaleRequest As String = "SeasonTicketSaleRequest"
        Private Const RetrievePurchaseHistoryRequest As String = "RetrievePurchaseHistoryRequest"
        Private Const RetrieveProfileDetailsRequest As String = "RetrieveProfileDetailsRequest"
        Private Const ProductListReturnAllRequest As String = "ProductListReturnAllRequest"
        Private Const ProductStockLoadRequest As String = "ProductStockLoadRequest"
        Private Const AddPPSEnrolmentRequest As String = "AddPPSEnrolmentRequest"
        Private Const AddTicketingSeasonTicketRenewalsRequest As String = "AddTicketingSeasonTicketRenewalsRequest"
        Private Const RetrieveTransactionProgressRequest As String = "RetrieveTransactionProgressRequest"
        Private Const ProductPriceLoadRequest As String = "ProductPriceLoadRequest"
        Private Const GeneratePasswordRequest As String = "GeneratePasswordRequest"
        Private Const SendEmailRequest As String = "SendEmailRequest"
        Private Const RetrieveOrdersByStatusRequest As String = "RetrieveOrdersByStatusRequest"
        Private Const DEXRedListRequest As String = "DEXRedListRequest"
        Private Const DEXWhiteListRequest As String = "DEXWhiteListRequest"
        Private Const ExternalSmartcardReprintRequest As String = "ExternalSmartcardReprintRequest"
        Private Const AmendTicketingOrderRequest As String = "AmendTicketingOrderRequest"
        Private Const RetrieveOrderByOrderIdRequest As String = "RetrieveOrderByOrderIdRequest"

        Public Property WebServiceName() As String
            Get
                Return _webServiceName
            End Get
            Set(ByVal value As String)
                _webServiceName = value
            End Set
        End Property
        Public Property ResponseName() As String
            Get
                Return _responseName
            End Get
            Set(ByVal value As String)
                _responseName = value
            End Set
        End Property
        Public Property TransactionID() As String
            Get
                Return _transactionID
            End Get
            Set(ByVal value As String)
                _transactionID = value
            End Set
        End Property
        Public Function InvokeWebService(ByVal loginId As String, _
                                     ByVal password As String, _
                                     ByVal company As String, _
                                     ByVal xmlString As String) As String

            Dim err As New ErrorObj
            Dim LoggingOn As Boolean = True
            Dim storeXml As Boolean = False
            Dim Defaults As New Defaults
            Dim senderID As String = String.Empty
            Dim businessUnit As String = String.Empty
            businessUnit = GetBusinessUnitFromURL(HttpContext.Current.Request.Url.ToString)

            Dim talLogging As New TalentLogging
            talLogging.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ConnectionString
            talLogging.GeneralLog(WebServiceName, "Request", xmlString, "WebService-Log")

            Const slash As String = "\"
            '--------------------------------------------------------------------------------------
            '   CODE FOR SPECIFYING LOGGING DETAILS
            '
            Dim xsltReqPath, xsltResPath, xsdReqPath, xsdResPath As String
            Dim location As String = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings("DocumentsFolder"))

            xsltReqPath = location & slash & ConfigurationManager.AppSettings("XSLTFolder") & _
                                     slash & ConfigurationManager.AppSettings("RequestsFolder")
            xsltResPath = location & slash & ConfigurationManager.AppSettings("XSLTFolder") & _
                                     slash & ConfigurationManager.AppSettings("ResponsesFolder")

            xsdReqPath = location & slash & ConfigurationManager.AppSettings("XSDFolder") & _
                                    slash & ConfigurationManager.AppSettings("RequestsFolder")
            xsdResPath = location & slash & ConfigurationManager.AppSettings("XSDFolder") & _
                                    slash & ConfigurationManager.AppSettings("ResponsesFolder")
            '--------------------------------------
            ' Talent common logging
            '
            Const loggingOnOffCacheKey As String = "TalentCommonLoggingOnOff"
            Const loggingConnectionStringCacheKey As String = "TalentCommonLoggingConnectionString"
            Const logPathCacheKey As String = "TalentCommonLogPath"
            Dim logPath As String = String.Empty
            If Not ConfigurationManager.AppSettings("TalentCommonLogPath") Is Nothing Then
                logPath = ConfigurationManager.AppSettings("TalentCommonLogPath")
            End If
            If Not ConfigurationManager.AppSettings("TalentCommonLoggingOnOff") Is Nothing AndAlso _
                    ConfigurationManager.AppSettings("TalentCommonLoggingOnOff").ToString.Trim().ToUpper = "TRUE" Then
                HttpContext.Current.Cache.Insert(loggingOnOffCacheKey, True, Nothing, System.DateTime.Now.AddMinutes(720), Caching.Cache.NoSlidingExpiration)
                HttpContext.Current.Cache.Insert(loggingConnectionStringCacheKey, ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ConnectionString, Nothing, System.DateTime.Now.AddMinutes(720), Caching.Cache.NoSlidingExpiration)
                HttpContext.Current.Cache.Insert(logPathCacheKey, logPath, Nothing, System.DateTime.Now.AddMinutes(720), Caching.Cache.NoSlidingExpiration)
            Else
                HttpContext.Current.Cache.Insert(loggingOnOffCacheKey, False, Nothing, System.DateTime.Now.AddMinutes(720), Caching.Cache.NoSlidingExpiration)
                HttpContext.Current.Cache.Insert(loggingConnectionStringCacheKey, "", Nothing, System.DateTime.Now.AddMinutes(720), Caching.Cache.NoSlidingExpiration)
                HttpContext.Current.Cache.Insert(logPathCacheKey, "", Nothing, System.DateTime.Now.AddMinutes(720), Caching.Cache.NoSlidingExpiration)
            End If
            '-------------------------------------------
            '   Check if a transaction ID has been passed within the XML document 
            '
            TransactionID = ExtractTransactionIDXmlDoc(xmlString)

            '-------------------------------------------
            '   Create a Defaults object
            '
            With Defaults
                .BusinessUnit = businessUnit
                If TransactionID = String.Empty Then
                    err = .GetTransactionNumber()
                    TransactionID = .TransactionNumber.ToString
                End If
            End With
            'LOG: web service has been invoked
            LogWriter.WriteDBLog(LoggingOn, TransactionID, "InvokeWebService", company, loginId, WebServiceName)

            '   Set up a global error message object
            '   Create an xmlResponse object of the relevant type
            Dim xmlResp As XmlResponse = Nothing

            Select Case WebServiceName()

                Case Is = AddLoyaltyPointsRequest
                    Dim xmlWork As New XmlAddLoyaltyPointsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = AddOrderTrackingRequest
                    Dim xmlWork As New XmlAddOrderTrackingResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = AddTicketingItemsRequest
                    Dim xmlWork As New XmlAddTicketingItemsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = AddTicketingItemsReturnBasketRequest
                    Dim xmlWork As New XmlAddTicketingItemsReturnBasketResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = AddTicketingReservedItemsRequest
                    Dim xmlWork As New XmlAddTicketingReservedItemsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = AmendBasketRequest
                    Dim xmlWork As New XmlAmendBasketResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = AmendTicketingItemsRequest
                    Dim xmlWork As New XmlAmendTicketingItemsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = AmendTicketingItemsReturnBasketRequest
                    Dim xmlWork As New XmlAmendTicketingItemsReturnBasketResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = AmendTemplateRequest
                    Dim xmlWork As New XmlAmendTemplateResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = AvailabilityRequest
                    Dim xmlWork As New XmlAvailabilityResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = CompletePNARequest
                    Dim xmlWork As New XmlCompletePNAResponse
                    xmlResp = CType(xmlWork, XmlCompletePNAResponse)

                Case Is = CreditNoteRequest
                    Dim xmlWork As New XmlCreditNoteResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = CustRemittRequest
                    Dim xmlWork As New XmlCustRemittResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = CustomerAddRequest
                    Dim xmlWork As New XmlCustomerAddResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = CustomerRetrievalRequest
                    Dim xmlWork As New XmlCustomerRetrievalResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = CustomerSearchRequest
                    Dim xmlWork As New XmlCustomerSearchResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = CustomerAssociationsRequest
                    Dim xmlWork As New XmlCustomerAssociationsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = DEXRedListRequest
                    Dim xmlWork As New xmlDEXRedListResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = DEXWhiteListRequest
                    Dim xmlWork As New XmlDEXWhiteListResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ExternalSmartcardReprintRequest
                    Dim xmlWork As New XmlExternalSmartcardReprintResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = DispatchAdviceRequest
                    Dim xmlWork As New XmlDispatchAdviceResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = DTypeOrderRequest
                    '-------------------------------------------------------------------
                    '       XmlDTypeOrderResponse is identical to XmlOrderResponse
                    '
                    '       So Order.DTypeOrderRequest passes control to XmlOrderResponse.vb
                    '
                    '       Thus save duplication
                    '-------------------------------------------------------------------
                    Dim xmlWork As New XmlOrderResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = GenerateTicketingBasketIDRequest
                    Dim xmlWork As New XmlGenerateTicketingBasketIDResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = InvoiceRequest
                    Dim xmlWork As New XmlInvoiceResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = InvoiceStatusRequest
                    Dim xmlWork As New XmlInvoiceStatusResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = MultiAvailabilityRequest
                    Dim xmlWork As New XmlMultiAvailabilityResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = OrderChangeRequest
                    Dim xmlWork As New XmlOrderChangeResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = OrderDetailRequest
                    Dim xmlWork As New XmlOrderDetailResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = OrderDetailTicketingRequest
                    Dim xmlWork As New XmlOrderDetailTicketingResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = OrderRequest
                    Dim xmlWork As New XmlOrderResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = OrderStatusRequest
                    Dim xmlWork As New XmlOrderStatusResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = OrderTrackingRequest
                    Dim xmlWork As New XmlOrderTrackingResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = PaymentRequest
                    Dim xmlWork As New XmlPaymentResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = PNARequest
                    Dim xmlWork As New XmlPNAResponse
                    xmlResp = CType(xmlWork, XmlPNAResponse)

                Case Is = ProductAlertRequest
                    Dim xmlWork As New XmlProductAlertResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ProductAlertOutboundRequest
                    Dim xmlWork As New XmlProductAlertOutboundResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ProductListRequest
                    Dim xmlWork As New XmlProductListResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ProductDetailsRequest
                    Dim xmlWork As New XmlProductDetailsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ProductPricingDetailsRequest
                    Dim xmlWork As New XmlProductPricingDetailsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RMAStatusRequest
                    Dim xmlWork As New XmlRMAStatusResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RemoveTicketingItemsRequest
                    Dim xmlWork As New XmlRemoveTicketingItemsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RemoveTicketingItemsReturnBasketRequest
                    Dim xmlWork As New XmlRemoveTicketingItemsReturnBasketResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RemoveExpiredTicketingBasketsRequest
                    Dim xmlWork As New XmlRemoveExpiredTicketingBasketsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RefundPaymentRequest
                    Dim xmlWork As New XmlRefundPaymentResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RetrieveTicketingItemsRequest
                    Dim xmlWork As New XmlRetrieveTicketingItemsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RetrieveOrdersByStatusRequest
                    Dim xmlWork As New XmlRetrieveOrdersByStatusResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ProductStadiumAvailabilityRequest
                    Dim xmlWork As New XmlProductStadiumAvailabilityResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ProductSeatAvailabilityRequest
                    Dim xmlWork As New XmlProductSeatAvailabilityResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ProductSeatNumbersRequest
                    Dim xmlWork As New XmlProductSeatNumbersResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = PurchaseOrderRequest
                    Dim xmlWork As New XmlPurchaseOrderResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = SupplierInvoiceRequest
                    Dim xmlWork As New XmlSupplierInvoiceResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = SupplierInvoiceAcceptedRequest
                    Dim xmlWork As New XmlSupplierInvoiceAcceptedResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = SuppRemittRequest
                    Dim xmlWork As New XmlSuppRemittResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = UploadTemplatesRequest
                    Dim xmlWork As New XmlUploadTemplatesResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = XmlLoadRequest
                    Dim xmlWork As New XmlLoadResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = TicketBookerRequest
                    Dim xmlWork As New XMLTicketBookerResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RemoveExpiredNoiseSessionsRequest
                    Dim xmlWork As New XmlRemoveExpiredNoiseSessionsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RetrieveEligibleTicketingCustomersRequest
                    Dim xmlWork As New XmlRetrieveEligibleTicketingCustomersResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ProductNavigationLoadRequest
                    Dim xmlWork As New XmlProductNavigationLoadResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ProductLoadRequest
                    Dim xmlWork As New XmlProductLoadResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ProductOptionsLoadRequest
                    Dim xmlWork As New XmlProductOptionsLoadResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ProductRelationsLoadRequest
                    Dim xmlWork As New XmlProductRelationsLoadResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = SupplierOrderAcknowledgementRequest
                    Dim xmlWork As New XmlSupplierOrderAcknowledgementResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ReceiveProfileManagerTransactionsRequest
                    Dim xmlWork As New XmlReceiveProfileManagerTransactionsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = VerifyPasswordRequest
                    Dim xmlWork As New XmlVerifyPasswordResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RetrievePasswordRequest
                    Dim xmlWork As New XmlRetrievePasswordResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = CustomerUpdateRequest
                    Dim xmlWork As New XmlCustomerUpdateResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = SeasonTicketRenewalsRequest
                    Dim xmlWork As New XmlSeasonTicketRenewalsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RetrievePromotionsRequest
                    Dim xmlWork As New XmlRetrievePromotionsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = AddCustomerAssociationsRequest
                    Dim xmlWork As New XmlAddCustomerAssociationsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = DeleteCustomerAssociationsRequest
                    Dim xmlWork As New XmlDeleteCustomerAssociationsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = SeasonTicketSaleRequest
                    Dim xmlWork As New XmlSeasonTicketSaleResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RetrievePurchaseHistoryRequest
                    Dim xmlWork As New XmlRetrievePurchaseHistoryResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RetrieveProfileDetailsRequest
                    Dim xmlWork As New XmlRetrieveProfileDetailsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ProductListReturnAllRequest
                    Dim xmlWork As New XmlProductListReturnAllResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ProductStockLoadRequest
                    Dim xmlWork As New XmlProductStockLoadResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = AddPPSEnrolmentRequest
                    Dim xmlWork As New XmlAddPPSEnrolmentResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = AddTicketingSeasonTicketRenewalsRequest
                    Dim xmlWork As New XmlAddTicketingSeasonTicketRenewalsResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RetrieveTransactionProgressRequest
                    Dim xmlWork As New XmlRetrieveTransactionProgressResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = ProductPriceLoadRequest
                    Dim xmlWork As New XmlProductPriceLoadResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = GeneratePasswordRequest
                    Dim xmlWork As New XmlGeneratePasswordResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = SendEmailRequest
                    Dim xmlWork As New XmlSendEmailResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = AmendTicketingOrderRequest
                    Dim xmlWork As New XmlAmendTicketingOrderResponse
                    xmlResp = CType(xmlWork, XmlResponse)

                Case Is = RetrieveOrderByOrderIdRequest
                    Dim xmlWork As New XmlRetrieveOrderByOrderIdResponse
                    xmlResp = CType(xmlWork, XmlResponse)

            End Select
            '--------------------------------------------------------------------------------------------
            '   LOG: Response has been created
            '
            LogWriter.WriteDBLog(LoggingOn, TransactionID, "L0005", resp)

            '   Set generic properties upon the xmlResponse object Root Element

            With xmlResp
                .TransactionID = TransactionID
                .RootElement = ResponseName()
                .LoginId = loginId
                .WebServiceName = WebServiceName
                .Company = company
            End With

            'LOG: Attempting to retrieve the credentials
            LogWriter.WriteDBLog(LoggingOn, TransactionID, "L0105", resp)

            ' If  loginId credentials not provided as part of the request, extract from the XML document (which assumes 
            ' document will be in our standard XML format).
            err = ExtractCredentialsFromXmlDoc(senderID, loginId, password, company, xmlString)

            If err.HasError Then

                'Set response variables
                xmlResp.Err = err
                xmlResp.DocumentVersion = "1.0"
                'Create the response
                xmlResp.CreateResponse()

                'LOG: Error retrieving credentials... Response Error sent
                LogWriter.WriteDBLog(LoggingOn, TransactionID, "L0110", resp)

            Else
                'LOG: Credentials retrieved successfully ... Attempting to create profile
                LogWriter.WriteDBLog(LoggingOn, TransactionID, "L0115", reqe)

                '------------------------
                'Authenticate & Authorise
                '------------------------
                'Profile object holds all relevant details from the database for the  loginId used. e.g. XSLT style sheet, 
                'XML version number, etc.
                Dim pr As New Profile
                pr.BusinessUnit = businessUnit
                pr.Company = company
                err = pr.CreateProfile(loginId, password, company, WebServiceName())

                LogWriter.WriteDBLog(LoggingOn, TransactionID, "La115", reqe)
                If err.HasError Then
                    LogWriter.WriteDBLog(LoggingOn, TransactionID, err.ErrorMessage, err.ErrorStatus, err.ErrorNumber)
                    'Set response variables
                    xmlResp.Err = err
                    xmlResp.DocumentVersion = "1.0"
                    'Create the response
                    xmlResp.CreateResponse()

                Else

                    'LOG: Profile created... Checking User Validity
                    LogWriter.WriteDBLog(LoggingOn, TransactionID, "L0120", reqe)

                    'Set properties upon the xmlResponse object
                    'Response version - will default to 1.0 (within Profile object) if not set
                    xmlResp.DocumentVersion = pr.ResponseVersion()

                    'Not a valid user or not authorised to service - return response containing error.
                    If Not pr.ValidUser Then

                        'Set response variables
                        xmlResp.Err = err
                        'Create the response
                        xmlResp.CreateResponse()

                        'LOG: User not valid... Response sent
                        LogWriter.WriteDBLog(pr.WriteLog, TransactionID, "L0125", resp, String.Empty, String.Empty, String.Empty, String.Empty, _
                                                xsltResPath & slash & pr.OutgoingStyleSheet, xsdResPath & slash & pr.Xsd)

                    Else
                        'Valid user, so:

                        '
                        ' Validate that the transaction ID is unique
                        '
                        err = validateTransactionNumber(TransactionID)
                        If err.HasError Then
                            'Set response variables
                            xmlResp.Err = err
                            'Create the response
                            xmlResp.CreateResponse()
                        Else

                            'LOG: user valid... Defaults are loading                        
                            LogWriter.WriteDBLog(pr.WriteLog, TransactionID, "L0130", reqe, String.Empty, String.Empty, String.Empty, String.Empty, _
                                                    xsltReqPath & slash & pr.IncomingStyleSheet, xsdReqPath & slash & pr.Xsd)
                            '
                            With Defaults
                                .BusinessUnit = businessUnit
                                .WebServiceName = WebServiceName
                                .Company = company
                                ' Override Defaults with profile settings
                                .DestinationDatabase = pr.WebServiceDestinationDatabase()
                                .Xsd = pr.Xsd
                                .CacheTimeMinutes = pr.CacheTimeMinutes()
                                .WriteLog = pr.WriteLog
                                .StoreXml = pr.StoreXml
                                .DatabaseType1 = pr.DatabaseType1
                                err = .GetDefaults()
                                '
                            End With
                            '
                            With xmlResp
                                'path to store the untransformed doc if store xml = true
                                .UntransformedDocPath = Defaults.XmlResponseDocPathU
                                'path to store the transformed doc if store xml = true
                                .TransformedDocPath = Defaults.XmlResponseDocPathT
                                .WriteLog = Defaults.WriteLog
                                .StoreXml = Defaults.StoreXml
                                .EmailFrom = Defaults.EmailFrom & String.Empty
                                .EmailCompany = pr.EmailCompany & String.Empty
                                .EmailUser = pr.EmailUser & String.Empty
                                .EmailXmlResponse = pr.EmailXmlResponse
                            End With
                            '-----------------------------------------------------------------------
                            '   If there was a problem with loading the Defaults - return response containing error
                            '
                            If err.HasError() Then
                                'Set response variables
                                xmlResp.Err = err
                                'Create the response
                                xmlResp.CreateResponse()
                                'LOG: error loading Defaults... response sent
                                LogWriter.WriteDBLog(Defaults.WriteLog, TransactionID, "L0135", resp, _
                                                    Defaults.XmlRequestDocPathU, Defaults.XmlRequestDocPathT, _
                                                    Defaults.XmlResponseDocPathU, Defaults.XmlResponseDocPathT, xsltResPath & slash & _
                                                    pr.OutgoingStyleSheet, xsdResPath & slash & pr.Xsd)
                            Else
                                'LOG: Defaults loaded sucessfully... generating request
                                LogWriter.WriteDBLog(Defaults.WriteLog, TransactionID, "L0140", reqe, _
                                                    Defaults.XmlRequestDocPathU, Defaults.XmlRequestDocPathT, _
                                                    Defaults.XmlResponseDocPathU, Defaults.XmlResponseDocPathT, xsltResPath & slash & _
                                                    pr.OutgoingStyleSheet, xsdReqPath & slash & pr.Xsd)
                                '-------------------------------------------------------------------------------------
                                'Create a request object - this will validate the document and apply XSLT if necessary
                                '-------------------------------------------------------------------------------------
                                'Create an xmlRequest object of the relevant type
                                Dim xmlReq As XmlRequest = Nothing
                                Select Case WebServiceName()

                                    Case Is = AddLoyaltyPointsRequest
                                        Dim xmlWork As New XmlAddLoyaltyPointsRequest
                                        xmlReq = CType(xmlWork, XmlAddLoyaltyPointsRequest)

                                    Case Is = AddOrderTrackingRequest
                                        Dim xmlWork As New XmlAddOrderTrackingRequest
                                        xmlReq = CType(xmlWork, XmlAddOrderTrackingRequest)

                                    Case Is = AddTicketingItemsRequest
                                        Dim xmlWork As New XmlAddTicketingItemsRequest
                                        xmlReq = CType(xmlWork, XmlAddTicketingItemsRequest)

                                    Case Is = AddTicketingItemsReturnBasketRequest
                                        Dim xmlWork As New XmlAddTicketingItemsReturnBasketRequest
                                        xmlReq = CType(xmlWork, XmlAddTicketingItemsReturnBasketRequest)

                                    Case Is = AddTicketingReservedItemsRequest
                                        Dim xmlWork As New XmlAddTicketingReservedItemsRequest
                                        xmlReq = CType(xmlWork, XmlAddTicketingReservedItemsRequest)

                                    Case Is = AmendBasketRequest
                                        Dim xmlWork As New XmlAmendBasketRequest
                                        xmlReq = CType(xmlWork, XmlAmendBasketRequest)

                                    Case Is = AmendTicketingItemsRequest
                                        Dim xmlWork As New XmlAmendTicketingItemsRequest
                                        xmlReq = CType(xmlWork, XmlAmendTicketingItemsRequest)

                                    Case Is = AmendTicketingItemsReturnBasketRequest
                                        Dim xmlWork As New XmlAmendTicketingItemsReturnBasketRequest
                                        xmlReq = CType(xmlWork, XmlAmendTicketingItemsReturnBasketRequest)

                                    Case Is = AmendTemplateRequest
                                        Dim xmlWork As New XmlAmendTemplateRequest
                                        xmlReq = CType(xmlWork, XmlAmendTemplateRequest)

                                    Case Is = AvailabilityRequest
                                        'Stuart20070529
                                        'Dim xmlWork As New XmlAvailabilityRequest
                                        Dim xmlWork As New XmlAvailabilityRequest(WebServiceName())
                                        'Stuart20070529
                                        xmlReq = CType(xmlWork, XmlAvailabilityRequest)

                                    Case Is = CompletePNARequest
                                        Dim xmlWork As New XmlCompletePNARequest()
                                        xmlReq = CType(xmlWork, XmlCompletePNARequest)


                                    Case Is = CreditNoteRequest
                                        Dim xmlWork As New XmlCreditNoteRequest
                                        xmlReq = CType(xmlWork, XmlCreditNoteRequest)

                                    Case Is = CustRemittRequest
                                        Dim xmlWork As New XmlCustRemittRequest
                                        xmlReq = CType(xmlWork, XmlCustRemittRequest)

                                    Case Is = CustomerAddRequest
                                        Dim xmlWork As New XmlCustomerAddRequest
                                        xmlReq = CType(xmlWork, XmlCustomerAddRequest)

                                    Case Is = CustomerRetrievalRequest
                                        Dim xmlWork As New XmlCustomerRetrievalRequest
                                        xmlReq = CType(xmlWork, XmlCustomerRetrievalRequest)

                                    Case Is = CustomerSearchRequest
                                        Dim xmlWork As New XmlCustomerSearchRequest
                                        xmlReq = CType(xmlWork, XmlCustomerSearchRequest)

                                    Case Is = CustomerAssociationsRequest
                                        Dim xmlWork As New XmlCustomerAssociationsRequest
                                        xmlReq = CType(xmlWork, XmlCustomerAssociationsRequest)

                                    Case Is = DEXRedListRequest
                                        Dim xmlWork As New XmlDEXRedListRequest
                                        xmlReq = CType(xmlWork, XmlDEXRedListRequest)

                                    Case Is = DEXWhiteListRequest
                                        Dim xmlWork As New XmlDEXWhiteListRequest
                                        xmlReq = CType(xmlWork, XmlDEXWhiteListRequest)

                                    Case Is = ExternalSmartcardReprintRequest
                                        Dim xmlWork As New XmlExternalSmartcardReprintRequest
                                        xmlReq = CType(xmlWork, XmlExternalSmartcardReprintRequest)

                                    Case Is = DispatchAdviceRequest
                                        Dim xmlWork As New XmlDispatchAdviceRequest
                                        xmlReq = CType(xmlWork, XmlDispatchAdviceRequest)

                                    Case Is = DTypeOrderRequest
                                        Dim xmlWork As New XmlDTypeOrderRequest
                                        xmlReq = CType(xmlWork, XmlDTypeOrderRequest)

                                    Case Is = GenerateTicketingBasketIDRequest
                                        Dim xmlWork As New XmlGenerateTicketingBasketIDRequest()
                                        xmlReq = CType(xmlWork, XmlGenerateTicketingBasketIDRequest)

                                    Case Is = InvoiceRequest
                                        Dim xmlWork As New XmlInvoiceRequest()
                                        xmlReq = CType(xmlWork, XmlInvoiceRequest)

                                    Case Is = InvoiceStatusRequest
                                        Dim xmlWork As New XmlInvoiceStatusRequest
                                        xmlReq = CType(xmlWork, XmlInvoiceStatusRequest)

                                    Case Is = MultiAvailabilityRequest
                                        Dim xmlWork As New XmlMultiAvailabilityRequest
                                        xmlReq = CType(xmlWork, XmlMultiAvailabilityRequest)

                                    Case Is = OrderChangeRequest
                                        Dim xmlWork As New XmlOrderChangeRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlOrderChangeRequest)

                                    Case Is = OrderDetailRequest
                                        Dim xmlWork As New XmlOrderDetailRequest
                                        xmlReq = CType(xmlWork, XmlOrderDetailRequest)

                                    Case Is = OrderDetailTicketingRequest
                                        Dim xmlWork As New XmlOrderDetailTicketingRequest
                                        xmlReq = CType(xmlWork, XmlOrderDetailTicketingRequest)

                                    Case Is = OrderRequest
                                        Dim xmlWork As New XmlOrderRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlOrderRequest)

                                    Case Is = OrderStatusRequest
                                        Dim xmlWork As New XmlOrderStatusRequest()
                                        xmlReq = CType(xmlWork, XmlOrderStatusRequest)

                                    Case Is = OrderTrackingRequest
                                        Dim xmlWork As New XmlOrderTrackingRequest()
                                        xmlReq = CType(xmlWork, XmlOrderTrackingRequest)

                                    Case Is = PaymentRequest
                                        Dim xmlWork As New XmlPaymentRequest()
                                        xmlReq = CType(xmlWork, XmlPaymentRequest)

                                    Case Is = PNARequest
                                        Dim xmlWork As New XmlPNARequest()
                                        xmlReq = CType(xmlWork, XmlPNARequest)

                                    Case Is = ProductAlertOutboundRequest
                                        Dim xmlWork As New XmlProductAlertOutboundRequest
                                        xmlReq = CType(xmlWork, XmlProductAlertOutboundRequest)

                                    Case Is = ProductListRequest
                                        Dim xmlWork As New XmlProductListRequest
                                        xmlReq = CType(xmlWork, XmlProductListRequest)

                                    Case Is = ProductDetailsRequest
                                        Dim xmlWork As New XmlProductDetailsRequest
                                        xmlReq = CType(xmlWork, XmlProductDetailsRequest)

                                    Case Is = ProductPricingDetailsRequest
                                        Dim xmlWork As New XmlProductPricingDetailsRequest
                                        xmlReq = CType(xmlWork, XmlProductPricingDetailsRequest)

                                    Case Is = ProductAlertRequest
                                        Dim xmlWork As New XmlProductAlertRequest
                                        xmlReq = CType(xmlWork, XmlProductAlertRequest)

                                    Case Is = ProductStadiumAvailabilityRequest
                                        Dim xmlWork As New XmlProductStadiumAvailabilityRequest
                                        xmlReq = CType(xmlWork, XmlProductStadiumAvailabilityRequest)

                                    Case Is = ProductSeatAvailabilityRequest
                                        Dim xmlWork As New XmlProductSeatAvailabilityRequest
                                        xmlReq = CType(xmlWork, XmlProductSeatAvailabilityRequest)

                                    Case Is = ProductSeatNumbersRequest
                                        Dim xmlWork As New XmlProductSeatNumbersRequest
                                        xmlReq = CType(xmlWork, XmlProductSeatNumbersRequest)

                                    Case Is = PurchaseOrderRequest
                                        Dim xmlWork As New XmlPurchaseOrderRequest
                                        xmlReq = CType(xmlWork, XmlPurchaseOrderRequest)

                                    Case Is = RMAStatusRequest
                                        Dim xmlWork As New XmlRMAStatusRequest
                                        xmlReq = CType(xmlWork, XmlRMAStatusRequest)

                                    Case Is = RefundPaymentRequest
                                        Dim xmlWork As New XmlRefundPaymentRequest
                                        xmlReq = CType(xmlWork, XmlRefundPaymentRequest)

                                    Case Is = RemoveTicketingItemsRequest
                                        Dim xmlWork As New XmlRemoveTicketingItemsRequest
                                        xmlReq = CType(xmlWork, XmlRemoveTicketingItemsRequest)

                                    Case Is = RemoveTicketingItemsReturnBasketRequest
                                        Dim xmlWork As New XmlRemoveTicketingItemsReturnBasketRequest
                                        xmlReq = CType(xmlWork, XmlRemoveTicketingItemsReturnBasketRequest)

                                    Case Is = RemoveExpiredTicketingBasketsRequest
                                        Dim xmlWork As New XmlRemoveExpiredTicketingBasketsRequest
                                        xmlReq = CType(xmlWork, XmlRemoveExpiredTicketingBasketsRequest)

                                    Case Is = RetrieveOrdersByStatusRequest
                                        Dim xmlWork As New XmlRetrieveOrdersByStatusRequest
                                        xmlReq = CType(xmlWork, XmlRetrieveOrdersByStatusRequest)

                                    Case Is = RetrieveTicketingItemsRequest
                                        Dim xmlWork As New XmlRetrieveTicketingItemsRequest
                                        xmlReq = CType(xmlWork, XmlRetrieveTicketingItemsRequest)

                                    Case Is = SupplierInvoiceRequest
                                        Dim xmlWork As New XmlSupplierInvoiceRequest
                                        xmlReq = CType(xmlWork, XmlSupplierInvoiceRequest)

                                    Case Is = SupplierInvoiceAcceptedRequest
                                        Dim xmlWork As New XmlSupplierInvoiceAcceptedRequest
                                        xmlReq = CType(xmlWork, XmlSupplierInvoiceAcceptedRequest)

                                    Case Is = SuppRemittRequest
                                        Dim xmlWork As New XmlSuppRemittRequest
                                        xmlReq = CType(xmlWork, XmlSuppRemittRequest)

                                    Case Is = UploadTemplatesRequest
                                        Dim xmlWork As New XmlUploadTemplatesRequest
                                        xmlReq = CType(xmlWork, XmlUploadTemplatesRequest)

                                    Case Is = XmlLoadRequest
                                        Dim xmlWork As New XmlLoadRequest
                                        xmlReq = CType(xmlWork, XmlLoadRequest)

                                    Case Is = TicketBookerRequest
                                        Dim xmlWork As New XMLTicketBookerRequest
                                        xmlReq = CType(xmlWork, XMLTicketBookerRequest)

                                    Case Is = RemoveExpiredNoiseSessionsRequest
                                        Dim xmlWork As New XmlRemoveExpiredNoiseSessionsRequest
                                        xmlReq = CType(xmlWork, XmlRemoveExpiredNoiseSessionsRequest)

                                    Case Is = RetrieveEligibleTicketingCustomersRequest
                                        Dim xmlWork As New XmlRetrieveEligibleTicketingCustomersRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlRetrieveEligibleTicketingCustomersRequest)

                                    Case Is = ProductNavigationLoadRequest
                                        Dim xmlWork As New XmlProductNavigationLoadRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlProductNavigationLoadRequest)

                                    Case Is = ProductLoadRequest
                                        Dim xmlWork As New XmlProductLoadRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlProductLoadRequest)

                                    Case Is = ProductOptionsLoadRequest
                                        Dim xmlWork As New XmlProductOptionsLoadRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlProductOptionsLoadRequest)

                                    Case Is = ProductRelationsLoadRequest
                                        Dim xmlWork As New XmlProductRelationsLoadRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlProductRelationsLoadRequest)

                                    Case Is = SupplierOrderAcknowledgementRequest
                                        Dim xmlWork As New XmlSupplierOrderAcknowledgementRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlSupplierOrderAcknowledgementRequest)

                                    Case Is = ReceiveProfileManagerTransactionsRequest
                                        Dim xmlWork As New XmlReceiveProfileManagerTransactionsRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlReceiveProfileManagerTransactionsRequest)

                                    Case Is = VerifyPasswordRequest
                                        Dim xmlWork As New XmlVerifyPasswordRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlVerifyPasswordRequest)

                                    Case Is = RetrievePasswordRequest
                                        Dim xmlWork As New XmlRetrievePasswordRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlRetrievePasswordRequest)

                                    Case Is = CustomerUpdateRequest
                                        Dim xmlWork As New XmlCustomerUpdateRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlCustomerUpdateRequest)

                                    Case Is = SeasonTicketRenewalsRequest
                                        Dim xmlWork As New XMLSeasonTicketRenewalsRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XMLSeasonTicketRenewalsRequest)

                                    Case Is = RetrievePromotionsRequest
                                        Dim xmlWork As New XmlRetrievePromotionsRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlRetrievePromotionsRequest)

                                    Case Is = AddCustomerAssociationsRequest
                                        Dim xmlWork As New XmlAddCustomerAssociationsRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlAddCustomerAssociationsRequest)

                                    Case Is = DeleteCustomerAssociationsRequest
                                        Dim xmlWork As New XmlDeleteCustomerAssociationsRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlDeleteCustomerAssociationsRequest)

                                    Case Is = SeasonTicketSaleRequest
                                        Dim xmlWork As New XmlSeasonTicketSaleRequest()
                                        xmlReq = CType(xmlWork, XmlSeasonTicketSaleRequest)

                                    Case Is = RetrievePurchaseHistoryRequest
                                        Dim xmlWork As New XmlRetrievePurchaseHistoryRequest()
                                        xmlReq = CType(xmlWork, XmlRetrievePurchaseHistoryRequest)

                                    Case Is = RetrieveProfileDetailsRequest
                                        Dim xmlWork As New XmlRetrieveProfileDetailsRequest()
                                        xmlReq = CType(xmlWork, XmlRequest)

                                    Case Is = ProductListReturnAllRequest
                                        Dim xmlWork As New XmlProductListReturnAllRequest()
                                        xmlReq = CType(xmlWork, XmlProductListReturnAllRequest)

                                    Case Is = ProductStockLoadRequest
                                        Dim xmlWork As New XmlProductStockLoadRequest(WebServiceName())
                                        xmlReq = CType(xmlWork, XmlProductStockLoadRequest)

                                    Case Is = AddPPSEnrolmentRequest
                                        Dim xmlWork As New XmlAddPPSEnrolmentRequest()
                                        xmlReq = CType(xmlWork, XmlAddPPSEnrolmentRequest)

                                    Case Is = AddTicketingSeasonTicketRenewalsRequest
                                        Dim xmlWork As New XmlAddTicketingSeasonTicketRenewalsRequest()
                                        xmlReq = CType(xmlWork, XmlAddTicketingSeasonTicketRenewalsRequest)

                                    Case Is = RetrieveTransactionProgressRequest
                                        Dim xmlWork As New XmlRetrieveTransactionProgressRequest()
                                        xmlReq = CType(xmlWork, XmlRetrieveTransactionProgressRequest)

                                    Case Is = ProductPriceLoadRequest
                                        Dim xmlWork As New XmlProductPriceLoadRequest(WebServiceName)
                                        xmlReq = CType(xmlWork, XmlProductPriceLoadRequest)

                                    Case Is = GeneratePasswordRequest
                                        Dim xmlWork As New XmlGeneratePasswordRequest(WebServiceName)
                                        xmlReq = CType(xmlWork, XmlGeneratePasswordRequest)

                                    Case Is = SendEmailRequest
                                        Dim xmlWork As New XmlSendEmailRequest(WebServiceName)
                                        xmlReq = CType(xmlWork, XmlSendEmailRequest)

                                    Case Is = AmendTicketingOrderRequest
                                        Dim xmlWork As New XmlAmendTicketingOrderRequest()
                                        xmlReq = CType(xmlWork, XmlAmendTicketingOrderRequest)

                                    Case Is = RetrieveOrderByOrderIdRequest
                                        Dim xmlWork As New XmlRetrieveOrderByOrderIdRequest
                                        xmlReq = CType(xmlWork, XmlRetrieveOrderByOrderIdRequest)

                                End Select
                                '-----------------------------------------------------------------------
                                With xmlReq
                                    'Set generic properties upon the xmlRequest object
                                    .ApplyStyleSheet = pr.ApplyIncomingStyleSheet()                 ' Apply Style Sheet?
                                    .DefaultCurrentVersion = Defaults.DefaultCurrentVersion()       ' Default current version
                                    .IncomingStyleSheet = pr.IncomingStyleSheet()                   ' Incoming Style Sheet
                                    .LoginId = loginId                                              ' logged in user
                                    .Company = company
                                    .Password = password
                                    .RootElement = WebServiceName()                                 ' Root Element
                                    .StoreXml = Defaults.StoreXml                                   ' Store XML True of False
                                    .TransformedDocPath = Defaults.XmlRequestDocPathT               ' path to store the transformed doc if store xml = true
                                    .TransactionID = TransactionID
                                    .UntransformedDocPath = Defaults.XmlRequestDocPathU             ' path to store the untransformed doc if store xml = true
                                    .WriteLog = Defaults.WriteLog                                   ' WriteLog True or False
                                    .Xsd = pr.Xsd                                                   ' XSD Document
                                    With .Settings
                                        .AccountNo1 = Defaults.AccountNo1                           ' account number part 1
                                        .AccountNo2 = Defaults.AccountNo2                           ' account number part 2
                                        .AccountNo3 = Defaults.AccountNo3                           ' account number part 3
                                        .AccountNo4 = Defaults.AccountNo4                           ' account number part 4
                                        .AccountNo5 = Defaults.AccountNo5                           ' account number part 5

                                        .BusinessUnit = businessUnit
                                        '  .BusinessUnit = ConfigurationManager.AppSettings("DefaultBusinessUnit").ToString
                                        .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("SYSTEM21").ToString
                                        .Cacheing = Defaults.Cacheing()                             ' Cacheing?
                                        .CacheTimeMinutes = pr.CacheTimeMinutes()                   ' Cache Time
                                        .Company = company                                          ' Company
                                        .DatabaseType1 = Defaults.DatabaseType1()                   ' Database type
                                        .DestinationDatabase = Defaults.DestinationDatabase()       ' Destination Database
                                        ' BF - Now the same DB
                                        .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                                        '.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005_CMS").ToString
                                        .SenderID = senderID                                        ' SenderID
                                        .WebServiceName = WebServiceName                            ' service instance name
                                        .RetryFailures = Defaults.RetryFailures                     ' Retry Failures on/off
                                        .RetryAttempts = Defaults.RetryAttempts                     ' Retry attempts
                                        .RetryWaitTime = Defaults.RetryWaitTime                     ' Retry wait time
                                        .RetryErrorNumbers = Defaults.RetryErrorNumbers             ' Retry error numbers
                                        ' Set Stored Procedure Group from DB, or if not found, from WebConfig
                                        If Not String.IsNullOrEmpty(pr.WebServiceStoredProcedureGroup) Then
                                            .StoredProcedureGroup = pr.WebServiceStoredProcedureGroup
                                        Else
                                            .StoredProcedureGroup = ConfigurationManager.AppSettings("DefaultStoredProcedureGroup")
                                        End If
                                        .ResponseDirectory = pr.ResponseDirectory
                                        .LogRequests = pr.LogRequests
                                        .TransactionID = TransactionID
                                        .LoginId = loginId
                                        .Partner = company
                                        .OriginatingSourceCode = "S"
                                    End With

                                    'Stuart20070529
                                    'Set module specific settings
                                    Select Case WebServiceName
                                        Case Is = AvailabilityRequest
                                            Dim ss As DEStockSettings
                                            ss = CType(.Settings, DEStockSettings)
                                            ss.TestProperty = "Test"
                                        Case Is = RetrieveEligibleTicketingCustomersRequest
                                            Dim ss As DEProductSettings
                                            ss = CType(.Settings, DEProductSettings)
                                        Case Is = RetrievePromotionsRequest
                                            Dim ss As DEPromotionSettings
                                            ss = CType(.Settings, DEPromotionSettings)
                                        Case Is = OrderRequest, OrderChangeRequest
                                            Dim ss As DEOrderSettings
                                            ss = CType(.Settings, DEOrderSettings)
                                            ss.RepriceBlankPrice = Defaults.RepriceBlankPrice
                                            ss.OrderCheckForAltProducts = Defaults.OrderCheckForAltProducts
                                    End Select
                                    'Stuart20070529

                                    err = .CreateRequest(xmlString)
                                End With
                                ' Invalid document - return response containing error
                                With xmlResp
                                    If Not xmlReq.ValidDocument Then
                                        'LOG: request invalid... sending response
                                        LogWriter.WriteDBLog(Defaults.WriteLog, TransactionID, "L0245", reqe, _
                                                            Defaults.XmlRequestDocPathU, Defaults.XmlRequestDocPathT, _
                                                            Defaults.XmlResponseDocPathU, Defaults.XmlResponseDocPathT, xsltResPath & slash & _
                                                            pr.OutgoingStyleSheet, xsdReqPath & slash & pr.Xsd)
                                        .Err = err                                                  ' Set response variables
                                        .LoginId = loginId                                          ' logged in user
                                        .WebServiceName = WebServiceName                            ' service name
                                        .Company = company                                          ' company
                                        .CreateResponse()                                           ' Create the response
                                        'LOG: Response sent
                                        LogWriter.WriteDBLog(Defaults.WriteLog, TransactionID, "L0250", resp, _
                                                             Defaults.XmlRequestDocPathU, Defaults.XmlRequestDocPathT, _
                                                             Defaults.XmlResponseDocPathU, Defaults.XmlResponseDocPathT, xsltResPath & slash & _
                                                             pr.OutgoingStyleSheet, xsdResPath & slash & pr.Xsd)
                                    Else
                                        'LOG: request valid... accessing DB
                                        LogWriter.WriteDBLog(Defaults.WriteLog, TransactionID, "L0255", reqe, _
                                                            Defaults.XmlRequestDocPathU, Defaults.XmlRequestDocPathT, _
                                                            Defaults.XmlResponseDocPathU, Defaults.XmlResponseDocPathT, xsltResPath & slash & _
                                                            pr.OutgoingStyleSheet, xsdReqPath & slash & pr.Xsd)
                                        'Valid document, so:
                                        '---------------------------------------------------------------------------------------------
                                        'Access the backend Database - pass the reference of the Response object to the Request object
                                        '---------------------------------------------------------------------------------------------
                                        xmlResp = xmlReq.AccessDatabase(xmlResp)

                                        'If no error occured during database access, continue
                                        If Not err.HasError Then

                                            'LOG: DB accessed without error... testing apply outgoing stylesheet
                                            LogWriter.WriteDBLog(Defaults.WriteLog, TransactionID, "L0260", resp, _
                                                            Defaults.XmlRequestDocPathU, Defaults.XmlRequestDocPathT, _
                                                            Defaults.XmlResponseDocPathU, Defaults.XmlResponseDocPathT, xsltResPath & slash & _
                                                            pr.OutgoingStyleSheet, xsdResPath & slash & pr.Xsd)

                                            '---------------------------------------------------------------------------------
                                            'Apply Outgoing XSLT Style Sheet - to convert standard XML doc to 3rd party schema
                                            '
                                            If pr.ApplyOutgoingStyleSheet Then
                                                'Set properties upon the Response object
                                                .OutgoingStyleSheet = pr.OutgoingStyleSheet()

                                                'path to store the untransformed doc if store xml = true
                                                .UntransformedDocPath = Defaults.XmlResponseDocPathU
                                                'path to store the transformed doc if store xml = true
                                                .TransformedDocPath = Defaults.XmlResponseDocPathT

                                                'Apply the outgoing transalation
                                                .ApplyOutgoingStyleSheet()
                                                LogWriter.WriteDBLog(Defaults.WriteLog, TransactionID, "L0290", resp, _
                                                            Defaults.XmlRequestDocPathU, Defaults.XmlRequestDocPathT, _
                                                            Defaults.XmlResponseDocPathU, Defaults.XmlResponseDocPathT, xsltResPath & slash & _
                                                            pr.OutgoingStyleSheet, xsdResPath & slash & pr.Xsd)
                                            Else
                                                LogWriter.WriteDBLog(Defaults.WriteLog, TransactionID, "L0295", resp, _
                                                            Defaults.XmlRequestDocPathU, Defaults.XmlRequestDocPathT, _
                                                            Defaults.XmlResponseDocPathU, Defaults.XmlResponseDocPathT, xsltResPath & slash & _
                                                            pr.OutgoingStyleSheet, xsdResPath & slash & pr.Xsd)
                                            End If
                                            '----------------------------------------------------------
                                            'Web Service specific elelements, after response is created
                                            ' 
                                            err = PostResponseSpecifics(xmlResp)
                                        End If
                                    End If
                                End With
                            End If
                            'LOG: Web Service Complete
                            LogWriter.WriteDBLog(Defaults.WriteLog, TransactionID, "L0300", resp, _
                                                            Defaults.XmlRequestDocPathU, Defaults.XmlRequestDocPathT, _
                                                            Defaults.XmlResponseDocPathU, Defaults.XmlResponseDocPathT, xsltResPath & slash & _
                                                            pr.OutgoingStyleSheet, xsdResPath & slash & pr.Xsd)
                            '------------------------------------------
                        End If
                    End If
                End If
            End If

            talLogging.GeneralLog(WebServiceName, "Response", xmlResp.GetXmlResponseAsString(), "WebService-Log")
            Return xmlResp.GetXmlResponseAsString()
        End Function
        Private Function PostResponseSpecifics(ByVal xmlResp As XmlResponse) As ErrorObj
            Const Module_name As String = "PostResponseSpecifics"
            Dim err As New ErrorObj
            '------------------------------------------------------------------------
            '   Send Order Acknowledgement Transaction (and email - on switch)
            '
            Dim _XmlResponseNumber As String = String.Empty
            Dim _XmlResponse As String = xmlResp.GetXmlResponseAsString()
            '
            Try
                Select Case WebServiceName()

                    Case Is = DispatchAdviceRequest
                        xmlResp.OrderNumber = Talent.Common.Utilities.xmlExtract(_XmlResponse, "DispatchAdviceNumber")
                        xmlResp.WebServiceDesc = "Order Acknowledgement "


                    Case Is = InvoiceRequest
                        xmlResp.OrderNumber = Talent.Common.Utilities.xmlExtract(_XmlResponse, "InvoiceNumber")
                        xmlResp.WebServiceDesc = "Invoice Acknowledgement "


                    Case Is = OrderRequest
                        xmlResp.OrderNumber = Talent.Common.Utilities.xmlExtract(_XmlResponse, "BranchOrderNumber")
                        xmlResp.WebServiceDesc = "Order Request Acknowledgement"

                        'Case Is = XmlLoadRequest
                        '    Dim cmd As New System.Data.SqlClient.SqlCommand
                        '    cmd.Connection = New System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
                        '    Try
                        '        cmd.Connection.Open()

                        '        '--------------------------------------
                        '        '       UPDATE ADDRESS LINES
                        '        '--------------------------------------
                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address		SET	TYPE=''					WHERE	(TYPE IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address		SET	SEQUENCE=0				WHERE	(SEQUENCE IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address		SET	DEFAULT_ADDRESS='True'			WHERE	(DEFAULT_ADDRESS IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address		SET	ADDRESS_LINE_1=''			WHERE	(ADDRESS_LINE_1 IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address		SET	ADDRESS_LINE_2=''			WHERE	(ADDRESS_LINE_2 IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address		SET	ADDRESS_LINE_3=''			WHERE	(ADDRESS_LINE_3 IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address		SET	ADDRESS_LINE_4=''			WHERE	(ADDRESS_LINE_4 IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address		SET	ADDRESS_LINE_5=''			WHERE	(ADDRESS_LINE_5 IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address		SET	POST_CODE=''				WHERE	(POST_CODE IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address		SET	COUNTRY='United Kingdom'		WHERE	(COUNTRY IS NULL)"
                        '        cmd.ExecuteNonQuery()
                        '        '---------------------------------------

                        '        '---------------------------------------
                        '        '       UPDATE USER DETAILS
                        '        '---------------------------------------
                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	TITLE=''				WHERE	(TITLE IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	INITIALS=''				WHERE	(INITIALS IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	FORENAME=''				WHERE	(FORENAME IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	SURNAME=''				WHERE	(SURNAME IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	FULL_NAME=FORENAME + ' ' + SURNAME	WHERE	(FULL_NAME IS NULL)  "
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	SALUTATION=''				WHERE	(SALUTATION IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	POSITION=''				WHERE	(POSITION IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	DOB=''					WHERE	(DOB IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	MOBILE_NUMBER=''			WHERE	(MOBILE_NUMBER IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	TELEPHONE_NUMBER=''			WHERE	(TELEPHONE_NUMBER IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	WORK_NUMBER=''				WHERE	(WORK_NUMBER IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	FAX_NUMBER=''				WHERE	(FAX_NUMBER IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	OTHER_NUMBER=''				WHERE	(OTHER_NUMBER IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	MESSAGING_ID=''				WHERE	(MESSAGING_ID IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	ORIGINATING_BUSINESS_UNIT=''		WHERE	(ORIGINATING_BUSINESS_UNIT IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	ACCOUNT_NO_2=''				WHERE	(ACCOUNT_NO_2 IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	ACCOUNT_NO_3=''				WHERE	(ACCOUNT_NO_3 IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	ACCOUNT_NO_4=''				WHERE	(ACCOUNT_NO_4 IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	ACCOUNT_NO_5=''				WHERE	(ACCOUNT_NO_5 IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	SUBSCRIBE_NEWSLETTER='False'		WHERE	(SUBSCRIBE_NEWSLETTER IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	HTML_NEWSLETTER='False'			WHERE	(HTML_NEWSLETTER IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	BIT1='False'				WHERE	(BIT1 IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	BIT2='False'				WHERE	(BIT2 IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	BIT3='False'				WHERE	(BIT3 IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	BIT4='False'				WHERE	(BIT4 IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_partner_user		SET	BIT5='False'				WHERE	(BIT5 IS NULL)"
                        '        cmd.ExecuteNonQuery()
                        '        '-------------------------------------

                        '        '-------------------------------------
                        '        '       UPDATE USER ACCOUNT TABLE
                        '        '-------------------------------------
                        '        'cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_authorized_users	SET	BUSINESS_UNIT='B2C'"
                        '        'cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE  talentebusinessdb.dbo.tbl_authorized_users 	SET     AUTO_PROCESS_DEFAULT_USER = 'False' 	WHERE 	(AUTO_PROCESS_DEFAULT_USER is null)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE  talentebusinessdb.dbo.tbl_authorized_users	SET     IS_APPROVED = 'True'			WHERE 	(IS_APPROVED IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE  talentebusinessdb.dbo.tbl_authorized_users	SET     IS_LOCKED_OUT = 'False'			WHERE 	(IS_LOCKED_OUT IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE  talentebusinessdb.dbo.tbl_authorized_users	SET	LAST_PASSWORD_CHANGED_DATE = CREATED_DATE	WHERE	(LAST_PASSWORD_CHANGED_DATE IS NULL)"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE  talentebusinessdb.dbo.tbl_authorized_users	SET	LAST_LOCKED_OUT_DATE = CREATED_DATE		WHERE	(LAST_LOCKED_OUT_DATE IS NULL)"
                        '        cmd.ExecuteNonQuery()
                        '        '---------------------------------

                        '        '---------------------------------
                        '        '       MISC UPDATES
                        '        '---------------------------------


                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address		" & _
                        '                            " SET	ADDRESS_LINE_4=ADDRESS_LINE_3, ADDRESS_LINE_3='' " & _
                        '                            " WHERE	(ADDRESS_LINE_3<>'' and ADDRESS_LINE_4='') "
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address		" & _
                        '                            " SET	ADDRESS_LINE_3=ADDRESS_LINE_2, ADDRESS_LINE_2='' " & _
                        '                            " WHERE	(ADDRESS_LINE_2<>'' and ADDRESS_LINE_3='') "
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address		" & _
                        '                            " SET	ADDRESS_LINE_2=ADDRESS_LINE_1, ADDRESS_LINE_1='' " & _
                        '                            " WHERE	(ADDRESS_LINE_1<>'' and ADDRESS_LINE_2='') "
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address	" & _
                        '                            " SET	ADDRESS_LINE_2 = substring((rtrim(ADDRESS_LINE_1) + ', ' + rtrim(ADDRESS_LINE_2)), 1, 50), ADDRESS_LINE_1='' " & _
                        '                            " WHERE	(ADDRESS_LINE_1<>'') "
                        '        cmd.ExecuteNonQuery()


                        '        cmd.CommandText = "UPDATE	talentebusinessdb.dbo.tbl_address		SET	REFERENCE=ADDRESS_LINE_2				WHERE	(REFERENCE IS NULL) OR (REFERENCE = '')"
                        '        cmd.ExecuteNonQuery()

                        '        cmd.CommandText = "update talentebusinessdb.dbo.tbl_authorized_users set tbl_authorized_users.PASSWORD =  " & _
                        '                           " ( " & _
                        '                           " select ACCOUNT_NO_1  " & _
                        '                           " from tbl_partner_user  " & _
                        '                           " WHERE ACCOUNT_NO_1 is not null and ACCOUNT_NO_1 <> '' and ACCOUNT_NO_1 <> '0' " & _
                        '                           " and tbl_authorized_users.LOGINID = tbl_partner_user.LOGINID " & _
                        '                           " ) " & _
                        '                           " WHERE tbl_authorized_users.PASSWORD = 'password' "
                        '        cmd.ExecuteNonQuery()

                        'Catch ex As Exception
                        'Finally
                        'cmd.Connection.Close()
                        'End Try

                    Case Is = TicketBookerRequest
                        ' temp code for uefa import - write out doc to c:\TEMP\
                        Try
                            Talent.Common.Utilities.WriteToLog(_XmlResponse)
                        Catch ex As Exception

                        End Try

                    Case Is = SeasonTicketRenewalsRequest
                        Try
                            Dim filename As String = DateTime.Now.Year.ToString & "-" & DateTime.Now.Month.ToString & "-" & _
                                                DateTime.Now.Day.ToString & "-" & DateTime.Now.Hour.ToString & DateTime.Now.Minute.ToString & _
                                                DateTime.Now.Second.ToString & DateTime.Now.Millisecond & ".xml"
                            Dim w As System.IO.StreamWriter
                            w = New System.IO.StreamWriter("C:\Temp\Responses\" & filename, True)
                            With w
                                .AutoFlush = True
                                .WriteLine(_XmlResponse)
                                .Close()
                            End With
                        Catch ex As Exception

                        End Try


                End Select

                If Not String.IsNullOrEmpty(xmlResp.ResponseDirectory) Then
                    Try
                        'Dim filename As String = Module_name & "_" & xmlResp.TransactionID & ".xml"
                        Dim filename As String = xmlResp.TransactionID & ".xml"
                        Dim w As System.IO.StreamWriter
                        w = New System.IO.StreamWriter(xmlResp.ResponseDirectory & filename, True)
                        With w
                            .AutoFlush = True
                            .WriteLine(_XmlResponse)
                            .Close()
                        End With
                    Catch ex As Exception

                    End Try
                End If

                '
                If xmlResp.EmailXmlResponse Then _
                        err = Email_Send(xmlResp)

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorNumber = "TTPWSWS-04"
                    .ErrorStatus = Module_name & " error "
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function Email_Send(ByVal xmlResp As XmlResponse) As ErrorObj
            Const Module_name As String = "EmailResponse"
            Dim err As New ErrorObj
            '------------------------------------------------------------------------
            Try
                Dim _address As String = xmlResp.EmailCompany.Trim
                If _address.Length < 7 Then _
                    _address = xmlResp.EmailUser
                '
                Utilities.SMTP = ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim
                '
                If Utilities.ValidateEmail(_address) Then
                    err = Utilities.Email_Send(xmlResp.EmailFrom, _
                                    _address, _
                                    xmlResp.WebServiceDesc & xmlResp.OrderNumber, _
                                    WebServiceName.Trim, _
                                    xmlResp.GetXmlResponseAsString, True)
                    '
                End If
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorNumber = "TTPWSWS-04"
                    .ErrorStatus = Module_name & " error "
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function ExtractTransactionIDXmlDoc(ByRef xmlString As String) As String
            Dim transactionIDStr As String
            transactionIDStr = String.Empty

            Try
                Dim xDoc As New XmlDocument
                xDoc.LoadXml(xmlString)

                For Each bNode As XmlNode In xDoc.SelectSingleNode("//TransactionHeader").ChildNodes
                    If bNode.Name = "TransactionID" Then
                        transactionIDStr = bNode.InnerXml
                    End If
                Next
            Catch ex As Exception
            End Try

            Return transactionIDStr
        End Function


        Private Function validateTransactionNumber(ByRef transactionIDStr As String) As ErrorObj
            'Const Module_name As String = "validateTransactionNumber"
            Dim err As New ErrorObj

            Dim conTalent As SqlConnection
            Dim param1 As String = "@Param1"
            Const SqlServer2005 As String = "SqlServer2005"

            Const strSelect1 As String = "SELECT * FROM tbl_supplynet_requests WHERE Transaction_ID = @Param1"

            Try
                conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                conTalent.Open()
                Dim cmdSelect As SqlCommand = New SqlCommand(strSelect1, conTalent)
                cmdSelect.Parameters.Add(New SqlParameter(param1, SqlDbType.Char, 50)).Value = transactionIDStr
                Dim reader As SqlDataReader = cmdSelect.ExecuteReader()
                If reader.HasRows Then
                    With err
                        .ErrorMessage = String.Empty
                        .ErrorNumber = "TTPWSWS-05"
                        .ErrorStatus = "Transaction Number has already been used "
                        .HasError = True
                    End With
                End If
                conTalent.Close()

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorNumber = "TTPWSWS-06"
                    .ErrorStatus = "Could not determine if Transaction ID is unique "
                    .HasError = True
                End With

            End Try

            Return err
        End Function

        Private Function ExtractCredentialsFromXmlDoc(ByRef senderID As String, _
                                            ByRef loginId As String, _
                                            ByRef password As String, _
                                            ByRef company As String, _
                                            ByRef xmlString As String) As ErrorObj
            Const Module_name As String = "ExtractCredentialsFromXmlDoc"
            Dim err As New ErrorObj

            Const sSenderID As String = "SenderID"
            Const sLoginID As String = "LoginID"
            Const sPassword As String = "Password"
            Const sCompany As String = "Company"
            '----------------------------------------------------------------------------
            If (loginId Is Nothing And password Is Nothing And company Is Nothing) _
                Or (loginId.Equals(String.Empty) And password.Equals(String.Empty) And company.Equals(String.Empty)) Then
                Try
                    Dim xr As New XmlTextReader(New StringReader(xmlString))
                    xr.MoveToContent()
                    While (xr.Read())
                        Select Case xr.Name
                            Case Is = sSenderID
                                senderID = xr.ReadString()
                            Case Is = sLoginID
                                loginId = xr.ReadString()        ' the word  loginId is a reserved work
                            Case Is = sPassword
                                password = xr.ReadString()
                            Case Is = sCompany
                                company = xr.ReadString()
                                Exit While
                        End Select
                    End While
                    If loginId.Equals(String.Empty) _
                        Or password.Equals(String.Empty) _
                        Or company.Equals(String.Empty) _
                        Or loginId Is Nothing _
                        Or password Is Nothing _
                        Or company Is Nothing Then


                        '*************************************************************************************
                        '   cXML - If the credentials are not in Supplynet Standard format, check cXML format
                        '*************************************************************************************
                        Try
                            Dim xDoc As New XmlDocument
                            xDoc.LoadXml(xmlString)

                            Dim n1 As XmlNode = xDoc.SelectSingleNode("cXML/Header/Sender")

                            For Each n2 As XmlNode In n1.ChildNodes
                                Select Case n2.Name
                                    Case "Credential"
                                        company = n2.Attributes("domain").Value
                                        For Each n3 As XmlNode In n2.ChildNodes
                                            Select Case n3.Name
                                                Case "Identity"
                                                    loginId = n3.InnerText
                                                Case "SharedSecret"
                                                    password = n3.InnerText
                                            End Select
                                        Next
                                End Select
                            Next
                        Catch ex As Exception
                        End Try
                        '*************************************************************************************

                        If loginId.Equals(String.Empty) _
                        Or password.Equals(String.Empty) _
                        Or company.Equals(String.Empty) _
                        Or loginId Is Nothing _
                        Or password Is Nothing _
                        Or company Is Nothing Then
                            With err
                                .ErrorMessage = String.Empty
                                .ErrorNumber = "TTPWSWS-02"
                                .ErrorStatus = Module_name & " error "
                                .HasError = True
                            End With

                        End If

                    End If
                Catch ex As Exception
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorNumber = "TTPWSWS-01"
                        .ErrorStatus = Module_name & " error "
                        .HasError = True
                    End With

                End Try
            End If
            Return err
        End Function
        Private Function GetBusinessUnitFromURL(ByVal url As String) As String
            Dim businessUnit As String = String.Empty
            Dim nextUrl As String = String.Empty
            Dim urlSplit() As String '
            '----------------------------------------
            'Get the url and split it on the "/" char
            '----------------------------------------
            urlSplit = url.Split("/")
            '--------------------------------------------------------------------
            ' Check for localhost:port (debugging locally) and treat as localhost
            '--------------------------------------------------------------------
            Dim j As Integer = 0
            If HttpContext.Current.Request.Url.ToString.Contains("localhost:") Then
                Do While j < url.Length
                    If urlSplit(j).Contains("localhost:") Then
                        urlSplit(j) = "localhost"
                        Exit Do
                    End If
                    j += 1
                Loop
            End If

            Dim conTalent As SqlConnection = Nothing
            Const SqlServer2005 As String = "SqlServer2005"
            Dim cmdSelect As SqlCommand = Nothing
            Dim dtrURL As SqlDataReader = Nothing
            Try


                conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                conTalent.Open()

                Const SQLString1 As String = "SELECT URL_BU_ID, URL, BUSINESS_UNIT, APPLICATION, BU_GROUP FROM dbo.tbl_url_bu WITH (NOLOCK)  " & _
                                                    "WHERE APPLICATION = @PARM2 "
                '---------------------------------------------------------------------------------
                'Loop backwards through the length of the array and check the DB for business unit
                '---------------------------------------------------------------------------------
                For i As Integer = urlSplit.Length - 1 To 0 Step -1
                    nextUrl = GetNextURLString(urlSplit, i)
                    cmdSelect = New SqlCommand(SQLString1, conTalent)
                    'cmdSelect.Parameters.Add(New SqlParameter("PARM1", SqlDbType.Char)).Value = nextUrl
                    cmdSelect.Parameters.Add(New SqlParameter("PARM2", SqlDbType.Char)).Value = "SupplyNet"
                    dtrURL = cmdSelect.ExecuteReader
                    If dtrURL.Read Then
                        businessUnit = dtrURL("BUSINESS_UNIT").ToString
                        Exit For
                    End If
                    dtrURL.Close()
                    dtrURL = Nothing
                Next

            Catch ex As Exception
            Finally
                Try
                    dtrURL.Close()
                    dtrURL = Nothing

                Catch ex As Exception
                Finally
                    conTalent.Close()
                    conTalent = Nothing
                End Try
            End Try
            '----------------------------------------------------------------------------
            ' If business unit is not found on DB then get it from web config (as before)
            '----------------------------------------------------------------------------
            If businessUnit = String.Empty Then
                businessUnit = ConfigurationManager.AppSettings("DefaultBusinessUnit").ToString
            End If

            Return businessUnit

        End Function

        Protected Shared Function GetNextURLString(ByVal url() As String, ByVal i As Integer) As String
            Dim nextURL As String = String.Empty
            'Contruct the url string
            For j As Integer = 0 To i
                If url(j) = "http:" OrElse url(j) = "https:" OrElse url(j) = "" Then
                Else
                    nextURL += url(j) & "/"
                End If
            Next

            'Remove the end "/"
            If nextURL.EndsWith("/") Then
                nextURL = nextURL.TrimEnd("/")
            End If

            Return nextURL
        End Function
    End Class

End Namespace