'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Address Details
'
'       Date                        1st Nov 2006
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDETR- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DeAddress
    '---------------------------------------------------------------------------------
    ' Request Fields
    '
    Private _category As String = String.Empty  ' EndUserInformation, ResellerInformation, invoice, Delivery, Shipping etc. 
    ' 
    Private _attention As String = String.Empty                                     ' Marion Grange
    Private _contactName As String = String.Empty                                   ' Brussel Sprout
    Private _line1 As String = String.Empty                                         ' 27 Our Street
    Private _line2 As String = String.Empty                                         ' JustDown FromPub
    Private _line3 As String = String.Empty                                         ' 
    Private _city As String = String.Empty                                          ' Nantwich
    Private _province As String = String.Empty                                      ' Cheshire
    Private _postalCode As String = String.Empty                                    ' CW6 1TY
    '---------------------------------------------------------------------------------
    ' Response Fields (Currently open to discussion, if to be used)
    '
    Private _addressErrorMessage As String = String.Empty                           ' response info
    Private _addressErrorType As String = String.Empty                              ' response info
    '---------------------------------------------------------------------------------
    ' Spare Fields for future development
    '
    Private _RepName As String = String.Empty                                       ' Jacob Lyons 
    Private _salutation As String = String.Empty                                    ' Mr B Sprout
    Private _department As String = String.Empty                                    ' Production
    Private _companyName As String = String.Empty                                   ' Electron Micro
    Private _houseNumber As String = String.Empty                                   ' 14
    Private _state As String = String.Empty                                         ' CH
    Private _countryCode As String = String.Empty                                   ' UK
    Private _country As String = String.Empty                                       ' England
    Private _zipCode As String = String.Empty                                       ' CH 3456
    Private _phoneNumber As String = String.Empty                                   ' 01606 454321
    Private _extensionNumber As String = String.Empty                               ' 33228
    Private _mobile As String = String.Empty                                        ' 07988 116 345
    Private _faxNumber As String = String.Empty                                     ' 01606 454323
    Private _email As String = String.Empty                                         ' fred@ourCompanyName.co.uk
    Private _notes As String = String.Empty                                         ' See security gaurd first
    Private _vATNumber As String = String.Empty                                     ' 123 4568
    Private _authorizationNumber As String = String.Empty                           ' A-123456
    Private _pricingLevel As String = String.Empty                                  ' P
    Private _suffix As String = String.Empty                                        '
    Private _title As String = String.Empty
    Private _forename As String = String.Empty
    Private _surname As String = String.Empty
    Private _addressingProvider As String = String.Empty                            ' e.g. Hopewiser or QAS
    Private _customerNumber As String = String.Empty
    Private _sessionID As String = String.Empty

    '---------------------------------------------------------------------------------
    ' Request Fields
    '
    Public Property Category() As String
        Get
            Return _category
        End Get
        Set(ByVal value As String)
            _category = value
        End Set
    End Property
    Public Property Attention() As String
        Get
            Return _attention
        End Get
        Set(ByVal value As String)
            _attention = value
        End Set
    End Property
    Public Property ContactName() As String
        Get
            Return _contactName
        End Get
        Set(ByVal value As String)
            _contactName = value
        End Set
    End Property
    Public Property Line1() As String
        Get
            Return _line1
        End Get
        Set(ByVal value As String)
            _line1 = value
        End Set
    End Property
    Public Property Line2() As String
        Get
            Return _line2
        End Get
        Set(ByVal value As String)
            _line2 = value
        End Set
    End Property
    Public Property Line3() As String
        Get
            Return _line3
        End Get
        Set(ByVal value As String)
            _line3 = value
        End Set
    End Property
    Public Property City() As String
        Get
            Return _city
        End Get
        Set(ByVal value As String)
            _city = value
        End Set
    End Property
    Public Property PostalCode() As String
        Get
            Return _postalCode
        End Get
        Set(ByVal value As String)
            _postalCode = value
        End Set
    End Property
    '---------------------------------------------------------------------------------
    ' Response Fields (Currently open to discussion, if to be used)
    '
    Public Property AddressErrorMessage() As String
        Get
            Return _addressErrorMessage
        End Get
        Set(ByVal value As String)
            _addressErrorMessage = value
        End Set
    End Property
    Public Property AddressErrorType() As String
        Get
            Return _addressErrorType
        End Get
        Set(ByVal value As String)
            _addressErrorType = value
        End Set
    End Property
    '---------------------------------------------------------------------------------
    ' Spare Fields for future development
    '
    Public Property RepName() As String
        Get
            Return _RepName
        End Get
        Set(ByVal value As String)
            _RepName = value
        End Set
    End Property
    Public Property Salutation() As String
        Get
            Return _salutation
        End Get
        Set(ByVal value As String)
            _salutation = value
        End Set
    End Property
    Public Property Department() As String
        Get
            Return _department
        End Get
        Set(ByVal value As String)
            _department = value
        End Set
    End Property
    Public Property CompanyName() As String
        Get
            Return _companyName
        End Get
        Set(ByVal value As String)
            _companyName = value
        End Set
    End Property
    Public Property HouseNumber() As String
        Get
            Return _houseNumber
        End Get
        Set(ByVal value As String)
            _houseNumber = value
        End Set
    End Property
    Public Property State() As String
        Get
            Return _state
        End Get
        Set(ByVal value As String)
            _state = value
        End Set
    End Property
    Public Property Province() As String
        Get
            Return _province
        End Get
        Set(ByVal value As String)
            _province = value
        End Set
    End Property
    Public Property ZipCode() As String
        Get
            Return _zipCode
        End Get
        Set(ByVal value As String)
            _zipCode = value
        End Set
    End Property
    Public Property Country() As String
        Get
            Return _country
        End Get
        Set(ByVal value As String)
            _country = value
        End Set
    End Property
    Public Property CountryCode() As String
        Get
            Return _countryCode
        End Get
        Set(ByVal value As String)
            _countryCode = value
        End Set
    End Property
    Public Property PhoneNumber() As String
        Get
            Return _phoneNumber
        End Get
        Set(ByVal value As String)
            _phoneNumber = value
        End Set
    End Property
    Public Property ExtensionNumber() As String
        Get
            Return _extensionNumber
        End Get
        Set(ByVal value As String)
            _extensionNumber = value
        End Set
    End Property
    Public Property Mobile() As String
        Get
            Return _mobile
        End Get
        Set(ByVal value As String)
            _mobile = value
        End Set
    End Property
    Public Property FaxNumber() As String
        Get
            Return _faxNumber
        End Get
        Set(ByVal value As String)
            _faxNumber = value
        End Set
    End Property
    Public Property Email() As String
        Get
            Return _email
        End Get
        Set(ByVal value As String)
            _email = value
        End Set
    End Property
    Public Property Notes() As String
        Get
            Return _notes
        End Get
        Set(ByVal value As String)
            _notes = value
        End Set
    End Property
    Public Property VATNumber() As String
        Get
            Return _vATNumber
        End Get
        Set(ByVal value As String)
            _vATNumber = value
        End Set
    End Property
    Public Property AuthorizationNumber() As String
        Get
            Return _authorizationNumber
        End Get
        Set(ByVal value As String)
            _authorizationNumber = value
        End Set
    End Property
    Public Property PricingLevel() As String
        Get
            Return _pricingLevel
        End Get
        Set(ByVal value As String)
            _pricingLevel = value
        End Set
    End Property
    Public Property Suffix() As String
        Get
            Return _suffix
        End Get
        Set(ByVal value As String)
            _suffix = value
        End Set
    End Property

    Public Property Title() As String
        Get
            Return _title
        End Get
        Set(ByVal value As String)
            _title = value
        End Set
    End Property

    Public Property Forename() As String
        Get
            Return _forename
        End Get
        Set(ByVal value As String)
            _forename = value
        End Set
    End Property

    Public Property Surname() As String
        Get
            Return _surname
        End Get
        Set(ByVal value As String)
            _surname = value
        End Set
    End Property

    Public Property AddressingProvider() As String
        Get
            Return _addressingProvider
        End Get
        Set(ByVal value As String)
            _addressingProvider = value
        End Set
    End Property
    Public Property CustomerNumber() As String
        Get
            Return _customerNumber
        End Get
        Set(ByVal value As String)
            _customerNumber = value
        End Set
    End Property
End Class
