Public Class ErrorObj

    Private _hasError As Boolean = False            ' true false flag

    Private _errorMessage As String = String.Empty  ' System generated error message
    Private _errorNumber As String = String.Empty   ' program generated error number
    Private _errorInfo As String = String.Empty
    Private _errorLogFile As String = "C:/DBAdminError.log"

    Public Property HasError() As Boolean
        Get
            Return _hasError
        End Get
        Set(ByVal value As Boolean)
            _hasError = value
        End Set
    End Property

    Public Property ErrorMessage() As String
        Get
            Return _errorMessage
        End Get
        Set(ByVal value As String)
            _errorMessage = value
        End Set
    End Property
    Public Property ErrorNumber() As String
        Get
            Return _errorNumber
        End Get
        Set(ByVal value As String)
            _errorNumber = value
        End Set
    End Property
    Public Property ErrorInfo() As String
        Get
            Return _errorInfo
        End Get
        Set(ByVal value As String)
            _errorInfo = value
        End Set
    End Property
    Public Property ErrorLogFile() As String
        Get
            Return _errorLogFile
        End Get
        Set(ByVal value As String)
            _errorLogFile = value
        End Set
    End Property

End Class
