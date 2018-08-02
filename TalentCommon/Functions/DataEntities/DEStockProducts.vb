Imports System.Collections.Generic

<Serializable()> _
Public Class DEStockProducts
    Private _mode As String
    Private _total As Integer
    Private _deStockProducts As ICollection(Of DEStockProduct) = New List(Of DEStockProduct)

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

    Public Property StockProducts() As ICollection(Of DEStockProduct)
        Get
            Return _deStockProducts
        End Get
        Set(ByVal value As ICollection(Of DEStockProduct))
            _deStockProducts = value
        End Set
    End Property
End Class
