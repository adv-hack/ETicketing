Imports System.Collections.Generic
Imports System.Data
Imports Talent.Common
Imports Talent.eCommerce
Imports TCUtilities = Talent.Common.Utilities
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class PagesPublic_ProductBrowse_LinkedProducts
    Inherits TalentBase01

#Region "Class Level Fields"

    Private _wfrPage As WebFormResource
    Private _languageCode As String
    Private _productCodeLinkingFrom As String = String.Empty
    Private _productTypeLinkingFrom As String = String.Empty
    Private _productSubTypeLinkingFrom As String = String.Empty
    Private _priceCodeLinkingFrom As String = String.Empty
    Private _productLinkingFromStadiumCode As String = String.Empty
    Private _dtProductRelations As DataTable
    Private _talentProduct As TalentProduct
    Private _deSettings As DESettings
    Private _nothingToShow As Boolean
    Private _errMsg As TalentErrorMessages
    Private _linkedProductId As Integer
    Private _quantityRequested As Integer

#End Region

#Region "Public variables"

    Public Property ItemsPurchasedCSS() As String = String.Empty

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        _wfrPage = New WebFormResource
        _languageCode = TCUtilities.GetDefaultLanguage
        With _wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile, .BusinessUnit)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = TEBUtilities.GetCurrentPageName
            .PageCode = TEBUtilities.GetCurrentPageName
        End With
        _errMsg = New TalentErrorMessages(_languageCode, _wfrPage.BusinessUnit, _wfrPage.PartnerCode, _wfrPage.FrontEndConnectionString)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _productCodeLinkingFrom = TDataObjects.BasketSettings.TblBasketHeader.GetLinkedMasterProduct(Profile.Basket.Basket_Header_ID)
        If Profile.Basket.BasketItems.Count = 0 OrElse CATHelper.IsItCATRequest(0) OrElse String.IsNullOrEmpty(_productCodeLinkingFrom) Then
            _nothingToShow = True
        Else
            ltlErrorMessage.Text = String.Empty
            If Request.QueryString("productsubtype") IsNot Nothing Then _productSubTypeLinkingFrom = Request.QueryString("productsubtype")
            If Request.QueryString("pricecode") IsNot Nothing Then _priceCodeLinkingFrom = Request.QueryString("pricecode")
            _dtProductRelations = TDataObjects.ProductsSettings.TblProductRelations.GetType2ProductRelationsByBUPartnerAndProductCode(_wfrPage.BusinessUnit, _wfrPage.PartnerCode, _productCodeLinkingFrom, _priceCodeLinkingFrom, _productSubTypeLinkingFrom)
            If _dtProductRelations IsNot Nothing AndAlso _dtProductRelations.Rows.Count > 0 Then
                _talentProduct = New TalentProduct
                _deSettings = New DESettings
                _deSettings = TEBUtilities.GetSettingsObject()
                _deSettings.Cacheing = True
                _deSettings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_wfrPage.Attribute("ProductDetailsCacheTimeMinutes"))
                _talentProduct.Settings() = _deSettings
                _talentProduct.De.Src = GlobalConstants.SOURCE

                'Reset the _nothingToShow variable to true here if the product linking from details are set correctly
                _nothingToShow = setProductLinkingFromDetails()
                rptLinkedProducts.DataSource = _dtProductRelations
                If Not IsPostBack Then rptLinkedProducts.DataBind()
                If rptLinkedProducts.Items.Count > 0 Then
                    _nothingToShow = True
                    For Each item As RepeaterItem In rptLinkedProducts.Items
                        If item.Visible Then
                            _nothingToShow = False
                            Exit For
                        End If
                    Next
                End If
            Else
                _nothingToShow = True
            End If
        End If
        If _nothingToShow Then
            performBasketRedirect()
        Else
            setPageText()
            Dim productCount As Integer = 0
            For Each item As RepeaterItem In rptLinkedProducts.Items
                If item.Visible Then productCount += 1
            Next
            registerAcordionMenuScript(productCount)
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        plhErrorMessages.Visible = (ltlErrorMessage.Text.Length > 0)
        plhListOfErrorMessages.Visible = (blErrorMessages.Items.Count > 0)
        plhListOfSuccessMessages.Visible = (blSuccessMessages.Items.Count > 0)
        Dim basketMini As Object = TEBUtilities.FindWebControl("MiniBasket1", Me.Page.Controls)
        If basketMini IsNot Nothing Then
            CallByName(basketMini, "ReBindBasket", CallType.Method)
        End If
    End Sub

    Protected Sub rptLinkedProducts_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptLinkedProducts.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim hdfProductCode As HiddenField = CType(e.Item.FindControl("hdfProductCode"), HiddenField)
            Dim hdfPriceCode As HiddenField = CType(e.Item.FindControl("hdfPriceCode"), HiddenField)
            Dim hdfCampaignCode As HiddenField = CType(e.Item.FindControl("hdfCampaignCode"), HiddenField)
            Dim hdfProductType As HiddenField = CType(e.Item.FindControl("hdfProductType"), HiddenField)
            Dim hdfProductSubType As HiddenField = CType(e.Item.FindControl("hdfProductSubType"), HiddenField)
            Dim hdfQuantity As HiddenField = CType(e.Item.FindControl("hdfQuantity"), HiddenField)
            Dim hlkSeatSelection As HyperLink = CType(e.Item.FindControl("hlkSeatSelection"), HyperLink)
            Dim btnSeatSelection As Button = CType(e.Item.FindControl("btnSeatSelection"), Button)
            Dim plhSeatSelectionLink As PlaceHolder = CType(e.Item.FindControl("plhSeatSelectionLink"), PlaceHolder)
            Dim ltlNumberOfTickets As Literal = CType(e.Item.FindControl("ltlNumberOfTickets"), Literal)
            Dim plhNumberOfTickets As PlaceHolder = CType(e.Item.FindControl("plhNumberOfTickets"), PlaceHolder)
            Dim ltlItemsPurchasedIndicator As Literal = CType(e.Item.FindControl("ltlItemsPurchasedIndicator"), Literal)
            Dim txtQuantity As TextBox = CType(e.Item.FindControl("txtQuantity"), TextBox)
            Dim basketQuantity As Integer = 0
            Dim restrictGraphical As Boolean
            Dim stadiumCode As String = String.Empty

            _talentProduct.De.ProductCode = hdfProductCode.Value
            _talentProduct.De.PriceCode = hdfPriceCode.Value
            _talentProduct.De.CampaignCode = hdfCampaignCode.Value
            _talentProduct.De.AllowPriceException = setAllowPriceException(hdfProductType.Value, hdfPriceCode.Value)
            _talentProduct.ResultDataSet = Nothing
            Dim err As ErrorObj = _talentProduct.ProductDetails
            If Not err.HasError AndAlso _talentProduct.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred").ToString() <> GlobalConstants.ERRORFLAG Then
                Dim dsProductDetails As DataSet = _talentProduct.ResultDataSet
                Dim ltlExtendedText1 As Literal = CType(e.Item.FindControl("ltlExtendedText1"), Literal)
                Dim ltlExtendedText2 As Literal = CType(e.Item.FindControl("ltlExtendedText2"), Literal)
                Dim ltlExtendedText3 As Literal = CType(e.Item.FindControl("ltlExtendedText3"), Literal)
                Dim ltlExtendedText4 As Literal = CType(e.Item.FindControl("ltlExtendedText4"), Literal)
                Dim ltlExtendedText5 As Literal = CType(e.Item.FindControl("ltlExtendedText5"), Literal)
                Dim divAccordionHeader As HtmlControl = CType(e.Item.FindControl("divAccordionHeader"), HtmlControl)
                Dim divAccordionContent As HtmlControl = CType(e.Item.FindControl("divAccordionContent"), HtmlControl)
                Dim divPurchasedIcon As HtmlControl = CType(e.Item.FindControl("divPurchasedIcon"), HtmlControl)

                ltlExtendedText1.Text = dsProductDetails.Tables(2).Rows(0)("ProductDetail1").ToString
                ltlExtendedText2.Text = dsProductDetails.Tables(2).Rows(0)("ProductDetail2").ToString
                ltlExtendedText3.Text = dsProductDetails.Tables(2).Rows(0)("ProductDetail3").ToString
                ltlExtendedText4.Text = dsProductDetails.Tables(2).Rows(0)("ProductDetail4").ToString
                ltlExtendedText5.Text = dsProductDetails.Tables(2).Rows(0)("ProductDetail5").ToString
                CType(e.Item.FindControl("plhExtendedTextPanel"), PlaceHolder).Visible = (ltlExtendedText1.Text.Trim().Length > 0 OrElse ltlExtendedText2.Text.Trim().Length > 0 OrElse _
                        ltlExtendedText3.Text.Trim().Length > 0 OrElse ltlExtendedText4.Text.Trim().Length > 0 OrElse ltlExtendedText5.Text.Trim().Length > 0)

                'Add Product specific content from tbl_product_specific_content
                Dim dtSpecificContent As DataTable = TDataObjects.ProductsSettings.TblProductSpecificContent.GetProductContent("ProductList", hdfProductCode.Value)
                Dim ltlSpecificContent1 As Literal = CType(e.Item.FindControl("ltlSpecificContent1"), Literal)
                If dtSpecificContent.Rows.Count > 0 Then
                    ltlSpecificContent1.Text = dtSpecificContent.Rows(0).Item("Product_Content").ToString
                End If

                For Each item As TalentBasketItem In Profile.Basket.BasketItems
                    If item.Product = hdfProductCode.Value Then
                        'Components tally quantity differently than other products.
                        If item.PRODUCT_TYPE = "C" OrElse AgentProfile.BulkSalesMode Then
                            basketQuantity = item.Quantity
                            Exit For
                        Else
                            basketQuantity += 1
                        End If
                    End If
                Next

                ltlItemsPurchasedIndicator.Text = TEBUtilities.CheckForDBNull_String(_wfrPage.Attribute("ItemsPurchasedIndicator"))
                hdfQuantity.Value = basketQuantity
                restrictGraphical = dsProductDetails.Tables(2).Rows(0).Item("RestrictGraphical")
                stadiumCode = dsProductDetails.Tables(2).Rows(0).Item("ProductStadium")
                plhNumberOfTickets.Visible = (basketQuantity > 0)
                If plhNumberOfTickets.Visible Then
                    ltlNumberOfTickets.Text = _wfrPage.Content("NumberOfTicketsText", _languageCode, True)
                    ltlNumberOfTickets.Text = ltlNumberOfTickets.Text.Replace("{0}", basketQuantity)
                    ItemsPurchasedCSS = "ebiz-item-purchased"
                    divPurchasedIcon.Attributes.Add("style", "display: block;")
                End If
                divPurchasedIcon.Attributes.Add("class", "ebiz-purchased-icon accordion_item_no_" & e.Item.ItemIndex)
                divAccordionHeader.Attributes.Add("class", "panel ebiz-header " & ItemsPurchasedCSS & " " & hdfProductCode.Value & " " & " accordion_item_no_" & e.Item.ItemIndex)
                txtQuantity.Attributes.Add("onkeyup", "addCSSOnQuantityChange(this," & e.Item.ItemIndex & ");")
                divAccordionContent.Attributes.Add("class", "panel ebiz-content " & ItemsPurchasedCSS & " " & hdfProductCode.Value)
                hlkSeatSelection.NavigateUrl = TEBUtilities.GetFormattedSeatSelectionUrl(String.Empty, stadiumCode, hdfProductCode.Value, hdfCampaignCode.Value, hdfProductType.Value, hdfProductSubType.Value, String.Empty, restrictGraphical, True)
                If (hdfProductType.Value = GlobalConstants.SEASONTICKETPRODUCTTYPE OrElse hdfProductType.Value = GlobalConstants.HOMEPRODUCTTYPE) Then
                    If dsProductDetails.Tables(2).Rows(0)("UseVisualSeatLevelSelection").ToString = True AndAlso dsProductDetails.Tables(2).Rows(0)("RestrictGraphical").ToString = False Then
                        plhSeatSelectionLink.Visible = True
                    Else
                        plhSeatSelectionLink.Visible = False
                    End If
                    If hlkSeatSelection.NavigateUrl.Length > 0 Then
                        hlkSeatSelection.Visible = True
                        btnSeatSelection.Visible = False
                        hlkSeatSelection.Text = _wfrPage.Content("SeatSelectionHyperlinkText", _languageCode, True)
                    Else
                        hlkSeatSelection.Visible = False
                        btnSeatSelection.Visible = True
                        btnSeatSelection.Text = _wfrPage.Content("SeatSelectionHyperlinkText", _languageCode, True)
                    End If
                Else
                    plhSeatSelectionLink.Visible = False
                End If
                bindRepeaterItem(e, hdfProductCode.Value, hdfCampaignCode.Value, stadiumCode)
            Else
                e.Item.Visible = False
            End If
        End If
    End Sub

    Protected Sub rptSeatDetails_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptSeatDetails.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim standDescription As String = String.Empty
            Dim areaDescription As String = String.Empty
            Dim seat As DESeatDetails = e.Item.DataItem
            Dim ltlSeatDetailsStandLabel As Literal = CType(e.Item.FindControl("ltlSeatDetailsStandLabel"), Literal)
            Dim ltlSeatDetailsAreaLabel As Literal = CType(e.Item.FindControl("ltlSeatDetailsAreaLabel"), Literal)
            Dim ltlSeatDetailsRowLabel As Literal = CType(e.Item.FindControl("ltlSeatDetailsRowLabel"), Literal)
            Dim ltlSeatDetailsSeatLabel As Literal = CType(e.Item.FindControl("ltlSeatDetailsSeatLabel"), Literal)
            Dim ltlSeatDetailsStandValue As Literal = CType(e.Item.FindControl("ltlSeatDetailsStandValue"), Literal)
            Dim ltlSeatDetailsAreaValue As Literal = CType(e.Item.FindControl("ltlSeatDetailsAreaValue"), Literal)
            Dim ltlSeatDetailsRowValue As Literal = CType(e.Item.FindControl("ltlSeatDetailsRowValue"), Literal)
            Dim ltlSeatDetailsSeatValue As Literal = CType(e.Item.FindControl("ltlSeatDetailsSeatValue"), Literal)
            TEBUtilities.RetrieveStandAndAreaDescriptions(_productLinkingFromStadiumCode, seat.Stand, seat.Area, standDescription, areaDescription)
            ltlSeatDetailsStandLabel.Text = _wfrPage.Content("StandDescriptionText", _languageCode, True)
            ltlSeatDetailsAreaLabel.Text = _wfrPage.Content("AreaDescriptionText", _languageCode, True)
            ltlSeatDetailsRowLabel.Text = _wfrPage.Content("RowDescriptionText", _languageCode, True)
            ltlSeatDetailsSeatLabel.Text = _wfrPage.Content("SeatDescriptionText", _languageCode, True)
            ltlSeatDetailsStandValue.Text = standDescription
            ltlSeatDetailsAreaValue.Text = areaDescription
            ltlSeatDetailsRowValue.Text = seat.Row
            ltlSeatDetailsSeatValue.Text = seat.Seat
        End If
    End Sub

    Protected Sub btnBuy_Click(sender As Object, e As System.EventArgs) Handles btnBuy.Click
        buyButtonClick()
    End Sub

    Protected Sub btnBuyTop_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBuyTop.Click
        buyButtonClick()
    End Sub

    Protected Sub btnBasket_Click(sender As Object, e As System.EventArgs) Handles btnBasket.Click
        basketButtonClick()
    End Sub

    Protected Sub btnBasketTop_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBasketTop.Click
        basketButtonClick()
    End Sub

    Protected Sub rptLinkedProducts_ItemCommand(source As Object, e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptLinkedProducts.ItemCommand
        If e.CommandName = "SelectSeats" Then
            Dim uscStandAndAreaSelection As UserControls_StandAndAreaSelection = CType(e.Item.FindControl("uscStandAndAreaSelection"), UserControls_StandAndAreaSelection)
            Dim ltlProductDescription1 As Literal = CType(e.Item.FindControl("ltlProductDescription1"), Literal)
            Dim standSelected As String = String.Empty
            Dim areaSelected As String = String.Empty
            If uscStandAndAreaSelection.ValidateSelection(standSelected, areaSelected, String.Empty, String.Empty, String.Empty) Then
                uscStandAndAreaSelection.PerformSelectSeatsRedirect()
            Else
                ltlErrorMessage.Text = _errMsg.GetErrorMessage("ST-AR").ERROR_MESSAGE.Replace("<<PRODUCT_DESCRIPTION>>", ltlProductDescription1.Text)
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Set the page objects text properties
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub setPageText()
        btnBuy.Text = _wfrPage.Content("BuyButtonText", _languageCode, True)
        btnBasket.Text = _wfrPage.Content("BasketButtonText", _languageCode, True)
        btnBuyTop.Text = _wfrPage.Content("BuyButtonText", _languageCode, True)
        btnBasketTop.Text = _wfrPage.Content("BasketButtonText", _languageCode, True)
        ltlQuantityRequestedLabel.Text = _wfrPage.Content("QuantityRequestedLabel", _languageCode, True)
        Dim MaxSeatListDiplay As String = If(_quantityRequested < TEBUtilities.CheckForDBNull_Int(_wfrPage.Attribute("MaxSeatListDiplay")), "", TEBUtilities.CheckForDBNull_String(_wfrPage.Attribute("MaxSeatListDiplay")))
        Dim mainProductIntro As String = CStr(_wfrPage.Content("MainProductIntroText", _languageCode, True)).Replace("{0}", MaxSeatListDiplay)
        ltlMainProductIntro.Text = mainProductIntro
    End Sub

    ''' <summary>
    ''' Bind the product to the repeater item
    ''' </summary>
    ''' <param name="e">The repeater item that is being bound</param>
    ''' <param name="productCode">The product that is being bound</param>
    ''' <param name="campaignCode">The product campaign code to use</param>
    ''' <param name="stadiumCode">The product stadium code that is being bound</param>
    ''' <remarks></remarks>
    Private Sub bindRepeaterItem(ByRef e As RepeaterItemEventArgs, ByVal productCode As String, ByVal campaignCode As String, ByVal stadiumCode As String)
        Dim plhInstructions As PlaceHolder = CType(e.Item.FindControl("plhInstructions"), PlaceHolder)
        Dim ltlInstructions As Literal = CType(e.Item.FindControl("ltlInstructions"), Literal)
        Dim ltlProductDescription1 As Literal = CType(e.Item.FindControl("ltlProductDescription1"), Literal)
        Dim ltlProductDescription2 As Literal = CType(e.Item.FindControl("ltlProductDescription2"), Literal)
        Dim plhProductDescription2 As PlaceHolder = CType(e.Item.FindControl("plhProductDescription2"), PlaceHolder)
        Dim ltlDate As Literal = CType(e.Item.FindControl("ltlDate"), Literal)
        Dim ltlTime As Literal = CType(e.Item.FindControl("ltlTime"), Literal)
        Dim imgOpposition As Image = CType(e.Item.FindControl("imgOpposition"), Image)
        Dim hdfProductType As HiddenField = CType(e.Item.FindControl("hdfProductType"), HiddenField)
        Dim hdfProductSubType As HiddenField = CType(e.Item.FindControl("hdfProductSubType"), HiddenField)
        Dim hdfPriceCode As HiddenField = CType(e.Item.FindControl("hdfPriceCode"), HiddenField)
        Dim hdfDefaultPriceCode As HiddenField = CType(e.Item.FindControl("hdfDefaultPriceCode"), HiddenField)
        Dim hdfDefaultPriceBand As HiddenField = CType(e.Item.FindControl("hdfDefaultPriceBand"), HiddenField)
        Dim hdfProductDetailCode As HiddenField = CType(e.Item.FindControl("hdfProductDetailCode"), HiddenField)
        Dim hdfQuantity As HiddenField = CType(e.Item.FindControl("hdfQuantity"), HiddenField)
        Dim plhQuantity As PlaceHolder = CType(e.Item.FindControl("plhQuantity"), PlaceHolder)
        Dim uscStandAndAreaSelection As UserControls_StandAndAreaSelection = CType(e.Item.FindControl("uscStandAndAreaSelection"), UserControls_StandAndAreaSelection)
        Dim uscPriceBandSelection As UserControls_PriceBandSelection = CType(e.Item.FindControl("uscPriceBandSelection"), UserControls_PriceBandSelection)
        Dim lblQuantity As Label = CType(e.Item.FindControl("lblQuantity"), Label)
        Dim txtQuantity As TextBox = CType(e.Item.FindControl("txtQuantity"), TextBox)
        Dim rgxQuantity As RegularExpressionValidator = CType(e.Item.FindControl("rgxQuantity"), RegularExpressionValidator)
        Dim rfvQuantity As RequiredFieldValidator = CType(e.Item.FindControl("rfvQuantity"), RequiredFieldValidator)
        Dim rngQuantity As RangeValidator = CType(e.Item.FindControl("rngQuantity"), RangeValidator)
        Dim plhSoldOut As PlaceHolder = CType(e.Item.FindControl("plhSoldOUt"), PlaceHolder)
        Dim ltlSoldOut As Literal = CType(e.Item.FindControl("ltlSoldOut"), Literal)
        Dim plhPriceCodeDropdown As PlaceHolder = CType(e.Item.FindControl("plhPriceCodeDropdown"), PlaceHolder)
        Dim plhTicketQuantity As PlaceHolder = CType(e.Item.FindControl("plhTicketQuantity"), PlaceHolder)

        ltlInstructions.Text = e.Item.DataItem("RELATED_INSTRUCTIONS").ToString()
        plhInstructions.Visible = (ltlInstructions.Text.Length > 0)
        ltlProductDescription1.Text = _talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductDescription").ToString().Trim()
        Dim longProductDescription As New StringBuilder
        longProductDescription.Append(_talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductText1").ToString().Trim())
        longProductDescription.Append(_talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductText2").ToString().Trim())
        longProductDescription.Append(_talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductText3").ToString().Trim())
        longProductDescription.Append(_talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductText4").ToString().Trim())
        longProductDescription.Append(_talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductText5").ToString().Trim())
        ltlProductDescription2.Text = longProductDescription.ToString()
        plhProductDescription2.Visible = (ltlProductDescription2.Text.Length > 0)
        If Not _talentProduct.ResultDataSet.Tables(2).Rows(0)("HideDate") AndAlso (TEBUtilities.CheckForDBNull_String(_talentProduct.ResultDataSet.Tables(2).Rows(0)("MDTE08Date"))).Length = 7 Then
            ltlDate.Text = TEBUtilities.GetFormattedDateAndTime(_talentProduct.ResultDataSet.Tables(2).Rows(0)("MDTE08Date").ToString(), String.Empty, " ", ModuleDefaults.GlobalDateFormat, ModuleDefaults.Culture)
        End If
        If Not _talentProduct.ResultDataSet.Tables(2).Rows(0)("HideTime") Then
            ltlTime.Text = _talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductTime").ToString().Trim()
        End If
        If Trim(ltlDate.Text) = String.Empty AndAlso Trim(ltlTime.Text) = String.Empty Then
            plhDateAndTime.Visible = False
        End If
        imgOpposition.ImageUrl = ImagePath.getImagePath("PRODTICKETING", _talentProduct.ResultDataSet.Tables(2).Rows(0)("OppositionCode").ToString(), _wfrPage.BusinessUnit, _wfrPage.PartnerCode)
        imgOpposition.Visible = (imgOpposition.ImageUrl <> ModuleDefaults.MissingImagePath)
        hdfDefaultPriceCode.Value = _talentProduct.ResultDataSet.Tables(2).Rows(0)("DefaultPriceCode").ToString().Trim()
        hdfDefaultPriceBand.Value = _talentProduct.ResultDataSet.Tables(2).Rows(0)("DefaultPriceBand").ToString().Trim()

        If TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_talentProduct.ResultDataSet.Tables(2).Rows(0)("AvailableOnline")) Then
            uscStandAndAreaSelection.Visible = False
            plhPriceCodeDropdown.Visible = False
            plhQuantity.Visible = True
            plhSoldOut.Visible = False

            If _talentProduct.ResultDataSet.Tables(2).Rows(0)("IsSoldOut") OrElse uscStandAndAreaSelection.SoldOut Then
                plhQuantity.Visible = False
                plhSoldOut.Visible = True
                ltlSoldOut.Text = _wfrPage.Content("SoldOutText", _languageCode, True)
            Else
                Select Case hdfProductType.Value
                    Case Is = GlobalConstants.HOMEPRODUCTTYPE, GlobalConstants.SEASONTICKETPRODUCTTYPE
                        lblQuantity.Text = _wfrPage.Content("QuantityText", _languageCode, True)
                        setQuantityDefaults(e.Item, txtQuantity, rngQuantity, ltlProductDescription1.Text)
                        rgxQuantity.ErrorMessage = _errMsg.GetErrorMessage("QV").ERROR_MESSAGE
                        rfvQuantity.Enabled = (TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(e.Item.DataItem("RELATED_PRODUCT_MANDATORY")) AndAlso Not txtQuantity.ReadOnly AndAlso TEBUtilities.CheckForDBNull_Int(hdfQuantity.Value) = 0)
                        If rfvQuantity.Enabled Then rfvQuantity.ErrorMessage = _errMsg.GetErrorMessage("AAQ2").ERROR_MESSAGE.Replace("<<PRODUCT_DESCRIPTION>>", ltlProductDescription1.Text)
                        uscStandAndAreaSelection.Visible = True
                        setStandAndAreaSelectionDefaults(e.Item, uscStandAndAreaSelection, productCode, hdfProductType.Value, hdfProductSubType.Value, campaignCode)

                    Case Is = GlobalConstants.AWAYPRODUCTTYPE, GlobalConstants.MEMBERSHIPPRODUCTTYPE
                        lblQuantity.Text = _wfrPage.Content("QuantityText", _languageCode, True)
                        setQuantityDefaults(e.Item, txtQuantity, rngQuantity, ltlProductDescription1.Text)
                        rgxQuantity.ErrorMessage = _errMsg.GetErrorMessage("QV").ERROR_MESSAGE
                        rfvQuantity.Enabled = (TEBUtilities.CheckForDBNull_Boolean_DefaultFalse(e.Item.DataItem("RELATED_PRODUCT_MANDATORY")) AndAlso Not txtQuantity.ReadOnly)
                        If rfvQuantity.Enabled Then rfvQuantity.ErrorMessage = _errMsg.GetErrorMessage("AAQ2").ERROR_MESSAGE.Replace("<<PRODUCT_DESCRIPTION>>", ltlProductDescription1.Text)
                        If hdfPriceCode.Value.Length = 0 Then
                            setupPriceCodeDropdownList(plhPriceCodeDropdown, e.Item, _talentProduct.ResultDataSet.Tables("PriceCodes"), productCode, stadiumCode)
                        End If
                        
                    Case Is = GlobalConstants.TRAVELPRODUCTTYPE, GlobalConstants.EVENTPRODUCTTYPE
                        plhTicketQuantity.Visible = False
                        LoadPriceBandSelection(uscPriceBandSelection, productCode, hdfProductType.Value, hdfProductSubType.Value, stadiumCode)
                End Select
            End If

            loadAdditionalInformation(e.Item, productCode, hdfProductType.Value, hdfProductSubType.Value)
        Else
            e.Item.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' This method loads the price band selection control for the product in action
    ''' </summary>
    ''' <param name="priceBandSelectionControl">Price band selection control</param>
    ''' <param name="productCode">Product code</param>
    ''' <param name="productType">Product type</param>
    ''' <param name="productSubType">Product subtype</param>
    ''' <param name="stadiumCode">Stadium code</param>
    ''' <remarks></remarks>
    Private Sub LoadPriceBandSelection(ByRef priceBandSelectionControl As UserControls_PriceBandSelection, ByVal productCode As String, ByVal productType As String, ByVal productSubType As String, ByVal stadiumCode As String)
        priceBandSelectionControl.Visible = True
        priceBandSelectionControl.ProductCode = productCode
        priceBandSelectionControl.ProductType = productType
        priceBandSelectionControl.ProductSubType = productSubType
        priceBandSelectionControl.ProductStadium = stadiumCode

        Dim tProduct As New TalentProduct
        Dim productDataEntity As New DEProductDetails
        Dim settings As DESettings = TEBUtilities.GetSettingsObject()
        Dim err As New ErrorObj
        settings.Cacheing = True
        settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_wfrPage.Attribute("ProductDetailsCacheTimeMinutes"))
        productDataEntity.StadiumCode = stadiumCode
        productDataEntity.ProductSubtype = productSubType
        productDataEntity.ProductType = productType

        tProduct.De = productDataEntity
        tProduct.Settings = settings
        err = tProduct.ProductList()
        If Not err.HasError AndAlso tProduct.ResultDataSet IsNot Nothing AndAlso tProduct.ResultDataSet.Tables("StatusResults").Rows.Count > 0 Then
            If tProduct.ResultDataSet.Tables("StatusResults").Rows(0)("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then
                If tProduct.ResultDataSet.Tables("ProductListResults").Rows.Count > 0 Then
                    Dim table As DataTable = tProduct.ResultDataSet.Tables("ProductListResults")
                    If (productType = GlobalConstants.TRAVELPRODUCTTYPE) Then
                        SetupTotalsForTravelDropdown(tProduct.ResultDataSet.Tables("ProductListResults"))
                        priceBandSelectionControl.DSProductDetail = tProduct.ResultDataSet
                        priceBandSelectionControl.QuantityAvailable = 10
                    ElseIf (productType = GlobalConstants.EVENTPRODUCTTYPE) Then
                        priceBandSelectionControl.QuantityAvailable = GetQuantityAvailable(table, productCode, productType, productSubType, stadiumCode)
                    End If
                End If
            End If
        End If

        priceBandSelectionControl.LoadPriceBandSelection()
    End Sub

    ''' <summary>
    ''' This method setup the totals for the travel dropdown
    ''' </summary>
    ''' <param name="tProductList">Product list</param>
    ''' <remarks></remarks>
    Private Sub SetupTotalsForTravelDropdown(ByRef tProductList As DataTable)
        If Not tProductList.Columns.Contains("Unique") Then
            tProductList.Columns.Add("Unique", GetType(Boolean))
            tProductList.Columns.Add("TotalCapacity", GetType(Integer))
            tProductList.Columns.Add("TotalReturns", GetType(Integer))
            tProductList.Columns.Add("TotalSales", GetType(Integer))
            tProductList.Columns.Add("TotalUnavailable", GetType(Integer))
            tProductList.Columns.Add("TotalBookings", GetType(Integer))
            tProductList.Columns.Add("TotalReservations", GetType(Integer))
            tProductList.Columns.Add("Available", GetType(Boolean))
        End If

        Dim lastCode As String = String.Empty
        Dim lastIndex As Integer = -1
        Dim lastCapacity As Integer = 0
        Dim lastReturns As Integer = 0
        Dim lastSales As Integer = 0
        Dim lastUnavailable As Integer = 0
        Dim lastBookings As Integer = 0
        Dim lastReservations As Integer = 0
        Dim indexCount As Integer = 0
        Dim needToSetLastUnique As Boolean = False
        ' 
        ' loop through and total up capacities for unique travel codes
        For Each dr As DataRow In tProductList.Rows
            If dr("ProductType").ToString = "T" Then
                If dr.Item("ProductCode").ToString <> lastCode Then
                    ' new code - update totals for last one..
                    If lastIndex >= 0 Then
                        tProductList.Rows(lastIndex)("TotalCapacity") = lastCapacity
                        tProductList.Rows(lastIndex)("TotalReturns") = lastReturns
                        tProductList.Rows(lastIndex)("TotalSales") = lastSales
                        tProductList.Rows(lastIndex)("TotalUnavailable") = lastUnavailable
                        tProductList.Rows(lastIndex)("TotalBookings") = lastBookings
                        tProductList.Rows(lastIndex)("TotalReservations") = lastReservations
                        needToSetLastUnique = False
                    End If
                    lastCode = dr.Item("ProductCode").ToString
                    dr.Item("Unique") = True
                    lastIndex = indexCount
                    lastCapacity = CInt(dr("capacityCnt"))
                    lastReturns = CInt(dr("returnsCnt"))
                    lastSales = CInt(dr("salesCnt"))
                    lastUnavailable = CInt(dr("unavailableCnt"))
                    lastBookings = CInt(dr("bookingsCnt"))
                    lastReservations = CInt(dr("reservationsCnt"))
                    needToSetLastUnique = True
                Else
                    dr.Item("Unique") = False
                    lastCapacity += CInt(dr("capacityCnt"))
                    lastReturns += CInt(dr("returnsCnt"))
                    lastSales += CInt(dr("salesCnt"))
                    lastUnavailable += CInt(dr("unavailableCnt"))
                    lastBookings += CInt(dr("bookingsCnt"))
                    lastReservations += CInt(dr("reservationsCnt"))
                    tProductList.Rows(lastIndex)("TotalCapacity") = 0
                    tProductList.Rows(lastIndex)("TotalReturns") = 0
                    tProductList.Rows(lastIndex)("TotalSales") = 0
                    tProductList.Rows(lastIndex)("TotalUnavailable") = 0
                    tProductList.Rows(lastIndex)("TotalBookings") = 0
                    tProductList.Rows(lastIndex)("TotalReservations") = 0
                End If

                If AvailabilityTotal(dr("capacityCnt").ToString.Trim, dr("returnsCnt").ToString.Trim, _
                                        dr("salesCnt").ToString.Trim, dr("unavailableCnt").ToString.Trim, _
                                        dr("bookingsCnt").ToString.Trim, dr("reservationsCnt").ToString.Trim) > 0 Then
                    dr("Available") = True
                Else
                    dr("Available") = False
                End If
            End If

            indexCount += 1
        Next
        ' set totals on last unique one..
        If lastIndex >= 0 AndAlso needToSetLastUnique Then
            tProductList.Rows(lastIndex)("TotalCapacity") = lastCapacity
            tProductList.Rows(lastIndex)("TotalReturns") = lastReturns
            tProductList.Rows(lastIndex)("TotalSales") = lastSales
            tProductList.Rows(lastIndex)("TotalUnavailable") = lastUnavailable
            tProductList.Rows(lastIndex)("TotalBookings") = lastBookings
            tProductList.Rows(lastIndex)("TotalReservations") = lastReservations
            needToSetLastUnique = False
        End If
    End Sub

    ''' <summary>
    ''' Load the addtional information option to the product if it is available
    ''' </summary>
    ''' <param name="e">The repeater item we are binding to</param>
    ''' <param name="productCode">The given product code that has addtional information</param>
    ''' <param name="productType">The product type</param>
    ''' <param name="productSubType">The product sub type</param>
    ''' <remarks></remarks>
    Private Sub loadAdditionalInformation(ByRef e As RepeaterItem, ByVal productCode As String, ByVal productType As String, ByVal productSubType As String)
        Dim plhAdditionalInformation As PlaceHolder = CType(e.FindControl("plhAdditionalInformation"), PlaceHolder)
        Dim IncludeFolderName As String = "Other"
        Dim HTMLFound As Boolean = False
        If TEBUtilities.DoesHtmlFileExists(_wfrPage.BusinessUnit & "\" & _wfrPage.PartnerCode & "\Product\" & IncludeFolderName & "\" & productCode & ".htm") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_wfrPage.BusinessUnit & "\" & _wfrPage.PartnerCode & "\Product\" & IncludeFolderName & "\" & productCode & ".html") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_wfrPage.BusinessUnit & "\" & _wfrPage.PartnerCode & "\Product\" & IncludeFolderName & "\" & productSubType & ".htm") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_wfrPage.BusinessUnit & "\" & _wfrPage.PartnerCode & "\Product\" & IncludeFolderName & "\" & productSubType & ".html") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_wfrPage.BusinessUnit & "\" & _wfrPage.PartnerCode & "\Product\" & IncludeFolderName & "\" & productType & ".htm") Then
            HTMLFound = True
        ElseIf TEBUtilities.DoesHtmlFileExists(_wfrPage.BusinessUnit & "\" & _wfrPage.PartnerCode & "\Product\" & IncludeFolderName & "\" & productType & ".html") Then
            HTMLFound = True
        End If

        If HTMLFound Then
            Dim hlkAdditionalInformation As HyperLink = CType(e.FindControl("hlkAdditionalInformation"), HyperLink)
            plhAdditionalInformation.Visible = True
            hlkAdditionalInformation.Text = TEBUtilities.CheckForDBNull_String(_wfrPage.Content("AdditionalInformationText", _languageCode, True))
            hlkAdditionalInformation.NavigateUrl = "~/PagesPublic/ProductBrowse/ProductSummary.aspx?ProductCode=" & productCode & "&ProductType=" & productType & "&ProductSubType=" & productSubType & "&IncludeFolderName=" & IncludeFolderName
        Else
            Dim pdfFound As Boolean = False
            Dim pdfLink As String = TEBUtilities.PDFLinkAvailable(productCode, productType, productSubType, _wfrPage.BusinessUnit, _wfrPage.PartnerCode)
            If Not String.IsNullOrEmpty(pdfLink) Then
                Dim hlkAdditionalInformation As HyperLink = CType(e.FindControl("hlkAdditionalInformation"), HyperLink)
                plhAdditionalInformation.Visible = True
                hlkAdditionalInformation.Text = Talent.eCommerce.Utilities.CheckForDBNull_String(_wfrPage.Content("AdditionalInformationText", _languageCode, True))
                hlkAdditionalInformation.Target = "_blank"
                hlkAdditionalInformation.NavigateUrl = pdfLink
            Else
                plhAdditionalInformation.Visible = False
            End If
        End If
    End Sub

    ''' <summary>
    ''' Setup the quantity box based on the defaults that have been selected during link creation
    ''' </summary>
    ''' <param name="e">The repeater item being bound</param>
    ''' <param name="txtQuantity">The quantity text box element</param>
    ''' <param name="rngQuantity">The quantity range validator</param>
    ''' <param name="productDescription">The product description</param>
    ''' <remarks></remarks>
    Private Sub setQuantityDefaults(ByRef e As RepeaterItem, ByRef txtQuantity As TextBox, ByRef rngQuantity As RangeValidator, ByVal productDescription As String)
        Dim tGatewayFunctions As New TicketingGatewayFunctions
        Dim minQuantity As Integer = 0
        Dim maxQuantity As Integer = 0
        Dim defaultQuantity As String = String.Empty
        Dim isReadOnly As Boolean = False
        tGatewayFunctions.GetQuantityDefintions(e.DataItem("RELATED_PRODUCT"), minQuantity, maxQuantity, defaultQuantity, isReadOnly)
        txtQuantity.Text = defaultQuantity
        txtQuantity.ReadOnly = isReadOnly
        If isReadOnly Then
            rngQuantity.Enabled = False
            CType(e.FindControl("plhSeatSelectionLink"), PlaceHolder).Visible = False
        Else
            txtQuantity.Attributes.Add("min", minQuantity)
            txtQuantity.Attributes.Add("max", maxQuantity)
            txtQuantity.MaxLength = maxQuantity.ToString().Length
            rngQuantity.Enabled = True
            rngQuantity.MinimumValue = minQuantity
            rngQuantity.MaximumValue = maxQuantity
            rngQuantity.Type = ValidationDataType.Integer
            If minQuantity = maxQuantity Then
                rngQuantity.ErrorMessage = _errMsg.GetErrorMessage("QZ4").ERROR_MESSAGE.Replace("<<MAX_VALUE>>", maxQuantity).Replace("<<PRODUCT_DESCRIPTION>>", productDescription)
            Else
                rngQuantity.ErrorMessage = _errMsg.GetErrorMessage("QZ3").ERROR_MESSAGE.Replace("<<MIN_VALUE>>", minQuantity).Replace("<<MAX_VALUE>>", maxQuantity).Replace("<<PRODUCT_DESCRIPTION>>", productDescription)
            End If
            rngQuantity.ErrorMessage = rngQuantity.ErrorMessage.Replace("<<PRODUCT_DESCRIPTION>>", productDescription)
        End If
    End Sub

    ''' <summary>
    ''' Setup the stand and area options based on the defaults that have been selected during link creation
    ''' </summary>
    ''' <param name="e">The repeater item being bound</param>
    ''' <param name="uscStandAndAreaSelection">The stand and area selection user control</param>
    ''' <param name="productCode">The given product code</param>
    ''' <param name="productType">The given product type</param>
    ''' <param name="productSubType">The given product sub type</param>
    ''' <param name="campaignCode">The given campaign code</param>
    ''' <remarks></remarks>
    Private Sub setStandAndAreaSelectionDefaults(ByRef e As RepeaterItem, ByRef uscStandAndAreaSelection As UserControls_StandAndAreaSelection, _
                                                 ByVal productCode As String, ByVal productType As String, ByVal productSubType As String, ByVal campaignCode As String)
        uscStandAndAreaSelection.ProductCode = productCode
        uscStandAndAreaSelection.ProductPriceBand = _talentProduct.ResultDataSet.Tables(2).Rows(0)("DefaultPriceBand").ToString().Trim()
        uscStandAndAreaSelection.ProductStadium = _talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductStadium").ToString().Trim()
        uscStandAndAreaSelection.ProductType = productType
        uscStandAndAreaSelection.ProductSubType = productSubType
        uscStandAndAreaSelection.CampaignCode = campaignCode
        uscStandAndAreaSelection.ProductHomeAsAway = _talentProduct.ResultDataSet.Tables(2).Rows(0)("HomeAsAway").ToString().Trim()
        uscStandAndAreaSelection.AlternativeSeatSelection = _talentProduct.ResultDataSet.Tables(2).Rows(0)("AlternativeSeatSelection")
        uscStandAndAreaSelection.AlternativeSeatSelectionAcrossStands = _talentProduct.ResultDataSet.Tables(2).Rows(0)("AlternativeSeatSelectionAcrossStands")
        uscStandAndAreaSelection.HideBuyingOptions = True
        uscStandAndAreaSelection.HideSelectSeatsButton = True
        uscStandAndAreaSelection.ProductDescription = ltlProductDescription1.Text

        If uscStandAndAreaSelection.SoldOut Then
            CType(e.FindControl("plhQuantity"), PlaceHolder).Visible = False
        Else
            Dim stand As String = TEBUtilities.CheckForDBNull_String(e.DataItem("RELATED_TICKETING_PRODUCT_STAND"))
            Dim area As String = TEBUtilities.CheckForDBNull_String(e.DataItem("RELATED_TICKETING_PRODUCT_AREA"))
            Dim standDropDown As DropDownList = CType(uscStandAndAreaSelection.FindControl("standDropDown"), DropDownList)
            Dim areaDropDownList As DropDownList = CType(uscStandAndAreaSelection.FindControl("areaDropDownList"), DropDownList)
            standDropDown.Enabled = Not e.DataItem("RELATED_TICKETING_PRODUCT_STAND_READONLY")
            areaDropDownList.Enabled = Not e.DataItem("RELATED_TICKETING_PRODUCT_AREA_READONLY")
            uscStandAndAreaSelection.StandDropDownFill(stand, area)
        End If
    End Sub

    ''' <summary>
    ''' Javascript accordion menu open item on page load code.
    ''' </summary>
    ''' <param name="productCount">Item number of accordion menu to open</param>
    ''' <remarks></remarks>
    Private Sub registerAcordionMenuScript(ByVal productCount As Integer)
        Dim jsFile As String
        If productCount = 1 Then
            jsFile = "ticketing-products-accordion-expanded.js"
        Else
            jsFile = "ticketing-products-accordion.js"
        End If
        ScriptManager.RegisterStartupScript(rptLinkedProducts, Me.GetType(), "accordionMenu", Talent.eCommerce.Utilities.FormatJavaScriptFileReference(jsFile, Nothing), False)
    End Sub

    ''' <summary>
    ''' Buy button method
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub buyButtonClick()
        Dim listOfTicketingItems As New List(Of DEAddTicketingItems)
        Dim dicProductDescriptions As New Dictionary(Of String, String)
        ltlErrorMessage.Text = String.Empty
        blErrorMessages.Items.Clear()
        blSuccessMessages.Items.Clear()
        Page.Validate()
        If Page.IsValid Then
            For Each item As RepeaterItem In rptLinkedProducts.Items
                If item.Visible Then
                    Dim txtQuantity As TextBox = CType(item.FindControl("txtQuantity"), TextBox)
                    Dim quantity As Integer = 0
                    Dim hdfProductIsMandatory As HiddenField = CType(item.FindControl("hdfProductIsMandatory"), HiddenField)
                    Dim ltlProductDescription1 As Literal = CType(item.FindControl("ltlProductDescription1"), Literal)
                    Dim hdfProductCode As HiddenField = CType(item.FindControl("hdfProductCode"), HiddenField)
                    Dim hdfProductType As HiddenField = CType(item.FindControl("hdfProductType"), HiddenField)
                    Dim hdfPriceCode As HiddenField = CType(item.FindControl("hdfPriceCode"), HiddenField)
                    Dim hdfCampaignCode As HiddenField = CType(item.FindControl("hdfCampaignCode"), HiddenField)
                    Dim hdfQuantity As HiddenField = CType(item.FindControl("hdfQuantity"), HiddenField)

                    If (txtQuantity.Text.Length > 0 AndAlso Integer.TryParse(txtQuantity.Text, quantity)) AndAlso TEBUtilities.CheckForDBNull_Int(hdfQuantity.Value) = 0 Then
                        'Validate quantities when something numeric has been entered ONLY in the quantity text box
                        Dim hdfDefaultPriceCode As HiddenField = CType(item.FindControl("hdfDefaultPriceCode"), HiddenField)
                        Dim hdfDefaultPriceBand As HiddenField = CType(item.FindControl("hdfDefaultPriceBand"), HiddenField)
                        Dim deATI As New DEAddTicketingItems
                        deATI.ProductCode = hdfProductCode.Value
                        deATI.Quantity01 = quantity
                        deATI.PriceCode = hdfPriceCode.Value
                        deATI.CampaignCode = hdfCampaignCode.Value
                        deATI.ProductIsMandatory = CBool(hdfProductIsMandatory.Value)
                        deATI.PriceBand01 = hdfDefaultPriceBand.Value
                        deATI.LinkedParentProductCode = _productCodeLinkingFrom

                        Select Case hdfProductType.Value
                            Case Is = GlobalConstants.HOMEPRODUCTTYPE, GlobalConstants.SEASONTICKETPRODUCTTYPE
                                Dim uscStandAndAreaSelection As UserControls_StandAndAreaSelection = CType(item.FindControl("uscStandAndAreaSelection"), UserControls_StandAndAreaSelection)
                                If Not uscStandAndAreaSelection.ValidateSelection(deATI.StandCode, deATI.AreaCode, String.Empty, String.Empty, String.Empty) Then
                                    ltlErrorMessage.Text = _errMsg.GetErrorMessage("ST-AR").ERROR_MESSAGE.Replace("<<PRODUCT_DESCRIPTION>>", ltlProductDescription1.Text)
                                    Exit For
                                End If
                            Case Is = GlobalConstants.AWAYPRODUCTTYPE, GlobalConstants.MEMBERSHIPPRODUCTTYPE
                                Dim ddlPriceCodes As DropDownList = CType(item.FindControl("ddlPriceCodes"), DropDownList)
                                If hdfPriceCode.Value.Length = 0 Then
                                    deATI.PriceCode = ddlPriceCodes.SelectedValue
                                End If
                        End Select

                        'Use the default price code if one hasn't been set
                        If String.IsNullOrEmpty(deATI.PriceCode) Then deATI.PriceCode = hdfDefaultPriceCode.Value

                        'Add the item to the list
                        listOfTicketingItems.Add(deATI)
                        dicProductDescriptions.Add(deATI.ProductCode, ltlProductDescription1.Text)

                    ElseIf txtQuantity.Text.Length = 0 AndAlso (hdfProductType.Value = GlobalConstants.TRAVELPRODUCTTYPE Or hdfProductType.Value = GlobalConstants.EVENTPRODUCTTYPE) Then
                        'Validate quantities when chosen from the price band drop down list
                        Dim priceBandSelectionControl As UserControls_PriceBandSelection = CType(item.FindControl("uscPriceBandSelection"), UserControls_PriceBandSelection)
                        Dim standAndAreaSelectionControl As UserControls_StandAndAreaSelection = CType(item.FindControl("uscStandAndAreaSelection"), UserControls_StandAndAreaSelection)
                        Dim selectedQuantity As Integer = 0
                        Dim tGatewayFunctions As New TicketingGatewayFunctions
                        Dim minQuantity As Integer = 0
                        Dim maxQuantity As Integer = 0
                        Dim defaultQuantity As String = String.Empty
                        Dim isReadOnly As Boolean = False
                        standAndAreaSelectionControl.Visible = False
                        If (priceBandSelectionControl.Visible) Then
                            selectedQuantity = AddTicketsFromPriceBandSelection(priceBandSelectionControl, hdfProductType.Value, hdfProductCode.Value, hdfPriceCode.Value, hdfCampaignCode.Value, CBool(hdfProductIsMandatory.Value), listOfTicketingItems)
                        End If
                        If CBool(hdfProductIsMandatory.Value) AndAlso selectedQuantity = 0 Then
                            ltlErrorMessage.Text = _errMsg.GetErrorMessage("QZ2").ERROR_MESSAGE.Replace("<<PRODUCT_DESCRIPTION>>", ltlProductDescription1.Text)
                            Exit For
                        End If
                        tGatewayFunctions.GetQuantityDefintions(hdfProductCode.Value, minQuantity, maxQuantity, defaultQuantity, isReadOnly)
                        If selectedQuantity < minQuantity OrElse selectedQuantity > maxQuantity Then
                            If minQuantity = maxQuantity Then
                                ltlErrorMessage.Text = _errMsg.GetErrorMessage("QZ4").ERROR_MESSAGE.Replace("<<MAX_VALUE>>", maxQuantity).Replace("<<PRODUCT_DESCRIPTION>>", ltlProductDescription1.Text)
                            Else
                                ltlErrorMessage.Text = _errMsg.GetErrorMessage("QZ3").ERROR_MESSAGE.Replace("<<MIN_VALUE>>", minQuantity).Replace("<<MAX_VALUE>>", maxQuantity).Replace("<<PRODUCT_DESCRIPTION>>", ltlProductDescription1.Text)
                            End If
                            Exit For
                        End If
                        dicProductDescriptions.Add(hdfProductCode.Value, ltlProductDescription1.Text)

                    Else
                        'Validate quantities when there is nothing entered or when there is a quantity entered via seat selection.
                        If TEBUtilities.CheckForDBNull_Int(hdfQuantity.Value) = 0 Then
                            If CBool(hdfProductIsMandatory.Value) AndAlso Not txtQuantity.ReadOnly Then
                                ltlErrorMessage.Text = _errMsg.GetErrorMessage("QZ2").ERROR_MESSAGE.Replace("<<PRODUCT_DESCRIPTION>>", ltlProductDescription1.Text)
                                Exit For
                            End If
                        Else
                            Dim selectedQuantity As Integer = TEBUtilities.CheckForDBNull_Int(hdfQuantity.Value) + TEBUtilities.CheckForDBNull_Int(txtQuantity.Text)
                            Dim tGatewayFunctions As New TicketingGatewayFunctions
                            Dim minQuantity As Integer = 0
                            Dim maxQuantity As Integer = 0
                            Dim defaultQuantity As String = String.Empty
                            Dim isReadOnly As Boolean = False
                            tGatewayFunctions.GetQuantityDefintions(hdfProductCode.Value, minQuantity, maxQuantity, defaultQuantity, isReadOnly)
                            If selectedQuantity < minQuantity OrElse selectedQuantity > maxQuantity Then
                                If minQuantity = maxQuantity Then
                                    ltlErrorMessage.Text = _errMsg.GetErrorMessage("QZ4").ERROR_MESSAGE.Replace("<<MAX_VALUE>>", maxQuantity).Replace("<<PRODUCT_DESCRIPTION>>", ltlProductDescription1.Text)
                                Else
                                    ltlErrorMessage.Text = _errMsg.GetErrorMessage("QZ3").ERROR_MESSAGE.Replace("<<MIN_VALUE>>", minQuantity).Replace("<<MAX_VALUE>>", maxQuantity).Replace("<<PRODUCT_DESCRIPTION>>", ltlProductDescription1.Text)
                                End If
                                Exit For
                            End If
                        End If
                    End If
                End If
            Next

            'Check for any errors. If no errors attempt to add the selected products to the basket.
            'If no products have been selected and there are no errors, go straight to the basket.
            If ltlErrorMessage.Text.Length = 0 AndAlso blErrorMessages.Items.Count = 0 Then
                If listOfTicketingItems.Count > 0 Then
                    Dim err As New ErrorObj
                    Dim ticketingGatewayFunctions As New TicketingGatewayFunctions
                    err = ticketingGatewayFunctions.AddMultipleProductsToBasket(listOfTicketingItems, _linkedProductId)
                    If err.HasError Then
                        If err.ErrorNumber = GlobalConstants.ERRORWHENADDINGMULTIPLEPRODUCTS Then
                            ltlErrorMessage.Text = _wfrPage.Content("ErrorWithMultipleProductRequestHeaderText", _languageCode, True)
                            For Each productDataEntity As DEAddTicketingItems In listOfTicketingItems
                                Dim productDescription As String = String.Empty
                                Dim errorMessageItem As New ListItem
                                dicProductDescriptions.TryGetValue(productDataEntity.ProductCode, productDescription)
                                If Not String.IsNullOrEmpty(productDataEntity.ErrorCode) Then
                                    errorMessageItem.Text = productDescription & " - " & _errMsg.GetErrorMessage(TCUtilities.GetAllString, _wfrPage.PageCode, productDataEntity.ErrorCode).ERROR_MESSAGE
                                Else
                                    errorMessageItem.Text = _wfrPage.Content("SuccessfullyAddedToBasket", _languageCode, True).Replace("<<PRODUCT_DESCRIPTION>>", productDescription)
                                End If
                                If Not blErrorMessages.Items.Contains(errorMessageItem) Then blErrorMessages.Items.Add(errorMessageItem)
                            Next
                            If blSuccessMessages.Items.Count > 0 Then Profile.Basket = Profile.Provider.GetBasket(Profile.UserName, True)
                        Else
                            ltlErrorMessage.Text = _errMsg.GetErrorMessage(err.ErrorNumber).ERROR_MESSAGE
                        End If
                    Else
                        performBasketRedirect()
                    End If
                Else
                    If TEBUtilities.CheckForDBNull_Boolean_Defaultfalse(_wfrPage.Attribute("ValidateQuantityForAdd"))
                        ltlErrorMessage.Text = _errMsg.GetErrorMessage("QZ6").ERROR_MESSAGE
                    else
                        performBasketRedirect()
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Basket Button method
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub basketButtonClick()
        Page.Validate()
        If Page.IsValid Then
            performBasketRedirect()
        End If
    End Sub

    ''' <summary>
    ''' Perform the basket redirect based on product type and system defaults.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub performBasketRedirect()
        Dim affectedRows As Integer = TDataObjects.BasketSettings.TblBasketHeader.UpdateLinkedProductMaster(Profile.Basket.Basket_Header_ID, String.Empty)
        Dim redirectUrl As String = "~/PagesPublic/Basket/Basket.aspx"
        Select Case _productTypeLinkingFrom
            Case Is = GlobalConstants.HOMEPRODUCTTYPE
                If Not ModuleDefaults.HomeProduct_ForwardToBasket Then redirectUrl = "~/PagesPublic/ProductBrowse/ProductHome.aspx?ProductSubtype=" & _productSubTypeLinkingFrom
            Case Is = GlobalConstants.AWAYPRODUCTTYPE
                If Not ModuleDefaults.AwayProduct_ForwardToBasket Then redirectUrl = "~/PagesPublic/ProductBrowse/ProductAway.aspx?ProductSubtype=" & _productSubTypeLinkingFrom
            Case Is = GlobalConstants.TRAVELPRODUCTTYPE
                If Not ModuleDefaults.TravelProduct_ForwardToBasket Then redirectUrl = "~/PagesPublic/ProductBrowse/ProductTravel.aspx?ProductSubtype=" & _productSubTypeLinkingFrom
            Case Is = GlobalConstants.EVENTPRODUCTTYPE
                If Not ModuleDefaults.EventProduct_ForwardToBasket Then redirectUrl = "~/PagesPublic/ProductBrowse/ProductEvent.aspx?ProductSubtype=" & _productSubTypeLinkingFrom
            Case Is = GlobalConstants.MEMBERSHIPPRODUCTTYPE
                If Not ModuleDefaults.MembershipProduct_ForwardToBasket Then redirectUrl = "~/PagesPublic/ProductBrowse/ProductMembership.aspx?ProductSubtype=" & _productSubTypeLinkingFrom
        End Select
        Response.Redirect(redirectUrl)
    End Sub

    ''' <summary>
    ''' Set the price code drop down list if required
    ''' </summary>
    ''' <param name="plhPriceCodeDropdown">The price code drop down list place holder</param>
    ''' <param name="item">The current repeater item being bound</param>
    ''' <param name="dtProductPriceCodes">The price codes data table from WS007R</param>
    ''' <param name="productCode">The current product code being bound</param>
    ''' <param name="stadiumCode">The current stadium code of the product being bound</param>
    ''' <remarks></remarks>
    Private Sub setupPriceCodeDropdownList(ByRef plhPriceCodeDropdown As PlaceHolder, ByRef item As RepeaterItem, ByRef dtProductPriceCodes As DataTable, ByVal productCode As String, ByVal stadiumCode As String)
        If dtProductPriceCodes.Rows.Count > 0 Then
            Dim tPricing As New TalentPricing
            Dim productDataEntity As New DEProductDetails
            Dim settings As DESettings = TEBUtilities.GetSettingsObject()
            Dim err As New ErrorObj
            settings.Cacheing = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(_wfrPage.Attribute("PriceCodeDescriptionsCaching"))
            settings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_wfrPage.Attribute("PriceCodeDescriptionsCacheTime"))
            productDataEntity.StadiumCode = stadiumCode
            productDataEntity.ProductCode = productCode
            productDataEntity.ProductType = GlobalConstants.AWAYPRODUCTTYPE
            productDataEntity.Src = GlobalConstants.SOURCE
            tPricing.De = productDataEntity
            tPricing.Settings = settings
            err = tPricing.PriceCodeDetails()

            If Not err.HasError AndAlso tPricing.TalDataSet IsNot Nothing Then
                If tPricing.TalDataSet.DictionaryOfPriceCodes.Count > 0 Then
                    Dim hasPriceCodes As Boolean = True
                    Dim ddlPriceCodes As DropDownList = CType(item.FindControl("ddlPriceCodes"), DropDownList)
                    For Each row As DataRow In dtProductPriceCodes.Rows
                        Dim priceCode As DEPriceCode = Nothing
                        If tPricing.TalDataSet.DictionaryOfPriceCodes.TryGetValue(TEBUtilities.CheckForDBNull_String(row("PriceCode")), priceCode) Then
                            hasPriceCodes = True
                            ddlPriceCodes.Items.Add(New ListItem(priceCode.LongDescription, priceCode.PriceCode))
                        End If
                    Next
                    If hasPriceCodes Then
                        Dim hdfPriceCode As HiddenField = CType(item.FindControl("hdfPriceCode"), HiddenField)
                        Dim lblPriceCodeDropdownDescription As Label = CType(item.FindControl("lblPriceCodeDropdownDescription"), Label)
                        lblPriceCodeDropdownDescription.Text = _wfrPage.Content("PleaseSelectText", _languageCode, True)
                        plhPriceCodeDropdown.Visible = True
                        hdfPriceCode.Value = String.Empty
                    End If
                End If
            End If
        End If
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Set the product details for the intial main product added to get to this page
    ''' </summary>
    ''' <returns>True if the product details have been set correctly</returns>
    ''' <remarks></remarks>
    Private Function setProductLinkingFromDetails() As Boolean
        Dim hasError As Boolean = True
        Dim priceCode As String = String.Empty
        Dim seatDetailsCollection As New List(Of DESeatDetails)
        Dim count As Integer = 0
        Dim numberOfSeatsLimited As Boolean = False
        _quantityRequested = 0
        For Each item As TalentBasketItem In Profile.Basket.BasketItems
            If item.Product = _productCodeLinkingFrom Then
                count += 1
                If count <= TEBUtilities.CheckForDBNull_Int(_wfrPage.Attribute("MaxSeatListDiplay")) Then
                    Dim seat As New DESeatDetails
                    seat.Stand = TEBUtilities.GetStandFromSeatDetails(item.SEAT).Trim()
                    seat.Area = TEBUtilities.GetAreaFromSeatDetails(item.SEAT).Trim()
                    seat.Row = TEBUtilities.GetRowFromSeatDetails(item.SEAT).Trim()
                    seat.Seat = TEBUtilities.GetSeatFromSeatDetails(item.SEAT).Trim()
                    seatDetailsCollection.Add(seat)
                End If
                priceCode = item.PRICE_CODE
                If Session("QuantityRequested") Is Nothing Then
                    _quantityRequested += CInt(item.Quantity)
                Else
                    _quantityRequested = TEBUtilities.CheckForDBNull_Int(Session("QuantityRequested"))
                End If
                _linkedProductId = item.LINKED_PRODUCT_ID
                hasError = False
            End If
        Next

        If Not hasError Then
            _talentProduct.ResultDataSet = Nothing
            _talentProduct.De.ProductCode = _productCodeLinkingFrom
            _talentProduct.De.PriceCode = priceCode
            _talentProduct.De.CampaignCode = _priceCodeLinkingFrom 'get this from the session variable set on page load
            Dim err As ErrorObj = _talentProduct.ProductDetails
            If Not err.HasError AndAlso _talentProduct.ResultDataSet IsNot Nothing AndAlso _talentProduct.ResultDataSet.Tables.Count = 5 Then
                If _talentProduct.ResultDataSet.Tables(0).Rows.Count > 0 AndAlso _talentProduct.ResultDataSet.Tables(0).Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
                    hasError = True
                Else
                    imgOpposition.ImageUrl = ImagePath.getImagePath("PRODTICKETING", _talentProduct.ResultDataSet.Tables(2).Rows(0)("OppositionCode").ToString(), _wfrPage.BusinessUnit, _wfrPage.PartnerCode)
                    imgOpposition.Visible = (imgOpposition.ImageUrl <> ModuleDefaults.MissingImagePath)
                    ltlProductDescription1.Text = _talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductDescription").ToString().Trim()
                    Dim longProductDescription As New StringBuilder
                    longProductDescription.Append(_talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductText1").ToString().Trim())
                    longProductDescription.Append(_talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductText2").ToString().Trim())
                    longProductDescription.Append(_talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductText3").ToString().Trim())
                    longProductDescription.Append(_talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductText4").ToString().Trim())
                    longProductDescription.Append(_talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductText5").ToString().Trim())
                    ltlProductDescription2.Text = longProductDescription.ToString()
                    plhProductDescription2.Visible = (ltlProductDescription2.Text.Length > 0)
                    Dim productDate As String = _talentProduct.ResultDataSet.Tables(2).Rows(0)("MDTE08Date").ToString()
                    Dim productTime As String = _talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductTime").ToString()
                    If Not _talentProduct.ResultDataSet.Tables(2).Rows(0)("HideDate") AndAlso productDate.Length > 0 Then
                        ltlDate.Text = TEBUtilities.GetFormattedDateAndTime(productDate, String.Empty, " ", ModuleDefaults.GlobalDateFormat, ModuleDefaults.Culture)
                    End If
                    If Not _talentProduct.ResultDataSet.Tables(2).Rows(0)("HideTime") Then
                        ltlTime.Text = _talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductTime").ToString().Trim()
                    End If
                    If Trim(ltlDate.Text) = String.Empty AndAlso Trim(ltlTime.Text) = String.Empty Then
                        plhDateAndTime.Visible = False
                    End If
                    If count <= TEBUtilities.CheckForDBNull_Int(_wfrPage.Attribute("MaxSeatListDiplay")) Then
                        ltlQuantityRequestedValue.Text = CInt(_quantityRequested)
                    Else
                        ltlQuantityRequestedValue.Text = _wfrPage.Content("QuantityRequestedLimitMessage", _languageCode, True).Replace("<<TOTAL_VISIBLE>>", seatDetailsCollection.Count).Replace("<<QUANTITY_REQUESTED>>", _quantityRequested)
                    End If

                    _productLinkingFromStadiumCode = _talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductStadium").ToString().Trim()
                    _productTypeLinkingFrom = _talentProduct.ResultDataSet.Tables(2).Rows(0)("ProductType").ToString().Trim()
                    If _productTypeLinkingFrom = GlobalConstants.HOMEPRODUCTTYPE OrElse _productTypeLinkingFrom = GlobalConstants.SEASONTICKETPRODUCTTYPE AndAlso seatDetailsCollection.Count > 0 Then
                        rptSeatDetails.DataSource = seatDetailsCollection
                        rptSeatDetails.DataBind()
                        plhSeatDetails.Visible = True
                    Else
                        plhSeatDetails.Visible = False
                    End If
                End If
            Else
                hasError = True
            End If
        End If
        Return hasError
    End Function

    ''' <summary>
    ''' Determine if the allow price exception flag should be true or false based on product code and price code details
    ''' </summary>
    ''' <param name="productType">The current product type</param>
    ''' <param name="priceCode">The current price code string value</param>
    ''' <returns><c>true</c> if the product type is A or M and there is no given price code otherwise <c>false</c></returns>
    ''' <remarks></remarks>
    Private Function setAllowPriceException(ByVal productType As String, ByVal priceCode As String) As Boolean
        Dim allowPriceException As Boolean = False
        If productType = GlobalConstants.AWAYPRODUCTTYPE OrElse productType = GlobalConstants.MEMBERSHIPPRODUCTTYPE Then
            If priceCode = String.Empty Then
                allowPriceException = True
            End If
        End If
        Return allowPriceException
    End Function

    ''' <summary>
    ''' This function gets the quantity available for the product in action
    ''' </summary>
    ''' <param name="productListResults">Product list results</param>
    ''' <param name="productCode">Product code</param>
    ''' <param name="productType">Product type</param>
    ''' <param name="productSubType">Product sub type</param>
    ''' <param name="stadiumCode">Stadium code</param>
    ''' <returns>The count of quantity available</returns>
    ''' <remarks></remarks>
    Private Function GetQuantityAvailable(ByRef productListResults As DataTable, ByVal productCode As String, ByVal productType As String, ByVal productSubType As String, ByVal stadiumCode As String) As Integer
        Dim quantityAvailable As Integer = 0
        For Each row As DataRow In productListResults.Rows
            If (row("ProductCode").ToString().Trim() = productCode.Trim()) Then
                quantityAvailable = AvailabilityTotal(row("capacityCnt").ToString.Trim, row("returnsCnt").ToString.Trim, _
                                                      row("salesCnt").ToString.Trim, row("unavailableCnt").ToString.Trim, _
                                                      row("bookingsCnt").ToString.Trim, row("reservationsCnt").ToString.Trim)
            End If
        Next
        Return quantityAvailable
    End Function

    ''' <summary>
    ''' Calculate the available total figure
    ''' </summary>
    ''' <param name="capacityCount">The given capacity</param>
    ''' <param name="returnsCount">The given returns number</param>
    ''' <param name="salesCount">The number of sales made</param>
    ''' <param name="unavailableCount">The availability count</param>
    ''' <param name="bookingsCount">The number of bookings count</param>
    ''' <param name="reservationsCount">The number of reservations made</param>
    ''' <returns>The number of returned tickets</returns>
    ''' <remarks></remarks>
    Private Function AvailabilityTotal(ByVal capacityCount As String, ByVal returnsCount As String, ByVal salesCount As String,
    ByVal unavailableCount As String, ByVal bookingsCount As String, ByVal reservationsCount As String) As Integer
        Dim total As Integer = 0
        Try
            Dim intCapacityCount As Integer = CInt(capacityCount)
            Dim intReturnsCount As Integer = CInt(returnsCount)
            Dim intSalesCount As Integer = CInt(salesCount)
            Dim intUnavailableCount As Integer = CInt(unavailableCount)
            Dim intBookingsCount As Integer = CInt(bookingsCount)
            Dim intReservationsCount As Integer = CInt(reservationsCount)
            If intCapacityCount > 0 Then
                total = intCapacityCount + intReturnsCount
                total = total - intSalesCount - intUnavailableCount - intBookingsCount - intReservationsCount
            End If
        Catch
        End Try
        Return total
    End Function

    ''' <summary>
    ''' This method adds the tickets from the price band selection, based on the quantity user selects in the dropdown list
    ''' </summary>
    ''' <param name="priceBandSelectionControl">Price band selection control</param>
    ''' <param name="productType">Product type</param>
    ''' <param name="productCode">Product code</param>
    ''' <param name="priceCode">Price code</param>
    ''' <param name="campaignCode">Campaign code</param>
    ''' <param name="productIsMandatory">Product is mandatory</param>
    ''' <param name="listOfTicketingItems">List of ticketing items</param>
    ''' <remarks></remarks>
    Private Function AddTicketsFromPriceBandSelection(ByRef priceBandSelectionControl As UserControls_PriceBandSelection, ByVal productType As String, ByVal productCode As String, ByVal priceCode As String, ByVal campaignCode As String, ByVal productIsMandatory As Boolean, ByVal listOfTicketingItems As List(Of DEAddTicketingItems)) As Integer
        Dim selectedQuantity As Integer = 0
        Dim productDetailCode As String = GetSelectedTravelOption(priceBandSelectionControl)
        Dim priceBands As Dictionary(Of String, Integer) = priceBandSelectionControl.GetPriceBands()
        For Each item As KeyValuePair(Of String, Integer) In priceBands
            Dim deATI As New DEAddTicketingItems
            With deATI
                .ProductCode = productCode
                .ProductDetailCode = productDetailCode
                .CampaignCode = campaignCode
                .ProductIsMandatory = productIsMandatory
                .LinkedParentProductCode = _productCodeLinkingFrom
                .PriceCode = priceCode
                .PriceBand01 = item.Key
                .Quantity01 = item.Value
                selectedQuantity += item.Value
            End With
            listOfTicketingItems.Add(deATI)
        Next
        Return selectedQuantity
    End Function

    ''' <summary>
    ''' This function gets the selected value from the travel options
    ''' dropdown list
    ''' </summary>
    ''' <param name="priceBandSelectionControl">Price band selection control</param>
    ''' <returns>Selected value</returns>
    ''' <remarks></remarks>
    Private Function GetSelectedTravelOption(ByRef priceBandSelectionControl As UserControls_PriceBandSelection) As String
        Return CType(priceBandSelectionControl.FindControl("ddlTravelOptions"), DropDownList).SelectedValue.Trim
    End Function

#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Return a css class name if the given product is mandatory
    ''' </summary>
    ''' <param name="isMandatory">Is the product mandatory</param>
    ''' <returns>The css class name</returns>
    ''' <remarks></remarks>
    Public Function GetMandatoryClassName(ByVal isMandatory As Boolean) As String
        Dim mandatoryCssClass As String = String.Empty
        If isMandatory Then
            mandatoryCssClass = " ebiz-mandatory-product"
        End If
        Return mandatoryCssClass
    End Function

    ''' <summary>
    ''' Get the correct column CSS layout when there is a seat selection option
    ''' </summary>
    ''' <param name="productType">The current product type</param>
    ''' <returns>The css class name</returns>
    ''' <remarks></remarks>
    Public Function GetCSSColumnClass(ByVal productType As String) As String
        Dim cssClassName As String = "medium-12"
        If productType = GlobalConstants.HOMEPRODUCTTYPE OrElse productType = GlobalConstants.SEASONTICKETPRODUCTTYPE Then
            cssClassName = "medium-6"
        End If
        Return cssClassName
    End Function

#End Region

End Class