Imports Microsoft.VisualBasic
Imports System.IO
Imports System.web
Imports System.Xml
Imports System.data
Imports System.Net
Imports System.Text
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    HSBCApiRequest.vb
'
'       Date                        15/08/07
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      HSBCAPI- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       15/08/07   /000    Ben     Created. Dunhill
'
'--------------------------------------------------------------------------------------------------

Namespace CardProcessing.HSBC

    Public Class HSBCApiRequest

        Private _xmlBuilder As New StringBuilder
        Private _xmlVoidBuilder As New StringBuilder
        Dim _xmlRequestDoc As New XmlDocument
        Dim _xmlVoidRequestDoc As New XmlDocument

        Dim _xmlResponseDoc As New XmlDocument
        Dim _xmlVoidResponseDoc As New XmlDocument

        Private _url As String = String.Empty
        Private _debug As String = String.Empty

        Private _user As String = String.Empty
        Private _password As String = String.Empty
        Private _clientId As String = String.Empty
        Private _clientAlias As String = String.Empty
        Private _processingMode As String = String.Empty
        Private _ccNumber As String = String.Empty
        Private _ccExpiry As String = String.Empty
        Private _ccStartDate As String = String.Empty
        Private _issueNo As String = String.empty
        Private _addressLine1 As String = String.empty
        Private _postCode As String = String.Empty
        Private _deliveryAddressLine1 As String = String.Empty
        Private _deliveryPostCode As String = String.empty
        Private _country As String = "826"
        Private _amountPence As String = String.Empty
        Private _csc As String = String.Empty
        Private _emailAddress As String = String.empty
        Private _firstName As String = String.Empty
        Private _lastName As String = String.Empty
        Private _currency As String = "826"

        Private _authType As String = String.Empty
        Private _failureReason As String = String.Empty
        Private _overviewXML As String = String.empty
        Private _payerSecurityLevel As String = String.Empty
        Private _payerAuthenticationCode As String = String.Empty
        Private _payerTxnId As String = String.Empty
        Private _cardHolderPresentCode As String = String.Empty

        Private _rejectAdAvs As Boolean = False
        Private _rejectPcAvs As Boolean = False
        Private _rejectCsc As Boolean = False
        '
        ' Default AVS and CSC to 'Y'
        Private _submitAvs As String = "Y"
        Private _submitCsc As String = "Y"
        '**************************************************************
        ' Return properties
        '**************************************************************
        '
        ' Is Approved - Has the transaction been accepted
        Private _isApproved As Boolean = False
        '
        ' Transaction Fail - E.g. Server errors
        Private _transactionFail As Boolean = False
        '
        ' Requires Review - E.g. Fails fraud check
        Private _requiresReview As Boolean = False

#Region "Property Definition Section"

        Public Property url() As String
            Get
                Return _url
            End Get
            Set(ByVal value As String)
                _url = value
            End Set
        End Property
        Public Property debug() As String
            Get
                Return _debug
            End Get
            Set(ByVal value As String)
                _debug = value
            End Set
        End Property
        Public WriteOnly Property user() As String
            Set(ByVal value As String)
                _user = value
            End Set
        End Property
        Public WriteOnly Property password() As String
            Set(ByVal value As String)
                _password = value
            End Set
        End Property
        Public Property clientId() As String
            Get
                Return _clientId
            End Get
            Set(ByVal value As String)
                _clientId = value
            End Set
        End Property
        Public Property clientAlias() As String
            Get
                Return _clientAlias
            End Get
            Set(ByVal value As String)
                _clientAlias = value
            End Set
        End Property
        Public Property ccNumber() As String
            Get
                Return _ccNumber
            End Get
            Set(ByVal value As String)
                _ccNumber = value
            End Set
        End Property
        Public Property ccExpiry() As String
            Get
                Return _ccExpiry
            End Get
            Set(ByVal value As String)
                _ccExpiry = value
            End Set
        End Property
        Public WriteOnly Property ccStartDate() As String
            Set(ByVal value As String)
                _ccStartDate = value
            End Set
        End Property
        Public WriteOnly Property issueNo() As String
            Set(ByVal value As String)
                _issueNo = value
            End Set
        End Property
        Public Property addressLine1() As String
            Get
                Return _addressLine1
            End Get
            Set(ByVal value As String)
                _addressLine1 = value
            End Set
        End Property
        Public WriteOnly Property postCode() As String
            Set(ByVal value As String)
                _postCode = value
            End Set
        End Property
        Public Property deliveryAddressLine1() As String
            Get
                Return _deliveryAddressLine1
            End Get
            Set(ByVal value As String)
                _deliveryAddressLine1 = value
            End Set
        End Property
        Public WriteOnly Property deliveryPostCode() As String
            Set(ByVal value As String)
                _deliveryPostCode = value
            End Set
        End Property
        Public WriteOnly Property country() As String
            Set(ByVal value As String)
                _country = value
            End Set
        End Property
        Public WriteOnly Property currency() As String
            Set(ByVal value As String)
                _currency = value
            End Set
        End Property
        Public Property amountPence() As String
            Get
                Return _amountPence
            End Get
            Set(ByVal value As String)
                _amountPence = value
            End Set
        End Property
        Public WriteOnly Property csc() As String
            Set(ByVal value As String)
                _csc = value
            End Set
        End Property
        Public WriteOnly Property processingMode() As String
            Set(ByVal value As String)
                _processingMode = value
            End Set
        End Property
        Public WriteOnly Property authType() As String
            Set(ByVal value As String)
                _authType = value
            End Set
        End Property
        Public WriteOnly Property submitAvs() As String
            Set(ByVal value As String)
                _submitAvs = value
            End Set
        End Property
        Public WriteOnly Property submitCsc() As String
            Set(ByVal value As String)
                _submitCsc = value
            End Set
        End Property
        Public WriteOnly Property emailAddress() As String
            Set(ByVal value As String)
                _emailAddress = value
            End Set
        End Property
        Public WriteOnly Property firstName() As String
            Set(ByVal value As String)
                _firstName = value
            End Set
        End Property
        Public WriteOnly Property lastName() As String
            Set(ByVal value As String)
                _lastName = value
            End Set
        End Property
        '
        ' Additonal 3dsecure properties
        Public WriteOnly Property payerSecurityLevel() As String
            Set(ByVal value As String)
                _payerSecurityLevel = value
            End Set
        End Property
        Public WriteOnly Property payerAuthenticationCode() As String
            Set(ByVal value As String)
                _payerAuthenticationCode = value
            End Set
        End Property
        Public WriteOnly Property payerTxnId() As String
            Set(ByVal value As String)
                _payerTxnId = value
            End Set
        End Property
        Public WriteOnly Property cardholderPresentCode() As String
            Set(ByVal value As String)
                _cardHolderPresentCode = value
            End Set
        End Property

        Public Property RejectAdAvs() As Boolean
            Get
                Return _rejectAdAvs
            End Get
            Set(ByVal Value As Boolean)
                _rejectAdAvs = Value
            End Set
        End Property
        Public Property RejectPcAvs() As Boolean
            Get
                Return _rejectPcAvs
            End Get
            Set(ByVal Value As Boolean)
                _rejectPcAvs = Value
            End Set
        End Property
        Public Property RejectCsc() As Boolean
            Get
                Return _rejectCsc
            End Get
            Set(ByVal Value As Boolean)
                _rejectCsc = Value
            End Set
        End Property
        Public Property HSBCOrderID() As String
        '****************************************************************
        ' IsApproved - Final check to see if a transaction was successful
        ' or not.
        '****************************************************************
        Public ReadOnly Property isApproved() As Boolean
            Get
                Return _isApproved
            End Get
        End Property
        '****************************************************************
        ' /006
        ' FailureReason - Additional text about why it failed
        '****************************************************************
        Public ReadOnly Property failureReason() As String
            Get
                Return _failureReason
            End Get
        End Property
        '****************************************************************
        ' OverviewXML - Inner xml of overview in return message
        '****************************************************************
        Public ReadOnly Property overviewXML() As String
            Get
                Return _overviewXML
            End Get
        End Property
        '**************************************************************
        ' TransactionFail - E.g. server/comms errors. 
        '**************************************************************
        Public ReadOnly Property transactionFail() As Boolean
            Get
                Return _transactionFail
            End Get
        End Property
        '**************************************************************
        ' RequiresReview - E.g. Fails Fraud rule. Review via Store's
        ' HSBC website
        '**************************************************************
        Public ReadOnly Property requiresReview() As Boolean
            Get
                Return _requiresReview
            End Get
        End Property

        Private _logText As String
        Public Property LogText() As String
            Get
                Return _logText
            End Get
            Set(ByVal value As String)
                _logText = value
            End Set
        End Property

        Private _AVSorCSCRejected As Boolean = False
        Public Property AvsOrCscRejected() As Boolean
            Get
                Return _AVSorCSCRejected
            End Get
            Set(ByVal value As Boolean)
                _AVSorCSCRejected = value
            End Set
        End Property


        Private _ipAddress As String
        Public Property IPAddress() As String
            Get
                Return _ipAddress
            End Get
            Set(ByVal value As String)
                _ipAddress = value
            End Set
        End Property
        '-------------------------------------------
        ' 3d Secure specific values from return post
        '-------------------------------------------
        Private _SecCardBrand As String
        Public Property SecCardBrand() As String
            Get
                Return _SecCardBrand
            End Get
            Set(ByVal value As String)
                _SecCardBrand = value
            End Set
        End Property

        Private _SecCcpaClientID As String
        Public Property SecCcpaClientID() As String
            Get
                Return _SecCcpaClientID
            End Get
            Set(ByVal value As String)
                _SecCcpaClientID = value
            End Set
        End Property

        Private _SecMd As String
        Public Property SecMd() As String
            Get
                Return _SecMd
            End Get
            Set(ByVal value As String)
                _SecMd = value
            End Set
        End Property

        Private _SecPurchaseCurrency As String
        Public Property SecPurchaseCurrency() As String
            Get
                Return _SecPurchaseCurrency
            End Get
            Set(ByVal value As String)
                _SecPurchaseCurrency = value
            End Set
        End Property

        Private _SecPurchaseAmountRaw As String
        Public Property SecPurchaseAmountRaw() As String
            Get
                Return _SecPurchaseAmountRaw
            End Get
            Set(ByVal value As String)
                _SecPurchaseAmountRaw = value
            End Set
        End Property

        Private _SecXid As String
        Public Property SecXid() As String
            Get
                Return _SecXid
            End Get
            Set(ByVal value As String)
                _SecXid = value
            End Set
        End Property

        Private _SecTransactionStatus As String
        Public Property SecTransactionStatus() As String
            Get
                Return _SecTransactionStatus
            End Get
            Set(ByVal value As String)
                _SecTransactionStatus = value
            End Set
        End Property

        Private _SecCAVV As String
        Public Property SecCAVV() As String
            Get
                Return _SecCAVV
            End Get
            Set(ByVal value As String)
                _SecCAVV = value
            End Set
        End Property

        Private _SecCaVVAlgorithm As String
        Public Property SecCaVVAlgorithm() As String
            Get
                Return _SecCaVVAlgorithm
            End Get
            Set(ByVal value As String)
                _SecCaVVAlgorithm = value
            End Set
        End Property

        Private _ECI As String
        Public Property SecECI() As String
            Get
                Return _ECI
            End Get
            Set(ByVal value As String)
                _ECI = value
            End Set
        End Property

        Private _SecPurchaseDate As String
        Public Property SecPurchaseDate() As String
            Get
                Return _SecPurchaseDate
            End Get
            Set(ByVal value As String)
                _SecPurchaseDate = value
            End Set
        End Property

        Private _CcpaResultsCode As String
        Public Property SecCcpaResultsCode() As String
            Get
                Return _CcpaResultsCode
            End Get
            Set(ByVal value As String)
                _CcpaResultsCode = value
            End Set
        End Property

        Private _SecIreqCode As String
        Public Property SecIreqCode() As String
            Get
                Return _SecIreqCode
            End Get
            Set(ByVal value As String)
                _SecIreqCode = value
            End Set
        End Property

        Private _SecIreqVendorCode As String
        Public Property SecIreqVendorCode() As String
            Get
                Return _SecIreqVendorCode
            End Get
            Set(ByVal value As String)
                _SecIreqVendorCode = value
            End Set
        End Property

        Private _SecIreqDetail As String
        Public Property SecIreqDetail() As String
            Get
                Return _SecIreqDetail
            End Get
            Set(ByVal value As String)
                _SecIreqDetail = value
            End Set
        End Property

#End Region
        '**************************************************************
        ' Buld the XML doc using properties and store in _XMLBuilder
        '**************************************************************
        Public Sub buildXml()
            LogText = "Building XML Doc"
            '
            ' Header details
            With _xmlBuilder
                .Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
                .Append("<EngineDocList>")
                .Append("<DocVersion>1.0</DocVersion>")
                .Append("<EngineDoc>")
                .Append("<ContentType>OrderFormDoc</ContentType>")
                ' Set customer IP address
                If Not IPAddress Is Nothing AndAlso IPAddress <> String.Empty Then
                    .Append("<IPAddress>" & IPAddress & "</IPAddress>")
                End If

                .Append("<User>")
            End With
            '
            ' Use client alias where possible
            If _clientAlias <> String.Empty Then
                _xmlBuilder.Append("<Alias>" & HttpUtility.HtmlEncode(_clientAlias.Trim) & "</Alias>")
            Else
                If _clientId <> String.Empty Then
                    _xmlBuilder.Append("<ClientId DataType=""S32"">" & HttpUtility.HtmlEncode(_clientId.Trim) & "</ClientId>")
                End If
            End If

            With _xmlBuilder
                .Append("<Name>" & HttpUtility.HtmlEncode(_user.Trim) & "</Name>")
                .Append("<Password>" & HttpUtility.HtmlEncode(_password.Trim) & "</Password>")
                .Append("</User>")
                .Append("<Instructions>")
                .Append("<Pipeline>Payment</Pipeline>")
                .Append("</Instructions>")

                .Append("<OrderFormDoc>")
            End With

            If _HSBCOrderID <> String.Empty Then
                _xmlBuilder.Append("<Id>" & _HSBCOrderID & "</Id>")
            End If

            '
            ' Mode (default to test if not set in page): 
            ' Y=Test. Simulator will accept.
            ' N=Test. Simulator will reject
            ' P=Production Mode (Live)
            If _processingMode = String.Empty Then
                _xmlBuilder.Append("<Mode>Y</Mode>")
            Else
                _xmlBuilder.Append("<Mode>" & _processingMode.Trim & "</Mode>")
            End If

            '
            ' Consumer details
            _xmlBuilder.Append("<Consumer>")

            If _emailAddress <> String.Empty Then
                _xmlBuilder.Append("<Email>" & HttpUtility.HtmlEncode(_emailAddress.Trim) & "</Email>")
            End If

            '
            ' Check if need to submit AVS
            If _submitAvs = "Y" Then
                With _xmlBuilder
                    '
                    ' AVS dets
                    .Append("<BillTo>")
                    .Append("<Location>")
                    .Append("<Address>")
                    .Append("<Country>" & HttpUtility.HtmlEncode(_country) & "</Country>")
                    .Append("<FirstName>" & HttpUtility.HtmlEncode(_firstName) & "</FirstName>")
                    .Append("<LastName>" & HttpUtility.HtmlEncode(_lastName) & "</LastName>")
                    .Append("<Name>" & HttpUtility.HtmlEncode(_firstName & _lastName) & "</Name>")
                    .Append("<PostalCode>" & HttpUtility.HtmlEncode(_postCode.Trim) & "</PostalCode>")
                    .Append("<Street1>" & HttpUtility.HtmlEncode(_addressLine1.Trim) & "</Street1>")
                    .Append("</Address>")
                    .Append("</Location>")
                    .Append("</BillTo>")
                End With
            End If

            With _xmlBuilder
                .Append("<PaymentMech>")
                '
                ' Credit card dets
                .Append("<CreditCard>")
            End With
            '
            ' Check if need to submit CSC
            If _submitCsc = "Y" And _csc.Trim <> String.Empty Then
                With _xmlBuilder
                    '
                    ' Cvv2Indicator:
                    ' 0=Store does not support CVV
                    ' 1=Submitted
                    ' 2=Not present on card
                    ' 3=Present but illegible
                    ' 4=Store does not support CVM
                    ' 5=CVM intentionally not provided
                    .Append("<Cvv2Indicator>1</Cvv2Indicator>")
                    .Append("<Cvv2Val>" & HttpUtility.HtmlEncode(_csc.Trim) & "</Cvv2Val>")
                End With
            End If

            _xmlBuilder.Append("<Expires DataType=")
            _xmlBuilder.Append("""")
            _xmlBuilder.Append("ExpirationDate")
            _xmlBuilder.Append("""")
            _xmlBuilder.Append(" Locale=")
            _xmlBuilder.Append("""")
            _xmlBuilder.Append(_country.Trim)
            _xmlBuilder.Append("""")
            _xmlBuilder.Append(">")
            _xmlBuilder.Append(HttpUtility.HtmlEncode(_ccExpiry.Trim))
            _xmlBuilder.Append("</Expires>")

            If _issueNo <> String.Empty Then
                _xmlBuilder.Append("<IssueNum>" & HttpUtility.HtmlEncode(_issueNo.Trim) & "</IssueNum>")
            End If

            _xmlBuilder.Append("<Number>" & HttpUtility.HtmlEncode(_ccNumber.Trim) & "</Number>")

            '
            ' Check if need to add issue date and issue number
            If _ccStartDate <> String.Empty Then
                ' /004 START
                ' _xmlBuilder.Append("<StartDate DataType=""StartDate"" Locale=""826"">" & HttpUtility.HtmlEncode(_ccStartDate.Trim) & "</StartDate>")
                _xmlBuilder.Append("<StartDate DataType=")
                _xmlBuilder.Append("""")
                _xmlBuilder.Append("StartDate")
                _xmlBuilder.Append("""")
                _xmlBuilder.Append(" Locale=")
                _xmlBuilder.Append("""")
                _xmlBuilder.Append(_country.Trim)
                _xmlBuilder.Append("""")
                _xmlBuilder.Append(">")
                _xmlBuilder.Append(HttpUtility.HtmlEncode(_ccStartDate.Trim))
                _xmlBuilder.Append("</StartDate>")
            End If

            With _xmlBuilder
                .Append("</CreditCard>")
            End With

            '
            ' Default Auth Type to PreAuth if not set
            If _authType = String.Empty Then
                _authType = "PreAuth"
            End If

            _xmlBuilder.Append("</PaymentMech>")

            '
            ' Check if need to write Delivery dets
            If _deliveryAddressLine1 <> String.Empty Or _deliveryPostCode <> String.Empty Then
                _xmlBuilder.Append("<ShipTo>")
                _xmlBuilder.Append("<Location>")
                _xmlBuilder.Append("<Address>")
                If _deliveryPostCode <> String.Empty Then
                    _xmlBuilder.Append("<PostalCode>" & HttpUtility.HtmlEncode(_deliveryPostCode.Trim) & "</PostalCode>")
                End If
                If _deliveryAddressLine1 <> String.Empty Then
                    _xmlBuilder.Append("<Street1>" & HttpUtility.HtmlEncode(_deliveryAddressLine1.Trim) & "</Street1>")
                End If
                _xmlBuilder.Append("</Address>")
                _xmlBuilder.Append("</Location>")
                _xmlBuilder.Append("</ShipTo>")
            End If

            With _xmlBuilder

                .Append("</Consumer>")
                '
                ' Transaction details
                .Append("<Transaction>")
            End With

            ' Additional 3d-secure info
            If _cardHolderPresentCode <> String.Empty Then
                _xmlBuilder.Append("<CardholderPresentCode>")
                _xmlBuilder.Append(HttpUtility.HtmlEncode(_cardHolderPresentCode))
                _xmlBuilder.Append("</CardholderPresentCode>")
            End If
            If _payerAuthenticationCode <> String.Empty Then
                _xmlBuilder.Append("<PayerAuthenticationCode>")
                _xmlBuilder.Append(HttpUtility.HtmlEncode(_payerAuthenticationCode))
                _xmlBuilder.Append("</PayerAuthenticationCode>")
            End If
            If _payerSecurityLevel <> String.Empty Then
                _xmlBuilder.Append("<PayerSecurityLevel>")
                _xmlBuilder.Append(HttpUtility.HtmlEncode(_payerSecurityLevel))
                _xmlBuilder.Append("</PayerSecurityLevel>")
            End If
            If _payerTxnId <> String.Empty Then
                _xmlBuilder.Append("<PayerTxnId>")
                _xmlBuilder.Append(HttpUtility.HtmlEncode(_payerTxnId))
                _xmlBuilder.Append("</PayerTxnId>")
            End If

            With _xmlBuilder
                .Append("<Type>" & HttpUtility.HtmlAttributeEncode(_authType) & "</Type>")
                '
                ' Totals
                .Append("<CurrentTotals>")
                .Append("<Totals>")
                .Append("<Total DataType=")
                .Append("""")
                .Append("Money")
                .Append("""")
                .Append(" Currency=")
                .Append("""")
                .Append(_currency.Trim)
                .Append("""")
                .Append(">")
                .Append(HttpUtility.HtmlEncode(_amountPence.Trim))
                .Append("</Total>")
                .Append("</Totals>")
                .Append("</CurrentTotals>")
                .Append("</Transaction>")

                .Append("</OrderFormDoc>")

                .Append("</EngineDoc>")
                .Append("</EngineDocList>")
            End With
            '

            Try
                ' Convert to XML object
                LogText = "Converting to XML Doc"
                _xmlRequestDoc.LoadXml(_xmlBuilder.ToString)
            Catch ex As Exception
            End Try

            If _debug <> "0" Then
                'LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), "CreditCards", _
                '       "XML Request:" & Environment.NewLine & _
                '      _xmlRequestDoc.OuterXml)
            End If
        End Sub
        '********************************************************
        ' DEBUG - Return Entire Xml Request as a string
        '********************************************************
        Public Function returnXmlRequestAsString() As String
            Return _xmlRequestDoc.OuterXml.ToString
        End Function
        '****************************************************************
        ' Send the XML document to the bank
        '****************************************************************
        Public Sub sendXml()
            Dim result As String = ""
            Dim strPost As String = System.Web.HttpUtility.UrlEncode(_xmlRequestDoc.OuterXml.ToString)

            Dim myWriter As StreamWriter = Nothing
            Dim objRequest As HttpWebRequest
            Dim transactionStatus As String = String.Empty
            'Dim objUTF8Encoding = New UTF8Encoding
            Dim arrRequest As Byte()
            Dim strmRequest As Stream
            arrRequest = Encoding.UTF8.GetBytes(strPost)

            LogText = "Begining send to bank.."

            objRequest = CType(WebRequest.Create(url), HttpWebRequest)

            objRequest.Method = "POST"
            '  objRequest.ContentLength = strPost.Length
            objRequest.ContentLength = arrRequest.Length

            objRequest.ContentType = "text/XML"
            ' objRequest.ContentType = "application/x-www-form-urlencoded"

          
            Try
                strmRequest = objRequest.GetRequestStream()
                strmRequest.Write(arrRequest, 0, arrRequest.Length)
                strmRequest.Close()
                ' myWriter = New StreamWriter(objRequest.GetRequestStream(), System.Text.Encoding.UTF8, 1000)
                ' myWriter = New StreamWriter(objRequest.GetRequestStream())
                ' myWriter.Write(byted, 0, byted.Length)
                ' myWriter.Write(strPost)
            Catch e As Exception
                'LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), _
                '  "HSBCApiRequest.SendXml.vb" & Environment.NewLine & e.Message)
                _failureReason = "Failed to create streamwriter"
                _transactionFail = True
                LogText = "Failed to create streamwriter"
                Exit Sub
            End Try

            '            myWriter.Close()

            Dim objResponse As HttpWebResponse

            Try
                objResponse = CType(objRequest.GetResponse(), HttpWebResponse) '

                Using sr As StreamReader = New StreamReader(objResponse.GetResponseStream())
                    result = sr.ReadToEnd()

                    ' Close and clean up the StreamReader
                    sr.Close()
                End Using
            Catch e As Exception
                'LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), _
                '  "HSBCApiRequest.SendXml." & Environment.NewLine & e.Message)
                _failureReason = "Failed to read stream"
                _transactionFail = True
                LogText = "Failed to read stream"
                Exit Sub
            End Try

            '
            ' Check valid XML doc returned
            Try
                _xmlResponseDoc.LoadXml(result)
            Catch e As Exception
                'LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), _
                '  "HSBCApiRequest.SendXml.vb. Invalid XML." & Environment.NewLine & e.Message)
                _failureReason = "Invalid XML doc returned"
                _transactionFail = True
                LogText = "Invalid XML doc returned"
                Exit Sub
            End Try

            If _debug <> "0" Then
                'LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), "CreditCards", _
                '       "XML Response:" & Environment.NewLine & _
                '      _xmlResponseDoc.OuterXml)
            End If

            Try
                _overviewXML = _xmlResponseDoc.SelectSingleNode("/EngineDocList/EngineDoc/Overview").InnerXml
            Catch ex As Exception

            End Try

            '
            ' Check results and set properties for caller to check
            transactionStatus = returnTransactionStatus()
            If transactionStatus = "A" Or transactionStatus = "F" Then
                '
                ' Do I need to validate CSC and Avs? If so and it's failed
                ' then void the current transaction
                If transactionStatus = "A" Then
                    LogText = "TransactionStatus = 'A' - Approved"
                    _isApproved = True
                Else
                    LogText = "TransactionStatus = 'F' - Needs review"
                End If

                ' Check AVS/CSC response
                Dim avsResp As String = returnAvsDisplay()
                Dim cscResp As String = returnCvv2Resp()
                If (RejectAdAvs AndAlso avsResp.Substring(0, 1) <> "Y") Or _
                    (RejectPcAvs AndAlso avsResp.Substring(1, 1) <> "Y") Or _
                    (RejectCsc AndAlso cscResp <> "1") Then

                    voidLastTransaction()
                    LogText = "Voiding transaction due to CSC failure - " & returnCcReturnMsg()
                    _failureReason = "CSC/AVS Failure - " & returnCcReturnMsg()
                    _isApproved = False
                    AvsOrCscRejected = True
                End If
            End If
            '
            ' Check if transaction is accepted but needs review
            If transactionStatus = "F" Then
                If returnFraudStatus() = "Review" Then
                    _requiresReview = True
                    _isApproved = True
                Else
                    _isApproved = False
                    _failureReason = "Fraud Status - " & returnCcReturnMsg()
                    LogText = _failureReason
                End If
            End If
            '
            ' Check if Max Sev has failed and payment still processed
            If returnResponseMaxSev() = "5" Or _
                returnResponseMaxSev() = "6" Or _
                returnResponseMaxSev() = "7" Or _
                returnResponseMaxSev() = "8 " Then
                If returnStatus() = "1" Then
                    'LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), _
                    '    "HSBCApiRequest.SendXml.vb" & Environment.NewLine & _
                    '    "REVIEW ERROR. Payment may have been processed:")
                    logErrors()

                End If
            End If

            If Not _isApproved And _failureReason = String.Empty Then
                _failureReason = returnCcReturnMsg()
                LogText = _failureReason
                If LogText.Contains("System error") Then
                    LogText = returnNotice()
                End If
            End If

        End Sub
        '********************************************************
        ' DEBUG - Return Entire Xml Response
        '********************************************************
        Public Function returnXmlResponseAsString() As String
            Return _xmlResponseDoc.OuterXml.ToString
        End Function
        '*********************************************************
        ' Return value of the MaxSev (max error severity) tag.
        ' Errors 0-4 wont normally stop a transaction
        ' 0 = Success
        ' 1 = Debug
        ' 2 = Informational
        ' 3 = Notice
        ' 4 = Warning
        ' 5 = Error
        ' 6 = Critical 
        ' 7 = Alert
        ' 8 = Emergency
        '*********************************************************
        Public Function returnResponseMaxSev() As String
            Return getXmlData("MaxSev")
        End Function
        '***************************************************************
        ' Return value of the ProcReturnMsg tag 
        ' Approved = Success
        ' Declined = Fail
        ' Transaction marked as Fraudulent = Fail
        '***************************************************************
        Public Function returnProcReturnMsg() As String
            Return getXmlData("ProcReturnMsg")
        End Function
        '***************************************************************
        ' Return value of the ProcReturnCode tag 
        ' 00     = Approved
        ' 02     = Referred
        ' 03     = Merchant unknown
        ' 04     = Card Retained
        ' 05     = Declined (general)
        ' 12     = Request Invalid
        ' 13     = Non-numeric amount
        ' 14     = Invalid digit check
        ' 30     = Error Condition
        ' 54     = Expired card
        ' 58     = Invalid Terminal ID
        ' (refer to appendix E of Api Engine reference for others)
        '***************************************************************
        Public Function returnProcReturnCode() As String
            Return getXmlData("ProcReturnCode")
        End Function
        '***************************************************************
        ' Return value of the CcErrCode tag 
        ' 1     = Approved
        ' 3     = Referred
        ' 1016  = Merchant unknown
        ' 2062  = Card Retained
        ' 50    = Declined (general)
        ' 1036  = Request Invalid
        ' 1002  = Non-numeric amount
        ' 1010  = Invalid digit check
        ' 1051  = Expired card
        ' 1035  = Invalid Terminal ID
        ' 51    = Connection timed out
        ' 52    = Error connecting to processor or sending data
        ' 53    = Error during payment commit phase
        ' 54    = Timed-out waiting for a response
        ' (refer to appendix E of Api Engine reference for others)
        '***************************************************************
        Public Function returnCCerrCode() As String
            Return getXmlData("CcErrCode")
        End Function
        '***************************************************************
        ' Return value of the CcReturnMsg tag 
        ' Approved = Success
        ' Declined = Fail
        ' Transaction marked as Fraudulent = Fail
        '***************************************************************
        Public Function returnCcReturnMsg() As String
            Return getXmlData("CcReturnMsg")
        End Function
        Public Function returnNotice() As String
            Return getXmlData("Notice")
        End Function
        '***************************************************************
        ' Return value of the TransactionStatus tag 
        ' A  = Approved (but not flagged for settlement)
        ' B  = Bulk ready
        ' C  = Captured
        ' D  = Declined
        ' H  = Host Captured
        ' I  = Pending
        ' L  = Locked Transaction
        ' NW = New transaction
        ' P  = Pending settlement
        ' R  = Referred
        ' S  = Settled
        ' SH  = Hard Settlement error
        ' SS  = Soft settlement error
        ' V   = Voided
        ' (refer to appendix E of Api Engine reference for more details)
        '***************************************************************
        Public Function returnTransactionStatus() As String
            Return getXmlData("TransactionStatus")
        End Function
        '***************************************************************
        ' Return Auth Code
        '***************************************************************
        Public Function returnAuthCode() As String
            Return getXmlData("AuthCode")
        End Function
        '***************************************************************
        ' Return Status
        ' This determines whether there was an internal problem in the
        ' transaction. If present, then payment processing has gone 
        ' through OK.
        '***************************************************************
        Public Function returnStatus() As String
            Return getXmlData("Status")
        End Function
        '***************************************************************
        ' Return AVSDisplay
        ' YY - Address and zip code or postal code match 
        ' YN - Address matches only 
        ' NY - Zip code or postal code matches 
        ' NN - Neither address nor zip code or postal code matches; address is not parsable 
        ' UU - Address information is unavailable, service is unavailable, or Error; Unknown 
        ' FF - Issuer does not participate in AVS Blank - No AVS performed 
        '**************************************************************
        Public Function returnAvsDisplay() As String
            Dim nodeList As XmlNodeList
            Dim returnStr As String = String.Empty

            nodeList = _xmlResponseDoc.GetElementsByTagName("AvsDisplay")
            Try

                If Not nodeList Is Nothing And nodeList.Count > 0 Then
                    If Not nodeList(0).FirstChild.Value.ToString Is Nothing Then
                        returnStr = nodeList(0).FirstChild.Value.ToString
                    End If

                End If
            Catch ex As Exception
                'LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), _
                '   "HSBCApiRequest.ReturnAVSDisplay.vb" & Environment.NewLine & ex.Message)
            End Try

            Return returnStr
        End Function
        '***************************************************************
        ' Return Cvv2Resp
        ' Must be one of the following: 
        ' 0 - Card type does not support CVM 
        ' 1 - CVM match 
        ' 2 - CVM did not match issuer value 
        ' 3 - CVM was not processed 
        ' 4 - CVM should be on the card but cardholder indicated otherwise 
        ' 5 - CVM not supported by issuer 
        ' 6 - Not valid 
        ' 7 - No response from server
        '**************************************************************
        Public Function returnCvv2Resp() As String
            Return getXmlData("Cvv2Resp")
        End Function
        '***************************************************************
        ' Return TransactionId
        '**************************************************************
        Public Function returnTransactionId() As String
            Return getXmlData("TransactionId")
        End Function
        '***************************************************************
        ' Return FraudStatus (within Overview section)
        '**************************************************************
        Public Function returnFraudStatus() As String
            Dim returnStr As String = String.Empty
            Dim nodeList As XmlNodeList

            nodeList = _xmlResponseDoc.GetElementsByTagName("Overview")
            If nodeList Is Nothing Then
            Else
                Dim XmlElem As XmlElement = nodeList(0).Item("FraudStatus")
                returnStr = XmlElem.InnerXml
            End If
            Return returnStr
        End Function
        Private Function getXmlData(ByVal node As String) As String
            Dim returnStr As String = String.Empty
            Dim nodeList As XmlNodeList
            nodeList = _xmlResponseDoc.GetElementsByTagName(node)
            If Not nodeList Is Nothing And nodeList.Count > 0 Then
                returnStr = nodeList(0).FirstChild.Value
            End If
            Return returnStr
        End Function
        '***************************************************************
        ' Void the last transaction sent to the bank 
        ' (e.g. if CSC/AVS not valid)
        '***************************************************************
        Public Sub voidLastTransaction()
            Dim transactionId As String = returnTransactionId()
            ' Header details
            With _xmlVoidBuilder
                .Append("<?xml version=""1.0"" encoding=""UTF-8""?>")
                .Append("<EngineDocList>")
                .Append("<DocVersion>1.0</DocVersion>")
                .Append("<EngineDoc>")
                .Append("<ContentType>OrderFormDoc</ContentType>")
                .Append("<User>")
            End With
            ' Check whether need to set CliendID or ClientAlias
            If _clientId <> String.Empty Then
                _xmlVoidBuilder.Append("<ClientId DataType=""S32"">" & HttpUtility.HtmlEncode(_clientId.Trim) & "</ClientId>")
            Else
                If _clientAlias <> "" Then
                    _xmlVoidBuilder.Append("<Alias>" & HttpUtility.HtmlEncode(_clientAlias.Trim) & "</Alias>")
                End If
            End If

            With _xmlVoidBuilder
                .Append("<Name>" & HttpUtility.HtmlEncode(_user.Trim) & "</Name>")
                .Append("<Password>" & HttpUtility.HtmlEncode(_password.Trim) & "</Password>")
                .Append("</User>")
                .Append("<Instructions>")
                .Append("<Pipeline>Payment</Pipeline>")
                .Append("</Instructions>")
                .Append("<OrderFormDoc>")
            End With
            '
            ' Mode (default to test if not set in page): 
            ' Y=Test. Simulator will accept.
            ' N=Test. Simulator will reject
            ' P=Production Mode (Live)
            If _processingMode = "" Then
                _xmlVoidBuilder.Append("<Mode>Y</Mode>")
            Else
                _xmlVoidBuilder.Append("<Mode>" & _processingMode.Trim & "</Mode>")
            End If
            '
            With _xmlVoidBuilder
                '
                ' Transaction details
                .Append("<Transaction>")
                .Append("<Id>" & HttpUtility.HtmlEncode(transactionId) & "</Id>")
                ''.Append("<Id>37189274891248912-42141h</Id>")
                .Append("<Type>Void</Type>")
                '
                .Append("</Transaction>")
                .Append("</OrderFormDoc>")

                .Append("</EngineDoc>")
                .Append("</EngineDocList>")
            End With
            '
            ' Convert to XML object
            _xmlVoidRequestDoc.LoadXml(_xmlVoidBuilder.ToString)

            If _debug <> "0" Then
                'LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), "CreditCards", _
                '       "XML Void Request:" & Environment.NewLine & _
                '      _xmlVoidRequestDoc.OuterXml)
            End If

            sendVoidXml()

        End Sub
        '****************************************************************
        ' Send the Void XML document to the bank
        '****************************************************************
        Private Sub sendVoidXml()
            Dim result As String = String.Empty
            Dim strPost As String = _xmlVoidRequestDoc.OuterXml.ToString

            Dim myWriter As StreamWriter = Nothing

            Dim objRequest As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)

            objRequest.Method = "POST"
            objRequest.ContentLength = strPost.Length
            objRequest.ContentType = "text/XML"

            Try
                myWriter = New StreamWriter(objRequest.GetRequestStream())
                myWriter.Write(strPost)
            Catch e As Exception
                'LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), _
                '  "HSBCApiRequest.SendVoidXML.vb" & Environment.NewLine & e.Message)
                Exit Sub
            End Try

            myWriter.Close()

            Dim objResponse As HttpWebResponse = CType(objRequest.GetResponse(), HttpWebResponse)

            Using sr As StreamReader = New StreamReader(objResponse.GetResponseStream())
                result = sr.ReadToEnd()

                ' Close and clean up the StreamReader
                sr.Close()
            End Using

            _xmlVoidResponseDoc.LoadXml(result)

            If _debug <> "0" Then
                'LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), "CreditCards", _
                '       "XML Void Response:" & Environment.NewLine & _
                '      _xmlVoidResponseDoc.OuterXml)
            End If

        End Sub
        '********************************************************
        ' DEBUG - Return Entire Xml Void Request as a string
        '********************************************************
        Public Function returnXmlVoidRequestAsString() As String
            Return _xmlVoidRequestDoc.OuterXml.ToString
        End Function
        '********************************************************
        ' DEBUG - Return Entire Xml Void Response as a string
        '********************************************************
        Public Function returnXmlVoidResponseAsString() As String
            Return _xmlVoidResponseDoc.OuterXml.ToString
        End Function
        '***************************************************************
        ' Log credit card errors
        '***************************************************************
        Public Sub logErrors()
            '
            ' Write header
            'LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), "CreditCards", _
            '                    "Begin Credit Card Auth Error:" & _ccNumber & "," & _
            '                    _ccExpiry & "," & _amountPence & "," & _
            '                    returnProcReturnMsg())
            ''
            ' Get messages
            Dim nodeList As XmlNodeList
            Dim messageItem As XmlNodeList
            Dim i, y As Integer
            Dim errorString As String = "     "

            nodeList = _xmlResponseDoc.GetElementsByTagName("Message")
            '
            ' Loop through message nodes and concat each field's data
            For i = 0 To nodeList.Count - 1 Step i + 1
                errorString = "     "
                errorString &= "(" & i.ToString & ")"
                messageItem = nodeList(i).ChildNodes
                For y = 0 To messageItem.Count - 1 Step i + 1
                    errorString &= messageItem(y).InnerText & ","
                Next y
                ' LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), "CreditCards", errorString)
            Next i
            '
            ' Write Trailer
            ' LogWriter.WriteToLog(ConfigurationManager.AppSettings("logRoot"), "CreditCards", "End Credit Card Auth Error")

        End Sub

        Public Function Check3dSecure() As Boolean

            Dim checkout3dSecureError As Boolean = False

            Select Case SecCcpaResultsCode
                ' Success - set fields
                Case Is = "0"
                    payerSecurityLevel = "2"
                    payerAuthenticationCode = SecCAVV
                    payerTxnId = SecXid
                    cardholderPresentCode = "13"

                    ' Not within a participating range
                Case Is = "1"
                    payerSecurityLevel = "5"
                    cardholderPresentCode = "13"

                    ' In participating range but not enrolled
                Case Is = "2"
                    payerSecurityLevel = "1"
                    cardholderPresentCode = "13"

                    ' Not enrolled, but authenticated
                Case Is = "3"
                    payerSecurityLevel = "6"
                    payerAuthenticationCode = SecCAVV
                    payerTxnId = SecXid
                    cardholderPresentCode = "13"

                    ' Enrolled, but a PARes not yet received
                Case Is = "4"
                    'If Resources.checkout3dSecure.failResultCode4 Then
                    '    checkout3dSecureError = True
                    'Else
                    payerSecurityLevel = "4"
                    ' End If

                    ' Failed payer auth
                Case Is = "5", "6"
                    checkout3dSecureError = True

                    ' ACS unable to provide results
                Case Is = "7"
                    payerSecurityLevel = "4"

                    ' Unable to communicate with the server
                Case Is = "8"
                    'If Resources.checkout3dSecure.failResultCode8 Then
                    '    checkout3dSecureError = True
                    'Else
                    payerSecurityLevel = "4"
                    'End If

                    ' Unable to interprete results
                Case Is = "9"
                    ' If Resources.checkout3dSecure.failResultCode9 Then
                    ' checkout3dSecureError = True
                    ' Else
                    payerSecurityLevel = "4"
                    ' End If

                    ' Failed to locate config for merchant
                Case Is = "10"
                    '  If Resources.checkout3dSecure.failResultCode10 Then
                    ' checkout3dSecureError = True
                    ' Else
                    payerSecurityLevel = "10"
                    ' End If

                    ' Failed validation checks
                Case Is = "11"
                    'If Resources.checkout3dSecure.failResultCode11 Then
                    ' checkout3dSecureError = True
                    ' Else
                    payerSecurityLevel = "4"
                    'End If

                    ' Unexpected System error
                Case Is = "12"
                    ' If Resources.checkout3dSecure.failResultCode12 Then
                    ' checkout3dSecureError = True
                    ' Else
                    payerSecurityLevel = "4"
                    ' End If

                    ' Card submitted is not recognised
                Case Is = "14"
                    'If Resources.checkout3dSecure.failResultCode14 Then
                    '  checkout3dSecureError = True
                    '  Else
                    cardholderPresentCode = "13"
                    ' End If

                Case Else
                    checkout3dSecureError = True
            End Select

            LogText = "3D Secure Result - " & SecCcpaResultsCode
            If SecCcpaResultsCode = "5" Then
                LogText &= " (fail)"
            End If

            Return checkout3dSecureError
        End Function


    End Class
End Namespace

