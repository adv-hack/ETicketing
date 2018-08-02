
<Serializable()> _
Public Class DEWarehouse
    Private _mode As String
    Private _code As String
    Private _description As String
    Private _descriptionLanguage As String

    Public Property Mode() As String
        Get
            Return _mode
        End Get
        Set(ByVal value As String)
            _mode = value
        End Set
    End Property

    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            _code = value
        End Set
    End Property

    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Property DescriptionLanguage() As String
        Get
            Return _descriptionLanguage
        End Get
        Set(ByVal value As String)
            _descriptionLanguage = value
        End Set
    End Property

End Class
