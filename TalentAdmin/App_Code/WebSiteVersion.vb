Public Class WebSiteVersion

    Private _version As Integer = 0
    Private _subVersion As Integer = 0
    Private _ptf As Integer = 0
    Private _client As String = ""

    Public Property Version() As Integer
        Get
            Return _version
        End Get
        Set(ByVal value As Integer)
            _version = value
        End Set
    End Property

    Public Property SubVersion() As Integer
        Get
            Return _subVersion
        End Get
        Set(ByVal value As Integer)
            _subVersion = value
        End Set
    End Property

    Public Property PTF() As Integer
        Get
            Return _ptf
        End Get
        Set(ByVal value As Integer)
            _ptf = value
        End Set
    End Property

    Public Property Client() As String
        Get
            Return _client
        End Get
        Set(ByVal value As String)
            _client = value
        End Set
    End Property

End Class
