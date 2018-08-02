<Serializable()> _
Public Class DETicketingItemDetails

#Region "Public Properties"

    Public Property SessionId() As String = String.Empty
    Public Property CustomerNo() As String = String.Empty
    Public Property ProductCode() As String = String.Empty
    Public Property PaymentReference() As Long = 0
    Public Property SeatDetails1() As New DESeatDetails
    Public Property UnprintedRecords() As String = String.Empty
    Public Property SetAsPrinted() As String = String.Empty
    Public Property EndOfSale() As String = String.Empty
    Public Property ReservationPeriod() As String = String.Empty
    Public Property PriceCode() As String = String.Empty
    Public Property Src() As String = String.Empty
    Public Property StadiumCode() As String = String.Empty
    Public Property Type() As String = String.Empty
    Public Property Comments() As String()
    Public Property ByPassPreReqCheck() As String = String.Empty
    Public Property RetryFailure() As Boolean = False
    Public Property PackageID() As Decimal = 0
    Public Property RequestType() As String = String.Empty
    Public Property LinkedProductId() As String = String.Empty
    Public Property BulkSalesID() As Integer = 0
    Public Property SavePackageMode() As String = String.Empty
    Public Property BusinessUnit() As String = String.Empty

#End Region

#Region "Public Functions"

    Public Function LogString() As String
        Dim sb As New System.Text.StringBuilder
        With sb
            .Append(SessionId & ",")
            .Append(CustomerNo & ",")
            .Append(ProductCode & ",")
            .Append(PaymentReference.ToString & ",")
            If SeatDetails1 IsNot Nothing Then
                .Append(SeatDetails1.LogString & ",")
            End If
            .Append(UnprintedRecords & ",")
            .Append(SetAsPrinted & ",")
            .Append(EndOfSale & ",")
            .Append(ReservationPeriod & ",")
            .Append(PriceCode & ",")
            .Append(Src & ",")
            .Append(StadiumCode & ",")
            .Append(Type & ",")
            .Append(PackageID.ToString & ",")
            If Not Comments Is Nothing Then
                .Append(Comments.ToString)
            End If
        End With
        Return sb.ToString.Trim
    End Function

#End Region

End Class