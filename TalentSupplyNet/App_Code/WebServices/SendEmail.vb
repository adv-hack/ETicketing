Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Talent.TradingPortal

<WebService(Namespace:="http://localhost/TradingPortal")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class SendEmail
    Inherits System.Web.Services.WebService

    <WebMethod()> _
         Public Function Send(ByVal loginId As String, _
                                         ByVal password As String, _
                                         ByVal company As String, _
                                         ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "SendEmailRequest"
        Const a2 As String = "SendEmailResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

End Class
