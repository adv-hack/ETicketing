<Serializable()> _
Public Class DEOrderSettings
    Inherits DESettings

    Private _repriceBlankPrice As Boolean
    Public Property RepriceBlankPrice() As Boolean
        Get
            Return _repriceBlankPrice
        End Get
        Set(ByVal value As Boolean)
            _repriceBlankPrice = value
        End Set
    End Property

    Private _orderCheckForAltProducts As Boolean
    Public Property OrderCheckForAltProducts() As Boolean
        Get
            Return _orderCheckForAltProducts
        End Get
        Set(ByVal value As Boolean)
            _orderCheckForAltProducts = value
        End Set
    End Property
End Class
