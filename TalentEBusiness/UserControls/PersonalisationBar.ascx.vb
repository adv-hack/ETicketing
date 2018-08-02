Imports Talent.eCommerce

Partial Class UserControls_PersonalisationBar
    Inherits ControlBase

    Private _ucr As Talent.Common.UserControlResource = Nothing
    Private _languageCode As String = Nothing

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        _ucr = New Talent.Common.UserControlResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage()
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Utilities.GetCurrentPageName()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PersonalisationBar.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ltlLoginStatus As New Literal
        If Profile.IsAnonymous AndAlso Not Page.User.Identity.IsAuthenticated Then
            ltlLoginStatus = LoginView.FindControl("ltlAnonymous")
            If ltlLoginStatus IsNot Nothing Then ltlLoginStatus.Text = _ucr.Content("anonymousLabel", _languageCode, True)
            Dim ltlRegister As Literal = LoginView.FindControl("ltlRegister")
            If ltlRegister IsNot Nothing Then ltlRegister.Text = _ucr.Content("RegisterLinkText", _languageCode, True)
        Else
            ltlLoginStatus = LoginView.FindControl("ltlLoggedIn")
            If ltlLoginStatus IsNot Nothing Then
                ltlLoginStatus.Text = _ucr.Content("LoggedInLabel", _languageCode, True)
                ltlLoginStatus.Text = ltlLoginStatus.Text.Replace("<<FULLNAME>>", Profile.User.Details.Full_Name)
                ltlLoginStatus.Text = ltlLoginStatus.Text.Replace("<<FORENAME>>", Profile.User.Details.Forename)
                ltlLoginStatus.Text = ltlLoginStatus.Text.Replace("<<SURNAME>>", Profile.User.Details.Surname)
                ltlLoginStatus.Text = ltlLoginStatus.Text.Replace("<<SALUTATION>>", Profile.User.Details.Salutation)
                If ModuleDefaults IsNot Nothing AndAlso ModuleDefaults.LoginidType IsNot Nothing AndAlso ModuleDefaults.LoginidType.Equals("1") Then
                    If Profile.User.Details.LoginID IsNot Nothing Then ltlLoginStatus.Text = ltlLoginStatus.Text.Replace("<<USERNAME>>", Profile.User.Details.LoginID.TrimStart("0"))
                Else
                    If Profile.User.Details.LoginID IsNot Nothing Then ltlLoginStatus.Text = ltlLoginStatus.Text.Replace("<<USERNAME>>", Profile.User.Details.LoginID)
                End If
            End If
            Dim hplLogout As New HyperLink
            hplLogout = LoginView.FindControl("hplLogout")
            If hplLogout IsNot Nothing Then
                hplLogout.Text = _ucr.Content("LogoutText", _languageCode, True).Replace("<<FORENAME>>", Profile.User.Details.Forename)
                hplLogout.NavigateUrl = "~/PagesPublic/Login/LoggedOut.aspx"
            End If
        End If
    End Sub

End Class
