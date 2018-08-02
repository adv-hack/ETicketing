Imports Talent.Common
Imports Talent.eCommerce.CATHelper
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Data
Imports System.Collections.Generic

Partial Class PagesLogin_Orders_PurchaseDetails
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfr As WebFormResource = Nothing
    Private _languageCode As String = Nothing
    Private _showPaymentDetailsColumn As Boolean = False
    Private _isTransactionEnquiry As Boolean = True
    Private _isDDTransaction As Boolean = False
    Private _isTicketCollection As Boolean = False
    Private _allowCancel As Boolean = False
    Private _allowTransfer As Boolean = False
    Private _allowAmend As Boolean = False
    Private _allowPrint As Boolean = False
    Private _allowSeatHist As Boolean = False
    Private _dtPurchaseHistory As DataTable = Nothing
    Private _dsProductsInABundle As DataSet = Nothing
    Private _dsTicketingPurchase As DataSet = Nothing
    Private _tempStandDesc As String = String.Empty
    Private _packageStatusCode As String = String.Empty
    Private _packageType As String = String.Empty
    Private _isPayOwnerLoggedIn As Boolean = False
    Private _loggedInCustomerNumber As String = String.Empty
    Private _fullCancelOnly As Boolean = False
    Private _isABulkSalePaymentRef As Boolean = False
    Private _isNull As Boolean = True
    Private _dtRetailHistory As DataTable = Nothing
    Private _showDespatchInformation As Boolean = False
    Private _component As ComponentType
    Private _callId As String = String.Empty
    Private _bulkID As String = String.Empty
    Private _bulkCall As Boolean = False
    Private _packageKey As String = String.Empty
    Private _payRefHasCancelledOrder As Boolean = False
    Private _showComponentSeatPrices As Boolean
    Private _showComponentNetPrice As Boolean
    Private _showComponentDiscount As Boolean
    Private _showComponentTotalPrice As Boolean
    Private _selectedABulkItemForAction As Boolean
    Private _selectedACorporateItemForAction As Boolean
    Private _CATMODE_CANCELMULTIPLE_SESSION_KEY As String = "CATMODE_CANCELMULTIPLE" & TalentCache.GetBusinessUnit
    Private _SUCCESSMESSAGE As String = "PurchDet_successMessage"
    Private _ERRORMESSAGE As String = "PurchDet_errorMessage"
    Private _productCode As String = String.Empty
    Private _isCorporateLinkedHomeGame As String = String.Empty

    ' 7 tables from order enquiry StatusResults OrderEnquiryDetails PackageDetail PackageHistory ComponentSummary PaymentOwnerDetails PaymentDetails
    Private _directDebitPaymentsMadeValue As Decimal = 0
    Private _DIRECT_DEBIT_PAYMENTS_MADE_SESSION_KEY As String = String.Empty

    Structure actionErrorList
        'WS005R error/reason codes:
        'NH - Transfers and amendments are only available for home games.
        'UV - This option is not available because voucher has been used.
        'SP - This option is not available as the product is suspended.
        'SI - Cancellations, transfers, and amendments are not available for items at this seat status.
        'PP - Cancellations, transfers, and amendments are not available for direct debit sales with pending payments.
        'PK - Cancellations, transfers, and amendments are not available for a package or corporate sales.
        'PA - Cancellations, transfers, and amendments are not available where a promotion has been applied.
        'FE - This option is not available for a fee.
        'EV - This option is not available because the voucher has expired.
        'EP - This option is not available because this is an e-purse top up product.
        'DT - This option is not available because the product date is in the past.
        'DS - Cancellations, transfers, and amendments are not available for carer or ambulant seating.
        'DF - Cancellations, transfers, and amendments are not available for direct debit refund products.
        'CS - Cancellations, transfers, and amendments are not available for cancelled seats.
        'CF - Cancellations, transfers, and amendments are not available for sales made via Credit Finance.

        'Additional error/reason codes
        Public Const NoCancelUnlessSeatOwner As String = "C1"
        Public Const FullCancelOnly As String = "C2"
        Public Const NoAmendUnlessSeatOwner As String = "A1"
        Public Const NoTransUnlessSeatOwner As String = "T1"
        Public Const NoPrintForItem As String = "P1"
        Public Const NoAmendForBulkHeader As String = "AB"
        Public Const NoTransferForBulkHeader As String = "TB"
        Public Const NoSeatHistForBulkHeader As String = "HB"
        Public Const NoSeatPHistForBulkHeader As String = "PB"
        Public Const NoMultipleSelectionsAllowed As String = "NM"
        Public Const NoMultipleSelectionsBulkHeaderAllowed As String = "NB"
        Public Const NoMultipleSelectionsCorpHeaderAllowed As String = "PM"
        Public Const NoSelectionsMade As String = "XX"
        Public Const NoAmendForBulkAgent As String = "BA"
        Public Const NoTransForBulkAgent As String = "TA"
        Public Const NoAmendForBulkSales As String = "AS"
        Public Const NoTransForBulkSales As String = "TS"
        Public Const NoActionForPackageHeader As String = "PH"
        Public Const TooManySelectedItems As String = "TM"
        Public Const ItemCancelled As String = "IC"
        Public Const AgentCannotGiveDDRefund As String = "AR"
        Public Const AGENTAUTHORITYFORCANCEL As String = "AC"
        Public Const AGENTAUTHORITYFORAMEND As String = "AA"
        Public Const AGENTAUTHORITYFORTRANSFER As String = "AT"
        Public Const AgentAurthorityForCancelPastTickets As String = "AP"
    End Structure

    Private Enum ComponentType
        None = 0 'Not a component type or not applicable
        PACKAGE = 1 'Goodwood package
        BULK = 2 'Silverstone package
        CORPORATE = 3 'Corporate package
    End Enum

    Private Enum ButtonOption
        Print = 0
        Cancel = 1
        Amend = 2
        Transfer = 3
        SeatHist = 4
        SeatPrintHist = 5
        Email = 5
    End Enum

#End Region

#Region "Public Properties"

    Public PaymentReference As String
    Public PaymentTypeColumnHeading As String
    Public AmountPaidColumnHeading As String
    Public DetailsColumnHeading As String
    Public EncryptedCardNumberPrefixString As String
    Public ProductColumnHeading As String
    Public StandColumnHeading As String
    Public SeatColumnHeading As String
    Public DateColumnHeading As String
    Public CustomerColumnHeading As String
    Public FulfilmentMethodColumnHeading As String
    Public PriceColumnHeading As String
    Public PriceBandColumnHeading As String
    Public LoyaltyPointsColumnHeading As String
    Public StatusColumnHeading As String
    Public RetailProductColumnHeading As String
    Public RetailDescriptionColumnHeading As String
    Public QuantityHeading As String
    Public ComponentHeading As String
    Public ComponentNetPriceHeading As String
    Public ComponentDiscountHeading As String
    Public PriceHeading As String
    Public DetailHeading As String
    Public PackageHistoryDateColHead As String
    Public PackageHistoryDescColHead As String
    Public DetailButtonText As String
    Public TicketNumberHeading As String
    Public DespatchDateHeading As String
    Public DespatchInformationHeading As String
    Public ItemSelectHeading As String
    Public HideSeatForPWS As Boolean

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessPurchaseHistory) Or Not AgentProfile.IsAgent Then
            _wfr = New WebFormResource
            _languageCode = TCUtilities.GetDefaultLanguage()
            With _wfr
                .BusinessUnit = TalentCache.GetBusinessUnit
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                .PageCode = "PurchaseDetails.aspx"
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "PurchaseDetails.aspx"
            End With
            HttpContext.Current.Session.Remove(_CATMODE_CANCELMULTIPLE_SESSION_KEY)
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "purchase-details.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("purchase-details.js", "/Module/Orders/"), False)
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "table-functions.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("table-functions.js", "/Application/Elements/"), False)
            _showDespatchInformation = Utilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("ShowDespatchInformation"))

            If Request.QueryString("payref") Is Nothing Then
                If AgentProfile.IsAgent Then
                    Response.Redirect("TransactionHistory.aspx")
                Else
                    Response.Redirect("PurchaseHistory.aspx")
                End If
            Else
                PaymentReference = Request.QueryString("payref")
            End If
            If Request.QueryString("CallId") Is Nothing OrElse String.IsNullOrEmpty(Request.QueryString("CallId")) Then
                _callId = "0"
            Else
                _callId = Request.QueryString("CallId")
            End If
            If Request.QueryString("PackageKey") IsNot Nothing OrElse Not String.IsNullOrEmpty(Request.QueryString("PackageKey")) Then
                _packageKey = Request.QueryString("PackageKey")
            End If
            If Request.QueryString("BulkId") Is Nothing OrElse String.IsNullOrEmpty(Request.QueryString("BulkId")) Then
                _bulkID = "0"
            Else
                _bulkID = Request.QueryString("BulkId")
            End If
            If Request.QueryString("BulkCall") Is Nothing OrElse String.IsNullOrEmpty(Request.QueryString("BulkCall")) OrElse Request.QueryString("BulkCall") <> "Y" Then
                _bulkCall = False
            Else
                _bulkCall = True
            End If
            If Not String.IsNullOrEmpty(Request.QueryString("CallId")) Then
                If Not String.IsNullOrEmpty(Request.QueryString("productcode")) Then
                    _productCode = Request.QueryString("productcode")
                    _isCorporateLinkedHomeGame = "Y"
                Else
                    _isCorporateLinkedHomeGame = "N"
                End If
            End If
            If HttpContext.Current.Session.Item(_SUCCESSMESSAGE) IsNot Nothing AndAlso HttpContext.Current.Session.Item(_SUCCESSMESSAGE) IsNot String.Empty Then
                ltlSuccessMessage.Text = HttpContext.Current.Session.Item(_SUCCESSMESSAGE)
                plhSuccessMessage.Visible = True
                HttpContext.Current.Session.Remove(_SUCCESSMESSAGE)
            End If
            If HttpContext.Current.Session.Item(_ERRORMESSAGE) IsNot Nothing AndAlso HttpContext.Current.Session.Item(_ERRORMESSAGE) IsNot String.Empty Then
                ltlError.Text = HttpContext.Current.Session.Item(_ERRORMESSAGE)
                plhErrorMessage.Visible = True
                HttpContext.Current.Session.Remove(_ERRORMESSAGE)
            End If
            _component = GetComponentType()
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim noRecordFound As Boolean = False
        Dim ticketingDataSet As DataSet = getTicketingDataSet()
        If ticketingDataSet Is Nothing Then
            noRecordFound = True
        End If

        If Not noRecordFound Then
            Dim dtOrderEnquiryDetails As DataTable = ticketingDataSet.Tables("OrderEnquiryDetails")
            _payRefHasCancelledOrder = HasCancelledOrder(dtOrderEnquiryDetails)

            Dim uscBreadCrumbTrail As UserControl = CType(Master.FindControl("uscBreadCrumbTrail"), UserControl)
            If uscBreadCrumbTrail IsNot Nothing AndAlso AgentProfile.IsAgent Then
                uscBreadCrumbTrail.Visible = False
            End If
            If ticketingDataSet IsNot Nothing Then
                _dsTicketingPurchase = ticketingDataSet
                If ticketingDataSet.Tables("StatusResults") IsNot Nothing AndAlso ticketingDataSet.Tables("StatusResults").Rows.Count > 0 Then
                    PaymentReference = TEBUtilities.CheckForDBNull_String(ticketingDataSet.Tables("StatusResults").Rows(0)("PaymentReference"))
                    If ticketingDataSet.Tables("PaymentDetails").Rows.Count > 0 AndAlso TEBUtilities.CheckForDBNull_String(ticketingDataSet.Tables("PaymentDetails").Rows(0)("PayType")) = GlobalConstants.DDPAYMENTTYPE Then
                        _directDebitPaymentsMadeValue = TEBUtilities.CheckForDBNull_Decimal(ticketingDataSet.Tables("StatusResults").Rows(0)("DDAmountAlreadyPaid"))
                        _DIRECT_DEBIT_PAYMENTS_MADE_SESSION_KEY = "_DIRECT_DEBIT_PAYMENTS_MADE_SESSION_KEY" & PaymentReference
                        Session.Add(_DIRECT_DEBIT_PAYMENTS_MADE_SESSION_KEY, _directDebitPaymentsMadeValue)
                    End If
                End If

                Dim dtHistoryToProcess As DataTable = Nothing
                Dim dtParentProduct As DataTable = Nothing
                If _component = ComponentType.CORPORATE Then
                    _dtPurchaseHistory = ticketingDataSet.Tables("PackageDetail")
                Else
                    _dtPurchaseHistory = GetComponentsOnly(ticketingDataSet.Tables("OrderEnquiryDetails"))
                End If
                dtParentProduct = GetParentOnly(ticketingDataSet.Tables("OrderEnquiryDetails"))
                If _dtPurchaseHistory IsNot Nothing AndAlso _dtPurchaseHistory.Rows.Count > 0 Then
                    Dim dvFeesDataTable As New DataView()
                    Try
                        AssignPaymentOwnerLoggedInFlag(ticketingDataSet.Tables("PaymentOwnerDetails"))
                        AssignCATTicketsFlag()
                        AssignPrintTicketsFlag()
                        AssignSeatHistoryTicketsFlag()
                        ShowPackageOrPurchaseDetail()
                        setPaymentOwnerData(ticketingDataSet.Tables("PaymentOwnerDetails"))
                        PopulateParentDetails(ticketingDataSet.Tables("StatusResults"), dtParentProduct)
                        If _component <> ComponentType.None Then
                            btnBack.Text = String.Format(_wfr.Content("btnBackWithReference", _languageCode, True), PaymentReference.TrimStart("0"))
                        Else
                            btnBack.Text = _wfr.Content("btnBack", _languageCode, True)
                        End If

                        PaymentTypeColumnHeading = _wfr.Content("PayTypeLabel", _languageCode, True)
                        AmountPaidColumnHeading = _wfr.Content("PayAmountLabel", _languageCode, True)
                        DetailsColumnHeading = _wfr.Content("PaymentDetailsTitle", _languageCode, True)
                        _dtRetailHistory = RetrieveRetailData()

                        If _isPayOwnerLoggedIn Or Not String.IsNullOrWhiteSpace(AgentProfile.Name) Then
                            ltlPaymentDetailsTitle.Text = _wfr.Content("PaymentDetailsTitle", _languageCode, True)
                            If ticketingDataSet.Tables("PaymentDetails").Rows.Count <= 1 Then
                                rptPaymentDetails.DataSource = ticketingDataSet.Tables("PaymentDetails")
                                rptPaymentDetails.DataBind()
                            Else
                                rptMultiPaymentdetails.DataSource = ticketingDataSet.Tables("PaymentDetails")
                                rptMultiPaymentdetails.DataBind()
                            End If
                        End If
                        setColumnHeadings()
                        Dim ticketingData As DataView = Nothing
                        If _component = ComponentType.CORPORATE Then
                            ticketingData = removeFees(ticketingDataSet.Tables("PackageDetail"), dvFeesDataTable)
                        Else
                            ticketingData = removeFees(_dtPurchaseHistory, dvFeesDataTable)
                        End If
                        If plhOrderDetailsRepeater.Visible Then
                            _fullCancelOnly = ticketingDataSet.Tables("StatusResults").Rows(0)("FullCancelOnly")
                            ltlTicketingOrdersTitle.Text = _wfr.Content("TicketingOrdersTitleLabel", _languageCode, True)
                            Dim dv As DataView = removeBundleProducts(ticketingData.ToTable)
                            dv.RowFilter += " And ProductCode <> '" & ModuleDefaults.RetailProductCode & "'"
                            IsLoyaltyPointEmpty(dv)
                            If dv.Count > 0 AndAlso dv.Table.Rows(0)("BulkID") > 0 Then
                                _isABulkSalePaymentRef = True
                            End If

                            ' We may have a retail only order so we may not have any products to display
                            If dv.Count = 0 Then
                                plhOrderDetailsRepeater.Visible = False
                            Else
                                dtOrderEnquiryDetails.DefaultView.RowFilter = "ProductDescription<>'' AND ProductCode <> '' "
                                rptOrderDetails.DataSource = dtOrderEnquiryDetails.DefaultView
                                rptOrderDetails.DataBind()
                            End If
                            BindRetailRepeater()
                        Else
                            BindPackageRepeatersAndSetVisibility(ticketingDataSet)
                        End If
                        setTotals(ticketingData, ticketingDataSet.Tables("StatusResults"))
                        rptFees.DataSource = dvFeesDataTable
                        rptFees.DataBind()
                        SetTrackingReferences()

                        If _dsProductsInABundle IsNot Nothing Then
                            rptBundles.DataSource = _dsProductsInABundle.Tables
                            rptBundles.DataBind()
                        End If

                        btnPrintSelectedItem.Text = _wfr.Content("PrintButtonText", _languageCode, True)
                        btnCancelSelectedItem.Text = _wfr.Content("CancelLinkText", _languageCode, True)
                        btnAmendSelectedItem.Text = _wfr.Content("AmendLinkText", _languageCode, True)
                        btnTransferSelectedItem.Text = _wfr.Content("TransferLinkText", _languageCode, True)
                        btnSeatHistorySelectedItem.Text = _wfr.Content("SeatHistoryLabel", _languageCode, True)
                        btnSeatPrintHistorySelectedItem.Text = _wfr.Content("SeatPrintHistoryLabel", _languageCode, True)
                        btnResendEmail.Text = _wfr.Content("ResendEmailButtonText", _languageCode, True)

                        hdfIsBulk.Value = If(Not String.IsNullOrEmpty(Request.QueryString("bulkid")) AndAlso Request.QueryString("bulkid").ToString() <> "0", "true", "false")
                        hdfSingleSelectOnlyErr.Value = getActionErrorDescription(actionErrorList.NoMultipleSelectionsAllowed)
                        hdfSingleSelectOnlyBulkHdrErr.Value = getActionErrorDescription(actionErrorList.NoMultipleSelectionsBulkHeaderAllowed)
                        hdfsingleSelectOnlyCorpHdrErr.Value = getActionErrorDescription(actionErrorList.NoMultipleSelectionsCorpHeaderAllowed)
                        hdfSelectionMandatoryErr.Value = getActionErrorDescription(actionErrorList.NoSelectionsMade)
                        hdfFullCancOnlyErr.Value = getActionErrorDescription(actionErrorList.FullCancelOnly)
                        hdfMaxSelLimitErr.Value = getActionErrorDescription(actionErrorList.TooManySelectedItems)
                        hdfStopCodeCancBtnText.Value = _wfr.Content("StopCodeDiagCancBtnText", _languageCode, True)
                        hdfStopCodeOkBtnText.Value = _wfr.Content("StopCodeDiagOkBtnText", _languageCode, True)
                        hdfStopCodeDiagMessage.Value = _wfr.Content("StopCodeDiagMessageText", _languageCode, True)
                        hdfStopCodeDiagTitle.Value = _wfr.Content("StopCodeDiagTitleText", _languageCode, True)
                        hdfCustomerIsOnStop.Value = (Profile.User.Details.STOP_CODE IsNot Nothing AndAlso Profile.User.Details.STOP_CODE IsNot String.Empty)
                        hdfFullCancOnly.Value = _fullCancelOnly
                    Catch ex As Exception
                        Dim s As String = ex.Message
                    End Try
                Else
                    noRecordFound = True
                End If
            Else
                noRecordFound = True
            End If
        End If
        If noRecordFound Then
            plhPurchaseDetail.Visible = False
            ltlError.Text = _wfr.Content("NoRecordFoundMessage", _languageCode, True)
        End If
    End Sub

    Protected Sub rptMultiPaymentdetails_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptMultiPaymentdetails.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim plhCreditCard As PlaceHolder = CType(e.Item.FindControl("plhCC"), PlaceHolder)
            Dim plhDD As PlaceHolder = CType(e.Item.FindControl("plhDD"), PlaceHolder)
            Dim plhCreditFinance As PlaceHolder = CType(e.Item.FindControl("plhCF"), PlaceHolder)
            Dim plhEP As PlaceHolder = CType(e.Item.FindControl("plhEP"), PlaceHolder)
            plhCreditFinance.Visible = (e.Item.DataItem("PayType") = GlobalConstants.CFPAYMENTTYPE)
            plhCreditCard.Visible = (e.Item.DataItem("PayType") = GlobalConstants.CCPAYMENTTYPE)
            plhDD.Visible = (e.Item.DataItem("PayType") = GlobalConstants.DDPAYMENTTYPE)
            plhEP.Visible = (e.Item.DataItem("PayType") = GlobalConstants.EPURSEPAYMENTTYPE)
            If plhCreditCard.Visible Then
                Dim ltlExpiryDateLabel As Literal = CType(e.Item.FindControl("ltlExpiryDate"), Literal)
                Dim ltlStartDateLabel As Literal = CType(e.Item.FindControl("ltlStartDate"), Literal)
                Dim ltlIssueNumberLabel As Literal = CType(e.Item.FindControl("ltlIssueNumber"), Literal)
                ltlExpiryDateLabel.Text = _wfr.Content("ExpiryDateLabel", _languageCode, True)
                ltlStartDateLabel.Text = _wfr.Content("StartDateLabel", _languageCode, True)
                ltlIssueNumberLabel.Text = _wfr.Content("IssueDetailLabel", _languageCode, True)
                EncryptedCardNumberPrefixString = _wfr.Content("EncryptedCardNumberPrefixString", _languageCode, True)
            End If
            If plhDD.Visible Then
                Dim ltlAccountNameLabel As Literal = CType(e.Item.FindControl("ltlAccountName"), Literal)
                Dim ltlAccountNumberLabel As Literal = CType(e.Item.FindControl("ltlAccountNumber"), Literal)
                Dim ltlSortCodeLabel As Literal = CType(e.Item.FindControl("ltlSortCode"), Literal)
                ltlAccountNameLabel.Text = _wfr.Content("AccountNameLabel", _languageCode, True)
                ltlAccountNumberLabel.Text = _wfr.Content("AccountNumberLabel", _languageCode, True)
                ltlSortCodeLabel.Text = _wfr.Content("SortCodeLabel", _languageCode, True)
            End If

            Dim ltlPayType As Literal = CType(e.Item.FindControl("ltlMultiplePayType"), Literal)
            ltlPayType.Text = TDataObjects.PaymentSettings.TblPaymentTypeLang.GetDescriptionByTypeAndLang(e.Item.DataItem("PayType"), _languageCode)
            If plhCreditFinance.Visible Then
                Dim ltlCreditFinancePlan As Literal = CType(e.Item.FindControl("ltlCreditFinance"), Literal)
                Dim ltlCreditFinanceRequestID As Literal = CType(e.Item.FindControl("ltlCreditFinanceRequest"), Literal)
                ltlCreditFinancePlan.Text = _wfr.Content("ltlCreditFinancePlan", _languageCode, True)
                ltlCreditFinanceRequestID.Text = _wfr.Content("ltlCreditFinanceRequestID", _languageCode, True)
            End If
            If plhEP.Visible Then
                Dim ltlEPCardNumber As Literal = CType(e.Item.FindControl("ltlEPCardNumber"), Literal)
                ltlEPCardNumber.Text = _wfr.Content("ltlEPCardNumber", _languageCode, True)
            End If
        End If
    End Sub

    Protected Sub rptRetailOrders_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptRetailOrders.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            'Dim plhCreditCard As PlaceHolder = CType(e.Item.FindControl("plhCreditCard"), PlaceHolder)
        End If
    End Sub

    Protected Sub rptPaymentDetails_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptPaymentDetails.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim plhCreditCard As PlaceHolder = CType(e.Item.FindControl("plhCreditCard"), PlaceHolder)
            Dim plhDirectDebit As PlaceHolder = CType(e.Item.FindControl("plhDirectDebit"), PlaceHolder)
            Dim plhDetails As PlaceHolder = CType(e.Item.FindControl("plhDetails"), PlaceHolder)
            Dim plhCreditFinance As PlaceHolder = CType(e.Item.FindControl("plhCreditFinance"), PlaceHolder)
            Dim plhEP As PlaceHolder = CType(e.Item.FindControl("plhEPurse"), PlaceHolder)
            Dim plhPayAmount As PlaceHolder = CType(e.Item.FindControl("plhPayAmount"), PlaceHolder)
            Dim ltlPayType As Literal = CType(e.Item.FindControl("ltlPayType"), Literal)
            Dim uscDirectDebitSummary As UserControls_DirectDebitSummary = CType(e.Item.FindControl("uscDirectDebitSummary"), UserControls_DirectDebitSummary)
            Dim hplDirectDebitSummary As HyperLink = CType(e.Item.FindControl("hplDirectDebitSummary"), HyperLink)

            ltlPayType.Text = TDataObjects.PaymentSettings.TblPaymentTypeLang.GetDescriptionByTypeAndLang(e.Item.DataItem("PayType"), _languageCode)
            plhCreditCard.Visible = (e.Item.DataItem("PayType") = GlobalConstants.CCPAYMENTTYPE)
            plhDirectDebit.Visible = (e.Item.DataItem("PayType") = GlobalConstants.DDPAYMENTTYPE)
            _isDDTransaction = plhDirectDebit.Visible
            plhCreditFinance.Visible = (e.Item.DataItem("PayType") = GlobalConstants.CFPAYMENTTYPE)
            plhEP.Visible = (e.Item.DataItem("PayType") = GlobalConstants.EPURSEPAYMENTTYPE)
            plhDetails.Visible = (plhCreditCard.Visible OrElse plhDirectDebit.Visible OrElse plhCreditFinance.Visible)
            'plhPayAmount.Visible = (_component = ComponentType.CORPORATE)
            plhPayAmount.Visible = False
            uscDirectDebitSummary.Visible = False
            hplDirectDebitSummary.Visible = False

            If plhCreditCard.Visible Then
                Dim ltlCreditCardNumberLabel As Literal = CType(e.Item.FindControl("ltlCreditCardNumberLabel"), Literal)
                Dim ltlExpiryDateLabel As Literal = CType(e.Item.FindControl("ltlExpiryDateLabel"), Literal)
                Dim ltlStartDateLabel As Literal = CType(e.Item.FindControl("ltlStartDateLabel"), Literal)
                Dim ltlIssueNumberLabel As Literal = CType(e.Item.FindControl("ltlIssueNumberLabel"), Literal)
                ltlCreditCardNumberLabel.Text = _wfr.Content("CreditCardNumberLabel", _languageCode, True)
                ltlExpiryDateLabel.Text = _wfr.Content("ExpiryDateLabel", _languageCode, True)
                ltlStartDateLabel.Text = _wfr.Content("StartDateLabel", _languageCode, True)
                ltlIssueNumberLabel.Text = _wfr.Content("IssueDetailLabel", _languageCode, True)
                EncryptedCardNumberPrefixString = _wfr.Content("EncryptedCardNumberPrefixString", _languageCode, True)
            End If
            If plhDirectDebit.Visible Then
                Dim ltlAccountNameLabel As Literal = CType(e.Item.FindControl("ltlAccountNameLabel"), Literal)
                Dim ltlAccountNumberLabel As Literal = CType(e.Item.FindControl("ltlAccountNumberLabel"), Literal)
                Dim ltlSortCodeLabel As Literal = CType(e.Item.FindControl("ltlSortCodeLabel"), Literal)
                ltlAccountNameLabel.Text = _wfr.Content("AccountNameLabel", _languageCode, True)
                ltlAccountNumberLabel.Text = _wfr.Content("AccountNumberLabel", _languageCode, True)
                ltlSortCodeLabel.Text = _wfr.Content("SortCodeLabel", _languageCode, True)
                If Not Profile.IsAnonymous AndAlso TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_wfr.Attribute("ShowDirectDebitSummary")) Then
                    hplDirectDebitSummary.Visible = True
                    hplDirectDebitSummary.Attributes.Add("data-open", "ebiz-direct-debit-summary-reveal-" & e.Item.ItemIndex)
                    uscDirectDebitSummary.Visible = True
                    uscDirectDebitSummary.PaymentRef = PaymentReference
                End If
            End If
            If plhCreditFinance.Visible Then
                Dim ltlCreditFinancePlan As Literal = CType(e.Item.FindControl("ltlCreditFinancePlan"), Literal)
                Dim ltlCreditFinanceRequestID As Literal = CType(e.Item.FindControl("ltlCreditFinanceRequestID"), Literal)
                ltlCreditFinancePlan.Text = _wfr.Content("ltlCreditFinancePlan", _languageCode, True)
                ltlCreditFinanceRequestID.Text = _wfr.Content("ltlCreditFinanceRequestID", _languageCode, True)
            End If
            If plhEP.Visible Then
                Dim ltlEPCardNumber As Literal = CType(e.Item.FindControl("ltlEPurseCardNumber"), Literal)
                ltlEPCardNumber.Text = _wfr.Content("ltlEPurseCardNumber", _languageCode, True)
            End If
        End If
    End Sub

    Protected Sub rptOrderDetails_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptOrderDetails.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim hlpPromotion As HyperLink = CType(e.Item.FindControl("hlpPromotion"), HyperLink)
            Dim plhLoyaltyPointsColumn As PlaceHolder = CType(e.Item.FindControl("plhLoyaltyPointsColumn"), PlaceHolder)
            Dim plhSeatsColumn As PlaceHolder = CType(e.Item.FindControl("plhSeatsColumn"), PlaceHolder)
            Dim plhQuantityColumn As PlaceHolder = CType(e.Item.FindControl("plhQuantityColumn"), PlaceHolder)
            Dim plhDespatchHeaders As PlaceHolder = CType(e.Item.FindControl("plhDespatchHeaders"), PlaceHolder)
            Dim plhPriceBand As PlaceHolder = CType(e.Item.FindControl("plhPriceBand"), PlaceHolder)
            Dim plhFulfilmentMethodColumn As PlaceHolder = CType(e.Item.FindControl("plhFulfilmentMethodColumn"), PlaceHolder)

            ' All buttons now moved to the header row.
            ' Need hiddenfields to determin which options are available per item.
            Dim hdfPrintAllowed As HiddenField = CType(e.Item.FindControl("hdfPrintAllowed"), HiddenField)
            Dim hdfPrintAllowedError As HiddenField = CType(e.Item.FindControl("hdfPrintAllowedError"), HiddenField)
            Dim hdfPrintURL As HiddenField = CType(e.Item.FindControl("hdfPrintURL"), HiddenField)
            Dim hdfCancelAllowed As HiddenField = CType(e.Item.FindControl("hdfCancelAllowed"), HiddenField)
            Dim hdfCancelAllowedError As HiddenField = CType(e.Item.FindControl("hdfCancelAllowedError"), HiddenField)
            Dim hdfCancelURL As HiddenField = CType(e.Item.FindControl("hdfCancelURL"), HiddenField)
            Dim hdfTransferAllowed As HiddenField = CType(e.Item.FindControl("hdfTransferAllowed"), HiddenField)
            Dim hdfTransferAllowedError As HiddenField = CType(e.Item.FindControl("hdfTransferAllowedError"), HiddenField)
            Dim hdfTransferURL As HiddenField = CType(e.Item.FindControl("hdfTransferURL"), HiddenField)
            Dim hdfAmendAllowed As HiddenField = CType(e.Item.FindControl("hdfAmendAllowed"), HiddenField)
            Dim hdfAmendAllowedError As HiddenField = CType(e.Item.FindControl("hdfAmendAllowedError"), HiddenField)
            Dim hdfAmendURL As HiddenField = CType(e.Item.FindControl("hdfAmendURL"), HiddenField)
            Dim hdfSeatHistoryAllowed As HiddenField = CType(e.Item.FindControl("hdfSeatHistoryAllowed"), HiddenField)
            Dim hdfSeatHistoryAllowedError As HiddenField = CType(e.Item.FindControl("hdfSeatHistoryAllowedError"), HiddenField)
            Dim hdfSeatHistoryURL As HiddenField = CType(e.Item.FindControl("hdfSeatHistoryURL"), HiddenField)
            Dim hdfSeatPrintHistoryAllowed As HiddenField = CType(e.Item.FindControl("hdfSeatPrintHistoryAllowed"), HiddenField)
            Dim hdfSeatPrintHistoryAllowedError As HiddenField = CType(e.Item.FindControl("hdfSeatPrintHistoryAllowedError"), HiddenField)
            Dim hdfSeatPrintHistoryURL As HiddenField = CType(e.Item.FindControl("hdfSeatPrintHistoryURL"), HiddenField)
            Dim orderDetailRow As DataRow = CType(e.Item.DataItem, DataRowView).Row

            plhDespatchHeaders.Visible = _showDespatchInformation
            plhPriceBand.Visible = (_component = ComponentType.None)
            If hlpPromotion.Visible Then
                Dim imgPromotion As Image = CType(e.Item.FindControl("imgPromotion"), Image)
                imgPromotion.AlternateText = _wfr.Content("PromotionsIconAltText", _languageCode, True)
                imgPromotion.ImageUrl = TEBUtilities.CheckForDBNull_String(_wfr.Attribute("PromotionIconUrl"))
            End If

            Dim seatOrPayOwnerCAT As Boolean = False
            If _isPayOwnerLoggedIn Or _loggedInCustomerNumber = TCUtilities.PadLeadingZeros(TEBUtilities.CheckForDBNull_String(e.Item.DataItem("CustomerNumber")), 12) Then
                seatOrPayOwnerCAT = True
            End If

            'Set the cancel links 
            If _allowCancel Then
                If Not AgentProfile.AgentPermissions.CanCancelTicket And Not AgentProfile.AgentPermissions.CanCancelPastDatedTickets And AgentProfile.IsAgent Then
                    hdfCancelAllowed.Value = False
                    hdfCancelAllowedError.Value = getActionErrorDescription(actionErrorList.AGENTAUTHORITYFORCANCEL)
                Else
                    If Not seatOrPayOwnerCAT Then
                        hdfCancelAllowed.Value = False
                        hdfCancelAllowedError.Value = getActionErrorDescription(actionErrorList.NoCancelUnlessSeatOwner)
                    ElseIf orderDetailRow("StatusCode") = "CANCEL" Then
                        hdfCancelAllowed.Value = False
                        hdfCancelAllowedError.Value = getActionErrorDescription(actionErrorList.ItemCancelled)
                    ElseIf (TEBUtilities.IsAgent AndAlso e.Item.DataItem("CATCancelAgent")) OrElse (Not (TEBUtilities.IsAgent) AndAlso e.Item.DataItem("CATCancelWeb")) Then
                        If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(orderDetailRow("IsDDRefundProduct")) AndAlso Not AgentProfile.AgentPermissions.CanGiveDirectDebitRefund Then
                            hdfCancelAllowed.Value = False
                            hdfCancelAllowedError.Value = getActionErrorDescription(actionErrorList.AgentCannotGiveDDRefund)
                            'can't cancel past dated tickets without AgentPermissions
                        ElseIf CDate(orderDetailRow("ProductDate")) < DateTime.Now AndAlso Not AgentProfile.AgentPermissions.CanCancelPastDatedTickets Then
                            hdfCancelAllowed.Value = False
                            hdfCancelAllowedError.Value = getActionErrorDescription(actionErrorList.AgentAurthorityForCancelPastTickets)
                        Else
                            hdfCancelAllowed.Value = True
                            hdfCancelURL.Value = GetCATNavigationLink(orderDetailRow, GlobalConstants.CATMODE_CANCEL)
                        End If
                    Else
                        hdfCancelAllowed.Value = False
                        hdfCancelAllowedError.Value = getActionErrorDescription(e.Item.DataItem("ReasonCancelUnavailable"))
                    End If
                End If
            End If

            'Set the ammend links
            If _allowAmend Then
                If Not AgentProfile.AgentPermissions.CanAmendTicket And AgentProfile.IsAgent Then
                    hdfAmendAllowed.Value = False
                    hdfAmendAllowedError.Value = getActionErrorDescription(actionErrorList.AGENTAUTHORITYFORAMEND)
                Else
                    If Not seatOrPayOwnerCAT Then
                        hdfAmendAllowed.Value = False
                        hdfAmendAllowedError.Value = getActionErrorDescription(actionErrorList.NoAmendUnlessSeatOwner)
                    ElseIf AgentProfile.BulkSalesMode Then
                        hdfAmendAllowed.Value = False
                        hdfAmendAllowedError.Value = getActionErrorDescription(actionErrorList.NoAmendForBulkAgent)
                    ElseIf _bulkID <> "0" Then
                        hdfAmendAllowed.Value = False
                        hdfAmendAllowedError.Value = getActionErrorDescription(actionErrorList.NoAmendForBulkSales)
                    ElseIf orderDetailRow("StatusCode") = "CANCEL" Then
                        hdfAmendAllowed.Value = False
                        hdfAmendAllowedError.Value = getActionErrorDescription(actionErrorList.ItemCancelled)
                    ElseIf (TEBUtilities.IsAgent AndAlso e.Item.DataItem("CATAmmendAgent") AndAlso Not AgentProfile.BulkSalesMode) OrElse
                                     (Not (TEBUtilities.IsAgent) AndAlso e.Item.DataItem("CATAmmendWeb")) Then
                        hdfAmendAllowed.Value = True
                        hdfAmendURL.Value = GetCATNavigationLink(orderDetailRow, GlobalConstants.CATMODE_AMEND)
                    Else
                        hdfAmendAllowed.Value = False
                        hdfAmendAllowedError.Value = getActionErrorDescription(e.Item.DataItem("ReasonAmendUnavailable"))
                    End If
                End If
            End If

            'Set the transfer links
            If _allowTransfer Then
                If Not AgentProfile.AgentPermissions.CanTransferTicket And AgentProfile.IsAgent Then
                    hdfTransferAllowed.Value = False
                    hdfTransferAllowedError.Value = getActionErrorDescription(actionErrorList.AGENTAUTHORITYFORTRANSFER)
                Else
                    If Not seatOrPayOwnerCAT Then
                        hdfTransferAllowed.Value = False
                        hdfTransferAllowedError.Value = getActionErrorDescription(actionErrorList.NoTransUnlessSeatOwner)
                    ElseIf AgentProfile.BulkSalesMode Then
                        hdfTransferAllowed.Value = False
                        hdfTransferAllowedError.Value = getActionErrorDescription(actionErrorList.NoTransForBulkAgent)
                    ElseIf _bulkID <> "0" Then
                        hdfTransferAllowed.Value = False
                        hdfTransferAllowedError.Value = getActionErrorDescription(actionErrorList.NoTransForBulkSales)
                    ElseIf orderDetailRow("StatusCode") = "CANCEL" Then
                        hdfTransferAllowed.Value = False
                        hdfTransferAllowedError.Value = getActionErrorDescription(actionErrorList.ItemCancelled)
                    ElseIf (TEBUtilities.IsAgent AndAlso e.Item.DataItem("CATTransferAgent")) OrElse
                                         (Not (Talent.eCommerce.Utilities.IsAgent) AndAlso e.Item.DataItem("CATTransferWeb")) Then
                        hdfTransferAllowed.Value = True
                        hdfTransferURL.Value = GetCATNavigationLink(orderDetailRow, GlobalConstants.CATMODE_TRANSFER)
                    Else
                        hdfTransferAllowed.Value = False
                        hdfTransferAllowedError.Value = getActionErrorDescription(e.Item.DataItem("ReasonTransferUnavailable"))
                    End If
                End If
            End If

            'Set the print
            If _allowPrint Then
                If orderDetailRow("StatusCode") = "CANCEL" Then
                    hdfPrintAllowed.Value = False
                    hdfPrintAllowedError.Value = getActionErrorDescription(actionErrorList.ItemCancelled)
                ElseIf e.Item.DataItem("IsPrintable") Then
                    hdfPrintAllowed.Value = True
                Else
                    hdfPrintAllowed.Value = False
                    hdfPrintAllowedError.Value = getActionErrorDescription(actionErrorList.NoPrintForItem)
                End If
            End If

            'Seat History and Print History
            If _allowSeatHist Then
                hdfSeatHistoryAllowed.Value = True
                hdfSeatPrintHistoryAllowed.Value = True
                hdfSeatHistoryURL.Value = GetSeatHistoryUrl(e.Item.DataItem("ProductCode").Trim, e.Item.DataItem("Seat").Trim)
                hdfSeatPrintHistoryURL.Value = GetSeatPrintHistoryUrl(e.Item.DataItem("ProductCode").Trim, e.Item.DataItem("Seat").Trim)
            End If

            'Loyalty Ponts
            If _isNull = False Then
                plhLoyaltyPointsColumn.Visible = True
                Dim loyaltyPointsValue = e.Item.DataItem("LoyaltyPoints")
                If String.IsNullOrEmpty(loyaltyPointsValue) Then
                    Dim td As HtmlTableCell = CType(e.Item.FindControl("loyalty"), HtmlTableCell)
                    td.InnerHtml = "&nbsp;"
                End If
            Else
                plhLoyaltyPointsColumn.Visible = False
            End If
            'Bundles
            Dim plhDetailsOption As PlaceHolder = CType(e.Item.FindControl("plhDetailsOption"), PlaceHolder)
            plhDetailsOption.Visible = e.Item.DataItem("IsProductBundle")

            plhFulfilmentMethodColumn.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("IsPurchaseDetailsFulfilmentEnabled"))
            ' Set the detail/hyperlink for additoinal information.
            ' Note: Current priority order for the link is 1-Corp 2-Bulk 3-Package
            ' Ticketing Packages / Corp Sales            
            If IsNumeric(Utilities.CheckForDBNull_String(e.Item.DataItem("CallId")).Trim) AndAlso Utilities.CheckForDBNull_Decimal(e.Item.DataItem("CallId")) > 0 Then
                hdfPrintAllowed.Value = False
                hdfAmendAllowed.Value = False
                hdfTransferAllowed.Value = False
                hdfSeatHistoryAllowed.Value = False
                hdfSeatPrintHistoryAllowed.Value = False
                hdfPrintAllowedError.Value = getActionErrorDescription(actionErrorList.NoActionForPackageHeader)
                hdfAmendAllowedError.Value = getActionErrorDescription(actionErrorList.NoActionForPackageHeader)
                hdfTransferAllowedError.Value = getActionErrorDescription(actionErrorList.NoActionForPackageHeader)
                hdfSeatHistoryAllowedError.Value = getActionErrorDescription(actionErrorList.NoActionForPackageHeader)
                hdfSeatPrintHistoryAllowedError.Value = getActionErrorDescription(actionErrorList.NoActionForPackageHeader)

                'is it package
                plhDetailsOption.Visible = False

                Dim plhPackageDetailLink As PlaceHolder = CType(e.Item.FindControl("plhPackageDetailLink"), PlaceHolder)

                If _component <> ComponentType.BULK Then
                    Dim hplDetails As HyperLink = CType(e.Item.FindControl("hplDetails"), HyperLink)
                    hplDetails.NavigateUrl = "~/PagesLogin/Orders/PurchaseDetails.aspx?payref=" & e.Item.DataItem("PaymentReference")
                    hplDetails.NavigateUrl &= "&callid=" & Utilities.CheckForDBNull_String(e.Item.DataItem("CallId")).Trim
                    Dim IsCorporateLinkedHomeGame As Boolean = CType(orderDetailRow.Item("IsCorporateLinkedHomeGame"), Boolean)
                    If e.Item.DataItem("ProductType") <> GlobalConstants.SEASONTICKETPRODUCTTYPE AndAlso IsCorporateLinkedHomeGame Then
                        hplDetails.NavigateUrl &= "&productcode=" & orderDetailRow.Item("ProductCode")
                    End If
                    plhPackageDetailLink.Visible = True

                    'Also include a flag to determine if this package was sold in bulk mode. 
                    If e.Item.DataItem("BulkID") > 0 Then
                        hplDetails.NavigateUrl = hplDetails.NavigateUrl & "&bulkCall=Y"
                    End If
                Else
                    plhPackageDetailLink.Visible = False
                End If
            End If

            'Bulk Sales - every row will be a bulk sale as they cannot be mixed with normal sales
            If e.Item.DataItem("BulkID") <> 0 Then
                hdfAmendAllowed.Value = False
                hdfTransferAllowed.Value = False
                hdfSeatHistoryAllowed.Value = False
                hdfSeatPrintHistoryAllowed.Value = False
                plhQuantityColumn.Visible = True
                plhSeatsColumn.Visible = False
                hdfAmendAllowedError.Value = getActionErrorDescription(actionErrorList.NoAmendForBulkHeader)
                hdfTransferAllowedError.Value = getActionErrorDescription(actionErrorList.NoTransferForBulkHeader)
                hdfSeatHistoryAllowedError.Value = getActionErrorDescription(actionErrorList.NoSeatHistForBulkHeader)
                hdfSeatPrintHistoryAllowedError.Value = getActionErrorDescription(actionErrorList.NoSeatPHistForBulkHeader)

                Dim plhPackageDetailLink As PlaceHolder = CType(e.Item.FindControl("plhPackageDetailLink"), PlaceHolder)
                If _component <> ComponentType.BULK AndAlso _showDespatchInformation Then
                    Dim hplDetails As HyperLink = CType(e.Item.FindControl("hplDetails"), HyperLink)
                    hplDetails.NavigateUrl = "~/PagesLogin/Orders/PurchaseDetails.aspx?payref=" & e.Item.DataItem("PaymentReference")
                    hplDetails.NavigateUrl = hplDetails.NavigateUrl & "&bulkid=" & Utilities.CheckForDBNull_String(e.Item.DataItem("BulkID")).Trim
                    plhPackageDetailLink.Visible = True
                Else
                    plhPackageDetailLink.Visible = False
                End If
            Else
                plhSeatsColumn.Visible = True
                plhQuantityColumn.Visible = False
            End If

            'Show Package items
            If CType(e.Item.DataItem("IsPackageItem"), Boolean) AndAlso e.Item.DataItem("BulkID") = 0 AndAlso
                    IsNumeric(Utilities.CheckForDBNull_String(e.Item.DataItem("CallId")).Trim) AndAlso
                    Utilities.CheckForDBNull_Decimal(e.Item.DataItem("CallId")) = 0 Then
                Dim plhPackageDetailLink As PlaceHolder = CType(e.Item.FindControl("plhPackageDetailLink"), PlaceHolder)
                If _component <> ComponentType.BULK AndAlso _showDespatchInformation Then
                    Dim seatDetails As DESeatDetails = CType(e.Item.DataItem("RelatingBundleSeat"), DESeatDetails)
                    seatDetails.FormattedSeat = e.Item.DataItem("Seat").ToString
                    Dim packageKey As String = Utilities.FixStringLength(e.Item.DataItem("ProductCode").ToString, 6) + seatDetails.UnFormattedSeat

                    Dim hplDetails As HyperLink = CType(e.Item.FindControl("hplDetails"), HyperLink)
                    hplDetails.NavigateUrl = "~/PagesLogin/Orders/PurchaseDetails.aspx?payref=" & e.Item.DataItem("PaymentReference")
                    hplDetails.NavigateUrl = hplDetails.NavigateUrl & "&packagekey=" & packageKey
                    plhPackageDetailLink.Visible = True
                Else
                    plhPackageDetailLink.Visible = False
                End If
            End If
        End If
    End Sub

    Protected Sub rptTrackingNumbers_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptTrackingNumbers.ItemDataBound
        Dim ltlOrderTrackingNumber As Literal = CType(e.Item.FindControl("ltlOrderTrackingNumber"), Literal)
        Dim hplOrderTrackingNumber As HyperLink = CType(e.Item.FindControl("hplOrderTrackingNumber"), HyperLink)
        ltlOrderTrackingNumber.Text = _wfr.Content("ltlOrderTrackingNumber", _languageCode, True)
        hplOrderTrackingNumber.NavigateUrl = (TEBUtilities.CheckForDBNull_String(_wfr.Attribute("OrderTrackingNumberNavigateURL"))).Trim()
        hplOrderTrackingNumber.Target = "_blank"
    End Sub

    Protected Sub rptOrderDetails_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles rptOrderDetails.PreRender
        If rptOrderDetails.Items.Count > 0 Then
            Dim hasLoyaltyPoints As Boolean = False
            Dim hasBundleProducts As Boolean = False
            Dim hasPackageLink As Boolean = False
            For Each item As RepeaterItem In rptOrderDetails.Items
                'Loop through the order repeater and see if there are any loyalty points to show the column header
                If item.FindControl("plhLoyaltyPointsColumn") IsNot Nothing AndAlso CType(item.FindControl("plhLoyaltyPointsColumn"), PlaceHolder).Visible Then
                    hasLoyaltyPoints = True
                End If
                'Loop through the order repeater and see if there are any bundle products to show the column header
                If item.FindControl("plhDetailsOption") IsNot Nothing AndAlso CType(item.FindControl("plhDetailsOption"), PlaceHolder).Visible Then
                    hasBundleProducts = True
                End If
                'Loop through the order repeater and see if there are any packages to show the column header
                If item.FindControl("plhPackageDetailLink") IsNot Nothing AndAlso CType(item.FindControl("plhPackageDetailLink"), PlaceHolder).Visible Then
                    hasPackageLink = True
                End If
            Next
            If rptOrderDetails.Controls(0).Controls(0).FindControl("plhLoyaltyPointsColumn") IsNot Nothing Then
                CType(rptOrderDetails.Controls(0).Controls(0).FindControl("plhLoyaltyPointsColumn"), PlaceHolder).Visible = hasLoyaltyPoints
            End If
            If rptOrderDetails.Controls(0).Controls(0).FindControl("plhDetailsOption1") IsNot Nothing Then
                CType(rptOrderDetails.Controls(0).Controls(0).FindControl("plhDetailsOption1"), PlaceHolder).Visible = hasBundleProducts
            End If
            If rptOrderDetails.Controls(0).Controls(0).FindControl("plhDetailsOption2") IsNot Nothing Then
                CType(rptOrderDetails.Controls(0).Controls(0).FindControl("plhDetailsOption2"), PlaceHolder).Visible = hasPackageLink
            End If
            If rptOrderDetails.Controls(0).Controls(0).FindControl("plhQuantityColumn") IsNot Nothing Then
                CType(rptOrderDetails.Controls(0).Controls(0).FindControl("plhQuantityColumn"), PlaceHolder).Visible = _isABulkSalePaymentRef
            End If
            If rptOrderDetails.Controls(0).Controls(0).FindControl("plhFulfilmentMethodColumn") IsNot Nothing Then
                CType(rptOrderDetails.Controls(0).Controls(0).FindControl("plhFulfilmentMethodColumn"), PlaceHolder).Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("IsPurchaseDetailsFulfilmentEnabled"))
            End If
            If rptOrderDetails.Controls(0).Controls(0).FindControl("plhSeatsColumn") IsNot Nothing Then
                CType(rptOrderDetails.Controls(0).Controls(0).FindControl("plhSeatsColumn"), PlaceHolder).Visible = Not _isABulkSalePaymentRef
            End If
            If rptOrderDetails.Controls(0).Controls(0).FindControl("plhDespatchHeaders") IsNot Nothing Then
                CType(rptOrderDetails.Controls(0).Controls(0).FindControl("plhDespatchHeaders"), PlaceHolder).Visible = _showDespatchInformation
            End If
            If rptOrderDetails.Controls(0).Controls(0).FindControl("plhPriceBand") IsNot Nothing Then
                CType(rptOrderDetails.Controls(0).Controls(0).FindControl("plhPriceBand"), PlaceHolder).Visible = (_component = ComponentType.None)
            End If
        End If
    End Sub

    Protected Sub btnPrintSelectedItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPrintSelectedItem.Click
        Dim errorList As List(Of String)
        Dim successURL As String
        Dim printedCnt As Integer = 0
        Dim errorCnt As Integer = 0

        'Print Validation of selected items
        errorList = validateButtonAction(ButtonOption.Print, successURL)
        If errorList.Count > 0 Then
            HttpContext.Current.Session.Add(_ERRORMESSAGE, errorList.Item(0))
            Response.Redirect(Request.RawUrl)
        Else
            Dim chkSelectAll As CheckBox = CType(rptOrderDetails.Controls(0).Controls(0).FindControl("chkSelectAll"), CheckBox)
            If chkSelectAll.Visible AndAlso chkSelectAll.Checked AndAlso Not _payRefHasCancelledOrder Then
                Dim printEntity As New DEPrint
                printEntity.PaymentReference = PaymentReference
                errorList = ProcessPrintTickets(printEntity)
                If errorList.Count > 0 Then
                    HttpContext.Current.Session.Add(_ERRORMESSAGE, errorList.Item(0))
                Else
                    Dim successMsg = _wfr.Content("PrintSuccessMessage", _languageCode, True)
                    successMsg.Replace("<<<TicketsPrinted>>>", "All")
                    HttpContext.Current.Session.Add(_SUCCESSMESSAGE, successMsg)
                End If
                Response.Redirect(Request.RawUrl)
            Else
                ' Cancel By Payment Reference or selected item cancel.
                For Each order As RepeaterItem In rptOrderDetails.Items
                    Dim chkSelectedItem As CheckBox = CType(order.FindControl("chkSelectedItem"), CheckBox)
                    If chkSelectedItem.Checked Then
                        Dim relativeRecordNumber As String = CType(order.FindControl("hdfRRN"), HiddenField).Value
                        Dim bulkSalesID As Integer = 0
                        Dim hdfBulkSalesID As HiddenField = CType(order.FindControl("hdfBulkID"), HiddenField)
                        Dim printEntity As New DEPrint
                        If hdfBulkSalesID IsNot Nothing AndAlso hdfBulkSalesID.Value.Length > 0 AndAlso Integer.TryParse(hdfBulkSalesID.Value, bulkSalesID) AndAlso bulkSalesID > 0 Then
                            printEntity.BulkSalesID = bulkSalesID
                            printEntity.RelativeRecordNumber = 0
                            printEntity.PaymentReference = PaymentReference
                        Else
                            printEntity.BulkSalesID = 0
                            printEntity.RelativeRecordNumber = relativeRecordNumber
                        End If
                        errorList = ProcessPrintTickets(printEntity)
                        If errorList.Count > 0 Then
                            errorCnt += 1
                        Else
                            printedCnt += 1
                        End If
                    End If
                Next
                If errorCnt > 0 Then
                    HttpContext.Current.Session.Add(_ERRORMESSAGE, _wfr.Content("PrintErrorMessage", _languageCode, True))
                End If
                If printedCnt > 0 Then
                    Dim successMsg = _wfr.Content("PrintSuccessMessage", _languageCode, True)
                    successMsg.Replace("<<<TicketsPrinted>>>", printedCnt.ToString)
                    HttpContext.Current.Session.Add(_SUCCESSMESSAGE, successMsg)
                End If
                Response.Redirect(Request.RawUrl)
            End If
        End If
    End Sub

    Protected Sub btnCancelSelectedItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelSelectedItem.Click
        Dim errorList As List(Of String)
        Dim cancellationLink As String = String.Empty
        cancellationLink = HttpContext.Current.Request.Url.AbsoluteUri

        'Print Validation of selected items
        errorList = validateButtonAction(ButtonOption.Cancel, String.Empty)
        If errorList.Count > 0 Then
            HttpContext.Current.Session.Add(_ERRORMESSAGE, errorList.Item(0))
            Response.Redirect(Request.RawUrl)
        Else
            Dim chkSelectAll As CheckBox = CType(rptOrderDetails.Controls(0).Controls(0).FindControl("chkSelectAll"), CheckBox)

            '  Cancel All (Cancel by payment ref)
            If chkSelectAll.Visible AndAlso chkSelectAll.Checked AndAlso _bulkID = "0" AndAlso Not _selectedACorporateItemForAction Then

                ' We present the confirmation page to inform the user that any current items int eh basket will be cleared before
                ' the cancellation basket is created.
                If Profile.Basket.BasketItems.Count > 0 Then
                    cancellationLink = "~/PagesLogin/Orders/CATConfirm.aspx?Mode=CA&TrnEnq=True&PayRef=" & PaymentReference
                Else
                    cancellationLink = "~/Redirect/TicketingGateway.aspx?page=catconfirm.aspx&function=addtobasket&catmode=CA&payref=" & PaymentReference & "&istrnxenq=True"
                End If
            Else

                'Cancel single or multiple items.
                Dim selectedOrderItems As New DataTable
                If HttpContext.Current.Session.Item(_CATMODE_CANCELMULTIPLE_SESSION_KEY) IsNot Nothing Then
                    HttpContext.Current.Session.Remove(_CATMODE_CANCELMULTIPLE_SESSION_KEY)
                End If
                createSelectItemsTable(selectedOrderItems)
                For Each order As RepeaterItem In rptOrderDetails.Items
                    Dim chkSelectedItem As CheckBox = CType(order.FindControl("chkSelectedItem"), CheckBox)
                    If chkSelectedItem.Checked Then
                        Dim orderItemRow As DataRow = selectedOrderItems.NewRow

                        addItemsDataToTable("PaymentReference", CType(order.FindControl("hdfPaymentReference"), HiddenField), orderItemRow)
                        addItemsDataToTable("ProductCode", CType(order.FindControl("hdfProductCode"), HiddenField), orderItemRow)
                        addItemsDataToTable("ProductDate", CType(order.FindControl("hdfProductDate"), HiddenField), orderItemRow)
                        addItemsDataToTable("CustomerNumber", CType(order.FindControl("hdfCustomerNumber"), HiddenField), orderItemRow)
                        addItemsDataToTable("ProductType", CType(order.FindControl("hdfProductType"), HiddenField), orderItemRow)
                        addItemsDataToTable("StadiumCode", CType(order.FindControl("hdfStadiumCode"), HiddenField), orderItemRow)
                        addItemsDataToTable("BulkID", CType(order.FindControl("hdfBulkID"), HiddenField), orderItemRow)
                        addItemsDataToTable("CallID", CType(order.FindControl("hdfCallID"), HiddenField), orderItemRow)

                        'If we have selected a bulk header then we add an empty seat to the item.
                        If _selectedABulkItemForAction OrElse _selectedACorporateItemForAction Then
                            addItemsDataToTable("Seat", New HiddenField, orderItemRow)
                        Else
                            addItemsDataToTable("Seat", CType(order.FindControl("hdfSeat"), HiddenField), orderItemRow)
                            Dim seat As New DESeatDetails
                            seat.FormattedSeat = orderItemRow("Seat").ToString()
                            orderItemRow("Seat") = seat.UnFormattedSeat
                        End If
                        selectedOrderItems.Rows.Add(orderItemRow)
                    End If
                Next

                ' Create a cancellation forwarding URL
                If selectedOrderItems.Rows.Count() > 0 Then
                    If _selectedABulkItemForAction OrElse _selectedACorporateItemForAction Then
                        cancellationLink = GetCATNavigationLink(selectedOrderItems.Rows(0), GlobalConstants.CATMODE_CANCEL)
                    Else
                        cancellationLink = GetCATNavigationLink(selectedOrderItems.Rows(0), GlobalConstants.CATMODE_CANCELMULTIPLE)
                        HttpContext.Current.Session.Add(_CATMODE_CANCELMULTIPLE_SESSION_KEY, selectedOrderItems)
                    End If
                End If
            End If
            Response.Redirect(cancellationLink)
        End If
    End Sub

    Protected Sub btnTransferSelectedItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnTransferSelectedItem.Click
        Dim errorList As List(Of String)
        Dim successURL As String

        'Transfer Validation of selected items
        errorList = validateButtonAction(ButtonOption.Transfer, successURL)
        If errorList.Count > 0 Then
            HttpContext.Current.Session.Add(_ERRORMESSAGE, errorList.Item(0))
            Response.Redirect(Request.RawUrl)
        ElseIf Not successURL Is Nothing Then
            Response.Redirect(successURL)
        End If
    End Sub

    Protected Sub btnAmendSelectedItem_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAmendSelectedItem.Click
        Dim errorList As List(Of String)
        Dim successURL As String

        'Amend Validation of selected items
        errorList = validateButtonAction(ButtonOption.Amend, successURL)
        If errorList.Count > 0 Then
            HttpContext.Current.Session.Add(_ERRORMESSAGE, errorList.Item(0))
            Response.Redirect(Request.RawUrl)
        ElseIf Not successURL Is Nothing Then
            Response.Redirect(successURL)
        End If
    End Sub

    Protected Sub rptBundles_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptBundles.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim rptProductsInABundle As Repeater = CType(e.Item.FindControl("rptProductsInABundle"), Repeater)
            If rptProductsInABundle IsNot Nothing Then
                Dim dtProducts As DataTable = CType(e.Item.DataItem, DataTable)
                rptProductsInABundle.DataSource = dtProducts
                rptProductsInABundle.DataBind()
            End If
        End If
    End Sub

    Protected Sub rptComponentSummary_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim drComponentSummary As DataRowView = CType(e.Item.DataItem, DataRowView)
            Dim ltlComponentStandDesc As Literal = CType(e.Item.FindControl("ltlComponentStandDesc"), Literal)
            Dim plhComponentStandDesc As PlaceHolder = CType(e.Item.FindControl("plhComponentStandDesc"), PlaceHolder)
            Dim ltlComponentDetail As Literal = CType(e.Item.FindControl("ltlComponentDetail"), Literal)
            Dim displayFormat As String = _wfr.Content("ComponentDetailDisplayFormat", _languageCode, True)
            If _tempStandDesc <> Utilities.CheckForDBNull_String(drComponentSummary.Row("StandDesc")) Then
                plhComponentStandDesc.Visible = True
                ltlComponentStandDesc.Text = Utilities.CheckForDBNull_String(drComponentSummary.Row("StandDesc"))
                _tempStandDesc = Utilities.CheckForDBNull_String(drComponentSummary.Row("StandDesc"))
            Else
                plhComponentStandDesc.Visible = False
            End If
            displayFormat = displayFormat.Replace("<<QUANTITY>>", Utilities.CheckForDBNull_String(drComponentSummary.Row("Quantity")))
            displayFormat = displayFormat.Replace("<<DESCRIPTION>>", Utilities.CheckForDBNull_String(drComponentSummary.Row("DisplayDesc")))
            ltlComponentDetail.Text = displayFormat
        End If
    End Sub

    Protected Sub rptPackageDetails_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles rptPackageDetails.PreRender
        For Each item As RepeaterItem In rptPackageDetails.Controls
            If item.ItemType = ListItemType.Header Then
                Dim plhComponentNetPriceHdr As PlaceHolder = CType(item.FindControl("plhComponentNetPriceHdr"), PlaceHolder)
                Dim plhComponentDiscountHdr As PlaceHolder = CType(item.FindControl("plhComponentDiscountHdr"), PlaceHolder)
                Dim plhComponentTotalPriceHdr As PlaceHolder = CType(item.FindControl("plhComponentTotalPriceHdr"), PlaceHolder)
                plhComponentNetPriceHdr.Visible = _showComponentNetPrice
                plhComponentDiscountHdr.Visible = _showComponentDiscount
                plhComponentTotalPriceHdr.Visible = _showComponentTotalPrice
            End If
            If item.ItemType = ListItemType.AlternatingItem OrElse item.ItemType = ListItemType.Item Then
                Dim plhComponentNetPrice As PlaceHolder = CType(item.FindControl("plhComponentNetPrice"), PlaceHolder)
                Dim plhComponentDiscount As PlaceHolder = CType(item.FindControl("plhComponentDiscount"), PlaceHolder)
                Dim plhComponentTotalPrice As PlaceHolder = CType(item.FindControl("plhComponentTotalPrice"), PlaceHolder)
                plhComponentNetPrice.Visible = _showComponentNetPrice
                plhComponentDiscount.Visible = _showComponentDiscount
                plhComponentTotalPrice.Visible = _showComponentTotalPrice
            End If
        Next
    End Sub

    Protected Sub rptPackageDetails_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptPackageDetails.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim drPackageDetail As DataRowView = CType(e.Item.DataItem, DataRowView)
            Dim plhComponentSeatSummary As PlaceHolder = CType(e.Item.FindControl("plhComponentSeatSummary"), PlaceHolder)
            Dim plhComponentSummary As PlaceHolder = CType(e.Item.FindControl("plhComponentSummary"), PlaceHolder)
            Dim plhComponentBulkSeatSummary As PlaceHolder = CType(e.Item.FindControl("plhComponentBulkSeatSummary"), PlaceHolder)
            Dim plhComponentDetails As PlaceHolder = CType(e.Item.FindControl("plhComponentDetails"), PlaceHolder)

            If Utilities.CheckForDBNull_String(drPackageDetail.Row("PackagePriceMode")) <> GlobalConstants.PACKAGE_PRICING Then
                _showComponentTotalPrice = True
            End If
            If Utilities.CheckForDBNull_Decimal(drPackageDetail.Row("ComponentUnitPrice")) <> 0 Then
                _showComponentNetPrice = True
            End If
            If Utilities.CheckForDBNull_Decimal(drPackageDetail.Row("ComponentDiscountPercentage")) <> 0 Then
                _showComponentDiscount = True
            End If
            If Not AgentProfile.IsAgent AndAlso Utilities.CheckForDBNull_Boolean_DefaultFalse(drPackageDetail.Row("HideSeatForPWS")) Then
                HideSeatForPWS = True
            Else
                HideSeatForPWS = False
            End If

            If (Utilities.CheckForDBNull_String(drPackageDetail.Row("ComponentType")) = GlobalConstants.COMPONENT_TYPE_AVAILABILITY OrElse Utilities.CheckForDBNull_Boolean_DefaultFalse(drPackageDetail.Row("ComponentShowSeats"))) And Me._bulkCall Then
                'seat components (purchased in bulk mode)
                plhComponentSummary.Visible = False
                plhComponentSeatSummary.Visible = False
                plhComponentBulkSeatSummary.Visible = True
                plhComponentDetails.Visible = True
            ElseIf (Utilities.CheckForDBNull_String(drPackageDetail.Row("ComponentType")) = GlobalConstants.COMPONENT_TYPE_AVAILABILITY OrElse Utilities.CheckForDBNull_Boolean_DefaultFalse(drPackageDetail.Row("ComponentShowSeats"))) Then
                'seat component
                If drPackageDetail.Item("Quantity") = 0 Then
                    plhComponentDetails.Visible = False
                Else
                    plhComponentDetails.Visible = True
                End If
                plhComponentSummary.Visible = False
                plhComponentSeatSummary.Visible = True
                plhComponentBulkSeatSummary.Visible = False
                If _dsTicketingPurchase IsNot Nothing _
                AndAlso _dsTicketingPurchase.Tables("OrderEnquiryDetails") IsNot Nothing AndAlso _dsTicketingPurchase.Tables("OrderEnquiryDetails").Rows.Count > 0 Then
                    _dsTicketingPurchase.Tables("OrderEnquiryDetails").DefaultView.RowFilter = ""
                    _dsTicketingPurchase.Tables("OrderEnquiryDetails").DefaultView.RowFilter = "FEESCODE='' AND COMPONENTID = " & Utilities.CheckForDBNull_String(drPackageDetail.Row("ComponentID"))
                    If _dsTicketingPurchase.Tables("OrderEnquiryDetails").DefaultView.Count > 0 Then
                        _showComponentSeatPrices = True
                        If Utilities.CheckForDBNull_String(drPackageDetail.Row("PackagePriceMode")) <> GlobalConstants.TICKETING_PRICING Then
                            _showComponentSeatPrices = False
                        End If

                        Dim rptComponentSeatSummary As Repeater = CType(e.Item.FindControl("rptComponentSeatSummary"), Repeater)
                        If rptComponentSeatSummary IsNot Nothing Then
                            rptComponentSeatSummary.DataSource = _dsTicketingPurchase.Tables("OrderEnquiryDetails").Delete("Seat = '' ").DefaultView
                            rptComponentSeatSummary.DataBind()
                        End If
                    End If
                End If
            Else
                'other components
                plhComponentSummary.Visible = True
                plhComponentSeatSummary.Visible = False
                plhComponentBulkSeatSummary.Visible = False
                plhComponentDetails.Visible = True
                If _dsTicketingPurchase IsNot Nothing _
                AndAlso _dsTicketingPurchase.Tables("ComponentSummary") IsNot Nothing AndAlso _dsTicketingPurchase.Tables("ComponentSummary").Rows.Count > 0 Then
                    _dsTicketingPurchase.Tables("ComponentSummary").DefaultView.RowFilter = ""
                    _dsTicketingPurchase.Tables("ComponentSummary").DefaultView.RowFilter = "COMPONENTID = " & Utilities.CheckForDBNull_String(drPackageDetail.Row("ComponentID"))
                    _dsTicketingPurchase.Tables("ComponentSummary").DefaultView.Sort = "STAND ASC"
                    If _dsTicketingPurchase.Tables("ComponentSummary").DefaultView.Count > 0 Then
                        Dim rptComponentSummary As Repeater = CType(e.Item.FindControl("rptComponentSummary"), Repeater)
                        If rptComponentSummary IsNot Nothing Then
                            rptComponentSummary.DataSource = _dsTicketingPurchase.Tables("ComponentSummary").DefaultView
                            rptComponentSummary.DataBind()
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Protected Sub rptPackageHistory_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptPackageHistory.ItemDataBound
        If e.Item.ItemType = ListItemType.AlternatingItem OrElse e.Item.ItemType = ListItemType.Item Then
            Dim drPackageHistory As DataRowView = CType(e.Item.DataItem, DataRowView)
            Dim ltlComment As Literal = CType(e.Item.FindControl("ltlComment"), Literal)
            Dim plhPackageDetailsButton As PlaceHolder = CType(e.Item.FindControl("plhPackageDetailsButton"), PlaceHolder)
            If Not String.IsNullOrWhiteSpace(_packageStatusCode) Then
                If _packageStatusCode = "A" Then
                    ltlComment.Text = _wfr.Content("PackageLinkMessage", _languageCode, True)
                Else
                    ltlComment.Text = TEBUtilities.CheckForDBNull_String(drPackageHistory("Comment"))
                    ltlComment.Text = ltlComment.Text.Replace("units added", _wfr.Content("PackageLinkAddedMessage", _languageCode, True))
                    ltlComment.Text = ltlComment.Text.Replace("units removed", _wfr.Content("PackageLinkRemovedMessage", _languageCode, True))
                    ltlComment.Text = ltlComment.Text.Replace("units amended", _wfr.Content("PackageLinkAmendedMessage", _languageCode, True))
                    ltlComment.Text = ltlComment.Text.Replace("Package Cancelled", _wfr.Content("PackageLinkCancelledMessage", _languageCode, True))
                End If
                If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(drPackageHistory("PackageDiscAmended")) OrElse TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(drPackageHistory("ComponentDiscAmended")) Then
                    plhPackageDetailsButton.Visible = False
                End If
            End If
        End If
    End Sub

    Protected Sub rptPackageHistory_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptPackageHistory.ItemCommand
        If e.CommandName = "PackageHistoryDetail" Then
            Dim amendedCallID As String = e.CommandArgument.ToString.Trim
            If Not String.IsNullOrWhiteSpace(amendedCallID) AndAlso IsNumeric(amendedCallID.Trim) Then
                If Request.QueryString("payref") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(Request.QueryString("payref")) Then
                    Dim redirectURL As String = "~/PagesLogin/Orders/PurchaseDetails.aspx?payref=" & Request.QueryString("payref") & "&callid=" & amendedCallID
                    If _bulkCall Then
                        redirectURL += "&bulkCall=Y"
                    End If
                    Response.Redirect(redirectURL)
                End If
            End If
        End If
    End Sub

    Protected Sub btnBack_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.Click
        If Session("ProductDetailsPath") Is Nothing Then
            Response.Redirect(ResolveUrl("~/PagesLogin/Orders/TransactionEnquiry.aspx"))
        ElseIf Session("ProductDetailsPath").Equals("TransactionEnquiry") OrElse String.IsNullOrEmpty(Session("ProductDetailsPath").ToString) Then
            Response.Redirect(ResolveUrl("~/PagesLogin/Orders/TransactionEnquiry.aspx"))
        ElseIf Session("ProductDetailsPath").Equals("CustomerSelection") Then
            Response.Redirect(ResolveUrl("~/PagesPublic/Profile/CustomerSelection.aspx"))
        ElseIf Session("ProductDetailsPath").Equals("PurchaseHistory") Then
            If _component = ComponentType.None Then
                Response.Redirect(ResolveUrl("~/PagesLogin/Orders/PurchaseHistory.aspx"))
            Else
                Response.Redirect(ResolveUrl("~/PagesLogin/Orders/PurchaseDetails.aspx?payref=" + PaymentReference))
            End If
        ElseIf Session("ProductDetailsPath").Equals("EndOfDay") Then
            Response.Redirect(ResolveUrl("~/PagesAgent/Orders/EndOfDay.aspx"))
        ElseIf Session("ProductDetailsPath").Equals("DespatchProcess") Then
            Response.Redirect(ResolveUrl("~/PagesAgent/Despatch/DespatchProcess.aspx"))
        End If
    End Sub

    Protected Sub btnResendEmail_Click(sender As Object, e As EventArgs) Handles btnResendEmail.Click
        If _dtPurchaseHistory IsNot Nothing Then
            Dim Order_Email As New Talent.eCommerce.Order_Email
            Dim pdfAttachments As New List(Of String)
            If ModuleDefaults.HospitalityPDFAttachmentsOnConfirmationEmail Then
                Dim orderDetails As DataView = rptOrderDetails.DataSource
                Dim tgatewayFunctions As New TicketingGatewayFunctions
                Dim productCode As String = String.Empty
                Dim packageId As String = String.Empty
                Dim callId As String = String.Empty
                Dim customerNumber As String = String.Empty
                Dim leadSourceList As New List(Of TalentBusinessLogic.DataTransferObjects.Hospitality.LeadSourceDetails)
                Dim htmlContent As String = String.Empty
                Dim cssContent As String = String.Empty
                Dim fileName As String = String.Empty
                Dim pdfCreator As New Talent.eCommerce.CreatePDF
                Dim pdfPathAndFile As String = String.Empty
                Dim filePath As String = ModuleDefaults.HtmlPathAbsolute

                If Not filePath.EndsWith("\") Then filePath &= "\"
                filePath &= "HospitalityPDF\"
                orderDetails.Sort = "CallId ASC"
                For Each row As DataRowView In orderDetails
                    If row("CallId").ToString() <> 0 Then
                        If callId = row("CallId").ToString() Then
                            'same booking, no need to create new attachment
                        Else
                            callId = row("CallId").ToString()
                            productCode = row("ProductCode").ToString()
                            packageId = row("PackageId").ToString()
                            customerNumber = Profile.User.Details.LoginID
                            fileName = String.Concat(callId, "-", Now.ToString("ddMMyyyy-HHmm"), ".pdf")
                            htmlContent &= tgatewayFunctions.ProcessHospitalityDetailsView(productCode, packageId, callId, leadSourceList)
                            htmlContent &= tgatewayFunctions.ProcessHospitalityBookingView(Profile.Basket.Basket_Header_ID, customerNumber, productCode, packageId, callId, leadSourceList)
                            cssContent = Talent.eCommerce.Utilities.GetCSSContentFromFile("HospitalityPDF\" & packageId & "_package.css")
                            If cssContent.Length = 0 Then cssContent = Talent.eCommerce.Utilities.GetCSSContentFromFile("HospitalityPDF\package.css")
                            pdfPathAndFile = pdfCreator.CreateFile(fileName, filePath, htmlContent, cssContent)
                            pdfAttachments.Add(pdfPathAndFile)
                        End If
                    End If
                Next
            End If
            Order_Email.SendTicketingConfirmationEmail(PaymentReference, Nothing, Nothing, pdfAttachments)
            HttpContext.Current.Session.Add(_SUCCESSMESSAGE, _wfr.Content("ResendEmailMessage", _languageCode, True))
            Response.Redirect(Request.RawUrl)
        Else
            ltlError.Text = _wfr.Content("CantResendEmailErrorMessage", _languageCode, True)
            plhErrorMessage.Visible = True
        End If
    End Sub

    Protected Function FormatDespatchDate(ByVal despatchDate As String, ByVal ticketNumber As String) As String
        If despatchDate.Equals("000000") OrElse String.IsNullOrEmpty(ticketNumber) OrElse ticketNumber.Equals("**MEMBERSHIP**") Then
            Return String.Empty
        Else
            Return Talent.Common.Utilities.ISeriesDate(despatchDate).Date.ToString("dd/MM/yyyy")
        End If
    End Function

    Protected Function FormatTicketNumber(ByVal ticketNumber As String) As String
        If ticketNumber.Equals("S") AndAlso Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(_wfr.Content("MembershipSentText", _languageCode, True))) Then
            Return Utilities.CheckForDBNull_String(_wfr.Content("MembershipSentText", _languageCode, True))
        ElseIf ticketNumber.Equals("N") AndAlso Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(_wfr.Content("MembershipNotDespatchedText", _languageCode, True))) Then
            Return Utilities.CheckForDBNull_String(_wfr.Content("MembershipNotDespatchedText", _languageCode, True))
        ElseIf ticketNumber.Equals("**MEMBERSHIP**") AndAlso Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(_wfr.Content("MembershipTicketNumber", _languageCode, True))) Then
            Return Utilities.CheckForDBNull_String(_wfr.Content("MembershipTicketNumber", _languageCode, True))
        End If
        Return ticketNumber
    End Function

    ''' <summary>
    ''' Returns product date as string to be displayed in purchase details table
    ''' </summary>
    ''' <param name="productDate">Date of single product</param>
    ''' <param name="isBundle">Is the product a bundle product? If so then start and end date required</param>
    ''' <param name="prodStartDate">Date of first bundle product</param>
    ''' <param name="prodEndDate">Date of last bundle product </param>
    ''' <param name="hideDate">Flag to override display of date. Set in product setup</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProductDate(ByVal productDate As Date, ByVal isBundle As Boolean, ByVal prodStartDate As Date, ByVal prodEndDate As Date, Optional ByVal hideDate As Boolean = False) As String
        Dim prodDateString As String = String.Empty
        If hideDate Then
            prodDateString = String.Empty
        Else
            If isBundle AndAlso ModuleDefaults.ShowBundleDateAsRange AndAlso (prodStartDate <> Nothing And prodEndDate <> Nothing) Then
                prodDateString = prodStartDate.ToString("dd/MM/yyyy") & " - " & prodEndDate.ToString("dd/MM/yyyy")
            Else
                If productDate <> Nothing Then
                    prodDateString = TEBUtilities.GetFormattedDateAndTime(productDate, String.Empty, " ", ModuleDefaults.GlobalDateFormat, ModuleDefaults.Culture)
                End If
            End If
        End If
        Return prodDateString
    End Function
#End Region

#Region "Private Methods"

    '
    ' Postback validation Required (when javascript switched off). Any changes here need to be reviewed also in
    ' purchase-details.js : validateButtonAction()
    '
    Private Function validateButtonAction(ByVal selectedOption As ButtonOption, ByRef successURL As String) As List(Of String)
        Dim selectedCount As Integer = 0
        Dim errorArray As New List(Of String)
        _selectedABulkItemForAction = False
        _selectedACorporateItemForAction = False

        For Each order As RepeaterItem In rptOrderDetails.Items
            Dim hdfBulkId As HiddenField = CType(order.FindControl("hdfBulkID"), HiddenField)
            Dim hdfCAllId As HiddenField = CType(order.FindControl("hdfCallID"), HiddenField)
            If TEBUtilities.CheckForDBNull_Int(hdfBulkId.Value) <> 0 Then
                _selectedABulkItemForAction = True
            End If
            If TEBUtilities.CheckForDBNull_Int(hdfCAllId.Value) <> 0 Then
                _selectedACorporateItemForAction = True
            End If

            Dim chkSelectedItem As CheckBox = CType(order.FindControl("chkSelectedItem"), CheckBox)
            If chkSelectedItem.Checked Then
                selectedCount += 1

                'Validate options dependant on button clicked
                Select Case selectedOption

                    'Print Option
                    Case ButtonOption.Print
                        Dim hdfPrintAllowed As HiddenField = CType(order.FindControl("hdfPrintAllowed"), HiddenField)
                        Dim hdfPrintAllowedError As HiddenField = CType(order.FindControl("hdfPrintAllowedError"), HiddenField)
                        If hdfPrintAllowed.Value = False Then
                            errorArray.Add(hdfPrintAllowedError.Value)
                        End If

                        'Cancel Option
                    Case ButtonOption.Cancel
                        successURL = CType(order.FindControl("hdfCancelURL"), HiddenField).Value
                        Dim hdfCancelAllowed As HiddenField = CType(order.FindControl("hdfCancelAllowed"), HiddenField)
                        Dim hdfCancelAllowedError As HiddenField = CType(order.FindControl("hdfCancelAllowedError"), HiddenField)
                        If hdfCancelAllowed.Value = False Then
                            errorArray.Add(hdfCancelAllowedError.Value)
                        End If

                        'Transfer Option
                    Case ButtonOption.Transfer
                        successURL = CType(order.FindControl("hdfTransferURL"), HiddenField).Value
                        Dim hdfTransferAllowed As HiddenField = CType(order.FindControl("hdfTransferAllowed"), HiddenField)
                        Dim hdfTransferAllowedError As HiddenField = CType(order.FindControl("hdfTransferAllowedError"), HiddenField)
                        If hdfTransferAllowed.Value = False Then
                            errorArray.Add(hdfTransferAllowedError.Value)
                        End If

                        'Amend Option
                    Case ButtonOption.Amend
                        successURL = CType(order.FindControl("hdfAmendURL"), HiddenField).Value
                        Dim hdfAmendAllowed As HiddenField = CType(order.FindControl("hdfAmendAllowed"), HiddenField)
                        Dim hdfAmendAllowedError As HiddenField = CType(order.FindControl("hdfAmendAllowedError"), HiddenField)
                        If hdfAmendAllowed.Value = False Then
                            errorArray.Add(hdfAmendAllowedError.Value)
                        End If

                        'Seat History Option
                    Case ButtonOption.SeatHist
                        successURL = CType(order.FindControl("hdfSeatHistoryURL"), HiddenField).Value
                        Dim hdfSeatHistoryAllowed As HiddenField = CType(order.FindControl("hdfSeatHistoryAllowed"), HiddenField)
                        Dim hdfSeatHistoryAllowedError As HiddenField = CType(order.FindControl("hdfSeatHistoryAllowedError"), HiddenField)
                        If hdfSeatHistoryAllowed.Value = False Then
                            errorArray.Add(hdfSeatHistoryAllowedError.Value)
                        End If

                        'Seat Print History Option
                    Case ButtonOption.SeatPrintHist
                        successURL = CType(order.FindControl("hdfSeatPrintHistoryURL"), HiddenField).Value
                        Dim hdfSeatPrintHistoryAllowed As HiddenField = CType(order.FindControl("hdfSeatPrintHistoryAllowed"), HiddenField)
                        Dim hdfSeatPrintHistoryAllowedError As HiddenField = CType(order.FindControl("hdfSeatPrintHistoryAllowedError"), HiddenField)
                        If hdfSeatPrintHistoryAllowed.Value = False Then
                            errorArray.Add(hdfSeatPrintHistoryAllowedError.Value)
                        End If

                End Select
            End If
        Next

        ' Certain Options are not allowed multiple selection
        If selectedCount > 1 Then
            Select Case selectedOption
                Case ButtonOption.Transfer, ButtonOption.Amend, ButtonOption.SeatHist, ButtonOption.SeatPrintHist
                    errorArray.Add(getActionErrorDescription(actionErrorList.NoMultipleSelectionsAllowed))
            End Select

            ' Multiple selections are not allowed for bulk headers 
            If _selectedABulkItemForAction And ButtonOption.Cancel Then
                errorArray.Add(getActionErrorDescription(actionErrorList.NoMultipleSelectionsBulkHeaderAllowed))
            End If

            ' Multiple selections are not allowed for corporate package headers 
            If _selectedACorporateItemForAction And ButtonOption.Cancel Then
                errorArray.Add(getActionErrorDescription(actionErrorList.NoMultipleSelectionsCorpHeaderAllowed))
            End If
        End If

        'Cannot cancel more than 500 individual items.
        If selectedCount > 500 And rptOrderDetails.Items.Count <> selectedCount Then
            Select Case selectedOption
                Case ButtonOption.Cancel
                    errorArray.Add(getActionErrorDescription(actionErrorList.TooManySelectedItems))
            End Select
        End If

        ' No items Selected
        If selectedCount = 0 Then
            Select Case selectedOption
                Case ButtonOption.Transfer, ButtonOption.Amend, ButtonOption.SeatHist, ButtonOption.SeatPrintHist, ButtonOption.Cancel, ButtonOption.Print
                    errorArray.Add(getActionErrorDescription(actionErrorList.NoSelectionsMade))
            End Select
        End If

        'Full cancellation allowed only.
        If ButtonOption.Cancel AndAlso _fullCancelOnly And Not rptOrderDetails.Items.Count = selectedCount Then
            errorArray.Add(getActionErrorDescription(actionErrorList.FullCancelOnly))
        End If

        Return errorArray

    End Function

    Private Sub createSelectItemsTable(ByRef selectedItems As DataTable)
        With selectedItems.Columns
            .Add("PaymentReference", GetType(String))
            .Add("Seat", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("ProductDate", GetType(String))
            .Add("CustomerNumber", GetType(String))
            .Add("ProductType", GetType(String))
            .Add("StadiumCode", GetType(String))
            .Add("BulkID", GetType(String))
            .Add("CallID", GetType(String))
        End With
    End Sub

    Private Sub addItemsDataToTable(ByVal columnName As String, ByVal orderItemDataHiddenField As HiddenField, ByRef selectedItemsRow As DataRow)
        If orderItemDataHiddenField IsNot Nothing Then
            selectedItemsRow(columnName) = orderItemDataHiddenField.Value
        End If
    End Sub

    ''' <summary>
    ''' Binds the repeaters belonging to packages and sets their visibility
    ''' </summary>
    ''' <param name="ticketingDataSet"></param>
    ''' <remarks></remarks>
    Private Sub BindPackageRepeatersAndSetVisibility(ByVal ticketingDataSet As DataSet)
        If plhPackageDetailRepeater.Visible Then
            If ticketingDataSet.Tables("PackageDetail").Rows.Count > 0 Then
                plhPackageDetailRepeater.Visible = True
                _showComponentNetPrice = False
                _showComponentDiscount = False
                _showComponentTotalPrice = False
                rptPackageDetails.DataSource = SortPackage(ticketingDataSet.Tables("PackageDetail")).ToTable()
                rptPackageDetails.DataBind()
            Else
                plhPackageDetailRepeater.Visible = False
            End If

            If ticketingDataSet.Tables("PackageHistory").Rows.Count > 0 Then
                plhPackageHistoryRepeater.Visible = True
                rptPackageHistory.DataSource = ticketingDataSet.Tables("PackageHistory")
                rptPackageHistory.DataBind()
            Else
                plhPackageHistoryRepeater.Visible = False
            End If
        End If
    End Sub

    ''' <summary>
    ''' Return the retail order details 
    ''' </summary>
    ''' <remarks></remarks>
    Private Function RetrieveRetailData() As DataTable
        Dim dt As New DataTable
        If _dsTicketingPurchase.Tables("PaymentOwnerDetails") IsNot Nothing AndAlso _dsTicketingPurchase.Tables("PaymentOwnerDetails").Rows.Count > 0 Then
            Dim tempOrderId As String = _dsTicketingPurchase.Tables("PaymentOwnerDetails").Rows(0).Item("RetailTempOrderId")
            If Not String.IsNullOrWhiteSpace(tempOrderId) Then
                dt = TDataObjects.OrderSettings.TblOrderDetail.GetOrderDetailRecordsByTempOrderID(tempOrderId)
            End If
        End If
        Return dt
    End Function

    ''' <summary>
    ''' Bind the retail repeater
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub BindRetailRepeater()
        If _dtRetailHistory.Rows.Count > 0 Then
            plhRetailOrdersRepeater.Visible = True
            rptRetailOrders.DataSource = _dtRetailHistory
            rptRetailOrders.DataBind()
            ltlRetailOrdersTitle.Text = _wfr.Content("RetailOrdersTitleLabel", _languageCode, True)
        End If
    End Sub

    ''' <summary>
    ''' Show or hide the regions for package or payment details.
    ''' </summary>
    ''' <param name="Component">Package Id. If this is available then the package sections would be visible</param>
    ''' <remarks></remarks>
    Private Sub ShowPackageOrPurchaseDetail()
        Select Case _component
            Case Is = ComponentType.None
                plhOrderDetailsRepeater.Visible = True
                plhPackageID.Visible = False
                plhPackageDetail.Visible = False
                plhPackageDetailRepeater.Visible = False
                plhPackageHistoryRepeater.Visible = False
            Case Is = ComponentType.CORPORATE
                plhOrderDetailsRepeater.Visible = False
                plhPackageID.Visible = True
                plhPackageDetail.Visible = True
                plhPackageDetailRepeater.Visible = True
                plhPackageHistoryRepeater.Visible = True
            Case Is = ComponentType.PACKAGE
                plhOrderDetailsRepeater.Visible = True
                plhPackageID.Visible = True
                plhPurchaseDetail.Visible = True
                plhPackageDetail.Visible = True
                plhPackageDetailRepeater.Visible = False
                plhPackageHistoryRepeater.Visible = False
            Case Is = ComponentType.BULK
                plhOrderDetailsRepeater.Visible = True
                plhPackageID.Visible = True
                plhPurchaseDetail.Visible = True
                plhPackageDetail.Visible = True
                plhPackageDetailRepeater.Visible = False
                plhPackageHistoryRepeater.Visible = False
        End Select
    End Sub
    Private Sub AssignPaymentOwnerLoggedInFlag(ByVal dtPaymentOwnerDetail As DataTable)
        _loggedInCustomerNumber = GlobalConstants.GENERIC_CUSTOMER_NUMBER
        If Profile.IsAnonymous Then
            _loggedInCustomerNumber = GlobalConstants.GENERIC_CUSTOMER_NUMBER
        Else
            _loggedInCustomerNumber = TCUtilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)
        End If
        If dtPaymentOwnerDetail IsNot Nothing AndAlso dtPaymentOwnerDetail.Rows.Count > 0 Then
            If TCUtilities.PadLeadingZeros(TEBUtilities.CheckForDBNull_String(dtPaymentOwnerDetail.Rows(0)("PayOwnerNumber")), 12) = _loggedInCustomerNumber Then
                _isPayOwnerLoggedIn = True
            End If
        End If
    End Sub

    ''' <summary>
    ''' Assigns the CAT tickets flag of whether to show the CAT button or not based on agent or user
    ''' </summary>
    Private Sub AssignCATTicketsFlag()
        Session("CATBasketOrderDetails") = Nothing
        If Session("TicketCollectionMode") IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(Session("TicketCollectionMode")) Then
            _isTicketCollection = CType(Session("TicketCollectionMode"), Boolean)
        End If
        If _isCorporateLinkedHomeGame.Equals("Y") Then
            _allowCancel = False
            _allowTransfer = False
            _allowAmend = False
        ElseIf _isTicketCollection Then
            _allowCancel = False
            _allowTransfer = False
            _allowAmend = False
        ElseIf (Not String.IsNullOrWhiteSpace(AgentProfile.Name)) AndAlso ModuleDefaults.AllowCATTicketsByAgent Then
            _allowCancel = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("Allow_Cancel_By_Agent"))
            _allowTransfer = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("Allow_Transfer_By_Agent"))
            _allowAmend = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("Allow_Amend_By_Agent"))
        ElseIf String.IsNullOrWhiteSpace(AgentProfile.Name) AndAlso ModuleDefaults.AllowCATTicketsByUser Then
            _allowCancel = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("Allow_Cancel_By_User"))
            _allowTransfer = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("Allow_Transfer_By_User"))
            _allowAmend = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("Allow_Amend_By_User"))
        Else
            _allowCancel = False
            _allowTransfer = False
            _allowAmend = False
        End If

        ' These flags control master visibility
        ' Other reasons to not allow the options for specific items are stored in the repeater hidden fields
        If Not _allowCancel Then
            btnCancelSelectedItem.Visible = False
        End If
        If Not _allowTransfer Then
            btnTransferSelectedItem.Visible = False
        End If
        If Not _allowAmend Then
            btnAmendSelectedItem.Visible = False
        End If

    End Sub

    ''' <summary>
    ''' Assigns the Printing flags to determine if the printing feature is enabled
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AssignPrintTicketsFlag()
        If AgentProfile.IsAgent Then
            If _isTicketCollection Then
                If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("Allow_Print_By_Agent")) Then
                    _allowPrint = True
                End If
            Else
                If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfr.Attribute("Allow_Print_FromDetail_By_Agent")) Then
                    _allowPrint = True
                End If
            End If
        Else
            _allowPrint = False
        End If

        If Not _allowPrint Then
            btnPrintSelectedItem.Visible = False
        End If

    End Sub

    ''' <summary>
    ''' Assigns the Printing flags to determine if the seat history feature is enabled
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AssignSeatHistoryTicketsFlag()
        If AgentProfile.IsAgent Then
            _allowSeatHist = True
        Else
            _allowSeatHist = False
        End If

        If Not _allowSeatHist Then
            btnSeatHistorySelectedItem.Visible = False
            btnSeatPrintHistorySelectedItem.Visible = False
        End If

    End Sub

    ''' <summary>
    ''' Return potential Error/Warning codes for CAT and print buttons for each item 
    ''' </summary>
    ''' <param name="errorCode">Error code to add to the description key</param>
    ''' <returns>Full description of the error for display</returns>
    ''' <remarks></remarks>
    Private Function getActionErrorDescription(ByVal errorCode As String) As String
        Dim actionErrorDescKey As String
        actionErrorDescKey = "ActionErrorDesc_" + errorCode
        Return Utilities.CheckForDBNull_String(_wfr.Content(actionErrorDescKey, _languageCode, True))
    End Function


    ''' <summary>
    ''' Set the payment owner information
    ''' </summary>
    ''' <param name="paymentOwnerData">The payment owner data table</param>
    ''' <remarks></remarks>
    Private Sub setPaymentOwnerData(ByRef paymentOwnerData As DataTable)
        If paymentOwnerData.Rows.Count = 1 Then
            ltlPaymentOwner.Text = _wfr.Content("PaymentOwnerDetailsTitle", _languageCode, True)
            ltlName.Text = paymentOwnerData.Rows(0)("Name").ToString().Trim()
            ltlAddressLine1.Text = paymentOwnerData.Rows(0)("AddressLine1").ToString().Trim()
            ltlAddressLine2.Text = paymentOwnerData.Rows(0)("AddressLine2").ToString().Trim()
            ltlAddressLine3.Text = paymentOwnerData.Rows(0)("AddressLine3").ToString().Trim()
            ltlAddressLine4.Text = paymentOwnerData.Rows(0)("AddressLine4").ToString().Trim()
            ltlPostCode1.Text = paymentOwnerData.Rows(0)("PostCodePart1").ToString().Trim()
            ltlPostCode2.Text = paymentOwnerData.Rows(0)("PostCodePart2").ToString().Trim()
        End If
        plhName.Visible = (ltlName.Text.Length > 0)
        plhAddressLine1.Visible = (ltlAddressLine1.Text.Length > 0)
        plhAddressLine2.Visible = (ltlAddressLine2.Text.Length > 0)
        plhAddressLine3.Visible = (ltlAddressLine3.Text.Length > 0)
        plhAddressLine4.Visible = (ltlAddressLine4.Text.Length > 0)
        plhPostCode.Visible = (Not String.IsNullOrWhiteSpace(ltlPostCode1.Text) AndAlso Not String.IsNullOrWhiteSpace(ltlPostCode2.Text))
        ltlPaymentDetails.Text = _wfr.Content("PurchaseDetailsTitle", _languageCode, True)
        ltlPaymentReferenceLabel.Text = _wfr.Content("PurchasePayRefLabel", _languageCode, True)
        If AgentProfile.IsAgent Then
            ltlAgentLabel.Text = _wfr.Content("AgentLabel", _languageCode, True)
            ltlAgentName.Text = paymentOwnerData.Rows(0)("OriginatedSource").ToString().Trim()
            ltlBatchReferenceLabel.Text = _wfr.Content("PurchaseBatchLabel", _languageCode, True)
            ltlBatchReference.Text = paymentOwnerData.Rows(0)("BatchDetail").ToString().Trim().TrimStart(GlobalConstants.LEADING_ZEROS)
            plhAgent.Visible = True
        Else
            plhAgent.Visible = False
        End If
        ltlTransactionDeteLabel.Text = _wfr.Content("PurchaseTransDateLabel", _languageCode, True)
        ltlTransactionDate.Text = TEBUtilities.GetFormattedDateAndTime(paymentOwnerData.Rows(0)("TransactionDate").ToString().Trim(), String.Empty, " ", ModuleDefaults.GlobalDateFormat, ModuleDefaults.Culture)
    End Sub

    ''' <summary>
    ''' Displays the package details
    ''' </summary>
    ''' <param name="dtStatus">Status table containing the package details</param>
    ''' <remarks></remarks>
    Private Sub PopulateParentDetails(ByVal dtStatus As DataTable, ByVal dtParentProduct As DataTable)
        If _component <> ComponentType.None Then

            If plhPackageDetail.Visible Then
                ltlPackageDescription.Text = Server.HtmlDecode(dtStatus.Rows(0)("PackageDescription").ToString().Trim())
                ltlProductDescription.Text = Server.HtmlDecode(dtStatus.Rows(0)("ProductDescription").ToString().Trim())
            End If

            Select Case _component
                Case Is = ComponentType.CORPORATE
                    If AgentProfile.IsAgent Then
                        plhPackageStatus.Visible = True
                        _packageStatusCode = TEBUtilities.CheckForDBNull_String(dtStatus.Rows(0)("PackageStatus")).Trim
                        ltlPackageStatus.Text = _wfr.Content("PackageStatus_" & _packageStatusCode, _languageCode, True)
                    End If
                    ltlPackageIdLabel.Text = _wfr.Content("CorporatePackageIdLabel", _languageCode, True)
                    ltlPaymentDetails.Text = TEBUtilities.CheckForDBNull_String(_wfr.Content("CorporatePackageDetailTitle", _languageCode, True))
                    _packageType = TEBUtilities.CheckForDBNull_String(dtStatus.Rows(0)("PackageType")).Trim
                    ltlPackageId.Text = _callId

                    If TEBUtilities.CheckForDBNull_Decimal(dtStatus.Rows(0)("PackageValueNet")) <> 0 Then
                        plhPackageValueNet.Visible = True
                        ltlPackageValueNetLabel.Text = TEBUtilities.CheckForDBNull_String(_wfr.Content("ltlPackageValueNetLabel", _languageCode, True))
                        ltlPackageValueNetValue.Text = GetPrice(dtStatus.Rows(0)("PackageValueNet"), False)
                    End If
                    If TEBUtilities.CheckForDBNull_Decimal(dtStatus.Rows(0)("PackageValueVAT")) <> 0 Then
                        plhPackageValueVat.Visible = True
                        ltlPackageValueVatLabel.Text = TEBUtilities.CheckForDBNull_String(_wfr.Content("ltlPackageValueVatLabel", _languageCode, True))
                        ltlPackageValueVatValue.Text = GetPrice(dtStatus.Rows(0)("PackageValueVAT"), False)
                    End If
                    If TEBUtilities.CheckForDBNull_Decimal(dtStatus.Rows(0)("PackageValueGross")) <> 0 Then
                        plhPackageValueGross.Visible = True
                        ltlPackageValueGrossLabel.Text = TEBUtilities.CheckForDBNull_String(_wfr.Content("ltlPackageValueGrossLabel", _languageCode, True))
                        ltlPackageValueGrossValue.Text = GetPrice(dtStatus.Rows(0)("PackageValueGross"), False)
                    End If
                    If TEBUtilities.CheckForDBNull_Decimal(dtStatus.Rows(0)("PackageComponentDiscount")) <> 0 Then
                        plhPackageValueDiscountComponent.Visible = True
                        ltlPackageValueDiscountComponentLabel.Text = TEBUtilities.CheckForDBNull_String(_wfr.Content("ltlPackageValueDiscountComponentLabel", _languageCode, True))
                        ltlPackageValueDiscountComponentValue.Text = GetPrice(dtStatus.Rows(0)("PackageComponentDiscount"), False)
                    End If
                    If TEBUtilities.CheckForDBNull_Decimal(dtStatus.Rows(0)("PackagePackageDiscount")) <> 0 Then
                        plhPackageValueDiscountPackage.Visible = True
                        ltlPackageValueDiscountPackageLabel.Text = TEBUtilities.CheckForDBNull_String(_wfr.Content("ltlPackageValueDiscountPackageLabel", _languageCode, True))
                        ltlPackageValueDiscountPackageValue.Text = GetPrice(dtStatus.Rows(0)("PackagePackageDiscount"), False)
                    End If
                Case Is = ComponentType.PACKAGE
                    plhPriceBand.Visible = True
                    ltlPackageIdLabel.Text = _wfr.Content("LinkedPackageIdLabel", _languageCode, True)
                    ltlPaymentDetails.Text = TEBUtilities.CheckForDBNull_String(_wfr.Content("LinkedPackageDetailTitle", _languageCode, True))
                    ltlPriceBandLabel.Text = TEBUtilities.CheckForDBNull_String(_wfr.Content("ltlPriceBandLabelLP", _languageCode, True))
                    ltlPriceBand.Text = TEBUtilities.CheckForDBNull_String(dtParentProduct.Rows(0)("PriceBand")).Trim() &
                        "(" & TEBUtilities.CheckForDBNull_String(dtParentProduct.Rows(0)("PriceBandDesc")).Trim() & ")"
                    ltlPackageId.Text = dtParentProduct.Rows(0)("ProductDescription").ToString.Trim + " (" + dtParentProduct.Rows(0)("Seat").ToString.Trim + ")"
                Case Is = ComponentType.BULK
                    plhPriceBand.Visible = True
                    ltlPackageIdLabel.Text = _wfr.Content("BulkPackageIdLabel", _languageCode, True)
                    ltlPaymentDetails.Text = TEBUtilities.CheckForDBNull_String(_wfr.Content("BulkPackageDetailTitle", _languageCode, True))
                    ltlPriceBandLabel.Text = TEBUtilities.CheckForDBNull_String(_wfr.Content("ltlPriceBandLabelBP", _languageCode, True))
                    ltlPriceBand.Text = TEBUtilities.CheckForDBNull_String(dtParentProduct.Rows(0)("PriceBand")).Trim() &
                        "(" & TEBUtilities.CheckForDBNull_String(dtParentProduct.Rows(0)("PriceBandDesc")).Trim() & ")"
                    ltlPackageId.Text = _bulkID
                Case Is = ComponentType.None
                    ltlPackageIdLabel.Text = _wfr.Content("PackageIdLabel", _languageCode, True)
            End Select


            ltlPaymentReferenceLabel.Text = _wfr.Content("PaymentReferenceLabel", _languageCode, True)
            ltlPackageBreakdown.Text = _wfr.Content("PackageBreakdownLabel", _languageCode, True)

            ltlPackageHistory.Text = _wfr.Content("PackageHistoryTitle", _languageCode, True)
            ltlPackageStatusLabel.Text = _wfr.Content("PackageStatusLabel", _languageCode, True)


            SetPackageCATLinks(hlnkCancel, "CancelLinkText", dtStatus, "PackageCancelFlag", GlobalConstants.CATMODE_CANCEL, _allowCancel, dtParentProduct.Rows(0)("ProductDate").ToString)
            SetPackageCATLinks(hlnkAmend, "AmendLinkText", dtStatus, "PackageAmendFlag", GlobalConstants.CATMODE_AMEND, _allowAmend, dtParentProduct.Rows(0)("ProductDate").ToString)
            SetPackageCATLinks(hlnkTransfer, "TransferLinkText", dtStatus, "PackageTransferFlag", GlobalConstants.CATMODE_TRANSFER, _allowTransfer, dtParentProduct.Rows(0)("ProductDate").ToString)
        End If
    End Sub

    ''' <summary>
    ''' Sets the package CAT links.
    ''' </summary>
    ''' <param name="hlnkControl">The HLNK control.</param>
    ''' <param name="wfrContentCode">The WFR content code.</param>
    ''' <param name="dtStatus">The dt status.</param>
    ''' <param name="columnName">Name of the column.</param>
    ''' <param name="catMode">The cat mode.</param>
    ''' <param name="canAllow">if set to <c>true</c> [can allow].</param>
    Private Sub SetPackageCATLinks(ByVal hlnkControl As HyperLink, ByVal wfrContentCode As String, ByVal dtStatus As DataTable, ByVal columnName As String, ByVal catMode As String, ByVal canAllow As Boolean, ByVal productDate As String)
        hlnkControl.Visible = False
        ' Y, N, A
        If canAllow AndAlso _isPayOwnerLoggedIn Then

            If columnName = "PackageCancelFlag" AndAlso CDate(productDate) < DateTime.Now AndAlso Not AgentProfile.AgentPermissions.CanCancelPastDatedTickets Then
                hlnkControl.Visible = False
            Else
                Dim packageCatCheck As String = Utilities.CheckForDBNull_String((dtStatus.Rows(0)(columnName)))
                If packageCatCheck = GlobalConstants.PACKAGE_CAT_ALLOWED OrElse (packageCatCheck = GlobalConstants.PACKAGE_CAT_AGENT_ONLY AndAlso AgentProfile.IsAgent) Then
                    hlnkControl.Visible = True
                    hlnkControl.NavigateUrl = GetCATPackageNavigationLink(dtStatus, catMode)
                    hlnkControl.Text = _wfr.Content(wfrContentCode, _languageCode, True)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Set the text for the column headings
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setColumnHeadings()
        ProductColumnHeading = _wfr.Content("membershipMatchLabel", _languageCode, True)
        StandColumnHeading = _wfr.Content("standLabel", _languageCode, True)
        SeatColumnHeading = _wfr.Content("seatLabel", _languageCode, True)
        DateColumnHeading = _wfr.Content("ProductDateLabel", _languageCode, True)
        CustomerColumnHeading = _wfr.Content("customerNameLabel", _languageCode, True)
        PriceColumnHeading = _wfr.Content("priceLabel", _languageCode, True)
        PriceBandColumnHeading = _wfr.Content("priceBandLabel", _languageCode, True)
        LoyaltyPointsColumnHeading = _wfr.Content("loyaltyLabel", _languageCode, True)
        StatusColumnHeading = _wfr.Content("StatusLabel", _languageCode, True)
        QuantityHeading = _wfr.Content("QuantityHeading", _languageCode, True)
        ComponentHeading = _wfr.Content("ComponentHeading", _languageCode, True)
        ComponentNetPriceHeading = _wfr.Content("ComponentNetPriceHeading", _languageCode, True)
        ComponentDiscountHeading = _wfr.Content("ComponentDiscountHeading", _languageCode, True)
        PriceHeading = _wfr.Content("PriceHeading", _languageCode, True)
        DetailHeading = _wfr.Content("DetailHeading", _languageCode, True)
        PackageHistoryDateColHead = _wfr.Content("PackageHistoryDateLabel", _languageCode, True)
        PackageHistoryDescColHead = _wfr.Content("PackageHistoryDescLabel", _languageCode, True)
        DetailButtonText = _wfr.Content("PackageHistoryDetailButtonText", _languageCode, True)
        RetailProductColumnHeading = _wfr.Content("RetailProductLabel", _languageCode, True)
        RetailDescriptionColumnHeading = _wfr.Content("RetailDescriptionLabel", _languageCode, True)
        TicketNumberHeading = _wfr.Content("TicketNumberLabel", _languageCode, True)
        DespatchDateHeading = _wfr.Content("DespatchDateLabel", _languageCode, True)
        ItemSelectHeading = _wfr.Content("ItemSelectHeaderLabel", _languageCode, True)
        DespatchInformationHeading = _wfr.Content("DespatchInformationLabel", _languageCode, True)
        FulfilmentMethodColumnHeading = _wfr.Content("FulfilmentMethodLabel", _languageCode, True)
    End Sub

    ''' <summary>
    ''' Set the totals labels
    ''' </summary>
    ''' <param name="ticketingData">The ticketing only data table</param>
    ''' <remarks></remarks>
    Private Sub setTotals(ByRef ticketingData As DataView, ByVal dtStatus As DataTable)
        Dim ticketingTotal As Decimal = 0
        Dim retailTotal As Decimal = 0
        plhRetailTotal.Visible = False
        plhTicketTotal.Visible = False

        ' Package paid price calculated back end New MDH
        If _component = ComponentType.CORPORATE AndAlso TEBUtilities.CheckForDBNull_Decimal(dtStatus.Rows(0)("PackagePricePaid")) <> 0 Then
            ltlTicketsTotalLabel.Text = _wfr.Content("PackageTotalLabel", _languageCode, True)
            ticketingTotal = dtStatus.Rows(0)("PackagePricePaid").ToString

            ' Package paid price calculated front end for SS promoted events & old MDH
        ElseIf _component = ComponentType.CORPORATE Then
            ltlTicketsTotalLabel.Text = _wfr.Content("PackageTotalLabel", _languageCode, True)
            For Each row As DataRow In ticketingData.ToTable.Rows
                ticketingTotal += TEBUtilities.CheckForDBNull_Decimal(row("TotalPrice"))
            Next
        Else
            ltlTicketsTotalLabel.Text = _wfr.Content("TicketsTotalLabel", _languageCode, True)
            For Each row As DataRow In ticketingData.ToTable.Rows
                If row("ProductCode") = ModuleDefaults.RetailProductCode Then
                    retailTotal += TEBUtilities.CheckForDBNull_Decimal(row("SalePrice"))
                Else
                    ticketingTotal += TEBUtilities.CheckForDBNull_Decimal(row("SalePrice"))
                End If
            Next
        End If
        If ticketingTotal > 0 Then
            plhTicketTotal.Visible = True
            ltlTicketsTotalValue.Text = FormatCurrency(ticketingTotal)
        End If
        If retailTotal > 0 Then
            ltlRetailTotalLabel.Text = _wfr.Content("RetailTotalLabel", _languageCode, True)
            plhRetailTotal.Visible = True
            ltlRetailTotalValue.Text = FormatCurrency(retailTotal)

            'Display the promotion value
            If _dtRetailHistory.Rows.Count > 0 AndAlso _dtRetailHistory.Rows(0).Item("PROMOTION_VALUE1") > 0 Then
                ltlRetailDiscountLabel.Text = _wfr.Content("RetailDiscountLabel", _languageCode, True)
                ltlRetailGoodsValueLabel.Text = _wfr.Content("RetailGoodsValueLabel", _languageCode, True)
                ltlRetailSummaryTotalLabel.Text = _wfr.Content("RetailTotalLabel", _languageCode, True)
                plhRetailDiscount.Visible = True
                ltlRetailDiscountValue.Text = FormatCurrency(_dtRetailHistory.Rows(0).Item("PROMOTION_VALUE1"))
                ltlRetailGoodsValue.Text = FormatCurrency(_dtRetailHistory.Rows(0).Item("PROMOTION_VALUE1") + retailTotal)
                ltlRetailSummaryTotalValue.Text = FormatCurrency(retailTotal)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Process the printing of a ticket based on the item in the list thats clicked and the print data entity provided, handle errors
    ''' </summary>
    ''' <param name="printEntity">The print data entity</param>
    ''' <remarks></remarks>
    Private Function ProcessPrintTickets(ByVal printEntity As DEPrint) As List(Of String)
        Dim errorList As New List(Of String)

        'assign the common properties
        printEntity.PaymentOwnerName = ltlName.Text
        printEntity.AddressLine1 = ltlAddressLine1.Text
        printEntity.AddressLine2 = ltlAddressLine2.Text
        printEntity.AddressLine3 = ltlAddressLine3.Text
        printEntity.AddressLine4 = ltlAddressLine4.Text
        printEntity.PostCodePart1 = ltlPostCode1.Text
        printEntity.PostCodePart2 = ltlPostCode2.Text

        Dim talPrint As New TalentPrint
        With talPrint
            .PrintEntity = printEntity
            .Settings = TEBUtilities.GetSettingsObject(False)
            .Settings.Cacheing = False
        End With
        Dim err As New ErrorObj
        err = talPrint.PrintTicketsByWeb()
        If Not err.HasError AndAlso talPrint.ResultDataSet IsNot Nothing Then
            If talPrint.ResultDataSet.Tables.Count > 0 Then
                'resultset tables count < 2
                If talPrint.ResultDataSet.Tables.Count = 1 Then
                    Dim errorCode As String = String.Empty
                    If talPrint.ResultDataSet.Tables("ResultStatus").Rows.Count > 0 Then
                        'assign the error message to lblPrintAllMsg
                        errorCode = TEBUtilities.CheckForDBNull_String(talPrint.ResultDataSet.Tables("ResultStatus").Rows(0)("ReturnCode")).Trim
                        If errorCode.Length > 0 OrElse (TEBUtilities.CheckForDBNull_String(talPrint.ResultDataSet.Tables("ResultStatus").Rows(0)("ErrorOccurred")).Trim).Length > 0 Then
                            Dim errMsg As New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
                            errorList.Add(errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, errorCode).ERROR_MESSAGE)
                        End If
                    End If
                End If
            End If
        Else
            'error or resultset is nothing
            errorList.Add(_wfr.Content("PrintErrorMessage", _languageCode, True))
        End If
        Return errorList
    End Function

    ''' <summary>
    ''' Is the loyalty points column empty.
    ''' </summary>
    ''' <param name="dv">Data view to use</param>
    ''' <remarks></remarks>
    Private Sub IsLoyaltyPointEmpty(ByVal dv As DataView)
        Dim LoyaltyPoints As String
        For Each row As DataRow In dv.ToTable().Rows
            LoyaltyPoints = row("LoyaltyPoints").ToString

            If Not String.IsNullOrEmpty(LoyaltyPoints) Then
                _isNull = False
                Exit For
            End If
        Next
    End Sub

    ''' <summary>
    ''' Bind the rptPaymentTrackingNumbers 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetTrackingReferences()
        rptTrackingNumbers.DataSource = _dsTicketingPurchase.Tables("OrderTrackingNumbers")
        rptTrackingNumbers.DataBind()
    End Sub

    ''' <summary>
    ''' Filter child items from order enquiry details
    ''' </summary>
    ''' <param name="OrderEnquiryDetails"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetComponentsOnly(ByVal OrderEnquiryDetails As DataTable) As DataTable
        Dim result As DataTable
        If _component <> ComponentType.None AndAlso _component <> ComponentType.CORPORATE Then
            Dim componentFilter As New DataView(OrderEnquiryDetails)
            If _component = ComponentType.PACKAGE Then
                componentFilter.RowFilter = "IsPackageItem ='False'"
            ElseIf _component = ComponentType.BULK Then
                componentFilter.RowFilter = "BulkId <>'" + _bulkID + "'"
            End If
            result = componentFilter.ToTable
        Else
            result = OrderEnquiryDetails
        End If
        Return result
    End Function

    ''' <summary>
    ''' Filter parent item from order enquiry details
    ''' </summary>
    ''' <param name="OrderEnquiryDetails"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetParentOnly(ByVal OrderEnquiryDetails As DataTable) As DataTable
        Dim result As DataTable
        If _component <> ComponentType.None AndAlso _component <> ComponentType.CORPORATE Then
            Dim componentFilter As New DataView(OrderEnquiryDetails)
            If _component = ComponentType.PACKAGE Then
                componentFilter.RowFilter = "IsPackageItem ='True'"
            ElseIf _component = ComponentType.BULK Then
                componentFilter.RowFilter = "BulkId ='" + _bulkID + "'"
            End If
            result = componentFilter.ToTable
        Else
            result = OrderEnquiryDetails
        End If
        Return result
    End Function

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
        If Profile.IsAnonymous Then
            settings.AccountNo1 = Talent.Common.Utilities.PadLeadingZeros(GlobalConstants.GENERIC_CUSTOMER_NUMBER, 12)
        Else
            settings.AccountNo1 = TCUtilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)
        End If

        order.Dep.PaymentReference = TCUtilities.PadLeadingZeros(String.Empty, 15)
        order.Dep.CustNumberPayRefShouldMatch = False
        settings.Cacheing = False
        If Talent.eCommerce.Utilities.IsAgent Then
            settings.OriginatingSource = AgentProfile.Name
        Else
            If CInt(order.Dep.PaymentReference) <= 0 Then
                settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_wfr.Attribute("CacheTimeMinutes"))
                settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
                settings.AuthorityUserProfile = ModuleDefaults.AuthorityUserProfile
            End If
        End If
        order.Dep.PackageKey = If(Not String.IsNullOrEmpty(_packageKey), _packageKey, String.Empty)
        order.Dep.BulkSalesFlag = If(Not String.IsNullOrEmpty(Request.QueryString("bulkid")), True, False)
        order.Dep.LastRecordNumber = 0
        order.Dep.ShowDespatchInformation = _showDespatchInformation
        order.Dep.TotalRecords = 0
        order.Dep.PaymentReference = PaymentReference
        order.Dep.BulkSalesID = If(Not String.IsNullOrEmpty(Request.QueryString("bulkid")), Request.QueryString("bulkid"), Nothing)
        order.Dep.CallId = _callId
        order.Settings() = settings
        order.Dep.ProductCode = _productCode
        order.Dep.IsCorporateLinkedHomeGame = _isCorporateLinkedHomeGame
        err = order.OrderEnquiryDetails()
        If Not err.HasError AndAlso order.ResultDataSet.Tables.Count = 9 AndAlso order.ResultDataSet.Tables("StatusResults").Rows.Count > 0 _
            AndAlso Not order.ResultDataSet.Tables("StatusResults").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG _
            AndAlso (order.ResultDataSet.Tables("OrderEnquiryDetails").Rows.Count > 0 _
                     OrElse order.ResultDataSet.Tables("PackageDetail").Rows.Count > 0) Then
            Return order.ResultDataSet
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Sort the package by component type and total price to a data view based on the data table
    ''' </summary>
    ''' <param name="dtPackageDetail">The given package data table</param>
    ''' <returns>A sorted data view</returns>
    ''' <remarks></remarks>
    Private Function SortPackage(ByVal dtPackageDetail As DataTable) As DataView
        Dim dvPackageDetail As New DataView(dtPackageDetail)
        dvPackageDetail.Sort = "ComponentType DESC, TotalPrice DESC"
        Return dvPackageDetail
    End Function

    ''' <summary>
    ''' Split the fees and ticketing information from the data table containing both
    ''' </summary>
    ''' <param name="ticketingData">Ticketing data table to work with</param>
    ''' <param name="dvFeesData">The fees table created from the ticketing data table</param>
    ''' <returns>The tickets only ticketing data table</returns>
    ''' <remarks></remarks>
    Private Function removeFees(ByRef ticketingData As DataTable, ByRef dvFeesData As DataView) As DataView
        Dim dvTicketsOnlyData As New DataView(ticketingData)
        If _component <> ComponentType.CORPORATE Then
            dvTicketsOnlyData.RowFilter = "FeesCode = ''"
            dvFeesData = New DataView(ticketingData)
            dvFeesData.RowFilter = "FeesCode <> ''"

            'Add the retail delivery charge to the fees table
            If _dtRetailHistory.Rows.Count > 0 AndAlso _dtRetailHistory.Rows(0).Item("TOTAL_DELIVERY_GROSS") > 0 Then
                Dim drRetailDelivery As DataRowView = dvFeesData.AddNew()
                drRetailDelivery("ProductDescription") = _dtRetailHistory.Rows(0).Item("DELIVERY_TYPE_DESCRIPTION")
                drRetailDelivery("SalePrice") = _dtRetailHistory.Rows(0).Item("TOTAL_DELIVERY_GROSS")
                drRetailDelivery("StatusCode") = "SOLD"
                'Needed as there is a filter on this field
                drRetailDelivery("FeesCode") = "XXXXXX"
                drRetailDelivery.EndEdit()
            End If
        End If
        If (ticketingData.TableName = "OrderEnquiryDetails") Then
            ' Summarise ONACCT fees if have more than one
            Dim dvONACCT As New DataView(ticketingData)
            dvONACCT.RowFilter = "FeesCode = 'ONACCT'"
            If dvONACCT.Count > 1 Then
                Dim ONACCTvalue As Decimal = 0
                Dim lastRecord As Integer = dvONACCT.Count
                Dim currentRecord As Integer = 0
                Dim FeeStatus As String
                For Each drv As System.Data.DataRowView In dvONACCT
                    currentRecord = currentRecord + 1

                    If drv("StatusCode").ToString() = "CANCEL" Then
                        ONACCTvalue = ONACCTvalue + drv("SalePrice")
                    Else
                        ONACCTvalue = ONACCTvalue - drv("SalePrice")
                    End If

                    If currentRecord = lastRecord Then
                        If ONACCTvalue > 0 Then
                            ONACCTvalue = ONACCTvalue * -1
                            FeeStatus = "SOLD"
                        Else
                            FeeStatus = "CANCEL"
                        End If

                        drv("StatusCode") = FeeStatus
                        drv("SalePrice") = ONACCTvalue
                    Else
                        drv.Delete()
                    End If

                Next
            End If
        End If
        Return dvTicketsOnlyData
    End Function

    ''' <summary>
    ''' Removes the products that are part of a bundle, but not the bundle products themselves from the data table for this current payment ref.
    ''' Creates a dataset of tables, each table is a list of products in each bundle. There could be a number of bundles, hence the dataset.
    ''' </summary>
    ''' <param name="ticketingData">The ticketing and bundle products</param>
    ''' <returns>The amended ticketing only data view</returns>
    ''' <remarks></remarks>
    Private Function removeBundleProducts(ByVal ticketingData As DataTable) As DataView
        Dim ticketingDataNoBundleProducts As New DataView(ticketingData)
        ticketingDataNoBundleProducts.RowFilter = "RelatingBundleProduct = ''"
        If ticketingData.Rows.Count > ticketingDataNoBundleProducts.Count Then
            Dim dvOnlyProductsInABundle As New DataView(ticketingData)
            dvOnlyProductsInABundle.RowFilter = "RelatingBundleProduct <> ''"
            _dsProductsInABundle = New DataSet
            For Each row As DataRow In dvOnlyProductsInABundle.ToTable.Rows
                Dim seat As DESeatDetails = row("RelatingBundleSeat")
                Dim tableName As New StringBuilder
                tableName.Append(row("RelatingBundleProduct")).Append("-")
                tableName.Append(seat.Stand)
                tableName.Append(seat.Area)
                tableName.Append(seat.Row)
                tableName.Append(seat.Seat).Append(seat.AlphaSuffix)
                If Not _dsProductsInABundle.Tables.Contains(tableName.ToString()) Then
                    Dim dtCopy As DataTable = ticketingData.Clone()
                    dtCopy.TableName = tableName.ToString()
                    _dsProductsInABundle.Tables.Add(dtCopy)
                End If
                _dsProductsInABundle.Tables(tableName.ToString()).ImportRow(row)
            Next
        End If
        Return ticketingDataNoBundleProducts
    End Function

    ''' <summary>
    ''' Get the cat navigation link based on mode and status
    ''' </summary>
    ''' <param name="dtStatus">The status data table</param>
    ''' <param name="catMode">The given cat mode</param>
    ''' <returns>Formatted link</returns>
    ''' <remarks></remarks>
    Private Function GetCATPackageNavigationLink(ByVal dtStatus As DataTable, ByVal catMode As String) As String
        Dim navigateURL As New StringBuilder
        'pay ref, seat, match code, customer number, mode, product type, stadium code
        Dim customerNumber As String = String.Empty
        If Not Profile.IsAnonymous Then
            customerNumber = Profile.User.Details.LoginID.PadLeft(12, "0")
        End If
        If Profile.Basket.BasketItems.Count > 0 Then
            navigateURL.Append("~/PagesLogin/Orders/CATConfirm.aspx?")
            navigateURL.Append("TrnEnq=").Append(_isTransactionEnquiry.ToString)
            navigateURL.Append("&PayRef=").Append(TEBUtilities.CheckForDBNull_String(PaymentReference))
            navigateURL.Append("&MatchCode=").Append(TEBUtilities.CheckForDBNull_String(dtStatus.Rows(0)("ProductCode")))
            navigateURL.Append("&CustomerNumber=").Append(GetFormattedCustomerNumber(customerNumber))
            navigateURL.Append("&ProductType=").Append(TEBUtilities.CheckForDBNull_String(dtStatus.Rows(0)("ProductType")))
            navigateURL.Append("&StadiumCode=").Append(TEBUtilities.CheckForDBNull_String(dtStatus.Rows(0)("StadiumCode")))
            navigateURL.Append("&CallId=").Append(TEBUtilities.CheckForDBNull_String(_callId))
            navigateURL.Append("&Mode=").Append(catMode)
        Else
            If catMode <> GlobalConstants.CATMODE_TRANSFER Then
                navigateURL.Append("~/Redirect/TicketingGateway.aspx")
                navigateURL.Append("?page=catconfirm.aspx")
                navigateURL.Append("&function=packageaddtobasket")
                navigateURL.Append("&catmode=").Append(catMode)
                navigateURL.Append("&product=").Append(TEBUtilities.CheckForDBNull_String(dtStatus.Rows(0)("ProductCode")))
                navigateURL.Append("&payref=").Append(TEBUtilities.CheckForDBNull_String(PaymentReference))
                navigateURL.Append("&istrnxenq=").Append(_isTransactionEnquiry.ToString)
                navigateURL.Append("&callid=").Append(TEBUtilities.CheckForDBNull_String(_callId))
                navigateURL.Append("&catseatcustomerno=").Append(GetFormattedCustomerNumber(customerNumber))
            Else
                navigateURL.Append(GetTransferSeatSelectionURL(TEBUtilities.CheckForDBNull_String(dtStatus.Rows(0)("StadiumCode")),
                                                          TEBUtilities.CheckForDBNull_String(dtStatus.Rows(0)("ProductCode")), String.Empty,
                                                          TEBUtilities.CheckForDBNull_String(dtStatus.Rows(0)("ProductType")),
                                                          TEBUtilities.CheckForDBNull_String(dtStatus.Rows(0)("ProductSubType")), String.Empty))
                navigateURL.Append("&catmode=").Append(catMode)
                navigateURL.Append("&payref=").Append(PaymentReference)
                navigateURL.Append("&istrnxenq=").Append(_isTransactionEnquiry.ToString)
                navigateURL.Append("&catseatcustomerno=").Append(GetFormattedCustomerNumber(customerNumber))
                navigateURL.Append("&callid=").Append(TEBUtilities.CheckForDBNull_String(_callId))
            End If
        End If
        Return navigateURL.ToString()
    End Function

    ''' <summary>
    ''' Get the navigation link for the CAT requested based on the order details and the CAT mode
    ''' </summary>
    ''' <param name="orderDetailRow">The order details row</param>
    ''' <param name="catMode">The CAT mode</param>
    ''' <returns>The navigation link</returns>
    ''' <remarks></remarks>
    Private Function GetCATNavigationLink(ByVal orderDetailRow As DataRow, ByVal catMode As String) As String
        Dim navigateURL As New StringBuilder

        'CAT confiorm page needs to be displayed to the agent either when:
        ' - We are a bulk agent and attempting to cancel individual items. In the is case we need to inform the agent that we will temporarily be taken out of bulk mode. 
        ' - We have items in our basket already. In the is case we need to inform the agent that the basket will be cleared.
        If Profile.Basket.BasketItems.Count > 0 OrElse (AgentProfile.BulkSalesMode AndAlso catMode = GlobalConstants.CATMODE_CANCELMULTIPLE) Then
            navigateURL.Append("~/PagesLogin/Orders/CATConfirm.aspx?")
            navigateURL.Append("TrnEnq=").Append(_isTransactionEnquiry.ToString)
            navigateURL.Append("&PayRef=").Append(TEBUtilities.CheckForDBNull_String(orderDetailRow("PaymentReference")))
            navigateURL.Append("&Mode=").Append(catMode)
            If TEBUtilities.CheckForDBNull_Int(orderDetailRow("BulkID")) <> 0 Then
                navigateURL.Append("&bulksalesid=").Append(TEBUtilities.CheckForDBNull_Int(orderDetailRow("BulkID")))
            Else
                navigateURL.Append("&bulksalesid=").Append(_bulkID)
            End If
            If TEBUtilities.CheckForDBNull_Int(orderDetailRow("CallID")) <> 0 Then
                navigateURL.Append("&CallId=").Append(TEBUtilities.CheckForDBNull_Int(orderDetailRow("CallID")))
            Else
                navigateURL.Append("&CallId=").Append(_callId)
            End If
            navigateURL.Append("&MatchCode=").Append(TEBUtilities.CheckForDBNull_String(orderDetailRow("ProductCode")))

            ' Seat Details not required when cancelling multiple
            If catMode <> GlobalConstants.CATMODE_CANCELMULTIPLE Then
                navigateURL.Append("&Seat=").Append(TEBUtilities.CheckForDBNull_String(orderDetailRow("Seat")))
                navigateURL.Append("&ProductType=").Append(TEBUtilities.CheckForDBNull_String(orderDetailRow("ProductType")))
                navigateURL.Append("&StadiumCode=").Append(TEBUtilities.CheckForDBNull_String(orderDetailRow("StadiumCode")))
                navigateURL.Append("&CustomerNumber=").Append(GetFormattedCustomerNumber(TEBUtilities.CheckForDBNull_String(orderDetailRow("CustomerNumber"))))
            End If
        Else
            If catMode <> GlobalConstants.CATMODE_TRANSFER Then
                navigateURL.Append("~/Redirect/TicketingGateway.aspx")
                navigateURL.Append("?page=catconfirm.aspx")
                If TEBUtilities.CheckForDBNull_Int(orderDetailRow("CallID")) <> 0 Then
                    navigateURL.Append("&function=packageaddtobasket")
                Else
                    navigateURL.Append("&function=addtobasket")
                End If
                navigateURL.Append("&istrnxenq=").Append(_isTransactionEnquiry.ToString)
                navigateURL.Append("&catseatcustomerno=").Append(GetFormattedCustomerNumber(TEBUtilities.CheckForDBNull_String(orderDetailRow("CustomerNumber"))))
                navigateURL.Append("&catmode=").Append(catMode)
                navigateURL.Append("&product=").Append(TEBUtilities.CheckForDBNull_String(orderDetailRow("ProductCode")))
                navigateURL.Append("&catseat=").Append(TEBUtilities.CheckForDBNull_String(orderDetailRow("Seat")))
                navigateURL.Append("&payref=").Append(TEBUtilities.CheckForDBNull_String(orderDetailRow("PaymentReference")))
                If TEBUtilities.CheckForDBNull_Int(orderDetailRow("BulkID")) <> 0 Then
                    navigateURL.Append("&bulksalesid=").Append(TEBUtilities.CheckForDBNull_Int(orderDetailRow("BulkID")))
                Else
                    navigateURL.Append("&bulksalesid=").Append(_bulkID)
                End If
                If TEBUtilities.CheckForDBNull_Int(orderDetailRow("CallID")) <> 0 Then
                    navigateURL.Append("&CallId=").Append(TEBUtilities.CheckForDBNull_Int(orderDetailRow("CallID")))
                Else
                    navigateURL.Append("&CallId=").Append(_callId)
                End If
            Else
                navigateURL.Append(GetTransferSeatSelectionURL(TEBUtilities.CheckForDBNull_String(orderDetailRow("StadiumCode")),
                                                          TEBUtilities.CheckForDBNull_String(orderDetailRow("ProductCode")), String.Empty,
                                                          TEBUtilities.CheckForDBNull_String(orderDetailRow("ProductType")),
                                                          String.Empty, String.Empty))
                navigateURL.Append("&catmode=").Append(HttpUtility.UrlEncode(catMode))
                navigateURL.Append("&catseat=").Append(HttpUtility.UrlEncode(TEBUtilities.CheckForDBNull_String(orderDetailRow("Seat"))))
                navigateURL.Append("&payref=").Append(TEBUtilities.CheckForDBNull_String(orderDetailRow("PaymentReference")))
                navigateURL.Append("&istrnxenq=").Append(_isTransactionEnquiry.ToString)
                navigateURL.Append("&catseatcustomerno=").Append(GetFormattedCustomerNumber(TEBUtilities.CheckForDBNull_String(orderDetailRow("CustomerNumber"))))
            End If
        End If
        Return navigateURL.ToString()
    End Function

    ''' <summary>
    ''' Get the correctly formatted seat transfer URL based on the details provided
    ''' </summary>
    ''' <param name="stadiumCode">The given stadium code</param>
    ''' <param name="productCode">The given product code</param>
    ''' <param name="campaignCode">The given campaign code</param>
    ''' <param name="productType">The given product type</param>
    ''' <param name="productSubType">The given product sub type</param>
    ''' <param name="productHomeAsAway">The home as away flag</param>
    ''' <returns>Correctly formatted destination URL</returns>
    ''' <remarks></remarks>
    Private Function GetTransferSeatSelectionURL(ByVal stadiumCode As String, ByVal productCode As String, ByVal campaignCode As String,
                                               ByVal productType As String, ByVal productSubType As String, ByVal productHomeAsAway As String) As String

        Dim redirectUrl As String = GetCATTransferSeatSelectionURL(TDataObjects.StadiumSettings.TblStadiums.GetStadiumNameByStadiumCode(stadiumCode, BusinessUnit),
                                                                   stadiumCode, productCode, campaignCode, productType, productSubType, productHomeAsAway)
        Return ResolveUrl(redirectUrl)
    End Function

    ''' <summary>
    ''' Check the current customer number is not generic
    ''' </summary>
    ''' <param name="customerNumber">The given customer number</param>
    ''' <returns>Formatted customer number if the customer is generic</returns>
    ''' <remarks></remarks>
    Private Function GetFormattedCustomerNumber(ByVal customerNumber As String) As String
        If Not String.IsNullOrWhiteSpace(customerNumber) Then
            If customerNumber.ToUpper = "*GENERIC" Then
                customerNumber = GlobalConstants.GENERIC_CUSTOMER_NUMBER
            End If
        End If
        Return customerNumber
    End Function

    ''' <summary>
    ''' Check the given datatable has cancelled order or not
    ''' </summary>
    ''' <param name="dtOrderEnquiryDetails">The given datatable</param>
    ''' <returns>Flag that indicates whether there is cancelled order or not</returns>
    ''' <remarks></remarks>
    Private Function HasCancelledOrder(ByVal dtOrderEnquiryDetails As DataTable) As Boolean
        Dim cancelled As Boolean = False
        If dtOrderEnquiryDetails IsNot Nothing AndAlso dtOrderEnquiryDetails.Rows.Count > 0 Then
            For Each dr As DataRow In dtOrderEnquiryDetails.Rows
                If dr("StatusCode") = "CANCEL" Then
                    cancelled = True
                    Exit For
                End If
            Next
        End If
        Return cancelled
    End Function
    Private Function GetComponentType() As ComponentType
        If IsNumeric(_callId.Trim) AndAlso CLng(_callId) > 0 Then
            Return ComponentType.CORPORATE
        ElseIf IsNumeric(_bulkID.Trim) AndAlso CLng(_bulkID) > 0 Then
            Return ComponentType.BULK
        ElseIf _packageKey IsNot Nothing AndAlso Not String.IsNullOrEmpty(_packageKey.Trim) Then
            Return ComponentType.PACKAGE
        Else
            Return ComponentType.None
        End If
    End Function
#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Get the seat history page Url including the values given as querystring parameters
    ''' </summary>
    ''' <param name="productCode">The product code</param>
    ''' <param name="seat">The full seat string</param>
    ''' <returns>The full seat history page Url</returns>
    ''' <remarks></remarks>
    Public Function GetSeatHistoryUrl(ByVal productCode As String, ByVal seat As String) As String
        Dim navigateUrl As New StringBuilder
        navigateUrl.Append(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority))

        If Not Request.ApplicationPath = "/" Then
            navigateUrl.Append(Request.ApplicationPath)
        End If

        navigateUrl.Append("/PagesAgent/Orders/SeatHistory.aspx?product=").Append(productCode)
        navigateUrl.Append("&seat=").Append(Server.UrlEncode(seat))
        navigateUrl.Append("&paymentref=").Append(PaymentReference)

        Return navigateUrl.ToString
    End Function

    ''' <summary>
    ''' Get the seat print history page Url including the values given as querystring parameters
    ''' </summary>
    ''' <param name="productCode">The product code</param>
    ''' <param name="seat">The full seat string</param>
    ''' <returns>The full seat print history page Url</returns>
    ''' <remarks></remarks>
    Public Function GetSeatPrintHistoryUrl(ByVal productCode As String, ByVal seat As String) As String
        Dim navigateUrl As New StringBuilder
        navigateUrl.Append(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority))

        If Not Request.ApplicationPath = "/" Then
            navigateUrl.Append(Request.ApplicationPath)
        End If

        navigateUrl.Append("/PagesAgent/Orders/SeatPrintHistory.aspx?product=").Append(productCode)
        navigateUrl.Append("&seat=").Append(Server.UrlEncode(seat))
        Return navigateUrl.ToString
    End Function

    ''' <summary>
    ''' Get the seat details for the component when sold in bulk mode.
    ''' </summary>
    ''' <param name="componentID">The component ID</param>
    ''' <returns>The package seat details url for this component/call id</returns>
    ''' <remarks></remarks>
    Public Function GetComponentBulkSeatURL(componentID As String) As String
        Dim navigateUrl As New StringBuilder
        navigateUrl.Append("~/PagesPublic/ProductBrowse/PackageSeatDetails.aspx?ComponentID=")
        navigateUrl.Append(componentID)
        navigateUrl.Append("&callID=")
        navigateUrl.Append(_callId)
        Return navigateUrl.ToString()
    End Function

    ''' <summary>
    ''' Get the soft-coded status text from the database
    ''' </summary>
    ''' <param name="PValue">The hardcoded text value</param>
    ''' <returns>The soft coded value</returns>
    ''' <remarks></remarks>
    Public Function GetStatusText(ByVal PValue As String) As String
        Dim str As String = _wfr.Content(PValue, _languageCode, True)
        If String.IsNullOrEmpty(str) Then
            str = PValue
        End If
        Return str
    End Function

    ''' <summary>
    ''' Check to see if the promotion id is present
    ''' </summary>
    ''' <param name="dataItem">The current repeater data item row</param>
    ''' <returns>Return true if the promotion id is present</returns>
    ''' <remarks></remarks>
    Public Function ShowPromotion(ByVal dataItem As DataRowView) As Boolean
        Dim hasPromotion As Boolean = False
        If TEBUtilities.CheckForDBNull_Int(dataItem("PromotionID1")) > 0 Then hasPromotion = True
        If TEBUtilities.CheckForDBNull_Int(dataItem("PromotionID2")) > 0 Then hasPromotion = True
        If TEBUtilities.CheckForDBNull_Int(dataItem("PromotionID3")) > 0 Then hasPromotion = True
        Return hasPromotion
    End Function

    ''' <summary>
    ''' Check to see if the promotion id is present
    ''' </summary>
    ''' <param name="dataItem">The current repeater data item row</param>
    ''' <returns>Return true if the promotion id is present</returns>
    ''' <remarks></remarks>
    Public Function ShowComponentSeatPrices(ByVal dataItem As DataRowView) As Boolean
        Return _showComponentSeatPrices
    End Function

    ''' <summary>
    ''' Append the URL, promotion id, original price and sale price together
    ''' </summary>
    ''' <param name="dataItem">The current repeater data item row</param>
    ''' <returns>a full url as a string</returns>
    ''' <remarks></remarks>
    Public Function GetNavigateUrl(ByVal dataItem As DataRowView) As String
        Dim promotionDetailsUrl As New StringBuilder
        promotionDetailsUrl.Append("~/PagesPublic/Basket/PromotionDetails.aspx?originalprice=")
        promotionDetailsUrl.Append(TEBUtilities.CheckForDBNull_Decimal(dataItem("OriginalPrice").ToString()))
        promotionDetailsUrl.Append("&saleprice=")
        promotionDetailsUrl.Append(TEBUtilities.CheckForDBNull_Decimal(dataItem("SalePrice").ToString()))
        If TEBUtilities.CheckForDBNull_Int(dataItem("PromotionID1")) > 0 Then
            promotionDetailsUrl.Append("&promotionid1=")
            promotionDetailsUrl.Append(TEBUtilities.CheckForDBNull_String(dataItem("PromotionID1")))
        End If
        If TEBUtilities.CheckForDBNull_Int(dataItem("PromotionID2")) > 0 Then
            promotionDetailsUrl.Append("&promotionid2=")
            promotionDetailsUrl.Append(TEBUtilities.CheckForDBNull_String(dataItem("PromotionID2")))
        End If
        If TEBUtilities.CheckForDBNull_Int(dataItem("PromotionID3")) > 0 Then
            promotionDetailsUrl.Append("&promotionid3=")
            promotionDetailsUrl.Append(TEBUtilities.CheckForDBNull_String(dataItem("PromotionID3")))
        End If
        promotionDetailsUrl.Append("&returnurl=")
        promotionDetailsUrl.Append(Request.RawUrl)
        Return promotionDetailsUrl.ToString()
    End Function

    ''' <summary>
    ''' Get the unique key used to show the correct products in the specified bundle
    ''' </summary>
    ''' <param name="productCode">The bundle product code</param>
    ''' <param name="seat">The bundle seat information</param>
    ''' <returns>The unique key for the bundle product</returns>
    ''' <remarks></remarks>
    Public Function GetBundleKey(ByRef productCode As String, ByRef seat As String) As String
        Dim currentBundleKey As String = String.Empty
        If seat.Length > 0 Then
            Dim bundleKey As New StringBuilder
            Dim seatDetails As New DESeatDetails
            seatDetails.FormattedSeat = seat
            bundleKey.Append(productCode).Append("-")
            bundleKey.Append(seatDetails.Stand.Trim())
            bundleKey.Append(seatDetails.Area.Trim())
            bundleKey.Append(seatDetails.Row.Trim())
            bundleKey.Append(seatDetails.Seat.Trim())
            bundleKey.Append(seatDetails.AlphaSuffix.Trim())
            currentBundleKey = bundleKey.ToString()
        End If
        Return currentBundleKey
    End Function

    ''' <summary>
    ''' Get the current bundle key based on the table name
    ''' </summary>
    ''' <param name="dataItem">The table</param>
    ''' <returns>The bundle key string</returns>
    ''' <remarks></remarks>
    Public Function GetCurrentBundleKey(ByRef dataItem As Object) As String
        Dim bundleKey As String = String.Empty
        If dataItem IsNot Nothing Then
            bundleKey = CType(dataItem, DataTable).TableName
        End If
        Return bundleKey
    End Function

    ''' <summary>
    ''' Returns the Package price
    ''' </summary>
    ''' <param name="Price">The total price of package</param>
    ''' <param name="IsNegative">If the price is negative, the value will be True</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetPrice(ByVal Price As Decimal, ByVal IsNegative As Boolean) As String
        Dim retValue As String = String.Empty
        Dim Minus As String = "-"
        If IsNegative Then
            retValue = String.Format("{0}{1}", Minus, FormatCurrency(Price))
        Else
            retValue = FormatCurrency(Price)
        End If
        Return retValue
    End Function

    ''' <summary>
    ''' Checks the seat and if it is roving or unreserved then replaced the text
    ''' If the return value is empty, we need to show it in the table as a non-breaking space as this affects the layout of the table (especially for mobile devices)
    ''' </summary>
    ''' <param name="seatDetails">the given seat details passed from WS005R</param>
    ''' <param name="roving">IF the seat is in a roving area</param>
    ''' <param name="unreserved">IF the seat is in an unreserved area</param>
    ''' <param name="productType">The given product type</param>
    ''' <param name="allocatedSeat">The allocated seat details</param>
    ''' <returns>The seat if it is a valid seat, or strings to represent roving or unreserved</returns>
    ''' <remarks></remarks>
    Protected Function FormatSeat(ByVal seatDetails As String, ByVal roving As Boolean, ByVal unreserved As Boolean, ByVal productType As String, ByVal allocatedSeat As String) As String
        Dim returnValue As String = seatDetails
        If roving Then
            returnValue = _wfr.Content("rovingAreaText", _languageCode, True)
        ElseIf unreserved Then
            returnValue = _wfr.Content("unreservedAreaText", _languageCode, True)
        Else
            returnValue = TEBUtilities.GetFormattedSeatForDisplay(seatDetails, productType, allocatedSeat)
        End If
        If String.IsNullOrEmpty(returnValue) Then returnValue = "&nbsp;"
        Return returnValue
    End Function

    ''' <summary>
    ''' Check the given status code for the fee to see if it's been cancelled and apend a string to the value if it has
    ''' </summary>
    ''' <param name="SalePrice">The fee price</param>
    ''' <param name="StatusCode">The status code</param>
    ''' <returns>Formatted string with price and cancelled label if required</returns>
    ''' <remarks></remarks>
    Public Function IsFeeCancelled(ByVal SalePrice As Decimal, ByVal StatusCode As String) As String
        Dim formattedString As String = FormatCurrency(SalePrice).ToString().Trim()
        If StatusCode = "CANCEL" Then
            formattedString = formattedString & _wfr.Content("CancelledFeeText", _languageCode, True)
        End If
        Return formattedString
    End Function

    Public Function GetFulfilmentTextFromCodes(ByVal fulfilmentCode As String) As String
        Select Case fulfilmentCode
            Case Is = GlobalConstants.POST_FULFILMENT : Return _wfr.Content("PostFulfilmentText", _languageCode, True)
            Case Is = GlobalConstants.REG_POST_FULFILMENT : Return _wfr.Content("RegisteredPostFulfilmentText", _languageCode, True)
            Case Is = GlobalConstants.PRINT_FULFILMENT : Return _wfr.Content("PrintFulfilmentText", _languageCode, True)
            Case Is = GlobalConstants.COLLECT_FULFILMENT : Return _wfr.Content("CollectFulfilmentText", _languageCode, True)
            Case Is = GlobalConstants.PRINT_AT_HOME_FULFILMENT : Return _wfr.Content("PrintAtHomeFulfilmentText", _languageCode, True)
            Case Is = GlobalConstants.SMARTCARD_UPLOAD_FULFILMENT : Return _wfr.Content("SmartcardUploadFulfilmentText", _languageCode, True)
            Case Else : Return _wfr.Content("NotApplicableFulfilmentText", _languageCode, True)
        End Select
    End Function

    ''' <summary>
    ''' Format the currency for the given value
    ''' </summary>
    ''' <param name="value">The valye amount</param>
    ''' <returns>The formatted value string</returns>
    ''' <remarks></remarks>
    Public Function FormatCurrency(ByVal value As Decimal) As String
        Dim formattedString As String = value
        formattedString = TDataObjects.PaymentSettings.FormatCurrency(value, _wfr.BusinessUnit, _wfr.PartnerCode)
        Return formattedString
    End Function

#End Region

End Class