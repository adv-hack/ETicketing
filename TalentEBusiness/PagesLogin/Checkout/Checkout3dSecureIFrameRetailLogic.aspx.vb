Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading
Imports System.Security.Cryptography
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Login Checkout 3d Secure iFrame Retail Logic
'
'       Date                        27/07/09
'
'       Author                      Alex C
'
'       CS Group 2009               All rights reserved.
'
'       Error Number Code base       
'
'       User Controls
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------

Partial Class PagesLogin_Checkout3dSecureIFrameRetailLogic
    Inherits System.Web.UI.Page

    Private _defaults As New Talent.eCommerce.ECommerceModuleDefaults
    Private _def As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private wfr As New Talent.Common.WebFormResource

    Public hdfRedirectUrl As String = String.Empty
    Public formActionUrl As String = ""
    Public requestOk As Boolean = False
    Public hdfRedirectValue As Boolean = False

    Dim paymentRequest As New Talent.Common.CardProcessing.HSBC.HSBCApiRequest
    Dim currentUrl As String = String.Empty
    Dim resultUrl As String = String.Empty
    Dim payment3dSecureDetails1 As String = String.Empty
    Dim payment3dSecureDetails2 As String = String.Empty
    Const doubleQuote As String = """"

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
#End Region

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        'Setup page
        _def = _defaults.GetDefaults
        With wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "Checkout3dSecureIFrameRetailLogic.aspx"
        End With
        If CType(ConfigurationManager.AppSettings("ThemeOverride").ToString, Boolean) Then
            Page.Theme = ConfigurationManager.AppSettings("Theme").ToString
        Else
            Page.Theme = _def.Theme
        End If
        ltlNoJavascriptMessage.Text = wfr.Content("NoJavascriptMessage", _languageCode, True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try
            'Get URLs and find out where we've come from
            currentUrl = Request.Url.ToString

            'Setup form and post to RetailLogic
            resultUrl = currentUrl.Substring(0, currentUrl.IndexOf("PagesLogin")) & _
                                "Redirect/PaymentGateway.aspx?page=checkout3dSecure.aspx&function=checkout3dSecure"
            hdfRedirectUrl = currentUrl.Substring(0, currentUrl.IndexOf("Checkout")) & "Checkout/checkout.aspx"

            If probeUrl() Then
                requestOk = True
            Else
                'Probing URL's Failed
                hdfRedirectValue = True
            End If
        Catch ex As Exception
        End Try

    End Sub

    Function probeUrl() As Boolean
        '--------------------------------------------------------------
        ' Get URL's from db and check sequencially if they are availble
        '--------------------------------------------------------------
        Dim paymentUrl1 As String = String.Empty
        Dim paymentUrl2 As String = String.Empty
        If _def.Payment3DSecureProvider = "COMMIDEA-MULTI-ACCOUNT-ID" Then
            Dim tDataObjects As New Talent.Common.TalentDataObjects
            Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.DestinationDatabase = "SQL2005"
            settings.BusinessUnit = wfr.BusinessUnit
            settings.Partner = wfr.PartnerCode
            tDataObjects.Settings = settings
            tDataObjects.PaymentSettings.TblPaymentDefaults.GetPaymentDefaults(Profile.Basket.PAYMENT_ACCOUNT_ID)
            paymentUrl1 = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentUrl1
            paymentUrl2 = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentUrl2
            payment3dSecureDetails1 = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentDetails1
            payment3dSecureDetails2 = tDataObjects.PaymentSettings.TblPaymentDefaults.PaymentDetails1
        Else
            paymentUrl1 = _def.Payment3DSecureURL1
            paymentUrl2 = _def.Payment3DSecureURL2
            payment3dSecureDetails1 = _def.PaymentDetails1
            payment3dSecureDetails2 = _def.PaymentDetails2
        End If
        Dim req As System.Net.HttpWebRequest
        Dim res As System.Net.HttpWebResponse

        Try
            'request url
            req = Net.WebRequest.Create(paymentUrl1)

            'set the user agent
            'some site might brush you off if it is not set
            'to stop bots and scrapers
            req.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705)"

            'get page
            res = req.GetResponse()
            res.Close()

            If res.ResponseUri.AbsoluteUri = paymentUrl1 Then
                ' URL has responded and response matches paymentUrl1
                formActionUrl = paymentUrl1
                Return True
            Else
                ' URLs do not match - try URL 2
                req = Net.WebRequest.Create(paymentUrl2)

                'get page
                res = req.GetResponse()
                res.Close()

                If res.ResponseUri.AbsoluteUri = paymentUrl2 Then
                    ' URL has responded and response matches paymentUrl2
                    formActionUrl = paymentUrl2
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function


    '----------------------------------------------------------------------
    ' Write out form using normal HTML tags 
    ' (can't use form runat=server as this always posts back to the server)
    '----------------------------------------------------------------------
    Function WriteForm() As String
        Dim returnString As String = ""
        Dim sb As New StringBuilder
        Dim ccDetails As New Generic.Dictionary(Of String, String)
        Dim priceListHeaderTableAdapter As New TalentBasketDatasetTableAdapters.tbl_price_list_headerTableAdapter
        Dim priceListHeaderTable As Data.DataTable = priceListHeaderTableAdapter.Get_PriceList_Header_By_BU_Partner(TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile))
        Dim currenciesTableAdapter As New TalentApplicationVariablesTableAdapters.tbl_currencyTableAdapter
        Dim currenciesTable As Data.DataTable = currenciesTableAdapter.GetDataByCurrencyCode(priceListHeaderTable.Rows(0)("CURRENCY_CODE").ToString)

        ' Form variables
        Dim httpVersion As String = "2.0"
        Dim cardType As String = String.Empty
        Dim pan As String = String.Empty
        Dim expiryDate As String = String.Empty
        Dim deviceCategory As String = "0"
        Dim purchAmount As String = CInt((Profile.Basket.BasketSummary.TotalBasket) * 100).ToString
        Dim exponent As String = "2"
        Dim description As String = "Tickets"
        Dim currency As String = String.Empty
        Dim xid As String = String.Empty
        Dim merchantId As String = payment3dSecureDetails1
        Dim md As String = Profile.Basket.TempOrderID.Trim
        Dim sharedSecret As String = payment3dSecureDetails1

        ' Get variables from the session and decode
        Try
            pan = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)
            expiryDate = Checkout.RetrievePaymentItemFromSession("CardNumber", GlobalConstants.CHECKOUTASPXSTAGE)

            ' format expiry date to YYMM
            If expiryDate.Length = 5 Then
                expiryDate = "0" & expiryDate
            End If
            expiryDate = expiryDate.Substring(4, 2) & expiryDate.Substring(0, 2)
        Catch ex As Exception
            'error grabbing the cc details from session
            hdfRedirectValue = True
        End Try

        ' Get the currency code
        If currenciesTable.Rows.Count > 0 AndAlso Utilities.CheckForDBNull_String(currenciesTable.Rows(0)("CURRENCY_CODE_1").ToString) <> String.Empty Then
            currency = currenciesTable.Rows(0)("CURRENCY_CODE_1").ToString
        Else
            currency = "826"
        End If

        '-----------------------------------------------------------------------------------
        ' Take the Basket ID and Timestamp as xid put into a 20 byte array encoded to BASE64
        '-----------------------------------------------------------------------------------
        Dim xidByte(19) As Byte
        Dim randomise As New Random
        randomise.NextBytes(xidByte)
        xid = Convert.ToBase64String(xidByte)

        '----------------------------------
        ' Build digest and encrypt to SHA-1
        '----------------------------------
        Dim digest As New StringBuilder
        With digest
            .Append(httpVersion)
            .Append(cardType)
            .Append(pan)
            .Append(expiryDate)
            .Append(deviceCategory)
            .Append(purchAmount)
            .Append(exponent)
            .Append(description)
            .Append(currency)
            .Append(merchantId)
            .Append(xid)
            .Append(resultUrl)
            .Append(resultUrl)
            .Append(md)
            .Append(sharedSecret)
        End With

        Dim encoder As New System.Text.UTF8Encoding
        Dim digestBytes() As Byte = encoder.GetBytes(digest.ToString)
        Dim SHA1Hasher As New SHA1CryptoServiceProvider
        digestBytes = SHA1Hasher.ComputeHash(digestBytes)
        Dim digestBase64 As String = Convert.ToBase64String(digestBytes)

        With sb
            .Append("<table>")
            .Append(vbCrLf)

            .Append("<tr><td>HTTP Version:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("version")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(httpVersion)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("<tr><td>Card Type:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("cardType")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(cardType)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("<tr><td>Cardholder PAN:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("pan")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(pan)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("<tr><td>Card Expiration:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("expiry")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(expiryDate)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("<tr><td>Device Category:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("deviceCategory")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(deviceCategory)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("<tr><td>Purchase Ammount:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("purchAmount")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(purchAmount)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("<tr><td>Currency Exponent:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("exponent")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(exponent)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("<tr><td>Brief Description:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("description")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(description)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("<tr><td>Currency:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("currency")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(currency)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("<tr><td>Merchant ID</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("merchantID")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(merchantId)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("<tr><td>XID</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("xid")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(xid)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("<tr><td>Success URL:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("okUrl")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(resultUrl)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("<tr><td>Fail URL:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("failUrl")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(resultUrl)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("<tr><td>Merchant Data:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("MD")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(md)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("<tr><td>Digest:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("digest")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(digestBase64)
            .Append(doubleQuote)
            .Append("/></td></tr>")
            .Append(vbCrLf)

            .Append("</table>")

        End With

        returnString = sb.ToString
        Return returnString
    End Function

End Class