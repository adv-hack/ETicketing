Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Talent  
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      ACTALMPR- 
'                                    
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Public Class TalentMembershipProvider
    Inherits System.Web.Security.MembershipProvider

    Private _ApplicationName As String
    Private _PWRegEx As String
    Private ucr As New Talent.Common.UserControlResource
    Private _deCustomerFromActivateAccount As Talent.Common.DECustomer = New Talent.Common.DECustomer
    Private _agentProfile As New Agent


    Public Overrides Property ApplicationName() As String
        Get
            Return _ApplicationName
        End Get
        Set(ByVal value As String)
            _ApplicationName = value
        End Set
    End Property
    Public ReadOnly Property BusinessUnit() As String
        Get
            Return TalentCache.GetBusinessUnitGroup
        End Get
    End Property
    Public ReadOnly Property BusinessUnitGroup() As String
        Get
            '-------------------------------------
            ' Returns the BU if no BU group is set
            '-------------------------------------
            Return TalentCache.GetBusinessUnitGroup
        End Get
    End Property
    Public ReadOnly Property Partner() As String
        Get
            Return TalentCache.GetPartner(HttpContext.Current.Profile)
        End Get
    End Property
    Public Property PasswordRegularExpression() As String
        Get
            Return _PWRegEx
        End Get
        Set(ByVal value As String)
            _PWRegEx = value
        End Set
    End Property
    Public Property DeCustomerFromActivateAccount() As Talent.Common.DECustomer
        Get
            Return _deCustomerFromActivateAccount
        End Get
        Set(ByVal value As Talent.Common.DECustomer)
            _deCustomerFromActivateAccount = value
        End Set
    End Property

    Public Overrides Sub Initialize(ByVal name As String, ByVal config As System.Collections.Specialized.NameValueCollection)
        MyBase.Initialize(name, config)
    End Sub
    Public Overrides Function ChangePassword(ByVal username As String, ByVal oldPassword As String, ByVal newPassword As String) As Boolean

        Dim users As New TalentMembershipDatasetTableAdapters.tbl_authorized_usersTableAdapter 
        Dim authPartners As New TalentMembershipDatasetTableAdapters.tbl_authorized_partnersTableAdapter
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        Dim oldPasswordHash As String
        Dim newPasswordHash As String
        If authPartners.Get_CheckFor_B2C_Login(BusinessUnit).Rows.Count > 0 Then
            If def.UseEncryptedPassword Then
                Dim passHash As New Talent.Common.PasswordHash
                oldPasswordHash = passHash.HashSalt(oldPassword, def.SaltString)
                newPasswordHash = passHash.HashSalt(newPassword, def.SaltString)
                Return CBool(users.ChangePassword_B2C(newPasswordHash, Microsoft.VisualBasic.DateAndTime.Now, BusinessUnitGroup, Partner, username, oldPasswordHash))
            Else
                Return CBool(users.ChangePassword_B2C(newPassword, Microsoft.VisualBasic.DateAndTime.Now, BusinessUnitGroup, Partner, username, oldPassword))
            End If
        Else
            Return CBool(users.ChangePassword_B2B(newPassword, Microsoft.VisualBasic.DateAndTime.Now, BusinessUnit, username, oldPassword))
        End If
    End Function

    Public Function ChangePasswordFromBackend(ByVal username As String, ByVal oldPassword As String, ByVal newPassword As String) As Boolean

        Dim users As New TalentMembershipDatasetTableAdapters.tbl_authorized_usersTableAdapter 
        Dim authPartners As New TalentMembershipDatasetTableAdapters.tbl_authorized_partnersTableAdapter 

        Dim changedDate As Date = Now
        Try
            changedDate = Me.GetUser(username, True).LastPasswordChangedDate
        Catch ex As Exception
        End Try

        If authPartners.Get_CheckFor_B2C_Login(BusinessUnit).Rows.Count > 0 Then
            Return CBool(users.ChangePassword_B2C(newPassword, changedDate, BusinessUnitGroup, Partner, username, oldPassword))
        Else
            Return CBool(users.ChangePassword_B2B(newPassword, changedDate, BusinessUnit, username, oldPassword))
        End If
    End Function

    Public Overrides Function ChangePasswordQuestionAndAnswer(ByVal username As String, ByVal password As String, ByVal newPasswordQuestion As String, ByVal newPasswordAnswer As String) As Boolean
        Return Nothing
    End Function
    Public Overrides Function CreateUser(ByVal username As String, ByVal password As String, ByVal email As String, ByVal passwordQuestion As String, ByVal passwordAnswer As String, ByVal isApproved As Boolean, ByVal providerUserKey As Object, ByRef status As System.Web.Security.MembershipCreateStatus) As System.Web.Security.MembershipUser

        Dim users As New TalentMembershipDatasetTableAdapters.tbl_authorized_usersTableAdapter 

        users.Insert(BusinessUnitGroup, Partner, username, password, False, isApproved, False, Now, Now, Now, Now)
        Return New TalentMembershipUser(Membership.Provider.Name, BusinessUnit, Partner, username, email, False, providerUserKey, passwordQuestion, Nothing, isApproved, False, Now, Now, Now, Now, Now, password)
    End Function

    Public Overloads Function CreateUser(ByVal username As String, ByVal password As String, ByVal email As String, ByVal passwordQuestion As String, ByVal passwordAnswer As String, ByVal isApproved As Boolean, ByVal providerUserKey As Object, ByRef status As System.Web.Security.MembershipCreateStatus, ByVal SuppliedPartner As String) As System.Web.Security.MembershipUser

        Dim users As New TalentMembershipDatasetTableAdapters.tbl_authorized_usersTableAdapter 

        users.Insert(BusinessUnitGroup, SuppliedPartner, username, password, False, isApproved, False, Now, Now, Now, Now)
        Return New TalentMembershipUser(Membership.Provider.Name, BusinessUnit, SuppliedPartner, username, email, False, providerUserKey, passwordQuestion, Nothing, isApproved, False, Now, Now, Now, Now, Now, password)
    End Function
    Public Overrides Function DeleteUser(ByVal username As String, ByVal deleteAllRelatedData As Boolean) As Boolean
        Return Nothing
    End Function
    Public Overrides ReadOnly Property EnablePasswordReset() As Boolean
        Get
            Return Nothing
        End Get
    End Property
    Public Overrides ReadOnly Property EnablePasswordRetrieval() As Boolean
        Get
            Return Nothing
        End Get
    End Property
    Public Overrides Function FindUsersByEmail(ByVal emailToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As System.Web.Security.MembershipUserCollection
        Return Nothing
    End Function
    Public Overrides Function FindUsersByName(ByVal usernameToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As System.Web.Security.MembershipUserCollection
        Return Nothing
    End Function

    Public Overrides Function GetAllUsers(ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As System.Web.Security.MembershipUserCollection
        Dim muc As New MembershipUserCollection
       
        Return muc
    End Function
    Public Overrides Function GetNumberOfUsersOnline() As Integer
        Return Nothing
    End Function
    Public Overrides Function GetPassword(ByVal username As String, ByVal answer As String) As String
        Return Nothing
    End Function
    Public Overloads Overrides Function GetUser(ByVal providerUserKey As Object, ByVal userIsOnline As Boolean) As System.Web.Security.MembershipUser
        Return Nothing
    End Function
    Public Overloads Overrides Function GetUser(ByVal username As String, ByVal userIsOnline As Boolean) As System.Web.Security.MembershipUser


        Dim users As New TalentMembershipDatasetTableAdapters.tbl_authorized_usersTableAdapter 
        Dim authPartners As New TalentMembershipDatasetTableAdapters.tbl_authorized_partnersTableAdapter 

        Dim dt As Data.DataTable
        'If userIsOnline Then
        Try
            If authPartners.Get_CheckFor_B2C_Login(BusinessUnit).Rows.Count > 0 Then
                dt = users.Get_B2C_GetUserMethod(BusinessUnitGroup, Partner, username)
                If dt.Rows.Count > 0 Then
                    Return New TalentMembershipUser(Membership.Provider.Name, _
                                                BusinessUnit, _
                                                Partner, _
                                                dt.Rows(0)("LOGINID"), _
                                                "", _
                                                dt.Rows(0)("AUTO_PROCESS_DEFAULT_USER"), _
                                                BusinessUnit & Partner & username, _
                                                Nothing, _
                                                Nothing, _
                                                True, _
                                                False, _
                                                Talent.eCommerce.Utilities.CheckForDBNull_Date(dt.Rows(0)("CREATED_DATE")), _
                                                Talent.eCommerce.Utilities.CheckForDBNull_Date(dt.Rows(0)("LAST_LOGIN_DATE")), _
                                                Talent.eCommerce.Utilities.CheckForDBNull_Date(dt.Rows(0)("LAST_LOGIN_DATE")), _
                                                Talent.eCommerce.Utilities.CheckForDBNull_Date(dt.Rows(0)("LAST_PASSWORD_CHANGED_DATE")), _
                                                Talent.eCommerce.Utilities.CheckForDBNull_Date(dt.Rows(0)("LAST_LOCKED_OUT_DATE")), _
                                                dt.Rows(0)("PASSWORD"))
                Else
                    Return Nothing
                End If
            Else
                dt = users.Get_B2B_GetUserMethod(BusinessUnit, username)
                If dt.Rows.Count > 0 Then
                    Return New TalentMembershipUser(Membership.Provider.Name, _
                                                BusinessUnit, _
                                                Partner, _
                                                dt.Rows(0)("LOGINID"), _
                                                "", _
                                                dt.Rows(0)("AUTO_PROCESS_DEFAULT_USER"), _
                                                BusinessUnit & Partner & username, _
                                                Nothing, _
                                                Nothing, _
                                                True, _
                                                False, _
                                                dt.Rows(0)("CREATED_DATE"), _
                                                dt.Rows(0)("LAST_LOGIN_DATE"), _
                                                dt.Rows(0)("LAST_LOGIN_DATE"), _
                                                dt.Rows(0)("LAST_PASSWORD_CHANGED_DATE"), _
                                                dt.Rows(0)("LAST_LOCKED_OUT_DATE"), _
                                                dt.Rows(0)("PASSWORD"))
                Else
                    Return Nothing
                End If
            End If
        Catch ex As Exception
            'TODO: Report/handle error
            Return Nothing
        End Try
        ' End If
    End Function
    Public Overrides Function GetUserNameByEmail(ByVal email As String) As String
        Return Nothing
    End Function
    Public Overrides ReadOnly Property MaxInvalidPasswordAttempts() As Integer
        Get
            Return Nothing
        End Get
    End Property
    Public Overrides ReadOnly Property MinRequiredNonAlphanumericCharacters() As Integer
        Get
            Return Nothing
        End Get
    End Property

    Public Overrides ReadOnly Property MinRequiredPasswordLength() As Integer
        Get
            Return Nothing
        End Get
    End Property
    Public Overrides ReadOnly Property PasswordAttemptWindow() As Integer
        Get
            Return Nothing
        End Get
    End Property
    Public Overrides ReadOnly Property PasswordFormat() As System.Web.Security.MembershipPasswordFormat
        Get
            Return Nothing
        End Get
    End Property
    Public Overrides ReadOnly Property PasswordStrengthRegularExpression() As String
        Get
            Return PasswordRegularExpression
        End Get
    End Property
    Public Overrides ReadOnly Property RequiresQuestionAndAnswer() As Boolean
        Get
            Return Nothing
        End Get
    End Property
    Public Overrides ReadOnly Property RequiresUniqueEmail() As Boolean
        Get
            Return Nothing
        End Get
    End Property
    Public Overrides Function ResetPassword(ByVal username As String, ByVal answer As String) As String
        Return Nothing
    End Function
    Public Overrides Function UnlockUser(ByVal userName As String) As Boolean
        Return Nothing
    End Function
    Public Overrides Sub UpdateUser(ByVal user As System.Web.Security.MembershipUser)

    End Sub
    Public Sub ChangeLoginID(ByVal OldLoginID As String, ByVal NewLoginID As String)

        Dim users As New TalentMembershipDatasetTableAdapters.tbl_authorized_usersTableAdapter 
        Dim authPartners As New TalentMembershipDatasetTableAdapters.tbl_authorized_partnersTableAdapter 

        If authPartners.Get_CheckFor_B2C_Login(BusinessUnit).Rows.Count > 0 Then
            users.ChangeLoginID_B2C(NewLoginID, BusinessUnitGroup, Partner, OldLoginID)
        Else
            users.ChangeLoginID_B2B(NewLoginID, BusinessUnit, OldLoginID)
        End If
    End Sub

    Public Overrides Function ValidateUser(ByVal username As String, ByVal password As String) As Boolean
        username = username.Trim()
        Dim users As New TalentMembershipDatasetTableAdapters.tbl_authorized_usersTableAdapter
        Dim authPartners As New TalentMembershipDatasetTableAdapters.tbl_authorized_partnersTableAdapter
        Dim tDataObjects As New Talent.Common.TalentDataObjects
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

        Try
            If authPartners.Get_CheckFor_B2C_Login(BusinessUnit).Rows.Count > 0 Then

                ' Refresh user data on login
                If def.RefreshCustomerDataOnLogin = True And def.ExternalPasswordIsMaster Then
                    Dim valid As Boolean = False
                    If username = GlobalConstants.GENERIC_CUSTOMER_NUMBER Then
                        valid = IsValidGenericCustomer(username, password)
                    Else
                        valid = VerifyAndRefreshCustomerData(username, password)
                    End If
                    If valid Then
                        SingleSignOn(username, password)
                        'Need to set any reset password tokens as expired in tbl_forgotten_password if the user 
                        'successfully logs in.
                        tDataObjects.ProfileSettings.tblForgottenPassword.SetCustomerTokensAsUsed(username, "Expired")
                    End If
                    Return valid
                Else

                    ' You can use email address or login id when login lookup is set
                    If def.UseLoginLookup Then
                        Dim profile1 As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter
                        Dim dt As Data.DataTable = profile1.GetDataByEmail(username, Partner)

                        ' Convert the email address to customer number.  If RefreshCustomerDataOnLogin is set to True
                        ' then this override needs to happen on the backend
                        If dt.Rows.Count = 1 And def.RefreshCustomerDataOnLogin <> True Then
                            username = dt.Rows(0).Item("LOGINID")
                        ElseIf dt.Rows.Count > 1 Then
                            ' Duplicate email addresses

                            If def.DisplayAccountSelectionOnLogin Then
                                'If multiple email addresses are allowed, then check that the password
                                'provided is valid for at least one of the accounts
                                Dim tryLogin As String = ""
                                For Each logon As DataRow In dt.Rows
                                    tryLogin = Talent.eCommerce.Utilities.CheckForDBNull_String(logon("LOGINID"))

                                    If Not String.IsNullOrEmpty(tryLogin) Then
                                        If users.Get_B2C_Login(TalentCache.GetBusinessUnitGroup, Partner, tryLogin, password).Rows.Count > 0 Then
                                            'Redirect to the account selection page
                                            Dim encryptionKey As String = ConfigurationManager.AppSettings("PrivateEncryptionKey") & Now.ToString("ddMMyyHH")
                                            Dim redirectUrl As String = "~/PagesPublic/Login/AccountSelection.aspx?email=" & username & "&password=" & _
                                                                        Talent.Common.Utilities.TripleDESEncode(password, encryptionKey)
                                            If Not String.IsNullOrEmpty(HttpContext.Current.Request("ReturnUrl")) Then
                                                redirectUrl += "&ReturnUrl=" & HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request("ReturnUrl"))
                                            End If
                                            HttpContext.Current.Response.Redirect(redirectUrl)
                                        End If
                                    End If
                                Next

                            Else
                                ' user must use login id
                                Return False
                            End If
                        End If

                    End If

                    ' Add leading zeros when loginId is customerNumber
                    'If CType(def.LoginidIsCustomerNumber, Boolean) Then
                    If def.LoginidType.Equals("1") Then
                        username = Talent.Common.Utilities.PadLeadingZeros(username.Trim, "12")
                    End If

                    'The remember me function uses auto login, therefore no password validation is done but the user is still logged in
                    If def.RememberMeFunction Then
                        If HttpContext.Current.Session.Item("passwordType") IsNot Nothing AndAlso HttpContext.Current.Session.Item("passwordType") = "N" Then
                            Utilities.UpdatePasswordValidated(username, False)
                            Return True
                        End If
                    End If

                    ' Check for validation against a BU Group. If this is not set, BU will be used instead
                    ' Also check if password encryption is enabled
                    Dim passwordValue As String = password
                    If def.LoginLookupType = GlobalConstants.ECOMMERCE_TYPE Then
                        If def.UseEncryptedPassword Then
                            Dim passHash As New Talent.Common.PasswordHash
                            passwordValue = passHash.HashSalt(password, def.SaltString)
                        End If
                    End If
                    Dim validUsers As DataTable = users.Get_B2C_Login(TalentCache.GetBusinessUnitGroup, Partner, username, passwordValue)
                    If validUsers.Rows.Count > 0 And password <> def.ExternalPasswordValue Then
                        Utilities.UpdatePasswordValidated(username, True)
                        SingleSignOn(username, passwordValue)
                        Return True
                    Else
                        Return False
                    End If

                End If

            Else
                Dim dt As New Data.DataTable
                ' You can use email address or login id when login lookup is set
                If def.UseLoginLookup Then

                    Dim profile1 As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter
                    dt = profile1.GetDataByEmailNoPartner(username)
                    If dt.Rows.Count = 1 Then
                        username = dt.Rows(0).Item("LOGINID")
                        '**TODO** Set partner = to partner returned from DB
                    ElseIf dt.Rows.Count > 1 Then
                        ' Duplicate email addresses
                        If def.DisplayAccountSelectionOnLogin Then
                            'If multiple email addresses are allowed, then check that the password
                            'provided is valid for at least one of the accounts
                            Dim tryLogin As String = ""
                            For Each logon As DataRow In dt.Rows
                                tryLogin = Talent.eCommerce.Utilities.CheckForDBNull_String(logon("LOGINID"))

                                If Not String.IsNullOrEmpty(tryLogin) Then

                                    If users.Get_B2B_Login(TalentCache.GetBusinessUnitGroup, tryLogin, password).Rows.Count > 0 Then
                                        '**TODO** Set partner = selected users partner within AccountSelection
                                        'Redirect to the account selection page
                                        Dim encryptionKey As String = ConfigurationManager.AppSettings("PrivateEncryptionKey") & Now.ToString("ddMMyyHH")
                                        Dim redirectUrl As String = "~/PagesPublic/Login/AccountSelection.aspx?email=" & username & "&password=" & _
                                                                    Talent.Common.Utilities.TripleDESEncode(password, encryptionKey)
                                        If Not String.IsNullOrEmpty(HttpContext.Current.Request("ReturnUrl")) Then
                                            redirectUrl += "&ReturnUrl=" & HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request("ReturnUrl"))
                                        End If
                                        HttpContext.Current.Response.Redirect(redirectUrl)
                                    End If
                                End If
                            Next
                        Else
                            ' user must use login id
                            Return False
                        End If
                    End If

                End If

                '-------------------------------------------------
                ' Check for validation against a BU Group. If this
                ' is not set, BU will be used instead
                '-------------------------------------------------
                Dim validUsers As DataTable = users.Get_B2B_Login(TalentCache.GetBusinessUnitGroup, username, password)
                If validUsers.Rows.Count > 0 Then
                    Dim restrictDirectAccess As Boolean = False
                    If def.ValidatePartnerDirectAccess Then
                        Dim tblPartnerAdapter As New TalentMembershipDatasetTableAdapters.tbl_partnerTableAdapter
                        Dim dtPartner As DataTable = tblPartnerAdapter.GetDataByPartner(validUsers.Rows(0)("PARTNER"))
                        If dtPartner.Rows.Count > 0 Then
                            restrictDirectAccess = Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(dtPartner.Rows(0)("RESTRICT_DIRECT_ACCESS"))
                        Else
                            restrictDirectAccess = False
                        End If
                    End If
                    If restrictDirectAccess Then
                        If HttpContext.Current.Session("SAP_OCI_HOOK_URL") Is Nothing Then
                            Return False
                        End If
                    End If

                    SingleSignOn(username, password)

                    If def.RefreshCustomerDataOnLogin And def.DisplayAccountSelectionOnLogin Then
                        If dt.Rows.Count = 0 Then
                            Dim profile1 As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter
                            dt = profile1.GetDataByLoginIDNoPartner(username)
                        End If
                        UpdateAddressRecordFromCRM(username, dt)
                    End If

                    Utilities.UpdatePasswordValidated(username, True, validUsers.Rows(0)("PARTNER"))
                    Return True
                Else
                    Return False
                End If
            End If

        Catch ex As Exception
            'TODO: Report/handle error
            Return False
        End Try


    End Function
    ''' <summary>
    ''' Validates the username against tbl_authorised_user
    ''' </summary>
    ''' <param name="username">The username.</param>
    ''' <returns>If exists <c>True</c> otherwise <c>False</c></returns>
    Public Function ValidateUserNoPassword(ByVal username As String) As Boolean
        Dim users As New TalentMembershipDatasetTableAdapters.tbl_authorized_usersTableAdapter
        Try
            Dim dt As Data.DataTable = users.GetByLoginID(username)
            If dt.Rows.Count = 1 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            'TODO: Report/handle error
            Return False
        End Try
    End Function

    Private Function VerifyAndRefreshCustomerData(ByRef username As String, ByVal password As String) As Boolean
        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay

        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        Dim validUser As Boolean = False
        Dim updatePassword As Boolean = True
        Dim tDataObjects As New Talent.Common.TalentDataObjects
        Dim tDataObjectsSettings As Talent.Common.DESettings = Utilities.GetSettingsObject()
        tDataObjects.Settings = tDataObjectsSettings

        Try

            ' Set up the calls to add items to ticketing basket
            Dim _talentErrObj As New Talent.Common.ErrorObj
            Dim _talentCustomer As New Talent.Common.TalentCustomer
            Dim _deCustomer As New Talent.Common.DECustomer
            Dim _deSettings As New Talent.Common.DESettings

            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .PageCode = Talent.Common.Utilities.GetAllString
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "MembershipProviderRefreshData.vb"
            End With

            ' Set the settings data entity. 
            _deSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            _deSettings.BusinessUnit = TalentCache.GetBusinessUnit()
            _deSettings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            _deSettings.Cacheing = CType(Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("Cacheing")), Boolean)
            _deSettings.CacheTimeMinutes = Talent.eCommerce.Utilities.CheckForDBNull_Int(ucr.Attribute("CacheTimeMinutes"))
            _deSettings.CacheDependencyPath = def.CacheDependencyPath
            _deSettings.TicketingKioskMode = def.TicketingKioskMode
            _deSettings.Partner = TalentCache.GetPartner(HttpContext.Current.Profile)

            ' Set the customer defaults
            If username.Contains("@") AndAlso username.Contains(".") Then
                _deCustomer.EmailAddress = username
            Else
                _deCustomer.CustomerNumber = username
            End If
            _deCustomer.Password = password
            _deCustomer.Source = "W"

            'If we are doing a customer retrieve from the activate account page
            'we need to populate the activate account properties from the form fields here to send to CRM
            _deCustomer.CompanySLNumber1 = _deCustomerFromActivateAccount.CompanySLNumber1
            _deCustomer.CompanySLNumber2 = _deCustomerFromActivateAccount.CompanySLNumber2
            _deCustomer.ContactForename = _deCustomerFromActivateAccount.ContactForename
            _deCustomer.ContactSurname = _deCustomerFromActivateAccount.ContactSurname
            _deCustomer.PostCode = _deCustomerFromActivateAccount.PostCode
            _deCustomer.RetreiveOnlyMode = _deCustomerFromActivateAccount.RetreiveOnlyMode


            If HttpContext.Current.Session("passwordType") IsNot Nothing Then
                _deCustomer.PasswordType = HttpContext.Current.Session("passwordType")
                HttpContext.Current.Session("passwordType") = Nothing
                updatePassword = False
                'Backend sometimes returns nonsense when signing in in BUI
                'Password causes some issues as a result
                If _deCustomer.PasswordType = "N" Then
                    password = String.Empty
                End If
            End If

            If moduleDefaults.GetDefaults.UseEncryptedPassword = True Then
                _deCustomer.UseEncryptedPassword = True
                If Not password Is String.Empty Then
                    If HttpContext.Current.Session("PasswordHashed") Is Nothing Then
                        Dim passHash As New Talent.Common.PasswordHash
                        _deCustomer.HashedPassword = passHash.HashSalt(password, moduleDefaults.GetDefaults.SaltString)
                    Else
                        _deCustomer.HashedPassword = password
                        HttpContext.Current.Session("PasswordHashed") = Nothing
                    End If
                End If
            End If

            Try
                Dim sStadia() As String = def.TicketingStadium.Split(",")
                _deSettings.Stadium = sStadia(0)
            Catch ex As Exception
            End Try

            Dim dtbl As New DataTable
            Dim ep As New Talent.eCommerce.ECommercePromotionDefaults
            dtbl = ep.GetPromotionDefaults

            Dim position As Integer = 1
            If dtbl.Rows.Count > 0 Then
                For Each r As DataRow In dtbl.Rows
                    Dim column As String = Talent.Common.Utilities.CheckForDBNull_String(r("REQUIRED_USER_ATTRIBUTE"))
                    Select Case position
                        Case Is = 1
                            _deCustomer.Attribute01 = column
                        Case Is = 2
                            _deCustomer.Attribute02 = column
                        Case Is = 3
                            _deCustomer.Attribute03 = column
                        Case Is = 4
                            _deCustomer.Attribute04 = column
                        Case Is = 5
                            _deCustomer.Attribute05 = column
                        Case Is = 6
                            _deCustomer.Attribute06 = column
                        Case Is = 7
                            _deCustomer.Attribute07 = column
                        Case Is = 8
                            _deCustomer.Attribute08 = column
                        Case Is = 9
                            _deCustomer.Attribute09 = column
                        Case Is = 10
                            _deCustomer.Attribute10 = column
                        Case Is = 11
                            _deCustomer.Attribute11 = column
                        Case Is = 12
                            _deCustomer.Attribute12 = column
                        Case Is = 13
                            _deCustomer.Attribute13 = column
                        Case Is = 14
                            _deCustomer.Attribute14 = column
                        Case Is = 15
                            _deCustomer.Attribute15 = column
                        Case Is = 16
                            _deCustomer.Attribute16 = column
                        Case Is = 17
                            _deCustomer.Attribute17 = column
                        Case Is = 18
                            _deCustomer.Attribute18 = column
                        Case Is = 19
                            _deCustomer.Attribute19 = column
                        Case Is = 20
                            _deCustomer.Attribute20 = column
                    End Select
                    position += 1
                Next
            End If

            If def.AlertsEnabled Then
                _talentCustomer.RefreshUserAttributesOnLogin = def.AlertsRefreshAttributesAtLogin
                _deCustomer.BirthdayAlertEnabled = tDataObjects.AlertSettings.TblAlertDefinition.AlertInUse(TalentCache.GetBusinessUnit(), TalentCache.GetPartner(HttpContext.Current.Profile), "BirthdayAlert")
                _deCustomer.FAndFBirthdayAlertEnabled = tDataObjects.AlertSettings.TblAlertDefinition.AlertInUse(TalentCache.GetBusinessUnit(), TalentCache.GetPartner(HttpContext.Current.Profile), "FFBirthdayAlert")
                _deCustomer.CardExpiryPPSAlertEnabled = tDataObjects.AlertSettings.TblAlertDefinition.AlertInUse(TalentCache.GetBusinessUnit(), TalentCache.GetPartner(HttpContext.Current.Profile), "CCExpiryAlertPPS")
                _deCustomer.CardExpirySAVAlertEnabled = tDataObjects.AlertSettings.TblAlertDefinition.AlertInUse(TalentCache.GetBusinessUnit(), TalentCache.GetPartner(HttpContext.Current.Profile), "CCExpiryAlertSAV")
                _deCustomer.CardExpiryPPSWarnPeriodDays = def.AlertsCCExpiryPPSWarnPeriod
                _deCustomer.CardExpirySAVWarnPeriodDays = def.AlertsCCExpirySAVWarnPeriod
            End If

            'Stuart 20090527
            _deCustomer.PhoneNoFormatting = def.PhoneNoFormat
            _deCustomer.PostCodeFormatting = def.PostCodeFormat
            'Stuart 20090527

            'The SelectedLoginOption session object is populated when on the login box there are additional login options allowing the supporter to enter
            'something other than a customer number to login with. They may enter a passport or national id number instead, these addtional options are
            'passed to talent in the user id fields.
            If HttpContext.Current.Session("SelectedLoginOption") IsNot Nothing Then
                Dim loginOption As Integer = HttpContext.Current.Session("SelectedLoginOption")
                _deCustomer.CustomerNumber = String.Empty
                Select Case loginOption
                    Case Is = 1 : _deCustomer.PassportNumber = username
                    Case Is = 2 : _deCustomer.GreenCardNumber = username
                    Case Is = 3 : _deCustomer.PIN_Number = username
                    Case Is = 4 : _deCustomer.User_ID_4 = username
                    Case Is = 5 : _deCustomer.User_ID_5 = username
                    Case Is = 6 : _deCustomer.User_ID_6 = username
                    Case Is = 7 : _deCustomer.User_ID_7 = username
                    Case Is = 8 : _deCustomer.User_ID_8 = username
                    Case Is = 9 : _deCustomer.User_ID_9 = username
                    Case Else : _deCustomer.CustomerNumber = username
                End Select
                HttpContext.Current.Session("SelectedLoginOption") = Nothing
            End If

            ' Invoke.
            Dim deCustV11 As New Talent.Common.DECustomerV11
            Dim talProfileProvider As New TalentProfileProvider
            Dim anonymousLoginId As String = HttpContext.Current.Profile.UserName
            Dim basket As TalentBasket = talProfileProvider.GetBasket(anonymousLoginId, False)
            _deSettings.IsAgent = _agentProfile.IsAgent
            _deCustomer.BasketID = basket.Basket_Header_ID
            deCustV11.DECustomersV1.Add(_deCustomer)
            _talentCustomer.DeV11 = deCustV11
            _talentCustomer.Settings = _deSettings
            _talentCustomer.UseSaveMyCard = def.UseSaveMyCard
            _talentErrObj = _talentCustomer.VerifyAndRetrieveCustomerDetails

            Dim registeredAddress As New TalentProfileAddress

            ' Refresh the front end data on a successful return
            If Not _talentErrObj.HasError _
                AndAlso Not _talentCustomer.ResultDataSet Is Nothing _
                AndAlso _talentCustomer.ResultDataSet.Tables(0).Rows.Count > 0 Then

                ' Has there been an internal error
                Dim errorTableDataRow As DataRow
                Dim userDetailsDataRow As DataRow
                Dim userAttributesDataRow As DataRow
                errorTableDataRow = _talentCustomer.ResultDataSet.Tables(0).Rows(0)
                If errorTableDataRow("ErrorOccurred") <> GlobalConstants.ERRORFLAG AndAlso _talentCustomer.ResultDataSet.Tables.Count > 1 Then

                    'Try to retrieve the current profile
                    Dim p As New TalentProfileUser
                    Dim Profile1 As New TalentProfileProvider
                    Dim userAddress As New TalentProfileAddress
                    Dim userDetails As New TalentProfileUserDetails

                    'Read the customer details
                    userDetailsDataRow = _talentCustomer.ResultDataSet.Tables(1).Rows(0)
                    userAttributesDataRow = _talentCustomer.ResultDataSet.Tables(3).Rows(0)

                    'Set the username to be customer number if required
                    'If CType(def.LoginidIsCustomerNumber, Boolean) Then
                    If def.LoginidType.Equals("1") Then
                        username = userDetailsDataRow("CustomerNumber")
                    End If

                    If Not String.IsNullOrEmpty(userDetailsDataRow.Item("CompanyNumber").ToString()) Then
                        HttpContext.Current.Session("LoggedInCompanyNumber") = userDetailsDataRow.Item("CompanyNumber").ToString()
                        HttpContext.Current.Session("LoggedInCompanyName") = userDetailsDataRow.Item("CRMCompanyName").ToString()
                    End If


                    'Set the profile details if the user has been to the web site before
                    p = Profile1.GetUserByLoginID(username)
                    If Not p.Details.LoginID Is Nothing Then
                        userAddress = ProfileHelper.ProfileAddressEnumerator(0, p.Addresses)
                        userDetails = p.Details
                    End If

                    'Refresh the user details from the backend
                    With userDetails
                        .LoginID = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("CustomerNumber"))
                        .Email = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("EmailAddress")).ToString.Trim
                        .Title = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactTitle")).ToString.Trim
                        .Initials = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactInitials")).ToString.Trim
                        .Forename = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactForename")).ToString.Trim
                        .Surname = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactSurname")).ToString.Trim
                        .Full_Name = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactForename")).ToString.Trim & " " & Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactSurname")).ToString.Trim
                        If CType(Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(def.ProfileFullNameWithTitle), Boolean) Then
                            If Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactTitle")).ToString.Trim.Length > 0 Then
                                .Full_Name = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactTitle")).ToString.Trim & " " & _
                                                Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactForename")).ToString.Trim & " " & _
                                                Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactSurname")).ToString.Trim
                            End If
                        End If
                        .Salutation = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("Salutation")).ToString.Trim
                        .CompanyName = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("CompanyName")).ToString.Trim
                        .Position = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("PositionInCompany")).ToString.Trim
                        If Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("DateBirth")).ToString = "19000000" _
                            Or Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("DateBirth")).ToString = "20000000" Then
                            .DOB = Date.ParseExact("01/01/1900", "dd/MM/yyyy", Nothing)
                        Else
                            .DOB = Date.ParseExact(Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("DateBirth")).ToString.Substring(6, 2) & _
                                    "/" & Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("DateBirth")).ToString.Substring(4, 2) & _
                                    "/" & Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("DateBirth")).ToString.Substring(0, 4), "dd/MM/yyyy", Nothing)
                        End If
                        .Sex = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("Gender"))
                        .Mobile_Number = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("MobileNumber")).ToString.Trim
                        .Telephone_Number = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("HomeTelephoneNumber")).ToString.Trim
                        .Work_Number = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("WorkTelephoneNumber")).ToString.Trim
                        .Account_No_1 = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("CustomerNumber"))
                        .Favourite_Seat = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("FavouriteSeat"))
                        .Smartcard_Number = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("SmartcardNumber"))
                        .STOP_CODE = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("StopCode"))
                        .PRICE_BAND = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("PriceBand"))
                        .CONTACT_BY_EMAIL = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactbyEmail"))
                        .CONTACT_BY_MOBILE = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactbyMobile"))
                        .CONTACT_BY_POST = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactbyPost"))
                        .CONTACT_BY_TELEPHONE_HOME = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactbyTelephoneHome"))
                        .CONTACT_BY_TELEPHONE_WORK = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactbyTelephoneWork"))
                        .BOOK_NUMBER = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("BookNumber"))
                        .NICKNAME = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("NickName"))
                        .USERNAME = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("AltUserName"))
                        .CUSTOMER_SUFFIX = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("Suffix"))
                        .PARENT_PHONE = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ParentPhone"))
                        .PARENT_EMAIL = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ParentEmail"))
                        .PARENTAL_CONSENT_STATUS = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ConsentStatus"))



                        'Retrieve Mail flags
                        .Bit0 = False
                        If (Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactViaMail")).Trim = "Y") Or _
                           (Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactViaMail")).Trim = "1") Then
                            .Bit0 = True
                        End If
                        .Bit1 = False
                        If Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactViaMail1")).Trim = "Y" Then
                            .Bit1 = True
                        End If
                        .Bit2 = False
                        If Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactViaMail2")).Trim = "Y" Then
                            .Bit2 = True
                        End If
                        .Bit3 = False
                        If Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactViaMail3")).Trim = "Y" Then
                            .Bit3 = True
                        End If
                        .Bit4 = False
                        If Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactViaMail4")).Trim = "Y" Then
                            .Bit4 = True
                        End If
                        .Bit5 = False
                        If Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactViaMail5")).Trim = "Y" Then
                            .Bit5 = True
                        End If

                        If Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("BondHolder")).ToString = "Y" Then
                            .BOND_HOLDER = True
                        Else
                            .BOND_HOLDER = False
                        End If

                        Dim first As Boolean = True
                        .ATTRIBUTES_LIST = String.Empty
                        For i As Integer = 1 To 20
                            If Not Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("Attribute" & i)).Equals(String.Empty) And _
                                Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(userAttributesDataRow("Attribute" & i & "Exists")) Then
                                If first Then
                                    .ATTRIBUTES_LIST = userAttributesDataRow("Attribute" & i).ToString.Trim
                                Else
                                    .ATTRIBUTES_LIST &= ", " & userAttributesDataRow("Attribute" & i).ToString.Trim
                                End If
                                first = False
                            End If
                        Next

                        ' The information below is only required when adding the profile
                        If p.Details.LoginID Is Nothing Then
                            .User_Number = Talent.Common.Utilities.GetNextUserNumber(TalentCache.GetBusinessUnit, _
                                                   Partner, _
                                                  ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
                            .Subscribe_Newsletter = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("ContactViaMail"))
                            .HTML_Newsletter = False
                            .User_Number_Prefix = def.UserNumberPrefix
                        End If
                        Try
                            .Ticketing_Loyalty_Points = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("LoyaltyPoints"))
                        Catch ex As Exception
                            Talent.Common.Utilities.TalentCommonLog("Exception Logging", "TalentMembershipProvider.vb - VerifyAndRefreshCustomerData - 21.1", ex.Message)

                        End Try

                        'STUART
                        If Not String.IsNullOrEmpty(Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("OwnsAutoMembership"))) Then
                            'STUART
                            If Convert.ToString(userAttributesDataRow("OwnsAutoMembership")).ToUpper = "Y" Then
                                .OWNS_AUTO_MEMBERSHIP = True
                            Else
                                .OWNS_AUTO_MEMBERSHIP = False
                            End If
                        Else
                            .OWNS_AUTO_MEMBERSHIP = False
                        End If

                        '----------------------------------------
                        ' Additional fields retrieved from ws121r
                        '----------------------------------------
                        .FAVOURITE_SPORT = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("FavouriteSport")).ToString.Trim
                        .FAVOURITE_TEAM_CODE = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("FavouriteTeam")).ToString.Trim
                        .SUPPORTER_CLUB_CODE = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("FavouriteSupportersClub")).ToString.Trim
                        .MAIL_TEAM_CODE_1 = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("SportMailFlag1")).ToString.Trim
                        .MAIL_TEAM_CODE_2 = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("SportMailFlag2")).ToString.Trim
                        .MAIL_TEAM_CODE_3 = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("SportMailFlag3")).ToString.Trim
                        .MAIL_TEAM_CODE_4 = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("SportMailFlag4")).ToString.Trim
                        .MAIL_TEAM_CODE_5 = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("SportMailFlag5")).ToString.Trim
                        .PREFERRED_CONTACT_METHOD = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("PreferredContactMethod")).ToString.Trim
                        .Mothers_Name = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("MothersName")).ToString.Trim
                        .Fathers_Name = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("FathersName")).ToString.Trim

                        registeredAddress.Reference = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("RegisteredAddress1")).ToString.Trim
                        registeredAddress.Address_Line_2 = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("RegisteredAddress1")).ToString.Trim
                        registeredAddress.Address_Line_3 = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("RegisteredAddress2")).ToString.Trim
                        registeredAddress.Address_Line_4 = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("RegisteredAddress3")).ToString.Trim
                        registeredAddress.Address_Line_5 = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("RegisteredAddress4")).ToString.Trim
                        If Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("RegisteredAddress5")).ToString.Trim.Length > 20 Then
                            registeredAddress.Country = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("RegisteredAddress5")).ToString.Trim.Substring(0, 20)
                        Else
                            registeredAddress.Country = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("RegisteredAddress5")).ToString.Trim
                        End If
                        registeredAddress.Post_Code = Talent.eCommerce.Utilities.CheckForDBNull_String(userAttributesDataRow("RegisteredPostcode")).ToString.Trim
                        registeredAddress.Address_Line_1 = ""

                        'Populate User ID Fields in userdetails
                        .Passport = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("PassportNumber")).ToString.Trim
                        .GreenCard = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("GreenCardNumber")).ToString.Trim
                        .PIN = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("PINNumber")).ToString.Trim
                        .User_ID_4 = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("UserID4")).ToString.Trim
                        .User_ID_5 = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("UserID5")).ToString.Trim
                        .User_ID_6 = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("UserID6")).ToString.Trim
                        .User_ID_7 = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("UserID7")).ToString.Trim
                        .User_ID_8 = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("UserID8")).ToString.Trim
                        .User_ID_9 = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("UserID9")).ToString.Trim
                    End With

                    ' Refresh the address details from the backend
                    With userAddress
                        .LoginID = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("CustomerNumber"))
                        .Reference = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("AddressLine1")).ToString.Trim
                        .Address_Line_2 = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("AddressLine1")).ToString.Trim
                        .Address_Line_3 = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("AddressLine2")).ToString.Trim
                        .Address_Line_4 = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("AddressLine3")).ToString.Trim
                        .Address_Line_5 = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("AddressLine4")).ToString.Trim
                        .Country = Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("AddressLine5")).ToString.Trim
                        .Post_Code = UCase(Talent.eCommerce.Utilities.CheckForDBNull_String(userDetailsDataRow("Postcode"))).ToString.Trim

                        'This information is only required on addition
                        If p.Details.LoginID Is Nothing Then
                            .Default_Address = True
                            .Sequence = 0
                            .Type = ""
                            .Address_Line_1 = ""
                        End If
                    End With

                    If Not p.Details.LoginID Is Nothing Then

                        ' Update the user
                        Dim userProfileDetails As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter
                        Dim strPartner As String = Partner
                        Dim count As Integer = 0
                        tDataObjects.ProfileSettings.tblAuthorizedUsers.AddAuthorisedUser(username, Partner, BusinessUnit)
                        With userDetails


                            userProfileDetails.UpdatePartnerUser(Partner, .LoginID, .Email, .Title, .Initials, .Forename, .Surname, .Full_Name, _
                                                        .Salutation, .Position, .DOB, .Mobile_Number, .Telephone_Number, .Work_Number, .Fax_Number, _
                                                        .Other_Number, .Messaging_ID, .User_Number, BusinessUnit, .Account_No_1, .Account_No_2, "", "", "", _
                                                        .Subscribe_Newsletter, .HTML_Newsletter, .Bit0, .Bit1, .Bit2, .Bit3, .Bit4, .Bit5, .Sex, .User_Number_Prefix, _
                                                        BusinessUnit, Talent.eCommerce.Utilities.GetCurrentLanguage, .RESTRICTED_PAYMENT_METHOD, .Subscribe_2, .Subscribe_3, _
                                                        .Ticketing_Loyalty_Points, .ATTRIBUTES_LIST, .BOND_HOLDER, .OWNS_AUTO_MEMBERSHIP, .DEFAULT_CLUB, .Enable_Price_View, _
                                                        .Minimum_Purchase_Quantity, .Minimum_Purchase_Amount, .Use_Minimum_Purchase_Quantity, .Use_Minimum_Purchase_Amount, _
                                                        .SUPPORTER_CLUB_CODE, .FAVOURITE_TEAM_CODE, .MAIL_TEAM_CODE_1, .MAIL_TEAM_CODE_2, .MAIL_TEAM_CODE_3, .MAIL_TEAM_CODE_4, _
                                                        .MAIL_TEAM_CODE_5, .PREFERRED_CONTACT_METHOD, .FAVOURITE_SPORT, .Mothers_Name, .Fathers_Name, _
                                                        .PIN, .Passport, .GreenCard, .User_ID_4, .User_ID_5, .User_ID_6, .User_ID_7, .User_ID_8, .User_ID_9, .CompanyName, .Favourite_Seat, .Smartcard_Number, .STOP_CODE, .PRICE_BAND, .CONTACT_BY_POST, .CONTACT_BY_TELEPHONE_HOME, .CONTACT_BY_TELEPHONE_WORK, .CONTACT_BY_MOBILE, .CONTACT_BY_EMAIL, .BOOK_NUMBER, .CUSTOMER_SUFFIX, .NICKNAME, .USERNAME, .PARENT_PHONE, .PARENT_EMAIL, .PARENTAL_CONSENT_STATUS, .LoginID)
                        End With

                        'Update the address
                        Dim ProfileAddress As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter
                        ProfileAddress.UpdateUserAddress(Partner, _
                                                        userAddress.LoginID, _
                                                        userAddress.Type, _
                                                        userAddress.Reference, _
                                                        userAddress.Sequence, _
                                                        userAddress.Default_Address, _
                                                        userAddress.Address_Line_1, _
                                                        userAddress.Address_Line_2, _
                                                        userAddress.Address_Line_3, _
                                                        userAddress.Address_Line_4, _
                                                        userAddress.Address_Line_5, _
                                                        UCase(userAddress.Post_Code), _
                                                        userAddress.Country, _
                                                        username, _
                                                        userAddress.Address_ID)

                        ' Update the registered address
                        Dim r As Integer = 0
                        r = ProfileAddress.UpdateAddressByLoginidTypeSequence(registeredAddress.Address_Line_1, _
                                                                            registeredAddress.Address_Line_2, _
                                                                            registeredAddress.Address_Line_3, _
                                                                            registeredAddress.Address_Line_4, _
                                                                            registeredAddress.Address_Line_5, _
                                                                            registeredAddress.Post_Code, _
                                                                            registeredAddress.Country, _
                                                                            userAddress.LoginID, _
                                                                            Partner, _
                                                                            "1", _
                                                                            "0")

                        'The remember me function uses auto login, therefore no password validation is done but the user is still logged in
                        If def.RememberMeFunction AndAlso _deCustomer.PasswordType = "N" Then
                            Utilities.UpdatePasswordValidated(username, False)
                        Else
                            Utilities.UpdatePasswordValidated(p.Details.LoginID, True)
                        End If
                    Else

                        'Create the profile
                        tDataObjects.ProfileSettings.tblAuthorizedUsers.AddAuthorisedUser(username, Partner, BusinessUnit)
                        Profile1.CreateProfile(userDetails, userAddress)
                        'The remember me function uses auto login, therefore no password validation is done but the user is still logged in
                        If def.RememberMeFunction AndAlso _deCustomer.PasswordType = "N" Then
                            Utilities.UpdatePasswordValidated(username, False)
                        Else
                            Utilities.UpdatePasswordValidated(p.Details.LoginID, True)
                        End If
                    End If

                    'Set the user name in session
                    HttpContext.Current.Session("BackEndUserName") = username
                    HttpContext.Current.Session("BackEndEmail") = userDetailsDataRow("EmailAddress").ToString.Trim

                    'Flag to determine that a reserved seat is available to purchase
                    If _talentCustomer.DeV11.DECustomersV1(0).HasReservedGameAvailable Then
                        HttpContext.Current.Session("ReservedSeatAvailableToBuy") = True
                    Else
                        HttpContext.Current.Session("ReservedSeatAvailableToBuy") = False
                    End If


                    validUser = True
                    If def.AlertsEnabled Then
                        HttpContext.Current.Session("NumberOfUnreadAlerts") = Nothing
                        Dim customerAttributes As DataTable = _talentCustomer.ResultDataSet.Tables("CustomerAttributes")
                        Dim specialAttributes As DataTable = _talentCustomer.ResultDataSet.Tables("SpecialAttributes")
                        If def.AlertsRefreshAttributesAtLogin Then tDataObjects.AlertSettings.TblUserAttribute.PopulateUserAttributes(customerAttributes, specialAttributes, userDetails.LoginID)
                        tDataObjects.AlertSettings.TblAlert.DeleteReservationAlertsByLoginID(userDetails.LoginID)
                        If def.AlertsGenerateAtLogin Then tDataObjects.AlertSettings.GenerateUserAlerts(TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile), userDetails.LoginID, def.AlertsCCExpiryPPSWarnPeriod, def.AlertsCCExpirySAVWarnPeriod)
                    End If
                Else
                    If errorTableDataRow("ReturnCode").ToString.Trim = "" Then
                        HttpContext.Current.Session("BackEndLoginError") = "XX"
                    Else
                        HttpContext.Current.Session("BackEndLoginError") = errorTableDataRow("ReturnCode")
                    End If
                End If
            Else
                HttpContext.Current.Session("BackEndLoginError") = "XX"
            End If
        Catch ex As Exception
            Talent.Common.Utilities.TalentCommonLog("Exception Logging", "TalentMembershipProvider.vb - VerifyAndRefreshCustomerData - 21.2", ex.Message)
            HttpContext.Current.Session("BackEndLoginError") = "XX"
        End Try

        Utilities.TalentLogging.LoadTestLog("TalentMembershipProvider.vb", "VerifyAndRefreshCustomerData", timeSpan)
        Return validUser

    End Function
    Private Sub UpdateAddressRecordFromCRM(ByVal username As String, ByVal tblPartnerUser As DataTable)
        Try
            Dim partner As String = tblPartnerUser.Rows(0)("PARTNER").ToString
            Dim accountNo2 As String = tblPartnerUser.Rows(0)("ACCOUNT_NO_2").ToString
            Dim forename As String = tblPartnerUser.Rows(0)("FORENAME").ToString
            Dim surname As String = tblPartnerUser.Rows(0)("SURNAME").ToString
            Dim postcode As String = String.Empty

            Dim dtAddress As DataTable = GetTalentDataObject().ProfileSettings.tblAddress.GetByLoginIdPartnerSequence(username, partner, 0)

            If dtAddress.Rows.Count = 1 Then
                postcode = dtAddress.Rows(0)("POST_CODE").ToString
                Dim dsCRMAddress As DataSet = VerifyAndRetrieveAccountFromCRM(partner, accountNo2, surname, postcode, forename)
                If dsCRMAddress.Tables.Count > 1 Then
                    If dsCRMAddress.Tables(0).Rows.Count > 0 Then
                        If String.IsNullOrEmpty(dsCRMAddress.Tables(0).Rows(0)(0).ToString) Then
                            Dim userTable As DataTable = dsCRMAddress.Tables(1)
                            If userTable.Rows.Count = 1 Then

                                'Create and populate the userAddress
                                Dim userAddress As New TalentProfileAddress
                                With userAddress
                                    .LoginID = username
                                    .Reference = userTable.Rows(0)("AddressLine1").ToString
                                    .Type = String.Empty
                                    .Default_Address = True
                                    .Address_Line_1 = userTable.Rows(0)("CompanyName").ToString
                                    .Address_Line_2 = userTable.Rows(0)("AddressLine1").ToString
                                    .Address_Line_3 = userTable.Rows(0)("AddressLine2").ToString
                                    .Address_Line_4 = userTable.Rows(0)("AddressLine3").ToString
                                    .Address_Line_5 = userTable.Rows(0)("AddressLine4").ToString
                                    .Post_Code = userTable.Rows(0)("PostCode").ToString
                                    .Country = userTable.Rows(0)("AddressLine5").ToString
                                    .Sequence = 0
                                End With

                                'Put address in database address table
                                Dim tblAddressTableAdapter As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter
                                tblAddressTableAdapter.UpdateAddressByLoginidTypeSequence(userAddress.Address_Line_1, _
                                    userAddress.Address_Line_2, userAddress.Address_Line_3, userAddress.Address_Line_4, _
                                    userAddress.Address_Line_5, userAddress.Post_Code, userAddress.Country, userAddress.LoginID, _
                                    partner, userAddress.Type, userAddress.Sequence)

                                'update restrict payment type
                                GetTalentDataObject().ProfileSettings.tblPartnerUser.UpdateRestrictPaymentMethod(userTable.Rows(0)("RestrictedPaymentTypes").ToString, username, partner)
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Function GetTalentDataObject() As Talent.Common.TalentDataObjects
        Dim talDataObjects As New Talent.Common.TalentDataObjects
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = "SQL2005"
        talDataObjects.Settings = settings
        Return talDataObjects
    End Function

    Private Sub SingleSignOn(ByVal username As String, ByVal password As String)
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        HttpContext.Current.Session("justLoggedOn") = "True"
        HttpContext.Current.Session("justLoggedOff") = "False"

        If def.CreateSingleSignOnCookies Then

            'If we are in password encrypted environment we must ensure we are storing encrypted password in sso cookie and not plain text password
            If moduleDefaults.GetDefaults.UseEncryptedPassword = True Then
                If Not password Is String.Empty Then
                    Dim passHash As New Talent.Common.PasswordHash
                    password = passHash.HashSalt(password, moduleDefaults.GetDefaults.SaltString)
                End If
            End If

            Dim secretKey As String = def.SingleSignOnSecretKey
            Dim hash1ToEncrypt As String = username & secretKey
            Dim hash2ToEncrypt As String = Talent.eCommerce.Utilities.aMD5Hash(password) & secretKey
            Dim cookieUsername As New System.Web.HttpCookie("USERNAME", username)
            Dim cookieEncryptedUsername As New System.Web.HttpCookie("HASH1", Talent.eCommerce.Utilities.aMD5Hash(hash1ToEncrypt))
            Dim cookieEncryptedPassword As New System.Web.HttpCookie("HASH2", Talent.eCommerce.Utilities.aMD5Hash(hash2ToEncrypt))

            If def.SingleSignOnDomain.Length > 0 Then
                cookieUsername.Domain = def.SingleSignOnDomain
                cookieEncryptedUsername.Domain = def.SingleSignOnDomain
                cookieEncryptedPassword.Domain = def.SingleSignOnDomain
            End If

            HttpContext.Current.Response.Cookies.Add(cookieUsername)
            HttpContext.Current.Response.Cookies.Add(cookieEncryptedUsername)
            HttpContext.Current.Response.Cookies.Add(cookieEncryptedPassword)
        End If
    End Sub

    Public Function ValidateBackendUser(ByVal username As String, ByVal password As String) As Boolean
        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults

        Dim isValid As Boolean = False

        ' Set up the calls to add items to ticketing basket
        Dim _talentErrObj As New Talent.Common.ErrorObj
        Dim _talentCustomer As New Talent.Common.TalentCustomer
        Dim _deCustomer As New Talent.Common.DECustomer
        Dim _deSettings As New Talent.Common.DESettings

        ' Set the settings data entity. 
        _deSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        _deSettings.BusinessUnit = TalentCache.GetBusinessUnit()
        _deSettings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()

        ' Set the customer defaults
        _deCustomer.UserName = username
        _deCustomer.Password = password
        _deCustomer.Source = "W"

        If moduleDefaults.GetDefaults.UseEncryptedPassword = True Then
            _deCustomer.UseEncryptedPassword = True
            If Not password Is String.Empty Then
                Dim passHash As New Talent.Common.PasswordHash
                _deCustomer.HashedPassword = passHash.HashSalt(password, moduleDefaults.GetDefaults.SaltString)
            End If
        End If

        ' Invoke.
        Dim deCustV11 As New Talent.Common.DECustomerV11
        deCustV11.DECustomersV1.Add(_deCustomer)
        _talentCustomer.DeV11 = deCustV11
        _talentCustomer.Settings = _deSettings
        _talentErrObj = _talentCustomer.VerifyPassword

        ' If it has error report a false login
        If _talentErrObj.HasError Then
            isValid = False
        Else
            Try
                ' Does this password equal the old ticketing password
                Dim dtStatusDetails As Data.DataTable
                Dim dr As Data.DataRow
                dtStatusDetails = _talentCustomer.ResultDataSet.Tables(0)
                dr = dtStatusDetails.Rows(0)

                If dr("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                    isValid = False
                Else
                    ' Ticketing password is correct, so update the authorised user table
                    isValid = True
                End If
            Catch ex As Exception
                isValid = False
            End Try
        End If

        Utilities.TalentLogging.LoadTestLog("TalentMembershipProvider.vb", "ValidateBackendUser", timeSpan)
        Return isValid
    End Function


    Public Function ChangeBackendPassword(ByVal username As String, ByVal Password As String, ByVal newPassword As String) As Boolean

        Dim isValid As Boolean = False

        ' Set up the calls to add items to ticketing basket
        Dim _talentErrObj As New Talent.Common.ErrorObj
        Dim _talentCustomer As New Talent.Common.TalentCustomer
        Dim _deCustomer As New Talent.Common.DECustomer
        Dim _deSettings As New Talent.Common.DESettings

        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults

        ' Set the settings data entity. 
        _deSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        _deSettings.BusinessUnit = TalentCache.GetBusinessUnit()
        _deSettings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()

        ' Set the customer defaults
        _deCustomer.UserName = username
        _deCustomer.NewPassword = newPassword
        _deCustomer.Password = Password
        _deCustomer.Source = "W"

        If moduleDefaults.GetDefaults.UseEncryptedPassword = True Then
            _deCustomer.UseEncryptedPassword = True
            Dim passHash As New Talent.Common.PasswordHash
            If Not newPassword Is String.Empty Then
                _deCustomer.NewHashedPassword = passHash.HashSalt(newPassword, moduleDefaults.GetDefaults.SaltString)
            End If
            If Not Password Is String.Empty Then
                _deCustomer.HashedPassword = passHash.HashSalt(Password, moduleDefaults.GetDefaults.SaltString)
            End If
            _deCustomer.SaltString = moduleDefaults.GetDefaults.SaltString
        End If


        ' Invoke.
        Dim deCustV11 As New Talent.Common.DECustomerV11
        deCustV11.DECustomersV1.Add(_deCustomer)
        _talentCustomer.DeV11 = deCustV11
        _talentCustomer.Settings = _deSettings
        'If in BUI, then user doesn't need to provide their current password.
        If _agentProfile.IsAgent Then
            _talentErrObj = _talentCustomer.ResetPassword
        Else
            _talentErrObj = _talentCustomer.UpdatePassword
        End If

        ' If it has error report a false login
        If _talentErrObj.HasError Then
            isValid = False
        Else
            Try
                ' Does this password equal the old ticketing password
                Dim dtStatusDetails As Data.DataTable
                Dim dr As Data.DataRow
                dtStatusDetails = _talentCustomer.ResultDataSet.Tables(0)
                dr = dtStatusDetails.Rows(0)

                If dr("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                    isValid = False
                Else
                    ' Update was successful
                    isValid = True
                End If
            Catch ex As Exception
                isValid = False
            End Try
        End If

        Return isValid
    End Function

    ''' <summary>
    ''' WARNING: This will delete the back-end password and return the Password Hint that the user can log in with.
    ''' </summary>
    ''' <param name="userName"></param>
    ''' <param name="email"></param>
    ''' <param name="loginType"></param>
    ''' <returns>Password Hint</returns>
    ''' <remarks></remarks>
    Public Function RetrieveBackendPassword(ByRef userName As String, ByVal email As String, ByVal loginType As Integer, ByRef errorCode As String) As String

        Dim myPassword As String = ""

        ' Set up the calls to add items to ticketing basket
        Dim _talentErrObj As New Talent.Common.ErrorObj
        Dim _talentCustomer As New Talent.Common.TalentCustomer
        Dim _deCustomer As New Talent.Common.DECustomer
        Dim _deSettings As New Talent.Common.DESettings

        ' Set the settings data entity. 
        _deSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        _deSettings.BusinessUnit = TalentCache.GetBusinessUnit()
        _deSettings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()

        ' Set the customer defaults
        _deCustomer.UserName = userName
        _deCustomer.EmailAddress = email
        _deCustomer.Source = "W"

        'The loginType paramater is greater than 0 when there are additional login options allowing the supporter to enter
        'something other than a customer number to login with. They may enter a passport or national id number instead, 
        'these addtional options are passed to talent in the user id fields.
        If loginType > 0 Then
            Dim loginOption As Integer = HttpContext.Current.Session("SelectedLoginOption")
            _deCustomer.UserName = String.Empty
            Select Case loginType
                Case Is = 1 : _deCustomer.PassportNumber = userName
                Case Is = 2 : _deCustomer.GreenCardNumber = userName
                Case Is = 3 : _deCustomer.PIN_Number = userName
                Case Is = 4 : _deCustomer.User_ID_4 = userName
                Case Is = 5 : _deCustomer.User_ID_5 = userName
                Case Is = 6 : _deCustomer.User_ID_6 = userName
                Case Is = 7 : _deCustomer.User_ID_7 = userName
                Case Is = 8 : _deCustomer.User_ID_8 = userName
                Case Is = 9 : _deCustomer.User_ID_9 = userName
                Case Else : _deCustomer.UserName = userName
            End Select

        End If

        ' Invoke.
        Dim deCustV11 As New Talent.Common.DECustomerV11
        deCustV11.DECustomersV1.Add(_deCustomer)
        _talentCustomer.DeV11 = deCustV11
        _talentCustomer.Settings = _deSettings
        _talentErrObj = _talentCustomer.RetrievePassword

        ' If it has error report a false login
        If _talentErrObj.HasError Then

        Else
            Try
                ' Does this password equal the old ticketing password
                If _talentCustomer.ResultDataSet.Tables.Count > 0 Then
                    Dim dtStatusDetails As Data.DataTable = _talentCustomer.ResultDataSet.Tables(0)
                    Dim dr As Data.DataRow = dtStatusDetails.Rows(0)

                    Dim dtValues As Data.DataTable = _talentCustomer.ResultDataSet.Tables(1)
                    Dim drValues As Data.DataRow = dtValues.Rows(0)

                    If dr("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                        errorCode = dr("ReturnCode").ToString()
                    Else
                        'Call was successful - return the new password:
                        myPassword = Talent.eCommerce.Utilities.CheckForDBNull_String(drValues("Password"))
                        If loginType > 0 AndAlso Not String.IsNullOrWhiteSpace(drValues("UserName")) Then
                            userName = Talent.eCommerce.Utilities.CheckForDBNull_String(drValues("UserName"))
                        End If
                    End If
                Else
                    'error
                End If

            Catch ex As Exception
                'error
            End Try
        End If

        Return myPassword.Trim

    End Function

    Public Sub StampLastUpdateDetailsDate(ByVal username As String)
        Dim users As New TalentMembershipDatasetTableAdapters.tbl_authorized_usersTableAdapter
        Dim authPartners As New TalentMembershipDatasetTableAdapters.tbl_authorized_partnersTableAdapter
        If authPartners.Get_CheckFor_B2C_Login(BusinessUnit).Rows.Count > 0 Then
            users.StampLastUpdatedDate_B2C(Now, BusinessUnit, Partner, username)
        Else
            users.StampLastUpdatedDate_B2B(Now, BusinessUnit, username)
        End If
    End Sub

    Public Function GetBackendUserDetails(ByRef username As String, ByVal password As String) As DataSet
        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay

        Dim myDs As New DataSet

        Try

            ' Set up the calls to add items to ticketing basket
            Dim _talentErrObj As New Talent.Common.ErrorObj
            Dim _talentCustomer As New Talent.Common.TalentCustomer
            Dim _deCustomer As New Talent.Common.DECustomer
            Dim _deSettings As New Talent.Common.DESettings


            ' Set the settings data entity. 
            _deSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            _deSettings.BusinessUnit = TalentCache.GetBusinessUnit()
            _deSettings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()

            ' Set the customer defaults
            If username.Contains("@") AndAlso username.Contains(".") Then
                _deCustomer.EmailAddress = username
            Else
                _deCustomer.CustomerNumber = username
            End If
            _deCustomer.Password = password
            _deCustomer.Source = "W"

            ' Invoke.
            Dim deCustV11 As New Talent.Common.DECustomerV11
            deCustV11.DECustomersV1.Add(_deCustomer)
            _talentCustomer.DeV11 = deCustV11
            _talentCustomer.Settings = _deSettings
            _talentErrObj = _talentCustomer.CustomerRetrieval

            ' Refresh the front end data on a successful return
            If Not _talentErrObj.HasError _
                AndAlso Not _talentCustomer.ResultDataSet Is Nothing _
                AndAlso _talentCustomer.ResultDataSet.Tables(0).Rows.Count > 0 Then

                Dim dr As DataRow
                ' Has there been an internal error
                dr = _talentCustomer.ResultDataSet.Tables(0).Rows(0)
                If dr("ErrorOccurred") <> GlobalConstants.ERRORFLAG AndAlso _talentCustomer.ResultDataSet.Tables.Count > 1 Then
                    myDs = _talentCustomer.ResultDataSet
                End If
            End If
        Catch ex As Exception
        End Try

        Utilities.TalentLogging.LoadTestLog("TalentMembershipProvider.vb", "GetBackendUserDetails", timeSpan)
        Return myDs

    End Function
    Public Function VerifyAndRetrieveAccountFromCRM(ByVal accountNo1 As String, ByVal accountNo2 As String,
                        ByVal surname As String, ByVal postcode As String, Optional ByVal forename As String = "") As DataSet
        Dim users As New DataSet
        Dim _talentErrObj As New Talent.Common.ErrorObj
        Dim _talentCustomer As New Talent.Common.TalentCustomer
        Dim _deCustomer As New Talent.Common.DECustomer
        Dim _deSettings As New Talent.Common.DESettings

        ' Set the settings data entity. 
        _deSettings.FrontEndConnectionString = ucr.FrontEndConnectionString
        _deSettings.BusinessUnit = ucr.BusinessUnit
        _deSettings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        _deSettings.Cacheing = False
        _deSettings.CacheTimeMinutes = 0

        _deCustomer.Source = "W"
        _deCustomer.ContactSurname = surname
        _deCustomer.ContactForename = forename
        _deCustomer.CompanySLNumber1 = accountNo1
        _deCustomer.CompanySLNumber2 = accountNo2
        _deCustomer.PostCode = postcode
        _deCustomer.RetreiveOnlyMode = False

        ' Invoke.
        Dim deCustV11 As New Talent.Common.DECustomerV11
        deCustV11.DECustomersV1.Add(_deCustomer)
        _talentCustomer.DeV11 = deCustV11
        _talentCustomer.Settings = _deSettings
        _talentErrObj = _talentCustomer.VerifyAndRetrieveCustomerDetails
        users = _talentCustomer.ResultDataSet
        Return users
    End Function

    Private Function IsValidGenericCustomer(ByVal username As String, ByVal password As String) As Boolean
        Dim isValid As Boolean = False
        If _agentProfile.IsAgent _
            AndAlso _agentProfile.Type = "2" _
            AndAlso HttpContext.Current.Session("passwordType") IsNot Nothing _
            AndAlso HttpContext.Current.Session("passwordType").ToString.ToUpper = "N" _
            AndAlso isGenericCustomerInPartnerUser() Then
            isValid = Talent.Common.Utilities.IsTalentRandomString(password)
        End If
        Return isValid
    End Function

    Private Function isGenericCustomerInPartnerUser() As Boolean
        Dim isExists As Boolean = False
        Dim talDataObjects As New Talent.Common.TalentDataObjects()
        talDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        Dim dtGenericUser As New DataTable
        dtGenericUser = talDataObjects.ProfileSettings.tblPartnerUser.GetByLoginId(GlobalConstants.GENERIC_CUSTOMER_NUMBER)
        If (dtGenericUser.Rows.Count > 0 AndAlso Talent.eCommerce.Utilities.CheckForDBNull_String(dtGenericUser.Rows(0)("LOGINID")) = GlobalConstants.GENERIC_CUSTOMER_NUMBER) Then
            isExists = True
        End If
        dtGenericUser = Nothing
        talDataObjects = Nothing
        Return isExists
    End Function
End Class
