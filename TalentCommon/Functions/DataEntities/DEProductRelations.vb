'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is for importing product relations into 
'                                   tbl_product_relations
'
'       Date                        23/10/08
'
'       Author                      Ben Ford
'
'       Copyright CS Group 2008     All rights reserved.
'
'--------------------------------------------------------------------------------------------------
'
<Serializable()> _
Public Class DEProductRelations

    Private _total As Integer
    Public Property Total() As Integer
        Get
            Return _total
        End Get
        Set(ByVal value As Integer)
            _total = value
        End Set
    End Property

    Private _productRelationCollection As Collection
    Public Property ProductRelationCollection() As Collection
        Get
            Return _productRelationCollection
        End Get
        Set(ByVal value As Collection)
            _productRelationCollection = value
        End Set
    End Property

End Class

<Serializable()> _
Public Class DEProductRelationCollection

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

    Private _totalRelations As Integer
    Public Property TotalRelations() As Integer
        Get
            Return _totalRelations
        End Get
        Set(ByVal value As Integer)
            _totalRelations = value
        End Set
    End Property

    Private _productRelations As Collection
    Public Property ProductRelations() As Collection
        Get
            Return _productRelations
        End Get
        Set(ByVal value As Collection)
            _productRelations = value
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
Public Class DEProductRelation

    Private _mode As String
    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Private _qualifier As String
    Public Property Qualifier() As String
        Get
            Return _qualifier
        End Get
        Set(ByVal value As String)
            _qualifier = value
        End Set
    End Property

    Private _productInfo As DEProductGroupHierarchyGroup
    Public Property ProductInfo() As DEProductGroupHierarchyGroup
        Get
            Return _productInfo
        End Get
        Set(ByVal value As DEProductGroupHierarchyGroup)
            _productInfo = value
        End Set
    End Property

    Private _relatedProductInfo As DEProductGroupHierarchyGroup
    Public Property RelatedProductInfo() As DEProductGroupHierarchyGroup
        Get
            Return _relatedProductInfo
        End Get
        Set(ByVal value As DEProductGroupHierarchyGroup)
            _relatedProductInfo = value
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





