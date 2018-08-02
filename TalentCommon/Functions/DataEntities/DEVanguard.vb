


<Serializable()> _
Public Class DEVanguard

    Public Enum ProcessingIdentifier

        AUTHANDCHARGE = 1
        AUTHONLY = 2
        CHARGEONLY = 3

    End Enum

    Public Enum CheckoutStages
        CHECKOUT
        PPS1
        PPS2
        SAVEMYCARD
        AMENDPPSCARD
    End Enum

    Public Enum ProcessingStep
        START_PAYMENT = 100
        CARD_TYPE_CHANGED = 200
        GENERATED_SESSION_IDENTIFIER = 300
        SAVE_CARD_FLAG_UPDATE = 400
        SECURE3D_OFF_PPS_TRANSACTION = 500
        SECURE3D_ON_PPS_TRANSACTION_PART1 = 600
        SECURE3D_ON_PPS_TRANSACTION_PART2 = 700
        SECURE3D_OFF_CHECKOUT_TRANSACTION = 800
        SECURE3D_ON_CHECKOUT_PART1 = 900
        SECURE3D_ON_CHECKOUT_PART2 = 1000
        GET_CARD_DETAIL = 1100
        CARD_TOKENISATION = 1200
        TRANSACTION_SEND = 1300
        TRANSACTION_REJECT = 1400
        TRANSACTION_CONFIRM = 1500
        SECURE3D_OFF_CHECKOUT_PARTPAYMENT = 1600
        SECURE3D_ON_CHECKOUT_PARTPAYMENT1 = 1700
        SECURE3D_ON_CHECKOUT_PARTPAYMENT2 = 1800

        START_SAVEMYCARD = 5000
        SAVE_MY_CARD = 6000

        START_AMENDPPSCARD = 8000
        SAVE_AMENDPPS_CARD = 9000

    End Enum

    Public Enum TransactionType
        PURCHASE = 1
        REFUND = 2
        CASH_ADVANCE = 4
        PURCHASE_WITH_CASH_BACK = 5
        CONTD_AUTHORITY = 6
    End Enum

    Private _payerAuth As New PayerAuthentication

    Private _talentVGGatewayPage As String = String.Empty

    Public Property CacheConfigSeconds() As Integer = 0
    Public Property GatewayUrl() As String = String.Empty
    Public Property SystemID() As Decimal
    Public Property SystemGUID() As String = String.Empty
    Public Property SystemPasscode() As String = String.Empty
    Public Property FullCapture() As Boolean
    Public Property CardCapturePage() As String = String.Empty
    Public Property ProcessingDB() As String = String.Empty
    Public Property CDATAWrapping() As Boolean

    Public Property PreviousProcessingStep() As Integer
    Public Property SessionID() As String = String.Empty
    Public Property BasketPaymentID() As Long
    Public Property BasketHeaderID() As Long
    Public Property TempOrderID() As String
    Public Property CheckOutStage() As CheckoutStages = CheckoutStages.CHECKOUT
    Public Property CaptureMethod() As String = String.Empty
    Public Property SaveThisCard() As Boolean
    Public Property PaymentType() As String = String.Empty
    Public Property ProcessStep() As ProcessingStep = Nothing
    Public Property ExpiryMonth() As String = String.Empty
    Public Property ExpiryYear() As String = String.Empty
    Public Property TxnType() As TransactionType = TransactionType.PURCHASE
    Public Property PaymentAmount() As String = "0"

    Public Property BasketAmount() As Decimal

    'returnurl where to set this front DB or backend
    Public Property TalentVGGatewayPage() As String
        Get
            Return _talentVGGatewayPage
        End Get
        Set(value As String)
            If Not value.Contains("?") Then
                _talentVGGatewayPage = value & "?"
            End If
        End Set
    End Property

    Public Property ReturnURL() As String = String.Empty
    Public Property SessionGUID() As String = String.Empty
    Public Property CanLogTheProcessError() As Boolean
    Public Property CanLogTheProcessXML() As Boolean = True
    Public Property SessionPassCode() As String = String.Empty
    Public Property CardType() As String = String.Empty
    Public Property PaymentProcess3dSecure() As Boolean
    Public Property TransactionCurrencyCode() As String = String.Empty
    Public Property ApacsTerminalCapabilities() As String = String.Empty
    Public Property TerminalCountryCode() As String = String.Empty
    Public Property ProcessIdentifier() As ProcessingIdentifier = ProcessingIdentifier.AUTHANDCHARGE
    Public Property CashBack() As Decimal = 0
    Public Property Gratuity() As Decimal = 0
    Public Property AccountID() As String = String.Empty
    Public Property AccountPasscode() As String
    Public Property ReturnHash() As Boolean = False
    Public Property TransactionID() As String = String.Empty
    Public Property OfflineAuthCode() As String = ""
    Public Property AddressLine1() As String = String.Empty
    Public Property Postcode() As String = String.Empty

    'Properties for 3D-Secure Transactions
    Public Property IframeWidth() As Integer
    Public Property MKAccountID() As Decimal
    Public Property MKAcquirerId() As Decimal
    Public Property MerchantName() As String
    Public Property MerchantUrl() As String
    Public Property VisaMerchantBankId() As String
    Public Property VisaMerchantNumber() As String
    Public Property VisaMerchantPassword() As String
    Public Property MCMMerchantBankId() As String
    Public Property MCMMerchantNumber() As String
    Public Property MCMMerchantPassword() As String
    Public Property CurrencyExponent() As String
    Public Property BrowserAcceptHeader() As String
    Public Property BrowserUserAgentHeader() As String
    Public Property TransactionAmount() As String
	Public Property SecurityNumber() As String

    Private _transactionDisplayAmount As String = String.Empty
    Public Property TransactionDisplayAmount() As String
        Get
            Return _transactionDisplayAmount
        End Get
        Set(value As String)
            _transactionDisplayAmount = String.Format("{0:0.00}", CDec(value))
        End Set
    End Property
    Public Property TransactionDescription() As String

    Public Property PayerAuth() As PayerAuthentication
        Get
            Return _payerAuth
        End Get
        Set(value As PayerAuthentication)
            _payerAuth = value
        End Set
    End Property

    Public Property Is3DTransactionCompleted As Boolean = False

    Public Class PayerAuthentication
        ' EXISTING VARIABLES TO BE CONVERTED TO PROPERTIES 
        Public Property AuthenticationStatus As String = String.Empty '"Y"
        Public Property AuthenticationCAVV As String = String.Empty '"jOJxBg52QGg3ABEAAABkKW//KuI="
        Public Property AuthenticationeCI As String = String.Empty '02
        Public Property ATSData As String = String.Empty ' D09100
        Public Property TransactionID As String = String.Empty '"160777"

        ' NEW PROPERTIES ADDED         
        Public Property PayerAuthRequestID As String = String.Empty
        Public Property Enrolled As String = String.Empty

        Public Property AcsUrl As String = String.Empty
        Public Property PareQ As String = String.Empty
        Public Property PareS As String = String.Empty
        Public Property AuthenticationCertificate As String = String.Empty
        Public Property AuthenticationTime As String = String.Empty

        Public ReadOnly Property IsEnrolled() As Boolean
            Get
                If Enrolled = "Y" Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

    End Class

End Class
