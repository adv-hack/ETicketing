Imports System.Collections.Generic

<Serializable()> _
Public Class DEStock
    Private _warehouses As ICollection(Of DEWarehouses) = New List(Of DEWarehouses)
    Private _products As ICollection(Of DEStockProducts) = New List(Of DEStockProducts)
    Private _defaults As ICollection(Of DEStockDefaults) = New List(Of DEStockDefaults)
    Private _mode As String

    Public Property deWarehouses() As ICollection(Of DEWarehouses)
        Get
            Return _warehouses
        End Get
        Set(ByVal value As ICollection(Of DEWarehouses))
            _warehouses = value
        End Set
    End Property

    Public Property Products() As ICollection(Of DEStockProducts)
        Get
            Return _products
        End Get
        Set(ByVal value As ICollection(Of DEStockProducts))
            _products = value
        End Set
    End Property

    Public Property Defaults() As ICollection(Of DEStockDefaults)
        Get
            Return _defaults
        End Get
        Set(ByVal value As ICollection(Of DEStockDefaults))
            _defaults = value
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
