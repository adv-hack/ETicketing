'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Supplier invoice requests and responses
'
'       Date                        Mar 2007
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDESI- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DESupplier

    Private _collDETrans As New Collection          ' Transaction details
    Private _collDEHeader As New Collection
    Private _collDEInfo As New Collection

    Public Property CollDETrans() As Collection
        Get
            Return _collDETrans
        End Get
        Set(ByVal value As Collection)
            _collDETrans = value
        End Set
    End Property
    Public Property collDEHeader() As Collection
        Get
            Return _collDEHeader
        End Get
        Set(ByVal value As Collection)
            _collDEHeader = value
        End Set
    End Property
    Public Property collDEInfo() As Collection
        Get
            Return _collDEInfo
        End Get
        Set(ByVal value As Collection)
            _collDEInfo = value
        End Set
    End Property

End Class

Public Class DESupplierInvoice
    '
    Private _companyCode As String = String.Empty
    Private _currencyCode As String = String.Empty
    Private _grossAmount As Double = 0
    Private _invoiceAmount As Double = 0
    Private _invoiceDate As String = String.Empty
    Private _invoiceNumber As String = String.Empty
    Private _invoicedProcessed As String = String.Empty
    Private _purchaseOrderNumber As String = String.Empty
    Private _vatAmount As String = String.Empty
    Private _vendorNumber As String = String.Empty

    Public Property CompanyCode() As String
        Get
            Return _companyCode
        End Get
        Set(ByVal value As String)
            _companyCode = value
        End Set
    End Property
    Public Property CurrencyCode() As String
        Get
            Return _currencyCode
        End Get
        Set(ByVal value As String)
            _currencyCode = value
        End Set
    End Property
    Public Property GrossAmount() As String
        Get
            Return _grossAmount
        End Get
        Set(ByVal value As String)
            _grossAmount = value
        End Set
    End Property
    Public Property InvoiceAmount() As Double
        Get
            Return _invoiceAmount
        End Get
        Set(ByVal value As Double)
            _invoiceAmount = value
        End Set
    End Property
    Public Property InvoiceDate() As String
        Get
            Return _invoiceDate
        End Get
        Set(ByVal value As String)
            _invoiceDate = value
        End Set
    End Property
    Public Property InvoiceNumber() As String
        Get
            Return _invoiceNumber
        End Get
        Set(ByVal value As String)
            _invoiceNumber = value
        End Set
    End Property
    Public Property InvoicedProcessed() As String
        Get
            Return _invoicedProcessed
        End Get
        Set(ByVal value As String)
            _invoicedProcessed = value
        End Set
    End Property
    Public Property PurchaseOrderNumber() As String
        Get
            Return _purchaseOrderNumber
        End Get
        Set(ByVal value As String)
            _purchaseOrderNumber = value
        End Set
    End Property
    Public Property VATAmount() As String
        Get
            Return _vatAmount
        End Get
        Set(ByVal value As String)
            _vatAmount = value
        End Set
    End Property
    Public Property VendorNumber() As String
        Get
            Return _vendorNumber
        End Get
        Set(ByVal value As String)
            _vendorNumber = value
        End Set
    End Property

    Private _customerOrderReference As String
    Public Property CustomerOrderReference() As String
        Get
            Return _customerOrderReference
        End Get
        Set(ByVal value As String)
            _customerOrderReference = value
        End Set
    End Property

    Private _orderNumber As String
    Public Property OrderNumber() As String
        Get
            Return _orderNumber
        End Get
        Set(ByVal value As String)
            _orderNumber = value
        End Set
    End Property

    Private _DespatchDate As String
    Public Property DespatchDate() As String
        Get
            Return _DespatchDate
        End Get
        Set(ByVal value As String)
            _DespatchDate = value
        End Set
    End Property

    Private _deliveryNoteNumber As String
    Public Property DeliveryNoteNumber() As String
        Get
            Return _deliveryNoteNumber
        End Get
        Set(ByVal value As String)
            _deliveryNoteNumber = value
        End Set
    End Property

    Private _proofOfDelivery As String
    Public Property ProofOfDelivery() As String
        Get
            Return _proofOfDelivery
        End Get
        Set(ByVal value As String)
            _proofOfDelivery = value
        End Set
    End Property

    Private _customerOrderDate As String
    Public Property CustomerOrderDate() As String
        Get
            Return _customerOrderDate
        End Get
        Set(ByVal value As String)
            _customerOrderDate = value
        End Set
    End Property

    Private _deliveryName As String
    Public Property DeliveryName() As String
        Get
            Return _deliveryName
        End Get
        Set(ByVal value As String)
            _deliveryName = value
        End Set
    End Property

    Private _deliveryAddress1 As String
    Public Property DeliveryAddress1() As String
        Get
            Return _deliveryAddress1
        End Get
        Set(ByVal value As String)
            _deliveryAddress1 = value
        End Set
    End Property

    Private _deliveryAddress2 As String
    Public Property DeliveryAddress2() As String
        Get
            Return _deliveryAddress2
        End Get
        Set(ByVal value As String)
            _deliveryAddress2 = value
        End Set
    End Property

    Private _deliveryAddress3 As String
    Public Property DeliveryAddress3() As String
        Get
            Return _deliveryAddress3
        End Get
        Set(ByVal value As String)
            _deliveryAddress3 = value
        End Set
    End Property

    Private _deliveryAddress4 As String
    Public Property DeliveryAddress4() As String
        Get
            Return _deliveryAddress4
        End Get
        Set(ByVal value As String)
            _deliveryAddress4 = value
        End Set
    End Property

    Private _deliveryAddress5 As String
    Public Property DeliveryAddress5() As String
        Get
            Return _deliveryAddress5
        End Get
        Set(ByVal value As String)
            _deliveryAddress5 = value
        End Set
    End Property

    Private _deliveryPostcode As String
    Public Property DeliveryPostcode() As String
        Get
            Return _deliveryPostcode
        End Get
        Set(ByVal value As String)
            _deliveryPostcode = value
        End Set
    End Property

    Private _paymentMethod As String
    Public Property PaymentMethod() As String
        Get
            Return _paymentMethod
        End Get
        Set(ByVal value As String)
            _paymentMethod = value
        End Set
    End Property

    Private _settlementDiscount As String
    Public Property SettlementDiscount() As String
        Get
            Return _settlementDiscount
        End Get
        Set(ByVal value As String)
            _settlementDiscount = value
        End Set
    End Property

End Class

Public Class DESupplierInvoiceLines
    '
    Private _companyCode As String = String.Empty
    Private _currencyCode As String = String.Empty
    Private _invoiceLine As String = String.Empty
    Private _invoiceLineLocationCode As String = String.Empty
    Private _invoiceLineNetAmount As Double = 0
    Private _invoiceLineVatAmount As Double = 0
    Private _invoiceNumber As String = String.Empty
    Private _locationCode As String = String.Empty
    Private _quantityInvoiced As String = String.Empty
    Private _productCode As String = String.Empty
    Private _unitOfMeasure As String = String.Empty
    Private _vatCode As String = String.Empty
    Private _vendorNumber As String = String.Empty

    Public Property CompanyCode() As String
        Get
            Return _companyCode
        End Get
        Set(ByVal value As String)
            _companyCode = value
        End Set
    End Property
    Public Property CurrencyCode() As String
        Get
            Return _currencyCode
        End Get
        Set(ByVal value As String)
            _currencyCode = value
        End Set
    End Property
    Public Property InvoiceLine() As String
        Get
            Return _invoiceLine
        End Get
        Set(ByVal value As String)
            _invoiceLine = value
        End Set
    End Property
    Public Property InvoiceLineLocationCode() As String
        Get
            Return _invoiceLineLocationCode
        End Get
        Set(ByVal value As String)
            _invoiceLineLocationCode = value
        End Set
    End Property
    Public Property InvoiceLineNetAmount() As Double
        Get
            Return _invoiceLineNetAmount
        End Get
        Set(ByVal value As Double)
            _invoiceLineNetAmount = value
        End Set
    End Property
    Public Property InvoiceLineVatAmount() As Double
        Get
            Return _invoiceLineVatAmount
        End Get
        Set(ByVal value As Double)
            _invoiceLineVatAmount = value
        End Set
    End Property
    Public Property InvoiceNumber() As String
        Get
            Return _invoiceNumber
        End Get
        Set(ByVal value As String)
            _invoiceNumber = value
        End Set
    End Property
    Public Property LocationCode() As String
        Get
            Return _locationCode
        End Get
        Set(ByVal value As String)
            _locationCode = value
        End Set
    End Property
    Public Property QuantityInvoiced() As String
        Get
            Return _quantityInvoiced
        End Get
        Set(ByVal value As String)
            _quantityInvoiced = value
        End Set
    End Property
    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property
    Public Property UnitOfMeasure() As String
        Get
            Return _unitOfMeasure
        End Get
        Set(ByVal value As String)
            _unitOfMeasure = value
        End Set
    End Property
    Public Property VATCode() As String
        Get
            Return _vatCode
        End Get
        Set(ByVal value As String)
            _vatCode = value
        End Set
    End Property
    Public Property VendorNumber() As String
        Get
            Return _vendorNumber
        End Get
        Set(ByVal value As String)
            _vendorNumber = value
        End Set
    End Property

    Private _description As String
    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Private _despatchTime As String
    Public Property DespatchTime() As String
        Get
            Return _despatchTime
        End Get
        Set(ByVal value As String)
            _despatchTime = value
        End Set
    End Property

    Private _VATRate As String
    Public Property VATRate() As String
        Get
            Return _VATRate
        End Get
        Set(ByVal value As String)
            _VATRate = value
        End Set
    End Property

    Private _unitCostPrice As String
    Public Property UnitCostPrice() As String
        Get
            Return _unitCostPrice
        End Get
        Set(ByVal value As String)
            _unitCostPrice = value
        End Set
    End Property
End Class

Public Class DESupplierOrderAcknowledgement

    Private _companyCode As String
    Public Property CompanyCode() As String
        Get
            Return _companyCode
        End Get
        Set(ByVal value As String)
            _companyCode = value
        End Set
    End Property

    Private _vendorNumber As String
    Public Property VendorNumber() As String
        Get
            Return _vendorNumber
        End Get
        Set(ByVal value As String)
            _vendorNumber = value
        End Set
    End Property

    Private _PoNumber As String
    Public Property PoNumber() As String
        Get
            Return _PoNumber
        End Get
        Set(ByVal value As String)
            _PoNumber = value
        End Set
    End Property

    Private _despatchDate As String
    Public Property DespatchDate() As String
        Get
            Return _despatchDate
        End Get
        Set(ByVal value As String)
            _despatchDate = value
        End Set
    End Property

End Class

Public Class DESupplierOrderAcknowledgementLines

    Private _companyCode As String
    Public Property CompanyCode() As String
        Get
            Return _companyCode
        End Get
        Set(ByVal value As String)
            _companyCode = value
        End Set
    End Property

    Private newPropertyValue As Integer
    Public Property NewProperty() As Integer
        Get
            Return newPropertyValue
        End Get
        Set(ByVal value As Integer)
            newPropertyValue = value
        End Set
    End Property
    Private _vendorNumber As String
    Public Property VendorNumber() As String
        Get
            Return _vendorNumber
        End Get
        Set(ByVal value As String)
            _vendorNumber = value
        End Set
    End Property

    Private _PoNumber As String
    Public Property PoNumber() As String
        Get
            Return _PoNumber
        End Get
        Set(ByVal value As String)
            _PoNumber = value
        End Set
    End Property

    Private _lineNumber As Integer
    Public Property LineNumber() As Integer
        Get
            Return _lineNumber
        End Get
        Set(ByVal value As Integer)
            _lineNumber = value
        End Set
    End Property

    Private _productCode As String
    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property

    Private _orderQuantity As Integer
    Public Property OrderQuantity() As Integer
        Get
            Return _orderQuantity
        End Get
        Set(ByVal value As Integer)
            _orderQuantity = value
        End Set
    End Property

    Private _unitPrice As String
    Public Property UnitPrice() As String
        Get
            Return _unitPrice
        End Get
        Set(ByVal value As String)
            _unitPrice = value
        End Set
    End Property

    Private _deliveryDate As String
    Public Property DeliveryDate() As String
        Get
            Return _deliveryDate
        End Get
        Set(ByVal value As String)
            _deliveryDate = value
        End Set
    End Property







End Class
