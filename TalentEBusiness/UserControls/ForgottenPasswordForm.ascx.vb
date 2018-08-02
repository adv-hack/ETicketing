Imports Talent.eCommerce
Imports System.Collections.Generic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
''
'       Function                    User Controls - Forgotten Password Form
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCFPWD- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_ForgottenPasswordForm

    'Inherits System.Web.UI.UserControl
    Inherits ControlBase
    Private languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Dim ucr As New Talent.Common.UserControlResource

    Protected Sub ForgottenPasswordButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ForgottenPasswordButton.Click
        If Page.IsValid Then

            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "ForgottenPasswordForm.ascx"
            End With

            Dim username As String = GetUserName()
            Dim userPassword As String = ""
            Dim loginToken As String = ""
            Dim hashedToken As String = ""
            Dim tokenList As List(Of String)
            Dim passHash As New Talent.Common.PasswordHash
            Dim userEmail As String = "", _
                emailSubject As String = "", _
                emailText As String = "", _
                userSalutation As String = "", _
                userFullname As String = "", _
                userTitle As String = "", _
                userSurname As String = ""




            ' Is this a duplicate email address
            If username = "DuplicateEmailAddress" Then
                CheckEmailAddress.ErrorMessage = ucr.Content("duplicateEmailAddressErrorText", languageCode, True)
            Else
                Dim lookupCustNo As String = ""
                Dim lookupEmail As String = ""
                If username.Contains("@") Then
                    lookupEmail = username
                Else
                    lookupCustNo = username
                    If ForgottenPasswordTextBox.Text.Contains("@") Then
                        lookupEmail = ForgottenPasswordTextBox.Text
                    End If
                End If
                '--------------------------------------------------------
                ' Retreive the customer details
                '---------------------------------------------------------
                Dim errorCode As String = String.Empty
                If ModuleDefaults.UseLoginLookup AndAlso ModuleDefaults.LoginLookupType.ToUpper <> "ECOMMERCE" Then

                    Dim loginType As Integer
                    If rblAddtionalOptions.Items.Count > 0 Then
                        loginType = CType(rblAddtionalOptions.SelectedValue, Integer)
                    End If
                    Dim mem As New TalentMembershipProvider
                    userPassword = mem.RetrieveBackendPassword(lookupCustNo, lookupEmail, loginType, errorCode)


                    Dim myDs As New Data.DataSet
                    If lookupCustNo = "000000000000" OrElse lookupCustNo = String.Empty Then
                        myDs = mem.GetBackendUserDetails(lookupEmail, userPassword)
                    Else
                        username = lookupCustNo
                        myDs = mem.GetBackendUserDetails(lookupCustNo, userPassword)
                    End If

                    If myDs.Tables.Count > 1 Then
                        If myDs.Tables(1).Rows.Count > 0 Then
                            Dim dr As Data.DataRow = myDs.Tables(1).Rows(0)
                            userEmail = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("EmailAddress")).ToString.Trim
                            userSalutation = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("Salutation")).ToString.Trim
                            userFullname = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("ContactForename")).ToString.Trim & " " & Talent.eCommerce.Utilities.CheckForDBNull_String(dr("ContactSurname")).ToString.Trim
                            userTitle = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("ContactTitle")).ToString.Trim
                            userSurname = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("ContactSurname")).ToString.Trim
                            If lookupCustNo = "000000000000" Then
                                username = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("CustomerNo")).ToString.Trim
                            End If
                        End If
                    End If

                Else
                    Dim userDets As New TalentProfileUser
                    Dim err As New Talent.Common.ErrorObj
                    Dim tmu As TalentMembershipUser = CType(Membership.Provider, TalentMembershipProvider).GetUser(username, False)
                    If Not tmu Is Nothing Then

                        Dim talProfile As TalentProfile = Profile.GetProfile(tmu.UserName), _
                            talProfileUser As TalentProfileUser = Profile.Provider.GetUserByLoginID(tmu.UserName)
                        userSalutation = talProfileUser.Details.Salutation
                        userTitle = talProfileUser.Details.Title
                        userFullname = talProfileUser.Details.Full_Name
                        userSurname = talProfileUser.Details.Surname
                        userEmail = talProfileUser.Details.Email
                        userPassword = tmu.Password

                    End If

                End If

                '--------------------------------------------------------
                ' Generate a login Token
                '---------------------------------------------------------
                tokenList = passHash.ResetPasswordToken()
                loginToken = tokenList(0)
                hashedToken = tokenList(1)

                If Not String.IsNullOrEmpty(userEmail) Then

                    If ModuleDefaults.ExternalPasswordIsMaster AndAlso String.IsNullOrEmpty(userPassword) Then
                        Dim loginType As Integer
                        If rblAddtionalOptions.Items.Count > 0 Then
                            loginType = CType(rblAddtionalOptions.SelectedValue, Integer)
                        End If
                        Dim mem As New TalentMembershipProvider
                        userPassword = mem.RetrieveBackendPassword(lookupCustNo, lookupEmail, loginType, errorCode)
                        If lookupCustNo <> "000000000000" AndAlso lookupCustNo = String.Empty Then
                            username = lookupCustNo
                        End If
                    End If

                    If ModuleDefaults.UseEncryptedPassword Then
                        SendChangePasswordEmail(lookupCustNo, userEmail, username, hashedToken)
                    Else
                        '---------------------------------------------------------------
                        ' Is password blank and an error setup for this? If so report if
                        '---------------------------------------------------------------
                        Dim passwordEmptyError As String = ucr.Content("passwordEmptyErrorText", languageCode, True)
                        If userPassword.Trim = String.Empty AndAlso passwordEmptyError <> String.Empty Then
                            If errorCode = String.Empty Then
                                Dim customError As New CustomValidationSummary
                                customError.ValidationGroup = "ForgottenPassword"
                                customError.AddErrorMessage(passwordEmptyError, Me.Page)
                            Else
                                Dim errMsg As Talent.Common.TalentErrorMessages
                                errMsg = New Talent.Common.TalentErrorMessages(languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ucr.FrontEndConnectionString)
                                CheckEmailAddress.ErrorMessage = errMsg.GetErrorMessage(errorCode).ERROR_MESSAGE
                                CheckEmailAddress.IsValid = False
                            End If
                        Else
                            '----------------
                            ' Build login URL
                            '----------------
                            Dim loginUrl As String = Utilities.GetCurrentApplicationUrl() & "/PagesPublic/Login/Login.aspx"
                            If Utilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("IsEmailNotByMonitor")) Then
                                Dim emailFormat As String = ucr.Attribute("ConfimationEMailBodyHTML")
                                Dim htmlFormat As Boolean = False
                                emailSubject = ucr.Content("emailSubject", languageCode, True)

                                If emailFormat.ToUpper = "PLAINTEXT" Then
                                    emailText = ucr.Content("emailText", languageCode, True)
                                ElseIf emailFormat.ToUpper = "HTML" Then
                                    emailText = ucr.Content("emailHTMLText", languageCode, True)
                                    htmlFormat = True
                                End If

                                emailText = Replace(emailText, "<<To>>", userSalutation)
                                emailText = Replace(emailText, "<<Title>>", userTitle)
                                emailText = Replace(emailText, "<<Surname>>", userSurname)
                                emailText = Replace(emailText, "<<Name>>", userFullname)
                                emailText = Replace(emailText, "<<NewLine>>", vbCrLf)
                                emailText = Replace(emailText, "<<Password>>", userPassword)
                                emailText = Replace(emailText, "<<LoginURL>>", loginUrl)
                                Talent.Common.Utilities.SMTP = ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim
                                Talent.Common.Utilities.Email_Send(ModuleDefaults.ForgottenPasswordFromEmail, userEmail, _
                                                                    emailSubject, emailText, "", False, htmlFormat)
                                ForgottenPasswordTextBox.Text = ""
                            Else
                                Dim talEmail As New Talent.Common.TalentEmail
                                Dim xmlDoc As String = talEmail.CreateForgottenPasswordXmlDocument(ModuleDefaults.ForgottenPasswordFromEmail, _
                                                                                                   userEmail, _
                                                                                                   ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim, _
                                                                                                   Utilities.GetSMTPPortNumber, _
                                                                                                   ucr.PartnerCode, _
                                                                                                   username, _
                                                                                                   loginUrl)

                                'Create the email request in the offline processing table
                                TDataObjects.AppVariableSettings.TblOfflineProcessing.Insert(TalentCache.GetBusinessUnit(), "*ALL", "Pending", 0, "", _
                                                                        "EmailMonitor", "ForgottenPassword", xmlDoc, "")
                            End If
                            Response.Redirect("~/PagesPublic/Profile/forgottenPasswordConfirmation.aspx")
                        End If
                    End If
                Else
                    If errorCode = String.Empty Then
                        CheckEmailAddress.ErrorMessage = ucr.Content("emailDoesNotExistErrorText", languageCode, True)
                        CheckEmailAddress.IsValid = False
                    Else
                        Dim errMsg As Talent.Common.TalentErrorMessages
                        errMsg = New Talent.Common.TalentErrorMessages(languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ucr.FrontEndConnectionString)
                        CheckEmailAddress.ErrorMessage = errMsg.GetErrorMessage(errorCode).ERROR_MESSAGE
                        CheckEmailAddress.IsValid = False
                    End If
                End If
            End If

        End If

    End Sub

    Protected Sub SendChangePasswordEmail(ByVal lookupCustNo As String, ByVal userEmail As String, ByVal LoginID As String, ByVal hashedToken As String)
        '
        ' Generate a login key
        Dim loginkey As String = Talent.Common.Utilities.RandomString(128)
        Dim defaultDate As New Date(1900, 1, 1)
        '
        ' Write a forgotten password record
        Me.TDataObjects.ProfileSettings.tblForgottenPassword.Insert(TalentCache.GetBusinessUnit(), "*ALL", hashedToken, "Created", "lookupCustNo", "userEmail", 30)
        '                                                           

        '
        ' Build login URL
        Dim loginUrl As String = Request.Url.AbsoluteUri
        Dim fpIndex As Integer = loginUrl.IndexOf("ForgottenPassword.aspx")
        loginUrl = loginUrl.Substring(0, fpIndex) & "ChangePassword.aspx?LoginKey=" & loginkey

        Dim talEmail As New Talent.Common.TalentEmail
        Dim xmlDoc As String = talEmail.CreateChangePasswordXmlDocument(ModuleDefaults.ForgottenPasswordFromEmail, _
                                                                        userEmail, _
                                                                        ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim, _
                                                                        Utilities.GetSMTPPortNumber, _
                                                                        ucr.PartnerCode, _
                                                                        LoginID, _
                                                                        loginUrl)

        ' CType(HttpContext.Current.Profile, TalentProfile).User.Details.LoginID

        'Create the email request in the offline processing table
        TDataObjects.AppVariableSettings.TblOfflineProcessing.Insert(TalentCache.GetBusinessUnit(), "*ALL", "Pending", 0, "", _
                                                "EmailMonitor", "ChangePassword", xmlDoc, "")

        Response.Redirect("~/PagesPublic/Profile/forgottenPasswordConfirmation.aspx")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ForgottenPasswordForm.ascx"

            ForgottenPasswordTitleLabel.Text = .Content("ForgottenPasswordTitleLabelText", _languageCode, True)
            ForgottenPasswordInstructionsLabel.Text = .Content("ForgottenPasswordInstructionsLabelText", _languageCode, True)
            ForgottenPasswordLabel.Text = .Content("ForgottenPasswordLabelText", _languageCode, True)
            ForgottenPasswordButton.Text = .Content("ForgottenPasswordButtonText", _languageCode, True)
            ForgottenPasswordRFV.ErrorMessage = .Content("ForgottenPasswordRequiredFieldErrorText", _languageCode, True)
            ForgottenPasswordRegEx.ErrorMessage = .Content("ForgottenPasswordRegularExpressionErrorText", _languageCode, True)
            ForgottenPasswordRegEx.ValidationExpression = .Attribute("ForgottenPasswordRegularExpression")
        End With

        If Profile.IsAnonymous Then buildAddtionalOptionsList()
    End Sub

    Protected Sub CheckEmailAddress_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles CheckEmailAddress.ServerValidate
        If Page.IsValid Then
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "ForgottenPasswordForm.ascx"
            End With

            Dim username As String = GetUserName()
            '
            ' Is this a duplicate email address
            If username = "DuplicateEmailAddress" Then
                args.IsValid = False
                CheckEmailAddress.ErrorMessage = ucr.Content("duplicateEmailAddressErrorText", languageCode, True)
            Else
                '--------------------------------------------------------
                ' Check email address is valid and retreive customer name
                '---------------------------------------------------------
                Dim userDets As New TalentProfileUser
                Dim err As New Talent.Common.ErrorObj

                Dim tmu As TalentMembershipUser = CType(Membership.Provider, TalentMembershipProvider).GetUser(username, False)
                If Not tmu Is Nothing OrElse ModuleDefaults.UseLoginLookup Then
                    args.IsValid = True
                Else
                    CheckEmailAddress.ErrorMessage = ucr.Content("emailDoesNotExistErrorText", languageCode, True)
                    args.IsValid = False
                End If
            End If
        End If
    End Sub

    Private Function GetUserName() As String

        Dim username As String = ForgottenPasswordTextBox.Text

        ' You can use email address or login id when login lookup is set
        If ModuleDefaults.UseLoginLookup Then
            Dim profile1 As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter

            'Dim dt As Data.DataTable = profile1.GetDataByEmail(ForgottenPasswordTextBox.Text, TalentCache.GetPartner(HttpContext.Current.Profile))
            Dim dt As Data.DataTable
            Dim authPartners As New TalentMembershipDatasetTableAdapters.tbl_authorized_partnersTableAdapter
            If authPartners.Get_CheckFor_B2C_Login(TalentCache.GetBusinessUnitGroup).Rows.Count > 0 Then
                dt = profile1.GetDataByEmail(ForgottenPasswordTextBox.Text, TalentCache.GetPartner(HttpContext.Current.Profile))
            Else
                dt = profile1.GetDataByEmailNoPartner(ForgottenPasswordTextBox.Text)
            End If

            ' Convert the email address to customer number.  If RefreshCustomerDataOnLogin is set to True
            ' then this override needs to happen on the backend
            'If dt.Rows.Count = 1 Then
            If dt.Rows.Count = 0 Then
                ' BF - Dont blank out the username if it's not an email address..
                If username.Contains("@") Then
                    username = ""
                Else
                    username = ForgottenPasswordTextBox.Text
                End If
            ElseIf dt.Rows.Count = 1 Or ModuleDefaults.AllowDuplicateEmail Then
                username = dt.Rows(0).Item("LOGINID")
            ElseIf dt.Rows.Count > 1 Then
                Return "DuplicateEmailAddress"
            End If
        End If

        ' Add leading zeros when loginId is customerNumber
        If ModuleDefaults.LoginidType.Equals("1") Then
            'Only add leading zeros to the Talent customer number
            If rblAddtionalOptions.Items.Count = 0 OrElse rblAddtionalOptions.SelectedIndex = 0 Then
                username = Talent.Common.Utilities.PadLeadingZeros(username.Trim, "12")
            End If
        End If

        Return username

    End Function

    Private Sub buildAddtionalOptionsList()
        Dim showAddtionalOptions As Boolean = Utilities.CheckForDBNull_Boolean_DefaultFalse(ucr.Attribute("ShowAddtionalOptions"))
        If showAddtionalOptions Then
            Dim defaultItemNumber As Integer = 1
            defaultItemNumber = Utilities.CheckForDBNull_Int(ucr.Attribute("DefaultSelectedItemNumber"))
            plhAddtionalOptions.Visible = True
            For i As Integer = 0 To 9
                Dim itemTextValue As String = ucr.Content(String.Format("rblAddtionalOption{0}", i), languageCode, True)
                If itemTextValue.Length > 0 Then
                    Dim rdoItem As New ListItem
                    rdoItem.Text = ucr.Content(String.Format("rblAddtionalOption{0}", i), languageCode, True)
                    rdoItem.Value = i
                    Dim className As New StringBuilder
                    className.Append("radio ")
                    className.Append("item")
                    className.Append(i)
                    rdoItem.Attributes.Add("class", className.ToString())
                    If defaultItemNumber = i Then rdoItem.Selected = True
                    rblAddtionalOptions.Items.Add(rdoItem)
                End If
            Next
        End If
    End Sub
End Class
