Imports System.Data
Imports System.Data.SqlClient

Partial Class UserControls_ActivateAccount
    Inherits ControlBase

    Protected _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Protected ucr As New Talent.Common.UserControlResource
    Protected mydefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        mydefaults = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults

        If mydefaults.ShowActivateAccountOnLoginPage Then
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "ActivateAccount.ascx"
            End With
        Else
            Me.Visible = False
            activateButton.Enabled = False
        End If

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If mydefaults.ShowActivateAccountOnLoginPage Then
            If Not Page.IsPostBack Then
                activateButton.Text = ucr.Content("ActivateButtonText", _languageCode, True)
                headerLabel.Text = ucr.Content("ActivateAccountTitleText", _languageCode, True)
                infoLabel.Text = ucr.Content("ActivateAccountInfoText", _languageCode, True)
                accountNoLabel.Text = ucr.Content("AccountNoLabel", _languageCode, True)
                surnameLabel.Text = ucr.Content("SurnameLabel", _languageCode, True)
                lblForename.Text = ucr.Content("ForenameLabel", _languageCode, True)
                postcodeLabel.Text = ucr.Content("PostcodeLabel", _languageCode, True)
            End If
        End If
    End Sub

    Protected Sub activateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles activateButton.Click
        errorList.Items.Clear()

        If String.IsNullOrEmpty(accountNo1.Text) Then
            errorList.Items.Add(ucr.Content("EnterAccountNoError", _languageCode, True))
        End If
        If String.IsNullOrEmpty(accountNo2.Text) Then
            errorList.Items.Add(ucr.Content("EnterAccountNo2Error", _languageCode, True))
        End If
        If CBool(ucr.Attribute("SurnameMandatory")) AndAlso String.IsNullOrEmpty(surname.Text) Then
            errorList.Items.Add(ucr.Content("EnterSurnameError", _languageCode, True))
        End If
        If CBool(ucr.Attribute("ForenameMandatory")) AndAlso String.IsNullOrEmpty(txtForename.Text) Then
            errorList.Items.Add(ucr.Content("EnterForenameError", _languageCode, True))
        End If

        Dim talentMembershipProvider As New TalentMembershipProvider
        If errorList.Items.Count = 0 Then
            Dim users As DataTable = GetUser(Not String.IsNullOrEmpty(surname.Text), CBool(ucr.Attribute("SurnameMandatory")), CBool(ucr.Attribute("ForenameMandatory")))
            Select Case users.Rows.Count
                Case Is = 1
                    Dim loginID As String = Talent.eCommerce.Utilities.CheckForDBNull_String(users.Rows(0)("LOGINID"))
                    Dim password As String = GetPasswordForUser(loginID)
                    ' If B2B mode then can skip TestForValidUser
                    If TestForValidUser(loginID) OrElse TalentCache.GetDefaultPartner = Talent.Common.Utilities.GetAllString Then
                        Dim validUser As Boolean = True
                        If mydefaults.RefreshCustomerDataOnLogin Then
                            Dim deCustomerFromActivateAccount As New Talent.Common.DECustomer
                            deCustomerFromActivateAccount.CompanySLNumber1 = accountNo1.Text
                            deCustomerFromActivateAccount.CompanySLNumber2 = accountNo2.Text
                            deCustomerFromActivateAccount.ContactForename = txtForename.Text
                            deCustomerFromActivateAccount.ContactSurname = surname.Text
                            deCustomerFromActivateAccount.PostCode = postcode.Text
                            deCustomerFromActivateAccount.RetreiveOnlyMode = True
                            talentMembershipProvider.DeCustomerFromActivateAccount = deCustomerFromActivateAccount
                            ' BF - not required
                            '    validUser = talentMembershipProvider.ValidateUser(loginID, password)
                        End If
                        If validUser Then
                            'FormsAuthentication.Authenticate(loginID, password)
                            Membership.ValidateUser(loginID, password)
                            FormsAuthentication.SetAuthCookie(users.Rows(0)("LOGINID"), False)
                            Session("FromActivation") = True
                            Response.Redirect("~/PagesLogin/Profile/UpdateProfile.aspx")
                        Else
                            errorList.Items.Add(ucr.Content("DetailsNotRecognisedError", _languageCode, True))
                        End If
                    Else
                        errorList.Items.Add(ucr.Content("DetailsNotRecognisedError", _languageCode, True))
                    End If
                Case Is > 1
                    ' If Forename is mandatory then just take first record and log in
                    If CBool(ucr.Attribute("ForenameMandatory")) Then
                        Dim loginID As String = Talent.eCommerce.Utilities.CheckForDBNull_String(users.Rows(0)("LOGINID"))
                        Dim password As String = GetPasswordForUser(loginID)
                        'FormsAuthentication.Authenticate(loginID, password)
                        Membership.ValidateUser(loginID, password)
                        FormsAuthentication.SetAuthCookie(users.Rows(0)("LOGINID"), False)
                        Session("FromActivation") = True
                        Response.Redirect("~/PagesLogin/Profile/UpdateProfile.aspx")
                    Else
                        errorList.Items.Add(ucr.Content("DuplicatedetailsError", _languageCode, True))
                    End If
                Case Else
                    ' If surname was filled then retry with blank surname
                    If Not String.IsNullOrEmpty(surname.Text) Then
                        users = GetUser(False, ucr.Attribute("SurnameMandatory"), False)
                        '-----------------------------------------------------
                        ' If set up to do so and matched except the surname is 
                        ' wrong then add surname as a new user
                        '-----------------------------------------------------
                        If CBool(ucr.Attribute("AddNewUserIfAccountMatched")) AndAlso users.Rows.Count >= 1 Then
                            AddNewUser(users, False)
                            Session("FromActivationNewContact") = True
                            Session("FromActivation") = True
                            Response.Redirect("~/PagesLogin/Profile/UpdateProfile.aspx")
                        Else
                            'Perform an account lookup to Talent CRM.
                            Dim usersFromCRM As DataSet = talentMembershipProvider.VerifyAndRetrieveAccountFromCRM(accountNo1.Text, accountNo2.Text, surname.Text, postcode.Text, txtForename.Text)
                            If usersFromCRM.Tables.Count > 1 Then
                                If usersFromCRM.Tables(0).Rows.Count > 0 Then
                                    If String.IsNullOrEmpty(usersFromCRM.Tables(0).Rows(0)(0).ToString) Then
                                        Dim userTable As DataTable = usersFromCRM.Tables(1)
                                        Select Case userTable.Rows.Count
                                            Case 0
                                                errorList.Items.Add(ucr.Content("DetailsNotRecognisedError", _languageCode, True))
                                            Case 1
                                                AddCRMUser(userTable)
                                                Session("FromActivationNewContact") = True
                                                Session("FromActivation") = True
                                                Response.Redirect("~/PagesLogin/Profile/UpdateProfile.aspx")
                                            Case Else
                                                errorList.Items.Add(ucr.Content("DuplicatedetailsError", _languageCode, True))
                                        End Select
                                    Else
                                        If usersFromCRM.Tables(1) IsNot Nothing _
                                            AndAlso usersFromCRM.Tables(1).Rows.Count > 0 _
                                            AndAlso Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(usersFromCRM.Tables(1).Rows(0)("FoundAddressOnly")) Then
                                            AddNewUser(usersFromCRM.Tables(1), True)
                                            Session("FromActivationNewContact") = True
                                            Session("FromActivation") = True
                                            Response.Redirect("~/PagesLogin/Profile/UpdateProfile.aspx")
                                        ElseIf usersFromCRM.Tables(0).Rows(0)(0).ToString.Equals(GlobalConstants.ERRORFLAG) Then
                                            errorList.Items.Add(ucr.Content("DetailsNotRecognisedError", _languageCode, True))
                                        End If
                                    End If
                                End If
                            Else
                                errorList.Items.Add(ucr.Content("DetailsNotRecognisedError", _languageCode, True))
                            End If
                        End If
                    Else
                        errorList.Items.Add(ucr.Content("DetailsNotRecognisedError", _languageCode, True))
                    End If
            End Select
        End If
    End Sub

    Protected Function GetPasswordForUser(ByVal loginID As String) As String
        Dim userSelect As String = " SELECT * FROM tbl_authorized_users WITH (NOLOCK)  " & _
                                   " WHERE BUSINESS_UNIT = @BusinessUnit " & _
                                   " AND PARTNER = @Partner " & _
                                   " AND LOGINID = @LoginID "

        Dim password As String = ""
        Dim cmd As New SqlCommand(userSelect, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))

        With cmd.Parameters
            .Clear()
            .Add("@BusinessUnit", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnitGroup
            .Add("@Partner", SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
            .Add("@LoginID", SqlDbType.NVarChar).Value = loginID
        End With

        Try
            cmd.Connection.Open()
        Catch ex As Exception
        End Try

        If cmd.Connection.State = ConnectionState.Open Then
            Dim dr As SqlDataReader = cmd.ExecuteReader
            If dr.HasRows Then
                dr.Read()
                password = Talent.eCommerce.Utilities.CheckForDBNull_String(dr("PASSWORD"))
            End If
        End If

        Try
            cmd.Connection.Close()
            cmd.Dispose()
        Catch ex As Exception
        End Try

        Return password
    End Function

    Protected Function TestForValidUser(ByVal loginID As String) As Boolean
        Dim userSelect As String = " SELECT * FROM tbl_partner_user WITH (NOLOCK)  " & _
                                   " WHERE PARTNER = @Partner " & _
                                   " AND LOGINID = @LoginID "

        Dim cmd As New SqlCommand(userSelect, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)), _
            users As New DataTable

        With cmd.Parameters
            .Clear()
            .Add("@BusinessUnit", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnitGroup
            .Add("@Partner", SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
            .Add("@LoginID", SqlDbType.NVarChar).Value = loginID
        End With

        Try
            cmd.Connection.Open()
        Catch ex As Exception
        End Try

        If cmd.Connection.State = ConnectionState.Open Then
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(users)
        End If

        Try
            cmd.Connection.Close()
            cmd.Dispose()
        Catch ex As Exception
        End Try

        If users.Rows.Count > 0 Then
            Return True
        Else : Return False
        End If
    End Function

    Protected Function GetUser(ByVal useSurname As Boolean, ByVal surnameMandatory As Boolean, ByVal forenameMandatory As Boolean) As DataTable
        Dim userSelect As String = String.Empty
        Dim defaultPartner As String = TalentCache.GetDefaultPartner

        ' Only check for partner if in B2C mode..
        If defaultPartner <> Talent.Common.Utilities.GetAllString Then
            userSelect = " SELECT tbl_partner_user.*, tbl_address.* " & _
                                         " FROM tbl_partner_user WITH (NOLOCK)  INNER JOIN tbl_address WITH (NOLOCK)  " & _
                                         " ON (tbl_partner_user.LOGINID = tbl_address.LOGINID " & _
                                         "    AND tbl_partner_user.PARTNER = tbl_address.PARTNER) " & _
                                         " WHERE tbl_partner_user.PARTNER = @partner " & _
                                         " AND tbl_partner_user.ACCOUNT_NO_1 = @accountNo1 " & _
                                         " AND tbl_partner_user.ACCOUNT_NO_2 = @accountNo2 " & _
                                         " AND tbl_address.DEFAULT_ADDRESS = 'True' " & _
                                         " AND (tbl_address.POST_CODE = @Postcode " & _
                                         " OR tbl_address.POST_CODE = @PostcodeNoSpaces) "
        Else
            userSelect = " SELECT tbl_partner_user.*, tbl_address.* " & _
                                        " FROM tbl_partner_user WITH (NOLOCK)  INNER JOIN tbl_address WITH (NOLOCK)  " & _
                                        " ON (tbl_partner_user.LOGINID = tbl_address.LOGINID " & _
                                        "    AND tbl_partner_user.PARTNER = tbl_address.PARTNER) " & _
                                        " WHERE tbl_partner_user.ACCOUNT_NO_1 = @accountNo1 " & _
                                        " AND tbl_partner_user.ACCOUNT_NO_2 = @accountNo2 " & _
                                        " AND tbl_address.DEFAULT_ADDRESS = 'True' " & _
                                        " AND (tbl_address.POST_CODE = @Postcode " & _
                                        " OR tbl_address.POST_CODE = @PostcodeNoSpaces) "

        End If

        If useSurname Then userSelect += " AND tbl_partner_user.SURNAME = @surname "
        If forenameMandatory Then userSelect += " AND tbl_partner_user.FORENAME = @forename "

        Dim cmd As New SqlCommand(userSelect, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)), _
            users As New DataTable

        With cmd.Parameters
            .Clear()
            .Add("@accountNo1", SqlDbType.NVarChar).Value = accountNo1.Text.Trim
            .Add("@accountNo2", SqlDbType.NVarChar).Value = accountNo2.Text.Trim
            .Add("@partner", SqlDbType.NVarChar).Value = TalentCache.GetDefaultPartner
            .Add("@Postcode", SqlDbType.NVarChar).Value = postcode.Text.Trim
            .Add("@PostcodeNoSpaces", SqlDbType.NVarChar).Value = postcode.Text.Replace(" ", "")
            If useSurname Then .Add("@surname", SqlDbType.NVarChar).Value = surname.Text.Trim
            If forenameMandatory Then .Add("@forename", SqlDbType.NVarChar).Value = txtForename.Text.Trim
        End With

        Try
            cmd.Connection.Open()
        Catch ex As Exception
        End Try

        If cmd.Connection.State = ConnectionState.Open Then
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(users)
        End If
        Try
            cmd.Connection.Close()
            cmd.Dispose()
        Catch ex As Exception
        End Try
        '---------------------------------------
        ' Additional check for Account existing.
        ' Check existing account's postcode
        '---------------------------------------
        If users.Rows.Count = 0 Then
            Dim accountSelect As String = " SELECT tbl_partner_user.*, tbl_address.* " & _
                                                 " FROM tbl_partner_user WITH (NOLOCK)  INNER JOIN tbl_address WITH (NOLOCK)  " & _
                                                 " ON (tbl_partner_user.LOGINID = tbl_address.LOGINID " & _
                                                 "    AND tbl_partner_user.PARTNER = tbl_address.PARTNER) " & _
                                                 " WHERE tbl_partner_user.ACCOUNT_NO_1 = @accountNo1 " & _
                                                 " AND tbl_partner_user.ACCOUNT_NO_2 = @accountNo2 " & _
                                                 " AND tbl_address.DEFAULT_ADDRESS = 'True' "

            If useSurname Then accountSelect += " AND tbl_partner_user.SURNAME = @surname "
            cmd = New SqlCommand(accountSelect, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            With cmd.Parameters
                .Clear()
                .Add("@accountNo1", SqlDbType.NVarChar).Value = accountNo1.Text.Trim
                .Add("@accountNo2", SqlDbType.NVarChar).Value = accountNo2.Text.Trim
                If useSurname Then .Add("@surname", SqlDbType.NVarChar).Value = surname.Text.Trim
                If forenameMandatory Then .Add("@forename", SqlDbType.NVarChar).Value = txtForename.Text.Trim
            End With
            Try
                cmd.Connection.Open()
            Catch ex As Exception
            End Try
            If cmd.Connection.State = ConnectionState.Open Then
                Dim da As New SqlDataAdapter(cmd)
                da.Fill(users)
            End If

            If (useSurname AndAlso users.Rows.Count = 1) OrElse _
                (Not useSurname AndAlso surnameMandatory AndAlso users.Rows.Count > 0) Then
                '-------------------------------------------------
                ' Row found for account - Check stripped postcodes
                '-------------------------------------------------
                Dim foundPostcode As String = Talent.eCommerce.Utilities.CheckForDBNull_String(users.Rows(0)("POST_CODE").ToString)
                If foundPostcode.Replace(" ", String.Empty) = postcode.Text.Replace(" ", String.Empty) Then
                    ' OK!
                Else
                    users.Rows.Clear()
                End If
            Else
                users.Rows.Clear()
            End If
            Try
                cmd.Connection.Close()
                cmd.Dispose()
            Catch ex As Exception
            End Try
        End If

        Return users
    End Function
    Protected Function GetUserNoAddress(ByVal useSurname As Boolean, ByVal surnameMandatory As Boolean, ByVal forenameMandatory As Boolean) As DataTable
        Dim userSelect As String = String.Empty

        ' Only check for partner if in B2C mode..

        userSelect = " SELECT tbl_partner_user.*, tbl_address.* " & _
                             " FROM tbl_partner_user WITH (NOLOCK)  LEFT OUTER JOIN tbl_address WITH (NOLOCK)  " & _
                             " ON (tbl_partner_user.LOGINID = tbl_address.LOGINID " & _
                             "    AND tbl_partner_user.PARTNER = tbl_address.PARTNER) " & _
                             " WHERE tbl_partner_user.ACCOUNT_NO_1 = @accountNo1 " & _
                             " AND tbl_partner_user.ACCOUNT_NO_2 = @accountNo2 "

        If useSurname Then userSelect += " AND tbl_partner_user.SURNAME = @surname "
        If forenameMandatory Then userSelect += " AND tbl_partner_user.FORENAME = @forename "

        Dim cmd As New SqlCommand(userSelect, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)), _
            users As New DataTable

        With cmd.Parameters
            .Clear()
            .Add("@accountNo1", SqlDbType.NVarChar).Value = accountNo1.Text.Trim
            .Add("@accountNo2", SqlDbType.NVarChar).Value = accountNo2.Text.Trim

            If useSurname Then .Add("@surname", SqlDbType.NVarChar).Value = surname.Text.Trim
            If forenameMandatory Then .Add("@forename", SqlDbType.NVarChar).Value = txtForename.Text.Trim
        End With

        Try
            cmd.Connection.Open()
        Catch ex As Exception
        End Try

        If cmd.Connection.State = ConnectionState.Open Then
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(users)
        End If
        Try
            cmd.Connection.Close()
            cmd.Dispose()
        Catch ex As Exception
        End Try
        '---------------------------------------

        Return users
    End Function
    Protected Sub AddNewUser(ByVal users As Data.DataTable, ByVal foundAddressOnly As Boolean)
        Dim userNumber As String = Talent.Common.Utilities.GetNextUserNumber(TalentCache.GetBusinessUnit, _
                                                    TalentCache.GetPartner(Profile), _
                                                    ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)

        CType(Membership.Provider, TalentMembershipProvider).CreateUser(userNumber, String.Empty, String.Empty, "", "", True, New Object, New System.Web.Security.MembershipCreateStatus, accountNo1.Text.ToUpper.Trim)

        'Create and populate the user object
        Dim userDetails As New TalentProfileUserDetails
        With userDetails

            .LoginID = userNumber
            .Email = String.Empty
            .Title = String.Empty
            .Forename = txtForename.Text
            .DOB = CDate("01/02/1900")
            .Surname = surname.Text
            .Full_Name = String.Empty
            .Position = String.Empty
            .Telephone_Number = String.Empty
            .Fax_Number = String.Empty
            .Account_No_1 = accountNo1.Text.ToUpper.Trim
            .Account_No_2 = accountNo2.Text.ToUpper.Trim

            .User_Number = userNumber
            .User_Number_Prefix = String.Empty
            .Subscribe_Newsletter = False
            .HTML_Newsletter = False

            .Bit1 = False
            .Bit2 = False
            .Bit3 = False
            .Bit4 = False
            .Bit5 = False
        End With

        'Create and populate the userAddress
        Dim userAddress As New TalentProfileAddress
        With userAddress
            .LoginID = userNumber
            If foundAddressOnly Then
                .Reference = users.Rows(0)("AddressLine1")
                .Type = ""
                .Default_Address = True
                .Address_Line_1 = users.Rows(0)("CompanyName")
                .Address_Line_2 = users.Rows(0)("AddressLine1")
                .Address_Line_3 = users.Rows(0)("AddressLine2")
                .Address_Line_4 = users.Rows(0)("AddressLine3")
                .Address_Line_5 = users.Rows(0)("AddressLine4")
                .Post_Code = users.Rows(0)("PostCode")
                .Country = users.Rows(0)("AddressLine5")
            Else
                .Reference = users.Rows(0)("REFERENCE")
                .Type = users.Rows(0)("TYPE")
                .Default_Address = True
                .Address_Line_1 = users.Rows(0)("ADDRESS_LINE_1")
                .Address_Line_2 = users.Rows(0)("ADDRESS_LINE_2")
                .Address_Line_3 = users.Rows(0)("ADDRESS_LINE_3")
                .Address_Line_4 = users.Rows(0)("ADDRESS_LINE_4")
                .Address_Line_5 = users.Rows(0)("ADDRESS_LINE_5")
                .Post_Code = users.Rows(0)("POST_CODE")
                .Country = users.Rows(0)("COUNTRY")
            End If

            .Sequence = 0
        End With

        'Create and populate the partnerDetails
        Dim partnerDetails As New TalentProfilePartnerDetails
        With partnerDetails
            .Account_No_1 = userDetails.Account_No_1
            .Account_No_2 = userDetails.Account_No_2
            .Account_No_3 = String.Empty
            .Account_No_4 = String.Empty
            .Account_No_5 = String.Empty

            .Partner = userDetails.Account_No_1

            .Telephone_Number = String.Empty
            .Fax_Number = String.Empty
            .Partner_Number = Talent.Common.Utilities.GetNextPartnerNumber(TalentCache.GetBusinessUnit, _
                userDetails.Account_No_1, _
                ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
            .VAT_NUMBER = String.Empty
        End With


        'Create the profile
        Profile.Provider.CreateProfileWithPartner(userDetails, userAddress, partnerDetails)

        'FormsAuthentication.Authenticate(userNumber, String.Empty)
        Membership.ValidateUser(userNumber, String.Empty)
        Profile.Initialize(String.Empty, True)
        FormsAuthentication.SetAuthCookie(userNumber, False)

    End Sub

    Protected Sub AddCRMUser(ByVal users As Data.DataTable)

        ' Check for just address wrong - if so we just need to write/update an address record.
        Dim usersNoAddress As DataTable = GetUserNoAddress(Not String.IsNullOrEmpty(surname.Text), CBool(ucr.Attribute("SurnameMandatory")), CBool(ucr.Attribute("ForenameMandatory")))
        If usersNoAddress.Rows.Count > 0 Then
            Dim ProfileAddress As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter
            Dim loginID As String = usersNoAddress.Rows(0)("LOGINID").ToString
            Dim partner As String = usersNoAddress.Rows(0)("PARTNER").ToString
            'Create and populate the userAddress
            Dim userProfAddress As New TalentProfileAddress
            With userProfAddress
                .LoginID = loginID
                .Reference = users.Rows(0)("AddressLine1").ToString
                .Type = String.Empty
                .Default_Address = True
                .Address_Line_1 = users.Rows(0)("CompanyName").ToString
                .Address_Line_2 = users.Rows(0)("AddressLine1").ToString
                .Address_Line_3 = users.Rows(0)("AddressLine2").ToString
                .Address_Line_4 = users.Rows(0)("AddressLine3").ToString
                .Address_Line_5 = users.Rows(0)("AddressLine4").ToString
                .Post_Code = users.Rows(0)("PostCode").ToString
                .Country = users.Rows(0)("AddressLine5").ToString
                .Sequence = 0
            End With

            If IsDBNull(usersNoAddress.Rows(0)("ADDRESS_ID")) Then
                ' no address record - Insert the address
                ProfileAddress.AddAddress(partner, _
                                     loginID, _
                                        String.Empty, _
                                        users.Rows(0)("AddressLine1").ToString,
                                        "0", _
                                        True, _
                                        users.Rows(0)("CompanyName").ToString, _
                                        users.Rows(0)("AddressLine1").ToString, _
                                        users.Rows(0)("AddressLine2").ToString, _
                                        users.Rows(0)("AddressLine3").ToString, _
                                        users.Rows(0)("AddressLine4").ToString, _
                                        users.Rows(0)("PostCode").ToString, _
                                        users.Rows(0)("AddressLine5").ToString, _
                                        String.Empty, _
                                        String.Empty)

            Else
                ' address record wrong - Update address table
                ProfileAddress.UpdateAddressByLoginidTypeSequence(userProfAddress.Address_Line_1, _
                    userProfAddress.Address_Line_2, userProfAddress.Address_Line_3, userProfAddress.Address_Line_4, _
                    userProfAddress.Address_Line_5, userProfAddress.Post_Code, userProfAddress.Country, userProfAddress.LoginID, _
                    partner, userProfAddress.Type, userProfAddress.Sequence)
            End If

            ' Login
            Dim password As String = GetPasswordForUser(loginID)

            'FormsAuthentication.Authenticate(loginID, password)
            Membership.ValidateUser(loginID, password)
            FormsAuthentication.SetAuthCookie(loginID, False)
            Session("FromActivation") = True
            Response.Redirect("~/PagesLogin/Profile/UpdateProfile.aspx")

        Else

            Dim userNumber As String = String.Empty
            userNumber = Talent.Common.Utilities.GetNextUserNumber(ucr.BusinessUnit, TalentCache.GetPartner(Profile), ucr.FrontEndConnectionString)
            CType(Membership.Provider, TalentMembershipProvider).CreateUser(userNumber, String.Empty, _
                users.Rows(0)("EmailAddress").ToString, String.Empty, String.Empty, True, New Object, _
                New System.Web.Security.MembershipCreateStatus, accountNo1.Text.ToUpper.Trim)

            'Create and populate the user object
            Dim userDetails As New TalentProfileUserDetails
            With userDetails
                .LoginID = userNumber
                .Email = users.Rows(0)("EmailAddress").ToString
                .Title = users.Rows(0)("ContactTitle").ToString
                .Forename = txtForename.Text
                If String.IsNullOrEmpty(users.Rows(0)("DateBirth").ToString) Then
                    .DOB = CDate("01/02/1900")
                Else
                    .DOB = Talent.eCommerce.Utilities.CheckForDBNull_Date(users.Rows(0)("DateBirth").ToString)
                End If
                .Surname = surname.Text
                .Full_Name = txtForename.Text & " " & surname.Text
                .Position = users.Rows(0)("PositionInCompany").ToString
                .Telephone_Number = users.Rows(0)("HomeTelephoneNumber").ToString
                .Fax_Number = users.Rows(0)("FaxNumber").ToString
                .Account_No_1 = accountNo1.Text.ToUpper.Trim
                .Account_No_2 = accountNo2.Text.ToUpper.Trim
                .User_Number = userNumber
                .User_Number_Prefix = String.Empty
                .Subscribe_Newsletter = False
                .HTML_Newsletter = False
                .Bit1 = False
                .Bit2 = False
                .Bit3 = False
                .Bit4 = False
                .Bit5 = False
                .CompanyName = users.Rows(0)("CompanyName").ToString
                .MAIL_TEAM_CODE_1 = users.Rows(0)("ContactViaMail1").ToString
                .MAIL_TEAM_CODE_2 = users.Rows(0)("ContactViaMail2").ToString
                .MAIL_TEAM_CODE_3 = users.Rows(0)("ContactViaMail3").ToString
                .MAIL_TEAM_CODE_4 = users.Rows(0)("ContactViaMail4").ToString
                .MAIL_TEAM_CODE_5 = users.Rows(0)("ContactViaMail5").ToString
                .Salutation = users.Rows(0)("Salutation").ToString
                .Work_Number = users.Rows(0)("WorkTelephoneNumber").ToString
                .Initials = users.Rows(0)("ContactInitials").ToString
                .Other_Number = users.Rows(0)("OtherNumber").ToString
            End With

            'Create and populate the userAddress
            Dim userAddress As New TalentProfileAddress
            With userAddress
                .LoginID = userNumber
                .Reference = users.Rows(0)("AddressLine1").ToString
                .Type = String.Empty
                .Default_Address = True
                .Address_Line_1 = users.Rows(0)("CompanyName").ToString
                .Address_Line_2 = users.Rows(0)("AddressLine1").ToString
                .Address_Line_3 = users.Rows(0)("AddressLine2").ToString
                .Address_Line_4 = users.Rows(0)("AddressLine3").ToString
                .Address_Line_5 = users.Rows(0)("AddressLine4").ToString
                .Post_Code = users.Rows(0)("PostCode").ToString
                .Country = users.Rows(0)("AddressLine5").ToString
                .Sequence = 0
            End With

            'Create and populate the partnerDetails
            Dim partnerDetails As New TalentProfilePartnerDetails
            With partnerDetails
                .Account_No_1 = userDetails.Account_No_1
                .Account_No_2 = userDetails.Account_No_2
                .Account_No_3 = String.Empty
                .Account_No_4 = String.Empty
                .Account_No_5 = String.Empty
                .Partner = userDetails.Account_No_1
                .Telephone_Number = String.Empty
                .Fax_Number = String.Empty
                .Partner_Number = userNumber
                .VAT_NUMBER = String.Empty
                .CRM_Branch = users.Rows(0)("CRMBranch").ToString
                .Partner_Desc = userDetails.Account_No_1
                .Destination_Database = String.Empty
                .Caching_Enabled = False
                .Cache_Time_Minutes = 0
                .Logging_Enabled = False
                .Store_XML = False
                .Email = String.Empty
                .Partner_URL = String.Empty
                .Order_Enquiry_Show_Partner_Orders = False
                .Enable_Alternate_SKU = False
                .Minimum_Purchase_Amount = 0
                .Minimum_Purchase_Quantity = 0
                .Use_Minimum_Purchase_Amount = False
                .Use_Minimum_Purchase_Quantity = False
                .COST_CENTRE = String.Empty
            End With

            'Create the profile
            Profile.Provider.CreateProfileWithPartner(userDetails, userAddress, partnerDetails)

            'FormsAuthentication.Authenticate(userNumber, String.Empty)
            Membership.ValidateUser(userNumber, String.Empty)
            Profile.Initialize(userNumber, True)
            FormsAuthentication.SetAuthCookie(userNumber, False)
        End If

    End Sub
End Class
