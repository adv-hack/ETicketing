Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce
Imports Talent.Common

Partial Class UserControls_ProductListGraphical2
    Inherits AbstractProductList

#Region "Class Level Fields"

    Private _emptyString As String = String.Empty
    Private _conTalent As SqlConnection = Nothing
    Private _cmdSelect As SqlCommand = Nothing
    Private _completeProductsList As ProductListGen
    Private Shadows _ucr As New Talent.Common.UserControlResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

#End Region

#Region "Constants"

    Const priceAsc As String = "priceasc"
    Const priceDesc As String = "pricedesc"
    Const nameAsc As String = "nameasc"
    Const nameDesc As String = "namedesc"
    Const bestSeller As String = "bestseller"

#End Region

#Region "Public Properties"

    Protected Property BlockGridStyleClass() As String = String.Empty
    Public Property ErrorMessage() As String = String.Empty

#End Region

    ' BF - Removed 'Handles me.load from here as it is not required because it overrides the base page load
    ' If 'handles..' is put in then page_load get's called twice
    Protected Overrides Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)
        MyBase.Page_Load(sender, e)
        If Display Then
            If IsValidID(Request("page")) Then
                _PageNumber = Int32.Parse(Request("page"))
                _IsPaging = True
            End If

            'dlsProductListGraphical.RepeatColumns = CInt(ucr.Attribute("RepeatColumns"))
            txtSortBy.Text = _ucr.Content("SortByText", _languageCode, True)

            If Not Page.IsPostBack Then
                'populate order by ddl
                With orderddl.Items
                    .Clear()
                    .Add(New ListItem(_ucr.Content("BestSellerSortText", _languageCode, True), bestSeller))
                    .Add(New ListItem(_ucr.Content("PriceAscSortText", _languageCode, True), priceAsc))
                    .Add(New ListItem(_ucr.Content("PriceDescSortText", _languageCode, True), priceDesc))
                    .Add(New ListItem(_ucr.Content("NameAscSortText", _languageCode, True), nameAsc))
                    .Add(New ListItem(_ucr.Content("NameDescSortText", _languageCode, True), nameDesc))
                End With
                If Not String.IsNullOrEmpty(Request.QueryString("order")) Then
                    Try
                        orderddl.SelectedIndex = CInt(Request.QueryString("order"))
                    Catch ex As Exception
                    End Try
                End If
            End If
            If Not Page.IsPostBack Then
                LoadProducts()
            End If

            Me.Visible = True
        Else
            Me.Visible = False
        End If

        ' Hide show all button if no text is setup
        If ShowAllButton.Text = String.Empty Then
            ShowAllButton.Visible = False
        End If
        setupPager()
    End Sub

    Protected Overrides Sub setupUCR()
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Usage()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "productListGraphical.ascx"
        End With
        BlockGridStyleClass = _ucr.Attribute("BlockGridStyleClass")
    End Sub

    ''' <summary>
    ''' Set s the dislpay of the pager based upon whether
    ''' we are showing all products or not.
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub setupPager()
        ltlNowShowingResultsTop.Text = NowShowingResultsString("TOP")
        ltlNowShowingResultsBottom.Text = NowShowingResultsString("BOTTOM")
        ltlPagingTop.Text = PagingString("TOP")
        ltlPagingBottom.Text = PagingString("BOTTOM")
        plhPagerTop.Visible = (ltlNowShowingResultsTop.Text.Length > 0 OrElse ltlPagingTop.Text.Length > 0)
        plhPagerBottom.Visible = (ltlNowShowingResultsBottom.Text.Length > 0 OrElse ltlPagingBottom.Text.Length > 0)

        If ShowAllItems Then
            plhPagerBottom.Visible = False
            ltlPagingTop.Visible = False
        End If
        If IsValidID(Request("page")) And Not ShowAllItems() Then
            _PageNumber = Int32.Parse(Request("page"))
            _IsPaging = True
        End If
        ShowAllButton.Text = ShowAllButtonText
        If ShowAllButtonText = String.Empty Then
            ShowAllButton.Visible = False
            ShowAllButton.Enabled = False
        End If
    End Sub

    Protected Sub LoadProducts()

        Dim pageProductsList As IList
        Dim err As New ErrorObj

        _caching = _ucr.Attribute("Cacheing")
        _cacheTimeMinutes = _ucr.Attribute("CacheTimeMinutes")

        _completeProductsList = RetrieveProducts()
        _completeProductsList.PageSize = CType(_ucr.Attribute("PageSize"), Integer)
        Try
            If IsOrderByChanged() Then
                Select Case orderddl.SelectedValue
                    Case Is = bestSeller
                        _caching = False
                        _completeProductsList = RetrieveProducts()
                        _completeProductsList.PageSize = CType(_ucr.Attribute("PageSize"), Integer)
                    Case Is = priceAsc
                        _completeProductsList.SortProductsByPrice("A")
                    Case Is = priceDesc
                        _completeProductsList.SortProductsByPrice("D")
                    Case Is = nameAsc
                        _completeProductsList.SortProductsByName("A")
                    Case Is = nameDesc
                        _completeProductsList.SortProductsByName("D")
                End Select
                If Session("completeProductsList") Is Nothing Then
                    Session.Add("completeProductsList", _completeProductsList)
                Else
                    Session.Remove("completeProductsList")
                    Session.Add("completeProductsList", _completeProductsList)
                End If
            End If
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Failed while sorting the complete product list"
                .ErrorNumber = "TEAPUIL-08"
                .HasError = True
            End With
        End Try

        ' -----------------------------------------------------------
        ' Retrieve the complete products list from the session and fill a 
        ' page from the complete products list
        ' ------------------------------------------------------------

        If Not _completeProductsList Is Nothing Then

            If ShowAllItems() Then
                pageProductsList = _completeProductsList.Products
            Else
                pageProductsList = _completeProductsList.GetPageProducts(_PageNumber)
            End If
            If _completeProductsList.Products.Count <= _completeProductsList.PageSize Then
                ShowAllButton.Visible = False
            End If
            Select Case ModuleDefaults.Product_List_Graphical_Template_Type
                Case 3
                    template1.Visible = False
                    template3.Visible = True
                    rptProductListGraphical2.DataSource = pageProductsList
                    rptProductListGraphical2.DataBind()

                Case Else
                    template1.Visible = True
                    template3.Visible = False
                    rptProductListGraphical.DataSource = pageProductsList
                    rptProductListGraphical.DataBind()

            End Select
        End If
    End Sub

    Private Function NowShowingResultsString(ByVal type As String) As String
        Dim startAmount As Integer = 0
        Dim endAmount As Integer = 0

        Dim displaying1 As String = _ucr.Content("displayingText1", _languageCode, True)
        Dim displaying2 As String = _ucr.Content("displayingText2", _languageCode, True)
        Dim displaying3 As String = _ucr.Content("displayingText3", _languageCode, True)
        Dim noProductsFound As String = _ucr.Content("NoProductsFoundText", _languageCode, True)
        Dim str As String = ""

        If Not _completeProductsList Is Nothing AndAlso _completeProductsList.Count > 0 Then
            startAmount = ((_PageNumber - 1) * _completeProductsList.PageSize) + 1
            If _completeProductsList.Count < (_PageNumber * _completeProductsList.PageSize) Then
                endAmount = _completeProductsList.Count
            Else
                endAmount = (_PageNumber * _completeProductsList.PageSize)
            End If
            str = "<li class=""ebiz-displaying1"">" & displaying1 & "</li><li class=""ebiz-start-amount"">" & startAmount & "</li><li class=""ebiz-displaying2"">" & displaying2 & "</li><li class=""ebiz-end-amount"">" & endAmount & "</li><li class=""ebiz-displaying3"">" & displaying3 & "</li><li class=""ebiz-product-list-count"">" & _completeProductsList.Count & "</li>"
        Else
            If type = "TOP" Then
                str = String.Empty
            Else
                str = String.Empty
            End If
        End If
        Return str
    End Function

    Private Function PagingString(ByVal type As String) As String
        Dim str As String = ""
        Dim linkPage As String = _currentPage & "?" & Request.QueryString.ToString
        Dim firstText As String = _ucr.Content("firstText", _languageCode, True)
        Dim previousText As String = _ucr.Content("previousText", _languageCode, True)
        Dim nextText As String = _ucr.Content("nextText", _languageCode, True)
        Dim lastText As String = _ucr.Content("lastText", _languageCode, True)
        Dim pageSeperator As String = _ucr.Attribute("pageSeperator")
        Dim numberOfLinks As Integer = CType(_ucr.Attribute("numberOfLinks"), Integer)
        Dim showFirstLast As Boolean = CType(_ucr.Attribute("showFirstLast"), Boolean)
        Dim showSeperator As Boolean = CType(_ucr.Attribute("showSeperator"), Boolean)
        Dim nextPreviousAsImages As Boolean = CType(_ucr.Attribute("nextPreviousAsImages"), Boolean)
        Dim previousImage As String = _ucr.Attribute("previousImage")
        Dim nextImage As String = _ucr.Attribute("nextImage")

        If linkPage.Contains("&page=") Then
            linkPage = linkPage.Substring(0, linkPage.IndexOf("&page"))
        End If
        If Not _completeProductsList Is Nothing Then
            '----------------------------------------------------------------
            'If there is only one page of data, don't display any paging info
            '----------------------------------------------------------------
            If _completeProductsList.Count <= _completeProductsList.PageSize Then
                Return ""
            End If
            '--------------------------------------------------
            'Is this a valid page for the amount of order lines
            '--------------------------------------------------
            If _PageNumber > _completeProductsList.NumberOfPages Then
                '----------------------------------------------------------------------
                'The requested page number is to high for the amount of pages available
                'Re-set the page to page 1.
                '----------------------------------------------------------------------
                _PageNumber = 1
            End If
            '--------------------------------------------
            'Display the 'First' Link and the 'Next' Link
            '--------------------------------------------
            If _PageNumber > 1 Then
                If showFirstLast Then
                    str = str & "<li><a href=""" & linkPage & "&page=1" & "&order=" & orderddl.SelectedIndex & """>" & firstText & "</a></li>"
                End If
                If nextPreviousAsImages Then
                    str = str & "<li><a href=""" & linkPage & "&page=" & (_PageNumber - 1) & "&order=" & orderddl.SelectedIndex & """>" & "<img src=""" & previousImage & """>" & "</a></li>"
                Else
                    str = str & "<li><a href=""" & linkPage & "&page=" & (_PageNumber - 1) & "&order=" & orderddl.SelectedIndex & """>" & previousText & "</a></li>"
                End If
            End If
            '-----------------------------------------------------------------------------------------------
            'Display the page links
            'No more than number_of_links Links should be displayed
            'Note: if number_of_links is an ever number and the current pagenumber is greater
            'than number_of_links then the the actual number of links displayed will be number_of_links + 1
            'this is ensure that the current page appears in the centre.
            '-----------------------------------------------------------------------------------------------
            Dim counter As Integer = 1
            If _completeProductsList.NumberOfPages <= numberOfLinks Then
                '----------------------------------------------------------
                'List out the pages, current page does not have a hyperlink
                '----------------------------------------------------------
                For counter = 1 To _completeProductsList.NumberOfPages
                    If counter = _PageNumber Then
                        If counter = _completeProductsList.NumberOfPages Or Not showSeperator Then
                            str = str & "<li class=""current""><a href=""#"">" & counter & "</a></li>"
                        Else
                            str = str & "<li><a href=""#""" & counter & "</a></li>"
                        End If
                    Else
                        If counter = _completeProductsList.NumberOfPages Or Not showSeperator Then
                            str = str & "<li><a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a></li>"
                        Else
                            str = str & "<li><a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a></li>"
                        End If
                    End If
                Next
            Else
                '------------------------------------------------------------------------------
                'If the current page is greater than number_of_links then the current page will
                'be shown in the centre of the list of links.
                '------------------------------------------------------------------------------
                If _PageNumber <= numberOfLinks Then
                    For counter = 1 To numberOfLinks
                        If counter = _PageNumber Then
                            If counter = numberOfLinks Or Not showSeperator Then
                                str = str & " " & counter & " "
                            Else
                                str = str & " " & counter & " " & pageSeperator & " "
                            End If
                        Else
                            If counter = numberOfLinks Or Not showSeperator Then
                                str = str & "<li><a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a></li>"
                            Else
                                str = str & "<li><a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a></li>"
                            End If
                        End If
                    Next
                ElseIf _PageNumber > (_completeProductsList.NumberOfPages - numberOfLinks) Then
                    '--------------------------------------------------
                    'display the last 'number_of_links number' of links
                    '--------------------------------------------------
                    For counter = (_completeProductsList.NumberOfPages - numberOfLinks) To _completeProductsList.NumberOfPages
                        If counter = _PageNumber Then
                            If counter = _completeProductsList.NumberOfPages Or Not showSeperator Then
                                str = str & " " & counter & " "
                            Else
                                str = str & " " & counter & " " & pageSeperator & " "
                            End If
                        Else
                            If counter = _completeProductsList.NumberOfPages Or Not showSeperator Then
                                str = str & "<li><a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a></li>"
                            Else
                                str = str & "<li><a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a></li>"
                            End If
                        End If
                    Next
                Else
                    Dim halfWay As Integer = numberOfLinks / 2
                    For counter = 1 To halfWay - 1
                        str = str & " <a href=""" & linkPage & "&page=" & (_PageNumber - halfWay + counter) & "&order=" & orderddl.SelectedIndex & """>" & (_PageNumber - halfWay + counter) & "</a></li>"
                    Next
                    str = str & " " & _PageNumber & " "
                    For counter = 1 To halfWay
                        If counter = halfWay Or Not showSeperator Then
                            str = str & "<li><a href=""" & linkPage & "&page=" & (_PageNumber + counter) & "&order=" & orderddl.SelectedIndex & """>" & (_PageNumber + counter) & "</a></li>"
                        Else
                            str = str & "<li><a href=""" & linkPage & "&page=" & (_PageNumber + counter) & "&order=" & orderddl.SelectedIndex & """>" & (_PageNumber + counter) & "</a></li>"
                        End If
                    Next
                End If
            End If
            '-------------------------------------------
            'Display the 'Next' link and the 'Last' link
            '-------------------------------------------
            If _PageNumber < _completeProductsList.NumberOfPages Then
                If nextPreviousAsImages Then
                    str = str & "<li><a href=""" & linkPage & "&page=" & (_PageNumber + 1) & "&order=" & orderddl.SelectedIndex & """>" & "<img src=""" & nextImage & """>" & "</a></li>"
                Else
                    str = str & "<li><a href=""" & linkPage & "&page=" & (_PageNumber + 1) & "&order=" & orderddl.SelectedIndex & """>" & nextText & "</a> "
                End If
                If showFirstLast Then
                    str = str & "<li><a href=""" & linkPage & "&page=" & _completeProductsList.NumberOfPages & "&order=" & orderddl.SelectedIndex & """>" & lastText & "</a></li>"
                End If
            End If

        End If
        Return str
    End Function

    Function IsValidID(ByVal strID As String) As Boolean
        Dim intID As Int32
        Try
            intID = Int32.Parse(strID)
        Catch
            Return False
        End Try
        '----------------------
        ' don't allow negatives
        '----------------------
        If intID < 0 Then
            Return False
        End If
        Return True
    End Function
    '--------------
    ' Add to basket
    '--------------
    Protected Sub DoItemCommand_dlsProductListGraphical(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs)
        Dim quant As Integer = ModuleDefaults.Default_Add_Quantity
        '    Dim customValidationSummary As New Talent.Commerce.CustomValidationSummary
        '   Dim pageref As System.Web.UI.Page = Me.Page
        Try
            quant = Integer.Parse(CType(e.Item.FindControl("txtQuantity"), TextBox).Text)
        Catch
        End Try

        Dim strProduct As String = e.CommandArgument.ToString

        '-------------------------
        ' Check if enough in stock
        '-------------------------
        Dim availQty As Integer = Stock.GetStockBalance(strProduct)
        If quant > availQty And ModuleDefaults.FrontEndStockRequiredToAddToBasket Then
            ErrorMessage = ucr.Content("stockError", _languageCode, True)
        Else
            If quant >= ModuleDefaults.Min_Add_Quantity Then

                If Talent.eCommerce.Utilities.IsPartnerHomeDeliveryType(Profile) AndAlso isQuickOrderInUse() Then
                    Dim dtMasterProduct As DataTable
                    Dim sbQry As New StringBuilder
                    dtMasterProduct = TDataObjects.ProductsSettings.TblProduct.GetProductByProductCode(e.CommandArgument.ToString, True, False)
                    sbQry.Append("../../PagesLogin/QuickOrder/QuickOrder.aspx?product=" & dtMasterProduct.Rows(0)("PRODUCT_ID").ToString() & "&quant=" & quant).ToString()
                    Response.Redirect(sbQry.ToString())
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
                                .Xml_Config = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PERSONALISABLE"))
                            End If
                        End If
                        Try
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
                        Catch ex As Exception
                        End Try
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

                    If _ucr Is Nothing Then
                        With _ucr
                            .BusinessUnit = TalentCache.GetBusinessUnit()
                            .PageCode = Usage()
                            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
                            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                            .KeyCode = "productListGraphical.ascx"
                        End With
                    End If
                    If CType(_ucr.Attribute("forwardToBasket"), Boolean) Then
                        Response.Redirect("../../pagesPublic/basket/Basket.aspx")
                    Else
                        Response.Redirect(Request.Url.ToString)
                    End If
                End If
            Else
                ErrorMessage = String.Format(ucr.Content("MinQuantityNotMetError", _languageCode, True), ModuleDefaults.Min_Add_Quantity.ToString)
            End If
        End If

    End Sub

    Protected Sub dlsProductListGraphical_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptProductListGraphical.ItemCommand
        DoItemCommand_dlsProductListGraphical(source, e)
    End Sub

    Protected Sub GetRegExErrorText(ByVal sender As Object, ByVal e As EventArgs)
        Dim regex As RegularExpressionValidator = (CType(sender, RegularExpressionValidator))
        Select Case regex.ID
            Case Is = "QuantityValidator"
                regex.ErrorMessage = _ucr.Content("QuantityValidatorErrorText", _languageCode, True)
        End Select
    End Sub

    Protected Sub dlsProductListGraphical_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProductListGraphical.ItemDataBound
        DoItemDataBound_dlsProductListGraphical(sender, e)
    End Sub

    Protected Sub DoItemDataBound_dlsProductListGraphical(ByVal sender As Object, ByVal e As RepeaterItemEventArgs)
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim p As New Talent.eCommerce.Product
            Try
                p = CType(e.Item.DataItem, Talent.eCommerce.Product)
                If Talent.eCommerce.Stock.GetStockBalance(p.Code) < 1 And ModuleDefaults.FrontEndStockRequiredToAddToBasket Then
                    CType(e.Item.FindControl("plhNoStock"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("plhAction"), PlaceHolder).Visible = False
                    CType(e.Item.FindControl("NoStockLabel"), Literal).Text = _ucr.Content("ProductOutOfStockText", _languageCode, True)
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
                    CType(e.Item.FindControl("lblPriceText"), Literal).Text = _ucr.Content("FromPriceLabel", _languageCode, True)
                    CType(e.Item.FindControl("lblPrice"), Literal).Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice_From, _ucr.BusinessUnit, _ucr.PartnerCode)
                Else
                    CType(e.Item.FindControl("lblPriceText"), Literal).Text = _ucr.Content("PriceLabel", _languageCode, True)
                    CType(e.Item.FindControl("lblPrice"), Literal).Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice, _ucr.BusinessUnit, _ucr.PartnerCode)
                End If

                If Talent.eCommerce.Utilities.IsPartnerHomeDeliveryType(Profile) AndAlso isQuickOrderInUse() Then
                    CType(e.Item.FindControl("btnBuy"), Button).Text = ucr.Content("AddToBasketButtonTextForHomeDelivery", _languageCode, True)
                Else
                    CType(e.Item.FindControl("btnBuy"), Button).Text = ucr.Content("BuyButtonText", _languageCode, True)
                End If
                Try
                    CType(e.Item.FindControl("txtQuantity"), TextBox).Columns = CInt(_ucr.Attribute("QuantityTextBoxSize"))
                Catch
                End Try
                Try
                    CType(e.Item.FindControl("txtQuantity"), TextBox).Text = CInt(ModuleDefaults.Default_Add_Quantity)
                Catch
                End Try

                If p.WebPrices.PRICE_BREAK_QUANTITY_1 >= p.WebPrices.PRICE_BREAK_QUANTITY_2 Then
                    CType(e.Item.FindControl("lblPriceText"), Literal).Text = _ucr.Content("PriceLabel", _languageCode, True)
                End If
            Catch ex As Exception

            End Try
        End If
    End Sub

    Protected Sub orderddl_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles orderddl.SelectedIndexChanged
        LoadProducts()
    End Sub

    Protected Sub ShowAllButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ShowAllButton.Click
        'We let the parent class handle the redirect
        handleShowAllRedirect()
    End Sub

    Protected Sub dlsProductListGraphical2_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptProductListGraphical2.ItemCommand
        DoItemCommand_dlsProductListGraphical(source, e)
    End Sub

    Protected Sub dlsProductListGraphical2_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProductListGraphical2.ItemDataBound
        DoItemDataBound_dlsProductListGraphical(sender, e)
    End Sub

    Protected Function GetText(ByVal ucrKey As String) As String
        Return _ucr.Content(ucrKey, _languageCode, True)
    End Function

    Public Function FormatUrl(ByVal navigateUrl As String) As String
        Dim url As String = String.Empty
        If navigateUrl IsNot Nothing AndAlso navigateUrl.Length > 0 Then
            url = Replace(navigateUrl, " ", "%20")
        End If
        Return url
    End Function

    Private Function IsOrderByChanged() As Boolean
        Dim isChanged As Boolean = False
        If Session("OrderDLLPreviousSelectedValue") IsNot Nothing Then
            If orderddl.SelectedValue <> Session("OrderDLLPreviousSelectedValue") OrElse (Not _IsPaging) Then
                Session("OrderDLLPreviousSelectedValue") = orderddl.SelectedValue
                isChanged = True
            End If
        Else
            Session("OrderDLLPreviousSelectedValue") = orderddl.SelectedValue
            isChanged = True
        End If
        'Set the isChanged in session as it is used in another usercontrol - PageLeftProductNav.ascx
        Session("OrderDDLChanged") = isChanged
        Return isChanged
    End Function
	
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
End Class
