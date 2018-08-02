Imports DataSetHelperProdouct
Imports Microsoft.VisualBasic
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.CATHelper
Imports Talent.eCommerce.Utilities
Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Web.UI.HtmlControls
Imports System.Xml

Partial Class TalentEBusiness_UserControls_ProductDetail
    Inherits ControlBase

#Region "Class Level Fields"
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _PPSType As String
    Private _PPSFreeAreas As String
    Private _AmendPPSEnrolmentInUse As Boolean
    Private _showPaymentOptions As Boolean = False
    Private _productDetailHelper As New ProductDetail
    Private _courseStadium As String = String.Empty
    Private _isSingleProductFilter As Boolean = False
    Private _isFacebookLikeOn As Boolean = False
    Private _facebookLikeHtmlTag As String = String.Empty
    Private _fbLikeMetaTagsForProductNotExists As Boolean = True
    Private _defaultListItemValue As String = "0"
    Private _pageNumber As String = Nothing
    Private _showSelectionBox As Boolean = True
    Private _errMsg As Talent.Common.TalentErrorMessages
    Private _ucr As New Talent.Common.UserControlResource
    Private _ProductType As String
    Private _ProductSubType As String
    Private _ds1 As New DataSet()
    Private _CPage As New Label
    Private _CurPage, _CurRec, _i, _totLinks, _lowPage, _highPage As Integer
    Private _vPerpage, _vTotalRec As Int16
    Private _objPds As New PagedDataSource()
    Private _filterDropDownHasItems As Boolean = True

    Const _cProductTable = 1
    Const _cStateSoldOut = " ui-state-soldOut"
    Const _cStateDisabled = " ui-state-disabled"

#End Region

#Region "Properties"
    Public ReadOnly Property ProductsRepeater() As Repeater
        Get
            Return Me.ProductRepeater
        End Get
    End Property

    Public Property ProductType() As String
        Get
            Return _ProductType
        End Get
        Set(ByVal value As String)
            _ProductType = value
        End Set
    End Property

    Public Property ProductSubType() As String
        Get
            Return _ProductSubType
        End Get
        Set(ByVal value As String)
            _ProductSubType = value
        End Set
    End Property

    Public Property PPSType() As String
        Get
            Return _PPSType
        End Get
        Set(ByVal value As String)
            _PPSType = value
        End Set
    End Property

    Public Property PPSFreeAreas() As String
        Get
            Return _PPSFreeAreas
        End Get
        Set(ByVal value As String)
            _PPSFreeAreas = value
        End Set
    End Property

    Public Property AmendPPSEnrolmentInUse() As Boolean
        Get
            Return _AmendPPSEnrolmentInUse
        End Get
        Set(ByVal value As Boolean)
            _AmendPPSEnrolmentInUse = value
        End Set
    End Property


    Public Property BlockGridStyleClass() As String = String.Empty
#End Region

#Region "Public Methods"
    Public Function ShowPrice(ByVal ProductPrice As String) As String
        If _ProductType <> "C" Then
            Return ""
        Else
            Return TDataObjects.PaymentSettings.FormatCurrency(ProductPrice, _ucr.BusinessUnit, _ucr.PartnerCode)
        End If
    End Function

    ''' <summary>
    ''' Format the Url for the visual seat selection hyperlink based on the current product being shown
    ''' </summary>
    ''' <param name="stadiumCode">The current stadium code</param>
    ''' <param name="productCode">The current product code</param>
    ''' <param name="campaignCode">The current camapign code</param>
    ''' <param name="productType">The current product type</param>
    ''' <param name="productSubType">The current product sub type</param>
    ''' <param name="productHomeAsAway">The home as away setting</param>
    ''' <param name="stadiumName">The stadium name</param>
    ''' <param name="restrictGraphical">The restrict graphical value</param>
    ''' <returns>A formatted string to redirect to</returns>
    ''' <remarks></remarks>
    Public Function setVisualSeatSelectionUrl(ByRef stadiumName As String, ByVal stadiumCode As String, ByVal productCode As String, ByVal campaignCode As String,
                                               ByVal productType As String, ByVal productSubType As String, ByVal productHomeAsAway As String, ByVal restrictGraphical As Boolean) As String
        Dim redirectUrl As String = GetFormattedSeatSelectionUrl(stadiumName, stadiumCode, productCode, campaignCode, productType, productSubType, productHomeAsAway, restrictGraphical)
        Return ResolveUrl(redirectUrl)
    End Function

#End Region

#Region "Protected Page Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ProductDetail.ascx"
        End With
        _errMsg = New Talent.Common.TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
        IsCATSessionsRemoved()
        BlockGridStyleClass = _ucr.Content("BlockGridStyleClass", _languageCode, True)
        If Not String.IsNullOrEmpty(Request.QueryString("page")) Then _pageNumber = Request.QueryString("page").ToString.Trim
        If Not String.IsNullOrEmpty(Request.QueryString("productsubtype")) Then _ProductSubType = Request.QueryString("productsubtype").ToString().Trim()
        If Page.IsPostBack Then _pageNumber = _productDetailHelper.GetPageNumber(_pageNumber)
        BindProductRepeater()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _courseStadium = ModuleDefaults.CourseStadium.Trim
        If Not Page.IsPostBack Then
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
                errorlist.Items.Add(_errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, myError).ERROR_MESSAGE)
                Session("TalentErrorCode") = Nothing
            End If
        End If

        'Load the product group include file if it exists
        plhProductSubType.Visible = False
        If Not String.IsNullOrWhiteSpace(Request.QueryString("ProductSubType")) Then
            Dim productSubTypePath As String = TalentCache.GetBusinessUnit() & "/" & TalentCache.GetPartner(HttpContext.Current.Profile) & "/Product/SubType/" & _ProductType.Trim & CType(Request.QueryString("ProductSubType"), String).Trim & ".htm"
            If Talent.eCommerce.Utilities.DoesHtmlFileExists(productSubTypePath) Then
                plhProductSubType.Visible = True
                ltlProductSubTypeHtmlInclude.Text = Talent.eCommerce.Utilities.GetHtmlFromFile(productSubTypePath)
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Dim showNoProductsMessage As Boolean = True
        SetProductFilterFieldsFromSessions(True, True)
        If ProductRepeater.Items.Count = 0 Then
            showNoProductsMessage = True
        Else
            For Each item As RepeaterItem In ProductRepeater.Items
                If item.Visible Then showNoProductsMessage = False
            Next
        End If
        If showNoProductsMessage AndAlso Talent.eCommerce.Utilities.GetCurrentPageName().ToUpper() <> "TICKETINGPREPAYMENTS.ASPX" Then
            plhNoProductsFound.Visible = True
            plhTicketingProducts.Visible = False
            ltlNoProductsFound.Text = _ucr.Content("NoProductsFoundMessageText", _languageCode, True)
            PagerBottom.Visible = False
            PagerTop.Visible = False
        Else
            plhNoProductsFound.Visible = False
            setBottomPagerValues()
            If AmendPPSEnrolmentInUse And _showPaymentOptions Then
                Me.Page.GetType.InvokeMember("ShowPaymentOptions", System.Reflection.BindingFlags.InvokeMethod, Nothing, Me.Page, New Object() {True})
            End If
            If (plhSimpleFilterDropDown.Visible = False OrElse filterByDropDown.Visible = False) AndAlso plhTopPager.Visible = False Then
                plhPagerTopWrapper.Visible = False
            End If
        End If

        If errorlist.Items.Count > 0 Then
            plhErrorList.Visible = True
        Else
            plhErrorList.Visible = False
        End If
    End Sub

#End Region

#Region "Product Repeater Binding"

    Protected Sub ProductRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles ProductRepeater.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then

            If ModuleDefaults.TicketingKioskMode Then
                Dim MyNormalViewPanel As PlaceHolder = CType(e.Item.FindControl("plhNormalView"), PlaceHolder)
                Dim MyKioskViewPanel As PlaceHolder = CType(e.Item.FindControl("plhKioskView"), PlaceHolder)
                MyNormalViewPanel.Visible = False
                MyKioskViewPanel.Visible = True
            Else

                'Find controls that are in the repeater
                Dim myRow As DataRow = CType(e.Item.DataItem, DataRowView).Row

                If _isSingleProductFilter Then
                    GenerateFaceBookLikeMetaTags(myRow)
                End If

                If _isFacebookLikeOn Then
                    Dim plhFacebookLike As PlaceHolder = CType(e.Item.FindControl("plhFacebookLike"), PlaceHolder)
                    Dim ltlFacebookLike As Literal = CType(e.Item.FindControl("ltlFacebookLike"), Literal)
                    plhFacebookLike.Visible = True
                    ltlFacebookLike.Visible = True
                    Dim fbHtmlTag As String = _facebookLikeHtmlTag
                    Dim fbProductURL As String = String.Empty
                    fbProductURL = Request.Url.GetLeftPart(UriPartial.Path) & "?IsSingleProduct=TRUE"
                    fbProductURL = fbProductURL & "&amp;ProductType=" & CheckForDBNull_String(myRow("ProductType"))
                    fbProductURL = fbProductURL & "&amp;ProductCode=" & CheckForDBNull_String(myRow("ProductCode"))
                    If CheckForDBNull_String(myRow("ProductSubType")).Trim().Length > 0 Then
                        fbProductURL = fbProductURL & "&amp;ProductSubType=" & CheckForDBNull_String(myRow("ProductSubType"))
                    End If
                    If ProductType = "T" Then
                        fbProductURL = fbProductURL & "&amp;ProductDetailCode=" & CheckForDBNull_String(myRow("ProductDetailCode")).Trim()
                    End If
                    fbHtmlTag = fbHtmlTag.Replace("[FBLIKEURL]", fbProductURL)
                    ltlFacebookLike.Text = fbHtmlTag
                End If

                ' Hide anything where image isn't found.
                Dim sponsorImage As Image = CType(e.Item.FindControl("SponsorImage"), Image)
                If Not sponsorImage Is Nothing AndAlso sponsorImage.ImageUrl = ModuleDefaults.MissingImagePath Then
                    CType(e.Item.FindControl("plhSponsorImage"), PlaceHolder).Visible = False
                End If
                Dim competitionImage As Image = CType(e.Item.FindControl("CompetitionImage"), Image)
                If Not competitionImage Is Nothing AndAlso competitionImage.ImageUrl = ModuleDefaults.MissingImagePath Then
                    competitionImage.Visible = False
                End If
                Dim oppositionImage As Image = CType(e.Item.FindControl("OppositionImage"), Image)
                If Not oppositionImage Is Nothing AndAlso oppositionImage.ImageUrl = ModuleDefaults.MissingImagePath Then
                    oppositionImage.Visible = False
                End If

                plhSimpleFilterDropDown.Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("ShowSimpleProductFilter"))

                'set the Accordian Css Class
                CType(e.Item.FindControl("pnlAccordian"), Panel).CssClass += " " + CheckForDBNull_String(myRow("ProductCode"))
                CType(e.Item.FindControl("pnlAccordian"), Panel).CssClass += " ebiz-product-type-" + CheckForDBNull_String(myRow("ProductType"))
                CType(e.Item.FindControl("pnlAccordian"), Panel).CssClass += ProductDetail.GetProductBundleCSSClassName(CheckForDBNullOrBlank_Boolean_DefaultFalse(myRow("IsProductBundle")))
                CType(e.Item.FindControl("pnlAccordian"), Panel).CssClass += DisableAccordion(CheckForDBNullOrBlank_Boolean_DefaultFalse(myRow("IsSoldOut")))

                'Control logic for specific product types
                Select Case _ProductType
                    Case "A" 'Away product
                        BindProductTypes_A(e)

                    Case "H" 'Home Product
                        BindProductTypes_H(e)

                    Case "T", "E" 'Travel & Events
                        BindProductTypes_T_E(e)

                    Case "C" 'Membership Product
                        BindProductTypes_C(e)

                    Case "P" 'Pre Payment Schemes
                        BindProductTypes_P(e)

                    Case "S" 'Season Ticket Product
                        BindProductTypes_S(e)
                    Case "CH" 'Match Day Hospitality
                        BindProductTypes_CH(e)
                End Select

                '---------------------------------------------
                ' Check if need to retrieve the extended text. This should be switched off if not used as 
                'it requires an additional call to back end per product
                '---------------------------------------------
                BindExtendedDescription(e)
            End If
        End If
    End Sub

    Private Sub BindProductTypes_A(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        setControlVisibility(e.Item)
        SetProductAvailabilityText(e, "UnavailableMessage_AwayGameProduct")
        Dim myRow As DataRow = CType(e.Item.DataItem, DataRowView).Row
        Dim productCode As String = CType(e.Item.FindControl("hfProductCode"), HiddenField).Value
        Dim plhEligibility As PlaceHolder = CType(e.Item.FindControl("plhEligibility"), PlaceHolder)
        Dim MyProductHomeAsAway As HiddenField = CType(e.Item.FindControl("hfProductHomeAsAway"), HiddenField)
        Dim hdfProductAvailability As HiddenField = CType(e.Item.FindControl("hdfProductAvailability"), HiddenField)
        Dim soldOut As Boolean = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(myRow("IsSoldOut"))
        Dim priceBandSelectionControl As UserControls_PriceBandSelection = CType(e.Item.FindControl("PriceBandSelection1"), UserControls_PriceBandSelection)
        Dim prod_availabilityPercentage As Integer = 0
        Dim prod_availability As Integer = 0
        Dim prod_capacity As Integer = 0
        Dim prod_returned As Integer = 0
        Dim prod_sold As Integer = 0
        Dim prod_reserved As Integer = 0
        Dim prod_booked As Integer = 0
        Dim prod_unavailable As Integer = 0

        If (myRow("ProductType").ToString.Trim.Equals("H") And MyProductHomeAsAway.Value.Trim().Equals("Y")) Then
            If hdfProductAvailability.Value.ToUpper <> "N" Then
                If soldOut Then
                    CType(e.Item.FindControl("plhSoldOut"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("ltlSoldOut"), Literal).Text = _ucr.Content("SoldOutText", _languageCode, True)
                    If Not String.IsNullOrWhiteSpace(_ucr.Content("SoldOutHeaderText", _languageCode, True)) Then
                        CType(e.Item.FindControl("plhSoldOutHeader"), PlaceHolder).Visible = True
                        CType(e.Item.FindControl("ltlSoldOutHeader"), Literal).Text = _ucr.Content("SoldOutHeaderText", _languageCode, True)
                    End If
                End If
            Else
                plhEligibility.Visible = False
            End If
        Else
            If soldOut Then
                priceBandSelectionControl.Visible = False
                CType(e.Item.FindControl("plhSoldOut"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("ltlSoldOut"), Literal).Text = _ucr.Content("SoldOutText", _languageCode, True)
                If Not String.IsNullOrWhiteSpace(_ucr.Content("SoldOutHeaderText", _languageCode, True)) Then
                    CType(e.Item.FindControl("plhSoldOutHeader"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("ltlSoldOutHeader"), Literal).Text = _ucr.Content("SoldOutHeaderText", _languageCode, True)
                End If
                plhEligibility.Visible = False
            Else
                priceBandSelectionControl.Visible = True
                priceBandSelectionControl.ProductType = Me.ProductType
                priceBandSelectionControl.ProductSubType = Me.ProductSubType
                priceBandSelectionControl.ProductStadium = myRow("ProductStadium").ToString.Trim
                priceBandSelectionControl.PriceCode = myRow("PriceCode").ToString()

                _productDetailHelper.getProductAvailabilityFigures_TAE(productCode, ProductType, myRow("PriceCode").ToString(), String.Empty, prod_availability, prod_availabilityPercentage,
                                                      prod_capacity, prod_returned, prod_sold, prod_reserved, prod_booked, prod_unavailable)
                priceBandSelectionControl.QuantityAvailable = prod_availability

                If priceBandSelectionControl.QuantityAvailable <= 0 Then
                    CType(e.Item.FindControl("plhSoldOutHeader"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("ltlSoldOutHeader"), Literal).Text = _ucr.Content("SoldOutHeaderText", _languageCode, True)
                Else
                    priceBandSelectionControl.LoadPriceBandSelection()
                End If
            End If
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowAvailabilityLabel")) Then
                CType(e.Item.FindControl("plhAvailability"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("ltlAvailabilityLabel"), Literal).Text = _ucr.Content("AvailabilityLabelText", _languageCode, True)
                Dim lblAvailabilityValue As New Label
                setAvailabilityLabel(lblAvailabilityValue, prod_availabilityPercentage, myRow("ProductStadium").ToString().Trim())

                Dim productTypeMask As String
                productTypeMask = _ucr.Content("awayAvailabilityMaskLabel", _languageCode, True)
                lblAvailabilityValue.Text = _productDetailHelper.setAvailabilityMask(productTypeMask, lblAvailabilityValue.Text, _ProductType, prod_availability.ToString,
                                                                prod_availabilityPercentage.ToString, prod_capacity.ToString, prod_returned.ToString,
                                                                prod_sold.ToString, prod_reserved.ToString, prod_booked.ToString, prod_unavailable.ToString, "")

                CType(e.Item.FindControl("lblAvailabilityValue"), Label).BackColor = lblAvailabilityValue.BackColor
                CType(e.Item.FindControl("lblAvailabilityValue"), Label).Text = lblAvailabilityValue.Text
                CType(e.Item.FindControl("lblAvailabilityValue"), Label).CssClass = lblAvailabilityValue.CssClass
            End If
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowLocationLabel")) Then
                If String.IsNullOrEmpty(myRow("location").ToString.Trim) Then
                    CType(e.Item.FindControl("plhLocation"), PlaceHolder).Visible = False
                Else
                    CType(e.Item.FindControl("plhLocation"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("ltlLocationLabel"), Literal).Text = _ucr.Content("LocationLabelText", _languageCode, True)
                    CType(e.Item.FindControl("ltlLocationValue"), Literal).Text = myRow("location").ToString.Trim
                End If
            End If
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowStadiumDescriptionLabel")) Then
                If String.IsNullOrEmpty(myRow("ProductStadiumDescription").ToString.Trim) Then
                    CType(e.Item.FindControl("plhStadiumDescription"), PlaceHolder).Visible = False
                Else
                    CType(e.Item.FindControl("plhStadiumDescription"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("ltlStadiumDescriptionLabel"), Literal).Text = _ucr.Content("StadiumDescriptionLabelText", _languageCode, True)
                    CType(e.Item.FindControl("ltlStadiumDescriptionValue"), Literal).Text = myRow("ProductStadiumDescription").ToString.Trim
                End If
            End If
        End If

        'show controls relevant to this type
        If Not String.IsNullOrEmpty(myRow("ProductDescription").ToString.Trim) Then
            CType(e.Item.FindControl("ltlProductDescription"), Literal).Text = myRow("ProductDescription").ToString.Trim
            CType(e.Item.FindControl("plhProductDescription"), PlaceHolder).Visible = True
        End If
        If Not String.IsNullOrEmpty(myRow("ProductDescription2").ToString.Trim) Then
            CType(e.Item.FindControl("ltlProductCompetitionLabel"), Literal).Text = _ucr.Content("ProductCompetitionLabel", _languageCode, True)
            CType(e.Item.FindControl("ltlProductCompetitionValue"), Literal).Text = myRow("ProductDescription2").ToString.Trim
            CType(e.Item.FindControl("plhProductCompetition"), PlaceHolder).Visible = True
        End If
        If Not String.IsNullOrEmpty(myRow("ProductMDTE08").ToString.Trim) AndAlso myRow("HideDate") = False Then
            CType(e.Item.FindControl("ltlProductDateLabel"), Literal).Text = _ucr.Content("ProductDateLabel", _languageCode, True)
            CType(e.Item.FindControl("ltlProductDateValue"), Literal).Text = GetFormattedProductDate(myRow("ProductMDTE08").ToString.Trim, myRow("ProductYear").ToString.Trim)
            CType(e.Item.FindControl("plhProductDate"), PlaceHolder).Visible = True
        End If
        If Not String.IsNullOrEmpty(myRow("ProductTime").ToString.Trim) AndAlso myRow("HideTime") = False Then
            CType(e.Item.FindControl("ltlProductTimeLabel"), Literal).Text = _ucr.Content("KickoffLabel", _languageCode, True)
            CType(e.Item.FindControl("ltlProductTimeValue"), Literal).Text = myRow("ProductTime").ToString.Trim
            CType(e.Item.FindControl("plhProductTime"), PlaceHolder).Visible = True
        End If
        If Not String.IsNullOrEmpty(myRow("ProductEntryTime").ToString.Trim) And ModuleDefaults.ShowProductEntryTime Then
            CType(e.Item.FindControl("ltlProductEntryTimeLabel"), Literal).Text = _ucr.Content("ProductTimeLabel", _languageCode, True)
            CType(e.Item.FindControl("ltlProductEntryTimeValue"), Literal).Text = myRow("ProductEntryTime").ToString.Trim
            CType(e.Item.FindControl("plhProductEntryTime"), PlaceHolder).Visible = True
        End If
        Try
            Dim loyaltyPoints As Integer = CInt(myRow("ProductReqdLoyalityPoints").ToString.Trim)
            If loyaltyPoints > 0 Then
                CType(e.Item.FindControl("ltlLoyaltyPointsLabel"), Literal).Text = _ucr.Content("LoyaltyPointsLabel", _languageCode, True)
                CType(e.Item.FindControl("ltlLoyaltyPointsValue"), Literal).Text = myRow("ProductReqdLoyalityPoints").ToString.Trim
                CType(e.Item.FindControl("plhLoyaltyPoints"), PlaceHolder).Visible = True
            End If
        Catch ex As Exception
        End Try
        If _showSelectionBox = False Then
            CType(e.Item.FindControl("plhProductNoAccordianMain"), PlaceHolder).Visible = _showSelectionBox
            CType(e.Item.FindControl("plhProductNoAccordianTop"), PlaceHolder).Visible = True
            CType(e.Item.FindControl("plhProductNoAccordianBottom"), PlaceHolder).Visible = True
        End If
        If Not Talent.Common.Utilities.convertToBool(myRow("ProductAvailForSale").ToString().Trim()) Then
            priceBandSelectionControl.Visible = False
        End If
        loadAdditionalInformation(productCode, e.Item)
    End Sub

    Private Sub BindProductTypes_C(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        setControlVisibility(e.Item)
        SetProductAvailabilityText(e, "UnavailableMessage_MembershipProduct")
        Dim myRow As DataRow = CType(e.Item.DataItem, DataRowView).Row
        Dim productCode As String = CType(e.Item.FindControl("hfProductCode"), HiddenField).Value
        Dim plhEligibility As PlaceHolder = CType(e.Item.FindControl("plhEligibility"), PlaceHolder)
        Dim priceBandSelectionControl As UserControls_PriceBandSelection = CType(e.Item.FindControl("PriceBandSelection1"), UserControls_PriceBandSelection)
        Dim soldOut As Boolean = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(myRow("IsSoldOut"))
        If soldOut Then
            priceBandSelectionControl.Visible = False
            CType(e.Item.FindControl("plhSoldOut"), PlaceHolder).Visible = True
            If Not String.IsNullOrWhiteSpace(_ucr.Content("SoldOutHeaderText", _languageCode, True)) Then
                CType(e.Item.FindControl("plhSoldOutHeader"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("ltlSoldOutHeader"), Literal).Text = _ucr.Content("SoldOutHeaderText", _languageCode, True)
            End If
            CType(e.Item.FindControl("ltlSoldOut"), Literal).Text = _ucr.Content("SoldOutText", _languageCode, True)
        Else
            priceBandSelectionControl.Visible = True
            priceBandSelectionControl.ProductType = Me.ProductType
            priceBandSelectionControl.ProductSubType = Me.ProductSubType
            priceBandSelectionControl.ProductStadium = myRow("ProductStadium").ToString.Trim
            priceBandSelectionControl.PriceCode = myRow("PriceCode").ToString()
            priceBandSelectionControl.QuantityAvailable = 0
            priceBandSelectionControl.LoadPriceBandSelection()
        End If

        If Not String.IsNullOrEmpty(myRow("ProductMDTE08").ToString.Trim) AndAlso myRow("HideDate") = False Then
            CType(e.Item.FindControl("ltlProductDateLabel"), Literal).Text = _ucr.Content("ProductDateLabel", _languageCode, True)
            CType(e.Item.FindControl("ltlProductDateValue"), Literal).Text = GetFormattedProductDate(myRow("ProductMDTE08").ToString.Trim, myRow("ProductYear").ToString.Trim)
            CType(e.Item.FindControl("plhProductDate"), PlaceHolder).Visible = True
        End If

        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowLocationLabel")) Then
            If String.IsNullOrEmpty(myRow("location").ToString.Trim) Then
                CType(e.Item.FindControl("plhLocation"), PlaceHolder).Visible = False
            Else
                CType(e.Item.FindControl("plhLocation"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("ltlLocationLabel"), Literal).Text = _ucr.Content("LocationLabelText", _languageCode, True)
                CType(e.Item.FindControl("ltlLocationValue"), Literal).Text = myRow("location").ToString.Trim
            End If
        End If

        CType(e.Item.FindControl("CompetitionImage"), Image).Visible = False
        plhEligibility.Visible = False
        filterByDropDown.Visible = False
        sortByLabel.Visible = False

        'show controls relevant to this type
        If Not String.IsNullOrEmpty(myRow("ProductDescription").ToString.Trim) Then
            CType(e.Item.FindControl("ltlProductDescription"), Literal).Text = myRow("ProductDescription").ToString.Trim
            CType(e.Item.FindControl("plhProductDescription"), PlaceHolder).Visible = True
        End If
        If Not String.IsNullOrEmpty(myRow("ProductDescription2").ToString.Trim) Then
            CType(e.Item.FindControl("ltlProductCompetitionLabel"), Literal).Text = _ucr.Content("ProductCompetitionLabel", _languageCode, True)
            CType(e.Item.FindControl("ltlProductCompetitionValue"), Literal).Text = myRow("ProductDescription2").ToString.Trim
            CType(e.Item.FindControl("plhProductCompetition"), PlaceHolder).Visible = True
        End If
        If Not Talent.Common.Utilities.convertToBool(myRow("ProductAvailForSale").ToString().Trim()) Then
            priceBandSelectionControl.Visible = False
        End If
        loadAdditionalInformation(productCode, e.Item)
    End Sub

    Private Sub BindProductTypes_H(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        setControlVisibility(e.Item)
        SetProductAvailabilityText(e, "UnavailableMessage_HomeGameProduct")
        Dim myRow As DataRow = CType(e.Item.DataItem, DataRowView).Row
        Dim hdfProductAvailability As HiddenField = CType(e.Item.FindControl("hdfProductAvailability"), HiddenField)
        Dim multiStadiumDiv As PlaceHolder = TryCast(e.Item.FindControl("multiStadiumText"), PlaceHolder)
        Dim stadiumName As String = TDataObjects.StadiumSettings.TblStadiums.GetStadiumNameByStadiumCode(CType(e.Item.FindControl("hfProductStadium"), HiddenField).Value, _ucr.BusinessUnit)
        Dim soldOut As Boolean = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(myRow("IsSoldOut"))
        Dim productLinked As String = myRow("ProductLinkedToHospitality").ToString
        Dim hospitalityNavigateUrl As New StringBuilder
        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowLocationLabel")) Then
            If String.IsNullOrEmpty(myRow("location").ToString.Trim) Then
                CType(e.Item.FindControl("plhLocation"), PlaceHolder).Visible = False
            Else
                CType(e.Item.FindControl("plhLocation"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("ltlLocationLabel"), Literal).Text = _ucr.Content("LocationLabelText", _languageCode, True)
                CType(e.Item.FindControl("ltlLocationValue"), Literal).Text = myRow("location").ToString.Trim
            End If
        End If
        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowStadiumDescriptionLabel")) Then
            If String.IsNullOrEmpty(myRow("ProductStadiumDescription").ToString.Trim) Then
                CType(e.Item.FindControl("plhStadiumDescription"), PlaceHolder).Visible = False
            Else
                CType(e.Item.FindControl("plhStadiumDescription"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("ltlStadiumDescriptionLabel"), Literal).Text = _ucr.Content("StadiumDescriptionLabelText", _languageCode, True)
                CType(e.Item.FindControl("ltlStadiumDescriptionValue"), Literal).Text = myRow("ProductStadiumDescription").ToString.Trim
            End If
        End If

        If hdfProductAvailability.Value.ToUpper <> "N" Then
            hospitalityNavigateUrl.Append("~/PagesPublic/Hospitality/HospitalityFixturePackages.aspx?product=")
            hospitalityNavigateUrl.Append(myRow("CorporateHospitalityProductCode"))
            If soldOut Then
                CType(e.Item.FindControl("plhSoldOut"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("ltlSoldOut"), Literal).Text = _ucr.Content("SoldOutText", _languageCode, True)
                If Not String.IsNullOrWhiteSpace(_ucr.Content("SoldOutHeaderText", _languageCode, True)) Then
                    CType(e.Item.FindControl("plhSoldOutHeader"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("ltlSoldOutHeader"), Literal).Text = _ucr.Content("SoldOutHeaderText", _languageCode, True)
                End If

                ' Display upgrade to hospitality link if configured and product is solout.
                If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowUpgradeToHospitalityWhenProductSoldOut")) AndAlso Not String.IsNullOrEmpty(myRow("CorporateHospitalityProductCode")) AndAlso (productLinked <> "N") Then
                    CType(e.Item.FindControl("plhUpgradeToHospitality"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("hplUpgradeToHospitality"), HyperLink).Text = _ucr.Content("UpgradeToHospitalityLinkTextForProductList", _languageCode, True)
                    CType(e.Item.FindControl("hplUpgradeToHospitality"), HyperLink).NavigateUrl = hospitalityNavigateUrl.ToString()
                    CType(e.Item.FindControl("hplSelectSeats"), HyperLink).Text = _ucr.Content("SeatSelectionLinkText", _languageCode, True)
                    CType(e.Item.FindControl("hplSelectSeats"), HyperLink).NavigateUrl = setVisualSeatSelectionUrl(String.Empty, Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductStadium")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductCode")),
                                                           Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("CampaignCode")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductType")),
                                                           Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductSubType")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductHomeAsAway")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("RestrictGraphical")))
                End If
            Else
                ' Display upgrade to hospitality link if configured and product not solout
                If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowUpgradeToHospitalityOnProductList")) AndAlso Not String.IsNullOrEmpty(myRow("CorporateHospitalityProductCode")) AndAlso (productLinked <> "N") Then
                    CType(e.Item.FindControl("plhUpgradeToHospitality"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("hplUpgradeToHospitality"), HyperLink).Text = _ucr.Content("UpgradeToHospitalityLinkTextForProductList", _languageCode, True)
                    CType(e.Item.FindControl("hplUpgradeToHospitality"), HyperLink).NavigateUrl = hospitalityNavigateUrl.ToString()
                    CType(e.Item.FindControl("hplSelectSeats"), HyperLink).Text = _ucr.Content("SeatSelectionLinkText", _languageCode, True)
                    CType(e.Item.FindControl("hplSelectSeats"), HyperLink).NavigateUrl = setVisualSeatSelectionUrl(String.Empty, Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductStadium")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductCode")),
                                                           Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("CampaignCode")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductType")),
                                                           Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductSubType")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductHomeAsAway")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("RestrictGraphical")))
                End If

            End If
        End If
        'show controls relevant to this type
        If Not String.IsNullOrEmpty(myRow("ProductDescription").ToString.Trim) Then
            CType(e.Item.FindControl("ltlProductDescription"), Literal).Text = myRow("ProductDescription").ToString.Trim
            CType(e.Item.FindControl("plhProductDescription"), PlaceHolder).Visible = True
        End If
        If Not String.IsNullOrEmpty(myRow("ProductDescription2").ToString.Trim) Then
            CType(e.Item.FindControl("ltlProductCompetitionLabel"), Literal).Text = _ucr.Content("ProductCompetitionLabel", _languageCode, True)
            CType(e.Item.FindControl("ltlProductCompetitionValue"), Literal).Text = myRow("ProductDescription2").ToString.Trim
            CType(e.Item.FindControl("plhProductCompetition"), PlaceHolder).Visible = True
        End If
        If Not String.IsNullOrEmpty(myRow("ProductMDTE08").ToString.Trim) AndAlso myRow("HideDate") = False Then
            CType(e.Item.FindControl("ltlProductDateLabel"), Literal).Text = _ucr.Content("ProductDateLabel", _languageCode, True)
            CType(e.Item.FindControl("ltlProductDateValue"), Literal).Text = GetFormattedProductDate(myRow("ProductMDTE08").ToString.Trim, myRow("ProductYear").ToString.Trim)
            CType(e.Item.FindControl("plhProductDate"), PlaceHolder).Visible = True
        End If
        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowTimeLabel")) Then
            If Not String.IsNullOrEmpty(myRow("ProductTime").ToString.Trim) AndAlso myRow("HideTime") = False Then
                CType(e.Item.FindControl("ltlProductTimeLabel"), Literal).Text = _ucr.Content("KickoffLabel", _languageCode, True)
                CType(e.Item.FindControl("ltlProductTimeValue"), Literal).Text = myRow("ProductTime").ToString.Trim
                CType(e.Item.FindControl("plhProductTime"), PlaceHolder).Visible = True
            End If
        End If
        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(myRow("IsProductBundle")) And ModuleDefaults.ShowBundleDateAsRange Then
            If Not String.IsNullOrEmpty(myRow("BundleStartDate").ToString.Trim) And Not myRow("BundleStartDate").ToString.Trim = "0000000" Then
                CType(e.Item.FindControl("ltlProductDateValue"), Literal).Text = GetFormattedProductDate(myRow("BundleStartDate").ToString.Trim, myRow("ProductYear").ToString.Trim) + " - " + GetFormattedProductDate(myRow("BundleEndDate").ToString.Trim, myRow("ProductYear").ToString.Trim)
                CType(e.Item.FindControl("plhProductDate"), PlaceHolder).Visible = True
            End If
            CType(e.Item.FindControl("plhProductTime"), PlaceHolder).Visible = False
        End If
        If Not String.IsNullOrEmpty(myRow("ProductEntryTime").ToString.Trim) And ModuleDefaults.ShowProductEntryTime Then
            CType(e.Item.FindControl("ltlProductEntryTimeLabel"), Literal).Text = _ucr.Content("ProductTimeLabel", _languageCode, True)
            CType(e.Item.FindControl("ltlProductEntryTimeValue"), Literal).Text = myRow("ProductEntryTime").ToString.Trim
            CType(e.Item.FindControl("plhProductEntryTime"), PlaceHolder).Visible = True
        End If
        Try
            Dim loyaltyPoints As Integer = CInt(myRow("ProductReqdLoyalityPoints").ToString.Trim)
            If loyaltyPoints > 0 Then
                CType(e.Item.FindControl("ltlLoyaltyPointsLabel"), Literal).Text = _ucr.Content("LoyaltyPointsLabel", _languageCode, True)
                CType(e.Item.FindControl("ltlLoyaltyPointsValue"), Literal).Text = myRow("ProductReqdLoyalityPoints").ToString.Trim
                CType(e.Item.FindControl("plhLoyaltyPoints"), PlaceHolder).Visible = True
            End If
        Catch ex As Exception
        End Try

        'Display the multi stadium text where applicable
        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowMultiStadiumPerProductTextForHomeGames")) Then
            TryCast(e.Item.FindControl("lblMultiStadiumPerProductText"), Label).Text = _ucr.Content("MultiStadiumPerProductTextForHomeGames", _languageCode, True).Replace("<<STADIUM_NAME>>", stadiumName)
            multiStadiumDiv.Visible = True
        Else
            multiStadiumDiv.Visible = False
        End If
        If _showSelectionBox = False Then
            If Talent.Common.Utilities.convertToBool(myRow("ProductAvailForSale").ToString().Trim()) Then
                CType(e.Item.FindControl("plhProductNoAccordianTop"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("plhProductNoAccordianBottom"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("plhProductNoAccordianMain"), PlaceHolder).Visible = False
            Else
                CType(e.Item.FindControl("plhProductNoAccordianTop"), PlaceHolder).Visible = False
                CType(e.Item.FindControl("plhProductNoAccordianBottom"), PlaceHolder).Visible = False
                CType(e.Item.FindControl("plhProductNoAccordianMain"), PlaceHolder).Visible = True
            End If
        End If
    End Sub

    Private Sub BindProductTypes_S(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        setControlVisibility(e.Item)
        SetProductAvailabilityText(e, "UnavailableMessage_SeasonTicketProduct")
        Dim myRow As DataRow = CType(e.Item.DataItem, DataRowView).Row
        Dim hdfProductAvailability As HiddenField = CType(e.Item.FindControl("hdfProductAvailability"), HiddenField)
        Dim plhEligibility As PlaceHolder = CType(e.Item.FindControl("plhEligibility"), PlaceHolder)
        Dim multiStadiumDiv As PlaceHolder = TryCast(e.Item.FindControl("multiStadiumText"), PlaceHolder)
        Dim stadiumName As String = TDataObjects.StadiumSettings.TblStadiums.GetStadiumNameByStadiumCode(CType(e.Item.FindControl("hfProductStadium"), HiddenField).Value, _ucr.BusinessUnit)
        Dim soldOut As Boolean = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(myRow("IsSoldOut"))
        Dim hospitalityNavigateUrl As New StringBuilder
        If hdfProductAvailability.Value.ToUpper <> "N" Then
            hospitalityNavigateUrl.Append("~/PagesPublic/Hospitality/HospitalityFixturePackages.aspx?product=")
            hospitalityNavigateUrl.Append(myRow("CorporateHospitalityProductCode"))
            If soldOut Then
                If Not String.IsNullOrWhiteSpace(_ucr.Content("SoldOutHeaderText", _languageCode, True)) Then
                    CType(e.Item.FindControl("plhSoldOutHeader"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("ltlSoldOutHeader"), Literal).Text = _ucr.Content("SoldOutHeaderText", _languageCode, True)
                End If
                CType(e.Item.FindControl("plhSoldOut"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("ltlSoldOut"), Literal).Text = _ucr.Content("SoldOutText", _languageCode, True)
                ' Display upgrade to hospitality link if configured and product is solout.
                If Not String.IsNullOrEmpty(myRow("CorporateHospitalityProductCode")) AndAlso Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowUpgradeToHospitalityWhenProductSoldOut")) Then
                    CType(e.Item.FindControl("plhUpgradeToHospitality"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("hplUpgradeToHospitality"), HyperLink).Text = _ucr.Content("UpgradeToHospitalityLinkTextForProductList", _languageCode, True)
                    CType(e.Item.FindControl("hplUpgradeToHospitality"), HyperLink).NavigateUrl = hospitalityNavigateUrl.ToString()
                    CType(e.Item.FindControl("hplSelectSeats"), HyperLink).Text = _ucr.Content("SeatSelectionLinkText", _languageCode, True)
                    CType(e.Item.FindControl("hplSelectSeats"), HyperLink).NavigateUrl = setVisualSeatSelectionUrl(String.Empty, Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductStadium")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductCode")),
                                                                                         Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("CampaignCode")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductType")),
                                                                                         Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductSubType")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductHomeAsAway")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("RestrictGraphical")))
                End If
            Else
                ' Display upgrade to hospitality link if configured and product not solout
                If Not String.IsNullOrEmpty(myRow("CorporateHospitalityProductCode")) AndAlso Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowUpgradeToHospitalityOnProductList")) Then
                    CType(e.Item.FindControl("plhUpgradeToHospitality"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("hplUpgradeToHospitality"), HyperLink).Text = _ucr.Content("UpgradeToHospitalityLinkTextForProductList", _languageCode, True)
                    CType(e.Item.FindControl("hplUpgradeToHospitality"), HyperLink).NavigateUrl = hospitalityNavigateUrl.ToString()
                    CType(e.Item.FindControl("hplSelectSeats"), HyperLink).Text = _ucr.Content("SeatSelectionLinkText", _languageCode, True)
                    CType(e.Item.FindControl("hplSelectSeats"), HyperLink).NavigateUrl = setVisualSeatSelectionUrl(String.Empty, Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductStadium")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductCode")),
                                                                                         Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("CampaignCode")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductType")),
                                                                                         Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductSubType")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("ProductHomeAsAway")), Talent.eCommerce.Utilities.CheckForDBNull_String(myRow("RestrictGraphical")))
                End If

            End If

        Else
            plhEligibility.Visible = False
        End If

        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowLocationLabel")) Then
            If String.IsNullOrEmpty(myRow("location").ToString.Trim) Then
                CType(e.Item.FindControl("plhLocation"), PlaceHolder).Visible = False
            Else
                CType(e.Item.FindControl("plhLocation"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("ltlLocationLabel"), Literal).Text = _ucr.Content("LocationLabelText", _languageCode, True)
                CType(e.Item.FindControl("ltlLocationValue"), Literal).Text = myRow("location").ToString.Trim
            End If
        End If
        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowStadiumDescriptionLabel")) Then
            If String.IsNullOrEmpty(myRow("ProductStadiumDescription").ToString.Trim) Then
                CType(e.Item.FindControl("plhStadiumDescription"), PlaceHolder).Visible = False
            Else
                CType(e.Item.FindControl("plhStadiumDescription"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("ltlStadiumDescriptionLabel"), Literal).Text = _ucr.Content("StadiumDescriptionLabelText", _languageCode, True)
                CType(e.Item.FindControl("ltlStadiumDescriptionValue"), Literal).Text = myRow("ProductStadiumDescription").ToString.Trim
            End If
        End If
        'show controls relevant to this type
        If Not String.IsNullOrEmpty(myRow("ProductDescription").ToString.Trim) Then
            CType(e.Item.FindControl("ltlProductDescription"), Literal).Text = myRow("ProductDescription").ToString.Trim
            CType(e.Item.FindControl("plhProductDescription"), PlaceHolder).Visible = True
        End If
        If Not String.IsNullOrEmpty(myRow("ProductDescription2").ToString.Trim) Then
            CType(e.Item.FindControl("ltlProductCompetitionValue"), Literal).Text = _ucr.Content("ProductCompetitionLabel", _languageCode, True)
            CType(e.Item.FindControl("ltlProductCompetitionValue"), Literal).Text = myRow("ProductDescription2").ToString.Trim
            CType(e.Item.FindControl("plhProductCompetition"), PlaceHolder).Visible = True
        End If
        Try
            Dim loyaltyPoints As Integer = CInt(myRow("ProductReqdLoyalityPoints").ToString.Trim)
            If loyaltyPoints > 0 Then
                CType(e.Item.FindControl("ltlLoyaltyPointsLabel"), Literal).Text = _ucr.Content("LoyaltyPointsLabel", _languageCode, True)
                CType(e.Item.FindControl("ltlLoyaltyPointsValue"), Literal).Text = myRow("ProductReqdLoyalityPoints").ToString.Trim
                CType(e.Item.FindControl("plhLoyaltyPoints"), PlaceHolder).Visible = True
            End If
        Catch ex As Exception
        End Try

        'Display the multi stadium text where applicable
        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowMultiStadiumPerProductTextForSeasonTickets")) Then
            TryCast(e.Item.FindControl("lblMultiStadiumPerProductText"), Label).Text = _ucr.Content("MultiStadiumPerProductTextForSeasonTickets", _languageCode, True).Replace("<<STADIUM_NAME>>", stadiumName)
            multiStadiumDiv.Visible = True
        Else
            multiStadiumDiv.Visible = False
        End If

        If _showSelectionBox = False Then
            If Talent.Common.Utilities.convertToBool(myRow("ProductAvailForSale").ToString().Trim()) Then
                CType(e.Item.FindControl("plhProductNoAccordianTop"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("plhProductNoAccordianBottom"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("plhProductNoAccordianMain"), PlaceHolder).Visible = False
            Else
                CType(e.Item.FindControl("plhProductNoAccordianTop"), PlaceHolder).Visible = False
                CType(e.Item.FindControl("plhProductNoAccordianBottom"), PlaceHolder).Visible = False
                CType(e.Item.FindControl("plhProductNoAccordianMain"), PlaceHolder).Visible = True
            End If
        End If
    End Sub

    Private Sub BindProductTypes_T_E(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        Dim prod_availabilityPercentage As Integer = 0
        Dim prod_availability As Integer = 0
        Dim prod_capacity As Integer = 0
        Dim prod_returned As Integer = 0
        Dim prod_sold As Integer = 0
        Dim prod_reserved As Integer = 0
        Dim prod_booked As Integer = 0
        Dim prod_unavailable As Integer = 0

        setControlVisibility(e.Item)
        If _ProductType = "T" Then
            SetProductAvailabilityText(e, "UnavailableMessage_TravelProduct")
        ElseIf _ProductType = "E" Then
            SetProductAvailabilityText(e, "UnavailableMessage_EventProduct")
        End If

        Dim myRow As DataRow = CType(e.Item.DataItem, DataRowView).Row
        Dim productCode As String = CType(e.Item.FindControl("hfProductCode"), HiddenField).Value
        Dim priceBandSelectionControl As UserControls_PriceBandSelection = CType(e.Item.FindControl("PriceBandSelection1"), UserControls_PriceBandSelection)
        priceBandSelectionControl.ProductType = Me.ProductType
        priceBandSelectionControl.ProductSubType = Me.ProductSubType
        priceBandSelectionControl.ProductStadium = myRow("ProductStadium").ToString.Trim

        'If we are showing travel as a single item with a DDL for sub-item then the capacity needs to be the total.
        If ModuleDefaults.ShowTravelAsDDL AndAlso _ProductType = "T" Then
            _productDetailHelper.getProductAvailabilityFigures_TAE(productCode, ProductType, String.Empty, String.Empty, prod_availability, prod_availabilityPercentage,
                                              prod_capacity, prod_returned, prod_sold, prod_reserved, prod_booked, prod_unavailable)
            priceBandSelectionControl.QuantityAvailable = prod_availability
            priceBandSelectionControl.DSProductDetail = _ds1

            ' Travel Products shown as seperate items - availability, capacity are retrieved for the Travel Item Code.
            ' Event Products are always shown as seperate items - ProductDetails Code if not used in this case.
        Else
            _productDetailHelper.getProductAvailabilityFigures_TAE(productCode, ProductType, String.Empty, myRow("ProductDetailCode").ToString(), prod_availability, prod_availabilityPercentage,
                                              prod_capacity, prod_returned, prod_sold, prod_reserved, prod_booked, prod_unavailable)
            priceBandSelectionControl.QuantityAvailable = prod_availability
        End If

        ' There is a new default in product setup which sets the product as sold out when there are still tickets available 
        If myRow("ProductSetAsSoldOut") Then
            priceBandSelectionControl.QuantityAvailable = 0
            prod_availability = 0
            prod_availabilityPercentage = 0
        End If

        If priceBandSelectionControl.QuantityAvailable <= 0 Then
            CType(e.Item.FindControl("plhSoldOutHeader"), PlaceHolder).Visible = True
            CType(e.Item.FindControl("ltlSoldOutHeader"), Literal).Text = _ucr.Content("SoldOutHeaderText", _languageCode, True)
            CType(e.Item.FindControl("pnlAccordian"), Panel).CssClass += DisableAccordion(True)
            priceBandSelectionControl.Visible = False
        Else
            CType(e.Item.FindControl("pnlAccordian"), Panel).CssClass = EnableAccordion(CType(e.Item.FindControl("pnlAccordian"), Panel).CssClass)
            priceBandSelectionControl.LoadPriceBandSelection()
        End If

        'Show any controls that are related to this product type
        Try
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowAvailabilityLabel")) Then
                CType(e.Item.FindControl("plhAvailability"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("ltlAvailabilityLabel"), Literal).Text = _ucr.Content("AvailabilityLabelText", _languageCode, True)
                Dim lblAvailabilityValue As New Label

                setAvailabilityLabel(lblAvailabilityValue, prod_availabilityPercentage, myRow("ProductStadium").ToString().Trim())

                Dim productTypeMask As String
                If ProductType = GlobalConstants.TRAVELPRODUCTTYPE Then
                    productTypeMask = _ucr.Content("travelAvailabilityMaskLabel", _languageCode, True)
                Else
                    productTypeMask = _ucr.Content("eventAvailabilityMaskLabel", _languageCode, True)
                End If
                lblAvailabilityValue.Text = _productDetailHelper.setAvailabilityMask(productTypeMask, lblAvailabilityValue.Text, _ProductType, prod_availability.ToString,
                                                                prod_availabilityPercentage.ToString, prod_capacity.ToString, prod_returned.ToString,
                                                                prod_sold.ToString, prod_reserved.ToString, prod_booked.ToString, prod_unavailable.ToString, "")

                CType(e.Item.FindControl("lblAvailabilityValue"), Label).BackColor = lblAvailabilityValue.BackColor
                CType(e.Item.FindControl("lblAvailabilityValue"), Label).Text = lblAvailabilityValue.Text
                CType(e.Item.FindControl("lblAvailabilityValue"), Label).CssClass = lblAvailabilityValue.CssClass
            End If
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowAgeLabel")) Then
                Dim ageRange As String = String.Empty
                ageRange = myRow("ageRangeFrm").ToString.Trim & _ucr.Content("AgeRangeDividerText", _languageCode, True) & myRow("ageRangeTo").ToString.Trim
                If String.IsNullOrEmpty(ageRange) Then
                    CType(e.Item.FindControl("plhAge"), PlaceHolder).Visible = False
                Else
                    CType(e.Item.FindControl("plhAge"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("ltlAgeLabel"), Literal).Text = _ucr.Content("AgeLabelText", _languageCode, True)
                    CType(e.Item.FindControl("ltlAgeValue"), Literal).Text = ageRange
                End If
            End If
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowDurationLabel")) Then
                If String.IsNullOrEmpty(myRow("duration").ToString.Trim) Then
                    CType(e.Item.FindControl("plhDuration"), PlaceHolder).Visible = False
                Else
                    CType(e.Item.FindControl("plhDuration"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("ltlDurationLabel"), Literal).Text = _ucr.Content("DurationLabelText", _languageCode, True)
                    CType(e.Item.FindControl("ltlDurationValue"), Literal).Text = myRow("duration").ToString.Trim
                End If
            End If
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowLocationLabel")) Then
                If String.IsNullOrEmpty(myRow("location").ToString.Trim) Then
                    CType(e.Item.FindControl("plhLocation"), PlaceHolder).Visible = False
                Else
                    CType(e.Item.FindControl("plhLocation"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("ltlLocationLabel"), Literal).Text = _ucr.Content("LocationLabelText", _languageCode, True)
                    CType(e.Item.FindControl("ltlLocationValue"), Literal).Text = myRow("location").ToString.Trim
                End If
            End If
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowTimeLabel")) Then
                If Not String.IsNullOrEmpty(myRow("ProductTime").ToString.Trim) AndAlso myRow("HideTime") = False Then
                    CType(e.Item.FindControl("ltlProductTimeLabel"), Literal).Text = _ucr.Content("KickoffLabel", _languageCode, True)
                    CType(e.Item.FindControl("ltlProductTimeValue"), Literal).Text = myRow("ProductTime").ToString.Trim
                    CType(e.Item.FindControl("plhProductTime"), PlaceHolder).Visible = True
                End If
            End If

            'show controls relevant to this type
            If Not String.IsNullOrEmpty(myRow("ProductDescription").ToString.Trim) Then
                CType(e.Item.FindControl("ltlProductDescription"), Literal).Text = myRow("ProductDescription").ToString.Trim
                CType(e.Item.FindControl("plhProductDescription"), PlaceHolder).Visible = True
            End If
            If Not String.IsNullOrEmpty(myRow("ProductDescription2").ToString.Trim) Then
                CType(e.Item.FindControl("ltlProductCompetitionLabel"), Literal).Text = _ucr.Content("ProductCompetitionLabel", _languageCode, True)
                CType(e.Item.FindControl("ltlProductCompetitionValue"), Literal).Text = myRow("ProductDescription2").ToString.Trim
                CType(e.Item.FindControl("plhProductCompetition"), PlaceHolder).Visible = True
            End If
            If Not String.IsNullOrEmpty(myRow("ProductMDTE08").ToString.Trim) AndAlso myRow("HideDate") = False Then
                CType(e.Item.FindControl("ltlProductDateLabel"), Literal).Text = _ucr.Content("ProductDateLabel", _languageCode, True)
                CType(e.Item.FindControl("ltlProductDateValue"), Literal).Text = GetFormattedProductDate(myRow("ProductMDTE08").ToString.Trim, myRow("ProductYear").ToString.Trim)
                CType(e.Item.FindControl("plhProductDate"), PlaceHolder).Visible = True
            End If
            If Not String.IsNullOrEmpty(myRow("ProductEntryTime").ToString.Trim) And ModuleDefaults.ShowProductEntryTime Then
                CType(e.Item.FindControl("ltlProductEntryTimeLabel"), Literal).Text = _ucr.Content("ProductTimeLabel", _languageCode, True)
                CType(e.Item.FindControl("ltlProductEntryTimeValue"), Literal).Text = myRow("ProductEntryTime").ToString.Trim
                CType(e.Item.FindControl("plhProductEntryTime"), PlaceHolder).Visible = True
            End If
            If _showSelectionBox = False Then
                CType(e.Item.FindControl("plhProductNoAccordianMain"), PlaceHolder).Visible = _showSelectionBox
                CType(e.Item.FindControl("plhProductNoAccordianTop"), PlaceHolder).Visible = True
                CType(e.Item.FindControl("plhProductNoAccordianBottom"), PlaceHolder).Visible = True
            End If
            If Not Talent.Common.Utilities.convertToBool(myRow("ProductAvailForSale").ToString().Trim()) Then
                priceBandSelectionControl.Visible = False
            End If
        Catch ex As Exception
        End Try
        loadAdditionalInformation(productCode, e.Item)

        ' Check if need to show travel dropdown (in priceBandSelection)
        If ModuleDefaults.ShowTravelAsDDL Then CType(e.Item.FindControl("plhProductCompetition"), PlaceHolder).Visible = False
    End Sub

    Private Sub BindProductTypes_CH(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        setControlVisibility(e.Item)
        SetProductAvailabilityText(e, "UnavailableMessage_MDHProduct")
        Dim myRow As DataRow = CType(e.Item.DataItem, DataRowView).Row
        Dim productCode As String = CType(e.Item.FindControl("hfProductCode"), HiddenField).Value
        Dim multiStadiumDiv As PlaceHolder = TryCast(e.Item.FindControl("multiStadiumText"), PlaceHolder)
        Dim stadiumName As String = TDataObjects.StadiumSettings.TblStadiums.GetStadiumNameByStadiumCode(CType(e.Item.FindControl("hfProductStadium"), HiddenField).Value, _ucr.BusinessUnit)
        Dim MDHUserControl As UserControls_MatchDayHospitality = CType(e.Item.FindControl("MatchDayHospitality1"), UserControls_MatchDayHospitality)
        Dim plhUnavailableBox As PlaceHolder = CType(e.Item.FindControl("plhUnavailableBox"), PlaceHolder)
        Dim ltlUnavailableBoxText As Literal = CType(e.Item.FindControl("ltlUnavailableBoxText"), Literal)
        MDHUserControl.ProductCode = myRow("ProductCode").ToString.Trim
        If Not MDHUserControl.LoadPackageDetails() Then
            plhUnavailableBox.Visible = True
            ltlUnavailableBoxText.Text = _ucr.Content("UnavailableMessage_MDHProduct", _languageCode, True)
        End If

        'show controls relevant to this type
        If Not String.IsNullOrEmpty(myRow("ProductDescription").ToString.Trim) Then
            CType(e.Item.FindControl("ltlProductDescription"), Literal).Text = myRow("ProductDescription").ToString.Trim
            CType(e.Item.FindControl("plhProductDescription"), PlaceHolder).Visible = True
        End If
        If Not String.IsNullOrEmpty(myRow("ProductDescription2").ToString.Trim) Then
            CType(e.Item.FindControl("ltlProductCompetitionLabel"), Literal).Text = _ucr.Content("ProductCompetitionLabel", _languageCode, True)
            CType(e.Item.FindControl("ltlProductCompetitionValue"), Literal).Text = myRow("ProductDescription2").ToString.Trim
            CType(e.Item.FindControl("plhProductCompetition"), PlaceHolder).Visible = True
        End If
        If Not String.IsNullOrEmpty(myRow("ProductMDTE08").ToString.Trim) AndAlso myRow("HideDate") = False Then
            CType(e.Item.FindControl("ltlProductDateLabel"), Literal).Text = _ucr.Content("ProductDateLabel", _languageCode, True)
            CType(e.Item.FindControl("ltlProductDateValue"), Literal).Text = GetFormattedProductDate(myRow("ProductMDTE08").ToString.Trim, myRow("ProductYear").ToString.Trim)
            CType(e.Item.FindControl("plhProductDate"), PlaceHolder).Visible = True
        End If
        If Not String.IsNullOrEmpty(myRow("ProductTime").ToString.Trim) AndAlso myRow("HideTime") = False Then
            CType(e.Item.FindControl("ltlProductTimeLabel"), Literal).Text = _ucr.Content("KickoffLabel", _languageCode, True)
            CType(e.Item.FindControl("ltlProductTimeValue"), Literal).Text = myRow("ProductTime").ToString.Trim
            CType(e.Item.FindControl("plhProductTime"), PlaceHolder).Visible = True
        End If
        If Not String.IsNullOrEmpty(myRow("ProductEntryTime").ToString.Trim) And ModuleDefaults.ShowProductEntryTime Then
            CType(e.Item.FindControl("ltlProductEntryTimeLabel"), Literal).Text = _ucr.Content("ProductTimeLabel", _languageCode, True)
            CType(e.Item.FindControl("ltlProductEntryTimeValue"), Literal).Text = myRow("ProductEntryTime").ToString.Trim
            CType(e.Item.FindControl("plhProductEntryTime"), PlaceHolder).Visible = True
        End If
        Try
            Dim loyaltyPoints As Integer = CInt(myRow("ProductReqdLoyalityPoints").ToString.Trim)
            If loyaltyPoints > 0 Then
                CType(e.Item.FindControl("ltlLoyaltyPointsLabel"), Literal).Text = _ucr.Content("LoyaltyPointsLabel", _languageCode, True)
                CType(e.Item.FindControl("ltlLoyaltyPointsValue"), Literal).Text = myRow("ProductReqdLoyalityPoints").ToString.Trim
                CType(e.Item.FindControl("plhLoyaltyPoints"), PlaceHolder).Visible = True
            End If
        Catch ex As Exception
        End Try
        loadAdditionalInformation(productCode, e.Item)
    End Sub

    Private Sub BindProductTypes_P(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        setControlVisibility(e.Item)
        Dim myRow As DataRow = CType(e.Item.DataItem, DataRowView).Row
        Dim plhEligibility As PlaceHolder = CType(e.Item.FindControl("plhEligibility"), PlaceHolder)
        Dim productCode As String = CType(e.Item.FindControl("hfProductCode"), HiddenField).Value
        plhEligibility.Visible = False

        'show controls relevant to this type
        If Not String.IsNullOrEmpty(myRow("ProductDescription").ToString.Trim) Then
            CType(e.Item.FindControl("ltlProductDescription"), Literal).Text = myRow("ProductDescription").ToString.Trim
            CType(e.Item.FindControl("plhProductDescription"), PlaceHolder).Visible = True
        End If

        If AmendPPSEnrolmentInUse Then
            BindAmendPPSEnrollment(e)
        Else
            Dim inst As New StringBuilder
            Dim processedStands As New ArrayList
            Dim li As ListItem
            Dim myPps As UserControls_TicketingPPS = CType(e.Item.FindControl("TicketingPPS1"), UserControls_TicketingPPS)
            myPps.setHiddenFields()
            With myPps.SeasonTicketsList
                For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                    If UCase(tbi.PRODUCT_TYPE.Trim) = "S" AndAlso UCase(tbi.Product.Trim) = e.Item.DataItem("RelatingSeasonTicket") Then
                        li = New ListItem(tbi.SEAT, tbi.SEAT)
                        .Items.Add(li)

                        ' Construct the new bespoke instructions text
                        If processedStands.IndexOf(tbi.SEAT.Substring(0, 3)) < 0 Then

                            ' Query the database for the bespoke instruction text
                            inst.Append(_ucr.Content("InstructionText" & myPps.ProductCode & tbi.SEAT.Substring(0, 7), _languageCode, True))
                            inst.Append(_ucr.Content("InstructionText" & myPps.ProductCode & tbi.SEAT.Substring(0, 3) & "*ALL", _languageCode, True))

                            ' Query the database for the bespoke instruction text - Bondholder specific
                            inst.Append(_ucr.Content("InstructionText" & myPps.ProductCode & tbi.SEAT.Substring(0, 7) & Profile.User.Details.BOND_HOLDER.ToString, _languageCode, True))
                            inst.Append(_ucr.Content("InstructionText" & myPps.ProductCode & tbi.SEAT.Substring(0, 3) & "*ALL" & Profile.User.Details.BOND_HOLDER.ToString, _languageCode, True))

                            ' Se the stand as processed
                            processedStands.Add(tbi.SEAT.Substring(0, 3))

                        End If
                    End If
                Next

                ' Set the bespoke stand and area instruction text
                If Not inst.ToString.Trim.Equals("") Then
                    CType(e.Item.FindControl("lblInstructionText"), Label).Text = inst.ToString
                    CType(e.Item.FindControl("plhInstructionText"), Panel).Visible = True
                End If

                'Display any errors
                For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                    If UCase(myPps.ProductCode.Trim) = UCase(tbi.Product.Trim) _
                        AndAlso UCase(tbi.PRODUCT_TYPE.Trim) = "P" _
                            AndAlso tbi.STOCK_ERROR_CODE.Trim <> "" Then
                        'Add the error message
                        myPps.Errors.Items.Add(TicketingBasketErrors(_errMsg, tbi))
                    End If
                Next

                'Set the attributes of the checkboxes
                For Each pli As ListItem In .Items

                    'tick the items that are already in the basket
                    For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
                        If UCase(myPps.ProductCode) = UCase(tbi.Product) _
                            AndAlso UCase(tbi.PRODUCT_TYPE.Trim) = "P" _
                                AndAlso UCase(tbi.SEAT.Trim) = UCase(pli.Text.Trim) Then
                            pli.Selected = True
                        End If
                    Next

                    'Is this customer entitled to this scheme for free.  The only schemes supported by
                    'this at the moment is season ticket schemes
                    If e.Item.DataItem("PPSSchemeType") = "S" Then
                        If UCase(pli.Text.Substring(0, 7)) = UCase(PPSFreeAreas.Substring(0, 7)) Or
                            UCase(pli.Text.Substring(0, 7)) = UCase(PPSFreeAreas.Substring(7, 7)) Or
                            UCase(pli.Text.Substring(0, 7)) = UCase(PPSFreeAreas.Substring(14, 7)) Or
                            UCase(pli.Text.Substring(0, 7)) = UCase(PPSFreeAreas.Substring(21, 7)) Or
                            UCase(pli.Text.Substring(0, 7)) = UCase(PPSFreeAreas.Substring(28, 7)) Then
                            pli.Selected = True
                            pli.Enabled = False
                        End If
                    End If
                Next

                'display an error if there are no season tickets in the basket
                If .Items.Count = 0 Then
                    Dim eli As New ListItem(_ucr.Content("NoSeasonTicketsForSchemesError", _languageCode, True))
                    If Not errorlist.Items.Contains(eli) Then
                        errorlist.Items.Add(eli)
                    End If
                End If
            End With
        End If
        loadAdditionalInformation(productCode, e.Item)
    End Sub

    Private Sub BindAmendPPSEnrollment(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        Dim productCode As String = CType(e.Item.FindControl("hfProductCode"), HiddenField).Value
        Dim amendPPSEnrolmentScheme As New TalentPPS
        Dim dEPPSEnrolmentScheme As New DEPPSEnrolmentScheme
        Dim deSettings As New DESettings
        deSettings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        deSettings.BusinessUnit = TalentCache.GetBusinessUnit()
        deSettings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
        deSettings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
        deSettings.OriginatingSourceCode = "W"
        amendPPSEnrolmentScheme.Settings = deSettings
        amendPPSEnrolmentScheme.DEPPSEnrolmentScheme = dEPPSEnrolmentScheme
        amendPPSEnrolmentScheme.DEPPSEnrolmentScheme.AmendPPSEnrolmentIgnoreFF = ModuleDefaults.AmendPPSEnrolmentIgnoreFF
        amendPPSEnrolmentScheme.DEPPSEnrolmentScheme.CustomerNumber = Profile.User.Details.LoginID
        amendPPSEnrolmentScheme.DEPPSEnrolmentScheme.ProductCode = productCode
        amendPPSEnrolmentScheme.AmendPPS()
        Dim dsAmendPPS As New DataSet
        dsAmendPPS = amendPPSEnrolmentScheme.ResultDataSet
        Dim _ticketingPPS As UserControls_TicketingPPS = CType(e.Item.FindControl("TicketingPPS1"), UserControls_TicketingPPS)

        If dsAmendPPS Is Nothing Then
            _ticketingPPS.Enabled = False
            _ticketingPPS.Visible = False
        Else
            _ticketingPPS.setHiddenFields()
            If dsAmendPPS.Tables("StatusResults").Rows.Count > 0 Then
                If Not dsAmendPPS.Tables("StatusResults").Rows(0)("ReturnCode").ToString.Trim.Equals("NF") Then
                    'Report any errors but do not report "NF" errors as these are for invalid Schemes that we don't want to show.
                    Dim returnCode As String = String.Empty
                    Dim errorMessage As New ListItem
                    returnCode = dsAmendPPS.Tables("StatusResults").Rows(0)(1).ToString.Trim
                    errorMessage.Text = _errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, returnCode).ERROR_MESSAGE
                    plhErrorList.Visible = True
                    If Not errorlist.Items.Contains(errorMessage) Then
                        errorlist.Items.Add(errorMessage)
                    End If
                Else
                    e.Item.Visible = False
                End If
            End If

            Dim _PPSSeatTable As New DataTable
            _PPSSeatTable = dsAmendPPS.Tables("PPSResults")
            If _PPSSeatTable.Rows.Count > 0 Then
                Try
                    Dim enrolled As Boolean = False
                    Dim schemeLocked As Boolean = False

                    For Each row As DataRow In _PPSSeatTable.Rows
                        enrolled = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(row("Enrolled"))
                        schemeLocked = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(row("schemeLocked"))
                        If Not enrolled Then
                            If String.IsNullOrEmpty(row("SeatDetails").ToString.Trim) Then
                                _ticketingPPS.Enabled = False
                                _ticketingPPS.Visible = False
                                e.Item.Visible = False
                                Exit For
                            Else
                                Dim hideSchemeWhenNotEnrolled As Boolean = CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("HideSchemeWhenNotEnrolled"))
                                If Not schemeLocked AndAlso hideSchemeWhenNotEnrolled Then
                                    e.Item.Visible = Not hideSchemeWhenNotEnrolled
                                    Exit For
                                End If
                            End If
                        End If
                    Next
                    _ticketingPPS.AmendPPSDetails.DataSource = _PPSSeatTable
                    _ticketingPPS.AmendPPSDetails.DataBind()
                    If Not _showPaymentOptions And Not enrolled And e.Item.Visible Then
                        _showPaymentOptions = True
                    End If
                    _ticketingPPS.SchemeLockedMessageplh.Visible = False

                    If schemeLocked Then
                        _ticketingPPS.SchemeLockedMessageplh.Visible = True
                    End If
                Catch
                End Try
            Else
                _ticketingPPS.Enabled = False
                _ticketingPPS.Visible = False
            End If
        End If
    End Sub

    Private Sub BindExtendedDescription(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        Dim productCode As String = CType(e.Item.FindControl("hfProductCode"), HiddenField).Value
        Dim priceCode As String = CType(e.Item.FindControl("hdfPriceCode"), HiddenField).Value
        If CType(_ucr.Attribute("RetrieveExtendedText"), Boolean) Or CType(_ucr.Attribute("RetrieveCoReqGrpDesc"), Boolean) Then
            If _ProductType = "T" Or _ProductType = "E" Then
                CType(e.Item.FindControl("plhExtendedTextPanel"), PlaceHolder).Visible = False
            End If
            Dim talentErrObj As New Talent.Common.ErrorObj
            Dim talProduct As New Talent.Common.TalentProduct
            Dim settingsEntity As Talent.Common.DESettings = Talent.eCommerce.Utilities.GetSettingsObject()
            talProduct.Settings() = settingsEntity
            talProduct.De.ProductCode = productCode
            talProduct.De.ProductType = ProductType
            talProduct.De.GetLowPrices = False
            talProduct.De.GetLowPrices = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowCheapestPrices"))
            talProduct.De.Src = GlobalConstants.SOURCE
            talProduct.De.PriceCode = priceCode
            talentErrObj = talProduct.ProductDetails
            Dim ds As New DataSet
            ds = talProduct.ResultDataSet
            If Not talentErrObj.HasError AndAlso ds.Tables.Count > 1 AndAlso ds.Tables(2).Rows.Count > 0 Then
                Try
                    Dim tbl1 As DataTable = ds.Tables(2)
                    Dim rw As DataRow = tbl1.Rows(0)
                    Dim ltlExtendedText1 As Literal = CType(e.Item.FindControl("ltlExtendedText1"), Literal)
                    Dim ltlExtendedText2 As Literal = CType(e.Item.FindControl("ltlExtendedText2"), Literal)
                    Dim ltlExtendedText3 As Literal = CType(e.Item.FindControl("ltlExtendedText3"), Literal)
                    Dim ltlExtendedText4 As Literal = CType(e.Item.FindControl("ltlExtendedText4"), Literal)
                    Dim ltlExtendedText5 As Literal = CType(e.Item.FindControl("ltlExtendedText5"), Literal)
                    ltlExtendedText1.Text = rw("ProductDetail1").ToString
                    ltlExtendedText2.Text = rw("ProductDetail2").ToString
                    ltlExtendedText3.Text = rw("ProductDetail3").ToString
                    ltlExtendedText4.Text = rw("ProductDetail4").ToString
                    ltlExtendedText5.Text = rw("ProductDetail5").ToString
                    ' If away, reset description header to "Description & Price Code desc" (currently set to 
                    ' just price code desc)
                    If _ProductType = "A" Then
                        If rw("ProductDescription").ToString.Trim <> String.Empty AndAlso
                            rw("ProductDescription").ToString.Trim <> CType(e.Item.FindControl("ltlProductDescription"), Literal).Text Then
                            Dim awayHeaderSeperator As String = _ucr.Content("awayHeaderSeperator", _languageCode, True)
                            If awayHeaderSeperator = String.Empty Then awayHeaderSeperator = " - "
                            CType(e.Item.FindControl("ltlProductDescription"), Literal).Text =
                                          rw("ProductDescription").ToString.Trim & awayHeaderSeperator & CType(e.Item.FindControl("ltlProductDescription"), Literal).Text
                        End If
                    End If

                    ' Set lowest prices if configured to do so...
                    If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowCheapestPrices")) AndAlso (_ProductType = "S" OrElse _ProductType = "H") Then
                        Dim litProdDesc As Literal = CType(e.Item.FindControl("ltlProductDescription"), Literal)
                        Dim lowPrice1 As String = rw("LowPrice1").ToString
                        Dim lowPrice2 As String = rw("LowPrice2").ToString
                        Dim fromText As String = _ucr.Content("CheapestPricesFromText", _languageCode, True)
                        Dim lowPriceBand1 As String = rw("LowPriceBand1").ToString.Trim
                        Dim lowPriceBand2 As String = rw("LowPriceBand2").ToString.Trim
                        If lowPrice1 <> "000000000" AndAlso lowPriceBand1 <> String.Empty Then
                            litProdDesc.Text &= " " & getPriceBandDescription(lowPriceBand1, ds.Tables(1)) & fromText & TDataObjects.PaymentSettings.FormatCurrency(CDec(lowPrice1.Insert(7, ".")), _ucr.BusinessUnit, _ucr.PartnerCode)
                        End If
                        If lowPrice2 <> "000000000" AndAlso lowPriceBand2 <> String.Empty Then
                            litProdDesc.Text &= " " & getPriceBandDescription(lowPriceBand2, ds.Tables(1)) & fromText & TDataObjects.PaymentSettings.FormatCurrency(CDec(lowPrice2.Insert(7, ".")), _ucr.BusinessUnit, _ucr.PartnerCode)
                        End If
                    End If

                    CType(e.Item.FindControl("plhExtendedTextPanel"), PlaceHolder).Visible = (ltlExtendedText1.Text.Trim().Length > 0 OrElse ltlExtendedText2.Text.Trim().Length > 0 OrElse
                        ltlExtendedText3.Text.Trim().Length > 0 OrElse ltlExtendedText4.Text.Trim().Length > 0 OrElse ltlExtendedText5.Text.Trim().Length > 0)

                    'Add Product specific content from tbl_product_specific_content
                    Dim dtSpecificContent As DataTable = TDataObjects.ProductsSettings.TblProductSpecificContent.GetProductContent("ProductList", productCode)
                    Dim ltlSpecificContent1 As Literal = CType(e.Item.FindControl("ltlSpecificContent1"), Literal)
                    If dtSpecificContent.Rows.Count > 0 Then
                        ltlSpecificContent1.Text = dtSpecificContent.Rows(0).Item("Product_Content").ToString
                    End If

                Catch
                End Try
            End If
        End If
    End Sub

    Private Sub setControlVisibility(ByVal item As RepeaterItem)
        'disable all
        '---------------------------------
        CType(item.FindControl("MatchDayHospitality1"), UserControls_MatchDayHospitality).Visible = False
        CType(item.FindControl("TicketingPPS1"), UserControls_TicketingPPS).Visible = False
        CType(item.FindControl("TicketingPPS1"), UserControls_TicketingPPS).Enabled = False
        CType(item.FindControl("PriceBandSelection1"), UserControls_PriceBandSelection).Visible = False

        'enable the controls for the relevant product type
        '--------------------------------------------------
        Select Case _ProductType
            Case "A"
                If (CType(item.FindControl("hfProductType"), HiddenField).Value.Trim().Equals("H") And CType(item.FindControl("hfProductHomeAsAway"), HiddenField).Value.Trim().Equals("Y")) Then
                Else
                    CType(item.FindControl("PriceBandSelection1"), UserControls_PriceBandSelection).Visible = True
                End If
            Case "H", "S"
                'Options have been removed
            Case "T", "E"
                If CType(item.FindControl("hdfProductAvailability"), HiddenField).Value.ToUpper <> "N" Then
                    CType(item.FindControl("PriceBandSelection1"), UserControls_PriceBandSelection).Visible = True
                End If
            Case "C"
                CType(item.FindControl("PriceBandSelection1"), UserControls_PriceBandSelection).Visible = True
            Case "P"
                CType(item.FindControl("TicketingPPS1"), UserControls_TicketingPPS).Visible = True
                CType(item.FindControl("TicketingPPS1"), UserControls_TicketingPPS).Enabled = True
            Case "CH"
                If Not CType(item.FindControl("plhUnavailableBox"), PlaceHolder).Visible Then
                    CType(item.FindControl("MatchDayHospitality1"), UserControls_MatchDayHospitality).Visible = True
                    CType(item.FindControl("MatchDayHospitality1"), UserControls_MatchDayHospitality).Enabled = True
                End If
        End Select
    End Sub

    ''' <summary>
    ''' Load the addtional information option to the product if it is available, this changes the css class names of the game selection div tag
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub loadAdditionalInformation(ByRef productCode As String, ByRef e As RepeaterItem)
        Dim IncludeFolderName As String = "Other"
        Dim HTMLFound As Boolean = False
        If Talent.eCommerce.Utilities.DoesHtmlFileExists(_ucr.BusinessUnit & "\" & _ucr.PartnerCode & "\Product\" & IncludeFolderName & "\" & productCode & ".htm") Then
            HTMLFound = True
        ElseIf Talent.eCommerce.Utilities.DoesHtmlFileExists(_ucr.BusinessUnit & "\" & _ucr.PartnerCode & "\Product\" & IncludeFolderName & "\" & productCode & ".html") Then
            HTMLFound = True
        ElseIf Talent.eCommerce.Utilities.DoesHtmlFileExists(_ucr.BusinessUnit & "\" & _ucr.PartnerCode & "\Product\" & IncludeFolderName & "\" & ProductSubType & ".htm") Then
            HTMLFound = True
        ElseIf Talent.eCommerce.Utilities.DoesHtmlFileExists(_ucr.BusinessUnit & "\" & _ucr.PartnerCode & "\Product\" & IncludeFolderName & "\" & ProductSubType & ".html") Then
            HTMLFound = True
        ElseIf Talent.eCommerce.Utilities.DoesHtmlFileExists(_ucr.BusinessUnit & "\" & _ucr.PartnerCode & "\Product\" & IncludeFolderName & "\" & ProductType & ".htm") Then
            HTMLFound = True
        ElseIf Talent.eCommerce.Utilities.DoesHtmlFileExists(_ucr.BusinessUnit & "\" & _ucr.PartnerCode & "\Product\" & IncludeFolderName & "\" & ProductType & ".html") Then
            HTMLFound = True
        End If

        Dim plhAdditionalInformation As PlaceHolder = CType(e.FindControl("plhAdditionalInformation"), PlaceHolder)
        Dim hlkAdditionalInformation As HyperLink = CType(e.FindControl("hlkAdditionalInformation"), HyperLink)
        Dim ltlMoreInfoText As Literal = CType(e.FindControl("ltlMoreInfoText"), Literal)
        If HTMLFound Then
            plhAdditionalInformation.Visible = True
            hlkAdditionalInformation.NavigateUrl = "~/PagesPublic/ProductBrowse/ProductSummary.aspx?ProductCode=" & productCode & "&ProductType=" & ProductType & "&ProductSubType=" & ProductSubType & "&IncludeFolderName=" & IncludeFolderName
            ltlMoreInfoText.Text = _ucr.Content("MoreInformationText", _languageCode, True)
        Else
            Dim pdfFound As Boolean = False
            Dim pdfLink As String = Talent.eCommerce.Utilities.PDFLinkAvailable(productCode, ProductType, ProductSubType, _ucr.BusinessUnit, _ucr.PartnerCode)
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

    Private Sub SetProductAvailabilityText(ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs, ByVal textCode As String)
        '----------------------------------------------------------------
        'Display "unavailable" for this product if it is set as view only or not yet on sale with sales channels. Ignore this if the 
        'unavailble setting has not been found in the datatable
        '----------------------------------------------------------------
        Dim hdfProductAvailability As HiddenField = CType(e.Item.FindControl("hdfProductAvailability"), HiddenField)
        If hdfProductAvailability IsNot Nothing AndAlso hdfProductAvailability.Value.ToUpper() = "N" Then
            Dim plhUnavailableBox As PlaceHolder = CType(e.Item.FindControl("plhUnavailableBox"), PlaceHolder)
            Dim ltlUnavailableBoxText As Literal = CType(e.Item.FindControl("ltlUnavailableBoxText"), Literal)
            plhUnavailableBox.Visible = True
            ltlUnavailableBoxText.Text = _ucr.Content(textCode, _languageCode, True)
        End If

    End Sub

#End Region

#Region "Protected Methods"
    Protected Sub setLabelValues()
        'All other labels 
        sortByLabel.Text = _ucr.Content("sortByLabel", _languageCode, True)
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

        'Set visiblity
        plhDisplayingLabelT.Visible = (displayingLabelT.Text.Length > 0)
        plhDisplayingLabelB.Visible = (displayingLabelB.Text.Length > 0)
        plhToLabelT.Visible = (toLabelT.Text.Length > 0)
        plhToLabelB.Visible = (toLabelB.Text.Length > 0)
        plhOfLabelT.Visible = (ofLabelT.Text.Length > 0)
        plhOfLabelB.Visible = (ofLabelB.Text.Length > 0)
    End Sub

    Protected Sub showData(ByVal pCurrentPage As String)
        Try
            Dim product As New Talent.Common.TalentProduct
            Dim settings As New Talent.Common.DESettings
            Dim err As New Talent.Common.ErrorObj
            Dim VnumberOfLinks As String
            Dim LinksToDisplay As Integer = 0
            Dim stadiumList As String = String.Empty

            'Get stadiumList querystring if it exists and populate a stadium string
            If Not String.IsNullOrEmpty(Request.QueryString("stadiumList")) Then
                stadiumList = Request.QueryString("stadiumList")
            End If

            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.BusinessUnit = TalentCache.GetBusinessUnit()
            settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            settings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
            settings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
            settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
            settings.AuthorityUserProfile = ModuleDefaults.AuthorityUserProfile
            settings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))
            settings.IsAgent = Talent.eCommerce.Utilities.IsAgent

            product.Settings() = settings
            If _ProductType <> "CH" Then
                product.De.ProductType = _ProductType
                'Check to see if we are getting the stadium code from querystring or DB
                If Not String.IsNullOrEmpty(stadiumList) Then
                    'only allow upto 5 stadium codes
                    Dim stadiumListArray() As String = stadiumList.Split(",")
                    If stadiumListArray.Length > 4 Then
                        Dim result As String = String.Empty
                        For x As Byte = 0 To 4
                            result &= stadiumListArray(x) + ","
                        Next
                        result = result.Substring(0, result.Length - 1)
                        product.De.StadiumCode = result
                    Else
                        product.De.StadiumCode = stadiumList
                    End If
                Else
                    product.De.StadiumCode = ModuleDefaults.TicketingStadium
                End If
                If _ProductType = GlobalConstants.PPSPRODUCTTYPE Then
                    If AmendPPSEnrolmentInUse Then
                        product.De.CustomerNumber = Profile.User.Details.LoginID
                    End If
                    If String.IsNullOrEmpty(Request.QueryString("ppspage")) Then
                        PPSType = "1"
                    Else
                        PPSType = Request.QueryString("ppspage")
                    End If
                End If
            Else
                product.De.ProductType = GlobalConstants.HOMEPRODUCTTYPE
                product.De.StadiumCode = ModuleDefaults.CorporateStadium
            End If
            product.De.PriceAndAreaSelection = ModuleDefaults.PriceAndAreaSelection
            If Not String.IsNullOrWhiteSpace(_ProductSubType) Then product.De.ProductSubtype = _ProductSubType

            product.De.PPSType = Me.PPSType
            product.De.Src = GlobalConstants.SOURCE
            err = product.ProductList()
            _ds1 = product.ResultDataSet

            If Not err.HasError Then
                Dim rowFilterCondition As String = GetFilterCondition()

                If ModuleDefaults.ShowTravelAsDDL AndAlso _ProductType = "T" Then
                    SetupTotalsForTravelDropdown()
                    rowFilterCondition &= " AND Unique = 'True'"
                End If
                If ModuleDefaults.ShowUniqueMemberships AndAlso _ProductType = "C" Then
                    SetupUniqueMemberships()
                    rowFilterCondition &= " AND Unique = 'True'"
                End If
                _ds1.Tables(_cProductTable).DefaultView.RowFilter = rowFilterCondition


                If Session("filterValue") <> "" Then
                    If filterByDropDown.Items.Count <> 0 Then
                        filterByDropDown.SelectedValue = Session("filterValue")
                    Else
                        loadSortBy()
                        filterByDropDown.SelectedValue = Session("filterValue")
                    End If
                Else
                    loadSortBy()
                End If

                If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowProductBundlesFirst")) Then
                    _ds1.Tables(_cProductTable).DefaultView.Sort = "IsProductBundle DESC, ProductMDTE08"
                Else
                    _ds1.Tables(_cProductTable).DefaultView.Sort = "ProductMDTE08"
                End If

                If filterByDropDown.Text <> "ALL" Then
                    _ds1.Tables(_cProductTable).DefaultView.RowFilter = "ProductCompetitionCode='" + filterByDropDown.SelectedValue + "' and (" + rowFilterCondition + ")"
                End If

                ' Do not display paging when in kiosk mode
                If ModuleDefaults.TicketingKioskMode Then
                    _objPds.DataSource = _ds1.Tables(_cProductTable).DefaultView
                    _objPds.AllowPaging = False
                    _objPds.PageSize = _ucr.Attribute("numberOfProductsVariable")
                    PagerTop.Visible = False
                    PagerBottom.Visible = False
                Else
                    If ProductType = "P" Then
                        PPSFreeAreas = _ds1.Tables(0).Rows(0).Item("PPSFreeAreas")
                        _objPds.AllowPaging = False
                        _objPds.PageSize = 10000
                        PagerTop.Visible = False
                        PagerBottom.Visible = False

                        Dim stCode As String = Request.QueryString("product")
                        If Not String.IsNullOrEmpty(stCode) Then
                            Dim rows() As DataRow = _ds1.Tables(_cProductTable).Select("RelatingSeasonTicket='" & stCode & "'")
                            Dim myTable As New DataTable
                            For Each col As DataColumn In _ds1.Tables(_cProductTable).Columns
                                myTable.Columns.Add(New DataColumn(col.ColumnName, col.DataType))
                            Next
                            For Each rw As DataRow In rows
                                Dim myRow As DataRow = myTable.NewRow
                                For Each col As DataColumn In myTable.Columns
                                    myRow(col.ColumnName) = rw(col.ColumnName)
                                Next
                                myTable.Rows.Add(myRow)
                            Next
                            _objPds.DataSource = myTable.DefaultView
                        Else
                            _objPds.DataSource = _ds1.Tables(_cProductTable).DefaultView
                        End If
                    Else
                        _objPds.DataSource = _ds1.Tables(_cProductTable).DefaultView
                        _objPds.AllowPaging = True
                        _objPds.PageSize = _ucr.Attribute("numberOfProductsVariable")
                    End If
                End If

                'refresh repeater when Rowfilter is done
                If filterByDropDown.Text <> "ALL" Then
                    ProductRepeater.DataSource = _objPds
                    ProductRepeater.DataBind()
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
                _vTotalRec = _ds1.Tables(_cProductTable).DefaultView.Count
                If _vTotalRec = 0 Then
                    displayingValueLabelT.Text = 0
                End If
                plhDisplayingValueLabelT.Visible = (displayingValueLabelT.Text.Length > 0)

                If Not (_ProductType = GlobalConstants.HOMEPRODUCTTYPE OrElse _ProductType = GlobalConstants.SEASONTICKETPRODUCTTYPE OrElse _ProductType = GlobalConstants.PPSPRODUCTTYPE) Then
                    GetJS_AccordionUrl(_vTotalRec)
                Else
                    _showSelectionBox = False
                End If

                ofValueLabelT.Text = _vTotalRec.ToString()
                plhOfValueLabelT.Visible = (ofValueLabelT.Text.Length > 0)
                If _CurRec > _vTotalRec Then
                    toValueLabelT.Text = _vTotalRec
                Else
                    toValueLabelT.Text = _CurRec
                End If
                plhToLabelT.Visible = (toValueLabelT.Text.Length > 0)

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


                If filterByDropDown.Text <> "ALL" Then
                    ProductRepeater.DataSource = _objPds
                    ProductRepeater.DataBind()
                Else
                    If filterByDropDown.Text = "ALL" Then
                        _ds1.Tables(_cProductTable).DefaultView.RowFilter = ""
                        _ds1.Tables(_cProductTable).DefaultView.RowFilter = rowFilterCondition
                    End If
                End If

                If _totLinks > 1 Then
                    Dim Url As New StringBuilder(Request.CurrentExecutionFilePath)

                    If Request.Url.Query = "" Then
                        Url.Append("?Page=")
                    Else
                        If Request.Url.Query.Contains("&Page=") Then
                            Url.Append(Request.Url.Query.Substring(0, Request.Url.Query.LastIndexOf("&")))
                            Url.Append("&Page=")
                        ElseIf Request.Url.Query.Contains("?Page=") Then
                            Url.Append("?Page=")
                        Else
                            Url.Append(Request.Url.Query)
                            Url.Append("&Page=")
                        End If
                    End If
                    If Not _objPds.IsFirstPage Then LnkPrevT.NavigateUrl = Url.ToString() + Convert.ToString(_CurPage - 1)
                    If Not _objPds.IsLastPage Then LnkNextT.NavigateUrl = Url.ToString() + Convert.ToString(_CurPage + 1)
                    If (_totLinks >= 5) Then
                        _lowPage = (_CurPage - 2)
                        If (_lowPage < 1) Then
                            _lowPage = 1
                        End If

                        _highPage = (_lowPage + 4)
                        If (_highPage > _totLinks) Then
                            _highPage = _totLinks
                            _lowPage = _highPage - 4
                        End If
                    Else
                        _lowPage = 1
                        _highPage = _totLinks
                    End If

                    For i As Integer = _lowPage To _highPage
                        If i <> _CurPage Then
                            linktxt = linktxt + "<li><a href='" + Url.ToString() + i.ToString() + "'>" + i.ToString() + "</a></li>"
                        Else
                            linktxt = linktxt + "<li class='current'><a href=''>" + i.ToString() + "</a></li>"
                        End If
                    Next
                    plhBottomPager.Visible = True
                    plhTopPager.Visible = True
                Else
                    plhLnkPrevT.Visible = False
                    plhLnkNextT.Visible = False
                    plhBottomPager.Visible = False
                    plhTopPager.Visible = False
                    If _totLinks <> 0 Then
                        linktxt = " 1 "
                    Else
                        linktxt = " 0 "
                    End If

                End If
                LinksLabelT.Text = linktxt
            Else
                If _ds1 IsNot Nothing AndAlso _ds1.Tables(0).Rows(0).Item("ErrorOccurred").ToString = GlobalConstants.ERRORFLAG Then
                    showError(_ds1.Tables(0).Rows(0).Item("ReturnCode").ToString())
                End If
            End If

            If Not plhTopPager.Visible And Not _filterDropDownHasItems Then
                plhPagerTopWrapper.Visible = False
            End If

        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' We are showing one row for travel poroducts, and the different coach options on a ddl in PriceBandSelection..
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub SetupTotalsForTravelDropdown()
        If Not _ds1.Tables(_cProductTable).Columns.Contains("Unique") Then
            _ds1.Tables(_cProductTable).Columns.Add("Unique", GetType(Boolean))
            _ds1.Tables(_cProductTable).Columns.Add("TotalCapacity", GetType(Integer))
            _ds1.Tables(_cProductTable).Columns.Add("TotalReturns", GetType(Integer))
            _ds1.Tables(_cProductTable).Columns.Add("TotalSales", GetType(Integer))
            _ds1.Tables(_cProductTable).Columns.Add("TotalUnavailable", GetType(Integer))
            _ds1.Tables(_cProductTable).Columns.Add("TotalBookings", GetType(Integer))
            _ds1.Tables(_cProductTable).Columns.Add("TotalReservations", GetType(Integer))
        End If

        Dim lastCode As String = String.Empty
        Dim lastIndex As Integer = -1
        Dim lastCapacity As Integer = 0
        Dim lastReturns As Integer = 0
        Dim lastSales As Integer = 0
        Dim lastUnavailable As Integer = 0
        Dim lastBookings As Integer = 0
        Dim lastReservations As Integer = 0
        Dim indexCount As Integer = 0
        Dim needToSetLastUnique As Boolean = False
        ' 
        ' loop through and total up capacities for unique travel codes
        For Each dr As DataRow In _ds1.Tables(_cProductTable).Rows
            If dr("ProductType").ToString = "T" Then
                If dr.Item("ProductCode").ToString <> lastCode Then
                    ' new code - update totals for last one..
                    If lastIndex >= 0 Then
                        _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalCapacity") = lastCapacity
                        _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalReturns") = lastReturns
                        _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalSales") = lastSales
                        _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalUnavailable") = lastUnavailable
                        _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalBookings") = lastBookings
                        _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalReservations") = lastReservations
                        needToSetLastUnique = False
                    End If
                    lastCode = dr.Item("ProductCode").ToString
                    dr.Item("Unique") = True
                    lastIndex = indexCount
                    lastCapacity = CInt(dr("capacityCnt"))
                    lastReturns = CInt(dr("returnsCnt"))
                    lastSales = CInt(dr("salesCnt"))
                    lastUnavailable = CInt(dr("unavailableCnt"))
                    lastBookings = CInt(dr("bookingsCnt"))
                    lastReservations = CInt(dr("reservationsCnt"))
                    needToSetLastUnique = True
                Else
                    dr.Item("Unique") = False
                    lastCapacity += CInt(dr("capacityCnt"))
                    lastReturns += CInt(dr("returnsCnt"))
                    lastSales += CInt(dr("salesCnt"))
                    lastUnavailable += CInt(dr("unavailableCnt"))
                    lastBookings += CInt(dr("bookingsCnt"))
                    lastReservations += CInt(dr("reservationsCnt"))
                    _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalCapacity") = 0
                    _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalReturns") = 0
                    _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalSales") = 0
                    _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalUnavailable") = 0
                    _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalBookings") = 0
                    _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalReservations") = 0
                End If
            End If
            indexCount += 1
        Next

        ' set totals on last unique one..
        If lastIndex >= 0 AndAlso needToSetLastUnique Then
            _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalCapacity") = lastCapacity
            _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalReturns") = lastReturns
            _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalSales") = lastSales
            _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalUnavailable") = lastUnavailable
            _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalBookings") = lastBookings
            _ds1.Tables(_cProductTable).Rows(lastIndex)("TotalReservations") = lastReservations
            needToSetLastUnique = False
        End If

        'Sort out the sold out flag based on the general summary availablity after everything else has been totalled up
        For Each dr As DataRow In _ds1.Tables(_cProductTable).Rows
            If dr("Unique") = True Then
                Dim totalCapacity As Integer = dr("TotalCapacity")
                Dim totalReturns As Integer = dr("TotalReturns")
                Dim totalSales As Integer = dr("TotalSales")
                Dim totalUnavailable As Integer = dr("TotalUnavailable")
                Dim totalBookings As Integer = dr("TotalBookings")
                Dim totalReservations As Integer = dr("TotalReservations")
                If _productDetailHelper.AvailabilityTotal(totalCapacity, totalReturns, totalSales, totalUnavailable, totalBookings, totalReservations) > 0 Then
                    dr("IsSoldOut") = False
                Else
                    dr("IsSoldOut") = True
                End If
            End If
        Next
    End Sub

    Protected Sub SetupUniqueMemberships()
        ' Only show unique memberships - price codes to be changed on the basket
        If Not _ds1.Tables(_cProductTable).Columns.Contains("Unique") Then
            _ds1.Tables(_cProductTable).Columns.Add("Unique", GetType(Boolean))
        End If

        Dim lastCode As String = String.Empty
        Dim lastIndex As Integer = -1
        Dim lastCapacity As Integer = 0
        Dim lastReturns As Integer = 0
        Dim lastSales As Integer = 0
        Dim lastUnavailable As Integer = 0
        Dim lastBookings As Integer = 0
        Dim lastReservations As Integer = 0
        Dim indexCount As Integer = 0
        Dim needToSetLastUnique As Boolean = False

        For Each dr As DataRow In _ds1.Tables(_cProductTable).Rows
            If dr("ProductType").ToString = "C" Then
                If dr.Item("ProductCode").ToString <> lastCode Then
                    ' new code 
                    lastCode = dr.Item("ProductCode").ToString
                    dr.Item("Unique") = True
                    lastIndex = indexCount
                Else
                    dr.Item("Unique") = False
                End If

            End If

            indexCount += 1
        Next

    End Sub

    Protected Sub loadSortBy()
        Dim dsHelper As New DataSetHelperProdouct(_ds1)
        filterByDropDown.DataSource = dsHelper.SelectDistinct(_ds1.Tables(_cProductTable).TableName, _ds1.Tables(_cProductTable),
                                                            "ProductCompetitionText", System.Type.GetType("System.String"),
                                                            "ProductCompetitionCode", System.Type.GetType("System.String"))
        filterByDropDown.DataTextField = "ProductCompetitionText"
        filterByDropDown.DataValueField = "ProductCompetitionCode"
        filterByDropDown.DataBind()
        filterByDropDown.Items.Insert(0, "ALL")
        _filterDropDownHasItems = False
        If filterByDropDown.Items.Count > 1 Then
            For Each item As ListItem In filterByDropDown.Items
                If item.Text <> "ALL" AndAlso item.Text.Trim().Length > 0 Then
                    _filterDropDownHasItems = True
                    Exit For
                End If
            Next
        End If
    End Sub

    Protected Sub setBottomPagerValues()
        displayingValueLabelB.Text = displayingValueLabelT.Text
        plhDisplayingValueLabelB.Visible = (displayingValueLabelB.Text.Length > 0)
        toValueLabelB.Text = toValueLabelT.Text
        plhToValueLabelB.Visible = (toValueLabelB.Text.Length > 0)
        ofValueLabelB.Text = ofValueLabelT.Text
        plhOfValueLabelB.Visible = (ofValueLabelB.Text.Length > 0)
        LnkPrevB.NavigateUrl = LnkPrevT.NavigateUrl
        LnkNextB.NavigateUrl = LnkNextT.NavigateUrl
        LinksLabelB.Text = LinksLabelT.Text

        Dim Url As New StringBuilder(Request.CurrentExecutionFilePath)

        If Request.Url.Query = "" Then
            Url.Append("?Page=")
        Else
            If Request.Url.Query.Contains("&Page=") Then

                Url.Append(Request.Url.Query.Substring(0, Request.Url.Query.LastIndexOf("&")))
                Url.Append("&Page=")

            ElseIf Request.Url.Query.Contains("?Page=") Then

                Url.Append("?Page=")

            Else

                Url.Append(Request.Url.Query)
                Url.Append("&Page=")

            End If

        End If

        If _CurPage = 1 Or _CurPage = 0 Then
            plhLnkFirstT.Visible = False
            plhLnkFirstB.Visible = False
            plhLnkPrevT.Visible = False
            plhLnkPrevB.Visible = False
        Else
            plhLnkFirstT.Visible = True
            LnkFirstT.NavigateUrl = Url.ToString() + "1"
            plhLnkFirstB.Visible = True
            LnkFirstB.NavigateUrl = Url.ToString() + "1"
        End If
        If _CurPage = _totLinks Or _totLinks = 0 Then
            plhLnkLastT.Visible = False
            plhLnkLastB.Visible = False
            plhLnkNextT.Visible = False
            plhLnkNextB.Visible = False
        Else
            plhLnkLastT.Visible = True
            LnkLastT.NavigateUrl = Url.ToString() + _totLinks.ToString()
            plhLnkLastB.Visible = True
            LnkLastB.NavigateUrl = Url.ToString() + _totLinks.ToString()
        End If

    End Sub

    Protected Sub showError(ByVal errCode As String)
        Dim errorList As New BulletedList
        ' Find the errorLabel if not already done.
        If errorList.ID Is Nothing Then
            Dim conControl As Control
            For Each conControl In Parent.Controls
                Select Case conControl.ID
                    Case Is = "ErrorList"
                        errorList = CType(conControl, BulletedList)
                        Dim errorMessage = _errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString,
                                        Talent.eCommerce.Utilities.GetCurrentPageName,
                                        errCode).ERROR_MESSAGE()
                        Dim errorAlreadyListed As Boolean = False
                        For Each listItem As ListItem In errorList.Items
                            If listItem.Text = errorMessage Then
                                errorAlreadyListed = True
                                Exit For
                            End If
                        Next
                        If Not errorAlreadyListed Then
                            errorList.Items.Add(errorMessage)
                        End If
                        Exit For
                End Select
            Next
        End If
    End Sub

    Protected Sub filterByDropDown_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles filterByDropDown.SelectedIndexChanged
        Session("filterValue") = filterByDropDown.SelectedValue
        Dim url As String = Request.Url.AbsoluteUri
        Try
            Dim _nameValueCollection As New NameValueCollection
            _nameValueCollection = HttpUtility.ParseQueryString(url)
            _nameValueCollection.Remove("page")
            url = Server.UrlDecode(_nameValueCollection.ToString)
        Catch ex As Exception
        End Try
        Response.Redirect(url)
    End Sub

    Protected Function getErrText(ByVal pCode As String) As String
        Dim wfrPage As New WebFormResource
        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
        End With
        Dim s As String
        s = wfrPage.Content(pCode, _languageCode, True)
        Return s
    End Function

    Protected Function GetProductEligibilityText(ByVal ProductReqdMemDesc As String) As String
        Dim str As String = String.Empty
        If Not String.IsNullOrEmpty(ProductReqdMemDesc) Then
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("RetrieveCoReqGrpDesc")) Then
                str = _ucr.Content("CoReqGrpDescCaption", _languageCode, True).Replace("<<PRE-REQ>>", ProductReqdMemDesc)
            Else
                str = _ucr.Content("EligibilityPreReqCaption", _languageCode, True).Replace("<<PRE-REQ>>", ProductReqdMemDesc)
            End If
        End If
        Return str
    End Function

    Protected Function GetFormattedProductDateTime(ByVal ProductDate As String, ByVal ProductYear As String, ByVal ProductKickOffTime As String, ByVal ProductEntryTime As String) As String
        If ModuleDefaults.ShowProductEntryTime Then
            Return GetFormattedProductDateTimeKO(ProductDate, ProductYear, ProductEntryTime)
        Else
            If _ProductType = "T" Or _ProductType = "E" Then
                Return GetFormattedProductDate(ProductDate, ProductYear)
            Else
                Return GetFormattedProductDateTimeKO(ProductDate, ProductYear, ProductKickOffTime)
            End If
        End If
    End Function

    Protected Function GetFormattedProductDateTimeKO(ByVal ProductDate As String, ByVal ProductYear As String, ByVal ProductTime As String) As String
        Dim str As String = String.Empty

        If Not (ProductDate.Trim = "0" Or ProductDate.Trim = "0000000") Then
            Dim dateValue As Date = Talent.Common.Utilities.ISeriesDate(ProductDate.Trim)
            If ModuleDefaults.GlobalDateFormat = "yyyy/MM/dd" Then

                Dim dateString As String = dateValue.ToString("dd MMMM")
                Dim culture As New CultureInfo(ModuleDefaults.Culture)
                Dim day As String = culture.DateTimeFormat.DayNames(dateValue.DayOfWeek)
                Dim dateSeparator As String = _ucr.Content("DateSeparator", _languageCode, True)
                str = day & dateSeparator & dateString & dateSeparator & ProductYear & dateSeparator & _ucr.Content("KickoffLabel", _languageCode, True) & ProductTime
            Else
                Dim dateSeparator As String = _ucr.Content("DateSeparator", _languageCode, True)
                'str = Day() & dateSeparator & DateString & dateSeparator & ProductYear & dateSeparator & ucr.Content("KickoffLabel", _languageCode, True) & ProductTime
                str = dateValue.ToString(ModuleDefaults.GlobalDateFormat) & dateSeparator & _ucr.Content("KickoffLabel", _languageCode, True) & ProductTime

            End If
        End If
        Return str
    End Function

    Protected Function GetFormattedProductDate(ByVal ProductDate As String, ByVal ProductYear As String) As String
        Dim str As String = String.Empty
        Dim dateValue As Date = Talent.Common.Utilities.ISeriesDate(ProductDate.Trim)
        ' if date format differ
        If ModuleDefaults.GlobalDateFormat = "yyyy/MM/dd" Then
            Dim dateString As String = dateValue.ToString("dd MMMM")
            Dim culture As New CultureInfo(ModuleDefaults.Culture)
            Dim day As String = culture.DateTimeFormat.DayNames(dateValue.DayOfWeek)
            Dim dateSeparator As String = _ucr.Content("DateSeparator", _languageCode, True)
            str = day & dateSeparator & dateString & dateSeparator & ProductYear
        Else
            Dim culture As New CultureInfo(ModuleDefaults.Culture)
            str = dateValue.ToString(ModuleDefaults.GlobalDateFormat, culture)
        End If
        Return str
    End Function

    Protected Function GetAreaSelectionUrl(ByVal stadium As String,
                                           ByVal product As String,
                                           ByVal campaign As String,
                                           ByVal type As String) As String
        Return Talent.eCommerce.Utilities.GetCurrentApplicationUrl() & "/PagesPublic/ProductBrowse/standAndAreaSelection.aspx?stadium=" & stadium.Trim & "&product=" & product.Trim & "&campaign=" & campaign.Trim & "&type=" & type.Trim
    End Function

    ''' <summary>
    ''' Set the availability text and colour of the availability label
    ''' </summary>
    ''' <param name="availabilityLabel">The availability label to work with</param>
    ''' <param name="percent">The availability percentage</param>
    ''' <param name="stadiumCode">The stadium code</param>
    ''' <remarks></remarks>
    Private Sub setAvailabilityLabel(ByRef availabilityLabel As Label, ByVal percent As Integer, ByVal stadiumCode As String)
        Dim labelText As String
        Dim labelColour As String
        Dim labelCSSClass As String
        _productDetailHelper.getAvailabilityProperties(_ucr.BusinessUnit, labelText, labelCSSClass, labelColour, percent, stadiumCode)
        availabilityLabel.Text = labelText
        Try
            Dim colConvert As New System.Drawing.ColorConverter
            availabilityLabel.BackColor = CType(colConvert.ConvertFromString(labelColour), System.Drawing.Color)
            availabilityLabel.CssClass = labelCSSClass
        Catch ex As Exception
        End Try
    End Sub

#End Region

#Region "Private Methods and Functions"

    Private Sub BindProductRepeater()
        Try
            If _pageNumber <> "" Then
                Session("ProductDetailCurrentPage") = _pageNumber
            Else
                Session("ProductDetailCurrentPage") = Nothing
            End If
            'If IsPostBack Then _productDetailHelper.ClearAdvPdtFilterSession()
            showData(_pageNumber)
            ProductRepeater.DataSource = _objPds
            ProductRepeater.DataBind()
            setBottomPagerValues()
            If CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("UseAdvancedFilter")) Then
                bindAdvancedProductFilterOptions()
            Else
                plhAdvancedProductFilter.Visible = False
            End If
            If ProductRepeater.Items.Count = 0 Then
                Try
                    Parent.FindControl("multiStadiumText").Visible = False
                Catch
                End Try
            End If
        Catch ex As Exception
            Dim s As String = String.Empty
        End Try
    End Sub

    Private Function getPriceBandDescription(ByVal acceptedPriceBand As String, ByVal priceBandTable As DataTable) As String
        For Each priceband As Data.DataRow In priceBandTable.Rows
            If priceband("PriceBand").ToString = acceptedPriceBand Then
                Return priceband("PriceBandDescription")
            End If
        Next
        Return String.Empty
    End Function

    Private Function FileExists(ByVal x As String) As Boolean
        Return True
    End Function

    Private Sub IncludeProductHtml(ByVal htmlPath As String)
        Dim productHtmlContent As String = String.Empty
        productHtmlContent = Talent.eCommerce.Utilities.GetHtmlFromFile(htmlPath)

    End Sub

    Private Sub GetJS_AccordionUrl(ByVal productCount As Integer)
        Dim jsFile As String
        If productCount = 1 Then
            jsFile = "ticketing-products-accordion-expanded.js"
        Else
            jsFile = "ticketing-products-accordion.js"
        End If
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "accordionMenu", Talent.eCommerce.Utilities.FormatJavaScriptFileReference(jsFile, Nothing))
    End Sub

    Private Function GetFilterCondition() As String
        Dim filterCondition As String = String.Empty
        'decide FacebbokLike
        _isFacebookLikeOn = CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("FacebookLikeOn"))
        _facebookLikeHtmlTag = CheckForDBNull_String(_ucr.Attribute("FacebookLikeHtmlTag"))
        'Deciding filter condition
        Dim rowFilterCondition As String = String.Empty
        If Not String.IsNullOrWhiteSpace(Request.QueryString("IsSingleProduct")) _
            AndAlso (Request.QueryString("IsSingleProduct") = "TRUE") _
            AndAlso Not String.IsNullOrWhiteSpace(Request.QueryString("ProductType")) _
            AndAlso Not String.IsNullOrWhiteSpace(Request.QueryString("ProductCode")) Then
            _productDetailHelper.ProductType = Request.QueryString("ProductType")
            If Not String.IsNullOrWhiteSpace(Request.QueryString("ProductSubType")) Then
                _productDetailHelper.ProductSubType = Request.QueryString("ProductSubType")
            End If
            _productDetailHelper.ProductCode = Request.QueryString("ProductCode")
            If Not String.IsNullOrWhiteSpace(Request.QueryString("ProductDetailCode")) Then
                _productDetailHelper.ProductDetailCode = Request.QueryString("ProductDetailCode")
            End If
            _productDetailHelper.IsSingleProductFilter = Request.QueryString("IsSingleProduct")
            _isSingleProductFilter = True
            AddFaceBookLikeMetaTags()
        Else
            _productDetailHelper.ProductType = _ProductType
            _productDetailHelper.ProductSubType = _ProductSubType
        End If

        SetProductFilterFieldsFromSessions(True, False)

        rowFilterCondition = _productDetailHelper.GetRowFilterCondition()
        If _ProductType = GlobalConstants.PPSPRODUCTTYPE AndAlso Not AmendPPSEnrolmentInUse Then
            rowFilterCondition = rowFilterCondition & " AND PPSSchemeType = 'S'"
        End If
        'deciding filter condition ends here
        Return rowFilterCondition
    End Function

    Private Sub GenerateFaceBookLikeMetaTags(ByVal productRow As DataRow)
        If _fbLikeMetaTagsForProductNotExists Then
            Dim fbProductTitle As String = String.Empty
            Dim fbProductDescription As String = String.Empty
            Dim fbProductURL As String = String.Empty
            Dim fbProductImage As String = String.Empty
            Select Case _ProductType
                Case "A"
                    fbProductTitle = CheckForDBNull_String(productRow("ProductDescription")).Trim()
                    fbProductDescription = CheckForDBNull_String(productRow("ProductDescription2")).Trim()
                    If CheckForDBNull_String(productRow("ProductMDTE08")).Trim.Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductMDTE08")).Trim("0").Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductYear")).Trim.Length > 0 Then
                        fbProductDescription = fbProductDescription & " " & GetFormattedProductDate(productRow("ProductMDTE08").ToString.Trim, productRow("ProductYear").ToString.Trim)
                    End If

                    fbProductURL = Request.Url.GetLeftPart(UriPartial.Path) & "?IsSingleProduct=TRUE"
                    fbProductURL = fbProductURL & "&ProductType=" & CheckForDBNull_String(productRow("ProductType")).Trim()
                    fbProductURL = fbProductURL & "&ProductCode=" & CheckForDBNull_String(productRow("ProductCode")).Trim()
                    If CheckForDBNull_String(productRow("ProductSubType")).Trim().Length > 0 Then
                        fbProductURL = fbProductURL & "&ProductSubType=" & CheckForDBNull_String(productRow("ProductSubType")).Trim()
                    End If
                    fbProductImage = ImagePath.getImagePath("APPTHEME", _ucr.Attribute("FacebookLikeThumbnail"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                Case "H"
                    fbProductTitle = CheckForDBNull_String(productRow("ProductDescription")).Trim()
                    fbProductDescription = CheckForDBNull_String(productRow("ProductDescription2")).Trim()
                    If CheckForDBNull_String(productRow("ProductMDTE08")).Trim.Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductMDTE08")).Trim("0").Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductYear")).Trim.Length > 0 Then
                        fbProductDescription = fbProductDescription & " " & GetFormattedProductDate(productRow("ProductMDTE08").ToString.Trim, productRow("ProductYear").ToString.Trim)
                    End If

                    fbProductURL = Request.Url.GetLeftPart(UriPartial.Path) & "?IsSingleProduct=TRUE"
                    fbProductURL = fbProductURL & "&ProductType=" & CheckForDBNull_String(productRow("ProductType")).Trim()
                    fbProductURL = fbProductURL & "&ProductCode=" & CheckForDBNull_String(productRow("ProductCode")).Trim()
                    If CheckForDBNull_String(productRow("ProductSubType")).Trim().Length > 0 Then
                        fbProductURL = fbProductURL & "&ProductSubType=" & CheckForDBNull_String(productRow("ProductSubType")).Trim()
                    End If
                    fbProductImage = ImagePath.getImagePath("APPTHEME", _ucr.Attribute("FacebookLikeThumbnail"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                Case "T"
                    fbProductTitle = CheckForDBNull_String(productRow("ProductDescription")).Trim()
                    fbProductDescription = CheckForDBNull_String(productRow("ProductDescription2")).Trim()
                    If CheckForDBNull_String(productRow("ProductMDTE08")).Trim.Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductMDTE08")).Trim("0").Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductYear")).Trim.Length > 0 Then
                        fbProductDescription = fbProductDescription & " " & GetFormattedProductDate(productRow("ProductMDTE08").ToString.Trim, productRow("ProductYear").ToString.Trim)
                    End If
                    fbProductURL = Request.Url.GetLeftPart(UriPartial.Path) & "?IsSingleProduct=TRUE"
                    fbProductURL = fbProductURL & "&ProductType=" & CheckForDBNull_String(productRow("ProductType")).Trim()
                    fbProductURL = fbProductURL & "&ProductCode=" & CheckForDBNull_String(productRow("ProductCode")).Trim()
                    If CheckForDBNull_String(productRow("ProductSubType")).Trim().Length > 0 Then
                        fbProductURL = fbProductURL & "&ProductSubType=" & CheckForDBNull_String(productRow("ProductSubType")).Trim()
                    End If
                    fbProductURL = fbProductURL & "&ProductDetailCode=" & CheckForDBNull_String(productRow("ProductDetailCode")).Trim()
                    fbProductImage = ImagePath.getImagePath("APPTHEME", _ucr.Attribute("FacebookLikeThumbnail"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))

                Case "E"
                    fbProductTitle = CheckForDBNull_String(productRow("ProductDescription")).Trim()
                    fbProductDescription = CheckForDBNull_String(productRow("ProductDescription2")).Trim()
                    If CheckForDBNull_String(productRow("ProductMDTE08")).Trim.Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductMDTE08")).Trim("0").Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductYear")).Trim.Length > 0 Then
                        fbProductDescription = fbProductDescription & " " & GetFormattedProductDate(productRow("ProductMDTE08").ToString.Trim, productRow("ProductYear").ToString.Trim)
                    End If
                    fbProductURL = Request.Url.GetLeftPart(UriPartial.Path) & "?IsSingleProduct=TRUE"
                    fbProductURL = fbProductURL & "&ProductType=" & CheckForDBNull_String(productRow("ProductType")).Trim()
                    fbProductURL = fbProductURL & "&ProductCode=" & CheckForDBNull_String(productRow("ProductCode")).Trim()
                    If CheckForDBNull_String(productRow("ProductSubType")).Trim().Length > 0 Then
                        fbProductURL = fbProductURL & "&ProductSubType=" & CheckForDBNull_String(productRow("ProductSubType")).Trim()
                    End If
                    fbProductImage = ImagePath.getImagePath("APPTHEME", _ucr.Attribute("FacebookLikeThumbnail"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                Case "C"
                    fbProductTitle = CheckForDBNull_String(productRow("ProductDescription")).Trim()
                    fbProductDescription = CheckForDBNull_String(productRow("ProductDescription2")).Trim()
                    ''''
                    If CheckForDBNull_String(productRow("ProductMDTE08")).Trim.Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductMDTE08")).Trim("0").Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductYear")).Trim.Length > 0 Then
                        fbProductDescription = fbProductDescription & " " & GetFormattedProductDate(productRow("ProductMDTE08").ToString.Trim, productRow("ProductYear").ToString.Trim)
                    End If
                    ''''
                    fbProductURL = Request.Url.GetLeftPart(UriPartial.Path) & "?IsSingleProduct=TRUE"
                    fbProductURL = fbProductURL & "&ProductType=" & CheckForDBNull_String(productRow("ProductType")).Trim()
                    fbProductURL = fbProductURL & "&ProductCode=" & CheckForDBNull_String(productRow("ProductCode")).Trim()
                    If CheckForDBNull_String(productRow("ProductSubType")).Trim().Length > 0 Then
                        fbProductURL = fbProductURL & "&ProductSubType=" & CheckForDBNull_String(productRow("ProductSubType")).Trim()
                    End If
                    fbProductImage = ImagePath.getImagePath("APPTHEME", _ucr.Attribute("FacebookLikeThumbnail"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                Case "P" 'Pre Payment Schemes
                    fbProductTitle = CheckForDBNull_String(productRow("ProductDescription")).Trim()
                    fbProductDescription = CheckForDBNull_String(productRow("ProductDescription2")).Trim()
                    ''''
                    If CheckForDBNull_String(productRow("ProductMDTE08")).Trim.Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductMDTE08")).Trim("0").Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductYear")).Trim.Length > 0 Then
                        fbProductDescription = fbProductDescription & " " & GetFormattedProductDate(productRow("ProductMDTE08").ToString.Trim, productRow("ProductYear").ToString.Trim)
                    End If
                    ''''
                    fbProductURL = Request.Url.GetLeftPart(UriPartial.Path) & "?IsSingleProduct=TRUE"
                    fbProductURL = fbProductURL & "&ProductType=" & CheckForDBNull_String(productRow("ProductType")).Trim()
                    fbProductURL = fbProductURL & "&ProductCode=" & CheckForDBNull_String(productRow("ProductCode")).Trim()
                    If CheckForDBNull_String(productRow("ProductSubType")).Trim().Length > 0 Then
                        fbProductURL = fbProductURL & "&ProductSubType=" & CheckForDBNull_String(productRow("ProductSubType")).Trim()
                    End If
                    fbProductImage = ImagePath.getImagePath("APPTHEME", _ucr.Attribute("FacebookLikeThumbnail"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                Case "S" 'Season Ticket Product
                    fbProductTitle = CheckForDBNull_String(productRow("ProductDescription")).Trim()
                    fbProductDescription = CheckForDBNull_String(productRow("ProductDescription2")).Trim()
                    ''''
                    If CheckForDBNull_String(productRow("ProductMDTE08")).Trim.Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductMDTE08")).Trim("0").Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductYear")).Trim.Length > 0 Then
                        fbProductDescription = fbProductDescription & " " & GetFormattedProductDate(productRow("ProductMDTE08").ToString.Trim, productRow("ProductYear").ToString.Trim)
                    End If
                    ''''
                    fbProductURL = Request.Url.GetLeftPart(UriPartial.Path) & "?IsSingleProduct=TRUE"
                    fbProductURL = fbProductURL & "&ProductType=" & CheckForDBNull_String(productRow("ProductType")).Trim()
                    fbProductURL = fbProductURL & "&ProductCode=" & CheckForDBNull_String(productRow("ProductCode")).Trim()
                    If CheckForDBNull_String(productRow("ProductSubType")).Trim().Length > 0 Then
                        fbProductURL = fbProductURL & "&ProductSubType=" & CheckForDBNull_String(productRow("ProductSubType")).Trim()
                    End If
                    fbProductImage = ImagePath.getImagePath("APPTHEME", _ucr.Attribute("FacebookLikeThumbnail"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                Case "CH" 'Match Day Hospitality
                    fbProductTitle = CheckForDBNull_String(productRow("ProductDescription")).Trim()
                    fbProductDescription = CheckForDBNull_String(productRow("ProductDescription2")).Trim()
                    If CheckForDBNull_String(productRow("ProductMDTE08")).Trim.Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductMDTE08")).Trim("0").Length > 0 _
                        AndAlso CheckForDBNull_String(productRow("ProductYear")).Trim.Length > 0 Then
                        fbProductDescription = fbProductDescription & " " & GetFormattedProductDate(productRow("ProductMDTE08").ToString.Trim, productRow("ProductYear").ToString.Trim)
                    End If
                    fbProductURL = Request.Url.GetLeftPart(UriPartial.Path) & "?IsSingleProduct=TRUE"
                    fbProductURL = fbProductURL & "&ProductType=" & CheckForDBNull_String(productRow("ProductType")).Trim()
                    fbProductURL = fbProductURL & "&ProductCode=" & CheckForDBNull_String(productRow("ProductCode")).Trim()
                    If CheckForDBNull_String(productRow("ProductSubType")).Trim().Length > 0 Then
                        fbProductURL = fbProductURL & "&ProductSubType=" & CheckForDBNull_String(productRow("ProductSubType")).Trim()
                    End If
                    fbProductImage = ImagePath.getImagePath("APPTHEME", _ucr.Attribute("FacebookLikeThumbnail"), TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
            End Select
            If fbProductImage = ModuleDefaults.MissingImagePath Then
                fbProductImage = String.Empty
            End If
            If Not String.IsNullOrWhiteSpace(fbProductTitle) Then
                AddFaceBookLikeMetaTag("og:title", fbProductTitle)
            End If
            If Not String.IsNullOrWhiteSpace(fbProductDescription) Then
                AddFaceBookLikeMetaTag("og:description", fbProductDescription)
            End If
            If Not String.IsNullOrWhiteSpace(fbProductURL) Then
                AddFaceBookLikeMetaTag("og:url", fbProductURL)
            End If
            If Not String.IsNullOrWhiteSpace(fbProductImage) Then
                AddFaceBookLikeMetaTag("og:image", fbProductImage)
            End If
            _fbLikeMetaTagsForProductNotExists = False
        End If
    End Sub

    Private Sub AddFaceBookLikeMetaTags()
        AddFaceBookLikeMetaTag("og:type", CheckForDBNull_String(_ucr.Attribute("FacebookLikeType")))
        AddFaceBookLikeMetaTag("og:site_name", CheckForDBNull_String(_ucr.Attribute("FacebookLikeSiteName")))
        AddFaceBookLikeMetaTag("fb:admins", CheckForDBNull_String(_ucr.Attribute("FacebookLikeAdminID")))
    End Sub

    Private Function DisableAccordion(ByVal soldOut As Boolean) As String
        Dim classVals As String
        classVals = String.Empty
        If soldOut Then
            classVals = _cStateSoldOut
            If (_ProductType = GlobalConstants.AWAYPRODUCTTYPE OrElse _ProductType = GlobalConstants.TRAVELPRODUCTTYPE OrElse _ProductType = GlobalConstants.EVENTPRODUCTTYPE) Then
                classVals += _cStateDisabled
            End If
        End If
        Return classVals
    End Function

    Private Function EnableAccordion(ByVal cssString As String) As String
        Dim classVals As String = cssString
        classVals = Replace(classVals, _cStateSoldOut, "")
        classVals = Replace(classVals, _cStateDisabled, "")
        Return classVals
    End Function

#End Region

#Region "Product Filter Functions"

    Private Sub bindAdvancedProductFilterOptions()
        If _ds1 IsNot Nothing AndAlso _ds1.Tables.Count > 0 Then
            If _ds1.Tables(0).Rows(0)("ErrorOccurred").ToString <> "E" Then
                'Bind the information labels
                If _ds1.Tables(1).Rows.Count > 0 Then
                    Dim dtProductList As New DataTable
                    dtProductList = _ds1.Tables(1)
                    Dim rowFilterCondition As String = String.Empty
                    _productDetailHelper.ProductType = _ProductType
                    _productDetailHelper.ProductSubType = _ProductSubType
                    rowFilterCondition = _productDetailHelper.GetRowFilterCondition(_pageNumber)
                    dtProductList.DefaultView.RowFilter = rowFilterCondition
                    If dtProductList.DefaultView.ToTable.Rows.Count > 0 Then
                        plhAdvancedProductFilter.Visible = True
                        SetupLayoutByProductType(dtProductList.DefaultView.ToTable, dtProductList)
                    Else
                        plhAdvancedProductFilter.Visible = False
                    End If
                Else
                    plhAdvancedProductFilter.Visible = False
                End If
            Else
                plhAdvancedProductFilter.Visible = False
            End If
        Else
            plhAdvancedProductFilter.Visible = False
        End If
    End Sub

    Private Sub SetupLayoutByProductType(ByVal dtProductList As DataTable, ByVal UnfilteredDtProductList As DataTable)
        Dim dateArray As String = String.Empty

        'Reset all fields
        ddlAge.Items.Clear()
        ddlDescription.Items.Clear()
        ddlLocation.Items.Clear()
        ddlStadiumDescription.Items.Clear()
        ddlDuration.Items.Clear()
        plhAge.Visible = False
        plhDescription.Visible = False
        plhLocation.Visible = False
        plhStadiumDescription.Visible = False
        plhDate.Visible = False
        plhDuration.Visible = False

        ltlLegend.Text = _ucr.Content("TitleText", _languageCode, True)
        Dim defaultListItemText As String = _ucr.Content("DefaultSelectedText", _languageCode, True)

        Select Case _ProductType
            Case Is = GlobalConstants.HOMEPRODUCTTYPE : layoutForCommonProducts(defaultListItemText, dtProductList)
            Case Is = GlobalConstants.AWAYPRODUCTTYPE : layoutForCommonProducts(defaultListItemText, dtProductList)
            Case Is = GlobalConstants.SEASONTICKETPRODUCTTYPE : layoutForCommonProducts(defaultListItemText, dtProductList)
            Case Is = GlobalConstants.TRAVELPRODUCTTYPE : layoutForTProducts(defaultListItemText, dtProductList)
            Case Is = GlobalConstants.EVENTPRODUCTTYPE : layoutForEProducts(defaultListItemText, dtProductList)
            Case Else : plhAdvancedProductFilter.Visible = False
        End Select

        'Check each product to work out available date array
        For Each row As DataRow In UnfilteredDtProductList.Rows
            If String.IsNullOrEmpty(dateArray) Then
                dateArray += Talent.Common.Utilities.ISeriesDate(row("ProductMDTE08")).ToString("[MM, dd, yyyy]")
            Else
                dateArray += Talent.Common.Utilities.ISeriesDate(row("ProductMDTE08")).ToString(",[MM, dd, yyyy]")
            End If
        Next

        If plhAdvancedProductFilter.Visible Then
            ltlAvailableDaysScript.Text = "" &
                    "<script type=""text/javascript"">" & vbCrLf &
                    "<!--" & vbCrLf &
                    "var availableDays = [" & dateArray & "];" & vbCrLf &
                    "//-->" & vbCrLf &
                    "</script>"
        End If
    End Sub

    Private Sub layoutForTProducts(ByVal defaultListItemText As String, ByRef dtProductList As DataTable)
        'Age
        If CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("PanelAgeVisible")) Then
            plhAge.Visible = True
            lblAge.Text = _ucr.Content("AgeLabelText", _languageCode, True)
            ddlAge.Items.Add(New ListItem(defaultListItemText, _defaultListItemValue))
        End If
        'Description
        If CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("PanelDescriptionVisible")) Then
            plhDescription.Visible = True
            lblDescription.Text = _ucr.Content("DescriptionLabelText", _languageCode, True)
            ddlDescription.Items.Add(New ListItem(defaultListItemText, _defaultListItemValue))
        End If
        'Location
        If CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("PanelLocationVisible")) Then
            plhLocation.Visible = True
            lblLocation.Text = _ucr.Content("LocationLabelText", _languageCode, True)
            ddlLocation.Items.Add(New ListItem(defaultListItemText, _defaultListItemValue))
        End If
        'Date
        If CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("PanelDateVisible")) Then
            plhDate.Visible = True
            ltlDate.Text = _ucr.Content("DateLabelText", _languageCode, True)
            txtDate.Text = defaultListItemText
        End If
        'Duration
        If CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("PanelDurationVisible")) Then
            plhDuration.Visible = True
            lblDuration.Text = _ucr.Content("DurationLabelText", _languageCode, True)
            ddlDuration.Items.Add(New ListItem(defaultListItemText, _defaultListItemValue))
        End If
        'Search Button
        btnSearch.Text = _ucr.Content("SearchButtonText", _languageCode, True)
        'Clear Button
        If CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("PanelClearButtonVisible")) Then
            plhClearButton.Visible = True
            btnClear.Text = _ucr.Content("ClearButtonText", _languageCode, True)
        End If

        'Description, Date Array data population
        If plhDescription.Visible Then
            Dim dvProductListByDescription2 = New DataView(dtProductList)
            dvProductListByDescription2.Sort = "ProductDescription2 ASC"
            For Each row As DataRow In dvProductListByDescription2.ToTable.Rows
                Dim productDescription2 As String = row("ProductDescription2").ToString.Trim
                If Not String.IsNullOrEmpty(productDescription2) Then
                    If ddlDescription.Items.FindByValue(productDescription2) Is Nothing Then
                        ddlDescription.Items.Add(productDescription2)
                    End If
                End If
            Next
        End If
        If plhLocation.Visible Then
            Dim dvProductListByLocation = New DataView(dtProductList)
            dvProductListByLocation.Sort = "location ASC"
            For Each row As DataRow In dvProductListByLocation.ToTable.Rows
                Dim location As String = row("location").ToString.Trim
                If Not String.IsNullOrEmpty(location) Then
                    If ddlLocation.Items.FindByValue(location) Is Nothing Then
                        ddlLocation.Items.Add(location)
                    End If
                End If
            Next
        End If
        If plhDuration.Visible Then
            Dim dvProductListByDuration = New DataView(dtProductList)
            dvProductListByDuration.Sort = "duration ASC"
            For Each row As DataRow In dvProductListByDuration.ToTable.Rows
                Dim duration As String = row("duration").ToString.Trim
                If Not String.IsNullOrEmpty(duration) Then
                    If ddlDuration.Items.FindByValue(duration) Is Nothing Then
                        ddlDuration.Items.Add(duration)
                    End If
                End If
            Next
        End If
     
    End Sub

    Private Sub layoutForEProducts(ByVal defaultListItemText As String, ByRef dtProductList As DataTable)
        'Age
        If CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("PanelAgeVisible")) Then
            plhAge.Visible = True
            lblAge.Text = _ucr.Content("AgeLabelText", _languageCode, True)
            ddlAge.Items.Add(New ListItem(defaultListItemText, _defaultListItemValue))
        End If
        'Description
        If CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("PanelDescriptionVisible")) Then
            plhDescription.Visible = True
            lblDescription.Text = _ucr.Content("DescriptionLabelText", _languageCode, True)
            ddlDescription.Items.Add(New ListItem(defaultListItemText, _defaultListItemValue))
        End If
        'Location
        If CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("PanelLocationVisible")) Then
            plhLocation.Visible = True
            lblLocation.Text = _ucr.Content("LocationLabelText", _languageCode, True)
            ddlLocation.Items.Add(New ListItem(defaultListItemText, _defaultListItemValue))
        End If
        'Date
        If CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("PanelDateVisible")) Then
            plhDate.Visible = True
            ltlDate.Text = _ucr.Content("DateLabelText", _languageCode, True)
            txtDate.Text = defaultListItemText
        End If
        'Duration
        If CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("PanelDurationVisible")) Then
            plhDuration.Visible = True
            lblDuration.Text = _ucr.Content("DurationLabelText", _languageCode, True)
            ddlDuration.Items.Add(New ListItem(defaultListItemText, _defaultListItemValue))
        End If
        'Search Button
        btnSearch.Text = _ucr.Content("SearchButtonText", _languageCode, True)
        'Clear Button
        If CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("PanelClearButtonVisible")) Then
            plhClearButton.Visible = True
            btnClear.Text = _ucr.Content("ClearButtonText", _languageCode, True)
        End If

        'Age, Description, Location, Duration, Date Array data population
        If plhDescription.Visible Then
            Dim dvProductListByDescription = New DataView(dtProductList)
            dvProductListByDescription.Sort = "ProductDescription ASC"
            For Each row As DataRow In dvProductListByDescription.ToTable.Rows
                Dim productDescription As String = row("ProductDescription").ToString.Trim
                If Not String.IsNullOrEmpty(productDescription) Then
                    If ddlDescription.Items.FindByValue(productDescription) Is Nothing Then
                        ddlDescription.Items.Add(productDescription)
                    End If
                End If
            Next
        End If
        If plhLocation.Visible Then
            Dim dvProductListByLocation = New DataView(dtProductList)
            dvProductListByLocation.Sort = "location ASC"
            For Each row As DataRow In dvProductListByLocation.ToTable.Rows
                Dim location As String = row("location").ToString.Trim
                If Not String.IsNullOrEmpty(location) Then
                    If ddlLocation.Items.FindByValue(location) Is Nothing Then
                        ddlLocation.Items.Add(location)
                    End If
                End If
            Next
        End If
        If plhDuration.Visible Then
            Dim dvProductListByDuration = New DataView(dtProductList)
            dvProductListByDuration.Sort = "duration ASC"
            For Each row As DataRow In dvProductListByDuration.ToTable.Rows
                Dim duration As String = row("duration").ToString.Trim
                If Not String.IsNullOrEmpty(duration) Then
                    If ddlDuration.Items.FindByValue(duration) Is Nothing Then
                        ddlDuration.Items.Add(duration)
                    End If
                End If
            Next
        End If
        For Each row As DataRow In dtProductList.Rows
            Dim ageRangeFrm As String = row("ageRangeFrm").ToString.Trim
            Dim ageRangeTo As String = row("ageRangeTo").ToString.Trim
            If Not String.IsNullOrEmpty(ageRangeFrm) And Not String.IsNullOrEmpty(ageRangeTo) Then
                Try
                    If CInt(ageRangeFrm) > 0 And CInt(ageRangeTo) > 0 Then
                        Dim ageRange As String = ageRangeFrm & _ucr.Content("AgeRangeDividerText", _languageCode, True) & ageRangeTo
                        If ddlAge.Items.FindByValue(ageRange) Is Nothing Then
                            ddlAge.Items.Add(ageRange)
                        End If
                    End If
                Catch ex As Exception
                End Try
            End If
        Next
    End Sub

    Private Sub layoutForCommonProducts(ByVal defaultListItemText As String, ByRef dtProductList As DataTable)
        'Location
        If CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("PanelLocationVisible")) Then
            plhLocation.Visible = True
            lblLocation.Text = _ucr.Content("LocationLabelText", _languageCode, True)
            ddlLocation.Items.Add(New ListItem(defaultListItemText, _defaultListItemValue))
        End If
        'Stadium Description
        If CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("PanelStadiumDescriptionVisible")) Then
            plhStadiumDescription.Visible = True
            lblStadiumDescription.Text = _ucr.Content("StadiumDescriptionLabelText", _languageCode, True)
            ddlStadiumDescription.Items.Add(New ListItem(defaultListItemText, _defaultListItemValue))
        End If
        'Date
        If CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("PanelDateVisible")) Then
            plhDate.Visible = True
            ltlDate.Text = _ucr.Content("DateLabelText", _languageCode, True)
            txtDate.Text = defaultListItemText
        End If

        'Search Button
        btnSearch.Text = _ucr.Content("SearchButtonText", _languageCode, True)
        'Clear Button
        If CheckForDBNullOrBlank_Boolean_DefaultTrue(_ucr.Attribute("PanelClearButtonVisible")) Then
            plhClearButton.Visible = True
            btnClear.Text = _ucr.Content("ClearButtonText", _languageCode, True)
        End If

        'Location and Stadium Description data population
        If plhLocation.Visible Then
            Dim dvProductListByLocation = New DataView(dtProductList)
            dvProductListByLocation.Sort = "location ASC"
            For Each row As DataRow In dvProductListByLocation.ToTable.Rows
                Dim location As String = row("location").ToString.Trim
                If Not String.IsNullOrEmpty(location) Then
                    If ddlLocation.Items.FindByValue(location) Is Nothing Then
                        ddlLocation.Items.Add(location)
                    End If
                End If
            Next
        End If
        If plhStadiumDescription.Visible Then
            Dim dvProductListByProductStadiumDescription = New DataView(dtProductList)
            dvProductListByProductStadiumDescription.Sort = "ProductStadiumDescription ASC"
            For Each row As DataRow In dvProductListByProductStadiumDescription.ToTable.Rows
                Dim productStadiumDescription As String = row("ProductStadiumDescription").ToString.Trim
                If Not String.IsNullOrEmpty(productStadiumDescription) Then
                    If ddlStadiumDescription.Items.FindByValue(productStadiumDescription) Is Nothing Then
                        ddlStadiumDescription.Items.Add(productStadiumDescription)
                    End If
                End If
            Next
        End If

    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Session("AdvancedSearchInUse") = True
        If plhAge.Visible Then Session("AgeSearch") = ddlAge.SelectedValue
        If plhDescription.Visible Then Session("DescriptionSearch") = ddlDescription.SelectedValue.Replace("'", "''")
        If plhLocation.Visible Then Session("LocationSearch") = ddlLocation.SelectedValue.Replace("'", "''")
        If plhStadiumDescription.Visible Then Session("StadiumDescriptionSearch") = ddlStadiumDescription.SelectedValue.Replace("'", "''")
        If plhDuration.Visible Then Session("DurationSearch") = ddlDuration.SelectedValue
        If txtDate.Text = _ucr.Content("DefaultSelectedText", _languageCode, True) Then
            Session("DateSearch") = "0"
            Session("DateSearchForCalendar") = String.Empty
        Else
            Try
                Session("DateSearch") = "1" & CDate(txtDate.Text).ToString("yyMMdd")
                Session("DateSearchForCalendar") = txtDate.Text
            Catch ex As Exception
                Session("DateSearch") = "0"
                Session("DateSearchForCalendar") = String.Empty
            End Try
        End If
        Session("AgeRangeDividerText") = _ucr.Content("AgeRangeDividerText", _languageCode, True)
        Session("ClearPageNumberQueryString") = "TRUE"
        Session("ConsiderRedirectAsPostback") = True
        Dim url As String = Request.Url.AbsoluteUri
        Try
            Dim _nameValueCollection As New NameValueCollection
            _nameValueCollection = HttpUtility.ParseQueryString(url)
            _nameValueCollection.Remove("page")
            url = Server.UrlDecode(_nameValueCollection.ToString)
        Catch ex As Exception
        End Try
        Response.Redirect(url)
    End Sub

    Protected Sub btnClear_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        _productDetailHelper.ClearAdvPdtFilterSession()
        Dim url As String = Request.Url.AbsoluteUri
        Try
            Dim _nameValueCollection As New NameValueCollection
            _nameValueCollection = HttpUtility.ParseQueryString(url)
            _nameValueCollection.Remove("page")
            url = Server.UrlDecode(_nameValueCollection.ToString)
        Catch ex As Exception
        End Try
        Response.Redirect(url)
    End Sub

    Private Sub SetProductFilterFieldsFromSessions(ByVal canSetFields As Boolean, ByVal clearConsiderRedirectAsPostback As Boolean)
        Dim canSet As Boolean = False
        If Session("ConsiderRedirectAsPostback") IsNot Nothing AndAlso Session("ConsiderRedirectAsPostback") = True Then
            canSet = True
        ElseIf _pageNumber Is Nothing OrElse _pageNumber.Length = 0 Then
            canSet = False
        Else
            canSet = True
        End If
        If clearConsiderRedirectAsPostback Then
            Session("ConsiderRedirectAsPostback") = Nothing
        End If
        If canSet Then
            If canSetFields Then
                SetProductFilterFieldsFromSessions()
            End If
        Else
            _productDetailHelper.ClearAdvPdtFilterSession()
        End If
    End Sub

    Private Sub SetProductFilterFieldsFromSessions()
        If Session("AdvancedSearchInUse") IsNot Nothing AndAlso Session("AdvancedSearchInUse") = True Then
            If plhAge.Visible Then ddlAge.SelectedValue = Session("AgeSearch")
            If plhDescription.Visible Then ddlDescription.SelectedValue = Session("DescriptionSearch")
            If plhLocation.Visible Then ddlLocation.SelectedValue = Session("LocationSearch")
            If plhStadiumDescription.Visible Then ddlStadiumDescription.SelectedValue = Session("StadiumDescriptionSearch")
            If plhDate.Visible Then txtDate.Text = Session("DateSearchForCalendar")
            If plhDuration.Visible Then ddlDuration.SelectedValue = Session("DurationSearch")
        End If
    End Sub

#End Region

End Class

#Region " Class to list Distinct items for a given DataTable updated to show ProductCompetition details"
Public Class DataSetHelperProdouct

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Public ds As DataSet
    Public ucr As New Talent.Common.UserControlResource
    Public Sub New(ByRef DataSet As DataSet)
        ds = DataSet
    End Sub
    Public Sub New()
        ds = Nothing
    End Sub

    Private Function ColumnEqual(ByVal A As Object, ByVal B As Object) As Boolean

        ' Compares two values to see if they are equal. Also compares DBNULL.Value.
        ' Note: If your DataTable contains object fields, then you must extend this
        ' function to handle them in a meaningful way if you intend to group on them.

        If A.Equals(DBNull.Value) AndAlso B.Equals(DBNull.Value) Then
            Return True
            '  both are DBNull.Value
        End If
        If A.Equals(DBNull.Value) OrElse B.Equals(DBNull.Value) Then
            Return False
            '  only one is DBNull.Value
        End If
        Return (A.Equals(B))
        ' value type standard comparison
    End Function

    Public Function SelectDistinct(ByVal TableName As String, ByVal SourceTable As DataTable, ByVal TextFieldName As String, ByVal TextFieldType As Type, ByVal ValueFieldName As String, ByVal ValueFieldType As Type) As DataTable

        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ProductDetail.ascx"
        End With

        Dim dt As New DataTable(TableName)
        Dim LastValue As Object = Nothing
        Dim sourceDT As New DataTable
        Dim strCompetition As String = String.Empty

        dt.Columns.Add(TextFieldName, TextFieldType)
        dt.Columns.Add(ValueFieldName, ValueFieldType)
        SourceTable.DefaultView.Sort = ValueFieldName
        sourceDT = SourceTable.DefaultView.ToTable()

        For Each dr As DataRow In sourceDT.[Select]("", ValueFieldName)
            If LastValue Is Nothing OrElse Not (ColumnEqual(LastValue, dr(ValueFieldName).ToString.Trim)) Then
                LastValue = dr(ValueFieldName).ToString.Trim
                strCompetition = ucr.Content("ProductCompetition" + dr(ValueFieldName).ToString.Trim, _languageCode, True)
                If strCompetition.Length > 0 Then dt.Rows.Add(strCompetition, dr(ValueFieldName).ToString.Trim)
            End If
        Next

        Return dt

    End Function

End Class

#End Region