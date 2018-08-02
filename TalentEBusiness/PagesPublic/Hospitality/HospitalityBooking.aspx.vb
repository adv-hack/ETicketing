Imports System.Collections.Generic
Imports System.Linq
Imports Talent.Common
Imports Talent.eCommerce
Imports TalentBusinessLogic.ModelBuilders.Hospitality
Imports TalentBusinessLogic.DataTransferObjects.Hospitality
Imports TalentBusinessLogic.DataTransferObjects.Customer
Imports TalentBusinessLogic.DataTransferObjects.Product
Imports TalentBusinessLogic.Models
Imports System.Data
Imports TalentBusinessLogic.DataTransferObjects


Partial Class PagesPublic_Hospitality_HospitalityBooking
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _viewModelHospitalityBooking As HospitalityBookingViewModel = Nothing
    Private _viewModelHospitalityDetails As HospitalityDetailsViewModel = Nothing
    Private _inputModelHospitalityBooking As HospitalityBookingInputModel
    Private _inputModelHospitalityDetails As HospitalityDetailsInputModel
    Private _bookingBuilder As HospitalityBookingBuilder = Nothing
    Private _detailsBuilder As HospitalityDetailsBuilder = Nothing
    Private _businessUnit As String = String.Empty
    Private _partner As String = String.Empty
    Private _bookingFound As Boolean = False
    Private _componentDiscountsApplied As Boolean = False
    Private _viewingBookingFromEnquiryPage As Boolean = False
    Private _viewingBookingAsOutputOnly As Boolean = False
    Private _viewingBookingCATMode As Boolean = False
    Private _bookingStatus As String = String.Empty
    Private _defaults As ECommerceModuleDefaults.DefaultValues = Nothing
    Private _moduleDefaults As ECommerceModuleDefaults = Nothing

#End Region

#Region "Public Properties"

    Public Property ProductCode() As String = String.Empty
    Public Property PackageId() As String = String.Empty
    Public Property CallId() As String = String.Empty
    Public Property PackageDescription() As String = String.Empty
    Public Property PackageDescriptionDetailsText() As String = String.Empty
    Public Property CustomerHeaderText() As String = String.Empty
    Public Property PriceBandHeaderText() As String = String.Empty
    Public Property PriceCodeHeaderText() As String = String.Empty
    Public Property PriceHeaderText() As String = String.Empty
    Public Property ChangeComponentSeatText() As String = String.Empty
    Public Property ChangeAllComponentSeatsText() As String = String.Empty
    Public Property ApplyComponentDiscountText() As String = String.Empty
    Public Property RemoveForComponentText() As String = String.Empty
    Public Property CantRemoveSeatWarningText() As String = String.Empty
    Public Property IncludedExtrasHeaderText() As String = String.Empty
    Public Property QuantityHeaderText() As String = String.Empty
    Public Property IsPriceColumnVisible() As Boolean = True
    Public Property BookingHeadingDynamicCSSClass() As String = String.Empty
    Public Property CantChangeTotalPrice As String = String.Empty
    Public Property PercentageDiscountText() As String = String.Empty
    Public Property CurrencySymbol() As String = String.Empty
    Public Property AddSeatUsingBestAvailableText() As String = String.Empty
    Public Property CantAddMoreSeatsWarningText() As String = String.Empty
    Public Property RemoveAllForComponentText() As String = String.Empty
    Public Property CantRemoveWholeComponentAvailabilityWarningText() As String = String.Empty
    Public Property CantRemoveWholeComponentDutToMinQtyWarningText() As String = String.Empty
    Public Property CantRemoveNonSeatedComponentDueToMinQtyWarningText() As String = String.Empty
    Public Property QAndAExpandDynamicClass() As String = String.Empty
    Public Property AdditionalQuestionText() As String = String.Empty
    Public Property QuestionAndAnswerHeaderText() As String = String.Empty
    Public Property UpdatePackageMessageText() As String = String.Empty
    Public Property PrintSingleSeatLinkText() As String = String.Empty
    Public Property PrintBookingLinkText() As String = String.Empty
    Public Property GenerateDocumentLinkText As String = String.Empty
    Public Property CreatePDFLinkText() As String = String.Empty

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "Hospitality-Booking.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("Hospitality-Booking.js", "/Module/Hospitality/"), False)
        _businessUnit = TalentCache.GetBusinessUnit()
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        _bookingBuilder = New HospitalityBookingBuilder
        _detailsBuilder = New HospitalityDetailsBuilder
        _moduleDefaults = New ECommerceModuleDefaults
        _defaults = _moduleDefaults.GetDefaults()
        _inputModelHospitalityBooking = setupHospitalityBookingInputModel()
        _inputModelHospitalityDetails = setupHospitalityDetailsInputModel()
        processController(_inputModelHospitalityBooking, _inputModelHospitalityDetails)
        createView()
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then
            If hdfCancelBooking.Value = "true" Then
                'Cancel button does not have an event handler because the user must confirm they're cancelling the order from the alertify.js confirm box
                performBookingCancel()
            End If
            If hdfPrintSingleSeat.Value = "true" Then
                'print seat button does not have an event handler because the user must confirm they're printing seat from the alertify.js confirm box
                performPrint()
            End If
            If hdfPrintBooking.Value = "true" Then
                'print seat button does not have an event handler because the user must confirm they're printing booking from the alertify.js confirm box
                performPrint()
            End If
            If hdfGenerateBookingDocument.Value = "true" Then
                'print generate document link button does not have an event handler because the user must confirm they're printing booking from the alertify.js confirm box
                generateDocumentForBooking()
            End If
        Else
            If HttpContext.Current.Session("Reserve") = "true" Then
                hdfReserveClick.Value = "true"
                HttpContext.Current.Session("Reserve") = Nothing
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        plhBookingErrors.Visible = (ltlBookingErrors.Text.Length > 0)
        plhWarningMessage.Visible = (ltlWarningMessage.Text.Length > 0)
        plhSuccessMessage.Visible = (ltlSuccessMessage.Text.Length > 0)
    End Sub

    Protected Sub Page_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        ' This is a required hidden field that allows the reservation processes to determine if there is a 
        ' hospitality booking error (other than ones raised by reservation processes)
        If Not IsPostBack Then
            hdfNoneReservationErrorCount.Value = ltlBookingErrors.Text.Length
        End If
    End Sub

    Protected Sub lbtnUpdate_Click(sender As Object, e As EventArgs) Handles lbtnUpdate.Click
        performBookingUpdate()
        _inputModelHospitalityBooking.CallId = CallId
        If hdfReserveClick.Value = "true" Then
            HttpContext.Current.Session("Reserve") = "true"
        End If
        Response.Redirect(Request.Url.AbsoluteUri)
    End Sub

    Protected Sub lbtnUpdateForSoldBooking_Click(sender As Object, e As EventArgs) Handles lbtnUpdateForSoldBooking.Click
        _inputModelHospitalityBooking.Mode = OperationMode.Amend
        _inputModelHospitalityBooking.ActivityQuestionAnswerList = getQuestionAndAnswer()
        _viewModelHospitalityBooking = _bookingBuilder.UpdateHospitalityForSoldBooking(_inputModelHospitalityBooking)
        If _viewModelHospitalityBooking.Error.HasError Then
            ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
        Else
            Response.Redirect(Request.Url.AbsoluteUri)
        End If
    End Sub

    Protected Sub lbtnContinue_Click(sender As Object, e As EventArgs) Handles lbtnContinue.Click
        performBookingUpdate()
        _inputModelHospitalityBooking.CallId = CallId
        Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
    End Sub

    Protected Sub lbtnSaveAsEnquiry_Click(sender As Object, e As EventArgs) Handles lbtnSaveAsEnquiry.Click
        performBookingUpdate()
        _inputModelHospitalityBooking.SavePackageMode = GlobalConstants.SAVEBACKENDCOMPONENTDETAILS
        _inputModelHospitalityBooking.Status = GlobalConstants.ENQUIRY_BOOKING_STATUS
        _inputModelHospitalityBooking.CallId = CallId
        _viewModelHospitalityBooking = _bookingBuilder.SaveHospitalityBookingAsEnquiry(_inputModelHospitalityBooking)
        If _viewModelHospitalityBooking.Error.HasError Then
            ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
        Else
            Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
            ltlSuccessMessage.Text = _viewModelHospitalityBooking.GetPageText("BookingSavedForLaterMessageText").Replace("<<CallID>>", CallId)
            plhBookingDetails.Visible = False
        End If
    End Sub

    Protected Sub lbtnBackToBookingEnquiry_Click(sender As Object, e As EventArgs) Handles lbtnBackToBookingEnquiry.Click
        If _viewingBookingAsOutputOnly Then
            Response.Redirect("~/PagesPublic/Hospitality/HospitalityBookingEnquiry.aspx")
        Else
            _inputModelHospitalityBooking.SavePackageMode = GlobalConstants.SAVEBACKENDCOMPONENTDETAILS
            If (_viewModelHospitalityBooking.PackageDetailsList(0).BookingStatus = GlobalConstants.RESERVATION_BOOKING_STATUS) Then
                _inputModelHospitalityBooking.Status = GlobalConstants.RESERVATION_BOOKING_STATUS
            Else
                _inputModelHospitalityBooking.Status = GlobalConstants.ENQUIRY_BOOKING_STATUS
            End If

            _inputModelHospitalityBooking.CallId = CallId
            _viewModelHospitalityBooking = _bookingBuilder.SaveHospitalityBookingAsEnquiry(_inputModelHospitalityBooking)
            If _viewModelHospitalityBooking.Error.HasError Then
                ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
            Else
                Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
                Response.Redirect("~/PagesPublic/Hospitality/HospitalityBookingEnquiry.aspx")
            End If
        End If
    End Sub

    Protected Sub lbtnRemoveDiscount_Click(sender As Object, e As EventArgs) Handles lbtnRemoveDiscount.Click
        removeAllDiscounts()
        Response.Redirect(Request.Url.AbsoluteUri)
    End Sub

    Protected Sub lbtnBoxOfficeAddPackageExtras_Click(sender As Object, e As EventArgs) Handles lbtnBoxOfficeAddPackageExtras.Click
        performBookingUpdate()
        addPackageExtras(ddlBoxOfficePackageExtras.SelectedValue)
    End Sub

    Protected Sub lbtnPWSAddPackageExtras_Click(sender As Object, e As EventArgs) Handles lbtnPWSAddPackageExtras.Click
        performBookingUpdate()
        addPackageExtras(ddlPWSPackageExtras.SelectedValue)
    End Sub

    Protected Sub lbtnCreatePDF_Click(sender As Object, e As EventArgs) Handles lbtnCreatePDF.Click
        Dim viewRenderer As New WebFormMVCUtil
        Dim detailsWriter As New System.IO.StringWriter
        Dim bookingWriter As New System.IO.StringWriter
        Dim htmlContent As String = String.Empty
        Dim cssContent As String = String.Empty
        Dim pdfCreator As New CreatePDF
        Dim fileName As String = String.Empty
        Dim filePath As String = ModuleDefaults.HtmlPathAbsolute
        Dim pdfPathAndFile As String = String.Empty
        Dim address As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)
        Dim addressStringBuilder As New StringBuilder
        Dim leadSourceList As List(Of LeadSourceDetails)
        Dim hasError As Boolean = False
        Try
            _viewModelHospitalityDetails.CallId = CallId
            If address.Address_Line_1.Length > 0 Then addressStringBuilder.Append(address.Address_Line_1)
            If address.Address_Line_2.Length > 0 Then
                If addressStringBuilder.Length > 0 Then addressStringBuilder.Append(", ")
                addressStringBuilder.Append(address.Address_Line_2)
            End If
            If address.Address_Line_3.Length > 0 Then
                If addressStringBuilder.Length > 0 Then addressStringBuilder.Append(", ")
                addressStringBuilder.Append(address.Address_Line_3)
            End If
            If address.Address_Line_4.Length > 0 Then
                If addressStringBuilder.Length > 0 Then addressStringBuilder.Append(", ")
                addressStringBuilder.Append(address.Address_Line_4)
            End If
            If address.Address_Line_5.Length > 0 Then
                If addressStringBuilder.Length > 0 Then addressStringBuilder.Append(", ")
                addressStringBuilder.Append(address.Address_Line_5)
            End If
            If address.Post_Code.Length > 0 Then
                If addressStringBuilder.Length > 0 Then addressStringBuilder.Append(", ")
                addressStringBuilder.Append(address.Post_Code)
            End If
            _viewModelHospitalityDetails.CustomerAddress = addressStringBuilder.ToString()
            _viewModelHospitalityDetails.CustomerName = Profile.User.Details.Full_Name
            _viewModelHospitalityDetails.CompanyName = Profile.User.Details.CompanyName
            _viewModelHospitalityDetails.MobileNumber = Profile.User.Details.Mobile_Number
            _viewModelHospitalityDetails.HomeNumber = Profile.User.Details.Telephone_Number
            _viewModelHospitalityDetails.WorkNumber = Profile.User.Details.Work_Number
            _viewModelHospitalityBooking.PaymentOwnerCustomerForename = Profile.User.Details.Forename
            _viewModelHospitalityBooking.PaymentOwnerCustomerSurname = Profile.User.Details.Surname
            leadSourceList = _viewModelHospitalityDetails.LeadSourceDetails
            If _viewModelHospitalityBooking.PackageDetailsList(0).LeadSourceID > 0 Then
                If leadSourceList.Exists(Function(x) x.LeadSourceID = _viewModelHospitalityBooking.PackageDetailsList(0).LeadSourceID) Then
                    _viewModelHospitalityBooking.PackageDetailsList(0).LeadSourceDescription = leadSourceList.Find(Function(x) x.LeadSourceID = _viewModelHospitalityBooking.PackageDetailsList(0).LeadSourceID).LeadSourceDescription
                End If
            End If
            detailsWriter = viewRenderer.RenderPartial("~/Views/PartialViews/_HospitalityDetails.cshtml", _viewModelHospitalityDetails)
            bookingWriter = viewRenderer.RenderPartial("~/Views/PartialViews/_HospitalityBooking.cshtml", _viewModelHospitalityBooking)
            htmlContent &= detailsWriter.GetStringBuilder().ToString()
            htmlContent &= bookingWriter.GetStringBuilder().ToString()
        Catch ex As Exception
            Logging.WriteLog(Profile.UserName, "lbtnCreatePDF_Click-Error", ex.StackTrace, ex.Message)
            hasError = True
        Finally
            detailsWriter.Dispose()
            bookingWriter.Dispose()
        End Try
        If htmlContent.Length > 0 Then
            fileName = String.Concat(CallId, "-", Now.ToString("ddMMyyyy-HHmm"), ".pdf")
            If Not filePath.EndsWith("\") Then filePath &= "\"
            filePath &= "HospitalityPDF\"
            cssContent = Talent.eCommerce.Utilities.GetCSSContentFromFile("HospitalityPDF\" & PackageId & "_package.css")
            If cssContent.Length = 0 Then cssContent = Talent.eCommerce.Utilities.GetCSSContentFromFile("HospitalityPDF\package.css")
            pdfPathAndFile = pdfCreator.CreateFile(fileName, filePath, htmlContent, cssContent)
            If pdfPathAndFile.Length > 0 Then
                Dim url As String = "~/Assets"
                If Not ModuleDefaults.HtmlIncludePathRelative.StartsWith("/") Then url &= "/"
                url &= ModuleDefaults.HtmlIncludePathRelative
                If Not url.EndsWith("/") Then url &= "/"
                url &= "HospitalityPDF/" & fileName
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "openPDFScript", "javascript:window.open('" & ResolveUrl(url) & "', '_blank');", True)
            Else
                hasError = True
            End If
        Else
            hasError = True
        End If
        If hasError Then
            ltlBookingErrors.Text = _viewModelHospitalityBooking.GetPageText("ErrorCreatingPDF")
        End If
    End Sub

    Protected Sub rptBoxOfficeSeatedComponents_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptBoxOfficeSeatedComponents.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim rptBoxOfficeSeats As Repeater = CType(e.Item.FindControl("rptBoxOfficeSeats"), Repeater)
            Dim plhComponentDescription As PlaceHolder = CType(e.Item.FindControl("plhComponentDescription"), PlaceHolder)
            Dim ltlComponentDescription As Literal = CType(e.Item.FindControl("ltlComponentDescription"), Literal)
            Dim plhComponentPriceTop As PlaceHolder = CType(e.Item.FindControl("plhComponentPriceTop"), PlaceHolder)
            Dim plhComponentPriceBottom As PlaceHolder = CType(e.Item.FindControl("plhComponentPriceBottom"), PlaceHolder)
            Dim plhComponentDiscount As PlaceHolder = CType(e.Item.FindControl("plhComponentDiscount"), PlaceHolder)
            Dim plhComponentDiscountedPrice As PlaceHolder = CType(e.Item.FindControl("plhComponentDiscountedPrice"), PlaceHolder)
            Dim plhComponentPrice As PlaceHolder = CType(e.Item.FindControl("plhComponentPrice"), PlaceHolder)
            Dim seatedComponent As ComponentDetails = CType(e.Item.DataItem, ComponentDetails)
            Dim seats As List(Of HospitalitySeatDetails) = seatedComponent.HospitalitySeatDetailsList
            Dim txtComponentDiscountPercentage As TextBox = CType(e.Item.FindControl("txtComponentDiscountPercentage"), TextBox)
            Dim rngComponentDiscountPercentage As RangeValidator = CType(e.Item.FindControl("rngComponentDiscountPercentage"), RangeValidator)
            Dim rfvComponentDiscountPercentage As RequiredFieldValidator = CType(e.Item.FindControl("rfvComponentDiscountPercentage"), RequiredFieldValidator)
            Dim hdfMaxComponentDiscountPercentage As HiddenField = CType(e.Item.FindControl("hdfMaxComponentDiscountPercentage"), HiddenField)
            Dim hdfQuantity As HiddenField = CType(e.Item.FindControl("hdfQuantity"), HiddenField)
            Dim hdfStandCode As HiddenField = CType(e.Item.FindControl("hdfStandCode"), HiddenField)
            Dim hdfAreaCode As HiddenField = CType(e.Item.FindControl("hdfAreaCode"), HiddenField)
            Dim hdfComponentType As HiddenField = CType(e.Item.FindControl("hdfComponentType"), HiddenField)
            Dim plhAddMoreSeats As PlaceHolder = CType(e.Item.FindControl("plhAddMoreSeats"), PlaceHolder)
            Dim plhCantAddMoreSeats As PlaceHolder = CType(e.Item.FindControl("plhCantAddMoreSeats"), PlaceHolder)
            Dim plhRemoveAllForComponent As PlaceHolder = CType(e.Item.FindControl("plhRemoveAllForComponent"), PlaceHolder)
            Dim plhCantRemoveWholeComponentAvailabilityComponent As PlaceHolder = CType(e.Item.FindControl("plhCantRemoveWholeComponentAvailability"), PlaceHolder)
            Dim plhCantRemoveWholeComponentDueToMinQtyComponent As PlaceHolder = CType(e.Item.FindControl("plhCantRemoveWholeComponentDueToMinQty"), PlaceHolder)
            Dim plhChangeAllSeats As PlaceHolder = CType(e.Item.FindControl("plhChangeAllSeats"), PlaceHolder)
            hdfQuantity.Value = seatedComponent.HospitalitySeatDetailsList.Count
            hdfStandCode.Value = seats.Item(0).StandCode
            hdfAreaCode.Value = seats.Item(0).AreaCode
            hdfComponentType.Value = seatedComponent.ComponentType
            plhComponentDiscount.Visible = _viewModelHospitalityBooking.PackageDetailsList(0).PricingMethod <> GlobalConstants.PACKAGE_PRICING
            If plhComponentDiscount.Visible Then
                txtComponentDiscountPercentage.Text = CInt(seatedComponent.Discount)
                hdfMaxComponentDiscountPercentage.Value = CInt(seatedComponent.MaxDiscountPercent)
                If _viewingBookingAsOutputOnly Then
                    txtComponentDiscountPercentage.Enabled = False
                    rngComponentDiscountPercentage.Enabled = False
                    rfvComponentDiscountPercentage.Enabled = False
                Else
                    If seatedComponent.MaxDiscountPercent = 0 Then
                        txtComponentDiscountPercentage.Enabled = False
                        txtComponentDiscountPercentage.Attributes.Add("title", _viewModelHospitalityBooking.GetPageText("CantApplyDiscountWarningText"))
                        rngComponentDiscountPercentage.Enabled = False
                        rfvComponentDiscountPercentage.Enabled = False
                    Else
                        Dim componentDiscountPercentageErrorMessage As String = _viewModelHospitalityBooking.GetPageText("ComponentDiscountPercentageInvalidText")
                        componentDiscountPercentageErrorMessage = componentDiscountPercentageErrorMessage.Replace("<<ComponentDescription>>", seatedComponent.ComponentDescription)
                        componentDiscountPercentageErrorMessage = componentDiscountPercentageErrorMessage.Replace("<<MaxDiscountPercentage>>", seatedComponent.MaxDiscountPercent)
                        txtComponentDiscountPercentage.Enabled = True
                        rngComponentDiscountPercentage.Enabled = True
                        rfvComponentDiscountPercentage.Enabled = True
                        rngComponentDiscountPercentage.MaximumValue = CInt(seatedComponent.MaxDiscountPercent)
                        rngComponentDiscountPercentage.MinimumValue = 0
                        rngComponentDiscountPercentage.Type = ValidationDataType.Double
                        rngComponentDiscountPercentage.ErrorMessage = componentDiscountPercentageErrorMessage
                        rfvComponentDiscountPercentage.ErrorMessage = _viewModelHospitalityBooking.GetPageText("ComponentDiscountPercentageRequiredText").Replace("<<ComponentDescription>>", seatedComponent.ComponentDescription)
                    End If
                End If
                If seatedComponent.Discount > 0 Then
                    _componentDiscountsApplied = True
                    plhComponentDiscountedPrice.Visible = True
                    plhComponentPrice.Visible = False
                Else
                    plhComponentDiscountedPrice.Visible = False
                    plhComponentPrice.Visible = True
                End If
            Else
                _componentDiscountsApplied = False
            End If

            If _viewingBookingAsOutputOnly Then
                plhRemoveAllForComponent.Visible = False
                plhCantRemoveWholeComponentAvailabilityComponent.Visible = False
                plhCantRemoveWholeComponentDueToMinQtyComponent.Visible = False
                plhAddMoreSeats.Visible = False
                plhAddMoreSeats.Visible = False
                plhCantAddMoreSeats.Visible = False
                plhChangeAllSeats.Visible = False
            Else
                plhChangeAllSeats.Visible = True
                plhAddMoreSeats.Visible = (seatedComponent.HospitalitySeatDetailsList.Count < seatedComponent.MaxQty)
                If plhAddMoreSeats.Visible Then
                    plhCantAddMoreSeats.Visible = False
                Else
                    plhCantAddMoreSeats.Visible = True
                End If
                If seatedComponent.ComponentType = "A" Then
                    plhCantRemoveWholeComponentAvailabilityComponent.Visible = True
                    plhRemoveAllForComponent.Visible = False
                    plhCantRemoveWholeComponentDueToMinQtyComponent.Visible = False
                Else
                    If seatedComponent.MinQty > 0 Then
                        plhCantRemoveWholeComponentDueToMinQtyComponent.Visible = True
                        plhCantRemoveWholeComponentAvailabilityComponent.Visible = False
                        plhRemoveAllForComponent.Visible = False
                    Else
                        plhRemoveAllForComponent.Visible = True
                        plhCantRemoveWholeComponentDueToMinQtyComponent.Visible = False
                        plhCantRemoveWholeComponentAvailabilityComponent.Visible = False
                    End If
                End If
            End If

            ltlComponentDescription.Text = _viewModelHospitalityBooking.GetPageText("SeatedComponentText")
            ltlComponentDescription.Text = ltlComponentDescription.Text.Replace("<<ComponentDescription>>", seatedComponent.ComponentDescription)
            ltlComponentDescription.Text = ltlComponentDescription.Text.Replace("<<NumberOfSeats>>", seats.Count)
            plhComponentPriceTop.Visible = IsPriceColumnVisible
            plhComponentPriceBottom.Visible = IsPriceColumnVisible

            rptBoxOfficeSeats.DataSource = seats
            rptBoxOfficeSeats.DataBind()
        End If
    End Sub

    Protected Sub rptBoxOfficeSeats_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)


        Dim rptBoxOfficeSeats As Repeater = CType(sender, Repeater)
        Dim rptBoxOfficeSeatedComponentsItem As RepeaterItem = CType(rptBoxOfficeSeats.BindingContainer, RepeaterItem)
        Dim seatedComponent As ComponentDetails = CType(rptBoxOfficeSeatedComponentsItem.DataItem, ComponentDetails)

        If e.Item.ItemType = ListItemType.Header Then
            Dim ltlComponentDescription As Literal = CType(e.Item.FindControl("ltlComponentDescription"), Literal)
            ltlComponentDescription.Text = seatedComponent.ComponentDescription
        ElseIf e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim rptBoxOfficeSTExceptionDetails As Repeater = CType(e.Item.FindControl("rptBoxOfficeSTExceptionDetails"), Repeater)
            Dim tdBoxOfficeSeatsItemHeader As HtmlTableCell = CType(e.Item.FindControl("tdBoxOfficeSeatsItemHeader"), HtmlTableCell)
            Dim trBoxOfficeSeatsItemHeader As HtmlTableRow = CType(e.Item.FindControl("trBoxOfficeSeatsItemHeader"), HtmlTableRow)
            Dim seat As HospitalitySeatDetails = CType(e.Item.DataItem, HospitalitySeatDetails)
            Dim seatExceptions As List(Of SeasonTicketExceptions) = seat.SeasonTicketExceptions
            Dim plhRemoveForComponent As PlaceHolder = CType(e.Item.FindControl("plhRemoveForComponent"), PlaceHolder)
            Dim plhCantDeleteComponent As PlaceHolder = CType(e.Item.FindControl("plhCantDeleteComponent"), PlaceHolder)
            Dim plhChangeSeat As PlaceHolder = CType(e.Item.FindControl("plhChangeSeat"), PlaceHolder)
            Dim plhSTException As PlaceHolder = CType(e.Item.FindControl("plhSTException"), PlaceHolder)
            Dim plhPrintSingleSeat As PlaceHolder = CType(e.Item.FindControl("plhPrintSingleSeat"), PlaceHolder)
            Dim navigateUrl As New StringBuilder


            tdBoxOfficeSeatsItemHeader.Attributes.Add("data-title", seat.ComponentDescription)
            If seatExceptions.Count > 0 Then
                plhSTException.Visible = True
                trBoxOfficeSeatsItemHeader.Attributes.Add("class", "c-package-st-exception")
                Dim ltlSTExceptionLink As Literal = CType(e.Item.FindControl("ltlSTExceptionLink"), Literal)
                Dim ltlSTExceptionHeader As Literal = CType(e.Item.FindControl("ltlSTExceptionHeader"), Literal)
                ltlSTExceptionLink.Text = _viewModelHospitalityBooking.GetPageText("SeatExceptionDetailsLinkText")
                ltlSTExceptionHeader.Text = _viewModelHospitalityBooking.GetPageText("SeatExceptionDetailsHeaderText")

                rptBoxOfficeSTExceptionDetails.DataSource = seatExceptions
                rptBoxOfficeSTExceptionDetails.DataBind()
            End If
            If _viewingBookingAsOutputOnly Then
                plhRemoveForComponent.Visible = False
                plhCantDeleteComponent.Visible = False
                plhChangeSeat.Visible = False
                If AgentProfile.AgentPermissions.CanPrintHospitalityTickets AndAlso seatedComponent.HasPrintableTicket Then
                    plhPrintSingleSeat.Visible = True
                Else
                    plhPrintSingleSeat.Visible = False
                End If

            Else
                plhRemoveForComponent.Visible = (seatedComponent.HospitalitySeatDetailsList.Count > seatedComponent.MinQty)
                If plhRemoveForComponent.Visible Then
                    plhCantDeleteComponent.Visible = False
                Else
                    plhCantDeleteComponent.Visible = True
                End If
                plhPrintSingleSeat.Visible = False
            End If
            setCustomerDropDownList(e)
            setPriceBandsDropDownList(e, False)
            setPriceCodesDropDownList(e)
        End If
    End Sub

    Protected Sub rptBoxOfficeNonSeatedComponents_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptBoxOfficeNonSeatedComponents.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim ddlComponentQuantity As DropDownList = CType(e.Item.FindControl("ddlComponentQuantity"), DropDownList)
            Dim plhComponentDiscountedPrice As PlaceHolder = CType(e.Item.FindControl("plhComponentDiscountedPrice"), PlaceHolder)
            Dim plhComponentPrice As PlaceHolder = CType(e.Item.FindControl("plhComponentPrice"), PlaceHolder)
            Dim component As ComponentDetails = CType(e.Item.DataItem, ComponentDetails)
            Dim minQuantity As Integer = component.MinQty
            Dim maxQuantity As Integer = component.MaxQty
            Dim txtComponentDiscountPercentage As TextBox = CType(e.Item.FindControl("txtComponentDiscountPercentage"), TextBox)
            Dim rngComponentDiscountPercentage As RangeValidator = CType(e.Item.FindControl("rngComponentDiscountPercentage"), RangeValidator)
            Dim rfvComponentDiscountPercentage As RequiredFieldValidator = CType(e.Item.FindControl("rfvComponentDiscountPercentage"), RequiredFieldValidator)
            Dim hdfMaxComponentDiscountPercentage As HiddenField = CType(e.Item.FindControl("hdfMaxComponentDiscountPercentage"), HiddenField)
            Dim lbtnRemoveForComponent As LinkButton = CType(e.Item.FindControl("lbtnRemoveForComponent"), LinkButton)
            Dim plhRemoveForComponent As PlaceHolder = CType(e.Item.FindControl("plhRemoveForComponent"), PlaceHolder)
            Dim plhCantDeleteComponent As PlaceHolder = CType(e.Item.FindControl("plhCantDeleteComponent"), PlaceHolder)

            If _viewModelHospitalityBooking.PackageDetailsList(0).PricingMethod <> GlobalConstants.PACKAGE_PRICING Then
                txtComponentDiscountPercentage.Text = CInt(component.Discount)
                hdfMaxComponentDiscountPercentage.Value = CInt(component.MaxDiscountPercent)

                If _viewingBookingAsOutputOnly Then
                    ddlComponentQuantity.Enabled = False
                    plhRemoveForComponent.Visible = False
                    plhCantDeleteComponent.Visible = False
                    txtComponentDiscountPercentage.Enabled = False
                    rngComponentDiscountPercentage.Enabled = False
                    rfvComponentDiscountPercentage.Enabled = False
                Else
                    If component.MaxDiscountPercent = 0 Then
                        txtComponentDiscountPercentage.Enabled = False
                        txtComponentDiscountPercentage.Attributes.Add("title", _viewModelHospitalityBooking.GetPageText("CantApplyDiscountWarningText"))
                        rngComponentDiscountPercentage.Enabled = False
                        rfvComponentDiscountPercentage.Enabled = False
                    Else
                        Dim componentDiscountPercentageErrorMessage As String = _viewModelHospitalityBooking.GetPageText("ComponentDiscountPercentageInvalidText")
                        componentDiscountPercentageErrorMessage = componentDiscountPercentageErrorMessage.Replace("<<ComponentDescription>>", component.ComponentDescription)
                        componentDiscountPercentageErrorMessage = componentDiscountPercentageErrorMessage.Replace("<<MaxDiscountPercentage>>", component.MaxDiscountPercent)
                        txtComponentDiscountPercentage.Enabled = True
                        rngComponentDiscountPercentage.Enabled = True
                        rfvComponentDiscountPercentage.Enabled = True
                        rngComponentDiscountPercentage.MaximumValue = CInt(component.MaxDiscountPercent)
                        rngComponentDiscountPercentage.MinimumValue = 0
                        rngComponentDiscountPercentage.Type = ValidationDataType.Double
                        rngComponentDiscountPercentage.ErrorMessage = componentDiscountPercentageErrorMessage
                        rfvComponentDiscountPercentage.ErrorMessage = _viewModelHospitalityBooking.GetPageText("ComponentDiscountPercentageRequiredText").Replace("<<ComponentDescription>>", component.ComponentDescription)
                    End If
                    If minQuantity > 0 Then
                        plhRemoveForComponent.Visible = False
                        plhCantDeleteComponent.Visible = True
                    Else
                        plhRemoveForComponent.Visible = True
                        plhCantDeleteComponent.Visible = False
                    End If
                End If

                If component.Discount > 0 Then
                    _componentDiscountsApplied = True
                    plhComponentDiscountedPrice.Visible = True
                    plhComponentPrice.Visible = False
                Else
                    plhComponentDiscountedPrice.Visible = False
                    plhComponentPrice.Visible = True
                End If
            Else
                _componentDiscountsApplied = False
                If _viewingBookingAsOutputOnly Then
                    ddlComponentQuantity.Enabled = False
                    plhRemoveForComponent.Visible = False
                    plhCantDeleteComponent.Visible = False
                Else
                    If minQuantity > 0 Then
                        plhRemoveForComponent.Visible = False
                        plhCantDeleteComponent.Visible = True
                    Else
                        plhRemoveForComponent.Visible = True
                        plhCantDeleteComponent.Visible = False
                    End If
                End If
            End If
            If minQuantity = 0 Then
                minQuantity += 1
            End If
            For i As Integer = minQuantity To maxQuantity Step 1
                Dim item As New ListItem
                item.Text = i
                item.Value = i
                If component.Quantity.TrimStart(GlobalConstants.LEADING_ZEROS).Trim.Length > 0 AndAlso CInt(component.Quantity.TrimStart(GlobalConstants.LEADING_ZEROS).Trim) = i Then
                    item.Selected = True
                End If
                ddlComponentQuantity.Items.Add(item)
            Next

        End If
    End Sub

    Protected Sub rptPWSSeatedComponents_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPWSSeatedComponents.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim rptPWSSeats As Repeater = CType(e.Item.FindControl("rptPWSSeats"), Repeater)
            Dim plhComponentPrice As PlaceHolder = CType(e.Item.FindControl("plhComponentPrice"), PlaceHolder)
            Dim seatedComponent As ComponentDetails = CType(e.Item.DataItem, ComponentDetails)
            Dim seats As List(Of HospitalitySeatDetails) = seatedComponent.HospitalitySeatDetailsList
            Dim hdfStandCode As HiddenField = CType(e.Item.FindControl("hdfStandCode"), HiddenField)
            Dim hdfAreaCode As HiddenField = CType(e.Item.FindControl("hdfAreaCode"), HiddenField)
            Dim plhChangeAllSeats As PlaceHolder = CType(e.Item.FindControl("plhChangeAllSeats"), PlaceHolder)
            Dim plhRemoveAllForComponent As PlaceHolder = CType(e.Item.FindControl("plhRemoveAllForComponent"), PlaceHolder)
            plhChangeAllSeats.Visible = (Not _viewingBookingAsOutputOnly) And (Not Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBooking.GetPageAttribute("HideChangeSeatsOptions")))
            hdfAreaCode.Value = seats.Item(0).AreaCode
            hdfStandCode.Value = seats.Item(0).StandCode

            plhComponentPrice.Visible = IsPriceColumnVisible
            rptPWSSeats.DataSource = seats
            rptPWSSeats.DataBind()

            If seatedComponent.IsExtraComponent Then
                plhRemoveAllForComponent.Visible = True
            Else
                plhRemoveAllForComponent.Visible = False
            End If
        End If
    End Sub

    Protected Sub rptPWSSeats_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Dim rptPWSSeats As Repeater = CType(sender, Repeater)
        Dim rptPWSSeatedComponentsItem As RepeaterItem = CType(rptPWSSeats.BindingContainer, RepeaterItem)
        Dim seatedComponent As ComponentDetails = CType(rptPWSSeatedComponentsItem.DataItem, ComponentDetails)
        If e.Item.ItemType = ListItemType.Header Then
            Dim ltlComponentDescription As Literal = CType(e.Item.FindControl("ltlComponentDescription"), Literal)
            ltlComponentDescription.Text = seatedComponent.ComponentDescription
        ElseIf e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim tdPWSSeatsItemHeader As HtmlTableCell = CType(e.Item.FindControl("tdPWSSeatsItemHeader"), HtmlTableCell)
            Dim seat As HospitalitySeatDetails = CType(e.Item.DataItem, HospitalitySeatDetails)
            Dim plhChangeSeat As PlaceHolder = CType(e.Item.FindControl("plhChangeSeat"), PlaceHolder)
            Dim lbtnRemoveForComponent As LinkButton = CType(e.Item.FindControl("lbtnRemoveForComponent"), LinkButton)
            Dim plhRemoveForComponent As PlaceHolder = CType(e.Item.FindControl("plhRemoveForComponent"), PlaceHolder)


            tdPWSSeatsItemHeader.Attributes.Add("data-title", seat.ComponentDescription)
            setCustomerDropDownList(e)
            setPriceBandsDropDownList(e, True)
            plhChangeSeat.Visible = (Not _viewingBookingAsOutputOnly) And (Not Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBooking.GetPageAttribute("HideChangeSeatsOptions")))

            If seatedComponent.IsExtraComponent Then
                plhRemoveForComponent.Visible = True
            Else
                plhRemoveForComponent.Visible = False
            End If
        End If
    End Sub

    Protected Sub rptPWSNonSeatedComponents_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptPWSNonSeatedComponents.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim component As ComponentDetails = CType(e.Item.DataItem, ComponentDetails)
            Dim lbtnRemoveForComponent As LinkButton = CType(e.Item.FindControl("lbtnRemoveForComponent"), LinkButton)
            Dim plhRemoveForComponent As PlaceHolder = CType(e.Item.FindControl("plhRemoveForComponent"), PlaceHolder)

            If component.IsExtraComponent Then
                plhRemoveForComponent.Visible = True
            Else
                plhRemoveForComponent.Visible = False
            End If
        End If
    End Sub

    Protected Sub rptBoxOfficeSeatedComponents_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
        Select Case e.CommandName
            Case "RemoveAllForComponent"
                removeAllForComponent(e)
            Case "AddSeatUsingBestAvailable"
                addSeatUsingBestAvailable(e, True)
            Case "ChangeAllSeats"
                changeAllSeats(e)
        End Select
    End Sub

    Protected Sub rptBoxOfficeSeats_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
        Select Case e.CommandName
            Case "ChangeSeat"
                changeSeat(e)
            Case "RemoveForComponent"
                If (DirectCast(source, System.Web.UI.WebControls.Repeater).Items.Count > 1) Then
                    removeForComponent(e)
                Else
                    removeAllForComponent(e)
                End If
        End Select
    End Sub

    Protected Sub rptBoxOfficeNonSeatedComponents_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptBoxOfficeNonSeatedComponents.ItemCommand
        removeAllForComponent(e)
    End Sub

    Protected Sub rptPWSSeatedComponents_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
        Select Case e.CommandName
            Case "RemoveAllForComponent"
                removeAllForComponent(e)
            Case "ChangeAllSeats"
                changeAllSeats(e)
            Case Else
                'Do Nothing
        End Select
    End Sub

    Protected Sub rptPWSSeats_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
        Select Case e.CommandName
            Case "ChangeSeat"
                changeSeat(e)
            Case "RemoveForComponent"
                If (DirectCast(source, System.Web.UI.WebControls.Repeater).Items.Count > 1) Then
                    removeForComponent(e)
                Else
                    removeAllForComponent(e)
                End If
        End Select
    End Sub

    Protected Sub rptPWSNonSeatedComponents_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles rptPWSNonSeatedComponents.ItemCommand
        removeAllForComponent(e)
    End Sub

    Protected Sub rptProductQuestions_ItemDataBound(sender As Object, e As RepeaterItemEventArgs) Handles rptProductQuestions.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim activityQuestionAnswer As ActivityQuestionAnswer = CType(e.Item.DataItem, ActivityQuestionAnswer)
            Select Case activityQuestionAnswer.AnswerType
                Case Is = GlobalConstants.FREE_TEXT_FIELD
                    Dim plhFreeTextField As PlaceHolder = CType(e.Item.FindControl("plhFreeTextField"), PlaceHolder)
                    Dim txtQuestionAnswerText As TextBox = CType(e.Item.FindControl("txtQuestionAnswerText"), TextBox)
                    Dim lblQuestionText As Label = CType(e.Item.FindControl("lblQuestionText"), Label)
                    lblQuestionText.Text = activityQuestionAnswer.QuestionText
                    If Not String.IsNullOrEmpty(activityQuestionAnswer.Answer) Then
                        txtQuestionAnswerText.Text = activityQuestionAnswer.Answer
                    End If
                    plhFreeTextField.Visible = True
                Case Is = GlobalConstants.CHECKBOX
                    Dim plhCheckbox As PlaceHolder = CType(e.Item.FindControl("plhCheckbox"), PlaceHolder)
                    Dim chkQuestionCheck As CheckBox = CType(e.Item.FindControl("chkQuestionCheckText"), CheckBox)
                    Dim lblQuestionCheckText As Label = CType(e.Item.FindControl("lblQuestionCheckText"), Label)
                    lblQuestionCheckText.Text = activityQuestionAnswer.QuestionText
                    If Not String.IsNullOrEmpty(activityQuestionAnswer.Answer) Then
                        chkQuestionCheck.Checked = CType(activityQuestionAnswer.Answer, Boolean)
                    End If
                    plhCheckbox.Visible = True
                Case Is = GlobalConstants.QUESTION_DATE
                    Dim plhDate As PlaceHolder = CType(e.Item.FindControl("plhDate"), PlaceHolder)
                    Dim txtDate As TextBox = CType(e.Item.FindControl("txtDate"), TextBox)
                    Dim lblDate As Label = CType(e.Item.FindControl("lblDate"), Label)

                    plhDate.Visible = True
                    If Not String.IsNullOrEmpty(activityQuestionAnswer.Answer) Then
                        txtDate.Text = activityQuestionAnswer.Answer
                    End If
                    lblDate.Text = activityQuestionAnswer.QuestionText
                Case Is = GlobalConstants.LIST_OF_ANSWERS
                    Dim plhListOfAnswers As PlaceHolder = CType(e.Item.FindControl("plhListOfAnswers"), PlaceHolder)
                    Dim ddlAnswers As DropDownList = CType(e.Item.FindControl("ddlAnswers"), DropDownList)
                    Dim lblListOfAnswers As Label = CType(e.Item.FindControl("lblListOfAnswers"), Label)
                    plhListOfAnswers.Visible = True

                    lblListOfAnswers.Text = activityQuestionAnswer.QuestionText

                    ddlAnswers.DataSource = activityQuestionAnswer.ListOfAnswers
                    ddlAnswers.DataValueField = "Key"
                    ddlAnswers.DataTextField = "Value"
                    ddlAnswers.DataBind()

                    If Not String.IsNullOrEmpty(activityQuestionAnswer.Answer) Then
                        Dim li As ListItem = ddlAnswers.Items.FindByText(activityQuestionAnswer.Answer)
                        If li IsNot Nothing Then ddlAnswers.SelectedIndex = ddlAnswers.Items.IndexOf(li)
                    End If
            End Select
            Dim answerType As HiddenField = CType(e.Item.FindControl("hdfAnswerType"), HiddenField)
            answerType.Value = activityQuestionAnswer.AnswerType
            Dim templateID As HiddenField = CType(e.Item.FindControl("hdfTemplateID"), HiddenField)
            templateID.Value = activityQuestionAnswer.TemplateID
            Dim questionID As HiddenField = CType(e.Item.FindControl("hdfQuestionID"), HiddenField)
            questionID.Value = activityQuestionAnswer.QuestionID
            Dim rememberedAnswer As HiddenField = CType(e.Item.FindControl("hdfRememberedAnswer"), HiddenField)
            rememberedAnswer.Value = activityQuestionAnswer.RememberedAnswer
            Dim totalNumberOfSeats As HiddenField = CType(e.Item.FindControl("hdfTotalNumberOfSeats"), HiddenField)
            totalNumberOfSeats.Value = activityQuestionAnswer.TotalNumberOfSeats
            Dim numberOfQuestion As HiddenField = CType(e.Item.FindControl("hdfNumberOfQuestion"), HiddenField)
            numberOfQuestion.Value = activityQuestionAnswer.NumberOfQuestions
            hdfDatePickerClearDateText.Value = _viewModelHospitalityBooking.GetPageText("DatePickerClearDateText")
            Dim isQuestionPerBooking As HiddenField = CType(e.Item.FindControl("hdfIsQuestionPerBooking"), HiddenField)
            isQuestionPerBooking.Value = activityQuestionAnswer.IsQuestionPerBooking
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Get Question and answer for update
    ''' </summary>
    ''' <returns>List of question and answer</returns>
    Private Function getQuestionAndAnswer() As List(Of ActivityQuestionAnswer)
        Dim activityQuestionAnswerList As New List(Of ActivityQuestionAnswer)
        For Each qAndAnswer As RepeaterItem In rptProductQuestions.Items
            Dim answerType As HiddenField = CType(qAndAnswer.FindControl("hdfAnswerType"), HiddenField)
            Dim activityQuestionAnswer As New ActivityQuestionAnswer

            Select Case Convert.ToInt32(answerType.Value)
                Case Is = GlobalConstants.FREE_TEXT_FIELD
                    Dim txtQuestionAnswerText As TextBox = CType(qAndAnswer.FindControl("txtQuestionAnswerText"), TextBox)
                    Dim lblQuestionText As Label = CType(qAndAnswer.FindControl("lblQuestionText"), Label)
                    activityQuestionAnswer.QuestionText = lblQuestionText.Text
                    activityQuestionAnswer.Answer = txtQuestionAnswerText.Text
                Case Is = GlobalConstants.CHECKBOX
                    Dim lblQuestionCheckText As Label = CType(qAndAnswer.FindControl("lblQuestionCheckText"), Label)
                    Dim chkQuestionCheckText As CheckBox = CType(qAndAnswer.FindControl("chkQuestionCheckText"), CheckBox)
                    activityQuestionAnswer.QuestionText = lblQuestionCheckText.Text
                    activityQuestionAnswer.Answer = CType(chkQuestionCheckText.Checked, String)
                Case Is = GlobalConstants.QUESTION_DATE
                    Dim txtDate As TextBox = CType(qAndAnswer.FindControl("txtDate"), TextBox)
                    Dim lblDate As Label = CType(qAndAnswer.FindControl("lblDate"), Label)
                    activityQuestionAnswer.QuestionText = lblDate.Text
                    Try
                        Dim result As Date = Date.ParseExact(txtDate.Text, "d", New System.Globalization.CultureInfo(ModuleDefaults.Culture))
                        activityQuestionAnswer.Answer = Convert.ToDateTime(txtDate.Text.Trim, New System.Globalization.CultureInfo(ModuleDefaults.Culture)).ToString("dd/MM/yyyy")
                    Catch err As FormatException
                        activityQuestionAnswer.Answer = String.Empty
                    End Try
                Case Is = GlobalConstants.LIST_OF_ANSWERS
                    Dim ddlAnswers As DropDownList = CType(qAndAnswer.FindControl("ddlAnswers"), DropDownList)
                    Dim lblListOfAnswers As Label = CType(qAndAnswer.FindControl("lblListOfAnswers"), Label)
                    activityQuestionAnswer.QuestionText = lblListOfAnswers.Text
                    activityQuestionAnswer.Answer = ddlAnswers.SelectedItem.Text
            End Select
            Dim templateID As HiddenField = CType(qAndAnswer.FindControl("hdfTemplateID"), HiddenField)
            Dim questionID As HiddenField = CType(qAndAnswer.FindControl("hdfQuestionID"), HiddenField)
            Dim rememberedAnswer As HiddenField = CType(qAndAnswer.FindControl("hdfRememberedAnswer"), HiddenField)
            Dim totalNumberOfSeats As HiddenField = CType(qAndAnswer.FindControl("hdfTotalNumberOfSeats"), HiddenField)
            Dim numberOfQuestion As HiddenField = CType(qAndAnswer.FindControl("hdfNumberOfQuestion"), HiddenField)
            Dim isQuestionPerBooking As HiddenField = CType(qAndAnswer.FindControl("hdfIsQuestionPerBooking"), HiddenField)
            activityQuestionAnswer.TemplateID = templateID.Value
            activityQuestionAnswer.QuestionID = questionID.Value
            activityQuestionAnswer.RememberedAnswer = rememberedAnswer.Value
            activityQuestionAnswer.TotalNumberOfSeats = totalNumberOfSeats.Value
            activityQuestionAnswer.NumberOfQuestions = numberOfQuestion.Value
            activityQuestionAnswer.IsQuestionPerBooking = isQuestionPerBooking.Value
            activityQuestionAnswerList.Add(activityQuestionAnswer)

        Next
        Return activityQuestionAnswerList
    End Function

    ''' <summary>
    ''' Setup the hospitality bookings input model and return the model for use
    ''' </summary>
    ''' <returns>The formatted input model based on form data</returns>
    ''' <remarks></remarks>
    Private Function setupHospitalityBookingInputModel() As HospitalityBookingInputModel
        Dim inputModel As New HospitalityBookingInputModel
        inputModel.ProductCode = String.Empty
        inputModel.CallId = 0
        inputModel.CustomerNumber = Profile.User.Details.LoginID
        If Request.QueryString("product") IsNot Nothing Then
            inputModel.ProductCode = Request.QueryString("product").ToString()
        End If
        If Request.QueryString("packageid") IsNot Nothing Then
            inputModel.PackageID = Request.QueryString("packageid").ToString()
        End If
        If Request.QueryString("callid") IsNot Nothing AndAlso Request.QueryString("status") IsNot Nothing Then
            inputModel.CallId = Request.QueryString("callid").ToString()
            _bookingStatus = Request.QueryString("status").ToString()
            _viewingBookingFromEnquiryPage = True
            If (_bookingStatus = GlobalConstants.SOLD_BOOKING_STATUS) Then
                _viewingBookingAsOutputOnly = True
                inputModel.IsSoldBooking = True
            ElseIf (_bookingStatus = GlobalConstants.RESERVATION_BOOKING_STATUS) Then
                inputModel.Status = _bookingStatus
            End If
            inputModel.RefreshBasket = True
        End If
        inputModel.BasketID = Profile.Basket.Basket_Header_ID
        ProductCode = inputModel.ProductCode
        PackageId = inputModel.PackageID
        CallId = inputModel.CallId
        HttpContext.Current.Session("LastAddedPackageId") = PackageId
        HttpContext.Current.Session("LastAddedProductCode") = ProductCode
        Return inputModel
    End Function

    ''' <summary>
    ''' Setup the hospitality details input model and return the model for use
    ''' </summary>
    ''' <returns>The formatted input model based on form data</returns>
    ''' <remarks></remarks>
    Private Function setupHospitalityDetailsInputModel() As HospitalityDetailsInputModel
        Dim inputModelHospitalityDetails As New HospitalityDetailsInputModel
        inputModelHospitalityDetails.ProductCode = ProductCode
        inputModelHospitalityDetails.PackageID = PackageId
        Return inputModelHospitalityDetails
    End Function

    ''' <summary>
    ''' Get the components being amended in the package
    ''' </summary>
    ''' <returns>The list of amend components</returns>
    ''' <remarks></remarks>
    Private Function getAmendComponents() As List(Of AmendComponent)
        Dim updatedComponents As New List(Of AmendComponent)
        If AgentProfile.IsAgent Then
            For Each item As RepeaterItem In rptBoxOfficeNonSeatedComponents.Items
                Dim component As New AmendComponent
                Dim hdfComponentID As HiddenField = CType(item.FindControl("hdfComponentID"), HiddenField)
                Dim ddlComponentQuantity As DropDownList = CType(item.FindControl("ddlComponentQuantity"), DropDownList)
                Dim txtComponentDiscountPercentage As TextBox = CType(item.FindControl("txtComponentDiscountPercentage"), TextBox)
                component.ComponentId = hdfComponentID.Value
                component.Quantity = ddlComponentQuantity.SelectedValue
                Dim ComponentDiscountPercentage As Decimal = 0
                If Decimal.TryParse(txtComponentDiscountPercentage.Text, ComponentDiscountPercentage) Then
                    component.Discount = ComponentDiscountPercentage
                End If
                updatedComponents.Add(component)
            Next

            For Each item As RepeaterItem In rptBoxOfficeSeatedComponents.Items
                Dim component As New AmendComponent
                Dim hdfComponentID As HiddenField = CType(item.FindControl("hdfComponentID"), HiddenField)
                Dim txtComponentDiscountPercentage As TextBox = CType(item.FindControl("txtComponentDiscountPercentage"), TextBox)
                component.ComponentId = hdfComponentID.Value
                Dim componentDiscountPercentage As Decimal = 0
                If Decimal.TryParse(txtComponentDiscountPercentage.Text, componentDiscountPercentage) Then
                    component.Discount = componentDiscountPercentage
                End If
                updatedComponents.Add(component)
            Next
        End If
        Return updatedComponents
    End Function

    ''' <summary>
    ''' Get the seated components being amended in the package
    ''' </summary>
    ''' <returns>The list of seat allocation</returns>
    ''' <remarks></remarks>
    Private Function getSeatAllocations() As List(Of SeatAllocation)
        Dim updatedSeats As New List(Of SeatAllocation)
        If AgentProfile.IsAgent Then
            For Each componentItem As RepeaterItem In rptBoxOfficeSeatedComponents.Items
                Dim rptBoxOfficeSeats As Repeater = CType(componentItem.FindControl("rptBoxOfficeSeats"), Repeater)
                For Each seatsItem In rptBoxOfficeSeats.Items
                    Dim seat As New SeatAllocation
                    Dim hdfComponentID As HiddenField = CType(seatsItem.FindControl("hdfComponentID"), HiddenField)
                    Dim ddlCustomer As DropDownList = CType(seatsItem.FindControl("ddlCustomer"), DropDownList)
                    Dim ddlPriceBands As DropDownList = CType(seatsItem.FindControl("ddlPriceBands"), DropDownList)
                    Dim ddlPriceCodes As DropDownList = CType(seatsItem.FindControl("ddlPriceCodes"), DropDownList)
                    Dim hdfSeatDetails As HiddenField = CType(seatsItem.FindControl("hdfSeatDetails"), HiddenField)
                    Dim hdfAlphaSuffix As HiddenField = CType(seatsItem.FindControl("hdfAlphaSuffix"), HiddenField)
                    seat.ComponentId = hdfComponentID.Value
                    seat.CustomerNumber = ddlCustomer.SelectedValue
                    seat.PriceBand = ddlPriceBands.SelectedValue
                    seat.Seat = hdfSeatDetails.Value
                    seat.AlphaSuffix = hdfAlphaSuffix.Value
                    seat.PriceCode = ddlPriceCodes.SelectedValue
                    seat.Action = OperationMode.Amend
                    updatedSeats.Add(seat)
                Next
            Next
        Else
            For Each componentItem As RepeaterItem In rptPWSSeatedComponents.Items
                Dim rptPWSSeats As Repeater = CType(componentItem.FindControl("rptPWSSeats"), Repeater)
                For Each seatsItem In rptPWSSeats.Items
                    Dim seat As New SeatAllocation
                    Dim hdfComponentID As HiddenField = CType(seatsItem.FindControl("hdfComponentID"), HiddenField)
                    Dim ddlCustomer As DropDownList = CType(seatsItem.FindControl("ddlCustomer"), DropDownList)
                    Dim ddlPriceBands As DropDownList = CType(seatsItem.FindControl("ddlPriceBands"), DropDownList)
                    Dim hdfSeatDetails As HiddenField = CType(seatsItem.FindControl("hdfSeatDetails"), HiddenField)
                    Dim hdfAlphaSuffix As HiddenField = CType(seatsItem.FindControl("hdfAlphaSuffix"), HiddenField)
                    Dim hdfPriceCode As HiddenField = CType(seatsItem.FindControl("hdfPriceCode"), HiddenField)
                    seat.ComponentId = hdfComponentID.Value
                    seat.CustomerNumber = ddlCustomer.SelectedValue
                    seat.PriceBand = ddlPriceBands.SelectedValue
                    seat.Seat = hdfSeatDetails.Value
                    seat.AlphaSuffix = hdfAlphaSuffix.Value
                    seat.PriceCode = hdfPriceCode.Value
                    seat.Action = OperationMode.Amend
                    updatedSeats.Add(seat)
                Next
            Next
        End If
        Return updatedSeats
    End Function

    ''' <summary>
    ''' Get ImageUrl of the playing area from a perspective of the package seat
    ''' </summary>
    ''' <param name="packageCode">The packageCode</param>
    ''' <returns>Image path</returns>
    Private Function viewFromAreaImgURL(ByVal packageCode As String) As String
        Dim imgURL As String = String.Empty
        Dim viewImageName As String = packageCode
        imgURL = ImagePath.getImagePath("PRODCORPORATE", viewImageName, _businessUnit, _partner)
        If imgURL.Contains(ModuleDefaults.MissingImagePath) Then
            imgURL = String.Empty
        End If
        Return imgURL
    End Function

    ''' <summary>
    ''' Get object question and answer with blank answer for add extra set of answer
    ''' </summary>
    ''' <param name="questionAnswer"></param>
    ''' <returns></returns>
    Private Function GetBlankQuestionAnswer(ByRef questionAnswer As ActivityQuestionAnswer) As ActivityQuestionAnswer
        Dim newQuestionAnswer = New ActivityQuestionAnswer
        newQuestionAnswer.QuestionID = questionAnswer.QuestionID
        newQuestionAnswer.Answer = String.Empty
        newQuestionAnswer.TemplateID = questionAnswer.TemplateID
        newQuestionAnswer.QuestionText = questionAnswer.QuestionText
        newQuestionAnswer.AnswerType = questionAnswer.AnswerType
        newQuestionAnswer.RegularExpression = questionAnswer.RegularExpression
        newQuestionAnswer.HyperLink = questionAnswer.HyperLink
        newQuestionAnswer.Sequence = questionAnswer.Sequence
        newQuestionAnswer.ListOfAnswers = questionAnswer.ListOfAnswers
        newQuestionAnswer.TotalNumberOfSeats = questionAnswer.TotalNumberOfSeats
        newQuestionAnswer.NumberOfQuestions = questionAnswer.NumberOfQuestions
        newQuestionAnswer.IsQuestionPerBooking = questionAnswer.IsQuestionPerBooking
        Return newQuestionAnswer
    End Function

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set the customer drop down list for the seat components repeater
    ''' </summary>
    ''' <param name="e">The current repeater item</param>
    ''' <remarks></remarks>
    Private Sub setCustomerDropDownList(ByRef e As RepeaterItemEventArgs)
        Dim ddlCustomer As DropDownList = CType(e.Item.FindControl("ddlCustomer"), DropDownList)
        Dim hospitalitySeatList As HospitalitySeatDetails = CType(e.Item.DataItem, HospitalitySeatDetails)
        Dim friendsAndFamilyList As List(Of FriendsAndFamilyDetails) = hospitalitySeatList.FriendsAndFamilyDetails
        Dim currentCustomerItemText As String = _viewModelHospitalityBooking.GetPageText("CustomerDropDownDisplayFormat")
        Dim item As New ListItem
        Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)

        currentCustomerItemText = currentCustomerItemText.Replace("<<CustomerNumber>>", Profile.User.Details.LoginID.TrimStart(GlobalConstants.LEADING_ZEROS))
        currentCustomerItemText = currentCustomerItemText.Replace("<<CustomerForename>>", Profile.User.Details.Forename)
        currentCustomerItemText = currentCustomerItemText.Replace("<<CustomerSurname>>", Profile.User.Details.Surname)
        ddlCustomer.Items.Clear()
        ddlCustomer.Attributes.Add("onchange", "CustomerDDLRedirect('" & ddlCustomer.ClientID & "');")
        item.Text = currentCustomerItemText
        item.Value = Profile.User.Details.LoginID
        If hospitalitySeatList.CustomerNumber = Profile.User.Details.LoginID Then
            item.Selected = True
        End If
        ddlCustomer.Items.Add(item)
        For Each customer As FriendsAndFamilyDetails In friendsAndFamilyList
            item = New ListItem
            currentCustomerItemText = _viewModelHospitalityBooking.GetPageText("CustomerDropDownDisplayFormat")
            currentCustomerItemText = currentCustomerItemText.Replace("<<CustomerNumber>>", customer.AssociatedCustomerNumber.TrimStart(GlobalConstants.LEADING_ZEROS))
            currentCustomerItemText = currentCustomerItemText.Replace("<<CustomerForename>>", customer.Forename)
            currentCustomerItemText = currentCustomerItemText.Replace("<<CustomerSurname>>", customer.Surname)
            item.Value = customer.AssociatedCustomerNumber
            item.Text = currentCustomerItemText
            If hospitalitySeatList.CustomerNumber = customer.AssociatedCustomerNumber Then
                item.Selected = True
            End If
            ddlCustomer.Items.Add(item)
        Next
        If ModuleDefaults.FriendsAndFamily Then
            hdfFFRedirectURL.Value = ResolveUrl("~/PagesPublic/Profile/Registration.aspx?source=fandfhospitality&returnurl=" & Server.UrlEncode(Request.Url.AbsoluteUri))
            ddlCustomer.Items.Add(New ListItem(_viewModelHospitalityBooking.GetPageText("CustomerDDLFAndFOptionText"), hdfFFRedirectURL.Value))
        End If

        Dim seat As HospitalitySeatDetails = TryCast(e.Item.DataItem, HospitalitySeatDetails)
        If AgentProfile.IsAgent AndAlso Not seat Is Nothing Then
            Dim url As New StringBuilder
            url.Append(ResolveUrl("~/PagesPublic/Profile/CustomerSelection.aspx?source=customerselecthospitalitybooking&displayMode=basket"))
            url.Append("&seat=").Append(seat.SeatDetails)
            url.Append("&productCode=").Append(seat.ProductCode)
            url.Append("&priceCode=").Append(seat.PriceCode)
            url.Append("&priceBand=").Append(seat.PriceBand)
            url.Append("&bulkId=").Append(seat.BulkID)
            url.Append("&componentID=").Append(seat.ComponentID)

            For Each basketItem As TalentBasketItem In Profile.Basket.BasketItems
                If basketItem.PACKAGE_ID = PackageId AndAlso basketItem.Product = ProductCode Then
                    url.Append("&productType=").Append(basketItem.PRODUCT_TYPE)
                    url.Append("&productSubType=").Append(basketItem.PRODUCT_SUB_TYPE)
                    url.Append("&packageID=").Append(basketItem.PACKAGE_ID)
                    url.Append("&basketId=").Append(basketItem.Basket_Detail_ID)
                    url.Append("&fulfilmentMethod=").Append(basketItem.CURR_FULFIL_SLCTN)
                    url.Append("&originalCustomer=").Append(basketItem.LOGINID)
                    If basketItem.BULK_SALES_ID > 0 Then
                        url.Append("&bulkQuantity=").Append(Math.Round(basketItem.Quantity))
                    Else
                        url.Append("&bulkQuantity=0")
                    End If
                    Exit For
                End If
            Next

            url.Append("&returnurl=").Append(Server.UrlEncode(Request.Url.AbsoluteUri))
            Dim newCustomerText As String = _viewModelHospitalityBooking.GetPageText("CustomerDDLSelectNewCustomerOptionText")
            ddlCustomer.Items.Add(New ListItem(newCustomerText, url.ToString))
            hdfNewCustomerRedirectText.Value = newCustomerText

        End If
        If _viewingBookingAsOutputOnly Then
            ddlCustomer.Enabled = False
        End If
    End Sub

    ''' <summary>
    ''' Set the price bands drop down list for the seat components repeater
    ''' </summary>
    ''' <param name="e">The current repeater item</param>
    ''' <remarks></remarks>
    Private Sub setPriceBandsDropDownList(ByRef e As RepeaterItemEventArgs, ByVal isPWSRepeater As Boolean)
        Dim ddlPriceBands As DropDownList = CType(e.Item.FindControl("ddlPriceBands"), DropDownList)
        Dim ltlPriceBand As New Literal
        Dim hospitalitySeatList As HospitalitySeatDetails = CType(e.Item.DataItem, HospitalitySeatDetails)
        Dim productPriceBandList As List(Of ProductPriceBandDetails) = hospitalitySeatList.ProductPriceBands
        Dim item As New ListItem
        If isPWSRepeater Then
            ltlPriceBand = CType(e.Item.FindControl("ltlPriceBand"), Literal)
            If (_viewModelHospitalityBooking.PackageDetailsList(0).PricingMethod = GlobalConstants.TICKETING_PRICING) Then
                ddlPriceBands.Visible = True
                ddlPriceBands.Enabled = (Not _viewingBookingAsOutputOnly)
                ltlPriceBand.Visible = False
            Else
                ddlPriceBands.Visible = False
                ltlPriceBand.Visible = True
                ltlPriceBand.Text = productPriceBandList(0).PriceBandDescription
            End If
        Else
            If (_viewModelHospitalityBooking.PackageDetailsList(0).PricingMethod <> GlobalConstants.TICKETING_PRICING) Or _viewingBookingAsOutputOnly Then
                ddlPriceBands.Enabled = False
                If Not _viewingBookingAsOutputOnly Then
                    ddlPriceBands.Attributes.Add("title", _viewModelHospitalityBooking.GetPageText("CantChangePriceBandWarningText"))
                End If
            Else
                ddlPriceBands.Enabled = True
            End If
        End If
        For Each priceBand As ProductPriceBandDetails In productPriceBandList
            item = New ListItem
            item.Value = priceBand.PriceBand
            item.Text = priceBand.PriceBandDescription
            If hospitalitySeatList.PriceBand = priceBand.PriceBand Then
                item.Selected = True
            End If
            ddlPriceBands.Items.Add(item)
        Next
    End Sub

    ''' <summary>
    ''' Set the price codes drop down list for the seat components repeater
    ''' </summary>
    ''' <param name="e">The current repeater item</param>
    ''' <remarks></remarks>
    Private Sub setPriceCodesDropDownList(ByRef e As RepeaterItemEventArgs)
        Dim ddlPriceCodes As DropDownList = CType(e.Item.FindControl("ddlPriceCodes"), DropDownList)
        Dim hospitalitySeatList As HospitalitySeatDetails = CType(e.Item.DataItem, HospitalitySeatDetails)
        Dim productPriceCodeList As List(Of ProductPriceCodeDetails) = hospitalitySeatList.ProductPriceCodes
        Dim item As New ListItem
        If (_viewModelHospitalityBooking.PackageDetailsList(0).PricingMethod <> GlobalConstants.TICKETING_PRICING) Or _viewingBookingAsOutputOnly Then
            ddlPriceCodes.Enabled = False
            If Not _viewingBookingAsOutputOnly Then
                ddlPriceCodes.Attributes.Add("title", _viewModelHospitalityBooking.GetPageText("CantChangePriceCodeWarningText"))
            End If
        Else
            ddlPriceCodes.Enabled = True
        End If
        If Not String.IsNullOrEmpty(hospitalitySeatList.PriceCode) Then
            For Each priceCode As ProductPriceCodeDetails In productPriceCodeList
                item = New ListItem
                item.Value = priceCode.PriceCode
                item.Text = priceCode.PriceCode
                If hospitalitySeatList.PriceCode = priceCode.PriceCode Then
                    item.Selected = True
                End If
                ddlPriceCodes.Items.Add(item)
            Next
        End If
    End Sub

    ''' <summary>
    ''' Update the booking with the requested changes
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub performBookingUpdate()
        Dim priceIncludingVATBeforePackageDiscount As Decimal = 0
        Dim enteredDiscountedPackagePrice As Decimal = 0

        ' PriceIncludingVATDecimal is price after current package discount applied so need to add current discount to it to get full package price  
        If plhDiscountTotal.Visible And Decimal.TryParse(txtBookingTotalCostIncVAT.Text, enteredDiscountedPackagePrice) Then
            priceIncludingVATBeforePackageDiscount = _viewModelHospitalityBooking.PackageDetailsList(0).PriceIncludingVAT + _viewModelHospitalityBooking.PackageDetailsList(0).PackageDiscountedByValue
            _inputModelHospitalityBooking.PackageDiscountedByValue = priceIncludingVATBeforePackageDiscount - enteredDiscountedPackagePrice
        End If
        _inputModelHospitalityBooking.Mode = OperationMode.Amend
        _inputModelHospitalityBooking.UpdateDiscount = True
        _inputModelHospitalityBooking.SeatAllocations = getSeatAllocations()
        _inputModelHospitalityBooking.AmendComponents = getAmendComponents()
        _inputModelHospitalityBooking.ActivityQuestionAnswerList = getQuestionAndAnswer()
        setInputmodelForUpdate()
        _viewModelHospitalityBooking = _bookingBuilder.UpdateHospitalityBooking(_inputModelHospitalityBooking)
        If _viewModelHospitalityBooking.Error.HasError Then
            ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
        End If
    End Sub

    ''' <summary>
    ''' Removes all discounts but does no other updates
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub removeAllDiscounts()
        _inputModelHospitalityBooking.RemoveAllDiscounts = True
        setInputmodelForUpdate()
        _viewModelHospitalityBooking = _bookingBuilder.UpdateHospitalityBooking(_inputModelHospitalityBooking)
        If _viewModelHospitalityBooking.Error.HasError Then
            ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
        End If
    End Sub

    ''' <summary>
    ''' Set the input model for an update on changed sreen fields.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setInputmodelForUpdate()
        If plhLeadSource.Visible AndAlso ddlLeadSource.Items.Count > 1 Then
            If ddlLeadSource.SelectedValue <> "-1" Then
                _inputModelHospitalityBooking.LeadSourceID = ddlLeadSource.SelectedValue
            End If
        End If

        If plhBookingType.Visible Then
            If rblMarkOrderFor.Items(0).Selected Then
                _inputModelHospitalityBooking.MarkOrderFor = GlobalConstants.MARK_FOR_PERSONAL
            ElseIf rblMarkOrderFor.Items(1).Selected Then
                _inputModelHospitalityBooking.MarkOrderFor = GlobalConstants.MARK_FOR_BUISINESS
            End If
        End If
    End Sub

    ''' <summary>
    ''' Cancel the booking, handle errors and redirect
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub performBookingCancel()
        _inputModelHospitalityBooking.SavePackageMode = GlobalConstants.CANCELBACKENDCOMPONENTDETAILS
        _viewModelHospitalityBooking = _bookingBuilder.DeleteHospitalityBooking(_inputModelHospitalityBooking)
        If _viewModelHospitalityBooking.Error.HasError Then
            ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
        Else
            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        End If
    End Sub

    ''' <summary>
    ''' Delete whole component from the booking page
    ''' </summary>
    ''' <param name="e">The item command that the delete action has been triggered from</param>
    ''' <remarks></remarks>
    Private Sub removeAllForComponent(ByRef e As RepeaterCommandEventArgs)
        Dim priceIncludingVATBeforePackageDiscount As Decimal = 0
        Dim enteredDiscountedPackagePrice As Decimal = 0
        ' PriceIncludingVATDecimal is price after current package discount applied so need to add current discount to it to get full package price  
        If plhDiscountTotal.Visible And Decimal.TryParse(txtBookingTotalCostIncVAT.Text, enteredDiscountedPackagePrice) Then
            priceIncludingVATBeforePackageDiscount = _viewModelHospitalityBooking.PackageDetailsList(0).PriceIncludingVAT + _viewModelHospitalityBooking.PackageDetailsList(0).PackageDiscountedByValue
            _inputModelHospitalityBooking.PackageDiscountedByValue = priceIncludingVATBeforePackageDiscount - enteredDiscountedPackagePrice
        End If
        _inputModelHospitalityBooking.Mode = OperationMode.Delete
        _inputModelHospitalityBooking.ComponentID = e.CommandArgument
        setInputmodelForUpdate()
        _viewModelHospitalityBooking = _bookingBuilder.UpdateHospitalityBooking(_inputModelHospitalityBooking)
        If _viewModelHospitalityBooking.Error.HasError Then
            ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
        Else
            Response.Redirect(Request.Url.AbsoluteUri)
        End If
    End Sub

    ''' <summary>
    ''' Delete single item from the booking based on the item the user has selected
    ''' </summary>
    ''' <param name="e">The item command that the delete action has been triggered from</param>
    ''' <remarks></remarks>
    Private Sub removeForComponent(ByRef e As RepeaterCommandEventArgs)
        Dim priceIncludingVATBeforePackageDiscount As Decimal = 0
        Dim enteredDiscountedPackagePrice As Decimal = 0
        ' PriceIncludingVATDecimal is price after current package discount applied so need to add current discount to it to get full package price  
        If plhDiscountTotal.Visible And Decimal.TryParse(txtBookingTotalCostIncVAT.Text, enteredDiscountedPackagePrice) Then
            priceIncludingVATBeforePackageDiscount = _viewModelHospitalityBooking.PackageDetailsList(0).PriceIncludingVAT + _viewModelHospitalityBooking.PackageDetailsList(0).PackageDiscountedByValue
            _inputModelHospitalityBooking.PackageDiscountedByValue = priceIncludingVATBeforePackageDiscount - enteredDiscountedPackagePrice
        End If
        Dim hdfSeatDetails As HiddenField = CType(e.Item.FindControl("hdfSeatDetails"), HiddenField)
        Dim hdfAlphaSuffix As HiddenField = CType(e.Item.FindControl("hdfAlphaSuffix"), HiddenField)
        Dim hdfComponentType As HiddenField = CType(e.Item.FindControl("hdfComponentType"), HiddenField)
        Dim seat As New SeatAllocation
        Dim seatAllocationList As New List(Of SeatAllocation)
        Dim perSeatQuestionsList As New List(Of ActivityQuestionAnswer)
        seat.Seat = hdfSeatDetails.Value
        seat.AlphaSuffix = hdfAlphaSuffix.Value
        seat.Action = OperationMode.Delete
        seat.ComponentId = e.CommandArgument
        seat.ProductCode = _inputModelHospitalityBooking.ProductCode
        seatAllocationList.Add(seat)
        _inputModelHospitalityBooking.SeatAllocations = seatAllocationList
        _inputModelHospitalityBooking.Mode = OperationMode.Amend
        If (hdfComponentType.Value = "A") Then  'Q&A are only for Availability component
            _inputModelHospitalityBooking.ActivityQuestionAnswerList = getQuestionAndAnswer()
            If _inputModelHospitalityBooking.ActivityQuestionAnswerList IsNot Nothing And _inputModelHospitalityBooking.ActivityQuestionAnswerList.Count > 0 Then
                For Each activityQuestionAnswer As ActivityQuestionAnswer In _inputModelHospitalityBooking.ActivityQuestionAnswerList
                    If Not activityQuestionAnswer.IsQuestionPerBooking Then
                        If Not perSeatQuestionsList.Exists(Function(x) x.QuestionID = activityQuestionAnswer.QuestionID) Then
                            perSeatQuestionsList.Add(activityQuestionAnswer)
                        End If
                    End If
                Next
                Dim amendedQuestionAnswerList As New List(Of ActivityQuestionAnswer)
                For index = 0 To (_inputModelHospitalityBooking.ActivityQuestionAnswerList.Count - perSeatQuestionsList.Count) - 1
                    Dim activityQuestionAnswer As ActivityQuestionAnswer
                    activityQuestionAnswer = _inputModelHospitalityBooking.ActivityQuestionAnswerList(index)
                    amendedQuestionAnswerList.Add(activityQuestionAnswer)
                Next
                _inputModelHospitalityBooking.ActivityQuestionAnswerList = amendedQuestionAnswerList
            End If
        End If
        _inputModelHospitalityBooking.ComponentID = e.CommandArgument
        setInputmodelForUpdate()
        _viewModelHospitalityBooking = _bookingBuilder.UpdateHospitalityBooking(_inputModelHospitalityBooking)
        If _viewModelHospitalityBooking.Error.HasError Then
            ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
        Else
            Response.Redirect(Request.Url.AbsoluteUri)
        End If
    End Sub

    ''' <summary>
    ''' Add an additional seat for this component
    ''' </summary>
    ''' <param name="e">The item command that the add seat action has been triggered from</param>
    ''' <param name="isSeatedComponent">Is the component seat based</param>
    ''' <remarks></remarks>
    Private Sub addSeatUsingBestAvailable(ByRef e As RepeaterCommandEventArgs, ByVal isSeatedComponent As Boolean)
        Dim component As New AmendComponent
        Dim componentList As New List(Of AmendComponent)
        Dim hdfComponentID As HiddenField = CType(e.Item.FindControl("hdfComponentID"), HiddenField)
        Dim hdfQuantity As HiddenField = CType(e.Item.FindControl("hdfQuantity"), HiddenField)
        Dim hdfComponentType As HiddenField = CType(e.Item.FindControl("hdfComponentType"), HiddenField)
        Dim perSeatQuestionsList As New List(Of ActivityQuestionAnswer)
        component.ComponentId = e.CommandArgument
        component.Quantity = hdfQuantity.Value + 1
        componentList.Add(component)
        _inputModelHospitalityBooking.AmendComponents = componentList
        If (hdfComponentType.Value = "A") Then  'Q&A are only for Availability component
            _inputModelHospitalityBooking.ActivityQuestionAnswerList = getQuestionAndAnswer()
            If _inputModelHospitalityBooking.ActivityQuestionAnswerList IsNot Nothing And _inputModelHospitalityBooking.ActivityQuestionAnswerList.Count > 0 Then
                For Each activityQuestionAnswer As ActivityQuestionAnswer In _inputModelHospitalityBooking.ActivityQuestionAnswerList
                    If Not activityQuestionAnswer.IsQuestionPerBooking Then
                        If Not perSeatQuestionsList.Exists(Function(x) x.QuestionID = activityQuestionAnswer.QuestionID) Then
                            perSeatQuestionsList.Add(activityQuestionAnswer)
                        End If
                    End If
                Next
                Dim amendedQuestionAnswerList As New List(Of ActivityQuestionAnswer)
                amendedQuestionAnswerList = _inputModelHospitalityBooking.ActivityQuestionAnswerList
                For index = 0 To perSeatQuestionsList.Count - 1
                    Dim activityQuestionAnswer As ActivityQuestionAnswer
                    activityQuestionAnswer = GetBlankQuestionAnswer(perSeatQuestionsList(index))
                    amendedQuestionAnswerList.Add(activityQuestionAnswer)
                Next
                _inputModelHospitalityBooking.ActivityQuestionAnswerList = amendedQuestionAnswerList
            End If
        End If
        setInputmodelForUpdate()
        _viewModelHospitalityBooking = _bookingBuilder.UpdateHospitalityBooking(_inputModelHospitalityBooking)
        If _viewModelHospitalityBooking.Error.HasError Then
            ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
        Else
            Response.Redirect(Request.Url.AbsoluteUri)
        End If
    End Sub

    ''' <summary>
    ''' Change a component seat 
    ''' </summary>
    ''' <param name="e">The item command that the change seat action has been triggered from</param>
    ''' <remarks></remarks>
    Private Sub changeSeat(ByRef e As RepeaterCommandEventArgs)
        setInputmodelForUpdate()
        _viewModelHospitalityBooking = _bookingBuilder.UpdateHospitalityBooking(_inputModelHospitalityBooking)
        If _viewModelHospitalityBooking.Error.HasError Then
            ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
        Else
            Dim hdfFormattedSeatDetails As HiddenField = CType(e.Item.FindControl("hdfFormattedSeatDetails"), HiddenField)
            Dim hdfStandCode As HiddenField = CType(e.Item.FindControl("hdfStandCode"), HiddenField)
            Dim hdfAreaCode As HiddenField = CType(e.Item.FindControl("hdfAreaCode"), HiddenField)
            Dim navigateUrl As New StringBuilder
            navigateUrl.Append(ResolveUrl("~/PagesPublic/ProductBrowse/seatSelection.aspx")).Append("?product=").Append(ProductCode)
            navigateUrl.Append("&stand=").Append(hdfStandCode.Value)
            navigateUrl.Append("&area=").Append(hdfAreaCode.Value)
            navigateUrl.Append("&stadium=").Append(_viewModelHospitalityDetails.ProductStadium)
            navigateUrl.Append("&campaign=&type=").Append(_viewModelHospitalityDetails.ProductType)
            navigateUrl.Append("&productsubtype=").Append(_viewModelHospitalityDetails.ProductSubType)
            navigateUrl.Append("&callid=").Append(CallId)
            navigateUrl.Append("&oldseat=").Append(hdfFormattedSeatDetails.Value)
            navigateUrl.Append("&packageId=").Append(PackageId)
            navigateUrl.Append("&componentId=").Append(e.CommandArgument)
            navigateUrl.Append("&catmode=&priceBreakId=0&selectedMinimumPrice=0&selectedMaximumPrice=0")
            Response.Redirect(navigateUrl.ToString)
        End If
    End Sub

    ''' <summary>
    ''' Change all component seats 
    ''' </summary>
    ''' <param name="e">The item command that the change all seats action has been triggered from</param>
    ''' <remarks></remarks>
    Private Sub changeAllSeats(ByRef e As RepeaterCommandEventArgs)
        setInputmodelForUpdate()
        _inputModelHospitalityBooking.ActivityQuestionAnswerList = getQuestionAndAnswer()
        _viewModelHospitalityBooking = _bookingBuilder.UpdateHospitalityBooking(_inputModelHospitalityBooking)
        If _viewModelHospitalityBooking.Error.HasError Then
            ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
        Else
            Dim hdfStandCode As HiddenField = CType(e.Item.FindControl("hdfStandCode"), HiddenField)
            Dim hdfAreaCode As HiddenField = CType(e.Item.FindControl("hdfAreaCode"), HiddenField)
            Dim navigateUrl As New StringBuilder
            navigateUrl.Append(ResolveUrl("~/PagesPublic/ProductBrowse/seatSelection.aspx")).Append("?product=").Append(ProductCode)
            navigateUrl.Append("&stand=").Append(hdfStandCode.Value.Trim)
            navigateUrl.Append("&area=").Append(hdfAreaCode.Value.Trim)
            navigateUrl.Append("&stadium=").Append(_viewModelHospitalityDetails.ProductStadium)
            navigateUrl.Append("&campaign=&type=").Append(_viewModelHospitalityDetails.ProductType)
            navigateUrl.Append("&productsubtype=").Append(_viewModelHospitalityDetails.ProductSubType)
            navigateUrl.Append("&callid=").Append(CallId)
            navigateUrl.Append("&changeallseats=").Append("Y")
            navigateUrl.Append("&packageId=").Append(PackageId)
            navigateUrl.Append("&componentId=").Append(e.CommandArgument)
            navigateUrl.Append("&catmode=&priceBreakId=0&selectedMinimumPrice=0&selectedMaximumPrice=0")
            Response.Redirect(navigateUrl.ToString)
        End If
    End Sub

    ''' <summary>
    ''' Process the controller, the code actioning the input values if there are any.
    ''' </summary>
    ''' <param name="inputModelHospitalityBooking">Hospitality Booking Input Model</param>
    ''' <param name="inputModelHospitalityDetails">Hospitality Details Input Model</param>
    ''' <remarks></remarks>
    Private Sub processController(ByVal inputModelHospitalityBooking As HospitalityBookingInputModel, ByVal inputModelHospitalityDetails As HospitalityDetailsInputModel)
        If (inputModelHospitalityBooking.ProductCode.Length > 0 AndAlso inputModelHospitalityBooking.PackageID > 0) OrElse (inputModelHospitalityBooking.CallId > 0) Then
            _viewModelHospitalityBooking = _bookingBuilder.RetrieveHospitalityBooking(inputModelHospitalityBooking)
            If Not _viewModelHospitalityBooking.Error.HasError Then
                uscHospitalityFixturePackageHeader.CallID = _viewModelHospitalityBooking.PackageDetailsList(0).CallID
                uscReserveTickets.CallID = _viewModelHospitalityBooking.PackageDetailsList(0).CallID
                CallId = _viewModelHospitalityBooking.PackageDetailsList(0).CallID
                _viewingBookingCATMode = (_viewModelHospitalityBooking.PackageDetailsList(0).CatMode.Trim <> String.Empty)
                _inputModelHospitalityBooking.CallId = CallId
                _viewModelHospitalityDetails = _detailsBuilder.GetProductPackageDetails(inputModelHospitalityDetails)
                _bookingFound = True
            Else
                _bookingFound = False
                _viewModelHospitalityBooking = New HospitalityBookingViewModel(True)
            End If
        Else
            _bookingFound = False
            _viewModelHospitalityBooking = New HospitalityBookingViewModel(True)
        End If
    End Sub

    ''' <summary>
    ''' Create the booking page view
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createView()
        If _bookingFound Then
            plhBookingDetails.Visible = True
            uscHospitalityFixturePackageHeader.ProductCode = ProductCode
            uscHospitalityFixturePackageHeader.PackageId = PackageId
            If AgentProfile.IsAgent Then
                plhBoxOfficeMode.Visible = True
                plhPublicWebSalesMode.Visible = False
                IsPriceColumnVisible = (_viewModelHospitalityBooking.PackageDetailsList(0).PricingMethod <> GlobalConstants.PACKAGE_PRICING)
                rptBoxOfficeSeatedComponents.DataSource = _viewModelHospitalityBooking.ComponentDetailsList.Where(Function(x) x.CanAmendSeat = True)
                rptBoxOfficeSeatedComponents.DataBind()
                rptBoxOfficeSeatedComponents.Visible = (rptBoxOfficeSeatedComponents.Items.Count > 0)
                rptBoxOfficeNonSeatedComponents.DataSource = _viewModelHospitalityBooking.ComponentDetailsList.Where(Function(x) x.CanAmendSeat = False)
                rptBoxOfficeNonSeatedComponents.DataBind()
                plhBoxOfficePackageIncludedExtras.Visible = (rptBoxOfficeNonSeatedComponents.Items.Count > 0)
                plhBoxOfficeSeatedComponents.Visible = (rptBoxOfficeSeatedComponents.Items.Count > 0)
                plhBoxOfficeSTExceptionWarning.Visible = _viewModelHospitalityBooking.PackageDetailsList.FirstOrDefault().HasSTExceptions

                If plhBoxOfficeSTExceptionWarning.Visible Then
                    ltlSTExceptionWarningMessage.Text = _viewModelHospitalityBooking.GetPageText("SeasonTicketExceptionWarningText")
                End If
            Else
                plhBoxOfficeMode.Visible = False
                plhPublicWebSalesMode.Visible = True
                IsPriceColumnVisible = (_viewModelHospitalityBooking.PackageDetailsList(0).PricingMethod <> GlobalConstants.PACKAGE_PRICING)
                rptPWSSeatedComponents.DataSource = _viewModelHospitalityBooking.ComponentDetailsList.Where(Function(x) x.CanAmendSeat = True)
                rptPWSSeatedComponents.DataBind()
                rptPWSSeatedComponents.Visible = (rptPWSSeatedComponents.Items.Count > 0)
                rptPWSNonSeatedComponents.DataSource = _viewModelHospitalityBooking.ComponentDetailsList.Where(Function(x) x.CanAmendSeat = False)
                rptPWSNonSeatedComponents.DataBind()
                plhPWSPackageIncludedExtras.Visible = (rptPWSNonSeatedComponents.Items.Count > 0)
                plhPWSSeatedComponents.Visible = (rptPWSSeatedComponents.Items.Count > 0)
            End If
            If _viewModelHospitalityBooking.PackageDetailsList(0).PackageDiscountRemovedDueToPriceRecalc Then
                ltlWarningMessage.Text = _viewModelHospitalityBooking.GetPageText("PackageDiscountRemovedDueToPriceRecalcWarningText")
            End If

            If _viewingBookingFromEnquiryPage Then
                If _viewingBookingAsOutputOnly Then
                    lbtnCancel.Visible = False
                    lbtnContinue.Visible = False
                    lbtnUpdate.Visible = False
                    lbtnRemoveDiscount.Visible = False
                    lbtnUpdateForSoldBooking.Visible = True
                    If _viewModelHospitalityBooking.HasPrintableTickets AndAlso AgentProfile.IsAgent AndAlso AgentProfile.AgentPermissions.CanPrintHospitalityTickets Then
                        plhPrintBooking.Visible = True
                    Else
                        plhPrintBooking.Visible = False
                    End If
                Else

                    Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
                    lbtnUpdateForSoldBooking.Visible = False
                    plhPrintBooking.Visible = False
                End If
                lbtnSaveAsEnquiry.Visible = False
                lbtnBackToBookingEnquiry.Visible = True
            Else
                lbtnBackToBookingEnquiry.Visible = False
                lbtnSaveAsEnquiry.Visible = True
                lbtnUpdateForSoldBooking.Visible = False
                plhPrintBooking.Visible = False
            End If

            If (_viewModelHospitalityBooking.PackageDetailsList(0).BookingStatus = GlobalConstants.RESERVATION_BOOKING_STATUS) Then
                lbtnCancel.Visible = False
                lbtnBackToBookingEnquiry.Visible = True
                lbtnSaveAsEnquiry.Visible = False
            End If

            If _viewingBookingCATMode Then
                lbtnBackToBookingEnquiry.Visible = False
                lbtnSaveAsEnquiry.Visible = False
                lbtnUpdate.Visible = False
                lbtnUpdateForSoldBooking.Visible = False
                plhPrintBooking.Visible = False
            End If
            If (_bookingStatus = GlobalConstants.SOLD_BOOKING_STATUS) Then
                plhGenerateDocument.Visible = Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBooking.GetPageAttribute("ShowGenerateDocumentButton"))
                plhCreatePDF.Visible = Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBooking.GetPageAttribute("ShowCreatePDFButton"))
            Else
                plhGenerateDocument.Visible = False
                plhCreatePDF.Visible = False
            End If

            setPageLabels()
            setupBookingOptions()
            setupViewFromArea()
            setTotalCost()
            setHiddenFields()
            setQuestionAndAnswer()
            setupExtraComponents()
            ReservationSetup()
        Else
            plhBookingDetails.Visible = False
            If Request.QueryString("callid") IsNot Nothing Then
                If Not _viewModelHospitalityBooking.CustomerMatched Then
                    ltlBookingErrors.Text = _viewModelHospitalityBooking.GetPageText("BookingNotFoundError")
                Else
                    ltlBookingErrors.Text = _viewModelHospitalityBooking.GetPageText("BookingCannotBeLoadedError")
                End If
            Else
                ltlBookingErrors.Text = _viewModelHospitalityBooking.GetPageText("BookingNotFoundError")
            End If
        End If
    End Sub

    ''' <summary>
    ''' Setup the various text labels on the screen based on the view model
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setPageLabels()
        PackageDescription = _viewModelHospitalityBooking.PackageDetailsList(0).PackageDescription
        PackageDescriptionDetailsText = _viewModelHospitalityBooking.GetPageText("PackageDescriptionDetailsText").Replace("<<PackageDescription>>", PackageDescription)
        CustomerHeaderText = _viewModelHospitalityBooking.GetPageText("CustomerHeaderText")
        PriceBandHeaderText = _viewModelHospitalityBooking.GetPageText("PriceBandHeaderText")
        PriceHeaderText = _viewModelHospitalityBooking.GetPageText("PriceHeaderText")
        IncludedExtrasHeaderText = _viewModelHospitalityBooking.GetPageText("IncludedExtrasHeaderText")
        QuantityHeaderText = _viewModelHospitalityBooking.GetPageText("QuantityHeaderText")
        PercentageDiscountText = _viewModelHospitalityBooking.GetPageText("PercentageDiscountText")
        ChangeComponentSeatText = _viewModelHospitalityBooking.GetPageText("ChangeComponentSeatText")
        ChangeAllComponentSeatsText = _viewModelHospitalityBooking.GetPageText("ChangeAllComponentSeatsText")
        RemoveForComponentText = _viewModelHospitalityBooking.GetPageText("RemoveForComponentText")
        RemoveAllForComponentText = _viewModelHospitalityBooking.GetPageText("RemoveAllForComponentText")
        If AgentProfile.IsAgent Then
            PriceCodeHeaderText = _viewModelHospitalityBooking.GetPageText("PriceCodeHeaderText")
            ApplyComponentDiscountText = _viewModelHospitalityBooking.GetPageText("ApplyComponentDiscountText")
            CantRemoveSeatWarningText = _viewModelHospitalityBooking.GetPageText("CantRemoveSeatWarningText")
            CantChangeTotalPrice = _viewModelHospitalityBooking.GetPageText("CantChangeTotalPrice")
            AddSeatUsingBestAvailableText = _viewModelHospitalityBooking.GetPageText("AddSeatUsingBestAvailableText")
            CantAddMoreSeatsWarningText = _viewModelHospitalityBooking.GetPageText("CantAddMoreSeatsWarningText")

            CantRemoveWholeComponentAvailabilityWarningText = _viewModelHospitalityBooking.GetPageText("CantRemoveWholeComponentAvailabilityWarningText")
            CantRemoveWholeComponentDutToMinQtyWarningText = _viewModelHospitalityBooking.GetPageText("CantRemoveWholeComponentDueToMinQtyWarningText")
            CantRemoveNonSeatedComponentDueToMinQtyWarningText = _viewModelHospitalityBooking.GetPageText("CantRemoveNonSeatedComponentDueToMinQtyWarningText")
            ltlBoxOfficePackageIncludedExtras.Text = _viewModelHospitalityBooking.GetPageText("PackageIncludedExtras").Replace("<<PackageDescription>>", PackageDescription)
            ltlDiscountLabel.Text = _viewModelHospitalityBooking.GetPageText("ltlDiscountLabel")
            lbtnRemoveDiscount.Text = _viewModelHospitalityBooking.GetPageText("RemoveDiscountButtonText")
            ltlBoxOfficePackageExtrasTitle.Text = _viewModelHospitalityBooking.GetPageText("packageExtrasTitle")
            lblBoxOfficePackageExtrasDDLLabel.Text = _viewModelHospitalityBooking.GetPageText("packageExtrasDDLLabel")
            lbtnBoxOfficeAddPackageExtras.Text = _viewModelHospitalityBooking.GetPageText("packageExtrasButtonText")
            If _viewingBookingAsOutputOnly Then
                PrintSingleSeatLinkText = _viewModelHospitalityBooking.GetPageText("PrintSingleSeatLinkText")
                PrintBookingLinkText = _viewModelHospitalityBooking.GetPageText("PrintBookingButtonText")
                GenerateDocumentLinkText = _viewModelHospitalityBooking.GetPageText("CreateBookingDocumentLinkText")
                CreatePDFLinkText = _viewModelHospitalityBooking.GetPageText("CreatePDFButtonText")
            End If
        Else
            ltlPWSPackageExtrasTitle.Text = _viewModelHospitalityBooking.GetPageText("packageExtrasTitle")
            lblPWSPackageExtrasDDLLabel.Text = _viewModelHospitalityBooking.GetPageText("packageExtrasDDLLabel")
            lbtnPWSAddPackageExtras.Text = _viewModelHospitalityBooking.GetPageText("packageExtrasButtonText")
        End If

        ltlPWSPackageIncludedExtras.Text = _viewModelHospitalityBooking.GetPageText("PackageIncludedExtras").Replace("<<PackageDescription>>", PackageDescription)
        ltlBookingTotalCostLabel.Text = _viewModelHospitalityBooking.GetPageText("BookingTotalCostLabelText")
        ltlBookingTotalCostIncVATLabel.Text = _viewModelHospitalityBooking.GetPageText("ltlBookingTotalCostIncVATLabelText")
        ltlTOBookingTotalCostIncVATLabel.Text = _viewModelHospitalityBooking.GetPageText("ltlBookingTotalCostIncVATLabelText")
        ltlBookingVATLabel.Text = _viewModelHospitalityBooking.GetPageText("BookingVATLabelText")
        lbtnCancel.Text = _viewModelHospitalityBooking.GetPageText("CancelButtonText")
        lbtnSaveAsEnquiry.Text = _viewModelHospitalityBooking.GetPageText("SaveAsEnquiryButtonText")
        lbtnBackToBookingEnquiry.Text = _viewModelHospitalityBooking.GetPageText("BackToBookingEnquiryButtonText")
        lbtnContinue.Text = _viewModelHospitalityBooking.GetPageText("ContinueButtonText")
        lbtnUpdate.Text = _viewModelHospitalityBooking.GetPageText("UpdateButtonText")
        lbtnUpdateForSoldBooking.Text = _viewModelHospitalityBooking.GetPageText("UpdateButtonText")
        hlkReserveBooking.Text = _viewModelHospitalityBooking.GetPageText("ReserveBookingButtonText")
    End Sub

    ''' <summary>
    ''' Setup the booking type options on the booking page
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setupBookingOptions()

        If (Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBooking.GetPageAttribute("ShowLeadSource")) OrElse
            Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBooking.GetPageAttribute("ShowBookingType"))) Then
            plhBookingOptions.Visible = True
            'Lead source option
            If (Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBooking.GetPageAttribute("ShowLeadSource"))) Then
                plhLeadSource.Visible = True
                ddlLeadSource.Enabled = (Not _viewingBookingAsOutputOnly)
                rfvLeadSource.Enabled = Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBooking.GetPageAttribute("LeadSourceRequired"))
                rfvLeadSource.ErrorMessage = _viewModelHospitalityBooking.GetPageText("LeadSourceRequiredErrorText")
                lblLeadSource.Text = _viewModelHospitalityBooking.GetPageText("leadSourceLabel")
                ddlLeadSource.Items.Clear()
                For Each leadSource As LeadSourceDetails In _viewModelHospitalityDetails.LeadSourceDetails
                    Dim leadSourceItem As New ListItem
                    leadSourceItem.Text = _viewModelHospitalityBooking.GetPageText("leadSourceDisplayFormat")
                    leadSourceItem.Text = leadSourceItem.Text.Replace("<<LeadSourceDescription>>", leadSource.LeadSourceDescription)
                    leadSourceItem.Text = leadSourceItem.Text.Replace("<<CampaignName>>", leadSource.CampaignName)
                    leadSourceItem.Value = leadSource.LeadSourceID
                    If _viewModelHospitalityBooking.PackageDetailsList(0).LeadSourceID IsNot Nothing AndAlso _viewModelHospitalityBooking.PackageDetailsList(0).LeadSourceID.Length > 0 Then
                        If leadSource.LeadSourceID = _viewModelHospitalityBooking.PackageDetailsList(0).LeadSourceID Then
                            leadSourceItem.Selected = True
                        End If
                    End If
                    ddlLeadSource.Items.Add(leadSourceItem)
                Next
                If ddlLeadSource.Items.Count > 1 Then
                    Dim leadSourceItem As New ListItem
                    leadSourceItem.Value = "-1"
                    leadSourceItem.Text = _viewModelHospitalityBooking.GetPageText("PleaseSelectLeadSource")
                    ddlLeadSource.Items.Insert(0, leadSourceItem)
                Else
                    plhLeadSource.Visible = False
                    rfvLeadSource.Enabled = False
                End If
            Else
                plhLeadSource.Visible = False
                rfvLeadSource.Enabled = False
            End If

            'Booking type (Business or personal) 
            If (Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBooking.GetPageAttribute("ShowBookingType"))) Then
                plhBookingType.Visible = True
                rfvBookingOptions.Enabled = Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultFalse(_viewModelHospitalityBooking.GetPageAttribute("ShowBookingType"))
                rfvBookingOptions.ErrorMessage = _viewModelHospitalityBooking.GetPageText("BookingTypeRequiredErrorText")
                rblMarkOrderFor.Items(0).Text = _viewModelHospitalityBooking.GetPageText("PersonalLabel")
                rblMarkOrderFor.Items(1).Text = _viewModelHospitalityBooking.GetPageText("BusinessLabel")
                lblBookingType.Text = _viewModelHospitalityBooking.GetPageText("BookingTypeLabel")
                If _viewModelHospitalityBooking.PackageDetailsList(0).MarkOrderFor = GlobalConstants.MARK_FOR_BUISINESS Then
                    rblMarkOrderFor.Items(1).Selected = True
                End If
                If _viewModelHospitalityBooking.PackageDetailsList(0).MarkOrderFor = GlobalConstants.MARK_FOR_PERSONAL Then
                    rblMarkOrderFor.Items(0).Selected = True
                End If
                If _viewingBookingFromEnquiryPage AndAlso _viewingBookingAsOutputOnly Then
                    rblMarkOrderFor.Enabled = False
                End If
            Else
                plhBookingType.Visible = False
                rfvBookingOptions.Enabled = False
            End If
        Else
            plhBookingOptions.Visible = False
        End If

        If lbtnContinue.Visible Then
            pnlHospitalityBooking.DefaultButton = lbtnContinue.ID
        ElseIf lbtnUpdateForSoldBooking.Visible Then
            pnlHospitalityBooking.DefaultButton = lbtnUpdateForSoldBooking.ID
        Else
            pnlHospitalityBooking.DefaultButton = lbtnBackToBookingEnquiry.ID
        End If
    End Sub

    ''' <summary>
    ''' Setup the view from area reveal options on the booking page
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setupViewFromArea()
        Dim viewImgURL As String = viewFromAreaImgURL(_viewModelHospitalityDetails.PackageCode)
        If String.IsNullOrEmpty(viewImgURL) Then
            plhViewFromArea1.Visible = False
            plhViewFromArea2.Visible = False
        Else
            plhViewFromArea1.Visible = True
            plhViewFromArea2.Visible = True
            BookingHeadingDynamicCSSClass = "medium-9"
            hlkView1.Text = _viewModelHospitalityBooking.GetPageText("ViewHeaderLabel")
            hlkView2.Text = _viewModelHospitalityBooking.GetPageText("ViewHeaderLabel")
            imgViewArea.ImageUrl = viewImgURL
            hlkView1.Attributes.Add("data-open", "view-area-1")
            hlkView2.Attributes.Add("data-open", "view-area-1")
        End If
    End Sub

    ''' <summary>
    ''' Set the total prices. Values including and excluding VAT as well as the VAT value itself
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setTotalCost()
        ltlBookingTotalCostExVAT.Text = _viewModelHospitalityBooking.PackageDetailsList(0).FormattedPriceBeforeVAT
        ltlBookingVATValue.Text = _viewModelHospitalityBooking.PackageDetailsList(0).FormattedVATPrice
        If _viewModelHospitalityBooking.PackageDetailsList(0).PackageComponentLevelDiscountValue > 0 Then
            ltlDiscountValue.Text = "-" + _viewModelHospitalityBooking.PackageDetailsList(0).FormattedPackageComponentLevelDiscountValue
        Else
            ltlDiscountValue.Text = "-" + _viewModelHospitalityBooking.PackageDetailsList(0).FormattedPackageDiscountedByValue
        End If
        txtBookingTotalCostIncVAT.Text = _viewModelHospitalityBooking.PackageDetailsList(0).PriceIncludingVAT.ToString("F")
        If _componentDiscountsApplied Or _viewingBookingAsOutputOnly Then
            txtBookingTotalCostIncVAT.Enabled = False
            spanBookingTotalCostIncVAT.Attributes.Add("data-tooltip", String.Empty)
            spanBookingTotalCostIncVAT.Attributes.Add("aria-haspopup", "true")
            spanBookingTotalCostIncVAT.Attributes.Add("data-disable-hover", "false")
            If Not _viewingBookingAsOutputOnly Then
                spanBookingTotalCostIncVAT.Attributes.Add("title", _viewModelHospitalityBooking.GetPageText("CantChangeTotalPrice"))
            End If

        End If
        lbtnRemoveDiscount.Visible = Not _viewingBookingAsOutputOnly AndAlso (_componentDiscountsApplied OrElse _viewModelHospitalityBooking.PackageDetailsList(0).PackageDiscountedByValue > 0)
        hdfMaxBookingTotalCostIncVAT.Value = _viewModelHospitalityBooking.PackageDetailsList(0).PriceIncludingVAT + _viewModelHospitalityBooking.PackageDetailsList(0).PackageDiscountedByValue

        plhDiscountTotal.Visible = (AgentProfile.IsAgent)
        If plhDiscountTotal.Visible Then
            CurrencySymbol = Server.HtmlEncode(_viewModelHospitalityBooking.PackageDetailsList(0).CurrencySymbol)
            plhLeftCurrencySymbol.Visible = False
            plhRightCurrencySymbol.Visible = False
            Dim currencyPosition As String = _viewModelHospitalityBooking.GetPageAttribute("PackageDiscountCurrencyPosition").ToUpper()
            plhLeftCurrencySymbol.Visible = (currencyPosition = "LEFT")
            plhRightCurrencySymbol.Visible = (currencyPosition = "RIGHT")
            If _componentDiscountsApplied Then
                rngBookingTotalCostIncVAT.Enabled = False
                rfvBookingTotalCostIncVAT.Enabled = False
            Else
                rngBookingTotalCostIncVAT.Enabled = True
                rfvBookingTotalCostIncVAT.Enabled = True
                'rngBookingTotalCostIncVAT.MaximumValue = CInt(_viewModelHospitalityBooking.PackageDetailsList(0).PriceBeforeVAT + _viewModelHospitalityBooking.PackageDetailsList(0).VATPrice)
                rngBookingTotalCostIncVAT.MaximumValue = (_viewModelHospitalityBooking.PackageDetailsList(0).PriceBeforeVAT + _viewModelHospitalityBooking.PackageDetailsList(0).VATPrice).ToString("F")
                rngBookingTotalCostIncVAT.MinimumValue = 0
                rngBookingTotalCostIncVAT.Type = ValidationDataType.Currency
                rngBookingTotalCostIncVAT.ErrorMessage = _viewModelHospitalityBooking.GetPageText("DiscountTotalInvalidText").Replace("<<MaxDiscountValue>>", rngBookingTotalCostIncVAT.MaximumValue)
                rfvBookingTotalCostIncVAT.ErrorMessage = _viewModelHospitalityBooking.GetPageText("DiscountTotalRequiredText")
            End If
            plhTotalOnly.Visible = False
        Else
            rngBookingTotalCostIncVAT.Enabled = False
            rfvBookingTotalCostIncVAT.Enabled = False
            plhTotalOnly.Visible = True
            ltlTOBookingTotalCostIncVATValue.Text = _viewModelHospitalityBooking.PackageDetailsList(0).FormattedPriceIncludingVAT
        End If
    End Sub

    ''' <summary>
    ''' Set the hiddenfield values on the booking page
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setHiddenFields()
        If Not _viewingBookingCATMode Then
            hdfAlertifyTitle.Value = _viewModelHospitalityBooking.GetPageText("AlertifyCancelBookingTitle")
            hdfAlertifyMessage.Value = _viewModelHospitalityBooking.GetPageText("AlertifyCancelBookingMessage")
            hdfAlertifyOK.Value = _viewModelHospitalityBooking.GetPageText("AlertifyCancelBookingOKButtonText")
            hdfAlertifyCancel.Value = _viewModelHospitalityBooking.GetPageText("AlertifyCancelBookingCancelButtonText")
        Else
            hdfAlertifyTitle.Value = _viewModelHospitalityBooking.GetPageText("AlertifyCancelAmendBookingTitle")
            hdfAlertifyMessage.Value = _viewModelHospitalityBooking.GetPageText("AlertifyCancelAmendBookingMessage")
            hdfAlertifyOK.Value = _viewModelHospitalityBooking.GetPageText("AlertifyCancelAmendBookingOKButtonText")
            hdfAlertifyCancel.Value = _viewModelHospitalityBooking.GetPageText("AlertifyCancelAmendBookingCancelButtonText")
        End If
        If _viewingBookingAsOutputOnly Then
            hdfAlertifyTitleForPrintSingleSeat.Value = _viewModelHospitalityBooking.GetPageText("AlertifyTitleForPrintSingleSeat")
            hdfAlertifyMessageForPrintSingleSeat.Value = _viewModelHospitalityBooking.GetPageText("AlertifyMessageForPrintSingleSeat")
            hdfAlertifyOKForPrintSingleSeat.Value = _viewModelHospitalityBooking.GetPageText("AlertifyOKForPrintSingleSeat")
            hdfAlertifyCancelForPrintSingleSeat.Value = _viewModelHospitalityBooking.GetPageText("AlertifyCancelForPrintSingleSeat")

            hdfAlertifyTitleForPrintBooking.Value = _viewModelHospitalityBooking.GetPageText("AlertifyTitleForPrintBooking")
            hdfAlertifyMessageForPrintBooking.Value = _viewModelHospitalityBooking.GetPageText("AlertifyMessageForPrintBooking")
            hdfAlertifyOKForPrintBooking.Value = _viewModelHospitalityBooking.GetPageText("AlertifyOKForPrintBooking")
            hdfAlertifyCancelForPrintBooking.Value = _viewModelHospitalityBooking.GetPageText("AlertifyCancelForPrintBooking")
            hdfNumberOfTicketsToPrint.Value = _viewModelHospitalityBooking.NumberOfTicketsToPrint
            hdfCallIdToBePrinted.Value = CallId

            hdfAlertifyTitleForSingleBookingDocument.Value = _viewModelHospitalityBooking.GetPageText("AlertifyTitleForCreateBookingDocument")
            hdfAlertifyMessageForSingleBookingDocument.Value = _viewModelHospitalityBooking.GetPageText("AlertifyMessageForCreateBookingDocument")
            hdfAlertifyOKForSingleBookingDocument.Value = _viewModelHospitalityBooking.GetPageText("AlertifyOKForCreateBookingDocument")
            hdfAlertifyCancelForSingleBookingDocument.Value = _viewModelHospitalityBooking.GetPageText("AlertifyCancelForCreateBookingDocument")
            hdfCallIdForDocumentProduction.Value = CallId
        End If

        hdfCancelBooking.Value = String.Empty
        hdfPrintSingleSeat.Value = String.Empty
        hdfSeatToBePrinted.Value = String.Empty
        hdfComponentId.Value = String.Empty
        hdfFormattedSeatToBePrinted.Value = String.Empty

        If Session("MergedWordDocumentDownloadPath") IsNot Nothing Then
            hdfMergedDocumentPath.Value = Session("MergedWordDocumentDownloadPath")
            Session("MergedWordDocumentDownloadPath") = Nothing
        End If

    End Sub

    ''' <summary>
    ''' Set question and answer fields
    ''' </summary>
    Private Sub setQuestionAndAnswer()
        If _viewModelHospitalityBooking.ActivityQuestionAnswerList.Count > 0 Then
            hdfUpdatePackageMessageText.Value = _viewModelHospitalityBooking.GetPageText("UpdatePackageMessageText")
            If _viewingBookingCATMode Then
                hdfUpdatePackageMessageText.Value = _viewModelHospitalityBooking.GetPageText("UpdatePackageCATMessageText")
            End If
            plhQAndA.Visible = True
            rptProductQuestions.DataSource = _viewModelHospitalityBooking.ActivityQuestionAnswerList
            rptProductQuestions.DataBind()
            If _viewModelHospitalityBooking.PackageDetailsList(0).ExpandAccordion = True Then
                QAndAExpandDynamicClass = "is-active"
            Else
                QAndAExpandDynamicClass = String.Empty
            End If
            QuestionAndAnswerHeaderText = _viewModelHospitalityBooking.GetPageText("QuestionAndAnswerHeaderText")
            AdditionalQuestionText = _viewModelHospitalityBooking.GetPageText("AdditionalQuestionText")
        Else
            plhQAndA.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Setup the extra components dropdown list
    ''' </summary>
    Private Sub setupExtraComponents()
        If AgentProfile.IsAgent Then
            plhPWSExtraComponents.Visible = False
            plhBoxOfficeExtraComponents.Visible = False
            rfvBoxOfficePackageExtras.ErrorMessage = _viewModelHospitalityBooking.GetPageText("PackageExtrasRequiredErrorText")
            If _viewModelHospitalityBooking.ExtraComponentDetailsList.Count > 0 Then
                ddlBoxOfficePackageExtras.Items.Clear()
                For Each extraComponent As ComponentDetails In _viewModelHospitalityBooking.ExtraComponentDetailsList
                    Dim extraComponentItem As New ListItem
                    If extraComponent.AvailabilityFlag Then
                        extraComponentItem.Text = extraComponent.ComponentDescription
                    Else
                        extraComponentItem.Text = _viewModelHospitalityBooking.GetPageText("UnavailableExtraItemsText").Replace("<<ComponentDescription>>", extraComponent.ComponentDescription)
                        extraComponentItem.Attributes.Add("disabled", "true")
                    End If
                    extraComponentItem.Value = extraComponent.ComponentID
                    ddlBoxOfficePackageExtras.Items.Add(extraComponentItem)
                Next

                If ddlBoxOfficePackageExtras.Items.Count >= 1 Then
                    Dim extraComponentItem As New ListItem
                    extraComponentItem.Value = "-1"
                    extraComponentItem.Text = _viewModelHospitalityBooking.GetPageText("PackageExtrasDDLPrompt")
                    ddlBoxOfficePackageExtras.Items.Insert(0, extraComponentItem)
                    plhBoxOfficeExtraComponents.Visible = True
                End If
            End If
        Else
            plhBoxOfficeExtraComponents.Visible = False
            plhPWSExtraComponents.Visible = False
            rfvPWSPackageExras.ErrorMessage = _viewModelHospitalityBooking.GetPageText("PackageExtrasRequiredErrorText")
            If _viewModelHospitalityBooking.ExtraComponentDetailsList.Count > 0 Then
                ddlPWSPackageExtras.Items.Clear()
                For Each extraComponent As ComponentDetails In _viewModelHospitalityBooking.ExtraComponentDetailsList
                    If extraComponent.PWSFlag Then
                        Dim extraComponentItem As New ListItem
                        If extraComponent.AvailabilityFlag Then
                            extraComponentItem.Text = extraComponent.ComponentDescription
                        Else
                            extraComponentItem.Text = _viewModelHospitalityBooking.GetPageText("UnavailableExtraItemsText").Replace("<<ComponentDescription>>", extraComponent.ComponentDescription)
                            extraComponentItem.Attributes.Add("disabled", "true")
                        End If
                        extraComponentItem.Value = extraComponent.ComponentID
                        ddlPWSPackageExtras.Items.Add(extraComponentItem)
                    End If
                Next

                If ddlPWSPackageExtras.Items.Count >= 1 Then
                    Dim extraComponentItem As New ListItem
                    extraComponentItem.Value = "-1"
                    extraComponentItem.Text = _viewModelHospitalityBooking.GetPageText("PackageExtrasDDLPrompt")
                    ddlPWSPackageExtras.Items.Insert(0, extraComponentItem)
                    plhPWSExtraComponents.Visible = True
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Add extra component to package
    ''' </summary>
    ''' <param name="componentId">Selected extra component</param>
    ''' <remarks></remarks>
    Private Sub addPackageExtras(ByVal componentId As Long)
        ''pass only extra component
        _inputModelHospitalityBooking.AmendComponents = New List(Of AmendComponent)
        Dim extraComponent As New AmendComponent
        extraComponent.ComponentId = componentId
        extraComponent.Quantity = 1
        _inputModelHospitalityBooking.AmendComponents.Add(extraComponent)
        _inputModelHospitalityBooking.Mode = OperationMode.Extra
        setInputmodelForUpdate()
        _viewModelHospitalityBooking = _bookingBuilder.AddExtraNonSeatedComponent(_inputModelHospitalityBooking)
        If _viewModelHospitalityBooking.Error.HasError Then
            ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
        Else
            Response.Redirect(Request.Url.AbsoluteUri)
        End If
    End Sub

    ''' <summary>
    ''' Request single seat print, handle errors and redirect
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub performPrint()
        If Not _viewModelHospitalityBooking.Error.HasError Then
            If Not String.IsNullOrEmpty(hdfPrintSingleSeat.Value) AndAlso hdfPrintSingleSeat.Value = True Then
                _inputModelHospitalityBooking.BoxOfficeUser = AgentProfile.Name
                _inputModelHospitalityBooking.SeatToBePrinted = hdfSeatToBePrinted.Value
                _inputModelHospitalityBooking.ProductCodeToBePrinted = ProductCode
                _inputModelHospitalityBooking.CallIdToBePrinted = CallId
                _inputModelHospitalityBooking.ComponentID = hdfComponentId.Value
                _viewModelHospitalityBooking = _bookingBuilder.PrintHospitalityBooking(_inputModelHospitalityBooking)
                hdfPrintSingleSeat.Value = False
                If Not _viewModelHospitalityBooking.PrintRequestSucceeded Then
                    ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
                Else
                    ltlSuccessMessage.Text = _viewModelHospitalityBooking.GetPageText("PrintSeatSubmitSuccessText").Replace("<<seat>>", hdfFormattedSeatToBePrinted.Value)
                End If

            End If
            If Not String.IsNullOrEmpty(hdfPrintBooking.Value) AndAlso hdfPrintBooking.Value = True Then
                _inputModelHospitalityBooking.BoxOfficeUser = AgentProfile.Name
                _inputModelHospitalityBooking.CallIdToBePrinted = CallId
                _viewModelHospitalityBooking = _bookingBuilder.PrintHospitalityBooking(_inputModelHospitalityBooking)
                hdfPrintBooking.Value = False
                If Not _viewModelHospitalityBooking.PrintRequestSucceeded Then
                    ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
                Else
                    ltlSuccessMessage.Text = _viewModelHospitalityBooking.GetPageText("PrintBookingSubmitSuccessText").Replace("<<NumberOfTicketsToPrint>>", _viewModelHospitalityBooking.NumberOfTicketsToPrint)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Request document creation for booking, handle errors and redirect
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub generateDocumentForBooking()
        If Not _viewModelHospitalityBooking.Error.HasError Then
            If Not String.IsNullOrEmpty(hdfGenerateBookingDocument.Value) AndAlso hdfGenerateBookingDocument.Value = True Then
                _inputModelHospitalityBooking.BoxOfficeUser = AgentProfile.Name
                _inputModelHospitalityBooking.CallIdForDocumentProduction = CallId
                _viewModelHospitalityBooking = _bookingBuilder.GenerateDocumentForBooking(_inputModelHospitalityBooking)
                hdfGenerateBookingDocument.Value = False
                If Not _viewModelHospitalityBooking.GenerateDocumentRequestSuccess Then
                    ltlBookingErrors.Text = _viewModelHospitalityBooking.Error.ErrorMessage
                Else
                    ltlSuccessMessage.Text = _viewModelHospitalityBooking.GetPageText("CreateBookingDocumentSuccessText").Replace("<<CallId>>", _viewModelHospitalityBooking.CallId)
                    Session("MergedWordDocumentDownloadPath") = _defaults.HospMergedDocumentPathRelative & _viewModelHospitalityBooking.MergedWordDocumentPath
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Setup for hospitality booking reservation
    ''' </summary>
    Private Sub ReservationSetup()
        'If AgentProfile.IsAgent AndAlso AgentProfile.AgentPermissions.CanReserveHospitalityBookings AndAlso _viewModelHospitalityDetails.ProductType = GlobalConstants.HOMEPRODUCTTYPE AndAlso (_viewModelHospitalityBooking.PackageDetailsList(0).BookingStatus = GlobalConstants.IN_PROGRESS_BOOKING_STATUS Or _viewModelHospitalityBooking.PackageDetailsList(0).BookingStatus = GlobalConstants.ENQUIRY_BOOKING_STATUS) Then
        If AgentProfile.IsAgent AndAlso _viewModelHospitalityDetails.ProductType = GlobalConstants.HOMEPRODUCTTYPE AndAlso (_viewModelHospitalityBooking.PackageDetailsList(0).BookingStatus = GlobalConstants.IN_PROGRESS_BOOKING_STATUS Or _viewModelHospitalityBooking.PackageDetailsList(0).BookingStatus = GlobalConstants.ENQUIRY_BOOKING_STATUS) Then
            uscReserveTickets.Visible = True
            hlkReserveBooking.Visible = True
        Else
            uscReserveTickets.Visible = False
            hlkReserveBooking.Visible = False
        End If
    End Sub
#End Region

End Class