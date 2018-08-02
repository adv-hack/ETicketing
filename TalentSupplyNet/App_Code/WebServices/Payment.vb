Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Talent.TradingPortal
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Product Webservice
'
'       Date                        May 2007
'
'       Author                      Des
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPWSPD- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<WebService(Namespace:="http://localhost/TradingPortal")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class Payment
    Inherits System.Web.Services.WebService
    <WebMethod()> _
        Public Function PaymentRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "PaymentRequest"
        Const a2 As String = "PaymentResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
        Public Function RefundPaymentRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "RefundPaymentRequest"
        Const a2 As String = "RefundPaymentResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
End Class