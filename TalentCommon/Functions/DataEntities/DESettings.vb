'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with generic settings
'
'       Date                        7th Nov 2006
'
'       Author                      Andy White
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDESE- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DESettings

    Private _accountNo1 As String = String.Empty
    Private _accountNo2 As String = String.Empty
    Private _accountNo3 As String = String.Empty
    Private _accountNo4 As String = String.Empty
    Private _accountNo5 As String = String.Empty
    Private _authorised As Boolean
    Private _backOfficeConnectionString As String = String.Empty
    Private _cacheing As Boolean
    Private _cacheTimeMinutes As Integer = 0
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

    Private _IgnoreErrors As String
    Public Property IgnoreErrors() As String
        Get
            Return _IgnoreErrors
        End Get
        Set(ByVal value As String)
            _IgnoreErrors = value
        End Set
    End Property

End Class
