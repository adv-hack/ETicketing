Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce

Partial Class UserControls_AccountSelector
    Inherits ControlBase

    Protected _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Protected ucr As New Talent.Common.UserControlResource

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "AccountSelector.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim email As String = ""

            If Profile.IsAnonymous Then
                email = Request.QueryString("email")
            Else
                email = Profile.User.Details.Email
            End If

            If String.IsNullOrEmpty(email) Then
                Response.Redirect("~/PagesPublic/Login/Login.aspx")
            Else
                BindAccountsList()
            End If
        End If
    End Sub

    Protected Sub BindAccountsList()
        Dim userValid As Boolean = False

        Const loginSelect As String = " SELECT * " & _
                                       " FROM tbl_authorized_users WITH (NOLOCK)  " & _
                                       " WHERE BUSINESS_UNIT = @businessunit " & _
                                       " AND LOGINID = @loginid " & _
                                       " AND PASSWORD = @password "

        Const userSelect As String = " SELECT P.account_no_1,PU.account_no_2,PU.email,PU.loginid,PU.PARTNER" & _
                                   " FROM tbl_partner_user as PU WITH (NOLOCK)  " & _
                                   " INNER join tbl_partner as P WITH (NOLOCK)  on PU.partner = P.partner " & _
                                   " WHERE PU.EMAIL = @email "

        Dim cmd As New SqlCommand(userSelect, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)), _
            users As New DataTable, _
            email As String = ""

        If Profile.IsAnonymous Then
            email = Request.QueryString("email")
        Else
            email = Profile.User.Details.Email
        End If
        With cmd.Parameters
            .Clear()
            .Add("@email", SqlDbType.NVarChar).Value = email
        End With

        Try
            cmd.Connection.Open()
        Catch ex As Exception
        End Try

        If cmd.Connection.State = ConnectionState.Open Then
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(users)


            'If the users table contains records with the given email address
            'then we must check each record to see if the password supplied is 
            'valid for any of the users
            If users.Rows.Count > 0 Then
                If Profile.IsAnonymous Then
                    Dim decryptionKey1 As String = ConfigurationManager.AppSettings("PrivateEncryptionKey") & Now.ToString("ddMMyyHH"), _
                        decryptionKey2 As String = ConfigurationManager.AppSettings("PrivateEncryptionKey") & (Now.AddHours(-1)).ToString("ddMMyyHH"), _
                        password As String = Talent.Common.Utilities.TripleDESDecode(Request.QueryString("password").Replace(" ", "+"), decryptionKey1)

                    If String.IsNullOrEmpty(password) Then
                        password = Talent.Common.Utilities.TripleDESDecode(Request.QueryString("password").Replace(" ", "+"), decryptionKey2)
                    End If
                    If Not String.IsNullOrEmpty(password) Then
                        cmd.CommandText = loginSelect
                        With cmd.Parameters
                            .Clear()
                            .Add("@businessunit", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnitGroup
                            .Add("@loginid", SqlDbType.NVarChar).Value = ""
                            .Add("@password", SqlDbType.NVarChar).Value = password
                        End With
                        Dim dr As SqlDataReader
                        For Each rw As DataRow In users.Rows
                            cmd.Parameters("@loginid").Value = Utilities.CheckForDBNull_String(rw("LOGINID"))
                            dr = cmd.ExecuteReader

                            If dr.HasRows Then
                                dr.Close()
                                userValid = True
                                Exit For
                            Else
                                dr.Close()
                            End If
                        Next
                    End If

                Else
                    userValid = True
                End If

            End If

        End If

        Try
            cmd.Connection.Close()
            cmd.Dispose()
        Catch ex As Exception
        End Try

        If userValid Then
            AccountsRepeater.DataSource = users
            AccountsRepeater.DataBind()
        Else
            'if we redirect to the Return Url even if the user is not logged in
            'then they will be redirected back to the login screen if necessary
            If Not String.IsNullOrEmpty(Request.QueryString("ReturnUrl")) Then
                Response.Redirect(Request.QueryString("ReturnUrl"))
            Else
                Response.Redirect("~/PagesPublic/Login/Login.aspx")
            End If
        End If
    End Sub

    Protected Sub AccountsRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles AccountsRepeater.ItemDataBound
        If Not e.Item.ItemIndex = -1 Then
            Dim accountNo As Label = CType(e.Item.FindControl("AccountNumber"), Label), _
                loginid As Label = CType(e.Item.FindControl("LoginID"), Label), _
                email As Label = CType(e.Item.FindControl("Email"), Label), _
                company As Label = CType(e.Item.FindControl("Company"), Label), _
                address As Label = CType(e.Item.FindControl("Address"), Label), _
                postcode As Label = CType(e.Item.FindControl("Postcode"), Label), _
                myRow As DataRow = CType(e.Item.DataItem, DataRowView).Row


            accountNo.Text = Utilities.CheckForDBNull_String(myRow("ACCOUNT_NO_1"))
            If Utilities.CheckForDBNull_String(myRow("ACCOUNT_NO_2")) <> String.Empty Then
                accountNo.Text &= " / " & Utilities.CheckForDBNull_String(myRow("ACCOUNT_NO_2"))
            End If
            loginid.Text = Utilities.CheckForDBNull_String(myRow("LOGINID"))
            email.Text = Utilities.CheckForDBNull_String(myRow("EMAIL"))


            Dim addresses As DataTable = GetAccountAddress(loginid.Text, Utilities.CheckForDBNull_String(myRow("PARTNER")))

            If addresses.Rows.Count > 0 Then
                company.Text = Utilities.CheckForDBNull_String(addresses.Rows(0)("REFERENCE"))
                address.Text = Utilities.CheckForDBNull_String(addresses.Rows(0)("ADDRESS_LINE_1"))
                postcode.Text = Utilities.CheckForDBNull_String(addresses.Rows(0)("POST_CODE"))
            End If
            '-----------------------------------------------------------------
            ' For empty cell insert non breaking space to make style consistent
            '-----------------------------------------------------------------
            If accountNo.Text = String.Empty Then
                accountNo.Text = "&nbsp;"
            End If
            If loginid.Text = String.Empty Then
                loginid.Text = "&nbsp;"
            End If
            If email.Text = String.Empty Then
                email.Text = "&nbsp;"
            End If
            If company.Text = String.Empty Then
                company.Text = "&nbsp;"
            End If
            If address.Text = String.Empty Then
                address.Text = "&nbsp;"
            End If
            If postcode.Text = String.Empty Then
                postcode.Text = "&nbsp;"
            End If

            If Not Page.IsPostBack Then
                Dim loginButton As Button = CType(e.Item.FindControl("LoginButton"), Button)
                If Profile.IsAnonymous Then
                    loginButton.Text = ucr.Content("LoginButtonText", _languageCode, True)
                Else
                    loginButton.Text = ucr.Content("SwitchUserButton", _languageCode, True)
                    If loginid.Text = Profile.UserName Then
                        loginButton.Enabled = False
                    End If
                End If
            End If
        End If
    End Sub


    Protected Function GetAccountAddress(ByVal loginID As String, ByVal partner As String) As DataTable
        Const addressSelect As String = " SELECT * " & _
                                           " FROM tbl_address WITH (NOLOCK)  " & _
                                           " WHERE PARTNER = @partner " & _
                                           " AND LOGINID = @loginid " & _
                                           " AND DEFAULT_ADDRESS = 'True' "

        Dim cmd As New SqlCommand(addressSelect, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)), _
            addresses As New DataTable

        With cmd.Parameters
            .Clear()
            .Add("@loginid", SqlDbType.NVarChar).Value = loginID
            .Add("@partner", SqlDbType.NVarChar).Value = partner
        End With

        Try
            cmd.Connection.Open()
        Catch ex As Exception
        End Try

        If cmd.Connection.State = ConnectionState.Open Then
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(addresses)
        End If

        Try
            cmd.Connection.Close()
            cmd.Dispose()
        Catch ex As Exception
        End Try

        Return addresses
    End Function

    Protected Sub LoginAccount(ByVal sender As Object, ByVal e As EventArgs)
        Dim loginBtn As Button = CType(sender, Button), _
            ri As RepeaterItem = CType(loginBtn.Parent, RepeaterItem), _
            accountNo As Label = CType(ri.FindControl("AccountNumber"), Label), _
            loginid As Label = CType(ri.FindControl("LoginID"), Label), _
            decryptionKey1 As String = ConfigurationManager.AppSettings("PrivateEncryptionKey") & Now.ToString("ddMMyyHH"), _
            decryptionKey2 As String = ConfigurationManager.AppSettings("PrivateEncryptionKey") & (Now.AddHours(-1)).ToString("ddMMyyHH"), _
            password As String = ""

        'decode the password
        If Profile.IsAnonymous Then
            password = Talent.Common.Utilities.TripleDESDecode(Request.QueryString("password").Replace(" ", "+"), decryptionKey1)
            If String.IsNullOrEmpty(password) Then
                password = Talent.Common.Utilities.TripleDESDecode(Request.QueryString("password").Replace(" ", "+"), decryptionKey2)
            End If
        Else
            password = CType(Membership.GetUser(loginid.Text), TalentMembershipUser).Password
        End If

        'Try to log in with the supplied password
        If Not Utilities.loginUser(loginid.Text, password) Then
            'If we could not log in with the supplied password,
            'get the password relating to the user selected
            password = CType(Membership.GetUser(loginid.Text), TalentMembershipUser).Password
        End If

        Dim redirectUrl As String = ""
        If Not String.IsNullOrEmpty(password) Then
            'attempt to log the user in
            If Utilities.loginUser(loginid.Text, password) Then
                redirectUrl = "~/PagesLogin/Profile/MyAccount.aspx"
            Else
                redirectUrl = "~/PagesPublic/Login/Login.aspx"
            End If
        Else
            redirectUrl = "~/PagesPublic/Login/Login.aspx"
        End If

        'if we redirect to the Return Url even if the user is not logged in
        'then they will be redirected back to the login screen if necessary
        If Not String.IsNullOrEmpty(Request.QueryString("ReturnUrl")) Then
            Response.Redirect(Request.QueryString("ReturnUrl"))
        Else
            Response.Redirect(redirectUrl)
        End If

    End Sub
End Class
