<Serializable()> _
Public Class DEPPS

    Private _sessionId As String = ""
    Private _enrolments As New Generic.List(Of DEPPSEnrolment)
    Private _customerNumber As String = String.Empty
    Private _productCode As String = String.Empty
    Private _seatDetails As String = String.Empty
    Private _updateMode As Boolean = False
    Private _retrieveMode As Boolean = False
    Private _cancelEnrolMode As Boolean = False

    Public Property SessionId() As String
        Get
            Return _sessionId
        End Get
        Set(ByVal value As String)
            _sessionId = value
        End Set
    End Property

    Public Property Enrolments() As Generic.List(Of DEPPSEnrolment)
        Get
            Return _enrolments
        End Get
        Set(ByVal value As Generic.List(Of DEPPSEnrolment))
            _enrolments = value
        End Set
    End Property

    Public Property CustomerNumber As String
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As String)
            _customerNumber = value
        End Set
    End Property

    Public Property ProductCode As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property

    Public Property SeatDetails As String
        Get
            Return _seatDetails
        End Get
        Set(ByVal value As String)
            _seatDetails = value
        End Set
    End Property

    Public Property UpdateMode As Boolean
        Get
            Return _updateMode
        End Get
        Set(ByVal value As Boolean)
            _updateMode = value
        End Set
    End Property

    Public Property RetrieveMode As Boolean
        Get
            Return _retrieveMode
        End Get
        Set(ByVal value As Boolean)
            _retrieveMode = value
        End Set
    End Property
    Public Property CancelEnrolMode As Boolean
        Get
            Return _cancelEnrolMode
        End Get
        Set(ByVal value As Boolean)
            _cancelEnrolMode = value
        End Set
    End Property

End Class
