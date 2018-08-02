
<Serializable()> _
Public Class DEAmendTicketingOrder

    '---------------------------------------------------------------------------------
    '
    Public Property CustomerNo As String = String.Empty
    Public Property PaymentReference As String = String.Empty
    Public Property ErrorCode As String = String.Empty
    Public Property Src As String = String.Empty
    Public Property BasketItem As New Generic.List(Of DETicketingBasketItem)
    '
End Class
