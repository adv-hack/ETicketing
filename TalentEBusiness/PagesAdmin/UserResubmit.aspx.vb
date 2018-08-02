Imports Talent.Common

Partial Class PagesAdmin_UserResubmit
    Inherits Base01


    Public ReadOnly Property BusinessUnit() As String
        Get
            Return TalentCache.GetBusinessUnit
        End Get
    End Property


    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click
        'SendDetailsToBackend("REGISTRATION", userDetails, userAddress)

        ErrorList.Items.Clear()
        Select Case IterativeOnOff.SelectedValue
            Case 1
                If String.IsNullOrEmpty(PartnerBox.Text) Then
                    ErrorList.Items.Add("Partner Value has not been specified")
                End If
                If String.IsNullOrEmpty(LoginIdBox.Text) Then
                    ErrorList.Items.Add("Login Id Value has not been specified")
                End If
                iterations.Text = 0
            Case 2
                Try
                    Dim myint As Integer = CInt(PartnerBox.Text)
                Catch ex As Exception
                    ErrorList.Items.Add("Partner Value is not an Integer")
                End Try
                Try
                    Dim myint As Integer = CInt(LoginIdBox.Text)
                Catch ex As Exception
                    ErrorList.Items.Add("LoginID Value is not an Integer")
                End Try
                Try
                    Dim myint As Integer = CInt(iterations.Text)
                Catch ex As Exception
                    ErrorList.Items.Add("Itterations Value is not an Integer")
                End Try
        End Select


        If ErrorList.Items.Count = 0 Then
            For i As Integer = 0 To CInt(iterations.Text)


                Dim loginIDVal As Integer = 0
                Dim partnerVal As Integer = 0

                Dim myUser As New TalentProfileUser
                Select Case UpOrDownList.SelectedValue
                    Case 1 'up
                        loginIDVal = CInt(LoginIdBox.Text) + i
                        partnerVal = CInt(PartnerBox.Text) + i
                        myUser = Me.LoadUser(loginIDVal.ToString, PartnerPrefixBox.Text & partnerVal.ToString("000000"))
                    Case 2 'down
                        loginIDVal = CInt(LoginIdBox.Text) - i
                        partnerVal = CInt(PartnerBox.Text) - i
                        myUser = Me.LoadUser(loginIDVal.ToString, PartnerPrefixBox.Text & partnerVal.ToString("000000"))
                End Select

                Dim result As Boolean = False
                Select Case IterativeOnOff.SelectedValue
                    Case 1
                        result = SubmitUserType1("REGISTRATION", myUser.Details, ProfileHelper.ProfileAddressEnumerator(0, myUser.Addresses))

                    Case 2
                        Dim myPartner As TalentProfilePartner = Me.GetPartnerData(PartnerPrefixBox.Text & partnerVal.ToString("000000"))
                        result = SubmitUserType2("REGISTRATION", myUser.Details, ProfileHelper.ProfileAddressEnumerator(0, myUser.Addresses), myPartner)
                End Select

                ErrorList.Items.Add(loginIDVal.ToString & " - Attempting to Re-Process...")
                If result Then
                    ErrorList.Items.Add(loginIDVal.ToString & " - Succeeded")
                Else
                    ErrorList.Items.Add(loginIDVal.ToString & " - Failed")
                End If

            Next

        End If

    End Sub

    Protected Function SubmitUserType1(ByVal call_origin As String, _
                                    ByVal userDetails As TalentProfileUserDetails, _
                                    ByVal userAddress As TalentProfileAddress) As Boolean

        Dim success As Boolean = True

        Dim myCustomer As New TalentCustomer
        Dim deCustV11 As New DECustomerV11
        Dim deCustV1 As New DECustomer
        deCustV11.DECustomersV1.Add(deCustV1)

        Dim myErrorObj As New ErrorObj
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        Dim p As New TalentProfileUser
        Dim address As New TalentProfileAddress


        p.Details = userDetails
        address = userAddress

        '
        ' Set the Customer object and invoke the SetCustomer method to post details to TALENT Customer Manager
        '
        With myCustomer
            .DeV11 = deCustV11

            ' Reset the settings entity to a customer specific settings entity 
            .Settings = CType(New DECustomerSettings, DESettings)

            With deCustV1
                .Password = ""
                .Action = ""
                '    .ThirdPartyContactRef = ucr.Content("crmExternalKeyName", _languageCode, True)
                .ThirdPartyContactRef = p.Details.User_Number
                .ThirdPartyCompanyRef1 = p.Details.User_Number
                .ThirdPartyCompanyRef1Supplement = p.Details.User_Number_Prefix
                .ThirdPartyCompanyRef2 = ""
                .DateFormat = "1"

                .ContactSurname = p.Details.Surname
                .ContactForename = p.Details.Forename
                .ContactTitle = p.Details.Title
                .ContactInitials = p.Details.Initials
                .Salutation = p.Details.Salutation
                .DateBirth = p.Details.DOB.Year.ToString("YY").PadLeft(2, "0") & _
                                p.Details.DOB.Month.ToString("YY").PadLeft(2, "0") & _
                                p.Details.DOB.Day.ToString("YY").PadLeft(2, "0")
                Select Case p.Details.Sex
                    Case Is = "M"
                        .Gender = "M"
                    Case Is = "F"
                        .Gender = "F"
                    Case Else
                        .Gender = String.Empty
                End Select
                .ProcessGender = "1"

                .EmailAddress = p.Details.Email
                .SLNumber1 = p.Details.Account_No_1
                .SLNumber2 = p.Details.Account_No_2

                .ProcessContactSurname = "1"
                .ProcessContactForename = "1"
                .ProcessContactTitle = "1"
                .ProcessContactInitials = "1"
                .ProcessDateBirth = "1"
                .ProcessEmailAddress = "1"
                .ProcessSLNumber1 = "1"
                .ProcessSLNumber2 = "1"
                .ProcessPassword = "1"
                .ProcessSalutation = "1"
                .Language = Talent.eCommerce.Utilities.GetCurrentLanguage()
                '------------------------------------------------------------------
                ' Pick up branch from Partner or if blank then from module defaults
                '------------------------------------------------------------------
                Dim partner As String = TalentCache.GetPartner(HttpContext.Current.Profile)
                Dim partnerData As New TalentApplicationVariablesTableAdapters.tbl_partnerTableAdapter
                Dim dt As Data.DataTable = partnerData.GetDataBy_Partner(partner)
                If dt.Rows.Count > 0 Then
                    .BranchCode = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0)("CRM_BRANCH"))
                End If

                If .BranchCode = String.Empty Then
                    'Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
                    'Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
                    .BranchCode = def.CrmBranch
                End If

                ' If QAS is used then Address Line is never populated and therefore Address1 for CRM 
                ' needs to be populated from different source fields.
                Dim eComDefs As New Talent.eCommerce.ECommerceModuleDefaults
                Dim defs As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = eComDefs.GetDefaults
                ' If QAS is switched on then do not show AddressLine1Row (building)
                If Not defs.AddressLine1RowVisible Then
                    .AddressLine1 = address.Address_Line_2.Trim
                Else
                    .AddressLine1 = Trim(address.Address_Line_1.Trim & " " & address.Address_Line_2.Trim)
                End If
                .AddressLine2 = address.Address_Line_3.Trim
                .AddressLine3 = address.Address_Line_4.Trim
                .AddressLine4 = address.Address_Line_5.Trim
                .AddressLine5 = address.Country.Trim
                .PostCode = UCase(address.Post_Code)
                .HomeTelephoneNumber = p.Details.Telephone_Number
                .WorkTelephoneNumber = p.Details.Work_Number
                .MobileNumber = p.Details.Mobile_Number
                .CompanyName = .AddressLine1
                .ProcessCompanyName = "1"

                .ProcessAddressLine1 = "1"
                .ProcessAddressLine2 = "1"
                .ProcessAddressLine3 = "1"
                .ProcessAddressLine4 = "1"
                .ProcessAddressLine5 = "1"
                .ProcessPostCode = "1"
                .ProcessHomeTelephoneNumber = "1"
                .ProcessWorkTelephoneNumber = "1"
                .ProcessMobileNumber = "1"

            End With

            ' Set the Customer Specific Settings
            Dim decs As New DECustomerSettings()
            decs = CType(.Settings, DECustomerSettings)
            decs.CreationType = "REGISTRATION"
            .Settings = CType(decs, DESettings)

            With .Settings
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .BusinessUnit = TalentCache.GetBusinessUnit
                .Company = "EBUSINESS"
                .Cacheing = False
                ' .DestinationDatabase = ucr.Content("crmDestinationDatabase", LC_ID, True)
                .DestinationDatabase = Talent.eCommerce.Utilities.GetCustomerDestinationDatabase()
                .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("TALENTCRM").ToString
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                .RetryFailures = def.RegistrationRetry
                .RetryAttempts = def.RegistrationRetryAttempts
                .RetryWaitTime = def.RegistrationRetryWait
                .RetryErrorNumbers = def.RegistrationRetryErrors
            End With
            myErrorObj = .SetCustomer()

            If myErrorObj.HasError Then
                Try
                    Talent.eCommerce.Logging.WriteLog(p.Details.LoginID, _
                                    myErrorObj.ErrorNumber, _
                                    "Registration Error - " & myErrorObj.ErrorMessage, _
                                    myErrorObj.ErrorStatus, _
                                    TalentCache.GetBusinessUnit, _
                                    TalentCache.GetPartner(Profile))
                Catch ex As Exception

                End Try
                Talent.eCommerce.Utilities.SerializeObject(myCustomer, _
                                                            myCustomer.GetType, _
                                                            TalentCache.GetBusinessUnit, _
                                                            TalentCache.GetPartner(Profile), _
                                                            Profile.UserName, _
                                                            call_origin)
            End If

        End With

        Return myErrorObj.HasError

    End Function

    Public Function SubmitUserType2(ByVal call_origin As String, _
                                   ByVal userDetails As TalentProfileUserDetails, _
                                   ByVal userAddress As TalentProfileAddress, _
                                    ByVal partObj As TalentProfilePartner) As Boolean
        Dim success As Boolean = True
        Try
            Dim myCustomer As New TalentCustomer

            Dim deCustV11 As New DECustomerV11
            Dim deCustV1 As New DECustomer
            deCustV11.DECustomersV1.Add(deCustV1)

            Dim myErrorObj As New ErrorObj
            Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
            Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
            Dim p As New TalentProfileUser
            Dim address As New TalentProfileAddress


            p.Details = userDetails
            address = userAddress
            '
            ' Set the Customer object and invoke the SetCustomer method to post details to TALENT Customer Manager
            '
            With myCustomer
                .DeV11 = deCustV11
                ' Reset the settings entity to a customer specific settings entity 
                .Settings = CType(New DECustomerSettings, DESettings)

                With deCustV1
                    .Action = ""

                    .ThirdPartyContactRef = p.Details.User_Number
                    '.ThirdPartyCompanyRef1 = p.Details.User_Number
                    '.ThirdPartyCompanyRef1Supplement = p.Details.User_Number_Prefix
                    .ThirdPartyCompanyRef2 = ""
                    .DateFormat = "1"

                    .ContactSurname = p.Details.Surname
                    .ContactForename = p.Details.Forename
                    .ContactTitle = p.Details.Title
                    .ContactInitials = p.Details.Initials
                    '   .DateBirth = dobYear.Text.ToString.PadLeft(2, "0") & _
                    '                   dobMonth.Text.ToString.PadLeft(2, "0") & _
                    '   dobDay.Text.PadLeft(2, "0")
                    'Select Case sex.SelectedValue
                    '    Case Is = "M"
                    '        .Gender = sex.SelectedValue
                    '    Case Is = "F"
                    '        .Gender = sex.SelectedValue
                    '    Case Else
                    '        .Gender = String.Empty
                    'End Select
                    .EmailAddress = p.Details.Email
                    '----------------------------------------
                    ' Set Company account number, not contact
                    '----------------------------------------
                    .CompanySLNumber1 = p.Details.Account_No_1
                    .CompanySLNumber2 = p.Details.Account_No_2

                    .ProcessContactSurname = "1"
                    .ProcessContactForename = "1"
                    .ProcessContactTitle = "1"
                    .ProcessContactInitials = "1"
                    .ProcessDateBirth = "1"
                    .ProcessEmailAddress = "1"
                    '.ProcessSLNumber1 = "1"
                    '.ProcessSLNumber2 = "1"

                    .Language = Talent.eCommerce.Utilities.GetCurrentLanguage()
                    '------------------------------------------------------------------
                    ' Pick up branch from Partner or if blank then from module defaults
                    '------------------------------------------------------------------
                    Try
                        .BranchCode = partObj.Details.CRM_Branch
                    Catch ex As Exception

                    End Try

                    If .BranchCode = String.Empty Then
                        .BranchCode = def.CrmBranch
                    End If

                    ' If QAS is used then Address Line is never populated and therefore Address1 for CRM 
                    ' needs to be populated from different source fields.
                    Dim eComDefs As New Talent.eCommerce.ECommerceModuleDefaults
                    Dim defs As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = eComDefs.GetDefaults


                    .AddressLine1 = address.Address_Line_2.Trim
                    .AddressLine2 = address.Address_Line_3.Trim
                    .AddressLine3 = address.Address_Line_4.Trim
                    .AddressLine4 = address.Address_Line_5.Trim
                    .AddressLine5 = address.Country.Trim

                    .PostCode = UCase(address.Post_Code)
                    .HomeTelephoneNumber = p.Details.Telephone_Number
                    .WorkTelephoneNumber = p.Details.Work_Number
                    .MobileNumber = p.Details.Mobile_Number
                    ' .CompanyName = .AddressLine1
                    .CompanyName = address.Address_Line_1

                    'If accountNumber.Text <> String.Empty AndAlso .SLNumber1 = String.Empty Then
                    '    .CompanySLNumber1 = accountNumber.Text
                    'End If
                    'If accountNumber2.Text <> String.Empty AndAlso .SLNumber2 = String.Empty Then
                    '    .CompanySLNumber2 = accountNumber2.Text
                    'End If
                    'If accountNumber.Text <> String.Empty AndAlso .SLNumber1 = String.Empty Then
                    '    .SLNumber1 = accountNumber.Text
                    'End If
                    'If accountNumber2.Text <> String.Empty AndAlso .SLNumber2 = String.Empty Then
                    '    .SLNumber2 = accountNumber2.Text
                    'End If
                    .VatCode = partObj.Details.VAT_NUMBER

                    .ProcessCompanyName = "1"
                    .ProcessAddressLine1 = "1"
                    .ProcessAddressLine2 = "1"
                    .ProcessAddressLine3 = "1"
                    .ProcessAddressLine4 = "1"
                    .ProcessAddressLine5 = "1"
                    .ProcessPostCode = "1"
                    .ProcessHomeTelephoneNumber = "1"
                    .ProcessWorkTelephoneNumber = "1"
                    .ProcessMobileNumber = "1"
                    .ProcessVatCode = "1"
                    .ProcessCompanySLNumber1 = "1"
                    .ProcessCompanySLNumber2 = "1"

                End With

                ' Set the Customer Specific Settings
                Dim decs As New DECustomerSettings()
                decs = CType(.Settings, DECustomerSettings)
                decs.CreationType = "REGISTRATION"
                .Settings = CType(decs, DESettings)

                With .Settings
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    .BusinessUnit = TalentCache.GetBusinessUnit
                    .Company = "EBUSINESS"
                    .Cacheing = False

                    .DestinationDatabase = Talent.eCommerce.Utilities.GetCustomerDestinationDatabase()
                    .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("TALENTCRM").ToString
                    .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                    .RetryFailures = def.RegistrationRetry
                    .RetryAttempts = def.RegistrationRetryAttempts
                    .RetryWaitTime = def.RegistrationRetryWait
                    .RetryErrorNumbers = def.RegistrationRetryErrors
                End With
                myErrorObj = .SetCustomer()

                If myErrorObj.HasError Then
                    ErrorList.Items.Add(myErrorObj.ErrorNumber & " - " & myErrorObj.ErrorMessage & " - " & myErrorObj.ErrorStatus)
                    success = False
                    Talent.eCommerce.Utilities.SerializeObject(myCustomer, _
                                                                myCustomer.GetType, _
                                                                TalentCache.GetBusinessUnit, _
                                                                TalentCache.GetPartner(Profile), _
                                                                userDetails.LoginID, _
                                                                call_origin)
                End If

            End With
        Catch ex As Exception
            success = False
            ErrorList.Items.Add(ex.Message)
        End Try

        Return success
    End Function

    Public Function LoadUser(ByVal username As String, ByVal partner As String) As TalentProfileUser

        Dim userProfileDetails As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter
        Dim ProfileAddress As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter
        Dim authPartners As New TalentMembershipDatasetTableAdapters.tbl_authorized_partnersTableAdapter

        'Create a new user obj
        Dim user As New TalentProfileUser
        Dim details As New TalentProfileUserDetails
        Dim address As New TalentProfileAddress

        '---------------------------------------
        'Populate the user.Details properties
        '---------------------------------------
        Dim dt As System.Data.DataTable
        If authPartners.Get_CheckFor_B2C_Login(BusinessUnit).Rows.Count > 0 Then
            dt = userProfileDetails.GetByLoginIDAndPartner(username, Partner)
        Else
            dt = userProfileDetails.GetDataByLoginIDNoPartner(username)
        End If
        Dim al As ArrayList = ProfileHelper.GetPropertyNames(details)
        details = PopulateProperties(al, dt, details, 0)
        details.IsDirty = False 'Set to false as we have just read the values from the DB
        user.Details = details
        '---------------------------------------

        '---------------------------------------
        'Populate the user.Addresses properties
        '---------------------------------------
        al = ProfileHelper.GetPropertyNames(address)
        dt = ProfileAddress.GetByLoginIDAndPartner(username, Partner)

        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                address = New TalentProfileAddress
                address = PopulateProperties(al, dt, address, i)
                address.IsDirty = False 'Set to false as we have just read the values from the DB
                Dim exists As Boolean = False
                For Each key As String In user.Addresses.Keys
                    If UCase(key) = UCase(Utilities.CheckForDBNull_String(dt.Rows(i)("REFERENCE"))) _
                        AndAlso Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dt.Rows(i)("REFERENCE"))) Then
                        exists = True
                        Exit For
                    End If
                Next
                If Not exists Then user.Addresses.Add(Utilities.CheckForDBNull_String(dt.Rows(i)("REFERENCE")), address)
            Next
        End If

        Return user
    End Function

    Protected Function GetPartnerData(ByVal partner As String) As TalentProfilePartner

        Dim partnerProfileDetails As New TalentMembershipDatasetTableAdapters.tbl_partnerTableAdapter
        Dim ProfileAddress As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter

        Dim partnerObj As New TalentProfilePartner
        Dim details As New TalentProfilePartnerDetails
        Dim address As New TalentProfileAddress

        '---------------------------------------
        'Populate the partner.Details properties
        '---------------------------------------
        Dim dt As System.Data.DataTable = partnerProfileDetails.GetDataByPartner(Partner)
        Dim al As ArrayList = ProfileHelper.GetPropertyNames(details)
        details = PopulateProperties(al, dt, details, 0)
        details.IsDirty = False 'Set to false as we have just read the values from the DB
        partnerObj.Details = details
        '---------------------------------------

        '-----------------------------------------
        'Populate the partner.Addresses properties
        '-----------------------------------------
        al = ProfileHelper.GetPropertyNames(address)
        dt = ProfileAddress.GetDataByPartnerAndType(Partner, "PARTNER")
        For i As Integer = 0 To dt.Rows.Count - 1
            address = New TalentProfileAddress
            address = PopulateProperties(al, dt, address, i)
            address.IsDirty = False 'Set to false as we have just read the values from the DB
            partnerObj.Addresses.Add(dt.Rows(i)("REFERENCE"), address)
        Next

        partnerProfileDetails.Dispose()
        ProfileAddress.Dispose()
        Return partnerObj
    End Function

    Protected Function PopulateProperties(ByVal propertiesList As ArrayList, ByVal dt As System.Data.DataTable, ByVal objectToPopulate As Object, ByVal rowIndex As Integer) As Object
        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To propertiesList.Count - 1
                If dt.Columns.Contains(propertiesList(i)) Then
                    CallByName(objectToPopulate, propertiesList(i), CallType.Set, ProfileHelper.CheckDBNull(dt.Rows(rowIndex)(propertiesList(i))))
                Else
                    'If the column does not exist, handle any properties on the class that we know of
                    Select Case propertiesList(i).ToString
                        Case Is = "IsDirty"
                            CallByName(objectToPopulate, propertiesList(i), CallType.Set, False)
                        Case Is = "MODULE_"
                            'Module is a KEYWORD in vb.net so the property name could not be called the same as the DB field name.
                            CallByName(objectToPopulate, propertiesList(i), CallType.Set, Utilities.CheckForDBNull_String(dt.Rows(rowIndex)("MODULE")))
                        Case Else
                            'Handle all other occurances
                    End Select
                End If
            Next
        End If

        Return objectToPopulate
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Select Case IterativeOnOff.SelectedValue
            Case 1
                NoOfItsLabel.Visible = False
                iterations.Visible = False
                ItDirectionLabel.Visible = False
                UpOrDownList.Visible = False
            Case 2
                NoOfItsLabel.Visible = True
                iterations.Visible = True
                ItDirectionLabel.Visible = True
                UpOrDownList.Visible = True
        End Select

    End Sub
End Class
