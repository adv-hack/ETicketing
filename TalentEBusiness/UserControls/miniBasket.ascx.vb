Imports System.Collections.Generic
Imports System.Data
Imports Talent.eCommerce

Partial Class UserControls_miniBasket
    Inherits ControlBase

    Private ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private ddValid As Boolean = False
    Private cfValid As Boolean = False
    Private ccValid As Boolean = True
    Private Shared defaults As New Talent.eCommerce.ECommerceModuleDefaults
    Private def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Private basketItemsPageBase As List(Of Talent.Common.DEBasketItem)
    Private webPricesPageBase As Talent.Common.TalentWebPricing
    Private _deliveryValue As Decimal = 0
    Private _OnAccountTotal As Decimal = 0
    Private log As Talent.Common.TalentLogging = Utilities.TalentLogging

    Public Enum UsageType As Integer
        TEXT = 0
        GRID = 1
    End Enum

    Private _usage As UsageType

    Public Property Usage() As UsageType
        Get
            Return _usage
        End Get
        Set(ByVal value As UsageType)
            _usage = value
        End Set
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "miniBasket.ascx"
        End With
        Dim showPrices As Boolean = Talent.eCommerce.Utilities.ShowPrices(Profile)

        ' Set visibility of price fields depending on partner/user/default settings

        Select Case Usage
            Case UsageType.TEXT
                MerchandiseTotal_Text.Visible = showPrices
                Total_Text.Visible = showPrices
                TotalLabel_Text.Visible = showPrices
                TicketTotalLabel_Text.Visible = showPrices
                TicketFeeTotalLabel_Text.Visible = showPrices
                TicketSubTotals_Text.Visible = showPrices
                MiniBasketMinimumQuantityLabel_Text.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMinimumPurchaseQuantity_Text"))
                MiniBasketMinimumAmountLabel_Text.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMinimumPurchaseAmount_Text"))

                If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayTicketSubTotals_Text")) Then
                    TicketSubTotals_Text.Visible = Talent.eCommerce.Utilities.ShowPrices(Profile)
                Else
                    TicketSubTotals_Text.Visible = False
                End If
                If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMerchandiseSubTotals_Text")) Then
                    plhMerchandiseSubTotals_Text.Visible = Talent.eCommerce.Utilities.ShowPrices(Profile)
                Else
                    plhMerchandiseSubTotals_Text.Visible = False
                End If


            Case UsageType.GRID
                MerchandiseTotal_Grid.Visible = showPrices
                Total_Grid.Visible = showPrices
                TotalLabel_Grid.Visible = showPrices
                TicketTotalLabel_Grid.Visible = showPrices
                TicketFeeTotalLabel_Grid.Visible = showPrices
                plhTicketSubTotals_Grid.Visible = showPrices
                MiniBasketMinimumQuantityLabel_Grid.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMinimumPurchaseQuantity_Grid"))
                MiniBasketMinimumAmountLabel_Grid.Visible = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMinimumPurchaseAmount_Grid"))

                If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayTicketSubTotals_Grid")) Then
                    plhTicketSubTotals_Grid.Visible = Talent.eCommerce.Utilities.ShowPrices(Profile)
                Else
                    plhTicketSubTotals_Grid.Visible = False
                End If
                If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMerchandiseSubTotals_Grid")) Then
                    plhMerchandiseSubTotals_Grid.Visible = Talent.eCommerce.Utilities.ShowPrices(Profile)
                Else
                    plhMerchandiseSubTotals_Grid.Visible = False
                End If

        End Select

    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        BasedOnPageSetBasket()
        HideAll()
        Select Case Usage
            Case UsageType.TEXT
                SetDisplayForText()
            Case UsageType.GRID
                SetDisplayForGrid()
        End Select
        def = defaults.GetDefaults
        If Not def.AllowCheckoutWithMixedBasket And Profile.Basket.BasketContentType = "C" Then
            CheckoutLinkButton.NavigateUrl = "~/PagesPublic/Basket/Basket.aspx"
        Else
            '            Dim checkoutNavigateUrl As String = "~/PagesLogin/Checkout/CheckoutDeliveryDetails.aspx"
            Dim checkoutNavigateUrl As String = "~/Redirect/TicketingGateway.aspx?page=basket.aspx&function=retailbasketcheckout"
            If Profile.Basket.BasketContentType = GlobalConstants.TICKETINGBASKETCONTENTTYPE Then
                checkoutNavigateUrl = "~/PagesLogin/Checkout/Checkout.aspx"
            End If
            Select Case Usage
                Case UsageType.TEXT
                    If CInt(NoOfItems_Text.Text) > 0 Then
                        CheckoutLinkButton.NavigateUrl = checkoutNavigateUrl
                    Else
                        CheckoutLinkButton.NavigateUrl = "~/PagesPublic/Basket/Basket.aspx"
                    End If
                Case UsageType.GRID
                    If miniBasketRepeater.Items.Count > 0 Then
                        CheckoutLinkButton.NavigateUrl = checkoutNavigateUrl
                    Else
                        CheckoutLinkButton.NavigateUrl = "~/PagesPublic/Basket/Basket.aspx"
                    End If
            End Select
        End If
    End Sub

    Protected Sub DeleteItem(ByVal sender As Object, ByVal e As EventArgs)
        Dim gvr As GridViewRow = CType(sender, LinkButton).Parent
        Dim basket As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        basket.Delete_Basket_Item(Profile.Basket.Basket_Header_ID, CType(gvr.FindControl("MiniBasketProductCodeLabel"), Label).Text)
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)

        Dim obj As Object = sender
        Select Case obj.ID.ToString
            Case Is = "NoOfItemsLabel_Text"
                obj.Text = ucr.Content("NoOfItemsLabelText", _languageCode, True)
            Case Is = "TotalLabel_Text"
                obj.Text = ucr.Content("TotalLabelText", _languageCode, True)
            Case Is = "BasketLinkButton"
                obj.Text = ucr.Content("BasketLinkButtonText", _languageCode, True)
            Case Is = "CheckoutLinkButton"
                obj.Text = ucr.Content("CheckoutLinkButtonText", _languageCode, True)
            Case Is = "TicketTotalLabel_Text"
                obj.Text = ucr.Content("TicketTotalLabelText", _languageCode, True)
            Case Is = "TicketFeeTotalLabel_Text"
                obj.Text = ucr.Content("TicketFeeTotalLabelText", _languageCode, True)
            Case Is = "BuyBackTotalLabel_Text"
                obj.Text = ucr.Content("BuyBackTotalLabelText", _languageCode, True)
            Case Is = "MerchandiseTotalLabel_Text"
                obj.Text = ucr.Content("MerchandiseTotalLabelText", _languageCode, True)


            Case Is = "TotalLabel_Grid"
                obj.Text = ucr.Content("TotalLabelText", _languageCode, True)
            Case Is = "TaxLabel_Grid"
                obj.Text = ucr.Content("TaxLabelText", _languageCode, True)
            Case Is = "TicketTotalLabel_Grid"
                obj.Text = ucr.Content("TicketTotalLabelText", _languageCode, True)
            Case Is = "TicketFeeTotalLabel_Grid"
                obj.Text = ucr.Content("TicketFeeTotalLabelText", _languageCode, True)
            Case Is = "BuyBackTotalLabel_Grid"
                obj.Text = ucr.Content("BuyBackTotalLabelText", _languageCode, True)
            Case Is = "MerchandiseTotalLabel_Grid"
                obj.Text = ucr.Content("MerchandiseTotalLabelText", _languageCode, True)
            Case Is = "ltlOnAccountTotal"
                obj.Text = ucr.Content("ltlOnAccountTotalText", _languageCode, True)
            Case Is = "ltlOnAccountTotal_Grid"
                obj.Text = ucr.Content("ltlOnAccountTotalText", _languageCode, True)
        End Select
    End Sub

    Protected Sub SetDisplayForText()
        plhMiniBasketTextView.Visible = True
        PopulateTotals()
    End Sub

    Protected Sub PopulateTotals()
        Try
            Dim defs As New Talent.eCommerce.ECommerceModuleDefaults
            Dim values As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
            values = defs.GetDefaults

            Dim prices As New Talent.Common.TalentWebPricing(Nothing, Nothing, True)
            Dim qtyAndCodes As New Generic.Dictionary(Of String, Talent.Common.WebPriceProduct)
            Dim items As Integer = 0
            Dim tot As Decimal = 0

            Dim merchTot As Decimal = 0
            Dim doCodesLoop As Boolean = False
            Dim dtAdhocFeesForRepeater As Data.DataTable = createAdhocFeesDataTable()
            Dim totalAdhocFees As Decimal = 0
            Dim isBookingFeeRemoved As Boolean = False

            tot = Profile.Basket.BasketSummary.TotalTicketing
            items = Profile.Basket.BasketSummary.TotalItemsTicketing
            prices = webPricesPageBase

            If Not prices.RetrievedPrices Is Nothing AndAlso prices.RetrievedPrices.Count > 0 Then
                For Each wp As Talent.Common.DEWebPrice In prices.RetrievedPrices.Values
                    items += wp.RequestedQuantity
                Next
            End If

            Select Case Usage
                Case UsageType.TEXT
                    NoOfItems_Text.Text = items
                    Total_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalBasket, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)

                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMerchandiseSubTotals_Text")) Then
                        MerchandiseTotal_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.MerchandiseSubTotal, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                    End If

                Case UsageType.GRID

                    Total_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalBasket, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                    If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE OrElse Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                        Tax_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.MerchandiseVAT, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        plhTaxLabel.Visible = True
                    Else
                        plhTaxLabel.Visible = False
                    End If

                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMerchandiseSubTotals_Grid")) Then
                        MerchandiseTotal_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.MerchandiseSubTotal, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                    End If
            End Select

            Select Case Usage
                Case UsageType.TEXT
                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMinimumPurchaseQuantity_Text")) AndAlso Profile.GetMinimumPurchaseQuantity - items > 0 Then
                        Dim text As String = ucr.Content("MinimumQuantityText", _languageCode, True).Replace("<<quantity>>", Profile.GetMinimumPurchaseQuantity - items)
                        If Profile.GetMinimumPurchaseQuantity - items = 1 Then
                            text = text.Replace("<<s>>", "")
                        Else
                            text = text.Replace("<<s>>", "s")
                        End If
                        MiniBasketMinimumQuantityLabel_Text.Text = text
                    Else
                        plhMinQtyPanel_Text.Visible = False
                        MiniBasketMinimumQuantityLabel_Text.Visible = False
                    End If

                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMinimumPurchaseAmount_Text")) AndAlso Profile.GetMinimumPurchaseAmount - prices.Total_Items_Value_Gross > 0 Then
                        MiniBasketMinimumAmountLabel_Text.Text = ucr.Content("MinimumAmountText", _languageCode, True).Replace("<<amount>>", TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.GetMinimumPurchaseAmount - prices.Total_Items_Value_Gross, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode))
                    Else
                        plhMinValPanel_Text.Visible = False
                        MiniBasketMinimumAmountLabel_Text.Visible = False
                    End If

                Case UsageType.GRID
                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMinimumPurchaseQuantity_Grid")) _
                        AndAlso Profile.GetMinimumPurchaseQuantity - items > 0 Then
                        Dim text As String = ucr.Content("MinimumQuantityText", _languageCode, True).Replace("<<quantity>>", Profile.GetMinimumPurchaseQuantity - items)
                        If Profile.GetMinimumPurchaseQuantity - items = 1 Then
                            text = text.Replace("<<s>>", "")
                        Else
                            text = text.Replace("<<s>>", "s")
                        End If
                        MiniBasketMinimumQuantityLabel_Grid.Text = text
                    Else
                        plhMinQtyPanel_Grid.Visible = False
                        MiniBasketMinimumQuantityLabel_Grid.Visible = False
                    End If

                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMinimumPurchaseAmount_Grid")) Then
                        If Profile.GetMinimumPurchaseAmount - prices.Total_Items_Value_Gross > 0 Then
                            MiniBasketMinimumAmountLabel_Grid.Text = ucr.Content("MinimumAmountText", _languageCode, True).Replace("<<amount>>", TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.GetMinimumPurchaseAmount - prices.Total_Items_Value_Gross, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode))
                            CheckoutLinkButton.Enabled = False
                        Else
                            CheckoutLinkButton.Enabled = True
                            MiniBasketMinimumAmountLabel_Grid.Text = ucr.Content("MinimumAmountSurpassedText", _languageCode, True)
                        End If
                    Else
                        CheckoutLinkButton.Enabled = True
                        plhMinValPanel_Grid.Visible = False
                        MiniBasketMinimumAmountLabel_Grid.Visible = False
                    End If
            End Select

            Select Case Usage
                Case UsageType.TEXT
                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayTicketSubTotals_Text")) Then
                        TicketTotal_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalTicketPrice, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        TicketFeeTotal_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalTicketingFees, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        If Profile.Basket.BasketSummary.TotalBuyBack <> 0 Then
                            pblBuyBackTotal_Text.Visible = True
                            BuyBackTotal_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalBuyBack, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        End If

                    End If

                    If Profile.Basket.BasketSummary.TotalOnAccount(values.CashBackFeeCode) <> 0 Then
                        plhCashbackTotal_Text.Visible = True
                        ltlCashbackLabel_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Profile.Basket.BasketSummary.TotalOnAccount(values.CashBackFeeCode), ucr.BusinessUnit, ucr.PartnerCode)
                    End If
                    If values.OnAccountEnabled AndAlso Profile.Basket.BasketSummary.TotalPartPayments <> 0 Then
                        plhOnAccountTotalSummary.Visible = True
                        ltlOnAccountValue.Text = TDataObjects.PaymentSettings.FormatCurrency(Profile.Basket.BasketSummary.TotalPartPayments, ucr.BusinessUnit, ucr.PartnerCode)
                    End If

                Case UsageType.GRID
                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayTicketSubTotals_Grid")) Then
                        TicketTotal_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalTicketPrice, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        TicketFeeTotal_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalTicketingFees, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        If Profile.Basket.BasketSummary.TotalBuyBack <> 0 Then
                            plhBuyBackTotal_Grid.Visible = True
                            BuyBackTotal_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalBuyBack, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        End If
                    End If

                    If Profile.Basket.BasketSummary.TotalOnAccount(values.CashBackFeeCode) <> 0 Then
                        plhCashbackTotal_Grid.Visible = True
                        ltlCashbackLabel_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Profile.Basket.BasketSummary.TotalOnAccount(values.CashBackFeeCode), ucr.BusinessUnit, ucr.PartnerCode)
                    End If

                    If values.OnAccountEnabled AndAlso Profile.Basket.BasketSummary.TotalPartPayments <> 0 Then
                        plhOnAccountTotalSummary_Grid.Visible = True
                        ltlOnAccountValue_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Profile.Basket.BasketSummary.TotalPartPayments, ucr.BusinessUnit, ucr.PartnerCode)
                    End If
            End Select
        Catch ex As Exception

            Select Case Usage
                Case UsageType.TEXT
                    NoOfItems_Text.Text = 0
                    Total_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(0, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                Case UsageType.GRID
                    If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE OrElse Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                        Tax_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(0, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        plhTaxLabel.Visible = True
                    Else
                        plhTaxLabel.Visible = False
                    End If
                    Total_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(0, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)

            End Select


        End Try
    End Sub

    Protected Sub PopulateTotals_OLD1()
        Try
            Dim defs As New Talent.eCommerce.ECommerceModuleDefaults
            Dim values As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
            values = defs.GetDefaults

            Dim prices As New Talent.Common.TalentWebPricing(Nothing, Nothing, True)
            Dim qtyAndCodes As New Generic.Dictionary(Of String, Talent.Common.WebPriceProduct)
            Dim items As Integer = 0
            Dim tot As Decimal = 0

            Dim merchTot As Decimal = 0
            Dim doCodesLoop As Boolean = False
            Dim dtAdhocFeesForRepeater As Data.DataTable = createAdhocFeesDataTable()
            Dim totalAdhocFees As Decimal = 0
            Dim isBookingFeeRemoved As Boolean = False

            tot = Profile.Basket.BasketSummary.TotalTicketing
            items = Profile.Basket.BasketSummary.TotalItemsTicketing
            prices = webPricesPageBase

            If Not prices.RetrievedPrices Is Nothing AndAlso prices.RetrievedPrices.Count > 0 Then
                For Each wp As Talent.Common.DEWebPrice In prices.RetrievedPrices.Values
                    items += wp.RequestedQuantity
                Next
            End If

            Select Case Usage
                Case UsageType.TEXT
                    NoOfItems_Text.Text = items
                    If values.ShowPricesExVAT Then
                        Total_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue((prices.Total_Items_Value_Net - prices.Total_Promotions_Value) + tot + _deliveryValue, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                    Else
                        Total_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue((prices.Total_Items_Value_Gross - prices.Total_Promotions_Value) + tot + _deliveryValue, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                    End If

                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMerchandiseSubTotals_Text")) Then
                        If values.ShowPricesExVAT Then
                            MerchandiseTotal_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue((prices.Total_Items_Value_Net - prices.Total_Promotions_Value), 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        Else
                            MerchandiseTotal_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue((prices.Total_Items_Value_Gross - prices.Total_Promotions_Value), 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        End If
                    End If

                Case UsageType.GRID
                    If values.ShowPricesExVAT Then
                        Total_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue((prices.Total_Items_Value_Net - prices.Total_Promotions_Value) + tot + _deliveryValue, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                    Else
                        Total_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue((prices.Total_Items_Value_Gross - prices.Total_Promotions_Value) + tot + _deliveryValue, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                    End If

                    If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE OrElse Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                        Tax_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(prices.Total_Items_Value_Tax + tot, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        plhTaxLabel.Visible = True
                    Else
                        plhTaxLabel.Visible = False
                    End If

                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMerchandiseSubTotals_Grid")) Then
                        If values.ShowPricesExVAT Then
                            MerchandiseTotal_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue((prices.Total_Items_Value_Net - prices.Total_Promotions_Value), 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        Else
                            MerchandiseTotal_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue((prices.Total_Items_Value_Gross - prices.Total_Promotions_Value), 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        End If
                    End If
            End Select

            Select Case Usage
                Case UsageType.TEXT
                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMinimumPurchaseQuantity_Text")) AndAlso Profile.GetMinimumPurchaseQuantity - items > 0 Then
                        Dim text As String = ucr.Content("MinimumQuantityText", _languageCode, True).Replace("<<quantity>>", Profile.GetMinimumPurchaseQuantity - items)
                        If Profile.GetMinimumPurchaseQuantity - items = 1 Then
                            text = text.Replace("<<s>>", "")
                        Else
                            text = text.Replace("<<s>>", "s")
                        End If
                        MiniBasketMinimumQuantityLabel_Text.Text = text
                    Else
                        plhMinQtyPanel_Text.Visible = False
                        MiniBasketMinimumQuantityLabel_Text.Visible = False
                    End If

                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMinimumPurchaseAmount_Text")) AndAlso Profile.GetMinimumPurchaseAmount - prices.Total_Items_Value_Gross > 0 Then
                        MiniBasketMinimumAmountLabel_Text.Text = ucr.Content("MinimumAmountText", _languageCode, True).Replace("<<amount>>", TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.GetMinimumPurchaseAmount - prices.Total_Items_Value_Gross, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode))
                    Else
                        plhMinValPanel_Text.Visible = False
                        MiniBasketMinimumAmountLabel_Text.Visible = False
                    End If

                Case UsageType.GRID
                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMinimumPurchaseQuantity_Grid")) _
                        AndAlso Profile.GetMinimumPurchaseQuantity - items > 0 Then
                        Dim text As String = ucr.Content("MinimumQuantityText", _languageCode, True).Replace("<<quantity>>", Profile.GetMinimumPurchaseQuantity - items)
                        If Profile.GetMinimumPurchaseQuantity - items = 1 Then
                            text = text.Replace("<<s>>", "")
                        Else
                            text = text.Replace("<<s>>", "s")
                        End If
                        MiniBasketMinimumQuantityLabel_Grid.Text = text
                    Else
                        plhMinQtyPanel_Grid.Visible = False
                        MiniBasketMinimumQuantityLabel_Grid.Visible = False
                    End If

                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMinimumPurchaseAmount_Grid")) Then
                        If Profile.GetMinimumPurchaseAmount - prices.Total_Items_Value_Gross > 0 Then
                            MiniBasketMinimumAmountLabel_Grid.Text = ucr.Content("MinimumAmountText", _languageCode, True).Replace("<<amount>>", TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.GetMinimumPurchaseAmount - prices.Total_Items_Value_Gross, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode))
                            CheckoutLinkButton.Enabled = False
                        Else
                            CheckoutLinkButton.Enabled = True
                            MiniBasketMinimumAmountLabel_Grid.Text = ucr.Content("MinimumAmountSurpassedText", _languageCode, True)
                        End If
                    Else
                        CheckoutLinkButton.Enabled = True
                        plhMinValPanel_Grid.Visible = False
                        MiniBasketMinimumAmountLabel_Grid.Visible = False
                    End If
            End Select

            Select Case Usage
                Case UsageType.TEXT
                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayTicketSubTotals_Text")) Then
                        TicketTotal_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalTicketPrice, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        TicketFeeTotal_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalTicketingFees, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        If Profile.Basket.BasketSummary.TotalBuyBack <> 0 Then
                            pblBuyBackTotal_Text.Visible = True
                            BuyBackTotal_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalBuyBack, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        End If

                    End If

                    If Profile.Basket.BasketSummary.TotalOnAccount(values.CashBackFeeCode) <> 0 Then
                        plhCashbackTotal_Text.Visible = True
                        ltlCashbackLabel_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Profile.Basket.BasketSummary.TotalOnAccount(values.CashBackFeeCode), ucr.BusinessUnit, ucr.PartnerCode)
                    End If
                    If values.OnAccountEnabled AndAlso Profile.Basket.BasketSummary.TotalPartPayments <> 0 Then
                        plhOnAccountTotalSummary.Visible = True
                        ltlOnAccountValue.Text = TDataObjects.PaymentSettings.FormatCurrency(Profile.Basket.BasketSummary.TotalPartPayments, ucr.BusinessUnit, ucr.PartnerCode)
                    End If

                Case UsageType.GRID
                    If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayTicketSubTotals_Grid")) Then
                        TicketTotal_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalTicketPrice, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        TicketFeeTotal_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalTicketingFees, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        If Profile.Basket.BasketSummary.TotalBuyBack <> 0 Then
                            plhBuyBackTotal_Grid.Visible = True
                            BuyBackTotal_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Profile.Basket.BasketSummary.TotalBuyBack, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        End If
                    End If

                    If Profile.Basket.BasketSummary.TotalOnAccount(values.CashBackFeeCode) <> 0 Then
                        plhCashbackTotal_Grid.Visible = True
                        ltlCashbackLabel_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Profile.Basket.BasketSummary.TotalOnAccount(values.CashBackFeeCode), ucr.BusinessUnit, ucr.PartnerCode)
                    End If

                    If values.OnAccountEnabled AndAlso Profile.Basket.BasketSummary.TotalPartPayments <> 0 Then
                        plhOnAccountTotalSummary_Grid.Visible = True
                        ltlOnAccountValue_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Profile.Basket.BasketSummary.TotalPartPayments, ucr.BusinessUnit, ucr.PartnerCode)
                    End If
            End Select
        Catch ex As Exception

            Select Case Usage
                Case UsageType.TEXT
                    NoOfItems_Text.Text = 0
                    Total_Text.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(0, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                Case UsageType.GRID
                    If Profile.Basket.BasketContentType = GlobalConstants.MERCHANDISEBASKETCONTENTTYPE OrElse Profile.Basket.BasketContentType = GlobalConstants.COMBINEDBASKETCONTENTTYPE Then
                        Tax_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(0, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                        plhTaxLabel.Visible = True
                    Else
                        plhTaxLabel.Visible = False
                    End If
                    Total_Grid.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(0, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)

            End Select


        End Try
    End Sub

    Protected Sub SetDisplayForGrid()
        plhMiniBasketGridView.Visible = True
        clearBasketButton.Text = ucr.Content("ClearBasketButtonText_Grid", _languageCode, True)
        If Profile.Basket.BasketContentType <> GlobalConstants.MERCHANDISEBASKETCONTENTTYPE Then
            Dim revisedBasketList As New List(Of TalentBasketItem)
            For Each item As TalentBasketItem In basketItemsPageBase
                If revisedBasketList.Count = 0 Then
                    revisedBasketList.Add(item)
                Else
                    Dim addProduct As Boolean = True
                    For Each newItem As TalentBasketItem In revisedBasketList
                        If newItem.Product = item.Product Then
                            If AgentProfile.BulkSalesMode Then
                                newItem.Quantity += item.Quantity
                            Else
                                newItem.Quantity += 1
                            End If
                            If newItem.MODULE_ = GlobalConstants.BASKETMODULETICKETING Then
                                newItem.Gross_Price += item.Gross_Price
                            End If
                            addProduct = False
                            Exit For
                        End If
                    Next
                    If addProduct Then
                        revisedBasketList.Add(item)
                    End If
                End If
            Next
            miniBasketRepeater.DataSource = revisedBasketList
        Else
            miniBasketRepeater.DataSource = basketItemsPageBase
        End If
        miniBasketRepeater.DataBind()
        PopulateTotals()
    End Sub

    Protected Sub HideAll()
        plhMiniBasketTextView.Visible = False
        plhMiniBasketGridView.Visible = False
    End Sub

    Public Sub ReBindBasket()
        BasedOnPageSetBasket()
        HideAll()
        Select Case Usage
            Case UsageType.TEXT
                SetDisplayForText()
            Case UsageType.GRID
                SetDisplayForGrid()
        End Select
    End Sub

    Public Function ShowNegativeSymbol(ByVal isNegative As Boolean) As String
        If isNegative Then
            Return "&ndash;&nbsp;"
        Else
            Return String.Empty
        End If
    End Function

    Protected Sub miniBasketRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles miniBasketRepeater.ItemDataBound
        If e.Item.ItemIndex > -1 Then

            Dim tbi As TalentBasketItem = CType(e.Item.DataItem, TalentBasketItem)

            Dim qtyWrap As HtmlGenericControl = CType(e.Item.FindControl("qtyWrap"), HtmlGenericControl)
            Dim qtyDividerWrap As HtmlGenericControl = CType(e.Item.FindControl("qtyDividerWrap"), HtmlGenericControl)
            Dim productCodeWrap As HtmlGenericControl = CType(e.Item.FindControl("productCodeWrap"), HtmlGenericControl)
            Dim productNameWrap As HtmlGenericControl = CType(e.Item.FindControl("productNameWrap"), HtmlGenericControl)
            Dim unitPriceWrap As HtmlGenericControl = CType(e.Item.FindControl("unitPriceWrap"), HtmlGenericControl)
            Dim linePriceWrap As HtmlGenericControl = CType(e.Item.FindControl("linePriceWrap"), HtmlGenericControl)
            Dim deleteWrap As HtmlGenericControl = CType(e.Item.FindControl("deleteWrap"), HtmlGenericControl)

            Dim qtyLabel As Label = CType(e.Item.FindControl("qtyLabel"), Label)
            Dim qtyDividerLabel As Label = CType(e.Item.FindControl("qtyDividerLabel"), Label)
            Dim productCodeLabel As Label = CType(e.Item.FindControl("productCodeLabel"), Label)
            Dim productNameLabel As Label = CType(e.Item.FindControl("productNameLabel"), Label)
            Dim unitPriceLabel As Label = CType(e.Item.FindControl("unitPriceLabel"), Label)
            Dim linePriceLabel As Label = CType(e.Item.FindControl("linePriceLabel"), Label)
            Dim hfProductCode As HiddenField = CType(e.Item.FindControl("hfProductCode"), HiddenField)

            Dim deleteButton As Button = CType(e.Item.FindControl("deleteButton"), Button)

            qtyWrap.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("ShowQuantity_Grid"))
            qtyDividerWrap.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("ShowQuantityDivider_Grid"))
            productCodeWrap.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("ShowProductCode_Grid"))
            productNameWrap.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("ShowProductName_Grid"))
            unitPriceWrap.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("ShowUnitPrice_Grid"))
            linePriceWrap.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("ShowLinePrice_Grid"))
            deleteWrap.Visible = Utilities.CheckForDBNullOrBlank_Boolean_DefaultTrue(ucr.Attribute("ShowDeleteButton_Grid"))

            qtyLabel.Text = tbi.Quantity.ToString("0.###")
            qtyDividerLabel.Text = ucr.Content("QuantityDividerSymbol_Grid", _languageCode, True)
            productCodeLabel.Text = tbi.Product
            productNameLabel.Text = tbi.PRODUCT_DESCRIPTION1
            unitPriceLabel.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(tbi.Gross_Price, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
            linePriceLabel.Text = TDataObjects.PaymentSettings.FormatCurrency(tbi.Quantity * Utilities.RoundToValue(tbi.Gross_Price, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
            deleteButton.Text = ucr.Content("MiniBasketDeleteButtonText", _languageCode, True)
            hfProductCode.Value = tbi.Product

            If tbi.MODULE_ = GlobalConstants.BASKETMODULETICKETING Then
                qtyDividerLabel.Text = ""
                unitPriceLabel.Text = ""
                linePriceLabel.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(tbi.Gross_Price, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
                If Utilities.IsTicketingFee(tbi.MODULE_, tbi.Product, tbi.FEE_CATEGORY) Then
                    If deleteWrap.Visible Then deleteWrap.Visible = False
                End If
            End If

            If String.IsNullOrEmpty(productNameLabel.Text) AndAlso productNameWrap.Visible = True Then
                Dim dt As Data.DataTable = Utilities.GetProductInfo(tbi.Product)
                If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                    productNameLabel.Text = Utilities.CheckForDBNull_String(dt.Rows(0)("PRODUCT_DESCRIPTION_1"))
                Else
                    If Utilities.IsTicketingFee(tbi.MODULE_, tbi.Product, tbi.FEE_CATEGORY) Then
                        Select Case tbi.Product
                            Case Is = GlobalConstants.BKFEE : productNameLabel.Text = ucr.Content("BookingFeesTotalLabel", _languageCode, True)
                            Case Is = GlobalConstants.CRFEE : productCodeLabel.Text = ucr.Content("CarriageFeesTotalLabel", _languageCode, True)
                            Case Is = GlobalConstants.WSFEE : productCodeLabel.Text = ucr.Content("WebSalesFeesTotalLabel", _languageCode, True)
                            Case Is = GlobalConstants.DDFEE : productCodeLabel.Text = ucr.Content("DirectDebitFeesTotalLabel", _languageCode, True)
                            Case Is = GlobalConstants.ATFEE : e.Item.Visible = False
                        End Select
                    End If
                End If
            End If

        End If
    End Sub

    Protected Sub doDeleteItem(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim ri As RepeaterItem = CType(sender, Button).Parent.Parent
            Dim hfProductCode As HiddenField = CType(ri.FindControl("hfProductCode"), HiddenField)
            Dim productCodeToDelete As String = hfProductCode.Value.ToUpper

            Dim basketItemEntity As TalentBasketItem = Nothing
            For itemIndex As Integer = 0 To Profile.Basket.BasketItems.Count - 1
                If Profile.Basket.BasketItems(itemIndex).Product.ToUpper = productCodeToDelete Then
                    basketItemEntity = Profile.Basket.BasketItems(itemIndex)
                    If basketItemEntity IsNot Nothing Then
                        If basketItemEntity.MODULE_ = GlobalConstants.BASKETMODULETICKETING Then
                            'delete ticket
                            Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
                            ticketingGatewayFunctions.Basket_RemoveFromBasket(False, "", basketItemEntity.Product, basketItemEntity.SEAT, basketItemEntity.PRICE_CODE, basketItemEntity.PACKAGE_ID, basketItemEntity.LOGINID)
                        Else
                            'delete merchandise
                            Dim basketAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
                            basketAdapter.Delete_Basket_Item(Profile.Basket.Basket_Header_ID, hfProductCode.Value)
                        End If
                    End If
                End If
            Next

            're-load the basket and re-bind the repeater
            Profile.Basket = CType(Profile.Provider, TalentProfileProvider).GetBasket(Profile.UserName, Not Profile.IsAnonymous)

            SetDisplayForGrid()
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub clearBasketButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles clearBasketButton.Click
        Dim clearTicketingBasket As Boolean = False
        If Profile.Basket.BasketItems.Count > 0 AndAlso String.IsNullOrWhiteSpace(Profile.Basket.CAT_MODE) Then
            Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
            ticketingGatewayFunctions.Basket_ClearBasket(False)
        End If
        Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
        Profile.Basket.EmptyBasket()
        Session("AllPrmotionCodesEnteredByUser") = Nothing
        Session("AlternativeProducts") = Nothing
        If Not Session("personalisationTransactions") Is Nothing Then
            Session("personalisationTransactions") = Nothing
        End If
        ReBindBasket()
    End Sub

    ''' <summary>
    ''' Based on page set basketitems for mini basket is loaded 
    ''' </summary>
    Private Sub BasedOnPageSetBasket()
        If Utilities.GetCurrentPageName.Trim.ToUpper = UCase("checkoutOrderConfirmation.aspx") Then
            basketItemsPageBase = New List(Of Talent.Common.DEBasketItem)
            webPricesPageBase = Profile.Provider.SetEmptyTalentWebPricing()
        Else
            Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
            basketItemsPageBase = Profile.Basket.BasketItems
            webPricesPageBase = Profile.Basket.WebPrices

            'Dim defs As New Talent.eCommerce.ECommerceModuleDefaults
            'Dim values As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
            'values = defs.GetDefaults
            'Dim qtyAndCodes As New Generic.Dictionary(Of String, Talent.Common.WebPriceProduct)
            'For Each tbi As TalentBasketItem In basketItemsPageBase
            '    If Not UCase(tbi.MODULE_) = "TICKETING" Then
            '        Select Case values.PricingType
            '            Case 2
            '                webPricesPageBase = Utilities.GetPrices_Type2
            '            Case Else
            '                If Not tbi.IS_FREE Then
            '                    If Not String.IsNullOrEmpty(tbi.MASTER_PRODUCT) Then
            '                        'Check to see if the multibuys are configured for this master product
            '                        Dim myPrice As Talent.Common.DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(tbi.MASTER_PRODUCT, 0, tbi.MASTER_PRODUCT)
            '                        If myPrice.SALE_PRICE_BREAK_QUANTITY_1 > 0 OrElse myPrice.PRICE_BREAK_QUANTITY_1 > 0 Then
            '                            'multibuys are configured
            '                            If qtyAndCodes.ContainsKey(tbi.MASTER_PRODUCT) Then
            '                                qtyAndCodes(tbi.MASTER_PRODUCT).Quantity += tbi.Quantity
            '                            Else
            '                                ' Pass in product otherwise Promotions don't work properly
            '                                ' qtyAndCodes.Add(tbi.MASTER_PRODUCT, New Talent.Common.WebPriceProduct(tbi.MASTER_PRODUCT, tbi.Quantity, tbi.MASTER_PRODUCT))
            '                                qtyAndCodes.Add(tbi.MASTER_PRODUCT, New Talent.Common.WebPriceProduct(tbi.Product, tbi.Quantity, tbi.MASTER_PRODUCT))
            '                            End If
            '                        Else
            '                            If Not qtyAndCodes.ContainsKey(tbi.Product) Then
            '                                qtyAndCodes.Add(tbi.Product, New Talent.Common.WebPriceProduct(tbi.Product, tbi.Quantity, tbi.MASTER_PRODUCT))
            '                            End If
            '                        End If
            '                    Else
            '                        If Not qtyAndCodes.ContainsKey(tbi.Product) Then
            '                            qtyAndCodes.Add(tbi.Product, New Talent.Common.WebPriceProduct(tbi.Product, tbi.Quantity, tbi.MASTER_PRODUCT))
            '                        End If
            '                    End If
            '                End If
            '        End Select
            '    End If
            'Next


        End If
    End Sub

    Public Sub RefreshTotalWithDelivery(ByVal deliveryValue As Decimal)
        _deliveryValue = deliveryValue
        ReBindBasket()
    End Sub

    Private Function createAdhocFeesDataTable() As Data.DataTable
        Dim dtAdhocFees As New Data.DataTable
        With dtAdhocFees.Columns
            .Add("FEE_CODE", GetType(String))
            .Add("FEE_DESCRIPTION", GetType(String))
            .Add("FEE_VALUE", GetType(Decimal))
            .Add("IS_NEGATIVE", GetType(Boolean))
        End With
        Return dtAdhocFees
    End Function

    Protected Sub GetOnAccountData_OLD()

        Try
            'Create the results data table
            Dim dtPartPayments As New DataTable
            Dim err As New Talent.Common.ErrorObj
            _OnAccountTotal = 0
            '
            ' Retrieve the total credit on the e-purse
            '
            Dim tp As New Talent.Common.TalentPayment
            Dim dePayment As New Talent.Common.DEPayments
            With dePayment
                .SessionId = Profile.Basket.Basket_Header_ID.ToString
                If Not Profile.User.Details Is Nothing Then
                    .CustomerNumber = Profile.User.Details.LoginID
                End If
            End With
            '
            tp.De = dePayment
            tp.Settings = Utilities.GetSettingsObject()
            tp.Settings.Cacheing = Utilities.CheckForDBNull_Boolean_DefaultTrue(ucr.Attribute("OnAccountCacheing"))
            tp.Settings.CacheTimeMinutes = Utilities.CheckForDBNull_Int(ucr.Attribute("OnAccountCacheTimeMinutes"))
            err = tp.RetrievePartPayments()

            ' Was the call successful
            If Not err.HasError AndAlso _
                Not tp.ResultDataSet Is Nothing AndAlso _
                tp.ResultDataSet.Tables.Count = 2 AndAlso _
                    tp.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then

                'Populate the new data table
                dtPartPayments = tp.ResultDataSet.Tables("PartPayments")

                'Retrieve the part payment totals
                For Each dr As DataRow In dtPartPayments.Rows
                    _OnAccountTotal = _OnAccountTotal + CType(dr.Item("PaymentAmount"), Decimal)
                Next
            End If
        Catch ex As Exception
            log.ExceptionLog("MiniBasket.ascx-GetAccountData", ex.Message)
        End Try


    End Sub

    Private Function GetCATSeatPrice() As Decimal
        'Dim catSeatPrice As Decimal = 0
        'Dim catBasketControl As UserControls_CATBasket = CType(Talent.eCommerce.Utilities.FindWebControl("CATBasket1", Me.Page.Controls), UserControls_CATBasket)
        'If catBasketControl IsNot Nothing Then
        '    catSeatPrice = catBasketControl.CatSeatsPrice
        'End If
        'Return catSeatPrice
        Return Session.Item("CatSeatsPrice")
    End Function

    ''' <summary>
    ''' Format the currency based on the given value
    ''' </summary>
    ''' <param name="value">The given value</param>
    ''' <returns>The formatted value</returns>
    ''' <remarks></remarks>
    Public Function FormatCurrency(ByVal value As Decimal) As String
        Dim formattedValue As String = value
        formattedValue = TDataObjects.PaymentSettings.FormatCurrency(value, ucr.BusinessUnit, ucr.PartnerCode)
        Return formattedValue
    End Function

End Class
