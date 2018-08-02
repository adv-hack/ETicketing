
<Serializable()> _
Public Class DEDeliveryDetails

    Private _contactName As String = String.Empty
    Private _address1 As String = String.Empty
    Private _address2 As String = String.Empty
    Private _address3 As String = String.Empty
    Private _address4 As String = String.Empty
    Private _address5 As String = String.Empty
    Private _postcode As String = String.Empty
    Private _country As String = String.Empty
    Private _countryCode As String = String.Empty
    Private _addressMoniker As String = String.Empty

    Private _countrySelectedIndex As String = String.Empty
    Private _addressSelectedIndex As String = String.Empty

    Private _deliveryInstructions As String = String.Empty
    Private _purchaseOrder As String = String.Empty
    Private _preferredDate As String = String.Empty
    Private _giftMessageTo As String = String.Empty
    Private _giftMessage As String = String.Empty

    Private _printOption As String = String.Empty

    Public Property ContactName() As String
        Get
            Return _contactName
        End Get
        Set(ByVal value As String)
            _contactName = value
        End Set
    End Property
    Public Property Address1() As String
        Get
            Return _address1
        End Get
        Set(ByVal value As String)
            _address1 = value
        End Set
    End Property
    Public Property Address2() As String
        Get
            Return _address2
        End Get
        Set(ByVal value As String)
            _address2 = value
        End Set
    End Property
    Public Property Address3() As String
        Get
            Return _address3
        End Get
        Set(ByVal value As String)
            _address3 = value
        End Set
    End Property
    Public Property Address4() As String
        Get
            Return _address4
        End Get
        Set(ByVal value As String)
            _address4 = value
        End Set
    End Property
    Public Property Address5() As String
        Get
            Return _address5
        End Get
        Set(ByVal value As String)
            _address5 = value
        End Set
    End Property
    Public Property Postcode() As String
        Get
            Return _postcode
        End Get
        Set(ByVal value As String)
            _postcode = value
        End Set
    End Property
    Public Property Country() As String
        Get
            Return _country
        End Get
        Set(ByVal value As String)
            _country = value
        End Set
    End Property

    Public Property CountryCode() As String
        Get
            Return _countryCode
        End Get
        Set(ByVal value As String)
            _countryCode = value
        End Set
    End Property

    Public Property AddressMoniker() As String
        Get
            Return _addressMoniker
        End Get
        Set(ByVal value As String)
            _addressMoniker = value
        End Set
    End Property
    Public Property CountrySelectedIndex() As String
        Get
            Return _countrySelectedIndex
        End Get
        Set(ByVal value As String)
            _countrySelectedIndex = value
        End Set
    End Property
    Public Property AddressSelectedIndex() As String
        Get
            Return _addressSelectedIndex
        End Get
        Set(ByVal value As String)
            _addressSelectedIndex = value
        End Set
    End Property



    Public Property DeliveryInstructions() As String
        Get
            Return _deliveryInstructions
        End Get
        Set(ByVal value As String)
            _deliveryInstructions = value
        End Set
    End Property
    Public Property PurchaseOrder() As String
        Get
            Return _purchaseOrder
        End Get
        Set(ByVal value As String)
            _purchaseOrder = value
        End Set
    End Property
    Public Property PreferredDate() As String
        Get
            Return _preferredDate
        End Get
        Set(ByVal value As String)
            _preferredDate = value
        End Set
    End Property
    Public Property GiftMessageTo() As String
        Get
            Return _giftMessageTo
        End Get
        Set(ByVal value As String)
            _giftMessageTo = value
        End Set
    End Property
    Public Property GiftMessage() As String
        Get
            Return _giftMessage
        End Get
        Set(ByVal value As String)
            _giftMessage = value
        End Set
    End Property

    Public Property PrintOption() As String
        Get
            Return _printOption
        End Get
        Set(ByVal value As String)
            _printOption = value
        End Set
    End Property

End Class
