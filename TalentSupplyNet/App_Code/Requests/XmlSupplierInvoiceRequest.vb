Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Supplier Invoice  
'
'       Date                        Mar 2007
'
'       Author                      Andy White
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSSI- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal
    Public Class XmlSupplierInvoiceRequest
        Inherits XmlRequest

        Private _din As DESupplier
        Public Property Din() As DESupplier
            Get
                Return _din
            End Get
            Set(ByVal value As DESupplier)
                _din = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            Dim xmlAction As XmlSupplierInvoiceResponse = CType(xmlResp, XmlSupplierInvoiceResponse)
            Dim invoicing As New TalentSupplier()
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            If Not err.HasError Then
                With invoicing
                    .Dep = Din
                    .Settings = Settings
                    err = .WriteInvoice
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                .ResultDataSet = invoicing.ResultDataSet
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim Node1, Node2, Node3, Node4, Node5 As XmlNode
            Dim detr As New DETransaction           ' Items
            Dim deoh As DESupplierInvoice = Nothing
            Dim deol As DESupplierInvoiceLines = Nothing
            Dim InvoiceNumber As String = String.Empty
            Din = New DESupplier
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//SupplierInvoiceRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Din.CollDETrans.Add(Extract_TransactionHeader(Node1))

                        Case Is = "Invoices"
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "Invoice"
                                        deoh = New DESupplierInvoice
                                        For Each Node3 In Node2.ChildNodes
                                            Select Case Node3.Name
                                                Case Is = "InvoiceHeader"
                                                    '----------------------------------------------------------
                                                    '<InvoiceHeader>
                                                    '	<VendorNumber></VendorNumber>
                                                    '	<InvoiceNumber></InvoiceNumber>
                                                    '	<InvoiceDate></InvoiceDate>
                                                    '	<PurchaseOrderNumber></PurchaseOrderNumber>
                                                    '	<InvoiceAmount></InvoiceAmount>
                                                    '	<VATAmount></VATAmount>
                                                    '	<GrossAmount></GrossAmount>
                                                    '	<InvoicedProcessed></InvoicedProcessed>
                                                    '	<CompanyCode></CompanyCode>
                                                    '	<CurrencyCode></CurrencyCode>
                                                    '</InvoiceHeader>
                                                    '----------------------------------------------------------
                                                    deoh = New DESupplierInvoice
                                                    With deoh
                                                        For Each Node4 In Node3.ChildNodes
                                                            Select Case Node4.Name
                                                                Case Is = "CompanyCode" : .CompanyCode = Node4.InnerText
                                                                Case Is = "CurrencyCode" : .CurrencyCode = Node4.InnerText
                                                                Case Is = "GrossAmount" : .GrossAmount = Node4.InnerText
                                                                Case Is = "InvoiceAmount" : .InvoiceAmount = Node4.InnerText
                                                                Case Is = "InvoiceDate" : .InvoiceDate = Node4.InnerText
                                                                Case Is = "InvoiceNumber" : .InvoiceNumber = Node4.InnerText
                                                                Case Is = "InvoicedProcessed" : .InvoicedProcessed = Node4.InnerText
                                                                Case Is = "PurchaseOrderNumber" : .PurchaseOrderNumber = Node4.InnerText
                                                                Case Is = "VATAmount" : .VATAmount = Node4.InnerText
                                                                Case Is = "VendorNumber" : .VendorNumber = Node4.InnerText
                                                                Case Is = "CustomerOrderReference" : .CustomerOrderReference = Node4.InnerText
                                                                Case Is = "OrderNumber" : .OrderNumber = Node4.InnerText
                                                                Case Is = "DespatchDate" : .DespatchDate = Node4.InnerText
                                                                Case Is = "DeliveryNoteNumber" : .DeliveryNoteNumber = Node4.InnerText
                                                                Case Is = "ProofOfDelivery" : .ProofOfDelivery = Node4.InnerText
                                                                Case Is = "CustomerOrderDate" : .CustomerOrderDate = Node4.InnerText
                                                                Case Is = "DeliveryName" : .DeliveryName = Node4.InnerText
                                                                Case Is = "DeliveryAddress1" : .DeliveryAddress1 = Node4.InnerText
                                                                Case Is = "DeliveryAddress2" : .DeliveryAddress2 = Node4.InnerText
                                                                Case Is = "DeliveryAddress3" : .DeliveryAddress3 = Node4.InnerText
                                                                Case Is = "DeliveryAddress4" : .DeliveryAddress4 = Node4.InnerText
                                                                Case Is = "DeliveryAddress5" : .DeliveryAddress5 = Node4.InnerText
                                                                Case Is = "DeliveryPostcode" : .DeliveryPostcode = Node4.InnerText
                                                                Case Is = "PaymentMethod" : .PaymentMethod = Node4.InnerText
                                                                Case Is = "SettlementDiscount" : .SettlementDiscount = Node4.InnerText


                                                            End Select
                                                        Next Node4
                                                        InvoiceNumber = .InvoiceNumber
                                                        Din.collDEHeader.Add(deoh)
                                                    End With

                                                Case Is = "InvoiceLines"
                                                    '----------------------------------------------------------
                                                    '<InvoiceLine LineNumber="1">
                                                    '   <QuantityInvoiced></QuantityInvoiced>
                                                    '   <UnitOfMeasure></UnitOfMeasure>
                                                    '   <InvoiceLineNetAmount></InvoiceLineNetAmount>
                                                    '   <InvoiceLineVATAmount></InvoiceLineVATAmount>
                                                    '   <ProductCode></ProductCode>
                                                    '   <CompanyCode></CompanyCode>
                                                    '   <VATCode></VATCode>
                                                    '   <LocationCode></LocationCode>
                                                    '   <CurrencyCode></CurrencyCode>
                                                    '</InvoiceLine>
                                                    '----------------------------------------------------------
                                                    For Each Node4 In Node3.ChildNodes
                                                        Select Case Node4.Name
                                                            Case Is = "InvoiceLine"
                                                                deol = New DESupplierInvoiceLines
                                                                With deol
                                                                    .InvoiceNumber = InvoiceNumber
                                                                    .InvoiceLine = Node4.Attributes("LineNumber").Value
                                                                    For Each Node5 In Node4.ChildNodes
                                                                        Select Case Node5.Name
                                                                            Case Is = "CompanyCode" : .CompanyCode = Node5.InnerText
                                                                            Case Is = "CurrencyCode" : .CurrencyCode = Node5.InnerText
                                                                            Case Is = "InvoiceLineNetAmount" : .InvoiceLineNetAmount = Node5.InnerText
                                                                            Case Is = "InvoiceLineVATAmount" : .InvoiceLineVatAmount = Node5.InnerText
                                                                            Case Is = "LocationCode" : .LocationCode = Node5.InnerText
                                                                            Case Is = "QuantityInvoiced" : .QuantityInvoiced = Node5.InnerText
                                                                            Case Is = "ProductCode" : .ProductCode = Node5.InnerText
                                                                            Case Is = "UnitOfMeasure" : .UnitOfMeasure = Node5.InnerText
                                                                            Case Is = "VATCode" : .VATCode = Node5.InnerText
                                                                            Case Is = "Description" : .Description = Node5.InnerText
                                                                            Case Is = "DespatchTime" : .DespatchTime = Node5.InnerText
                                                                            Case Is = "VATRate" : .VATRate = Node5.InnerText
                                                                            Case Is = "UnitCostPrice" : .UnitCostPrice = Node5.InnerText
                                                                        End Select
                                                                    Next Node5
                                                                    Din.collDEInfo.Add(deol)
                                                                End With
                                                        End Select
                                                    Next Node4
                                                    '----------------------------------------------------------------------------------------------
                                            End Select
                                        Next Node3
                                End Select
                            Next Node2
                    End Select
                Next Node1

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRSSI-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function


    End Class

End Namespace