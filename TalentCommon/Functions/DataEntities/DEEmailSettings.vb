'--------------------------------------------------------------------------------------------------
'       Project                     Talent Common
'
'       Function                    Data Entity For Email Settings
'
'       Date                        08/07/2010
'
'       Author                      Des Webster
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEEmailSettings

    'Email Settings
    Private _fromAddress As String = String.Empty
    Private _toAddress As String = String.Empty
    Private _cCAddress As String = String.Empty
    Private _smtpServer As String = String.Empty
    Private _smtpServerPort As String = String.Empty
    Private _emailTemplateID As String = String.Empty
    Private _attachments As New Generic.List(Of String)
    Private _OrderConfirmation As New DEEmailOrderConfirmation()
    Private _ForgottenPassword As New DEEmailForgottenPassword()
    Private _ChangePassword As New DEEmailChangePassword()
    Private _CustomerRegistration As New DEEmailCustomerRegistration()
    Private _OrderReturnConfirmation As New DEEmailOrderReturnConfirmation()
    Private _PPSPayment As New DEEmailPPSPayment()
    Private _FailedEmails As New DEEmailFailedEmails()
    Private _AmendPPS As New DEEmailAmendPPSEmail()
    Private _TicketExchangeConfirmation As New DETicketExchangeConfirmation()
    Private _TicketExchangeSaleConfirmation As New DETicketExchangeSaleConfirmation()
    Private _HospitalityQAReminderEmail As New DEHospitalityQAReminderEmail()

    Public Property FromAddress() As String
        Get
            Return _fromAddress
        End Get
        Set(ByVal value As String)
            _fromAddress = value
        End Set
    End Property

    Public Property ToAddress() As String
        Get
            Return _toAddress
        End Get
        Set(ByVal value As String)
            _toAddress = value
        End Set
    End Property

    Public Property CCAddress() As String
        Get
            Return _cCAddress
        End Get
        Set(ByVal value As String)
            _cCAddress = value
        End Set
    End Property

    Public Property SmtpServer() As String
        Get
            Return _smtpServer
        End Get
        Set(ByVal value As String)
            _smtpServer = value
        End Set
    End Property

    Public Property SmtpServerPort() As String
        Get
            Return _smtpServerPort
        End Get
        Set(ByVal value As String)
            _smtpServerPort = value
        End Set
    End Property

    Public Property EmailTemplateID() As String
        Get
            Return _emailTemplateID
        End Get
        Set(ByVal value As String)
            _emailTemplateID = value
        End Set
    End Property

    Public Property Attachments() As Generic.List(Of String)
        Get
            Return _attachments
        End Get
        Set(ByVal value As Generic.List(Of String))
            _attachments = value
        End Set
    End Property

    Public Property OrderConfirmation() As DEEmailOrderConfirmation
        Get
            Return _OrderConfirmation
        End Get
        Set(ByVal value As DEEmailOrderConfirmation)
            _OrderConfirmation = value
        End Set
    End Property

    Public Property ForgottenPassword() As DEEmailForgottenPassword
        Get
            Return _ForgottenPassword
        End Get
        Set(ByVal value As DEEmailForgottenPassword)
            _ForgottenPassword = value
        End Set
    End Property

    Public Property ChangePassword() As DEEmailChangePassword
        Get
            Return _ChangePassword
        End Get
        Set(ByVal value As DEEmailChangePassword)
            _ChangePassword = value
        End Set
    End Property

    Public Property CustomerRegistration() As DEEmailCustomerRegistration
        Get
            Return _CustomerRegistration
        End Get
        Set(ByVal value As DEEmailCustomerRegistration)
            _CustomerRegistration = value
        End Set
    End Property

    Public Property OrderReturnConfirmation() As DEEmailOrderReturnConfirmation
        Get
            Return _OrderReturnConfirmation
        End Get
        Set(ByVal value As DEEmailOrderReturnConfirmation)
            _OrderReturnConfirmation = value
        End Set
    End Property

    Public Property TicketExchangeConfirmation() As DETicketExchangeConfirmation
        Get
            Return _TicketExchangeConfirmation
        End Get
        Set(ByVal value As DETicketExchangeConfirmation)
            _TicketExchangeConfirmation = value
        End Set
    End Property

    Public Property TicketExchangeSaleConfirmation() As DETicketExchangeSaleConfirmation
        Get
            Return _TicketExchangeSaleConfirmation
        End Get
        Set(ByVal value As DETicketExchangeSaleConfirmation)
            _TicketExchangeSaleConfirmation = value
        End Set
    End Property
    Public Property PPSPayment() As DEEmailPPSPayment
        Get
            Return _PPSPayment
        End Get
        Set(ByVal value As DEEmailPPSPayment)
            _PPSPayment = value
        End Set
    End Property

    Public Property FailedEmails() As DEEmailFailedEmails
        Get
            Return _FailedEmails
        End Get
        Set(ByVal value As DEEmailFailedEmails)
            _FailedEmails = value
        End Set
    End Property

    Public Property AmendPPS() As DEEmailAmendPPSEmail
        Get
            Return _AmendPPS
        End Get
        Set(ByVal value As DEEmailAmendPPSEmail)
            _AmendPPS = value
        End Set
    End Property
    Public Property HospitalityQAReminderEmail() As DEHospitalityQAReminderEmail
        Get
            Return _HospitalityQAReminderEmail
        End Get
        Set(ByVal value As DEHospitalityQAReminderEmail)
            _HospitalityQAReminderEmail = value
        End Set
    End Property

    Public Class DEEmailOrderConfirmation
        'Parameter Settings for Order Confirmation emails
        Private _paymentReference As String = String.Empty
        Private _customer As String = String.Empty
        Private _websiteURL As String = String.Empty

        Public Property PaymentReference() As String
            Get
                Return _paymentReference
            End Get
            Set(ByVal value As String)
                _paymentReference = value
            End Set
        End Property

        Public Property Customer() As String
            Get
                Return _customer
            End Get
            Set(ByVal value As String)
                _customer = value
            End Set
        End Property

        Public Property WebsiteURL() As String
            Get
                Return _websiteURL
            End Get
            Set(ByVal value As String)
                _websiteURL = value
            End Set
        End Property
    End Class

    Public Class DEEmailForgottenPassword
        Private _Customer As String
        Private _LoginUrl As String
        Private _Token As String

        Public Property Customer() As String
            Get
                Return _Customer
            End Get
            Set(ByVal value As String)
                _Customer = value
            End Set
        End Property

        Public Property LoginUrl() As String
            Get
                Return _LoginUrl
            End Get
            Set(ByVal value As String)
                _LoginUrl = value
            End Set
        End Property
        Public Property Token() As String
            Get
                Return _Token
            End Get
            Set(ByVal value As String)
                _Token = value
            End Set
        End Property

    End Class

    Public Class DEEmailChangePassword
        Private _Customer As String
        Private _LoginUrl As String

        Public Property Customer() As String
            Get
                Return _Customer
            End Get
            Set(ByVal value As String)
                _Customer = value
            End Set
        End Property

        Public Property LoginUrl() As String
            Get
                Return _LoginUrl
            End Get
            Set(ByVal value As String)
                _LoginUrl = value
            End Set
        End Property

    End Class

    Public Class DEEmailCustomerRegistration
        Private _Customer As String
        Private _WebsiteAddress As String

        Public Property Customer() As String
            Get
                Return _Customer
            End Get
            Set(ByVal value As String)
                _Customer = value
            End Set
        End Property

        Public Property WebsiteAddress() As String
            Get
                Return _WebsiteAddress
            End Get
            Set(ByVal value As String)
                _WebsiteAddress = value
            End Set
        End Property

    End Class

    Public Class DEEmailOrderReturnConfirmation
        Private _Customer As String
        Private _OrderReturnReference As String
        Private _Mode As String

        Public Property Customer() As String
            Get
                Return _Customer
            End Get
            Set(ByVal value As String)
                _Customer = value
            End Set
        End Property

        Public Property OrderReturnReference() As String
            Get
                Return _OrderReturnReference
            End Get
            Set(ByVal value As String)
                _OrderReturnReference = value
            End Set
        End Property

        Public Property Mode() As String
            Get
                Return _Mode
            End Get
            Set(ByVal value As String)
                _Mode = value
            End Set
        End Property

    End Class

    Public Class DETicketExchangeConfirmation
        Private _Customer As String
        Private _TicketExchangeReference As String

        Public Property Customer() As String
            Get
                Return _Customer
            End Get
            Set(ByVal value As String)
                _Customer = value
            End Set
        End Property

        Public Property TicketExchangeReference() As String
            Get
                Return _TicketExchangeReference
            End Get
            Set(ByVal value As String)
                _TicketExchangeReference = value
            End Set
        End Property

    End Class

    Public Class DETicketExchangeSaleConfirmation
        Private _Customer As String
        Private _PaymentReference As Integer


        Public Property Customer() As String
            Get
                Return _Customer
            End Get
            Set(ByVal value As String)
                _Customer = value
            End Set
        End Property

        Public Property PaymentReference() As String
            Get
                Return _PaymentReference
            End Get
            Set(ByVal value As String)
                _PaymentReference = value
            End Set
        End Property


    End Class

    Public Class DEEmailPPSPayment
        Private _Customer As String
        Private _Description As String
        Private _Turnstiles As String
        Private _Gates As String
        Private _Seat As String

        Public Property Customer() As String
            Get
                Return _Customer
            End Get
            Set(ByVal value As String)
                _Customer = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal value As String)
                _Description = value
            End Set
        End Property

        Public Property Turnstiles() As String
            Get
                Return _Turnstiles
            End Get
            Set(ByVal value As String)
                _Turnstiles = value
            End Set
        End Property

        Public Property Gates() As String
            Get
                Return _Gates
            End Get
            Set(ByVal value As String)
                _Gates = value
            End Set
        End Property

        Public Property Seat() As String
            Get
                Return _Seat
            End Get
            Set(ByVal value As String)
                _Seat = value
            End Set
        End Property

        Public Property PaymentValue() As String

    End Class

    Public Class DEEmailFailedEmails
        Private _FEConnectionStrings As New Generic.List(Of String)
        Private _SupplyNetBusinessUnit As String
        
        Public Property FEConnectionStrings() As Generic.List(Of String)
            Get
                Return _FEConnectionStrings
            End Get
            Set(ByVal value As Generic.List(Of String))
                _FEConnectionStrings = value
            End Set
        End Property

        Public Property SupplyNetBusinessUnit() As String
            Get
                Return _SupplyNetBusinessUnit
            End Get
            Set(ByVal value As String)
                _SupplyNetBusinessUnit = value
            End Set
        End Property
    End Class

    Public Class DEEmailAmendPPSEmail
        Private _Customer As String
        Private _ProductCode As String

        Public Property Customer() As String
            Get
                Return _Customer
            End Get
            Set(ByVal value As String)
                _Customer = value
            End Set
        End Property

        Public Property ProductCode() As String
            Get
                Return _ProductCode
            End Get
            Set(ByVal value As String)
                _ProductCode = value
            End Set
        End Property
    End Class

    Public Class DEHospitalityQAReminderEmail
        Private _Customer As String
        Private _CallId As String
        Private _BookingPageURL As String
        Private _BusinessUnit As String

        Public Property Customer() As String
            Get
                Return _Customer
            End Get
            Set(ByVal value As String)
                _Customer = value
            End Set
        End Property

        Public Property CallId() As String
            Get
                Return _CallId
            End Get
            Set(ByVal value As String)
                _CallId = value
            End Set
        End Property
        Public Property BookingPageURL() As String
            Get
                Return _BookingPageURL
            End Get
            Set(ByVal value As String)
                _BookingPageURL = value
            End Set
        End Property
        Public Property BusinessUnit() As String
            Get
                Return _BusinessUnit
            End Get
            Set(ByVal value As String)
                _BusinessUnit = value
            End Set
        End Property
    End Class

End Class
