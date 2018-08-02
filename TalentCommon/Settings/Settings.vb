

<Serializable()> _
Public Class Settings

    Private Shared _priceList As String
    Public Shared Property PriceList() As String
        Get
            Return _priceList
        End Get
        Set(ByVal value As String)
            _priceList = value
        End Set
    End Property


End Class
