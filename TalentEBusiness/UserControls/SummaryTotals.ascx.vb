Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Summary Totals
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCCODA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_SummaryTotals
    Inherits ControlBase

    Dim ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _usage As String
    Private _deliveryValue As Decimal = 0
    Private _merchandiseTotal As Decimal
    Const sqlDatabase As String = "SQL2005"

    Public Property Usage() As String
        Get
            Return _usage
        End Get
        Set(ByVal value As String)
            _usage = value
        End Set
    End Property

    Public Property MerchandiseTotal() As Decimal
        Get
            Return _merchandiseTotal
        End Get
        Set(ByVal value As Decimal)
            _merchandiseTotal = value
        End Set
    End Property

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "SummaryTotals.ascx"
        End With
    End Sub

    'Perform task as late as possible so promo property can be set by another control
    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Not Page.IsPostBack AndAlso Talent.eCommerce.Utilities.ShowPrices(Profile) Then
            SetLabelText()
            SetUpTotals()
        End If
        If Utilities.GetCurrentPageName().ToLower() = "checkout.aspx" Then
            Me.Visible = False
        End If
    End Sub

    Public Sub SetUpTotals()
        Select Case UCase(Usage)
            Case Is = "BASKET", "ORDERSUMMARY", "PAYMENT", "ORDER"
                SetPreOrderTotals()
            Case Is = "ORDERTEMPLATE"
                SetOrderTemplateTotals()
            Case Is = "ORDERENQUIRY"
                SetOrderEnquiryTotals()
        End Select
    End Sub

    Protected Sub SetLabelText()
        With ucr
            SubTotalLabel.Text = .Content("SubTotalLabel", _languageCode, True)
            DeliveryLabel.Text = .Content("DeliveryLabel", _languageCode, True)
            PromotionLabel.Text = .Content("PromotionLabel", _languageCode, True)
            Tax1Label.Text = .Content("Tax1Label", _languageCode, True)
            Tax2Label.Text = .Content("Tax2Label", _languageCode, True)
            Tax3Label.Text = .Content("Tax3Label", _languageCode, True)
            Tax4Label.Text = .Content("Tax4Label", _languageCode, True)
            Tax5Label.Text = .Content("Tax5Label", _languageCode, True)
            TotalLabel.Text = .Content("TotalLabel", _languageCode, True)
            TotalsHeaderLabel.Text = .Content("TotalsHeaderLabel", _languageCode, True)

        End With
    End Sub

    Protected Sub SetOrderTemplateTotals()

        'Dim details As New OrderTemplatesDataSetTableAdapters.ProductDetailsTableAdapter
        'Dim products As OrderTemplatesDataSet.ProductDetailsDataTable
        'products = details.GetProductDetails(defaults.PriceList, Request.QueryString("hid"))

        'Dim al As New ArrayList
        'Dim productsToRemove As New ArrayList
        'Dim exists As Boolean = False
        'For Each product As OrderTemplatesDataSet.ProductDetailsRow In products.Rows
        '    exists = False

        '    For Each code As String In al
        '        If code = product.PRODUCT_CODE Then
        '            exists = True
        '        End If
        '    Next

        '    If exists Then
        '        productsToRemove.Add(product)
        '    Else
        '        al.Add(product.PRODUCT_CODE)
        '    End If
        'Next

        'For Each product As OrderTemplatesDataSet.ProductDetailsRow In productsToRemove
        '    products.Rows.Remove(product)
        'Next


        Dim DE_OrdTemplates As New Talent.Common.DEOrderTemplates("SELECT")
        Dim ordTemplate As New Talent.Common.DEOrderTemplate(CType(Request.QueryString("hid"), Long))

        DE_OrdTemplates.OrderTemplates.Add(ordTemplate)

        Dim DB_OrdTemplates As New Talent.Common.DBOrderTemplates(DE_OrdTemplates)
        DB_OrdTemplates.Settings = Utilities.GetSettingsObject()
        Dim err As Talent.Common.ErrorObj = DB_OrdTemplates.AccessDatabase()
        Dim tot, subtot, taxval As Decimal

        If Not err.HasError Then
            Dim detail As Data.DataTable = DB_OrdTemplates.ResultDataSet.Tables("OrderTemplatesDetail")
            For Each product As Data.DataRow In detail.Rows
                Dim productCode As String = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_CODE"))
                Dim mastProd As String = Talent.eCommerce.Utilities.CheckForDBNull_String(product("MASTER_PRODUCT"))
                If String.IsNullOrEmpty(mastProd) Then mastProd = productCode

                Dim deWp As Talent.Common.DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(productCode, CDec(product("QUANTITY")), mastProd)
                If ModuleDefaults.ShowPricesExVAT Then
                    subtot += Utilities.RoundToValue(deWp.Purchase_Price_Net, 0.01, False) * CDec(product("QUANTITY"))
                Else
                    subtot += Utilities.RoundToValue(deWp.Purchase_Price_Gross, 0.01, False) * CDec(product("QUANTITY"))
                End If
                taxval += Utilities.RoundToValue(deWp.Purchase_Price_Tax, 0.01, False) * CDec(product("QUANTITY"))
                tot += Utilities.RoundToValue(deWp.Purchase_Price_Gross, 0.01, False) * CDec(product("QUANTITY"))
            Next
        End If

        

        SubTotal.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(subtot, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
        Tax1.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(taxval, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)
        Total.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(subtot + taxval, 0.01, False), ucr.BusinessUnit, ucr.PartnerCode)

        tax1Row.Visible = True
        tax2Row.Visible = False
        tax3Row.Visible = False
        tax4Row.Visible = False
        tax5Row.Visible = False

        Promotion.Visible = False
        PromotionLabel.Visible = False
        Delivery.Visible = False
        DeliveryLabel.Visible = False

    End Sub

    Protected Sub SetOrderEnquiryTotals(Optional ByVal unitDelGross As Decimal = 0, _
                                        Optional ByVal unitDelNet As Decimal = 0, _
                                        Optional ByVal unitDelTax As Decimal = 0)

        '---------------------------------------------------------------
        ' From Order Summary - If displayed then show simplified version
        ' showing Gross values only (as Order Enquiry currently only 
        ' shows Gross Values)
        '---------------------------------------------------------------
        If Me.Visible Then
            Dim processedOrderHeaderId As String = String.Empty
            Dim tempOrderHeaderId As String = String.Empty
            processedOrderHeaderId = Request.QueryString("wid")

            Dim ordHeader As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
            Dim dt1 As Data.DataTable = ordHeader.Get_Order_Header_By_Processed_Order_ID(processedOrderHeaderId)

            tempOrderHeaderId = dt1.Rows(0)("TEMP_ORDER_ID")

            Dim ordDetails As New TalentBasketDatasetTableAdapters.tbl_order_detailTableAdapter
            Dim dt2 As Data.DataTable = ordDetails.Get_Order_Lines(tempOrderHeaderId)

            Dim subtot As Decimal = 0, _
                del As Decimal = 0, _
                delnet As Decimal = 0, _
                deltax As Decimal = 0, _
                tot As Decimal = 0, _
                promo As Decimal = 0, _
                taxval1 As Decimal = 0, _
                taxval2 As Decimal = 0, _
                taxval3 As Decimal = 0, _
                taxval4 As Decimal = 0, _
                taxval5 As Decimal = 0, _
                dispInc1 As Boolean = False, _
                dispInc2 As Boolean = False, _
                dispInc3 As Boolean = False, _
                dispInc4 As Boolean = False, _
                dispInc5 As Boolean = False, _
                promoCode As String = ""


            If dt1.Rows.Count > 0 Then

                subtot = Utilities.CheckForDBNull_Decimal(dt1.Rows(0)("TOTAL_ORDER_ITEMS_VALUE_GROSS"))

                If ModuleDefaults.DeliveryCalculationInUse Then
                    del = Utilities.CheckForDBNull_Decimal(dt1.Rows(0)("TOTAL_DELIVERY_GROSS"))
                    delnet = Utilities.CheckForDBNull_Decimal(dt1.Rows(0)("TOTAL_DELIVERY_NET"))
                    deltax = Utilities.CheckForDBNull_Decimal(dt1.Rows(0)("TOTAL_DELIVERY_TAX"))
                Else
                    deliveryRow.Visible = False
                End If

                taxval1 = Utilities.CheckForDBNull_Decimal(dt1.Rows(0)("TOTAL_ORDER_ITEMS_TAX")) + deltax
                promo = Utilities.CheckForDBNull_Decimal(dt1.Rows(0)("PROMOTION_VALUE"))
                tot = Utilities.CheckForDBNull_Decimal(dt1.Rows(0)("TOTAL_ORDER_ITEMS_VALUE_GROSS")) + del - promo
                tax1Row.Visible = False
                tax2Row.Visible = False
                tax3Row.Visible = False
                tax4Row.Visible = False
                tax5Row.Visible = False
            End If

            Dim roundUp As Boolean = False

            SubTotal.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(subtot, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
            Total.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(tot, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)

            Promotion.Text = String.Empty
            Dim openingSymbol As String = ucr.Content("TaxOpeningSymbolText", _languageCode, True)
            Dim closingSymbol As String = ucr.Content("TaxClosingSymbolText", _languageCode, True)

            Delivery.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(del, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
            _deliveryValue = Utilities.RoundToValue(del, 0.01, roundUp)
            If promo <> 0 Then
                Promotion.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(promo, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
            Else
                Promotion.Text = String.Empty
                PromotionLabel.Text = String.Empty
                Promotion.Visible = False
                PromotionLabel.Visible = False
            End If
            Me.MerchandiseTotal = tot
        End If
        TotalsHeaderLabel.Text = String.Empty
    End Sub

    Protected Sub SetPreOrderTotals(Optional ByVal unitDelGross As Decimal = 0, _
                                    Optional ByVal unitDelNet As Decimal = 0, _
                                    Optional ByVal unitDelTax As Decimal = 0, _
                                    Optional ByVal tempOrderID As String = "")


        If String.IsNullOrEmpty(tempOrderID) Then tempOrderID = Profile.Basket.TempOrderID

        Dim hasRetailProducts As Boolean = False
        'Test to see if there are non-ticketing items in the basket
        Select Case Profile.Basket.BasketContentType
            Case "M", "C"
                hasRetailProducts = True
        End Select

        If hasRetailProducts Then
            Dim subtot As Decimal = 0, _
                del As Decimal = 0, _
                delnet As Decimal = 0, _
                deltax As Decimal = 0, _
                tot As Decimal = 0, _
                promo As Decimal = 0, _
                taxval1 As Decimal = 0, _
                taxval2 As Decimal = 0, _
                taxval3 As Decimal = 0, _
                taxval4 As Decimal = 0, _
                taxval5 As Decimal = 0, _
                dispInc1 As Boolean = False, _
                dispInc2 As Boolean = False, _
                dispInc3 As Boolean = False, _
                dispInc4 As Boolean = False, _
                dispInc5 As Boolean = False, _
                promoCode As String = ""

            Try
                Dim promoBox As Control = Utilities.FindWebControl("PromotionsBox", Me.Page.Controls)
                promoCode = CallByName(promoBox, "PromotionCode", CallType.Get)
            Catch ex As Exception
            End Try

            Dim defaults As ECommerceModuleDefaults.DefaultValues = (New ECommerceModuleDefaults).GetDefaults
            Dim totals As Talent.Common.TalentWebPricing

            'Select Case defaults.PricingType
            '    Case 2
            '        totals = Utilities.GetPrices_Type2

            '    Case Else
            'Dim qtyAndCodes As New Generic.Dictionary(Of String, Talent.Common.WebPriceProduct)
            'For Each tbi As TalentBasketItem In Profile.Basket.BasketItems
            '    If UCase(tbi.MODULE_) <> "TICKETING" Then
            '        If Not tbi.IS_FREE Then
            '            If Not String.IsNullOrEmpty(tbi.MASTER_PRODUCT) Then
            '                'Check to see if the multibuys are configured for this master product
            '                Dim myPrice As Talent.Common.DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(tbi.MASTER_PRODUCT, 0, tbi.MASTER_PRODUCT)
            '                If myPrice.SALE_PRICE_BREAK_QUANTITY_1 > 0 OrElse myPrice.PRICE_BREAK_QUANTITY_1 > 0 Then
            '                    'multibuys are configured
            '                    If qtyAndCodes.ContainsKey(tbi.MASTER_PRODUCT) Then
            '                        qtyAndCodes(tbi.MASTER_PRODUCT).Quantity += tbi.Quantity
            '                    Else
            '                        ' Pass in product otherwise Promotions don't work properly
            '                        ' qtyAndCodes.Add(tbi.MASTER_PRODUCT, New Talent.Common.WebPriceProduct(tbi.MASTER_PRODUCT, tbi.Quantity, tbi.MASTER_PRODUCT))
            '                        qtyAndCodes.Add(tbi.MASTER_PRODUCT, New Talent.Common.WebPriceProduct(tbi.Product, tbi.Quantity, tbi.MASTER_PRODUCT))
            '                    End If
            '                Else
            '                    If Not qtyAndCodes.ContainsKey(tbi.Product) Then
            '                        qtyAndCodes.Add(tbi.Product, New Talent.Common.WebPriceProduct(tbi.Product, tbi.Quantity, tbi.MASTER_PRODUCT))
            '                    End If
            '                End If
            '            Else
            '                If Not qtyAndCodes.ContainsKey(tbi.Product) Then
            '                    qtyAndCodes.Add(tbi.Product, New Talent.Common.WebPriceProduct(tbi.Product, tbi.Quantity, tbi.MASTER_PRODUCT))
            '                End If
            '            End If
            '        End If
            '    End If
            'Next
            If (Profile.Basket.WebPrices Is Nothing) Then

                Dim defaultPriceList As String = defaults.DefaultPriceList
                If Not defaults.UseGlobalPriceListWithCustomerPriceList Then defaultPriceList = defaults.PriceList

                Dim pricingsettings As New Talent.Common.DEWebPriceSetting(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
                                           "SQL2005", _
                                           "", _
                                           TalentCache.GetBusinessUnit, _
                                           TalentCache.GetPartner(HttpContext.Current.Profile), _
                                           defaults.PriceList, _
                                           False, _
                                           Talent.Common.Utilities.GetDefaultLanguage, _
                                           Utilities.GetPartnerGroup, _
                                           defaultPriceList, _
                                           "")
                Dim products As New Generic.Dictionary(Of String, Talent.Common.WebPriceProduct)

                Dim productPricing As New Talent.Common.TalentWebPricing(pricingsettings, products, Not defaults.ShowPricesExVAT)
                totals = productPricing
            Else
                totals = Profile.Basket.WebPrices
            End If
            'totals = Utilities.GetWebPrices_WithTotals(qtyAndCodes, tempOrderID, defaults.PromotionPriority, promoCode)
            'End Select

            If defaults.Call_Tax_WebService Then
                'If we are using the tax calculator then do the following
                Dim taxCalc As New TaxWebService
                Dim results As New Data.DataSet

                Select Case UCase(Usage)
                    Case Is = "BASKET"
                        results = taxCalc.CallTaxWebService("BASKET")
                    Case Is = "ORDERSUMMARY", "PAYMENT", "ORDER"
                        results = taxCalc.CallTaxWebService("ORDER", tempOrderID, promoCode)
                End Select

                If results.Tables.Count > 0 Then
                    Dim str As String = results.GetXml.ToString
                    Dim header As Data.DataTable = results.Tables(0)
                    If header.Rows.Count > 0 Then

                        If CBool(header.Rows(0)("Success")) Then
                            Try
                                tax1Row.Visible = CBool(header.Rows(0)("DisplayTax1"))
                            Catch ex As Exception
                                tax1Row.Visible = False
                            End Try
                            Try
                                tax2Row.Visible = CBool(header.Rows(0)("DisplayTax2"))
                            Catch ex As Exception
                                tax2Row.Visible = False
                            End Try
                            Try
                                tax3Row.Visible = CBool(header.Rows(0)("DisplayTax3"))
                            Catch ex As Exception
                                tax3Row.Visible = False
                            End Try
                            Try
                                tax4Row.Visible = CBool(header.Rows(0)("DisplayTax4"))
                            Catch ex As Exception
                                tax4Row.Visible = False
                            End Try
                            Try
                                tax5Row.Visible = CBool(header.Rows(0)("DisplayTax5"))
                            Catch ex As Exception
                                tax5Row.Visible = False
                            End Try

                            Try
                                dispInc1 = CBool(header.Rows(0)("DisplayInclusive1"))
                            Catch ex As Exception
                            End Try
                            Try
                                dispInc2 = CBool(header.Rows(0)("DisplayInclusive2"))
                            Catch ex As Exception
                            End Try
                            Try
                                dispInc3 = CBool(header.Rows(0)("DisplayInclusive3"))
                            Catch ex As Exception
                            End Try
                            Try
                                dispInc4 = CBool(header.Rows(0)("DisplayInclusive4"))
                            Catch ex As Exception
                            End Try
                            Try
                                dispInc5 = CBool(header.Rows(0)("DisplayInclusive5"))
                            Catch ex As Exception
                            End Try


                            Try
                                taxval1 = CDec(header.Rows(0)("TotalTax1"))
                            Catch ex As Exception
                            End Try
                            Try
                                taxval2 = CDec(header.Rows(0)("TotalTax2"))
                            Catch ex As Exception
                            End Try
                            Try
                                taxval3 = CDec(header.Rows(0)("TotalTax3"))
                            Catch ex As Exception
                            End Try
                            Try
                                taxval4 = CDec(header.Rows(0)("TotalTax4"))
                            Catch ex As Exception
                            End Try
                            Try
                                taxval5 = CDec(header.Rows(0)("TotalTax5"))
                            Catch ex As Exception
                            End Try

                            Try
                                del = CDec(header.Rows(0)("DeliveryGROSS"))
                            Catch ex As Exception
                            End Try
                            Try
                                delnet = CDec(header.Rows(0)("DeliveryNET"))
                            Catch ex As Exception
                            End Try
                            Try
                                deltax = CDec(header.Rows(0)("DeliveryTAX"))
                            Catch ex As Exception
                            End Try

                            Try
                                Dim subtract As Decimal = 0
                                If Not dispInc1 Then
                                    subtract += taxval1 + delnet
                                Else
                                    subtract += del
                                End If
                                If Not dispInc2 Then subtract += taxval2
                                If Not dispInc3 Then subtract += taxval3
                                If Not dispInc4 Then subtract += taxval4
                                If Not dispInc5 Then subtract += taxval5

                                subtot = (CDec(header.Rows(0)("TotalGross")) - (subtract))
                            Catch ex As Exception
                            End Try

                            Try
                                tot = CDec(header.Rows(0)("TotalGross"))
                            Catch ex As Exception
                            End Try

                        End If

                    End If
                End If
            Else
                'If we are not performing tax calcultations then do this
                Select Case UCase(Usage)
                    Case Is = "BASKET"
                        If Not totals Is Nothing Then
                            dispInc1 = True
                            If defaults.ShowPricesExVAT Then
                                subtot = totals.Total_Items_Value_Net
                            Else
                                subtot = totals.Total_Items_Value_Gross
                            End If
                            tot = totals.Total_Order_Value_Gross
                            taxval1 = totals.Total_Order_Value_Tax

                            If defaults.DeliveryCalculationInUse Then
                                Select Case UCase(defaults.DeliveryPriceCalculationType)
                                    Case "UNIT", "WEIGHT"
                                        deliveryRow.Visible = False
                                        del = 0
                                        delnet = 0
                                        deltax = 0
                                    Case Else
                                        If totals.Qualifies_For_Free_Delivery Then
                                            '----------------------------------------------------------
                                            ' Need to display delivery if free delivery is a promotion.
                                            ' This keeps it consistent with order summary
                                            '----------------------------------------------------------
                                            If totals.Total_Promotions_Value > 0 AndAlso totals.FreeDeliveryPromotion Then
                                                del = totals.Max_Delivery_Gross
                                                delnet = totals.Max_Delivery_Net
                                                deltax = totals.Max_Delivery_Tax
                                            Else
                                                del = 0
                                                delnet = 0
                                                deltax = 0
                                            End If

                                        Else
                                            del = totals.Max_Delivery_Gross
                                            delnet = totals.Max_Delivery_Net
                                            deltax = totals.Max_Delivery_Tax
                                        End If
                                End Select
                            Else
                                deliveryRow.Visible = False
                                del = 0
                                delnet = 0
                                deltax = 0
                            End If


                            tax2Row.Visible = False
                            tax3Row.Visible = False
                            tax4Row.Visible = False
                            tax5Row.Visible = False
                        End If
                    Case Is = "ORDERSUMMARY", "PAYMENT", "ORDER"
                        Dim orders As New TalentBasketDatasetTableAdapters.tbl_order_headerTableAdapter
                        Dim dt As New Data.DataTable

                        If Usage.ToUpper = "ORDER" Then
                            dt = orders.Get_PROCESSED_Order(tempOrderID)
                        Else
                            dt = orders.Get_UNPROCESSED_Order(TalentCache.GetBusinessUnit, Profile.UserName)
                        End If

                        dispInc1 = True
                        If dt.Rows.Count > 0 Then
                            If defaults.ShowPricesExVAT Then
                                subtot = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_ORDER_ITEMS_VALUE_NET"))
                            Else
                                subtot = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_ORDER_ITEMS_VALUE_GROSS"))
                            End If

                            If defaults.DeliveryCalculationInUse Then
                                If Usage.ToUpper = "PAYMENT" OrElse Usage.ToUpper = "ORDER" Then
                                    del = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_DELIVERY_GROSS"))
                                    delnet = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_DELIVERY_NET"))
                                    deltax = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_DELIVERY_TAX"))
                                Else
                                    Select Case UCase(defaults.DeliveryPriceCalculationType)
                                        Case "UNIT", "WEIGHT"
                                            del = unitDelGross
                                            delnet = unitDelNet
                                            deltax = unitDelTax
                                        Case Else
                                            del = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_DELIVERY_GROSS"))
                                            delnet = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_DELIVERY_NET"))
                                            deltax = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_DELIVERY_TAX"))

                                    End Select
                                End If
                            Else
                                deliveryRow.Visible = False
                                del = 0
                                delnet = 0
                                deltax = 0
                            End If

                            taxval1 = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_ORDER_ITEMS_TAX")) + deltax
                            promo = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("PROMOTION_VALUE"))
                            tot = Utilities.CheckForDBNull_Decimal(dt.Rows(0)("TOTAL_ORDER_ITEMS_VALUE_GROSS")) + del
                            tax2Row.Visible = False
                            tax3Row.Visible = False
                            tax4Row.Visible = False
                            tax5Row.Visible = False
                        End If
                End Select

            End If

            Dim roundUp As Boolean = False 'def.ShowPricesExVAT

            SubTotal.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(subtot, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
            Tax1.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(taxval1, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
            Tax2.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(taxval2, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
            Tax3.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(taxval3, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
            Tax4.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(taxval4, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
            Tax5.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(taxval5, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
            Total.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(tot, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)


            Dim openingSymbol As String = ucr.Content("TaxOpeningSymbolText", _languageCode, True)
            Dim closingSymbol As String = ucr.Content("TaxClosingSymbolText", _languageCode, True)

            If dispInc1 Then
                If defaults.ShowPricesExVAT Then
                    Delivery.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(delnet, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
                    _deliveryValue = Utilities.RoundToValue(delnet, 0.01, roundUp)
                Else
                    Delivery.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(del, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
                    _deliveryValue = Utilities.RoundToValue(del, 0.01, roundUp)
                End If
                If Not Tax1Label.Text.StartsWith(openingSymbol) Then Tax1Label.Text = openingSymbol & Tax1Label.Text
                Tax1.Text += closingSymbol
            Else
                Delivery.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(delnet, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
                _deliveryValue = Utilities.RoundToValue(delnet, 0.01, roundUp)
            End If
            If dispInc2 Then
                If Not Tax2Label.Text.StartsWith(openingSymbol) Then Tax2Label.Text = openingSymbol & Tax2Label.Text
                Tax2.Text += closingSymbol
            End If
            If dispInc3 Then
                If Not Tax3Label.Text.StartsWith(openingSymbol) Then Tax3Label.Text = openingSymbol & Tax3Label.Text
                Tax3.Text += closingSymbol
            End If
            If dispInc4 Then
                If Not Tax4Label.Text.StartsWith(openingSymbol) Then Tax4Label.Text = openingSymbol & Tax4Label.Text
                Tax4.Text += closingSymbol
            End If
            If dispInc5 Then
                If Not Tax5Label.Text.StartsWith(openingSymbol) Then Tax5Label.Text = openingSymbol & Tax5Label.Text
                Tax5.Text += closingSymbol
            End If

            Dim promotionValue As Decimal = 0

            Select Case Usage.ToUpper
                Case "PAYMENT", "ORDER"
                    promotionValue = promo
                Case Else
                    If Not totals Is Nothing Then
                        promotionValue = totals.Total_Promotions_Value
                    End If
            End Select

            If promotionValue > 0 Then
                If defaults.Call_Tax_WebService Then
                    Promotion.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(promotionValue, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
                Else
                    If tot - promotionValue < 0 Then
                        Promotion.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(tot, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
                        Total.Text = TDataObjects.PaymentSettings.FormatCurrency(0, ucr.BusinessUnit, ucr.PartnerCode)
                    Else
                        Promotion.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(promotionValue, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
                        If defaults.Call_Tax_WebService Then
                            Total.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(tot, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
                        Else
                            Total.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(tot - promotionValue, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
                        End If
                    End If
                End If
            Else
                Promotion.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.RoundToValue(promotionValue, 0.01, roundUp), ucr.BusinessUnit, ucr.PartnerCode)
            End If

            Me.MerchandiseTotal = tot - promotionValue

        End If
    End Sub

    Public Sub RefreshDeliveryValue(ByVal delGross As Decimal, ByVal delNet As Decimal, ByVal delTax As Decimal)
        If UCase(ModuleDefaults.DeliveryPriceCalculationType) = "UNIT" OrElse (UCase(ModuleDefaults.DeliveryPriceCalculationType) = "WEIGHT") Then
            Select Case UCase(Usage)
                Case Is = "BASKET"
                    'Do nothing
                Case Is = "ORDERSUMMARY", "PAYMENT", "ORDER"
                    SetPreOrderTotals(delGross, delNet, delTax)
                    RefreshMiniBasketTotal()
                Case Is = "ORDERTEMPLATE"
                    'Do nothing
            End Select
        End If

    End Sub

    Public Sub ReBindSummaryTotalsOnConfirmation(ByVal tempOrderID As String)
        If UCase(ModuleDefaults.DeliveryPriceCalculationType) = "UNIT" OrElse (UCase(ModuleDefaults.DeliveryPriceCalculationType) = "WEIGHT") Then
            Select Case UCase(Usage)
                Case "ORDER"
                    SetPreOrderTotals(0, 0, 0, tempOrderID)
            End Select
        End If

    End Sub

    Private Sub RefreshMiniBasketTotal()
        If _deliveryValue > 0 Then
            Dim basketMini As Object = Utilities.FindWebControl("MiniBasket1", Me.Page.Controls)
            If basketMini IsNot Nothing Then
                CallByName(basketMini, "RefreshTotalWithDelivery", CallType.Method, _deliveryValue)
            End If
        End If
    End Sub
End Class
