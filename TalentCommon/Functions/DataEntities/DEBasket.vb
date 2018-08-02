<Serializable()> _
Public Class DEBasket

    Private _basketItems As New List(Of DEBasketItem)
    Public Property BasketItems() As List(Of DEBasketItem)
        Get
            Return _basketItems
        End Get
        Set(ByVal value As List(Of DEBasketItem))
            _basketItems = value
        End Set
    End Property
    Private _basketSummary As New DEBasketSummary
    Public Property BasketSummary() As DEBasketSummary
        Get
            Return _basketSummary
        End Get
        Set(ByVal value As DEBasketSummary)
            _basketSummary = value
        End Set
    End Property

    Public ReadOnly Property IsEmpty() As Boolean
        Get
            If Me.BasketItems.Count = 0 Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Private _isDirty As Boolean
    Public Property IsDirty() As Boolean
        Get
            Return _isDirty
        End Get
        Set(ByVal value As Boolean)
            _isDirty = value
        End Set
    End Property
    Private _masterProducts As Generic.Dictionary(Of String, WebPriceProduct)
    ''' <summary>
    ''' Gets or sets the list of master products of type WebPriceProduct
    ''' </summary>
    ''' <value>The master products.</value>
    Public Property MasterProducts() As Generic.Dictionary(Of String, WebPriceProduct)
        Get
            If (_masterProducts Is Nothing) Then
                _masterProducts = New Generic.Dictionary(Of String, WebPriceProduct)
            End If
            Return _masterProducts
        End Get
        Set(ByVal value As Generic.Dictionary(Of String, WebPriceProduct))
            _masterProducts = value
        End Set
    End Property

    Private _masterProductsPriceList As Generic.Dictionary(Of String, Talent.Common.DEWebPrice)
    ''' <summary>
    ''' Gets or sets the master products price list of type DEWebPrice
    ''' </summary>
    ''' <value>The master products price list.</value>
    Public Property MasterProductsPriceList() As Generic.Dictionary(Of String, Talent.Common.DEWebPrice)
        Get
            Return _masterProductsPriceList
        End Get
        Set(ByVal value As Generic.Dictionary(Of String, Talent.Common.DEWebPrice))
            _masterProductsPriceList = value
        End Set
    End Property
    Private _webPrices As Talent.Common.TalentWebPricing
    ''' <summary>
    ''' Gets or sets the web prices of type TalentWebPricing for the retail items in the basket
    ''' </summary>
    ''' <value>The web prices.</value>
    Public Property WebPrices() As Talent.Common.TalentWebPricing
        Get
            Return _webPrices
        End Get
        Set(ByVal value As Talent.Common.TalentWebPricing)
            _webPrices = value
        End Set
    End Property

    ''' <summary>
    ''' This version will return the TempOrderID regardless of whether or not
    ''' it has been assigned a value - will only ever have a value at checkout 
    ''' order confirmation stage (after payment).
    ''' </summary>
    ''' <remarks></remarks>
    Private _temp_Order_Id As String
    Public Property Temp_Order_Id() As String
        Get
            Return _temp_Order_Id
        End Get
        Set(ByVal value As String)
            _temp_Order_Id = value
        End Set
    End Property

    Private _basketHeaderID As String
    Public Property Basket_Header_ID() As String
        Get
            Return _basketHeaderID
        End Get
        Set(ByVal value As String)
            _basketHeaderID = value
        End Set
    End Property

    Private _created As Date
    Public Property Created_Date() As Date
        Get
            Return _created
        End Get
        Set(ByVal value As Date)
            _created = value
        End Set
    End Property

    Private _accessed As Date
    Public Property Last_Accessed_Date() As Date
        Get
            Return _accessed
        End Get
        Set(ByVal value As Date)
            _accessed = value
        End Set
    End Property

    Private _processed As Boolean
    Public Property Processed() As Boolean
        Get
            Return _processed
        End Get
        Set(ByVal value As Boolean)
            _processed = value
        End Set
    End Property

    Private _stockError As Boolean
    Public Property STOCK_ERROR() As Boolean
        Get
            If _stockError = Nothing Then
                Return False
            End If
            Return _stockError
        End Get
        Set(ByVal value As Boolean)
            _stockError = value
        End Set
    End Property

    Private _marketingCampaign As Boolean
    Public Property MARKETING_CAMPAIGN() As Boolean
        Get
            If _marketingCampaign = Nothing Then
                Return False
            End If
            Return _marketingCampaign
        End Get
        Set(ByVal value As Boolean)
            _marketingCampaign = value
        End Set
    End Property

    Private _userFulfilment As String
    Public Property USER_SELECT_FULFIL() As String
        Get
            If _userFulfilment = Nothing Then
                Return "N"
            End If
            Return _userFulfilment
        End Get
        Set(ByVal value As String)
            _userFulfilment = value
        End Set
    End Property

    Private _PaymentOptions As String
    Public Property PAYMENT_OPTIONS() As String
        Get
            If _PaymentOptions = Nothing Then
                Return ""
            End If
            Return _PaymentOptions
        End Get
        Set(ByVal value As String)
            _PaymentOptions = value
        End Set
    End Property

    Private _RestrictPaymentOptions As Boolean
    Public Property RESTRICT_PAYMENT_OPTIONS() As Boolean
        Get
            Return _RestrictPaymentOptions
        End Get
        Set(ByVal value As Boolean)
            _RestrictPaymentOptions = value
        End Set
    End Property

    Private _CampaignCode As String
    Public Property CAMPAIGN_CODE As String
        Get
            Return _CampaignCode
        End Get
        Set(ByVal value As String)
            _CampaignCode = value
        End Set
    End Property

    Private _PaymentAccountId As String
    Public Property PAYMENT_ACCOUNT_ID As String
        Get
            Return _PaymentAccountId
        End Get
        Set(ByVal value As String)
            _PaymentAccountId = value
        End Set
    End Property

    Private _externalPaymentToken As String
    Public Property EXTERNAL_PAYMENT_TOKEN As String
        Get
            Return _externalPaymentToken
        End Get
        Set(ByVal value As String)
            _externalPaymentToken = value
        End Set
    End Property

    Private _catMode As String
    ''' <summary>
    ''' Gets or sets the CAT mode (cancel, amend or transfer)
    ''' </summary>
    ''' <value>
    ''' The CAT MODE.
    ''' </value>
    Public Property CAT_MODE As String
        Get
            Return _catMode
        End Get
        Set(ByVal value As String)
            _catMode = value
        End Set
    End Property

    Private _catPrice As Decimal
    ''' <summary>
    ''' Gets or sets the CAT price 
    ''' </summary>
    ''' <value>
    ''' The CAT price of the basket being cancelled.
    ''' </value>
    Public Property CAT_PRICE As Decimal
        Get
            Return _catPrice
        End Get
        Set(ByVal value As Decimal)
            _catPrice = value
        End Set
    End Property


    Private _paymentType As String
    Public Property PAYMENT_TYPE As String
        Get
            Return _paymentType
        End Get
        Set(ByVal value As String)
            _paymentType = value
        End Set
    End Property

    Private _cardTypeCode As String
    Public Property CARD_TYPE_CODE As String
        Get
            Return _cardTypeCode
        End Get
        Set(ByVal value As String)
            _cardTypeCode = value
        End Set
    End Property

    Private _OriginalSalePaidWithCF As Boolean
    Public Property ORIGINAL_SALE_PAID_WITH_CF() As Boolean
        Get
            Return _OriginalSalePaidWithCF
        End Get
        Set(ByVal value As Boolean)
            _OriginalSalePaidWithCF = value
        End Set
    End Property

    Private _deliveryZoneCode As String
    Public Property DELIVERY_ZONE_CODE As String
        Get
            Return _deliveryZoneCode
        End Get
        Set(ByVal value As String)
            _deliveryZoneCode = value
        End Set
    End Property

    Private _deliveryCountryCode As String
    Public Property DELIVERY_COUNTRY_CODE As String
        Get
            Return _deliveryCountryCode
        End Get
        Set(ByVal value As String)
            _deliveryCountryCode = value
        End Set
    End Property
End Class
