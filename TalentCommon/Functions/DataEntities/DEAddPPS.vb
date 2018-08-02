<Serializable()> _
Public Class DEAddPPS


    Private _custNo As String
    Public Property CustomerNumber() As String
        Get
            Return _custNo
        End Get
        Set(ByVal value As String)
            _custNo = value
        End Set
    End Property

    Private _ppsStage As String
    Public Property PPSStage() As String
        Get
            Return _ppsStage
        End Get
        Set(ByVal value As String)
            _ppsStage = value
        End Set
    End Property

    Private _regPost As Boolean
    Public Property RegisteredPost() As Boolean
        Get
            Return _regPost
        End Get
        Set(ByVal value As Boolean)
            _regPost = value
        End Set
    End Property

    Private _errorCode As String
    Public Property ErrorCode() As String
        Get
            Return _errorCode
        End Get
        Set(ByVal value As String)
            _errorCode = value
        End Set
    End Property


    Private _source As String
    Public Property Source() As String
        Get
            Return _source
        End Get
        Set(ByVal value As String)
            _source = value
        End Set
    End Property


    Private _ppsMode As String
    Public Property PPSMode() As String
        Get
            Return _ppsMode
        End Get
        Set(ByVal value As String)
            _ppsMode = value
        End Set
    End Property


    Private _ccDetails As Generic.Dictionary(Of String, String)
    Public Property CreditCardDetails() As Generic.Dictionary(Of String, String)
        Get
            Return _ccDetails
        End Get
        Set(ByVal value As Generic.Dictionary(Of String, String))
            _ccDetails = value
        End Set
    End Property

    Private _ppsItems As Generic.List(Of DePPSItem)
    Public Property PPSItems() As Generic.List(Of DePPSItem)
        Get
            Return _ppsItems
        End Get
        Set(ByVal value As Generic.List(Of DePPSItem))
            _ppsItems = value
        End Set
    End Property
    Private _sessionID As String
    Public Property SessionId() As String
        Get
            Return _sessionID
        End Get
        Set(ByVal value As String)
            _sessionID = value
        End Set
    End Property

    Private _seasonTkt As String = String.Empty
    Public Property SeasonTicket() As String
        Get
            Return _seasonTkt
        End Get
        Set(ByVal value As String)
            _seasonTkt = value
        End Set
    End Property

    Private _byPassPreReqCheck As String
    Public Property ByPassPreReqCheck() As String
        Get
            Return _byPassPreReqCheck
        End Get
        Set(ByVal value As String)
            _byPassPreReqCheck = value
        End Set
    End Property

    Public Sub New()
        MyBase.New()

        Me.PPSItems = New Generic.List(Of DePPSItem)
        Me.CreditCardDetails = New Generic.Dictionary(Of String, String)
    End Sub

End Class

<Serializable()> _
Public Class DePPSItem


    Private _productCode As String
    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property

    Private _seat As String
    Public Property Seat() As String
        Get
            Return _seat
        End Get
        Set(ByVal value As String)
            _seat = value
        End Set
    End Property

    Private _customer As String
    Public Property CustomerNumber() As String
        Get
            Return _customer
        End Get
        Set(ByVal value As String)
            _customer = value
        End Set
    End Property

End Class
