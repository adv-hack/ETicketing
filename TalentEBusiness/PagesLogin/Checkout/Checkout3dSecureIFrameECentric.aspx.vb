Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports System.Data
Imports System.Data.SqlClient
Imports System.Threading
Imports System.Security.Cryptography

Partial Class PagesLogin_Checkout3dSecureIFrameECentric
    Inherits Base01

#Region "Class Level Fields"

    Private _defaults As New Talent.eCommerce.ECommerceModuleDefaults
    Private _def As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Private _currentUrl As String = String.Empty
    Private _resultUrl As String = String.Empty
    Private _talentLogging As New Talent.Common.TalentLogging

#End Region

#Region "Public Properties"

    Public hdfRedirectUrl As String = String.Empty
    Public formActionUrl As String = ""
    Public requestOk As Boolean = False
    Public hdfRedirectValue As Boolean = False

#End Region

#Region "Constants"

    Const DOUBLEQUOTE As String = """"

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
        _resultUrl = Session("TermUrl") ' set in checkout3dSecure.aspx
        _currentUrl = Request.Url.ToString
        hdfRedirectUrl = _currentUrl.Substring(0, _currentUrl.IndexOf("Checkout")) & "Checkout/Checkout.aspx"

        If Session("AcsUrl") IsNot Nothing Then
            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load - Valid ACSUrl", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")
            formActionUrl = Session("AcsUrl")
            Session("AcsUrl") = ""
            requestOk = True
        Else
            _talentLogging.GeneralLog(ProfileHelper.GetPageName, "Page_Load - InValid ACSUrl", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")
            hdfRedirectValue = True
        End If
    End Sub

#End Region

#Region "Public Functions"

    Public Function WriteForm() As String
        _talentLogging.GeneralLog(ProfileHelper.GetPageName, "WriteForm", "Customer: " & Profile.User.Details.LoginID, "3dSecureLogging")
        Dim returnString As String = ""
        Dim sb As New StringBuilder
        Dim httpVersion As String = "2.0"
        Dim currentUrl As String = Request.Url.ToString
        _resultUrl = currentUrl.Substring(0, currentUrl.IndexOf("PagesLogin")) & "Redirect/PaymentGateway.aspx?page=checkout3dSecure.aspx&function=checkout3dSecure"

        With sb
            .Append("<input type='text' id='PaReq' name='PaReq' readonly='readonly' value='" & Session("PARes") & "' />")
            .Append("<input type='text' id='TermUrl' name='TermUrl' readonly='readonly' value='" & Session("TermUrl") & "' />")
            .Append("<input type='text' id='MD' name='MD' readonly='readonly' value='" & Session("MD") & "' />")
        End With

        returnString = sb.ToString
        Return returnString
    End Function

#End Region

End Class