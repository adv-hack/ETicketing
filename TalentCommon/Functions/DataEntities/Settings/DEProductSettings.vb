<Serializable()> _
Public Class DEProductSettings
    Inherits DESettings


    Private _includeProductPurchasers As String
    Public Property IncludeProductPurchasers() As String
        Get
            Return _includeProductPurchasers
        End Get
        Set(ByVal value As String)
            _includeProductPurchasers = value
        End Set
    End Property

End Class
