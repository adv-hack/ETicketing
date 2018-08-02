'------------------------------------------------------------------------------
' <autogenerated>
'     This code was generated by a tool.
'     Runtime Version: 1.0.3705.288
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
'This source code was auto-generated by wsdl, Version=1.0.3705.288.
'
Namespace com.qas.proweb.soap
    
    '<remarks/>
    <System.Diagnostics.DebuggerStepThroughAttribute(),  _
     System.ComponentModel.DesignerCategoryAttribute("code"),  _
     System.Web.Services.WebServiceBindingAttribute(Name:="QASoapBinding", [Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class ProWeb
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
        
        '<remarks/>
        Public Sub New()
            MyBase.New
            Me.Url = "http://localhost:2021/"
        End Sub
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.qas.com/web-2005-02/DoSearch", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Bare)>  _
        Public Function DoSearch(<System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.qas.com/web-2005-02")> ByVal QASearch As QASearch) As <System.Xml.Serialization.XmlElementAttribute("QASearchResult", [Namespace]:="http://www.qas.com/web-2005-02")> QASearchResult
            Dim results() As Object = Me.Invoke("DoSearch", New Object() {QASearch})
            Return CType(results(0),QASearchResult)
        End Function
        
        '<remarks/>
        Public Function BeginDoSearch(ByVal QASearch As QASearch, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DoSearch", New Object() {QASearch}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndDoSearch(ByVal asyncResult As System.IAsyncResult) As QASearchResult
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),QASearchResult)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.qas.com/web-2005-02/DoRefine", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Bare)>  _
        Public Function DoRefine(<System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.qas.com/web-2005-02")> ByVal QARefine As QARefine) As <System.Xml.Serialization.XmlElementAttribute("Picklist", [Namespace]:="http://www.qas.com/web-2005-02")> Picklist
            Dim results() As Object = Me.Invoke("DoRefine", New Object() {QARefine})
            Return CType(results(0),Picklist)
        End Function
        
        '<remarks/>
        Public Function BeginDoRefine(ByVal QARefine As QARefine, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DoRefine", New Object() {QARefine}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndDoRefine(ByVal asyncResult As System.IAsyncResult) As Picklist
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Picklist)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.qas.com/web-2005-02/DoGetAddress", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Bare)>  _
        Public Function DoGetAddress(<System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.qas.com/web-2005-02")> ByVal QAGetAddress As QAGetAddress) As <System.Xml.Serialization.XmlElementAttribute("Address", [Namespace]:="http://www.qas.com/web-2005-02")> Address
            Dim results() As Object = Me.Invoke("DoGetAddress", New Object() {QAGetAddress})
            Return CType(results(0),Address)
        End Function
        
        '<remarks/>
        Public Function BeginDoGetAddress(ByVal QAGetAddress As QAGetAddress, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DoGetAddress", New Object() {QAGetAddress}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndDoGetAddress(ByVal asyncResult As System.IAsyncResult) As Address
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),Address)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.qas.com/web-2005-02/DoGetData", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Bare)>  _
        Public Function DoGetData() As <System.Xml.Serialization.XmlArrayAttribute("QAData", [Namespace]:="http://www.qas.com/web-2005-02"), System.Xml.Serialization.XmlArrayItemAttribute("DataSet", [Namespace]:="http://www.qas.com/web-2005-02", IsNullable:=false)> QADataSet()
            Dim results() As Object = Me.Invoke("DoGetData", New Object(-1) {})
            Return CType(results(0),QADataSet())
        End Function
        
        '<remarks/>
        Public Function BeginDoGetData(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DoGetData", New Object(-1) {}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndDoGetData(ByVal asyncResult As System.IAsyncResult) As QADataSet()
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),QADataSet())
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.qas.com/web-2005-02/DoGetLicenseInfo", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Bare)>  _
        Public Function DoGetLicenseInfo() As <System.Xml.Serialization.XmlElementAttribute("QALicenceInfo", [Namespace]:="http://www.qas.com/web-2005-02")> QALicenceInfo
            Dim results() As Object = Me.Invoke("DoGetLicenseInfo", New Object(-1) {})
            Return CType(results(0),QALicenceInfo)
        End Function
        
        '<remarks/>
        Public Function BeginDoGetLicenseInfo(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DoGetLicenseInfo", New Object(-1) {}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndDoGetLicenseInfo(ByVal asyncResult As System.IAsyncResult) As QALicenceInfo
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),QALicenceInfo)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.qas.com/web-2005-02/DoGetSystemInfo", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Bare)>  _
        Public Function DoGetSystemInfo() As <System.Xml.Serialization.XmlArrayAttribute("QASystemInfo", [Namespace]:="http://www.qas.com/web-2005-02"), System.Xml.Serialization.XmlArrayItemAttribute("SystemInfo", [Namespace]:="http://www.qas.com/web-2005-02", IsNullable:=false)> String()
            Dim results() As Object = Me.Invoke("DoGetSystemInfo", New Object(-1) {})
            Return CType(results(0),String())
        End Function
        
        '<remarks/>
        Public Function BeginDoGetSystemInfo(ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DoGetSystemInfo", New Object(-1) {}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndDoGetSystemInfo(ByVal asyncResult As System.IAsyncResult) As String()
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),String())
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.qas.com/web-2005-02/DoGetExampleAddresses", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Bare)>  _
        Public Function DoGetExampleAddresses(<System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.qas.com/web-2005-02")> ByVal QAGetExampleAddresses As QAGetExampleAddresses) As <System.Xml.Serialization.XmlArrayAttribute("QAExampleAddresses", [Namespace]:="http://www.qas.com/web-2005-02"), System.Xml.Serialization.XmlArrayItemAttribute("ExampleAddress", [Namespace]:="http://www.qas.com/web-2005-02", IsNullable:=false)> QAExampleAddress()
            Dim results() As Object = Me.Invoke("DoGetExampleAddresses", New Object() {QAGetExampleAddresses})
            Return CType(results(0),QAExampleAddress())
        End Function
        
        '<remarks/>
        Public Function BeginDoGetExampleAddresses(ByVal QAGetExampleAddresses As QAGetExampleAddresses, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DoGetExampleAddresses", New Object() {QAGetExampleAddresses}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndDoGetExampleAddresses(ByVal asyncResult As System.IAsyncResult) As QAExampleAddress()
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),QAExampleAddress())
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.qas.com/web-2005-02/DoGetLayouts", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Bare)>  _
        Public Function DoGetLayouts(<System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.qas.com/web-2005-02")> ByVal QAGetLayouts As QAGetLayouts) As <System.Xml.Serialization.XmlArrayAttribute("QALayouts", [Namespace]:="http://www.qas.com/web-2005-02"), System.Xml.Serialization.XmlArrayItemAttribute("Layout", [Namespace]:="http://www.qas.com/web-2005-02", IsNullable:=false)> QALayout()
            Dim results() As Object = Me.Invoke("DoGetLayouts", New Object() {QAGetLayouts})
            Return CType(results(0),QALayout())
        End Function
        
        '<remarks/>
        Public Function BeginDoGetLayouts(ByVal QAGetLayouts As QAGetLayouts, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DoGetLayouts", New Object() {QAGetLayouts}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndDoGetLayouts(ByVal asyncResult As System.IAsyncResult) As QALayout()
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),QALayout())
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.qas.com/web-2005-02/DoGetPromptSet", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Bare)>  _
        Public Function DoGetPromptSet(<System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.qas.com/web-2005-02")> ByVal QAGetPromptSet As QAGetPromptSet) As <System.Xml.Serialization.XmlElementAttribute("QAPromptSet", [Namespace]:="http://www.qas.com/web-2005-02")> QAPromptSet
            Dim results() As Object = Me.Invoke("DoGetPromptSet", New Object() {QAGetPromptSet})
            Return CType(results(0),QAPromptSet)
        End Function
        
        '<remarks/>
        Public Function BeginDoGetPromptSet(ByVal QAGetPromptSet As QAGetPromptSet, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DoGetPromptSet", New Object() {QAGetPromptSet}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndDoGetPromptSet(ByVal asyncResult As System.IAsyncResult) As QAPromptSet
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),QAPromptSet)
        End Function
        
        '<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.qas.com/web-2005-02/DoCanSearch", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Bare)>  _
        Public Function DoCanSearch(<System.Xml.Serialization.XmlElementAttribute([Namespace]:="http://www.qas.com/web-2005-02")> ByVal QACanSearch As QACanSearch) As <System.Xml.Serialization.XmlElementAttribute("QASearchOk", [Namespace]:="http://www.qas.com/web-2005-02")> QASearchOk
            Dim results() As Object = Me.Invoke("DoCanSearch", New Object() {QACanSearch})
            Return CType(results(0),QASearchOk)
        End Function
        
        '<remarks/>
        Public Function BeginDoCanSearch(ByVal QACanSearch As QACanSearch, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("DoCanSearch", New Object() {QACanSearch}, callback, asyncState)
        End Function
        
        '<remarks/>
        Public Function EndDoCanSearch(ByVal asyncResult As System.IAsyncResult) As QASearchOk
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0),QASearchOk)
        End Function
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QASearch
        
        '<remarks/>
        Public Country As String
        
        '<remarks/>
        Public Engine As EngineType
        
        '<remarks/>
        Public Layout As String
        
        '<remarks/>
        Public QAConfig As QAConfigType
        
        '<remarks/>
        Public Search As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class EngineType
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute()>  _
        Public Flatten As Boolean
        
        '<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public FlattenSpecified As Boolean
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute()>  _
        Public Intensity As EngineIntensityType
        
        '<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public IntensitySpecified As Boolean
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute()>  _
        Public PromptSet As PromptSetType
        
        '<remarks/>
        <System.Xml.Serialization.XmlIgnoreAttribute()>  _
        Public PromptSetSpecified As Boolean
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="positiveInteger")>  _
        Public Threshold As String
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="nonNegativeInteger")>  _
        Public Timeout As String
        
        '<remarks/>
        <System.Xml.Serialization.XmlTextAttribute()>  _
        Public Value As EngineEnumType
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Enum EngineIntensityType
        
        '<remarks/>
        Exact
        
        '<remarks/>
        Close
        
        '<remarks/>
        Extensive
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Enum PromptSetType
        
        '<remarks/>
        OneLine
        
        '<remarks/>
        [Default]
        
        '<remarks/>
        Generic
        
        '<remarks/>
        Optimal
        
        '<remarks/>
        Alternate
        
        '<remarks/>
        Alternate2
        
        '<remarks/>
        Alternate3
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Enum EngineEnumType
        
        '<remarks/>
        Singleline
        
        '<remarks/>
        Typedown
        
        '<remarks/>
        Verification
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QASearchOk
        
        '<remarks/>
        Public IsOk As Boolean
        
        '<remarks/>
        Public ErrorCode As String
        
        '<remarks/>
        Public ErrorMessage As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QACanSearch
        
        '<remarks/>
        Public Country As String
        
        '<remarks/>
        Public Engine As EngineType
        
        '<remarks/>
        Public Layout As String
        
        '<remarks/>
        Public QAConfig As QAConfigType
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QAConfigType
        
        '<remarks/>
        Public IniFile As String
        
        '<remarks/>
        Public IniSection As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class PromptLine
        
        '<remarks/>
        Public Prompt As String
        
        '<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(DataType:="nonNegativeInteger")>  _
        Public SuggestedInputLength As String
        
        '<remarks/>
        Public Example As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QAPromptSet
        
        '<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("Line")>  _
        Public Line() As PromptLine
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public Dynamic As Boolean = false
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QAGetPromptSet
        
        '<remarks/>
        Public Country As String
        
        '<remarks/>
        Public Engine As EngineType
        
        '<remarks/>
        Public PromptSet As PromptSetType
        
        '<remarks/>
        Public QAConfig As QAConfigType
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QALayout
        
        '<remarks/>
        Public Name As String
        
        '<remarks/>
        Public Comment As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QAGetLayouts
        
        '<remarks/>
        Public Country As String
        
        '<remarks/>
        Public QAConfig As QAConfigType
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QAExampleAddress
        
        '<remarks/>
        Public Address As QAAddressType
        
        '<remarks/>
        Public Comment As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QAAddressType
        
        '<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("AddressLine")>  _
        Public AddressLine() As AddressLineType
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public Overflow As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public Truncated As Boolean = false
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class AddressLineType
        
        '<remarks/>
        Public Label As String
        
        '<remarks/>
        Public Line As String
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(LineContentType.Address)>  _
        Public LineContent As LineContentType = LineContentType.Address
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public Overflow As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public Truncated As Boolean = false
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Enum LineContentType
        
        '<remarks/>
        None
        
        '<remarks/>
        Address
        
        '<remarks/>
        Name
        
        '<remarks/>
        Ancillary
        
        '<remarks/>
        DataPlus
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QAGetExampleAddresses
        
        '<remarks/>
        Public Country As String
        
        '<remarks/>
        Public Layout As String
        
        '<remarks/>
        Public QAConfig As QAConfigType
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QALicensedSet
        
        '<remarks/>
        Public ID As String
        
        '<remarks/>
        Public Description As String
        
        '<remarks/>
        Public Copyright As String
        
        '<remarks/>
        Public Version As String
        
        '<remarks/>
        Public BaseCountry As String
        
        '<remarks/>
        Public Status As String
        
        '<remarks/>
        Public Server As String
        
        '<remarks/>
        Public WarningLevel As LicenceWarningLevel
        
        '<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(DataType:="nonNegativeInteger")>  _
        Public DaysLeft As String
        
        '<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(DataType:="nonNegativeInteger")>  _
        Public DataDaysLeft As String
        
        '<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(DataType:="nonNegativeInteger")>  _
        Public LicenceDaysLeft As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Enum LicenceWarningLevel
        
        '<remarks/>
        None
        
        '<remarks/>
        DataExpiring
        
        '<remarks/>
        LicenceExpiring
        
        '<remarks/>
        ClicksLow
        
        '<remarks/>
        Evaluation
        
        '<remarks/>
        NoClicks
        
        '<remarks/>
        DataExpired
        
        '<remarks/>
        EvalLicenceExpired
        
        '<remarks/>
        FullLicenceExpired
        
        '<remarks/>
        LicenceNotFound
        
        '<remarks/>
        DataUnreadable
    End Enum
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QALicenceInfo
        
        '<remarks/>
        Public WarningLevel As LicenceWarningLevel
        
        '<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("LicensedSet")>  _
        Public LicensedSet() As QALicensedSet
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QADataSet
        
        '<remarks/>
        Public ID As String
        
        '<remarks/>
        Public Name As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class Address
        
        '<remarks/>
        Public QAAddress As QAAddressType
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QAGetAddress
        
        '<remarks/>
        Public Layout As String
        
        '<remarks/>
        Public Moniker As String
        
        '<remarks/>
        Public QAConfig As QAConfigType
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class Picklist
        
        '<remarks/>
        Public QAPicklist As QAPicklistType
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QAPicklistType
        
        '<remarks/>
        Public FullPicklistMoniker As String
        
        '<remarks/>
        <System.Xml.Serialization.XmlElementAttribute("PicklistEntry")>  _
        Public PicklistEntry() As PicklistEntryType
        
        '<remarks/>
        Public Prompt As String
        
        '<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(DataType:="nonNegativeInteger")>  _
        Public Total As String
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public AutoFormatSafe As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public AutoFormatPastClose As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public AutoStepinSafe As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public AutoStepinPastClose As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public LargePotential As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public MaxMatches As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public MoreOtherMatches As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public OverThreshold As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public Timeout As Boolean = false
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class PicklistEntryType
        
        '<remarks/>
        Public Moniker As String
        
        '<remarks/>
        Public PartialAddress As String
        
        '<remarks/>
        Public Picklist As String
        
        '<remarks/>
        Public Postcode As String
        
        '<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(DataType:="nonNegativeInteger")>  _
        Public Score As String
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public FullAddress As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public Multiples As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public CanStep As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public AliasMatch As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public PostcodeRecoded As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public CrossBorderMatch As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public DummyPOBox As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public Name As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public Information As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public WarnInformation As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public IncompleteAddr As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public UnresolvableRange As Boolean = false
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(false)>  _
        Public PhantomPrimaryPoint As Boolean = false
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QARefine
        
        '<remarks/>
        Public Moniker As String
        
        '<remarks/>
        Public Refinement As String
        
        '<remarks/>
        Public QAConfig As QAConfigType
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="positiveInteger")>  _
        Public Threshold As String
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(DataType:="nonNegativeInteger")>  _
        Public Timeout As String
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Class QASearchResult
        
        '<remarks/>
        Public QAPicklist As QAPicklistType
        
        '<remarks/>
        Public QAAddress As QAAddressType
        
        '<remarks/>
        <System.Xml.Serialization.XmlAttributeAttribute(),  _
         System.ComponentModel.DefaultValueAttribute(VerifyLevelType.None)>  _
        Public VerifyLevel As VerifyLevelType = VerifyLevelType.None
    End Class
    
    '<remarks/>
    <System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://www.qas.com/web-2005-02")>  _
    Public Enum VerifyLevelType
        
        '<remarks/>
        None
        
        '<remarks/>
        Verified
        
        '<remarks/>
        InteractionRequired
        
        '<remarks/>
        PremisesPartial
        
        '<remarks/>
        StreetPartial
        
        '<remarks/>
        Multiple
    End Enum
End Namespace