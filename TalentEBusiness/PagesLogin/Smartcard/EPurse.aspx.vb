Imports System.Data
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesLogin_Smartcard_EPurse
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfrPage As WebFormResource = Nothing
    Private _languageCode As String = Nothing
    Private _topUpAmountMultiples As Decimal
    Private _cardNumber As String = Nothing
    Private _cardAmount As String = Nothing
    Private _cardAmountToSpend As String = Nothing
    Private _topUpAmountSelectedValue As String = Nothing

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        If Not Profile.IsAnonymous Then
            'Check if agent has access on ECash menu item
            If (AgentProfile.IsAgent And AgentProfile.AgentPermissions.CanAccessECash) Or Not AgentProfile.IsAgent Then
                _wfrPage = New WebFormResource
                _languageCode = TCUtilities.GetDefaultLanguage
                With _wfrPage
                    .BusinessUnit = TalentCache.GetBusinessUnit()
                    .PageCode = TEBUtilities.GetCurrentPageName()
                    .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    .KeyCode = .PageCode
                End With

                Session("EPurseTopUp-CardNumber") = Nothing
                Session("EPurseTopUp-Amount") = Nothing

                If Session("TicketingGatewayError") IsNot Nothing Then
                    Dim errMsg As TalentErrorMessages
                    errMsg = New TalentErrorMessages(_languageCode, TalentCache.GetBusinessUnitGroup, TalentCache.GetPartner(Profile), _wfrPage.FrontEndConnectionString)
                    Dim talentErrorMsg As TalentErrorMessage = errMsg.GetErrorMessage(Session("TicketingGatewayError"))
                    Dim listItemObject As ListItem = blErrorMessage.Items.FindByText(talentErrorMsg.ERROR_MESSAGE)
                    If listItemObject Is Nothing Then blErrorMessage.Items.Add(talentErrorMsg.ERROR_MESSAGE)
                    Session("TicketingGatewayError") = Nothing
                End If
            Else
                Session("UnavailableErrorCode") = "GenericUnauthorisedAccess"
                Session("UnavailableReturnPage") = String.Empty
                Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
            End If
        Else
                Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub btnEPurseTopUp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnEPurseTopUp.Click
        Page.Validate()
        If Page.IsValid Then
            Dim topUpAmount As String = String.Empty
            If plhEPurseTopUpTextBox.Visible Then topUpAmount = txtEPurseTopUpAmount.Text.Trim()
            If plhEPurseTopUpDropDownList.Visible Then topUpAmount = ddlEPurseTopUpAmount.SelectedValue
            If (CDec(topUpAmount) Mod _topUpAmountMultiples) = 0 Then
                Dim redirectUrl As New StringBuilder
                Dim priceBandArrayIndex As Integer = 0
                Dim priceBandArray(25, 1) As String
                While priceBandArrayIndex < 26
                    priceBandArray(priceBandArrayIndex, 0) = String.Empty
                    priceBandArray(priceBandArrayIndex, 1) = String.Empty
                    priceBandArrayIndex += 1
                End While
                priceBandArray(0, 0) = "A"
                priceBandArray(0, 1) = "1"
                redirectUrl.Append("~/Redirect/TicketingGateway.aspx?page=ProductMembership.aspx&function=AddToBasket&product=")
                redirectUrl.Append(ModuleDefaults.EPurseTopUpProductCode)
                redirectUrl.Append("&priceCode=")
                'Session("EPurseTopUp-CardNumber") = ltlEPurseCardNumberValue.Text
                Session("EPurseTopUp-CardNumber") = ddlSelectCard.SelectedItem.ToString
                Session("EPurseTopUp-Amount") = CDec(topUpAmount).ToString("F2").Replace(".", String.Empty)
                Session("PriceBandSelectionOptions") = priceBandArray
                Response.Redirect(redirectUrl.ToString())
            Else
                blErrorMessage.Items.Clear()
                blErrorMessage.Items.Add(_wfrPage.Content("InvalidAmountMultiplesError", _languageCode, True).Replace("<<AMOUNT_MULTIPLES>>", _topUpAmountMultiples))
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set the page based on what card data has been retrieved
    ''' </summary>
    ''' <param name="validCardData">Boolean value to indicate whether or not there is a valid card to use</param>
    ''' <remarks></remarks>
    Private Sub setCardInformation(ByVal validCardData As Boolean)
        If validCardData Then
            pnlEPurseTopUp.Visible = True
            ' ltlEPurseCardNumberValue.Text = _cardNumber
            ltlEPurseCardAmountValue.Text = TDataObjects.PaymentSettings.FormatCurrency(_cardAmount, _wfrPage.BusinessUnit, _wfrPage.PartnerCode)
            ltlEPurseCardAmountToSpendValue.Text = TDataObjects.PaymentSettings.FormatCurrency(_cardAmountToSpend, _wfrPage.BusinessUnit, _wfrPage.PartnerCode)
        Else
            pnlEPurseTopUp.Visible = False
            ' ltlEPurseCardNumberValue.Text = _wfrPage.Content("NoCardNumber", _languageCode, True)
            ltlEPurseCardAmountValue.Text = TDataObjects.PaymentSettings.FormatCurrency(0, _wfrPage.BusinessUnit, _wfrPage.PartnerCode)
            ltlEPurseCardAmountToSpendValue.Text = TDataObjects.PaymentSettings.FormatCurrency(0, _wfrPage.BusinessUnit, _wfrPage.PartnerCode)
        End If

        If String.IsNullOrWhiteSpace(ModuleDefaults.EPurseTopUpProductCode) Then
            pnlEPurseTopUp.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Set the default text values for the page
    ''' </summary>
    ''' <param name="validCardData">Valid card boolean</param>
    ''' <remarks></remarks>
    Private Sub setPageDefaults(ByVal validCardData As Boolean)
        ltlEPurseBalanceHeader.Text = _wfrPage.Content("EPurseBalanceHeader", _languageCode, True)
        plhEPurseBalanceHeader.Visible = (ltlEPurseBalanceHeader.Text.Length > 0)
        ltlEPurseCardNumberLabel.Text = _wfrPage.Content("EPurseCardNumberLabel", _languageCode, True)
        ltlPleaseChooseCard.Text = _wfrPage.Content("ltlPleaseChooseCard", _languageCode, True)
        ltlEPurseCardAmountLabel.Text = _wfrPage.Content("EPurseCardAmountLabel", _languageCode, True)
        plhEPurseCardAmountToSpend.Visible = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(_wfrPage.Attribute("ShowEPurseAmountToSpend"))
        If plhEPurseCardAmountToSpend.Visible Then ltlEPurseCardAmountToSpendLabel.Text = _wfrPage.Content("EPurseCardAmountToSpendLabel", _languageCode, True)

        If validCardData AndAlso pnlEPurseTopUp.Visible Then
            ltlEPurseTopUpHeader.Text = _wfrPage.Content("EPurseTopUpHeader", _languageCode, True)
            plhEPurseTopUpHeader.Visible = (ltlEPurseTopUpHeader.Text.Length > 0)
            ltlEPurseTopUpLegend.Text = _wfrPage.Content("EPurseTopUpLegend", _languageCode, True)
            btnEPurseTopUp.Text = _wfrPage.Content("EPurseTopUpButtonText", _languageCode, True)
            rfvEPurseTopUpAmount.ErrorMessage = _wfrPage.Content("NoAmountError", _languageCode, True)
            rgxEPurseTopUpAmount.ErrorMessage = _wfrPage.Content("InvalidAmountError", _languageCode, True)
            rgxEPurseTopUpAmount.ValidationExpression = TEBUtilities.CheckForDBNull_String(_wfrPage.Attribute("InvalidAmountValidationExpression"))
            _topUpAmountMultiples = TEBUtilities.CheckForDBNull_Decimal(_wfrPage.Attribute("TopUpAmountMulitples"))
            ltlEPurseTopUpInstructions.Text = _wfrPage.Content("EPurseTopUpAmountInstructions", _languageCode, True).Replace("<<AMOUNT_MULTIPLES>>", _topUpAmountMultiples)
            If _wfrPage.Attribute("TopUpTextOrDropDownList") = "DDL" Then
                plhEPurseTopUpDropDownList.Visible = True
                plhEPurseTopUpTextBox.Visible = False
                lblEPurseTopUpAmountDropDownList.Text = _wfrPage.Content("EPurseTopUpAmount", _languageCode, True)
                bindTopUpDropDownList()
            ElseIf _wfrPage.Attribute("TopUpTextOrDropDownList") = "TEXT" Then
                plhEPurseTopUpDropDownList.Visible = False
                plhEPurseTopUpTextBox.Visible = True
                lblEPurseTopUpAmountText.Text = _wfrPage.Content("EPurseTopUpAmount", _languageCode, True)
            End If
        End If
        If ddlSelectCard.Items.Count = 0 Then
            plhEPurseCardSelectionandValues.Visible = False
        End If
        If ddlSelectCard.Items.Count > 1 Then
            ltlEPurseCardNumberLabel.Visible = False
            ltlPleaseChooseCard.Visible = True
        Else
            ltlEPurseCardNumberLabel.Visible = True
            ltlPleaseChooseCard.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Populate the top up drop down list based on the number of items to show and the amount multiplier
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub bindTopUpDropDownList()
        ddlEPurseTopUpAmount.Items.Clear()
        Dim itemsToBind As Integer = 10
        Integer.TryParse(_wfrPage.Attribute("TopUpDropDownListNoOfItems"), itemsToBind)
        For count As Integer = 1 To itemsToBind Step 1
            ddlEPurseTopUpAmount.Items.Add(_topUpAmountMultiples * count)
        Next
    End Sub
    Protected Sub ddlSelectCard_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSelectCard.SelectedIndexChanged
        retrieveCardDetails()
    End Sub
#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Call TALENT to retrieve the card information for the current customer. Only retrieve from cache when a post back has occurred.
    ''' </summary>
    ''' <returns>True if a valid card has been found</returns>
    ''' <remarks></remarks>
    Private Function retrieveCardDetails() As Boolean
        Dim validCardData As Boolean = True
        Dim err As New ErrorObj
        Dim tp As New TalentPayment
        tp.De.CustomerNumber = Profile.User.Details.LoginID
        tp.Settings = TEBUtilities.GetSettingsObject()
        tp.Settings.Cacheing = IsPostBack
        tp.Settings.CacheTimeMinutes = 5
        tp.RetreiveEPurseCardNumberFromCache = IsPostBack
        tp.De.CardNumber = ddlSelectCard.SelectedItem.ToString
        err = tp.RetrieveEPurseTotal()

        blErrorMessage.Items.Clear()
        If Not err.HasError AndAlso tp.ResultDataSet IsNot Nothing AndAlso tp.ResultDataSet.Tables.Contains("EPurse") Then
            If tp.ResultDataSet.Tables("StatusResults").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                Dim errorCode As String = tp.ResultDataSet.Tables("StatusResults").Rows(0)("ReturnCode")
                Dim errMsg As New TalentErrorMessages(_languageCode, _wfrPage.BusinessUnit, _wfrPage.PartnerCode, _wfrPage.FrontEndConnectionString)
                blErrorMessage.Items.Add(errMsg.GetErrorMessage(String.Empty, _wfrPage.PageCode, errorCode).ERROR_MESSAGE)
                validCardData = False
            Else
                If tp.ResultDataSet.Tables("EPurse").Rows.Count > 0 Then
                    _cardNumber = tp.De.CardNumber
                    If _cardNumber.Length > 0 Then
                        _cardAmount = tp.ResultDataSet.Tables("EPurse").Rows(0)("EPurseValue").ToString()
                        _cardAmountToSpend = tp.ResultDataSet.Tables("EPurse").Rows(0)("EPurseAllowToSpendValue").ToString()
                        validCardData = True
                    Else
                        blErrorMessage.Items.Add(_wfrPage.Content("ErrorRetrievingCard", _languageCode, True))
                        validCardData = False
                    End If
                Else
                    blErrorMessage.Items.Add(_wfrPage.Content("CardHasNoFundsError", _languageCode, True))
                    validCardData = False
                End If
            End If
        Else
            blErrorMessage.Items.Add(_wfrPage.Content("ErrorRetrievingCard", _languageCode, True))
            validCardData = False
        End If
        Return validCardData
    End Function

    Protected Sub retrieveCardList()
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
        ts.Settings = TEBUtilities.GetSettingsObject()
        'F 
        'ts.Settings.Cacheing = ucr.Attribute("Cacheing")
        'ts.Settings.CacheTimeMinutes = ucr.Attribute("CacheTimeMinutes")
        err = ts.RetrieveSmartcardCards
        '
        ' Populate the drop down lists
        If Not err.HasError AndAlso _
            ts.ResultDataSet.Tables.Count = 2 AndAlso _
            ts.ResultDataSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then

            'Clear the current list
            Me.ddlSelectCard.Items.Clear()

            'Display the no card message when empty
            If ts.ResultDataSet.Tables("Smartcards").Rows.Count = 0 Then
                'TODO ErrorList.Items.Add(ucr.Content("NoEPurseCards", _languageCode, True))
            End If
         


            'loop through the returned data table 
            Dim selected As Boolean = False

            For Each dr As DataRow In ts.ResultDataSet.Tables("Smartcards").Rows
                Dim li As String = dr("CardNumber").ToString.TrimStart("0")
                Me.ddlSelectCard.Items.Add(li)
            Next
        End If
    End Sub

#End Region

    Protected Sub Page_PreRender1(sender As Object, e As EventArgs) Handles Me.PreRender
        Dim hasValidCard As Boolean
        If Not IsPostBack Then
            retrieveCardList()
        End If
        If ddlSelectCard.Items.Count > 0 Then
            hasValidCard = retrieveCardDetails()
        Else
            Dim errMsg As New TalentErrorMessages(_languageCode, _wfrPage.BusinessUnit, _wfrPage.PartnerCode, _wfrPage.FrontEndConnectionString)
            blErrorMessage.Items.Add(errMsg.GetErrorMessage(String.Empty, _wfrPage.PageCode, "MF").ERROR_MESSAGE)
        End If
        setCardInformation(hasValidCard)
        setPageDefaults(hasValidCard)
    End Sub
End Class