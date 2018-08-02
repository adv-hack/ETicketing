Imports Microsoft.VisualBasic

Public Interface IProduct
    Property ProductCode() As String
    Property ProductDescription1() As String
    Property ProductDescription2() As String
    Property ProductCountry() As String
    Property ProductColour() As String
    Property ProductSequenceNumber() As String
    Property ProductGroup1() As String
    Property ProductGroup2() As String
    Property ProductGroup3() As String
    Property ProductGroup4() As String
    Property ProductGroup5() As String
    Property ProductGroup6() As String
    Property ProductGroup7() As String
    Property ProductGroup8() As String
    Property ProductGroup9() As String
    Property ProductGroup10() As String
    Property ProductAvailableOnline() As String
    Property ProductImagePath() As String
    Property ProductNavigateURL() As String
    Property ProductLinkEnabled() As Boolean
    Property ProductIsSalePrice() As Boolean
    Property ProductAltText() As String
    Property ProductSortPrice() As Decimal
    Property ProductWebPrice() As Talent.Common.DEWebPrice
    Property ProductHTML1() As String
    Property ProductHTML2() As String
    Property ProductHTML3() As String
    Property ProductBrand() As String
    Property ProductPackSize() As String

    Function generateQueryString(ByVal intMaxNoOfGroupLevels As Integer) As String
End Interface
