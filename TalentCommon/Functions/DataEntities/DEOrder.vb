<Serializable()> _
Public Class DEOrder

    Public Property CollDETrans() As New Collection
    Public Property CollDEOrders() As New Collection
    Public Property PaymentReference() As String = String.Empty
    Public Property FromDate() As String = String.Empty
    Public Property ToDate() As String = String.Empty
    Public Property CustNumberPayRefShouldMatch As Boolean = True
    Public Property LastRecordNumber As Integer = 0
    Public Property TotalRecords As Integer = 0
    Public Property OrderStatus As String = String.Empty
    Public Property CorporateProductsOnly As Boolean = False
    Public Property ProductCodes() As String()
    Public Property CallId() As Int64 = 0
    Public Property ComponentId() As Int64 = 0
    Public Property BulkSalesID As Integer = 0
    Public Property RequestLastPage As Boolean = False
    Public Property RequestFirstPage As Boolean = False
    Public Property RequestPreviousPage As Boolean = False
    Public Property IsLastPage As Boolean = False
    Public Property CurrentPage As Integer = 0
    Public Property LastRelativeRecordNumber As Integer = 0
    Public Property FirstRecordOnPageRelativeRecordNumber As Integer = 0
    Public Property IncludeBuybackSales As Boolean = True
    Public Property IncludeRoyaltyInformation As Boolean = True
    Public Property IncludeReservations As Boolean = True
    Public Property IncludeBooked As Boolean = True
    Public Property ShowRetailItems As Boolean = True
    Public Property ShowTicketingItems As Boolean = True
    Public Property ShowDespatchInformation As Boolean = False
    Public Property PackageKey As String = String.Empty
    Public Property BulkSalesFlag As Boolean = False
    Public Property CustomerNumber As String = String.Empty
    Public Property DistChan As String = String.Empty
    Public Property PoMethod As String = String.Empty
    Public Property SendCustomerEmailAddressToSAP As Boolean = False
    Public Property EnableAlternativeSKU As Boolean = False
    Public Property ProductOrDescription As String = String.Empty
    Public Property SendCustomerPostCodeToSAP As Boolean = False
    Public Property IsCorporateLinkedHomeGame As String = String.Empty
    Public Property ProductCode As String = String.Empty

End Class
