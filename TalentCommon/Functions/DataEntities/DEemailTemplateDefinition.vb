''' <summary>
''' this data entity class hold data for tbl_email_templates
''' Related to email templates
''' </summary>
<Serializable()> _
Public Class DEEmailTemplateDefinition

    Public Property BusinessUnit() As String
    Public Property Partner() As String
    Public Property EmailTemplateID() As Long
    Public Property Name() As String
    Public Property Description() As String
    Public Property TemplateType() As String
    Public Property HTMLFormat() As Boolean
    Public Property FromAddress() As String
    Public Property Action() As String
    Public Property EmailSubject() As String
    Public Property EmailBody() As String
    Public Property Active() As Boolean
    Public Property Master() As Boolean
End Class
