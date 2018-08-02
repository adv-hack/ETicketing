Imports System.Data
Imports Talent.eCommerce

Partial Class UserControls_EPurse
    Inherits ControlBase

#Region "Public Properties"

    Public Property Display() As Boolean = True
    Public Property TakingPayment() As Boolean = True
    Public Property ErrorList() As New BulletedList
    Public Property SuccessList() As New BulletedList

    Public Property PaymentOptionSelected As Boolean = False
    Public Property BasketTotal() As Decimal
        Get
            Return _basketTotal
        End Get
        Set(ByVal value As Decimal)
            _basketTotal = value
        End Set
    End Property
    Public Property BasketTotalWithFees() As Decimal
        Get
            Return _basketTotalWithFees
        End Get
        Set(ByVal value As Decimal)
            _basketTotalWithFees = value
        End Set
    End Property
    Public ReadOnly Property Card() As Literal
        Get
            Return Me.ltlEPurseCardValue
        End Get
    End Property
    Public ReadOnly Property PaymentAmount() As String
        Get
            Return txtPaymentAmount.Text
        End Get
    End Property
    Public ReadOnly Property EPurseOptionsChanged() As Boolean
        Get
            Return _ePurseOptionsChanged
        End Get
    End Property
    Public Property IsGiftCard() As Boolean
        Get
            Return _isGiftCard
        End Get
        Set(ByVal value As Boolean)
            _isGiftCard = value
        End Set
    End Property
    Public ReadOnly Property PIN() As String
        Get
            Return txtPin.Text
        End Get
    End Property

    Public ReadOnly Property GiftCardNumber() As String
        Get
            Return txtGiftCard.Text
        End Get
    End Property
    Public Property ShowTAndCs() As Boolean

#End Region

#Region "Class Level Fields"

    Private ucr As New Talent.Common.UserControlResource
    Private log As Talent.Common.TalentLogging = Utilities.TalentLogging
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private def As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Private errMsg As Talent.Common.TalentErrorMessages
    Private _basketTotal As Decimal = 0
    Private _basketTotalWithFees As Decimal = 0
    Private _ePurseOptionsChanged As Boolean = False
    Private _isGiftCard As Boolean
    Private _pin As String

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "EPurse.ascx"
        End With
        errMsg = New Talent.Common.TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), ucr.FrontEndConnectionString)
        _basketTotal = Profile.Basket.BasketSummary.TotalBasket
        _basketTotalWithFees = Profile.Basket.BasketSummary.TotalBasketWithoutPayTypeFee
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        If Me.Display Then
            _basketTotal = Profile.Basket.BasketSummary.TotalBasket
            _basketTotalWithFees = Profile.Basket.BasketSummary.TotalBasketWithoutPayTypeFee
            SuccessList.Items.Clear()
            If Not Me.TakingPayment Then Me.plhPartPayment.Visible = False
            errMsg = New Talent.Common.TalentErrorMessages(_languageCode, _
                                                        TalentCache.GetBusinessUnitGroup, _
                                                        TalentCache.GetPartner(Profile), _
                                                        ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)

            'Build the javascript
            Dim sbJavaScript As New StringBuilder
            sbJavaScript.Append("<script language=""javascript"" type=""text/javascript"">")
            sbJavaScript.Append(" " & "function ePursePopulateTextBox(card) { ")
            sbJavaScript.Append(" " & "document.getElementById('" & txtEPurseCard.ClientID & "').value = card; }")
            sbJavaScript.Append(" " & "</script>")
            Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "DDLTextBoxJS", sbJavaScript.ToString(), False)
            sbJavaScript = Nothing

            If Not IsPostBack OrElse Me.hdfDisplay.Value <> Me.Display.ToString OrElse PaymentOptionSelected Then

                'Display a part payment success message when required
                If Session.Item("PartPaymentSuccess") = "True" Then
                    ErrorList.Items.Add(ucr.Content("PartPaymentSuccessMessage", _languageCode, True))
                    Session.Remove("PartPaymentSuccess")
                End If


                'Display the first screen
                Me.plhEPurse.Visible = True

                If _isGiftCard Then
                    txtGiftCard.Enabled = True
                    txtPin.Enabled = True
                    Me.plhGiftCard.Visible = True
                    txtGiftCard.Enabled = True
                    txtPin.Enabled = True
                    txtGiftCard.Text = String.Empty
                    txtPin.Text = String.Empty
                    Me.plhEPFirst.Visible = False
                    If Me.hdfTotalAvailableValue.Value IsNot Nothing AndAlso Me.hdfTotalAvailableValue.Value <> "0.00" AndAlso Not String.IsNullOrEmpty(Me.hdfTotalAvailableValue.Value) AndAlso Not PaymentOptionSelected Then
                        Me.plhEPSecond.Visible = True
                        plhNoBalance.Visible = False
                    Else
                        Me.plhEPSecond.Visible = False
                    End If

                    Me.ltlEPurseSummaryTitle.Visible = False
                    Me.ltlEPurseSummaryTitle.Visible = False
                    Me.ltlEPurseCard.Visible = False
                    Me.ltlTotalPoints.Visible = False
                    Me.ltlTotalAvailable.Visible = False
                    Dim plhEPPartPmt As PlaceHolder = CType(Parent.FindControl("plhEPPartPmt"), PlaceHolder)
                    plhEPPartPmt.Visible = False
                    _pin = txtPin.Text
                Else
                    Me.plhEPFirst.Visible = True

                    Me.plhEPSecond.Visible = False
                    If (Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowedInAgentMode")) Or
                        (Not Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowed")) Then
                        Me.plhManualEnter.Visible = True
                        Me.plhDDLSelect.Visible = False
                    Else
                        Me.plhManualEnter.Visible = False
                        Me.plhDDLSelect.Visible = True
                    End If
                End If
                'Set up the screen fields
                SetText()
                SetValidators()
                SetFields()
            End If
            'Put the total amount available or the basket total amount in the payment text box, which ever is less
            If hdfTotalAvailableValue.Value.Length = 0 Then hdfTotalAvailableValue.Value = 0
            If Convert.ToDecimal(hdfTotalAvailableValue.Value) < _basketTotalWithFees Then
                txtPaymentAmount.Text = Convert.ToDecimal(hdfTotalAvailableValue.Value).ToString("N2")
            Else
                txtPaymentAmount.Text = _basketTotalWithFees.ToString("N2")
            End If

            'Setup the display of the various options
            If plhEPSecond.Visible AndAlso ModuleDefaults.EPurseTopUpProductCode.Length > 0 Then plhNextCard.Visible = False
            Dim plhEPurseTAndC As PlaceHolder = Parent.FindControl("plhEPurseTAndC")
            If plhEPurseTAndC IsNot Nothing Then
                If ShowTAndCs Then
                    plhEPurseTAndC.Visible = plhEPSecond.Visible
                Else
                    plhEPurseTAndC.Visible = False
                End If
            End If
            Dim btnConfirmEPursePayment As Button = Parent.FindControl("btnConfirmEPursePayment")
            If btnConfirmEPursePayment IsNot Nothing Then btnConfirmEPursePayment.Visible = plhEPSecond.Visible
        Else
            plhEPurse.Visible = False
        End If

        Me.hdfDisplay.Value = Me.Display.ToString

    End Sub

    Protected Sub SetText()
        With ucr
            Me.ltlEPurseCard.Text = .Content("ltlEPurseCardText", _languageCode, True)
            Me.ltlEPurseCard1.Text = .Content("ltlEPurseCardText", _languageCode, True)
            Me.ltlEPurseCard2.Text = .Content("ltlEPurseCardText", _languageCode, True)
            Me.ltlTotalAvailable.Text = .Content("ltlTotalAvailableText", _languageCode, True)
            Me.ltlPaymentTotal.Text = .Content("ltlPaymentTotalText", _languageCode, True)
            Me.ltlPartPaymentTitle.Text = .Content("ltlPartPaymentTitle", _languageCode, True)
            Me.ltlPartPaymentDescription.Text = .Content("ltlPartPaymentDescriptionText", _languageCode, True)
            Me.ltlEPurseSummaryTitle.Text = .Content("ltlEPurseSummaryTitleText", _languageCode, True)
            Me.ltlTotalPoints.Text = .Content("ltlltlTotalPointsText", _languageCode, True)
            Me.btnRetrieveBalance.Text = .Content("btnRetrieveBalanceText", _languageCode, True)
            Me.btnNextCard.Text = .Content("btnNextCardText", _languageCode, True)
            Me.lblPin.Text = .Content("ltlPinText", _languageCode, True)
            Me.lblGiftCard.Text = .Content("ltlGiftCardText", _languageCode, True)
            Me.btnRetrieveGiftCardBalance.Text = .Content("btnRetrieveGiftCardBalanceText", _languageCode, True)
        End With
    End Sub

    Protected Sub btnRetrieveBalance_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRetrieveBalance.Click
        ErrorList.Items.Clear()
        SuccessList.Items.Clear()
        If Page.IsValid Then
            RetrieveBalance(False)
        End If
        _ePurseOptionsChanged = True
    End Sub

    Protected Sub btnRetrieveGiftCardBalance_Click(sender As Object, e As EventArgs) Handles btnRetrieveGiftCardBalance.Click
        ErrorList.Items.Clear()
        SuccessList.Items.Clear()
        If Page.IsValid Then
            RetrieveGiftCardBalance()
        End If
        _ePurseOptionsChanged = True
    End Sub

    Protected Sub SetFields()

        ErrorList.Items.Clear()
        Try
            '
            ' Retrieve this customer smartcards
            '
            Dim err As New Talent.Common.ErrorObj
            Dim ts As New Talent.Common.TalentSmartcard
            Dim deSmartcard As New Talent.Common.DESmartcard
            With deSmartcard
                If Not Profile.User.Details.LoginID Is Nothing Then
                    .CustomerNumber = Profile.User.Details.LoginID
                End If
            End With
            '
            ts.DE = deSmartcard
            ts.Settings = Utilities.GetSettingsObject()
            ts.Settings.Cacheing = ucr.Attribute("Cacheing")
            ts.Settings.CacheTimeMinutes = ucr.Attribute("CacheTimeMinutes")
            err = ts.RetrieveSmartcardCards
            '
            ' Populate the drop down lists
            If Not err.HasError AndAlso _
                ts.ResultDataSet.Tables.Count = 2 AndAlso _
                ts.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then

                'Clear the current list
                If (Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowedInAgentMode")) Or
                        (Not Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowed")) Then
                    If ts.ResultDataSet.Tables("Smartcards").Rows.Count = 0 Then
                        Me.ddeEPurseCard.Enabled = False
                    End If
                Else
                    Me.ddlSelectCard.Items.Clear()

                    'Display the no card message when empty
                    If ts.ResultDataSet.Tables("Smartcards").Rows.Count = 0 Then
                        ErrorList.Items.Add(ucr.Content("NoEPurseCards", _languageCode, True))
                        Me.plhEPFirst.Visible = False
                    End If
                End If

                'loop through the returned data table 
                Dim selected As Boolean = False

                If (Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowedInAgentMode")) Or
                        (Not Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowed")) Then
                    rptSmartcards.DataSource = ts.ResultDataSet.Tables("Smartcards")
                    rptSmartcards.DataBind()
                End If

                For Each dr As DataRow In ts.ResultDataSet.Tables("Smartcards").Rows
                    Dim li As String = dr("CardNumber").ToString.TrimStart("0")
                    If Not (Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowedInAgentMode")) AndAlso
                       Not (Not Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowed")) Then
                        Me.ddlSelectCard.Items.Add(li)
                    End If

                    'If the logged in customer has a smartcard then we should use this to retrieve the
                    'balance
                    If Me.TakingPayment Then
                        If Not Profile.User.Details.LoginID Is Nothing AndAlso Not selected Then
                            If Not String.IsNullOrEmpty(Session.Item("LastEPurseCard")) Then
                                If (Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowedInAgentMode")) Or
                                        (Not Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowed")) Then
                                    Me.txtEPurseCard.Text = Session.Item("LastEPurseCard")
                                Else
                                    Me.ddlSelectCard.SelectedValue = Session.Item("LastEPurseCard")
                                End If
                                selected = True
                            Else
                                If dr("CustomerNo").ToString.TrimStart("0") = Profile.User.Details.LoginID.ToString.TrimStart("0") Then
                                    If (Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowedInAgentMode")) Or
                                        (Not Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowed")) Then
                                        Me.txtEPurseCard.Text = li
                                    Else
                                        Me.ddlSelectCard.SelectedValue = li
                                    End If
                                    selected = True
                                End If
                            End If
                        End If
                    End If
                Next

                ' Retrieve the balance automatically if a card has been selected. 
                If selected Then
                    'This call needed to be seperate from WS174R for caching reasons
                    RetrieveBalance(True)
                    If Me.plhEPFirst.Visible Then
                        If (Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowedInAgentMode")) Or
                                    (Not Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowed")) Then
                            Me.txtEPurseCard.Text = ""
                        End If
                    End If
                End If


            Else
                'Display no card message on error when manual input is disabled
                If Me.plhDDLSelect.Visible Then
                    ErrorList.Items.Add(ucr.Content("NoEPurseCards", _languageCode, True))
                    Me.plhEPFirst.Visible = False
                End If
            End If


        Catch ex As Exception
            log.ExceptionLog("EPurse.ascx-SetFields", ex.Message)
        End Try

        Session.Remove("LastEPurseCard")

    End Sub

    Protected Sub SetValidators()
        With ucr
            If (Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowedInAgentMode")) Or
                        (Not Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowed")) Then
                Me.rfvEPurseCard.Enabled = True
                Me.rfvEPurseCard.ErrorMessage = .Content("NoEPurseCardErrorMessage", _languageCode, True)
                Me.regExEPurseCard.Enabled = True
                Me.regExEPurseCard.ErrorMessage = .Content("InvalidEPurseCardErrorMessage", _languageCode, True)
                Me.regExEPurseCard.ValidationExpression = .Attribute("EPurseRegularExpression")
            Else
                Me.rfvEPurseCard.Enabled = False
                Me.regExEPurseCard.Enabled = False
            End If

            Me.rfvPaymentAmount.Enabled = True
            Me.rfvPaymentAmount.ErrorMessage = .Content("NoPartPaymentErrorMessage", _languageCode, True)
            Me.regExPaymentAmount.Enabled = True
            Me.regExPaymentAmount.ErrorMessage = .Content("InvalidPaymentAmountErrorMessage", _languageCode, True)
            Me.regExPaymentAmount.ValidationExpression = .Attribute("PartPaymentAmountRegularExpression")

        End With

    End Sub

    Protected Sub btnNextCard_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNextCard.Click
        If _isGiftCard Then
            Me.plhGiftCard.Visible = True
            Me.plhEPSecond.Visible = False

            txtGiftCard.Enabled = True
            txtPin.Enabled = True

            ErrorList.Items.Clear()
            SuccessList.Items.Clear()
            txtGiftCard.Text = String.Empty
            txtPin.Text = String.Empty
            _ePurseOptionsChanged = True
        Else
            Me.plhEPFirst.Visible = True
            Me.plhEPSecond.Visible = False
            Me.txtEPurseCard.Text = ""
            ErrorList.Items.Clear()
            SuccessList.Items.Clear()
            _ePurseOptionsChanged = True
        End If
       
    End Sub

    Protected Sub rptSmartcards_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptSmartcards.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            CType(e.Item.FindControl("lbtnSmartcardListItem"), LinkButton).Text = e.Item.DataItem("CardNumber").ToString.TrimStart("0")
            CType(e.Item.FindControl("lbtnSmartcardListItem"), LinkButton).OnClientClick = "ePursePopulateTextBox('" & e.Item.DataItem("CardNumber").ToString.TrimStart("0") & "');return false;"
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub RetrieveBalance(ByVal pageLoad As Boolean)
        ErrorList.Items.Clear()
        Try
            '
            ' Retrieve the total credit on the e-purse
            Dim err As New Talent.Common.ErrorObj
            Dim tp As New Talent.Common.TalentPayment
            Dim dePayment As New Talent.Common.DEPayments
            With dePayment
                If _isGiftCard Then
                    .CardNumber = txtGiftCard.Text
                    .IsGiftCard = _isGiftCard
                    .PIN = txtPin.Text
                    .Currency = TDataObjects.PaymentSettings.GetCurrencyCode(ucr.BusinessUnit, ucr.PartnerCode)
                Else
                    If (Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowedInAgentMode")) Or
                                           (Not Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowed")) Then
                        .CardNumber = Me.txtEPurseCard.Text
                    Else
                        .CardNumber = Me.ddlSelectCard.SelectedItem.Text
                    End If
                End If
                If Not Profile.User.Details.LoginID Is Nothing Then
                    .CustomerNumber = Profile.User.Details.LoginID
                End If
            End With
            '
            tp.De = dePayment
            tp.Settings = Utilities.GetSettingsObject()
            err = tp.RetrieveEPurseTotal()
            '
            ' Was the call successful
            If err.HasError OrElse tp.ResultDataSet.Tables.Count <> 2 Then
                ErrorList.Items.Add(errMsg.GetErrorMessage("XX").ERROR_MESSAGE)
            ElseIf tp.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                ErrorList.Items.Add(errMsg.GetErrorMessage(tp.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode")).ERROR_MESSAGE)
            Else
                Dim dt As DataTable = tp.ResultDataSet.Tables("EPurse")
                If dt.Rows(0).Item("EPurseValue").ToString.Trim <> "0.00" OrElse Not pageLoad Then
                    If (Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowedInAgentMode")) Or
                            (Not Utilities.IsAgent AndAlso ucr.Attribute("CardEntryAllowed")) Then
                        Me.ltlEPurseCardValue.Text = Me.txtEPurseCard.Text
                    Else
                        Me.ltlEPurseCardValue.Text = Me.ddlSelectCard.SelectedItem.Text
                    End If
                    Me.ltlTotalAvailableValue.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(CType(dt.Rows(0).Item("EPurseValue"), Decimal), 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                    Me.hdfTotalAvailableValue.Value = dt.Rows(0).Item("EPurseValue").ToString

                    'Teamcard ePurse works in points but this will not be relevant for other suppliers
                    If CType(dt.Rows(0).Item("EPursePoints"), Integer) > 0 Then
                        Me.ltlTotalPointsValue.Text = dt.Rows(0).Item("EPursePoints").ToString
                        Me.ltlTotalPointsValue.Visible = True
                        Me.ltlTotalPoints.Visible = True
                    Else
                        Me.ltlTotalPointsValue.Visible = False
                        Me.ltlTotalPoints.Visible = False
                    End If
                    Me.hdfEPursePWValue.Value = dt.Rows(0).Item("EPursePWValue").ToString
                    Me.hdfEPursePWPoints.Value = dt.Rows(0).Item("EPursePWPoints").ToString
                    Me.plhEPFirst.Visible = False
                    Me.plhEPSecond.Visible = True

                    'Put the total amount available or the basket total amount in the payment text box, which ever is less
                    If Convert.ToDecimal(hdfTotalAvailableValue.Value) < _basketTotalWithFees Then
                        txtPaymentAmount.Text = Convert.ToDecimal(hdfTotalAvailableValue.Value).ToString("N2")
                    Else
                        txtPaymentAmount.Text = _basketTotalWithFees.ToString("N2")
                    End If

                    If ModuleDefaults.EPurseTopUpProductCode.Length > 0 AndAlso CDec(txtPaymentAmount.Text) = CDec(0) Then
                        plhEPSecond.Visible = False
                        plhNoBalance.Visible = True
                        ltlNoBalance.Text = ucr.Content("NoBalanceText", _languageCode, True)
                        hplNoBalance.Text = ucr.Content("NoBalanceLink", _languageCode, True)
                        hplNoBalance.NavigateUrl = "~/PagesLogin/Smartcard/EPurse.aspx"
                    Else
                        plhNoBalance.Visible = False
                        If Me.TakingPayment Then
                            'Output a message if you need to pay in multiples of 0.xx
                            If CType(Me.hdfEPursePWValue.Value, Decimal) > 0.01 Then
                                Dim pointsWorth As Decimal = CType(Me.hdfEPursePWValue.Value, Decimal)

                                'Output a message if you need to pay in multiples of 0.xx
                                Dim msg As String = ucr.Content("MutiplesOfMessage", _languageCode, True)
                                msg = msg.Replace("<<<multiples>>>", pointsWorth.ToString)
                                SuccessList.Items.Add(msg)
                                If Not Session.Item("PartPaymentAmount") Is Nothing Then
                                    _basketTotal -= CType(Session.Item("PartPaymentAmount"), Decimal)
                                    Session.Remove("PartPaymentAmount")
                                End If

                                Dim basketPoints As Integer = Math.Ceiling(Decimal.Divide(_basketTotal, pointsWorth))
                                If Decimal.Remainder(_basketTotal, pointsWorth) > 0 Then
                                    msg = ucr.Content("OverPaymentMessage", _languageCode, True)
                                    msg = msg.Replace("<<<basketPoints>>>", basketPoints.ToString)
                                    msg = msg.Replace("<<<overPayment>>>", CType(basketPoints * pointsWorth, String))
                                    SuccessList.Items.Add(msg)
                                End If

                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            ErrorList.Items.Add(errMsg.GetErrorMessage("XX").ERROR_MESSAGE)
            log.ExceptionLog("EPurse.ascx-RetrieveBalance", ex.Message)
        End Try
    End Sub
    Private Sub RetrieveGiftCardBalance()
        ErrorList.Items.Clear()
        Try
            '
            ' Retrieve the total credit on the e-purse
            Dim err As New Talent.Common.ErrorObj
            Dim tp As New Talent.Common.TalentPayment
            Dim dePayment As New Talent.Common.DEPayments
            With dePayment
                .CardNumber = Me.txtEPurseCard.Text
                If Not Profile.User.Details Is Nothing Then
                    .CustomerNumber = Profile.User.Details.LoginID
                Else
                    .CustomerNumber = GlobalConstants.GENERIC_CUSTOMER_NUMBER
                End If
                If _isGiftCard Then
                    .CardNumber = txtGiftCard.Text
                    .IsGiftCard = _isGiftCard
                    .PIN = txtPin.Text
                    .Currency = TDataObjects.PaymentSettings.GetCurrencyCode(ucr.BusinessUnit, ucr.PartnerCode)
                End If
            End With
            '
            tp.De = dePayment
            tp.Settings = Utilities.GetSettingsObject()
            err = tp.RetrieveEPurseTotal()
            '
            ' Was the call successful
            If err.HasError OrElse tp.ResultDataSet.Tables.Count <> 2 Then
                ErrorList.Items.Add(errMsg.GetErrorMessage("XX").ERROR_MESSAGE)
            ElseIf tp.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                ErrorList.Items.Add(errMsg.GetErrorMessage(tp.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode")).ERROR_MESSAGE)
            Else
                Dim dt As DataTable = tp.ResultDataSet.Tables("EPurse")
                If dt.Rows(0).Item("EPurseValue").ToString.Trim <> "0.00" Then
                    Me.ltlEPurseCardValue.Text = Me.txtEPurseCard.Text
                    Me.ltlTotalAvailableValue.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(CType(dt.Rows(0).Item("EPurseValue"), Decimal), 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                    Me.hdfTotalAvailableValue.Value = dt.Rows(0).Item("EPurseValue").ToString


                    Me.ltlTotalPointsValue.Visible = False
                    Me.ltlTotalPoints.Visible = False

                    Me.hdfEPursePWValue.Value = dt.Rows(0).Item("EPursePWValue").ToString

                    Me.plhEPFirst.Visible = False
                    Me.plhEPSecond.Visible = True

                    txtGiftCard.Enabled = False
                    txtPin.Enabled = False
                    'Put the total amount available or the basket total amount in the payment text box, which ever is less
                    If Convert.ToDecimal(hdfTotalAvailableValue.Value) < _basketTotalWithFees Then
                        txtPaymentAmount.Text = Convert.ToDecimal(hdfTotalAvailableValue.Value).ToString("N2")
                    Else
                        txtPaymentAmount.Text = _basketTotalWithFees.ToString("N2")
                    End If
                    plhNoBalance.Visible = False
                Else
                    ltlNoBalance.Text = ucr.Content("NoBalanceText", _languageCode, True)
                    hplNoBalance.Text = ucr.Content("NoBalanceLink", _languageCode, True)
                    plhNoBalance.Visible = True
                    plhEPSecond.Visible = False
                End If
            End If
        Catch ex As Exception
            ErrorList.Items.Add(errMsg.GetErrorMessage("XX").ERROR_MESSAGE)
            log.ExceptionLog("EPurse.ascx-RetrieveBalance", ex.Message)
        End Try
    End Sub
#End Region

#Region "Public Functions"

    Public Function TakePartPayment() As Boolean
        Dim partPaymentTaken As Boolean = False
        ErrorList.Items.Clear()
        SuccessList.Items.Clear()
        Try
            If ValidatePartPayment() Then
                '
                ' Take part payment
                Dim err As New Talent.Common.ErrorObj
                Dim tp As New Talent.Common.TalentPayment
                Dim dePayment As New Talent.Common.DEPayments
                With dePayment
                    .SessionId = Profile.Basket.Basket_Header_ID
                    .CardNumber = Me.ltlEPurseCardValue.Text
                    .Amount = formatDecimalAmmount(txtPaymentAmount.Text)
                    If Not Profile.User.Details Is Nothing Then
                        .CustomerNumber = Profile.User.Details.LoginID
                    End If
                    If _isGiftCard Then
                        .CardNumber = txtGiftCard.Text
                        .IsGiftCard = _isGiftCard
                        .PIN = txtPin.Text
                        .Currency = TDataObjects.PaymentSettings.GetCurrencyCode(ucr.BusinessUnit, ucr.PartnerCode)
                    End If
                    .PaymentType = GlobalConstants.EPURSEPAYMENTTYPE
                End With
                tp.De = dePayment
                tp.Settings = Utilities.GetSettingsObject()
                tp.Settings.Cacheing = ucr.Attribute("PaymentCacheing")
                tp.Settings.CacheTimeMinutes = ucr.Attribute("PaymentCacheTimeMinutes")
                err = tp.TakePartPayment()
                '
                ' Was the call successful
                If err.HasError OrElse tp.ResultDataSet Is Nothing OrElse tp.ResultDataSet.Tables.Count = 0 Then
                    ErrorList.Items.Add(errMsg.GetErrorMessage("XX").ERROR_MESSAGE)
                ElseIf tp.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                    ErrorList.Items.Add(errMsg.GetErrorMessage(tp.ResultDataSet.Tables("StatusResults").Rows(0).Item("ReturnCode")).ERROR_MESSAGE)
                Else
                    Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)

                    'Redirect to load the page again and to prevent an F5 refresh submitting the same request
                    Session.Add("LastEPurseCard", Me.ltlEPurseCardValue.Text)
                    Session.Add("SelectPaymentTypeOption", "EP")
                    Session.Add("PartPaymentSuccess", "True")
                    Session.Add("PartPaymentAmount", Me.txtPaymentAmount.Text)
                End If
            End If
        Catch ex As Exception
            ErrorList.Items.Add(errMsg.GetErrorMessage("XX").ERROR_MESSAGE)
            log.ExceptionLog("EPurse.ascx-TakePartPayment", ex.Message)
        End Try

        If ErrorList.Items.Count = 0 Then partPaymentTaken = True
        Return partPaymentTaken
    End Function

    Public Function ValidateUserInput() As Boolean
        Dim err As Boolean = False
        Try
            ErrorList.Items.Clear()

            'Execute the page validation
            If Not Page.IsValid Then
                err = True
                Exit Try
            End If

            'The second screen needs to be displayed before the customer can complete
            If String.IsNullOrEmpty(Me.ltlEPurseCardValue.Text) AndAlso Not _isGiftCard Then
                ErrorList.Items.Add(ucr.Content("NoEPurseCardsToUse", _languageCode, True))
                err = True
                Exit Try
            End If

            'Is the part payment total greater than the basket total
            If CDec(txtPaymentAmount.Text) > _basketTotal Then
                ErrorList.Items.Add(ucr.Content("PartPaymentExceedsBasketErrorMessage", _languageCode, True))
                err = True
                Exit Try
            End If

            'Is what has been entered is greater than what's available
            If CDec(txtPaymentAmount.Text) > CDec(hdfTotalAvailableValue.Value) Then
                ErrorList.Items.Add(ucr.Content("InsufficientFundsErrorMessage", _languageCode, True))
                err = True
                Exit Try
            End If

        Catch ex As Exception
            err = True
        End Try

        _ePurseOptionsChanged = err
        Return Not err
    End Function

    Public Function ValidatePartPayment() As Boolean
        Dim err As Boolean = False
        Try
            ErrorList.Items.Clear()

            'Execute the page validation
            If Not Page.IsValid Then
                err = True
                Exit Try
            End If

            'The second screen needs to be displayed before the customer can take a part payment
            If String.IsNullOrEmpty(Me.ltlEPurseCardValue.Text) AndAlso Not _isGiftCard Then
                ErrorList.Items.Add(ucr.Content("NoEPurseCardsToUse", _languageCode, True))
                err = True
                Exit Try
            End If

            Dim paymentAmmount As Decimal = 0
            paymentAmmount = CDec(txtPaymentAmount.Text)

            'Is the part payment total greater than the basket total
            If _basketTotal <= paymentAmmount Then
                ErrorList.Items.Add(ucr.Content("PartPaymentExceedsBasketErrorMessage", _languageCode, True))
                err = True
                Exit Try
            End If

            'Is the part payment total greater than the card amount
            If CType(Me.hdfTotalAvailableValue.Value, Decimal) < paymentAmmount Then
                ErrorList.Items.Add(ucr.Content("PartPaymentExceedsCardErrorMessage", _languageCode, True))
                err = True
                Exit Try
            End If

            'Teamcard points - The value must round exactly to Teamcard points - not applicable to gift cards
            If Not _isGiftCard Then
                Dim ePursePWValue As Decimal = CType(Me.hdfEPursePWValue.Value, Decimal)
                Dim remainder As Decimal = Decimal.Remainder(paymentAmmount, ePursePWValue)
                If remainder > 0 Then
                    Dim errMsg As String = ucr.Content("InvalidPartPaymentErrorMessage", _languageCode, True)
                    errMsg = errMsg.Replace("<<<multiples>>>", Me.hdfEPursePWValue.Value)
                    errMsg = errMsg.Replace("<<<points>>>", Me.hdfEPursePWPoints.Value)
                    ErrorList.Items.Add(errMsg)
                    err = True
                    Exit Try
                End If
            End If


        Catch ex As Exception
            err = True
        End Try

        Return Not err
    End Function

#End Region

#Region "Private Functions"

    Private Function formatDecimalAmmount(ByVal paymentAmmount As String) As String
        Dim formattedValue As New StringBuilder
        Const FULLSTOP As String = "."
        If Not paymentAmmount.Contains(FULLSTOP) Then
            formattedValue.Append(paymentAmmount)
            formattedValue.Append("00")
            Return formattedValue.ToString
        Else
            paymentAmmount = paymentAmmount.Replace(FULLSTOP, String.Empty)
            Return paymentAmmount
        End If
    End Function

#End Region


End Class
