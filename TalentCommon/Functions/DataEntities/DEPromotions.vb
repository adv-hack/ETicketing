<Serializable()> _
Public Class DEPromotions

#Region "Public Properties"

    Public Property BasketTotal() As Decimal
    Public Property Username() As String
    Public Property BusinessUnit() As String
    Public Property Partner() As String
    Public Property CurrentDate() As DateTime
    Public Property BasketItems() As Generic.Dictionary(Of String, WebPriceProduct)
    Public Property PromotionCode() As String
    Public Property LanguageCode() As String
    Public Property DiscountValue() As Decimal

    ''' <summary>
    ''' The promotion activation mechanism that should take priority
    ''' AUTO = automatic promotions
    ''' CODE = code/voucher based promotions
    ''' </summary>
    ''' <value>AUTO or CODE</value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExecutionPrecedence() As String
    Public Property BasketHeaderID() As Long
    Public Property InvalidCodeError() As String
    Public Property TempOrderID() As String
    Public Property PriceList() As String
    Public Property SecondaryPriceList() As String
    Public Property PartnerGroup() As String
    Public Property UsersAttributeList() As String
    Public Property PromotionId() As String
    Public Property CustomerNumber() As String
    Public Property Agent() As String
    Public Property Company() As String
    Public Property Source() As String
#End Region

#Region "Constructors"

    Sub New()
        MyBase.New()
    End Sub

    Sub New(ByVal __basketHeaderID As Long, _
            ByVal __basketItems As Generic.Dictionary(Of String, WebPriceProduct), _
            ByVal __businessUnit As String, _
            ByVal __username As String, _
            ByVal __currentDate As DateTime, _
            ByVal __promotionPriorityType As String, _
            ByVal __languageCode As String, _
            ByVal __partner As String, _
            ByVal __basketTotal As Decimal, _
            ByVal __promotionCode As String, _
            ByVal __discountValue As Decimal, _
            ByVal __invalidCodeError As String, _
            ByVal __tempOrderID As String, _
            ByVal __priceList As String, _
            ByVal __partnerGroup As String, _
            ByVal __secondaryPriceList As String, _
            ByVal __usersAttributeList As String)

        MyBase.New()

        Me.BasketHeaderID = __basketHeaderID
        Me.BasketItems = __basketItems
        Me.BusinessUnit = __businessUnit
        Me.CurrentDate = __currentDate
        Me.DiscountValue = __discountValue
        Me.ExecutionPrecedence = __promotionPriorityType
        Me.LanguageCode = __languageCode
        Me.Partner = __partner
        Me.BasketTotal = __basketTotal
        Me.PromotionCode = __promotionCode
        Me.Username = __username
        Me.InvalidCodeError = __invalidCodeError
        Me.TempOrderID = __tempOrderID
        Me.PriceList = __priceList
        Me.PartnerGroup = __partnerGroup
        Me.SecondaryPriceList = __secondaryPriceList
        Me.UsersAttributeList = __usersAttributeList

    End Sub

#End Region

End Class
