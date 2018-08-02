Imports Talent.Common
Imports Talent.eCommerce
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Data

Partial Class PagesPublic_Agent_AgentLogin
    Inherits TalentBase01

#Region "Class Level Fields"
    'don't use GetSettingsObject instead use GetSettingsObjectForAgent
    Private _wfrPage As New WebFormResource
    Private _languageCode As String = String.Empty
    Private _agentPassword As String = String.Empty

#End Region

#Region "Constants"

    Const KEYCODE As String = "AgentLogin.aspx"
    Const GENERALAGENTLOGINERROR As String = "GENERALAGENTLOGINERROR"

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = KEYCODE
        End With
        _languageCode = TCUtilities.GetDefaultLanguage()
        If Request.QueryString("logout") IsNot Nothing Then
            If Request.QueryString("logout").ToUpper = "TRUE" Then
                LogoutAgent()
            End If
            If Request.QueryString("logout").ToUpper = "EXPIRED" Then
                LogoutAgentOnSessionExpire()
            End If
        End If
        If Session("agentPassword") IsNot Nothing AndAlso Session("agentPassword") IsNot String.Empty Then
            _agentPassword = Session("agentPassword")
        Else
            _agentPassword = txtPassword.Text
        End If
        Session("agentPassword") = Nothing

        btnLogoutLogin.Visible = AgentProfile.IsAgent
        plhAgentAlreadyLoggedIn.Visible = AgentProfile.IsAgent
        plhAgentLogin.Visible = Not AgentProfile.IsAgent
        SetControlDisplayText()

        If Not AgentProfile.IsAgent Then
            LogoutCustomer()
            Session.Remove("LastNCustomerLogins")
            txtUserName.Attributes.Add("placeholder", lblUsername.Text)
            txtPassword.Attributes.Add("placeholder", lblPassword.Text)
        Else
            Dim scriptString As String = TEBUtilities.CheckForDBNull_String(_wfrPage.Content("TxtUsernameAutoFocusScript", _languageCode, True)).Trim
            If scriptString.Trim.Length > 0 Then
                scriptString = scriptString.Replace("<<<TEXTBOX_ID>>>", txtUserName.ClientID)
                Page.ClientScript.RegisterStartupScript(Me.GetType(), "AutoFocusScript", scriptString)
                If Not Page.IsPostBack() And ModuleDefaults.ShowAgentDepartment Then
                    LoadDepartments()
                End If
            End If
        End If
    End Sub

    Private Sub SetControlDisplayText()
        ltlHeader.Text = _wfrPage.Content("HeaderText", _languageCode, True)
        ltlFieldsetLegend.Text = _wfrPage.Content("FieldsetLegend", _languageCode, True)
        ltlAlreadyLoggedIn.Text = _wfrPage.Content("AlreadyLoggedInText", _languageCode, True)
        lblUsername.Text = _wfrPage.Content("UsernameText", _languageCode, True)
        lblPassword.Text = _wfrPage.Content("PasswordText", _languageCode, True)
        lblDepartment.Text = _wfrPage.Content("DepartmentText", _languageCode, True)
        btnLogin.Text = _wfrPage.Content("LoginButtonText", _languageCode, True)
        btnBackToLogin.Text = _wfrPage.Content("BackButtonText", _languageCode, True)
        btnLogout.Text = _wfrPage.Content("LogoutButtonText", _languageCode, True)
        btnLogoutLogin.Text = _wfrPage.Content("LogoutLoginButtonText", _languageCode, True)
        rfvUserName.ErrorMessage = _wfrPage.Content("RequiredUsernameText", _languageCode, True)
        rfvPassword.ErrorMessage = _wfrPage.Content("RequiredPasswordText", _languageCode, True)
        rfvDepartment.ErrorMessage = _wfrPage.Content("RequiredDepartmentText", _languageCode, True)
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If btnLogoutLogin.Visible Then
            ltlErrorMessage.Text = _wfrPage.Content("AnotherSessionFoundErrorText", _languageCode, True)
            plhHeader.Visible = False
            plhUserName.Visible = False
            plhPassword.Visible = False
            plhDepartment.Visible = False
            btnLogin.Visible = False
            btnBackToLogin.Visible = True
        Else
            btnLogin.Visible = True
            btnBackToLogin.Visible = False
        End If
        plhHeader.Visible = (ltlHeader.Text.Length > 0)
        plhErrorMessage.Visible = (ltlErrorMessage.Text.Length > 0)
        plhDepartment.Visible = ModuleDefaults.ShowAgentDepartment AndAlso ddlDepartment.Items.Count > 0
    End Sub

    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogin.Click
        LoginAgent(True)
    End Sub

    Protected Sub btnLogout_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnLogout.Click
        LogoutAgent()
    End Sub

    Protected Sub btnLogoutLogin_Click(sender As Object, e As EventArgs) Handles btnLogoutLogin.Click
        'Remove the tbl_active_noise_sessions records on all servers
        Dim sessionStartTime As DateTime = Now
        Dim err As New ErrorObj
        Dim agentSettings As DESettings = TEBUtilities.GetSettingsObjectForAgent
        Dim myNoise As New TalentNoise(agentSettings, Session.SessionID, sessionStartTime,
                                       sessionStartTime.AddMinutes(-ModuleDefaults.NOISE_THRESHOLD_MINUTES),
                                       ModuleDefaults.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES, True,
                                       TblActiveNoiseSessions_Usage.TICKETING, 0, txtUserName.Text.ToUpper.Trim())

        myNoise.MultipleSQLConnectionStrings = TalAdminDataObjects.TalentAdminSettings.GetConnectionStringListForMultiDB(ModuleDefaults.TalentAdminClientName, ModuleDefaults.IsTestOrLive)
        err = myNoise.RemoveSpecificNoiseSessionByAgentName_MultiDBs()

        'Remove the tbl_agent records on all servers
        TDataObjects.Settings.ConnectionStringList = myNoise.MultipleSQLConnectionStrings
        TDataObjects.AgentSettings.TblAgent.Delete_MultiDB(txtUserName.Text.ToUpper)

        LoginAgent(False)
    End Sub

    Protected Sub btnBackToLogin_Click(sender As Object, e As EventArgs) Handles btnBackToLogin.Click
        Response.Redirect(Request.CurrentExecutionFilePath)
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Populate the Agent class properties and update tbl_active_noise_sessions
    ''' </summary>
    ''' <param name="agentDataEntity">DEAgent to use with agent population</param>
    ''' <returns>An error object</returns>
    ''' <remarks></remarks>
    Private Function performSessionUpdate(ByVal agentDataEntity As DEAgent) As ErrorObj
        AgentProfile.Populate(True, agentDataEntity.AgentUsername, agentDataEntity.AgentType)
        Dim err As New ErrorObj
        Dim sessionStartTime As DateTime = Now
        'don't use GetSettingsObject instead use GetSettingsObjectForAgent
        Dim noiseSettings As DESettings = TEBUtilities.GetSettingsObjectForAgent
        Dim myNoise As New TalentNoise(noiseSettings, Session.SessionID, sessionStartTime, _
                                       sessionStartTime.AddMinutes(-ModuleDefaults.NOISE_THRESHOLD_MINUTES), _
                                       ModuleDefaults.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES, AgentProfile.IsAgent, _
                                       TblActiveNoiseSessions_Usage.TICKETING, AgentProfile.Type, AgentProfile.Name)
        err = myNoise.AddOrUpdateNoiseSession()
        If Not err.HasError AndAlso myNoise.SuccessfullCall AndAlso myNoise.RowsAffected > 0 Then
            err.HasError = False
        End If
        Return err
    End Function

    ''' <summary>
    ''' Check to see if the agent is already logged in (has a tbl_active_noise_session record or tbl_agent record or both)
    ''' </summary>
    ''' <param name="checkSessionTables">Set this flag if it is required to make this check</param>
    ''' <returns>True if the agent is logged in otherwise false</returns>
    ''' <remarks></remarks>
    Private Function isAlreadyLoggedIn(ByRef checkSessionTables As Boolean) As Boolean
        Dim alreadyLoggedIn As Boolean = False
        If checkSessionTables Then
            Dim sessionStartTime As DateTime = Now
            Dim err As New ErrorObj
            Dim agentSettings As DESettings = TEBUtilities.GetSettingsObjectForAgent
            Dim myNoise As New TalentNoise(agentSettings, Session.SessionID, sessionStartTime, _
                                           sessionStartTime.AddMinutes(-ModuleDefaults.NOISE_THRESHOLD_MINUTES), _
                                           ModuleDefaults.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES, True, _
                                           TblActiveNoiseSessions_Usage.TICKETING, 0, txtUserName.Text.ToUpper.Trim())

            myNoise.MultipleSQLConnectionStrings = TalAdminDataObjects.TalentAdminSettings.GetConnectionStringListForMultiDB(ModuleDefaults.TalentAdminClientName, ModuleDefaults.IsTestOrLive)
            err = myNoise.CheckForExistingAgentNoiseSession_MultiDBs
            If Not err.HasError AndAlso myNoise.SuccessfullCall AndAlso myNoise.UsersOnLine > 0 Then
                err.HasError = False
                alreadyLoggedIn = True
            End If

            TDataObjects.Settings.ConnectionStringList = myNoise.MultipleSQLConnectionStrings
            If TDataObjects.AgentSettings.TblAgent.GetByAgentName_MultiDB(txtUserName.Text.ToUpper) > 0 Then
                alreadyLoggedIn = True
            End If
        End If
        Return alreadyLoggedIn
    End Function

#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Functionality to Login the agent
    ''' </summary>
    ''' <param name="checkForAlreadyLoggedIn">Do we need to check if we are already logged in</param>
    ''' <remarks></remarks>
    Private Sub LoginAgent(ByVal checkForAlreadyLoggedIn As Boolean)
        If Page.IsValid Then
            Dim errMsg As TalentErrorMessages
            Dim talentErrorMsg As TalentErrorMessage
            Dim talAgent As New TalentAgent
            Dim err As New ErrorObj
            Dim settings As DESettings = TEBUtilities.GetSettingsObjectForAgent()
            Dim agentDataEntity As New DEAgent
            ltlErrorMessage.Text = String.Empty
            errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, _wfrPage.PartnerCode, _wfrPage.FrontEndConnectionString)
            agentDataEntity.AgentUsername = txtUserName.Text.ToUpper
            agentDataEntity.AgentPassword = _agentPassword
            agentDataEntity.Source = GlobalConstants.SOURCE
            agentDataEntity.BulkSalesMode = False
            agentDataEntity.SessionID = Profile.Basket.Basket_Header_ID
            agentDataEntity.Department = ddlDepartment.SelectedValue
            talAgent.AgentDataEntity = agentDataEntity
            talAgent.Settings = settings
            err = talAgent.AgentLogin()

            If err.HasError Then
                plhErrorMessage.Visible = True
                talentErrorMsg = errMsg.GetErrorMessage(GENERALAGENTLOGINERROR)
                ltlErrorMessage.Text = talentErrorMsg.ERROR_MESSAGE
            Else
                If String.IsNullOrWhiteSpace(agentDataEntity.ErrorCode) Then
                    If isAlreadyLoggedIn(checkForAlreadyLoggedIn) Then
                        plhAgentAlreadyLoggedIn.Visible = False
                        btnLogoutLogin.Visible = True
                        Session("agentPassword") = _agentPassword
                    Else
                        If Not err.HasError Then
                            If ModuleDefaults.AllowAgentPreferencesUpdates Then
                                err = talAgent.RetrieveAgentPrinters
                            End If
                        End If
                        Session("TemplateIDs") = Nothing
                        Session("AddInfoCompleted") = Nothing
                        err = performSessionUpdate(agentDataEntity)
                        If err.HasError Then
                            AgentProfile.Clear()
                            plhErrorMessage.Visible = True
                            talentErrorMsg = errMsg.GetErrorMessage(GENERALAGENTLOGINERROR)
                            ltlErrorMessage.Text = talentErrorMsg.ERROR_MESSAGE
                        Else
                            UpdateAgentDetails(agentDataEntity)
                            AgentProfile.PopulateAgentPermissions()
                            plhErrorMessage.Visible = False
                            Dim redirectUrl As String = ModuleDefaults.REDIRECT_AFTER_AGENT_LOGIN_URL
                            If Request.QueryString("ReturnUrl") IsNot Nothing Then
                                redirectUrl = Request.QueryString("ReturnUrl").ToString()
                            End If
                            'success refresh basket content if any items in the basket
                            If Profile.Basket.BasketItems.Count > 0 Then
                                Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
                                ticketingGatewayFunctions.RefreshBasketContent()
                            End If
                            If redirectUrl.Length > 0 Then
                                Response.Redirect(redirectUrl)
                            Else
                                Response.Redirect("~/PagesPublic/Profile/CustomerSelection.aspx")
                            End If
                        End If
                    End If
                Else
                    plhErrorMessage.Visible = True
                    talentErrorMsg = errMsg.GetErrorMessage(agentDataEntity.ErrorCode)
                    ltlErrorMessage.Text = talentErrorMsg.ERROR_MESSAGE
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Perform the Agent Logout
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LogoutAgent()
        clearAgentSpecificSessionValues()
        LogoutCustomer()
        Profile.Basket.EmptyBasket()
        If Not String.IsNullOrWhiteSpace(_wfrPage.Attribute("DisplayEndOfDayOnLogout")) AndAlso
            CType(_wfrPage.Attribute("DisplayEndOfDayOnLogout"), Boolean) Then
            Session("AgentLogout") = True
            HttpContext.Current.Response.Redirect("~/PagesAgent/Orders/EndOfDay.aspx")
        Else
            AgentProfile.Logout()
            Dim redirectUrl As String = ModuleDefaults.REDIRECT_AFTER_AGENT_LOGOUT_URL
            If redirectUrl.Length > 0 Then
                HttpContext.Current.Response.Redirect(redirectUrl)
            Else
                HttpContext.Current.Response.Redirect(Talent.eCommerce.Utilities.GetSiteHomePage())
            End If
        End If
    End Sub

    ''' <summary>
    ''' Clear the Agent Specific Session Values from the HospitalityBookingEnquiry.aspx page
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub clearAgentSpecificSessionValues()
        Session("HospitalityBookingEnquiryAgent") = Nothing
        Session("HospitalityBookingEnquiryCallID") = Nothing
        Session("HospitalityBookingEnquiryFromDate") = Nothing
        Session("HospitalityBookingEnquiryToDate") = Nothing
        Session("HospitalityBookingEnquiryStatus") = Nothing
        Session("HospitalityBookingEnquiryCustomer") = Nothing
        Session("HospitalityBookingEnquiryPackage") = Nothing
        Session("HospitalityBookingEnquiryProduct") = Nothing
        Session("HospitalityBookingEnquiryProductGroup") = Nothing
        Session("HospitalityBookingEnquiryMarkOrderFor") = Nothing
        Session("HospitalityBookingEnquiryQAndAStatus") = Nothing
        Session("HospitalityBookingEnquiryPrintStatus") = Nothing
        Session("HospitalityBookingEnquiryDataTableState") = Nothing
        Session("TemplateOverrideBusinessUnit") = Nothing
    End Sub

    ''' <summary>
    ''' Perform the customer logout
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LogoutCustomer()
        If (Not Profile.IsAnonymous) OrElse (Page.User.Identity.IsAuthenticated) Then
            FormsAuthentication.SignOut()
        End If
    End Sub

    ''' <summary>
    ''' Updates the agent details in tbl_agent
    ''' </summary>
    ''' <param name="agentDataEntity">The agent data entity.</param>
    Private Sub UpdateAgentDetails(ByVal agentDataEntity As DEAgent)
        Dim passwordEncrypted As String = String.Empty
        Try
            passwordEncrypted = TCUtilities.TripleDESEncode(_agentPassword, ModuleDefaults.NOISE_ENCRYPTION_KEY)
        Catch ex As Exception
            passwordEncrypted = String.Empty
        End Try
        TDataObjects.AgentSettings.TblAgent.InsertOrUpdate(HttpContext.Current.Session.SessionID, agentDataEntity.AgentUsername, passwordEncrypted, agentDataEntity)
    End Sub

    ''' <summary>
    ''' Logouts the agent on session expire.
    ''' </summary>
    Private Sub LogoutAgentOnSessionExpire()
        If Session("IsAgentProfileEmpty") IsNot Nothing AndAlso CType(Session("IsAgentProfileEmpty"), Boolean) Then
            Session.Remove("IsAgentProfileEmpty")
            LogoutCustomer()
            Profile.Basket.EmptyBasket()
            AgentProfile.Logout()
            Response.Redirect("~/PagesPublic/Agent/AgentLogin.aspx?logout=expired")
        End If
        Dim errMsg As TalentErrorMessages
        Dim talentErrorMsg As TalentErrorMessage
        errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, _wfrPage.PartnerCode, _wfrPage.FrontEndConnectionString)
        plhErrorMessage.Visible = True
        talentErrorMsg = errMsg.GetErrorMessage("Agent_Session_Expired")
        ltlErrorMessage.Text = talentErrorMsg.ERROR_MESSAGE
    End Sub

    ''' <summary>
    ''' Load Departments
    ''' </summary>
    Private Sub LoadDepartments()
        Dim err As New ErrorObj
        Dim deAgent As New DEAgent
        Dim settings As DESettings = TEBUtilities.GetSettingsObjectForAgent()
        Dim ta As New TalentAgent

        deAgent.Source = GlobalConstants.SOURCE
        ta.AgentDataEntity = deAgent
        ta.Settings = settings
        err = ta.RetrieveAgentDepartments()

        If Not err.HasError Then
            If ta.ResultDataSet IsNot Nothing And ta.ResultDataSet.Tables("Departments").Rows.Count = 0 Then
                plhDepartment.Visible = False
            Else
                ddlDepartment.Items.Clear()
                Dim dt As DataTable = ta.ResultDataSet.Tables("Departments")
                Dim locationListItem As New ListItem
                ddlDepartment.DataTextField = "DepartmentDescription"
                ddlDepartment.DataValueField = "DepartmentReference"
                ddlDepartment.DataSource = dt
                ddlDepartment.DataBind()
                If ddlDepartment.Items.Count > 1 Then
                    locationListItem.Value = "-1"
                    locationListItem.Text = _wfrPage.Content("PleaseSelectOptionText", _languageCode, True)
                    ddlDepartment.Items.Insert(0, locationListItem)
                End If
            End If
        End If
    End Sub

#End Region

End Class