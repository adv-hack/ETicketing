Imports System.Data
Imports System.Globalization
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities


Partial Class PagesLogin_Profile_OnAccountAdjustment
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _ucr As New UserControlResource
    Private _wfr As New WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _product As New TalentProduct
    Private _settings As New DESettings
    Private _standAndAreaDescriptions As New DataTable
    Private errMsg As Talent.Common.TalentErrorMessages
    Private _totalBalance As Decimal
    Private _refundableBalance As Decimal
    Private _mode As String

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "OnAccountAdjustment.aspx"
        End With

        errMsg = New Talent.Common.TalentErrorMessages(_languageCode,
                                                       TalentCache.GetBusinessUnitGroup,
                                                       TalentCache.GetPartner(Profile),
                                                       ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString)

        _mode = Talent.eCommerce.Utilities.CheckForDBNull_String(Request.QueryString("Mode"))

        ' This is used in 2 modes Refund to card or on-account adjustment
        ' If public websales Only refund to card is available - must not allow adjustment option
        If _mode <> "RefundToCard" Then
            If Not AgentProfile.IsAgent Then
                _mode = "RefundToCard"
            End If
        End If
        If _mode = "RefundToCard" Then
            plhDDLSavedCard.Visible = True
        End If

    End Sub
    Protected Sub Page_PreRenderComplete(sender As Object, e As EventArgs) Handles Me.PreRenderComplete
        ' If no saved cards can't refund so give a message
        Dim ddlSavedCards As DropDownList = uscSavedCards.FindControl("ddlSavedCardList")
        If _mode = "RefundToCard" AndAlso ddlSavedCards.Items.Count = 0 Then
            btnOkay.Enabled = False
            errorList.Items.Add(_wfr.Content("ltlYouHaveNoSavedCards", _languageCode, True))
        End If
        plhErrorList.Visible = (errorList.Items.Count > 0)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        SetupPageText()

        If ModuleDefaults.CashbackRewardsActive Then
            rdoSpecificDate.Enabled = True
            txtExpiryDate.Enabled = True
            plhExpiryDetails.Visible = True
            plhConfirmAllowance.Visible = True
        Else
            rdoSpecificDate.Enabled = False
            txtExpiryDate.Enabled = False
            plhExpiryDetails.Visible = False
            plhConfirmAllowance.Visible = False
        End If

        ' If in refund to card mode show reduced options and only allow a negative adjustment 

        If _mode = "RefundToCard" Then
            plhRefundToCardHeading.Visible = True
            plhAdjustmentType.Visible = False
            plhActivationDate.Visible = False
            plhReason.Visible = False
            plhExpiryDetails.Visible = False
            plhConfirmAllowance.Visible = False
            plhExpiryDetails.Visible = False
            plhConfirmAllowance.Visible = False
            plhDDLSavedCard.Visible = True
            plhRefundToCardHeading.Visible = True
            rdbPositive.Checked = False
            rdbNegative.Checked = True
            RetrieveOnAccountBalances(_totalBalance, _refundableBalance)
            TotalOnAccount.Text = FormatCurrency(_totalBalance)
            RefundableOnAccount.Text = FormatCurrency(_refundableBalance)
            If txtAmount.Text = String.Empty Then
                txtAmount.Text = _refundableBalance
            End If

            txtReason.Text = _wfr.Content("ltlRefundedToCard", _languageCode, True)
        Else
            plhRefundToCardHeading.Visible = False
        End If


        btnOkay.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnOkay))
        ' btnOkay.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(btnOkay, Nothing) + ";")

    End Sub

    Protected Sub btnBack_Click(sender As Object, e As EventArgs)

        Response.Redirect("~\PagesLogin\Profile\OnAccount.aspx")

    End Sub

    Protected Sub btnOkay_Click(sender As Object, e As System.EventArgs) Handles btnOkay.Click
        Page.Validate()

        If Page.IsValid Then

            ' If in card refund mode this page need some extra validation
            If _mode = "ManualAdjustment" OrElse isCardRefundValid() Then

                Dim dePayment As New DEPayments()

                If Not Profile.User.Details Is Nothing Then
                    dePayment.CustomerNumber = Profile.User.Details.LoginID
                End If
                dePayment.SessionId = Profile.Basket.Basket_Header_ID
                dePayment.Source = "W"
                dePayment.AgentName = AgentProfile.Name

                Dim amt As String = txtAmount.Text.Trim()

                Dim amount As Decimal = Convert.ToDecimal(amt).ToString("N2")
                amount = amount * 100

                amt = amount.ToString()
                amt = amt.Remove(amt.IndexOf("."))

                dePayment.Amount = amt
                dePayment.Reason = txtReason.Text.Trim()

                If rdbNegative.Checked Then
                    dePayment.AdjustmentType = "N"
                    dePayment.ConfirmCarryOverAllowance = "Y"
                    dePayment.ActivationDate = 0
                    dePayment.ExpiryOption = 1

                    Dim ddlSavedCards As DropDownList = uscSavedCards.FindControl("ddlSavedCardList")
                    If ddlSavedCards.Items.Count > 0 AndAlso Not ddlSavedCards.SelectedValue = -1 Then
                        dePayment.UniqueCardId = ddlSavedCards.SelectedValue
                    End If

                Else
                    dePayment.AdjustmentType = "P"

                    'Set the Activation Date
                    If txtActivationDate.Text.Length > 0 Then
                        Dim dtActivationDate As New Date
                        'This gets the date from the date picker in dd/MM/yyyy format
                        Dim strActivateDate As String = txtActivationDate.Text.Trim()

                        If DateTime.TryParseExact(strActivateDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, dtActivationDate) Or
                            DateTime.TryParseExact(strActivateDate, "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, dtActivationDate) Then
                            dePayment.ActivationDate = TCUtilities.DateToIseriesFormat(dtActivationDate)
                        Else
                            dePayment.ActivationDate = 0
                        End If
                    Else
                        dePayment.ActivationDate = 0
                    End If

                    'Set the ConfirmCarryOverAllowance
                    If chkConfirmAllowance.Checked Then
                        dePayment.ConfirmCarryOverAllowance = "Y"
                    Else
                        dePayment.ConfirmCarryOverAllowance = "N"
                    End If

                    'Set the Expiry Option
                    If rdoNoExpiry.Checked Then
                        dePayment.ExpiryOption = "1"
                    ElseIf rdoRewardPeriodEnd.Checked Then
                        dePayment.ExpiryOption = "2"
                    Else
                        dePayment.ExpiryOption = "3"

                        Dim dtExpiryDate As New Date
                        Dim strExpiryDate As String = txtExpiryDate.Text.Trim()

                        If DateTime.TryParseExact(strExpiryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, dtExpiryDate) Or
                            DateTime.TryParseExact(strExpiryDate, "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, dtExpiryDate) Then
                            dePayment.ExpiryDate = TCUtilities.DateToIseriesFormat(dtExpiryDate)
                        Else
                            dePayment.ExpiryDate = 0
                        End If
                    End If
                    dePayment.UniqueCardId = 0
                End If

                Dim tp As New Talent.Common.TalentPayment
                tp.De = dePayment
                tp.Settings = TEBUtilities.GetSettingsObject()

                Dim err As New Talent.Common.ErrorObj
                err = tp.OnAccountAdjustment()

                If Not err.HasError AndAlso _
                    tp.ResultDataSet.Tables.Count = 2 AndAlso _
                    tp.ResultDataSet.Tables("StatusResults").Rows.Count = 0 Then
                    Response.Redirect("~\PagesLogin\Profile\OnAccount.aspx")
                Else
                    If err.HasError OrElse tp.ResultDataSet Is Nothing OrElse tp.ResultDataSet.Tables.Count = 0 Then
                        errorList.Items.Add(errMsg.GetErrorMessage("XX").ERROR_MESSAGE)
                    ElseIf tp.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                        errorList.Items.Add(errMsg.GetErrorMessage(tp.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode")).ERROR_MESSAGE)

                    End If
                End If
            End If
        End If
    End Sub


    Protected Sub RdbAdjustmentType_OnCheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        Try
            If rdbNegative.Checked Then
                If (_mode = "RefundToCard") Then
                    plhDDLSavedCard.Visible = True
                Else
                    plhDDLSavedCard.Visible = False
                End If
                plhActivationDate.Visible = False
                plhExpiryDetails.Visible = False
                plhConfirmAllowance.Visible = False
            ElseIf rdbPositive.Checked Then
                plhDDLSavedCard.Visible = False
                plhActivationDate.Visible = True
                If ModuleDefaults.CashbackRewardsActive Then
                    plhExpiryDetails.Visible = True
                    plhConfirmAllowance.Visible = True
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub SetRegex(ByVal sender As Object, ByVal e As EventArgs)
        Dim reg As RegularExpressionValidator = CType(sender, RegularExpressionValidator)
        reg.ValidationExpression = _wfr.Attribute("AccountAdjustmentAmountRegularExpression") ' ^\d{0,9}(\.\d{0,2})?$
        reg.ErrorMessage = _wfr.Content("AccountAdjustmentAmountErrorMessage", _languageCode, True)
    End Sub

    Protected Sub SetErrorMessageAmount(ByVal sender As Object, ByVal e As EventArgs)
        Dim req As RequiredFieldValidator = CType(sender, RequiredFieldValidator)
        req.ErrorMessage = _wfr.Content("AccountAdjustmentAmountRequired", _languageCode, True)
    End Sub

    Protected Sub SetErrorMessageReason(ByVal sender As Object, ByVal e As EventArgs)
        Dim req As RequiredFieldValidator = CType(sender, RequiredFieldValidator)
        req.ErrorMessage = _wfr.Content("AccountAdjustmentReasonRequired", _languageCode, True)
    End Sub

    Protected Sub ValidateExpiryDate(source As Object, args As ServerValidateEventArgs)
        If rdoSpecificDate.Checked Then
            If String.IsNullOrWhiteSpace(txtExpiryDate.Text.Trim()) Then
                cvExpiry.ErrorMessage = _wfr.Content("ExpiryDateRequired", _languageCode, True)
                args.IsValid = False
            Else
                Dim dtExpiryDate As Date
                If Not (DateTime.TryParseExact(txtExpiryDate.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, dtExpiryDate) Or
                                DateTime.TryParseExact(txtExpiryDate.Text.Trim(), "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, dtExpiryDate)) Then
                    cvExpiry.ErrorMessage = _wfr.Content("ExpiryDateInvalid", _languageCode, True)
                    args.IsValid = False
                End If
            End If
        End If
    End Sub
    Protected Sub ValidateActivationDate(source As Object, args As ServerValidateEventArgs)
        If Not String.IsNullOrWhiteSpace(txtActivationDate.Text.Trim()) Then
            Dim dtActivationDate As Date
            If Not (DateTime.TryParseExact(txtActivationDate.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, dtActivationDate) Or
                            DateTime.TryParseExact(txtActivationDate.Text.Trim(), "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, dtActivationDate)) Then
                cvActDate.ErrorMessage = _wfr.Content("ActivationDateInvalid", _languageCode, True)
                args.IsValid = False
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub RetrieveOnAccountBalances(ByRef TotalBalance As Decimal, ByRef RefundableBalance As Decimal)
        Const ModuleName As String = "RetrieveOnAccountDetails"
        Dim err As New ErrorObj
        Dim tp As New TalentPayment
        tp.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        tp.Settings.ModuleName = ModuleName
        tp.De.CustomerNumber = Profile.User.Details.LoginID
        tp.De.CashbackMode = "R"
        tp.De.Source = GlobalConstants.SOURCE
        err = tp.RetrieveOnAccountDetails
        If Not err.HasError AndAlso Not tp.ResultDataSet Is Nothing AndAlso tp.ResultDataSet.Tables.Count = 2 AndAlso _
          tp.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then
            TotalBalance = Talent.Common.Utilities.CheckForDBNull_Decimal(tp.ResultDataSet.Tables("dtStatusResults").Rows(0)("TotalBalance"))
            RefundableBalance = Talent.Common.Utilities.CheckForDBNull_Decimal(tp.ResultDataSet.Tables("dtStatusResults").Rows(0)("RefundableBalance"))
        End If

    End Sub
    ' If in refund to card mode need to do extra checks 
    Private Function isCardRefundValid() As Boolean
        Dim isvalid As Boolean = True
        errorList.Items.Clear()
        RetrieveOnAccountBalances(_totalBalance, _refundableBalance)
        If TEBUtilities.CheckForDBNull_Decimal(txtAmount.Text) > _refundableBalance Then
            errorList.Items.Add(_wfr.Content("ltlMaxYouCanRefund", _languageCode, True) + " " + FormatCurrency(_refundableBalance))
            isvalid = False
        End If
        Dim ddlSavedCards As DropDownList = uscSavedCards.FindControl("ddlSavedCardList")
        If ddlSavedCards.SelectedIndex = 0 Then
            errorList.Items.Add(_wfr.Content("NoCardSelected", _languageCode, True))
            isvalid = False
        End If
        If TEBUtilities.CheckForDBNull_Decimal(txtAmount.Text) < 0 Then
            errorList.Items.Add(_wfr.Content("AmountMustBePositive", _languageCode, True))
            isvalid = False
        End If
        ' This should never happen as we have set this to be unchecked and hidden it 
        ' However if checked this would allow people to add money to their on-account balance so make sure 
        If rdbPositive.Checked Then
            errorList.Items.Add(_wfr.Content("AmountMustBePositive", _languageCode, True))
            isvalid = False
        End If
        If isvalid Then
            txtReason.Text = (_wfr.Content("ltlRefundedToCard", _languageCode, True) + " " + ddlSavedCards.SelectedItem.ToString)
        End If
        Return isvalid
    End Function

    Private Sub SetupPageText()

        lblAmount.Text = _wfr.Content("lblAmount", _languageCode, True)
        lblReason.Text = _wfr.Content("lblReason", _languageCode, True)
        lblActivationDate.Text = _wfr.Content("lblActivationDate", _languageCode, True)
        lblConfirmAllowance.Text = _wfr.Content("lblConfirmAllowance", _languageCode, True)
        lblExpiryDate.Text = _wfr.Content("lblExpiryDate", _languageCode, True)
        lblExpiryOr.Text = _wfr.Content("lblExpiryOr", _languageCode, True)
        lblAdjustmentType.Text = _wfr.Content("lblAdjustmentType", _languageCode, True)
        rdbPositive.Text = _wfr.Content("lblPositive", _languageCode, True)
        rdbNegative.Text = _wfr.Content("lblNegative", _languageCode, True)
        rdoNoExpiry.Text = _wfr.Content("lblNoExpiry", _languageCode, True)
        rdoRewardPeriodEnd.Text = _wfr.Content("lblRewardPeriodEnd", _languageCode, True)
        rdoSpecificDate.Text = _wfr.Content("lblSpecificDate", _languageCode, True)
        btnOkay.Text = _wfr.Content("btnOkay", _languageCode, True)
        btnBack.Text = _wfr.Content("btnBack", _languageCode, True)
        lblRefundableOnAccount.Text = _wfr.Content("lblRefundableOnAccount", _languageCode, True)
        lblTotalOnAccount.Text = _wfr.Content("lblTotalOnAccount", _languageCode, True)
        ltlrefundtocard.Text = _wfr.Content("ltlrefundtocard", _languageCode, True)
    End Sub

    Private Function getJSFunctionForConfirmButton(ByRef ConfirmButton As Button) As String
        Dim javascriptFunction As New StringBuilder()
        javascriptFunction.Append("this.disabled = true;")
        javascriptFunction.Append(Me.Page.ClientScript.GetPostBackEventReference(ConfirmButton, Nothing))
        javascriptFunction.Append(";")
        Return javascriptFunction.ToString()
    End Function

#End Region

End Class
