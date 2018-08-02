Imports System.Text
Imports System.Web
Imports Talent.Common.Utilities

<Serializable()> _
Public Class TalentProduct
    Inherits TalentBase

#Region "Fields and Properties"

    Private _de As New DEProductDetails
    Private _deStock As DEStock
    Private _resultDataSet As DataSet
    Public Property De() As DEProductDetails
        Get
            Return _de
        End Get
        Set(ByVal value As DEProductDetails)
            _de = value
        End Set
    End Property
    Private _dep As New DEProduct
    Public Property Dep() As DEProduct
        Get
            Return _dep
        End Get
        Set(ByVal value As DEProduct)
            _dep = value
        End Set
    End Property
    Private _deProductGroupHierarchies As New DEProductGroupHierarchies
    Public Property DeProductGroupHierarchies() As DEProductGroupHierarchies
        Get
            Return _deProductGroupHierarchies
        End Get
        Set(ByVal value As DEProductGroupHierarchies)
            _deProductGroupHierarchies = value
        End Set
    End Property
    Private _deProductGroups As New DEProductGroups
    Public Property DeProductGroups() As DEProductGroups
        Get
            Return _deProductGroups
        End Get
        Set(ByVal value As DEProductGroups)
            _deProductGroups = value
        End Set
    End Property
    Private _deProductRelations As New DEProductRelations
    Public Property DeProductRelations() As DEProductRelations
        Get
            Return _deProductRelations
        End Get
        Set(ByVal value As DEProductRelations)
            _deProductRelations = value
        End Set
    End Property

    Private _deProductOptions As DEProductOptions
    Public Property DEProductOptions() As DEProductOptions
        Get
            Return _deProductOptions
        End Get
        Set(ByVal value As DEProductOptions)
            _deProductOptions = value
        End Set
    End Property


    Private _productCollection As Collection
    Public Property ProductCollection() As Collection
        Get
            Return _productCollection
        End Get
        Set(ByVal value As Collection)
            _productCollection = value
        End Set
    End Property

    Private _DE_productPriceLoad As DEProductPriceLoad
    Public Property DE_ProductPriceLoad() As DEProductPriceLoad
        Get
            Return _DE_productPriceLoad
        End Get
        Set(ByVal value As DEProductPriceLoad)
            _DE_productPriceLoad = value
        End Set
    End Property

    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property

    Private _deOrder As New DEOrder
    Public Property DEOrder() As DEOrder
        Get
            Return _deOrder
        End Get
        Set(ByVal value As DEOrder)
            _deOrder = value
        End Set
    End Property

    Private _dePna As New DePNA
    Public Property DePNA() As DePNA
        Get
            Return _dePna
        End Get
        Set(ByVal value As DePNA)
            _dePna = value
        End Set
    End Property

    Public Property Stock() As DEStock
        Get
            Return _deStock
        End Get
        Set(ByVal value As DEStock)
            _deStock = value
        End Set
    End Property

    Public Property AgentLevelCacheForProductStadiumAvailability() As Boolean = False

#End Region

#Region "Public Methods"

    Public Function ProductList() As ErrorObj
        Const ModuleName As String = "ProductList"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)
        Settings.ModuleName = ModuleName

        Dim err As New ErrorObj
        Dim cacheKey As String = ProductListCacheKey()
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Dim dbProduct As New DBProduct
            With dbProduct
                .Settings = Settings
                .De = De
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductListClearCache() As ErrorObj
        'We need to clear both the website and the agent cache as the product will be close to sold out when this function is called.
        Dim saveAgent As Boolean = Settings.IsAgent()
        Settings.IsAgent = False
        HttpContext.Current.Cache.Remove(ProductListCacheKey)
        Settings.IsAgent = True
        HttpContext.Current.Cache.Remove(ProductListCacheKey)
        Settings.IsAgent = saveAgent
        Return New ErrorObj
    End Function

    Public Function ProductListCacheKey() As String

        Dim cacheKey As String = "ProductList" & Settings.Company & De.ProductType
        If Not String.IsNullOrEmpty(De.CustomerNumber) Then
            cacheKey += "Cust=" & De.CustomerNumber
        End If
        If Not String.IsNullOrEmpty(De.StadiumCode) Then
            cacheKey += "Stad=" & De.StadiumCode
        End If
        If Not String.IsNullOrEmpty(De.PPSType) Then
            cacheKey += "PPS=" & De.PPSType
        End If
        'Sales channel for web
        cacheKey += "Agent=" & Settings.IsAgent
        'Sub types
        If Not String.IsNullOrWhiteSpace(De.productSubtype) Then
            cacheKey += "subtype=" & De.productSubtype
        End If

        Return cacheKey
    End Function

    Public Function AvailableStandsWithoutDescriptions() As ErrorObj
        Const ModuleName As String = "AvailableStandsWithoutDescriptions"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj
        Dim cacheKey As String = AvailableStandsWithoutDescriptionsCacheKey()
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .De = De
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        Settings.ModuleName = "ProductList"
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                        Settings.ModuleName = ModuleName
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function StandDescriptions() As ErrorObj
        Const ModuleName As String = "StandDescriptions"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & De.StadiumCode & De.WaitList & De.ProductCode
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .De = De
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function AvailableStands() As ErrorObj
        Const ModuleName As String = "AvailableStands"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj
        Dim dtAvailableStands As DataTable
        Dim dtStandDescriptions As DataTable
        Dim dsAvailableStands As DataSet
        Dim dr, dr2 As DataRow
        Dim cacheTime As Integer = Settings.CacheTimeMinutes
        Dim cacheKey As String = AvailableStandsCacheKey()

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            err = ProductStadiumAvailability()
            If Not ResultDataSet Is Nothing AndAlso Not err.HasError Then

                'Save the result set for later use and set the stadium code
                dsAvailableStands = ResultDataSet.Copy
                dtAvailableStands = dsAvailableStands.Tables(1)
                dr = dsAvailableStands.Tables(0).Rows(0)
                De.StadiumCode = dr("StadiumCode")

                'Set the cache time for the stand descriptions function
                Settings.CacheTimeMinutes = Settings.CacheTimeMinutesSecondFunction

                'Retrieve the stand descriptions
                ResultDataSet = Nothing
                StandDescriptions()
                If Not ResultDataSet Is Nothing Then
                    dtStandDescriptions = ResultDataSet.Tables(1)

                    'Loop around the available stands adding the description
                    Dim searchKey As String
                    For Each dr In dtAvailableStands.Rows

                        'Retrieve the descriptions
                        Dim selectedRecords() As DataRow = Nothing
                        searchKey = "StandCode='" & dr("StandCode").ToString.Trim & "' AND AreaCode='" & dr("AreaCode").ToString.Trim & "'"
                        selectedRecords = dtStandDescriptions.Select(searchKey)

                        'Populate the data row with the description
                        If selectedRecords.Length > 0 Then
                            dr2 = selectedRecords(0)
                            dr("StandDescription") = dr2("StandDescription")
                            dr("AreaDescription") = dr2("AreaDescription")
                        End If
                    Next dr

                    'Cache the result set
                    ResultDataSet = Nothing
                    ResultDataSet = dsAvailableStands
                    Settings.CacheTimeMinutes = cacheTime

                    'AvailableStands should be associated with Productlist cache
                    Settings.ModuleName = "ProductList"
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    Settings.ModuleName = ModuleName
                End If
            End If
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductDetails() As ErrorObj
        Const ModuleName As String = "ProductDetails"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)
        Dim err As New ErrorObj
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim cacheKey As String = ModuleName & Settings.Company & De.ProductCode & De.PriceCode & De.AllowPriceException & Settings.IsAgent
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .De = De
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    Settings.ModuleName = "ProductList"
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    Settings.ModuleName = ModuleName
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        Settings.ModuleName = "ProductList"
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                        Settings.ModuleName = ModuleName

                        Dim rowsAffected As Integer = 0
                        If De.SVGStadiumDescriptionAvailable Then
                            rowsAffected = TDataObjects.StadiumSettings.TblStadiumOverride.InsertProductStadium(De.ProductCode, ResultDataSet.Tables("ProductDetails").Rows(0)("SVGStadiumDescription"))
                            If rowsAffected > 0 Then
                                TDataObjects.StadiumSettings.TblStadiumOverride.RemoveOverridenStadiumCacheKey(De.ProductCode, Settings.BusinessUnit)
                            End If
                        Else
                            Dim outputDataTable As New DataTable
                            outputDataTable = TDataObjects.StadiumSettings.TblStadiumOverride.GetByProductCode(De.ProductCode)

                            'Delete if product code exists
                            If outputDataTable.Rows.Count > 0 Then
                                rowsAffected = TDataObjects.StadiumSettings.TblStadiumOverride.DeleteByID(outputDataTable.Rows(0)(0))
                            End If

                            If rowsAffected > 0 Then
                                TDataObjects.StadiumSettings.TblStadiumOverride.RemoveOverridenStadiumCacheKey(De.ProductCode, Settings.BusinessUnit)
                            End If

                        End If
                    End If
                End If
            End With
        End If

        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductDetails(ByVal settingsObj As DESettings, ByVal ProductCode As String,
                                        Optional ByVal PriceCode As String = "",
                                        Optional ByVal Caching As Boolean = True,
                                        Optional CacheTimeMinutes As Integer = 30) As ErrorObj
        Dim err As New ErrorObj

        Settings = settingsObj
        Settings.Cacheing = Caching
        Settings.CacheTimeMinutes = CacheTimeMinutes
        De.ProductCode = ProductCode
        De.PriceCode = PriceCode
        De.Src = settingsObj.OriginatingSourceCode
        err = ProductDetails()

        Return err
    End Function

    Public Function ProductPricingDetails() As ErrorObj
        Const ModuleName As String = "ProductPricingDetails"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type od cache, Company name and all relevent 
        '   incoming unique keys, If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database
        '
        Dim cacheKey As String = ModuleName & Settings.Company & De.ProductCode & Utilities.CheckForDBNull_Boolean_DefaultFalse(Settings.IsAgent).ToString
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .De = De
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings, "ProductList")
                        ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductStadiumAvailability() As ErrorObj
        Const ModuleName As String = "ProductStadiumAvailability"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj
        Dim cacheKey As New StringBuilder
        cacheKey.Append(ModuleName).Append(Settings.Company).Append(De.ProductCode).Append("Camp=").Append(De.CampaignCode).Append("CAT=").Append(De.CATMode)
        cacheKey.Append("IsAgent=").Append(Settings.IsAgent).Append("ComponentID=").Append(De.ComponentID).Append("PriceBreakId=").Append(De.PriceBreakId.ToString())
        cacheKey.Append("IncludeTicketExchangeSeats=").Append(De.IncludeTicketExchangeSeats).Append("MinPrice=").Append(De.SelectedMinimumPrice.ToString()).Append("MaxPrice=").Append(De.SelectedMaximumPrice.ToString())
        If AgentLevelCacheForProductStadiumAvailability Then
            cacheKey = cacheKey.Append("Agent=").Append(Settings.OriginatingSource)
        End If
        Dim cacheKeyString As String = cacheKey.ToString()

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKeyString) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKeyString), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .Settings = Settings
                .De = De
                .AgentLevelCacheForProductStadiumAvailability = AgentLevelCacheForProductStadiumAvailability
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKeyString, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        Settings.ModuleName = "ProductList"
                        AddItemToCache(cacheKey.ToString(), ResultDataSet, Settings)
                        Settings.ModuleName = ModuleName
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function CourseAvailabilityPreCheck() As ErrorObj
        Const ModuleName As String = "CourseAvailabilityPreCheck"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type od cache, Company name and all relevent 
        '   incoming unique keys, If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database
        '
        Dim cacheKey As String = ModuleName & Settings.Company & De.ProductCode & De.CampaignCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .Settings = Settings
                .De = De
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductSeatAvailability() As ErrorObj
        Const ModuleName As String = "ProductSeatAvailability"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)
        Dim err As New ErrorObj
        Dim cacheKey As New StringBuilder
        cacheKey.Append(ModuleName).Append("Comp=").Append(Settings.Company).Append("Product=").Append(De.ProductCode).Append("Stand=").Append(De.StandCode)
        cacheKey.Append("Area=").Append(De.AreaCode).Append("Campaign=").Append(De.CampaignCode).Append("PriceBreakID=").Append(De.PriceBreakId).Append(De.SelectedMinimumPrice).Append(De.SelectedMaximumPrice)
        If Settings.IsAgent Then
            cacheKey.Append("User=BoxOffice")
        End If

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey.ToString()) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey.ToString()), DataSet)
        End If
        If ResultDataSet Is Nothing Then
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .Settings = Settings
                .De = De
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey.ToString(), ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey.ToString(), ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductSeatAvailabilityClearCache() As Boolean
        Dim cacheCleared As Boolean = False
        Const ModuleName As String = "ProductSeatAvailability"
        Dim cacheKey As New StringBuilder
        cacheKey.Append(ModuleName).Append("Comp=").Append(Settings.Company).Append("Product=").Append(De.ProductCode).Append("Stand=").Append(De.StandCode)
        cacheKey.Append("Area=").Append(De.AreaCode).Append("Campaign=").Append(De.CampaignCode).Append("PriceBreakID=").Append(De.PriceBreakId)
        If Settings.IsAgent Then
            cacheKey.Append("User=BoxOffice")
        End If
        If HttpContext.Current.Cache.Item(cacheKey.ToString()) IsNot Nothing Then
            HttpContext.Current.Cache.Remove(cacheKey.ToString())
            cacheCleared = True
        End If
        Return cacheCleared
    End Function

    Public Function ProductSeatNumbers() As ErrorObj
        Const ModuleName As String = "ProductSeatNumbers"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type od cache, Company name and all relevent 
        '   incoming unique keys, If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database
        '
        Dim cacheKey As String = ModuleName & "Comp=" & Settings.Company & "Product=" & De.ProductCode & "Stand=" & De.StandCode & "Area=" & De.AreaCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .Settings = Settings
                .De = De
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    '  HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                        ' HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductSeatRestrictions() As ErrorObj
        Const ModuleName As String = "ProductSeatRestrictions"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type od cache, Company name and all relevent 
        '   incoming unique keys, If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database
        '

        Dim cacheKey As String = ModuleName & "Comp=" & Settings.Company & "Product=" & De.ProductCode & "Stand=" & De.StandCode & "Area=" & De.AreaCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            ResultDataSet = Nothing
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .Settings = Settings
                .De = De
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function AvailableStandsClearCache() As ErrorObj
        Dim cacheKey As String = AvailableStandsCacheKey()
        HttpContext.Current.Cache.Remove(cacheKey)
        cacheKey = AvailableStandsWithoutDescriptionsCacheKey()
        HttpContext.Current.Cache.Remove(cacheKey)
        Return New ErrorObj
    End Function

    Public Function AvailableStandsWithoutDescriptionsClearCache() As ErrorObj
        Dim cacheKey As String = AvailableStandsWithoutDescriptionsCacheKey()
        HttpContext.Current.Cache.Remove(cacheKey)
        Return New ErrorObj
    End Function

    Public Function AvailableStandsCacheKey() As String
        If De.CampaignCode Is Nothing Then De.CampaignCode = String.Empty
        Dim cacheKey As New StringBuilder
        cacheKey.Append("AvailableStands").Append(Settings.Company.Trim).Append(De.ProductCode.Trim).Append(De.CampaignCode.Trim).Append(De.AvailableToSellAvailableTickets).Append(De.AvailableToSell03).Append(De.ComponentID).Append(De.SelectedMinimumPrice).Append(De.SelectedMaximumPrice).Append(De.PriceBreakId)
        Return cacheKey.ToString
    End Function

    Public Function AvailableStandsWithoutDescriptionsCacheKey() As String
        If De.CampaignCode Is Nothing Then De.CampaignCode = String.Empty
        Dim cacheKey As New StringBuilder
        cacheKey.Append("AvailableStandsWithoutDescriptions").Append(Settings.Company.Trim).Append(De.ProductCode.Trim).Append(De.CampaignCode.Trim).Append(De.AvailableToSellAvailableTickets).Append(De.AvailableToSell03).Append(De.ComponentID)
        cacheKey.Append(De.PriceBreakId.ToString()).Append(Utilities.CheckForDBNull_Boolean_DefaultFalse(Settings.IsAgent).ToString)
        Return cacheKey.ToString()
    End Function

    Public Function AvailableTravel() As ErrorObj
        Return New ErrorObj
    End Function

    Public Function RetrieveEligibleTicketingCustomers() As ErrorObj
        Const ModuleName As String = "RetrieveEligibleTicketingCustomers"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)
        Settings.ModuleName = ModuleName

        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type od cache, Company name and all relevent 
        '   incoming unique keys, If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database
        '
        Dim cacheKey As String = ModuleName & Settings.Company & De.ProductType
        If Not String.IsNullOrEmpty(De.PPSType) Then
            cacheKey += "PPS" & De.PPSType
        End If
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            '  Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .Settings = Settings
                .De = De
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    '  HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        'HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductNavigationLoad() As ErrorObj
        Const ModuleName As String = "ProductNavigationLoad"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Settings.ModuleName = ModuleName
        Dim dbProduct As New DBProduct
        With dbProduct
            .Settings = Settings
            .De = De
            .DeProductGroupHierarchies = DeProductGroupHierarchies
            .DeProductGroups = DeProductGroups

            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductStockLoad() As ErrorObj
        Const ModuleName As String = "ProductStockLoad"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Settings.ModuleName = ModuleName
        Dim dbProduct As New DBProduct
        With dbProduct
            .Settings = Settings
            .De = De
            .Stock = Stock

            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductLoad() As ErrorObj
        Const ModuleName As String = "ProductLoad"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Settings.ModuleName = ModuleName
        Dim dbProduct As New DBProduct
        With dbProduct
            .Settings = Settings
            .De = De
            .Dep = Dep
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductOptionsLoad() As ErrorObj
        Const ModuleName As String = "ProductOptionsLoad"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request")

        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Settings.ModuleName = ModuleName
        Dim dbProduct As New DBProduct
        With dbProduct
            .Settings = Settings
            .Depo = Me.DEProductOptions
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With

        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)

        Return err
    End Function

    Public Function ProductRelationsLoad() As ErrorObj
        Const ModuleName As String = "ProductRelationsLoad"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Settings.ModuleName = ModuleName
        Dim dbProduct As New DBProduct
        With dbProduct
            .Settings = Settings
            .De = De
            .DeProductRelations = DeProductRelations
            .Dep = Dep
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function RetrieveAlternativeProducts() As ErrorObj
        Const ModuleName As String = "RetrieveAlternativeProducts"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Settings.ModuleName = ModuleName

        'Dim cacheKey As String = ModuleName & Settings.Company & De.ProductType
        'If Not String.IsNullOrEmpty(De.PPSType) Then
        '    cacheKey += "RetrieveAlternativeProducts" & De.PPSType
        'End If
        'If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
        '    ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        'Else

        Dim dbProduct As New DBProduct
        With dbProduct
            .Settings = Settings
            .ProductCollection = ProductCollection
            .DeProductRelations = DeProductRelations
            .Dep = Dep
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    ' AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End If
        End With
        ' End If

        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductListReturnAll() As ErrorObj
        Const ModuleName As String = "ProductListReturnAll"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)
        Settings.ModuleName = ModuleName

        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        '   Cache key should be constructed Type od cache, Company name and all relevent 
        '   incoming unique keys, If cacheing enabled for this web service and there is 
        '   something contained within the cache, use it instead of going back to the database
        '
        Dim cacheKey As String = ModuleName & Settings.Company & De.ProductType
        If Not String.IsNullOrEmpty(De.CustomerNumber) Then
            cacheKey += "Cust=" & De.CustomerNumber
        End If
        If Not String.IsNullOrEmpty(De.StadiumCode) Then
            cacheKey += "Stad=" & De.StadiumCode
        End If
        If Not String.IsNullOrEmpty(De.PPSType) Then
            cacheKey += "PPS=" & De.PPSType
        End If
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            '  Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .Settings = Settings
                .De = De
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    '  HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        '                        ResultDataSet = .addPreReqList(ResultDataSet)
                        '                        ResultDataSet = .addLoyaltySchedule(ResultDataSet)
                        'HttpContext.Current.Cache.Insert(cacheKey, ResultDataSet, Nothing, System.DateTime.Now.AddMinutes(Settings.CacheTimeMinutes()), Caching.Cache.NoSlidingExpiration)
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductPriceLoad() As ErrorObj
        Const ModuleName As String = "ProductPriceLoad"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Settings.ModuleName = ModuleName
        Dim dbProduct As New DBProduct
        With dbProduct
            .Settings = Settings
            .De = De
            .DeProductPriceLoad = DE_ProductPriceLoad
            .Dep = Dep
            err = .ValidateAgainstDatabase()
            If Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
            If Not err.HasError And ResultDataSet Is Nothing Then
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductHospitality() As ErrorObj
        Const ModuleName As String = "ProductHospitality"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & De.ProductCode & Settings.BusinessUnit
        Settings.ModuleName = ModuleName
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Dim dbProduct As New DBProduct
            With dbProduct
                .De = De
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductHospitalityClearCache() As ErrorObj
        Dim cacheKey As String = ProductHospitalityCacheKey(De.ProductCode)
        HttpContext.Current.Cache.Remove(cacheKey)
        Return New ErrorObj
    End Function

    Public Function ProductHospitalityCacheKey(ByVal productCode As String) As String
        Return "ProductHospitality" & Settings.Company.Trim & productCode.Trim
    End Function

    Public Function ProductSubTypesList() As ErrorObj
        Const ModuleName As String = "ProductSubTypesList"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request")

        Dim err As New ErrorObj

        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)

        Dim cacheKey As String = ModuleName & De.ProductDate & De.StadiumCode & Settings.Company
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else

            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .Settings = Settings
                .De = Me.De
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        'ProductDetails should be associated with Productlist cache
                        Settings.ModuleName = "ProductList"
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                        Settings.ModuleName = ModuleName
                    End If
                End If
            End With
        End If

        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)

        Return err
    End Function

    Public Function ProductSeatDetails() As ErrorObj
        Const ModuleName As String = "ProductSeatDetails"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)
        Dim err As New ErrorObj
        Dim cacheKey As New StringBuilder
        cacheKey.Append(ModuleName).Append(Settings.Company).Append(De.ProductCode)
        cacheKey.Append(De.StandCode).Append("\").Append(De.AreaCode).Append("\").Append(De.SeatRow).Append("\").Append(De.SeatNumber)

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey.ToString()) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey.ToString()), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .Settings = Settings
                .De = De
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey.ToString(), ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey.ToString(), ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function ProductReservationCodes() As ErrorObj
        Const ModuleName As String = "RetrieveProductReservationCodes"
        Dim err As New ErrorObj
        Dim dbProduct As New DBProduct
        Dim cacheKey As String = ModuleName & "Comp=" & Settings.Company & "Product=" & De.ProductCode & "Stand=" & De.StandCode & "Area=" & De.AreaCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
            Settings.ModuleName = ModuleName
            With dbProduct
                .De = De
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError Then
                    err = .AccessDatabase()
                End If
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings, "ReservedSeats" & De.ProductCode)
                End If
            End With
        End If

        Return err

    End Function

    ''' <summary>
    ''' Retrieves the history for a given seat based on product and payment reference
    ''' </summary>
    ''' <returns>Error object based on the results</returns>
    ''' <remarks></remarks>
    Public Function RetrieveSeatHistory() As ErrorObj
        Const ModuleName As String = "RetrieveSeatHistory"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With dbProd
            .De = De
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Retrieves the seat print history for the given seat based on the product code
    ''' </summary>
    ''' <returns>Error Object</returns>
    ''' <remarks></remarks>
    Public Function RetrieveSeatPrintHistory() As ErrorObj
        Const ModuleName As String = "RetrieveSeatPrintHistory"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With dbProd
            .De = De
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Amend the product addtional information
    ''' </summary>
    ''' <returns>Error Object</returns>
    ''' <remarks></remarks>
    Public Function AmendAdditionalProductInformationTemplate() As ErrorObj
        Const ModuleName As String = "ProductAdditionalInformationOption"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With dbProd
            .De = De
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Add product questions and answer data
    ''' </summary>
    ''' <returns>Error Object</returns>
    ''' <remarks></remarks>
    Public Function AddProductQuestionAnswers() As ErrorObj
        Const ModuleName As String = "ProductAdditionalInformation"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With dbProd
            .De = De
            .Dep = Dep
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Retrieve the product questions and answer text
    ''' </summary>
    ''' <returns>Error Object</returns>
    ''' <remarks></remarks>
    Public Function RetrieveProductQuestionAnswers() As ErrorObj
        Const ModuleName As String = "RetrieveProductQuestionAnswers"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With dbProd
            .Dep = Dep
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    Public Function RetrieveProductsFiltered() As ErrorObj
        Const ModuleName As String = "RetrieveProductsFiltered"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        Dim cacheKey As String = ModuleName & "SuperType=" & De.ProductSupertype.Trim & "SubType=" & De.ProductSubtype.Trim & "PastYears=" & De.YearsOfPastProductsToShow.ToString
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
        ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With dbProd
            .De = De
            .Dep = Dep
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
                AddItemToCache(cacheKey, ResultDataSet, Settings)
            End If
        End With
        End If
        Return err
    End Function

    Public Function RetrieveStadiums() As ErrorObj
        Const ModuleName As String = "RetrieveStadiums"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        Dim cacheKey As String = ModuleName
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
            Settings.ModuleName = ModuleName
            With dbProd
                .De = De
                .Dep = Dep
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError Then
                    err = .AccessDatabase()
                End If
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End With
        End If
        Return err
    End Function

    Public Function RetrieveStadiumStands() As ErrorObj
        Const ModuleName As String = "RetrieveStadiumStands"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        Dim cacheKey As String = ModuleName & "Stadium=" & De.StadiumCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
            Settings.ModuleName = ModuleName
            With dbProd
                .De = De
                .Dep = Dep
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError Then
                    err = .AccessDatabase()
                End If
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End With
        End If
        Return err
    End Function

    Public Function RetrieveStadiumStandAreas() As ErrorObj
        Const ModuleName As String = "RetrieveStadiumStandAreas"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        Dim cacheKey As String = ModuleName & "Stadium=" & De.StadiumCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
            Settings.ModuleName = ModuleName
            With dbProd
                .De = De
                .Dep = Dep
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not err.HasError Then
                    err = .AccessDatabase()
                End If
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
            End With
        End If
        Return err
    End Function

    ''' <summary>
    ''' Insert new package into LP001TBL
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateLinkedProductPackage() As ErrorObj
        Const ModuleName As String = "CreateLinkedProductPackage"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With dbProd
            .Dep = Dep
            .De = De
            .De.LinkedProductPackageMode = "C"
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Update package in LP001TBL
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function UpdateLinkedProductPackage() As ErrorObj
        Const ModuleName As String = "UpdateLinkedProductPackage"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With dbProd
            .Dep = Dep
            .De = De
            .De.LinkedProductPackageMode = "U"
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Updates ACTR01 in LP001TBL to 'D'
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteLinkedProductPackage() As ErrorObj
        Const ModuleName As String = "DeleteLinkedProductPackage"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With dbProd
            .Dep = Dep
            .De = De
            .De.LinkedProductPackageMode = "D"
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Get all the away product availability details
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function AwayProductAvailability() As ErrorObj
        Const ModuleName As String = "AwayProductAvailability"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString)
        GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim cacheKey As String = ModuleName

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            With dbProd
                dbProd.De = De
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    Settings.ModuleName = "ProductList"
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    Settings.ModuleName = ModuleName
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Get all the travel product availability details
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function TravelProductAvailability() As ErrorObj
        Const ModuleName As String = "TravelProductAvailability"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString)
        GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim cacheKey As String = ModuleName

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            With dbProd
                dbProd.De = De
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    Settings.ModuleName = "ProductList"
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    Settings.ModuleName = ModuleName
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Get all the travel product availability details
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function EventProductAvailability() As ErrorObj
        Const ModuleName As String = "EventProductAvailability"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        TalentCommonLog(ModuleName, "", "Talent.Common Request = De=" & De.LogString)
        GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        Dim cacheKey As String = ModuleName

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            With dbProd
                dbProd.De = De
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    Settings.ModuleName = "ProductList"
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    Settings.ModuleName = ModuleName
                End If
            End With
        End If
        TalentCommonLog(ModuleName, "", ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Retrieve the product price break details per product
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function ProductPriceBreaks() As ErrorObj
        Const ModuleName As String = "ProductPriceBreaks"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        TalentCommonLog(ModuleName, String.Empty, "Talent.Common Request = De=" & De.LogString)
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        Dim cacheKey As String = ModuleName & De.ProductCode & Settings.IsAgent

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            With dbProd
                dbProd.De = De
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    Settings.ModuleName = "ProductList"
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                    Settings.ModuleName = ModuleName
                End If
            End With
        End If
        TalentCommonLog(ModuleName, String.Empty, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Retrieve the price break details for the seating by given product, stand and area
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function PriceBreakSeatDetails() As ErrorObj
        Const ModuleName As String = "PriceBreakSeatDetails"
        Dim err As New ErrorObj
        Dim dbProd As New DBProduct
        Dim cacheKey As New StringBuilder

        TalentCommonLog(ModuleName, String.Empty, "Talent.Common Request = De=" & De.LogString)
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        cacheKey.Append(ModuleName).Append(Utilities.FixStringLength(De.ProductCode, 6)).Append(Utilities.FixStringLength(De.StandCode, 3)).Append(Utilities.FixStringLength(De.AreaCode, 4)).Append(Utilities.FixStringLength(De.CampaignCode, 2))

        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey.ToString()) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey.ToString()), DataSet)
        Else
            With dbProd
                dbProd.De = De
                .Settings = Settings
                err = .AccessDatabase()
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey.ToString(), ResultDataSet, Settings)
                End If
            End With
        End If
        TalentCommonLog(ModuleName, String.Empty, ResultDataSet, err)
        Return err
    End Function

    Public Function RetrieveProductsByDescription() As ErrorObj
        Const ModuleName As String = "RetrieveProductsByDescription"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & De.ProductDescription
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .De = De
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.ProductDescription, ResultDataSet, err)
        Return err
    End Function
    Public Function RetrieveCorporatePackagesByDescription() As ErrorObj
        Const ModuleName As String = "RetrieveCorporatePackagesByDescription"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & De.ProductDescription
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .De = De
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.ProductDescription, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Retrieve hospitality product group details
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function RetrieveProductGroups() As ErrorObj
        Const ModuleName As String = "RetrieveProductGroups"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & De.ProductGroupCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .De = De
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Retrieve hospitality product group Fixtures by product group code
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function RetrieveProductGroupFixtures() As ErrorObj
        Const ModuleName As String = "RetrieveProductGroupFixtures"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & De.ProductGroupCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .De = De
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Retrieve hospitality product group packages by product group code
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function RetrieveProductGroupPackages() As ErrorObj
        Const ModuleName As String = "RetrieveProductGroupPackages"
        TalentCommonLog(ModuleName, De.CustomerNumber, "Talent.Common Request = De=" & De.LogString)

        Dim err As New ErrorObj
        Dim cacheKey As String = ModuleName & Settings.Company & De.ProductGroupCode
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            ResultDataSet = CType(HttpContext.Current.Cache.Item(cacheKey), DataSet)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Settings.ModuleName = ModuleName
            Dim dbProduct As New DBProduct
            With dbProduct
                .De = De
                .Settings = Settings
                err = .ValidateAgainstDatabase()
                If Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                    AddItemToCache(cacheKey, ResultDataSet, Settings)
                End If
                If Not err.HasError And ResultDataSet Is Nothing Then
                    err = .AccessDatabase()
                    If Not err.HasError And Not .ResultDataSet Is Nothing Then
                        ResultDataSet = .ResultDataSet
                        AddItemToCache(cacheKey, ResultDataSet, Settings)
                    End If
                End If
            End With
        End If
        TalentCommonLog(ModuleName, De.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

#End Region

#Region "Private Methods"
    Private Function addDummyPreReqList(ByVal ResultDataSetToModify As DataSet) As DataSet
        ResultDataSetToModify.Tables(1).Columns.Add("PreReqProductGroup")
        ResultDataSetToModify.Tables(1).Columns.Add("PreReqDescription")
        ResultDataSetToModify.Tables(1).Columns.Add("PreReqMultiGroup")
        ResultDataSetToModify.Tables(1).Columns.Add("PreReqStadium")
        ResultDataSetToModify.Tables(1).Columns.Add("PreReqValidationRule")
        ResultDataSetToModify.Tables(1).Columns.Add("PreReqComments")

        For Each row As DataRow In ResultDataSetToModify.Tables(1).Rows
            row("PreReqProductGroup") = "pre-req group"
            row("PreReqDescription") = "A description"
            row("PreReqMultiGroup") = "A Multi-Group"
            row("PreReqStadium") = "Stadium codes"
            row("PreReqValidationRule") = "validation rules"
            row("PreReqComments") = "some comments go here"
        Next

        Dim dt As DataTable = New DataTable()
        dt.Columns.Add("ProductCode")
        dt.Columns.Add("PreReqProductCode")
        dt.Columns.Add("PreReqProductDescription")
        dt.Columns.Add("PreReqProductType")
        dt.Columns.Add("PreReqProductDate")

        Dim index As Integer = 0
        While Not index = ResultDataSetToModify.Tables(1).Rows.Count
            Dim row2 As DataRow = dt.NewRow()
            row2("ProductCode") = ResultDataSetToModify.Tables(1).Rows(index)("ProductCode")
            row2("PreReqProductCode") = "CODE1"
            row2("PreReqProductDescription") = "Description for product (prereq)"
            row2("PreReqProductType") = "type1"
            row2("PreReqProductDate") = "2009-03-17"
            dt.Rows.Add(row2)

            Dim row3 As DataRow = dt.NewRow()
            row3("ProductCode") = ResultDataSetToModify.Tables(1).Rows(index)("ProductCode")
            row3("PreReqProductCode") = "CODE2"
            row3("PreReqProductDescription") = "Description for product (prereq)"
            row3("PreReqProductType") = "type2"
            row3("PreReqProductDate") = "2009-03-18"
            dt.Rows.Add(row3)

            Dim row4 As DataRow = dt.NewRow()
            row4("ProductCode") = ResultDataSetToModify.Tables(1).Rows(index)("ProductCode")
            row4("PreReqProductCode") = "CODE2"
            row4("PreReqProductDescription") = "Description for product (prereq)"
            row4("PreReqProductType") = "type3"
            row4("PreReqProductDate") = "2009-03-19"
            dt.Rows.Add(row4)
            index = index + 1
        End While

        ResultDataSetToModify.Tables.Add(dt)
        Return ResultDataSetToModify
    End Function

    Private Function addDummyLoyaltySchedule(ByVal ResultDataSetToModify As DataSet) As DataSet
        ResultDataSetToModify.Tables(1).Columns.Add("LoyaltyDetailsApplyRestriction")
        ResultDataSetToModify.Tables(1).Columns.Add("LoyaltyDetailsNoOfPointsAwarded")
        ResultDataSetToModify.Tables(1).Columns.Add("LoyaltyDetailsUpdatePreviouslyAwardedPoints")
        ResultDataSetToModify.Tables(1).Columns.Add("LoyaltyDetailsUpdateFromDate")
        ResultDataSetToModify.Tables(1).Columns.Add("LoyaltyDetailsUpdateToDate")
        ResultDataSetToModify.Tables(1).Columns.Add("LoyaltyDetailsNoOfPurchasePointsAwarded")
        ResultDataSetToModify.Tables(1).Columns.Add("LoyaltyDetailsAwardToSeasonTicketHolders")
        ResultDataSetToModify.Tables(1).Columns.Add("LoyaltyDetailsSeasonTicketID")

        For Each row As DataRow In ResultDataSetToModify.Tables(1).Rows
            row("LoyaltyDetailsApplyRestriction") = "Y"
            row("LoyaltyDetailsNoOfPointsAwarded") = "1"
            row("LoyaltyDetailsUpdatePreviouslyAwardedPoints") = "Y"
            row("LoyaltyDetailsUpdateFromDate") = "2008-08-02"
            row("LoyaltyDetailsUpdateToDate") = "2008-09-02"
            row("LoyaltyDetailsNoOfPurchasePointsAwarded") = "5"
            row("LoyaltyDetailsAwardToSeasonTicketHolders") = "Y"
            row("LoyaltyDetailsSeasonTicketID") = "0000000001"
        Next

        Dim dt As DataTable = New DataTable()
        dt.Columns.Add("ProductCode")
        dt.Columns.Add("From")
        dt.Columns.Add("RequiredPoints")

        Dim index As Integer = 0
        While Not index = ResultDataSetToModify.Tables(1).Rows.Count
            Dim row2 As DataRow = dt.NewRow()
            row2("ProductCode") = ResultDataSetToModify.Tables(1).Rows(index)("ProductCode")
            row2("From") = "2008-08-02"
            row2("RequiredPoints") = "5"
            dt.Rows.Add(row2)

            Dim row3 As DataRow = dt.NewRow()
            row3("ProductCode") = ResultDataSetToModify.Tables(1).Rows(index)("ProductCode")
            row3("From") = "2008-08-02"
            row3("RequiredPoints") = "4"
            dt.Rows.Add(row3)

            Dim row4 As DataRow = dt.NewRow()
            row4("ProductCode") = ResultDataSetToModify.Tables(1).Rows(index)("ProductCode")
            row4("From") = "2008-08-02"
            row4("RequiredPoints") = "3"
            dt.Rows.Add(row4)
            index = index + 1
        End While

        ResultDataSetToModify.Tables.Add(dt)
        Return ResultDataSetToModify
    End Function
#End Region

End Class
