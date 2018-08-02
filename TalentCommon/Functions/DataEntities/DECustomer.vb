'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Customer Details
'
'       Date                        Feb 2007
'
'       Author                      Andrew Green
'
'       � CS Group 2006             All rights reserved.
'
'       Error Number Code base      TACDECC- 
'                                   
'--------------------------------------------------------------------------------------------------
'

Public Enum SingleModeFieldsEnum
    FNAM20
    SNAM20
    ADD120
    ADD220
    ADD320
    ADD420
    ADD520
    PCD220
    DOBI20
    SEXP20
    STD120
    TEL120
    STD220
    TEL220
    FSTD20
    FAXN20
    CNAM20
    CPOS20
    EMAL20
    PSRM20
    FAVS20
    FAVA20
    MAIL
    PPNO
    DTUC20
    PHOTO
    FSEAT
    HOLD20
End Enum

<Serializable()> _
Public Class DECustomer

    '---------------------------------------------------------------------------------
    '   General Fields
    '
    Private _hasReservedGameAvailable As Boolean = False
    Private _basketID As String = String.Empty
    Private _customerID As String = String.Empty
    Private _contactID As String = String.Empty
    Private _customerNumber As String = String.Empty
    Private _customerNo As String = String.Empty
    Private _branchCode As String = String.Empty
    Private _action As String = String.Empty
    Private _manualDeDupe As String = String.Empty
    Private _manualDeDupeBranch As String = String.Empty
    Private _thirdPartyContactRef As String = String.Empty
    Private _dateFormat As String = String.Empty
    Private _thirdPartyCompanyRef1 As String = String.Empty
    Private _thirdPartyCompanyRef1Supplement As String = String.Empty
    Private _thirdPartyCompanyRef2 As String = String.Empty
    Private _thirdPartyCompanyRef2Supplement As String = String.Empty
    Private _friendsAndFamilyId As String = String.Empty
    Private _friendsAndFamilyMode As String = String.Empty
    Private _category As String = String.Empty
    Private _phoneNoFormatting As String = String.Empty
    Private _postCodeFormatting As String = String.Empty

    '---------------------------------------------------------------------------------
    '   Customer Details
    '
    Private _addressLine1 As String = String.Empty
    Private _addressLine2 As String = String.Empty
    Private _addressLine3 As String = String.Empty
    Private _addressLine4 As String = String.Empty
    Private _addressLine5 As String = String.Empty
    Private _postCode As String = String.Empty
    Private _customerDesc1 As String = String.Empty
    Private _customerDesc2 As String = String.Empty
    Private _customerDesc3 As String = String.Empty
    Private _customerDesc4 As String = String.Empty
    Private _customerDesc5 As String = String.Empty
    Private _customerDesc6 As String = String.Empty
    Private _customerDesc7 As String = String.Empty
    Private _customerDesc8 As String = String.Empty
    Private _customerDesc9 As String = String.Empty
    Private _owningAgent As String = String.Empty
    Private _webAddress As String = String.Empty
    Private _companyName As String = String.Empty
    Private _language As String = String.Empty
    Private _companySLnumber1 As String = String.Empty
    Private _companySLnumber2 As String = String.Empty
    Private _campaignCode As String = String.Empty
    Private _campaignEventCode As String = String.Empty
    Private _campaignResponseMethod As String = String.Empty
    Private _campaignResult As String = String.Empty
    Private _membershipNumber As String = String.Empty

    '---------------------------------------------------------------------------------
    '   Password Details
    '
    Private _userName As String = String.Empty
    Private _password As String = String.Empty
    Private _source As String = String.Empty
    Private _passwordType As String = String.Empty
    Private _useEncryptedPassword As String = String.Empty
    Private _passwordLength As String = String.Empty

    '---------------------------------------------------------------------------------
    '   Contact Details
    '
    Private _contactTitle As String = String.Empty
    Private _contactInitials As String = String.Empty
    Private _contactSurname As String = String.Empty
    Private _contactForename As String = String.Empty
    Private _homeTelephoneNumber As String = String.Empty
    Private _contactViaHomeTelephone As String = String.Empty
    Private _workTelephoneNumber As String = String.Empty
    Private _contactViaWorkTelephone As String = String.Empty
    Private _mobileNumber As String = String.Empty
    Private _contactViaMobile As String = String.Empty
    Private _telephone4 As String = String.Empty
    Private _contactViaTelephone4 As String = String.Empty
    Private _telephone5 As String = String.Empty
    Private _contactViaTelephone5 As String = String.Empty
    Private _contactCaptionA As String = String.Empty
    Private _contactCaptionB As String = String.Empty
    Private _contactCaptionC As String = String.Empty
    Private _contactCaptionD As String = String.Empty
    Private _contactCaptionE As String = String.Empty
    Private _contactCaptionF As String = String.Empty
    Private _contactCaptionG As String = String.Empty
    Private _contactCaptionH As String = String.Empty
    Private _contactCaptionI As String = String.Empty
    Private _contactCaptionJ As String = String.Empty
    Private _contactCaptionK As String = String.Empty
    Private _contactCaptionL As String = String.Empty
    Private _contactViaMail As String = String.Empty
    Private _contactViaMail1 As String = String.Empty
    Private _contactViaMail2 As String = String.Empty
    Private _contactViaMail3 As String = String.Empty
    Private _contactViaMail4 As String = String.Empty
    Private _contactViaMail5 As String = String.Empty
    Private _subscription1 As String = String.Empty
    Private _subscription2 As String = String.Empty
    Private _subscription3 As String = String.Empty
    Private _emailAddress As String = String.Empty
    Private _contactViaEmail As String = String.Empty
    Private _dateBirth As String = String.Empty
    Private _gender As String = String.Empty
    Private _salutation As String = String.Empty
    Private _positionInCompany As String = String.Empty
    Private _slnumber1 As String = String.Empty
    Private _slnumber2 As String = String.Empty
    Private _vatCode As String = String.Empty
    Private _nickName As String = String.Empty
    Private _Suffix As String = String.Empty
    Private _altUserName As String = String.Empty

    '---------------------------------------------------------------------------------
    ' Process flags for customer and contact fields
    '
    Private _useOptionalFields As String = String.Empty
    Private _processAddressLine1 As String = String.Empty
    Private _processAddressLine2 As String = String.Empty
    Private _processAddressLine3 As String = String.Empty
    Private _processAddressLine4 As String = String.Empty
    Private _processAddressLine5 As String = String.Empty
    Private _processPostCode As String = String.Empty
    Private _processCustomerDesc1 As String = String.Empty
    Private _processCustomerDesc2 As String = String.Empty
    Private _processCustomerDesc3 As String = String.Empty
    Private _processCustomerDesc4 As String = String.Empty
    Private _processCustomerDesc5 As String = String.Empty
    Private _processCustomerDesc6 As String = String.Empty
    Private _processCustomerDesc7 As String = String.Empty
    Private _processCustomerDesc8 As String = String.Empty
    Private _processCustomerDesc9 As String = String.Empty
    Private _processOwningAgent As String = String.Empty
    Private _processWebAddress As String = String.Empty
    Private _processCompanyName As String = String.Empty
    Private _processCompanySLnumber1 As String = String.Empty
    Private _processCompanySLnumber2 As String = String.Empty
    Private _processCampaignCode As String = String.Empty
    Private _processCampaignEventCode As String = String.Empty
    Private _processCampaignResponseMethod As String = String.Empty
    Private _processCampaignResult As String = String.Empty

    Private _processContactTitle As String = String.Empty
    Private _processContactInitials As String = String.Empty
    Private _processContactSurname As String = String.Empty
    Private _processContactForename As String = String.Empty
    Private _processHomeTelephoneNumber As String = String.Empty
    Private _processContactViaHomeTelephone As String = String.Empty
    Private _processWorkTelephoneNumber As String = String.Empty
    Private _processContactViaWorkTelephone As String = String.Empty
    Private _processMobileNumber As String = String.Empty
    Private _processContactViaMobile As String = String.Empty
    Private _processTelephone4 As String = String.Empty
    Private _processContactViaTelephone4 As String = String.Empty
    Private _processTelephone5 As String = String.Empty
    Private _processContactViaTelephone5 As String = String.Empty
    Private _processContactCaptionA As String = String.Empty
    Private _processContactCaptionB As String = String.Empty
    Private _processContactCaptionC As String = String.Empty
    Private _processContactCaptionD As String = String.Empty
    Private _processContactCaptionE As String = String.Empty
    Private _processContactCaptionF As String = String.Empty
    Private _processContactCaptionG As String = String.Empty
    Private _processContactCaptionH As String = String.Empty
    Private _processContactCaptionI As String = String.Empty
    Private _processContactCaptionJ As String = String.Empty
    Private _processContactCaptionK As String = String.Empty
    Private _processContactCaptionL As String = String.Empty
    Private _processContactViaMail As String = String.Empty
    Private _processContactViaMail1 As String = String.Empty
    Private _processContactViaMail2 As String = String.Empty
    Private _processContactViaMail3 As String = String.Empty
    Private _processContactViaMail4 As String = String.Empty
    Private _processContactViaMail5 As String = String.Empty
    Private _processSubscription1 As String = String.Empty
    Private _processSubscription2 As String = String.Empty
    Private _processSubscription3 As String = String.Empty
    Private _processEmailAddress As String = String.Empty
    Private _processContactViaEmail As String = String.Empty
    Private _processDateBirth As String = String.Empty
    Private _processGender As String = String.Empty
    Private _processSalutation As String = String.Empty
    Private _processPositionInCompany As String = String.Empty
    Private _processSLNumber1 As String = String.Empty
    Private _processSLNumber2 As String = String.Empty
    Private _processVatCode As String = String.Empty
    Private _processAttributes As String = String.Empty
    Private _processPassportNumber As String = String.Empty
    Private _processGreenCardNumber As String = String.Empty
    Private _processPIN_Number As String = String.Empty
    Private _processSupporterClubCode As String = String.Empty
    Private _processFavouriteTeamCode As String = String.Empty
    Private _processMailTeamCode1 As String = String.Empty
    Private _processMailTeamCode2 As String = String.Empty
    Private _processMailTeamCode3 As String = String.Empty
    Private _processMailTeamCode4 As String = String.Empty
    Private _processMailTeamCode5 As String = String.Empty
    Private _processPreferredContactMethod As String = String.Empty
    Private _processMothersName As String = String.Empty
    Private _processFathersName As String = String.Empty
    Private _processFavouriteSport As String = String.Empty
    Private _processUpdateCompanyInformation As String = String.Empty
    Private _processCustomerNumber As String = String.Empty
    Private _processLoginID As String = String.Empty
    Private _processPassword As String = String.Empty

    Public Property ProcessPassword() As String
        Get
            Return _processPassword
        End Get
        Set(ByVal value As String)
            _processPassword = value
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


    '---------------------------------------------------------------------------------
    '   Profile Details
    '
    Private _attribute01 As String = String.Empty
    Private _attribute01Action As String = String.Empty
    Private _attribute02 As String = String.Empty
    Private _attribute02Action As String = String.Empty
    Private _attribute03 As String = String.Empty
    Private _attribute03Action As String = String.Empty
    Private _attribute04 As String = String.Empty
    Private _attribute04Action As String = String.Empty
    Private _attribute05 As String = String.Empty
    Private _attribute05Action As String = String.Empty
    Private _attribute06 As String = String.Empty
    Private _attribute06Action As String = String.Empty
    Private _attribute07 As String = String.Empty
    Private _attribute07Action As String = String.Empty
    Private _attribute08 As String = String.Empty
    Private _attribute08Action As String = String.Empty
    Private _attribute09 As String = String.Empty
    Private _attribute09Action As String = String.Empty
    Private _attribute10 As String = String.Empty
    Private _attribute10Action As String = String.Empty
    Private _attribute11 As String = String.Empty
    Private _attribute11Action As String = String.Empty
    Private _attribute12 As String = String.Empty
    Private _attribute12Action As String = String.Empty
    Private _attribute13 As String = String.Empty
    Private _attribute13Action As String = String.Empty
    Private _attribute14 As String = String.Empty
    Private _attribute14Action As String = String.Empty
    Private _attribute15 As String = String.Empty
    Private _attribute15Action As String = String.Empty
    Private _attribute16 As String = String.Empty
    Private _attribute16Action As String = String.Empty
    Private _attribute17 As String = String.Empty
    Private _attribute17Action As String = String.Empty
    Private _attribute18 As String = String.Empty
    Private _attribute18Action As String = String.Empty
    Private _attribute19 As String = String.Empty
    Private _attribute19Action As String = String.Empty
    Private _attribute20 As String = String.Empty
    Private _attribute20Action As String = String.Empty
    Private _attribute21 As String = String.Empty
    Private _attribute21Action As String = String.Empty
    Private _attribute22 As String = String.Empty
    Private _attribute22Action As String = String.Empty
    Private _attribute23 As String = String.Empty
    Private _attribute23Action As String = String.Empty
    Private _attribute24 As String = String.Empty
    Private _attribute24Action As String = String.Empty
    Private _attribute25 As String = String.Empty
    Private _attribute25Action As String = String.Empty
    Private _attribute26 As String = String.Empty
    Private _attribute26Action As String = String.Empty
    Private _attribute27 As String = String.Empty
    Private _attribute27Action As String = String.Empty
    Private _attribute28 As String = String.Empty
    Private _attribute28Action As String = String.Empty
    Private _attribute29 As String = String.Empty
    Private _attribute29Action As String = String.Empty
    Private _attribute30 As String = String.Empty
    Private _attribute30Action As String = String.Empty
    Private _attribute31 As String = String.Empty
    Private _attribute31Action As String = String.Empty
    Private _attribute32 As String = String.Empty
    Private _attribute32Action As String = String.Empty
    Private _attribute33 As String = String.Empty
    Private _attribute33Action As String = String.Empty
    Private _attribute34 As String = String.Empty
    Private _attribute34Action As String = String.Empty
    Private _attribute35 As String = String.Empty
    Private _attribute35Action As String = String.Empty
    Private _attribute36 As String = String.Empty
    Private _attribute36Action As String = String.Empty
    Private _attribute37 As String = String.Empty
    Private _attribute37Action As String = String.Empty
    Private _attribute38 As String = String.Empty
    Private _attribute38Action As String = String.Empty
    Private _attribute39 As String = String.Empty
    Private _attribute39Action As String = String.Empty
    Private _attribute40 As String = String.Empty
    Private _attribute40Action As String = String.Empty
    Private _attribute41 As String = String.Empty
    Private _attribute41Action As String = String.Empty
    Private _attribute42 As String = String.Empty
    Private _attribute42Action As String = String.Empty
    Private _attribute43 As String = String.Empty
    Private _attribute43Action As String = String.Empty
    Private _attribute44 As String = String.Empty
    Private _attribute44Action As String = String.Empty
    Private _attribute45 As String = String.Empty
    Private _attribute45Action As String = String.Empty
    Private _attribute46 As String = String.Empty
    Private _attribute46Action As String = String.Empty
    Private _attribute47 As String = String.Empty
    Private _attribute47Action As String = String.Empty
    Private _attribute48 As String = String.Empty
    Private _attribute48Action As String = String.Empty
    Private _attribute49 As String = String.Empty
    Private _attribute49Action As String = String.Empty
    Private _attribute50 As String = String.Empty
    Private _attribute50Action As String = String.Empty
    Private _attribute51 As String = String.Empty
    Private _attribute51Action As String = String.Empty
    Private _attribute52 As String = String.Empty
    Private _attribute52Action As String = String.Empty
    Private _attribute53 As String = String.Empty
    Private _attribute53Action As String = String.Empty
    Private _attribute54 As String = String.Empty
    Private _attribute54Action As String = String.Empty
    Private _attribute55 As String = String.Empty
    Private _attribute55Action As String = String.Empty
    Private _attribute56 As String = String.Empty
    Private _attribute56Action As String = String.Empty
    Private _attribute57 As String = String.Empty
    Private _attribute57Action As String = String.Empty
    Private _attribute58 As String = String.Empty
    Private _attribute58Action As String = String.Empty
    Private _attribute59 As String = String.Empty
    Private _attribute59Action As String = String.Empty
    Private _attribute60 As String = String.Empty
    Private _attribute60Action As String = String.Empty

    '---------------------------------------------------------------------------------
    '   Action Details
    '
    Private _actionCode As String = String.Empty
    Private _actionCodeFixed As String = String.Empty
    Private _actionStatus As String = String.Empty
    Private _actionPty As String = String.Empty
    Private _actionAgent As String = String.Empty
    Private _actionComment01 As String = String.Empty
    Private _actionComment02 As String = String.Empty
    Private _actionComment03 As String = String.Empty
    Private _actionComment04 As String = String.Empty
    Private _actionComment05 As String = String.Empty
    Private _actionComment06 As String = String.Empty
    Private _actionComment07 As String = String.Empty
    Private _actionComment08 As String = String.Empty
    Private _actionComment09 As String = String.Empty
    Private _actionComment10 As String = String.Empty
    Private _actionComment11 As String = String.Empty
    Private _actionComment12 As String = String.Empty
    Private _actionComment13 As String = String.Empty
    Private _actionComment14 As String = String.Empty
    Private _actionComment15 As String = String.Empty
    Private _actionComment16 As String = String.Empty
    Private _actionDate As String = String.Empty
    Private _actionID As String = String.Empty
    Private _actionSubject As String = String.Empty
    Private _actionMemo As String = String.Empty
    Private _actionDepartmentID As String = String.Empty
    Private _actionProductID As String = String.Empty
    Private _actionProjectID As String = String.Empty
    Private _actionCampaignID As String = String.Empty
    Private _actionField1 As String = String.Empty
    Private _actionField2 As String = String.Empty
    Private _actionField3 As String = String.Empty
    Private _actionField4 As String = String.Empty
    Private _actionField5 As String = String.Empty

    '---------------------------------------------------------------------------------
    '   Note Details
    '
    Private _noteCode As String = String.Empty
    Private _noteComment01 As String = String.Empty
    Private _noteComment02 As String = String.Empty
    Private _noteComment03 As String = String.Empty
    Private _noteComment04 As String = String.Empty
    Private _noteComment05 As String = String.Empty
    Private _noteComment06 As String = String.Empty
    Private _noteComment07 As String = String.Empty
    Private _noteComment08 As String = String.Empty
    Private _noteComment09 As String = String.Empty
    Private _noteComment10 As String = String.Empty
    Private _noteComment11 As String = String.Empty
    Private _noteComment12 As String = String.Empty
    Private _noteComment13 As String = String.Empty
    Private _noteComment14 As String = String.Empty
    Private _noteComment15 As String = String.Empty
    Private _noteComment16 As String = String.Empty

    Private _errorCode As String = ""
    Private _errorFlag As String = ""
    Public Property BasketID As String
        Get
            Return _basketID
        End Get
        Set(ByVal value As String)
            _basketID = value
        End Set
    End Property
    Public Property HasReservedGameAvailable As Boolean
        Get
            Return _hasReservedGameAvailable
        End Get
        Set(ByVal value As Boolean)
            _hasReservedGameAvailable = value
        End Set
    End Property
    Public Property CustomerID() As String
        Get
            Return _customerID
        End Get
        Set(ByVal value As String)
            _customerID = value
        End Set
    End Property
    Public Property ContactID() As String
        Get
            Return _contactID
        End Get
        Set(ByVal value As String)
            _contactID = value
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
    Public Property CustomerNo() As String
        Get
            Return _customerNo
        End Get
        Set(ByVal value As String)
            _customerNo = value
        End Set
    End Property
    Public Property BranchCode() As String
        Get
            Return _branchCode
        End Get
        Set(ByVal value As String)
            _branchCode = value
        End Set
    End Property
    Public Property Action() As String
        Get
            Return _action
        End Get
        Set(ByVal value As String)
            _action = value
        End Set
    End Property
    Public Property ManualDeDupe() As String
        Get
            Return _manualDeDupe
        End Get
        Set(ByVal value As String)
            _manualDeDupe = value
        End Set
    End Property
    Public Property ManualDeDupeBranch() As String
        Get
            Return _manualDeDupeBranch
        End Get
        Set(ByVal value As String)
            _manualDeDupeBranch = value
        End Set
    End Property
    Public Property ThirdPartyContactRef() As String
        Get
            Return _thirdPartyContactRef
        End Get
        Set(ByVal value As String)
            _thirdPartyContactRef = value
        End Set
    End Property
    Public Property DateFormat() As String
        Get
            Return _dateFormat
        End Get
        Set(ByVal value As String)
            _dateFormat = value
        End Set
    End Property
    Public Property ThirdPartyCompanyRef1() As String
        Get
            Return _thirdPartyCompanyRef1
        End Get
        Set(ByVal value As String)
            _thirdPartyCompanyRef1 = value
        End Set
    End Property
    Public Property ThirdPartyCompanyRef2() As String
        Get
            Return _thirdPartyCompanyRef2
        End Get
        Set(ByVal value As String)
            _thirdPartyCompanyRef2 = value
        End Set
    End Property
    Public Property ThirdPartyCompanyRef1Supplement() As String
        Get
            Return _thirdPartyCompanyRef1Supplement
        End Get
        Set(ByVal value As String)
            _thirdPartyCompanyRef1Supplement = value
        End Set
    End Property
    Public Property ThirdPartyCompanyRef2Supplement() As String
        Get
            Return _thirdPartyCompanyRef2Supplement
        End Get
        Set(ByVal value As String)
            _thirdPartyCompanyRef2Supplement = value
        End Set
    End Property
    Public Property FriendsAndFamilyId() As String
        Get
            Return _friendsAndFamilyId
        End Get
        Set(ByVal value As String)
            _friendsAndFamilyId = value
        End Set
    End Property
    Public Property FriendsAndFamilyMode() As String
        Get
            Return _friendsAndFamilyMode
        End Get
        Set(ByVal value As String)
            _friendsAndFamilyMode = value
        End Set
    End Property

    Public Property AddressLine1() As String
        Get
            Return _addressLine1
        End Get
        Set(ByVal value As String)
            _addressLine1 = value
        End Set
    End Property
    Public Property AddressLine2() As String
        Get
            Return _addressLine2
        End Get
        Set(ByVal value As String)
            _addressLine2 = value
        End Set
    End Property
    Public Property AddressLine3() As String
        Get
            Return _addressLine3
        End Get
        Set(ByVal value As String)
            _addressLine3 = value
        End Set
    End Property
    Public Property AddressLine4() As String
        Get
            Return _addressLine4
        End Get
        Set(ByVal value As String)
            _addressLine4 = value
        End Set
    End Property
    Public Property AddressLine5() As String
        Get
            Return _addressLine5
        End Get
        Set(ByVal value As String)
            _addressLine5 = value
        End Set
    End Property
    Public Property PostCode() As String
        Get
            Return _postCode
        End Get
        Set(ByVal value As String)
            _postCode = value
        End Set
    End Property
    Public Property CustomerDesc1() As String
        Get
            Return _customerDesc1
        End Get
        Set(ByVal value As String)
            _customerDesc1 = value
        End Set
    End Property
    Public Property CustomerDesc2() As String
        Get
            Return _customerDesc2
        End Get
        Set(ByVal value As String)
            _customerDesc2 = value
        End Set
    End Property
    Public Property CustomerDesc3() As String
        Get
            Return _customerDesc3
        End Get
        Set(ByVal value As String)
            _customerDesc3 = value
        End Set
    End Property
    Public Property CustomerDesc4() As String
        Get
            Return _customerDesc4
        End Get
        Set(ByVal value As String)
            _customerDesc4 = value
        End Set
    End Property
    Public Property CustomerDesc5() As String
        Get
            Return _customerDesc5
        End Get
        Set(ByVal value As String)
            _customerDesc5 = value
        End Set
    End Property
    Public Property CustomerDesc6() As String
        Get
            Return _customerDesc6
        End Get
        Set(ByVal value As String)
            _customerDesc6 = value
        End Set
    End Property
    Public Property CustomerDesc7() As String
        Get
            Return _customerDesc7
        End Get
        Set(ByVal value As String)
            _customerDesc7 = value
        End Set
    End Property
    Public Property CustomerDesc8() As String
        Get
            Return _customerDesc8
        End Get
        Set(ByVal value As String)
            _customerDesc8 = value
        End Set
    End Property
    Public Property CustomerDesc9() As String
        Get
            Return _customerDesc9
        End Get
        Set(ByVal value As String)
            _customerDesc9 = value
        End Set
    End Property
    Public Property OwningAgent() As String
        Get
            Return _owningAgent
        End Get
        Set(ByVal value As String)
            _owningAgent = value
        End Set
    End Property
    Public Property WebAddress() As String
        Get
            Return _webAddress
        End Get
        Set(ByVal value As String)
            _webAddress = value
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
    Public Property Language() As String
        Get
            Return _language
        End Get
        Set(ByVal value As String)
            _language = value
        End Set
    End Property
    Public Property CompanySLNumber1() As String
        Get
            Return _companySLnumber1
        End Get
        Set(ByVal value As String)
            _companySLnumber1 = value
        End Set
    End Property
    Public Property CompanySLNumber2() As String
        Get
            Return _companySLnumber2
        End Get
        Set(ByVal value As String)
            _companySLnumber2 = value
        End Set
    End Property
    Public Property CampaignCode() As String
        Get
            Return _campaignCode
        End Get
        Set(ByVal value As String)
            _campaignCode = value
        End Set
    End Property
    Public Property CampaignEventCode() As String
        Get
            Return _campaignEventCode
        End Get
        Set(ByVal value As String)
            _campaignEventCode = value
        End Set
    End Property
    Public Property CampaignResponseMethod() As String
        Get
            Return _campaignResponseMethod
        End Get
        Set(ByVal value As String)
            _campaignResponseMethod = value
        End Set
    End Property
    Public Property CampaignResult() As String
        Get
            Return _campaignResult
        End Get
        Set(ByVal value As String)
            _campaignResult = value
        End Set
    End Property
    Public Property MembershipNumber() As String
        Get
            Return _membershipNumber
        End Get
        Set(ByVal value As String)
            _membershipNumber = value
        End Set
    End Property
    Public Property Password() As String
        Get
            Return _password
        End Get
        Set(ByVal value As String)
            _password = value
        End Set
    End Property
    Public Property UserName() As String
        Get
            Return _userName
        End Get
        Set(ByVal value As String)
            _userName = value
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
    Public Property PasswordType() As String
        Get
            Return _passwordType
        End Get
        Set(ByVal value As String)
            _passwordType = value
        End Set
    End Property

    'Public Property useEncryptedPassword() As String
    '    Get
    '        Return _useEncryptedPassword
    '    End Get
    '    Set(ByVal value As String)
    '        _useEncryptedPassword = value
    '    End Set
    'End Property
    Public Property PasswordLength() As Integer
        Get
            Return _passwordLength
        End Get
        Set(ByVal value As Integer)
            _passwordLength = value
        End Set
    End Property

    Public Property ContactTitle() As String
        Get
            Return _contactTitle
        End Get
        Set(ByVal value As String)
            _contactTitle = value
        End Set
    End Property
    Public Property ContactInitials() As String
        Get
            Return _contactInitials
        End Get
        Set(ByVal value As String)
            _contactInitials = value
        End Set
    End Property
    Public Property ContactSurname() As String
        Get
            Return _contactSurname
        End Get
        Set(ByVal value As String)
            _contactSurname = value
        End Set
    End Property
    Public Property ContactForename() As String
        Get
            Return _contactForename
        End Get
        Set(ByVal value As String)
            _contactForename = value
        End Set
    End Property
    Public Property HomeTelephoneNumber() As String
        Get
            Return _homeTelephoneNumber
        End Get
        Set(ByVal value As String)
            _homeTelephoneNumber = value
        End Set
    End Property
    Public Property ContactViaHomeTelephone() As String
        Get
            Return _contactViaHomeTelephone
        End Get
        Set(ByVal value As String)
            _contactViaHomeTelephone = value
        End Set
    End Property
    Public Property WorkTelephoneNumber() As String
        Get
            Return _workTelephoneNumber
        End Get
        Set(ByVal value As String)
            _workTelephoneNumber = value
        End Set
    End Property
    Public Property ContactViaWorkTelephone() As String
        Get
            Return _contactViaWorkTelephone
        End Get
        Set(ByVal value As String)
            _contactViaWorkTelephone = value
        End Set
    End Property
    Public Property MobileNumber() As String
        Get
            Return _mobileNumber
        End Get
        Set(ByVal value As String)
            _mobileNumber = value
        End Set
    End Property
    Public Property ContactViaMobile() As String
        Get
            Return _contactViaMobile
        End Get
        Set(ByVal value As String)
            _contactViaMobile = value
        End Set
    End Property
    Public Property Telephone4() As String
        Get
            Return _telephone4
        End Get
        Set(ByVal value As String)
            _telephone4 = value
        End Set
    End Property
    Public Property ContactViaTelephone4() As String
        Get
            Return _contactViaTelephone4
        End Get
        Set(ByVal value As String)
            _contactViaTelephone4 = value
        End Set
    End Property
    Public Property Telephone5() As String
        Get
            Return _telephone5
        End Get
        Set(ByVal value As String)
            _telephone5 = value
        End Set
    End Property
    Public Property ContactViaTelephone5() As String
        Get
            Return _contactViaTelephone5
        End Get
        Set(ByVal value As String)
            _contactViaTelephone5 = value
        End Set
    End Property
    Public Property ContactCaptionA() As String
        Get
            Return _contactCaptionA
        End Get
        Set(ByVal value As String)
            _contactCaptionA = value
        End Set
    End Property
    Public Property ContactCaptionB() As String
        Get
            Return _contactCaptionB
        End Get
        Set(ByVal value As String)
            _contactCaptionB = value
        End Set
    End Property
    Public Property ContactCaptionC() As String
        Get
            Return _contactCaptionC
        End Get
        Set(ByVal value As String)
            _contactCaptionC = value
        End Set
    End Property
    Public Property ContactCaptionD() As String
        Get
            Return _contactCaptionD
        End Get
        Set(ByVal value As String)
            _contactCaptionD = value
        End Set
    End Property
    Public Property ContactCaptionE() As String
        Get
            Return _contactCaptionE
        End Get
        Set(ByVal value As String)
            _contactCaptionE = value
        End Set
    End Property
    Public Property ContactCaptionF() As String
        Get
            Return _contactCaptionF
        End Get
        Set(ByVal value As String)
            _contactCaptionF = value
        End Set
    End Property
    Public Property ContactCaptionG() As String
        Get
            Return _contactCaptionG
        End Get
        Set(ByVal value As String)
            _contactCaptionG = value
        End Set
    End Property
    Public Property ContactCaptionH() As String
        Get
            Return _contactCaptionH
        End Get
        Set(ByVal value As String)
            _contactCaptionH = value
        End Set
    End Property
    Public Property ContactCaptionI() As String
        Get
            Return _contactCaptionI
        End Get
        Set(ByVal value As String)
            _contactCaptionI = value
        End Set
    End Property
    Public Property ContactCaptionJ() As String
        Get
            Return _contactCaptionJ
        End Get
        Set(ByVal value As String)
            _contactCaptionJ = value
        End Set
    End Property
    Public Property ContactCaptionK() As String
        Get
            Return _contactCaptionK
        End Get
        Set(ByVal value As String)
            _contactCaptionK = value
        End Set
    End Property
    Public Property ContactCaptionL() As String
        Get
            Return _contactCaptionL
        End Get
        Set(ByVal value As String)
            _contactCaptionL = value
        End Set
    End Property
    Public Property ContactViaMail() As String
        Get
            Return _contactViaMail
        End Get
        Set(ByVal value As String)
            _contactViaMail = value
        End Set
    End Property
    Public Property ContactViaMail1() As String
        Get
            Return _contactViaMail1
        End Get
        Set(ByVal value As String)
            _contactViaMail1 = value
        End Set
    End Property
    Public Property ContactViaMail2() As String
        Get
            Return _contactViaMail2
        End Get
        Set(ByVal value As String)
            _contactViaMail2 = value
        End Set
    End Property
    Public Property ContactViaMail3() As String
        Get
            Return _contactViaMail3
        End Get
        Set(ByVal value As String)
            _contactViaMail3 = value
        End Set
    End Property
    Public Property ContactViaMail4() As String
        Get
            Return _contactViaMail4
        End Get
        Set(ByVal value As String)
            _contactViaMail4 = value
        End Set
    End Property
    Public Property ContactViaMail5() As String
        Get
            Return _contactViaMail5
        End Get
        Set(ByVal value As String)
            _contactViaMail5 = value
        End Set
    End Property
    Public Property Subscription1() As String
        Get
            Return _subscription1
        End Get
        Set(ByVal value As String)
            _subscription1 = value
        End Set
    End Property
    Public Property Subscription2() As String
        Get
            Return _subscription2
        End Get
        Set(ByVal value As String)
            _subscription2 = value
        End Set
    End Property
    Public Property Subscription3() As String
        Get
            Return _subscription3
        End Get
        Set(ByVal value As String)
            _subscription3 = value
        End Set
    End Property
    Public Property EmailAddress() As String
        Get
            Return _emailAddress
        End Get
        Set(ByVal value As String)
            _emailAddress = value
        End Set
    End Property
    Public Property ContactViaEmail() As String
        Get
            Return _contactViaEmail
        End Get
        Set(ByVal value As String)
            _contactViaEmail = value
        End Set
    End Property
    Public Property DateBirth() As String
        Get
            Return _dateBirth
        End Get
        Set(ByVal value As String)
            _dateBirth = value
        End Set
    End Property
    Public Property Gender() As String
        Get
            Return _gender
        End Get
        Set(ByVal value As String)
            _gender = value
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
    Public Property PositionInCompany() As String
        Get
            Return _positionInCompany
        End Get
        Set(ByVal value As String)
            _positionInCompany = value
        End Set
    End Property
    Public Property SLNumber1() As String
        Get
            Return _slnumber1
        End Get
        Set(ByVal value As String)
            _slnumber1 = value
        End Set
    End Property
    Public Property SLNumber2() As String
        Get
            Return _slnumber2
        End Get
        Set(ByVal value As String)
            _slnumber2 = value
        End Set
    End Property
    Public Property VatCode() As String
        Get
            Return _vatCode
        End Get
        Set(ByVal value As String)
            _vatCode = value
        End Set
    End Property
    Public Property Nickname() As String
        Get
            Return _nickName
        End Get
        Set(ByVal value As String)
            _nickName = value
        End Set
    End Property

    Public Property Suffix() As String
        Get
            Return _Suffix
        End Get
        Set(ByVal value As String)
            _Suffix = value
        End Set
    End Property
    Public Property AltUserName() As String
        Get
            Return _altUserName
        End Get
        Set(ByVal value As String)
            _altUserName = value
        End Set
    End Property
    Public Property ContactSource As String
    Public Property ContactSLAccount As String
    Public Property MinimalRegistration As Boolean = False

    Public Property UseOptionalFields() As String
        Get
            Return _useOptionalFields
        End Get
        Set(ByVal value As String)
            _useOptionalFields = value
        End Set
    End Property

    Public Property ProcessAddressLine1() As String
        Get
            Return _processAddressLine1
        End Get
        Set(ByVal value As String)
            _processAddressLine1 = value
        End Set
    End Property
    Public Property ProcessAddressLine2() As String
        Get
            Return _processAddressLine2
        End Get
        Set(ByVal value As String)
            _processAddressLine2 = value
        End Set
    End Property
    Public Property ProcessAddressLine3() As String
        Get
            Return _processAddressLine3
        End Get
        Set(ByVal value As String)
            _processAddressLine3 = value
        End Set
    End Property
    Public Property ProcessAddressLine4() As String
        Get
            Return _processAddressLine4
        End Get
        Set(ByVal value As String)
            _processAddressLine4 = value
        End Set
    End Property
    Public Property ProcessAddressLine5() As String
        Get
            Return _processAddressLine5
        End Get
        Set(ByVal value As String)
            _processAddressLine5 = value
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
    Public Property ProcessCustomerDesc1() As String
        Get
            Return _processCustomerDesc1
        End Get
        Set(ByVal value As String)
            _processCustomerDesc1 = value
        End Set
    End Property
    Public Property ProcessCustomerDesc2() As String
        Get
            Return _processCustomerDesc2
        End Get
        Set(ByVal value As String)
            _processCustomerDesc2 = value
        End Set
    End Property
    Public Property ProcessCustomerDesc3() As String
        Get
            Return _processCustomerDesc3
        End Get
        Set(ByVal value As String)
            _processCustomerDesc3 = value
        End Set
    End Property
    Public Property ProcessCustomerDesc4() As String
        Get
            Return _processCustomerDesc4
        End Get
        Set(ByVal value As String)
            _processCustomerDesc4 = value
        End Set
    End Property
    Public Property ProcessCustomerDesc5() As String
        Get
            Return _processCustomerDesc5
        End Get
        Set(ByVal value As String)
            _processCustomerDesc5 = value
        End Set
    End Property
    Public Property ProcessCustomerDesc6() As String
        Get
            Return _processCustomerDesc6
        End Get
        Set(ByVal value As String)
            _processCustomerDesc6 = value
        End Set
    End Property
    Public Property ProcessCustomerDesc7() As String
        Get
            Return _processCustomerDesc7
        End Get
        Set(ByVal value As String)
            _processCustomerDesc7 = value
        End Set
    End Property
    Public Property ProcessCustomerDesc8() As String
        Get
            Return _processCustomerDesc8
        End Get
        Set(ByVal value As String)
            _processCustomerDesc8 = value
        End Set
    End Property
    Public Property ProcessCustomerDesc9() As String
        Get
            Return _processCustomerDesc9
        End Get
        Set(ByVal value As String)
            _processCustomerDesc9 = value
        End Set
    End Property
    Public Property ProcessOwningAgent() As String
        Get
            Return _processOwningAgent
        End Get
        Set(ByVal value As String)
            _processOwningAgent = value
        End Set
    End Property
    Public Property ProcessWebAddress() As String
        Get
            Return _processWebAddress
        End Get
        Set(ByVal value As String)
            _processWebAddress = value
        End Set
    End Property
    Public Property ProcessCompanyName() As String
        Get
            Return _processCompanyName
        End Get
        Set(ByVal value As String)
            _processCompanyName = value
        End Set
    End Property
    Public Property ProcessCompanySLNumber1() As String
        Get
            Return _processCompanySLnumber1
        End Get
        Set(ByVal value As String)
            _processCompanySLnumber1 = value
        End Set
    End Property
    Public Property ProcessCompanySLNumber2() As String
        Get
            Return _processCompanySLnumber2
        End Get
        Set(ByVal value As String)
            _processCompanySLnumber2 = value
        End Set
    End Property
    Public Property ProcessCampaignCode() As String
        Get
            Return _processCampaignCode
        End Get
        Set(ByVal value As String)
            _processCampaignCode = value
        End Set
    End Property
    Public Property ProcessCampaignEventCode() As String
        Get
            Return _processCampaignEventCode
        End Get
        Set(ByVal value As String)
            _processCampaignEventCode = value
        End Set
    End Property

    Public Property ProcessCampaignResponseMethod() As String
        Get
            Return _processCampaignResponseMethod
        End Get
        Set(ByVal value As String)
            _processCampaignResponseMethod = value
        End Set
    End Property
    Public Property ProcessCampaignResult() As String
        Get
            Return _processCampaignResult
        End Get
        Set(ByVal value As String)
            _processCampaignResult = value
        End Set
    End Property
    Public Property ProcessContactTitle() As String
        Get
            Return _processContactTitle
        End Get
        Set(ByVal value As String)
            _processContactTitle = value
        End Set
    End Property
    Public Property ProcessContactInitials() As String
        Get
            Return _processContactInitials
        End Get
        Set(ByVal value As String)
            _processContactInitials = value
        End Set
    End Property
    Public Property ProcessContactSurname() As String
        Get
            Return _processContactSurname
        End Get
        Set(ByVal value As String)
            _processContactSurname = value
        End Set
    End Property
    Public Property ProcessContactForename() As String
        Get
            Return _processContactForename
        End Get
        Set(ByVal value As String)
            _processContactForename = value
        End Set
    End Property
    Public Property ProcessHomeTelephoneNumber() As String
        Get
            Return _processHomeTelephoneNumber
        End Get
        Set(ByVal value As String)
            _processHomeTelephoneNumber = value
        End Set
    End Property
    Public Property ProcessContactViaHomeTelephone() As String
        Get
            Return _processContactViaHomeTelephone
        End Get
        Set(ByVal value As String)
            _processContactViaHomeTelephone = value
        End Set
    End Property
    Public Property ProcessWorkTelephoneNumber() As String
        Get
            Return _processWorkTelephoneNumber
        End Get
        Set(ByVal value As String)
            _processWorkTelephoneNumber = value
        End Set
    End Property
    Public Property ProcessContactViaWorkTelephone() As String
        Get
            Return _processContactViaWorkTelephone
        End Get
        Set(ByVal value As String)
            _processContactViaWorkTelephone = value
        End Set
    End Property
    Public Property ProcessMobileNumber() As String
        Get
            Return _processMobileNumber
        End Get
        Set(ByVal value As String)
            _processMobileNumber = value
        End Set
    End Property
    Public Property ProcessContactViaMobile() As String
        Get
            Return _processContactViaMobile
        End Get
        Set(ByVal value As String)
            _processContactViaMobile = value
        End Set
    End Property
    Public Property ProcessTelephone4() As String
        Get
            Return _processTelephone4
        End Get
        Set(ByVal value As String)
            _processTelephone4 = value
        End Set
    End Property
    Public Property ProcessContactViaTelephone4() As String
        Get
            Return _processContactViaTelephone4
        End Get
        Set(ByVal value As String)
            _processContactViaTelephone4 = value
        End Set
    End Property
    Public Property ProcessTelephone5() As String
        Get
            Return _processTelephone5
        End Get
        Set(ByVal value As String)
            _processTelephone5 = value
        End Set
    End Property
    Public Property ProcessContactViaTelephone5() As String
        Get
            Return _processContactViaTelephone5
        End Get
        Set(ByVal value As String)
            _processContactViaTelephone5 = value
        End Set
    End Property
    Public Property ProcessContactCaptionA() As String
        Get
            Return _processContactCaptionA
        End Get
        Set(ByVal value As String)
            _processContactCaptionA = value
        End Set
    End Property
    Public Property ProcessContactCaptionB() As String
        Get
            Return _processContactCaptionB
        End Get
        Set(ByVal value As String)
            _processContactCaptionB = value
        End Set
    End Property
    Public Property ProcessContactCaptionC() As String
        Get
            Return _processContactCaptionC
        End Get
        Set(ByVal value As String)
            _processContactCaptionC = value
        End Set
    End Property
    Public Property ProcessContactCaptionD() As String
        Get
            Return _processContactCaptionD
        End Get
        Set(ByVal value As String)
            _processContactCaptionD = value
        End Set
    End Property
    Public Property ProcessContactCaptionE() As String
        Get
            Return _processContactCaptionE
        End Get
        Set(ByVal value As String)
            _processContactCaptionE = value
        End Set
    End Property
    Public Property ProcessContactCaptionF() As String
        Get
            Return _processContactCaptionF
        End Get
        Set(ByVal value As String)
            _processContactCaptionF = value
        End Set
    End Property
    Public Property ProcessContactCaptionG() As String
        Get
            Return _processContactCaptionG
        End Get
        Set(ByVal value As String)
            _processContactCaptionG = value
        End Set
    End Property
    Public Property ProcessContactCaptionH() As String
        Get
            Return _processContactCaptionH
        End Get
        Set(ByVal value As String)
            _processContactCaptionH = value
        End Set
    End Property
    Public Property ProcessContactCaptionI() As String
        Get
            Return _processContactCaptionI
        End Get
        Set(ByVal value As String)
            _processContactCaptionI = value
        End Set
    End Property
    Public Property ProcessContactCaptionJ() As String
        Get
            Return _processContactCaptionJ
        End Get
        Set(ByVal value As String)
            _processContactCaptionJ = value
        End Set
    End Property
    Public Property ProcessContactCaptionK() As String
        Get
            Return _processContactCaptionK
        End Get
        Set(ByVal value As String)
            _processContactCaptionK = value
        End Set
    End Property
    Public Property ProcessContactCaptionL() As String
        Get
            Return _processContactCaptionL
        End Get
        Set(ByVal value As String)
            _processContactCaptionL = value
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
    Public Property ProcessContactViaMail1() As String
        Get
            Return _processContactViaMail1
        End Get
        Set(ByVal value As String)
            _processContactViaMail1 = value
        End Set
    End Property
    Public Property ProcessContactViaMail2() As String
        Get
            Return _processContactViaMail2
        End Get
        Set(ByVal value As String)
            _processContactViaMail2 = value
        End Set
    End Property
    Public Property ProcessContactViaMail3() As String
        Get
            Return _processContactViaMail3
        End Get
        Set(ByVal value As String)
            _processContactViaMail3 = value
        End Set
    End Property
    Public Property ProcessContactViaMail4() As String
        Get
            Return _processContactViaMail4
        End Get
        Set(ByVal value As String)
            _processContactViaMail4 = value
        End Set
    End Property
    Public Property ProcessContactViaMail5() As String
        Get
            Return _processContactViaMail5
        End Get
        Set(ByVal value As String)
            _processContactViaMail5 = value
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
    Public Property ProcessEmailAddress() As String
        Get
            Return _processEmailAddress
        End Get
        Set(ByVal value As String)
            _processEmailAddress = value
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
    Public Property ProcessDateBirth() As String
        Get
            Return _processDateBirth
        End Get
        Set(ByVal value As String)
            _processDateBirth = value
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
    Public Property ProcessSalutation() As String
        Get
            Return _processSalutation
        End Get
        Set(ByVal value As String)
            _processSalutation = value
        End Set
    End Property
    Public Property ProcessPositionInCompany() As String
        Get
            Return _processPositionInCompany
        End Get
        Set(ByVal value As String)
            _processPositionInCompany = value
        End Set
    End Property
    Public Property ProcessSLNumber1() As String
        Get
            Return _processSLNumber1
        End Get
        Set(ByVal value As String)
            _processSLNumber1 = value
        End Set
    End Property
    Public Property ProcessSLNumber2() As String
        Get
            Return _processSLNumber2
        End Get
        Set(ByVal value As String)
            _processSLNumber2 = value
        End Set
    End Property

    Public Property ProcessVatCode() As String
        Get
            Return _processVatCode
        End Get
        Set(ByVal value As String)
            _processVatCode = value
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

    Public Property ProcessPassportNumber() As String
        Get
            Return _processPassportNumber
        End Get
        Set(ByVal value As String)
            _processPassportNumber = value
        End Set
    End Property

    Public Property ProcessGreenCardNumber() As String
        Get
            Return _processGreenCardNumber
        End Get
        Set(ByVal value As String)
            _processGreenCardNumber = value
        End Set
    End Property

    Public Property ProcessPinNumber() As String
        Get
            Return _processPIN_Number
        End Get
        Set(ByVal value As String)
            _processPIN_Number = value
        End Set
    End Property

    Public Property ProcessSupporterClubCode() As String
        Get
            Return _processSupporterClubCode
        End Get
        Set(ByVal value As String)
            _processSupporterClubCode = value
        End Set
    End Property

    Public Property ProcessFavouriteTeamCode() As String
        Get
            Return _processFavouriteTeamCode
        End Get
        Set(ByVal value As String)
            _processFavouriteTeamCode = value
        End Set
    End Property

    Public Property ProcessMailTeamCode1() As String
        Get
            Return _processMailTeamCode1
        End Get
        Set(ByVal value As String)
            _processMailTeamCode1 = value
        End Set
    End Property

    Public Property ProcessMailTeamCode2() As String
        Get
            Return _processMailTeamCode2
        End Get
        Set(ByVal value As String)
            _processMailTeamCode2 = value
        End Set
    End Property

    Public Property ProcessMailTeamCode3() As String
        Get
            Return _processMailTeamCode3
        End Get
        Set(ByVal value As String)
            _processMailTeamCode3 = value
        End Set
    End Property

    Public Property ProcessMailTeamCode4() As String
        Get
            Return _processMailTeamCode4
        End Get
        Set(ByVal value As String)
            _processMailTeamCode4 = value
        End Set
    End Property

    Public Property ProcessMailTeamCode5() As String
        Get
            Return _processMailTeamCode5
        End Get
        Set(ByVal value As String)
            _processMailTeamCode5 = value
        End Set
    End Property

    Public Property ProcessPreferredContactMethod() As String
        Get
            Return _processPreferredContactMethod
        End Get
        Set(ByVal value As String)
            _processPreferredContactMethod = value
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

    Public Property ProcessFavouriteSport() As String
        Get
            Return _processFavouriteSport
        End Get
        Set(ByVal value As String)
            _processFavouriteSport = value
        End Set
    End Property

    Public Property ProcessUpdateCompanyInformation() As String
        Get
            Return _processUpdateCompanyInformation
        End Get
        Set(ByVal value As String)
            _processUpdateCompanyInformation = value
        End Set
    End Property

    Public Property ProcessCustomerNumber() As String
        Get
            Return _processCustomerNumber
        End Get
        Set(ByVal value As String)
            _processCustomerNumber = value
        End Set
    End Property

    Public Property Attribute01() As String
        Get
            Return _attribute01
        End Get
        Set(ByVal value As String)
            _attribute01 = value
        End Set
    End Property
    Public Property Attribute01Action() As String
        Get
            Return _attribute01Action
        End Get
        Set(ByVal value As String)
            _attribute01Action = value
        End Set
    End Property
    Public Property Attribute02() As String
        Get
            Return _attribute02
        End Get
        Set(ByVal value As String)
            _attribute02 = value
        End Set
    End Property
    Public Property Attribute02Action() As String
        Get
            Return _attribute02Action
        End Get
        Set(ByVal value As String)
            _attribute02Action = value
        End Set
    End Property
    Public Property Attribute03() As String
        Get
            Return _attribute03
        End Get
        Set(ByVal value As String)
            _attribute03 = value
        End Set
    End Property
    Public Property Attribute03Action() As String
        Get
            Return _attribute03Action
        End Get
        Set(ByVal value As String)
            _attribute03Action = value
        End Set
    End Property
    Public Property Attribute04() As String
        Get
            Return _attribute04
        End Get
        Set(ByVal value As String)
            _attribute04 = value
        End Set
    End Property
    Public Property Attribute04Action() As String
        Get
            Return _attribute04Action
        End Get
        Set(ByVal value As String)
            _attribute04Action = value
        End Set
    End Property
    Public Property Attribute05() As String
        Get
            Return _attribute05
        End Get
        Set(ByVal value As String)
            _attribute05 = value
        End Set
    End Property
    Public Property Attribute05Action() As String
        Get
            Return _attribute05Action
        End Get
        Set(ByVal value As String)
            _attribute05Action = value
        End Set
    End Property
    Public Property Attribute06() As String
        Get
            Return _attribute06
        End Get
        Set(ByVal value As String)
            _attribute06 = value
        End Set
    End Property
    Public Property Attribute06Action() As String
        Get
            Return _attribute06Action
        End Get
        Set(ByVal value As String)
            _attribute06Action = value
        End Set
    End Property
    Public Property Attribute07() As String
        Get
            Return _attribute07
        End Get
        Set(ByVal value As String)
            _attribute07 = value
        End Set
    End Property
    Public Property Attribute07Action() As String
        Get
            Return _attribute07Action
        End Get
        Set(ByVal value As String)
            _attribute07Action = value
        End Set
    End Property
    Public Property Attribute08() As String
        Get
            Return _attribute08
        End Get
        Set(ByVal value As String)
            _attribute08 = value
        End Set
    End Property
    Public Property Attribute08Action() As String
        Get
            Return _attribute08Action
        End Get
        Set(ByVal value As String)
            _attribute08Action = value
        End Set
    End Property
    Public Property Attribute09() As String
        Get
            Return _attribute09
        End Get
        Set(ByVal value As String)
            _attribute09 = value
        End Set
    End Property
    Public Property Attribute09Action() As String
        Get
            Return _attribute09Action
        End Get
        Set(ByVal value As String)
            _attribute09Action = value
        End Set
    End Property
    Public Property Attribute10() As String
        Get
            Return _attribute10
        End Get
        Set(ByVal value As String)
            _attribute10 = value
        End Set
    End Property
    Public Property Attribute10Action() As String
        Get
            Return _attribute10Action
        End Get
        Set(ByVal value As String)
            _attribute10Action = value
        End Set
    End Property
    Public Property Attribute11() As String
        Get
            Return _attribute11
        End Get
        Set(ByVal value As String)
            _attribute11 = value
        End Set
    End Property
    Public Property Attribute11Action() As String
        Get
            Return _attribute11Action
        End Get
        Set(ByVal value As String)
            _attribute11Action = value
        End Set
    End Property
    Public Property Attribute12() As String
        Get
            Return _attribute12
        End Get
        Set(ByVal value As String)
            _attribute12 = value
        End Set
    End Property
    Public Property Attribute12Action() As String
        Get
            Return _attribute12Action
        End Get
        Set(ByVal value As String)
            _attribute12Action = value
        End Set
    End Property
    Public Property Attribute13() As String
        Get
            Return _attribute13
        End Get
        Set(ByVal value As String)
            _attribute13 = value
        End Set
    End Property
    Public Property Attribute13Action() As String
        Get
            Return _attribute13Action
        End Get
        Set(ByVal value As String)
            _attribute13Action = value
        End Set
    End Property
    Public Property Attribute14() As String
        Get
            Return _attribute14
        End Get
        Set(ByVal value As String)
            _attribute14 = value
        End Set
    End Property
    Public Property Attribute14Action() As String
        Get
            Return _attribute14Action
        End Get
        Set(ByVal value As String)
            _attribute14Action = value
        End Set
    End Property
    Public Property Attribute15() As String
        Get
            Return _attribute15
        End Get
        Set(ByVal value As String)
            _attribute15 = value
        End Set
    End Property
    Public Property Attribute15Action() As String
        Get
            Return _attribute15Action
        End Get
        Set(ByVal value As String)
            _attribute15Action = value
        End Set
    End Property
    Public Property Attribute16() As String
        Get
            Return _attribute16
        End Get
        Set(ByVal value As String)
            _attribute16 = value
        End Set
    End Property
    Public Property Attribute16Action() As String
        Get
            Return _attribute16Action
        End Get
        Set(ByVal value As String)
            _attribute16Action = value
        End Set
    End Property
    Public Property Attribute17() As String
        Get
            Return _attribute17
        End Get
        Set(ByVal value As String)
            _attribute17 = value
        End Set
    End Property
    Public Property Attribute17Action() As String
        Get
            Return _attribute17Action
        End Get
        Set(ByVal value As String)
            _attribute17Action = value
        End Set
    End Property
    Public Property Attribute18() As String
        Get
            Return _attribute18
        End Get
        Set(ByVal value As String)
            _attribute18 = value
        End Set
    End Property
    Public Property Attribute18Action() As String
        Get
            Return _attribute18Action
        End Get
        Set(ByVal value As String)
            _attribute18Action = value
        End Set
    End Property
    Public Property Attribute19() As String
        Get
            Return _attribute19
        End Get
        Set(ByVal value As String)
            _attribute19 = value
        End Set
    End Property
    Public Property Attribute19Action() As String
        Get
            Return _attribute19Action
        End Get
        Set(ByVal value As String)
            _attribute19Action = value
        End Set
    End Property
    Public Property Attribute20() As String
        Get
            Return _attribute20
        End Get
        Set(ByVal value As String)
            _attribute20 = value
        End Set
    End Property
    Public Property Attribute20Action() As String
        Get
            Return _attribute20Action
        End Get
        Set(ByVal value As String)
            _attribute20Action = value
        End Set
    End Property
    Public Property Attribute21() As String
        Get
            Return _attribute21
        End Get
        Set(ByVal value As String)
            _attribute21 = value
        End Set
    End Property
    Public Property Attribute21Action() As String
        Get
            Return _attribute21Action
        End Get
        Set(ByVal value As String)
            _attribute21Action = value
        End Set
    End Property
    Public Property Attribute22() As String
        Get
            Return _attribute22
        End Get
        Set(ByVal value As String)
            _attribute22 = value
        End Set
    End Property
    Public Property Attribute22Action() As String
        Get
            Return _attribute22Action
        End Get
        Set(ByVal value As String)
            _attribute22Action = value
        End Set
    End Property
    Public Property Attribute23() As String
        Get
            Return _attribute23
        End Get
        Set(ByVal value As String)
            _attribute23 = value
        End Set
    End Property
    Public Property Attribute23Action() As String
        Get
            Return _attribute23Action
        End Get
        Set(ByVal value As String)
            _attribute23Action = value
        End Set
    End Property
    Public Property Attribute24() As String
        Get
            Return _attribute24
        End Get
        Set(ByVal value As String)
            _attribute24 = value
        End Set
    End Property
    Public Property Attribute24Action() As String
        Get
            Return _attribute24Action
        End Get
        Set(ByVal value As String)
            _attribute24Action = value
        End Set
    End Property
    Public Property Attribute25() As String
        Get
            Return _attribute25
        End Get
        Set(ByVal value As String)
            _attribute25 = value
        End Set
    End Property
    Public Property Attribute25Action() As String
        Get
            Return _attribute25Action
        End Get
        Set(ByVal value As String)
            _attribute25Action = value
        End Set
    End Property
    Public Property Attribute26() As String
        Get
            Return _attribute26
        End Get
        Set(ByVal value As String)
            _attribute26 = value
        End Set
    End Property
    Public Property Attribute26Action() As String
        Get
            Return _attribute26Action
        End Get
        Set(ByVal value As String)
            _attribute26Action = value
        End Set
    End Property
    Public Property Attribute27() As String
        Get
            Return _attribute27
        End Get
        Set(ByVal value As String)
            _attribute27 = value
        End Set
    End Property
    Public Property Attribute27Action() As String
        Get
            Return _attribute27Action
        End Get
        Set(ByVal value As String)
            _attribute27Action = value
        End Set
    End Property
    Public Property Attribute28() As String
        Get
            Return _attribute28
        End Get
        Set(ByVal value As String)
            _attribute28 = value
        End Set
    End Property
    Public Property Attribute28Action() As String
        Get
            Return _attribute28Action
        End Get
        Set(ByVal value As String)
            _attribute28Action = value
        End Set
    End Property
    Public Property Attribute29() As String
        Get
            Return _attribute29
        End Get
        Set(ByVal value As String)
            _attribute29 = value
        End Set
    End Property
    Public Property Attribute29Action() As String
        Get
            Return _attribute29Action
        End Get
        Set(ByVal value As String)
            _attribute29Action = value
        End Set
    End Property
    Public Property Attribute30() As String
        Get
            Return _attribute30
        End Get
        Set(ByVal value As String)
            _attribute30 = value
        End Set
    End Property
    Public Property Attribute30Action() As String
        Get
            Return _attribute30Action
        End Get
        Set(ByVal value As String)
            _attribute30Action = value
        End Set
    End Property
    Public Property Attribute31() As String
        Get
            Return _attribute31
        End Get
        Set(ByVal value As String)
            _attribute31 = value
        End Set
    End Property
    Public Property Attribute31Action() As String
        Get
            Return _attribute31Action
        End Get
        Set(ByVal value As String)
            _attribute31Action = value
        End Set
    End Property
    Public Property Attribute32() As String
        Get
            Return _attribute32
        End Get
        Set(ByVal value As String)
            _attribute32 = value
        End Set
    End Property
    Public Property Attribute32Action() As String
        Get
            Return _attribute32Action
        End Get
        Set(ByVal value As String)
            _attribute32Action = value
        End Set
    End Property
    Public Property Attribute33() As String
        Get
            Return _attribute33
        End Get
        Set(ByVal value As String)
            _attribute33 = value
        End Set
    End Property
    Public Property Attribute33Action() As String
        Get
            Return _attribute33Action
        End Get
        Set(ByVal value As String)
            _attribute33Action = value
        End Set
    End Property
    Public Property Attribute34() As String
        Get
            Return _attribute34
        End Get
        Set(ByVal value As String)
            _attribute34 = value
        End Set
    End Property
    Public Property Attribute34Action() As String
        Get
            Return _attribute34Action
        End Get
        Set(ByVal value As String)
            _attribute34Action = value
        End Set
    End Property
    Public Property Attribute35() As String
        Get
            Return _attribute35
        End Get
        Set(ByVal value As String)
            _attribute35 = value
        End Set
    End Property
    Public Property Attribute35Action() As String
        Get
            Return _attribute35Action
        End Get
        Set(ByVal value As String)
            _attribute35Action = value
        End Set
    End Property
    Public Property Attribute36() As String
        Get
            Return _attribute36
        End Get
        Set(ByVal value As String)
            _attribute36 = value
        End Set
    End Property
    Public Property Attribute36Action() As String
        Get
            Return _attribute36Action
        End Get
        Set(ByVal value As String)
            _attribute36Action = value
        End Set
    End Property
    Public Property Attribute37() As String
        Get
            Return _attribute37
        End Get
        Set(ByVal value As String)
            _attribute37 = value
        End Set
    End Property
    Public Property Attribute37Action() As String
        Get
            Return _attribute37Action
        End Get
        Set(ByVal value As String)
            _attribute37Action = value
        End Set
    End Property
    Public Property Attribute38() As String
        Get
            Return _attribute38
        End Get
        Set(ByVal value As String)
            _attribute38 = value
        End Set
    End Property
    Public Property Attribute38Action() As String
        Get
            Return _attribute38Action
        End Get
        Set(ByVal value As String)
            _attribute38Action = value
        End Set
    End Property
    Public Property Attribute39() As String
        Get
            Return _attribute39
        End Get
        Set(ByVal value As String)
            _attribute39 = value
        End Set
    End Property
    Public Property Attribute39Action() As String
        Get
            Return _attribute39Action
        End Get
        Set(ByVal value As String)
            _attribute39Action = value
        End Set
    End Property
    Public Property Attribute40() As String
        Get
            Return _attribute40
        End Get
        Set(ByVal value As String)
            _attribute40 = value
        End Set
    End Property
    Public Property Attribute40Action() As String
        Get
            Return _attribute40Action
        End Get
        Set(ByVal value As String)
            _attribute40Action = value
        End Set
    End Property
    Public Property Attribute41() As String
        Get
            Return _attribute41
        End Get
        Set(ByVal value As String)
            _attribute41 = value
        End Set
    End Property
    Public Property Attribute41Action() As String
        Get
            Return _attribute41Action
        End Get
        Set(ByVal value As String)
            _attribute41Action = value
        End Set
    End Property
    Public Property Attribute42() As String
        Get
            Return _attribute42
        End Get
        Set(ByVal value As String)
            _attribute42 = value
        End Set
    End Property
    Public Property Attribute42Action() As String
        Get
            Return _attribute42Action
        End Get
        Set(ByVal value As String)
            _attribute42Action = value
        End Set
    End Property
    Public Property Attribute43() As String
        Get
            Return _attribute43
        End Get
        Set(ByVal value As String)
            _attribute43 = value
        End Set
    End Property
    Public Property Attribute43Action() As String
        Get
            Return _attribute43Action
        End Get
        Set(ByVal value As String)
            _attribute43Action = value
        End Set
    End Property
    Public Property Attribute44() As String
        Get
            Return _attribute44
        End Get
        Set(ByVal value As String)
            _attribute44 = value
        End Set
    End Property
    Public Property Attribute44Action() As String
        Get
            Return _attribute44Action
        End Get
        Set(ByVal value As String)
            _attribute44Action = value
        End Set
    End Property
    Public Property Attribute45() As String
        Get
            Return _attribute45
        End Get
        Set(ByVal value As String)
            _attribute45 = value
        End Set
    End Property
    Public Property Attribute45Action() As String
        Get
            Return _attribute45Action
        End Get
        Set(ByVal value As String)
            _attribute45Action = value
        End Set
    End Property
    Public Property Attribute46() As String
        Get
            Return _attribute46
        End Get
        Set(ByVal value As String)
            _attribute46 = value
        End Set
    End Property
    Public Property Attribute46Action() As String
        Get
            Return _attribute46Action
        End Get
        Set(ByVal value As String)
            _attribute46Action = value
        End Set
    End Property
    Public Property Attribute47() As String
        Get
            Return _attribute47
        End Get
        Set(ByVal value As String)
            _attribute47 = value
        End Set
    End Property
    Public Property Attribute47Action() As String
        Get
            Return _attribute47Action
        End Get
        Set(ByVal value As String)
            _attribute47Action = value
        End Set
    End Property
    Public Property Attribute48() As String
        Get
            Return _attribute48
        End Get
        Set(ByVal value As String)
            _attribute48 = value
        End Set
    End Property
    Public Property Attribute48Action() As String
        Get
            Return _attribute48Action
        End Get
        Set(ByVal value As String)
            _attribute48Action = value
        End Set
    End Property
    Public Property Attribute49() As String
        Get
            Return _attribute49
        End Get
        Set(ByVal value As String)
            _attribute49 = value
        End Set
    End Property
    Public Property Attribute49Action() As String
        Get
            Return _attribute49Action
        End Get
        Set(ByVal value As String)
            _attribute49Action = value
        End Set
    End Property
    Public Property Attribute50() As String
        Get
            Return _attribute50
        End Get
        Set(ByVal value As String)
            _attribute50 = value
        End Set
    End Property
    Public Property Attribute50Action() As String
        Get
            Return _attribute50Action
        End Get
        Set(ByVal value As String)
            _attribute50Action = value
        End Set
    End Property
    Public Property Attribute51() As String
        Get
            Return _attribute51
        End Get
        Set(ByVal value As String)
            _attribute51 = value
        End Set
    End Property
    Public Property Attribute51Action() As String
        Get
            Return _attribute51Action
        End Get
        Set(ByVal value As String)
            _attribute51Action = value
        End Set
    End Property
    Public Property Attribute52() As String
        Get
            Return _attribute52
        End Get
        Set(ByVal value As String)
            _attribute52 = value
        End Set
    End Property
    Public Property Attribute52Action() As String
        Get
            Return _attribute52Action
        End Get
        Set(ByVal value As String)
            _attribute52Action = value
        End Set
    End Property
    Public Property Attribute53() As String
        Get
            Return _attribute53
        End Get
        Set(ByVal value As String)
            _attribute53 = value
        End Set
    End Property
    Public Property Attribute53Action() As String
        Get
            Return _attribute53Action
        End Get
        Set(ByVal value As String)
            _attribute53Action = value
        End Set
    End Property
    Public Property Attribute54() As String
        Get
            Return _attribute54
        End Get
        Set(ByVal value As String)
            _attribute54 = value
        End Set
    End Property
    Public Property Attribute54Action() As String
        Get
            Return _attribute54Action
        End Get
        Set(ByVal value As String)
            _attribute54Action = value
        End Set
    End Property
    Public Property Attribute55() As String
        Get
            Return _attribute55
        End Get
        Set(ByVal value As String)
            _attribute55 = value
        End Set
    End Property
    Public Property Attribute55Action() As String
        Get
            Return _attribute55Action
        End Get
        Set(ByVal value As String)
            _attribute55Action = value
        End Set
    End Property
    Public Property Attribute56() As String
        Get
            Return _attribute56
        End Get
        Set(ByVal value As String)
            _attribute56 = value
        End Set
    End Property
    Public Property Attribute56Action() As String
        Get
            Return _attribute56Action
        End Get
        Set(ByVal value As String)
            _attribute56Action = value
        End Set
    End Property
    Public Property Attribute57() As String
        Get
            Return _attribute57
        End Get
        Set(ByVal value As String)
            _attribute57 = value
        End Set
    End Property
    Public Property Attribute57Action() As String
        Get
            Return _attribute57Action
        End Get
        Set(ByVal value As String)
            _attribute57Action = value
        End Set
    End Property
    Public Property Attribute58() As String
        Get
            Return _attribute58
        End Get
        Set(ByVal value As String)
            _attribute58 = value
        End Set
    End Property
    Public Property Attribute58Action() As String
        Get
            Return _attribute58Action
        End Get
        Set(ByVal value As String)
            _attribute58Action = value
        End Set
    End Property
    Public Property Attribute59() As String
        Get
            Return _attribute59
        End Get
        Set(ByVal value As String)
            _attribute59 = value
        End Set
    End Property
    Public Property Attribute59Action() As String
        Get
            Return _attribute59Action
        End Get
        Set(ByVal value As String)
            _attribute59Action = value
        End Set
    End Property
    Public Property Attribute60() As String
        Get
            Return _attribute60
        End Get
        Set(ByVal value As String)
            _attribute60 = value
        End Set
    End Property
    Public Property Attribute60Action() As String
        Get
            Return _attribute60Action
        End Get
        Set(ByVal value As String)
            _attribute60Action = value
        End Set
    End Property

    Public Property ActionCode() As String
        Get
            Return _actionCode
        End Get
        Set(ByVal value As String)
            _actionCode = value
        End Set
    End Property
    Public Property ActionCodeFixed() As String
        Get
            Return _actionCodeFixed
        End Get
        Set(ByVal value As String)
            _actionCodeFixed = value
        End Set
    End Property
    Public Property ActionStatus() As String
        Get
            Return _actionStatus
        End Get
        Set(ByVal value As String)
            _actionStatus = value
        End Set
    End Property
    Public Property ActionPty() As String
        Get
            Return _actionPty
        End Get
        Set(ByVal value As String)
            _actionPty = value
        End Set
    End Property
    Public Property ActionAgent() As String
        Get
            Return _actionAgent
        End Get
        Set(ByVal value As String)
            _actionAgent = value
        End Set
    End Property
    Public Property ActionComment01() As String
        Get
            Return _actionComment01
        End Get
        Set(ByVal value As String)
            _actionComment01 = value
        End Set
    End Property
    Public Property ActionComment02() As String
        Get
            Return _actionComment02
        End Get
        Set(ByVal value As String)
            _actionComment02 = value
        End Set
    End Property
    Public Property ActionComment03() As String
        Get
            Return _actionComment03
        End Get
        Set(ByVal value As String)
            _actionComment03 = value
        End Set
    End Property
    Public Property ActionComment04() As String
        Get
            Return _actionComment04
        End Get
        Set(ByVal value As String)
            _actionComment04 = value
        End Set
    End Property
    Public Property ActionComment05() As String
        Get
            Return _actionComment05
        End Get
        Set(ByVal value As String)
            _actionComment05 = value
        End Set
    End Property
    Public Property ActionComment06() As String
        Get
            Return _actionComment06
        End Get
        Set(ByVal value As String)
            _actionComment06 = value
        End Set
    End Property
    Public Property ActionComment07() As String
        Get
            Return _actionComment07
        End Get
        Set(ByVal value As String)
            _actionComment07 = value
        End Set
    End Property
    Public Property ActionComment08() As String
        Get
            Return _actionComment08
        End Get
        Set(ByVal value As String)
            _actionComment08 = value
        End Set
    End Property
    Public Property ActionComment09() As String
        Get
            Return _actionComment09
        End Get
        Set(ByVal value As String)
            _actionComment09 = value
        End Set
    End Property
    Public Property ActionComment10() As String
        Get
            Return _actionComment10
        End Get
        Set(ByVal value As String)
            _actionComment10 = value
        End Set
    End Property
    Public Property ActionComment11() As String
        Get
            Return _actionComment11
        End Get
        Set(ByVal value As String)
            _actionComment11 = value
        End Set
    End Property
    Public Property ActionComment12() As String
        Get
            Return _actionComment12
        End Get
        Set(ByVal value As String)
            _actionComment12 = value
        End Set
    End Property
    Public Property ActionComment13() As String
        Get
            Return _actionComment13
        End Get
        Set(ByVal value As String)
            _actionComment13 = value
        End Set
    End Property
    Public Property ActionComment14() As String
        Get
            Return _actionComment14
        End Get
        Set(ByVal value As String)
            _actionComment14 = value
        End Set
    End Property
    Public Property ActionComment15() As String
        Get
            Return _actionComment15
        End Get
        Set(ByVal value As String)
            _actionComment15 = value
        End Set
    End Property
    Public Property ActionComment16() As String
        Get
            Return _actionComment16
        End Get
        Set(ByVal value As String)
            _actionComment16 = value
        End Set
    End Property
    Public Property ActionDate() As String
        Get
            Return _actionDate
        End Get
        Set(ByVal value As String)
            _actionDate = value
        End Set
    End Property
    Public Property ActionID() As String
        Get
            Return _actionID
        End Get
        Set(ByVal value As String)
            _actionID = value
        End Set
    End Property
    Public Property ActionSubject() As String
        Get
            Return _actionSubject
        End Get
        Set(ByVal value As String)
            _actionSubject = value
        End Set
    End Property
    Public Property ActionMemo() As String
        Get
            Return _actionMemo
        End Get
        Set(ByVal value As String)
            _actionMemo = value
        End Set
    End Property
    Public Property ActionDepartmentID() As String
        Get
            Return _actionDepartmentID
        End Get
        Set(ByVal value As String)
            _actionDepartmentID = value
        End Set
    End Property
    Public Property ActionProductID() As String
        Get
            Return _actionProductID
        End Get
        Set(ByVal value As String)
            _actionProductID = value
        End Set
    End Property
    Public Property ActionProjectID() As String
        Get
            Return _actionProjectID
        End Get
        Set(ByVal value As String)
            _actionProjectID = value
        End Set
    End Property
    Public Property ActionCampaignID() As String
        Get
            Return _actionCampaignID
        End Get
        Set(ByVal value As String)
            _actionCampaignID = value
        End Set
    End Property
    Public Property ActionField1() As String
        Get
            Return _actionField1
        End Get
        Set(ByVal value As String)
            _actionField1 = value
        End Set
    End Property
    Public Property ActionField2() As String
        Get
            Return _actionField2
        End Get
        Set(ByVal value As String)
            _actionField2 = value
        End Set
    End Property
    Public Property ActionField3() As String
        Get
            Return _actionField3
        End Get
        Set(ByVal value As String)
            _actionField3 = value
        End Set
    End Property
    Public Property ActionField4() As String
        Get
            Return _actionField4
        End Get
        Set(ByVal value As String)
            _actionField4 = value
        End Set
    End Property
    Public Property ActionField5() As String
        Get
            Return _actionField5
        End Get
        Set(ByVal value As String)
            _actionField5 = value
        End Set
    End Property
    Public Property NoteCode() As String
        Get
            Return _noteCode
        End Get
        Set(ByVal value As String)
            _noteCode = value
        End Set
    End Property
    Public Property NoteComment01() As String
        Get
            Return _noteComment01
        End Get
        Set(ByVal value As String)
            _noteComment01 = value
        End Set
    End Property
    Public Property NoteComment02() As String
        Get
            Return _noteComment02
        End Get
        Set(ByVal value As String)
            _noteComment02 = value
        End Set
    End Property
    Public Property NoteComment03() As String
        Get
            Return _noteComment03
        End Get
        Set(ByVal value As String)
            _noteComment03 = value
        End Set
    End Property
    Public Property NoteComment04() As String
        Get
            Return _noteComment04
        End Get
        Set(ByVal value As String)
            _noteComment04 = value
        End Set
    End Property
    Public Property NoteComment05() As String
        Get
            Return _noteComment05
        End Get
        Set(ByVal value As String)
            _noteComment05 = value
        End Set
    End Property
    Public Property NoteComment06() As String
        Get
            Return _noteComment06
        End Get
        Set(ByVal value As String)
            _noteComment06 = value
        End Set
    End Property
    Public Property NoteComment07() As String
        Get
            Return _noteComment07
        End Get
        Set(ByVal value As String)
            _noteComment07 = value
        End Set
    End Property
    Public Property NoteComment08() As String
        Get
            Return _noteComment08
        End Get
        Set(ByVal value As String)
            _noteComment08 = value
        End Set
    End Property
    Public Property NoteComment09() As String
        Get
            Return _noteComment09
        End Get
        Set(ByVal value As String)
            _noteComment09 = value
        End Set
    End Property
    Public Property NoteComment10() As String
        Get
            Return _noteComment10
        End Get
        Set(ByVal value As String)
            _noteComment10 = value
        End Set
    End Property
    Public Property NoteComment11() As String
        Get
            Return _noteComment11
        End Get
        Set(ByVal value As String)
            _noteComment11 = value
        End Set
    End Property
    Public Property NoteComment12() As String
        Get
            Return _noteComment12
        End Get
        Set(ByVal value As String)
            _noteComment12 = value
        End Set
    End Property
    Public Property NoteComment13() As String
        Get
            Return _noteComment13
        End Get
        Set(ByVal value As String)
            _noteComment13 = value
        End Set
    End Property
    Public Property NoteComment14() As String
        Get
            Return _noteComment14
        End Get
        Set(ByVal value As String)
            _noteComment14 = value
        End Set
    End Property
    Public Property NoteComment15() As String
        Get
            Return _noteComment15
        End Get
        Set(ByVal value As String)
            _noteComment15 = value
        End Set
    End Property
    Public Property NoteComment16() As String
        Get
            Return _noteComment16
        End Get
        Set(ByVal value As String)
            _noteComment16 = value
        End Set
    End Property
    Public Property ErrorCode() As String
        Get
            Return _errorCode
        End Get
        Set(ByVal value As String)
            _errorCode = value
        End Set
    End Property
    Public Property ErrorFlag() As String
        Get
            Return _errorFlag
        End Get
        Set(ByVal value As String)
            _errorFlag = value
        End Set
    End Property
    Public Property Category() As String
        Get
            Return _category
        End Get
        Set(ByVal value As String)
            _category = value
        End Set
    End Property

    Public Property PostCodeFormatting() As String
        Get
            Return _postCodeFormatting
        End Get
        Set(ByVal value As String)
            _postCodeFormatting = value
        End Set
    End Property
    Public Property PhoneNoFormatting() As String
        Get
            Return _phoneNoFormatting
        End Get
        Set(ByVal value As String)
            _phoneNoFormatting = value
        End Set
    End Property

    Private _oldPassword As String
    Public Property OldPassword() As String
        Get
            Return _oldPassword
        End Get
        Set(ByVal value As String)
            _oldPassword = value
        End Set
    End Property
    Private _newPassword As String
    Public Property NewPassword() As String
        Get
            Return _newPassword
        End Get
        Set(ByVal value As String)
            _newPassword = value
        End Set
    End Property
    Private _saltString As String
    Public Property SaltString() As String
        Get
            Return _saltString
        End Get
        Set(ByVal value As String)
            _saltString = value
        End Set
    End Property


    Private _passportNo As String = String.Empty
    Public Property PassportNumber() As String
        Get
            Return _passportNo
        End Get
        Set(ByVal value As String)
            _passportNo = value
        End Set
    End Property

    Private _greenCardNo As String = String.Empty
    Public Property GreenCardNumber() As String
        Get
            Return _greenCardNo
        End Get
        Set(ByVal value As String)
            _greenCardNo = value
        End Set
    End Property

    Private _PINNo As String = String.Empty
    Public Property PIN_Number() As String
        Get
            Return _PINNo
        End Get
        Set(ByVal value As String)
            _PINNo = value
        End Set
    End Property

    Private _userID4 As String
    Public Property User_ID_4() As String
        Get
            Return _userID4
        End Get
        Set(ByVal value As String)
            _userID4 = value
        End Set
    End Property

    Private _userID5 As String
    Public Property User_ID_5() As String
        Get
            Return _userID5
        End Get
        Set(ByVal value As String)
            _userID5 = value
        End Set
    End Property

    Private _userID6 As String
    Public Property User_ID_6() As String
        Get
            Return _userID6
        End Get
        Set(ByVal value As String)
            _userID6 = value
        End Set
    End Property

    Private _userID7 As String
    Public Property User_ID_7() As String
        Get
            Return _userID7
        End Get
        Set(ByVal value As String)
            _userID7 = value
        End Set
    End Property

    Private _userID8 As String
    Public Property User_ID_8() As String
        Get
            Return _userID8
        End Get
        Set(ByVal value As String)
            _userID8 = value
        End Set
    End Property

    Private _userID9 As String
    Public Property User_ID_9() As String
        Get
            Return _userID9
        End Get
        Set(ByVal value As String)
            _userID9 = value
        End Set
    End Property

    Private _SUPPORTER_CLUB_CODE As String
    Public Property SUPPORTER_CLUB_CODE() As String
        Get
            Return _SUPPORTER_CLUB_CODE
        End Get
        Set(ByVal value As String)
            _SUPPORTER_CLUB_CODE = value
        End Set
    End Property

    Private _FAVOURITE_TEAM_CODE As String
    Public Property FAVOURITE_TEAM_CODE() As String
        Get
            Return _FAVOURITE_TEAM_CODE
        End Get
        Set(ByVal value As String)
            _FAVOURITE_TEAM_CODE = value
        End Set
    End Property

    Private _FAVOURITE_SPORT As String
    Public Property FAVOURITE_SPORT() As String
        Get
            Return _FAVOURITE_SPORT
        End Get
        Set(ByVal value As String)
            _FAVOURITE_SPORT = value
        End Set
    End Property

    Private _MAIL_TEAM_CODE_1 As String
    Public Property MAIL_TEAM_CODE_1() As String
        Get
            Return _MAIL_TEAM_CODE_1
        End Get
        Set(ByVal value As String)
            _MAIL_TEAM_CODE_1 = value
        End Set
    End Property

    Private _MAIL_TEAM_CODE_2 As String
    Public Property MAIL_TEAM_CODE_2() As String
        Get
            Return _MAIL_TEAM_CODE_2
        End Get
        Set(ByVal value As String)
            _MAIL_TEAM_CODE_2 = value
        End Set
    End Property

    Private _MAIL_TEAM_CODE_3 As String
    Public Property MAIL_TEAM_CODE_3() As String
        Get
            Return _MAIL_TEAM_CODE_3
        End Get
        Set(ByVal value As String)
            _MAIL_TEAM_CODE_3 = value
        End Set
    End Property

    Private _MAIL_TEAM_CODE_4 As String
    Public Property MAIL_TEAM_CODE_4() As String
        Get
            Return _MAIL_TEAM_CODE_4
        End Get
        Set(ByVal value As String)
            _MAIL_TEAM_CODE_4 = value
        End Set
    End Property

    Private _MAIL_TEAM_CODE_5 As String
    Public Property MAIL_TEAM_CODE_5() As String
        Get
            Return _MAIL_TEAM_CODE_5
        End Get
        Set(ByVal value As String)
            _MAIL_TEAM_CODE_5 = value
        End Set
    End Property

    Private _PREFERRED_CONTACT_METHOD As String
    Public Property PREFERRED_CONTACT_METHOD() As String
        Get
            Return _PREFERRED_CONTACT_METHOD
        End Get
        Set(ByVal value As String)
            _PREFERRED_CONTACT_METHOD = value
        End Set
    End Property

    Private _hashedPassword As String
    Public Property HashedPassword() As String
        Get
            Return _hashedPassword
        End Get
        Set(ByVal value As String)
            _hashedPassword = value
        End Set
    End Property

    Public Property UseEncryptedPassword() As Boolean
        Get
            Return _useEncryptedPassword
        End Get
        Set(ByVal value As Boolean)
            _useEncryptedPassword = value
        End Set
    End Property

    Private _encryptionType As String
    Public Property EncryptionType() As String
        Get
            Return _encryptionType
        End Get
        Set(ByVal value As String)
            _encryptionType = value
        End Set
    End Property

    Private _oldHashedPassword As String
    Public Property OldHashedPassword() As String
        Get
            Return _oldHashedPassword
        End Get
        Set(ByVal value As String)
            _oldHashedPassword = value
        End Set
    End Property
    Private _newHashedPassword As String
    Public Property NewHashedPassword() As String
        Get
            Return _newHashedPassword
        End Get
        Set(ByVal value As String)
            _newHashedPassword = value
        End Set
    End Property

    Private _updateCompanyInformation As String
    Public Property UpdateCompanyInformation() As String
        Get
            Return _updateCompanyInformation
        End Get
        Set(ByVal value As String)
            _updateCompanyInformation = value
        End Set
    End Property

    Private _regAddress1 As String
    Public Property RegisteredAddress1() As String
        Get
            Return _regAddress1
        End Get
        Set(ByVal value As String)
            _regAddress1 = value
        End Set
    End Property
    Private _regAddress2 As String
    Public Property RegisteredAddress2() As String
        Get
            Return _regAddress2
        End Get
        Set(ByVal value As String)
            _regAddress2 = value
        End Set
    End Property
    Private _regAddress3 As String
    Public Property RegisteredAddress3() As String
        Get
            Return _regAddress3
        End Get
        Set(ByVal value As String)
            _regAddress3 = value
        End Set
    End Property
    Private _regAddress4 As String
    Public Property RegisteredAddress4() As String
        Get
            Return _regAddress4
        End Get
        Set(ByVal value As String)
            _regAddress4 = value
        End Set
    End Property
    Private _regAddress5 As String
    Public Property RegisteredAddress5() As String
        Get
            Return _regAddress5
        End Get
        Set(ByVal value As String)
            _regAddress5 = value
        End Set
    End Property

    Private _regPostcode As String
    Public Property RegisteredPostcode() As String
        Get
            Return _regPostcode
        End Get
        Set(ByVal value As String)
            _regPostcode = value
        End Set
    End Property

    Private _regCountry As String
    Public Property RegisteredCountry() As String
        Get
            Return _regCountry
        End Get
        Set(ByVal value As String)
            _regCountry = value
        End Set
    End Property

    Private _mothersname As String
    Public Property MothersName() As String
        Get
            Return _mothersname
        End Get
        Set(ByVal value As String)
            _mothersname = value
        End Set
    End Property

    Private _fathersname As String
    Public Property FathersName() As String
        Get
            Return _fathersname
        End Get
        Set(ByVal value As String)
            _fathersname = value
        End Set
    End Property


    Private _agent As String
    Public Property Agent() As String
        Get
            Return _agent
        End Get
        Set(ByVal value As String)
            _agent = value
        End Set
    End Property

    Private _fanFlag As String = String.Empty
    Public Property FanFlag() As String
        Get
            Return _fanFlag
        End Get
        Set(ByVal value As String)
            _fanFlag = value
        End Set
    End Property

    Private _emergencyContactName As String = String.Empty
    Public Property EmergencyContactName() As String
        Get
            Return _emergencyContactName
        End Get
        Set(ByVal value As String)
            _emergencyContactName = value
        End Set
    End Property

    Private _emergencyContactNumber As String = String.Empty
    Public Property EmergencyContactNumber() As String
        Get
            Return _emergencyContactNumber
        End Get
        Set(ByVal value As String)
            _emergencyContactNumber = value
        End Set
    End Property

    Private _medicalInformation As String = String.Empty
    Public Property MedicalInformation() As String
        Get
            Return _medicalInformation
        End Get
        Set(ByVal value As String)
            _medicalInformation = value
        End Set
    End Property

    Private _favouriteStand As String = String.Empty
    Public Property FavouriteStand() As String
        Get
            Return _favouriteStand
        End Get
        Set(ByVal value As String)
            _favouriteStand = value
        End Set
    End Property

    Private _favouriteArea As String = String.Empty
    Public Property FavouriteArea() As String
        Get
            Return _favouriteArea
        End Get
        Set(ByVal value As String)
            _favouriteArea = value
        End Set
    End Property

    Private _favouriteRow As String = String.Empty
    Public Property FavouriteRow() As String
        Get
            Return _favouriteRow
        End Get
        Set(ByVal value As String)
            _favouriteRow = value
        End Set
    End Property

    Private _favouriteSeat As String = String.Empty
    Public Property FavouriteSeat() As String
        Get
            Return _favouriteSeat
        End Get
        Set(ByVal value As String)
            _favouriteSeat = value
        End Set
    End Property

    Private _id As String = String.Empty
    Public Property ID() As String
        Get
            Return _id
        End Get
        Set(ByVal value As String)
            _id = value
        End Set
    End Property

    Private _idType As String = String.Empty
    Public Property IDType() As String
        Get
            Return _idType
        End Get
        Set(ByVal value As String)
            _idType = value
        End Set
    End Property

    Public Property SingleFieldMode() As SingleModeFieldsEnum

    Private _retreiveOnlyMode As Boolean = False
    Public Property RetreiveOnlyMode() As Boolean
        Get
            Return _retreiveOnlyMode
        End Get
        Set(ByVal value As Boolean)
            _retreiveOnlyMode = value
        End Set
    End Property

    'Special Attributes for User Alerts
    Private _birthdayAlertEnabled As Boolean = False
    Public Property BirthdayAlertEnabled() As Boolean
        Get
            Return _birthdayAlertEnabled
        End Get
        Set(ByVal value As Boolean)
            _birthdayAlertEnabled = value
        End Set
    End Property

    Private _fAndFBirthdayAlertEnabled As Boolean = False
    Public Property FAndFBirthdayAlertEnabled() As Boolean
        Get
            Return _fAndFBirthdayAlertEnabled
        End Get
        Set(ByVal value As Boolean)
            _fAndFBirthdayAlertEnabled = value
        End Set
    End Property

    Private _cardExpiryPPSAlertEnabled As Boolean = False
    Public Property CardExpiryPPSAlertEnabled() As Boolean
        Get
            Return _cardExpiryPPSAlertEnabled
        End Get
        Set(ByVal value As Boolean)
            _cardExpiryPPSAlertEnabled = value
        End Set
    End Property

    Private _cardExpirySAVAlertEnabled As Boolean = False
    Public Property CardExpirySAVAlertEnabled() As Boolean
        Get
            Return _cardExpirySAVAlertEnabled
        End Get
        Set(ByVal value As Boolean)
            _cardExpirySAVAlertEnabled = value
        End Set
    End Property

    Private _cardExpiryPPSWarnPeriodDays As Integer = 0
    Public Property CardExpiryPPSWarnPeriodDays() As Integer
        Get
            Return _cardExpiryPPSWarnPeriodDays
        End Get
        Set(ByVal value As Integer)
            _cardExpiryPPSWarnPeriodDays = value
        End Set
    End Property

    Private _stopcodeforid3 As String = " "
    Public Property stopcodeforid3() As String
        Get
            Return _stopcodeforid3
        End Get
        Set(ByVal value As String)
            _stopcodeforid3 = value
        End Set
    End Property

    Private _cardExpirySAVWarnPeriodDays As Integer = 0
    Public Property CardExpirySAVWarnPeriodDays() As Integer
        Get
            Return _cardExpirySAVWarnPeriodDays
        End Get
        Set(ByVal value As Integer)
            _cardExpirySAVWarnPeriodDays = value
        End Set
    End Property

    Public Property StopCode() As String
    Public Property PriceBand() As String
    Public Property BookNumber() As String
    Public Property DirectDebitAccountName() As String
    Public Property DirectDebitSortCode() As String
    Public Property DirectDebitAccount() As String
    Public Property DirectDebitTreasurer() As Boolean
    Public Property ContactbyPost() As Boolean
    Public Property ContactbyTelephoneHome() As Boolean
    Public Property ContactbyTelephoneWork() As Boolean
    Public Property ContactbyMobile() As Boolean
    Public Property ContactbyEmail() As Boolean
    Public Property MembershipsProductSubType() As String
    Public Property IncludeBoxOfficeLinks() As Boolean
    Public Property ProcessSuffix As String = String.Empty
    Public Property ProcessNickname As String = String.Empty
    Public Property ProcessAltUserName As String = String.Empty
    Public Property ProcessContactSLAccount As String = String.Empty
    Public Property SearchResultLimit As Integer
    Public Property CorporateSaleID As String = String.Empty
    Public Property PaymentReference As String = String.Empty
    Public Property PhoneNumber As String = String.Empty
    Public Property OriginalAddress As String = String.Empty
    Public Property NewAddress As String = String.Empty
    Public Property CustomerList As String = String.Empty
    Public Property Country As String = String.Empty
    Public Property ContactNumber As String = String.Empty
    Public Property CompanyNumber As String = String.Empty
    Public Property ResultsLimit As String = String.Empty
    Public Property Start As String = String.Empty
    Public Property Length As String = String.Empty
    Public Property SortOrder As String = String.Empty
    Public Property Draw As String = String.Empty
    Public Property ConsentStatus As String = String.Empty
    Public Property ParentPhone As String = String.Empty
    Public Property ParentEmail As String = String.Empty




    'Public Property OnWatchList As String = String.Empty
    'Public Property RowNumber As String = String.Empty
    Public Function LogString() As String

        Dim sb As New System.Text.StringBuilder

        With sb
            .Append(CustomerID & ",")
            .Append(ContactID & ",")
            .Append(CustomerNumber & ",")
            .Append(BranchCode & ",")
            .Append(Action & ",")
            .Append(ManualDeDupe & ",")
            .Append(ManualDeDupeBranch & ",")
            .Append(ThirdPartyContactRef & ",")
            .Append(DateFormat & ",")
            .Append(ThirdPartyCompanyRef1 & ",")
            .Append(ThirdPartyCompanyRef1Supplement & ",")
            .Append(ThirdPartyCompanyRef2 & ",")
            .Append(ThirdPartyCompanyRef2Supplement & ",")
            .Append(FriendsAndFamilyId & ",")
            .Append(FriendsAndFamilyMode & ",")

            .Append(AddressLine1 & ",")
            .Append(AddressLine2 & ",")
            .Append(AddressLine3 & ",")
            .Append(AddressLine4 & ",")
            .Append(AddressLine5 & ",")
            .Append(PostCode & ",")
            .Append(CustomerDesc1 & ",")
            .Append(CustomerDesc2 & ",")
            .Append(CustomerDesc3 & ",")
            .Append(CustomerDesc4 & ",")
            .Append(CustomerDesc5 & ",")
            .Append(CustomerDesc6 & ",")
            .Append(CustomerDesc7 & ",")
            .Append(CustomerDesc8 & ",")
            .Append(CustomerDesc9 & ",")
            .Append(OwningAgent & ",")
            .Append(WebAddress & ",")
            .Append(CompanyName & ",")
            .Append(Language & ",")
            .Append(CompanySLNumber1 & ",")
            .Append(CompanySLNumber2 & ",")
            .Append(CampaignCode & ",")
            .Append(CampaignEventCode & ",")
            .Append(CampaignResponseMethod & ",")
            .Append(CampaignResult & ",")


            .Append(UserName & ",")
            .Append(Password & ",")
            .Append(Source & ",")
            .Append(PasswordType & ",")

            .Append(ContactTitle & ",")
            .Append(ContactInitials & ",")
            .Append(ContactSurname & ",")
            .Append(ContactForename & ",")
            .Append(HomeTelephoneNumber & ",")
            .Append(ContactViaHomeTelephone & ",")
            .Append(WorkTelephoneNumber & ",")
            .Append(ContactViaWorkTelephone & ",")
            .Append(MobileNumber & ",")
            .Append(ContactViaMobile & ",")
            .Append(Telephone4 & ",")
            .Append(ContactViaTelephone4 & ",")
            .Append(Telephone5 & ",")
            .Append(ContactViaTelephone5 & ",")
            .Append(ContactCaptionA & ",")
            .Append(ContactCaptionB & ",")
            .Append(ContactCaptionC & ",")
            .Append(ContactCaptionD & ",")
            .Append(ContactCaptionE & ",")
            .Append(ContactCaptionF & ",")
            .Append(ContactCaptionG & ",")
            .Append(ContactCaptionH & ",")
            .Append(ContactCaptionI & ",")
            .Append(ContactCaptionJ & ",")
            .Append(ContactCaptionK & ",")
            .Append(ContactCaptionL & ",")
            .Append(ContactViaMail & ",")
            .Append(ContactViaMail1 & ",")
            .Append(ContactViaMail2 & ",")
            .Append(ContactViaMail3 & ",")
            .Append(ContactViaMail4 & ",")
            .Append(ContactViaMail5 & ",")
            .Append(Subscription1 & ",")
            .Append(Subscription2 & ",")
            .Append(Subscription3 & ",")
            .Append(EmailAddress & ",")
            .Append(ContactViaEmail & ",")
            .Append(DateBirth & ",")
            .Append(Gender & ",")
            .Append(Salutation & ",")
            .Append(PositionInCompany & ",")
            .Append(SLNumber1 & ",")
            .Append(SLNumber2 & ",")
            .Append(VatCode & ",")


            .Append(ProcessAddressLine1 & ",")
            .Append(ProcessAddressLine2 & ",")
            .Append(ProcessAddressLine3 & ",")
            .Append(ProcessAddressLine4 & ",")
            .Append(ProcessAddressLine5 & ",")
            .Append(ProcessPostCode & ",")
            .Append(ProcessCustomerDesc1 & ",")
            .Append(ProcessCustomerDesc2 & ",")
            .Append(ProcessCustomerDesc3 & ",")
            .Append(ProcessCustomerDesc4 & ",")
            .Append(ProcessCustomerDesc5 & ",")
            .Append(ProcessCustomerDesc6 & ",")
            .Append(ProcessCustomerDesc7 & ",")
            .Append(ProcessCustomerDesc8 & ",")
            .Append(ProcessCustomerDesc9 & ",")
            .Append(ProcessOwningAgent & ",")
            .Append(ProcessWebAddress & ",")
            .Append(ProcessCompanyName & ",")
            .Append(ProcessCompanySLNumber1 & ",")
            .Append(ProcessCompanySLNumber2 & ",")
            .Append(ProcessCampaignCode & ",")
            .Append(ProcessCampaignEventCode & ",")
            .Append(ProcessCampaignResponseMethod & ",")
            .Append(ProcessCampaignResult & ",")

            .Append(ProcessContactTitle & ",")
            .Append(ProcessContactInitials & ",")
            .Append(ProcessContactSurname & ",")
            .Append(ProcessContactForename & ",")
            .Append(ProcessHomeTelephoneNumber & ",")
            .Append(ProcessContactViaHomeTelephone & ",")
            .Append(ProcessWorkTelephoneNumber & ",")
            .Append(ProcessContactViaWorkTelephone & ",")
            .Append(ProcessMobileNumber & ",")
            .Append(ProcessContactViaMobile & ",")
            .Append(ProcessTelephone4 & ",")
            .Append(ProcessContactViaTelephone4 & ",")
            .Append(ProcessTelephone5 & ",")
            .Append(ProcessContactViaTelephone5 & ",")
            .Append(ProcessContactCaptionA & ",")
            .Append(ProcessContactCaptionB & ",")
            .Append(ProcessContactCaptionC & ",")
            .Append(ProcessContactCaptionD & ",")
            .Append(ProcessContactCaptionE & ",")
            .Append(ProcessContactCaptionF & ",")
            .Append(ProcessContactCaptionG & ",")
            .Append(ProcessContactCaptionH & ",")
            .Append(ProcessContactCaptionI & ",")
            .Append(ProcessContactCaptionJ & ",")
            .Append(ProcessContactCaptionK & ",")
            .Append(ProcessContactCaptionL & ",")
            .Append(ProcessContactViaMail & ",")
            .Append(ProcessContactViaMail1 & ",")
            .Append(ProcessContactViaMail2 & ",")
            .Append(ProcessContactViaMail3 & ",")
            .Append(ProcessContactViaMail4 & ",")
            .Append(ProcessContactViaMail5 & ",")
            .Append(ProcessSubscription1 & ",")
            .Append(ProcessSubscription2 & ",")
            .Append(ProcessSubscription3 & ",")
            .Append(ProcessEmailAddress & ",")
            .Append(ProcessContactViaEmail & ",")
            .Append(ProcessDateBirth & ",")
            .Append(ProcessGender & ",")
            .Append(ProcessSalutation & ",")
            .Append(ProcessPositionInCompany & ",")
            .Append(ProcessSLNumber1 & ",")
            .Append(ProcessSLNumber2 & ",")
            .Append(ProcessVatCode & ",")

            .Append(Attribute01 & ",")
            .Append(Attribute01Action & ",")
            .Append(Attribute02 & ",")
            .Append(Attribute02Action & ",")
            .Append(Attribute03 & ",")
            .Append(Attribute03Action & ",")
            .Append(Attribute04 & ",")
            .Append(Attribute04Action & ",")
            .Append(Attribute05 & ",")
            .Append(Attribute05Action & ",")
            .Append(Attribute06 & ",")
            .Append(Attribute06Action & ",")
            .Append(Attribute07 & ",")
            .Append(Attribute07Action & ",")
            .Append(Attribute08 & ",")
            .Append(Attribute08Action & ",")
            .Append(Attribute09 & ",")
            .Append(Attribute09Action & ",")
            .Append(Attribute10 & ",")
            .Append(Attribute10Action & ",")
            .Append(Attribute11 & ",")
            .Append(Attribute11Action & ",")
            .Append(Attribute12 & ",")
            .Append(Attribute12Action & ",")
            .Append(Attribute13 & ",")
            .Append(Attribute13Action & ",")
            .Append(Attribute14 & ",")
            .Append(Attribute14Action & ",")
            .Append(Attribute15 & ",")
            .Append(Attribute15Action & ",")
            .Append(Attribute16 & ",")
            .Append(Attribute16Action & ",")
            .Append(Attribute17 & ",")
            .Append(Attribute17Action & ",")
            .Append(Attribute18 & ",")
            .Append(Attribute18Action & ",")
            .Append(Attribute19 & ",")
            .Append(Attribute19Action & ",")
            .Append(Attribute20 & ",")
            .Append(Attribute20Action & ",")
            .Append(Attribute21 & ",")
            .Append(Attribute21Action & ",")
            .Append(Attribute22 & ",")
            .Append(Attribute22Action & ",")
            .Append(Attribute23 & ",")
            .Append(Attribute23Action & ",")
            .Append(Attribute24 & ",")
            .Append(Attribute24Action & ",")
            .Append(Attribute25 & ",")
            .Append(Attribute25Action & ",")
            .Append(Attribute26 & ",")
            .Append(Attribute26Action & ",")
            .Append(Attribute27 & ",")
            .Append(Attribute27Action & ",")
            .Append(Attribute28 & ",")
            .Append(Attribute28Action & ",")
            .Append(Attribute29 & ",")
            .Append(Attribute29Action & ",")
            .Append(Attribute30 & ",")
            .Append(Attribute30Action & ",")
            .Append(Attribute31 & ",")
            .Append(Attribute31Action & ",")
            .Append(Attribute32 & ",")
            .Append(Attribute32Action & ",")
            .Append(Attribute33 & ",")
            .Append(Attribute33Action & ",")
            .Append(Attribute34 & ",")
            .Append(Attribute34Action & ",")
            .Append(Attribute35 & ",")
            .Append(Attribute35Action & ",")
            .Append(Attribute36 & ",")
            .Append(Attribute36Action & ",")
            .Append(Attribute37 & ",")
            .Append(Attribute37Action & ",")
            .Append(Attribute38 & ",")
            .Append(Attribute38Action & ",")
            .Append(Attribute39 & ",")
            .Append(Attribute39Action & ",")
            .Append(Attribute40 & ",")
            .Append(Attribute40Action & ",")
            .Append(Attribute41 & ",")
            .Append(Attribute41Action & ",")
            .Append(Attribute42 & ",")
            .Append(Attribute42Action & ",")
            .Append(Attribute43 & ",")
            .Append(Attribute43Action & ",")
            .Append(Attribute44 & ",")
            .Append(Attribute44Action & ",")
            .Append(Attribute45 & ",")
            .Append(Attribute45Action & ",")
            .Append(Attribute46 & ",")
            .Append(Attribute46Action & ",")
            .Append(Attribute47 & ",")
            .Append(Attribute47Action & ",")
            .Append(Attribute48 & ",")
            .Append(Attribute48Action & ",")
            .Append(Attribute49 & ",")
            .Append(Attribute49Action & ",")
            .Append(Attribute50 & ",")
            .Append(Attribute50Action & ",")
            .Append(Attribute51 & ",")
            .Append(Attribute51Action & ",")
            .Append(Attribute52 & ",")
            .Append(Attribute52Action & ",")
            .Append(Attribute53 & ",")
            .Append(Attribute53Action & ",")
            .Append(Attribute54 & ",")
            .Append(Attribute54Action & ",")
            .Append(Attribute55 & ",")
            .Append(Attribute55Action & ",")
            .Append(Attribute56 & ",")
            .Append(Attribute56Action & ",")
            .Append(Attribute57 & ",")
            .Append(Attribute57Action & ",")
            .Append(Attribute58 & ",")
            .Append(Attribute58Action & ",")
            .Append(Attribute59 & ",")
            .Append(Attribute59Action & ",")
            .Append(Attribute60 & ",")
            .Append(Attribute60Action & ",")

            .Append(ActionCode & ",")
            .Append(ActionCodeFixed & ",")
            .Append(ActionStatus & ",")
            .Append(ActionPty & ",")
            .Append(ActionAgent & ",")
            .Append(ActionComment01 & ",")
            .Append(ActionComment02 & ",")
            .Append(ActionComment03 & ",")
            .Append(ActionComment04 & ",")
            .Append(ActionComment05 & ",")
            .Append(ActionComment06 & ",")
            .Append(ActionComment07 & ",")
            .Append(ActionComment08 & ",")
            .Append(ActionComment09 & ",")
            .Append(ActionComment10 & ",")
            .Append(ActionComment11 & ",")
            .Append(ActionComment12 & ",")
            .Append(ActionComment13 & ",")
            .Append(ActionComment14 & ",")
            .Append(ActionComment15 & ",")
            .Append(ActionComment16 & ",")
            .Append(ActionDate & ",")
            .Append(ActionID & ",")
            .Append(ActionSubject & ",")
            .Append(ActionMemo & ",")
            .Append(ActionDepartmentID & ",")
            .Append(ActionProductID & ",")
            .Append(ActionProjectID & ",")
            .Append(ActionCampaignID & ",")
            .Append(ActionField1 & ",")
            .Append(ActionField2 & ",")
            .Append(ActionField3 & ",")
            .Append(ActionField4 & ",")
            .Append(ActionField5 & ",")

            .Append(NoteCode & ",")
            .Append(NoteComment01 & ",")
            .Append(NoteComment02 & ",")
            .Append(NoteComment03 & ",")
            .Append(NoteComment04 & ",")
            .Append(NoteComment05 & ",")
            .Append(NoteComment06 & ",")
            .Append(NoteComment07 & ",")
            .Append(NoteComment08 & ",")
            .Append(NoteComment09 & ",")
            .Append(NoteComment10 & ",")
            .Append(NoteComment11 & ",")
            .Append(NoteComment12 & ",")
            .Append(NoteComment13 & ",")
            .Append(NoteComment14 & ",")
            .Append(NoteComment15 & ",")
            .Append(NoteComment16 & ",")

            .Append(ErrorCode & ",")
            .Append(ErrorFlag)
        End With

        Return sb.ToString.Trim

    End Function
End Class
