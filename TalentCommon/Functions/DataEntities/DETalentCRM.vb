<Serializable()>
Public Class DETalentCRM

    Private _customerNumber As String = String.Empty
    Private _userName As String = String.Empty
    Private _attributeCategoryCode As String = String.Empty
    Private _attributeCode As String = String.Empty
    Private _attributeID As String = String.Empty
    Private _attributeOperation As String = String.Empty
    Private _productSubtype As String = String.Empty

    Public Property CustomerNumber() As String
        Get
            Return _userName
        End Get
        Set(ByVal value As String)
            _userName = value
        End Set
    End Property

    Public Property UserName() As String
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As String)
            _customerNumber = value
        End Set
    End Property

    Public Property AttributeCategoryCode() As String
        Get
            Return _attributeCategoryCode
        End Get
        Set(ByVal value As String)
            _attributeCategoryCode = value
        End Set
    End Property

    Public Property AttributeCode() As String
        Get
            Return _attributeCode
        End Get
        Set(ByVal value As String)
            _attributeCode = value
        End Set
    End Property

    Public Property AttributeID() As String
        Get
            Return _attributeID
        End Get
        Set(ByVal value As String)
            _attributeID = value
        End Set
    End Property

    Public Property AttributeOperation() As String
        Get
            Return _attributeOperation
        End Get
        Set(ByVal value As String)
            _attributeOperation = value
        End Set
    End Property

    Public Property ProductSubtype() As String
        Get
            Return _productSubtype
        End Get
        Set(ByVal value As String)
            _productSubtype = value
        End Set
    End Property

    Public Property emailTemplateDefinition As New List(Of DEEmailTemplateDefinition)

End Class