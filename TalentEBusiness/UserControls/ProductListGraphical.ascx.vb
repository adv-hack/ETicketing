Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Xml
Imports System.Globalization
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Product List Graphical
'
'       Date                        07/03/07
'
'       Author                      Ben Ford
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
Partial Class UserControls_ProductListGraphical
    Inherits AbstractProductList
    Private emptyString As String = String.Empty

    Private conTalent As SqlConnection = Nothing
    Private cmdSelect As SqlCommand = Nothing
    Private dtrProduct As SqlDataReader = Nothing

    Private requestGroups() As String = {"GROUP_L01_GROUP = @GROUP1", "GROUP_L02_GROUP = @GROUP2",
                                         "GROUP_L03_GROUP = @GROUP3", "GROUP_L04_GROUP = @GROUP4",
                                         "GROUP_L05_GROUP = @GROUP5", "GROUP_L06_GROUP = @GROUP6",
                                         "GROUP_L07_GROUP = @GROUP7", "GROUP_L08_GROUP = @GROUP8",
                                         "GROUP_L09_GROUP = @GROUP9", "GROUP_L10_GROUP = @GROUP10"}

    ' Dim completeProductsList As ProductList
    Dim completeProductsList As ProductListGen

    Shadows ucr As New Talent.Common.UserControlResource

    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected MoreInfoButtonText As String = ucr.Content("MoreInfoButtonText", _languageCode, True)


#Region "Public variables"
    Protected Property BlockGridStyleClass() As String = String.Empty

    Public ShowPrices As Boolean = True
#End Region

    Public Overloads ReadOnly Property HasRows() As Boolean
        Get
            Return _hasRows
        End Get
    End Property

    Const priceAsc As String = "priceasc",
         priceDesc As String = "pricedesc",
         nameAsc As String = "nameasc",
         nameDesc As String = "namedesc",
         bestSeller As String = "bestseller"

    'Page load can not use Handles Me.Load on override for performance reasons (page_load fires twice with Handles Me.Load)
    Protected Overrides Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)
        MyBase.Page_Load(sender, e)
        If (Profile.PartnerInfo.Details IsNot Nothing) Then
            ShowPrices = Not Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(Profile.PartnerInfo.Details.HIDE_PRICES)
        End If
        If Display Then

            If IsValidID(Request("page")) Then
                _PageNumber = Int32.Parse(Request("page"))
                _IsPaging = True
            End If


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
                If Not String.IsNullOrEmpty(Request.QueryString("order")) Then
                    Try
                        orderddl.SelectedIndex = CInt(Request.QueryString("order"))
                    Catch ex As Exception
                    End Try
                End If
            End If
            setupPager()
            'dlsProductListGraphical.RepeatColumns = CInt(ucr.Attribute("RepeatColumns"))

            'If (Not Page.IsPostBack And Display) Then
            If Not Page.IsPostBack Then
                'If (Not Page.IsPostBack And Display) Or completeProductsList Is Nothing Then
                LoadProducts()
            End If

            pnlProductListGraphical.Visible = True
        Else
            pnlProductListGraphical.Visible = False
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
    ''' 
    Protected Sub setupPager()
        If ShowAllItems Then
            pagerDisplayString.Visible = False
            pagerDisplayString2.Visible = False
            pagerDisplayString3.Visible = False
            pagerDisplayString4.Visible = False
        End If
        If IsValidID(Request("page")) And Not ShowAllItems() Then
            _PageNumber = Int32.Parse(Request("page"))
            _IsPaging = True
        End If
        ShowAllButton.Text = ShowAllButtonText
    End Sub

    Protected Overrides Sub setupUCR()
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Usage()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "productListGraphical.ascx"
        End With
        BlockGridStyleClass = ucr.Attribute("BlockGridStyleClass")
    End Sub

    Protected Sub LoadProducts()

        Dim pageProductsList As IList
        Dim filePath As String = String.Empty
        Dim err As New ErrorObj

        _caching = ucr.Attribute("Cacheing")
        _cacheTimeMinutes = ucr.Attribute("CacheTimeMinutes")
        completeProductsList = RetrieveProducts()
        completeProductsList.PageSize = CType(ucr.Attribute("PageSize"), Integer)
        Try
            If IsOrderByChanged() Then
                Select Case orderddl.SelectedValue
                    Case Is = bestSeller
                        _caching = False
                        completeProductsList = RetrieveProducts()
                        completeProductsList.PageSize = CType(ucr.Attribute("PageSize"), Integer)
                    Case Is = priceAsc
                        completeProductsList.SortProductsByPrice("A")
                    Case Is = priceDesc
                        completeProductsList.SortProductsByPrice("D")
                    Case Is = nameAsc
                        completeProductsList.SortProductsByName("A")
                    Case Is = nameDesc
                        completeProductsList.SortProductsByName("D")
                End Select
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

        If Not completeProductsList Is Nothing Then

            If ShowAllItems() Then
                pageProductsList = completeProductsList.Products
            Else
                pageProductsList = completeProductsList.GetPageProducts(_PageNumber)
            End If
            If completeProductsList.Products.Count <= completeProductsList.PageSize Then
                ShowAllButton.Visible = False
            End If
            rptProductListGraphical.DataSource = pageProductsList
            ' If Not Page.IsPostBack Then
            rptProductListGraphical.DataBind()
            ' End If
        End If

    End Sub

    Public Function NowShowingResultsString(ByVal type As String) As String
        Dim str As String = ""
        If Display Then
            Dim startAmount As Integer = 0
            Dim endAmount As Integer = 0

            Dim displaying1 As String = ucr.Content("displayingText1", _languageCode, True)
            Dim displaying2 As String = ucr.Content("displayingText2", _languageCode, True)
            Dim displaying3 As String = ucr.Content("displayingText3", _languageCode, True)
            Dim noProductsFound As String = ucr.Content("NoProductsFoundText", _languageCode, True)

            If Not completeProductsList Is Nothing AndAlso completeProductsList.Count > 0 Then
                startAmount = ((_PageNumber - 1) * completeProductsList.PageSize) + 1
                If completeProductsList.Count < (_PageNumber * completeProductsList.PageSize) Then
                    endAmount = completeProductsList.Count
                Else
                    endAmount = (_PageNumber * completeProductsList.PageSize)
                End If
                str = displaying1 & " " & startAmount & " " & displaying2 & " " & endAmount & " " & displaying3 & " " & completeProductsList.Count
            Else
                If type = "TOP" Then
                    str = noProductsFound
                Else
                    str = String.Empty
                End If
            End If

        End If

        Return str

    End Function

    Public Function PagingString(ByVal type As String) As String
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
            If Not completeProductsList Is Nothing Then
                '----------------------------------------------------------------
                'If there is only one page of data, don't display any paging info
                '----------------------------------------------------------------
                If completeProductsList.Count <= completeProductsList.PageSize Then
                    Return ""
                End If
                '--------------------------------------------------
                'Is this a valid page for the amount of order lines
                '--------------------------------------------------
                If _PageNumber > completeProductsList.NumberOfPages Then
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
                        str = str & "<span><a href=""" & linkPage & "&page=1" & "&order=" & orderddl.SelectedIndex & """>" & firstText & "</a></span></span> "
                    End If
                    If nextPreviousAsImages Then
                        str = str & "<span><a href=""" & linkPage & "&page=" & (_PageNumber - 1) & "&order=" & orderddl.SelectedIndex & """>" & "<img src=""" & previousImage & """>" & "</a></span></span> "
                    Else
                        str = str & "<span><a href=""" & linkPage & "&page=" & (_PageNumber - 1) & "&order=" & orderddl.SelectedIndex & """>" & previousText & "</a></span></span> "
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
                If completeProductsList.NumberOfPages <= numberOfLinks Then
                    '----------------------------------------------------------
                    'List out the pages, current page does not have a hyperlink
                    '----------------------------------------------------------
                    For counter = 1 To completeProductsList.NumberOfPages
                        If counter = _PageNumber Then
                            If counter = completeProductsList.NumberOfPages Or Not showSeperator Then
                                str = str & " <span>" & counter & "</span> "
                            Else
                                str = str & " <span>" & counter & "</span> " & pageSeperator & " "
                            End If
                        Else
                            If counter = completeProductsList.NumberOfPages Or Not showSeperator Then
                                str = str & " <span><a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a></span> "
                            Else
                                str = str & " <span><a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a></span> " & " " & pageSeperator
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
                                    str = str & " <span>" & counter & "</span> "
                                Else
                                    str = str & " <span>" & counter & " " & pageSeperator & "</span> "
                                End If
                            Else
                                If counter = numberOfLinks Or Not showSeperator Then
                                    str = str & " <span><a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a></span> "
                                Else
                                    str = str & " <span><a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a></span> " & " " & pageSeperator
                                End If
                            End If
                        Next
                    ElseIf _PageNumber > (completeProductsList.NumberOfPages - numberOfLinks) Then
                        '--------------------------------------------------
                        'display the last 'number_of_links number' of links
                        '--------------------------------------------------
                        For counter = (completeProductsList.NumberOfPages - numberOfLinks) To completeProductsList.NumberOfPages
                            If counter = _PageNumber Then
                                If counter = completeProductsList.NumberOfPages Or Not showSeperator Then
                                    str = str & " <span>" & counter & "</span> "
                                Else
                                    str = str & " <span>" & counter & "</span> " & pageSeperator & " "
                                End If
                            Else
                                If counter = completeProductsList.NumberOfPages Or Not showSeperator Then
                                    str = str & " <span><a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a></span> "
                                Else
                                    str = str & " <span><a href=""" & linkPage & "&page=" & counter & "&order=" & orderddl.SelectedIndex & """>" & counter & "</a></span> " & " " & pageSeperator
                                End If
                            End If
                        Next
                    Else
                        Dim halfWay As Integer = numberOfLinks / 2
                        For counter = 1 To halfWay - 1
                            str = str & " <span><a href=""" & linkPage & "&page=" & (_PageNumber - halfWay + counter) & "&order=" & orderddl.SelectedIndex & """>" & (_PageNumber - halfWay + counter) & "</a></span> "
                        Next
                        str = str & " <span>" & _PageNumber & "</span> "
                        For counter = 1 To halfWay
                            If counter = halfWay Or Not showSeperator Then
                                str = str & " <span><a href=""" & linkPage & "&page=" & (_PageNumber + counter) & "&order=" & orderddl.SelectedIndex & """>" & (_PageNumber + counter) & "</a></span> "
                            Else
                                str = str & " <span><a href=""" & linkPage & "&page=" & (_PageNumber + counter) & "&order=" & orderddl.SelectedIndex & """>" & (_PageNumber + counter) & "</a></span> " & " " & pageSeperator
                            End If
                        Next
                    End If
                End If
                '-------------------------------------------
                'Display the 'Next' link and the 'Last' link
                '-------------------------------------------
                If _PageNumber < completeProductsList.NumberOfPages Then
                    If nextPreviousAsImages Then
                        str = str & "<span><a href=""" & linkPage & "&page=" & (_PageNumber + 1) & "&order=" & orderddl.SelectedIndex & """>" & "<img src=""" & nextImage & """>" & "</a></span> "
                    Else
                        str = str & "<span><a href=""" & linkPage & "&page=" & (_PageNumber + 1) & "&order=" & orderddl.SelectedIndex & """>" & nextText & "</a></span> "
                    End If
                    If showFirstLast Then
                        str = str & "<span><a href=""" & linkPage & "&page=" & completeProductsList.NumberOfPages & "&order=" & orderddl.SelectedIndex & """>" & lastText & "</a></span>"
                    End If
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
    Protected Sub dlsProductListGraphical_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptProductListGraphical.ItemCommand
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
            If quant < 0 Then
                lblError.Text = ucr.Content("QuantityLessThanZeroError", _languageCode, True)
            ElseIf quant >= ModuleDefaults.Min_Add_Quantity Then

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
                End If
            Else
                lblError.Text = String.Format(ucr.Content("MinQuantityNotMetError", _languageCode, True), ModuleDefaults.Min_Add_Quantity.ToString)
            End If
        End If

    End Sub


    'Protected Sub rptProducts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProducts.ItemDataBound
    '    'Dim talProduct As Talent.eCommerce.Product
    '    'Dim lblPrice As Label
    '    Dim btnBuy As Button
    '    '-------------------
    '    ' Set column Headers
    '    '-------------------
    '    If e.Item.ItemType = ListItemType.Header Then
    '        CType(e.Item.FindControl("colHeader1"), Label).Text = ucr.Content("colHeaderName", LC.ID, True)
    '        CType(e.Item.FindControl("colHeader2"), Label).Text = ucr.Content("colHeaderColour", LC.ID, True)
    '        CType(e.Item.FindControl("colHeader3"), Label).Text = ucr.Content("colHeaderCountry", LC.ID, True)
    '        CType(e.Item.FindControl("colHeader4"), Label).Text = ucr.Content("colHeaderPrice", LC.ID, True)
    '    End If
    '    '---------------------------
    '    ' Calculate and format price
    '    '---------------------------
    '    If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
    '        ' Price now calculated when product is loaded
    '        '  talProduct = CType(e.Item.DataItem, Talent.eCommerce.Product)
    '        '  lblPrice = CType(e.Item.FindControl("lblPrice"), Label)
    '        '  lblPrice.Text = ProductPrice.Get_Price(talProduct.Code, def.PriceList).GrossPrice.ToString("c")
    '        '--------------------
    '        ' Set buy button text
    '        '--------------------
    '        btnBuy = CType(e.Item.FindControl("btn_buy"), Button)
    '        btnBuy.Text = ucr.Content("BuyButtonText", LC.ID, True)

    '    End If

    'End Sub
    Protected Sub GetRegExErrorText(ByVal sender As Object, ByVal e As EventArgs)
        If Display Then
            Dim regex As RegularExpressionValidator = (CType(sender, RegularExpressionValidator))
            Select Case regex.ID
                Case Is = "QuantityValidator"
                    regex.ErrorMessage = ucr.Content("QuantityValidatorErrorText", _languageCode, True)
            End Select
        End If

    End Sub

    Protected Sub dlsProductListGraphical_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProductListGraphical.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim p As New Product
            p = CType(e.Item.DataItem, Talent.eCommerce.Product)
            Try
                If Talent.eCommerce.Stock.GetStockBalance(p.Code) < 1 And ModuleDefaults.FrontEndStockRequiredToAddToBasket Then
                    CType(e.Item.FindControl("plhNoStock"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("plhAction"), PlaceHolder).Visible = False
                    Dim stockTime As String = String.Empty
                    Dim reStockCode As String = String.Empty
                    CType(e.Item.FindControl("NoStockLabel"), Literal).Text = Talent.eCommerce.Stock.GetNoStockDescription(p.Code, stockTime, reStockCode)
                End If
            Catch ex As Exception

            End Try

            '---------------------------
            ' Set attributes in Datalist
            '---------------------------

            CType(e.Item.FindControl("lblPriceText"), Literal).Text = ucr.Content("PriceLabel", _languageCode, True)
            If Talent.eCommerce.Utilities.IsPartnerHomeDeliveryType(Profile) AndAlso isQuickOrderInUse() Then
                CType(e.Item.FindControl("btnBuy"), Button).Text = ucr.Content("AddToBasketButtonTextForHomeDelivery", _languageCode, True)				
            Else
                CType(e.Item.FindControl("btnBuy"), Button).Text = ucr.Content("BuyButtonText", _languageCode, True)
            End If
            'The following few lines check if MoreInfo button is visible, and if it is then set the text
            Dim MoreInfoButtonVisible As Boolean = Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(ucr.Attribute("DisplayMoreInfoButton_ProductListGraphical"))
            If (MoreInfoButtonVisible) Then
                Dim moreInfoLink As HyperLink = CType(e.Item.FindControl("MoreInfoButton"), HyperLink)
                moreInfoLink.Visible = MoreInfoButtonVisible
                moreInfoLink.NavigateUrl = FormatUrl(CType(e.Item.DataItem, Talent.eCommerce.Product).NavigateURL)
                moreInfoLink.Enabled = CType(e.Item.DataItem, Talent.eCommerce.Product).LinkEnabled
                moreInfoLink.ToolTip = CType(e.Item.DataItem, Talent.eCommerce.Product).Description1.Trim
                moreInfoLink.Text = ucr.Content("MoreInfoButtonText", _languageCode, True)
            End If

            Try
                CType(e.Item.FindControl("txtQuantity"), TextBox).Columns = CInt(ucr.Attribute("QuantityTextBoxSize"))
            Catch
            End Try
            Try
                CType(e.Item.FindControl("txtQuantity"), TextBox).Text = CInt(ModuleDefaults.Default_Add_Quantity)
            Catch
            End Try

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
                        CType(e.Item.FindControl("lblPriceText"), Literal).Text = ucr.Content("PriceLabel", _languageCode, True)
                        CType(e.Item.FindControl("lblGrossPrice"), Literal).Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice, ucr.BusinessUnit, ucr.PartnerCode)
                    Else
                        CType(e.Item.FindControl("lblPriceText"), Literal).Text = ucr.Content("FromPriceLabel", _languageCode, True)
                        CType(e.Item.FindControl("lblGrossPrice"), Literal).Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice_From, ucr.BusinessUnit, ucr.PartnerCode)
                    End If
                    CType(e.Item.FindControl("lblGrossPrice"), Literal).Visible = True
                Else
                    CType(e.Item.FindControl("lblPriceText"), Literal).Text = ucr.Content("PriceLabel", _languageCode, True)
                    CType(e.Item.FindControl("lblGrossPrice"), Literal).Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice, ucr.BusinessUnit, ucr.PartnerCode)
                    CType(e.Item.FindControl("lblGrossPrice"), Literal).Visible = True
                End If

                If p.WebPrices.IsSalePrice Then
                    CType(e.Item.FindControl("lblPriceText"), Literal).Text = ucr.Content("SalePriceLabel", _languageCode, True)
                End If

                If p.WebPrices.PRICE_BREAK_QUANTITY_1 > p.WebPrices.PRICE_BREAK_QUANTITY_2 Then
                    CType(e.Item.FindControl("lblPriceText"), Literal).Visible = False
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
