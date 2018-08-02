Imports System.Collections.Generic
Imports System.Data
Imports System.IO
Imports System.Linq
Imports System.Web.Script.Serialization
Imports System.Web.HttpUtility
Imports System.Xml
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.ModelBuilders.Products
Imports Talent.Common
Imports Talent.eCommerce
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_SeatSelection
    Inherits ControlBase

#Region "Constants"

    Private Const POINTS As String = "points"
    Private Const FILL As String = "fill"
    Private Const STROKE As String = "stroke"
    Private Const STROKE_MITER_LIMIT As String = "stroke-miterlimit"
    Private Const FILL_RULE As String = "fill-rule"
    Private Const CLIP_RULE As String = "clip-rule"
    Private Const STROKE_WIDTH As String = "stroke-width"
    Private Const TRANSFORM As String = "transform"
    Private Const FONT_FAMILY As String = "font-family"
    Private Const FONT_SIZE As String = "font-size"
    Private Const XMLID As String = "id"
    Private Const HYPHEN As String = "-"
    Private Const AREA_INFORMATION_SH As String = "AIn"
    Private Const AREA_SH As String = "A"
    Private Const WIDTH As String = "width"
    Private Const HEIGHT As String = "height"
    Private Const WIDTH_SH As String = "w"
    Private Const HEIGHT_SH As String = "h"
    Private Const FILL_SH As String = "fl"
    Private Const PRICE_FILL_SH As String = "pfl"
    Private Const AVAILABILITY_FILL_SH As String = "afl"
    Private Const STROKE_SH As String = "str"
    Private Const STROKE_MITER_LIMIT_SH As String = "sml"
    Private Const FILL_RULE_SH As String = "flr"
    Private Const CLIP_RULE_SH As String = "clr"
    Private Const TYPE_SH As String = "t"
    Private Const SEAT_SELECTION_SH As String = "ss"
    Private Const POINTS_SH As String = "p"
    Private Const TRANSFORM_SH As String = "tr"
    Private Const FONT_FAMILY_SH As String = "ff"
    Private Const FONT_SIZE_SH As String = "fs"
    Private Const TEXT_SH As String = "tx"
    Private Const STAND_AND_AREA_ID_SH As String = "fid"
    Private Const AVAILABILITY_SH As String = "a"
    Private Const STROKE_WIDTH_SH As String = "sw"
    Private Const CX As String = "cx"
    Private Const CY As String = "cy"
    Private Const R As String = "r"
    Private Const X1 As String = "x1"
    Private Const Y1 As String = "y1"
    Private Const X2 As String = "x2"
    Private Const Y2 As String = "y2"
    Private Const D As String = "d"
    Private Const X As String = "x"
    Private Const Y As String = "y"
    Private Const NA As String = "na"
    Private Const AVAILABILITY_PERCENTAGE_SH = "ap"
    Private Const HOVER_COLOUR_SH = "hvc"
    Private Const TICKET_EXCHANGE_FLAG = "txf"
    Private Const TICKET_EXCHANGE_MIN_SLIDER_PRICE = "isp"
    Private Const TICKET_EXCHANGE_MAX_SLIDER_PRICE = "xsp"
    Private Const SELECTED_SH = "sel"
    Private Const SELECTED_TEXT_SH = "selt"
    Private Const EMPTY_STRING = " "
#End Region

#Region "Class Level Fields"

    Private _errMsg As TalentErrorMessages
    Private _ucr As UserControlResource = Nothing
    Private _businessUnit As String = Nothing
    Private _partnerCode As String = Nothing
    Private _languageCode As String = Nothing
    Private _standAndAreaXml As XmlDocument = Nothing
    Private _svgContent As XmlDocument = Nothing
    Private _product As TalentProduct = Nothing
    Private _err As ErrorObj = Nothing
    Private _settings As DESettings = Nothing
    Private _dtProductList As DataTable = Nothing
    Private _dtProductAvailability As DataTable = Nothing
    Private _dtAvailablePriceBreakIds As DataTable = Nothing
    Private _dtProductPriceDetails As DataTable = Nothing
    Private _dtStandAreaDescriptions As DataTable = Nothing
    Private _dtStadiumAvailablityAreaColoursAndText As DataTable = Nothing
    Private _dtStadiumPricingAreaColoursAndText As DataTable = Nothing
    Private _dtStadiumSeatColoursAndText As DataTable = Nothing

    Private _dtStadiums As DataTable = Nothing
    Private _stadiumName As String = String.Empty
    Private _defaultPriceBand As String = Nothing
    Private _isSeatSelection As Boolean
    Private _loggedInCustomerPriceBand As String = String.Empty
    Private _log As TalentLogging = Nothing
    Private _isProductSeatSelectionEnabled As Boolean
    Private _isProductBundle As Boolean
    Private _hoverColour As String = String.Empty
    Private _selectedColour As String = String.Empty
    Private _selectedTextColour As String = String.Empty
    Private _componentId As String = String.Empty
    Private _packageId As String = String.Empty
    Private _priceBands As DataTable = Nothing
    Private _priceBandsDescription As DataTable = Nothing
    Private _rowSeatRowOnSVGOffText As String = String.Empty
    Private _rowSeatRowOnSVGOnText As String = String.Empty
    Private _linkedMasterProduct As String = String.Empty
    Private _pickingExceptionSeat As Boolean = False
    Private _currentExceptionSeat As String = String.Empty
    Private _selectedPriceBreakId As Long = 0
    Private _productIsEnabledForTicketExchange As Boolean = False
    Private _productIsAvailableForSale As Boolean = True
    Private _productAllowPricebandSelection As Boolean = True
    Private _productPriceBandForBasketMode As String = String.Empty
    Private _hasError As Boolean = False
    Private _defaultStandAndAreaClick As Boolean = False
    Private _pickingNewComponentSeat As Boolean = False
    Private _changeAllSeats As Boolean = False
    Private _oldComponentSeat As String = String.Empty
    Private _dtCorporateHospitalityDetails As DataTable = Nothing
    Private _dtComponentSeats As DataTable = Nothing
    Private _dtComponents As DataTable = Nothing
    Private _dBasketSeats As Dictionary(Of String, String)
#End Region

#Region "Public Properties"

    Public Property ProductCode() As String
    Public Property StadiumCode() As String
    Public Property ProductType() As String
    Public Property ProductSubType() As String
    Public Property CampaignCode() As String
    Public Property ProductIsHomeAsAway() As String
    Public Property CATMode() As String
    Public Property IsCatMode() As Boolean
    Public Property StadiumXml() As String
    Public Property StadiumAvailabilityJSON() As String
    Public Property DescriptionsXml() As String
    Public Property StandAndAreaCode() As String
    Public Property IsSeatSelectionOnly() As Boolean
    Public Property DefaultToSeatSelection() As Boolean
    Public Property DefaultToQuickBuy() As Boolean
    Public Property ViewFromAreaButtonText() As String
    Public Property ClearSelectionButtonText() As String
    Public Property ChangeStandViewButtonText() As String
    Public Property LoginMessageTitle() As String
    Public Property CloseModalWindowText() As String
    Public Property MultiSelectOnText() As String
    Public Property MultiSelectOffText() As String
    Public Property BackToStadiumViewText() As String
    Public Property InformationTextCSSClassname() As String
    Public Property ClearQuickBuyText() As String
    Public Property StandPrefixText() As String
    Public Property AreaPrefixText() As String
    Public Property RowPrefixText() As String
    Public Property SeatPrefixText() As String
    Public Property AvailabilityPrefixText() As String
    Public Property PricePrefixText() As String
    Public Property SeatPricePrefixText() As String
    Public Property ResetButtonText() As String
    Public Property TransferJavaScriptBasket() As String
    Public Property RowSeatRowOnSVGText() As String
    Public Property MediumClass() As String
    Public Property Ticketing3DSeatView() As Boolean
    Public Property CallId() As String
    Public Property TicketExchangeOptionText() As String
    Public Property IncludeTicketExchangeChecked() As Boolean
    Public Property SelectedMinimumPrice() As Decimal
    Public Property SelectedMaximumPrice() As Decimal
    Public Property SelectedStand() As String
    Public Property SelectedArea() As String
    Public Property ItemsInBasketText() As String
    Public Property Stand As String = String.Empty
    Public Property Area As String = String.Empty

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Request.QueryString("callid") IsNot Nothing Then CallId = Request.QueryString("callid")
        If Request.QueryString("stand") IsNot Nothing Then Stand = Request.QueryString("stand")
        If Request.QueryString("area") IsNot Nothing Then Area = Request.QueryString("area")
        If IsSeatSelectionOnly Then
            StadiumCode = Request.QueryString("stadium")
            StandAndAreaCode = Stand & HYPHEN & Area
        Else
            If Stand.Length > 0 AndAlso Area.Length > 0 Then
                DefaultToSeatSelection = True
                StandAndAreaCode = Stand & HYPHEN & Area
            ElseIf ModuleDefaults.DefaultStandAndAreaClick Then
                StandAndAreaCode = Stand & HYPHEN & Area
                _defaultStandAndAreaClick = True
            End If
        End If
        If Request.QueryString("oldseat") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Request.QueryString("oldseat")) AndAlso Request.QueryString("oldseat").ToString().Trim.Length > 0 Then
            _pickingExceptionSeat = True
            _currentExceptionSeat = Request.QueryString("oldseat")
        Else
            _pickingExceptionSeat = False
        End If
        If Not String.IsNullOrEmpty(Request.QueryString("callid")) AndAlso (Not String.IsNullOrEmpty(Request.QueryString("oldseat")) OrElse Not String.IsNullOrEmpty(Request.QueryString("changeallseats"))) AndAlso Not String.IsNullOrEmpty(Request.QueryString("packageId")) AndAlso Not String.IsNullOrEmpty(Request.QueryString("componentId")) Then
            _pickingNewComponentSeat = True
            _packageId = Request.QueryString("packageId")
            _componentId = Request.QueryString("componentId")

            If Not String.IsNullOrEmpty(Request.QueryString("oldseat")) Then
                _oldComponentSeat = Request.QueryString("oldseat")
                plhStandAndArea.Visible = False
            ElseIf Not String.IsNullOrEmpty(Request.QueryString("changeallseats")) Then
                _changeAllSeats = TCUtilities.convertToBool(Request.QueryString("changeallseats"))
            Else
                _pickingNewComponentSeat = False
                plhStandAndArea.Visible = False
            End If
        Else
            _pickingNewComponentSeat = False
            plhStandAndArea.Visible = False
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        initialiseClassObjects()
        populateGameSelectionList()
        If Not IsPostBack Then
            getColoursAndTextValues()
            getStadiumDescriptions()
            createStandAndAreaDescriptionXML()
            If IsSeatSelectionOnly Then
                uscFavouriteSeatSelection.Visible = False
                uscStandAndAreaSelection.Visible = False
                MediumClass = String.Empty
                getProductDetails(True, False)
                getProductPriceDetails(True)
                getComponentStandAndAreaList()
            Else
                getProductDetails()
                getStadiumSVG()
                getProductAvailability()
                getProductPriceDetails()
                populateStandAndAreaXml()
                populateAvailabilityKey()
                populatePricingKey()
                MediumClass = "medium-6"
            End If
            _dtStadiums = TDataObjects.StadiumSettings.TblStadiums.GetStadiumByStadiumCodeAndName(StadiumCode, _stadiumName, _businessUnit)

            plhStandPriceSelectionBoxes.Visible = uscStandAndAreaSelection.Visible

            populateJavaScriptBasket()
            _linkedMasterProduct = TDataObjects.BasketSettings.TblBasketHeader.GetLinkedMasterProduct(Profile.Basket.Basket_Header_ID)
            populateSeatingKey()
            populateTextAndDefaultsSettings()
            setJavascriptDefaults()
            setFacebookLike()
            populateCorporateHospitalityDetails()

            If _pickingExceptionSeat OrElse _pickingNewComponentSeat Then
                uscFavouriteSeatSelection.Visible = False
                uscStandAndAreaSelection.PickingExceptionSeat = _pickingExceptionSeat
                ddlGameSelection.Enabled = False

                If _pickingNewComponentSeat Then
                    hplBackToHospitalityBookingPage.Visible = True
                    Dim navigateUrl As New StringBuilder
                    navigateUrl.Append("~/PagesPublic/Hospitality/HospitalityBooking.aspx?product=").Append(ProductCode)
                    navigateUrl.Append("&packageid=").Append(_packageId)
                    hplBackToHospitalityBookingPage.NavigateUrl = navigateUrl.ToString()
                    hplBackToHospitalityBookingPage.Text = _ucr.Content("BackToHospitalityBookingPage", _languageCode, True)
                ElseIf _pickingExceptionSeat Then
                    hplBackToSTExceptionsPage.Visible = True
                    hplBackToSTExceptionsPage.NavigateUrl = "~/PagesPublic/ProductBrowse/SeasonTicketExceptions.aspx"
                    hplBackToSTExceptionsPage.Text = _ucr.Content("BackToExceptionsPage", _languageCode, True)
                End If
            Else
                hplBackToSTExceptionsPage.Visible = False
                hplBackToHospitalityBookingPage.Visible = False
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If CATHelper.IsItCATRequest(-2) Then
            ddlGameSelection.AutoPostBack = False
            ddlGameSelection.Enabled = False
        End If
        If Not String.IsNullOrEmpty(Session("TicketingGatewayError")) Then
            Dim errorMsg As New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), _ucr.FrontEndConnectionString)
            Dim talentErrorMsg As TalentErrorMessage = errorMsg.GetErrorMessage(GlobalConstants.STARALLPARTNER, _ucr.PageCode, Session("TicketingGatewayError"))
            blErrorMessages.Items.Clear()
            blErrorMessages.Items.Add(talentErrorMsg.ERROR_MESSAGE)
            Session("TicketingGatewayError") = Nothing
        End If
        If Not String.IsNullOrEmpty(Session("TalentErrorCode")) Then
            Dim errorMsg As New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), _ucr.FrontEndConnectionString)
            Dim talentErrorMsg As TalentErrorMessage = errorMsg.GetErrorMessage(GlobalConstants.STARALLPARTNER, _ucr.PageCode, Session("TalentErrorCode"))
            blErrorMessages.Items.Clear()
            blErrorMessages.Items.Add(talentErrorMsg.ERROR_MESSAGE)
            Session("TalentErrorCode") = Nothing
        End If
        If Not String.IsNullOrEmpty(Session("StandAreaSelectionError")) Then
            blErrorMessages.Items.Clear()
            blErrorMessages.Items.Add(Session("StandAreaSelectionError"))
            Session("StandAreaSelectionError") = Nothing
        End If
        plhErrorList.Visible = blErrorMessages.Items.Count > 0
    End Sub

    Protected Sub btnQuickBuy_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnQuickBuy.Click
        Dim quantity As Integer
        If Int32.TryParse(hdfQuickBuyQuantity.Value, quantity) AndAlso quantity > 0 Then
            Dim standAndAreaCode() As String = hdfQuickBuyStandAreaCode.Value.Split(HYPHEN)
            Dim redirectUrl As New StringBuilder
            redirectUrl.Append("~/Redirect/TicketingGateway.aspx?page=VisualSeatSelection.aspx&function=AddToBasket")
            redirectUrl.Append("&product=").Append(ProductCode)
            redirectUrl.Append("&stand=").Append(standAndAreaCode(0))
            redirectUrl.Append("&area=").Append(standAndAreaCode(1))
            redirectUrl.Append("&quantity=").Append(hdfQuickBuyQuantity.Value)
            redirectUrl.Append("&priceBand=A&campaign=").Append(CampaignCode)
            redirectUrl.Append("&productIsHomeAsAway=").Append(ProductIsHomeAsAway)
            redirectUrl.Append("&productsubtype=").Append(ProductSubType)
            redirectUrl.Append("&productstadium=").Append(StadiumCode)
            redirectUrl.Append("&type=").Append(ProductType)
            redirectUrl.Append("&isproductbundle=").Append(_isProductBundle.ToString())
            redirectUrl.Append("&componentId=").Append(_componentId)
            redirectUrl.Append("&packageId=").Append(_packageId)
            redirectUrl.Append("&minPrice=").Append(uscStandAndAreaSelection.SelectedPriceMinimum)
            redirectUrl.Append("&maxPrice=").Append(uscStandAndAreaSelection.SelectedPriceMaximum)
            redirectUrl.Append("&priceBreakId=").Append(TCUtilities.CheckForDBNull_BigInt(uscStandAndAreaSelection.PriceBreakId))
            Response.Redirect(redirectUrl.ToString())
        Else
            blErrorMessages.Items.Add(_ucr.Content("ZeroSeatsSelectedError", _languageCode, True))
        End If
    End Sub

    Protected Sub btnBuyTickets_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBuy.Click
        Dim seats As String = hdfSelectedSeats.Value
        Dim responseString As String = String.Empty
        Dim seatsList As New List(Of DESeatDetails)
        Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
        Dim oldSeat As String = String.Empty

        Dim temp As New Control
        seats = seats.Replace("-", "/").ToUpper()
        For Each item As String In seats.Split(",")
            Dim details As String() = item.Split("#")
            Dim seat As New DESeatDetails
            seat.FormattedSeat = details(0)
            seat.PriceBand = details(1)
            seatsList.Add(seat)
        Next
        If _pickingExceptionSeat Then oldSeat = _currentExceptionSeat
        If _pickingNewComponentSeat Then oldSeat = _oldComponentSeat
        responseString = ticketingGatewayFunctions.SeatSelectionAddToBasket(ProductCode, ProductType, ProductSubType, StadiumCode, CampaignCode, seatsList, _isProductBundle, oldSeat, 0, IsSeatSelectionOnly, _pickingNewComponentSeat, CallId, _changeAllSeats, _packageId, _componentId)
        Response.Redirect(responseString)
    End Sub

    Protected Sub ddlGameSelection_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlGameSelection.SelectedIndexChanged
        Dim productCode As String = ddlGameSelection.SelectedValue
        Dim redirectUrl As String = String.Empty
        For Each row As DataRow In _dtProductList.Rows
            If row("ProductCode") = productCode Then
                If row("ProductType") <> GlobalConstants.SEASONTICKETPRODUCTTYPE Then CampaignCode = String.Empty
                redirectUrl = TEBUtilities.GetFormattedSeatSelectionUrl(String.Empty, TEBUtilities.CheckForDBNull_String(row("ProductStadium")), productCode, CampaignCode,
                            TEBUtilities.CheckForDBNull_String(row("ProductType")), TEBUtilities.CheckForDBNull_String(row("ProductSubType")),
                            TEBUtilities.CheckForDBNull_String(row("ProductHomeAsAway")), TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(row("RestrictGraphical")))
                Exit For
            End If
        Next
        Response.Redirect(ResolveUrl(redirectUrl))
    End Sub

#End Region

#Region "Private Methods"
    ''' <summary>
    ''' Set the class level field properties
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub initialiseClassObjects()
        _err = New ErrorObj
        _product = New TalentProduct
        _settings = TEBUtilities.GetSettingsObject()
        _settings.Cacheing = True
        _product.Settings() = _settings
        _product.De.ProductCode = ProductCode
        _product.De.CapacityByStadium = ModuleDefaults.CapacityByStadium
        _product.De.Src = GlobalConstants.SOURCE
        _product.De.CampaignCode = CampaignCode
        _product.De.ProductType = ProductType
        _product.De.CATMode = CATMode
        _product.De.StadiumCode = StadiumCode
        CATHelper.AssignPackageTransferDetail(_packageId, ProductCode)
        Ticketing3DSeatView = False

        _ucr = New UserControlResource
        _businessUnit = TalentCache.GetBusinessUnit()
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        _languageCode = TCUtilities.GetDefaultLanguage()
        _log = TEBUtilities.TalentLogging
        _dBasketSeats = New Dictionary(Of String, String)
        _dtComponentSeats = New DataTable
        _dtComponents = New DataTable
        With _ucr
            .BusinessUnit = _businessUnit
            .PageCode = TEBUtilities.GetCurrentPageName()
            .PartnerCode = _partnerCode
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "SeatSelection.ascx"
        End With
    End Sub

    ''' <summary>
    ''' Get the Stadium SVG graphic data from the SVG file and add to the cache if it's not present already
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub getStadiumSVG(Optional ByVal doErrorHandlingRedirect As Boolean = True)
        _stadiumName = TDataObjects.StadiumSettings.TblStadiums.GetStadiumNameByStadiumCode(StadiumCode, _businessUnit)
        Dim overiddenStadiumName As String = TDataObjects.StadiumSettings.TblStadiumOverride.GetOverridenStadiumNameForProduct(ProductCode, _businessUnit)
        If overiddenStadiumName <> String.Empty Then
            _stadiumName = overiddenStadiumName
        End If
        If _stadiumName.Length > 0 Then
            _svgContent = New XmlDocument
            Dim cacheKey As String = ProductCode & _stadiumName & "_StadiumSVGContent"
            Dim svgSettings As DESettings = _settings
            svgSettings.Cacheing = True
            svgSettings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("SVGContentCacheTimeInMins"))
            If svgSettings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                _svgContent = CType(HttpContext.Current.Cache.Item(cacheKey), XmlDocument)
            Else
                Dim stadiumSVGpath As New StringBuilder
                stadiumSVGpath.Append(HttpContext.Current.Server.MapPath("~/Stadiums/")).Append(_stadiumName).Append(".svg")
                If File.Exists(stadiumSVGpath.ToString()) Then
                    Try
                        _svgContent.Load(stadiumSVGpath.ToString())
                    Catch ex As Exception
                        handleErrors("SS-01", ex.Message, doErrorHandlingRedirect)
                    End Try
                Else
                    handleErrors("SS-02", "SVG file does not exist.", doErrorHandlingRedirect)
                End If
                Dim talentCommonBase As New TalentBase
                talentCommonBase.AddItemToCache(cacheKey, _svgContent, svgSettings)
            End If
        Else
            handleErrors("SS-03", "Missing stadium code.", doErrorHandlingRedirect)
        End If
    End Sub

    ''' <summary>
    ''' Call TALENT to get the product details
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub getProductDetails(Optional ByVal doErrorHandlingRedirect As Boolean = True, Optional ByVal callSetupMethods As Boolean = True)
        Dim dsProductDetails As New DataSet
        _product.ResultDataSet = Nothing
        _err = _product.ProductDetails
        dsProductDetails = _product.ResultDataSet

        If Not _err.HasError AndAlso dsProductDetails IsNot Nothing AndAlso dsProductDetails.Tables.Count > 0 Then
            If dsProductDetails.Tables(0).Rows.Count > 0 Then
                If dsProductDetails.Tables(0).Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                    handleErrors("SS-04", "Talent error: " + dsProductDetails.Tables(0).Rows(0)("ReturnCode"), doErrorHandlingRedirect)
                Else
                    _productIsEnabledForTicketExchange = isproductEnabledForTicketExchange(dsProductDetails.Tables("ProductDetails"))
                    _productIsAvailableForSale = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dsProductDetails.Tables(2).Rows(0)("AvailableOnline"))
                    _isProductSeatSelectionEnabled = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(dsProductDetails.Tables(2).Rows(0).Item("UseVisualSeatLevelSelection"))
                    _productAllowPricebandSelection = False
                    If AgentProfile.IsAgent OrElse
                         (Profile.IsAnonymous AndAlso TEBUtilities.CheckForDBNull_String(dsProductDetails.Tables(2).Rows(0)("AllowPriceBandAlterations")).Trim() = GlobalConstants.PRICE_BAND_ALTERATIONS_PLUS_ANONYMOUS) OrElse
                         (Not Profile.IsAnonymous AndAlso Not TEBUtilities.CheckForDBNull_String(dsProductDetails.Tables(2).Rows(0)("AllowPriceBandAlterations")).Trim() = GlobalConstants.PRICE_BAND_ALTERATIONS_RESTRICTED) Then
                        If (_pickingExceptionSeat OrElse _pickingNewComponentSeat) Then
                            _productAllowPricebandSelection = False
                        Else
                            _productAllowPricebandSelection = True
                        End If
                    End If

                    _productPriceBandForBasketMode = TEBUtilities.CheckForDBNull_String(dsProductDetails.Tables(2).Rows(0)("DefaultPriceBandForBasket"))
                    If _productPriceBandForBasketMode = GlobalConstants.PRICE_BAND_DEFAULT_CUSTOMER Then
                        getCustomerPriceBand()
                    End If
                    checkAgentAuthority(TEBUtilities.CheckForDBNull_String(dsProductDetails.Tables(2).Rows(0).Item("ProductType")))
                    If callSetupMethods Then
                        setProductDetails(dsProductDetails.Tables(2))
                        setStandAndAreaSelection(dsProductDetails.Tables(2))
                        setFavouriteSeatSelection(dsProductDetails.Tables(2))
                        If ModuleDefaults.ShowProductTextOnVisualSeatSelection Then
                            setExtendedProductText(dsProductDetails.Tables(2))
                        End If
                    End If
                    If dsProductDetails.Tables(1).Rows.Count > 0 Then
                        _priceBandsDescription = dsProductDetails.Tables(1)
                    End If
                End If
            End If
        Else
            handleErrors("SS-05", "Product Details error.", doErrorHandlingRedirect)
        End If
    End Sub

    ''' <summary>
    ''' Set the text based on the product
    ''' </summary>
    ''' <param name="dtProductDetails">Data table of product details</param>
    ''' <remarks></remarks>
    Private Sub setProductDetails(ByRef dtProductDetails As DataTable)
        ltlMatchHeader.Text = dtProductDetails.Rows(0)("ProductDescription").ToString().Trim()
        lblInformationText.Text = ltlMatchHeader.Text
        ltlCompetition.Text = dtProductDetails.Rows(0)("ProductText1").ToString().Trim()
        If (TEBUtilities.CheckForDBNull_String(dtProductDetails.Rows(0)("ProductDate"))).Length = 7 Then
            ltlDateAndTime.Text = TEBUtilities.GetFormattedDateAndTime(dtProductDetails.Rows(0)("ProductDate").ToString().Substring(3, 4),
                                dtProductDetails.Rows(0)("ProductTime").ToString().Trim(), _ucr.Content("DateSeparator", _languageCode, True), ModuleDefaults.GlobalDateFormat, ModuleDefaults.Culture)
        End If
        If ProductType = GlobalConstants.HOMEPRODUCTTYPE Then ltlVersus.Text = _ucr.Content("VersusText", _languageCode, True)
        plhMatchHeader.Visible = (ltlMatchHeader.Text.Length > 0)
        plhInformationText.Visible = (lblInformationText.Text.Length > 0)
        plhCompetition.Visible = (ltlCompetition.Text.Length > 0)
        plhDateAndTime.Visible = (ltlDateAndTime.Text.Length > 0)
        plhVersus.Visible = (ltlVersus.Text.Length > 0)
        imgHomeTeam.ImageUrl = ImagePath.getImagePath("PRODTICKETING", dtProductDetails.Rows(0)("HomeTeamCode").ToString().Trim(), _businessUnit, _partnerCode)
        If imgHomeTeam.ImageUrl = ModuleDefaults.MissingImagePath Then
            plhHomeTeam.Visible = False
        Else
            plhHomeTeam.Visible = True
            imgHomeTeam.AlternateText = ltlMatchHeader.Text
        End If
        If ProductType = GlobalConstants.HOMEPRODUCTTYPE Then
            imgAwayTeam.ImageUrl = ImagePath.getImagePath("PRODTICKETING", dtProductDetails.Rows(0)("OppositionCode").ToString().Trim(), _businessUnit, _partnerCode)
            If imgAwayTeam.ImageUrl = ModuleDefaults.MissingImagePath Then
                plhAwayTeam.Visible = False
            Else
                plhAwayTeam.Visible = True
                imgAwayTeam.AlternateText = ltlMatchHeader.Text
            End If
        Else
            plhAwayTeam.Visible = False
        End If
        plhTeamsWrapper.Visible = (plhHomeTeam.Visible OrElse plhVersus.Visible OrElse plhAwayTeam.Visible)
        _isProductBundle = dtProductDetails.Rows(0)("IsProductBundle")
    End Sub

    ''' <summary>
    ''' Call TALENT to get the product stadium availability - WS011R
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub getProductAvailability(Optional ByVal componentID As Long = 0, Optional ByVal doErrorHandlingRedirect As Boolean = True)
        Dim dsStadiumAvailablity As New DataSet
        If Not componentID = 0 Then
            _product.De.ComponentID = componentID
        Else
            _product.De.ComponentID = CATHelper.GetPackageComponentId(ProductCode, CallId)
        End If
        _product.AgentLevelCacheForProductStadiumAvailability = ModuleDefaults.AgentLevelCacheForProductStadiumAvailability
        _product.De.PriceBreakId = _selectedPriceBreakId
        _product.De.IncludeTicketExchangeSeats = IncludeTicketExchangeChecked
        _product.De.SelectedMaximumPrice = SelectedMaximumPrice
        _product.De.SelectedMinimumPrice = SelectedMinimumPrice
        _product.De.PackageID = _packageId
        _product.ResultDataSet = Nothing
        _err = _product.ProductStadiumAvailability()
        dsStadiumAvailablity = _product.ResultDataSet

        'If return with errorcode send them to basket page and if it is for cat clear the cache
        If Not _err.HasError AndAlso dsStadiumAvailablity IsNot Nothing AndAlso dsStadiumAvailablity.Tables.Count > 0 Then
            If TCUtilities.CheckForDBNull_String(dsStadiumAvailablity.Tables(0).Rows(0)("ErrorOccurred")).Trim.Length > 0 Then
                If doErrorHandlingRedirect Then
                    Session("TalentErrorCode") = TCUtilities.CheckForDBNull_String(dsStadiumAvailablity.Tables(0).Rows(0)("ReturnCode"))
                    If IsCatMode Then TEBUtilities.ClearOrderEnquiryDetailsCache()
                    Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                End If
            End If
            If dsStadiumAvailablity.Tables(0).Rows.Count > 0 Then
                If dsStadiumAvailablity.Tables(0).Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                    handleErrors("SS-06", "Talent error: " + dsStadiumAvailablity.Tables(0).Rows(0)("ReturnCode"), doErrorHandlingRedirect)
                Else
                    _dtProductAvailability = dsStadiumAvailablity.Tables("StadiumAvailability")
                    _dtAvailablePriceBreakIds = dsStadiumAvailablity.Tables("AvailablePriceBreaks")
                    If Not String.IsNullOrEmpty(SelectedStand) OrElse Not String.IsNullOrEmpty(SelectedArea) Then
                        Dim filterString As String = String.Empty
                        If Not String.IsNullOrEmpty(SelectedArea) Then
                            filterString = "AreaCode='" & SelectedArea & "'"
                        End If
                        If Not String.IsNullOrEmpty(SelectedStand) Then
                            Dim standCode As String = SelectedStand
                            If SelectedStand.Contains("*" & _ucr.Content("RovingAreaText", _languageCode, True)) Then
                                standCode = SelectedStand.Substring(0, SelectedStand.IndexOf("*"))
                            End If
                            If String.IsNullOrEmpty(filterString) Then
                                filterString = "StandCode='" & standCode & "'"
                            Else
                                filterString = filterString & " AND StandCode='" & standCode & "'"
                            End If
                        End If
                        Dim dvFilterProductAvailability As New DataView(_dtProductAvailability)
                        dvFilterProductAvailability.RowFilter = filterString
                        _dtProductAvailability = dvFilterProductAvailability.ToTable()
                    End If
                End If
            Else
                handleErrors("SS-07", "Unknown error.", doErrorHandlingRedirect)
            End If
        Else
            handleErrors("SS-08", "Stadium Availability error.", doErrorHandlingRedirect)
        End If
    End Sub

    Private Sub getCustomerPriceBand()
        Dim profile As ProfileCommon = HttpContext.Current.Profile
        If Not profile.IsAnonymous AndAlso profile.User.Details IsNot Nothing Then
            _loggedInCustomerPriceBand = profile.User.Details.PRICE_BAND
        End If
    End Sub

    ''' <summary>
    ''' Call TALENT to get the product default price data
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub getProductPriceDetails(Optional ByVal doErrorHandlingRedirect As Boolean = True)
        Dim dsProductPriceDetails As New DataSet
        _product.ResultDataSet = Nothing
        _err = _product.ProductPricingDetails()
        dsProductPriceDetails = _product.ResultDataSet

        If dsProductPriceDetails IsNot Nothing AndAlso dsProductPriceDetails.Tables.Count > 0 Then
            If dsProductPriceDetails.Tables(0).Rows.Count > 0 Then
                If dsProductPriceDetails.Tables(0).Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                    handleErrors("SS-09", "Talent error: " + dsProductPriceDetails.Tables(0).Rows(0)("ReturnCode"), doErrorHandlingRedirect)
                Else
                    _defaultPriceBand = dsProductPriceDetails.Tables(0).Rows(0)("DefaultPriceBand")
                    _priceBands = dsProductPriceDetails.Tables(2)
                    _dtProductPriceDetails = dsProductPriceDetails.Tables(1)
                End If
            Else
                handleErrors("SS-10a", "Unknown error.", doErrorHandlingRedirect)
            End If
        Else
            handleErrors("SS-11a", "Price Details error.", doErrorHandlingRedirect)
        End If
    End Sub

    ''' <summary>
    ''' Populate area and seat colours datatables for use with availability key, FF seating, pricing area colouring and seat colouring
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub getColoursAndTextValues()
        _dtStadiumAvailablityAreaColoursAndText = New DataTable
        _dtStadiumAvailablityAreaColoursAndText = TDataObjects.StadiumSettings.TblStadiumAreaColours.GetStadiumAvailabilityColoursAndText(_businessUnit, StadiumCode)
        _dtStadiumPricingAreaColoursAndText = New DataTable
        _dtStadiumPricingAreaColoursAndText = TDataObjects.StadiumSettings.TblStadiumAreaColours.GetStadiumPricingColoursAndText(_businessUnit, StadiumCode)
        _dtStadiumSeatColoursAndText = New DataTable
        _dtStadiumSeatColoursAndText = TDataObjects.StadiumSettings.TblStadiumSeatColours.GetStadiumSeatColoursAndText(_businessUnit, StadiumCode, False)
        _hoverColour = TDataObjects.StadiumSettings.TblStadiumAreaColours.GetSingleStadiumColour(_businessUnit, StadiumCode, "HOVER")
        _selectedColour = TDataObjects.StadiumSettings.TblStadiumAreaColours.GetSingleStadiumColour(_businessUnit, StadiumCode, "SELECTED")
        _selectedTextColour = TDataObjects.StadiumSettings.TblStadiumAreaColours.GetSingleStadiumColour(_businessUnit, StadiumCode, "SELECTEDTEXT")
    End Sub
    ''' <summary>
    ''' Sets Extended Product Text
    ''' </summary>
    ''' <param name="dtProductDetails"></param>
    ''' <remarks></remarks>
    Private Sub setExtendedProductText(ByRef dtProductDetails As DataTable)
        Dim rw As DataRow = dtProductDetails.Rows(0)
        Dim productDetailText As String = rw("ProductDetail1").ToString &
                                          rw("ProductDetail2").ToString &
                                          rw("ProductDetail3").ToString &
                                          rw("ProductDetail4").ToString &
                                          rw("ProductDetail5").ToString
        If String.IsNullOrWhiteSpace(productDetailText) Then
            plhExtendedText.Visible = False
        Else
            lblExtendedText1.Text = rw("ProductDetail1").ToString
            lblExtendedText2.Text = rw("ProductDetail2").ToString
            lblExtendedText3.Text = rw("ProductDetail3").ToString
            lblExtendedText4.Text = rw("ProductDetail4").ToString
            lblExtendedText5.Text = rw("ProductDetail5").ToString
        End If
    End Sub
    ''' <summary>
    ''' Call TALENT to get the stadium stand and area descriptions
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub getStadiumDescriptions(Optional ByVal doErrorHandlingRedirect As Boolean = True)
        Dim dsStandAreaDescriptions As New DataSet
        _product.ResultDataSet = Nothing
        _settings.Cacheing = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("StandAreaDescriptionsCacheing"))
        _settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("StandAreaDescriptionsCacheTimeMinutes"))
        _err = _product.StandDescriptions()
        dsStandAreaDescriptions = _product.ResultDataSet

        If Not _err.HasError AndAlso dsStandAreaDescriptions IsNot Nothing AndAlso dsStandAreaDescriptions.Tables.Count > 0 Then
            If dsStandAreaDescriptions.Tables(0).Rows.Count > 0 Then
                If dsStandAreaDescriptions.Tables(0).Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                    handleErrors("SS-11", "Talent error: " + dsStandAreaDescriptions.Tables(0).Rows(0)("ReturnCode"), doErrorHandlingRedirect)
                Else
                    _dtStandAreaDescriptions = dsStandAreaDescriptions.Tables(1)
                End If
            End If
        Else
            handleErrors("SS-12", "Stand Description error.", doErrorHandlingRedirect)
        End If
    End Sub

    ''' <summary>
    ''' Populate a hidden field with stand and area descriptions for seating hover over
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createStandAndAreaDescriptionXML()
        Dim standAreaDescriptionXML As XmlDocument = New XmlDocument
        Dim ndSADescNode As XmlNode
        ndSADescNode = standAreaDescriptionXML.CreateElement("SAD")
        standAreaDescriptionXML.AppendChild(ndSADescNode)
        For Each description In _dtStandAreaDescriptions.Rows
            Dim atSCode, atSDescription, atACode, atADescription As XmlAttribute
            Dim colourSAA As XmlNode
            With standAreaDescriptionXML
                colourSAA = .CreateElement("saa")
                atSCode = .CreateAttribute("sc")
                atSDescription = .CreateAttribute("sd")
                atACode = .CreateAttribute("ac")
                atADescription = .CreateAttribute("ad")
            End With
            atSCode.Value = description("StandCode")
            atSDescription.Value = description("StandDescription")
            atACode.Value = description("AreaCode")
            atADescription.Value = description("AreaDescription")

            colourSAA.Attributes.Append(atSCode)
            colourSAA.Attributes.Append(atSDescription)
            colourSAA.Attributes.Append(atACode)
            colourSAA.Attributes.Append(atADescription)
            ndSADescNode.AppendChild(colourSAA)
        Next

        DescriptionsXml = standAreaDescriptionXML.InnerXml.Replace("""", "'")
    End Sub

    ''' <summary>
    ''' Populate the hiddenfield used by Javascript Raphael to draw the areas and all other graphics
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateStandAndAreaXml()
        _standAndAreaXml = New XmlDocument
        Dim ndAreaInformation As XmlNode
        Dim atHover, atSelected, atSelectedText As XmlAttribute
        ndAreaInformation = _standAndAreaXml.CreateElement(AREA_INFORMATION_SH)
        With _standAndAreaXml
            atHover = .CreateAttribute(HOVER_COLOUR_SH)
            atSelected = .CreateAttribute(SELECTED_SH)
            atSelectedText = .CreateAttribute(SELECTED_TEXT_SH)
            atHover.Value = _hoverColour
            atSelected.Value = _selectedColour
            atSelectedText.Value = _selectedTextColour
        End With

        _standAndAreaXml.AppendChild(ndAreaInformation)
        ndAreaInformation.Attributes.Append(atHover)
        ndAreaInformation.Attributes.Append(atSelected)
        ndAreaInformation.Attributes.Append(atSelectedText)
        Dim svgNodeList As XmlNodeList = _svgContent.SelectSingleNode("//svg").ChildNodes
        For Each ndSVGElement As XmlNode In svgNodeList
            Select Case ndSVGElement.Name
                Case Is = "rect" : defineRectangleStandAndArea(ndSVGElement, ndAreaInformation)
                Case Is = "polygon" : definePolygonStandAndArea(ndSVGElement, ndAreaInformation)
                Case Is = "path" : definePathStandAndArea(ndSVGElement, ndAreaInformation)
                Case Is = "circle" : defineCircleStandAndArea(ndSVGElement, ndAreaInformation)
                Case Is = "line" : defineLineStandAndArea(ndSVGElement, ndAreaInformation)
                Case Is = "text" : defineTextStandAndArea(ndSVGElement, ndAreaInformation)
                Case Is = "polyline" : definePolylineStandAndArea(ndSVGElement, ndAreaInformation)
            End Select
            If ndSVGElement.ChildNodes.Count > 0 Then
                recurseThroughNodes(ndSVGElement, ndAreaInformation)
            End If
        Next

        StadiumXml = _standAndAreaXml.InnerXml.Replace("""", "'")
    End Sub

    ''' <summary>
    ''' Loop through child nodes within the current node until all nodes are defined
    ''' </summary>
    ''' <param name="svgNodeList">The current node working with</param>
    ''' <param name="ndAreaInformation">The area information node</param>
    ''' <remarks></remarks>
    Private Sub recurseThroughNodes(ByVal svgNodeList As XmlNode, ByRef ndAreaInformation As XmlNode)
        For Each ndSVGElement As XmlNode In svgNodeList
            Select Case ndSVGElement.Name
                Case Is = "rect" : defineRectangleStandAndArea(ndSVGElement, ndAreaInformation)
                Case Is = "polygon" : definePolygonStandAndArea(ndSVGElement, ndAreaInformation)
                Case Is = "path" : definePathStandAndArea(ndSVGElement, ndAreaInformation)
                Case Is = "circle" : defineCircleStandAndArea(ndSVGElement, ndAreaInformation)
                Case Is = "line" : defineLineStandAndArea(ndSVGElement, ndAreaInformation)
                Case Is = "text" : defineTextStandAndArea(ndSVGElement, ndAreaInformation)
                Case Is = "polyline" : definePolylineStandAndArea(ndSVGElement, ndAreaInformation)
            End Select
            If ndSVGElement.ChildNodes.Count > 0 Then
                recurseThroughNodes(ndSVGElement, ndAreaInformation)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Get the Area Description from the stand and area code data
    ''' </summary>
    ''' <param name="standAndAreaId">The concatenated stand and area id split by hyphen</param>
    ''' <returns>The area description as a string</returns>
    ''' <remarks></remarks>
    Private Function getAreaDescription(ByVal standAndAreaId As String) As String
        Dim areaDescription As String = String.Empty
        Dim id() As String = standAndAreaId.Split(HYPHEN)
        Dim areaCode As String = String.Empty
        Dim standCode As String = String.Empty
        If id.Length > 1 Then
            standCode = id(0).ToUpper()
            areaCode = id(1).ToUpper()
        Else
            areaCode = id(0).ToUpper()
        End If
        For Each row As DataRow In _dtStandAreaDescriptions.Rows
            If row("StandCode") = standCode AndAlso row("AreaCode") = areaCode Then
                areaDescription = row("AreaDescription").ToString()
                Exit For
            End If
        Next
        Return HtmlEncode(areaDescription)
    End Function

    ''' <summary>
    ''' Get the Stand Description from the stand and area code data
    ''' </summary>
    ''' <param name="standAndAreaId">The concatenated stand and area id split by hyphen</param>
    ''' <returns>The stand description as a string</returns>
    ''' <remarks></remarks>
    Private Function getStandDescription(ByVal standAndAreaId As String) As String
        Dim standDescription As String = String.Empty
        Dim standId() As String = standAndAreaId.Split(HYPHEN)
        For Each row As DataRow In _dtStandAreaDescriptions.Rows
            If row("StandCode") = standId(0).ToUpper() Then
                standDescription = row("StandDescription").ToString()
                Exit For
            End If
        Next
        Return HtmlEncode(standDescription)
    End Function

    ''' <summary>
    ''' Get the seat selection enabled flag as Y/N string value
    ''' </summary>
    ''' <param name="standAndAreaId">The concatenated stand and area id split by hyphen</param>
    ''' <returns>Y or N depending on seat selection is enabled</returns>
    ''' <remarks></remarks>
    Private Function getSeatSelectionSetting(ByVal standAndAreaId As String) As String
        Dim seatSelectionEnabled As String = String.Empty
        Dim standId() As String = standAndAreaId.Split(HYPHEN)
        For Each row As DataRow In _dtProductAvailability.Rows
            If row("StandCode") = standId(0).ToUpper() AndAlso row("AreaCode") = standId(1).ToUpper() Then
                seatSelectionEnabled = row("SeatSelection").ToString()
                'If Seat Selection disabled, bring up quick buy menu instead
                If DefaultToSeatSelection AndAlso standAndAreaId = StandAndAreaCode AndAlso Not TCUtilities.convertToBool(seatSelectionEnabled) Then
                    DefaultToSeatSelection = False
                    DefaultToQuickBuy = True
                End If
                Exit For
            End If
        Next
        Return seatSelectionEnabled
    End Function

    ''' <summary>
    ''' Get the default price for the given stand and area based on the default price band
    ''' </summary>
    ''' <param name="standAndAreaId">The concatenated stand and area id split by hyphen</param>
    ''' <returns>A formatted string along with the currency type</returns>
    ''' <remarks></remarks>
    Private Function getAreaDefaultPrice(ByVal standAndAreaId As String) As String
        Dim price As String = String.Empty
        Dim standId() As String = standAndAreaId.Split(HYPHEN)
        For Each row As DataRow In _dtProductPriceDetails.Rows
            If row("StandCode") = standId(0).ToUpper() AndAlso row("AreaCode") = standId(1).ToUpper() AndAlso row("PriceBand") = _defaultPriceBand Then
                price = TDataObjects.PaymentSettings.FormatCurrency(CDec(row("Price")), _ucr.BusinessUnit, _ucr.PartnerCode)
                price = HttpContext.Current.Server.HtmlDecode(price)
                Exit For
            End If
        Next
        Return price
    End Function

    ''' <summary>
    ''' Get the area availabilty colour and text values by the given stand and area code data.
    ''' If the area has no availability set the noAvailability flag
    ''' </summary>
    ''' <param name="standAndAreaId">The concatenated stand and area id split by hyphen</param>
    ''' <param name="availabilityColour">The availabilty colour code as a CSS HEX value</param>
    ''' <param name="availabilityText">The availability colour text value</param>
    ''' <param name="noAvailability">Boolean value to represent if the area has no availability</param>
    ''' <remarks></remarks>
    Private Sub getAvailabilityColourAndText(ByVal standAndAreaId As String, ByRef availabilityColour As String, ByRef availabilityText As String, ByRef noAvailability As Boolean)
        Dim availabilityPercentage As Integer = 0
        Dim standId() As String = standAndAreaId.Split(HYPHEN)
        For Each row As DataRow In _dtProductAvailability.Rows
            If row("StandCode") = standId(0).ToUpper() AndAlso row("AreaCode") = standId(1).ToUpper() And _productIsAvailableForSale Then
                Integer.TryParse(row("Availability").ToString(), availabilityPercentage)
                Exit For
            End If
        Next
        If _dtStadiumAvailablityAreaColoursAndText.Rows.Count > 0 Then
            Dim availabiltyRow() As DataRow = _dtStadiumAvailablityAreaColoursAndText.Select("MIN <= " & availabilityPercentage & " AND MAX >= " & availabilityPercentage)
            If availabiltyRow.Length > 0 Then
                availabilityColour = availabiltyRow(0)("COLOUR").ToString()
                availabilityText = availabiltyRow(0)("TEXT").ToString()
            End If
        End If
        noAvailability = (availabilityPercentage < 1)
    End Sub

    ''' <summary>
    ''' Get the stand availability value from the given stand and area information
    ''' </summary>
    ''' <param name="standAndAreaId">The given stand and area code</param>
    ''' <param name="availability">The availability value</param>
    ''' <remarks></remarks>
    Private Sub getStandAvailability(ByVal standAndAreaId As String, ByRef availability As Integer, ByRef ticketingExchangeFlag As String, ByRef minTicketExchangePrice As String, ByRef maxTicketExchangePrice As String)
        Dim standId() As String = standAndAreaId.Split(HYPHEN)
        For Each row As DataRow In _dtProductAvailability.Rows
            If row("StandCode") = standId(0).ToUpper() AndAlso row("AreaCode") = standId(1).ToUpper() Then
                If _productIsAvailableForSale AndAlso Not (TEBUtilities.CheckForDBNull_Decimal(row("MinTicketExchangePrice")) = 0 AndAlso TEBUtilities.CheckForDBNull_Decimal(row("MaxTicketExchangePrice")) = 0) Then
                    Integer.TryParse(row("Availability").ToString(), availability)
                    ticketingExchangeFlag = (TEBUtilities.CheckForDBNull_StringBoolean_DefaultFalse(row("TicketExchangeAllowPurchase")) AndAlso Not AgentProfile.BulkSalesMode)
                    minTicketExchangePrice = TEBUtilities.CheckForDBNull_Decimal(row("MinTicketExchangePrice"))
                    maxTicketExchangePrice = TEBUtilities.CheckForDBNull_Decimal(row("MaxTicketExchangePrice"))
                    ticketingExchangeFlag = ticketingExchangeFlag.ToLower()
                Else
                    availability = 0
                    ticketingExchangeFlag = "false"
                    minTicketExchangePrice = "0"
                    maxTicketExchangePrice = "0"
                End If

                Exit For
            End If
        Next
    End Sub

    ''' <summary>
    ''' Get the price colour and text information information based on the given stand and area details
    ''' </summary>
    ''' <param name="standAndAreaId">The stand and area id</param>
    ''' <param name="priceColour">The price colour</param>
    ''' <param name="priceText">The price text</param>
    ''' <remarks></remarks>
    Private Sub getPriceColourAndText(ByVal standAndAreaId As String, ByRef priceColour As String, ByRef priceText As String)
        Dim areaCategory As String = String.Empty
        Dim standId() As String = standAndAreaId.Split(HYPHEN)
        Dim priceRow() As DataRow = Nothing
        Dim priceTextHold As String = String.Empty
        Dim priceColourHold As String = String.Empty
        Dim semiColon As String = ";"
        Dim isDefaultPriceBand As Boolean = False
        Dim iExit As Integer = 0
        For Each row As DataRow In _dtProductPriceDetails.Rows
            If row("StandCode") = standId(0).ToUpper() AndAlso row("AreaCode") = standId(1).ToUpper() AndAlso isPriceBandValid(row("PriceBand"), isDefaultPriceBand) Then
                priceText = TDataObjects.PaymentSettings.FormatCurrency(CDec(row("Price")), _ucr.BusinessUnit, _ucr.PartnerCode)
                priceText = HttpContext.Current.Server.HtmlDecode(priceText) 'Replace "&pound;" with "£"
                areaCategory = row("AreaCategory")
                priceRow = _dtStadiumPricingAreaColoursAndText.Select("AREA_CATEGORY = '" & areaCategory & "'")
                PricePrefixText = _ucr.Content("PriceTextPrefix", _languageCode, True)
                If Not String.IsNullOrEmpty(priceText) OrElse Not String.IsNullOrEmpty(priceColour) Then
                    If priceRow.Length > 0 Then
                        priceTextHold &= getPriceBandDescription(row("PriceBand")) & EMPTY_STRING & PricePrefixText & "@" & priceRow(0)("TEXT").ToString().Replace("<<PRICE_RANGE>>", priceText) & semiColon
                        priceColourHold = priceRow(0)("COLOUR").ToString()
                    Else
                        priceTextHold &= getPriceBandDescription(row("PriceBand")) & EMPTY_STRING & PricePrefixText & "@" & priceText & semiColon
                    End If
                End If
                iExit += 1
                If isDefaultPriceBand OrElse (iExit = _priceBands.Rows.Count) Then
                    Exit For
                End If

            End If
        Next
        priceColour = priceColourHold
        priceText = priceTextHold
    End Sub

    ''' <summary>
    ''' Format the description for the stand and area based on the given stand and area ID and the text passed in
    ''' </summary>
    ''' <param name="ID">The given stand and area ID</param>
    ''' <param name="priceText">The price text value</param>
    ''' <param name="availabilityText">The availability text value</param>
    ''' <returns>A formatted string</returns>
    ''' <remarks></remarks>
    Private Function getStandAndAreaDescription(ByVal ID As String, ByVal priceText As String, ByVal availabilityText As String) As String
        Dim standAndAreaDescription As New StringBuilder
        Dim semiColon As String = ";"
        standAndAreaDescription.Append(getStandDescription(ID))
        standAndAreaDescription.Append(semiColon).Append(getAreaDescription(ID))
        standAndAreaDescription.Append(semiColon).Append(availabilityText)
        standAndAreaDescription.Append(semiColon).Append(priceText)
        Return standAndAreaDescription.ToString()
    End Function

    ''' <summary>
    ''' Populate the availability key repeater with the availability bands
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateAvailabilityKey()
        If _dtStadiumAvailablityAreaColoursAndText.Rows.Count > 0 Then
            rptAvailabilityKey.DataSource = _dtStadiumAvailablityAreaColoursAndText
            rptAvailabilityKey.DataBind()
            ltlAreaAvailability.Text = _ucr.Content("AvailabilityHeaderLabel", _languageCode, True)
            plhStandAvailabilityKey.Visible = True
        Else
            plhStandAvailabilityKey.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Populate the pricing key repeater with the price categories
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populatePricingKey()
        If _dtStadiumPricingAreaColoursAndText.Rows.Count > 0 Then
            plhPricingKey.Visible = True
            rptPricingKey.DataSource = _dtStadiumPricingAreaColoursAndText
            rptPricingKey.DataBind()
            ltlAreaPricing.Text = _ucr.Content("PricingHeaderLabel", _languageCode, True)
        Else
            plhPricingKey.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Populate the seating key repeater with the types and colours
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateSeatingKey()
        If _dtStadiumSeatColoursAndText.Rows.Count > 0 Then
            Dim dvStadiumSeatColoursAndText As New DataView(_dtStadiumSeatColoursAndText)
            Dim rowFilterString As String = "1=1"
            If Not _pickingExceptionSeat AndAlso _currentExceptionSeat <> GlobalConstants.ST_EXCCEPTION_UNALLOCATED_SEAT Then
                rowFilterString &= " AND SEAT_TYPE <> 'EXCEPTION'"
            End If
            If Not _productIsEnabledForTicketExchange Then
                rowFilterString &= " AND SEAT_TYPE <> 'TX'"
            End If
            dvStadiumSeatColoursAndText.RowFilter = rowFilterString
            rptSeatingKey.DataSource = dvStadiumSeatColoursAndText
            rptSeatingKey.DataBind()
            ltlSeatingKey.Text = _ucr.Content("SeatingKeyHeaderLabel", _languageCode, True)
            plhSeatingKey.Visible = True
        Else
            plhSeatingKey.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Populate the user control text, control attributes and stadium defaults settings
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateTextAndDefaultsSettings()
        btnQuickBuy.Text = _ucr.Content("QuickBuyButton", _languageCode, True)
        ViewFromAreaButtonText = _ucr.Content("ViewFromAreaButton", _languageCode, True)
        ClearSelectionButtonText = _ucr.Content("ClearSelectionButton", _languageCode, True)
        ChangeStandViewButtonText = _ucr.Content("StandPriceButtonText", _languageCode, True)
        ltlLoginMessage.Text = _ucr.Content("LoginMessageText", _languageCode, True)
        LoginMessageTitle = _ucr.Content("LoginMessageTitle", _languageCode, True)
        CloseModalWindowText = _ucr.Content("CloseModalWindowText", _languageCode, True)
        ltlQuickBuyTitle.Text = _ucr.Content("QuickBuyTitle", _languageCode, True)
        ltlGameSelectionTitle.Text = _ucr.Content("GameSelectionTitle", _languageCode, True)
        lblGameSelection.Text = _ucr.Content("GameSelection", _languageCode, True)
        MultiSelectOnText = _ucr.Content("MultiSelectOnLabel", _languageCode, True)
        MultiSelectOffText = _ucr.Content("MultiSelectOffLabel", _languageCode, True)
        BackToStadiumViewText = _ucr.Content("BackToStadiumViewText", _languageCode, True)
        ClearQuickBuyText = _ucr.Content("ClearQuickBuy", _languageCode, True)
        StandPrefixText = _ucr.Content("StandTextPrefix", _languageCode, True)
        AreaPrefixText = _ucr.Content("AreaTextPrefix", _languageCode, True)
        RowPrefixText = _ucr.Content("RowTextPrefix", _languageCode, True)
        SeatPrefixText = _ucr.Content("SeatTextPrefix", _languageCode, True)
        SeatPricePrefixText = _ucr.Content("SeatPricePrefixText", _languageCode, True)
        AvailabilityPrefixText = _ucr.Content("AvailabilityTextPrefix", _languageCode, True)
        PricePrefixText = _ucr.Content("PriceTextPrefix", _languageCode, True)
        ResetButtonText = _ucr.Content("ResetButtonText", _languageCode, True)
        _rowSeatRowOnSVGOffText = _ucr.Content("RowSeatRowOnSVGOffText", _languageCode, True)
        _rowSeatRowOnSVGOnText = _ucr.Content("RowSeatRowOnSVGOnText", _languageCode, True)
        ItemsInBasketText = _ucr.Content("ItemsInBasketText", _languageCode, True)
        hdfRowText.Value = RowPrefixText
        hdfSeatText.Value = SeatPrefixText
        If _pickingExceptionSeat Then
            btnBuy.Text = _ucr.Content("ChangeExceptionSeatButton", _languageCode, True)
        ElseIf _pickingNewComponentSeat Then
            btnBuy.Text = _ucr.Content("ChangeComponentSeatButton", _languageCode, True)
        Else
            btnBuy.Text = _ucr.Content("BuyButton", _languageCode, True)
        End If

        plhPitchTop.Visible = False
        plhPitchBottom.Visible = False
        If _dtStadiums.Rows.Count > 0 Then
            Select Case TEBUtilities.CheckForDBNull_String(_dtStadiums.Rows(0)("PITCH_POSITION"))
                Case Is = "TOP"
                    plhPitchTop.Visible = True
                    ltlPitchTop.Text = _ucr.Content("PitchThisWayText", _languageCode, True)
                Case Is = "BOTTOM"
                    plhPitchBottom.Visible = True
                    ltlPitchBottom.Text = _ucr.Content("PitchThisWayText", _languageCode, True)
            End Select
            If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_dtStadiums.Rows(0).Item("TOGGLE_ROW_SEATS_ON_SVG")) Then
                RowSeatRowOnSVGText = _ucr.Content("RowSeatRowOnSVGOffText", _languageCode, True)
            Else
                RowSeatRowOnSVGText = _ucr.Content("RowSeatRowOnSVGOnText", _languageCode, True)
            End If
            plhChangeStandView.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_dtStadiums.Rows(0)("CHANGE_STANDVIEW_VISIBLE"))
            plhViewFromArea.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_dtStadiums.Rows(0)("VIEW_FROM_AREA_ENABLED"))
        End If

        plhSeatDetails.Visible = showCustomerDetails()
        If plhSeatDetails.Visible Then
            ltlSeatDeailsTitle.Text = _ucr.Content("SeatDetailsTitle", _languageCode, True)
            ltlRestrictionDetails.Text = _ucr.Content("SeatRestrictionDetailsTitle", _languageCode, True)
            ltlSeatDetailsCustomerNumber.Text = _ucr.Content("CustomerNumberLabel", _languageCode, True)
            ltlSeatDetailsCustomerForename.Text = _ucr.Content("CustomerForenameLabel", _languageCode, True)
            ltlSeatDetailsCustomerSurname.Text = _ucr.Content("CustomerSurnameLabel", _languageCode, True)
            ltlAddress1.Text = _ucr.Content("Address1Label", _languageCode, True)
            ltlAddress2.Text = _ucr.Content("Address2Label", _languageCode, True)
            ltlAddress3.Text = _ucr.Content("Address3Label", _languageCode, True)
            ltlSeasonTicket.Text = _ucr.Content("SeasonTicketLabel", _languageCode, True)
            ltlBookNumber.Text = _ucr.Content("BookNumberLabel", _languageCode, True)
            ltlRestrictionCode.Text = _ucr.Content("RestrictionCodeLabel", _languageCode, True)
            ltlTextDescRestriction.Text = _ucr.Content("RestrictionDetailsLabel", _languageCode, True)
            ltlRestrictionDescription.Text = _ucr.Content("RestrictionDescriptionLabel", _languageCode, True)
            ltlReservedDate.Text = _ucr.Content("ReservedDateLabelText", _languageCode, True)
            ltlReservedTime.Text = _ucr.Content("ReservedTimeLabelText", _languageCode, True)
            ltlCustomerReserationTime.Text = _ucr.Content("CustomerReservedDetailsLabel", _languageCode, True)
        End If
        plhBackToLinkedProducts.Visible = False
        If Not String.IsNullOrEmpty(_linkedMasterProduct) Then
            For Each item As TalentBasketItem In Profile.Basket.BasketItems
                If item.Product = _linkedMasterProduct Then
                    plhBackToLinkedProducts.Visible = True
                    hplLinkedProductBack.Text = _ucr.Content("BackToLinkedProducts", _languageCode, True)
                    hplLinkedProductBack.NavigateUrl = "~/PagesPublic/ProductBrowse/LinkedProducts.aspx?pricecode=" & item.PRICE_CODE & "&productsubtype=" & item.PRODUCT_SUB_TYPE
                    Exit For
                End If
            Next
        End If
    End Sub

    ''' <summary>
    ''' Check the agent authority for adding home and season items to the basket
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub checkAgentAuthority(ByVal productType As String)
        If AgentProfile.IsAgent And Not IsCatMode Then
            If (Not AgentProfile.AgentPermissions.CanAddHomeGameToBasket AndAlso productType = GlobalConstants.HOMEPRODUCTTYPE) OrElse
                (Not AgentProfile.AgentPermissions.CanAddSeasonGameToBasket AndAlso productType = GlobalConstants.SEASONTICKETPRODUCTTYPE) Then
                Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
                Session("UnavailableReturnPage") = String.Empty
                Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
            End If
        End If
    End Sub

    ''' <summary>
    ''' Populate the game selection drop down list, only list games that have an associated stadium graphic, 
    ''' some stadium codes in module defaults may not have a proper stadium graphic
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateGameSelectionList()
        Dim productTypesToShow As String = _ucr.Attribute("ProductTypesToShow")
        If productTypesToShow.Length = 0 Then productTypesToShow = "'H'"

        Dim err As New ErrorObj
        Dim talProduct As New TalentProduct
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        settings.Cacheing = True
        If productTypesToShow = "'H'" Then
            talProduct.De.ProductType = "H"
        End If
        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowOnlyMatchingSubTypeInGameSelection")) AndAlso ProductSubType.Length > 0 Then
            talProduct.De.ProductSubtype = ProductSubType
        End If
        talProduct.De.PriceAndAreaSelection = False
        talProduct.De.Src = GlobalConstants.SOURCE
        talProduct.Settings = settings
        talProduct.De.StadiumCode = ModuleDefaults.TicketingStadium
        err = talProduct.ProductList()

        If Not err.HasError AndAlso talProduct.ResultDataSet.Tables.Count > 0 Then
            Dim dvProductList As New DataView
            dvProductList.Table = talProduct.ResultDataSet.Tables(1)
            _dtCorporateHospitalityDetails = talProduct.ResultDataSet.Tables(1)

            Dim filter As New StringBuilder
            If productTypesToShow <> "'H'" Then
                filter.Append("(ProductType IN (" & productTypesToShow & ") AND ExcludeProductFromWebSales='False')")
            End If
            If AgentProfile.IsAgent Then
                If Not AgentProfile.AgentPermissions.CanAddHomeGameToBasket Then
                    If filter.ToString.Trim = String.Empty Then
                        filter.Append("(ProductType <> 'H')")
                    Else
                        filter.Append(" AND (ProductType <> 'H')")
                    End If
                End If
                If Not AgentProfile.AgentPermissions.CanAddSeasonGameToBasket Then
                    If filter.ToString.Trim = String.Empty Then
                        filter.Append("(ProductType <> 'S')")
                    Else
                        filter.Append(" AND (ProductType <> 'S')")
                    End If
                End If
            End If

            dvProductList.RowFilter = filter.ToString
            dvProductList.Sort = "ProductMDTE08"
            _dtProductList = dvProductList.ToTable()
            If Not IsPostBack Then
                ddlGameSelection.DataSource = _dtProductList
                ddlGameSelection.DataValueField = "ProductCode"
                ddlGameSelection.DataTextField = "ProductDescription"
                ddlGameSelection.DataBind()
                ddlGameSelection.SelectedValue = ProductCode
            End If
            plhGameSelection.Visible = (ddlGameSelection.Items.Count > 1)
            loadAdditionalInformation()
        Else
            handleErrors("SS-13", "Error retrieving the list of products for the game selection list.")
        End If
    End Sub

    ''' <summary>
    ''' Set the facebook 'Like' icon based on the current product
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setFacebookLike()
        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("FacebookLikeOn")) Then
            Dim fbProductImage As String = ImagePath.getImagePath("APPTHEME", _ucr.Attribute("FacebookLikeThumbnail"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
            Dim fbHtmlTag As String = TEBUtilities.CheckForDBNull_String(_ucr.Attribute("FacebookLikeHtmlTag"))
            Dim fbProductURL As New StringBuilder
            InformationTextCSSClassname = "large-10"
            plhFacebookLike.Visible = True
            fbProductURL.Append(Request.Url.GetLeftPart(UriPartial.Path))
            fbProductURL.Append("?stadium=").Append(StadiumCode)
            fbProductURL.Append("&ProductCode=").Append(ProductCode)
            fbProductURL.Append("&ProductType=").Append(ProductType)
            If ProductSubType.Length > 0 Then
                fbProductURL.Append("&ProductSubType=").Append(ProductSubType)
            End If
            fbProductURL.Append("&productIsHomeAsAway=").Append(ProductIsHomeAsAway)
            fbHtmlTag = fbHtmlTag.Replace("[FBLIKEURL]", fbProductURL.ToString())
            ltlFacebookLike.Text = fbHtmlTag

            If Not String.IsNullOrWhiteSpace(lblInformationText.Text) Then AddFaceBookLikeMetaTag("og:title", lblInformationText.Text)
            If Not String.IsNullOrWhiteSpace(ltlCompetition.Text) Then AddFaceBookLikeMetaTag("og:description", ltlCompetition.Text)
            If Not String.IsNullOrWhiteSpace(fbProductImage) Then AddFaceBookLikeMetaTag("og:image", fbProductImage)
            AddFaceBookLikeMetaTag("og:url", fbProductURL.ToString())
            AddFaceBookLikeMetaTag("og:type", TEBUtilities.CheckForDBNull_String(_ucr.Attribute("FacebookLikeType")))
            AddFaceBookLikeMetaTag("og:site_name", TEBUtilities.CheckForDBNull_String(_ucr.Attribute("FacebookLikeSiteName")))
            AddFaceBookLikeMetaTag("fb:admins", TEBUtilities.CheckForDBNull_String(_ucr.Attribute("FacebookLikeAdminID")))
        Else
            InformationTextCSSClassname = "large-12"
            plhFacebookLike.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Populate the corporate hospitality product details and redirect to HospitalityFixturePackages.aspx for a linked corporate product
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateCorporateHospitalityDetails()
        If _dtCorporateHospitalityDetails.Rows.Count > 0 And String.Compare(StadiumCode, ModuleDefaults.CorporateStadium) Then
            Dim productTypesToShow As String = _ucr.Attribute("ProductTypesToShow")
            Dim hospitalityNavigateUrl As New StringBuilder
            If productTypesToShow.Length = 0 Then productTypesToShow = "'H'"
            Dim dvLinkedCorporateProductList As New DataView
            dvLinkedCorporateProductList.Table = _dtCorporateHospitalityDetails

            Dim filter As New StringBuilder
            If productTypesToShow <> "'H'" Then
                filter.Append("(ProductType IN (" & productTypesToShow & ") AND ExcludeProductFromWebSales='False' AND ProductCode = '" & ProductCode & "') ")
            End If
            dvLinkedCorporateProductList.RowFilter = filter.ToString
            dvLinkedCorporateProductList.Sort = "ProductMDTE08"
            _dtCorporateHospitalityDetails = dvLinkedCorporateProductList.ToTable()

            Dim soldOut As Boolean = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(dvLinkedCorporateProductList.ToTable.Rows(0)("IsSoldOut"))
            Dim productAvailability As String = dvLinkedCorporateProductList.ToTable.Rows(0)("ProductAvailForSale").ToString().ToUpper()
            Dim productLinked As String = dvLinkedCorporateProductList.ToTable.Rows(0)("ProductLinkedToHospitality").ToString().ToUpper()
            If (productAvailability <> "N") Then
                hospitalityNavigateUrl.Append("~/PagesPublic/Hospitality/HospitalityFixturePackages.aspx?product=")
                hospitalityNavigateUrl.Append(dvLinkedCorporateProductList.ToTable.Rows(0)("CorporateHospitalityProductCode"))
                If soldOut Then
                    ' Display upgrade to hospitality link if configured and product is solout.
                    If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowUpgradeToHospitalityOnSVGWhenProductSoldOut")) AndAlso Not String.IsNullOrEmpty(dvLinkedCorporateProductList.ToTable.Rows(0)("CorporateHospitalityProductCode")) AndAlso (productLinked <> "N") Then
                        plhUpgradeToHospitality.Visible = True
                        ltlUpgradeToHospitality.Text = _ucr.Content("upgradeToHospitalityText", _languageCode, True)
                        ltlUpgradeToHospitalityDesc.Text = _ucr.Content("upgradeToHospitalityDescText", _languageCode, True)
                        hplUpgradeToHospitality.Text = _ucr.Content("UpgradeToHospitalityLinkTextForSeatSelection", _languageCode, True)
                        hplUpgradeToHospitality.NavigateUrl = hospitalityNavigateUrl.ToString()
                    Else
                        plhUpgradeToHospitality.Visible = False
                    End If
                Else
                    ' Display upgrade to hospitality link if configured and product not solout
                    If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowUpgradeToHospitalityOnSeatSelectionPage")) AndAlso Not String.IsNullOrEmpty(dvLinkedCorporateProductList.ToTable.Rows(0)("CorporateHospitalityProductCode")) AndAlso (productLinked <> "N") Then
                        plhUpgradeToHospitality.Visible = True
                        ltlUpgradeToHospitality.Text = _ucr.Content("upgradeToHospitalityText", _languageCode, True)
                        ltlUpgradeToHospitalityDesc.Text = _ucr.Content("upgradeToHospitalityDescText", _languageCode, True)
                        hplUpgradeToHospitality.Text = _ucr.Content("UpgradeToHospitalityLinkTextForSeatSelection", _languageCode, True)
                        hplUpgradeToHospitality.NavigateUrl = hospitalityNavigateUrl.ToString()
                    Else
                        plhUpgradeToHospitality.Visible = False
                    End If
                End If
            End If
        Else
            plhUpgradeToHospitality.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Setup the stand and area selection usercontrol
    ''' </summary>
    ''' <param name="dtProductDetails">The product details that we are currently using</param>
    ''' <remarks></remarks>
    Private Sub setStandAndAreaSelection(ByRef dtProductDetails As DataTable)
        uscStandAndAreaSelection.AlternativeSeatSelection = dtProductDetails.Rows(0)("AlternativeSeatSelection")
        uscStandAndAreaSelection.AlternativeSeatSelectionAcrossStands = dtProductDetails.Rows(0)("AlternativeSeatSelectionAcrossStands")
        uscStandAndAreaSelection.ProductCode = ProductCode
        uscStandAndAreaSelection.ProductStadium = StadiumCode
        uscStandAndAreaSelection.ProductType = ProductType
        uscStandAndAreaSelection.ProductSubType = ProductSubType
        uscStandAndAreaSelection.CampaignCode = CampaignCode
        uscStandAndAreaSelection.ProductHomeAsAway = ProductIsHomeAsAway
        uscStandAndAreaSelection.ProductPriceBand = TEBUtilities.CheckForDBNull_String(dtProductDetails.Rows(0)("DefaultPriceBand"))
        uscStandAndAreaSelection.PriceBreakId = _selectedPriceBreakId
        uscStandAndAreaSelection.HideSelectSeatsButton = True
        uscStandAndAreaSelection.ProductDescription = lblInformationText.Text
    End Sub

    ''' <summary>
    ''' Setup the favourite seat selection usercontrol
    ''' </summary>
    ''' <param name="dtProductDetails">The product details that we are currently using</param>
    ''' <remarks></remarks>
    Private Sub setFavouriteSeatSelection(ByRef dtProductDetails As DataTable)
        uscFavouriteSeatSelection.ProductCode = ProductCode
        uscFavouriteSeatSelection.ProductStadium = StadiumCode
        uscFavouriteSeatSelection.ProductType = ProductType
        uscFavouriteSeatSelection.ProductSubType = ProductSubType
        uscFavouriteSeatSelection.CampaignCode = CampaignCode
        uscFavouriteSeatSelection.CanStadiumUseFavouriteSeat = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(dtProductDetails.Rows(0)("CanStadiumUseFavouriteSeat"))
        uscFavouriteSeatSelection.ProductPriceBand = TEBUtilities.CheckForDBNull_String(dtProductDetails.Rows(0)("DefaultPriceBand"))
        uscFavouriteSeatSelection.ProductDescription = lblInformationText.Text
    End Sub

    ''' <summary>
    ''' Send server side variables up to javascript to be used by the browser
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setJavascriptDefaults()
        Dim jScript As New StringBuilder
        Const SEMICOLON As String = ";"
        Const SPEECHMARKS As String = """"
        jScript.Append("var isSeatSelectionOnly = ").Append(IsSeatSelectionOnly.ToString().ToLower()).AppendLine(SEMICOLON)
        jScript.Append("var defaultToSeatSelection = ").Append(DefaultToSeatSelection.ToString().ToLower()).AppendLine(SEMICOLON)
        jScript.Append("var defaultToQuickBuy = ").Append(DefaultToQuickBuy.ToString().ToLower()).AppendLine(SEMICOLON)
        jScript.Append("var defaultStandAndAreaClick = ").Append(_defaultStandAndAreaClick.ToString().ToLower()).AppendLine(SEMICOLON)
        jScript.Append("var isAnonymous = ").Append(Profile.IsAnonymous.ToString().ToLower()).AppendLine(SEMICOLON)
        jScript.Append("var singleStand = """).Append(Request.QueryString("stand")).Append(SPEECHMARKS).Append(SEMICOLON)
        jScript.Append("var singleArea = """).Append(Request.QueryString("area")).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var showCustomerDetails = ").Append(CStr(showCustomerDetails()).ToLower()).AppendLine(SEMICOLON)
        jScript.Append("var isAgent = ").Append(CStr(AgentProfile.IsAgent).ToLower()).AppendLine(SEMICOLON)
        jScript.Append("var loggedInCustomerPriceBand = """).Append(_loggedInCustomerPriceBand).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var allowPriceBandSelection = """).Append(_productAllowPricebandSelection).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var defaultPriceBand = """).Append(_defaultPriceBand).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var defaultPriceBandDescription = """).Append(getPriceBandDescription(_defaultPriceBand)).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var isBulkMode = ").Append(CStr(AgentProfile.BulkSalesMode).ToLower()).AppendLine(SEMICOLON)
        jScript.Append("var loginLink = """).Append(ResolveUrl("~/PagesPublic/Login/Login.aspx")).Append("?ReturnUrl=").Append(Server.UrlEncode(Request.RawUrl)).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var standAvailabilityText = """).Append(_ucr.Content("StandAvailabilityButtonText", _languageCode, True)).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var standPriceText = """).Append(_ucr.Content("StandPriceButtonText", _languageCode, True)).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var orphanedSeatErrorText = """).Append(_ucr.Content("OrphanedSeatErrorText", _languageCode, True)).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var isCATMode = ").Append(CStr(IsCatMode.ToString().ToLower())).AppendLine(SEMICOLON)
        jScript.Append("var CATModeErrorText = """).Append(_ucr.Content("CATModeErrorText", _languageCode, True)).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var orphanSeatFlag = ").Append(isOrphanSeat()).AppendLine(SEMICOLON)
        jScript.Append("var isSeasonTicket = ").Append((ProductType = GlobalConstants.SEASONTICKETPRODUCTTYPE AndAlso Not _isProductBundle).ToString().ToLower()).AppendLine(SEMICOLON)
        jScript.Append("var multiSelectOnText = """).Append(MultiSelectOnText).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var multiSelectOffText = """).Append(MultiSelectOffText).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var rowSeatRowOnSVGOffText = """).Append(_rowSeatRowOnSVGOffText).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var rowSeatRowOnSVGOnText = """).Append(_rowSeatRowOnSVGOnText).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var productCode = """).Append(ProductCode).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var stadiumCode = """).Append(StadiumCode).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var callId = """).Append(CallId).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        If _currentExceptionSeat = GlobalConstants.ST_EXCCEPTION_UNALLOCATED_SEAT Then
            jScript.Append("var currentExceptionSeat = """).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        Else
            jScript.Append("var currentExceptionSeat = """).Append(_currentExceptionSeat).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        End If
        jScript.Append("var campaignCode = """).Append(CampaignCode).Append(SPEECHMARKS).AppendLine(SEMICOLON)

        Dim minQuantity As Integer = 0
        Dim maxQuantity As Integer = 0
        Dim multiSelectMax As Integer = 0
        Dim exceededBasketLimit As String = _ucr.Content("ExceededBasketLimit", _languageCode, True)
        Dim basketRangeErrorText As String = _ucr.Content("BasketRangeErrorText", _languageCode, True)
        Dim tGatewayFunctions As New TicketingGatewayFunctions
        Dim defaultQuantity As String = String.Empty
        Dim isReadOnly As Boolean = False
        Dim isSingleSeatRestriction As Boolean = (_pickingExceptionSeat OrElse (_pickingNewComponentSeat And Not _changeAllSeats))
        tGatewayFunctions.GetQuantityDefintions(ProductCode, minQuantity, maxQuantity, defaultQuantity, isReadOnly, True, isSingleSeatRestriction, _dtComponents, _dtComponentSeats, _changeAllSeats)
        exceededBasketLimit = exceededBasketLimit.Replace("*", maxQuantity)
        hdfQuickBuyQuantity.Value = maxQuantity
        jScript.Append("var basketMin =").Append(minQuantity).AppendLine(SEMICOLON)
        jScript.Append("var basketMax = ").Append(maxQuantity).AppendLine(SEMICOLON)
        jScript.Append("var basketMaxQuickBuy = ").Append(maxQuantity).AppendLine(SEMICOLON)
        jScript.Append("var ExceededBasketLimitText = """).Append(exceededBasketLimit).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        If AgentProfile.IsAgent Then
            basketRangeErrorText = basketRangeErrorText.Replace("minQuantity", minQuantity)
            basketRangeErrorText = basketRangeErrorText.Replace("maxQuantity", maxQuantity)
            jScript.Append("var BasketRangeErrorText = """).Append(basketRangeErrorText).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        Else
            basketRangeErrorText = basketRangeErrorText.Replace("minQuantity-maxQuantity", minQuantity)
            jScript.Append("var BasketRangeErrorText = """).Append(basketRangeErrorText).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        End If
        jScript.Append("var itemsInBasketText = """).Append(_ucr.Content("ItemsInBasketText", _languageCode, True)).Append(SPEECHMARKS).AppendLine(SEMICOLON)
        jScript.Append("var noSeatsSelected = """).Append(_ucr.Content("NoSeatsSelectedMessage", _languageCode, True)).Append(SPEECHMARKS).AppendLine(SEMICOLON)

        If _dtStadiums.Rows.Count = 0 Then
            jScript.Append("var isQuickBuy = ").Append(CStr(False).ToLower()).AppendLine(SEMICOLON)
            jScript.Append("var isStadiumExpaned = ").Append(CStr(False).ToLower()).AppendLine(SEMICOLON)
            jScript.Append("var stadiumCanvasWidth = ").Append(TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("DefaultSeatingAreaCanvasWidth"))).AppendLine(SEMICOLON)
            jScript.Append("var stadiumCanvasHeight = ").Append(TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("DefaultSeatingAreaCanvasHeight"))).AppendLine(SEMICOLON)
            jScript.Append("var stadiumCanvasWidthResized = ").Append(TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("DefaultMiniSeatingAreaCanvasWidth"))).AppendLine(SEMICOLON)
            jScript.Append("var stadiumCanvasHeightResized = ").Append(TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("DefaultMiniSeatingAreaCanvasHeight"))).AppendLine(SEMICOLON)
            jScript.Append("var zoomOnLoad = ").Append(CStr(TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("DefaultZoomOnLoad"))).ToLower()).Append(SEMICOLON)
            jScript.Append("var seatingAreaCanvasWidth = ").Append(TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("DefaultSeatingAreaCanvasWidth"))).AppendLine(SEMICOLON)
            jScript.Append("var seatingAreaCanvasHeight = ").Append(TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("DefaultSeatingAreaCanvasHeight"))).AppendLine(SEMICOLON)
            jScript.Append("var viewFromAreaUrlPrefix = """).Append(SPEECHMARKS).AppendLine(SEMICOLON)
            jScript.Append("var isRowSeatOnSVG = ").Append("false").AppendLine(SEMICOLON)
        Else
            Dim isQuickBuy As Boolean = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_dtStadiums.Rows(0).Item("QUICK_BUY"))
            If Not isQuickBuy Then isQuickBuy = Not _isProductSeatSelectionEnabled
            jScript.Append("var isQuickBuy = ").Append(CStr(isQuickBuyEnabled(isQuickBuy)).ToLower()).AppendLine(SEMICOLON)
            jScript.Append("var stadiumCanvasWidth = ").Append(_dtStadiums.Rows(0).Item("STADIUM_WIDTH")).AppendLine(SEMICOLON)
            jScript.Append("var stadiumCanvasHeight = ").Append(_dtStadiums.Rows(0).Item("STADIUM_HEIGHT")).AppendLine(SEMICOLON)
            jScript.Append("var zoomOnLoad = ").Append(CStr(_dtStadiums.Rows(0).Item("ZOOM_ON_LOAD")).ToLower()).Append(SEMICOLON)
            jScript.Append("var stadiumCanvasWidthResized = ").Append(_dtStadiums.Rows(0).Item("STADIUM_WIDTH_RESIZED")).AppendLine(SEMICOLON)
            jScript.Append("var stadiumCanvasHeightResized = ").Append(_dtStadiums.Rows(0).Item("STADIUM_HEIGHT_RESIZED")).AppendLine(SEMICOLON)
            jScript.Append("var seatingAreaCanvasWidth = ").Append(_dtStadiums.Rows(0).Item("SEATING_AREA_WIDTH")).AppendLine(SEMICOLON)
            jScript.Append("var seatingAreaCanvasHeight = ").Append(_dtStadiums.Rows(0).Item("SEATING_AREA_HEIGHT")).AppendLine(SEMICOLON)
            jScript.Append("var viewFromAreaUrlPrefix = """).Append(_ucr.Attribute("ImagePrefixLocation")).Append(_dtStadiums.Rows(0).Item("STADIUM_NAME")).Append("/viewFromArea/").Append(SPEECHMARKS).AppendLine(SEMICOLON)
            jScript.Append("var orphanBenchmark = ").Append(_dtStadiums.Rows(0).Item("ORPHAN_SEAT_BENCHMARK")).AppendLine(SEMICOLON)
            If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_dtStadiums.Rows(0).Item("TOGGLE_ROW_SEATS_ON_SVG")) And AgentProfile.IsAgent Then
                jScript.Append("var isRowSeatOnSVG = ").Append("true").AppendLine(SEMICOLON)
            Else
                jScript.Append("var isRowSeatOnSVG = ").Append("false").AppendLine(SEMICOLON)
            End If
            jScript.Append("var use3DSeatView = ").Append(CStr(is3DSeatViewEnabled()).ToLower()).AppendLine(SEMICOLON)
            If Ticketing3DSeatView Then
                jScript.Append("var tk3d = ").Append("new Ticketing3D('ticketing3DSeatView', '").Append(_dtStadiums.Rows(0).Item("3D_SEAT_VIEW_VENUE_ID")).Append("')").AppendLine(SEMICOLON)
            End If
        End If

        ScriptManager.RegisterClientScriptBlock(Me.Page, Me.GetType, "seat-selection", jScript.ToString(), True)
    End Sub

    ''' <summary>
    ''' Check to see if the orphan seat is enabled, based on agent mode and orphaned seat flag.
    ''' The orphaned seat flag exists in tbl_stadiums when there is a graphical stadium and tbl_control_attribute when there isn't a graphical stadium
    ''' </summary>
    ''' <returns>True if orphan seat is enabled.</returns>
    ''' <remarks></remarks>
    Private Function isOrphanSeat() As String
        Dim orphanSeatFlag As Boolean = False
        If Not AgentProfile.IsAgent Then
            If _dtStadiums.Rows.Count > 0 Then
                orphanSeatFlag = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_dtStadiums.Rows(0).Item("ORPHAN_SEAT_VALIDATION"))
            Else
                orphanSeatFlag = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("DefaultOrphanedSeatFlag"))
            End If
        End If
        Return orphanSeatFlag.ToString().ToLower()
    End Function

    ''' <summary>
    ''' Check to see if the quick buy option is enabled based on agent mode
    ''' </summary>
    ''' <param name="quickBuySetting">The quick buy setting</param>
    ''' <returns>Set the value if it is enabled</returns>
    ''' <remarks></remarks>
    Private Function isQuickBuyEnabled(ByVal quickBuySetting As Boolean) As Boolean
        Dim quickBuyFlag As Boolean = False
        If Not AgentProfile.IsAgent Then
            quickBuyFlag = quickBuySetting
        End If
        Return quickBuyFlag
    End Function

    ''' <summary>
    ''' Is 3D Seat view option enabled against the stadiums table for the current stadium
    ''' Set the public variable for use in the VisualSeatSelection.aspx page
    ''' </summary>
    ''' <returns>True if the option is enabled</returns>
    ''' <remarks></remarks>
    Private Function is3DSeatViewEnabled() As Boolean
        Dim seatViewEnabled As Boolean = False
        If Not IsSeatSelectionOnly AndAlso _dtStadiums.Columns.Contains("TOGGLE_3D_SEAT_VIEW") AndAlso TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_dtStadiums.Rows(0).Item("TOGGLE_3D_SEAT_VIEW")) Then
            seatViewEnabled = True
            Ticketing3DSeatView = True
        End If
        Return seatViewEnabled
    End Function

    ''' <summary>
    ''' Check to see if the customer details by clicking the seat is in use
    ''' </summary>
    ''' <returns>True if the function is in use</returns>
    ''' <remarks></remarks>
    Private Function showCustomerDetails() As Boolean
        If AgentProfile.IsAgent AndAlso TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("CustomerDetailsEnabled")) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Load the addtional information option to the product if it is available, this changes the css class names of the game selection div tag
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub loadAdditionalInformation()
        Dim IncludeFolderName As String = "Other"
        Dim HTMLFound As Boolean = False
        If TEBUtilities.DoesHtmlFileExists(_ucr.BusinessUnit & "\" & _ucr.PartnerCode & "\Product\" & IncludeFolderName & "\" & ProductCode & ".htm") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_ucr.BusinessUnit & "\" & _ucr.PartnerCode & "\Product\" & IncludeFolderName & "\" & ProductCode & ".html") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_ucr.BusinessUnit & "\" & _ucr.PartnerCode & "\Product\" & IncludeFolderName & "\" & ProductSubType & ".htm") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_ucr.BusinessUnit & "\" & _ucr.PartnerCode & "\Product\" & IncludeFolderName & "\" & ProductSubType & ".html") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_ucr.BusinessUnit & "\" & _ucr.PartnerCode & "\Product\" & IncludeFolderName & "\" & ProductType & ".htm") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_ucr.BusinessUnit & "\" & _ucr.PartnerCode & "\Product\" & IncludeFolderName & "\" & ProductType & ".html") Then
            HTMLFound = True
        End If

        If HTMLFound Then
            plhAdditionalInformation.Visible = True
            hlkAdditionalInformation.NavigateUrl = "~/PagesPublic/ProductBrowse/ProductSummary.aspx?ProductCode=" & ProductCode & "&ProductType=" & ProductType & "&ProductSubType=" & ProductSubType & "&IncludeFolderName=" & IncludeFolderName
            ltlMoreInfoText.Text = _ucr.Content("MoreInformationText", _languageCode, True)
        Else
            Dim pdfFound As Boolean = False
            Dim pdfLink As String = TEBUtilities.PDFLinkAvailable(ProductCode, ProductType, ProductSubType, _ucr.BusinessUnit, _ucr.PartnerCode)
            If Not String.IsNullOrEmpty(pdfLink) Then
                plhAdditionalInformation.Visible = True
                hlkAdditionalInformation.Target = "_blank"
                hlkAdditionalInformation.NavigateUrl = pdfLink
                ltlMoreInfoText.Text = _ucr.Content("MoreInformationText", _languageCode, True)
            Else
                plhAdditionalInformation.Visible = False
            End If
        End If
    End Sub

    ''' <summary>
    ''' Populate the javascript basket
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateJavaScriptBasket()
        Dim dtPackageSeats As New DataTable
        Dim err As ErrorObj = CATHelper.GetPackageComponentSeats(dtPackageSeats, _packageId, ProductCode)
        If CATHelper.IsBasketInTransferMode() Then
            If Not err.HasError AndAlso dtPackageSeats IsNot Nothing Then
                If dtPackageSeats.Rows.Count > 0 Then
                    Dim i As Integer = 0
                    For Each transferSeat As DataRow In dtPackageSeats.Rows
                        i += 1
                        If i < dtPackageSeats.Rows.Count Then
                            hdfTransferableBasketItems.Value &= transferSeat("Stand").ToString().ToUpper() & "-" & transferSeat("Area").ToString().ToUpper() & "/" & transferSeat("Row") & "/" & transferSeat("Seat") & ">"
                        Else
                            hdfTransferableBasketItems.Value &= transferSeat("Stand").ToString().ToUpper() & "-" & transferSeat("Area").ToString().ToUpper() & "/" & transferSeat("Row") & "/" & transferSeat("Seat")
                        End If
                    Next
                    'set hdfTransferableBasketItems.Value based on results
                End If
            End If
        Else
            If _changeAllSeats Then
                Dim package As New TalentPackage
                Dim seat As New DESeatDetails
                package.Settings() = TEBUtilities.GetSettingsObject()
                package.DePackages.PackageID = _packageId
                package.DePackages.CallId = _CallId
                package.DePackages.ProductCode = _ProductCode
                package.DePackages.BasketId = Profile.Basket.Basket_Header_ID
                err = package.GetCustomerPackageInformation()
                If Not err.HasError AndAlso package.ResultDataSet IsNot Nothing Then
                    If package.ResultDataSet.Tables("Seat").Rows.Count > 0 Then
                        For Each row As DataRow In package.ResultDataSet.Tables("Seat").Rows
                            If row("ComponentID").ToString() = _componentId Then
                                seat.UnFormattedSeat = row("SeatDetails").ToString()
                                If hdfTransferableBasketItems.Value <> "" Then
                                    hdfTransferableBasketItems.Value &= ">"
                                End If
                                hdfTransferableBasketItems.Value &= seat.Stand.Trim().ToUpper() & "-" & seat.Area.Trim().ToUpper() & "/" & seat.Row.Trim().ToUpper() & "/" & seat.Seat.Trim().ToUpper()
                                _dBasketSeats.Add(seat.FormattedSeat, _componentId)
                            End If
                        Next
                    End If
                    If package.ResultDataSet.Tables("Component").Rows.Count > 0 Then
                        Dim dvComponents As New DataView(package.ResultDataSet.Tables("Component"))
                        Dim rowFilterString As New StringBuilder
                        rowFilterString.Append("ComponentID = ").Append(_componentId)
                        dvComponents.RowFilter = rowFilterString.ToString()
                        _dtComponents = dvComponents.ToTable()
                    End If
                    If package.ResultDataSet.Tables("Seat").Rows.Count > 0 Then
                        Dim dvSeats As New DataView(package.ResultDataSet.Tables("Seat"))
                        Dim rowFilterString As New StringBuilder
                        rowFilterString.Append("ComponentID = ").Append(_componentId)
                        dvSeats.RowFilter = rowFilterString.ToString()
                        _dtComponentSeats = dvSeats.ToTable()
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Is the price band selected valid?
    ''' </summary>
    ''' <param name="acceptedPriceBand">The given price band</param>
    ''' <param name="isDefaultPriceBand">Is the price band a default price band</param>
    ''' <returns>True if the price band is valid</returns>
    ''' <remarks></remarks>
    Private Function isPriceBandValid(ByVal acceptedPriceBand As String, ByRef isDefaultPriceBand As Boolean) As Boolean
        If _priceBands.Rows.Count = 0 Then
            If acceptedPriceBand = _defaultPriceBand Then
                isDefaultPriceBand = True
                Return True
            End If
        End If
        For Each priceBand In _priceBands.Rows
            If acceptedPriceBand = priceBand("PriceBand") Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Get the price band description based on the given price band code
    ''' </summary>
    ''' <param name="priceband">The price band</param>
    ''' <returns>The price band description</returns>
    ''' <remarks></remarks>
    Private Function getPriceBandDescription(ByVal priceband As String) As String
        For Each priceBandDescription In _priceBandsDescription.Rows
            If priceBandDescription("PriceBand") = priceband Then
                Return priceBandDescription("PriceBandDescription")
            End If
        Next
        Return String.Empty
    End Function

    ''' <summary>
    ''' Handle any errors and perform a redirect based on the property values that are present
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub handleErrors(ByVal errCode As String, ByVal errMessage As String, Optional doErrorHandlingRedirect As Boolean = True)
        _hasError = True
        If _product.De.ProductCode Is Nothing Then
            _log.GeneralLog("SeatSelectionError", errCode, errMessage, "SeatSelectionLog")
        Else
            _log.GeneralLog("SeatSelectionError", errCode, "Product Code: " & _product.De.ProductCode & " | " & errMessage, "SeatSelectionLog")
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

    Private Function isproductEnabledForTicketExchange(ByRef productDetails As DataTable) As Boolean
        If productDetails.Rows.Count > 0 Then
            If TalentDefaults.TicketExchangeEnabled AndAlso TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(productDetails.Rows(0)("TicketExchangeEnabled")) = True AndAlso Not AgentProfile.BulkSalesMode Then
                Return True
            End If
        End If
        Return False
    End Function

    ''' <summary>
    ''' get stand/area for seatselection page
    ''' </summary>
    Private Sub getComponentStandAndAreaList()
        If _changeAllSeats Then
            Dim dtStadiumAvailablity As New DataTable
            Dim formattedStandAreaDescription As String = String.Empty
            Dim dtFilteredStandAndArea As DataTable = New DataTable
            Dim currentStandAreaCode As String = Stand.Trim & "-" & Area.Trim
            Dim dRow As DataRow = Nothing
            lblChangeStandAndArea.Text = _ucr.Content("ChangeStandAndAreaText", _languageCode, True)
            Dim standAreaDescriptionMask As String = _ucr.Content("StandAreaDescriptionMask", _languageCode, True)

            getProductAvailability(_componentId)
            dtStadiumAvailablity = _product.ResultDataSet.Tables("StadiumAvailability")

            Dim dtStandAndArea As DataTable = New DataTable
            dtStandAndArea.TableName = "CombinedStandAndArea"
            With dtStandAndArea.Columns
                .Add("StandCode", GetType(String))
                .Add("AreaCode", GetType(String))
                .Add("StandAreaDescription", GetType(String))
                .Add("StandAreaCode", GetType(String))
            End With
            Dim dvStandAndArea As New DataView(_dtStandAreaDescriptions)
            If dtStadiumAvailablity IsNot Nothing AndAlso dtStadiumAvailablity.Rows.Count > 0 Then
                For Each row As DataRow In dtStadiumAvailablity.Rows
                    dRow = dtStandAndArea.NewRow
                    Dim filterString As String
                    filterString = "StandCode='" & row("StandCode") & "'"
                    filterString = filterString & " AND AreaCode='" & row("AreaCode") & "'"
                    dvStandAndArea.RowFilter = filterString
                    dtFilteredStandAndArea = dvStandAndArea.ToTable
                    formattedStandAreaDescription = standAreaDescriptionMask
                    formattedStandAreaDescription = formattedStandAreaDescription.Replace("<<STAND_DESCRIPTION>>", dtFilteredStandAndArea.Rows(0).Item("StandDescription"))
                    formattedStandAreaDescription = formattedStandAreaDescription.Replace("<<AREA_DESCRIPTION>>", dtFilteredStandAndArea.Rows(0).Item("AreaDescription"))

                    dRow("StandCode") = dtFilteredStandAndArea.Rows(0).Item("StandCode").ToString
                    dRow("AreaCode") = dtFilteredStandAndArea.Rows(0).Item("AreaCode").ToString
                    dRow("StandAreaCode") = dRow("StandCode").ToString & "-" & dRow("AreaCode").ToString
                    dRow("StandAreaDescription") = formattedStandAreaDescription
                    dtStandAndArea.Rows.Add(dRow)
                Next
            End If
            Dim foundRow() As DataRow
            foundRow = dtStandAndArea.Select("StandAreaCode ='" & currentStandAreaCode & "'")
            If foundRow.Count = 0 Then
                dRow = dtStandAndArea.NewRow
                dRow("StandCode") = Stand
                dRow("AreaCode") = Area
                dRow("StandAreaCode") = dRow("StandCode").Trim & "-" & dRow("AreaCode").Trim
                dRow("StandAreaDescription") = dRow("StandCode").Trim & "-" & dRow("AreaCode").Trim
                dtStandAndArea.Rows.Add(dRow)
            End If
            ddlChangeStandAndArea.DataSource = dtStandAndArea
            ddlChangeStandAndArea.DataTextField = "StandAreaDescription"
            ddlChangeStandAndArea.DataValueField = "StandAreaCode"
            ddlChangeStandAndArea.DataBind()
            ddlChangeStandAndArea.SelectedValue = currentStandAreaCode
            plhStandAndArea.Visible = (ddlChangeStandAndArea.Items.Count >= 1)
            ddlChangeStandAndArea.Attributes.Add("onchange", "ReDrawStandAndArea(this.value);")
        Else
            plhStandAndArea.Visible = False
        End If
    End Sub

#End Region

#Region "Define Stand And Area Shapes"

    ''' <summary>
    ''' Define the XML Node for a PolyLine Object
    ''' </summary>
    ''' <param name="ndSVGElement">The SVG XML node working with</param>
    ''' <param name="ndAreaInformation">The XML node that is changed and presented to Raphael</param>
    ''' <remarks></remarks>
    Private Sub definePolylineStandAndArea(ByRef ndSVGElement As XmlNode, ByRef ndAreaInformation As XmlNode)
        Dim ndArea As XmlNode
        Dim atType, atId, atPoints, atStroke, atFill, atStrokeMiterlimit, atPriceFill, atAvailibiltyFill,
          atSeatSelection, atFillRule, atClipRule, atAvailability, atStrokeWidth, atTransform, atAvailibilitPercentage,
          atTicketExchangeFlag, atTicketExchangeSilderMaxPrice, atTicketExchangeSliderMinPrice As XmlAttribute
        With _standAndAreaXml
            ndArea = .CreateElement(AREA_SH)
            atType = .CreateAttribute(TYPE_SH)
            atPoints = .CreateAttribute(POINTS_SH)
            atAvailability = .CreateAttribute(AVAILABILITY_SH)
            atStrokeWidth = .CreateAttribute(STROKE_WIDTH_SH)
            atTransform = .CreateAttribute(TRANSFORM_SH)
            atAvailibilitPercentage = .CreateAttribute(AVAILABILITY_PERCENTAGE_SH)
            atTicketExchangeFlag = .CreateAttribute(TICKET_EXCHANGE_FLAG)
            atTicketExchangeSilderMaxPrice = .CreateAttribute(TICKET_EXCHANGE_MAX_SLIDER_PRICE)
            atTicketExchangeSliderMinPrice = .CreateAttribute(TICKET_EXCHANGE_MIN_SLIDER_PRICE)

            If ndSVGElement.Attributes(CLIP_RULE) IsNot Nothing Then
                atClipRule = .CreateAttribute(CLIP_RULE_SH)
                atClipRule.Value = ndSVGElement.Attributes(CLIP_RULE).Value
                ndArea.Attributes.Append(atClipRule)
            End If
            If ndSVGElement.Attributes(TRANSFORM) IsNot Nothing Then
                Dim transformReplaced As String = ndSVGElement.Attributes(TRANSFORM).Value.Replace("matrix(", "m")
                atTransform.Value = transformReplaced.Trim(")")
                ndArea.Attributes.Append(atTransform)
            End If
            If ndSVGElement.Attributes(STROKE_WIDTH) IsNot Nothing Then
                atStrokeWidth.Value = ndSVGElement.Attributes(STROKE_WIDTH).Value
                ndArea.Attributes.Append(atStrokeWidth)
            End If
            If ndSVGElement.Attributes(FILL_RULE) IsNot Nothing Then
                atFillRule = .CreateAttribute(FILL_RULE_SH)
                atFillRule.Value = ndSVGElement.Attributes(FILL_RULE).Value
                ndArea.Attributes.Append(atFillRule)
            End If
            If ndSVGElement.Attributes(STROKE_MITER_LIMIT) IsNot Nothing Then
                atStrokeMiterlimit = .CreateAttribute(STROKE_MITER_LIMIT_SH)
                atStrokeMiterlimit.Value = ndSVGElement.Attributes(STROKE_MITER_LIMIT).Value
                ndArea.Attributes.Append(atStrokeMiterlimit)
            End If
            If ndSVGElement.Attributes(XMLID) IsNot Nothing Then
                atId = .CreateAttribute(XMLID)
                atId.Value = ndSVGElement.Attributes(XMLID).Value
                atSeatSelection = .CreateAttribute(SEAT_SELECTION_SH)
                atSeatSelection.Value = getSeatSelectionSetting(ndSVGElement.Attributes(XMLID).Value)
                ndArea.Attributes.Append(atSeatSelection)
                ndArea.Attributes.Append(atId)
                atPriceFill = .CreateAttribute(PRICE_FILL_SH)
                atAvailibiltyFill = .CreateAttribute(AVAILABILITY_FILL_SH)

                Dim availabilityPercentage As Integer = 0
                Dim ticketingExchangeFlag As String = String.Empty
                Dim minTicketExchangePrice As String = String.Empty
                Dim maxTicketExchangePrice As String = String.Empty
                getStandAvailability(ndSVGElement.Attributes(XMLID).Value, availabilityPercentage, ticketingExchangeFlag, minTicketExchangePrice, maxTicketExchangePrice)
                atAvailibilitPercentage.Value = availabilityPercentage
                ndArea.Attributes.Append(atAvailibilitPercentage)
                atTicketExchangeFlag.Value = ticketingExchangeFlag
                ndArea.Attributes.Append(atTicketExchangeFlag)
                atTicketExchangeSliderMinPrice.Value = minTicketExchangePrice
                ndArea.Attributes.Append(atTicketExchangeSliderMinPrice)
                atTicketExchangeSilderMaxPrice.Value = maxTicketExchangePrice
                ndArea.Attributes.Append(atTicketExchangeSilderMaxPrice)

                Dim availabilityColour As String = String.Empty
                Dim availabilityText As String = String.Empty
                Dim noAvailability As Boolean = False
                getAvailabilityColourAndText(ndSVGElement.Attributes(XMLID).Value, availabilityColour, availabilityText, noAvailability)
                atAvailibiltyFill.Value = availabilityColour
                ndArea.Attributes.Append(atAvailibiltyFill)

                Dim priceColour As String = String.Empty
                Dim priceText As String = String.Empty
                getPriceColourAndText(ndSVGElement.Attributes(XMLID).Value, priceColour, priceText)
                atPriceFill.Value = priceColour
                ndArea.Attributes.Append(atPriceFill)
                If noAvailability Then
                    atAvailability.Value = NA
                    ndArea.Attributes.Append(atAvailability)
                End If
                ndArea.AppendChild(.CreateCDataSection(getStandAndAreaDescription(ndSVGElement.Attributes(XMLID).Value, priceText, availabilityText)))
            Else
                If ndSVGElement.Attributes(FILL) IsNot Nothing Then
                    atFill = .CreateAttribute(FILL_SH)
                    atFill.Value = ndSVGElement.Attributes(FILL).Value
                    ndArea.Attributes.Append(atFill)
                End If
            End If
            If ndSVGElement.Attributes(STROKE) IsNot Nothing Then
                atStroke = .CreateAttribute(STROKE_SH)
                atStroke.Value = ndSVGElement.Attributes(STROKE).Value
                ndArea.Attributes.Append(atStroke)
            End If
        End With

        atType.Value = ndSVGElement.Name
        atPoints.Value = ndSVGElement.Attributes(POINTS).Value
        ndArea.Attributes.Append(atType)
        ndArea.Attributes.Append(atPoints)
        ndAreaInformation.AppendChild(ndArea)
    End Sub

    ''' <summary>
    ''' Define the XML Node for a Text Object
    ''' </summary>
    ''' <param name="ndSVGElement">The SVG XML node working with</param>
    ''' <param name="ndAreaInformation">The XML node that is changed and presented to Raphael</param>
    ''' <remarks></remarks>
    Private Sub defineTextStandAndArea(ByRef ndSVGElement As XmlNode, ByRef ndAreaInformation As XmlNode)
        Dim ndArea As XmlNode
        Dim atType, atId, atTransform, atFill, atText, atFontFamily, atFontSize, atFID As XmlAttribute
        With _standAndAreaXml
            ndArea = .CreateElement(AREA_SH)
            atType = .CreateAttribute(TYPE_SH)
            atFontSize = .CreateAttribute(FONT_SIZE_SH)
            atId = .CreateAttribute(XMLID)
            atTransform = .CreateAttribute(TRANSFORM_SH)
            atFill = .CreateAttribute(FILL_SH)
            atText = .CreateAttribute(TEXT_SH)
            atFID = .CreateAttribute(STAND_AND_AREA_ID_SH)
            atFontFamily = .CreateAttribute(FONT_FAMILY_SH)
            If ndSVGElement.Attributes(STAND_AND_AREA_ID_SH) IsNot Nothing Then
                Dim availabilityColour As String = String.Empty
                Dim availabilityText As String = String.Empty
                Dim noAvailability As Boolean = False
                getAvailabilityColourAndText(ndSVGElement.Attributes(STAND_AND_AREA_ID_SH).Value, availabilityColour, availabilityText, noAvailability)
                Dim priceColour As String = String.Empty
                Dim priceText As String = String.Empty
                getPriceColourAndText(ndSVGElement.Attributes(STAND_AND_AREA_ID_SH).Value, priceColour, priceText)
                ndArea.AppendChild(.CreateCDataSection(getStandAndAreaDescription(ndSVGElement.Attributes(STAND_AND_AREA_ID_SH).Value, priceText, availabilityText)))
                atFID.Value = ndSVGElement.Attributes(STAND_AND_AREA_ID_SH).Value
                ndArea.Attributes.Append(atFID)
            End If
        End With

        Dim fontFamilyTrim As String = ndSVGElement.Attributes(FONT_FAMILY).Value.Trim("'")
        atType.Value = ndSVGElement.Name
        atFontFamily.Value = fontFamilyTrim
        If ndSVGElement.Attributes(XMLID) IsNot Nothing Then
            atId.Value = ndSVGElement.Attributes(XMLID).Value
        End If
        If ndSVGElement.Attributes(TRANSFORM) IsNot Nothing Then
            Dim transformReplaced As String = ndSVGElement.Attributes(TRANSFORM).Value.Replace("matrix(", "m")
            atTransform.Value = transformReplaced.Trim(")")
        End If
        If ndSVGElement.InnerText IsNot Nothing Then
            atText.Value = ndSVGElement.InnerText
        End If
        If ndSVGElement.Attributes(FILL) IsNot Nothing Then
            atFill.Value = ndSVGElement.Attributes(FILL).Value
        Else
            atFill.Value = "#FFFFFF"
        End If
        If ndSVGElement.Attributes(FONT_SIZE) IsNot Nothing Then
            atFontSize.Value = ndSVGElement.Attributes(FONT_SIZE).Value
        End If

        ndArea.Attributes.Append(atType)
        ndArea.Attributes.Append(atFontSize)
        ndArea.Attributes.Append(atText)
        ndArea.Attributes.Append(atFill)
        ndArea.Attributes.Append(atTransform)
        ndArea.Attributes.Append(atFontFamily)
        ndAreaInformation.AppendChild(ndArea)
    End Sub

    ''' <summary>
    ''' Define the XML Node for a Circle Object
    ''' </summary>
    ''' <param name="ndSVGElement">The SVG XML node working with</param>
    ''' <param name="ndAreaInformation">The XML node that is changed and presented to Raphael</param>
    ''' <remarks></remarks>
    Private Sub defineCircleStandAndArea(ByRef ndSVGElement As XmlNode, ByRef ndAreaInformation As XmlNode)
        Dim ndArea As XmlNode
        Dim atType, atId, atCX, atFill, atStroke, atCY, atR, atStrokeMiterlimit, atStrokeWidth, atTransform, atAvailibilitPercentage,
          atTicketExchangeFlag, atTicketExchangeSilderMaxPrice, atTicketExchangeSliderMinPrice As XmlAttribute
        With _standAndAreaXml
            ndArea = .CreateElement(AREA_SH)
            atType = .CreateAttribute(TYPE_SH)
            atId = .CreateAttribute(XMLID)
            atCX = .CreateAttribute(CX)
            atCY = .CreateAttribute(CY)
            atR = .CreateAttribute(R)
            atStrokeWidth = .CreateAttribute(STROKE_WIDTH_SH)
            atTransform = .CreateAttribute(TRANSFORM_SH)
            atAvailibilitPercentage = .CreateAttribute(AVAILABILITY_PERCENTAGE_SH)
            atTicketExchangeFlag = .CreateAttribute(TICKET_EXCHANGE_FLAG)
            atTicketExchangeSilderMaxPrice = .CreateAttribute(TICKET_EXCHANGE_MAX_SLIDER_PRICE)
            atTicketExchangeSliderMinPrice = .CreateAttribute(TICKET_EXCHANGE_MIN_SLIDER_PRICE)

            If ndSVGElement.Attributes(STROKE_MITER_LIMIT) IsNot Nothing Then
                atStrokeMiterlimit = .CreateAttribute(STROKE_MITER_LIMIT_SH)
                atStrokeMiterlimit.Value = ndSVGElement.Attributes(STROKE_MITER_LIMIT).Value
                ndArea.Attributes.Append(atStrokeMiterlimit)
            End If
            If ndSVGElement.Attributes(TRANSFORM) IsNot Nothing Then
                Dim transformReplaced As String = ndSVGElement.Attributes(TRANSFORM).Value.Replace("matrix(", "m")
                atTransform.Value = transformReplaced.Trim(")")
                ndArea.Attributes.Append(atTransform)
            End If
            If ndSVGElement.Attributes(STROKE) IsNot Nothing Then
                atStroke = .CreateAttribute(STROKE_SH)
                atStroke.Value = ndSVGElement.Attributes(STROKE).Value
                ndArea.Attributes.Append(atStroke)
            End If
            If ndSVGElement.Attributes(STROKE_WIDTH) IsNot Nothing Then
                atStrokeWidth.Value = ndSVGElement(STROKE_WIDTH).Value
                ndArea.Attributes.Append(atStrokeWidth)
            End If
            If ndSVGElement.Attributes(FILL) IsNot Nothing Then
                atFill = .CreateAttribute(FILL_SH)
                atFill.Value = ndSVGElement.Attributes(FILL).Value
                ndArea.Attributes.Append(atFill)
            End If
            If ndSVGElement.Attributes(XMLID) IsNot Nothing Then
                Dim availabilityPercentage As Integer = 0
                Dim ticketingExchangeFlag As String = String.Empty
                Dim minTicketExchangePrice As String = String.Empty
                Dim maxTicketExchangePrice As String = String.Empty
                getStandAvailability(ndSVGElement.Attributes(XMLID).Value, availabilityPercentage, ticketingExchangeFlag, minTicketExchangePrice, maxTicketExchangePrice)
                atAvailibilitPercentage.Value = availabilityPercentage
                ndArea.Attributes.Append(atAvailibilitPercentage)
                atTicketExchangeFlag.Value = ticketingExchangeFlag
                ndArea.Attributes.Append(atTicketExchangeFlag)
                atTicketExchangeSliderMinPrice.Value = minTicketExchangePrice
                ndArea.Attributes.Append(atTicketExchangeSliderMinPrice)
                atTicketExchangeSilderMaxPrice.Value = maxTicketExchangePrice
                ndArea.Attributes.Append(atTicketExchangeSilderMaxPrice)


                Dim availabilityColour As String = String.Empty
                Dim availabilityText As String = String.Empty
                Dim noAvailability As Boolean = False
                getAvailabilityColourAndText(ndSVGElement.Attributes(XMLID).Value, availabilityColour, availabilityText, noAvailability)
                Dim priceColour As String = String.Empty
                Dim priceText As String = String.Empty
                getPriceColourAndText(ndSVGElement.Attributes(XMLID).Value, priceColour, priceText)
                ndArea.AppendChild(.CreateCDataSection(getStandAndAreaDescription(ndSVGElement.Attributes(XMLID).Value, priceText, availabilityText)))
            End If
        End With

        atType.Value = ndSVGElement.Name
        If ndSVGElement.Attributes(XMLID) IsNot Nothing Then atId.Value = ndSVGElement.Attributes(XMLID).Value
        atCX.Value = ndSVGElement.Attributes(CX).Value
        atCY.Value = ndSVGElement.Attributes(CY).Value
        atR.Value = ndSVGElement.Attributes(R).Value
        ndArea.Attributes.Append(atType)
        ndArea.Attributes.Append(atId)
        ndArea.Attributes.Append(atCX)
        ndArea.Attributes.Append(atCY)
        ndArea.Attributes.Append(atR)
        ndAreaInformation.AppendChild(ndArea)
    End Sub

    ''' <summary>
    ''' Define the XML Node for a Line Object
    ''' </summary>
    ''' <param name="ndSVGElement">The SVG XML node working with</param>
    ''' <param name="ndAreaInformation">The XML node that is changed and presented to Raphael</param>
    ''' <remarks></remarks>
    Private Sub defineLineStandAndArea(ByRef ndSVGElement As XmlNode, ByRef ndAreaInformation As XmlNode)
        Dim ndArea As XmlNode
        Dim atType, atId, atX1, atY1, atX2, atY2, atStroke, atFill, atStrokeMiterlimit, atStrokeWidth, atTransform As XmlAttribute
        With _standAndAreaXml
            ndArea = .CreateElement(AREA_SH)
            atType = .CreateAttribute(TYPE_SH)
            atId = .CreateAttribute(XMLID)
            atX1 = .CreateAttribute(X1)
            atX2 = .CreateAttribute(Y1)
            atY1 = .CreateAttribute(Y1)
            atY2 = .CreateAttribute(Y2)
            atStrokeWidth = .CreateAttribute(STROKE_WIDTH_SH)
            atTransform = .CreateAttribute(TRANSFORM_SH)

            If ndSVGElement.Attributes(STROKE) IsNot Nothing Then
                atStroke = .CreateAttribute(STROKE_SH)
                atStroke.Value = ndSVGElement.Attributes(STROKE).Value
                ndArea.Attributes.Append(atStroke)
            End If
            If ndSVGElement.Attributes(TRANSFORM) IsNot Nothing Then
                Dim transformReplaced As String = ndSVGElement.Attributes(TRANSFORM).Value.Replace("matrix(", "m")
                atTransform.Value = transformReplaced.Trim(")")
                ndArea.Attributes.Append(atTransform)
            End If
            If ndSVGElement.Attributes(STROKE_WIDTH) IsNot Nothing Then
                atStrokeWidth.Value = ndSVGElement.Attributes(STROKE_WIDTH).Value
                ndArea.Attributes.Append(atStrokeWidth)
            End If
            If ndSVGElement.Attributes(STROKE_MITER_LIMIT) IsNot Nothing Then
                atStrokeMiterlimit = .CreateAttribute(STROKE_MITER_LIMIT_SH)
                atStrokeMiterlimit.Value = ndSVGElement.Attributes(STROKE_MITER_LIMIT).Value
                ndArea.Attributes.Append(atStrokeMiterlimit)
            End If
            If ndSVGElement.Attributes(FILL) IsNot Nothing Then
                atFill = .CreateAttribute(FILL_SH)
                atFill.Value = ndSVGElement.Attributes(FILL).Value
                ndArea.Attributes.Append(atFill)
            End If
        End With

        atType.Value = ndSVGElement.Name
        If ndSVGElement.Attributes(XMLID) IsNot Nothing Then atId.Value = ndSVGElement.Attributes(XMLID).Value
        atX1.Value = ndSVGElement.Attributes(X1).Value
        atY1.Value = ndSVGElement.Attributes(Y1).Value
        atX2.Value = ndSVGElement.Attributes(X2).Value
        atY2.Value = ndSVGElement.Attributes(Y2).Value
        ndArea.Attributes.Append(atType)
        ndArea.Attributes.Append(atId)
        ndArea.Attributes.Append(atX1)
        ndArea.Attributes.Append(atY1)
        ndArea.Attributes.Append(atX2)
        ndArea.Attributes.Append(atY2)
        ndAreaInformation.AppendChild(ndArea)
    End Sub

    ''' <summary>
    ''' Define the XML Node for a Path Object
    ''' </summary>
    ''' <param name="ndSVGElement">The SVG XML node working with</param>
    ''' <param name="ndAreaInformation">The XML node that is changed and presented to Raphael</param>
    ''' <remarks></remarks>
    Private Sub definePathStandAndArea(ByRef ndSVGElement As XmlNode, ByRef ndAreaInformation As XmlNode)
        Dim ndArea As XmlNode
        Dim atType, atId, atD, atPriceFill, atAvailibiltyFill, atStroke, atStrokeMiterlimit,
            atSeatSelection, atFillRule, atClipRule, atAvailability, atStrokeWidth, atTransform, atAvailibilitPercentage,
          atTicketExchangeFlag, atTicketExchangeSilderMaxPrice, atTicketExchangeSliderMinPrice As XmlAttribute

        With _standAndAreaXml
            ndArea = .CreateElement(AREA_SH)
            atType = .CreateAttribute(TYPE_SH)
            atD = .CreateAttribute(D)
            atStrokeWidth = .CreateAttribute(STROKE_WIDTH_SH)
            atAvailability = .CreateAttribute(AVAILABILITY_SH)
            atTransform = .CreateAttribute(TRANSFORM_SH)
            atAvailibilitPercentage = .CreateAttribute(AVAILABILITY_PERCENTAGE_SH)
            atTicketExchangeFlag = .CreateAttribute(TICKET_EXCHANGE_FLAG)
            atTicketExchangeSilderMaxPrice = .CreateAttribute(TICKET_EXCHANGE_MAX_SLIDER_PRICE)
            atTicketExchangeSliderMinPrice = .CreateAttribute(TICKET_EXCHANGE_MIN_SLIDER_PRICE)

            If ndSVGElement.Attributes(CLIP_RULE) IsNot Nothing Then
                atClipRule = .CreateAttribute(CLIP_RULE_SH)
                atClipRule.Value = ndSVGElement.Attributes(CLIP_RULE).Value
                ndArea.Attributes.Append(atClipRule)
            End If
            If ndSVGElement.Attributes(TRANSFORM) IsNot Nothing Then
                Dim transformReplaced As String = ndSVGElement.Attributes(TRANSFORM).Value.Replace("matrix(", "m")
                atTransform.Value = transformReplaced.Trim(")")
                ndArea.Attributes.Append(atTransform)
            End If
            If ndSVGElement.Attributes(STROKE_WIDTH) IsNot Nothing Then
                atStrokeWidth.Value = ndSVGElement.Attributes(STROKE_WIDTH).Value
                ndArea.Attributes.Append(atStrokeWidth)
            End If
            If ndSVGElement.Attributes(FILL_RULE) IsNot Nothing Then
                atFillRule = .CreateAttribute(FILL_RULE_SH)
                atFillRule.Value = ndSVGElement.Attributes(FILL_RULE).Value
                ndArea.Attributes.Append(atFillRule)
            End If
            If ndSVGElement.Attributes(XMLID) IsNot Nothing Then
                atPriceFill = .CreateAttribute(PRICE_FILL_SH)
                atAvailibiltyFill = .CreateAttribute(AVAILABILITY_FILL_SH)

                Dim availabilityPercentage As Integer = 0
                Dim ticketingExchangeFlag As String = String.Empty
                Dim minTicketExchangePrice As String = String.Empty
                Dim maxTicketExchangePrice As String = String.Empty
                getStandAvailability(ndSVGElement.Attributes(XMLID).Value, availabilityPercentage, ticketingExchangeFlag, minTicketExchangePrice, maxTicketExchangePrice)
                atAvailibilitPercentage.Value = availabilityPercentage
                ndArea.Attributes.Append(atAvailibilitPercentage)
                atTicketExchangeFlag.Value = ticketingExchangeFlag
                ndArea.Attributes.Append(atTicketExchangeFlag)
                atTicketExchangeSliderMinPrice.Value = minTicketExchangePrice
                ndArea.Attributes.Append(atTicketExchangeSliderMinPrice)
                atTicketExchangeSilderMaxPrice.Value = maxTicketExchangePrice
                ndArea.Attributes.Append(atTicketExchangeSilderMaxPrice)

                Dim availabilityColour As String = String.Empty
                Dim availabilityText As String = String.Empty
                Dim noAvailability As Boolean = False
                getAvailabilityColourAndText(ndSVGElement.Attributes(XMLID).Value, availabilityColour, availabilityText, noAvailability)
                atAvailibiltyFill.Value = availabilityColour
                ndArea.Attributes.Append(atAvailibiltyFill)

                Dim priceColour As String = String.Empty
                Dim priceText As String = String.Empty
                getPriceColourAndText(ndSVGElement.Attributes(XMLID).Value, priceColour, priceText)
                atPriceFill.Value = priceColour
                ndArea.Attributes.Append(atPriceFill)
                ndArea.AppendChild(.CreateCDataSection(getStandAndAreaDescription(ndSVGElement.Attributes(XMLID).Value, priceText, availabilityText)))
                If noAvailability Then
                    atAvailability.Value = NA
                    ndArea.Attributes.Append(atAvailability)
                End If
            Else
                If ndSVGElement.Attributes(FILL) IsNot Nothing Then
                    atAvailibiltyFill = .CreateAttribute(FILL_SH)
                    atAvailibiltyFill.Value = ndSVGElement.Attributes(FILL).Value
                    ndArea.Attributes.Append(atAvailibiltyFill)
                End If
            End If

            If ndSVGElement.Attributes(STROKE) IsNot Nothing Then
                atStroke = .CreateAttribute(STROKE_SH)
                atStroke.Value = ndSVGElement.Attributes(STROKE).Value
                ndArea.Attributes.Append(atStroke)
            End If
            If ndSVGElement.Attributes(STROKE_MITER_LIMIT) IsNot Nothing Then
                atStrokeMiterlimit = .CreateAttribute(STROKE_MITER_LIMIT_SH)
                atStrokeMiterlimit.Value = ndSVGElement.Attributes(STROKE_MITER_LIMIT).Value
                ndArea.Attributes.Append(atStrokeMiterlimit)
            End If
            If ndSVGElement.Attributes(XMLID) IsNot Nothing Then
                atId = .CreateAttribute(XMLID)
                atId.Value = ndSVGElement.Attributes(XMLID).Value
                ndArea.Attributes.Append(atId)
            End If
            If ndSVGElement.Attributes(XMLID) IsNot Nothing Then
                atSeatSelection = .CreateAttribute(SEAT_SELECTION_SH)
                atSeatSelection.Value = getSeatSelectionSetting(ndSVGElement.Attributes(XMLID).Value)
                ndArea.Attributes.Append(atSeatSelection)
            End If
        End With

        atType.Value = ndSVGElement.Name
        atD.Value = ndSVGElement.Attributes(D).Value
        ndArea.Attributes.Append(atType)
        ndArea.Attributes.Append(atD)
        ndAreaInformation.AppendChild(ndArea)
    End Sub

    ''' <summary>
    ''' Define the XML Node for a Polygon Object
    ''' </summary>
    ''' <param name="ndSVGElement">The SVG XML node working with</param>
    ''' <param name="ndAreaInformation">The XML node that is changed and presented to Raphael</param>
    ''' <remarks></remarks>
    Private Sub definePolygonStandAndArea(ByRef ndSVGElement As XmlNode, ByRef ndAreaInformation As XmlNode)
        Dim ndArea As XmlNode
        Dim atType, atId, atPoints, atStroke, atFill, atStrokeMiterlimit, atPriceFill, atAvailibiltyFill,
          atSeatSelection, atFillRule, atClipRule, atAvailability, atStrokeWidth, atTransform, atAvailibilitPercentage,
          atTicketExchangeFlag, atTicketExchangeSilderMaxPrice, atTicketExchangeSliderMinPrice As XmlAttribute
        With _standAndAreaXml
            ndArea = .CreateElement(AREA_SH)
            atType = .CreateAttribute(TYPE_SH)
            atPoints = .CreateAttribute(POINTS_SH)
            atAvailability = .CreateAttribute(AVAILABILITY_SH)
            atTransform = .CreateAttribute(TRANSFORM_SH)
            atStrokeWidth = .CreateAttribute(STROKE_WIDTH_SH)
            atAvailibilitPercentage = .CreateAttribute(AVAILABILITY_PERCENTAGE_SH)
            atTicketExchangeFlag = .CreateAttribute(TICKET_EXCHANGE_FLAG)
            atTicketExchangeSilderMaxPrice = .CreateAttribute(TICKET_EXCHANGE_MAX_SLIDER_PRICE)
            atTicketExchangeSliderMinPrice = .CreateAttribute(TICKET_EXCHANGE_MIN_SLIDER_PRICE)

            If ndSVGElement.Attributes(CLIP_RULE) IsNot Nothing Then
                atClipRule = .CreateAttribute(CLIP_RULE_SH)
                atClipRule.Value = ndSVGElement.Attributes(CLIP_RULE).Value
                ndArea.Attributes.Append(atClipRule)
            End If
            If ndSVGElement.Attributes(TRANSFORM) IsNot Nothing Then
                Dim transformReplaced As String = ndSVGElement.Attributes(TRANSFORM).Value.Replace("matrix(", "m")
                atTransform.Value = transformReplaced.Trim(")")
                ndArea.Attributes.Append(atTransform)
            End If
            If ndSVGElement.Attributes(STROKE_WIDTH) IsNot Nothing Then
                atStrokeWidth.Value = ndSVGElement.Attributes(STROKE_WIDTH).Value
                ndArea.Attributes.Append(atStrokeWidth)
            End If
            If ndSVGElement.Attributes(FILL_RULE) IsNot Nothing Then
                atFillRule = .CreateAttribute(FILL_RULE_SH)
                atFillRule.Value = ndSVGElement.Attributes(FILL_RULE).Value
                ndArea.Attributes.Append(atFillRule)
            End If
            If ndSVGElement.Attributes(STROKE_MITER_LIMIT) IsNot Nothing Then
                atStrokeMiterlimit = .CreateAttribute(STROKE_MITER_LIMIT_SH)
                atStrokeMiterlimit.Value = ndSVGElement.Attributes(STROKE_MITER_LIMIT).Value
                ndArea.Attributes.Append(atStrokeMiterlimit)
            End If
            If ndSVGElement.Attributes(XMLID) IsNot Nothing Then
                atId = .CreateAttribute(XMLID)
                atId.Value = ndSVGElement.Attributes(XMLID).Value
                atSeatSelection = .CreateAttribute(SEAT_SELECTION_SH)
                atSeatSelection.Value = getSeatSelectionSetting(ndSVGElement.Attributes(XMLID).Value)
                ndArea.Attributes.Append(atSeatSelection)
                ndArea.Attributes.Append(atId)
            End If
            If ndSVGElement.Attributes(XMLID) IsNot Nothing Then
                atPriceFill = .CreateAttribute(PRICE_FILL_SH)
                atAvailibiltyFill = .CreateAttribute(AVAILABILITY_FILL_SH)

                Dim availabilityPercentage As Integer = 0
                Dim ticketingExchangeFlag As String = String.Empty
                Dim minTicketExchangePrice As String = String.Empty
                Dim maxTicketExchangePrice As String = String.Empty
                getStandAvailability(ndSVGElement.Attributes(XMLID).Value, availabilityPercentage, ticketingExchangeFlag, minTicketExchangePrice, maxTicketExchangePrice)
                atAvailibilitPercentage.Value = availabilityPercentage
                ndArea.Attributes.Append(atAvailibilitPercentage)
                atTicketExchangeFlag.Value = ticketingExchangeFlag
                ndArea.Attributes.Append(atTicketExchangeFlag)
                atTicketExchangeSliderMinPrice.Value = minTicketExchangePrice
                ndArea.Attributes.Append(atTicketExchangeSliderMinPrice)
                atTicketExchangeSilderMaxPrice.Value = maxTicketExchangePrice
                ndArea.Attributes.Append(atTicketExchangeSilderMaxPrice)

                Dim availabilityColour As String = String.Empty
                Dim availabilityText As String = String.Empty
                Dim noAvailability As Boolean = False
                getAvailabilityColourAndText(ndSVGElement.Attributes(XMLID).Value, availabilityColour, availabilityText, noAvailability)
                atAvailibiltyFill.Value = availabilityColour
                ndArea.Attributes.Append(atAvailibiltyFill)

                Dim priceColour As String = String.Empty
                Dim priceText As String = String.Empty
                getPriceColourAndText(ndSVGElement.Attributes(XMLID).Value, priceColour, priceText)
                atPriceFill.Value = priceColour
                ndArea.Attributes.Append(atPriceFill)
                ndArea.AppendChild(.CreateCDataSection(getStandAndAreaDescription(ndSVGElement.Attributes(XMLID).Value, priceText, availabilityText)))
                If noAvailability Then
                    atAvailability.Value = NA
                    ndArea.Attributes.Append(atAvailability)
                End If
            Else
                If ndSVGElement.Attributes(FILL) IsNot Nothing Then
                    atFill = .CreateAttribute(FILL_SH)
                    atFill.Value = ndSVGElement.Attributes(FILL).Value
                    ndArea.Attributes.Append(atFill)
                End If
            End If
            If ndSVGElement.Attributes(STROKE) IsNot Nothing Then
                atStroke = .CreateAttribute(STROKE_SH)
                atStroke.Value = ndSVGElement.Attributes(STROKE).Value
                ndArea.Attributes.Append(atStroke)
            End If
        End With

        atType.Value = ndSVGElement.Name
        atPoints.Value = ndSVGElement.Attributes(POINTS).Value
        ndArea.Attributes.Append(atType)
        ndArea.Attributes.Append(atPoints)
        ndAreaInformation.AppendChild(ndArea)
    End Sub

    ''' <summary>
    ''' Define the XML Node for a Rectangle Object
    ''' </summary>
    ''' <param name="ndSVGElement">The SVG XML node working with</param>
    ''' <param name="ndAreaInformation">The XML node that is changed and presented to Raphael</param>
    ''' <remarks></remarks>
    Private Sub defineRectangleStandAndArea(ByRef ndSVGElement As XmlNode, ByRef ndAreaInformation As XmlNode)
        Dim ndArea As XmlNode
        Dim atType, atId, atX, atY, atStroke, atFill, atWidth, atHeight, atStrokeMiterlimit, atPriceFill,
              atSeatSelection, atFillRule, atClipRule, atAvailabiltyFill, atAvailability, atStrokeWidth, atTransform, atAvailibilitPercentage,
          atTicketExchangeFlag, atTicketExchangeSilderMaxPrice, atTicketExchangeSliderMinPrice As XmlAttribute
        With _standAndAreaXml
            ndArea = .CreateElement(AREA_SH)
            atType = .CreateAttribute(TYPE_SH)
            atY = .CreateAttribute(Y)
            atX = .CreateAttribute(X)

            atWidth = .CreateAttribute(WIDTH_SH)
            atHeight = .CreateAttribute(HEIGHT_SH)
            atFill = .CreateAttribute(FILL_SH)
            atAvailability = .CreateAttribute(AVAILABILITY_SH)
            atStrokeWidth = .CreateAttribute(STROKE_WIDTH_SH)
            atTransform = .CreateAttribute(TRANSFORM_SH)
            atAvailibilitPercentage = .CreateAttribute(AVAILABILITY_PERCENTAGE_SH)
            atTicketExchangeFlag = .CreateAttribute(TICKET_EXCHANGE_FLAG)
            atTicketExchangeSilderMaxPrice = .CreateAttribute(TICKET_EXCHANGE_MAX_SLIDER_PRICE)
            atTicketExchangeSliderMinPrice = .CreateAttribute(TICKET_EXCHANGE_MIN_SLIDER_PRICE)

            If ndSVGElement.Attributes(STROKE_WIDTH) IsNot Nothing Then
                atStrokeWidth.Value = ndSVGElement.Attributes(STROKE_WIDTH).Value
                ndArea.Attributes.Append(atStrokeWidth)
            End If
            If ndSVGElement.Attributes(TRANSFORM) IsNot Nothing Then
                Dim transformReplaced As String = ndSVGElement.Attributes(TRANSFORM).Value.Replace("matrix(", "m")
                atTransform.Value = transformReplaced.Trim(")")
                ndArea.Attributes.Append(atTransform)
            End If
            If ndSVGElement.Attributes(CLIP_RULE) IsNot Nothing Then
                atClipRule = .CreateAttribute(CLIP_RULE_SH)
                atClipRule.Value = ndSVGElement.Attributes(CLIP_RULE).Value
                ndArea.Attributes.Append(atClipRule)
            End If
            If ndSVGElement.Attributes(FILL_RULE) IsNot Nothing Then
                atFillRule = .CreateAttribute(FILL_RULE_SH)
                atFillRule.Value = ndSVGElement.Attributes(FILL_RULE).Value
                ndArea.Attributes.Append(atFillRule)
            End If
            If ndSVGElement.Attributes(STROKE) IsNot Nothing Then
                atStroke = .CreateAttribute(STROKE_SH)
                atStroke.Value = ndSVGElement.Attributes(STROKE).Value
                ndArea.Attributes.Append(atStroke)
            End If
            If ndSVGElement.Attributes(XMLID) IsNot Nothing Then
                atId = .CreateAttribute(XMLID)
                atId.Value = ndSVGElement.Attributes(XMLID).Value
                ndArea.Attributes.Append(atId)
                atSeatSelection = .CreateAttribute(SEAT_SELECTION_SH)
                atSeatSelection.Value = getSeatSelectionSetting(ndSVGElement.Attributes(XMLID).Value)
                ndArea.Attributes.Append(atSeatSelection)

                Dim availabilityPercentage As Integer = 0
                Dim ticketingExchangeFlag As String = String.Empty
                Dim minTicketExchangePrice As String = String.Empty
                Dim maxTicketExchangePrice As String = String.Empty
                getStandAvailability(ndSVGElement.Attributes(XMLID).Value, availabilityPercentage, ticketingExchangeFlag, minTicketExchangePrice, maxTicketExchangePrice)
                atAvailibilitPercentage.Value = availabilityPercentage
                ndArea.Attributes.Append(atAvailibilitPercentage)
                atTicketExchangeFlag.Value = ticketingExchangeFlag
                ndArea.Attributes.Append(atTicketExchangeFlag)
                atTicketExchangeSliderMinPrice.Value = minTicketExchangePrice
                ndArea.Attributes.Append(atTicketExchangeSliderMinPrice)
                atTicketExchangeSilderMaxPrice.Value = maxTicketExchangePrice
                ndArea.Attributes.Append(atTicketExchangeSilderMaxPrice)

                atPriceFill = .CreateAttribute(PRICE_FILL_SH)
                atAvailabiltyFill = .CreateAttribute(AVAILABILITY_FILL_SH)
                Dim availabilityColour As String = String.Empty
                Dim availabilityText As String = String.Empty
                Dim noAvailability As Boolean = False
                getAvailabilityColourAndText(ndSVGElement.Attributes(XMLID).Value, availabilityColour, availabilityText, noAvailability)
                atAvailabiltyFill.Value = availabilityColour
                ndArea.Attributes.Append(atAvailabiltyFill)

                Dim priceColour As String = String.Empty
                Dim priceText As String = String.Empty
                getPriceColourAndText(ndSVGElement.Attributes(XMLID).Value, priceColour, priceText)
                atPriceFill.Value = priceColour
                ndArea.Attributes.Append(atPriceFill)
                ndArea.AppendChild(.CreateCDataSection(getStandAndAreaDescription(ndSVGElement.Attributes(XMLID).Value, priceText, availabilityText)))
                If noAvailability Then
                    atAvailability.Value = NA
                    ndArea.Attributes.Append(atAvailability)
                End If
            Else
                If ndSVGElement.Attributes(FILL) IsNot Nothing Then
                    atFill.Value = ndSVGElement.Attributes(FILL).Value
                    ndArea.Attributes.Append(atFill)
                End If
            End If
            If ndSVGElement.Attributes(STROKE_MITER_LIMIT) IsNot Nothing Then
                atStrokeMiterlimit = .CreateAttribute(STROKE_MITER_LIMIT_SH)
                atStrokeMiterlimit.Value = ndSVGElement.Attributes(STROKE_MITER_LIMIT).Value
                ndArea.Attributes.Append(atStrokeMiterlimit)
            End If
        End With

        atType.Value = ndSVGElement.Name
        atX.Value = ndSVGElement.Attributes(X).Value
        atY.Value = ndSVGElement.Attributes(Y).Value
        atWidth.Value = ndSVGElement.Attributes(WIDTH).Value
        atHeight.Value = ndSVGElement.Attributes(HEIGHT).Value
        ndArea.Attributes.Append(atType)
        ndArea.Attributes.Append(atX)
        ndArea.Attributes.Append(atY)
        ndArea.Attributes.Append(atWidth)
        ndArea.Attributes.Append(atHeight)
        ndAreaInformation.AppendChild(ndArea)
    End Sub

#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Get the price text depending on the price category of the area
    ''' </summary>
    ''' <param name="areaCategory">The price category</param>
    ''' <param name="text">The text value that needs to be changed</param>
    ''' <returns>The changed text value</returns>
    ''' <remarks></remarks>
    Public Function GetPriceTextBasedOnCategory(ByVal areaCategory As String, ByVal text As String) As String
        Dim priceText As String = String.Empty
        Dim dvProductPrices As New DataView(_dtProductPriceDetails)
        dvProductPrices.RowFilter = "AreaCategory = '" & areaCategory & "'"
        dvProductPrices.Sort = "Price"
        If dvProductPrices.Count > 0 Then
            Dim lowestPrice As Decimal = CDec(dvProductPrices.Item(0)("Price"))
            Dim highestPrice As String = CDec(dvProductPrices.Item(dvProductPrices.Count - 1)("Price"))
            'Use of HTMLDecode here replace "&pound;" with "£"
            If lowestPrice = highestPrice Then
                priceText = text.Replace("<<PRICE_RANGE>>", HttpContext.Current.Server.HtmlDecode(TDataObjects.PaymentSettings.FormatCurrency(lowestPrice, _ucr.BusinessUnit, _ucr.PartnerCode)))
            Else
                priceText = text.Replace("<<PRICE_RANGE>>", HttpContext.Current.Server.HtmlDecode(TDataObjects.PaymentSettings.FormatCurrency(lowestPrice, _ucr.BusinessUnit, _ucr.PartnerCode)) & " - " & HttpContext.Current.Server.HtmlDecode(TDataObjects.PaymentSettings.FormatCurrency(highestPrice, _ucr.BusinessUnit, _ucr.PartnerCode)))
            End If
        End If
        Return priceText
    End Function

    ''' <summary>
    ''' Rebuild the Stadium layout XML for the SVG based on the specified price break ID
    ''' </summary>
    ''' <param name="priceBreakId">The given price break ID</param>
    ''' <returns>An error boolean</returns>
    ''' <remarks></remarks>
    Public Function ReBuildStadiumXml(ByVal priceBreakId As Long) As Boolean
        Dim hasError As Boolean = False
        Try
            _selectedPriceBreakId = priceBreakId
            initialiseClassObjects()
            If Not _hasError Then getStadiumSVG(False)
            If Not _hasError Then getColoursAndTextValues()
            If Not _hasError Then getProductDetails(False, False)
            If Not _hasError Then getProductAvailability(False)
            If Not _hasError Then getProductPriceDetails(False)
            If Not _hasError Then getStadiumDescriptions(False)
            If Not _hasError Then populateStandAndAreaXml()
            If Not _hasError Then
                Dim productAvailabilityModelBuilder As New ProductAvailabilityBuilder
                Dim inputModel As New ProductAvailabilityInputModel
                Dim viewModel As New ProductAvailabilityViewModel
                Dim serializer As New JavaScriptSerializer
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
                    .StandCode = SelectedStand
                    .AreaCode = SelectedArea
                    .SelectedMinimumPrice = SelectedMinimumPrice
                    .SelectedMaximumPrice = SelectedMaximumPrice
                    .DefaultPriceBand = _defaultPriceBand
                End With
                viewModel = productAvailabilityModelBuilder.GetProductAvailability(inputModel)
                viewModel.StadiumXml = StadiumXml
                StadiumAvailabilityJSON = serializer.Serialize(viewModel)
                hasError = viewModel.Error.HasError
                If hasError Then
                    handleErrors("SS-14a", viewModel.Error.ErrorMessage, False)
                End If
            End If
            hasError = _hasError
        Catch ex As Exception
            hasError = True
            handleErrors("SS-14b", ex.Message, False)
        End Try
        Return hasError
    End Function

#End Region

End Class