<Serializable()> _
Public Class DEPartnerUserClub
    Private _clubCode As String
    Private _available As Boolean
    Private _isDefault As Boolean

    Public Property ClubCode() As String
        Get
            Return _clubCode
        End Get
        Set(ByVal value As String)
            _clubCode = value
        End Set
    End Property
    Public Property Available() As Boolean
        Get
            Return _available
        End Get
        Set(ByVal value As Boolean)
            _available = value
        End Set
    End Property
    Public Property IsDefault() As Boolean
        Get
            Return _isDefault
        End Get
        Set(ByVal value As Boolean)
            _isDefault = value
        End Set
    End Property
End Class
