Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Xml
Imports System.Globalization
Imports Talent.UI
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Product Search Results
'
'       Date                        01/04/07
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCPLIST- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_ProductSearchResultsList
    Inherits AbstractProductSearchList
    Protected Property BlockGridStyleClass() As String = String.Empty
    ' BF - Removed 'Handles me.load from here as it is not required because it overrides the base page load
    ' If 'handles..' is put in then page_load get's called twice
    Protected Overrides Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)
        MyBase.Page_Load(sender, e)
        If Display Then
            If IsValidID(Request("page")) Then
                _PageNumber = Int32.Parse(Request("page"))
                _IsPaging = True
            End If
            pnlProductSearchResultsList.Visible = True
            txtSortBy.Text = ucr.Content("SortByText", _languageCode, True)
            If Not Page.IsPostBack Then
                'populate order by ddl
                With orderddl.Items
                    .Clear()
                    .Add(New ListItem(ucr.Content("BestSellerSortText", _languageCode, True), bestSeller))
                    .Add(New ListItem(ucr.Content("PriceAscSortText", _languageCode, True), priceAsc))
                    .Add(New ListItem(ucr.Content("PriceDescSortText", _languageCode, True), priceDesc))
                    .Add(New ListItem(ucr.Content("NameAscSortText", _languageCode, True), nameAsc))
                    .Add(New ListItem(ucr.Content("NameDescSortText", _languageCode, True), nameDesc))
                End With
                Try
                    orderddl.SelectedValue = Session("orderddl_selectedValue")
                Catch ex As Exception
                End Try
            End If
            If Not Page.IsPostBack Then
                LoadProducts()
                If completeSearchProductsList.Products.Count = 1 Then
                    If Request.QueryString("searchterm") IsNot Nothing Then
                        If Request.QueryString("searchterm").ToUpper() = CType(completeSearchProductsList.Products(0), Product).Code.ToUpper() Then
                            Response.Redirect(CType(completeSearchProductsList.Products(0), Product).NavigateURL)
                        End If
                    End If
                End If
            End If
        Else
            pnlProductSearchResultsList.Visible = False
        End If
        If Not Page.IsPostBack Then
            setupPager()
        End If
        ' Hide show all button if no text is setup
        If ShowAllButton.Text = String.Empty Then
            ShowAllButton.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Set s the dislpay of the pager based upon whether
    ''' we are showing all products or not.
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub setupPager()
        If ShowAllItems Then
            pagerDisplayString.Visible = False
            pagerDisplayString2.Visible = False
            pagerDisplayString3.Visible = False
            pagerDisplayString4.Visible = False
        End If
        If IsValidID(Request.QueryString("page")) And Not ShowAllItems() Then
            _PageNumber = Int32.Parse(Request.QueryString("page"))
            _IsPaging = True
        End If
        ShowAllButton.Text = ShowAllButtonText
    End Sub

    Protected Overrides Sub setSearchResultsLabelText(ByVal text As String)
        lblSearchResults.Text = text
    End Sub

    Protected Overrides Function getOrderDropDownListSelectedValue() As String
        Return orderddl.SelectedValue
    End Function

    Protected Overrides Sub displayProductsList(ByVal pageProductsList As IList)
        If Not pageProductsList Is Nothing Then
            rptProducts.DataSource = pageProductsList
            rptProducts.DataBind()
        End If

        If completeSearchProductsList.Products.Count <= completeSearchProductsList.PageSize Then
            ShowAllButton.Visible = False
        End If
    End Sub


    '--------------
    ' Add to basket
    '--------------
    Protected Sub rptProducts_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptProducts.ItemCommand
        lblError.Text = String.Empty
        Dim quant As Integer = 1
        Dim strProduct As String = e.CommandArgument.ToString

        If ucr Is Nothing Then
            With ucr
                .BusinessUnit = TalentCache.GetBusinessUnit()
                .PageCode = Usage()
                .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "productList.ascx"
            End With
        End If
        
        '-------------------------
        ' Check if enough in stock
        '-------------------------
        Dim availQty As Integer = Stock.GetStockBalance(strProduct)
        If quant > availQty And ModuleDefaults.FrontEndStockRequiredToAddToBasket Then
            lblError.Text = ucr.Content("stockError", _languageCode, True)
        Else

            Dim al As New ArrayList
            For Each strKey As String In Request.QueryString.Keys
                If strKey.ToLower.Contains("group") Then
                    al.Add(Request.QueryString(strKey))
                End If
            Next

            Dim tbi As New TalentBasketItem
            With tbi
                .Product = strProduct
                .Quantity = quant
                Dim products As Data.DataTable = Talent.eCommerce.Utilities.GetProductInfo(strProduct)
                If products IsNot Nothing Then
                    If products.Rows.Count > 0 Then
                        .ALTERNATE_SKU = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("ALTERNATE_SKU"))
                        .PRODUCT_DESCRIPTION1 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_1"))
                        .PRODUCT_DESCRIPTION2 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_2"))
                        .PRODUCT_DESCRIPTION3 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_3"))
                        .PRODUCT_DESCRIPTION4 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_4"))
                        .PRODUCT_DESCRIPTION5 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_5"))
                        .WEIGHT = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(products.Rows(0)("PRODUCT_WEIGHT"))
                    End If
                End If
                Select Case ModuleDefaults.PricingType
                    Case 2
                        Dim prices As DataTable = Talent.eCommerce.Utilities.GetChorusPrice(strProduct, quant)
                        If prices.Rows.Count > 0 Then
                            .Gross_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("GrossPrice"))
                            .Net_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("NetPrice"))
                            .Tax_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("TaxPrice"))
                        End If
                    Case Else
                        Dim deWp As DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(strProduct, quant, strProduct)
                        .Gross_Price = deWp.Purchase_Price_Gross
                        .Net_Price = deWp.Purchase_Price_Net
                        .Tax_Price = deWp.Purchase_Price_Tax
                End Select
                Try
                    .GROUP_LEVEL_01 = al(0)
                    .GROUP_LEVEL_02 = al(1)
                    .GROUP_LEVEL_03 = al(2)
                    .GROUP_LEVEL_04 = al(3)
                    .GROUP_LEVEL_05 = al(4)
                    .GROUP_LEVEL_06 = al(5)
                    .GROUP_LEVEL_07 = al(6)
                    .GROUP_LEVEL_08 = al(7)
                    .GROUP_LEVEL_09 = al(8)
                    .GROUP_LEVEL_10 = al(9)
                Catch ex As Exception
                End Try
                If Not Profile.IsAnonymous And Not Profile.PartnerInfo.Details Is Nothing Then
                    .Cost_Centre = Profile.PartnerInfo.Details.COST_CENTRE
                    .Account_Code = Order.GetLastAccountNo(Profile.User.Details.LoginID)
                End If
            End With
            Profile.Basket.AddItem(tbi)

            If ucr Is Nothing Then
                With ucr
                    .BusinessUnit = TalentCache.GetBusinessUnit()
                    .PageCode = Usage()
                    .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                    .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                    .KeyCode = "productList.ascx"
                End With
            End If
            If CType(ucr.Attribute("forwardToBasket"), Boolean) Then
                Response.Redirect("../../pagesPublic/basket/Basket.aspx")
            Else
                Response.Redirect(Request.Url.ToString)
            End If
        End If

    End Sub

    Protected Sub rptProducts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProducts.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            
            Dim p As New Talent.eCommerce.Product
            Try
                p = CType(e.Item.DataItem, Talent.eCommerce.Product)
                If Talent.eCommerce.Stock.GetStockBalance(p.Code) < 1 And ModuleDefaults.FrontEndStockRequiredToAddToBasket Then
                    CType(e.Item.FindControl("plhNoStock"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("plhAction"), PlaceHolder).Visible = False
                    Dim stockTime As String = String.Empty
                    Dim reStockCode As String = String.Empty
                    CType(e.Item.FindControl("NoStockLabel"), Literal).Text = Talent.eCommerce.Stock.GetNoStockDescription(p.Code, stockTime, reStockCode)
                Else
                    If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
                        CType(e.Item.FindControl("plhAction"), PlaceHolder).Visible = Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(ModuleDefaults.AllowMasterProductsToBePurchased)
                    End If
                End If
            Catch ex As Exception
            End Try

            '---------------------------
            ' Set attributes in Datalist
            '---------------------------

            Try

                Dim img As Image = CType(e.Item.FindControl("promoImage"), Image)

                If p.WebPrices.IsPartOfPromotion AndAlso Not String.IsNullOrEmpty(p.WebPrices.PromotionImagePath) Then
                    img.ImageUrl = ImagePath.getImagePath("PROMOMOTION", p.WebPrices.PromotionImagePath, TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile))
                    img.AlternateText = p.WebPrices.PromotionDescriptionText
                Else
                    img.Visible = False
                End If

                'If no promo is found, or there is more than one required product
                'then just show the normal price
                If p.WebPrices.IsPartOfPromotion AndAlso ModuleDefaults.ShowFromPrices Then
                    CType(e.Item.FindControl("lblPriceText"), Literal).Text = ucr.Content("FromPriceLabel", _languageCode, True)
                    CType(e.Item.FindControl("lblPrice"), Literal).Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice_From, ucr.BusinessUnit, ucr.PartnerCode)
                Else
                    CType(e.Item.FindControl("lblPriceText"), Literal).Text = ucr.Content("PriceLabel", _languageCode, True)
                    CType(e.Item.FindControl("lblPrice"), Literal).Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice, ucr.BusinessUnit, ucr.PartnerCode)
                End If
                If Not Talent.eCommerce.Utilities.ShowPrices(Profile) Then
                    CType(e.Item.FindControl("lblPriceText"), Literal).Visible = False
                    CType(e.Item.FindControl("lblPrice"), Literal).Visible = False
                End If

                CType(e.Item.FindControl("btnBuy"), Button).Text = ucr.Content("BuyButtonText", _languageCode, True)
                Try
                    CType(e.Item.FindControl("txtQuantity"), TextBox).Columns = CInt(ucr.Attribute("QuantityTextBoxSize"))
                Catch
                End Try
                Try
                    CType(e.Item.FindControl("txtQuantity"), TextBox).Text = CInt(ModuleDefaults.Default_Add_Quantity)
                Catch
                End Try

                If p.WebPrices.PRICE_BREAK_QUANTITY_1 >= p.WebPrices.PRICE_BREAK_QUANTITY_2 Then
                    CType(e.Item.FindControl("lblPriceText"), Literal).Text = ucr.Content("PriceLabel", _languageCode, True)
                End If
            Catch ex As Exception

            End Try
        End If

    End Sub

    Protected Sub orderddl_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles orderddl.SelectedIndexChanged
        Session("orderddl_selectedValue") = orderddl.SelectedValue.ToString()
        LoadProducts()
    End Sub

    Protected Sub ShowAllButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ShowAllButton.Click
        'We let the parent class handle the redirect
        handleShowAllRedirect()
    End Sub

    Public Function FormatUrl(ByVal navigateUrl As String) As String
        Dim url As String = String.Empty
        If navigateUrl IsNot Nothing AndAlso navigateUrl.Length > 0 Then
            url = Replace(navigateUrl, " ", "%20")
        End If
        Return url
    End Function
    Protected Sub GetRegExErrorText(ByVal sender As Object, ByVal e As EventArgs)
        Dim regex As RegularExpressionValidator = (CType(sender, RegularExpressionValidator))
        Select Case regex.ID
            Case Is = "QuantityValidator"
                regex.ErrorMessage = ucr.Content("QuantityValidatorErrorText", _languageCode, True)
        End Select
    End Sub
    Protected Overrides Sub setupUCR()
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Usage()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "productSearchResultsList.ascx"
        End With
        BlockGridStyleClass = ucr.Attribute("BlockGridStyleClass")
    End Sub

End Class
