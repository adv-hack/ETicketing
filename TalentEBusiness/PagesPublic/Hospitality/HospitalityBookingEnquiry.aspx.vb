Imports System.Data
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports TalentBusinessLogic.DataTransferObjects.Hospitality
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.ModelBuilders.Hospitality
Imports TalentBusinessLogic.ModelBuilders.Hospitality.Booking
Imports TalentBusinessLogic.Models.Hospitality.Booking
Imports System.Collections.Generic

Partial Class PagesPublic_Hospitality_HospitalityBookingEnquiry
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _viewModelHospitalityBookingEnquiry As HospitalityBookingEnquiryViewModel = Nothing
    Private _inputModelHospitalityBookingEnquiry As HospitalityBookingEnquiryInputModel
    Private _bookingEnquiryBuilder As HospitalityBookingEnquiryBuilder = Nothing
    Private _businessUnit As String = String.Empty
    Private _partner As String = String.Empty
    Private _isFilterApplicable As Boolean = False
    Private _defaults As ECommerceModuleDefaults.DefaultValues = Nothing
    Private _moduleDefaults As ECommerceModuleDefaults = Nothing

#End Region

#Region "Public Properties"

    Public CallIDColumnHeading As String
    Public PackageColumnHeading As String
    Public ProductColumnHeading As String
    Public AgentColumnHeading As String
    Public DateColumnHeading As String
    Public ValueColumnHeading As String
    Public StatusColumnHeading As String
    Public QandAStatusColumnHeading As String
    Public PrintStatusColumnHeading As String

    Public CallIDFilterVisible As Boolean
    Public PackageFilterVisible As Boolean
    Public ProductFilterVisible As Boolean
    Public AgentFilterVisible As Boolean
    Public FromDateFilterVisible As Boolean
    Public ToDateFilterVisible As Boolean
    Public StatusFilterVisible As Boolean
    Public ProductGroupFilterVisible As Boolean
    Public CustomerFilterVisible As Boolean

    Public PrintSingleBookingTitleText As String
    Public SingleBookingDocumentTitleText As String

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "Hospitality-Booking-Enquiry.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("Hospitality-Booking-Enquiry.js", "/Module/Hospitality/"), False)

        'Check if agent has access for Hospitality Functions
        If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessHospitalityBookingEnquiry) Or Not AgentProfile.IsAgent Then
            _businessUnit = TalentCache.GetBusinessUnit()
            _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
            _moduleDefaults = New ECommerceModuleDefaults
            _defaults = _moduleDefaults.GetDefaults()
            _bookingEnquiryBuilder = New HospitalityBookingEnquiryBuilder
            populateAgents()
            populateBookingStatus()
            populateMarkForOrder()
            populateQandAStatus()
            populatePrintStatus()
            If Not IsPostBack Then
                If Profile.User.Details IsNot Nothing Then
                    txtCustomer.Text = Profile.User.Details.LoginID.TrimStart("0"c)
                End If
                setFormValues()
                _inputModelHospitalityBookingEnquiry = setupHospitalityBookingEnquiryInputModel(_isFilterApplicable)
                processController(_inputModelHospitalityBookingEnquiry)
                createView()
            End If
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If

        rgvFromDate.MinimumValue = "01/01/1900"
        rgvFromDate.MaximumValue = DateTime.Today.ToString(ModuleDefaults.GlobalDateFormat)
        rgvToDate.MinimumValue = "01/01/1900"
        rgvToDate.MaximumValue = DateTime.Today.ToString(ModuleDefaults.GlobalDateFormat)
        revBookingReference.ValidationExpression = _viewModelHospitalityBookingEnquiry.GetPageAttribute("BookingRefRegex")
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then
            If hdfForwardingUrl.Value IsNot String.Empty Then
                Response.Redirect(hdfForwardingUrl.Value)
            End If
            If hdfSendQAndAReminder.Value = "True" Then
                lbtnSendQAndAReminder_Click(sender, e)
            End If
            If hdfPrintBookings.Value = "True" Then
                lbtnPrintAllTickets_Click(sender, e)
            End If
            If hdfPrintSingleBooking.Value = "True" Then
                _isFilterApplicable = True
                setSessionValues()
                _inputModelHospitalityBookingEnquiry = setupHospitalityBookingEnquiryInputModel(_isFilterApplicable)
                _inputModelHospitalityBookingEnquiry.LoggedInBoxOfficeUser = AgentProfile.Name
                processController(_inputModelHospitalityBookingEnquiry)
                createView()
            End If
            If hdfGenerateBookingDocument.Value = "True" Then
                _isFilterApplicable = True
                setSessionValues()
                _inputModelHospitalityBookingEnquiry = setupHospitalityBookingEnquiryInputModel(_isFilterApplicable)
                _inputModelHospitalityBookingEnquiry.LoggedInBoxOfficeUser = AgentProfile.Name
                processController(_inputModelHospitalityBookingEnquiry)
                createView()
            End If
            If hdfCreatePDFForBooking.Value = "True" Then
                _isFilterApplicable = True
                setSessionValues()
                _inputModelHospitalityBookingEnquiry = setupHospitalityBookingEnquiryInputModel(_isFilterApplicable)
                _inputModelHospitalityBookingEnquiry.LoggedInBoxOfficeUser = AgentProfile.Name
                processController(_inputModelHospitalityBookingEnquiry)
                createView()
            End If
        Else
            If Not HttpContext.Current.Session("PDFUrl") Is Nothing Then
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "openPDFScript", "javascript:window.open('" & ResolveUrl(HttpContext.Current.Session("PDFUrl")) & "', '_blank');", True)
                HttpContext.Current.Session("PDFUrl") = Nothing
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        plhBookingListErrors.Visible = (ltlBookingListErrors.Text.Length > 0)
        plhSendQAndAReminderSuccess.Visible = (ltlSendQAndAReminderSuccessText.Text.Length > 0)
        plhPrintAllSuccess.Visible = (ltlPrintAllSubmitSuccessText.Text.Length > 0)
        plhPrintBookingSuccess.Visible = (ltlPrintBookingSuccessText.Text.Length > 0)
        plhDocumentGenerationSuccess.Visible = (ltlDocumentGenerationSuccessText.Text.Length > 0)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        _isFilterApplicable = True
        setSessionValues()
        _inputModelHospitalityBookingEnquiry = setupHospitalityBookingEnquiryInputModel(_isFilterApplicable)
        processController(_inputModelHospitalityBookingEnquiry)
        createView()
    End Sub

    Protected Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        _isFilterApplicable = False
        clearSessionValues()
        clearFilterValues()
        _inputModelHospitalityBookingEnquiry = setupHospitalityBookingEnquiryInputModel(_isFilterApplicable)
        processController(_inputModelHospitalityBookingEnquiry)
        createView()
    End Sub

    Protected Sub rptHospitalityBookings_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptHospitalityBookings.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim bookingEnquiryDetails As HospitalityBookingEnquiryDetails = CType(e.Item.DataItem, HospitalityBookingEnquiryDetails)
            Dim plhViewBooking As PlaceHolder = CType(e.Item.FindControl("plhViewBooking"), PlaceHolder)
            Dim plhPrintSingleBooking As PlaceHolder = CType(e.Item.FindControl("plhPrintSingleBooking"), PlaceHolder)
            Dim plhGenerateDocumentForBooking As PlaceHolder = CType(e.Item.FindControl("plhGenerateDocumentForBooking"), PlaceHolder)
            Dim plhGeneratePDFForBooking As PlaceHolder = CType(e.Item.FindControl("plhGeneratePDFForBooking"), PlaceHolder)
            Dim lbtnGeneratePDFForBooking As LinkButton = CType(e.Item.FindControl("lbtnGeneratePDFForBooking"), LinkButton)
            Dim printStatus As String = "PrintStatus-"

            If bookingEnquiryDetails.Status = GlobalConstants.SOLD_BOOKING_STATUS Then
                plhGenerateDocumentForBooking.Visible = Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("ShowGenerateDocumentIcon", False))
                plhGeneratePDFForBooking.Visible = Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("ShowGeneratePDFIcon", False))
                If plhGeneratePDFForBooking.Visible Then
                    lbtnGeneratePDFForBooking.Attributes.Add("title", _viewModelHospitalityBookingEnquiry.GetPageText("GeneratePDFForBookingText"))
                    lbtnGeneratePDFForBooking.OnClientClick = "CreatePDFForBookingClick('" & ResolveUrl(bookingEnquiryDetails.PDFForwardingUrl) & "','" & bookingEnquiryDetails.RequiresLogin & "');"
                    If bookingEnquiryDetails.RequiresLogin Then
                        lbtnGeneratePDFForBooking.OnClientClick &= "return false;"
                    Else
                        lbtnGeneratePDFForBooking.OnClientClick &= "return true;"
                    End If
                End If
            Else
                plhGenerateDocumentForBooking.Visible = False
                plhGeneratePDFForBooking.Visible = False
            End If
            setRowDataTitles(e)
            plhViewBooking.Visible = (bookingEnquiryDetails.Status = GlobalConstants.ENQUIRY_BOOKING_STATUS Or bookingEnquiryDetails.Status = GlobalConstants.SOLD_BOOKING_STATUS Or bookingEnquiryDetails.Status = GlobalConstants.RESERVATION_BOOKING_STATUS)
            plhPrintSingleBooking.Visible = AgentProfile.AgentPermissions.CanPrintHospitalityTickets AndAlso (bookingEnquiryDetails.Status = GlobalConstants.SOLD_BOOKING_STATUS And bookingEnquiryDetails.PrintStatus <> _viewModelHospitalityBookingEnquiry.GetPageText(printStatus & GlobalConstants.NA_STATUS))
        End If
    End Sub

    Protected Sub rptHospitalityBookings_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptHospitalityBookings.ItemCommand
        Select Case e.CommandName
            Case "GeneratePDFForBooking"
                If e.CommandName.Length > 0 Then
                    Dim tgateway As New TicketingGatewayFunctions
                    Dim hdfProductCode As HiddenField = CType(e.Item.FindControl("hdfProductCode"), HiddenField)
                    Dim hdfPackageId As HiddenField = CType(e.Item.FindControl("hdfPackageId"), HiddenField)
                    Dim hdfCustomerNumber As HiddenField = CType(e.Item.FindControl("hdfCustomerNumber"), HiddenField)
                    Dim productCode As String = hdfProductCode.Value
                    Dim packageId As String = hdfPackageId.Value
                    Dim customerNumber As String = hdfCustomerNumber.Value
                    Dim callId As String = e.CommandArgument
                    Dim pdfCreator As New CreatePDF
                    Dim pdfPathAndFile As String = String.Empty
                    Dim leadSourceList As New List(Of LeadSourceDetails)
                    Dim hospitalityDetailsContent As String = tgateway.ProcessHospitalityDetailsView(productCode, packageId, callId, leadSourceList)
                    Dim hospitalityBookingContent As String = tgateway.ProcessHospitalityBookingView(Profile.Basket.Basket_Header_ID, customerNumber, productCode, packageId, callId, leadSourceList)
                    Dim cssContent As String = String.Empty
                    Dim htmlContent As String = hospitalityDetailsContent & hospitalityBookingContent
                    Dim hasError As Boolean = False

                    If htmlContent.Length > 0 Then
                        Dim filePath As String = ModuleDefaults.HtmlPathAbsolute
                        Dim fileName As String = String.Concat(callId.ToString(), "-", Now.ToString("ddMMyyyy-HHmm"), ".pdf")
                        If Not filePath.EndsWith("\") Then filePath &= "\"
                        filePath &= "HospitalityPDF\"
                        cssContent = Talent.eCommerce.Utilities.GetCSSContentFromFile("HospitalityPDF\" & packageId & "_package.css")
                        If cssContent.Length = 0 Then cssContent = Talent.eCommerce.Utilities.GetCSSContentFromFile("HospitalityPDF\package.css")
                        pdfPathAndFile = pdfCreator.CreateFile(fileName, filePath, htmlContent, cssContent)
                        If pdfPathAndFile.Length > 0 Then
                            Dim url As String = "~/Assets"
                            If Not ModuleDefaults.HtmlIncludePathRelative.StartsWith("/") Then url &= "/"
                            url &= ModuleDefaults.HtmlIncludePathRelative
                            If Not url.EndsWith("/") Then url &= "/"
                            url &= "HospitalityPDF/" & fileName
                            HttpContext.Current.Session("PDFUrl") = url
                            Response.Redirect(Request.Url.AbsoluteUri)  'This is required to prevent form resubmit on force refresh
                        Else
                            hasError = True
                        End If
                    Else
                        hasError = True
                    End If
                    If hasError Then
                        ltlBookingListErrors.Text = _viewModelHospitalityBookingEnquiry.GetPageText("ErrorCreatingPDF")
                    End If
                End If
            Case Else
        End Select
    End Sub

    Protected Sub lbtnSendQAndAReminder_Click(sender As Object, e As EventArgs) Handles lbtnSendQAndAReminder.Click
        If (hdfSendQAndAReminder.Value = True) Then
            _isFilterApplicable = True
            setSessionValues()
            _inputModelHospitalityBookingEnquiry = setupHospitalityBookingEnquiryInputModel(_isFilterApplicable)
            processController(_inputModelHospitalityBookingEnquiry)
            createView()
        End If
    End Sub

    Protected Sub lbtnPrintAllTickets_Click(sender As Object, e As EventArgs) Handles lbtnPrintAllTickets.Click
        If (hdfPrintBookings.Value = True) Then
            _isFilterApplicable = True
            setSessionValues()
            _inputModelHospitalityBookingEnquiry = setupHospitalityBookingEnquiryInputModel(_isFilterApplicable)
            _inputModelHospitalityBookingEnquiry.LoggedInBoxOfficeUser = AgentProfile.Name
            processController(_inputModelHospitalityBookingEnquiry)
            createView()
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Setup the hospitality bookings input model and return the model for use
    ''' </summary>
    ''' <returns>The formatted input model based on form data</returns>
    ''' <remarks></remarks>
    Private Function setupHospitalityBookingEnquiryInputModel(ByRef isFilter As Boolean) As HospitalityBookingEnquiryInputModel
        _viewModelHospitalityBookingEnquiry = New HospitalityBookingEnquiryViewModel(True)
        Dim inputModel As New HospitalityBookingEnquiryInputModel
        If Not AgentProfile.IsAgent Then
            If Profile.IsAnonymous Then
                inputModel.CustomerNumber = String.Empty
            Else
                inputModel.CustomerNumber = Profile.User.Details.LoginID
            End If
        ElseIf txtCustomer.Text IsNot String.Empty Then
            inputModel.CustomerNumber = txtCustomer.Text.ToString().PadLeft(12, "0")
        Else
            inputModel.CustomerNumber = String.Empty
        End If
        If Profile.IsAnonymous Then
            inputModel.LoggedInCustomerNumber = String.Empty
        Else
            inputModel.LoggedInCustomerNumber = Profile.User.Details.LoginID
        End If

        If isFilter Then
            If ddlAgent.SelectedItem.Text = _viewModelHospitalityBookingEnquiry.GetPageText("AllAgentsText") Then inputModel.BoxOfficeUser = String.Empty Else inputModel.BoxOfficeUser = ddlAgent.SelectedItem.Value.Trim()
            If String.IsNullOrEmpty(txtCallID.Text) Then inputModel.CallID = 0 Else inputModel.CallID = txtCallID.Text.Trim()
            If String.IsNullOrEmpty(txtFromdate.Text) Then
                inputModel.FromDate = Talent.Common.Utilities.DateToIseries8Format(Now.AddDays(-30))
            Else
                inputModel.FromDate = Talent.Common.Utilities.DateToIseries8Format(CDate(txtFromdate.Text))
            End If
            If String.IsNullOrEmpty(txtToDate.Text) Then
                inputModel.ToDate = Talent.Common.Utilities.DateToIseries8Format(Now)
            Else
                inputModel.ToDate = Talent.Common.Utilities.DateToIseries8Format(CDate(txtToDate.Text))
            End If
            inputModel.Status = ddlStatus.SelectedItem.Value.Trim()
            If String.IsNullOrEmpty(txtPackage.Text) Then inputModel.Package = String.Empty Else inputModel.Package = txtPackage.Text
            If String.IsNullOrEmpty(txtProduct.Text) Then inputModel.ProductCode = String.Empty Else inputModel.ProductCode = txtProduct.Text
            inputModel.MarkOrderFor = ddlMarkOrderFor.SelectedItem.Value.Trim()
            inputModel.MaxRecords = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(_viewModelHospitalityBookingEnquiry.GetPageAttribute("MaxBookingsToDisplay"))
            inputModel.QandAStatus = ddlQandAStatus.SelectedItem.Value.Trim()
            inputModel.PrintStatus = ddlPrintStatus.SelectedItem.Value.Trim()
        Else
            txtFromdate.Text = Now.AddDays(-30).Date.ToString(ModuleDefaults.GlobalDateFormat)
            txtToDate.Text = Now.Date.ToString(ModuleDefaults.GlobalDateFormat)
            inputModel.BoxOfficeUser = String.Empty
            inputModel.CallID = 0
            inputModel.FromDate = Talent.Common.Utilities.DateToIseries8Format(Now.AddDays(-30))
            inputModel.ToDate = Talent.Common.Utilities.DateToIseries8Format(Now)
            inputModel.Status = String.Empty
            inputModel.Package = String.Empty
            inputModel.ProductCode = String.Empty
            inputModel.MarkOrderFor = String.Empty
            inputModel.MaxRecords = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(_viewModelHospitalityBookingEnquiry.GetPageAttribute("MaxBookingsToDisplay"))
            inputModel.QandAStatus = String.Empty
            inputModel.PrintStatus = String.Empty
        End If
        Return inputModel
    End Function

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Process the controller, the code actioning the input values if there are any.
    ''' </summary>
    ''' <param name="inputModelHospitalityBookingEnquiry">Hospitality Booking Enquiry Input Model</param>
    ''' <remarks></remarks>
    Private Sub processController(ByVal inputModelHospitalityBookingEnquiry As HospitalityBookingEnquiryInputModel)
        _viewModelHospitalityBookingEnquiry = _bookingEnquiryBuilder.RetrieveHospitalityBookings(inputModelHospitalityBookingEnquiry)
        If IsPostBack Then
            If hdfSendQAndAReminder.Value = "True" Then
                Dim callidsarray() As String = hdfListOfCallIds.Value.ToString().Split(",")
                _inputModelHospitalityBookingEnquiry.CallIdList = New List(Of String)(callidsarray)
                _viewModelHospitalityBookingEnquiry = _bookingEnquiryBuilder.SendQAndAReminder(inputModelHospitalityBookingEnquiry)
                hdfSendQAndAReminder.Value = False
                HttpContext.Current.Session("SuccessfullySentQAndAReminder") = _viewModelHospitalityBookingEnquiry.SuccessfullySentQAndAReminder
                If Not _viewModelHospitalityBookingEnquiry.SuccessfullySentQAndAReminder Then
                    HttpContext.Current.Session("ErrorSentQAndAReminder") = _viewModelHospitalityBookingEnquiry.Error.ErrorMessage
                End If
                Response.Redirect(Request.Url.AbsoluteUri)  'This is required to prevent form resubmit on force refresh
            End If
            If hdfPrintBookings.Value = "True" Then
                _viewModelHospitalityBookingEnquiry = _bookingEnquiryBuilder.PrintHospitalityBookings(_inputModelHospitalityBookingEnquiry)
                hdfPrintBookings.Value = False
                HttpContext.Current.Session("SuccessfullySubmittedPrintAll") = _viewModelHospitalityBookingEnquiry.PrintRequestSuccess
                If Not _viewModelHospitalityBookingEnquiry.PrintRequestSuccess Then
                    HttpContext.Current.Session("ErrorSubmitPrintAll") = _viewModelHospitalityBookingEnquiry.Error.ErrorMessage
                End If
                Response.Redirect(Request.Url.AbsoluteUri)  'This is required to prevent form resubmit on force refresh
            End If
            If hdfPrintSingleBooking.Value = "True" Then
                _inputModelHospitalityBookingEnquiry.CallIdToBePrinted = hdfSelectedCallIdToBePrinted.Value
                HttpContext.Current.Session("NumberOfTicketsInBooking") = hdfNumberOfTicketsInBooking.Value
                _viewModelHospitalityBookingEnquiry = _bookingEnquiryBuilder.PrintHospitalityBookings(_inputModelHospitalityBookingEnquiry)
                hdfPrintSingleBooking.Value = False
                HttpContext.Current.Session("SuccessfullySubmittedPrintBooking") = _viewModelHospitalityBookingEnquiry.PrintRequestSuccess
                If Not _viewModelHospitalityBookingEnquiry.PrintRequestSuccess Then
                    HttpContext.Current.Session("ErrorSubmitPrintBooking") = _viewModelHospitalityBookingEnquiry.Error.ErrorMessage
                End If
                Response.Redirect(Request.Url.AbsoluteUri)  'This is required to prevent form resubmit on force refresh
            End If
            If hdfGenerateBookingDocument.Value = "True" Then
                _inputModelHospitalityBookingEnquiry.CallIdForDocumentProduction = hdfCallIdForDocumentProduction.Value
                HttpContext.Current.Session("CallIdForDocumentProduction") = hdfCallIdForDocumentProduction.Value
                _viewModelHospitalityBookingEnquiry = _bookingEnquiryBuilder.GenerateDocumentForBooking(_inputModelHospitalityBookingEnquiry)
                hdfGenerateBookingDocument.Value = False
                HttpContext.Current.Session("SuccessfullyGeneratedDocumentForBooking") = _viewModelHospitalityBookingEnquiry.GenerateDocumentRequestSuccess
                If Not _viewModelHospitalityBookingEnquiry.GenerateDocumentRequestSuccess Then
                    HttpContext.Current.Session("ErrorGeneratingDocumentForBooking") = _viewModelHospitalityBookingEnquiry.Error.ErrorMessage
                Else
                    Session("MergedDocumentDownloadPath") = _defaults.HospMergedDocumentPathRelative & _viewModelHospitalityBookingEnquiry.MergedWordDocument
                End If
                Response.Redirect(Request.Url.AbsoluteUri)  'This is required to prevent form resubmit on force refresh
            End If
            If hdfCreatePDFForBooking.Value = "True" Then
                hdfCreatePDFForBooking.Value = False
            End If
        End If
    End Sub

    ''' <summary>
    ''' Create the booking page view
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createView()
        If _viewModelHospitalityBookingEnquiry.Error.HasError And IsPostBack Then
            plhBookingList.Visible = False
            ltlBookingListErrors.Text = _viewModelHospitalityBookingEnquiry.Error.ErrorMessage.Replace("<<MaxRecords>>", _viewModelHospitalityBookingEnquiry.GetPageAttribute("MaxBookingsToDisplay"))
        Else
            setColumnHeadingsAndVisibility()
            rptHospitalityBookings.DataSource = _viewModelHospitalityBookingEnquiry.HospitalityBookingEnquiryList
            rptHospitalityBookings.DataBind()
        End If
        setFiltersVisibility()
        setPageLabels()
        setPageHiddenFields()
        If HttpContext.Current.Session("SuccessfullySentQAndAReminder") = True Then
            plhSendQAndAReminderSuccess.Visible = True
            ltlSendQAndAReminderSuccessText.Text = _viewModelHospitalityBookingEnquiry.GetPageText("SendQAndAReminderSuccessText")
            HttpContext.Current.Session("SuccessfullySentQAndAReminder") = Nothing
        End If
        If Not HttpContext.Current.Session("ErrorSentQAndAReminder") Is Nothing Then
            plhBookingListErrors.Visible = True
            ltlBookingListErrors.Text = HttpContext.Current.Session("ErrorSentQAndAReminder")
            HttpContext.Current.Session("ErrorSentQAndAReminder") = Nothing
        End If
        If HttpContext.Current.Session("SuccessfullySubmittedPrintAll") = True Then
            plhPrintAllSuccess.Visible = True
            ltlPrintAllSubmitSuccessText.Text = _viewModelHospitalityBookingEnquiry.GetPageText("PrintAllSubmitSuccessText").Replace("<<NumberOfTicketsToPrint>>", _viewModelHospitalityBookingEnquiry.NumberOfTicketsToPrint)
            HttpContext.Current.Session("SuccessfullySubmittedPrintAll") = Nothing
        End If
        If Not HttpContext.Current.Session("ErrorSubmitPrintAll") Is Nothing Then
            plhBookingListErrors.Visible = True
            ltlBookingListErrors.Text = HttpContext.Current.Session("ErrorSubmitPrintAll")
            HttpContext.Current.Session("ErrorSubmitPrintAll") = Nothing
        End If
        If HttpContext.Current.Session("SuccessfullySubmittedPrintBooking") = True Then
            plhPrintBookingSuccess.Visible = True
            ltlPrintBookingSuccessText.Text = _viewModelHospitalityBookingEnquiry.GetPageText("PrintBookingSubmitSuccessText").Replace("<<NumberOfTicketsToPrint>>", HttpContext.Current.Session("NumberOfTicketsInBooking"))
            HttpContext.Current.Session("SuccessfullySubmittedPrintBooking") = Nothing
            HttpContext.Current.Session("NumberOfTicketsInBooking") = Nothing
        End If
        If Not HttpContext.Current.Session("ErrorSubmitPrintBooking") Is Nothing Then
            plhBookingListErrors.Visible = True
            ltlBookingListErrors.Text = HttpContext.Current.Session("ErrorSubmitPrintBooking")
            HttpContext.Current.Session("ErrorSubmitPrintBooking") = Nothing
        End If
        If HttpContext.Current.Session("SuccessfullyGeneratedDocumentForBooking") = True Then
            plhDocumentGenerationSuccess.Visible = True
            ltlDocumentGenerationSuccessText.Text = _viewModelHospitalityBookingEnquiry.GetPageText("CreateBookingDocumentSuccessText").Replace("<<CallId>>", HttpContext.Current.Session("CallIdForDocumentProduction"))
            HttpContext.Current.Session("SuccessfullyGeneratedDocumentForBooking") = Nothing
            HttpContext.Current.Session("CallIdForDocumentProduction") = Nothing
        End If
        If Not HttpContext.Current.Session("ErrorGeneratingDocumentForBooking") Is Nothing Then
            plhBookingListErrors.Visible = True
            ltlBookingListErrors.Text = HttpContext.Current.Session("ErrorGeneratingDocumentForBooking")
            HttpContext.Current.Session("ErrorGeneratingDocumentForBooking") = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Setup the various hidden fields based on the view model
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setPageHiddenFields()
        hdfForwardingUrl.Value = String.Empty
        hdfDataTablesInfoEmpty.Value = _viewModelHospitalityBookingEnquiry.GetPageText("EmptyDataTableWarningText")

        hdfAlertifyCancel.Value = _viewModelHospitalityBookingEnquiry.GetPageText("LoginConfirmationCancelButtonText")
        hdfAlertifyMessage.Value = _viewModelHospitalityBookingEnquiry.GetPageText("LoginConfirmationBoxText")
        hdfAlertifyOK.Value = _viewModelHospitalityBookingEnquiry.GetPageText("LoginConfirmationOkButtonText")
        hdfAlertifyTitle.Value = _viewModelHospitalityBookingEnquiry.GetPageText("LoginConfirmationBoxTitle")

        hdfAlertifyCancelForQAndAReminder.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyCancelForQAndAReminder")
        hdfAlertifyMessageForQAndAReminder.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyMessageForQAndAReminder")
        hdfAlertifyOKForQAndAReminder.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyOkForQAndAReminder")
        hdfAlertifyTitleForQAndAReminder.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyTitleForQAndAReminder")
        hdfQAndAReminderDisabledMessage.Value = _viewModelHospitalityBookingEnquiry.GetPageText("QAndAReminderDisabledMessage")

        hdfAlertifyCancelForPrintAll.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyCancelForPrintAll")
        hdfAlertifyMessageForPrintAll.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyMessageForPrintAll").Replace("<<NumberOfTicketsToPrint>>", _viewModelHospitalityBookingEnquiry.NumberOfTicketsToPrint).Replace("<<NumberOfBookings>>", _viewModelHospitalityBookingEnquiry.NumberOfBookings)
        hdfAlertifyOKForPrintAll.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyOkForPrintAll")
        hdfAlertifyTitleForPrintAll.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyTitleForPrintAll")
        hdfPrintAllDisabledMessage.Value = _viewModelHospitalityBookingEnquiry.GetPageText("PrintAllDisabledMessage")

        hdfAlertifyTitleForPrintSingleBooking.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyTitleForPrintBooking")
        hdfAlertifyMessageForPrintSingleBooking.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyMessageForPrintBooking")
        hdfAlertifyOKForPrintSingleBooking.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyOkForPrintBooking")
        hdfAlertifyCancelForPrintSingleBooking.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyCancelForPrintBooking")

        hdfPrintStatusNotPrinted.Value = _viewModelHospitalityBookingEnquiry.GetPageText("PrintStatus-" + GlobalConstants.NOT_PRINTED_STATUS)
        hdfPrintStatusPartiallyPrinted.Value = _viewModelHospitalityBookingEnquiry.GetPageText("PrintStatus-" + GlobalConstants.PARTIALLY_PRINTED_STATUS)
        hdfPrintStatusFullyPrinted.Value = _viewModelHospitalityBookingEnquiry.GetPageText("PrintStatus-" + GlobalConstants.FULLY_PRINTED_STATUS)
        hdfBookingStatus.Value = _viewModelHospitalityBookingEnquiry.GetPageText("BookingStatus-" + GlobalConstants.SOLD_BOOKING_STATUS)

        hdfAlertifyTitleForSingleBookingDocument.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyTitleForBookingDocument")
        hdfAlertifyMessageForSingleBookingDocument.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyMessageForBookingDocument")
        hdfAlertifyOKForSingleBookingDocument.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyOkForBookingDocument")
        hdfAlertifyCancelForSingleBookingDocument.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyCancelForBookingDocument")

        hdfAlertifyTitleForCreatePDF.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyTitleForCreatePDF")
        hdfAlertifyMessageForCreatePDF.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyMessageForCreatePDF")
        hdfAlertifyOKForCreatePDF.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyOkForCreatePDF")
        hdfAlertifyCancelForCreatePDF.Value = _viewModelHospitalityBookingEnquiry.GetPageText("AlertifyCancelForCreatePDF")

        Dim hideColumns As New List(Of Integer)()
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("callidcolumnvisible")) Then hideColumns.Add(0)
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("packagecolumnvisible")) Then hideColumns.Add(1)
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("productcolumnvisible")) Then hideColumns.Add(2)
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("agentcolumnvisible")) Then hideColumns.Add(3)
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("datecolumnvisible")) Then hideColumns.Add(4)
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("valuecolumnvisible")) Then hideColumns.Add(5)
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("statuscolumnvisible")) Then hideColumns.Add(6)
        If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("qandastatuscolumnvisible")) Then hideColumns.Add(7)
        If AgentProfile.IsAgent Then
            If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("printstatuscolumnvisible")) Then hideColumns.Add(8)
        Else
            hideColumns.Add(8) ' Always hide Print column for PWS
        End If

        ' Always hide the work field column for CYYMMDD date
        hideColumns.Add(9)
        hdfHideColumns.Value = String.Join(",", hideColumns.ToArray())
        hdfSendQAndAReminder.Value = String.Empty
        hdfPrintBookings.Value = String.Empty

        If Session("MergedDocumentDownloadPath") IsNot Nothing Then
            hdfMergedDocumentPath.Value = Session("MergedDocumentDownloadPath")
            Session("MergedDocumentDownloadPath") = Nothing
        End If

    End Sub

    ''' <summary>
    ''' Setup the various text labels on the screen based on the view model
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setPageLabels()
        If AgentProfile.IsAgent Then
            If plhAgent.Visible Then
                lblAgent.Text = _viewModelHospitalityBookingEnquiry.GetPageText("AgentLabelText")
            End If
            If plhCustomer.Visible Then
                lblCustomer.Text = _viewModelHospitalityBookingEnquiry.GetPageText("CustomerLabelText")
            End If
            If plhSendQAndAReminder.Visible Then
                lbtnSendQAndAReminder.Text = lbtnSendQAndAReminder.Text + " " + _viewModelHospitalityBookingEnquiry.GetPageText("SendQAndAReminderEMailText")
            End If
            If plhPrintStatus.Visible Then
                lblPrintStatus.Text = _viewModelHospitalityBookingEnquiry.GetPageText("PrintStatus")
            End If
            If plhPrintAllTickets.Visible Then
                lbtnPrintAllTickets.Text = lbtnPrintAllTickets.Text + " " + _viewModelHospitalityBookingEnquiry.GetPageText("PrintAllTicketsLinkButtonText")
            End If
        End If

        lblCallID.Text = _viewModelHospitalityBookingEnquiry.GetPageText("CallIDLabelText")
        lblFromdate.Text = _viewModelHospitalityBookingEnquiry.GetPageText("FromDateLabelText")
        lblToDate.Text = _viewModelHospitalityBookingEnquiry.GetPageText("ToDateLabelText")
        lblStatus.Text = _viewModelHospitalityBookingEnquiry.GetPageText("StatusLabelText")
        lblPackage.Text = _viewModelHospitalityBookingEnquiry.GetPageText("PackageLabelText")
        lblProduct.Text = _viewModelHospitalityBookingEnquiry.GetPageText("ProductLabelText")
        lblProductGroup.Text = _viewModelHospitalityBookingEnquiry.GetPageText("ProductGroupLabelText")
        lblMarkOrderFor.Text = _viewModelHospitalityBookingEnquiry.GetPageText("MarkOrderFor")
        lblQandAStatus.Text = _viewModelHospitalityBookingEnquiry.GetPageText("QandAStatus")
        btnClear.Text = _viewModelHospitalityBookingEnquiry.GetPageText("ClearButtonText")
        btnSearch.Text = _viewModelHospitalityBookingEnquiry.GetPageText("SearchButtonText")

        'Placeholder
        txtCallID.Attributes.Add("placeholder", _viewModelHospitalityBookingEnquiry.GetPageText("CallIdPlaceholder"))
        txtFromdate.Attributes.Add("placeholder", _viewModelHospitalityBookingEnquiry.GetPageText("FromDatePlaceholder"))
        txtToDate.Attributes.Add("placeholder", _viewModelHospitalityBookingEnquiry.GetPageText("ToDatePlaceholder"))
        txtPackage.Attributes.Add("placeholder", _viewModelHospitalityBookingEnquiry.GetPageText("PackagePlaceholder"))
        txtProduct.Attributes.Add("placeholder", _viewModelHospitalityBookingEnquiry.GetPageText("ProductPlaceholder"))
        txtProductGroup.Attributes.Add("placeholder", _viewModelHospitalityBookingEnquiry.GetPageText("ProductGroupPlaceholder"))
        txtCustomer.Attributes.Add("placeholder", _viewModelHospitalityBookingEnquiry.GetPageText("CustomerPlaceholder"))

        rgvToDate.ErrorMessage = _viewModelHospitalityBookingEnquiry.GetPageText("FutureDatesErrorText")
        rgvFromDate.ErrorMessage = rgvToDate.ErrorMessage
        cmpToDate.ErrorMessage = _viewModelHospitalityBookingEnquiry.GetPageText("FromToDateCompareErrorText")
        revBookingReference.ErrorMessage = _viewModelHospitalityBookingEnquiry.GetPageText("BookingRefRegexErrorText")
    End Sub

    ''' <summary>
    ''' Setup the filters visibility on the screen based on the view model
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setFiltersVisibility()
        If AgentProfile.IsAgent Then
            If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("AgentFilterVisible")) Then
                plhAgent.Visible = False
            Else
                plhAgent.Visible = True
            End If
            If Not Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("CustomerFilterVisible")) Then
                plhCustomer.Visible = False
            Else
                plhCustomer.Visible = True
            End If
            plhSendQAndAReminder.Visible = True
            plhPrintStatus.Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("PrintStatusFilterVisible"))
            plhPrintAllTickets.Visible = AgentProfile.AgentPermissions.CanPrintHospitalityTickets
        Else
            plhAgent.Visible = False
            plhCustomer.Visible = False
            plhSendQAndAReminder.Visible = False
            plhPrintStatus.Visible = False
            plhPrintAllTickets.Visible = False
        End If
        'Filter visibility
        plhCallId.Visible = _viewModelHospitalityBookingEnquiry.GetPageAttribute("CallIDFilterVisible")
        plhPackage.Visible = _viewModelHospitalityBookingEnquiry.GetPageAttribute("PackageFilterVisible")
        plhProduct.Visible = _viewModelHospitalityBookingEnquiry.GetPageAttribute("ProductFilterVisible")
        plhFromdate.Visible = _viewModelHospitalityBookingEnquiry.GetPageAttribute("FromDateFilterVisible")
        plhToDate.Visible = _viewModelHospitalityBookingEnquiry.GetPageAttribute("ToDateFilterVisible")
        plhStatus.Visible = _viewModelHospitalityBookingEnquiry.GetPageAttribute("StatusFilterVisible")
        'This is currently out of scope
        plhProductGroup.Visible = False '_viewModelHospitalityBookingEnquiry.GetPageAttribute("ProductGroupFilterVisible")
        plhQandAStatus.Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("QandAStatusFilterVisible"))
        plhMarkOrderFor.Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBookingEnquiry.GetPageAttribute("BookingTypeFilterVisible"))
    End Sub

    ''' <summary>
    ''' Set the text for the column headings
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setColumnHeadingsAndVisibility()
        CallIDColumnHeading = _viewModelHospitalityBookingEnquiry.GetPageText("CallIDColumnHeading")
        PackageColumnHeading = _viewModelHospitalityBookingEnquiry.GetPageText("PackageColumnHeading")
        ProductColumnHeading = _viewModelHospitalityBookingEnquiry.GetPageText("ProductColumnHeading")
        AgentColumnHeading = _viewModelHospitalityBookingEnquiry.GetPageText("AgentColumnHeading")
        DateColumnHeading = _viewModelHospitalityBookingEnquiry.GetPageText("DateColumnHeading")
        ValueColumnHeading = _viewModelHospitalityBookingEnquiry.GetPageText("ValueColumnHeading")
        StatusColumnHeading = _viewModelHospitalityBookingEnquiry.GetPageText("StatusColumnHeading")
        QandAStatusColumnHeading = _viewModelHospitalityBookingEnquiry.GetPageText("QandAStatusColumnHeading")
        PrintStatusColumnHeading = _viewModelHospitalityBookingEnquiry.GetPageText("PrintStatusColumnHeading")
        PrintSingleBookingTitleText = _viewModelHospitalityBookingEnquiry.GetPageText("PrintSingleBookingTitleText")
        SingleBookingDocumentTitleText = _viewModelHospitalityBookingEnquiry.GetPageText("SingleBookingDocumentTitleText")
    End Sub

    ''' <summary>
    ''' Set the data-title attribute for each row in the data-table
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setRowDataTitles(e As RepeaterItemEventArgs)
        Dim htmlTdCallIDColumnElement As HtmlTableCell = CType(e.Item.FindControl("CallIdColumn"), HtmlTableCell)
        htmlTdCallIDColumnElement.Attributes.Add("data-title", CallIDColumnHeading)
        Dim htmlTdPackageColumnElement As HtmlTableCell = CType(e.Item.FindControl("PackageColumn"), HtmlTableCell)
        htmlTdPackageColumnElement.Attributes.Add("data-title", PackageColumnHeading)
        Dim htmlTdProductColumnElement As HtmlTableCell = CType(e.Item.FindControl("ProductColumn"), HtmlTableCell)
        htmlTdProductColumnElement.Attributes.Add("data-title", ProductColumnHeading)
        Dim htmlTdAgentColumnElement As HtmlTableCell = CType(e.Item.FindControl("AgentColumn"), HtmlTableCell)
        htmlTdAgentColumnElement.Attributes.Add("data-title", AgentColumnHeading)
        Dim htmlTdDateColumnElement As HtmlTableCell = CType(e.Item.FindControl("DateColumn"), HtmlTableCell)
        htmlTdDateColumnElement.Attributes.Add("data-title", DateColumnHeading)
        Dim htmlTdValueColumnElement As HtmlTableCell = CType(e.Item.FindControl("ValueColumn"), HtmlTableCell)
        htmlTdValueColumnElement.Attributes.Add("data-title", ValueColumnHeading)
        Dim htmlTdStatusColumnElement As HtmlTableCell = CType(e.Item.FindControl("StatusColumn"), HtmlTableCell)
        htmlTdStatusColumnElement.Attributes.Add("data-title", StatusColumnHeading)
        Dim htmlTdQandAStatusColumnElement As HtmlTableCell = CType(e.Item.FindControl("QandAStatusColumn"), HtmlTableCell)
        htmlTdQandAStatusColumnElement.Attributes.Add("data-title", QandAStatusColumnHeading)
        Dim htmlTdPrintStatusColumnElement As HtmlTableCell = CType(e.Item.FindControl("PrintStatusColumn"), HtmlTableCell)
        htmlTdPrintStatusColumnElement.Attributes.Add("data-title", PrintStatusColumnHeading)
    End Sub

    ''' <summary>
    ''' Retreive the list of agent data and populate the drop down list
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateAgents()
        _viewModelHospitalityBookingEnquiry = _bookingEnquiryBuilder.RetrieveAgentList()
        ddlAgent.Items.Clear()
        ddlAgent.DataSource = _viewModelHospitalityBookingEnquiry.AgentList
        ddlAgent.DataTextField = "UserName"
        ddlAgent.DataValueField = "UserCode"
        ddlAgent.DataBind()
        ddlAgent.Items.Insert(0, New ListItem(_viewModelHospitalityBookingEnquiry.GetPageText("AllAgentsText"), String.Empty))
    End Sub

    ''' <summary>
    ''' Get the status values and bind them to the drop down list
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateBookingStatus()
        _viewModelHospitalityBookingEnquiry = _bookingEnquiryBuilder.RetrieveBookingStatusList()
        ddlStatus.DataSource = _viewModelHospitalityBookingEnquiry.StatusList
        ddlStatus.DataTextField = "Value"
        ddlStatus.DataValueField = "Key"
        ddlStatus.DataBind()
    End Sub

    ''' <summary>
    ''' Get the mark order for values and bind them to the drop down list
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateMarkForOrder()
        _viewModelHospitalityBookingEnquiry = _bookingEnquiryBuilder.RetrieveMarkOrderForList()
        ddlMarkOrderFor.DataSource = _viewModelHospitalityBookingEnquiry.MarkForOrderList
        ddlMarkOrderFor.DataTextField = "Value"
        ddlMarkOrderFor.DataValueField = "Key"
        ddlMarkOrderFor.DataBind()
    End Sub

    ''' <summary>
    ''' Get the Q&A status values and bind them to the drop down list
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateQandAStatus()
        _viewModelHospitalityBookingEnquiry = _bookingEnquiryBuilder.RetrieveQandAStatusList()
        ddlQandAStatus.DataSource = _viewModelHospitalityBookingEnquiry.QandAStatusList
        ddlQandAStatus.DataTextField = "Value"
        ddlQandAStatus.DataValueField = "Key"
        ddlQandAStatus.DataBind()
    End Sub

    ''' <summary>
    ''' Get the Print status values and bind them to the drop down list
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populatePrintStatus()
        _viewModelHospitalityBookingEnquiry = _bookingEnquiryBuilder.RetrievePrintStatusList()
        ddlPrintStatus.DataSource = _viewModelHospitalityBookingEnquiry.PrintStatusList
        ddlPrintStatus.DataTextField = "Value"
        ddlPrintStatus.DataValueField = "Key"
        ddlPrintStatus.DataBind()
    End Sub

    ''' <summary>
    ''' clear filter controls 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub clearFilterValues()
        ddlAgent.SelectedIndex = 0
        txtCallID.Text = String.Empty
        txtFromdate.Text = String.Empty
        txtToDate.Text = String.Empty
        ddlStatus.SelectedIndex = 0
        If Profile.User.Details IsNot Nothing Then
            txtCustomer.Text = Profile.User.Details.LoginID.TrimStart("0"c)
        Else
            txtCustomer.Text = String.Empty
        End If
        txtPackage.Text = String.Empty
        txtProduct.Text = String.Empty
        txtProductGroup.Text = String.Empty
        ddlMarkOrderFor.SelectedIndex = 0
        ddlQandAStatus.SelectedIndex = 0
        ddlPrintStatus.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' Set the selected filter values to session state so that they can be used when the user pages through search results.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setSessionValues()
        Session("HospitalityBookingEnquiryAgent") = ddlAgent.SelectedValue
        Session("HospitalityBookingEnquiryCallID") = txtCallID.Text
        Session("HospitalityBookingEnquiryFromDate") = txtFromdate.Text
        Session("HospitalityBookingEnquiryToDate") = txtToDate.Text
        Session("HospitalityBookingEnquiryStatus") = ddlStatus.SelectedValue
        Session("HospitalityBookingEnquiryCustomer") = txtCustomer.Text
        Session("HospitalityBookingEnquiryPackage") = txtPackage.Text
        Session("HospitalityBookingEnquiryProduct") = txtProduct.Text
        Session("HospitalityBookingEnquiryProductGroup") = txtProductGroup.Text
        Session("HospitalityBookingEnquiryMarkOrderFor") = ddlMarkOrderFor.SelectedValue
        Session("HospitalityBookingEnquiryQAndAStatus") = ddlQandAStatus.SelectedValue
        Session("HospitalityBookingEnquiryPrintStatus") = ddlPrintStatus.SelectedValue
        Session("HospitalityBookingEnquiryDataTableState") = True
    End Sub

    ''' <summary>
    ''' Clear session state search values so they are no longer used
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub clearSessionValues()
        Session("HospitalityBookingEnquiryAgent") = Nothing
        Session("HospitalityBookingEnquiryCallID") = Nothing
        Session("HospitalityBookingEnquiryFromDate") = Nothing
        Session("HospitalityBookingEnquiryToDate") = Nothing
        Session("HospitalityBookingEnquiryStatus") = Nothing
        Session("HospitalityBookingEnquiryCustomer") = Nothing
        Session("HospitalityBookingEnquiryPackage") = Nothing
        Session("HospitalityBookingEnquiryProduct") = Nothing
        Session("HospitalityBookingEnquiryProductGroup") = Nothing
        Session("HospitalityBookingEnquiryMarkOrderFor") = Nothing
        Session("HospitalityBookingEnquiryQAndAStatus") = Nothing
        Session("HospitalityBookingEnquiryPrintStatus") = Nothing
        Session("HospitalityBookingEnquiryDataTableState") = Nothing
    End Sub

    ''' <summary>
    ''' Set the form values based on the current session values
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setFormValues()
        If Session("HospitalityBookingEnquiryAgent") IsNot Nothing Then
            ddlAgent.SelectedValue = Session("HospitalityBookingEnquiryAgent")
            _isFilterApplicable = True
        End If
        If Session("HospitalityBookingEnquiryCallID") IsNot Nothing Then
            txtCallID.Text = Session("HospitalityBookingEnquiryCallID")
            _isFilterApplicable = True
        End If
        If Session("HospitalityBookingEnquiryFromDate") IsNot Nothing Then
            txtFromdate.Text = Session("HospitalityBookingEnquiryFromDate")
            _isFilterApplicable = True
        End If
        If Session("HospitalityBookingEnquiryToDate") IsNot Nothing Then
            txtToDate.Text = Session("HospitalityBookingEnquiryToDate")
            _isFilterApplicable = True
        End If
        If Session("HospitalityBookingEnquiryStatus") IsNot Nothing Then
            ddlStatus.SelectedValue = Session("HospitalityBookingEnquiryStatus")
            _isFilterApplicable = True
        End If
        If Session("HospitalityBookingEnquiryCustomer") IsNot Nothing Then
            txtCustomer.Text = Session("HospitalityBookingEnquiryCustomer")
            _isFilterApplicable = True
        End If
        If Session("HospitalityBookingEnquiryPackage") IsNot Nothing Then
            txtPackage.Text = Session("HospitalityBookingEnquiryPackage")
            _isFilterApplicable = True
        End If
        If Session("HospitalityBookingEnquiryProduct") IsNot Nothing Then
            txtProduct.Text = Session("HospitalityBookingEnquiryProduct")
            _isFilterApplicable = True
        End If
        If Session("HospitalityBookingEnquiryProductGroup") IsNot Nothing Then
            txtProductGroup.Text = Session("HospitalityBookingEnquiryProductGroup")
            _isFilterApplicable = True
        End If
        If Session("HospitalityBookingEnquiryMarkOrderFor") IsNot Nothing Then
            ddlMarkOrderFor.SelectedValue = Session("HospitalityBookingEnquiryMarkOrderFor")
            _isFilterApplicable = True
        End If
        If Session("HospitalityBookingEnquiryQAndAStatus") IsNot Nothing Then
            ddlQandAStatus.SelectedValue = Session("HospitalityBookingEnquiryQAndAStatus")
            _isFilterApplicable = True
        End If
        If Session("HospitalityBookingEnquiryPrintStatus") IsNot Nothing Then
            ddlPrintStatus.SelectedValue = Session("HospitalityBookingEnquiryPrintStatus")
            _isFilterApplicable = True
        End If
        If Session("HospitalityBookingEnquiryDataTableState") IsNot Nothing Then
            hdfClearDataTableState.Value = String.Empty
        Else
            hdfClearDataTableState.Value = "true"
            Session("HospitalityBookingEnquiryDataTableState") = True
        End If
    End Sub

#End Region
End Class