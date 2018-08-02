
<Serializable()> _
Public Class DEPPSEnrolment
    Private _customerNumber As String = String.Empty
    Private _paymentDetails As New DEPayments
    Private _enrolmentSchemes As New Generic.List(Of DEPPSEnrolmentScheme)


    Public Property CustomerNumber() As String
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As String)
            _customerNumber = value
        End Set
    End Property

    Public Property PaymentDetails() As DEPayments
        Get
            Return _paymentDetails
        End Get
        Set(ByVal value As DEPayments)
            _paymentDetails = value
        End Set
    End Property

    Public Property EnrolmentSchemes() As Generic.List(Of DEPPSEnrolmentScheme)
        Get
            Return _enrolmentSchemes
        End Get
        Set(ByVal value As Generic.List(Of DEPPSEnrolmentScheme))
            _enrolmentSchemes = value
        End Set
    End Property


End Class
