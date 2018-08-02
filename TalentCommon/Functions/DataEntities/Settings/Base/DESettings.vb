'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with generic settings
'
'       Date                        7th Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDESE- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DESettings

    Private _destinationType As String
    Public Property DestinationType() As String
        Get
            Return _destinationType
        End Get
        Set(ByVal value As String)
            _destinationType = value
        End Set
    End Property

    Private _accountNo1 As String = String.Empty
    Private _accountNo2 As String = String.Empty
    Private _accountNo3 As String = String.Empty
    Private _accountNo4 As String = String.Empty
    Private _accountNo5 As String = String.Empty
    Private _authorised As Boolean
    Private _backOfficeConnectionString As String = String.Empty
    Private _cacheing As Boolean
    Private _cacheTimeMinutes As Integer = 0
    Private _cacheTimeMinutesSecondFunction As Integer = 0
    Private _cacheTimeMinutesThirdFunction As Integer = 0
    Private _cacheTimeMinutesFourthFunction As Integer = 0
    Private _cacheTimeMinutesFifthFunction As Integer = 0
    Private _cMSUseStoredProcedures As Boolean = False
    Private _company As String = String.Empty
    Private _databaseType1 As String = String.Empty
    Private _destinationDatabase As String = String.Empty
    Private _frontEndConnectionString As String = String.Empty
    Private _language As String = String.Empty             ' 
    Private _senderID As String = String.Empty
    Private _webServiceName As String = String.Empty
    Private _retryFailures As Boolean = False
    Private _retryAttempts As Integer = 0
    Private _retryWaitTime As Int32 = 0
    Private _retryErrorNumbers As String = String.Empty
    Private _storedProcedureGroup As String = String.Empty
    Private _moduleName As String = String.Empty
    Private _partner As String = String.Empty
    Private _originatingSource As String = String.Empty
    Private _originatingSourceCode As String = String.Empty
    Private _stadium As String = String.Empty
    Private _logRequests As Boolean = False
    Private _responseDirectory As String = String.Empty
    Private _transactionID As String = String.Empty
    Private _RequestCount As Integer = 0
    Private _supplyNetRequestName As String = String.Empty
    'AUTHORITY_USER_PROFILE from ecommercemoduledefaults
    Private _authorityUserProfile As String = String.Empty
    Private _logging As New TalentLogging
    Private _connectionStringList As Generic.List(Of String)

    Private _bu As String = String.Empty
    Public Property BusinessUnit() As String
        Get
            Return _bu
        End Get
        Set(ByVal value As String)
            _bu = value
        End Set
    End Property

    Private _buNew As String
    Public Property BusinessUnitNew() As String
        Get
            Return _buNew
        End Get
        Set(ByVal value As String)
            _buNew = value
        End Set
    End Property

    Public Property ConnectionStringList() As Generic.List(Of String)
        Get
            Return _connectionStringList
        End Get
        Set(ByVal value As Generic.List(Of String))
            _connectionStringList = value
        End Set
    End Property
    Public Property AccountNo1() As String
        Get
            Return _accountNo1
        End Get
        Set(ByVal value As String)
            _accountNo1 = value
        End Set
    End Property
    Public Property AccountNo2() As String
        Get
            Return _accountNo2
        End Get
        Set(ByVal value As String)
            _accountNo2 = value
        End Set
    End Property
    Public Property AccountNo3() As String
        Get
            Return _accountNo3
        End Get
        Set(ByVal value As String)
            _accountNo3 = value
        End Set
    End Property
    Public Property AccountNo4() As String
        Get
            Return _accountNo4
        End Get
        Set(ByVal value As String)
            _accountNo4 = value
        End Set
    End Property
    Public Property AccountNo5() As String
        Get
            Return _accountNo5
        End Get
        Set(ByVal value As String)
            _accountNo5 = value
        End Set
    End Property
    Public Property Authorised() As Boolean
        Get
            Return _authorised
        End Get
        Set(ByVal value As Boolean)
            _authorised = value
        End Set
    End Property
    Public Property BackOfficeConnectionString() As String
        Get
            Return _backOfficeConnectionString
        End Get
        Set(ByVal value As String)
            _backOfficeConnectionString = value
        End Set
    End Property
    Public Property Cacheing() As Boolean
        Get
            Return _cacheing
        End Get
        Set(ByVal value As Boolean)
            _cacheing = value
        End Set
    End Property
    Public Property CacheTimeMinutes() As Integer
        Get
            Return _cacheTimeMinutes
        End Get
        Set(ByVal value As Integer)
            _cacheTimeMinutes = value
        End Set
    End Property
    Public Property CacheTimeSeconds() As Integer
    Public Property CacheTimeMinutesSecondFunction() As Integer
        Get
            Return _cacheTimeMinutesSecondFunction
        End Get
        Set(ByVal value As Integer)
            _cacheTimeMinutesSecondFunction = value
        End Set
    End Property
    Public Property CacheTimeMinutesThirdFunction() As Integer
        Get
            Return _cacheTimeMinutesThirdFunction
        End Get
        Set(ByVal value As Integer)
            _cacheTimeMinutesThirdFunction = value
        End Set
    End Property
    Public Property CacheTimeMinutesFourthFunction() As Integer
        Get
            Return _cacheTimeMinutesFourthFunction
        End Get
        Set(ByVal value As Integer)
            _cacheTimeMinutesFourthFunction = value
        End Set
    End Property
    Public Property CacheTimeMinutesFifthFunction() As Integer
        Get
            Return _cacheTimeMinutesFifthFunction
        End Get
        Set(ByVal value As Integer)
            _cacheTimeMinutesFifthFunction = value
        End Set
    End Property
    Public Property Company() As String
        Get
            Return _company
        End Get
        Set(ByVal value As String)
            _company = value
        End Set
    End Property
    Public Property CMSUseStoredProcedures() As Boolean
        Get
            Return _cMSUseStoredProcedures
        End Get
        Set(ByVal value As Boolean)
            _cMSUseStoredProcedures = value
        End Set
    End Property
    Public Property DatabaseType1() As String
        Get
            Return _databaseType1
        End Get
        Set(ByVal value As String)
            _databaseType1 = value
        End Set
    End Property
    Public Property DestinationDatabase() As String
        Get
            Return _destinationDatabase
        End Get
        Set(ByVal value As String)
            _destinationDatabase = value
        End Set
    End Property
    Public Property FrontEndConnectionString() As String
        Get
            Return _frontEndConnectionString
        End Get
        Set(ByVal value As String)
            _frontEndConnectionString = value
            _logging.FrontEndConnectionString = value
        End Set
    End Property
    Public Property Language() As String
        Get
            Return _language
        End Get
        Set(ByVal value As String)
            _language = value
        End Set
    End Property                ' 
    Public Property SenderID() As String
        Get
            Return _senderID
        End Get
        Set(ByVal value As String)
            _senderID = value
        End Set
    End Property
    Public Property WebServiceName() As String
        Get
            Return _webServiceName
        End Get
        Set(ByVal value As String)
            _webServiceName = value
        End Set
    End Property
    Public Property RetryFailures() As String
        Get
            Return _retryFailures
        End Get
        Set(ByVal value As String)
            _retryFailures = value
        End Set
    End Property
    Public Property RetryAttempts() As String
        Get
            Return _retryAttempts
        End Get
        Set(ByVal value As String)
            _retryAttempts = value
        End Set
    End Property
    Public Property RetryWaitTime() As String
        Get
            Return _retryWaitTime
        End Get
        Set(ByVal value As String)
            _retryWaitTime = value
        End Set
    End Property
    Public Property RetryErrorNumbers() As String
        Get
            Return _retryErrorNumbers
        End Get
        Set(ByVal value As String)
            _retryErrorNumbers = value
        End Set
    End Property
    Public Property StoredProcedureGroup() As String
        Get
            Return _storedProcedureGroup
        End Get
        Set(ByVal value As String)
            _storedProcedureGroup = value
        End Set
    End Property
    Public Property ModuleName() As String
        Get
            Return _moduleName
        End Get
        Set(ByVal value As String)
            _moduleName = value
        End Set
    End Property

    Public Property Partner() As String
        Get
            Return _partner
        End Get
        Set(ByVal value As String)
            _partner = value
        End Set
    End Property

    Private _IgnoreErrors As String
    Public Property IgnoreErrors() As String
        Get
            Return _IgnoreErrors
        End Get
        Set(ByVal value As String)
            _IgnoreErrors = value
        End Set
    End Property

    Private _LookUpType1 As String
    Public Property LookUpType1() As String
        Get
            Return _LookUpType1
        End Get
        Set(ByVal value As String)
            _LookUpType1 = value
        End Set
    End Property

    Private _LookUpType2 As String
    Public Property LookUpType2() As String
        Get
            Return _LookUpType2
        End Get
        Set(ByVal value As String)
            _LookUpType2 = value
        End Set
    End Property

    Private _LookUpType3 As String
    Public Property LookUpType3() As String
        Get
            Return _LookUpType3
        End Get
        Set(ByVal value As String)
            _LookUpType3 = value
        End Set
    End Property

    Private _LookUpType4 As String
    Public Property LookUpType4() As String
        Get
            Return _LookUpType4
        End Get
        Set(ByVal value As String)
            _LookUpType4 = value
        End Set
    End Property

    Private _LookUpType5 As String
    Public Property LookUpType5() As String
        Get
            Return _LookUpType5
        End Get
        Set(ByVal value As String)
            _LookUpType5 = value
        End Set
    End Property

    Private _loginId As String = String.Empty
    Public Property LoginId() As String
        Get
            Return _loginId
        End Get
        Set(ByVal value As String)
            _loginId = value
        End Set
    End Property

    Private _cacheDependencyPath As String
    Public Property CacheDependencyPath() As String
        Get
            Return _cacheDependencyPath
        End Get
        Set(ByVal value As String)
            _cacheDependencyPath = value
        End Set
    End Property

    Private _cacheStringExtension As String = String.Empty
    Public Property CacheStringExtension() As String
        Get
            Return _cacheStringExtension
        End Get
        Set(ByVal value As String)
            _cacheStringExtension = value
        End Set
    End Property

    Private _ticketingKioskMode As Boolean = False
    Public Property TicketingKioskMode() As Boolean
        Get
            Return _ticketingKioskMode
        End Get
        Set(ByVal value As Boolean)
            _ticketingKioskMode = value
        End Set
    End Property

    Public Property OriginatingSource() As String
        Get
            Return _originatingSource
        End Get
        Set(ByVal value As String)
            _originatingSource = value
        End Set
    End Property

    Public Property OriginatingSourceCode() As String
        Get
            Return _originatingSourceCode
        End Get
        Set(ByVal value As String)
            _originatingSourceCode = value
        End Set
    End Property

    Public Property Stadium() As String
        Get
            Return _stadium
        End Get
        Set(ByVal value As String)
            _stadium = value
        End Set
    End Property

    Private _xmlSettings As DEXmlSettings
    Public Property XmlSettings() As DEXmlSettings
        Get
            Return _xmlSettings
        End Get
        Set(ByVal value As DEXmlSettings)
            _xmlSettings = value
        End Set
    End Property

    Private _AutoAddMembership As Boolean
    Public Property AutoAddMembership() As Boolean
        Get
            Return _AutoAddMembership
        End Get
        Set(ByVal value As Boolean)
            _AutoAddMembership = value
        End Set
    End Property

    Public Property LogRequests() As Boolean
        Get
            Return _logRequests
        End Get
        Set(ByVal value As Boolean)
            _logRequests = value
        End Set
    End Property

    Public Property ResponseDirectory() As String
        Get
            Return _responseDirectory
        End Get
        Set(ByVal value As String)
            _responseDirectory = value
        End Set
    End Property

    Public Property TransactionID() As String
        Get
            Return _transactionID
        End Get
        Set(ByVal value As String)
            _transactionID = value
        End Set
    End Property

    Public Property RequestCount() As Integer
        Get
            Return _RequestCount
        End Get
        Set(ByVal value As Integer)
            _RequestCount = value
        End Set
    End Property

    Public Property SupplyNetRequestName() As String
        Get
            Return _supplyNetRequestName
        End Get
        Set(ByVal value As String)
            _supplyNetRequestName = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the authority user profile 
    ''' which used to decide the display of home game in away product
    ''' </summary>
    ''' <value>The authority user profile.</value>
    Public Property AuthorityUserProfile() As String
        Get
            Return _authorityUserProfile
        End Get
        Set(ByVal value As String)
            _authorityUserProfile = value
        End Set
    End Property

    Public Property Logging() As TalentLogging
        Get
            Return _logging
        End Get
        Set(ByVal value As TalentLogging)
            _logging = value
        End Set
    End Property

    Private _performWatchListCheck As Boolean
    Public Property PerformWatchListCheck() As Boolean
        Get
            Return _performWatchListCheck
        End Get
        Set(ByVal value As Boolean)
            _performWatchListCheck = value
        End Set
    End Property

    Private _agentType As String
    ''' <summary>
    ''' deprecated: Don't use this instead of use AgentProfile.AgentType.
    ''' </summary>
    ''' <value>
    ''' The type of the agent.
    ''' </value>
    Public Property AgentType() As String
        Get
            Return _agentType
        End Get
        Set(ByVal value As String)
            _agentType = value
        End Set
    End Property

    Private _agentEntity As DEAgent = Nothing
    Public Property AgentEntity() As DEAgent
        Get
            Return _agentEntity
        End Get
        Set(ByVal value As DEAgent)
            _agentEntity = value
        End Set
    End Property
    Public Property IsAgent As Boolean = False
    Public Property EcommerceModuleDefaultsValues As DEEcommerceModuleDefaults.DefaultValues = Nothing
    Public Property CurrentPageName() As String = String.Empty
    Public Property CanProcessFeesParallely() As Boolean = False
    Public Property DeliveryCountryCode() As String = String.Empty
    Sub New()
        MyBase.New()
        Me.XmlSettings = New DEXmlSettings
    End Sub

End Class

'XMLSettings Class

<Serializable()> _
Public Class DEXmlSettings

    Private _destType As String
    Public Property DestinationType() As String
        Get
            Return _destType
        End Get
        Set(ByVal value As String)
            _destType = value
        End Set
    End Property

    Private _xmlVersion As String
    Public Property XmlVersion() As String
        Get
            Return _xmlVersion
        End Get
        Set(ByVal value As String)
            _xmlVersion = value
        End Set
    End Property

    Private _inputXmlLocation As String
    Public Property InputXmlLocation() As String
        Get
            Return _inputXmlLocation
        End Get
        Set(ByVal value As String)
            _inputXmlLocation = value
        End Set
    End Property

    Private _validateXsd As Boolean
    Public Property ValidateXsd() As Boolean
        Get
            Return _validateXsd
        End Get
        Set(ByVal value As Boolean)
            _validateXsd = value
        End Set
    End Property

    Private _validateXsdLocation As String
    Public Property ValidateXsdLocation() As String
        Get
            Return _validateXsdLocation
        End Get
        Set(ByVal value As String)
            _validateXsdLocation = value
        End Set
    End Property

    Private _storeXml As Boolean
    Public Property StoreXml() As Boolean
        Get
            Return _storeXml
        End Get
        Set(ByVal value As Boolean)
            _storeXml = value
        End Set
    End Property

    Private _storeXmlLocation As String
    Public Property StoreXmlLocation() As String
        Get
            Return _storeXmlLocation
        End Get
        Set(ByVal value As String)
            _storeXmlLocation = value
        End Set
    End Property

    Private _archivexml As Boolean
    Public Property ArchiveXml() As Boolean
        Get
            Return _archivexml
        End Get
        Set(ByVal value As Boolean)
            _archivexml = value
        End Set
    End Property

    Private _ArchiveXmlLocation As String
    Public Property ArchiveXmlLocation() As String
        Get
            Return _ArchiveXmlLocation
        End Get
        Set(ByVal value As String)
            _ArchiveXmlLocation = value
        End Set
    End Property

    Private _postXml As Boolean
    Public Property PostXml() As Boolean
        Get
            Return _postXml
        End Get
        Set(ByVal value As Boolean)
            _postXml = value
        End Set
    End Property

    Private _postXmlUrl As String
    Public Property PostXmlUrl() As String
        Get
            Return _postXmlUrl
        End Get
        Set(ByVal value As String)
            _postXmlUrl = value
        End Set
    End Property

    Private _emailXmlAttach As Boolean
    Public Property EmailXmlAttach() As Boolean
        Get
            Return _emailXmlAttach
        End Get
        Set(ByVal value As Boolean)
            _emailXmlAttach = value
        End Set
    End Property

    Private _emailXmlContent As Boolean
    Public Property EmailXmlContent() As Boolean
        Get
            Return _emailXmlContent
        End Get
        Set(ByVal value As Boolean)
            _emailXmlContent = value
        End Set
    End Property

    Private _emailxmlRecipient As String
    Public Property EmailXmlRecipient() As String
        Get
            Return _emailxmlRecipient
        End Get
        Set(ByVal value As String)
            _emailxmlRecipient = value
        End Set
    End Property

    Private _username As String
    Public Property UserName() As String
        Get
            Return _username
        End Get
        Set(ByVal value As String)
            _username = value
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

    Private _domainName As String
    Public Property DomainName() As String
        Get
            Return _domainName
        End Get
        Set(ByVal value As String)
            _domainName = value
        End Set
    End Property

    Public Property SessionID As String
End Class
