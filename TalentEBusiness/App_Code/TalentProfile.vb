Imports Microsoft.VisualBasic
Imports System.Web.Profile
Imports System.Data
Imports System.Data.SqlClient

Public Class TalentProfile
    Inherits ProfileBase

    <SettingsAllowAnonymous(False)> _
    Public Property User() As TalentProfileUser
        Get
            Return Me("User")
        End Get
        Set(ByVal value As TalentProfileUser)
            Me("User") = value
        End Set
    End Property

    <SettingsAllowAnonymous(False)> _
    Public Property PartnerInfo() As TalentProfilePartner
        Get
            Return Me("PartnerInfo")
        End Get
        Set(ByVal value As TalentProfilePartner)
            Me("PartnerInfo") = value
        End Set
    End Property

    <SettingsAllowAnonymous(True)> _
    Public Property Basket() As TalentBasket
        Get
            Return Me("Basket")
        End Get
        Set(ByVal value As TalentBasket)
            Me("Basket") = value
        End Set
    End Property

    Public ReadOnly Property Provider() As TalentProfileProvider
        Get
            Return CType(Me.Providers("TalentProfileProvider"), TalentProfileProvider)
        End Get
    End Property

    Public Function GetMinimumPurchaseQuantity() As Decimal
        Dim moduleDefaults As New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults

        'If NOT anonymous, check user and partner level settings, otherwise take global default settings
        If HttpContext.Current.Profile.IsAnonymous Then
            If def.UseMinimumPurchaseQuantity Then
                Return def.MinimumPurchaseQuantity
            Else
                Return 0
            End If
        Else
            If User.Details.Use_Minimum_Purchase_Quantity Then
                Return User.Details.Minimum_Purchase_Quantity
            ElseIf PartnerInfo.Details.Use_Minimum_Purchase_Quantity Then
                Return PartnerInfo.Details.Minimum_Purchase_Quantity
            ElseIf def.UseMinimumPurchaseQuantity Then
                Return def.MinimumPurchaseQuantity
            Else
                Return 0
            End If
        End If
    End Function

    Public Function GetMinimumPurchaseAmount() As Decimal
        Dim moduleDefaults As New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        If HttpContext.Current.Profile.IsAnonymous Then
            If def.UseMinimumPurchaseAmount Then
                Return def.MinimumPurchaseAmount
            Else
                Return 0
            End If
        Else
            If User.Details.Use_Minimum_Purchase_Amount Then
                Return User.Details.Minimum_Purchase_Amount
            ElseIf PartnerInfo.Details.Use_Minimum_Purchase_Amount Then
                Return PartnerInfo.Details.Minimum_Purchase_Amount
            ElseIf def.UseMinimumPurchaseAmount Then
                Return def.MinimumPurchaseAmount
            Else
                Return 0
            End If
        End If
    End Function

    Public Sub CustomerLogout(ByVal returnUrl As String)
        PreLogOut()
        FormsAuthentication.SignOut()
        LoggedOut(returnUrl)
    End Sub

    Public Sub PreLogOut()
        'We have to set a cookie to remember the Basket Header ID of the logged in user 
        'so that we can access it when the user has logged out
        If HttpContext.Current.Response.Cookies(".ASPNET_BASKET_ID") Is Nothing Then
            Dim newCookie As New HttpCookie(".ASPNET_BASKET_ID", Basket.Basket_Header_ID)
            newCookie.Expires = Now.AddHours(1)
            HttpContext.Current.Response.Cookies.Add(newCookie)
        Else
            HttpContext.Current.Response.Cookies(".ASPNET_BASKET_ID").Value = Basket.Basket_Header_ID
        End If
    End Sub

    Public Sub LoggedOut(ByVal returnUrl As String)
        'We must remove all the ticketing items when logging out of the web site
        Dim basketTA As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        Dim myDefs As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        basketTA.Delete_All_Retail_Items(Basket.Basket_Header_ID)

        Try
            If myDefs.CreateSingleSignOnCookies Then
                Dim cookieUsername As System.Web.HttpCookie = CType(HttpContext.Current.Response.Cookies.Item("USERNAME"), System.Web.HttpCookie)
                Dim cookieEncryptedUsername As System.Web.HttpCookie = CType(HttpContext.Current.Response.Cookies.Item("HASH1"), System.Web.HttpCookie)
                Dim cookieEncryptedPassword As System.Web.HttpCookie = CType(HttpContext.Current.Response.Cookies.Item("HASH2"), System.Web.HttpCookie)
                If myDefs.SingleSignOnDomain.Length > 0 Then
                    cookieUsername.Domain = myDefs.SingleSignOnDomain
                    cookieEncryptedUsername.Domain = myDefs.SingleSignOnDomain
                    cookieEncryptedPassword.Domain = myDefs.SingleSignOnDomain
                End If
                Talent.eCommerce.Utilities.RemoveCookie(cookieUsername)
                Talent.eCommerce.Utilities.RemoveCookie(cookieEncryptedUsername)
                Talent.eCommerce.Utilities.RemoveCookie(cookieEncryptedPassword)
                HttpContext.Current.Session("justLoggedOff") = "True"
            End If

            ' Alerts - removed cached set of alerts (per customer) when logged out.
            If (HttpContext.Current.Session("CanShowAlertOnPageLoad") IsNot Nothing) Then HttpContext.Current.Session.Remove("CanShowAlertOnPageLoad")
            Dim sCacheKey As String = "SQLAccessDataObjects_AlertsGetUnReadUserAlertsByBUPartnerLoginID" & HttpContext.Current.Profile.UserName & TalentCache.GetBusinessUnit & TalentCache.GetPartner(HttpContext.Current.Profile)
            If Not HttpContext.Current.Cache.Item(sCacheKey) Is Nothing Then
                HttpContext.Current.Cache.Remove(sCacheKey)
            End If

            ' Remove sessiom variables used as temporary store for comments 
            Dim itemsToDelete As New Collection
            For Each cacheItem In HttpContext.Current.Session.Contents
                If cacheItem.ToString().StartsWith("comments_") Then itemsToDelete.Add(cacheItem.ToString())
            Next

            For Each deleteItem As String In itemsToDelete
                HttpContext.Current.Session.Remove(deleteItem)
            Next

            'Retrieve the destination
            Dim sPath As String = HttpContext.Current.Request.Url.PathAndQuery
            If Not (String.IsNullOrEmpty(HttpContext.Current.Session.Item("SAP_OCI_HOOK_URL"))) Then
                HttpContext.Current.Session.Remove("SAP_OCI_HOOK_URL")
                If HttpContext.Current.Session.Item("SAP_OCI_REFERER_URL") IsNot Nothing Then
                    sPath = HttpContext.Current.Session.Item("SAP_OCI_REFERER_URL").ToString
                Else
                    sPath = Talent.eCommerce.Utilities.GetSiteHomePage()
                End If
            ElseIf Not String.IsNullOrWhiteSpace(returnUrl) Then
                sPath = returnUrl
            ElseIf myDefs.LogoutPage <> String.Empty Then
                sPath = myDefs.LogoutPage
            End If

            Talent.eCommerce.Utilities.RemoveCookie(myDefs.RememberMeCookieName)

            'Clear sessions
            If (HttpContext.Current.Session("CodeFromPromotionsBox") IsNot Nothing) Then HttpContext.Current.Session("CodeFromPromotionsBox") = Nothing
            If (HttpContext.Current.Session("AllPrmotionCodesEnteredByUser") IsNot Nothing) Then HttpContext.Current.Session("AllPrmotionCodesEnteredByUser") = Nothing
            If (HttpContext.Current.Session("UserFromPromotionsBox") IsNot Nothing) Then HttpContext.Current.Session("UserFromPromotionsBox") = Nothing
            If (HttpContext.Current.Session("SAP_OCI_HOOK_URL") IsNot Nothing) Then HttpContext.Current.Session.Remove("SAP_OCI_HOOK_URL")
            If (HttpContext.Current.Session("CanShowAlertOnPageLoad") IsNot Nothing) Then HttpContext.Current.Session.Remove("CanShowAlertOnPageLoad")
            If (HttpContext.Current.Session("PartnerPromotionCode") IsNot Nothing) Then HttpContext.Current.Session.Remove("PartnerPromotionCode")
            If (HttpContext.Current.Session("LoggedInCompanyNumber") IsNot Nothing) Then HttpContext.Current.Session("LoggedInCompanyNumber") = Nothing
            If (HttpContext.Current.Session("LoggedInCompanyName") IsNot Nothing) Then HttpContext.Current.Session("LoggedInCompanyName") = Nothing
            Dim basketItems As DataTable = basketTA.GetBasketItems_ByHeaderID_Ticketing(CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID)
            If Not basketItems Is Nothing AndAlso basketItems.Rows.Count > 0 Then
                HttpContext.Current.Session("CanClearAllSessions") = True
                HttpContext.Current.Response.Redirect("~/Redirect/TicketingGateway.aspx?page=Basket.aspx&function=ClearBasket&basketHeaderId=" & CType(HttpContext.Current.Profile, TalentProfile).Basket.Basket_Header_ID & "&returnUrl=" & HttpContext.Current.Server.UrlEncode(sPath))
            Else
                Talent.eCommerce.Utilities.ClearAllSessions()
                HttpContext.Current.Response.Redirect(sPath)
            End If
        Catch ex As Exception
        Finally
            basketTA.Dispose()
        End Try
    End Sub

End Class

Public Class TalentProfileUser

    Private _userDetails As TalentProfileUserDetails
    Public Property Details() As TalentProfileUserDetails
        Get
            Return _userDetails
        End Get
        Set(ByVal value As TalentProfileUserDetails)
            _userDetails = value
        End Set
    End Property

    Private _addresses As New System.Collections.Generic.Dictionary(Of String, TalentProfileAddress)
    Public Property Addresses() As System.Collections.Generic.Dictionary(Of String, TalentProfileAddress)
        Get
            Return _addresses
        End Get
        Set(ByVal value As System.Collections.Generic.Dictionary(Of String, TalentProfileAddress))
            _addresses = value
        End Set
    End Property

End Class

Public Class TalentProfileUserDetails

    Private _isdirty As Boolean
    Public Property IsDirty() As Boolean
        Get
            Return _isdirty
        End Get
        Set(ByVal value As Boolean)
            _isdirty = value
        End Set
    End Property

    Private _loginID As String
    <SettingsAllowAnonymous(False)> _
    Public Property LoginID() As String
        Get
            Return _loginID
        End Get
        Set(ByVal value As String)
            _loginID = value
            IsDirty = True
        End Set
    End Property

    Private _partner_User_Id As String
    <SettingsAllowAnonymous(False)> _
    Public Property Partner_User_Id() As String
        Get
            Return _partner_User_Id
        End Get
        Set(ByVal value As String)
            _partner_User_Id = value
            IsDirty = True
        End Set
    End Property

    Private _user_Number As String
    <SettingsAllowAnonymous(False)> _
    Public Property User_Number() As String
        Get
            Return _user_Number
        End Get
        Set(ByVal value As String)
            _user_Number = value
            IsDirty = True
        End Set
    End Property

    Private _account_No_1 As String
    <SettingsAllowAnonymous(False)> _
    Public Property Account_No_1() As String
        Get
            Return _account_No_1
        End Get
        Set(ByVal value As String)
            _account_No_1 = value
            IsDirty = True
        End Set
    End Property

    Private _account_No_2 As String
    <SettingsAllowAnonymous(False)> _
    Public Property Account_No_2() As String
        Get
            Return _account_No_2
        End Get
        Set(ByVal value As String)
            _account_No_2 = value
            IsDirty = True
        End Set
    End Property

    Private _email As String
    <SettingsAllowAnonymous(False)> _
    Public Property Email() As String
        Get
            Return _email
        End Get
        Set(ByVal value As String)
            _email = value
            IsDirty = True
        End Set
    End Property

    Private _title As String
    <SettingsAllowAnonymous(False)> _
    Public Property Title() As String
        Get
            Return _title
        End Get
        Set(ByVal value As String)
            _title = value
            IsDirty = True
        End Set
    End Property


    Private _initials As String
    <SettingsAllowAnonymous(False)> _
    Public Property Initials() As String
        Get
            Return _initials
        End Get
        Set(ByVal value As String)
            _initials = value
            IsDirty = True
        End Set
    End Property


    Private _forename As String
    <SettingsAllowAnonymous(False)> _
    Public Property Forename() As String
        Get
            Return _forename
        End Get
        Set(ByVal value As String)
            _forename = value
            IsDirty = True
        End Set
    End Property


    Private _surname As String
    <SettingsAllowAnonymous(False)> _
    Public Property Surname() As String
        Get
            Return _surname
        End Get
        Set(ByVal value As String)
            _surname = value
            IsDirty = True
        End Set
    End Property

    Private _fullname As String
    <SettingsAllowAnonymous(False)> _
    Public Property Full_Name() As String
        Get
            Return _fullname
        End Get
        Set(ByVal value As String)
            _fullname = value
            IsDirty = True
        End Set
    End Property


    Private _salutation As String
    <SettingsAllowAnonymous(False)> _
    Public Property Salutation() As String
        Get
            Return _salutation
        End Get
        Set(ByVal value As String)
            _salutation = value
            IsDirty = True
        End Set
    End Property

    Private _position As String
    <SettingsAllowAnonymous(False)> _
    Public Property Position() As String
        Get
            Return _position
        End Get
        Set(ByVal value As String)
            _position = value
            IsDirty = True
        End Set
    End Property

    Private _companyName As String
    <SettingsAllowAnonymous(False)> _
    Public Property CompanyName() As String
        Get
            Return _companyName
        End Get
        Set(ByVal value As String)
            _companyName = value
            IsDirty = True
        End Set
    End Property

    Private _dob As Date
    <SettingsAllowAnonymous(False)> _
    Public Property DOB() As Date
        Get
            Return _dob
        End Get
        Set(ByVal value As Date)
            _dob = value
            IsDirty = True
        End Set
    End Property

    Private _mobile As String
    <SettingsAllowAnonymous(False)> _
    Public Property Mobile_Number() As String
        Get
            Return _mobile
        End Get
        Set(ByVal value As String)
            _mobile = value
            IsDirty = True
        End Set
    End Property

    Private _telephone As String
    <SettingsAllowAnonymous(False)> _
    Public Property Telephone_Number() As String
        Get
            Return _telephone
        End Get
        Set(ByVal value As String)
            _telephone = value
            IsDirty = True
        End Set
    End Property

    Private _work As String
    <SettingsAllowAnonymous(False)> _
    Public Property Work_Number() As String
        Get
            Return _work
        End Get
        Set(ByVal value As String)
            _work = value
            IsDirty = True
        End Set
    End Property

    Private _fax As String
    <SettingsAllowAnonymous(False)> _
    Public Property Fax_Number() As String
        Get
            Return _fax
        End Get
        Set(ByVal value As String)
            _fax = value
            IsDirty = True
        End Set
    End Property

    Private _other As String
    <SettingsAllowAnonymous(False)> _
    Public Property Other_Number() As String
        Get
            Return _other
        End Get
        Set(ByVal value As String)
            _other = value
            IsDirty = True
        End Set
    End Property

    Private _messagingID As String
    <SettingsAllowAnonymous(False)> _
    Public Property Messaging_ID() As String
        Get
            Return _messagingID
        End Get
        Set(ByVal value As String)
            _messagingID = value
            IsDirty = True
        End Set
    End Property


    Private _newsletter As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property Subscribe_Newsletter() As Boolean
        Get
            Return _newsletter
        End Get
        Set(ByVal value As Boolean)
            _newsletter = value
            IsDirty = True
        End Set
    End Property

    Private _htmlnewsletter As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property HTML_Newsletter() As Boolean
        Get
            Return _htmlnewsletter
        End Get
        Set(ByVal value As Boolean)
            _htmlnewsletter = value
            IsDirty = True
        End Set
    End Property

    Private _subscribe2 As Boolean
    <SettingsAllowAnonymous(False)>
    Public Property Subscribe_2() As Boolean
        Get
            Return _subscribe2
        End Get
        Set(ByVal value As Boolean)
            _subscribe2 = value
            IsDirty = True
        End Set
    End Property

    Private _subscribe21 As Boolean
    <SettingsAllowAnonymous(False)>
    Public Property Subscribe_21() As Boolean
        Get
            Return _subscribe21
        End Get
        Set(ByVal value As Boolean)
            _subscribe21 = value
            IsDirty = True
        End Set
    End Property

    Private _subscribe3 As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property Subscribe_3() As Boolean
        Get
            Return _subscribe3
        End Get
        Set(ByVal value As Boolean)
            _subscribe3 = value
            IsDirty = True
        End Set
    End Property

    Private _subscribe31 As Boolean
    <SettingsAllowAnonymous(False)>
    Public Property Subscribe_31() As Boolean
        Get
            Return _subscribe31
        End Get
        Set(ByVal value As Boolean)
            _subscribe31 = value
            IsDirty = True
        End Set
    End Property

    Private _sex As String
    Public Property Sex() As String
        Get
            Return _sex
        End Get
        Set(ByVal value As String)
            _sex = value
            IsDirty = True
        End Set
    End Property

    Private _mothers_Name As String
    Public Property Mothers_Name() As String
        Get
            Return _mothers_Name
        End Get
        Set(ByVal value As String)
            _mothers_Name = value
            IsDirty = True
        End Set
    End Property

    Private _fathers_Name As String
    Public Property Fathers_Name() As String
        Get
            Return _fathers_Name
        End Get
        Set(ByVal value As String)
            _fathers_Name = value
            IsDirty = True
        End Set
    End Property

    Private _bit0 As Boolean
    Public Property Bit0() As Boolean
        Get
            Return _bit0
        End Get
        Set(ByVal value As Boolean)
            _bit0 = value
        End Set
    End Property

    Private _bit1 As Boolean
    Public Property Bit1() As Boolean
        Get
            Return _bit1
        End Get
        Set(ByVal value As Boolean)
            _bit1 = value
        End Set
    End Property

    Private _bit2 As Boolean
    Public Property Bit2() As Boolean
        Get
            Return _bit2
        End Get
        Set(ByVal value As Boolean)
            _bit2 = value
        End Set
    End Property

    Private _bit3 As Boolean
    Public Property Bit3() As Boolean
        Get
            Return _bit3
        End Get
        Set(ByVal value As Boolean)
            _bit3 = value
        End Set
    End Property

    Private _bit4 As Boolean
    Public Property Bit4() As Boolean
        Get
            Return _bit4
        End Get
        Set(ByVal value As Boolean)
            _bit4 = value
        End Set
    End Property

    Private _bit5 As Boolean
    Public Property Bit5() As Boolean
        Get
            Return _bit5
        End Get
        Set(ByVal value As Boolean)
            _bit5 = value
        End Set
    End Property


    Private _user_Number_Prefix As String
    <SettingsAllowAnonymous(False)> _
    Public Property User_Number_Prefix() As String
        Get
            Return _user_Number_Prefix
        End Get
        Set(ByVal value As String)
            _user_Number_Prefix = value
            IsDirty = True
        End Set
    End Property

    Private _Restricted_Payment_Method As String
    <SettingsAllowAnonymous(False)> _
    Public Property RESTRICTED_PAYMENT_METHOD() As String
        Get
            Return _Restricted_Payment_Method
        End Get
        Set(ByVal value As String)
            _Restricted_Payment_Method = value
        End Set
    End Property

    Private _ticketingLoyaltyPoints As Long
    <SettingsAllowAnonymous(False)> _
    Public Property Ticketing_Loyalty_Points() As Long
        Get
            Return _ticketingLoyaltyPoints
        End Get
        Set(ByVal value As Long)
            _ticketingLoyaltyPoints = value
        End Set
    End Property

    Private _attributesList As String = ""
    Public Property ATTRIBUTES_LIST() As String
        Get
            Return _attributesList
        End Get
        Set(ByVal value As String)
            _attributesList = value
        End Set
    End Property

    Private _bondHolder As Boolean
    Public Property BOND_HOLDER() As Boolean
        Get
            Return _bondHolder
        End Get
        Set(ByVal value As Boolean)
            _bondHolder = value
        End Set
    End Property

    Private _ownsAutoMembership As Boolean
    Public Property OWNS_AUTO_MEMBERSHIP() As Boolean
        Get
            Return _ownsAutoMembership
        End Get
        Set(ByVal value As Boolean)
            _ownsAutoMembership = value
        End Set
    End Property


    Private _defaultClub As String
    Public Property DEFAULT_CLUB() As String
        Get
            Return _defaultClub
        End Get
        Set(ByVal value As String)
            _defaultClub = value
        End Set
    End Property

    Private _enable_price_view As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property Enable_Price_View() As Boolean
        Get
            Return _enable_price_view
        End Get
        Set(ByVal value As Boolean)
            _enable_price_view = value
            IsDirty = True
        End Set
    End Property

    Private _SUPPORTER_CLUB_CODE As String
    Public Property SUPPORTER_CLUB_CODE() As String
        Get
            Return _SUPPORTER_CLUB_CODE
        End Get
        Set(ByVal value As String)
            _SUPPORTER_CLUB_CODE = value
        End Set
    End Property


    Private _FAVOURITE_TEAM_CODE As String
    Public Property FAVOURITE_TEAM_CODE() As String
        Get
            Return _FAVOURITE_TEAM_CODE
        End Get
        Set(ByVal value As String)
            _FAVOURITE_TEAM_CODE = value
        End Set
    End Property

    Private _FAVOURITE_SPORT As String
    Public Property FAVOURITE_SPORT() As String
        Get
            Return _FAVOURITE_SPORT
        End Get
        Set(ByVal value As String)
            _FAVOURITE_SPORT = value
        End Set
    End Property

    Private _MAIL_TEAM_CODE_1 As String
    Public Property MAIL_TEAM_CODE_1() As String
        Get
            Return _MAIL_TEAM_CODE_1
        End Get
        Set(ByVal value As String)
            _MAIL_TEAM_CODE_1 = value
        End Set
    End Property

    Private _MAIL_TEAM_CODE_2 As String
    Public Property MAIL_TEAM_CODE_2() As String
        Get
            Return _MAIL_TEAM_CODE_2
        End Get
        Set(ByVal value As String)
            _MAIL_TEAM_CODE_2 = value
        End Set
    End Property

    Private _MAIL_TEAM_CODE_3 As String
    Public Property MAIL_TEAM_CODE_3() As String
        Get
            Return _MAIL_TEAM_CODE_3
        End Get
        Set(ByVal value As String)
            _MAIL_TEAM_CODE_3 = value
        End Set
    End Property

    Private _MAIL_TEAM_CODE_4 As String
    Public Property MAIL_TEAM_CODE_4() As String
        Get
            Return _MAIL_TEAM_CODE_4
        End Get
        Set(ByVal value As String)
            _MAIL_TEAM_CODE_4 = value
        End Set
    End Property


    Private _MAIL_TEAM_CODE_5 As String
    Public Property MAIL_TEAM_CODE_5() As String
        Get
            Return _MAIL_TEAM_CODE_5
        End Get
        Set(ByVal value As String)
            _MAIL_TEAM_CODE_5 = value
        End Set
    End Property

    Private _PREFERRED_CONTACT_METHOD As String
    Public Property PREFERRED_CONTACT_METHOD() As String
        Get
            Return _PREFERRED_CONTACT_METHOD
        End Get
        Set(ByVal value As String)
            _PREFERRED_CONTACT_METHOD = value
        End Set
    End Property

    Private _minimum_Purchase_Quantity As Decimal
    <SettingsAllowAnonymous(False)> _
    Public Property Minimum_Purchase_Quantity() As Decimal
        Get
            Return _minimum_Purchase_Quantity
        End Get
        Set(ByVal value As Decimal)
            _minimum_Purchase_Quantity = value
            IsDirty = True
        End Set
    End Property

    Private _use_Minimum_Purchase_Quantity As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property Use_Minimum_Purchase_Quantity() As Boolean
        Get
            Return _use_Minimum_Purchase_Quantity
        End Get
        Set(ByVal value As Boolean)
            _use_Minimum_Purchase_Quantity = value
            IsDirty = True
        End Set
    End Property
    Private _minimum_Purchase_Amount As Decimal
    <SettingsAllowAnonymous(False)> _
    Public Property Minimum_Purchase_Amount() As Decimal
        Get
            Return _minimum_Purchase_Amount
        End Get
        Set(ByVal value As Decimal)
            _minimum_Purchase_Amount = value
            IsDirty = True
        End Set
    End Property
    Private _use_Minimum_Purchase_Amount As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property Use_Minimum_Purchase_Amount() As Boolean
        Get
            Return _use_Minimum_Purchase_Amount
        End Get
        Set(ByVal value As Boolean)
            _use_Minimum_Purchase_Amount = value
            IsDirty = True
        End Set
    End Property
    Private _passport As String
    <SettingsAllowAnonymous(False)> _
    Public Property Passport() As String
        Get
            Return _passport
        End Get
        Set(ByVal value As String)
            _passport = value
            IsDirty = True
        End Set
    End Property

    Private _greenCard As String
    <SettingsAllowAnonymous(False)> _
    Public Property GreenCard() As String
        Get
            Return _greenCard
        End Get
        Set(ByVal value As String)
            _greenCard = value
            IsDirty = True
        End Set
    End Property

    Private _PIN As String
    <SettingsAllowAnonymous(False)> _
    Public Property PIN() As String
        Get
            Return _PIN
        End Get
        Set(ByVal value As String)
            _PIN = value
            IsDirty = True
        End Set
    End Property

    Private _userID4 As String
    <SettingsAllowAnonymous(False)> _
    Public Property User_ID_4() As String
        Get
            Return _userID4
        End Get
        Set(ByVal value As String)
            _userID4 = value
            IsDirty = True
        End Set
    End Property

    Private _userID5 As String
    <SettingsAllowAnonymous(False)> _
    Public Property User_ID_5() As String
        Get
            Return _userID5
        End Get
        Set(ByVal value As String)
            _userID5 = value
            IsDirty = True
        End Set
    End Property

    Private _userID6 As String
    <SettingsAllowAnonymous(False)> _
    Public Property User_ID_6() As String
        Get
            Return _userID6
        End Get
        Set(ByVal value As String)
            _userID6 = value
            IsDirty = True
        End Set
    End Property

    Private _userID7 As String
    <SettingsAllowAnonymous(False)> _
    Public Property User_ID_7() As String
        Get
            Return _userID7
        End Get
        Set(ByVal value As String)
            _userID7 = value
            IsDirty = True
        End Set
    End Property

    Private _userID8 As String
    <SettingsAllowAnonymous(False)> _
    Public Property User_ID_8() As String
        Get
            Return _userID8
        End Get
        Set(ByVal value As String)
            _userID8 = value
            IsDirty = True
        End Set
    End Property

    Private _userID9 As String
    <SettingsAllowAnonymous(False)> _
    Public Property User_ID_9() As String
        Get
            Return _userID9
        End Get
        Set(ByVal value As String)
            _userID9 = value
            IsDirty = True
        End Set
    End Property

    Private _favouriteSeat As String
    <SettingsAllowAnonymous(False)> _
    Public Property Favourite_Seat() As String
        Get
            Return _favouriteSeat
        End Get
        Set(ByVal value As String)
            _favouriteSeat = value
            IsDirty = True
        End Set
    End Property

    Private _SmartcardNumber As String
    <SettingsAllowAnonymous(False)> _
    Public Property Smartcard_Number() As String
        Get
            Return _SmartcardNumber
        End Get
        Set(ByVal value As String)
            _SmartcardNumber = value
            IsDirty = True
        End Set
    End Property

    'Agent Mode Fields


    Private _stop_code As String
    <SettingsAllowAnonymous(False)> _
    Public Property STOP_CODE() As String
        Get
            Return _stop_code
        End Get
        Set(ByVal value As String)
            _stop_code = value
            IsDirty = True
        End Set
    End Property

    Private _priceBand As String
    <SettingsAllowAnonymous(False)> _
    Public Property PRICE_BAND() As String
        Get
            Return _priceBand
        End Get
        Set(ByVal value As String)
            _priceBand = value
            IsDirty = True
        End Set
    End Property


    Private _contact_by_post As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property CONTACT_BY_POST() As Boolean
        Get
            Return _contact_by_post
        End Get
        Set(ByVal value As Boolean)
            _contact_by_post = value
            IsDirty = True
        End Set
    End Property

    Private _contact_by_telephone_home As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property CONTACT_BY_TELEPHONE_HOME() As Boolean
        Get
            Return _contact_by_telephone_home
        End Get
        Set(ByVal value As Boolean)
            _contact_by_telephone_home = value
            IsDirty = True
        End Set
    End Property

    Private _contact_by_telephone_work As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property CONTACT_BY_TELEPHONE_WORK() As Boolean
        Get
            Return _contact_by_telephone_work
        End Get
        Set(ByVal value As Boolean)
            _contact_by_telephone_work = value
            IsDirty = True
        End Set
    End Property

    Private _contact_by_mobile As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property CONTACT_BY_MOBILE() As Boolean
        Get
            Return _contact_by_mobile
        End Get
        Set(ByVal value As Boolean)
            _contact_by_mobile = value
            IsDirty = True
        End Set
    End Property

    Private _contact_by_email As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property CONTACT_BY_EMAIL() As Boolean
        Get
            Return _contact_by_email
        End Get
        Set(ByVal value As Boolean)
            _contact_by_email = value
            IsDirty = True
        End Set
    End Property

    Private _book_Number As String
    <SettingsAllowAnonymous(False)> _
    Public Property BOOK_NUMBER() As String
        Get
            Return _book_Number
        End Get
        Set(ByVal value As String)
            _book_Number = value
            IsDirty = True
        End Set
    End Property

    Private _customer_suffix As String
    <SettingsAllowAnonymous(False)> _
    Public Property CUSTOMER_SUFFIX() As String
        Get
            Return _customer_suffix
        End Get
        Set(ByVal value As String)
            _customer_suffix = value
            IsDirty = True
        End Set
    End Property

    Private _nickName As String
    <SettingsAllowAnonymous(False)> _
    Public Property NICKNAME() As String
        Get
            Return _nickName
        End Get
        Set(ByVal value As String)
            _nickName = value
            IsDirty = True
        End Set
    End Property

    Private _userName As String
    <SettingsAllowAnonymous(False)> _
    Public Property USERNAME() As String
        Get
            Return _userName
        End Get
        Set(ByVal value As String)
            _userName = value
            IsDirty = True
        End Set
    End Property

    Private _parental_consent_status As String
    Public Property PARENTAL_CONSENT_STATUS() As String
        Get
            Return _parental_consent_status
        End Get
        Set(ByVal value As String)
            _parental_consent_status = value
            IsDirty = True
        End Set
    End Property

    Private _parent_phone As String
    Public Property PARENT_PHONE() As String
        Get
            Return _parent_phone
        End Get
        Set(ByVal value As String)
            _parent_phone = value
            IsDirty = True
        End Set
    End Property
    Private _parent_email As String
    Public Property PARENT_EMAIL() As String
        Get
            Return _parent_email
        End Get
        Set(ByVal value As String)
            _parent_email = value
            IsDirty = True
        End Set
    End Property

End Class

Public Class TalentProfilePartner

    Private _partnerDetails As TalentProfilePartnerDetails
    Public Property Details() As TalentProfilePartnerDetails
        Get
            Return _partnerDetails
        End Get
        Set(ByVal value As TalentProfilePartnerDetails)
            _partnerDetails = value
        End Set
    End Property

    Private _addresses As New System.Collections.Generic.Dictionary(Of String, TalentProfileAddress)
    Public Property Addresses() As System.Collections.Generic.Dictionary(Of String, TalentProfileAddress)
        Get
            Return _addresses
        End Get
        Set(ByVal value As System.Collections.Generic.Dictionary(Of String, TalentProfileAddress))
            _addresses = value
        End Set
    End Property

End Class

Public Class TalentProfilePartnerDetails
    Private _isdirty As Boolean
    Public Property IsDirty() As Boolean
        Get
            Return _isdirty
        End Get
        Set(ByVal value As Boolean)
            _isdirty = value
        End Set
    End Property


    Private _partnerID As String
    Public Property Partner_ID() As String
        Get
            Return _partnerID
        End Get
        Set(ByVal value As String)
            _partnerID = value
            IsDirty = True
        End Set
    End Property


    Private _partner As String
    Public Property Partner() As String
        Get
            Return _partner
        End Get
        Set(ByVal value As String)
            _partner = value
            IsDirty = True
        End Set
    End Property

    Private _partnerdesc As String
    Public Property Partner_Desc() As String
        Get
            Return _partnerdesc
        End Get
        Set(ByVal value As String)
            _partnerdesc = value
            IsDirty = True
        End Set
    End Property

    Private _destDB As String
    Public Property Destination_Database() As String
        Get
            Return _destDB
        End Get
        Set(ByVal value As String)
            _destDB = value
            IsDirty = True
        End Set
    End Property

    Private _cacheEnabled As Boolean
    Public Property Caching_Enabled() As Boolean
        Get
            Return _cacheEnabled
        End Get
        Set(ByVal value As Boolean)
            _cacheEnabled = value
            IsDirty = True
        End Set
    End Property

    Private _cacheMinutes As String
    Public Property Cache_Time_Minutes() As String
        Get
            Return _cacheMinutes
        End Get
        Set(ByVal value As String)
            _cacheMinutes = value
            IsDirty = True
        End Set
    End Property


    Private _logging As Boolean
    Public Property Logging_Enabled() As Boolean
        Get
            Return _logging
        End Get
        Set(ByVal value As Boolean)
            _logging = value
            IsDirty = True
        End Set
    End Property

    Private _storeXML As Boolean
    Public Property Store_XML() As Boolean
        Get
            Return _storeXML
        End Get
        Set(ByVal value As Boolean)
            _storeXML = value
            IsDirty = True
        End Set
    End Property

    Private _accountNo1 As String
    Public Property Account_No_1() As String
        Get
            Return _accountNo1
        End Get
        Set(ByVal value As String)
            _accountNo1 = value
            IsDirty = True
        End Set
    End Property

    Private _accountNo2 As String
    Public Property Account_No_2() As String
        Get
            Return _accountNo2
        End Get
        Set(ByVal value As String)
            _accountNo2 = value
            IsDirty = True
        End Set
    End Property

    Private _accountNo3 As String
    Public Property Account_No_3() As String
        Get
            Return _accountNo3
        End Get
        Set(ByVal value As String)
            _accountNo3 = value
            IsDirty = True
        End Set
    End Property


    Private _accountNo4 As String
    Public Property Account_No_4() As String
        Get
            Return _accountNo4
        End Get
        Set(ByVal value As String)
            _accountNo4 = value
            IsDirty = True
        End Set
    End Property


    Private _accountNo5 As String
    Public Property Account_No_5() As String
        Get
            Return _accountNo5
        End Get
        Set(ByVal value As String)
            _accountNo5 = value
            IsDirty = True
        End Set
    End Property


    Private _email As String
    Public Property Email() As String
        Get
            Return _email
        End Get
        Set(ByVal value As String)
            _email = value
            IsDirty = True
        End Set
    End Property

    Private _phoneNo As String
    Public Property Telephone_Number() As String
        Get
            Return _phoneNo
        End Get
        Set(ByVal value As String)
            _phoneNo = value
            IsDirty = True
        End Set
    End Property

    Private _faxNo As String
    Public Property Fax_Number() As String
        Get
            Return _faxNo
        End Get
        Set(ByVal value As String)
            _faxNo = value
            IsDirty = True
        End Set
    End Property

    Private _partnerURL As String
    Public Property Partner_URL() As String
        Get
            Return _partnerURL
        End Get
        Set(ByVal value As String)
            _partnerURL = value
            IsDirty = True
        End Set
    End Property
    Private _crmBranch As String
    Public Property CRM_Branch() As String
        Get
            Return _crmBranch
        End Get
        Set(ByVal value As String)
            _crmBranch = value
            IsDirty = True
        End Set
    End Property
    Private _partnerNumber As String
    Public Property Partner_Number() As String
        Get
            Return _partnerNumber
        End Get
        Set(ByVal value As String)
            _partnerNumber = value
            IsDirty = True
        End Set
    End Property

    Private _vatNumber As String
    Public Property VAT_NUMBER() As String
        Get
            Return _vatNumber
        End Get
        Set(ByVal value As String)
            _vatNumber = value
            IsDirty = False
        End Set
    End Property

    Private _enable_price_view As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property Enable_Price_View() As Boolean
        Get
            Return _enable_price_view
        End Get
        Set(ByVal value As Boolean)
            _enable_price_view = value
            IsDirty = True
        End Set
    End Property

    Private _order_enquiry_show_partner_orders As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property Order_Enquiry_Show_Partner_Orders() As Boolean
        Get
            Return _order_enquiry_show_partner_orders
        End Get
        Set(ByVal value As Boolean)
            _order_enquiry_show_partner_orders = value
            IsDirty = True
        End Set
    End Property

    Private _minimum_Purchase_Quantity As Decimal
    <SettingsAllowAnonymous(False)> _
    Public Property Minimum_Purchase_Quantity() As Decimal
        Get
            Return _minimum_Purchase_Quantity
        End Get
        Set(ByVal value As Decimal)
            _minimum_Purchase_Quantity = value
        End Set
    End Property

    Private _use_Minimum_Purchase_Quantity As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property Use_Minimum_Purchase_Quantity() As Boolean
        Get
            Return _use_Minimum_Purchase_Quantity
        End Get
        Set(ByVal value As Boolean)
            _use_Minimum_Purchase_Quantity = value
        End Set
    End Property
    Private _minimum_Purchase_Amount As Decimal
    <SettingsAllowAnonymous(False)> _
    Public Property Minimum_Purchase_Amount() As Decimal
        Get
            Return _minimum_Purchase_Amount
        End Get
        Set(ByVal value As Decimal)
            _minimum_Purchase_Amount = value
        End Set
    End Property

    Private _use_Minimum_Purchase_Amount As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property Use_Minimum_Purchase_Amount() As Boolean
        Get
            Return _use_Minimum_Purchase_Amount
        End Get
        Set(ByVal value As Boolean)
            _use_Minimum_Purchase_Amount = value
        End Set
    End Property

    Private _enable_alternate_SKU As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property Enable_Alternate_SKU() As Boolean
        Get
            Return _enable_alternate_SKU
        End Get
        Set(ByVal value As Boolean)
            _enable_alternate_SKU = value
            IsDirty = True
        End Set
    End Property

    Private _costCentre As String
    <SettingsAllowAnonymous(False)> _
    Public Property COST_CENTRE() As String
        Get
            Return _costCentre
        End Get
        Set(ByVal value As String)
            _costCentre = value
            IsDirty = True
        End Set
    End Property

    Private _restrictDirectAccess As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property RESTRICT_DIRECT_ACCESS() As Boolean
        Get
            Return _restrictDirectAccess
        End Get
        Set(ByVal value As Boolean)
            _restrictDirectAccess = value
        End Set
    End Property

    Private _showPreferredDeliveryDate As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property SHOW_PREFERRED_DELIVERY_DATE() As Boolean
        Get
            Return _showPreferredDeliveryDate
        End Get
        Set(ByVal value As Boolean)
            _showPreferredDeliveryDate = value
        End Set
    End Property

    Private _partnerType As String
    <SettingsAllowAnonymous(False)> _
    Public Property PARTNER_TYPE() As String
        Get
            Return _partnerType
        End Get
        Set(ByVal value As String)
            _partnerType = value
        End Set
    End Property

    Private _orderConfirmationEmail As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property ORDER_CONFIRMATION_EMAIL() As Boolean
        Get
            Return _orderConfirmationEmail
        End Get
        Set(ByVal value As Boolean)
            _orderConfirmationEmail = value
        End Set
    End Property

    Private _hidePrices As Boolean
    <SettingsAllowAnonymous(False)> _
    Public Property HIDE_PRICES() As Boolean
        Get
            Return _hidePrices
        End Get
        Set(ByVal value As Boolean)
            _hidePrices = value
        End Set
    End Property

    Private _carrierCode As String
    <SettingsAllowAnonymous(False)> _
    Public Property CARRIER_CODE() As String
        Get
            Return _carrierCode
        End Get
        Set(ByVal value As String)
            _carrierCode = value
        End Set
    End Property
   

End Class

Public Class TalentProfileAddress


    Private _isdirty As Boolean
    Public Property IsDirty() As Boolean
        Get
            Return _isdirty
        End Get
        Set(ByVal value As Boolean)
            _isdirty = value
        End Set
    End Property


    Private _addressID As Long
    Public Property Address_ID() As Long
        Get
            Return _addressID
        End Get
        Set(ByVal value As Long)
            _addressID = value
        End Set
    End Property


    Private _loginID As String
    Public Property LoginID() As String
        Get
            Return _loginID
        End Get
        Set(ByVal value As String)
            _loginID = value
            IsDirty = True
        End Set
    End Property


    Private _type As String
    Public Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
            IsDirty = True
        End Set
    End Property


    Private _ref As String
    Public Property Reference() As String
        Get
            Return _ref
        End Get
        Set(ByVal value As String)
            _ref = value
            IsDirty = True
        End Set
    End Property


    Private _seq As Integer
    Public Property Sequence() As Integer
        Get
            Return _seq
        End Get
        Set(ByVal value As Integer)
            _seq = value
            IsDirty = True
        End Set
    End Property


    Private _default As Boolean
    Public Property Default_Address() As Boolean
        Get
            Return _default
        End Get
        Set(ByVal value As Boolean)
            _default = value
            IsDirty = True
        End Set
    End Property


    Private _line1 As String
    Public Property Address_Line_1() As String
        Get
            Return _line1
        End Get
        Set(ByVal value As String)
            _line1 = value
            IsDirty = True
        End Set
    End Property


    Private _line2 As String
    Public Property Address_Line_2() As String
        Get
            Return _line2
        End Get
        Set(ByVal value As String)
            _line2 = value
            IsDirty = True
        End Set
    End Property


    Private _line3 As String
    Public Property Address_Line_3() As String
        Get
            Return _line3
        End Get
        Set(ByVal value As String)
            _line3 = value
            IsDirty = True
        End Set
    End Property


    Private _line4 As String
    Public Property Address_Line_4() As String
        Get
            Return _line4
        End Get
        Set(ByVal value As String)
            _line4 = value
            IsDirty = True
        End Set
    End Property


    Private _line5 As String
    Public Property Address_Line_5() As String
        Get
            Return _line5
        End Get
        Set(ByVal value As String)
            _line5 = value
            IsDirty = True
        End Set
    End Property


    Private _postcode As String
    Public Property Post_Code() As String
        Get
            Return _postcode
        End Get
        Set(ByVal value As String)
            _postcode = value
            IsDirty = True
        End Set
    End Property


    Private _country As String
    Public Property Country() As String
        Get
            Return _country
        End Get
        Set(ByVal value As String)
            _country = value
            IsDirty = True
        End Set
    End Property

    Private _deliveryZoneCode As String
    Public Property Delivery_Zone_Code() As String
        Get
            Return _deliveryZoneCode
        End Get
        Set(ByVal value As String)
            _deliveryZoneCode = value
            IsDirty = True
        End Set
    End Property

    Private _externalID As String
    Public Property External_ID() As String
        Get
            Return _externalID
        End Get
        Set(ByVal value As String)
            _externalID = value
            IsDirty = True
        End Set
    End Property

End Class

Public Class ProfileHelper

    Public Shared Function GetPageName() As String
        Dim pagename As String = HttpContext.Current.Request.Url.AbsolutePath
        pagename = pagename.Split("/")(pagename.Split("/").Length - 1)
        Return pagename
    End Function

    Public Shared Function GetPropertyNames(ByVal obj As Object) As ArrayList
        Dim al As New ArrayList
        Dim inf() As System.Reflection.PropertyInfo = obj.GetType.GetProperties
        For Each info As System.Reflection.PropertyInfo In inf
            al.Add(info.Name)
        Next
        Return al
    End Function

    Public Shared Function CheckDBNull(ByVal value As Object) As Object
        If value.Equals(DBNull.Value) Then
            Return Nothing
        Else
            Return value
        End If
    End Function

    Public Shared Function CheckDBNull(ByVal value As Object, ByVal defaultReturnObject As Object) As Object
        If value.Equals(DBNull.Value) Then
            Return defaultReturnObject
        Else
            Return value
        End If
    End Function

    Public Shared Function ProfileAddressEnumerator(ByVal index As Integer, ByVal addresses As Generic.Dictionary(Of String, TalentProfileAddress)) As TalentProfileAddress
        Dim address As New TalentProfileAddress
        Try
            Dim enm As Collections.Generic.Dictionary(Of String, TalentProfileAddress).KeyCollection.Enumerator
            enm = addresses.Keys.GetEnumerator
            Dim count As Integer = 0
            While enm.MoveNext
                If count = index Then
                    address = addresses(enm.Current)
                    Exit While
                End If
                count += 1
            End While
        Catch ex As Exception
            address = New TalentProfileAddress
        End Try

        Return address
    End Function

    Public Shared Function GetAge(ByVal DOB As Date) As Integer
        Dim age As Integer = 0
        If DOB.Month < Now.Month Then
            'if the dob month has passed for this year
            'age = current year - dob year
            age = Now.Year - DOB.Year
        ElseIf DOB.Month = Now.Month Then
            'if dob month is = to current month then check
            'to see if the dob day has passed
            If DOB.Day <= Now.Day Then
                'if b'day passed or today, age is current year - dob year
                age = Now.Year - DOB.Year
            Else
                'if b'day not passed, age is now (year - dob year) -1
                age = (Now.Year - DOB.Year) - 1
            End If
        ElseIf DOB.Month > Now.Month Then
            'if b'day not passed age is now (year - dob year) -1
            age = (Now.Year - DOB.Year) - 1
        End If
        Return age
    End Function

End Class