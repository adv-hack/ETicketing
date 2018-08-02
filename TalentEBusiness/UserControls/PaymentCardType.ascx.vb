Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports Talent.Common

Partial Class UserControls_PaymentCardType
    Inherits ControlBase

#Region "Class Level Fields"

    Private _ucr As Talent.Common.UserControlResource = Nothing
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _canSaveCardDetails As Boolean = False
    Private _errorMessages As New BulletedList
    Private _showFeeValueWithCard As Boolean = False

#End Region

#Region "Public Properties"

    Public Property CardTypeSelectionChanged() As Boolean = False

    Public ReadOnly Property CardTypeDDL() As DropDownList
        Get
            Return ddlCardType
        End Get
    End Property

    Public ReadOnly Property ErrorMessages() As BulletedList
        Get
            Return _errorMessages
        End Get
    End Property


#End Region

#Region "Public Methods"

    Public Function StartPayment() As Boolean
        Dim isStarted As Boolean = False
        UpdateBasketCardType(False)
        Dim talVanguard As New TalentVanguard
        Dim err As New ErrorObj
        talVanguard.Settings = TEBUtilities.GetSettingsObject()
        talVanguard.VanguardAttributes.BasketHeaderID = Profile.Basket.Basket_Header_ID
        talVanguard.VanguardAttributes.TempOrderID = Profile.Basket.Temp_Order_Id
        talVanguard.VanguardAttributes.CheckOutStage = DEVanguard.CheckoutStages.CHECKOUT
        talVanguard.VanguardAttributes.ProcessStep = DEVanguard.ProcessingStep.START_PAYMENT
        talVanguard.VanguardAttributes.PaymentType = GlobalConstants.CCPAYMENTTYPE
        talVanguard.VanguardAttributes.CaptureMethod = ddlCaptureMethod.SelectedValue
        talVanguard.VanguardAttributes.CardType = ddlCardType.SelectedValue
        talVanguard.VanguardAttributes.SessionID = Session.SessionID
        If Profile.Basket.BasketSummary.TotalBasket < 0 Then
            talVanguard.VanguardAttributes.ProcessIdentifier = DEVanguard.ProcessingIdentifier.CHARGEONLY
            talVanguard.VanguardAttributes.TxnType = DEVanguard.TransactionType.REFUND
        End If
        If (AgentProfile.IsAgent) AndAlso (Profile.Basket.BasketSummary.TotalBasket > 0) AndAlso (Me.uscCCPartPayment.FindControl("txtPartPmt") IsNot Nothing) Then
            talVanguard.VanguardAttributes.PaymentAmount = DirectCast(Me.uscCCPartPayment.FindControl("txtPartPmt"), TextBox).Text
        Else
            talVanguard.VanguardAttributes.PaymentAmount = Profile.Basket.BasketSummary.TotalBasket
        End If
        talVanguard.VanguardAttributes.BasketAmount = Profile.Basket.BasketSummary.TotalBasket
        Dim dicVanguardAttributes As Dictionary(Of String, String) = TDataObjects.AppVariableSettings.TblDefaults.GetVanguardAttributes(talVanguard.Settings.BusinessUnit, _languageCode)
        talVanguard = TEBUtilities.SetVanguardDefaultAttributes(dicVanguardAttributes, talVanguard)
        err = talVanguard.ProcessVanguard()
        If (Not err.HasError) AndAlso talVanguard.VanguardAttributes IsNot Nothing Then
            Session("VGPOSTPAGEURL") = talVanguard.VanguardAttributes.CardCapturePage
            TEBUtilities.SetVGAttributesSession(talVanguard.VanguardAttributes.BasketPaymentID, talVanguard.VanguardAttributes, talVanguard.BasketPayEntity)
            isStarted = True
        Else
            If err.ErrorNumber.Contains("TACTV-SE") Then err.ErrorNumber = "VGSGERR"
            Dim errMsg As New Talent.Common.TalentErrorMessages(_languageCode, _
                                        TalentCache.GetBusinessUnitGroup, _
                                        TalentCache.GetPartner(HttpContext.Current.Profile), _
                                        ConfigurationManager.ConnectionStrings("SqlServer2005").ToString)
            HttpContext.Current.Session("GatewayErrorMessage") = errMsg.GetErrorMessage(Talent.Common.Utilities.GetAllString, _
            Talent.eCommerce.Utilities.GetCurrentPageName, err.ErrorNumber).ERROR_MESSAGE
            HttpContext.Current.Session("TicketingGatewayError") = err.ErrorNumber
            HttpContext.Current.Session("TalentErrorCode") = err.ErrorNumber
        End If
        Return isStarted
    End Function

    Public Sub IsValidInputsAreSaved()
        IsInputSaved()
    End Sub

    Public Sub SetPartPaymentAmount()
        Dim txtCC As TextBox = Me.uscCCPartPayment.FindControl("txtPartPmt")
        txtCC.Text = Convert.ToDecimal(Profile.Basket.BasketSummary.TotalBasket).ToString("F2")
    End Sub

#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _ucr = New Talent.Common.UserControlResource
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PaymentCardType.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SetLabelText()
        If Not Page.IsPostBack Then
            LoadDDLCardType()
            AssignPartPaymentAmount()
        End If
        SetCaptureMethod()
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If Session("plhCCExternal") Is Nothing Then
            LoadDDLCardType()
            AssignPartPaymentAmount()
        End If
    End Sub

    Protected Sub CardSelector_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlCardType.SelectedIndexChanged
        UpdateBasketCardType(True)
    End Sub

#End Region

#Region "Private Methods"

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
            Else
                Dim pwsDefaultCapture As String = TEBUtilities.GetVanguardDefaultAttribute(dicVanguardAttributes, "PWS_DEFAULT_CAPTURE_METHOD", "12")
                ddlCaptureMethod.SelectedIndex = ddlCaptureMethod.Items.IndexOf(ddlCaptureMethod.Items.FindByValue(pwsDefaultCapture))
                plhCaptureMethod.Visible = False
            End If
        End If
    End Sub

    Private Sub SetCardDDLFlags()
        If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
            ddlCardType.AutoPostBack = False
        ElseIf Profile.Basket.BasketSummary.TotalBasket <= 0 AndAlso Profile.Basket.CAT_MODE.Trim.Length > 0 Then
            ddlCardType.AutoPostBack = False
        Else
            ddlCardType.AutoPostBack = True
        End If
        If Profile.Basket.BasketContentType <> GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
            If Profile.Basket.BasketSummary.TotalBasket <= 0 AndAlso Profile.Basket.CAT_MODE.Trim.Length > 0 Then
                _showFeeValueWithCard = False
            Else
                _showFeeValueWithCard = True
            End If
        End If
    End Sub

    Private Sub LoadDDLCardType()
        SetCardDDLFlags()
        ddlCardType.DataSource = TalentCache.GetDropDownControlText(TEBUtilities.GetCurrentLanguageForDDLPopulation, "PAYMENT", "CARD")
        ddlCardType.DataTextField = "Text"
        ddlCardType.DataValueField = "Value"
        ddlCardType.DataBind()

        If ddlCardType.Items.Count > 0 Then
            For itemIndex As Integer = 0 To ddlCardType.Items.Count - 1
                ddlCardType.Items(itemIndex).Text = GetCardDisplayText(ddlCardType.Items(itemIndex).Value, ddlCardType.Items(itemIndex).Text)
            Next
        End If
        SetBasketCardTypeCode()
    End Sub

    Private Sub SetBasketCardTypeCode()
        If Profile.Basket.CARD_TYPE_CODE IsNot Nothing AndAlso Profile.Basket.CARD_TYPE_CODE.Trim.Length > 0 Then
            For itemIndex As Integer = 0 To ddlCardType.Items.Count - 1
                If (ddlCardType.Items(itemIndex).Value.ToUpper = Profile.Basket.CARD_TYPE_CODE) Then
                    ddlCardType.Items(itemIndex).Selected = True
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub SetLabelText()
        With _ucr
            lblCardSelection.Text = .Content("CardSelectLabel", _languageCode, True)
            lblCaptureMethod.Text = .Content("CaptureMethodLabel", _languageCode, True)
        End With
    End Sub

    Private Sub IsInputSaved()
        Dim err As Boolean = False
        Dim affectedRows As Integer = 0
        If CardTypeDDL.SelectedIndex = 0 Then
            _errorMessages.Items.Add(_ucr.Content("ErrorCardTypeNotSelected", _languageCode, True))
            err = True
        End If
    End Sub

    Private Sub UpdateBasketCardType(ByVal isFromCardTypeDDL As Boolean)
        If Talent.eCommerce.Utilities.BasketContentTypeWithOverride = GlobalConstants.TICKETINGBASKETCONTENTTYPE OrElse Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
            If ddlCardType.SelectedValue.Trim.ToUpper = "" OrElse ddlCardType.SelectedValue.Trim.ToUpper = "--" Then
                CardTypeSelectionChanged = isFromCardTypeDDL
            Else
                Dim talBasketSummary As New Talent.Common.TalentBasketSummary
                talBasketSummary.Settings = TEBUtilities.GetSettingsObject()
                talBasketSummary.LoginID = Profile.UserName
                talBasketSummary.CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(talBasketSummary.Settings.BusinessUnit)
                talBasketSummary.FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(talBasketSummary.Settings.BusinessUnit)
                If talBasketSummary.UpdateBasketPayTypeOrCardType(Profile.Basket.Basket_Header_ID, GlobalConstants.CCPAYMENTTYPE, ddlCardType.SelectedValue.ToUpper, False, True) Then
                    Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
                    CardTypeSelectionChanged = isFromCardTypeDDL
                End If
            End If
        Else
            CardTypeSelectionChanged = True
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

    Private Sub AssignPartPaymentAmount()
        Dim ccCheckoutPartPayment As UserControls_CheckoutPartPayments = TEBUtilities.FindWebControl("uscCCPartPayment", Me.Page.Controls)
        If ccCheckoutPartPayment IsNot Nothing Then ccCheckoutPartPayment.SetPartPaymentAmount()
    End Sub

#End Region

End Class
