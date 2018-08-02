Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Data
Imports System.Globalization

Partial Class PagesLogin_Orders_PurchaseHistory
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfr As WebFormResource = Nothing
    Private _languageCode As String = Nothing
    Private _dsPurchaseHistory As DataSet = Nothing
    Private _paymentRefRegex As String
    Private _hasBulkSales As Boolean = False
    Private _pagerAction As String = String.Empty
    Private _customerHasTicketingPurchaseHistory As Boolean = False
    Private _customerHasRetailPurchaseHistory As Boolean = False
    Const RETAIL_DESCRIPTIONS_QUALIFIER As String = "OrderEnquiry"
    Const RETAIL_DESCRIPTIONS_TYPE As String = "BackOfficeStatus"

#End Region

#Region "Public Properties"

    Public ProductColumnHeading As String
    Public PaymentRefColumnHeading As String
    Public QuantityColumnHeading As String
    Public DateColumnHeading As String
    Public CustomerColumnHeading As String
    Public PriceColumnHeading As String
    Public StatusColumnHeading As String
    Public RetailPurchaseRefColumnHeading As String
    Public Direction As String
    Public IsLastPage As Boolean
    Public ShowPrices As Boolean

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Profile.IsAnonymous Then Response.Redirect("~/PagesPublic/Login/Login.aspx")
        If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessPurchaseHistory) Or Not AgentProfile.IsAgent Then
            _wfr = New WebFormResource
            _languageCode = TCUtilities.GetDefaultLanguage()
            _dsPurchaseHistory = New DataSet
            With _wfr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .PageCode = "PurchaseHistory.aspx"
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "PurchaseHistory.aspx"
            End With
            _paymentRefRegex = _wfr.Attribute("PaymentRefRegex")
            revPaymentReference.ValidationExpression = _paymentRefRegex
            revPaymentReference.ErrorMessage = _wfr.Content("PaymentRefRegexErrorText", _languageCode, True)
            populateTextLabels()
            populateProductTypes()
            setFormValues()
            setStatusList()
            setFormVisibility()
            If Not IsPostBack Then
                bindRepeater()
            End If

            Session("ProductDetailsPath") = "PurchaseHistory"
            chkTicketingProducts.Attributes.Add("onchange", "ticketOptions();")
            chkRetailProducts.Attributes.Add("onchange", "retailOptions();")
            rgvFromDate.MinimumValue = "01/01/1900"
            rgvFromDate.MaximumValue = DateTime.Today.ToString("dd/MM/yyyy")
            rgvToDate.MinimumValue = "01/01/1900"
            rgvToDate.MaximumValue = DateTime.Today.ToString("dd/MM/yyyy")
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub rptOrderHeaderHistory_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptOrderHeaderHistory.ItemDataBound
        If e.Item.ItemType <> ListItemType.Footer Then
            Dim productcolumn As HtmlTableCell = CType(e.Item.FindControl("productcolumn"), HtmlTableCell)
            Dim plhRetailPurchaseRef As PlaceHolder = CType(e.Item.FindControl("plhRetailPurchaseRef"), PlaceHolder)
            productcolumn.Visible = False
            If chkTicketingProducts.Checked Then
                productcolumn.Visible = True
            End If
            plhRetailPurchaseRef.Visible = Not TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_wfr.Attribute("RetailFromTicketingDB"))
        End If
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim hplDetails As HyperLink = CType(e.Item.FindControl("hplDetails"), HyperLink)
            Dim ltlSaleDate As Literal = CType(e.Item.FindControl("ltlSaleDate"), Literal)
            Dim saleDate As Date = CDate(e.Item.DataItem("SALE_DATE"))
            Dim plhBackOfficerOrderNumber As PlaceHolder = CType(e.Item.FindControl("plhBackOfficerOrderNumber"), PlaceHolder)
            Dim plhWebOrderNumber As PlaceHolder = CType(e.Item.FindControl("plhWebOrderNumber"), PlaceHolder)
            Dim ltlRetailProduct As Literal = CType(e.Item.FindControl("ltlRetailProduct"), Literal)
            If e.Item.DataItem("IS_TICKETING_OR_RETAIL") = GlobalConstants.TICKETINGTYPE Then
                Dim ltlPaymentReference As Literal = CType(e.Item.FindControl("ltlPaymentReference"), Literal)
                If e.Item.DataItem("PAYMENT_REF") = "Not Applicable" Then
                    ltlPaymentReference.Text = _wfr.Content("NoPaymentReference", _languageCode, True)
                    hplDetails.Visible = False
                Else
                    If TEBUtilities.CheckForDBNull_String(e.Item.DataItem("STATUS")).ToString() = "RETURN" Then
                        hplDetails.Visible = False
                    Else
                        hplDetails.NavigateUrl = "~/PagesLogin/Orders/PurchaseDetails.aspx?payref=" & e.Item.DataItem("PAYMENT_REF")
                    End If
                    ltlPaymentReference.Text = e.Item.DataItem("PAYMENT_REF")
                End If
                ltlSaleDate.Text = TEBUtilities.GetFormattedDateAndTime(saleDate.ToString("dd/MM/yyyy"), String.Empty, " ", ModuleDefaults.GlobalDateFormat, ModuleDefaults.Culture)
                plhBackOfficerOrderNumber.Visible = False
                plhWebOrderNumber.Visible = False
            ElseIf e.Item.DataItem("IS_TICKETING_OR_RETAIL") = GlobalConstants.RETAILTYPE Then
                Dim ltlWebOrderNumberLabel As Literal = CType(e.Item.FindControl("ltlWebOrderNumberLabel"), Literal)
                Dim ltlBackOfficerOrderNumberLabel As Literal = CType(e.Item.FindControl("ltlBackOfficerOrderNumberLabel"), Literal)
                If e.Item.DataItem("RETAIL_WEB_ORDER_NUMBER").ToString().Length > 0 Then
                    ltlWebOrderNumberLabel.Text = _wfr.Content("WebOrderNumberLabel", _languageCode, True)
                    plhWebOrderNumber.Visible = True
                Else
                    plhWebOrderNumber.Visible = False
                End If
                If e.Item.DataItem("RETAIL_BACK_OFFICE_ORDER_NUMBER").ToString().Length > 0 Then
                    plhBackOfficerOrderNumber.Visible = True
                    ltlBackOfficerOrderNumberLabel.Text = _wfr.Content("BackOfficerOrderNumberLabel", _languageCode, True)
                Else
                    plhBackOfficerOrderNumber.Visible = False
                End If
                ltlRetailProduct.Text = _wfr.Content("RetailOrderItemText", _languageCode, True)
                hplDetails.NavigateUrl = "~/PagesLogin/Orders/OrderDetails.aspx?wid=" & e.Item.DataItem("RETAIL_WEB_ORDER_NUMBER")
                ltlSaleDate.Text = TEBUtilities.GetFormattedDateAndTime(saleDate.ToString("dd/MM/yyyy"), saleDate.ToString("HH:mm"), " ", ModuleDefaults.GlobalDateFormat, ModuleDefaults.Culture)
            End If
            If e.Item.DataItem("BULK_ID") > 0 Then
                _hasBulkSales = True
            End If
        End If
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If txtPaymentReference.Text.Length > 0 AndAlso plhTicketingProducts.Visible Then chkTicketingProducts.Checked = True
        If txtProductOrDescription.Text.Length > 0 AndAlso plhProductOrDescription.Visible Then chkTicketingProducts.Checked = True
        If txtPackageID.Text.Length > 0 AndAlso plhTicketingProducts.Visible Then chkTicketingProducts.Checked = True
        If ddlStatus.SelectedValue = "BOOK" Then chkShowBooked.Checked = True
        If ddlStatus.SelectedValue = "RESERV" Then chkShowReservations.Checked = True
        setSessionValues()
        hdnCurrentPage.Value = 0
        hdnLastRRN.Value = 0
        hdnRRNOfFirstRecordOnPage.Value = 0
        bindRepeater()
    End Sub

    Protected Sub btnClear_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClear.Click
        clearSessionValues()
        Response.Redirect("PurchaseHistory.aspx")
    End Sub

    Protected Sub PaymentRefValidation(ByVal sender As Object, ByVal e As ServerValidateEventArgs)
        Dim paymentRegex As New Regex(_paymentRefRegex)
        If paymentRegex.IsMatch(txtPaymentReference.Text) OrElse String.IsNullOrEmpty(txtPaymentReference.Text) Then
            blErrorMessages.Items.Add("ERROR")
            e.IsValid = True
        Else
            e.IsValid = False
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If blErrorMessages.Items.Count > 0 Then
            plhErrorList.Visible = True
        Else
            plhErrorList.Visible = False
        End If
    End Sub

    Protected Sub PagerButtonClick(ByVal sender As Object, ByVal e As System.EventArgs)

        Select Case DirectCast(sender, System.Web.UI.WebControls.LinkButton).CommandArgument
            Case "N"
                Direction = "N"
            Case "P"
                Direction = "P"
            Case "F"
                Direction = "F"
            Case "L"
                Direction = "L"
        End Select

        bindRepeater()
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Populate the static text values that are configured to display
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateTextLabels()
        lblFromDate.Text = _wfr.Content("FromDateLabel", _languageCode, True)
        lblToDate.Text = _wfr.Content("ToDateLabel", _languageCode, True)
        lblStatus.Text = _wfr.Content("StatusLabel", _languageCode, True)
        lblProductType.Text = _wfr.Content("ProductTypeLabel", _languageCode, True)
        lblTicketingProducts.Text = _wfr.Content("ShowTicketingProductsLabel", _languageCode, True)
        lblCorporateProducts.Text = _wfr.Content("ShowCorporateProductsLabel", _languageCode, True)
        lblRetailProducts.Text = _wfr.Content("ShowRetailProductsLabel", _languageCode, True)
        lblPaymentReference.Text = _wfr.Content("TicketingPaymentRefLabel", _languageCode, True)
        lblProductOrDescription.Text = _wfr.Content("ProductOrDescriptionSearchLabel", _languageCode, True)
        btnSearch.Text = _wfr.Content("filterButton", _languageCode, True)
        btnClear.Text = _wfr.Content("ClearButtonText", _languageCode, True)
        lblPackageID.Text = _wfr.Content("TicketingPackageIDLabel", _languageCode, True)

        lblShowBuybackProducts.Text = _wfr.Content("ShowBuyBackProductsText", _languageCode, True)
        lblShowLoyaltyInformation.Text = _wfr.Content("ShowLoyaltyInformationText", _languageCode, True)
        lblReservations.Text = _wfr.Content("ShowReservationsText", _languageCode, True)
        lblBooked.Text = _wfr.Content("ShowBookedText", _languageCode, True)
        rgvToDate.ErrorMessage = _wfr.Content("FutureDatesErrorText", _languageCode, True)
        rgvFromDate.ErrorMessage = rgvToDate.ErrorMessage
        cmpToDate.ErrorMessage = _wfr.Content("FromToDateCompareErrorText", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Populate the product types drop down list
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateProductTypes()
        ddlProductType.Items.Clear()
        Dim dicProductTypes As New Generic.Dictionary(Of String, String)
        dicProductTypes.Add("ALL", "ALL")
        dicProductTypes.Add(GlobalConstants.HOMEPRODUCTTYPE, _wfr.Content("ProductType-H", _languageCode, True))
        dicProductTypes.Add(GlobalConstants.AWAYPRODUCTTYPE, _wfr.Content("ProductType-A", _languageCode, True))
        dicProductTypes.Add(GlobalConstants.SEASONTICKETPRODUCTTYPE, _wfr.Content("ProductType-S", _languageCode, True))
        dicProductTypes.Add(GlobalConstants.MEMBERSHIPPRODUCTTYPE, _wfr.Content("ProductType-C", _languageCode, True))
        dicProductTypes.Add(GlobalConstants.TRAVELPRODUCTTYPE, _wfr.Content("ProductType-T", _languageCode, True))
        dicProductTypes.Add(GlobalConstants.EVENTPRODUCTTYPE, _wfr.Content("ProductType-E", _languageCode, True))
        ddlProductType.DataSource = dicProductTypes
        ddlProductType.DataTextField = "Value"
        ddlProductType.DataValueField = "Key"
        ddlProductType.DataBind()
    End Sub

    ''' <summary>
    ''' Set the various form objects to display based on the customer purchase history available
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setFormVisibility()
        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("ShowRetailPurchaseHistoryOptions")) Then
            plhRetailProducts.Visible = True
        Else
            plhRetailProducts.Visible = False
            chkRetailProducts.Checked = False
        End If
        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("ShowTicketingPurchaseHistoryOptions")) Then
            plhTicketingProducts.Visible = True
            plhProductType.Visible = True
            plhPackageID.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("ShowPackageProductsOption"))
            plhCorporateProducts.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("ShowCorporateProductsCheckBox"))
            plhBuybackProducts.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("ShowBuybackProductsCheckBox"))
            plhLoyaltyInformation.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("ShowLoyaltyInformationCheckBox"))
            plhShowReservations.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("ShowReservationsCheckBox"))
            plhShowBooked.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("ShowBookedCheckBox"))
            plhProductOrDescription.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("ShowProductOrDescriptionSearchBox"))
        Else
            plhTicketingProducts.Visible = False
            plhProductType.Visible = False
            plhPackageID.Visible = False
            plhCorporateProducts.Visible = False
            plhBuybackProducts.Visible = False
            plhShowReservations.Visible = False
            plhShowBooked.Visible = False
            plhProductOrDescription.Visible = False
            plhLoyaltyInformation.Visible = False
            chkTicketingProducts.Checked = False
        End If
        'Hide the "show retail/ticketing products" tick boxes if we're displaying ONLY retail OR ticketing as these are only required when we show both retail and ticketing together
        If plhTicketingProducts.Visible AndAlso Not plhRetailProducts.Visible Then plhTicketingProducts.Visible = False
        If Not plhTicketingProducts.Visible AndAlso plhRetailProducts.Visible Then plhRetailProducts.Visible = False
        If (Profile.PartnerInfo.Details IsNot Nothing) Then
            ShowPrices = Not TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(Profile.PartnerInfo.Details.HIDE_PRICES)
        Else
            ShowPrices = True
        End If
    End Sub

    ''' <summary>
    ''' Populate the status list
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setStatusList()
        ddlStatus.Items.Clear()
        Dim descAdapter As New TalentDescriptionsDataSetTableAdapters.tbl_ebusiness_descriptions_buTableAdapter
        Dim descs As TalentDescriptionsDataSet.tbl_ebusiness_descriptions_buDataTable = descAdapter.GetDatabyBUetc(RETAIL_DESCRIPTIONS_QUALIFIER, _wfr.BusinessUnit, TalentCache.GetPartner(Profile), RETAIL_DESCRIPTIONS_TYPE)
        If descs.Rows.Count > 0 Then
            ddlStatus.DataSource = descs
            ddlStatus.DataValueField = "DESCRIPTION_CODE"
            ddlStatus.DataTextField = "LANGUAGE_DESCRIPTION"
            ddlStatus.DataBind()
        Else
            descs = descAdapter.GetDatabyBUetc(RETAIL_DESCRIPTIONS_QUALIFIER, _wfr.BusinessUnit, TCUtilities.GetAllString, RETAIL_DESCRIPTIONS_TYPE)
            If descs.Rows.Count > 0 Then
                ddlStatus.DataSource = descs
                ddlStatus.DataValueField = "DESCRIPTION_CODE"
                ddlStatus.DataTextField = "LANGUAGE_DESCRIPTION"
                ddlStatus.DataBind()
            End If
        End If

        If _wfr.Content("StatusOptions", _languageCode, True).Trim.Length > 0 Then
            Dim arrOption As String() = _wfr.Content("StatusOptions", _languageCode, True).Trim.Split(";")
            If arrOption.Length > 0 Then
                For arrIndex As Integer = 0 To arrOption.Length - 1
                    Dim tempListItem As New ListItem
                    tempListItem.Text = _wfr.Content(arrOption(arrIndex).Trim, _languageCode, True)
                    tempListItem.Value = arrOption(arrIndex).Trim
                    ddlStatus.Items.Add(tempListItem)
                Next
            End If
        End If

        If ddlStatus.Items.Count > 0 Then
            plhStatus.Visible = True
        Else
            plhStatus.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Clear session state search values so they are no longer used
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub clearSessionValues()
        Session("SearchClicked") = Nothing
        Session("OrderHistoryFromDate") = Nothing
        Session("OrderHistoryToDate") = Nothing
        Session("OrderHistoryStatus") = Nothing
        Session("OrderHistoryProductType") = Nothing
        Session("OrderHistoryCorporateProducts") = Nothing
        Session("OrderHistoryRetailProducts") = Nothing
        Session("OrderHistoryTicketingProducts") = Nothing
        Session("OrderHistoryAgent") = Nothing
        Session("OrderHistoryCustomerNumber") = Nothing
        Session("OrderHistoryPaymentRef") = Nothing
        Session("OrderHistoryProductOrDescription") = Nothing
        Session("OrderHistoryOrderNumber") = Nothing
        Session("OrderHistoryPackageID") = Nothing
    End Sub

    ''' <summary>
    ''' Clear the form values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub clearFormValues()
        txtFromDate.Text = String.Empty
        txtToDate.Text = String.Empty
        setStatusList()
        populateProductTypes()
        chkRetailProducts.Checked = True
        chkTicketingProducts.Checked = True
        chkCorporateProducts.Checked = True
        txtPaymentReference.Text = String.Empty
        txtProductOrDescription.Text = String.Empty
        txtPackageID.Text = String.Empty
    End Sub

    ''' <summary>
    ''' Set the selected search values to session state so that they can be used when the user pages through search results.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setSessionValues()
        Session("OrderHistoryFromDate") = txtFromDate.Text
        Session("OrderHistoryToDate") = txtToDate.Text
        Session("OrderHistoryStatus") = ddlStatus.SelectedValue
        Session("OrderHistoryProductType") = ddlProductType.SelectedValue
        Session("OrderHistoryCorporateProducts") = chkCorporateProducts.Checked
        Session("OrderHistoryRetailProducts") = chkRetailProducts.Checked
        Session("OrderHistoryTicketingProducts") = chkRetailProducts.Checked
        Session("OrderHistoryPaymentRef") = txtPaymentReference.Text
        Session("OrderHistoryProductOrDescription") = txtProductOrDescription.Text
        Session("OrderHistoryPackageID") = txtPackageID.Text
    End Sub

    ''' <summary>
    ''' Set the form values based on the current session values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setFormValues()
        If Request.QueryString("page") Is Nothing OrElse IsPostBack Then
            clearSessionValues()
        Else
            If Session("OrderHistoryFromDate") IsNot Nothing Then txtFromDate.Text = Session("OrderHistoryFromDate")
            If Session("OrderHistoryToDate") IsNot Nothing Then txtToDate.Text = Session("OrderHistoryToDate")
            If Session("OrderHistoryStatus") IsNot Nothing Then ddlStatus.SelectedValue = Session("OrderHistoryStatus")
            If Session("OrderHistoryProductType") IsNot Nothing Then ddlProductType.SelectedValue = Session("OrderHistoryProductType")
            If Session("OrderHistoryRetailProducts") IsNot Nothing Then chkRetailProducts.Checked = Session("OrderHistoryRetailProducts")
            If Session("OrderHistoryTicketingProducts") IsNot Nothing Then chkTicketingProducts.Checked = Session("OrderHistoryTicketingProducts")
            If Session("OrderHistoryPaymentRef") IsNot Nothing Then txtPaymentReference.Text = Session("OrderHistoryPaymentRef")
            If Session("OrderHistoryProductOrDescription") IsNot Nothing Then txtProductOrDescription.Text = Session("OrderHistoryProductOrDescription")
            If Session("OrderHistoryPackageID") IsNot Nothing Then txtPackageID.Text = Session("OrderHistoryPackageID")
        End If
        ' Initially assign default value from tbl_page_attribute table for both PWS & BoxOffice
        chkCorporateProducts.Checked = _wfr.Attribute("IncludeCorporateHospitalityItems")
        ' For BoxOffice - AgentPreference flag always takes priority over system defaults value 
        If AgentProfile.IsAgent Then
            chkCorporateProducts.Checked = AgentProfile.CorporateHospitalityMode
        End If
    End Sub

    ''' <summary>
    ''' Bind the repeater based on paging information
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bindRepeater()
        Dim pagedPurchaseHistory As New PagedDataSource
        Dim ticketingDataSet As DataSet = Nothing
        Dim totalRecordNumber As Integer = 0
        Dim displayPager As Boolean = True
        Dim currentPage As Integer = 0

        ' Retail Orders are now stored in the ticketing database so we can return the details with WS005R.  This will allow us
        ' to only call WS005R to load one page of data at a time.  Currently merging these two tables is causing slow response times 
        ' becuase it requires many calls.  I have added a new attribute to decide whether to do this because we may need this for customers
        ' with no ticketing database.
        Dim retailDataTable As DataTable
        If TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_wfr.Attribute("RetailFromTicketingDB")) Then
            retailDataTable = New DataTable
        Else
            retailDataTable = getRetailDataTable()
        End If
        If retailDataTable IsNot Nothing AndAlso retailDataTable.Rows.Count > 0 Then _customerHasRetailPurchaseHistory = True

        ' get the data 
        ticketingDataSet = getTicketingDataSet()
        If ticketingDataSet IsNot Nothing AndAlso ticketingDataSet.Tables.Count > 0 AndAlso ticketingDataSet.Tables("OrderEnquiryDetails").Rows.Count > 0 Then
            totalRecordNumber = ticketingDataSet.Tables("StatusResults").Rows(0).Item("TotalRecordNumber")
            IsLastPage = ticketingDataSet.Tables("StatusResults").Rows(0)("IsLastPage")
            hdnLastRRN.Value = ticketingDataSet.Tables("StatusResults").Rows(0)("LastRRN")
            hdnRRNOfFirstRecordOnPage.Value = ticketingDataSet.Tables("StatusResults").Rows(0)("RRNOfFirstRecordOnPage")
            currentPage = ticketingDataSet.Tables("StatusResults").Rows(0)("CurrentPage")
            _customerHasTicketingPurchaseHistory = True
        Else
            If ticketingDataSet.Tables.Count = 0 Then
                ticketingDataSet.Tables.Add("OrderEnquiryDetails")
            End If
        End If

        hdnCurrentPage.Value = currentPage
        Try
            pagedPurchaseHistory.DataSource = prepareDataTableToBind(ticketingDataSet.Tables("OrderEnquiryDetails"), retailDataTable)
            If pagedPurchaseHistory.Count > 0 Then
                setColumnHeadings()
                rptOrderHeaderHistory.DataSource = pagedPurchaseHistory
                rptOrderHeaderHistory.DataBind()
                plhNoPurchasesFound.Visible = False
            Else
                plhNoPurchasesFound.Visible = True
                rptOrderHeaderHistory.Visible = False
                ltlNoPurchaseFound.Text = _wfr.Content("NoPurchaseFound", _languageCode, True)
                displayPager = False
            End If
        Catch ex As Exception
            'Binding error has occurred
            Dim s As String = ex.Message
        End Try

        setupPager(IsLastPage, currentPage, displayPager)
        setupDatePicker()
    End Sub

    ''' <summary>
    ''' Setup the pager displayed at the top and bottom of the page
    ''' </summary>
    ''' <param name="IsLastPage">Boolean is this the last page</param>
    ''' <param name="currentPage">The current integer value</param>
    ''' <param name="displayPager">Boolean to display the pager</param>
    ''' <remarks></remarks>
    Private Sub setupPager(ByVal IsLastPage As Boolean, ByVal currentPage As Integer, ByVal displayPager As Boolean)
        If displayPager AndAlso currentPage > 0 AndAlso IIf(currentPage = 1 AndAlso IsLastPage, False, True) Then
            plhFirstPageTop.Visible = (currentPage > 1)
            plhFirstPageBottom.Visible = (currentPage > 1)
            plhPrevPageTop.Visible = (currentPage > 1)
            plhPrevPageBottom.Visible = (currentPage > 1)
            plhNextPageTop.Visible = Not IsLastPage
            plhNextPageBottom.Visible = Not IsLastPage
            plhLastPageTop.Visible = Not IsLastPage
            plhLastPageBottom.Visible = Not IsLastPage
            ltlCurrentPageTop.Text = currentPage
            ltlCurrentPageBottom.Text = currentPage
            plhPagerTop.Visible = True
            plhPagerBottom.Visible = True
            setPagerText()
        Else
            plhPagerTop.Visible = False
            plhPagerBottom.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Set the text for the pager
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setPagerText()
        lnkFirstTop.Text = _wfr.Content("LnkFirstT", _languageCode, True)
        lnkFirstBottom.Text = _wfr.Content("LnkFirstB", _languageCode, True)
        lnkPreviousTop.Text = _wfr.Content("LnkPrevT", _languageCode, True)
        lnkPreviousBottom.Text = _wfr.Content("LnkPrevB", _languageCode, True)
        lnkNextTop.Text = _wfr.Content("LnkNextT", _languageCode, True)
        lnkNextBottom.Text = _wfr.Content("LnkNextB", _languageCode, True)
        lnkLastTop.Text = _wfr.Content("LnkLastT", _languageCode, True)
        lnkLastBottom.Text = _wfr.Content("LnkLastB", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Set the text for the column headings
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setColumnHeadings()
        ProductColumnHeading = _wfr.Content("membershipMatchLabel", _languageCode, True)
        PaymentRefColumnHeading = _wfr.Content("payRefLabel", _languageCode, True)
        QuantityColumnHeading = _wfr.Content("quantityLabel", _languageCode, True)
        DateColumnHeading = _wfr.Content("dateLabel", _languageCode, True)
        CustomerColumnHeading = _wfr.Content("customerNumberLabel", _languageCode, True)
        PriceColumnHeading = _wfr.Content("priceLabel", _languageCode, True)
        StatusColumnHeading = _wfr.Content("statusLabel", _languageCode, True)
        RetailPurchaseRefColumnHeading = _wfr.Content("RetailCustomerPurchaseRefColumnHeading", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Set and register the date picker script onto the page based on the global date format
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setupDatePicker()
        Dim jscript As New StringBuilder
        jscript.Append("$(document).ready(function () { $("".datepicker"").datepicker({ dateFormat: 'dd/mm/yy' }); });")
        ClientScript.RegisterClientScriptBlock(Page.GetType(), "DatePicker", jscript.ToString(), True)
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Get the ticketing puchase history data
    ''' </summary>
    ''' <returns>Results of ticketing information</returns>
    ''' <remarks></remarks>
    Private Function getTicketingDataSet() As DataSet
        Dim order As New TalentOrder
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim err As New ErrorObj
        settings.AccountNo1 = TCUtilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)
        If txtPackageID.Text.Trim.Length > 0 AndAlso IsNumeric(txtPackageID.Text.Trim) Then
            order.Dep.CallId = txtPackageID.Text.Trim
        End If
        If revPaymentReference.IsValid Then
            order.Dep.PaymentReference = txtPaymentReference.Text.Trim()
        Else
            order.Dep.PaymentReference = 0
        End If
        order.Dep.CustNumberPayRefShouldMatch = False
        settings.Cacheing = False
        If Talent.eCommerce.Utilities.IsAgent Then
            settings.OriginatingSource = AgentProfile.Name
        Else
            If TEBUtilities.CheckForDBNull_Int(order.Dep.PaymentReference) <= 0 Then
                settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_wfr.Attribute("CacheTimeMinutes"))
                settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
                settings.AuthorityUserProfile = ModuleDefaults.AuthorityUserProfile
            End If
        End If
        order.Dep.ProductOrDescription = txtProductOrDescription.Text.Trim()
        order.Dep.ShowRetailItems = chkRetailProducts.Checked
        order.Dep.ShowTicketingItems = chkTicketingProducts.Checked
        order.Dep.FromDate = getFormattedDateString(txtFromDate.Text.Trim)
        order.Dep.ToDate = getFormattedDateString(txtToDate.Text.Trim)
        order.Dep.OrderStatus = ddlStatus.SelectedValue
        order.Dep.CorporateProductsOnly = chkCorporateProducts.Checked
        order.Dep.LastRecordNumber = 0
        order.Dep.IncludeBuybackSales = chkShowBuybackProducts.Checked
        order.Dep.IncludeRoyaltyInformation = chkShowLoyaltyInformation.Checked
        order.Dep.IncludeReservations = chkShowReservations.Checked
        order.Dep.IncludeBooked = chkShowBooked.Checked
        order.Dep.CurrentPage = hdnCurrentPage.Value

        If Direction = "P" Then
            order.Dep.RequestPreviousPage = True
        ElseIf Direction = "L" Then
            order.Dep.RequestLastPage = True
        ElseIf Direction = "F" Then
            order.Dep.RequestFirstPage = True
        End If

        If hdnRRNOfFirstRecordOnPage.Value <> "" Then
            order.Dep.FirstRecordOnPageRelativeRecordNumber = CInt(hdnRRNOfFirstRecordOnPage.Value)
        End If
        If hdnLastRRN.Value <> "" Then
            order.Dep.LastRelativeRecordNumber = CInt(hdnLastRRN.Value)
        End If

        order.Dep.TotalRecords = 0
        order.Settings() = settings
        err = order.OrderEnquiryDetails()
        If Not err.HasError AndAlso order.ResultDataSet IsNot Nothing AndAlso order.ResultDataSet.Tables.Count = 9 AndAlso order.ResultDataSet.Tables("StatusResults").Rows.Count > 0 Then
            'ErrorOccurred
            'is it package detail filter
            If TEBUtilities.CheckForDBNull_String(order.ResultDataSet.Tables("StatusResults").Rows(0)("ErrorOccurred")).Trim.Length = 0 Then
                If txtPackageID.Text.Trim.Length > 0 Then
                    If order.ResultDataSet.Tables("PackageDetail") IsNot Nothing AndAlso order.ResultDataSet.Tables("PackageDetail").Rows.Count > 0 Then
                        Response.Redirect("~/PagesLogin/Orders/PurchaseDetails.aspx?payref=" & txtPaymentReference.Text.Trim & "&callid=" & txtPackageID.Text.Trim)
                    End If
                End If
            End If
            _dsPurchaseHistory = order.ResultDataSet
            Return order.ResultDataSet
        Else
            Return New DataSet
        End If
    End Function

    ''' <summary>
    ''' Get the retail purchase history header data
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getRetailDataTable() As DataTable
        Dim status As String = ddlStatus.SelectedValue
        If status = "ALL" Then status = String.Empty
        Dim b2bMode As Boolean = TDataObjects.AppVariableSettings.TblAuthorizedPartners.CheckB2BMode(BusinessUnit)
        Dim isShowPartnerDetails As Boolean = ModuleDefaults.ORDER_ENQUIRY_SHOW_PARTNER_ORDERS And (Not Profile.IsAnonymous AndAlso Boolean.Parse(Profile.PartnerInfo.Details.Order_Enquiry_Show_Partner_Orders))
        Dim retailDataTable As DataTable = Nothing
        retailDataTable = TDataObjects.OrderSettings.TblOrderHeader.GetRetailPurchaseHistory(BusinessUnit, TalentCache.GetBusinessUnitGroup(), Profile.UserName, TalentCache.GetPartner(Profile),
                                                    b2bMode, isShowPartnerDetails, txtPaymentReference.Text.Trim(), txtFromDate.Text.Trim(), txtToDate.Text.Trim(), status, False)
        Return retailDataTable
    End Function

    ''' <summary>
    ''' Merge the retail and ticketing data into a table that can be used by the repeater and order the data in descending date order
    ''' Retail data is now part of the ticketing data, but the table must still be formatted correctly.
    ''' </summary>
    ''' <param name="ticketingData">The ticketing data</param>
    ''' <param name="retailData">The retail data</param>
    ''' <returns>Data table ordered by most recent date</returns>
    ''' <remarks></remarks>
    Private Function prepareDataTableToBind(ByRef ticketingData As DataTable, ByRef retailData As DataTable) As DataView
        Dim dataTableToBind As New DataTable
        Dim mergedRow As DataRow
        With dataTableToBind.Columns
            .Add("RETAIL_WEB_ORDER_NUMBER", GetType(String))
            .Add("RETAIL_BACK_OFFICE_ORDER_NUMBER", GetType(String))
            .Add("RETAIL_PURCHASE_ORDER", GetType(String))
            .Add("PAYMENT_REF", GetType(String))
            .Add("SALE_DATE", GetType(Date))
            .Add("CUSTOMER_NUMBER", GetType(String))
            .Add("CUSTOMER_NAME", GetType(String))
            .Add("TICKETING_PRODUCT", GetType(String))
            .Add("TICKETING_PRODUCT_TYPE", GetType(String))
            .Add("PRICE", GetType(String))
            .Add("STATUS", GetType(String))
            .Add("IS_TICKETING_OR_RETAIL", GetType(String))
            .Add("RELATING_BUNDLE_PRODUCT", GetType(String))
            .Add("TICKETING_CALL_ID", GetType(Long))
            .Add("PAYMENT_OWNER", GetType(Boolean))
            .Add("BULK_QTY", GetType(Integer))
            .Add("BULK_ID", GetType(Integer))
        End With

        If ticketingData IsNot Nothing Then
            For Each row As DataRow In ticketingData.Rows
                If ddlProductType.SelectedValue = "ALL" OrElse ddlProductType.SelectedValue = row("ProductType").ToString().Trim() Then
                    mergedRow = dataTableToBind.NewRow()
                    mergedRow("RETAIL_WEB_ORDER_NUMBER") = String.Empty
                    mergedRow("RETAIL_BACK_OFFICE_ORDER_NUMBER") = String.Empty
                    mergedRow("RETAIL_PURCHASE_ORDER") = String.Empty
                    mergedRow("PAYMENT_REF") = row("PaymentReference").ToString().Trim().TrimStart(GlobalConstants.LEADING_ZEROS)
                    mergedRow("SALE_DATE") = DateTime.ParseExact(row("SaleDate").ToString().Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture)
                    mergedRow("CUSTOMER_NUMBER") = row("CustomerNumber").ToString().Trim().TrimStart(GlobalConstants.LEADING_ZEROS)
                    mergedRow("CUSTOMER_NAME") = row("CustomerName").ToString().Trim()

                    'We need to override the description when the product is a retail product
                    If ModuleDefaults.RetailProductCode = row("ProductCode").ToString() Then
                        mergedRow("TICKETING_PRODUCT") = _wfr.Content("RetailOrderItemText", _languageCode, True)
                    Else
                        mergedRow("TICKETING_PRODUCT") = row("ProductDescription").ToString().Trim()
                    End If

                    mergedRow("TICKETING_PRODUCT_TYPE") = row("ProductType").ToString().Trim()
                    mergedRow("PRICE") = FormatCurrency(CDec(row("SalePrice").ToString().Trim()))
                    mergedRow("STATUS") = row("StatusCode").ToString().Trim()
                    mergedRow("IS_TICKETING_OR_RETAIL") = GlobalConstants.TICKETINGTYPE
                    mergedRow("RELATING_BUNDLE_PRODUCT") = row("RelatingBundleProduct").ToString().Trim()
                    mergedRow("TICKETING_CALL_ID") = Utilities.CheckForDBNull_BigInt(row("CallId").ToString().Trim())
                    mergedRow("PAYMENT_OWNER") = row("PaymentOwner")
                    mergedRow("BULK_QTY") = row("BulkQty")
                    mergedRow("BULK_ID") = row("BulkID")
                    dataTableToBind.Rows.Add(mergedRow)
                End If
            Next
        End If
        If retailData IsNot Nothing AndAlso chkRetailProducts.Checked Then
            For Each row As DataRow In retailData.Rows
                If ddlProductType.SelectedValue = "ALL" Then
                    mergedRow = dataTableToBind.NewRow()
                    mergedRow("RETAIL_WEB_ORDER_NUMBER") = TEBUtilities.CheckForDBNull_String(row("PROCESSED_ORDER_ID").ToString().Trim())
                    mergedRow("RETAIL_BACK_OFFICE_ORDER_NUMBER") = TEBUtilities.CheckForDBNull_String(row("BACK_OFFICE_ORDER_ID").ToString().Trim())
                    mergedRow("RETAIL_PURCHASE_ORDER") = TEBUtilities.CheckForDBNull_String(row("PURCHASE_ORDER").ToString().Trim())
                    mergedRow("PAYMENT_REF") = String.Empty
                    mergedRow("SALE_DATE") = CDate(TEBUtilities.CheckForDBNull_String(row("CREATED_DATE")))
                    mergedRow("CUSTOMER_NUMBER") = TEBUtilities.CheckForDBNull_String(row("LOGINID").ToString().Trim().TrimStart(GlobalConstants.LEADING_ZEROS))
                    mergedRow("CUSTOMER_NAME") = TEBUtilities.CheckForDBNull_String(row("CONTACT_NAME").ToString().Trim())
                    mergedRow("TICKETING_PRODUCT") = String.Empty
                    mergedRow("TICKETING_PRODUCT_TYPE") = String.Empty
                    mergedRow("PRICE") = TDataObjects.PaymentSettings.FormatCurrency(TEBUtilities.RoundToValue(TEBUtilities.CheckForDBNull_Decimal(row("TOTAL_ORDER_VALUE")), 0.01, False),
                                                                     TEBUtilities.CheckForDBNull_String(row("BUSINESS_UNIT")), TalentCache.GetPartner(Profile))
                    mergedRow("STATUS") = TEBUtilities.CheckForDBNull_String(row("STATUS").ToString().Trim())
                    mergedRow("IS_TICKETING_OR_RETAIL") = GlobalConstants.RETAILTYPE
                    mergedRow("RELATING_BUNDLE_PRODUCT") = String.Empty
                    mergedRow("TICKETING_CALL_ID") = 0
                    mergedRow("PAYMENT_OWNER") = True
                    mergedRow("BULK_QTY") = TEBUtilities.CheckForDBNull_Int(row("LINES"))
                    mergedRow("BULK_ID") = 0
                    dataTableToBind.Rows.Add(mergedRow)
                End If
            Next
        End If
        Dim dvMergedData As New DataView(dataTableToBind)
        dvMergedData.Sort = "SALE_DATE DESC, PAYMENT_REF DESC"
        Return dvMergedData
    End Function

    ''' <summary>
    ''' Get the promotion details
    ''' </summary>
    ''' <returns>data set of promotion details</returns>
    ''' <remarks></remarks>
    Private Function getPromotionDetails() As DataSet
        Dim dsPromotionDetails As New DataSet
        Dim promos As New TalentPromotions
        Dim promoSettings As New DEPromotionSettings
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim err As New ErrorObj

        promoSettings.FrontEndConnectionString = settings.FrontEndConnectionString
        promoSettings.BusinessUnit = settings.BusinessUnit
        promoSettings.StoredProcedureGroup = settings.StoredProcedureGroup
        If Profile.IsAnonymous Then
            promoSettings.AccountNo1 = TCUtilities.PadLeadingZeros(GlobalConstants.GENERIC_CUSTOMER_NUMBER, 12)
        Else
            promoSettings.AccountNo1 = TCUtilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)
        End If
        promoSettings.Cacheing = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_wfr.Attribute("Caching"))
        promoSettings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_wfr.Attribute("CacheTimeInMins"))
        promoSettings.CacheDependencyPath = settings.CacheDependencyPath
        promoSettings.OriginatingSource = settings.OriginatingSource
        promos.Settings() = promoSettings
        promos.Dep = New DEPromotions

        err = promos.GetPromotionDetails
        dsPromotionDetails = promos.ResultDataSet()
        Return dsPromotionDetails
    End Function

    ''' <summary>
    ''' Format the DD/MM/YYYY to an iseries date format.
    ''' </summary>
    ''' <param name="dateString">The proper date format</param>
    ''' <returns>The iSeries date format</returns>
    ''' <remarks></remarks>
    Private Function getFormattedDateString(ByVal dateString As String) As String
        Dim formattedDateString As String = String.Empty
        If dateString.Length >= 10 Then
            formattedDateString = dateString.Substring(0, 2) & dateString.Substring(3, 2) & dateString.Substring(6, 4)
        End If
        Return formattedDateString
    End Function

#End Region

#Region "Public Function"

    ''' <summary>
    ''' Get the soft-coded status text from the database
    ''' </summary>
    ''' <param name="PValue">The hardcoded text value</param>
    ''' <returns>The soft coded value</returns>
    ''' <remarks></remarks>
    Protected Function GetStatusText(ByVal PValue As String, ByVal ticketingOrRetail As String) As String
        Dim str As String = String.Empty
        If ticketingOrRetail = GlobalConstants.TICKETINGPRODUCTTYPE Then
            str = _wfr.Content(PValue, _languageCode, True)
            If String.IsNullOrEmpty(str) Then
                str = PValue
            End If
        ElseIf ticketingOrRetail = GlobalConstants.RETAILTYPE Then
            Dim rowsFound As Boolean = True
            Dim descAdapter As New TalentDescriptionsDataSetTableAdapters.tbl_ebusiness_descriptions_buTableAdapter
            Dim descs As TalentDescriptionsDataSet.tbl_ebusiness_descriptions_buDataTable = descAdapter.GetDataByBUetcLang(RETAIL_DESCRIPTIONS_QUALIFIER, _wfr.BusinessUnit, TalentCache.GetPartner(Profile), RETAIL_DESCRIPTIONS_TYPE, _languageCode)
            If descs.Rows.Count = 0 Then
                descs = descAdapter.GetDataByBUetcLang(RETAIL_DESCRIPTIONS_QUALIFIER, _wfr.BusinessUnit, TCUtilities.GetAllString, RETAIL_DESCRIPTIONS_TYPE, _languageCode)
                If descs.Rows.Count = 0 Then
                    rowsFound = False
                End If
            End If
            If rowsFound Then
                For Each row As DataRow In descs.Rows
                    If TEBUtilities.CheckForDBNull_String(row("DESCRIPTION_CODE")) = PValue Then
                        str = row("DESCRIPTION_DESCRIPTION")
                        Exit For
                    End If
                Next
            End If
        End If
        Return str
    End Function

#End Region

End Class