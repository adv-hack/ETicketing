Imports Microsoft.VisualBasic
Imports System.Data
Imports System.IO
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This is a Base class 
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQ-BASE-  
'                                   
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlResponse

        Private _company As String = String.Empty
        Private _documentID As String = String.Empty
        Private _documentVersion As String = String.Empty
        Private _emailFrom As String = String.Empty
        Private _emailCompany As String = String.Empty
        Private _emailUser As String = String.Empty
        Private _emailXmlResponse As Boolean = False
        Private _err As ErrorObj = Nothing
        Private _err2 As ErrorObj = Nothing
        Private _errorStatus As String = String.Empty
        Private _loginId As String = String.Empty
        Private _orderNumber As String = String.Empty
        Private _outgoingStyleSheet As String = String.Empty
        Private _resultDataSet As DataSet
        Private _receiverID As String = String.Empty
        Private _rootElement As String = String.Empty
        Private _senderID As String = String.Empty
        Private _storeXml As Boolean = False
        Private _timeStamp As Date
        Private _transactionID As String = String.Empty
        Private _transformedDocPath As String = String.Empty
        Private _untransformedDocPath As String = String.Empty
        Private _webServiceDesc As String = String.Empty
        Private _webServiceName As String = String.Empty
        Private _writeLog As Boolean = False
        Private _xsd As String = String.Empty
        Private _reponseDirectrory As String = String.Empty
        Private _complete As Boolean
        Private _totalRecords As Integer
        Private _processedRecords As Integer
        Private _progressTransactionID As String
        Private _markOrdersAsComplete As Boolean
        Private _settings As DESettings
        Private _orderCompleteStatus As String = String.Empty

        '
        Protected dtHeader As New DataTable
        Protected dtDetail As New DataTable
        Protected dtText As New DataTable
        Protected dtCarrier As New DataTable
        Protected dtPackage As New DataTable
        Protected dtProduct As New DataTable
        '----------------------------------------------------------------------------------------
        '   Protected  means that elements are accessible only from within their 
        '       own class or from a derived class. Protected can also be used with the 
        '       Friend keyword. When they're used together, elements are accessible from 
        '       the same assembly, from their own class, and from any derived classes. 
        '----------------------------------------------------------------------------------------
        Protected xmlDoc As New XmlDocument
        Protected Const resp As String = "RESPONSE"
        Protected Const SYSTEM21 As String = "SYSTEM21"
        Protected Const SQL2005 As String = "SQL2005"
        Protected Const bslash As String = "\"
        Protected Const fslash As String = "/"

        Public Property Company() As String
            Get
                Return _company
            End Get
            Set(ByVal value As String)
                _company = value
            End Set
        End Property
        Public Property DocumentID() As String
            Get
                Return _documentID
            End Get
            Set(ByVal value As String)
                _documentID = value
            End Set
        End Property
        Public Property DocumentVersion() As String
            Get
                Return _documentVersion
            End Get
            Set(ByVal value As String)
                _documentVersion = value
            End Set
        End Property
        Public Property EmailFrom() As String
            Get
                Return _emailFrom
            End Get
            Set(ByVal value As String)
                _emailFrom = value
            End Set
        End Property
        Public Property EmailCompany() As String
            Get
                Return _emailCompany
            End Get
            Set(ByVal value As String)
                _emailCompany = value
            End Set
        End Property
        Public Property EmailUser() As String
            Get
                Return _emailUser
            End Get
            Set(ByVal value As String)
                _emailUser = value
            End Set
        End Property
        Public Property EmailXmlResponse() As Boolean
            Get
                Return _emailXmlResponse
            End Get
            Set(ByVal value As Boolean)
                _emailXmlResponse = value
            End Set
        End Property
        Public Property Err() As ErrorObj               ' Global
            Get
                Return _err
            End Get
            Set(ByVal value As ErrorObj)
                _err = value
            End Set
        End Property
        Public Property Err2() As ErrorObj              ' Local
            Get
                Return _err2
            End Get
            Set(ByVal value As ErrorObj)
                _err2 = value
            End Set
        End Property
        Public Property ErrorStatus() As String
            Get
                Return _errorStatus
            End Get
            Set(ByVal value As String)
                _errorStatus = value
            End Set
        End Property
        Public Property LoginId() As String
            Get
                Return _loginId
            End Get
            Set(ByVal value As String)
                _loginId = value
            End Set
        End Property
        Public Property OrderNumber() As String
            Get
                Return _orderNumber
            End Get
            Set(ByVal value As String)
                _orderNumber = value
            End Set
        End Property
        Public Property OutgoingStyleSheet() As String
            Get
                Return _outgoingStyleSheet
            End Get
            Set(ByVal value As String)
                _outgoingStyleSheet = value
            End Set
        End Property
        Public Property ReceiverID() As String
            Get
                Return _receiverID
            End Get
            Set(ByVal value As String)
                _receiverID = value
            End Set
        End Property
        Public Property ResultDataSet() As DataSet
            Get
                Return _resultDataSet
            End Get
            Set(ByVal value As DataSet)
                _resultDataSet = value
            End Set
        End Property
        Public Property RootElement() As String
            Get
                Return _rootElement
            End Get
            Set(ByVal value As String)
                _rootElement = value
            End Set
        End Property
        Public Property SenderID() As String
            Get
                Return _senderID
            End Get
            Set(ByVal value As String)
                _senderID = value
            End Set
        End Property
        Public Property StoreXml() As Boolean
            Get
                Return _storeXml
            End Get
            Set(ByVal value As Boolean)
                _storeXml = value
            End Set
        End Property
        Public Property TimeStamp() As Date
            Get
                Return _timeStamp
            End Get
            Set(ByVal value As Date)
                _timeStamp = value
            End Set
        End Property
        Public Property TransactionID() As String
            Get
                Return _transactionID
            End Get
            Set(ByVal value As String)
                _transactionID = value
            End Set
        End Property
        Public Property TransformedDocPath() As String
            Get
                Return _transformedDocPath
            End Get
            Set(ByVal value As String)
                _transformedDocPath = value
            End Set
        End Property
        Public Property UntransformedDocPath() As String
            Get
                Return _untransformedDocPath
            End Get
            Set(ByVal value As String)
                _untransformedDocPath = value
            End Set
        End Property
        Public Property WebServiceDesc() As String
            Get
                Return _webServiceDesc
            End Get
            Set(ByVal value As String)
                _webServiceDesc = value
            End Set
        End Property
        Public Property WebServiceName() As String
            Get
                Return _webServiceName
            End Get
            Set(ByVal value As String)
                _webServiceName = value
            End Set
        End Property
        Public Property WriteLog() As Boolean
            Get
                Return _writeLog
            End Get
            Set(ByVal value As Boolean)
                _writeLog = value
            End Set
        End Property
        Public Property Xsd() As String
            Get
                Return _xsd
            End Get
            Set(ByVal value As String)
                _xsd = value
            End Set
        End Property
        Public Property Complete() As Boolean
            Get
                Return _complete
            End Get
            Set(ByVal value As Boolean)
                _complete = value
            End Set
        End Property
        Public Property TotalRecords() As Integer
            Get
                Return _totalRecords
            End Get
            Set(ByVal value As Integer)
                _totalRecords = value
            End Set
        End Property
        Public Property ProcessedRecords() As Integer
            Get
                Return _processedRecords
            End Get
            Set(ByVal value As Integer)
                _processedRecords = value
            End Set
        End Property
        Public Property ProgressTransactionID() As String
            Get
                Return _progressTransactionID
            End Get
            Set(ByVal value As String)
                _progressTransactionID = value
            End Set
        End Property
        Public Property ResponseDirectory() As String
            Get
                Return _reponseDirectrory
            End Get
            Set(ByVal value As String)
                _reponseDirectrory = value
            End Set
        End Property
        Public Property MarkOrdersAsComplete() As Boolean
            Get
                Return _markOrdersAsComplete
            End Get
            Set(ByVal value As Boolean)
                _markOrdersAsComplete = value
            End Set
        End Property
        Public Property Settings() As DESettings
            Get
                Return _settings
            End Get
            Set(ByVal value As DESettings)
                _settings = value
            End Set
        End Property
        Public Property OrderCompleteStatus() As String
            Get
                Return _orderCompleteStatus
            End Get
            Set(ByVal value As String)
                _orderCompleteStatus = value
            End Set
        End Property

        Public Sub CreateResponse()

            Select Case DocumentVersion()
                Case "1.0"
                    InsertHeaderV1()
                    InsertBodyV1()
                Case "1.1"
                    InsertHeaderV1()
                    InsertBodyV1_1()
                Case "1.2"
                    InsertHeaderV1()
                    InsertBodyV1_2()
                Case "1.3"
                    InsertHeaderV1()
                    InsertBodyV1_3()
            End Select
            '----------------------------------------------------------------------------------------
            If Not Err2 Is Nothing Then
                If Err2.HasError Then
                    LogWriter.WriteDBLog(True, TransactionID, _
                                        Err2.ErrorMessage, _
                                        Err2.ErrorStatus, _
                                        Err2.ErrorNumber)
                End If
            End If            '
            If Not Err Is Nothing Then
                If Err.HasError Then
                    LogWriter.WriteDBLog(True, TransactionID, _
                                        Err.ErrorMessage, _
                                        Err.ErrorStatus, _
                                        Err.ErrorNumber)
                End If
            End If
            '----------------------------------------------------------------------------------------
        End Sub
        Protected Overridable Sub InsertHeaderV1()

            Dim ndProcessingInstruction, ndRoot, ndVersion, ndTransactionHeader, _
                ndSenderID, ndReceiverID, ndErrorStatus, ndDocumentID, _
                ndTransactionID, ndTimeStamp As XmlNode

            Dim atXmlNs, atErrorNumber As XmlAttribute
            '
            With xmlDoc
                '-------------------------------------------------------------------------------------
                '   Constants are faster at runtime
                '
                Const str1 As String = "xml"
                Const str2 As String = "version=""1.0"" encoding=""utf-8"""
                Const str3 As String = "xmlns"
                Const str4 As String = "XMLNamespace"
                ndProcessingInstruction = .CreateProcessingInstruction(str1, str2)
                ndRoot = .CreateElement(RootElement())
                atXmlNs = .CreateAttribute(str3)
                atXmlNs.Value = ConfigurationManager.AppSettings(str4).ToString()
                ndRoot.Attributes.Append(atXmlNs)
                '-----------------------------------------------------------
                Const str5 As String = "Version"
                Const str6 As String = "TransactionHeader"
                Const str7 As String = "SenderID"
                Const str8 As String = "ReceiverID"
                ndVersion = .CreateElement(str5)
                ndTransactionHeader = .CreateElement(str6)
                ndSenderID = .CreateElement(str7)
                ndReceiverID = .CreateElement(str8)
                '
                ndVersion.InnerText = DocumentVersion()
                ndSenderID.InnerText = SenderID()
                ndReceiverID.InnerText = ReceiverID()
                '-----------------------------------------------------------
                Const str9 As String = "ErrorStatus"
                Const str10 As String = "ErrorNumber"
                Const str11 As String = "DocumentID"
                ndErrorStatus = .CreateElement(str9)
                If Not Err Is Nothing Then _
                    ndErrorStatus.InnerText = Err.ErrorStatus()
                atErrorNumber = .CreateAttribute(str10)
                If Not Err Is Nothing Then _
                    atErrorNumber.Value = Err.ErrorNumber()
                ndErrorStatus.Attributes.Append(atErrorNumber)
                ndDocumentID = .CreateElement(str11)
                '-----------------------------------------------------------
                Const str12 As String = "TransactionID"
                Const str13 As String = "TimeStamp"
                Const str14 As String = "yyyy-MM-ddTHH:mm:ss"
                ndDocumentID.InnerText = System.Guid.NewGuid().ToString
                ndTransactionID = .CreateElement(str12)
                ndTransactionID.InnerText = TransactionID()
                ndTimeStamp = .CreateElement(str13)
                ndTimeStamp.InnerText = DateTime.Now.ToString(str14)
                '-----------------------------------------------------------
                'Build XML Document
                .AppendChild(ndProcessingInstruction)
                .AppendChild(ndRoot)
                '
            End With
            '
            ndRoot.AppendChild(ndVersion)
            ndRoot.AppendChild(ndTransactionHeader)
            '
            With ndTransactionHeader
                .AppendChild(ndSenderID)
                .AppendChild(ndReceiverID)
                .AppendChild(ndErrorStatus)
                .AppendChild(ndDocumentID)
                .AppendChild(ndTransactionID)
                .AppendChild(ndTimeStamp)
            End With
            '
        End Sub
        Public Sub ApplyOutgoingStyleSheet()
            '--------------------------------------------------------------------------------------
            ' Constants are faster at run time
            '
            Const documentsFolder As String = "DocumentsFolder"
            Const responsesFolder As String = "ResponsesFolder"
            Const xSDFolder As String = "xSDFolder"
            Const xSLTFolder As String = "XSLTFolder"
            '--------------------------------------------------------------------------------------
            '   CODE FOR SPECIFYING LOGGING DETAILS
            '
            Dim location As String = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings(documentsFolder).ToString)

            Dim xsltResPath As String = location & _
                                bslash & ConfigurationManager.AppSettings(xSLTFolder).ToString & _
                                bslash & ConfigurationManager.AppSettings(responsesFolder).ToString

            Dim xsdResPath As String = location & _
                                bslash & ConfigurationManager.AppSettings(xSDFolder).ToString & _
                                bslash & ConfigurationManager.AppSettings(responsesFolder).ToString
            '--------------------------------------------------------------------------------------
            '   LOG: XMLRequest loaded... applying outgoing stylesheet
            '
            If StoreXml Then                                            '   LOG: StoreXML = True
                LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                "L0265", resp, String.Empty, String.Empty, UntransformedDocPath, _
                                TransformedDocPath, xsltResPath & bslash & OutgoingStyleSheet, _
                                xsdResPath & bslash & Xsd, xmlDoc, 3)
            Else                                                        '   LOG: StoreXml = False
                LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                "L0270", resp, String.Empty, String.Empty, UntransformedDocPath, _
                                TransformedDocPath, xsltResPath & bslash & OutgoingStyleSheet, _
                                xsdResPath & bslash & Xsd)
            End If
            '--------------------------------------------------------------------------------------
            '   Apply style sheet
            ' 
            Dim xsltDocPath As String = HttpContext.Current.Server.MapPath(Path.Combine( _
                                Path.Combine(Path.Combine(ConfigurationManager.AppSettings(documentsFolder).ToString, _
                                ConfigurationManager.AppSettings(xSLTFolder).ToString), _
                                ConfigurationManager.AppSettings(responsesFolder).ToString), OutgoingStyleSheet()))

            Dim xmlUtilsObj As New XmlUtils
            xmlDoc = xmlUtilsObj.ApplyXsltStyleSheet(xmlDoc, xsltDocPath)

            '--------------------------------------------------------------------------------------
            '   LOG: Outgoing stylesheet application complete
            '
            If StoreXml Then                                            '   LOG: StoreXml = True
                LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                "L0275", resp, String.Empty, String.Empty, UntransformedDocPath, _
                                TransformedDocPath, xsltResPath & bslash & OutgoingStyleSheet, _
                                xsdResPath & bslash & Xsd, xmlDoc, 4)
            Else                                                        '  LOG: StoreXml = False
                LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                "L0280", resp, String.Empty, String.Empty, UntransformedDocPath, _
                                TransformedDocPath, xsltResPath & bslash & OutgoingStyleSheet, _
                                xsdResPath & bslash & Xsd)
            End If
            '--------------------------------------------------------------------------------------
            '   Check to see if an error has occured during the transformation
            '
            If xmlUtilsObj Is Nothing Then
                If xmlUtilsObj.XsltErr.HasError Then                    '   LOG: Error during StyleSheet application
                    LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                "L0285", resp, String.Empty, String.Empty, UntransformedDocPath, _
                                TransformedDocPath, xsltResPath & bslash & OutgoingStyleSheet, _
                                xsdResPath & bslash & Xsd)
                    With Err
                        .ErrorMessage = String.Empty
                        .HasError = xmlUtilsObj.XsltErr.HasError
                        .ErrorStatus = xmlUtilsObj.XsltErr.ErrorStatus
                        .ErrorNumber = xmlUtilsObj.XsltErr.ErrorNumber
                    End With
                End If
            End If
        End Sub
        Protected Overridable Sub InsertBodyV1()
        End Sub
        Protected Overridable Sub InsertBodyV1_1()
        End Sub
        Protected Overridable Sub InsertBodyV1_2()
        End Sub
        Protected Overridable Sub InsertBodyV1_3()
        End Sub
        Protected Overridable Sub InsertBodyV2()
        End Sub
        Protected Overridable Sub InsertBodyV3()
        End Sub
        Friend Function GetXmlResponseAsString() As String
            Dim s As String
            Dim xmlUtilsObj As New XmlUtils
            s = xmlUtilsObj.GetXmlDocAsString(xmlDoc)
            Return s
        End Function
        Friend Function CreateNamespaceAttribute() As XmlAttribute
            Dim atXmlNsXsi As XmlAttribute
            '---------------------------------------------------------------------
            '   Constants and StringBuilder are faster at run time
            '
            Const space As String = " "
            Const dot As String = "."
            '
            Const str0 As String = ".xsd"
            Const str1 As String = "XMLNamespace"
            Const str2 As String = "DocumentsFolder"
            Const str3 As String = "XSDFolder"
            Const str4 As String = "ResponsesFolder"
            Const str5 As String = "schemaLocation"
            Const str6 As String = "XMLSchemaInstance"
            Const str7 As String = "xsi"
            '
            Dim docName As String = RootElement & DocumentVersion.Replace(dot, String.Empty) & str0
            Dim xsdDocPath As New StringBuilder

            With xsdDocPath
                .Append(ConfigurationManager.AppSettings(str1).ToString)
                .Append(space)
                .Append(ConfigurationManager.AppSettings(str1).ToString)
                .Append(fslash)
                .Append(ConfigurationManager.AppSettings(str2).ToString)
                .Append(fslash)
                .Append(ConfigurationManager.AppSettings(str3).ToString)
                .Append(fslash)
                .Append(ConfigurationManager.AppSettings(str4).ToString)
                .Append(fslash)
                .Append(docName)
            End With
            '---------------------------------------------------------------------
            Dim nsmgr As New XmlNamespaceManager(xmlDoc.NameTable)
            nsmgr.AddNamespace(str7, ConfigurationManager.AppSettings(str1).ToString)

            atXmlNsXsi = xmlDoc.CreateAttribute(str7, str5, _
                                ConfigurationManager.AppSettings(str6).ToString)
            atXmlNsXsi.Value = xsdDocPath.ToString
            Return atXmlNsXsi
        End Function

        Public Function ConvertFromDate7(ByVal date7 As String) As String
            Dim returnDate As String = String.Empty
            If date7.Length = 7 Then
                Select Case date7.Substring(0, 1)
                    Case Is = "1"
                        returnDate = "20" & date7.Substring(1, 2) & "-" & date7.Substring(3, 2) & "-" & date7.Substring(5, 2)
                    Case Is = "0"
                        returnDate = "19" & date7.Substring(1, 2) & "-" & date7.Substring(3, 2) & "-" & date7.Substring(5, 2)
                End Select
            End If

            Return returnDate

        End Function
    End Class

End Namespace