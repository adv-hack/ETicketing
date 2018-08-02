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
Partial Class UserControls_ProductSearchResultsList2
    Inherits AbstractProductSearchList
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

            dlsProductListGraphical.RepeatColumns = CInt(ucr.Attribute("RepeatColumns"))

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
                If Not String.IsNullOrEmpty(Request.QueryString("order")) Then
                    Try
                        orderddl.SelectedIndex = CInt(Request.QueryString("order"))
                    Catch ex As Exception
                    End Try
                End If
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
        If Not completeSearchProductsList Is Nothing Then
            If completeSearchProductsList.Products.Count <= completeSearchProductsList.PageSize Then
                ShowAllButton.Visible = False
            End If
        End If
    End Sub

    Public Overloads Function PagingString(ByVal type As String) As String
        Dim str As String = ""
        If Display Then
            Dim linkPage As String = _currentPage & "?" & Request.QueryString.ToString
            Dim firstText As String = ucr.Content("firstText", _languageCode, True)
            Dim previousText As String = ucr.Content("previousText", _languageCode, True)
            Dim nextText As String = ucr.Content("nextText", _languageCode, True)
            Dim lastText As String = ucr.Content("lastText", _languageCode, True)
            Dim pageSeperator As String = ucr.Attribute("pageSeperator")
            Dim numberOfLinks As Integer = CType(ucr.Attribute("numberOfLinks"), Integer)
            Dim showFirstLast As Boolean = CType(ucr.Attribute("showFirstLast"), Boolean)
            Dim showSeperator As Boolean = CType(ucr.Attribute("showSeperator"), Boolean)
            Dim nextPreviousAsImages As Boolean = CType(ucr.Attribute("nextPreviousAsImages"), Boolean)
            Dim previousImage As String = ucr.Attribute("previousImage")
            Dim nextImage As String = ucr.Attribute("nextImage")

            If linkPage.Contains("&page=") Then
                linkPage = linkPage.Substring(0, linkPage.IndexOf("&page"))
            End If
            If Not completeSearchProductsList Is Nothing Then
                '----------------------------------------------------------------
                'If there is only one page of data, don't display any paging info
                '----------------------------------------------------------------
                If completeSearchProductsList.Count <= completeSearchProductsList.PageSize Then
                    Return ""
                End If
                '--------------------------------------------------
                'Is this a valid page for the amount of order lines
                '--------------------------------------------------
                If _PageNumber > completeSearchProductsList.NumberOfPages Then
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
                        str = str & "<a href=""" & linkPage & "&page=1" & "&order=" & orderddl.SelectedIndex & """>" & firstText & "</a> "
                    End If
                    If nextPreviousAsImages Then
                        str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber - 1) & "&order=" & orderddl.SelectedIndex & """>" & "<img src=""" & previousImage & """>" & "</a> "
                    Else
                        str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber - 1) & "&order=" & orderddl.SelectedIndex & """>" & previousText & "</a> "
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
                If completeSearchProductsList.NumberOfPages <= numberOfLinks Then
                    '----------------------------------------------------------
                    'List out the pages, current page does not have a hyperlink
                    '----------------------------------------------------------
                    For counter = 1 To completeSearchProductsList.NumberOfPages
                        If counter = _PageNumber Then
                            If counter = completeSearchProductsList.NumberOfPages Or Not showSeperator Then
                                str = str & " " & counter & " "
                            Else
                                str = str & " " & counter & " " & pageSeperator & " "
                            End If
                        Else
                            If counter = completeSearchProductsList.NumberOfPages Or Not showSeperator Then
                                str = str & " <a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a> "
                            Else
                                str = str & " <a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a> " & " " & pageSeperator
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
                                    str = str & " <a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a> "
                                Else
                                    str = str & " <a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a> " & " " & pageSeperator
                                End If
                            End If
                        Next
                    ElseIf _PageNumber > (completeSearchProductsList.NumberOfPages - numberOfLinks) Then
                        '--------------------------------------------------
                        'display the last 'number_of_links number' of links
                        '--------------------------------------------------
                        For counter = (completeSearchProductsList.NumberOfPages - numberOfLinks) To completeSearchProductsList.NumberOfPages
                            If counter = _PageNumber Then
                                If counter = completeSearchProductsList.NumberOfPages Or Not showSeperator Then
                                    str = str & " " & counter & " "
                                Else
                                    str = str & " " & counter & " " & pageSeperator & " "
                                End If
                            Else
                                If counter = completeSearchProductsList.NumberOfPages Or Not showSeperator Then
                                    str = str & " <a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a> "
                                Else
                                    str = str & " <a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a> " & " " & pageSeperator
                                End If
                            End If
                        Next
                    Else
                        Dim halfWay As Integer = numberOfLinks / 2
                        For counter = 1 To halfWay - 1
                            str = str & " <a href=""" & linkPage & "&page=" & (_PageNumber - halfWay + counter) & "&order=" & orderddl.SelectedIndex & """>" & (_PageNumber - halfWay + counter) & "</a> "
                        Next
                        str = str & " " & _PageNumber & " "
                        For counter = 1 To halfWay
                            If counter = halfWay Or Not showSeperator Then
                                str = str & " <a href=""" & linkPage & "&page=" & (_PageNumber + counter) & "&order=" & orderddl.SelectedIndex & """>" & (_PageNumber + counter) & "</a> "
                            Else
                                str = str & " <a href=""" & linkPage & "&page=" & (_PageNumber + counter) & "&order=" & orderddl.SelectedIndex & """>" & (_PageNumber + counter) & "</a> " & " " & pageSeperator
                            End If
                        Next
                    End If
                End If
                '-------------------------------------------
                'Display the 'Next' link and the 'Last' link
                '-------------------------------------------
                If _PageNumber < completeSearchProductsList.NumberOfPages Then
                    If nextPreviousAsImages Then
                        str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber + 1) & "&order=" & orderddl.SelectedIndex & """>" & "<img src=""" & nextImage & """>" & "</a> "
                    Else
                        str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber + 1) & "&order=" & orderddl.SelectedIndex & """>" & nextText & "</a> "
                    End If
                    If showFirstLast Then
                        str = str & "<a href=""" & linkPage & "&page=" & completeSearchProductsList.NumberOfPages & "&order=" & orderddl.SelectedIndex & """>" & lastText & "</a>"
                    End If
                End If

            End If
        End If

        Return str

    End Function

    Protected Overrides Sub displayProductsList(ByVal pageProductsList As IList)
        Select Case ModuleDefaults.Product_List_Graphical_Template_Type
            Case 3
                template1.Visible = False
                template3.Visible = True
                dlsProductListGraphical2.DataSource = pageProductsList
                dlsProductListGraphical2.DataBind()
            Case Else
                template1.Visible = True
                template3.Visible = False
                dlsProductListGraphical.DataSource = pageProductsList
                dlsProductListGraphical.DataBind()
        End Select
        graphicalView.Visible = True        
    End Sub

    Protected Overrides Sub setSearchResultsLabelText(ByVal text As String)
        lblSearchResults.Text = text
    End Sub

    Protected Overrides Function getOrderDropDownListSelectedValue() As String
        Return orderddl.SelectedValue
    End Function

    Protected Sub dlsProductListGraphical_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles dlsProductListGraphical.ItemCommand
        DoItemCommand_dlsProductListGraphical(source, e)
    End Sub

    Protected Sub DoItemCommand_dlsProductListGraphical(ByVal sender As Object, ByVal e As DataListCommandEventArgs)
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
            lblError.Text = ucr.Content("stockError", _languageCode, True)
        Else
            If quant >= ModuleDefaults.Min_Add_Quantity Then
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
                        .KeyCode = "productListGraphical.ascx"
                    End With
                End If
                If CType(ucr.Attribute("forwardToBasket"), Boolean) Then
                    Response.Redirect("../../pagesPublic/basket/Basket.aspx")
                Else
                    Response.Redirect(Request.Url.ToString)
                End If
            Else
                lblError.Text = String.Format(ucr.Content("MinQuantityNotMetError", _languageCode, True), ModuleDefaults.Min_Add_Quantity.ToString)
            End If
        End If
    End Sub

    Protected Sub DoItemDataBound_dlsProductListGraphical(ByVal sender As Object, ByVal e As DataListItemEventArgs)
        Dim p As New Talent.eCommerce.Product
        Try
            p = CType(e.Item.DataItem, Talent.eCommerce.Product)
            If Talent.eCommerce.Stock.GetStockBalance(p.Code) < 1 And ModuleDefaults.FrontEndStockRequiredToAddToBasket Then
                CType(e.Item.FindControl("btnBuy"), Button).Visible = False
                CType(e.Item.FindControl("txtQuantity"), TextBox).Visible = False
                CType(e.Item.FindControl("NoStockLabel"), Label).Visible = True
                Dim stockTime As String = String.Empty
                Dim reStockCode As String = String.Empty
                CType(e.Item.FindControl("NoStockLabel"), Literal).Text = Talent.eCommerce.Stock.GetNoStockDescription(p.Code, stockTime, reStockCode)
            End If
        Catch ex As Exception

        End Try



        '---------------------------
        ' Set attributes in Datalist
        '---------------------------
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
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
                    If p.WebPrices.PRICE_BREAK_QUANTITY_1 >= p.WebPrices.PRICE_BREAK_QUANTITY_2 Then
                        CType(e.Item.FindControl("lblPriceText"), Label).Text = ucr.Content("PriceLabel", _languageCode, True)
                        CType(e.Item.FindControl("lblPrice"), Label).Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice, ucr.BusinessUnit, ucr.PartnerCode)
                    Else
                        CType(e.Item.FindControl("lblPriceText"), Label).Text = ucr.Content("FromPriceLabel", _languageCode, True)
                        CType(e.Item.FindControl("lblPrice"), Label).Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice_From, ucr.BusinessUnit, ucr.PartnerCode)
                    End If
                Else
                    CType(e.Item.FindControl("lblPriceText"), Label).Text = ucr.Content("PriceLabel", _languageCode, True)
                    CType(e.Item.FindControl("lblPrice"), Label).Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice, ucr.BusinessUnit, ucr.PartnerCode)
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

            Catch ex As Exception

            End Try


        End If
    End Sub

    Protected Sub dlsProductListGraphical_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles dlsProductListGraphical.ItemDataBound
        DoItemDataBound_dlsProductListGraphical(sender, e)
    End Sub

    Protected Sub GetRegExErrorText(ByVal sender As Object, ByVal e As EventArgs)
        If Display Then
            Dim regex As RegularExpressionValidator = (CType(sender, RegularExpressionValidator))
            Select Case regex.ID
                Case Is = "QuantityValidator"
                    Dim errStr As String = ""
                    Try
                        errStr = ucr.Content("QuantityValidatorErrorText", _languageCode, True)
                    Catch ex As Exception
                    End Try
                    regex.ErrorMessage = errStr
            End Select
        End If

    End Sub

    Protected Sub orderddl_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles orderddl.SelectedIndexChanged
        If Display Then LoadProducts()
    End Sub

    Protected Sub ShowAllButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ShowAllButton.Click
        'We let the parent class handle the redirect
        handleShowAllRedirect()
    End Sub

    Protected Function GetText(ByVal ucrKey As String) As String
        Return ucr.Content(ucrKey, _languageCode, True)
    End Function

    Protected Sub dlsProductListGraphical2_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles dlsProductListGraphical2.ItemCommand
        DoItemCommand_dlsProductListGraphical(source, e)
    End Sub

    Protected Sub dlsProductListGraphical2_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles dlsProductListGraphical2.ItemDataBound
        DoItemDataBound_dlsProductListGraphical(sender, e)
    End Sub
End Class
