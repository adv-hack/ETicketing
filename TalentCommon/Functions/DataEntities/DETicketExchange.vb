<Serializable()>
Public Class DETicketExchange
    Public Property Customer() As String
    Public Property ResoldPayref As String
    Public Property ListingCustomer() As String
    Public Property ProductCode() As String
    Public Property ProductDescription() As String
    Public Property FromDate() As String = String.Empty
    Public Property TicketExchangeReference() As String
    Public Property OriginatingSourceCode() As String
    Public Property TicketExchangeItems() As New List(Of DETicketExchangeItem)
    Public Property Comment1() As String
    Public Property Comment2() As String
    Public Property ReturnAllProducts() As Boolean

    Public Property AllowTicketExchangeReturn As Boolean
    Public Property AllowTicketExchangePurchase As Boolean
    Public Property ClubFeePercentageOrFixed As String
    Public Property MinMaxBoundaryPercentageOrFixed As String
    Public Property CustomerRetainsPrerequisite As Boolean
    Public Property CustomerRetainsMaxLimit As Boolean
    Public Property MinimumResalePrice As Decimal
    Public Property MaximumResalePrice As Decimal
    Public Property ClubFee As Decimal
    Public Property ProdMaximumTEPerCustomer As Decimal
    Public Property NumberOfStandAreas As Decimal
    Public Property StandAreaDefaults As New List(Of DETicketExchangeStandAreaDefaults)
End Class

<Serializable()>
Public Class DETicketExchangeItem
    Public Property SeatedCustomerNo() As String = String.Empty
    Public Property ProductCode() As String = String.Empty
    Public Property SeatDetails() As New DESeatDetails
    Public Property Status() As Decimal
    Public Property Comments() As String()
    Public Property PaymentOwnerCustomerNo() As String = String.Empty
    Public Property FaceValuePrice() As Decimal
    Public Property RequestedPrice() As Decimal
    Public Property OriginalPrice() As Decimal
    Public Property PotentialEarning() As Decimal
    Public Property Fee() As Decimal
    Public Property FeeType() As String
End Class

<Serializable()>
Public Class DETicketExchangeStandAreaDefaults
    Public Property StandAreaAreaCode As String
    Public Property StandAreaStandCode As String
    Public Property StandAreaAllowTicketExchangeReturnFlag As Boolean
    Public Property StandAreaAllowTicketExchangePurchaseFlag As Boolean
End Class
