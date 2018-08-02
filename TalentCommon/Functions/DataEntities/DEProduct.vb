Imports Microsoft.VisualBasic

<Serializable()> _
Public Class DEProduct

    Private _collDETrans As New Collection
    Private _categoryMode As String
    Private _collDECategoryDefinitions As New Collection
    Private _attributeMode As String
    Private _collDEAttributeDefinitions As New Collection
    Private _productMode As String
    Private _collDEProducts As New Collection
    Private _collDEProductQuestionAnswers As New List(Of DEProductQuestionsAnswers)
    Private _productQuestionAnswers As DEProductQuestionsAnswers

    Public Property CollDETrans() As Collection
        Get
            Return _collDETrans
        End Get
        Set(ByVal value As Collection)
            _collDETrans = value
        End Set
    End Property

    Public Property CategoryMode() As String
        Get
            Return _categoryMode
        End Get
        Set(ByVal value As String)
            _categoryMode = value
        End Set
    End Property

    Public Property CollDECategoryDefinitions() As Collection
        Get
            Return _collDECategoryDefinitions
        End Get
        Set(ByVal value As Collection)
            _collDECategoryDefinitions = value
        End Set
    End Property

    Public Property AttributeMode() As String
        Get
            Return _attributeMode
        End Get
        Set(ByVal value As String)
            _attributeMode = value
        End Set
    End Property

    Public Property CollDEAttributeDefinitions() As Collection
        Get
            Return _collDEAttributeDefinitions
        End Get
        Set(ByVal value As Collection)
            _collDEAttributeDefinitions = value
        End Set
    End Property

    Public Property ProductMode() As String
        Get
            Return _productMode
        End Get
        Set(ByVal value As String)
            _productMode = value
        End Set
    End Property

    Public Property CollDEProducts() As Collection
        Get
            Return _collDEProducts
        End Get
        Set(ByVal value As Collection)
            _collDEProducts = value
        End Set
    End Property
    Private _UpdateDescriptions As Boolean
    Public Property UpdateDescriptions() As Boolean
        Get
            Return _UpdateDescriptions
        End Get
        Set(ByVal value As Boolean)
            _UpdateDescriptions = value
        End Set
    End Property
    Public Property CollDEProductQuestionAnswers() As List(Of DEProductQuestionsAnswers)
        Get
            Return _collDEProductQuestionAnswers
        End Get
        Set(ByVal value As List(Of DEProductQuestionsAnswers))
            _collDEProductQuestionAnswers = value
        End Set
    End Property
    Public Property ProductQuestionAnswers() As DEProductQuestionsAnswers
        Get
            Return _productQuestionAnswers
        End Get
        Set(ByVal value As DEProductQuestionsAnswers)
            _productQuestionAnswers = value
        End Set
    End Property
End Class
