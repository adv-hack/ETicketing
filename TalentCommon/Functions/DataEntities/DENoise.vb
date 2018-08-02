Public Enum TblActiveNoiseSessions_Usage
    TRADEPLACE
    ''' <summary>
    ''' Default value for usage
    ''' </summary>
    TICKETING
End Enum

<Serializable()> _
Public Class DENoise

    Public Property ServerIP As String = String.Empty
    Public Property ClientIP As String = String.Empty

    Private _sessionID As String
    Public Property SessionID() As String
        Get
            Return _sessionID
        End Get
        Set(ByVal value As String)
            _sessionID = value
        End Set
    End Property

    Private _lastActivity As DateTime
    Public Property LastActivity() As DateTime
        Get
            Return _lastActivity
        End Get
        Set(ByVal value As DateTime)
            _lastActivity = value
        End Set
    End Property

    Private _lastActivityThreshold As DateTime
    Public Property LastActivityThreshold() As DateTime
        Get
            Return _lastActivityThreshold
        End Get
        Set(ByVal value As DateTime)
            _lastActivityThreshold = value
        End Set
    End Property

    Private _maxConcurrentUsers As Integer
    Public Property MaxConcurrentUsers() As Integer
        Get
            Return _maxConcurrentUsers
        End Get
        Set(ByVal value As Integer)
            _maxConcurrentUsers = value
        End Set
    End Property

    Private _maxsessionKeepAliveMinutes As Integer
    Public Property MaxSessionKeepAliveMinutes() As Integer
        Get
            Return _maxsessionKeepAliveMinutes
        End Get
        Set(ByVal value As Integer)
            _maxsessionKeepAliveMinutes = value
        End Set
    End Property

    Private _isAgent As Boolean
    Public Property IsAgent() As Boolean
        Get
            Return _isAgent
        End Get
        Set(ByVal value As Boolean)
            _isAgent = value
        End Set
    End Property

    Private _agentName As String
    Public Property AgentName() As String
        Get
            Return _agentName
        End Get
        Set(ByVal value As String)
            _agentName = value
        End Set
    End Property

    Private _agentType As Integer
    Public Property AgentType() As Integer
        Get
            Return _agentType
        End Get
        Set(ByVal value As Integer)
            _agentType = value
        End Set
    End Property

    Private _usage As TblActiveNoiseSessions_Usage = TblActiveNoiseSessions_Usage.TICKETING

    ''' <summary>
    ''' Gets or sets the usage type based on the enum TblActiveNoiseSessions_Usage
    ''' </summary>
    ''' <value>The usage.</value>
    Public Property Usage() As TblActiveNoiseSessions_Usage
        Get
            Return _usage
        End Get
        Set(ByVal value As TblActiveNoiseSessions_Usage)
            _usage = value
        End Set
    End Property

End Class
