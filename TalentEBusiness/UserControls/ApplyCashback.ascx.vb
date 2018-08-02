Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities

Partial Class UserControls_ApplyCashback
    Inherits ControlBase

#Region "Class Level Fields"

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _errorMessage As String = String.Empty
    Private _successMessage As String = String.Empty
    Private _cashbackJustCleared As Boolean = False
    Private _cashbackChanged As Boolean = False
    Private _ucr As New UserControlResource
    Private _price As Decimal = 0

#End Region

#Region "Public Properties"

    Public ReadOnly Property ErrorMessage As String
        Get
            Return _errorMessage
        End Get
    End Property
    Public ReadOnly Property SuccessMessage As String
        Get
            Return _successMessage
        End Get
    End Property
    Public ReadOnly Property CashbackChanged As Boolean
        Get
            Return _cashbackChanged
        End Get
    End Property
    Public SupporterColumnHeader As String
    Public TotalRewardColumnHeader As String
    Public AvailableRewardColumnHeader As String
    Public SelectThisRewardColumnHeader As String
    Public ApplyAmountColumnHeader As String


#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ApplyCashback.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _errorMessage = String.Empty
        _successMessage = String.Empty
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Try
            Dim cashbackValue As Decimal = 0
            Dim basketHasCashback As Boolean = False
            For Each item As TalentBasketItem In Profile.Basket.BasketItems
                If item.Product = ModuleDefaults.CashBackFeeCode Then
                    basketHasCashback = True
                    cashbackValue = item.Gross_Price
                    Exit For
                End If
            Next
            If basketHasCashback Then
                rptApplyCashback.Visible = False
                plhTotalAvailable.Visible = False
                plhTotalApplied.Visible = True
                btnApplyNow.Visible = False
                btnClearAppliedCashback.Visible = True
                ltlCashbackAppliedLabel.Text = _ucr.Content("CashBackAppliedLabel", _languageCode, True)
                btnClearAppliedCashback.Text = _ucr.Content("ClearAppliedCashbackButtonText", _languageCode, True)
                ltlCashbackApplied.Text = FormatCurrency(cashbackValue)
            Else
                rptApplyCashback.Visible = True
                plhTotalAvailable.Visible = True
                plhTotalApplied.Visible = False
                btnApplyNow.Visible = True
                btnClearAppliedCashback.Visible = False
                bindRepeater()
                btnApplyNow.Text = _ucr.Content("ApplyButtonText", _languageCode, True)
                btnApplyNow.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnApplyNow))
            End If
            If rptApplyCashback.Visible Then
                btnApplyNow.Visible = True
                plhTotalAvailable.Visible = True
                plhTotalApplied.Visible = False
                btnClearAppliedCashback.Visible = False
            Else
                btnApplyNow.Visible = False
                plhTotalAvailable.Visible = False
                plhTotalApplied.Visible = True
                btnClearAppliedCashback.Visible = True
            End If


        Catch ex As Exception
            Me.Visible = False
        End Try
    End Sub

    Protected Sub checkChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim hello As String = String.Empty
        _cashbackChanged = True
    End Sub

    Protected Sub rptApplyCashback_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptApplyCashback.ItemDataBound
        If e.Item.ItemType = ListItemType.Header Then
            Try
                SupporterColumnHeader = _ucr.Content("SupporterColumnHeader", _languageCode, True)
                TotalRewardColumnHeader = _ucr.Content("TotalRewardColumnHeader", _languageCode, True)
                AvailableRewardColumnHeader = _ucr.Content("AvailableRewardColumnHeader", _languageCode, True)
                SelectThisRewardColumnHeader = _ucr.Content("SelectThisRewardColumnHeader", _languageCode, True)
                ApplyAmountColumnHeader = _ucr.Content("ApplyAmountColumnHeader", _languageCode, True)
                ' ApplyMaxColumnHeader = "ApplyMax".
            Catch ex As Exception
                rptApplyCashback.Visible = False
                _errorMessage = _ucr.Content("ErrorRequestingCashbackRewards", _languageCode, True)
            End Try
        ElseIf e.Item.ItemType = ListItemType.AlternatingItem Or e.Item.ItemType = ListItemType.Item Then
            Try
                CType(e.Item.FindControl("ltlSupporterNumberNameSeparator"), Literal).Text = _ucr.Content("SupporterNumberNameSeparator", _languageCode, True)
                ' for signed in customer, available reward must be set to total that can be applied...
                If e.Item.DataItem("CustomerNumber") = Profile.User.Details.LoginID Then
                    If CType(e.Item.FindControl("hdfAvailableReward"), HiddenField).Value = 0 Then
                        CType(e.Item.FindControl("txtApplyAmount"), TextBox).Visible = False
                        CType(e.Item.FindControl("chkSelectThisReward"), CheckBox).Visible = False
                    End If
                Else
                    ' apply amount is not valid.
                    CType(e.Item.FindControl("txtApplyAmount"), TextBox).Visible = False
                    If Not CBool(e.Item.DataItem("OnAccountEnabled")) Then
                        CType(e.Item.FindControl("lblSelectThisRewardLabel"), Label).Visible = False
                    End If
                End If
            Catch ex As Exception
                rptApplyCashback.Visible = False
                _errorMessage = _ucr.Content("ErrorRequestingCashbackRewards", _languageCode, True)
            End Try
        End If
    End Sub

    Protected Sub btnApplyNow_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnApplyNow.Click
        Dim successfulUpdate As Boolean = True
        Dim customersSelectedCashback As New Collection
        Dim totalRewardSelected As New Collection
        Dim availableRewardSelected As New Collection
        _errorMessage = String.Empty
        Dim totalCashbackSelected As Decimal = 0
        Dim reason As String = String.Empty
        Dim totalPayableInBasket As Decimal = 0

        If hdfOnaccountEnabled.Value = "True" Then
            totalPayableInBasket = CDec(hdfTicketsCost.Value)
        End If

        For Each item As RepeaterItem In rptApplyCashback.Items
            Try
                Dim chkSelectThisReward As CheckBox = CType(item.FindControl("chkSelectThisReward"), CheckBox)

                If Not item.FindControl("txtApplyAmount") Is Nothing AndAlso (CType(item.FindControl("txtApplyAmount"), TextBox).Text) <> String.Empty Then
                    chkSelectThisReward.Checked = False
                End If

                ' Apply Max is checked - Apply Amount must be empty
                If chkSelectThisReward.Checked Then
                    If Not item.FindControl("txtApplyAmount") Is Nothing AndAlso (CType(item.FindControl("txtApplyAmount"), TextBox).Text) <> String.Empty Then
                        successfulUpdate = False
                        reason = "ApplyAmountAndApplyMaxError"
                    Else
                        customersSelectedCashback.Add(CType(item.FindControl("hdfCustomerNumber"), HiddenField).Value)
                        totalRewardSelected.Add(CType(item.FindControl("hdfTotalReward"), HiddenField).Value.Replace(".", ""))
                        If hdfOnaccountEnabled.Value = "True" Then
                            availableRewardSelected.Add(CType(item.FindControl("hdfApplyMax"), HiddenField).Value.Replace(".", ""))
                            totalCashbackSelected += CDec(CType(item.FindControl("hdfApplyMax"), HiddenField).Value)
                        Else
                            '   Dim availReward As dec
                            availableRewardSelected.Add(CType(item.FindControl("hdfAvailableReward"), HiddenField).Value.Replace(".", ""))
                            totalCashbackSelected += CDec(CType(item.FindControl("hdfAvailableReward"), HiddenField).Value)
                        End If
                        'availableRewardSelected.Add(CType(item.FindControl("hdfAvailableReward"), HiddenField).Value.Replace(".", ""))
                    End If
                Else
                    ' Apply Max is not checked - see if there's anything in Apply Amount and check it's a valid amount
                    If Not item.FindControl("txtApplyAmount") Is Nothing Then
                        Dim applyAmount As Decimal = 0
                        Try
                            applyAmount = CDec(CType(item.FindControl("txtApplyAmount"), TextBox).Text)
                            If applyAmount > 0 AndAlso applyAmount <= CDec(CType(item.FindControl("hdfAvailableReward"), HiddenField).Value) Then
                                customersSelectedCashback.Add(CType(item.FindControl("hdfCustomerNumber"), HiddenField).Value)
                                totalRewardSelected.Add(CType(item.FindControl("hdfTotalReward"), HiddenField).Value.Replace(".", ""))
                                'availableRewardSelected.Add(CType(item.FindControl("hdfAvailableReward"), HiddenField).Value.Replace(".", ""))
                                availableRewardSelected.Add(applyAmount * 100)
                                totalCashbackSelected += applyAmount
                            Else
                                If applyAmount <> 0 Then
                                    successfulUpdate = False
                                    reason = "ApplyAmountError"
                                End If

                            End If
                        Catch ex As Exception
                        End Try
                    End If
                End If
            Catch ex As Exception
                successfulUpdate = False
            End Try
        Next

        If hdfOnaccountEnabled.Value = "True" AndAlso totalCashbackSelected > totalPayableInBasket Then
            successfulUpdate = False
            reason = "ApplyAmountError"
        End If

        If successfulUpdate Then
            Try
                If customersSelectedCashback.Count > 0 Then
                    Dim payment As TalentPayment = createPaymentObject("F")
                    Dim err As New ErrorObj
                    payment.De.CashbackCustomersSelected = customersSelectedCashback
                    payment.De.TotalRewardSelected = totalRewardSelected
                    payment.De.AvailableRewardSelected = availableRewardSelected
                    err = payment.UpdateCashback
                    If err.HasError Then
                        _errorMessage = _ucr.Content("ErrorApplyingCashbackRewards", _languageCode, True)
                    Else
                        If payment.ResultDataSet.Tables(0).Rows.Count > 0 Then
                            Dim returnCode As String = payment.ResultDataSet.Tables(0).Rows(0)("ReturnCode").ToString().Trim
                            If Not String.IsNullOrEmpty(returnCode) Then
                                Dim talentErrorMessages As New TalentErrorMessages(_languageCode, _ucr.BusinessUnit, _ucr.PartnerCode, _ucr.FrontEndConnectionString)
                                Dim talentErrorMessage As New TalentErrorMessage
                                talentErrorMessage = talentErrorMessages.GetErrorMessage(GetAllString, GetCurrentPageName, returnCode, True)
                                _errorMessage = talentErrorMessage.ERROR_MESSAGE
                            End If
                        Else
                            err = updateBasketWithCashbackFee(customersSelectedCashback, availableRewardSelected)
                            If err.HasError Then
                                _errorMessage = _ucr.Content("ErrorApplyingCashbackRewards", _languageCode, True)
                            Else
                                _successMessage = _ucr.Content("SuccessfullyAppliedCashback", _languageCode, True).Replace("<<Amount>>", _price.ToString("N2"))
                                ' If basket is now zero, or just has the CCFEE then pay for zero basket
                                Dim completeSale As Boolean = False
                                Dim bkFee As Decimal = 0
                                Try
                                    Dim BasketTotalWithoutFees As Decimal = Profile.Basket.BasketSummary.TotalBasket
                                    Dim BasketTotalWithFees As Decimal = Profile.Basket.BasketSummary.TotalBasketWithoutPayTypeFee
                                    If Profile.Basket.BasketSummary.TotalOnAccount(ModuleDefaults.CashBackFeeCode) <> 0 AndAlso Profile.Basket.BasketSummary.TotalBasket = 0 Then
                                        Checkout.StoreOADetails()
                                        Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
                                        ticketingGatewayFunctions.CheckoutPayment()
                                    End If
                                Catch ex As Exception

                                End Try
                            End If
                        End If
                    End If
                Else
                    _errorMessage = _ucr.Content("NoRewardsSelectedError", _languageCode, True)
                End If
            Catch ex As Exception
                _errorMessage = _ucr.Content("ErrorApplyingCashbackRewards", _languageCode, True)
            End Try
        Else
            ' if a specific reason on validation then output appropriate error
            If reason <> String.Empty Then
                _errorMessage = _ucr.Content(reason, _languageCode, True)
            Else
                _errorMessage = _ucr.Content("ErrorApplyingCashbackRewards", _languageCode, True)
            End If
        End If

        If _errorMessage.Length > 0 Then
            bindRepeater()
            btnApplyNow.Visible = True
            btnApplyNow.Text = _ucr.Content("ApplyButtonText", _languageCode, True)
            btnApplyNow.Attributes.Add("onclick", getJSFunctionForConfirmButton(btnApplyNow))
        Else
            rptApplyCashback.Visible = False
        End If
        _cashbackChanged = True
    End Sub

    Protected Sub btnClearAppliedCashback_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnClearAppliedCashback.Click
        Dim err As New ErrorObj
        err = ClearCashback()
        If Not err.HasError Then
            _cashbackJustCleared = True
        End If
        rptApplyCashback.Visible = True
        _cashbackChanged = True
        Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True, True)
    End Sub

    Protected Sub SetRegex(ByVal sender As Object, ByVal e As EventArgs)
        Dim reg As RegularExpressionValidator = CType(sender, RegularExpressionValidator)
        reg.ValidationExpression = _ucr.Attribute("CashBackAmountRegularExpression") ' ^-?\d{0,9}(\.\d{1,2})?$
        reg.ErrorMessage = _ucr.Content("CashBackAmountErrorMessage", _languageCode, True)
    End Sub
#End Region

#Region "Public Methods"
    ' below code is same as getCashbackAvailable() in cashBackSummary.aspx so if change here change there too 
    Public Function RetrieveCashback() As Decimal
        Dim maxOnAccountAvailableForBasket As Decimal = 0
        Dim payment As TalentPayment = createPaymentObject("E")
        Dim err As New ErrorObj
        err = payment.RetrieveCashback
        Dim feeTotal As Decimal = Profile.Basket.BasketSummary.TotalTicketingFees
        If Not err.HasError Then
            If payment.ResultDataSet.Tables.Count > 1 Then
                If payment.ResultDataSet.Tables(0).Rows.Count = 0 Then
                    If payment.ResultDataSet.Tables(1).Rows.Count > 0 Then
                        Dim paymentOwnerRow As DataRow
                        paymentOwnerRow = payment.ResultDataSet.Tables(1).Rows(0)
                        If paymentOwnerRow("AvailableReward") > 0 Then
                            maxOnAccountAvailableForBasket = paymentOwnerRow("AvailableReward") + feeTotal
                        End If
                        If maxOnAccountAvailableForBasket > paymentOwnerRow("TotalReward") Then
                            maxOnAccountAvailableForBasket = paymentOwnerRow("TotalReward")
                        End If
                    End If
                End If
            End If
        End If
        Return maxOnAccountAvailableForBasket
    End Function

    Public Function GetCustomerName(ByVal customerNumber As String) As String
        Dim customerName As String = String.Empty
        Try
            If ModuleDefaults.FriendsAndFamily Then
                If customerNumber = Profile.User.Details.LoginID Then
                    customerName = Profile.User.Details.Forename & " " & Profile.User.Details.Surname
                Else
                    Dim _talentErrObj As New ErrorObj
                    Dim _deCustomer As New DECustomer
                    Dim _deSettings As New DESettings
                    Dim _talentCustomer As New TalentCustomer
                    Dim _deCustV11 As New DECustomerV11

                    _deSettings.FrontEndConnectionString = _ucr.FrontEndConnectionString
                    _deSettings.BusinessUnit = _ucr.BusinessUnit
                    _deSettings.StoredProcedureGroup = GetStoredProcedureGroup()
                    _deSettings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
                    _deSettings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)

                    _deCustomer.UserName = Profile.UserName
                    _deCustomer.CustomerNumber = Profile.User.Details.LoginID
                    _deCustomer.Source = "W"
                    _deCustV11.DECustomersV1.Add(_deCustomer)
                    _talentCustomer.DeV11 = _deCustV11
                    _talentCustomer.Settings = _deSettings
                    _talentErrObj = _talentCustomer.CustomerAssociations

                    If Not _talentErrObj.HasError Then
                        If _talentCustomer.ResultDataSet.Tables.Count > 1 Then
                            Dim dtFriendsAndFamily As DataTable = _talentCustomer.ResultDataSet.Tables(1)
                            For Each row As DataRow In dtFriendsAndFamily.Rows
                                If customerNumber = row("AssociatedCustomerNumber").ToString Then
                                    customerName = row("Forename").ToString.Trim & " " & row("Surname").ToString.Trim
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                End If
            Else
                customerName = Profile.User.Details.Forename & " " & Profile.User.Details.Surname
            End If
        Catch ex As Exception
            customerName = String.Empty
        End Try
        Return customerName
    End Function

    ''' <summary>
    ''' Format the currency based on the given value
    ''' </summary>
    ''' <param name="value">The given value</param>
    ''' <returns>The formatted value</returns>
    ''' <remarks></remarks>
    Public Function FormatCurrency(ByVal value As Decimal) As String
        Dim formattedValue As String = value
        formattedValue = TDataObjects.PaymentSettings.FormatCurrency(value, _ucr.BusinessUnit, _ucr.PartnerCode)
        Return formattedValue
    End Function

#End Region

#Region "Private Methods"

    Private Sub bindRepeater()
        Dim payment As TalentPayment = createPaymentObject("R")
        Dim err As New ErrorObj
        err = payment.RetrieveCashback

        If Not err.HasError Then
            If payment.ResultDataSet.Tables.Count > 1 Then
                If payment.ResultDataSet.Tables(0).Rows.Count > 0 Then
                    'Error Returned (Talent error)
                    Dim returnCode As String = payment.ResultDataSet.Tables(0).Rows(0)("ReturnCode").ToString()
                    Dim talentErrorMessages As New TalentErrorMessages(_languageCode, _ucr.BusinessUnit, _ucr.PartnerCode, _ucr.FrontEndConnectionString)
                    Dim talentErrorMessage As New TalentErrorMessage
                    talentErrorMessage = talentErrorMessages.GetErrorMessage(GetAllString, GetCurrentPageName, returnCode, True)
                    _errorMessage = talentErrorMessage.ERROR_MESSAGE
                    Me.Visible = False

                Else
                    'No Error Record
                    If payment.ResultDataSet.Tables(1).Rows.Count > 0 Then
                        'Valid Results returned  
                        Dim totalAvailable As Decimal = 0
                        Dim paymentOwnerTotal As Decimal = 0
                        Dim basketApplicableTotal As Decimal = 0
                        Dim FandFTotal As Decimal = 0
                        Dim dtRewards As DataTable = payment.ResultDataSet.Tables(1).Copy
                        Dim feeTotal As Decimal = Profile.Basket.BasketSummary.TotalTicketingFees

                        If Not dtRewards.Columns.Contains("ApplyMax") Then
                            dtRewards.Columns.Add("ApplyMax", GetType(Decimal))
                        End If

                        If CBool(dtRewards.Rows(0)("OnAccountEnabled")) Then
                            hdfOnaccountEnabled.Value = "True"
                            plhCashBackAvailable.Visible = False
                            For Each row As DataRow In dtRewards.Rows
                                totalAvailable += row("AvailableReward")
                                ' If on account is active Payment owner can pay for their seat, F&F Seats amd fees
                                If row("CustomerNumber").ToString = Profile.User.Details.LoginID Then
                                    If row("AvailableReward") > 0 Then
                                        paymentOwnerTotal = row("AvailableReward") + feeTotal
                                    End If
                                    If paymentOwnerTotal > CDec(row("TotalReward")) Then
                                        paymentOwnerTotal = CDec(row("TotalReward"))
                                    End If
                                    row("AvailableReward") = paymentOwnerTotal
                                    row("ApplyMax") = 0
                                Else
                                    ' Apply Max amount available for non-payment owner 
                                    ' is the same as available reward
                                    FandFTotal += CDec(row("AvailableReward"))
                                    row("ApplyMax") = CDec(row("AvailableReward"))
                                End If
                            Next

                            ' now reprocess the basket and update the payment owners' applyMax
                            For Each row As DataRow In dtRewards.Rows
                                If row("CustomerNumber").ToString = Profile.User.Details.LoginID Then
                                    If paymentOwnerTotal > totalAvailable Then
                                        row("ApplyMax") = paymentOwnerTotal - FandFTotal
                                    Else
                                        If paymentOwnerTotal > 0 Then
                                            row("ApplyMax") = paymentOwnerTotal + feeTotal
                                        End If
                                        End If
                                        If CDec(row("ApplyMax")) < 0 Then
                                            row("ApplyMax") = 0
                                        End If
                                        If CDec(row("ApplyMax")) > paymentOwnerTotal Then
                                            row("ApplyMax") = paymentOwnerTotal
                                        End If
                                End If

                                ' Total on-account that can be applied
                                basketApplicableTotal += row("ApplyMax")
                            Next
                        Else
                            ' Cashback only... No ApplyMax field
                            hdfOnaccountEnabled.Value = "False"
                            ltlBalanceLabel.Visible = False
                            ltlBalance.Visible = False
                            ltlTicketsCostLabel.Visible = False
                            ltlTicketsCost.Visible = False
                            For Each row As DataRow In dtRewards.Rows
                                row("ApplyMax") = 0
                                totalAvailable += row("AvailableReward")
                            Next
                        End If

                        ' Balance is remaining to pay assuming no credit card fee will be charged  
                        Dim balance As Decimal = Profile.Basket.BasketSummary.TotalBasket - basketApplicableTotal

                        ' If each member applying maximum on-account more than covers the total reduce the payment owner max apply    
                        If balance < 0 Then
                            For Each row As DataRow In dtRewards.Rows
                                If row("CustomerNumber").ToString = Profile.User.Details.LoginID Then
                                    row("ApplyMax") += balance
                                    '  row("AvailableReward") += balance
                                    basketApplicableTotal += balance
                                    Exit For
                                End If
                            Next
                            balance = 0
                        End If

                        ltlBalanceLabel.Text = _ucr.Content("BalanceLabel", _languageCode, True)
                        ltlTicketsCostLabel.Text = _ucr.Content("TicketsCostLabel", _languageCode, True)
                        ltlTicketsCost.Text = FormatCurrency(basketApplicableTotal)
                        hdfTicketsCost.Value = basketApplicableTotal
                        ltlBalance.Text = FormatCurrency(balance)
                        rptApplyCashback.DataSource = dtRewards
                        rptApplyCashback.DataBind()
                        ltlCashbackAvailable.Text = FormatCurrency(totalAvailable)
                        ltlCashbackAvailableLabel.Text = _ucr.Content("CashbackAvailableLabel", _languageCode, True)
                        rptApplyCashback.Visible = True
                    Else
                        'No Results
                        rptApplyCashback.Visible = False
                        _errorMessage = _ucr.Content("NoCashbackRewardsFound", _languageCode, True)
                    End If
                End If
            Else
                'No Results
                rptApplyCashback.Visible = False
                _errorMessage = _ucr.Content("NoCashbackRewardsFound", _languageCode, True)
            End If
        Else
            'Error Occured (DB Access/Stored Procedure/Data cast issue)
            rptApplyCashback.Visible = False
            _errorMessage = _ucr.Content("ErrorRequestingCashbackRewards", _languageCode, True)
        End If
    End Sub

#End Region

#Region "Private Functions"

    Private Function updateBasketWithCashbackFee(ByVal customersSelectedCashback As Collection, ByVal availableRewardSelected As Collection) As ErrorObj
        Dim err As New ErrorObj
        Dim _TDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        Dim affectedRows As Integer = 0
        Dim i As Integer = 1

        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = "SQL2005"
        _TDataObjects.Settings = settings
        Try
            While customersSelectedCashback.Count >= i
                Try
                    _price += availableRewardSelected(i) / 100
                Catch
                    _price += 0
                End Try
                i += 1
            End While
            If _price > 0 Then
                Dim basketFeeEntity As New DEBasketFees
                basketFeeEntity.FeeCode = ModuleDefaults.CashBackFeeCode
                basketFeeEntity.BasketHeaderID = Profile.Basket.Basket_Header_ID
                basketFeeEntity.FeeDescription = "Cashback"
                basketFeeEntity.FeeValue = (_price) * (-1)
                affectedRows = _TDataObjects.PaymentSettings.TblBasketDetail.InsertFee(Profile.Basket.Basket_Header_ID, Profile.UserName, basketFeeEntity, False, True)
            End If

            If affectedRows = 0 Then
                err.HasError = True
                err.ErrorMessage = "Error during database access"
            Else
                Profile.Basket = Profile.Provider.GetBasket(Profile.User.Details.LoginID, True, True)
            End If
        Catch ex As Exception
            err.HasError = True
            err.ErrorMessage = ex.Message
        End Try

        Return err
    End Function

    Private Function createPaymentObject(ByVal mode As String) As TalentPayment
        Dim payment As New TalentPayment
        Dim deSettings As New DESettings

        deSettings.FrontEndConnectionString = _ucr.FrontEndConnectionString
        deSettings.BusinessUnit = _ucr.BusinessUnit
        deSettings.StoredProcedureGroup = GetStoredProcedureGroup()
        deSettings.Cacheing = CType(_ucr.Attribute("Cacheing"), Boolean)
        deSettings.CacheTimeMinutes = CType(_ucr.Attribute("CacheTimeMinutes"), Integer)
        deSettings.OriginatingSource = GetOriginatingSource(Session.Item("Agent"))

        payment.De.CashbackMode = mode
        payment.De.Source = "W"
        payment.De.SessionId = Profile.Basket.Basket_Header_ID
        payment.De.CustomerNumber = Profile.User.Details.LoginID
        If Profile.Basket.BasketContentType <> GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
            Dim merchandiseAmount As Decimal = 0
            merchandiseAmount = Profile.Basket.BasketSummary.MerchandiseTotal
            payment.De.Amount = Format(merchandiseAmount, "0.00")
        End If
        payment.Settings() = deSettings
        Return payment
    End Function

    Private Function ClearCashback() As ErrorObj
        Dim err As New ErrorObj
        If Profile.Basket.BasketItems.Count > 0 Then
            Dim CashbackFeeItem As TalentBasketItem = Profile.Basket.BasketItems.Find(Function(basketDetailItem As TalentBasketItem) basketDetailItem.Product = ModuleDefaults.CashBackFeeCode)
            If CashbackFeeItem IsNot Nothing Then
                TDataObjects.PaymentSettings.TblBasketDetail.DeleteFee(ModuleDefaults.CashBackFeeCode, Profile.Basket.Basket_Header_ID)
                Dim payment As TalentPayment = createPaymentObject("D")
                err = payment.UpdateCashback
                If Not err.HasError Then
                    _successMessage = _ucr.Content("CashbackClearedMessage", _languageCode, True)
                    Dim basketItem As New TalentBasketItem
                    For Each item As TalentBasketItem In Profile.Basket.BasketItems
                        If item.Product = ModuleDefaults.CashBackFeeCode Then
                            basketItem = item
                            Exit For
                        End If
                    Next
                    Profile.Basket.BasketItems.Remove(basketItem)
                End If
            End If
        End If
        Return err
    End Function

    ''' <summary>
    ''' Returns a string that disables the confirm button and changes its text
    ''' </summary>
    ''' <param name="ConfirmButton">The button we are applying the change to</param>
    ''' <returns>The string we need to set on the onlick attribute</returns>
    ''' <remarks></remarks>
    Private Function getJSFunctionForConfirmButton(ByRef ConfirmButton As Button) As String
        Dim javascriptFunction As New StringBuilder()
        javascriptFunction.Append("if (typeof(Page_ClientValidate) == 'function') { ")
        javascriptFunction.Append("var oldPage_IsValid = Page_IsValid; var oldPage_BlockSubmit = Page_BlockSubmit;")
        javascriptFunction.Append("if (Page_ClientValidate('")
        javascriptFunction.Append(ConfirmButton.ValidationGroup)
        javascriptFunction.Append("') == false) {")
        javascriptFunction.Append(" Page_IsValid = oldPage_IsValid; Page_BlockSubmit = oldPage_BlockSubmit; return false; }} ")
        javascriptFunction.Append("this.value = '")
        javascriptFunction.Append(_ucr.Content("ProcessingText", _languageCode, True))
        javascriptFunction.Append("';")
        javascriptFunction.Append("this.disabled = true;")
        javascriptFunction.Append(Me.Page.ClientScript.GetPostBackEventReference(ConfirmButton, Nothing))
        javascriptFunction.Append(";")
        javascriptFunction.Append("return true;")
        Return javascriptFunction.ToString()
    End Function

#End Region

End Class