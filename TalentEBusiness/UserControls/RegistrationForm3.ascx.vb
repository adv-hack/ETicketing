Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Data

'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Registration Form 3
'
'       Date                        Sept 2009
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
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_RegistrationForm3
    Inherits ControlBase

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private moduleName As String = "Registration"
    Private ucr As New Talent.Common.UserControlResource
    Private errMsg As Talent.Common.TalentErrorMessages
    Dim myDefs As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults

    'Need to default to true because the other controls that use this one may not set the display property
    Public Display As Boolean = True

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Display Then
            SetUpUCR()
            If Not Page.IsPostBack Then
                ResetSections()
            End If
        Else
            Me.Visible = False
        End If
    End Sub

    Protected Sub ResetSections()
        Panel1.Visible = True
        Panel2.Visible = False
        Panel3.Visible = False
        Panel4.Visible = False
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Me.Display Then
            SetUpUCR()
            If Not Page.IsPostBack Then
                PopulateCountriesDropDownList()
                SetLabelText()
                SetControlForRegistration()
            End If
        End If


    End Sub

    Protected Sub SetUpUCR()
        If Me.Display Then
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.Common.Utilities.GetAllString
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "RegistrationForm3.ascx"
            End With
        End If
    End Sub

    Protected Sub SetControlForRegistration()
        If Me.Display Then
            registerBtn.Visible = True
        End If
    End Sub

    Private Sub SetLabelText()
        If Me.Display Then
            Try

                With ucr

                    RegistrationHeaderLabel.Text = .Content("registrationHeaderLabel", _languageCode, True)              ' Registration Title
                    registerBtn.Text = .Content("registerBtnText", _languageCode, True)                                  ' Register Button

                    companyDtlsTitleLabel.Text = .Content("companyDtlsTitleLabel", _languageCode, True)                  ' Company Details Title
                    companyNameLabel.Text = .Content("companyNameLabel", _languageCode, True)                            ' Company Name
                    deliveryAddressTitleLabel.Text = .Content("deliveryAddressTitleLabel", _languageCode, True)          ' Address Details Title
                    add1Label.Text = .Content("add1Label", _languageCode, True)                                          ' Address Line 1
                    add2Label.Text = .Content("add2Label", _languageCode, True)                                          ' Address Line 2
                    add3Label.Text = .Content("add3Label", _languageCode, True)                                          ' Address Line 3
                    add4Label.Text = .Content("add4Label", _languageCode, True)                                          ' Address Line 4
                    add5Label.Text = .Content("add5Label", _languageCode, True)                                          ' Address Line 5
                    postcodeLabel.Text = .Content("postcodeLabel", _languageCode, True)                                  ' Postcode 
                    countryLabel.Text = .Content("countryLabel", _languageCode, True)                                    ' Country
                    phoneLabel.Text = .Content("phoneLabel", _languageCode, True)                                        ' Phone
                    faxLabel.Text = .Content("faxLabel", _languageCode, True)                                            ' Fax
                    emailLabel.Text = .Content("emailLabel", _languageCode, True)                                        ' Email
                    emailConfirmLabel.Text = .Content("emailConfirmLabel", _languageCode, True)                          ' Email Confirmation
                    openingTimesLabel.Text = .Content("openingTimesLabel", _languageCode, True)                          ' Opening Times
                    estAnnExpLabel.Text = .Content("estAnnExpLabel", _languageCode, True)                                ' Estimated Annual Expenditure
                    credLimLabel.Text = .Content("credLimLabel", _languageCode, True)                                    ' Credit Limit
                    dlvryInstLabel.Text = .Content("dlvryInstLabel", _languageCode, True)                                ' Delivery Instructions
                    partNCCLabel.Text = .Content("partNCCLabel", _languageCode, True)                                    ' Part of Group?
                    expCodeLabel.Text = .Content("expCodeLabel", _languageCode, True)                                    ' Expenditure Code
                    costCenLabel.Text = .Content("costCenLabel", _languageCode, True)                                    ' Cost Centre

                    contactDtlsTitleLabel.Text = .Content("contactDtlsTitleLabel", _languageCode, True)                  ' Contact Details Title
                    mainContactNameLabel.Text = .Content("mainContactNameLabel", _languageCode, True)                    ' Contact Name 
                    mainContactPosLabel.Text = .Content("mainContactPosLabel", _languageCode, True)                      ' Contact Postion
                    mainContactTelLabel.Text = .Content("mainContactTelLabel", _languageCode, True)                      ' Contact Tel 
                    mainContactExtLabel.Text = .Content("mainContactExtLabel", _languageCode, True)                      ' Contact Extension
                    scndContactNameLabel.Text = .Content("scndContactNameLabel", _languageCode, True)                    ' Contact Name
                    scndContactPosLabel.Text = .Content("scndContactPosLabel", _languageCode, True)                      ' Contact Postion
                    scndContactTelLabel.Text = .Content("scndContactTelLabel", _languageCode, True)                      ' Contact Tel 
                    scndContactExtLabel.Text = .Content("scndContactExtLabel", _languageCode, True)                      ' Contact Extension


                    'JDW - 31/11/2009 New Field Texts
                    '-------------------------------------
                    nextButton1.Text = .Content("nextButtonText", _languageCode, True)
                    nextButton2.Text = .Content("nextButtonText", _languageCode, True)
                    nextButton3.Text = .Content("nextButtonText", _languageCode, True)
                    prevButton2.Text = .Content("prevButtonText", _languageCode, True)
                    prevButton3.Text = .Content("prevButtonText", _languageCode, True)
                    prevButton4.Text = .Content("prevButtonText", _languageCode, True)


                    'Section 2: Bank Details
                    '----------------------------------
                    bankTitleLabel.Text = .Content("bankDetailsTitleLabel", _languageCode, True)

                    nameOfBankLabel.Text = .Content("nameOfBankLabel", _languageCode, True)
                    addressOfBankLabel.Text = .Content("addressOfBankLabel", _languageCode, True)
                    bankPostcodeLabel.Text = .Content("bankPostcodeLabel", _languageCode, True)
                    accNameLabel.Text = .Content("accNameLabel", _languageCode, True)
                    sortCodeLabel.Text = .Content("sortCodeLabel", _languageCode, True)
                    accNoLabel.Text = .Content("accNoLabel", _languageCode, True)
                    bankCheckAgree.Text = .Content("bankCheckLabel", _languageCode, True)

                    '----------------------------------


                    'Section 3: Trade References
                    '----------------------------------
                    tradeTitleLabel.Text = .Content("tradeReferencesTitleLabel", _languageCode, True)

                    supplier1NameLabel.Text = .Content("supplier1NameLabel", _languageCode, True)
                    supplier1AddressLabel.Text = .Content("supplier1AddressLabel", _languageCode, True)
                    supplier1PostcodeLabel.Text = .Content("supplier1PostcodeLabel", _languageCode, True)
                    supplier1TelLabel.Text = .Content("supplier1TelBox", _languageCode, True)
                    supplier1FaxLabel.Text = .Content("supplier1FaxLabel", _languageCode, True)
                    supplier1AccNoLabel.Text = .Content("supplier1AccNoLabel", _languageCode, True)

                    supplier2NameLabel.Text = .Content("supplier2NameLabel", _languageCode, True)
                    supplier2AddressLabel.Text = .Content("supplier2AddressLabel", _languageCode, True)
                    supplier2PostcodeLabel.Text = .Content("supplier2PostcodeLabel", _languageCode, True)
                    supplier2TelLabel.Text = .Content("supplier2TelBox", _languageCode, True)
                    supplier2FaxLabel.Text = .Content("supplier2FaxLabel", _languageCode, True)
                    supplier2AccNoLabel.Text = .Content("supplier2AccNoLabel", _languageCode, True)


                    'Section 4: Organistation
                    '----------------------------------
                    orgTitleLabel.Text = .Content("organisationTypesTitleLabel", _languageCode, True)

                    q1Label.Text = .Content("organisationQ1Text", _languageCode, True)
                    q2Label.Text = .Content("organisationQ2Text", _languageCode, True)
                    q3Label.Text = .Content("organisationQ3Text", _languageCode, True)
                    q4Label.Text = .Content("organisationQ4Text", _languageCode, True)

                    With q1RadioList.Items
                        .Clear()
                        For i As Integer = 1 To 10
                            If Not String.IsNullOrEmpty(ucr.Content("organisationQ1Option" & i.ToString("00"), _languageCode, True)) Then _
                                        .Add(ucr.Content("organisationQ1Option" & i.ToString("00"), _languageCode, True))
                        Next
                    End With
                    q1CharityLabel.Text = .Content("organisationQ1CharityLabel", _languageCode, True)


                    With q2RadioList.Items
                        .Clear()
                        For i As Integer = 1 To 10
                            If Not String.IsNullOrEmpty(ucr.Content("organisationQ2Option" & i.ToString("00"), _languageCode, True)) Then _
                                        .Add(ucr.Content("organisationQ2Option" & i.ToString("00"), _languageCode, True))
                        Next
                    End With


                    With q3CheckBoxList.Items
                        .Clear()
                        For i As Integer = 1 To 10
                            If Not String.IsNullOrEmpty(ucr.Content("organisationQ3Option" & i.ToString("00"), _languageCode, True)) Then _
                                        .Add(ucr.Content("organisationQ3Option" & i.ToString("00"), _languageCode, True))
                        Next
                    End With

                    q4CatalogueCheck.Text = .Content("organisationQ4CatalogueOptionText", _languageCode, True)
                    q4OrgWorkLabel.Text = .Content("organisationQ4WorkTypeLabel", _languageCode, True)
                    q4HearAboutUsLabel.Text = .Content("organisationQ4HearAboutUsLabel", _languageCode, True)

                    tsandcsCheck.Text = .Content("tsAndCsCheckBoxText", _languageCode, True)
                    appNameLabel.Text = .Content("applicantNameLabel", _languageCode, True)
                    appPosLabel.Text = .Content("applicantPositionLabel", _languageCode, True)

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

        Return errMsg.GetErrorMessage(moduleName, _
                            Talent.eCommerce.Utilities.GetCurrentPageName, _
                            errCode).ERROR_MESSAGE
    End Function

    Public Sub SetupCompareValidator(ByVal sender As Object, ByVal e As EventArgs)
        If Me.Display Then
            Dim cv As CompareValidator = CType(sender, CompareValidator)
            Try
                SetUpUCR()
                cv.ErrorMessage = ucr.Content(cv.ControlToValidate & "CompareValidator", _languageCode, True)
            Catch ex As Exception
                cv.Enabled = False
            End Try
        End If

    End Sub

    Public Sub SetupRequiredValidator(ByVal sender As Object, ByVal e As EventArgs)

        'JDW - 30/11/2009 - New RFVs
        '--------------------------------
        'nameOfBankBox 
        'addressOfBankBox 
        'bankPostcodeBox 
        'accNameBox 
        'sortCodeBox 
        'accNoBox
        'supplier1NameBox 
        'supplier1AddressBox 
        'supplier1PostcodeBox 
        'supplier1TelBox 
        'supplier1FaxBox 
        'supplier1AccNoBox 
        'supplier2NameBox 
        'supplier2AddressBox 
        'supplier2PostcodeBox 
        'supplier2TelBox 
        'supplier2FaxBox 
        'supplier2AccNoBox 
        'q4OrgWorkBox 
        'q4HearAboutUsBox 
        'appNameBox 
        'appPosBox

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
                rfv.Enabled = False
            End Try
        End If
    End Sub

    Protected Sub SetupRegExValidator(ByVal sender As Object, ByVal e As EventArgs)


        'JDW - 30/11/2009 - New RFVs
        '--------------------------------
        'nameOfBankBox 
        'addressOfBankBox 
        'accNameBox 
        'supplier1NameBox 
        'supplier1AddressBox 
        'supplier1AccNoBox 
        'supplier2NameBox 
        'supplier2AddressBox 
        'supplier2AccNoBox 
        'q1CharityBox 
        'q4OrgWorkBox 
        'q4HearAboutUsBox 
        'appNameBox 
        'appPosBox
        'bankPostcodeBox 
        'supplier1PostcodeBox 
        'supplier2PostcodeBox 
        'supplier1TelBox 
        'supplier1FaxBox 
        'supplier2TelBox 
        'supplier2FaxBox 
        'sortCodeBox 
        'accNoBox



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

                    Case Is = "companyName", _
                                "add1", _
                                "add2", _
                                "add3", _
                                "add4", _
                                "add5", _
                                "openingTimes", _
                                "dlvryInst", _
                                "mainContactPos", _
                                "scndContactPos", _
                                "nameOfBankBox", _
                                "addressOfBankBox", _
                                "accNameBox", _
                                "supplier1NameBox", _
                                "supplier1AddressBox", _
                                "supplier1AccNoBox", _
                                "supplier2NameBox", _
                                "supplier2AddressBox", _
                                "supplier2AccNoBox", _
                                "q1CharityBox", _
                                "q4OrgWorkBox", _
                                "q4HearAboutUsBox", _
                                "appNameBox", _
                                "appPosBox"
                        rev.ValidationExpression = ucr.Attribute("AlphaNumericExpression")

                    Case Is = "postcode", _
                                "bankPostcodeBox", _
                                "supplier1PostcodeBox", _
                                "supplier2PostcodeBox"
                        rev.ValidationExpression = ucr.Attribute("PostcodeExpression")

                    Case Is = "phone", _
                                "fax", _
                                "mainContactTel", _
                                "scndContactTel", _
                                "supplier1TelBox", _
                                "supplier1FaxBox", _
                                "supplier2TelBox", _
                                "supplier2FaxBox"
                        rev.ValidationExpression = ucr.Attribute("PhoneNumberExpression")

                    Case Is = "email"
                        rev.ValidationExpression = ucr.Attribute("EmailExpression")

                    Case Is = "mainContactExt", "scndContactExt", "sortCodeBox", "accNoBox"
                        rev.ValidationExpression = ucr.Attribute("NumberExpression")

                    Case Is = "mainContactName", "scndContactName", "country"
                        rev.ValidationExpression = ucr.Attribute("TextOnlyExpression")

                    Case Is = "estAnnExp", "credLim"
                        rev.ValidationExpression = ucr.Attribute("IntDecExpression")

                    Case Is = "costCen"
                        rev.ValidationExpression = ucr.Attribute("CostCentreExpression")
                        rev.Enabled = False

                End Select

                If rev.ValidationExpression = String.Empty Then
                    rev.Enabled = False
                End If
            End If
        End If
    End Sub

    Protected Sub PopulateCountriesDropDownList()
        If Me.Display Then
            country.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "REGISTRATION", "COUNTRY")
            country.DataTextField = "Text"
            country.DataValueField = "Value"
            country.DataBind()
            Dim defaultCountry As String = TalentCache.GetDefaultCountryForBU()
            'Select the default country
            If myDefs.UseDefaultCountryOnRegistration Then
                If defaultCountry <> String.Empty Then
                    country.SelectedValue = defaultCountry
                End If
            End If

            country.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "REGISTRATION", "COUNTRY")
            country.DataTextField = "Text"
            country.DataValueField = "Value"
            country.DataBind()
            If myDefs.UseDefaultCountryOnRegistration Then
                If defaultCountry <> String.Empty Then
                    country.SelectedValue = defaultCountry
                End If
            End If
        End If
    End Sub

    Protected Sub registerBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles registerBtn.Click
        If Me.Display Then

            '
            ' Validate the registration details the user has entered.
            '
            If validRegistration() Then
                '
                ' If the registration data is valid then email all registration details off to be processed.
                '
                SendRegistrationEmail()
                '
                ' Send a confirmation email to the web user. Indicating that the registration is going to be processed.
                '
                SendConfirmationEmail()
                '
                ' Forward onto Registration Confirmation Page
                '
                Response.Redirect("~/PagesPublic/Profile/RegistrationConfirmation.aspx")
            End If
        End If
    End Sub

    Function validRegistration() As Boolean

        'JDW - 30/11/2009 - New check required
        '--------------------------------
        'bankCheckList selection
        'tsandcs check box


        SetUpUCR()
        ErrorLabel.Text = String.Empty
        '
        ' If the 'organisation part of NCC checkbox is ticked, then the cost centre field is mandatory.
        '
        If partNCC.Checked And costCen.Text = String.Empty Then
            ErrorLabel.Text = getError("CostCentreMandatory")
            Return False
        End If

        '
        ' If the 'organisation part of NCC checkbox is unchecked, then the cost centre field should be blank.
        '
        If Not partNCC.Checked And Not costCen.Text = String.Empty Then
            ErrorLabel.Text = getError("CostCentreEmpty")
            Return False
        End If

        '
        ' If the cost centre field is entered then it must be a valid 7 char alphanumeric.
        '
        If costCen.Text <> String.Empty Then
            costCenRegEx.Enabled = True
            costCenRegEx.Validate()
            If Not costCenRegEx.IsValid Then
                ErrorLabel.Text = getError("CostCentreInvalid")
                costCenRegEx.Enabled = False
                Return False
            End If
            costCenRegEx.Enabled = False
        End If

        '
        ' The country field must be selected.
        '
        If country.Text.Trim = "--" Or country.Text.Trim = String.Empty Then
            ErrorLabel.Text = getError("CountryInvalid")
            Return False
        End If

        If Not tsandcsCheck.Checked Then
            ErrorLabel.Text = getError("RegistrationForm3TsAndCsError")
            Return False
        End If

        If q1RadioList.SelectedItem Is Nothing Then
            ErrorLabel.Text = getError("RegistrationForm3OrgTypeError")
            Return False
        End If

        If q2RadioList.SelectedItem Is Nothing Then
            ErrorLabel.Text = getError("RegistrationForm3EduTypeError")
            Return False
        End If

        Dim selectionCheck As Boolean = False
        For Each li As ListItem In q3CheckBoxList.Items
            If li.Selected Then
                selectionCheck = True
                Exit For
            End If
        Next
        If Not selectionCheck Then
            ErrorLabel.Text = getError("RegistrationForm3FundTypeError")
            Return False
        End If

        Return True
    End Function

    Protected Sub SendRegistrationEmail()
        If Me.Display Then

            Dim body As String = ucr.Content("RegistrationEmailHeader", _languageCode, True) & vbCrLf & vbCrLf & _
                                 ucr.Content("registrationHeaderLabel", _languageCode, True) & vbCrLf & _
                                 companyNameLabel.Text & ": " & companyName.Text & "." & vbCrLf & _
                                 add1Label.Text & ": " & add1.Text & "." & vbCrLf & _
                                 add2Label.Text & ": " & add2.Text & "." & vbCrLf & _
                                 add3Label.Text & ": " & add3.Text & "." & vbCrLf & _
                                 add4Label.Text & ": " & add4.Text & "." & vbCrLf & _
                                 add5Label.Text & ": " & add5.Text & "." & vbCrLf & _
                                 postcodeLabel.Text & ": " & postcode.Text & "." & vbCrLf & _
                                 countryLabel.Text & ": " & country.SelectedItem.Text & "." & vbCrLf & _
                                 phoneLabel.Text & ": " & phone.Text & "." & vbCrLf & _
                                 faxLabel.Text & ": " & fax.Text & "." & vbCrLf & _
                                 emailLabel.Text & ": " & email.Text & "." & vbCrLf & _
                                 openingTimesLabel.Text & ": " & openingTimes.Text & "." & vbCrLf & _
                                 estAnnExpLabel.Text & ": " & estAnnExp.Text & "." & vbCrLf & _
                                 credLimLabel.Text & ": " & credLim.Text & "." & vbCrLf & _
                                 dlvryInstLabel.Text & ": " & dlvryInst.Text & "." & vbCrLf & _
                                 partNCCLabel.Text & ": " & partNCC.Checked.ToString & "." & vbCrLf & _
                                 costCenLabel.Text & ": " & costCen.Text & " " & vbCrLf & _
                                 mainContactNameLabel.Text & ": " & mainContactName.Text & "." & vbCrLf & _
                                 mainContactPosLabel.Text & ": " & mainContactPos.Text & "." & vbCrLf & _
                                 mainContactTelLabel.Text & ": " & mainContactTel.Text & "." & vbCrLf & _
                                 mainContactExtLabel.Text & ": " & mainContactExt.Text & "." & vbCrLf & _
                                 scndContactNameLabel.Text & ": " & scndContactName.Text & "." & vbCrLf & _
                                 scndContactPosLabel.Text & ": " & scndContactPos.Text & "." & vbCrLf & _
                                 scndContactTelLabel.Text & ": " & scndContactTel.Text & "." & vbCrLf & _
                                 scndContactExtLabel.Text & ": " & scndContactExt.Text & "." & vbCrLf & vbCrLf & vbCrLf & _
                                 bankTitleLabel.Text & vbCrLf & vbCrLf & _
                                 nameOfBankLabel.Text & ": " & nameOfBankBox.Text & "." & vbCrLf & _
                                 addressOfBankLabel.Text & ": " & addressOfBankBox.Text & "." & vbCrLf & _
                                 bankPostcodeLabel.Text & ": " & bankPostcodeBox.Text & "." & vbCrLf & _
                                 accNameLabel.Text & ": " & accNameBox.Text & "." & vbCrLf & _
                                 sortCodeLabel.Text & ": " & sortCodeBox.Text & "." & vbCrLf & _
                                 accNoLabel.Text & ": " & accNoBox.Text & "." & vbCrLf & _
                                 bankCheckAgree.Text & ": " & bankCheckAgree.Checked.ToString & "." & vbCrLf & vbCrLf & vbCrLf & _
                                 tradeTitleLabel.Text & vbCrLf & vbCrLf & _
                                 supplier1NameLabel.Text & ": " & supplier1NameBox.Text & "." & vbCrLf & _
                                 supplier1AddressLabel.Text & ": " & supplier1AddressBox.Text & "." & vbCrLf & _
                                 supplier1PostcodeLabel.Text & ": " & supplier1PostcodeBox.Text & "." & vbCrLf & _
                                 supplier1TelLabel.Text & ": " & supplier1TelBox.Text & "." & vbCrLf & _
                                 supplier1FaxLabel.Text & ": " & supplier1FaxBox.Text & "." & vbCrLf & _
                                 supplier1AccNoLabel.Text & ": " & supplier1AccNoBox.Text & "." & vbCrLf & _
                                 supplier2NameLabel.Text & ": " & supplier2NameBox.Text & "." & vbCrLf & _
                                 supplier2AddressLabel.Text & ": " & supplier2AddressBox.Text & "." & vbCrLf & _
                                 supplier2PostcodeLabel.Text & ": " & supplier2PostcodeBox.Text & "." & vbCrLf & _
                                 supplier2TelLabel.Text & ": " & supplier2TelBox.Text & "." & vbCrLf & _
                                 supplier2FaxLabel.Text & ": " & supplier2FaxBox.Text & "." & vbCrLf & _
                                 supplier2AccNoLabel.Text & ": " & supplier2AccNoBox.Text & "." & vbCrLf & vbCrLf & vbCrLf & _
                                 orgTitleLabel.Text & vbCrLf & vbCrLf & _
                                 q1Label.Text & ": " & q1RadioList.SelectedItem.Text & "." & vbCrLf & _
                                 q1CharityLabel.Text & ": " & q1CharityBox.Text & "." & vbCrLf & _
                                 q2Label.Text & ": " & q2RadioList.SelectedItem.Text & "." & vbCrLf
            body += q3Label.Text & ": "
            Dim hasAdded As Boolean = False
            For Each li As ListItem In q3CheckBoxList.Items
                If li.Selected Then
                    If hasAdded Then
                        body += ", "
                    End If
                    body += li.Text
                    hasAdded = True
                End If
            Next
            body += vbCrLf & vbCrLf & vbCrLf & _
                     q4Label.Text & vbCrLf & vbCrLf & _
                     q4OrgWorkLabel.Text & ": " & q4OrgWorkBox.Text & "." & vbCrLf & _
                     q4HearAboutUsLabel.Text & ": " & q4HearAboutUsBox.Text & "." & vbCrLf & _
                     appNameLabel.Text & ": " & appNameBox.Text & "." & vbCrLf & _
                     appPosLabel.Text & ": " & appPosBox.Text & "." & vbCrLf & _
                     q4CatalogueCheck.Text & ": " & q4CatalogueCheck.Checked.ToString & "." & vbCrLf

            Talent.Common.Utilities.SMTP = ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim
            Talent.Common.Utilities.SMTPPortNumber = Talent.eCommerce.Utilities.GetSMTPPortNumber
            If (ucr.Attribute("emailFormat") = "PLAINTEXT") Then
                Dim err As Talent.Common.ErrorObj = Talent.Common.Utilities.Email_Send(myDefs.RegistrationConfirmationFromEmail, ucr.Attribute("RegistrationEmailAddress"), _
                                                            ucr.Content("RegistrationEmailSubject", _languageCode, True), _
                                                            body)
            ElseIf (ucr.Attribute("emailFormat") = "HTML") Then
                Dim err As Talent.Common.ErrorObj = Talent.Common.Utilities.Email_Send(myDefs.RegistrationConfirmationFromEmail, ucr.Attribute("RegistrationEmailAddress"), _
                                                ucr.Content("RegistrationEmailSubject", _languageCode, True), _
                                                body, "", False, True)
            End If
        End If
    End Sub

    Protected Sub SendConfirmationEmail()
        If Me.Display Then
            Talent.Common.Utilities.SMTP = ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim
            If (ucr.Attribute("emailFormat") = "PLAINTEXT") Then
                Dim body As String = ucr.Content("ConfirmationEmailBody", _languageCode, True)
                body = body.Replace("<<NewLine>>", vbCrLf)
                body = body.Replace("<<FullName>>", mainContactName.Text)
                body = body.Replace("<<Organisation>>", companyName.Text)
                body = body.Replace("<<WebSiteAddress>>", Talent.eCommerce.Utilities.GetCurrentApplicationUrl & "/PagesPublic/Home/Home.aspx")
                Dim err As Talent.Common.ErrorObj = Talent.Common.Utilities.Email_Send(myDefs.RegistrationConfirmationFromEmail, email.Text, _
                                                            ucr.Content("ConfirmationEmailSubject", _languageCode, True), _
                                                            body)
            ElseIf (ucr.Attribute("emailFormat") = "HTML") Then
                Dim body As String = ucr.Content("ConfirmationEmailBodyHTML", _languageCode, True)
                body = body.Replace("<<NewLine>>", "<br>")
                body = body.Replace("<<FullName>>", mainContactName.Text)
                body = body.Replace("<<Organisation>>", companyName.Text)
                body = body.Replace("<<WebSiteAddress>>", Talent.eCommerce.Utilities.GetCurrentApplicationUrl & "/PagesPublic/Home/Home.aspx")
                Dim err As Talent.Common.ErrorObj = Talent.Common.Utilities.Email_Send(myDefs.RegistrationConfirmationFromEmail, email.Text, _
                                                ucr.Content("ConfirmationEmailSubject", _languageCode, True), _
                                                body, "", False, True)
            End If
        End If
    End Sub

    Protected Sub nextButton1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles nextButton1.Click
        If partNCC.Checked AndAlso Not String.IsNullOrEmpty(costCen.Text) Then
            nextButton2_Click(sender, e)
        ElseIf partNCC.Checked AndAlso String.IsNullOrEmpty(costCen.Text) Then
            ErrorLabel.Text = getError("CostCentreMandatory")
        Else
            Panel1.Visible = False
            Panel2.Visible = True
            Panel3.Visible = False
            Panel4.Visible = False
        End If
    End Sub

    Protected Sub nextButton2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles nextButton2.Click
        If Not partNCC.Checked AndAlso Not bankCheckAgree.Checked Then
            Me.ErrorLabel.Text = getError("RegistrationForm3BankCheckError")
        Else
            ErrorLabel.Text = ""
            Panel1.Visible = False
            Panel2.Visible = False
            Panel3.Visible = True
            Panel4.Visible = False
        End If
    End Sub

    Protected Sub nextButton3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles nextButton3.Click
        Panel1.Visible = False
        Panel2.Visible = False
        Panel3.Visible = False
        Panel4.Visible = True
    End Sub

    Protected Sub prevButton2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles prevButton2.Click
        Panel1.Visible = True
        Panel2.Visible = False
        Panel3.Visible = False
        Panel4.Visible = False
    End Sub

    Protected Sub prevButton3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles prevButton3.Click
        If partNCC.Checked Then
            prevButton2_Click(sender, e)
        Else
            Panel1.Visible = False
            Panel2.Visible = True
            Panel3.Visible = False
            Panel4.Visible = False
        End If
    End Sub

    Protected Sub prevButton4_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles prevButton4.Click
        Panel1.Visible = False
        Panel2.Visible = False
        Panel3.Visible = True
        Panel4.Visible = False
    End Sub
End Class
