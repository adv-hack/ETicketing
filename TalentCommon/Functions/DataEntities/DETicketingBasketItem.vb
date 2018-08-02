<Serializable()> _
Public Class DETicketingBasketItem

    Public Property SessionId() As String = String.Empty
    Public Property PurchaseMemberNo() As String = String.Empty
    Public Property AllocatedMemberNo() As String = String.Empty
    Public Property ProductCode() As String = String.Empty
    Public Property Quantity() As String = String.Empty
    Public Property PriceCode() As String = String.Empty
    Public Property PriceBand() As String = String.Empty
    Public Property Seat() As String = String.Empty
    Public Property Price() As String = String.Empty
    Public Property KeepFlag() As String = String.Empty
    Public Property Type() As String = String.Empty
    Public Property TransactionDate() As String = String.Empty
    Public Property TransactionTime() As String = String.Empty
    Public Property ProductDesc() As String = String.Empty
    Public Property PriceCodeDesc() As String = String.Empty
    Public Property PriceBandDesc() As String = String.Empty
    Public Property StandDesc() As String = String.Empty
    Public Property AreaDesc() As String = String.Empty
    Public Property ForceCollect() As String = String.Empty
    Public Property FulfilmentMethod() As String = String.Empty
    Public Property MaxTickets() As String = String.Empty
    Public Property TicketLimit() As String = String.Empty
    Public Property ErrorCode() As String = String.Empty
    Public Property ErrorInfo() As String = String.Empty
    Public Property PackageID() As Decimal = 0
    Public Property PriceCodeOverridden() As String = String.Empty
    Public Property TicketText() As String = String.Empty
    Public Property BulkSalesID() As Integer = 0
    Public Property BulkSalesQuantity() As Integer = 0
    Public Property DeliveryCountry() As String = String.Empty
    Public Property AllocatedSeat() As New DESeatDetails


    Public Function LogString() As String
        Dim sb As New System.Text.StringBuilder
        With sb
            .Append(SessionId & ",")
            .Append(ProductCode & ",")
            .Append(PurchaseMemberNo & ",")
            .Append(AllocatedMemberNo & ",")
            .Append(Quantity.ToString & ",")
            .Append(PriceCode & ",")
            .Append(PriceBand & ",")
            .Append(Seat & ",")
            .Append(Price & ",")
            .Append(KeepFlag & ",")
            .Append(Type & ",")
            .Append(TransactionDate & ",")
            .Append(TransactionTime & ",")
            .Append(ProductDesc & ",")
            .Append(PriceCodeDesc & ",")
            .Append(PriceBandDesc & ",")
            .Append(StandDesc & ",")
            .Append(AreaDesc & ",")
            .Append(ForceCollect & ",")
            .Append(FulfilmentMethod & ",")
            .Append(MaxTickets & ",")
            .Append(TicketLimit & ",")
            .Append(PackageID.ToString & ",")
            .Append(PriceCodeOverridden & ",")
            .Append(ErrorCode & ",")
            .Append(ErrorInfo)
        End With
        Return sb.ToString.Trim
    End Function

End Class