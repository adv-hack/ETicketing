<Serializable()> _
Public Class DECustomerV11

    Private _businessUnit As String
    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property


    Private _deCustomers As Generic.List(Of DECustomer)
    Public Property DECustomersV1() As Generic.List(Of DECustomer)
        Get
            Return _deCustomers
        End Get
        Set(ByVal value As Generic.List(Of DECustomer))
            _deCustomers = value
        End Set
    End Property



    Private _sites As Generic.List(Of DECustomerSite)
    Public Property Sites() As Generic.List(Of DECustomerSite)
        Get
            Return _sites
        End Get
        Set(ByVal value As Generic.List(Of DECustomerSite))
            _sites = value
        End Set
    End Property

    Sub New()
        MyBase.New()
        Sites = New Generic.List(Of DECustomerSite)
        DECustomersV1 = New Generic.List(Of DECustomer)
    End Sub


    '================================
    '       Sub Classes
    '================================
    <Serializable()> _
    Class DECustomerSite

        Private _UpdateMode As String
        Public Property UpdateMode() As String
            Get
                Return _UpdateMode
            End Get
            Set(ByVal value As String)
                _UpdateMode = value
            End Set
        End Property

        Private _name As String
        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Private _acc1 As String
        Public Property AccountNumber1() As String
            Get
                Return _acc1
            End Get
            Set(ByVal value As String)
                _acc1 = value
            End Set
        End Property
        Private _acc2 As String
        Public Property AccountNumber2() As String
            Get
                Return _acc2
            End Get
            Set(ByVal value As String)
                _acc2 = value
            End Set
        End Property
        Private _acc3 As String
        Public Property AccountNumber3() As String
            Get
                Return _acc3
            End Get
            Set(ByVal value As String)
                _acc3 = value
            End Set
        End Property
        Private _acc4 As String
        Public Property AccountNumber4() As String
            Get
                Return _acc4
            End Get
            Set(ByVal value As String)
                _acc4 = value
            End Set
        End Property
        Private _acc5 As String
        Public Property AccountNumber5() As String
            Get
                Return _acc5
            End Get
            Set(ByVal value As String)
                _acc5 = value
            End Set
        End Property


        Private _address As DECustomerSiteAddress
        Public Property Address() As DECustomerSiteAddress
            Get
                Return _address
            End Get
            Set(ByVal value As DECustomerSiteAddress)
                _address = value
            End Set
        End Property


        Private _telNo As String
        Public Property TelephoneNumber() As String
            Get
                Return _telNo
            End Get
            Set(ByVal value As String)
                _telNo = value
            End Set
        End Property

        Private _faxNo As String
        Public Property FaxNumber() As String
            Get
                Return _faxNo
            End Get
            Set(ByVal value As String)
                _faxNo = value
            End Set
        End Property

        Private _vatNo As String
        Public Property VATNumber() As String
            Get
                Return _vatNo
            End Get
            Set(ByVal value As String)
                _vatNo = value
            End Set
        End Property

        Private _url As String
        Public Property URL() As String
            Get
                Return _url
            End Get
            Set(ByVal value As String)
                _url = value
            End Set
        End Property

        Private _id As String
        Public Property ID() As String
            Get
                Return _id
            End Get
            Set(ByVal value As String)
                _id = value
            End Set
        End Property


        Private _crmBranch As String
        Public Property CRMBranch() As String
            Get
                Return _crmBranch
            End Get
            Set(ByVal value As String)
                _crmBranch = value
            End Set
        End Property

        Private _processName As String = String.Empty
        Public Property ProcessName() As String
            Get
                Return _processName
            End Get
            Set(ByVal value As String)
                _processName = value
            End Set
        End Property

        Private _processAccountNumber1 As String = String.Empty
        Public Property ProcessAccountNumber1() As String
            Get
                Return _processAccountNumber1
            End Get
            Set(ByVal value As String)
                _processAccountNumber1 = value
            End Set
        End Property

        Private _processAccountNumber2 As String = String.Empty
        Public Property ProcessAccountNumber2() As String
            Get
                Return _processAccountNumber2
            End Get
            Set(ByVal value As String)
                _processAccountNumber2 = value
            End Set
        End Property

        Private _processAccountNumber3 As String = String.Empty
        Public Property ProcessAccountNumber3() As String
            Get
                Return _processAccountNumber3
            End Get
            Set(ByVal value As String)
                _processAccountNumber3 = value
            End Set
        End Property

        Private _processAccountNumber4 As String = String.Empty
        Public Property ProcessAccountNumber4() As String
            Get
                Return _processAccountNumber4
            End Get
            Set(ByVal value As String)
                _processAccountNumber4 = value
            End Set
        End Property

        Private _processAccountNumber5 As String = String.Empty
        Public Property ProcessAccountNumber5() As String
            Get
                Return _processAccountNumber5
            End Get
            Set(ByVal value As String)
                _processAccountNumber5 = value
            End Set
        End Property

        Private _processAddress As String = String.Empty
        Public Property ProcessAddress() As String
            Get
                Return _processAddress
            End Get
            Set(ByVal value As String)
                _processAddress = value
            End Set
        End Property

        Private _processTelephoneNumber As String = String.Empty
        Public Property ProcessTelephoneNumber() As String
            Get
                Return _processTelephoneNumber
            End Get
            Set(ByVal value As String)
                _processTelephoneNumber = value
            End Set
        End Property

        Private _processfaxNumber As String = String.Empty
        Public Property ProcessFaxNumber() As String
            Get
                Return _processfaxNumber
            End Get
            Set(ByVal value As String)
                _processfaxNumber = value
            End Set
        End Property

        Private _processVATNumber As String = String.Empty
        Public Property ProcessVATNumber() As String
            Get
                Return _processVATNumber
            End Get
            Set(ByVal value As String)
                _processVATNumber = value
            End Set
        End Property

        Private _processURL As String = String.Empty
        Public Property ProcessURL() As String
            Get
                Return _processURL
            End Get
            Set(ByVal value As String)
                _processURL = value
            End Set
        End Property

        Private _processID As String = String.Empty
        Public Property ProcessID() As String
            Get
                Return _processID
            End Get
            Set(ByVal value As String)
                _processID = value
            End Set
        End Property

        Private _processCRMBranch As String = String.Empty
        Public Property ProcessCRMBranch() As String
            Get
                Return _processCRMBranch
            End Get
            Set(ByVal value As String)
                _processCRMBranch = value
            End Set
        End Property

        Private _contacts As Generic.List(Of DECustomerSiteContact)
        Public Property Contacts() As Generic.List(Of DECustomerSiteContact)
            Get
                Return _contacts
            End Get
            Set(ByVal value As Generic.List(Of DECustomerSiteContact))
                _contacts = value
            End Set
        End Property


        Sub New()
            MyBase.New()
            Address = New DECustomerSiteAddress
            Contacts = New Generic.List(Of DECustomerSiteContact)
        End Sub

    End Class
    <Serializable()> _
    Class DECustomerSiteContact
        Private _processTitle As String = String.Empty
        Private _processInitials As String = String.Empty
        Private _processForeName As String = String.Empty
        Private _processSurname As String = String.Empty
        Private _processFullName As String = String.Empty
        Private _processSalutation As String = String.Empty
        Private _processMothersName As String = String.Empty
        Private _processFathersName As String = String.Empty
        Private _processEmailAddress As String = String.Empty
        Private _processLoginID As String = String.Empty
        Private _processPassword As String = String.Empty
        Private _processAccountNumber1 As String = String.Empty
        Private _processAccountNumber2 As String = String.Empty
        Private _processAccountNumber3 As String = String.Empty
        Private _processAccountNumber4 As String = String.Empty
        Private _processAccountNumber5 As String = String.Empty
        Private _processAddress As String = String.Empty
        Private _processPosition As String = String.Empty
        Private _processGender As String = String.Empty
        Private _processTelephoneNumber1 As String = String.Empty
        Private _processTelephoneNumber2 As String = String.Empty
        Private _processTelephoneNumber3 As String = String.Empty
        Private _processTelephoneNumber4 As String = String.Empty
        Private _processTelephoneNumber5 As String = String.Empty
        Private _processDateOfBirth As String = String.Empty
        Private _processContactViaEmail As String = String.Empty
        Private _processContactViaMail As String = String.Empty
        Private _processHTMLNewsLetter As String = String.Empty
        Private _processSubscription1 As String = String.Empty
        Private _processSubscription2 As String = String.Empty
        Private _processSubscription3 As String = String.Empty
        Private _processMailFlag As String = String.Empty
        Private _processExternal1 As String = String.Empty
        Private _processExternal2 As String = String.Empty
        Private _processMessageingField As String = String.Empty
        Private _processBoolean1 As String = String.Empty
        Private _processBoolean2 As String = String.Empty
        Private _processBoolean3 As String = String.Empty
        Private _processBoolean4 As String = String.Empty
        Private _processBoolean5 As String = String.Empty
        Private _processID As String = String.Empty
        Private _processRestrictedPaymentTypes As String = String.Empty
        Private _processAttributes As String = String.Empty
        Private _processLoyaltyPoints As String = String.Empty
        Private _processIsLockedOut As String = String.Empty
        Private _processCustomerPurchaseHistory As String = String.Empty

        Public Property ProcessTitle() As String
            Get
                Return _processTitle
            End Get
            Set(ByVal value As String)
                _processTitle = value
            End Set
        End Property

        Public Property ProcessInitials() As String
            Get
                Return _processInitials
            End Get
            Set(ByVal value As String)
                _processInitials = value
            End Set
        End Property

        Public Property ProcessForeName() As String
            Get
                Return _processForeName
            End Get
            Set(ByVal value As String)
                _processForeName = value
            End Set
        End Property

        Public Property ProcessSurname() As String
            Get
                Return _processSurname
            End Get
            Set(ByVal value As String)
                _processSurname = value
            End Set
        End Property

        Public Property ProcessFullName() As String
            Get
                Return _processFullName
            End Get
            Set(ByVal value As String)
                _processFullName = value
            End Set
        End Property

        Public Property ProcessSalutation() As String
            Get
                Return _processSalutation
            End Get
            Set(ByVal value As String)
                _processSalutation = value
            End Set
        End Property

        Public Property ProcessMothersName() As String
            Get
                Return _processMothersName
            End Get
            Set(ByVal value As String)
                _processMothersName = value
            End Set
        End Property

        Public Property ProcessFathersName() As String
            Get
                Return _processFathersName
            End Get
            Set(ByVal value As String)
                _processFathersName = value
            End Set
        End Property

        Public Property ProcessEmailAddress() As String
            Get
                Return _processEmailAddress
            End Get
            Set(ByVal value As String)
                _processEmailAddress = value
            End Set
        End Property

        Public Property ProcessLoginID() As String
            Get
                Return _processLoginID
            End Get
            Set(ByVal value As String)
                _processLoginID = value
            End Set
        End Property

        Public Property ProcessPassword() As String
            Get
                Return _processPassword
            End Get
            Set(ByVal value As String)
                _processPassword = value
            End Set
        End Property

        Public Property ProcessAccountNumber1() As String
            Get
                Return _processAccountNumber1
            End Get
            Set(ByVal value As String)
                _processAccountNumber1 = value
            End Set
        End Property

        Public Property ProcessAccountNumber2() As String
            Get
                Return _processAccountNumber2
            End Get
            Set(ByVal value As String)
                _processAccountNumber2 = value
            End Set
        End Property

        Public Property ProcessAccountNumber3() As String
            Get
                Return _processAccountNumber3
            End Get
            Set(ByVal value As String)
                _processAccountNumber3 = value
            End Set
        End Property

        Public Property ProcessAccountNumber4() As String
            Get
                Return _processAccountNumber4
            End Get
            Set(ByVal value As String)
                _processAccountNumber4 = value
            End Set
        End Property

        Public Property ProcessAccountNumber5() As String
            Get
                Return _processAccountNumber5
            End Get
            Set(ByVal value As String)
                _processAccountNumber5 = value
            End Set
        End Property

        Public Property ProcessAddress() As String
            Get
                Return _processAddress
            End Get
            Set(ByVal value As String)
                _processAddress = value
            End Set
        End Property

        Public Property ProcessPosition() As String
            Get
                Return _processPosition
            End Get
            Set(ByVal value As String)
                _processPosition = value
            End Set
        End Property

        Public Property ProcessGender() As String
            Get
                Return _processGender
            End Get
            Set(ByVal value As String)
                _processGender = value
            End Set
        End Property

        Public Property ProcessTelephoneNumber1() As String
            Get
                Return _processTelephoneNumber1
            End Get
            Set(ByVal value As String)
                _processTelephoneNumber1 = value
            End Set
        End Property

        Public Property ProcessTelephoneNumber2() As String
            Get
                Return _processTelephoneNumber2
            End Get
            Set(ByVal value As String)
                _processTelephoneNumber2 = value
            End Set
        End Property

        Public Property ProcessTelephoneNumber3() As String
            Get
                Return _processTelephoneNumber3
            End Get
            Set(ByVal value As String)
                _processTelephoneNumber3 = value
            End Set
        End Property

        Public Property ProcessTelephoneNumber4() As String
            Get
                Return _processTelephoneNumber4
            End Get
            Set(ByVal value As String)
                _processTelephoneNumber4 = value
            End Set
        End Property

        Public Property ProcessTelephoneNumber5() As String
            Get
                Return _processTelephoneNumber5
            End Get
            Set(ByVal value As String)
                _processTelephoneNumber5 = value
            End Set
        End Property

        Public Property ProcessDateOfBirth() As String
            Get
                Return _processDateOfBirth
            End Get
            Set(ByVal value As String)
                _processDateOfBirth = value
            End Set
        End Property

        Public Property ProcessContactViaEmail() As String
            Get
                Return _processContactViaEmail
            End Get
            Set(ByVal value As String)
                _processContactViaEmail = value
            End Set
        End Property
        Public Property ProcessContactViaMail() As String
            Get
                Return _processContactViaMail
            End Get
            Set(ByVal value As String)
                _processContactViaMail = value
            End Set
        End Property

        Public Property ProcessHTMLNewsletter() As String
            Get
                Return _processHTMLNewsLetter
            End Get
            Set(ByVal value As String)
                _processHTMLNewsLetter = value
            End Set
        End Property

        Public Property ProcessSubscription1() As String
            Get
                Return _processSubscription1
            End Get
            Set(ByVal value As String)
                _processSubscription1 = value
            End Set
        End Property

        Public Property ProcessSubscription2() As String
            Get
                Return _processSubscription2
            End Get
            Set(ByVal value As String)
                _processSubscription2 = value
            End Set
        End Property

        Public Property ProcessSubscription3() As String
            Get
                Return _processSubscription3
            End Get
            Set(ByVal value As String)
                _processSubscription3 = value
            End Set
        End Property

        Public Property ProcessMailFlag() As String
            Get
                Return _processMailFlag
            End Get
            Set(ByVal value As String)
                _processMailFlag = value
            End Set
        End Property

        Public Property ProcessExternal1() As String
            Get
                Return _processExternal1
            End Get
            Set(ByVal value As String)
                _processExternal1 = value
            End Set
        End Property

        Public Property ProcessExternal2() As String
            Get
                Return _processExternal2
            End Get
            Set(ByVal value As String)
                _processExternal2 = value
            End Set
        End Property

        Public Property ProcessMessagingField() As String
            Get
                Return _processMessageingField
            End Get
            Set(ByVal value As String)
                _processMessageingField = value
            End Set
        End Property

        Public Property ProcessBoolean1() As String
            Get
                Return _processBoolean1
            End Get
            Set(ByVal value As String)
                _processBoolean1 = value
            End Set
        End Property

        Public Property ProcessBoolean2() As String
            Get
                Return _processBoolean2
            End Get
            Set(ByVal value As String)
                _processBoolean2 = value
            End Set
        End Property

        Public Property ProcessBoolean3() As String
            Get
                Return _processBoolean3
            End Get
            Set(ByVal value As String)
                _processBoolean3 = value
            End Set
        End Property

        Public Property ProcessBoolean4() As String
            Get
                Return _processBoolean4
            End Get
            Set(ByVal value As String)
                _processBoolean4 = value
            End Set
        End Property

        Public Property ProcessBoolean5() As String
            Get
                Return _processBoolean5
            End Get
            Set(ByVal value As String)
                _processBoolean5 = value
            End Set
        End Property

        Public Property ProcessID() As String
            Get
                Return _processID
            End Get
            Set(ByVal value As String)
                _processID = value
            End Set
        End Property

        Public Property ProcessRestrictedPaymentTypes() As String
            Get
                Return _processRestrictedPaymentTypes
            End Get
            Set(ByVal value As String)
                _processRestrictedPaymentTypes = value
            End Set
        End Property

        Public Property ProcessAttributes() As String
            Get
                Return _processAttributes
            End Get
            Set(ByVal value As String)
                _processAttributes = value
            End Set
        End Property

        Public Property ProcessLoyaltyPoints() As String
            Get
                Return _processLoyaltyPoints
            End Get
            Set(ByVal value As String)
                _processLoyaltyPoints = value
            End Set
        End Property

        Public Property ProcessIsLockedOut() As String
            Get
                Return _processIsLockedOut
            End Get
            Set(ByVal value As String)
                _processIsLockedOut = value
            End Set
        End Property

        Public Property ProcessCustomerPurchaseHistory() As String
            Get
                Return _processCustomerPurchaseHistory
            End Get
            Set(ByVal value As String)
                _processCustomerPurchaseHistory = value
            End Set
        End Property

        Private _UpdateMode As String
        Public Property UpdateMode() As String
            Get
                Return _UpdateMode
            End Get
            Set(ByVal value As String)
                _UpdateMode = value
            End Set
        End Property

        Private _title As String
        Public Property Title() As String
            Get
                Return _title
            End Get
            Set(ByVal value As String)
                _title = value
            End Set
        End Property

        Private _initials As String
        Public Property Initials() As String
            Get
                Return _initials
            End Get
            Set(ByVal value As String)
                _initials = value
            End Set
        End Property

        Private _forename As String
        Public Property Forename() As String
            Get
                Return _forename
            End Get
            Set(ByVal value As String)
                _forename = value
            End Set
        End Property

        Private _surname As String
        Public Property Surname() As String
            Get
                Return _surname
            End Get
            Set(ByVal value As String)
                _surname = value
            End Set
        End Property

        Private _fullname As String
        Public Property FullName() As String
            Get
                Return _fullname
            End Get
            Set(ByVal value As String)
                _fullname = value
            End Set
        End Property

        Private _salutation As String
        Public Property Salutation() As String
            Get
                Return _salutation
            End Get
            Set(ByVal value As String)
                _salutation = value
            End Set
        End Property

        Private _mothersName As String
        Public Property MothersName() As String
            Get
                Return _mothersName
            End Get
            Set(ByVal value As String)
                _mothersName = value
            End Set
        End Property

        Private _fathersName As String
        Public Property FathersName() As String
            Get
                Return _fathersName
            End Get
            Set(ByVal value As String)
                _fathersName = value
            End Set
        End Property

        Private _email As String
        Public Property EmailAddress() As String
            Get
                Return _email
            End Get
            Set(ByVal value As String)
                _email = value
            End Set
        End Property

        Private _loginID As String
        Public Property LoginID() As String
            Get
                Return _loginID
            End Get
            Set(ByVal value As String)
                _loginID = value
            End Set
        End Property

        Private _password As String
        Public Property Password() As String
            Get
                Return _password
            End Get
            Set(ByVal value As String)
                _password = value
            End Set
        End Property

        Private _acc1 As String
        Public Property AccountNumber1() As String
            Get
                Return _acc1
            End Get
            Set(ByVal value As String)
                _acc1 = value
            End Set
        End Property
        Private _acc2 As String
        Public Property AccountNumber2() As String
            Get
                Return _acc2
            End Get
            Set(ByVal value As String)
                _acc2 = value
            End Set
        End Property
        Private _acc3 As String
        Public Property AccountNumber3() As String
            Get
                Return _acc3
            End Get
            Set(ByVal value As String)
                _acc3 = value
            End Set
        End Property
        Private _acc4 As String
        Public Property AccountNumber4() As String
            Get
                Return _acc4
            End Get
            Set(ByVal value As String)
                _acc4 = value
            End Set
        End Property
        Private _acc5 As String
        Public Property AccountNumber5() As String
            Get
                Return _acc5
            End Get
            Set(ByVal value As String)
                _acc5 = value
            End Set
        End Property


        Private _addresses As Generic.List(Of DECustomerSiteAddress)
        Public Property Addresses() As Generic.List(Of DECustomerSiteAddress)
            Get
                Return _addresses
            End Get
            Set(ByVal value As Generic.List(Of DECustomerSiteAddress))
                _addresses = value
            End Set
        End Property

        Private _position As String
        Public Property Position() As String
            Get
                Return _position
            End Get
            Set(ByVal value As String)
                _position = value
            End Set
        End Property

        Private _gender As String
        Public Property Gender() As String
            Get
                Return _gender
            End Get
            Set(ByVal value As String)
                _gender = value
            End Set
        End Property
        Private _tel1 As String
        Public Property TelephoneNumber1() As String
            Get
                Return _tel1
            End Get
            Set(ByVal value As String)
                _tel1 = value
            End Set
        End Property
        Private _tel2 As String
        Public Property TelephoneNumber2() As String
            Get
                Return _tel2
            End Get
            Set(ByVal value As String)
                _tel2 = value
            End Set
        End Property
        Private _tel3 As String
        Public Property TelephoneNumber3() As String
            Get
                Return _tel3
            End Get
            Set(ByVal value As String)
                _tel3 = value
            End Set
        End Property
        Private _tel4 As String
        Public Property TelephoneNumber4() As String
            Get
                Return _tel4
            End Get
            Set(ByVal value As String)
                _tel4 = value
            End Set
        End Property
        Private _tel5 As String
        Public Property TelephoneNumber5() As String
            Get
                Return _tel5
            End Get
            Set(ByVal value As String)
                _tel5 = value
            End Set
        End Property


        Private _dob As Date
        Public Property DateOfBirth() As Date
            Get
                Return _dob
            End Get
            Set(ByVal value As Date)
                _dob = value
            End Set
        End Property

        Private _contactViaEmail As Boolean
        Public Property ContactViaEmail() As Boolean
            Get
                Return _contactViaEmail
            End Get
            Set(ByVal value As Boolean)
                _contactViaEmail = value
            End Set
        End Property
        Private _contactViaMail As Boolean
        Public Property ContactViaMail() As Boolean
            Get
                Return _contactViaMail
            End Get
            Set(ByVal value As Boolean)
                _contactViaMail = value
            End Set
        End Property

        Private _subs1 As Boolean
        Public Property Subscription1() As Boolean
            Get
                Return _subs1
            End Get
            Set(ByVal value As Boolean)
                _subs1 = value
            End Set
        End Property

        Private _htmlNewsletter As Boolean
        Public Property HTMLNewsletter() As Boolean
            Get
                Return _htmlNewsletter
            End Get
            Set(ByVal value As Boolean)
                _htmlNewsletter = value
            End Set
        End Property

        Private _subs2 As Boolean
        Public Property Subscription2() As Boolean
            Get
                Return _subs2
            End Get
            Set(ByVal value As Boolean)
                _subs2 = value
            End Set
        End Property
        Private _subs3 As Boolean
        Public Property Subscription3() As Boolean
            Get
                Return _subs3
            End Get
            Set(ByVal value As Boolean)
                _subs3 = value
            End Set
        End Property

        Private _mailFlag1 As Boolean
        Public Property MailFlag1() As Boolean
            Get
                Return _mailFlag1
            End Get
            Set(ByVal value As Boolean)
                _mailFlag1 = value
            End Set
        End Property
        Private _extId1 As String
        Public Property ExternalId1() As String
            Get
                Return _extId1
            End Get
            Set(ByVal value As String)
                _extId1 = value
            End Set
        End Property

        Private _extId2 As String
        Public Property ExternalId2() As String
            Get
                Return _extId2
            End Get
            Set(ByVal value As String)
                _extId2 = value
            End Set
        End Property

        Private _messagingID As String
        Public Property MessagingID() As String
            Get
                Return _messagingID
            End Get
            Set(ByVal value As String)
                _messagingID = value
            End Set
        End Property
        Private _bool1 As Boolean
        Public Property Boolean1() As Boolean
            Get
                Return _bool1
            End Get
            Set(ByVal value As Boolean)
                _bool1 = value
            End Set
        End Property
        Private _bool2 As Boolean
        Public Property Boolean2() As Boolean
            Get
                Return _bool2
            End Get
            Set(ByVal value As Boolean)
                _bool2 = value
            End Set
        End Property
        Private _bool3 As Boolean
        Public Property Boolean3() As Boolean
            Get
                Return _bool3
            End Get
            Set(ByVal value As Boolean)
                _bool3 = value
            End Set
        End Property
        Private _bool4 As Boolean
        Public Property Boolean4() As Boolean
            Get
                Return _bool4
            End Get
            Set(ByVal value As Boolean)
                _bool4 = value
            End Set
        End Property
        Private _bool5 As Boolean
        Public Property Boolean5() As Boolean
            Get
                Return _bool5
            End Get
            Set(ByVal value As Boolean)
                _bool5 = value
            End Set
        End Property

        Private _id As String
        Public Property ID() As String
            Get
                Return _id
            End Get
            Set(ByVal value As String)
                _id = value
            End Set
        End Property


        Private _restrictedPaymentTypes As Generic.List(Of DECustomerSiteRestrictedPaymentType)
        Public Property RestrictedPaymentTypes() As Generic.List(Of DECustomerSiteRestrictedPaymentType)
            Get
                Return _restrictedPaymentTypes
            End Get
            Set(ByVal value As Generic.List(Of DECustomerSiteRestrictedPaymentType))
                _restrictedPaymentTypes = value
            End Set
        End Property

        Private _attributes As Generic.List(Of DECustomerSiteAttributes)
        Public Property Attributes() As Generic.List(Of DECustomerSiteAttributes)
            Get
                Return _attributes
            End Get
            Set(ByVal value As Generic.List(Of DECustomerSiteAttributes))
                _attributes = value
            End Set
        End Property

        Private _loyaltyPoints As String
        Public Property LoyaltyPoints() As String
            Get
                Return _loyaltyPoints
            End Get
            Set(ByVal value As String)
                _loyaltyPoints = value
            End Set
        End Property

        Private _isLockedOut As Boolean
        Public Property IsLockedOut() As Boolean
            Get
                Return _isLockedOut
            End Get
            Set(ByVal value As Boolean)
                _isLockedOut = value
            End Set
        End Property

        Private _custPurchaseHistory As Boolean
        Public Property CustomerPurchaseHistory() As Boolean
            Get
                Return _custPurchaseHistory
            End Get
            Set(ByVal value As Boolean)
                _custPurchaseHistory = value
            End Set
        End Property

        Public Property ContactSource As String
        Public Property ContactSuffix As String
        Public Property ContactNickname As String
        Public Property ContactUsername As String
        Public Property ContactSLAccount As String
        Public Property MinimalRegistration As Boolean = False

        Sub New()
            MyBase.New()
            Addresses = New Generic.List(Of DECustomerSiteAddress)
            RestrictedPaymentTypes = New Generic.List(Of DECustomerSiteRestrictedPaymentType)
            Attributes = New Generic.List(Of DECustomerSiteAttributes)
        End Sub


    End Class
    <Serializable()> _
    Class DECustomerSiteAttributes

        Private _UpdateMode As String
        Public Property UpdateMode() As String
            Get
                Return _UpdateMode
            End Get
            Set(ByVal value As String)
                _UpdateMode = value
            End Set
        End Property

        Private _attribute As String
        Public Property Attribute() As String
            Get
                Return _attribute
            End Get
            Set(ByVal value As String)
                _attribute = value
            End Set
        End Property

    End Class
    <Serializable()> _
    Class DECustomerSiteRestrictedPaymentType

        Private _UpdateMode As String
        Public Property UpdateMode() As String
            Get
                Return _UpdateMode
            End Get
            Set(ByVal value As String)
                _UpdateMode = value
            End Set
        End Property

        Private _PaymentType As String
        Public Property PaymentType() As String
            Get
                Return _PaymentType
            End Get
            Set(ByVal value As String)
                _PaymentType = value
            End Set
        End Property

    End Class

    <Serializable()> _
    Class DECustomerSiteAddress

        Private _UpdateMode As String
        Public Property UpdateMode() As String
            Get
                Return _UpdateMode
            End Get
            Set(ByVal value As String)
                _UpdateMode = value
            End Set
        End Property

        Private _seq As Integer
        Public Property SequenceNumber() As Integer
            Get
                Return _seq
            End Get
            Set(ByVal value As Integer)
                _seq = value
            End Set
        End Property

        Private _default As Boolean
        Public Property IsDefault() As Boolean
            Get
                Return _default
            End Get
            Set(ByVal value As Boolean)
                _default = value
            End Set
        End Property


        Private _line1 As String
        Public Property Line1() As String
            Get
                Return _line1
            End Get
            Set(ByVal value As String)
                _line1 = value
            End Set
        End Property
        Private _line2 As String
        Public Property Line2() As String
            Get
                Return _line2
            End Get
            Set(ByVal value As String)
                _line2 = value
            End Set
        End Property
        Private _line3 As String
        Public Property Line3() As String
            Get
                Return _line3
            End Get
            Set(ByVal value As String)
                _line3 = value
            End Set
        End Property
        Private _line4 As String
        Public Property Line4() As String
            Get
                Return _line4
            End Get
            Set(ByVal value As String)
                _line4 = value
            End Set
        End Property
        Private _line5 As String
        Public Property Line5() As String
            Get
                Return _line5
            End Get
            Set(ByVal value As String)
                _line5 = value
            End Set
        End Property

        Private _postcode As String
        Public Property Postcode() As String
            Get
                Return _postcode
            End Get
            Set(ByVal value As String)
                _postcode = value
            End Set
        End Property

        Private _country As String
        Public Property Country() As String
            Get
                Return _country
            End Get
            Set(ByVal value As String)
                _country = value
            End Set
        End Property

        Private _processSequenceNumber As String = String.Empty
        Private _processDefault As String = String.Empty
        Private _processReference As String = String.Empty
        Private _processLine1 As String = String.Empty
        Private _processLine2 As String = String.Empty
        Private _processLine3 As String = String.Empty
        Private _processLine4 As String = String.Empty
        Private _processLine5 As String = String.Empty
        Private _processPostCode As String = String.Empty
        Private _processCountry As String = String.Empty

        Public Property ProcessSequenceNumber() As String
            Get
                Return _processSequenceNumber
            End Get
            Set(ByVal value As String)
                _processSequenceNumber = value
            End Set
        End Property

        Public Property ProcessDefault() As String
            Get
                Return _processDefault
            End Get
            Set(ByVal value As String)
                _processDefault = value
            End Set
        End Property

        Public Property ProcessReference() As String
            Get
                Return _processReference
            End Get
            Set(ByVal value As String)
                _processReference = value
            End Set
        End Property

        Public Property ProcessLine1() As String
            Get
                Return _processLine1
            End Get
            Set(ByVal value As String)
                _processLine1 = value
            End Set
        End Property

        Public Property ProcessLine2() As String
            Get
                Return _processLine2
            End Get
            Set(ByVal value As String)
                _processLine2 = value
            End Set
        End Property


        Public Property ProcessLine3() As String
            Get
                Return _processLine3
            End Get
            Set(ByVal value As String)
                _processLine3 = value
            End Set
        End Property


        Public Property ProcessLine4() As String
            Get
                Return _processLine4
            End Get
            Set(ByVal value As String)
                _processLine4 = value
            End Set
        End Property

        Public Property ProcessLine5() As String
            Get
                Return _processLine5
            End Get
            Set(ByVal value As String)
                _processLine5 = value
            End Set
        End Property

        Public Property ProcessPostCode() As String
            Get
                Return _processPostCode
            End Get
            Set(ByVal value As String)
                _processPostCode = value
            End Set
        End Property

        Public Property ProcessCountry() As String
            Get
                Return _processCountry
            End Get
            Set(ByVal value As String)
                _processCountry = value
            End Set
        End Property
    End Class

End Class

