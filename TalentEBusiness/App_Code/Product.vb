Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports Talent.Common

'******************************************************************
'   Product.vb
'  
'   Modification Summary
'
'   dd/mm/yy    ID      By      Description
'   --------    -----   ---     -----------

'******************************************************************
Namespace Talent.eCommerce
    Public Class Product
        Private _code As String
        Private _description1 As String
        Private _description2 As String
        Private _navigateURL As String
        Private _country As String
        Private _colour As String
        Private _sequenceNo As String
        Private _imagePath As String
        Private _group1 As String
        Private _group2 As String
        Private _group3 As String
        Private _group4 As String
        Private _group5 As String
        Private _group6 As String
        Private _group7 As String
        Private _group8 As String
        Private _group9 As String
        Private _group10 As String
        Private _altText As String
        Private _availableOnline As Boolean
        Private _linkEnabled As Boolean = False


        Private _html1 As String
        Public Property HTML_1() As String
            Get
                Return _html1
            End Get
            Set(ByVal value As String)
                _html1 = value
            End Set
        End Property

        Private _html2 As String
        Public Property HTML_2() As String
            Get
                Return _html2
            End Get
            Set(ByVal value As String)
                _html2 = value
            End Set
        End Property


        Private __html3 As String
        Public Property HTML_3() As String
            Get
                Return __html3
            End Get
            Set(ByVal value As String)
                __html3 = value
            End Set
        End Property


        Private _brand As String
        Public Property Brand() As String
            Get
                Return _brand
            End Get
            Set(ByVal value As String)
                _brand = value
            End Set
        End Property

        Private _packSize As String
        Public Property PackSize() As String
            Get
                Return _packSize
            End Get
            Set(ByVal value As String)
                _packSize = value
            End Set
        End Property

        Public Property Code() As String
            Get
                Return _code
            End Get
            Set(ByVal value As String)
                _code = value
            End Set
        End Property

        Public Property Description1() As String
            Get
                Return _description1
            End Get
            Set(ByVal value As String)
                _description1 = value
            End Set
        End Property

        Public Property Description2() As String
            Get
                Return _description2
            End Get
            Set(ByVal value As String)
                _description2 = value
            End Set
        End Property

        Public Property NavigateURL() As String
            Get
                Return _navigateURL
            End Get
            Set(ByVal value As String)
                _navigateURL = value
            End Set
        End Property
        Public Property Country() As String
            Get
                Return _country
            End Get
            Set(ByVal value As String)
                _country = value
            End Set
        End Property
        Public Property Colour() As String
            Get
                Return _colour
            End Get
            Set(ByVal value As String)
                _colour = value
            End Set
        End Property
        Public Property SequenceNo() As String
            Get
                Return _sequenceNo
            End Get
            Set(ByVal value As String)
                _sequenceNo = value
            End Set
        End Property
        Public Property ImagePath() As String
            Get
                Return _imagePath
            End Get
            Set(ByVal value As String)
                _imagePath = value
            End Set
        End Property
        Public Property Group1() As String
            Get
                Return _group1
            End Get
            Set(ByVal value As String)
                _group1 = value
            End Set
        End Property
        Public Property Group2() As String
            Get
                Return _group2
            End Get
            Set(ByVal value As String)
                _group2 = value
            End Set
        End Property
        Public Property Group3() As String
            Get
                Return _group3
            End Get
            Set(ByVal value As String)
                _group3 = value
            End Set
        End Property
        Public Property Group4() As String
            Get
                Return _group4
            End Get
            Set(ByVal value As String)
                _group4 = value
            End Set
        End Property
        Public Property Group5() As String
            Get
                Return _group5
            End Get
            Set(ByVal value As String)
                _group5 = value
            End Set
        End Property
        Public Property Group6() As String
            Get
                Return _group6
            End Get
            Set(ByVal value As String)
                _group6 = value
            End Set
        End Property
        Public Property Group7() As String
            Get
                Return _group7
            End Get
            Set(ByVal value As String)
                _group7 = value
            End Set
        End Property
        Public Property Group8() As String
            Get
                Return _group8
            End Get
            Set(ByVal value As String)
                _group8 = value
            End Set
        End Property
        Public Property Group9() As String
            Get
                Return _group9
            End Get
            Set(ByVal value As String)
                _group9 = value
            End Set
        End Property
        Public Property Group10() As String
            Get
                Return _group10
            End Get
            Set(ByVal value As String)
                _group10 = value
            End Set
        End Property
        Public Property AltText() As String
            Get
                Return _altText
            End Get
            Set(ByVal value As String)
                _altText = value
            End Set
        End Property


        Private _isSalePrice As Boolean
        Public Property IsSalePrice() As Boolean
            Get
                Return _isSalePrice
            End Get
            Set(ByVal value As Boolean)
                _isSalePrice = value
            End Set
        End Property


        Private _sortPrice As Decimal
        Public Property PriceForSorting() As Decimal
            Get
                Return _sortPrice
            End Get
            Set(ByVal value As Decimal)
                _sortPrice = value
            End Set
        End Property



        Private _webPrices As Talent.Common.DEWebPrice
        Public Property WebPrices() As Talent.Common.DEWebPrice
            Get
                Return _webPrices
            End Get
            Set(ByVal value As Talent.Common.DEWebPrice)
                _webPrices = value
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
        Public Property LinkEnabled() As Boolean
            Get
                Return _linkEnabled
            End Get
            Set(ByVal value As Boolean)
                _linkEnabled = value
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

        Private _id As Decimal
        Public Property ID() As Decimal
            Get
                Return _id
            End Get
            Set(ByVal value As Decimal)
                _id = value
            End Set
        End Property

        Private _masterProduct As String
        Public Property MasterProduct() As String
            Get
                Return _masterProduct
            End Get
            Set(ByVal value As String)
                _masterProduct = value
            End Set
        End Property
    End Class
End Namespace

