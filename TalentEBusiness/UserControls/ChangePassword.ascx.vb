
Partial Class UserControls_ChangePassword
    Inherits ControlBase


    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private ucr As New Talent.Common.UserControlResource
    Private myDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues

    Public ReadOnly Property ConfirmNewPasswordBox() As TextBox
        Get
            Return ConfirmNewPassword
        End Get
    End Property
    Public ReadOnly Property lblCurrentPasswordLabel() As Label
        Get
            Return CurrentPasswordLabel
        End Get
    End Property
    Public ReadOnly Property CurrentPasswordBox() As TextBox
        Get
            Return CurrentPassword
        End Get
    End Property
    Public ReadOnly Property NewPasswordBox() As TextBox
        Get
            Return NewPassword
        End Get
    End Property
    Public ReadOnly Property ChangePasswordButton() As Button
        Get
            Return ChangeButton
        End Get
    End Property
    Public ReadOnly Property NewPasswordRequiredFieldValidator() As RequiredFieldValidator
        Get
            Return Me.NewPasswordRequired
        End Get
    End Property
    Public ReadOnly Property ConfirmNewPasswordRequiredFieldValidator() As RequiredFieldValidator
        Get
            Return Me.ConfirmNewPasswordRequired
        End Get
    End Property
    Public ReadOnly Property CurrentPasswordRequiredFieldValidator() As RequiredFieldValidator
        Get
            Return Me.CurrentPasswordRequired
        End Get
    End Property

    Private _loginID As String
    Public Property LoginID() As String
        Get
            Return Me._loginID
        End Get
        Set(ByVal value As String)
            _loginID = value
        End Set
    End Property

    Private _doAutoLogin As Boolean
    Public Property DoAutoLogin() As Boolean
        Get
            Return Me._doAutoLogin
        End Get
        Set(ByVal value As Boolean)
            _doAutoLogin = value
        End Set
    End Property

    Private _displayControl As Boolean = True
    Public Property DisplayControl() As Boolean
        Get
            Return _displayControl
        End Get
        Set(ByVal value As Boolean)
            _displayControl = value
        End Set
    End Property


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        myDefaults = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.Common.Utilities.GetAllString 'GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ChangePassword.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        CurrentPasswordLabel.Text = ucr.Content("passwordLabel", _languageCode, True)                               ' Password:"
        ChangeButton.Text = ucr.Content("changePasswordButton", _languageCode, True)                                ' Change Password"
        TitleLabel.Text = ucr.Content("changePasswordTitle", _languageCode, True)                                   '  
        InstructionsLabel.Text = ucr.Content("PasswordInstructions", _languageCode, True)                           ' Enter you current password, followed by you new password, and then confirmation of your new password:"
        NewPasswordLabel.Text = ucr.Content("newPasswordLabel", _languageCode, True)                                ' New Password:"
        ConfirmNewPasswordLabel.Text = ucr.Content("confirmNewPasswordLabel", _languageCode, True)                  ' Confirm New Password:"

        NewPasswordCompare.ErrorMessage = ucr.Content("confirmPasswordCompareErrorMessage", _languageCode, True)
        CurrentPasswordRequired.ErrorMessage = ucr.Content("passwordRequiredErrorMessage", _languageCode, True)
        NewPasswordRequired.ErrorMessage = ucr.Content("newPasswordRequiredErrorMessage", _languageCode, True)
        ConfirmNewPasswordRequired.ErrorMessage = ucr.Content("confirmPasswordRequiredErrorMessage", _languageCode, True)

        NewPasswordRegEx.ValidationExpression = ucr.Attribute("PasswordExpression")
        NewPasswordRegEx.ErrorMessage = ucr.Content("newPasswordErrorMessage", _languageCode, True)

        ' If coming in with a login key then user is expect to enter a new password
        ' and wont know their old. 
        If Not Request("LoginKey") Is Nothing Then
            CurrentPassword.Visible = False
            CurrentPasswordLabel.Visible = False
            'If MyBase.ModuleDefaults.useEncryptedPassword Then
            ' Check for query string
        End If

        'If in agent mode dont show current password
        If Talent.eCommerce.Utilities.IsAgent Then
            CurrentPasswordLabel.Text = String.Empty
            CurrentPassword.Visible = False
        End If

    End Sub

    Protected Sub ChangeButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ChangeButton.Click

        Dim userName As String
        plhError.Visible = False
        If (String.Compare(CurrentPassword.Text, NewPassword.Text, False) = 0) Then
            plhError.Visible = True
            ErrorText.Text = ucr.Content("currentNewPasswordCompareError", _languageCode, True)
        Else
            If Not LoginID Is String.Empty And Not LoginID Is Nothing Then
                userName = LoginID
            Else
                userName = Profile.UserName
            End If

            If myDefaults.ExternalPasswordIsMaster Then

                Dim mem As New TalentMembershipProvider
                'If mem.ChangeBackendPassword(Profile.UserName, CurrentPassword.Text, NewPassword.Text) Then
                If mem.ChangeBackendPassword(userName, CurrentPassword.Text, NewPassword.Text) Then
                    mem.ChangePassword(userName, CurrentPassword.Text, NewPassword.Text)

                    '
                    ' Are we are auto-logging the user in after successfull password change?
                    If DoAutoLogin Then
                        If Talent.eCommerce.Utilities.loginUser(userName, NewPassword.Text) Then
                            '
                            'Update the record to set that the key has been used
                            Me.TDataObjects.ProfileSettings.tblForgottenPassword.SetTokenAsUsed(Request("LoginKey"), DateTime.Now)
                            Response.Redirect(Talent.eCommerce.Utilities.GetSiteHomePage())
                        End If
                    End If

                    If Request.QueryString("ReturnUrl") Is Nothing Then
                        Response.Redirect("~/PagesLogin/Profile/updateProfileConfirmation.aspx")
                    Else
                        Response.Redirect(Request.QueryString("ReturnUrl"))
                    End If
                Else
                    ErrorText.Text = ucr.Content("changePasswordFailure", _languageCode, True)
                End If

            Else
                Dim mem As New TalentMembershipProvider
                'If mem.ValidateUser(Profile.UserName, CurrentPassword.Text) Then
                'If mem.ChangePassword(Profile.UserName, CurrentPassword.Text, NewPassword.Text) Then
                If mem.ValidateUser(userName, CurrentPassword.Text) Then
                    If mem.ChangePassword(userName, CurrentPassword.Text, NewPassword.Text) Then
                        If Request.QueryString("ReturnUrl") Is Nothing Then
                            Response.Redirect("~/PagesLogin/Profile/updateProfileConfirmation.aspx")
                        Else
                            Response.Redirect(Request.QueryString("ReturnUrl"))
                        End If
                    Else
                        ErrorText.Text = ucr.Content("changePasswordFailure", _languageCode, True)
                    End If
                Else
                    ErrorText.Text = ucr.Content("currentPasswordError", _languageCode, True)
                End If
            End If
        End If

        If ErrorText.Text.Trim.Length > 0 Then
            CurrentPassword.Focus()
        End If

    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        'Should we display the password box
        Me.pnlPasswordBox.Visible = DisplayControl
        plhInstructionsLabel.Visible = (InstructionsLabel.Text.Length > 0)
    End Sub
    


End Class
