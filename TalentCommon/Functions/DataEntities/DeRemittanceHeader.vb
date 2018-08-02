Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Remittance headers
'
'       Date                        Apr 2007
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDERH- 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DeRemittanceHeader
    '---------------------------------------------------------------------------------
    '   Addressing Information 
    '
    Private _remittanceHeader As String = String.Empty
    Private _companyCode As String = String.Empty
    Private _bankAccountNo As String = String.Empty
    Private _sOPorderNo As String = String.Empty
    Private _bankReference As String = String.Empty
    Private _paymentMethod As String = String.Empty
    'Private _postingDate As Date
    Private _postingDate As String = String.Empty
    Private _currencyCode As String = String.Empty
    Private _currencyValue As Decimal = 0
    Private _confirmedBaseCurrencyValue As Decimal = 0
    Private _customerBankCode As String = String.Empty
    Private _thirdPartyName As String = String.Empty
    Private _thirdPartyAddressLine1 As String = String.Empty
    Private _thirdPartyAddressLine2 As String = String.Empty
    Private _thirdPartyAddressLine3 As String = String.Empty
    Private _thirdPartyAddressLine4 As String = String.Empty
    Private _thirdPartyAddressLine5 As String = String.Empty
    Private _thirdPartyPostcode As String = String.Empty
    Private _originatingAccountName As String = String.Empty
    Private _ourBankDetails As String = String.Empty
    Private _thirdPartyCountry As String = String.Empty
    Private _ourBankCountryCode As String = String.Empty
    '
    Public Property RemittanceHeader() As String
        Get
            Return _remittanceHeader
        End Get
        Set(ByVal value As String)
            _remittanceHeader = value
        End Set
    End Property
    Public Property CompanyCode() As String
        Get
            Return _companyCode
        End Get
        Set(ByVal value As String)
            _companyCode = value
        End Set
    End Property
    Public Property BankAccountNo() As String
        Get
            Return _bankAccountNo
        End Get
        Set(ByVal value As String)
            _bankAccountNo = value
        End Set
    End Property
    Public Property SOPorderNo() As String
        Get
            Return _sOPorderNo
        End Get
        Set(ByVal value As String)
            _sOPorderNo = value
        End Set
    End Property
    Public Property BankReference() As String
        Get
            Return _bankReference
        End Get
        Set(ByVal value As String)
            _bankReference = value
        End Set
    End Property
    Public Property PaymentMethod() As String
        Get
            Return _paymentMethod
        End Get
        Set(ByVal value As String)
            _paymentMethod = value
        End Set
    End Property
    Public Property PostingDate() As String
        Get
            Return _postingDate
        End Get
        Set(ByVal value As String)
            _postingDate = value
        End Set
    End Property
    Public Property CurrencyCode() As String
        Get
            Return _currencyCode
        End Get
        Set(ByVal value As String)
            _currencyCode = value
        End Set
    End Property
    Public Property CurrencyValue() As Decimal
        Get
            Return _currencyValue
        End Get
        Set(ByVal value As Decimal)
            _currencyValue = value
        End Set
    End Property
    Public Property ConfirmedBaseCurrencyValue() As Decimal
        Get
            Return _confirmedBaseCurrencyValue
        End Get
        Set(ByVal value As Decimal)
            _confirmedBaseCurrencyValue = value
        End Set
    End Property
    Public Property CustomerBankCode() As String
        Get
            Return _customerBankCode
        End Get
        Set(ByVal value As String)
            _customerBankCode = value
        End Set
    End Property
    Public Property ThirdPartyName() As String
        Get
            Return _thirdPartyName
        End Get
        Set(ByVal value As String)
            _thirdPartyName = value
        End Set
    End Property
    Public Property ThirdPartyAddressLine1() As String
        Get
            Return _thirdPartyAddressLine1
        End Get
        Set(ByVal value As String)
            _thirdPartyAddressLine1 = value
        End Set
    End Property
    Public Property ThirdPartyAddressLine2() As String
        Get
            Return _thirdPartyAddressLine2
        End Get
        Set(ByVal value As String)
            _thirdPartyAddressLine2 = value
        End Set
    End Property
    Public Property ThirdPartyAddressLine3() As String
        Get
            Return _thirdPartyAddressLine3
        End Get
        Set(ByVal value As String)
            _thirdPartyAddressLine3 = value
        End Set
    End Property
    Public Property ThirdPartyAddressLine4() As String
        Get
            Return _thirdPartyAddressLine4
        End Get
        Set(ByVal value As String)
            _thirdPartyAddressLine4 = value
        End Set
    End Property
    Public Property ThirdPartyAddressLine5() As String
        Get
            Return _thirdPartyAddressLine5
        End Get
        Set(ByVal value As String)
            _thirdPartyAddressLine5 = value
        End Set
    End Property
    Public Property ThirdPartyPostcode() As String
        Get
            Return _thirdPartyPostcode
        End Get
        Set(ByVal value As String)
            _thirdPartyPostcode = value
        End Set
    End Property
    Public Property OriginatingAccountName() As String
        Get
            Return _originatingAccountName
        End Get
        Set(ByVal value As String)
            _originatingAccountName = value
        End Set
    End Property
    Public Property OurBankDetails() As String
        Get
            Return _ourBankDetails
        End Get
        Set(ByVal value As String)
            _ourBankDetails = value
        End Set
    End Property
    Public Property ThirdPartyCountry() As String
        Get
            Return _thirdPartyCountry
        End Get
        Set(ByVal value As String)
            _thirdPartyCountry = value
        End Set
    End Property
    Public Property OurBankCountryCode() As String
        Get
            Return _ourBankCountryCode
        End Get
        Set(ByVal value As String)
            _ourBankCountryCode = value
        End Set
    End Property
    '
End Class
