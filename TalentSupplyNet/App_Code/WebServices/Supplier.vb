Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Talent.TradingPortal
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Supplier Invoicing Webservice
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPWSSI- 
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
Public Class Supplier
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function InvoiceRequest(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "SupplierInvoiceRequest"
        Const a2 As String = "SupplierInvoiceResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '   Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
   Public Function InvoiceStatusRequest(ByVal loginId As String, _
                                   ByVal password As String, _
                                   ByVal company As String, _
                                   ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "SupplierInvoiceAcceptedRequest"
        Const a2 As String = "SupplierInvoiceAcceptedResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '   Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
     Public Function OrderAcknowledgementRequest(ByVal loginId As String, _
                                     ByVal password As String, _
                                     ByVal company As String, _
                                     ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "SupplierOrderAcknowledgementRequest"
        Const a2 As String = "SupplierOrderAcknowledgementResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '   Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
End Class
