Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TCUtilities = Talent.Common.Utilities
Imports System.Xml
Imports System.Globalization
Imports System.Threading

Partial Class UserControls_TransactionEnquiry
    Inherits ControlBase

    Private _businessUnit As String = String.Empty
    Private _partnerCode As String = String.Empty
    Private _languageCode As String = String.Empty
    Private _ucr As Talent.Common.UserControlResource
    Private _dicTrnxSummaryByPayType As Generic.Dictionary(Of String, Decimal) = Nothing
    Private _totalTransactionAmount As Decimal = 0

    Dim ds1 As New DataSet()
    Dim objPds As New PagedDataSource()

    Public Property EndOfDay As Boolean = False
    Public Property TicketCollection As Boolean = False
    Public ViewDespatchDetailsIcon As String

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _ucr = New Talent.Common.UserControlResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _businessUnit = TalentCache.GetBusinessUnit
        _partnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
        With _ucr
            .BusinessUnit = _businessUnit
            .PartnerCode = _partnerCode
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "TransactionEnquiry.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SetText()
        If EndOfDay Then
            Session("ProductDetailsPath") = "EndOfDay"
        Else
            Session("ProductDetailsPath") = "TransactionEnquiry"
        End If
        plhPaymentReferenceFilter.Visible = False   'functionality deprecated
        If Not Page.IsPostBack Then LoadDatesOnFirstPageLoad()
        pnlTransactionEnquiryFilter.Visible = canDisplayTransactionEnquiryFilter()
        If pnlTransactionEnquiryFilter.Visible Then
            plhAgentFilter.Visible = canDisplayAgentFilter()
            plhCustomerFilter.Visible = canDisplayCustomerFilter()
        End If
        pnlTransactionEnquirySearch.Visible = CanDisplayTransactionEnquirySearch()
        plhEnquiryResults.Visible = CanDisplayEnquiryResults()
        If Not Page.IsPostBack AndAlso Not TicketCollection Then
            ResetFieldValuesFromSession()
            BindOrderHistoryView()
        End If
        If Not String.IsNullOrWhiteSpace(_ucr.Attribute("EndOfDayFiltersOutputOnly")) AndAlso
                CType(_ucr.Attribute("EndOfDayFiltersOutputOnly"), Boolean) = True Then
            EndOfDayFilterOutputOnly()
        End If

    End Sub

    Protected Sub filterButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles filterButton.Click
        If Page.IsValid Then
            Dim errorCode As String = String.Empty
            Session("filterPayRef") = Nothing
            txtPaymentReferenceSearch.Text = ""
            lblError.Text = ""
            If TryParseFilterValues(errorCode) Then
                BindOrderHistoryView()
                SetSessionVariables()
            Else
                ProcessErrorMessage(errorCode)
            End If
        End If
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        If Page.IsValid Then
            Session("filterDetailsCustomer") = Nothing
            Session("TicketCollectionMode") = TicketCollection
            If (Not String.IsNullOrWhiteSpace(txtPaymentReferenceSearch.Text)) Then
                Dim callid As String = "0"
                If txtPackageIDSearch.Text.Trim.Length > 0 AndAlso IsNumeric(txtPackageIDSearch.Text.Trim) Then
                    callid = txtPackageIDSearch.Text.Trim
                End If
                If plhCustomerNumberSearch.Visible Then
                    SetSessionVariables()
                    Session("filterPayRef") = txtPaymentReferenceSearch.Text
                    Session("filterPrintCustomer") = txtCustomerNumberSearch.Text
                    Session("filterPackageID") = txtPackageIDSearch.Text
                    Response.Redirect("~/PagesLogin/Orders/PurchaseDetails.aspx?payref=" & txtPaymentReferenceSearch.Text & "&callid=" & callid)
                Else
                    SetSessionVariables()
                    Session("filterPayRef") = txtPaymentReferenceSearch.Text
                    Session("filterPackageID") = txtPackageIDSearch.Text
                    If Profile.IsAnonymous Then
                        Session("filterPrintCustomer") = ""
                    Else
                        Session("filterPrintCustomer") = Talent.Common.Utilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)
                    End If
                    Response.Redirect("~/PagesLogin/Orders/PurchaseDetails.aspx?payref=" & txtPaymentReferenceSearch.Text & "&callid=" & callid)
                End If
            Else
                If Not String.IsNullOrWhiteSpace(txtPackageIDSearch.Text.Trim) Then
                    ProcessPackageIDSearch()
                End If
            End If
        End If
    End Sub

    Protected Sub btnCustomerSelect_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCustomerSelect.Click
        Response.Redirect("~/PagesPublic/Profile/CustomerSelection.aspx?ReturnUrl=" & Server.UrlEncode(Request.Url.PathAndQuery))
    End Sub

    Protected Sub rptTransactionSummary_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptTransactionSummary.ItemDataBound
        Try
            If e.Item.ItemType = ListItemType.Header Then
                TryCast(e.Item.FindControl("ltlTrnxSumPayTypeLabel"), Literal).Text = _ucr.Content("TrnxSummaryPayTypeLabel", _languageCode, True)
                TryCast(e.Item.FindControl("ltlTrnxSumPayAmountLabel"), Literal).Text = _ucr.Content("TrnxSummaryPayAmountLabel", _languageCode, True)
            ElseIf e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                TryCast(e.Item.FindControl("ltlTrnxSumPayTypeValue"), Literal).Text = TEBUtilities.GetPaymentTypeCodeDescription(DirectCast(e.Item.DataItem, Generic.KeyValuePair(Of String, Decimal)).Key, _businessUnit, _partnerCode, _languageCode)
                _totalTransactionAmount += DirectCast(e.Item.DataItem, Generic.KeyValuePair(Of String, Decimal)).Value.ToString
                TryCast(e.Item.FindControl("ltlTrnxSumPayAmountValue"), Literal).Text = FormatCurrency(DirectCast(e.Item.DataItem, Generic.KeyValuePair(Of String, Decimal)).Value)
                Dim hplViewDetails As HyperLink = CType(e.Item.FindControl("hplViewDetails"), HyperLink)
                hplViewDetails.NavigateUrl = "~/PagesLogin/Orders/PurchaseDetails.aspx?payref=" & Talent.eCommerce.Utilities.CheckForDBNull_String(e.Item.DataItem("PaymentReference"))
            ElseIf e.Item.ItemType = ListItemType.Footer Then
                TryCast(e.Item.FindControl("ltlTrnxSumTotalLabel"), Literal).Text = _ucr.Content("ltlTrnxSumTotalLabel", _languageCode, True)
                TryCast(e.Item.FindControl("ltlTrnxSumTotalValue"), Literal).Text = FormatCurrency(_totalTransactionAmount)
            End If
        Catch ex As Exception
        End Try

    End Sub

    Private Sub SetText()
        With _ucr
            PayRefLabel.Text = .Content("PayRefLabel", _languageCode, True)
            rgxPaymentReferenceSearch.ErrorMessage = .Content("InvalidPaymentReferenceError", _languageCode, True)
            rgxCustomerNumberSearch.ErrorMessage = .Content("InvalidCustomerNumberError", _languageCode, True)
            rgxPackageIDSearch.ErrorMessage = .Content("InvalidPackageIDError", _languageCode, True)
            FromDateLabel.Text = .Content("FromDateLabel", _languageCode, True)
            ToDateLabel.Text = .Content("ToDateLabel", _languageCode, True)
            AgentLabel.Text = .Content("AgentLabel", _languageCode, True)
            CustomerLabel.Text = .Content("CustomerLabel", _languageCode, True)
            filterButton.Text = .Content("FilterButtonText", _languageCode, True)
            If EndOfDay Then
                InstructionsLabel.Visible = False
            Else
                If AgentProfile.IsAgent AndAlso Profile.IsAnonymous Then
                    If ddlAgent.SelectedValue.ToUpper() = "ALL" Then
                        InstructionsLabel.Text = .Content("InstructionsText", _languageCode, True) & " " & "All Agents"
                    Else
                        InstructionsLabel.Text = .Content("InstructionsText", _languageCode, True) & " " & AgentProfile.Name
                    End If
                Else
                    InstructionsLabel.Text = .Content("InstructionsText", _languageCode, True) & " " & Profile.User.Details.Account_No_1.TrimStart(GlobalConstants.LEADING_ZEROS)
                End If
            End If
            lblClubCodeLabel.Text = .Content("ClubCodeLabel", _languageCode, True)

            'Search fields (for print tickets)
            ltlTranEnqSearchTitle.Text = .Content("TransactionEnquirySearchTitle", _languageCode, True)
            lblPayRefSearch.Text = .Content("PayRefSearchLabel", _languageCode, True)
            lblCustomerNumberSearch.Text = .Content("CustomerNumberSearchLabel", _languageCode, True)
            btnSearch.Text = .Content("SearchButtonText", _languageCode, True)
            btnCustomerSelect.Text = .Content("SelectCustomerButtonText", _languageCode, True)
            cvdFromDate.ErrorMessage = .Content("InvalidFromDateText", _languageCode, True)
            cvdToDate.ErrorMessage = .Content("InvalidToDateText", _languageCode, True)
            lblPackageIDSearch.Text = .Content("PackageIDSearchLabel", _languageCode, True)

            'Load icon 
            ViewDespatchDetailsIcon = _ucr.Content("ViewDespatchDetailsIcon", _languageCode, True)
        End With
    End Sub

    Private Sub EndOfDayFilterOutputOnly()
        If EndOfDay Then
            lblFromDateValue.Text = FromDate.Text
            FromDate.Visible = False
            lblFromDateValue.Visible = True
            lblToDateValue.Text = ToDate.Text
            cvdFromDate.Visible = False
            ToDate.Visible = False
            lblToDateValue.Visible = True
            lblAgentValue.Text = ddlAgent.SelectedValue
            ddlAgent.Visible = False
            cvdToDate.Visible = False
            lblAgentValue.Visible = True
            filterButton.Visible = False
        End If
    End Sub

    Private Sub SetSessionVariables()
        If ddlClubCode.Visible Then
            Session("TransactionClub") = ddlClubCode.SelectedValue
        End If
        Session("TransactionPayRef") = PaymentReference.Text
        Session("TransactionPackageID") = txtPackageIDSearch.Text
        Session("TransactionFromDate") = FromDate.Text
        Session("TransactionToDate") = ToDate.Text
        If plhAgentFilter.Visible AndAlso ddlAgent.Visible Then
            Session("TransactionAgentName") = ddlAgent.SelectedValue
        End If
        If plhCustomerFilter.Visible AndAlso txtCustomer.Visible Then
            Session("TransactionCustomer") = GetFilterCustomer()
        End If
    End Sub

    Private Sub ResetFieldValuesFromSession()
        Session.Remove("filterDetailsCustomer")
        Session.Remove("CustPayRefShouldMatch")
        If Request.UrlReferrer IsNot Nothing AndAlso Request.UrlReferrer.PathAndQuery.ToLower.Contains("purchasedetails.aspx") Then
            If ddlClubCode.Visible AndAlso Session("TransactionClub") IsNot Nothing Then
                For itemIndex As Integer = 0 To ddlClubCode.Items.Count - 1
                    If ddlClubCode.Items(itemIndex).Value = Session("TransactionClub") Then
                        ddlClubCode.SelectedIndex = itemIndex
                        Exit For
                    End If
                Next
            End If
            If Session("TransactionPayRef") IsNot Nothing Then PaymentReference.Text = Session("TransactionPayRef")
            If Session("TransactionFromDate") IsNot Nothing Then FromDate.Text = Session("TransactionFromDate")
            If Session("TransactionToDate") IsNot Nothing Then ToDate.Text = Session("TransactionToDate")
            If plhAgentFilter.Visible AndAlso ddlAgent.Visible AndAlso Session("TransactionAgentName") IsNot Nothing Then
                ddlAgent.SelectedValue = Session("TransactionAgentName")
            End If
            If plhCustomerFilter.Visible AndAlso txtCustomer.Visible AndAlso Session("TransactionCustomer") IsNot Nothing Then
                txtCustomer.Text = Session("TransactionCustomer")
            End If
            If pnlTransactionEnquirySearch.Visible Then
                Session.Remove("filterPayRef")
                Session.Remove("filterPrintCustomer")
            End If
        End If
        Session.Remove("TicketCollectionMode")
    End Sub

    Private Sub LoadDatesOnFirstPageLoad()
        If Session("TransactionFromDate") IsNot Nothing AndAlso Session("TransactionToDate") IsNot Nothing AndAlso Request.UrlReferrer IsNot Nothing AndAlso Request.UrlReferrer.PathAndQuery.ToLower.Contains("purchasedetails.aspx") Then
        Else
            If EndOfDay Then
                ToDate.Text = Now.ToString("dd/MM/yyyy")
                FromDate.Text = Now.ToString("dd/MM/yyyy")
            Else
                Dim daysBtwDatesOnFirstLoad As Integer = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("DaysBetweenDatesFirstLoad"))
                Dim datesMaxDiff As Integer = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("DatesMaxDifference"))
                If daysBtwDatesOnFirstLoad <= 0 Then
                    daysBtwDatesOnFirstLoad = 30
                ElseIf daysBtwDatesOnFirstLoad > datesMaxDiff Then
                    daysBtwDatesOnFirstLoad = datesMaxDiff
                End If
                'always dd/mm/yyyy
                ToDate.Text = Now.ToString("dd/MM/yyyy")
                FromDate.Text = Now.AddDays(-daysBtwDatesOnFirstLoad).ToString("dd/MM/yyyy")
            End If
        End If
    End Sub

    Private Function canDisplayTransactionEnquiryFilter() As Boolean
        canDisplayTransactionEnquiryFilter = True
        If TicketCollection Then
            canDisplayTransactionEnquiryFilter = False
        End If
        Return canDisplayTransactionEnquiryFilter
    End Function

    Private Function canDisplayAgentFilter() As Boolean
        If AgentProfile.IsAgent AndAlso Not Page.IsPostBack Then
            ddlAgent.Items.Add(New ListItem(AgentProfile.Name, AgentProfile.Name.ToUpper()))
            ddlAgent.Items.Add(New ListItem(_ucr.Content("AllAgents_DisplayText", _languageCode, True), "ALL"))
        End If
        Return (AgentProfile.IsAgent)
    End Function

    Private Function canDisplayCustomerFilter() As Boolean
        canDisplayCustomerFilter = False
        If Not EndOfDay Then
            If AgentProfile.IsAgent AndAlso Profile.IsAnonymous Then
                canDisplayCustomerFilter = True
            End If
        End If
        Return canDisplayCustomerFilter
    End Function

    Private Function CanDisplayTransactionEnquirySearch() As Boolean
        CanDisplayTransactionEnquirySearch = True
        If Profile.IsAnonymous AndAlso TicketCollection Then
            plhCustomerNumberSearch.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("DisplayCustomerSearchField"))
        Else
            plhCustomerNumberSearch.Visible = False
        End If
        If EndOfDay Then
            CanDisplayTransactionEnquirySearch = False
        End If
        Return CanDisplayTransactionEnquirySearch
    End Function

    Private Function CanDisplayEnquiryResults() As Boolean
        If EndOfDay OrElse TicketCollection Then
            CanDisplayEnquiryResults = False
        Else
            CanDisplayEnquiryResults = True
        End If
        Return CanDisplayEnquiryResults
    End Function

    Private Function TryParseFilterValues(ByRef errorCode As String) As Boolean
        'pay ref or from and to date any one of these should always be a filter if both or not there raise error message
        '1. payment reference
        '2. from and to date
        Dim isValid As Boolean = False
        If plhPaymentReferenceFilter.Visible Then
            If String.IsNullOrWhiteSpace(PaymentReference.Text) AndAlso (String.IsNullOrWhiteSpace(FromDate.Text) OrElse String.IsNullOrWhiteSpace(ToDate.Text)) Then
                'both filters are not having values
                isValid = False
                errorCode = "ErrorMissingFilters"
            ElseIf (Not String.IsNullOrWhiteSpace(PaymentReference.Text)) Then
                'pay ref value exists make sure date filters are valid if exists
                If IsValidDateFilters(True) Then
                    isValid = True
                Else
                    isValid = False
                    errorCode = "ErrorDateFilters"
                End If
            ElseIf (String.IsNullOrWhiteSpace(PaymentReference.Text)) Then
                'pay ref not exists so need valid date filters
                If IsValidDateFilters(False) Then
                    isValid = True
                Else
                    isValid = False
                    errorCode = "ErrorDateFilters"
                End If
            End If
        Else
            'pay ref not exists so need valid date filters
            If IsValidDateFilters(False) Then
                isValid = True
            Else
                isValid = False
                errorCode = "ErrorDateFilters"
            End If
        End If

        Return isValid
    End Function

    Private Function IsValidDateFilters(ByVal canAllowEmptyDates As Boolean) As Boolean
        Dim isValid As Boolean = False
        If canAllowEmptyDates AndAlso (String.IsNullOrWhiteSpace(FromDate.Text) AndAlso (String.IsNullOrWhiteSpace(ToDate.Text))) Then
            isValid = True
        ElseIf (Not String.IsNullOrWhiteSpace(FromDate.Text)) AndAlso (Not String.IsNullOrWhiteSpace(ToDate.Text)) Then
            If IsValidDateFormat(FromDate.Text) AndAlso IsValidDateFormat(ToDate.Text) Then
                Dim givenFromDate As New Date(FromDate.Text.Substring(6, 4), FromDate.Text.Substring(3, 2), FromDate.Text.Substring(0, 2))
                Dim givenToDate As New Date(ToDate.Text.Substring(6, 4), ToDate.Text.Substring(3, 2), ToDate.Text.Substring(0, 2))
                If givenFromDate <= givenToDate Then
                    Dim datesMaxDiff As Integer = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("DatesMaxDifference"))
                    If Profile.IsAnonymous Then
                        If givenFromDate.AddDays(datesMaxDiff - 1) >= givenToDate Then
                            isValid = True
                        End If
                    Else
                        isValid = True
                    End If
                End If
            End If
        End If
        Return isValid
    End Function

    Private Function IsValidDateFormat(ByVal dateString As String) As Boolean
        Dim isValid As Boolean = False
        Try
            If dateString.Trim.Length > 0 Then
                Dim givenDateTime As New Date(dateString.Substring(6, 4), dateString.Substring(3, 2), dateString.Substring(0, 2))
                isValid = True
            End If
        Catch ex As Exception
            isValid = False
        End Try
        Return isValid
    End Function

    Private Function GetFormattedDateString(ByVal dateString As String) As String
        Dim formattedDateString As String = String.Empty
        If dateString.Length >= 10 Then
            formattedDateString = dateString.Substring(0, 2) & dateString.Substring(3, 2) & dateString.Substring(6, 4)
        End If
        Return formattedDateString
    End Function

    Private Sub ProcessErrorMessage(ByVal errorCode As String)
        lblError.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(_ucr.Content(errorCode, _languageCode, True))
    End Sub

    Private Sub BindOrderHistoryView()
        Dim order As New Talent.Common.TalentOrder
        Dim settings As New Talent.Common.DESettings
        Dim err As New Talent.Common.ErrorObj
        Dim moduleDefaults As New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues
        def = moduleDefaults.GetDefaults()

        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.BusinessUnit = TalentCache.GetBusinessUnit()
        settings.StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()

        If plhAgentFilter.Visible Then
            'agent mode - generic customer
            If ddlAgent.SelectedValue.ToUpper() = "ALL" Then
                settings.OriginatingSource = ""
            Else
                settings.OriginatingSource = AgentProfile.Name
            End If
        Else
            settings.OriginatingSource = ""
        End If
        settings.AccountNo1 = GetFilterCustomer()
        settings.Cacheing = False
        order.Settings() = settings
        order.Dep.PaymentReference = PaymentReference.Text.Trim
        order.Dep.FromDate = GetFormattedDateString(FromDate.Text.Trim)
        order.Dep.ToDate = GetFormattedDateString(ToDate.Text.Trim)
        err = order.TransactionEnquiryDetails()
        If Not err.HasError AndAlso order.ResultDataSet IsNot Nothing Then
            _dicTrnxSummaryByPayType = New Generic.Dictionary(Of String, Decimal)
            ds1 = order.ResultDataSet()

            Dim dt1 As Data.DataTable = ds1.Tables("TransactionHeaderDetails")
            If Not Page.IsPostBack Then
                If ds1.Tables("ClubCode") IsNot Nothing AndAlso ds1.Tables("ClubCode").Rows.Count > 1 Then
                    PopulateClubCode(ds1.Tables("ClubCode"))
                Else
                    plhClubCode.Visible = False
                End If
            End If
            '-------------------
            ' Check date filters
            '-------------------
            objPds.DataSource = setDSProductDate(dt1)
            objPds.AllowPaging = True
            objPds.PageSize = _ucr.Attribute("ResultsPageSize")
            TransactionHistoryView.DataSource = objPds
            Session("TransactionDetailsObjPds") = objPds
            TransactionHistoryView.DataBind()
            plhEnquiryResults.Visible = (TransactionHistoryView.Items.Count > 0)
            TransactionHistoryView.Visible = plhEnquiryResults.Visible
            DisplayPagerNav(objPds)
            DisplaySummarySection()

            Session("TransactionDetails_dicTrnxSummaryByPayType") = _dicTrnxSummaryByPayType

        End If
        plhEnquiryResults.Visible = (TransactionHistoryView.Items.Count > 0)
    End Sub

    Private Sub DisplaySummarySection()
        If _dicTrnxSummaryByPayType.Count > 0 Then
            If EndOfDay OrElse CType(_ucr.Attribute("DisplayTransactionSectionAsStandard"), Boolean) Then
                plhTransactionSummary.Visible = True
                rptTransactionSummary.Visible = True
                plhNoPaymentDetails.Visible = False
                rptTransactionSummary.DataSource = _dicTrnxSummaryByPayType
                rptTransactionSummary.DataBind()
            Else
                plhTransactionSummary.Visible = False
                rptTransactionSummary.Visible = False
                plhNoPaymentDetails.Visible = False
            End If
        Else
            plhTransactionSummary.Visible = False
            rptTransactionSummary.Visible = False
            plhNoPaymentDetails.Visible = True
            ltlNoPaymentDetails.Text = _ucr.Content("NoPaymentDetailsLabel", _languageCode, True)
        End If
    End Sub

    Private Function setDSProductDate(ByVal DT As DataTable) As DataView

        Dim DTClub As New DataTable
        If ddlClubCode.Visible AndAlso ddlClubCode.SelectedValue <> "All Clubs" Then
            DT.DefaultView.RowFilter = " ClubCode = '" & ddlClubCode.SelectedValue & "'"
            DTClub = DT.DefaultView.ToTable()
        Else
            DTClub = DT
        End If

        Dim DTNew As New DataTable("DateFilteredDetails")
        Dim DC1 As New System.Data.DataColumn()
        Dim tDate As New DateTime()

        Dim toDt As DateTime
        Dim fromDt As DateTime
        Dim sfilter As String
        '

        For Each col As DataColumn In DTClub.Columns
            If col.ColumnName = "Date" Then
                DTNew.Columns.Add(col.ColumnName, Type.GetType("System.DateTime"))
            Else
                DTNew.Columns.Add(col.ColumnName, col.DataType)
            End If
        Next

        Dim myRow As DataRow
        Dim strTempDate As String = String.Empty
        Dim dtTempDate As Date
        For Each rw As DataRow In DTClub.Rows
            myRow = DTNew.NewRow
            For Each col As DataColumn In DTNew.Columns
                If col.ColumnName = "Date" Then
                    strTempDate = rw(col.ColumnName).ToString
                    dtTempDate = SetDateFromString(strTempDate)
                    myRow(col.ColumnName) = dtTempDate
                ElseIf col.ColumnName = "PaymentType" Then
                    myRow(col.ColumnName) = TEBUtilities.GetPaymentTypeCodeDescription(rw(col.ColumnName), _businessUnit, _partnerCode, _languageCode)
                Else
                    myRow(col.ColumnName) = rw(col.ColumnName)
                End If
            Next
            DTNew.Rows.Add(myRow)
            ' Calc totals if in date range
            If checkDates(FromDate.Text, ToDate.Text) AndAlso FromDate.Text <> String.Empty Then
                fromDt = SetDateFromString(FromDate.Text)
                toDt = SetDateFromString(ToDate.Text)
                If rw("Date") >= fromDt AndAlso rw("Date") <= toDt Then
                    AccumulateTrnxSummaryByPayType(rw("PaymentType").ToString, CDec(rw("Value").ToString))
                End If
            Else
                AccumulateTrnxSummaryByPayType(rw("PaymentType").ToString, CDec(rw("Value").ToString))
            End If

        Next

        sfilter = ""

        If checkDates(FromDate.Text, ToDate.Text) AndAlso FromDate.Text <> String.Empty Then
            fromDt = SetDateFromString(FromDate.Text)
            toDt = SetDateFromString(ToDate.Text)
            If toDt < fromDt Then
            Else
                sfilter = "Date >= '" + fromDt + "' and Date <= '" + toDt + "'"
            End If

        End If

        DTNew.DefaultView.Sort = "PaymentReference DESC"

        DTNew.DefaultView.RowFilter = sfilter

        Return DTNew.DefaultView
    End Function

    Private Function GetFilterCustomer() As String
        Dim strFilterCustomer As String = String.Empty
        If Not EndOfDay Then
            If plhCustomerFilter.Visible Then
                strFilterCustomer = txtCustomer.Text.Trim
                If strFilterCustomer.Length = 0 Then
                    strFilterCustomer = ""
                ElseIf IsNumeric(strFilterCustomer) Then
                    If CInt(strFilterCustomer) <= 0 Then
                        strFilterCustomer = GlobalConstants.GENERIC_CUSTOMER_NUMBER
                    Else
                        strFilterCustomer = Talent.Common.Utilities.PadLeadingZeros(strFilterCustomer, 12)
                    End If
                Else
                    strFilterCustomer = ""
                End If
            Else
                If Not Profile.User.Details Is Nothing AndAlso Not Profile.User.Details.Account_No_1 Is Nothing Then
                    strFilterCustomer = Talent.Common.Utilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)
                End If
            End If
        End If
        Return strFilterCustomer
    End Function

    Private Sub AccumulateTrnxSummaryByPayType(ByVal payType As String, ByVal payAmount As Decimal)
        Dim existingAmountForPayType As Decimal = 0
        payType = payType.Trim.ToUpper()
        If _dicTrnxSummaryByPayType.TryGetValue(payType, existingAmountForPayType) Then
            _dicTrnxSummaryByPayType(payType) = _dicTrnxSummaryByPayType(payType) + payAmount
        Else
            _dicTrnxSummaryByPayType.Add(payType, payAmount)
        End If
    End Sub

    Protected Function SetDateFromString(ByVal strDate As String) As Date
        Dim returnDate As Date
        returnDate = New Date(strDate.Substring(6, 4), strDate.Substring(3, 2), strDate.Substring(0, 2))
        Return returnDate
    End Function

    Protected Sub DisplayPagerNav(ByVal pds As PagedDataSource)
        If pds.PageCount > 1 OrElse Nav1Bottom.Text <> String.Empty Then
            Dim startPage As Integer = 0
            Dim loopToPage As Integer = 0
            Dim currentPage As Integer = 0
            plhPagerBottom.Visible = True
            plhPagerTop.Visible = True
            currentPage = pds.CurrentPageIndex + 1

            'If there are more than 10 pages we need to sort out which 10 pages either side
            'of the current page to display in the navigation:
            'E.g. if page = 8 and ther are 15 pages, the navigation should be:
            'First 3 4 5 6 7 8 9 10 11 12 Last
            If pds.PageCount > 10 Then
                If Not pds.CurrentPageIndex = 0 Then
                    For i As Integer = 3 To 0 Step -1
                        If currentPage + i <= pds.PageCount Then
                            startPage = currentPage - (9 - i)
                            If startPage < 0 Then
                                startPage = 0
                            End If
                            Exit For
                        End If
                    Next
                End If
                loopToPage = startPage + 9
            Else
                loopToPage = pds.PageCount
            End If

            If loopToPage = pds.PageCount Then
                If pds.PageCount > 10 Then
                    startPage -= 1
                    loopToPage -= 1
                Else
                    loopToPage -= 1
                End If
            End If


            If pds.PageCount > 10 Then
                FirstTop.Text = _ucr.Content("FirstItemNavigationText", _languageCode, True)
                FirstBottom.Text = _ucr.Content("FirstItemNavigationText", _languageCode, True)
            Else
                FirstTop.Text = String.Empty
                FirstBottom.Text = String.Empty
            End If

            Dim count As Integer = 0
            For i As Integer = startPage To loopToPage
                count += 1
                CType(Me.FindControl("Nav" & count.ToString & "Top"), LinkButton).Text = (i + 1).ToString
                CType(Me.FindControl("Nav" & count.ToString & "Bottom"), LinkButton).Text = (i + 1).ToString

                'Set the font to bold if is the selected page
                'otherwise set it back to normal
                If pds.CurrentPageIndex = i Then
                    CType(Me.FindControl("Nav" & count.ToString & "Top"), LinkButton).Font.Bold = True
                    CType(Me.FindControl("Nav" & count.ToString & "Bottom"), LinkButton).Font.Bold = True
                Else
                    CType(Me.FindControl("Nav" & count.ToString & "Top"), LinkButton).Font.Bold = False
                    CType(Me.FindControl("Nav" & count.ToString & "Bottom"), LinkButton).Font.Bold = False
                End If
            Next

            If pds.PageCount > 10 Then
                LastTop.Text = _ucr.Content("LastItemNavigationText", _languageCode, True)
                LastBottom.Text = _ucr.Content("LastItemNavigationText", _languageCode, True)
            Else
                LastTop.Text = String.Empty
                LastBottom.Text = String.Empty
            End If

            count += 1
            For i As Integer = count To 10
                CType(Me.FindControl("Nav" & i.ToString & "Top"), LinkButton).Text = String.Empty
                CType(Me.FindControl("Nav" & i.ToString & "Bottom"), LinkButton).Text = String.Empty
            Next

            Dim arg0, arg1, arg2 As Integer
            arg0 = pds.PageSize * pds.CurrentPageIndex + 1
            If pds.IsLastPage Then
                arg1 = pds.DataSourceCount
            Else
                arg1 = (arg0 - 1) + pds.PageSize
            End If
            arg2 = pds.DataSourceCount

            CurrentResultsDisplaying.Text = String.Format(_ucr.Content("ResultsCurrentViewText", _languageCode, True), arg0.ToString, arg1.ToString, arg2.ToString)
            If arg2 = 0 Then
                CurrentResultsDisplaying.Text = String.Empty
            End If
            If pds.PageCount = 1 Then
                Nav1Bottom.Text = String.Empty
                Nav1Top.Text = String.Empty
            End If
        Else
            plhPagerBottom.Visible = False
            plhPagerTop.Visible = False
        End If
    End Sub

    Protected Sub ChangePage(ByVal sender As Object, ByVal e As EventArgs)
        If objPds.DataSource Is Nothing Then
            objPds = Session("TransactionDetailsObjPds")
        End If
        If _dicTrnxSummaryByPayType Is Nothing Then
            _dicTrnxSummaryByPayType = Session("TransactionDetails_dicTrnxSummaryByPayType")
        End If


        Dim lb As LinkButton = CType(sender, LinkButton)
        Dim pageIndex As Integer = 0
        Try
            pageIndex = CInt(lb.Text)
            pageIndex -= 1
        Catch ex As Exception
            If lb.Text = _ucr.Content("FirstItemNavigationText", _languageCode, True) Then
                pageIndex = 0
            ElseIf lb.Text = _ucr.Content("LastItemNavigationText", _languageCode, True) Then
                pageIndex = objPds.PageCount - 1
            End If
        End Try

        If objPds Is Nothing OrElse _dicTrnxSummaryByPayType Is Nothing Then
            BindOrderHistoryView()
        Else
            objPds.CurrentPageIndex = pageIndex
            TransactionHistoryView.DataSource = objPds
            TransactionHistoryView.DataBind()
            plhEnquiryResults.Visible = (TransactionHistoryView.Items.Count > 0)
            TransactionHistoryView.Visible = plhEnquiryResults.Visible
            DisplaySummarySection()
        End If
        DisplayPagerNav(objPds)
    End Sub

    Protected Function checkDates(ByVal fromDate As String, ByVal toDate As String) As Boolean
        '--------------------------------------------------------------------
        ' Check that entered dates are valid and that TO date is >= FROM date
        '--------------------------------------------------------------------
        Dim datesOK As Boolean = False
        Try
            Dim tempFromdate As Date
            Dim tempTodate As Date
            If toDate <> String.Empty Then
                tempTodate = SetDateFromString(toDate)
            End If
            If fromDate <> String.Empty Then
                tempFromdate = SetDateFromString(fromDate)
            End If
            If tempTodate >= tempFromdate Then
                datesOK = True
            End If
            If tempFromdate > Today Then
                datesOK = False
            End If
            If tempTodate > Today Then
                datesOK = False
            End If
        Catch ex As Exception

        End Try

        Return datesOK
    End Function

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        Try
            With _ucr
                Select Case sender.ID
                    Case Is = "PaymentReferenceHeader"
                        sender.Text = .Content("PaymentReferenceHeader", _languageCode, True)
                    Case Is = "DateHeader"
                        sender.Text = .Content("DateHeader", _languageCode, True)
                    Case Is = "MemberNumberHeader"
                        sender.Text = .Content("MemberNumberHeader", _languageCode, True)
                    Case Is = "ValueHeader"
                        sender.Text = .Content("ValueHeader", _languageCode, True)
                    Case Is = "LinesHeader"
                        sender.Text = .Content("LinesHeader", _languageCode, True)
                    Case Is = "PaymentTypeHeader"
                        sender.Text = .Content("PaymentTypeHeader", _languageCode, True)
                    Case Is = "ViewDetailsLinkHeader"
                        sender.Text = .Content("ViewDetailsLinkHeader", _languageCode, True)
                    Case Is = "ViewDetailsLink"
                        sender.Text = .Content("ViewDetailsLinkText", _languageCode, True)
                    Case Is = "hplViewDetails"
                        sender.Text = .Content("ViewDetailsLinkText", _languageCode, True)
                    Case Is = "ClubCodeHeader"
                        sender.Text = .Content("ClubCodeHeader", _languageCode, True)
                    Case Is = "PackageId"
                        sender.Text = .Content("PackageId", _languageCode, True)
                End Select
            End With
        Catch ex As Exception
        End Try
    End Sub

    Public Function GetText(ByVal ContentName As String) As String
        Return _ucr.Content(ContentName, _languageCode, True)
    End Function

    Private Sub PopulateClubCode(ByVal dtClubCode As DataTable)
        ddlClubCode.DataSource = dtClubCode
        ddlClubCode.DataTextField = "ClubCode"
        ddlClubCode.DataValueField = "ClubCode"
        ddlClubCode.DataBind()
        Dim allClubsText As String = _ucr.Content("AllClubsText", _languageCode, True)
        ddlClubCode.Items.Insert(0, New ListItem(allClubsText, "All Clubs"))
        ddlClubCode.SelectedIndex = 0
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not String.IsNullOrWhiteSpace(Session("AgentLogout")) AndAlso CType(Session("AgentLogout"), Boolean) = True Then
            Session.Remove("AgentLogout")
            AgentProfile.Logout()
            EndOfDayFilterOutputOnly()
        End If
        plhError.Visible = (lblError.Text.Length > 0)
    End Sub

    Protected Sub TransactionHistoryView_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles TransactionHistoryView.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim hplViewDetails As HyperLink = CType(e.Item.FindControl("hplViewDetails"), HyperLink)
            Dim hplViewDespatchDetails As HyperLink = CType(e.Item.FindControl("hplViewDespatchDetails"), HyperLink)
            'Don't remove this commented lines
            'hplViewDetails.NavigateUrl = String.Format("~/PagesLogin/Orders/PurchaseDetails.aspx?payref={0}&CallId={1}", Talent.eCommerce.Utilities.CheckForDBNull_String(e.Item.DataItem("PaymentReference")), e.Item.DataItem("CallId"))
            hplViewDetails.NavigateUrl = String.Format("~/PagesLogin/Orders/PurchaseDetails.aspx?payref={0}", Talent.eCommerce.Utilities.CheckForDBNull_String(e.Item.DataItem("PaymentReference")))
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ViewDespatchDetailsIcon")) AndAlso e.Item.DataItem("Member").ToString() <> "*GENERIC" Then
                hplViewDespatchDetails.NavigateUrl = "~/PagesAgent/Despatch/DespatchProcess.aspx?payref=" & Talent.eCommerce.Utilities.CheckForDBNull_String(e.Item.DataItem("PaymentReference"))
            Else
                hplViewDespatchDetails.Visible = False
            End If
        End If
    End Sub

    Private Sub ProcessPackageIDSearch()
        Dim order As New TalentOrder
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim err As New ErrorObj
        If Not Profile.IsAnonymous AndAlso Profile.User IsNot Nothing AndAlso Profile.User.Details IsNot Nothing Then
            settings.AccountNo1 = TCUtilities.PadLeadingZeros(Profile.User.Details.Account_No_1, 12)
        Else
            settings.AccountNo1 = TCUtilities.PadLeadingZeros("", 12)
        End If
        If txtPackageIDSearch.Text.Trim.Length > 0 AndAlso IsNumeric(txtPackageIDSearch.Text.Trim) Then
            order.Dep.CallId = txtPackageIDSearch.Text.Trim
        End If
        order.Dep.PaymentReference = ""
        order.Dep.CustNumberPayRefShouldMatch = False
        settings.Cacheing = False
        If Talent.eCommerce.Utilities.IsAgent Then
            settings.OriginatingSource = AgentProfile.Name
        Else
            If TEBUtilities.CheckForDBNull_Int(order.Dep.PaymentReference) <= 0 Then
                settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("CacheTimeMinutes"))
                settings.CacheDependencyPath = ModuleDefaults.CacheDependencyPath
                settings.AuthorityUserProfile = ModuleDefaults.AuthorityUserProfile
            End If
        End If
        order.Dep.LastRecordNumber = 0
        order.Dep.TotalRecords = 0
        order.Settings() = settings
        err = order.OrderEnquiryDetails()
        If Not err.HasError AndAlso order.ResultDataSet.Tables("StatusResults").Rows.Count > 0 Then
            If TEBUtilities.CheckForDBNull_String(order.ResultDataSet.Tables("StatusResults").Rows(0)("ErrorOccurred")).Trim.Length = 0 Then
                If txtPackageIDSearch.Text.Trim.Length > 0 Then
                    If order.ResultDataSet.Tables("OrderEnquiryDetails") IsNot Nothing AndAlso order.ResultDataSet.Tables("OrderEnquiryDetails").Rows.Count > 0 Then
                        If order.ResultDataSet.Tables("PackageDetail") IsNot Nothing AndAlso order.ResultDataSet.Tables("PackageDetail").Rows.Count > 0 Then
                            Response.Redirect("~/PagesLogin/Orders/PurchaseDetails.aspx?payref=" & TEBUtilities.CheckForDBNull_String(order.ResultDataSet.Tables("OrderEnquiryDetails").Rows(0)("PaymentReference")).Trim & "&callid=" & txtPackageIDSearch.Text.Trim)
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Format the currency for the given value
    ''' </summary>
    ''' <param name="value">The valye amount</param>
    ''' <returns>The formatted value string</returns>
    ''' <remarks></remarks>
    Public Function FormatCurrency(ByVal value As Decimal) As String
        Dim formattedString As String = value
        formattedString = TDataObjects.PaymentSettings.FormatCurrency(value, _ucr.BusinessUnit, _ucr.PartnerCode)
        Return formattedString
    End Function

End Class
