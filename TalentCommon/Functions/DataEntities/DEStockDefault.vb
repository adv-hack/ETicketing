<Serializable()> _
Public Class DEStockDefault
    Private _warehouse As String
    Private _businessUnit As String
    Private _partner As String
    Private _mode As String

    Public Property Warehouse() As String
        Get
            Return _warehouse
        End Get
        Set(ByVal value As String)
            _warehouse = value
        End Set
    End Property

    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property

    Public Property Partner() As String
        Get
            Return _partner
        End Get
        Set(ByVal value As String)
            _partner = value
        End Set
    End Property

    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property


End Class
