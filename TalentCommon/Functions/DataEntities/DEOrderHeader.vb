Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order headers
'
'       Date                        1st Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEOH- 
'                                   application.code(3) + object code(4) + number(2)
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DeOrderHeader
    '---------------------------------------------------------------------------------
    '   Addressing Information 
    '
    Private _branchOrderNumber As String        ' 2048043
    Private _category As String                 ' Order Detail, RMA Status       
    Private _customerPO As String               ' CustomerPO_1
    Private _newCustomerPO As String            ' CustomerPO_1
    Private _endUserPO As String                ' EndUserPO_1
    Private _orderSuffix As String              ' 21
    Private _orderActionCode As String          ' R
    Private _billToSuffix As String             ' 100
    Private _shipToSuffix As String             ' 200
    Private _webOrderNumber As String           ' 200
    Private _customerNumber As Decimal
    Private _customerNumberPrefix As String
    Private _basketNumber As String
    Private _promotionCode As String
    Private _promotionCodeDescription As String
    '---------------------------------------------------------------------------------
    '   Processing Options 
    '
    Private _carrierCode As String              ' CT
    Private _carrierCodeValue As String         '
    Private _carrierCodeVAT As Decimal
    Private _carrierCodeNet As Decimal
    Private _autoRelease As String              ' 0
    Private _salesPerson As String              '
    Private _orderDueDate As String             '
    Private _suspendCode As String              '

    '---------------------------------------------------------------------------------
    '   Shipment Options 
    '
    Private _backOrderFlag As String            ' Y
    Private _splitShipmentFlag As String        ' N
    Private _splitLine As String                ' N
    Private _shipFromBranches As String         ' 20
    Private _shippingCode As String
    '---------------------------------------------------------------------------------
    '   Dynamic Message 
    '
    Private _messageLines As String             ' Please deliver to Mrs Jones
    Private _message As Boolean
    '
    Private _collDEAddress As New Collection
    Private _collDEPayments As New Collection       ' Order payment lines
    Private _collDECharges As New Collection        ' Order Charges lines

    '---------------------------------------------------------------------------------
    '  Costs
    '
    Private _totalOrderItemsValue As String
    Private _totalOrderItemsValueNet As String
    Private _totalOrderItemsValueTax As String
    Private _currency As String
    '
    '---------------------------------------------------------------------------------
    '  Tax
    '
    Private _taxInclusive1 As Boolean
    Private _taxInclusive2 As Boolean
    Private _taxInclusive3 As Boolean
    Private _taxInclusive4 As Boolean
    Private _taxInclusive5 As Boolean
    Private _taxDisplay1 As Boolean
    Private _taxDisplay2 As Boolean
    Private _taxDisplay3 As Boolean
    Private _taxDisplay4 As Boolean
    Private _taxDisplay5 As Boolean
    Private _taxCode1 As String
    Private _taxCode2 As String
    Private _taxCode3 As String
    Private _taxCode4 As String
    Private _taxCode5 As String
    Private _taxAmount1 As Decimal
    Private _taxAmount2 As Decimal
    Private _taxAmount3 As Decimal
    Private _taxAmount4 As Decimal
    Private _taxAmount5 As Decimal

    'To pass more than one promotion codes
    Public Property PromotionCodes() As String()



    Private _promoValue As Decimal
    Public Property PromotionValue() As Decimal
        Get
            Return _promoValue
        End Get
        Set(ByVal value As Decimal)
            _promoValue = value
        End Set
    End Property

    Private _totalValueCharged As Decimal
    Public Property TotalValueCharged() As Decimal
        Get
            Return _totalValueCharged
        End Get
        Set(ByVal value As Decimal)
            _totalValueCharged = value
        End Set
    End Property


    Private _projectedDate As Date
    Public Property ProjectedDeliveryDate() As Date
        Get
            Return _projectedDate
        End Get
        Set(ByVal value As Date)
            _projectedDate = value
        End Set
    End Property

    Private _paymentType As String
    Public Property PaymentType() As String
        Get
            Return _paymentType
        End Get
        Set(ByVal value As String)
            _paymentType = value
        End Set
    End Property

    Private _paymentSubType As String
    Public Property PaymentSubType() As String
        Get
            Return _paymentSubType
        End Get
        Set(ByVal value As String)
            _paymentSubType = value
        End Set
    End Property

    Public Property BranchOrderNumber() As String
        Get
            Return _branchOrderNumber
        End Get
        Set(ByVal value As String)
            _branchOrderNumber = value
        End Set
    End Property
    Public Property Category() As String
        Get
            Return _category
        End Get
        Set(ByVal value As String)
            _category = value
        End Set
    End Property
    Public Property CustomerPO() As String
        Get
            Return _customerPO
        End Get
        Set(ByVal value As String)
            _customerPO = value
        End Set
    End Property
    Public Property NewCustomerPO() As String
        Get
            Return _newCustomerPO
        End Get
        Set(ByVal value As String)
            _newCustomerPO = value
        End Set
    End Property
    Public Property EndUserPO() As String
        Get
            Return _endUserPO
        End Get
        Set(ByVal value As String)
            _endUserPO = value
        End Set
    End Property
    Public Property OrderSuffix() As String
        Get
            Return _orderSuffix
        End Get
        Set(ByVal value As String)
            _orderSuffix = value
        End Set
    End Property
    Public Property OrderActionCode() As String
        Get
            Return _orderActionCode
        End Get
        Set(ByVal value As String)
            _orderActionCode = value
        End Set
    End Property
    Public Property BillToSuffix() As String
        Get
            Return _billToSuffix
        End Get
        Set(ByVal value As String)
            _billToSuffix = value
        End Set
    End Property
    Public Property ShipToSuffix() As String
        Get
            Return _shipToSuffix
        End Get
        Set(ByVal value As String)
            _shipToSuffix = value
        End Set
    End Property
    Public Property WebOrderNumber() As String
        Get
            Return _webOrderNumber
        End Get
        Set(ByVal value As String)
            _webOrderNumber = value
        End Set
    End Property
    Public Property CarrierCode() As String
        Get
            Return _carrierCode
        End Get
        Set(ByVal value As String)
            _carrierCode = value
        End Set
    End Property
    Public Property CarrierCodeValue() As String
        Get
            Return _carrierCodeValue
        End Get
        Set(ByVal value As String)
            _carrierCodeValue = value
        End Set
    End Property
    Public Property CarrierCodeVAT() As Decimal
        Get
            Return _carrierCodeVAT
        End Get
        Set(ByVal value As Decimal)
            _carrierCodeVAT = value
        End Set
    End Property
    Public Property CarrierCodeNet() As Decimal
        Get
            Return _carrierCodeNet
        End Get
        Set(ByVal value As Decimal)
            _carrierCodeNet = value
        End Set
    End Property
    Public Property AutoRelease() As String
        Get
            Return _autoRelease
        End Get
        Set(ByVal value As String)
            _autoRelease = value
        End Set
    End Property
    Public Property SalesPerson() As String
        Get
            Return _salesPerson
        End Get
        Set(ByVal value As String)
            _salesPerson = value
        End Set
    End Property
    Public Property OrderDueDate() As String
        Get
            Return _orderDueDate
        End Get
        Set(ByVal value As String)
            _orderDueDate = value
        End Set
    End Property
    Public Property SuspendCode() As String
        Get
            Return _suspendCode
        End Get
        Set(ByVal value As String)
            _suspendCode = value
        End Set
    End Property
    Public Property BackOrderFlag() As String
        Get
            Return _backOrderFlag
        End Get
        Set(ByVal value As String)
            _backOrderFlag = value
        End Set
    End Property

    Public Property SplitShipmentFlag() As String
        Get
            Return _splitShipmentFlag
        End Get
        Set(ByVal value As String)
            _splitShipmentFlag = value
        End Set
    End Property
    Public Property SplitLine() As String
        Get
            Return _splitLine
        End Get
        Set(ByVal value As String)
            _splitLine = value
        End Set
    End Property
    Public Property ShipFromBranches() As String
        Get
            Return _shipFromBranches
        End Get
        Set(ByVal value As String)
            _shipFromBranches = value
        End Set
    End Property
    Public Property ShippingCode() As String
        Get
            Return _shippingCode
        End Get
        Set(ByVal value As String)
            _shippingCode = value
        End Set
    End Property
    Public Property MessageLines() As String
        Get
            Return _messageLines
        End Get
        Set(ByVal value As String)
            _messageLines = value
        End Set
    End Property
    Public Property Message() As Boolean
        Get
            Return _message
        End Get
        Set(ByVal value As Boolean)
            _message = value
        End Set
    End Property
    Public Property TotalOrderItemsValue() As String
        Get
            Return _totalOrderItemsValue
        End Get
        Set(ByVal value As String)
            _totalOrderItemsValue = value
        End Set
    End Property

    Public Property TotalOrderItemsValueNet() As String
        Get
            Return _totalOrderItemsValueNet
        End Get
        Set(ByVal value As String)
            _totalOrderItemsValueNet = value
        End Set
    End Property
    Public Property TotalOrderItemsValueTax() As String
        Get
            Return _totalOrderItemsValueTax
        End Get
        Set(ByVal value As String)
            _totalOrderItemsValueTax = value
        End Set
    End Property
    Public Property Currency() As String
        Get
            Return _currency
        End Get
        Set(ByVal value As String)
            _currency = value
        End Set
    End Property
    '
    Public Property CollDEAddress() As Collection
        Get
            Return _collDEAddress
        End Get
        Set(ByVal value As Collection)
            _collDEAddress = value
        End Set
    End Property
    Public Property CollDEPayments() As Collection
        Get
            Return _collDEPayments
        End Get
        Set(ByVal value As Collection)
            _collDEPayments = value
        End Set
    End Property
    Public Property CollDECharges() As Collection
        Get
            Return _collDECharges
        End Get
        Set(ByVal value As Collection)
            _collDECharges = value
        End Set
    End Property
    '
    Public Property TaxInclusive1() As Boolean
        Get
            Return _taxInclusive1
        End Get
        Set(ByVal value As Boolean)
            _taxInclusive1 = value
        End Set
    End Property
    Public Property TaxInclusive2() As Boolean
        Get
            Return _taxInclusive2
        End Get
        Set(ByVal value As Boolean)
            _taxInclusive2 = value
        End Set
    End Property
    Public Property TaxInclusive3() As Boolean
        Get
            Return _taxInclusive3
        End Get
        Set(ByVal value As Boolean)
            _taxInclusive3 = value
        End Set
    End Property
    Public Property TaxInclusive4() As Boolean
        Get
            Return _taxInclusive4
        End Get
        Set(ByVal value As Boolean)
            _taxInclusive4 = value
        End Set
    End Property
    Public Property TaxInclusive5() As Boolean
        Get
            Return _taxInclusive5
        End Get
        Set(ByVal value As Boolean)
            _taxInclusive5 = value
        End Set
    End Property
    Public Property TaxDisplay1() As Boolean
        Get
            Return _taxDisplay1
        End Get
        Set(ByVal value As Boolean)
            _taxDisplay1 = value
        End Set
    End Property
    Public Property TaxDisplay2() As Boolean
        Get
            Return _taxDisplay2
        End Get
        Set(ByVal value As Boolean)
            _taxDisplay2 = value
        End Set
    End Property
    Public Property TaxDisplay3() As Boolean
        Get
            Return _taxDisplay3
        End Get
        Set(ByVal value As Boolean)
            _taxDisplay3 = value
        End Set
    End Property
    Public Property TaxDisplay4() As Boolean
        Get
            Return _taxDisplay4
        End Get
        Set(ByVal value As Boolean)
            _taxDisplay4 = value
        End Set
    End Property
    Public Property TaxDisplay5() As Boolean
        Get
            Return _taxDisplay5
        End Get
        Set(ByVal value As Boolean)
            _taxDisplay5 = value
        End Set
    End Property
    Public Property TaxCode1() As String
        Get
            Return _taxCode1
        End Get
        Set(ByVal value As String)
            _taxCode1 = value
        End Set
    End Property
    Public Property TaxCode2() As String
        Get
            Return _taxCode2
        End Get
        Set(ByVal value As String)
            _taxCode2 = value
        End Set
    End Property
    Public Property TaxCode3() As String
        Get
            Return _taxCode3
        End Get
        Set(ByVal value As String)
            _taxCode3 = value
        End Set
    End Property
    Public Property TaxCode4() As String
        Get
            Return _taxCode4
        End Get
        Set(ByVal value As String)
            _taxCode4 = value
        End Set
    End Property
    Public Property TaxCode5() As String
        Get
            Return _taxCode5
        End Get
        Set(ByVal value As String)
            _taxCode5 = value
        End Set
    End Property
    Public Property TaxAmount1() As Decimal
        Get
            Return _taxAmount1
        End Get
        Set(ByVal value As Decimal)
            _taxAmount1 = value
        End Set
    End Property
    Public Property TaxAmount2() As Decimal
        Get
            Return _taxAmount2
        End Get
        Set(ByVal value As Decimal)
            _taxAmount2 = value
        End Set
    End Property
    Public Property TaxAmount3() As Decimal
        Get
            Return _taxAmount3
        End Get
        Set(ByVal value As Decimal)
            _taxAmount3 = value
        End Set
    End Property
    Public Property TaxAmount4() As Decimal
        Get
            Return _taxAmount4
        End Get
        Set(ByVal value As Decimal)
            _taxAmount4 = value
        End Set
    End Property
    Public Property TaxAmount5() As Decimal
        Get
            Return _taxAmount5
        End Get
        Set(ByVal value As Decimal)
            _taxAmount5 = value
        End Set
    End Property
    Public Property CustomerNumber() As Decimal
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As Decimal)
            _customerNumber = value
        End Set
    End Property
    Public Property CustomerNumberPrefix() As String
        Get
            Return _customerNumberPrefix
        End Get
        Set(ByVal value As String)
            _customerNumberPrefix = value
        End Set
    End Property
    Public Property BasketNumber() As String
        Get
            Return _basketNumber
        End Get
        Set(ByVal value As String)
            _basketNumber = value
        End Set
    End Property
    Public Property PromotionCode() As String
        Get
            Return _promotionCode
        End Get
        Set(ByVal value As String)
            _promotionCode = value
        End Set
    End Property
    Public Property PromotionCodeDescription() As String
        Get
            Return _promotionCodeDescription
        End Get
        Set(ByVal value As String)
            _promotionCodeDescription = value
        End Set
    End Property

    Private _totalOrderValue As Decimal
    Public Property TotalOrderValue() As Decimal
        Get
            Return _totalOrderValue
        End Get
        Set(ByVal value As Decimal)
            _totalOrderValue = value
        End Set
    End Property

    Private _extensionReference1 As String
    Public Property ExtensionReference1() As String
        Get
            Return _extensionReference1
        End Get
        Set(ByVal value As String)
            _extensionReference1 = value
        End Set
    End Property
    Private _extensionReference2 As String
    Public Property ExtensionReference2() As String
        Get
            Return _extensionReference2
        End Get
        Set(ByVal value As String)
            _extensionReference2 = value
        End Set
    End Property
    Private _extensionReference3 As String
    Public Property ExtensionReference3() As String
        Get
            Return _extensionReference3
        End Get
        Set(ByVal value As String)
            _extensionReference3 = value
        End Set
    End Property
    Private _extensionReference4 As String
    Public Property ExtensionReference4() As String
        Get
            Return _extensionReference4
        End Get
        Set(ByVal value As String)
            _extensionReference4 = value
        End Set
    End Property

    Private _extensionFixedPrice1 As Decimal
    Public Property ExtensionFixedPrice1() As Decimal
        Get
            Return _extensionFixedPrice1
        End Get
        Set(ByVal value As Decimal)
            _extensionFixedPrice1 = value
        End Set
    End Property

    Private _extensionFixedPrice2 As Decimal
    Public Property ExtensionFixedPrice2() As Decimal
        Get
            Return _extensionFixedPrice2
        End Get
        Set(ByVal value As Decimal)
            _extensionFixedPrice2 = value
        End Set
    End Property
    Private _extensionFixedPrice3 As Decimal
    Public Property ExtensionFixedPrice3() As Decimal
        Get
            Return _extensionFixedPrice3
        End Get
        Set(ByVal value As Decimal)
            _extensionFixedPrice3 = value
        End Set
    End Property
    Private _extensionFixedPrice4 As Decimal
    Public Property ExtensionFixedPrice4() As Decimal
        Get
            Return _extensionFixedPrice4
        End Get
        Set(ByVal value As Decimal)
            _extensionFixedPrice4 = value
        End Set
    End Property

    Private _extensionDealID1 As String
    Public Property ExtensionDealID1() As String
        Get
            Return _extensionDealID1
        End Get
        Set(ByVal value As String)
            _extensionDealID1 = value
        End Set
    End Property
   
    Private _extensionDealID2 As String
    Public Property ExtensionDealID2() As String
        Get
            Return _extensionDealID2
        End Get
        Set(ByVal value As String)
            _extensionDealID2 = value
        End Set
    End Property
    Private _extensionDealID3 As String
    Public Property ExtensionDealID3() As String
        Get
            Return _extensionDealID3
        End Get
        Set(ByVal value As String)
            _extensionDealID3 = value
        End Set
    End Property
    Private _extensionDealID4 As String
    Public Property ExtensionDealID4() As String
        Get
            Return _extensionDealID4
        End Get
        Set(ByVal value As String)
            _extensionDealID4 = value
        End Set
    End Property
    Private _extensionDealID5 As String
    Public Property ExtensionDealID5() As String
        Get
            Return _extensionDealID5
        End Get
        Set(ByVal value As String)
            _extensionDealID5 = value
        End Set
    End Property
    Private _extensionDealID6 As String
    Public Property ExtensionDealID6() As String
        Get
            Return _extensionDealID6
        End Get
        Set(ByVal value As String)
            _extensionDealID6 = value
        End Set
    End Property
    Private _extensionDealID7 As String
    Public Property ExtensionDealID7() As String
        Get
            Return _extensionDealID7
        End Get
        Set(ByVal value As String)
            _extensionDealID7 = value
        End Set
    End Property
    Private _extensionDealID8 As String
    Public Property ExtensionDealID8() As String
        Get
            Return _extensionDealID8
        End Get
        Set(ByVal value As String)
            _extensionDealID8 = value
        End Set
    End Property

    Private _extensionFlag1 As String
    Public Property ExtensionFlag1() As String
        Get
            Return _extensionFlag1
        End Get
        Set(ByVal value As String)
            _extensionFlag1 = value
        End Set
    End Property
    Private _extensionFlag2 As String
    Public Property ExtensionFlag2() As String
        Get
            Return _extensionFlag2
        End Get
        Set(ByVal value As String)
            _extensionFlag2 = value
        End Set
    End Property
    Private _extensionFlag3 As String
    Public Property ExtensionFlag3() As String
        Get
            Return _extensionFlag3
        End Get
        Set(ByVal value As String)
            _extensionFlag3 = value
        End Set
    End Property
    Private _extensionFlag4 As String
    Public Property ExtensionFlag4() As String
        Get
            Return _extensionFlag4
        End Get
        Set(ByVal value As String)
            _extensionFlag4 = value
        End Set
    End Property
    Private _extensionFlag5 As String
    Public Property ExtensionFlag5() As String
        Get
            Return _extensionFlag5
        End Get
        Set(ByVal value As String)
            _extensionFlag5 = value
        End Set
    End Property
    Private _extensionFlag6 As String
    Public Property ExtensionFlag6() As String
        Get
            Return _extensionFlag6
        End Get
        Set(ByVal value As String)
            _extensionFlag6 = value
        End Set
    End Property
    Private _extensionFlag7 As String
    Public Property ExtensionFlag7() As String
        Get
            Return _extensionFlag7
        End Get
        Set(ByVal value As String)
            _extensionFlag7 = value
        End Set
    End Property

    Private _extensionStatus As String
    Public Property ExtensionStatus() As String
        Get
            Return _extensionStatus
        End Get
        Set(ByVal value As String)
            _extensionStatus = value
        End Set
    End Property

    Private _status As String
    Public Property Status() As String
        Get
            Return _status
        End Get
        Set(ByVal value As String)
            _status = value
        End Set
    End Property
    Private _homeDelivery As Boolean
    Public Property HomeDelivery() As Boolean
        Get
            Return _homeDelivery
        End Get
        Set(ByVal value As Boolean)
            _homeDelivery = value
        End Set
    End Property
    Private _orderCustomerName As String
    Public Property OrderCustomerName() As String
        Get
            Return _orderCustomerName
        End Get
        Set(ByVal value As String)
            _orderCustomerName = value
        End Set
    End Property

    Private _SmartcardNumber As String
    Public Property SmartcardNumber() As String
        Get
            Return _SmartcardNumber
        End Get
        Set(ByVal value As String)
            _SmartcardNumber = value
        End Set
    End Property
    Public Property BasketPaymentID() As Long = 0

End Class
