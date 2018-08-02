Imports Talent.Common
Imports System.Data
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Collections.Generic
Imports System.Web.UI.WebControls


Partial Class PagesAgent_DespatchNoteGeneration
    Inherits TalentBase01

#Region "Public Properties"
    Public LoadingText As String = String.Empty
    Public Property PageNumber As Integer
        Get
            If ViewState("PageNumber") IsNot Nothing Then
                Return Convert.ToInt32(ViewState("PageNumber"))
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Integer)
            ViewState("PageNumber") = value
        End Set
    End Property
#End Region

#Region "Private variables"
    Private _wfrPage As WebFormResource = Nothing
    Private _languageCode As String = String.Empty
    Private _results As DataTable = Nothing
    Private _errMessage As TalentErrorMessages = Nothing
    Private _dateValid As Boolean = True
    Private _errorMessage As String = String.Empty
    Private _pageMode As PageMode
    Private Enum PageMode
        SCAN = 0
        PRINT = 1
    End Enum
#End Region

#Region "Constants"
    Const KEYCODE As String = "DespatchNoteGeneration.aspx"
#End Region

#Region "Protected variables"
    Protected ColumnHeaderText_Ref As String
    Protected ColumnHeaderText_Status As String
    Protected ColumnHeaderText_Customer As String
    Protected ColumnHeaderText_Quantity As String
    Protected ColumnHeaderText_Select As String
#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        InitialiseClassLevelFields()
        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = KEYCODE
        End With

        If TalentDefaults.IsDespatchInPrintMode Then
            _pageMode = PageMode.PRINT
        End If

        SetTextAndAttributes()
        SetSearchButtonVisibility()

        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "table-functions.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("table-functions.js", "/Application/Elements/"), False)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If btnSearchTop.Visible Then
            Me.Page.Form.DefaultButton = btnSearchTop.UniqueID
        Else
            Me.Page.Form.DefaultButton = btnSearchBottom.UniqueID
        End If

        If Page.IsPostBack = False Then
            SetRequiredFieldValidators()
            LoadSearchDDLs()
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If rptTransaction.Items.Count = 0 Then
            rptTransaction.Visible = False
            Dim ctrl As String = Request.Params.Get("__EVENTTARGET")
            If Page.IsPostBack Then
                If ctrl.Contains("btnSearchTop") Or ctrl.Contains("btnSearchBottom") Then
                    If _dateValid = False Then
                        blErrorMessages.Items.Clear()
                        blErrorMessages.Items.Add(_errorMessage)
                    End If
                End If
            End If
            plhDespatchButtonsTop.Visible = False
            plhDespatchButtonsBottom.Visible = False
            plhCreatePDFTop.Visible = False
            plhCreatePDFBottom.Visible = False
            PlhCreateCSVTop.Visible = False
            plhCreateCSVBottom.Visible = False
        Else
            plhDespatchButtonsBottom.Visible = True
            plhDespatchButtonsTop.Visible = True
            plhCreatePDFTop.Visible = True
            plhCreatePDFBottom.Visible = True
            PlhCreateCSVTop.Visible = True
            plhCreateCSVBottom.Visible = True
            rptTransaction.Visible = True
            SetRepeaterColumnHeaders()
        End If

        If Session("SuccessMessage") IsNot Nothing AndAlso Not String.IsNullOrEmpty(Session("SuccessMessage")) Then
            blSuccessMessages.Items.Add(Session("SuccessMessage"))
        End If
        Session("SuccessMessage") = Nothing

        plhErrorList.Visible = (blErrorMessages.Items.Count > 0)
        plhSuccessList.Visible = (blSuccessMessages.Items.Count > 0)

        If _pageMode <> PageMode.PRINT Then
            plhDespatchButtonsTop.Visible = False
            plhDespatchButtonsBottom.Visible = False
        End If
    End Sub

    Protected Function FormatCustomerNumber(ByVal customrNumber As String) As String
        If Not String.IsNullOrEmpty(customrNumber) Then
            customrNumber = customrNumber.TrimStart("0")
        End If
        Return customrNumber
    End Function

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearchTop.Click, btnSearchBottom.Click
        If ValidateDates() Then

            'Clear the message
            blSuccessMessages.Items.Clear()
            blErrorMessages.Items.Clear()
            PageNumber = 0

            ' Perform relevant search
            Select Case ddlType.SelectedValue
                Case Is = "1"
                    RetrieveDespatchTransactionItems()
                Case Is = "2"
                    RetrieveDespatchTransactionItems()
                Case Is = "3"
                    RetrieveDespatchTransactionItems()
                Case Is = "4"
                    RetrieveDespatchTransactionItems()
            End Select

            Dim dtDNGResults As DataTable = Session("dtDNGResults")
            If Not dtDNGResults Is Nothing Then
                blSuccessMessages.Items.Add(String.Format(_wfrPage.Content("msgResultsFoundText", _languageCode, True), dtDNGResults.Rows.Count.ToString))
            Else
                blErrorMessages.Items.Add(_wfrPage.Content("noSearchResultsErrorMsg", _languageCode, True))
            End If
        End If
    End Sub

    Protected Sub EventDespatchPrint(sender As Object, e As EventArgs) Handles btnPrintAllTop.Click, btnPrintUnprintedTop.Click, btnPrintAllBottom.Click, btnPrintUnprintedBottom.Click
        Dim talentPrint As New TalentPrint
        Dim print As New DEPrint
        Dim buttonSender As Button = CType(sender, Button)

        Select Case buttonSender.ID
            Case "btnPrintAllTop"
                print.PrintAll = True
            Case "btnPrintAllBottom"
                print.PrintAll = True
            Case "btnPrintUnprintedTop"
                print.UnPrintedTickets = True
            Case "btnPrintUnprintedBottom"
                print.UnPrintedTickets = True
        End Select

        print.PaymentReferences = getPaymentReferencesFromTransactions()
        talentPrint.PrintEntity = print
        talentPrint.Settings = GetSettings()
        Dim err As ErrorObj = talentPrint.DespatchTicketPrint()

        If Not err.HasError Then
            blSuccessMessages.Items.Clear()
            Session("SuccessMessage") = _wfrPage.Content("PrintSuccessMessage", _languageCode, True)
            If Not TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("KeepResultsAfterPrint")) Then
                Response.Redirect(Request.Url.AbsoluteUri)
            Else
                If cbIncompleteTransactionsOnly.Checked Then
                    Dim dtDNGResults As DataTable = Session("dtDNGResults")
                    Dim dr As DataRow() = Nothing
                    For Each payref As String In print.PaymentReferences
                        dr = dtDNGResults.Select("TRANSACTIONREF =" + payref)
                        For Each row As DataRow In dr
                            dtDNGResults.Rows.Remove(row)
                        Next
                    Next
                    Session("dtDNGResults") = dtDNGResults
                    blSuccessMessages.Items.Add(String.Format(_wfrPage.Content("msgResultsFoundText", _languageCode, True), dtDNGResults.Rows.Count.ToString))
                    BindRepeater()
                End If
            End If
        Else
            blErrorMessages.Items.Clear()
            Dim errorCode As String = talentPrint.ResultDataSet.Tables("ErrorStatus").Rows(0)("ReturnCode")
            Dim talentErrorMsg As TalentErrorMessage = _errMessage.GetErrorMessage(errorCode)
            blErrorMessages.Items.Add(talentErrorMsg.ERROR_MESSAGE)
        End If
    End Sub
    Protected Sub btnCreatePDF_Click(sender As Object, e As EventArgs) Handles btnCreatePDFTop.Click, btnCreatePDFBottom.Click
        Dim err As New ErrorObj
        Dim despatch As New TalentDespatch
        Dim sBatchID As String = String.Empty
        blErrorMessages.Items.Clear()

        ' Error if no search results
        If rptTransaction.Items.Count = 0 Then
            blErrorMessages.Items.Add(_wfrPage.Content("noSearchResultsErrorMsg", _languageCode, True))
            Exit Sub
        End If

        ' Create data table of selected payment refereneces (for passing to 
        Dim dtPayrefs As New Data.DataTable("Payrefs")
        dtPayrefs.Columns.Add("PYRF", GetType(String))
        Dim dRow As Data.DataRow
        For Each rptTransactionItem As RepeaterItem In rptTransaction.Items
            Dim chkSelectedItem As CheckBox = CType(rptTransactionItem.FindControl("chkSelectedItem"), CheckBox)
            If chkSelectedItem.Checked Then
                Dim hdfBatchID As HiddenField = CType(rptTransactionItem.FindControl("hdfBatchID"), HiddenField)
                Dim hdfTransactionRef As HiddenField = CType(rptTransactionItem.FindControl("hdfTransactionRef"), HiddenField)
                sBatchID = Utilities.PadLeadingZeros(hdfBatchID.Value, 13)
                dRow = Nothing
                dRow = dtPayrefs.NewRow
                dRow("PYRF") = Utilities.PadLeadingZeros(hdfTransactionRef.Value, 15)
                dtPayrefs.Rows.Add(dRow)
            End If
        Next

        ' If none selected then display message 
        If dtPayrefs.Rows.Count = 0 Then
            blErrorMessages.Items.Add(_wfrPage.Content("noSearchResultsSelectedErrorMsg", _languageCode, True))
            Exit Sub
        End If

        Dim deDespatch As New DEDespatch
        deDespatch.DespatchPDFLayoutName = _wfrPage.Attribute("DespatchPDFLayoutName")
        deDespatch.DespatchPDFDocumentPath = _wfrPage.Attribute("DespatchPDFDocumentPathAbsolute")
        deDespatch.DespatchNoteTableColumnHeaders = _wfrPage.Content("DespatchNoteTableColumnHeaders", _languageCode, True)
        deDespatch.DespatchNoteTableColumnWidths = _wfrPage.Attribute("DespatchNoteTableColumnWidths")
        deDespatch.DespatchNoteTableWidth = _wfrPage.Attribute("DespatchNoteTableWidth")
        deDespatch.DespatchNoteTableMaxRowsPerPage = _wfrPage.Attribute("DespatchNoteTableMaxRowsPerPage")
        deDespatch.DespatchNoteFirstLineFormat = _wfrPage.Content("DespatchNoteFirstLineFormat", _languageCode, True)
        deDespatch.DespatchNoteSecondLineFormat = _wfrPage.Content("DespatchNoteSecondLineFormat", _languageCode, True)
        deDespatch.DespatchNoteCollectText = _wfrPage.Content("DespatchNoteCollectFeeText", _languageCode, True)
        deDespatch.DespatchNoteRegPostText = _wfrPage.Content("DespatchNoteRegPostFeeText", _languageCode, True)
        deDespatch.DespatchNotePostText = _wfrPage.Content("DespatchNotePostFeeText", _languageCode, True)
        deDespatch.DespatchNoteCharityFeeText = _wfrPage.Content("DespatchNoteCharityFeeText", _languageCode, True)
        deDespatch.DespatchNoteGiftwrapFeeText = _wfrPage.Content("DespatchNoteGiftwrapFeeText", _languageCode, True)
        deDespatch.DespatchNoteGeographicalZoneTextFormat = _wfrPage.Content("DespatchNoteGeographicalZoneTextFormat", _languageCode, True)
        deDespatch.DespatchNotePostageNotAvailableText = _wfrPage.Content("DespatchNotePostageNotAvailableText", _languageCode, True)
        deDespatch.DespatchNoteCharityNotAvailableText = _wfrPage.Content("DespatchNoteCharityNotAvailableText", _languageCode, True)
        deDespatch.DespatchNoteGiftwrapNotAvailableText = _wfrPage.Content("DespatchNoteGiftwrapNotAvailableText", _languageCode, True)
        deDespatch.DespatchNoteGeoZoneNotAvailableText = _wfrPage.Content("DespatchNoteGeoZoneNotAvailableText", _languageCode, True)
        deDespatch.DespatchNoteTableColumnCount = _wfrPage.Attribute("DespatchNoteTableColumnCount")
        deDespatch.DespatchNoteTableColumnDetailValues = _wfrPage.Attribute("DespatchNoteTableColumnDetailValues")
        deDespatch.DespatchNoteSummaryOrDetail = _wfrPage.Attribute("DespatchNoteSummaryOrDetail")
        deDespatch.DespatchNoteOrientation = _wfrPage.Attribute("DespatchNoteOrientation")

        deDespatch.DespatchNoteGeographicalZoneTable = RetrieveGeographicalZones()
        deDespatch.BatchID = sBatchID
        deDespatch.DataTable = dtPayrefs
        deDespatch.DespatchReservations = (ddlType.SelectedValue = 2)
        deDespatch.DespatchRetail = (ddlType.SelectedValue = 3)
        despatch.Settings = GetSettings()
        despatch.Settings.Language = Talent.eCommerce.Utilities.GetCurrentLanguage()
        despatch.DeDespatch = deDespatch
        err = despatch.GenerateDespatchNotes()

        If err.HasError Then
            blErrorMessages.Items.Add(err.ErrorNumber + ": " + err.ErrorMessage)
        Else
            Dim sURL As String = String.Format("{0}://{1}:{2}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port) & _
                                    _wfrPage.Attribute("DespatchPDFDocumentPathRelative").ToString & _
                                    despatch.DeDespatch.GeneratedDespatchPDFDocument

            ' Create client-side script to enable new document to be displayed in new browser tab
            ScriptManager.RegisterStartupScript(updSearch, updSearch.GetType(), "openPDFScript", "javascript:openWindow('" + sURL + "')", True)

            ' Display success message
            blSuccessMessages.Items.Add(String.Format(_wfrPage.Content("pdfSuccessMsg", _languageCode, True), despatch.DeDespatch.GeneratedDespatchPDFDocument, despatch.DeDespatch.GeneratedDespatchPDFDocumentPageCount, sURL))
        End If
    End Sub

    Private Sub CreateAddressLabelCSV()
        Dim err As New ErrorObj
        Dim despatch As New TalentDespatch
        Dim sBatchID As String = String.Empty
        blErrorMessages.Items.Clear()

        ' Error if no search results
        If rptTransaction.Items.Count = 0 Then
            blErrorMessages.Items.Add(_wfrPage.Content("noSearchResultsErrorMsg", _languageCode, True))
            Exit Sub
        End If

        ' Create data table of selected payment refereneces (for passing to 
        Dim dtPayrefs As New Data.DataTable("Payrefs")
        dtPayrefs.Columns.Add("PYRF", GetType(String))
        Dim dRow As Data.DataRow
        For Each rptTransactionItem As RepeaterItem In rptTransaction.Items
            Dim chkSelectedItem As CheckBox = CType(rptTransactionItem.FindControl("chkSelectedItem"), CheckBox)
            If chkSelectedItem.Checked Then
                Dim hdfBatchID As HiddenField = CType(rptTransactionItem.FindControl("hdfBatchID"), HiddenField)
                Dim hdfTransactionRef As HiddenField = CType(rptTransactionItem.FindControl("hdfTransactionRef"), HiddenField)
                sBatchID = Utilities.PadLeadingZeros(hdfBatchID.Value, 13)
                dRow = Nothing
                dRow = dtPayrefs.NewRow
                dRow("PYRF") = Utilities.PadLeadingZeros(hdfTransactionRef.Value, 15)
                dtPayrefs.Rows.Add(dRow)
            End If
        Next

        ' If none selected then display message 
        If dtPayrefs.Rows.Count = 0 Then
            blErrorMessages.Items.Add(_wfrPage.Content("noSearchResultsSelectedErrorMsg", _languageCode, True))
            Exit Sub
        End If

        Dim deDespatch As New DEDespatch
        deDespatch.DespatchPDFDocumentPath = _wfrPage.Attribute("DespatchPDFDocumentPathAbsolute")
        deDespatch.BatchID = sBatchID
        deDespatch.DataTable = dtPayrefs
        despatch.Settings = GetSettings()
        despatch.Settings.Language = Talent.eCommerce.Utilities.GetCurrentLanguage()
        despatch.DeDespatch = deDespatch
        err = despatch.RetrieveDespatchAddressLabelItems()

        If err.HasError Then
            blErrorMessages.Items.Add(err.ErrorNumber + ": " + err.ErrorMessage)
        Else

            Dim sComma As String = ","
            Dim sCSV As New StringBuilder
            Dim sCSVitem As String = Nothing
            Dim vCSVview As New DataView(despatch.ResultDataSet.Tables("DespatchAddressLabelItems"))
            Dim dtCSV As DataTable = Nothing
            For Each dRow In dtPayrefs.Rows
                vCSVview.RowFilter = "PYRF07 = " + Convert.ToInt32(dRow.Item("PYRF")).ToString
                dtCSV = vCSVview.ToTable()
                If dtCSV.Rows.Count > 0 Then
                    ' Append
                    For i = 0 To dtCSV.Columns.Count - 1
                        sCSVitem = Trim(dtCSV.Rows(0).Item(i).ToString)
                        sCSVitem = sCSVitem.Replace(",", " ")
                        sCSV.Append(sCSVitem)
                        sCSV.Append(sComma)
                    Next
                    sCSV.Append(sBatchID)
                    sCSV.Append(vbCrLf)
                End If
                dtCSV.Dispose()
            Next


            Dim outFile As System.IO.StreamWriter
            Dim sFileName As String = "DespatchAddressLabels-" & Now.Year.ToString & Now.Month.ToString & Now.Day.ToString & _
                                        Now.Hour.ToString & Now.Minute.ToString & Now.Second.ToString & Now.Millisecond.ToString & ".csv"

            outFile = New System.IO.StreamWriter(_wfrPage.Attribute("DespatchPDFDocumentPathAbsolute") & sFileName, False)
            outFile.Write(sCSV.ToString)
            outFile.Close()

            Dim sURL As String = String.Format("{0}://{1}:{2}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port) & _
                                        _wfrPage.Attribute("DespatchPDFDocumentPathRelative").ToString & sFileName

            ' Create client-side script to enable new document to be displayed in new browser tab
            ScriptManager.RegisterStartupScript(updSearch, updSearch.GetType(), "openPDFScript", "javascript:openWindow('" + sURL + "')", True)
            ' Display success message
            blSuccessMessages.Items.Add(String.Format(_wfrPage.Content("pdfSuccessMsg", _languageCode, True), despatch.DeDespatch.GeneratedDespatchPDFDocument, despatch.DeDespatch.GeneratedDespatchPDFDocumentPageCount, sURL))
        End If


    End Sub
    Protected Sub btnCreateCSV_Click(sender As Object, e As EventArgs) Handles btnCreateCSVTop.Click, btnCreateCSVBottom.Click
        CreateAddressLabelCSV()
    End Sub
    Protected Sub btnClearFilter_Click(sender As Object, e As EventArgs) Handles btnClearFilterTop.Click, btnClearFilterBottom.Click
        ClearAll()

        If ddlType.Items.Count > 0 Then
            ddlType.SelectedValue = 1
        End If

        If ddlCategory.Items.Count > 0 Then
            ddlCategory.SelectedValue = "-1"
        End If

        If ddlSubCategory.Items.Count > 0 Then
            ddlSubCategory.SelectedValue = "-1"
        End If

        If ddlEvent.Items.Count > 0 Then
            ddlEvent.SelectedValue = "-1"
        End If

        If ddlStand.Items.Count > 0 Then
            ddlStand.SelectedValue = "-1"
        End If

        If ddlArea.Items.Count > 0 Then
            ddlArea.SelectedValue = "-1"
        End If

        If ddlDeliveryMethod.Items.Count > 0 Then
            ddlDeliveryMethod.SelectedValue = _wfrPage.Attribute("DefaultDeliveryMethod")
        End If

        If ddlCountry.Items.Count > 0 Then
            ddlCountry.SelectedValue = _wfrPage.Attribute("DefaultCountryMethod")
        End If

        If ddlPostcode.Items.Count > 0 Then
            ddlPostcode.SelectedValue = _wfrPage.Attribute("DefaultPostcodeMethod")
        End If
        If ddlPayMeth.Items.Count > 0 Then
            ddlPayMeth.SelectedValue = "-1"
        End If
        If ddlSaleAgent.Items.Count > 0 Then
            ddlSaleAgent.SelectedValue = "-1"
        End If
        txtDateFrom.Text = ""
        txtPaymentRef.Text = ""
        txtDateTo.Text = ""
        cbGiftWrap.Checked = False
    End Sub
    Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlType.SelectedIndexChanged
        ClearAll()
        ShowFilters()
    End Sub
    Protected Sub ddlCategory_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlCategory.SelectedIndexChanged
        ClearAll()
        LoadDDL_Product()
    End Sub

    Protected Sub ddlSubCategory_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlSubCategory.SelectedIndexChanged
        ClearAll()
        LoadDDL_Product()
    End Sub

    Protected Sub ddlEvent_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlEvent.SelectedIndexChanged
        ClearAll()
        LoadDDL_Stand()
    End Sub

    Protected Sub ddlStand_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlStand.SelectedIndexChanged
        Dim selectedStand As String = ""
        'clear area dropdown list
        ddlArea.Items.Clear()
        selectedStand = ddlStand.SelectedValue
        If (selectedStand <> "" AndAlso selectedStand <> "-1") Then
            LoadDDL_Area(selectedStand)
        End If
    End Sub

    Protected Sub rptPaging_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs)
        PageNumber = Convert.ToInt32(e.CommandArgument) - 1
        BindRepeater()
    End Sub

#End Region

#Region "Private Methods"
    Private Sub BindRepeater()
        Dim dt As DataTable = Nothing
        If Session("dtDNGResults") IsNot Nothing Then
            dt = Session("dtDNGResults")

            Dim pgitems As PagedDataSource = New PagedDataSource()
            pgitems.DataSource = dt.DefaultView
            pgitems.AllowPaging = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("AllowPaging"))
            pgitems.PageSize = Utilities.CheckForDBNull_Int(_wfrPage.Attribute("PageSize"))
            If PageNumber <= pgitems.PageCount - 1 Then
                pgitems.CurrentPageIndex = PageNumber
            Else
                pgitems.CurrentPageIndex = 0
            End If
            If pgitems.PageCount > 1 Then
                rptPagingTop.Visible = True
                rptPagingBottom.Visible = True
                Dim pages As ArrayList = New ArrayList()
                For i As Integer = 0 To pgitems.PageCount - 1
                    pages.Add((i + 1).ToString())
                Next

                rptPagingTop.DataSource = pages
                rptPagingTop.DataBind()
                rptPagingBottom.DataSource = pages
                rptPagingBottom.DataBind()

                'Set the class for the active button
                For Each rptPage As RepeaterItem In rptPagingTop.Items
                    Dim btnPage As LinkButton = CType(rptPage.FindControl("btnPageTop"), LinkButton)
                    If btnPage.CommandArgument.ToString = (PageNumber + 1).ToString Then
                        'code here for active
                        btnPage.Attributes.Add("class", "ebiz-current-page")
                    End If
                Next
                'Set the class for the active button
                For Each rptPage As RepeaterItem In rptPagingBottom.Items
                    Dim btnPage As LinkButton = CType(rptPage.FindControl("btnPageBottom"), LinkButton)
                    If btnPage.CommandArgument.ToString = (PageNumber + 1).ToString Then
                        'code here for active
                        btnPage.Attributes.Add("class", "ebiz-current-page")
                    End If
                Next
            Else
                rptPagingTop.Visible = False
                rptPagingBottom.Visible = False
            End If
            rptTransaction.DataSource = pgitems
        Else
            rptTransaction.DataSource = Nothing
        End If
        rptTransaction.DataBind()
    End Sub

    Protected Function TransactionStatus(sStatusCode As String) As String
        Dim ret As String = sStatusCode
        Select Case sStatusCode
            Case Is = "D"
                ret = _wfrPage.Content("FullyDespatched", _languageCode, True)
            Case Is = "P"
                ret = _wfrPage.Content("PartDespatched", _languageCode, True)
            Case Is = "N"
                ret = _wfrPage.Content("NotDespatched", _languageCode, True)
        End Select
        Return ret
    End Function

    Private Function ValidateDates() As Boolean
        If Not String.IsNullOrEmpty(txtDateTo.Text) Then
            Dim todayDate As Date = Date.Now.ToString
            Dim dateTo As Date = txtDateTo.Text.ToString
            Dim dateFrom As Date = txtDateFrom.Text.ToString

            If dateTo.Date > todayDate.Date Then
                _dateValid = False

                _errorMessage = _wfrPage.Content("FutureDateErrorMessage", _languageCode, True)
            End If
            If dateFrom.Date > dateTo.Date Then
                _dateValid = False
                _errorMessage = _wfrPage.Content("FromDateErrorMessage", _languageCode, True)
            End If
            If _dateValid Then
                Dim intDateRangeLimit As Integer = Utilities.CheckForDBNull_Int(_wfrPage.Attribute("DateRangeLimit"))
                If intDateRangeLimit > 0 Then
                    Dim diff As TimeSpan = dateTo.Subtract(dateFrom)
                    If diff.TotalDays > intDateRangeLimit Then
                        _dateValid = False
                        _errorMessage = String.Format(_wfrPage.Content("DateRangeExceededErrorMessage", _languageCode, True), intDateRangeLimit.ToString)
                    End If
                End If
            End If
        End If
        Return _dateValid
    End Function

    Private Sub ShowFilters()
        blErrorMessages.Items.Clear()

        Dim typeVisible As Boolean = True
        Dim dateFromVisible As Boolean = False
        Dim dateToVisible As Boolean = False
        Dim categoryVisible As Boolean = False
        Dim subCategoryVisible As Boolean = False
        Dim eventVisible As Boolean = False
        Dim standVisible As Boolean = False
        Dim areaVisible As Boolean = False
        Dim payRefVisible As Boolean = False
        Dim deliveryMethodVisible As Boolean = False
        Dim postcodeVisible As Boolean = False
        Dim countryVisible As Boolean = False
        Dim giftWrapVisible As Boolean = False
        Dim saleAgentVisible As Boolean = False
        Dim payMethVisible As Boolean = False

        SetSearchButtonVisibility()
        SetCreatePDFButtonVisibility()
        SetCreateCSVButtonVisibility()
        SetClearFilterButtonVisibility()
        If ddlType.SelectedValue = "-1" Then
            btnSearchTop.Visible = False
            btnSearchBottom.Visible = False
            btnCreatePDFTop.Visible = False
            btnCreatePDFBottom.Visible = False
            btnCreateCSVTop.Visible = False
            btnCreateCSVBottom.Visible = False
            btnClearFilterTop.Visible = False
            btnClearFilterBottom.Visible = False
            rptTransaction.DataSource = Nothing
            rptTransaction.DataBind()

            ' Tickets or Manual Intervention
        ElseIf ddlType.SelectedValue = "1" Or ddlType.SelectedValue = "4" Then
            ColumnHeaderText_Ref = _wfrPage.Content("ColumnHeaderPaymentRef", _languageCode, True)

            dateFromVisible = True
            dateToVisible = True
            categoryVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowCategoryFilter"))
            subCategoryVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowSubCategoryFilter"))
            eventVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowEventFilter"))
            standVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowStandFilter"))
            areaVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowAreaFilter"))
            payRefVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowPayRefFilter"))
            deliveryMethodVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowDeliveryMethodFilter"))
            postcodeVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowPostcodeFilter"))
            countryVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowCountryFilter"))
            giftWrapVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowGiftWrapFilter"))
            saleAgentVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowSaleAgentFilter"))
            payMethVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowPayMethFilter"))
            rptTransaction.DataSource = Nothing
            rptTransaction.DataBind()

            ' SoR Reservations
        ElseIf ddlType.SelectedValue = "2" Then
            ColumnHeaderText_Ref = _wfrPage.Content("ColumnHeaderReservationRef", _languageCode, True)

            dateFromVisible = True
            dateToVisible = True
            categoryVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowCategoryFilter"))
            subCategoryVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowSubCategoryFilter"))
            eventVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowEventFilter"))
            standVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowStandFilter"))
            areaVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowAreaFilter"))
            postcodeVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowPostcodeFilter"))
            countryVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowCountryFilter"))
            rptTransaction.DataSource = Nothing
            rptTransaction.DataBind()

            ' Retail
        ElseIf ddlType.SelectedValue = "3" Then
            ColumnHeaderText_Ref = _wfrPage.Content("ColumnHeaderPaymentRef", _languageCode, True)
            dateFromVisible = True
            dateToVisible = True
            payRefVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowPayRefFilter"))
            deliveryMethodVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowDeliveryMethodFilter"))
            postcodeVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowPostcodeFilter"))
            countryVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowCountryFilter"))
            giftWrapVisible = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("ShowGiftWrapFilter"))
            rptTransaction.DataSource = Nothing
            rptTransaction.DataBind()
        End If

        SetType(typeVisible)
        SetDateFrom(dateFromVisible)
        SetDateTo(dateToVisible)
        SetCategory(categoryVisible)
        SetSubCategory(subCategoryVisible)
        SetEvent(eventVisible)
        SetStand(standVisible)
        SetArea(areaVisible)
        SetPaymentRef(payRefVisible)
        SetDeliveryMethod(deliveryMethodVisible)
        SetPostcode(postcodeVisible)
        SetCountry(countryVisible)
        SetSaleAgent(saleAgentVisible)
        SetPayMeth(payMethVisible)
        SetGiftWrap(giftWrapVisible)
    End Sub

    Private Sub SetType(visibility As Boolean)
        plhType.Visible = visibility
        If plhType.Visible Then
            rfvType.Enabled = True
        Else
            rfvType.Enabled = False
            ddlType.SelectedValue = "-1"
        End If
    End Sub

    Private Sub SetDateFrom(visibility As Boolean)
        plhDateFrom.Visible = visibility
        If plhDateFrom.Visible Then
            rfvFromDate.Enabled = True
        Else
            rfvFromDate.Enabled = False
            txtDateFrom.Text = String.Empty
        End If
    End Sub

    Private Sub SetDateTo(visibility As Boolean)
        plhDateTo.Visible = visibility
        If plhDateTo.Visible Then
            rfvToDate.Enabled = True
        Else
            rfvToDate.Enabled = False
            txtDateTo.Text = String.Empty
        End If
    End Sub

    Private Sub SetCategory(visibility As Boolean)
        plhCategory.Visible = visibility
        If Not plhCategory.Visible Then
            ddlCategory.SelectedValue = "-1"
            LoadDDL_Product()
        End If
    End Sub

    Private Sub SetSubCategory(visibility As Boolean)
        plhSubCategory.Visible = visibility
        If Not plhSubCategory.Visible Then
            ddlSubCategory.SelectedValue = "-1"
            LoadDDL_Product()
        End If
    End Sub

    Private Sub SetEvent(visibility As Boolean)
        plhEvent.Visible = visibility
        If Not plhEvent.Visible Then
            ddlEvent.SelectedValue = "-1"
        End If
    End Sub

    Private Sub SetStand(visibility As Boolean)
        plhStand.Visible = visibility
        If Not plhStand.Visible Then
            ddlStand.Items.Clear()
        End If
    End Sub

    Private Sub SetArea(visibility As Boolean)
        plhArea.Visible = visibility
        If Not plhArea.Visible Then
            ddlArea.Items.Clear()
        End If
    End Sub

    Private Sub SetPaymentRef(visibility As Boolean)
        plhPayRef.Visible = visibility
        If Not plhPayRef.Visible Then
            txtPaymentRef.Text = String.Empty
        End If
    End Sub

    Private Sub SetDeliveryMethod(visibility As Boolean)
        plhDeliveryMethod.Visible = visibility
        If Not plhDeliveryMethod.Visible Then
            ddlDeliveryMethod.SelectedValue = "-1"
        End If
    End Sub

    Private Sub SetPostcode(visibility As Boolean)
        plhPostcode.Visible = visibility
        If plhPostcode.Visible Then
            rfvPostcode.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("PostcodeMandatory"))
        Else
            rfvPostcode.Enabled = False
            ddlPostcode.SelectedValue = "-1"
        End If
    End Sub

    Private Sub SetCountry(visibility As Boolean)
        plhCountry.Visible = visibility

        If plhCountry.Visible Then
            rfvCountry.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("CountryMandatory"))
        Else
            rfvCountry.Enabled = False
            ddlCountry.SelectedValue = "-1"
        End If
    End Sub

    Private Sub SetSaleAgent(visibility As Boolean)
        plhSaleAgent.Visible = visibility

        If plhSaleAgent.Visible Then
            rfvSaleAgent.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("SaleAgentMandatory"))
        Else
            rfvSaleAgent.Enabled = False
            ddlSaleAgent.SelectedValue = "-1"
        End If
    End Sub

    Private Sub SetPayMeth(visibility As Boolean)
        plhPayMeth.Visible = visibility

        If plhPayMeth.Visible Then
            rfvPayMeth.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("PayMethMandatory"))
        Else
            rfvPayMeth.Enabled = False
            ddlPayMeth.SelectedValue = "-1"
        End If
    End Sub

    Private Sub SetGiftWrap(visibility As Boolean)
        plhGiftWrap.Visible = visibility
        If Not plhGiftWrap.Visible Then
            cbGiftWrap.Checked = False
        End If
    End Sub

    Private Sub InitialiseClassLevelFields()
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        _wfrPage = New WebFormResource
        _errMessage = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)
    End Sub

    Private Sub SetTextAndAttributes()
        lblType.Text = _wfrPage.Content("TypeLabel", _languageCode, True)
        lblDateFrom.Text = _wfrPage.Content("DateFromLabel", _languageCode, True)
        lblDateTo.Text = _wfrPage.Content("DateToLabel", _languageCode, True)
        lblCategory.Text = _wfrPage.Content("CategoryLabel", _languageCode, True)
        lblSubCategory.Text = _wfrPage.Content("SubCategoryLabel", _languageCode, True)
        lblEvent.Text = _wfrPage.Content("EventLabel", _languageCode, True)
        lblStand.Text = _wfrPage.Content("StandLabel", _languageCode, True)
        lblArea.Text = _wfrPage.Content("AreaLabel", _languageCode, True)
        lblPaymentRef.Text = _wfrPage.Content("PaymentRefLabel", _languageCode, True)
        lblDeliveryMethod.Text = _wfrPage.Content("DeliveryMethodLabel", _languageCode, True)
        lblPostcode.Text = _wfrPage.Content("PostcodeLabel", _languageCode, True)
        lblCountry.Text = _wfrPage.Content("CountryLabel", _languageCode, True)
        lblIncompleteTransactionsOnly.Text = _wfrPage.Content("IncompleteTransactionsOnlyLabel", _languageCode, True)
        lblGiftWrap.Text = _wfrPage.Content("GiftWrapOnlyLabel", _languageCode, True)
        lblIncludeAdditionalTickets.Text = _wfrPage.Content("IncludeAdditionalTicketsLabel", _languageCode, True)
        lblPayMeth.Text = _wfrPage.Content("PayMethLabel", _languageCode, True)
        lblSaleAgent.Text = _wfrPage.Content("SaleAgentLabel", _languageCode, True)
        btnSearchTop.Text = _wfrPage.Content("Search", _languageCode, True)
        btnSearchBottom.Text = _wfrPage.Content("Search", _languageCode, True)
        btnCreatePDFTop.Text = _wfrPage.Content("CreatePDF", _languageCode, True)
        btnCreatePDFBottom.Text = _wfrPage.Content("CreatePDF", _languageCode, True)
        btnCreateCSVTop.Text = _wfrPage.Content("CreateCSV", _languageCode, True)
        btnCreateCSVBottom.Text = _wfrPage.Content("CreateCSV", _languageCode, True)
        btnPrintAllTop.Text = _wfrPage.Content("PrintAll", _languageCode, True)
        btnPrintAllBottom.Text = _wfrPage.Content("PrintAll", _languageCode, True)
        btnPrintUnprintedTop.Text = _wfrPage.Content("PrintUnprinted", _languageCode, True)
        btnPrintUnprintedBottom.Text = _wfrPage.Content("PrintUnprinted", _languageCode, True)
        btnClearFilterTop.Text = _wfrPage.Content("ClearFilter", _languageCode, True)
        btnClearFilterBottom.Text = _wfrPage.Content("ClearFilter", _languageCode, True)
        cbIncludeAdditionalTickets.Checked = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("DefaultIncludeadditionaltickets"))
        cbIncompleteTransactionsOnly.Checked = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("DefaultIncompleteTransactionsOnly"))

        SetClearFilterButtonVisibility()

        plhType.Visible = True
        plhDateFrom.Visible = False
        plhDateTo.Visible = False
        plhCategory.Visible = False
        plhSubCategory.Visible = False
        plhEvent.Visible = False
        plhStand.Visible = False
        plhArea.Visible = False
        plhPayRef.Visible = False
        plhDeliveryMethod.Visible = False
        plhPostcode.Visible = False
        plhCountry.Visible = False
        plhGiftWrap.Visible = False
        plhSaleAgent.Visible = False
        plhPayMeth.Visible = False
        regExPaymentRef.ValidationExpression = _wfrPage.Attribute("PaymentRefRegularExpression")
        regExPaymentRef.ErrorMessage = _wfrPage.Content("ErrorInvalidPaymentRef", _languageCode, True)
        rfvType.ErrorMessage = _wfrPage.Content("ErrorTypeNotSelected", _languageCode, True)
        LoadingText = _wfrPage.Content("LoadingText", _languageCode, True)
    End Sub

    Private Sub SetClearFilterButtonVisibility()
        btnClearFilterTop.Visible = False
        btnClearFilterBottom.Visible = False
        Dim clearButtonPlace As String = _wfrPage.Attribute("ClearFilterButtonPlace")
        Dim showClearFilterButton As Boolean = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_wfrPage.Attribute("ShowClearFilterButton"))
        If showClearFilterButton Then
            If Not String.IsNullOrWhiteSpace(clearButtonPlace) Then
                If clearButtonPlace = "Top" Then
                    btnClearFilterTop.Visible = showClearFilterButton

                ElseIf clearButtonPlace = "Bottom" Then
                    btnClearFilterBottom.Visible = showClearFilterButton
                Else
                    btnClearFilterBottom.Visible = showClearFilterButton
                    btnClearFilterTop.Visible = showClearFilterButton
                End If
            End If
        End If
    End Sub

    Private Sub SetSearchButtonVisibility()
        btnSearchTop.Visible = False
        btnSearchBottom.Visible = False
        Dim searchButtonPlace As String = _wfrPage.Attribute("SearchButtonPlace")
        If Not String.IsNullOrWhiteSpace(searchButtonPlace) Then
            If searchButtonPlace = "Top" Then
                btnSearchTop.Visible = True
            ElseIf searchButtonPlace = "Bottom" Then
                btnSearchBottom.Visible = True
            Else
                btnSearchTop.Visible = True
                btnSearchBottom.Visible = True
            End If
        End If
    End Sub

    Private Sub SetCreatePDFButtonVisibility()
        btnCreatePDFTop.Visible = False
        btnCreatePDFBottom.Visible = False
        Dim createPDFButtonPlace As String = _wfrPage.Attribute("CreatePDFButtonPlace")
        If Not String.IsNullOrWhiteSpace(createPDFButtonPlace) Then
            If createPDFButtonPlace = "Top" Then
                btnCreatePDFTop.Visible = True
            ElseIf createPDFButtonPlace = "Bottom" Then
                btnCreatePDFBottom.Visible = True
            Else
                btnCreatePDFTop.Visible = True
                btnCreatePDFBottom.Visible = True
            End If
        End If
    End Sub
    Private Sub SetCreateCSVButtonVisibility()
        btnCreateCSVTop.Visible = False
        btnCreateCSVBottom.Visible = False
        Dim createCSVButtonPlace As String = _wfrPage.Attribute("CreateCSVButtonPlace")
        If Not String.IsNullOrWhiteSpace(createCSVButtonPlace) Then
            If createCSVButtonPlace = "Top" Then
                btnCreateCSVTop.Visible = True
            ElseIf createCSVButtonPlace = "Bottom" Then
                btnCreateCSVBottom.Visible = True
            Else
                btnCreateCSVTop.Visible = True
                btnCreateCSVBottom.Visible = True
            End If
        End If
    End Sub
    Private Sub SetRequiredFieldValidators()
        rfvType.ErrorMessage = _wfrPage.Content("ErrorTypeNotSelected", _languageCode, True)
        rfvFromDate.ErrorMessage = _wfrPage.Content("ErrorFromDateNotSelected", _languageCode, True)
        rfvToDate.ErrorMessage = _wfrPage.Content("ErrorToDateNotSelected", _languageCode, True)

        rfvCategory.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("CategoryMandatory"))
        rfvCategory.ErrorMessage = _wfrPage.Content("ErrorCategoryNotSelected", _languageCode, True)

        rfvSubCategory.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("SubCategoryMandatory"))
        rfvSubCategory.ErrorMessage = _wfrPage.Content("ErrorSubCategoryNotSelected", _languageCode, True)

        rfvEvent.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("EventMandatory"))
        rfvEvent.ErrorMessage = _wfrPage.Content("ErrorEventNotSelected", _languageCode, True)

        rfvArea.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("AreaMandatory"))
        rfvArea.ErrorMessage = _wfrPage.Content("ErrorAreaNotSelected", _languageCode, True)

        rfvStand.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("StandMandatory"))
        rfvStand.ErrorMessage = _wfrPage.Content("ErrorStandNotSelected", _languageCode, True)

        rfvPayRef.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("PaymentReferenceMandatory"))
        rfvPayRef.ErrorMessage = _wfrPage.Content("ErrorPayRefNotEntered", _languageCode, True)

        rfvDeliveryMethod.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("DeliveryMethodMandatory"))
        rfvDeliveryMethod.ErrorMessage = _wfrPage.Content("ErrorDeliveryMethodNotSelected", _languageCode, True)

        rfvCountry.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("CountryMandatory"))
        rfvCountry.ErrorMessage = _wfrPage.Content("ErrorCountryNotSelected", _languageCode, True)

        rfvPostcode.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("PostcodeMandatory"))
        rfvPostcode.ErrorMessage = _wfrPage.Content("ErrorPostcodeNotSelected", _languageCode, True)

        rfvPayMeth.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("PayMethMandatory"))
        rfvPayMeth.ErrorMessage = _wfrPage.Content("ErrorPayMetheNotSelected", _languageCode, True)

        rfvSaleAgent.Enabled = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_wfrPage.Attribute("SaleAgentMandatory"))
        rfvSaleAgent.ErrorMessage = _wfrPage.Content("ErrorSaleAgentNotSelected", _languageCode, True)
    End Sub

    Private Sub LoadSearchDDLs()
        LoadDDL_Types()
        LoadDDL_Category()
        LoadDDL_SubCategory()
        LoadDDL_Product()
        LoadDDL_DeliveryMethod()
        LoadDDL_Countries()
        LoadDDL_Postcodes()
        LoadDDL_SaleAgent()
        LoadDDL_PayMeth()
    End Sub

    Private Sub LoadDDL_Types()
        Dim err As New ErrorObj
        Dim dt As New DataTable
        Dim utilitites As New TalentUtiltities
        utilitites.DescriptionKey = "DSTY"
        utilitites.Settings = GetSettings()
        err = utilitites.RetrieveDescriptionEntries()
        If utilitites.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
        Else
            dt = utilitites.ResultDataSet.Tables("DescriptionsEntries")
        End If

        ddlType.Items.Clear()
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            AddDDLOption(ddlType, _wfrPage.Content("PleaseSelectOptionText", _languageCode, True), "-1")
            dt.DefaultView.Sort = "VALUES ASC"
            dt = dt.DefaultView.ToTable
            Dim i As Integer = 1
            Dim selectedIndex As Integer = 0

            For Each row As DataRow In dt.Rows
                AddDDLOption(ddlType, row("DESCRIPTION"), row("VALUES"))

                If row("VALUES") = 1 Then
                    selectedIndex = i
                End If
                i = i + 1
            Next
            ddlType.SelectedIndex = selectedIndex
            ShowFilters()
        End If
    End Sub

    Private Sub LoadDDL_Category()
        Dim err As New ErrorObj
        Dim dt As New DataTable
        Dim utilitites As New TalentUtiltities
        utilitites.DescriptionKey = "ETYP"
        utilitites.Settings = GetSettings()
        err = utilitites.RetrieveDescriptionEntries()
        If utilitites.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
        Else
            dt = utilitites.ResultDataSet.Tables("DescriptionsEntries")
        End If

        ddlCategory.Items.Clear()
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            AddDDLOption(ddlCategory, _wfrPage.Content("PleaseSelectOptionText", _languageCode, True), "-1")
            dt.DefaultView.Sort = "DESCRIPTION ASC"
            dt = dt.DefaultView.ToTable
            For Each row As DataRow In dt.Rows
                AddDDLOption(ddlCategory, row("DESCRIPTION"), row("CODE"))
            Next
        End If
    End Sub

    Private Sub LoadDDL_SubCategory()
        Dim err As New ErrorObj
        Dim dt As New DataTable
        Dim utilitites As New TalentUtiltities
        utilitites.DescriptionKey = "AWST,CLST,EVST,HMST,STST,TVST"
        utilitites.Settings = GetSettings()
        err = utilitites.RetrieveDescriptionEntries()
        If utilitites.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
        Else
            dt = utilitites.ResultDataSet.Tables("DescriptionsEntries")
        End If

        ddlSubCategory.Items.Clear()
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            AddDDLOption(ddlSubCategory, _wfrPage.Content("PleaseSelectOptionText", _languageCode, True), "-1")
            dt.DefaultView.Sort = "DESCRIPTION ASC"
            dt = dt.DefaultView.ToTable
            For Each row As DataRow In dt.Rows
                AddDDLOption(ddlSubCategory, row("DESCRIPTION"), row("CODE"))
            Next
        End If
    End Sub
    Private Sub LoadDDL_Product()
        Dim superType As String = ""
        Dim subType As String = ""
        If (Not ddlCategory.SelectedValue = "-1") Then superType = ddlCategory.SelectedValue
        If (Not ddlSubCategory.SelectedValue = "-1") Then subType = ddlSubCategory.SelectedValue
        Dim deProduct As New DEProductDetails
        deProduct.ProductSupertype = superType
        deProduct.ProductSubtype = subType
        Dim err As New ErrorObj
        Dim dt As New DataTable
        Dim product As New TalentProduct
        product.Settings = GetSettings()
        product.De = deProduct
        err = product.RetrieveProductsFiltered()

        If product.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
        Else
            dt = product.ResultDataSet.Tables("ProductEntries")
            SetStadiumDictionary(dt)
        End If

        ddlEvent.Items.Clear()
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            AddDDLOption(ddlEvent, _wfrPage.Content("PleaseSelectOptionText", _languageCode, True), "-1")
            dt.DefaultView.Sort = "PRODUCTDESCRIPTION ASC"
            dt = dt.DefaultView.ToTable
            For Each row As DataRow In dt.Rows
                AddDDLOption(ddlEvent, row("PRODUCTDESCRIPTION"), row("PRODUCTCODE"))
            Next
        End If

    End Sub

    Private Sub LoadDDL_Stand()
        Dim stadiumCode As String = GetStadiumCode()
        Dim deProduct As New DEProductDetails
        deProduct.StadiumCode = stadiumCode
        Dim err As New ErrorObj
        Dim dt As New DataTable
        Dim product As New TalentProduct
        product.Settings = GetSettings()
        product.De = deProduct
        err = product.RetrieveStadiumStands()

        If product.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
        Else
            dt = product.ResultDataSet.Tables("StadiumStands")
        End If
        ddlStand.Items.Clear()
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            AddDDLOption(ddlStand, _wfrPage.Content("PleaseSelectOptionText", _languageCode, True), "-1")
            dt.DefaultView.Sort = "STANDDESCRIPTION ASC"
            dt = dt.DefaultView.ToTable
            For Each row As DataRow In dt.Rows
                AddDDLOption(ddlStand, row("STANDDESCRIPTION"), row("STANDCODE"))
            Next
        End If
    End Sub

    Private Sub LoadDDL_Area(ByVal standSelected As String)
        Dim stadiumCode As String = GetStadiumCode()
        Dim deProduct As New DEProductDetails
        Dim whereCondition As String = ""
        deProduct.StadiumCode = stadiumCode
        Dim err As New ErrorObj
        Dim dt As New DataTable
        Dim product As New TalentProduct
        product.Settings = GetSettings()
        product.De = deProduct
        err = product.RetrieveStadiumStandAreas()

        If product.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
        Else
            dt = product.ResultDataSet.Tables("StadiumStandAreas")
        End If
        ddlArea.Items.Clear()
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Dim rowsArea() As DataRow
            rowsArea = dt.Select("STANDCODE='" & standSelected & "'", "AREATEXT ASC")
            If rowsArea.Length > 0 Then
                AddDDLOption(ddlArea, _wfrPage.Content("PleaseSelectOptionText", _languageCode, True), "-1")
                For Each row As DataRow In rowsArea
                    AddDDLOption(ddlArea, row("AREATEXT"), row("AREACODE"))
                Next
            End If
        End If
    End Sub

    Private Function GetStadiumCode() As String
        Dim dc As Dictionary(Of String, String) = Session("StadiumDictionary")
        Dim stadiumCode As String = String.Empty
        If (Not ddlEvent.SelectedValue = String.Empty AndAlso Not ddlEvent.SelectedValue = "-1") Then
            stadiumCode = dc(ddlEvent.SelectedValue)
        End If
        Return stadiumCode
    End Function

    Private Sub LoadDDL_DeliveryMethod()
        Dim err As New ErrorObj
        Dim dt As New DataTable
        Dim utilitites As New TalentUtiltities
        utilitites.DescriptionKey = "FULT"
        utilitites.Settings = GetSettings()
        err = utilitites.RetrieveDescriptionEntries()
        If utilitites.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
        Else
            dt = utilitites.ResultDataSet.Tables("DescriptionsEntries")
        End If

        ddlDeliveryMethod.Items.Clear()
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            AddDDLOption(ddlDeliveryMethod, _wfrPage.Content("PleaseSelectOptionText", _languageCode, True), "-1")
            For Each row As DataRow In dt.Rows
                AddDDLOption(ddlDeliveryMethod, row("DESCRIPTION"), row("CODE"))
            Next
            ddlDeliveryMethod.SelectedValue = _wfrPage.Attribute("DefaultDeliveryMethod")
        End If
    End Sub

    Private Sub LoadDDL_Countries()
        Dim err As New ErrorObj
        Dim dt As New DataTable
        Dim utilitites As New TalentUtiltities
        utilitites.DescriptionKey = "RCTY"
        utilitites.Settings = GetSettings()
        err = utilitites.RetrieveDescriptionEntries()
        If utilitites.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
        Else
            dt = utilitites.ResultDataSet.Tables("DescriptionsEntries")
        End If

        ddlCountry.Items.Clear()
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            AddDDLOption(ddlCountry, _wfrPage.Content("PleaseSelectOptionText", _languageCode, True), "-1")
            For Each row As DataRow In dt.Rows
                AddDDLOption(ddlCountry, row("DESCRIPTION"), row("CODE"))
            Next
            ddlCountry.SelectedValue = _wfrPage.Attribute("DefaultCountryMethod")
        End If
    End Sub

    Private Sub LoadDDL_Postcodes()
        Dim err As New ErrorObj
        Dim dt As New DataTable
        Dim utilitites As New TalentUtiltities
        utilitites.DescriptionKey = "RPCD"
        utilitites.Settings = GetSettings()
        err = utilitites.RetrieveDescriptionEntries()
        If utilitites.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
        Else
            dt = utilitites.ResultDataSet.Tables("DescriptionsEntries")
        End If

        ddlPostcode.Items.Clear()
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            AddDDLOption(ddlPostcode, _wfrPage.Content("PleaseSelectOptionText", _languageCode, True), "-1")
            For Each row As DataRow In dt.Rows
                AddDDLOption(ddlPostcode, row("DESCRIPTION"), row("CODE"))
            Next
            ddlPostcode.SelectedValue = _wfrPage.Attribute("DefaultPostcodeMethod")
        End If
    End Sub

    Private Sub LoadDDL_SaleAgent()
        Dim talAgent As New TalentAgent
        Dim err As New ErrorObj
        Dim agents As New DataTable
        Dim agentDataEntity As New DEAgent

        If plhSaleAgent.Visible Then
            talAgent.Settings = TEBUtilities.GetSettingsObject()
            agentDataEntity.Source = GlobalConstants.SOURCE
            talAgent.AgentDataEntity = agentDataEntity
            err = talAgent.RetrieveAllAgents()

            ddlSaleAgent.Items.Clear()

            If Not err.HasError AndAlso talAgent.ResultDataSet IsNot Nothing AndAlso talAgent.ResultDataSet.Tables("ErrorStatus").Rows.Count > 0 And
                Not talAgent.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG And
                    talAgent.ResultDataSet.Tables("AgentUsers").Rows.Count() > 0 Then
                agents = talAgent.ResultDataSet.Tables("AgentUsers")
                ddlSaleAgent.DataSource = agents
                ddlSaleAgent.DataTextField = "USERNAME"
                ddlSaleAgent.DataValueField = "USERCODE"
                ddlSaleAgent.DataBind()

                Dim lstitem As New ListItem
                lstitem.Text = _wfrPage.Content("PleaseSelectOptionText", _languageCode, True)
                lstitem.Value = "-1"
                ddlSaleAgent.Items.Insert(0, lstitem)

                ddlSaleAgent.SelectedValue = AgentProfile.Name
                ddlSaleAgent.SelectedValue = "-1"
            End If
        End If

    End Sub

    Private Sub LoadDDL_PayMeth()
        Dim err As New ErrorObj
        Dim dt As New DataTable
        Dim utilitites As New TalentUtiltities

        If plhPayMeth.Visible Then
            utilitites.DescriptionKey = "PAYT"
            utilitites.Settings = GetSettings()
            err = utilitites.RetrieveDescriptionEntries()
            If utilitites.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
            Else
                dt = utilitites.ResultDataSet.Tables("DescriptionsEntries")
            End If

            ddlPayMeth.Items.Clear()

            Dim payMethList As New List(Of ListItem)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                For Each row As DataRow In dt.Rows
                    If IsNumeric(row("CODE")) Then
                        AddListOption(payMethList, row("DESCRIPTION"), row("CODE"))
                    ElseIf IsNumeric(row("VALUES")) Then
                        If CType(row("VALUES"), Integer) <> 0 Then
                            AddListOption(payMethList, row("DESCRIPTION"), row("VALUES"))
                        End If
                    End If
                Next
                payMethList.Sort(Function(x, y) x.Text.CompareTo(y.Text))

                Dim lstitem As New ListItem
                lstitem.Text = _wfrPage.Content("PleaseSelectOptionText", _languageCode, True)
                lstitem.Value = "-1"
                payMethList.Insert(0, lstitem)

                ddlPayMeth.DataSource = payMethList
                ddlPayMeth.DataTextField = "Text"
                ddlPayMeth.DataValueField = "Value"
                ddlPayMeth.DataBind()
                ddlPayMeth.SelectedValue = "-1"
            End If
        End If
    End Sub

    ''' <summary>
    ''' Add the option to the drop down list
    ''' </summary>
    ''' <param name="ddl">The drop down list to add the option to</param>
    ''' <param name="optionText">The text value</param>
    ''' <param name="optionValue">The select value</param>
    ''' <remarks></remarks>
    ''' 
    Private Sub AddDDLOption(ByRef ddl As DropDownList, ByVal optionText As String, ByVal optionValue As String)
        If Not String.IsNullOrEmpty(optionText) Then
            Dim lstitem As New ListItem
            lstitem.Text = optionText.Trim
            lstitem.Value = optionValue.Trim
            ddl.Items.Add(lstitem)
            lstitem = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Add the option to a list
    ''' </summary>
    ''' <param name="list">The list to add the option to</param>
    ''' <param name="optionText">The text value</param>
    ''' <param name="optionValue">The select value</param>
    ''' <remarks></remarks>
    ''' 
    Private Sub AddListOption(ByRef list As List(Of ListItem), ByVal optionText As String, ByVal optionValue As String)
        If Not String.IsNullOrEmpty(optionText) Then
            Dim lstitem As New ListItem
            lstitem.Text = optionText.Trim
            lstitem.Value = optionValue.Trim
            list.Add(lstitem)
            lstitem = Nothing
        End If
    End Sub

    Private Sub SetStadiumDictionary(dt As DataTable)
        Dim dc As New Dictionary(Of String, String)
        For Each r As DataRow In dt.Rows
            Dim productCode As String = r.Item(1).ToString().Trim
            Dim stadiumCode As String = r.Item(0).ToString().Trim
            If Not dc.ContainsKey(productCode) Then dc.Add(productCode, stadiumCode)
        Next
        Session("StadiumDictionary") = dc
    End Sub

    Private Sub RetrieveDespatchTransactionItems()
        Dim err As New ErrorObj
        Dim despatch As New TalentDespatch
        despatch.Settings = GetSettings()
        Dim deDespatch As New DEDespatch
        deDespatch.Type = ""
        deDespatch.FromDate = ""
        deDespatch.ToDate = ""
        deDespatch.SuperType = ""
        deDespatch.SubType = ""
        deDespatch.Product = ""
        deDespatch.SeatDetails.Stand = ""
        deDespatch.SeatDetails.Area = ""
        deDespatch.PaymentRef = 0
        deDespatch.DeliveryMethod = ""
        deDespatch.Country = ""
        deDespatch.Postcode = ""
        deDespatch.PaymentMethod = ""
        deDespatch.AgentName = ""
        If Me.ddlType.SelectedValue <> "-1" Then deDespatch.Type = Me.ddlType.SelectedValue
        If Me.txtDateFrom.Text <> "" Then deDespatch.FromDate = Utilities.DateToIseriesFormat(txtDateFrom.Text)
        If Me.txtDateTo.Text <> "" Then deDespatch.ToDate = Utilities.DateToIseriesFormat(Me.txtDateTo.Text)
        If Me.ddlCategory.SelectedValue <> "-1" Then deDespatch.SuperType = Me.ddlCategory.SelectedValue
        If Me.ddlSubCategory.SelectedValue <> "-1" Then deDespatch.SubType = Me.ddlSubCategory.SelectedValue
        If Me.ddlEvent.SelectedValue <> "-1" Then deDespatch.Product = Me.ddlEvent.SelectedValue
        If Me.ddlStand.SelectedValue <> "-1" Then deDespatch.SeatDetails.Stand = Me.ddlStand.SelectedValue
        If Me.ddlArea.SelectedValue <> "-1" Then deDespatch.SeatDetails.Area = Me.ddlArea.SelectedValue
        If Me.txtPaymentRef.Text <> "" Then deDespatch.PaymentRef = CType(Me.txtPaymentRef.Text, Long)
        If Me.ddlDeliveryMethod.SelectedValue <> "-1" Then deDespatch.DeliveryMethod = Me.ddlDeliveryMethod.SelectedValue
        If Me.ddlCountry.SelectedValue <> "-1" Then deDespatch.Country = Me.ddlCountry.SelectedValue
        If Me.ddlPostcode.SelectedValue <> "-1" Then deDespatch.Postcode = Me.ddlPostcode.SelectedValue
        If Me.ddlSaleAgent.SelectedValue <> "-1" Then deDespatch.AgentName = Me.ddlSaleAgent.SelectedValue
        If Me.ddlPayMeth.SelectedValue <> "-1" Then deDespatch.PaymentMethod = Me.ddlPayMeth.SelectedValue
        If Me.cbIncompleteTransactionsOnly.Checked Then
            deDespatch.TransactionType = "I"
        Else
            deDespatch.TransactionType = "A"
        End If
        If Me.cbGiftWrap.Checked Then
            deDespatch.GiftWrap = "Y"
        Else
            deDespatch.GiftWrap = "N"
        End If
        deDespatch.StrictSearch = Not cbIncludeAdditionalTickets.Checked

        deDespatch.IncludeGenericTransactions = Utilities.CheckForDBNull_Boolean_DefaultTrue(_wfrPage.Attribute("IncludeGenericTransactions"))
        deDespatch.CommandTimeout = Utilities.CheckForDBNull_Int(_wfrPage.Attribute("SearchCommandTimeout"))
        deDespatch.AttributeCategory = Utilities.CheckForDBNull_String(_wfrPage.Attribute("AttributeCategory"))
        despatch.DeDespatch = deDespatch
        despatch.Settings.Cacheing = False
        err = despatch.RetrieveDespatchTransactionItems()

        If Not err.HasError AndAlso despatch.ResultDataSet IsNot Nothing AndAlso despatch.ResultDataSet.Tables("StatusResults").Rows.Count > 0 Then
            If despatch.ResultDataSet.Tables("StatusResults").Rows(0)("ReturnCode") = GlobalConstants.ERRORFLAG Then
                ClearAll()
                Dim talentErrorMsg As TalentErrorMessage = _errMessage.GetErrorMessage(despatch.ResultDataSet.Tables("StatusResults").Rows(0)("ErrorOccurred"))
                Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(talentErrorMsg.ERROR_MESSAGE)
                If listItemObject Is Nothing Then blErrorMessages.Items.Add(talentErrorMsg.ERROR_MESSAGE)
            Else
                SetRepeaterColumnHeaders()
                Dim dtTransactionsItems As Data.DataTable = despatch.ResultDataSet.Tables("DespatchTransactionsItems")
                Session("dtDNGResults") = dtTransactionsItems
                BindRepeater()
            End If
        Else
            ClearAll()
            Dim errorString As String = err.ErrorStatus + " (" + err.ErrorNumber + ": " + err.ErrorMessage + ")"
            Dim listItemObject As ListItem = blErrorMessages.Items.FindByText(errorString)
            If listItemObject Is Nothing Then blErrorMessages.Items.Add(errorString)
        End If
    End Sub

    Private Sub SetRepeaterColumnHeaders()
        If ddlType.SelectedValue = "2" Then
            ColumnHeaderText_Ref = _wfrPage.Content("ColumnHeaderReservationRef", _languageCode, True)
        Else
            ColumnHeaderText_Ref = _wfrPage.Content("ColumnHeaderPaymentRef", _languageCode, True)
        End If
        ColumnHeaderText_Status = _wfrPage.Content("ColumnHeaderStatus", _languageCode, True)
        ColumnHeaderText_Customer = _wfrPage.Content("ColumnHeaderCustomer", _languageCode, True)
        ColumnHeaderText_Quantity = _wfrPage.Content("ColumnHeaderQuantity", _languageCode, True)
        ColumnHeaderText_Select = _wfrPage.Content("ColumnHeaderSelect", _languageCode, True)
    End Sub

    Private Sub RetrieveDespatchRetailItems()
        'TODO
        rptTransaction.DataSource = Nothing
        rptTransaction.DataBind()
    End Sub

    Private Function GetSettings() As DESettings
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        settings.Cacheing = TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_wfrPage.Attribute("Cacheing"))
        settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_wfrPage.Attribute("CacheTimeMinutes"))
        Return settings
    End Function

    Private Sub ClearAll()
        Session("dtDNGResults") = Nothing
        blSuccessMessages.Items.Clear()
        blErrorMessages.Items.Clear()
        Dim dtEmpty As New Data.DataTable
        rptTransaction.DataSource = dtEmpty
        rptTransaction.DataBind()
    End Sub

    Private Function RetrieveGeographicalZones() As Data.DataTable
        Dim err As New ErrorObj
        Dim dt As New DataTable
        Dim utilitites As New TalentUtiltities
        utilitites.DescriptionKey = "CZON"
        utilitites.Settings = GetSettings()
        err = utilitites.RetrieveDescriptionEntries()
        If utilitites.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
        Else
            dt = utilitites.ResultDataSet.Tables("DescriptionsEntries")
        End If

        Return dt
    End Function
    ''' <summary>
    ''' Retrieves all payment references transactions items repeater for WS633R DespatchTicketPrint call
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getPaymentReferencesFromTransactions() As List(Of String)
        Dim paymentReferences As New List(Of String)
        For Each rptTransactionItem As RepeaterItem In rptTransaction.Items
            Dim chkSelectedItem As CheckBox = CType(rptTransactionItem.FindControl("chkSelectedItem"), CheckBox)
            If chkSelectedItem.Checked Then
                Dim hdfTransactionRef As HiddenField = CType(rptTransactionItem.FindControl("hdfTransactionRef"), HiddenField)
                paymentReferences.Add(hdfTransactionRef.Value)
            End If
        Next
        Return paymentReferences
    End Function

#End Region


End Class
