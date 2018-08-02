Imports System.Data
Imports System.Collections.Generic
Imports Talent.Common
Imports Talent.eCommerce
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_MySavedCards
    Inherits ControlBase

#Region "Public Properties"

    Public CantSaveAnymoreCards As Boolean = False
    Public Property ShowDeleteButton As Boolean = False
    Public Property ShowDefaultButton As Boolean = False
    Public Property ShowSecurityNumber As Boolean = False
    Public ReadOnly Property SelectedCard() As String
        Get
            Return ddlSavedCardList.SelectedValue
        End Get
    End Property
    Public ReadOnly Property SecurityNumber() As String
        Get
            Return txtCardSecurityNumber.Text
        End Get
    End Property
    Public ReadOnly Property UserHasSavedCards() As Boolean
        Get
            Return _userHasSavedCards
        End Get
    End Property

    Public ReadOnly Property SavedCardSelectionChanged() As Boolean
        Get
            Return _savedCardSelectionChanged
        End Get
    End Property

    Public ReadOnly Property CaptureMethod() As String
        Get
            Return ddlCaptureMethod.SelectedValue
        End Get
    End Property

#End Region

#Region "Class Level Fields"

    Private _ucr As New UserControlResource
    Private _languageCode As String = TCUtilities.GetDefaultLanguage
    Private _userHasSavedCards As Boolean = False
    Private _userHasOnly1SavedCard As Boolean = False
    Private _savedCardSelectionChanged As Boolean = False
    Private Const KEYCODE_MYSAVEDCARDS As String = "MySavedCards.ascx"
    Private Const KEYCODE_PAYMENTDETAILS As String = "PaymentDetails.ascx"

    Private ShowSelectOptionInSavedCardDropDown As Boolean = False

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = KEYCODE_MYSAVEDCARDS
        End With
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Me.Visible Then

            ddlSavedCardList.AutoPostBack = True
            SetCaptureMethod()
            If TEBUtilities.GetCurrentPageName.ToUpper.Equals("CHECKOUT.ASPX") Or TEBUtilities.GetCurrentPageName.ToUpper.Equals("SAVEMYCARD.ASPX") Then

                If TEBUtilities.GetCurrentPageName.ToUpper.Equals("SAVEMYCARD.ASPX") Then
                    ddlSavedCardList.AutoPostBack = False
                End If

                Me.Visible = True
                If ShowSecurityNumber AndAlso CATHelper.IsBasketNotInCancelMode Then
                    plhSecurityNumber.Visible = True
                    rfvSecurityNumber.Enabled = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("SecurityNumberEnableRFV"))
                    _ucr.KeyCode = KEYCODE_PAYMENTDETAILS
                    If rfvSecurityNumber.Enabled Then rgxSecurityNumber.ValidationExpression = _ucr.Content("SecurityNumberRegularExpression", _languageCode, True)
                    lblCardSecurityNumber.Text = _ucr.Content("SecurityNumberLabel", _languageCode, True)
                    imgSecurityNumber.AlternateText = _ucr.Content("SecurityNumberImageAltText", _languageCode, True)
                    _ucr.KeyCode = KEYCODE_MYSAVEDCARDS
                    rfvSecurityNumber.ErrorMessage = _ucr.Content("NoSecurityNumberErrorMessage", _languageCode, True)
                    rgxSecurityNumber.ErrorMessage = _ucr.Content("InvalidSecurityNumberErrorMessage", _languageCode, True)
                End If

                If ShowDeleteButton Then
                    plhDeleteButton.Visible = True
                    btnDeleteThisCard.Text = _ucr.Content("DeleteButtonText", _languageCode, True)
                End If
                If ShowDefaultButton Then
                    plhDefaultButton.Visible = True
                    btnSetThisCardAsDefault.Text = _ucr.Content("DefaultButtonText", _languageCode, True)
                    If _userHasOnly1SavedCard Then
                        btnSetThisCardAsDefault.Enabled = False
                    Else
                        btnSetThisCardAsDefault.Enabled = True
                    End If
                End If
                If Not Page.IsPostBack OrElse ddlSavedCardList.Items.Count = 0 Then
                    PopulateSavedCardDDL()
                    If UpdateBasketPayTypeOrCardType(False) Then
                        Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
                    End If
                    AssignPartPaymentAmount()
                End If
                If (Not _savedCardSelectionChanged) AndAlso Page.IsPostBack AndAlso ddlSavedCardList.Items.Count > 0 Then
                    If UpdateBasketPayTypeOrCardType(False) Then
                        Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
                        AssignPartPaymentAmount()
                    End If
                End If

                If _userHasSavedCards Then
                    plhMySavedCards.Visible = True
                    plhNoSavedCards.Visible = False
                    ltlMySavedCardsIntroText.Text = _ucr.Content("MySavedCardsIntroText", _languageCode, True)
                    plhMySavedCardsIntroText.Visible = (ltlMySavedCardsIntroText.Text.Length > 0)
                    lblSavedCardList.Text = _ucr.Content("SavedCardsLabel", _languageCode, True)
                    lblSavedCardList.Visible = (lblSavedCardList.Text.Length > 0)
                Else
                    plhMySavedCards.Visible = False
                    ltlNoSavedCards.Text = _ucr.Content("NoSavedCardsText", _languageCode, True)
                    If ltlNoSavedCards.Text.Length > 0 Then
                        plhNoSavedCards.Visible = True
                    Else
                        plhNoSavedCards.Visible = False
                    End If
                End If
            ElseIf TEBUtilities.GetCurrentPageName.ToUpper.Equals("ONACCOUNTADJUSTMENT.ASPX") Then
                Me.Visible = True
                plhDeleteButton.Visible = False
                plhDefaultButton.Visible = False
                plhSecurityNumber.Visible = False
                plhMySavedCardsIntroText.Visible = False

                ShowSelectOptionInSavedCardDropDown = True
                ddlSavedCardList.AutoPostBack = False

                If Not Page.IsPostBack OrElse ddlSavedCardList.Items.Count = 0 Then
                    PopulateSavedCardDDL()
                End If

                If ddlSavedCardList.Items.Count > 0 Then
                    plhMySavedCards.Visible = True
                    plhNoSavedCards.Visible = False
                    lblSavedCardList.Text = _ucr.Content("SavedCardsLabel", _languageCode, True)
                    lblSavedCardList.Visible = (lblSavedCardList.Text.Length > 0)
                Else
                    plhMySavedCards.Visible = False
                    ltlNoSavedCards.Text = _ucr.Content("NoSavedCardsText", _languageCode, True)
                    If ltlNoSavedCards.Text.Length > 0 Then
                        plhNoSavedCards.Visible = True
                    Else
                        plhNoSavedCards.Visible = False
                    End If
                End If
            Else
                Me.Visible = False
            End If

        End If
    End Sub

    Protected Sub btnDeleteThisCard_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDeleteThisCard.Click
        deleteThisCard(ddlSavedCardList.SelectedValue)
    End Sub
    Protected Sub btnSetThisCardAsDefault_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSetThisCardAsDefault.Click
        setThisCardAsDefault(ddlSavedCardList.SelectedValue)
    End Sub

    Protected Sub ddlSavedCardList_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSavedCardList.SelectedIndexChanged

        If TEBUtilities.GetCurrentPageName.ToUpper.Equals("ONACCOUNTADJUSTMENT.ASPX") Then
            Return
        End If

        If UpdateBasketPayTypeOrCardType(True) Then
            Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
            AssignPartPaymentAmount()
        End If

    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Method to retrieve the saved cards from TALENT and alter the display acordingly
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RetrieveMySavedCards(Optional ByRef dataSet As Data.DataSet = Nothing)
        Dim err As New ErrorObj
        Dim payment As New TalentPayment
        Dim dePayment As New DEPayments
        dePayment.SessionId = Profile.Basket.Basket_Header_ID
        dePayment.CustomerNumber = Profile.UserName
        dePayment.Source = "W"
        If ModuleDefaults.PaymentGatewayType = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
            dePayment.PaymentType = GlobalConstants.PAYMENTTYPE_VANGUARD
        End If
        payment.De = dePayment
        payment.Settings = TEBUtilities.GetSettingsObject
        payment.Settings.Cacheing = TCUtilities.CheckForDBNull_Boolean_DefaultTrue(_ucr.Attribute("RetrieveMySavedCardsCache"))
        payment.Settings.CacheTimeMinutes = TCUtilities.CheckForDBNull_Int(_ucr.Attribute("RetrieveMySavedCardsCacheTime"))
        err = payment.RetrieveMySavedCards

        Try
            If Not err.HasError Then
                If payment.ResultDataSet.Tables.Count > 1 Then
                    If payment.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        If String.IsNullOrWhiteSpace(payment.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString) Then
                            If payment.ResultDataSet.Tables("CardDetails").Rows.Count > 0 Then
                                dataSet = payment.ResultDataSet
                                _userHasSavedCards = True
                                If payment.ResultDataSet.Tables("CardDetails").Rows.Count = 1 Then
                                    _userHasOnly1SavedCard = True
                                End If
                                If payment.ResultDataSet.Tables("CardDetails").Rows.Count = 9 Then
                                    CantSaveAnymoreCards = True
                                End If
                            End If
                        End If
                    Else
                        If payment.ResultDataSet.Tables("CardDetails").Rows.Count > 0 Then
                            dataSet = payment.ResultDataSet
                            _userHasSavedCards = True
                            If payment.ResultDataSet.Tables("CardDetails").Rows.Count = 1 Then
                                _userHasOnly1SavedCard = True
                            End If
                            If payment.ResultDataSet.Tables("CardDetails").Rows.Count = 9 Then
                                CantSaveAnymoreCards = True
                            End If
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            _userHasSavedCards = False
        End Try
    End Sub

    ''' <summary>
    ''' Reset the security number text box
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ResetForm()
        txtCardSecurityNumber.Text = String.Empty
    End Sub

    Public Sub PopulateSavedCardDDL()
        Dim dsMySavedCards As New Data.DataSet
        RetrieveMySavedCards(dsMySavedCards)

        ddlSavedCardList.Items.Clear()

        If dsMySavedCards.Tables("CardDetails") IsNot Nothing Then

            Dim item As New ListItem
            If ShowSelectOptionInSavedCardDropDown Then
                item.Text = _ucr.Content("PleaseSelectOptionText", _languageCode, True)
                item.Value = "-1"
                ddlSavedCardList.Items.Add(item)
            End If

            For Each row As DataRow In dsMySavedCards.Tables("CardDetails").Rows
                item = New ListItem
                If row("DefaultCard") = "Y" Then
                    item.Text = _ucr.Content("CardDisplayTextDefault", _languageCode, True)
                    item.Text = item.Text.Replace("<<DEFAULT_CARD_TEXT>>", _ucr.Content("DefaultCardString", _languageCode, True))
                    item.Selected = true
                Else
                    item.Text = _ucr.Content("CardDisplayText", _languageCode, True)
                    item.Text = item.Text.Replace("<<DEFAULT_CARD_TEXT>>", "")
                End If
                item.Text = item.Text.Replace("<<CARD_NUMBER_PREFIX_TEXT>>", _ucr.Content("EncryptedCardNumberPrefixString", _languageCode, True))
                item.Text = item.Text.Replace("<<LAST_FOUR_CARD_DIGITS>>", row("LastFourCardDigits"))
                item.Value = row("UniqueCardId")
                ddlSavedCardList.Items.Add(item)
            Next
        End If

    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Deletes the selected card from the card details file in TALENT against this customer
    ''' </summary>
    ''' <param name="uniqueCardId">The record ID given to the card/customer number in the card details file in TALENT</param>
    ''' <remarks></remarks>
    Private Sub deleteThisCard(ByVal uniqueCardId As String)
        Dim err As New ErrorObj
        Dim payment As New TalentPayment
        Dim dePayment As New DEPayments
        Dim hasCardBeenDeleted As Boolean = False

        dePayment.UniqueCardId = uniqueCardId
        dePayment.CustomerNumber = Profile.UserName
        dePayment.SessionId = Profile.Basket.Basket_Header_ID
        dePayment.Source = GlobalConstants.SOURCE
        If ModuleDefaults.PaymentGatewayType = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
            dePayment.PaymentType = GlobalConstants.PAYMENTTYPE_VANGUARD
        End If
        payment.De = dePayment
        payment.Settings = TEBUtilities.GetSettingsObject()
        err = payment.DeleteMyCard

        Try
            If Not err.HasError Then
                If payment.ResultDataSet.Tables.Count > 1 Then
                    If payment.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        If String.IsNullOrWhiteSpace(payment.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString) Then
                            hasCardBeenDeleted = True
                        End If
                    Else
                        hasCardBeenDeleted = True
                    End If
                End If
            End If
        Catch ex As Exception
            hasCardBeenDeleted = False
        End Try

        If hasCardBeenDeleted Then
            payment.RetrieveMySavedCardsClearCache()
            Dim ltlSuccessMessage As Literal = CType(Parent.FindControl("ltlSuccessMessage"), Literal)
            Dim plhSuccessMessage As PlaceHolder = CType(Parent.FindControl("plhSuccessMessage"), PlaceHolder)
            If ltlSuccessMessage IsNot Nothing Then ltlSuccessMessage.Text = _ucr.Content("CardDeletedSuccessMessage", _languageCode, True)
            If plhSuccessMessage IsNot Nothing Then plhSuccessMessage.Visible = True
            PopulateSavedCardDDL()
        Else
            Dim ltlErrorMessage As Literal = CType(Parent.FindControl("ltlErrorMessage"), Literal)
            Dim plhErrorMessage As PlaceHolder = CType(Parent.FindControl("plhErrorMessage"), PlaceHolder)
            If ltlErrorMessage IsNot Nothing Then ltlErrorMessage.Text = _ucr.Content("CardNotDeletedErrorMessage", _languageCode, True)
            If plhErrorMessage IsNot Nothing Then plhErrorMessage.Visible = True
        End If
    End Sub

    ''' <summary>
    ''' Updates the selected card as the default in the card details file in TALENT against this customer
    ''' </summary>
    ''' <param name="uniqueCardId">The record ID given to the card/customer number in the card details file in TALENT</param>
    ''' <remarks></remarks>
    Private Sub setThisCardAsDefault(ByVal uniqueCardId As String)
        Dim err As New ErrorObj
        Dim payment As New TalentPayment
        Dim dePayment As New DEPayments
        Dim hasCardBeenSetAsDefault As Boolean = False

        dePayment.UniqueCardId = uniqueCardId
        dePayment.CustomerNumber = Profile.UserName
        dePayment.SessionId = Profile.Basket.Basket_Header_ID
        dePayment.Source = GlobalConstants.SOURCE
        If ModuleDefaults.PaymentGatewayType = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
            dePayment.PaymentType = GlobalConstants.PAYMENTTYPE_VANGUARD
        End If
        payment.De = dePayment
        payment.Settings = TEBUtilities.GetSettingsObject()
        err = payment.SetMyCardAsDefault

        Try
            If Not err.HasError Then
                If payment.ResultDataSet.Tables.Count > 1 Then
                    If payment.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        If String.IsNullOrWhiteSpace(payment.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString) Then
                            hasCardBeenSetAsDefault = True
                        End If
                    Else
                        hasCardBeenSetAsDefault = True
                    End If
                End If
            End If
        Catch ex As Exception
            hasCardBeenSetAsDefault = False
        End Try

        If hasCardBeenSetAsDefault Then
            payment.RetrieveMySavedCardsClearCache()
            Dim ltlSuccessMessage As Literal = CType(Parent.FindControl("ltlSuccessMessage"), Literal)
            Dim plhSuccessMessage As PlaceHolder = CType(Parent.FindControl("plhSuccessMessage"), PlaceHolder)
            If ltlSuccessMessage IsNot Nothing Then ltlSuccessMessage.Text = _ucr.Content("CardSetAsDefaultSuccessMessage", _languageCode, True)
            If plhSuccessMessage IsNot Nothing Then plhSuccessMessage.Visible = True
            PopulateSavedCardDDL()
        Else
            Dim ltlErrorMessage As Literal = CType(Parent.FindControl("ltlErrorMessage"), Literal)
            Dim plhErrorMessage As PlaceHolder = CType(Parent.FindControl("plhErrorMessage"), PlaceHolder)
            If ltlErrorMessage IsNot Nothing Then ltlErrorMessage.Text = _ucr.Content("CardNotSetAsDefaultErrorMessage", _languageCode, True)
            If TEBUtilities.CheckForDBNull_String(payment.ResultDataSet.Tables(0).Rows(0)("ReturnCode").ToString) = "DE" Then
                ltlErrorMessage.Text = _ucr.Content("CardNotSetAsDefaultExpiredErrorMessage", _languageCode, True)
            End If

            If plhErrorMessage IsNot Nothing Then plhErrorMessage.Visible = True
        End If
    End Sub

    Private Function UpdateBasketPayTypeOrCardType(ByVal isCardSelectionChanged As Boolean) As Boolean
        Dim isUpdated As Boolean = False
        If Not TEBUtilities.GetCurrentPageName.ToUpper.Equals("SAVEMYCARD.ASPX") Then
            Dim cardTypeCode As String = GetCardTypeCode()
            If Not String.IsNullOrWhiteSpace(cardTypeCode) Then
                Dim talBasketSummary As New Talent.Common.TalentBasketSummary
                talBasketSummary.Settings = TEBUtilities.GetSettingsObject()
                talBasketSummary.LoginID = Profile.UserName
                If talBasketSummary.UpdateBasketPayTypeOrCardType(Profile.Basket.Basket_Header_ID, GlobalConstants.CCPAYMENTTYPE, cardTypeCode, False, True) Then
                    isUpdated = True
                End If
            End If
        End If
        If isCardSelectionChanged Then
            _savedCardSelectionChanged = True
        End If
        Return isUpdated
    End Function

    Private Function GetCardTypeCode() As String
        Dim cardTypeCode As String = String.Empty
        Dim dsMySavedCards As New Data.DataSet
        RetrieveMySavedCards(dsMySavedCards)
        If ddlSavedCardList.Items.Count > 0 AndAlso dsMySavedCards.Tables("CardDetails") IsNot Nothing AndAlso dsMySavedCards.Tables("CardDetails").Rows.Count > 0 Then
            For rowIndex As Integer = 0 To dsMySavedCards.Tables("CardDetails").Rows.Count - 1
                If dsMySavedCards.Tables("CardDetails").Rows(rowIndex)("UniqueCardId").ToString = ddlSavedCardList.SelectedValue Then
                    'got the card
                    cardTypeCode = TCUtilities.GetCardType(TEBUtilities.CheckForDBNull_String(dsMySavedCards.Tables("CardDetails").Rows(rowIndex)("CardNumber")), ConfigurationManager.ConnectionStrings("SqlServer2005").ConnectionString)
                    Exit For
                End If
            Next
        End If
        Return cardTypeCode
    End Function

    Private Sub AssignPartPaymentAmount()
        Dim scCheckoutPartPayment As UserControls_CheckoutPartPayments = TEBUtilities.FindWebControl("uscSCPartPayment", Me.Page.Controls)
        If scCheckoutPartPayment IsNot Nothing Then scCheckoutPartPayment.SetPartPaymentAmount()
    End Sub

    Private Sub SetCaptureMethod()
        If ddlCaptureMethod.Items.Count <= 0 Then
            Dim dicVanguardCaptureMethod As Dictionary(Of String, String) = TDataObjects.AppVariableSettings.TblDefaults.GetVanguardCaptureMethod(_ucr.BusinessUnit, _languageCode)
            ddlCaptureMethod.DataSource = dicVanguardCaptureMethod
            ddlCaptureMethod.DataTextField = "Value"
            ddlCaptureMethod.DataValueField = "Key"
            ddlCaptureMethod.DataBind()
            ddlCaptureMethod.SelectedIndex = -1
            Dim dicVanguardAttributes As Dictionary(Of String, String) = TDataObjects.AppVariableSettings.TblDefaults.GetVanguardAttributes(_ucr.BusinessUnit, _languageCode)
            If AgentProfile.IsAgent Then
                Dim buiDefaultCapture As String = String.Empty
                buiDefaultCapture = AgentProfile.DefaultCaptureMethod.Trim
                If Not (buiDefaultCapture.Length > 0 AndAlso buiDefaultCapture <> "0") Then
                    buiDefaultCapture = TEBUtilities.GetVanguardDefaultAttribute(dicVanguardAttributes, "BUI_DEFAULT_CAPTURE_METHOD", "12")
                End If
                ddlCaptureMethod.SelectedIndex = ddlCaptureMethod.Items.IndexOf(ddlCaptureMethod.Items.FindByValue(buiDefaultCapture))
                plhCaptureMethod.Visible = True
                lblCaptureMethod.Text = _ucr.Content("CaptureMethodLabel", _languageCode, True)
            Else
                Dim pwsDefaultCapture As String = TEBUtilities.GetVanguardDefaultAttribute(dicVanguardAttributes, "PWS_DEFAULT_CAPTURE_METHOD", "12")
                ddlCaptureMethod.SelectedIndex = ddlCaptureMethod.Items.IndexOf(ddlCaptureMethod.Items.FindByValue(pwsDefaultCapture))
			End If
		End If
		' don`t display the dropdown list on the page anymore
		plhCaptureMethod.Visible = False
    End Sub

#End Region
End Class