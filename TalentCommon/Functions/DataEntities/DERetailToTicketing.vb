Public Class DERetailToTicketing

    Public Property ListOfDERetailToTicketingProducts() As New List(Of DERetailToTicketingProduct)
    Public Property BasketId As String
    Public Property TempOrderId As String
    Public Property DeliveryPrice As Decimal
    Public Property PromotionPrice As Decimal
    Public Property PaymentTaken As Boolean


End Class

Public Class DERetailToTicketingProduct
    Public Property LineNumber As Integer
    Public Property Quantity As Integer
    Public Property Price As Decimal
End Class
