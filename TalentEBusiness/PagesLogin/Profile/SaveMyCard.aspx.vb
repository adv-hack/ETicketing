Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities

Partial Class PagesLogin_Profile_SaveMyCard
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfr As New WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private Const KEYCODE As String = "SaveMyCard.aspx"

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _wfr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = ProfileHelper.GetPageName
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = KEYCODE
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        plhErrorMessage.Visible = False
        plhSuccessMessage.Visible = False
        blErrorMessageList.Items.Clear()
        plhSuccessMessage.Visible = False
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If ModuleDefaults.UseSaveMyCard And Not Profile.IsAnonymous Then
            If Not Page.IsPostBack Then savedCardsList.RetrieveMySavedCards()
            If savedCardsList.CantSaveAnymoreCards Or Not CBool(_wfr.Attribute("AddACardFucntion")) Then
                plhAddACard.Visible = False
            Else
                plhAddACard.Visible = True
                ltlAddACardIntroText.Text = _wfr.Content("AddACardIntroText", _languageCode, True)
                If ltlAddACardIntroText.Text.Length > 0 Then
                    plhAddACardIntroText.Visible = True
                Else
                    plhAddACardIntroText.Visible = False
                End If
                If ModuleDefaults.PaymentGatewayType.ToUpper = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
                    plhCardDetail.Visible = False
                    plhCardDetailToVG.Visible = True
                    btnSaveThisCardVG.Text = _wfr.Content("SaveThisCardButtonText", _languageCode, True)
                Else
                    plhCardDetail.Visible = True
                    plhCardDetailToVG.Visible = False
                    payDetails.HideSecurityNumber = True
                    payDetails.NoSavedCardDetails = Not savedCardsList.UserHasSavedCards
                    btnSaveThisCard.Text = _wfr.Content("SaveThisCardButtonText", _languageCode, True)
                End If

            End If
        Else
            Response.Redirect("~/PagesPublic/Error/Unavailable.aspx")
        End If
    End Sub

    Protected Sub btnSaveThisCard_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveThisCard.Click
        If payDetails.ValidateUserInput(True) Then
            Dim startDate As String = FormatCardDate(payDetails.StartMonthDDL.SelectedValue, payDetails.StartYearDDL.SelectedValue, True)
            Dim expiryDate As String = FormatCardDate(payDetails.ExpiryMonthDDL.SelectedValue, payDetails.ExpiryYearDDL.SelectedValue, True)
            saveThisCard(payDetails.CardNumberBox.Text.Trim, startDate, expiryDate, payDetails.SetAsDefault)
        Else
            plhErrorMessageList.Visible = True
            For Each item As ListItem In payDetails.ErrorMessages.Items
                blErrorMessageList.Items.Add(item)
            Next
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Saves the current card details to the card details file in TALENT against this customer
    ''' </summary>
    ''' <param name="cardNumber">The given card number string</param>
    ''' <param name="expiryDate">The 4 digit formatted card expiry date</param>
    ''' <param name="startDate">The 4 digit formatted card start date</param>
    ''' <param name="setAsDefault">Whether to set the current card details as the default or not</param>
    ''' <remarks></remarks>
    Private Sub saveThisCard(ByVal cardNumber As String, ByVal startDate As String, ByVal expiryDate As String, ByVal setAsDefault As Boolean)
        Dim err As New ErrorObj
        Dim payment As New TalentPayment
        Dim settings As New DESettings
        Dim dePayment As New DEPayments
        Dim hasCardBeenSaved As Boolean = False
        With settings
            .FrontEndConnectionString = _wfr.FrontEndConnectionString
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
            .OriginatingSourceCode = "W"
        End With
        If ModuleDefaults.PaymentGatewayType = GlobalConstants.PAYMENTGATEWAY_VANGUARD Then
            dePayment.PaymentType = GlobalConstants.PAYMENTTYPE_VANGUARD
        End If
        dePayment.CardNumber = cardNumber
        dePayment.StartDate = startDate
        dePayment.ExpiryDate = expiryDate
        If setAsDefault Then
            dePayment.DefaultCard = True
        End If
        dePayment.CustomerNumber = Profile.UserName
        dePayment.SessionId = Profile.Basket.Basket_Header_ID
        dePayment.Source = "W"
        payment.De = dePayment
        payment.Settings = settings
        err = payment.SaveMyCard

        Try
            If Not err.HasError Then
                If payment.ResultDataSet.Tables.Count > 1 Then
                    If payment.ResultDataSet.Tables(0).Rows.Count > 0 Then
                        If String.IsNullOrWhiteSpace(payment.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString) Then
                            hasCardBeenSaved = True
                        End If
                    Else
                        hasCardBeenSaved = True
                    End If
                End If
            End If
        Catch ex As Exception
            hasCardBeenSaved = False
        End Try

        If hasCardBeenSaved Then
            payment.RetrieveMySavedCardsClearCache()
            ltlSuccessMessage.Text = _wfr.Content("CardAddedSuccessMessage", _languageCode, True)
            plhSuccessMessage.Visible = True
            savedCardsList.PopulateSavedCardDDL()
            If savedCardsList.CantSaveAnymoreCards Then
                plhAddACard.Visible = False
            Else
                plhAddACard.Visible = True
                payDetails.ResetCCForm()
            End If
        Else
            ltlErrorMessage.Text = _wfr.Content("CardNotAddedErrorMessage", _languageCode, True)
            If CheckForDBNull_String(payment.ResultDataSet.Tables(0).Rows(0)("ReturnCode").ToString) = "SL" Then
                ltlErrorMessage.Text = _wfr.Content("CardNotAddedMaxSavedErrorMessage", _languageCode, True)
            End If
            plhErrorMessage.Visible = True
        End If
    End Sub

#End Region
End Class