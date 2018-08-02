Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.data

Partial Class UserControls_ProductControls
    Inherits ControlBase

#Region "Private Variables"

    Private _ucr As Talent.Common.UserControlResource = Nothing
    Private _languageCode As String = String.Empty
    Private _product As String = String.Empty
    Private _businessUnit As String = String.Empty
    Private _partner As String = String.Empty
    Private _group1 As String = String.Empty

#End Region

#Region "Public Properties"

    Public Property ErrorMessage() As String

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Request.QueryString("product") Is Nothing Then
            _product = Request.QueryString("product")
        End If
        If Not Request.QueryString("group1") Is Nothing Then
            _group1 = Request.QueryString("group1")
        End If

        _businessUnit = TalentCache.GetBusinessUnit
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        _ucr = New Talent.Common.UserControlResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage()
        Dim productTemplate As String = String.Empty

        If _product <> String.Empty Then
            With _ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = ProfileHelper.GetPageName
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "ProductControls.ascx"
                If Not ShowPrices(Profile) Then
                    lblPriceText.Visible = False
                    lblPrice.Visible = False
                Else
                    If ((Not Profile.IsAnonymous) AndAlso (Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(Profile.PartnerInfo.Details.HIDE_PRICES))) Then
                        lblPriceText.Visible = False
                        lblPrice.Visible = False
                    Else
                        lblPriceText.Text = .Content("PriceText", _languageCode, True)
                    End If
                End If

                Dim prc As Talent.Common.DEWebPrice
                If ProductOptions1.HasItems Then
                    Dim masterProduct As String = _product
                    Dim product As String = _product
                    Try
                        product = masterProduct
                        Dim ddlOptionLevel1 As DropDownList = CType(ProductOptions1.FindControl("ddlOptionLevel1"), DropDownList)
                        Dim ddlOptionLevel2 As DropDownList = CType(ProductOptions1.FindControl("ddlOptionLevel2"), DropDownList)
                        Dim ddlOptionLevel3 As DropDownList = CType(ProductOptions1.FindControl("ddlOptionLevel3"), DropDownList)
                        If Not String.IsNullOrEmpty(ddlOptionLevel1.SelectedValue) Then product &= ddlOptionLevel1.SelectedValue
                        If Not String.IsNullOrEmpty(ddlOptionLevel2.SelectedValue) Then product &= ddlOptionLevel2.SelectedValue
                        If Not String.IsNullOrEmpty(ddlOptionLevel3.SelectedValue) Then product &= ddlOptionLevel3.SelectedValue
                    Catch ex As Exception
                        prc = Utilities.GetWebPrices(_product, 0, _product)
                    End Try
                    prc = Utilities.GetWebPrices(product, 0, masterProduct)
                Else
                    prc = Utilities.GetWebPrices(_product, 0, _product)
                End If

                If prc.IsSalePrice Then
                    lblSalePrice.Visible = True
                    lblSalePriceText.Visible = True
                    If ModuleDefaults.ShowFromPrices Then
                        lblSalePrice.Text = TDataObjects.PaymentSettings.FormatCurrency(prc.DisplayPrice_From, _ucr.BusinessUnit, _ucr.PartnerCode)
                    Else
                        lblSalePrice.Text = TDataObjects.PaymentSettings.FormatCurrency(prc.DisplayPrice, _ucr.BusinessUnit, _ucr.PartnerCode)
                    End If
                    lblSalePriceText.Text = .Content("SalePriceText", _languageCode, True)
                    lblPriceText.Text = .Content("SalePriceWasText", _languageCode, True)
                    If ModuleDefaults.ShowPricesExVAT Then
                        lblPrice.Text = TDataObjects.PaymentSettings.FormatCurrency(prc.NET_PRICE, _ucr.BusinessUnit, _ucr.PartnerCode)
                    Else
                        lblPrice.Text = TDataObjects.PaymentSettings.FormatCurrency(prc.GROSS_PRICE, _ucr.BusinessUnit, _ucr.PartnerCode)
                    End If
                Else
                    If ModuleDefaults.ShowFromPrices Then
                        lblPrice.Text = TDataObjects.PaymentSettings.FormatCurrency(prc.DisplayPrice_From, _ucr.BusinessUnit, _ucr.PartnerCode)
                    Else
                        lblPrice.Text = TDataObjects.PaymentSettings.FormatCurrency(prc.DisplayPrice, _ucr.BusinessUnit, _ucr.PartnerCode)
                    End If
                End If

                lblQuantity.Text = .Content("QuantityText", _languageCode, True)				
				If Talent.eCommerce.Utilities.IsPartnerHomeDeliveryType(Profile) AndAlso isQuickOrderInUse() Then
                btnAddToBasket.Text = .Content("AddToBasketButtonTextForHomeDelivery", _languageCode, True)								
                Else
                btnAddToBasket.Text =.Content("AddToBasketText", _languageCode, True)  
                End If
				
                txtQuantity.Columns = .Attribute("QuantityTextBoxSize")
                cvaldQuantityStock.ErrorMessage = .Content("txtQuantityStockError", _languageCode, True)
                cvaldNegative.ErrorMessage = .Content("txtQuantityNegativeError", _languageCode, True)
            End With
        End If

        '
        ' Get product information template from top level group to determine what ucl to display
        Dim topLevelInfo As New TalentGroupInformationTableAdapters.tbl_group_level_01TableAdapter
        Dim dt As Data.DataTable = topLevelInfo.GetDataByBU_Partner_Group(_businessUnit, _partner, _group1)
        If dt.Rows.Count > 0 Then
            productTemplate = dt.Rows(0)("GROUP_L01_PRODUCT_PAGE_TEMPLATE")
        Else
            dt = topLevelInfo.GetDataByBU_Partner_Group(_businessUnit, Talent.Common.Utilities.GetAllString, _group1)
            If dt.Rows.Count > 0 Then
                productTemplate = dt.Rows(0)("GROUP_L01_PRODUCT_PAGE_TEMPLATE")
            Else
                dt = topLevelInfo.GetDataByBU_Partner_Group(Talent.Common.Utilities.GetAllString, _partner, _group1)
                If dt.Rows.Count > 0 Then
                    productTemplate = dt.Rows(0)("GROUP_L01_PRODUCT_PAGE_TEMPLATE")
                Else
                    dt = topLevelInfo.GetDataByBU_Partner_Group(Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString, _group1)
                End If
            End If

        End If

        Try
            If ModuleDefaults.FrontEndStockRequiredToAddToBasket Then
                OutOfStockLabel.Visible = True
                Dim stockTime As String = String.Empty
                Dim reStockCode As String = String.Empty
                OutOfStockLabel.Text = Talent.eCommerce.Stock.GetNoStockDescription(_product, stockTime, reStockCode)
                If (reStockCode <> "" AndAlso _ucr.Attribute("UnavailableRestockCodes").Contains(reStockCode)) Then
                    buyOptions.Visible = False
                Else
                    buyOptions.Visible = True
                End If
            End If
        Catch ex As Exception

        End Try

        ' If there are options to display from the ProductOptions control, hide the price, 
        ' buy button and stock labels
        If ProductOptions1.HasItems Then
            plhNormal.Visible = False
            plhOptions.Visible = True
        ElseIf ModuleDefaults.AllowMasterProductsToBePurchased Then
            plhNormal.Visible = True
            plhOptions.Visible = False
        Else
            plhNormal.Visible = False
            plhOptions.Visible = False
        End If
    End Sub

    Protected Sub btnAddToBasket_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddToBasket.Click
        If Not Request.QueryString("product") Is Nothing And Page.IsValid Then
            _product = Request.QueryString("product")

            Dim quant As Integer = ModuleDefaults.Default_Add_Quantity
            Try
                quant = Integer.Parse(txtQuantity.Text)
            Catch
            End Try

            If quant >= ModuleDefaults.Min_Add_Quantity Then
			
			     If Talent.eCommerce.Utilities.IsPartnerHomeDeliveryType(Profile) AndAlso isQuickOrderInUse() Then
                    Dim dtMasterProduct As DataTable
                    Dim sbQry As New StringBuilder
                    dtMasterProduct = TDataObjects.ProductsSettings.TblProduct.GetProductByProductCode(_product, True, False)
                    sbQry.Append("../../PagesLogin/QuickOrder/QuickOrder.aspx?product=" & dtMasterProduct.Rows(0)("PRODUCT_ID").ToString() & "&quant=" & quant).ToString()
                    Response.Redirect(sbQry.ToString())
                Else
				     Dim tbi As New TalentBasketItem
                With tbi
                    .Product = _product

                    Dim products As Data.DataTable = Utilities.GetProductInfo(_product)
                    If products IsNot Nothing Then
                        If products.Rows.Count > 0 Then
                            .ALTERNATE_SKU = Utilities.CheckForDBNull_String(products.Rows(0)("ALTERNATE_SKU"))
                            .PRODUCT_DESCRIPTION1 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_1"))
                            .PRODUCT_DESCRIPTION2 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_2"))
                            .PRODUCT_DESCRIPTION3 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_3"))
                            .PRODUCT_DESCRIPTION4 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_4"))
                            .PRODUCT_DESCRIPTION5 = Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_5"))
                            .Xml_Config = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PERSONALISABLE"))
                            .WEIGHT = Utilities.CheckForDBNull_Decimal(products.Rows(0)("PRODUCT_WEIGHT"))
                        End If
                    End If

                    .Quantity = quant
                    If Not Profile.IsAnonymous And Not Profile.PartnerInfo.Details Is Nothing Then
                        .Cost_Centre = Profile.PartnerInfo.Details.COST_CENTRE
                        .Account_Code = Order.GetLastAccountNo(Profile.User.Details.LoginID)
                    End If
                    Select Case ModuleDefaults.PricingType
                        Case 2
                            Dim prices As Data.DataTable = Talent.eCommerce.Utilities.GetChorusPrice(.Product, .Quantity)
                            If prices.Rows.Count > 0 Then
                                .Gross_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("GrossPrice"))
                                .Net_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("NetPrice"))
                                .Tax_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("TaxPrice"))
                            End If
                        Case Else
                            Dim deWp As Talent.Common.DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(.Product, .Quantity, .Product)
                            .Gross_Price = deWp.Purchase_Price_Gross
                            .Net_Price = deWp.Purchase_Price_Net
                            .Tax_Price = deWp.Purchase_Price_Tax
                    End Select
                    Try
                        .GROUP_LEVEL_01 = Request.QueryString("group1")
                        .GROUP_LEVEL_02 = Request.QueryString("group2")
                        .GROUP_LEVEL_03 = Request.QueryString("group3")
                        .GROUP_LEVEL_04 = Request.QueryString("group4")
                        .GROUP_LEVEL_05 = Request.QueryString("group5")
                        .GROUP_LEVEL_06 = Request.QueryString("group6")
                        .GROUP_LEVEL_07 = Request.QueryString("group7")
                        .GROUP_LEVEL_08 = Request.QueryString("group8")
                        .GROUP_LEVEL_09 = Request.QueryString("group9")
                        .GROUP_LEVEL_10 = Request.QueryString("group10")
                    Catch ex As Exception
                    End Try
                End With
                Profile.Basket.AddItem(tbi)

                With _ucr
                    .BusinessUnit = TalentCache.GetBusinessUnit()
                    .PageCode = ProfileHelper.GetPageName
                    .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    .KeyCode = "ProductControls.ascx"
                End With

                If CType(_ucr.Attribute("forwardToBasket"), Boolean) Then
                    Response.Redirect("../../pagesPublic/basket/Basket.aspx")
                Else
                    Response.Redirect(Request.Url.ToString)
                End If
				
				end if		
              
			  
            Else
                ErrorMessage = String.Format(_ucr.Content("MinQuantityNotMetError", _languageCode, True), ModuleDefaults.Min_Add_Quantity.ToString("0"))
            End If
        End If

    End Sub

    Protected Sub cvaldQuantityStock_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvaldQuantityStock.ServerValidate
        '-------------------------
        ' Check if enough in stock
        '-------------------------
        Dim quant As Integer = 1
        Try
            quant = Integer.Parse(txtQuantity.Text)
        Catch
        End Try
        If ModuleDefaults.FrontEndStockRequiredToAddToBasket Then
            Dim availQty As Integer = Stock.GetStockBalance(_product)
            Dim stockTime As String = String.Empty
            Dim reStockCode As String = String.Empty
            Talent.eCommerce.Stock.GetNoStockDescription(_product, stockTime, reStockCode)
            If quant > availQty AndAlso reStockCode <> "" AndAlso _ucr.Attribute("UnavailableRestockCodes").Contains(reStockCode) Then
                args.IsValid = False
            End If
        End If

    End Sub

    Protected Sub cvaldNegative_ServerValidate(ByVal source As Object, ByVal args As System.Web.UI.WebControls.ServerValidateEventArgs) Handles cvaldNegative.ServerValidate
        '-------------------
        ' Check non negative
        '-------------------
        Dim quant As Integer = 1
        Try
            quant = Integer.Parse(txtQuantity.Text)
        Catch
        End Try

        If quant < 0 Then
            args.IsValid = False
        End If
    End Sub

#End Region

#Region "Public Methods"

    Protected Sub GetRegExErrorText(ByVal sender As Object, ByVal e As EventArgs)
        Dim regex As RegularExpressionValidator = (CType(sender, RegularExpressionValidator))
        Select Case regex.ID
            Case Is = "QuantityValidator"
                regex.ErrorMessage = _ucr.Content("QuantityValidatorErrorText", _languageCode, True)
        End Select
    End Sub

    Protected Sub GetRfvErrorText(ByVal sender As Object, ByVal e As EventArgs)
        Dim rfv As RequiredFieldValidator = (CType(sender, RequiredFieldValidator))
        Select Case rfv.ID
            Case Is = "rfvQuantityValidator"
                rfv.ErrorMessage = _ucr.Content("QuantityValidatorErrorText", _languageCode, True)
        End Select
    End Sub

    Protected Sub GetDefaultQuantity(ByVal sender As Object, ByVal e As EventArgs)
        CType(sender, TextBox).Text = ModuleDefaults.Default_Add_Quantity.ToString("0")
    End Sub

#End Region

#Region "Private Functions"
    ''' <summary>
    ''' Get the page "in use" flag for for the Quick Order page
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function isQuickOrderInUse() As Boolean
        Dim quickOrderInUse As Boolean = False
        Dim dtPage As New System.Data.DataTable
        dtPage = TDataObjects.PageSettings.TblPage.GetByPageCode("QuickOrder.aspx", TalentCache.GetBusinessUnit())
        If dtPage.Rows.Count > 0 Then
            quickOrderInUse = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(dtPage.Rows(0)("IN_USE"))
        End If
        Return quickOrderInUse
    End Function
#End Region

End Class