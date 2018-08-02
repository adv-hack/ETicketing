Imports System.Data
Imports Talent.Common
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesPublic_ProductBrowse_AmendPPSPayments
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private wfrPage As New WebFormResource
    Private customerNumber As String = String.Empty
    Private seatDetails As String = String.Empty
    Private productCode As String = String.Empty
    Private showPaymentDetails As Boolean = True
    Private talentPPS As New TalentPPS
    Private Const CCPAYMENTTYPE As String = "CC"
    Private Const DDPAYMENTTYPE As String = "DD"
    Private _isExternalGateway As Boolean = False

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsRedirectFromExternalGateway() Then
            If customerNumber.Length > 0 And seatDetails.Length > 0 And productCode.Length > 0 And showPaymentDetails Then
                loadPaymentDetails()
            End If
            If showPaymentDetails Then
                plhNoPaymentDetails.Visible = False
                plhShowPaymentDetails.Visible = True
                If plhCCForm.Visible Then
                    btnUpdatePayment.Text = wfrPage.Content("UpdateButtonText", _languageCode, True)
                    hplCancelPayment.Text = wfrPage.Content("CancelButtonText", _languageCode, True)
                    If Request.UrlReferrer IsNot Nothing Then
                        hplCancelPayment.NavigateUrl = Request.UrlReferrer.ToString
                    Else
                        hplCancelPayment.NavigateUrl = TEBUtilities.GetSiteHomePage()
                    End If
                End If
            Else
                plhNoPaymentDetails.Visible = True
                plhShowPaymentDetails.Visible = False
                ltlNoPaymentDetails.Text = wfrPage.Content("NoPaymentDetailsFound", _languageCode, True)
            End If
        End If
    End Sub

    Protected Sub btnUpdatePayment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdatePayment.Click
        If PaymentDetailsCreditCardForm.ValidateUserInput Then
            ProcessPPSPayUpdate()
        Else
            For Each item As ListItem In PaymentDetailsCreditCardForm.ErrorMessages.Items
                plhErrorMessage.Visible = True
                ltlErrorMessage.Text = item.Text
            Next
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Retrieve the current PPS payment details from TALENT
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub loadPaymentDetails()
        Dim dePPS As New DEPPS
        Dim deSettings As New DESettings
        Dim err As New ErrorObj

        With deSettings
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            .OriginatingSourceCode = "W"
        End With
        dePPS.CustomerNumber = customerNumber
        dePPS.ProductCode = productCode
        dePPS.SeatDetails = seatDetails
        dePPS.UpdateMode = False
        dePPS.RetrieveMode = True
        talentPPS.DEPPS = dePPS
        talentPPS.Settings = deSettings
        err = talentPPS.AddPPSRequest

        If Not err.HasError Then
            If talentPPS.ResultDataSet.Tables.Count = 2 Then
                If talentPPS.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    showPaymentDetails = False
                Else
                    If talentPPS.ResultDataSet.Tables(1).Rows.Count = 1 Then
                        Dim paymentType As String = talentPPS.ResultDataSet.Tables(1).Rows(0)("PaymentType").ToString
                        ltlPaymentDetailsValue.Text = talentPPS.ResultDataSet.Tables(1).Rows(0)("PaymentDetails").ToString
                        If paymentType.Equals(DDPAYMENTTYPE) Then
                            ltlPaymentDetailsLabel.Text = wfrPage.Content("PaymentTypeDDMessage", _languageCode, True)
                            plhDirectDebitMessage.Visible = True
                            ltlDirectDebitMessage.Text = wfrPage.Content("CannotUpdateDDMessage", _languageCode, True)
                            plhCCForm.Visible = False
                        ElseIf paymentType.Equals(CCPAYMENTTYPE) Then
                            plhDirectDebitMessage.Visible = False
                            ltlPaymentDetailsLabel.Text = wfrPage.Content("PaymentTypeCCMessage", _languageCode, True)
                            If ModuleDefaults.PaymentGatewayType = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
                                plhCCFormExternal.Visible = True
                                plhCCForm.Visible = False
                            Else
                                plhCCFormExternal.Visible = False
                                plhCCForm.Visible = True
                            End If

                        Else
                            showPaymentDetails = False
                        End If
                    Else
                        showPaymentDetails = False
                    End If
                End If
            Else
                showPaymentDetails = False
            End If
        Else
            showPaymentDetails = False
        End If
    End Sub

    ''' <summary>
    ''' Send the confirmation E-Mail
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub sendConfirmationEmail()
        Dim talEmail As New TalentEmail
        Dim mDefaults As New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = mDefaults.GetDefaults
        Dim xmlDoc As String = talEmail.CreateAmendPPSPaymentsXmlDocument(def.OrdersFromEmail, _
                                        Profile.User.Details.Email, _
                                        ConfigurationManager.AppSettings("EmailSMTP").ToString.Trim, _
                                        Talent.eCommerce.Utilities.GetSMTPPortNumber, _
                                        wfrPage.PartnerCode, _
                                        Profile.User.Details.LoginID, _
                                        productCode, seatDetails)

        'Create the email request in the offline processing table
        TDataObjects.AppVariableSettings.TblOfflineProcessing.Insert(TalentCache.GetBusinessUnit(), "*ALL", "Pending", 0, "", _
                                                "EmailMonitor", "AmendPPSPaymentUpdate", xmlDoc, "")
    End Sub

#End Region

#Region "Private Functions"

    Private Function IsRedirectFromExternalGateway() As Boolean
        Dim isItRedirect As Boolean = False
        Try
            customerNumber = Request.QueryString("customer").ToString
            seatDetails = Request.QueryString("seat").ToString
            productCode = Request.QueryString("product").ToString
            If Not Page.IsPostBack Then
                If Session("DEPPSFromExternal") IsNot Nothing Then
                    Dim dePPS As DEPPS = Session("DEPPSFromExternal")
                    customerNumber = dePPS.CustomerNumber
                    seatDetails = dePPS.SeatDetails
                    productCode = dePPS.ProductCode
                    isItRedirect = True
                    Session("DEPPSFromExternal") = Nothing
                    ProcessPPSPayUpdate()
                Else
                    If ModuleDefaults.PaymentGatewayType = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
                        Dim dePPS As New DEPPS
                        dePPS.CustomerNumber = customerNumber
                        dePPS.SeatDetails = seatDetails
                        dePPS.ProductCode = productCode
                        Session("DEPPSFromExternal") = dePPS
                    End If
                End If
            End If
        Catch ex As Exception
            showPaymentDetails = False
        End Try
        Return isItRedirect
    End Function

    Private Function GetPaymentEntity() As DEPayments
        Dim dePayments As New DEPayments
        If Session("DEPaymentsFromExternal") IsNot Nothing Then
            dePayments = Session("DEPaymentsFromExternal")
            Session("DEPaymentsFromExternal") = Nothing
        Else
            With dePayments
                .CardNumber = PaymentDetailsCreditCardForm.CardNumberBox.Text.Trim
                .CardType = PaymentDetailsCreditCardForm.CardTypeDDL.SelectedValue
                .CV2Number = PaymentDetailsCreditCardForm.SecurityNumberBox.Text.Trim
                .StartDate = TEBUtilities.FormatCardDate(PaymentDetailsCreditCardForm.StartMonthDDL.SelectedValue, PaymentDetailsCreditCardForm.StartYearDDL.SelectedValue)
                .ExpiryDate = TEBUtilities.FormatCardDate(PaymentDetailsCreditCardForm.ExpiryMonthDDL.SelectedValue, PaymentDetailsCreditCardForm.ExpiryYearDDL.SelectedValue)
                .CardHolderName = PaymentDetailsCreditCardForm.CardHolderNameBox.Text.Trim
                .IssueNumber = PaymentDetailsCreditCardForm.IssueNumberBox.Text.Trim
                .PaymentType = CCPAYMENTTYPE
            End With
        End If

        Return dePayments
    End Function

    ''' <summary>
    ''' Perform the PPS Amendment based on the Payment type provided.
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Private Function processPayment() As ErrorObj
        Dim err As New ErrorObj
        Dim deSettings As New DESettings
        Dim dePPS As New DEPPS
        Dim dePPSEnrolmentsList As New Generic.List(Of DEPPSEnrolment)
        Dim dePPSEnrolment As New DEPPSEnrolment

        With deSettings
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            .OriginatingSourceCode = "W"
            .Cacheing = False
        End With

        dePPSEnrolment.PaymentDetails = GetPaymentEntity()
        dePPSEnrolmentsList.Add(dePPSEnrolment)
        dePPS.Enrolments = dePPSEnrolmentsList
        dePPS.CustomerNumber = customerNumber
        dePPS.SeatDetails = seatDetails
        dePPS.ProductCode = productCode
        dePPS.UpdateMode = True
        dePPS.RetrieveMode = False
        talentPPS.Settings = deSettings
        talentPPS.DEPPS = dePPS
        talentPPS.ResultDataSet = Nothing
        err = talentPPS.AddPPSRequest()
        If talentPPS.ResultDataSet.Tables("StatusResults").Rows.Count > 0 Then
            err.HasError = True
        End If
        Return err
    End Function

    Private Sub ProcessPPSPayUpdate()
        Dim err As New ErrorObj
        Dim errorMessage As String = String.Empty
        err = processPayment()
        plhErrorMessage.Visible = False
        plhSuccessMessage.Visible = False
        If err.HasError Then
            Try
                If Not talentPPS.ResultDataSet Is Nothing Then
                    If talentPPS.ResultDataSet.Tables("StatusResults").Rows.Count > 0 Then
                        Dim statusResultsTable As DataTable = talentPPS.ResultDataSet.Tables("StatusResults")
                        Dim failedRequests As Integer = CInt(statusResultsTable.Rows(0)("ErrorCount").ToString)
                        If failedRequests > 0 Then
                            Dim failedRequestsTable As DataTable = talentPPS.ResultDataSet.Tables("FailedRequests")
                            If failedRequestsTable.Rows.Count > 0 Then
                                Dim myError As String = failedRequestsTable.Rows(0)("ReturnCode").ToString
                                Dim errMsg As New TalentErrorMessages(_languageCode, wfrPage.BusinessUnit, wfrPage.PartnerCode, wfrPage.FrontEndConnectionString)
                                errorMessage = errMsg.GetErrorMessage(myError).ERROR_MESSAGE
                            End If
                        Else
                            errorMessage = wfrPage.Content("PaymentUpdateFailedMessage", _languageCode, True)
                        End If
                    End If
                Else
                    errorMessage = wfrPage.Content("PaymentUpdateFailedMessage", _languageCode, True)
                End If
            Catch ex As Exception
                errorMessage = wfrPage.Content("PaymentUpdateFailedMessage", _languageCode, True)
            End Try
            If errorMessage.Length > 0 Then
                plhErrorMessage.Visible = True
                ltlErrorMessage.Text = errorMessage
            End If
        Else
            plhShowPaymentDetails.Visible = False
            plhSuccessMessage.Visible = True
            ltlSuccessMessage.Text = wfrPage.Content("PaymentUpdateSuccessMessage", _languageCode, True)
            sendConfirmationEmail()
        End If
    End Sub

#End Region

End Class
