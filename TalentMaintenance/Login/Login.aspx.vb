
Partial Class Login_Login
    Inherits System.Web.UI.Page

    Private _wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = String.Empty
    Const MAINTENANCEBUSINESSUNIT As String = "MAINTENANCE"
    Const STARALLPARTNER As String = "*ALL"
    Const PAGECODE As String = "Login.aspx"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = MAINTENANCEBUSINESSUNIT
            .PartnerCode = STARALLPARTNER
            .PageCode = PAGECODE
            .KeyCode = PAGECODE
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage()
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        'Add autocomplte regardless of pre-poulate
        Dim ackey As String = "autocomplete"
        Dim acvalue As String = "off"
        Dim txtUsername As TextBox
        Dim txtPassword As TextBox
        Try
            txtUsername = Login1.FindControl("UserName")
            txtPassword = Login1.FindControl("Password")
            txtUsername.Attributes.Add(ackey, acvalue)
            txtPassword.Attributes.Add(ackey, acvalue)
        Catch ex As Exception
        End Try
        For Each ctrl As Control In Page.Controls
            If TypeOf ctrl Is HtmlForm Then
                CType(ctrl, HtmlForm).Attributes.Add(ackey, acvalue)
            End If
        Next
    End Sub
    
    Protected Sub Login1_LoggedIn(ByVal sender As Object, ByVal e As System.EventArgs) Handles Login1.LoggedIn
        Response.Write("You are now logged in!")
    End Sub

    Protected Sub Login1_LoggingIn(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.LoginCancelEventArgs) Handles Login1.LoggingIn
        If FormsAuthentication.Authenticate(Login1.UserName, Login1.Password) Then
            FormsAuthentication.RedirectFromLoginPage(Login1.UserName, False)
        Else
            plhLoginError.Visible = True
            ltlLoginError.Text = _wfrPage.Content("IncorrectLoginDetails", _languageCode, True)
        End If
    End Sub
End Class
