<Serializable()> _
Public Class DELoyaltyList

    Private _loyaltyList As Generic.List(Of DELoyalty)
    Public Property LoyaltyList As Generic.List(Of DELoyalty)
        Get
            Return _loyaltyList
        End Get
        Set(ByVal value As Generic.List(Of DELoyalty))
            _loyaltyList = value
        End Set
    End Property

    Sub New()
        MyBase.New()
        LoyaltyList = New Generic.List(Of DELoyalty)
    End Sub

End Class