<Serializable()> _
Public Class DEDespatch

    Public Property Type() As String
    Public Property FromDate() As String
    Public Property ToDate() As String
    Public Property SuperType() As String
    Public Property SubType() As String
    Public Property Product() As String
    Public Property SeatDetails() As New DESeatDetails
    Public Property PaymentRef() As Long
    Public Property Customer() As String
    Public Property DeliveryMethod() As String
    Public Property PaymentMethod() As String
    Public Property Country() As String
    Public Property Postcode() As String
    Public Property GiftWrap() As String
    Public Property TicketNumber() As String
    Public Property DespatchNoteID() As String
    Public Property BatchID() As Long
    Public Property TimeStampToken() As String
    Public Property GeneratedDespatchPDFDocument() As String
    Public Property DespatchPDFDocumentPath() As String
    Public Property DespatchPDFLayoutName() As String
    Public Property GeneratedDespatchPDFDocumentPageCount() As String
    Public Property DespatchReservations As Boolean
    Public Property DespatchRetail As Boolean
    Public Property Source() As String
    Public Property SessionID() As String
    Public Property Comments() As String
    Public Property AgentName() As String
    Public Property DataTable As Data.DataTable
    Public Property TrackingReference() As String
    Public Property TransactionType() As String
    Public Property Mode() As String
    Public Property DespatchNoteTableColumnHeaders As String
    Public Property DespatchNoteTableColumnWidths As String
    Public Property DespatchNoteTableWidth As String
    Public Property DespatchNoteTableMaxRowsPerPage As String
    Public Property DespatchNotePageNumber As Integer
    Public Property DespatchNoteTotalNumberOfPages As Integer
    Public Property DespatchNoteTotalNumberOfRows As Integer
    Public Property DespatchNoteTotalNumberOfRowsDone As Integer
    Public Property IncludeGenericTransactions As Integer
    Public Property CommandTimeout As Integer
    Public Property AttributeCategory As String
    Public Property DespatchNoteFirstLineFormat As String
    Public Property DespatchNoteSecondLineFormat As String
    Public Property DespatchNoteCollectText As String
    Public Property DespatchNoteRegPostText As String
    Public Property DespatchNotePostText As String
    Public Property DespatchNoteGiftwrapFeeText As String
    Public Property DespatchNoteCharityFeeText As String
    Public Property DespatchNotePostageNotAvailableText As String
    Public Property DespatchNoteGiftwrapNotAvailableText As String
    Public Property DespatchNoteCharityNotAvailableText As String
    Public Property DespatchNoteGeoZoneNotAvailableText As String
    Public Property DespatchNoteGeographicalZoneTextFormat As String
    Public Property DespatchNoteTableColumnCount As String
    Public Property DespatchNoteTableColumnDetailValues As String
    Public Property DespatchNoteSummaryOrDetail As String
    Public Property DespatchNoteOrientation As String
    Public Property PrintSelectAll As Boolean
    Public Property StrictSearch As Boolean
    Public Property RefreshBatch As String
    Public Property DespatchNoteGeographicalZoneTable As Data.DataTable
End Class

Public Class DEDespatchItem

    Public Property Product() As String
    Public Property SeatDetails() As New DESeatDetails
    Public Property TicketNumber() As String
    Public Property MembershipRFID() As String
    Public Property MembershipMagScan() As String
    Public Property MembershipMetalBadge() As String
    Public Property Status() As String
    Public Property ErrorCode() As String
    Public Property PrintTicket() As String

End Class
