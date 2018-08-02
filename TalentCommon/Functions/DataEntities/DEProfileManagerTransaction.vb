'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Profile Manager transactions
'
'       Date                        24/11/08 
'
'       Author                      Ben Ford
'
'       CS Group 2007               All rights reserved.
'
'       Error Number Code base      
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEProfileManagerTransactions

    Private _collDETrans As New Collection          ' Transaction details
    Private _collDEHeader As New Collection

    Public Property CollDETrans() As Collection
        Get
            Return _collDETrans
        End Get
        Set(ByVal value As Collection)
            _collDETrans = value
        End Set
    End Property

    Public Property CollDEHeader() As Collection
        Get
            Return _collDEHeader
        End Get
        Set(ByVal value As Collection)
            _collDEHeader = value
        End Set
    End Property

End Class

Public Class DEProfileManagerTransaction
    '
    Private _HeaderID As String
    Public Property HeaderID() As String
        Get
            Return _HeaderID
        End Get
        Set(ByVal value As String)
            _HeaderID = value
        End Set
    End Property

    Private _sourceSystemID As String
    Public Property SourceSystemID() As String
        Get
            Return _sourceSystemID
        End Get
        Set(ByVal value As String)
            _sourceSystemID = value
        End Set
    End Property

    Private _sourceRecordType As String
    Public Property SourceRecordType() As String
        Get
            Return _sourceRecordType
        End Get
        Set(ByVal value As String)
            _sourceRecordType = value
        End Set
    End Property

    Private _recordEntryDate As String
    Public Property RecordEntryDate() As String
        Get
            Return _recordEntryDate
        End Get
        Set(ByVal value As String)
            _recordEntryDate = value
        End Set
    End Property

    Private _recordEntryTime As String
    Public Property RecordEntryTime() As String
        Get
            Return _recordEntryTime
        End Get
        Set(ByVal value As String)
            _recordEntryTime = value
        End Set
    End Property

    Private _recordEntryMethod As String
    Public Property RecordEntryMethod() As String
        Get
            Return _recordEntryMethod
        End Get
        Set(ByVal value As String)
            _recordEntryMethod = value
        End Set
    End Property

    Private _headerUnitPrice As String
    Public Property HeaderUnitPrice() As String
        Get
            Return _headerUnitPrice
        End Get
        Set(ByVal value As String)
            _headerUnitPrice = value
        End Set
    End Property

    Private _headerTotalPrice As String
    Public Property HeaderTotalPrice() As String
        Get
            Return _headerTotalPrice
        End Get
        Set(ByVal value As String)
            _headerTotalPrice = value
        End Set
    End Property

    Private _headerVATValue As String
    Public Property HeaderVatValue() As String
        Get
            Return _headerVATValue
        End Get
        Set(ByVal value As String)
            _headerVATValue = value
        End Set
    End Property

    Private _headerMargin As String
    Public Property HeaderMargin() As String
        Get
            Return _headerMargin
        End Get
        Set(ByVal value As String)
            _headerMargin = value
        End Set
    End Property

    Private _headerTotalQuantity As String
    Public Property HeaderTotalQuantity() As String
        Get
            Return _headerTotalQuantity
        End Get
        Set(ByVal value As String)
            _headerTotalQuantity = value
        End Set
    End Property

    Private _sourceCustomerId As String
    Public Property SourceCustomerID() As String
        Get
            Return _sourceCustomerId
        End Get
        Set(ByVal value As String)
            _sourceCustomerId = value
        End Set
    End Property

    Private _talentCustomerId As String
    Public Property TalentCustomerId() As String
        Get
            Return _talentCustomerId
        End Get
        Set(ByVal value As String)
            _talentCustomerId = value
        End Set
    End Property

    Private _talentContactId As String
    Public Property TalentContactId() As String
        Get
            Return _talentContactId
        End Get
        Set(ByVal value As String)
            _talentContactId = value
        End Set
    End Property

    Private _memberNo As String
    Public Property MemberNo() As String
        Get
            Return _memberNo
        End Get
        Set(ByVal value As String)
            _memberNo = value
        End Set
    End Property

    Private _noteType As String
    Public Property NoteType() As String
        Get
            Return _noteType
        End Get
        Set(ByVal value As String)
            _noteType = value
        End Set
    End Property

    Private _actionType As String
    Public Property ActionType() As String
        Get
            Return _actionType
        End Get
        Set(ByVal value As String)
            _actionType = value
        End Set
    End Property

    Private _attribute1 As String
    Public Property Attribute1() As String
        Get
            Return _attribute1
        End Get
        Set(ByVal value As String)
            _attribute1 = value
        End Set
    End Property

    Private _attribute2 As String
    Public Property Attribute2() As String
        Get
            Return _attribute2
        End Get
        Set(ByVal value As String)
            _attribute2 = value
        End Set
    End Property

    Private _attribute3 As String
    Public Property Attribute3() As String
        Get
            Return _attribute3
        End Get
        Set(ByVal value As String)
            _attribute3 = value
        End Set
    End Property

    Private _attribute4 As String
    Public Property Attribute4() As String
        Get
            Return _attribute4
        End Get
        Set(ByVal value As String)
            _attribute4 = value
        End Set
    End Property

    Private _attribute5 As String
    Public Property Attribute5() As String
        Get
            Return _attribute5
        End Get
        Set(ByVal value As String)
            _attribute5 = value
        End Set
    End Property

    Private _attribute6 As String
    Public Property Attribute6() As String
        Get
            Return _attribute6
        End Get
        Set(ByVal value As String)
            _attribute6 = value
        End Set
    End Property

    Private _attribute7 As String
    Public Property Attribute7() As String
        Get
            Return _attribute7
        End Get
        Set(ByVal value As String)
            _attribute7 = value
        End Set
    End Property

    Private _attribute8 As String
    Public Property Attribute8() As String
        Get
            Return _attribute8
        End Get
        Set(ByVal value As String)
            _attribute8 = value
        End Set
    End Property

    Private _attribute9 As String
    Public Property Attribute9() As String
        Get
            Return _attribute9
        End Get
        Set(ByVal value As String)
            _attribute9 = value
        End Set
    End Property

    Private _attribute10 As String
    Public Property Attribute10() As String
        Get
            Return _attribute10
        End Get
        Set(ByVal value As String)
            _attribute10 = value
        End Set
    End Property

    Private _colTransactionLines As Collection
    Public Property ColTransactionLines() As Collection
        Get
            Return _colTransactionLines
        End Get
        Set(ByVal value As Collection)
            _colTransactionLines = value
        End Set
    End Property

End Class

Public Class DEProfileManagerTransactionLine

    Private _detailHeaderId As String
    Public Property DetailHeaderId() As String
        Get
            Return _detailHeaderId
        End Get
        Set(ByVal value As String)
            _detailHeaderId = value
        End Set
    End Property

    Private _detailSourceSystemId As String
    Public Property DetailSourceSystemId() As String
        Get
            Return _detailSourceSystemId
        End Get
        Set(ByVal value As String)
            _detailSourceSystemId = value
        End Set
    End Property

    Private _detailSourceRecordType As String
    Public Property DetailSourceRecordType() As String
        Get
            Return _detailSourceRecordType
        End Get
        Set(ByVal value As String)
            _detailSourceRecordType = value
        End Set
    End Property

    Private _type As String
    Public Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
        End Set
    End Property

    Private _lineDate As String
    Public Property LineDate() As String
        Get
            Return _lineDate
        End Get
        Set(ByVal value As String)
            _lineDate = value
        End Set
    End Property

    Private _lineTime As String
    Public Property LineTime() As String
        Get
            Return _lineTime
        End Get
        Set(ByVal value As String)
            _lineTime = value
        End Set
    End Property

    Private _agent As String
    Public Property Agent() As String
        Get
            Return _agent
        End Get
        Set(ByVal value As String)
            _agent = value
        End Set
    End Property

    Private _saleLocation As String
    Public Property SaleLocation() As String
        Get
            Return _saleLocation
        End Get
        Set(ByVal value As String)
            _saleLocation = value
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

    Private _productCategory1 As String
    Public Property ProductCategory1() As String
        Get
            Return _productCategory1
        End Get
        Set(ByVal value As String)
            _productCategory1 = value
        End Set
    End Property

    Private _productCategory2 As String
    Public Property ProductCategory2() As String
        Get
            Return _productCategory2
        End Get
        Set(ByVal value As String)
            _productCategory2 = value
        End Set
    End Property

    Private _productCategory3 As String
    Public Property ProductCategory3() As String
        Get
            Return _productCategory3
        End Get
        Set(ByVal value As String)
            _productCategory3 = value
        End Set
    End Property

    Private _productCategory4 As String
    Public Property ProductCategory4() As String
        Get
            Return _productCategory4
        End Get
        Set(ByVal value As String)
            _productCategory4 = value
        End Set
    End Property

    Private _productCategory5 As String
    Public Property ProductCategory5() As String
        Get
            Return _productCategory5
        End Get
        Set(ByVal value As String)
            _productCategory5 = value
        End Set
    End Property

    Private _productCategory6 As String
    Public Property ProductCategory6() As String
        Get
            Return _productCategory6
        End Get
        Set(ByVal value As String)
            _productCategory6 = value
        End Set
    End Property

    Private _productDescription As String
    Public Property ProductDescription() As String
        Get
            Return _productDescription
        End Get
        Set(ByVal value As String)
            _productDescription = value
        End Set
    End Property

    Private _productSupplier As String
    Public Property ProductSupplier() As String
        Get
            Return _productSupplier
        End Get
        Set(ByVal value As String)
            _productSupplier = value
        End Set
    End Property

    Private _quantity As String
    Public Property Quantity() As String
        Get
            Return _quantity
        End Get
        Set(ByVal value As String)
            _quantity = value
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

    Private _totalPrice As String
    Public Property TotalPrice() As String
        Get
            Return _totalPrice
        End Get
        Set(ByVal value As String)
            _totalPrice = value
        End Set
    End Property

    Private _VatValue As String
    Public Property VatValue() As String
        Get
            Return _VatValue
        End Get
        Set(ByVal value As String)
            _VatValue = value
        End Set
    End Property

    Private _lineNumber As String
    Public Property LineNumber() As String
        Get
            Return _lineNumber
        End Get
        Set(ByVal value As String)
            _lineNumber = value
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

    Private _creditCardType As String
    Public Property CreditCardType() As String
        Get
            Return _creditCardType
        End Get
        Set(ByVal value As String)
            _creditCardType = value
        End Set
    End Property

    Private _margin As String
    Public Property Margin() As String
        Get
            Return _margin
        End Get
        Set(ByVal value As String)
            _margin = value
        End Set
    End Property

    Private _UOM As String
    Public Property UOM() As String
        Get
            Return _UOM
        End Get
        Set(ByVal value As String)
            _UOM = value
        End Set
    End Property

    Private _conversionFactor As String
    Public Property ConversionFactor() As String
        Get
            Return _conversionFactor
        End Get
        Set(ByVal value As String)
            _conversionFactor = value
        End Set
    End Property

    Private _currency As String
    Public Property Currency() As String
        Get
            Return _currency
        End Get
        Set(ByVal value As String)
            _currency = value
        End Set
    End Property

    Private _campaign As String
    Public Property Campaign() As String
        Get
            Return _campaign
        End Get
        Set(ByVal value As String)
            _campaign = value
        End Set
    End Property

    Private _campaignCode As String
    Public Property CampaignCode() As String
        Get
            Return _campaignCode
        End Get
        Set(ByVal value As String)
            _campaignCode = value
        End Set
    End Property

    Private _eventCode As String
    Public Property EventCode() As String
        Get
            Return _eventCode
        End Get
        Set(ByVal value As String)
            _eventCode = value
        End Set
    End Property

    Private _specificDetail As String
    Public Property SpecificDetail() As String
        Get
            Return _specificDetail
        End Get
        Set(ByVal value As String)
            _specificDetail = value
        End Set
    End Property

    Private _discountValue As String
    Public Property DiscountValue() As String
        Get
            Return _discountValue
        End Get
        Set(ByVal value As String)
            _discountValue = value
        End Set
    End Property

    Private _noteType As String
    Public Property NoteType() As String
        Get
            Return _noteType
        End Get
        Set(ByVal value As String)
            _noteType = value
        End Set
    End Property

    Private _actionType As String
    Public Property ActionType() As String
        Get
            Return _actionType
        End Get
        Set(ByVal value As String)
            _actionType = value
        End Set
    End Property

End Class

