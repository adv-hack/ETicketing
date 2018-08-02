Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Xml
Imports System.Xml.Schema
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
'       Error Number Code base      TTPRE-BASE- 
'                                   
'--------------------------------------------------------------------------------------------------
'   Modification Summary
'
'   dd/mm/yy    ID      By      Description
'   --------    -----   ---     -----------
'   29/11/06    /001    Ben     Add Account no's
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlRequest

        'Stuart20070529
        'Private _settings As New DESettings
        Private _settings As DESettings
        'Stuart20070529
        Private _TDataObjects As Talent.Common.TalentDataObjects
        Private Const dFormat As String = "dd/MMM/yyyy HH:mm:ss"

        Private _applyStyleSheet As Boolean = False
        Private _defaultCurrentVersion As String = String.Empty
        Private _destinationDatabase As String = String.Empty
        Private _documentVersion As String = String.Empty
        Private _IncomingStyleSheet As String = String.Empty
        Private _loginId As String = String.Empty
        Private _resultDataSet As DataSet
        Private _rootElement As String = String.Empty
        Private _storeXml As Boolean = False
        Private _transactionID As String = 0
        Private _transformedDocPath As String = String.Empty
        Private _untransformedDocPath As String = String.Empty
        Private _validDocument As Boolean = True
        Private _writeLog As Boolean = False
        Private _xsd As String = String.Empty
        Private _company As String = String.Empty
        Private _password As String = String.Empty

        Private _TotalRecords As Integer = 0
        Private _ProcessRecords As Integer = 0
        Private _Complete As Boolean
        Private _transactionEndDate As DateTime
        Private _progressTransactionID As String

        Private unconvertedXmlDoc As New XmlDocument
        '----------------------------------------------------------------------------------------
        '   Protected  means that elements are accessible only from within their 
        '       own class or from a derived class. Protected can also be used with the 
        '       Friend keyword. When they're used together, elements are accessible from 
        '       the same assembly, from their own class, and from any derived classes. 
        '----------------------------------------------------------------------------------------
        Protected Const req As String = "Request"
        Protected Const res As String = "Response"
        Protected Const SYSTEM21 As String = "SYSTEM21"
        Protected Const SQL2005 As String = "SQL2005"
        Protected Const bSlash As String = "\"
        Protected Const fSlash As String = "/"
        Private _dep As DEOrder
        Public Property Dep() As DEOrder
            Get
                Return _dep
            End Get
            Set(ByVal value As DEOrder)
                _dep = value
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

        Public Property ApplyStyleSheet() As Boolean
            Get
                Return _applyStyleSheet
            End Get
            Set(ByVal value As Boolean)
                _applyStyleSheet = value
            End Set
        End Property
        Public Property DestinationDatabase() As String
            Get
                Return _destinationDatabase
            End Get
            Set(ByVal value As String)
                _destinationDatabase = value
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
        Public Property IncomingStyleSheet() As String
            Get
                Return _IncomingStyleSheet
            End Get
            Set(ByVal value As String)
                _IncomingStyleSheet = value
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
        Public Property DefaultCurrentVersion() As String
            Get
                Return _defaultCurrentVersion
            End Get
            Set(ByVal value As String)
                _defaultCurrentVersion = value
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
        Public Property StoreXml() As Boolean
            Get
                Return _storeXml
            End Get
            Set(ByVal value As Boolean)
                _storeXml = value
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
        Public Property ValidDocument() As Boolean
            Get
                Return _validDocument
            End Get
            Set(ByVal value As Boolean)
                _validDocument = value
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
        Public Property Password() As String
            Get
                Return _password
            End Get
            Set(ByVal value As String)
                _password = value
            End Set
        End Property

        Public Property Company() As String
            Get
                Return _company
            End Get
            Set(ByVal value As String)
                _company = value
            End Set
        End Property

        Public Property TotalRecords() As Integer
            Get
                Return _TotalRecords
            End Get
            Set(ByVal value As Integer)
                _TotalRecords = value
            End Set
        End Property

        Public Property ProcessedRecords() As Integer
            Get
                Return _ProcessRecords
            End Get
            Set(ByVal value As Integer)
                _ProcessRecords = value
            End Set
        End Property

        Public ReadOnly Property Complete() As Boolean
            Get
                Return (Not TransactionEndDate = Nothing)
            End Get
        End Property

        Public Property TransactionEndDate() As DateTime
            Get
                Return _transactionEndDate
            End Get
            Set(ByVal value As DateTime)
                _transactionEndDate = value
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
        Public Property TDataObjects() As Talent.Common.TalentDataObjects
            Get
                _TDataObjects.Settings = Settings
                Return _TDataObjects
            End Get
            Set(ByVal value As Talent.Common.TalentDataObjects)
                _TDataObjects = value
            End Set
        End Property
        Protected xmlDoc As New XmlDocument

        'Stuart20070529
        'Create the settings object of the correct type
        Public Sub New(ByVal webServiceName As String)
            Select Case webServiceName
                Case Is = "AvailabilityRequest"
                    Dim ss As New DEStockSettings
                    Settings() = CType(ss, DEStockSettings)
                Case Is = "RetrieveEligibleTicketingCustomersRequest"
                    Dim ss As New DEProductSettings
                    Settings() = CType(ss, DEProductSettings)
                Case Is = "OrderRequest", "OrderChangeRequest"
                    Dim ss As New DEOrderSettings
                    Settings() = CType(ss, DEOrderSettings)
                Case Is = "RetrievePromotionsRequest"
                    Dim ss As New DEPromotionSettings
                    Settings() = CType(ss, DEPromotionSettings)
                Case Else
                    Settings() = New DESettings
            End Select
        End Sub

        Public Sub New()
            Settings() = New DESettings
            If _TDataObjects Is Nothing Then
                _TDataObjects = New Talent.Common.TalentDataObjects()
                Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
                settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                settings.DestinationDatabase = "SQL2005"
                _TDataObjects.Settings = settings
            End If
        End Sub
        'Stuart20070529

        Public Function CreateRequest(ByVal xmlRequestString As String) As ErrorObj
            Dim err As New ErrorObj
            '
            '--------------------------------------------------------------------------------------
            ' Constants are faster at run time
            '
            Const documentsFolder As String = "DocumentsFolder"
            Const requestsFolder As String = "RequestsFolder"
            Const xSDFolder As String = "xSDFolder"
            Const xSLTFolder As String = "XSLTFolder"
            '--------------------------------------------------------------------------------------
            '   CODE FOR SPECIFYING LOGGING DETAILS
            '
            Dim location As String = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings(documentsFolder))

            Dim xsltReqPath As String = location & _
                                bSlash & ConfigurationManager.AppSettings(xSLTFolder).ToString & _
                                bSlash & ConfigurationManager.AppSettings(requestsFolder).ToString
            '
            Dim xsdReqPath As String = location & _
                                bSlash & ConfigurationManager.AppSettings(xSDFolder).ToString & _
                                bSlash & ConfigurationManager.AppSettings(requestsFolder).ToString
            '--------------------------------------------------------------------------------------

            'LOG: XmlRequest loaded
            If StoreXml Then
                'LOG: StoreXml = True
                unconvertedXmlDoc.LoadXml(xmlRequestString)
                LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0145", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd, unconvertedXmlDoc, 1)
            Else
                'LOG: StoreXml = False
                LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0150", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd)
            End If

            If ApplyStyleSheet Then

                'Create an XML document from the string. If this fails, return an error.
                Try

                    Dim xsltDocPath As String = HttpContext.Current.Server.MapPath(Path.Combine( _
                        Path.Combine(Path.Combine(ConfigurationManager.AppSettings(documentsFolder).ToString, _
                        ConfigurationManager.AppSettings(xSLTFolder).ToString), _
                        ConfigurationManager.AppSettings(requestsFolder).ToString), IncomingStyleSheet()))

                    'LOG: ApplySS = True... loading XML doc
                    LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0155", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd)

                    unconvertedXmlDoc.LoadXml(xmlRequestString)

                    'LOG: XML doc loaded... applying stylesheet
                    LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0160", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd)

                    '---------------------
                    'Apply the style sheet
                    '---------------------
                    Dim xmlUtilsObj As New XmlUtils
                    xmlDoc = xmlUtilsObj.ApplyXsltStyleSheet(unconvertedXmlDoc, xsltDocPath)

                    'LOG: Stylesheet applied
                    If StoreXml Then
                        'LOG: StoreXml = True... Testing Document Validity
                        LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0165", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd, xmlDoc, 2)
                    Else
                        'LOG: StoreXml = False... Testing Document Validity
                        LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0170", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd)
                    End If


                    If xmlUtilsObj Is Nothing Then
                        'Check to see if an error has occured during the transformation
                        If xmlUtilsObj.XsltErr.HasError Then
                            'LOG: Error during application of Stylesheet... Ending request
                            LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0175", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd)
                            With err
                                .ErrorMessage = String.Empty
                                .HasError = xmlUtilsObj.XsltErr.HasError
                                .ErrorStatus = "Error loading or executing the XSL Document: " & xmlUtilsObj.XsltErr.ErrorStatus
                                .ErrorNumber = xmlUtilsObj.XsltErr.ErrorNumber
                            End With
                            ValidDocument = False
                        End If
                    End If
                    'Always use the latest request version when a translation has been carried out
                    DocumentVersion = DefaultCurrentVersion()
                Catch ex As Exception
                    'LOG: Error during Try in ApplyStylesheet... Ending Request
                    LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0180", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd)
                    ValidDocument = False
                    Const strError11 As String = "Not a valid XML document - cannot convert"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorNumber = "TTPRE-BASE-01"
                        .ErrorStatus = strError11
                        .HasError = True
                    End With
                End Try

            Else

                Try
                    xmlDoc.LoadXml(xmlRequestString)
                    DocumentVersion = xmlDoc.SelectSingleNode(fSlash & RootElement & "/Version").InnerText
                    'LOG: ApplyStyleSheet = False... Testing Document Validity
                    LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0185", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd)
                Catch ex As Exception
                    ValidDocument = False
                    Const strError11 As String = "Not a valid XML document  "
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorNumber = "TTPRE-BASE-02"
                        .ErrorStatus = strError11
                        .HasError = True
                    End With
                    'LOG: Error loading XmlDoc... Ending Request
                    LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0190", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd)
                End Try

            End If

            '-----------------------------------------
            'Validate the document if no errors so far
            '-----------------------------------------
            If ValidDocument() Then
                'LOG: Document Valid.. Checking content of doc
                LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0195", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd)

                'If XSD document specified, use it for validation
                If String.IsNullOrEmpty(Xsd) Then
                    'LOG: Document empty
                    LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0200", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd)
                Else

                    'But only if it exists
                    Dim xsdDocPath As String = HttpContext.Current.Server.MapPath(Path.Combine( _
                        Path.Combine(Path.Combine(ConfigurationManager.AppSettings(documentsFolder).ToString, _
                        ConfigurationManager.AppSettings(xSDFolder).ToString), _
                        ConfigurationManager.AppSettings(requestsFolder).ToString), Xsd))
                    If Not File.Exists(xsdDocPath) Then
                        'LOG: XSD File does not exist... ending request
                        LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0205", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd)
                        Const strError11 As String = "Cannot locate XSD"
                        With err
                            .ErrorMessage = String.Empty
                            .ErrorNumber = "TTPRE-BASE-03"
                            .ErrorStatus = strError11
                            .HasError = True
                        End With
                        ValidDocument = False
                    Else
                        'LOG: XSD File exists... validating Xml
                        LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0210", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd)
                        err = ValidateAgainstXsd(xsdDocPath)
                    End If

                End If
                If ValidDocument() Then
                    '----------------------------------------------------------------------------------------------
                    'Perform extended validation - i.e. that cannot be achieved via XSD doc, e.g. Field A > Field B 
                    '----------------------------------------------------------------------------------------------
                    err = ExtendedValidation()
                    'LOG: XML Doc has passed all checks
                    LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0240", res, UntransformedDocPath, TransformedDocPath, String.Empty, String.Empty, _
                                        xsltReqPath & bSlash & IncomingStyleSheet, _
                                        xsdReqPath & bSlash & Xsd)
                End If

            End If
            Return err
        End Function
        Private Function ValidateAgainstXsd(ByVal xsdDocPath As String) As ErrorObj
            Dim err As New ErrorObj

            Dim xu As New XmlUtils
            Dim vreader As XmlReader = Nothing
            '---------------------------------------------------------------------------------
            Try
                'vreader = xu.GetXMLwithXSD(xmlDoc, xsdDocPath, ConfigurationManager.AppSettings("XMLNamespace").ToString())
                vreader = xu.GetXMLwithXSD(xmlDoc, xsdDocPath, String.Empty)
                'LOG: XSD loaded... checking schema
                LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0215", res, UntransformedDocPath, _
                                        TransformedDocPath, String.Empty, String.Empty, IncomingStyleSheet, _
                                        xsdDocPath)
            Catch ex As Exception
                Const strError11 As String = "Cannot locate XSD"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorNumber = "TTPRE-BASE-11"
                    .ErrorStatus = strError11
                    .HasError = True
                End With
                ValidDocument = False
                'LOG: Error loading xsd... Ending request
                LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0220", res, UntransformedDocPath, _
                                        TransformedDocPath, String.Empty, String.Empty, IncomingStyleSheet, _
                                        xsdDocPath)
            End Try
            '---------------------------------------------------------------------------------
            If ValidDocument() Then
                Try
                    While vreader.Read
                        If xu.ErrorText <> String.Empty Then
                            Dim a As String = "a"
                        End If
                        'Read to the end of the document
                    End While
                Catch ex As Exception
                    vreader.Close()
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorNumber = "TTPRE-BASE-12"
                        .ErrorStatus = "XML Document is not well-formed: " & ex.Message
                        .HasError = True
                    End With
                    ValidDocument = False
                    'LOG: Error xml document is not well formed... Ending Request
                    LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0225", res, UntransformedDocPath, _
                                        TransformedDocPath, String.Empty, String.Empty, IncomingStyleSheet, _
                                        xsdDocPath)
                End Try
                vreader.Close()
                '------------------------------------------------------------------------------
                If ValidDocument() Then
                    'Set the error in the error object
                    If xu.ErrorText().Equals(String.Empty) Or xu.ErrorText() Is Nothing Then
                        'LOG: XML schema OK
                        LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0230", res, UntransformedDocPath, _
                                        TransformedDocPath, String.Empty, String.Empty, IncomingStyleSheet, _
                                        xsdDocPath)
                    Else
                        With err
                            .ErrorMessage = String.Empty
                            .ErrorNumber = "TTPRE-BASE-13"
                            .ErrorStatus = xu.ErrorText()
                            .HasError = True
                        End With
                        ValidDocument = False
                        'LOG: XML doc does not conform to schema
                        LogWriter.WriteDBLog(WriteLog, TransactionID, _
                                        "L0235", res, UntransformedDocPath, _
                                        TransformedDocPath, String.Empty, String.Empty, IncomingStyleSheet, _
                                        xsdDocPath)
                    End If
                End If
            End If
            '---------------------------------------------------------------------------------
            Return err
        End Function
        Protected Overridable Function ExtendedValidation() As ErrorObj
            Dim err As New ErrorObj
            Return err
        End Function
        Public Overridable Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xr As New XmlResponse

            Return xr
        End Function
        Public Function Extract_TransactionHeader(ByVal Node1 As XmlNode) As DETransaction
            '-----------------------------------------------------------------------
            '   <TransactionHeader>
            '       <SenderID>123456789</SenderID>
            '       <ReceiverID>987654321</ReceiverID>
            '       <CountryCode>UK</CountryCode>
            '       <LoginID>UK3833HHD</LoginID>
            '       <Password>Re887Jky52</Password>
            '       <Company>CSG</Company>
            '       <TransactionID>54321</TransactionID>
            '   </TransactionHeader>
            '-----------------------------------------------------------------------
            '   Constant Strings are dealt with at compile time so can be a lot faster, than
            '   having to deal with a load of strings umpteen times at runtime
            '
            Const sSenderID As String = "SenderID"
            Const sReceiverID As String = "ReceiverID"
            Const sCountryCode As String = "CountryCode"
            Const sLoginID As String = "LoginID"
            Const sPassword As String = "Password"
            Const sCompany As String = "Company"
            Const sTransactionID As String = "TransactionID"
            '------------------------------------------------------------------------------
            Dim detr As New DETransaction           ' Items
            Dim Node2 As XmlNode
            With detr
                For Each Node2 In Node1.ChildNodes
                    Select Case Node2.Name
                        Case Is = sCompany : .Company = Node2.InnerText
                        Case Is = sCountryCode : .CountryCode = Node2.InnerText
                        Case Is = sLoginID : .loginId = Node2.InnerText
                        Case Is = sPassword : .Password = Node2.InnerText
                        Case Is = sReceiverID : .ReceiverID = Node2.InnerText
                        Case Is = sSenderID : .SenderID = Node2.InnerText
                        Case Is = sTransactionID : .TransactionID = Node2.InnerText
                    End Select
                Next Node2
            End With
            Return detr

        End Function
        Public Function Extract_OrderHeader(ByVal Node1 As XmlNode) As DeOrderHeader
            '-----------------------------------------------------------------------
            Dim deoh As New DeOrderHeader           ' Items and DEAddress as Collection
            Dim dead1 As New DeAddress              ' Single Line Item
            Dim dead2 As New DeAddress              ' Single Line Item
            Dim Node2, Node3 As XmlNode
            '-----------------------------------------------------------------------
            With deoh
                .Category = Node1.Name
                For Each Node2 In Node1.ChildNodes
                    Select Case Node2.Name
                        Case Is = "BranchOrderNumber" : .BranchOrderNumber = Node2.InnerText
                        Case Is = "BillToSuffix" : .BillToSuffix = Node2.InnerText
                        Case Is = "CustomerPO" : .CustomerPO = Node2.InnerText
                        Case Is = "OrderActionCode" : .OrderActionCode = Node2.InnerText
                        Case Is = "OrderSuffix" : .OrderSuffix = Node2.InnerText
                        Case Is = "ShipToSuffix" : .ShipToSuffix = Node2.InnerText
                            '
                        Case Is = "BillToAddressInformation"
                            With dead1
                                .Category = "BillTo"
                                For Each Node3 In Node2.SelectSingleNode("BillTo/Address").ChildNodes
                                    'For Each Node3 In Node2.ChildNodes
                                    Select Case Node3.Name
                                        '-----------------------------------------------------------
                                        '   Billing Address info
                                        '
                                        Case Is = "NewBillToAttention" : .Attention = Node3.InnerText
                                        Case Is = "NewBillToAddress1" : .Line1 = Node3.InnerText
                                        Case Is = "NewBillToAddress2" : .Line2 = Node3.InnerText
                                        Case Is = "NewBillToAddress3" : .Line3 = Node3.InnerText
                                        Case Is = "NewBillToCity" : .City = Node3.InnerText
                                        Case Is = "NewBillToProvince" : .Province = Node3.InnerText
                                        Case Is = "NewBillToPostalCode" : .PostalCode = Node3.InnerText
                                        Case Is = "NewBillToCountry" : .CountryCode = Node3.InnerText
                                    End Select
                                Next Node3
                            End With
                            deoh.CollDEAddress.Add(dead1)
                        Case Is = "ShipmentAddressInformation"
                            With dead2
                                .Category = "ShipTo"
                                For Each Node3 In Node2.SelectSingleNode("ShipTo/Address").ChildNodes
                                    Select Case Node3.Name
                                        '-----------------------------------------------------------
                                        '   Shipping Address info
                                        '
                                        Case Is = "NewBillToSuffix" : deoh.BillToSuffix = Node3.InnerText
                                        Case Is = "NewCarrierCode" : deoh.CarrierCode = Node3.InnerText
                                        Case Is = "NewCustomerPO" : deoh.NewCustomerPO = Node3.InnerText
                                        Case Is = "NewEndUserPO" : deoh.EndUserPO = Node3.InnerText
                                        Case Is = "NewShipToSuffix" : deoh.ShipToSuffix = Node3.InnerText

                                        Case Is = "NewShipToAttention" : .Attention = Node3.InnerText
                                        Case Is = "NewShipToAddress1" : .Line1 = Node3.InnerText
                                        Case Is = "NewShipToAddress2" : .Line2 = Node3.InnerText
                                        Case Is = "NewShipToAddress3" : .Line3 = Node3.InnerText
                                        Case Is = "NewShipToCity" : .City = Node3.InnerText
                                        Case Is = "NewShipToProvince" : .Province = Node3.InnerText
                                        Case Is = "NewShipToPostalCode" : .PostalCode = Node3.InnerText
                                        Case Is = "NewShipToCountry" : .CountryCode = Node3.InnerText
                                    End Select
                                Next Node3
                            End With
                            deoh.CollDEAddress.Add(dead2)
                        Case Is = "Extension"
                            For Each Node3 In Node2.ChildNodes
                                Select Case Node3.Name
                                    Case Is = "Reference1" : .ExtensionReference1 = Node3.InnerText
                                    Case Is = "Reference2" : .ExtensionReference2 = Node3.InnerText
                                    Case Is = "Reference3" : .ExtensionReference3 = Node3.InnerText
                                    Case Is = "Reference4" : .ExtensionReference4 = Node3.InnerText
                                    Case Is = "FixedPrice1" : .ExtensionFixedPrice1 = Node3.InnerText
                                    Case Is = "FixedPrice2" : .ExtensionFixedPrice2 = Node3.InnerText
                                    Case Is = "FixedPrice3" : .ExtensionFixedPrice3 = Node3.InnerText
                                    Case Is = "FixedPrice4" : .ExtensionFixedPrice4 = Node3.InnerText
                                    Case Is = "DealID1" : .ExtensionDealID1 = Node3.InnerText
                                    Case Is = "DealID2" : .ExtensionDealID2 = Node3.InnerText
                                    Case Is = "DealID3" : .ExtensionDealID3 = Node3.InnerText
                                    Case Is = "DealID4" : .ExtensionDealID4 = Node3.InnerText
                                    Case Is = "DealID5" : .ExtensionDealID5 = Node3.InnerText
                                    Case Is = "DealID6" : .ExtensionDealID6 = Node3.InnerText
                                    Case Is = "DealID7" : .ExtensionDealID7 = Node3.InnerText
                                    Case Is = "DealID8" : .ExtensionDealID8 = Node3.InnerText
                                    Case Is = "Flag1" : .ExtensionFlag1 = Node3.InnerText
                                    Case Is = "Flag2" : .ExtensionFlag2 = Node3.InnerText
                                    Case Is = "Flag3" : .ExtensionFlag3 = Node3.InnerText
                                    Case Is = "Flag4" : .ExtensionFlag4 = Node3.InnerText
                                    Case Is = "Flag5" : .ExtensionFlag5 = Node3.InnerText
                                    Case Is = "Flag6" : .ExtensionFlag6 = Node3.InnerText
                                    Case Is = "Flag7" : .ExtensionFlag7 = Node3.InnerText
                                    Case Is = "Status" : .ExtensionStatus = Node3.InnerText

                                End Select
                            Next Node3

                    End Select
                Next Node2
            End With
            Return deoh
        End Function
        Public Function LoadDefaultXmlV1(ByVal webService As String, ByVal NodeName As String) As ErrorObj
            Const ModuleName As String = "LoadDefaultXmlV1"
            Dim err As New ErrorObj
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Dim Node1 As XmlNode
            Const TransactionHeader As String = "TransactionHeader"
            Const DoubleSlash As String = "//"
            Dep = New DEOrder
            Try
                For Each Node1 In xmlDoc.SelectSingleNode(DoubleSlash & webService).ChildNodes
                    Select Case Node1.Name
                        Case Is = TransactionHeader
                            Dep.CollDETrans.Add(Extract_TransactionHeader(Node1))
                        Case Is = NodeName
                            Dep.CollDEOrders.Add(Extract_OrderHeader(Node1))
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPREBASE-09"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Public Overridable Function ValidateV1() As ErrorObj
            Dim err As ErrorObj = Nothing
            '----------------------------------------------------------------------
            '   Validation that cannot be achieved via XSD doc, e.g. Field A > Field B 
            '            
            Select Case Settings.DestinationDatabase
                Case Is = SYSTEM21
                    err = ValidateV1System21()
                Case Is = SQL2005
                    err = ValidateV1SQL2005()
            End Select

            Return err
        End Function
        Protected Overridable Function ValidateV1System21() As ErrorObj
            Dim err As ErrorObj = Nothing
            '----------------------------------------------------------------
            '   System 21 specific validation that cannot be acieved via XSD doc
            '
            Return err
        End Function
        Protected Overridable Function ValidateV1SQL2005() As ErrorObj
            Dim err As ErrorObj = Nothing
            '----------------------------------------------------------------
            '   SQL2005 specific validation that cannot be acieved via XSD doc
            '
            Return err
        End Function

        Public Function SetAttributeAction(ByVal action As String) As String
            Dim returnVal As String = String.Empty
            Select Case action
                Case Is = "add"
                    returnVal = "A"
                Case Is = "delete"
                    returnVal = "D"
            End Select
            Return returnVal
        End Function
        Public Function SetBooleanFromYN(ByVal yOrN As String, Optional ByVal defaultBool As Boolean = False) As Boolean
            Dim returnVal As Boolean = defaultBool
            Select Case yOrN.ToUpper
                Case Is = "Y"
                    returnVal = True
                Case Is = "N"
                    returnVal = False
            End Select
            Return returnVal
        End Function

        Public Sub getProgress()
            Dim conTalent As SqlConnection
            Dim param1 As String = "@Param1"
            Const SqlServer2005 As String = "SqlServer2005"


            Const strSelect1 As String = "SELECT * FROM tbl_supplynet_requests WHERE Transaction_ID = @Param1"

            Try
                conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                conTalent.Open()
                Dim cmdSelect As SqlCommand = New SqlCommand(strSelect1, conTalent)
                cmdSelect.Parameters.Add(New SqlParameter(param1, SqlDbType.Char, 50)).Value = ProgressTransactionID
                Dim reader As SqlDataReader = cmdSelect.ExecuteReader()
                If reader.HasRows Then
                    reader.Read()
                    TotalRecords = reader.Item("Total_Records")
                    ProcessedRecords = reader.Item("Processed_Records")
                    TransactionEndDate = reader.Item("Transaction_End_Date")
                End If
                conTalent.Close()
            Catch ex As Exception

            End Try

        End Sub

        Public Sub createSupplyNetRequest(ByVal Partner As String, _
                                            ByVal LoginID As String, _
                                            ByVal RequestModule As String, _
                                            ByVal TransactionID As String, _
                                            ByVal TotalRecords As Integer, _
                                            ByVal ProcessedRecords As Integer, _
                                            ByVal TransactionStartDate As DateTime, _
                                            ByVal TransactionEndDate As DateTime, _
                                            ByVal Overwrite As Boolean)
            Dim dbAccess As New DBAccess
            With dbAccess
                .Settings = Settings
            End With
            dbAccess.createSupplyNetRequest(Partner, LoginID, RequestModule, TransactionID, TotalRecords, ProcessedRecords, TransactionStartDate, TransactionEndDate, Overwrite)
        End Sub

        Public Sub updateSupplyNetProgressCount(ByVal TransactionId As Integer, ByVal count As Integer)
            Dim dbAccess As New DBAccess
            With dbAccess
                .Settings = Settings
            End With
            dbAccess.updateSupplyNetProgressCount(TransactionId, count)
        End Sub

        Public Function incrementSupplyNetProgressCount(ByVal TransactionID As String) As Integer
            Dim dbAccess As New DBAccess
            With dbAccess
                .Settings = Settings
            End With
            Return dbAccess.incrementSupplyNetProgressCount(TransactionID)
        End Function

        Public Sub updateSupplyNetProgressCount(ByVal TransactionID As String, ByVal Count As Integer)
            Dim dbAccess As New DBAccess
            With dbAccess
                .Settings = Settings
            End With
            dbAccess.updateSupplyNetProgressCount(TransactionID, Count)
        End Sub

        Public Sub markSupplyNetTransactionAsCompleted(ByVal TransactionID As String)
            Dim dbAccess As New DBAccess
            With dbAccess
                .Settings = Settings
            End With
            dbAccess.markSupplyNetTransactionAsCompleted(TransactionID)
        End Sub

    End Class

End Namespace