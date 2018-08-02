Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Response
'
'       Date                        15th Jan 2007
'
'       Author                      Andy White
'
'       © CS Group 2006             All rights reserved.
'
'       Error Number Code base      TACDEOR- 
'                                   application.code(3) + object code(4) + number(2)
'--------------------------------------------------------------------------------------------------
Public Class DEOrderResponse
    '----------------------------------------------------------------------------------------------
    ' Read order header                                                     double quote is dealt with 
    '
    Private _branchOrderNumber As String = String.Empty                 '' System21OrderNo
    Private _customerPO As String = String.Empty                        '' CUSO40 on ORDERHDR
    Private _cUSN40 As String = String.Empty                            '' CUSO40 on ORDERHDR
    Private _dSEQ40 As String = String.Empty                            '' DSEQ40 on ORDERHDR
    '---------------------------------------------------------------------------------------------
    '   Order Detail Response
    '
    Private _backOrderStatus As String = String.Empty                   '' QTDS55 <> QTOR55 on OEP55   
    Private _configFlag As String = String.Empty                        ' 
    Private _configTimeStamp As String = String.Empty                   ' 
    Private _creditCardSW As String = String.Empty                      ' 
    Private _creditMemoReasonCode As String = String.Empty              ' 
    Private _endUserPO As String = String.Empty                         ' C1COR (not written to S/21)
    Private _entryMethod As String = String.Empty                       ' 
    Private _frtOutCode As String = String.Empty                        ' 
    Private _fulfillmentFlag As String = String.Empty                   ' 
    Private _govEndUserType As String = String.Empty                    ' 
    Private _holdReason As String = String.Empty                        '' SUSP40 on OEP40 / ORDERHDR
    Private _invoiceDate As Date                                        '' DTIN65 on OEP65
    Private _invoiceNumber As String                                    '
    Private _numberOfCartons As Integer = 0                             '
    Private _oECarrier As String = String.Empty                         '' CARR40 on OEP40 / ORDERHDR 
    Private _orderNumber As String = String.Empty                       ' 
    Private _orderWeight As String = String.Empty                       '' Try TOWT40 and WUOM40 on OEP40 
    Private _orderEntryDate As Date                                     '' DTSO40 on OEP40
    Private _orderType As String = String.Empty                         '' ORTP40 on OEP40   
    Private _promiseDate As Date                                        '' DTLP40 on OEP40
    Private _proNbr As String = String.Empty                            ' 
    Private _proNbrSW As String = String.Empty                          '    
    Private _resellerNBR As String = String.Empty                       ' 
    Private _rMACode As String = String.Empty                           ' 
    Private _selSrcAcctnoHdr As String = String.Empty                   ' 
    Private _selSrcSlsHdr As String = String.Empty                      ' 
    Private _shipComplete As String = String.Empty                      ' 
    Private _shippableSW As String = String.Empty                       ' 
    Private _splitBillToSwitch As String = String.Empty                 ' 
    Private _splitFromOrderNumber As String = String.Empty              ' 
    Private _termsCode As String = String.Empty                         '' PAYT40 on OEP40
    Private _termID As String = String.Empty                            ' 
    '
    Private _cODAmount As Decimal = 0                                   '
    Private _companyCurrency As String = String.Empty                   '
    Private _currencyCode As String = String.Empty                      '
    Private _currencyRate As Decimal = 0                                '
    Private _discountAmount As Decimal = 0                              '
    Private _freightTotal As Decimal = 0                                '
    Private _grandTotal As Decimal = 0                                  '
    Private _salePlusTax As Decimal = 0                                 '
    Private _salesTotal As Decimal = 0                                  '
    Private _taxTotal As Decimal = 0                                    '
    '---------------------------------------------------------------------------------------------
    '   OrderStatusRequest
    '
    Private _orderStatus As String = String.Empty                       '' STAT40 on OEP40 
    Private _orderShipDate As Date                                        ' Try DSDT56 on INP56
    Private _shipFromBranch As String = String.Empty                    ' STRN20 on INP20
    Private _shipFromBranchNumber As String = String.Empty              ' Try LOCD56 on INP56
    '                                                                   ' LOCD55 on ORDLINES
    Private _totalSales As String = String.Empty                        ' More detail required 
    '---------------------------------------------------------------------------------------------
    '   OrderTrackingRequest
    '
    Private _cartonCount As String = String.Empty                       ' Don't think held in System 21
    Private _totalWeight As String = String.Empty                       '' TOWT65 on OEP65 
    '---------------------------------------------------------------------------------------------
    '   Read carriage details
    '
    Private _carrierCode As String = String.Empty                       '' CHGC50 on CHARGES
    Private _carrierCodeDescription As String = String.Empty            '' ODES50 on CHARGES
    Private _distributionWeight As Double = 0               ' 
    Private _freightRate As Decimal = 0                                 '' CHGV50 on CHARGES
    Private _thirdPartyFreight As String = String.Empty                 ' More detail required (probably some value of TMTH40 on OEP40)
    '---------------------------------------------------------------------------------------------
    '   Read delivery address 
    ' 
    Private _shipTo As String = String.Empty                            '' DESQ45 on DELADDS  
    Private _shipToAttention As String = String.Empty                   '' ONAM45 on DELADDS  
    Private _shipToName As String = String.Empty                        '' ONAM45 on DELADDS  
    Private _shipToAddress1 As String = String.Empty                    '' OAD145 on DELADDS  
    Private _shipToAddress2 As String = String.Empty                    '' OAD245 on DELADDS  
    Private _shipToAddress3 As String = String.Empty                    '' OAD345 on DELADDS  
    Private _shipToAddress4 As String = String.Empty                    '' OAD445 on DELADDS  
    Private _ShipToCity As String = String.Empty                        '' OAD445 on DELADDS 
    Private _ShipToProvince As String = String.Empty                    '' OAD545 on DELADDS  
    Private _shipToPostalCode As String = String.Empty                  '' OPST45 on DELADDS  
    '---------------------------------------------------------------------------------------------
    '   Read billingaddress 
    ' 
    Private _billTo As String = String.Empty                            '' CNAM05 on CUSNAMES
    Private _billToAttention As String = String.Empty                   '' CNAM05 on CUSNAMES
    Private _billToName As String = String.Empty                        '' CAD105 on CUSNAMES
    Private _billToAddress1 As String = String.Empty                    '' CAD205 on CUSNAMES
    Private _billToAddress2 As String = String.Empty                    '' CAD305 on CUSNAMES
    Private _billToAddress3 As String = String.Empty                    '' CAD405 on CUSNAMES
    Private _billToAddress4 As String = String.Empty                    '' CAD505 on CUSNAMES
    Private _billToCity As String = String.Empty                        '' CAD505 on CUSNAMES
    Private _billToProvince As String = String.Empty                    '' PCD105 on CUSNAMES
    Private _billToPostalCode As String = String.Empty                  '' PCD205 on CUSNAMES
    '-----------------------------------------------------------------------------
    '   Read order Details
    '
    Private _sku As String = String.Empty                               '' CATN55 on ORDLINES
    Private _unitPrice As String = String.Empty                         '' UPRC55 on ORDLINES
    Private _lineNumber As String = String.Empty                        '' ORDL55 on ORDLINES
    Private _orderQuantity As String = String.Empty                     '' QTOR55 on ORDLINES
    Private _allocatedQuantity As String = String.Empty                 '' QTAL55 on ORDLINES
    Private _backOrderedQuantity As String = String.Empty               '' QTOR55 on ORDLINES
    '-----------------------------------------------------------------------------
    '   Read order text
    ' 
    Private _text As String = String.Empty                              '' TLIN40 on INP40
    Private _textLineNumber As String = String.Empty                    '' TLNO40 on INP40
    '-----------------------------------------------------------------------------
    '   Read order header
    '
    Public Property BranchOrderNumber() As String
        Get
            Return _branchOrderNumber
        End Get
        Set(ByVal value As String)
            _branchOrderNumber = value
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
    Public Property CUSN40() As String
        Get
            Return _cUSN40
        End Get
        Set(ByVal value As String)
            _cUSN40 = value
        End Set
    End Property
    Public Property DSEQ40() As String
        Get
            Return _dSEQ40
        End Get
        Set(ByVal value As String)
            _dSEQ40 = value
        End Set
    End Property
    '-----------------------------------------------------------------------------
    '   Order Detail Response
    '
    Public Property BackOrderStatus() As String
        Get
            Return _backOrderStatus
        End Get
        Set(ByVal value As String)
            _backOrderStatus = value
        End Set
    End Property
    Public Property ConfigFlag() As String
        Get
            Return _configFlag
        End Get
        Set(ByVal value As String)
            _configFlag = value
        End Set
    End Property
    Public Property ConfigTimeStamp() As String
        Get
            Return _configTimeStamp
        End Get
        Set(ByVal value As String)
            _configTimeStamp = value
        End Set
    End Property
    Public Property CreditCardSW() As String
        Get
            Return _creditCardSW
        End Get
        Set(ByVal value As String)
            _creditCardSW = value
        End Set
    End Property
    Public Property CreditMemoReasonCode() As String
        Get
            Return _creditMemoReasonCode
        End Get
        Set(ByVal value As String)
            _creditMemoReasonCode = value
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
    Public Property EntryMethod() As String
        Get
            Return _entryMethod
        End Get
        Set(ByVal value As String)
            _entryMethod = value
        End Set
    End Property
    Public Property FrtOutCode() As String
        Get
            Return _frtOutCode
        End Get
        Set(ByVal value As String)
            _frtOutCode = value
        End Set
    End Property
    Public Property FulfillmentFlag() As String
        Get
            Return _fulfillmentFlag
        End Get
        Set(ByVal value As String)
            _fulfillmentFlag = value
        End Set
    End Property
    Public Property GovEndUserType() As String
        Get
            Return _govEndUserType
        End Get
        Set(ByVal value As String)
            _govEndUserType = value
        End Set
    End Property
    Public Property HoldReason() As String
        Get
            Return _holdReason
        End Get
        Set(ByVal value As String)
            _holdReason = value
        End Set
    End Property
    Public Property InvoiceDate() As Date
        Get
            Return _invoiceDate
        End Get
        Set(ByVal value As Date)
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
    Public Property NumberOfCartons() As Integer
        Get
            Return _numberOfCartons
        End Get
        Set(ByVal value As Integer)
            _numberOfCartons = value
        End Set
    End Property
    Public Property OECarrier() As String
        Get
            Return _oECarrier
        End Get
        Set(ByVal value As String)
            _oECarrier = value
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
    Public Property OrderWeight() As String
        Get
            Return _orderWeight
        End Get
        Set(ByVal value As String)
            _orderWeight = value
        End Set
    End Property
    Public Property OrderEntryDate() As Date
        Get
            Return _orderEntryDate
        End Get
        Set(ByVal value As Date)
            _orderEntryDate = value
        End Set
    End Property
    Public Property OrderType() As String
        Get
            Return _orderType
        End Get
        Set(ByVal value As String)
            _orderType = value
        End Set
    End Property
    Public Property PromiseDate() As Date
        Get
            Return _promiseDate
        End Get
        Set(ByVal value As Date)
            _promiseDate = value
        End Set
    End Property
    Public Property ProNbr() As String
        Get
            Return _proNbr
        End Get
        Set(ByVal value As String)
            _proNbr = value
        End Set
    End Property
    Public Property ProNbrSW() As String
        Get
            Return _proNbrSW
        End Get
        Set(ByVal value As String)
            _proNbrSW = value
        End Set
    End Property
    Public Property ResellerNBR() As String
        Get
            Return _resellerNBR
        End Get
        Set(ByVal value As String)
            _resellerNBR = value
        End Set
    End Property
    Public Property RMACode() As String
        Get
            Return _rMACode
        End Get
        Set(ByVal value As String)
            _rMACode = value
        End Set
    End Property
    Public Property SelSrcAcctnoHdr() As String
        Get
            Return _selSrcAcctnoHdr
        End Get
        Set(ByVal value As String)
            _selSrcAcctnoHdr = value
        End Set
    End Property
    Public Property SelSrcSlsHdr() As String
        Get
            Return _selSrcSlsHdr
        End Get
        Set(ByVal value As String)
            _selSrcSlsHdr = value
        End Set
    End Property
    Public Property ShipComplete() As String
        Get
            Return _shipComplete
        End Get
        Set(ByVal value As String)
            _shipComplete = value
        End Set
    End Property
    Public Property ShippableSW() As String
        Get
            Return _shippableSW
        End Get
        Set(ByVal value As String)
            _shippableSW = value
        End Set
    End Property
    Public Property SplitBillToSwitch() As String
        Get
            Return _splitBillToSwitch
        End Get
        Set(ByVal value As String)
            _splitBillToSwitch = value
        End Set
    End Property
    Public Property SplitFromOrderNumber() As String
        Get
            Return _splitFromOrderNumber
        End Get
        Set(ByVal value As String)
            _splitFromOrderNumber = value
        End Set
    End Property
    Public Property TermsCode() As String
        Get
            Return _termsCode
        End Get
        Set(ByVal value As String)
            _termsCode = value
        End Set
    End Property
    Public Property TermID() As String
        Get
            Return _termID
        End Get
        Set(ByVal value As String)
            _termID = value
        End Set
    End Property

    Public Property CODAmount() As Decimal
        Get
            Return _cODAmount
        End Get
        Set(ByVal value As Decimal)
            _cODAmount = value
        End Set
    End Property
    Public Property CompanyCurrency() As String
        Get
            Return _companyCurrency
        End Get
        Set(ByVal value As String)
            _companyCurrency = value
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
    Public Property CurrencyRate() As Decimal
        Get
            Return _currencyRate
        End Get
        Set(ByVal value As Decimal)
            _currencyRate = value
        End Set
    End Property
    Public Property DiscountAmount() As Decimal
        Get
            Return _discountAmount
        End Get
        Set(ByVal value As Decimal)
            _discountAmount = value
        End Set
    End Property
    Public Property FreightTotal() As Decimal
        Get
            Return _freightTotal
        End Get
        Set(ByVal value As Decimal)
            _freightTotal = value
        End Set
    End Property
    Public Property GrandTotal() As Decimal
        Get
            Return _grandTotal
        End Get
        Set(ByVal value As Decimal)
            _grandTotal = value
        End Set
    End Property
    Public Property SalePlusTax() As Decimal
        Get
            Return _salePlusTax
        End Get
        Set(ByVal value As Decimal)
            _salePlusTax = value
        End Set
    End Property
    Public Property SalesTotal() As Decimal
        Get
            Return _salesTotal
        End Get
        Set(ByVal value As Decimal)
            _salesTotal = value
        End Set
    End Property
    Public Property TaxTotal() As Decimal
        Get
            Return _taxTotal
        End Get
        Set(ByVal value As Decimal)
            _taxTotal = value
        End Set
    End Property
    '-----------------------------------------------------------------------------
    '   OrderStatusRequest
    '
    Public Property OrderStatus() As String
        Get
            Return _orderStatus
        End Get
        Set(ByVal value As String)
            _orderStatus = value
        End Set
    End Property
    Public Property OrderShipDate() As Date
        Get
            Return _orderShipDate
        End Get
        Set(ByVal value As Date)
            _orderShipDate = value
        End Set
    End Property
    Public Property ShipFromBranch() As String
        Get
            Return _shipFromBranch
        End Get
        Set(ByVal value As String)
            _shipFromBranch = value
        End Set
    End Property
    Public Property ShipFromBranchNumber() As String
        Get
            Return _shipFromBranchNumber
        End Get
        Set(ByVal value As String)
            _shipFromBranchNumber = value
        End Set
    End Property
    Public Property TotalSales() As String
        Get
            Return _totalSales
        End Get
        Set(ByVal value As String)
            _totalSales = value
        End Set
    End Property
    '-----------------------------------------------------------------------------
    '   OrderTrackingRequest
    '
    Public Property CartonCount() As String
        Get
            Return _cartonCount
        End Get
        Set(ByVal value As String)
            _cartonCount = value
        End Set
    End Property
    Public Property TotalWeight() As String
        Get
            Return _totalWeight
        End Get
        Set(ByVal value As String)
            _totalWeight = value
        End Set
    End Property
    '-----------------------------------------------------------------------------
    '   Read carriage details
    '
    Public Property CarrierCode() As String
        Get
            Return _carrierCode
        End Get
        Set(ByVal value As String)
            _carrierCode = value
        End Set
    End Property
    Public Property CarrierCodeDescription() As String
        Get
            Return _carrierCodeDescription
        End Get
        Set(ByVal value As String)
            _carrierCodeDescription = value
        End Set
    End Property
    Public Property DistributionWeight() As String
        Get
            Return _distributionWeight
        End Get
        Set(ByVal value As String)
            _distributionWeight = value
        End Set
    End Property
    Public Property FreightRate() As Decimal
        Get
            Return _freightRate
        End Get
        Set(ByVal value As Decimal)
            _freightRate = value
        End Set
    End Property
    Public Property ThirdPartyFreight() As String
        Get
            Return _thirdPartyFreight
        End Get
        Set(ByVal value As String)
            _thirdPartyFreight = value
        End Set
    End Property
    '------------------------------------------------------------------------------
    ' Read delivery address - try OEP45 first
    ' 
    Public Property ShipTo() As String
        Get
            Return _shipTo
        End Get
        Set(ByVal value As String)
            _shipTo = value
        End Set
    End Property
    Public Property ShipToAttention() As String
        Get
            Return _shipToAttention
        End Get
        Set(ByVal value As String)
            _shipToAttention = value
        End Set
    End Property
    Public Property ShipToName() As String
        Get
            Return _shipToName
        End Get
        Set(ByVal value As String)
            _shipToName = value
        End Set
    End Property
    Public Property ShipToAddress1() As String
        Get
            Return _shipToAddress1
        End Get
        Set(ByVal value As String)
            _shipToAddress1 = value
        End Set
    End Property
    Public Property ShipToAddress2() As String
        Get
            Return _shipToAddress2
        End Get
        Set(ByVal value As String)
            _shipToAddress2 = value
        End Set
    End Property
    Public Property ShipToAddress3() As String
        Get
            Return _shipToAddress3
        End Get
        Set(ByVal value As String)
            _shipToAddress3 = value
        End Set
    End Property
    Public Property ShipToAddress4() As String
        Get
            Return _shipToAddress4
        End Get
        Set(ByVal value As String)
            _shipToAddress4 = value
        End Set
    End Property
    Public Property ShipToCity() As String
        Get
            Return _ShipToCity
        End Get
        Set(ByVal value As String)
            _ShipToCity = value
        End Set
    End Property
    Public Property ShipToProvince() As String
        Get
            Return _ShipToProvince
        End Get
        Set(ByVal value As String)
            _ShipToProvince = value
        End Set
    End Property
    Public Property ShipToPostalCode() As String
        Get
            Return _shipToPostalCode
        End Get
        Set(ByVal value As String)
            _shipToPostalCode = value
        End Set
    End Property

    Public Property BillTo() As String
        Get
            Return _billTo
        End Get
        Set(ByVal value As String)
            _billTo = value
        End Set
    End Property
    Public Property BillToAttention() As String
        Get
            Return _billToAttention
        End Get
        Set(ByVal value As String)
            _billToAttention = value
        End Set
    End Property
    Public Property BillToName() As String
        Get
            Return _billToName
        End Get
        Set(ByVal value As String)
            _billToName = value
        End Set
    End Property
    Public Property BillToAddress1() As String
        Get
            Return _billToAddress1
        End Get
        Set(ByVal value As String)
            _billToAddress1 = value
        End Set
    End Property
    Public Property BillToAddress2() As String
        Get
            Return _billToAddress2
        End Get
        Set(ByVal value As String)
            _billToAddress2 = value
        End Set
    End Property
    Public Property BillToAddress3() As String
        Get
            Return _billToAddress3
        End Get
        Set(ByVal value As String)
            _billToAddress3 = value
        End Set
    End Property
    Public Property BillToAddress4() As String
        Get
            Return _billToAddress4
        End Get
        Set(ByVal value As String)
            _billToAddress4 = value
        End Set
    End Property
    Public Property BillToCity() As String
        Get
            Return _billToCity
        End Get
        Set(ByVal value As String)
            _billToCity = value
        End Set
    End Property
    Public Property BillToProvince() As String
        Get
            Return _billToProvince
        End Get
        Set(ByVal value As String)
            _billToProvince = value
        End Set
    End Property
    Public Property BillToPostalCode() As String
        Get
            Return _billToPostalCode
        End Get
        Set(ByVal value As String)
            _billToPostalCode = value
        End Set
    End Property

End Class
