Imports System.Collections.Generic
Imports System.Data
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports Talent.Common


Partial Class UserControls_PaymentCardDetails
    Inherits ControlBase

    'todo SetCreditCardTypeRanges is this method has to move out of page.ispostback

#Region "Class Level Fields"
    Private Const USAGE_CHECKOUT As String = "CHECKOUT"
    Private Const USAGE_SAVEMYCARD As String = "SAVEMYCARD"
    Private Const USAGE_AMENDPPSCARD As String = "AMENDPPSCARD"
    Private _errorMessages As New BulletedList
    Private _hideSecurityNumber As Boolean = False
    Private _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _display As Boolean = True
    Private _canSaveCardDetails As Boolean = False
    Private _showFeeValueWithCard As Boolean = False
    Private _vanguardAttributes As DEVanguard = Nothing

#End Region

#Region "Public Properties"

    Public Property Usage() As String = "USAGE_CHECKOUT"

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

    Public Property CanSaveCardDetails As Boolean
        Get
            Return _canSaveCardDetails
        End Get
        Set(ByVal value As Boolean)
            _canSaveCardDetails = value
        End Set
    End Property

    Public ReadOnly Property BasketCardTypeCode() As String
        Get
            If String.IsNullOrWhiteSpace(Profile.Basket.CARD_TYPE_CODE) Then
                Return ""
            Else
                Return Profile.Basket.CARD_TYPE_CODE.ToUpper
            End If
        End Get
    End Property

    Public Property TotalCardCodes As Integer
    Public Property RFVCreditCardNumberMsg As String
    Public Property RFVStartDateMsg As String
    Public Property RFVIssueNumberMsg As String
    Public Property RFVSecurityNumberMsg As String
    Public Property RFVExpiryDateMsg As String
    Public Property RFVCreditCardType As String
    Public Property CFVExpiryDateInPastMsg As String
    Public Property CFVStartDateInFutureMsg As String
    Public Property CFVIssueNumberInvalidMsg As String
    Public Property CFVSecurityInvalidMsg As String
    Public Property CFVCreditCardNumberInvalidMsg As String
    Public Property CFVCreditCardRangeInvalidMsg As String
    Public Property RFVNoTermsConditionsChecked As String
    Public Property ValidateCardType As Boolean

    Public ReadOnly Property VGCardCapturePage() As String
        Get
            If Session("VGPOSTPAGEURL") IsNot Nothing _
                AndAlso Not String.IsNullOrWhiteSpace(Session("VGPOSTPAGEURL")) Then
                Return Session("VGPOSTPAGEURL").ToString()
            Else
                Return ""
            End If
        End Get
    End Property

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "PaymentCardDetails.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Usage = USAGE_CHECKOUT AndAlso (Not Page.IsPostBack) Then
            Dim canRemove As Boolean = False
            If Session("CheckoutExternalStarted") IsNot Nothing Then
                canRemove = True
                If Request.QueryString("_rvgpps") IsNot Nothing Then
                    If Request.QueryString("_rvgpps") = "pps1" OrElse Request.QueryString("_rvgpps") = "pps2" Then
                        If Session("VGPPSRedirect") IsNot Nothing AndAlso Session("VGPPSRedirect") = "STEP2" Then
                            Session.Remove("VGPPSRedirect")
                            canRemove = False
                        End If
                    End If
                End If
                If canRemove Then Session.Remove("CheckoutExternalStarted")
            End If
        End If

    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        Dim paymentAmount As Decimal = 0
        If Me.Visible Then
            If AgentProfile.IsAgent Then
                plhCustomerPresentCC.Visible = False
                lblCustomerPresentCC.Text = _ucr.Content("CustomerPresentText", _languageCode, True)
            End If

            SetLabelText()
            SetCardTypeDDL()
            SetCreditCardTypeRanges()
            plhSecurityNumber.Visible = (CATHelper.IsBasketNotInCancelMode AndAlso Not _hideSecurityNumber)

            If Not TryGetPartPaymentAmount(paymentAmount) Then
                paymentAmount = Profile.Basket.BasketSummary.TotalBasket
            End If
            If Usage = USAGE_CHECKOUT Then
                TEBUtilities.TicketingCheckoutExternalStart(Page.IsPostBack, ModuleDefaults.PaymentGatewayType, paymentAmount < Profile.Basket.BasketSummary.TotalBasket)
            End If
            Dim javascriptString As New StringBuilder
            javascriptString.Append("$(function () {")
            javascriptString.Append("    InitialiseTheControls();")
            javascriptString.Append("});")
            ScriptManager.RegisterStartupScript(ltlHiddenFields, Me.GetType(), "InitPaymentCardDetailCntrl", javascriptString.ToString(), True)
        End If
        Session("VanguardStage") = "STAGE1"
        GetAndWriteHiddenFields(paymentAmount)

    End Sub

#End Region

#Region "Private Methods"

    Private Sub GetAndWriteHiddenFields(ByVal paymentAmount As Decimal)
        Dim err As New ErrorObj
        Dim vanguardAttributes As DEVanguard = Nothing
        Dim basketPayEntity As DEBasketPayment = Nothing
        Dim basketPaymentID As Long

        If TEBUtilities.TryGetVGAttributesSession(basketPaymentID, vanguardAttributes, basketPayEntity) Then

            Dim registrationAddress As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)

            Dim sbHiddenFields As New StringBuilder
            sbHiddenFields.Append("<input type=""hidden"" name=""basketpaymentid"" id=""basketpaymentid"" value=""[[BASKETPAYMENTID]]""/>")
            sbHiddenFields.Append("<input type=""hidden"" name=""sessionguid"" id=""sessionguid"" value=""[[SESSIONGUID]]""/>")
            sbHiddenFields.Append("<input type=""hidden"" name=""sessionpasscode"" id=""sessionpasscode"" value=""[[SESSIONPASSCODE]]""/>")
            sbHiddenFields.Append("<input type=""hidden"" name=""processingdb"" id=""processingdb"" value=""[[PROCESSINGDB]]""/>")
            sbHiddenFields.Append("<input type=""hidden"" name=""address1"" id=""address1"" value=""[[ADDRESSLINE1]]""/>")
            sbHiddenFields.Append("<input type=""hidden"" name=""postcode"" id=""postcode"" value=""[[POSTCODE]]""/>")
            sbHiddenFields.Append("<input type=""hidden"" name=""country"" id=""country"" value=""[[COUNTRY]]""/>")
            sbHiddenFields.Append("<input type=""hidden"" name=""paymentAmount"" id=""paymentAmount"" value=""[[PAYAMOUNT]]""/>")
            sbHiddenFields.Append("<input type=""hidden"" name=""pleaseWaitText"" id=""pleaseWaitText"" value=""[[PLEASEWAITTEXT]]""/>")
            sbHiddenFields.Append(_ucr.Content("VanguardHTMLHiddenFields", _languageCode, True))
            If Not String.IsNullOrWhiteSpace(sbHiddenFields.ToString) Then
                sbHiddenFields.Replace("[[BASKETPAYMENTID]]", basketPaymentID)
                sbHiddenFields.Replace("[[SESSIONGUID]]", vanguardAttributes.SessionGUID)
                sbHiddenFields.Replace("[[SESSIONPASSCODE]]", vanguardAttributes.SessionPassCode)
                sbHiddenFields.Replace("[[PROCESSINGDB]]", vanguardAttributes.ProcessingDB)
                sbHiddenFields.Replace("[[ADDRESSLINE1]]", registrationAddress.Address_Line_2)
                sbHiddenFields.Replace("[[POSTCODE]]", registrationAddress.Post_Code)
                sbHiddenFields.Replace("[[COUNTRY]]", registrationAddress.Country)
                sbHiddenFields.Replace("[[PAYAMOUNT]]", paymentAmount)
                sbHiddenFields.Replace("[[PLEASEWAITTEXT]]", (_ucr.Content("PleaseWaitText", _languageCode, True)).Replace("""", "'").Replace("<", "[[").Replace(">", "]]"))
                ltlHiddenFields.Text = sbHiddenFields.ToString
            End If
        End If
    End Sub

    Private Sub SetCardTypeDDL()
        ddlCardTypes.ClientIDMode = UI.ClientIDMode.Static
        ddlCardTypes.DataSource = TalentCache.GetDropDownControlText(TEBUtilities.GetCurrentLanguageForDDLPopulation, "PAYMENT", "CARD")
        ddlCardTypes.DataTextField = "Text"
        ddlCardTypes.DataValueField = "Value"
        ddlCardTypes.DataBind()
        'set the visibility now
        plhCardTypes.Visible = False
        plhSaveTheseCardDetails.Visible = ModuleDefaults.UseSaveMyCard
        Dim checkoutStage As String = DEVanguard.CheckoutStages.CHECKOUT
        Dim processStep As String = DEVanguard.ProcessingStep.START_PAYMENT
        If Usage = USAGE_CHECKOUT Then
            If Session("basketPPS1List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") Is Nothing Then
                plhCardTypes.Visible = True
                checkoutStage = DEVanguard.CheckoutStages.PPS1
            ElseIf Session("basketPPS2List") IsNot Nothing AndAlso Session("PPS1PaymentComplete") = True AndAlso Session("PPS2PaymentComplete") Is Nothing Then
                plhCardTypes.Visible = True
                checkoutStage = DEVanguard.CheckoutStages.PPS2
            End If
        ElseIf Usage = USAGE_SAVEMYCARD Then
            plhCardTypes.Visible = True
            checkoutStage = DEVanguard.CheckoutStages.SAVEMYCARD
            processStep = DEVanguard.ProcessingStep.START_SAVEMYCARD
        ElseIf Usage = USAGE_AMENDPPSCARD Then
            plhCardTypes.Visible = True
            checkoutStage = DEVanguard.CheckoutStages.AMENDPPSCARD
            processStep = DEVanguard.ProcessingStep.START_AMENDPPSCARD
        End If

        If plhCardTypes.Visible Then
            plhSaveTheseCardDetails.Visible = False
            'this is pps payment or save my card so start here
            Dim talVanguard As New TalentVanguard
            Dim err As New ErrorObj
            talVanguard.Settings = TEBUtilities.GetSettingsObject()
            talVanguard.VanguardAttributes.BasketHeaderID = Profile.Basket.Basket_Header_ID
            talVanguard.VanguardAttributes.TempOrderID = Profile.Basket.Temp_Order_Id
            talVanguard.VanguardAttributes.ProcessStep = processStep
            talVanguard.VanguardAttributes.PaymentType = GlobalConstants.CCPAYMENTTYPE
            talVanguard.VanguardAttributes.SessionID = Session.SessionID
            talVanguard.VanguardAttributes.CheckOutStage = checkoutStage
            Dim dicVanguardAttributes As Dictionary(Of String, String) = TDataObjects.AppVariableSettings.TblDefaults.GetVanguardAttributes(talVanguard.Settings.BusinessUnit, _languageCode)
            talVanguard = TEBUtilities.SetVanguardDefaultAttributes(dicVanguardAttributes, talVanguard)
            err = talVanguard.ProcessVanguard()
            If (Not err.HasError) AndAlso talVanguard.VanguardAttributes IsNot Nothing Then
                Session("VGPOSTPAGEURL") = talVanguard.VanguardAttributes.CardCapturePage
                TEBUtilities.SetVGAttributesSession(talVanguard.VanguardAttributes.BasketPaymentID, talVanguard.VanguardAttributes, talVanguard.BasketPayEntity)
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
        End If
        Session("VG_FROM_3DSECURE") = Nothing
    End Sub

    Private Sub SetCreditCardTypeRanges()
        ValidateCardType = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ValidateCardType"))
        If ValidateCardType Then
            Dim ds As DataSet = Nothing
            Dim dtCreditCardTypeRanges As DataTable = Nothing
            Dim dtCreditCardTypeCodes As DataTable = Nothing
            Dim sbCreditCardTypeRanges As StringBuilder = Nothing
            Dim countCCCode As Integer = 0
            Try
                ds = TDataObjects.PaymentSettings.GetCreditCardTypeRanges(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString())
                If Not IsNothing(ds) AndAlso ds.Tables.Count > 0 Then
                    dtCreditCardTypeRanges = ds.Tables(0)
                    dtCreditCardTypeCodes = ds.Tables(1)
                End If
                ' total # of card type codes 
                TotalCardCodes = dtCreditCardTypeCodes.Rows.Count
                sbCreditCardTypeRanges = New StringBuilder()
                sbCreditCardTypeRanges.Append("<script type=""text/javascript"">")
                sbCreditCardTypeRanges.Append("function Create2DArray(rows){var arr = [];for (var i = 0; i < rows; i++){arr[i] = [];}return arr;}var CCTypeRanges = Create2DArray(" & TotalCardCodes.ToString() & ");")
                sbCreditCardTypeRanges.Append("var CCTypeCodes = [")
                For Each dr As DataRow In dtCreditCardTypeCodes.Rows
                    sbCreditCardTypeRanges.Append("""" & dr("card_code").ToString().ToUpper & """" & IIf(countCCCode <= dtCreditCardTypeCodes.Rows.Count - 2, ",", "") & "")
                    countCCCode = countCCCode + 1
                Next
                sbCreditCardTypeRanges.Append("];")
                For i As Integer = 0 To dtCreditCardTypeCodes.Rows.Count - 1
                    countCCCode = 0
                    For Each dr As DataRow In dtCreditCardTypeRanges.Select("card_code='" & dtCreditCardTypeCodes.Rows(i)("card_code") & "'")
                        sbCreditCardTypeRanges.Append("CCTypeRanges[" & i.ToString() & "][" & countCCCode.ToString() & "] = """ & dr("card_from_range").ToString() & "-" & dr("card_to_range").ToString() & """;")
                        countCCCode = countCCCode + 1
                    Next
                Next
                sbCreditCardTypeRanges.Append("</script>")
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "PaymentCardDetails", sbCreditCardTypeRanges.ToString(), False)

            Finally
                ds = Nothing
                dtCreditCardTypeRanges = Nothing
                dtCreditCardTypeRanges = Nothing
                dtCreditCardTypeCodes = Nothing
                sbCreditCardTypeRanges = Nothing
                countCCCode = Nothing
            End Try
        End If
    End Sub

    Private Sub SetLabelText()
        With _ucr
            lblCardNumber.Text = .Content("CardNumberLabel", _languageCode, False)
            lblExpiryDate.Text = .Content("ExpiryDateLabel", _languageCode, False)
            lblStartDate.Text = .Content("StartDateLabel", _languageCode, False)
            lblIssueNumber.Text = .Content("IssueNumberLabel", _languageCode, False)
            lblSecurityNumber.Text = .Content("SecurityNumberLabel", _languageCode, False)
            lblCardHolderName.Text = .Content("CardHolderNameLabel", _languageCode, False)
            lblSaveTheseCardDetails.Text = .Content("SaveThisCardLabel", _languageCode, True)
            lblCardTAndC.Text = .Content("TAndCLabelText", _languageCode, True)
            lblCardTypes.Text = .Content("CardTypesLabelText", _languageCode, True)
            RFVCreditCardNumberMsg = .Content("NoCardNumberErrorMessage", _languageCode, True)
            RFVExpiryDateMsg = .Content("ErrorExpiryDateNotSelected", _languageCode, True)
            RFVIssueNumberMsg = .Content("NoIssueNumberErrorMessage", _languageCode, True)
            RFVSecurityNumberMsg = .Content("NoSecurityNumberErrorMessage", _languageCode, True)
            RFVStartDateMsg = .Content("ErrorStartDateNotSelected", _languageCode, True)
            CFVExpiryDateInPastMsg = .Content("ErrorExpiryDateIsInThePast", _languageCode, True)
            CFVStartDateInFutureMsg = .Content("ErrorStartDateIsInTheFuture", _languageCode, True)
            CFVIssueNumberInvalidMsg = .Content("InvalidIssueNumberErrorMessage", _languageCode, True)
            CFVSecurityInvalidMsg = .Content("InvalidSecurityNumberErrorMessage", _languageCode, True)
            CFVCreditCardNumberInvalidMsg = .Content("InvalidCardNumber", _languageCode, True)
            CFVCreditCardRangeInvalidMsg = .Content("InvalidCardRange", _languageCode, True)
            RFVNoTermsConditionsChecked = .Content("NoTermsConditionsChecked", _languageCode, True)
            RFVCreditCardType = .Content("NoCreditCardTypeSelected", _languageCode, True)
        End With
    End Sub

    Private Function TryGetPartPaymentAmount(ByRef paymentAmount As Decimal) As Boolean
        Dim isPartPayFound As Boolean = False
        Dim ccCheckoutPartPayment As UserControls_CheckoutPartPayments = TEBUtilities.FindWebControl("uscCCPartPayment", Me.Page.Controls)
        If ccCheckoutPartPayment IsNot Nothing Then
            paymentAmount = ccCheckoutPartPayment.PartPaymentAmount
            isPartPayFound = True
        End If
        Return isPartPayFound
    End Function

#End Region

End Class
