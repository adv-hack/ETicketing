Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Talent.TradingPortal
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Customer Webservice
'
'       Date                        March 2007
'
'       Author                      Des
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPWSCC- 
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
Public Class Customer
    Inherits System.Web.Services.WebService
    <WebMethod()> _
        Public Function CustomerAddRequest(ByVal loginId As String, _
                                        ByVal password As String, _
                                        ByVal company As String, _
                                        ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "CustomerAddRequest"
        Const a2 As String = "CustomerAddResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
    Public Function CustomerRetrievalRequest(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "CustomerRetrievalRequest"
        Const a2 As String = "CustomerRetrievalResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
    Public Function CustomerAssociationsRequest(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "CustomerAssociationsRequest"
        Const a2 As String = "CustomerAssociationsResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
   Public Function VerifyPasswordRequest(ByVal loginId As String, _
                                   ByVal password As String, _
                                   ByVal company As String, _
                                   ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "VerifyPasswordRequest"
        Const a2 As String = "VerifyPasswordResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
   Public Function RetrievePasswordRequest(ByVal loginId As String, _
                                   ByVal password As String, _
                                   ByVal company As String, _
                                   ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "RetrievePasswordRequest"
        Const a2 As String = "RetrievePasswordResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
          Public Function CustomerUpdateRequest(ByVal loginId As String, _
                                          ByVal password As String, _
                                          ByVal company As String, _
                                          ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "CustomerUpdateRequest"
        Const a2 As String = "CustomerUpdateResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
              Public Function AddCustomerAssociationsRequest(ByVal loginId As String, _
                                              ByVal password As String, _
                                              ByVal company As String, _
                                              ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "AddCustomerAssociationsRequest"
        Const a2 As String = "AddCustomerAssociationsResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
          Public Function DeleteCustomerAssociationsRequest(ByVal loginId As String, _
                                          ByVal password As String, _
                                          ByVal company As String, _
                                          ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "DeleteCustomerAssociationsRequest"
        Const a2 As String = "DeleteCustomerAssociationsResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

    <WebMethod()> _
          Public Function RetrieveProfileDetailsRequest(ByVal loginId As String, _
                                          ByVal password As String, _
                                          ByVal company As String, _
                                          ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "RetrieveProfileDetailsRequest"
        Const a2 As String = "RetrieveProfileDetailsResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
 Public Function GeneratePasswordRequest(ByVal loginId As String, _
                                 ByVal password As String, _
                                 ByVal company As String, _
                                 ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "GeneratePasswordRequest"
        Const a2 As String = "GeneratePasswordResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
    Public Function CustomerSearchRequest(ByVal loginId As String, _
                                 ByVal password As String, _
                                 ByVal company As String, _
                                 ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "CustomerSearchRequest"
        Const a2 As String = "CustomerSearchResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        '
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
End Class