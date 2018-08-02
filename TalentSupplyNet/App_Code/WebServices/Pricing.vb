Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Talent.TradingPortal
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Pricing Webservice
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPWSPR- 
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
Public Class Pricing
    Inherits System.Web.Services.WebService
    <WebMethod()> _
    Public Function PNARequest(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "PNARequest"
        Const a2 As String = "PNAResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
    Public Function CompletePNARequest(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "CompletePNARequest"
        Const a2 As String = "CompletePNAResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    '<WebMethod()> _
    'Public Function TicketingPricesRequest(ByVal loginId As String, _
    '                                ByVal password As String, _
    '                                ByVal company As String, _
    '                                ByVal XMLString As String) As String
    '    Dim csgWS As New TalentWebService
    '    Const a1 As String = "TicketingPricesRequest"
    '    Const a2 As String = "TicketingPricesResponse"
    '    '
    '    csgWS.WebServiceName = a1               'Set the Web Service Name
    '    csgWS.ResponseName = a2                 'And Response Name
    '    'Do the work!
    '    Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
    '    Return xmlResponseString
    'End Function
End Class
