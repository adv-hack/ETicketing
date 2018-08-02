Imports System.Collections.Generic

<Serializable()> _
Public Class DEWarehouses
    Private _mode As String
    Private _total As Integer
    Private _warehouses As ICollection(Of DEWarehouse) = New List(Of DEWarehouse)

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

    Public Property Warehouses() As ICollection(Of DEWarehouse)
        Get
            Return _warehouses
        End Get
        Set(ByVal value As ICollection(Of DEWarehouse))
            _warehouses = value
        End Set
    End Property

End Class
