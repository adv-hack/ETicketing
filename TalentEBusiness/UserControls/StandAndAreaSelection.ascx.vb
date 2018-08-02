Imports System.Data
Imports System.Data.SqlClient
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.ModelBuilders.Products
Imports Talent.Common
Imports Talent.eCommerce
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Collections.Generic
Imports TalentBusinessLogic.DataTransferObjects

Partial Class UserControls_StandAndAreaSelection
    Inherits ControlBase

#Region "Class Level Fields"

    Private _errMsg As Talent.Common.TalentErrorMessages
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _cProductTable As Integer = 1
    Private _settings As New DESettings
    Private _err As New ErrorObj
    Private _depd As New Talent.Common.DEProductDetails
    Private _dsProductDetails As New DataSet
    Private _ucr As New Talent.Common.UserControlResource
    Private _dAgentReservationCodes As Dictionary(Of String, String)
    Private _productIsEnabledForTicketExchange As Boolean = False
    Private _product As TalentProduct = Nothing
    Private _selectedPriceBreakId As Long = 0
    Private _viewModel As New ProductAvailabilityViewModel
    Private _priceBands As New List(Of PriceBandPrices)
    Private _onLinkedProductsPage As Boolean = False
    Private _onVisualSeatSelectionPage As Boolean = False

#End Region

#Region "Properties"

    Public Property ValueID() As String
    Public Property ProductDescription() As String
    Public Property HideBuyingOptions() As Boolean
    Public Property HideSelectSeatsButton() As Boolean
    Public Property SoldOut() As Boolean
    Public Property TicketExchangeSearchButtonText() As String
    Public Property ProductCode() As String
        Get
            Return hdfProductCode.Value
        End Get
        Set(value As String)
            hdfProductCode.Value = value
        End Set
    End Property
    Public Property ProductType() As String
        Get
            Return hdfProductType.Value
        End Get
        Set(value As String)
            hdfProductType.Value = value
        End Set
    End Property
    Public Property ProductPriceBand() As String
        Get
            Return hdfProductPriceBand.Value
        End Get
        Set(value As String)
            hdfProductPriceBand.Value = value
        End Set
    End Property
    Public Property CampaignCode() As String
        Get
            Return hdfCampaignCode.Value
        End Get
        Set(value As String)
            hdfCampaignCode.Value = value
        End Set
    End Property
    Public Property ProductHomeAsAway() As String
        Get
            Return hdfProductHomeAsAway.Value
        End Get
        Set(value As String)
            hdfProductHomeAsAway.Value = value
        End Set
    End Property
    Public Property ProductSubType() As String
        Get
            Return hdfProductSubType.Value
        End Get
        Set(value As String)
            hdfProductSubType.Value = value
        End Set
    End Property
    Public Property ProductStadium() As String
        Get
            Return hdfProductStadium.Value
        End Get
        Set(value As String)
            hdfProductStadium.Value = value
        End Set
    End Property
    Public Property AlternativeSeatSelection() As Boolean
        Get
            Return TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(hdfAlternativeSeatSelection.Value)
        End Get
        Set(ByVal value As Boolean)
            hdfAlternativeSeatSelection.Value = value
        End Set
    End Property
    Public Property AlternativeSeatSelectionAcrossStands() As Boolean
        Get
            Return TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(hdfAlternativeSeatSelectionAcrossStands.Value)
        End Get
        Set(ByVal value As Boolean)
            hdfAlternativeSeatSelectionAcrossStands.Value = value
        End Set
    End Property
    Public Property StandDDLPleaseSelectText() As String
    Public Property AreaDDLFirstOption() As String
    Public Property IsPriceAndAreaSelection() As Boolean
    Public Property PriceBreakId() As Long
    Public Property TicketExchangeOptionText() As String
    Public Property TicketExchangeSliderText() As String
    Public Property IsCatMode() As Boolean
    Public Property IncludeTicketExchangeChecked() As Boolean
    Public Property CallId() As String
    Public ReadOnly Property SelectedPriceMinimum() As Decimal
        Get
            Return TEBUtilities.CheckForDBNull_Decimal(ddlMinimumPrice.SelectedValue)
        End Get
    End Property
    Public ReadOnly Property SelectedPriceMaximum() As Decimal
        Get
            Return TEBUtilities.CheckForDBNull_Decimal(ddlMaximumPrice.SelectedValue)
        End Get
    End Property
    Public Property ResetButtonText() As String
    Public Property PickingExceptionSeat() As Boolean

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "StandAndAreaSelection.ascx"
        End With
        If Request.QueryString("callid") IsNot Nothing Then CallId = Request.QueryString("callid")
        IsPriceAndAreaSelection = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(ModuleDefaults.PriceAndAreaSelection)
        _errMsg = New Talent.Common.TalentErrorMessages(_languageCode, _ucr.BusinessUnit, TalentCache.GetPartner(Profile), _ucr.FrontEndConnectionString)
        _settings = TEBUtilities.GetSettingsObject()
        _settings.Cacheing = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("Cacheing"))
        _settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("CacheTimeMinutes"))
        _settings.CacheTimeMinutesSecondFunction = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("StandDescriptionsCacheTimeMinutes"))
        StandDDLPleaseSelectText = _ucr.Content("standDDLPleaseSelectLabel", _languageCode, True)
        _onLinkedProductsPage = (TEBUtilities.GetCurrentPageName().ToUpper() = "LINKEDPRODUCTS.ASPX")
        _onVisualSeatSelectionPage = (TEBUtilities.GetCurrentPageName().ToUpper() = "VISUALSEATSELECTION.ASPX")
        hdfShowPricingOptionsAsDropDown.Value = "false"
        hdfShowStandAreaOptionsAsDropDown.Value = "false"
        hdfShowPriceBandListAsDropDown.Value = "false"
        hdfShowTicketExchangeAsDropDown.Value = "false"
    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Me.Visible Then
            SoldOut = False
            ltlError.Text = String.Empty
            getProductDetails()
            populateViewModel()
            setDisplayType()
            setupTicketExchange()
            setupProductPriceBreakDefinitions()
            setupMinimumAndMaximumDropDownList()
            setupPriceBandList()
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If SoldOut Then
            ltlError.Visible = True
            ltlError.Text = _ucr.Content("SoldOutCaption", _languageCode, True)
            pnlStandArea.Visible = False
            plhSoldOut.Visible = True
        Else
            If TEBUtilities.GetCurrentPageName().ToUpper().Contains("VISUALSEATSELECTION.ASPX") Then
                plhSelectSeatsButton.Visible = False
            ElseIf TEBUtilities.GetCurrentPageName().ToUpper().Contains("STANDANDAREASELECTION.ASPX") Then
                plhSelectSeatsButton.Visible = Not HideBuyingOptions
                If plhSelectSeatsButton.Visible Then
                    btnSelectSeats.Text = _ucr.Content("SelectSeatsButtonText", _languageCode, True)
                End If
            Else
                plhBuyingOptions.Visible = False
                plhSelectSeatsButton.Visible = False
            End If
            areaDropDownList.Attributes.Add("onchange", GetJavascriptStringAreaChange())
        End If
    End Sub

    Protected Sub AddTicketingItems(ByVal sender As Object, ByVal e As EventArgs)
        Dim selectedPriceBreakId As String = String.Empty
        Dim selectedMinimumPrice As String = String.Empty
        Dim selectedMaximumPrice As String = String.Empty
        Dim standSelected As String = String.Empty
        Dim areaSelected As String = String.Empty
        Dim priceBands(25, 1) As String
        Dim count As Integer = 0
        Dim quantity As String = String.Empty
        Dim defaultPriceBand As String = String.Empty

        If plhPriceBandList.Visible Then
            For Each pricebandItem As RepeaterItem In rptPriceBand.Items
                Dim txtPriceBandQuantity As TextBox = CType(pricebandItem.FindControl("txtPriceBandQuantity"), TextBox)
                Dim hdfPriceBand As HiddenField = CType(pricebandItem.FindControl("hdfPriceBand"), HiddenField)
                If Not String.IsNullOrEmpty(txtPriceBandQuantity.Text) Then
                    If count < 25 Then
                        priceBands(count, 0) = hdfPriceBand.Value
                        priceBands(count, 1) = txtPriceBandQuantity.Text
                    Else
                        count = -1
                        Exit For
                    End If
                    count += 1
                End If
            Next
            Session("SelectedPriceBands") = priceBands
            quantity = "0"
        End If
        If plhDefaultPriceBand.Visible Then
            quantity = txtQuantity.Text
            defaultPriceBand = hdfProductPriceBand.Value
            count = 1

            ' Should we default price band from the customer or the product default.
            Dim profile As ProfileCommon = HttpContext.Current.Profile
            If Not profile.IsAnonymous AndAlso profile.User.Details IsNot Nothing Then
                Dim productPriceBandForBasketMode = TEBUtilities.CheckForDBNull_String(_dsProductDetails.Tables(2).Rows(0)("DefaultPriceBandForBasket"))
                If productPriceBandForBasketMode = GlobalConstants.PRICE_BAND_DEFAULT_CUSTOMER Then
                    For Each priceBand As PriceBandPrices In _viewModel.PriceBandPricesList
                        If profile.User.Details.PRICE_BAND = priceBand.PriceBand Then
                            defaultPriceBand = profile.User.Details.PRICE_BAND
                        End If
                    Next
                End If
            End If
        End If

        If count > 0 Then
            Dim validSelection As Boolean = ValidateSelection(standSelected, areaSelected, selectedMinimumPrice, selectedMaximumPrice, selectedPriceBreakId)
            'If the selection is valid, check the quantity and proceed with the request
            If validSelection Then
                CATHelper.AssignPackageCATSeatDetail(Me.Page.Controls)
                Session("errormsg") = ""
                Dim callid As String = String.Empty
                If Request.QueryString("callid") IsNot Nothing AndAlso IsNumeric(Request.QueryString("callid").Trim) Then
                    callid = Request.QueryString("callid").Trim
                End If
                Dim redirectUrl As New StringBuilder
                redirectUrl.Append("~/Redirect/TicketingGateway.aspx?page=")
                redirectUrl.Append(Talent.eCommerce.Utilities.GetCurrentPageName(False))
                redirectUrl.Append("&function=AddToBasket&product=").Append(hdfProductCode.Value)
                redirectUrl.Append("&catmode=").Append(Request.QueryString("catmode"))
                redirectUrl.Append("&callid=").Append(callid)
                redirectUrl.Append("&stand=").Append(standSelected.Trim())
                redirectUrl.Append("&area=").Append(areaSelected.Trim())
                redirectUrl.Append("&quantity=").Append(quantity)
                redirectUrl.Append("&priceBand=").Append(defaultPriceBand)
                redirectUrl.Append("&campaign=").Append(hdfCampaignCode.Value)
                redirectUrl.Append("&productIsHomeAsAway=").Append(hdfProductHomeAsAway.Value)
                redirectUrl.Append("&productsubtype=").Append(hdfProductSubType.Value)
                redirectUrl.Append("&productstadium=").Append(hdfProductStadium.Value)
                redirectUrl.Append("&type=").Append(hdfProductType.Value)
                redirectUrl.Append("&defaultPrice=").Append("0")
                redirectUrl.Append("&isproductbundle=").Append(isProductBundle())
                redirectUrl.Append("&minPrice=").Append(selectedMinimumPrice)
                redirectUrl.Append("&maxPrice=").Append(selectedMaximumPrice)
                redirectUrl.Append("&priceBreakId=").Append(selectedPriceBreakId)
                Response.Redirect(redirectUrl.ToString())
            End If
        ElseIf count = 0 Then
            showError(String.Empty, _ucr.Content("NoQuantityErrorMessage", _languageCode, True))
        Else
            showError(String.Empty, _ucr.Content("ExessiveQuantityErrorMessage", _languageCode, True))
        End If
    End Sub

    Protected Sub btnSelectSeats_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSelectSeats.Click
        PerformSelectSeatsRedirect()
    End Sub

    Protected Sub rptPriceBand_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPriceBand.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim lblPriceBandDescription As Label = CType(e.Item.FindControl("lblPriceBandDescription"), Label)
            Dim txtPriceBandQuantity As TextBox = CType(e.Item.FindControl("txtPriceBandQuantity"), TextBox)
            Dim hdfPriceBand As HiddenField = CType(e.Item.FindControl("hdfPriceBand"), HiddenField)
            Dim rngPriceBandQuantity As RangeValidator = CType(e.Item.FindControl("rngPriceBandQuantity"), RangeValidator)
            Dim plhPriceBandLine As PlaceHolder = CType(e.Item.FindControl("plhPriceBandLine"), PlaceHolder)
            Dim priceBand As PriceBandPrices = CType(e.Item.DataItem, PriceBandPrices)

            hdfPriceBand.Value = priceBand.PriceBand
            lblPriceBandDescription.Text = priceBand.PriceBandDescription
            setQuantityDefaults(txtPriceBandQuantity, rngPriceBandQuantity)
        End If
    End Sub

    Protected Sub checkPriceBandList(ByVal productDefaultPriceBand As String, ByVal productAllowPBAlterations As String, ByVal productPriceBandMode As String)

        'Restricting PWS Price Band options at product level (customer or product default PB)
        If Not AgentProfile.IsAgent And productAllowPBAlterations = GlobalConstants.PRICE_BAND_ALTERATIONS_RESTRICTED Then
            Dim singlePriceBand As String = productDefaultPriceBand
            Dim customerPriceBand As String = String.Empty

            If productPriceBandMode = GlobalConstants.PRICE_BAND_DEFAULT_CUSTOMER AndAlso Not Profile.IsAnonymous AndAlso Profile.User.Details IsNot Nothing AndAlso Profile.User.Details.PRICE_BAND <> String.Empty Then
                customerPriceBand = Profile.User.Details.PRICE_BAND
            End If

            'Loop 1 - Check if we can match the customer price band if we are attempting to
            ' Price in customer mode.
            If customerPriceBand <> String.Empty Then
                For Each item As RepeaterItem In rptPriceBand.Items
                    Dim hdfPriceBand As HiddenField = CType(item.FindControl("hdfPriceBand"), HiddenField)
                    If hdfPriceBand.Value = customerPriceBand Then
                        singlePriceBand = customerPriceBand
                    End If
                Next
            End If

            'Loop 2 - Hide all items apart from the single item allowed (product or customer)
            For Each item As RepeaterItem In rptPriceBand.Items
                Dim hdfPriceBand As HiddenField = CType(item.FindControl("hdfPriceBand"), HiddenField)
                If hdfPriceBand.Value <> singlePriceBand Then
                    item.Visible = False
                End If
            Next
        End If
    End Sub

    Protected Sub ddlMinimumPrice_DataBound(sender As Object, e As EventArgs) Handles ddlMinimumPrice.DataBound
        For Each item As ListItem In ddlMinimumPrice.Items
            Dim price As Decimal = CDec(item.Text)
            item.Text = Server.HtmlDecode(TDataObjects.PaymentSettings.FormatCurrency(price, _ucr.BusinessUnit, _ucr.PartnerCode))
        Next
    End Sub

    Protected Sub ddlMaximumPrice_DataBound(sender As Object, e As EventArgs) Handles ddlMaximumPrice.DataBound
        For Each item As ListItem In ddlMaximumPrice.Items
            Dim price As Decimal = CDec(item.Text)
            item.Text = Server.HtmlDecode(TDataObjects.PaymentSettings.FormatCurrency(price, _ucr.BusinessUnit, _ucr.PartnerCode))
        Next
    End Sub

    Protected Function GetLoggedInCustomerPriceBandCss(ByVal priceBand As String) As String
        Dim cssClass As String = String.Empty
        Dim profile As ProfileCommon = HttpContext.Current.Profile
        If Not profile.IsAnonymous AndAlso profile.User.Details IsNot Nothing AndAlso profile.User.Details.PRICE_BAND = priceBand AndAlso ModuleDefaults.HighlightLoggedInCustomerPriceBand Then
            cssClass = "ebiz-priceband-logged-in-customer"
        End If
        Return cssClass
    End Function

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Check the product is a bundle type product and return value
    ''' </summary>
    ''' <returns>If the product is a bundle return true</returns>
    ''' <remarks></remarks>
    Private Function isProductBundle() As Boolean
        Dim productBundle As Boolean = False
        If Not _err.HasError AndAlso _dsProductDetails IsNot Nothing AndAlso _dsProductDetails.Tables.Count > 0 Then
            If _dsProductDetails.Tables(0).Rows.Count > 0 Then
                If _dsProductDetails.Tables(0).Rows(0)("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then
                    productBundle = _dsProductDetails.Tables(2).Rows(0)("IsProductBundle")
                End If
            End If
        End If
        Return productBundle
    End Function

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Determine the layout based on price/area or stand/area display
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setDisplayType()
        If ModuleDefaults.PriceAndAreaSelection Then
            AreaDDLFirstOption = _ucr.Content("areaPleaseSelectText", _languageCode, True)
            plhCombinedStandAndArea.Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("ShowPriceDropDownList"))
            plhStandDropDownList.Visible = False
            lblCombinedStandAndArea.Text = _ucr.Content("areaLabel", _languageCode, True)
            setCombinedStandAreaDropDownList()
        Else
            plhCombinedStandAndArea.Visible = False
            plhStandDropDownList.Visible = True
            standLabel.Text = _ucr.Content("standLabel", _languageCode, True)
            setStandDropDownList()
            WriteDDLJavascriptForStandArea()
        End If
        ltlStandAreaSelectionLegend.Text = _ucr.Content("BestAvailableLegendText", _languageCode, True)
        ltlStandAndAreaHeader.Text = _ucr.Content("StandAndAreaHeaderText", _languageCode, True)
        lblPriceBreakSelection.Text = _ucr.Content("priceBreakSelectionLabel", _languageCode, True)
        areaLabel.Text = _ucr.Content("areaLabel", _languageCode, True)
        plhHeader.Visible = (ltlStandAndAreaHeader.Text.Length > 0)
        If TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("ShowPriceBandList")) Then
            plhPriceBandList.Visible = True
            plhDefaultPriceBand.Visible = False
            btnPriceBandListBuyButton.Text = _ucr.Content("buyButton", _languageCode, True)
            pnlStandArea.DefaultButton = btnPriceBandListBuyButton.ID
        Else
            plhPriceBandList.Visible = False
            plhDefaultPriceBand.Visible = True
            lblQuantity.Text = _ucr.Content("qtyLabel", _languageCode, True)
            btnSingleQuantityBuyButton.Text = _ucr.Content("buyButton", _languageCode, True)
            pnlStandArea.DefaultButton = btnSingleQuantityBuyButton.ID
            setQuantityDefaults(txtQuantity, rngQuantity)
        End If
    End Sub

    ''' <summary>
    ''' Setup the minimum and maximum prices for the product
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setupMinimumAndMaximumDropDownList()
        plhMaximumAndMinimumPrice.Visible = False
        plhResetOption.Visible = False
        If Not _onLinkedProductsPage AndAlso Not _viewModel.Error.HasError Then
            If _viewModel.PriceBreakPrices.Count > 0 Then
                Dim minimumPriceAttribute As String = String.Empty
                Dim maximumPriceAttribute As String = String.Empty
                lblMinimumPrice.Text = _ucr.Content("MinimumPriceLabelText", _languageCode, True)
                ddlMinimumPrice.DataSource = _viewModel.PriceBreakPrices
                ddlMinimumPrice.DataValueField = "Price"
                ddlMinimumPrice.DataTextField = "Price"
                ddlMinimumPrice.DataBind()
                ddlMinimumPrice.SelectedIndex = 0
                If _onVisualSeatSelectionPage Then
                    minimumPriceAttribute = "document.getElementById('" & hdfSelectedMinimumPrice.ClientID & "').value=this.value; ReDrawStadium(false, true, false, false, false, false);"
                ElseIf _onLinkedProductsPage Then
                    minimumPriceAttribute = "document.getElementById('" & hdfSelectedMinimumPrice.ClientID & "').value=this.value;"
                Else
                    minimumPriceAttribute = "document.getElementById('" & hdfSelectedMinimumPrice.ClientID & "').value=this.value; RetrieveDynamicSeatSelectionOptions(false, true, false, false, false);"
                End If
                ddlMinimumPrice.Attributes.Add("onchange", minimumPriceAttribute)
                hdfSelectedMinimumPrice.Value = ddlMinimumPrice.SelectedValue

                lblMaximumPrice.Text = _ucr.Content("MaximumPriceLabelText", _languageCode, True)
                ddlMaximumPrice.DataSource = _viewModel.PriceBreakPrices
                ddlMaximumPrice.DataValueField = "Price"
                ddlMaximumPrice.DataTextField = "Price"
                ddlMaximumPrice.DataBind()
                ddlMaximumPrice.SelectedIndex = (_viewModel.PriceBreakPrices.Count - 1)
                If _onVisualSeatSelectionPage Then
                    maximumPriceAttribute = "document.getElementById('" & hdfSelectedMaximumPrice.ClientID & "').value=this.value; ReDrawStadium(false, false, true, false, false, false);"
                ElseIf _onLinkedProductsPage Then
                    maximumPriceAttribute = "document.getElementById('" & hdfSelectedMaximumPrice.ClientID & "').value=this.value;"
                Else
                    maximumPriceAttribute = "document.getElementById('" & hdfSelectedMaximumPrice.ClientID & "').value=this.value; RetrieveDynamicSeatSelectionOptions(false, false, true, false, false);"
                End If
                ddlMaximumPrice.Attributes.Add("onchange", maximumPriceAttribute)
                hdfSelectedMaximumPrice.Value = ddlMaximumPrice.SelectedValue

                ltlPricingFilterOption.Text = _ucr.Content("PricingFilterOptionText", _languageCode, True)
                ltlMinMaxPricingHeader.Text = _ucr.Content("MinMaxPricingHeaderText", _languageCode, True)
                ResetButtonText = _ucr.Content("ResetButtonText", _languageCode, True)
                plhMaximumAndMinimumPrice.Visible = True
                If _onVisualSeatSelectionPage Then
                    plhResetOption.Visible = True
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Bind the list of price bands to the repeater, unless we're on the linked products page
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setupPriceBandList()
        If Not IsPostBack AndAlso Not _onLinkedProductsPage AndAlso Not PickingExceptionSeat AndAlso TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowPriceBandList")) Then
            Dim productDefaultPriceBand = _dsProductDetails.Tables(2).Rows(0).Item("DefaultPriceBand")
            Dim productAllowPBAlterations = _dsProductDetails.Tables(2).Rows(0).Item("AllowPriceBandAlterations")
            Dim productPriceBandMode = _dsProductDetails.Tables(2).Rows(0).Item("DefaultPriceBandForBasket")
            rptPriceBand.DataSource = _viewModel.PriceBandPricesList
            rptPriceBand.DataBind()
            checkPriceBandList(productDefaultPriceBand, productAllowPBAlterations, productPriceBandMode)
            hdfShowPriceBandListAsDropDown.Value = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("ShowPriceBandListAsDropDown")).ToString().ToLower()
            ltlPriceBandOptions.Text = _ucr.Content("PriceBandOptionsText", _languageCode, True)
            ltlPriceBandOptionsHeader.Text = _ucr.Content("PriceBandOptionsHeaderText", _languageCode, True)
        End If
    End Sub

    ''' <summary>
    ''' Set the stand drop down list for stand/area selection
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setStandDropDownList()
        If Not _viewModel.Error.HasError Then
            If _viewModel.StadiumAvailabilityList.Count = 0 Then
                SoldOut = True
            Else
                If standDropDown.Items.Count = 0 Then
                    standDropDown.Visible = True
                    standDropDown.ToolTip = _ValueID
                    'Filter out the duplicate stand codes and only bind distinct records
                    Dim distinctList As New List(Of StadiumAvailability)
                    For i As Integer = 0 To _viewModel.StadiumAvailabilityList.Count - 1 Step 1
                        Dim okToAddItem As Boolean = True
                        If i > 0 Then
                            For Each item As StadiumAvailability In distinctList
                                If _viewModel.StadiumAvailabilityList.Item(i).StandCode = item.StandCode Then
                                    okToAddItem = False
                                    Exit For
                                End If
                            Next
                        Else
                            okToAddItem = True
                        End If
                        If okToAddItem Then distinctList.Add(_viewModel.StadiumAvailabilityList.Item(i))
                    Next
                    standDropDown.DataSource = distinctList
                    standDropDown.DataTextField = "StandDescription"
                    standDropDown.DataValueField = "StandCode"
                    standDropDown.DataBind()
                    If AlternativeSeatSelectionAcrossStands Then
                        standDropDown.Items.Insert(0, New ListItem(_ucr.Content("AnyStandLabel", _languageCode, True), String.Empty))
                    Else
                        standDropDown.Items.Insert(0, New ListItem(StandDDLPleaseSelectText, String.Empty))
                    End If
                    Dim areaListItem As ListItem = Nothing
                    If AlternativeSeatSelection Then
                        areaListItem = New ListItem(_ucr.Content("AnyAreaLabel", _languageCode, True), String.Empty)
                    Else
                        areaListItem = New ListItem(_ucr.Content("areaPleaseSelectText", _languageCode, True), String.Empty)
                    End If
                    If Not areaDropDownList.Items.Contains(areaListItem) Then
                        areaDropDownList.Items.Insert(0, areaListItem)
                    End If
                    If _onVisualSeatSelectionPage Then
                        hdfShowStandAreaOptionsAsDropDown.Value = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("ShowStandAreaAsDropDown")).ToString().ToLower()
                    End If
                    ltlSelectStandAreaOptions.Text = _ucr.Content("SelectStandAreaOptionsText", _languageCode, True)
                    ltlSelectStandAreaHeader.Text = _ucr.Content("SelectStandAreaHeaderText", _languageCode, True)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Set the quantity defaults based on CAT or quantity defintions (product relationships or Bulk Sales Mode)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setQuantityDefaults(ByRef txtQuantity As TextBox, ByRef rngQuantity As RangeValidator)
        txtQuantity.Attributes.Add("onkeyup", "onQuantityChanged();")
        txtQuantity.Attributes.Add("onchange", "onQuantityChanged();")
        If CATHelper.IsItCATRequest(-2) AndAlso Not CATHelper.IsPackageTransferRequested() Then
            txtQuantity.Text = "1"
            txtQuantity.ReadOnly = True
            txtQuantity.Attributes.Remove("onkeyup")
            txtQuantity.Attributes.Remove("onchange")
            rngQuantity.Enabled = False
        Else
            Dim tGatewayFunctions As New TicketingGatewayFunctions
            Dim minQuantity As Integer = 0
            Dim maxQuantity As Integer = 0
            Dim defaultQuantity As String = String.Empty
            Dim isReadOnly As Boolean = False
            tGatewayFunctions.GetQuantityDefintions(ProductCode, minQuantity, maxQuantity, defaultQuantity, isReadOnly)
            txtQuantity.Attributes.Add("min", minQuantity)
            txtQuantity.Attributes.Add("max", maxQuantity)
            txtQuantity.MaxLength = maxQuantity.ToString().Length
            rngQuantity.MinimumValue = minQuantity
            rngQuantity.MaximumValue = maxQuantity
            rngQuantity.Enabled = True
            rngQuantity.Type = ValidationDataType.Integer
            If minQuantity = maxQuantity Then
                rngQuantity.ErrorMessage = _errMsg.GetErrorMessage("QZ4").ERROR_MESSAGE.Replace("<<MAX_VALUE>>", maxQuantity)
            Else
                rngQuantity.ErrorMessage = _errMsg.GetErrorMessage("QZ3").ERROR_MESSAGE.Replace("<<MIN_VALUE>>", minQuantity).Replace("<<MAX_VALUE>>", maxQuantity)
            End If
            rngQuantity.ErrorMessage = rngQuantity.ErrorMessage.Replace("<<PRODUCT_DESCRIPTION>>", ProductDescription)
        End If
    End Sub

    ''' <summary>
    ''' Show the error message in parent control/page based on error code and message
    ''' </summary>
    ''' <param name="errCode">The error code</param>
    ''' <param name="errMessage">The error message</param>
    ''' <remarks></remarks>
    Private Sub showError(ByVal errCode As String, ByVal errMessage As String)
        'Retrieve the error message when error code is passed in
        If Not String.IsNullOrEmpty(errCode.Trim) Then
            errMessage = _errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, errCode.Trim).ERROR_MESSAGE
        End If

        'Add the error the session object which is used when the page is reloaded
        If Not String.IsNullOrEmpty(errMessage) Then
            Session("StandAreaSelectionError") = errMessage
            For Each item As RepeaterItem In rptPriceBand.Items
                Dim txtPriceBandQuantity As TextBox = CType(item.FindControl("txtPriceBandQuantity"), TextBox)
                txtPriceBandQuantity.Text = String.Empty
            Next
            txtQuantity.Text = String.Empty
            Response.Redirect(Request.Url.AbsoluteUri)
        End If
    End Sub

    ''' <summary>
    ''' Get the product details
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub getProductDetails()
        Dim settings As DESettings = Talent.eCommerce.Utilities.GetSettingsObject()
        Dim dsProductDetails As New DataSet
        settings.Cacheing = True
        _product = New TalentProduct
        _product.Settings() = settings
        _depd.CampaignCode = CampaignCode
        _depd.ProductCode = ProductCode
        _depd.Src = GlobalConstants.SOURCE
        _product.De = _depd
        _err = _product.ProductDetails
        dsProductDetails = _product.ResultDataSet

        If Not _err.HasError AndAlso dsProductDetails IsNot Nothing AndAlso dsProductDetails.Tables.Count > 0 Then
            If dsProductDetails.Tables(0).Rows.Count > 0 Then
                If dsProductDetails.Tables(0).Rows(0)("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then
                    _dsProductDetails = dsProductDetails
                    _productIsEnabledForTicketExchange = (TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dsProductDetails.Tables(2).Rows(0).Item("TicketExchangeEnabled")) AndAlso Not AgentProfile.BulkSalesMode)
                    If String.IsNullOrEmpty(ProductDescription) Then
                        ProductDescription = TEBUtilities.CheckForDBNull_String(dsProductDetails.Tables(2).Rows(0).Item("ProductText1")).Trim()
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Populate the DDLs for Price/Area
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setCombinedStandAreaDropDownList()
        If Not SoldOut Then
            Dim dtCombinedStandAndArea As DataTable = New DataTable
            Dim dRow As DataRow = Nothing
            Dim standAreaDescriptionMask As String = _ucr.Content("StandAreaDescriptionMask", _languageCode, True)
            Dim formattedStandAreaDescription As String = String.Empty
            Dim jScriptText As New StringBuilder

            dtCombinedStandAndArea.TableName = "CombinedStandAndArea"
            With dtCombinedStandAndArea.Columns
                .Add("StandAreaDescription", GetType(String))
                .Add("StandAreaCode", GetType(String))
            End With
            For Each item As StadiumAvailability In _viewModel.StadiumAvailabilityList
                dRow = dtCombinedStandAndArea.NewRow
                formattedStandAreaDescription = standAreaDescriptionMask
                formattedStandAreaDescription = formattedStandAreaDescription.Replace("<<STAND_DESCRIPTION>>", item.StandDescription)
                formattedStandAreaDescription = formattedStandAreaDescription.Replace("<<AREA_DESCRIPTION>>", item.AreaDescription)
                dRow("StandAreaDescription") = formattedStandAreaDescription
                dRow("StandAreaCode") = item.StandCode & "-" & item.AreaCode
                dtCombinedStandAndArea.Rows.Add(dRow)
            Next

            jScriptText.Append("Javascript: ").Append("onQuantityChanged();")
            jScriptText.Append("var standAreaSelected = this.value.split('-');")
            jScriptText.Append("var standSelected = standAreaSelected[0];")
            jScriptText.Append("var areaSelected = standAreaSelected[1];")
            jScriptText.Append("document.getElementById('").Append(hdfStandSelected.ClientID).Append("').value = standSelected;")
            jScriptText.Append("document.getElementById('").Append(hdfAreaSelected.ClientID).Append("').value = areaSelected;")
            If _onVisualSeatSelectionPage Then
                jScriptText.Append("ReDrawStadium(false, false, false, true, true, false);")
            ElseIf _onLinkedProductsPage Then
                jScriptText.Append("")
            Else
                jScriptText.Append("RetrieveDynamicSeatSelectionOptions(false, false, false, true, true);")
            End If
            ddlCombinedStandAndArea.DataSource = dtCombinedStandAndArea
            ddlCombinedStandAndArea.DataTextField = "StandAreaDescription"
            ddlCombinedStandAndArea.DataValueField = "StandAreaCode"
            ddlCombinedStandAndArea.DataBind()
            ddlCombinedStandAndArea.Items.Insert(0, New ListItem(AreaDDLFirstOption, String.Empty))
            ddlCombinedStandAndArea.SelectedIndex = 0
            ddlCombinedStandAndArea.Attributes.Add("onchange", jScriptText.ToString())

            If _onVisualSeatSelectionPage Then
                hdfShowStandAreaOptionsAsDropDown.Value = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("ShowStandAreaAsDropDown")).ToString().ToLower()
            End If
            ltlSelectStandAreaOptions.Text = _ucr.Content("SelectStandAreaOptionsText", _languageCode, True)
            ltlSelectStandAreaHeader.Text = _ucr.Content("SelectStandAreaHeaderText", _languageCode, True)
        End If
    End Sub

    ''' <summary>
    ''' Get the price band descript for the given price band code
    ''' </summary>
    ''' <param name="acceptedPriceBand">The price band code</param>
    ''' <returns>The full price band description</returns>
    ''' <remarks></remarks>
    Private Function getPriceBandDescription(ByVal acceptedPriceBand As String) As String
        For Each priceband In _dsProductDetails.Tables(1).Rows
            If priceband("PriceBand") = acceptedPriceBand Then
                Return priceband("PriceBandDescription")
            End If
        Next
        Return String.Empty
    End Function

    ''' <summary>
    ''' Setup the ticket exchange
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setupTicketExchange()
        hdfShowTicketExchangeAsDropDown.Value = False
        ticketExchangePanel.Visible = False
        If (TalentDefaults.TicketExchangeEnabled AndAlso Not AgentProfile.BulkSalesMode) AndAlso ProductType = GlobalConstants.HOMEPRODUCTTYPE AndAlso _onVisualSeatSelectionPage AndAlso Not PickingExceptionSeat Then
            If _productIsEnabledForTicketExchange Then
                If _onVisualSeatSelectionPage Then
                    hdfShowTicketExchangeAsDropDown.Value = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("ShowTicketExchangeAsDropDown")).ToString().ToLower()
                End If
                ticketExchangePanel.Visible = True
                divTicketExchangeSlider.Attributes.Add("data-step", 1)
                TicketExchangeOptionText = _ucr.Content("IncludeTicketExchangeSeatsText", _languageCode, True)
                TicketExchangeSliderText = _ucr.Content("IncludeTicketExchangeSliderText", _languageCode, True)
                IncludeTicketExchangeChecked = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("TicketExchangeDefaultOption"))
                ticketExchangeOption.Checked = IncludeTicketExchangeChecked
                ltlTicketExchangeOptions.Text = _ucr.Content("TicketExchangeOptionsText", _languageCode, True)
                ltlTicketExchangeHeader.Text = _ucr.Content("TicketExchangeHeaderText", _languageCode, True)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Populate the price break defintions for the given product
    ''' Use the entire list of price breaks setup on the product from MD141S and compare against the available price break ID list from WS011R
    ''' This functionality should only be enabled when "PRICE_AND_AREA_SELECTION" is disabled
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setupProductPriceBreakDefinitions()
        plhPriceBreaks.Visible = False
        If TalentDefaults.PriceBreaksEnabled AndAlso Not _onLinkedProductsPage Then
            lblPriceBreakSelection.Text = _ucr.Content("PriceBreakSelectionText", _languageCode, True)
            ltlPriceBreakSelection.Text = _ucr.Content("PriceBreakSelectionText", _languageCode, True)
            ddlPriceBreakSelection.DataSource = _viewModel.PriceBreakAvailabilityList
            ddlPriceBreakSelection.DataValueField = "PriceBreakID"
            ddlPriceBreakSelection.DataTextField = "PriceBreakDescription"
            ddlPriceBreakSelection.DataBind()
            If ddlPriceBreakSelection.Items.Count = 1 Then
                plhPriceBreaks.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("AlwaysShowPriceBreakDDL"))
            Else
                Dim pleaseSelectListItem As New ListItem
                pleaseSelectListItem.Value = 0
                pleaseSelectListItem.Text = _ucr.Content("PleaseSelectPriceBreakText", _languageCode, True)
                ddlPriceBreakSelection.Items.Insert(0, pleaseSelectListItem)
            End If
            _selectedPriceBreakId = ddlPriceBreakSelection.SelectedValue
            plhPriceBreaks.Visible = True
            If _onVisualSeatSelectionPage Then
                ddlPriceBreakSelection.Attributes.Add("onchange", "document.getElementById('" & hdfSelectedPriceBreakId.ClientID & "').value=this.value; ReDrawStadium(true, false, false, false, false, false);")
            ElseIf _onLinkedProductsPage Then
                ddlPriceBreakSelection.Attributes.Add("onchange", "document.getElementById('" & hdfSelectedPriceBreakId.ClientID & "').value=this.value;")
            Else
                ddlPriceBreakSelection.Attributes.Add("onchange", "document.getElementById('" & hdfSelectedPriceBreakId.ClientID & "').value=this.value; RetrieveDynamicSeatSelectionOptions(true, false, false, false, false);")
            End If
            If _onVisualSeatSelectionPage Then
                hdfShowPricingOptionsAsDropDown.Value = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("ShowPricingOptionsAsDropDown")).ToString().ToLower()
            End If
        End If
    End Sub

    ''' <summary>
    ''' Call TALENT to get the product stadium availability - WS011R
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateViewModel()
        Dim productAvailabilityModelBuilder As New ProductAvailabilityBuilder
        Dim inputModel As New ProductAvailabilityInputModel
        With inputModel
            .AgentLevelCacheForProductStadiumAvailability = ModuleDefaults.AgentLevelCacheForProductStadiumAvailability
            .CampaignCode = _product.De.CampaignCode
            .CapacityByStadium = _product.De.CapacityByStadium
            .CATMode = _product.De.CATMode
            .ComponentID = CATHelper.GetPackageComponentId(ProductCode, CallId)
            .IncludeTicketExchangeSeats = IncludeTicketExchangeChecked
            .PriceBreakId = _selectedPriceBreakId
            .ProductCode = _product.De.ProductCode
            .ProductType = _product.De.ProductType
            .Source = GlobalConstants.SOURCE
            .StadiumCode = _product.De.StadiumCode
            .DefaultPriceBand = ProductPriceBand
        End With
        _viewModel = productAvailabilityModelBuilder.GetProductAvailability(inputModel)
    End Sub

    ''' <summary>
    ''' Write the javascript code for changing the area options when using stand/area
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub WriteDDLJavascriptForStandArea()
        Dim sb As New StringBuilder
        Dim saveStandCode As String = String.Empty

        If Not SoldOut Then
            sb.Append(vbCrLf)
            sb.Append("<script language=""javascript"" type=""text/javascript"">").Append(vbCrLf)
            sb.Append("function LoadAreasFor").Append(ProductCode).Append(CampaignCode).Append("(thisform){").Append(vbCrLf)
            sb.Append("var ddl = document.getElementById('").Append(areaDropDownList.ClientID).Append("');").Append(vbCrLf)
            sb.Append("var newStandCode = document.getElementById('").Append(standDropDown.ClientID).Append("').value;").Append(vbCrLf)
            sb.Append("if (trim(newStandCode) == """") {").Append(vbCrLf)
            sb.Append("removeAllOptions(ddl);").Append(vbCrLf)
            If AlternativeSeatSelection Then
                sb.Append("addOption(ddl, """", """).Append(_ucr.Content("AnyAreaLabel", _languageCode, True)).Append(""");").Append(vbCrLf)
            Else
                sb.Append("addOption(ddl, """", """).Append(_ucr.Content("areaPleaseSelectText", _languageCode, True)).Append(""");").Append(vbCrLf)
            End If
            sb.Append("}").Append(vbCrLf)
            Dim sortExp As String = "StandCode, AreaDescription"

            For Each availability As StadiumAvailability In _viewModel.StadiumAvailabilityList
                ' New 'if' clause for each stand code 
                If availability.StandCode <> saveStandCode Then
                    If Not saveStandCode.Trim = "" Then
                        sb.Append("}").Append(vbCrLf)
                    End If
                    sb.Append("if (trim(newStandCode) == """).Append(availability.StandCode).Append(""") {").Append(vbCrLf)
                    sb.Append("removeAllOptions(ddl);").Append(vbCrLf)
                    If AlternativeSeatSelection Then
                        sb.Append("addOption(ddl, """", """).Append(_ucr.Content("AnyAreaLabel", _languageCode, True)).Append(""");").Append(vbCrLf)
                    Else
                        sb.Append("addOption(ddl, """", """).Append(_ucr.Content("areaPleaseSelectText", _languageCode, True)).Append(""");").Append(vbCrLf)
                    End If
                End If

                ' New 'addOption' function call for each area code
                sb.Append("addOption(ddl, """).Append(availability.AreaCode).Append(""", """).Append(availability.AreaDescription).Append(""");").Append(vbCrLf)
                saveStandCode = availability.StandCode
            Next
            sb.Append("}").Append(vbCrLf)
            sb.Append("}").Append(vbCrLf)

            ' Function to reset DDL immediately prior to form_submit so that back to default when returning to page
            sb.Append("function LoadDefaultsFor").Append(ProductCode).Append(CampaignCode).Append("(standDDL, areaDDL) {").Append(vbCrLf)
            sb.Append("var theStandCode = document.getElementById('").Append(standDropDown.ClientID).Append("').value;").Append(vbCrLf)
            sb.Append("var theAreaCode = document.getElementById('").Append(areaDropDownList.ClientID).Append("').value;").Append(vbCrLf)
            sb.Append("document.getElementById('").Append(hdfStandSelected.ClientID).Append("').value=theStandCode;").Append(vbCrLf)
            sb.Append("document.getElementById('").Append(hdfAreaSelected.ClientID).Append("').value=theAreaCode;").Append(vbCrLf)
            sb.Append("removeAllOptions(areaDDL);").Append(vbCrLf)
            sb.Append("addOption(areaDDL, """", """");").Append(vbCrLf)
            sb.Append("standDDL.selectedIndex=0;").Append(vbCrLf)
            sb.Append("}").Append(vbCrLf)

            ' Javascript trim function
            sb.Append("function trim(s) { ").Append(vbCrLf).Append("var r=/\b(.*)\b/.exec(s); ").Append(vbCrLf).Append("return (r==null)?"""":r[1]; ").Append(vbCrLf).Append("}").Append(vbCrLf)
            If AlternativeSeatSelection Then
                sb.Append("function validateAreaDropdown() {").Append(vbCrLf).AppendLine("// if there are 2 options only - in area dropdown then by default select the second value instead of ""Any area""")
                sb.AppendLine("if(document.getElementById('" & areaDropDownList.ClientID & "').options.length == 2) { ")
                sb.AppendLine("document.getElementById('" & areaDropDownList.ClientID & "').selectedIndex = 1;")
                sb.AppendLine("document.getElementById('" & _hdfAreaSelected.ClientID & "').value = document.getElementById('" & areaDropDownList.ClientID & "').options[1].value ;").AppendLine("}").AppendLine("}")
            End If

            sb.Append("</script>")
        End If
        ltlJavascriptDDL.Text = sb.ToString()
    End Sub

    ''' <summary>
    ''' Handle any errors and perform a redirect based on the property values that are present
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub handleErrors(ByVal errCode As String, ByVal errMessage As String, Optional doErrorHandlingRedirect As Boolean = True)
        Dim log As New TalentLogging
        If _product.De.ProductCode Is Nothing Then
            log.GeneralLog("SeatSelectionError", errCode, errMessage, "SeatSelectionLog")
        Else
            log.GeneralLog("SeatSelectionError", errCode, "Product Code: " & _product.De.ProductCode & " | " & errMessage, "SeatSelectionLog")
        End If
        If doErrorHandlingRedirect Then
            Dim redirectUrl As New StringBuilder
            If Request.UrlReferrer Is Nothing Then
                redirectUrl.Append("~/PagesPublic/ProductBrowse/")
                If ProductType = GlobalConstants.HOMEPRODUCTTYPE Then
                    redirectUrl.Append("ProductHome.aspx?")
                ElseIf ProductType = GlobalConstants.SEASONTICKETPRODUCTTYPE Then
                    redirectUrl.Append("ProductSeason.aspx?")
                End If
                If ProductSubType.Length > 0 Then redirectUrl.Append("ProductSubType=").Append(ProductSubType).Append("&")
                If CampaignCode.Length > 0 Then redirectUrl.Append("Campaign").Append(CampaignCode)
            Else
                redirectUrl.Append(Request.UrlReferrer.AbsoluteUri)
            End If
            Response.Redirect(redirectUrl.ToString())
        End If
    End Sub

#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Validate the selection of area and stand drop down options and return a boolean value to indicate whether it is valid
    ''' </summary>
    ''' <param name="areaSelected">The selected area string, which is set here</param>
    ''' <param name="standSelected">The selected stand string, which is set here</param>
    ''' <returns>Whether or not selection is valid</returns>
    ''' <remarks></remarks>
    Public Function ValidateSelection(ByRef standSelected As String, ByRef areaSelected As String, ByRef selectedMinimumPrice As String, ByRef selectedMaximumPrice As String, ByRef selectedPriceBreakId As String) As Boolean
        Dim validSelection As Boolean = False

        'Validate the min/max price and price break Id
        selectedMinimumPrice = TEBUtilities.CheckForDBNull_Decimal(hdfSelectedMinimumPrice.Value).ToString()
        selectedMaximumPrice = TEBUtilities.CheckForDBNull_Decimal(hdfSelectedMaximumPrice.Value).ToString()
        selectedPriceBreakId = TEBUtilities.CheckForDBNull_Long(hdfSelectedPriceBreakId.Value).ToString()

        'Validate Price/Area or Stand/Area
        If ModuleDefaults.PriceAndAreaSelection Then
            If String.IsNullOrEmpty(ddlCombinedStandAndArea.SelectedValue) Then
                standSelected = hdfStandSelected.Value
            Else
                standSelected = ddlCombinedStandAndArea.SelectedValue.Split("-")(0)
            End If
            If String.IsNullOrEmpty(ddlCombinedStandAndArea.SelectedValue) Then
                areaSelected = hdfAreaSelected.Value
            Else
                areaSelected = ddlCombinedStandAndArea.SelectedValue.Split("-")(1)
            End If
        Else
            If String.IsNullOrEmpty(standDropDown.SelectedValue) Then
                standSelected = hdfStandSelected.Value
            Else
                standSelected = standDropDown.SelectedValue
            End If
            If String.IsNullOrEmpty(areaDropDownList.SelectedValue) Then
                areaSelected = hdfAreaSelected.Value
            Else
                areaSelected = areaDropDownList.SelectedValue
            End If
        End If

        'Validate against alternative seat selection
        If AlternativeSeatSelection Then
            If AlternativeSeatSelectionAcrossStands Then
                validSelection = True
            Else
                If standSelected.Length > 0 Then
                    validSelection = True
                Else
                    showError(String.Empty, _errMsg.GetErrorMessage("ST-AR").ERROR_MESSAGE.Replace("<<PRODUCT_DESCRIPTION>>", ProductDescription))
                End If
            End If
        Else
            If standSelected.Length > 0 AndAlso areaSelected.Length > 0 Then
                validSelection = True
            Else
                showError(String.Empty, _errMsg.GetErrorMessage("ST-AR").ERROR_MESSAGE.Replace("<<PRODUCT_DESCRIPTION>>", ProductDescription))
            End If
        End If

        'Validate *ALL and Roving
        If validSelection Then
            If standSelected.Length = 0 Then
                standSelected = GlobalConstants.STARALLSTAND
            Else
                ' if roving then set stand code correctly again
                If standSelected.Contains("*" & _ucr.Content("RovingAreaText", _languageCode, True)) Then
                    standSelected = standSelected.Substring(0, standSelected.IndexOf("*"))
                End If
            End If
            If areaSelected.Length = 0 Then areaSelected = GlobalConstants.STARALLAREA
        End If
        Return validSelection
    End Function

    ''' <summary>
    ''' Gewt the stand change javascript function code
    ''' </summary>
    ''' <returns>The formatted javascript function</returns>
    ''' <remarks></remarks>
    Public Function GetJavascriptStringStandChange() As String
        Dim jScriptText As New StringBuilder
        jScriptText.Append("Javascript: document.getElementById('")
        jScriptText.Append(hdfStandSelected.ClientID)
        jScriptText.Append("').value=this.value; LoadAreasFor").Append(ProductCode).Append(CampaignCode)
        jScriptText.Append("(this.form);")
        jScriptText.Append("Javascript: document.getElementById('")
        jScriptText.Append(hdfAreaSelected.ClientID).Append("').value=document.getElementById('")
        jScriptText.Append(areaDropDownList.ClientID).Append("').value;")
        If AlternativeSeatSelection Then jScriptText.Append("validateAreaDropdown();")
        jScriptText.Append("Javascript: ").Append("onQuantityChanged();")
        If _onVisualSeatSelectionPage Then
            jScriptText.Append("Javascript: ").Append("ReDrawStadium(false, false, false, true, false, false);")
        ElseIf _onLinkedProductsPage Then
            jScriptText.Append("")
        Else
            jScriptText.Append("Javascript: ").Append("RetrieveDynamicSeatSelectionOptions(false, false, false, true, false);")
        End If
        Return jScriptText.ToString()
    End Function

    ''' <summary>
    ''' Get the area change javascript function to set the correct hiddenfields
    ''' </summary>
    ''' <returns>The formatted javascript function</returns>
    ''' <remarks></remarks>
    Public Function GetJavascriptStringAreaChange() As String
        Dim jScriptText As New StringBuilder
        jScriptText.Append("Javascript: document.getElementById('")
        jScriptText.Append(hdfAreaSelected.ClientID)
        jScriptText.Append("').value=this.value;")
        jScriptText.Append("Javascript: ").Append("onQuantityChanged();")
        If _onVisualSeatSelectionPage Then
            jScriptText.Append("Javascript: ").Append("ReDrawStadium(false, false, false, false, true, false);")
        ElseIf _onLinkedProductsPage Then
            jScriptText.Append("")
        Else
            jScriptText.Append("Javascript: ").Append("RetrieveDynamicSeatSelectionOptions(false, false, false, false, true);")
        End If
        Return jScriptText.ToString()
    End Function

    ''' <summary>
    ''' Get the mouse up event for the buy button
    ''' </summary>
    ''' <returns>The formatted javascript function</returns>
    ''' <remarks></remarks>
    Public Function GetJavascriptStringBuyMouseUp() As String
        Dim jScriptText As New StringBuilder
        jScriptText.Append("Javascript: LoadDefaultsFor")
        jScriptText.Append(ProductCode).Append(CampaignCode)
        jScriptText.Append("(this.form.")
        jScriptText.Append(standDropDown.ClientID)
        jScriptText.Append(", this.form.")
        jScriptText.Append(areaDropDownList.ClientID).Append(");")
        Return jScriptText.ToString()
    End Function

    ''' <summary>
    ''' Redirect to the seat selection page based on the public properties
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PerformSelectSeatsRedirect()
        Dim standSelected As String = String.Empty
        Dim areaSelected As String = String.Empty
        Dim validSelection As Boolean = ValidateSelection(standSelected, areaSelected, hdfSelectedMinimumPrice.Value, hdfSelectedMaximumPrice.Value, hdfSelectedPriceBreakId.Value)

        If validSelection Then
            Session("errormsg") = ""
            Dim redirectUrl As New StringBuilder
            redirectUrl.Append("~/PagesPublic/ProductBrowse/seatSelection.aspx?product=")
            redirectUrl.Append(ProductCode)
            redirectUrl.Append("&stand=")
            redirectUrl.Append(standSelected)
            redirectUrl.Append("&area=")
            redirectUrl.Append(areaSelected)
            redirectUrl.Append("&stadium=")
            redirectUrl.Append(ProductStadium)
            redirectUrl.Append("&campaign=")
            redirectUrl.Append(CampaignCode)
            redirectUrl.Append("&type=")
            redirectUrl.Append(ProductType)
            redirectUrl.Append("&productsubtype=")
            redirectUrl.Append(ProductSubType)
            redirectUrl.Append("&catmode=")
            redirectUrl.Append(Request.QueryString("catmode"))
            If Session("callid") IsNot Nothing AndAlso IsNumeric(Session("callid").ToString()) Then
                redirectUrl.Append("&callid=" & Session("callid"))
                redirectUrl.Append("&istrnxenq=True")
            End If
            If Session("payref") IsNot Nothing AndAlso IsNumeric(Session("payref").ToString()) Then
                redirectUrl.Append("&payref=" & Session("payref"))
            End If
            If Session("catseatcustomerno") IsNot Nothing AndAlso IsNumeric(Session("catseatcustomerno").ToString()) Then
                redirectUrl.Append("&catseatcustomerno=" & Session("catseatcustomerno"))
            End If
            redirectUrl.Append("&priceBreakId=").Append(hdfSelectedPriceBreakId.Value)
            redirectUrl.Append("&selectedMinimumPrice=").Append(hdfSelectedMinimumPrice.Value)
            redirectUrl.Append("&selectedMaximumPrice=").Append(hdfSelectedMaximumPrice.Value)
            Response.Redirect(redirectUrl.ToString())
        End If
    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Fill the default stand information when the stand is pre-loaded
    ''' </summary>
    ''' <param name="strStrnd">The stand code</param>
    ''' <param name="strArea">The area code</param>
    ''' <remarks></remarks>
    Public Sub StandDropDownFill(ByVal strStrnd As String, ByVal strArea As String)
        getProductDetails()
        populateViewModel()
        setStandDropDownList()
        areaDropDownList.DataSource = _viewModel.StadiumAvailabilityList.FindAll(Function(x) (x.StandCode = strStrnd))
        areaDropDownList.DataTextField = "AreaDescription"
        areaDropDownList.DataValueField = "AreaCode"
        areaDropDownList.DataBind()
        Dim areaListItem As ListItem = Nothing
        If AlternativeSeatSelection Then
            areaListItem = New ListItem(_ucr.Content("AnyAreaLabel", _languageCode, True), String.Empty)
        Else
            areaListItem = New ListItem(_ucr.Content("areaPleaseSelectText", _languageCode, True), String.Empty)
        End If
            If Not areaDropDownList.Items.Contains(areaListItem) Then
                areaDropDownList.Items.Insert(0, areaListItem)
            End If
        If strStrnd.Length > 0 Then
            standDropDown.SelectedValue = strStrnd
            hdfStandSelected.Value = strStrnd
        Else
            hdfStandSelected.Value = standDropDown.SelectedValue
        End If
        If strArea.Length > 0 Then
            areaDropDownList.SelectedValue = strArea
            hdfAreaSelected.Value = strArea
        Else
            hdfAreaSelected.Value = areaDropDownList.SelectedValue
        End If
    End Sub

#End Region

End Class