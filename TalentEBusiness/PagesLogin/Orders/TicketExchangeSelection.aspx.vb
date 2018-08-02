Imports TalentBusinessLogic.ModelBuilders
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.BusinessObjects.Definitions
Imports System.Collections.Generic
Imports TalentBusinessLogic.DataTransferObjects
Imports Talent.Common
Imports TEBUtilities = Talent.eCommerce.Utilities
Partial Class PagesLogin_Orders_TicketExchangeSelection
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _ticketExchangeSelectionViewModel As TicketExchangeSelectionViewModel
    Private _confirmViewModel As TicketExchangeViewModel
    Private _product As String = String.Empty

    Private _stage As stage
    Private _resetScreen As Boolean

    Private Enum stage
        None = 0
        SelectTickets = 1
        Review = 2
        Confirm = 3
    End Enum

#End Region

#Region "Protected Page Events"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If String.IsNullOrEmpty(Request.QueryString("Product")) Then
            Response.Redirect("~/Pageslogin/orders/TicketExchangeProducts.aspx")
        End If
        If TalentDefaults.TicketExchangeEnabled Then
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "error-handling.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("error-handling.js", "/Application/Status/"), False)
            ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "ticket-exchange-products.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("ticket-exchange-products.js", "/Module/TicketExchange/"), False)
            'ScriptManager.RegisterStartupScript(Me.Page, GetType(Page), "ticket-exchange-selection.js", Talent.eCommerce.Utilities.FormatJavaScriptFileReference("ticket-exchange-selection.js", "/Module/TicketExchange/"), False)
        Else
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
        _product = Request.QueryString("product")
        If Session("TESelectTicketView") IsNot Nothing Then
            _ticketExchangeSelectionViewModel = (CType(Session("TESelectTicketView"), TicketExchangeSelectionViewModel))
        End If
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        errorProcessing()
        setStage()
        processController()
        progressBar()
        createView()
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        plhErrorList.Visible = (blErrorList.Items.Count > 0)
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Set the input model to retrieve the list of ticket exchange items
    ''' </summary>
    ''' <remarks></remarks>
    Private Function getSelectionInputModel() As TicketExchangeSelectionInputModel
        Dim inputModel As New TicketExchangeSelectionInputModel
        inputModel.CustomerNumber = Profile.UserName
        inputModel.ProductCode = _product
        Return inputModel
    End Function

    ''' <summary>
    ''' Set the confirmation input model to confirm the changes used within the controller
    ''' </summary>
    ''' <remarks></remarks>
    Private Function getConfirmationInputModel() As TicketExchangeInputModel
        Dim inputModel As New TicketExchangeInputModel
        inputModel.ListingCustomerNumber = Profile.UserName
        inputModel.Tickets = New List(Of TicketExchangeItem)

        For Each seat In _ticketExchangeSelectionViewModel.TicketExchangeSeatList.FindAll(Function(x) (x.HasChanged = True))
            inputModel.Tickets.Add(seat)
        Next
        Return inputModel
    End Function

    Private Function setTESummaryHeaderMask(ByVal TicketExchangeMask As String, ByVal _tep As TicketExchangeSelectionViewModel) As String
        Dim TicketExchangeString As String = TicketExchangeMask
        If (blErrorList.Items.Count = 0) Then
            If TicketExchangeMask.Contains("<<") Then
                TicketExchangeString = TicketExchangeString.Replace("<<ProductCode>>", _tep.ProductCode)
                TicketExchangeString = TicketExchangeString.Replace("<<ProductDescription>>", _tep.ProductDescription)
                TicketExchangeString = TicketExchangeString.Replace("<<ProductDate>>", _tep.ProductDate.Substring(0, 10))
            End If
        End If
        Return TicketExchangeString
    End Function

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Error Processing
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub errorProcessing()
        blErrorList.Items.Clear()

        ' Validate that some items have been altered before progressing to Review stage.
        If Not String.IsNullOrWhiteSpace(Request.Params(btnNext.UniqueID)) Then
            TicketExchangeSelection.ViewModel = _ticketExchangeSelectionViewModel
            TicketExchangeSelection.getAlteredTicketExchangeItems(_ticketExchangeSelectionViewModel.TicketExchangeSeatList)
            If _ticketExchangeSelectionViewModel.TicketExchangeSeatList.FindAll(Function(x) (x.HasChanged = True)).Count = 0 Then
                blErrorList.Items.Add(_ticketExchangeSelectionViewModel.GetPageText("NoActionSelectedErrorMessage"))
            End If
        End If

    End Sub

    ''' <summary>
    ''' Determine what stage we are in
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setStage()
        _resetScreen = True
        _stage = stage.SelectTickets
        'Handles query string values for different stages to allow for html includes to work via query string

        btnPrevious.Attributes.Add("onclick", "updateStageOnQueryString('" & _stage + 1 & "', '" & _stage & "');")
        btnReset.Attributes.Add("onclick", "updateStageOnQueryString('" & _stage + 1 & "', '" & _stage & "');")
        btnNext.Attributes.Add("onclick", "updateStageOnQueryString('" & _stage & "', '" & _stage + 1 & "');")
        If IsPostBack Then
            If (blErrorList.Items.Count > 0) Then
                ' Do not change stage for errors.
                _resetScreen = False
                _stage = stage.SelectTickets
            ElseIf Not String.IsNullOrWhiteSpace(Request.Params(btnNext.UniqueID)) Then
                _stage = stage.Review

            ElseIf Not String.IsNullOrWhiteSpace(Request.Params(btnCancel.UniqueID)) Then
                Response.Redirect("~/Pageslogin/orders/TicketExchangeProducts.aspx")
            ElseIf Not String.IsNullOrWhiteSpace(Request.Params(btnPrevious.UniqueID)) Then
                If Session("LastStage") = stage.SelectTickets Then
                    Response.Redirect("~/Pageslogin/orders/TicketExchangeProducts.aspx")
                End If
                If Session("LastStage") = stage.Review Then
                    _resetScreen = False
                    _stage = stage.SelectTickets
                End If
            ElseIf Not String.IsNullOrWhiteSpace(Request.Params(btnReset.UniqueID)) Then
                _stage = stage.SelectTickets
            ElseIf Not String.IsNullOrWhiteSpace(Request.Params(btnConfirm.UniqueID)) Then
                _stage = stage.Confirm
            ElseIf Not String.IsNullOrWhiteSpace(Request.Params(btnFinished.UniqueID)) Then
                Response.Redirect("~/Pageslogin/orders/TicketExchangeProducts.aspx")
            End If
        End If

        Session("LastStage") = _stage
    End Sub

    ''' <summary>
    ''' Process the controller with the given input model and setup the display
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub processController()
        Dim TicketExchangeSelectionBuilder As New TicketExchangeSelectionBuilder()
        If _stage = stage.SelectTickets And _resetScreen Then
            _ticketExchangeSelectionViewModel = TicketExchangeSelectionBuilder.TicketingExchangeSelectionGetSeats(getSelectionInputModel())
            Session.Add("TESelectTicketView", _ticketExchangeSelectionViewModel)
        End If
        If _ticketExchangeSelectionViewModel.TicketExchangeSeatList Is Nothing Then
            Response.Redirect("~/Pageslogin/orders/TicketExchangeProducts.aspx")
        End If
        If _stage = stage.Confirm Then
            _confirmViewModel = TicketExchangeSelectionBuilder.TicketingExchangeSelectionConfirm(getConfirmationInputModel())
        End If

    End Sub

    ''' <summary>
    ''' Set the HTML Include for the progress bar.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub progressBar()
        If ModuleDefaults.ShowTicketExchangeProgressBar Then
            If _stage = stage.SelectTickets Then
                ProgressBar1.HTMLInclude = "TEProgressBarSelectTickets.html"
            ElseIf _stage = stage.Review Then
                ProgressBar1.HTMLInclude = "TEProgressBarReview.html"
            ElseIf _stage = stage.Confirm Then
                ProgressBar1.HTMLInclude = "TEProgressBarConfirm.html"
            End If
        End If
    End Sub

    ''' <summary>
    ''' Determine what view to create based on the stage we are at
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub createView()

        If _ticketExchangeSelectionViewModel.Error IsNot Nothing Then
            If _ticketExchangeSelectionViewModel.Error.HasError Then

                'if there are no tickets available then don't show placeholders
                If _ticketExchangeSelectionViewModel.TicketExchangeSeatList Is Nothing Then
                    TicketExchangeSelection.Visible = False
                    blErrorList.Items.Add(_ticketExchangeSelectionViewModel.GetPageText("NoTicketErrorMessage"))
                End If
                blErrorList.Items.Add(_ticketExchangeSelectionViewModel.Error.ErrorMessage)
            End If
        End If

        If _confirmViewModel IsNot Nothing AndAlso _confirmViewModel.Error IsNot Nothing Then
            If _confirmViewModel.Error.HasError Then
                blErrorList.Items.Add(_confirmViewModel.Error.ErrorMessage)
            End If
        End If

        ltlTicketingExchangeSelection.Text = setTESummaryHeaderMask(_ticketExchangeSelectionViewModel.GetPageText("ltlTicketExchangeSelectionHeader"), _ticketExchangeSelectionViewModel)

        btnCancel.Visible = False
        btnReset.Visible = False
        btnPrevious.Visible = False
        btnNext.Visible = False
        btnConfirm.Visible = False
        btnFinished.Visible = False
        plhSuccessMessage.Visible = False

        btnCancel.Text = _ticketExchangeSelectionViewModel.GetPageText("btnCancel")
        btnReset.Text = _ticketExchangeSelectionViewModel.GetPageText("btnreset")
        btnNext.Text = _ticketExchangeSelectionViewModel.GetPageText("btnnext")
        btnPrevious.Text = _ticketExchangeSelectionViewModel.GetPageText("btnPrevious")
        btnConfirm.Text = _ticketExchangeSelectionViewModel.GetPageText("btnConfirm")
        btnFinished.Text = _ticketExchangeSelectionViewModel.GetPageText("btnFinished")

        TicketExchangeSelection.Visible = False
        TicketExchangePlaceSeatsOnTE.Visible = False
        TicketExchangeTakeSeatsOffTE.Visible = False
        TicketExchangePriceChange.Visible = False

        TicketExchangeSelection.TEStage = _stage
        TicketExchangePlaceSeatsOnTE.TEStage = _stage
        TicketExchangeTakeSeatsOffTE.TEStage = _stage
        TicketExchangePriceChange.TEStage = _stage

        TicketExchangeSelection.ViewModel = _ticketExchangeSelectionViewModel
        TicketExchangePlaceSeatsOnTE.ViewModel = _ticketExchangeSelectionViewModel
        TicketExchangeTakeSeatsOffTE.ViewModel = _ticketExchangeSelectionViewModel
        TicketExchangePriceChange.ViewModel = _ticketExchangeSelectionViewModel

        Select Case _stage

            Case stage.SelectTickets
                TicketExchangeSelection.SeatList = _ticketExchangeSelectionViewModel.TicketExchangeSeatList
                TicketExchangeSelection.Visible = True
                btnCancel.Visible = True
                btnPrevious.Visible = True
                btnNext.Visible = True
                btnReset.Visible = True

            Case stage.Review
                setReviewListVisibility()
                btnCancel.Visible = True
                btnConfirm.Visible = True
                btnPrevious.Visible = True
                btnReset.Visible = True

            Case stage.Confirm
                setReviewListVisibility()
                btnFinished.Visible = True

                If Not _confirmViewModel.Error.HasError Then
                    plhSuccessMessage.Visible = True
                    Dim confirmationMessage As String
                    confirmationMessage = _confirmViewModel.GetPageText("ConfirmErrorText")
                    confirmationMessage = confirmationMessage.Replace("<<TicketExchangeReference>>", _confirmViewModel.TicketExchangeReference.TrimStart(GlobalConstants.LEADING_ZEROS))
                    ltlSuccessMessage.Text = confirmationMessage
                    sendConfirmationEmail()
                End If
        End Select
    End Sub

    Private Sub setReviewListVisibility()
        'Retrieve all altered seats from the user control and set the item list for each of the 3 repearters.
        TicketExchangeSelection.getAlteredTicketExchangeItems(_ticketExchangeSelectionViewModel.TicketExchangeSeatList)
        Dim filteredSeatList As List(Of TicketExchangeItem)

        filteredSeatList = _ticketExchangeSelectionViewModel.TicketExchangeSeatList.FindAll(Function(x) (x.Status = GlobalConstants.TicketExchangeItemStatus.PriceChanged AndAlso x.HasChanged = True))
        If filteredSeatList.Count > 0 Then
            TicketExchangePriceChange.SeatList = filteredSeatList
            TicketExchangePriceChange.Visible = True
        End If

        filteredSeatList = _ticketExchangeSelectionViewModel.TicketExchangeSeatList.FindAll(Function(x) (x.Status = GlobalConstants.TicketExchangeItemStatus.PlacingOnSale AndAlso x.HasChanged = True))
        If filteredSeatList.Count > 0 Then
            TicketExchangePlaceSeatsOnTE.SeatList = filteredSeatList
            TicketExchangePlaceSeatsOnTE.Visible = True
        End If

        filteredSeatList = _ticketExchangeSelectionViewModel.TicketExchangeSeatList.FindAll(Function(x) (x.Status = GlobalConstants.TicketExchangeItemStatus.TakingOffSale AndAlso x.HasChanged = True))
        If filteredSeatList.Count > 0 Then
            TicketExchangeTakeSeatsOffTE.SeatList = filteredSeatList
            TicketExchangeTakeSeatsOffTE.Visible = True
        End If
    End Sub

    Private Sub sendConfirmationEmail()

        Dim partner As String = Profile.PartnerInfo.Details.Partner
        Dim talEmail As New TalentEmail
        Dim TicketExchangeReference As String = _confirmViewModel.TicketExchangeReference.TrimStart(GlobalConstants.LEADING_ZEROS).Trim
        Dim xmlDoc As String = talEmail.CreateTicketExchangeConfirmationXmlDocument(ModuleDefaults.OrdersFromEmail, _
                                        Profile.User.Details.Email, _
                                        ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim, _
                                        Talent.eCommerce.Utilities.GetSMTPPortNumber, _
                                        partner, _
                                        Profile.User.Details.LoginID, TicketExchangeReference, GlobalConstants.SOURCE)


        'Create the email request in the offline processing table
        TDataObjects.AppVariableSettings.TblOfflineProcessing.Insert(TalentCache.GetBusinessUnit(), "*ALL", "Pending", 0, "", _
                                                "EmailMonitor", "TicketExchangeConfirmation", xmlDoc, "")
    End Sub

#End Region

End Class