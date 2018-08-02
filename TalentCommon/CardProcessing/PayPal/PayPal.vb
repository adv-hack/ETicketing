Imports System
Imports System.Collections
Imports System.Collections.Specialized
Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Data
Imports System.Configuration
Imports System.Web

Namespace CardProcessing.PayPal

    ''' <summary>
    ''' API for Paypal Integration
    ''' </summary>
    Public Class PayPal

#Region "Class Level Fields"
        Private Const CVV2 As String = "CVV2"
        Private Const SIGNATURE As String = "SIGNATURE"
        Private Const PWD As String = "PWD"
        Private Const ACCT As String = "ACCT"

        'Flag that determines the PayPal environment (live or sandbox) 
        Private ReadOnly BSANDBOX As Boolean = True
        Private ReadOnly TIMEOUT As Integer = 5000
        Private Shared ReadOnly SECURED_NVPS As String() = New String() {ACCT, CVV2, SIGNATURE, PWD}

        Private _APIUsername As String = String.Empty
        Private _APIPassword As String = String.Empty
        Private _APISignature As String = String.Empty
        Private _subject As String = String.Empty
        Private _hostURLLive As String = String.Empty
        Private _hostURLSandbox As String = String.Empty
        Private _pendpointurl As String = String.Empty
        Private _endURLSandbox As String = String.Empty
        Private _returnURL As String = String.Empty
        Private _cancelURL As String = String.Empty
#End Region

        Public Property RequestFields() As Generic.Dictionary(Of String, String)

#Region "Constructors"

        ''' <summary>
        ''' Initializes a new instance of the <see cref="PayPal" /> class.
        ''' </summary>
        ''' <param name="IsSandbox">if set to <c>true</c> [is sandbox/test].</param>
        ''' <param name="httpTimeOut">HttpWebRequest Timeout specified in milliseconds</param>
        ''' <param name="endURLLive">The end URL live.</param>
        ''' <param name="endURLSandbox">The end URL sandbox.</param>
        ''' <param name="hostURLLive">The host URL live.</param>
        ''' <param name="hostURLSandbox">The host URL sandbox.</param>
        ''' <param name="returnURL">The return URL.</param>
        ''' <param name="cancelURL">The cancel URL.</param>
        Public Sub New(ByVal IsSandbox As Boolean, ByVal httpTimeOut As Integer,
                       ByVal endURLLive As String, ByVal endURLSandbox As String,
                       ByVal hostURLLive As String, ByVal hostURLSandbox As String,
                       ByVal returnURL As String, ByVal cancelURL As String)
            BSANDBOX = IsSandbox
            TIMEOUT = httpTimeOut
            _pendpointurl = endURLLive
            _endURLSandbox = endURLSandbox
            _hostURLLive = hostURLLive
            _hostURLSandbox = hostURLSandbox
            _returnURL = returnURL
            _cancelURL = cancelURL
        End Sub

#End Region

#Region "Public Methods"


        Public Sub SetCredentials(ByVal userID As String, ByVal password As String, ByVal signature As String, ByVal subject As String)
            _APIUsername = userID
            _APIPassword = password
            _APISignature = signature
            _subject = subject
        End Sub

        ''' <summary> 
        ''' ShortcutExpressCheckout: The method that calls SetExpressCheckout API 
        ''' </summary> 
        ''' <param name="amt"></param> 
        ''' <param name="token"></param> 
        ''' <param name="retMsg"></param> 
        ''' <returns></returns> 
        Public Function ShortcutExpressCheckout(ByVal amt As String, ByRef token As String, ByRef retMsg As String) As Boolean
            Dim host As String = _hostURLLive
            If BSANDBOX Then
                _pendpointurl = _endURLSandbox
                host = _hostURLSandbox
            End If

            Dim returnURL As String = _returnURL
            Dim cancelURL As String = _cancelURL

            Dim encoder As New NVPCodec()
            encoder("METHOD") = "SetExpressCheckout"
            encoder("PAYMENTREQUEST_0_PAYMENTACTION") = "Sale"
            encoder("RETURNURL") = returnURL
            encoder("CANCELURL") = cancelURL
            encoder("PAYMENTREQUEST_0_AMT") = amt
            encoder = GetRequestFields(encoder)
            Dim pStrrequestforNvp As String = encoder.Encode()
            Dim pStresponsenvp As String = HttpCall(pStrrequestforNvp)

            Dim decoder As New NVPCodec()
            decoder.Decode(pStresponsenvp)

            Dim strAck As String = decoder("ACK").ToLower()
            If strAck IsNot Nothing AndAlso (strAck = "success" OrElse strAck = "successwithwarning") Then
                token = decoder("TOKEN")
                Dim ECURL As String = "https://" & host & "/checkoutnow?token=" & token & "&useraction=commit"
                retMsg = ECURL
                Return True
            Else
                retMsg = (("ErrorCode=" & decoder("L_ERRORCODE0") & "&" & "Desc=") + decoder("L_SHORTMESSAGE0") & "&" & "Desc2=") + decoder("L_LONGMESSAGE0")

                Return False
            End If
        End Function

        ''' <summary>
        ''' MarkExpressCheckout: The method that calls SetExpressCheckout API, invoked from the Billing Page EC placement 
        ''' </summary>
        ''' <param name="amt">The payment amount</param>
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
        ''' <param name="addrOverride">The add override setting</param>
        ''' <param name="token">The paypal payment token</param>
        ''' <param name="retMsg">The return message from the service</param>
        ''' <param name="errorCode">Returned error code if there is one</param>
        ''' <returns>True if successful otherwise false</returns>
        ''' <remarks></remarks>
        Public Function MarkExpressCheckout(ByVal amt As String, ByVal shipToName As String, ByVal shipToStreet As String, ByVal shipToStreet2 As String, ByVal shipToCity As String, ByVal shipToState As String, _
            ByVal shipToZip As String, ByVal shipToCountryCode As String, ByVal noShippingFlag As String, ByVal addrOverride As String, ByRef token As String, ByRef retMsg As String, ByRef errorCode As String) As Boolean
            Dim host As String = _hostURLLive
            If BSANDBOX Then
                _pendpointurl = _endURLSandbox
                host = _hostURLSandbox
            End If

            Dim returnURL As String = _returnURL
            Dim cancelURL As String = _cancelURL

            Dim encoder As New NVPCodec()
            encoder("METHOD") = "SetExpressCheckout"
            encoder("PAYMENTREQUEST_0_PAYMENTACTION") = "Sale"
            encoder("RETURNURL") = returnURL
            encoder("CANCELURL") = cancelURL
            encoder("PAYMENTREQUEST_0_AMT") = amt
            encoder = GetRequestFields(encoder)
            'Optional Shipping Address entered on the merchant site 
            encoder("PAYMENTREQUEST_0_SHIPTONAME") = shipToName
            encoder("PAYMENTREQUEST_0_SHIPTOSTREET") = shipToStreet
            encoder("PAYMENTREQUEST_0_SHIPTOSTREET2") = shipToStreet2
            encoder("PAYMENTREQUEST_0_SHIPTOCITY") = shipToCity
            encoder("PAYMENTREQUEST_0_SHIPTOSTATE") = shipToState
            encoder("PAYMENTREQUEST_0_SHIPTOZIP") = shipToZip
            encoder("PAYMENTREQUEST_0_SHIPTOCOUNTRYCODE") = shipToCountryCode
            encoder("NOSHIPPING") = noShippingFlag
            encoder("ADDROVERRIDE") = addrOverride

            Dim pStrrequestforNvp As String = encoder.Encode()
            Dim pStresponsenvp As String = HttpCall(pStrrequestforNvp)
            Dim decoder As New NVPCodec()
            decoder.Decode(pStresponsenvp)

            Dim strAck As String = decoder("ACK").ToLower()
            If strAck IsNot Nothing AndAlso (strAck = "success" OrElse strAck = "successwithwarning") Then
                token = decoder("TOKEN")
                Dim ECURL As String = "https://" & host & "/checkoutnow?token=" & token & "&useraction=commit"
                retMsg = ECURL
                Return True
            Else
                errorCode = decoder("L_ERRORCODE0")
                retMsg = (("ErrorCode=" & decoder("L_ERRORCODE0") & "&" & "Desc=") + decoder("L_SHORTMESSAGE0") & "&" & "Desc2=") + decoder("L_LONGMESSAGE0")
                Return False
            End If
        End Function

        ''' <summary> 
        ''' GetShippingDetails: The method that calls SetExpressCheckout API, invoked from the 
        ''' Billing Page EC placement 
        ''' </summary> 
        ''' <param name="token"></param> 
        ''' <param name="retMsg"></param> 
        ''' <returns></returns> 
        Public Function GetShippingDetails(ByVal token As String, ByRef PayerId As String, ByRef ShippingAddress As String, ByRef retMsg As String) As Boolean

            If BSANDBOX Then
                _pendpointurl = _endURLSandbox
            End If

            Dim encoder As New NVPCodec()
            encoder("METHOD") = "GetExpressCheckoutDetails"
            encoder("TOKEN") = token

            Dim pStrrequestforNvp As String = encoder.Encode()
            Dim pStresponsenvp As String = HttpCall(pStrrequestforNvp)

            Dim decoder As New NVPCodec()
            decoder.Decode(pStresponsenvp)

            Dim strAck As String = decoder("ACK").ToLower()
            If strAck IsNot Nothing AndAlso (strAck = "success" OrElse strAck = "successwithwarning") Then
                ShippingAddress = "<table><tr>"
                ShippingAddress += "<td> First Name </td><td>" & decoder("FIRSTNAME") & "</td></tr>"
                ShippingAddress += "<td> Last Name </td><td>" & decoder("LASTNAME") & "</td></tr>"
                ShippingAddress += "<td colspan='2'> Shipping Address</td></tr>"
                ShippingAddress += "<td> Name </td><td>" & decoder("SHIPTONAME") & "</td></tr>"
                ShippingAddress += "<td> Street1 </td><td>" & decoder("SHIPTOSTREET") & "</td></tr>"
                ShippingAddress += "<td> Street2 </td><td>" & decoder("SHIPTOSTREET2") & "</td></tr>"
                ShippingAddress += "<td> City </td><td>" & decoder("SHIPTOCITY") & "</td></tr>"
                ShippingAddress += "<td> State </td><td>" & decoder("SHIPTOSTATE") & "</td></tr>"
                ShippingAddress += "<td> Zip </td><td>" & decoder("SHIPTOZIP") & "</td>"
                ShippingAddress += "</tr>"

                Return True
            Else
                retMsg = (("ErrorCode=" & decoder("L_ERRORCODE0") & "&" & "Desc=") + decoder("L_SHORTMESSAGE0") & "&" & "Desc2=") + decoder("L_LONGMESSAGE0")

                Return False
            End If
        End Function

        ''' <summary> 
        ''' ConfirmPayment: The method that calls SetExpressCheckout API, invoked from the 
        ''' Billing Page EC placement 
        ''' </summary> 
        ''' <param name="token"></param> 
        ''' <param name="retMsg"></param> 
        ''' <returns></returns> 
        Public Function ConfirmPayment(ByVal finalPaymentAmount As String, ByVal token As String, ByVal PayerId As String, ByRef decoder As NVPCodec, ByRef retMsg As String, ByRef doRedirect As Boolean) As Boolean
            Dim host As String = _hostURLLive
            Dim errorCode As String = String.Empty
            If BSANDBOX Then
                _pendpointurl = _endURLSandbox
                host = _hostURLSandbox
            End If

            Dim encoder As New NVPCodec()
            encoder("METHOD") = "DoExpressCheckoutPayment"
            encoder("TOKEN") = token
            encoder("PAYMENTREQUEST_0_PAYMENTACTION") = "Sale"
            encoder("PAYERID") = PayerId
            encoder("PAYMENTREQUEST_0_AMT") = finalPaymentAmount
            encoder("PAYMENTREQUEST_0_CURRENCYCODE") = GetRequestField("PAYMENTREQUEST_0_CURRENCYCODE")
            encoder("PAYMENTREQUEST_0_ALLOWEDPAYMENTMETHOD") = GetRequestField("PAYMENTREQUEST_0_ALLOWEDPAYMENTMETHOD")
            encoder = GetRequestFields(encoder)
            Dim pStrrequestforNvp As String = encoder.Encode()
            Dim pStresponsenvp As String = HttpCall(pStrrequestforNvp)

            decoder = New NVPCodec()
            decoder.Decode(pStresponsenvp)
            doRedirect = False

            Dim strAck As String = decoder("ACK").ToLower()
            If strAck IsNot Nothing AndAlso (strAck = "success" OrElse strAck = "successwithwarning") Then
                Return True
            Else
                errorCode = decoder("L_ERRORCODE0")
                If errorCode = "10486" Then
                    'This error code is a funding source error and must be used to allow the user to select a different wallet - therefore redirect back to PayPal
                    token = decoder("TOKEN")
                    Dim ECURL As String = "https://" & host & "/checkoutnow?token=" & token & "&useraction=commit"
                    retMsg = ECURL
                    doRedirect = True
                    Return False
                Else
                    retMsg = (("ErrorCode=" & decoder("L_ERRORCODE0") & "&" & "Desc=") + decoder("L_SHORTMESSAGE0") & "&" & "Desc2=") + decoder("L_LONGMESSAGE0")
                    Return False
                End If
            End If
        End Function

        ''' <summary>
        ''' Refund the PayPal transaction based on the PayPal integration guide: https://developer.paypal.com/docs/classic/api/merchant/RefundTransaction_API_Operation_NVP/
        ''' </summary>
        ''' <param name="payPalTransactionId">The current PayPal transaction ID being refunded</param>
        ''' <param name="paymentReference">The TALENT payment reference to send to PayPal for reference purposes</param>
        ''' <param name="amt">The basket total value</param>
        ''' <param name="currencyCode">The current application currency code, eg. GBP</param>
        ''' <param name="decoder">The decoder object used during communications with PayPal</param>
        ''' <param name="retMsg">The return message data</param>
        ''' <param name="refundTransactionId">The PayPal refunded transaction ID</param>
        ''' <returns>Boolean value based on whether or not the refund has succeeded</returns>
        ''' <remarks></remarks>
        Public Function RefundTransaction(ByVal payPalTransactionId As String, ByVal basketHeaderId As String, ByVal paymentReference As String, ByVal amt As String, ByVal currencyCode As String, ByRef decoder As NVPCodec, ByRef retMsg As String, ByRef refundTransactionId As String) As Boolean
            If BSANDBOX Then
                _pendpointurl = _endURLSandbox
            End If

            Dim encoder As New NVPCodec()
            encoder("METHOD") = "RefundTransaction"
            encoder("TRANSACTIONID") = payPalTransactionId
            encoder("MSGSUBID") = basketHeaderId
            encoder("INVOICEID") = paymentReference
            encoder("REFUNDTYPE") = "Partial"
            encoder("AMT") = amt.TrimStart("-")
            encoder("CURRENCYCODE") = currencyCode

            Dim pStrrequestforNvp As String = encoder.Encode()
            Dim pStresponsenvp As String = HttpCall(pStrrequestforNvp)
            decoder = New NVPCodec()
            decoder.Decode(pStresponsenvp)
            refundTransactionId = decoder("REFUNDTRANSACTIONID").ToLower()

            Dim strAck As String = decoder("ACK").ToLower()
            If strAck IsNot Nothing AndAlso strAck = "success" Then
                If refundTransactionId IsNot Nothing AndAlso (refundTransactionId.Length > 0) Then
                    Return True
                Else
                    Return False
                End If
            Else
                retMsg = (("ErrorCode=" & decoder("L_ERRORCODE0") & "&" & "Desc=") + decoder("L_SHORTMESSAGE0") & "&" & "Desc2=") + decoder("L_LONGMESSAGE0")
                Return False
            End If
        End Function

        ''' <summary> 
        ''' HttpCall: The main method that is used for all API calls 
        ''' </summary> 
        ''' <param name="NvpRequest"></param> 
        ''' <returns></returns> 
        Public Function HttpCall(ByVal NvpRequest As String) As String
            'CallNvpServer 
            Dim url As String = _pendpointurl
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            'To Add the credentials from the profile
            Dim codec As New NVPCodec()
            Dim strPost As String = (NvpRequest & "&") + buildCredentialsNVPString()
            Dim retryCount As Integer = 1
            Dim doTheRequest As Boolean = True
            Dim result As String = String.Empty

            Do While doTheRequest
                Dim objRequest As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
                objRequest.Timeout = TIMEOUT
                objRequest.Method = "POST"
                objRequest.ContentLength = strPost.Length
                Try
                    Using myWriter As New StreamWriter(objRequest.GetRequestStream())
                        myWriter.Write(strPost)
                    End Using
                Catch e As Exception
                    Throw
                End Try

                'Retrieve the Response returned from the NVP API call to PayPal 
                Dim objResponse As HttpWebResponse = DirectCast(objRequest.GetResponse(), HttpWebResponse)
                Using sr As New StreamReader(objResponse.GetResponseStream())
                    result = sr.ReadToEnd()
                End Using

                codec.Decode(result)
                If codec("ACK").ToLower() = "failure" Then
                    If retryCount = 3 Then
                        doTheRequest = False
                    Else
                        doTheRequest = True
                    End If
                Else
                    doTheRequest = False
                End If
                retryCount += 1
            Loop
            Return result
        End Function

#End Region

#Region "Private Methods"
        ''' <summary> 
        ''' Credentials added to the NVP string 
        ''' </summary>
        ''' <returns></returns> 
        Private Function buildCredentialsNVPString() As String
            Dim codec As New NVPCodec()

            If Not IsEmpty(_APIUsername) Then
                codec("USER") = _APIUsername
            End If

            If Not IsEmpty(_APIPassword) Then
                codec(PWD) = _APIPassword
            End If

            If Not IsEmpty(_APISignature) Then
                codec(SIGNATURE) = _APISignature
            End If

            If Not IsEmpty(_subject) Then
                codec("SUBJECT") = _subject
            End If
            codec("VERSION") = "204.0"

            Return codec.Encode()
        End Function

        Private Function GetRequestFields(ByVal NVPCoder As NVPCodec) As NVPCodec
            For Each RequestField As KeyValuePair(Of String, String) In RequestFields
                NVPCoder(RequestField.Key) = RequestField.Value
            Next
            Return NVPCoder
        End Function

        Private Function GetRequestField(ByVal fieldName As String) As String
            Dim fieldValue As String = String.Empty
            If Not RequestFields.TryGetValue(fieldName, fieldValue) Then
                fieldValue = String.Empty
            End If
            Return fieldValue
        End Function


        ''' <summary> 
        ''' Returns if a string is empty or null 
        ''' </summary> 
        ''' <param name="s">the string</param> 
        ''' <returns>true if the string is not null and is not empty or just whitespace</returns> 
        Public Shared Function IsEmpty(ByVal s As String) As Boolean
            Return s Is Nothing OrElse s.Trim() = String.Empty
        End Function

#End Region

    End Class

End Namespace




