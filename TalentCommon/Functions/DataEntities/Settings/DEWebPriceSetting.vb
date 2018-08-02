Public Class DEWebPriceSetting
    Inherits DESettings

    Private _checkForAllPartnerIfNotFound As Boolean
    Public Property UseAllStringToCheckPartner() As Boolean
        Get
            Return _checkForAllPartnerIfNotFound
        End Get
        Set(ByVal value As Boolean)
            _checkForAllPartnerIfNotFound = value
        End Set
    End Property


    Private _priceList As String
    Public Property PriceList() As String
        Get
            Return _priceList
        End Get
        Set(ByVal value As String)
            _priceList = value
        End Set
    End Property


    Private _promoCode As String
    Public Property PromotionCode() As String
        Get
            Return _promoCode
        End Get
        Set(ByVal value As String)
            _promoCode = value
        End Set
    End Property


    Private _invalidPromoerror As String
    Public Property InvalidPromotionCodeErrorMessage() As String
        Get
            Return _invalidPromoerror
        End Get
        Set(ByVal value As String)
            _invalidPromoerror = value
        End Set
    End Property


    Private _tempOrderID As String
    Public Property TempOrderID() As String
        Get
            Return _tempOrderID
        End Get
        Set(ByVal value As String)
            _tempOrderID = value
        End Set
    End Property

    Private _promoPriority As String
    Public Property PromotionTypePriority() As String
        Get
            Return _promoPriority
        End Get
        Set(ByVal value As String)
            _promoPriority = value
        End Set
    End Property

    Private _basketHeaderID As String
    Public Property BasketHeaderID() As String
        Get
            Return _basketHeaderID
        End Get
        Set(ByVal value As String)
            _basketHeaderID = value
        End Set
    End Property

    Private _username As String
    Public Property Username() As String
        Get
            Return _username
        End Get
        Set(ByVal value As String)
            _username = value
        End Set
    End Property


    Private _partnerGroup As String
    Public Property PartnerGroup() As String
        Get
            Return _partnerGroup
        End Get
        Set(ByVal value As String)
            _partnerGroup = value
        End Set
    End Property


    Private _secondaryPriceList As String
    Public Property SecondaryPriceList() As String
        Get
            Return _secondaryPriceList
        End Get
        Set(ByVal value As String)
            _secondaryPriceList = value
        End Set
    End Property

    Private _UsersAttributeList As String
    Public Property UsersAttributeList() As String
        Get
            Return _UsersAttributeList
        End Get
        Set(ByVal value As String)
            _UsersAttributeList = value
        End Set
    End Property


    Public Sub New(ByVal frontEnd_Connectionstring As String, _
                    ByVal destination_Database As String, _
                    ByVal company_ As String, _
                    ByVal business_unit As String, _
                    ByVal partner_ As String, _
                    ByVal price_list As String, _
                    ByVal useAllStringPartner As Boolean, _
                    ByVal languageCode_ As String, _
                    ByVal partner_group As String, _
                    ByVal secondary_pricelist As String, _
                    ByVal user_attribute_list As String)
        MyBase.New()
        Me.PriceList = price_list
        Me.UseAllStringToCheckPartner = useAllStringPartner
        Me.FrontEndConnectionString = frontEnd_Connectionstring
        Me.DestinationDatabase = destination_Database
        Me.Company = company_
        Me.BusinessUnit = business_unit
        Me.Partner = partner_
        Me.Language = languageCode_
        Me.PartnerGroup = partner_group
        Me.SecondaryPriceList = secondary_pricelist
        Me.UsersAttributeList = user_attribute_list
    End Sub

End Class
