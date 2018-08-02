Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports Talent.Common

Partial Class UserControls_ContactUsForm
    Inherits ControlBase

#Region "Class Level Fields"

    Private _languageCode As String = Nothing
    Private _ucr As Talent.Common.UserControlResource = Nothing

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _ucr = New Talent.Common.UserControlResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ContactUsForm.ascx"
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName(True)
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        End With
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not Page.IsPostBack Then
            PopulateTitleDropDownList()
            PopulateCategoryDropDownList()
            SetLabelText()
            SetDetailsVisibility()
            SetUserDetails()
            SetupRequiredValidators()
            SetupRegularExpressionValidators()
        End If
        plhErrorMessage.Visible = (ltlErrorMessage.Text.Length > 0)
        plhInstructions.Visible = (ContactUsInstructionsLabel.Text.Length > 0)
    End Sub

    Protected Sub submitBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles submitBtn.Click
        ' Either create an action in TALENT or send email 
        If Not (Profile.IsAnonymous) AndAlso Not ModuleDefaults.ContactUsFormShouldAlwaysEmail Then
            If message.Text.Length > 960 Then
                ltlErrorMessage.Text = _ucr.Content("MessageLengthError", _languageCode, True)
            Else
                SendDetailsToCRM()
            End If
        Else
            SendDetailsViaEmail()
        End If

        If Request.QueryString("ReturnUrl") Is Nothing Then
            Response.Redirect("~/PagesPublic/Contact/ContactUsConfirmation.aspx")
        Else
            Response.Redirect(Request.QueryString("ReturnUrl"))
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub SetupRequiredValidators()
        If Not (Profile.IsAnonymous) Then
            forenameRFV.Enabled = False
            surnameRFV.Enabled = False
            emailRFV.Enabled = False
            phoneRFV.Enabled = False
            mobileRFV.Enabled = False
        Else
            EnableRFV(forenameRFV, "forenameRFVErrorMessage")
            EnableRFV(surnameRFV, "surnameRFVErrorMessage")
            EnableRFV(emailRFV, "emailRFVErrorMessage")
            EnableRFV(phoneRFV, "phoneRFVErrorMessage")
            EnableRFV(mobileRFV, "mobileRFVErrorMessage")
        End If
        messageRFV.ErrorMessage = _ucr.Content("messageRFVErrorMessage", _languageCode, True)
    End Sub

    Private Sub SetupRegularExpressionValidators()
        If Not (Profile.IsAnonymous) Then
            titleRegEx.Enabled = False
            forenameRegEx.Enabled = False
            surnameRegEx.Enabled = False
            emailRegEx.Enabled = False
            phoneRegEx.Enabled = False
            workRegEx.Enabled = False
            mobileRegEx.Enabled = False
        Else
            titleRegEx.ValidationExpression = _ucr.Attribute("titleRegExValidationExpression")
            titleRegEx.ErrorMessage = _ucr.Content("titleRegExErrorMessage", _languageCode, True)
            forenameRegEx.ValidationExpression = _ucr.Attribute("forenameRegExValidationExpression")
            forenameRegEx.ErrorMessage = _ucr.Content("forenameRegExErrorMessage", _languageCode, True)
            surnameRegEx.ValidationExpression = _ucr.Attribute("surnameRegExValidationExpression")
            surnameRegEx.ErrorMessage = _ucr.Content("surnameRegExErrorMessage", _languageCode, True)
            emailRegEx.ValidationExpression = _ucr.Attribute("emailRegExValidationExpression1") & _ucr.Attribute("emailRegExValidationExpression2")
            emailRegEx.ErrorMessage = _ucr.Content("emailRegExErrorMessage", _languageCode, True)
            phoneRegEx.ValidationExpression = _ucr.Attribute("phoneRegExValidationExpression")
            phoneRegEx.ErrorMessage = _ucr.Content("phoneRegExErrorMessage", _languageCode, True)
            workRegEx.ValidationExpression = _ucr.Attribute("workRegExValidationExpression")
            workRegEx.ErrorMessage = _ucr.Content("workRegExErrorMessage", _languageCode, True)
            mobileRegEx.ValidationExpression = _ucr.Attribute("mobileRegExValidationExpression")
            mobileRegEx.ErrorMessage = _ucr.Content("mobileRegExErrorMessage", _languageCode, True)
        End If
        categoryRegEx.ValidationExpression = _ucr.Attribute("categoryRegExValidationExpression")
        categoryRegEx.ErrorMessage = _ucr.Content("categoryRegExErrorMessage", _languageCode, True)
    End Sub

    Private Sub PopulateTitleDropDownList()
        title.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "CONTACTUS", "TITLE")
        title.DataBind()
    End Sub

    Private Sub PopulateCategoryDropDownList()
        category.DataSource = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "CONTACTUS", "DESCRIPTION", "category")
        category.DataBind()
    End Sub

    Private Sub SetDetailsVisibility()
        If Not (Profile.IsAnonymous) Then
            title.Enabled = False
            forename.Enabled = False
            surname.Enabled = False
            email.Enabled = False
            phone.Enabled = False
            work.Enabled = False
            mobile.Enabled = False
        Else
            title.Enabled = True
            forename.Enabled = True
            surname.Enabled = True
            email.Enabled = True
            phone.Enabled = True
            work.Enabled = True
            mobile.Enabled = True
        End If
        Try
            With _ucr
                plhTitleRow.Visible = CType(.Attribute("titleRowVisible"), Boolean)
                plhForenameRow.Visible = CType(.Attribute("forenameRowVisible"), Boolean)
                plhSurnameRow.Visible = CType(.Attribute("surnameRowVisible"), Boolean)
                plhEmailRow.Visible = CType(.Attribute("emailRowVisible"), Boolean)
                plhPhoneRow.Visible = CType(.Attribute("phoneRowVisible"), Boolean)
                plhWorkRow.Visible = CType(.Attribute("workRowVisible"), Boolean)
                plhMobileRow.Visible = CType(.Attribute("mobileRowVisible"), Boolean)
                plhCategoryRow.Visible = CType(.Attribute("categoryRowVisible"), Boolean)
                plhMessageRow.Visible = CType(.Attribute("messageRowVisible"), Boolean)
                plhFlagRow.Visible = CType(.Attribute("flagRowVisible"), Boolean)
                plhPrivacyRow.Visible = CType(.Attribute("privacyRowVisible"), Boolean)
            End With
        Catch ex As Exception
        End Try
    End Sub

    Private Sub SetLabelText()
        With _ucr
            ContactUsInstructionsLabel.Text = .Content("contactUsInstructionsLabel", _languageCode, True)
            PersonalDetailsLabel.Text = .Content("contactUsPersonalDetailsLabel", _languageCode, True)
            titleLabel.Text = .Content("contactUsTitleLabel", _languageCode, True)
            forenameLabel.Text = .Content("contactUsForenameLabel", _languageCode, True)
            surnameLabel.Text = .Content("contactUsSurnameLabel", _languageCode, True)
            emailLabel.Text = .Content("contactUsEmailLabel", _languageCode, True)
            mobileLabel.Text = .Content("contactUsMobileLabel", _languageCode, True)
            phoneLabel.Text = .Content("contactUsPhoneLabel", _languageCode, True)
            workLabel.Text = .Content("contactUsWorkLabel", _languageCode, True)
            categoryLabel.Text = .Content("contactUsCategoryLabel", _languageCode, True)
            messageLabel.Text = .Content("contactUsMessageLabel", _languageCode, True)
            flagCheckbox.Text = .Content("contactUsFlagCheckboxText", _languageCode, True)
            privacyLabel1.Text = .Content("contactUsPrivacyText1", _languageCode, True)
            privacyLink.Text = .Content("contactUsPrivacyLinkText", _languageCode, True)
            privacyLink.NavigateUrl = .Content("contactUsPrivacyLinkURL", _languageCode, True)
            privacyLabel2.Text = .Attribute("contactUsPrivacyText2")
            submitBtn.Text = .Content("contactUsSubmitButtonText", _languageCode, True)
        End With
    End Sub

    Private Sub SetUserDetails()
        If Not (Profile.IsAnonymous) Then
            title.Text = Profile.User.Details.Title
            forename.Text = Profile.User.Details.Forename
            surname.Text = Profile.User.Details.Surname
            email.Text = Profile.User.Details.Email
            mobile.Text = Profile.User.Details.Mobile_Number
            phone.Text = Profile.User.Details.Telephone_Number
            work.Text = Profile.User.Details.Work_Number
        Else
            title.Text = String.Empty
            forename.Text = String.Empty
            surname.Text = String.Empty
            email.Text = String.Empty
            mobile.Text = String.Empty
            phone.Text = String.Empty
            work.Text = String.Empty
        End If
    End Sub

    Private Sub SendDetailsToCRM()
        Dim myCustomer As New TalentCustomer
        Dim myErrorObj As New ErrorObj
        Dim deCustV11 As New DECustomerV11
        Dim deCustV1 As New DECustomer
        deCustV11.DECustomersV1.Add(deCustV1)

        ' Set the Customer object and invoke the SetCustomer method to post details to TALENT Customer Manager
        With myCustomer
            .DeV11 = deCustV11
            ' Reset the settings entity to a customer specific settings entity 
            .Settings = CType(New DECustomerSettings, DESettings)
            With deCustV1
                .Action = ""
                .ThirdPartyContactRef = _ucr.Content("crmExternalKeyName", _languageCode, True)
                .ThirdPartyCompanyRef1 = Profile.User.Details.User_Number
                .ThirdPartyCompanyRef2 = ""
                .ThirdPartyCompanyRef1Supplement = Profile.User.Details.User_Number_Prefix
                .Language = Talent.eCommerce.Utilities.GetCurrentLanguage()
                .DateFormat = "1"
                .ActionCode = category.Text.ToString.Trim
                .ActionCodeFixed = _ucr.Attribute("crmActionCode")
                .ActionAgent = _ucr.Attribute("crmActionAgent")
                Dim myComments As String = message.Text.Trim.PadRight(960, " ")
                Try
                    .ActionComment01 = myComments.Substring(0, 60).Trim
                    .ActionComment02 = myComments.Substring(61, 60).Trim
                    .ActionComment03 = myComments.Substring(121, 60).Trim
                    .ActionComment04 = myComments.Substring(181, 60).Trim
                    .ActionComment05 = myComments.Substring(241, 60).Trim
                    .ActionComment06 = myComments.Substring(301, 60).Trim
                    .ActionComment07 = myComments.Substring(361, 60).Trim
                    .ActionComment08 = myComments.Substring(421, 60).Trim
                    .ActionComment09 = myComments.Substring(481, 60).Trim
                    .ActionComment10 = myComments.Substring(541, 60).Trim
                    .ActionComment11 = myComments.Substring(601, 60).Trim
                    .ActionComment12 = myComments.Substring(661, 60).Trim
                    .ActionComment13 = myComments.Substring(721, 60).Trim
                    .ActionComment14 = myComments.Substring(781, 60).Trim
                    .ActionComment15 = myComments.Substring(841, 60).Trim
                    .ActionComment16 = myComments.Substring(901, 60).Trim
                Catch ex As Exception

                End Try
                .ActionDate = Year(Now).ToString.PadLeft(2, "0") & Month(Now).ToString.PadLeft(2, "0") & Day(Now).ToString.PadLeft(2, "0")
                .ActionDate = .ActionDate.Substring(2)
                .ActionPty = _ucr.Attribute("crmActionPriority")
                .ActionStatus = _ucr.Attribute("crmActionStatus")
                .ContactSurname = Profile.User.Details.Surname
                .ProcessContactSurname = "1"
            End With

            Dim decs As New DECustomerSettings()
            decs = CType(.Settings, DECustomerSettings)
            decs.CreationType = "CONTACTUS"
            .Settings = CType(decs, DESettings)
            With .Settings
                .BusinessUnit = TalentCache.GetBusinessUnit
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .Company = _ucr.Content("crmExternalKeyName", _languageCode, True)
                .Cacheing = False
                .DestinationDatabase = Talent.eCommerce.Utilities.GetCustomerDestinationDatabase()
                .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("TALENTCRM").ToString
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            End With
            myErrorObj = .SetCustomer()
        End With
    End Sub

    Private Sub SendDetailsViaEmail()
        Dim strFrom As String = String.Empty
        Dim strTo(0) As String
        Dim strCC(0) As String
        Dim strBcc(0) As String
        Dim strSubject As String = String.Empty
        Dim strMessage As String = String.Empty
        Dim strFileList(0) As String
        Dim boolHTMLEmail As Boolean = False
        Dim strSourceText As String = String.Empty
        Dim strMergeCode(10) As String
        Dim strMergeData(10) As String

        strFrom = email.Text.ToString.Trim
        strTo(0) = _ucr.Content("contactUsEmailToAddress", _languageCode, True)
        strCC(0) = _ucr.Content("contactUsEmailCCAddress", _languageCode, True)
        strBcc(0) = _ucr.Content("contactUsEmailBCCAddress", _languageCode, True)
        strSubject = _ucr.Content("contactUsEmailSubject", _languageCode, True)
        strFileList(0) = String.Empty
        If _ucr.Content("contactUsEmailHTML", _languageCode, True) <> String.Empty Then
            boolHTMLEmail = True
        End If

        ' Perform a data merge to produce the email body
        strSourceText = _ucr.Content("contactUsEmailMessage", _languageCode, True)
        strMergeCode(0) = _ucr.Content("contactUsEmailMergeCode01", _languageCode, True)
        strMergeCode(1) = _ucr.Content("contactUsEmailMergeCode02", _languageCode, True)
        strMergeCode(2) = _ucr.Content("contactUsEmailMergeCode03", _languageCode, True)
        strMergeCode(3) = _ucr.Content("contactUsEmailMergeCode04", _languageCode, True)
        strMergeCode(4) = _ucr.Content("contactUsEmailMergeCode05", _languageCode, True)
        strMergeCode(5) = _ucr.Content("contactUsEmailMergeCode06", _languageCode, True)
        strMergeCode(6) = _ucr.Content("contactUsEmailMergeCode07", _languageCode, True)
        strMergeCode(7) = _ucr.Content("contactUsEmailMergeCode08", _languageCode, True)
        strMergeCode(8) = _ucr.Content("contactUsEmailMergeCode09", _languageCode, True)
        strMergeCode(9) = _ucr.Content("contactUsEmailMergeCode10", _languageCode, True)
        strMergeCode(10) = _ucr.Content("contactUsEmailMergeCode11", _languageCode, True)
        strMergeData(0) = title.Text.ToString.Trim
        strMergeData(1) = forename.Text.ToString.Trim
        strMergeData(2) = surname.Text.ToString.Trim
        strMergeData(3) = email.Text.ToString.Trim
        strMergeData(4) = phone.Text.ToString.Trim
        strMergeData(5) = work.Text.ToString.Trim
        strMergeData(6) = mobile.Text.ToString.Trim
        strMergeData(7) = category.Text.ToString.Trim
        strMergeData(8) = message.Text.ToString.Trim
        strMergeData(9) = flagCheckbox.Checked.ToString
        If Not (Profile.IsAnonymous) Then
            strMergeData(10) = Profile.User.Details.LoginID
        Else
            strMergeData(10) = String.Empty
        End If
        strMessage = MergeDataIntoText(strSourceText, strMergeCode, strMergeData)

        ' Perform the send operation
        Talent.Common.Utilities.SMTP = ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim
        Talent.Common.Utilities.SMTPPortNumber = Talent.eCommerce.Utilities.GetSMTPPortNumber
        Talent.Common.Utilities.Email_Send(strFrom, strTo, strCC, strBcc, strSubject, strMessage, strFileList, False, boolHTMLEmail)
    End Sub

    ''' <summary>
    ''' Enables the given required field validator.
    ''' </summary>
    ''' <param name="reqFieldValidator">The req field validator.</param>
    ''' <param name="ucrContentErrorTextCode">The ucr content error text code.</param>
    Private Sub EnableRFV(ByVal reqFieldValidator As RequiredFieldValidator, ByVal ucrContentErrorTextCode As String)
        Dim canEnable As Boolean = False
        reqFieldValidator.Enabled = False
        If Not String.IsNullOrWhiteSpace(_ucr.Content(ucrContentErrorTextCode, _languageCode, True)) Then
            reqFieldValidator.Enabled = True
            reqFieldValidator.ErrorMessage = _ucr.Content(ucrContentErrorTextCode, _languageCode, True)
            canEnable = True
        End If
    End Sub

#End Region

#Region "Private Functions"

    Private Function MergeDataIntoText(ByVal strSourceText As String, ByVal strMergeCode() As String, ByVal strMergeData() As String) As String
        Dim intCount As Integer = 0
        Dim intMergeCodes As Integer = strMergeCode.Length
        Do While intCount < intMergeCodes
            If Not strMergeCode(intCount).Trim.Equals("") Then
                strSourceText = strSourceText.Replace(strMergeCode(intCount).Trim, strMergeData(intCount).Trim)
            End If
            intCount = intCount + 1
        Loop
        Return strSourceText
    End Function

#End Region

End Class