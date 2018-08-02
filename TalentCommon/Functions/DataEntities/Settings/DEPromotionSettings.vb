<Serializable()> _
Public Class DEPromotionSettings
    Inherits DESettings

    Private _returnOnlyValidPromotions As String
    Public Property IncludeProductPurchasers() As String
        Get
            Return _returnOnlyValidPromotions
        End Get
        Set(ByVal value As String)
            _returnOnlyValidPromotions = value
        End Set
    End Property
End Class
