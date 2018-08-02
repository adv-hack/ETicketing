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

Partial Class PagesLogin_Checkout3dSecureIFrameWinbank
    Inherits Base01

    Private _defaults As New Talent.eCommerce.ECommerceModuleDefaults
    Private _def As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Dim paymentRequest As New Talent.Common.CardProcessing.HSBC.HSBCApiRequest
    Dim currentUrl As String = String.Empty
    Dim resultUrl As String = String.Empty
    Const doubleQuote As String = """"
    Public hdfRedirectUrl As String = String.Empty
    Public formActionUrl As String = ""
    Public requestOk As Boolean = False
    Public hdfRedirectValue As Boolean = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        resultUrl = Session("TermUrl") ' set in checkout3dSecure.aspx
        currentUrl = Request.Url.ToString
        hdfRedirectUrl = currentUrl.Substring(0, currentUrl.IndexOf("Checkout")) & "Checkout/checkout.aspx"

        If probeUrl() Then
            requestOk = True
        Else
            'Probing URL's Failed
            hdfRedirectValue = True
        End If
    End Sub

    Function probeUrl() As Boolean
        '--------------------------------------------------------------
        ' Get URL's from db and check sequencially if they are availble
        '--------------------------------------------------------------
        _def = _defaults.GetDefaults
        Dim paymentUrl1 As String = Session("AcsUrl")

        Session("AcsUrl") = ""

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
                Return False
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

        '' Form variables
        Dim httpVersion As String = "2.0"

        '-----------------
        ' Build Result URL
        '-----------------
        Dim currentUrl As String = Request.Url.ToString
        resultUrl = currentUrl.Substring(0, currentUrl.IndexOf("PagesLogin")) & _
            "Redirect/PaymentGateway.aspx?page=checkout3dSecure.aspx&function=checkout3dSecure"


        With sb
            .Append("<input type='text' id='PaReq' name='PaReq' readonly='readonly' value='" & Session("PaReq") & "' />")
            .Append("<input type='text' id='TermUrl' name='TermUrl' readonly='readonly' value='" & Session("TermUrl") & "' />")
            .Append("<input type='text' id='MD' name='MD' readonly='readonly' value='" & Session("MD") & "' />")
        End With

        returnString = sb.ToString
        Return returnString
    End Function

End Class

