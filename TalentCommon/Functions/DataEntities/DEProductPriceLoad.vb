'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is for importing product prices
'
'       Date                        26/10/09
'
'       Author                      Ben Ford
'
'       Copyright CS Group 2008     All rights reserved.
'
'--------------------------------------------------------------------------------------------------
'
<Serializable()> _
Public Class DEProductPriceLoad

    Private _ColTaxCodes As Collection
    Public Property ColTaxCodes() As Collection
        Get
            Return _ColTaxCodes
        End Get
        Set(ByVal value As Collection)
            _ColTaxCodes = value
        End Set
    End Property

    Private _totalTaxCodes As Integer
    Public Property TotalTaxCodes() As Integer
        Get
            Return _totalTaxCodes
        End Get
        Set(ByVal value As Integer)
            _totalTaxCodes = value
        End Set
    End Property

    Private _taxCodeMode As String
    Public Property TaxCodeMode() As String
        Get
            Return _taxCodeMode
        End Get
        Set(ByVal value As String)
            _taxCodeMode = value
        End Set
    End Property


    Private _colCurrencyCodes As Collection
    Public Property ColCurrencyCodes() As Collection
        Get
            Return _colCurrencyCodes
        End Get
        Set(ByVal value As Collection)
            _colCurrencyCodes = value
        End Set
    End Property


    Private _totalCurrencyCodes As Integer
    Public Property TotalCurrencyCodes() As Integer
        Get
            Return _totalCurrencyCodes
        End Get
        Set(ByVal value As Integer)
            _totalCurrencyCodes = value
        End Set
    End Property

    Private _currencyCodeMode As String
    Public Property CurrencyCodeMode() As String
        Get
            Return _currencyCodeMode
        End Get
        Set(ByVal value As String)
            _currencyCodeMode = value
        End Set
    End Property

    Private _colPriceLists As Collection
    Public Property ColPriceLists() As Collection
        Get
            Return _colPriceLists
        End Get
        Set(ByVal value As Collection)
            _colPriceLists = value
        End Set
    End Property

    Private _colDefaults As Collection
    Public Property ColDefaults() As Collection
        Get
            Return _colDefaults
        End Get
        Set(ByVal value As Collection)
            _colDefaults = value
        End Set
    End Property

    Private _totalPriceLists As Integer
    Public Property TotalPriceLists() As Integer
        Get
            Return _totalPriceLists
        End Get
        Set(ByVal value As Integer)
            _totalPriceLists = value
        End Set
    End Property

    Private _priceListMode As String
    Public Property PriceListMode() As String
        Get
            Return _priceListMode
        End Get
        Set(ByVal value As String)
            _priceListMode = value
        End Set
    End Property
    Private _totalDefaults As Integer
    Public Property TotalDefaults() As Integer
        Get
            Return _totalDefaults
        End Get
        Set(ByVal value As Integer)
            _totalDefaults = value
        End Set
    End Property

    Private _defaultsMode As String
    Public Property DefaultsMode() As String
        Get
            Return _defaultsMode
        End Get
        Set(ByVal value As String)
            _defaultsMode = value
        End Set
    End Property
    Public Sub New()
        ColDefaults = New Collection
        ColTaxCodes = New Collection
        ColCurrencyCodes = New Collection
        ColPriceLists = New Collection
    End Sub
End Class
<Serializable()> _
Public Class DETaxCode

    Private _taxCode As String
    Public Property TaxCode() As String
        Get
            Return _taxCode
        End Get
        Set(ByVal value As String)
            _taxCode = value
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

    Private _mode As String
    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

End Class
<Serializable()> _
Public Class DECurrencyCode

    Private _currencyCode As String
    Public Property CurrencyCode() As String
        Get
            Return _currencyCode
        End Get
        Set(ByVal value As String)
            _currencyCode = value
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

    Private _htmlCurrencySymbol As String
    Public Property HtmlCurrencySymbol() As String
        Get
            Return _htmlCurrencySymbol
        End Get
        Set(ByVal value As String)
            _htmlCurrencySymbol = value
        End Set
    End Property
    Private _mode As String
    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

End Class

<Serializable()> _
Public Class DEPriceList

    Private _code As String
    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            _code = value
        End Set
    End Property
    Private _mode As String
    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
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

    Private _currencyCode As String
    Public Property CurrencyCode() As String
        Get
            Return _currencyCode
        End Get
        Set(ByVal value As String)
            _currencyCode = value
        End Set
    End Property

    Private _freeDeliveryValue As Decimal
    Public Property FreeDeliveryValue() As Decimal
        Get
            Return _freeDeliveryValue
        End Get
        Set(ByVal value As Decimal)
            _freeDeliveryValue = value
        End Set
    End Property


    Private _minimumDeliveryValue As Decimal
    Public Property MinimumDeliveryValue() As Decimal
        Get
            Return _minimumDeliveryValue
        End Get
        Set(ByVal value As Decimal)
            _minimumDeliveryValue = value
        End Set
    End Property

    Private _startDate As String
    Public Property StartDate() As String
        Get
            Return _startDate
        End Get
        Set(ByVal value As String)
            _startDate = value
        End Set
    End Property

    Private _endDate As String
    Public Property EndDate() As String
        Get
            Return _endDate
        End Get
        Set(ByVal value As String)
            _endDate = value
        End Set
    End Property


    Private _colProductPrice As Collection
    Public Property ColProductPrice() As Collection
        Get
            Return _colProductPrice
        End Get
        Set(ByVal value As Collection)
            _colProductPrice = value
        End Set
    End Property

    Private _totalProductPrices As Integer
    Public Property TotalProductPrices() As Integer
        Get
            Return _totalProductPrices
        End Get
        Set(ByVal value As Integer)
            _totalProductPrices = value
        End Set
    End Property

    Private _productPricesMode As String
    Public Property ProductPricesMode() As String
        Get
            Return _productPricesMode
        End Get
        Set(ByVal value As String)
            _productPricesMode = value
        End Set
    End Property

    Private _err As ErrorObj
    Public Property Err() As ErrorObj
        Get
            Return _err
        End Get
        Set(ByVal value As ErrorObj)
            _err = value
        End Set
    End Property

End Class

<Serializable()> _
Public Class DEProductPrice

    Private _priceList As String = String.Empty
    Public Property PriceList() As String
        Get
            Return _priceList
        End Get
        Set(ByVal value As String)
            _priceList = value
        End Set
    End Property

    Private _productCode As String = String.Empty
    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property

    Private _price1 As Decimal = 0
    Public Property Price1() As Decimal
        Get
            Return _price1
        End Get
        Set(ByVal value As Decimal)
            _price1 = value
        End Set
    End Property

    Private _price2 As Decimal = 0
    Public Property Price2() As Decimal
        Get
            Return _price2
        End Get
        Set(ByVal value As Decimal)
            _price2 = value
        End Set
    End Property

    Private _price3 As Decimal = 0
    Public Property Price3() As Decimal
        Get
            Return _price3
        End Get
        Set(ByVal value As Decimal)
            _price3 = value
        End Set
    End Property

    Private _price4 As Decimal = 0
    Public Property Price4() As Decimal
        Get
            Return _price4
        End Get
        Set(ByVal value As Decimal)
            _price4 = value
        End Set
    End Property

    Private _price5 As Decimal = 0
    Public Property Price5() As Decimal
        Get
            Return _price5
        End Get
        Set(ByVal value As Decimal)
            _price5 = value
        End Set
    End Property

    Private _sku As String = String.Empty
    Public Property SKU() As String
        Get
            Return _sku
        End Get
        Set(ByVal value As String)
            _sku = value
        End Set
    End Property

    Private _mode As String = String.Empty
    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Private _fromDate As String = String.Empty
    Public Property FromDate() As String
        Get
            Return _fromDate
        End Get
        Set(ByVal value As String)
            _fromDate = value
        End Set
    End Property

    Private _toDate As String = String.Empty
    Public Property ToDate() As String
        Get
            Return _toDate
        End Get
        Set(ByVal value As String)
            _toDate = value
        End Set
    End Property

    Private _taxCode As String = String.Empty
    Public Property TaxCode() As String
        Get
            Return _taxCode
        End Get
        Set(ByVal value As String)
            _taxCode = value
        End Set
    End Property

    Private _tariffCode As String = String.Empty
    Public Property TariffCode() As String
        Get
            Return _tariffCode
        End Get
        Set(ByVal value As String)
            _tariffCode = value
        End Set
    End Property


    Private _price As DEPricingDetails
    Public Property Price() As DEPricingDetails
        Get
            Return _price
        End Get
        Set(ByVal value As DEPricingDetails)
            _price = value
        End Set
    End Property

    Private _priceBreak1 As DEPricingDetails
    Public Property PriceBreak1() As DEPricingDetails
        Get
            Return _priceBreak1
        End Get
        Set(ByVal value As DEPricingDetails)
            _priceBreak1 = value
        End Set
    End Property
    Private _priceBreak2 As DEPricingDetails
    Public Property PriceBreak2() As DEPricingDetails
        Get
            Return _priceBreak2
        End Get
        Set(ByVal value As DEPricingDetails)
            _priceBreak2 = value
        End Set
    End Property

    Private _priceBreak3 As DEPricingDetails
    Public Property PriceBreak3() As DEPricingDetails
        Get
            Return _priceBreak3
        End Get
        Set(ByVal value As DEPricingDetails)
            _priceBreak3 = value
        End Set
    End Property

    Private _priceBreak4 As DEPricingDetails
    Public Property PriceBreak4() As DEPricingDetails
        Get
            Return _priceBreak4
        End Get
        Set(ByVal value As DEPricingDetails)
            _priceBreak4 = value
        End Set
    End Property

    Private _priceBreak5 As DEPricingDetails
    Public Property PriceBreak5() As DEPricingDetails
        Get
            Return _priceBreak5
        End Get
        Set(ByVal value As DEPricingDetails)
            _priceBreak5 = value
        End Set
    End Property

    Private _priceBreak6 As DEPricingDetails
    Public Property PriceBreak6() As DEPricingDetails
        Get
            Return _priceBreak6
        End Get
        Set(ByVal value As DEPricingDetails)
            _priceBreak6 = value
        End Set
    End Property

    Private _priceBreak7 As DEPricingDetails
    Public Property PriceBreak7() As DEPricingDetails
        Get
            Return _priceBreak7
        End Get
        Set(ByVal value As DEPricingDetails)
            _priceBreak7 = value
        End Set
    End Property

    Private _priceBreak8 As DEPricingDetails
    Public Property PriceBreak8() As DEPricingDetails
        Get
            Return _priceBreak8
        End Get
        Set(ByVal value As DEPricingDetails)
            _priceBreak8 = value
        End Set
    End Property

    Private _priceBreak9 As DEPricingDetails
    Public Property PriceBreak9() As DEPricingDetails
        Get
            Return _priceBreak9
        End Get
        Set(ByVal value As DEPricingDetails)
            _priceBreak9 = value
        End Set
    End Property

    Private _priceBreak10 As DEPricingDetails
    Public Property PriceBreak10() As DEPricingDetails
        Get
            Return _priceBreak10
        End Get
        Set(ByVal value As DEPricingDetails)
            _priceBreak10 = value
        End Set
    End Property

    Public Sub New()
        Price = New DEPricingDetails
        PriceBreak1 = New DEPricingDetails
        PriceBreak2 = New DEPricingDetails
        PriceBreak3 = New DEPricingDetails
        PriceBreak4 = New DEPricingDetails
        PriceBreak5 = New DEPricingDetails
        PriceBreak6 = New DEPricingDetails
        PriceBreak7 = New DEPricingDetails
        PriceBreak8 = New DEPricingDetails
        PriceBreak9 = New DEPricingDetails
        PriceBreak10 = New DEPricingDetails
    End Sub
End Class
<Serializable()> _
Public Class DEPricingDetails

    Private _priceBreakCode As String = ""
    Public Property PriceBreakCode() As String
        Get
            Return _priceBreakCode
        End Get
        Set(ByVal value As String)
            _priceBreakCode = value
        End Set
    End Property

    Private _priceBreakQty As Integer = 0
    Public Property PriceBreakQty() As Integer
        Get
            Return _priceBreakQty
        End Get
        Set(ByVal value As Integer)
            _priceBreakQty = value
        End Set
    End Property


    Private _grossPrice As Decimal = 0
    Public Property GrossPrice() As Decimal
        Get
            Return _grossPrice
        End Get
        Set(ByVal value As Decimal)
            _grossPrice = value
        End Set
    End Property

    Private _netPrice As Decimal = 0
    Public Property NetPrice() As Decimal
        Get
            Return _netPrice
        End Get
        Set(ByVal value As Decimal)
            _netPrice = value
        End Set
    End Property

    Private _tax As Decimal = 0
    Public Property Tax() As Decimal
        Get
            Return _tax
        End Get
        Set(ByVal value As Decimal)
            _tax = value
        End Set
    End Property

    Private _deliveryGross As Decimal = 0
    Public Property DeliveryGross() As Decimal
        Get
            Return _deliveryGross
        End Get
        Set(ByVal value As Decimal)
            _deliveryGross = value
        End Set
    End Property

    Private _deliveryNet As Decimal = 0
    Public Property DeliveryNet() As Decimal
        Get
            Return _deliveryNet
        End Get
        Set(ByVal value As Decimal)
            _deliveryNet = value
        End Set
    End Property

    Private _deliveryTax As Decimal = 0
    Public Property DeliveryTax() As Decimal
        Get
            Return _deliveryTax
        End Get
        Set(ByVal value As Decimal)
            _deliveryTax = value
        End Set
    End Property

    Private _salePriceBreakQuantity As Integer = 0
    Public Property SalePriceBreakQuantity() As Integer
        Get
            Return _salePriceBreakQuantity
        End Get
        Set(ByVal value As Integer)
            _salePriceBreakQuantity = value
        End Set
    End Property

    Private _salePriceGross As Decimal = 0
    Public Property SalePriceGross() As Decimal
        Get
            Return _salePriceGross
        End Get
        Set(ByVal value As Decimal)
            _salePriceGross = value
        End Set
    End Property

    Private _salePriceNet As Decimal = 0
    Public Property SalePriceNet() As Decimal
        Get
            Return _salePriceNet
        End Get
        Set(ByVal value As Decimal)
            _salePriceNet = value
        End Set
    End Property

    Private _salePriceTax As Decimal = 0
    Public Property SalePriceTax() As Decimal
        Get
            Return _salePriceTax
        End Get
        Set(ByVal value As Decimal)
            _salePriceTax = value
        End Set
    End Property

    Private _saleDeliveryGross As Decimal = 0
    Public Property SaleDeliveryGross() As Decimal
        Get
            Return _saleDeliveryGross
        End Get
        Set(ByVal value As Decimal)
            _saleDeliveryGross = value
        End Set
    End Property

    Private _saleDeliveryNet As Decimal = 0
    Public Property SaleDeliveryNet() As Decimal
        Get
            Return _saleDeliveryNet
        End Get
        Set(ByVal value As Decimal)
            _saleDeliveryNet = value
        End Set
    End Property

    Private _saleDeliveryTax As Decimal = 0
    Public Property SaleDeliveryTax() As Decimal
        Get
            Return _saleDeliveryTax
        End Get
        Set(ByVal value As Decimal)
            _saleDeliveryTax = value
        End Set
    End Property

    Public Sub New()

    End Sub
End Class
<Serializable()> _
Public Class DEPriceDefault

    Private _defaultName As String
    Public Property DefaultName() As String
        Get
            Return _defaultName
        End Get
        Set(ByVal value As String)
            _defaultName = value
        End Set
    End Property

    Private _businessUnit As String
    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property

    Private _partner As String
    Public Property Partner() As String
        Get
            Return _partner
        End Get
        Set(ByVal value As String)
            _partner = value
        End Set
    End Property

    Private _mode As String
    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Private _value As String
    Public Property Value() As String
        Get
            Return _value
        End Get
        Set(ByVal value As String)
            _value = value
        End Set
    End Property

End Class


