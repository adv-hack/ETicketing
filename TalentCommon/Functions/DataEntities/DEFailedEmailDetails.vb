
<Serializable()> _
Public Class DEFailedEmailDetails

    Private _CustomerNumber As String
    Private _Title As String
    Private _Forename As String
    Private _Surname As String
    Private _EmailAddress As String
    Private _ReasonForFailure As String
    Private _DateSent As New DateTime()

    Public Property CustomerNumber() As String
        Get
            Return _CustomerNumber
        End Get
        Set(ByVal value As String)
            _CustomerNumber = value
        End Set
    End Property

    Public Property Title() As String
        Get
            Return _Title
        End Get
        Set(ByVal value As String)
            _Title = value
        End Set
    End Property

    Public Property Forename() As String
        Get
            Return _Forename
        End Get
        Set(ByVal value As String)
            _Forename = value
        End Set
    End Property

    Public Property Surname() As String
        Get
            Return _Surname
        End Get
        Set(ByVal value As String)
            _Surname = value
        End Set
    End Property

    Public Property EmailAddress() As String
        Get
            Return _EmailAddress
        End Get
        Set(ByVal value As String)
            _EmailAddress = value
        End Set
    End Property

    Public Property ReasonForFailure() As String
        Get
            Return _ReasonForFailure
        End Get
        Set(ByVal value As String)
            _ReasonForFailure = value
        End Set
    End Property

    Public Property DateSent() As DateTime
        Get
            Return _DateSent
        End Get
        Set(ByVal value As DateTime)
            _DateSent = value
        End Set
    End Property

End Class
