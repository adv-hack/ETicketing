Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Xml
Imports System.Globalization

Partial Class UserControls_Flash
    Inherits ControlBase

    Private _ucr As Talent.Common.UserControlResource = Nothing
    Private _businessUnit As String = Nothing
    Private _partnerCode As String = Nothing
    Private _pageCode As String = Nothing
    Private _languageCode As String = Nothing

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _ucr = New Talent.Common.UserControlResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _businessUnit = TalentCache.GetBusinessUnit
        _pageCode = Talent.eCommerce.Utilities.GetCurrentPageName
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        With _ucr
            .BusinessUnit = _businessUnit
            .PageCode = _pageCode
            .PartnerCode = _partnerCode
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "Flash.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim pagename As String = Talent.eCommerce.Utilities.GetCurrentPageName()
        Dim businessUnit As String = TalentCache.GetBusinessUnit
        pagename = pagename.Remove(pagename.Length - 5, 5)
        pagename = pagename.Split(".")(0)
        Dim movieName As String = pagename ' Movie Name
        Dim movieId As String = pagename ' Movie Id
        Const d As String = """"
        Const dd As String = ChrW(34)
        Dim embed As New StringBuilder
        Dim movieURL As String = "../../Flash/" & pagename & "/" & pagename ' Folder where swf is kept
        Dim movieWidth As String = getAttributeValue("movieWidth") ' Movie Width
        Dim movieHeight As String = getAttributeValue("movieHeight") ' Movie Height
        Dim movieWMode As String = getAttributeValue("movieWMode") ' window mode
        Dim rawURL As String = Request.RawUrl
        Dim baseURL As String = Request.Url.AbsoluteUri
        Dim chk As String = baseURL.Substring(8, baseURL.Length - 8)

        baseURL = baseURL.Substring(0, chk.IndexOf("/") + 8)
        If Request.RawUrl.IndexOf("TalentEBusiness") > -1 Then baseURL += "/TalentEBusiness"

        embed.Append(vbCrLf)
        embed.Append("<script type=").Append(dd).Append("text/javascript").Append(dd).Append(" src=").Append(dd).Append("../../JavaScript/swfIN.js").Append(dd).Append("></script>").Append(vbCrLf)
        embed.Append("<script type=").Append(dd).Append("text/javascript").Append(dd).Append(" src=").Append(dd).Append("../../JavaScript/swfaddress.js").Append(dd).Append("></script>").Append(vbCrLf)
        embed.Append("<script type=").Append(dd).Append("text/javascript").Append(dd).Append(" >").Append(vbCrLf)
        embed.Append("  var s = new swfIN(").Append(dd).Append(movieURL & ".swf").Append(dd).Append(", ").Append(dd).Append("F").Append(dd).Append(", ").Append(dd).Append(movieWidth).Append(dd).Append(", ").Append(dd).Append(movieHeight).Append(dd).Append(");").Append(vbCrLf)
        embed.Append("  s.useSWFAddress();").Append(vbCrLf)
        embed.Append("	s.addParam(").Append(d).Append("allowFullScreen").Append(d).Append(", ").Append(d).Append("true").Append(d).Append(");").Append(vbCrLf)
        embed.Append("	s.addParam(").Append(d).Append("menu").Append(d).Append(", ").Append(d).Append("true").Append(d).Append(");").Append(vbCrLf)
        embed.Append("	s.addParam(").Append(d).Append("wmode").Append(d).Append(", ").Append(d).Append(movieWMode).Append(d).Append(");").Append(vbCrLf)
        embed.Append("  s.addVar(").Append(d).Append("baseURL").Append(d).Append(", ").Append(d).Append(baseURL & "/").Append(d).Append(");").Append(vbCrLf)
        embed.Append("	s.addVar(").Append(d).Append("product").Append(d).Append(", ").Append(d).Append(validateString(Request.QueryString("product"))).Append(d).Append(");").Append(vbCrLf)
        embed.Append("	s.addVar(").Append(d).Append("productMaster").Append(d).Append(", ").Append(d).Append(validateString(Request.QueryString("master"))).Append(d).Append(");").Append(vbCrLf)
        embed.Append("	s.addVar(").Append(d).Append("basketId").Append(d).Append(", ").Append(d).Append(validateString(Request.QueryString("basketId"))).Append(d).Append(");").Append(vbCrLf)
        embed.Append("	s.addVar(").Append(d).Append("transactionId").Append(d).Append(", ").Append(d).Append(validateString(Request.QueryString("transactionId"))).Append(d).Append(");").Append(vbCrLf)
        embed.Append("  s.write();").Append(vbCrLf)
        embed.Append("  function swfFocus() {").Append(vbCrLf)
        embed.Append("       window.document.F.focus();").Append(vbCrLf)
        embed.Append("  }swfFocus();").Append(vbCrLf)
        embed.Append("</script><noscript>Javascript must be enabled to view this page</noscript>")

        lblscript.Text = embed.ToString()
    End Sub

    Private Function getAttributeValue(ByVal attributeName As String) As String
        Dim queryStringParm As String = _ucr.Attribute("QueryStringParameter1")
        If Not String.IsNullOrEmpty(queryStringParm) AndAlso Not String.IsNullOrEmpty(Request(queryStringParm)) Then
            queryStringParm += "=" & Request(queryStringParm)
        Else
            queryStringParm = String.Empty
        End If
        Dim attributeValue As String = String.Empty
        attributeValue = TDataObjects.FlashSettings.TblFlashSettings.GetAttributeByBUPartnerPageCodeAttributeQueryString(_businessUnit, _partnerCode, _pageCode, attributeName, queryStringParm)
        Return attributeValue
    End Function

    Private Function validateString(ByVal inputString As String) As String
        Dim outputString As String = String.Empty
        Try
            If inputString IsNot Nothing Then
                inputString = inputString.Replace("'", "")
                inputString = inputString.Replace("""", "")
                outputString = Server.UrlEncode(inputString.Trim)
            End If
        Catch ex As Exception
            Return String.Empty
        End Try
        Return outputString
    End Function

End Class
