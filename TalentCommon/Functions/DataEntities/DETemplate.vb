<Serializable()>
Public Class DETemplate
    ''' <summary>
    ''' Template override id
    ''' </summary>
    Public Property TemplateOverrideId As Decimal

    ''' <summary>
    ''' Template description
    ''' </summary>
    Public Property Description As String

    ''' <summary>
    ''' Businessunit
    ''' </summary>
    Public Property BusinessUnit As String

    ''' <summary>
    ''' Sale confirmation email id
    ''' </summary>
    Public Property SaleConfirmationEmailId As Decimal

    ''' <summary>
    ''' Sale confirmation email description
    ''' </summary>
    Public Property SaleConfirmationEmailDescription As String

    ''' <summary>
    ''' Q&A template id
    ''' </summary>
    Public Property QAndATemplateID As Decimal

    ''' <summary>
    ''' Q&A template description
    ''' </summary>
    ''' <returns></returns>
    Public Property QAndATemplateDescription As String

    ''' <summary>
    ''' Data capture template id
    ''' </summary>
    Public Property DataCaptureTemplateId As Decimal

    ''' <summary>
    ''' Data capture template description
    ''' </summary>
    Public Property DataCaptureTemplateDescription As String

    ''' <summary>
    ''' Auto expand Q&A
    ''' </summary>
    Public Property AutoExpandQAndA As Int32

    ''' <summary>
    ''' Source
    ''' </summary>
    Public Property Source As String

    ''' <summary>
    ''' Mode
    ''' </summary>
    Public Property Mode As String

    ''' <summary>
    ''' Boxoffice user name
    ''' </summary>
    Public Property BoxOfficeUser As String

    ''' <summary>
    ''' Template override criterias
    ''' </summary>
    Public Property TemplateOverrideCriterias As New List(Of TemplateOverrideCriteria)

End Class

Public Class TemplateOverrideCriteria

    ''' <summary>
    ''' Template use id
    ''' </summary>
    Public Property TemplateUseId As Decimal

    ''' <summary>
    ''' Criteria type
    ''' </summary>
    Public Property CriteriaType As String

    ''' <summary>
    ''' Criteria value
    ''' </summary>
    Public Property CriteriaValue As String

    ''' <summary>
    ''' Criteria type description
    ''' </summary>
    Public Property CriteriaTypeDescription As String

End Class


