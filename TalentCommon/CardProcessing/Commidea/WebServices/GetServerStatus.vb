'******************************************************************
'   GetServerStatus.vb
'  
'   Modification Summary
'
'   dd/mm/yy    ID      By      Description
'   --------    -----   ---     -----------
'   03/04/06    /000    Ben     Created. Imported from Orion Dev
'******************************************************************
'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated by a tool.
'     Runtime Version: 1.1.4322.2032
'
'     Changes to this file may cause incorrect behavior and will be lost if 
'     the code is regenerated.
' </autogenerated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml.Serialization

'
'This source code was auto-generated by wsdl, Version=1.1.4322.2032.
'
Namespace CardProcessing.Commidea.WebServices

    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.ComponentModel.DesignerCategoryAttribute("code"), _
     System.Web.Services.WebServiceBindingAttribute(Name:="ServerStatusSoap", [Namespace]:="https://www.commidea.webservices.com")> _
    Public Class ServerStatus
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol

        '<remarks/>
        Public Sub New()
            MyBase.New()
            Me.Url = "https://www.commidea.net/serverstatus.asmx"
        End Sub

        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://www.commidea.webservices.com/GetServerStatus", RequestNamespace:="https://www.commidea.webservices.com", ResponseNamespace:="https://www.commidea.webservices.com", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)> _
        Public Function GetServerStatus() As String
            Dim results() As Object = Me.Invoke("GetServerStatus", New Object(-1) {})
            Return CType(results(0), String)
        End Function

        '<remarks/>
        Public Function BeginGetServerStatus(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("GetServerStatus", New Object(-1) {}, callback, asyncState)
        End Function

        '<remarks/>
        Public Function EndGetServerStatus(ByVal asyncResult As System.IAsyncResult) As String
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0), String)
        End Function
    End Class
End Namespace
