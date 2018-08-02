Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Pages Login Checkout 3d Secure iFrame HASBC
'
'       Date                        15/08/07
'
'       Author                      Ben
'
'       � CS Group 2007             All rights reserved.
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

Partial Class PagesLogin_checkout3dSecureiFrameHSBC
    Inherits Base01

    Dim paymentRequest As New Talent.Common.CardProcessing.HSBC.HSBCApiRequest
    Const doubleQuote As String = """"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        Session("CardError") = Nothing

      
        '
        ' Check if there's an HSBC payment request created to process
        If Session("HSBCRequest") Is Nothing Then
            Response.Redirect("checkout.aspx")
        End If

        paymentRequest = CType(Session("HSBCRequest"), Talent.Common.CardProcessing.HSBC.HSBCApiRequest)


    End Sub

    '----------------------------------------------------------------------
    ' Write out form using normal HTML tags 
    ' (can't use form runat=server as this always posts back to the server)
    '----------------------------------------------------------------------
    Function WriteForm() As String
        Dim returnString As String = ""
        Dim sb As New StringBuilder
        Dim expiryDate As String = ""
        Dim expiryDateYYMM As String = ""
        paymentRequest = CType(Session("HSBCRequest"), Talent.Common.CardProcessing.HSBC.HSBCApiRequest)

        ' Format expiry to YYMM
        expiryDate = paymentRequest.ccExpiry
        expiryDateYYMM = expiryDate.Substring(3, 2) & expiryDate.Substring(0, 2)
        '-----------------
        ' Build Result URL
        '-----------------
        Dim currentUrl As String = Request.Url.ToString

        Dim resultURL As String = currentUrl.Substring(0, currentUrl.IndexOf("checkout3dSecureIframe")) & "CheckoutOrderConfirmation.aspx"

        With sb
            .Append("<table>")
            .Append("<tr><td>Card Expiration:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("CardExpiration")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(expiryDateYYMM)
            .Append(doubleQuote)
            .Append("/></td></tr>")

            .Append("<tr><td>Cardholder PAN:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("CardholderPan")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(paymentRequest.ccNumber)
            .Append(doubleQuote)
            .Append("/></td></tr>")

            .Append("<tr><td>Ccpa Client Id:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("CcpaClientId")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(paymentRequest.clientAlias & "01")
            .Append(doubleQuote)
            .Append("/></td></tr>")

            .Append("<tr><td>Currency Exponent:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("CurrencyExponent")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append("2")
            .Append(doubleQuote)
            .Append("/></td></tr>")

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
            .Append(Profile.Basket.TempOrderID)
            .Append(" ")
            .Append(doubleQuote)
            .Append("/></td></tr>")

            .Append("<tr><td>Purchase Amount:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("PurchaseAmount")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(Chr(163) & ((paymentRequest.amountPence / 100).ToString))
            .Append(doubleQuote)
            .Append("/></td></tr>")

            .Append("<tr><td>Purchase Amount Raw:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("PurchaseAmountRaw")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(paymentRequest.amountPence)
            .Append(doubleQuote)
            .Append("/></td></tr>")

            .Append("<tr><td>Purchase Currency:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("PurchaseCurrency")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append("826")
            .Append(doubleQuote)
            .Append("/></td></tr>")

            .Append("<tr><td>Purchase Description:</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("PurchaseDesc")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append("Alfred Dunhill Store Purchase")
            .Append(doubleQuote)
            .Append("/></td></tr>")

            .Append("<tr><td>ResultUrl</td><td><input type=")
            .Append(doubleQuote)
            .Append("text")
            .Append(doubleQuote)
            .Append(" name=")
            .Append(doubleQuote)
            .Append("ResultUrl")
            .Append(doubleQuote)
            .Append(" readonly=")
            .Append(doubleQuote)
            .Append("readonly")
            .Append(doubleQuote)
            .Append(" value=")
            .Append(doubleQuote)
            .Append(resultURL)

            .Append(doubleQuote)
            .Append("/></td></tr>")

            .Append("</table>")


        End With

        returnString = sb.ToString
        Return returnString
    End Function

End Class

