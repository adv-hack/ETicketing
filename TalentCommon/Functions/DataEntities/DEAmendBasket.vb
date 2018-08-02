<Serializable()> _
Public Class DEAmendBasket

#Region "Class Level Fields"

    Private _collDETrans As New Collection      ' Transaction details
    Private _collDEAlerts As New Collection     ' Amend Basket details
    Private _addToBasket As Boolean = False
    Private _basketId As Long = 0
    Private _businessUnit As String = String.Empty
    Private _deleteBasket As Boolean = False
    Private _deleteFromBasket As Boolean = False
    Private _partnerCode As String = String.Empty
    Private _processed As Boolean = False
    Private _replaceBasket As Boolean = False
    Private _userID As String = String.Empty
    Private _priceList As String
    Private _isfree As Boolean
    Private _delFreeItems As Boolean
    Private _delMultipleBaskets As Boolean
    Private _basketIdList As Generic.List(Of String)
    Private _delModule As String
    Private _externalOrderNumber As String = String.Empty
    Private _basketVerificationMode As String = String.Empty

#End Region

#Region "Public Properties"

    Public Property AddFreeItems() As Boolean = False
    Public Property CollDETrans() As Collection
        Get
            Return _collDETrans
        End Get
        Set(ByVal value As Collection)
            _collDETrans = value
        End Set
    End Property
    Public Property CollDEAlerts() As Collection
        Get
            Return _collDEAlerts
        End Get
        Set(ByVal value As Collection)
            _collDEAlerts = value
        End Set
    End Property
    Public Property AddToBasket() As Boolean
        Get
            Return _addToBasket
        End Get
        Set(ByVal value As Boolean)
            _addToBasket = value
        End Set
    End Property
    Public Property BasketId() As Long
        Get
            Return _basketId
        End Get
        Set(ByVal value As Long)
            _basketId = value
        End Set
    End Property
    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property
    Public Property DeleteBasket() As Boolean
        Get
            Return _deleteBasket
        End Get
        Set(ByVal value As Boolean)
            _deleteBasket = value
        End Set
    End Property
    Public Property DeleteFromBasket() As Boolean
        Get
            Return _deleteFromBasket
        End Get
        Set(ByVal value As Boolean)
            _deleteFromBasket = value
        End Set
    End Property
    Public Property PartnerCode() As String
        Get
            Return _partnerCode
        End Get
        Set(ByVal value As String)
            _partnerCode = value
        End Set
    End Property
    Public Property Processed() As Boolean
        Get
            Return _processed
        End Get
        Set(ByVal value As Boolean)
            _processed = value
        End Set
    End Property
    Public Property ReplaceBasket() As Boolean
        Get
            Return _replaceBasket
        End Get
        Set(ByVal value As Boolean)
            _replaceBasket = value
        End Set
    End Property
    Public Property UserID() As String
        Get
            Return _userID
        End Get
        Set(ByVal value As String)
            _userID = value
        End Set
    End Property
    Public Property Price_List() As String
        Get
            Return _priceList
        End Get
        Set(ByVal value As String)
            _priceList = value
        End Set
    End Property
    Public Property IsFreeItem() As Boolean
        Get
            Return _isfree
        End Get
        Set(ByVal value As Boolean)
            _isfree = value
        End Set
    End Property
    Public Property DeleteFreeItems() As Boolean
        Get
            Return _delFreeItems
        End Get
        Set(ByVal value As Boolean)
            _delFreeItems = value
        End Set
    End Property
    Public Property DeleteMultipleBaskets() As Boolean
        Get
            Return _delMultipleBaskets
        End Get
        Set(ByVal value As Boolean)
            _delMultipleBaskets = value
        End Set
    End Property
    Public Property BasketIdList() As Generic.List(Of String)
        Get
            Return _basketIdList
        End Get
        Set(ByVal value As Generic.List(Of String))
            _basketIdList = value
        End Set
    End Property
    Public Property DeleteModule() As String
        Get
            Return _delModule
        End Get
        Set(ByVal value As String)
            _delModule = value
        End Set
    End Property
    Public Property ExternalOrderNumber() As String
        Get
            Return _externalOrderNumber
        End Get
        Set(ByVal value As String)
            _externalOrderNumber = value
        End Set
    End Property
    Public Property BasketVerificationMode() As String
        Get
            Return _basketVerificationMode
        End Get
        Set(ByVal value As String)
            _basketVerificationMode = value
        End Set
    End Property
#End Region


End Class
