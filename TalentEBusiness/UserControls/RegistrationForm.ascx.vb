Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Data
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TalentBusinessLogic.ModelBuilders.CRM
Imports TalentBusinessLogic.Models

Partial Class UserControls_RegistrationForm
    Inherits ControlBase

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private ucr As New Talent.Common.UserControlResource
    Private _customerNumber As String = ""
    Private addressLine1RowVisible As Boolean = True
    Private addressLine2RowVisible As Boolean = True
    Private addressLine3RowVisible As Boolean = True
    Private addressLine4RowVisible As Boolean = True
    Private addressLine5RowVisible As Boolean = True
    Private addressPostcodeRowVisible As Boolean = True
    Private addressCountryRowVisible As Boolean = True
    Private addressTypeRowVisible As Boolean = True
    Private addressReferenceRowVisible As Boolean = True
    Private addressSequenceRowVisible As Boolean = True
    Private mothersnameRowVisible As Boolean = True
    Private fathersnameRowVisible As Boolean = True
    Private mailOptionTrueValue As Boolean = True
    Private opt1TrueValue As Boolean = True
    Private opt2TrueValue As Boolean = True
    Private opt3TrueValue As Boolean = True
    Private opt4TrueValue As Boolean = True
    Private opt5TrueValue As Boolean = True
    Private errMsg As Talent.Common.TalentErrorMessages
    Private _crmBranch As String = String.Empty
    Private _ParentalConsentCeilingDob As Date = TalentDefaults.ParentalConsentCeilingDoB

    'Need to default to true because the other controls that use this one may not set the display property
    Public Display As Boolean = True

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Display Then


            'Check to see if the registration via search is being used
            If Request.QueryString("source") = "search" Then
                Dim foundCustomerNumber As Boolean = False
                Try
                    Dim dtBasketCustomerSearch As New DataTable
                    If Session("BasketCustomerSearch") Is Nothing Then
                        Session("BasketCustomerSearch") = CreateBasketCustomerSearchDataTable()
                    End If
                    dtBasketCustomerSearch = Session("BasketCustomerSearch")
                    If dtBasketCustomerSearch.Rows.Count > 0 Then
                        Dim i As Integer
                        For i = 0 To dtBasketCustomerSearch.Rows.Count - 1
                            If dtBasketCustomerSearch.Rows(i)("CustomerNumber").ToString = "999999999999" Then
                                foundCustomerNumber = True
                            End If
                        Next
                    End If
                Catch ex As Exception
                End Try
                If Not foundCustomerNumber Then Response.Redirect("~/PagesPublic/Error/error.aspx")
            End If

            SetEnterButtonClickEvent()
            plhRegisteredAddress.Visible = False
            SetUpUCR()
            With ucr
                Try
                    If Talent.eCommerce.Utilities.IsAgent Then
                        ' If in agent mode then check if need to display Registered Address Panel
                        plhRegisteredAddress.Visible = CBool(.Attribute("DisplayRegisteredPanel"))
                        plhTandCs.Visible = False
                        plhEmailCompare.Visible = False
                    End If
                Catch
                End Try

            End With
        Else
            Me.Visible = False
        End If


    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Me.Display Then
            SetUpUCR()
            If Not Page.IsPostBack Then

                Session("AddressFormat") = Nothing

                SetUpOpt()
                SetUpRegistrationTypeBox()
                PopulateTitleDropDownList()
                PopulateDOBDropDownLists()
                PopulateSexDropDownLists()
                PopulateCountriesDropDownList()
                PopulatePriceBandsDropDownList()
                PopulateStopcodesDropDownList()
                SetDetailsVisibility()
                SetCredentialsVisibility()
                SetAddressVisibilityProperties()
                SetAddressVisibility()
                SetupUserIDsGroup()
                SetLabelText()
                SetControlForRegistration()
                SetAttributes()


                Setup_Supporter_Favourites_And_Club_Contact_Options()

                If Not CType(ucr.Attribute("dobEnableRFV"), Boolean) Then
                    dobDayRegEx.Enabled = False
                    dobMonthRegEx.Enabled = False
                    dobYearRegEx.Enabled = False
                End If


                If Not CType(ucr.Attribute("txtCompanyNameEnableRFV"), Boolean) Then
                    companyNameRFV.Enabled = False
                End If


                ddlRegistrationType_SelectedIndexChanged(Nothing, Nothing)
                'Me.Page.ClientScript.RegisterForEventValidation(supporterSelectDDL1.ID)
                'Me.Page.ClientScript.RegisterForEventValidation(supporterSelectDDL2.ID)
                'Me.Page.ClientScript.RegisterForEventValidation(supporterSelectDDL3.ID)
                If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("IsConfirmationEmailSend")) Then
                    cbIsConfirmationEmailSend.Checked = True
                Else
                    cbIsConfirmationEmailSend.Checked = False
                End If
                If plhIsConfirmationEmailSend.Visible Then
                    isConfirmationEmailSend.Visible = True
                Else
                    isConfirmationEmailSend.Visible = False
                End If
            Else
                ' on postback still need to set Registered Panel enabled or not 
                ' due to validators
                ' chkRegAddressSame_CheckedChanged(chkRegAddressSame, New System.EventArgs)

                ' on postback still need to set address as format might have changed (galatasaray mods to show AddressFormat1.ascx
                SetAddressVisibilityProperties()
                SetAddressVisibility()
            End If

            UseAddresslessRegistration()
            ErrorLabel.Visible = (ErrorLabel.Text.Trim().Length > 0)

        End If



    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        MyBase.Render(writer)
        Dim dt1 As DataTable = Talent.eCommerce.Utilities.GetSupporterSportDDL
        Dim dt2 As DataTable = Talent.eCommerce.Utilities.GetSupporterSportTeamDDL
        Dim dt3 As DataTable = Talent.eCommerce.Utilities.GetSupporterSportTeamClubDDL

        If Not dt1 Is Nothing Then
            For Each dr As DataRow In dt1.Rows
                Me.Page.ClientScript.RegisterForEventValidation(supporterSelectDDL1.ID, Talent.eCommerce.Utilities.CheckForDBNull_String(dr("SPORT_CODE")))
            Next
        End If

        If Not dt2 Is Nothing Then
            For Each dr As DataRow In dt2.Rows
                Me.Page.ClientScript.RegisterForEventValidation(supporterSelectDDL2.ID, Talent.eCommerce.Utilities.CheckForDBNull_String(dr("TEAM_CODE")))
            Next
        End If

        If Not dt3 Is Nothing Then
            For Each dr As DataRow In dt3.Rows
                Me.Page.ClientScript.RegisterForEventValidation(supporterSelectDDL3.ID, Talent.eCommerce.Utilities.CheckForDBNull_String(dr("SC_CODE")))
            Next
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
            customersuffix.Attributes.Add(onKeyDown, EventText)
            nickname.Attributes.Add(onKeyDown, EventText)
            contactsl.Attributes.Add(onKeyDown, EventText)
            altusername.Attributes.Add(onKeyDown, EventText)
            initials.Attributes.Add(onKeyDown, EventText)
            salutation.Attributes.Add(onKeyDown, EventText)
            txtCompanyName.Attributes.Add(onKeyDown, EventText)
            position.Attributes.Add(onKeyDown, EventText)
            email.Attributes.Add(onKeyDown, EventText)
            emailConfirm.Attributes.Add(onKeyDown, EventText)
            phone.Attributes.Add(onKeyDown, EventText)
            work.Attributes.Add(onKeyDown, EventText)
            mobile.Attributes.Add(onKeyDown, EventText)
            fax.Attributes.Add(onKeyDown, EventText)
            other.Attributes.Add(onKeyDown, EventText)
            loginid.Attributes.Add(onKeyDown, EventText)
            password1.Attributes.Add(onKeyDown, EventText)
            password2.Attributes.Add(onKeyDown, EventText)
            type.Attributes.Add(onKeyDown, EventText)
            reference.Attributes.Add(onKeyDown, EventText)
            sequence.Attributes.Add(onKeyDown, EventText)
            building.Attributes.Add(onKeyDown, EventText)
            postcode.Attributes.Add(onKeyDown, AddressEventText)
            street.Attributes.Add(onKeyDown, EventText)
            town.Attributes.Add(onKeyDown, EventText)
            city.Attributes.Add(onKeyDown, EventText)
            county.Attributes.Add(onKeyDown, EventText)
            country.Attributes.Add(onKeyDown, EventText)
            If Not Talent.eCommerce.Utilities.IsAgent Then TandCs.Attributes.Add(onKeyDown, EventText)
        End If

    End Sub

    Protected Sub SetUpRegistrationTypeBox()
        'PageTrackingDetailsView.DataSource = TDataObjects.TrackingSettings.TblPageTracking.GetPageTrackingAll(False)
        ' PageTrackingDetailsView.DataBind()
        Dim dt As Data.DataTable = TDataObjects.ProfileSettings.tblRegistrationType.GetRegistrationTypeAll(True)
        If dt.Rows.Count = 0 Then
            plhRegistrationTypeBox.Visible = False
        Else
            RegistrationTypeHeaderLabel.Text = ucr.Content("registrationTypeHeaderLabel", _languageCode, True).ToString
            registrationTypeLabel.Text = ucr.Content("registrationTypeLabel", _languageCode, True).ToString
            With ddlRegistrationType
                .DataSource = dt
                .DataTextField = "REGISTRATION_TYPE"
                .DataValueField = "REGISTRATION_TYPE"
                .DataBind()
                For Each rw As Data.DataRow In dt.Rows
                    If CBool(rw("IS_DEFAULT")) Then
                        .SelectedValue = rw("REGISTRATION_TYPE")
                    End If
                Next
            End With

        End If
    End Sub

    Protected Sub SetUpUCR()
        If Me.Display Then
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.Common.Utilities.GetAllString 'GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "RegistrationForm.ascx"
            End With
        End If
    End Sub

    Protected Sub SetUpOpt()
        If Me.Display Then
            With ucr
                'do visibility
                plhMailOption.Visible = CType(.Attribute("mailOptionEnabled"), Boolean)
                plhOpt1.Visible = CType(.Attribute("opt1Enabled"), Boolean)
                plhOpt2.Visible = CType(.Attribute("opt2Enabled"), Boolean)
                plhOpt3.Visible = CType(.Attribute("opt3Enabled"), Boolean)
                plhOpt4.Visible = CType(.Attribute("opt4Enabled"), Boolean)
                plhOpt5.Visible = CType(.Attribute("opt5Enabled"), Boolean)
                'do default status
                mailOption.Checked = CType(.Attribute("mailOptionDefault"), Boolean)
                opt1.Checked = CType(.Attribute("opt1Default"), Boolean)
                opt2.Checked = CType(.Attribute("opt2Default"), Boolean)
                opt3.Checked = CType(.Attribute("opt3Default"), Boolean)
                opt4.Checked = CType(.Attribute("opt4Default"), Boolean)
                opt5.Checked = CType(.Attribute("opt5Default"), Boolean)
                cbContactbyEmail.Checked = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ContactbyEmailDefault"))
                cbContactbyTelephoneHome.Checked = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ContactbyTelephoneHomeDefault"))
                cbContactbyTelephoneWork.Checked = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ContactbyTelephoneWorkDefault"))
                cbContactbyMobile.Checked = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ContactbyMobileDefault"))
                cbContactbyPost.Checked = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ContactbyPostDefault"))
                'do meaning
                mailOptionTrueValue = CType(.Attribute("mailOptionTrueValue"), Boolean)
                opt1TrueValue = CType(.Attribute("opt1TrueValue"), Boolean)
                opt2TrueValue = CType(.Attribute("opt2TrueValue"), Boolean)
                opt3TrueValue = CType(.Attribute("opt3TrueValue"), Boolean)
                opt4TrueValue = CType(.Attribute("opt4TrueValue"), Boolean)
                opt5TrueValue = CType(.Attribute("opt5TrueValue"), Boolean)
                'do text
                mailOption.Text = CType(.Content("mailOptionText", _languageCode, True), String)
                opt1.Text = CType(.Content("opt1Text", _languageCode, True), String)
                opt2.Text = CType(.Content("opt2Text", _languageCode, True), String)
                opt3.Text = CType(.Content("opt3Text", _languageCode, True), String)
                opt4.Text = CType(.Content("opt4Text", _languageCode, True), String)
                opt5.Text = CType(.Content("opt5Text", _languageCode, True), String)

                Me.plhAddMembership.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(.Attribute("AddMembership_ShowCheckbox"))
                Me.addMembershipCheck.Checked = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(.Attribute("AddMembership_CheckedByDefault"))
                Me.addMembershipCheck.Text = .Content("AddMembership_CheckboxLabel", _languageCode, True)
            End With
        End If
    End Sub

    Protected Sub SetControlForRegistration()
        If Me.Display Then
            plhCredential.Visible = True
            registerBtn.Visible = True
        End If
    End Sub

    Private Sub SetLabelText()
        If Me.Display Then
            Try

                registerBtn.Text = ucr.Content("registerBtn", _languageCode, True)                                      ' Register"
                passwordInstructionsLabel.Text = ucr.Content("passwordInstructionsLabel", _languageCode, True)          ' (Passwords need to be 6-10 characters long and contain at least 1 number)"
                loginIDLabel.Text = ucr.Content("loginIDLabel", _languageCode, True)                                    ' Username"
                password1Label.Text = ucr.Content("password1Label", _languageCode, True)                                ' Password"
                password2Label.Text = ucr.Content("password2Label", _languageCode, True)                                ' Confirm Password"

                With ucr
                    CredentialsLabel.Text = .Content("credentialsLabel", _languageCode, True)                                ' Login Credentials"
                    PersonalDetailsLabel.Text = .Content("personalDetailsLabel", _languageCode, True)                        ' Personal Details"
                    AddressInfoLabel.Text = .Content("addressInfoLabel", _languageCode, True)                                ' Address Info"
                    '
                    emailLabel.Text = .Content("emailLabel", _languageCode, True)                                            ' Email"
                    emailConfirmLabel.Text = .Content("ConfirmEmailLabel", _languageCode, True)                              ' Confirm Email"
                    titleLabel.Text = .Content("titleLabel", _languageCode, True)                                            ' Title"
                    forenameLabel.Text = .Content("forenameLabel", _languageCode, True)                                      ' Forename"
                    initialsLabel.Text = .Content("initialsLabel", _languageCode, True)                                      ' Initials"
                    surnameLabel.Text = .Content("surnameLabel", _languageCode, True)                                        ' Surname"
                    customersuffixLabel.Text = .Content("customersuffixLabel", _languageCode, True)                                        ' Customer Suffix"
                    nicknameLabel.Text = .Content("nicknameLabel", _languageCode, True)                                        ' Nickname"
                    altusernameLabel.Text = .Content("altusernameLabel", _languageCode, True)                                        ' Username"
                    contactslLabel.Text = .Content("contactSLLabel", _languageCode, True)                                        ' Username"
                    salutationLabel.Text = .Content("salutationLabel", _languageCode, True)                                  ' Salutation"
                    lblCompanyName.Text = .Content("CompanyNameLabelText", _languageCode, True)                              ' Company"
                    positionLabel.Text = .Content("positionLabel", _languageCode, True)                                      ' Position"
                    dobLabel.Text = .Content("dobLabel", _languageCode, True)                                                ' Date of Birth"
                    sexLabel.Text = .Content("sexLabel", _languageCode, True)
                    '
                    mobileLabel.Text = .Content("mobileLabel", _languageCode, True)                                          ' Mobile Number"
                    phoneLabel.Text = .Content("phoneLabel", _languageCode, True)                                            ' Telephone Number"
                    workLabel.Text = .Content("workLabel", _languageCode, True)                                              ' Work Number"
                    faxLabel.Text = .Content("faxLabel", _languageCode, True)                                                ' Fax Number"
                    otherLabel.Text = .Content("otherLabel", _languageCode, True)                                            ' Misc Number"
                    mothersnameLabel.Text = .Content("mothersnameLabel", _languageCode, True)
                    fathersnameLabel.Text = .Content("fathersnameLabel", _languageCode, True)

                    '
                    typeLabel.Text = .Content("typeLabel", _languageCode, True)                                              ' Type"
                    referenceLabel.Text = .Content("referenceLabel", _languageCode, True)                                    ' Address Reference"
                    sequenceLabel.Text = .Content("sequenceLabel", _languageCode, True)                                      ' Sequence"
                    buildingLabel.Text = .Content("buildingLabel", _languageCode, True)                                      ' Building Name/No"
                    streetLabel.Text = .Content("streetLabel", _languageCode, True)                                          ' Street"
                    townLabel.Text = .Content("townLabel", _languageCode, True)                                              ' Town"
                    cityLabel.Text = .Content("cityLabel", _languageCode, True)                                              ' City"
                    countyLabel.Text = .Content("countyLabel", _languageCode, True)                                          ' County"
                    postcodeLabel.Text = .Content("postcodeLabel", _languageCode, True)                                      ' Postcode"
                    countryLabel.Text = .Content("countryLabel", _languageCode, True)                                        ' Country"

                    ltlContactPrefsH2.Text = .Content("ContactPreferencesH2", _languageCode, True)
                    Newsletter.Text = .Content("SubscribeToNewsletterText", _languageCode, True)
                    Subscribe2.Text = .Content("Subscribe2Text", _languageCode, True)
                    Subscribe3.Text = .Content("Subscribe3Text", _languageCode, True)
                    TandCs.Text = .Content("AcceptTandCsText", _languageCode, True)
                    If Talent.eCommerce.Utilities.IsAgent Then
                        PlhShowPermissionCheckBox.Visible = True
                    Else
                        PlhShowPermissionCheckBox.Visible = False
                    End If
                    lblParentalConsentCeilingYear.Text = _ParentalConsentCeilingDob.Year
                    lblParentalConsentCeilingMonth.Text = _ParentalConsentCeilingDob.Month
                    lblParentalConsentCeilingDay.Text = _ParentalConsentCeilingDob.Day

                    '**davetodo - move to database
                    NeedAdultConsentForContactPermissionsHeading.Text = .Content("NeedAdultConsentForContactPermissionsHeading", _languageCode, True)
                    NeedAdultConsentForContactPermisionsInsructions.Text = .Content("NeedAdultConsentForContactPermisionsInsructions", _languageCode, True)
                    parentemailLabel.Text = .Content("parentemailLabeltext", _languageCode, True)
                    parentphonelabel.Text = .Content("parentphonelabeltext", _languageCode, True)
                    cbParentalPermissionGranted.Text = .Content("ParentalPermissionGrantedtext", _languageCode, True)
                    lblParentalConsentAlertHeading.Text = .Content("ParentalConsentAlertHeadingtext", _languageCode, True)
                    lblParentalConsentAlertDetails.Text = .Content("ParentalConsentAlertDetails", _languageCode, True)
                    lblShowAParentalConsentAlert.Text = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("ShowAParentalConsentAlert"))
                    lblparentEmailRFVStatus.Text = parentemailRFV.Enabled.ToString.ToLower
                    lblparentPhoneRFVStatus.Text = parentphoneRFV.Enabled.ToString.ToLower

                    '----------------------------------------------
                    ' Set Registration Address panel for Agent Mode
                    '----------------------------------------------
                    If plhRegisteredAddress.Visible Then
                        ltlRegAddressHdr.Text = .Content("RegAddressHeaderLabel", _languageCode, True)
                        chkRegAddressSame.Text = .Content("RegAddressSameAsDeliveryCheckbox", _languageCode, True)
                        lblRegAddress1.Text = .Content("RegAddress1Label", _languageCode, True)
                        lblRegAddress2.Text = .Content("RegAddress2Label", _languageCode, True)
                        lblRegAddress3.Text = .Content("RegAddress3Label", _languageCode, True)
                        lblRegAddress4.Text = .Content("RegAddress4Label", _languageCode, True)
                        lblRegAddress5.Text = .Content("RegAddress5Label", _languageCode, True)
                        lblRegPostcode.Text = .Content("RegPostcodeLabel", _languageCode, True)
                        lblRegCountry.Text = .Content("RegCountryLabel", _languageCode, True)

                        chkRegAddressSame.Checked = True
                        chkRegAddressSame_CheckedChanged(chkRegAddressSame, New System.EventArgs)

                    End If

                    '---------------------------------------------
                    ' Set fields that are only shown in agent mode 
                    '---------------------------------------------
                    If Talent.eCommerce.Utilities.IsAgent Then
                        ddlpriceband.Visible = True
                        plhAgentOnlyFields.Visible = True
                        pricebandlabel.Text = .Content("pricebandlabel", _languageCode, True)
                        Stopcodelabel.Text = .Content("Stopcodelabel", _languageCode, True)
                        BookNumberLabel.Text = .Content("BookNumberLabel", _languageCode, True)
                    Else
                        plhContactByPost.Visible = False
                        ddlpriceband.Visible = False
                        plhAgentOnlyFields.Visible = False
                    End If

                    ContactbyTelephoneHomeLabel.Text = .Content("ContactbyTelHomeLabel", _languageCode, True)
                    ContactbyTelephoneWorkLabel.Text = .Content("ContactbyTelWorkLabel", _languageCode, True)
                    ContactbyEmailLabel.Text = .Content("ContactbyEMailLabel", _languageCode, True)
                    ContactbyMobileLabel.Text = .Content("ContactbyMobileLabel", _languageCode, True)
                    ContactbyPostLabel.Text = .Content("ContactbyPostLabel", _languageCode, True)
                    plhContactByPost.Visible = True
                    '-----------------------------

                    '-----------------------------
                    ' Set Text for User IDs Group
                    '-----------------------------
                    If plhUserIDs.Visible Then
                        ltlUserIDsHeaderLabel.Text = .Content("lblUserIDsHeaderLabelText", _languageCode, True)
                        lblPin.Text = .Content("lblUserID1Text", _languageCode, True)
                        lblPassport.Text = .Content("lblUserID2Text", _languageCode, True)
                        lblPassportMsg.Text = .Content("lblUserID2Msg", _languageCode, True)
                        lblGreenCard.Text = .Content("lblUserID3Text", _languageCode, True)
                        lblGreenCardMsg.Text = .Content("lblUserID3Msg", _languageCode, True)
                        lblUserID4.Text = .Content("lblUserID4Text", _languageCode, True)
                        lblUserID5.Text = .Content("lblUserID5Text", _languageCode, True)
                        lblUserID6.Text = .Content("lblUserID6Text", _languageCode, True)
                        lblUserID7.Text = .Content("lblUserID7Text", _languageCode, True)
                        lblUserID8.Text = .Content("lblUserID8Text", _languageCode, True)
                        lblUserID9.Text = .Content("lblUserID9Text", _languageCode, True)
                    End If
                    IsConfirmationEmailLabel.Text = .Content("IsConfirmationEmailLabel", _languageCode, True)
                End With
                '

                With NewsletterChoice
                    .Items.Add(ucr.Content("HTMLNewsletterText", _languageCode, True))
                    .Items.Add(ucr.Content("PlainTextNewsletterText", _languageCode, True))
                End With

                'Setup Max Lengths for textboxes
                With ucr
                    forename.MaxLength = .Attribute("forenameMaxLength")
                    surname.MaxLength = .Attribute("surnameMaxLength")
                    customersuffix.MaxLength = .Attribute("customersuffixMaxLength")
                    nickname.MaxLength = .Attribute("nicknameMaxLength")
                    altusername.MaxLength = .Attribute("altusernameMaxLength")
                    contactsl.MaxLength = .Attribute("contactslMaxLength")
                    initials.MaxLength = .Attribute("initialsMaxLength")
                    salutation.MaxLength = .Attribute("salutationMaxLength")
                    txtCompanyName.MaxLength = .Attribute("CompanyNameMaxLength")
                    position.MaxLength = .Attribute("positionMaxLength")
                    email.MaxLength = .Attribute("emailMaxLength")
                    emailConfirm.MaxLength = .Attribute("emailMaxLength")
                    phone.MaxLength = .Attribute("phoneMaxLength")
                    work.MaxLength = .Attribute("workMaxLength")
                    mobile.MaxLength = .Attribute("mobileMaxLength")
                    fax.MaxLength = .Attribute("faxMaxLength")
                    other.MaxLength = .Attribute("otherMaxLength")
                    loginid.MaxLength = .Attribute("loginIDMaxLength")
                    password1.MaxLength = .Attribute("passwordMaxLength")
                    password2.MaxLength = .Attribute("passwordMaxLength")
                    type.MaxLength = .Attribute("typeMaxLength")
                    reference.MaxLength = .Attribute("referenceMaxLength")
                    sequence.MaxLength = .Attribute("sequenceMaxLength")
                    building.MaxLength = .Attribute("buildingMaxLength")
                    postcode.MaxLength = .Attribute("postcodeMaxLength")
                    street.MaxLength = .Attribute("streetMaxLength")
                    town.MaxLength = .Attribute("townMaxLength")
                    city.MaxLength = .Attribute("cityMaxLength")
                    county.MaxLength = .Attribute("countyMaxLength")
                    txtRegPostcode.MaxLength = .Attribute("postcodeMaxLength")
                    parentemail.MaxLength = .Attribute("emailMaxLength")
                    parentphone.MaxLength = .Attribute("mobileMaxLength")
                End With

            Catch ex As Exception
            End Try
        End If

    End Sub

    Protected Function getError(ByVal errCode As String) As String
        errMsg = New Talent.Common.TalentErrorMessages(_languageCode, _
                                                TalentCache.GetBusinessUnitGroup, _
                                                TalentCache.GetPartner(Profile), _
                                                ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)

        Return errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                            errCode).ERROR_MESSAGE
    End Function

    Public Sub SetupCompareValidator(ByVal sender As Object, ByVal e As EventArgs)
        If Me.Display Then
            Try
                SetUpUCR()
                Dim cv As CompareValidator = CType(sender, CompareValidator)
                Select Case cv.ControlToValidate
                    Case Is = "password2"
                        cv.ErrorMessage = ucr.Content("ComparePasswordsErrorText", _languageCode, True)
                    Case Is = "emailConfirm"
                        cv.ErrorMessage = ucr.Content("CompareEmailErrorText", _languageCode, True)
                    Case Else
                        cv.ErrorMessage = ucr.Content(cv.ControlToValidate & "CompareValidator", _languageCode, True)
                End Select
            Catch ex As Exception
            End Try
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
                ElseIf rfv.ControlToValidate = "emailConfirm" Then
                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("emailEnableRFV")) Then
                        Try
                            rfv.ErrorMessage = ucr.Content("CompareEmailErrorText", _languageCode, True)
                        Catch ex As Exception
                        End Try
                    Else
                        rfv.Enabled = False
                    End If
                Else
                    rfv.Enabled = False
                    rfv.Visible = False
                End If
            Catch ex As Exception
            End Try
        End If

    End Sub

    Public Sub SetupRegRequiredValidator(ByVal sender As Object, ByVal e As EventArgs)
        If Me.Display Then
            Dim rfv As RequiredFieldValidator = CType(sender, RequiredFieldValidator)
            Try
                SetUpUCR()
                If plhRegisteredAddress.Visible AndAlso _
                    Not chkRegAddressSame.Checked AndAlso _
                    Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute(rfv.ControlToValidate & "EnableRFV")) Then
                    Try
                        rfv.ErrorMessage = ucr.Content(rfv.ControlToValidate & "RequiredFieldValidator", _languageCode, True)
                        rfv.Enabled = True
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

            Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
            Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

            Dim rev As RegularExpressionValidator = CType(sender, RegularExpressionValidator)

            rev.ErrorMessage = ucr.Content(rev.ControlToValidate & "ErrorText", _languageCode, True)
            If Not String.IsNullOrEmpty(ucr.Attribute(rev.ControlToValidate & "RegEx")) Then
                rev.ValidationExpression = ucr.Attribute(rev.ControlToValidate & "RegEx")
            Else
                Select Case (rev.ControlToValidate)
                    Case Is = "title"
                        'rev.ValidationExpression = "^(?! -- )?[a-zA-Z\s]+"
                        '   rev.ValidationExpression = "^[a-zA-Z\s]{0,50}$"
                        rev.ValidationExpression = "^[^-]{0,50}$"

                    Case Is = "password1"
                        rev.ValidationExpression = ucr.Attribute("PasswordExpression")

                    Case Is = "txtCompanyName"
                        rev.ValidationExpression = ucr.Attribute("CompanyNameExpression")

                    Case Is = "forename", _
                                "surname", _
                                "customersuffix", _
                                "nickname", _
                                "altusername", _
                                "salutation", _
                                "position", _
                                "type", _
                                "reference", _
                                "contactsl"
                        rev.ValidationExpression = ucr.Attribute("TextOnlyExpression")

                    Case Is = "initials"
                        rev.ValidationExpression = ucr.Attribute("InitialsExpression")

                    Case Is = "dobDay"
                        'rev.ValidationExpression = "^(?! -- )?[a-zA-Z0-9\s]+"
                        rev.ValidationExpression = "^[a-zA-Z0-9\s]+"

                    Case Is = "dobMonth"
                        'rev.ValidationExpression = "^(?! -- )?[a-zA-Z0-9\s]+"
                        rev.ValidationExpression = "^[a-zA-Z0-9\s]+"

                    Case Is = "dobYear"
                        'rev.ValidationExpression = "^(?! -- )?[a-zA-Z0-9\s]+"
                        rev.ValidationExpression = "^[a-zA-Z0-9\s]+"

                    Case Is = "email", "parentemail"
                        rev.ValidationExpression = ucr.Attribute("EmailExpression")

                    Case Is = "phone", _
                                "work", _
                                "mobile", _
                                "fax", _
                                "parentphone", _
                                "other"
                        rev.ValidationExpression = ucr.Attribute("PhoneNumberExpression")

                    Case Is = "sequence"
                        rev.ValidationExpression = ucr.Attribute("NumberExpression")

                    Case Is = "postcode", "txtRegPostcode"
                        rev.ValidationExpression = ucr.Attribute("PostcodeExpression")

                    Case Is = "country", "ddlRegCountry"
                        'rev.ValidationExpression = "^(?! -- )?[a-zA-Z\s]+"
                        rev.ValidationExpression = "^[a-zA-Z\s]{0,50}$"

                    Case Is = "sex"
                        rev.ValidationExpression = "^(?! -- )?[a-zA-Z\s]+"
                        rev.Enabled = def.ProfileSexMandatory

                    Case Is = "building", _
                                "street", _
                                "town", _
                                "city", _
                                "county", _
                                "txtRegAddress1", _
                                "txtRegAddress2", _
                                "txtRegAddress3", _
                                "txtRegAddress4", _
                                "txtRegAddress5"
                        rev.ValidationExpression = ucr.Attribute("AddressExpression")

                End Select
            End If
        End If

    End Sub

    Protected Sub PopulateSexDropDownLists()
        If Me.Display Then
            With ucr
                Dim err As New ErrorObj
                Dim utilitites As New TalentUtiltities
                Dim Settings As DESettings = Talent.eCommerce.Utilities.GetSettingsObject

                utilitites.DescriptionKey = "GEND"
                utilitites.Settings = Settings

                err = utilitites.RetrieveDescriptionEntries()

                If Not err.HasError AndAlso Not utilitites.ResultDataSet Is Nothing AndAlso utilitites.ResultDataSet.Tables.Count > 1 Then
                    For Each dr As DataRow In utilitites.ResultDataSet.Tables(1).Rows
                        Dim l2 As New ListItem(dr.Item("Description").ToString.Trim, dr.Item("Code").ToString.Trim)
                        sex.Items.Add(l2)
                    Next
                End If
                sex.Items.Insert(0, New ListItem(" -- ", " -- "))
            End With
        End If
    End Sub

    Protected Sub PopulateDOBDropDownLists()
        If Me.Display Then
            Dim l1 As New ListItem(" -- ", " -- ")
            dobDay.Items.Add(l1)
            For i As Integer = 0 To 30
                l1 = New ListItem(i + 1, i + 1)
                dobDay.Items.Add(l1)
            Next
            '---------------------------------------------------------------------
            'Set Culture for Month Names ComboBox
            Dim myCulture As System.Globalization.CultureInfo
            myCulture = New System.Globalization.CultureInfo(ModuleDefaults.Culture)
            System.Threading.Thread.CurrentThread.CurrentCulture = myCulture

            ' if populated, pick up months from here rather than trying to get
            ' them from culture
            Dim months() As String

            If ModuleDefaults.NonEnglishMonths <> String.Empty Then
                months = ("," + (ModuleDefaults.NonEnglishMonths)).Split(",")
            Else
                months = Nothing
            End If

            Dim l2 As New ListItem(" -- ", " -- ")
            With dobMonth
                .Items.Add(l2)
                For i As Integer = 1 To 12
                    If ModuleDefaults.NonEnglishMonths <> String.Empty Then
                        l2 = New ListItem(months(i), i)
                    Else
                        l2 = New ListItem(MonthName(i), i)
                    End If

                    .Items.Add(l2)
                Next
            End With
            '---------------------------------------------------------------------
            Dim l3 As New ListItem(" -- ", " -- ")
            dobYear.Items.Add(l3)

            Dim yearsSubtract As Integer = Year(Now) - 1900
            Try
                yearsSubtract = CInt(ucr.Attribute("DOBNumberOfYearsBackFromCurrent"))
            Catch ex As Exception
            End Try

            For i As Integer = (Year(Now)) To ((Year(Now)) - yearsSubtract) Step -1
                l3 = New ListItem(i, i)
                dobYear.Items.Add(l3)
            Next
            '---------------------------------------------------------------------
        End If

    End Sub

    Protected Sub PopulateTitleDropDownList()
        If Me.Display Then
            If ModuleDefaults.GetCustomerTitlesFromIseries = True Then
                Dim Settings As DESettings = Talent.eCommerce.Utilities.GetSettingsObject
                Dim defaults As New TalentDefaults
                Dim err As New Talent.Common.ErrorObj
                defaults.Settings = Settings
                err = defaults.RetrieveCustomerTitles
                title.DataSource = TalentCache.GetListCollectionFromTitlesDataTable(defaults.ResultDataSet.Tables("CustomerTitles"))
            Else
                title.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "REGISTRATION", "TITLE")
            End If
            title.DataTextField = "Text"
            title.DataValueField = "Value"
            title.DataBind()
        End If

    End Sub
    Protected Sub PopulateCountriesDropDownList()
        If Me.Display Then
            country.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "REGISTRATION", "COUNTRY")
            country.DataTextField = "Text"
            country.DataValueField = "Value"
            country.DataBind()
            Dim defaultCountry As String = String.Empty
            'Select the default country
            If ModuleDefaults.UseDefaultCountryOnRegistration Then
                defaultCountry = TalentCache.GetDefaultCountryForBU()
                country.SelectedValue = defaultCountry
            End If

            If plhRegisteredAddress.Visible Then
                ddlRegCountry.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "REGISTRATION", "COUNTRY")
                ddlRegCountry.DataTextField = "Text"
                ddlRegCountry.DataValueField = "Value"
                ddlRegCountry.DataBind()
                If ModuleDefaults.UseDefaultCountryOnRegistration Then
                    If defaultCountry <> String.Empty Then
                        ddlRegCountry.SelectedValue = defaultCountry
                    End If
                End If
            End If
        End If
    End Sub

    Protected Sub SetCredentialsVisibility()
        If Me.Display Then
            Try
                With ucr
                    If Not CType(.Attribute("password1RowVisible"), Boolean) Then
                        plhCredential.Visible = False
                        CredentialsBoxTitle.Visible = False
                        PasswordInstructionsRow.Visible = False
                        plhUserName.Visible = False
                        plhPassword1Row.Visible = False
                        plhPassword2Row.Visible = False
                    Else
                        CredentialsBoxTitle.Visible = CType(.Attribute("credentialsBoxTitleVisible"), Boolean)
                        PasswordInstructionsRow.Visible = CType(.Attribute("passwordInstructionsRowVisible"), Boolean)
                        plhUserName.Visible = CType(.Attribute("usernameRowVisible"), Boolean)
                        plhPassword1Row.Visible = CType(.Attribute("password1RowVisible"), Boolean)
                        plhPassword2Row.Visible = CType(.Attribute("password2RowVisible"), Boolean)
                    End If
                End With
            Catch ex As Exception
            End Try
        End If

    End Sub

    Protected Sub Setup_Supporter_Favourites_And_Club_Contact_Options()
        If Me.Display Then
            Try
                plhSupporter.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("SupporterSelectionsInUse"))
                'Stuart
                'favouritesDiv.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("FavouriteClubSelectionInUse"))
                plhContactMethod.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("ClubsContactSelectionInUse"))

                If plhSupporter.Visible Then
                    supporterSelectLabel1.Text = ucr.Content("supporterSelectLabel1_Text", _languageCode, True)
                    supporterSelectLabel2.Text = ucr.Content("supporterSelectLabel2_Text", _languageCode, True)
                    supporterSelectLabel3.Text = ucr.Content("supporterSelectLabel3_Text", _languageCode, True)
                    supporterSelectButton1.Text = ucr.Content("supporterSelectButton1_Text", _languageCode, True)
                    supporterSelectButton2.Text = ucr.Content("supporterSelectButton2_Text", _languageCode, True)

                    PopulateSupporterSportDDL()

                End If

                'StuartS
                'If favouritesDiv.Visible Then
                '    favouritesLabel.Text = ucr.Content("favouritesLabel_Text", _languageCode, True)
                '    PopulateFavouriteTeamDDL()
                'End If

                If plhContactMethod.Visible Then
                    contactMethodLabel.Text = ucr.Content("contactMethodLabel_Text", _languageCode, True)
                    PopulateSportTeamMailFlags()
                    PopulateContactMethodDDL()
                End If

            Catch ex As Exception
            End Try
        End If

    End Sub

    Protected Sub SetupUserIDsGroup()
        plhUserIDs.Visible = ModuleDefaults.ShowUserIDFields
        If plhUserIDs.Visible Then
            plhPassport.Visible = ModuleDefaults.ShowPassportField
            plhGreencard.Visible = ModuleDefaults.ShowGreenCardField
            plhPin.Visible = ModuleDefaults.ShowPinField
            plhUserID4.Visible = ModuleDefaults.ShowUserID4Field
            plhUserID5.Visible = ModuleDefaults.ShowUserID5Field
            plhUserID6.Visible = ModuleDefaults.ShowUserID6Field
            plhUserID7.Visible = ModuleDefaults.ShowUserID7Field
            plhUserID8.Visible = ModuleDefaults.ShowUserID8Field
            plhUserID9.Visible = ModuleDefaults.ShowUserID9Field
            If Session("Passport") IsNot Nothing AndAlso Not Session("Passport").Equals(String.Empty) Then
                txtPassport.Text = CType(Session("Passport"), String)
            End If
            If Session("PIN") IsNot Nothing AndAlso Not Session("PIN").Equals(String.Empty) Then
                txtPin.Text = CType(Session("PIN"), String)
            End If
            If Session("Greencard") IsNot Nothing AndAlso Not Session("Greencard").Equals(String.Empty) Then
                txtGreenCard.Text = CType(Session("Greencard"), String)
            End If
            If Session("UserID4") IsNot Nothing AndAlso Not Session("UserID4").Equals(String.Empty) Then
                txtUserID4.Text = CType(Session("UserID4"), String)
            End If
            If Session("UserID5") IsNot Nothing AndAlso Not Session("UserID5").Equals(String.Empty) Then
                txtUserID5.Text = CType(Session("UserID5"), String)
            End If
            If Session("UserID6") IsNot Nothing AndAlso Not Session("UserID6").Equals(String.Empty) Then
                txtUserID6.Text = CType(Session("UserID6"), String)
            End If
            If Session("UserID7") IsNot Nothing AndAlso Not Session("UserID7").Equals(String.Empty) Then
                txtUserID7.Text = CType(Session("UserID7"), String)
            End If
            If Session("UserID8") IsNot Nothing AndAlso Not Session("UserID8").Equals(String.Empty) Then
                txtUserID8.Text = CType(Session("UserID8"), String)
            End If
            If Session("UserID9") IsNot Nothing AndAlso Not Session("UserID9").Equals(String.Empty) Then
                txtUserID9.Text = CType(Session("UserID9"), String)
            End If
        End If

        'Populate fields from additional RAS sessions if they are available.
        If Session("AgentForename") IsNot Nothing AndAlso Not Session("AgentForename").Equals(String.Empty) Then
            forename.Text = CType(Session("AgentForename"), String)
            Session("AgentForename") = Nothing
        End If
        If Session("AgentSurname") IsNot Nothing AndAlso Not Session("AgentSurname").Equals(String.Empty) Then
            surname.Text = CType(Session("AgentSurname"), String)
            Session("AgentSurname") = Nothing
        End If
        If Session("AgentAddressLine1") IsNot Nothing AndAlso Not Session("AgentAddressLine1").Equals(String.Empty) Then
            street.Text = CType(Session("AgentAddressLine1"), String)
            Session("AgentAddressLine1") = Nothing
        End If
        If Session("AgentAddressLine2") IsNot Nothing AndAlso Not Session("AgentAddressLine2").Equals(String.Empty) Then
            town.Text = CType(Session("AgentAddressLine2"), String)
            Session("AgentAddressLine2") = Nothing
        End If
        If Session("AgentAddressLine3") IsNot Nothing AndAlso Not Session("AgentAddressLine3").Equals(String.Empty) Then
            city.Text = CType(Session("AgentAddressLine3"), String)
            Session("AgentAddressLine3") = Nothing
        End If
        If Session("AgentAddressLine4") IsNot Nothing AndAlso Not Session("AgentAddressLine4").Equals(String.Empty) Then
            county.Text = CType(Session("AgentAddressLine4"), String)
            Session("AgentAddressLine4") = Nothing
        End If
        If Session("AgentPostCode") IsNot Nothing AndAlso Not Session("AgentPostCode").Equals(String.Empty) Then
            postcode.Text = CType(Session("AgentPostCode"), String)
            Session("AgentPostCode") = Nothing
        End If
        If Session("AgentEmail") IsNot Nothing AndAlso Not Session("AgentEmail").Equals(String.Empty) Then
            email.Text = CType(Session("AgentEmail"), String)
            Session("AgentEmail") = Nothing
        End If
    End Sub

    Protected Sub PopulateSportTeamMailFlags()
        Dim dt As DataTable = Talent.eCommerce.Utilities.GetSupporterSportDDL
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            contactRepeater.DataSource = dt
            contactRepeater.DataBind()
        End If
    End Sub

    Protected Sub contactRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles contactRepeater.ItemDataBound
        If e.Item.ItemIndex > -1 Then
            Dim dr As DataRow = CType(e.Item.DataItem, DataRowView).Row

            Dim lbl As Label = CType(e.Item.FindControl("sportLabel"), Label)
            Dim ddl As DropDownList = CType(e.Item.FindControl("teamDDL"), DropDownList)

            lbl.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("DESCRIPTION"))

            Dim sportsTeam As DataTable = Talent.eCommerce.Utilities.GetSupporterSportTeamDDL

            If Not sportsTeam Is Nothing AndAlso sportsTeam.Rows.Count > 0 Then
                Dim rows() As DataRow = sportsTeam.Select("SPORT_CODE = '" & Talent.eCommerce.Utilities.CheckForDBNull_String(dr("SPORT_CODE")) & "'")
                ddl.Items.Clear()
                If rows.Length > 0 Then
                    For i As Integer = 0 To rows.Length - 1
                        Dim li As New ListItem(rows(i)("DESCRIPTION"), rows(i)("TEAM_CODE"))
                        ddl.Items.Add(li)
                    Next
                    ddl.Items.Insert(0, New ListItem(ucr.Content("MailFlagsTeam_SelectAll_Text", _languageCode, True), "ALL"))
                    ddl.Items.Insert(0, New ListItem(ucr.Content("MailFlagsTeam_SelectNone_Text", _languageCode, True), "  "))
                End If
            End If

        Else
            If e.Item.ItemIndex = -1 Then
                Dim lbl As Label = CType(e.Item.FindControl("TeamContactHeaderLabel"), Label)
                lbl.Text = ucr.Content("MailFlagsTeam_Header_Text", _languageCode, True)
            End If
        End If
    End Sub

    Protected Sub PopulateSupporterSportDDL()

        Dim dt As DataTable = Talent.eCommerce.Utilities.GetSupporterSportDDL

        If Not dt Is Nothing AndAlso Not dt.Rows.Count = 0 Then
            supporterSelectDDL1.DataSource = dt
            supporterSelectDDL1.DataTextField = "DESCRIPTION"
            supporterSelectDDL1.DataValueField = "SPORT_CODE"
            supporterSelectDDL1.DataBind()

            supporterSelectDDL1.Items.Insert(0, ucr.Content("supporterSelectDDL1_PleaseSelect_Text", _languageCode, True))

            'Dim i As Integer = 1
            'For Each r As DataRow In dt.Rows
            '    If r.Item("SPORT_CODE") = "FB" Then
            '        Me.Page.RegisterStartupScript("pst", _
            '        "<script language=javascript>" & vbCrLf & _
            '        "   function preSelectTeam() {" & vbCrLf & _
            '        "       document.getElementById('" & supporterSelectDDL1.ClientID & "').selectedIndex = " & i & ";" & vbCrLf & _
            '        "       LoadSupporterSelectDDL2(this.form);" & vbCrLf & _
            '        "       PopulateDDL1Hidden();" & vbCrLf & _
            '        "   }" & vbCrLf & _
            '        "   preSelectTeam();" & vbCrLf & _
            '        "</script>")
            '    End If
            '    i += 1
            'Next

        End If

    End Sub

    Protected Sub PopulateContactMethodDDL()

        Dim dt As DataTable = Talent.eCommerce.Utilities.GetPreferredContactMethodDDL

        If Not dt Is Nothing AndAlso Not dt.Rows.Count = 0 Then
            contactMethodDDL.DataSource = dt
            contactMethodDDL.DataTextField = "DESCRIPTION"
            contactMethodDDL.DataValueField = "CONTACT_CODE"
            contactMethodDDL.DataBind()
            For Each dr As DataRow In dt.Rows
                If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(dr("IS_DEFAULT")) Then
                    For Each li As ListItem In contactMethodDDL.Items
                        If li.Value.ToLower = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("CONTACT_CODE")).ToLower Then
                            li.Selected = True
                        Else
                            li.Selected = False
                        End If
                    Next
                End If
            Next
        End If

    End Sub

    Protected Sub supporterSelectButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles supporterSelectButton1.Click
        Dim dt As DataTable = Talent.eCommerce.Utilities.GetSupporterSportTeamDDL
        If Not dt Is Nothing AndAlso Not dt.Rows.Count = 0 Then
            Dim dr() As DataRow = dt.Select("SPORT_CODE = '" & Me.supporterSelectDDL1.SelectedValue & "'")
            supporterSelectDDL2.Items.Clear()
            If dr.Length > 0 Then
                For i As Integer = 0 To dr.Length - 1
                    Dim li As New ListItem(dr(i)("DESCRIPTION"), dr(i)("TEAM_CODE"))
                    supporterSelectDDL2.Items.Add(li)
                Next
                supporterSelectDDL2.Items.Insert(0, ucr.Content("supporterSelectDDL2_PleaseSelect_Text", _languageCode, True))
            End If
        End If
    End Sub

    Protected Sub supporterSelectButton2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles supporterSelectButton2.Click
        Dim dt As DataTable = Talent.eCommerce.Utilities.GetSupporterSportTeamClubDDL
        If Not dt Is Nothing AndAlso Not dt.Rows.Count = 0 Then
            Dim dr() As DataRow = dt.Select("SPORT_CODE = '" & supporterSelectDDL1.SelectedValue & "' AND TEAM_CODE = '" & supporterSelectDDL2.SelectedValue & "'")
            supporterSelectDDL3.Items.Clear()
            If dr.Length > 0 Then
                For i As Integer = 0 To dr.Length - 1
                    Dim li As New ListItem(dr(i)("DESCRIPTION"), dr(i)("SC_CODE"))
                    supporterSelectDDL3.Items.Add(li)
                Next
                supporterSelectDDL3.Items.Insert(0, ucr.Content("supporterSelectDDL3_PleaseSelect_Text", _languageCode, True))
            End If
        End If
    End Sub

    Protected Sub WriteDDLJavascript()

        If Session("AddressFormat") Is Nothing OrElse Session("AddressFormat") <> "1" Then
            Dim sb As New StringBuilder
            Const sCrLf As String = vbCrLf

            Dim dtSport As DataTable = Talent.eCommerce.Utilities.GetSupporterSportDDL
            Dim dtSportTeam As DataTable = Talent.eCommerce.Utilities.GetSupporterSportTeamDDL
            Dim dtSportTeamClub As DataTable = Talent.eCommerce.Utilities.GetSupporterSportTeamClubDDL

            Dim sSportCode As String = ""
            Dim lastSportCode As String = ""
            Dim sTeamCode As String = ""
            Dim sTeamDesc As String = ""
            Dim lastTeamCode As String = ""
            Dim sClubCode As String = ""
            Dim sClubDesc As String = ""

            sb.Append(sCrLf)
            sb.Append("<script language=""javascript"" type=""text/javascript"">" & sCrLf)

            'Hide the 'Load Areas Button' if Javascript is on
            '--------------------------------------------------------
            sb.Append("document.getElementById('" & supporterSelectButton1.ClientID & "').style.display = 'none';" & sCrLf)
            sb.Append("document.getElementById('" & supporterSelectButton2.ClientID & "').style.display = 'none';" & sCrLf)
            '--------------------------------------------------------

            sb.Append("function removeAllOptions(ddl){ ")
            sb.Append("var i; ")
            sb.Append("for(i=ddl.options.length-1;i>=0;i--)")
            sb.Append("{")
            sb.Append("	ddl.remove(i);")
            sb.Append("}")
            sb.Append("}")
            sb.Append("function addOption(selectbox, value, text){")
            sb.Append("var optn = document.createElement(""OPTION"");")
            sb.Append("optn.text = text;")
            sb.Append("optn.value = value;")
            sb.Append("selectbox.options.add(optn);")
            sb.Append("}")

            'Populate DDL2
            '----------------------------------------------
            sb.Append("function LoadSupporterSelectDDL2(thisform){" & sCrLf)
            sb.Append("var ddl = document.getElementById('" & supporterSelectDDL2.ClientID & "');" & sCrLf)
            sb.Append("var ddl2 = document.getElementById('" & supporterSelectDDL3.ClientID & "');" & sCrLf)
            sb.Append("var sportCode = document.getElementById('" & supporterSelectDDL1.ClientID & "').value;" & sCrLf)
            sb.Append("removeAllOptions(ddl);" & sCrLf)
            sb.Append("removeAllOptions(ddl2);" & sCrLf)
            sb.Append("addOption(ddl, """ & ucr.Content("supporterSelectDDL2_PleaseSelect_Text", _languageCode, True) & """, """ & ucr.Content("supporterSelectDDL2_PleaseSelect_Text", _languageCode, True) & """);" & sCrLf)

            For Each dr As DataRow In dtSportTeam.Rows
                sSportCode = Convert.ToString(dr("SPORT_CODE")).Trim
                sTeamCode = Convert.ToString(dr("TEAM_CODE")).Trim
                sTeamDesc = Convert.ToString(dr("DESCRIPTION")).Trim
                If Not (sSportCode = lastSportCode) Then
                    If Not lastSportCode.Trim = "" Then
                        sb.Append("}" & sCrLf)
                    End If
                    sb.Append("if (trim(sportCode) == """ & sSportCode & """) {" & sCrLf)
                End If
                sb.Append("addOption(ddl, """ & sTeamCode & """, """ & sTeamDesc & """);" & sCrLf)
                lastSportCode = sSportCode
            Next
            sb.Append("}" & sCrLf)
            sb.Append("}" & sCrLf)


            'Populate DDL3
            '----------------------------------------------
            sb.Append("function LoadSupporterSelectDDL3(thisform){" & sCrLf)
            sb.Append("var ddl = document.getElementById('" & supporterSelectDDL3.ClientID & "');" & sCrLf)
            sb.Append("var sportCode = document.getElementById('" & supporterSelectDDL1.ClientID & "').value;" & sCrLf)
            sb.Append("var teamCode = document.getElementById('" & supporterSelectDDL2.ClientID & "').value;" & sCrLf)
            sb.Append("removeAllOptions(ddl);" & sCrLf)
            sb.Append("addOption(ddl, """ & ucr.Content("supporterSelectDDL3_PleaseSelect_Text", _languageCode, True) & """, """ & ucr.Content("supporterSelectDDL3_PleaseSelect_Text", _languageCode, True) & """);" & sCrLf)

            For Each dr As DataRow In dtSportTeamClub.Rows
                sSportCode = Convert.ToString(dr("SPORT_CODE")).Trim
                sTeamCode = Convert.ToString(dr("TEAM_CODE")).Trim
                sClubCode = Convert.ToString(dr("SC_CODE")).Trim
                sClubDesc = Convert.ToString(dr("DESCRIPTION")).Trim
                If Not (sTeamCode = lastTeamCode) Then
                    If Not lastTeamCode.Trim = "" Then
                        sb.Append("}" & sCrLf)
                    End If
                    sb.Append("if (trim(sportCode) == """ & sSportCode & """ && trim(teamCode) == """ & sTeamCode & """) {" & sCrLf)
                End If
                sb.Append("addOption(ddl, """ & sClubCode & """, """ & sClubDesc & """);" & sCrLf)
                lastTeamCode = sTeamCode
            Next
            sb.Append("}" & sCrLf)
            sb.Append("}" & sCrLf)

            ' Javascript trim function
            sb.Append("function trim(s) { " & sCrLf & "var r=/\b(.*)\b/.exec(s); " & sCrLf & "return (r==null)?"""":r[1]; " & sCrLf & "}" & sCrLf)

            sb.Append("function PopulateDDL1Hidden(){" & sCrLf)
            sb.Append("document.getElementById('" & supporterSelect1Hidden.ClientID & "').value = document.getElementById('" & supporterSelectDDL1.ClientID & "').value;" & sCrLf)
            sb.Append("}" & sCrLf)

            sb.Append("function PopulateDDL2Hidden(){" & sCrLf)
            sb.Append("document.getElementById('" & supporterSelect2Hidden.ClientID & "').value = document.getElementById('" & supporterSelectDDL2.ClientID & "').value;" & sCrLf)
            sb.Append("}" & sCrLf)

            sb.Append("function PopulateDDL3Hidden(){" & sCrLf)
            sb.Append("document.getElementById('" & supporterSelect3Hidden.ClientID & "').value = document.getElementById('" & supporterSelectDDL3.ClientID & "').value;" & sCrLf)
            sb.Append("}" & sCrLf)

            sb.Append("</script>" & sCrLf)

            Response.Write(sb.ToString)
        End If
    End Sub

    Protected Sub PopulatePriceBandsDropDownList()
        If Talent.eCommerce.Utilities.IsAgent Then
            Dim err As New Talent.Common.ErrorObj
            Dim pb As New TalentPriceBands
            Dim Settings As DESettings = Talent.eCommerce.Utilities.GetSettingsObject()
            Dim ta As New Talent.eCommerce.Agent

            Settings.Cacheing = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(ucr.Attribute("PriceBandCache"))
            Settings.CacheTimeMinutes = Talent.eCommerce.Utilities.CheckForDBNull_Int(ucr.Attribute("PriceBandCacheTimeInMins"))
            pb.Settings = Settings
            pb.Company = ta.GetAgentCompany
            err = pb.RetrieveTalentPriceBands()

            ' Was the call successful
            If Not err.HasError AndAlso pb.ResultDataSet.Tables.Count() > 0 Then
                ddlpriceband.DataSource = pb.ResultDataSet
                ddlpriceband.DataValueField = "Code"
                ddlpriceband.DataTextField = "Description"
                ddlpriceband.DataBind()
            End If
        End If
    End Sub

    Protected Sub PopulateStopcodesDropDownList()
        If Talent.eCommerce.Utilities.IsAgent Then
            Dim err As New Talent.Common.ErrorObj
            Dim pb As New TalentStopcodes
            Dim Settings As New DESettings

            With Settings
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .BusinessUnit = TalentCache.GetBusinessUnit
                .Company = ucr.Content("crmExternalKeyName", _languageCode, True)
                .Cacheing = True
                .DestinationDatabase = Talent.eCommerce.Utilities.GetCustomerDestinationDatabase()
                .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("TALENTCRM").ToString
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            End With

            pb.Settings = Settings
            Dim ta As New Talent.eCommerce.Agent
            pb.Company = ta.GetAgentCompany

            err = pb.RetrieveTalentStopcodes()

            ' Was the call successful
            If Not err.HasError AndAlso pb.ResultDataSet.Tables.Count() > 0 Then

                ''SortStopcode
                pb.ResultDataSet.Tables(0).DefaultView.Sort = "Code ASC"

                ddlStopcode.DataSource = pb.ResultDataSet.Tables(0).DefaultView
                ddlStopcode.DataValueField = "Code"
                ddlStopcode.DataTextField = "Description"
                ddlStopcode.DataBind()
                ddlStopcode.Items.Insert(0, New ListItem(ucr.Content("NoStopCodeText", _languageCode, True), String.Empty))
            End If
        End If
    End Sub

    Protected Sub SetDetailsVisibility()
        If Me.Display Then
            Try
                With ucr
                    plhTitleRow.Visible = CType(.Attribute("titleRowVisible"), Boolean)
                    plhForenameRow.Visible = CType(.Attribute("forenameRowVisible"), Boolean)
                    plhSurnameRow.Visible = CType(.Attribute("surnameRowVisible"), Boolean)
                    plhCustomersuffixRow.Visible = CType(.Attribute("customersuffixRowVisible"), Boolean)
                    plhNicknameRow.Visible = CType(.Attribute("nicknameRowVisible"), Boolean)
                    plhAltusernameRow.Visible = CType(.Attribute("altusernameRowVisible"), Boolean)
                    plhContactSL.Visible = CType(.Attribute("contactslrowVisible"), Boolean)
                    plhinitialsRow.Visible = CType(.Attribute("initialsRowVisible"), Boolean)
                    plhSalutationRow.Visible = CType(.Attribute("salutationRowVisible"), Boolean)
                    plhCompanyRow.Visible = CType(.Attribute("CompanyNameRowVisible"), Boolean)
                    plhPositionRow.Visible = CType(.Attribute("positionRowVisible"), Boolean)
                    plhDOBRow.Visible = CType(.Attribute("dobRowVisible"), Boolean)
                    plhSexRow.Visible = CType(.Attribute("sexRowVisible"), Boolean)
                    plhEmailRow.Visible = CType(.Attribute("emailRowVisible"), Boolean)
                    plhPhoneRow.Visible = CType(.Attribute("phoneRowVisible"), Boolean)
                    plhWorkRow.Visible = CType(.Attribute("workRowVisible"), Boolean)
                    plhMobileRow.Visible = CType(.Attribute("mobileRowVisible"), Boolean)
                    plhFaxRow.Visible = CType(.Attribute("faxRowVisible"), Boolean)
                    plhOtherRow.Visible = CType(.Attribute("otherRowVisible"), Boolean)
                    plhNewsletter.Visible = CType(.Attribute("newsletterVisible"), Boolean)
                    plhSubscribe2.Visible = CType(.Attribute("subscribe2Visible"), Boolean)
                    plhSubscribe3.Visible = CType(.Attribute("subscribe3Visible"), Boolean)
                    plhMothersNameRow.Visible = CType(.Attribute("mothersnameRowVisible"), Boolean)
                    plhFathersNameRow.Visible = CType(.Attribute("fathersnameRowVisible"), Boolean)
                    plhSupporter.Visible = Convert.ToBoolean(.Attribute("SupporterSelectionsInUse"))
                    plhContactMethod.Visible = Convert.ToBoolean(.Attribute("ClubsContactSelectionInUse"))
                    plhIsConfirmationEmailSend.Visible = CType(.Attribute("IsConfirmationEmailCheckboxVisible"), Boolean)
                End With
                If plhNewsletter.Visible OrElse plhSubscribe2.Visible OrElse
                    plhSubscribe3.Visible OrElse plhMailOption.Visible OrElse
                    plhOpt1.Visible OrElse plhOpt2.Visible OrElse
                    plhOpt3.Visible OrElse plhOpt4.Visible OrElse
                    plhOpt5.Visible Then
                    plhContactPreferences.Visible = True
                Else
                    plhContactPreferences.Visible = False
                End If
            Catch ex As Exception
            End Try
        End If

    End Sub

    Protected Function GetNewsletterType() As Boolean
        If Me.Display Then
            If NewsletterChoice.SelectedValue = ucr.Content("HTMLNewsletterText", _languageCode, True) Then
                Return True
            ElseIf NewsletterChoice.SelectedValue = ucr.Content("PlainTextNewsletterText", _languageCode, True) Then
                Return False
            Else
                Return False
            End If
        End If
        Return False
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

    Protected Function CheckCompanyName() As Boolean

        ' Check if company is mandatory for selected registration type
        Dim valid As Boolean = True

        If plhRegistrationTypeBox.Visible Then
            Dim dt As Data.DataTable = TDataObjects.ProfileSettings.tblRegistrationType.GetRegistrationTypeAll(True)
            For Each dr As Data.DataRow In dt.Rows
                If dr("REGISTRATION_TYPE").ToString = ddlRegistrationType.SelectedValue Then
                    _crmBranch = dr("REGISTRATION_BRANCH").ToString
                    If CBool(dr("IS_COMPANY_MANDATORY").ToString) AndAlso txtCompanyName.Text = String.Empty Then
                        valid = False
                    End If
                End If
            Next
        End If
        Return valid
    End Function

    Protected Sub registerBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles registerBtn.Click
        If Me.Display Then
            If Page.IsValid Then
                Dim profile1 As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter

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
                            If SessionDate <= DateTime.Now AndAlso SessionDate2 <= SessionDate AndAlso Not Profile.IsAnonymous Then
                                If Talent.eCommerce.Utilities.IsAgent Then
                                    Talent.eCommerce.Utilities.GetSiteHomePage()
                                Else
                                    Response.Redirect("~/PagesLogin/Profile/RegistrationConfirmation.aspx")
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                End If

                Dim d As Date
                If CType(ucr.Attribute("dobEnableRFV"), Boolean) OrElse (dobYear.SelectedIndex > 0 AndAlso dobDay.SelectedIndex > 0 AndAlso dobMonth.SelectedIndex > 0) Then
                    Try
                        d = New Date(CInt(dobYear.Text), CInt(dobMonth.Text), CInt(dobDay.Text))
                        '--------------------------
                        ' Check DOB isn't in future
                        '--------------------------
                        If d > Date.Today Then
                            ErrorLabel.Text = ucr.Content("InvalidDOBErrorText", _languageCode, True)
                            registerBtn.Enabled = True
                            Exit Sub
                        End If
                    Catch ex As Exception
                        ErrorLabel.Text = ucr.Content("InvalidDOBErrorText", _languageCode, True)
                        registerBtn.Enabled = True
                        Exit Sub
                    End Try
                Else
                    Try
                        d = CDate("01/01/1900")
                        dobDay.SelectedIndex = 0
                        dobMonth.SelectedIndex = 0
                        dobYear.SelectedIndex = 0
                    Catch ex As Exception
                        registerBtn.Enabled = True
                        ErrorLabel.Text = ucr.Content("InvalidDOBErrorText", _languageCode, True)
                        Exit Sub
                    End Try
                End If

                If Not CheckPostcode() Then
                    ErrorLabel.Text = ucr.Content("PostcodeRequiredFieldValidator", _languageCode, True)
                    registerBtn.Enabled = True
                    Exit Sub
                End If


                If Not CheckCompanyName() Then
                    ErrorLabel.Text = ucr.Content("txtCompanyNameRequiredFieldValidator", _languageCode, True)
                    registerBtn.Enabled = True
                    Exit Sub
                End If
                '

                If AddressFormat1Form1.Visible Then
                    If Not AddressFormat1Form1.ValidateAddress() Then
                        ErrorLabel.Text = ucr.Content("addressFormat1Errors", _languageCode, True)
                        registerBtn.Enabled = True
                        Exit Sub
                    End If
                End If


                ' Check User IDs are required
                If plhUserIDs.Visible AndAlso ModuleDefaults.AgeAtWhichIDFieldsAreMandatory >= 0 Then
                    Dim nowMinus14 As DateTime = Today
                    nowMinus14 = nowMinus14.AddYears(-ModuleDefaults.AgeAtWhichIDFieldsAreMandatory)
                    If d <= nowMinus14 Then
                        'adult
                        If String.IsNullOrEmpty(txtPin.Text.Trim) _
                            AndAlso String.IsNullOrEmpty(txtPassport.Text.Trim) _
                            AndAlso String.IsNullOrEmpty(txtGreenCard.Text.Trim) _
                            AndAlso String.IsNullOrEmpty(txtUserID4.Text.Trim) Then
                            ErrorLabel.Text = ucr.Content("IDRequiredFieldValidator", _languageCode, True)
                            Exit Sub
                        End If

                        ' National ID or Passport ID is required not both
                        If ModuleDefaults.UserIDValidationTypeforID3 = "1" AndAlso
                            Not String.IsNullOrEmpty(txtPassport.Text.Trim) _
                            AndAlso Not String.IsNullOrEmpty(txtPin.Text.Trim) Then
                            ErrorLabel.Text = ucr.Content("IDRequiredOneValidator", _languageCode, True)
                            Exit Sub
                        End If

                    Else
                        'minor
                        If Request.QueryString("source") <> "fandf" Then
                            ErrorLabel.Text = ucr.Content("InvalidRegistrationForMinor", _languageCode, True)
                            Exit Sub
                        End If
                    End If
                End If

                If Talent.eCommerce.Utilities.IsAgent OrElse TandCs.Checked Then

                    ' Duplicate email address check
                    Dim dt As Data.DataTable = profile1.GetDataByEmail(email.Text, TalentCache.GetPartner(HttpContext.Current.Profile))
                    If dt.Rows.Count = 0 Or CType(ModuleDefaults.AllowDuplicateEmail, Boolean) Then

                        ' Do we need to validate and add the customer on the backend first
                        ErrorLabel.Text = String.Empty
                        Try

                            ' If QAS is used then Address Line is never populated and therefore Address1 for CRM 
                            ' needs to be populated from different source fields.
                            Dim eComDefs As New ECommerceModuleDefaults
                            Dim defs As ECommerceModuleDefaults.DefaultValues = eComDefs.GetDefaults
                            ' If QAS is switched on then do not show AddressLine1Row (building)
                            If Not defs.AddressLine1RowVisible Then
                                If String.IsNullOrEmpty(reference.Text) Then
                                    reference.Text = street.Text & " " & town.Text
                                End If
                            Else
                                If String.IsNullOrEmpty(reference.Text) Then
                                    reference.Text = building.Text & " " & street.Text
                                End If
                            End If

                            If String.IsNullOrEmpty(loginid.Text) Then loginid.Text = email.Text

                            'Create and populate the user object
                            Dim userDetails As New TalentProfileUserDetails
                            With userDetails
                                .LoginID = loginid.Text
                                .Email = email.Text
                                .Title = title.SelectedItem.Text
                                .Initials = initials.Text
                                .Forename = forename.Text
                                .Surname = surname.Text
                                .CUSTOMER_SUFFIX = customersuffix.Text
                                .NICKNAME = nickname.Text
                                .USERNAME = altusername.Text
                                .Account_No_2 = contactsl.Text
                                .Full_Name = forename.Text & " " & surname.Text
                                .Salutation = salutation.Text
                                .CompanyName = txtCompanyName.Text
                                .Position = position.Text
                                .DOB = d
                                .Sex = sex.SelectedValue
                                .Mothers_Name = mothersname.Text
                                .Fathers_Name = fathersname.Text
                                .Mobile_Number = mobile.Text
                                .Telephone_Number = phone.Text
                                .Work_Number = work.Text
                                .Fax_Number = fax.Text
                                .Other_Number = other.Text
                                .User_Number = Talent.Common.Utilities.GetNextUserNumber(TalentCache.GetBusinessUnit, _
                                                        TalentCache.GetPartner(Profile), _
                                                        ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
                                .Subscribe_Newsletter = Newsletter.Checked
                                .Subscribe_2 = Subscribe2.Checked
                                .Subscribe_3 = Subscribe3.Checked
                                .HTML_Newsletter = GetNewsletterType()
                                .User_Number_Prefix = ModuleDefaults.UserNumberPrefix

                                .Bit0 = mailOption.Checked
                                .Bit1 = opt1.Checked
                                .Bit2 = opt2.Checked
                                .Bit3 = opt3.Checked
                                .Bit4 = opt4.Checked
                                .Bit5 = opt5.Checked


                                If Not String.IsNullOrEmpty(supporterSelect1Hidden.Value) And supporterSelect1Hidden.Value.ToString.Trim <> ucr.Content("supporterSelectDDL1_PleaseSelect_Text", _languageCode, True).Trim Then
                                    .FAVOURITE_SPORT = Convert.ToString(supporterSelect1Hidden.Value)
                                Else
                                    .FAVOURITE_SPORT = ""
                                End If
                                If Not String.IsNullOrEmpty(supporterSelect2Hidden.Value) And supporterSelect2Hidden.Value.ToString.Trim <> ucr.Content("supporterSelectDDL2_PleaseSelect_Text", _languageCode, True).Trim Then
                                    .FAVOURITE_TEAM_CODE = Convert.ToString(supporterSelect2Hidden.Value)
                                Else
                                    .FAVOURITE_TEAM_CODE = ""
                                End If
                                If Not String.IsNullOrEmpty(supporterSelect3Hidden.Value) And supporterSelect3Hidden.Value.ToString.Trim <> ucr.Content("supporterSelectDDL3_PleaseSelect_Text", _languageCode, True).Trim Then
                                    .SUPPORTER_CLUB_CODE = Convert.ToString(supporterSelect3Hidden.Value)
                                Else
                                    .SUPPORTER_CLUB_CODE = ""
                                End If
                                .PARENT_EMAIL = parentemail.Text
                                .PARENT_PHONE = parentphone.Text

                                .PARENTAL_CONSENT_STATUS = String.Empty
                                If DateTime.Compare(.DOB, _ParentalConsentCeilingDob) Then
                                    If cbParentalPermissionGranted.Checked = True Then
                                        .PARENTAL_CONSENT_STATUS = "G"
                                    Else
                                        .PARENTAL_CONSENT_STATUS = "R"
                                    End If
                                End If

                                'Stuart
                                'If favouritesDDL.SelectedItem.Text = ucr.Content("favouritesDDL_PleaseSelect_Text", _languageCode, True) Then
                                '    .FAVOURITE_TEAM_CODE = ""
                                'Else
                                '    .FAVOURITE_TEAM_CODE = Convert.ToString(favouritesDDL.SelectedValue)
                                'End If

                                Dim riCount As Integer = 1
                                For Each ri As RepeaterItem In contactRepeater.Items
                                    Try
                                        Select Case riCount
                                            Case 1
                                                .MAIL_TEAM_CODE_1 = CType(ri.FindControl("teamDDL"), DropDownList).SelectedValue
                                            Case 2
                                                .MAIL_TEAM_CODE_2 = CType(ri.FindControl("teamDDL"), DropDownList).SelectedValue
                                            Case 3
                                                .MAIL_TEAM_CODE_3 = CType(ri.FindControl("teamDDL"), DropDownList).SelectedValue
                                            Case 4
                                                .MAIL_TEAM_CODE_4 = CType(ri.FindControl("teamDDL"), DropDownList).SelectedValue
                                            Case 5
                                                .MAIL_TEAM_CODE_5 = CType(ri.FindControl("teamDDL"), DropDownList).SelectedValue
                                        End Select
                                    Catch ex As Exception
                                    End Try
                                    riCount += 1
                                Next
                                .PREFERRED_CONTACT_METHOD = Convert.ToString(contactMethodDDL.SelectedValue)
                                '-------------------------------------------------------------------------------
                                'Populate User IDs from Text Boxes rather than Sessions as could be over written

                                If Session("Passport") IsNot Nothing AndAlso Not Session("Passport").Equals(String.Empty) Then
                                    .Passport = CType(Session("Passport"), String).ToUpper
                                End If
                                If Session("PIN") IsNot Nothing AndAlso Not Session("PIN").Equals(String.Empty) Then
                                    .PIN = CType(Session("PIN"), String).ToUpper
                                End If
                                If Session("Greencard") IsNot Nothing AndAlso Not Session("Greencard").Equals(String.Empty) Then
                                    .GreenCard = CType(Session("Greencard"), String).ToUpper
                                End If
                                If Not String.IsNullOrEmpty(txtPassport.Text) Then
                                    .Passport = txtPassport.Text.ToUpper
                                End If
                                If Not String.IsNullOrEmpty(txtPin.Text) Then
                                    .PIN = txtPin.Text.ToUpper
                                End If
                                If Not String.IsNullOrEmpty(txtGreenCard.Text) Then
                                    .GreenCard = txtGreenCard.Text.ToUpper
                                End If
                                If Not String.IsNullOrEmpty(txtUserID4.Text) Then
                                    .User_ID_4 = txtUserID4.Text.ToUpper
                                End If
                                If Not String.IsNullOrEmpty(txtUserID5.Text) Then
                                    .User_ID_5 = txtUserID5.Text.ToUpper
                                End If
                                If Not String.IsNullOrEmpty(txtUserID6.Text) Then
                                    .User_ID_6 = txtUserID6.Text.ToUpper
                                End If
                                If Not String.IsNullOrEmpty(txtUserID7.Text) Then
                                    .User_ID_7 = txtUserID7.Text.ToUpper
                                End If
                                If Not String.IsNullOrEmpty(txtUserID8.Text) Then
                                    .User_ID_8 = txtUserID8.Text.ToUpper
                                End If
                                If Not String.IsNullOrEmpty(txtUserID9.Text) Then
                                    .User_ID_9 = txtUserID9.Text.ToUpper
                                End If
                            End With

                            'Create and populate the userAddress
                            Dim userAddress As New TalentProfileAddress
                            With userAddress
                                .LoginID = loginid.Text
                                .Reference = reference.Text
                                .Type = type.Text
                                .Default_Address = True
                                If Not Session("AddressFormat") Is Nothing AndAlso Session("AddressFormat") = "1" Then
                                    .Address_Line_1 = String.Empty
                                    .Address_Line_2 = AddressFormat1Form1.ADF1_Street
                                    .Address_Line_3 = AddressFormat1Form1.ADF1_County
                                    .Address_Line_4 = AddressFormat1Form1.ADF1_City
                                    .Address_Line_5 = AddressFormat1Form1.ADF1_Town
                                    .Post_Code = UCase(AddressFormat1Form1.ADF1_Postcode)
                                    If ModuleDefaults.StoreCountryAsWholeName Then
                                        .Country = UCase(AddressFormat1Form1.ADF1_CountryText)
                                    Else
                                        .Country = UCase(AddressFormat1Form1.ADF1_Country)
                                    End If
                                Else
                                    .Address_Line_1 = building.Text
                                    .Address_Line_2 = street.Text
                                    .Address_Line_3 = town.Text
                                    .Address_Line_4 = city.Text
                                    .Address_Line_5 = county.Text
                                    .Post_Code = UCase(postcode.Text)
                                    If ModuleDefaults.StoreCountryAsWholeName Then
                                        .Country = UCase(country.SelectedItem.Text)
                                    Else
                                        .Country = country.SelectedValue
                                    End If
                                End If
                                If String.IsNullOrEmpty(sequence.Text) Then
                                    .Sequence = 0
                                Else
                                    .Sequence = CInt(sequence.Text)
                                End If
                                .Delivery_Zone_Code = ModuleDefaults.DefaultDeliveryZoneCode
                                DoAddresslessRegistration(userAddress)
                            End With
                            '-------------------------------
                            ' Set registered address details
                            '-------------------------------
                            Dim registeredCountry As String = String.Empty
                            Dim registeredAddress As New TalentProfileAddress
                            Dim registeredReference As String = String.Empty
                            If plhRegisteredAddress.Visible Then
                                If chkRegAddressSame.Checked Then
                                    txtRegAddress1.Text = userAddress.Address_Line_1
                                    txtRegAddress2.Text = userAddress.Address_Line_2
                                    txtRegAddress3.Text = userAddress.Address_Line_3
                                    txtRegAddress4.Text = userAddress.Address_Line_4
                                    txtRegAddress5.Text = userAddress.Address_Line_5
                                    txtRegPostcode.Text = userAddress.Post_Code
                                    registeredCountry = userAddress.Country
                                End If
                                registeredReference = "REGISTERED " & txtRegAddress1.Text.Trim & " " & txtRegAddress2.Text.Trim

                                If registeredCountry = String.Empty Then
                                    If ModuleDefaults.StoreCountryAsWholeName Then
                                        registeredCountry = UCase(ddlRegCountry.SelectedItem.Text)
                                    Else
                                        registeredCountry = ddlRegCountry.SelectedValue
                                    End If
                                End If
                                registeredAddress.Address_Line_1 = txtRegAddress1.Text
                                registeredAddress.Address_Line_2 = txtRegAddress2.Text
                                registeredAddress.Address_Line_3 = txtRegAddress3.Text
                                registeredAddress.Address_Line_4 = txtRegAddress4.Text
                                registeredAddress.Address_Line_5 = txtRegAddress5.Text
                                registeredAddress.Post_Code = txtRegPostcode.Text
                                registeredAddress.Country = registeredCountry
                                registeredAddress.Type = "1"
                                registeredAddress.Delivery_Zone_Code = ModuleDefaults.DefaultDeliveryZoneCode
                            End If

                            If SendDetailsToBackendFirst(CType(ModuleDefaults.SendRegistrationToBackendFirst, Boolean), userDetails, userAddress, registeredAddress) Then

                                'Create the association when in friends and family mode
                                If CreateAssociation() Then
                                    Dim passwordValue As String = password1.Text
                                    Dim hasedPassword As String = String.Empty
                                    Select Case ModuleDefaults.LoginidType
                                        Case Is = "1"
                                            If _customerNumber.Trim <> "" Then
                                                loginid.Text = _customerNumber.Trim
                                                userAddress.LoginID = _customerNumber.Trim
                                                userDetails.LoginID = _customerNumber.Trim
                                                userDetails.Account_No_1 = _customerNumber.Trim
                                            End If
                                        Case Is = "2"
                                            loginid.Text = userDetails.User_Number.Trim
                                            userAddress.LoginID = userDetails.User_Number.Trim
                                            userDetails.LoginID = userDetails.User_Number.Trim
                                            userDetails.Account_No_1 = userDetails.User_Number.Trim
                                            If ModuleDefaults.LoginLookupType = GlobalConstants.ECOMMERCE_TYPE Then
                                                _customerNumber = userDetails.User_Number.Trim
                                                If ModuleDefaults.UseEncryptedPassword Then
                                                    Dim passHash As New PasswordHash
                                                    hasedPassword = passHash.HashSalt(password1.Text.Trim, ModuleDefaults.SaltString)
                                                End If
                                            End If
                                    End Select


                                    'Default dummy password if empty (falls over if password is empty)
                                    If password1.Text = String.Empty Then
                                        password1.Text = " "
                                    End If

                                    'Create the user
                                    If hasedPassword.Length > 0 Then passwordValue = hasedPassword
                                    Membership.CreateUser(loginid.Text, passwordValue)

                                    'Create the profile
                                    Profile.Provider.CreateProfile(userDetails, userAddress)

                                    ' If Registered Address Panel is on then write an additional address
                                    If plhRegisteredAddress.Visible Then
                                        ' If checked, set the Registered address to the same as main address



                                        Dim addressData As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter
                                        addressData.AddAddress(TalentCache.GetPartner(Profile), _
                                                               userDetails.LoginID, _
                                                               "1", _
                                                               registeredReference, _
                                                               0, _
                                                               False, _
                                                               txtRegAddress1.Text, _
                                                               txtRegAddress2.Text, _
                                                               txtRegAddress3.Text, _
                                                               txtRegAddress4.Text, _
                                                               txtRegAddress5.Text, _
                                                               txtRegPostcode.Text, _
                                                               registeredCountry, _
                                                               String.Empty, _
                                                               String.Empty)

                                    End If

                                    ' We only want to add the friends and family customer to the database.
                                    ' We need to leave the current user as logged in
                                    If (Request.QueryString("source") Is Nothing OrElse Not Request.QueryString("source").Contains("fandf")) AndAlso Request.QueryString("source") <> "search" Then
                                        Membership.ValidateUser(loginid.Text, passwordValue)
                                        Profile.Initialize(loginid.Text, True)
                                        FormsAuthentication.SetAuthCookie(loginid.Text, False)
                                    End If

                                    'Save details to Backend
                                    If CType(ModuleDefaults.SendRegistrationToBackEnd, Boolean) And _
                                         Not CType(ModuleDefaults.SendRegistrationToBackendFirst, Boolean) Then
                                        SendDetailsToBackend("REGISTRATION", userDetails, userAddress, registeredAddress)
                                    End If

                                    ' If all OK then blank session variables to Greencard/pin/etc so they 
                                    ' are not used in any F&F registrations
                                    If Not Session("Passport") Is Nothing Then
                                        Session("Passport") = Nothing
                                    End If
                                    If Not Session("PIN") Is Nothing Then
                                        Session("PIN") = Nothing
                                    End If
                                    If Not Session("Greencard") Is Nothing Then
                                        Session("Greencard") = Nothing
                                    End If
                                    If cbIsConfirmationEmailSend.Checked = True Then
                                        SendConfirmationEmail()
                                    End If
                                    ' Shouldn't login here if F&F
                                    If (Request.QueryString("source") Is Nothing OrElse Not Request.QueryString("source").Contains("fandf")) AndAlso Request.QueryString("source") <> "search" Then
                                        Talent.eCommerce.Utilities.loginUser(loginid.Text, passwordValue)
                                        TEBUtilities.addCustomerLoggedInToSession(ModuleDefaults.NoOfRecentlyUsedCustomers, loginid.Text, forename.Text.Trim(), surname.Text.Trim())
                                    End If

                                End If
                            End If

                        Catch ex As Exception
                            Exit Sub
                        End Try
                        'End Using

                        'Only move on when no error exists
                        If ErrorLabel.Text.Trim = "" Then
                            If Me.plhAddMembership.Visible AndAlso Me.addMembershipCheck.Checked Then
                                If String.IsNullOrEmpty(ucr.Attribute("AddAutoMembership_ReturnUrl")) Then
                                    Response.Redirect("~/Redirect/TicketingGateway.aspx?page=Registration.aspx&function=AddFreeMembership")
                                Else
                                    Dim url As String = ucr.Attribute("AddAutoMembership_ReturnUrl")
                                    If url.Contains("?") Then
                                        url += "&"
                                    Else
                                        url += "?"
                                    End If
                                    url += "customer=" & _customerNumber
                                    Response.Redirect("~/Redirect/TicketingGateway.aspx?page=Registration.aspx&function=AddFreeMembership&ReturnUrl=" & Server.UrlEncode(url))
                                End If
                            Else
                                If Request.QueryString("source") = "fandf" Then
                                    If String.IsNullOrEmpty(ucr.Attribute("FandFRegistration_ReturnUrl")) Then
                                        Response.Redirect("~/PagesLogin/FriendsAndFamily/FriendsAndFamily.aspx")
                                    Else
                                        Dim url As String = ucr.Attribute("FandFRegistration_ReturnUrl")
                                        If url.Contains("?") Then
                                            url += "&"
                                        Else
                                            url += "?"
                                        End If
                                        url += "customer=" & _customerNumber
                                        Response.Redirect(url)
                                    End If
                                ElseIf Request.QueryString("source") = "fandfhospitality" Then
                                    Dim redirectUrl As String = Server.UrlDecode(Request.QueryString("returnurl"))
                                    Response.Redirect(redirectUrl)
                                ElseIf Request.QueryString("source") = "fandfbasket" Then
                                    If String.IsNullOrEmpty(ucr.Attribute("FandFBasketRegistration_ReturnUrl")) Then
                                        Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                                    Else
                                        Response.Redirect(ucr.Attribute("FandFBasketRegistration_ReturnUrl"))
                                    End If
                                ElseIf Request.QueryString("source") = "companycontacts" Then
                                    If Session("SelectedCompanyNumber") IsNot Nothing Then
                                        AddContactToCompany(_customerNumber, Session("SelectedCompanyNumber"))
                                        Response.Redirect(Request.QueryString("ReturnUrl"))
                                    End If
                                ElseIf Request.QueryString("source") = "search" Then
                                    'Registration Via Search - update the BasketCustomerSearch session and redirect to basket
                                    Dim dtBasketCustomerSearch As New DataTable

                                    'Check if data table exists first, and if not create it
                                    If Session("BasketCustomerSearch") Is Nothing Then
                                        dtBasketCustomerSearch = CreateBasketCustomerSearchDataTable()
                                    Else
                                        dtBasketCustomerSearch = CType(Session("BasketCustomerSearch"), DataTable)
                                    End If

                                    'Update or add the data in the table
                                    If dtBasketCustomerSearch.Rows.Count > 0 Then
                                        Dim i As Integer
                                        For i = 0 To dtBasketCustomerSearch.Rows.Count - 1
                                            If dtBasketCustomerSearch.Rows(i)("CustomerNumber").ToString = "999999999999" Then
                                                dtBasketCustomerSearch.Rows(i)("CustomerNumber") = Profile.User.Details.LoginID
                                                dtBasketCustomerSearch.Rows(i)("FoundCustomerNumber") = loginid.Text
                                                dtBasketCustomerSearch.Rows(i)("Forename") = forename.Text
                                                dtBasketCustomerSearch.Rows(i)("Surname") = surname.Text
                                            End If
                                        Next
                                    Else
                                        Dim newRow As DataRow = dtBasketCustomerSearch.NewRow
                                        newRow("CustomerNumber") = Profile.User.Details.LoginID
                                        newRow("FoundCustomerNumber") = loginid.Text
                                        newRow("Forename") = forename.Text
                                        newRow("Surname") = surname.Text
                                    End If

                                    'Add updated table to session
                                    Session("BasketCustomerSearch") = dtBasketCustomerSearch
                                    Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                                Else
                                    If Request.QueryString("ReturnUrl") Is Nothing Then
                                        If Talent.eCommerce.Utilities.IsAgent Then
                                            Response.Redirect(Talent.eCommerce.Utilities.GetSiteHomePage())
                                        Else
                                            Response.Redirect("~/PagesLogin/Profile/RegistrationConfirmation.aspx")
                                        End If
                                    Else
                                        Response.Redirect(Request.QueryString("ReturnUrl"))
                                    End If
                                End If
                            End If

                        End If

                    Else
                        ErrorLabel.Text = ucr.Content("UserAlreadyExistsErrorText", _languageCode, True)
                        registerBtn.Enabled = True
                    End If
                Else
                    ErrorLabel.Text = ucr.Content("TandCNotTickedErrorText", _languageCode, True)
                    registerBtn.Enabled = True
                End If
            End If
        End If

    End Sub

    Protected Sub SendDetailsToBackend(ByVal call_origin As String, _
                                    ByVal userDetails As TalentProfileUserDetails, _
                                    ByVal userAddress As TalentProfileAddress, _
                                     ByVal registeredAddress As TalentProfileAddress)

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
        If CType(def.SendRegistrationToBackendFirst, Boolean) Then
            p.Details = userDetails
            With p.Details
                If mailOption.Checked = mailOptionTrueValue Then
                    .Bit0 = mailOptionTrueValue
                Else
                    .Bit0 = Not mailOptionTrueValue
                End If
                If opt1.Checked = opt1TrueValue Then
                    .Bit1 = opt1TrueValue
                Else
                    .Bit1 = Not opt1TrueValue
                End If
                If opt2.Checked = opt2TrueValue Then
                    .Bit2 = opt2TrueValue
                Else
                    .Bit2 = Not opt2TrueValue
                End If
                If opt3.Checked = opt3TrueValue Then
                    .Bit3 = opt3TrueValue
                Else
                    .Bit3 = Not opt3TrueValue
                End If
                If opt4.Checked = opt4TrueValue Then
                    .Bit4 = opt4TrueValue
                Else
                    .Bit4 = Not opt4TrueValue
                End If
                If opt5.Checked = opt5TrueValue Then
                    .Bit5 = opt5TrueValue
                Else
                    .Bit5 = Not opt5TrueValue
                End If

            End With
            address = userAddress
        Else
            p = Profile.Provider.GetUserByLoginID(email.Text)
            address = ProfileHelper.ProfileAddressEnumerator(0, p.Addresses)
        End If

        '
        ' Set the Customer object and invoke the SetCustomer method to post details to TALENT Customer Manager
        '
        With myCustomer

            ' Reset the settings entity to a customer specific settings entity 
            .Settings = CType(New DECustomerSettings, DESettings)
            .DeV11 = deCustV11

            With deCustV1
                If Talent.eCommerce.Utilities.IsAgent Then
                    .Agent = Session("Agent").ToString
                End If
                .Password = moduleDefaults.GetDefaults.EncryptedPasswordPlaceholder
                .CompanyNumber = TEBUtilities.CheckForDBNull_String(Session("SelectedCompanyNumber"))

                'Encrypt the password if defaults are on.
                If moduleDefaults.GetDefaults.UseEncryptedPassword = True Then
                    '.useEncryptedPassword = "Y"
                    .UseEncryptedPassword = True
                    If Not password1.Text.Trim Is String.Empty Then
                        Dim passHash As New Talent.Common.PasswordHash
                        .NewHashedPassword = passHash.HashSalt(password1.Text.Trim, moduleDefaults.GetDefaults.SaltString)
                    End If
                    .SaltString = moduleDefaults.GetDefaults.SaltString
                End If
                .Action = ""
                '    .ThirdPartyContactRef = ucr.Content("crmExternalKeyName", _languageCode, True)
                .ThirdPartyContactRef = p.Details.User_Number
                .ThirdPartyCompanyRef1 = p.Details.User_Number
                .ThirdPartyCompanyRef1Supplement = p.Details.User_Number_Prefix
                .ThirdPartyCompanyRef2 = ""
                .DateFormat = "1"

                .ContactSurname = p.Details.Surname
                .ContactForename = p.Details.Forename
                .Suffix = p.Details.CUSTOMER_SUFFIX
                .Nickname = p.Details.NICKNAME
                .ContactSLAccount = p.Details.Account_No_2
                .AltUserName = p.Details.USERNAME
                .ContactTitle = p.Details.Title
                .ContactInitials = p.Details.Initials
                .Salutation = p.Details.Salutation
                .DateBirth = dobYear.Text.ToString.PadLeft(2, "0") & _
                                dobMonth.Text.ToString.PadLeft(2, "0") & _
                                dobDay.Text.PadLeft(2, "0")

                .Gender = sex.SelectedValue
                .EmailAddress = p.Details.Email
                .SLNumber1 = p.Details.Account_No_1
                .SLNumber2 = p.Details.Account_No_2

                .MothersName = mothersname.Text
                .ProcessMothersName = "1"
                .FathersName = fathersname.Text
                .ParentEmail = parentemail.Text
                .ParentPhone = parentphone.Text
                .ConsentStatus = String.Empty
                If DateTime.Compare(p.Details.DOB, _ParentalConsentCeilingDob) > 0 Then
                    If cbParentalPermissionGranted.Checked = True Then
                        .ConsentStatus = "G"
                    Else
                        .ConsentStatus = "R"
                    End If
                End If

                ' Fields added for agents
                .StopCode = ddlStopcode.SelectedValue
                .PriceBand = ddlpriceband.SelectedValue
                .ContactbyEmail = cbContactbyEmail.Checked
                .ContactbyTelephoneHome = cbContactbyTelephoneHome.Checked
                .ContactbyTelephoneWork = cbContactbyTelephoneWork.Checked
                .ContactbyMobile = cbContactbyMobile.Checked
                .ContactbyPost = cbContactbyPost.Checked

                .ProcessFathersName = "1"

                If plhSalutationRow.Visible Then
                    .ProcessSalutation = "1"
                Else
                    .ProcessSalutation = "0"
                End If

                .ProcessContactSurname = "1"
                .ProcessContactForename = "1"
                .ProcessContactTitle = "1"
                .ProcessContactInitials = "1"
                .ProcessDateBirth = "1"
                .ProcessEmailAddress = "1"
                .ProcessSLNumber1 = "1"
                .ProcessSLNumber2 = "1"
                .Language = Talent.eCommerce.Utilities.GetCurrentLanguage()
                '------------------------------------------------------------------
                ' Set it to the Registration Type branch if set, otherwise
                '  pick up branch from Partner or if blank then from module defaults
                '------------------------------------------------------------------
                If _crmBranch = String.Empty Then
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
                Else
                    .BranchCode = _crmBranch
                End If



                ' If QAS is used then Address Line is never populated and therefore Address1 for CRM 
                ' needs to be populated from different source fields.
                Dim eComDefs As New ECommerceModuleDefaults
                Dim defs As ECommerceModuleDefaults.DefaultValues = eComDefs.GetDefaults
                ' If QAS is switched on then do not show AddressLine1Row (building)
                If Not defs.AddressLine1RowVisible Then
                    .AddressLine1 = address.Address_Line_2.Trim
                Else
                    .AddressLine1 = Trim(address.Address_Line_1.Trim & " " & address.Address_Line_2.Trim)
                End If


                If Not ucr.Content("DefaultAddressStringForAddresslessRegistration", _languageCode, True).Trim = "" Then
                    If .AddressLine1.Trim = "" Then
                        .AddressLine1 = ucr.Content("DefaultAddressStringForAddresslessRegistration", _languageCode, True)
                    End If
                End If

                .AddressLine2 = address.Address_Line_3.Trim
                .AddressLine3 = address.Address_Line_4.Trim
                .AddressLine4 = address.Address_Line_5.Trim
                .AddressLine5 = UCase(address.Country.Trim)
                .PostCode = UCase(address.Post_Code)
                .HomeTelephoneNumber = p.Details.Telephone_Number
                .WorkTelephoneNumber = p.Details.Work_Number
                .MobileNumber = p.Details.Mobile_Number
                If plhCompanyRow.Visible Then
                    .CompanyName = p.Details.CompanyName
                    .UpdateCompanyInformation = "Y"
                Else
                    .CompanyName = .AddressLine1
                End If
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

                '------------------------------------------
                ' Set registeredAddress for call to backend
                '------------------------------------------
                If plhRegisteredAddress.Visible Then
                    If Not defs.AddressLine1RowVisible Then
                        .RegisteredAddress1 = registeredAddress.Address_Line_2.Trim
                    Else
                        .RegisteredAddress1 = Trim(registeredAddress.Address_Line_1.Trim & " " & registeredAddress.Address_Line_2.Trim)
                    End If
                    .RegisteredAddress2 = registeredAddress.Address_Line_3
                    .RegisteredAddress3 = registeredAddress.Address_Line_4
                    .RegisteredAddress4 = registeredAddress.Address_Line_5
                    .RegisteredAddress5 = UCase(registeredAddress.Country)
                    .RegisteredPostcode = UCase(registeredAddress.Post_Code)
                End If
                If mailOption.Checked = mailOptionTrueValue Then
                    .ContactViaMail = "1"
                Else
                    .ContactViaMail = "0"
                End If
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

                .ProcessContactViaEmail = "1"
                .ProcessContactViaMail1 = "1"
                .ProcessContactViaMail2 = "1"
                .ProcessContactViaMail3 = "1"
                .ProcessContactViaMail4 = "1"
                .ProcessContactViaMail5 = "1"

                'If pnlRegisteredAddress.Visible Then
                '    .RegisteredAddress1 = 
                'End If

                ' /001 Process Newsletter Prefs attributes
                'If plhNewsletter.Visible Then
                '    If Newsletter.Checked Then
                '        .Attribute01 = ucr.Attribute("newsletterAttribute")
                '        .Attribute01Action = "A"
                '        If NewsletterChoice.SelectedValue = ucr.Content("HTMLNewsletterText", _languageCode, True) Then
                '            .Attribute02 = ucr.Attribute("newsletterAttributeTypeHTML")
                '            .Attribute02Action = "A"
                '            .Attribute03 = ucr.Attribute("newsletterAttributeTypePlain")
                '            .Attribute03Action = "D"
                '        ElseIf NewsletterChoice.SelectedValue = ucr.Content("PlainTextNewsletterText", _languageCode, True) Then
                '            .Attribute02 = ucr.Attribute("newsletterAttributeTypePlain")
                '            .Attribute02Action = "A"
                '            .Attribute03 = ucr.Attribute("newsletterAttributeTypeHTML")
                '            .Attribute03Action = "D"
                '        End If
                '    Else
                '        .Attribute01 = ucr.Attribute("newsletterAttribute")
                '        .Attribute01Action = "D"
                '        .Attribute02 = ucr.Attribute("newsletterAttributeTypeHTML")
                '        .Attribute02Action = "D"
                '        .Attribute03 = ucr.Attribute("newsletterAttributeTypePlain")
                '        .Attribute03Action = "D"
                '    End If
                'End If
                .ProcessAttributes = "1"

                If plhNewsletter.Visible Then
                    If (Newsletter.Checked And CType(ucr.Attribute("newsletterCheckedMeansTrue"), Boolean)) Or _
                        (Not Newsletter.Checked And Not CType(ucr.Attribute("newsletterCheckedMeansTrue"), Boolean)) Then
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
                End If

                If Subscribe2.Visible Then
                    If (Subscribe2.Checked And CType(ucr.Attribute("subscribe2CheckedMeansTrue"), Boolean)) Or _
                        (Not Subscribe2.Checked And Not CType(ucr.Attribute("subscribe2CheckedMeansTrue"), Boolean)) Then
                        .Attribute04 = ucr.Attribute("subscribe2Attribute")
                        .Attribute04Action = "A"
                    Else
                        .Attribute04 = ucr.Attribute("subscribe2Attribute")
                        .Attribute04Action = "D"
                    End If
                End If
                If Subscribe3.Visible Then
                    If (Subscribe3.Checked And CType(ucr.Attribute("subscribe3CheckedMeansTrue"), Boolean)) Or _
                        (Not Subscribe3.Checked And Not CType(ucr.Attribute("subscribe3CheckedMeansTrue"), Boolean)) Then
                        .Attribute05 = ucr.Attribute("subscribe3Attribute")
                        .Attribute05Action = "A"
                    Else
                        .Attribute05 = ucr.Attribute("subscribe3Attribute")
                        .Attribute05Action = "D"
                    End If
                End If

                .ProcessSubscription1 = "1"
                .ProcessSubscription2 = "1"
                .ProcessSubscription3 = "1"


                .SUPPORTER_CLUB_CODE = p.Details.SUPPORTER_CLUB_CODE
                .FAVOURITE_TEAM_CODE = p.Details.FAVOURITE_TEAM_CODE
                .FAVOURITE_SPORT = p.Details.FAVOURITE_SPORT
                .MAIL_TEAM_CODE_1 = p.Details.MAIL_TEAM_CODE_1
                .MAIL_TEAM_CODE_2 = p.Details.MAIL_TEAM_CODE_2
                .MAIL_TEAM_CODE_3 = p.Details.MAIL_TEAM_CODE_3
                .MAIL_TEAM_CODE_4 = p.Details.MAIL_TEAM_CODE_4
                .MAIL_TEAM_CODE_5 = p.Details.MAIL_TEAM_CODE_5
                .PREFERRED_CONTACT_METHOD = p.Details.PREFERRED_CONTACT_METHOD

                .ProcessSupporterClubCode = "1"
                .ProcessFavouriteTeamCode = "1"
                .ProcessFavouriteSport = "1"
                .ProcessMailTeamCode1 = "1"
                .ProcessMailTeamCode2 = "1"
                .ProcessMailTeamCode3 = "1"
                .ProcessMailTeamCode4 = "1"
                .ProcessMailTeamCode5 = "1"
                .ProcessPreferredContactMethod = "1"

                .ProcessSupporterClubCode = "1"

                .PhoneNoFormatting = def.PhoneNoFormat
                .PostCodeFormatting = def.PostCodeFormat

                'Stop Code for Passport ID
                If def.StopCodeForID3.Length > 0 Then
                    .stopcodeforid3 = def.StopCodeForID3
                    ddlStopcode.SelectedValue = def.StopCodeForID3
                End If
                '-------------------------------------------------------------------------------
                'Populate User IDs from Text Boxes rather than Sessions as could be over written

                'If Session("Passport") IsNot Nothing AndAlso Not Session("Passport").Equals(String.Empty) Then
                '    .PassportNumber = CType(Session("Passport"), String).ToUpper
                '    .ProcessPassportNumber = "1"
                'End If
                'If Session("PIN") IsNot Nothing AndAlso Not Session("PIN").Equals(String.Empty) Then
                '    .PIN_Number = CType(Session("PIN"), String).ToUpper
                '    .ProcessPinNumber = "1"
                'End If
                'If Session("Greencard") IsNot Nothing AndAlso Not Session("Greencard").Equals(String.Empty) Then
                '    .GreenCardNumber = CType(Session("Greencard"), String).ToUpper
                '    .ProcessGreenCardNumber = "1"
                'End If
                If Not String.IsNullOrEmpty(txtPassport.Text) Then
                    .PassportNumber = txtPassport.Text.ToUpper
                    .ProcessPassportNumber = "1"
                End If
                If Not String.IsNullOrEmpty(txtPin.Text) Then
                    .PIN_Number = txtPin.Text.ToUpper
                    .ProcessPinNumber = "1"
                End If
                If Not String.IsNullOrEmpty(txtGreenCard.Text) Then
                    .GreenCardNumber = txtGreenCard.Text.ToUpper
                    .ProcessGreenCardNumber = "1"
                End If
                If Not String.IsNullOrEmpty(txtUserID4.Text) Then
                    .User_ID_4 = txtUserID4.Text.ToUpper
                End If
                If Not String.IsNullOrEmpty(txtUserID5.Text) Then
                    .User_ID_5 = txtUserID5.Text.ToUpper
                End If
                If Not String.IsNullOrEmpty(txtUserID6.Text) Then
                    .User_ID_6 = txtUserID6.Text.ToUpper
                End If
                If Not String.IsNullOrEmpty(txtUserID7.Text) Then
                    .User_ID_7 = txtUserID7.Text.ToUpper
                End If
                If Not String.IsNullOrEmpty(txtUserID8.Text) Then
                    .User_ID_8 = txtUserID8.Text.ToUpper
                End If
                If Not String.IsNullOrEmpty(txtUserID9.Text) Then
                    .User_ID_9 = txtUserID9.Text.ToUpper
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
                    Try
                        Logging.WriteLog(Me.loginid.Text, _
                                        myErrorObj.ErrorNumber, _
                                        "Registration Error - " & myErrorObj.ErrorMessage, _
                                        myErrorObj.ErrorStatus, _
                                        TalentCache.GetBusinessUnit, _
                                        TalentCache.GetPartner(Profile))
                    Catch ex As Exception

                    End Try
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
                    'ErrorLabel.Text = Talent.Common.Utilities.getErrorDescription(ucr, _languageCode, ("XX"), True)
                    ErrorLabel.Text = getError("XX")
                Else
                    'Has an error has occurred
                    If myCustomer.DeV11.DECustomersV1(0).ErrorCode.Trim <> "" Then
                        'ErrorLabel.Text = Talent.Common.Utilities.getErrorDescription(ucr, _languageCode, myCustomer.De.ErrorCode, True)
                        ErrorLabel.Text = getError(myCustomer.DeV11.DECustomersV1(0).ErrorCode)
                    Else
                        'Set the returned customer number
                        _customerNumber = myCustomer.DeV11.DECustomersV1(0).CustomerNumber.Trim
                    End If
                End If
            End If
        End With
    End Sub

    Protected Function SendDetailsToBackendFirst(ByVal sendRegistrationToBackEndFirst As Boolean, _
                                            ByVal userDetails As TalentProfileUserDetails, _
                                            ByVal userAddress As TalentProfileAddress, _
                                             ByVal registeredAddress As TalentProfileAddress) As Boolean

        Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

        ' Are we sending the customer information to the back end first
        If Not CType(sendRegistrationToBackEndFirst, Boolean) Or _
            Not CType(def.SendRegistrationToBackEnd, Boolean) Then
            Return True
        End If

        'Send the information to the backend 
        SendDetailsToBackend("REGISTRATION", userDetails, userAddress, registeredAddress)

        'Was the call a success?
        If ErrorLabel.Text.Trim = "" Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function CreateAssociation() As Boolean
        'Are we in friends and family mode
        If Request.QueryString("source") Is Nothing OrElse Not Request.QueryString("source").Contains("fandf") Then
            Return True
        End If

        'Add the friends and family
        Dim myCustomer As New TalentCustomer
        Dim deCustV11 As New DECustomerV11
        Dim deCustV1 As New DECustomer
        deCustV11.DECustomersV1.Add(deCustV1)
        Dim err As New ErrorObj
        With myCustomer
            .DeV11 = deCustV11
            ' Set the settings data entity. 
            .Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .Settings.BusinessUnit = TalentCache.GetBusinessUnit()
            .Settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()

            'Set the customer values
            deCustV1.FriendsAndFamilyId = _customerNumber
            deCustV1.CustomerNumber = Profile.User.Details.LoginID.ToString
            deCustV1.FriendsAndFamilyMode = "A"
            deCustV1.Source = GlobalConstants.SOURCE
            .ResultDataSet = Nothing
            If AgentProfile.IsAgent Then deCustV1.IncludeBoxOfficeLinks = True

            'Process
            err = .AddCustomerAssociation
        End With

        'Did the call complete successfully
        If err.HasError Then
            ErrorLabel.Text = getError("XX")
        Else
            'API error
            If myCustomer.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                ErrorLabel.Text = getError(myCustomer.ResultDataSet.Tables(0).Rows(0).Item("ReturnCode"))
            End If
        End If

        'Was the call successful
        If ErrorLabel.Text.Trim = "" Then
            myCustomer.CustomerAssociationsClearSession()
            Return True
        Else
            Return False
        End If
    End Function

    Protected Sub SendConfirmationEmail()
        If Me.Display Then
            Dim defs As New ECommerceModuleDefaults
            Dim defaults As ECommerceModuleDefaults.DefaultValues = defs.GetDefaults

            Dim talEmail As New Talent.Common.TalentEmail
            Dim xmlDoc As String = talEmail.CreateCustomerRegistrationXmlDocument(defaults.RegistrationConfirmationFromEmail, _
                                                                                email.Text, _
                                                                                ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim, _
                                                                                Talent.eCommerce.Utilities.GetSMTPPortNumber, _
                                                                                ucr.PartnerCode, _
                                                                                _customerNumber, _
                                                                                Talent.eCommerce.Utilities.GetCurrentApplicationUrl & "/PagesPublic/Home/Home.aspx")

            'Create the email request in the offline processing table
            TDataObjects.AppVariableSettings.TblOfflineProcessing.Insert(TalentCache.GetBusinessUnit(), "*ALL", "Pending", 0, "", _
                                                                         "EmailMonitor", "CustomerRegistration", xmlDoc, "")

        End If
    End Sub

    Protected Sub SetAddressVisibility()
        If Me.Display Then

            plhAddressLine1Row.Visible = True
            buildingRegEx.Enabled = True
            plhRegAddressLine1Row.Visible = True
            rfvTxtRegAddress1.Enabled = True
            regexTxtRegAddress1.Enabled = True

            plhAddressLine2Row.Visible = True
            streetRegEx.Enabled = True
            plhRegAddressLine2Row.Visible = True
            rfvTxtRegAddress2.Enabled = True
            regexTxtRegAddress2.Enabled = True

            plhAddressLine3Row.Visible = True
            townRegEx.Enabled = True
            plhRegAddressLine3Row.Visible = True
            regexTxtRegAddress3.Enabled = True

            plhAddressLine4Row.Visible = True
            cityRegEx.Enabled = True
            RegAddressLine4Row.Visible = True
            regexTxtRegAddress4.Enabled = True

            plhAddressLine5Row.Visible = True
            countyRegEx.Enabled = True
            plhRegAddressLine5Row.Visible = True
            regexTxtRegAddress5.Enabled = True

            plhAddressPostcodeRow.Visible = True
            plhRegAddressPostcodeRow.Visible = True
            regexTxtRegAddress5.Enabled = True
            postcodeRegEx.Enabled = True
            plhAddressCountryRow.Visible = True
            countryRegEx.Enabled = True
            plhRegAddressCountryRow.Visible = True

            If Not addressTypeRowVisible Then
                plhAddressTypeRow.Visible = False
                typeRegEx.Enabled = False
            End If
            If Not addressReferenceRowVisible Then
                plhAddressReferenceRow.Visible = False
                referenceRegEx.Enabled = False
            End If
            If Not addressSequenceRowVisible Then
                AddressSequenceRow.Visible = False
                sequenceRegEx.Enabled = False
            End If
            If Not addressLine1RowVisible Then
                plhAddressLine1Row.Visible = False
                buildingRFV.Enabled = False
                buildingRegEx.Enabled = False
                plhRegAddressLine1Row.Visible = False
                rfvTxtRegAddress1.Enabled = False
                regexTxtRegAddress1.Enabled = False
            End If
            If Not addressLine2RowVisible Then
                plhAddressLine2Row.Visible = False
                streetRFV.Enabled = False
                streetRegEx.Enabled = False
                plhRegAddressLine2Row.Visible = False
                rfvTxtRegAddress2.Enabled = False
                regexTxtRegAddress2.Enabled = False
            End If
            If Not addressLine3RowVisible Then
                plhAddressLine3Row.Visible = False
                townRegEx.Enabled = False
                plhRegAddressLine3Row.Visible = False
                regexTxtRegAddress3.Enabled = False
            End If
            If Not addressLine4RowVisible Then
                plhAddressLine4Row.Visible = False
                cityRegEx.Enabled = False
                RegAddressLine4Row.Visible = False
                regexTxtRegAddress4.Enabled = False
            End If
            If Not addressLine5RowVisible Then
                plhAddressLine5Row.Visible = False
                countyRFV.Enabled = False
                countyRegEx.Enabled = False
                plhRegAddressLine5Row.Visible = False
                regexTxtRegAddress5.Enabled = False
            End If
            If Not mothersnameRowVisible Then
                plhMothersNameRow.Visible = False
                mothersname.Enabled = False
            End If
            If Not fathersnameRowVisible Then
                plhFathersNameRow.Visible = False
                fathersname.Enabled = False
            End If
            If Not addressPostcodeRowVisible Then
                plhAddressPostcodeRow.Visible = False
                postcodeRFV.Enabled = False
                plhRegAddressPostcodeRow.Visible = False
                postcodeRFV.Enabled = False
                regexTxtRegAddress5.Enabled = False
                postcodeRegEx.Enabled = False
            End If
            If Not addressCountryRowVisible Then
                plhAddressCountryRow.Visible = False
                countryRegEx.Enabled = False
                plhRegAddressCountryRow.Visible = False
            End If
            If Not ucr.Attribute("addressTitleRowVisible").Trim = "" Then
                If Not CType(ucr.Attribute("addressTitleRowVisible"), Boolean) Then
                    plhAddressTitleRow.Visible = False
                End If
            End If
            If Not ucr.Attribute("addressingOnOff").Trim = "" Then
                If Not CType(ucr.Attribute("addressingOnOff"), Boolean) Then
                    plhFindAddressButtonRow.Visible = False
                End If
            End If
        End If


    End Sub

    Protected Sub SetAddressVisibilityProperties()
        If Me.Display Then
            ' Set common address field visibility using system defaults, and then override by any control-level defaults.
            If Not ModuleDefaults.AddressLine1RowVisible Then
                addressLine1RowVisible = False
            End If
            If Not ModuleDefaults.AddressLine2RowVisible Then
                addressLine2RowVisible = False
            End If
            If Not ModuleDefaults.AddressLine3RowVisible Then
                addressLine3RowVisible = False
            End If
            If Not ModuleDefaults.AddressLine4RowVisible Then
                addressLine4RowVisible = False
            End If
            If Not ModuleDefaults.AddressLine5RowVisible Then
                addressLine5RowVisible = False
            End If
            If Not ModuleDefaults.AddressPostcodeRowVisible Then
                addressPostcodeRowVisible = False
            End If
            If Not ModuleDefaults.AddressCountryRowVisible Then
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
            If Not ucr.Attribute("mothersnameRowVisible").Trim = "" Then
                If Not CType(ucr.Attribute("mothersnameRowVisible"), Boolean) Then
                    mothersnameRowVisible = False
                Else
                    mothersnameRowVisible = True
                End If
            End If
            If Not ucr.Attribute("fathersnameRowVisible").Trim = "" Then
                If Not CType(ucr.Attribute("fathersnameRowVisible"), Boolean) Then
                    fathersnameRowVisible = False
                Else
                    fathersnameRowVisible = True
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
            If Not CType(ucr.Attribute("addressTypeRowVisible"), Boolean) Then
                addressTypeRowVisible = False
            End If
            If Not CType(ucr.Attribute("addressReferenceRowVisible"), Boolean) Then
                addressReferenceRowVisible = False
            End If
            If Not CType(ucr.Attribute("addressSequenceRowVisible"), Boolean) Then
                addressSequenceRowVisible = False
            End If
            If Not CType(ucr.Attribute("addressingOnOff"), Boolean) Then
                plhFindAddressButtonRow.Visible = False
            End If
            If Not CType(ucr.Attribute("addressTitleRowVisible"), Boolean) Then
                plhAddressTitleRow.Visible = False
            End If

            ' override if the address format comes from a user control.
            If ModuleDefaults.AddressFormat = "1" AndAlso (Session("AddressFormat") Is Nothing OrElse Session("AddressFormat") = "1") Then
                Session("AddressFormat") = "1"
                addressLine1RowVisible = False
                addressLine2RowVisible = False
                addressLine3RowVisible = False
                addressLine4RowVisible = False
                addressLine5RowVisible = False
                addressCountryRowVisible = False
                addressPostcodeRowVisible = False
                plhAddressTitleRow.Visible = False
                buildingRFV.Enabled = False
                streetRFV.Enabled = False
                cityRFV.Enabled = False
                countyRFV.Enabled = False
                postcodeRFV.Enabled = False
                AddressFormat1Form1.Visible = True
            Else
                If ModuleDefaults.AddressFormat = "1" Then
                    country.AutoPostBack = True
                    If Not Session("AddressFormatCountry") Is Nothing Then
                        country.SelectedValue = Session("AddressFormatCountry")
                        Session("AddressFormatCountry") = Nothing
                    End If
                End If
                AddressFormat1Form1.Visible = False
            End If
        End If
    End Sub

    Protected Function GetAddressingLinkText() As String
        Return ucr.Content("addressingLinkButtonText", _languageCode, True)
    End Function

    Protected Sub CreateAddressingJavascript()
        If Me.Display Then
            If CType(ucr.Attribute("addressingOnOff"), Boolean) Then
                Dim sString As String = String.Empty
                Dim sAllFields() As String = ModuleDefaults.AddressingFields.ToString.Split(",")
                Dim count As Integer = 0

                Response.Write(vbCrLf & "<script language=""javascript"" type=""text/javascript"">" & vbCrLf)
                Select Case ModuleDefaults.AddressingProvider.ToUpper
                    Case Is = "QAS", "QASONDEMAND"
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

                ' Create function to populate the address fields.  This function is called from FlatAddress.aspx.
                Response.Write("function UpdateAddressFields() {" & vbCrLf)

                ' Create local function variables used to indicate whether an address element has already been used.
                Do While count < sAllFields.Length
                    Response.Write("var usedHiddenAdr" & count.ToString & " = '';" & vbCrLf)
                    count = count + 1
                Loop

                ' Clear all address fields
                If addressTypeRowVisible Then Response.Write("document.forms[0]." & type.UniqueID & ".value = '';" & vbCrLf)
                If addressReferenceRowVisible Then Response.Write("document.forms[0]." & reference.UniqueID & ".value = '';" & vbCrLf)
                If addressSequenceRowVisible Then Response.Write("document.forms[0]." & sequence.UniqueID & ".value = '';" & vbCrLf)
                If plhCompanyRow.Visible Then Response.Write("document.forms[0]." & txtCompanyName.UniqueID & ".value = '';" & vbCrLf)
                If addressLine1RowVisible Then Response.Write("document.forms[0]." & building.UniqueID & ".value = '';" & vbCrLf)
                If addressLine2RowVisible Then Response.Write("document.forms[0]." & street.UniqueID & ".value = '';" & vbCrLf)
                If addressLine3RowVisible Then Response.Write("document.forms[0]." & town.UniqueID & ".value = '';" & vbCrLf)
                If addressLine4RowVisible Then Response.Write("document.forms[0]." & city.UniqueID & ".value = '';" & vbCrLf)
                If addressLine5RowVisible Then Response.Write("document.forms[0]." & county.UniqueID & ".value = '';" & vbCrLf)
                If addressPostcodeRowVisible Then Response.Write("document.forms[0]." & postcode.UniqueID & ".value = '';" & vbCrLf)
                If addressCountryRowVisible Then Response.Write("document.forms[0]." & country.UniqueID & ".value = '';" & vbCrLf)

                ' If an address field is in use and is defined to contain a QAS address element then create Javascript code to populate correctly.
                If addressLine1RowVisible And Not ModuleDefaults.AddressingMapAdr1.Trim = "" Then
                    sString = GetJavascriptString("document.forms[0]." & building.UniqueID, ModuleDefaults.AddressingMapAdr1, ModuleDefaults.AddressingFields)
                    Response.Write(sString)
                End If
                If plhCompanyRow.Visible And Not ModuleDefaults.AddressingMapOrganisation.Trim = "" Then
                    sString = GetJavascriptString("document.forms[0]." & txtCompanyName.UniqueID, ModuleDefaults.AddressingMapOrganisation, ModuleDefaults.AddressingFields)
                    Response.Write(sString)
                End If
                If addressLine2RowVisible And Not ModuleDefaults.AddressingMapAdr2.Trim = "" Then
                    sString = GetJavascriptString("document.forms[0]." & street.UniqueID, ModuleDefaults.AddressingMapAdr2, ModuleDefaults.AddressingFields)
                    Response.Write(sString)
                End If
                If addressLine3RowVisible And Not ModuleDefaults.AddressingMapAdr3.Trim = "" Then
                    sString = GetJavascriptString("document.forms[0]." & town.UniqueID, ModuleDefaults.AddressingMapAdr3, ModuleDefaults.AddressingFields)
                    Response.Write(sString)
                End If
                If addressLine4RowVisible And Not ModuleDefaults.AddressingMapAdr4.Trim = "" Then
                    sString = GetJavascriptString("document.forms[0]." & city.UniqueID, ModuleDefaults.AddressingMapAdr4, ModuleDefaults.AddressingFields)
                    Response.Write(sString)
                End If
                If addressLine5RowVisible And Not ModuleDefaults.AddressingMapAdr5.Trim = "" Then
                    sString = GetJavascriptString("document.forms[0]." & county.UniqueID, ModuleDefaults.AddressingMapAdr5, ModuleDefaults.AddressingFields)
                    Response.Write(sString)
                End If
                If Not ModuleDefaults.AddressingMapPost.Trim = "" Then
                    sString = GetJavascriptString("document.forms[0]." & postcode.UniqueID, ModuleDefaults.AddressingMapPost, ModuleDefaults.AddressingFields)
                    Response.Write(sString)
                End If
                If Not ModuleDefaults.AddressingMapCountry.Trim = "" Then
                    sString = GetJavascriptString("document.forms[0]." & country.UniqueID, ModuleDefaults.AddressingMapCountry, ModuleDefaults.AddressingFields)
                    Response.Write(sString)
                End If

                Response.Write("}" & vbCrLf)
                Response.Write("function trim(s) { " & vbCrLf & "var r=/\b(.*)\b/.exec(s); " & vbCrLf & "return (r==null)?"""":r[1]; " & vbCrLf & "}")
                Response.Write("</script>" & vbCrLf)
            End If
        End If
    End Sub

    Protected Function GetJavascriptString(ByVal sFieldString As String, ByVal sAddressingMap As String, ByVal sAddressingFields As String) As String
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
                                    "if (trim(" & sFieldString & sStr2 & ") == '' || " & sFieldString & " == document.forms[0]." & country.UniqueID & ") {" & vbCrLf & _
                                    sFieldString & sStr2 & " = " & sStr1 & count2.ToString & sStr2 & ";" & vbCrLf & _
                                    "}" & vbCrLf & _
                                    "else {" & vbCrLf & _
                                    sFieldString & sStr2 & " = " & sFieldString & sStr2 & " + ', ' + " & sStr1 & count2.ToString & sStr2 & ";" & vbCrLf & _
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
            Return String.Empty
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

    Protected Sub chkRegAddressSame_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkRegAddressSame.CheckedChanged

        txtRegAddress1.Enabled = Not chkRegAddressSame.Checked
        txtRegAddress1.Visible = Not chkRegAddressSame.Checked
        lblRegAddress1.Visible = Not chkRegAddressSame.Checked
        txtRegAddress2.Enabled = Not chkRegAddressSame.Checked
        txtRegAddress2.Visible = Not chkRegAddressSame.Checked
        lblRegAddress2.Visible = Not chkRegAddressSame.Checked
        txtRegAddress3.Enabled = Not chkRegAddressSame.Checked
        txtRegAddress3.Visible = Not chkRegAddressSame.Checked
        lblRegAddress3.Visible = Not chkRegAddressSame.Checked
        txtRegAddress4.Enabled = Not chkRegAddressSame.Checked
        txtRegAddress4.Visible = Not chkRegAddressSame.Checked
        lblRegAddress4.Visible = Not chkRegAddressSame.Checked
        txtRegAddress5.Enabled = Not chkRegAddressSame.Checked
        txtRegAddress5.Visible = Not chkRegAddressSame.Checked
        lblRegAddress5.Visible = Not chkRegAddressSame.Checked

        txtRegPostcode.Enabled = Not chkRegAddressSame.Checked
        txtRegPostcode.Visible = Not chkRegAddressSame.Checked
        lblRegPostcode.Visible = Not chkRegAddressSame.Checked

        ddlRegCountry.Visible = Not chkRegAddressSame.Checked
        lblRegCountry.Visible = Not chkRegAddressSame.Checked
        ddlRegCountry.Enabled = Not chkRegAddressSame.Checked

        ' Black out values if same as home address
        If chkRegAddressSame.Checked Then
            txtRegAddress1.Text = String.Empty
            txtRegAddress2.Text = String.Empty
            txtRegAddress3.Text = String.Empty
            txtRegAddress4.Text = String.Empty
            txtRegAddress5.Text = String.Empty
            txtRegPostcode.Text = String.Empty
        End If
        SetupRegRequiredValidator(rfvTxtRegAddress1, New System.EventArgs)
        SetupRegRequiredValidator(rfvTxtRegAddress2, New System.EventArgs)
        SetupRegRequiredValidator(rfvTxtRegAddress3, New System.EventArgs)
        SetupRegRequiredValidator(rfvTxtRegAddress4, New System.EventArgs)
        SetupRegRequiredValidator(rfvTxtRegAddress5, New System.EventArgs)
        SetupRegRequiredValidator(rfvTxtRegPostcode, New System.EventArgs)
    End Sub

    Protected Function CreateBasketCustomerSearchDataTable() As DataTable
        Dim dtBasketCustomerSearch As New DataTable
        With dtBasketCustomerSearch.Columns
            .Add(New DataColumn("CustomerNumber", GetType(String)))
            .Add(New DataColumn("FoundCustomerNumber", GetType(String)))
            .Add(New DataColumn("Forename", GetType(String)))
            .Add(New DataColumn("Surname", GetType(String)))
        End With
        Return dtBasketCustomerSearch
    End Function

    Protected Sub lblUserID4_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblUserID4.Load

    End Sub

    Protected Sub country_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles country.SelectedIndexChanged
        ' if turkish then show turkey address format
        If ModuleDefaults.AddressFormat = "1" AndAlso country.SelectedValue.Trim = "TR" Then
            Session("AddressFormat") = "1"
            Session("AddressFormatCountry") = country.SelectedValue
        End If
    End Sub

    Private Sub UseAddresslessRegistration()

        If Not ucr.Content("DefaultAddressStringForAddresslessRegistration", _languageCode, True).Trim = "" Then
            plhAddressBox.Visible = False
            typeRFV.Enabled = False
            typeRegEx.Enabled = False
            referenceRFV.Enabled = False
            referenceRegEx.Enabled = False
            sequenceRFV.Enabled = False
            sequenceRegEx.Enabled = False
            buildingRFV.Enabled = False
            buildingRegEx.Enabled = False
            streetRFV.Enabled = False
            streetRegEx.Enabled = False
            townRFV.Enabled = False
            townRegEx.Enabled = False
            cityRFV.Enabled = False
            cityRegEx.Enabled = False
            countyRFV.Enabled = False
            countyRegEx.Enabled = False
            postcodeRFV.Enabled = False
            postcodeRegEx.Enabled = False
            countryRegEx.Enabled = False
        End If
    End Sub

    Private Sub DoAddresslessRegistration(ByRef userAddress As TalentProfileAddress)
        If Not ucr.Content("DefaultAddressStringForAddresslessRegistration", _languageCode, True).Trim = "" Then
            With userAddress
                If .Address_Line_1.Trim = "" And .Address_Line_2.Trim = "" And .Address_Line_3.Trim = "" And _
                    .Address_Line_4.Trim = "" And .Address_Line_5.Trim = "" And .Post_Code.Trim = "" Then
                    .Address_Line_1 = ucr.Content("DefaultAddressStringForAddresslessRegistration", _languageCode, True)
                End If
            End With
        End If
    End Sub

    Private Sub SetAttributes()
        With ucr
            plhBookNumber.Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ShowBookNumber"))
        End With
    End Sub

    Protected Sub ddlRegistrationType_SelectedIndexChanged(sender As Object, e As EventArgs)


        If plhRegistrationTypeBox.Visible Then
            If Not AgentProfile.IsAgent() Then

                If Not CheckCompanyName() Then
                    companyNameRFV.Enabled = True
                    companyNameRFV.Visible = True
                End If
            Else
                companyNameRFV.Enabled = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("txtCompanyNameEnableRFV"))
                companyNameRFV.Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("txtCompanyNameEnableRFV"))
            End If
        Else
            companyNameRFV.Enabled = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("txtCompanyNameEnableRFV"))
            companyNameRFV.Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("txtCompanyNameEnableRFV"))
        End If

        If companyNameRFV.Enabled Then
            companyNameRFV.ErrorMessage = ucr.Content("txtCompanyNameRequiredFieldValidator", _languageCode, True)
        End If



    End Sub

    Protected Sub AddContactToCompany(ByVal CustomerNumber As String, ByVal CompanyNumber As String)
        Dim companyBuilder As New CompanyModelBuilders()
        Dim customerCompanyInputModel As New CustomerCompanyInputModel()
        customerCompanyInputModel.CompanyNumber = CompanyNumber
        customerCompanyInputModel.CustomerNumber = Profile.UserName
        Dim err As ErrorModel = companyBuilder.AddCustomerCompanyAssociation(customerCompanyInputModel)
        If err.HasError Then
            'Add error
            ErrorLabel.Text = err.ErrorMessage
        Else
            Response.Redirect("~/PagesPublic/CRM/CompanyContacts.aspx?CompanyNumber=" + CompanyNumber)
        End If
        
    End Sub
End Class