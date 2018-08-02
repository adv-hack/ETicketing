Imports System.Xml
Imports System.Xml.Serialization
Imports System.Security.Cryptography
Imports Talent.eCommerce
Imports cardinalCommerce

Partial Class Redirect_PaymentGateway
    Inherits Base01

#Region "Class Level Fields"

    Private _modDefs As New Talent.eCommerce.ECommerceModuleDefaults
    Private _myDefs As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Private _wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _javaScriptRedirect As Boolean = False
    Private _talentLogging As New Talent.Common.TalentLogging

#End Region

#Region "Retail Logic Form Variables"
    Dim PAResSyntaxOK As String = String.Empty
    Dim PAResVerified As String = String.Empty
    Dim httpPostVersion As String = String.Empty
    Dim OpalMerchantID As String = String.Empty
    Dim XID As String = String.Empty
    Dim mdStatus As String = String.Empty
    Dim mdErrorMsg As String = String.Empty
    Dim txstatus As String = String.Empty
    Dim iReqCode As String = String.Empty
    Dim iReqDetail As String = String.Empty
    Dim vendorCode As String = String.Empty
    Dim eci As String = String.Empty
    Dim cavv As String = String.Empty
    Dim cavvAlgorithm As String = String.Empty
    Dim MD As String = String.Empty
    Dim DigestFromRetailLogic As String = String.Empty
    Dim sID As String = String.Empty
    Dim opalErrorCode As String = String.Empty
    Dim authenticationStatus As String = String.Empty
    Dim atsData As String = String.Empty
    Dim talent3dSecureTransactionID As String = String.Empty
    Dim enrolled As String = String.Empty
    Dim ParesStatus As String = String.Empty
    Dim SignatureVerification As String = String.Empty
#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PaymentGateway.aspx"
        End With
        _myDefs = _modDefs.GetDefaults
        Dim referrerUrl As String = String.Empty
        Try
            referrerUrl = Request.UrlReferrer.AbsoluteUri
        Catch ex As Exception
            referrerUrl = ex.Message
        End Try
        _talentLogging.FrontEndConnectionString = _wfr.FrontEndConnectionString
        _talentLogging.GeneralLog(_wfr.PageCode, "Page_Init", "Customer: " & Profile.User.Details.LoginID & ", Referrer URL: " & referrerUrl, "3dSecureLogging")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _talentLogging.FrontEndConnectionString = _wfr.FrontEndConnectionString
        _talentLogging.GeneralLog(_wfr.PageCode, "Page_Load", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")
        If Not String.IsNullOrEmpty(Request("page")) AndAlso Not String.IsNullOrEmpty(Request("function")) Then
            Dim page As String = Request("page"), _
                func As String = Request("function")

            Select Case LCase(Request("page"))
                Case Is = LCase("checkout3dSecure.aspx")
                    Select Case LCase(func)
                        Case Is = LCase("checkout3dSecure") : checkout3dSecure()
                    End Select
            End Select
        Else
            Response.Redirect(Request.Url.ToString.Substring(0, Request.Url.ToString.IndexOf("Redirect")))
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub checkout3dSecure()
        _talentLogging.FrontEndConnectionString = _wfr.FrontEndConnectionString
        _talentLogging.GeneralLog(_wfr.PageCode, "checkout3dSecure", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")

        Dim cardAuthenticated As Boolean = True
        Dim returnUrl As String = String.Empty
        Dim currentUrl As String = Utilities.FormatSSLOffloadedURL()

        If _myDefs.PaymentProcess3dSecure Then
            Select Case _myDefs.Payment3DSecureProvider
                Case Is = "RETAILLOGIC" : cardAuthenticated = check3dSecureRetailLogic()
                Case Is = "COMMIDEA" : cardAuthenticated = check3dSecureCommidea()
                Case Is = "COMMIDEA-MULTI-ACCOUNT-ID" : cardAuthenticated = check3dSecureCommideaMultiAccountId()
                Case Is = "WINBANK" : cardAuthenticated = check3dSecureWinbank()
                Case Is = GlobalConstants.ECENTRICGATEWAY : cardAuthenticated = check3DSecureECentric()
            End Select
        End If

        'Was the 3d secure request successful
        If cardAuthenticated Then

            'Store the 3d secure values
            Checkout.Store3dDetails(eci, cavv, XID, authenticationStatus, sID, atsData, talent3dSecureTransactionID, enrolled, ParesStatus, SignatureVerification)

            
            Select Case Talent.eCommerce.Utilities.BasketContentTypeWithOverride
                Case "T", "C"
                    returnUrl = currentUrl.Substring(0, currentUrl.IndexOf("Redirect")) & "Redirect/TicketingGateway.aspx?page=checkoutPaymentDetails.aspx&function=Payment"
                Case Else
                    returnUrl = currentUrl.Substring(0, currentUrl.IndexOf("Redirect")) & "Redirect/PaymentGateway.aspx?page=checkoutPaymentDetails.aspx&function=Payment"
            End Select
        Else
            ' Card Authentication Issue, send to payment details page with Card Auth Error (mdStatus) - picked up from check3dSecureRetailLogic
            returnUrl = currentUrl.Substring(0, currentUrl.IndexOf("Redirect")) & "PagesLogin/Checkout/checkout.aspx"
        End If

        _talentLogging.GeneralLog(_wfr.PageCode, "checkout3dSecure", "Customer: " & Profile.User.Details.LoginID & ", Redirect To: " & returnUrl, "3dSecureLogging")

        'Do we need to process the redirect via javascript to jump out of the iframe
        If _javaScriptRedirect Then
            'Output the waiting meassage
            plhFormToPostToPaymentRoutines.Visible = True
            ltlPleaseWait.Text = _wfr.Content("PleaseWaitText", _languageCode, True)
            imgWaitingIcon.ImageUrl = ImagePath.getImagePath("APPTHEME", _wfr.Attribute("WaitingImagePath"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
            Dim jsScript As String = "<script language=JavaScript>"
            jsScript += " top.location = '" & returnUrl & "';"
            jsScript += "</script>"
            Page.ClientScript.RegisterStartupScript(GetType(String), "redirectScript", jsScript)
        Else
            'Use standard method
            Response.Redirect(returnUrl)
        End If
    End Sub

    Private Function check3dSecureCommidea() As Boolean

        _talentLogging.FrontEndConnectionString = _wfr.FrontEndConnectionString
        _talentLogging.GeneralLog(_wfr.PageCode, "check3dSecureCommidea", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")

        Dim transactionOk As Boolean = False

        Dim ourAccountId As String = _myDefs.Payment3dSecureOurAccountId
        Dim ourSystemGUID As String = _myDefs.Payment3dSecureOurSystemGUID
        Dim ourSystemId As String = _myDefs.Payment3dSecureOurSystemId
        Dim ourPasscode As String = _myDefs.Payment3dSecureOurPasscode
        Dim Resp As String = String.Empty

        If Session("Enrolled") = "N" Or Session("Enrolled") = "" Then
            'Call Authentication Check Web Service passing the following Params:
            'PayerAuthRequestID
            'Enrolled = "N"

            Dim ClientHeader As com.commidea.testweb.ClientHeader = New com.commidea.testweb.ClientHeader()

            ClientHeader.CDATAWrapping = True
            ClientHeader.SystemGUID = ourSystemGUID
            ClientHeader.SystemID = ourSystemId
            ClientHeader.Passcode = ourPasscode
            ClientHeader.SendAttempt = 0
            ClientHeader.CDATAWrapping = False 'Sets whether the respose will be wrapped in CDATA tags
            ClientHeader.ProcessingDB = Session("ProcessingDB")

            Dim dd As String = """"
            Dim MsgData As String = _
            "<?xml version=""1.0"" encoding=""utf-8""?>" & _
            "<payerauthauthenticationcheckrequest xmlns:xsi=" & dd & "http://www.w3.org/2001/XMLSchema-instance" & dd & " xmlns=" & dd & "PAYERAUTH" & dd & ">" & _
            "		<merchantreference></merchantreference>" & _
            "		<payerauthrequestid>" & Session("PayerAuthRequestId") & "</payerauthrequestid>" & _
            "		<pares></pares>" & _
            "		<enrolled>N</enrolled>" & _
            "</payerauthauthenticationcheckrequest>"

            _talentLogging.GeneralLog(_wfr.PageCode, "check3dSecureCommidea", "Customer: " & Profile.User.Details.LoginID & ", MsgData: " & MsgData, "3dSecureLogging")

            Dim Message As com.commidea.testweb.Message = New com.commidea.testweb.Message
            Message.MsgData = MsgData
            Message.MsgType = "PAI"

            Message.ClientHeader = ClientHeader

            Dim CommideaGateway As com.commidea.testweb.CommideaGateway = New com.commidea.testweb.CommideaGateway
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
            CommideaGateway.Url = _myDefs.Payment3DSecureURL1
            Dim ProcessMsg As com.commidea.testweb.Message = CommideaGateway.ProcessMsg(Message)

            Resp = ProcessMsg.MsgData.ToString()

            _talentLogging.GeneralLog(_wfr.PageCode, "check3dSecureCommidea", "Customer: " & Profile.User.Details.LoginID & ", Response: " & Resp, "3dSecureLogging")

        Else
            Session("Enrolled") = "Y"
            _javaScriptRedirect = True
            'Call Authentication Check Web Service Passing the following Params:
            'Session("PaRes")
            'merchantreference
            'payerauthrequestid
            'enrolled = "Y"

            Dim ClientHeader As com.commidea.testweb.ClientHeader = New com.commidea.testweb.ClientHeader()

            ClientHeader.CDATAWrapping = True
            ClientHeader.SystemGUID = ourSystemGUID
            ClientHeader.SystemID = ourSystemId
            ClientHeader.Passcode = ourPasscode
            ClientHeader.SendAttempt = 0
            ClientHeader.CDATAWrapping = False 'Sets whether the respose will be wrapped in CDATA tags
            ClientHeader.ProcessingDB = Session("ProcessingDB")

            Dim dd As String = """"
            Dim MsgData As String = _
            "<?xml version=""1.0"" encoding=""utf-8""?>" & _
            "<payerauthauthenticationcheckrequest xmlns:xsi=" & dd & "http://www.w3.org/2001/XMLSchema-instance" & dd & " xmlns=" & dd & "PAYERAUTH" & dd & ">" & _
            "<merchantreference>" & Session("MerchantReference") & "</merchantreference>" & _
            "<payerauthrequestid>" & Session("PayerAuthRequestId") & "</payerauthrequestid>"
            If Not Request.Form.Item("PaRes") Is Nothing Then MsgData += "<pares>" & Request.Form.Item("PaRes").ToString & "</pares>"
            If Not Request.Form.Item("PaReq") Is Nothing Then MsgData += "<pareq>" & Request.Form.Item("PaReq").ToString & "</pareq>"
            MsgData += "<enrolled>Y</enrolled>" & _
            "</payerauthauthenticationcheckrequest>"

            _talentLogging.GeneralLog(_wfr.PageCode, "check3dSecureCommidea", "Customer: " & Profile.User.Details.LoginID & ", MsgData: " & MsgData, "3dSecureLogging")

            Dim Message As com.commidea.testweb.Message = New com.commidea.testweb.Message
            Message.MsgData = MsgData
            Message.MsgType = "PAI"

            Message.ClientHeader = ClientHeader

            Dim CommideaGateway As com.commidea.testweb.CommideaGateway = New com.commidea.testweb.CommideaGateway
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
            CommideaGateway.Url = _myDefs.Payment3DSecureURL1

            Dim ProcessMsg As com.commidea.testweb.Message = CommideaGateway.ProcessMsg(Message)
            Resp = ProcessMsg.MsgData.ToString()

            _talentLogging.GeneralLog(_wfr.PageCode, "check3dSecureCommidea", "Customer: " & Profile.User.Details.LoginID & ", Response: " & Resp, "3dSecureLogging")

        End If

        Dim merchantreference As String = getFromXML("merchantreference", Resp)
        Dim processingdb As String = getFromXML("processingdb", Resp)
        Dim payerauthrequestid As String = getFromXML("payerauthrequestid", Resp)
        atsData = getFromXML("atsdata", Resp)
        authenticationStatus = Trim(getFromXML("authenticationstatus", Resp)).PadRight(2, " ")
        Dim authenticationcertificate As String = getFromXML("authenticationcertificate", Resp)
        cavv = getFromXML("authenticationcavv", Resp)
        eci = getFromXML("authenticationeci", Resp)
        Dim authenticationtime As String = getFromXML("authenticationtime", Resp)
        Dim errorcode As String = getFromXML("errorcode", Resp)
        XID = Session("PayerAuthRequestId")

        '***********************************************
        '   JDW - Edit - 14/04/2011 - START
        '***********************************************
        'If Enrolled statement added due to bug found during testing
        '------------------------------------------------------------

        If Session("Enrolled") = "N" Then
            transactionOk = True
        Else
            'Check if authentication was successfull
            For Each sc As String In _myDefs.Payment3DSecureSuccessCodes
                If sc = authenticationStatus.Trim Then
                    transactionOk = True
                End If
            Next
        End If

        '-----------------------------------------------
        'Old code
        '-----------------------------------------------
        ' '' ''Check if authentication was successfull
        '' ''For Each sc As String In myDefs.Payment3DSecureSuccessCodes
        '' ''    If sc = authenticationStatus.Trim Then
        '' ''        transactionOk = True
        '' ''    End If
        '' ''Next
        '-----------------------------------------------

        '***********************************************
        '   JDW - Edit - 14/04/2011 - END
        '***********************************************

        If transactionOk = False Then
            'Failed, Redirect User to Checkout.aspx with suitable Error Message
            Session("TicketingGatewayError") = "3DSecureMDStatus0" 'authenticationstatus = "F"
        End If

        Return transactionOk
    End Function

    Private Function check3dSecureCommideaMultiAccountId() As Boolean
        _talentLogging.FrontEndConnectionString = _wfr.FrontEndConnectionString
        _talentLogging.GeneralLog(_wfr.PageCode, "check3dSecureCommideaMultiAccountId", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")
        Dim tDataObjects As New Talent.Common.TalentDataObjects
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = _wfr.FrontEndConnectionString
        settings.DestinationDatabase = "SQL2005"
        settings.BusinessUnit = _wfr.BusinessUnit
        settings.Partner = _wfr.PartnerCode
        tDataObjects.Settings = settings
        tDataObjects.PaymentSettings.TblPaymentDefaults.GetPaymentDefaults(Profile.Basket.PAYMENT_ACCOUNT_ID)
        Dim ourAccountId As String = tDataObjects.PaymentSettings.TblPaymentDefaults.OurAccountID
        Dim ourSystemGUID As String = tDataObjects.PaymentSettings.TblPaymentDefaults.OurSystemGUID
        Dim ourSystemId As String = tDataObjects.PaymentSettings.TblPaymentDefaults.OurSystemID
        Dim ourPasscode As String = tDataObjects.PaymentSettings.TblPaymentDefaults.OurPasscode
        Dim Resp As String = String.Empty
        Dim transactionOk As Boolean = False

        If Session("Enrolled") = "N" Or Session("Enrolled") = "" Then
            Dim ClientHeader As com.commidea.testweb.ClientHeader = New com.commidea.testweb.ClientHeader()
            ClientHeader.CDATAWrapping = True
            ClientHeader.SystemGUID = ourSystemGUID
            ClientHeader.SystemID = ourSystemId
            ClientHeader.Passcode = ourPasscode
            ClientHeader.SendAttempt = 0
            ClientHeader.CDATAWrapping = False 'Sets whether the respose will be wrapped in CDATA tags
            ClientHeader.ProcessingDB = Session("ProcessingDB")

            Dim dd As String = """"
            Dim MsgData As String = _
            "<?xml version=""1.0"" encoding=""utf-8""?>" & _
            "<payerauthauthenticationcheckrequest xmlns:xsi=" & dd & "http://www.w3.org/2001/XMLSchema-instance" & dd & " xmlns=" & dd & "PAYERAUTH" & dd & ">" & _
            "		<merchantreference></merchantreference>" & _
            "		<payerauthrequestid>" & Session("PayerAuthRequestId") & "</payerauthrequestid>" & _
            "		<pares></pares>" & _
            "		<enrolled>N</enrolled>" & _
            "</payerauthauthenticationcheckrequest>"

            _talentLogging.GeneralLog(_wfr.PageCode, "check3dSecureCommideaMultiAccountId", "Customer: " & Profile.User.Details.LoginID & ", MsgData: " & MsgData, "3dSecureLogging")
            Dim Message As com.commidea.testweb.Message = New com.commidea.testweb.Message
            Message.MsgData = MsgData
            Message.MsgType = "PAI"
            Message.ClientHeader = ClientHeader
            Dim CommideaGateway As com.commidea.testweb.CommideaGateway = New com.commidea.testweb.CommideaGateway
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
            CommideaGateway.Url = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentUrl1
            Dim ProcessMsg As com.commidea.testweb.Message = CommideaGateway.ProcessMsg(Message)
            Resp = ProcessMsg.MsgData.ToString()
            _talentLogging.GeneralLog(_wfr.PageCode, "check3dSecureCommideaMultiAccountId", "Customer: " & Profile.User.Details.LoginID & ", Response: " & Resp, "3dSecureLogging")

        Else
            Session("Enrolled") = "Y"
            _javaScriptRedirect = True
            Dim ClientHeader As com.commidea.testweb.ClientHeader = New com.commidea.testweb.ClientHeader()
            ClientHeader.CDATAWrapping = True
            ClientHeader.SystemGUID = ourSystemGUID
            ClientHeader.SystemID = ourSystemId
            ClientHeader.Passcode = ourPasscode
            ClientHeader.SendAttempt = 0
            ClientHeader.CDATAWrapping = False 'Sets whether the respose will be wrapped in CDATA tags
            ClientHeader.ProcessingDB = Session("ProcessingDB")

            Dim dd As String = """"
            Dim MsgData As String = _
            "<?xml version=""1.0"" encoding=""utf-8""?>" & _
            "<payerauthauthenticationcheckrequest xmlns:xsi=" & dd & "http://www.w3.org/2001/XMLSchema-instance" & dd & " xmlns=" & dd & "PAYERAUTH" & dd & ">" & _
            "<merchantreference>" & Session("MerchantReference") & "</merchantreference>" & _
            "<payerauthrequestid>" & Session("PayerAuthRequestId") & "</payerauthrequestid>"
            If Not Request.Form.Item("PaRes") Is Nothing Then MsgData += "<pares>" & Request.Form.Item("PaRes").ToString & "</pares>"
            If Not Request.Form.Item("PaReq") Is Nothing Then MsgData += "<pareq>" & Request.Form.Item("PaReq").ToString & "</pareq>"
            MsgData += "<enrolled>Y</enrolled>" & _
            "</payerauthauthenticationcheckrequest>"

            _talentLogging.GeneralLog(_wfr.PageCode, "check3dSecureCommideaMultiAccountId", "Customer: " & Profile.User.Details.LoginID & ", MsgData: " & MsgData, "3dSecureLogging")
            Dim Message As com.commidea.testweb.Message = New com.commidea.testweb.Message
            Message.MsgData = MsgData
            Message.MsgType = "PAI"
            Message.ClientHeader = ClientHeader
            Dim CommideaGateway As com.commidea.testweb.CommideaGateway = New com.commidea.testweb.CommideaGateway
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12
            CommideaGateway.Url = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentUrl1
            Dim ProcessMsg As com.commidea.testweb.Message = CommideaGateway.ProcessMsg(Message)
            Resp = ProcessMsg.MsgData.ToString()
            _talentLogging.GeneralLog(_wfr.PageCode, "check3dSecureCommideaMultiAccountId", "Customer: " & Profile.User.Details.LoginID & ", Response: " & Resp, "3dSecureLogging")
        End If

        Dim merchantreference As String = getFromXML("merchantreference", Resp)
        Dim processingdb As String = getFromXML("processingdb", Resp)
        Dim payerauthrequestid As String = getFromXML("payerauthrequestid", Resp)
        Dim authenticationcertificate As String = getFromXML("authenticationcertificate", Resp)
        Dim authenticationtime As String = getFromXML("authenticationtime", Resp)
        Dim errorcode As String = getFromXML("errorcode", Resp)
        atsData = getFromXML("atsdata", Resp)
        authenticationStatus = Trim(getFromXML("authenticationstatus", Resp)).PadRight(2, " ")
        cavv = getFromXML("authenticationcavv", Resp)
        eci = getFromXML("authenticationeci", Resp)
        XID = Session("PayerAuthRequestId")

        If Session("Enrolled") = "N" Then
            transactionOk = True
        Else
            'Check if authentication was successfull
            For Each sc As String In tDataObjects.PaymentSettings.TblPaymentDefaults.SuccessCodes
                If sc = authenticationStatus.Trim Then
                    transactionOk = True
                End If
            Next
        End If
        If transactionOk = False Then
            'Failed, Redirect User to Checkout.aspx with suitable Error Message
            Session("TicketingGatewayError") = "3DSecureMDStatus0" 'authenticationstatus = "F"
        End If

        Return transactionOk
    End Function

    Private Function getFromXML(ByVal key As String, ByVal x As String) As String
        _talentLogging.FrontEndConnectionString = _wfr.FrontEndConnectionString
        _talentLogging.GeneralLog(_wfr.PageCode, "getFromXML", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")

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

    Private Function check3dSecureRetailLogic() As Boolean

        _talentLogging.FrontEndConnectionString = _wfr.FrontEndConnectionString
        _talentLogging.GeneralLog(_wfr.PageCode, "check3dSecureRetailLogic", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")

        Dim transactionOk As Boolean = False
        _javaScriptRedirect = True

        PAResSyntaxOK = Request.Params("PAResSyntaxOK")
        PAResVerified = Request.Params("PAResVerified")
        httpPostVersion = Request.Params("version")
        OpalMerchantID = Request.Params("merchantID")
        XID = Request.Params("XID")
        mdStatus = Request.Params("mdStatus")
        mdErrorMsg = Request.Params("mdErrorMsg")
        txstatus = Request.Params("txstatus")
        iReqCode = Request.Params("iReqCode")
        iReqDetail = Request.Params("iReqDetail")
        vendorCode = Request.Params("vendorCode")
        eci = Request.Params("eci")
        cavv = Request.Params("cavv")
        cavvAlgorithm = Request.Params("cavvAlgorithm")
        MD = Request.Params("MD")
        DigestFromRetailLogic = Request.Params("Digest")
        sID = Request.Params("sID")
        opalErrorCode = Request.Params("opalErrorCode")

        'If there are 2 values in the MD then take the first
        If MD.Contains(",") Then
            MD = MD.Substring(0, (MD.IndexOf(",") - 1))
        End If

        'check the mdStatus against the database success codes
        Dim mdStatusCode As Int32 = 0
        For Each mdStatusCode In _myDefs.Payment3DSecureSuccessCodes
            If mdStatus = mdStatusCode Then
                'Success/
                transactionOk = True
                Exit For
            End If
        Next

        If Not transactionOk Then
            'Transaction Failed
            Select Case mdStatus
                Case Is = 0
                    Session("TicketingGatewayError") = "3DSecureMDStatus0"
                Case Is = 1
                    Session("TicketingGatewayError") = "3DSecureMDStatus1"
                Case Is = 2
                    Session("TicketingGatewayError") = "3DSecureMDStatus2"
                Case Is = 3
                    Session("TicketingGatewayError") = "3DSecureMDStatus3"
                Case Is = 4
                    Session("TicketingGatewayError") = "3DSecureMDStatus4"
                Case Is = 5
                    Session("TicketingGatewayError") = "3DSecureMDStatus5"
                Case Is = 6
                    Session("TicketingGatewayError") = "3DSecureMDStatus6"
                Case Is = 7
                    Session("TicketingGatewayError") = "3DSecureMDStatus7"
                Case Is = 8
                    Session("TicketingGatewayError") = "3DSecureMDStatus8"
                Case Else
                    Session("TicketingGatewayError") = "3DSecureMDStatusNothing"
            End Select
        Else
            'Transaction Success - check the message digest
            Dim DigestFromSessionValues As New StringBuilder
            With DigestFromSessionValues
                .Append(httpPostVersion)
                .Append(OpalMerchantID)
                .Append(XID)
                .Append(mdStatus)
                .Append(mdErrorMsg)
                .Append(txstatus)
                .Append(iReqCode)
                .Append(iReqDetail)
                .Append(vendorCode)
                .Append(eci)
                .Append(cavv)
                .Append(cavvAlgorithm)
                .Append(MD)
                .Append(_myDefs.Payment3DSecureDetails2)
            End With

            Dim encoder As New System.Text.UTF8Encoding
            Dim digestBytes() As Byte = encoder.GetBytes(DigestFromSessionValues.ToString)
            Dim SHA1Hasher As New SHA1CryptoServiceProvider
            digestBytes = SHA1Hasher.ComputeHash(digestBytes)
            Dim digestBase64FromSessionValues As String = Convert.ToBase64String(digestBytes)

        End If
        Return transactionOk
    End Function

    Private Function check3dSecureWinbank() As Boolean

        _talentLogging.FrontEndConnectionString = _wfr.FrontEndConnectionString
        _talentLogging.GeneralLog(_wfr.PageCode, "check3dSecureWinbank", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")

        Dim transactionOk As Boolean = False

        Dim ourAccountId As String = _myDefs.Payment3dSecureOurAccountId
        Dim ourSystemGUID As String = _myDefs.Payment3dSecureOurSystemGUID
        Dim ourSystemId As String = _myDefs.Payment3dSecureOurSystemId
        Dim ourPasscode As String = _myDefs.Payment3dSecureOurPasscode
        Dim Resp As String = String.Empty

        Dim PARes As String = String.Empty
        If Not Request.Params("PARes") Is Nothing Then
            PARes = Request.Params("PARes")
        End If

        Dim ccRequest As New CentinelRequest()
        Dim ccResponse As New CentinelResponse()

        If Not String.IsNullOrEmpty(Session("Enrolled")) Then
            enrolled = Session("Enrolled").ToString
        End If
        If Session("Enrolled") = "N" Or Session("Enrolled") = "U" Then
            If Not Session("WinbankECI") Is Nothing Then
                eci = Session("WinbankECI")
                Session("WinbankECI") = Nothing
            End If
            If Not Session("MerchantReference") Is Nothing Then
                talent3dSecureTransactionID = Session("MerchantReference")
                Session("MerchantReference") = Nothing
            End If

            transactionOk = True
        ElseIf Session("Enrolled") = "U" Then
            transactionOk = _myDefs.Payment3dSecurePassIfEnrolledCodeU
            If transactionOk Then
                If Not Session("WinbankECI") Is Nothing Then
                    eci = Session("WinbankECI")
                    Session("WinbankECI") = Nothing
                End If
                If Not Session("MerchantReference") Is Nothing Then
                    talent3dSecureTransactionID = Session("MerchantReference")
                    Session("MerchantReference") = Nothing
                End If
            Else
                Session("TicketingGatewayError") = "3DSecureMDStatus0"
            End If
        ElseIf Session("Enrolled") = "TIMEOUT" Then
            transactionOk = _myDefs.Payment3dSecurePassIfResponseTimeout
            If transactionOk Then
                If Not Session("WinbankECI") Is Nothing Then
                    eci = Session("WinbankECI")
                    Session("WinbankECI") = Nothing
                End If
                If Not Session("MerchantReference") Is Nothing Then
                    talent3dSecureTransactionID = Session("MerchantReference")
                    Session("MerchantReference") = Nothing
                End If
            Else
                Session("TicketingGatewayError") = "3DSecureMDStatus0"
            End If
        ElseIf Session("Enrolled") = "Y" Then
            _javaScriptRedirect = True
            '---------------------------
            ' Call Authenticate function
            '---------------------------
            ccRequest.add("MsgType", "cmpi_authenticate")
            ccRequest.add("Version", "1.7")
            ccRequest.add("ProcessorId", _myDefs.Payment3DSecureDetails1)
            ccRequest.add("MerchantId", _myDefs.Payment3DSecureDetails2)
            ccRequest.add("TransactionPwd", _myDefs.Payment3DSecureDetails3)
            ccRequest.add("TransactionType", "C")
            ccRequest.add("TransactionId", Session("PayerAuthRequestId").ToString)
            ccRequest.add("PAResPayload", PARes)



            Dim errorNo As String = String.Empty
            Dim errorDesc As String = String.Empty
            Dim transactionId As String = String.Empty

            ' https://centinel.cardinalcommerce.com/maps/txns.asp = Test
            ' https://centinel.piraeusbank.fdsecure.com/maps/txns.asp = Live
            Try
                ccResponse = ccRequest.sendHTTP(_myDefs.Payment3DSecureURL1, 10000)
                errorNo = ccResponse.getValue("ErrorNo")
                errorDesc = ccResponse.getValue("ErrorDesc")
                ParesStatus = ccResponse.getValue("PAResStatus")
                SignatureVerification = ccResponse.getValue("SignatureVerification")
                cavv = ccResponse.getValue("Cavv")
                transactionId = ccResponse.getValue("TransactionId")
                eci = ccResponse.getValue("EciFlag")
                XID = ccResponse.getValue("Xid")
            Catch ex As Exception
                Utilities.TalentLogging.ExceptionLog("3dSecure - cmpi_authenticate: " & _
                                                               " Basket: " & Profile.Basket.Basket_Header_ID.ToString, ex.Message)
            End Try


            talent3dSecureTransactionID = Session("MerchantReference")
            If SignatureVerification = "Y" AndAlso ParesStatus <> "N" Then
                If ParesStatus = "U" Then
                    transactionOk = _myDefs.Payment3dSecurePassIfEnrolledCodeU
                Else
                    transactionOk = True
                End If
            End If

            If transactionOk = False Then
                'Failed, Redirect User to Checkout.aspx with suitable Error Message
                Session("TicketingGatewayError") = "3DSecureMDStatus0" 'authenticationstatus = "F"
            End If
        Else
            transactionOk = False

        End If


        Return transactionOk
    End Function

    Protected Function check3DSecureECentric() As Boolean
        _talentLogging.FrontEndConnectionString = _wfr.FrontEndConnectionString
        _talentLogging.GeneralLog(_wfr.PageCode, "check3DSecureECentric", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")
        Dim transactionOk As Boolean = False
        _javaScriptRedirect = True

        If Request.Params("PARes") IsNot Nothing AndAlso Request.Params("MD") IsNot Nothing Then
            Session("PARes") = Request.Params("PARes")
            Session("MD") = Request.Params("MD")
            If Profile.Basket.Basket_Header_ID = Session("MD") Then
                transactionOk = True
            End If
        End If
        If Not transactionOk Then
            Session("TicketingGatewayError") = GlobalConstants.ECENTRICGENERICERROR
        End If
        Return transactionOk
    End Function

#End Region

End Class
