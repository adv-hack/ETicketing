Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Data
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_UpdateDetailsForm
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
    Private addressTypeRowVisible As Boolean = True
    Private addressReferenceRowVisible As Boolean = True
    Private addressSequenceRowVisible As Boolean = True
    Private _display As Boolean = True
    Private mailOptionTrueValue As Boolean = True
    Private opt1TrueValue As Boolean = True
    Private opt2TrueValue As Boolean = True
    Private opt3TrueValue As Boolean = True
    Private opt4TrueValue As Boolean = True
    Private opt5TrueValue As Boolean = True
    Private otherMembersatAddress As Boolean = False
    Private _ParentalConsentCeilingDob As Date = TalentDefaults.ParentalConsentCeilingDoB
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        SetEnterButtonClickEvent()
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender


        ' Clear any Agent Portal session variables - no longer needed in update mode
        If Not Session("Passport") Is Nothing Then
            Session("Passport") = Nothing
        End If
        If Not Session("PIN") Is Nothing Then
            Session("PIN") = Nothing
        End If
        If Not Session("Greencard") Is Nothing Then
            Session("Greencard") = Nothing
        End If
        If Not Session("UserID4") Is Nothing Then
            Session("UserID4") = Nothing
        End If
        If Not Session("UserID5") Is Nothing Then
            Session("UserID5") = Nothing
        End If
        If Not Session("UserID6") Is Nothing Then
            Session("UserID6") = Nothing
        End If
        If Not Session("UserID7") Is Nothing Then
            Session("UserID7") = Nothing
        End If
        If Not Session("UserID8") Is Nothing Then
            Session("UserID8") = Nothing
        End If
        If Not Session("UserID9") Is Nothing Then
            Session("UserID9") = Nothing
        End If

        SetUpUCR()
        If Not Page.IsPostBack Then

            Session("AddressFormat") = Nothing
            Dim address As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)
            Dim licCountries As ListItemCollection = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "MYACCOUNT", "COUNTRY")
            Dim tr As ListItem
            Try
                tr = licCountries.FindByValue("TR ")
                If tr Is Nothing Then
                    licCountries.FindByValue("TR")
                End If
                If Not tr Is Nothing AndAlso address.Country.ToUpper <> tr.Text.ToUpper Then
                    Session("AddressFormat") = "0"
                End If

            Catch ex As Exception

            End Try
            SetUpOpt()
            PopulateTitleDropDownList()
            PopulateSexDropDownLists()
            PopulateDOBDropDownLists()
            PopulateCountriesDropDownList()
            PopulatePriceBandsDropDownList()
            PopulateStopcodesDropDownList()
            PopulateCustomerTextBox()
            SetDetailsVisibility()
            SetAddressVisibilityProperties()
            SetAddressVisibility()
            SetupUserIDsGroup()
            SetLabelText()
            Setup_Supporter_Favourites_And_Club_Contact_Options()
            SetControlForUserDetails()
            SetAttributes()
            plhUserIDs.Visible = ModuleDefaults.ShowUserIDFields

            If Not CType(ucr.Attribute("dobEnableRFV"), Boolean) Then
                dobDayRegEx.Enabled = False
                dobMonthRegEx.Enabled = False
                dobYearRegEx.Enabled = False
            End If

            If Not Profile.IsAnonymous Then
                capturePhotoBtn.Value = ucr.Content("capturePhotoBtn", _languageCode, True)
            End If

        Else
            ' still need to do address visibility on postback
            ' as address format might change if country is changed (galatasaray mods)
            SetAddressVisibilityProperties()
            SetAddressVisibility()
        End If

        'Show password control
        If Talent.eCommerce.Utilities.IsAgent Then
            ChangeMyPassword1.DisplayControl = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayPasswordControlForAgent"))
        Else
            ChangeMyPassword1.DisplayControl = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("DisplayPasswordControl"))
        End If

        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("ShowCaptureButton")) Then
            pnlCapturePhoto.Visible = True
            If Not Profile.IsAnonymous Then
                capturePhotoBtn.Value = ucr.Content("capturePhotoBtn", _languageCode, True)
            End If
        Else
            pnlCapturePhoto.Visible = False
        End If

        ' final checks for setting label visibility
        ErrorLabel.Visible = (ErrorLabel.Text.Length > 0)
        ErrorLabelID.Visible = (ErrorLabelID.Text.Length > 0)
    End Sub

    Protected Sub SetEnterButtonClickEvent()
        Dim buttonID As String = String.Empty
        buttonID = updateBtn.ClientID
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
        altusername.Attributes.Add(onKeyDown, EventText)
        contactsl.Attributes.Add(onKeyDown, EventText)
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
    End Sub

    Protected Sub SetUpUCR()
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.Common.Utilities.GetAllString 'GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "UpdateDetailsForm.ascx"
        End With
    End Sub

    Protected Sub SetUpOpt()

        With ucr
            'do visibility
            plhMailOption.Visible = CType(.Attribute("mailOptionEnabled"), Boolean)
            plhOpt1.Visible = CType(.Attribute("opt1Enabled"), Boolean)
            plhOpt2.Visible = CType(.Attribute("opt2Enabled"), Boolean)
            plhOpt3.Visible = CType(.Attribute("opt3Enabled"), Boolean)
            plhOpt4.Visible = CType(.Attribute("opt4Enabled"), Boolean)
            plhOpt5.Visible = CType(.Attribute("opt5Enabled"), Boolean)
            plhOpt5Radio.Visible = CType(.Attribute("opt5Enabled"), Boolean)
            'do default status
            mailOption.Checked = CType(.Attribute("mailOptionDefault"), Boolean)
            opt1.Checked = CType(.Attribute("opt1Default"), Boolean)
            opt2.Checked = CType(.Attribute("opt2Default"), Boolean)
            opt3.Checked = CType(.Attribute("opt3Default"), Boolean)
            opt4.Checked = CType(.Attribute("opt4Default"), Boolean)
            opt5.Checked = CType(.Attribute("opt5Default"), Boolean)
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


            If Profile.User.Details.OWNS_AUTO_MEMBERSHIP Then
                Me.plhAddMembership.Visible = False
                Me.addMembershipCheck.Checked = False
            Else
                Me.plhAddMembership.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(.Attribute("AddMembership_ShowCheckbox"))
                Me.addMembershipCheck.Checked = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(.Attribute("AddMembership_CheckedByDefault"))
                Me.addMembershipCheck.Text = .Content("AddMembership_CheckboxLabel", _languageCode, True)
            End If
        End With
    End Sub

    Protected Sub SetControlForUserDetails()

        Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

        If Request.QueryString("reason") IsNot Nothing Then
            If UCase(Request.QueryString("reason").ToString) = "USERNAME" Then
                ErrorLabel.Text = ucr.Content("InvalidUsernameError", _languageCode, True)
            ElseIf UCase(Request.QueryString("reason").ToString) = "ACCOUNTERROR" Then
                If UCase(Request.QueryString("type").ToString) = "ADDRESS" Then
                    ErrorLabel.Text = ucr.Content("InvalidAddressError", _languageCode, True)
                Else
                    ErrorLabel.Text = ucr.Content("InvalidAccountError", _languageCode, True)
                End If


            End If
        End If
        Dim blankDOBField As Boolean = False
        If Profile.User.Details.DOB.Date.ToString.Equals("01/01/1900 00:00:00") Then blankDOBField = True
        SetUserDetails(blankDOBField)
        ChangeMyPassword1.Visible = True
        '-----------------------------------------------------------------------------------------
        ' If from activation (activateAccount.ascx) then protect 'old' password field and make new
        ' password mandatory
        '-----------------------------------------------------------------------------------------
        If Not Session("FromActivation") Is Nothing AndAlso Session("FromActivation") = True Then
            ChangeMyPassword1.CurrentPasswordBox.Enabled = False
            ChangeMyPassword1.CurrentPasswordRequiredFieldValidator.Enabled = False
        End If

        ' Protect the name fields? 
        If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("titleRowVisible")) Then
            If title.SelectedValue = " -- " Then
                title.Enabled = True
            Else
                title.Enabled = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("titleRowEnabled"))
            End If
        End If
        If Not String.IsNullOrEmpty(initials.Text) AndAlso Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("initialsRowEnabled")) Then
            initials.Enabled = False
        End If
        If Not String.IsNullOrEmpty(forename.Text) AndAlso Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("forenameRowEnabled")) Then
            forename.Enabled = False
        End If
        If def.UserIDValidationTypeforID3 = "1" AndAlso String.IsNullOrEmpty(txtGreenCard.Text) AndAlso String.IsNullOrEmpty(txtPassport.Text) Then
            txtGreenCard.Enabled = True
        End If
        If def.UserIDValidationTypeforID3 = "1" AndAlso Not String.IsNullOrEmpty(txtGreenCard.Text) Then
            txtPassport.Enabled = False
        End If
        If Not String.IsNullOrEmpty(txtGreenCard.Text) AndAlso Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("PassportID3RowEnabled")) Then
            txtGreenCard.Enabled = False
        End If
        If Not String.IsNullOrEmpty(surname.Text) AndAlso Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("surnameRowEnabled")) Then
            surname.Enabled = False
        End If
        If Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("SupporterSelectEnabled")) Then
            supporterSelectDDL1.Enabled = False
            supporterSelectDDL2.Enabled = False
            supporterSelectDDL3.Enabled = False
            supporterSelectButton1.Enabled = False
        End If
        If Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("MothersNameRowEnabled")) Then
            mothersname.Enabled = False
        End If
        If Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("FathersNameRowEnabled")) Then
            fathersname.Enabled = False
        End If
        If Talent.eCommerce.Utilities.IsAgent Then
            If Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("dobRowEnabledForAgent")) Then
                dobDay.Enabled = False
                dobMonth.Enabled = False
                dobYear.Enabled = False
            End If
        Else
            If Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("dobRowEnabled")) Then
                If blankDOBField Then
                    dobDay.Enabled = True
                    dobMonth.Enabled = True
                    dobYear.Enabled = True
                Else
                    dobDay.Enabled = False
                    dobMonth.Enabled = False
                    dobYear.Enabled = False
                End If
            End If
        End If
        updateBtn.Visible = True
    End Sub

    Private Sub SetLabelText()
        Try
            updateBtn.Text = ucr.Content("updateBtn", _languageCode, True)                                           ' Update"
            btnPrintAddressLabelBottom.Text = ucr.Content("btnPrintAddressLabel", _languageCode, True)
            btnPrintAddressLabelTop.Text = ucr.Content("btnPrintAddressLabel", _languageCode, True)

            With ucr
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
                contactslLabel.Text = .Content("contactslLabel", _languageCode, True)                                        ' contactsl
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
                mothersnameLabel.Text = .Content("mothersnameLabel", _languageCode, True)
                fathersnameLabel.Text = .Content("fathersnameLabel", _languageCode, True)

                ltlContactPrefsH2.Text = .Content("ContactPreferencesH2", _languageCode, True)
                Newsletter.Text = .Content("SubscribeToNewsletterText", _languageCode, True)
                Subscribe2.Text = .Content("Subscribe2Text", _languageCode, True)
                Subscribe3.Text = .Content("Subscribe3Text", _languageCode, True)


                'davetodo put in database !!!!!!!!!!
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

                If Talent.eCommerce.Utilities.IsAgent Then
                    PlhShowPermissionCheckBox.Visible = True
                Else
                    PlhShowPermissionCheckBox.Visible = False
                End If
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

                    chkRegAddressSame.Checked = False
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
                    hplStopCodeAudit.Text = .Content("StopcodeAuditLabel", _languageCode, True)
                    BookNumberLabel.Text = .Content("BookNumberLabel", _languageCode, True)
                Else
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
                type.MaxLength = .Attribute("typeMaxLength")
                reference.MaxLength = .Attribute("referenceMaxLength")
                sequence.MaxLength = .Attribute("sequenceMaxLength")
                building.MaxLength = .Attribute("buildingMaxLength")
                postcode.MaxLength = .Attribute("postcodeMaxLength")
                street.MaxLength = .Attribute("streetMaxLength")
                town.MaxLength = .Attribute("townMaxLength")
                city.MaxLength = .Attribute("cityMaxLength")
                county.MaxLength = .Attribute("countyMaxLength")
                mothersname.MaxLength = .Attribute("mothersnameMaxLength")
                fathersname.MaxLength = .Attribute("fathersnameMaxLength")
                txtRegPostcode.MaxLength = .Attribute("postcodeMaxLength")
            End With

        Catch ex As Exception
        End Try
    End Sub

    Protected Sub PopulateSexDropDownLists()
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
    End Sub

    Public Sub SetupCompareValidator(ByVal sender As Object, ByVal e As EventArgs)
        Try
            SetUpUCR()
            Dim cv As CompareValidator = CType(sender, CompareValidator)
            Select Case cv.ControlToValidate
                Case Is = "password2"
                    cv.ErrorMessage = ucr.Content("ComparePasswordsErrortText", _languageCode, True)
                Case Is = "emailConfirm"
                    cv.ErrorMessage = ucr.Content("CompareEmailErrorText", _languageCode, True)
                Case Else
                    cv.ErrorMessage = ucr.Content(cv.ControlToValidate & "CompareValidator", _languageCode, True)
            End Select
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

    Public Sub SetupRegRequiredValidator(ByVal sender As Object, ByVal e As EventArgs)
        Dim rfv As RequiredFieldValidator = CType(sender, RequiredFieldValidator)
        Try
            SetUpUCR()
            If plhRegisteredAddress.Visible AndAlso
                Not chkRegAddressSame.Checked AndAlso
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
                    '   rev.ValidationExpression = "^[a-zA-Z\s]{0,50}$"
                    rev.ValidationExpression = "^[^-]{0,50}$"

                Case Is = "NewPassword"
                    rev.ErrorMessage = ucr.Content("newPasswordErrorMessage", _languageCode, True) ' You password does not meet the requirements"
                    rev.ValidationExpression = ucr.Attribute("PasswordExpression")

                Case Is = "txtCompanyName"
                    rev.ValidationExpression = ucr.Attribute("CompanyNameExpression")

                Case Is = "forename",
                            "surname",
                            "customersuffix",
                            "nickname",
                            "altusername",
                            "salutation",
                            "position",
                            "type",
                            "reference",
                            "city",
                            "county",
                            "contactsl"
                    rev.ValidationExpression = ucr.Attribute("TextOnlyExpression")

                Case Is = "initials"
                    rev.ValidationExpression = ucr.Attribute("InitialsExpression")

                Case Is = "dobDay"
                    'rev.ValidationExpression = "^(?! -- )?[a-zA-Z0-9\s]+"
                    If CType(ucr.Attribute("dobRowEnabled"), Boolean) Then rev.ValidationExpression = ucr.Attribute("dobDayExpression") Else rev.Enabled = False

                Case Is = "dobMonth"
                    'rev.ValidationExpression = "^(?! -- )?[a-zA-Z0-9\s]+"
                    If CType(ucr.Attribute("dobRowEnabled"), Boolean) Then rev.ValidationExpression = ucr.Attribute("dobMonthExpression") Else rev.Enabled = False

                Case Is = "dobYear"
                    'rev.ValidationExpression = "^(?! -- )?[a-zA-Z0-9\s]+"
                    If CType(ucr.Attribute("dobRowEnabled"), Boolean) Then rev.ValidationExpression = ucr.Attribute("dobYearExpression") Else rev.Enabled = False

                Case Is = "email", "parentemail"
                    rev.ValidationExpression = ucr.Attribute("EmailExpression")

                Case Is = "phone",
                            "work",
                            "mobile",
                            "fax",
                           "parentphone", _
                            "other"
                    rev.ValidationExpression = ucr.Attribute("PhoneNumberExpression")

                Case Is = "sequence"
                    rev.ValidationExpression = ucr.Attribute("NumberExpression")

                Case Is = "building",
                            "street",
                            "town",
                             "txtRegAddress1",
                             "txtRegAddress2",
                             "txtRegAddress3",
                             "txtRegAddress4",
                             "txtRegAddress5"
                    rev.ValidationExpression = ucr.Attribute("TextAndNumberExpression")

                Case Is = "postcode", "txtRegPostcode"
                    rev.ValidationExpression = ucr.Attribute("PostcodeExpression")

                Case Is = "country", "ddlRegCountry"
                    'rev.ValidationExpression = "^(?! -- )?[a-zA-Z\s]+"
                    rev.ValidationExpression = "^[a-zA-Z\s]{0,50}$"

                Case Is = "sex"
                    rev.ValidationExpression = "^(?! -- )?[a-zA-Z\s]+"
                    rev.Enabled = ModuleDefaults.ProfileSexMandatory

            End Select
        End If
    End Sub

    Protected Sub SetupUserIDsGroup()
        plhUserIDs.Visible = ModuleDefaults.ShowUserIDFields
        If plhUserIDs.Visible Then
            passportDiv.Visible = ModuleDefaults.ShowPassportField
            greencardDiv.Visible = ModuleDefaults.ShowGreenCardField
            pinDiv.Visible = ModuleDefaults.ShowPinField
            userID4Div.Visible = ModuleDefaults.ShowUserID4Field
            userID5Div.Visible = ModuleDefaults.ShowUserID5Field
            userID6Div.Visible = ModuleDefaults.ShowUserID6Field
            userID7Div.Visible = ModuleDefaults.ShowUserID7Field
            userID8Div.Visible = ModuleDefaults.ShowUserID8Field
            userID9Div.Visible = ModuleDefaults.ShowUserID9Field
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
    End Sub

    Protected Sub PopulateDOBDropDownLists()
        Dim l1 As New ListItem(" -- ", "--")
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

        Dim l2 As New ListItem(" -- ", "--")
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
        Dim l3 As New ListItem(" -- ", "--")
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
    End Sub

    Protected Sub PopulateTitleDropDownList()

        If ModuleDefaults.GetCustomerTitlesFromIseries = True Then
            Dim Settings As DESettings = Talent.eCommerce.Utilities.GetSettingsObject
            Dim defaults As New TalentDefaults
            Dim err As New Talent.Common.ErrorObj
            defaults.Settings = Settings
            err = defaults.RetrieveCustomerTitles
            title.DataSource = TalentCache.GetListCollectionFromTitlesDataTable(defaults.ResultDataSet.Tables("CustomerTitles"))
        Else
            title.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "MYACCOUNT", "TITLE")
        End If
        title.DataTextField = "Text"
        title.DataValueField = "Value"
        title.DataBind()
    End Sub

    Protected Sub PopulateCountriesDropDownList()
        Dim defaultCountry As String = TalentCache.GetDefaultCountryForBU()
        country.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "MYACCOUNT", "COUNTRY")
        country.DataTextField = "Text"
        country.DataValueField = "Value"
        country.DataBind()

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
                .DestinationDatabase = Talent.eCommerce.Utilities.GetCustomerDestinationDatabase()
                .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("TALENTCRM").ToString
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            End With

            pb.Settings = Settings
            Dim ta As New Talent.eCommerce.Agent
            pb.Company = ta.GetAgentCompany

            err = pb.RetrieveTalentStopcodes()

            ' Was the call successful
            If Not err.HasError AndAlso
                 pb.ResultDataSet.Tables.Count() > 0 Then

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

    Protected Sub PopulateCustomerTextBox()

        If Talent.eCommerce.Utilities.IsAgent Then

            Dim err As New Talent.Common.ErrorObj
            Dim ct As New TalentCustomerText
            Dim Settings As New DESettings

            With Settings
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .BusinessUnit = TalentCache.GetBusinessUnit
                .Company = ucr.Content("crmExternalKeyName", _languageCode, True)
                .Cacheing = False
                .DestinationDatabase = Talent.eCommerce.Utilities.GetCustomerDestinationDatabase()
                '  .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("TALENTCRM").ToString
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            End With

            ct.Settings = Settings
            ct.deCustomerText.Customer = Profile.User.Details.LoginID.Trim
            ct.deCustomerText.Agent = AgentProfile.Name
            err = ct.RetrieveTalentCustomerText

        End If

    End Sub

    Protected Sub SetDetailsVisibility()
        Try
            With ucr
                title.Enabled = CType(.Attribute("titleRowEnabled"), Boolean)
                initials.Enabled = CType(.Attribute("initialsRowEnabled"), Boolean)
                forename.Enabled = CType(.Attribute("forenameRowEnabled"), Boolean)
                surname.Enabled = CType(.Attribute("surnameRowEnabled"), Boolean)
                contactsl.Enabled = CType(.Attribute("contactSLRowEnabled"), Boolean)
                plhTitleRow.Visible = CType(.Attribute("titleRowVisible"), Boolean)
                plhForenameRow.Visible = CType(.Attribute("forenameRowVisible"), Boolean)
                plhSurnameRow.Visible = CType(.Attribute("surnameRowVisible"), Boolean)
                plhCustomersuffixRow.Visible = CType(.Attribute("customersuffixRowVisible"), Boolean)
                plhNicknameRow.Visible = CType(.Attribute("nicknameRowVisible"), Boolean)
                plhAltusernameRow.Visible = CType(.Attribute("altusernameRowVisible"), Boolean)
                plhContactSLRow.Visible = CType(.Attribute("contactslRowVisible"), Boolean)
                plhInitialsRow.Visible = CType(.Attribute("initialsRowVisible"), Boolean)
                plhSalutationRow.Visible = CType(.Attribute("salutationRowVisible"), Boolean)
                plhCompanyRow.Visible = CType(.Attribute("CompanyNameRowVisible"), Boolean)
                plhPositionRow.Visible = CType(.Attribute("positionRowVisible"), Boolean)
                plhDOBRow.Visible = CType(.Attribute("dobRowVisible"), Boolean)
                plhSexRow.Visible = CType(.Attribute("sexRowVisible"), Boolean)
                If plhSexRow.Visible Then
                    sex.Enabled = CType(.Attribute("sexRowEnabled"), Boolean)
                End If
                mothersname.Visible = CType(.Attribute("mothersnameRowVisible"), Boolean)
                fathersname.Visible = CType(.Attribute("fathersnameRowVisible"), Boolean)
                plhEmailRow.Visible = CType(.Attribute("emailRowVisible"), Boolean)
                plhPhoneRow.Visible = CType(.Attribute("phoneRowVisible"), Boolean)
                plhWorkRow.Visible = CType(.Attribute("workRowVisible"), Boolean)
                plhMobileRow.Visible = CType(.Attribute("mobileRowVisible"), Boolean)
                plhFaxRow.Visible = CType(.Attribute("faxRowVisible"), Boolean)
                plhotherRow.Visible = CType(.Attribute("otherRowVisible"), Boolean)
                plhNewsletter.Visible = CType(.Attribute("newsletterVisible"), Boolean)
                plhSubscribe2.Visible = CType(.Attribute("subscribe2Visible"), Boolean)
                plhSubscribe3.Visible = CType(.Attribute("subscribe3Visible"), Boolean)
                plhMothersnameRow.Visible = CType(.Attribute("mothersnameRowVisible"), Boolean)
                plhFathersnameRow.Visible = CType(.Attribute("fathersnameRowVisible"), Boolean)

                ' Agent visibility checks
                If Talent.eCommerce.Utilities.IsAgent Then
                    plhEmailCompare.Visible = False
                End If

                If plhNewsletter.Visible OrElse plhSubscribe2.Visible OrElse
                    plhSubscribe3.Visible OrElse plhMailOption.Visible OrElse
                    plhOpt1.Visible OrElse plhOpt2.Visible OrElse
                    plhOpt3.Visible OrElse plhOpt4.Visible OrElse
                    plhOpt5.Visible Then

                    plhContactPreferences.Visible = True
                Else
                    plhContactPreferences.Visible = False
                End If
            End With
        Catch ex As Exception
        End Try
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

    Protected Sub SetUserDetails(ByVal hasInvalidDOB As Boolean)


        With Profile.User.Details
            email.Text = .Email
            emailConfirm.Text = .Email
            For Each item As ListItem In title.Items
                If .Title = item.Text Then
                    item.Selected = True
                    Exit For
                End If
            Next
            forename.Text = .Forename
            initials.Text = .Initials
            surname.Text = .Surname
            customersuffix.Text = .CUSTOMER_SUFFIX
            nickname.Text = .NICKNAME
            altusername.Text = .USERNAME
            contactsl.Text = .Account_No_2
            salutation.Text = .Salutation
            txtCompanyName.Text = .CompanyName
            position.Text = .Position
            fathersname.Text = .Fathers_Name
            cbContactbyEmail.Checked = .CONTACT_BY_EMAIL
            cbContactbyMobile.Checked = .CONTACT_BY_MOBILE
            cbContactbyPost.Checked = .CONTACT_BY_POST
            cbContactbyTelephoneHome.Checked = .CONTACT_BY_TELEPHONE_HOME
            cbContactbyTelephoneWork.Checked = .CONTACT_BY_TELEPHONE_WORK
            ddlStopcode.SelectedValue = .STOP_CODE
            ddlpriceband.SelectedValue = .PRICE_BAND
            BookNumber.Text = .BOOK_NUMBER
            parentemail.Text = .PARENT_EMAIL
            parentphone.Text = .PARENT_PHONE

            lblParentalConsentCeilingYear.Text = _ParentalConsentCeilingDob.Year
            lblParentalConsentCeilingMonth.Text = _ParentalConsentCeilingDob.Month
            lblParentalConsentCeilingDay.Text = _ParentalConsentCeilingDob.Day
            If Talent.eCommerce.Utilities.IsAgent Then
                PlhShowPermissionCheckBox.Visible = True
            Else
                PlhShowPermissionCheckBox.Visible = False
            End If

            
            cbParentalPermissionGranted.Checked = (.PARENTAL_CONSENT_STATUS = "G")

            If hasInvalidDOB Then
                dobDay.Text = " -- "
                dobMonth.Text = " -- "
                dobYear.Text = " -- "
            Else
                dobDay.Text = Day(.DOB.ToShortDateString)
                dobMonth.Text = Month(.DOB.ToShortDateString)
                dobYear.Text = Year(.DOB.ToShortDateString)
            End If
            If .Bit0 = mailOptionTrueValue Then
                mailOption.Checked = mailOptionTrueValue
            Else
                mailOption.Checked = Not mailOptionTrueValue
            End If
            If .Bit1 = opt1TrueValue Then
                opt1.Checked = opt1TrueValue
            Else
                opt1.Checked = Not opt1TrueValue
            End If
            If .Bit2 = opt2TrueValue Then
                opt2.Checked = opt2TrueValue
            Else
                opt2.Checked = Not opt2TrueValue
            End If
            If .Bit3 = opt3TrueValue Then
                opt3.Checked = opt3TrueValue
            Else
                opt3.Checked = Not opt3TrueValue
            End If
            If .Bit4 = opt4TrueValue Then
                opt4.Checked = opt4TrueValue
            Else
                opt4.Checked = Not opt4TrueValue
            End If
            If .Bit5 = opt5TrueValue Then
                opt5.Checked = opt5TrueValue
            Else
                opt5.Checked = Not opt5TrueValue
            End If

            Dim li As New ListItem()
            li = sex.Items.FindByValue(.Sex)
            li.Selected = True

            mobile.Text = .Mobile_Number
            phone.Text = .Telephone_Number
            work.Text = .Work_Number
            fax.Text = .Fax_Number
            other.Text = .Other_Number
            Newsletter.Checked = .Subscribe_Newsletter
            Subscribe2.Checked = .Subscribe_21
            Subscribe3.Checked = .Subscribe_31


            Try
                Select Case .HTML_Newsletter
                    Case Is = True
                        NewsletterChoice.SelectedValue = ucr.Content("HTMLNewsletterText", _languageCode, True)
                    Case Is = False
                        NewsletterChoice.SelectedValue = ucr.Content("PlainTextNewsletterText", _languageCode, True)
                End Select
            Catch
            End Try

            If Not String.IsNullOrEmpty(.SUPPORTER_CLUB_CODE) Then
                Dim dtSportTeamClub As DataTable = Talent.eCommerce.Utilities.GetSupporterSportTeamClubDDL

                If dtSportTeamClub IsNot Nothing AndAlso dtSportTeamClub.Rows.Count > 0 Then
                    Dim stc() As DataRow = dtSportTeamClub.Select("SC_CODE = '" & .SUPPORTER_CLUB_CODE & "'")
                    If stc IsNot Nothing AndAlso stc.Length > 0 Then
                        Try
                            supporterSelectDDL1.SelectedValue = Talent.eCommerce.Utilities.CheckForDBNull_String(stc(0)("SPORT_CODE"))
                            Me.supporterSelectButton1_Click(New Object, New EventArgs)
                            supporterSelectDDL2.SelectedValue = Talent.eCommerce.Utilities.CheckForDBNull_String(stc(0)("TEAM_CODE"))
                            Me.supporterSelectButton2_Click(New Object, New EventArgs)
                            supporterSelectDDL3.SelectedValue = .SUPPORTER_CLUB_CODE
                        Catch ex As Exception
                        End Try
                    End If
                End If
            ElseIf Not String.IsNullOrEmpty(.FAVOURITE_TEAM_CODE) Then
                Dim dtSportTeam As DataTable = Talent.eCommerce.Utilities.GetSupporterSportTeamDDL

                If dtSportTeam IsNot Nothing AndAlso dtSportTeam.Rows.Count > 0 Then
                    Dim stc() As DataRow = dtSportTeam.Select("TEAM_CODE = '" & .FAVOURITE_TEAM_CODE & "'")
                    If stc IsNot Nothing AndAlso stc.Length > 0 Then
                        Try
                            supporterSelectDDL1.SelectedValue = Talent.eCommerce.Utilities.CheckForDBNull_String(stc(0)("SPORT_CODE"))
                            Me.supporterSelectButton1_Click(New Object, New EventArgs)
                            supporterSelectDDL2.SelectedValue = Talent.eCommerce.Utilities.CheckForDBNull_String(stc(0)("TEAM_CODE"))
                            Me.supporterSelectButton2_Click(New Object, New EventArgs)
                            supporterSelectDDL3.Items.Insert(0, ucr.Content("supporterSelectDDL3_PleaseSelect_Text", _languageCode, True))
                        Catch ex As Exception
                        End Try
                    End If
                End If
            ElseIf Not String.IsNullOrEmpty(.FAVOURITE_SPORT) Then
                Dim dtSport As DataTable = Talent.eCommerce.Utilities.GetSupporterSportDDL
                If dtSport IsNot Nothing AndAlso dtSport.Rows.Count > 0 Then
                    Dim stc() As DataRow = dtSport.Select("SPORT_CODE = '" & .FAVOURITE_SPORT & "'")
                    If stc IsNot Nothing AndAlso stc.Length > 0 Then
                        Try
                            supporterSelectDDL1.SelectedValue = Talent.eCommerce.Utilities.CheckForDBNull_String(stc(0)("SPORT_CODE"))
                            Me.supporterSelectButton1_Click(New Object, New EventArgs)
                            supporterSelectDDL2.Items.Insert(0, ucr.Content("supporterSelectDDL2_PleaseSelect_Text", _languageCode, True))
                            supporterSelectDDL3.Items.Insert(0, ucr.Content("supporterSelectDDL3_PleaseSelect_Text", _languageCode, True))
                        Catch ex As Exception
                        End Try
                    End If
                End If
            End If

            supporterSelect1Hidden.Value = supporterSelectDDL1.SelectedValue
            supporterSelect2Hidden.Value = supporterSelectDDL2.SelectedValue
            supporterSelect3Hidden.Value = supporterSelectDDL3.SelectedValue

            'Stuart
            'favouritesDDL.SelectedValue = .FAVOURITE_TEAM_CODE
            Dim riCount As Integer = 1
            For Each ri As RepeaterItem In contactRepeater.Items
                Try
                    Select Case riCount
                        Case 1
                            CType(ri.FindControl("teamDDL"), DropDownList).SelectedValue = .MAIL_TEAM_CODE_1
                        Case 2
                            CType(ri.FindControl("teamDDL"), DropDownList).SelectedValue = .MAIL_TEAM_CODE_2
                        Case 3
                            CType(ri.FindControl("teamDDL"), DropDownList).SelectedValue = .MAIL_TEAM_CODE_3
                        Case 4
                            CType(ri.FindControl("teamDDL"), DropDownList).SelectedValue = .MAIL_TEAM_CODE_4
                        Case 5
                            CType(ri.FindControl("teamDDL"), DropDownList).SelectedValue = .MAIL_TEAM_CODE_5
                    End Select
                Catch ex As Exception
                End Try
                riCount += 1
            Next
            contactMethodDDL.SelectedValue = .PREFERRED_CONTACT_METHOD

            '---------------
            ' User ID Fields
            '---------------
            txtGreenCard.Text = .GreenCard
            txtPin.Text = .PIN
            txtPassport.Text = .Passport
            txtUserID4.Text = .User_ID_4
            txtUserID5.Text = .User_ID_5
            txtUserID6.Text = .User_ID_6
            txtUserID7.Text = .User_ID_7
            txtUserID8.Text = .User_ID_8
            txtUserID9.Text = .User_ID_9


        End With

        Dim address As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)



        With address

            If AddressFormat1Form1.Visible Then
                AddressFormat1Form1.ADF1_Street = .Address_Line_2
                AddressFormat1Form1.ADF1_Town = .Address_Line_5
                AddressFormat1Form1.ADF1_CityText = .Address_Line_4
                AddressFormat1Form1.ADF1_CountyText = .Address_Line_3

                AddressFormat1Form1.ADF1_Postcode = .Post_Code

                ' set drop downs from text fields
                AddressFormat1Form1.SetDropdownsForUpdateMode()

                If Not AddressFormat1Form1.ValidateAddress() Then
                    '' blank and force to re-enter..
                    'AddressFormat1Form1.ADF1_Street = String.Empty
                    AddressFormat1Form1.ADF1_Town = String.Empty
                    AddressFormat1Form1.ADF1_City = String.Empty
                    AddressFormat1Form1.ADF1_County = String.Empty
                    ' AddressFormat1Form1.ADF1_Postcode = String.Empty
                End If
                sequence.Text = .Sequence
            Else

                type.Text = .Type
                reference.Text = .Reference
                sequence.Text = .Sequence
                building.Text = .Address_Line_1
                street.Text = .Address_Line_2
                town.Text = .Address_Line_3
                city.Text = .Address_Line_4
                county.Text = .Address_Line_5
                postcode.Text = .Post_Code
            End If

            If .Country.Trim().Length > 0 Then
                Try
                    Dim i As Integer = 0
                    For Each li As ListItem In country.Items
                        If li.Value.ToLower = .Country.ToLower OrElse li.Text.ToLower = .Country.ToLower OrElse (li.Text.ToLower.Length > 20 AndAlso li.Text.ToLower.StartsWith(.Country.ToLower)) Then
                            country.SelectedIndex = i
                            Exit For
                        End If
                        i += 1
                    Next
                Catch
                End Try
            End If

        End With
        '--------------------------------------------
        ' Check if need to display registered address
        '--------------------------------------------
        Dim registeredAddress As New TalentProfileAddress
        Dim foundFirst As Boolean = False

        For Each profileAddress As Generic.KeyValuePair(Of String, TalentProfileAddress) In Profile.User.Addresses
            If Not foundFirst Then
                registeredAddress = profileAddress.Value
                foundFirst = True
            End If
            If profileAddress.Value.Type = "1" Then
                registeredAddress = profileAddress.Value
                Exit For
            End If
        Next

        If foundFirst AndAlso CBool(ucr.Attribute("DisplayRegisteredPanel")) AndAlso
            Not Session("Agent") Is Nothing AndAlso Session("Agent").ToString <> String.Empty Then
            plhRegisteredAddress.Visible = True
            txtRegAddress1.Text = registeredAddress.Address_Line_1
            txtRegAddress2.Text = registeredAddress.Address_Line_2
            txtRegAddress3.Text = registeredAddress.Address_Line_3
            txtRegAddress4.Text = registeredAddress.Address_Line_4
            txtRegAddress5.Text = registeredAddress.Address_Line_5
            txtRegPostcode.Text = registeredAddress.Post_Code

            Try
                Dim i As Integer = 0
                For Each li As ListItem In country.Items
                    If li.Value.ToLower = registeredAddress.Country.ToLower OrElse li.Text.ToLower = registeredAddress.Country.ToLower Then
                        ddlRegCountry.SelectedIndex = i
                        Exit For
                    End If
                    i += 1
                Next
            Catch
            End Try
            '-------------------------------------------------------------------
            ' If Registered Address is the same as main address then hide fields
            '-------------------------------------------------------------------
            If registeredAddress.Address_Line_1.ToUpper = address.Address_Line_1.ToUpper AndAlso
                registeredAddress.Address_Line_2.ToUpper = address.Address_Line_2.ToUpper AndAlso
                registeredAddress.Address_Line_3.ToUpper = address.Address_Line_3.ToUpper AndAlso
                registeredAddress.Address_Line_4.ToUpper = address.Address_Line_4.ToUpper AndAlso
                registeredAddress.Address_Line_5.ToUpper = address.Address_Line_5.ToUpper AndAlso
                checkPostcodesAreTheSame(registeredAddress.Post_Code, address.Post_Code) Then

                chkRegAddressSame.Checked = True
                chkRegAddressSame_CheckedChanged(chkRegAddressSame, New System.EventArgs)
            End If
        Else
            plhRegisteredAddress.Visible = False
        End If
    End Sub

    Protected Function checkPostcodesAreTheSame(ByVal regPostcode As String, ByVal mainPostcode As String) As Boolean
        Dim postcodesMatch As Boolean = False
        If regPostcode <> String.Empty Then
            regPostcode = Replace(regPostcode, " ", String.Empty)
        End If
        If mainPostcode <> String.Empty Then
            mainPostcode = Replace(mainPostcode, " ", String.Empty)
        End If
        If regPostcode.ToUpper = mainPostcode.ToUpper Then
            postcodesMatch = True
        End If
        Return postcodesMatch
    End Function

    Protected Function CheckPostcode() As Boolean
        Dim valid As Boolean = True
        Dim cacheKey As String = "Countries_Bu_Table_For_Postcode_Validation_MYACCOUNT_" & TalentCache.GetBusinessUnit & "_" & TalentCache.GetPartner(Profile)
        Dim countries As New TalentApplicationVariablesTableAdapters.tbl_country_bu1TableAdapter
        Dim myCountries As New TalentApplicationVariables.tbl_country_bu1DataTable
        If TalentThreadSafe.ItemIsInCache(cacheKey) Then
            myCountries = CType(Cache.Item(cacheKey), TalentApplicationVariables.tbl_country_bu1DataTable)
        Else
            myCountries = countries.GetDataByQualifierBUPartner("MYACCOUNT", TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
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
            TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
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

    Protected Function UpdateProfile() As Boolean

        Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

        If (plhDOBRow.Visible AndAlso (Profile.User.Details.DOB = (New Date(1900, 1, 1)) OrElse Profile.User.Details.DOB = (New Date(1900, 2, 1)) OrElse Profile.User.Details.DOB = Date.MinValue)) Then
            Try
                Profile.User.Details.DOB = New Date(CInt(dobYear.SelectedItem.Value), CInt(dobMonth.SelectedItem.Value), CInt(dobDay.SelectedItem.Value))
            Catch ex As Exception
                ErrorLabel.Text = ucr.Content("InvalidDOBErrorText", _languageCode, True)
                Return False
            End Try
        End If

       

        ' Check User IDs are required
        If plhUserIDs.Visible AndAlso def.AgeAtWhichIDFieldsAreMandatory >= 0 Then
            Dim nowMinus14 As DateTime = Today
            nowMinus14 = nowMinus14.AddYears(-def.AgeAtWhichIDFieldsAreMandatory)
            Dim formDate As DateTime
            formDate = New Date(CInt(dobYear.SelectedItem.Value), CInt(dobMonth.SelectedItem.Value), CInt(dobDay.SelectedItem.Value))
            If formDate <= nowMinus14 Then
                If String.IsNullOrEmpty(txtPin.Text) _
                    AndAlso String.IsNullOrEmpty(txtPassport.Text) _
                    AndAlso String.IsNullOrEmpty(txtGreenCard.Text) _
                    AndAlso String.IsNullOrEmpty(txtUserID4.Text) Then
                    ErrorLabel.Text = ucr.Content("IDRequiredFieldValidator", _languageCode, True)
                    Return False
                End If
                If def.UserIDValidationTypeforID3 = "1" AndAlso
                        Not String.IsNullOrEmpty(txtPassport.Text.Trim) _
                         AndAlso Not String.IsNullOrEmpty(txtGreenCard.Text.Trim) Then
                    ErrorLabelID.Text = ucr.Content("MoreThanOneIDError", _languageCode, True)
                    Return False
                End If

            End If
        End If

        ' Validate the page in case javascript is turned off
        ' Need to turn off validators for parental contact fields because javascript disable doesn't seem to work for page.validate   
        parentemailRFV.Enabled = False
        parentphoneRFV.Enabled = False
        Page.Validate("Registration")
        If Not Page.IsValid Then
            Return False
        End If


        If LCase(email.Text) = LCase(def.DefaultTicketingEmailAddress) Then
            ErrorLabel.Text = ucr.Content("InvalidUsernameError", _languageCode, True)
            Return False
        End If

        If UCase(Profile.UserName) <> UCase(email.Text) Then
            If Not Membership.GetUser(email.Text, False) Is Nothing And Not CType(def.AllowDuplicateEmail, Boolean) Then
                ErrorLabel.Text = ucr.Content("UserAlreadyExistsErrorText", _languageCode, True)
                Return False
            Else
                'If Not CType(def.LoginidIsCustomerNumber, Boolean) Then
                If Not def.LoginidType.Equals("1") And Not def.LoginidType.Equals("2") Then
                    CType(Membership.Provider, TalentMembershipProvider).ChangeLoginID(Profile.UserName, email.Text)
                End If
            End If
        End If

        With Profile.User.Details

            'If Not CType(def.LoginidIsCustomerNumber, Boolean) Then .LoginID = email.Text
            Select Case def.LoginidType
                Case Is = "1"
                Case Is = "2"
                Case Else
                    .LoginID = email.Text
            End Select

            If mailOption.Checked = mailOptionTrueValue Then
                .Bit0 = "1"
            Else
                .Bit0 = "0"
            End If
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

            .Email = email.Text
            .Title = title.SelectedItem.Text
            .Forename = forename.Text
            .Initials = initials.Text
            .Surname = surname.Text

            .CUSTOMER_SUFFIX = customersuffix.Text
            .NICKNAME = nickname.Text
            .USERNAME = altusername.Text
            .Account_No_2 = contactsl.Text
            .Salutation = salutation.Text
            .Position = position.Text
            .CompanyName = txtCompanyName.Text
            .Mothers_Name = mothersname.Text
            .Fathers_Name = fathersname.Text
            ' Added for box office  
            .CONTACT_BY_EMAIL = cbContactbyEmail.Checked
            .CONTACT_BY_MOBILE = cbContactbyMobile.Checked
            .CONTACT_BY_POST = cbContactbyPost.Checked
            .CONTACT_BY_TELEPHONE_HOME = cbContactbyTelephoneHome.Checked
            .CONTACT_BY_TELEPHONE_WORK = cbContactbyTelephoneWork.Checked
            .STOP_CODE = ddlStopcode.SelectedValue
            .PRICE_BAND = ddlpriceband.SelectedValue
            .BOOK_NUMBER = BookNumber.Text
            .PARENT_EMAIL = parentemail.Text
            .PARENT_PHONE = parentphone.Text


            .PARENTAL_CONSENT_STATUS = String.Empty
            If DateTime.Compare(.DOB, _ParentalConsentCeilingDob) > 0 Then
                If cbParentalPermissionGranted.Checked = True Then
                    .PARENTAL_CONSENT_STATUS = "G"
                Else
                    .PARENTAL_CONSENT_STATUS = "R"
                End If
            End If

                .Full_Name = forename.Text & " " & surname.Text
                If CType(Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(def.ProfileFullNameWithTitle), Boolean) Then
                    If title.SelectedValue.Trim.Length > 0 Then
                        .Full_Name = title.SelectedItem.Text.Trim & " " & forename.Text.Trim & " " & surname.Text.Trim
                    End If
                End If

                If (plhDOBRow.Visible) Then
                    Try
                        .DOB = New Date(CInt(dobYear.SelectedItem.Value), CInt(dobMonth.SelectedItem.Value), CInt(dobDay.SelectedItem.Value))
                        '------------------------------
                        ' Check if DOB is in the future
                        '------------------------------
                        If .DOB > Date.Today Then
                            ErrorLabel.Text = ucr.Content("InvalidDOBErrorText", _languageCode, True)
                            Return False
                        End If
                    Catch ex As Exception
                        ErrorLabel.Text = ucr.Content("InvalidDOBErrorText", _languageCode, True)
                        Return False
                    End Try
                End If

                If Not CheckPostcode() Then
                    ErrorLabel.Text = ucr.Content("PostcodeRequiredFieldValidator", _languageCode, True)
                    Return False
                End If

                If AddressFormat1Form1.Visible Then
                    If Not AddressFormat1Form1.ValidateAddress() Then
                        ErrorLabel.Text = ucr.Content("addressFormat1Errors", _languageCode, True)
                        Return False
                    End If
                End If

                .Mobile_Number = mobile.Text
                .Telephone_Number = phone.Text
                .Work_Number = work.Text
                .Fax_Number = fax.Text
                .Other_Number = other.Text
                .Subscribe_Newsletter = Newsletter.Checked
                .HTML_Newsletter = GetNewsletterType()
                .Sex = sex.SelectedValue
                .Subscribe_2 = Subscribe2.Checked
                .Subscribe_3 = Subscribe3.Checked

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

                '---------------
                ' User ID Fields
                '---------------
                .GreenCard = txtGreenCard.Text.ToUpper
                .PIN = txtPin.Text.ToUpper
                .Passport = txtPassport.Text.ToUpper
                .User_ID_4 = txtUserID4.Text.ToUpper
                .User_ID_5 = txtUserID5.Text.ToUpper
                .User_ID_6 = txtUserID6.Text.ToUpper
                .User_ID_7 = txtUserID7.Text.ToUpper
                .User_ID_8 = txtUserID8.Text.ToUpper
                .User_ID_9 = txtUserID9.Text.ToUpper
        End With

        Dim address As TalentProfileAddress

        If Profile.UserName <> email.Text And Not def.LoginidType.Equals("1") And Not def.LoginidType.Equals("2") Then
            For i As Integer = 0 To Profile.User.Addresses.Count - 1
                address = ProfileHelper.ProfileAddressEnumerator(i, Profile.User.Addresses)

                With address
                    .LoginID = email.Text
                End With
            Next
        End If
        address = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)

        With address
            Session("oldAddress_Line_1") = .Address_Line_1
            Session("oldAddress_Line_2") = .Address_Line_2
            Session("oldAddress_Line_3") = .Address_Line_3
            Session("oldAddress_Line_4") = .Address_Line_4
            Session("oldAddress_Line_5") = .Address_Line_5
            Session("oldCountry") = .Country
            Session("oldPost_Code") = .Post_Code
            .Type = type.Text
            .Reference = reference.Text
            .Sequence = sequence.Text
            If Not Session("AddressFormat") Is Nothing AndAlso Session("AddressFormat") = "1" Then
                .Address_Line_1 = String.Empty
                .Address_Line_2 = AddressFormat1Form1.ADF1_Street
                .Address_Line_3 = AddressFormat1Form1.ADF1_County
                .Address_Line_4 = AddressFormat1Form1.ADF1_City
                .Address_Line_5 = AddressFormat1Form1.ADF1_Town
                .Post_Code = UCase(AddressFormat1Form1.ADF1_Postcode)
                If def.StoreCountryAsWholeName Then
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
                If def.StoreCountryAsWholeName Then
                    .Country = UCase(country.SelectedItem.Text)
                Else
                    .Country = country.SelectedValue
                End If
            End If
            Session("newAddress_Line_1") = .Address_Line_1
            Session("newAddress_Line_2") = .Address_Line_2
            Session("newAddress_Line_3") = .Address_Line_3
            Session("newAddress_Line_4") = .Address_Line_4
            Session("newAddress_Line_5") = .Address_Line_5
            Session("newCountry") = .Country
            Session("newPost_Code") = .Post_Code
        End With

        '--------------------------
        ' Update REGISTERED address
        '--------------------------
        Dim registeredAddress As New TalentProfileAddress
        Dim registeredCountry As String = String.Empty
        If plhRegisteredAddress.Visible Then
            If chkRegAddressSame.Checked Then
                txtRegAddress1.Text = address.Address_Line_1
                txtRegAddress2.Text = address.Address_Line_2
                txtRegAddress3.Text = address.Address_Line_3
                txtRegAddress4.Text = address.Address_Line_4
                txtRegAddress5.Text = address.Address_Line_5
                txtRegPostcode.Text = address.Post_Code
                registeredCountry = address.Country

            End If
            Dim registeredReference As String = "REGISTERED " & txtRegAddress1.Text.Trim & " " & txtRegAddress2.Text.Trim

            If registeredCountry = String.Empty Then
                If def.StoreCountryAsWholeName Then
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

            'The registered address may not exist.  Let's see if it does!!!!!!!!!!!!
            Dim foundIt As Boolean = False

            For Each profileAddress As Generic.KeyValuePair(Of String, TalentProfileAddress) In Profile.User.Addresses
                If profileAddress.Value.Type = "1" Then
                    foundIt = True
                    Exit For
                End If
            Next

            Dim addressData As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter

            'Update\Create the registered address
            If Not foundIt Then
                addressData.AddAddress(TalentCache.GetPartner(Profile),
                                   Profile.User.Details.LoginID,
                                   "1",
                                   registeredReference,
                                   0,
                                   False,
                                   txtRegAddress1.Text,
                                   txtRegAddress2.Text,
                                   txtRegAddress3.Text,
                                   txtRegAddress4.Text,
                                   txtRegAddress5.Text,
                                   txtRegPostcode.Text,
                                   registeredCountry,
                                   String.Empty,
                                   String.Empty)
            Else

                addressData.UpdateAddressByLoginidTypeSequence(txtRegAddress1.Text,
                                   txtRegAddress2.Text,
                                   txtRegAddress3.Text,
                                   txtRegAddress4.Text,
                                   txtRegAddress5.Text,
                                   txtRegPostcode.Text,
                                   registeredCountry,
                                   Profile.User.Details.LoginID,
                                   TalentCache.GetPartner(Profile),
                                   "1",
                                   0)

            End If

        End If

        If def.SendRegistrationToBackendFirst Then
            If def.SendRegistrationToBackEnd Then
                Try
                    SendDetailsToBackend("UPDATE", Profile.User.Details, address, registeredAddress)
                    Profile.Save()
                Catch ex As Exception
                    Return False
                End Try
            End If
        Else
            If def.SendRegistrationToBackEnd Then
                Try
                    Profile.Save()
                    SendDetailsToBackend("UPDATE", Profile.User.Details, address, registeredAddress)
                Catch ex As Exception
                    Return False
                End Try
            Else
                Profile.Save()
            End If
        End If


        'Was the back-end call a success?
        If ErrorLabel.Text.Trim = "" Then
            Return True
        Else
            Return False
        End If

        Return True

    End Function

    Protected Sub updateBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles updateBtn.Click

        Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

        ErrorLabel.Text = ""
        Try
            If UpdateProfile() Then

                TEBUtilities.addCustomerLoggedInToSession(def.NoOfRecentlyUsedCustomers, Profile.User.Details.LoginID, Profile.User.Details.Forename, Profile.User.Details.Surname)

                If Not AgentProfile.IsAgent AndAlso Not def.LoginidType.Equals("1") Then
                    Try
                        FormsAuthentication.SignOut()
                        ' Use login id not email
                        Dim tmu As TalentMembershipUser = Membership.Provider.GetUser(Profile.User.Details.LoginID, True)
                        'FormsAuthentication.Authenticate(Profile.User.Details.LoginID, tmu.Password)
                        Membership.ValidateUser(Profile.User.Details.LoginID, tmu.Password)
                        Profile.Initialize(Profile.User.Details.LoginID, True)
                        FormsAuthentication.SetAuthCookie(Profile.User.Details.LoginID, False)
                    Catch ex As Exception
                    End Try
                End If
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
                        url += "customer=" & Profile.User.Details.LoginID.Trim
                        Response.Redirect("~/Redirect/TicketingGateway.aspx?page=Registration.aspx&function=AddFreeMembership&ReturnUrl=" & Server.UrlEncode(url))
                    End If
                Else
                End If
                Dim redirectURL As String = String.Empty
                If otherMembersatAddress AndAlso Talent.eCommerce.Utilities.IsAgent AndAlso addressHasChanged() Then
                    redirectURL = "~/PagesAgent/Profile/AddressChange.aspx"
                ElseIf Not String.IsNullOrWhiteSpace(Request.QueryString("ReturnUrl")) Then
                    redirectURL = Request.QueryString("ReturnUrl")
                ElseIf Not String.IsNullOrWhiteSpace(Session("RedirectUrl")) Then
                    redirectURL = Session("RedirectUrl")
                    Session.Remove("RedirectUrl")
                ElseIf Not String.IsNullOrWhiteSpace(ucr.Attribute("ConfirmationUrl")) Then
                    redirectURL = ucr.Attribute("ConfirmationUrl")
                    Session("UpdateDetailsConfirmMessageDisplay") = "TRUE"
                ElseIf String.IsNullOrWhiteSpace(redirectURL) Then
                    redirectURL = "~/PagesLogin/Profile/UpdateProfileConfirmation.aspx?addauto=" & Profile.User.Details.OWNS_AUTO_MEMBERSHIP.ToString.ToLower
                End If
                Response.Redirect(redirectURL)
            End If
        Catch ex As Exception
            ErrorLabel.Text = ucr.Content("ErrorUpdatingFrontEndRecords", _languageCode, True)
        End Try

    End Sub
    Protected Function addressHasChanged() As Boolean
        If Session("newAddress_Line_1") <> Session("oldAddress_Line_1") OrElse Session("newAddress_Line_2") <> Session("oldAddress_Line_2") OrElse Session("newAddress_Line_3") <> Session("oldAddress_Line_3") OrElse Session("newAddress_Line_4") <> Session("oldAddress_Line_4") OrElse Session("newAddress_Line_5") <> Session("oldAddress_Line_5") OrElse Session("newCountry") <> Session("oldCountry") OrElse Session("newPost_Code") <> Session("oldPost_Code") Then
            Return True
        Else
            Return False
        End If
    End Function

    Protected Sub btnPrintAddressLabelBottom_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrintAddressLabelBottom.Click
        printAddressLabel()
    End Sub

    Protected Sub btnPrintAddressLabelTop_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrintAddressLabelTop.Click
        printAddressLabel()
    End Sub
    Protected Sub printAddressLabel()
        Dim TalentAddressing As New TalentAddressing
        Dim err As New ErrorObj
        TalentAddressing.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        TalentAddressing.De.CustomerNumber = TalentAddressing.Settings.LoginId
        TalentAddressing.PrintAddressLabel()

        If TalentAddressing.ResultDataSet.Tables("Status") IsNot Nothing AndAlso TalentAddressing.ResultDataSet.Tables("Status").Rows.Count > 0 AndAlso TalentAddressing.ResultDataSet.Tables("Status").Rows(0)("ErrorOccurred") = "E" Then
            Dim errMsg As New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ucr.FrontEndConnectionString)
            Dim message As TalentErrorMessage = errMsg.GetErrorMessage(GlobalConstants.STARALLPARTNER, ucr.PageCode, "AL")
            ErrorLabel.Text = message.ERROR_MESSAGE
            If TalentAddressing.ResultDataSet.Tables("Status").Rows(0)("ReturnCode").ToString = "AO" Then
                message = errMsg.GetErrorMessage(GlobalConstants.STARALLPARTNER, ucr.PageCode, "AO")
                ErrorLabel.Text = message.ERROR_MESSAGE
            End If
        Else
            ErrorLabel.Text = ucr.Content("msgPrintAddressLabelSuccess", _languageCode, True)
        End If
    End Sub


    Protected Sub SendDetailsToBackend(ByVal call_origin As String,
                                    ByVal userDetails As TalentProfileUserDetails,
                                    ByVal userAddress As TalentProfileAddress,
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
            .DeV11 = deCustV11
            ' Reset the settings entity to a customer specific settings entity 
            .Settings = CType(New DECustomerSettings, DESettings)

            With deCustV1
                If Talent.eCommerce.Utilities.IsAgent Then
                    .Agent = Session("Agent").ToString
                End If
                .Action = ""
                .ThirdPartyContactRef = p.Details.User_Number
                .ThirdPartyCompanyRef1 = p.Details.User_Number
                .ThirdPartyCompanyRef1Supplement = p.Details.User_Number_Prefix
                .ThirdPartyCompanyRef2 = ""
                .DateFormat = "1"
                .MothersName = p.Details.Mothers_Name
                .FathersName = p.Details.Fathers_Name
                If def.LoginidType.Equals("1") Then
                    .CustomerNumber = p.Details.LoginID
                End If
                .ContactSurname = p.Details.Surname
                .Suffix = p.Details.CUSTOMER_SUFFIX
                .Nickname = p.Details.NICKNAME
                .AltUserName = p.Details.USERNAME
                .ContactSLAccount = p.Details.Account_No_2
                .ContactForename = p.Details.Forename
                .ContactTitle = p.Details.Title
                .ContactInitials = p.Details.Initials
                .Salutation = p.Details.Salutation
                .ParentEmail = p.Details.PARENT_EMAIL
                .ParentPhone = p.Details.PARENT_PHONE
                .ConsentStatus = p.Details.PARENTAL_CONSENT_STATUS


                If (plhDOBRow.Visible) Then
                    Try
                        .DateBirth = dobYear.SelectedItem.Value.ToString.PadLeft(2, "0") & dobMonth.SelectedItem.Value.ToString.PadLeft(2, "0") & dobDay.SelectedItem.Value.PadLeft(2, "0")
                    Catch ex As Exception
                        ErrorLabel.Text = ucr.Content("InvalidDOBErrorText", _languageCode, True)
                        Exit Sub
                    End Try
                End If

                .Gender = sex.SelectedValue
                .EmailAddress = p.Details.Email
                If p.Details.Position.Trim <> String.Empty Then
                    .PositionInCompany = p.Details.Position
                    .ProcessPositionInCompany = "1"
                End If

                .SLNumber1 = p.Details.Account_No_1
                .SLNumber2 = p.Details.Account_No_2

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
                If Not defs.AddressLine1RowVisible Then
                    .AddressLine1 = address.Address_Line_2.Trim
                Else
                    .AddressLine1 = address.Address_Line_1.Trim & " " & address.Address_Line_2.Trim
                End If
                .AddressLine2 = address.Address_Line_3.Trim
                .AddressLine3 = address.Address_Line_4.Trim
                .AddressLine4 = address.Address_Line_5.Trim
                .AddressLine5 = UCase(address.Country.Trim)
                .PostCode = address.Post_Code
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
                        .RegisteredAddress1 = registeredAddress.Address_Line_2
                    Else
                        .RegisteredAddress1 = registeredAddress.Address_Line_1.Trim & " " & registeredAddress.Address_Line_2.Trim
                    End If

                    .RegisteredAddress2 = registeredAddress.Address_Line_3
                    .RegisteredAddress3 = registeredAddress.Address_Line_4
                    .RegisteredAddress4 = registeredAddress.Address_Line_5
                    .RegisteredAddress5 = UCase(registeredAddress.Country)
                    .RegisteredPostcode = UCase(registeredAddress.Post_Code)
                    .RegisteredCountry = registeredAddress.Country
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
                End If

                .ProcessAttributes = "1"
                If plhNewsletter.Visible Then
                    If (Newsletter.Checked And CType(ucr.Attribute("newsletterCheckedMeansTrue"), Boolean)) Or
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

                .ProcessSubscription1 = "1"
                .ProcessSubscription2 = "1"
                .ProcessSubscription3 = "1"

                If Subscribe2.Visible Then
                    If (Subscribe2.Checked And CType(ucr.Attribute("subscribe2CheckedMeansTrue"), Boolean)) Or
                        (Not Subscribe2.Checked And Not CType(ucr.Attribute("subscribe2CheckedMeansTrue"), Boolean)) Then
                        .Attribute04 = ucr.Attribute("subscribe2Attribute")
                        .Attribute04Action = "A"
                    Else
                        .Attribute04 = ucr.Attribute("subscribe2Attribute")
                        .Attribute04Action = "D"
                    End If
                End If
                If Subscribe3.Visible Then
                    If (Subscribe3.Checked And CType(ucr.Attribute("subscribe3CheckedMeansTrue"), Boolean)) Or
                        (Not Subscribe3.Checked And Not CType(ucr.Attribute("subscribe3CheckedMeansTrue"), Boolean)) Then
                        .Attribute05 = ucr.Attribute("subscribe3Attribute")
                        .Attribute05Action = "A"
                    Else
                        .Attribute05 = ucr.Attribute("subscribe3Attribute")
                        .Attribute05Action = "D"
                    End If
                End If

                .SUPPORTER_CLUB_CODE = p.Details.SUPPORTER_CLUB_CODE
                .FAVOURITE_TEAM_CODE = p.Details.FAVOURITE_TEAM_CODE
                .FAVOURITE_SPORT = p.Details.FAVOURITE_SPORT
                .MAIL_TEAM_CODE_1 = p.Details.MAIL_TEAM_CODE_1
                .MAIL_TEAM_CODE_2 = p.Details.MAIL_TEAM_CODE_2
                .MAIL_TEAM_CODE_3 = p.Details.MAIL_TEAM_CODE_3
                .MAIL_TEAM_CODE_4 = p.Details.MAIL_TEAM_CODE_4
                .MAIL_TEAM_CODE_5 = p.Details.MAIL_TEAM_CODE_5
                .PREFERRED_CONTACT_METHOD = p.Details.PREFERRED_CONTACT_METHOD
                .MothersName = p.Details.Mothers_Name
                .FathersName = p.Details.Fathers_Name
                If Not String.IsNullOrEmpty(Session.Item("Agent")) Then
                    .Agent = Session.Item("Agent")
                End If

                .ProcessSupporterClubCode = "1"
                .ProcessFavouriteTeamCode = "1"
                .ProcessFavouriteSport = "1"
                .ProcessMailTeamCode1 = "1"
                .ProcessMailTeamCode2 = "1"
                .ProcessMailTeamCode3 = "1"
                .ProcessMailTeamCode4 = "1"
                .ProcessMailTeamCode5 = "1"
                .ProcessPreferredContactMethod = "1"
                .ProcessMothersName = "1"
                .ProcessFathersName = "1"

                .PhoneNoFormatting = def.PhoneNoFormat
                .PostCodeFormatting = def.PostCodeFormat

                .GreenCardNumber = p.Details.GreenCard

                .stopcodeforid3 = ""
                If def.UserIDValidationTypeforID3 = "1" AndAlso .GreenCardNumber <> String.Empty Then
                    .stopcodeforid3 = def.StopCodeForID3
                End If

                .PassportNumber = p.Details.Passport
                .PIN_Number = p.Details.PIN
                .User_ID_4 = p.Details.User_ID_4
                .User_ID_5 = p.Details.User_ID_5
                .User_ID_6 = p.Details.User_ID_6
                .User_ID_7 = p.Details.User_ID_7
                .User_ID_8 = p.Details.User_ID_8
                .User_ID_9 = p.Details.User_ID_9
                If p.Details.Subscribe_2 Then
                    .Subscription2 = "1"
                Else
                    .Subscription2 = "0"
                End If
                If p.Details.Subscribe_3 Then
                    .Subscription3 = "1"
                Else
                    .Subscription3 = "0"
                End If
                ' Fields added for box office
                .StopCode = ddlStopcode.SelectedValue
                .PriceBand = ddlpriceband.SelectedValue
                .ContactbyEmail = cbContactbyEmail.Checked
                .ContactbyTelephoneHome = cbContactbyTelephoneHome.Checked
                .ContactbyTelephoneWork = cbContactbyTelephoneWork.Checked
                .ContactbyMobile = cbContactbyMobile.Checked
                .ContactbyPost = cbContactbyPost.Checked

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

            If Not myErrorObj.HasError Then
                If .ResultDataSet.Tables.Count > 1 Then
                    Dim ownsAutoMember As String = ""
                    Try
                        ownsAutoMember = Talent.Common.Utilities.CheckForDBNull_String(.ResultDataSet.Tables(1).Rows(0)("OwnsAutoMembership"))
                        If Talent.Common.Utilities.CheckForDBNull_String(.ResultDataSet.Tables(1).Rows(0)("otherMembersatAddress")) = "Y" Then
                            otherMembersatAddress = True
                        End If
                    Catch ex As Exception
                    End Try
                    If ownsAutoMember.ToLower = "y" Then
                        Profile.User.Details.OWNS_AUTO_MEMBERSHIP = True
                    Else
                        Profile.User.Details.OWNS_AUTO_MEMBERSHIP = False
                    End If
                    If Not CType(def.SendRegistrationToBackendFirst, Boolean) Then
                        Profile.Save()
                    End If
                End If
            End If

            'Only serialize the customer when the call to the backend can be ignored
            If Not CType(def.SendRegistrationToBackendFirst, Boolean) Then
                If myErrorObj.HasError Then
                    Dim sendEmail As Boolean = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("SendBackEndFailureEmail"))
                    Talent.eCommerce.Utilities.SerializeObject(myCustomer,
                                                                myCustomer.GetType,
                                                                TalentCache.GetBusinessUnit,
                                                                TalentCache.GetPartner(Profile),
                                                                Profile.UserName,
                                                                call_origin,
                                                                sendEmail,
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

    Protected Function SendDetailsToBackendFirst(ByVal sendRegistrationToBackEndFirst As Boolean,
                                               ByVal userDetails As TalentProfileUserDetails,
                                               ByVal userAddress As TalentProfileAddress,
                                               ByVal registeredAddress As TalentProfileAddress) As Boolean

        Dim moduleDefaults As ECommerceModuleDefaults = New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

        ' Are we sending the customer information to the back end first
        If Not CType(sendRegistrationToBackEndFirst, Boolean) Or
            Not CType(def.SendRegistrationToBackEnd, Boolean) Then
            Return True
        End If

        'Send the information to the backend 
        SendDetailsToBackend("UPDATE", userDetails, userAddress, registeredAddress)

        'Was the call a success?
        If ErrorLabel.Text.Trim = "" Then
            Return True
        Else
            Return False
        End If

    End Function

    Protected Sub SetAddressVisibility()

        Dim eComDefs As New ECommerceModuleDefaults
        Dim defs As ECommerceModuleDefaults.DefaultValues = eComDefs.GetDefaults

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

        AddressLine4Row.Visible = True
        cityRegEx.Enabled = True
        plhRegAddressLine4Row.Visible = True
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
            plhAddressSequenceRow.Visible = False
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
            AddressLine4Row.Visible = False
            cityRegEx.Enabled = False
            plhRegAddressLine4Row.Visible = False
            regexTxtRegAddress4.Enabled = False
        End If
        If Not addressLine5RowVisible Then
            plhAddressLine5Row.Visible = False
            countyRFV.Enabled = False
            countyRegEx.Enabled = False
            plhRegAddressLine5Row.Visible = False
            regexTxtRegAddress5.Enabled = False
        End If
        If Not addressPostcodeRowVisible Then
            plhAddressPostcodeRow.Visible = False
            postcodeRFV.Enabled = False
            postcodeRegEx.Enabled = False
            plhRegAddressPostcodeRow.Visible = False
            postcodeRFV.Enabled = False
            regexTxtRegAddress5.Enabled = False
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


    End Sub

    Protected Sub SetAddressVisibilityProperties()

        Dim eComDefs As New ECommerceModuleDefaults
        Dim defs As ECommerceModuleDefaults.DefaultValues = eComDefs.GetDefaults

        ' 
        ' Set common address field visibility using system defaults, and then override by any control-level defaults.
        If Not defs.AddressLine1RowVisible Then
            addressLine1RowVisible = False
        End If
        If Not defs.AddressLine2RowVisible Then
            addressLine2RowVisible = False
        End If
        If Not defs.AddressLine3RowVisible Then
            addressLine3RowVisible = False
        End If
        If Not defs.AddressLine4RowVisible Then
            addressLine4RowVisible = False
        End If
        If Not defs.AddressLine5RowVisible Then
            addressLine5RowVisible = False
        End If
        If Not defs.AddressPostcodeRowVisible Then
            addressPostcodeRowVisible = False
        End If
        If Not defs.AddressCountryRowVisible Then
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
        If defs.AddressFormat = "1" AndAlso (Session("AddressFormat") Is Nothing OrElse Session("AddressFormat") = "1") Then
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
            If defs.AddressFormat = "1" Then
                country.AutoPostBack = True
                If Not Session("AddressFormatCountry") Is Nothing Then
                    country.SelectedValue = Session("AddressFormatCountry")
                    Session("AddressFormatCountry") = Nothing
                End If
            End If
            AddressFormat1Form1.Visible = False
        End If
    End Sub

    Protected Function GetAddressingLinkText() As String
        Return ucr.Content("addressingLinkButtonText", _languageCode, True)
    End Function

    Protected Sub CreateAddressingJavascript()
        If CType(ucr.Attribute("addressingOnOff"), Boolean) Then
            Dim sString As String = String.Empty
            Dim sAllFields() As String = ModuleDefaults.AddressingFields.ToString.Split(",")
            Dim count As Integer = 0
            Response.Write(vbCrLf & "<script language=""javascript"" type=""text/javascript"">" & vbCrLf)
            Select Case ModuleDefaults.AddressingProvider.ToUpper
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
    End Sub

    Protected Function GetJavascriptString(ByVal sFieldString As String, ByVal sAddressingMap As String, ByVal sAddressingFields As String) As String
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
                        sString = sString & vbCrLf &
                                "if (trim(" & sStr3 & count2.ToString & ") != 'Y' && trim(" & sStr1 & count2.ToString & sStr2 & ") != '') {" & vbCrLf &
                                "if (trim(" & sFieldString & sStr2 & ") == '' || " & sFieldString & " == document.forms[0]." & country.UniqueID & ") {" & vbCrLf &
                                sFieldString & sStr2 & " = " & sStr1 & count2.ToString & sStr2 & ";" & vbCrLf &
                                "}" & vbCrLf &
                                "else {" & vbCrLf &
                                 sFieldString & sStr2 & " = " & sFieldString & sStr2 & " + ', ' + " & sStr1 & count2.ToString & sStr2 & ";" & vbCrLf &
                                "}" & vbCrLf &
                                sStr3 & count2.ToString & " = 'Y';" & vbCrLf &
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

    Protected Sub Setup_Supporter_Favourites_And_Club_Contact_Options()
        Try
            plhSupporter.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("SupporterSelectionsInUse"))
            plhContactMethod.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("ClubsContactSelectionInUse"))

            If plhSupporter.Visible Then
                supporterSelectLabel1.Text = ucr.Content("supporterSelectLabel1_Text", _languageCode, True)
                supporterSelectLabel2.Text = ucr.Content("supporterSelectLabel2_Text", _languageCode, True)
                supporterSelectLabel3.Text = ucr.Content("supporterSelectLabel3_Text", _languageCode, True)
                supporterSelectButton1.Text = ucr.Content("supporterSelectButton1_Text", _languageCode, True)
                supporterSelectButton2.Text = ucr.Content("supporterSelectButton2_Text", _languageCode, True)
                PopulateSupporterSportDDL()
            End If

            If plhContactMethod.Visible Then
                contactMethodLabel.Text = ucr.Content("contactMethodLabel_Text", _languageCode, True)
                PopulateSportTeamMailFlags()
                PopulateContactMethodDDL()
            End If
        Catch ex As Exception
        End Try
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
                    ddl.Items.Insert(0, New ListItem(ucr.Content("MailFlagsTeam_SelectNone_Text", _languageCode, True), " "))
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
    End Sub

    Protected Sub btnCapturePhoto_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles capturePhotoBtn.Load
        Dim photoProgramUrl As String = ucr.Attribute("PhotoProgramCaptureUrl")
        If photoProgramUrl.Contains("http://") Then
            Dim querystring As New StringBuilder
            querystring.Append("fileName=")
            If Not Profile.IsAnonymous AndAlso Not String.IsNullOrEmpty(Profile.User.Details.Account_No_1.ToString.Trim) Then
                querystring.Append(Profile.User.Details.Account_No_1.ToString.Trim)
            End If
            querystring.Append("&folderPath=")
            querystring.Append(ucr.Attribute("PhotoFolderPath"))
            querystring.Append("&photoEffects=")
            querystring.Append(ucr.Attribute("PhotoEffects"))
            querystring.Append("&exposureMode=")
            querystring.Append(ucr.Attribute("ExposureMode"))
            querystring.Append("&AFDistance=")
            querystring.Append(ucr.Attribute("AFDistance"))
            querystring.Append("&meteringMode=")
            querystring.Append(ucr.Attribute("MeteringMode"))
            querystring.Append("&imageSize=")
            querystring.Append(ucr.Attribute("ImageSize"))
            querystring.Append("&imageQuality=")
            querystring.Append(ucr.Attribute("ImageQuality"))
            querystring.Append("&zoomLevel=")
            querystring.Append(ucr.Attribute("ZoomLevel"))
            querystring.Append("&brightnessLevel=")
            querystring.Append(ucr.Attribute("BrightnessLevel"))
            Dim buttonClickValue As String = "location.href='" + photoProgramUrl + "?" + querystring.ToString + "'"
            capturePhotoBtn.Attributes.Add("onclick", buttonClickValue)
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
            ddlRegCountry.SelectedValue = country.SelectedValue
        End If
        SetupRegRequiredValidator(rfvTxtRegAddress1, New System.EventArgs)
        SetupRegRequiredValidator(rfvTxtRegAddress2, New System.EventArgs)
        SetupRegRequiredValidator(rfvTxtRegAddress3, New System.EventArgs)
        SetupRegRequiredValidator(rfvTxtRegAddress4, New System.EventArgs)
        SetupRegRequiredValidator(rfvTxtRegAddress5, New System.EventArgs)
        SetupRegRequiredValidator(rfvTxtRegPostcode, New System.EventArgs)

    End Sub

    Protected Sub country_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles country.SelectedIndexChanged
        ' if turkish then show turkey address format
        Dim eComDefs As New ECommerceModuleDefaults
        Dim defs As ECommerceModuleDefaults.DefaultValues = eComDefs.GetDefaults
        If defs.AddressFormat = "1" AndAlso country.SelectedValue.Trim = "TR" Then
            Session("AddressFormat") = "1"
            Session("AddressFormatCountry") = country.SelectedValue
        End If
    End Sub
    Private Sub SetAttributes()
        With ucr
            plhBookNumber.Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(.Attribute("ShowBookNumber"))
            If AgentProfile.IsAgent Then
                plhPrintAddressLabelBottom.Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(.Attribute("DisplayPrintAddressLabelButtonBottom"))
                plhPrintAddressLabelTop.Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(.Attribute("DisplayPrintAddressLabelButtonTop"))
            Else
                plhPrintAddressLabelBottom.Visible = False
                plhPrintAddressLabelTop.Visible = False
            End If
        End With
    End Sub
End Class
