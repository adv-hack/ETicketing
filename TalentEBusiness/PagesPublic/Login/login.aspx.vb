Imports Microsoft.VisualBasic
Imports Talent.eCommerce

Partial Class PagesPublic_login
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfr As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If ModuleDefaults.WholeSiteIsInAgentMode Then
            Dim redirectUrl As New StringBuilder
            redirectUrl.Append("~/PagesPublic/Profile/CustomerSelection.aspx")
            If Request.QueryString("returnurl") IsNot Nothing Then
                redirectUrl.Append("?returnurl=")
                redirectUrl.Append(HttpContext.Current.Server.UrlEncode(Request.QueryString("returnurl")))
            End If
            Response.Redirect(redirectUrl.ToString())
        Else
            If ModuleDefaults.ShowRegistrationFormOnLoginPage Then
                Select Case ModuleDefaults.RegistrationTemplate
                    Case "1"
                        RegistrationForm1.Display = True
                        RegistrationForm2.Display = False
                        RegistrationForm3.Display = False
                    Case "2"
                        RegistrationForm1.Display = False
                        RegistrationForm2.Display = True
                        RegistrationForm3.Display = False
                    Case "3"
                        RegistrationForm1.Display = False
                        RegistrationForm2.Display = False
                        RegistrationForm3.Display = True
                    Case Else
                        RegistrationForm1.Display = True
                        RegistrationForm2.Display = False
                        RegistrationForm3.Display = False
                End Select
            Else
                RegistrationForm1.Display = False
                RegistrationForm2.Display = False
                RegistrationForm3.Display = False
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "Login.aspx"
            MessageLabel.Text = .Content("MessageText", _languageCode, True)
            MessageLabel.Visible = (MessageLabel.Text.Length > 0)
        End With

        If Not Page.IsPostBack Then
            If AgentProfile.IsAgent Then
                MessageLabel.Visible = False
                LoginStatus.Visible = False
                'decide whether to show login page or customer selection page
                If Request.QueryString("ReturnUrl") IsNot Nothing AndAlso Request.QueryString("ReturnUrl").ToUpper().Contains("/PRODUCTBROWSE/") Then
                    Response.Redirect("/PagesPublic/Profile/CustomerSelection.aspx?RequiresLogin=True")
                End If
            Else
                If Not ModuleDefaults.Show_Login_Control_On_Reminder_Page Then MainLoginBox.Visible = False
                If Not ModuleDefaults.ShowRegisterAccountOnLoginPage Then RegisterAccount1.Visible = False
                If Not ModuleDefaults.ShowActivateAccountOnLoginPage Then ActivateAccount1.Visible = False
                Try
                    If Profile.IsAnonymous Then
                        LoginStatus.Text = _wfr.Content("loggedOutText", _languageCode, True)
                    Else
                        LoginStatus.Text = _wfr.Content("loggedInText", _languageCode, True)
                        If Request.QueryString("ReturnUrl") IsNot Nothing Then Response.Redirect(Request.QueryString("ReturnUrl"))
                    End If
                Catch ex As Exception
                    LoginStatus.Text = ""
                End Try
            End If
        End If
    End Sub

#End Region

End Class