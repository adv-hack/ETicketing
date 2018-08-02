'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with PNA Requests and Responses
'
'       Date                        7th Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEPN- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DePNA
    Private _collDETrans As New Collection      ' Transaction details
    '
    Private _manufacturerPartNumber As String = String.Empty
    Private _price As String = String.Empty
    Private _priceAndAvailabilitySku As String = String.Empty
    Private _quantity As Double = 0
    Private _sKU As String = String.Empty
    Private _AlternateSKU As String = String.Empty
    Private _reservedInventory As String = String.Empty
    Private _showDetail As String = String.Empty
    Private _warehouse As String = String.Empty

    Private _webProductCode As String
    Public Property WebProductCode() As String
        Get
            Return _webProductCode
        End Get
        Set(ByVal value As String)
            _webProductCode = value
        End Set
    End Property

    Private _accountNo1 As String
    Public Property AccountNumber1() As String
        Get
            Return _accountNo1
        End Get
        Set(ByVal value As String)
            _accountNo1 = value
        End Set
    End Property
    Private _accountNo2 As String
    Public Property AccountNumber2() As String
        Get
            Return _accountNo2
        End Get
        Set(ByVal value As String)
            _accountNo2 = value
        End Set
    End Property



    Public Property CollDETrans() As Collection
        Get
            Return _collDETrans
        End Get
        Set(ByVal value As Collection)
            _collDETrans = value
        End Set
    End Property

    Public Property ManufacturerPartNumber() As String
        Get
            Return _manufacturerPartNumber
        End Get
        Set(ByVal value As String)
            _manufacturerPartNumber = value
        End Set
    End Property
    Public Property Price() As String
        Get
            Return _price
        End Get
        Set(ByVal value As String)
            _price = value
        End Set
    End Property '
    Public Property PriceAndAvailabilitySku() As String
        Get
            Return _priceAndAvailabilitySku
        End Get
        Set(ByVal value As String)
            _priceAndAvailabilitySku = value
        End Set
    End Property
    Public Property Quantity() As Double
        Get
            Return _quantity
        End Get
        Set(ByVal value As Double)
            _quantity = value
        End Set
    End Property
    Public Property SKU() As String
        Get
            Return _sKU
        End Get
        Set(ByVal value As String)
            _sKU = value
        End Set
    End Property
    Public Property AlternateSKU() As String
        Get
            Return _AlternateSKU
        End Get
        Set(ByVal value As String)
            _AlternateSKU = value
        End Set
    End Property
    Public Property ReservedInventory() As String
        Get
            Return _reservedInventory
        End Get
        Set(ByVal value As String)
            _reservedInventory = value
        End Set
    End Property
    Public Property ShowDetail() As String
        Get
            Return _showDetail
        End Get
        Set(ByVal value As String)
            _showDetail = value
        End Set
    End Property
    Public Property Warehouse() As String
        Get
            Return _warehouse
        End Get
        Set(ByVal value As String)
            _warehouse = value
        End Set
    End Property

End Class
