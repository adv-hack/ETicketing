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
Partial Class UserControls_RegistrationForm2
    Inherits ControlBase

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private ucr As New Talent.Common.UserControlResource
    Private addressLine1RowVisible As Boolean = True
    Private addressLine2RowVisible As Boolean = True
    Private addressLine3RowVisible As Boolean = True
    Private addressLine4RowVisible As Boolean = True
    Private addressLine5RowVisible As Boolean = True
    Private addressPostcodeRowVisible As Boolean = True
    Private addressCountryRowVisible As Boolean = True
    Private addressCompanyNameRowVisible As Boolean = True
    Private addressOrganisationRowVisible As Boolean = True
    Private opt1TrueValue As Boolean = True
    Private opt2TrueValue As Boolean = True
    Private opt3TrueValue As Boolean = True
    Private opt4TrueValue As Boolean = True
    Private opt5TrueValue As Boolean = True
    Dim myDefs As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults

    Public Display As Boolean = True

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Display Then
            SetEnterButtonClickEvent()
        Else
            Me.Visible = False
        End If
        If Me.Display Then
            If Not Page.IsPostBack Then
                If ucr.Attribute("showCustomerAccountNumber").ToString.ToUpper = "FALSE" Then
                    accountNumberLabel.Visible = False
                    accountNumber.Visible = False
                    accountNumberRow.Visible = False
                End If
                If ucr.Attribute("showCustomerAccountNumber2").ToString.ToUpper = "FALSE" Then
                    accountNumberLabel2.Visible = False
                    accountNumber2.Visible = False
                    accountNumberRow2.Visible = False
                End If
                If ucr.Attribute("showCustomerAccountNumber3").ToString.ToUpper = "FALSE" Then
                    accountNumberLabel3.Visible = False
                    accountNumber3.Visible = False
                    accountNumberRow3.Visible = False
                End If
                If ucr.Attribute("showCustomerAccountNumber4").ToString.ToUpper = "FALSE" Then
                    accountNumberLabel4.Visible = False
                    accountNumber4.Visible = False
                    accountNumberRow4.Visible = False
                End If
                If ucr.Attribute("showCustomerAccountNumber5").ToString.ToUpper = "FALSE" Then
                    accountNumberLabel5.Visible = False
                    accountNumber5.Visible = False
                    accountNumberRow5.Visible = False
                End If
                If ucr.Attribute("showLoginId").ToString.ToUpper = "FALSE" Then
                    loginIDLabel.Visible = False
                    loginid.Visible = False
                    usernameRow.Visible = False
                End If
            End If

        End If

    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Me.Display Then
            SetUpUCR()
            If Not Page.IsPostBack Then
                SetUpOpt()
                PopulateTitleDropDownList()
                PopulateCountriesDropDownList()
                PopulateCompanyDetailsDropDownLists()
                SetAddressVisibilityProperties()
                SetAddressVisibility()
                SetLabelText()
            End If
        End If

    End Sub

    Protected Sub SetEnterButtonClickEvent()
        If Me.Display Then
            Dim buttonID As String = String.Empty
            buttonID = registerBtn.ClientID
            Const onKeyDown As String = "onkeydown"
            Dim EventText As String = "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" & buttonID & "').click();return false;}} else {return true}; "
            Dim AddressEventText As String = String.Empty
            If CType(ucr.Attribute("addressingOnOff"), Boolean) Then
                AddressEventText = "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {addressingPopup();return false;}} else {return true}; "
            Else
                AddressEventText = "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {return false;}} else {return true}; "
            End If
            title.Attributes.Add(onKeyDown, EventText)
            forename.Attributes.Add(onKeyDown, EventText)
            surname.Attributes.Add(onKeyDown, EventText)
            position.Attributes.Add(onKeyDown, EventText)
            email.Attributes.Add(onKeyDown, EventText)
            emailConfirm.Attributes.Add(onKeyDown, EventText)
            phone.Attributes.Add(onKeyDown, EventText)
            fax.Attributes.Add(onKeyDown, EventText)
            Address1.Attributes.Add(onKeyDown, EventText)
            postcode.Attributes.Add(onKeyDown, AddressEventText)
            Address2.Attributes.Add(onKeyDown, EventText)
            Address3.Attributes.Add(onKeyDown, EventText)
            Address4.Attributes.Add(onKeyDown, EventText)
            Address5.Attributes.Add(onKeyDown, EventText)
            country.Attributes.Add(onKeyDown, EventText)
        End If

    End Sub

    Protected Sub SetUpUCR()
        If Me.Display Then
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.Common.Utilities.GetAllString 'GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "RegistrationForm2.ascx"
            End With
        End If

    End Sub


    Private Sub SetLabelText()
        If Me.Display Then
            Try
                RegistrationHeaderLabel.Text = ucr.Content("registrationHeaderLabel1", _languageCode, True)             ' CSG Registration"
                registerBtn.Text = ucr.Content("registerBtn", _languageCode, True)                                      ' Register"

                With ucr
                    PersonalDetailsLabel.Text = .Content("personalDetailsLabel", _languageCode, True)                        ' Personal Details"
                    AddressInfoLabel.Text = .Content("addressInfoLabel", _languageCode, True)                                ' Address Info"
                    CompanyInfoLabel.Text = .Content("companyInfoLabel", _languageCode, True)
                    '
                    emailLabel.Text = .Content("emailLabel", _languageCode, True)                                            ' Email"
                    emailConfirmLabel.Text = .Content("ConfirmEmailLabel", _languageCode, True)                              ' Confirm Email"
                    titleLabel.Text = .Content("titleLabel", _languageCode, True)                                            ' Title"
                    forenameLabel.Text = .Content("forenameLabel", _languageCode, True)                                      ' Forename"
                    surnameLabel.Text = .Content("surnameLabel", _languageCode, True)                                        ' Surname"

                    positionLabel.Text = .Content("positionLabel", _languageCode, True)                                      ' Position"
                    '
                    phoneLabel.Text = .Content("phoneLabel", _languageCode, True)                                            ' Telephone Number"
                    faxLabel.Text = .Content("faxLabel", _languageCode, True)                                                ' Fax Number"
                    '
                    CredentialsLabel.Text = .Content("credentialsLabel", _languageCode, True)                                ' Login Credentials"
                    TandCs.Text = .Content("AcceptTandCsText", _languageCode, True)                                          ' Terms and Conditions"
                    passwordInstructionsLabel.Text = ucr.Content("passwordInstructionsLabel", _languageCode, True)          ' (Passwords need to be 6-10 characters long and contain at least 1 number)"
                    loginIDLabel.Text = ucr.Content("loginIDLabel", _languageCode, True)                                    ' Username"
                    password1Label.Text = ucr.Content("password1Label", _languageCode, True)                                ' Password"
                    password2Label.Text = ucr.Content("password2Label", _languageCode, True)

                    lblAddress1.Text = .Content("address1Label", _languageCode, True)                                      ' Building Name/No"
                    lblAddress2.Text = .Content("address2Label", _languageCode, True)                                          ' Street"
                    lblAddress3.Text = .Content("address3Label", _languageCode, True)                                              ' Town"
                    lblAddress4.Text = .Content("address4Label", _languageCode, True)                                              ' City"
                    lblAddress5.Text = .Content("address5Label", _languageCode, True)
                    postcodeLabel.Text = .Content("postcodeLabel", _languageCode, True)                                      ' Postcode"
                    countryLabel.Text = .Content("countryLabel", _languageCode, True)                                        ' Country"

                    accountNumberLabel.Text = .Content("accountNumberLabel", _languageCode, True)
                    accountNumberLabel2.Text = .Content("accountNumberLabel2", _languageCode, True)
                    accountNumberLabel3.Text = .Content("accountNumberLabel3", _languageCode, True)
                    accountNumberLabel4.Text = .Content("accountNumberLabel4", _languageCode, True)
                    accountNumberLabel5.Text = .Content("accountNumberLabel5", _languageCode, True)

                    supplementaryTextLabel.Text = .Content("supplementaryTextLabel", _languageCode, True)

                    vatNumberLabel.Text = .Content("vatNumberLabel", _languageCode, True)
                    lblCompanyDetails1.Text = .Content("companyDetails1Label", _languageCode, True)
                    lblCompanyDetails2.Text = .Content("companyDetails2Label", _languageCode, True)

                    Newsletter.Text = .Content("SubscribeToNewsletterText", _languageCode, True)

                End With
                With NewsletterChoice
                    .Items.Add(ucr.Content("HTMLNewsletterText", _languageCode, True))
                    .Items.Add(ucr.Content("PlainTextNewsletterText", _languageCode, True))
                End With
                '
                'Setup Max Lengths for textboxes
                With ucr
                    forename.MaxLength = .Attribute("forenameMaxLength")
                    surname.MaxLength = .Attribute("surnameMaxLength")
                    position.MaxLength = .Attribute("positionMaxLength")
                    email.MaxLength = .Attribute("emailMaxLength")
                    emailConfirm.MaxLength = .Attribute("emailMaxLength")
                    phone.MaxLength = .Attribute("phoneMaxLength")
                    fax.MaxLength = .Attribute("faxMaxLength")

                    Address1.MaxLength = .Attribute("address1MaxLength")
                    postcode.MaxLength = .Attribute("postcodeMaxLength")
                    Address2.MaxLength = .Attribute("address2MaxLength")
                    Address3.MaxLength = .Attribute("address3MaxLength")
                    Address4.MaxLength = .Attribute("address4MaxLength")
                    Address5.MaxLength = .Attribute("address5MaxLength")

                    accountNumber.MaxLength = .Attribute("accountNumberMaxLength")
                    accountNumber2.MaxLength = .Attribute("accountNumberMaxLength2")
                    accountNumber3.MaxLength = .Attribute("accountNumberMaxLength3")
                    accountNumber4.MaxLength = .Attribute("accountNumberMaxLength4")
                    accountNumber5.MaxLength = .Attribute("accountNumberMaxLength5")

                    loginid.MaxLength = .Attribute("loginIDMaxLength")
                    password1.MaxLength = .Attribute("passwordMaxLength")
                    password2.MaxLength = .Attribute("passwordMaxLength")

                    vatNumber.MaxLength = .Attribute("vatNumberMaxLength")
                End With

            Catch ex As Exception
            End Try
        End If

    End Sub
    Public Sub SetupCompareValidator(ByVal sender As Object, ByVal e As EventArgs)
        If Me.Display Then
            Try
                SetUpUCR()
                Dim cv As CompareValidator = CType(sender, CompareValidator)
                Select Case cv.ControlToValidate
                    Case Is = "emailConfirm"
                        cv.ErrorMessage = ucr.Content("CompareEmailErrorText", _languageCode, True)
                    Case Else
                        cv.ErrorMessage = ucr.Content(cv.ControlToValidate & "CompareValidator", _languageCode, True)
                End Select
            Catch ex As Exception
            End Try
        End If

    End Sub

    Protected Sub SetUpOpt()

        If Me.Display Then
            With ucr
                'do visibility
                opt1.Visible = CType(.Attribute("opt1Enabled"), Boolean)
                opt2.Visible = CType(.Attribute("opt2Enabled"), Boolean)
                opt3.Visible = CType(.Attribute("opt3Enabled"), Boolean)
                opt4.Visible = CType(.Attribute("opt4Enabled"), Boolean)
                opt5.Visible = CType(.Attribute("opt5Enabled"), Boolean)
                'do default status
                opt1.Checked = CType(.Attribute("opt1Default"), Boolean)
                opt2.Checked = CType(.Attribute("opt2Default"), Boolean)
                opt3.Checked = CType(.Attribute("opt3Default"), Boolean)
                opt4.Checked = CType(.Attribute("opt4Default"), Boolean)
                opt5.Checked = CType(.Attribute("opt5Default"), Boolean)
                'do meaning
                opt1TrueValue = CType(.Attribute("opt1TrueValue"), Boolean)
                opt2TrueValue = CType(.Attribute("opt2TrueValue"), Boolean)
                opt3TrueValue = CType(.Attribute("opt3TrueValue"), Boolean)
                opt4TrueValue = CType(.Attribute("opt4TrueValue"), Boolean)
                opt5TrueValue = CType(.Attribute("opt5TrueValue"), Boolean)
                'do text
                opt1.Text = CType(.Content("opt1Text", _languageCode, True), String)
                opt2.Text = CType(.Content("opt2Text", _languageCode, True), String)
                opt3.Text = CType(.Content("opt3Text", _languageCode, True), String)
                opt4.Text = CType(.Content("opt4Text", _languageCode, True), String)
                opt5.Text = CType(.Content("opt5Text", _languageCode, True), String)
            End With
        End If


    End Sub

    Public Sub SetupRequiredValidator(ByVal sender As Object, ByVal e As EventArgs)
        If Me.Display Then
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
        End If

    End Sub
    Protected Sub SetupRegExValidator(ByVal sender As Object, ByVal e As EventArgs)
        If Me.Display Then
            SetUpUCR()
            Dim rev As RegularExpressionValidator = CType(sender, RegularExpressionValidator)

            rev.ErrorMessage = ucr.Content(rev.ControlToValidate & "ErrorText", _languageCode, True)

            If Not String.IsNullOrEmpty(ucr.Attribute(rev.ControlToValidate & "RegEx")) Then
                rev.ValidationExpression = ucr.Attribute(rev.ControlToValidate & "RegEx")
            Else
                Select Case (rev.ControlToValidate)
                    Case Is = "password1"
                        rev.ValidationExpression = ucr.Attribute("PasswordExpression")

                    Case Is = "title"
                        'rev.ValidationExpression = "^(?! -- )?[a-zA-Z\s]+"
                        rev.ValidationExpression = "^[a-zA-Z\s]{0,50}$"

                    Case Is = "forename", _
                                "surname", _
                                "companyName", _
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

                    Case Is = "Address1", _
                                "Address2", _
                                    "Address3", _
                                "Address4", _
                                "Address5"
                        rev.ValidationExpression = ucr.Attribute("TextAndNumberExpression")

                    Case Is = "postcode"
                        rev.ValidationExpression = ucr.Attribute("PostcodeExpression")

                    Case Is = "country"
                        'rev.ValidationExpression = "^(?! -- )?[a-zA-Z\s]+"
                        rev.ValidationExpression = "^[a-zA-Z\s]{0,50}$"

                End Select
            End If
        End If

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

        'Select the default country
        If myDefs.UseDefaultCountryOnRegistration Then
            Dim defaultCountry As String = TalentCache.GetDefaultCountryForBU()
            If defaultCountry <> String.Empty Then
                country.SelectedValue = defaultCountry
            End If
        End If

    End Sub

    Protected Sub PopulateCompanyDetailsDropDownLists()
        Dim companyDetails1Key, companyDetails2Key As String
        companyDetails1Key = ucr.Content("CompanyDetails1Key", _languageCode, True)
        companyDetails2Key = ucr.Content("CompanyDetails2Key", _languageCode, True)
        If companyDetails1Key <> String.Empty Then

            ddlCompanyDetails1.Items.Clear()
            Dim lic As ListItemCollection = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, companyDetails1Key, "DESCRIPTION", companyDetails1Key)
            Dim count As Integer = 0
            For Each li As ListItem In lic
                'Don't add the first item as it is a dummy item
                If count > 0 Then
                    ddlCompanyDetails1.Items.Add(li)
                End If
                count += 1
            Next
            If ddlCompanyDetails1.Items.Count = 1 Then
                ddlCompanyDetails1.SelectedIndex = 0
                ddlCompanyDetails1.Enabled = False
            ElseIf ddlCompanyDetails1.Items.Count = 0 Then
                'ddlCompanyDetails1.Items.Add(ucr.Content("NoDefaultPaymentMethod", _languageCode, True))
            End If
        Else
            lblCompanyDetails1.Visible = False
            ddlCompanyDetails1.Visible = False
        End If


        If companyDetails2Key <> String.Empty Then

            ddlCompanyDetails2.Items.Clear()
            Dim lic2 As ListItemCollection = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, companyDetails2Key, "DESCRIPTION", companyDetails2Key)
            Dim count2 As Integer = 0
            For Each li As ListItem In lic2
                'Don't add the first item as it is a dummy item
                If count2 > 0 Then
                    ddlCompanyDetails2.Items.Add(li)
                End If
                count2 += 1
            Next
            If ddlCompanyDetails2.Items.Count = 1 Then
                ddlCompanyDetails2.SelectedIndex = 0
                ddlCompanyDetails2.Enabled = False
            ElseIf ddlCompanyDetails2.Items.Count = 0 Then
                'ddlCompanyDetail21.Items.Add(ucr.Content("NoDefaultPaymentMethod", _languageCode, True))
            End If
        Else
            lblCompanyDetails2.Visible = False
            ddlCompanyDetails2.Visible = False
        End If


    End Sub

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
                If myCountry.COUNTRY_CODE = country.SelectedValue Then
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

    Protected Sub registerBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles registerBtn.Click
        If Me.Display Then
            If TandCs.Checked = True Then

                If Not CheckPostcode() Then
                    ErrorLabel.Text = ucr.Content("PostcodeRequiredFieldValidator", _languageCode, True)
                    Exit Sub
                End If

                'Check to see if a registration session is available
                If String.IsNullOrEmpty(Session("Registration")) Then
                    'Set Registration Session
                    Session("Registration") = forename.Text & "," & surname.Text & "," & email.Text & "," & DateTime.Now
                Else
                    'Session is available, check that it was created within the last 2 minutes and matches the form values
                    'if so, dont submit the form data to the database, but send the user the confirmation screen.
                    Dim registrationSession As Array = Split(Session("Registration").ToString, ",")
                    If registrationSession(0) = forename.Text AndAlso _
                        registrationSession(1) = surname.Text AndAlso _
                        registrationSession(2) = email.Text Then
                        Try
                            Dim SessionDate As DateTime = CType(registrationSession(3), DateTime)
                            Dim SessionDate2 As DateTime = DateTime.Now
                            SessionDate2 = SessionDate2.AddMinutes(-2)
                            'Check that it hasnt been created within the last 2 mins
                            If SessionDate <= DateTime.Now AndAlso SessionDate2 <= SessionDate Then
                                Response.Redirect("~/PagesLogin/Profile/RegistrationConfirmation.aspx")
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                End If

                Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
                Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

                Dim defaultPriceList As String = def.PriceList

                If Membership.GetUser(email.Text, False) Is Nothing Then
                    ErrorLabel.Text = String.Empty
                    Try

                        Dim userNumber As String = Talent.Common.Utilities.GetNextUserNumber(TalentCache.GetBusinessUnit, _
                            TalentCache.GetPartner(Profile), _
                            ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)

                        Dim generatedAccountNo1 As String = String.Empty
                        Dim generatedAccountNo2 As String = String.Empty
                        ' Check if need to generate SL account no..
                        If def.AutoCreateAccountNo Then

                            generatedAccountNo1 = Talent.Common.Utilities.GenerateAccountNumber(TalentCache.GetBusinessUnit, _
                                                           TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                           ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString, _
                                                           def.Account1Prefix, _
                                                           def.Account1Length, _
                                                           "ACCOUNT_1_NEXT_NUMBER")
                            generatedAccountNo2 = Talent.Common.Utilities.GenerateAccountNumber(TalentCache.GetBusinessUnit, _
                                                                                   TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                                   ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString, _
                                                                                   def.Account2Prefix, _
                                                                                   def.Account2Length, _
                                                                                   "ACCOUNT_2_NEXT_NUMBER")
                        End If

                        ' Write User_Number instead of email address
                        Select Case def.LoginidType
                            Case Is = "2"
                                If def.AutoCreateAccountNo Then
                                    CType(Membership.Provider, TalentMembershipProvider).CreateUser(userNumber, password1.Text, email.Text, "", "", True, New Object, New System.Web.Security.MembershipCreateStatus, generatedAccountNo1)
                                Else
                                    CType(Membership.Provider, TalentMembershipProvider).CreateUser(userNumber, password1.Text, email.Text, "", "", True, New Object, New System.Web.Security.MembershipCreateStatus, accountNumber.Text)
                                End If

                            Case Else
                                If def.AutoCreateAccountNo Then
                                    CType(Membership.Provider, TalentMembershipProvider).CreateUser(loginid.Text, password1.Text, email.Text, "", "", True, New Object, New System.Web.Security.MembershipCreateStatus, generatedAccountNo2)
                                Else
                                    CType(Membership.Provider, TalentMembershipProvider).CreateUser(loginid.Text, password1.Text, email.Text, "", "", True, New Object, New System.Web.Security.MembershipCreateStatus, accountNumber.Text)
                                End If
                        End Select

                        'Create and populate the user object
                        Dim userDetails As New TalentProfileUserDetails
                        With userDetails
                            .LoginID = email.Text
                            .Email = email.Text
                            .Title = title.SelectedValue
                            .Forename = forename.Text
                            .DOB = CDate("01/02/1900")
                            .Surname = surname.Text
                            .Full_Name = forename.Text & " " & surname.Text
                            .Position = position.Text
                            .Telephone_Number = phone.Text
                            .Fax_Number = fax.Text
                            .Account_No_1 = accountNumber.Text
                            '.User_Number = Talent.Common.Utilities.GetNextUserNumber(TalentCache.GetBusinessUnit, _
                            '                        TalentCache.GetPartner(Profile), _
                            '                        ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
                            .User_Number = userNumber
                            .User_Number_Prefix = def.UserNumberPrefix
                            .Subscribe_Newsletter = Newsletter.Checked
                            .HTML_Newsletter = GetNewsletterType()

                            If opt1.Checked = opt1TrueValue Then
                                .Bit1 = "1"
                            Else
                                .Bit1 = "0"
                            End If
                            If opt2.Checked = opt2TrueValue Then
                                .Bit2 = "1"
                            Else
                                .Bit2 = "0"
                            End If
                            If opt3.Checked = opt3TrueValue Then
                                .Bit3 = "1"
                            Else
                                .Bit3 = "0"
                            End If
                            If opt4.Checked = opt4TrueValue Then
                                .Bit4 = "1"
                            Else
                                .Bit4 = "0"
                            End If
                            If opt5.Checked = opt5TrueValue Then
                                .Bit5 = "1"
                            Else
                                .Bit5 = "0"
                            End If

                        End With


                        'Create and populate the userAddress
                        Dim userAddress As New TalentProfileAddress
                        With userAddress
                            .LoginID = email.Text
                            Dim eComDefs As New ECommerceModuleDefaults
                            Dim defs As ECommerceModuleDefaults.DefaultValues = eComDefs.GetDefaults

                            .Reference = Address1.Text & " " & Address2.Text
                            .Type = String.Empty
                            .Default_Address = True
                            '.Address_Line_1 = building.Text
                            '.Address_Line_2 = street.Text
                            '.Address_Line_3 = town.Text
                            '.Address_Line_4 = city.Text
                            '.Address_Line_5 = county.Text
                            .Address_Line_1 = Address1.Text
                            .Address_Line_2 = Address2.Text
                            .Address_Line_3 = Address3.Text
                            .Address_Line_4 = Address4.Text
                            .Address_Line_5 = Address5.Text

                            .Post_Code = postcode.Text
                            If def.StoreCountryAsWholeName Then
                                .Country = UCase(country.SelectedItem.Text)
                            Else
                                .Country = country.SelectedValue
                            End If
                            .Sequence = 0
                        End With

                        'Create and populate the partnerDetails
                        Dim partnerDetails As New TalentProfilePartnerDetails
                        With partnerDetails
                            If def.AutoCreateAccountNo Then
                                .Account_No_1 = generatedAccountNo1
                                .Account_No_2 = generatedAccountNo2
                            Else
                                .Account_No_1 = accountNumber.Text
                                .Account_No_2 = accountNumber2.Text
                                .Account_No_3 = accountNumber3.Text
                                .Account_No_4 = accountNumber4.Text
                                .Account_No_5 = accountNumber5.Text
                            End If

                            If def.AutoCreateAccountNo Then
                                .Partner = generatedAccountNo1
                            Else
                                .Partner = accountNumber.Text
                            End If

                            .Telephone_Number = phone.Text
                            .Fax_Number = fax.Text
                            .Partner_Number = Talent.Common.Utilities.GetNextPartnerNumber(TalentCache.GetBusinessUnit, _
                                Address1.Text, _
                                ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
                            .VAT_NUMBER = vatNumber.Text
                        End With

                        ' Write User_Number instead of email address
                        Select Case def.LoginidType
                            Case Is = "2"
                                loginid.Text = userDetails.User_Number.Trim
                                userAddress.LoginID = userDetails.User_Number.Trim
                                userDetails.LoginID = userDetails.User_Number.Trim
                                '     userDetails.Account_No_1 = userDetails.User_Number.Trim
                                userDetails.Account_No_1 = generatedAccountNo1
                                userDetails.Account_No_2 = generatedAccountNo2
                        End Select

                        ' Check price list generation type
                        Dim priceList As String = String.Empty
                        Select Case def.RegistrationPricelistSelectionType
                            Case Is = "1"
                                priceList = Talent.eCommerce.Utilities.GetPriceListType1(country.SelectedValue, vatNumber.Text)
                        End Select

                        ' If price list isn't the default one then create a parter specific default record
                        ' (if one doesn't already exist)
                        If priceList <> String.Empty AndAlso priceList <> defaultPriceList Then
                            Dim ecommerceDefaults As New TalentApplicationVariablesTableAdapters.tbl_ecommerce_module_defaults_buTableAdapter
                            Dim dt As Data.DataTable = ecommerceDefaults.GetDataByBuPartnerDefaultName(TalentCache.GetBusinessUnit, partnerDetails.Partner, "PRICE_LIST")
                            If dt.Rows.Count = 0 Then
                                ecommerceDefaults.InsertQuery(TalentCache.GetBusinessUnit, partnerDetails.Partner, String.Empty, String.Empty, "PRICE_LIST", priceList)
                            End If

                        End If

                        ' Determine CRM branch according to rules
                        Dim crmBranch As String = String.Empty
                        Dim restrictedPaymentTypes As String = String.Empty
                        Select Case def.RegistrationBranchSelectionType
                            Case Is = "1"
                                crmBranch = Talent.eCommerce.Utilities.GetBranchType1(country.SelectedValue, Address1.Text, vatNumber.Text)
                                ' If crm branch is restricted (branch 3) then apply restrictions
                                Dim restrictedPaymentBranch As String = String.Empty
                                Dim countryTable As New TalentApplicationVariablesTableAdapters.tbl_countryTableAdapter
                                Dim dtCountry As New Data.DataTable
                                dtCountry = countryTable.GetDataByCountryCode(country.SelectedValue)
                                If dtCountry.Rows.Count > 0 Then
                                    restrictedPaymentBranch = dtCountry.Rows(0)("REGISTRATION_BRANCH_3")
                                    'If crmBranch <> String.Empty AndAlso crmBranch = restrictedPaymentBranch Then
                                    'restrictedPaymentTypes = dtCountry.Rows(0)("RESTRICTED_PAYMENT_METHOD")
                                    'End If
                                    restrictedPaymentTypes = "INV"
                                End If
                        End Select

                        partnerDetails.CRM_Branch = crmBranch

                        userDetails.RESTRICTED_PAYMENT_METHOD = restrictedPaymentTypes

                        'Create the profile
                        Profile.Provider.CreateProfileWithPartner(userDetails, userAddress, partnerDetails)

                        FormsAuthentication.Authenticate(loginid.Text, password1.Text)
                        Profile.Initialize(email.Text, True)
                        FormsAuthentication.SetAuthCookie(loginid.Text, False)

                        ' Remove any items out of the basket if a price isn't found with the 
                        ' new price list
                        Dim removeItems As New ArrayList

                        If priceList <> String.Empty AndAlso priceList <> defaultPriceList Then
                            If Profile.Basket.BasketItems.Count > 0 Then
                                For Each basketItem As TalentBasketItem In Profile.Basket.BasketItems
                                    Dim wp As New DEWebPrice
                                    wp = Talent.eCommerce.Utilities.GetWebPrices(basketItem.Product.Trim, basketItem.Quantity, priceList)
                                    If Not wp.PriceFound Then
                                        removeItems.Add(basketItem.Product.Trim)
                                        ' Profile.Basket.BasketItems.Remove(basketItem)
                                    End If
                                Next
                            End If
                            Dim basketAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
                            For Each itemToRemove As String In removeItems
                                basketAdapter.Delete_Basket_Item(Profile.Basket.Basket_Header_ID, itemToRemove)
                                ' Profile.Basket.BasketItems.Remove(itemToRemove)
                            Next
                        End If

                        'Save details to CRM
                        If CType(def.SendRegistrationToBackEnd, Boolean) Then
                            SendDetailsToBackend("REGISTRATION", userDetails, userAddress, crmBranch)
                        End If

                        SendConfirmationEmail()

                    Catch ex As Exception
                        '--------------------------------------------------------------------------
                        ' Log any errors which may cause partial registrations (happened at Adnams)
                        '--------------------------------------------------------------------------
                        Logging.WriteLog(Profile.UserName, "REG2-00020", "Error Creating customer:" & ex.Message, "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                        Exit Sub
                    End Try
                    'End Using

                    If Request.QueryString("ReturnUrl") Is Nothing Then
                        Response.Redirect("~/PagesPublic/Profile/registrationConfirmation.aspx")
                    Else
                        Response.Redirect(Request.QueryString("ReturnUrl"))
                    End If

                Else
                    ErrorLabel.Text = ucr.Content("UserAlreadyExistsErrorText", _languageCode, True)
                End If
            Else
                ErrorLabel.Text = ucr.Content("AcceptTandCsErrorText", _languageCode, True)
            End If
        End If

    End Sub

    Protected Sub SendConfirmationEmail()
        If Me.Display Then
            Dim defaults As ECommerceModuleDefaults.DefaultValues
            Dim defs As New ECommerceModuleDefaults
            defaults = defs.GetDefaults

            Dim body As String = ucr.Content("ConfirmationEmailBody", _languageCode, True)
            body = body.Replace("<<To>>", forename.Text)
            body = body.Replace("<<NewLine>>", vbCrLf)
            If defaults.LoginidType.Equals("1") Then
                body = body.Replace("<<UserName>>", loginid.Text.TrimStart("0"))
            Else
                body = body.Replace("<<UserName>>", loginid.Text)
            End If
            body = body.Replace("<<Password>>", password1.Text)
            Talent.Common.Utilities.SMTP = ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim
            Talent.Common.Utilities.SMTPPortNumber = Talent.eCommerce.Utilities.GetSMTPPortNumber
            body = body.Replace("<<WebSiteAddress>>", Talent.eCommerce.Utilities.GetCurrentApplicationUrl & "/PagesPublic/Home/Home.aspx")

            Dim err As Talent.Common.ErrorObj = Talent.Common.Utilities.Email_Send(defaults.RegistrationConfirmationFromEmail, email.Text, _
                                                ucr.Content("ConfirmationEmailSubject", _languageCode, True), _
                                                body)
        End If

    End Sub

    Protected Sub SetAddressVisibility()
        If Me.Display Then
            If Not addressLine1RowVisible Then
                AddressLine1Row.Visible = False
                rfvAddress1.Enabled = False
                regExAddress1.Enabled = False
            End If
            If Not addressLine2RowVisible Then
                AddressLine2Row.Visible = False
                rfvAddress2.Enabled = False
                regExAddress2.Enabled = False
            End If
            If Not addressLine3RowVisible Then
                AddressLine3Row.Visible = False
                ' rfvAddress3.Enabled = False
                regExAddress3.Enabled = False
            End If
            If Not addressLine4RowVisible Then
                AddressLine4Row.Visible = False
                ' rfvAddress4.Enabled = False
                regExAddress4.Enabled = False
            End If
            If Not addressLine5RowVisible Then
                AddressLine5Row.Visible = False
                rfvAddress5.Enabled = False
                regExAddress5.Enabled = False
            End If
            If Not addressPostcodeRowVisible Then
                AddressPostcodeRow.Visible = False
                postcodeRFV.Enabled = False
                postcodeRegEx.Enabled = False
            End If
            If Not addressCountryRowVisible Then
                AddressCountryRow.Visible = False
                countryRegEx.Enabled = False
            End If
            If Not ucr.Attribute("addressTitleRowVisible").Trim = "" Then
                If Not CType(ucr.Attribute("addressTitleRowVisible"), Boolean) Then
                    AddressTitleRow.Visible = False
                End If
            End If
            If Not ucr.Attribute("addressingOnOff").Trim = "" Then
                If Not CType(ucr.Attribute("addressingOnOff"), Boolean) Then
                    FindAddressButtonRow.Visible = False
                End If
            End If
        End If



    End Sub
    Protected Sub SetAddressVisibilityProperties()
        If Me.Display Then
            ' 
            ' Set common address field visibility using system defaults, and then override by any control-level defaults.
            If Not myDefs.AddressLine1RowVisible Then
                addressLine1RowVisible = False
            End If
            If Not myDefs.AddressLine2RowVisible Then
                addressLine2RowVisible = False
            End If
            If Not myDefs.AddressLine3RowVisible Then
                addressLine3RowVisible = False
            End If
            If Not myDefs.AddressLine4RowVisible Then
                addressLine4RowVisible = False
            End If
            If Not myDefs.AddressLine5RowVisible Then
                addressLine5RowVisible = False
            End If
            If Not myDefs.AddressPostcodeRowVisible Then
                addressPostcodeRowVisible = False
            End If
            If Not myDefs.AddressCountryRowVisible Then
                addressCountryRowVisible = False
            End If

            '
            ' Control-level overrides for visibility (these DO NOT need to exist on tbl_control_attributes)
            If Not ucr.Attribute("addressLine1RowVisible").Trim = "" Then
                If Not CType(ucr.Attribute("addressLine1RowVisible"), Boolean) Then
                    addressLine1RowVisible = False
                Else
                    addressLine1RowVisible = True
                End If
            End If
            If Not ucr.Attribute("addressLine2RowVisible").Trim = "" Then
                If Not CType(ucr.Attribute("addressLine2RowVisible"), Boolean) Then
                    addressLine2RowVisible = False
                Else
                    addressLine2RowVisible = True
                End If
            End If
            If Not ucr.Attribute("addressLine3RowVisible").Trim = "" Then
                If Not CType(ucr.Attribute("addressLine3RowVisible"), Boolean) Then
                    addressLine3RowVisible = False
                Else
                    addressLine3RowVisible = True
                End If
            End If
            If Not ucr.Attribute("addressLine4RowVisible").Trim = "" Then
                If Not CType(ucr.Attribute("addressLine4RowVisible"), Boolean) Then
                    addressLine4RowVisible = False
                Else
                    addressLine4RowVisible = True
                End If
            End If
            If Not ucr.Attribute("addressLine5RowVisible").Trim = "" Then
                If Not CType(ucr.Attribute("addressLine5RowVisible"), Boolean) Then
                    addressLine5RowVisible = False
                Else
                    addressLine5RowVisible = True
                End If
            End If
            If Not ucr.Attribute("addressPostcodeRowVisible").Trim = "" Then
                If Not CType(ucr.Attribute("addressPostcodeRowVisible"), Boolean) Then
                    addressPostcodeRowVisible = False
                Else
                    addressPostcodeRowVisible = True
                End If
            End If
            If Not ucr.Attribute("addresscountryRowVisible").Trim = "" Then
                If Not CType(ucr.Attribute("addressCountryRowVisible"), Boolean) Then
                    addressCountryRowVisible = False
                Else
                    addressCountryRowVisible = True
                End If
            End If

            '
            ' Now the control-specific fields (these DO need to exist on tbl_control_attributes)
            If Not CType(ucr.Attribute("addressCompanyNameRowVisible"), Boolean) Then
                addressCompanyNameRowVisible = False
            End If
            If Not CType(ucr.Attribute("addressOrganisationRowVisible"), Boolean) Then
                addressOrganisationRowVisible = False
            End If
            If Not CType(ucr.Attribute("addressingOnOff"), Boolean) Then
                FindAddressButtonRow.Visible = False
            End If
            If Not CType(ucr.Attribute("addressTitleRowVisible"), Boolean) Then
                AddressTitleRow.Visible = False
            End If
        End If



    End Sub

    Protected Function GetAddressingLinkText() As String
        If Me.Display Then
            Return ucr.Content("addressingLinkButtonText", _languageCode, True)
        Else
            Return ""
        End If

    End Function
    Protected Sub CreateAddressingJavascript()
        If Me.Display Then
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
                        Response.Write("win1 = window.open('../../PagesPublic/QAS/FlatCountry.aspx?" & _
                                           com.qas.prowebintegration.Constants.FIELD_COUNTRY_NAME & _
                                           "=' + document.forms[0].ctl00$ContentPlaceHolder1$registrationForm2$country.options[document.forms[0].ctl00$ContentPlaceHolder1$registrationForm2$country.selectedIndex].text + '&postCode=' + document.getElementById('ctl00_ContentPlaceHolder1_registrationForm2_postcode').value, 'QAS', '" & ucr.Attribute("addressingWindowAttributes") & "');" & vbCrLf)

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

                '
                ' Clear all address fields
                '       If addressCompanyNameRowVisible Then Response.Write("document.forms[0]." & companyName.UniqueID & ".value = '';" & vbCrLf)
                '       If addressOrganisationRowVisible Then Response.Write("document.forms[0]." & organisation.UniqueID & ".value = '';" & vbCrLf)

                'If addressLine1RowVisible Then Response.Write("document.forms[0]." & building.UniqueID & ".value = '';" & vbCrLf)
                'If addressLine2RowVisible Then Response.Write("document.forms[0]." & street.UniqueID & ".value = '';" & vbCrLf)
                'If addressLine3RowVisible Then Response.Write("document.forms[0]." & town.UniqueID & ".value = '';" & vbCrLf)
                'If addressLine4RowVisible Then Response.Write("document.forms[0]." & city.UniqueID & ".value = '';" & vbCrLf)
                'If addressLine5RowVisible Then Response.Write("document.forms[0]." & county.UniqueID & ".value = '';" & vbCrLf)

                If addressLine1RowVisible Then Response.Write("document.forms[0]." & Address1.UniqueID & ".value = '';" & vbCrLf)
                If addressLine2RowVisible Then Response.Write("document.forms[0]." & Address2.UniqueID & ".value = '';" & vbCrLf)
                If addressLine3RowVisible Then Response.Write("document.forms[0]." & Address3.UniqueID & ".value = '';" & vbCrLf)
                If addressLine4RowVisible Then Response.Write("document.forms[0]." & Address4.UniqueID & ".value = '';" & vbCrLf)
                If addressLine5RowVisible Then Response.Write("document.forms[0]." & Address5.UniqueID & ".value = '';" & vbCrLf)
                If addressPostcodeRowVisible Then Response.Write("document.forms[0]." & postcode.UniqueID & ".value = '';" & vbCrLf)
                If addressCountryRowVisible Then Response.Write("document.forms[0]." & country.UniqueID & ".value = '';" & vbCrLf)

                '
                ' If an address field is in use and is defined to contain a QAS address element then create Javascript code to populate correctly.
                'If addressCompanyNameRowVisible And Not defaults.AddressingMapCompanyName.Trim = "" Then
                '    sString = GetJavascriptString("document.forms[0]." & companyName.UniqueID & ".value", defaults.AddressingMapCompanyName, defaults.AddressingFields)
                '    Response.Write(sString)
                'End If
                'If addressOrganisationRowVisible And Not defaults.AddressingMapOrganisation.Trim = "" Then
                '    sString = GetJavascriptString("document.forms[0]." & organisation.UniqueID & ".value", defaults.AddressingMapOrganisation, defaults.AddressingFields)
                '    Response.Write(sString)
                'End If

                If addressLine1RowVisible And Not defaults.AddressingMapAdr1.Trim = "" Then
                    sString = GetJavascriptString("document.forms[0]." & Address1.UniqueID & ".value", defaults.AddressingMapAdr1, defaults.AddressingFields)
                    Response.Write(sString)
                End If
                If addressLine2RowVisible And Not defaults.AddressingMapAdr2.Trim = "" Then
                    sString = GetJavascriptString("document.forms[0]." & Address2.UniqueID & ".value", defaults.AddressingMapAdr2, defaults.AddressingFields)
                    Response.Write(sString)
                End If
                If addressLine3RowVisible And Not defaults.AddressingMapAdr3.Trim = "" Then
                    sString = GetJavascriptString("document.forms[0]." & Address3.UniqueID & ".value", defaults.AddressingMapAdr3, defaults.AddressingFields)
                    Response.Write(sString)
                End If
                If addressLine4RowVisible And Not defaults.AddressingMapAdr4.Trim = "" Then
                    sString = GetJavascriptString("document.forms[0]." & Address4.UniqueID & ".value", defaults.AddressingMapAdr4, defaults.AddressingFields)
                    Response.Write(sString)
                End If
                If addressLine5RowVisible And Not defaults.AddressingMapAdr5.Trim = "" Then
                    sString = GetJavascriptString("document.forms[0]." & Address5.UniqueID & ".value", defaults.AddressingMapAdr5, defaults.AddressingFields)
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
        End If


    End Sub
    Protected Function GetJavascriptString(ByVal sFieldString, ByVal sAddressingMap, ByVal sAddressingFields) As String
        If Me.Display Then
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
        Else
            Return ""
        End If


    End Function
    Protected Sub CreateAddressingHiddenFields()
        If Me.Display Then
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
        End If


    End Sub

    Protected Sub SendDetailsToBackend(ByVal call_origin As String, _
                                   ByVal userDetails As TalentProfileUserDetails, _
                                   ByVal userAddress As TalentProfileAddress, _
                                   ByVal crmBranch As String)

        Dim myCustomer As New TalentCustomer
        Dim myErrorObj As New ErrorObj
        Dim deCustV11 As New DECustomerV11
        Dim deCustV1 As New DECustomer
        deCustV11.DECustomersV1.Add(deCustV1)

        Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        Dim p As New TalentProfileUser
        Dim address As New TalentProfileAddress

        'Set the profile information
        ' BF - this doesn't work. Have r-fitted the line from v2.028 (SJ version)
        '    If CType(def.SendRegistrationToBackendFirst, Boolean) Then
        If Not userDetails Is Nothing AndAlso Not userAddress Is Nothing Then
            p.Details = userDetails
            If opt1.Checked = opt1TrueValue Then
                p.Details.Bit1 = "1"
            Else
                p.Details.Bit1 = "0"
            End If
            If opt2.Checked = opt2TrueValue Then
                p.Details.Bit2 = "1"
            Else
                p.Details.Bit2 = "0"
            End If
            If opt3.Checked = opt3TrueValue Then
                p.Details.Bit3 = "1"
            Else
                p.Details.Bit3 = "0"
            End If
            If opt4.Checked = opt4TrueValue Then
                p.Details.Bit4 = "1"
            Else
                p.Details.Bit4 = "0"
            End If
            If opt5.Checked = opt5TrueValue Then
                p.Details.Bit5 = "1"
            Else
                p.Details.Bit5 = "0"
            End If
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
                .PositionInCompany = p.Details.Position
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
                .ProcessPositionInCompany = "1"
                '.ProcessSLNumber1 = "1"
                '.ProcessSLNumber2 = "1"

                If opt1.Checked = opt1TrueValue Then
                    .ContactViaMail1 = "1"
                Else
                    .ContactViaMail1 = "0"
                End If
                If opt2.Checked = opt2TrueValue Then
                    .ContactViaMail2 = "1"
                Else
                    .ContactViaMail2 = "0"
                End If
                If opt3.Checked = opt3TrueValue Then
                    .ContactViaMail3 = "1"
                Else
                    .ContactViaMail3 = "0"
                End If
                If opt4.Checked = opt4TrueValue Then
                    .ContactViaMail4 = "1"
                Else
                    .ContactViaMail4 = "0"
                End If
                If opt5.Checked = opt5TrueValue Then
                    .ContactViaMail5 = "1"
                Else
                    .ContactViaMail5 = "0"
                End If


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
                    .BranchCode = crmBranch
                End If
                If .BranchCode = String.Empty Then
                    .BranchCode = def.CrmBranch
                End If

                ' If QAS is used then Address Line is never populated and therefore Address1 for CRM 
                ' needs to be populated from different source fields.
                Dim eComDefs As New ECommerceModuleDefaults
                Dim defs As ECommerceModuleDefaults.DefaultValues = eComDefs.GetDefaults


                .AddressLine1 = address.Address_Line_2.Trim
                .AddressLine2 = address.Address_Line_3.Trim
                .AddressLine3 = address.Address_Line_4.Trim
                .AddressLine4 = address.Address_Line_5.Trim
                .AddressLine5 = UCase(address.Country.Trim)

                .PostCode = UCase(address.Post_Code)
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
                .VatCode = vatNumber.Text

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

                .ProcessAttributes = "1"
                ' Process Newsletter Prefs attributes
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
                ' /001 Process Newsletter Prefs attributes
                'If Newsletter.Checked Then
                '    .Attribute01 = ucr.Attribute("newsletterAttribute")
                '    .Attribute01Action = "A"
                '    If NewsletterChoice.SelectedValue = ucr.Content("HTMLNewsletterText", _languageCode, True) Then
                '        .Attribute02 = ucr.Attribute("newsletterAttributeTypeHTML")
                '        .Attribute02Action = "A"
                '        .Attribute03 = ucr.Attribute("newsletterAttributeTypePlain")
                '        .Attribute03Action = "D"
                '    ElseIf NewsletterChoice.SelectedValue = ucr.Content("PlainTextNewsletterText", _languageCode, True) Then
                '        .Attribute02 = ucr.Attribute("newsletterAttributeTypePlain")
                '        .Attribute02Action = "A"
                '        .Attribute03 = ucr.Attribute("newsletterAttributeTypeHTML")
                '        .Attribute03Action = "D"
                '    End If
                'Else
                '    .Attribute01 = ucr.Attribute("newsletterAttribute")
                '    .Attribute01Action = "D"
                '    .Attribute02 = ucr.Attribute("newsletterAttributeTypeHTML")
                '    .Attribute02Action = "D"
                '    .Attribute03 = ucr.Attribute("newsletterAttributeTypePlain")
                '    .Attribute03Action = "D"

                'End If
                '--------------------------------
                ' Send company details attributes
                '--------------------------------
                If ddlCompanyDetails1.Visible AndAlso Not ddlCompanyDetails1.SelectedItem Is Nothing Then
                    .Attribute04 = ddlCompanyDetails1.SelectedItem.Value
                    .Attribute04Action = "A"
                End If
                If ddlCompanyDetails2.Visible Then
                    If ddlCompanyDetails1.Visible AndAlso Not ddlCompanyDetails1.SelectedItem Is Nothing Then
                        .Attribute05 = ddlCompanyDetails2.SelectedItem.Value
                        .Attribute05Action = "A"
                    Else
                        If Not ddlCompanyDetails2.SelectedItem Is Nothing Then
                            .Attribute04 = ddlCompanyDetails2.SelectedItem.Value
                            .Attribute04Action = "A"
                        End If
                    End If
                End If
            End With

            ' Set the Customer Specific Settings
            Dim decs As New DECustomerSettings()
            decs = CType(.Settings, DECustomerSettings)
            decs.CreationType = "REGISTRATION"
            .Settings = CType(decs, DESettings)

            With .Settings
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .BusinessUnit = TalentCache.GetBusinessUnit
                .Company = ucr.Content("crmExternalKeyName", _languageCode, True)
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
                Logging.WriteLog(Profile.UserName, myErrorObj.ErrorNumber, myErrorObj.ErrorMessage, myErrorObj.ErrorStatus, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                Logging.WriteLog(Profile.UserName, "REG2-00010", "Error Sending Customer to Back-End, Serializing...", "", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                Talent.eCommerce.Utilities.SerializeObject(myCustomer, _
                                                            myCustomer.GetType, _
                                                            TalentCache.GetBusinessUnit, _
                                                            TalentCache.GetPartner(Profile), _
                                                            Profile.UserName, _
                                                            call_origin)
            End If

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
                    Else
                        'Set the returned customer number
                        ' _customerNumber = myCustomer.De.CustomerNumber.Trim
                    End If
                End If
            End If
        End With
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
End Class
