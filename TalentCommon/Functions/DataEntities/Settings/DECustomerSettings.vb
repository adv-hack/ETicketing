<Serializable()> _
Public Class DECustomerSettings
    Inherits DESettings

    Private _creationType As String
    Public Property CreationType() As String
        Get
            Return _creationType
        End Get
        Set(ByVal value As String)
            _creationType = value
        End Set
    End Property

End Class
