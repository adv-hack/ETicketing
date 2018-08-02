Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Talent.TradingPortal

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://localhost/TradingPortal")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class AccessControl
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function DEXRedListRequest(ByVal loginId As String, ByVal password As String, ByVal company As String, ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "DEXRedListRequest"
        Const a2 As String = "DEXRedListResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
    Public Function DEXWhiteListRequest(ByVal loginId As String, ByVal password As String, ByVal company As String, ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "DEXWhiteListRequest"
        Const a2 As String = "DEXWhiteListResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()>
    Public Function ExternalSmartcardReprintRequest(ByVal loginId As String, ByVal password As String, ByVal company As String, ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ExternalSmartcardReprintRequest"
        Const a2 As String = "ExternalSmartcardReprintResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

End Class