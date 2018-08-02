Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services
Imports System.Web.Script.Services
Imports Talent.Common
Imports Talent.eCommerce
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Collections.Generic
Imports TalentBusinessLogic.ModelBuilders.CRM
Imports TalentBusinessLogic.Models

Public Class TalentBase01
    Inherits Base01

#Region "Class level fields"

    Private _conSql2005 As SqlConnection = Nothing

    Private _frontEndConnectionString As String = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
    Private _overrideNotPostBack As Boolean = False
    Private _moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Private _talentDefaults As Talent.eCommerce.ECommerceTalentDefaults.DefaultValues
    Private _pageDefaults As Talent.eCommerce.ECommercePageDefaults.DefaultValues
    Private _HTML As New System.Collections.Generic.Dictionary(Of String, String)
    Private _bctEntries As New List(Of Talent.eCommerce.ECommerceBctDefaults.BctEntry)
    Private _pageHeaderText As String
    Private _pageProductBrowseIntroText As String
    Private _pageProductBrowseHtml As String
    Private _TDataObjects As Talent.Common.TalentDataObjects
    Private _TalAdminDataObjects As Talent.Common.TalentAdminDataObjects
    Private _agentProfile As Agent

#End Region

#Region "Public Properties"

    Public BusinessUnit As String = TalentCache.GetBusinessUnit().ToString
    Dim pagename As String = Talent.eCommerce.Utilities.GetCurrentPageName
    Public PageCode As String = pagename.ToLower
    Public PartnerCode As String = TalentCache.GetPartner(HttpContext.Current.Profile, BusinessUnit).ToString
    Public ValidUser As Boolean = False
    Public Property ModuleDefaults() As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
        Get
            Return _moduleDefaults
        End Get
        Set(ByVal value As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues)
            _moduleDefaults = value
        End Set
    End Property

    Public Property TalentDefaults() As Talent.eCommerce.ECommerceTalentDefaults.DefaultValues
        Get
            Return _talentDefaults
        End Get
        Set(ByVal value As Talent.eCommerce.ECommerceTalentDefaults.DefaultValues)
            _talentDefaults = value
        End Set
    End Property

    Public Property PageDefaults() As Talent.eCommerce.ECommercePageDefaults.DefaultValues
        Get
            Return _pageDefaults
        End Get
        Set(ByVal value As Talent.eCommerce.ECommercePageDefaults.DefaultValues)
            _pageDefaults = value
        End Set
    End Property
    Public ReadOnly Property HTMLStrings() As System.Collections.Generic.Dictionary(Of String, String)
        Get
            Return _HTML
        End Get
    End Property
    Public Property BctEntries() As List(Of Talent.eCommerce.ECommerceBctDefaults.BctEntry)
        Get
            Return _bctEntries
        End Get
        Set(ByVal value As List(Of Talent.eCommerce.ECommerceBctDefaults.BctEntry))
            _bctEntries = value
        End Set
    End Property
    Public Property PageHeaderText() As String
        Get
            Return _pageHeaderText
        End Get
        Set(ByVal value As String)
            _pageHeaderText = value
        End Set
    End Property
    Public Property PageProductBrowseIntroText() As String
        Get
            Return _pageProductBrowseIntroText
        End Get
        Set(ByVal value As String)
            _pageProductBrowseIntroText = value
        End Set
    End Property
    Public Property PageProductBrowseHtml() As String
        Get
            Return _pageProductBrowseHtml
        End Get
        Set(ByVal value As String)
            _pageProductBrowseHtml = value
        End Set
    End Property
    Public Property TDataObjects() As Talent.Common.TalentDataObjects
        Get
            Return _TDataObjects
        End Get
        Set(ByVal value As Talent.Common.TalentDataObjects)
            _TDataObjects = value
        End Set
    End Property
    Public Property TalAdminDataObjects() As Talent.Common.TalentAdminDataObjects
        Get
            Return _TalAdminDataObjects
        End Get
        Set(ByVal value As Talent.Common.TalentAdminDataObjects)
            _TalAdminDataObjects = value
        End Set
    End Property
    Public Property AgentProfile() As Agent
        Get
            Return _agentProfile
        End Get
        Set(ByVal value As Agent)
            _agentProfile = value
        End Set
    End Property

#End Region

#Region "Class Constructor"

    Sub New()
        If _TDataObjects Is Nothing Then
            _TDataObjects = New Talent.Common.TalentDataObjects()
            _TDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject(True)
        End If
        If _TalAdminDataObjects Is Nothing Then
            _TalAdminDataObjects = New Talent.Common.TalentAdminDataObjects()
            _TalAdminDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject(True)
            _TalAdminDataObjects.Settings.DestinationDatabase = GlobalConstants.TALENTADMINDESTINATIONDATABASE
            _TalAdminDataObjects.Settings.BackOfficeConnectionString = TDataObjects.AppVariableSettings.TblDatabaseVersion.TalentAdminDatabaseConnectionString()
        End If
        If _agentProfile Is Nothing Then
            _agentProfile = New Agent
        End If
    End Sub

#End Region

#Region "Page Events"

    Private Sub Page_PreInit_Base(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit

        If BusinessUnit = String.Empty Then
            Server.Transfer("~/PagesPublic/Error/InvalidURL.htm")
        End If

        Dim myModuleDefaults As New Talent.eCommerce.ECommerceModuleDefaults
        Dim ecomTalentDefaults As New Talent.eCommerce.ECommerceTalentDefaults
        Dim myPageDefaults As New Talent.eCommerce.ECommercePageDefaults
        PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, BusinessUnit).ToString
        ModuleDefaults() = myModuleDefaults.GetDefaults()
        TalentDefaults() = ecomTalentDefaults.GetDefaults()
        PageDefaults() = myPageDefaults.GetDefaults
        CheckForValidSession()

        If String.IsNullOrEmpty(Talent.Common.Settings.PriceList) Then Talent.Common.Settings.PriceList = ModuleDefaults.PriceList

        '------------------------------------------------------------------------------------------
        '   Check out if we have the correct master page
        '
        Dim strMasterPage As String = Talent.eCommerce.Templates.GetPageTemplate(BusinessUnit, PartnerCode)
        If strMasterPage.Length > 0 Then
            Select Case strMasterPage
                Case Is = "HopewiserContent.master", "DatePicker.master", "Blank.master", "QASContent.master", "Reveal.master"
                    Me.MasterPageFile = "~/MasterPages/Shared/" & strMasterPage
                Case Else
                    Dim masterPagePath As New StringBuilder
                    If ModuleDefaults.WholeSiteIsInAgentMode Then
                        masterPagePath.Append("~/MasterPages/BoxOffice")
                    Else
                        masterPagePath.Append("~/MasterPages/PublicWebSales/")
                        masterPagePath.Append(ModuleDefaults.MasterPageFolder)
                    End If
                    Me.MasterPageFile = masterPagePath.ToString() & "/" & strMasterPage
                    If Not strMasterPage.Contains("Modal") Then
                        Me.Page.Master.MasterPageFile = masterPagePath.ToString() & "/Site.master"
                    End If
            End Select
        End If
        LoginToProductBrowse()
        If Not Page.IsPostBack Then
            Check_QueryString_Secure()
            If ModuleDefaults.AlertsEnabled Then
                TEBUtilities.ProcessUserAlertRedirect()
            End If
            checkForCampaignCodeQueryString()
            If ModuleDefaults.BctInUse Then
                GetBct()
            End If
        End If

        GetHTMLStringsFromDB(sender)
        SetPageTheme()
    End Sub

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        CheckForPageRestrictions()
        IsBasketAlreadyPaid()
        BasketContainsItemLinkedToTemplate(CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketItems, ModuleDefaults.CacheDependencyPath, CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
        DeleteExternalPayDirtyBasketHeader()
        If Not Page.IsPostBack Then
            InsertPrintCSS()
        End If
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.Page.Form.Attributes.Add("class", "form-" & LCase(Talent.eCommerce.Utilities.GetCurrentPageName.Split(".")(0)))
        If Not Page.IsPostBack Then
            CheckUserAccountIsValid()
            If ModuleDefaults.MetaTagsInUse Then
                CreateMetaTags()
            End If

            If PageCode.ToLower.Equals("tradeplacelogin.aspx") Then
                doAutoLogin()
            ElseIf PageCode.ToLower.Equals("sapocilogin.aspx") Then
                SapOciAutoLogin()
            Else
                If ModuleDefaults.AllowAutoLogin AndAlso (Session("Agent") Is Nothing OrElse String.IsNullOrEmpty(Session("Agent").ToString)) Then
                    doAutoLogin()
                End If
            End If

        End If
    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Logon function without redirect (code from original doAutoLogon) 
    ''' </summary>
    ''' <param name="forceLogoutFirst"></param>
    ''' <param name="encType (set to 1)"></param>
    ''' <param name="customerNumber"></param>
    ''' <param name="password"></param>
    ''' <param name="passwordType(pass N to login without password)"></param>
    ''' <remarks></remarks>
    Public Sub doAutoLogin(ByVal forceLogoutFirst As Boolean, ByVal encType As String, ByVal customerNumber As String, ByVal password As String, ByVal passwordType As String)

        If AgentProfile.IsAgent() Then
            Session.Item("passwordType") = "N"
        Else
            Session.Item("passwordType") = passwordType
        End If

        Select Case encType
            Case Is = "1" 'TripleDES
                Dim key As String = _moduleDefaults.NOISE_ENCRYPTION_KEY
                customerNumber = Talent.Common.Utilities.TripleDESDecode(customerNumber, key)
                password = Talent.Common.Utilities.TripleDESDecode(password, key)
                ValidUser = Talent.eCommerce.Utilities.loginUser(customerNumber, password)
                If ValidUser Then
                    HttpContext.Current.Profile.Initialize(customerNumber, True)
                    Dim tempProfile As ProfileCommon = CType(HttpContext.Current.Profile, ProfileCommon).GetProfile(customerNumber)
                    If tempProfile IsNot Nothing Then
                        CType(HttpContext.Current.Profile, TalentProfile).User = tempProfile.User
                        CType(HttpContext.Current.Profile, TalentProfile).Basket = tempProfile.Basket
                        CType(HttpContext.Current.Profile, TalentProfile).PartnerInfo = tempProfile.PartnerInfo
                    End If
                End If
            Case Is = "2"
            Case Is = "3"
            Case Else
        End Select

    End Sub



    Public Sub doAutoLogin(Optional ByVal forceLogoutFirst As Boolean = False)

        If PageCode.ToLower.Equals("tradeplacelogin.aspx") Then
            TradePlaceAutoLogin()
        Else
            Dim justLoggedOut As Boolean = False
            If forceLogoutFirst Then
                If Not HttpContext.Current.Profile.IsAnonymous Then
                    FormsAuthentication.SignOut()
                    justLoggedOut = True

                    ' Clear existing basket items after new login. Required now the GetBasket stored proc
                    ' does not create a new basket and reassigns the anonymous one.
                    Dim tGatewayFunctions As New TicketingGatewayFunctions
                    tGatewayFunctions.Basket_ClearBasket(False)
                    CType(HttpContext.Current.Profile, TalentProfile).Basket.EmptyBasket(True)
                End If
            End If

            If Not User.Identity.IsAuthenticated OrElse justLoggedOut Then

                'Auto login by querystring paramters
                If Not PageCode.ToLower.Equals("login.aspx") And Request.QueryString("t") IsNot Nothing And Request.QueryString("l") IsNot Nothing And Request.QueryString("p") IsNot Nothing And Request.QueryString("y") IsNot Nothing Then

                    Dim encType, usr, pass, passwordType As String

                    encType = Request.QueryString("t")
                    usr = Request.QueryString("l")
                    pass = Request.QueryString("p")
                    passwordType = Request.QueryString("Y")

                    doAutoLogin(forceLogoutFirst, encType, usr, pass, passwordType)

                    If Not ValidUser And Not PageCode.ToLower.Equals("ticketinggateway.aspx") Then
                        Response.Redirect("~/PagesPublic/Login/login.aspx?ReturnUrl=" & HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.PathAndQuery))
                    End If

                End If

                'Auto login by encrypted cookie         
                Dim justLoggedOff As Boolean = False
                Dim justLoggedOff2 As Boolean = False
                If Session("justLoggedOff2") IsNot Nothing Then
                    justLoggedOff2 = CType(Session("justLoggedOff2"), Boolean)
                    Session("justLoggedOff2") = Nothing
                    justLoggedOff = True
                End If
                If Session("justLoggedOff") IsNot Nothing Then
                    justLoggedOff = CType(Session("justLoggedOff"), Boolean)
                    Session("justLoggedOff") = Nothing
                    Session("justLoggedOff2") = "True"
                End If

                Dim justLoggedOn As Boolean = False
                If Session("justLoggedOn") IsNot Nothing Then
                    justLoggedOn = CType(Session("justLoggedOn"), Boolean)
                    Session("justLoggedOn") = Nothing
                End If

                If Request.Cookies.Item("USERNAME") IsNot Nothing AndAlso Not Request.Cookies.Item("USERNAME").Value = String.Empty _
                    And Request.Cookies.Item("HASH1") IsNot Nothing AndAlso Not Request.Cookies.Item("HASH1").Value = String.Empty _
                    And Request.Cookies.Item("HASH2") IsNot Nothing AndAlso Not Request.Cookies.Item("HASH2").Value = String.Empty _
                    And Not justLoggedOn And Not justLoggedOff Then

                    Dim usernameCookie As String = Request.Cookies.Item("USERNAME").Value
                    Dim encryptedUsernameCookie As String = Request.Cookies.Item("HASH1").Value
                    Dim encryptedPasswordCookie As String = Request.Cookies.Item("HASH2").Value

                    ' If in a non-password-encrypted environment then plain text password is stored in cookie
                    If Not ModuleDefaults.UseEncryptedPassword Then

                        'Compare hashed password retrieved from back-end with hashed password passed in cookie
                        'Retrieve back-end password
                        Dim password As String = retrievePassword(usernameCookie)

                        'Translate to Customer Number if email provided
                        If usernameCookie.Contains("@") And ModuleDefaults.UseLoginLookup Then
                            usernameCookie = translateEmail(usernameCookie)
                        End If

                        Dim secretKey As String = ModuleDefaults.SingleSignOnSecretKey

                        Dim compareHashPassword As String = Talent.eCommerce.Utilities.aMD5Hash(Talent.eCommerce.Utilities.aMD5Hash(password) & secretKey)

                        If compareHashPassword = encryptedPasswordCookie Then
                            ValidUser = Talent.eCommerce.Utilities.loginUser(usernameCookie, password)
                        End If
                    Else
                        ' If in a password-encrypted environment then encrypted password string is stored in cookie


                        'Retrieve the encrypted password string from back-end
                        Dim encPasswordFromBackend As String = retrievePassword(usernameCookie)

                        'Translate to Customer Number if email provided
                        If usernameCookie.Contains("@") And ModuleDefaults.UseLoginLookup Then
                            usernameCookie = translateEmail(usernameCookie)
                        End If

                        ' Get the encrpted password string from cookie
                        Dim secretKey As String = ModuleDefaults.SingleSignOnSecretKey
                        ' Re-create cookie string using the hashed password returned from the backend
                        Dim compareHashPassword As String = Talent.eCommerce.Utilities.aMD5Hash(Talent.eCommerce.Utilities.aMD5Hash(encPasswordFromBackend) & secretKey)

                        ' Compare the two cookie strings
                        If compareHashPassword = encryptedPasswordCookie Then
                            HttpContext.Current.Session("PasswordHashed") = compareHashPassword
                            ValidUser = Talent.eCommerce.Utilities.loginUser(usernameCookie, encPasswordFromBackend)
                        End If

                    End If
                End If

                Dim rememberMeCookie As String = getRememberMeCookieValue()
                If rememberMeCookie.Length > 0 Then
                    Session.Item("passwordType") = "N"
                    ValidUser = Talent.eCommerce.Utilities.loginUser(rememberMeCookie, String.Empty)
                    Dim redirectUrl As String = ModuleDefaults.RedirectAfterAutoLogin
                    If Request.Url.AbsoluteUri IsNot Nothing Then redirectUrl = Request.Url.AbsoluteUri
                    If ValidUser Then Response.Redirect(redirectUrl)
                End If

            End If

        End If 'TradePlaceLogin.aspx if ends


    End Sub

#End Region

#Region "Protected Methods"

    ''' Get the software Version 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function GetVersion() As String
        Return Talent.eCommerce.Utilities.getVersionQueryString
    End Function

    Protected Sub AddHTMLString(ByVal HtmlName As String, ByVal HTML As String)
        If _HTML.ContainsKey(HtmlName) Then
            Talent.eCommerce.Logging.WriteLog("", "Duplicate HTML", "Duplicate HTML entries - " & HtmlName, "")
        Else
            _HTML.Add(HtmlName, HTML)
        End If
    End Sub

    Protected Sub EmptyHTMLString()
        _HTML.Clear()
    End Sub

    Protected Sub CheckUserAccountIsValid()
        ModuleDefaults = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults

        If ModuleDefaults.ValidateUserProfilesWhenLoggedIn AndAlso _
           Not HttpContext.Current.Profile.IsAnonymous _
               AndAlso Not UCase(Talent.eCommerce.Utilities.GetCurrentPageName).Contains("UPDATEPROFILE") _
               AndAlso Not UCase(Talent.eCommerce.Utilities.GetCurrentPageName).Contains("VALIDATEPASSWORD") _
               AndAlso Not UCase(Talent.eCommerce.Utilities.GetCurrentPageName).Contains("LOGGEDOUT") _
               AndAlso Not UCase(Talent.eCommerce.Utilities.GetCurrentPageName).Contains("CLEARCACHE") _
               AndAlso Not LCase(HttpContext.Current.CurrentHandler.ToString).Contains("pageslogin_profile_alerts_aspx") Then

            Dim myProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            Dim myUser As TalentProfileUserDetails = myProfile.User.Details
            Dim myPartner As TalentProfilePartnerDetails = myProfile.PartnerInfo.Details
            Dim myAddress As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, myProfile.User.Addresses)
            Dim redirectToUpdate As Boolean = False
            Dim errorType As String = ""

            If UCase(myUser.Email) = UCase(ModuleDefaults.DefaultTicketingEmailAddress) Or myUser.Email.Trim.Equals("") Then
                Response.Redirect("~/PagesLogin/Profile/UpdateProfile.aspx?reason=username")
            Else
                Dim myUcr As New Talent.Common.UserControlResource
                With myUcr
                    .BusinessUnit = TalentCache.GetBusinessUnit
                    .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                    .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    Select Case ModuleDefaults.RegistrationTemplate
                        Case "2"
                            .KeyCode = "RegistrationForm2.ascx"
                        Case Else
                            .KeyCode = "RegistrationForm.ascx"
                    End Select
                End With


                Select Case ModuleDefaults.RegistrationTemplate
                    Case "2"
                        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("accountNumber2EnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myPartner.Account_No_2) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("accountNumber3EnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myPartner.Account_No_3) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("accountNumber4EnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myPartner.Account_No_4) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("accountNumber5EnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myPartner.Account_No_5) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("accountNumberEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myPartner.Account_No_1) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("address1EnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Address_Line_1) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("address2EnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Address_Line_2) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("address3EnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Address_Line_3) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("address4EnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Address_Line_4) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("address5EnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Address_Line_5) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("emailEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Fax_Number) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("faxEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Fax_Number) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("forenameEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Forename) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("phoneEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Telephone_Number) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("positionEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Position) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("postcodeEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Post_Code) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("surnameEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Surname) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("vatNumberEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Fax_Number) Then
                            redirectToUpdate = True
                        End If
                    Case Else
                        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("buildingEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Address_Line_1) _
                                AndAlso ModuleDefaults.AddressLine1RowVisible Then
                            redirectToUpdate = True
                            errorType = "1"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("cityEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Address_Line_4) Then
                            redirectToUpdate = True
                            errorType = "2"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("countyEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Address_Line_5) Then
                            redirectToUpdate = True
                            errorType = "3"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("dobEnableRFV")) _
                            AndAlso (TCUtilities.CheckForDBNull_DateTime(myUser.DOB) = Date.MinValue _
                                OrElse myUser.DOB.ToString("dd/MM/yyyy") = ("01/01/1900")) Then
                            redirectToUpdate = True
                            errorType = "4"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("emailEnableRFV")) _
                            AndAlso UCase(myUser.Email) = UCase(ModuleDefaults.DefaultTicketingEmailAddress) Then
                            redirectToUpdate = True
                            errorType = "5"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("faxEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Fax_Number) Then
                            redirectToUpdate = True
                            errorType = "6"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("forenameEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Forename) Then
                            redirectToUpdate = True
                            errorType = "7"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("initialsEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Initials) Then
                            redirectToUpdate = True
                            errorType = "8"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("loginidEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.LoginID) Then
                            redirectToUpdate = True
                            errorType = "9"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("otherEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Other_Number) Then
                            redirectToUpdate = True
                            errorType = "10"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("phoneEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Telephone_Number) Then
                            redirectToUpdate = True
                            errorType = "11"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("positionEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Position) Then
                            redirectToUpdate = True
                            errorType = "12"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("postcodeEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Post_Code) Then
                            redirectToUpdate = True
                            errorType = "13"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("referenceEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Reference) Then
                            redirectToUpdate = True
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("salutationEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Salutation) Then
                            redirectToUpdate = True
                            errorType = "14"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("sequenceEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Sequence) Then
                            redirectToUpdate = True
                            errorType = "15"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("streetEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Address_Line_2) Then
                            redirectToUpdate = True
                            errorType = "16"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("surnameEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Surname) Then
                            redirectToUpdate = True
                            errorType = "17"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("townEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Address_Line_3) Then
                            redirectToUpdate = True
                            errorType = "18"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("typeEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Type) Then
                            redirectToUpdate = True
                            errorType = "19"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("workEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Work_Number) Then
                            redirectToUpdate = True
                            errorType = "20"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("titleEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Title) Then
                            redirectToUpdate = True
                            errorType = "21"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("countryEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myAddress.Country) Then
                            redirectToUpdate = True
                            errorType = "22"
                        ElseIf TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(myUcr.Attribute("mobileEnableRFV")) _
                            AndAlso String.IsNullOrEmpty(myUser.Mobile_Number) Then
                            redirectToUpdate = True
                            errorType = "23"
                        End If
                End Select

                If redirectToUpdate Then

                    Dim redirectUrl As String = "~/PagesLogin/Profile/updateProfile.aspx?reason=accounterror&type=" & errorType
                    Try
                        Session("RedirectUrl") = Request.RawUrl
                    Catch ex As Exception
                    End Try
                    Response.Redirect(redirectUrl)
                End If

            End If

        End If


        If ModuleDefaults.CheckAddressIsValidOnLogin AndAlso _
           Not HttpContext.Current.Profile.IsAnonymous _
               AndAlso Not UCase(Talent.eCommerce.Utilities.GetCurrentPageName).Contains("UPDATEPROFILE") _
               AndAlso Not LCase(HttpContext.Current.CurrentHandler.ToString).Contains("pageslogin_profile_alerts_aspx") Then

            Dim myProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            Dim myUser As TalentProfileUserDetails = myProfile.User.Details
            Dim myPartner As TalentProfilePartnerDetails = myProfile.PartnerInfo.Details
            Dim myAddress As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, myProfile.User.Addresses)
            Dim redirectToUpdate As Boolean = False
            Dim errorType As String = ""

            ' Check Countryis Turkey
            Dim licCountries As ListItemCollection = TalentCache.GetDropDownControlText(Talent.eCommerce.Utilities.GetCurrentLanguageForDDLPopulation, "MYACCOUNT", "COUNTRY")
            Dim tr As ListItem
            Dim isTurkey As Boolean = False
            Try
                tr = licCountries.FindByValue("TR ")
                If tr Is Nothing Then
                    licCountries.FindByValue("TR")
                End If
                If Not tr Is Nothing AndAlso myAddress.Country.ToUpper = tr.Text.ToUpper Then
                    isTurkey = True
                End If

            Catch ex As Exception

            End Try


            Select Case ModuleDefaults.AddressFormat
                Case Is = "1"
                    ' galatasaray format.
                    ' check street is populated
                    ' Check details on AddressFormat1 are OK (Galatasaray)
                    If isTurkey Then
                        Dim valid As Boolean = True

                        If myAddress.Address_Line_2 = String.Empty OrElse
                            myAddress.Address_Line_3 = String.Empty OrElse
                            myAddress.Address_Line_3 = String.Empty OrElse
                            myAddress.Address_Line_4 = String.Empty OrElse
                            myAddress.Address_Line_5 = String.Empty OrElse
                            myAddress.Country = String.Empty Then

                            redirectToUpdate = True

                        Else
                            ' check town/city is a valid value (this isa free text with an auto complete and it must match one of the autocompletes
                            Dim tDataObjects = New TalentDataObjects()
                            Dim settings As DESettings = New DESettings()

                            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                            settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
                            settings.BusinessUnit = TalentCache.GetBusinessUnit
                            tDataObjects.Settings = settings

                            Dim strTArray As String() = myAddress.Address_Line_5.Split(",")
                            Dim strTown As String = String.Empty
                            If strTArray.Length > 0 Then
                                strTown = strTArray(0)
                            End If
                            Dim strCity As String = String.Empty
                            If strTArray.Length > 1 Then
                                strCity = strTArray(1)
                            End If

                            Dim dtTownAndCities As DataTable = tDataObjects.ProfileSettings.TblAddressFormat1Data.GetDistinctTownCity()

                            If dtTownAndCities IsNot Nothing AndAlso dtTownAndCities.Rows.Count > 0 Then
                                Dim matchedRows() As DataRow = Nothing
                                matchedRows = dtTownAndCities.Select("TOWN LIKE '%" & strTown & "%'" & " and CITY LIKE '%" & strCity & "%'")
                                If matchedRows.Length > 0 Then

                                Else
                                    redirectToUpdate = True
                                End If
                            End If
                        End If
                    End If

            End Select

            If redirectToUpdate Then

                Dim redirectUrl As String = "~/PagesLogin/Profile/updateProfile.aspx?reason=accounterror&type=Address"
                Try
                    Session("RedirectUrl") = Request.RawUrl
                Catch ex As Exception
                End Try
                Response.Redirect(redirectUrl)
            End If
        End If



    End Sub

    Protected Sub InsertPrintCSS()
        Dim pageCSSPrintCacheKey As String = BusinessUnit & PageCode & "_CSSPrint"
        Dim printCss As Boolean = False
        If Not Talent.Common.TalentThreadSafe.ItemIsInCache(pageCSSPrintCacheKey) Then
            PopulatePageSettingsCache()
        End If

        If Not String.IsNullOrEmpty(HttpContext.Current.Cache(pageCSSPrintCacheKey)) Then
            printCss = CBool(HttpContext.Current.Cache(pageCSSPrintCacheKey))
        End If

        If printCss Then
            Dim link As New HtmlGenericControl("link")

            Dim linkUrl As String = "../../App_Themes/Print/" & ModuleDefaults.Theme & ".css"

            link.Attributes.Add("href", linkUrl)
            link.Attributes.Add("type", "text/css")
            link.Attributes.Add("rel", "stylesheet")
            link.Attributes.Add("media", "print")

            Me.Page.Header.Controls.Add(link)
        End If
    End Sub

    Protected Sub SetPageTheme()

        'Retrieve the theme from the group level
        Dim groupTheme As String = String.Empty
        Dim pageName As String = Talent.eCommerce.Utilities.GetCurrentPageName

        Dim groupDetails As Talent.eCommerce.GroupLevelDetails
        Dim dets As Talent.eCommerce.GroupLevelDetails.GroupDetails
        groupDetails = New Talent.eCommerce.GroupLevelDetails

        Select Case PageDefaults.Page_Type

            Case Is = "PRODUCT", "BROWSE"

                If groupTheme = "" And (pageName = "browse10.aspx" Or pageName = "product.aspx") Then
                    If pageName = "product.aspx" Then
                        pageName = "browse10.aspx"
                    End If
                    dets = groupDetails.GetGroupLevelDetails(pageName)
                    If dets.GroupTheme = "" Then
                        pageName = "browse09.aspx"
                    Else
                        groupTheme = dets.GroupTheme
                    End If
                End If

                If groupTheme = "" And pageName = "browse09.aspx" Then
                    dets = groupDetails.GetGroupLevelDetails(pageName)
                    If dets.GroupTheme = "" Then
                        pageName = "browse08.aspx"
                    Else
                        groupTheme = dets.GroupTheme
                    End If
                End If

                If groupTheme = "" And pageName = "browse08.aspx" Then
                    dets = groupDetails.GetGroupLevelDetails(pageName)
                    If dets.GroupTheme = "" Then
                        pageName = "browse07.aspx"
                    Else
                        groupTheme = dets.GroupTheme
                    End If
                End If

                If groupTheme = "" And pageName = "browse07.aspx" Then
                    dets = groupDetails.GetGroupLevelDetails(pageName)
                    If dets.GroupTheme = "" Then
                        pageName = "browse06.aspx"
                    Else
                        groupTheme = dets.GroupTheme
                    End If
                End If

                If groupTheme = "" And pageName = "browse06.aspx" Then
                    dets = groupDetails.GetGroupLevelDetails(pageName)
                    If dets.GroupTheme = "" Then
                        pageName = "browse05.aspx"
                    Else
                        groupTheme = dets.GroupTheme
                    End If
                End If

                If groupTheme = "" And pageName = "browse05.aspx" Then
                    dets = groupDetails.GetGroupLevelDetails(pageName)
                    If dets.GroupTheme = "" Then
                        pageName = "browse04.aspx"
                    Else
                        groupTheme = dets.GroupTheme
                    End If
                End If

                If groupTheme = "" And pageName = "browse04.aspx" Then
                    dets = groupDetails.GetGroupLevelDetails(pageName)
                    If dets.GroupTheme = "" Then
                        pageName = "browse03.aspx"
                    Else
                        groupTheme = dets.GroupTheme
                    End If
                End If

                If groupTheme = "" And pageName = "browse03.aspx" Then
                    dets = groupDetails.GetGroupLevelDetails(pageName)
                    If dets.GroupTheme = "" Then
                        pageName = "browse02.aspx"
                    Else
                        groupTheme = dets.GroupTheme
                    End If
                End If

                If groupTheme = "" And pageName = "browse02.aspx" Then
                    dets = groupDetails.GetGroupLevelDetails(pageName)
                    If dets.GroupTheme = "" Then
                        pageName = "browse01.aspx"
                    Else
                        groupTheme = dets.GroupTheme
                    End If
                End If
        End Select

        If groupTheme <> "" And IO.Directory.Exists(Me.MapPath("~/App_Themes/" & groupTheme)) Then
            Me.Theme = groupTheme
        Else
            If IO.Directory.Exists(Me.MapPath("~/App_Themes/" & ModuleDefaults.Theme)) Then
                Me.Theme = ModuleDefaults.Theme
            End If
        End If

        If Me.MasterPageFile.Contains("Reveal.master") Then
            Me.Theme = String.Empty
        End If
    End Sub

    Protected Sub LoginToProductBrowse()
        If ModuleDefaults.WholeSiteIsInAgentMode AndAlso Not AgentProfile.IsAgent Then
            'Force agent login if the user is trying to access any area of the site
            ProcessAgentModeForWholeSite()
        ElseIf AgentProfile.IsAgent Then
            'agent logged in can we process the page without authentication
            ProcessAgentModeForSession()
        ElseIf Request.Url.AbsolutePath.ToUpper.IndexOf("/PAGESAGENT/") >= 0 Then
            'not an agent trying to access agent page can we allow
            ProcessAgentPageForSession()
        Else
            'not an agent and not an agent page
            'normal user can we process the page without authentication
            ProcessPageForAuthentication()
        End If
    End Sub

    Protected Sub GetBct()
        Dim bctDefaults As New Talent.eCommerce.ECommerceBctDefaults
        BctEntries() = bctDefaults.GetBctDefaults
    End Sub

#End Region

#Region "Private Methods"

    Private Sub GetHTMLStringsFromDB(ByVal sender As Object)
        If ModuleDefaults.HtmlPerPage Then
            If PageDefaults.Html_In_Use Then
                Dim cacheKey As String = String.Empty
                Dim queryString As String = HttpContext.Current.Request.QueryString.ToString
                Dim htmlTypeUPPER As String = ModuleDefaults.HtmlPerPageType.ToUpper
                Dim htmlEntries As New TalentApplicationVariablesTableAdapters.tbl_page_htmlTableAdapter
                Dim dt As Data.DataTable = Nothing

                '---------------------------------------------------------------------------------
                ' Performance mods - BF 25/06/08
                ' - Seperate SQL Queries depending on whether from FILE or DB. If from FILE then
                '   don't return the ntext fields HTML1,HTML2,HTML3 as these have a large overhead
                ' - Check the cache and cache the item at each point it accesses the data base. 
                '   It is done this way so that the datatable isn't stored for each different query
                '   string. Instead a lot of empty datatables are stored in cache when specific 
                '   data isn't found.
                '---------------------------------------------------------------------------------
                '        
                Dim tDataObjects As New TalentDataObjects
                tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()

                Select Case htmlTypeUPPER
                    Case Is = "FILE"
                        dt = tDataObjects.PageSettings.TblPageHtml.GetDataFILEBy_BU_Partner_PageName(BusinessUnit, pagename)
                    Case Is = "DB"
                        dt = tDataObjects.PageSettings.TblPageHtml.GetDataBy_BU_Partner_PageName(BusinessUnit, pagename)
                End Select

                Dim resultTable As New DataTable
                Dim view1 As New DataView(dt.DefaultView.Table)
                Dim view2 As New DataView(dt.DefaultView.Table)
                Dim view3 As New DataView(dt.DefaultView.Table)
                Try
                    view1.RowFilter = "[PAGE_QUERYSTRING] = '" & TEBUtilities.ReplaceSingleQuote(queryString) & "'"
                Catch ex As Exception
                    Dim errMessage As String = String.Empty
                    For Each column As DataColumn In dt.Columns
                        errMessage = errMessage & column.ColumnName & ";"
                    Next
                    Talent.eCommerce.LogWriter.WriteCacheIssueToLog("CACHE-001", errMessage, ex.Message, ex)
                End Try
                If view1.ToTable().Rows.Count > 0 Then
                    If Not String.IsNullOrEmpty(TEBUtilities.CheckForDBNull_String(view1.ToTable().Rows(0).Item("PAGE_QUERYSTRING"))) Then
                        resultTable = view1.ToTable()
                    Else
                        view2.RowFilter = "[PAGE_QUERYSTRING] IS NULL OR [PAGE_QUERYSTRING] = 'Null' OR [PAGE_QUERYSTRING] = ''"
                        If view2.ToTable().Rows.Count > 0 Then
                            resultTable = view2.ToTable()
                        End If
                    End If
                Else
                    matchQueryStringValues(view2)
                    If view2.ToTable().Rows.Count > 0 Then
                        If Not String.IsNullOrEmpty(TEBUtilities.CheckForDBNull_String(view2.ToTable().Rows(0).Item("PAGE_QUERYSTRING"))) Then
                            resultTable = view2.ToTable()
                        Else
                            view3.RowFilter = "[PAGE_QUERYSTRING] IS NULL OR [PAGE_QUERYSTRING] = 'Null' OR [PAGE_QUERYSTRING] = ''"
                            If view3.ToTable().Rows.Count > 0 Then
                                resultTable = view3.ToTable()
                            End If
                        End If
                    Else
                        view3.RowFilter = "[PAGE_QUERYSTRING] IS NULL OR [PAGE_QUERYSTRING] = 'Null' OR [PAGE_QUERYSTRING] = ''"
                        If view3.ToTable().Rows.Count > 0 Then
                            resultTable = view3.ToTable()
                        End If
                    End If
                End If

                If resultTable.Rows.Count > 0 Then
                    EmptyHTMLString()
                    PopulateTheHTMLStrings(resultTable, ModuleDefaults.HtmlPerPageType)
                End If
            End If
        End If

        Select Case PageDefaults.Page_Type
            Case Is = "STANDARD"
                Dim wfrPage As New WebFormResource
                With wfrPage
                    .BusinessUnit = TalentCache.GetBusinessUnit()
                    .PageCode = String.Empty
                    .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    .KeyCode = pagename
                    .PageCode = pagename
                End With
                Page.Title = wfrPage.Title(Talent.Common.Utilities.GetDefaultLanguage)
                If PageDefaults.Show_Page_Header Then
                    PageHeaderText = wfrPage.HeaderText(Talent.Common.Utilities.GetDefaultLanguage)
                End If
                PageProductBrowseIntroText = String.Empty
            Case Is = "PRODUCT"
                Dim prodCode As String = String.Empty
                If Not Request("product") Is Nothing Then
                    prodCode = Request("product")
                End If

                Dim productData As New TalentProductInformationTableAdapters.tbl_productTableAdapter
                Dim dt As DataTable = productData.GetDataByProduct_Code(prodCode)
                If dt.Rows.Count > 0 Then
                    If PageDefaults.Show_Page_Header Then
                        PageHeaderText = dt.Rows(0)("PRODUCT_DESCRIPTION_1")
                    End If

                    Dim wfrPage As New WebFormResource
                    With wfrPage
                        .BusinessUnit = BusinessUnit
                        .PageCode = String.Empty
                        .PartnerCode = PartnerCode
                        .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                        .KeyCode = pagename
                        .PageCode = pagename
                    End With
                    Page.Title = wfrPage.Title(Talent.Common.Utilities.GetDefaultLanguage) & dt.Rows(0)("PRODUCT_DESCRIPTION_1")
                End If

            Case Is = "BROWSE"
                Dim groupDetails As Talent.eCommerce.GroupLevelDetails
                Dim dets As Talent.eCommerce.GroupLevelDetails.GroupDetails
                groupDetails = New Talent.eCommerce.GroupLevelDetails
                dets = groupDetails.GetGroupLevelDetails()
                If PageDefaults.Show_Page_Header Then
                    PageHeaderText = dets.GroupDescription1
                End If
                PageProductBrowseIntroText = dets.GroupDescription2
                '-----------------------------------------------------------------------------
                ' Check whether HTML should be displayed and where it should come from (DB/FILE)
                ' (this is set on the top level group)
                '-----------------------------------------------------------------------------
                Dim groupDefaults As New TalentGroupInformationTableAdapters.tbl_group_level_01TableAdapter
                Dim dtGroupDefaults As New DataTable
                Dim group01 As String = String.Empty
                If Not String.IsNullOrEmpty(HttpContext.Current.Request("group1")) Then
                    group01 = HttpContext.Current.Request("group1")
                End If

                dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Group(BusinessUnit, PartnerCode, group01)
                If dtGroupDefaults.Rows.Count = 0 Then
                    dtGroupDefaults = groupDefaults.GetDataByBU_Partner_Group(BusinessUnit, Talent.Common.Utilities.GetAllString, group01)
                End If
                '-----------
                ' Show HTML?
                '-----------
                If dtGroupDefaults.Rows.Count > 0 AndAlso TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtGroupDefaults.Rows(0)("GROUP_L01_HTML_GROUP")) Then
                    Dim htmlType As String = dtGroupDefaults.Rows(0)("GROUP_L01_HTML_GROUP_TYPE").ToString
                    Select Case htmlType
                        Case Is = "FILE"
                            '-----------------
                            ' Build group path
                            '-----------------
                            Dim group1 As String = String.Empty, group2 As String = String.Empty, group3 As String = String.Empty, group4 As String = String.Empty, group5 As String = String.Empty
                            Dim group6 As String = String.Empty, group7 As String = String.Empty, group8 As String = String.Empty, group9 As String = String.Empty, group10 As String = String.Empty
                            Dim arrGroup(10) As String
                            arrGroup(0) = HttpContext.Current.Request("group1")
                            arrGroup(1) = HttpContext.Current.Request("group2")
                            arrGroup(2) = HttpContext.Current.Request("group3")
                            arrGroup(3) = HttpContext.Current.Request("group4")
                            arrGroup(4) = HttpContext.Current.Request("group5")
                            arrGroup(5) = HttpContext.Current.Request("group6")
                            arrGroup(6) = HttpContext.Current.Request("group7")
                            arrGroup(7) = HttpContext.Current.Request("group8")
                            arrGroup(8) = HttpContext.Current.Request("group9")
                            arrGroup(9) = HttpContext.Current.Request("group10")

                            Dim count As Integer = 0
                            Dim groupFolderPath As String = String.Empty
                            Dim sbGroupFolders As New StringBuilder
                            Do While count < 10
                                If Not arrGroup(count) Is Nothing AndAlso arrGroup(count) <> "*EMPTY" Then
                                    sbGroupFolders.Append("/").Append(arrGroup(count).Trim)
                                End If
                                count += 1
                            Loop
                            If sbGroupFolders.ToString <> String.Empty Then
                                sbGroupFolders.Append(".htm")
                            End If
                            '--------------------
                            ' Try with BU/PARTNER
                            '--------------------
                            ' Don't try when partner = *ALL
                            If PartnerCode <> Talent.Common.Utilities.GetAllString Then
                                PageProductBrowseHtml = TEBUtilities.GetHtmlFromFile(BusinessUnit & "/" & PartnerCode & "/GROUP" & sbGroupFolders.ToString)
                            Else
                                PageProductBrowseHtml = String.Empty
                            End If
                            If String.IsNullOrEmpty(PageProductBrowseHtml) Then
                                '------------
                                ' Try with BU
                                '------------
                                PageProductBrowseHtml = TEBUtilities.GetHtmlFromFile(BusinessUnit & "/GROUP" & sbGroupFolders.ToString)
                                If String.IsNullOrEmpty(PageProductBrowseHtml) Then
                                    '------------
                                    ' Try default
                                    '------------
                                    PageProductBrowseHtml = TEBUtilities.GetHtmlFromFile("GROUP" & sbGroupFolders.ToString)
                                End If
                            End If
                        Case Is = "DB"
                            PageProductBrowseHtml = dets.GroupHtml1 & dets.GroupHtml2 & dets.GroupHtml3

                    End Select
                End If
                Page.Title = dets.GroupPageTitle

            Case Is = "ALLQUERYSTRING"
                Dim wfrPage As New WebFormResource
                With wfrPage
                    .BusinessUnit = BusinessUnit
                    .PageCode = String.Empty
                    .PartnerCode = PartnerCode
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    .KeyCode = pagename
                    .PageCode = pagename
                End With
                Page.Title = wfrPage.Title(Talent.Common.Utilities.GetDefaultLanguage)
                If PageDefaults.Show_Page_Header Then
                    PageHeaderText = wfrPage.HeaderText(Talent.Common.Utilities.GetDefaultLanguage)
                End If
                PageProductBrowseIntroText = String.Empty

            Case Else
                Try
                    Dim wfrPage As New WebFormResource
                    With wfrPage
                        .BusinessUnit = BusinessUnit
                        .PageCode = String.Empty
                        .PartnerCode = PartnerCode
                        .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                        .KeyCode = pagename
                        .PageCode = pagename
                    End With
                    Page.Title = wfrPage.Title(Talent.Common.Utilities.GetDefaultLanguage)
                    If PageDefaults.Show_Page_Header Then
                        PageHeaderText = wfrPage.HeaderText(Talent.Common.Utilities.GetDefaultLanguage)
                    End If
                    PageProductBrowseIntroText = String.Empty
                Catch ex As Exception
                End Try
        End Select
    End Sub

    Private Sub matchQueryStringValues(ByRef view2 As DataView)
        Dim querystring As String = String.Empty
        Dim tempDataView As DataView = Nothing
        Dim i As Integer = 0
        For Each key As String In Request.QueryString.AllKeys
            If i > 0 Then querystring &= "&"
            querystring &= key & "=" & Request.QueryString.Item(key)
            i += 1
            view2.RowFilter = "[PAGE_QUERYSTRING] = '" & TEBUtilities.ReplaceSingleQuote(querystring) & "'"
            If view2.ToTable().Rows.Count > 0 Then Exit For
        Next
    End Sub

    Private Sub PopulateTheHTMLStrings(ByVal dt As Data.DataTable, ByVal source As String)
        For Each row As Data.DataRow In dt.Rows

            Select Case source.ToUpper
                Case Is = "FILE"
                    AddHTMLString(TEBUtilities.CheckForDBNull_String(row("SECTION")) & TEBUtilities.CheckForDBNull_String(row("SEQUENCE")), TEBUtilities.GetHtmlFromFile(TEBUtilities.CheckForDBNull_String((row("HTML_LOCATION")))))
                Case Is = "DB"
                    AddHTMLString(TEBUtilities.CheckForDBNull_String(row("SECTION")) & TEBUtilities.CheckForDBNull_String(row("SEQUENCE")), TEBUtilities.CheckForDBNull_String(row("HTML_1")) & " " & TEBUtilities.CheckForDBNull_String(row("HTML_2")) & " " & TEBUtilities.CheckForDBNull_String(row("HTML_3")))
                Case Else
            End Select
        Next
    End Sub

    ''' <summary>
    ''' Check the page the user is trying to access has any restrictions in place that must be checked first
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CheckForPageRestrictions()
        Dim userProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
        Dim tDataObjects As New TalentDataObjects
        tDataObjects.Settings = Talent.eCommerce.Utilities.GetSettingsObject()

        'Validate agent authority for each product type. If agent do not have access on any particular product then return unauthorised access error 
        Dim _canAccessToAddProductToBasket As Boolean = True
        Dim _canAccessTemplateOverride As Boolean = True
        Dim redirectToUnavailablePage As Boolean = False
        Select Case PageCode
            Case "producthome.aspx"
                If Not AgentProfile.AgentPermissions.CanAddHomeGameToBasket Then
                    _canAccessToAddProductToBasket = False
                End If
            Case "productseason.aspx"
                If Not AgentProfile.AgentPermissions.CanAddSeasonGameToBasket Then
                    _canAccessToAddProductToBasket = False
                End If
            Case "productaway.aspx"
                If Not AgentProfile.AgentPermissions.CanAddAwayGameToBasket Then
                    _canAccessToAddProductToBasket = False
                End If
            Case "productevent.aspx"
                If Not AgentProfile.AgentPermissions.CanAddEventProductToBasket Then
                    _canAccessToAddProductToBasket = False
                End If
            Case "producttravel.aspx"
                If Not AgentProfile.AgentPermissions.CanAddTravelProductToBasket Then
                    _canAccessToAddProductToBasket = False
                End If
            Case "productmembership.aspx"
                If Not AgentProfile.AgentPermissions.CanAddMembershipsProductToBasket Then
                    _canAccessToAddProductToBasket = False
                End If
            Case "templateoverride.aspx"
                If Not AgentProfile.IsAgent Then
                    _canAccessTemplateOverride = False
                End If
        End Select
        If AgentProfile.IsAgent Then
            redirectToUnavailablePage = Not _canAccessToAddProductToBasket
        Else
            redirectToUnavailablePage = Not _canAccessTemplateOverride
        End If

        If redirectToUnavailablePage Then
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If


        If ModuleDefaults.PageRestrictionsInUse Then
            Dim agentRestriction As Boolean = False
            If (userProfile.User.Details IsNot Nothing) Then
                agentRestriction = userProfile.User.Details.OWNS_AUTO_MEMBERSHIP
            End If

            Dim restrictionReturnCode As String = Talent.eCommerce.Utilities.PageRestrictions(BusinessUnit, PartnerCode, AgentProfile.IsAgent, AgentProfile.Type, agentRestriction, PageCode)
            If ((Not restrictionReturnCode.Equals(String.Empty)) And (restrictionReturnCode.Trim().Length > 0)) Then
                Session("UnavailableErrorCode") = restrictionReturnCode
                If (HttpContext.Current.Request.UrlReferrer Is Nothing) Then
                    Session("UnavailableReturnPage") = ""
                Else
                    Session("UnavailableReturnPage") = HttpContext.Current.Request.UrlReferrer.AbsoluteUri
                End If
                Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
            End If
        End If

        If Not userProfile.IsAnonymous AndAlso ModuleDefaults.RememberMeFunction AndAlso Not AgentProfile.IsAgent() Then
            If SensitivePages.IsSensitivePage(HttpContext.Current.Request.Url) Then
                If Not _TDataObjects.ProfileSettings.tblAuthorizedUsers.IsPasswordValidated(PartnerCode, BusinessUnit, userProfile.UserName, False) Then
                    Session("SensitivePageURL") = HttpContext.Current.Request.Url.AbsoluteUri
                    Response.Redirect("~/PagesLogin/Profile/ValidatePassword.aspx")
                End If
            End If
        End If

        If ModuleDefaults.AlertsEnabled Then
            If TEBUtilities.IsPageRestrictedByAlert(PageDefaults.RESTRICTING_ALERT_NAME) Then
                If userProfile.IsAnonymous Then
                    Dim returnURL As String = String.Empty
                    If Not String.IsNullOrWhiteSpace(Request.QueryString("ReturnUrl")) Then
                        returnURL += "&ReturnUrl=" & HttpContext.Current.Server.UrlEncode(Request.QueryString("ReturnUrl"))
                    Else
                        returnURL += "&ReturnUrl=" & HttpContext.Current.Server.UrlEncode(Request.Url.AbsolutePath & "?" & Request.QueryString.ToString)
                    End If
                    If AgentProfile.IsAgent Then
                        Response.Redirect("~/PagesPublic/Profile/CustomerSelection.aspx?RequiresLogin=True" & returnURL)
                    Else
                        Response.Redirect("~/PagesPublic/Login/Login.aspx?RequiresLogin=True" & returnURL)
                    End If
                Else
                    Dim dv As DataView = TEBUtilities.GetAlertDataViewByAction(GlobalConstants.ALERT_ACTION_PAGE_RESTRICT)
                    If dv.Count = 0 Then
                        HttpContext.Current.Session("RestrictedRedirectShowShadowBox") = True
                        Dim PageRestrictedAlertErrorPageURL As String = ResolveUrl(ModuleDefaults.PageRestrictedAlertErrorPageURL())
                        If PageRestrictedAlertErrorPageURL.Contains("?") Then

                            Response.Redirect(PageRestrictedAlertErrorPageURL & "&unauthorisedMsg=" & PageDefaults.RESTRICTING_ALERT_NAME)
                        Else
                            Response.Redirect(PageRestrictedAlertErrorPageURL & "?unauthorisedMsg=" & PageDefaults.RESTRICTING_ALERT_NAME)
                        End If
                    End If
                End If
                'End If
            End If
        End If



    End Sub

    Private Function CanProcessSessionValidation() As Boolean
        Dim canProcess As Boolean = True
        If (Not String.IsNullOrWhiteSpace(Request.QueryString("IsSingleProduct"))) AndAlso (Request.QueryString("IsSingleProduct") = "TRUE") Then
            If (Page.IsPostBack) Then
                _overrideNotPostBack = True
            Else
                canProcess = False
            End If
        End If
        Return canProcess
    End Function

    Private Function IsThisPageRequireSessionValidation(ByVal currentPageName As String) As Boolean
        Dim isRequired As Boolean = True
        currentPageName = currentPageName.ToLower
        If currentPageName = LCase("TicketingGateway.aspx") _
            OrElse currentPageName = LCase("AgentLogin.aspx") _
            OrElse currentPageName = LCase("clearcache.aspx") _
            OrElse currentPageName = LCase("PrerequisiteMissing.aspx") _
            OrElse currentPageName = LCase("SessionError.aspx") Then
            isRequired = False
        End If
        Return isRequired
    End Function

    Private Sub CheckForValidSession()
        'splited this logic into two calls
        'one for check valid session or not
        'if not valid session call the second one with required attributes
        'this will reduce unnecessary calls to cache to get the business unit url device entities
        'as this method will be called for each page
        Dim talSessionProvider As New TalentSessionProvider
        talSessionProvider.InValidSessionPageURL = ResolveUrl("~/PagesPublic/Error/SessionError.aspx")
        talSessionProvider.EcomModuleDefaultsValues = ModuleDefaults
        talSessionProvider.PageIsPostBack = Page.IsPostBack
        talSessionProvider.AgentProfile = AgentProfile
        talSessionProvider.SettingsNoise = Talent.eCommerce.Utilities.GetSettingsObject()
        talSessionProvider.CurrentPageName = Talent.eCommerce.Utilities.GetCurrentPageName()
        If talSessionProvider.IsInvalidSession Then
            talSessionProvider.TrackingScript = TrackingCodes.DoInsertTrackingCodes("BODY")
            talSessionProvider.CurrentBusinessUnitURLDeviceEntity = TalentCache.GetBusinessUnitURLDeviceEntity()
            talSessionProvider.AllBUURLDeviceEntities = Talent.eCommerce.Utilities.GetAllBUURLDeviceEntities()
            talSessionProvider.ProcessInValidSession()
        End If
        AgentProfile = talSessionProvider.AgentProfile
    End Sub

    Private Sub CreateMetaTags()

        Dim metaKeywords As String = String.Empty
        Dim metaDescription As String = String.Empty

        Dim hm1 As HtmlMeta = New HtmlMeta()
        Dim hm2 As HtmlMeta = New HtmlMeta()

        Dim head As HtmlHead = CType(Page.Header, HtmlHead)
        '----------------------
        ' Pick up standard tags
        '----------------------
        Dim wfrPage As New WebFormResource
        With wfrPage
            .BusinessUnit = BusinessUnit
            .PageCode = String.Empty
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
        End With
        metaKeywords = wfrPage.MetaKeywords(Talent.Common.Utilities.GetDefaultLanguage)
        metaDescription = wfrPage.MetaDescription(Talent.Common.Utilities.GetDefaultLanguage)
        Dim cacheKey As String = String.Empty
        Select Case PageDefaults.Page_Type
            Case Is = "STANDARD"
                '--------------------
                ' Standard - no extra
                '--------------------

            Case Is = "PRODUCT"
                '-----------------------------------------------------------
                ' Product - Check whether to override tags with product tags
                '-----------------------------------------------------------
                Dim prodCode As String = String.Empty
                If Not Request("product") Is Nothing Then
                    prodCode = Request("product")
                End If

                Dim productData As New TalentProductInformationTableAdapters.tbl_productTableAdapter
                Dim dt As DataTable

                cacheKey = "CreateMetaTags - " & "PRODUCT - " & prodCode
                If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                    dt = HttpContext.Current.Cache.Item(cacheKey)
                Else
                    dt = productData.GetDataByProduct_Code(prodCode)
                    'Cache the result
                    TalentCache.AddPropertyToCache(cacheKey, dt, CInt(ConfigurationManager.AppSettings("CacheTimeInMinutes")), TimeSpan.Zero, CacheItemPriority.Normal)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If

                If dt.Rows.Count > 0 Then
                    If TEBUtilities.CheckForDBNull_String(dt.Rows(0)("PRODUCT_META_KEYWORDS")) <> String.Empty Then
                        metaKeywords = TEBUtilities.CheckForDBNull_String(dt.Rows(0)("PRODUCT_META_KEYWORDS"))
                    End If
                    If TEBUtilities.CheckForDBNull_String(dt.Rows(0)("PRODUCT_META_DESCRIPTION")) <> String.Empty Then
                        metaDescription = TEBUtilities.CheckForDBNull_String(dt.Rows(0)("PRODUCT_META_DESCRIPTION"))
                    End If
                End If

            Case Is = "BROWSE"
                '---------------------------------------------------------------
                ' Product - Check whether to override tags with group level tags
                '---------------------------------------------------------------
                Dim groupDetails As Talent.eCommerce.GroupLevelDetails
                Dim dets As Talent.eCommerce.GroupLevelDetails.GroupDetails
                groupDetails = New Talent.eCommerce.GroupLevelDetails
                dets = groupDetails.GetGroupLevelDetails()
                If dets.GroupMetaKeywords <> String.Empty Then
                    metaKeywords = dets.GroupMetaKeywords
                End If
                If dets.GroupMetaDescription <> String.Empty Then
                    metaDescription = dets.GroupMetaDescription
                End If
        End Select

        hm1.Name = "Keywords"
        hm1.Content = metaKeywords
        head.Controls.Add(hm1)

        hm2.Name = "description"
        hm2.Content = metaDescription
        head.Controls.Add(hm2)

    End Sub

    Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If ModuleDefaults.TicketingKioskMode Then
            Dim supplier = ModuleDefaults.KioskSupplier
            'Ensure the user is signed out upon entry (in case left over from last session)
            If PageCode.ToLower.Equals("home.aspx") And Not Request("fgbsmc") Is Nothing Then
                FormsAuthentication.SignOut()
            End If
            If Not User.Identity.IsAuthenticated And supplier = "FORTRESS" Then
                doLoginForKiosk()
            End If
            doKioskTimeOut()
        End If
    End Sub

    ''' <summary>
    ''' If the page is tradeplacelogin.aspx then doautologin if the sessionid already exist in
    ''' tbl_active_noise_sessions
    ''' </summary>
    Private Sub TradePlaceAutoLogin()
        Dim username As String = String.Empty
        Dim userpassword As String = String.Empty
        Dim sessionIdTicket As String = String.Empty
        Dim extraData As String = String.Empty
        Dim signatureReceived As String = String.Empty
        Dim additionalInfo As String = String.Empty

        If Request.QueryString("userid") IsNot Nothing _
            And Request.QueryString("password") IsNot Nothing _
            And Request.QueryString("ticket") IsNot Nothing _
            And Request.QueryString("ed") IsNot Nothing _
            And Request.QueryString("signature") IsNot Nothing Then

            username = Request.QueryString("userid")
            userpassword = Request.QueryString("password")
            sessionIdTicket = Request.QueryString("ticket")
            extraData = Request.QueryString("ed")
            signatureReceived = Request.QueryString("signature")

            If Request.QueryString("additional") IsNot Nothing Then
                additionalInfo = Request.QueryString("additional")
            End If

            Dim ecomModuleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
            Dim ecomModuleDefaultValues As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = ecomModuleDefaults.GetDefaults
            'Get the key from ecommercemoduledefaults
            Dim securityKey As String = ecomModuleDefaultValues.TradePlaceEncryptionKey

            'Decrypts the paramaters
            If (username.Trim().Length > 0) Then
                username = TCUtilities.SHA1TripleDESDecode(username, securityKey)
            End If
            If (userpassword.Trim().Length > 0) Then
                userpassword = TCUtilities.SHA1TripleDESDecode(userpassword, securityKey)
            End If
            If (sessionIdTicket.Trim().Length > 0) Then
                sessionIdTicket = TCUtilities.SHA1TripleDESDecode(sessionIdTicket, securityKey)
            End If
            If (extraData.Trim().Length > 0) Then
                extraData = TCUtilities.SHA1TripleDESDecode(extraData, securityKey)
            End If

            Dim valueToCreateSignature As String = username & sessionIdTicket & extraData


            Dim signatureCreated As String = String.Empty
            signatureCreated = TCUtilities.SHA1TripleDESEncode(valueToCreateSignature, securityKey)
            If signatureReceived = signatureCreated Then
                'check the ticket is active in Noise
                Dim isAgent As Boolean = False
                'Getting the DESettings object with default values
                Dim DESettingsValues As DESettings = TEBUtilities.GetSettingsObject
                DESettingsValues.LoginId = username
                'TalentNoise initialised with DESettings
                Dim talentNoiseRecorder As New TalentNoise(DESettingsValues, sessionIdTicket, _
                                                            Now, _
                                                            Now.AddMinutes(-ecomModuleDefaultValues.NOISE_THRESHOLD_MINUTES), _
                                                            ecomModuleDefaultValues.NOISE_MAX_SESSION_KEEP_ALIVE_MINUTES, _
                                                            isAgent, _
                                                             TblActiveNoiseSessions_Usage.TRADEPLACE)
                'Checks and update the given session in tbl_active_noise_session 
                talentNoiseRecorder.CheckAndUpdateNoiseSession()
                If talentNoiseRecorder.SuccessfullCall AndAlso talentNoiseRecorder.RowsAffected > 0 Then
                    ValidUser = TEBUtilities.loginUser(username, userpassword)
                    If ValidUser Then
                        HttpContext.Current.Profile.Initialize(username, True)
                        Dim tempProfile As ProfileCommon = CType(HttpContext.Current.Profile, ProfileCommon).GetProfile(username)
                        If tempProfile IsNot Nothing Then
                            CType(HttpContext.Current.Profile, TalentProfile).User = tempProfile.User
                            CType(HttpContext.Current.Profile, TalentProfile).Basket = tempProfile.Basket
                            CType(HttpContext.Current.Profile, TalentProfile).PartnerInfo = tempProfile.PartnerInfo
                        End If
                        If ecomModuleDefaultValues.RedirectAfterAutoLogin.Trim().Length > 0 Then
                            If Not HttpContext.Current.Profile.IsAnonymous AndAlso CType(HttpContext.Current.Profile, TalentProfile).PartnerInfo.Details.PARTNER_TYPE = "HOME" Then
                                Response.Redirect("~/PagesLogin/QuickOrder/QuickOrder.aspx")
                            Else
                                'Response.Redirect(def.PAGE_AFTER_LOGIN)
                                Response.Redirect(ecomModuleDefaultValues.RedirectAfterAutoLogin, True)
                            End If
                        End If
                    Else
                        'Nothing
                    End If
                Else
                    'Nothing
                End If
            Else
                'Nothing
            End If
        Else
            'Nothing
        End If
    End Sub

    ''' <summary>
    ''' If the page is sapocilogin.aspx then doautologin if required defaults are enabled
    ''' </summary>
    Private Sub SapOciAutoLogin()
        Dim username As String = String.Empty
        Dim userpassword As String = String.Empty
        Dim sapHookURL As String = String.Empty
        If Request.QueryString("username") IsNot Nothing _
            AndAlso Request.QueryString("password") IsNot Nothing _
            AndAlso Request.QueryString("HOOK_URL") IsNot Nothing _
            AndAlso Request.UrlReferrer IsNot Nothing Then
            username = Request.QueryString("username")
            userpassword = Request.QueryString("password")
            sapHookURL = Request.QueryString("HOOK_URL")
            Session("SAP_OCI_HOOK_URL") = sapHookURL
            'move referrer to session
            'After test : add urlreferer in if condition
            If Request.UrlReferrer IsNot Nothing Then
                Session("SAP_OCI_REFERER_URL") = Request.UrlReferrer.ToString()
            End If
            Dim ecomModuleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
            Dim ecomModuleDefaultValues As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = ecomModuleDefaults.GetDefaults
            If ecomModuleDefaultValues.ValidatePartnerDirectAccess AndAlso ecomModuleDefaultValues.SapOciPartner.Length > 0 Then
                ValidUser = Talent.eCommerce.Utilities.loginUser(username, userpassword)
                If ValidUser Then
                    HttpContext.Current.Profile.Initialize(username, True)
                    Dim tempProfile As ProfileCommon = CType(HttpContext.Current.Profile, ProfileCommon).GetProfile(username)
                    If tempProfile IsNot Nothing Then
                        CType(HttpContext.Current.Profile, TalentProfile).User = tempProfile.User
                        CType(HttpContext.Current.Profile, TalentProfile).Basket = tempProfile.Basket
                        CType(HttpContext.Current.Profile, TalentProfile).PartnerInfo = tempProfile.PartnerInfo
                    End If
                    'For testing
                    If ecomModuleDefaultValues.RedirectAfterAutoLogin.Trim().Length > 0 Then
                        Response.Redirect(ecomModuleDefaultValues.RedirectAfterAutoLogin, True)
                    End If
                Else
                    Session("ParameterError") = "INVALID"
                End If
            Else
                Session("ParameterError") = "CONFIG"
            End If
        Else
            Session("ParameterError") = "TRUE"
        End If

    End Sub

    Private Sub doLoginForKiosk()

        'Auto Login Code Here...
        If Not Request("memid") Is Nothing And Not Request("fgbsmc") Is Nothing Then

            Dim userName As String = String.Empty
            Dim password As String = String.Empty
            Dim fgbsmc As String = String.Empty

            Try

                'login using memid as customer number
                userName = Request("memid")
                If ModuleDefaults.LoginidType = "1" Then 'currently = '2'
                    'left pad the user name with 0's
                    userName = userName.PadLeft(12, "0")
                End If

                'Try to retrieve the customer details for non ticketing clubs
                If Not ModuleDefaults.RefreshCustomerDataOnLogin Or Not ModuleDefaults.ExternalPasswordIsMaster Then
                    Dim mu As TalentMembershipUser = CType(Membership.Provider, TalentMembershipProvider).GetUser(userName, True)
                    If Not mu Is Nothing Then
                        password = mu.Password
                    End If
                Else
                    'Authentication for the customer will be made by the kiosk, use the default password
                    password = ModuleDefaults.ExternalPasswordValue
                End If

                'Login the user in
                If Talent.eCommerce.Utilities.loginUser(userName, password) Then

                    'Successful Login - Generate Key
                    fgbsmc = Request("fgbsmc")
                    Dim dlsKFWSecurity As New Talent.eCommerce.clsKFWSecurity
                    Session("tktKey") = dlsKFWSecurity.EncryptData("TestKey001", fgbsmc)
                End If

            Catch ex As Exception

            End Try
        End If
        Dim dlsKFWSecurity2 As New Talent.eCommerce.clsKFWSecurity
        'Response.Write("key ='TestKey001' and fgbsmc=" & Request("fgbsmc") & " :" & dlsKFWSecurity2.EncryptData("TestKey001", fgbsmc))
    End Sub

    Private Sub doKioskTimeOut()
        'Set Timeout
        Dim timeout = ModuleDefaults.Kiosk_Inactive_Period_Timeout
        Dim forceInactiveShutdown = ModuleDefaults.ForceInactiveShutdown

        If forceInactiveShutdown And Not String.IsNullOrEmpty(Session("tktKey")) Then

            'Dim script As New HtmlGenericControl("SCRIPT")
            'script.Attributes.Add("type", "text/javascript")
            'script.InnerText = " alert('Javascript added! Your session tktKey is " & Session("tktKey") & ", page will timeout in " & timeout & " seconds');"
            'Me.Page.Header.Controls.Add(script)

            Dim script2 As New HtmlGenericControl("SCRIPT")
            script2.Attributes.Add("type", "text/javascript")
            script2.InnerText = "window.external.KFWInactivityTimeoutSec('" & Session("tktKey") & "', " & timeout & ");"
            Me.Page.Header.Controls.Add(script2)

            'Dim printURL = HttpContext.Current.Request.Url.AbsoluteUri

            'Dim script3 As New HtmlGenericControl("SCRIPT")
            'script3.Attributes.Add("type", "text/javascript")
            'script3.InnerText = "window.external.KFWPrinter('" & Session("tktKey") & "',0,'" & printURL & "')"
            'Me.Page.Header.Controls.Add(script3)

            'window.external.KFWInactivityTimeout(<SecurityToken>,<Kiosk Inactive Period Timeout>)
        End If

    End Sub

    Private Sub IsBasketAlreadyPaid()
        Dim pageName(2) As String
        Dim checkBasket As Boolean = False
        Dim currentPageName As String = PageCode.ToLower
        pageName(0) = "checkoutdeliverydetails.aspx"
        pageName(1) = "checkout.aspx"
        pageName(2) = "checkoutordersummary.aspx"
        Dim pageNameLength As Integer = pageName.Length - 1
        For loopCounter As Integer = 0 To pageNameLength
            If (currentPageName.Equals(pageName(loopCounter))) Then
                checkBasket = True
                Exit For
            End If
        Next
        If checkBasket Then
            If TEBUtilities.IsOrderAlreadyPaid() Then
                Session("OrderAlreadyPaid") = "YES"
                Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            End If
        End If
    End Sub

    Private Sub checkForCampaignCodeQueryString()
        Try
            If Session("CAMPAIGN_CODE") Is Nothing Then
                If LCase(Request.QueryString("campaigntracking")).Length > 0 Then
                    Session("CAMPAIGN_CODE") = LCase(Request.QueryString("campaigntracking"))
                End If
            End If
        Catch ex As Exception
            Session("CAMPAIGN_CODE") = Nothing
        End Try
    End Sub

    ''' <summary>
    ''' Update any basket header record that aren't processed for the current user if the basket has an EXTERNAL_PAYMENT_TOKEN.
    ''' Only do this if the unprocessed basket is not in cancel mode. In "C" mode the basket will have a transaction ID in EXTERNAL_PAYMENT_TOKEN and will be unprocessed.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteExternalPayDirtyBasketHeader()
        If Not HttpContext.Current.Profile.IsAnonymous Then
            Dim userProfile As TalentProfile = CType(HttpContext.Current.Profile, TalentProfile)
            If userProfile.Basket IsNot Nothing Then
                If Not String.IsNullOrWhiteSpace(userProfile.Basket.EXTERNAL_PAYMENT_TOKEN) AndAlso CATHelper.IsBasketNotInCancelMode Then
                    Dim basketHeader As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
                    Try
                        basketHeader.Update_External_Payment_Token(String.Empty, CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
                    Catch ex As Exception
                    End Try
                End If
            End If
        End If
    End Sub

    Private Sub ProcessAgentModeForWholeSite()
        Dim invalidSessionPageName As String = "SessionError.aspx"
        Dim currentPageName As String = TEBUtilities.GetCurrentPageName
        If Not AgentProfile.IsAgent AndAlso Not LCase(currentPageName) = LCase(invalidSessionPageName) _
            AndAlso Not LCase(currentPageName) = LCase("clearcache.aspx") AndAlso Not LCase(currentPageName) = LCase("TicketingGateway.aspx") _
            AndAlso Not LCase(currentPageName) = LCase("AgentLogin.aspx") Then
            Response.Redirect("~/PagesPublic/Agent/AgentLogin.aspx" & "?ReturnUrl=" & HttpContext.Current.Server.UrlEncode(Request.Url.AbsolutePath & "?" & Request.QueryString.ToString))
        End If
    End Sub

    Private Sub ProcessAgentModeForSession()
        'page requires autneticated profile and also it not allowed for generic sales
        If IsPageRquiresAuthentication() AndAlso (Not PageDefaults.Allow_Generic_Sales) Then
            Dim redirectPath As String = "~/PagesPublic/Profile/CustomerSelection.aspx?RequiresLogin=True"
            If Not String.IsNullOrWhiteSpace(Request.QueryString("ReturnUrl")) Then
                redirectPath += "&ReturnUrl=" & HttpContext.Current.Server.UrlEncode(Request.QueryString("ReturnUrl"))
            Else
                redirectPath += "&ReturnUrl=" & HttpContext.Current.Server.UrlEncode(Request.Url.AbsolutePath & "?" & Request.QueryString.ToString)
            End If
            Response.Redirect(redirectPath)
        End If
    End Sub

    Private Sub ProcessAgentPageForSession()
        'Force agent login if the user is trying to access the agent pages and isn't an agent
        If Not AgentProfile.IsAgent Then
            Response.Redirect("~/PagesPublic/Agent/AgentLogin.aspx" & "?ReturnUrl=" & HttpContext.Current.Server.UrlEncode(Request.Url.AbsolutePath & "?" & Request.QueryString.ToString))
        End If
    End Sub

    Private Sub ProcessPageForAuthentication()
        'page requires autneticated profile and also it not allowed for generic sales
        If IsPageRquiresAuthentication() Then
            Dim redirectPath As String = "~/PagesPublic/Login/login.aspx?RequiresLogin=True"
            If Not String.IsNullOrWhiteSpace(Request.QueryString("ReturnUrl")) Then
                redirectPath += "&ReturnUrl=" & HttpContext.Current.Server.UrlEncode(Request.QueryString("ReturnUrl"))
            Else
                redirectPath += "&ReturnUrl=" & HttpContext.Current.Server.UrlEncode(Request.Url.AbsolutePath & "?" & Request.QueryString.ToString)
            End If
            Response.Redirect(redirectPath)
        End If
    End Sub

    Private Function Club() As DataRow
        Dim clubs As DataTable = Talent.eCommerce.Utilities.GetClubs
        If clubs.Rows.Count > 0 Then
            Return clubs.Rows(0)
        End If
        Return Nothing
    End Function
#End Region

#Region "Public Functions"

    Public Function GetHtmlByUsage(ByVal usage As String) As String
        Return _HTML(usage)
    End Function

    ''' <summary>
    ''' Check to see if there are any product question templates to show
    ''' </summary>
    ''' <param name="basketItems">The basket items</param>
    ''' <param name="cacheDependencyPath">The cache dependancy path</param>
    ''' <param name="basketHeaderId">The basket header ID</param>
    ''' <param name="redirectMode">Redirect To Additional Products Page if templates are found</param>
    ''' <remarks></remarks>
    Public Sub BasketContainsItemLinkedToTemplate(ByVal basketItems As List(Of Talent.Common.DEBasketItem), ByVal cacheDependencyPath As String, ByVal basketHeaderId As String, Optional ByVal redirectMode As Boolean = True)

        Dim currentPageName As String = PageCode.ToLower
        If Not HttpContext.Current.Profile.IsAnonymous AndAlso (Not redirectMode Or currentPageName.Equals("checkout.aspx")) AndAlso HttpContext.Current.Session("TemplateIDs") Is Nothing AndAlso HttpContext.Current.Session("AddInfoCompleted") <> True _
            AndAlso CATHelper.IsBasketNotInCancelMode() AndAlso CType(HttpContext.Current.Profile, TalentProfile).Basket.BasketContentType <> GlobalConstants.MERCHANDISEBASKETCONTENTTYPE AndAlso Not IsPostBack AndAlso Not AgentProfile.BulkSalesMode Then
            Dim inputModel As New ActivitiesTemplateInputModel
            inputModel.basket = basketItems
            inputModel.BasketHeaderID = basketHeaderId
            inputModel.Username = HttpContext.Current.Profile.UserName
            inputModel.Fullname = CType(HttpContext.Current.Profile, TalentProfile).User.Details.Full_Name

            Dim builder As New ActivitiesModelBuilder
            builder.SetActivitiesSession(inputModel)

            If Not HttpContext.Current.Session("TemplateIDs") Is Nothing Then
                Dim templateIDData As List(Of TalentBusinessLogic.DataTransferObjects.ActivityTemplateQA) = HttpContext.Current.Session("TemplateIDs")
                If templateIDData.Count > 0 And redirectMode Then
                    Response.Redirect("~/PagesLogin/Checkout/AdditionalProductInformation.aspx")
                End If
            Else
                HttpContext.Current.Session("AddInfoCompleted") = False
            End If
        End If
    End Sub

#End Region

#Region "Web Methods"

    <WebMethod(), ScriptMethod()> _
    Public Shared Function GetProductCodeList(ByVal prefixText As String, ByVal count As Integer) As List(Of String)
        Dim wfrPage As New WebFormResource
        Dim productCodeList As List(Of String) = New List(Of String)
        Dim fieldToSearchOn As String = String.Empty
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        Dim myModuleDefaults As New Talent.eCommerce.ECommerceModuleDefaults
        Dim ModuleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
        ModuleDefaults = myModuleDefaults.GetDefaults()

        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageNameWebMethod()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageNameWebMethod()
        End With
        fieldToSearchOn = Talent.eCommerce.Utilities.CheckForDBNull_String(wfrPage.Attribute("ProductFieldForAutoComplete"))
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = wfrPage.BusinessUnit
        tDataObjects.Settings = settings
        Dim productOptionMaster As Boolean = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(wfrPage.Attribute("ACEProductOptionMasterFilter"))
        Dim productCodes As DataTable = tDataObjects.ProductsSettings.GetDataForAutoCompleteProductSearch(fieldToSearchOn, productOptionMaster, wfrPage.BusinessUnit, wfrPage.PartnerCode, ModuleDefaults.PriceList.ToString())
        tDataObjects = Nothing
        If productCodes IsNot Nothing AndAlso productCodes.Rows.Count > 0 Then
            Dim matchedRows() As DataRow = Nothing
            matchedRows = productCodes.Select(fieldToSearchOn & " LIKE'%" & prefixText & "%'")
            If matchedRows.Length > 0 Then
                For rowIndex As Integer = 0 To matchedRows.Length - 1
                    productCodeList.Add(matchedRows(rowIndex)(fieldToSearchOn).ToString())
                Next
            Else
                productCodeList.Add(wfrPage.Content("NothingFoundText", Talent.Common.Utilities.GetDefaultLanguage(), True))
            End If
        End If
        Return productCodeList
    End Function

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Determines whether [is page rquires authentication].
    ''' </summary><returns>
    '''   <c>true</c> if [is page rquires authentication]; otherwise, <c>false</c>.
    ''' </returns>
    Private Function IsPageRquiresAuthentication() As Boolean
        Dim isRequireAuthenticatedProfile As Boolean = False
        If HttpContext.Current.Profile.IsAnonymous AndAlso (Not Request.Url.AbsolutePath.ToLower.Contains("checkoutorderconfirmation.aspx")) Then
            If PageDefaults.Page_Type IsNot Nothing AndAlso ModuleDefaults.LoginToProductBrowse AndAlso Request.Url.AbsolutePath.ToUpper.IndexOf("PRODUCTBROWSE") > -1 Then
                isRequireAuthenticatedProfile = True
            ElseIf PageDefaults.Page_Type IsNot Nothing AndAlso PageDefaults.Force_Login Then
                isRequireAuthenticatedProfile = True
            ElseIf (Not String.IsNullOrWhiteSpace(ModuleDefaults.PagesLoginPath)) AndAlso (Request.Url.AbsolutePath.ToUpper.IndexOf(ModuleDefaults.PagesLoginPath.ToUpper) >= 0) Then
                isRequireAuthenticatedProfile = True
            End If
        End If
        Return isRequireAuthenticatedProfile
    End Function

    Private Function PopulatePageSettingsCache() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------------------
        Dim pageQueryString As String = String.Empty
        Dim PageNeedsToBeSecure As String = String.Empty
        Dim cacheKey1 As String = BusinessUnit & PageCode & "_PageQueryString"
        Dim cacheKey2 As String = BusinessUnit & PageCode & "_PageNeedsToBeSecure"
        Dim pageInUseCacheKey As String = BusinessUnit & PageCode & "_PageInUse"
        Dim pageCSSPrintCacheKey As String = BusinessUnit & PageCode & "_CSSPrint"
        Dim pageCSSPrint As Boolean = False
        Dim pageInUse As Boolean = True
        Dim AbsoluteUri As String = UCase(Request.Url.AbsoluteUri.Substring(0, 5))
        Dim cacheKey3 As String = PageCode & "Url"
        Dim cacheKey4 As String = PageCode & "SecureUrl"
        Dim cacheKey5 As String = PageCode & "SecurePort"
        Dim URL As String = String.Empty
        Dim SecureURL As String = String.Empty
        Dim SecurePort As String = String.Empty
        '
        Try
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey1) _
                OrElse Not Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey2) _
                    OrElse Not Talent.Common.TalentThreadSafe.ItemIsInCache(pageInUseCacheKey) _
                        OrElse Not Talent.Common.TalentThreadSafe.ItemIsInCache(pageCSSPrintCacheKey) Then
                '------------------------------------------------------------------------------------------
                '   Attempt to open database
                '
                err = Sql2005Open()
                '------------------------------------------------------------------------------------------
                Const Param1 As String = "@Param1"
                Const Param2 As String = "@Param2"
                Const Param3 As String = "@Param3"
                '
                Const strSelect As String = "SELECT * FROM tbl_page WITH (NOLOCK)                 " & _
                                                " WHERE  BUSINESS_UNIT	= @Param1   " & _
                                                " AND    PARTNER_CODE	= @Param2   " & _
                                                " AND    PAGE_CODE	    = @Param3   "
                '------------------------------------------------------------------------------------------
                If Not err.HasError Then
                    Try
                        Dim cmdSelect As SqlCommand = New SqlCommand(strSelect, _conSql2005)
                        '----------------------------------
                        ' Try for business unit and partner
                        '----------------------------------
                        cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                        cmdSelect.Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = PartnerCode
                        cmdSelect.Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = PageCode
                        Dim dtr As SqlDataReader = cmdSelect.ExecuteReader()
                        If dtr.HasRows Then
                            dtr.Read()
                            pageQueryString = dtr.Item("PAGE_QUERYSTRING").ToString
                            If _moduleDefaults.ServeAllPagesSecure Then
                                PageNeedsToBeSecure = True
                            Else
                                PageNeedsToBeSecure = dtr.Item("USE_SECURE_URL").ToString
                            End If
                            pageInUse = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(dtr.Item("IN_USE"))
                            pageCSSPrint = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtr.Item("CSS_PRINT"))
                        Else
                            dtr.Close()
                            '---------------------------------------
                            ' Try for business unit and all partners
                            '---------------------------------------
                            cmdSelect = New SqlCommand(strSelect, _conSql2005)
                            cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                            cmdSelect.Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = TEBUtilities.GetAllString
                            cmdSelect.Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = PageCode
                            dtr = cmdSelect.ExecuteReader
                            If dtr.HasRows Then
                                dtr.Read()
                                pageQueryString = dtr.Item("PAGE_QUERYSTRING").ToString
                                If _moduleDefaults.ServeAllPagesSecure Then
                                    PageNeedsToBeSecure = True
                                Else
                                    PageNeedsToBeSecure = dtr.Item("USE_SECURE_URL").ToString
                                End If
                                pageInUse = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(dtr.Item("IN_USE"))
                                pageCSSPrint = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtr.Item("CSS_PRINT"))
                            Else
                                dtr.Close()
                                '----------------------------------------------------------
                                ' Try for all business units and all partners and all pages
                                '----------------------------------------------------------
                                cmdSelect = New SqlCommand(strSelect, _conSql2005)
                                cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = TEBUtilities.GetAllString
                                cmdSelect.Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = TEBUtilities.GetAllString
                                cmdSelect.Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = PageCode
                                dtr = cmdSelect.ExecuteReader
                                If dtr.HasRows Then
                                    dtr.Read()
                                    pageQueryString = dtr.Item("PAGE_QUERYSTRING").ToString
                                    If _moduleDefaults.ServeAllPagesSecure Then
                                        PageNeedsToBeSecure = True
                                    Else
                                        PageNeedsToBeSecure = dtr.Item("USE_SECURE_URL").ToString
                                    End If
                                    pageInUse = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(dtr.Item("IN_USE"))
                                    pageCSSPrint = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtr.Item("CSS_PRINT"))
                                Else
                                    dtr.Close()
                                    '----------------------------------------------------------
                                    ' Try for all business units and all partners and all pages
                                    '----------------------------------------------------------
                                    cmdSelect = New SqlCommand(strSelect, _conSql2005)
                                    cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = TEBUtilities.GetAllString
                                    cmdSelect.Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = TEBUtilities.GetAllString
                                    cmdSelect.Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = TEBUtilities.GetAllString
                                    dtr = cmdSelect.ExecuteReader
                                    If dtr.HasRows Then
                                        dtr.Read()
                                        pageQueryString = dtr.Item("PAGE_QUERYSTRING").ToString
                                        If _moduleDefaults.ServeAllPagesSecure Then

                                            PageNeedsToBeSecure = True
                                        Else
                                            PageNeedsToBeSecure = dtr.Item("USE_SECURE_URL").ToString
                                        End If
                                        pageInUse = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(dtr.Item("IN_USE"))
                                        pageCSSPrint = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dtr.Item("CSS_PRINT"))
                                    End If
                                End If
                            End If
                        End If
                        dtr.Close()
                    Catch ex As Exception
                        Const strError8 As String = "Error during database access"
                        With err
                            .ErrorMessage = ex.Message
                            .ErrorStatus = strError8
                            .ErrorNumber = "ACTB01-12"
                            .HasError = True
                        End With
                    End Try
                End If
                '-----------------------------------------------------------------------------
                '   Even if no record found still write to cache
                '
                HttpContext.Current.Cache.Insert(cacheKey1, pageQueryString)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey1)

                HttpContext.Current.Cache.Insert(cacheKey2, PageNeedsToBeSecure)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey2)

                HttpContext.Current.Cache.Insert(pageInUseCacheKey, pageInUse)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(pageInUseCacheKey)

                HttpContext.Current.Cache.Insert(pageCSSPrintCacheKey, pageCSSPrint)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(pageCSSPrintCacheKey)

                '
                err = Sql2005Close()
            End If

            '------------------------------------------------------------------------------------------
            '   See if we need to switch to secure or none secure URL
            '
            Select Case AbsoluteUri.ToUpper
                Case Is = "HTTP:"
                    URL = AbsoluteUri & "//" & Request.Url.Host
                    If Request.Url.Port <> "80" Then URL = URL + ":" + Request.Url.Port.ToString.Trim
                Case Is = "HTTPS"
                    SecureURL = AbsoluteUri & "://" & Request.Url.Host
                    If Request.Url.Port <> "443" Then SecureURL = SecureURL + ":" + Request.Url.Port.ToString.Trim
            End Select
            '
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey3) _
                OrElse Not Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey4) _
                    OrElse Not Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey5) Then
                '--------------------------------------------------------------------------------------
                '   Attempt to open database
                '
                err = Sql2005Open()
                '--------------------------------------------------------------------------------------
                Const Param1 As String = "@Param1"
                Const strSelect1 As String = "SELECT * FROM tbl_url_secure WITH (NOLOCK)  WHERE URL = @Param1 "
                Const strSelect2 As String = "SELECT * FROM tbl_url_secure WITH (NOLOCK)  WHERE SECURE_URL = @Param1 "
                '
                Dim cmdSelect As SqlCommand = Nothing
                '--------------------------------------------------------------------------------------
                If Not err.HasError Then
                    Try
                        If URL.Length > 0 Then
                            cmdSelect = New SqlCommand(strSelect1, _conSql2005)
                            cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = URL
                        Else
                            cmdSelect = New SqlCommand(strSelect2, _conSql2005)
                            cmdSelect.Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = SecureURL
                        End If
                        Dim dtr As SqlDataReader = cmdSelect.ExecuteReader()
                        If dtr.HasRows Then
                            dtr.Read()
                            URL = dtr.Item("Url").ToString
                            SecureURL = dtr.Item("SECURE_URL").ToString
                            SecurePort = dtr.Item("SECURE_PORT").ToString
                        End If
                        dtr.Close()
                    Catch ex As Exception
                        Const strError8 As String = "Error during database access"
                        With err
                            .ErrorMessage = ex.Message
                            .ErrorStatus = strError8
                            .ErrorNumber = "ACTB01-18"
                            .HasError = True
                        End With
                    End Try
                End If
                '-----------------------------------------------------------------------------
                '   Even if no record found still write to cache
                '
                HttpContext.Current.Cache.Insert(cacheKey3, URL)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey3)

                HttpContext.Current.Cache.Insert(cacheKey4, SecureURL)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey4)

                HttpContext.Current.Cache.Insert(cacheKey5, SecurePort)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey5)

                HttpContext.Current.Cache.Insert("SecurePort", SecurePort)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord("SecurePort")

                err = Sql2005Close()
            End If
        Catch ex As Exception
            Const strError8 As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "ACTB01-13"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function Check_QueryString_Secure() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------------------
        Dim useSecureURL As Boolean = False
        Dim URLLower As String = HttpContext.Current.Request.Url.ToString.ToLower
        If URLLower.Contains("http://localhost") OrElse URLLower.Contains("clearcache.aspx") Then
            useSecureURL = False
        Else
            useSecureURL = CType(ConfigurationManager.AppSettings("useSecureURL"), Boolean)
        End If

        Dim pageQueryString As String = String.Empty
        Dim PageNeedsToBeSecure As String = String.Empty
        Dim cacheKey1 As String = BusinessUnit & PageCode & "_PageQueryString"
        Dim cacheKey2 As String = BusinessUnit & PageCode & "_PageNeedsToBeSecure"
        Dim pageInUseCacheKey As String = BusinessUnit & PageCode & "_PageInUse"
        Dim pageInUse As Boolean = True
        Dim AbsoluteUri As String = UCase(Request.Url.AbsoluteUri.Substring(0, 5))
        Dim cacheKey3 As String = PageCode & "Url"
        Dim cacheKey4 As String = PageCode & "SecureUrl"
        Dim cacheKey5 As String = PageCode & "SecurePort"
        Dim URL As String = String.Empty
        Dim SecureURL As String = String.Empty
        Dim SecurePort As String = String.Empty
        '
        Try
            If Not Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey1) _
                OrElse Not Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey2) _
                    OrElse Not Talent.Common.TalentThreadSafe.ItemIsInCache(pageInUseCacheKey) _
                        OrElse Not Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey3) _
                            OrElse Not Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey4) _
                                OrElse Not Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey5) Then
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey1)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey2)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(pageInUseCacheKey)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey3)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey4)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey5)
                err = PopulatePageSettingsCache()
            End If

            pageQueryString = HttpContext.Current.Cache.Item(cacheKey1).ToString.Trim
            If _moduleDefaults.ServeAllPagesSecure Then
                PageNeedsToBeSecure = True
            Else
                PageNeedsToBeSecure = HttpContext.Current.Cache.Item(cacheKey2).ToString.Trim
            End If
            pageInUse = HttpContext.Current.Cache.Item(pageInUseCacheKey).ToString.Trim
            URL = HttpContext.Current.Cache.Item(cacheKey3).ToString.Trim
            SecureURL = HttpContext.Current.Cache.Item(cacheKey4).ToString.Trim
            SecurePort = HttpContext.Current.Cache.Item(cacheKey5).ToString.Trim

            'If the Page is not set to IN_USE, then redirct to Not Found page
            If Not pageInUse Then
                Response.Redirect("~/PagesPublic/Error/NotFound.aspx")
            End If

            If ModuleDefaults.EnsureValidQueryString Then
                '------------------------------------------------------------------------------------
                '   If mandatory fields in the query string exist, then check each one to see if 
                '   the current page's URL contains it
                ' 
                Dim queryStringValid As Boolean = True
                Dim queryString As NameValueCollection = HttpContext.Current.Request.QueryString
                '
                If pageQueryString.Length > 0 Then
                    Dim requiredFields() As String = pageQueryString.Split("&")
                    For Each requiredField As String In requiredFields
                        If requiredField <> String.Empty Then
                            If queryString.Item(requiredField) = String.Empty Then
                                queryStringValid = False
                                Exit For
                            End If
                        End If
                    Next
                End If
                '
                If Not queryStringValid Then Response.Redirect("~/PagesPublic/error/error.aspx")
            End If

            '------------------------------------------------------------------------------------------
            '   See if we need to switch to secure or none secure URL
            '
            Select Case AbsoluteUri.ToUpper
                Case Is = "HTTP:"
                    URL = AbsoluteUri & "//" & Request.Url.Host
                Case Is = "HTTPS"
                    SecureURL = AbsoluteUri & "://" & Request.Url.Host
            End Select

            '------------------------------------------------------------------------------------
            '   Redirect to secure URL?
            '
            Dim SSLOffloaded As String = ConfigurationManager.AppSettings("SSLOffloaded")
            If useSecureURL And PageNeedsToBeSecure.Length > 0 Then
                '
                If Not (SSLOffloaded Is Nothing) AndAlso SSLOffloaded.ToLower = "true" Then
                    ' Redirect to secure page 
                    If CType(PageNeedsToBeSecure, Boolean) And Not (Request.Url.Port.ToString.Trim = SecurePort) Then
                        Response.Redirect(SecureURL & Request.RawUrl)
                    End If
                    ' Redirect to non-secure page
                    If Not CType(PageNeedsToBeSecure, Boolean) And (Request.Url.Port.ToString.Trim = SecurePort) Then
                        Response.Redirect(URL & Request.RawUrl)
                    End If
                Else
                    ' Redirect to secure page
                    If CType(PageNeedsToBeSecure, Boolean) And Not (Request.IsSecureConnection Or Request.Url.Port.ToString.Trim = SecurePort) Then
                        Response.Redirect(SecureURL & Request.RawUrl)
                    End If
                    ' Redirect to non-secure page
                    If Not CType(PageNeedsToBeSecure, Boolean) And (Request.IsSecureConnection Or Request.Url.Port.ToString.Trim = SecurePort) Then
                        Response.Redirect(URL & Request.RawUrl)
                    End If
                End If
                '
            End If
            '------------------------------------------------------------------------------
        Catch ex As Exception
        End Try

        Return err
    End Function

    Private Function retrievePassword(ByVal username As String) As String

        Dim password As String = String.Empty

        Dim lookupCustNo As String = String.Empty
        Dim lookupEmail As String = String.Empty

        If username.Contains("@") Then
            lookupEmail = username
            lookupCustNo = translateEmail(username)
        Else
            lookupCustNo = username
        End If
        '--------------------------------------------------------
        ' Retreive the customer details
        '---------------------------------------------------------
        If ModuleDefaults.UseLoginLookup Then
            Dim mem As New TalentMembershipProvider
            Dim errorCode As String = String.Empty
            password = mem.RetrieveBackendPassword(lookupCustNo, lookupEmail, 0, errorCode)
        End If

        Return password

    End Function

    Private Function translateEmail(ByVal username As String) As String

        Dim customerNumber As String = String.Empty

        ' You can use email address or login id when login lookup is set
        If ModuleDefaults.UseLoginLookup Then

            Dim profile1 As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter
            Dim dt As Data.DataTable

            Dim authPartners As New TalentMembershipDatasetTableAdapters.tbl_authorized_partnersTableAdapter
            If authPartners.Get_CheckFor_B2C_Login(TalentCache.GetBusinessUnitGroup).Rows.Count > 0 Then
                dt = profile1.GetDataByEmail(username, TalentCache.GetPartner(HttpContext.Current.Profile))
            Else
                dt = profile1.GetDataByEmailNoPartner(username)
            End If

            If dt.Rows.Count > 0 Then
                customerNumber = dt.Rows(0).Item("LOGINID")
            End If
        End If

        ' Add leading zeros when loginId is customerNumber
        If ModuleDefaults.LoginidType.Equals("1") Then
            customerNumber = Talent.Common.Utilities.PadLeadingZeros(customerNumber.Trim, "12")
        End If

        Return customerNumber

    End Function

    Private Function Sql2005Open() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------------
        '   Attempt to open database
        '
        Try
            If _conSql2005 Is Nothing Then
                _conSql2005 = New SqlConnection(_frontEndConnectionString)
                _conSql2005.Open()
            ElseIf _conSql2005.State <> ConnectionState.Open Then
                _conSql2005 = New SqlConnection(_frontEndConnectionString)
                _conSql2005.Open()
            End If
        Catch ex As Exception
            Const strError As String = "Could not establish connection to the Sql2005 database"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "ACTB01-P1"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function Sql2005Close() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------
        '   Warning :   Using ErrorObj here when closing the DB does cause a   
        '               problem as it will clear the err object when an actual
        '               error has occured elsewhere   
        '------------------------------------------------------------------------
        Try
            If Not (_conSql2005 Is Nothing) Then
                If (_conSql2005.State = ConnectionState.Open) Then
                    _conSql2005.Close()
                End If
            End If
        Catch ex As Exception
            Const strError As String = "Failed to close the Sql2005 database connection"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "ACTB01-P2"
                .HasError = True
                Return err
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Get the username from the cookie
    ''' </summary>
    ''' <returns>Empty string if the functionality is disabled or cookie invalid/not found, otherwise returns valid username</returns>
    ''' <remarks></remarks>
    Private Function getRememberMeCookieValue() As String
        Dim username As String = String.Empty
        If ModuleDefaults.RememberMeFunction AndAlso ModuleDefaults.RememberMeCookieEncodeKey.Length > 0 Then
            Dim cookieName As String = ModuleDefaults.RememberMeCookieName
            If cookieName.Length = 0 Then cookieName = "KMLI"
            If Request.Cookies.Item(cookieName) IsNot Nothing Then
                username = TCUtilities.TripleDESDecode(Request.Cookies.Item(cookieName).Value, ModuleDefaults.RememberMeCookieEncodeKey)
            End If
        End If
        Return username
    End Function

#End Region

End Class