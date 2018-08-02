<Serializable()> _
Public Class DEWebPrice

    Private _isSalePrice As Boolean
    Public Property IsSalePrice() As Boolean
        Get
            Return _isSalePrice
        End Get
        Set(ByVal value As Boolean)
            _isSalePrice = value
        End Set
    End Property


    Private _isPartOfPromotion As Boolean
    Public Property IsPartOfPromotion() As Boolean
        Get
            Return _isPartOfPromotion
        End Get
        Set(ByVal value As Boolean)
            _isPartOfPromotion = value
        End Set
    End Property

    Private _PromotionImagePath As String
    Public Property PromotionImagePath() As String
        Get
            Return _PromotionImagePath
        End Get
        Set(ByVal value As String)
            _PromotionImagePath = value
        End Set
    End Property

    Private _PromotionDescriptionText As String
    Public Property PromotionDescriptionText() As String
        Get
            Return _PromotionDescriptionText
        End Get
        Set(ByVal value As String)
            _PromotionDescriptionText = value
        End Set
    End Property

    Private _productcode As String
    Public Property PRODUCT_CODE() As String
        Get
            Return _productcode
        End Get
        Set(ByVal value As String)
            _productcode = value
        End Set
    End Property


    Private _priceBreakCode As String
    Public Property PRICE_BREAK_CODE() As String
        Get
            Return _priceBreakCode
        End Get
        Set(ByVal value As String)
            _priceBreakCode = value
        End Set
    End Property


    Private _requestedQuantity As Decimal
    Public Property RequestedQuantity() As Decimal
        Get
            Return _requestedQuantity
        End Get
        Set(ByVal value As Decimal)
            _requestedQuantity = value
        End Set
    End Property


    Private _promoInclusivePrice As Decimal
    Public Property PromotionInclusivePriceGross() As Decimal
        Get
            Return _promoInclusivePrice
        End Get
        Set(ByVal value As Decimal)
            _promoInclusivePrice = value
        End Set
    End Property


    Private _purchasePriceGross As Decimal
    Public Property Purchase_Price_Gross() As Decimal
        Get
            Return _purchasePriceGross
        End Get
        Set(ByVal value As Decimal)
            _purchasePriceGross = value
        End Set
    End Property
    Private _purchasePriceNet As Decimal
    Public Property Purchase_Price_Net() As Decimal
        Get
            Return _purchasePriceNet
        End Get
        Set(ByVal value As Decimal)
            _purchasePriceNet = value
        End Set
    End Property

    Private _purchasePriceTax As Decimal
    Public Property Purchase_Price_Tax() As Decimal
        Get
            Return _purchasePriceTax
        End Get
        Set(ByVal value As Decimal)
            _purchasePriceTax = value
        End Set
    End Property

    Private _FromPriceGross As Decimal
    Public Property From_Price_Gross() As Decimal
        Get
            Return _FromPriceGross
        End Get
        Set(ByVal value As Decimal)
            _FromPriceGross = value
        End Set
    End Property

    Private _FromPriceNet As Decimal
    Public Property From_Price_Net() As Decimal
        Get
            Return _FromPriceNet
        End Get
        Set(ByVal value As Decimal)
            _FromPriceNet = value
        End Set
    End Property

    Private _FromPriceTax As Decimal
    Public Property From_Price_Tax() As Decimal
        Get
            Return _FromPriceTax
        End Get
        Set(ByVal value As Decimal)
            _FromPriceTax = value
        End Set
    End Property


    Private _NET_PRICE_1 As Decimal
    Public Property NET_PRICE() As Decimal
        Get
            Return _NET_PRICE_1
        End Get
        Set(ByVal value As Decimal)
            _NET_PRICE_1 = value
        End Set
    End Property

    Private _GROSS_PRICE_1 As Decimal
    Public Property GROSS_PRICE() As Decimal
        Get
            Return _GROSS_PRICE_1
        End Get
        Set(ByVal value As Decimal)
            _GROSS_PRICE_1 = value
        End Set
    End Property


    Private _tariffCode As String
    Public Property TARIFF_CODE() As String
        Get
            Return _tariffCode
        End Get
        Set(ByVal value As String)
            _tariffCode = value
        End Set
    End Property

    Private _taxCode As String
    Public Property TAX_CODE() As String
        Get
            Return _taxCode
        End Get
        Set(ByVal value As String)
            _taxCode = value
        End Set
    End Property


    Private _TAX_AMOUNT_1 As Decimal
    Public Property TAX_AMOUNT() As Decimal
        Get
            Return _TAX_AMOUNT_1
        End Get
        Set(ByVal value As Decimal)
            _TAX_AMOUNT_1 = value
        End Set
    End Property

    Private _PRICE_BREAK_QUANTITY_1 As Decimal
    Public Property PRICE_BREAK_QUANTITY_1() As Decimal
        Get
            Return _PRICE_BREAK_QUANTITY_1
        End Get
        Set(ByVal value As Decimal)
            _PRICE_BREAK_QUANTITY_1 = value
        End Set
    End Property

    Private _SALE_NET_PRICE_1 As Decimal
    Public Property SALE_NET_PRICE() As Decimal
        Get
            Return _SALE_NET_PRICE_1
        End Get
        Set(ByVal value As Decimal)
            _SALE_NET_PRICE_1 = value
        End Set
    End Property

    Private _SALE_GROSS_PRICE_1 As Decimal
    Public Property SALE_GROSS_PRICE() As Decimal
        Get
            Return _SALE_GROSS_PRICE_1
        End Get
        Set(ByVal value As Decimal)
            _SALE_GROSS_PRICE_1 = value
        End Set
    End Property

    Private _SALE_TAX_AMOUNT_1 As Decimal
    Public Property SALE_TAX_AMOUNT() As Decimal
        Get
            Return _SALE_TAX_AMOUNT_1
        End Get
        Set(ByVal value As Decimal)
            _SALE_TAX_AMOUNT_1 = value
        End Set
    End Property

    Private _SALE_PRICE_BREAK_QUANTITY_1 As Decimal
    Public Property SALE_PRICE_BREAK_QUANTITY_1() As Decimal
        Get
            Return _SALE_PRICE_BREAK_QUANTITY_1
        End Get
        Set(ByVal value As Decimal)
            _SALE_PRICE_BREAK_QUANTITY_1 = value
        End Set
    End Property

    Private _NET_PRICE_2 As Decimal
    Public Property NET_PRICE_2() As Decimal
        Get
            Return _NET_PRICE_2
        End Get
        Set(ByVal value As Decimal)
            _NET_PRICE_2 = value
        End Set
    End Property

    Private _GROSS_PRICE_2 As Decimal
    Public Property GROSS_PRICE_2() As Decimal
        Get
            Return _GROSS_PRICE_2
        End Get
        Set(ByVal value As Decimal)
            _GROSS_PRICE_2 = value
        End Set
    End Property

    Private _TAX_AMOUNT_2 As Decimal
    Public Property TAX_AMOUNT_2() As Decimal
        Get
            Return _TAX_AMOUNT_2
        End Get
        Set(ByVal value As Decimal)
            _TAX_AMOUNT_2 = value
        End Set
    End Property

    Private _PRICE_BREAK_QUANTITY_2 As Decimal
    Public Property PRICE_BREAK_QUANTITY_2() As Decimal
        Get
            Return _PRICE_BREAK_QUANTITY_2
        End Get
        Set(ByVal value As Decimal)
            _PRICE_BREAK_QUANTITY_2 = value
        End Set
    End Property

    Private _SALE_NET_PRICE_2 As Decimal
    Public Property SALE_NET_PRICE_2() As Decimal
        Get
            Return _SALE_NET_PRICE_2
        End Get
        Set(ByVal value As Decimal)
            _SALE_NET_PRICE_2 = value
        End Set
    End Property

    Private _SALE_GROSS_PRICE_2 As Decimal
    Public Property SALE_GROSS_PRICE_2() As Decimal
        Get
            Return _SALE_GROSS_PRICE_2
        End Get
        Set(ByVal value As Decimal)
            _SALE_GROSS_PRICE_2 = value
        End Set
    End Property

    Private _SALE_TAX_AMOUNT_2 As Decimal
    Public Property SALE_TAX_AMOUNT_2() As Decimal
        Get
            Return _SALE_TAX_AMOUNT_2
        End Get
        Set(ByVal value As Decimal)
            _SALE_TAX_AMOUNT_2 = value
        End Set
    End Property

    Private _SALE_PRICE_BREAK_QUANTITY_2 As Decimal
    Public Property SALE_PRICE_BREAK_QUANTITY_2() As Decimal
        Get
            Return _SALE_PRICE_BREAK_QUANTITY_2
        End Get
        Set(ByVal value As Decimal)
            _SALE_PRICE_BREAK_QUANTITY_2 = value
        End Set
    End Property

    Private _NET_PRICE_3 As Decimal
    Public Property NET_PRICE_3() As Decimal
        Get
            Return _NET_PRICE_3
        End Get
        Set(ByVal value As Decimal)
            _NET_PRICE_3 = value
        End Set
    End Property

    Private _GROSS_PRICE_3 As Decimal
    Public Property GROSS_PRICE_3() As Decimal
        Get
            Return _GROSS_PRICE_3
        End Get
        Set(ByVal value As Decimal)
            _GROSS_PRICE_3 = value
        End Set
    End Property

    Private _TAX_AMOUNT_3 As Decimal
    Public Property TAX_AMOUNT_3() As Decimal
        Get
            Return _TAX_AMOUNT_3
        End Get
        Set(ByVal value As Decimal)
            _TAX_AMOUNT_3 = value
        End Set
    End Property

    Private _PRICE_BREAK_QUANTITY_3 As Decimal
    Public Property PRICE_BREAK_QUANTITY_3() As Decimal
        Get
            Return _PRICE_BREAK_QUANTITY_3
        End Get
        Set(ByVal value As Decimal)
            _PRICE_BREAK_QUANTITY_3 = value
        End Set
    End Property

    Private _SALE_NET_PRICE_3 As Decimal
    Public Property SALE_NET_PRICE_3() As Decimal
        Get
            Return _SALE_NET_PRICE_3
        End Get
        Set(ByVal value As Decimal)
            _SALE_NET_PRICE_3 = value
        End Set
    End Property

    Private _SALE_GROSS_PRICE_3 As Decimal
    Public Property SALE_GROSS_PRICE_3() As Decimal
        Get
            Return _SALE_GROSS_PRICE_3
        End Get
        Set(ByVal value As Decimal)
            _SALE_GROSS_PRICE_3 = value
        End Set
    End Property

    Private _SALE_TAX_AMOUNT_3 As Decimal
    Public Property SALE_TAX_AMOUNT_3() As Decimal
        Get
            Return _SALE_TAX_AMOUNT_3
        End Get
        Set(ByVal value As Decimal)
            _SALE_TAX_AMOUNT_3 = value
        End Set
    End Property

    Private _SALE_PRICE_BREAK_QUANTITY_3 As Decimal
    Public Property SALE_PRICE_BREAK_QUANTITY_3() As Decimal
        Get
            Return _SALE_PRICE_BREAK_QUANTITY_3
        End Get
        Set(ByVal value As Decimal)
            _SALE_PRICE_BREAK_QUANTITY_3 = value
        End Set
    End Property


    Private _NET_PRICE_4 As Decimal
    Public Property NET_PRICE_4() As Decimal
        Get
            Return _NET_PRICE_4
        End Get
        Set(ByVal value As Decimal)
            _NET_PRICE_4 = value
        End Set
    End Property

    Private _GROSS_PRICE_4 As Decimal
    Public Property GROSS_PRICE_4() As Decimal
        Get
            Return _GROSS_PRICE_4
        End Get
        Set(ByVal value As Decimal)
            _GROSS_PRICE_4 = value
        End Set
    End Property

    Private _TAX_AMOUNT_4 As Decimal
    Public Property TAX_AMOUNT_4() As Decimal
        Get
            Return _TAX_AMOUNT_4
        End Get
        Set(ByVal value As Decimal)
            _TAX_AMOUNT_4 = value
        End Set
    End Property

    Private _PRICE_BREAK_QUANTITY_4 As Decimal
    Public Property PRICE_BREAK_QUANTITY_4() As Decimal
        Get
            Return _PRICE_BREAK_QUANTITY_4
        End Get
        Set(ByVal value As Decimal)
            _PRICE_BREAK_QUANTITY_4 = value
        End Set
    End Property

    Private _SALE_NET_PRICE_4 As Decimal
    Public Property SALE_NET_PRICE_4() As Decimal
        Get
            Return _SALE_NET_PRICE_4
        End Get
        Set(ByVal value As Decimal)
            _SALE_NET_PRICE_4 = value
        End Set
    End Property

    Private _SALE_GROSS_PRICE_4 As Decimal
    Public Property SALE_GROSS_PRICE_4() As Decimal
        Get
            Return _SALE_GROSS_PRICE_4
        End Get
        Set(ByVal value As Decimal)
            _SALE_GROSS_PRICE_4 = value
        End Set
    End Property

    Private _SALE_TAX_AMOUNT_4 As Decimal
    Public Property SALE_TAX_AMOUNT_4() As Decimal
        Get
            Return _SALE_TAX_AMOUNT_4
        End Get
        Set(ByVal value As Decimal)
            _SALE_TAX_AMOUNT_4 = value
        End Set
    End Property

    Private _SALE_PRICE_BREAK_QUANTITY_4 As Decimal
    Public Property SALE_PRICE_BREAK_QUANTITY_4() As Decimal
        Get
            Return _SALE_PRICE_BREAK_QUANTITY_4
        End Get
        Set(ByVal value As Decimal)
            _SALE_PRICE_BREAK_QUANTITY_4 = value
        End Set
    End Property


    Private _NET_PRICE_5 As Decimal
    Public Property NET_PRICE_5() As Decimal
        Get
            Return _NET_PRICE_5
        End Get
        Set(ByVal value As Decimal)
            _NET_PRICE_5 = value
        End Set
    End Property

    Private _GROSS_PRICE_5 As Decimal
    Public Property GROSS_PRICE_5() As Decimal
        Get
            Return _GROSS_PRICE_5
        End Get
        Set(ByVal value As Decimal)
            _GROSS_PRICE_5 = value
        End Set
    End Property

    Private _TAX_AMOUNT_5 As Decimal
    Public Property TAX_AMOUNT_5() As Decimal
        Get
            Return _TAX_AMOUNT_5
        End Get
        Set(ByVal value As Decimal)
            _TAX_AMOUNT_5 = value
        End Set
    End Property

    Private _PRICE_BREAK_QUANTITY_5 As Decimal
    Public Property PRICE_BREAK_QUANTITY_5() As Decimal
        Get
            Return _PRICE_BREAK_QUANTITY_5
        End Get
        Set(ByVal value As Decimal)
            _PRICE_BREAK_QUANTITY_5 = value
        End Set
    End Property

    Private _SALE_NET_PRICE_5 As Decimal
    Public Property SALE_NET_PRICE_5() As Decimal
        Get
            Return _SALE_NET_PRICE_5
        End Get
        Set(ByVal value As Decimal)
            _SALE_NET_PRICE_5 = value
        End Set
    End Property

    Private _SALE_GROSS_PRICE_5 As Decimal
    Public Property SALE_GROSS_PRICE_5() As Decimal
        Get
            Return _SALE_GROSS_PRICE_5
        End Get
        Set(ByVal value As Decimal)
            _SALE_GROSS_PRICE_5 = value
        End Set
    End Property

    Private _SALE_TAX_AMOUNT_5 As Decimal
    Public Property SALE_TAX_AMOUNT_5() As Decimal
        Get
            Return _SALE_TAX_AMOUNT_5
        End Get
        Set(ByVal value As Decimal)
            _SALE_TAX_AMOUNT_5 = value
        End Set
    End Property

    Private _SALE_PRICE_BREAK_QUANTITY_5 As Decimal
    Public Property SALE_PRICE_BREAK_QUANTITY_5() As Decimal
        Get
            Return _SALE_PRICE_BREAK_QUANTITY_5
        End Get
        Set(ByVal value As Decimal)
            _SALE_PRICE_BREAK_QUANTITY_5 = value
        End Set
    End Property

    Private _NET_PRICE_6 As Decimal
    Public Property NET_PRICE_6() As Decimal
        Get
            Return _NET_PRICE_6
        End Get
        Set(ByVal value As Decimal)
            _NET_PRICE_6 = value
        End Set
    End Property

    Private _GROSS_PRICE_6 As Decimal
    Public Property GROSS_PRICE_6() As Decimal
        Get
            Return _GROSS_PRICE_6
        End Get
        Set(ByVal value As Decimal)
            _GROSS_PRICE_6 = value
        End Set
    End Property

    Private _TAX_AMOUNT_6 As Decimal
    Public Property TAX_AMOUNT_6() As Decimal
        Get
            Return _TAX_AMOUNT_6
        End Get
        Set(ByVal value As Decimal)
            _TAX_AMOUNT_6 = value
        End Set
    End Property

    Private _PRICE_BREAK_QUANTITY_6 As Decimal
    Public Property PRICE_BREAK_QUANTITY_6() As Decimal
        Get
            Return _PRICE_BREAK_QUANTITY_6
        End Get
        Set(ByVal value As Decimal)
            _PRICE_BREAK_QUANTITY_6 = value
        End Set
    End Property

    Private _SALE_NET_PRICE_6 As Decimal
    Public Property SALE_NET_PRICE_6() As Decimal
        Get
            Return _SALE_NET_PRICE_6
        End Get
        Set(ByVal value As Decimal)
            _SALE_NET_PRICE_6 = value
        End Set
    End Property

    Private _SALE_GROSS_PRICE_6 As Decimal
    Public Property SALE_GROSS_PRICE_6() As Decimal
        Get
            Return _SALE_GROSS_PRICE_6
        End Get
        Set(ByVal value As Decimal)
            _SALE_GROSS_PRICE_6 = value
        End Set
    End Property

    Private _SALE_TAX_AMOUNT_6 As Decimal
    Public Property SALE_TAX_AMOUNT_6() As Decimal
        Get
            Return _SALE_TAX_AMOUNT_6
        End Get
        Set(ByVal value As Decimal)
            _SALE_TAX_AMOUNT_6 = value
        End Set
    End Property

    Private _SALE_PRICE_BREAK_QUANTITY_6 As Decimal
    Public Property SALE_PRICE_BREAK_QUANTITY_6() As Decimal
        Get
            Return _SALE_PRICE_BREAK_QUANTITY_6
        End Get
        Set(ByVal value As Decimal)
            _SALE_PRICE_BREAK_QUANTITY_6 = value
        End Set
    End Property

    Private _NET_PRICE_7 As Decimal
    Public Property NET_PRICE_7() As Decimal
        Get
            Return _NET_PRICE_7
        End Get
        Set(ByVal value As Decimal)
            _NET_PRICE_7 = value
        End Set
    End Property

    Private _GROSS_PRICE_7 As Decimal
    Public Property GROSS_PRICE_7() As Decimal
        Get
            Return _GROSS_PRICE_7
        End Get
        Set(ByVal value As Decimal)
            _GROSS_PRICE_7 = value
        End Set
    End Property

    Private _TAX_AMOUNT_7 As Decimal
    Public Property TAX_AMOUNT_7() As Decimal
        Get
            Return _TAX_AMOUNT_7
        End Get
        Set(ByVal value As Decimal)
            _TAX_AMOUNT_7 = value
        End Set
    End Property

    Private _PRICE_BREAK_QUANTITY_7 As Decimal
    Public Property PRICE_BREAK_QUANTITY_7() As Decimal
        Get
            Return _PRICE_BREAK_QUANTITY_7
        End Get
        Set(ByVal value As Decimal)
            _PRICE_BREAK_QUANTITY_7 = value
        End Set
    End Property

    Private _SALE_NET_PRICE_7 As Decimal
    Public Property SALE_NET_PRICE_7() As Decimal
        Get
            Return _SALE_NET_PRICE_7
        End Get
        Set(ByVal value As Decimal)
            _SALE_NET_PRICE_7 = value
        End Set
    End Property

    Private _SALE_GROSS_PRICE_7 As Decimal
    Public Property SALE_GROSS_PRICE_7() As Decimal
        Get
            Return _SALE_GROSS_PRICE_7
        End Get
        Set(ByVal value As Decimal)
            _SALE_GROSS_PRICE_7 = value
        End Set
    End Property

    Private _SALE_TAX_AMOUNT_7 As Decimal
    Public Property SALE_TAX_AMOUNT_7() As Decimal
        Get
            Return _SALE_TAX_AMOUNT_7
        End Get
        Set(ByVal value As Decimal)
            _SALE_TAX_AMOUNT_7 = value
        End Set
    End Property

    Private _SALE_PRICE_BREAK_QUANTITY_7 As Decimal
    Public Property SALE_PRICE_BREAK_QUANTITY_7() As Decimal
        Get
            Return _SALE_PRICE_BREAK_QUANTITY_7
        End Get
        Set(ByVal value As Decimal)
            _SALE_PRICE_BREAK_QUANTITY_7 = value
        End Set
    End Property

    Private _NET_PRICE_8 As Decimal
    Public Property NET_PRICE_8() As Decimal
        Get
            Return _NET_PRICE_8
        End Get
        Set(ByVal value As Decimal)
            _NET_PRICE_8 = value
        End Set
    End Property

    Private _GROSS_PRICE_8 As Decimal
    Public Property GROSS_PRICE_8() As Decimal
        Get
            Return _GROSS_PRICE_8
        End Get
        Set(ByVal value As Decimal)
            _GROSS_PRICE_8 = value
        End Set
    End Property

    Private _TAX_AMOUNT_8 As Decimal
    Public Property TAX_AMOUNT_8() As Decimal
        Get
            Return _TAX_AMOUNT_8
        End Get
        Set(ByVal value As Decimal)
            _TAX_AMOUNT_8 = value
        End Set
    End Property

    Private _PRICE_BREAK_QUANTITY_8 As Decimal
    Public Property PRICE_BREAK_QUANTITY_8() As Decimal
        Get
            Return _PRICE_BREAK_QUANTITY_8
        End Get
        Set(ByVal value As Decimal)
            _PRICE_BREAK_QUANTITY_8 = value
        End Set
    End Property

    Private _SALE_NET_PRICE_8 As Decimal
    Public Property SALE_NET_PRICE_8() As Decimal
        Get
            Return _SALE_NET_PRICE_8
        End Get
        Set(ByVal value As Decimal)
            _SALE_NET_PRICE_8 = value
        End Set
    End Property

    Private _SALE_GROSS_PRICE_8 As Decimal
    Public Property SALE_GROSS_PRICE_8() As Decimal
        Get
            Return _SALE_GROSS_PRICE_8
        End Get
        Set(ByVal value As Decimal)
            _SALE_GROSS_PRICE_8 = value
        End Set
    End Property

    Private _SALE_TAX_AMOUNT_8 As Decimal
    Public Property SALE_TAX_AMOUNT_8() As Decimal
        Get
            Return _SALE_TAX_AMOUNT_8
        End Get
        Set(ByVal value As Decimal)
            _SALE_TAX_AMOUNT_8 = value
        End Set
    End Property

    Private _SALE_PRICE_BREAK_QUANTITY_8 As Decimal
    Public Property SALE_PRICE_BREAK_QUANTITY_8() As Decimal
        Get
            Return _SALE_PRICE_BREAK_QUANTITY_8
        End Get
        Set(ByVal value As Decimal)
            _SALE_PRICE_BREAK_QUANTITY_8 = value
        End Set
    End Property

    Private _NET_PRICE_9 As Decimal
    Public Property NET_PRICE_9() As Decimal
        Get
            Return _NET_PRICE_9
        End Get
        Set(ByVal value As Decimal)
            _NET_PRICE_9 = value
        End Set
    End Property

    Private _GROSS_PRICE_9 As Decimal
    Public Property GROSS_PRICE_9() As Decimal
        Get
            Return _GROSS_PRICE_9
        End Get
        Set(ByVal value As Decimal)
            _GROSS_PRICE_9 = value
        End Set
    End Property

    Private _TAX_AMOUNT_9 As Decimal
    Public Property TAX_AMOUNT_9() As Decimal
        Get
            Return _TAX_AMOUNT_9
        End Get
        Set(ByVal value As Decimal)
            _TAX_AMOUNT_9 = value
        End Set
    End Property

    Private _PRICE_BREAK_QUANTITY_9 As Decimal
    Public Property PRICE_BREAK_QUANTITY_9() As Decimal
        Get
            Return _PRICE_BREAK_QUANTITY_9
        End Get
        Set(ByVal value As Decimal)
            _PRICE_BREAK_QUANTITY_9 = value
        End Set
    End Property

    Private _SALE_NET_PRICE_9 As Decimal
    Public Property SALE_NET_PRICE_9() As Decimal
        Get
            Return _SALE_NET_PRICE_9
        End Get
        Set(ByVal value As Decimal)
            _SALE_NET_PRICE_9 = value
        End Set
    End Property

    Private _SALE_GROSS_PRICE_9 As Decimal
    Public Property SALE_GROSS_PRICE_9() As Decimal
        Get
            Return _SALE_GROSS_PRICE_9
        End Get
        Set(ByVal value As Decimal)
            _SALE_GROSS_PRICE_9 = value
        End Set
    End Property

    Private _SALE_TAX_AMOUNT_9 As Decimal
    Public Property SALE_TAX_AMOUNT_9() As Decimal
        Get
            Return _SALE_TAX_AMOUNT_9
        End Get
        Set(ByVal value As Decimal)
            _SALE_TAX_AMOUNT_9 = value
        End Set
    End Property

    Private _SALE_PRICE_BREAK_QUANTITY_9 As Decimal
    Public Property SALE_PRICE_BREAK_QUANTITY_9() As Decimal
        Get
            Return _SALE_PRICE_BREAK_QUANTITY_9
        End Get
        Set(ByVal value As Decimal)
            _SALE_PRICE_BREAK_QUANTITY_9 = value
        End Set
    End Property

    Private _NET_PRICE_10 As Decimal
    Public Property NET_PRICE_10() As Decimal
        Get
            Return _NET_PRICE_10
        End Get
        Set(ByVal value As Decimal)
            _NET_PRICE_10 = value
        End Set
    End Property

    Private _GROSS_PRICE_10 As Decimal
    Public Property GROSS_PRICE_10() As Decimal
        Get
            Return _GROSS_PRICE_10
        End Get
        Set(ByVal value As Decimal)
            _GROSS_PRICE_10 = value
        End Set
    End Property

    Private _TAX_AMOUNT_10 As Decimal
    Public Property TAX_AMOUNT_10() As Decimal
        Get
            Return _TAX_AMOUNT_10
        End Get
        Set(ByVal value As Decimal)
            _TAX_AMOUNT_10 = value
        End Set
    End Property

    Private _PRICE_BREAK_QUANTITY_10 As Decimal
    Public Property PRICE_BREAK_QUANTITY_10() As Decimal
        Get
            Return _PRICE_BREAK_QUANTITY_10
        End Get
        Set(ByVal value As Decimal)
            _PRICE_BREAK_QUANTITY_10 = value
        End Set
    End Property

    Private _SALE_NET_PRICE_10 As Decimal
    Public Property SALE_NET_PRICE_10() As Decimal
        Get
            Return _SALE_NET_PRICE_10
        End Get
        Set(ByVal value As Decimal)
            _SALE_NET_PRICE_10 = value
        End Set
    End Property

    Private _SALE_GROSS_PRICE_10 As Decimal
    Public Property SALE_GROSS_PRICE_10() As Decimal
        Get
            Return _SALE_GROSS_PRICE_10
        End Get
        Set(ByVal value As Decimal)
            _SALE_GROSS_PRICE_10 = value
        End Set
    End Property

    Private _SALE_TAX_AMOUNT_10 As Decimal
    Public Property SALE_TAX_AMOUNT_10() As Decimal
        Get
            Return _SALE_TAX_AMOUNT_10
        End Get
        Set(ByVal value As Decimal)
            _SALE_TAX_AMOUNT_10 = value
        End Set
    End Property

    Private _SALE_PRICE_BREAK_QUANTITY_10 As Decimal
    Public Property SALE_PRICE_BREAK_QUANTITY_10() As Decimal
        Get
            Return _SALE_PRICE_BREAK_QUANTITY_10
        End Get
        Set(ByVal value As Decimal)
            _SALE_PRICE_BREAK_QUANTITY_10 = value
        End Set
    End Property


    Private _delNetValue As Decimal
    Public Property DELIVERY_GROSS_PRICE() As Decimal
        Get
            Return _delNetValue
        End Get
        Set(ByVal value As Decimal)
            _delNetValue = value
        End Set
    End Property

    Private _delNetPrice As Decimal
    Public Property DELIVERY_NET_PRICE() As Decimal
        Get
            Return _delNetPrice
        End Get
        Set(ByVal value As Decimal)
            _delNetPrice = value
        End Set
    End Property

    Private _delTaxAmount As Decimal
    Public Property DELIVERY_TAX_AMOUNT() As Decimal
        Get
            Return _delTaxAmount
        End Get
        Set(ByVal value As Decimal)
            _delTaxAmount = value
        End Set
    End Property

    Private _priceFound As Boolean
    Public Property PriceFound() As Boolean
        Get
            Return _priceFound
        End Get
        Set(ByVal value As Boolean)
            _priceFound = value
        End Set
    End Property

    Private _webDisplayPrice As Decimal
    Public Property DisplayPrice() As Decimal
        Get
            Return _webDisplayPrice
        End Get
        Set(ByVal value As Decimal)
            _webDisplayPrice = value
        End Set
    End Property

    Private _fromDisplayPrice As Decimal
    Public Property DisplayPrice_From() As Decimal
        Get
            Return _fromDisplayPrice
        End Get
        Set(ByVal value As Decimal)
            _fromDisplayPrice = value
        End Set
    End Property

    Private _purchasedProductCode As String
    Public Property PurchasedProductCode() As String
        Get
            Return _purchasedProductCode
        End Get
        Set(ByVal value As String)
            _purchasedProductCode = value
        End Set
    End Property

End Class
