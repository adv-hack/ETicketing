Imports System.Data
Imports Talent.Common
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities
Partial Class UserControls_ProductSubTypesFilter
    Inherits ControlBase

#Region "Class Level Members"
    Private _ucr As UserControlResource
    Private _languageCode As String = String.Empty
    Private _partner As String = String.Empty
    Private _searchType As String = String.Empty
    Private _searchSortBy As String = String.Empty
    Private _searchViewType As String = String.Empty
    Private _searchKeyword As String = String.Empty
    Private _searchDate As String = String.Empty
    Private _searchLocation As String = String.Empty
    Private _searchCategory As Integer = 0
    Private _searchProductType As String = String.Empty
    Private _searchStadium As String = String.Empty
#End Region

#Region "Properties"
    Public Property DtSearchType() As DataTable = Nothing
    Public Property DtSortBy() As DataTable = Nothing
    Public Property DtViewType() As DataTable = Nothing
    Public Property DtLocation() As DataTable = Nothing
    Public Property DtCategories() As DataTable = Nothing
    Public Property DtProductType() As DataTable = Nothing
    Public Property DtStadium() As DataTable = Nothing
#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        InitialiseClassLevelMembers()
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PartnerCode = _partner
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ProductSubTypesFilter.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SetControlDisplayText()
        If Not Page.IsPostBack Then
            If IsSearchTermExists() Then
                plhClearButton.Visible = True
            End If
            plhSaveAndSearchButton.Visible = ModuleDefaults.SavedSearchEnabled
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not IsPostBack Then
            initialiseSearchTypeOptions()
            populateSearchTypes()
            txtKeyword.Text = GetSearchTermValue(Request.QueryString("SearchKeyword"))
            txtDate.Text = GetSearchTermValue(Request.QueryString("SearchDate"))
            populateCategories()
            populateLocation()
            populateProductTypes()
            populateStadiums()
            populateSortBy()
            populateViewType()
        End If
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        Search()
    End Sub

    Protected Sub ddlSearchType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSearchType.SelectedIndexChanged
        _searchType = ddlSearchType.SelectedValue
        ClearFilter()
    End Sub
    Protected Sub ddlViewType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlViewType.SelectedIndexChanged
        Search()
    End Sub
    Protected Sub ddlSortBy_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSortBy.SelectedIndexChanged
        Search()
    End Sub
    Protected Sub btnSaveAndSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveAndSearch.Click
        Dim err As New ErrorObj
        _searchType = ddlSearchType.SelectedValue
        _searchSortBy = ddlSortBy.SelectedValue
        _searchViewType = ddlViewType.SelectedValue
        _searchKeyword = txtKeyword.Text
        _searchDate = txtDate.Text
        _searchLocation = ddlLocation.SelectedValue
        _searchCategory = Talent.eCommerce.Utilities.CheckForDBNull_Int(ddlCategory.SelectedValue)
        _searchProductType = ddlProductType.SelectedValue
        _searchStadium = ddlStadium.SelectedValue

        If IsSearchTermExists() Then
            Dim deAgent As New DEAgent
            Dim settings As DESettings = TEBUtilities.GetSettingsObjectForAgent()
            Dim ta As New TalentAgent
            deAgent.SavedSearchType = _searchType
            deAgent.SavedSearchCategory = _searchCategory
            deAgent.SavedSearchLocation = _searchLocation
            deAgent.SavedSearchStadium = _searchStadium
            deAgent.SavedSearchProductType = _searchProductType
            deAgent.SavedSearchKeyword = _searchKeyword
            deAgent.SavedSearchDate = _searchDate
            deAgent.SavedSearchLimit = ModuleDefaults.SavedSearchEnabled
            deAgent.Source = GlobalConstants.SOURCE
            deAgent.AgentUsername = AgentProfile.Name
            settings.Company = AgentProfile.GetAgentCompany
            settings.Cacheing = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("SavedAgentSearchCaching"))
            settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("SavedAgentSearchCachingTimeInMins"))
            ta.AgentDataEntity = deAgent
            ta.Settings = settings

            err = ta.CreateNewSavedSearch()
            If Not err.HasError Then
                Search()
            End If
        End If
    End Sub

    Protected Sub btnClear_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClear.Click
        ClearFilter()
    End Sub

#End Region

#Region "Private Methods"
    Private Sub InitialiseClassLevelMembers()
        _languageCode = TCUtilities.GetDefaultLanguage
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        _ucr = New UserControlResource
    End Sub

    Private Function GetSearchTermValue(ByVal searchTerm As String) As String
        Dim tempValue As String = String.Empty
        If Not String.IsNullOrWhiteSpace(searchTerm) Then
            tempValue = searchTerm
        End If
        Return tempValue
    End Function

    Private Function IsSearchTermExists() As Boolean
        Dim isExists As Boolean = False
        If ((Not String.IsNullOrWhiteSpace(Request.QueryString("SearchKeyword"))) OrElse (Not String.IsNullOrWhiteSpace(Request.QueryString("SearchDate"))) _
            OrElse (Not String.IsNullOrWhiteSpace(Request.QueryString("SearchLocation"))) OrElse (Not String.IsNullOrWhiteSpace(Request.QueryString("SearchCategoryId"))) _
            OrElse (Not String.IsNullOrWhiteSpace(Request.QueryString("SearchStadium"))) OrElse (Not String.IsNullOrWhiteSpace(Request.QueryString("SearchProductType")))) Then
            isExists = True
        End If
        Return isExists
    End Function

    Private Function GetSearchTermsQueryString() As String
        Dim searchTermsInQuery As String = String.Empty
        searchTermsInQuery = "?SearchType=" & ddlSearchType.SelectedValue
        searchTermsInQuery = searchTermsInQuery & "&SearchSortBy=" & ddlSortBy.SelectedValue
        searchTermsInQuery = searchTermsInQuery & "&SearchViewType=" & ddlViewType.SelectedValue
        searchTermsInQuery = searchTermsInQuery & "&SearchKeyword=" & txtKeyword.Text
        searchTermsInQuery = searchTermsInQuery & "&SearchDate=" & txtDate.Text
        searchTermsInQuery = searchTermsInQuery & "&SearchLocation=" & ddlLocation.SelectedValue
        searchTermsInQuery = searchTermsInQuery & "&SearchCategoryId=" & ddlCategory.SelectedValue
        searchTermsInQuery = searchTermsInQuery & "&SearchStadium=" & ddlStadium.SelectedValue
        searchTermsInQuery = searchTermsInQuery & "&SearchProductType=" & ddlProductType.SelectedValue
        Return searchTermsInQuery
    End Function

    Private Sub SetControlDisplayText()
        lblSearchType.Text = _ucr.Content("SearchTypeLabel", _languageCode, True)
        lblSortBy.Text = _ucr.Content("SortByLabel", _languageCode, True)
        lblViewType.Text = _ucr.Content("ViewTypeLabel", _languageCode, True)
        lblKeyword.Text = _ucr.Content("KeywordLabel", _languageCode, True)
        lblLocation.Text = _ucr.Content("LocationLabel", _languageCode, True)
        lblCategory.Text = _ucr.Content("CategoryLabel", _languageCode, True)
        lblStadium.Text = _ucr.Content("StadiumLabel", _languageCode, True)
        lblProductType.Text = _ucr.Content("ProductTypeLabel", _languageCode, True)
        lblDates.Text = _ucr.Content("DateLabel", _languageCode, True)
        btnSearch.Text = _ucr.Content("SearchButtonText", _languageCode, True)
        btnClear.Text = _ucr.Content("ClearButtonText", _languageCode, True)
        btnSaveAndSearch.Text = _ucr.Content("SaveAndSearchButtonText", _languageCode, True)
        Me.txtDateRegEx.ErrorMessage = _ucr.Content("txtDateRegExErrorText", _languageCode, True)
        Me.txtDateRegEx.ValidationExpression = _ucr.Attribute("txtDateRegEx")
        txtKeyword.Attributes.Add("placeholder", lblKeyword.Text)
        txtDate.Attributes.Add("placeholder", lblDates.Text)
    End Sub

    Private Sub initialiseSearchTypeOptions()
        If GetSearchTermValue(Request.QueryString("SearchType")).Length > 0 Then
            _searchType = GetSearchTermValue(Request.QueryString("SearchType"))
        Else
            _searchType = GlobalConstants.SEARCH_SUBTYPE
        End If
        If GetSearchTermValue(Request.QueryString("SearchViewType")).Length > 0 Then
            _searchViewType = GetSearchTermValue(Request.QueryString("SearchViewType"))
        Else
            _searchViewType = GlobalConstants.VIEW_BY_TILE
        End If
        If GetSearchTermValue(Request.QueryString("SearchSortBy")).Length > 0 Then
            _searchSortBy = GetSearchTermValue(Request.QueryString("SearchSortBy"))
        Else
            _searchSortBy = GlobalConstants.SORT_BY_DESCRIPTION
        End If
    End Sub

    Private Sub populateSearchTypes()
        If DtSearchType Is Nothing OrElse DtSearchType.Rows.Count = 0 Then
            populateSearchTypesTable()
        End If

        If DtSearchType IsNot Nothing AndAlso DtSearchType.Rows.Count > 0 Then
            plhSearchType.Visible = True
            ddlSearchType.DataSource = DtSearchType
            ddlSearchType.DataTextField = "SearchTypeDescription"
            ddlSearchType.DataValueField = "SearchType"
            ddlSearchType.DataBind()
            If Not String.IsNullOrWhiteSpace(_searchType) AndAlso Not ddlSearchType.Items.FindByValue(_searchType) Is Nothing Then
                ddlSearchType.SelectedValue = _searchType
            Else
                If GetSearchTermValue(Request.QueryString("SearchType")).Length > 0 Then
                    ddlSearchType.SelectedValue = GetSearchTermValue(Request.QueryString("SearchType"))
                End If
            End If
        Else
            plhSearchType.Visible = False
        End If
    End Sub
    Private Sub populateSearchTypesTable()
        DtSearchType = New DataTable
        With DtSearchType.Columns
            .Add("SearchType", GetType(String))
            .Add("SearchTypeDescription", GetType(String))
        End With

        addSearchTypeRow(GlobalConstants.SEARCH_SUBTYPE)
        addSearchTypeRow(GlobalConstants.SEARCH_PRODUCT)
    End Sub

    Private Sub addSearchTypeRow(ByVal searchType As String)
        Dim searchDescription As String
        searchDescription = _ucr.Content("SearchType-" & searchType, _languageCode, True)

        Dim dRow As DataRow = DtSearchType.NewRow
        dRow("SearchType") = searchType
        dRow("SearchTypeDescription") = searchDescription
        DtSearchType.Rows.Add(dRow)
    End Sub

    Private Sub populateSortBy()
        If DtSortBy Is Nothing OrElse DtSortBy.Rows.Count = 0 Then
            populateSortByTable()
        End If

        If DtSortBy IsNot Nothing AndAlso DtSortBy.Rows.Count > 0 Then
            plhSortBy.Visible = True
            ddlSortBy.DataSource = DtSortBy
            ddlSortBy.DataTextField = "SortByDescription"
            ddlSortBy.DataValueField = "SortBy"
            ddlSortBy.DataBind()
            If Not String.IsNullOrWhiteSpace(_searchSortBy) AndAlso Not ddlSortBy.Items.FindByValue(_searchSortBy) Is Nothing Then
                ddlSortBy.SelectedValue = _searchSortBy
            Else
                If GetSearchTermValue(Request.QueryString("SortBy")).Length > 0 Then
                    ddlSortBy.SelectedValue = GetSearchTermValue(Request.QueryString("SortBy"))
                End If
            End If
        Else
            plhSortBy.Visible = False
        End If
    End Sub
    Private Sub populateSortByTable()
        DtSortBy = New DataTable
        With DtSortBy.Columns
            .Add("SortBy", GetType(String))
            .Add("SortByDescription", GetType(String))
        End With

        addSortByRow(GlobalConstants.SORT_BY_DESCRIPTION)
        addSortByRow(GlobalConstants.SORT_BY_PRODUCT_DATE)

        ' Extra 'Sort by' fields will work, but leaving simple for now.
        'If DtLocation IsNot Nothing AndAlso DtLocation.Rows.Count > 0 Then
        '    addSortByRow(GlobalConstants.SORT_BY_LOCATION)
        'End If
        'If DtCategories IsNot Nothing AndAlso DtCategories.Rows.Count > 0 Then
        '    addSortByRow(GlobalConstants.SORT_BY_CATEGORY)
        'End If
        'If DtStadium IsNot Nothing AndAlso DtStadium.Rows.Count > 0 Then
        '    addSortByRow(GlobalConstants.SORT_BY_STADIUM)
        'End If
        'If DtProductType IsNot Nothing AndAlso DtProductType.Rows.Count > 0 Then
        '    addSortByRow(GlobalConstants.SORT_BY_PRODUCT_TYPE)
        'End If
    End Sub

    Private Sub addSortByRow(ByVal sortBy As String)
        Dim searchDescription As String
        searchDescription = _ucr.Content("SortBy-" & sortBy, _languageCode, True)

        Dim dRow As DataRow = DtSortBy.NewRow
        dRow("SortBy") = sortBy
        dRow("SortByDescription") = searchDescription
        DtSortBy.Rows.Add(dRow)
    End Sub

    Private Sub populateViewType()
        If DtViewType Is Nothing OrElse DtViewType.Rows.Count = 0 Then
            populateViewTypeTable()
        End If

        If DtViewType IsNot Nothing AndAlso DtViewType.Rows.Count > 0 Then
            plhViewType.Visible = True
            ddlViewType.DataSource = DtViewType
            ddlViewType.DataTextField = "ViewTypeDescription"
            ddlViewType.DataValueField = "ViewType"
            ddlViewType.DataBind()
            If Not String.IsNullOrWhiteSpace(_searchViewType) AndAlso Not ddlViewType.Items.FindByValue(_searchViewType) Is Nothing Then
                ddlViewType.SelectedValue = _searchViewType
            Else
                If GetSearchTermValue(Request.QueryString("ViewType")).Length > 0 Then
                    ddlViewType.SelectedValue = GetSearchTermValue(Request.QueryString("ViewType"))
                End If
            End If
        Else
            plhViewType.Visible = False
        End If
    End Sub
    Private Sub populateViewTypeTable()
        DtViewType = New DataTable
        With DtViewType.Columns
            .Add("ViewType", GetType(String))
            .Add("ViewTypeDescription", GetType(String))
        End With

        addViewTypeRow(GlobalConstants.VIEW_BY_TILE)
        addViewTypeRow(GlobalConstants.VIEW_BY_LIST)
    End Sub

    Private Sub addViewTypeRow(ByVal ViewType As String)
        Dim searchDescription As String
        searchDescription = _ucr.Content("ViewType-" & ViewType, _languageCode, True)

        Dim dRow As DataRow = DtViewType.NewRow
        dRow("ViewType") = ViewType
        dRow("ViewTypeDescription") = searchDescription
        DtViewType.Rows.Add(dRow)
    End Sub

    Private Sub populateCategories()
        If DtCategories Is Nothing OrElse DtCategories.Rows.Count = 0 Then
            populateCategoryTable()
        End If

        If DtCategories IsNot Nothing AndAlso DtCategories.Rows.Count > 0 Then
            plhCategory.Visible = True
            ddlCategory.DataSource = DtCategories
            ddlCategory.DataTextField = "CATEGORY_DESCRIPTION"
            ddlCategory.DataValueField = "CATEGORY_NUMBER"
            ddlCategory.DataBind()
            ddlCategory.Items.Insert(0, New ListItem(_ucr.Content("CategoryPleaseSelectText", _languageCode, True), String.Empty))
            If GetSearchTermValue(Request.QueryString("SearchCategoryId")).Length > 0 Then
                ddlCategory.SelectedValue = GetSearchTermValue(Request.QueryString("SearchCategoryId"))
            End If
        Else
            plhCategory.Visible = False
        End If
    End Sub

    Private Sub populateCategoryTable()
        DtCategories = TDataObjects.ProductsSettings.TblEventCategory.GetAllEventCategoriesByBUAndPartner(_ucr.BusinessUnit, _ucr.PartnerCode)
    End Sub
    Private Sub populateLocation()
        If DtLocation Is Nothing OrElse DtLocation.Rows.Count = 0 Then
            populateLocationTable()
        End If
        If DtLocation IsNot Nothing AndAlso DtLocation.Rows.Count > 0 Then
            plhLocation.Visible = True
            ddlLocation.DataSource = DtLocation
            ddlLocation.DataTextField = "Location"
            ddlLocation.DataValueField = "LocationId"
            ddlLocation.DataBind()
            ddlLocation.Items.Insert(0, New ListItem(_ucr.Content("LocationPleaseSelectText", _languageCode, True), String.Empty))
            If Not String.IsNullOrWhiteSpace(_searchLocation) AndAlso Not ddlLocation.Items.FindByValue(_searchLocation) Is Nothing Then
                ddlLocation.SelectedValue = _searchLocation
            Else
                If GetSearchTermValue(Request.QueryString("SearchLocation")).Length > 0 Then
                    ddlLocation.SelectedValue = GetSearchTermValue(Request.QueryString("SearchLocation"))
                End If
            End If
        Else
            plhLocation.Visible = False
        End If
    End Sub

    Private Sub populateLocationTable()
        Dim talentProduct As New TalentProduct
        Dim settings As New DESettings
        Dim err As New ErrorObj
        Dim dtLocations As New DataTable("Details")
        With dtLocations.Columns
            .Add("LocationId", GetType(String))
            .Add("Location", GetType(String))
        End With

        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.BusinessUnit = TalentCache.GetBusinessUnit()
        settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        settings.Cacheing = True
        talentProduct.Settings() = settings
        talentProduct.Settings.OriginatingSourceCode = "W"

        err = talentProduct.ProductSubTypesList
        If Not err.HasError Then
            DtLocation = talentProduct.ResultDataSet.Tables("LocationDetails")
        End If
    End Sub

    Private Sub populateStadiums()
        If _searchType = GlobalConstants.SEARCH_PRODUCT Then
            If DtStadium Is Nothing OrElse DtStadium.Rows.Count = 0 Then
                populateStadiumsTable()
            End If

            If DtStadium IsNot Nothing AndAlso DtStadium.Rows.Count > 0 Then
                plhStadium.Visible = True
                ddlStadium.DataSource = DtStadium
                ddlStadium.DataTextField = "StadiumText"
                ddlStadium.DataValueField = "StadiumCode"
                ddlStadium.DataBind()
                ddlStadium.Items.Insert(0, New ListItem(_ucr.Content("StadiumPleaseSelectText", _languageCode, True), String.Empty))
                If Not String.IsNullOrWhiteSpace(_searchStadium) AndAlso Not ddlStadium.Items.FindByValue(_searchStadium) Is Nothing Then
                    ddlStadium.SelectedValue = _searchStadium
                Else
                    If GetSearchTermValue(Request.QueryString("SearchStadium")).Length > 0 Then
                        ddlStadium.SelectedValue = GetSearchTermValue(Request.QueryString("SearchStadium"))
                    End If
                End If
            Else
                plhStadium.Visible = False
            End If
        Else
            plhStadium.Visible = False
        End If
    End Sub

    Private Sub populateStadiumsTable()
        Dim feeds As New TalentProduct
        Dim deFeeds As New DEProduct
        Dim Err As ErrorObj
        Dim settings As DESettings

        settings = TEBUtilities.GetSettingsObjectForAgent()
        settings.Company = AgentProfile.GetAgentCompany
        feeds.Dep = deFeeds
        feeds.Settings = settings

        Err = feeds.RetrieveStadiums

        If Not Err.HasError Then
            DtStadium = feeds.ResultDataSet.Tables("Stadiums")
        End If
    End Sub

    Private Sub populateProductTypes()
        If _searchType = GlobalConstants.SEARCH_PRODUCT Then
            If DtProductType Is Nothing OrElse DtProductType.Rows.Count = 0 Then
                populateProductTypesTable()
            End If

            If DtProductType IsNot Nothing AndAlso DtProductType.Rows.Count > 0 Then
                plhProductType.Visible = True
                ddlProductType.DataSource = DtProductType
                ddlProductType.DataTextField = "ProductTypeDescription"
                ddlProductType.DataValueField = "ProductTypeCode"
                ddlProductType.DataBind()
                ddlProductType.Items.Insert(0, New ListItem(_ucr.Content("ProductTypePleaseSelectText", _languageCode, True), String.Empty))
                If Not String.IsNullOrWhiteSpace(_searchProductType) AndAlso Not ddlProductType.Items.FindByValue(_searchProductType) Is Nothing Then
                    ddlProductType.SelectedValue = _searchProductType
                Else
                    If GetSearchTermValue(Request.QueryString("SearchProductType")).Length > 0 Then
                        ddlProductType.SelectedValue = GetSearchTermValue(Request.QueryString("SearchProductType"))
                    End If
                End If
            Else
                plhProductType.Visible = False
            End If
        Else
            plhProductType.Visible = False
        End If
    End Sub
    Private Sub populateProductTypesTable()
        DtProductType = New DataTable
        With DtProductType.Columns
            .Add("ProductTypeCode", GetType(String))
            .Add("ProductTypeDescription", GetType(String))
        End With

        addProductTypeRow(GlobalConstants.HOMEPRODUCTTYPE)
        addProductTypeRow(GlobalConstants.AWAYPRODUCTTYPE)
        addProductTypeRow(GlobalConstants.EVENTPRODUCTTYPE)
        addProductTypeRow(GlobalConstants.TRAVELPRODUCTTYPE)
        addProductTypeRow(GlobalConstants.SEASONTICKETPRODUCTTYPE)
    End Sub

    Private Sub addProductTypeRow(ByVal productType As String)
        Dim productDescription As String
        productDescription = _ucr.Content("ProductTypeDescription-" & productType, _languageCode, True)

        Dim dRow As DataRow = DtProductType.NewRow
        dRow("ProductTypeCode") = productType
        dRow("ProductTypeDescription") = productDescription
        DtProductType.Rows.Add(dRow)
    End Sub

    Private Sub ClearFilter()
        plhClearButton.Visible = False
        txtKeyword.Text = ""
        txtDate.Text = ""
        ddlLocation.SelectedIndex = -1
        ddlProductType.SelectedIndex = -1
        ddlStadium.SelectedIndex = -1
        ddlCategory.SelectedIndex = -1
        _searchKeyword = String.Empty
        _searchDate = String.Empty
        _searchLocation = String.Empty
        _searchCategory = 0
        _searchProductType = String.Empty
        _searchStadium = String.Empty
        Dim Url As New StringBuilder(Request.CurrentExecutionFilePath)
        Url.Append(GetSearchTermsQueryString)
        Response.Redirect(Url.ToString)
    End Sub

    Private Sub Search()
        _searchType = ddlSearchType.SelectedValue
        _searchSortBy = ddlSortBy.SelectedValue
        _searchViewType = ddlViewType.SelectedValue
        _searchKeyword = txtKeyword.Text
        _searchDate = txtDate.Text
        _searchLocation = ddlLocation.SelectedValue
        _searchCategory = Talent.eCommerce.Utilities.CheckForDBNull_Int(ddlCategory.SelectedValue)
        _searchProductType = ddlSearchType.SelectedValue
        _searchStadium = ddlStadium.SelectedValue

        If Not String.IsNullOrWhiteSpace(_searchType) Then
            Session("SearchType") = _searchType
        Else
            Session.Remove("SearchType")
        End If
        If Not String.IsNullOrWhiteSpace(_searchSortBy) Then
            Session("SearchSortBy") = _searchSortBy
        Else
            Session.Remove("SearchSortBy")
        End If
        If Not String.IsNullOrWhiteSpace(_searchViewType) Then
            Session("SearchViewType") = _searchViewType
        Else
            Session.Remove("SearchViewType")
        End If
        If Not String.IsNullOrWhiteSpace(_searchKeyword) Then
            Session("SearchKeyword") = _searchKeyword
        Else
            Session.Remove("SearchKeyword")
        End If
        If Not String.IsNullOrWhiteSpace(txtDate.Text) Then
            Session("SearchDate") = "1" & txtDate.Text
        Else
            Session.Remove("SearchDate")
        End If
        If Not String.IsNullOrWhiteSpace(_searchLocation) Then
            Session("SearchLocation") = ddlLocation.SelectedValue
        Else
            Session.Remove("SearchLocation")
        End If
        If Not String.IsNullOrWhiteSpace(_searchCategory) Then
            Session("SearchCategoryId") = ddlCategory.SelectedValue
        Else
            Session.Remove("SearchCategoryId")
        End If
        If Not String.IsNullOrWhiteSpace(_searchStadium) Then
            Session("SearchStadium") = ddlStadium.SelectedValue
        Else
            Session.Remove("SearchStadium")
        End If
        If Not String.IsNullOrWhiteSpace(_searchProductType) Then
            Session("SearchProductType") = ddlProductType.SelectedValue
        Else
            Session.Remove("SearchProductType")
        End If

        plhClearButton.Visible = True
        Dim Url As New StringBuilder(Request.CurrentExecutionFilePath)
        Url.Append(GetSearchTermsQueryString)
        Response.Redirect(Url.ToString)
    End Sub
#End Region

End Class