Imports System.Data
Imports Talent.Common
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities
Partial Class UserControls_BasketSummary
    Inherits ControlBase

    Private _ucr As UserControlResource = Nothing
    Private _languageCode As String = Nothing
    Private _basketSummaryUpdated As Boolean = False
    Private _validPageToShowInclBtn As Boolean = True
    Private _pageCode As String = String.Empty
    Private _pricesVisible As Boolean = True
    Public ReadOnly Property BasketSummaryUpdated As Boolean
        Get
            Return _basketSummaryUpdated
        End Get
    End Property

    Public ReadOnly Property PartPaymentOptionsChanged() As Boolean
        Get
            Return uscPartPayments.PartPaymentOptionsChanged
        End Get
    End Property

    Public Property RemovePartPaymentAllowed() As Boolean = False

    Public Sub ReBindPartPayments()
        Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
        uscPartPayments.ReBindPartPayments()
    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        _ucr = New UserControlResource
        _languageCode = TCUtilities.GetDefaultLanguage
        _pageCode = Talent.eCommerce.Utilities.GetCurrentPageName
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = _pageCode
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "BasketSummary.ascx"
        End With
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If _pageCode.ToLower.Contains("checkoutorderconfirmation.aspx") Then
            _validPageToShowInclBtn = False
        End If
        ltlBasketSummaryTitle.Text = _ucr.Content("BasketSummaryTitle", _languageCode, True)
        btnRefundAll.Text = _ucr.Content("RefundAllPayments", _languageCode, True)
        uscPartPayments.RemovePartPaymentAllowed = RemovePartPaymentAllowed
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If (Profile.PartnerInfo.Details IsNot Nothing) Then
            _pricesVisible = Not TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(Profile.PartnerInfo.Details.HIDE_PRICES)
        End If
        BindBasketSummary()
        Dim OnAccountFeeItem As TalentBasketItem = Profile.Basket.BasketItems.Find(Function(basketDetailItem As TalentBasketItem) basketDetailItem.Product = ModuleDefaults.CashBackFeeCode)
        If RemovePartPaymentAllowed AndAlso (Profile.Basket.BasketSummary.TotalPartPayments > 0 OrElse OnAccountFeeItem IsNot Nothing) Then
            plhRefundAll.Visible = True
        Else
            plhRefundAll.Visible = False
        End If
    End Sub

    Protected Sub rptBasketSummary_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptBasketSummary.ItemCommand
        If e.CommandName = "ProcessInclude" Then
            Dim productCode As String = CType(e.Item.FindControl("hidSummaryProduct"), HiddenField).Value
            Dim feeCategory As String = CType(e.Item.FindControl("hidFeeCategory"), HiddenField).Value
            Dim isIncluded As Boolean = CType(CType(e.Item.FindControl("hidIsIncluded"), HiddenField).Value, Boolean)
            Dim currentBasketTotal As Decimal = Profile.Basket.BasketSummary.TotalBasket
            If (Not String.IsNullOrWhiteSpace(productCode)) AndAlso (Not String.IsNullOrWhiteSpace(feeCategory)) Then
                If TDataObjects.BasketSettings.TblBasketDetail.UpdateOrDeleteFeeForIncludeStatus(Profile.Basket.Basket_Header_ID, productCode, feeCategory) > 0 Then
                    ProcessBasketTotalChange(currentBasketTotal, feeCategory)
                    _basketSummaryUpdated = True
                Else
                    'error in update the fee include status
                End If
            Else
                'error in fetching the value
            End If
        End If
    End Sub

    Protected Sub rptBasketSummary_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptBasketSummary.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            If AgentProfile.IsAgent Then
                AgentProfile_ItemDataBound(sender, e)
            Else
                If CBool(e.Item.DataItem("IS_DISPLAY_TYPE")) Then
                    CType(e.Item.FindControl("ltlSummaryItemLabel"), Literal).Text = GetFormattedDisplay("label", _ucr.Content(e.Item.DataItem("LABEL_CODE"), _languageCode, True, e.Item.DataItem("LABEL_CODE")), TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(e.Item.DataItem("IS_INCLUDED")))
                    CType(e.Item.FindControl("ltlSummaryItemLabel"), Literal).Visible = _pricesVisible
                    CType(e.Item.FindControl("ltlSummaryItemValue"), Literal).Text = GetFormattedDisplay("value", TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(e.Item.DataItem("GROSS_PRICE"), 0.01, False), _ucr.BusinessUnit, _ucr.PartnerCode), TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(e.Item.DataItem("IS_INCLUDED")))
                    CType(e.Item.FindControl("ltlSummaryItemValue"), Literal).Visible = _pricesVisible
                    CType(e.Item.FindControl("lnkButtonForInclude"), LinkButton).Visible = False
                    CType(e.Item.FindControl("iconForIncludeButton"), HtmlControl).Visible = False
                    Dim hplMerchandisePromotions As HyperLink = CType(e.Item.FindControl("hplMerchandisePromotions"), HyperLink)
                    If TEBUtilities.CheckForDBNull_String(e.Item.DataItem("LABEL_CODE")) = GlobalConstants.BSKTSMRY_TOTAL_MERCHANDISE_PROMOTIONS_PRICE Then
                        hplMerchandisePromotions.Visible = True
                        Dim imgMerchandisePromotions As Image = CType(e.Item.FindControl("imgMerchandisePromotions"), Image)
                        imgMerchandisePromotions.AlternateText = _ucr.Content("MerchandisePromotionAppliedAltText", _languageCode, True)
                        imgMerchandisePromotions.ImageUrl = Utilities.CheckForDBNull_String(_ucr.Attribute("PromotionIconUrl"))
                        hplMerchandisePromotions.NavigateUrl = "~/PagesPublic/Basket/PromotionDetails.aspx"
                    Else
                        hplMerchandisePromotions.Visible = False
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub AgentProfile_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)
        If CBool(e.Item.DataItem("IS_DISPLAY_TYPE")) Then
            CType(e.Item.FindControl("ltlSummaryItemLabel"), Literal).Text = GetFormattedDisplay("label", _ucr.Content(e.Item.DataItem("LABEL_CODE"), _languageCode, True, e.Item.DataItem("LABEL_CODE")), TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(e.Item.DataItem("IS_INCLUDED")))
            CType(e.Item.FindControl("ltlSummaryItemValue"), Literal).Text = GetFormattedDisplay("value", TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(e.Item.DataItem("GROSS_PRICE"), 0.01, False), _ucr.BusinessUnit, _ucr.PartnerCode), TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(e.Item.DataItem("IS_INCLUDED")))
            Dim lnkButtonForInclude As LinkButton = CType(e.Item.FindControl("lnkButtonForInclude"), LinkButton)
            CType(e.Item.FindControl("hidSummaryProduct"), HiddenField).Value = TEBUtilities.CheckForDBNull_String(e.Item.DataItem("SUMMARY_PRODUCT"))
            CType(e.Item.FindControl("hidFeeCategory"), HiddenField).Value = TEBUtilities.CheckForDBNull_String(e.Item.DataItem("FEE_CATEGORY"))
            CType(e.Item.FindControl("hidIsIncluded"), HiddenField).Value = TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(e.Item.DataItem("IS_INCLUDED")).ToString
            lnkButtonForInclude.Visible = (_validPageToShowInclBtn AndAlso TEBUtilities.CheckForDBNull_String(e.Item.DataItem("FEE_CATEGORY")).Trim.Length > 0)
            If lnkButtonForInclude.Visible AndAlso TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(e.Item.DataItem("IS_INCLUDED")) Then
                CType(e.Item.FindControl("iconForIncludeButton"), HtmlControl).Attributes.Add("class", "fa fa-times")
            Else
                CType(e.Item.FindControl("iconForIncludeButton"), HtmlControl).Attributes.Add("class", "fa fa-plus")
            End If
        End If
    End Sub

    Private Sub BindBasketSummary()
        If HttpContext.Current.Profile IsNot Nothing Then
            If Profile.Basket.BasketSummary.SummaryTable IsNot Nothing Then
                rptBasketSummary.DataSource = Profile.Basket.BasketSummary.SummaryTable
                rptBasketSummary.DataBind()
                BindOverallTotals()
                rptBasketSummary.Visible = True
                plhBasketSummary.Visible = True
            End If
        End If
    End Sub

    Private Sub BindOverallTotals()
        ltlBasketSummaryTotalLabel.Visible = _pricesVisible
        ltlBasketSummaryTotalValue.Visible = _pricesVisible
        If _pricesVisible Then
            ltlBasketSummaryTotalLabel.Text = _ucr.Content("TOTAL_BASKET", _languageCode, True, "TOTALS")
            ltlBasketSummaryTotalValue.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalBasket, 0.01, False), _ucr.BusinessUnit, _ucr.PartnerCode)
        End If
    End Sub

    Private Sub ProcessBasketTotalChange(ByVal currentBasketTotal As Decimal, ByVal feeCategory As String)
        If feeCategory = GlobalConstants.FEECATEGORY_BOOKING Then
            Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
        Else
            Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True, True)
        End If
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

    Private Function GetFormattedDisplay(ByVal styleClassPrefix As String, ByVal displayText As String, ByVal isIncluded As Boolean) As String
        Dim formattedText As String = String.Empty
        If isIncluded Then
            formattedText = "<span class=""" & styleClassPrefix & "-included-true"">" & displayText.Replace("-", "(-)") & "</span>"
        Else
            formattedText = "<span style=""text-decoration: line-through;"" class=""" & styleClassPrefix & "-included-false"">" & displayText.Replace("-", "(-)") & "</span>"
        End If
        Return formattedText
    End Function


    Protected Sub btnRefundAll_Click(sender As Object, e As EventArgs)

        Dim dePayment As New DEPayments()
        dePayment.PartPaymentId = 0
        dePayment.CancelAll = "Y"
        dePayment.Amount = "0.00"
        If Not Profile.User.Details Is Nothing Then
            dePayment.CustomerNumber = Profile.User.Details.LoginID
        End If
        dePayment.SessionId = Profile.Basket.Basket_Header_ID
        dePayment.Source = "W"

        Dim Err As ErrorObj
        Dim resultSet As DataSet

        Dim tp As New TalentPayment()
        With tp
            .De = dePayment
            .Settings = TEBUtilities.GetSettingsObject()
            Err = .CancelPartPayment()
            resultSet = .ResultDataSet
            If Err.HasError OrElse resultSet Is Nothing OrElse resultSet.Tables.Count = 0 Then
                HttpContext.Current.Session("TicketingGatewayError") = "XX"
                HttpContext.Current.Session("TalentErrorCode") = "XX"
            ElseIf resultSet.Tables("StatusResults").Rows(0).Item("ErrorOccurred") = "E" Then
                HttpContext.Current.Session("TicketingGatewayError") = resultSet.Tables("StatusResults").Rows(0).Item("ReturnCode")
                HttpContext.Current.Session("TalentErrorCode") = resultSet.Tables("StatusResults").Rows(0).Item("ReturnCode")
            Else

                'Remove onaccount fee
                Dim OnAccountFeeItem As TalentBasketItem = Profile.Basket.BasketItems.Find(Function(basketDetailItem As TalentBasketItem) basketDetailItem.Product = ModuleDefaults.CashBackFeeCode)
                If OnAccountFeeItem IsNot Nothing Then
                    TDataObjects.PaymentSettings.TblBasketDetail.DeleteFee(ModuleDefaults.CashBackFeeCode, Profile.Basket.Basket_Header_ID)
                End If

                'Redirect to load the page again and to prevent an F5 refresh submitting the same request
                ReBindPartPayments()
                Dim canProcessbookingfees As Boolean = TEBUtilities.ProcessPartPayment()
                Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True, canProcessbookingfees)
                Response.Redirect(Request.Url.PathAndQuery)

            End If
        End With
    End Sub



End Class
