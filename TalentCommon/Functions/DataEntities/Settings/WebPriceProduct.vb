Public Class WebPriceProduct
    Private _ProductCode As String
    Public Property ProductCode() As String
        Get
            Return _ProductCode
        End Get
        Set(ByVal value As String)
            _ProductCode = value
        End Set
    End Property

    Private _masterProduct As String
    Public Property MasterProductCode() As String
        Get
            Return _masterProduct
        End Get
        Set(ByVal value As String)
            _masterProduct = value
        End Set
    End Property

    Private _quantity As Decimal
    Public Property Quantity() As Decimal
        Get
            Return _quantity
        End Get
        Set(ByVal value As Decimal)
            _quantity = value
        End Set
    End Property

    Sub New(ByVal product_code As String, ByVal qty As Decimal, ByVal master_product_code As String)
        MyBase.New()
        Me.ProductCode = product_code
        Me.MasterProductCode = master_product_code
        Me.Quantity = qty
    End Sub
End Class
