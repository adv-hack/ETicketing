''' <summary>
''' Holds the country related details
''' </summary>
<Serializable()> _
Public Class DECountry

    Public Property Country() As String = String.Empty
    Public Property CountryISOCode() As String = String.Empty
    Public Property ZoneNumber() As String = String.Empty
    Public Property PostAllowed() As Boolean = False
    Public Property PostCodeMandatory() As Boolean = False
    Public Property PostDaysBefore() As Integer = 0
    Public Property RegPostDaysBefore() As Integer = 0
    Public Property ZoneCode() As String = String.Empty

End Class