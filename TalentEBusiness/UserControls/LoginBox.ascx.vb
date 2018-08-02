Imports Microsoft.VisualBasic
Imports Talent.eCommerce
Imports System.Web.Security

Partial Class UserControls_LoginBox
    Inherits ControlBase

#Region "Class level fields"

    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _usage As Display

#End Region

#Region "Public Properties"

    Public Enum Display
        STANDARD = 1
        HORIZONTAL = 2
    End Enum

    Public Property Usage() As Display
        Get
            Return _usage
        End Get
        Set(ByVal value As Display)
            _usage = value
        End Set
    End Property

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        CType(Login1.FindControl("UsernameFailureText"), Literal).Visible = False
        CType(Login1.FindControl("PasswordFailureText"), Literal).Visible = False
        CType(Login1.FindControl("DuplicateEmailFailureText"), Literal).Visible = False
        If Utilities.GetCurrentPageName().ToUpper = "LOGIN.ASPX" Then
            Me.Visible = True
        ElseIf Utilities.GetCurrentPageName().ToUpper = "CHECKOUTORDERCONFIRMATION.ASPX" Then
            Me.Visible = False
        Else
            Me.Visible = ModuleDefaults.Show_Mini_Login_Control
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Page.User.Identity.IsAuthenticated Then
            Dim currentPageName As String = Talent.eCommerce.Utilities.GetCurrentPageName().ToUpper()
            If currentPageName = "LOGGEDOUT.ASPX" OrElse currentPageName = "SESSIONERROR.ASPX" Then
                Dim allowRedirectToHome As Boolean = True
                If currentPageName = "SESSIONERROR.ASPX" Then
                    Dim myDefaults As New ECommerceModuleDefaults
                    Dim def As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()
                    If def.ValidatePartnerDirectAccess AndAlso def.SapOciPartner = TalentCache.GetPartner(HttpContext.Current.Profile).ToString() Then
                        FormsAuthentication.SignOut()
                        allowRedirectToHome = False
                    End If
                End If
                If allowRedirectToHome Then
                    Response.Redirect(Utilities.GetSiteHomePage())
                End If
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "LoginBox.ascx"
        End With

        If AgentProfile.IsAgent Then
            plhLogin.Visible = False
        Else
            plhLogin.Visible = True
            LoginStatus1.Visible = False
            If Usage.Equals(Display.HORIZONTAL) Then
                Login1.Visible = False
                Login2.Visible = True
            Else
                Login2.Visible = False
                Login1.Visible = True
            End If
            If Me.Page.User.Identity.IsAuthenticated Then
                If Usage.Equals(Display.HORIZONTAL) Then
                    LoginStatus1.Visible = True
                    Login1.Visible = False
                    Login2.Visible = False
                Else
                    Me.Visible = False
                End If
            Else
                Me.Visible = True
            End If
            If Not Page.IsPostBack Then
                If Not String.IsNullOrEmpty(Request.QueryString("error")) Then
                    processQuery(Request.QueryString("error"), sender)
                End If
            End If

            'Add autocomplte regardless of pre-poulate
            Dim ackey As String = "autocomplete"
            Dim acvalue As String = "off"
            Dim txtUsername As TextBox
            Dim txtPassword As TextBox
            Try
                If Login1.Visible Then
                    txtUsername = Login1.FindControl("UserName")
                    txtPassword = Login1.FindControl("Password")
                    txtUsername.Attributes.Add(ackey, acvalue)
                    txtPassword.Attributes.Add(ackey, acvalue)
                    If ModuleDefaults.RememberMeFunction Then
                        CType(Login1.FindControl("plhRemeberMe"), PlaceHolder).Visible = True
                        Dim securityMessage As String = _ucr.Content("RememberMeSecurityMessage", _languageCode, True)
                        If securityMessage.Length > 0 Then
                            CType(Login1.FindControl("plhSecurityMessage"), PlaceHolder).Visible = True
                            CType(Login1.FindControl("hplSecurityMessage"), HyperLink).Text = securityMessage
                        Else
                            CType(Login1.FindControl("plhSecurityMessage"), PlaceHolder).Visible = False
                        End If
                    Else
                        CType(Login1.FindControl("plhRemeberMe"), PlaceHolder).Visible = False
                    End If
                End If
                If Login2.Visible Then
                    txtUsername = Login2.FindControl("UserName")
                    txtPassword = Login2.FindControl("Password")
                    txtUsername.Attributes.Add(ackey, acvalue)
                    txtPassword.Attributes.Add(ackey, acvalue)
                End If
            Catch ex As Exception
            End Try
            For Each ctrl As Control In Page.Master.Controls
                If TypeOf ctrl Is HtmlForm Then
                    CType(ctrl, HtmlForm).Attributes.Add(ackey, acvalue)
                End If
            Next
            SetUsernameAppendJS()
            If Not Page.IsPostBack AndAlso Profile.IsAnonymous Then buildAddtionalOptionsList()
        End If
    End Sub

    Protected Sub Login1_LoggedIn(ByVal sender As Object, ByVal e As System.EventArgs) Handles Login1.LoggedIn
        LoginBoxes_LoggedIn(sender, e, Login1.UserName, Login1.Password)
    End Sub

    Protected Sub Login2_LoggedIn(ByVal sender As Object, ByVal e As System.EventArgs) Handles Login2.LoggedIn
        LoginBoxes_LoggedIn(sender, e, Login2.UserName, Login2.Password)
    End Sub

    Protected Sub Login1_LoggingIn(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.LoginCancelEventArgs) Handles Login1.LoggingIn
        Dim rblAddtionalOptions As RadioButtonList = CType(Login1.FindControl("rblAddtionalOptions"), RadioButtonList)
        If rblAddtionalOptions.Items.Count > 0 Then
            Session("SelectedLoginOption") = rblAddtionalOptions.SelectedValue
        End If
    End Sub

    Protected Sub Login1_LoginError(ByVal sender As Object, ByVal e As System.EventArgs) Handles Login1.LoginError
        Dim login As Login = CType(sender, Login)
        Dim storeLogin As String = login.UserName
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

        ' Check if need to lookup login id as an external key  i.e. so Ticketing users can login with memb no or email address.
        'Was the error generated from the backend
        If def.RefreshCustomerDataOnLogin AndAlso def.ExternalPasswordIsMaster AndAlso Not Session("BackEndLoginError") Is Nothing Then
            CType(CType(sender, Login).FindControl("ErrorLabel"), Label).Visible = True
            Dim errMsg As Talent.Common.TalentErrorMessages
            errMsg = New Talent.Common.TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
            CType(CType(sender, Login).FindControl("ErrorLabel"), Label).Text = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                                                                                Talent.eCommerce.Utilities.GetCurrentPageName, _
                                                                                                Session("BackEndLoginError")).ERROR_MESSAGE
            loginErrorMessage("BackEndLoginError")
        Else
            Dim profile1 As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter
            Dim dt As Data.DataTable = profile1.GetDataByEmail(Login1.UserName.Trim, TalentCache.GetPartner(HttpContext.Current.Profile))

            'Set the real loginid 
            If dt.Rows.Count = 1 Then
                Login1.UserName = dt.Rows(0).Item("LOGINID")
            Else
                'Add leading zeros if the login id is the customer id
                'If CType(def.LoginidIsCustomerNumber, Boolean) Then
                If def.LoginidType.Equals("1") Then
                    Login1.UserName = Talent.Common.Utilities.PadLeadingZeros(Login1.UserName.Trim, "12")
                End If
            End If

            'Check for duplicate email address
            If dt.Rows.Count > 1 Then
                CType(CType(sender, Login).FindControl("DuplicateEmailFailureText"), Literal).Visible = True
                loginErrorMessage("DuplicateEmailFailureText")
            Else
                'Validate the userid
                If login.UserName.Contains(",") OrElse Membership.GetUser(Login1.UserName.Trim, False) Is Nothing Then
                    CType(CType(sender, Login).FindControl("UsernameFailureText"), Literal).Visible = True
                    loginErrorMessage("UsernameFailureText")
                Else
                    CType(CType(sender, Login).FindControl("PasswordFailureText"), Literal).Visible = True
                    loginErrorMessage("PasswordFailureText")
                End If
            End If
        End If
        CType(Login1.FindControl("plhLoginFailure"), PlaceHolder).Visible = True

        'Reset the login id in case of error
        Dim test As Integer = 0
        If Integer.TryParse(storeLogin, test) Then storeLogin = CInt(storeLogin).ToString
        Login1.UserName = storeLogin
        Session("BackEndLoginError") = Nothing
    End Sub

    Protected Sub Login2_LoginError(ByVal sender As Object, ByVal e As System.EventArgs) Handles Login2.LoginError
        If Usage.Equals(Display.HORIZONTAL) Then
            Dim redirectUrl As String = "~/PagesPublic/Login/login.aspx?error="
            If Membership.GetUser(Login2.UserName.Trim, False) Is Nothing Then
                redirectUrl += "un"
            Else
                redirectUrl += "pw"
            End If
            Response.Redirect(redirectUrl)
        End If
    End Sub

    Protected Sub loginErrorMessage(ByVal ErrorMessage As String)
        'Display a login error message depending on what error has occured
        'The same message will be displayed acording to PCI spec
        Dim SingularErrorLabel As Label
        Dim ErrorLabel As Label
        Dim PasswordFailureText As Literal
        Dim UsernameFailureText As Literal
        Dim DuplicateEmailFailureText As Literal

        Try
            'Hide all Error messages
            ErrorLabel = Login1.FindControl("ErrorLabel")
            PasswordFailureText = Login1.FindControl("PasswordFailureText")
            UsernameFailureText = Login1.FindControl("UsernameFailureText")
            SingularErrorLabel = Login1.FindControl("SingularErrorLabel")
            DuplicateEmailFailureText = Login1.FindControl("DuplicateEmailFailureText")
            ErrorLabel.Visible = False
            PasswordFailureText.Visible = False
            UsernameFailureText.Visible = False
            DuplicateEmailFailureText.Visible = False

            'Display singular error message label with particular text
            Dim errMsg As Talent.Common.TalentErrorMessages
            errMsg = New Talent.Common.TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)

            Select Case ErrorMessage
                Case "PasswordFailureText"
                    SingularErrorLabel.Text = errMsg.GetErrorMessage("SEL1").ERROR_MESSAGE
                    SingularErrorLabel.Visible = True
                Case "UsernameFailureText"
                    SingularErrorLabel.Text = errMsg.GetErrorMessage("SEL2").ERROR_MESSAGE
                    SingularErrorLabel.Visible = True
                Case "BackEndLoginError"
                    SingularErrorLabel.Text = errMsg.GetErrorMessage("SEL3").ERROR_MESSAGE
                    SingularErrorLabel.Visible = True
                Case "DuplicateEmailFailureText"
                    SingularErrorLabel.Text = errMsg.GetErrorMessage("SEL4").ERROR_MESSAGE
                    SingularErrorLabel.Visible = True
            End Select
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region "Public Method"

    Public Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        With _ucr
            Select Case sender.ID.ToString
                Case Is = "TitleLabel"
                    Dim content As String = .Content("TitleText", _languageCode, True)
                    If content.Length > 0 Then
                        CType(sender, Literal).Text = content
                        CType(Login1.FindControl("plhTitle"), PlaceHolder).Visible = True
                    Else
                        CType(Login1.FindControl("plhTitle"), PlaceHolder).Visible = False
                    End If
                Case Is = "LoginInstructionsLabel"
                    Dim content As String = .Content("LoginInstructionsText", _languageCode, True)
                    If content.Length > 0 Then
                        CType(sender, Literal).Text = content
                        CType(Login1.FindControl("plhInstructions"), PlaceHolder).Visible = True
                    Else
                        CType(Login1.FindControl("plhInstructions"), PlaceHolder).Visible = False
                    End If
                Case Is = "UserNameLabel"
                    CType(sender, Label).Text = .Content("UsernameLabelText", _languageCode, True)
                Case Is = "PasswordLabel"
                    CType(sender, Label).Text = .Content("PasswordLabelText", _languageCode, True)
                Case Is = "ForgottenPasswordLink"
                    Dim content As String = .Content("PasswordRecoveryText", _languageCode, True)
                    If content.Length > 0 Then
                        CType(sender, HyperLink).Text = .Content("PasswordRecoveryText", _languageCode, True)
                        CType(sender, HyperLink).NavigateUrl = .Content("PasswordRecoveryUrl", _languageCode, True)
                        CType(Login1.FindControl("plhForgottenPassword"), PlaceHolder).Visible = True
                    Else
                        CType(Login1.FindControl("plhForgottenPassword"), PlaceHolder).Visible = False
                    End If
                Case Is = "RegisterLink"
                    Dim content As String = .Content("RegisterLinkText", _languageCode, True)
                    If content.Length > 0 Then
                        CType(sender, HyperLink).Text = content
                        CType(sender, HyperLink).NavigateUrl = .Content("RegisterLinkURL", _languageCode, True)
                        CType(Login1.FindControl("plhRegisterLink"), PlaceHolder).Visible = True
                    Else
                        CType(Login1.FindControl("plhRegisterLink"), PlaceHolder).Visible = False
                    End If
                Case Is = "LoginButton"
                    Dim loginBtn As Button = CType(sender, Button)
                    loginBtn.Text = .Content("LoginButtonText", _languageCode, True)
                Case Is = "PasswordFailureText"
                    CType(sender, Literal).Text = .Content("PasswordFailedLoginText", _languageCode, True)
                Case Is = "UsernameFailureText"
                    CType(sender, Literal).Text = .Content("UsernameFailedLoginText", _languageCode, True)
                Case Is = "DuplicateEmailFailureText"
                    CType(sender, Literal).Text = .Content("DuplicateEmailFailureText", _languageCode, True)
                Case Is = "PasswordRequired"
                    CType(sender, RequiredFieldValidator).ErrorMessage = .Content("PasswordRequiredErrorMessage", _languageCode, True)
                Case Is = "UserNameRequired"
                    CType(sender, RequiredFieldValidator).ErrorMessage = .Content("UsernameRequiredErrorMessage", _languageCode, True)
                Case Is = "plhRegisterLink"
                    ' Display registration link?
                    CType(sender, PlaceHolder).Visible = CType(Me.Page, TalentBase01).ModuleDefaults.RegistrationEnabled
                Case Is = "ltlLoginLegend"
                    CType(sender, Literal).Text = .Content("LoginLegend", _languageCode, True)
                Case Is = "RememberMe"
                    CType(sender, CheckBox).Text = .Content("RememberMeText", _languageCode, True)
                Case Is = "ltlPasswordLink"
                    CType(sender, Literal).Text = .Content("UnderPasswordLinkText", _languageCode, True)
                    CType(Login1.FindControl("plhPasswordLink"), PlaceHolder).Visible = (CType(sender, Literal).Text.Trim().Length > 0)
            End Select
        End With
    End Sub

    Public Sub SetEnterKeyAction(ByVal sender As Object, ByVal e As EventArgs)
        Dim tbox As TextBox = CType(sender, TextBox)
        Dim loginBtn As Button = CType(CType(sender, TextBox).Parent.FindControl("LoginButton"), Button)
        Const onKeyDown As String = "onkeydown"
        Dim EventText As String = "if(event.which || event.keyCode) " & _
                                    "{ " & _
                                        "if ((event.which == 13) || (event.keyCode == 13))" & _
                                        "{ " & _
                                            "document.getElementById('" & loginBtn.ClientID & "').focus();" & _
                                            "return true" & _
                                        "} " & _
                                    "} " & _
                                    "else " & _
                                    "{ " & _
                                        "return true" & _
                                    "}; "
        tbox.Attributes.Add(onKeyDown, EventText)
    End Sub

#End Region

#Region "Private Methods"

    Private Sub LoginBoxes_LoggedIn(ByVal sender As Object, ByVal e As EventArgs, ByVal username As String, ByVal password As String)
        Dim loginEmail As String = username.Trim
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        Dim loginerr As Boolean = False
        Session("TemplateIDs") = Nothing
        Session("AddInfoCompleted") = Nothing
        Utilities.ClearDeliveryAddressCntrlSessions()
        Try
            With _ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "LoginBox.ascx"
            End With

            'Use the values from session if we have refreshed from the backend.  Saves on DB access!!!
            If def.RefreshCustomerDataOnLogin AndAlso def.ExternalPasswordIsMaster AndAlso Not Session("BackEndUserName") Is Nothing Then
                If username.ToUpper <> Session("BackEndUserName").ToString.ToUpper Then
                    username = Session("BackEndUserName")
                    'FormsAuthentication.SignOut()
                    'FormsAuthentication.Authenticate(username, password)
                    'Membership.ValidateUser(username, password)
                    FormsAuthentication.SetAuthCookie(username, False)
                    loginEmail = Session("BackEndEmail")
                End If
            Else
                If def.UseLoginLookup Then
                    Dim profile1 As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter
                    Dim authPartners As New TalentMembershipDatasetTableAdapters.tbl_authorized_partnersTableAdapter
                    Dim dt As Data.DataTable
                    If authPartners.Get_CheckFor_B2C_Login(TalentCache.GetBusinessUnitGroup).Rows.Count > 0 Then
                        dt = profile1.GetDataByEmail(username, TalentCache.GetPartner(HttpContext.Current.Profile))
                    Else
                        dt = profile1.GetDataByEmailNoPartner(username)
                    End If
                    If dt.Rows.Count > 0 Then
                        username = dt.Rows(0).Item("LOGINID")
                        FormsAuthentication.SignOut()
                        Membership.ValidateUser(dt.Rows(0).Item("LOGINID"), password)
                        FormsAuthentication.SetAuthCookie(dt.Rows(0).Item("LOGINID"), False)
                    Else
                        ' Add leading zeros to customer number 
                        'If CType(def.LoginidIsCustomerNumber, Boolean) Then
                        If def.LoginidType.Equals("1") Then
                            If username.Length <> 12 Then
                                username = Talent.Common.Utilities.PadLeadingZeros(username.Trim, "12")
                                FormsAuthentication.SignOut()
                                'FormsAuthentication.Authenticate(username, password)
                                Membership.ValidateUser(username, password)
                                FormsAuthentication.SetAuthCookie(username, False)
                            End If
                        End If

                        'We may be logging in with customer number.  Retrieve the email address from 
                        'the login lookup table as we still need to validate that it's valid.
                        dt = profile1.GetByLoginIDAndPartner(username, TalentCache.GetPartner(HttpContext.Current.Profile))
                        If dt.Rows.Count = 1 Then
                            loginEmail = Utilities.CheckForDBNull_String(dt.Rows(0).Item("EMAIL"))
                        End If
                    End If
                End If
            End If

            setRememberMeCookie(username)

            'Validate the email address
            If CBool(Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("CheckForUsernameAsEmail"))) AndAlso loginEmail.Contains("@") AndAlso loginEmail.Contains(".") Then
                If String.IsNullOrEmpty(Request.QueryString("returnUrl")) Then
                    If Not String.IsNullOrEmpty(def.PAGE_AFTER_LOGIN) Then
                        Response.Redirect(def.PAGE_AFTER_LOGIN)
                    End If
                End If
            Else
                If CBool(Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("CheckForUsernameAsEmail"))) Then
                    Response.Redirect("~/PagesLogin/Profile/updateProfile.aspx?reason=username")
                Else
                    If String.IsNullOrEmpty(Request.QueryString("returnUrl")) Then
                        If Not String.IsNullOrEmpty(def.PAGE_AFTER_LOGIN) Then
                            Response.Redirect(def.PAGE_AFTER_LOGIN)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
        End Try

        'Remove the session entries
        Session("BackEndLoginError") = Nothing
        Session("BackEndUserName") = Nothing
        Session("BackEndEmail") = Nothing

        If def.CREDIT_CHECK_ON_LOGIN OrElse def.StatusCheckOnLogin Then
            loginerr = DoCreditCheck(username, def.StatusCheckOnLogin)
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnUrl")) Then
            Dim returnUrl As String = Request.QueryString("returnUrl")
            If returnUrl.Contains("/Redirect/TicketingGateway.aspx?page") AndAlso Not returnUrl.Contains("function") AndAlso Not String.IsNullOrEmpty(Request.QueryString("function")) Then
                returnUrl += "&function=" & Request.QueryString("function")
            End If

            Response.Redirect(returnUrl)


        Else
            If Utilities.GetCurrentPageName().ToUpper() = "BASKET.ASPX" Then
                Session("FromRightBarLogin") = "TRUE"
                Response.Redirect(Utilities.GetSiteHomePage())
            End If
        End If
    End Sub

    Private Sub processQuery(ByVal queryString As String, ByVal sender As Object)
        If Not Usage.Equals(Display.HORIZONTAL) Then
            If String.IsNullOrEmpty(queryString) Then
                CType(CType(Login1, Login).FindControl("ErrorLabel"), Label).Visible = True
                Dim errMsg As Talent.Common.TalentErrorMessages
                errMsg = New Talent.Common.TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
                CType(CType(Login1, Login).FindControl("ErrorLabel"), Label).Text = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                                                                                Talent.eCommerce.Utilities.GetCurrentPageName, _
                                                                                                Session("BackEndLoginError")).ERROR_MESSAGE
                'Pass error message text to Error Message Sub
                loginErrorMessage("BackEndLoginError")
            Else
                Select Case queryString.ToLower
                    Case "pw"
                        CType(Login1.FindControl("PasswordFailureText"), Literal).Visible = True
                        loginErrorMessage("PasswordFailureText")
                    Case "un"
                        CType(Login1.FindControl("UsernameFailureText"), Literal).Visible = True
                        loginErrorMessage("UsernameFailureText")
                End Select
            End If
        End If
    End Sub

    Private Sub buildAddtionalOptionsList()
        Dim showAddtionalOptions As Boolean = Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowAddtionalOptions"))
        If showAddtionalOptions Then
            Dim defaultItemNumber As Integer = 1
            defaultItemNumber = Utilities.CheckForDBNull_Int(_ucr.Attribute("DefaultSelectedItemNumber"))
            CType(Login1.FindControl("plhAddtionalOptions"), PlaceHolder).Visible = True
            Dim rblAddtionalOptions As RadioButtonList = CType(Login1.FindControl("rblAddtionalOptions"), RadioButtonList)
            For i As Integer = 0 To 9
                Dim itemTextValue As String = _ucr.Content(String.Format("rblAddtionalOption{0}", i), _languageCode, True)
                If itemTextValue.Length > 0 Then
                    Dim rdoItem As New ListItem
                    rdoItem.Text = _ucr.Content(String.Format("rblAddtionalOption{0}", i), _languageCode, True)
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

    ''' <summary>
    ''' Set the "Remember Me" cookie if it is configured to be used
    ''' </summary>
    ''' <param name="username">The customer username (NEEDS TO ALWAYS BE THE CUSTOMER NUMBER)</param>
    ''' <remarks></remarks>
    Private Sub setRememberMeCookie(ByVal username As String)
        If ModuleDefaults.RememberMeFunction AndAlso CType(Login1.FindControl("RememberMe"), CheckBox).Checked Then
            If ModuleDefaults.RememberMeCookieEncodeKey.Length > 0 Then
                Dim cookieName As String = ModuleDefaults.RememberMeCookieName
                If cookieName.Length = 0 Then cookieName = "KMLI"
                Dim noOfDaysUntilCookieExpires As Double = 14
                If ModuleDefaults.RememberMeCookieExiryDays > 0 Then
                    noOfDaysUntilCookieExpires = CDbl(ModuleDefaults.RememberMeCookieExiryDays)
                End If
                Dim cookieUsername As New System.Web.HttpCookie(cookieName, Talent.Common.Utilities.TripleDESEncode(username, ModuleDefaults.RememberMeCookieEncodeKey))
                cookieUsername.Expires = Now.AddDays(noOfDaysUntilCookieExpires)
                HttpContext.Current.Response.Cookies.Add(cookieUsername)
            End If
        End If
    End Sub

    Private Sub SetUsernameAppendJS()
        Try
            Dim txtUsername As TextBox = Nothing
            Dim txtPassword As TextBox = Nothing
            If Login1.Visible Then
                txtUsername = Login1.FindControl("UserName")
                txtPassword = Login1.FindControl("Password")
            End If
            If Login2.Visible Then
                txtUsername = Login2.FindControl("UserName")
                txtPassword = Login1.FindControl("Password")
            End If
            If txtUsername IsNot Nothing Then
                If ModuleDefaults.RefreshCustomerDataOnLogin AndAlso ModuleDefaults.ExternalPasswordIsMaster Then
                    'txtUsername.Attributes.Add("onblur", "AppendUsername('true','" & txtUsername.ClientID & "')")
                    'txtPassword.Attributes.Add("onblur", "AppendUsername('true','" & txtUsername.ClientID & "')")
                End If

            End If
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region "Private Functions"

    Private Function DoCreditCheck(ByVal usr As String, ByVal doStatusCheck As Boolean) As Boolean
        Dim crederr As Boolean = False
        HttpContext.Current.Profile.Initialize(usr, True)
        Dim tempProfile As ProfileCommon = CType(HttpContext.Current.Profile, ProfileCommon).GetProfile(usr)
        If tempProfile IsNot Nothing Then
            Dim deCred As New Talent.Common.DECreditCheck(tempProfile.User.Details.Account_No_1)
            Dim dbCred As New Talent.Common.DBCreditCheck
            Dim credCheck As New Talent.Common.TalentCreditCheck
            Dim settings As New Talent.Common.DESettings
            settings.AccountNo1 = tempProfile.User.Details.Account_No_1
            settings.AccountNo2 = tempProfile.User.Details.Account_No_2
            deCred.TotalOrderValue = tempProfile.GetMinimumPurchaseAmount
            settings.BusinessUnit = TalentCache.GetBusinessUnit
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            credCheck.Settings = settings
            credCheck.CreditCheck = deCred

            Dim err As Talent.Common.ErrorObj = credCheck.PerformCreditLimitCheck()
            If Not err.HasError Then
                Try
                    Dim dt As Data.DataTable = credCheck.ResultDataSet.Tables("CreditCheckHeader")
                    If dt.Rows.Count > 0 Then
                        Dim credStatus As String = dt.Rows(0)("CreditStatus")
                        Select Case credStatus
                            Case Is = "2", "3", "4"
                                'dont allow login for disabled accounts
                                If doStatusCheck AndAlso credStatus = "3" Then
                                    FormsAuthentication.SignOut()
                                    crederr = True
                                    'loginErrorMessage("UsernameFailureText")
                                    HttpContext.Current.Response.Redirect("~/PagesPublic/Login/login.aspx?error=un")
                                Else
                                    If Not doStatusCheck Then
                                        HttpContext.Current.Response.Redirect("~/PagesLogin/Profile/MyAccount.aspx?error=CC&status=" & credStatus)
                                    End If
                                End If
                        End Select
                    End If
                Catch ex As Exception
                End Try
            End If
        End If
        Return crederr
    End Function

#End Region

End Class