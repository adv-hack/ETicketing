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

Partial Class PagesLogin_Checkout3dSecureIFrameCommidea
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

    Dim talentLogging As New Talent.Common.TalentLogging

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim referrerUrl As String = String.Empty
        Try
            referrerUrl = Request.UrlReferrer.AbsoluteUri
        Catch ex As Exception
            referrerUrl = ex.Message
        End Try

        talentLogging.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load", "Customer: " & Profile.User.Details.LoginID & ", Referrer URL: " & referrerUrl, "3dSecureLogging")
        resultUrl = Session("TermUrl") ' set in checkout3dSecure.aspx
        currentUrl = Request.Url.ToString
        hdfRedirectUrl = currentUrl.Substring(0, currentUrl.IndexOf("Checkout")) & "Checkout/Checkout.aspx"

        If Session("AcsUrl") IsNot Nothing Then
            talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load - Valid ACSUrl", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")
            formActionUrl = Session("AcsUrl")
            Session("AcsUrl") = ""
            requestOk = True
        Else
            talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load - InValid ACSUrl", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")
            hdfRedirectValue = True
        End If
    End Sub

    '----------------------------------------------------------------------
    ' Write out form using normal HTML tags 
    ' (can't use form runat=server as this always posts back to the server)
    '----------------------------------------------------------------------
    Function WriteForm() As String
        talentLogging.GeneralLog(ProfileHelper.GetPageName, "WriteForm", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")
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

