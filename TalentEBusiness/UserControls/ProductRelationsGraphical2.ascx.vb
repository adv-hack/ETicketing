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
'       Function                    User Controls - Product Relations Graphical
'
'       Date                        23.03.2007
'
'       Author                      Andrew Green
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCPRELS- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_ProductRelationsGraphical2
    Inherits AbstractProductList

    Private emptyString As String = String.Empty

    Private conTalent As SqlConnection = Nothing
    Private cmdSelect As SqlCommand = Nothing
    Private dtrProduct As SqlDataReader = Nothing

    Private Shadows _display As Boolean = True

    Private requestGroups() As String = {"GROUP_L01_GROUP = @GROUP1", "GROUP_L02_GROUP = @GROUP2", _
                                         "GROUP_L03_GROUP = @GROUP3", "GROUP_L04_GROUP = @GROUP4", _
                                         "GROUP_L05_GROUP = @GROUP5", "GROUP_L06_GROUP = @GROUP6", _
                                         "GROUP_L07_GROUP = @GROUP7", "GROUP_L08_GROUP = @GROUP8", _
                                         "GROUP_L09_GROUP = @GROUP9", "GROUP_L10_GROUP = @GROUP10"}
    Private requestGroupsBlank() As String = {"GROUP_L01_GROUP = ''", "GROUP_L02_GROUP = ''", _
                                              "GROUP_L03_GROUP = ''", "GROUP_L04_GROUP = ''", _
                                              "GROUP_L05_GROUP = ''", "GROUP_L06_GROUP = ''", _
                                              "GROUP_L07_GROUP = ''", "GROUP_L08_GROUP = ''", _
                                              "GROUP_L09_GROUP = ''", "GROUP_L10_GROUP = ''"}

    Dim productRelationsList As ProductListGen
    
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

    Private intRepeatColumns As Integer = 0
    Private intPageSize As Integer = 0
    Private boolOrderBySequence As Boolean = False
    Private boolShowImage As Boolean = False
    Private boolShowText As Boolean = False
    Private boolShowPrice As Boolean = False
    Private boolShowBuy As Boolean = False
    Private boolShowLink As Boolean = False
    Private strHeaderText As String = String.Empty
    Private strPriceLabel As String = String.Empty
    Private strPriceFromLabel As String = String.Empty
    Private strBuyButtonText As String = String.Empty
    Private intQuantityTextBoxSize As Integer = 0
    Private setupControlSuccess As Boolean = False

    Private _forwardToBasket As Boolean
    Public Property ForwardToBasket() As Boolean
        Get
            Return _forwardToBasket
        End Get
        Set(ByVal value As Boolean)
            _forwardToBasket = value
        End Set
    End Property
    Dim _pos As Integer
    Public Property PagePosition() As Integer
        Get
            Return _pos
        End Get
        Set(ByVal value As Integer)
            _pos = value
        End Set
    End Property

    Dim _template As String
    Public Property TemplateType() As String
        Get
            Return _template
        End Get
        Set(ByVal value As String)
            _template = value
        End Set
    End Property
    ' BF - Removed 'Handles me.load from here as it is not required because it overrides the base page load
    ' If 'handles..' is put in then page_load get's called twice
    Protected Overrides Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)
        MyBase.Page_Load(sender, e)
        setupPager()
        If Display Then
            SetupControl()
            ' This *must* always load regardless of postback otherwise if there's a validation error 
            ' on the screen nothing displays. 

            ' If Not Page.IsPostBack And Display Then
            If Not Page.IsPostBack AndAlso setupControlSuccess Then
                LoadProducts()
            End If
        Else
            Me.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Set s the dislpay of the pager based upon whether
    ''' we are showing all products or not.
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub setupPager()
        If ShowAllItems Then
            'pagerDisplayString.Visible = False
            'pagerDisplayString2.Visible = False
            'pagerDisplayString3.Visible = False
            'pagerDisplayString4.Visible = False
        End If
        ShowAllButton.Text = ShowAllButtonText
    End Sub


    Protected Overrides Sub setupUCR()
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Usage()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "productRelationsGraphical.ascx"
        End With
    End Sub

    Protected Sub SetupControl()
        If Display Then
            pnlProductRelationsGraphical.Visible = True
        Else
            pnlProductRelationsGraphical.Visible = False
        End If

        Dim ds As DataSet = GetRelationsVariables()

        If Not ds.Tables("RelationsText") Is Nothing _
                            AndAlso Not ds.Tables("RelationsAttributes") Is Nothing Then

            setupControlSuccess = True
            Dim text As DataTable = ds.Tables("RelationsText")
            Dim attrib As DataTable = ds.Tables("RelationsAttributes")

            If text.Rows.Count > 0 Then
                For Each dr As DataRow In text.Rows
                    Select Case dr("TEXT_CODE")
                        Case Is = "HEADER_TEXT"
                            strHeaderText = dr("TEXT_VALUE")
                        Case Is = "PRICE_LABEL"
                            strPriceLabel = dr("TEXT_VALUE")
                        Case Is = "BUY_BUTTON_TEXT"
                            strBuyButtonText = dr("TEXT_VALUE")
                        Case Is = "FROM_PRICE_LABEL"
                            strPriceFromLabel = dr("TEXT_VALUE")
                    End Select
                Next
            End If

            ' Defaults
            If attrib.Rows.Count > 0 Then
                For Each dr As DataRow In attrib.Rows
                    Select Case dr("ATTRIBUTE_CODE")
                        Case Is = "REPEAT_COLUMNS"
                            intRepeatColumns = CInt(dr("ATTRIBUTE_VALUE"))
                        Case Is = "PAGE_SIZE"
                            intPageSize = CInt(dr("ATTRIBUTE_VALUE"))
                        Case Is = "ORDER_BY_SEQUENCE"
                            boolOrderBySequence = CBool(dr("ATTRIBUTE_VALUE"))
                        Case Is = "SHOW_IMAGE"
                            boolShowImage = CBool(dr("ATTRIBUTE_VALUE"))
                        Case Is = "SHOW_TEXT"
                            boolShowText = CBool(dr("ATTRIBUTE_VALUE"))
                        Case Is = "SHOW_PRICE"
                            boolShowPrice = CBool(dr("ATTRIBUTE_VALUE"))
                        Case Is = "SHOW_BUY"
                            boolShowBuy = CBool(dr("ATTRIBUTE_VALUE"))
                        Case Is = "SHOW_LINK"
                            boolShowLink = CBool(dr("ATTRIBUTE_VALUE"))
                        Case Is = "QUANTITY_BOX_MAX_LENGTH"
                            intQuantityTextBoxSize = CInt(dr("ATTRIBUTE_VALUE"))
                        Case Is = "FORWARD_TO_BASKET"
                            ForwardToBasket = CBool(dr("ATTRIBUTE_VALUE"))
                    End Select
                Next

            End If

            dlsProductRelationsGraphical.RepeatColumns = intRepeatColumns
        End If


    End Sub

    Protected Function GetRelationsVariables() As Data.DataSet
        Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
        Dim ds As New DataSet("RelationsVariables")

        Dim CacheName As String = "RelationsVariables_" & TalentCache.GetBusinessUnit & "_" & _
                                        Talent.eCommerce.Utilities.GetCurrentPageName.ToLower & "_" & _
                                        Usage & "_" & _
                                        PagePosition & "_" & _
                                        TemplateType

        Dim text As New DataTable("RelationsText")
        Dim attrib As New DataTable("RelationsAttributes")

        Const SelectText As String = " SELECT * FROM tbl_product_relations_text_lang WITH (NOLOCK)  " & _
                                        " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                        "  AND PARTNER = @PARTNER " & _
                                        "  AND PAGE_CODE = @PAGE_CODE " & _
                                        "  AND QUALIFIER = @QUALIFIER "

        Const SelectAttrib As String = " SELECT * FROM tbl_product_relations_attribute_values WITH (NOLOCK)  " & _
                                        " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                        "  AND PARTNER = @PARTNER " & _
                                        "  AND PAGE_CODE = @PAGE_CODE " & _
                                        "  AND QUALIFIER = @QUALIFIER " & _
                                        "  AND PAGE_POSITION = @PAGE_POSITION " & _
                                        "  AND TEMPLATE_TYPE = @TEMPLATE_TYPE "
        cmd.CommandText = SelectText


        If Not Talent.Common.TalentThreadSafe.ItemIsInCache(CacheName)  Then
            Try

                Dim da As New SqlDataAdapter(cmd)

                cmd.Connection.Open()

                '--------------------------------------------
                '   tbl_product_relations_attribute_values
                '--------------------------------------------
                If TemplateType Is Nothing Then
                    TemplateType = String.Empty
                End If
                With cmd.Parameters
                    .Clear()
                    .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                    .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
                    .Add("@PAGE_CODE", SqlDbType.NVarChar).Value = Talent.eCommerce.Utilities.GetCurrentPageName
                    .Add("@QUALIFIER", SqlDbType.NVarChar).Value = Usage
                    .Add("@PAGE_POSITION", SqlDbType.NVarChar).Value = PagePosition
                    .Add("@TEMPLATE_TYPE", SqlDbType.NVarChar).Value = TemplateType
                    .Add("@LANGUAGE_CODE", SqlDbType.NVarChar).Value = _languageCode
                End With

                da.Fill(text)

                If Not text.Rows.Count > 0 Then
                    cmd.Parameters("@PARTNER").Value = "*ALL"
                    da.Fill(text)
                    If Not text.Rows.Count > 0 Then
                        cmd.Parameters("@BUSINESS_UNIT").Value = "*ALL"
                        da.Fill(text)
                        If Not text.Rows.Count > 0 Then
                            cmd.Parameters("@BUSINESS_UNIT").Value = TalentCache.GetBusinessUnit
                            cmd.Parameters("@PARTNER").Value = TalentCache.GetPartner(Profile)
                            cmd.Parameters("@PAGE_CODE").Value = "*ALL"
                            da.Fill(text)
                            If Not text.Rows.Count > 0 Then
                                cmd.Parameters("@PARTNER").Value = "*ALL"
                                da.Fill(text)
                                If Not text.Rows.Count > 0 Then
                                    cmd.Parameters("@BUSINESS_UNIT").Value = "*ALL"
                                    da.Fill(text)
                                    If Not text.Rows.Count > 0 Then
                                        cmd.Parameters("@QUALIFIER").Value = "*ALL"
                                        cmd.Parameters("@PAGE_POSITION").Value = "*ALL"
                                        cmd.Parameters("@TEMPLATE_TYPE").Value = "*ALL"
                                        da.Fill(text)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If


                '--------------------------------------------
                '   tbl_product_relations_attribute_values
                '--------------------------------------------
                da.SelectCommand.CommandText = SelectAttrib
                With cmd.Parameters
                    .Clear()
                    .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                    .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
                    .Add("@PAGE_CODE", SqlDbType.NVarChar).Value = Talent.eCommerce.Utilities.GetCurrentPageName
                    .Add("@QUALIFIER", SqlDbType.NVarChar).Value = Usage
                    .Add("@PAGE_POSITION", SqlDbType.NVarChar).Value = PagePosition
                    .Add("@TEMPLATE_TYPE", SqlDbType.NVarChar).Value = TemplateType
                End With

                da.Fill(attrib)

                If Not attrib.Rows.Count > 0 Then
                    cmd.Parameters("@PARTNER").Value = "*ALL"
                    da.Fill(attrib)
                    If Not attrib.Rows.Count > 0 Then
                        cmd.Parameters("@BUSINESS_UNIT").Value = "*ALL"
                        da.Fill(attrib)
                        If Not attrib.Rows.Count > 0 Then
                            cmd.Parameters("@BUSINESS_UNIT").Value = TalentCache.GetBusinessUnit
                            cmd.Parameters("@PARTNER").Value = TalentCache.GetPartner(Profile)
                            cmd.Parameters("@PAGE_CODE").Value = "*ALL"
                            da.Fill(attrib)
                            If Not attrib.Rows.Count > 0 Then
                                cmd.Parameters("@PARTNER").Value = "*ALL"
                                da.Fill(attrib)
                                If Not attrib.Rows.Count > 0 Then
                                    cmd.Parameters("@BUSINESS_UNIT").Value = "*ALL"
                                    da.Fill(attrib)
                                    If Not attrib.Rows.Count > 0 Then
                                        cmd.Parameters("@QUALIFIER").Value = "*ALL"
                                        cmd.Parameters("@PAGE_POSITION").Value = "*ALL"
                                        cmd.Parameters("@TEMPLATE_TYPE").Value = "*ALL"
                                        da.Fill(attrib)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                ds.Tables.Add(text)
                ds.Tables.Add(attrib)
                TalentCache.AddPropertyToCache(CacheName, ds, 30, TimeSpan.Zero, CacheItemPriority.Normal)
                Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(CacheName)
            Catch ex As Exception

            End Try
        Else
            ds = CType(Cache.Item(CacheName), DataSet)
        End If

        Return ds
    End Function

    Protected Sub LoadProducts()

        Dim pageProductsList As IList
        Dim filePath As String = String.Empty

        _caching = Talent.Common.Utilities.CheckForDBNull_Boolean_DefaultTrue(ucr.Attribute("Cacheing"))
        _cacheTimeMinutes = ucr.Attribute("CacheTimeMinutes")
        productRelationsList = RetrieveProducts()
        productRelationsList.PageSize = intPageSize

        

        If Not productRelationsList Is Nothing Then
            If ShowAllItems() Then
                pageProductsList = productRelationsList.Products
            Else
                pageProductsList = productRelationsList.GetPageProducts(_PageNumber)
            End If
            If productRelationsList.Products.Count <= productRelationsList.PageSize Then
                ShowAllButton.Visible = False
            End If
          
            dlsProductRelationsGraphical.DataSource = pageProductsList
            dlsProductRelationsGraphical.DataBind()
        End If


    End Sub

    '--------------
    ' Add to basket
    '--------------
    Protected Sub dlsProductListGraphical_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataListCommandEventArgs) Handles dlsProductRelationsGraphical.ItemCommand
        Dim quant As Integer = ModuleDefaults.Default_Add_Quantity
        '    Dim customValidationSummary As New Talent.Commerce.CustomValidationSummary
        '   Dim pageref As System.Web.UI.Page = Me.Page
        Try
            quant = Integer.Parse(CType(e.Item.FindControl("txtQuantity"), TextBox).Text)
        Catch
        End Try

        Dim strProduct As String = e.CommandArgument.ToString

        Dim al As New ArrayList
        For Each strKey As String In Request.QueryString.Keys
            If strKey.ToLower.Contains("group") Then
                al.Add(Request.QueryString(strKey))
            End If
        Next

        Dim availQty As Integer = Stock.GetStockBalance(strProduct)
        If quant > availQty And ModuleDefaults.FrontEndStockRequiredToAddToBasket Then
            lblError.Text = ucr.Content("stockError", _languageCode, True)
        Else
            If quant >= ModuleDefaults.Min_Add_Quantity Then
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
                        .KeyCode = "productRelationsGraphical.ascx"
                    End With
                End If
                If ForwardToBasket Then
                    Response.Redirect("../../pagesPublic/basket/Basket.aspx")
                Else
                    Response.Redirect(Request.Url.ToString)
                End If
            Else
                lblError.Text = String.Format(ucr.Content("MinQuantityNotMetError", _languageCode, True), ModuleDefaults.Min_Add_Quantity.ToString("0"))
            End If
        End If
    End Sub

    Protected Sub dlsProductRelationsGraphical_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataListItemEventArgs) Handles dlsProductRelationsGraphical.ItemDataBound

        Dim p As Talent.eCommerce.Product = CType(e.Item.DataItem, Talent.eCommerce.Product)
        '---------------------------
        ' Set attributes in Datalist
        '---------------------------
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            If boolShowImage Then
                CType(e.Item.FindControl("ImageHyperLink"), HyperLink).Visible = True
            Else
                CType(e.Item.FindControl("ImageHyperLink"), HyperLink).Visible = False
            End If
            If boolShowLink Then
                CType(e.Item.FindControl("hypProductName"), HyperLink).Visible = True
            Else
                CType(e.Item.FindControl("hypProductName"), HyperLink).Visible = False
            End If
            If boolShowText Then
                CType(e.Item.FindControl("lblProductDescription"), Label).Visible = True
            Else
                CType(e.Item.FindControl("lblProductDescription"), Label).Visible = False
            End If
            If boolShowPrice Then
                CType(e.Item.FindControl("lblPriceText"), Label).Text = strPriceLabel
                CType(e.Item.FindControl("lblPriceText"), Label).Visible = True
                CType(e.Item.FindControl("lblPrice"), Label).Visible = True
            Else
                CType(e.Item.FindControl("lblPrice"), Label).Visible = False
                CType(e.Item.FindControl("lblPriceText"), Label).Visible = False
            End If
            If boolShowBuy Then
                CType(e.Item.FindControl("btnBuy"), Button).Text = strBuyButtonText
                CType(e.Item.FindControl("txtQuantity"), TextBox).Columns = intQuantityTextBoxSize
                CType(e.Item.FindControl("btnBuy"), Button).Visible = True
                CType(e.Item.FindControl("txtQuantity"), TextBox).Visible = True
            Else
                CType(e.Item.FindControl("btnBuy"), Button).Visible = False
                CType(e.Item.FindControl("txtQuantity"), TextBox).Visible = False
            End If
        End If

        Try
            If Talent.eCommerce.Stock.GetStockBalance(p.Code) < 1 And ModuleDefaults.FrontEndStockRequiredToAddToBasket Then
                CType(e.Item.FindControl("btnBuy"), Button).Visible = False
                CType(e.Item.FindControl("txtQuantity"), TextBox).Visible = False
                CType(e.Item.FindControl("NoStockLabel"), Label).Visible = True
                CType(e.Item.FindControl("NoStockLabel"), Label).Text = ucr.Content("ProductOutOfStockText", _languageCode, True)
            End If
        Catch ex As Exception

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
                    CType(e.Item.FindControl("lblPriceText"), Label).Text = strPriceLabel
                Else
                    CType(e.Item.FindControl("lblPriceText"), Label).Text = strPriceFromLabel
                End If
                CType(e.Item.FindControl("lblPrice"), Label).Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice_From, ucr.BusinessUnit, ucr.PartnerCode)
            Else
                CType(e.Item.FindControl("lblPriceText"), Label).Text = strPriceLabel
                CType(e.Item.FindControl("lblPrice"), Label).Text = TDataObjects.PaymentSettings.FormatCurrency(p.WebPrices.DisplayPrice, ucr.BusinessUnit, ucr.PartnerCode)
            End If

            CType(e.Item.FindControl("btnBuy"), Button).Text = strBuyButtonText
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
    End Sub

    Protected Sub GetRegExErrorText(ByVal sender As Object, ByVal e As EventArgs)
        If Display Then
            Dim regex As RegularExpressionValidator = (CType(sender, RegularExpressionValidator))
            Select Case regex.ID
                Case Is = "QuantityValidator"
                    regex.ErrorMessage = ucr.Content("QuantityValidatorErrorText", _languageCode, True)
            End Select
        End If

    End Sub

    Protected Sub GetDefaultQuantity(ByVal sender As Object, ByVal e As EventArgs)
        CType(sender, TextBox).Text = ModuleDefaults.Default_Add_Quantity.ToString("0")
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        If Display AndAlso Not pnlProductRelationsGraphical Is Nothing Then
            Try
                pnlProductRelationsGraphical.CssClass = "template-" & TemplateType.ToString
            Catch ex As Exception
            End Try
        End If

    End Sub

    Protected Sub ShowAllButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ShowAllButton.Click
        'We let the parent class handle the redirect
        handleShowAllRedirect()
    End Sub

End Class
