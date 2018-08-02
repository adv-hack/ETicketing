Imports System.Data
Imports Talent.Common
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities
Partial Class UserControls_ProductSubTypes
    Inherits ControlBase

#Region "Class Level Members"
    Private _ucr As UserControlResource
    Private _ecomModuleDefaults As ECommerceModuleDefaults
    Private _ecomModuleDefaultsValue As ECommerceModuleDefaults.DefaultValues
    Private _languageCode As String = String.Empty
    Private _errMsg As TalentErrorMessages
    Private _partner As String = String.Empty
    Private _CPage As Label
    Private _CurPage, _CurRec, _i, _totLinks As Integer
    Private _vPerpage, _vTotalRec As Int16
    Private _objPds As PagedDataSource

    Private _dsResultsData As DataSet
    Private _searchType As String = String.Empty
    Private _searchViewType As String = String.Empty
    Private _searchSortBy As String = String.Empty
    Private _searchKeyword As String = String.Empty
    Private _searchDate As String = String.Empty
    Private _searchLocation As String = String.Empty
    Private _searchStadium As String = String.Empty
    Private _searchProductType As String = String.Empty
    Private _searchCategoryId As Integer = 0
#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        InitialiseClassLevelMembers()

        'all are intialised do any other settings
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PartnerCode = _partner
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ProductSubTypes.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        setLabelValues()
        ' Show any errors from the ticketing gateway
        If Not Session("TicketingGatewayError") Is Nothing Then
            showError(Session("TicketingGatewayError"))
            If Session("TalentErrorCode") = Session("TicketingGatewayError") Then
                Session("TalentErrorCode") = Nothing
            End If
            Session("TicketingGatewayError") = Nothing
        End If
        If Not Session("TalentErrorCode") Is Nothing Then
            Dim myError As String = CStr(Session("TalentErrorCode"))
            errorlist.Items.Add(_errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString,
                                Talent.eCommerce.Utilities.GetCurrentPageName,
                                myError).ERROR_MESSAGE)
            Session("TalentErrorCode") = Nothing
        End If

    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        LoadDataAndPager()
        plhErrorList.Visible = (errorlist.Items.Count > 0)
    End Sub

    Private Sub LoadDataAndPager()
        Dim pageNumber As String = String.Empty
        If (Not Page.IsPostBack) AndAlso (Not String.IsNullOrEmpty(Request.QueryString("page"))) Then
            pageNumber = Request.QueryString("page").ToString.Trim
        End If
        Try
            showData(pageNumber)
            setBottomPagerValues()
        Catch ex As Exception
        End Try

        Dim showNoProductsMessage As Boolean = True
        If rptTicketingResults.Items.Count = 0 Then
            showNoProductsMessage = True
        Else
            For Each item As RepeaterItem In rptTicketingResults.Items
                If item.Visible Then showNoProductsMessage = False
            Next
        End If
        If showNoProductsMessage Then
            plhNoProductsFound.Visible = True
            plhTicketingResults.Visible = False
            ltlNoProductsFound.Text = _ucr.Content("NoProductsFoundMessageText", _languageCode, True)
            PagerBottom.Visible = False
            PagerTop.Visible = False
        Else
            plhNoProductsFound.Visible = False
            setBottomPagerValues()
            plhTicketingResults.Visible = True
            If plhTopPager.Visible = False Then
                plhPagerTopWrapper.Visible = False
            End If
        End If

        If errorlist.Items.Count > 0 Then
            plhErrorList.Visible = True
        Else
            plhErrorList.Visible = False
        End If
    End Sub

    Protected Sub rptTicketingResults_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptTicketingResults.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
            Dim hlkTicketingResult As HyperLink = CType(e.Item.FindControl("hlkTicketingResult"), HyperLink)
            Dim imgSubType As Image = CType(e.Item.FindControl("imgSubType"), Image)
            Dim imgOpposition As Image = CType(e.Item.FindControl("imgOpposition"), Image)
            Dim imgSponsor As Image = CType(e.Item.FindControl("imgSponsor"), Image)
            Dim imgCompetition As Image = CType(e.Item.FindControl("imgCompetition"), Image)
            Dim ltlDescription As Literal = CType(e.Item.FindControl("ltlDescription"), Literal)
            Dim ltlStartDateLabel As Literal = CType(e.Item.FindControl("ltlStartDateLabel"), Literal)
            Dim ltlStartDateValue As Literal = CType(e.Item.FindControl("ltlStartDateValue"), Literal)
            Dim ltlEndDateLabel As Literal = CType(e.Item.FindControl("ltlEndDateLabel"), Literal)
            Dim ltlEndDateValue As Literal = CType(e.Item.FindControl("ltlEndDateValue"), Literal)
            Dim plhEndDate As PlaceHolder = CType(e.Item.FindControl("plhEndDate"), PlaceHolder)
            Dim plhStartDate As PlaceHolder = CType(e.Item.FindControl("plhStartDate"), PlaceHolder)
            Dim divResultItem As HtmlGenericControl = CType(e.Item.FindControl("divResultItem"), HtmlGenericControl)
            Dim resultRow As DataRow = CType(e.Item.DataItem, DataRowView).Row

            ltlStartDateLabel.Text = _ucr.Content("ltlStartDateLabel", _languageCode, True)
            Dim boolStartDateVisible = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("showStartDate"))

            plhEndDate.Visible = False
            imgSubType.Visible = False
            imgOpposition.Visible = False
            imgSponsor.Visible = False
            imgCompetition.Visible = False

            If _searchType = GlobalConstants.SEARCH_PRODUCT Then
                hlkTicketingResult.NavigateUrl = GetProductTypeLink(resultRow)
                ltlDescription.Text = TEBUtilities.CheckForDBNull_String(resultRow("ProductDescription"))
                ltlStartDateValue.Text = CType(resultRow("ProductDateYear"), Date).ToShortDateString
                imgOpposition.ImageUrl = ProductDetail.GetImageURL("PRODTICKETING", resultRow("ProductOppositionCode"))
                imgSponsor.ImageUrl = ProductDetail.GetImageURL("PRODSPONSOR", resultRow("ProductSponsorCode"))
                imgCompetition.ImageUrl = ProductDetail.GetImageURL("PRODCOMPETITION", resultRow("ProductCompetitionCode"))
                imgOpposition.Visible = True
                imgSponsor.Visible = True
                imgCompetition.Visible = True
                boolStartDateVisible = (boolStartDateVisible AndAlso Not TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(resultRow("HideDate")))
            Else
                hlkTicketingResult.NavigateUrl = GetProductSubTypeLink(resultRow)
                ltlDescription.Text = TEBUtilities.CheckForDBNull_String(resultRow("SubTypeDescription"))
                ltlStartDateValue.Text = CType(resultRow("StartDate"), Date).ToShortDateString
                ltlEndDateValue.Text = CType(resultRow("EndDate"), Date).ToShortDateString
                ltlEndDateLabel.Text = _ucr.Content("ltlEndDateLabel", _languageCode, True)
                Dim boolEndDateVisible = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("showEndDate"))
                plhEndDate.Visible = boolEndDateVisible

                Dim tempImgPath As String = String.Empty
                If TryGetImageURL(TEBUtilities.CheckForDBNull_String(resultRow("ProductType")), TEBUtilities.CheckForDBNull_String(resultRow("ProductSubType")), tempImgPath) Then
                    imgSubType.ImageUrl = tempImgPath
                    imgSubType.Visible = True
                End If
            End If

            plhStartDate.Visible = boolStartDateVisible

            Dim cssClass As String = String.Empty
            cssClass += "column ebiz-product-sub-type-item"

            If _searchViewType = GlobalConstants.VIEW_BY_TILE Then
                cssClass += " ebiz-view-tile"
            Else
                cssClass += " ebiz-view-list"
            End If

            If _searchType = GlobalConstants.SEARCH_SUBTYPE Then
                cssClass += " ebiz-result-sub-type"
                cssClass += " ebiz-sub-type-" & resultRow("ProductSubType")
            Else
                cssClass += " ebiz-result-product"
                cssClass += " ebiz-product-mttp-" & resultRow("ProductType")
                If resultRow("ProductSubType") = String.Empty Then
                    cssClass += " ebiz-product-sub-type-" & resultRow("ProductSubType")
                End If
            End If
            divResultItem.Attributes("class") = cssClass
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub InitialiseClassLevelMembers()
        _languageCode = TCUtilities.GetDefaultLanguage
        _ecomModuleDefaults = New ECommerceModuleDefaults
        _ecomModuleDefaultsValue = _ecomModuleDefaults.GetDefaults
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        _CPage = New Label
        _objPds = New PagedDataSource()
        _ucr = New UserControlResource
        _errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup,
                                              _partner, ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
    End Sub

    Private Sub setLabelValues()
        'All other labels 
        displayingLabelT.Text = _ucr.Content("displayingLabelT", _languageCode, True)
        displayingLabelB.Text = _ucr.Content("displayingLabelB", _languageCode, True)
        toLabelT.Text = _ucr.Content("toLabelT", _languageCode, True)
        toLabelB.Text = _ucr.Content("toLabelB", _languageCode, True)
        ofLabelT.Text = _ucr.Content("ofLabelT", _languageCode, True)
        ofLabelB.Text = _ucr.Content("ofLabelB", _languageCode, True)
        'set links text
        LnkFirstT.Text = _ucr.Content("LnkFirstT", _languageCode, True)
        LnkPrevT.Text = _ucr.Content("LnkPrevT", _languageCode, True)
        LnkNextT.Text = _ucr.Content("LnkNextT", _languageCode, True)
        LnkLastT.Text = _ucr.Content("LnkLastT", _languageCode, True)
        LnkFirstB.Text = _ucr.Content("LnkFirstB", _languageCode, True)
        LnkPrevB.Text = _ucr.Content("LnkPrevB", _languageCode, True)
        LnkNextB.Text = _ucr.Content("LnkNextB", _languageCode, True)
        LnkLastB.Text = _ucr.Content("LnkLastB", _languageCode, True)
    End Sub
    Private Sub showData(ByVal pCurrentPage As String)
        Try
            Dim settings As New DESettings
            Dim err As New ErrorObj
            Dim VnumberOfLinks As String
            Dim LinksToDisplay As Integer = 0

            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.BusinessUnit = TalentCache.GetBusinessUnit()
            settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            settings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
            settings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
            settings.CacheDependencyPath = _ecomModuleDefaultsValue.CacheDependencyPath
            settings.AuthorityUserProfile = _ecomModuleDefaultsValue.AuthorityUserProfile
            settings.OriginatingSource = TEBUtilities.GetOriginatingSource(Session.Item("Agent"))
            settings.OriginatingSourceCode = GlobalConstants.SOURCE

            AssignSearchTerms()

            If _searchType = GlobalConstants.SEARCH_PRODUCT Then
                err = RetrieveProductData(settings)
            Else
                err = RetrieveSubTypesData(settings)
            End If

            If Not err.HasError Then
                plhTicketingResults.Visible = True
                rptTicketingResults.Visible = True

                ' Do not display paging when in kiosk mode
                _objPds.PageSize = _ucr.Attribute("numberOfProductsVariable-" & _searchViewType)
                If _ecomModuleDefaultsValue.TicketingKioskMode Then
                    PagerTop.Visible = False
                    PagerBottom.Visible = False
                    _objPds.AllowPaging = False
                Else
                    PagerTop.Visible = True
                    PagerBottom.Visible = True
                    _objPds.AllowPaging = True
                End If

                If pCurrentPage <> "" Then
                    _CurPage = pCurrentPage
                Else
                    _CurPage = 1
                End If

                _objPds.CurrentPageIndex = _CurPage - 1
                _CPage.Text = _CurPage.ToString()
                _vPerpage = _objPds.PageSize
                _CurRec = _CurPage * _vPerpage
                displayingValueLabelT.Text = _CurRec - _vPerpage + 1

                _vTotalRec = _dsResultsData.Tables(1).DefaultView.Count
                If _vTotalRec = 0 Then
                    displayingValueLabelT.Text = 0
                End If
                ofValueLabelT.Text = _vTotalRec.ToString()
                If _CurRec > _vTotalRec Then
                    toValueLabelT.Text = _vTotalRec
                Else
                    toValueLabelT.Text = _CurRec
                End If

                Dim linktxt = New String("")

                'add link buttons
                _totLinks = Fix(_vTotalRec / _objPds.PageSize)

                If (_vTotalRec Mod _objPds.PageSize) <> 0 Then
                    _totLinks = _totLinks + 1
                End If
                VnumberOfLinks = _ucr.Attribute("numberOfLinks")
                If _totLinks > VnumberOfLinks Then
                    LinksToDisplay = VnumberOfLinks
                Else
                    LinksToDisplay = _totLinks
                End If


                If _totLinks > 1 Then
                    Dim Url As New StringBuilder(Request.CurrentExecutionFilePath)
                    Url.Append("?Page=")

                    If Not _objPds.IsFirstPage Then LnkPrevT.NavigateUrl = Url.ToString() + Convert.ToString(_CurPage - 1) + GetSearchTermsQueryString()

                    If Not _objPds.IsLastPage Then LnkNextT.NavigateUrl = Url.ToString() + Convert.ToString(_CurPage + 1) + GetSearchTermsQueryString()

                    For i As Integer = 1 To LinksToDisplay
                        If i <> _CurPage Then
                            linktxt = linktxt + "<li><a href='" + Url.ToString() + i.ToString() + GetSearchTermsQueryString() + "'>" + i.ToString() + "</a></li>"
                        Else
                            linktxt = linktxt + "<li class=""current""><a href='#'>" + i.ToString() + "</a></li>"
                        End If
                    Next
                    plhPagerTopWrapper.Visible = True
                    plhBottomPager.Visible = True
                    plhTopPager.Visible = True
                Else
                    LnkPrevT.Visible = False
                    LnkNextT.Visible = False
                    plhBottomPager.Visible = False
                    plhTopPager.Visible = False
                    If _totLinks <> 0 Then
                        linktxt = " 1 "
                    Else
                        linktxt = " 0 "
                    End If
                End If
                LinksLabelT.Text = linktxt
                rptTicketingResults.DataSource = _objPds
                rptTicketingResults.DataBind()
            Else
                If _dsResultsData.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows(0).Item("ErrorOccurred").ToString = GlobalConstants.ERRORFLAG Then
                    showError(_dsResultsData.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows(0).Item("ReturnCode").ToString())
                End If
            End If

            If Not plhTopPager.Visible Then
                plhPagerTopWrapper.Visible = False
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Function RetrieveProductData(ByVal settings As DESettings) As ErrorObj
        Dim feeds As New TalentFeeds
        Dim deFeeds As New DEFeeds

        Dim Err As ErrorObj
        deFeeds.Product_Type = "ALL"
        deFeeds.Online_Products_Only = True
        feeds.FeedsEntity = deFeeds
        feeds.Settings = settings

        Err = feeds.ProductFeeds
        _dsResultsData = feeds.ResultDataSet

        If Not Err.HasError Then
            _dsResultsData.Tables("FeedsProduct").DefaultView.Sort = GetRowSortByConditionProducts()
            _dsResultsData.Tables("FeedsProduct").DefaultView.RowFilter = GetRowFilterCondition()
            _objPds.DataSource = _dsResultsData.Tables("FeedsProduct").DefaultView

            ' We need to override the filter location table for product searches as we may have products
            ' with locations which do not sit in subtypes.
            overrideFilterLocationTable()

        End If
        Return Err
    End Function

    Private Sub overrideFilterLocationTable()
        Dim dv As New DataView(_dsResultsData.Tables("FeedsProduct"))
        dv.RowFilter = "Location <> ''"
        Dim dt As DataTable = dv.ToTable(True, "LocationId", "Location")
        ProductSubTypesFilter.DtLocation = dt
    End Sub

    Private Function RetrieveSubTypesData(ByVal settings As DESettings) As ErrorObj
        Dim Err As ErrorObj
        Dim product As New TalentProduct
        product.Settings() = settings

        If _searchDate.Length > 0 Then
            product.De.ProductDate = _searchDate
        End If

        Err = product.ProductSubTypesList
        _dsResultsData = product.ResultDataSet

        If Not Err.HasError Then
            _dsResultsData.Tables("ProductSubTypes").DefaultView.Sort = GetRowSortByConditionSubTypes()
            _dsResultsData.Tables("ProductSubTypes").DefaultView.RowFilter = GetRowFilterCondition()
            _objPds.DataSource = _dsResultsData.Tables("ProductSubTypes").DefaultView
        End If

        Return Err
    End Function

    Private Sub setBottomPagerValues()

        displayingValueLabelB.Text = displayingValueLabelT.Text
        toValueLabelB.Text = toValueLabelT.Text
        ofValueLabelB.Text = ofValueLabelT.Text
        LnkPrevB.NavigateUrl = LnkPrevT.NavigateUrl
        LnkNextB.NavigateUrl = LnkNextT.NavigateUrl
        LinksLabelB.Text = LinksLabelT.Text

        Dim Url As New StringBuilder(Request.CurrentExecutionFilePath)
        Url.Append("?Page=")

        If _CurPage = 1 Or _CurPage = 0 Then
            LnkFirstT.Visible = False
            LnkFirstB.Visible = False
            LnkPrevT.Visible = False
            LnkPrevB.Visible = False
        Else
            LnkFirstT.Visible = True
            LnkFirstT.NavigateUrl = Url.ToString() + "1" + GetSearchTermsQueryString()
            LnkFirstB.Visible = True
            LnkFirstB.NavigateUrl = Url.ToString() + "1" + GetSearchTermsQueryString()
        End If
        If _CurPage = _totLinks Or _totLinks = 0 Then
            LnkLastT.Visible = False
            LnkLastB.Visible = False
            LnkNextT.Visible = False
            LnkNextB.Visible = False
        Else
            LnkLastT.Visible = True
            LnkLastT.NavigateUrl = Url.ToString() + _totLinks.ToString() + GetSearchTermsQueryString()
            LnkLastB.Visible = True
            LnkLastB.NavigateUrl = Url.ToString() + _totLinks.ToString() + GetSearchTermsQueryString()
        End If

    End Sub

    Private Sub showError(ByVal errCode As String)
        errorlist.Items.Clear()
        Dim errorMessage = _errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, errCode).ERROR_MESSAGE()
        errorlist.Items.Add(errorMessage)
    End Sub

    Private Function GetProductSubTypeLink(ByVal productSubTypeRow As DataRow) As String
        Dim productType As String = productSubTypeRow("ProductType")
        Dim productSubType As String = productSubTypeRow("ProductSubType")
        Dim linkURL As String = String.Empty
        Select Case productType
            Case GlobalConstants.HOMEPRODUCTTYPE
                linkURL = "~/PagesPublic/ProductBrowse/productHome.aspx?ProductSubType=" & productSubType
            Case GlobalConstants.AWAYPRODUCTTYPE
                linkURL = "~/PagesPublic/ProductBrowse/productAway.aspx?ProductSubType=" & productSubType
            Case GlobalConstants.TRAVELPRODUCTTYPE
                linkURL = "~/PagesPublic/ProductBrowse/ProductTravel.aspx?ProductSubType=" & productSubType
            Case GlobalConstants.EVENTPRODUCTTYPE
                linkURL = "~/PagesPublic/ProductBrowse/ProductEvent.aspx?ProductSubType=" & productSubType
            Case "CH"
                linkURL = "~/PagesPublic/ProductBrowse/MatchDayHospitality.aspx?ProductSubType=" & productSubType
            Case GlobalConstants.SEASONTICKETPRODUCTTYPE
                linkURL = "~/PagesPublic/ProductBrowse/ProductSeason.aspx?ProductSubType=" & productSubType
            Case GlobalConstants.MEMBERSHIPPRODUCTTYPE
                linkURL = "~/PagesPublic/ProductBrowse/ProductMembership.aspx?ProductSubType=" & productSubType
            Case Else
                linkURL = String.Empty
        End Select
        Return linkURL
    End Function

    Private Function GetProductTypeLink(ByVal productRow As DataRow) As String
        Dim productType As String = productRow("ProductType")
        Dim queryString As String = String.Empty
        queryString += "?"
        If productRow("ProductSubType").trim <> String.Empty Then
            queryString += "ProductSubType=" & productRow("ProductSubType") & "&"
        End If
        queryString += "IsSingleProduct=TRUE" & "&ProductCode=" & productRow("ProductCode") & "&ProductType=" & productRow("ProductType")

        Dim linkURL As String = String.Empty
        Select Case productType
            Case GlobalConstants.HOMEPRODUCTTYPE, GlobalConstants.SEASONTICKETPRODUCTTYPE
                linkURL = Talent.eCommerce.Utilities.GetFormattedSeatSelectionUrl(String.Empty, productRow("ProductStadium"), productRow("ProductCode"),
                                                                                            productRow("CampaignCode"), productRow("ProductType"), productRow("ProductSubType"),
                                                                                            productRow("ProductHomeAsAway"), productRow("RestrictGraphical"))
            Case GlobalConstants.AWAYPRODUCTTYPE
                linkURL = "~/PagesPublic/ProductBrowse/productAway.aspx" & queryString
            Case GlobalConstants.TRAVELPRODUCTTYPE
                linkURL = "~/PagesPublic/ProductBrowse/ProductTravel.aspx" & queryString
            Case GlobalConstants.EVENTPRODUCTTYPE
                linkURL = "~/PagesPublic/ProductBrowse/ProductEvent.aspx" & queryString
            Case "CH"
                linkURL = "~/PagesPublic/ProductBrowse/MatchDayHospitality.aspx" & queryString
            Case GlobalConstants.MEMBERSHIPPRODUCTTYPE
                linkURL = "~/PagesPublic/ProductBrowse/ProductMembership.aspx" & queryString
            Case Else
                linkURL = String.Empty
        End Select
        Return linkURL
    End Function

    Private Sub AssignSearchTerms()
        
        _searchType = GetSearchTermValue(Request.QueryString("SearchType"))
        _searchSortBy = GetSearchTermValue(Request.QueryString("SearchSortBy"))
        _searchViewType = GetSearchTermValue(Request.QueryString("SearchViewType"))
        _searchKeyword = GetSearchTermValue(Request.QueryString("SearchKeyword"))
        _searchDate = GetSearchTermValue(Request.QueryString("SearchDate"))
        _searchLocation = GetSearchTermValue(Request.QueryString("SearchLocation"))
        _searchStadium = GetSearchTermValue(Request.QueryString("SearchStadium"))
        _searchProductType = GetSearchTermValue(Request.QueryString("SearchProductType"))
        _searchCategoryId = 0
        If Not String.IsNullOrEmpty(Request.QueryString("SearchCategoryId")) Then
            Try
                _searchCategoryId = CType(Request.QueryString("SearchCategoryId"), Integer)
            Catch ex As Exception
            End Try
        End If

        'Default Values
        If _searchType = String.Empty Then
            _searchType = GlobalConstants.SEARCH_SUBTYPE
        End If
        If _searchSortBy = String.Empty Then
            _searchSortBy = GlobalConstants.SORT_BY_DESCRIPTION
        End If
        If _searchViewType = String.Empty Then
            _searchViewType = GlobalConstants.VIEW_BY_TILE
        End If
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
        If ((Not String.IsNullOrWhiteSpace(_searchType)) OrElse (Not String.IsNullOrWhiteSpace(_searchKeyword)) OrElse (Not String.IsNullOrWhiteSpace(_searchDate)) OrElse (Not String.IsNullOrWhiteSpace(_searchLocation)) OrElse (Not String.IsNullOrWhiteSpace(_searchStadium)) OrElse (Not String.IsNullOrWhiteSpace(_searchProductType))) Then
            isExists = True
        ElseIf _searchCategoryId > 0 Then
            isExists = True
        End If
        Return isExists
    End Function

    Private Function GetSearchTermsQueryString() As String
        Dim searchTermsInQuery As String = String.Empty
        If IsSearchTermExists() Then
            If Not String.IsNullOrWhiteSpace(_searchType) Then searchTermsInQuery += "&SearchType=" & _searchType
            If Not String.IsNullOrWhiteSpace(_searchSortBy) Then searchTermsInQuery += "&SearchSortBy=" & _searchSortBy
            If Not String.IsNullOrWhiteSpace(_searchViewType) Then searchTermsInQuery += "&SearchViewType=" & _searchViewType
            If Not String.IsNullOrWhiteSpace(_searchKeyword) Then searchTermsInQuery += "&SearchKeyword=" & _searchKeyword
            If Not String.IsNullOrWhiteSpace(_searchDate) Then searchTermsInQuery += "&SearchDate=" & _searchDate
            If Not String.IsNullOrWhiteSpace(_searchLocation) Then searchTermsInQuery = "&SearchLocation=" & _searchLocation
            If _searchCategoryId > 0 Then searchTermsInQuery += "&SearchCategoryId=" & _searchCategoryId.ToString
            If Not String.IsNullOrWhiteSpace(_searchStadium) Then searchTermsInQuery = "&SearchStadium=" & _searchStadium
            If Not String.IsNullOrWhiteSpace(_searchProductType) Then searchTermsInQuery = "&SearchProductType=" & _searchProductType
        End If
        Return searchTermsInQuery
    End Function

    Private Function GetRowFilterCondition() As String
        Dim filterCondition As String = String.Empty
        If IsSearchTermExists() Then

            Dim keywordCondition As New String("")
            Dim locationIdCondition As New String("")
            If _searchType = GlobalConstants.SEARCH_PRODUCT Then
                keywordCondition = "(ProductCode Like '%" & _searchKeyword & "%' OR ProductDescription Like '%" & _searchKeyword & "%')"
                locationIdCondition = "(LocationId <> '' AND LocationId = '" & _searchLocation & "')"
            End If
            If _searchType = GlobalConstants.SEARCH_SUBTYPE Then
                keywordCondition = "SubTypeDescription Like '%" & _searchKeyword & "%'"
                locationIdCondition = "("
                locationIdCondition = locationIdCondition & "(LocationIds Like '" & _searchLocation & ",%')"
                locationIdCondition = locationIdCondition & " OR (LocationIds Like '%," & _searchLocation & ",%')"
                locationIdCondition = locationIdCondition & " OR (LocationIds Like '%," & _searchLocation & "')"
                locationIdCondition = locationIdCondition & ")"
            End If

            If _searchKeyword.Length > 0 Then
                filterCondition = keywordCondition
            End If
            If _searchStadium.Length > 0 Then
                If filterCondition.Length > 0 Then
                    filterCondition = filterCondition & " AND "
                End If
                filterCondition = filterCondition & "(ProductStadium = '" & _searchStadium & "')"
            End If
            If _searchProductType.Length > 0 Then
                If filterCondition.Length > 0 Then
                    filterCondition = filterCondition & " AND "
                End If
                filterCondition = filterCondition & "(ProductType = '" & _searchProductType & "')"
            End If
            If _searchDate.Length > 0 Then
                If filterCondition.Length > 0 Then
                    filterCondition = filterCondition & " AND "
                End If
                Dim iSeriesDate = TCUtilities.DateToIseriesFormat(_searchDate)
                filterCondition = filterCondition & "(ProductMDTE08 = '" & iSeriesDate & "')"
            End If
            If _searchLocation.Length > 0 Then
                If filterCondition.Length > 0 Then
                    filterCondition = filterCondition & " AND "
                End If
                filterCondition = filterCondition & locationIdCondition
            End If
            If _searchCategoryId > 0 Then
                If filterCondition.Length > 0 Then
                    filterCondition = filterCondition & " AND "
                End If
                filterCondition = filterCondition & "(CategoryId = " & _searchCategoryId.ToString & ")"
            End If
        Else
            filterCondition = ""
        End If
        Return filterCondition
    End Function

    Private Function GetRowSortByConditionProducts() As String
        Dim sortByCondition As String = String.Empty

        Select Case _searchSortBy
            Case GlobalConstants.SORT_BY_DESCRIPTION
                sortByCondition = "ProductDescription"
            Case GlobalConstants.SORT_BY_PRODUCT_DATE
                sortByCondition = "ProductMDTE08"
            Case GlobalConstants.SORT_BY_CATEGORY
                sortByCondition = "CategoryId"
            Case GlobalConstants.SORT_BY_LOCATION
                sortByCondition = "LocationIds"
            Case GlobalConstants.SORT_BY_PRODUCT_TYPE
                sortByCondition = "ProductType"
            Case GlobalConstants.SORT_BY_STADIUM
                sortByCondition = "ProductStadium"
            Case Else
                sortByCondition = "ProductDescription"
        End Select

        Return sortByCondition
    End Function

    Private Function GetRowSortByConditionSubTypes() As String
        Dim sortByCondition As String = String.Empty

        Select Case _searchSortBy
            Case GlobalConstants.SORT_BY_DESCRIPTION
                sortByCondition = "SubTypeDescription"
            Case GlobalConstants.SORT_BY_PRODUCT_DATE
                sortByCondition = "ProductMDTE08"
            Case GlobalConstants.SORT_BY_CATEGORY
                sortByCondition = "CategoryId"
            Case GlobalConstants.SORT_BY_LOCATION
                sortByCondition = "LocationIds"
            Case Else
                sortByCondition = "SubTypeDescription"
        End Select

        Return sortByCondition
    End Function


    Protected Function TryGetImageURL(ByVal productType As String, ByVal productSubType As String, ByRef imgPath As String) As Boolean
        Dim isImgExists As Boolean = False
        imgPath = ImagePath.getImagePath("PRODSUBTYPE", productType & productSubType, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
        If imgPath = _ecomModuleDefaultsValue.MissingImagePath Then
            imgPath = String.Empty
        Else
            isImgExists = True
        End If
        Return isImgExists
    End Function

#End Region

End Class
