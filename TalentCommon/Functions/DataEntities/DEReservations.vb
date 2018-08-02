<Serializable()> _
Public Class DEReservations
    Public Property CustomerNumber() As String
    Public Property Source() As String
    Public Property Agent() As String
    Public Property SaleOrReturn() As Boolean
    Public Property NumberOfSeatsReserved() As Integer
    Public Property ProductCode() As String
    Public Property RetrieveCustomerFF() As String
    Public Property Comment() As String
    Public Property SessionID() As String
    Public Property ExpiryDate() As String
    Public Property ExpiryTime() As DateTime
    Public Property UnreserveAll() As Boolean
    Public Property Seats As List(Of DESeatDetails)
    Public Property ByPassPreReqCheck() As Boolean
    Public Property ReturnSeatDetails() As Boolean
    Public Property AddToBasket() As Boolean
    Public Property CallId As Long
End Class