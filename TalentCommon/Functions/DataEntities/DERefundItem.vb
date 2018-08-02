'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Ticketing Shopping Basket Items
'
'       Date                        20th June 2007
'
'       Author                      Andrew Green
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDETI- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DERefundItem
    '
    Private _productCode As String = ""
    Private _productDetails As String = ""
    Private _quantity As String = ""
    Private _customerNo As String = ""
    Private _priceBand As String = ""
    Private _priceCode As String = ""
    Private _cancelAllMatching As String = ""
    '
    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property
    Public Property ProductDetails() As String
        Get
            Return _productDetails
        End Get
        Set(ByVal value As String)
            _productDetails = value
        End Set
    End Property
    Public Property Quantity() As String
        Get
            Return _quantity
        End Get
        Set(ByVal value As String)
            _quantity = value
        End Set
    End Property
    Public Property CustomerNo() As String
        Get
            Return _customerNo
        End Get
        Set(ByVal value As String)
            _customerNo = value
        End Set
    End Property
    Public Property PriceBand() As String
        Get
            Return _priceBand
        End Get
        Set(ByVal value As String)
            _priceBand = value
        End Set
    End Property
    Public Property PriceCode() As String
        Get
            Return _priceCode
        End Get
        Set(ByVal value As String)
            _priceCode = value
        End Set
    End Property
    Public Property CancelAllMatching() As String
        Get
            Return _cancelAllMatching
        End Get
        Set(ByVal value As String)
            _cancelAllMatching = value
        End Set
    End Property

    Public Function LogString() As String

        Dim sb As New System.Text.StringBuilder

        With sb
            .Append(ProductCode & ",")
            .Append(ProductDetails & ",")
            .Append(Quantity & ",")
            .Append(CustomerNo & ",")
            .Append(PriceBand & ",")
            .Append(PriceCode & ",")
            .Append(CancelAllMatching & ",")
        End With

        Return sb.ToString.Trim

    End Function
End Class
