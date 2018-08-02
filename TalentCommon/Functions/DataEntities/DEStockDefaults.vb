<Serializable()> _
Public Class DEStockDefaults
    Private _mode As String
    Private _total As Integer
    Private _defaults As ICollection(Of DEStockDefault) = New List(Of DEStockDefault)

    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Public Property Total() As Integer
        Get
            Return _total
        End Get
        Set(ByVal value As Integer)
            _total = value
        End Set
    End Property

    Public Property Defaults() As ICollection(Of DEStockDefault)
        Get
            Return _defaults
        End Get
        Set(ByVal value As ICollection(Of DEStockDefault))
            _defaults = value
        End Set
    End Property

End Class
