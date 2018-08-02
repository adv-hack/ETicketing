'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Attribute Details
'
'       Date                        13th Oct 2008
'
'       Author                      Stuart Atkinson
'
'       � CS Group 2008               All rights reserved.
'
'       Error Number Code base      - 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEProductEcommerceDetails

    Private _mode As String = ""
    Private _sku As String = ""
    Private _alternateSKU As String = ""
    Private _displaySequence As String = ""
    Private _masterProduct As Boolean = False
    Private _collDEProductDescriptions As New Collection
    Private _collDEProductAttributes As New Collection
    Private _availableOnline As Boolean

    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Public Property Sku() As String
        Get
            Return _sku
        End Get
        Set(ByVal value As String)
            _sku = value
        End Set
    End Property

    Public Property AlternateSku() As String
        Get
            Return _alternateSKU
        End Get
        Set(ByVal value As String)
            _alternateSKU = value
        End Set
    End Property

    Public Property DisplaySequence() As String
        Get
            Return _displaySequence
        End Get
        Set(ByVal value As String)
            _displaySequence = value
        End Set
    End Property

    Public Property MasterProduct() As Boolean
        Get
            Return _masterProduct
        End Get
        Set(ByVal value As Boolean)
            _masterProduct = value
        End Set
    End Property

    Public Property CollDEProductDescriptions() As Collection
        Get
            Return _collDEProductDescriptions
        End Get
        Set(ByVal value As Collection)
            _collDEProductDescriptions = value
        End Set
    End Property

    Public Property CollDEProductAttributes() As Collection
        Get
            Return _collDEProductAttributes
        End Get
        Set(ByVal value As Collection)
            _collDEProductAttributes = value
        End Set
    End Property

    Public Property AvailableOnline() As Boolean
        Get
            Return _availableOnline
        End Get
        Set(ByVal value As Boolean)
            _availableOnline = value
        End Set
    End Property

End Class
