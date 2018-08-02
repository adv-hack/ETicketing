'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is for importing product group hierarchys into 
'                                   tbl_group_product
'
'       Date                        04/10/08
'
'       Author                      Ben Ford
'
'       Copyright CS Group 2008     All rights reserved.
'
'--------------------------------------------------------------------------------------------------
'
<Serializable()> _
Public Class DEProductGroupHierarchies

    Private _productGroupHierarchies As Collection
    Public Property ProductGroupHierarchies() As Collection
        Get
            Return _productGroupHierarchies
        End Get
        Set(ByVal value As Collection)
            _productGroupHierarchies = value
        End Set
    End Property

End Class

<Serializable()> _
Public Class DEProductGroupHierarchy

    Private _businessUnit As String
    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property

    Private _partner As String
    Public Property Partner() As String
        Get
            Return _partner
        End Get
        Set(ByVal value As String)
            _partner = value
        End Set
    End Property

    Private _mode As String
    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Private _lastProductAfterLevel As String
    Public Property LastProductAfterLevel() As String
        Get
            Return _lastProductAfterLevel
        End Get
        Set(ByVal value As String)
            _lastProductAfterLevel = value
        End Set
    End Property

    Private _level1GroupsTotal As Integer
    Public Property Level1GroupsTotal() As Integer
        Get
            Return _level1GroupsTotal
        End Get
        Set(ByVal value As Integer)
            _level1GroupsTotal = value
        End Set
    End Property

    Private _level1Groups As Collection
    Public Property Level1Groups() As Collection
        Get
            Return _level1Groups
        End Get
        Set(ByVal value As Collection)
            _level1Groups = value
        End Set
    End Property

    Private _err As ErrorObj
    Public Property Err() As ErrorObj
        Get
            Return _err
        End Get
        Set(ByVal value As ErrorObj)
            _err = value
        End Set
    End Property


End Class

<Serializable()> _
Public Class DEProductGroupHierarchyGroup

    Private _mode As String = String.Empty
    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Private _businessUnit As String
    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property

    Private _partner As String
    Public Property Partner() As String
        Get
            Return _partner
        End Get
        Set(ByVal value As String)
            _partner = value
        End Set
    End Property

    Private _code As String = String.Empty
    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            _code = value
        End Set
    End Property

    Private _l01Group As String = String.Empty
    Public Property L01Group() As String
        Get
            Return _l01Group
        End Get
        Set(ByVal value As String)
            _l01Group = value
        End Set
    End Property
    Private _l02Group As String = String.Empty
    Public Property L02Group() As String
        Get
            Return _l02Group
        End Get
        Set(ByVal value As String)
            _l02Group = value
        End Set
    End Property
    Private _l03Group As String = String.Empty
    Public Property L03Group() As String
        Get
            Return _l03Group
        End Get
        Set(ByVal value As String)
            _l03Group = value
        End Set
    End Property
    Private _l04Group As String = String.Empty
    Public Property L04Group() As String
        Get
            Return _l04Group
        End Get
        Set(ByVal value As String)
            _l04Group = value
        End Set
    End Property
    Private _l05Group As String = String.Empty
    Public Property L05Group() As String
        Get
            Return _l05Group
        End Get
        Set(ByVal value As String)
            _l05Group = value
        End Set
    End Property
    Private _l06Group As String = String.Empty
    Public Property L06Group() As String
        Get
            Return _l06Group
        End Get
        Set(ByVal value As String)
            _l06Group = value
        End Set
    End Property
    Private _l07Group As String = String.Empty
    Public Property L07Group() As String
        Get
            Return _l07Group
        End Get
        Set(ByVal value As String)
            _l07Group = value
        End Set
    End Property
    Private _l08Group As String = String.Empty
    Public Property L08Group() As String
        Get
            Return _l08Group
        End Get
        Set(ByVal value As String)
            _l08Group = value
        End Set
    End Property
    Private _l09Group As String = String.Empty
    Public Property L09Group() As String
        Get
            Return _l09Group
        End Get
        Set(ByVal value As String)
            _l09Group = value
        End Set
    End Property
    Private _l10Group As String = String.Empty
    Public Property L10Group() As String
        Get
            Return _l10Group
        End Get
        Set(ByVal value As String)
            _l10Group = value
        End Set
    End Property

    Private _displaySequence As String = String.Empty
    Public Property DisplaySequence() As String
        Get
            Return _displaySequence
        End Get
        Set(ByVal value As String)
            _displaySequence = value
        End Set
    End Property

    Private _advancedSearchTemplate As String = String.Empty
    Public Property AdvancedSearchTemplate() As String
        Get
            Return _advancedSearchTemplate
        End Get
        Set(ByVal value As String)
            _advancedSearchTemplate = value
        End Set
    End Property

    Private _productPageTemplate As String = String.Empty
    Public Property ProductPageTemplate() As String
        Get
            Return _productPageTemplate
        End Get
        Set(ByVal value As String)
            _productPageTemplate = value
        End Set
    End Property

    Private _productListTemplate As String = String.Empty
    Public Property ProductListTemplate() As String
        Get
            Return _productListTemplate
        End Get
        Set(ByVal value As String)
            _productListTemplate = value
        End Set
    End Property

    Private _showChildrenAsGroups As String = String.Empty
    Public Property ShowChildrenAsGroups() As String
        Get
            Return _showChildrenAsGroups
        End Get
        Set(ByVal value As String)
            _showChildrenAsGroups = value
        End Set
    End Property

    Private _showProductsAsLists As String = String.Empty
    Public Property ShowProductAsList() As String
        Get
            Return _showProductsAsLists
        End Get
        Set(ByVal value As String)
            _showProductsAsLists = value
        End Set
    End Property

    Private _showInNavigation As String = String.Empty
    Public Property ShowInNavigation() As String
        Get
            Return _showInNavigation
        End Get
        Set(ByVal value As String)
            _showInNavigation = value
        End Set
    End Property

    Private _showInGroupedNavigation As String = String.Empty
    Public Property ShowInGroupedNavigation() As String
        Get
            Return _showInGroupedNavigation
        End Get
        Set(ByVal value As String)
            _showInGroupedNavigation = value
        End Set
    End Property

    Private _HtmlGroup As String = String.Empty
    Public Property HtmlGroup() As String
        Get
            Return _HtmlGroup
        End Get
        Set(ByVal value As String)
            _HtmlGroup = value
        End Set
    End Property

    Private _HtmlGroupType As String = String.Empty
    Public Property HtmlGroupType() As String
        Get
            Return _HtmlGroupType
        End Get
        Set(ByVal value As String)
            _HtmlGroupType = value
        End Set
    End Property

    Private _showProductDisplay As String = String.Empty
    Public Property ShowProductDisplay() As String
        Get
            Return _showProductDisplay
        End Get
        Set(ByVal value As String)
            _showProductDisplay = value
        End Set
    End Property

    Private _theme As String = String.Empty
    Public Property Theme() As String
        Get
            Return _theme
        End Get
        Set(ByVal value As String)
            _theme = value
        End Set
    End Property

    Private _productsTotal As Integer = 0
    Public Property ProductsTotal() As Integer
        Get
            Return _productsTotal
        End Get
        Set(ByVal value As Integer)
            _productsTotal = value
        End Set
    End Property

    Private _products As Collection
    Public Property Products() As Collection
        Get
            Return _products
        End Get
        Set(ByVal value As Collection)
            _products = value
        End Set
    End Property

    Private _nextLevelGroupsTotal As Integer = 0
    Public Property NextLevelGroupsTotal() As Integer
        Get
            Return _nextLevelGroupsTotal
        End Get
        Set(ByVal value As Integer)
            _nextLevelGroupsTotal = value
        End Set
    End Property

    Private _nextLevelGroups As Collection
    Public Property NextLevelGroups() As Collection
        Get
            Return _nextLevelGroups
        End Get
        Set(ByVal value As Collection)
            _nextLevelGroups = value
        End Set
    End Property
    Private _err As ErrorObj
    Public Property Err() As ErrorObj
        Get
            Return _err
        End Get
        Set(ByVal value As ErrorObj)
            _err = value
        End Set
    End Property

    Public Property ProductGroupDetails() As New DEProductGroupDetails

End Class



