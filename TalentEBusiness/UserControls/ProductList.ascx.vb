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
'       Function                    User Controls - Product List
'
'       Date                        Feb 2007
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
Partial Class UserControls_ProductList
    Inherits AbstractProductList

    Private emptyString As String = String.Empty

    Private conTalent As SqlConnection = Nothing
    Private cmdSelect As SqlCommand = Nothing
    Private dtrProduct As SqlDataReader = Nothing    

    Private requestGroups() As String = {"GROUP_L01_GROUP = @GROUP1", "GROUP_L02_GROUP = @GROUP2", _
                                         "GROUP_L03_GROUP = @GROUP3", "GROUP_L04_GROUP = @GROUP4", _
                                         "GROUP_L05_GROUP = @GROUP5", "GROUP_L06_GROUP = @GROUP6", _
                                         "GROUP_L07_GROUP = @GROUP7", "GROUP_L08_GROUP = @GROUP8", _
                                         "GROUP_L09_GROUP = @GROUP9", "GROUP_L10_GROUP = @GROUP10"}

    Dim completeProductsList As ProductListGen
    
    Private Shadows ucr As New Talent.Common.UserControlResource

    Private languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    'Page load can not use Handles Me.Load on override for performance reasons (page_load fires twice with Handles Me.Load)
    Protected Overrides Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)
        MyBase.Page_Load(sender, e)
        setupPager()
        populateSortByDropDown()
        setupProductList()
        ' Hide show all button if no text is setup
        If ShowAllButton.Text = String.Empty Then
            ShowAllButton.Visible = False
        End If
    End Sub

    Private Sub setupProductList()
        If Display Then
            pnlProductList.Visible = True
        Else
            pnlProductList.Visible = False
        End If

        If Not Page.IsPostBack And Display Then
            LoadProducts()
        End If
    End Sub

    Private Sub populateSortByDropDown()
        If Not Page.IsPostBack Then
            '-----------------------------
            ' Populate 'Sort By' drop down
            '-----------------------------
            txtSortBy.Text = ucr.Content("SortByText", languageCode, True)
            ddlSortBy.Items.Clear()
            ddlSortBy.Items.Add(ucr.Content("SortByOpt1", languageCode, True))
            ddlSortBy.Items.Add(ucr.Content("SortByOpt2", languageCode, True))
            ddlSortBy.Items.Add(ucr.Content("SortByOpt3", languageCode, True))
            ddlSortBy.Items.Add(ucr.Content("SortByOpt4", languageCode, True))
            ddlSortBy.Items.Add(ucr.Content("SortByOpt5", languageCode, True))
            ddlSortBy.AutoPostBack = True
            If Not Session("OrderByPreviousSelectedIndex") Is Nothing Then
                ddlSortBy.SelectedIndex = CInt(Session("OrderByPreviousSelectedIndex"))
            End If
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
            .KeyCode = "productList.ascx"
        End With
    End Sub

    Protected Sub LoadProducts()

        Dim pageProductsList As IList
        Dim filePath As String = String.Empty
        Dim err As New ErrorObj
        _caching = ucr.Attribute("Cacheing")
        _cacheTimeMinutes = ucr.Attribute("CacheTimeMinutes")
        completeProductsList = RetrieveProducts()
        
        Try
            If IsOrderByChanged() Then
                Select Case ddlSortBy.SelectedIndex
                    Case Is = 0
                        completeProductsList.SortProductsByBestSeller("A")
                    Case Is = 1
                        completeProductsList.SortProductsByName("A")
                    Case Is = 2
                        completeProductsList.SortProductsByName("D")
                    Case Is = 3
                        completeProductsList.SortProductsByPrice("A")
                    Case Is = 4
                        completeProductsList.SortProductsByPrice("D")
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

        If Not completeProductsList Is Nothing Then

            If ShowAllItems() Then
                pageProductsList = completeProductsList.Products
            Else
                pageProductsList = completeProductsList.GetPageProducts(_PageNumber)
            End If
            If completeProductsList.Products.Count <= completeProductsList.PageSize Then
                ShowAllButton.Visible = False
            End If

            rptProducts.DataSource = pageProductsList
            rptProducts.DataBind()

        End If


    End Sub

    Public Function NowShowingResultsString(ByVal type As String) As String

        Dim startAmount As Integer = 0
        Dim endAmount As Integer = 0

        Dim displaying1 As String = ucr.Content("displayingText1", languageCode, True)
        Dim displaying2 As String = ucr.Content("displayingText2", languageCode, True)
        Dim displaying3 As String = ucr.Content("displayingText3", languageCode, True)
        Dim noProductsFound As String = ucr.Content("NoProductsFoundText", languageCode, True)
        Dim str As String = ""

        Dim a As String
        Dim b As Integer = 1
        a = b

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

        Return str

    End Function

    Public Function PagingString(ByVal type As String) As String

        Dim str As String = ""
        Dim linkPage As String = _currentPage & "?" & Request.QueryString.ToString
        Dim firstText As String = ucr.Content("firstText", languageCode, True)
        Dim previousText As String = ucr.Content("previousText", languageCode, True)
        Dim nextText As String = ucr.Content("nextText", languageCode, True)
        Dim lastText As String = ucr.Content("lastText", languageCode, True)
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
        '----------------------------------------------------------------
        'If there is only one page of data, don't display any paging info
        '----------------------------------------------------------------

        If Not completeProductsList Is Nothing Then

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
                    str = str & "<a href=""" & linkPage & "&page=1"">" & firstText & "</a> "
                End If
                If nextPreviousAsImages Then
                    str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber - 1) & """>" & "<img src=""" & previousImage & """>" & "</a> "
                Else
                    str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber - 1) & """>" & previousText & "</a> "
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
                            str = str & " " & counter & " "
                        Else
                            str = str & " " & counter & " " & pageSeperator & " "
                        End If
                    Else
                        If counter = completeProductsList.NumberOfPages Or Not showSeperator Then
                            str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> "
                        Else
                            str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> " & " " & pageSeperator
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
                                str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> "
                            Else
                                str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> " & " " & pageSeperator
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
                                str = str & " " & counter & " "
                            Else
                                str = str & " " & counter & " " & pageSeperator & " "
                            End If
                        Else
                            If counter = completeProductsList.NumberOfPages Or Not showSeperator Then
                                str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> "
                            Else
                                str = str & " <a href=""" & linkPage & "&page=" & counter & """>" & counter & "</a> " & " " & pageSeperator
                            End If
                        End If
                    Next
                Else
                    Dim halfWay As Integer = numberOfLinks / 2
                    For counter = 1 To halfWay - 1
                        str = str & " <a href=""" & linkPage & "&page=" & (_PageNumber - halfWay + counter) & """>" & (_PageNumber - halfWay + counter) & "</a> "
                    Next
                    str = str & " " & _PageNumber & " "
                    For counter = 1 To halfWay
                        If counter = halfWay Or Not showSeperator Then
                            str = str & " <a href=""" & linkPage & "&page=" & (_PageNumber + counter) & """>" & (_PageNumber + counter) & "</a> "
                        Else
                            str = str & " <a href=""" & linkPage & "&page=" & (_PageNumber + counter) & """>" & (_PageNumber + counter) & "</a> " & " " & pageSeperator
                        End If
                    Next
                End If
            End If
            '-------------------------------------------
            'Display the 'Next' link and the 'Last' link
            '-------------------------------------------
            If _PageNumber < completeProductsList.NumberOfPages Then
                If nextPreviousAsImages Then
                    str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber + 1) & """>" & "<img src=""" & nextImage & """>" & "</a> "
                Else
                    str = str & "<a href=""" & linkPage & "&page=" & (_PageNumber + 1) & """>" & nextText & "</a> "
                End If
                If showFirstLast Then
                    str = str & "<a href=""" & linkPage & "&page=" & completeProductsList.NumberOfPages & """>" & lastText & "</a>"
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
    Protected Sub rptProducts_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptProducts.ItemCommand
        lblError.Text = String.Empty
        Dim quant As Integer = ModuleDefaults.Default_Add_Quantity
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
            lblError.Text = ucr.Content("stockError", languageCode, True)
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
                        .GROUP_LEVEL_01 = al(0).ToString
                        .GROUP_LEVEL_02 = al(1).ToString
                        .GROUP_LEVEL_03 = al(2).ToString
                        .GROUP_LEVEL_04 = al(3).ToString
                        .GROUP_LEVEL_05 = al(4).ToString
                        .GROUP_LEVEL_06 = al(5).ToString
                        .GROUP_LEVEL_07 = al(6).ToString
                        .GROUP_LEVEL_08 = al(7).ToString
                        .GROUP_LEVEL_09 = al(8).ToString
                        .GROUP_LEVEL_10 = al(9).ToString
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
            Else
                lblError.Text = String.Format(ucr.Content("MinQuantityNotMetError", languageCode, True), ModuleDefaults.Min_Add_Quantity.ToString)
            End If
        End If

    End Sub


    Protected Sub rptProducts_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptProducts.ItemDataBound

        Dim lblPrice As Label = CType(e.Item.FindControl("lblPrice"), Label)
        Dim p As New Talent.eCommerce.Product
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then

            Try
                p = CType(e.Item.DataItem, Talent.eCommerce.Product)
                If Talent.eCommerce.Stock.GetStockBalance(p.Code) < 1 And ModuleDefaults.FrontEndStockRequiredToAddToBasket Then
                    CType(e.Item.FindControl("btn_buy"), Button).Visible = False
                    CType(e.Item.FindControl("NoStockLabel"), Label).Visible = True
                    CType(e.Item.FindControl("NoStockLabel"), Label).Text = ucr.Content("ProductOutOfStockText", languageCode, True)
                End If
            Catch ex As Exception

            End Try

        End If



        Dim btnBuy As Button
        '-------------------
        ' Set column Headers
        '-------------------
        If e.Item.ItemType = ListItemType.Header Then
            CType(e.Item.FindControl("colHeader1"), Label).Text = ucr.Content("colHeaderName", languageCode, True)
            CType(e.Item.FindControl("colHeader2"), Label).Text = ucr.Content("colHeaderColour", languageCode, True)
            CType(e.Item.FindControl("colHeader3"), Label).Text = ucr.Content("colHeaderCountry", languageCode, True)
            If Talent.eCommerce.Utilities.ShowPrices(Profile) Then
                CType(e.Item.FindControl("colHeader4"), Label).Text = ucr.Content("colHeaderPrice", languageCode, True)
            End If
        End If
        '---------------------------
        ' Calculate and format price
        '---------------------------
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then

            ' Hide if necessary 
            If Profile.PartnerInfo.Details.Enable_Price_View Then
                ' Price now calculated when product is loaded 
                If ModuleDefaults.ShowFromPrices Then
                    lblPrice.Text = ucr.Content("FromPriceText", languageCode, True) & " " & TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice_From, ucr.BusinessUnit, ucr.PartnerCode)
                Else
                    lblPrice.Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice, ucr.BusinessUnit, ucr.PartnerCode)
                End If

                '--------------------
                ' Set buy button text
                '--------------------
                btnBuy = CType(e.Item.FindControl("btn_buy"), Button)
                If Not ModuleDefaults.AllowMasterProductsToBePurchased Then
                    btnBuy.Visible = False
                Else
                    btnBuy.Text = ucr.Content("BuyButtonText", languageCode, True)
                End If
            Else
                lblPrice.Visible = False
            End If

        End If

    End Sub
    '-----------------------------------
    ' Reload if sort by order is changed
    '-----------------------------------
    Protected Sub ddlSortBy_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSortBy.SelectedIndexChanged
        LoadProducts()
    End Sub

    Protected Sub ShowAllButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ShowAllButton.Click
        'We let the parent class handle the redirect
        handleShowAllRedirect()        
    End Sub

    Private Function IsOrderByChanged() As Boolean
        Dim isChanged As Boolean = False
        If Session("OrderByPreviousSelectedIndex") IsNot Nothing Then
            If ddlSortBy.SelectedIndex <> Session("OrderByPreviousSelectedIndex") OrElse (Not _IsPaging) Then
                Session("OrderByPreviousSelectedIndex") = ddlSortBy.SelectedIndex
                isChanged = True
            End If
        Else
            Session("OrderByPreviousSelectedIndex") = ddlSortBy.SelectedIndex
            isChanged = True
        End If
        'Set the isChanged in session as it is used in another usercontrol - PageLeftProductNav.ascx
        Session("OrderDDLChanged") = isChanged
        Return isChanged
    End Function

End Class
