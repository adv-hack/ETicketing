'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is for importing products
'
'       Date                        13/10/08
'
'       Author                      Stuart Atkinson
'
'       Copyright CS Group 2008     All rights reserved.
'
'--------------------------------------------------------------------------------------------------
'
<Serializable()> _
Public Class DEProductDescriptions

    Private _language As String
    Public Property Language() As String
        Get
            Return _language
        End Get
        Set(ByVal value As String)
            _language = value
        End Set
    End Property

    Private _description1 As String = String.Empty
    Public Property Description1() As String
        Get
            Return _description1
        End Get
        Set(ByVal value As String)
            _description1 = value
        End Set
    End Property

    Private _description2 As String = String.Empty
    Public Property Description2() As String
        Get
            Return _description2
        End Get
        Set(ByVal value As String)
            _description2 = value
        End Set
    End Property

    Private _description3 As String = String.Empty
    Public Property Description3() As String
        Get
            Return _description3
        End Get
        Set(ByVal value As String)
            _description3 = value
        End Set
    End Property

    Private _description4 As String = String.Empty
    Public Property Description4() As String
        Get
            Return _description4
        End Get
        Set(ByVal value As String)
            _description4 = value
        End Set
    End Property

    Private _description5 As String = String.Empty
    Public Property Description5() As String
        Get
            Return _description5
        End Get
        Set(ByVal value As String)
            _description5 = value
        End Set
    End Property

    Private _html1 As String = String.Empty
    Public Property Html1() As String
        Get
            Return _html1
        End Get
        Set(ByVal value As String)
            _html1 = value
        End Set
    End Property

    Private _html2 As String = String.Empty
    Public Property Html2() As String
        Get
            Return _html2
        End Get
        Set(ByVal value As String)
            _html2 = value
        End Set
    End Property

    Private _html3 As String = String.Empty
    Public Property Html3() As String
        Get
            Return _html3
        End Get
        Set(ByVal value As String)
            _html3 = value
        End Set
    End Property

    Private _html4 As String = String.Empty
    Public Property Html4() As String
        Get
            Return _html4
        End Get
        Set(ByVal value As String)
            _html4 = value
        End Set
    End Property

    Private _html5 As String = String.Empty
    Public Property Html5() As String
        Get
            Return _html5
        End Get
        Set(ByVal value As String)
            _html5 = value
        End Set
    End Property

    Private _html6 As String = String.Empty
    Public Property Html6() As String
        Get
            Return _html6
        End Get
        Set(ByVal value As String)
            _html6 = value
        End Set
    End Property

    Private _html7 As String = String.Empty
    Public Property Html7() As String
        Get
            Return _html7
        End Get
        Set(ByVal value As String)
            _html7 = value
        End Set
    End Property

    Private _html8 As String = String.Empty
    Public Property Html8() As String
        Get
            Return _html8
        End Get
        Set(ByVal value As String)
            _html8 = value
        End Set
    End Property

    Private _html9 As String = String.Empty
    Public Property Html9() As String
        Get
            Return _html9
        End Get
        Set(ByVal value As String)
            _html9 = value
        End Set
    End Property

    Private _pageTitle As String = String.Empty
    Public Property PageTitle() As String
        Get
            Return _pageTitle
        End Get
        Set(ByVal value As String)
            _pageTitle = value
        End Set
    End Property

    Private _metaDescription As String = String.Empty
    Public Property MetaDescription() As String
        Get
            Return _metaDescription
        End Get
        Set(ByVal value As String)
            _metaDescription = value
        End Set
    End Property

    Private _metaKeywords As String = String.Empty
    Public Property MetaKeywords() As String
        Get
            Return _metaKeywords
        End Get
        Set(ByVal value As String)
            _metaKeywords = value
        End Set
    End Property

    Private _searchKeywords As String = String.Empty
    Public Property SearchKeywords() As String
        Get
            Return _searchKeywords
        End Get
        Set(ByVal value As String)
            _searchKeywords = value
        End Set
    End Property


    Public Property Length() As Decimal = 0
    Public Property Width() As Decimal = 0
    Public Property Depth() As Decimal = 0
    Public Property Height() As Decimal = 0
    Public Property Size() As Decimal = 0
    Public Property Weight() As Decimal = 0
    Public Property Volume() As Decimal = 0
    Public Property Vintage() As Integer = 0
    Public Property Vegetarian() As Boolean = False
    Public Property Vegan() As Boolean = False
    Public Property Organic() As Boolean = False
    Public Property BioDynamic() As Boolean = False
    Public Property Lutte() As Boolean = False
    Public Property MinimumAge() As Integer = 0
    Public Property SearchRange1() As Decimal = 0
    Public Property SearchRange2() As Decimal = 0
    Public Property SearchRange3() As Decimal = 0
    Public Property SearchRange4() As Decimal = 0
    Public Property SearchRange5() As Decimal = 0
    Public Property SearchSwitch1() As Boolean = False
    Public Property SearchSwitch2() As Boolean = False
    Public Property SearchSwitch3() As Boolean = False
    Public Property SearchSwitch4() As Boolean = False
    Public Property SearchSwitch5() As Boolean = False
    Public Property SearchSwitch6() As Boolean = False
    Public Property SearchSwitch7() As Boolean = False
    Public Property SearchSwitch8() As Boolean = False
    Public Property SearchSwitch9() As Boolean = False
    Public Property SearchSwitch10() As Boolean = False
    Public Property SearchDate1() As Date
    Public Property SearchDate2() As Date
    Public Property SearchDate3() As Date
    Public Property SearchDate4() As Date
    Public Property SearchDate5() As Date
    Public Property ProductOptionMaster() As Boolean = False
    Public Property AvailableOnline() As Boolean = True
    Public Property Discontinued() As Boolean = False
    Public Property Personalisable() As Boolean = False
    Public Property GLCode1() As String = String.Empty
    Public Property GLCode2() As String = String.Empty
    Public Property GLCode3() As String = String.Empty
    Public Property GLCode4() As String = String.Empty
    Public Property GLCode5() As String = String.Empty
End Class