Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Talent.TradingPortal
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    OrderTemplates Template Webservice
'
'       Date                        30/07/07
'
'       Author                      Ben Ford
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPBASK- 
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
Public Class OrderTemplates
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function AmendTemplateRequest(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "AmendTemplateRequest"
        Const a2 As String = "AmendTemplateResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function
    <WebMethod()> _
    Public Function UploadTemplates(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "UploadTemplatesRequest"
        Const a2 As String = "UploadTemplatesResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2                 'And Response Name
        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
    End Function

End Class
