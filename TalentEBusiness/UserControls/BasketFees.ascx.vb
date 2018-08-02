Imports System.Data
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Partial Class UserControls_BasketFees
    Inherits ControlBase

#Region "Class Level Fields"
    Private _ucr As UserControlResource = Nothing
    Private _businessUnit As String = Nothing
    Private _partner As String = Nothing
    Private _languageCode As String = Nothing
    Private _showCharityFeeCategoryGiftAidOption As Boolean = False
    Private _basketFeesUpdated As Boolean = False
    Private _totalItemsAppliedVariable As Integer = 0
    Private _totalItemsAppliedVariableCount As Integer = 0
    Private _charityFeeDisplayFormat As String = Nothing
    Private _adhocFeeDisplayFormat As String = Nothing
    Private _variableFeeAllowedDisplayFormat As String = Nothing
    Private _variableFeeAppliedDisplayFormat As String = Nothing
    Private _disableFormElements As Boolean = False
#End Region

#Region "Properties"
    Public ReadOnly Property GiftAidSelected() As Boolean
        Get
            Return chkGiftAid.Checked
        End Get
    End Property

    Public Property DisableFormElements() As Boolean
        Set(ByVal value As Boolean)
            _disableFormElements = value
        End Set
        Get
            Return _disableFormElements
        End Get
    End Property

    Public Property BasketFeesUpdated As Boolean
        Set(ByVal value As Boolean)
            _basketFeesUpdated = value
        End Set
        Get
            Return _basketFeesUpdated
        End Get
    End Property
#End Region

#Region "Public Methods"

#End Region

#Region "Protected Methods"
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _ucr = New UserControlResource
        _businessUnit = TalentCache.GetBusinessUnit()
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        _languageCode = TCUtilities.GetDefaultLanguage
        With _ucr
            .BusinessUnit = _businessUnit
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PartnerCode = _partner
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "BasketFees.ascx"
        End With
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Dim uscBasketSummary As UserControls_BasketSummary = Talent.eCommerce.Utilities.FindWebControl("uscBasketSummary", Me.Page.Controls)
        If (Not Page.IsPostBack) OrElse (BasketFeesUpdated) OrElse (IsBasketSummaryUpdated()) Then
            ProcessFeesCharity()
            ProcessFeesAdhoc()
            ProcessFeesVariable()
        End If
        plhBasketFees.Visible = (plhFeesCharity.Visible OrElse plhFeesAdhoc.Visible OrElse plhFeesVariable.Visible)
        plhFeesAccordion.Visible = AgentProfile.IsAgent

    End Sub

    Protected Sub rptFeesCharityDetail_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptFeesCharityDetail.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            CType(e.Item.FindControl("lblFeesCharityLabel"), Label).Text = GetFeeCodeDisplayText(_charityFeeDisplayFormat, _
                                                                                                   TEBUtilities.CheckForDBNull_String(e.Item.DataItem("FEE_CODE")), _
                                                                                                   TEBUtilities.CheckForDBNull_String(e.Item.DataItem("FEE_DESCRIPTION")), _
                                                                                                   TEBUtilities.CheckForDBNull_String(e.Item.DataItem("FEE_VALUE")))

            CType(e.Item.FindControl("hidFeesCharityCode"), HiddenField).Value = e.Item.DataItem("FEE_CODE")
            Dim checkboxFeesCharity As CheckBox = CType(e.Item.FindControl("chkFeesCharity"), CheckBox)
            If AgentProfile.Name Is String.Empty Then
                checkboxFeesCharity.Attributes.Add("onclick", GetScriptToDisableChkBox(checkboxFeesCharity))
            End If
            If CBool(e.Item.DataItem("IS_EXISTS_IN_BASKET")) Then
                checkboxFeesCharity.Checked = True
                chkGiftAid.Enabled = True
            Else
                checkboxFeesCharity.Checked = False
            End If
        End If
    End Sub

    Protected Sub ChkFeesCharity_OnCheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim chkFeesCharity As CheckBox = TryCast(sender, CheckBox)
            Dim rptFeesCharityItem As RepeaterItem = TryCast(chkFeesCharity.Parent, RepeaterItem)
            Dim hidFeesCharityCode As HiddenField = TryCast(rptFeesCharityItem.FindControl("hidFeesCharityCode"), HiddenField)
            If HasUpdatedTheFees(hidFeesCharityCode.Value, GlobalConstants.FEECATEGORY_CHARITY, chkFeesCharity.Checked) Then
                BasketFeesUpdated = True
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub rptFeesAdhocDetail_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptFeesAdhocDetail.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            CType(e.Item.FindControl("lblFeesAdhocLabel"), Label).Text = GetFeeCodeDisplayText(_adhocFeeDisplayFormat, _
                                                                                                  TEBUtilities.CheckForDBNull_String(e.Item.DataItem("FEE_CODE")), _
                                                                                                  TEBUtilities.CheckForDBNull_String(e.Item.DataItem("FEE_DESCRIPTION")), _
                                                                                                  TEBUtilities.CheckForDBNull_String(e.Item.DataItem("FEE_VALUE")))

            CType(e.Item.FindControl("hidFeesAdhocCode"), HiddenField).Value = e.Item.DataItem("FEE_CODE")
            Dim checkboxFeesAdhoc As CheckBox = CType(e.Item.FindControl("chkFeesAdhoc"), CheckBox)
            If AgentProfile.Name Is String.Empty Then
                checkboxFeesAdhoc.Attributes.Add("onclick", GetScriptToDisableChkBox(checkboxFeesAdhoc))
            End If
            If CBool(e.Item.DataItem("IS_EXISTS_IN_BASKET")) Then
                checkboxFeesAdhoc.Checked = True
            Else
                checkboxFeesAdhoc.Checked = False
            End If
        End If
    End Sub

    Protected Sub ChkFeesAdhoc_OnCheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim chkFeesAdhoc As CheckBox = TryCast(sender, CheckBox)
            Dim rptFeesAdhocItem As RepeaterItem = TryCast(chkFeesAdhoc.Parent, RepeaterItem)
            Dim hidFeesAdhocCode As HiddenField = TryCast(rptFeesAdhocItem.FindControl("hidFeesAdhocCode"), HiddenField)
            If HasUpdatedTheFees(hidFeesAdhocCode.Value, GlobalConstants.FEECATEGORY_ADHOC, chkFeesAdhoc.Checked) Then
                BasketFeesUpdated = True
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub rptFeesVariableDetail_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptFeesVariableDetail.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            CType(e.Item.FindControl("lblFeesVariableLabel"), Label).Text = GetFeeCodeDisplayText(_variableFeeAppliedDisplayFormat, _
                                                                                                    TEBUtilities.CheckForDBNull_String(e.Item.DataItem("PRODUCT")), _
                                                                                                    TEBUtilities.CheckForDBNull_String(e.Item.DataItem("PRODUCT_DESCRIPTION1")), _
                                                                                                    TEBUtilities.CheckForDBNull_String(e.Item.DataItem("GROSS_PRICE")))
            CType(e.Item.FindControl("hidFeesVariableCode"), HiddenField).Value = e.Item.DataItem("PRODUCT")
            Dim checkboxFeesVariable As CheckBox = CType(e.Item.FindControl("chkFeesVariable"), CheckBox)
            If AgentProfile.Name Is String.Empty Then
                checkboxFeesVariable.Attributes.Add("onclick", GetScriptToDisableChkBox(checkboxFeesVariable))
            End If
            checkboxFeesVariable.Checked = True
        End If
    End Sub

    Protected Sub ChkFeesVariable_OnCheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim chkFeesVariable As CheckBox = TryCast(sender, CheckBox)
            Dim rptFeesVariableItem As RepeaterItem = TryCast(chkFeesVariable.Parent, RepeaterItem)
            Dim hidFeesVariableCode As HiddenField = TryCast(rptFeesVariableItem.FindControl("hidFeesVariableCode"), HiddenField)
            If HasUpdatedTheFees(hidFeesVariableCode.Value, GlobalConstants.FEECATEGORY_VARIABLE, chkFeesVariable.Checked) Then
                BasketFeesUpdated = True
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub btnVariableFeeAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVariableFeeAdd.Click
        If IsValidVariableFeeToAdd() Then
            If HasUpdatedTheFees(ddlVariableFeesAvail.SelectedValue, GlobalConstants.FEECATEGORY_VARIABLE, True, txtVariableFeeValue.Text) Then
                BasketFeesUpdated = True
            End If
        End If
    End Sub
#End Region

#Region "Private Methods"

    Private Sub DecideGiftAidSelection()
        If chkGiftAid.Enabled Then
            If Session("GiftAidSelected") IsNot Nothing AndAlso CType(Session("GiftAidSelected"), Boolean) Then
                chkGiftAid.Checked = True
            Else
                chkGiftAid.Checked = False
            End If
        Else
            Session("GiftAidSelected") = False
        End If
    End Sub
    Private Sub ProcessFeesCharity()
        If Profile.Basket.BasketSummary IsNot Nothing Then
            If Profile.Basket.BasketSummary.FeesDTCharity IsNot Nothing AndAlso Profile.Basket.BasketSummary.FeesDTCharity.Rows.Count > 0 Then
                _charityFeeDisplayFormat = _ucr.Content("CharityFeeDisplayFormat", _languageCode, True)
                If TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("ShowCharityFeeCategoryGiftAidOption")) AndAlso CheckUserCountryCanGiftAid() Then
                    plhGiftAid.Visible = True
                    lblGiftAid.Text = _ucr.Content("CharityFeeCategoryGiftAidText", _languageCode, True)
                    chkGiftAid.Enabled = False
                End If
                ltlFeesCharityHeaderLabel.Text = _ucr.Content("FeesHeaderTextCharity", _languageCode, True)
                rptFeesCharityDetail.DataSource = Profile.Basket.BasketSummary.FeesDTCharity
                rptFeesCharityDetail.DataBind()
                plhFeesCharity.Visible = True
                If chkGiftAid.Enabled AndAlso chkGiftAid.Checked Then
                    chkGiftAid.Checked = True
                Else
                    chkGiftAid.Checked = False
                End If
            Else
                rptFeesCharityDetail.DataSource = Nothing
                rptFeesCharityDetail.DataBind()
                rptFeesCharityDetail.Visible = False
            End If
        End If
    End Sub

    Private Sub ProcessFeesAdhoc()
        If (Profile.Basket.BasketSummary IsNot Nothing) Then
            If Profile.Basket.BasketSummary.FeesDTAdhoc IsNot Nothing AndAlso Profile.Basket.BasketSummary.FeesDTAdhoc.Rows.Count > 0 Then
                _adhocFeeDisplayFormat = _ucr.Content("AdhocFeeDisplayFormat", _languageCode, True)
                ltlFeesAdhocHeaderLabel.Text = _ucr.Content("FeesHeaderTextAdhoc", _languageCode, True)
                rptFeesAdhocDetail.DataSource = Profile.Basket.BasketSummary.FeesDTAdhoc
                rptFeesAdhocDetail.DataBind()
                plhFeesAdhoc.Visible = True
            Else
                rptFeesAdhocDetail.DataSource = Nothing
                rptFeesAdhocDetail.DataBind()
                rptFeesAdhocDetail.Visible = False
            End If
        End If
    End Sub

    Private Sub ProcessFeesVariable()
        If (AgentProfile.IsAgent) AndAlso (Profile.Basket.BasketSummary IsNot Nothing) Then
            If Profile.Basket.BasketSummary.FeesDTVariableApplied IsNot Nothing AndAlso Profile.Basket.BasketSummary.FeesDTVariableApplied.Rows.Count > 0 Then
                _variableFeeAppliedDisplayFormat = _ucr.Content("VariableFeeAppliedDisplayFormat", _languageCode, True)
                rptFeesVariableDetail.Visible = True
                rptFeesVariableDetail.DataSource = Profile.Basket.BasketSummary.FeesDTVariableApplied
                rptFeesVariableDetail.DataBind()
                plhFeesVariable.Visible = True
            Else
                rptFeesVariableDetail.DataSource = Nothing
                rptFeesVariableDetail.DataBind()
                rptFeesVariableDetail.Visible = False
            End If
            If Profile.Basket.BasketSummary.FeesDTVariable IsNot Nothing AndAlso Profile.Basket.BasketSummary.FeesDTVariable.Rows.Count > 0 Then
                _variableFeeAllowedDisplayFormat = _ucr.Content("VariableFeeAllowedDisplayFormat", _languageCode, True)
                ddlVariableFeesAvail.DataSource = Profile.Basket.BasketSummary.FeesDTVariable
                ddlVariableFeesAvail.DataTextField = "FEE_DESCRIPTION"
                ddlVariableFeesAvail.DataValueField = "FEE_CODE"
                ddlVariableFeesAvail.DataBind()
                For itemIndex As Integer = 0 To ddlVariableFeesAvail.Items.Count - 1
                    ddlVariableFeesAvail.Items(itemIndex).Text = GetFeeCodeDisplayText(_variableFeeAllowedDisplayFormat, ddlVariableFeesAvail.Items(itemIndex).Value, ddlVariableFeesAvail.Items(itemIndex).Text, Nothing)
                Next
                If ddlVariableFeesAvail.Items.Count > 0 Then
                    lblVariableFeeDDLLabel.Text = _ucr.Content("VariableFeeDDLLabel", _languageCode, True)
                    lblVariableFeeDDLLabel.Visible = (lblVariableFeeDDLLabel.Text.Length > 0)
                    lblVariableFeeValLabel.Text = _ucr.Content("VariableFeeValLabel", _languageCode, True)
                    lblVariableFeeValLabel.Visible = (lblVariableFeeValLabel.Text.Length > 0)
                    btnVariableFeeAdd.Text = _ucr.Content("VariableFeeAddButtonText", _languageCode, True)
                    rgxVariableFee.ValidationExpression = _ucr.Attribute("VariableFeeValueRegEx")
                    rgxVariableFee.ErrorMessage = _ucr.Content("VariableFeeValidationErrorMessage", _languageCode, True)
                    txtVariableFeeValue.Text = ""
                    plhFeesVariable.Visible = True
                    plhVariableFeesAvail.Visible = True
                End If
            Else
                ddlVariableFeesAvail.Items.Clear()
                plhVariableFeesAvail.Visible = False
            End If
            If plhFeesVariable.Visible OrElse plhVariableFeesAvail.Visible Then
                ltlFeesVariableHeaderLabel.Text = _ucr.Content("FeesHeaderTextVariable", _languageCode, True)
            End If
        End If
    End Sub

    Private Sub ProcessBasketTotalChange(ByVal currentBasketTotal As Decimal)
        Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True, True)
        Dim updatedBasketTotal As Decimal = Profile.Basket.BasketSummary.TotalBasket
        Dim totalChangeAffectPayOptions As Boolean = False
        If currentBasketTotal > 0 AndAlso updatedBasketTotal <= 0 Then
            totalChangeAffectPayOptions = True
        ElseIf currentBasketTotal < 0 AndAlso updatedBasketTotal >= 0 Then
            totalChangeAffectPayOptions = True
        ElseIf currentBasketTotal = 0 AndAlso updatedBasketTotal <> 0 Then
            totalChangeAffectPayOptions = True
        End If
        If totalChangeAffectPayOptions Then
            If Request.Url.PathAndQuery.Contains("/PagesLogin/Checkout/Checkout.aspx") Then
                Response.Redirect(Request.Url.AbsoluteUri)
            End If
        End If
    End Sub

    Private Function GetVarFeeCodeDDLDisplayText(ByVal feeCode As String) As String
        Dim itemText As String = _ucr.Content(feeCode, _languageCode, True)
        If String.IsNullOrWhiteSpace(itemText) Then
            itemText = feeCode
        Else
            itemText = itemText.Replace("<<FEE_CODE>>", feeCode)
        End If
        Return itemText
    End Function

    Private Function HasUpdatedTheFees(ByVal feeCode As String, ByVal feeCategory As String, ByVal addFeeToBasket As Boolean) As Boolean
        Dim loginID As String = ""
        If Not Profile.IsAnonymous Then
            If Profile.User IsNot Nothing AndAlso Profile.User.Details IsNot Nothing Then
                loginID = Profile.User.Details.LoginID
            End If
        End If
        Dim currentBasketTotal As Decimal = Profile.Basket.BasketSummary.TotalBasket
        Dim hasUpdated As Boolean = TDataObjects.BasketSettings.HasMovedFeeInOrOutOfBasket(Profile.Basket.Basket_Header_ID, loginID, feeCode, feeCategory, addFeeToBasket)
        If hasUpdated Then
            ProcessBasketTotalChange(currentBasketTotal)
        End If
        Return hasUpdated
    End Function

    Private Function HasUpdatedTheFees(ByVal feeCode As String, ByVal feeCategory As String, ByVal addFeeToBasket As Boolean, ByVal feeValue As String) As Boolean
        Dim loginID As String = ""
        If Not Profile.IsAnonymous Then
            If Profile.User IsNot Nothing AndAlso Profile.User.Details IsNot Nothing Then
                loginID = Profile.User.Details.LoginID
            End If
        End If
        Dim currentBasketTotal As Decimal = Profile.Basket.BasketSummary.TotalBasket
        Dim hasUpdated As Boolean = TDataObjects.BasketSettings.HasMovedFeeInOrOutOfBasket(Profile.Basket.Basket_Header_ID, loginID, feeCode, feeCategory, addFeeToBasket, feeValue)
        If hasUpdated Then
            ProcessBasketTotalChange(currentBasketTotal)
        End If
        Return hasUpdated
    End Function

    Private Function GetFeeCodeDisplayText(ByVal displayFormat As String, ByVal feeCode As String, ByVal feeDescription As String, ByVal feeValue As Decimal) As String
        feeCode = TEBUtilities.CheckForDBNull_String(feeCode)
        feeDescription = TEBUtilities.CheckForDBNull_String(feeDescription)
        If String.IsNullOrWhiteSpace(displayFormat) Then
            If feeValue = Nothing Then
                displayFormat = "<<FEE_DESCRIPTION>> (<<FEE_CODE>>)"
            Else
                displayFormat = "<<FEE_DESCRIPTION>> (<<FEE_CODE>>) (<<FEE_VALUE_WITH_SYMBOL>>)"
            End If

        End If
        displayFormat = displayFormat.Replace("<<FEE_CODE>>", feeCode)
        displayFormat = displayFormat.Replace("<<FEE_DESCRIPTION>>", feeDescription)
        If feeValue = Nothing Then
            displayFormat = displayFormat.Replace("<<FEE_VALUE>>", "")
            displayFormat = displayFormat.Replace("<<FEE_VALUE_WITH_SYMBOL>>", "")
        Else
            feeValue = TEBUtilities.CheckForDBNull_Decimal(feeValue)
            displayFormat = displayFormat.Replace("<<FEE_VALUE>>", feeValue)
            displayFormat = displayFormat.Replace("<<FEE_VALUE_WITH_SYMBOL>>", TDataObjects.PaymentSettings.FormatCurrency(feeValue, _ucr.BusinessUnit, _ucr.PartnerCode))
        End If
        Return displayFormat
    End Function

    Private Function CheckUserCountryCanGiftAid() As Boolean
        Dim canGiftAid As Boolean = False
        Try
            If (Not Profile.IsAnonymous) AndAlso Profile.User IsNot Nothing Then
                Dim address As TalentProfileAddress = ProfileHelper.ProfileAddressEnumerator(0, Profile.User.Addresses)
                canGiftAid = TDataObjects.ProfileSettings.tblCountry.CanCountryUseGiftAid(address.Country.ToUpper)
            End If
        Catch ex As Exception
            canGiftAid = False
        End Try
        Return canGiftAid
    End Function

    Private Function IsBasketSummaryUpdated() As Boolean
        Dim uscBasketSummary As UserControls_BasketSummary = Talent.eCommerce.Utilities.FindWebControl("uscBasketSummary", Me.Page.Controls)
        Return (uscBasketSummary IsNot Nothing AndAlso uscBasketSummary.BasketSummaryUpdated)
    End Function

    Private Function IsValidVariableFeeToAdd() As Boolean
        Dim isValidToAdd As Boolean = False
        Try
            If ddlVariableFeesAvail.SelectedValue.Length > 0 Then
                If IsNumeric(txtVariableFeeValue.Text) Then
                    isValidToAdd = True
                End If
            End If
        Catch ex As Exception
            isValidToAdd = False
        End Try
        Return isValidToAdd
    End Function

    Private Function GetScriptToDisableChkBox(ByVal checkBoxControl As CheckBox) As String
        Dim javascriptFunction As New StringBuilder()
        javascriptFunction.Append("this.disabled = true;")
        javascriptFunction.Append(Me.Page.ClientScript.GetPostBackEventReference(checkBoxControl, Nothing))
        javascriptFunction.Append(";return true;")
        Return javascriptFunction.ToString
    End Function

    Private Function isFeesCharityChecked() As Boolean
        Dim feesCharityChecked As Boolean = False
        For Each rptFeesCharityDetailItem As RepeaterItem In rptFeesCharityDetail.Items
            Dim chkFeesCharity As CheckBox = CType(rptFeesCharityDetailItem.FindControl("chkFeesCharity"), CheckBox)
            If chkFeesCharity.Checked Then
                feesCharityChecked = True
                Exit For
            End If
        Next
        Return feesCharityChecked
    End Function
#End Region

End Class
