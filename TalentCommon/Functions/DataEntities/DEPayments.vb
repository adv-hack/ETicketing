'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Payments
'
'       Date                        28th Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       When passed to System21 must restrict _PaymentType to max length of 1024
'       so if greater multiple calls will be needed.
'
'       Error Number Code base      TACDEPY- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DEPayments
    '---------------------------------------------------------------------------------
    '
    Private _paymentType As String = String.Empty        ' CC, DD, SO, CQ, CS
    Private _paymentMode As String = String.Empty        ' Inline, None, Batch
    Private _paymentStage As String = String.Empty       ' Checkout Page
    Private _bank As String = String.Empty               ' HSBC
    Private _branch As String = String.Empty             ' Nantwich
    Private _originator As String = String.Empty         ' 123456
    Private _sortCode As String = String.Empty           ' 23-05-87
    Private _paymentDay As String = String.Empty           ' 23
    Private _amount As String = String.Empty             ' �1,234.00
    Private _paymentDate As String = String.Empty        ' 05 Nov 2006
    Private _paymentRef As String = String.Empty         ' Payment Ref
    '
    Private _accountType As String = String.Empty        ' Current, VISA, Delta,
    Private _accountNumber As String = String.Empty      ' 4796587658975985
    Private _accountName As String = String.Empty        ' Fred Bloggso
    Private _accountPostCode As String = String.Empty    ' CW7 3WE
    Private _accountAddress As String = String.Empty     ' 12, Our Street, Birmingham
    Private _ddiRef As String = String.Empty             ' 1234567890   

    Private _defaultCard As Boolean = False              ' Default Card Flag
    Private _cardType As String = String.Empty           ' Current, VISA, Delta,
    Private _cardNumber As String = String.Empty         ' see Utilities.ValidateCardNumber
    Private _cardName As String = String.Empty           ' Fred Bloggso
    Private _cardPostCode As String = String.Empty       ' CW7 3WE
    Private _cardAddress As String = String.Empty        ' 12, Our Street, Birmingham
    Private _saveMyCardMode As String = String.Empty     ' Options: 'S'=Save, 'U'=Use, 'R'=Retrieve, 'D'=Delete
    Private _uniqueCardId As String = String.Empty       ' This is the ID given to the record in CD050/CD051 where the card number and customer are stored
    Private _lastFourCardDigits As String = String.Empty ' Used for save my card the server side property for the selected card last 4 digits

    Private _chequeNumber As String = String.Empty       ' Cheque Number
    Private _chequeAccount As String = String.Empty      ' Cheque Account

    Private _expiryDate As String = String.Empty         ' 0506
    Private _startDate As String = String.Empty          ' 0104
    Private _issueNumber As String = String.Empty        ' 1
    Private _cV2Number As String = String.Empty          ' 155
    Private _RetainCustReservation As String = String.Empty   'Retain Customer Reservation flag

    Private _sessionId As String = String.Empty          ' Unique session id 
    Private _source As String = String.Empty             ' Source (of request) ('W'=web site, 'S'=Web service)
    Private _feeDepartment As String = String.Empty
    Private _exPayment As String = String.Empty
    Private _marketingCampaign As String = String.Empty
    Private _sessionCampaignCode As String = String.Empty 'Campaign code from Session("CAMPAIGN_CODE")
    Private _autoEnrol As Boolean = False

    Private _cashbackMode As String = String.Empty      ' Mode to call WS057R in ('1' = request, '2' = update)
    Private _cashbackCustomersSelected As Collection    ' List of customers that have been selected for cashback rewards
    Private _totalRewardSelected As Collection          ' List of customers total reward that have been selected
    Private _availableRewardSelected As Collection      ' List of customers available reward that have been selected
    Private _feeCode As String = String.Empty           ' Adhoc Fee Code
    Private _feeMode As String = String.Empty           ' The mode to determine if we are adding the fee or removing the fee ('A' or 'D')
    Private _giftAid As Boolean = False                 ' Gift Aid Flag - determines whether or not gift aid has been selected
    Private _accountId As String = String.Empty         ' Account/Merchant ID used during Commidea 3D Secure
    Private _chipAndPinIPAddress As String = String.Empty ' When using chip and pin this is the device IP address selected by the agent on the checkout page
    Private _pointOfSaleIPAddress As String = String.Empty
    Private _pointOfSaleTCPPort As String = String.Empty
    Private _variablePricedProductType As String = String.Empty

    Private _deFeesPartPayment As New DEFeesPartPayment

    Public Property PaymentOptionCode() As String
    Public Property clubProductCodeForFinance() As String = String.Empty
    Public Property PartPaymentId() As String
    Public Property CATMode() As String = String.Empty
    Public Property BasketPaymentFeesEntityList() As New Generic.List(Of DEFeesPayment)
    Public Property FeesCount() As Integer
    Public Property OptionTokencode() As String
    Public Property CustomerPresent() As Boolean
    Public Property AdjustmentType() As String
    Public Property CancelAll() As String
    Public Property Reason() As String
    Public Property ActivationDate() As String
    Public Property ConfirmCarryOverAllowance() As String
    Public Property ExpiryOption() As String
    Public Property IsGiftCard() As Boolean
    Public Property PIN() As String
    Public Property Currency() As String
    Public Property PartPaymentApplyTypeFlag() As String = String.Empty
    Public Property AllowManualIntervention() As Boolean = False

    Public Property TokenID() As String = String.Empty
    Public Property ProcessingDB() As String = String.Empty
    Public Property TransactionID() As String = String.Empty
    Public Property TokenDate() As String = String.Empty
    Public Property CanResetPayProcess() As Boolean = False
    Public Property BasketContentType() As String = String.Empty

    Public Property FeesPartPaymentEntity() As DEFeesPartPayment
        Get
            Return _deFeesPartPayment
        End Get
        Set(ByVal value As DEFeesPartPayment)
            _deFeesPartPayment = value
        End Set
    End Property

    Public Property PaymentType() As String
        Get
            Return _paymentType
        End Get
        Set(ByVal value As String)
            _paymentType = value
        End Set
    End Property         ' 
    Public Property PaymentMode() As String
        Get
            Return _paymentMode
        End Get
        Set(ByVal value As String)
            _paymentMode = value
        End Set
    End Property
    Public Property PaymentStage() As String
        Get
            Return _paymentStage
        End Get
        Set(ByVal value As String)
            _paymentStage = value
        End Set
    End Property
    Public Property Bank() As String
        Get
            Return _bank
        End Get
        Set(ByVal value As String)
            _bank = value
        End Set
    End Property                ' 
    Public Property Branch() As String
        Get
            Return _branch
        End Get
        Set(ByVal value As String)
            _branch = value
        End Set
    End Property
    Public Property Originator() As String
        Get
            Return _originator
        End Get
        Set(ByVal value As String)
            _originator = value
        End Set
    End Property
    Public Property SortCode() As String
        Get
            Return _sortCode
        End Get
        Set(ByVal value As String)
            _sortCode = value
        End Set
    End Property            ' 
    Public Property PaymentDay() As String
        Get
            Return _paymentDay
        End Get
        Set(ByVal value As String)
            _paymentDay = value
        End Set
    End Property            ' 
    Public Property Amount() As String
        Get
            Return _amount
        End Get
        Set(ByVal value As String)
            _amount = value
        End Set
    End Property              ' 
    Public Property RetailAmount() As String
    Public Property PaymentDate() As String
        Get
            Return _paymentDate
        End Get
        Set(ByVal value As String)
            _paymentDate = value
        End Set
    End Property
    Public Property PaymentRef() As String
        Get
            Return _paymentRef
        End Get
        Set(ByVal value As String)
            _paymentRef = value
        End Set
    End Property

    Public Property AccountType() As String
        Get
            Return _accountType
        End Get
        Set(ByVal value As String)
            _accountType = value
        End Set
    End Property
    Public Property AccountNumber() As String
        Get
            Return _accountNumber
        End Get
        Set(ByVal value As String)
            _accountNumber = value
        End Set
    End Property
    Public Property AccountName() As String
        Get
            Return _accountName
        End Get
        Set(ByVal value As String)
            _accountName = value
        End Set
    End Property
    Public Property AccountpostCode() As String
        Get
            Return _accountPostCode
        End Get
        Set(ByVal value As String)
            _accountPostCode = value
        End Set
    End Property
    Public Property AccountAddress() As String
        Get
            Return _accountAddress
        End Get
        Set(ByVal value As String)
            _accountAddress = value
        End Set
    End Property
    Public Property DDIReference() As String
        Get
            Return _ddiRef
        End Get
        Set(ByVal value As String)
            _ddiRef = value
        End Set
    End Property

    Public Property DefaultCard() As Boolean
        Get
            Return _defaultCard
        End Get
        Set(ByVal value As Boolean)
            _defaultCard = value
        End Set
    End Property
    Public Property CardType() As String
        Get
            Return _cardType
        End Get
        Set(ByVal value As String)
            _cardType = value
        End Set
    End Property
    Public Property CardNumber() As String
        Get
            Return _cardNumber
        End Get
        Set(ByVal value As String)
            _cardNumber = value
        End Set
    End Property
    Public Property CardName() As String
        Get
            Return _cardName
        End Get
        Set(ByVal value As String)
            _cardName = value
        End Set
    End Property
    Public Property CardpostCode() As String
        Get
            Return _cardPostCode
        End Get
        Set(ByVal value As String)
            _cardPostCode = value
        End Set
    End Property
    Public Property CardAddress() As String
        Get
            Return _cardAddress
        End Get
        Set(ByVal value As String)
            _cardAddress = value
        End Set
    End Property
    Public Property SaveMyCardMode() As String
        Get
            Return _saveMyCardMode
        End Get
        Set(ByVal value As String)
            _saveMyCardMode = value
        End Set
    End Property
    Public Property UniqueCardId() As String
        Get
            Return _uniqueCardId
        End Get
        Set(ByVal value As String)
            _uniqueCardId = value
        End Set
    End Property
    Public Property LastFourCardDigits() As String
        Get
            Return _lastFourCardDigits
        End Get
        Set(ByVal value As String)
            _lastFourCardDigits = value
        End Set
    End Property

    Public Property ChequeNumber() As String
        Get
            Return _chequeNumber
        End Get
        Set(ByVal value As String)
            _chequeNumber = value
        End Set
    End Property
    Public Property ChequeAccount() As String
        Get
            Return _chequeAccount
        End Get
        Set(ByVal value As String)
            _chequeAccount = value
        End Set
    End Property

    Public Property ExpiryDate() As String
        Get
            Return _expiryDate
        End Get
        Set(ByVal value As String)
            _expiryDate = value
        End Set
    End Property
    Public Property StartDate() As String
        Get
            Return _startDate
        End Get
        Set(ByVal value As String)
            _startDate = value
        End Set
    End Property
    Public Property IssueNumber() As String
        Get
            Return _issueNumber
        End Get
        Set(ByVal value As String)
            _issueNumber = value
        End Set
    End Property
    Public Property CV2Number() As String
        Get
            Return _cV2Number
        End Get
        Set(ByVal value As String)
            _cV2Number = value
        End Set
    End Property

    Public Property RetainCustomerReservations() As String = "N"

    Public Property SessionId() As String
        Get
            Return _sessionId
        End Get
        Set(ByVal value As String)
            _sessionId = value
        End Set
    End Property
    Public Property Source() As String
        Get
            Return _source
        End Get
        Set(ByVal value As String)
            _source = value
        End Set
    End Property

    Public Property exPayment() As String
        Get
            Return _exPayment
        End Get
        Set(ByVal value As String)
            _exPayment = value
        End Set
    End Property

    Private _customerNumber As String
    Public Property CustomerNumber() As String
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As String)
            _customerNumber = value
        End Set
    End Property

    Public Property FeeDepartment() As String
        Get
            Return _feeDepartment
        End Get
        Set(ByVal value As String)
            _feeDepartment = value
        End Set
    End Property

    Public Property MarketingCampaign() As String
        Get
            Return _marketingCampaign
        End Get
        Set(ByVal value As String)
            _marketingCampaign = value
        End Set
    End Property

    Public Property SessionCampaignCode() As String
        Get
            Return _sessionCampaignCode
        End Get
        Set(ByVal value As String)
            _sessionCampaignCode = value
        End Set
    End Property

    Public Property AutoEnrol() As Boolean
        Get
            Return _autoEnrol
        End Get
        Set(ByVal value As Boolean)
            _autoEnrol = value
        End Set
    End Property

    Private _ThreeDSecureTransactionID As String
    Public Property ThreeDSecureTransactionId() As String
        Get
            Return _ThreeDSecureTransactionID
        End Get
        Set(ByVal value As String)
            _ThreeDSecureTransactionID = value
        End Set
    End Property

    Private _ThreeDSecureECI As String
    Public Property ThreeDSecureECI() As String
        Get
            Return _ThreeDSecureECI
        End Get
        Set(ByVal value As String)
            _ThreeDSecureECI = value
        End Set
    End Property

    Private _ThreeDSecureCAVV As String
    Public Property ThreeDSecureCAVV() As String
        Get
            Return _ThreeDSecureCAVV
        End Get
        Set(ByVal value As String)
            _ThreeDSecureCAVV = value
        End Set
    End Property

    Private _ThreeDSecureStatus As String
    Public Property ThreeDSecureStatus() As String
        Get
            Return _ThreeDSecureStatus
        End Get
        Set(ByVal value As String)
            _ThreeDSecureStatus = value
        End Set
    End Property

    Private _ThreeDSecureCardScheme As String
    Public Property ThreeDSecureCardScheme() As String
        Get
            Return _ThreeDSecureCardScheme
        End Get
        Set(ByVal value As String)
            _ThreeDSecureCardScheme = value
        End Set
    End Property

    Private _ThreeDSecureATSData As String
    Public Property ThreeDSecureATSData() As String
        Get
            Return _ThreeDSecureATSData
        End Get
        Set(ByVal value As String)
            _ThreeDSecureATSData = value
        End Set
    End Property

    Private _ThreeDSecureAuthenticationStatus As String
    Public Property ThreeDSecureAuthenticationStatus() As String
        Get
            Return _ThreeDSecureAuthenticationStatus
        End Get
        Set(ByVal value As String)
            _ThreeDSecureAuthenticationStatus = value
        End Set
    End Property

    Private _ThreeDSecureMode As String
    Public Property ThreeDSecureMode() As String
        Get
            Return _ThreeDSecureMode
        End Get
        Set(ByVal value As String)
            _ThreeDSecureMode = value
        End Set
    End Property

    Private _GenerateTransactionFileID As String
    Public Property GenerateTransactionFileID() As String
        Get
            Return _GenerateTransactionFileID
        End Get
        Set(ByVal value As String)
            _GenerateTransactionFileID = value
        End Set
    End Property

    Private _cardHolderName As String
    Public Property CardHolderName() As String
        Get
            Return _cardHolderName
        End Get
        Set(ByVal value As String)
            _cardHolderName = value
        End Set
    End Property

    Private _installments As String
    Public Property Installments() As String
        Get
            Return _installments
        End Get
        Set(ByVal value As String)
            _installments = value
        End Set
    End Property

    Private _ThreeDSecureEnrolled As String
    Public Property ThreeDSecureEnrolled() As String
        Get
            Return _ThreeDSecureEnrolled
        End Get
        Set(ByVal value As String)
            _ThreeDSecureEnrolled = value
        End Set
    End Property
    Private _ThreeDSecurePAResStatus As String
    Public Property ThreeDSecurePAResStatus() As String
        Get
            Return _ThreeDSecurePAResStatus
        End Get
        Set(ByVal value As String)
            _ThreeDSecurePAResStatus = value
        End Set
    End Property
    Private _ThreeDSecureSignatureVerification As String
    Public Property ThreeDSecureSignatureVerification() As String
        Get
            Return _ThreeDSecureSignatureVerification
        End Get
        Set(ByVal value As String)
            _ThreeDSecureSignatureVerification = value
        End Set
    End Property
    Private _ThreeDSecureXid As String
    Public Property ThreeDSecureXid() As String
        Get
            Return _ThreeDSecureXid
        End Get
        Set(ByVal value As String)
            _ThreeDSecureXid = value
        End Set
    End Property

    Public Property YearsAtAddress() As String
    Public Property ProductCodeForDD() As String
    Public Property AgentName() As String
    Public Property CustomerDataEntity() As New DECustomer
    Public Property MonthsAtAddress() As String
    Public Property HomeStatus() As String
    Public Property EmploymentStatus() As String
    Public Property GrossIncome() As String

   


    Public Function LogString() As String

        Dim sb As New System.Text.StringBuilder

        With sb
            .Append(PaymentType & ",")
            .Append(PaymentMode & ",")
            .Append(Bank & ",")
            .Append(Branch & ",")
            .Append(SortCode & ",")
            .Append(Amount & ",")
            .Append(PaymentDate & ",")
            .Append(AccountType & ",")
            .Append(AccountNumber & ",")
            .Append(AccountName & ",")
            .Append(AccountpostCode & ",")
            .Append(AccountAddress & ",")
            .Append(CardType & ",")
            .Append(CardNumber & ",")
            .Append(CardName & ",")
            .Append(CardpostCode & ",")
            .Append(CardAddress & ",")
            .Append(ChequeNumber & ",")
            .Append(ChequeAccount & ",")
            .Append(ExpiryDate & ",")
            .Append(StartDate & ",")
            .Append(IssueNumber & ",")
            .Append(CV2Number & ",")
            .Append(RetainCustomerReservations & ",")
            .Append(SessionId & ",")
            .Append(PaymentOptionCode & ",")
            .Append(YearsAtAddress & ",")
            .Append(Source & ",")
            .Append(AdjustmentType & ",")
            .Append(CancelAll & ",")
            .Append(Reason & ",")
            .Append(ConfirmCarryOverAllowance & ",")
            .Append(ExpiryOption)
        End With

        Return sb.ToString.Trim

    End Function

    Public Property CashbackMode() As String
        Get
            Return _cashbackMode
        End Get
        Set(ByVal value As String)
            _cashbackMode = value
        End Set
    End Property

    Public Property CashbackCustomersSelected() As Collection
        Get
            Return _cashbackCustomersSelected
        End Get
        Set(ByVal value As Collection)
            _cashbackCustomersSelected = value
        End Set
    End Property

    Public Property TotalRewardSelected() As Collection
        Get
            Return _totalRewardSelected
        End Get
        Set(ByVal value As Collection)
            _totalRewardSelected = value
        End Set
    End Property

    Public Property AvailableRewardSelected() As Collection
        Get
            Return _availableRewardSelected
        End Get
        Set(ByVal value As Collection)
            _availableRewardSelected = value
        End Set
    End Property

    Public Property FeeCode() As String
        Get
            Return _feeCode
        End Get
        Set(ByVal value As String)
            _feeCode = value
        End Set
    End Property

    Public Property FeeMode() As String
        Get
            Return _feeMode
        End Get
        Set(ByVal value As String)
            _feeMode = value
        End Set
    End Property

    Public Property GiftAid() As Boolean
        Get
            Return _giftAid
        End Get
        Set(ByVal value As Boolean)
            _giftAid = value
        End Set
    End Property

    Public Property AccountId() As String
        Get
            Return _accountId
        End Get
        Set(ByVal value As String)
            _accountId = value
        End Set
    End Property

    Public Property ChipAndPinIPAddress() As String
        Get
            Return _chipAndPinIPAddress
        End Get
        Set(ByVal value As String)
            _chipAndPinIPAddress = value
        End Set
    End Property

    Public Property PointOfSaleIPAddress() As String
        Get
            Return _pointOfSaleIPAddress
        End Get
        Set(ByVal value As String)
            _pointOfSaleIPAddress = value
        End Set
    End Property

    Public Property PointOfSaleTCPPort() As String
        Get
            Return _pointOfSaleTCPPort
        End Get
        Set(ByVal value As String)
            _pointOfSaleTCPPort = value
        End Set
    End Property
    Public Property variablePricedProductType() As String
        Get
            Return _variablePricedProductType
        End Get
        Set(ByVal value As String)
            _variablePricedProductType = value
        End Set
    End Property
End Class
