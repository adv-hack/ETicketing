Imports Microsoft.VisualBasic

Public Class ProductImpl
    Implements IProduct

    Public _productCode As String
    Public _productDescription1 As String
    Public _productDescription2 As String
    Public _productCountry As String
    Public _productColour As String
    Public _productSequenceNumber As String
    Public _productGroup1 As String
    Public _productGroup2 As String
    Public _productGroup3 As String
    Public _productGroup4 As String
    Public _productGroup5 As String
    Public _productGroup6 As String
    Public _productGroup7 As String
    Public _productGroup8 As String
    Public _productGroup9 As String
    Public _productGroup10 As String
    Public _productAvailableOnline As String
    Public _productImagePath As String
    Public _productNavigateURL As String
    Public _productLinkEnabled As Boolean
    Public _productAltText As String
    Public _productSortPrice As Decimal
    Public _productWebPrice As Talent.Common.DEWebPrice
    Public _productHTML1 As String
    Public _productHTML2 As String
    Public _productHTML3 As String
    Public _productIsSalePrice As Boolean
    Public _productBrand As String
    Public _productPackSize As String

    Property ProductCode() As String Implements IProduct.ProductCode
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property

    Property ProductDescription1() As String Implements IProduct.ProductDescription1
        Get
            Return _productDescription1
        End Get
        Set(ByVal value As String)
            _productDescription1 = value
        End Set
    End Property

    Property ProductDescription2() As String Implements IProduct.ProductDescription2
        Get
            Return _productDescription2
        End Get
        Set(ByVal value As String)
            _productDescription2 = value
        End Set
    End Property

    Property ProductCountry() As String Implements IProduct.ProductCountry
        Get
            Return _productCountry
        End Get
        Set(ByVal value As String)
            _productCountry = value
        End Set
    End Property

    Property ProductColour() As String Implements IProduct.ProductColour
        Get
            Return _productColour
        End Get
        Set(ByVal value As String)
            _productColour = value
        End Set
    End Property

    Property ProductSequenceNumber() As String Implements IProduct.ProductSequenceNumber
        Get
            Return _productSequenceNumber
        End Get
        Set(ByVal value As String)
            _productSequenceNumber = value
        End Set
    End Property

    Property ProductGroup1() As String Implements IProduct.ProductGroup1
        Get
            Return _productGroup1
        End Get
        Set(ByVal value As String)
            _productGroup1 = value
        End Set
    End Property

    Property ProductGroup2() As String Implements IProduct.ProductGroup2
        Get
            Return _productGroup2
        End Get
        Set(ByVal value As String)
            _productGroup2 = value
        End Set
    End Property

    Property ProductGroup3() As String Implements IProduct.ProductGroup3
        Get
            Return _productGroup3
        End Get
        Set(ByVal value As String)
            _productGroup3 = value
        End Set
    End Property

    Property ProductGroup4() As String Implements IProduct.ProductGroup4
        Get
            Return _productGroup4
        End Get
        Set(ByVal value As String)
            _productGroup4 = value
        End Set
    End Property

    Property ProductGroup5() As String Implements IProduct.ProductGroup5
        Get
            Return _productGroup5
        End Get
        Set(ByVal value As String)
            _productGroup5 = value
        End Set
    End Property

    Property ProductGroup6() As String Implements IProduct.ProductGroup6
        Get
            Return _productGroup6
        End Get
        Set(ByVal value As String)
            _productGroup6 = value
        End Set
    End Property

    Property ProductGroup7() As String Implements IProduct.ProductGroup7
        Get
            Return _productGroup7
        End Get
        Set(ByVal value As String)
            _productGroup7 = value
        End Set
    End Property

    Property ProductGroup8() As String Implements IProduct.ProductGroup8
        Get
            Return _productGroup8
        End Get
        Set(ByVal value As String)
            _productGroup8 = value
        End Set
    End Property

    Property ProductGroup9() As String Implements IProduct.ProductGroup9
        Get
            Return _productGroup9
        End Get
        Set(ByVal value As String)
            _productGroup9 = value
        End Set
    End Property

    Property ProductGroup10() As String Implements IProduct.ProductGroup10
        Get
            Return _productGroup10
        End Get
        Set(ByVal value As String)
            _productGroup10 = value
        End Set
    End Property

    Property ProductAvailableOnline() As String Implements IProduct.ProductAvailableOnline
        Get
            Return _productAvailableOnline
        End Get
        Set(ByVal value As String)
            _productAvailableOnline = value
        End Set
    End Property
    Property ProductImagePath() As String Implements IProduct.ProductImagePath
        Get
            Return _productImagePath
        End Get
        Set(ByVal value As String)
            _productImagePath = value
        End Set
    End Property

    Property ProductNavigateURL() As String Implements IProduct.ProductNavigateURL
        Get
            Return _productNavigateURL
        End Get
        Set(ByVal value As String)
            _productNavigateURL = value
        End Set
    End Property

    Property ProductLinkEnabled() As Boolean Implements IProduct.ProductLinkEnabled
        Get
            Return _productLinkEnabled
        End Get
        Set(ByVal value As Boolean)
            _productLinkEnabled = value
        End Set
    End Property

    Public Property ProductAltText() As String Implements IProduct.ProductAltText
        Get
            Return _productAltText
        End Get
        Set(ByVal value As String)
            _productAltText = value
        End Set
    End Property

    Public Property ProductSortPrice() As Decimal Implements IProduct.ProductSortPrice
        Get
            Return _productSortPrice
        End Get
        Set(ByVal value As Decimal)
            _productSortPrice = value
        End Set
    End Property

    Public Property ProductWebPrice() As Talent.Common.DEWebPrice Implements IProduct.ProductWebPrice
        Get
            Return _productWebPrice
        End Get
        Set(ByVal value As Talent.Common.DEWebPrice)
            _productWebPrice = value
        End Set
    End Property

    Property ProductHTML1() As String Implements IProduct.ProductHTML1
        Get
            Return _productHTML1
        End Get
        Set(ByVal value As String)
            _productHTML1 = value
        End Set
    End Property

    Property ProductHTML2() As String Implements IProduct.ProductHTML2
        Get
            Return _productHTML2
        End Get
        Set(ByVal value As String)
            _productHTML2 = value
        End Set
    End Property

    Property ProductHTML3() As String Implements IProduct.ProductHTML3
        Get
            Return _productHTML3
        End Get
        Set(ByVal value As String)
            _productHTML3 = value
        End Set
    End Property

    Property ProductBrand() As String Implements IProduct.ProductBrand
        Get
            Return _productBrand
        End Get
        Set(ByVal value As String)
            _productBrand = value
        End Set
    End Property

    Property ProductPackSize() As String Implements IProduct.ProductPackSize
        Get
            Return _productPackSize
        End Get
        Set(ByVal value As String)
            _productPackSize = value
        End Set
    End Property

    Property ProductIsSalePrice() As Boolean Implements IProduct.ProductIsSalePrice
        Get
            Return _productIsSalePrice
        End Get
        Set(ByVal value As Boolean)
            _productIsSalePrice = value
        End Set
    End Property

    Public Function generateQueryString(ByVal intMaxNoOfGroupLevels As Integer) As String Implements IProduct.generateQueryString
        Dim sbQry As New StringBuilder
        With sbQry
            .Append("~/PagesPublic/ProductBrowse/product.aspx?group1=").Append(ProductGroup1)
            If intMaxNoOfGroupLevels > 1 Then
                .Append("&group2=").Append(ProductGroup2)
            End If
            If intMaxNoOfGroupLevels > 2 Then
                .Append("&group3=").Append(ProductGroup3)
            End If
            If intMaxNoOfGroupLevels > 3 Then
                .Append("&group4=").Append(ProductGroup4)
            End If
            If intMaxNoOfGroupLevels > 4 Then
                .Append("&group5=").Append(ProductGroup5)
            End If
            If intMaxNoOfGroupLevels > 5 Then
                .Append("&group6=").Append(ProductGroup6)
            End If
            If intMaxNoOfGroupLevels > 6 Then
                .Append("&group7=").Append(ProductGroup7)
            End If
            If intMaxNoOfGroupLevels > 7 Then
                .Append("&group8=").Append(ProductGroup8)
            End If
            If intMaxNoOfGroupLevels > 8 Then
                .Append("&group9=").Append(ProductGroup9)
            End If
            If intMaxNoOfGroupLevels > 9 Then
                .Append("&group10=").Append(ProductGroup10)
            End If
            .Append("&product=").Append(ProductCode)
        End With
        Return sbQry.ToString
    End Function
End Class
