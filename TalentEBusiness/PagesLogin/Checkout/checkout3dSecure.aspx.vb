Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports TECUtilities = Talent.eCommerce.Utilities
Imports System.Xml
Imports System.Xml.Serialization
Imports CardinalCommerce
Imports Talent.Common
'Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesLogin_checkout3dSecure
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _talentLogging As New Talent.Common.TalentLogging

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim referrerUrl As String = String.Empty
        Try
            referrerUrl = Request.UrlReferrer.AbsoluteUri
        Catch ex As Exception
            referrerUrl = ex.Message
        End Try
        _talentLogging.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load", "Customer: " & Profile.User.Details.LoginID & ", Referrer URL: " & referrerUrl, "3dSecureLogging")
        Dim iframe3dSecureWidth As String = ModuleDefaults.Payment3DSecureIFrameWidth
        iframe3dSecure.Attributes("width") = iframe3dSecureWidth

        Select Case ModuleDefaults.Payment3DSecureProvider
            Case Is = "RETAILLOGIC" : iframe3dSecure.Attributes("src") = "Checkout3dSecureIframeRetailLogic.aspx"
            Case Is = "HSBC"
                If Session("HSBCRequest") Is Nothing Then
                    Response.Redirect("Checkout.aspx")
                Else
                    iframe3dSecure.Attributes("src") = "checkout3dSecureIframeHSBC.aspx"
                End If
            Case Is = "COMMIDEA" : processCommidea3DSecure()
            Case Is = "COMMIDEA-MULTI-ACCOUNT-ID" : processCommideaMultiMerchant3DSecure()
            Case Is = "WINBANK" : processWinbank3DSecure()
            Case Is = GlobalConstants.ECENTRICGATEWAY : processECentric3DSecure()
            Case Is = GlobalConstants.PAYMENTGATEWAY_VANGUARD : processVanguard3DSecure()
                'Phase 2 Case Is = GlobalConstants.PAYMENTGATEWAY_VANGUARD_MULTI : processVanguard3DSecure()
        End Select
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Process 3D Secure using Commidea as the provider
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub processCommidea3DSecure()
        Try
            Dim priceListHeaderTableAdapter As New TalentBasketDatasetTableAdapters.tbl_price_list_headerTableAdapter
            Dim priceListHeaderTable As Data.DataTable = priceListHeaderTableAdapter.GetPriceListHeaderByPriceList(ModuleDefaults.PriceList)
            Dim currenciesTableAdapter As New TalentApplicationVariablesTableAdapters.tbl_currencyTableAdapter
            Dim currenciesTable As Data.DataTable = currenciesTableAdapter.GetDataByCurrencyCode(priceListHeaderTable.Rows(0)("CURRENCY_CODE").ToString)

            '--------------------------------------
            ' Enrolment Check (WebService Call)
            '--------------------------------------

            Dim ourAccountId As String = ModuleDefaults.Payment3dSecureOurAccountId '"10006780"
            Dim ourSystemGUID As String = ModuleDefaults.Payment3dSecureOurSystemGUID '"C9423C55-1500-4425-AC75-DDAD06281DBE"
            Dim ourSystemId As String = ModuleDefaults.Payment3dSecureOurSystemId '"10002485"
            Dim ourPasscode As String = ModuleDefaults.Payment3dSecureOurPasscode '"12232377"

            Dim merchantreference As String = Profile.Basket.Basket_Header_ID ' Unique Generated Merchant Reference
            Dim mkaccountid As String = ModuleDefaults.Payment3DSecureDetails1 '"10004271" 'Profile.Basket.TempOrderID.Trim
            Dim mkacquirerid As String = ModuleDefaults.Payment3DSecureDetails2 '"999999"
            Dim merchantname As String = ModuleDefaults.Payment3DSecureDetails3 '"IRIS Talent4Sport"
            Dim merchantcountrycode As String = "826" ' UK
            Dim merchanturl As String = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.LastIndexOf("PagesLogin")) '"http://www.club.talent-sport.co.uk" 'Retrieve from current URL

            Dim cardnumber As String = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)
            Dim expDate As String = Checkout.RetrievePaymentItemFromSession("ExpiryDate", GlobalConstants.CHECKOUTASPXSTAGE)
            If expDate.Length = 5 Then
                expDate = "0" & expDate
            ElseIf expDate.Length = 4 Then
                expDate = expDate
            Else
                expDate = expDate.Substring(4, 2) & expDate.Substring(0, 2)
            End If
            Dim cardexpyear As String = expDate.Substring(2, 2) 'Card expiry year (YY)
            Dim cardexpmonth As String = expDate.Substring(0, 2) 'Card expiry month (MM)

            ' Get the currency code
            Dim currencycode As String = "826" 'Retrieve as per Retail Logic
            If currenciesTable.Rows.Count > 0 AndAlso TECUtilities.CheckForDBNull_String(currenciesTable.Rows(0)("CURRENCY_CODE_1").ToString) <> String.Empty Then
                currencycode = currenciesTable.Rows(0)("CURRENCY_CODE_1").ToString
            End If

            Dim currencyexponent As String = "2" 'No of decimal places in currency. Retrieve as per Retail Logic
            Dim transactionamount As String = CInt((Profile.Basket.BasketSummary.TotalBasket) * 100).ToString 'Transaction amount in pence
            Dim transactiondisplayamount As String = Profile.Basket.BasketSummary.TotalBasket 'Amount to display to customer
            Dim visaMerchantBankId As String = ModuleDefaults.Payment3DSecureDetails4 '123456
            Dim visaMerchantNumber As String = ModuleDefaults.Payment3DSecureDetails5 '22270438
            Dim visaMerchantPassword As String = ModuleDefaults.Payment3DSecureDetails6 '12345678
            Dim mcmMerchantBankId As String = ModuleDefaults.Payment3DSecureDetails7 '542515
            Dim mcmMerchantNumber As String = ModuleDefaults.Payment3DSecureDetails8 '83589362
            Dim mcmMerchantPassword As String = ModuleDefaults.Payment3DSecureDetails9 '12345678
            Dim dd As String = """"
            Dim MsgData As String

            MsgData = _
            "<?xml version=""1.0"" encoding=""utf-8""?>" & _
            "<payerauthenrollmentcheckrequest xmlns:xsi=" & dd & "http://www.w3.org/2001/XMLSchema-instance" & dd & " xmlns=" & dd & "PAYERAUTH" & dd & ">" & _
                "<mkaccountid>" & mkaccountid & "</mkaccountid>" & _
                "<mkacquirerid>" & mkacquirerid & "</mkacquirerid>" & _
                "<merchantname>" & merchantname & "</merchantname>" & _
                "<merchantcountrycode>" & merchantcountrycode & "</merchantcountrycode>" & _
                "<merchanturl>" & merchanturl & "</merchanturl>" & _
                 "<visamerchantbankid>" & visaMerchantBankId & "</visamerchantbankid>" & _
                    "<visamerchantnumber>" & visaMerchantNumber & "</visamerchantnumber>" & _
                    "<visamerchantpassword>" & visaMerchantPassword & "</visamerchantpassword>" & _
                    "<mcmmerchantbankid>" & mcmMerchantBankId & "</mcmmerchantbankid>" & _
                    "<mcmmerchantnumber>" & mcmMerchantNumber & "</mcmmerchantnumber>" & _
                    "<mcmmerchantpassword>" & mcmMerchantPassword & "</mcmmerchantpassword>"
            If cardnumber.Contains("**") Then
                Dim payType As String = Checkout.RetrievePaymentItemFromSession("PaymentType", GlobalConstants.CHECKOUTASPXSTAGE)
                If payType = GlobalConstants.SAVEDCARDPAYMENTTYPE Then
                    Dim tokenid As String = Checkout.RetrievePaymentItemFromSession("VGTokenID", GlobalConstants.CHECKOUTASPXSTAGE)
                    MsgData = MsgData & "<tokenid>" & tokenid & "</tokenid>"
                End If
            End If
            MsgData = MsgData & "<cardnumber>" & cardnumber & "</cardnumber>" & _
            "<cardexpyear>" & cardexpyear & "</cardexpyear>" & _
            "<cardexpmonth>" & cardexpmonth & "</cardexpmonth>" & _
            "<currencycode>" & currencycode & "</currencycode>" & _
            "<currencyexponent>" & currencyexponent & "</currencyexponent>" & _
            "<transactionamount>" & transactionamount & "</transactionamount>" & _
            "<transactiondisplayamount>" & transactiondisplayamount & "</transactiondisplayamount>" & _
        "</payerauthenrollmentcheckrequest>"

            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load", "Customer: " & Profile.User.Details.LoginID & ", MsgData: " & MsgData, "3dSecureLogging")

            Dim ClientHeader As com.commidea.testweb.ClientHeader = New com.commidea.testweb.ClientHeader()

            ClientHeader.CDATAWrapping = True
            ClientHeader.SystemGUID = ourSystemGUID
            ClientHeader.SystemID = ourSystemId
            ClientHeader.Passcode = ourPasscode
            ClientHeader.SendAttempt = 0
            ClientHeader.CDATAWrapping = False 'Sets whether the respose will be wrapped in CDATA tags

            Dim Message As com.commidea.testweb.Message = New com.commidea.testweb.Message
            Message.MsgData = MsgData
            Message.MsgType = "PAI"

            Message.ClientHeader = ClientHeader

            Dim CommideaGateway As com.commidea.testweb.CommideaGateway = New com.commidea.testweb.CommideaGateway
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
            CommideaGateway.Url = ModuleDefaults.Payment3DSecureURL1
            Dim ProcessMsg As com.commidea.testweb.Message = CommideaGateway.ProcessMsg(Message)

            Dim Resp As String = ProcessMsg.MsgData.ToString()

            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load", "Customer: " & Profile.User.Details.LoginID & ", Response: " & Resp, "3dSecureLogging")

            'Extract Values from the Response
            Session("MerchantReference") = getFromXML("merchantreference", Resp)
            Dim RespProcessingDB As String = getFromXML("processingdb", Resp)
            Session("ProcessingDB") = getFromXML("processingdb", Resp)
            Session("PayerAuthRequestId") = getFromXML("payerauthrequestid", Resp)
            Session("Enrolled") = getFromXML("enrolled", Resp)
            Session("AcsUrl") = getFromXML("acsurl", Resp)
            Session("PAReq") = getFromXML("pareq", Resp)
            Dim RespErrorCode As String = getFromXML("errorcode", Resp)
            Dim RespErrorDescription As String = getFromXML("errordescription", Resp)

            'Set Return URL
            Dim currentUrl As String = Request.Url.ToString
            Session("TermUrl") = currentUrl.Substring(0, currentUrl.IndexOf("PagesLogin")) & _
                        "Redirect/PaymentGateway.aspx?page=checkout3dSecure.aspx&function=checkout3dSecure"

            Session("MD") = Profile.Basket.Temp_Order_Id 'Temp Order Id

            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load", "Customer: " & Profile.User.Details.LoginID & ", Redirect URL: " & Session("TermUrl"), "3dSecureLogging")

            If Session("Enrolled") = "Y" Then
                iframe3dSecure.Attributes("src") = "Checkout3dSecureIFrameCommidea.aspx"
            Else
                Response.Redirect(Session("TermUrl"))
            End If
        Catch ex As Exception
            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load - Commidea Try-Catch", "Customer: " & Profile.User.Details.LoginID & ", Exception: " & ex.Message, "3dSecureLogging")
        End Try
    End Sub

    ''' <summary>
    ''' Process 3D Secure using Commidea as the provider but as a Multi merchant ID setup
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub processCommideaMultiMerchant3DSecure()
        Try
            Dim tDataObjects As New Talent.Common.TalentDataObjects
            Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.DestinationDatabase = "SQL2005"
            settings.BusinessUnit = BusinessUnit
            settings.Partner = PartnerCode
            tDataObjects.Settings = settings
            tDataObjects.PaymentSettings.TblPaymentDefaults.GetPaymentDefaults(Profile.Basket.PAYMENT_ACCOUNT_ID)

            Dim priceListHeaderTableAdapter As New TalentBasketDatasetTableAdapters.tbl_price_list_headerTableAdapter
            Dim priceListHeaderTable As Data.DataTable = priceListHeaderTableAdapter.GetPriceListHeaderByPriceList(ModuleDefaults.PriceList)
            Dim currenciesTableAdapter As New TalentApplicationVariablesTableAdapters.tbl_currencyTableAdapter
            Dim currenciesTable As Data.DataTable = currenciesTableAdapter.GetDataByCurrencyCode(priceListHeaderTable.Rows(0)("CURRENCY_CODE").ToString)
            iframe3dSecure.Attributes("width") = tDataObjects.PaymentSettings.TblPaymentDefaults.IFrameWidth
            '--------------------------------------
            ' Enrolment Check (WebService Call)
            '--------------------------------------

            Dim ourAccountId As String = tDataObjects.PaymentSettings.TblPaymentDefaults.OurAccountID
            Dim ourSystemGUID As String = tDataObjects.PaymentSettings.TblPaymentDefaults.OurSystemGUID
            Dim ourSystemId As String = tDataObjects.PaymentSettings.TblPaymentDefaults.OurSystemID
            Dim ourPasscode As String = tDataObjects.PaymentSettings.TblPaymentDefaults.OurPasscode
            Dim merchantreference As String = Profile.Basket.Basket_Header_ID ' Unique Generated Merchant Reference
            Dim mkaccountid As String = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentDetails1
            Dim mkacquirerid As String = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentDetails2
            Dim merchantname As String = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentDetails3
            Dim merchantcountrycode As String = "826" ' UK
            Dim merchanturl As String = Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.LastIndexOf("PagesLogin")) '"http://www.club.talent-sport.co.uk" 'Retrieve from current URL

            Dim cardnumber As String = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)
            Dim expDate As String = Checkout.RetrievePaymentItemFromSession("ExpiryDate", GlobalConstants.CHECKOUTASPXSTAGE)
            expDate = Checkout.DecodePaymentDetails(expDate)
            If expDate.Length = 5 Then
                expDate = "0" & expDate
            ElseIf expDate.Length = 4 Then
                expDate = expDate
            Else
                expDate = expDate.Substring(4, 2) & expDate.Substring(0, 2)
            End If
            expDate = expDate.Substring(4, 2) & expDate.Substring(0, 2)
            Dim cardexpyear As String = expDate.Substring(2, 2) 'Card expiry year (YY)
            Dim cardexpmonth As String = expDate.Substring(0, 2) 'Card expiry month (MM)

            ' Get the currency code
            Dim currencycode As String = "826" 'Retrieve as per Retail Logic
            If currenciesTable.Rows.Count > 0 AndAlso TECUtilities.CheckForDBNull_String(currenciesTable.Rows(0)("CURRENCY_CODE_1").ToString) <> String.Empty Then
                currencycode = currenciesTable.Rows(0)("CURRENCY_CODE_1").ToString
            End If

            Dim currencyexponent As String = "2" 'No of decimal places in currency. Retrieve as per Retail Logic
            Dim transactionamount As String = CInt((Profile.Basket.BasketSummary.TotalBasket) * 100).ToString 'Transaction amount in pence
            Dim transactiondisplayamount As String = Profile.Basket.BasketSummary.TotalBasket 'Amount to display to customer

            Dim visaMerchantBankId As String = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentDetails4
            Dim visaMerchantNumber As String = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentDetails5
            Dim visaMerchantPassword As String = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentDetails6
            Dim mcmMerchantBankId As String = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentDetails7
            Dim mcmMerchantNumber As String = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentDetails8
            Dim mcmMerchantPassword As String = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentDetails9

            Dim dd As String = """"
            Dim MsgData As String
            MsgData = _
            "<?xml version=""1.0"" encoding=""utf-8""?>" & _
            "<payerauthenrollmentcheckrequest xmlns:xsi=" & dd & "http://www.w3.org/2001/XMLSchema-instance" & dd & " xmlns=" & dd & "PAYERAUTH" & dd & ">" & _
                "<mkaccountid>" & mkaccountid & "</mkaccountid>" & _
                "<mkacquirerid>" & mkacquirerid & "</mkacquirerid>" & _
                "<merchantname>" & merchantname & "</merchantname>" & _
                "<merchantcountrycode>" & merchantcountrycode & "</merchantcountrycode>" & _
                "<merchanturl>" & merchanturl & "</merchanturl>" & _
                 "<visamerchantbankid>" & visaMerchantBankId & "</visamerchantbankid>" & _
                    "<visamerchantnumber>" & visaMerchantNumber & "</visamerchantnumber>" & _
                    "<visamerchantpassword>" & visaMerchantPassword & "</visamerchantpassword>" & _
                    "<mcmmerchantbankid>" & mcmMerchantBankId & "</mcmmerchantbankid>" & _
                    "<mcmmerchantnumber>" & mcmMerchantNumber & "</mcmmerchantnumber>" & _
                    "<mcmmerchantpassword>" & mcmMerchantPassword & "</mcmmerchantpassword>" & _
                "<cardnumber>" & cardnumber & "</cardnumber>" & _
                "<cardexpyear>" & cardexpyear & "</cardexpyear>" & _
                "<cardexpmonth>" & cardexpmonth & "</cardexpmonth>" & _
                "<currencycode>" & currencycode & "</currencycode>" & _
                "<currencyexponent>" & currencyexponent & "</currencyexponent>" & _
                "<transactionamount>" & transactionamount & "</transactionamount>" & _
                "<transactiondisplayamount>" & transactiondisplayamount & "</transactiondisplayamount>" & _
            "</payerauthenrollmentcheckrequest>"

            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load", "Customer: " & Profile.User.Details.LoginID & ", MsgData: " & MsgData, "3dSecureLogging")

            Dim ClientHeader As com.commidea.testweb.ClientHeader = New com.commidea.testweb.ClientHeader()

            ClientHeader.CDATAWrapping = True
            ClientHeader.SystemGUID = ourSystemGUID
            ClientHeader.SystemID = ourSystemId
            ClientHeader.Passcode = ourPasscode
            ClientHeader.SendAttempt = 0
            ClientHeader.CDATAWrapping = False 'Sets whether the respose will be wrapped in CDATA tags

            Dim Message As com.commidea.testweb.Message = New com.commidea.testweb.Message
            Message.MsgData = MsgData
            Message.MsgType = "PAI"

            Message.ClientHeader = ClientHeader

            Dim CommideaGateway As com.commidea.testweb.CommideaGateway = New com.commidea.testweb.CommideaGateway
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
            CommideaGateway.Url = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentUrl1
            Dim ProcessMsg As com.commidea.testweb.Message = CommideaGateway.ProcessMsg(Message)

            Dim Resp As String = ProcessMsg.MsgData.ToString()

            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load", "Customer: " & Profile.User.Details.LoginID & ", Response: " & Resp, "3dSecureLogging")

            'Extract Values from the Response
            Session("MerchantReference") = getFromXML("merchantreference", Resp)
            Dim RespProcessingDB As String = getFromXML("processingdb", Resp)
            Session("ProcessingDB") = getFromXML("processingdb", Resp)
            Session("PayerAuthRequestId") = getFromXML("payerauthrequestid", Resp)
            Session("Enrolled") = getFromXML("enrolled", Resp)
            Session("AcsUrl") = getFromXML("acsurl", Resp)
            Session("PAReq") = getFromXML("pareq", Resp)
            Dim RespErrorCode As String = getFromXML("errorcode", Resp)
            Dim RespErrorDescription As String = getFromXML("errordescription", Resp)

            'Set Return URL
            Dim currentUrl As String = Request.Url.ToString
            Session("TermUrl") = currentUrl.Substring(0, currentUrl.IndexOf("PagesLogin")) & _
                        "Redirect/PaymentGateway.aspx?page=checkout3dSecure.aspx&function=checkout3dSecure"

            Session("MD") = Profile.Basket.Temp_Order_Id 'Temp Order Id

            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load", "Customer: " & Profile.User.Details.LoginID & ", Redirect URL: " & Session("TermUrl"), "3dSecureLogging")

            If Session("Enrolled") = "Y" Then
                iframe3dSecure.Attributes("src") = "Checkout3dSecureIFrameCommidea.aspx"
            Else
                Response.Redirect(Session("TermUrl"))
            End If
        Catch ex As Exception
            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load - Commidea Try-Catch", "Customer: " & Profile.User.Details.LoginID & ", Exception: " & ex.Message, "3dSecureLogging")
        End Try
    End Sub

    ''' <summary>
    ''' Process 3D Secure using Winbank as the provider
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub processWinbank3DSecure()
        Dim ccRequest As New CentinelRequest()
        Dim ccResponse As New CentinelResponse()
        Dim priceListHeaderTableAdapter As New TalentBasketDatasetTableAdapters.tbl_price_list_headerTableAdapter
        Dim priceListHeaderTable As Data.DataTable = priceListHeaderTableAdapter.GetPriceListHeaderByPriceList(ModuleDefaults.PriceList)
        Dim currenciesTableAdapter As New TalentApplicationVariablesTableAdapters.tbl_currencyTableAdapter
        Dim currenciesTable As Data.DataTable = currenciesTableAdapter.GetDataByCurrencyCode(priceListHeaderTable.Rows(0)("CURRENCY_CODE").ToString)

        ' Get the currency code
        Dim currencycode As String = "826" 'Retrieve as per Retail Logic
        If currenciesTable.Rows.Count > 0 AndAlso TECUtilities.CheckForDBNull_String(currenciesTable.Rows(0)("CURRENCY_CODE_1").ToString) <> String.Empty Then
            currencycode = currenciesTable.Rows(0)("CURRENCY_CODE_1").ToString
        End If

        ' Get the transaction amount
        Dim currencyexponent As String = "2" 'No of decimal places in currency. Retrieve as per Retail Logic
        Dim transactionamount As String = CInt((Profile.Basket.BasketSummary.TotalBasket) * 100).ToString 'Transaction amount in pence
        Dim transactiondisplayamount As String = Profile.Basket.BasketSummary.TotalBasket 'Amount to display to customer

        ' Get card number and expiry date
        Dim cardnumber As String = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)
        Dim expDate As String = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)
        If expDate.Length = 5 Then
            expDate = "0" & expDate
        ElseIf expDate.Length = 4 Then
            expDate = expDate
        Else
            expDate = expDate.Substring(4, 2) & expDate.Substring(0, 2)
        End If
        expDate = expDate.Substring(4, 2) & expDate.Substring(0, 2)
        Dim cardexpyear As String = "20" + expDate.Substring(0, 2) 'Card expiry year (YY)
        Dim cardexpmonth As String = expDate.Substring(2, 2) 'Card expiry month (MM)

        Dim cardHolderName As String = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)
        ' Generate order number via Talent Common
        Dim talentTransactionNo As String = String.Empty
        Dim payment As New Talent.Common.TalentPayment
        Dim returnErrorCode As String = String.Empty
        Dim returnError As Boolean = False
        Dim dePayment As New Talent.Common.DEPayments
        With dePayment
            .Source = "W"
            .GenerateTransactionFileID = "WB304"
        End With
        payment.De = dePayment
        payment.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        payment.Settings.BusinessUnit = TalentCache.GetBusinessUnit
        payment.Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup
        payment.Settings.Cacheing = False
        payment.Settings.CacheTimeMinutes = 0

        Dim err As New Talent.Common.ErrorObj
        err = payment.GenerateTransactionID
        If Not err.HasError Then
            Try
                Dim dt As Data.DataTable
                dt = payment.ResultDataSet.Tables(0)
                If dt.Rows(0)("ErrorOccurred").ToString = GlobalConstants.ERRORFLAG Then
                    err.HasError = True
                    err.ErrorMessage = dt.Rows(0)("ReturnCode")
                Else
                    talentTransactionNo = dt.Rows(0)("GeneratedID")
                End If

            Catch ex As Exception
                err.HasError = True
            End Try
        End If

        '--------------------------------------
        ' Enrolment Check (WebService Call)
        '--------------------------------------
        ccRequest.add("MsgType", "cmpi_lookup")
        ccRequest.add("Version", "1.7")
        ccRequest.add("ProcessorId", ModuleDefaults.Payment3DSecureDetails1)
        ccRequest.add("MerchantId", ModuleDefaults.Payment3DSecureDetails2)
        ccRequest.add("TransactionPwd", ModuleDefaults.Payment3DSecureDetails3)
        ccRequest.add("TransactionType", "C")
        ccRequest.add("Amount", transactionamount.ToString)
        ccRequest.add("CurrencyCode", currencycode)
        '  ccRequest.add("CurrencyCode", "978") - Eur
        ccRequest.add("CardNumber", cardnumber)
        ccRequest.add("CardExpMonth", cardexpmonth)
        ccRequest.add("CardExpYear", cardexpyear)
        'ccRequest.add("OrderNumber", "0000000000100")
        ccRequest.add("OrderNumber", talentTransactionNo)
        '  ccRequest.add("OrderDesc", "TalentEBusinessOrder")
        ccRequest.add("OrderDesc", cardHolderName)

        Dim errorNo As String = String.Empty
        Dim errorDesc As String = String.Empty
        Dim enrolled As String = String.Empty
        Dim payload As String = String.Empty
        Dim acsurl As String = String.Empty
        Dim transactionId As String = String.Empty
        Dim eciFlag As String = String.Empty

        ' https://centineltest.cardinalcommerce.com/maps/txns.asp = Test
        ' https://centinel.piraeusbank.fdsecure.com/maps/txns.asp = Live
        Try
            ccResponse = ccRequest.sendHTTP(ModuleDefaults.Payment3DSecureURL1, 10000)
            errorNo = ccResponse.getValue("ErrorNo")
            errorDesc = ccResponse.getValue("ErrorDesc")
            enrolled = ccResponse.getValue("Enrolled")
            payload = ccResponse.getValue("Payload")
            acsurl = ccResponse.getValue("ACSUrl")
            transactionId = ccResponse.getValue("TransactionId")
            eciFlag = ccResponse.getValue("EciFlag")

            ' Log 'U' errors
            If enrolled = "U" Then
                TECUtilities.TalentLogging.ExceptionLog("3dSecure - cmpi_lookup: " & talentTransactionNo & _
                                                                " Basket: " & Profile.Basket.Basket_Header_ID.ToString, "Enrolled=U " & _
                                                                 errorNo & " " & errorDesc)
            End If

        Catch ex As Exception
            ' Carry on with transaction but log the eror
            enrolled = "TIMEOUT"
            eciFlag = ccResponse.getValue("EciFlag")
            TECUtilities.TalentLogging.ExceptionLog("3dSecure - cmpi_lookup: " & talentTransactionNo & _
                                        " Basket: " & Profile.Basket.Basket_Header_ID.ToString, ex.Message)
        End Try



        'Extract Values from the Response
        Session("MerchantReference") = talentTransactionNo
        Session("PayerAuthRequestId") = transactionId
        Session("Enrolled") = enrolled
        Session("AcsUrl") = acsurl
        Session("PAReq") = payload

        'Set Return URL
        Dim currentUrl As String = Request.Url.ToString
        Session("TermUrl") = currentUrl.Substring(0, currentUrl.IndexOf("PagesLogin")) & _
                    "Redirect/PaymentGateway.aspx?page=checkout3dSecure.aspx&function=checkout3dSecure"

        Session("MD") = Profile.Basket.Temp_Order_Id 'Temp Order Id

        If Session("Enrolled") = "Y" Then
            iframe3dSecure.Attributes("src") = "Checkout3dSecureIFrameWinbank.aspx"
        Else
            Session("WinbankECI") = eciFlag
            Response.Redirect(Session("TermUrl"))
        End If
    End Sub

    ''' <summary>
    ''' Process 3D Secure using eCentric as the provider
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub processECentric3DSecure()
        Const ECENTRIC_PAYMENT_LOG As String = "ECentricPaymentLog"
        Dim redirectToTakePayment As Boolean = False
        Dim redirectBackToCheckout As Boolean = False
        Dim client As PaymentGatewayServiceClient = New PaymentGatewayServiceClient()
        Dim transactionId As String = Now.Ticks.ToString()
        Dim customerDetails As String = "Customer: " & Profile.UserName
        Dim basketDetails As String = "Basket: " & Profile.Basket.Basket_Header_ID & " Value: " & Profile.Basket.BasketSummary.TotalBasket() & " Transaction ID: " & transactionId
        Dim responseDetails As New ResponseDetail
        Dim acsUrl As String = String.Empty
        Dim currentUrl As String = Talent.eCommerce.Utilities.FormatSSLOffloadedURL()
        Session("MD") = Nothing
        Session("PARes") = Nothing
        Session("AcsUrl") = Nothing
        Session("3DSecureTransactionId") = Nothing
        Session("TermUrl") = currentUrl.Substring(0, currentUrl.IndexOf("PagesLogin")) & "Redirect/PaymentGateway.aspx?page=checkout3dSecure.aspx&function=checkout3dSecure"
        _talentLogging.GeneralLog(customerDetails, basketDetails, "Start 3D Secure process", ECENTRIC_PAYMENT_LOG)
        Try
            Dim merchantId As String = ModuleDefaults.PaymentDetails1
            Dim transactionDateTime As Date = Now
            Dim amount As Long = Profile.Basket.BasketSummary.TotalBasket().ToString("F2").Replace(".", String.Empty)
            Dim priceListHeaderTableAdapter As New TalentBasketDatasetTableAdapters.tbl_price_list_headerTableAdapter
            Dim priceListHeaderTable As Data.DataTable = priceListHeaderTableAdapter.GetPriceListHeaderByPriceList(ModuleDefaults.PriceList)
            Dim currenciesTableAdapter As New TalentApplicationVariablesTableAdapters.tbl_currencyTableAdapter
            Dim currenciesTable As Data.DataTable = currenciesTableAdapter.GetDataByCurrencyCode(priceListHeaderTable.Rows(0)("CURRENCY_CODE").ToString)
            Dim currencyCode As String = String.Empty
            If currenciesTable.Rows.Count > 0 AndAlso TECUtilities.CheckForDBNull_String(currenciesTable.Rows(0)("CURRENCY_CODE").ToString) <> String.Empty Then
                currencyCode = currenciesTable.Rows(0)("CURRENCY_CODE").ToString
            End If
            Dim orderNumber As String = Profile.Basket.Basket_Header_ID

            Dim card As New BankCard
            card.CardAssociation = Checkout.RetrievePaymentItemFromSession("CardType", GlobalConstants.CHECKOUTASPXSTAGE)
            card.CardholderName = Checkout.RetrievePaymentItemFromSession("CardHolderName", GlobalConstants.CHECKOUTASPXSTAGE)
            card.CardNumber = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)
            card.ExpiryMonth = Checkout.RetrievePaymentItemFromSession("ExpiryDate", GlobalConstants.CHECKOUTASPXSTAGE).Substring(0, 2)
            card.ExpiryYear = Checkout.RetrievePaymentItemFromSession("ExpiryDate", GlobalConstants.CHECKOUTASPXSTAGE).Substring(2, 2)
            card.SecurityCode = Checkout.RetrievePaymentItemFromSession("CV2Number", GlobalConstants.CHECKOUTASPXSTAGE)
            card.ExpiryYearSpecified = True
            card.ExpiryMonthSpecified = True
            card.CardTypeSpecified = True

            Dim msgHeader As New MessageHeader
            Dim reconId As String = Session.SessionID
            Dim transactionStatus As TransactionStatusType
            Dim enrolled As Boolean
            Dim pAResPayload As String = String.Empty

            _talentLogging.GeneralLog(customerDetails, basketDetails, "Attempt 3D Secure enrollment check", ECENTRIC_PAYMENT_LOG)
            transactionStatus = client.Secure3DLookup(msgHeader, merchantId, transactionId, transactionDateTime, reconId, amount, currencyCode, orderNumber, _
                                                      card, responseDetails, enrolled, acsUrl, pAResPayload)

            If responseDetails.Code = GlobalConstants.ECENTRICSUCCESSCODE1 OrElse responseDetails.Code = GlobalConstants.ECENTRICSUCCESSCODE2 Then
                Session("3DSecureTransactionId") = transactionId 'always set this even if the card isn't enrolled
                If enrolled Then
                    'Go to the IFrame page
                    _talentLogging.GeneralLog(customerDetails, basketDetails, "Card enrolled for 3D secure", ECENTRIC_PAYMENT_LOG)
                    Session("MD") = Profile.Basket.Basket_Header_ID
                    Session("PARes") = pAResPayload
                    Session("AcsUrl") = acsUrl
                    iframe3dSecure.Attributes("src") = "Checkout3dSecureIFrameECentric.aspx"
                Else
                    'Go straight to taking payment (skipping authentication)
                    _talentLogging.GeneralLog(customerDetails, basketDetails, "Card not enrolled for 3D secure", ECENTRIC_PAYMENT_LOG)
                    redirectToTakePayment = True
                End If
            Else
                'need to know under what codes can be redirected to 3D secure/take payment/back to checkout
                With responseDetails
                    _talentLogging.GeneralLog(customerDetails, basketDetails, "Error doing an enrollment check for 3D secure (1): Code:" & .Code & " Description:" & .Description _
                                             & " Client Message:" & .ClientMessage & " Source:" & .Source, ECENTRIC_PAYMENT_LOG)
                End With
            End If
        Catch ex As Exception
            'Redirect back to the checkout page as a serious issue has happened
            If responseDetails IsNot Nothing AndAlso responseDetails.Code IsNot Nothing AndAlso responseDetails.Code.Length > 0 Then
                With responseDetails
                    _talentLogging.GeneralLog(customerDetails, basketDetails, _
                                "Error doing an enrollment check for 3D secure (2): Code:" & .Code & " Description:" & .Description & " Client Message:" & .ClientMessage & " Source:" & .Source & _
                                "Exception Details:" & ex.Message & " Exception Stack Trace:" & ex.StackTrace, ECENTRIC_PAYMENT_LOG)
                End With
            Else
                _talentLogging.GeneralLog(customerDetails, basketDetails, "Error doing an enrollment check for 3D secure (3): No response object to look at. Exception Details:" & ex.Message & " Exception Stack Trace:" & ex.StackTrace, ECENTRIC_PAYMENT_LOG)
            End If
            HttpContext.Current.Session("TicketingGatewayError") = GlobalConstants.ECENTRICGENERICERROR
            redirectBackToCheckout = True
        Finally
            client.Close()
        End Try

        If redirectToTakePayment Then
            Response.Redirect("~/Redirect/TicketingGateway.aspx?page=checkoutPaymentDetails.aspx&function=Payment")
        Else
            If redirectBackToCheckout Then
                Response.Redirect("~/PagesLogin/Checkout/Checkout.aspx")
            End If
        End If
    End Sub

    Private Sub processVanguard3DSecure()
        Try
            Dim basketPaymentID As Long = 0
            Dim vgAttributes As DEVanguard = Nothing
            Dim basketPayEntity As DEBasketPayment = Nothing
            If Talent.eCommerce.Utilities.TryGetVGAttributesSession(basketPaymentID, vgAttributes, basketPayEntity) Then
                '_talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load", "Customer: " & Profile.User.Details.LoginID & ", Response: " & Resp, "3dSecureLogging")



                'Dim RespProcessingDB As String = getFromXML("processingdb", Resp)

                'Session("ProcessingDB") = getFromXML("processingdb", Resp)
                Session("MerchantReference") = basketPaymentID
                Session("PayerAuthRequestId") = vgAttributes.PayerAuth.PayerAuthRequestID
                Session("Enrolled") = vgAttributes.PayerAuth.IsEnrolled
                Session("AcsUrl") = vgAttributes.PayerAuth.AcsUrl
                Session("PAReq") = vgAttributes.PayerAuth.PareQ

                'Dim RespErrorCode As String = getFromXML("errorcode", Resp)
                'Dim RespErrorDescription As String = getFromXML("errordescription", Resp)

                ''Set Return URL
                Dim currentUrl As String = Request.Url.ToString
                Session("TermUrl") = currentUrl.Substring(0, currentUrl.IndexOf("PagesLogin")) & _
                            "Redirect/VanguardGateway.aspx?page=checkout3dSecure.aspx&function=checkout3dSecure"

                Session("MD") = Profile.Basket.Temp_Order_Id 'Temp Order Id

                _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load", "Customer: " & Profile.User.Details.LoginID & ", Redirect URL: " & Session("TermUrl"), "3dSecureLogging")
                Session("VG_FROM_3DSECURE") = True
                If Convert.ToBoolean(Session("Enrolled")) Then
                    iframe3dSecure.Attributes("src") = "Checkout3dSecureIFrameVanguard.aspx"
                Else
                    Response.Redirect(Session("TermUrl"))
                End If
            End If


        Catch ex As Exception
            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load - Vanguard Try-Catch", "Customer: " & Profile.User.Details.LoginID & ", Exception: " & ex.Message, "3dSecureLogging")
        End Try
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Get a value from an XML string
    ''' </summary>
    ''' <param name="key">The xml node</param>
    ''' <param name="x">The xml document</param>
    ''' <returns>The inner xml value of the xml node</returns>
    ''' <remarks></remarks>
    Private Function getFromXML(ByVal key As String, ByVal x As String) As String
        _talentLogging.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "getFromXML", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")

        Dim rtn As String = String.Empty
        If x <> "" Then
            Dim xmlDoc As New XmlDocument
            Dim Nodes As XmlNodeList
            Dim Node As XmlNode

            xmlDoc.LoadXml(x)
            Nodes = xmlDoc.GetElementsByTagName(key)
            For Each Node In Nodes
                If Node.InnerXml <> "" Then
                    rtn = Node.InnerXml
                End If
            Next
        End If
        Return rtn
    End Function

#End Region

End Class