
<Serializable()> _
Public Class DEStockProduct
    Private _mode As String
    Private _sku As String
    Private _quantity As Integer
    Private _quantityWarehouse As String
    Private _quantityReStockCode As String

    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Public Property SKU() As String
        Get
            Return _sku
        End Get
        Set(ByVal value As String)
            _sku = value
        End Set
    End Property

    Public Property Quantity() As Integer
        Get
            Return _quantity
        End Get
        Set(ByVal value As Integer)
            _quantity = value
        End Set
    End Property

    Public Property QuantityWarehouse() As String
        Get
            Return _quantityWarehouse
        End Get
        Set(ByVal value As String)
            _quantityWarehouse = value
        End Set
    End Property

    Public Property QuantityReStockCode() As String
        Get
            Return _quantityReStockCode
        End Get
        Set(ByVal value As String)
            _quantityReStockCode = value
        End Set
    End Property

    Private _prodCode As String = ""
    Public Property ProductCode() As String
        Get
            Return _prodCode
        End Get
        Set(ByVal value As String)
            _prodCode = value
        End Set
    End Property

    Private _stockLocation As String = ""
    Public Property StockLocation() As String
        Get
            Return _stockLocation
        End Get
        Set(ByVal value As String)
            _stockLocation = value
        End Set
    End Property

    Private _allocatedQuantity As Integer
    Public Property AllocatedQuantity() As Integer
        Get
            Return _quantity
        End Get
        Set(ByVal value As Integer)
            _quantity = value
        End Set
    End Property

    Private _availableQuantity As Integer
    Public Property AvailableQuantity() As Integer
        Get
            Return _availableQuantity
        End Get
        Set(ByVal value As Integer)
            _availableQuantity = value
        End Set
    End Property

End Class
