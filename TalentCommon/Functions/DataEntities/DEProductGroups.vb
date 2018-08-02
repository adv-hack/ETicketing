'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is for importing product groups into 
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
Public Class DEProductGroups

    Private _ColProductGroup As Collection
    Public Property ColProductGroup() As Collection
        Get
            Return _ColProductGroup
        End Get
        Set(ByVal value As Collection)
            _ColProductGroup = value
        End Set
    End Property

    Private _mode As String = String.Empty
    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Private _total As Integer
    Public Property Total() As Integer
        Get
            Return _total
        End Get
        Set(ByVal value As Integer)
            _total = value
        End Set
    End Property


End Class


<Serializable()> _
Public Class DEProductGroup
    Private _code As String = String.Empty
    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            _code = value
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

    Private _details As Collection
    Public Property Details() As Collection
        Get
            Return _details
        End Get
        Set(ByVal value As Collection)
            _details = value
        End Set
    End Property


End Class


<Serializable()> _
Public Class DEProductGroupDetails
    Private _language As String = String.Empty
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


    Public Sub New()


    End Sub
End Class