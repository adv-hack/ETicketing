Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesLogin_Profile_ValidatePassword
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfrPage As WebFormResource
    Private _languageCode As String = String.Empty

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _wfrPage = New WebFormResource
        _languageCode = TCUtilities.GetDefaultLanguage()
        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ValidatePassword.aspx"
        End With
        setupPageText()
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        plhErrorMessages.Visible = (blErrorMessages.Items.Count > 0)
    End Sub

    Protected Sub btnValidatePassword_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnValidatePassword.Click
        Dim err As New ErrorObj
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim customer As New TalentCustomer
        Dim customerV11 As New DECustomerV11
        Dim customerDataEntity As New DECustomer
        Dim validPassword As Boolean = False
        Dim passHash As New PasswordHash()

        customer.Settings = settings
        customerDataEntity.Password = txtPassword.Text
        customerDataEntity.UseEncryptedPassword = ModuleDefaults.UseEncryptedPassword
        If ModuleDefaults.UseEncryptedPassword = True Then
            customerDataEntity.HashedPassword = passHash.HashSalt(customerDataEntity.Password, ModuleDefaults.SaltString)
        End If
        If Profile.User.Details.LoginID Is Nothing AndAlso Profile.UserName.Contains("@") Then
            customerDataEntity.EmailAddress = Profile.UserName
        Else
            customerDataEntity.UserName = Profile.User.Details.LoginID
        End If
        customerDataEntity.Source = GlobalConstants.SOURCE
        customerV11.DECustomersV1.Add(customerDataEntity)
        customer.DeV11 = customerV11
        err = customer.VerifyPassword()

        If Not err.HasError AndAlso customer.ResultDataSet IsNot Nothing AndAlso
            customer.ResultDataSet.Tables.Count = 2 AndAlso customer.ResultDataSet.Tables(1).Rows.Count > 0 Then
            validPassword = TCUtilities.CheckForDBNull_Boolean_DefaultFalse(customer.ResultDataSet.Tables(1).Rows(0)(1))
            If validPassword Then
                If customer.Settings.DestinationDatabase <> GlobalConstants.SQL2005DESTINATIONDATABASE Then
                    TDataObjects.ProfileSettings.tblAuthorizedUsers.AddAuthorisedUser(Profile.UserName, PartnerCode, BusinessUnit)
                End If
                If TDataObjects.ProfileSettings.tblAuthorizedUsers.UpdatePasswordValidated(PartnerCode, BusinessUnit, Profile.UserName, True) > 0 Then
                    Dim redirectUrl As String = TEBUtilities.GetSiteHomePage()
                    If Session("SensitivePageURL") IsNot Nothing Then redirectUrl = Session("SensitivePageURL")
                    Response.Redirect(redirectUrl)
                Else
                    blErrorMessages.Items.Clear()
                    blErrorMessages.Items.Add(_wfrPage.Content("ErrorValidatingPassword", _languageCode, True))
                End If
            Else
                passwordValidationFailed()
            End If
        Else
            blErrorMessages.Items.Clear()
            blErrorMessages.Items.Add(_wfrPage.Content("ErrorValidatingPassword", _languageCode, True))
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set the page text properties
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setupPageText()
        ltlTitle.Text = _wfrPage.Content("TitleLabel", _languageCode, True)
        plhTitle.Visible = (ltlTitle.Text.Length > 0)
        ltlValidatePasswordLegend.Text = _wfrPage.Content("ValidatePasswordLegend", _languageCode, True)
        lblPassword.Text = _wfrPage.Content("PasswordLabel", _languageCode, True)
        btnValidatePassword.Text = _wfrPage.Content("ValidatePasswordButton", _languageCode, True)
        rfvPassword.ErrorMessage = _wfrPage.Content("PasswordRequired", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Handle the password validation attempt. Show the number of retries they are allowed.
    ''' If the user has used their maximum number of validation attempts. Delete the "Remember Me" cookie and force a logout.
    ''' The retry attempts are stored in session so that if they move to a different page the number of retry attempts isn't reset.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub passwordValidationFailed()
        Dim retryAttempts As Integer = 3
        If TCUtilities.CheckForDBNull_Int(_wfrPage.Attribute("ValidatePasswordAttempt")) > 0 Then
            retryAttempts = TCUtilities.CheckForDBNull_Int(_wfrPage.Attribute("ValidatePasswordAttempt"))
        End If
        If Session("ValidatePasswordAttemptsLeft") Is Nothing Then
            Session("ValidatePasswordAttemptsLeft") = retryAttempts - 1
        Else
            If Session("ValidatePasswordAttemptsLeft") > 1 Then
                Session("ValidatePasswordAttemptsLeft") = Session("ValidatePasswordAttemptsLeft") - 1
            Else
                Session("ValidatePasswordAttemptsLeft") = Nothing
                TEBUtilities.RemoveCookie(ModuleDefaults.RememberMeCookieName)
                Profile.CustomerLogout(TEBUtilities.GetSiteHomePage())
            End If
        End If
        Dim retryAttemptsMessage As String = _wfrPage.Content("RetryValidationMessage", _languageCode, True)
        retryAttemptsMessage = retryAttemptsMessage.Replace("<<RETRY_ATTEMPTS>>", Session("ValidatePasswordAttemptsLeft").ToString())
        blErrorMessages.Items.Clear()
        blErrorMessages.Items.Add(retryAttemptsMessage)
    End Sub

#End Region

End Class