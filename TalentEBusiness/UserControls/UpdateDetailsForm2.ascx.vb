Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports Talent.Common

'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Registration Form
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCREGFM- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_UpdateDetailsForm2
    Inherits AbstractTalentUserControl

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _display As Boolean = True
    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        SetUpUCR()

        If Not Page.IsPostBack Then
            Try
                If UCase(Request.QueryString("reason").ToString) = "USERNAME" Then
                    ErrorLabel.Text = ucr.Content("InvalidUsernameError", _languageCode, True)
                ElseIf UCase(Request.QueryString("reason").ToString) = "ACCOUNTERROR" Then
                    ErrorLabel.Text = ucr.Content("InvalidAccountError", _languageCode, True)
                End If
            Catch ex As Exception
            End Try

            PopulateTitleDropDownList()
            PopulateCountriesDropDownList()
            SetLabelText()
            SetControlForUserDetails()
            SetUserDetails()

        End If

        'Show password control
        Me.ChangeMyPassword1.DisplayControl = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("DisplayPasswordControl"))

    End Sub
    Protected Overrides Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        MyBase.Page_Load(sender, e)
    End Sub

    Protected Sub SetUserDetails()
        With Profile.User.Details
            title.SelectedValue = .Title
            fullName.Text = .Full_Name
            forename.Text = .Forename
            surname.Text = .Surname
            position.Text = .Position
            email.Text = .Email
            emailConfirm.Text = .Email
            phone.Text = .Telephone_Number
            fax.Text = .Fax_Number
            Newsletter.Checked = .Subscribe_Newsletter
            Try
                Select Case .HTML_Newsletter
                    Case Is = True
                        NewsletterChoice.SelectedValue = ucr.Content("HTMLNewsletterText", _languageCode, True)
                    Case Is = False
                        NewsletterChoice.SelectedValue = ucr.Content("PlainTextNewsletterText", _languageCode, True)
                End Select
            Catch
            End Try
        End With

        costCen.Text = Profile.PartnerInfo.Details.COST_CENTRE
        companyName.Text = Profile.PartnerInfo.Details.Partner_Desc

        Dim address As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)

        With address
            address1.Text = .Address_Line_1
            address2.Text = .Address_Line_2
            address3.Text = .Address_Line_3
            address4.Text = .Address_Line_4
            address5.Text = .Address_Line_5
            postcode.Text = .Post_Code

            Try
                Dim i As Integer = 0
                For Each li As ListItem In country.Items
                    If li.Value.ToLower = .Country.ToLower OrElse li.Text.ToLower = .Country.ToLower Then
                        country.SelectedIndex = i
                        Exit For
                    End If
                    i += 1
                Next
            Catch
            End Try

        End With

        With Profile.PartnerInfo.Details
            vatNumber.Text = .VAT_NUMBER()
        End With

    End Sub

    Protected Overrides Sub SetUpUCR()
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.Common.Utilities.GetAllString 'GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "UpdateDetailsForm2.ascx"
        End With
    End Sub

    Protected Sub SetControlForUserDetails()
        '-----------------------------------------------------------------------------------------
        ' If from activation (activateAccount.ascx) then protect 'old' password field and make new
        ' password mandatory. Remove Change Password buttons, but make 'Update' also write the 
        ' password.
        '-----------------------------------------------------------------------------------------
        If Not Session("FromActivation") Is Nothing AndAlso Session("FromActivation") = True Then
            With ChangeMyPassword1
                .ChangePasswordButton.Visible = False
                .CurrentPasswordBox.Enabled = False
                .CurrentPasswordBox.Visible = False
                .CurrentPasswordRequiredFieldValidator.Enabled = False
                .NewPasswordRequiredFieldValidator.Enabled = True
                .NewPasswordRequiredFieldValidator.ValidationGroup = "Registration"
                .ConfirmNewPasswordRequiredFieldValidator.Enabled = True
                .ConfirmNewPasswordRequiredFieldValidator.ValidationGroup = "Registration"
            End With
            surname.Enabled = False
        End If
    End Sub
    Private Sub SetLabelText()
        Try

            RegistrationHeaderLabel.Text = ucr.Content("registrationHeaderLabel1", _languageCode, True)             ' CSG Registration"
            updateBtn.Text = ucr.Content("updateBtn", _languageCode, True)                                      ' Register"
            If Not Session("FromActivationNewContact") Is Nothing AndAlso Session("FromActivationNewContact") = True Then
                lblFromActivationNewContact.Text = ucr.Content("FromActivationNewContact", _languageCode, True)
                Session("FromActivationNewContact") = Nothing
            End If

            With ucr
                PersonalDetailsLabel.Text = .Content("personalDetailsLabel", _languageCode, True)                        ' Personal Details"
                AddressInfoLabel.Text = .Content("addressInfoLabel", _languageCode, True)                                ' Address Info"
                CompanyInfoLabel.Text = .Content("companyInfoLabel", _languageCode, True)
                '
                emailLabel.Text = .Content("emailLabel", _languageCode, True)                                            ' Email"
                emailConfirmLabel.Text = .Content("ConfirmEmailLabel", _languageCode, True)                              ' Confirm Email"
                titleLabel.Text = .Content("titleLabel", _languageCode, True)                                            ' Title"
                fullNameLabel.Text = .Content("fullNameLabel", _languageCode, True)                                      ' Full name Label
                forenameLabel.Text = .Content("forenameLabel", _languageCode, True)                                      ' Forename"
                surnameLabel.Text = .Content("surnameLabel", _languageCode, True)                                        ' Surname"
                positionLabel.Text = .Content("positionLabel", _languageCode, True)                                      ' Position"
                '
                phoneLabel.Text = .Content("phoneLabel", _languageCode, True)                                            ' Telephone Number"
                faxLabel.Text = .Content("faxLabel", _languageCode, True)                                                ' Fax Number"

                companyNameLabel.Text = .Content("companyNameLabel", _languageCode, True)                                ' Company Name
                lblAddress1.Text = .Content("address1Label", _languageCode, True)                                        ' Building Name/No"
                lblAddress2.Text = .Content("address2Label", _languageCode, True)                                          ' Street"
                lblAddress3.Text = .Content("address3Label", _languageCode, True)                                              ' Town"
                lblAddress4.Text = .Content("address4Label", _languageCode, True)                                              ' City"
                lblAddress5.Text = .Content("address5Label", _languageCode, True)
                postcodeLabel.Text = .Content("postcodeLabel", _languageCode, True)                                      ' Postcode"
                countryLabel.Text = .Content("countryLabel", _languageCode, True)                                        ' Country"
                accountNumberLabel.Text = .Content("accountNumberLabel", _languageCode, True)
                supplementaryTextLabel.Text = .Content("supplementaryTextLabel", _languageCode, True)
                Newsletter.Text = .Content("SubscribeToNewsletterText", _languageCode, True)
                vatNumberLabel.Text = .Content("vatNumberLabel", _languageCode, True)
                costCenLabel.Text = .Content("costCenLabel", _languageCode, True)                                         ' Cost centre
            End With

            With NewsletterChoice
                .Items.Add(ucr.Content("HTMLNewsletterText", _languageCode, True))
                .Items.Add(ucr.Content("PlainTextNewsletterText", _languageCode, True))
            End With

            'Setup Max Lengths for textboxes
            With ucr
                forename.MaxLength = .Attribute("forenameMaxLength")
                surname.MaxLength = .Attribute("surnameMaxLength")
                position.MaxLength = .Attribute("positionMaxLength")
                email.MaxLength = .Attribute("emailMaxLength")
                emailConfirm.MaxLength = .Attribute("emailMaxLength")
                phone.MaxLength = .Attribute("phoneMaxLength")
                fax.MaxLength = .Attribute("faxMaxLength")
                address1.MaxLength = .Attribute("address1MaxLength")
                address2.MaxLength = .Attribute("address2MaxLength")
                address3.MaxLength = .Attribute("address3MaxLength")
                address4.MaxLength = .Attribute("address4MaxLength")
                address5.MaxLength = .Attribute("address5MaxLength")

                vatNumber.MaxLength = .Attribute("vatNumberMaxLength")
            End With
        Catch ex As Exception
        End Try
    End Sub

    Protected Function UpdateProfile() As Boolean

        If Not CheckPostcode() Then
            ErrorLabel.Text = ucr.Content("PostcodeRequiredFieldValidator", _languageCode, True)
            Return False
        End If

        If LCase(email.Text) = LCase(ModuleDefaults.DefaultTicketingEmailAddress) Then
            ErrorLabel.Text = ucr.Content("InvalidUsernameError", _languageCode, True)
            Return False
        End If

        ' Validate the page in case javascript is turned off
        Page.Validate("Registration")
        If Not Page.IsValid Then
            Return False
        End If

        If UCase(Profile.UserName) <> UCase(email.Text) Then

            If Not Membership.GetUser(email.Text, False) Is Nothing AndAlso Not CType(ModuleDefaults.AllowDuplicateEmail, Boolean) Then
                ErrorLabel.Text = ucr.Content("UserAlreadyExistsErrorText", _languageCode, True)
                Return False
            Else
                If Not ModuleDefaults.LoginidType.Equals("1") And Not ModuleDefaults.LoginidType.Equals("2") Then
                    CType(Membership.Provider, TalentMembershipProvider).ChangeLoginID(Profile.UserName, email.Text)
                End If
            End If
        End If

        With Profile.User.Details
            .Email = email.Text
            .Full_Name = fullName.Text
            .Title = title.SelectedValue
            .Forename = forename.Text
            .DOB = CDate("01/02/1900")
            .Surname = surname.Text
            .Full_Name = forename.Text & " " & surname.Text
            .Position = position.Text
            .Telephone_Number = phone.Text
            .Fax_Number = fax.Text
            .Subscribe_Newsletter = Newsletter.Checked
            .HTML_Newsletter = GetNewsletterType()
        End With

        Profile.PartnerInfo.Details.COST_CENTRE = costCen.Text
        Profile.PartnerInfo.Details.Partner_Desc = companyName.Text

        Dim address As TalentProfileAddress

        If Profile.UserName <> email.Text And Not ModuleDefaults.LoginidType.Equals("1") And Not ModuleDefaults.LoginidType.Equals("2") Then
            For i As Integer = 0 To Profile.User.Addresses.Count - 1
                address = ProfileHelper.ProfileAddressEnumerator(i, Profile.User.Addresses)

                With address
                    .LoginID = email.Text
                End With
            Next
        End If
        address = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)

        With address
            Dim eComDefs As New ECommerceModuleDefaults
            Dim defs As ECommerceModuleDefaults.DefaultValues = eComDefs.GetDefaults
            ' If QAS is switched on then do not show AddressLine1Row (building)

            .Reference = address1.Text & " " & address2.Text
            .Type = String.Empty
            .Default_Address = True
            .Address_Line_1 = address1.Text
            .Address_Line_2 = address2.Text
            .Address_Line_3 = address3.Text
            .Address_Line_4 = address4.Text
            .Address_Line_5 = address5.Text
            .Post_Code = postcode.Text
            If ModuleDefaults.StoreCountryAsWholeName Then
                .Country = UCase(country.SelectedItem.Text)
            Else
                .Country = country.SelectedValue
            End If
            .Sequence = 0
        End With

        'Populate the partnerDetails
        Dim partnerDetails As TalentProfilePartnerDetails
        partnerDetails = Profile.PartnerInfo.Details

        With partnerDetails
            .VAT_NUMBER = vatNumber.Text
        End With

        '-------------------------------------------------------------------------
        ' If from activate account also need to set the new password at this point
        '-------------------------------------------------------------------------
        If Not Session("FromActivation") Is Nothing AndAlso Session("FromActivation") = True Then
            Dim authUsers As New TalentMembershipDatasetTableAdapters.tbl_authorized_usersTableAdapter 
            Dim newPassword As String = ChangeMyPassword1.NewPasswordBox.Text
            Try
                authUsers.ChangePassword_B2B(newPassword, Date.Now, TalentCache.GetBusinessUnit, Profile.User.Details.LoginID, String.Empty)
            Catch ex As Exception

            End Try
            Session("FromActivation") = Nothing
        End If

        If ModuleDefaults.SendRegistrationToBackendFirst Then
            If ModuleDefaults.SendRegistrationToBackEnd Then
                Try
                    SendDetailsToBackend("UPDATE", Profile.User.Details, address, Profile.PartnerInfo.Details)
                    '-----------------------------------------------------------------------------------
                    ' Check there is an address record at this point as from some uploads there won't be
                    '-----------------------------------------------------------------------------------
                    If Profile.User.Addresses.Count = 0 Then
                        If address.LoginID Is Nothing Then
                            address.LoginID = Profile.User.Details.LoginID
                        End If
                        Profile.Provider.AddAddressToUserProfile(address)
                    End If
                    Profile.Save()
                Catch ex As Exception
                    Return False
                End Try
            End If
        Else
            If ModuleDefaults.SendRegistrationToBackEnd Then
                Try
                    '-----------------------------------------------------------------------------------
                    ' Check there is an address record at this point as from some uploads there won't be
                    '-----------------------------------------------------------------------------------
                    If Profile.User.Addresses.Count = 0 Then
                        If address.LoginID Is Nothing Then
                            address.LoginID = Profile.User.Details.LoginID
                        End If
                        Profile.Provider.AddAddressToUserProfile(address)
                    End If
                    Profile.Save()
                    SendDetailsToBackend("UPDATE", Profile.User.Details, address, Profile.PartnerInfo.Details)
                Catch ex As Exception
                    Return False
                End Try
            End If
        End If


        'Was the back-end call a success?
        If ErrorLabel.Text.Trim = "" Then
            Return True
        Else
            Return False
        End If

    End Function

    Protected Function CheckPostcode() As Boolean
        Dim valid As Boolean = True
        Dim cacheKey As String = "Countries_Bu_Table_For_Postcode_Validation_REGISTRATION_" & TalentCache.GetBusinessUnit & "_" & TalentCache.GetPartner(Profile)
        Dim countries As New TalentApplicationVariablesTableAdapters.tbl_country_bu1TableAdapter
        Dim myCountries As New TalentApplicationVariables.tbl_country_bu1DataTable
        If TalentThreadSafe.ItemIsInCache(cacheKey) Then
            myCountries = CType(Cache.Item(cacheKey), TalentApplicationVariables.tbl_country_bu1DataTable)
        Else
            myCountries = countries.GetDataByQualifierBUPartner("REGISTRATION", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
            If myCountries.Rows.Count = 0 Then
                myCountries = countries.GetDataByQualifierBUPartner("*ALL", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                If myCountries.Rows.Count = 0 Then
                    myCountries = countries.GetDataByQualifierBUPartner("*ALL", TalentCache.GetBusinessUnit, "*ALL")
                    If myCountries.Rows.Count = 0 Then
                        myCountries = countries.GetDataByQualifierBUPartner("*ALL", "*ALL", "*ALL")
                    End If
                End If
            End If
            TalentCache.AddPropertyToCache(cacheKey, myCountries, 30, TimeSpan.FromMinutes(30), CacheItemPriority.Low)
            'TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
        End If
        Dim postcodeReq As Boolean = False
        If myCountries.Rows.Count > 0 Then
            For Each myCountry As TalentApplicationVariables.tbl_country_bu1Row In myCountries.Rows
                If myCountry.COUNTRY_CODE.ToLower = country.SelectedValue.ToLower Then
                    postcodeReq = myCountry.POSTCODE_MANDATORY
                    Exit For
                End If
            Next
        End If

        If postcodeReq Then
            If String.IsNullOrEmpty(postcode.Text) Then
                valid = False
            End If
        End If

        Return valid
    End Function

    Protected Sub updateBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles updateBtn.Click

        Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

        ErrorLabel.Text = ""
        Try
            If UpdateProfile() Then

                Try
                    FormsAuthentication.SignOut()
                    ' 
                    'Dim tmu As TalentMembershipUser = Membership.Provider.GetUser(email.Text, True)
                    'FormsAuthentication.Authenticate(email.Text, tmu.Password)
                    'Profile.Initialize(email.Text, True)
                    'FormsAuthentication.SetAuthCookie(email.Text, False)
                    '
                    ' Use login id not email
                    Dim tmu As TalentMembershipUser = Membership.Provider.GetUser(Profile.User.Details.LoginID, True)
                    'FormsAuthentication.Authenticate(Profile.User.Details.LoginID, tmu.Password)
                    Membership.ValidateUser(Profile.User.Details.LoginID, tmu.Password)
                    Profile.Initialize(Profile.User.Details.LoginID, True)
                    FormsAuthentication.SetAuthCookie(Profile.User.Details.LoginID, False)
                Catch ex As Exception
                End Try

                If Request.QueryString("ReturnUrl") Is Nothing Then
                    Response.Redirect("~/PagesLogin/Profile/updateProfileConfirmation.aspx")
                Else
                    Response.Redirect(Request.QueryString("ReturnUrl"))
                End If
            End If
        Catch ex As Exception
            ErrorLabel.Text = ucr.Content("ErrorUpdatingFrontEndRecords", _languageCode, True)
        End Try

    End Sub

    Protected Sub PopulateTitleDropDownList()
        title.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "REGISTRATION", "TITLE")
        title.DataTextField = "Text"
        title.DataValueField = "Value"
        title.DataBind()
    End Sub

    Protected Sub PopulateCountriesDropDownList()
        country.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "REGISTRATION", "COUNTRY")
        country.DataTextField = "Text"
        country.DataValueField = "Value"
        country.DataBind()
    End Sub

    Public Sub SetupCompareValidator(ByVal sender As Object, ByVal e As EventArgs)
        Try
            'add if statement here to check that RFV is
            'enabled using control attribute setting. (AA)
            SetUpUCR()
            Dim cv As CompareValidator = CType(sender, CompareValidator)
            cv.ErrorMessage = ucr.Content(cv.ControlToValidate & "CompareValidator", _languageCode, True)

        Catch ex As Exception
        End Try
    End Sub
    Public Sub SetupRequiredValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim rfv As RequiredFieldValidator = CType(sender, RequiredFieldValidator)

        Try
            SetUpUCR()
            If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute(rfv.ControlToValidate & "EnableRFV")) Then
                Try

                    rfv.ErrorMessage = ucr.Content(rfv.ControlToValidate & "RequiredFieldValidator", _languageCode, True)
                Catch ex As Exception
                End Try
            Else
                rfv.Enabled = False
            End If
        Catch ex As Exception
        End Try
    End Sub
    Protected Sub SetupRegExValidator(ByVal sender As Object, ByVal e As EventArgs)

        SetUpUCR()
        Dim rev As RegularExpressionValidator = CType(sender, RegularExpressionValidator)

        rev.ErrorMessage = ucr.Content(rev.ControlToValidate & "ErrorText", _languageCode, True)

        If Not String.IsNullOrEmpty(ucr.Attribute(rev.ControlToValidate & "RegEx")) Then
            rev.ValidationExpression = ucr.Attribute(rev.ControlToValidate & "RegEx")
        Else
            Select Case (rev.ControlToValidate)

                Case Is = "title"
                    'rev.ValidationExpression = "^(?! -- )?[a-zA-Z\s]+"
                    rev.ValidationExpression = "^[a-zA-Z\s]{0,50}$"

                Case Is = "forename", _
                            "surname", _
                            "companyName", _
                            "organisation", _
                            "position", _
                            "salutation"

                    rev.ValidationExpression = ucr.Attribute("TextOnlyExpression")

                Case Is = "initials"
                    rev.ValidationExpression = ucr.Attribute("InitialsExpression")

                Case Is = "email"
                    rev.ValidationExpression = ucr.Attribute("EmailExpression")

                Case Is = "phone", _
                            "fax"
                    rev.ValidationExpression = ucr.Attribute("PhoneNumberExpression")

                Case Is = "address1", _
                            "address2", _
                            "address3", _
                            "address4", _
                            "address5"
                    rev.ValidationExpression = ucr.Attribute("TextAndNumberExpression")

                Case Is = "postcode"
                    rev.ValidationExpression = ucr.Attribute("PostcodeExpression")

                Case Is = "country"
                    'rev.ValidationExpression = "^(?! -- )?[a-zA-Z\s]+"
                    rev.ValidationExpression = "^[a-zA-Z\s]{0,50}$"

            End Select
        End If
    End Sub

    Protected Sub SetupDisplayType(ByVal sender As Object, ByVal e As EventArgs)

        SetUpUCR()
        Dim htmlCtl As HtmlGenericControl = CType(sender, HtmlGenericControl)

        ' This function is atteched to each row of the user control
        ' On initialisation, check to see what should be done to the row (non display / output only / editable field)

        Select Case ucr.Attribute(htmlCtl.ID.Trim & "DisplayType").ToUpper

            '
            ' Hidden field. Set the row to non-display and disable any validation controls.
            '
            Case Is = "HIDDEN"
                htmlCtl.Visible = False

                '
                ' Output field. Set all child controls to output only and disable any validation controls.
                '
            Case Is = "OUTPUT"
                htmlCtl.Visible = True
                For Each child As Control In htmlCtl.Controls
                    Dim webCtl As WebControl
                    Dim labelCtl As Label

                    Try
                        webCtl = CType(child, WebControl)
                        Try
                            labelCtl = CType(child, Label)
                        Catch ex As Exception
                            '
                            ' Do not disable the label controls.
                            '
                            webCtl.Enabled = False
                            webCtl.Attributes.Add("disabled", True)
                        End Try
                    Catch ex As Exception
                        'Only interested in disabling web controls
                    End Try

                Next

                '
                ' Input/Output field. No processing as defaults will automatically display the fields for editing.
                '
            Case Is = "INPUTOUTPUT"

        End Select
    End Sub

    Protected Sub SendConfirmationEmail()
        Dim defaults As ECommerceModuleDefaults.DefaultValues
        Dim defs As New ECommerceModuleDefaults
        defaults = defs.GetDefaults

        Dim body As String = ucr.Content("ConfirmationEmailBody", _languageCode, True)
        body = body.Replace("<<To>>", forename.Text)
        body = body.Replace("<<NewLine>>", vbCrLf)
        body = body.Replace("<<UserName>>", email.Text)
        Talent.Common.Utilities.SMTP = ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim
        Talent.Common.Utilities.SMTPPortNumber = Talent.eCommerce.Utilities.GetSMTPPortNumber
        body = body.Replace("<<WebSiteAddress>>", Talent.eCommerce.Utilities.GetCurrentApplicationUrl & "/PagesPublic/Home/Home.aspx")

        Dim err As Talent.Common.ErrorObj = Talent.Common.Utilities.Email_Send(defaults.RegistrationConfirmationFromEmail, email.Text, _
                                            ucr.Content("ConfirmationEmailSubject", _languageCode, True), _
                                            body)
    End Sub

    Protected Function GetNewsletterType() As Boolean
        If NewsletterChoice.SelectedValue = ucr.Content("HTMLNewsletterText", _languageCode, True) Then
            Return True
        ElseIf NewsletterChoice.SelectedValue = ucr.Content("PlainTextNewsletterText", _languageCode, True) Then
            Return False
        Else
            Return False
        End If
    End Function


    Protected Sub SendDetailsToBackend(ByVal call_origin As String, _
                                    ByVal userDetails As TalentProfileUserDetails, _
                                    ByVal userAddress As TalentProfileAddress, _
                                    ByVal partnerDetails As TalentProfilePartnerDetails)

        Dim myCustomer As New TalentCustomer
        Dim deCustV11 As New DECustomerV11
        Dim deCustV1 As New DECustomer
        deCustV11.DECustomersV1.Add(deCustV1)

        Dim myErrorObj As New ErrorObj
        Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        Dim p As New TalentProfileUser
        Dim address As New TalentProfileAddress

        'Set the profile information
        'If CType(def.SendRegistrationToBackendFirst, Boolean) Then
        If Not userDetails Is Nothing AndAlso Not userAddress Is Nothing Then
            p.Details = userDetails
            address = userAddress
        Else
            p = Profile.Provider.GetUserByLoginID(email.Text)
            address = ProfileHelper.ProfileAddressEnumerator(0, p.Addresses)
        End If
        '
        ' Set the Customer object and invoke the SetCustomer method to post details to TALENT Customer Manager
        '
        With myCustomer
            .DeV11 = deCustV11
            ' Reset the settings entity to a customer specific settings entity 
            .Settings = CType(New DECustomerSettings, DESettings)

            With deCustV1
                .Action = ""
                '    .ThirdPartyContactRef = ucr.Content("crmExternalKeyName", _languageCode, True)
                .ThirdPartyContactRef = p.Details.User_Number
                '.ThirdPartyCompanyRef1 = p.Details.User_Number
                '.ThirdPartyCompanyRef1Supplement = p.Details.User_Number_Prefix
                .ThirdPartyCompanyRef2 = ""
                .DateFormat = "1"

                If CType(def.LoginidIsCustomerNumber, Boolean) Then
                    .CustomerNumber = p.Details.LoginID
                End If
                .ContactSurname = p.Details.Surname
                .ContactForename = p.Details.Forename
                .ContactTitle = p.Details.Title
                .ContactInitials = p.Details.Initials
                .DateBirth = ("1900").PadLeft(2, "0") & _
                                ("02").PadLeft(2, "0") & _
                                ("01").PadLeft(2, "0")
                .Gender = String.Empty
                .EmailAddress = p.Details.Email
                '---------------------------------------
                ' Use company SL account no, not contact
                '---------------------------------------
                .CompanySLNumber1 = p.Details.Account_No_1
                .CompanySLNumber2 = p.Details.Account_No_2
                '.SLNumber1 = p.Details.Account_No_1
                '.SLNumber2 = p.Details.Account_No_2

                .ProcessContactSurname = "1"
                .ProcessContactForename = "1"
                .ProcessContactTitle = "1"
                .ProcessContactInitials = "1"
                .ProcessDateBirth = "1"
                .ProcessEmailAddress = "1"

                '.ProcessSLNumber1 = "1"
                '.ProcessSLNumber2 = "1"

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
                Dim eComDefs As New ECommerceModuleDefaults
                Dim defs As ECommerceModuleDefaults.DefaultValues = eComDefs.GetDefaults
                ' If QAS is switched on then do not show AddressLine1Row (building)
                .AddressLine1 = address.Address_Line_2.Trim
                .AddressLine2 = address.Address_Line_3.Trim
                .AddressLine3 = address.Address_Line_4.Trim
                .AddressLine4 = address.Address_Line_5.Trim
                .AddressLine5 = UCase(address.Country.Trim)
                .PostCode = address.Post_Code
                '--------------------------------------------------------
                ' Check if blanks are to be removed from ph number before 
                ' sending to the back end
                '--------------------------------------------------------
                .HomeTelephoneNumber = p.Details.Telephone_Number
                .WorkTelephoneNumber = p.Details.Work_Number
                .MobileNumber = p.Details.Mobile_Number
                If def.RegistrationRemoveBlanksFromTelNo Then
                    If Not .HomeTelephoneNumber Is Nothing Then .HomeTelephoneNumber = .HomeTelephoneNumber.Replace(" ", String.Empty)
                    If Not .WorkTelephoneNumber Is Nothing Then .WorkTelephoneNumber = .WorkTelephoneNumber.Replace(" ", String.Empty)
                    If Not .MobileNumber Is Nothing Then .MobileNumber = .MobileNumber.Replace(" ", String.Empty)
                End If
                .CompanyName = address.Address_Line_1
                .ProcessCompanyName = "1"
                .ProcessCompanySLNumber1 = "1"
                .ProcessCompanySLNumber2 = "1"

                .ProcessAddressLine1 = "1"
                .ProcessAddressLine2 = "1"
                .ProcessAddressLine3 = "1"
                .ProcessAddressLine4 = "1"
                .ProcessAddressLine5 = "1"
                .ProcessPostCode = "1"
                .ProcessHomeTelephoneNumber = "1"
                .ProcessWorkTelephoneNumber = "1"
                .ProcessMobileNumber = "1"

                .ProcessAttributes = "1"
                ' /001 Process Newsletter Prefs attributes
                If Newsletter.Checked Then
                    .Attribute01 = ucr.Attribute("newsletterAttribute")
                    .Attribute01Action = "A"
                    If NewsletterChoice.SelectedValue = ucr.Content("HTMLNewsletterText", _languageCode, True) Then
                        .Attribute02 = ucr.Attribute("newsletterAttributeTypeHTML")
                        .Attribute02Action = "A"
                        .Attribute03 = ucr.Attribute("newsletterAttributeTypePlain")
                        .Attribute03Action = "D"
                    ElseIf NewsletterChoice.SelectedValue = ucr.Content("PlainTextNewsletterText", _languageCode, True) Then
                        .Attribute02 = ucr.Attribute("newsletterAttributeTypePlain")
                        .Attribute02Action = "A"
                        .Attribute03 = ucr.Attribute("newsletterAttributeTypeHTML")
                        .Attribute03Action = "D"
                    End If
                Else
                    .Attribute01 = ucr.Attribute("newsletterAttribute")
                    .Attribute01Action = "D"
                    .Attribute02 = ucr.Attribute("newsletterAttributeTypeHTML")
                    .Attribute02Action = "D"
                    .Attribute03 = ucr.Attribute("newsletterAttributeTypePlain")
                    .Attribute03Action = "D"
                End If

                .VatCode = partnerDetails.VAT_NUMBER
                .ProcessVatCode = "1"

            End With

            Dim decs As New DECustomerSettings()
            decs = CType(.Settings, DECustomerSettings)
            decs.CreationType = "UPDATE"
            .Settings = CType(decs, DESettings)

            With .Settings
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .BusinessUnit = TalentCache.GetBusinessUnit
                .Company = ucr.Content("crmExternalKeyName", _languageCode, True)
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

            'Only serialize the customer when the call to the backend can be ignored
            If Not CType(def.SendRegistrationToBackendFirst, Boolean) Then
                If myErrorObj.HasError Then
                    Dim sendEmail As Boolean = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("SendBackEndFailureEmail"))
                    Talent.eCommerce.Utilities.SerializeObject(myCustomer, _
                                                                myCustomer.GetType, _
                                                                TalentCache.GetBusinessUnit, _
                                                                TalentCache.GetPartner(Profile), _
                                                                Profile.UserName, _
                                                                call_origin, _
                                                                sendEmail, _
                                                                Context)
                End If
            Else
                'Check for errors
                ' Handle success or failure
                ' (XX = Failure in accessing the iSeries data (...all types of issues))
                ' (else = Specific errors (...including no error)
                If myErrorObj.HasError Then
                    ErrorLabel.Text = Talent.Common.Utilities.getErrorDescription(ucr, _languageCode, ("XX"), True)
                Else
                    'Has an error has occurred
                    If myCustomer.DeV11.DECustomersV1(0).ErrorCode.Trim <> "" Then
                        ErrorLabel.Text = Talent.Common.Utilities.getErrorDescription(ucr, _languageCode, myCustomer.DeV11.DECustomersV1(0).ErrorCode, True)
                    End If
                End If
            End If
        End With
    End Sub
    Protected Function SendDetailsToBackendFirst(ByVal sendRegistrationToBackEndFirst As Boolean, _
                                               ByVal userDetails As TalentProfileUserDetails, _
                                               ByVal userAddress As TalentProfileAddress, _
                                               ByVal partnerDetails As TalentProfilePartnerDetails) As Boolean

        Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

        ' Are we sending the customer information to the back end first
        If Not CType(sendRegistrationToBackEndFirst, Boolean) Or _
            Not CType(def.SendRegistrationToBackEnd, Boolean) Then
            Return True
        End If

        'Send the information to the backend 
        SendDetailsToBackend("UPDATE", userDetails, userAddress, partnerDetails)

        'Was the call a success?
        If ErrorLabel.Text.Trim = "" Then
            Return True
        Else
            Return False
        End If

    End Function

    Protected Function GetAddressingLinkText() As String
        Return ucr.Content("addressingLinkButtonText", _languageCode, True)
    End Function
    Protected Sub CreateAddressingJavascript()

        If CType(ucr.Attribute("addressingOnOff"), Boolean) Then

            Dim defaults As ECommerceModuleDefaults.DefaultValues
            Dim defs As New ECommerceModuleDefaults
            Dim sString As String = String.Empty
            defaults = defs.GetDefaults


            Response.Write(vbCrLf & "<script language=""javascript"" type=""text/javascript"">" & vbCrLf)

            Select Case defaults.AddressingProvider.ToUpper

                Case Is = "QAS"
                    ' Create function to open child window
                    Response.Write("function addressingPopup() {" & vbCrLf)
                    Response.Write("win1 = window.open('../../PagesPublic/QAS/FlatCountry.aspx', 'QAS', '" & ucr.Attribute("addressingWindowAttributes") & "');" & vbCrLf)
                    Response.Write("win1.creator=self;" & vbCrLf)
                    Response.Write("}" & vbCrLf)
                Case Is = "HOPEWISER"
                    ' Create function to open child window
                    Response.Write("function addressingPopup() {" & vbCrLf)
                    Response.Write("win1 = window.open('../../PagesPublic/Hopewiser/HopewiserPostcodeAndCountry.aspx', 'Hopewiser', '" & ucr.Attribute("addressingWindowAttributes") & "');" & vbCrLf)
                    Response.Write("win1.creator=self;" & vbCrLf)
                    Response.Write("}" & vbCrLf)
            End Select

            Dim sAllFields() As String = defaults.AddressingFields.ToString.Split(",")
            Dim count As Integer = 0

            '
            ' Create function to populate the address fields.  This function is called from FlatAddress.aspx.
            Response.Write("function UpdateAddressFields() {" & vbCrLf)

            '
            ' Create local function variables used to indicate whether an address element has already been used.
            Do While count < sAllFields.Length
                Response.Write("var usedHiddenAdr" & count.ToString & " = '';" & vbCrLf)
                count = count + 1
            Loop

            If AddressLine1Row.Visible Then Response.Write("document.forms[0]." & address1.UniqueID & ".value = '';" & vbCrLf)
            If AddressLine2Row.Visible Then Response.Write("document.forms[0]." & address2.UniqueID & ".value = '';" & vbCrLf)
            If AddressLine3Row.Visible Then Response.Write("document.forms[0]." & address3.UniqueID & ".value = '';" & vbCrLf)
            If AddressLine4Row.Visible Then Response.Write("document.forms[0]." & address4.UniqueID & ".value = '';" & vbCrLf)
            If AddressLine5Row.Visible Then Response.Write("document.forms[0]." & address5.UniqueID & ".value = '';" & vbCrLf)
            If AddressPostcodeRow.Visible Then Response.Write("document.forms[0]." & postcode.UniqueID & ".value = '';" & vbCrLf)
            If AddressCountryRow.Visible Then Response.Write("document.forms[0]." & country.UniqueID & ".value = '';" & vbCrLf)

            If AddressLine1Row.Visible And Not defaults.AddressingMapAdr1.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & address1.UniqueID & ".value", defaults.AddressingMapAdr1, defaults.AddressingFields)
                Response.Write(sString)
            End If
            If AddressLine2Row.Visible And Not defaults.AddressingMapAdr2.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & address2.UniqueID & ".value", defaults.AddressingMapAdr2, defaults.AddressingFields)
                Response.Write(sString)
            End If
            If AddressLine3Row.Visible And Not defaults.AddressingMapAdr3.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & address3.UniqueID & ".value", defaults.AddressingMapAdr3, defaults.AddressingFields)
                Response.Write(sString)
            End If
            If AddressLine4Row.Visible And Not defaults.AddressingMapAdr4.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & address4.UniqueID & ".value", defaults.AddressingMapAdr4, defaults.AddressingFields)
                Response.Write(sString)
            End If
            If AddressLine5Row.Visible And Not defaults.AddressingMapAdr5.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & address5.UniqueID & ".value", defaults.AddressingMapAdr5, defaults.AddressingFields)
                Response.Write(sString)
            End If
            If Not defaults.AddressingMapPost.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & postcode.UniqueID & ".value", defaults.AddressingMapPost, defaults.AddressingFields)
                Response.Write(sString)
            End If
            If Not defaults.AddressingMapCountry.Trim = "" Then
                sString = GetJavascriptString("document.forms[0]." & country.UniqueID & ".value", defaults.AddressingMapCountry, defaults.AddressingFields)
                Response.Write(sString)
            End If

            Response.Write("}" & vbCrLf)

            Response.Write("function trim(s) { " & vbCrLf & "var r=/\b(.*)\b/.exec(s); " & vbCrLf & "return (r==null)?"""":r[1]; " & vbCrLf & "}")
            Response.Write("</script>" & vbCrLf)
        End If

    End Sub
    Protected Function GetJavascriptString(ByVal sFieldString, ByVal sAddressingMap, ByVal sAddressingFields) As String

        Dim sString As String = String.Empty
        Dim count As Integer = 0
        Dim count2 As Integer = 0
        Const sStr1 As String = "document.forms[0].hiddenAdr"
        Const sStr2 As String = ".value"
        Const sStr3 As String = "usedHiddenAdr"

        Dim sAddressingMapFields() As String = sAddressingMap.ToString.Split(",")
        Dim sAddressingAllFields() As String = sAddressingFields.ToString.Split(",")

        Do While count < sAddressingMapFields.Length
            If Not sAddressingMapFields(count).Trim = "" Then
                count2 = 0
                Do While count2 < sAddressingAllFields.Length
                    If sAddressingMapFields(count).Trim = sAddressingAllFields(count2).Trim Then
                        sString = sString & vbCrLf & _
                                "if (trim(" & sStr3 & count2.ToString & ") != 'Y' && trim(" & sStr1 & count2.ToString & sStr2 & ") != '') {" & vbCrLf & _
                                "if (trim(" & sFieldString & ") == '') {" & vbCrLf & _
                                sFieldString & " = " & sStr1 & count2.ToString & sStr2 & ";" & vbCrLf & _
                                "}" & vbCrLf & _
                                "else {" & vbCrLf & _
                                sFieldString & " = " & sFieldString & " + ', ' + " & sStr1 & count2.ToString & sStr2 & ";" & vbCrLf & _
                                "}" & vbCrLf & _
                                sStr3 & count2.ToString & " = 'Y';" & vbCrLf & _
                                "}"
                        Exit Do
                    End If
                    count2 = count2 + 1
                Loop
            End If
            count = count + 1
        Loop

        Return sString

    End Function
    Protected Sub CreateAddressingHiddenFields()

        '
        ' Create hidden fields for each Addressing field defined in defaults.
        Dim defaults As ECommerceModuleDefaults.DefaultValues
        Dim defs As New ECommerceModuleDefaults
        Dim qasFields() As String = Nothing
        Dim count As Integer = 0
        Dim sString As String = String.Empty

        defaults = defs.GetDefaults

        If CType(ucr.Attribute("addressingOnOff"), Boolean) Then
            qasFields = defaults.AddressingFields.ToString.Split(",")
            Do While count < qasFields.Length
                If count = 0 Then
                    Response.Write(vbCrLf)
                End If
                sString = "<input type=""hidden"" name=""hiddenAdr" & count.ToString & """ value="" "" />"
                Response.Write(sString & vbCrLf)
                count = count + 1
            Loop
        End If

    End Sub

End Class
