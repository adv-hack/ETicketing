Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.eCommerce
Imports Talent.Common
Imports System.Xml
Imports System.Globalization
Partial Class UserControls_ProductList2
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
    Dim _reSort As Boolean = False

    Private Shadows ucr As New Talent.Common.UserControlResource
    Private languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Protected Overrides Sub setupUCR()
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Usage()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "productList.ascx"
        End With
    End Sub
    'Page load can not use Handles Me.Load on override for performance reasons (page_load fires twice with Handles Me.Load)
    Protected Overrides Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)
        MyBase.Page_Load(sender, e)
        _businessUnit = TalentCache.GetBusinessUnit
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        _currentPage = Talent.eCommerce.Utilities.GetCurrentPageName()
        If IsValidID(Request("page")) Then
            _PageNumber = Int32.Parse(Request.QueryString("page"))
            _IsPaging = True
        End If

        If Not Page.IsPostBack Then
            '-----------------------------
            ' Populate 'Sort By' drop down
            '-----------------------------
            txtSortBy.Text = ucr.Content("SortByText", languageCode, True)
            ddlSortBy.Items.Add(ucr.Content("SortByOpt1", languageCode, True))
            ddlSortBy.Items.Add(ucr.Content("SortByOpt2", languageCode, True))
            ddlSortBy.Items.Add(ucr.Content("SortByOpt3", languageCode, True))
            ddlSortBy.Items.Add(ucr.Content("SortByOpt4", languageCode, True))
            ddlSortBy.Items.Add(ucr.Content("SortByOpt5", languageCode, True))
            ddlSortBy.AutoPostBack = True
            If Not Session("sortByIndex") Is Nothing Then
                ddlSortBy.SelectedIndex = CInt(Session("sortByIndex"))
            End If
        End If

        If Display Then
            pnlProductList.Visible = True
        Else
            pnlProductList.Visible = False
        End If

        If Not Page.IsPostBack And Display Then
            LoadProducts()
        End If


    End Sub


    Protected Sub LoadProducts()

        Dim pageProductsList As IList
        _caching = ucr.Attribute("Cacheing")
        _cacheTimeMinutes = ucr.Attribute("CacheTimeMinutes")
        completeProductsList = RetrieveProducts()

        '----------------------------
        ' Sort according to drop down
        '----------------------------
        Session("sortByIndex") = ddlSortBy.SelectedIndex
        '----------------------
        ' Only resort if 
        If Not completeProductsList Is Nothing Then
            If _reSort OrElse Not Page.IsPostBack Then
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
                _reSort = False
            End If

            pageProductsList = completeProductsList.GetPageProducts(_PageNumber)

            rptProducts.DataSource = pageProductsList
            rptProducts.DataBind()

        End If


    End Sub

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
        If Not Session("ProductOptionsError") Is Nothing AndAlso Not Session("ProductOptionsError").ToString = "" Then
            lblError.Text = Session("ProductOptionsError")
            Session("ProductOptionsError") = ""
            Exit Sub
        Else
            lblError.Text = String.Empty
        End If

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
        If Not e.Item.ItemIndex = -1 Then

            Dim po1 As UserControls_ProductOptions = CType(e.Item.FindControl("ProductOptions1"), UserControls_ProductOptions)
            Dim buyBtnPnl As HtmlControl = CType(e.Item.FindControl("buyButtonPanel"), HtmlControl)
            Dim buyBtn As Button = CType(e.Item.FindControl("btn_buy"), Button)

            If Not po1 Is Nothing Then
                po1.PopulateOptions()
            End If

            If Not po1.Visible AndAlso ModuleDefaults.AllowMasterProductsToBePurchased Then
                buyBtnPnl.Visible = True
                buyBtn.Enabled = True
                buyBtn.Text = ucr.Content("BuyButtonText", languageCode, True)
            Else
                buyBtnPnl.Visible = False
                buyBtn.Enabled = False
            End If
        End If
    End Sub

    '-----------------------------------
    ' Reload if sort by order is changed
    '-----------------------------------
    Protected Sub ddlSortBy_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSortBy.SelectedIndexChanged
        _currentPage = Talent.eCommerce.Utilities.GetCurrentPageName
        _reSort = True
        '  LoadProducts()
        Session("sortByIndex") = ddlSortBy.SelectedIndex
        Dim newUrl As String = String.Empty
        If Not Request.QueryString("page") Is Nothing Then
            newUrl = Request.Url.ToString.Substring(0, Request.Url.ToString.LastIndexOf("&page="))
        Else
            newUrl = Request.Url.ToString
        End If
        Response.Redirect(newUrl)
    End Sub

    Protected Sub rptProducts_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles rptProducts.Load

    End Sub
End Class
