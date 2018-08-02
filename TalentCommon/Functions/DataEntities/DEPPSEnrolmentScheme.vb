
<Serializable()> _
Public Class DEPPSEnrolmentScheme
    Private _customerNumber As String = String.Empty
    Private _productCode As String = String.Empty
    Private _seasonTicket As String = String.Empty
    Private _registeredPost As String = String.Empty
    Private _errorCode As String = String.Empty
    Private _amendPPSEnrolmentIgnoreFF As Boolean = False

    Public Property CustomerNumber() As String
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As String)
            _customerNumber = value
        End Set
    End Property

    Public Property ProductCode() As String
        Get
            Return _productCode
        End Get
        Set(ByVal value As String)
            _productCode = value
        End Set
    End Property

    Public Property SeasonTicket() As String
        Get
            Return _seasonTicket
        End Get
        Set(ByVal value As String)
            _seasonTicket = value
        End Set
    End Property

    Public Property RegisteredPost() As String
        Get
            Return _registeredPost
        End Get
        Set(ByVal value As String)
            _registeredPost = value
        End Set
    End Property

    Public Property AmendPPSEnrolmentIgnoreFF() As Boolean
        Get
            Return _amendPPSEnrolmentIgnoreFF
        End Get
        Set(ByVal value As Boolean)
            _amendPPSEnrolmentIgnoreFF = value
        End Set
    End Property

    Public Property ErrorCode() As String
        Get
            Return _errorCode
        End Get
        Set(ByVal value As String)
            _errorCode = value
        End Set
    End Property

End Class
