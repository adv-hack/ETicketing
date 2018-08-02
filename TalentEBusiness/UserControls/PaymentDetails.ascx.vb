Imports System.Collections.Generic
Imports System.Data
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_PaymentDetails
    Inherits ControlBase

#Region "Class Level Fields"

    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _display As Boolean = True
    Private _canSaveCardDetails As Boolean = False
    Private _errorMessages As New BulletedList
    Private _hideSecurityNumber As Boolean = False
    Private _noSavedCardDetails As Boolean = False
    Private _showFeeValueWithCard As Boolean = False
    Private _listOfFeesPerCard As Dictionary(Of String, String) = Nothing

#End Region

#Region "Public Properties"

    Public ReadOnly Property CardTypeDDL() As DropDownList
        Get
            Return CardSelector
        End Get
    End Property
    Public ReadOnly Property CardNumberBox() As System.Web.UI.WebControls.TextBox
        Get
            Return CardNumber
        End Get
    End Property
    Public ReadOnly Property StartMonthDDL() As DropDownList
        Get
            Return StartMonth
        End Get
    End Property
    Public ReadOnly Property StartYearDDL() As DropDownList
        Get
            Return StartYear
        End Get
    End Property
    Public ReadOnly Property ExpiryMonthDDL() As DropDownList
        Get
            Return ExpiryMonth
        End Get
    End Property
    Public ReadOnly Property ExpiryYearDDL() As DropDownList
        Get
            Return ExpiryYear
        End Get
    End Property
    Public ReadOnly Property IssueNumberBox() As System.Web.UI.WebControls.TextBox
        Get
            Return IssueNumber
        End Get
    End Property
    Public ReadOnly Property SecurityNumberBox() As System.Web.UI.WebControls.TextBox
        Get
            Return SecurityNumber
        End Get
    End Property
    Public ReadOnly Property CardHolderNameBox() As System.Web.UI.WebControls.TextBox
        Get
            Return CardHolderName
        End Get
    End Property
    Public ReadOnly Property SetAsDefault() As Boolean
        Get
            Return chkSetAsDefault.Checked
        End Get
    End Property
    Public ReadOnly Property InstallmentsBox() As System.Web.UI.WebControls.TextBox
        Get
            Return txtInstallments
        End Get
    End Property
    Public ReadOnly Property InstallmentsDDL() As System.Web.UI.WebControls.DropDownList
        Get
            Return ddlInstallments
        End Get
    End Property
    Public ReadOnly Property SaveTheseCardDetails As Boolean
        Get
            Return chkSaveTheseCardDetails.Checked
        End Get
    End Property
    Public Property Display() As Boolean
        Get
            Return _display
        End Get
        Set(ByVal value As Boolean)
            _display = value
        End Set
    End Property
    Public Property CanSaveCardDetails As Boolean
        Get
            Return _canSaveCardDetails
        End Get
        Set(ByVal value As Boolean)
            _canSaveCardDetails = value
        End Set
    End Property
    Public ReadOnly Property ErrorMessages() As BulletedList
        Get
            Return _errorMessages
        End Get
    End Property
    Public Property HideSecurityNumber() As Boolean
        Get
            Return _hideSecurityNumber
        End Get
        Set(ByVal value As Boolean)
            _hideSecurityNumber = value
        End Set
    End Property
    Public Property NoSavedCardDetails() As Boolean
        Get
            Return _noSavedCardDetails
        End Get
        Set(ByVal value As Boolean)
            _noSavedCardDetails = value
        End Set
    End Property
    Public Property InstallmentsPostBack() As Boolean

    Public Property CardTypeSelectionChanged() As Boolean = False

#End Region

#Region "Protected Sub Routines"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PaymentDetails.ascx"
        End With
        _listOfFeesPerCard = New Dictionary(Of String, String)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Display Then
            If (StartYear.Items.Count <= 0) OrElse (StartMonth.Items.Count <= 0) OrElse (ExpiryMonth.Items.Count <= 0) OrElse (ExpiryYear.Items.Count <= 0) OrElse (CardSelector.Items.Count <= 0) Then
                If Not Page.IsPostBack Then
                    SetCardDDL()
                    SetDateDDLs()
                End If
            End If
            SetLabelText()
            SetValidators()
            If Utilities.GetCurrentPageName().ToUpper() = "AMENDPPSENROLMENT.ASPX" OrElse Utilities.GetCurrentPageName().ToUpper() = "AMENDPPSPAYMENTS.ASPX" Then SetCardRequirements()
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Session("DeliverySelectionRequiresResetCCForm") IsNot Nothing AndAlso Utilities.CheckForDBNull_Boolean_DefaultFalse(Session("DeliverySelectionRequiresResetCCForm")) Then
            Session.Remove("DeliverySelectionRequiresResetCCForm")
            ResetCCForm()
        End If
        SetFields()
        SetCardRequirements()
    End Sub

    Protected Sub CardNumber_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CardNumber.TextChanged
        InstallmentsPostBack = True
        Dim cardNumberEntered As String = CardNumber.Text
        If cardNumberEntered.Length > 6 Then cardNumberEntered = cardNumberEntered.Substring(0, 6)
        Dim installments As List(Of Integer) = TDataObjects.PaymentSettings.TblCreditCardTypeControl.GetInstallmentsByCard(cardNumberEntered)

        If installments.Count = 1 Then
            plhInstallments.Visible = True
            txtInstallments.Visible = True
            ddlInstallments.Visible = False
            lblInstallments.Visible = True
            lblInstallments.Text = _ucr.Content("MaxInstallmentsLabel", _languageCode, True).Replace("<<MAX_INSTALLMENTS>>", installments(0))
            rfvInstallments.Enabled = False
        ElseIf installments.Count > 1 Then
            plhInstallments.Visible = True
            txtInstallments.Visible = False
            ddlInstallments.Visible = True
            ddlInstallments.Items.Clear()
            For Each item As String In installments
                ddlInstallments.Items.Add(item)
            Next
            lblInstallments.Visible = False
            Dim installmentsListItem As New ListItem
            installmentsListItem.Value = String.Empty
            installmentsListItem.Text = _ucr.Content("InstallmentsPleaseSelect", _languageCode, True)
            ddlInstallments.Items.Insert(0, installmentsListItem)
            rfvInstallments.Enabled = True
            rfvInstallments.ErrorMessage = _ucr.Content("PleaseSelectAndInstallmentOption", _languageCode, True)
        Else
            plhInstallments.Visible = False
            rfvInstallments.Enabled = False
        End If

        ltlInstallmentsIntro.Text = _ucr.Content("InstallmentsExplaination", _languageCode, True)
        plhInstallmentsIntro.Visible = (ltlInstallmentsIntro.Text.Length > 0)
        InstallmentsLabel.Text = _ucr.Content("InstallmentsLabel", _languageCode, True)
    End Sub

    Protected Sub SetDateDDLs()

        '-----------------------------
        '   Set Month Drop Downs
        '-----------------------------
        Dim lic As New ListItemCollection
        Dim li As New ListItem(GlobalConstants.CARD_DDL_INITIAL_VALUE, GlobalConstants.CARD_DDL_INITIAL_VALUE)
        lic.Add(li)
        For i As Integer = 1 To 12
            li = New ListItem(i, i)
            lic.Add(li)
        Next

        ExpiryMonth.DataSource = lic
        ExpiryMonth.DataBind()

        StartMonth.DataSource = lic
        StartMonth.DataBind()
        '-----------------------------

        '-----------------------------
        '   Set Expiry Year Drop Down
        '-----------------------------
        lic = New ListItemCollection
        li = New ListItem(GlobalConstants.CARD_DDL_INITIAL_VALUE, GlobalConstants.CARD_DDL_INITIAL_VALUE)
        lic.Add(li)
        Dim count As Integer = Now.Year
        While count <= Now.Year + 20
            li = New ListItem(count, count)
            lic.Add(li)
            count += 1
        End While

        ExpiryYear.DataSource = lic
        ExpiryYear.DataBind()
        '-----------------------------


        '-----------------------------
        '   Set Start Year Drop Down
        '-----------------------------
        lic = New ListItemCollection
        li = New ListItem(GlobalConstants.CARD_DDL_INITIAL_VALUE, GlobalConstants.CARD_DDL_INITIAL_VALUE)
        lic.Add(li)
        count = Now.Year
        While count >= Now.Year - 20
            li = New ListItem(count, count)
            lic.Add(li)
            count -= 1
        End While

        StartYear.DataSource = lic
        StartYear.DataBind()
        '-----------------------------
    End Sub

    Protected Sub SetCardDDL()
        SetCardDDLFlags()
        If Session("PPSPayment") IsNot Nothing AndAlso Session("PPSPayment") = True Then
            CardSelector.DataSource = TalentCache.GetDropDownControlText(TEBUtilities.GetCurrentLanguageForDDLPopulation, "PPS_PAYMENT", "CARD")
        Else
            CardSelector.DataSource = TalentCache.GetDropDownControlText(TEBUtilities.GetCurrentLanguageForDDLPopulation, "PAYMENT", "CARD")
        End If
        CardSelector.DataTextField = "Text"
        CardSelector.DataValueField = "Value"
        CardSelector.DataBind()
        _listOfFeesPerCard.Clear()
        If Utilities.GetCurrentPageName.ToUpper.Contains("CHECKOUT") AndAlso CardSelector.Items.Count > 0 Then
            For itemIndex As Integer = 0 To CardSelector.Items.Count - 1
                CardSelector.Items(itemIndex).Text = GetCardDisplayText(CardSelector.Items(itemIndex).Value, CardSelector.Items(itemIndex).Text)
            Next
            CardSelector.AutoPostBack = (_listOfFeesPerCard.Count > 0)
        Else
            CardSelector.AutoPostBack = False
        End If
        SetBasketCardTypeCode()
    End Sub

    Protected Sub SetLabelText()
        With _ucr
            CardSelectionLabel.Text = .Content("CardSelectLabel", _languageCode, True)
            CardNumberLabel.Text = .Content("CardNumberLabel", _languageCode, True)
            ExpiryDateLabel.Text = .Content("ExpiryDateLabel", _languageCode, True)
            StartDateLabel.Text = .Content("StartDateLabel", _languageCode, True)
            IssueNumberLabel.Text = .Content("IssueNumberLabel", _languageCode, True)
            SecurityNumberLabel.Text = .Content("SecurityNumberLabel", _languageCode, True)
            plhInstallments.Visible = False
            If CType(.Attribute("displayInstallments"), Boolean) Then
                Dim totalAmount As Decimal = Profile.Basket.BasketSummary.TotalBasket
                Dim minInstallmentsVal As Decimal = CDec(_ucr.Attribute("minimumInstallmentValue"))
                If totalAmount < minInstallmentsVal Then
                    CardNumber.AutoPostBack = False
                Else
                    CardNumber.AutoPostBack = True
                End If
            Else
                CardNumber.AutoPostBack = False
            End If
            If CType(.Attribute("displayCardHolderName"), Boolean) Then
                CardHolderNameLabel.Text = .Content("CardHolderNameLabel", _languageCode, True)
                plhCardHolderName.Visible = True
            Else
                plhCardHolderName.Visible = False
            End If
            If CType(.Attribute("displaySetAsDefault"), Boolean) Then
                lblSetAsDefault.Text = .Content("lblSetAsDefault", _languageCode, True)
                plhSetAsDefault.Visible = True
            Else
                plhSetAsDefault.Visible = False
            End If
        End With
    End Sub

    Protected Sub SetValidators()
        With _ucr
            rfvCardType.ErrorMessage = .Content("ErrorCardTypeNotSelected", _languageCode, True)
            CardNumberRFV.ErrorMessage = .Content("NoCardNumberErrorMessage", _languageCode, True)
            CardNumberRegEx.ErrorMessage = .Content("ErrorWithCardNumber", _languageCode, True)
            CardNumberRegEx.ValidationExpression = .Content("CardNumberRegularExpression", _languageCode, True)

            rfvCardValidToMM.ErrorMessage = .Content("ErrorExpiryMonthNotSelected", _languageCode, True)
            rfvCardValidToYYYY.ErrorMessage = .Content("ErrorExpiryYearNotSelected", _languageCode, True)

            IssueNumberRFV.ErrorMessage = .Content("NoIssueNumberErrorMessage", _languageCode, True)
            IssueNumberRegEx.ErrorMessage = .Content("InvalidIssueNumberErrorMessage", _languageCode, True)
            IssueNumberRegEx.ValidationExpression = .Content("IssueNumberRegularExpression", _languageCode, True)

            SecurityNumberRFV.ErrorMessage = .Content("NoSecurityNumberErrorMessage", _languageCode, True)
            SecurityNumberRegEx.ErrorMessage = .Content("InvalidSecurityNumberErrorMessage", _languageCode, True)
            SecurityNumberRegEx.ValidationExpression = .Content("SecurityNumberRegularExpression", _languageCode, True)

            CardHolderNameRFV.Enabled = False
            CardHolderNameRFV.Visible = False
            If CType(.Attribute("displayCardHolderName"), Boolean) Then
                If Not .Content("NoCardHolderNameErrorMessage", _languageCode, True) = String.Empty Then
                    CardHolderNameRFV.ErrorMessage = .Content("NoCardHolderNameErrorMessage", _languageCode, True)
                    CardHolderNameRFV.Enabled = True
                    CardHolderNameRFV.Visible = True
                End If
            End If

        End With
    End Sub

    Protected Sub SetCardRequirements()
        'IssueNumber
        If Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("IssueNumberVisible")) Then
            IssueNumberLabel.Text = _ucr.Content("IssueNumberLabel", _languageCode, True)
            If Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("IssueNumberEnableRFV")) Then
                IssueNumberRFV.Enabled = True
                IssueNumberRegEx.Enabled = True
                IssueNumberLabel.Visible = True
            Else
                IssueNumberRFV.Enabled = False
                IssueNumberRegEx.Enabled = False
                IssueNumberLabel.Visible = True
                If IssueNumberLabel.Text.EndsWith("*") Then IssueNumberLabel.Text = IssueNumberLabel.Text.TrimEnd("*")
            End If
        Else
            plhIssueNumber.Visible = False
            IssueNumberRFV.Enabled = False
            IssueNumberRegEx.Enabled = False
            IssueNumberLabel.Visible = False
            IssueNumber.Visible = False
        End If

        'Security Number
        If Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("SecurityNumberVisible")) AndAlso Not _hideSecurityNumber AndAlso CATHelper.IsBasketNotInCancelMode Then
            plhSecurityNumber.Visible = True
            SecurityNumber.Visible = True
            SecurityImage.Visible = True
            SecurityNumberLabel.Text = _ucr.Content("SecurityNumberLabel", _languageCode, True)
            If Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("SecurityNumberEnableRFV")) Then
                SecurityNumberRFV.Enabled = True
                SecurityNumberRegEx.Enabled = True
                SecurityNumberLabel.Visible = True
            Else
                SecurityNumberRFV.Enabled = False
                SecurityNumberRegEx.Enabled = True
                SecurityNumberLabel.Visible = True
                If SecurityNumberLabel.Text.EndsWith("*") Then SecurityNumberLabel.Text = SecurityNumberLabel.Text.TrimEnd("*")
            End If
        Else
            plhSecurityNumber.Visible = False
            SecurityNumberRFV.Enabled = False
            SecurityNumberRegEx.Enabled = False
            SecurityNumberLabel.Visible = False
            SecurityNumber.Visible = False
            SecurityImage.Visible = False
        End If

        'Start Date
        Dim useStart As Boolean = False
        If useStart Then
            StartDateLabel.Text = _ucr.Content("StartDateLabel", _languageCode, True)
        Else
            If StartDateLabel.Text.EndsWith("*") Then StartDateLabel.Text = StartDateLabel.Text.TrimEnd("*")
        End If

    End Sub

    Protected Sub SetFields()
        Try
            'Add any refferal url's which you'd like to enable preloading card details on below...
            Dim tryPrePopulate As Boolean = False
            Dim allowedRefs As ArrayList = New ArrayList()
            allowedRefs.Add("checkout3dsecure.aspx")
            allowedRefs.Add("checkout.aspx")
            For Each allowedRef As String In allowedRefs
                If Request.UrlReferrer IsNot Nothing AndAlso Request.UrlReferrer.ToString.ToLower.IndexOf(allowedRef) > -1 Then tryPrePopulate = True
            Next
            If tryPrePopulate Then
                'Can we pre-populate this control
                If Not Checkout.RetrievePaymentItem("PaymentType", UCase(Utilities.GetCurrentPageName.Trim), True, True, "CC").Trim = "" Then
                    'Text Boxes 
                    CardNumber.Text = Checkout.RetrievePaymentItem("CardNumber", Utilities.GetCurrentPageName.Trim, True, True, "CC")
                    IssueNumber.Text = Checkout.RetrievePaymentItem("IssueNumber", Utilities.GetCurrentPageName.Trim, True, True, "CC")
                    SecurityNumber.Text = Checkout.RetrievePaymentItem("CV2Number", Utilities.GetCurrentPageName.Trim, True, True, "CC")

                    'Drop Down Lists - Card Type
                    Dim value As String = Checkout.RetrievePaymentItem("CardType", Utilities.GetCurrentPageName.Trim, True, True, "CC")
                    If Not value.Trim.Equals("") Then
                        CardSelector.SelectedValue = value
                    End If

                    'Expiry Date
                    value = Checkout.RetrievePaymentItem("ExpiryDate", Utilities.GetCurrentPageName.Trim, True, True, "CC")
                    If Not value.Trim.Equals("") AndAlso _
                        Not value.Trim.Equals("0000") AndAlso _
                        value.Length = 4 Then
                        ExpiryMonth.SelectedValue = value.Substring(0, 2).TrimStart("0")
                        ExpiryYear.SelectedValue = "20" & value.Substring(2, 2)
                    End If

                    'Start Date
                    value = Checkout.RetrievePaymentItem("StartDate", Utilities.GetCurrentPageName.Trim, True, True, "CC")
                    If Not value.Trim.Equals("") AndAlso _
                        Not value.Trim.Equals("0000") AndAlso _
                        value.Length = 4 Then
                        StartMonth.SelectedValue = value.Substring(0, 2).TrimStart("0")
                        If CType(value.Substring(2, 2), Integer) > 80 Then
                            StartYear.SelectedValue = "19" & value.Substring(2, 2)
                        Else
                            StartYear.SelectedValue = "20" & value.Substring(2, 2)
                        End If
                    End If
                End If
            End If

            'Add autocomplte regardless of pre-poulate
            Dim ackey As String = "autocomplete"
            Dim acvalue As String = "off"
            If CardNumber.Attributes.Item("autocomplete") <> "off" Then
                CardNumber.Attributes.Add(ackey, acvalue)
                IssueNumber.Attributes.Add(ackey, acvalue)
                SecurityNumber.Attributes.Add(ackey, acvalue)
                For Each ctrl As System.Web.UI.Control In Page.Master.Controls
                    If TypeOf ctrl Is HtmlForm Then
                        CType(ctrl, HtmlForm).Attributes.Add(ackey, acvalue)
                    End If
                Next
            End If

            If Not chkSetAsDefault.Visible Then
                chkSetAsDefault.Enabled = False
                chkSetAsDefault.Checked = False
            ElseIf _noSavedCardDetails Then
                chkSetAsDefault.Enabled = False
                chkSetAsDefault.Checked = True
            Else
                chkSetAsDefault.Enabled = True
                chkSetAsDefault.Checked = False
            End If

            plhSaveTheseCardDetails.Visible = False
            If (Not Profile.IsAnonymous) AndAlso (ModuleDefaults.UseSaveMyCard And _ucr.PageCode.ToLower.Equals("checkout.aspx")) Then
                If Profile.Basket.BasketContentType <> GlobalConstants.MERCHANDISEBASKETCONTENTTYPE OrElse Utilities.IsTicketingDBforRetailOrders Then
                    Dim err As New Talent.Common.ErrorObj
                    Dim payment As New Talent.Common.TalentPayment
                    Dim dePayment As New Talent.Common.DEPayments
                    Dim userHasSavedCards As Boolean = False
                    If ModuleDefaults.PaymentGatewayType = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
                        dePayment.PaymentType = GlobalConstants.PAYMENTTYPE_VANGUARD
                    End If
                    dePayment.SessionId = Profile.Basket.Basket_Header_ID
                    dePayment.CustomerNumber = Profile.UserName
                    dePayment.Source = GlobalConstants.SOURCE
                    payment.De = dePayment
                    payment.Settings = Utilities.GetSettingsObject
                    payment.Settings.CacheTimeMinutes = CType(_ucr.Attribute("MySavedCardsCacheTime"), Integer)
                    payment.Settings.Cacheing = Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("MySavedCardsCacheing"))
                    err = payment.RetrieveMySavedCards
                    If Not err.HasError Then
                        If payment.ResultDataSet.Tables.Count > 1 Then
                            If payment.ResultDataSet.Tables("CardDetails").Rows.Count < 9 Then
                                If Session("basketPPS1List") IsNot Nothing OrElse Session("basketPPS2List") IsNot Nothing Then
                                    If Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") = True Then
                                        plhSaveTheseCardDetails.Visible = True
                                        lblSaveTheseCardDetails.Text = _ucr.Content("SaveThisCardText", _languageCode, True)
                                    Else
                                        plhSaveTheseCardDetails.Visible = False
                                    End If
                                Else
                                    plhSaveTheseCardDetails.Visible = True
                                    lblSaveTheseCardDetails.Text = _ucr.Content("SaveThisCardText", _languageCode, True)
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
        End Try

    End Sub

#End Region

#Region "Public Functions"

    Public Function ValidateUserInput(Optional ByVal saveMyCardPage As Boolean = False) As Boolean
        Dim err As Boolean = False

        _errorMessages.Items.Clear()
        If String.IsNullOrEmpty(CardNumber.Text) Then
            _errorMessages.Items.Add(_ucr.Content("NoCardNumberErrorMessage", _languageCode, True))
            err = True
        End If
        If Not saveMyCardPage AndAlso IssueNumberRFV.Enabled AndAlso String.IsNullOrEmpty(IssueNumber.Text) Then
            _errorMessages.Items.Add(_ucr.Content("NoIssueNumberErrorMessage", _languageCode, True))
            err = True
        End If
        If Not saveMyCardPage AndAlso SecurityNumberRFV.Enabled AndAlso String.IsNullOrEmpty(SecurityNumber.Text) Then
            _errorMessages.Items.Add(_ucr.Content("NoSecurityNumberErrorMessage", _languageCode, True))
            err = True
        End If
        If CardTypeDDL.SelectedIndex = 0 Then
            _errorMessages.Items.Add(_ucr.Content("ErrorCardTypeNotSelected", _languageCode, True))
            err = True
        End If
        If Talent.Common.Utilities.CheckCardNumber(CardNumber.Text) Then

            'Validate the card Type
            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("ValidateCardType")) AndAlso _
                Not (Talent.Common.Utilities.CheckCardType(CardNumber.Text, CardTypeDDL.SelectedValue, ConfigurationManager.ConnectionStrings("SqlServer2005").ConnectionString, 0)) Then
                _errorMessages.Items.Add(_ucr.Content("ErrorWithCardType", _languageCode, True))
                err = True
            Else
                Dim dt As New TalentApplicationVariables.tbl_creditcardDataTable
                Dim useStart As Boolean = False
                dt = CType(Cache.Item(TalentCache.GetBusinessUnit & TalentCache.GetPartner(Profile) & "CreditCardSettings"), Data.DataTable)
                If dt Is Nothing OrElse dt.Rows.Count < 1 Then
                    Dim cc As New TalentApplicationVariablesTableAdapters.tbl_creditcardTableAdapter
                    dt = cc.Get_CreditCard_Settings
                    If dt.Rows.Count > 0 Then
                        TalentCache.AddPropertyToCache(TalentCache.GetBusinessUnit & TalentCache.GetPartner(Profile) & "CreditCardSettings", dt, 30, TimeSpan.Zero, CacheItemPriority.Normal)
                    End If
                End If

                For Each row As TalentApplicationVariables.tbl_creditcardRow In dt.Rows
                    If row("CARD_CODE") = CardSelector.SelectedValue Then
                        Try
                            useStart = row.USE_START_DATE
                            Exit For
                        Catch ex As Exception
                        End Try
                        Exit For
                    End If
                Next

                If useStart Then
                    If StartMonth.SelectedItem.Text = GlobalConstants.CARD_DDL_INITIAL_VALUE OrElse StartYear.SelectedItem.Text = GlobalConstants.CARD_DDL_INITIAL_VALUE Then
                        _errorMessages.Items.Add(_ucr.Content("ErrorStartDateNotSelected", _languageCode, True))
                        err = True
                    ElseIf CInt(StartMonth.SelectedValue) > Now.Month And CInt(StartYear.SelectedValue) = Now.Year Then
                        _errorMessages.Items.Add(_ucr.Content("ErrorStartDateIsInTheFuture", _languageCode, True))
                        err = True
                    End If
                End If
                If ExpiryMonth.SelectedItem.Text = GlobalConstants.CARD_DDL_INITIAL_VALUE OrElse ExpiryYear.SelectedItem.Text = GlobalConstants.CARD_DDL_INITIAL_VALUE Then
                    err = True
                    _errorMessages.Items.Add(_ucr.Content("ErrorExpiryDateNotSelected", _languageCode, True))
                ElseIf CInt(ExpiryMonth.SelectedValue) < Now.Month And CInt(ExpiryYear.SelectedValue) = Now.Year Then
                    err = True
                    _errorMessages.Items.Add(_ucr.Content("ErrorExpiryDateIsInThePast", _languageCode, True))
                End If
            End If


            ' Check the number of installments
            Dim installmentsErr As Boolean = False
            Dim enteredInstallments As Integer = 0
            If (txtInstallments.Text.Trim <> String.Empty AndAlso txtInstallments.Text.Trim <> "0") OrElse _
                (ddlInstallments.SelectedValue <> "0" AndAlso ddlInstallments.SelectedValue <> String.Empty) Then
                Dim maxInstallmentsList As List(Of Integer) = TDataObjects.PaymentSettings.TblCreditCardTypeControl.GetInstallmentsByCard(CardNumber.Text)
                Dim maxInstallments As Integer = 0
                Dim minInstallmentsVal As Decimal = CDec(_ucr.Attribute("minimumInstallmentValue"))
                Try
                    If maxInstallmentsList.Count = 1 Then
                        maxInstallments = maxInstallmentsList(0)
                        enteredInstallments = CInt(txtInstallments.Text)
                    ElseIf maxInstallmentsList.Count > 1 Then
                        enteredInstallments = ddlInstallments.SelectedValue
                        maxInstallments = enteredInstallments
                    End If
                    If enteredInstallments > maxInstallments Then
                        Throw New Exception
                    End If
                Catch ex As Exception
                    installmentsErr = True
                End Try
                If installmentsErr = True Then
                    Dim installmentErrMessage As String = _ucr.Content("ErrorInstalmentsNotValid", _languageCode, True)
                    installmentErrMessage = installmentErrMessage.Replace("<MaxInstallments>", maxInstallments.ToString)
                    installmentErrMessage = installmentErrMessage.Replace("<MinValue>", TDataObjects.PaymentSettings.FormatCurrency(minInstallmentsVal, _ucr.BusinessUnit, _ucr.PartnerCode))
                    err = True
                    _errorMessages.Items.Add(Server.HtmlDecode(installmentErrMessage))
                End If
            End If

        Else
            _errorMessages.Items.Add(_ucr.Content("ErrorWithCardNumber", _languageCode, True))
            err = True
        End If

        Return Not err
    End Function

    Public Sub ResetCCForm()
        _errorMessages.Items.Clear()
        SetCardDDL()
        SetDateDDLs()
        CardNumber.Text = String.Empty
        IssueNumber.Text = String.Empty
        SecurityNumber.Text = String.Empty
        CardHolderName.Text = String.Empty
        chkSetAsDefault.Checked = False
        txtInstallments.Text = String.Empty
    End Sub

#End Region

    Private Sub SetCardDDLFlags()
        If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE AndAlso Not Utilities.IsTicketingDBforRetailOrders Then
            CardSelector.AutoPostBack = False
        ElseIf Session("PPSPayment") IsNot Nothing AndAlso Session("PPSPayment") = True Then
            CardSelector.AutoPostBack = False
        ElseIf Profile.Basket.BasketSummary.TotalBasket <= 0 AndAlso Profile.Basket.CAT_MODE.Trim.Length > 0 Then
            CardSelector.AutoPostBack = False
        Else
            CardSelector.AutoPostBack = True
        End If
        If Profile.Basket.BasketContentType <> GlobalConstants.MERCHANDISEBASKETCONTENTTYPE OrElse Utilities.IsTicketingDBforRetailOrders Then
            If Session("PPSPayment") IsNot Nothing AndAlso Session("PPSPayment") = True Then
                _showFeeValueWithCard = False
            ElseIf Profile.Basket.BasketSummary.TotalBasket <= 0 AndAlso Profile.Basket.CAT_MODE.Trim.Length > 0 Then
                _showFeeValueWithCard = False
            Else
                _showFeeValueWithCard = True
            End If
        End If
    End Sub

    Private Sub SetBasketCardTypeCode()
        If Profile.Basket.CARD_TYPE_CODE IsNot Nothing AndAlso Profile.Basket.CARD_TYPE_CODE.Trim.Length > 0 Then
            For itemIndex As Integer = 0 To CardSelector.Items.Count - 1
                If (CardSelector.Items(itemIndex).Value.ToUpper = Profile.Basket.CARD_TYPE_CODE) Then
                    CardSelector.Items(itemIndex).Selected = True
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Function GetCardDisplayText(ByVal cardCode As String, ByVal cardDisplayText As String) As String
        Dim itemText As String = cardDisplayText
        If _showFeeValueWithCard Then
            itemText = _ucr.Content(cardCode, _languageCode, True)
            If String.IsNullOrWhiteSpace(itemText) Then
                itemText = cardDisplayText
            End If
            Dim cardFeeValue As String = GetCardCodeFeeValue(cardCode)
            itemText = itemText.Replace("<<CARD_CODE_TEXT>>", cardDisplayText)
            itemText = itemText.Replace("<<FEE_VALUE>>", cardFeeValue)
            itemText = itemText.Replace("<<FEE_VALUE_WITH_SYMBOL>>", HttpUtility.HtmlDecode(TDataObjects.PaymentSettings.FormatCurrency(cardFeeValue, _ucr.BusinessUnit, _ucr.PartnerCode)))
            If cardFeeValue > 0 Then
                _listOfFeesPerCard.Add(cardCode, cardFeeValue)
            End If
        End If
        Return itemText
    End Function

    Private Function GetCardCodeFeeValue(ByVal cardCode As String) As String
        Dim cardFeeValue As String = "0"
        If Profile.Basket IsNot Nothing AndAlso Profile.Basket.BasketSummary IsNot Nothing AndAlso Profile.Basket.BasketSummary.FeesDTCardTypeBooking IsNot Nothing Then
            For rowIndex As Integer = 0 To Profile.Basket.BasketSummary.FeesDTCardTypeBooking.Rows.Count - 1
                If cardCode.ToUpper = TEBUtilities.CheckForDBNull_String(Profile.Basket.BasketSummary.FeesDTCardTypeBooking.Rows(rowIndex)("CARD_TYPE_CODE")) Then
                    cardFeeValue = TEBUtilities.CheckForDBNull_String(Profile.Basket.BasketSummary.FeesDTCardTypeBooking.Rows(rowIndex)("FEE_VALUE"))
                    Exit For
                End If
            Next
        End If
        Return cardFeeValue
    End Function

    Protected Sub CardSelector_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CardSelector.SelectedIndexChanged
        If Talent.eCommerce.Utilities.BasketContentTypeWithOverride = GlobalConstants.TICKETINGBASKETCONTENTTYPE OrElse Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
            If CardSelector.SelectedValue.Trim.ToUpper = "" OrElse CardSelector.SelectedValue.Trim.ToUpper = "--" Then
                CardTypeSelectionChanged = True
            Else
                Dim talBasketSummary As New Talent.Common.TalentBasketSummary
                talBasketSummary.Settings = TEBUtilities.GetSettingsObject()
                talBasketSummary.LoginID = Profile.UserName
                talBasketSummary.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talBasketSummary.Settings.BusinessUnit)
                talBasketSummary.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talBasketSummary.Settings.BusinessUnit)
                If talBasketSummary.UpdateBasketPayTypeOrCardType(Profile.Basket.Basket_Header_ID, GlobalConstants.CCPAYMENTTYPE, CardSelector.SelectedValue.ToUpper, False, True) Then
                    Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
                    CardTypeSelectionChanged = True
                End If
            End If
        Else
            CardTypeSelectionChanged = True
        End If
    End Sub
End Class