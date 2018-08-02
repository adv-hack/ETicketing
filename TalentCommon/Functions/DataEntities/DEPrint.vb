<Serializable()> _
Public Class DEPrint
    Public Property PaymentReferences() As List(Of String)
    Public Property PaymentReference() As String = String.Empty
    ''' <summary>
    ''' Gets or sets the name of the printer (output queue)
    ''' </summary>
    ''' <value>
    ''' The name of the printer.
    ''' </value>
    Public Property PrinterName() As String = String.Empty
    ''' <summary>
    ''' Gets or sets the relative record number (rrn)
    ''' </summary>
    ''' <value>
    ''' The relative record number.
    ''' </value>
    Public Property RelativeRecordNumber() As String = String.Empty
    Public Property PrintAddress() As Boolean = False
    Public Property PrintTransaction() As Boolean = False
    Public Property PaymentOwnerName() As String = String.Empty
    Public Property AddressLine1() As String = String.Empty
    Public Property AddressLine2() As String = String.Empty
    Public Property AddressLine3() As String = String.Empty
    Public Property AddressLine4() As String = String.Empty
    Public Property PostCodePart1() As String = String.Empty
    Public Property PostCodePart2() As String = String.Empty
    Public Property BulkSalesID() As Integer = 0
    Public Property UnPrintedTickets() As Boolean = False
    Public Property PrintAll() As Boolean = False
End Class
