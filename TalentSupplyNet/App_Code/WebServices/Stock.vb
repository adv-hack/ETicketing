Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Talent.TradingPortal
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Stock Webservice
'
'       Date                        Dec 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPWSAV- 
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
Public Class Stock
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function AvailabilityRequest(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "AvailabilityRequest"
        Const a2 As String = "AvailabilityResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
        '
    End Function
    <WebMethod()> _
    Public Function MultiAvailabilityRequest(ByVal loginId As String, _
                                  ByVal password As String, _
                                  ByVal company As String, _
                                  ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "MultiAvailabilityRequest"
        Const a2 As String = "MultiAvailabilityResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
        '
    End Function
    <WebMethod()> _
    Public Function ProductAlertRequest(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductAlertRequest"
        Const a2 As String = "ProductAlertResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
        '
    End Function
    <WebMethod()> _
    Public Function ProductAlertOutboundRequest(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "ProductAlertOutboundRequest"
        Const a2 As String = "ProductAlertOutboundResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
        '
    End Function

End Class

