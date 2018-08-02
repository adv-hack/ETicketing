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
Public Class Product
    Inherits System.Web.Services.WebService
    <WebMethod()> _
        Public Function ProductListRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductListRequest"
        Const a2 As String = "ProductListResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
        Public Function ProductStadiumAvailabilityRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductStadiumAvailabilityRequest"
        Const a2 As String = "ProductStadiumAvailabilityResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
        Public Function ProductSeatAvailabilityRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductSeatAvailabilityRequest"
        Const a2 As String = "ProductSeatAvailabilityResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
            Public Function ProductSeatNumbersRequest(ByVal loginId As String, _
                                            ByVal password As String, _
                                            ByVal company As String, _
                                            ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductSeatNumbersRequest"
        Const a2 As String = "ProductSeatNumbersResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
        Public Function ProductDetailsRequest(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductDetailsRequest"
        Const a2 As String = "ProductDetailsResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function


    <WebMethod()> _
        Public Function ProductPricingDetailsRequest(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductPricingDetailsRequest"
        Const a2 As String = "ProductPricingDetailsResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
      Public Function RetrieveEligibleTicketingCustomersRequest(ByVal loginId As String, _
                                  ByVal password As String, _
                                  ByVal company As String, _
                                  ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "RetrieveEligibleTicketingCustomersRequest"
        Const a2 As String = "RetrieveEligibleTicketingCustomersResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
      Public Function ProductLoadRequest(ByVal loginId As String, _
                                      ByVal password As String, _
                                      ByVal company As String, _
                                      ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductLoadRequest"
        Const a2 As String = "ProductLoadResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2

        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
        '
    End Function
    <WebMethod()> _
         Public Function ProductNavigationLoadRequest(ByVal loginId As String, _
                                         ByVal password As String, _
                                         ByVal company As String, _
                                         ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductNavigationLoadRequest"
        Const a2 As String = "ProductNavigationLoadResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2

        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
        '
    End Function

    <WebMethod()> _
    Public Function ProductOptionsLoadRequest(ByVal loginId As String, _
                                             ByVal password As String, _
                                             ByVal company As String, _
                                             ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductOptionsLoadRequest"
        Const a2 As String = "ProductOptionsLoadResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2

        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
        '
    End Function

    <WebMethod()> _
     Public Function ProductRelationsLoadRequest(ByVal loginId As String, _
                                          ByVal password As String, _
                                          ByVal company As String, _
                                          ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductRelationsLoadRequest"
        Const a2 As String = "ProductRelationsLoadResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2

        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
        '
    End Function

    <WebMethod()> _
            Public Function ProductListReturnAllRequest(ByVal loginId As String, _
                                            ByVal password As String, _
                                            ByVal company As String, _
                                            ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductListReturnAllRequest"
        Const a2 As String = "ProductListReturnAllResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
        Public Function ProductStockLoadRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductStockLoadRequest"
        Const a2 As String = "ProductStockLoadResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
            Public Function ProductPriceLoadRequest(ByVal loginId As String, _
                                            ByVal password As String, _
                                            ByVal company As String, _
                                            ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductPriceLoadRequest"
        Const a2 As String = "ProductPriceLoadResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function


End Class