Imports Microsoft.VisualBasic
Imports System.Web.Profile
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce

Public Class TalentProfileProvider
    Inherits ProfileProvider

    Private _pricingModuleDefaultsValues As ECommerceModuleDefaults.DefaultValues
    Private _defaultPriceList As String = String.Empty
    Private _userAttributeList As String = ""

    Private _ApplicationName As String
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Public Overrides Property ApplicationName() As String
        Get
            Return _ApplicationName
        End Get
        Set(ByVal value As String)
            _ApplicationName = value
        End Set
    End Property

    Public ReadOnly Property PricingBusinessUnit() As String
        Get
            Return TalentCache.GetBusinessUnit
        End Get
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
    Private _partner As String
    Private Property Partner() As String
        Get
            If _partner Is Nothing Then
                'Return CType(HttpContext.Current.Profile, TalentProfile).PartnerInfo.Details.Partner
                Return TalentCache.GetPartner(HttpContext.Current.Profile)
            Else
                Return _partner
            End If
        End Get
        Set(ByVal value As String)
            _partner = value
        End Set
    End Property



    Public Overrides Sub Initialize(ByVal name As String, ByVal config As System.Collections.Specialized.NameValueCollection)

        'ApplicationName = ConfigurationManager.AppSettings("ApplicationName").ToString
        'If Not String.IsNullOrEmpty(config("applicationName")) Then
        '    ApplicationName = config("applicationName")
        'Else
        '    config.Add("applicationName", ApplicationName)
        'End If
        'BusinessUnit = TalentCache.GetBusinessUnit
        'If Not String.IsNullOrEmpty(config("BusinessUnit")) Then
        '    BusinessUnit = config("BusinessUnit")
        'Else
        '    config.Add("BusinessUnit", BusinessUnit)
        'End If
        'Partner = ConfigurationManager.AppSettings("Partner").ToString
        'If Not String.IsNullOrEmpty(config("Partner")) Then
        '    Partner = config("Partner")
        'Else
        '    config.Add("Partner", Partner)
        'End If
        MyBase.Initialize(name, config)
    End Sub

    Public Overrides Function DeleteInactiveProfiles(ByVal authenticationOption As System.Web.Profile.ProfileAuthenticationOption, ByVal userInactiveSinceDate As Date) As Integer
        Return 0
    End Function

    Public Overloads Overrides Function DeleteProfiles(ByVal usernames() As String) As Integer
        Return 0
    End Function

    Public Overloads Overrides Function DeleteProfiles(ByVal profiles As System.Web.Profile.ProfileInfoCollection) As Integer
        Return 0
    End Function

    Public Overrides Function FindInactiveProfilesByUserName(ByVal authenticationOption As System.Web.Profile.ProfileAuthenticationOption, ByVal usernameToMatch As String, ByVal userInactiveSinceDate As Date, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As System.Web.Profile.ProfileInfoCollection
        Return Nothing
    End Function

    Public Overrides Function FindProfilesByUserName(ByVal authenticationOption As System.Web.Profile.ProfileAuthenticationOption, ByVal usernameToMatch As String, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As System.Web.Profile.ProfileInfoCollection
        Return Nothing
    End Function

    Public Overrides Function GetAllInactiveProfiles(ByVal authenticationOption As System.Web.Profile.ProfileAuthenticationOption, ByVal userInactiveSinceDate As Date, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As System.Web.Profile.ProfileInfoCollection
        Return Nothing
    End Function

    Public Overrides Function GetAllProfiles(ByVal authenticationOption As System.Web.Profile.ProfileAuthenticationOption, ByVal pageIndex As Integer, ByVal pageSize As Integer, ByRef totalRecords As Integer) As System.Web.Profile.ProfileInfoCollection
        Return Nothing
    End Function

    Public Overrides Function GetNumberOfInactiveProfiles(ByVal authenticationOption As System.Web.Profile.ProfileAuthenticationOption, ByVal userInactiveSinceDate As Date) As Integer
        Return 0
    End Function

    Private Sub SetPartner(ByVal IsAuthenticated As Boolean, ByVal username As String)
        Try
            If IsAuthenticated And Not String.IsNullOrEmpty(username) Then
                Dim usersTbl As New TalentMembershipDatasetTableAdapters.tbl_authorized_usersTableAdapter
                Dim dt As Data.DataTable = usersTbl.Get_B2B_GetUserMethod(BusinessUnitGroup, username)

                If dt.Rows.Count > 0 Then
                    Partner = Talent.eCommerce.Utilities.CheckForDBNull_String(dt.Rows(0)("PARTNER"))
                Else

                    Partner = TalentCache.GetDefaultPartner
                    ' BF - Don't signout. Causes problems when updating email address in B2C environment
                    ' (from updateDetailsForm, only tbl_authorized_users record gets updated with new loginid)

                    'FormsAuthentication.SignOut()
                    'HttpContext.Current.Response.Redirect(HttpContext.Current.Request.Url.PathAndQuery)
                End If
            Else
                Dim tDataObjects As New Talent.Common.TalentDataObjects
                tDataObjects.Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                tDataObjects.Settings.BusinessUnit = TalentCache.GetBusinessUnit()
                If (tDataObjects.AppVariableSettings.TblAuthorizedPartners.CheckB2BMode(BusinessUnit)) Then
                    Partner = TalentCache.GetDefaultPartner(BusinessUnit)
                End If
            End If
        Catch ex As Exception
            If IsAuthenticated Then
                HttpContext.Current.Request.Cookies.Remove(".ASPXFORMSAUTH")
            End If
        End Try

    End Sub


    Public Overrides Function GetPropertyValues(ByVal context As System.Configuration.SettingsContext, ByVal collection As System.Configuration.SettingsPropertyCollection) As System.Configuration.SettingsPropertyValueCollection

        'Get the users values
        Dim userName As String = CType(context("UserName"), String)
        Dim isAuthenticated As Boolean = CType(context("IsAuthenticated"), Boolean)
        'userName = Talent.Common.Utilities.PadLeadingZeros(userName, 12)

        SetPartner(isAuthenticated, userName)

        'Create a property collection to populate with the user's values
        Dim TalentPropertyValues As SettingsPropertyValueCollection = New SettingsPropertyValueCollection

        For Each TalentProperty As SettingsProperty In Collection
            TalentPropertyValues.Add(New SettingsPropertyValue(TalentProperty))
        Next

        '** If we wish to retrieve data for 'simple' types on the profile object
        '   add a call to the database here and add code to the Case Else section
        '   to populate the specific property **

        For Each TalentPropertyValue As SettingsPropertyValue In TalentPropertyValues

            Select Case TalentPropertyValue.Name
                Case Is = "User"
                    If isAuthenticated Then 'only do if user is logged in
                        TalentPropertyValue.PropertyValue = GetUserData(userName)
                    Else 'otherwise return an empty user obj
                        TalentPropertyValue.PropertyValue = New TalentProfileUser
                    End If
                Case Is = "PartnerInfo"
                    If isAuthenticated Or Not String.IsNullOrWhiteSpace(Partner) Then 'only do if user is logged in
                        TalentPropertyValue.PropertyValue = GetPartnerData()
                    Else 'otherwise return an empty partner obj
                        TalentPropertyValue.PropertyValue = New TalentProfilePartner
                    End If
                Case Is = "Basket"
                    TalentPropertyValue.PropertyValue = GetBasket(userName, isAuthenticated)
                Case Else

            End Select

        Next


        'UpdateActivityDates(userName, isAuthenticated, True)
        Return TalentPropertyValues
    End Function

    Public Function GetBasket(ByVal username As String, ByVal isAuthenticated As Boolean) As TalentBasket
        Dim basket As TalentBasket = GetBasket(username, isAuthenticated, False)
        Return basket
    End Function

    'CK: todo cleanup required to move all to talentbasketsummary when find some time
    Public Function GetBasket(ByVal username As String, ByVal isAuthenticated As Boolean, ByVal canProcessBookingFees As Boolean) As TalentBasket
        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay
        Dim loginID As String = String.Empty
        'this defaults are used only in web pricing methods 
        _pricingModuleDefaultsValues = (New ECommerceModuleDefaults).GetDefaults(Partner, PricingBusinessUnit)
        If _pricingModuleDefaultsValues.UseGlobalPriceListWithCustomerPriceList Then
            _defaultPriceList = _pricingModuleDefaultsValues.DefaultPriceList
        Else
            _defaultPriceList = _pricingModuleDefaultsValues.PriceList
        End If
        Dim basket As New TalentBasket
        Dim item As New TalentBasketItem
        Dim dt As System.Data.DataTable
        Dim campaignCode As String = String.Empty
        Try
            If HttpContext.Current.Session("CAMPAIGN_CODE") IsNot Nothing Then campaignCode = HttpContext.Current.Session("CAMPAIGN_CODE").ToString
        Catch ex As Exception
            campaignCode = String.Empty
        End Try

        '-------------------------------------
        ' Get the basket header for the user
        '-------------------------------------
        Dim basketHeaderID As String = String.Empty


        Dim tDataObjects As New Talent.Common.TalentDataObjects
        tDataObjects.Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
        tDataObjects.Settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        tDataObjects.Settings.BusinessUnit = BusinessUnit
        tDataObjects.BasketSettings.BasketHelperEntity.BusinessUnit = BusinessUnit
        tDataObjects.BasketSettings.BasketHelperEntity.CampaignCode = campaignCode
        tDataObjects.BasketSettings.BasketHelperEntity.CatMode = ""
        tDataObjects.BasketSettings.BasketHelperEntity.ExternalPaymentToken = ""
        tDataObjects.BasketSettings.BasketHelperEntity.IsAnonymous = HttpContext.Current.Profile.IsAnonymous
        tDataObjects.BasketSettings.BasketHelperEntity.IsAuthenticated = isAuthenticated
        tDataObjects.BasketSettings.BasketHelperEntity.LoginId = username
        tDataObjects.BasketSettings.BasketHelperEntity.MarketingCampaign = False
        tDataObjects.BasketSettings.BasketHelperEntity.RestrictPaymentOptions = False
        tDataObjects.BasketSettings.BasketHelperEntity.Partner = Partner
        tDataObjects.BasketSettings.BasketHelperEntity.PaymentAccountID = ""
        tDataObjects.BasketSettings.BasketHelperEntity.PaymentOptions = ""
        tDataObjects.BasketSettings.BasketHelperEntity.Processed = False
        If HttpContext.Current.Session IsNot Nothing Then
            tDataObjects.BasketSettings.BasketHelperEntity.SessionID = HttpContext.Current.Session.SessionID.ToString
        Else
            tDataObjects.BasketSettings.BasketHelperEntity.SessionID = ""
        End If
        tDataObjects.BasketSettings.BasketHelperEntity.StockError = False
        tDataObjects.BasketSettings.BasketHelperEntity.UserSelectFulfilment = "N"
        Dim profileBasket As DataSet = tDataObjects.BasketSettings.GetBasket()

        If profileBasket IsNot Nothing AndAlso profileBasket.Tables.Count > 0 Then
            'basket header
            dt = profileBasket.Tables(1)
            basketHeaderID = CStr(dt.Rows(0)("BASKET_HEADER_ID"))
            If Utilities.CheckForDBNull_Boolean_DefaultFalse(profileBasket.Tables(0).Rows(0)(0)) Then
                Talent.eCommerce.Logging.WriteLog(username, _
                                                            "NEW BASKET GENERATED", _
                                                                "New Basket Generated - " & basketHeaderID, _
                                                                "", _
                                                                TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner)
            End If
            Dim properties As ArrayList = ProfileHelper.GetPropertyNames(basket)
            basket = PopulateProperties(properties, dt, basket, 0)
            'basket detail
            dt = profileBasket.Tables(2)
            properties = ProfileHelper.GetPropertyNames(item)
            If dt.Rows.Count > 0 Then
                For i As Integer = 0 To dt.Rows.Count - 1
                    item = New TalentBasketItem
                    item = PopulateProperties(properties, dt, item, i)
                    basket.BasketItems.Add(item)
                    If (UCase(item.MODULE_) <> "TICKETING") Then
                        If Not item.IS_FREE Then
                            If (item.MASTER_PRODUCT.Length > 0) Then
                                If Not basket.MasterProducts.ContainsKey(item.MASTER_PRODUCT) Then
                                    basket.MasterProducts.Add(item.MASTER_PRODUCT, New Talent.Common.WebPriceProduct(item.MASTER_PRODUCT, item.Quantity, item.MASTER_PRODUCT))
                                Else
                                    basket.MasterProducts.Item(item.MASTER_PRODUCT).Quantity = basket.MasterProducts.Item(item.MASTER_PRODUCT).Quantity + item.Quantity
                                End If
                            Else
                                If Not basket.MasterProducts.ContainsKey(item.Product) Then
                                    basket.MasterProducts.Add(item.Product, New Talent.Common.WebPriceProduct(item.Product, 0, item.Product))
                                End If
                            End If
                        End If
                    End If
                Next
            End If
            Dim currentPageName As String = (New System.IO.FileInfo(System.Web.HttpContext.Current.Request.Url.AbsolutePath)).Name.ToLower
            'calulate the web price for retail items in the basket
            basket = CalculateWebPricing(username, isAuthenticated, basket)
            'modify the web price totals
            basket = GetModifiedWebPrices(_pricingModuleDefaultsValues.WebPricesModifyingMode, basket, currentPageName)

            Dim talBasketSummary As New Talent.Common.TalentBasketSummary

            talBasketSummary.Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
            talBasketSummary.Settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
            talBasketSummary.Settings.BusinessUnit = PricingBusinessUnit
            talBasketSummary.Settings.Partner = Partner
            talBasketSummary.Settings.EcommerceModuleDefaultsValues = _pricingModuleDefaultsValues
            talBasketSummary.Settings.StoredProcedureGroup = _pricingModuleDefaultsValues.StoredProcedureGroup
            talBasketSummary.Settings.CacheDependencyPath = _pricingModuleDefaultsValues.CacheDependencyPath
            talBasketSummary.Settings.OriginatingSourceCode = GlobalConstants.SOURCE
            talBasketSummary.Settings.CurrentPageName = currentPageName
            talBasketSummary.Settings.CanProcessFeesParallely = _pricingModuleDefaultsValues.CanProcessFeesParallely
            If _pricingModuleDefaultsValues.TicketingKioskMode Then
                talBasketSummary.Settings.OriginatingSource = "KIOSK"
            Else
                If HttpContext.Current.Session IsNot Nothing AndAlso HttpContext.Current.Session("Agent") IsNot Nothing Then
                    talBasketSummary.Settings.AgentEntity = Utilities.GetAgentEntity(_pricingModuleDefaultsValues.CacheDependencyPath)
                    talBasketSummary.Settings.IsAgent = True
                    talBasketSummary.Settings.OriginatingSource = Convert.ToString(HttpContext.Current.Session.Item("Agent"))
                Else
                    talBasketSummary.Settings.OriginatingSource = String.Empty
                End If
            End If
            talBasketSummary.WebPrices = basket.WebPrices
            talBasketSummary.LoginID = username
            talBasketSummary.Settings.LoginId = username
            talBasketSummary.DeliveryChargeEntity = Utilities.GetDeliveryCharges(_pricingModuleDefaultsValues.DeliveryCalculationInUse)
            Dim errObj As Talent.Common.ErrorObj = talBasketSummary.GetBasket(basketHeaderID, canProcessBookingFees)

            If Not errObj.HasError AndAlso talBasketSummary.ResultDataSet IsNot Nothing AndAlso talBasketSummary.ResultDataSet.Tables.Count > 1 Then

                'Populate header as TalentBasket
                properties = ProfileHelper.GetPropertyNames(basket)
                basket = PopulateProperties(properties, talBasketSummary.ResultDataSet.Tables("BasketHeader"), basket, 0)

                '  Populate the items in the basket
                dt = talBasketSummary.ResultDataSet.Tables("BasketDetail")
                properties = ProfileHelper.GetPropertyNames(item)
                If dt.Rows.Count > 0 Then
                    basket.BasketItems.Clear()
                    For i As Integer = 0 To dt.Rows.Count - 1
                        item = New TalentBasketItem
                        item = PopulateProperties(properties, dt, item, i)
                        basket.BasketItems.Add(item)
                        'todo is this rellay needed to populate master products
                        If (UCase(item.MODULE_) <> "TICKETING") Then
                            If Not item.IS_FREE Then
                                If (item.MASTER_PRODUCT.Length > 0) Then
                                    If Not basket.MasterProducts.ContainsKey(item.MASTER_PRODUCT) Then
                                        basket.MasterProducts.Add(item.MASTER_PRODUCT, New Talent.Common.WebPriceProduct(item.MASTER_PRODUCT, item.Quantity, item.MASTER_PRODUCT))
                                    Else
                                        basket.MasterProducts.Item(item.MASTER_PRODUCT).Quantity = basket.MasterProducts.Item(item.MASTER_PRODUCT).Quantity + item.Quantity

                                    End If
                                Else
                                    If Not basket.MasterProducts.ContainsKey(item.Product) Then
                                        basket.MasterProducts.Add(item.Product, New Talent.Common.WebPriceProduct(item.Product, 0, item.Product))
                                    End If
                                End If
                            End If
                        End If
                    Next
                End If

                'Populate summary
                basket.BasketSummary = talBasketSummary.BasketSummaryEntity
            End If 'talbasketsummary if ends
        Else
            'This should never happen, but if it does it's because we haven't been able to update the basket records.
            'Attempt to go to the default page to re-validate the session and basket/profile
            HttpContext.Current.Response.Redirect("~/default.aspx")
        End If

        Utilities.TalentLogging.LoadTestLog("TalentProfileProvider.vb", "GetBasket", timeSpan)
        Return basket
    End Function



    'ck todo worst implementation method needs cleanup - everything should goes to talentbasketsummary
    ''' <summary>
    ''' Gets the basket for the current user. First checks to see if the user is authenticated, if they are it will get their latest basket.
    ''' If not, it will check to see if a basket exists for them, if so it returns it from the DB. If not it will create a new one.
    ''' </summary>
    ''' <param name="username">The current user's username</param>
    ''' <param name="isAuthenticated">Boolean indicating whether or not the current user is logged in</param>
    ''' <returns>An instance of the TAlentBasket object, populated from the database.</returns>
    ''' <remarks></remarks>
    Public Function GetBasket1(ByVal username As String, ByVal isAuthenticated As Boolean) As TalentBasket
        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay
        Dim loginID As String = String.Empty
        Dim basketDetails As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        Dim basketHeader As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
        'this defaults are used only in web pricing methods 
        _pricingModuleDefaultsValues = (New ECommerceModuleDefaults).GetDefaults(Partner, PricingBusinessUnit)
        If _pricingModuleDefaultsValues.UseGlobalPriceListWithCustomerPriceList Then
            _defaultPriceList = _pricingModuleDefaultsValues.DefaultPriceList
        Else
            _defaultPriceList = _pricingModuleDefaultsValues.PriceList
        End If
        Dim basket As New TalentBasket
        Dim item As New TalentBasketItem
        Dim dt As System.Data.DataTable
        Dim campaignCode As String = String.Empty
        Try
            If HttpContext.Current.Session("CAMPAIGN_CODE") IsNot Nothing Then campaignCode = HttpContext.Current.Session("CAMPAIGN_CODE").ToString
        Catch ex As Exception
            campaignCode = String.Empty
        End Try

        '-------------------------------------
        ' Get the basket header for the user
        '-------------------------------------
        Dim basketHeaderID As String = String.Empty

        If Not isAuthenticated Then

            If HttpContext.Current.Profile.IsAnonymous Then
                ' .NET framework will regenerate the anonymousid (username) and therefore occasionally requires updating on tbl_basket_header
                Dim dt0 As System.Data.DataTable
                dt0 = basketHeader.GetDataBy_SessionID(HttpContext.Current.Session.SessionID.ToString, BusinessUnit)
                If dt0.Rows.Count = 1 Then
                    If dt0.Rows(0)("LOGINID").ToString <> username Then
                        basketHeader.Update_Loginid(username, HttpContext.Current.Session.SessionID.ToString)
                    End If
                End If
            End If

            ' Retrieve the current basket by username (i.e. .NET anonymous id)
            dt = basketHeader.GetDataBy_Anonymous_Username(username, BusinessUnit)
            If Not dt.Rows.Count > 0 Then
                basketHeader.CreateBasket(BusinessUnit, Partner, username, Now, Now, False, False, False, "N", "", False, campaignCode, String.Empty, "", "", HttpContext.Current.Session.SessionID.ToString, False)
                dt = basketHeader.GetDataBy_Anonymous_Username(username, BusinessUnit)
            End If

        Else
            loginID = username
            dt = basketHeader.GetDataBy_LoginID(BusinessUnit, Partner, username)
            If Not dt.Rows.Count > 0 Then
                basketHeader.CreateBasket(BusinessUnit, Partner, username, Now, Now, False, False, False, "N", "", False, campaignCode, String.Empty, "", "", HttpContext.Current.Session.SessionID.ToString, False)
                dt = basketHeader.GetDataBy_LoginID(BusinessUnit, Partner, username)
                If Not HttpContext.Current.Profile.IsAnonymous Then
                    Dim bid As String = ""
                    Try
                        bid = CStr(dt.Rows(0)("BASKET_HEADER_ID"))
                    Catch ex As Exception
                    End Try
                    Talent.eCommerce.Logging.WriteLog(username, _
                                                        "NEW BASKET GENERATED", _
                                                        "New Basket Generated - " & bid, _
                                                        "", _
                                                        TalentCache.GetBusinessUnit, TalentCache.GetDefaultPartner)
                End If
            End If
        End If

        basketHeaderID = CStr(dt.Rows(0)("BASKET_HEADER_ID"))
        Dim properties As ArrayList = ProfileHelper.GetPropertyNames(basket)
        basket = PopulateProperties(properties, dt, basket, 0)
        dt = basketDetails.GetBasketItems_ByHeaderID_ALL(basket.Basket_Header_ID)
        properties = ProfileHelper.GetPropertyNames(item)
        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                item = New TalentBasketItem
                item = PopulateProperties(properties, dt, item, i)
                basket.BasketItems.Add(item)
                If (UCase(item.MODULE_) <> "TICKETING") Then
                    If Not item.IS_FREE Then
                        If (item.MASTER_PRODUCT.Length > 0) Then
                            If Not basket.MasterProducts.ContainsKey(item.MASTER_PRODUCT) Then
                                basket.MasterProducts.Add(item.MASTER_PRODUCT, New Talent.Common.WebPriceProduct(item.MASTER_PRODUCT, item.Quantity, item.MASTER_PRODUCT))
                            Else
                                basket.MasterProducts.Item(item.MASTER_PRODUCT).Quantity = basket.MasterProducts.Item(item.MASTER_PRODUCT).Quantity + item.Quantity
                            End If
                        Else
                            If Not basket.MasterProducts.ContainsKey(item.Product) Then
                                basket.MasterProducts.Add(item.Product, New Talent.Common.WebPriceProduct(item.Product, 0, item.Product))
                            End If
                        End If
                    End If
                End If
            Next
        End If

        'calulate the web price for retail items in the basket
        basket = CalculateWebPricing(username, isAuthenticated, basket)

        Dim talBasketSummary As New Talent.Common.TalentBasketSummary

        talBasketSummary.Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
        talBasketSummary.Settings.DestinationDatabase = "SQL2005"
        talBasketSummary.Settings.BusinessUnit = PricingBusinessUnit
        talBasketSummary.Settings.Partner = Partner
        talBasketSummary.Settings.EcommerceModuleDefaultsValues = _pricingModuleDefaultsValues
        talBasketSummary.Settings.StoredProcedureGroup = _pricingModuleDefaultsValues.StoredProcedureGroup
        talBasketSummary.Settings.CacheDependencyPath = _pricingModuleDefaultsValues.CacheDependencyPath
        talBasketSummary.Settings.OriginatingSourceCode = GlobalConstants.SOURCE
        talBasketSummary.Settings.CurrentPageName = (New System.IO.FileInfo(System.Web.HttpContext.Current.Request.Url.AbsolutePath)).Name.ToLower
        If _pricingModuleDefaultsValues.TicketingKioskMode Then
            talBasketSummary.Settings.OriginatingSource = "KIOSK"
        Else
            If HttpContext.Current.Session IsNot Nothing AndAlso _
            HttpContext.Current.Session("Agent") IsNot Nothing Then
                talBasketSummary.Settings.IsAgent = True
                talBasketSummary.Settings.OriginatingSource = HttpContext.Current.Session.Item("Agent")
            End If
        End If
        talBasketSummary.WebPrices = basket.WebPrices
        talBasketSummary.LoginID = username
        Dim errObj As Talent.Common.ErrorObj = talBasketSummary.GetBasket(basketHeaderID, False)

        If Not errObj.HasError AndAlso talBasketSummary.ResultDataSet IsNot Nothing AndAlso talBasketSummary.ResultDataSet.Tables.Count > 1 Then

            'Populate header as TalentBasket
            properties = ProfileHelper.GetPropertyNames(basket)
            basket = PopulateProperties(properties, talBasketSummary.ResultDataSet.Tables("BasketHeader"), basket, 0)

            '  Populate the items in the basket
            dt = talBasketSummary.ResultDataSet.Tables("BasketDetail")
            properties = ProfileHelper.GetPropertyNames(item)
            If dt.Rows.Count > 0 Then
                basket.BasketItems.Clear()
                For i As Integer = 0 To dt.Rows.Count - 1
                    item = New TalentBasketItem
                    item = PopulateProperties(properties, dt, item, i)
                    basket.BasketItems.Add(item)
                    'todo is this rellay needed to populate master products
                    If (UCase(item.MODULE_) <> "TICKETING") Then
                        If Not item.IS_FREE Then
                            If (item.MASTER_PRODUCT.Length > 0) Then
                                If Not basket.MasterProducts.ContainsKey(item.MASTER_PRODUCT) Then
                                    basket.MasterProducts.Add(item.MASTER_PRODUCT, New Talent.Common.WebPriceProduct(item.MASTER_PRODUCT, item.Quantity, item.MASTER_PRODUCT))
                                Else
                                    basket.MasterProducts.Item(item.MASTER_PRODUCT).Quantity = basket.MasterProducts.Item(item.MASTER_PRODUCT).Quantity + item.Quantity

                                End If
                            Else
                                If Not basket.MasterProducts.ContainsKey(item.Product) Then
                                    basket.MasterProducts.Add(item.Product, New Talent.Common.WebPriceProduct(item.Product, 0, item.Product))
                                End If
                            End If
                        End If
                    End If
                Next
            End If

            'Populate summary
            basket.BasketSummary = talBasketSummary.BasketSummaryEntity
        End If 'talbasketsummary if ends

        Utilities.TalentLogging.LoadTestLog("TalentProfileProvider.vb", "GetBasket", timeSpan)
        Return basket
    End Function

    ''' <summary>
    ''' Populates a TalentProfilePartner object contianing partner details and address data etc
    ''' </summary>
    ''' <returns>
    ''' TalentProfilePartner obj populated with data from the database for the current partner
    ''' </returns>
    ''' <remarks></remarks>
    Protected Function GetPartnerData() As TalentProfilePartner

        Dim partnerProfileDetails As New TalentMembershipDatasetTableAdapters.tbl_partnerTableAdapter
        Dim ProfileAddress As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter

        Dim partnerObj As New TalentProfilePartner
        Dim details As New TalentProfilePartnerDetails
        Dim address As New TalentProfileAddress

        '---------------------------------------
        'Populate the partner.Details properties
        '---------------------------------------




        'Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
        'Dim selectstring As String = "SELECT     PARTNER_ID, PARTNER, PARTNER_DESC, DESTINATION_DATABASE, CACHEING_ENABLED, CACHE_TIME_MINUTES, LOGGING_ENABLED," & _
        '             " STORE_XML, ACCOUNT_NO_1, ACCOUNT_NO_2, ACCOUNT_NO_3, ACCOUNT_NO_4, ACCOUNT_NO_5, EMAIL, TELEPHONE_NUMBER, FAX_NUMBER," & _
        '             " PARTNER_URL, PARTNER_NUMBER, ORIGINATING_BUSINESS_UNIT, CRM_BRANCH, VAT_NUMBER, ENABLE_PRICE_VIEW," & _
        '             " ORDER_ENQUIRY_SHOW_PARTNER_ORDERS, ENABLE_ALTERNATE_SKU, MINIMUM_PURCHASE_QUANTITY, MINIMUM_PURCHASE_AMOUNT," & _
        '               " USE_MINIMUM_PURCHASE_QUANTITY, USE_MINIMUM_PURCHASE_AMOUNT, COST_CENTRE " & _
        '             " FROM         tbl_partner WITH (NOLOCK)" & _
        '             " WHERE     (PARTNER = @partner) "

        'cmd.CommandText = selectstring

        'With cmd.Parameters
        '    .Add("@PARTNER", SqlDbType.NVarChar).Value = Partner
        'End With

        'Try
        '    cmd.Connection.Open()

        '    Dim da As New SqlDataAdapter(cmd)
        '    Dim d1 As New DataTable
        '    da.Fill(d1)
        '    da.Dispose()
        'Catch ex As Exception
        'Finally
        '    If cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
        'End Try




        Dim dt As System.Data.DataTable = partnerProfileDetails.GetDataByPartner(Partner)
        Dim al As ArrayList = ProfileHelper.GetPropertyNames(details)
        details = PopulateProperties(al, dt, details, 0)
        details.IsDirty = False 'Set to false as we have just read the values from the DB
        partnerObj.Details = details
        '---------------------------------------

        '-----------------------------------------
        'Populate the partner.Addresses properties
        '-----------------------------------------
        al = ProfileHelper.GetPropertyNames(address)
        dt = ProfileAddress.GetDataByPartnerAndType(Partner, "PARTNER")
        For i As Integer = 0 To dt.Rows.Count - 1
            address = New TalentProfileAddress
            address = PopulateProperties(al, dt, address, i)
            address.IsDirty = False 'Set to false as we have just read the values from the DB
            partnerObj.Addresses.Add(dt.Rows(i)("REFERENCE"), address)
        Next

        partnerProfileDetails.Dispose()
        ProfileAddress.Dispose()
        Return partnerObj
    End Function

    ''' <summary>
    ''' Populates a TalentProfileUser object contianing user details and address data etc
    ''' </summary>
    ''' <param name="username"></param>
    ''' <returns>
    ''' TalentProfileUser obj populated with data from the database for the
    ''' specific username (and partner specified in the provider's properties) supplied
    ''' </returns>
    ''' <remarks></remarks>
    Protected Function GetUserData(ByVal username As String) As TalentProfileUser

        Dim userProfileDetails As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter
        Dim ProfileAddress As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter
        Dim authPartners As New TalentMembershipDatasetTableAdapters.tbl_authorized_partnersTableAdapter

        'Create a new user obj
        Dim user As New TalentProfileUser
        Dim details As New TalentProfileUserDetails
        Dim address As New TalentProfileAddress

        '---------------------------------------
        'Populate the user.Details properties
        '---------------------------------------
        Dim dt As System.Data.DataTable
        If authPartners.Get_CheckFor_B2C_Login(BusinessUnit).Rows.Count > 0 Then
            dt = userProfileDetails.GetByLoginIDAndPartner(username, Partner)
        Else
            dt = userProfileDetails.GetDataByLoginIDNoPartner(username)
        End If
        Dim al As ArrayList = ProfileHelper.GetPropertyNames(details)
        details = PopulateProperties(al, dt, details, 0)
        details.IsDirty = False 'Set to false as we have just read the values from the DB
        user.Details = details
        '---------------------------------------

        '---------------------------------------
        'Populate the user.Addresses properties
        '---------------------------------------
        al = ProfileHelper.GetPropertyNames(address)
        dt = ProfileAddress.GetByLoginIDAndPartner(username, Partner)

        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To dt.Rows.Count - 1
                address = New TalentProfileAddress
                address = PopulateProperties(al, dt, address, i)
                address.IsDirty = False 'Set to false as we have just read the values from the DB
                Dim exists As Boolean = False
                For Each key As String In user.Addresses.Keys
                    If UCase(key) = UCase(Utilities.CheckForDBNull_String(dt.Rows(i)("REFERENCE"))) AndAlso _
                        Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dt.Rows(i)("REFERENCE"))) Then
                        exists = True
                        Exit For
                    End If
                Next
                If Not exists Then user.Addresses.Add(Utilities.CheckForDBNull_String(dt.Rows(i)("REFERENCE")), address)
            Next
        End If
        _userAttributeList = user.Details.ATTRIBUTES_LIST
        Return user
    End Function

    ''' <summary>
    ''' Populates the properties on an object with the corresponding values in the DataTable where the property name is equal to the column name.
    ''' </summary>
    ''' <param name="propertiesList">A list of the properties on the object to be populated.</param>
    ''' <param name="dt">The DataTable containing the data to populate the properties with.</param>
    ''' <param name="objectToPopulate">The object that is to be populated.</param>
    ''' <param name="rowIndex">The index of the row of the DataTable to extract the values from.</param>
    ''' <returns>The object with its newly populated properties.</returns>
    ''' <remarks></remarks>
    Protected Function PopulateProperties(ByVal propertiesList As ArrayList, ByVal dt As System.Data.DataTable, ByVal objectToPopulate As Object, ByVal rowIndex As Integer) As Object
        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To propertiesList.Count - 1
                If dt.Columns.Contains(propertiesList(i)) Then
                    CallByName(objectToPopulate, propertiesList(i), CallType.Set, ProfileHelper.CheckDBNull(dt.Rows(rowIndex)(propertiesList(i))))
                Else
                    'If the column does not exist, handle any properties on the class that we know of
                    Select Case propertiesList(i).ToString
                        Case Is = "IsDirty"
                            CallByName(objectToPopulate, propertiesList(i), CallType.Set, False)
                        Case Is = "MODULE_"
                            'Module is a KEYWORD in vb.net so the property name could not be called the same as the DB field name.
                            CallByName(objectToPopulate, propertiesList(i), CallType.Set, Utilities.CheckForDBNull_String(dt.Rows(rowIndex)("MODULE")))
                        Case Else
                            'Handle all other occurances
                    End Select
                End If
            Next
        End If

        Return objectToPopulate
    End Function



    Public Overrides Sub SetPropertyValues(ByVal context As System.Configuration.SettingsContext, ByVal collection As System.Configuration.SettingsPropertyValueCollection)
        ' Get Settings
        Dim userName As String = CType(context("UserName"), String)
        Dim isAuthenticated As Boolean = CType(context("IsAuthenticated"), Boolean)
        SetPartner(isAuthenticated, userName)

        For Each TalentProperty As SettingsPropertyValue In collection
            Select Case TalentProperty.Name
                Case Is = "User"
                    If isAuthenticated Then
                        SetUserData(userName, CType(TalentProperty.PropertyValue, TalentProfileUser))
                    End If
                Case Is = "PartnerInfo"
                    If isAuthenticated Then
                        SetPartnerData(CType(TalentProperty.PropertyValue, TalentProfilePartner))
                    End If
                Case Is = "Basket"
                    SetBasket(userName, isAuthenticated, CType(TalentProperty.PropertyValue, TalentBasket))
            End Select
        Next
    End Sub

    Public Sub SetBasket(ByVal username As String, ByVal isAuthenticated As Boolean, ByVal basket As TalentBasket)
        'Dim basketDetails As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        'If basket.IsDirty Then
        '    basketDetails.Empty_Basket(basket.Basket_Header_ID)
        '    For Each tbi As TalentBasketItem In basket.BasketItems
        '        basketDetails.Add_Item_To_Basket(basket.Basket_Header_ID, tbi.Product, tbi.Quantity, tbi.Price)
        '    Next
        'End If

        Dim newItem As Boolean = False
        Dim dt As Data.DataTable
        Dim basketDetails As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter

        'If the basket has changed
        If basket.IsDirty Then
            Try
                Dim basketHeader As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
                basketHeader.SetBasketStockErrorStatus(basket.STOCK_ERROR, basket.Basket_Header_ID)
                basketHeader.Dispose()
            Catch ex As Exception
            End Try

            'Loop through all items in the basket
            For Each tbi As TalentBasketItem In basket.BasketItems

                ' Basket updates will happen in ticketing gateway for ticketing products
                If Not UCase(tbi.MODULE_) = "TICKETING" Then

                    newItem = True 'Initially mark the item as new
                    Dim tDataObjects As New Talent.Common.TalentDataObjects
                    tDataObjects.Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
                    tDataObjects.Settings.BusinessUnit = TalentCache.GetBusinessUnit()
                    dt = tDataObjects.BasketSettings.TblBasketDetail.GetDetailByBasketHeaderID(basket.Basket_Header_ID)
                    'dt = basketDetails.GetBasketItems_ByHeaderID_ALL(basket.Basket_Header_ID) 'Get the basket for the header

                    'Loop through each item in the saved basket
                    For Each row As Data.DataRow In dt.Rows
                        'Check the current basket item against the data table item
                        If row("PRODUCT") = tbi.Product AndAlso row("Is_FREE") = tbi.IS_FREE Then
                            'If they match, update the values and mark the item as NOT new
                            '   basketDetails.Update_Basket_Item(tbi.Quantity, tbi.STOCK_ERROR, tbi.QUANTITY_AVAILABLE, tbi.Gross_Price, tbi.Net_Price, tbi.Tax_Price, basket.Basket_Header_ID, tbi.Product)
                            ' basketDetails.Update_basket_item_inc_error_code(tbi.Quantity, tbi.STOCK_ERROR, tbi.QUANTITY_AVAILABLE, tbi.Gross_Price, tbi.STOCK_ERROR_CODE, tbi.Net_Price, tbi.Tax_Price, basket.Basket_Header_ID, tbi.Product)
                            basketDetails.Update_basket_item_inc_error_code(tbi.Quantity, tbi.STOCK_ERROR, tbi.QUANTITY_AVAILABLE, tbi.Gross_Price, tbi.STOCK_ERROR_CODE, tbi.Net_Price, tbi.Tax_Price, tbi.Cost_Centre, tbi.Account_Code, tbi.FULFIL_OPT_POST, tbi.FULFIL_OPT_COLL, tbi.FULFIL_OPT_PAH, tbi.FULFIL_OPT_UPL, tbi.FULFIL_OPT_REGPOST, tbi.FULFIL_OPT_PRINT, tbi.CURR_FULFIL_SLCTN, tbi.PACKAGE_ID, basket.Basket_Header_ID, tbi.Product, tbi.Xml_Config, tbi.IS_FREE)
                            newItem = False
                        End If
                    Next

                    'If it is a new item add it to the basket
                    If newItem Then
                        If tbi.GROUP_LEVEL_01 Is Nothing Then tbi.GROUP_LEVEL_01 = ""
                        If tbi.GROUP_LEVEL_02 Is Nothing Then tbi.GROUP_LEVEL_02 = ""
                        If tbi.GROUP_LEVEL_03 Is Nothing Then tbi.GROUP_LEVEL_03 = ""
                        If tbi.GROUP_LEVEL_04 Is Nothing Then tbi.GROUP_LEVEL_04 = ""
                        If tbi.GROUP_LEVEL_05 Is Nothing Then tbi.GROUP_LEVEL_05 = ""
                        If tbi.GROUP_LEVEL_06 Is Nothing Then tbi.GROUP_LEVEL_06 = ""
                        If tbi.GROUP_LEVEL_07 Is Nothing Then tbi.GROUP_LEVEL_07 = ""
                        If tbi.GROUP_LEVEL_08 Is Nothing Then tbi.GROUP_LEVEL_08 = ""
                        If tbi.GROUP_LEVEL_09 Is Nothing Then tbi.GROUP_LEVEL_09 = ""
                        If tbi.GROUP_LEVEL_10 Is Nothing Then tbi.GROUP_LEVEL_10 = ""
                        If tbi.MASTER_PRODUCT Is Nothing Then tbi.MASTER_PRODUCT = ""
                        If tbi.MODULE_ Is Nothing Then tbi.MODULE_ = ""
                        If tbi.PRICE_BAND Is Nothing Then tbi.PRICE_BAND = ""
                        If tbi.PRICE_CODE Is Nothing Then tbi.PRICE_CODE = ""
                        If tbi.PRODUCT_DESCRIPTION1 Is Nothing Then tbi.PRODUCT_DESCRIPTION1 = ""
                        If tbi.PRODUCT_DESCRIPTION2 Is Nothing Then tbi.PRODUCT_DESCRIPTION2 = ""
                        If tbi.PRODUCT_DESCRIPTION3 Is Nothing Then tbi.PRODUCT_DESCRIPTION3 = ""
                        If tbi.PRODUCT_DESCRIPTION4 Is Nothing Then tbi.PRODUCT_DESCRIPTION4 = ""
                        If tbi.PRODUCT_DESCRIPTION5 Is Nothing Then tbi.PRODUCT_DESCRIPTION5 = ""
                        If tbi.PRODUCT_DESCRIPTION6 Is Nothing Then tbi.PRODUCT_DESCRIPTION6 = ""
                        If tbi.PRODUCT_DESCRIPTION7 Is Nothing Then tbi.PRODUCT_DESCRIPTION7 = ""
                        If tbi.PRODUCT_TYPE Is Nothing Then tbi.PRODUCT_TYPE = ""
                        If tbi.RESERVED_SEAT Is Nothing Then tbi.RESERVED_SEAT = ""
                        If tbi.SEAT Is Nothing Then tbi.SEAT = ""
                        If tbi.Size Is Nothing Then tbi.Size = ""
                        If tbi.STOCK_ERROR_CODE Is Nothing Then tbi.STOCK_ERROR_CODE = ""
                        If tbi.STOCK_LIMIT Is Nothing Then tbi.STOCK_LIMIT = ""
                        If tbi.STOCK_REQUESTED Is Nothing Then tbi.STOCK_REQUESTED = ""
                        If tbi.Xml_Config Is Nothing Then
                            tbi.Xml_Config = ""
                        ElseIf UCase(tbi.Xml_Config) = "TRUE" Then
                            tbi.Xml_Config = "1"
                        End If

                        If tbi.Cost_Centre Is Nothing Then tbi.Cost_Centre = ""
                        If tbi.Account_Code Is Nothing Then tbi.Account_Code = ""


                        basketDetails.Add_Item_To_Basket(basket.Basket_Header_ID, _
                                                            tbi.Product, _
                                                            tbi.Quantity, _
                                                            tbi.Gross_Price, _
                                                            tbi.PRODUCT_DESCRIPTION1, _
                                                            tbi.PRODUCT_DESCRIPTION2, _
                                                            tbi.GROUP_LEVEL_01, _
                                                            tbi.GROUP_LEVEL_02, _
                                                            tbi.GROUP_LEVEL_03, _
                                                            tbi.GROUP_LEVEL_04, _
                                                            tbi.GROUP_LEVEL_05, _
                                                            tbi.GROUP_LEVEL_06, _
                                                            tbi.GROUP_LEVEL_07, _
                                                            tbi.GROUP_LEVEL_08, _
                                                            tbi.GROUP_LEVEL_09, _
                                                            tbi.GROUP_LEVEL_10, _
                                                            False, _
                                                            tbi.Size, _
                                                            tbi.MASTER_PRODUCT, _
                                                            tbi.ALTERNATE_SKU, _
                                                            tbi.Net_Price, _
                                                            tbi.Tax_Price, _
                                                            tbi.Xml_Config, _
                                                            tbi.Cost_Centre, _
                                                            tbi.Account_Code, _
                                                            tbi.FULFIL_OPT_POST, _
                                                            tbi.FULFIL_OPT_COLL, _
                                                            tbi.FULFIL_OPT_PAH, _
                                                            tbi.FULFIL_OPT_UPL, _
                                                            tbi.FULFIL_OPT_REGPOST, _
                                                            tbi.FULFIL_OPT_PRINT, _
                                                            tbi.CURR_FULFIL_SLCTN, _
                                                            tbi.PACKAGE_ID,
                                                            tbi.ALLOW_SELECT_OPTION,
                                                            tbi.OPTION_SELECTED,
                                                            tbi.WEIGHT)
                    End If
                End If
            Next
        End If
    End Sub

    ''' <summary>
    ''' Checks the TalentProfilePartner object to see if it has changed and stores the data back in the DB if it has
    ''' </summary>
    ''' <param name="partnerObj">The TalentProfilePartner object containing the properties to be stored.</param>
    ''' <remarks></remarks>
    Protected Sub SetPartnerData(ByVal partnerObj As TalentProfilePartner)


        Dim partnerProfileDetails As New TalentMembershipDatasetTableAdapters.tbl_partnerTableAdapter
        Dim ProfileAddress As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter

        If partnerObj.Details.IsDirty Then
            Dim details As TalentProfilePartnerDetails = partnerObj.Details
            partnerProfileDetails.UpdatePartnerDetails(details.Partner, _
                                                        details.Partner_Desc, _
                                                        details.Destination_Database, _
                                                        details.Caching_Enabled, _
                                                        details.Cache_Time_Minutes, _
                                                        details.Logging_Enabled, _
                                                        details.Store_XML, _
                                                        details.Account_No_1, _
                                                        details.Account_No_2, _
                                                        details.Account_No_3, _
                                                        details.Account_No_4, _
                                                        details.Account_No_5, _
                                                        details.Email, _
                                                        details.Telephone_Number, _
                                                        details.Fax_Number, _
                                                        details.Partner_URL, _
                                                        details.VAT_NUMBER, _
                                                        details.Enable_Price_View, _
                                                        details.Order_Enquiry_Show_Partner_Orders, _
                                                        details.Enable_Alternate_SKU, _
                                                        details.Minimum_Purchase_Quantity, _
                                                        details.Minimum_Purchase_Amount, _
                                                        details.Use_Minimum_Purchase_Quantity, _
                                                        details.Use_Minimum_Purchase_Amount, _
                                                        details.COST_CENTRE)
        End If

        Dim enumer As Collections.Generic.Dictionary(Of String, TalentProfileAddress).KeyCollection.Enumerator
        enumer = partnerObj.Addresses.Keys.GetEnumerator
        Dim partnerAddress As TalentProfileAddress
        While enumer.MoveNext
            partnerAddress = partnerObj.Addresses(enumer.Current)
            If partnerAddress.IsDirty Then
                ProfileAddress.UpdatePartnerAddress(Partner, _
                                                    partnerAddress.LoginID, _
                                                    partnerAddress.Type, _
                                                    partnerAddress.Reference, _
                                                    partnerAddress.Sequence, _
                                                    partnerAddress.Default_Address, _
                                                    partnerAddress.Address_Line_1, _
                                                    partnerAddress.Address_Line_2, _
                                                    partnerAddress.Address_Line_3, _
                                                    partnerAddress.Address_Line_4, _
                                                    partnerAddress.Address_Line_5, _
                                                    partnerAddress.Post_Code, _
                                                    partnerAddress.Country, _
                                                    partnerAddress.Address_ID)
            End If
        End While
    End Sub

    ''' <summary>
    ''' Checks the TalentProfileUser object to see if it has changed and stores the data back into the database if it has.
    ''' </summary>
    ''' <param name="username">UserName for the current user</param>
    ''' <param name="user">The TalentProfileUser object containing the data to be stored</param>
    ''' <remarks></remarks>
    Protected Sub SetUserData(ByVal username As String, ByVal user As TalentProfileUser)

        Dim userProfileDetails As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter
        Dim ProfileAddress As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter

        'Update user details if necessary
        If user.Details.IsDirty Then
            Dim userDetails As TalentProfileUserDetails = user.Details
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

            Try
                Dim member As New TalentMembershipProvider
                member.StampLastUpdateDetailsDate(username)
            Catch ex As Exception
            End Try
        End If


        Dim enumer As Collections.Generic.Dictionary(Of String, TalentProfileAddress).KeyCollection.Enumerator
        enumer = user.Addresses.Keys.GetEnumerator
        Dim userAddress As TalentProfileAddress
        While enumer.MoveNext
            userAddress = user.Addresses(enumer.Current)
            If userAddress.IsDirty Then
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
            End If

        End While
    End Sub


    Public Sub CreateProfile(ByVal userDetails As TalentProfileUserDetails, ByVal userAddress As TalentProfileAddress)
        Dim userProfileDetails As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter
        Dim ProfileAddress As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter

        With userDetails
            userProfileDetails.InsertPartnerUser(Partner, .LoginID, .Email, .Title, .Initials, .Forename, .Surname, .Full_Name, _
                                                        .Salutation, .Position, .DOB, .Mobile_Number, .Telephone_Number, .Work_Number, .Fax_Number, _
                                                        .Other_Number, .Messaging_ID, .User_Number, BusinessUnit, .Account_No_1, .Account_No_2, "", "", "", _
                                                        .Subscribe_Newsletter, .HTML_Newsletter, .Bit0, .Bit1, .Bit2, .Bit3, .Bit4, .Bit5, .Sex, .User_Number_Prefix, _
                                                        BusinessUnit, Talent.eCommerce.Utilities.GetCurrentLanguage, .RESTRICTED_PAYMENT_METHOD, .Subscribe_2, .Subscribe_3, _
                                                        .Ticketing_Loyalty_Points, .ATTRIBUTES_LIST, .BOND_HOLDER, .OWNS_AUTO_MEMBERSHIP, .DEFAULT_CLUB, .Enable_Price_View, _
                                                        .SUPPORTER_CLUB_CODE, .FAVOURITE_TEAM_CODE, .MAIL_TEAM_CODE_1, .MAIL_TEAM_CODE_2, .MAIL_TEAM_CODE_3, .MAIL_TEAM_CODE_4, _
                                                        .MAIL_TEAM_CODE_5, .PREFERRED_CONTACT_METHOD, .FAVOURITE_SPORT, .Mothers_Name, .Fathers_Name, _
                                                        .PIN, .Passport, .GreenCard, .User_ID_4, .User_ID_5, .User_ID_6, .User_ID_7, .User_ID_8, .User_ID_9, _
                                                         .Minimum_Purchase_Quantity, .Minimum_Purchase_Amount, _
                                                        .Use_Minimum_Purchase_Quantity, .Use_Minimum_Purchase_Amount, .CompanyName, .Favourite_Seat, .Smartcard_Number, .STOP_CODE, .PRICE_BAND, .CONTACT_BY_POST, .CONTACT_BY_TELEPHONE_HOME, .CONTACT_BY_TELEPHONE_WORK, .CONTACT_BY_MOBILE, .CONTACT_BY_EMAIL, .BOOK_NUMBER, .CUSTOMER_SUFFIX, .NICKNAME, .USERNAME, .PARENT_PHONE, .PARENT_EMAIL, .PARENTAL_CONSENT_STATUS)
        End With

        'Insert the address
        ProfileAddress.DeleteByLoginTypeSeqDefaddPartner(userAddress.LoginID, _
                                userAddress.Type, _
                                userAddress.Sequence, _
                                userAddress.Default_Address, _
                                Partner)

        ProfileAddress.AddAddress(Partner, _
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
                                userAddress.Delivery_Zone_Code, _
                                userAddress.External_ID)
    End Sub

    Public Sub CreateProfileWithPartner(ByVal userDetails As TalentProfileUserDetails, ByVal userAddress As TalentProfileAddress, ByVal partnerDetails As TalentProfilePartnerDetails)
        Dim userProfilePartnerDetails As New TalentMembershipDatasetTableAdapters.tbl_partnerTableAdapter
        Dim userProfileDetails As New TalentMembershipDatasetTableAdapters.tbl_partner_userTableAdapter
        Dim ProfileAddress As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter

        Dim dt As New DataTable
        dt = userProfilePartnerDetails.GetDataByPartner(partnerDetails.Partner)
        If Not dt.Rows.Count > 0 Then
            userProfilePartnerDetails.InsertNewPartnerDetails(partnerDetails.Partner, _
                                                              String.Empty, _
                                                              String.Empty, _
                                                              False, _
                                                              0, _
                                                              False, _
                                                              False, _
                                                              partnerDetails.Account_No_1, _
                                                              partnerDetails.Account_No_2, _
                                                              partnerDetails.Account_No_3, _
                                                              partnerDetails.Account_No_4, _
                                                              partnerDetails.Account_No_5, _
                                                              String.Empty, _
                                                              String.Empty, _
                                                              String.Empty, _
                                                              String.Empty, _
                                                              partnerDetails.Partner_Number, _
                                                              String.Empty, _
                                                              partnerDetails.CRM_Branch, _
                                                              partnerDetails.VAT_NUMBER, _
                                                              True, _
                                                              partnerDetails.Order_Enquiry_Show_Partner_Orders, _
                                                              partnerDetails.Enable_Alternate_SKU, _
                                                              partnerDetails.Minimum_Purchase_Quantity, _
                                                              partnerDetails.Minimum_Purchase_Amount, _
                                                              partnerDetails.Use_Minimum_Purchase_Quantity, _
                                                              partnerDetails.Use_Minimum_Purchase_Amount, _
                                                              partnerDetails.COST_CENTRE)

        End If



        With userDetails
            userProfileDetails.InsertPartnerUser(partnerDetails.Partner, .LoginID, .Email, .Title, .Initials, .Forename, .Surname, .Full_Name, _
                                                        .Salutation, .Position, .DOB, .Mobile_Number, .Telephone_Number, .Work_Number, .Fax_Number, _
                                                        .Other_Number, .Messaging_ID, .User_Number, BusinessUnit, .Account_No_1, .Account_No_2, "", "", "", _
                                                        .Subscribe_Newsletter, .HTML_Newsletter, .Bit0, .Bit1, .Bit2, .Bit3, .Bit4, .Bit5, .Sex, .User_Number_Prefix, _
                                                        BusinessUnit, Talent.eCommerce.Utilities.GetCurrentLanguage, .RESTRICTED_PAYMENT_METHOD, .Subscribe_2, .Subscribe_3, _
                                                        .Ticketing_Loyalty_Points, .ATTRIBUTES_LIST, .BOND_HOLDER, .OWNS_AUTO_MEMBERSHIP, .DEFAULT_CLUB, .Enable_Price_View, _
                                                        .SUPPORTER_CLUB_CODE, .FAVOURITE_TEAM_CODE, .MAIL_TEAM_CODE_1, .MAIL_TEAM_CODE_2, .MAIL_TEAM_CODE_3, .MAIL_TEAM_CODE_4, _
                                                        .MAIL_TEAM_CODE_5, .PREFERRED_CONTACT_METHOD, .FAVOURITE_SPORT, .Mothers_Name, .Fathers_Name, _
                                                        .PIN, .Passport, .GreenCard, .User_ID_4, .User_ID_5, .User_ID_6, .User_ID_7, .User_ID_8, .User_ID_9, _
                                                         .Minimum_Purchase_Quantity, .Minimum_Purchase_Amount, _
                                                        .Use_Minimum_Purchase_Quantity, .Use_Minimum_Purchase_Amount, .CompanyName, .Favourite_Seat, .Smartcard_Number, .STOP_CODE, .PRICE_BAND, .CONTACT_BY_POST, .CONTACT_BY_TELEPHONE_HOME, .CONTACT_BY_TELEPHONE_WORK, .CONTACT_BY_MOBILE, .CONTACT_BY_EMAIL, .BOOK_NUMBER, .CUSTOMER_SUFFIX, .NICKNAME, .USERNAME, .PARENT_PHONE, .PARENT_EMAIL, .PARENTAL_CONSENT_STATUS)
        End With

        'Insert the address
        ProfileAddress.AddAddress(partnerDetails.Partner, _
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
                                String.Empty, _
                                String.Empty)

    End Sub

    Public Function AddAddressToUserProfile(ByVal Address As TalentProfileAddress) As Boolean
        Dim ProfileAddress As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter
        'Insert the address
        Return ProfileAddress.AddAddress(Partner, _
                                Address.LoginID, _
                                Address.Type, _
                                Address.Reference, _
                                Address.Sequence, _
                                Address.Default_Address, _
                                Address.Address_Line_1, _
                                Address.Address_Line_2, _
                                Address.Address_Line_3, _
                                Address.Address_Line_4, _
                                Address.Address_Line_5, _
                                UCase(Address.Post_Code), _
                                Address.Country, _
                                Address.Delivery_Zone_Code, _
                                String.Empty)
    End Function

    Public Function UpdateDefaultAddress(ByVal LoginID As String, ByVal ptner As String, ByVal addressID As Integer) As Boolean
        'Removed to stop default address being changed, meaning the address shown in the 
        'update details section is the one used last, and not the one the user registered
        'with.

        ''Dim address As New TalentMembershipDatasetTableAdapters.tbl_addressTableAdapter
        ''address.Set_All_Addresses_To_False(ptner, LoginID)
        ''address.Set_Default_Address(ptner, LoginID, addressID)
        Return True
    End Function

    Public Function AddItemToBasket(ByVal BasketHeaderID As String, ByVal tbi As TalentBasketItem) As Integer
        Dim basketDetails As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter

        If tbi.GROUP_LEVEL_01 Is Nothing Then tbi.GROUP_LEVEL_01 = ""
        If tbi.GROUP_LEVEL_02 Is Nothing Then tbi.GROUP_LEVEL_02 = ""
        If tbi.GROUP_LEVEL_03 Is Nothing Then tbi.GROUP_LEVEL_03 = ""
        If tbi.GROUP_LEVEL_04 Is Nothing Then tbi.GROUP_LEVEL_04 = ""
        If tbi.GROUP_LEVEL_05 Is Nothing Then tbi.GROUP_LEVEL_05 = ""
        If tbi.GROUP_LEVEL_06 Is Nothing Then tbi.GROUP_LEVEL_06 = ""
        If tbi.GROUP_LEVEL_07 Is Nothing Then tbi.GROUP_LEVEL_07 = ""
        If tbi.GROUP_LEVEL_08 Is Nothing Then tbi.GROUP_LEVEL_08 = ""
        If tbi.GROUP_LEVEL_09 Is Nothing Then tbi.GROUP_LEVEL_09 = ""
        If tbi.GROUP_LEVEL_10 Is Nothing Then tbi.GROUP_LEVEL_10 = ""
        If tbi.MASTER_PRODUCT Is Nothing Then tbi.MASTER_PRODUCT = ""
        If tbi.MODULE_ Is Nothing Then tbi.MODULE_ = ""
        If tbi.PRICE_BAND Is Nothing Then tbi.PRICE_BAND = ""
        If tbi.PRICE_CODE Is Nothing Then tbi.PRICE_CODE = ""
        If tbi.PRODUCT_DESCRIPTION1 Is Nothing Then tbi.PRODUCT_DESCRIPTION1 = ""
        If tbi.PRODUCT_DESCRIPTION2 Is Nothing Then tbi.PRODUCT_DESCRIPTION2 = ""
        If tbi.PRODUCT_DESCRIPTION3 Is Nothing Then tbi.PRODUCT_DESCRIPTION3 = ""
        If tbi.PRODUCT_DESCRIPTION4 Is Nothing Then tbi.PRODUCT_DESCRIPTION4 = ""
        If tbi.PRODUCT_DESCRIPTION5 Is Nothing Then tbi.PRODUCT_DESCRIPTION5 = ""
        If tbi.PRODUCT_DESCRIPTION6 Is Nothing Then tbi.PRODUCT_DESCRIPTION6 = ""
        If tbi.PRODUCT_DESCRIPTION7 Is Nothing Then tbi.PRODUCT_DESCRIPTION7 = ""
        If tbi.PRODUCT_TYPE Is Nothing Then tbi.PRODUCT_TYPE = ""
        If tbi.RESERVED_SEAT Is Nothing Then tbi.RESERVED_SEAT = ""
        If tbi.SEAT Is Nothing Then tbi.SEAT = ""
        If tbi.Size Is Nothing Then tbi.Size = ""
        If tbi.STOCK_ERROR_CODE Is Nothing Then tbi.STOCK_ERROR_CODE = ""
        If tbi.STOCK_LIMIT Is Nothing Then tbi.STOCK_LIMIT = ""
        If tbi.STOCK_REQUESTED Is Nothing Then tbi.STOCK_REQUESTED = ""
        If tbi.Xml_Config Is Nothing Then tbi.Xml_Config = ""
        If tbi.Cost_Centre Is Nothing Then tbi.Cost_Centre = ""
        If tbi.Account_Code Is Nothing Then tbi.Account_Code = ""

        Return basketDetails.Add_Item_To_Basket(BasketHeaderID, _
                                                tbi.Product, _
                                                tbi.Quantity, _
                                                tbi.Gross_Price, _
                                                tbi.PRODUCT_DESCRIPTION1, _
                                                tbi.PRODUCT_DESCRIPTION2, _
                                                tbi.GROUP_LEVEL_01, _
                                                tbi.GROUP_LEVEL_02, _
                                                tbi.GROUP_LEVEL_03, _
                                                tbi.GROUP_LEVEL_04, _
                                                tbi.GROUP_LEVEL_05, _
                                                tbi.GROUP_LEVEL_06, _
                                                tbi.GROUP_LEVEL_07, _
                                                tbi.GROUP_LEVEL_08, _
                                                tbi.GROUP_LEVEL_09, _
                                                tbi.GROUP_LEVEL_10, _
                                                False, _
                                                tbi.Size, _
                                                tbi.MASTER_PRODUCT, _
                                                tbi.ALTERNATE_SKU, _
                                                tbi.Net_Price, _
                                                tbi.Tax_Price, _
                                                tbi.Xml_Config, _
                                                tbi.Cost_Centre, _
                                                tbi.Account_Code, _
                                                tbi.FULFIL_OPT_POST, _
                                                tbi.FULFIL_OPT_COLL, _
                                                tbi.FULFIL_OPT_PAH, _
                                                tbi.FULFIL_OPT_UPL, _
                                                tbi.FULFIL_OPT_REGPOST, _
                                                tbi.FULFIL_OPT_PRINT, _
                                                tbi.CURR_FULFIL_SLCTN, _
                                                tbi.PACKAGE_ID,
                                                tbi.ALLOW_SELECT_OPTION,
                                                tbi.OPTION_SELECTED,
                                                tbi.WEIGHT)
    End Function

    Public Function MigrateBasketToUser(ByVal LoginUsername As String, ByVal AnonymousUsername As String, ByVal Profile As TalentProfile) As String
        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay
        Dim modDefs As New Talent.eCommerce.ECommerceModuleDefaults
        Dim myDefs As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = modDefs.GetDefaults
        Dim basketDetails As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        Dim basketHeader As New TalentBasketDatasetTableAdapters.tbl_basket_headerTableAdapter
        Dim anonBasket As System.Data.DataTable
        Dim preMigratedBasket As System.Data.DataTable
        Dim basket As System.Data.DataTable
        Dim useAnonymousBasket As Boolean = False
        Dim basketItems As New System.Data.DataTable
        Dim itemCount As Integer = 1
        Dim redirectPath As String = String.Empty
        Dim redirectQuery As New StringBuilder
        Dim ticketAddedBeforeLogin As Boolean = False

        'Basket will be migrated only if
        'Not exists - order paid but basket not processed
        If Not Utilities.IsOrderAlreadyPaid() Then

            basket = New DataTable()
            anonBasket = basketHeader.GetDataBy_Anonymous_Username(AnonymousUsername, BusinessUnit)
            If anonBasket.Rows.Count > 0 Then
                basket = anonBasket
                basketHeader.Delete_Users_Basket_Before_Migration(BusinessUnit, Partner, LoginUsername)
                basketHeader.Migrate_Basket_To_Authenticated_User(BusinessUnit, Partner, LoginUsername, Now, AnonymousUsername)
            Else
                ' There have been some recent changes to the usp_Basket_GetBasket stored procedure which has changed the login/basket
                ' migration process. The stored procedure now does the migration update from anon -> log in on the basket.
                ' As a result, we now have to assume that the basket has already been migrated. 
                preMigratedBasket = basketHeader.GetDataBy_LoginID(BusinessUnit, Profile.PartnerInfo.Details.Partner, LoginUsername)
                If preMigratedBasket.Rows.Count > 0 Then
                    basket = preMigratedBasket
                End If
            End If

            basketItems = basketDetails.GetBasketItems_ByHeaderID_ALL(basket.Rows(0)("BASKET_HEADER_ID"))
            If basketItems.Rows.Count > 0 Then
                '
                ' Update basket items with the cost centre of the partner if it exists
                If Not Profile.PartnerInfo.Details.COST_CENTRE = String.Empty And Not Profile.PartnerInfo.Details.COST_CENTRE Is Nothing Then
                    basketDetails.Update_Cost_Centre(Profile.PartnerInfo.Details.COST_CENTRE, basket.Rows(0)("BASKET_HEADER_ID"))
                End If
                '
                'Update basket items with the last used account code if it exists
                Dim lastAccCode As String
                lastAccCode = (Talent.eCommerce.Order.GetLastAccountNo(LoginUsername)).Trim
                If Not lastAccCode Is Nothing And Not lastAccCode = String.Empty Then
                    basketDetails.Update_Account_Code(lastAccCode, basket.Rows(0)("BASKET_HEADER_ID"))
                End If
                useAnonymousBasket = True
            End If

            If myDefs.ClearBackEndBasketOnLogin And useAnonymousBasket Then
                'Web Sales Back End Basket functionality:
                '----------------------------------------
                'If the Anonymous Basket is to be used, there must be items in it.
                'Therefore, we should re-add the Front End items into the Back End Basket
                For Each item As DataRow In basketItems.Rows
                    If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(item("MODULE"))) Then
                        If item("MODULE").ToString.Trim.ToUpper.Equals("TICKETING") Then
                            If Not Utilities.IsTicketingFee(item("MODULE").ToString.Trim, item("PRODUCT").ToString.Trim, item("FEE_CATEGORY").ToString) Then
                                redirectQuery.Append("&product").Append(itemCount.ToString).Append("=").Append(HttpContext.Current.Server.UrlEncode(Utilities.CheckForDBNull_String(item("PRODUCT"))))
                                'Decide seat or package
                                Dim isPackage As String = String.Empty
                                If (Utilities.CheckForDBNull_String(item("SEAT")).Trim.Length > 0) Then
                                    redirectQuery.Append("&seat").Append(itemCount.ToString).Append("=").Append(HttpContext.Current.Server.UrlEncode(Utilities.CheckForDBNull_String(item("SEAT"))))
                                    isPackage = "N"
                                ElseIf (Utilities.CheckForDBNull_Decimal(item("PACKAGE_ID")) > 0) Then
                                    redirectQuery.Append("&seat").Append(itemCount.ToString).Append("=").Append(HttpContext.Current.Server.UrlEncode(Utilities.CheckForDBNull_String(item("PACKAGE_ID"))))
                                    isPackage = "Y"
                                End If
                                redirectQuery.Append("&isPackage").Append(itemCount.ToString).Append("=").Append(HttpContext.Current.Server.UrlEncode(isPackage))
                                redirectQuery.Append("&customer").Append(itemCount.ToString).Append("=").Append(HttpContext.Current.Server.UrlEncode(Utilities.CheckForDBNull_String(item("LOGINID"))))
                                redirectQuery.Append("&concession").Append(itemCount.ToString).Append("=").Append(HttpContext.Current.Server.UrlEncode(Utilities.CheckForDBNull_String(item("PRICE_BAND"))))
                                redirectQuery.Append("&priceCode").Append(itemCount.ToString).Append("=").Append(HttpContext.Current.Server.UrlEncode(Utilities.CheckForDBNull_String(item("PRICE_CODE"))))
                                redirectQuery.Append("&priceCodeOverridden").Append(itemCount.ToString).Append("=").Append(HttpContext.Current.Server.UrlEncode(Utilities.CheckForDBNull_String(item("PRICE_CODE"))))
                                redirectQuery.Append("&productType").Append(itemCount.ToString).Append("=").Append(HttpContext.Current.Server.UrlEncode(Utilities.CheckForDBNull_String(item("PRODUCT_TYPE"))))
                                redirectQuery.Append("&originalCust").Append(itemCount.ToString).Append("=").Append(GlobalConstants.GENERIC_CUSTOMER_NUMBER)
                                redirectQuery.Append("&fulfilmentMethod").Append(itemCount.ToString).Append("=").Append(HttpContext.Current.Server.UrlEncode(Utilities.CheckForDBNull_String(item("CURR_FULFIL_SLCTN"))))
                                redirectQuery.Append("&bulkSalesID").Append(itemCount.ToString).Append("=").Append(HttpContext.Current.Server.UrlEncode(Utilities.CheckForDBNull_Int(item("BULK_SALES_ID"))))
                                redirectQuery.Append("&bulkSalesQuantity").Append(itemCount.ToString).Append("=").Append(HttpContext.Current.Server.UrlEncode(Utilities.CheckForDBNull_Int(item("QUANTITY"))))
                                itemCount += 1
                            End If
                        End If
                    End If
                Next

                'There is no need to go to the ticketing gateway if there are no tickets in the basket
                If itemCount > 1 Then
                    ticketAddedBeforeLogin = True
                    Dim returnUrl As String = HttpContext.Current.Request("ReturnUrl")
                    If HttpContext.Current.Session("ReservedSeatAvailableToBuy") IsNot Nothing Then
                        If Not String.IsNullOrWhiteSpace(HttpContext.Current.Session("ReservedSeatAvailableToBuy")) Then
                            Try
                                If CBool(HttpContext.Current.Session("ReservedSeatAvailableToBuy")) Then
                                    HttpContext.Current.Session("ReservedSeatAvailableToBuy") = "~/Redirect/TicketingGateway.aspx?page=home.aspx&function=addreserveditemstobasket"
                                End If
                            Catch ex As Exception
                            End Try
                        End If
                    End If
                    If String.IsNullOrEmpty(returnUrl) Then
                        returnUrl = getReturnUrlString()
                    Else
                        returnUrl = "&returnUrl=" & HttpContext.Current.Server.UrlEncode(returnUrl)
                    End If
                    HttpContext.Current.Session("ClearAndAdd") = redirectQuery.ToString()
                    redirectPath = "~/Redirect/TicketingGateway.aspx?page=login.aspx&function=clearandadd" & returnUrl
                End If
            Else
                'Forward the user to the basket if they have something in it. However if they are trying to get to a return url don't attempt to go to the basket
                If Not Profile.IsAnonymous Then
                    If Profile.Basket.BasketItems.Count > 0 Then
                        Dim returnUrl As String = String.Empty
                        If HttpContext.Current.Request.UrlReferrer IsNot Nothing Then returnUrl = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query())("ReturnUrl")
                        If returnUrl Is Nothing OrElse String.IsNullOrEmpty(returnUrl) Then redirectPath = "~/PagesPublic/Basket/Basket.aspx"
                    End If
                End If
            End If

            If HttpContext.Current.Session IsNot Nothing Then
                HttpContext.Current.Session("ReturnUrl") = Nothing
                If HttpContext.Current.Session("ReservedSeatAvailableToBuy") IsNot Nothing And Not ticketAddedBeforeLogin Then
                    If Not String.IsNullOrWhiteSpace(HttpContext.Current.Session("ReservedSeatAvailableToBuy")) Then
                        Try
                            If CBool(HttpContext.Current.Session("ReservedSeatAvailableToBuy")) Then
                                HttpContext.Current.Session.Remove("ReservedSeatAvailableToBuy")
                                Dim returnUrl As String = String.Empty
                                If HttpContext.Current.Request.UrlReferrer IsNot Nothing Then returnUrl = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query())("ReturnUrl")
                                If returnUrl IsNot Nothing AndAlso returnUrl.Length > 0 Then
                                    HttpContext.Current.Session("ReturnUrl") = returnUrl
                                End If
                                redirectPath = "~/Redirect/TicketingGateway.aspx?page=home.aspx&function=addreserveditemstobasket"
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                End If
            End If
        End If
        Utilities.TalentLogging.LoadTestLog("TalentProfileProvider.vb", "MigrateBasketToUser", timeSpan)
        Return redirectPath
    End Function

    Public Function GetUserByLoginID(ByVal loginID As String) As TalentProfileUser
        Return GetUserData(loginID)
    End Function

    ''' <summary>
    ''' Calculates the web pricing for the products and its master product for retail items in the basket and assign the masterproductlist, webprice of profile basket
    ''' </summary>
    ''' <param name="basket">The basket.</param>
    ''' <returns>The Profile Basket</returns>
    Private Function CalculateWebPricing(ByVal userName As String, ByVal isAuthenticated As Boolean, ByVal basket As TalentBasket) As TalentBasket
        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay
        If (basket.BasketItems.Count > 0) Then
            Select Case _pricingModuleDefaultsValues.PricingType
                Case 2
                    basket.WebPrices = GetPrices_Type2(userName, basket)
                Case Else
                    ' Do not call Utilities.GetCurrentPageName as this creates circular reference
                    Dim sPath As String = System.Web.HttpContext.Current.Request.Url.AbsolutePath
                    Dim oInfo As System.IO.FileInfo = New System.IO.FileInfo(sPath)
                    Dim sRet As String = oInfo.Name
                    If sRet <> GlobalConstants.BASKET_PAGE_NAME Or sRet <> GlobalConstants.CHECKOUT_ORDER_SUMMARY_PAGE_NAME Or sRet <> GlobalConstants.CHECKOUT_PAGE_NAME Then
                        Dim finalQtyAndCodes As New Generic.Dictionary(Of String, Talent.Common.WebPriceProduct)
                        If basket.MasterProducts.Count > 0 Then
                            basket.MasterProductsPriceList = GetWebPrices(basket.MasterProducts)
                            For Each tbi As TalentBasketItem In basket.BasketItems
                                If (UCase(tbi.MODULE_) <> "TICKETING") Then
                                    If Not tbi.IS_FREE Then
                                        If Not String.IsNullOrEmpty(tbi.MASTER_PRODUCT) Then
                                            Dim productPriceList As Talent.Common.DEWebPrice = Nothing
                                            If (basket.MasterProductsPriceList.TryGetValue(tbi.MASTER_PRODUCT, productPriceList)) Then
                                                If productPriceList.SALE_PRICE_BREAK_QUANTITY_1 > 0 OrElse productPriceList.PRICE_BREAK_QUANTITY_1 > 0 Then
                                                    'multibuys are configured
                                                    If finalQtyAndCodes.ContainsKey(tbi.MASTER_PRODUCT) Then
                                                        finalQtyAndCodes(tbi.MASTER_PRODUCT).Quantity += tbi.Quantity
                                                    Else
                                                        ' Pass in product otherwise Promotions don't work properly
                                                        finalQtyAndCodes.Add(tbi.MASTER_PRODUCT, New Talent.Common.WebPriceProduct(tbi.Product, tbi.Quantity, tbi.MASTER_PRODUCT))
                                                    End If
                                                Else
                                                    If Not finalQtyAndCodes.ContainsKey(tbi.Product) Then
                                                        finalQtyAndCodes.Add(tbi.Product, New Talent.Common.WebPriceProduct(tbi.Product, tbi.Quantity, tbi.MASTER_PRODUCT))
                                                    End If
                                                End If
                                            Else
                                                If Not finalQtyAndCodes.ContainsKey(tbi.Product) Then
                                                    finalQtyAndCodes.Add(tbi.Product, New Talent.Common.WebPriceProduct(tbi.Product, tbi.Quantity, tbi.MASTER_PRODUCT))
                                                End If
                                            End If
                                        Else
                                            If Not finalQtyAndCodes.ContainsKey(tbi.Product) Then
                                                finalQtyAndCodes.Add(tbi.Product, New Talent.Common.WebPriceProduct(tbi.Product, tbi.Quantity, tbi.MASTER_PRODUCT))
                                            End If
                                        End If
                                    End If
                                End If
                            Next
                        End If
                        If (finalQtyAndCodes.Count > 0) Then
                            Dim prices As Talent.Common.TalentWebPricing
                            prices = CalculateWebPricingTotal(userName, isAuthenticated, finalQtyAndCodes, basket.Temp_Order_Id, basket.Basket_Header_ID, basket.BasketItems.Count, _pricingModuleDefaultsValues.PromotionPriority)
                            basket.WebPrices = prices
                            prices = Nothing
                        Else
                            If (basket.WebPrices Is Nothing) Then
                                basket.WebPrices = SetEmptyTalentWebPricing()
                            End If
                        End If
                        finalQtyAndCodes = Nothing

                    Else
                        basket.WebPrices = GetPrices_Type2(userName, basket)
                    End If
            End Select
        Else
            ' if no products then assign a empty object to webprices
            basket.MasterProductsPriceList = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)()
            If (basket.WebPrices Is Nothing) Then
                basket.WebPrices = SetEmptyTalentWebPricing()
            End If
        End If
        Utilities.TalentLogging.LoadTestLog("TalentProfileProvider.vb", "CalculateWebPricing", timeSpan)
        Return basket
    End Function

    ''' <summary>
    ''' Calculates the web pricing total for the retail products in the basket
    ''' </summary>
    ''' <param name="products">The products.</param>
    ''' <param name="basketTempOrderID">The basket temp order ID.</param>
    ''' <param name="basketHeaderID">The basket header ID.</param>
    ''' <param name="basketItemsCount">The basket items count.</param>
    ''' <param name="PromotionTypePriority">The promotion type priority.</param>
    ''' <param name="PromotionCode">The promotion code.</param>
    ''' <param name="InvalidPromotionCode_ErrorText">The invalid promotion code_ error text.</param>
    ''' <returns>TalentWebPricing Type</returns>
    Private Function CalculateWebPricingTotal(ByVal userName As String, ByVal isAuthenticated As Boolean, _
                                                    ByVal products As Generic.Dictionary(Of String, Talent.Common.WebPriceProduct), _
                                                    ByVal basketTempOrderID As String, _
                                                    ByVal basketHeaderID As String, _
                                                    ByVal basketItemsCount As Integer, _
                                                    ByVal PromotionTypePriority As String, _
                                                    Optional ByVal PromotionCode As String = "", _
                                                    Optional ByVal InvalidPromotionCode_ErrorText As String = "") As Talent.Common.TalentWebPricing
        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay
        Dim _businessUnit As String = String.Empty
        _businessUnit = TalentCache.GetBusinessUnit

        Dim userCntlResource As New Talent.Common.UserControlResource
        With userCntlResource
            .BusinessUnit = PricingBusinessUnit
            .PartnerCode = Partner
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName(False)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "BasketDetails.ascx"
        End With
        InvalidPromotionCode_ErrorText = userCntlResource.Content("InvalidPromotionCodeError", _languageCode, True)
        userCntlResource = Nothing

        If ((HttpContext.Current.Session("CodeFromPromotionsBox") IsNot Nothing) _
            AndAlso (HttpContext.Current.Session("UserFromPromotionsBox") IsNot Nothing) _
            AndAlso (HttpContext.Current.Session("AllPrmotionCodesEnteredByUser") IsNot Nothing)) Then
            If HttpContext.Current.Session("UserFromPromotionsBox").ToString() = userName.Trim().ToLower() Then
                PromotionCode = HttpContext.Current.Session("AllPrmotionCodesEnteredByUser").ToString().Trim()
            End If
        End If

        Dim pricingsettings As New Talent.Common.DEWebPriceSetting(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
                                                            "SQL2005", _
                                                            "", _
                                                            PricingBusinessUnit, _
                                                            Partner, _
                                                            _pricingModuleDefaultsValues.PriceList, _
                                                            False, _
                                                            _languageCode, _
                                                            GetPartnerGroup(), _
                                                            _defaultPriceList, _
                                                            _userAttributeList)
        Dim productPricing As New Talent.Common.TalentWebPricing(pricingsettings, products, Not _pricingModuleDefaultsValues.ShowPricesExVAT)

        If productPricing.Total_Promotions_Value > 0 AndAlso basketTempOrderID IsNot Nothing Then
            Dim orderObject As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
            orderObject.Set_Promotion_Value(productPricing.Total_Promotions_Value, 0, basketTempOrderID)
        End If

        With pricingsettings
            .PromotionCode = PromotionCode
            .InvalidPromotionCodeErrorMessage = InvalidPromotionCode_ErrorText
            .TempOrderID = basketTempOrderID
            .PromotionTypePriority = PromotionTypePriority
            .BasketHeaderID = basketHeaderID
            .Username = userName
        End With

        productPricing.GetWebPricesWithTotals()


        If _pricingModuleDefaultsValues.Call_Tax_WebService AndAlso basketItemsCount > 0 Then
            Dim results As Data.DataSet
            Dim taxWS As New TaxWebService

            results = taxWS.CallTaxWebService("BASKET")
            If results.Tables.Count > 0 Then
                Dim header As Data.DataTable = results.Tables(0)
                If header.Rows.Count > 0 Then
                    If Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(header.Rows(0)("Success")) Then

                        'delivery charges
                        productPricing.Max_Delivery_Gross = Utilities.CheckForDBNull_Decimal(header.Rows(0)("DeliveryGROSS"))
                        productPricing.Max_Delivery_Net = Utilities.CheckForDBNull_Decimal(header.Rows(0)("DeliveryTAX"))
                        productPricing.Max_Delivery_Tax = Utilities.CheckForDBNull_Decimal(header.Rows(0)("DeliveryNET"))

                        'items tax totals
                        productPricing.Total_Items_Tax1 = Utilities.CheckForDBNull_Decimal(header.Rows(0)("TotalTax1"))
                        productPricing.Total_Items_Tax2 = Utilities.CheckForDBNull_Decimal(header.Rows(0)("TotalTax2"))
                        productPricing.Total_Items_Tax3 = Utilities.CheckForDBNull_Decimal(header.Rows(0)("TotalTax3"))
                        productPricing.Total_Items_Tax4 = Utilities.CheckForDBNull_Decimal(header.Rows(0)("TotalTax4"))
                        productPricing.Total_Items_Tax5 = Utilities.CheckForDBNull_Decimal(header.Rows(0)("TotalTax5"))
                        productPricing.Total_Items_Tax = (productPricing.Total_Items_Tax1 _
                                                            + productPricing.Total_Items_Tax2 _
                                                            + productPricing.Total_Items_Tax3 _
                                                            + productPricing.Total_Items_Tax4 _
                                                            + productPricing.Total_Items_Tax5) _
                                                            - productPricing.Max_Delivery_Tax

                        'order totals
                        productPricing.Total_Order_Value_Gross = Utilities.CheckForDBNull_Decimal(header.Rows(0)("TotalGross"))
                        productPricing.Total_Order_Value_Net = productPricing.Total_Order_Value_Gross - productPricing.Total_Items_Tax
                        productPricing.Total_Order_Value_Tax = productPricing.Total_Items_Tax + productPricing.Max_Delivery_Tax

                        'item totals
                        productPricing.Total_Items_Value_Gross = productPricing.Total_Order_Value_Gross - productPricing.Max_Delivery_Gross
                        productPricing.Total_Items_Value_Net = Utilities.CheckForDBNull_Decimal(header.Rows(0)("GoodsTotalNet"))
                    End If
                End If
            End If
        End If
        If productPricing.RetrievedPrices Is Nothing Then
            productPricing.RetrievedPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)()
        End If

        Utilities.TalentLogging.LoadTestLog("TalentProfileProvider.vb", "CalculateWebPricingTotal", timeSpan)
        Return productPricing

    End Function

    Private Function GetWebPrices(ByVal products As Generic.Dictionary(Of String, Talent.Common.WebPriceProduct)) As Generic.Dictionary(Of String, Talent.Common.DEWebPrice)

        Dim pricingsettings As New Talent.Common.DEWebPriceSetting(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
                                                                    "SQL2005", _
                                                                    "", _
                                                                    PricingBusinessUnit, _
                                                                    Partner, _
                                                                    _pricingModuleDefaultsValues.PriceList, _
                                                                    False, _
                                                                    _languageCode, _
                                                                    GetPartnerGroup(), _
                                                                    _defaultPriceList, _
                                                                    _userAttributeList)

        Dim productPricing As New Talent.Common.TalentWebPricing(pricingsettings, products, Not _pricingModuleDefaultsValues.ShowPricesExVAT)

        productPricing.GetWebPrices()
        If productPricing.RetrievedPrices Is Nothing Then
            productPricing.RetrievedPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)()
        End If
        Return productPricing.RetrievedPrices
    End Function

    ''' <summary>
    ''' Returns a TalentWebPricing object that has been populated directly form the basket and it's price values, rather than
    ''' re-pricing everyting from the front-end db as occurs under standard functionality.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetPrices_Type2(ByVal userName As String, ByVal basket As TalentBasket) As Talent.Common.TalentWebPricing

        Dim pricingsettings As New Talent.Common.DEWebPriceSetting(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
                                                                    "SQL2005", _
                                                                    "", _
                                                                    PricingBusinessUnit, _
                                                                    Partner, _
                                                                    _pricingModuleDefaultsValues.PriceList, _
                                                                    False, _
                                                                    _languageCode, _
                                                                    GetPartnerGroup(), _
                                                                    _defaultPriceList, _
                                                                    _userAttributeList)

        Dim products As New Generic.Dictionary(Of String, Talent.Common.WebPriceProduct)
        Dim totals As New Talent.Common.TalentWebPricing(pricingsettings, products, Not _pricingModuleDefaultsValues.ShowPricesExVAT)

        With pricingsettings
            .TempOrderID = basket.Temp_Order_Id
            .BasketHeaderID = basket.Basket_Header_ID
            .Username = userName
        End With

        totals.Total_Items_Tax = 0
        totals.Total_Items_Value_Gross = 0
        totals.Total_Items_Value_Net = 0
        totals.Total_Items_Value_Tax = 0
        totals.Total_Order_Value_Gross = 0
        totals.Total_Order_Value_Net = 0
        totals.Total_Order_Value_Tax = 0
        totals.Total_Promotions_Value = 0
        totals.Max_Delivery_Gross = 0
        totals.Max_Delivery_Net = 0
        totals.Max_Delivery_Tax = 0
        totals.RetrievedPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)()

        For Each tbi As TalentBasketItem In basket.BasketItems
            If (UCase(tbi.MODULE_) <> "TICKETING") Then
                Dim de As New Talent.Common.DEWebPrice
                de.PRODUCT_CODE = tbi.Product
                de.Purchase_Price_Gross = tbi.Gross_Price
                de.Purchase_Price_Net = tbi.Net_Price
                de.Purchase_Price_Tax = tbi.Tax_Price
                de.RequestedQuantity = tbi.Quantity
                de.TAX_CODE = ""
                totals.RetrievedPrices.Add(de.PRODUCT_CODE, de)
            End If
        Next
        If totals.RetrievedPrices IsNot Nothing Then
            totals.CalculateTotals()
        Else
            totals.RetrievedPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)()
        End If
        Return totals
    End Function

    Public Function SetEmptyTalentWebPricing() As Talent.Common.TalentWebPricing
        Dim finalQtyAndCodes As New Generic.Dictionary(Of String, Talent.Common.WebPriceProduct)
        Dim pricingsettings As New Talent.Common.DEWebPriceSetting(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
                                                    "SQL2005", _
                                                    "", _
                                                    PricingBusinessUnit, _
                                                    Partner, _
                                                    _pricingModuleDefaultsValues.PriceList, _
                                                    False, _
                                                    _languageCode, _
                                                    GetPartnerGroup(), _
                                                    _defaultPriceList, _
                                                    _userAttributeList)
        Dim productPricing As New Talent.Common.TalentWebPricing(pricingsettings, finalQtyAndCodes, Not _pricingModuleDefaultsValues.ShowPricesExVAT)
        productPricing.RetrievedPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)()
        Return productPricing
    End Function

    Private Function GetPartnerGroup() As String
        Dim pGroup As String = ""
        Select Case _pricingModuleDefaultsValues.PartnerGroupType
            Case Is = 1
                pGroup = _pricingModuleDefaultsValues.PriceList
        End Select
        Return pGroup
    End Function

    Private Function getReturnUrlString() As String
        Dim returnUrl As String = "&returnUrl=" & HttpContext.Current.Server.UrlEncode(HttpContext.Current.Request.Url.PathAndQuery)
        If HttpContext.Current.Request.UrlReferrer IsNot Nothing Then
            If Not String.IsNullOrWhiteSpace(HttpContext.Current.Request.UrlReferrer.AbsoluteUri) Then
                returnUrl = HttpUtility.ParseQueryString(HttpContext.Current.Request.UrlReferrer.Query())("ReturnUrl")
            End If
        End If
        If returnUrl Is Nothing OrElse String.IsNullOrEmpty(returnUrl) Then
            returnUrl = "&returnUrl=~/PagesPublic/Basket/basket.aspx"
        Else
            returnUrl = "&returnUrl=" & returnUrl
        End If
        Return returnUrl
    End Function

    Private Function GetModifiedWebPrices(ByVal webPricesModifyingMode As Integer, ByVal talBasket As TalentBasket, ByVal currentPageName As String) As TalentBasket
        If webPricesModifyingMode <> Nothing AndAlso webPricesModifyingMode > 0 Then
            Try
                Dim talentWebPriceModifier As New TalentWebPricingModifier(talBasket.Basket_Header_ID, webPricesModifyingMode)
                talentWebPriceModifier.CurrentPageName = currentPageName
                talBasket = talentWebPriceModifier.GetModifiedWebPricesBasket(talBasket)
            Catch ex As Exception
                Utilities.TalentLogging.ExceptionLog("GetModifiedWebPricesBasket", ex)
            End Try
        End If
        Return talBasket
    End Function

End Class
