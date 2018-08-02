Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Talent.TradingPortal
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Order Webservice
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPWSOR- 
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
Public Class Order
    Inherits System.Web.Services.WebService
    <WebMethod()> _
    Public Function DispatchAdvice(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "DispatchAdviceRequest"
        Const a2 As String = "DispatchAdviceResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        '--------------------------------------------------------------------------------------
        '   At this point we have a pure XML formated string, which is passed back to ASP.Net
        '   which then formats the page, ands adds the HTML bits, including the viewstate
        '
        Return xmlResponseString
    End Function
    <WebMethod()> _
   Public Function DTypeOrderRequest(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "DTypeOrderRequest"
        Const a2 As String = "DTypeOrderResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        ' 
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
    Public Function OrderChangeRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "OrderChangeRequest"
        Const a2 As String = "OrderChangeResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
   Public Function OrderDetailRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "OrderDetailRequest"
        Const a2 As String = "OrderDetailResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
        Public Function OrderRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "OrderRequest"
        Const a2 As String = "OrderResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
    Public Function OrderStatusRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "OrderStatusRequest"
        Const a2 As String = "OrderStatusResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
     Public Function OrderTrackingRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "OrderTrackingRequest"
        Const a2 As String = "OrderTrackingResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
    Public Function OrderDetailTicketingRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "OrderDetailTicketingRequest"
        Const a2 As String = "OrderDetailTicketingResponse"
        '    
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
    Public Function RetrievePurchaseHistoryRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "RetrievePurchaseHistoryRequest"
        Const a2 As String = "RetrievePurchaseHistoryResponse"
        '    
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
    Public Function RetrieveOrdersByStatusRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "RetrieveOrdersByStatusRequest"
        Const a2 As String = "RetrieveOrdersByStatusResponse"
        '    
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
    Public Function AmendTicketingOrderRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "AmendTicketingOrderRequest"
        Const a2 As String = "AmendTicketingOrderRequest"
        '    
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
    Public Function RetrieveOrderByOrderIdRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "RetrieveOrderByOrderIdRequest"
        Const a2 As String = "RetrieveOrderByOrderIdResponse"
        '    
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
    Public Function AddOrderTrackingRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String


        Dim csgWS As New TalentWebService
        Const a1 As String = "AddOrderTrackingRequest"  'Must match the entry in the Frontend DB defaults tables.
        Const a2 As String = "AddOrderTrackingResponse"
        '    
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)

        Return xmlResponseString

    End Function
End Class