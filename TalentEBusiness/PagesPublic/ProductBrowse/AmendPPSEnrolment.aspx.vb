Imports System.Data
Imports Talent.Common
Imports Talent.eCommerce

Partial Class PagesPublic_ProductBrowse_AmendPPSEnrolment
    Inherits TalentBase01

#Region "Public Properties"

    Public LoadingText As String = String.Empty

#End Region

#Region "Class Level Fields"

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _wfrPage As WebFormResource = Nothing
    Private _errMsg As TalentErrorMessages = Nothing
    Private _talentPPS As TalentPPS = Nothing
    Private _noSchemesSelected As Boolean = True

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'Check if agent has access on Amend PPS enrolment menu item
        If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessPPS) Or Not AgentProfile.IsAgent Then
            Dim myTicketingMenu As New TalentTicketingMenu
            _wfrPage = New WebFormResource
            _talentPPS = New TalentPPS
            myTicketingMenu.LoadTicketingProducts(TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), Talent.eCommerce.Utilities.GetCurrentLanguage)
            PaymentDetailsDirectDebitMandate.PageCode = UCase(Talent.eCommerce.Utilities.GetCurrentPageName.Trim)
            If myTicketingMenu.TicketingProductIsActive("AMENDPPS") Then
                With _wfrPage
                    .BusinessUnit = TalentCache.GetBusinessUnit()
                    .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
                    .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
                End With
                _errMsg = New TalentErrorMessages(_languageCode,
                                                    TalentCache.GetBusinessUnitGroup,
                                                    TalentCache.GetPartner(Profile),
                                                    ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
            Else
                Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
            End If
        Else
            Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
            Session("UnavailableReturnPage") = String.Empty
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LoadingText = _wfrPage.Content("LoadingText", _languageCode, True)
        If Not String.IsNullOrEmpty(Session("success")) Then
            If Session("success").ToString.Equals("true") Then
                plhSuccessMessage.Visible = True
                ltlSuccessMessage.Text = _wfrPage.Content("EnrolmentSuccessMessageText", _languageCode, True)
                Session("success") = String.Empty
                plhAmendPPSList.Visible = False
                plhPayment.Visible = False
            End If
        End If
        If Not Page.IsPostBack Then
            Try
                'Report any TG errors
                '----------------------------------------------
                If Session("TicketingGatewayError") IsNot Nothing Then
                    ErrorList.Items.Add(_errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, CStr(Session("TicketingGatewayError"))).ERROR_MESSAGE)
                    If Session("TalentErrorCode") = Session("TicketingGatewayError") Then
                        Session("TalentErrorCode") = Nothing
                    End If
                    Session("TicketingGatewayError") = Nothing
                End If
                If Not Session("TalentErrorCode") Is Nothing Then
                    Dim myError As String = CStr(Session("TalentErrorCode"))
                    ErrorList.Items.Add(_errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, Talent.eCommerce.Utilities.GetCurrentPageName, myError).ERROR_MESSAGE)
                    Session("TalentErrorCode") = Nothing
                End If
                '----------------------------------------------
            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Dim productDetailRepeater As Repeater = CType(ProductDetail1.FindControl("ProductRepeater"), Repeater)
        Dim visibleTicketingPPSItems As Integer = 0
        Dim schemesAvailableToEnrol As Integer = 0
        For Each productItem As RepeaterItem In productDetailRepeater.Items
            Dim ticketingPPS As UserControls_TicketingPPS = CType(productItem.FindControl("TicketingPPS1"), UserControls_TicketingPPS)
            For Each item As ListItem In ticketingPPS.Errors.Items
                If Not ErrorList.Items.Contains(item) Then
                    ErrorList.Items.Add(item)
                End If
            Next
            If ticketingPPS.Visible Then
                visibleTicketingPPSItems = +1
                For Each item As RepeaterItem In ticketingPPS.AmendPPSDetails.Items
                    Dim chkAmendPPSEnrol As CheckBox = CType(item.FindControl("chkAmendPPSEnrol"), CheckBox)
                    If chkAmendPPSEnrol.Enabled Then schemesAvailableToEnrol += 1
                Next
            Else
                productItem.Visible = False
            End If
        Next
        If ErrorList.Items.Count > 0 Then
            plhErrorList.Visible = True
        Else
            plhErrorList.Visible = False
        End If
        plhPayment.Visible = (schemesAvailableToEnrol > 0)
        plhRegiesteredPostOption.Visible = (Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(_wfrPage.Attribute("ShowRegPostOption")) AndAlso visibleTicketingPPSItems > 0 AndAlso plhPayment.Visible)
    End Sub

    Protected Sub ddlPaymentOptions_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlPaymentOptions.SelectedIndexChanged
        determinePaymentOptions()
    End Sub

    ''' <summary>
    ''' This function will be hit when the no-javascript button next to the drop down payment options is pressed
    ''' </summary>
    ''' <param name="sender">button</param>
    ''' <param name="e">click event</param>
    ''' <remarks></remarks>
    Protected Sub btnSubmitPaymentSelection_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmitPaymentSelection.Click
        determinePaymentOptions()
    End Sub

    ''' <summary>
    ''' Determine payment type and choose to perform a PPS amendment call
    ''' </summary>
    ''' <param name="sender">button</param>
    ''' <param name="e">click event</param>
    ''' <remarks></remarks>
    Protected Sub btnSubmitPaymentOptions_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmitPaymentOptions.Click
        ErrorList.Items.Clear()
        hidPosition.Value = "FALSE"
        Dim err As New ErrorObj
        Select Case ddlPaymentOptions.SelectedValue
            Case Is = "1"
                If PaymentDetailsCreditCardForm.ValidateUserInput Then
                    err = processPayment("CC")
                    ProcessValidForm(err)
                Else
                    PaymentDetailsCreditCardForm.CardTypeDDL.Focus()
                    For Each item As ListItem In PaymentDetailsCreditCardForm.ErrorMessages.Items
                        ErrorList.Items.Add(item)
                    Next
                End If
            Case Is = "2"
                If PaymentDetailsDirectDebitForm.ValidateUserInput Then
                    Dim productCodeForDD As String = ""
                    Dim productDetailRepeater As Repeater = CType(ProductDetail1.FindControl("ProductRepeater"), Repeater)
                    For Each productItem As RepeaterItem In productDetailRepeater.Items
                        If productCodeForDD = "" Then
                            Dim ticketingPPS As UserControls_TicketingPPS = CType(productItem.FindControl("TicketingPPS1"), UserControls_TicketingPPS)
                            For Each rptAmendPPSItem As RepeaterItem In ticketingPPS.AmendPPSDetails.Items
                                Try
                                    Dim chkAmendPPSEnrol As CheckBox = CType(rptAmendPPSItem.FindControl("chkAmendPPSEnrol"), CheckBox)
                                    If chkAmendPPSEnrol.Enabled And chkAmendPPSEnrol.Checked Then
                                        Dim hdfProductCode As HiddenField = CType(ticketingPPS.FindControl("hfProductCode"), HiddenField)
                                        productCodeForDD = hdfProductCode.Value.Trim
                                        Exit For
                                    End If
                                Catch ex As Exception
                                End Try
                            Next
                        Else
                            Exit For
                        End If
                    Next
                    If productCodeForDD <> "" Then
                        Checkout.StoreDDDetails(PaymentDetailsDirectDebitForm.AccountNameBox.Text, _
                                    PaymentDetailsDirectDebitForm.SortCode1Box.Text & PaymentDetailsDirectDebitForm.SortCode2Box.Text & PaymentDetailsDirectDebitForm.SortCode3Box.Text, _
                                    PaymentDetailsDirectDebitForm.AccountNumberBox.Text, _
                                    PaymentDetailsDirectDebitForm.BankName, _
                                    PaymentDetailsDirectDebitForm.PaymentDayDropDownList.SelectedValue, productCodeForDD, UCase(Talent.eCommerce.Utilities.GetCurrentPageName.Trim))
                        btnSubmitDirectDebitMandate.Visible = True
                        btnSubmitPaymentOptions.Visible = False
                        PaymentDetailsDirectDebitMandate.Visible = True
                        PaymentDetailsDirectDebitMandate.PageCode = UCase(Talent.eCommerce.Utilities.GetCurrentPageName.Trim)
                        PaymentDetailsDirectDebitForm.Visible = False
                        PaymentDetailsDirectDebitMandate.SetTextandValues()
                    Else
                        ErrorList.Items.Add(_wfrPage.Content("NoPPSSelected", _languageCode, True))
                    End If
                    If ErrorList.Items.Count > 0 Then
                        plhErrorList.Visible = True
                        hidPosition.Value = "TRUE"
                    End If
                Else
                    ErrorList.Items.Add(PaymentDetailsDirectDebitForm.ErrorMessage)
                End If
        End Select

    End Sub

    Protected Sub btnSubmitDirectDebitMandate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmitDirectDebitMandate.Click
        Dim err As New ErrorObj
        err = processPayment("DD")
        ProcessValidForm(err)
    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' This sub routine is called outside of this page and determines whether or not to show any payment options. No payment options will be
    ''' displayed if there are no enrolments available to choose. This determines what text to load from the database.
    ''' </summary>
    ''' <param name="showOptions">boolean</param>
    ''' <remarks></remarks>
    Public Sub ShowPaymentOptions(ByVal showOptions As Boolean)
        updPaymentOptions.Visible = showOptions
        If showOptions And Not Page.IsPostBack Then
            ddlPaymentOptions.Items.Clear()
            AddDDLPaymentOptions(_wfrPage.Content("PleaseSelectOptionText", _languageCode, True), "0")
            AddDDLPaymentOptions(_wfrPage.Content("CreditCardOptionText", _languageCode, True), "1")
            AddDDLPaymentOptions(_wfrPage.Content("DirectDebitOptionText", _languageCode, True), "2")
            btnSubmitPaymentOptions.Text = _wfrPage.Content("SubmitPaymentButtonText", _languageCode, True)
            btnSubmitPaymentSelection.Text = _wfrPage.Content("SubmitPaymentOptionButtonText", _languageCode, True)
            ltlRegistedPost.Text = _wfrPage.Content("RegisteredPostText", _languageCode, True)
            ltlPaymentOptions.Text = _wfrPage.Content("PaymentOptionsLabelText", _languageCode, True)
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Add payment options to the drop down list
    ''' </summary>
    ''' <param name="optionText">The option text</param>
    ''' <param name="optionValue">The option value</param>
    ''' <remarks></remarks>
    Private Sub AddDDLPaymentOptions(ByVal optionText As String, ByVal optionValue As String)
        If Not String.IsNullOrEmpty(optionText) Then
            Dim lstitem As New ListItem
            lstitem.Text = optionText
            lstitem.Value = optionValue
            ddlPaymentOptions.Items.Add(lstitem)
            lstitem = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Handle the page layout based on the payment option selected
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub determinePaymentOptions()
        Select Case ddlPaymentOptions.SelectedValue
            Case "0"
                plhCCPaymentOptions.Visible = False
                plhDDPaymentOptions.Visible = False
                plhSumbitButton.Visible = False
            Case "1"
                plhCCPaymentOptions.Visible = True
                plhDDPaymentOptions.Visible = False
                plhSumbitButton.Visible = True
            Case "2"
                plhCCPaymentOptions.Visible = False
                plhDDPaymentOptions.Visible = True
                plhSumbitButton.Visible = True
        End Select
    End Sub

    ''' <summary>
    ''' Send the confirmation E-Mail
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub sendConfirmationEmail()
        Dim talEmail As New TalentEmail
        Dim productCode As String = String.Empty
        If Not String.IsNullOrEmpty(Request.QueryString("product")) Then
            productCode = Request.QueryString("product").Trim
        End If
        Dim xmlDoc As String = talEmail.CreateAmendPPSXmlDocument(ModuleDefaults.OrdersFromEmail, _
                                        Profile.User.Details.Email, _
                                        ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim, _
                                        Talent.eCommerce.Utilities.GetSMTPPortNumber, _
                                        _wfrPage.PartnerCode, _
                                        Profile.User.Details.LoginID, _
                                        productCode)

        'Create the email request in the offline processing table
        TDataObjects.AppVariableSettings.TblOfflineProcessing.Insert(TalentCache.GetBusinessUnit(), "*ALL", "Pending", 0, "", _
                                                "EmailMonitor", "AmendPPS", xmlDoc, "")
    End Sub

    ''' <summary>
    ''' Process the form if it's valid
    ''' </summary>
    ''' <param name="err">The error object</param>
    ''' <remarks></remarks>
    Private Sub ProcessValidForm(ByVal err As ErrorObj)
        hidPosition.Value = "FALSE"
        If err.HasError Then
            ErrorList.Items.Clear()
            Try
                If Not _talentPPS.ResultDataSet Is Nothing Then
                    If _talentPPS.ResultDataSet.Tables("StatusResults").Rows.Count > 0 Then
                        Dim statusResultsTable As DataTable = _talentPPS.ResultDataSet.Tables("StatusResults")
                        Dim failedRequests As Integer = CInt(statusResultsTable.Rows(0)("ErrorCount").ToString)
                        Dim successRequests As Integer = CInt(statusResultsTable.Rows(0)("SuccessCount").ToString)
                        If failedRequests > 0 Then
                            Dim failedRequestsTable As DataTable = _talentPPS.ResultDataSet.Tables("FailedRequests")
                            If failedRequestsTable.Rows.Count > 0 Then
                                Dim myError As String = failedRequestsTable.Rows(0)("ReturnCode").ToString
                                Dim errorMessage As String = _errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
                                                        Talent.eCommerce.Utilities.GetCurrentPageName, _
                                                        myError).ERROR_MESSAGE
                                errorMessage = errorMessage.Replace("<<FailedRequests>>", failedRequests.ToString)
                                ErrorList.Items.Add(errorMessage)
                            End If
                        Else
                            ErrorList.Items.Add(_wfrPage.Content("UnknownPaymentFailureErrorText", _languageCode, True))
                        End If
                    End If
                ElseIf _noSchemesSelected Then
                    ErrorList.Items.Add(_wfrPage.Content("NoSchemeSelected", _languageCode, True))
                Else
                    'failed to generate session id error
                    ErrorList.Items.Add(_wfrPage.Content("UnknownPaymentFailureErrorText", _languageCode, True))
                End If
            Catch
                ErrorList.Items.Add(_wfrPage.Content("UnknownPaymentFailureErrorText", _languageCode, True))
            End Try
            If ErrorList.Items.Count > 0 Then
                plhErrorList.Visible = True
                hidPosition.Value = "TRUE"
            End If
        Else
            Session("success") = "true"
            sendConfirmationEmail()
            Response.Redirect(Request.Url.AbsoluteUri)
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Perform the PPS Amendment based on the Payment type provided.
    ''' </summary>
    ''' <param name="paymentType">"CC" or "DD" string</param>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Private Function processPayment(ByVal paymentType As String) As ErrorObj
        Dim err As New ErrorObj
        Dim currentLoggedInCustomer As String = Profile.User.Details.LoginID
        Dim sessionId As String = String.Empty
        Dim deSettings As New DESettings
        Dim dePayments As New DEPayments
        Dim dePPS As New DEPPS
        Dim dePPSEnrolment As New DEPPSEnrolment
        Dim dePPSEnrolmentsList As New Generic.List(Of DEPPSEnrolment)
        Dim dePPSEnrolmentSchemeList As New Generic.List(Of DEPPSEnrolmentScheme)
        Dim talentBasket As New Talent.Common.TalentBasket

        With deSettings
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            .OriginatingSourceCode = "W"
            .Cacheing = False
        End With

        With dePayments
            If paymentType.Equals("CC") Then
                .CardNumber = PaymentDetailsCreditCardForm.CardNumberBox.Text.Trim
                .CardType = PaymentDetailsCreditCardForm.CardTypeDDL.SelectedValue
                .CV2Number = PaymentDetailsCreditCardForm.SecurityNumberBox.Text.Trim
                .StartDate = formatCardDate(PaymentDetailsCreditCardForm.StartMonthDDL.SelectedValue, PaymentDetailsCreditCardForm.StartYearDDL.SelectedValue)
                .ExpiryDate = formatCardDate(PaymentDetailsCreditCardForm.ExpiryMonthDDL.SelectedValue, PaymentDetailsCreditCardForm.ExpiryYearDDL.SelectedValue)
                .CardHolderName = PaymentDetailsCreditCardForm.CardHolderNameBox.Text.Trim
                .IssueNumber = PaymentDetailsCreditCardForm.IssueNumberBox.Text.Trim
                If ModuleDefaults.PaymentGatewayExternal = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
                    .PaymentType = GlobalConstants.PAYMENTTYPE_VANGUARD
                Else
                    .PaymentType = paymentType
                End If
            ElseIf paymentType.Equals("DD") Then
                .AccountName = PaymentDetailsDirectDebitForm.AccountNameBox.Text.Trim
                .AccountNumber = PaymentDetailsDirectDebitForm.AccountNumberBox.Text.Trim
                .SortCode = PaymentDetailsDirectDebitForm.SortCode1Box.Text & PaymentDetailsDirectDebitForm.SortCode2Box.Text & PaymentDetailsDirectDebitForm.SortCode3Box.Text
                .PaymentType = paymentType
            End If
        End With

        With talentBasket
            .Settings = deSettings
            .De.CustomerNo = currentLoggedInCustomer
            err = .GenerateTicketingBasketID
            sessionId = .De.SessionId
        End With

        If Not err.HasError Then
            Dim productDetailRepeater As Repeater = CType(ProductDetail1.FindControl("ProductRepeater"), Repeater)
            For Each productItem As RepeaterItem In productDetailRepeater.Items
                Dim ticketingPPS As UserControls_TicketingPPS = CType(productItem.FindControl("TicketingPPS1"), UserControls_TicketingPPS)

                For Each rptAmendPPSItem As RepeaterItem In ticketingPPS.AmendPPSDetails.Items
                    Try
                        Dim chkAmendPPSEnrol As CheckBox = CType(rptAmendPPSItem.FindControl("chkAmendPPSEnrol"), CheckBox)
                        If chkAmendPPSEnrol.Enabled And chkAmendPPSEnrol.Checked Then
                            Dim dePPSEnrolmentScheme As New DEPPSEnrolmentScheme
                            Dim hdfAmendPPSSeatDetails As HiddenField = CType(rptAmendPPSItem.FindControl("hdfAmendPPSSeatDetails"), HiddenField)
                            Dim hdfAmendPPSCustomerNumber As HiddenField = CType(rptAmendPPSItem.FindControl("hdfAmendPPSCustomerNumber"), HiddenField)
                            Dim hdfProductCode As HiddenField = CType(ticketingPPS.FindControl("hfProductCode"), HiddenField)
                            With dePPSEnrolmentScheme
                                .SeasonTicket = hdfAmendPPSSeatDetails.Value.Substring(0, 3).Trim & "/"
                                .SeasonTicket = .SeasonTicket & hdfAmendPPSSeatDetails.Value.Substring(3, 4).Trim & "/"
                                .SeasonTicket = .SeasonTicket & hdfAmendPPSSeatDetails.Value.Substring(7, 4).Trim & "/"
                                .SeasonTicket = .SeasonTicket & hdfAmendPPSSeatDetails.Value.Substring(11, 4).Trim
                                If Not String.IsNullOrEmpty(hdfAmendPPSSeatDetails.Value.Substring(15, 1).Trim) Then
                                    .SeasonTicket = .SeasonTicket & "/" & hdfAmendPPSSeatDetails.Value.Substring(15, 1)
                                End If
                                .ProductCode = hdfProductCode.Value
                                .CustomerNumber = hdfAmendPPSCustomerNumber.Value
                                If chkRegisteredPost.Checked Then
                                    .RegisteredPost = "Y"
                                Else
                                    .RegisteredPost = "N"
                                End If
                            End With
                            dePPSEnrolmentSchemeList.Add(dePPSEnrolmentScheme)
                            dePPSEnrolmentScheme = Nothing
                            _noSchemesSelected = False
                        End If
                    Catch ex As Exception
                    End Try
                Next
            Next

            If Not _noSchemesSelected Then
                With dePPSEnrolment
                    .CustomerNumber = currentLoggedInCustomer
                    .EnrolmentSchemes = dePPSEnrolmentSchemeList
                    .PaymentDetails = dePayments
                End With

                dePPSEnrolmentsList.Add(dePPSEnrolment)
                dePPS.Enrolments = dePPSEnrolmentsList
                dePPS.SessionId = sessionId
                _talentPPS.Settings = deSettings
                _talentPPS.DEPPS = dePPS
                err = _talentPPS.AddPPSRequest()
                If _talentPPS.ResultDataSet.Tables("FailedRequests").Rows.Count > 0 Then
                    err.HasError = True
                End If
            Else
                err.HasError = True
            End If
        End If
        Return err
    End Function

    ''' <summary>
    ''' Formats the month/year options to 4 digit string to use when calling TALENT.
    ''' </summary>
    ''' <param name="month">1 or 2 digit string</param>
    ''' <param name="year">4 digit string</param>
    ''' <returns>4 digit formatted date</returns>
    ''' <remarks></remarks>
    Private Function formatCardDate(ByVal month As String, ByVal year As String) As String
        Dim cardDate As String = String.Empty
        Try
            If String.IsNullOrEmpty(month) Or String.IsNullOrEmpty(year) Then
                cardDate = String.Empty
            Else
                If month.Length = 1 Then month = "0" & month
                If month.Trim.Equals(GlobalConstants.CARD_DDL_INITIAL_VALUE.Trim()) Then
                    month = "00"
                End If
                If year.Trim.Equals(GlobalConstants.CARD_DDL_INITIAL_VALUE.Trim()) Then
                    year = "00"
                End If
                cardDate = month & year.Substring(2, 2)
            End If
        Catch ex As Exception
            cardDate = String.Empty
        End Try
        Return cardDate
    End Function

#End Region

End Class