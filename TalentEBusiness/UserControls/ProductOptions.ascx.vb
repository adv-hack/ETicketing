Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common

Partial Class UserControls_ProductOptions
    Inherits ControlBase

#Region "Class Level Fields"

    Private _promotionItemEditMode As Boolean = False
    Private _personaliseButtonText As String = String.Empty
    Private _promotionOptionSelectButtonText As String = String.Empty
    Private _autoPopulate As Boolean = True
    Private _masterProduct As String
    Private _hasItems As Boolean = False
    Private _languageCode As String = String.Empty
    Private _ucr As Talent.Common.UserControlResource = Nothing
    Private _defs As DataTable
    Private _options As DataTable
    Private _langs As DataTable
    Private _stock As DataTable
    Private _optionTypes As Generic.List(Of String)
    Private _productPrices As Generic.Dictionary(Of String, Talent.Common.DEWebPrice)

#End Region

#Region "Public Properties"

    Public Property AutoPopulate() As Boolean
        Get
            Return _autoPopulate
        End Get
        Set(ByVal value As Boolean)
            _autoPopulate = value
        End Set
    End Property
    Public Property MasterProduct() As String
        Get
            Return _masterProduct
        End Get
        Set(ByVal value As String)
            _masterProduct = value
        End Set
    End Property
    Public Property HasItems() As Boolean
        Get
            Return _hasItems
        End Get
        Set(ByVal value As Boolean)
            _hasItems = value
        End Set
    End Property
    Public Property OptionDefaults() As DataTable
        Get
            Return _defs
        End Get
        Set(ByVal value As DataTable)
            _defs = value
        End Set
    End Property
    Public Property ProductOptions() As DataTable
        Get
            Return _options
        End Get
        Set(ByVal value As DataTable)
            _options = value
        End Set
    End Property
    Public Property OptionsLanguage() As DataTable
        Get
            Return _langs
        End Get
        Set(ByVal value As DataTable)
            _langs = value
        End Set
    End Property
    Public Property StockLevels() As DataTable
        Get
            Return _stock
        End Get
        Set(ByVal value As DataTable)
            _stock = value
        End Set
    End Property
    Public Property OptionTypes() As Generic.List(Of String)
        Get
            Return _optionTypes
        End Get
        Set(ByVal value As Generic.List(Of String))
            _optionTypes = value
        End Set
    End Property
    Public ReadOnly Property SingleOption() As Boolean
        Get
            Try
                If OptionDefaults.Rows.Count = 1 Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try
        End Get
    End Property
    Public Property OptionPrices() As Generic.Dictionary(Of String, Talent.Common.DEWebPrice)
        Get
            Return _productPrices
        End Get
        Set(ByVal value As Generic.Dictionary(Of String, Talent.Common.DEWebPrice))
            _productPrices = value
        End Set
    End Property
    Public Property ErrorMessage() As String

#End Region

#Region "Private Prod Class"

    Private Class Prod
        Private _quant As Decimal
        Private _size As String
        Public Property Quant() As Decimal
            Get
                Return _quant
            End Get
            Set(ByVal value As Decimal)
                _quant = value
            End Set
        End Property
        Public Property Size() As String
            Get
                Return _size
            End Get
            Set(ByVal value As String)
                _size = value
            End Set
        End Property

        Public Property Weight As Decimal = 0
    End Class

#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If Me.AutoPopulate Then Me.MasterProduct = Request.QueryString("product")
        _ucr = New Talent.Common.UserControlResource
        _languageCode = Talent.Common.Utilities.GetDefaultLanguage
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = Talent.Common.Utilities.GetAllString()
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "ProductOptions.ascx"
        End With
        _personaliseButtonText = _ucr.Content("PersonaliseButton", _languageCode, True)
        _promotionOptionSelectButtonText = _ucr.Content("PromotionOptionSelectButton", _languageCode, True)
        If Not (String.IsNullOrWhiteSpace(Request.QueryString("promotionbasketid"))) Then
            Dim basketItemsCollection As Generic.List(Of DEBasketItem) = Profile.Basket.BasketItems
            Dim promotionBasketItem As TalentBasketItem = basketItemsCollection.Find(Function(basketDetailItem As TalentBasketItem) basketDetailItem.Basket_Detail_ID = (Request.QueryString("promotionbasketid")).Trim)
            If promotionBasketItem IsNot Nothing AndAlso promotionBasketItem.IS_FREE Then
                _promotionItemEditMode = True
            End If
        End If
        If Me.AutoPopulate Then PopulateOptions()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If TABLE_BuyOptions.Visible Then
            Dim localPath As String = String.Empty
            If Talent.eCommerce.Utilities.DoesHtmlFileExists("\SizeChart\" & Me.MasterProduct & ".htm") Then
                plhSizeChart.Visible = True
                hplSizeChart.Text = _ucr.Content("SizeChartHyperlinkText", _languageCode, True)
                localPath = ModuleDefaults.HtmlPathAbsolute & "\SizeChart\" & Me.MasterProduct & ".htm"
            Else
                If Talent.eCommerce.Utilities.DoesHtmlFileExists("\SizeChart\" & Me.MasterProduct & ".html") Then
                    plhSizeChart.Visible = True
                    hplSizeChart.Text = _ucr.Content("SizeChartHyperlinkText", _languageCode, True)
                    localPath = ModuleDefaults.HtmlPathAbsolute & "\SizeChart\" & Me.MasterProduct & ".html"
                Else
                    plhSizeChart.Visible = False
                End If
            End If
            If plhSizeChart.Visible Then
                Dim virtualPath As String = localPath.Replace(ModuleDefaults.HtmlPathAbsolute, ModuleDefaults.HtmlIncludePathRelative)
                virtualPath = virtualPath.Replace("\", "/")
                hplSizeChart.NavigateUrl = virtualPath
            End If
        End If
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        Select Case CType(sender, Control).ID
            Case Is = "Option1Label"
                CType(sender, Literal).Text = _ucr.Content("Option1ColumnHeader", _languageCode, True)
            Case Is = "PriceLabel"
                If Talent.eCommerce.Utilities.ShowPrices(Profile) Then
                    CType(sender, Literal).Text = _ucr.Content("PriceColumnHeader", _languageCode, True)
                End If
            Case Is = "QuantityLabel"
                CType(sender, Literal).Text = _ucr.Content("QtyColumnHeader", _languageCode, True)
            Case Is = "Option2Label"
                CType(sender, Literal).Text = _ucr.Content("Option2ColumnHeader", _languageCode, True)
            Case Is = "StockLabel"
                CType(sender, Literal).Text = _ucr.Content("StockColumnHeader", _languageCode, True)
            Case Is = "PersonaliseLabel"
                CType(sender, Literal).Text = _ucr.Content("PersonaliseColumnHeader", _languageCode, True)
            Case Is = "PromotionSelectLabel"
                CType(sender, Literal).Text = _ucr.Content("PromotionSelectLabel", _languageCode, True)
        End Select
    End Sub

    ''' <summary>
    ''' Returns a ListItemCollection conatining the list items for the specified options level
    ''' </summary>
    ''' <param name="level">The level for the results required</param>
    ''' <returns>ListItemCollection</returns>
    ''' <remarks></remarks>
    Protected Function GetLevelOptions(ByVal level As Integer, _
                                        ByVal selections As Generic.List(Of String), _
                                        Optional ByVal onlyShowInStockItems As Boolean = True) As DataTable

        'Table to hold the option values for the list
        '--------------------------------------
        Dim levelOptions As New DataTable
        With levelOptions.Columns
            .Add("OptionType", GetType(String))
            .Add("DisplayType", GetType(String))
            .Add("OptionCode", GetType(String))
            .Add("DisplayName", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("Stock", GetType(String))
            .Add("DisplayOrder", GetType(Integer))
            .Add("Personalisable", GetType(Boolean))
            .Add("CountOfPrices", GetType(Integer))
            .Add("Weight", GetType(Decimal))
        End With

        'Table to place the re-ordered results into
        '--------------------------------------
        Dim reOrderedOptions As New DataTable
        With reOrderedOptions.Columns
            .Add("OptionType", GetType(String))
            .Add("DisplayType", GetType(String))
            .Add("OptionCode", GetType(String))
            .Add("DisplayName", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("Stock", GetType(String))
            .Add("DisplayOrder", GetType(Integer))
            .Add("Personalisable", GetType(Boolean))
            .Add("CountOfPrices", GetType(Integer))
            .Add("Weight", GetType(Decimal))
        End With

        Dim newRow As DataRow
        Try

            'Loop through all options for the current product
            For Each opt As DataRow In ProductOptions.Rows

                'Check for a match against the option type for the current option level we are querying
                If opt("OPTION_TYPE") = OptionDefaults.Rows(level - 1)("OPTION_TYPE") Then

                    'Loop through each of the language specific details
                    For Each lang As DataRow In OptionsLanguage.Rows

                        'Check to see if the option code for the current product option
                        'matches the option code for the current language details
                        If opt("OPTION_CODE") = lang("OPTION_CODE") Then

                            Dim found As Boolean = False 'indicates whether a valid option was found or not

                            'Loop through the stock levels table
                            For Each stockLevel As DataRow In StockLevels.Rows

                                'Check to ensure we are on the correct option product code within the stock table
                                If stockLevel(OptionDefaults.Rows(level - 1)("OPTION_TYPE")) = opt("PRODUCT_CODE") Then

                                    Dim passed As Boolean = False 'indicates whether the current selection tests were passed

                                    'If we are on the first level, there are no previous selections and so
                                    'we don't need to loop through
                                    If level > 1 Then

                                        'If we are not on the first level we need to only select items that
                                        'are in stock based on the current selections
                                        For i As Integer = 0 To level - 2

                                            'The OptionDefaults are in Display Order and so the order is 
                                            'the same as the selections list order
                                            If stockLevel(OptionDefaults.Rows(i)("OPTION_TYPE")) = selections(i) Then
                                                passed = True
                                            Else
                                                passed = False
                                                Exit For 'If we fail any then we MUST exit
                                            End If
                                        Next
                                    Else
                                        passed = True
                                    End If


                                    'If the current selection tests were passed, 
                                    If passed Then
                                        'Check to see if the product is in stock
                                        If stockLevel("Stock") > 0 OrElse Not onlyShowInStockItems Then
                                            'If so, add the details to the data table
                                            newRow = levelOptions.NewRow
                                            newRow("OptionType") = OptionDefaults.Rows(0)("OPTION_TYPE")
                                            newRow("DisplayType") = OptionDefaults.Rows(0)("DISPLAY_TYPE")
                                            newRow("OptionCode") = opt("OPTION_CODE")
                                            newRow("DisplayName") = lang("DISPLAY_NAME")
                                            newRow("ProductCode") = opt("PRODUCT_CODE")
                                            newRow("Stock") = stockLevel("Stock")
                                            newRow("DisplayOrder") = Utilities.CheckForDBNull_Int(opt("DISPLAY_ORDER"))
                                            newRow("Personalisable") = opt("PERSONALISABLE")
                                            newRow("CountOfPrices") = opt("COUNT_OF_PRICES")
                                            newRow("Weight") = stockLevel("Weight")
                                            levelOptions.Rows.Add(newRow)
                                            found = True

                                            'We have added a result so exit the "Stock" For loop
                                            Exit For
                                        End If

                                    End If
                                End If

                            Next

                            'As we have found a valid result, exit the "Language" For loop
                            If found Then Exit For

                        End If
                    Next
                End If
            Next
        Catch ex As Exception

        End Try
        Try
            Dim drc As DataRow() = levelOptions.Select("1=1", "DisplayOrder, DisplayName ASC")
            For Each dr As DataRow In drc
                reOrderedOptions.Rows.Add(dr.ItemArray)
            Next
        Catch ex As Exception
        End Try
        Return reOrderedOptions
    End Function

    Protected Sub DisplaySizingOptions()
        'Populates the sizing options with the relevant content
        If Not OptionDefaults Is Nothing AndAlso OptionDefaults.Rows.Count > 0 Then
            Dim selections As New Generic.List(Of String)
            Select Case UCase(Utilities.CheckForDBNull_String(OptionDefaults.Rows(0)("DISPLAY_TYPE")))
                Case Is = "DDL"
                    ddlOptionLevel1.DataSource = GetLevelOptions(1, selections)
                    ddlOptionLevel1.DataTextField = "DisplayName"
                    ddlOptionLevel1.DataValueField = "ProductCode"
                    ddlOptionLevel1.DataBind()
                    rptOptionLevel1.Visible = False
                    DDL_BuyOptions.Visible = True
                    TABLE_BuyOptions.Visible = False
                    If _promotionItemEditMode Then
                        Quantity.Visible = False
                        DDL_BuyButton.Visible = False
                        btnDDLPromotionOptionSelect.Visible = True
                    Else
                        btnDDLPromotionOptionSelect.Visible = False
                    End If
                Case Is = "TABLE"
                    rptOptionLevel1.DataSource = GetLevelOptions(1, selections, False)
                    rptOptionLevel1.DataBind()
                    For Each ri As RepeaterItem In rptOptionLevel1.Items
                        Dim option2DDL As DropDownList = CType(ri.FindControl("option2DDL"), DropDownList)
                        If Not option2DDL Is Nothing Then option2DDL_IndexChanged(option2DDL, New EventArgs)
                    Next
                    ddlLabelLevel1.Visible = False
                    ddlOptionLevel1.Visible = False
                    DDL_BuyOptions.Visible = False
                    TABLE_BuyOptions.Visible = True
                    If _promotionItemEditMode Then
                        TABLE_BuyOptions.Visible = False
                    End If
            End Select


            Try
                selections.Add(ddlOptionLevel1.SelectedValue)
            Catch ex As Exception
            End Try

            If OptionDefaults.Rows.Count > 1 AndAlso UCase(Utilities.CheckForDBNull_String(OptionDefaults.Rows(0)("DISPLAY_TYPE"))) <> "TABLE" Then
                Select Case UCase(Utilities.CheckForDBNull_String(OptionDefaults.Rows(1)("DISPLAY_TYPE")))
                    Case Is = "DDL"
                        ddlOptionLevel2.DataSource = GetLevelOptions(2, selections)
                        ddlOptionLevel2.DataTextField = "DisplayName"
                        ddlOptionLevel2.DataValueField = "ProductCode"
                        ddlOptionLevel2.DataBind()
                End Select
                If ddlOptionLevel2.Items.Count = 0 Then
                    Level2Options.Visible = False
                End If
            Else
                Level2Options.Visible = False
            End If


            If OptionDefaults.Rows.Count > 2 AndAlso UCase(Utilities.CheckForDBNull_String(OptionDefaults.Rows(0)("DISPLAY_TYPE"))) <> "TABLE" Then
                Try
                    selections.Add(ddlOptionLevel2.SelectedValue)
                Catch ex As Exception
                End Try

                Select Case UCase(Utilities.CheckForDBNull_String(OptionDefaults.Rows(2)("DISPLAY_TYPE")))
                    Case Is = "DDL"
                        ddlOptionLevel3.DataSource = GetLevelOptions(3, selections)
                        ddlOptionLevel3.DataTextField = "DisplayName"
                        ddlOptionLevel3.DataValueField = "ProductCode"
                        ddlOptionLevel3.DataBind()
                End Select
                If ddlOptionLevel3.Items.Count = 0 Then
                    Level3Options.Visible = False
                End If
            Else
                Level3Options.Visible = False
            End If

        Else
            Me.Visible = False
        End If


    End Sub

    Protected Sub TABLE_BuyButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles TABLE_BuyButton.Click
        Dim lastItem As Boolean = False
        Dim productsToAdd2 As New Generic.Dictionary(Of String, Prod)
        Dim DeproductsToAdd As New Generic.Dictionary(Of String, DeProductLines)
        ErrorMessage = String.Empty

        Dim itemSelected As Boolean = False

        For Each ri As RepeaterItem In rptOptionLevel1.Items
            Dim qtyBox As TextBox = CType(ri.FindControl("quantityBox"), TextBox)
            Dim maxQuantity As Integer = 0
            Try
                maxQuantity = CInt(_ucr.Attribute("MaximumQuantityValue"))
            Catch ex As Exception
            End Try
            If Not itemSelected AndAlso qtyBox.Text.Length > 0 Then
                itemSelected = True
            End If
            If maxQuantity > 0 Then
                'apply the maximum quantity value
                If qtyBox.Text.Length > 0 Then
                    Try
                        Dim quantity As Integer = CType(qtyBox.Text, Integer)
                        If quantity > 0 And quantity <= maxQuantity Then
                            'Quantity is ok
                        Else
                            Session("ProductOptionsError") = _ucr.Content("invalidQuantityErrorText", _languageCode, True)
                            ErrorMessage = Session("ProductOptionsError")
                            Exit Sub
                        End If
                    Catch ex As Exception
                        Session("ProductOptionsError") = _ucr.Content("invalidQuantityErrorText", _languageCode, True)
                        ErrorMessage = Session("ProductOptionsError")
                        Exit Sub
                    End Try
                End If
            Else
                'ignore any maximum quantity value
                If qtyBox.Text.Length > 0 Then
                    Try
                        qtyBox.Text = CType(qtyBox.Text, Integer)
                    Catch ex As Exception
                        Session("ProductOptionsError") = _ucr.Content("invalidQuantityErrorText", _languageCode, True)
                        ErrorMessage = Session("ProductOptionsError")
                        Exit Sub
                    End Try
                End If
            End If
        Next

        If Not itemSelected Then
            Session("ProductOptionsError") = _ucr.Content("invalidQuantityErrorText", _languageCode, False)
            ErrorMessage = Session("ProductOptionsError")
            Exit Sub
        End If

        For Each ri As RepeaterItem In rptOptionLevel1.Items
            Dim newProductCode As Literal = CType(ri.FindControl("newProductCode"), Literal)
            Dim qtyBox As TextBox = CType(ri.FindControl("quantityBox"), TextBox)
            Dim sizeLabel As Literal = CType(ri.FindControl("optionLabel"), Literal)
            If Not String.IsNullOrEmpty(qtyBox.Text) Then
                'Set the quantity
                '-----------------------
                Dim quant As Decimal = ModuleDefaults.Min_Add_Quantity
                Try
                    quant = CInt(qtyBox.Text)
                Catch ex As Exception
                    quant = ModuleDefaults.Min_Add_Quantity
                End Try
                If quant = 0 Then
                    quant = ModuleDefaults.Min_Add_Quantity
                End If
                '-----------------------
                'productsToAdd.Add(newProductCode.Text, quant)
                Dim p As New Prod
                p.Quant = quant
                p.Size = sizeLabel.Text
                productsToAdd2.Add(newProductCode.Text, p)
                Dim dep As New DeProductLines
                dep.Quantity = quant
                dep.ProductDescription = sizeLabel.Text
                DeproductsToAdd.Add(newProductCode.Text, dep)
            End If
        Next

        If Talent.eCommerce.Utilities.IsPartnerHomeDeliveryType(Profile) Then
            Session("QuickOrderProducts") = DeproductsToAdd
            Response.Redirect("~/PagesLogin/QuickOrder/QuickOrder.aspx")
        Else

            If productsToAdd2.Count > 0 Then
                Dim i As Integer = 0
                'For Each product As String In productsToAdd.Keys
                For Each product As String In productsToAdd2.Keys
                    i += 1
                    If i = productsToAdd2.Count Then lastItem = True
                    AddToBasket(productsToAdd2.Item(product).Quant, product, productsToAdd2.Item(product).Size, lastItem)
                Next
            Else
                'report no item quantities specified
            End If
        End If
    End Sub

    Protected Sub buyButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDL_BuyButton.Click
        If Talent.eCommerce.Utilities.IsPartnerHomeDeliveryType(Profile) Then
            Response.Redirect("~/PagesLogin/QuickOrder/QuickOrder.aspx")
        Else
            Dim productCode As String = ""
            Dim quant As Decimal = ModuleDefaults.Min_Add_Quantity
            Try
                quant = CInt(Quantity.Text)
            Catch ex As Exception
                quant = ModuleDefaults.Min_Add_Quantity
            End Try
            If quant = 0 Then
                quant = ModuleDefaults.Min_Add_Quantity
            End If
            productCode = GetProductCodeFromDDLOption()
            AddToBasket(quant, productCode, "")
        End If
    End Sub

    Protected Sub AddToBasket(ByVal itemQty As Decimal, ByVal productCode As String, ByVal size As String, Optional ByVal isLastItem As Boolean = True)
        Dim tbi As New TalentBasketItem
        Dim newProductCode As String = ""
        Dim price As Decimal = 0

        If Me.MasterProduct Is Nothing Then
            Try
                '------------------------------------------------------------------
                ' If called from a product list then Master Product will be nothing
                ' Need to get from parents controls
                '------------------------------------------------------------------
                Dim h As System.Type = Me.Parent.GetType
                If h.Name = "RepeaterItem" Then
                    Dim hiddenCode As HiddenField = CType(Me.Parent.FindControl("productCodeHidden"), HiddenField)
                    Me.MasterProduct = hiddenCode.Value
                End If

            Catch ex As Exception
            End Try
        End If

        tbi.MASTER_PRODUCT = Me.MasterProduct
        tbi.Quantity = itemQty
        tbi.Product = productCode
        tbi.Size = size

        Dim err As Talent.Common.ErrorObj
        Dim products As DataTable
        Dim prodInfo As New DEProductInfo(TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), tbi.Product, _languageCode)
        Dim DBProdInfo As New DBProductInfo(prodInfo)
        DBProdInfo.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        err = DBProdInfo.AccessDatabase()

        If Not err.HasError Then
            products = DBProdInfo.ResultDataSet.Tables("ProductInformation")
            If products.Rows.Count > 0 Then
                Dim product As Data.DataRow = products.Rows(0)
                With tbi
                    Select Case ModuleDefaults.PricingType
                        Case 2
                            Dim prices As DataTable = Talent.eCommerce.Utilities.GetChorusPrice(productCode, itemQty)
                            If prices.Rows.Count > 0 Then
                                .Gross_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("GrossPrice"))
                                .Net_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("NetPrice"))
                                .Tax_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("TaxPrice"))
                            End If
                        Case Else
                            Dim deWp As DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(productCode, itemQty, Me.MasterProduct)
                            .Gross_Price = deWp.Purchase_Price_Gross
                            .Net_Price = deWp.Purchase_Price_Net
                            .Tax_Price = deWp.Purchase_Price_Tax
                    End Select
                    Try
                        .ALTERNATE_SKU = Talent.eCommerce.Utilities.CheckForDBNull_String(product("ALTERNATE_SKU"))
                        .PRODUCT_DESCRIPTION1 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_1"))
                        .PRODUCT_DESCRIPTION2 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_2"))
                        .PRODUCT_DESCRIPTION3 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_3"))
                        .PRODUCT_DESCRIPTION4 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_4"))
                        .PRODUCT_DESCRIPTION5 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_5"))
                        .GROUP_LEVEL_01 = product("GROUP_L01_GROUP")
                        .GROUP_LEVEL_02 = product("GROUP_L02_GROUP")
                        .GROUP_LEVEL_03 = product("GROUP_L03_GROUP")
                        .GROUP_LEVEL_04 = product("GROUP_L04_GROUP")
                        .GROUP_LEVEL_05 = product("GROUP_L05_GROUP")
                        .GROUP_LEVEL_06 = product("GROUP_L06_GROUP")
                        .GROUP_LEVEL_07 = product("GROUP_L07_GROUP")
                        .GROUP_LEVEL_08 = product("GROUP_L08_GROUP")
                        .GROUP_LEVEL_09 = product("GROUP_L09_GROUP")
                        .GROUP_LEVEL_10 = product("GROUP_L10_GROUP")
                        .Xml_Config = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PERSONALISABLE"))
                        .WEIGHT = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_WEIGHT"))
                    Catch ex As Exception
                    End Try
                    If Not Profile.IsAnonymous And Not Profile.PartnerInfo.Details Is Nothing Then
                        .Cost_Centre = Profile.PartnerInfo.Details.COST_CENTRE
                        .Account_Code = Talent.eCommerce.Order.GetLastAccountNo(Profile.User.Details.LoginID)
                    End If
                End With
                Profile.Basket.AddItem(tbi)
                If isLastItem Then
                    Try
                        If Talent.eCommerce.Utilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(CType(_ucr.Attribute("ForwardToBasketOnAdd"), Boolean)) Then
                            Response.Redirect("../../PagesPublic/Basket/Basket.aspx")
                        Else
                            If Me.AutoPopulate Then
                                Response.Redirect(Request.Url.ToString)
                            Else
                                RefreshMiniBasketAndClearCurrentOptions()
                            End If
                        End If
                    Catch ex As Exception
                        Response.Redirect("../../PagesPublic/Basket/Basket.aspx")
                    End Try

                End If
            Else
                'ERROR: the product does not exist
            End If
        Else
            'ERROR: could not retrieve product info
        End If
    End Sub

    Protected Sub RefreshMiniBasketAndClearCurrentOptions()
        Try
            Dim miniBasket As Control = Talent.eCommerce.Utilities.FindWebControl("MiniBasket1", Page.Controls)
            If Not miniBasket Is Nothing Then
                CallByName(miniBasket, "ReBindBasket", CallType.Method)
            End If
            For Each ri As RepeaterItem In Me.rptOptionLevel1.Items
                Dim qtyBox As TextBox = CType(ri.FindControl("quantityBox"), TextBox)
                qtyBox.Text = ""
            Next
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub ddlOptionLevel1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlOptionLevel1.SelectedIndexChanged
        Try
            Dim selections As New Generic.List(Of String)
            Try
                selections.Add(ddlOptionLevel1.SelectedValue)
            Catch ex As Exception
            End Try
            ddlOptionLevel2.DataSource = GetLevelOptions(2, selections)
            ddlOptionLevel2.DataTextField = "DisplayName"
            ddlOptionLevel2.DataValueField = "ProductCode"
            ddlOptionLevel2.DataBind()

            Try
                selections.Add(ddlOptionLevel2.SelectedValue)
            Catch ex As Exception
            End Try
            ddlOptionLevel3.Items.Clear()

            ddlOptionLevel3.DataSource = GetLevelOptions(3, selections)
            ddlOptionLevel3.DataTextField = "DisplayName"
            ddlOptionLevel3.DataValueField = "ProductCode"
            ddlOptionLevel3.DataBind()
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ddlOptionLevel2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlOptionLevel2.SelectedIndexChanged
        Try
            Dim selections As New Generic.List(Of String)
            Try
                selections.Add(ddlOptionLevel1.SelectedValue)
                selections.Add(ddlOptionLevel2.SelectedValue)
            Catch ex As Exception
            End Try
            ddlOptionLevel3.DataSource = GetLevelOptions(3, selections)
            ddlOptionLevel3.DataTextField = "DisplayName"
            ddlOptionLevel3.DataValueField = "ProductCode"
            ddlOptionLevel3.DataBind()
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ddlOptionLevel3_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlOptionLevel3.SelectedIndexChanged
        Try

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub rptOptionLevel1_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptOptionLevel1.ItemDataBound
        If e.Item.ItemIndex > -1 Then
            Dim ri As RepeaterItem = e.Item
            Dim newProductCode As Literal = CType(ri.FindControl("newProductCode"), Literal)
            Dim optionValue As Literal = CType(ri.FindControl("option1Value"), Literal)
            Dim optionLabel As Literal = CType(ri.FindControl("optionLabel"), Literal)
            Dim priceLabel As Literal = CType(ri.FindControl("priceLabel"), Literal)
            Dim qtyBox As TextBox = CType(ri.FindControl("quantityBox"), TextBox)
            Dim stockLabel As Literal = CType(ri.FindControl("stockLabel"), Literal)
            Dim option2DDL As DropDownList = CType(ri.FindControl("option2DDL"), DropDownList)
            Dim option3DDL As DropDownList = CType(ri.FindControl("option3DDL"), DropDownList)
            Dim btnPersonalise As Button = CType(ri.FindControl("btnPersonalise"), Button)
            Dim plhPersonalise As PlaceHolder = CType(ri.FindControl("plhPersonalise"), PlaceHolder)
            Dim btnPromotionOptionSelect As Button = CType(ri.FindControl("btnPromotionOptionSelect"), Button)
            Dim hdfMasterProductCode As HiddenField = CType(ri.FindControl("hdfMasterProductCode"), HiddenField)
            Dim hdfProductWeight As HiddenField = CType(ri.FindControl("hdfProductWeight"), HiddenField)
            Dim selections As New Generic.List(Of String)
            Dim dRow As DataRow = CType(e.Item.DataItem, DataRowView).Row

            btnPromotionOptionSelect.Text = _promotionOptionSelectButtonText

            If Utilities.CheckForDBNull_Boolean_DefaultFalse(dRow("Personalisable")) Then
                btnPersonalise.Text = _personaliseButtonText
                plhPersonalise.Visible = True
            Else
                plhPersonalise.Visible = False
            End If

            If ModuleDefaults.HideOptionsWithoutPrice Then
                Dim DoesOptionHavePrice As Integer = 0
                DoesOptionHavePrice = Utilities.CheckForDBNull_Int(dRow("CountOfPrices"))
                If DoesOptionHavePrice <= 0 Then
                    ' hide the row
                    e.Item.Visible = False
                End If
            End If

            optionLabel.Text = Utilities.CheckForDBNull_String(dRow("DisplayName"))
            newProductCode.Text = Utilities.CheckForDBNull_String(dRow("ProductCode"))
            hdfMasterProductCode.Value = Utilities.CheckForDBNull_String(Me.MasterProduct)
            optionValue.Text = Utilities.CheckForDBNull_String(dRow("ProductCode"))
            hdfProductWeight.Value = Utilities.CheckForDBNull_String(dRow("Weight"))
            hdfProductWeight.Visible = False
            optionValue.Visible = False
            newProductCode.Visible = False

            selections.Clear()
            selections.Add(Utilities.CheckForDBNull_String(dRow("ProductCode")))

            ' option defaults must be greater than 1 to have level 2 options, otherwise it just causes an exception
            If OptionDefaults.Rows.Count > 1 Then
                Dim level2Options As DataTable = GetLevelOptions(2, selections, False)
                If level2Options.Rows.Count > 0 Then
                    option2DDL.DataSource = level2Options
                    option2DDL.DataTextField = "DisplayName"
                    option2DDL.DataValueField = "ProductCode"
                    option2DDL.DataBind()
                    'option2DDL.Items.Insert(0, "--")
                End If


                Try
                    If OptionDefaults.Rows.Count > 1 Then

                        selections.Add(option2DDL.SelectedValue)
                        ' option defaults must be greater than 2 to have level3 options, otherwise it just causes an exception
                        Dim level3Options As DataTable = GetLevelOptions(3, selections, False)
                        If level3Options.Rows.Count > 0 Then
                            option3DDL.DataSource = level3Options
                            option3DDL.DataTextField = "DisplayName"
                            option3DDL.DataValueField = "ProductCode"
                            option3DDL.DataBind()
                            'option3DDL.Items.Insert(0, "--")
                        End If
                    End If
                Catch ex As Exception

                End Try
            End If
        End If

    End Sub

    Protected Sub rptOptionLevel1_OnItemCommand(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs)
        If e.CommandName = "Personalise" Then
            Dim childProductCode As Literal = CType(e.Item.FindControl("newProductCode"), Literal)
            Dim masterProductCode As String = Request.QueryString("product")
            If String.IsNullOrEmpty(masterProductCode) And Not String.IsNullOrEmpty(childProductCode.Text) Then
                masterProductCode = _masterProduct
                If String.IsNullOrEmpty(_masterProduct) Then
                    masterProductCode = CType(e.Item.FindControl("hdfMasterProductCode"), HiddenField).Value
                End If
            End If
            Session("overrideXML") = ""
            Session("personalisationMode") = "A"
            Response.Redirect("Personalise.aspx?product=" & childProductCode.Text & "&master=" & masterProductCode)
        ElseIf e.CommandName = "PromotionOptionSelect" Then
            Dim childProductCode As Literal = CType(e.Item.FindControl("newProductCode"), Literal)
            Dim masterProductCode As String = CType(e.Item.FindControl("hdfMasterProductCode"), HiddenField).Value
            Dim sizeLabel As Literal = CType(e.Item.FindControl("optionLabel"), Literal)
            UpdateFreeItemInBasket(childProductCode.Text, sizeLabel.Text, masterProductCode)
        End If
    End Sub

    Protected Sub option2DDL_IndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim option2DDL As DropDownList = CType(sender, DropDownList)
        Dim ri As RepeaterItem = CType(CType(option2DDL.Parent, HtmlTableCell).Parent, RepeaterItem)
        Dim newProductCode As Literal = CType(ri.FindControl("newProductCode"), Literal)
        Dim optionLabel As Literal = CType(ri.FindControl("optionLabel"), Literal)
        Dim optionValue As Literal = CType(ri.FindControl("option1Value"), Literal)
        Dim priceLabel As Literal = CType(ri.FindControl("priceLabel"), Literal)
        Dim qtyBox As TextBox = CType(ri.FindControl("quantityBox"), TextBox)
        Dim stockLabel As Literal = CType(ri.FindControl("stockLabel"), Literal)

        Dim selections As New Generic.List(Of String)
        selections.Add(newProductCode.Text)
        selections.Add(option2DDL.SelectedValue)

        're-order the table rows by the APPEND_SEQUENCE value
        Dim drc As DataRow() = OptionDefaults.Select("1=1", "APPEND_SEQUENCE ASC")

        Dim fullProductCode As String = Me.MasterProduct
        Dim levelCount As Integer = 0

        'Loop through the re-ordered datarows
        Dim validProduct As Boolean = True
        For Each dr As DataRow In drc
            levelCount = 0

            'We need to know which display level the row is associated with
            'so loop through each row of the original results table, and take 
            'the row number of the row that matches the ID of the re-ordered 
            'row
            For Each opt As DataRow In OptionDefaults.Rows
                levelCount += 1
                If Utilities.CheckForDBNull_BigInt(dr("ID")) = Utilities.CheckForDBNull_BigInt(opt("ID")) Then
                    Exit For
                End If
            Next
            Dim addCode As String = GetOptionProductCode(Utilities.CheckForDBNull_String(dr("OPTION_TYPE")), _
                                                                 levelCount, _
                                                                 "TABLE", _
                                                                 ri.ItemIndex)

            If dr("MATCH_ACTION").ToString.Trim.ToUpper = "APPEND" Then
                fullProductCode += addCode
            Else
                fullProductCode = addCode
            End If

        Next

        For Each rw As DataRow In Me.StockLevels.Rows
            If rw("ProductCode") = fullProductCode Then
                If CDec(rw("Stock")) > 0 Then
                    stockLabel.Text = _ucr.Content("InStockText", _languageCode, True)
                Else
                    Dim stockTime As String = String.Empty
                    stockLabel.Text = Talent.eCommerce.Stock.GetNoStockDescription(fullProductCode, stockTime)
                End If
                Exit For
            End If
        Next

        If Talent.eCommerce.Utilities.ShowPrices(Profile) Then

            Dim price As New DEWebPrice
            If OptionPrices.ContainsKey(fullProductCode) And Not ModuleDefaults.UseOptionPriceFromMasterProduct Then
                price = OptionPrices(fullProductCode)
            ElseIf OptionPrices.ContainsKey(Me.MasterProduct) Then
                price = OptionPrices(Me.MasterProduct)
            ElseIf ModuleDefaults.UseOptionPriceFromMasterProduct Then
                'The master product price should be displayed
                Dim attribList As String = ""

                If Not CType(HttpContext.Current.Profile, TalentProfile).IsAnonymous Then
                    attribList = CType(HttpContext.Current.Profile, TalentProfile).User.Details.ATTRIBUTES_LIST
                End If

                Dim defaultPriceList As String = ModuleDefaults.DefaultPriceList
                If Not ModuleDefaults.UseGlobalPriceListWithCustomerPriceList Then defaultPriceList = ModuleDefaults.PriceList

                Dim pricingsettings As New Talent.Common.DEWebPriceSetting(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString, _
                                                                                        "SQL2005", _
                                                                                        "", _
                                                                                        TalentCache.GetBusinessUnit, _
                                                                                        TalentCache.GetPartner(HttpContext.Current.Profile), _
                                                                                        ModuleDefaults.PriceList, _
                                                                                        False, _
                                                                                        Talent.Common.Utilities.GetDefaultLanguage, _
                                                                                        Talent.eCommerce.Utilities.GetPartnerGroup, _
                                                                                        defaultPriceList, _
                                                                                        attribList)

                Dim products As New Generic.Dictionary(Of String, WebPriceProduct)

                If Me.StockLevels.Rows.Count > 0 Then
                    'Add all the child products
                    For Each stkRow As DataRow In StockLevels.Rows
                        Dim pCode As String = Utilities.CheckForDBNull_String(stkRow("ProductCode"))
                        If Not products.ContainsKey(pCode) Then
                            products.Add(pCode, New WebPriceProduct(pCode, 0, Me.MasterProduct))
                        End If
                    Next
                End If

                Dim productPricing As New Talent.Common.TalentWebPricing(pricingsettings, products, Not ModuleDefaults.ShowPricesExVAT)
                productPricing.GetWebPricesWithMasterProducts()

                If productPricing.RetrievedPrices Is Nothing Then
                    productPricing.RetrievedPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)()
                End If

                If productPricing.RetrievedPrices.ContainsKey(fullProductCode) Then
                    price = productPricing.RetrievedPrices(fullProductCode)
                End If
            End If

            priceLabel.Text = TDataObjects.PaymentSettings.FormatCurrency(Utilities.CheckForDBNull_Decimal(price.DisplayPrice).ToString, _ucr.BusinessUnit, _ucr.PartnerCode)
        End If

        If validProduct Then newProductCode.Text = fullProductCode
    End Sub

    Protected Sub option3DDL_IndexChanged(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Protected Sub checkLevel2Visibility(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Select Case OptionDefaults.Rows(1)("DISPLAY_TYPE")
                Case Is = "HIDDENDDL"
                    CType(sender, HtmlTableCell).Visible = False
            End Select
        Catch ex As Exception
            CType(sender, HtmlTableCell).Visible = False
        End Try
    End Sub

    Protected Sub checkLevel3Visibility(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Select Case OptionDefaults.Rows(2)("DISPLAY_TYPE")
                Case Is = "HIDDENDDL"
                    CType(sender, HtmlTableCell).Visible = False
            End Select
        Catch ex As Exception
            CType(sender, HtmlTableCell).Visible = False
        End Try
    End Sub

    Protected Sub CheckVisibilityForFreeItem(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim htmTableCell As HtmlTableCell = CType(sender, HtmlTableCell)
            If _promotionItemEditMode Then
                If htmTableCell.ID.ToUpper() = "PROMOTIONCOLUMN" Then
                    htmTableCell.Visible = True
                Else
                    htmTableCell.Visible = False
                End If
            Else
                If htmTableCell.ID.ToUpper() = "PROMOTIONCOLUMN" Then
                    htmTableCell.Visible = False
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    Protected Sub btnDDLPromotionOptionSelect_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDDLPromotionOptionSelect.Click
        Dim productCode As String = ""
        productCode = GetProductCodeFromDDLOption()
        UpdateFreeItemInBasket(productCode, "", Me.MasterProduct)
    End Sub

    Protected Sub rptOptionLevel1_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles rptOptionLevel1.PreRender
        If rptOptionLevel1.Items.Count > 0 Then
            Dim hasPersonalisation As Boolean = False
            For Each optionLevel As RepeaterItem In rptOptionLevel1.Items
                Dim btnPersonalise As Button = CType(optionLevel.FindControl("btnPersonalise"), Button)
                If btnPersonalise.Visible Then
                    hasPersonalisation = True
                    Exit For
                End If
            Next

            If Not hasPersonalisation Then
                CType(rptOptionLevel1.Controls(0).Controls(0).FindControl("plhPersonalise"), PlaceHolder).Visible = False
            End If
        End If
    End Sub

#End Region

#Region "Private Methods"

    Public Sub PopulateOptions()
        If Not Page.IsPostBack Then
            If Talent.eCommerce.Utilities.IsPartnerHomeDeliveryType(Profile) Then
                TABLE_BuyButton.Text = _ucr.Content("AddToBasketButtonTextForHomeDelivery", _languageCode, True)
                DDL_BuyButton.Text = _ucr.Content("AddToBasketButtonTextForHomeDelivery", _languageCode, True)
            Else
                TABLE_BuyButton.Text = _ucr.Content("AddToBasketButtonText", _languageCode, True)
                DDL_BuyButton.Text = _ucr.Content("AddToBasketButtonText", _languageCode, True)
            End If
        End If
        GetProductOptions()
        If Not ProductOptions Is Nothing Then setupTexts()

        DisplaySizingOptions()

        If ddlOptionLevel1.Items.Count > 0 OrElse rptOptionLevel1.Items.Count > 0 Then
            HasItems = True
        End If
    End Sub

    Private Sub setupTexts()
        Quantity.Text = Math.Floor(ModuleDefaults.Default_Add_Quantity)
        For i As Integer = 0 To OptionDefaults.Rows.Count - 1
            Select Case i
                Case Is = 0
                    ddlLabelLevel1.Text = Utilities.CheckForDBNull_String(OptionDefaults.Rows(i)("LABEL_TEXT"))
                Case Is = 1
                    ddlLabelLevel2.Text = Utilities.CheckForDBNull_String(OptionDefaults.Rows(i)("LABEL_TEXT"))
                Case Is = 2
                    ddlLabelLevel3.Text = Utilities.CheckForDBNull_String(OptionDefaults.Rows(i)("LABEL_TEXT"))
            End Select
        Next
    End Sub

    Private Sub NewStockTable(ByVal columns As Generic.List(Of String))
        StockLevels = New DataTable("StockTable")
        With StockLevels
            .Columns.Add("ProductCode", GetType(String))
            .Columns.Add("Weight", GetType(Decimal))
            For Each column As String In columns
                .Columns.Add(column, GetType(String))
            Next
            .Columns.Add("Stock", GetType(Decimal))
        End With
    End Sub

    Private Sub GetProductOptions()
        If Not String.IsNullOrEmpty(Me.MasterProduct) Then
            Dim productCode As String = Me.MasterProduct

            Dim cmd As New SqlCommand("", New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))

            Dim defs As New DataTable, _
                options As New DataTable, _
                langs As New DataTable, _
                typesLang As New DataTable

            Dim optTypes As New Generic.List(Of String)

            Try
                Dim da As New SqlDataAdapter(cmd)
                cmd.Connection.Open()

                'Get the product sizes for the current product
                options = GetProductOptions(da, productCode)

                If options.Rows.Count > 0 Then
                    optTypes = GetOptionTypes(options)
                    langs = GetOptionsLangs(da)
                    defs = GetOptionDefaults(da, optTypes)

                    OptionDefaults = defs
                    ProductOptions = options
                    OptionsLanguage = langs
                    OptionTypes = optTypes

                    'Build a table containing stock levels
                    '-------------------------
                    're-order the table rows by the APPEND_SEQUENCE value
                    Dim drc As DataRow() = OptionDefaults.Select("1=1", "APPEND_SEQUENCE ASC")
                    Dim columns As New Generic.List(Of String)
                    For Each dr As DataRow In drc
                        If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dr("OPTION_TYPE"))) Then
                            columns.Add(dr("OPTION_TYPE"))
                        End If
                    Next
                    NewStockTable(columns)

                    Dim masterList As New Generic.List(Of Generic.List(Of String))
                    Dim newList As New Generic.List(Of String)


                    'Loop through each option type
                    For Each dr As DataRow In drc
                        If Not String.IsNullOrEmpty(dr("OPTION_TYPE")) Then
                            newList = New Generic.List(Of String)

                            'Add all options for the current type to a list
                            For Each product As DataRow In ProductOptions.Rows
                                If Utilities.CheckForDBNull_String(dr("OPTION_TYPE")) = Utilities.CheckForDBNull_String(product("OPTION_TYPE")) Then
                                    newList.Add(product("PRODUCT_CODE"))
                                End If
                            Next

                            'Once all have been added, add the options list to the master list
                            masterList.Add(newList)
                        End If
                    Next

                    Dim completeList As Generic.List(Of Generic.List(Of String)) = BuildOptionList(masterList)
                    PopulateStockTable(completeList, columns)

                End If

            Catch ex As Exception
            Finally
                If cmd.Connection.State = ConnectionState.Open Then
                    cmd.Connection.Close()
                End If
            End Try

        End If
    End Sub

    Private Sub PopulateStockTable(ByVal completeList As Generic.List(Of Generic.List(Of String)), _
                                            ByVal columns As Generic.List(Of String))

        Dim stockRow As DataRow, _
            codes As New ArrayList, _
            code As String = ""


        'Generate a list of all product codes
        '----------------------------------
        For Each list As Generic.List(Of String) In completeList
            code = ""

            'setup the product code
            '----------------------
            If SingleOption Then
                If OptionDefaults.Rows(0)("MATCH_ACTION").ToString.ToUpper = "APPEND" Then
                    code = Me.MasterProduct
                Else
                    code = ""
                End If
            Else
                code = Me.MasterProduct
            End If
            '---------------------

            For i As Integer = 0 To list.Count - 1
                code += list(i)
            Next

            If Not String.IsNullOrEmpty(code) Then codes.Add(code)
        Next
        '----------------------------------------------------------

        'Get Product Info for those products
        '-----------------------------------
        Dim err As Talent.Common.ErrorObj
        Dim products As New DataTable
        Dim prodInfo As New DEProductInfo(TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), codes, _languageCode)
        Dim DBProdInfo As New DBProductInfo(prodInfo)
        DBProdInfo.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
        err = DBProdInfo.AccessDatabase()
        If Not err.HasError Then
            products = DBProdInfo.ResultDataSet.Tables("ProductInformation")
        End If
        '-----------------------------------


        'Get the stock levels for each of the products in the list
        '---------------------------------------------------------
        For Each list As Generic.List(Of String) In completeList
            stockRow = StockLevels.NewRow

            'setup the product code
            '----------------------
            If SingleOption Then
                If OptionDefaults.Rows(0)("MATCH_ACTION").ToString.ToUpper = "APPEND" Then
                    stockRow("ProductCode") = Me.MasterProduct
                Else
                    stockRow("ProductCode") = ""
                End If
            Else
                stockRow("ProductCode") = Me.MasterProduct
            End If
            '---------------------

            For i As Integer = 0 To list.Count - 1
                stockRow("ProductCode") += list(i)
                stockRow(columns(i)) = list(i)
            Next


            'Only add the product if it was returned in the results from the Product Info call
            '---------------------------------------------------------------------------------
            Dim OkToAdd As Boolean = False
            Try
                If products.Rows.Count > 0 Then
                    For Each prd As DataRow In products.Rows
                        If UCase(Utilities.CheckForDBNull_String(prd("PRODUCT_CODE"))) = UCase(stockRow("ProductCode")) Then
                            OkToAdd = True
                            stockRow("Stock") = Talent.eCommerce.Stock.GetStockBalance(stockRow("ProductCode"))
                            stockRow("Weight") = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prd("PRODUCT_WEIGHT"))
                            Exit For
                        End If
                    Next
                Else
                    'ERROR: could not retrieve product info
                    'dr("Stock") = 0
                    OkToAdd = False
                End If
            Catch ex As Exception
                'dr("Stock") = 0
                OkToAdd = False
            End Try

            If OkToAdd Then StockLevels.Rows.Add(stockRow)
            '---------------------------------------------------------------------------------
        Next


        'Now that we have a complete Stock Table, 
        'get the Prices for each Product that is in the stock table
        '----------------------------------------------------------
        GetProductPrices()
        '----------------------------------------------------------

    End Sub

    Private Sub GetProductPrices()
        OptionPrices = New Generic.Dictionary(Of String, Talent.Common.DEWebPrice)
        Dim products As New Generic.Dictionary(Of String, WebPriceProduct)

        If Me.StockLevels.Rows.Count > 0 Then
            'Add all the child products
            For Each stkRow As DataRow In StockLevels.Rows
                Dim pCode As String = Utilities.CheckForDBNull_String(stkRow("ProductCode"))
                If Not products.ContainsKey(pCode) Then
                    products.Add(pCode, New WebPriceProduct(pCode, 0, Me.MasterProduct))
                End If
            Next

            OptionPrices = Talent.eCommerce.Utilities.GetWebPrices(products)
        End If
    End Sub

    Private Sub UpdateFreeItemInBasket(ByVal productCode As String, ByVal size As String, ByVal masterProduct As String)

        Dim basketDetailAdapter As New TalentBasketDatasetTableAdapters.tbl_basket_detailTableAdapter
        If Not (String.IsNullOrWhiteSpace(Request.QueryString("promotionbasketid"))) AndAlso _promotionItemEditMode Then
            Dim basketDetailID As String = Request.QueryString("promotionbasketid").Trim
            Dim tbiPromotionItem As New TalentBasketItem
            If Me.MasterProduct Is Nothing Then
                Try
                    Me.MasterProduct = masterProduct
                Catch ex As Exception
                End Try
            End If
            tbiPromotionItem.MASTER_PRODUCT = Me.MasterProduct
            tbiPromotionItem.Quantity = 1
            tbiPromotionItem.Product = productCode
            tbiPromotionItem.Size = size
            Dim err As Talent.Common.ErrorObj
            Dim products As DataTable
            Dim prodInfo As New DEProductInfo(TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), tbiPromotionItem.Product, _languageCode)
            Dim DBProdInfo As New DBProductInfo(prodInfo)
            DBProdInfo.Settings = Talent.eCommerce.Utilities.GetSettingsObject()
            err = DBProdInfo.AccessDatabase()
            If Not err.HasError Then
                products = DBProdInfo.ResultDataSet.Tables("ProductInformation")
                If products.Rows.Count > 0 Then
                    Dim product As Data.DataRow = products.Rows(0)
                    With tbiPromotionItem
                        Try
                            .ALTERNATE_SKU = Talent.eCommerce.Utilities.CheckForDBNull_String(product("ALTERNATE_SKU"))
                            .PRODUCT_DESCRIPTION1 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_1"))
                            .PRODUCT_DESCRIPTION2 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_2"))
                            .PRODUCT_DESCRIPTION3 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_3"))
                            .PRODUCT_DESCRIPTION4 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_4"))
                            .PRODUCT_DESCRIPTION5 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_5"))
                            .PRODUCT_DESCRIPTION6 = ""
                            .PRODUCT_DESCRIPTION7 = ""
                            .WEIGHT = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(product("PRODUCT_WEIGHT"))
                        Catch ex As Exception
                        End Try
                    End With
                End If
            End If
            basketDetailAdapter.Update_Basket_FreeItem(tbiPromotionItem.Product,
                                                       tbiPromotionItem.PRODUCT_DESCRIPTION1,
                                                       tbiPromotionItem.PRODUCT_DESCRIPTION2,
                                                       tbiPromotionItem.PRODUCT_DESCRIPTION3,
                                                       tbiPromotionItem.PRODUCT_DESCRIPTION4,
                                                       tbiPromotionItem.PRODUCT_DESCRIPTION5,
                                                       tbiPromotionItem.PRODUCT_DESCRIPTION6,
                                                       tbiPromotionItem.PRODUCT_DESCRIPTION7,
                                                       tbiPromotionItem.Size,
                                                       True,
                                                       tbiPromotionItem.WEIGHT,
                                                       basketDetailID)

            DBProdInfo = Nothing
            prodInfo = Nothing
            tbiPromotionItem = Nothing
            Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
        End If
    End Sub

#End Region

#Region "Protected Functions"

    Protected Function BuildOptionList(ByVal masterList As Generic.List(Of Generic.List(Of String))) As Generic.List(Of Generic.List(Of String))

        Dim completeList As New Generic.List(Of Generic.List(Of String))

        Dim addList As New Generic.List(Of String)
        For Each item As String In masterList(0)
            addList = New Generic.List(Of String)
            addList.Add(item)
            completeList = ProcessOptionList(masterList, 1, addList, completeList)
        Next

        Return completeList

    End Function

    Protected Function ProcessOptionList(ByVal masterList As Generic.List(Of Generic.List(Of String)), _
                                            ByVal masterIndex As Integer, _
                                            ByVal addList As Generic.List(Of String), _
                                            ByVal completeList As Generic.List(Of Generic.List(Of String))) As Generic.List(Of Generic.List(Of String))

        If SingleOption Then
            'Add the values to the completed list
            completeList.Add(addList)

        ElseIf masterIndex = masterList.Count - 1 Then
            For Each str As String In masterList(masterIndex)

                'Create a new list and populate with the AddList values 
                'otherwise the last instance of the values overrides the 
                'already added values
                Dim newAdd As New Generic.List(Of String)
                For Each item As String In addList
                    newAdd.Add(item)
                Next

                'Swap in the latest add value
                If newAdd.Count <= masterIndex Then
                    newAdd.Add(str)
                Else
                    newAdd(masterIndex) = str
                End If

                'Add the values to the completed list
                completeList.Add(newAdd)
            Next
        Else
            For Each item As String In masterList(masterIndex)
                If addList.Count <= masterIndex Then
                    addList.Add(item)
                Else
                    addList(masterIndex) = item
                End If
                completeList = ProcessOptionList(masterList, masterIndex + 1, addList, completeList)
            Next
        End If

        Return completeList

    End Function

    Protected Function GetOptionProductCode(ByVal optionType As String, _
                                                ByVal optionLevel As Integer, _
                                                ByVal masterOptionDisplayType As String, _
                                                Optional ByVal repeaterItemIndex As Integer = -1) As String

        Dim selectedOption As String = ""
        Dim newProductCode As String = ""

        Select Case optionLevel
            Case Is = 1
                Select Case UCase(masterOptionDisplayType)
                    Case Is = "DDL"
                        selectedOption = ddlOptionLevel1.SelectedValue
                    Case Is = "TABLE"
                        If repeaterItemIndex > -1 Then
                            selectedOption = CType(rptOptionLevel1.Items(repeaterItemIndex).FindControl("option1Value"), Literal).Text
                        End If
                End Select
            Case Is = 2
                Select Case UCase(masterOptionDisplayType)
                    Case Is = "DDL"
                        selectedOption = ddlOptionLevel2.SelectedValue
                    Case Is = "TABLE"
                        If repeaterItemIndex > -1 Then
                            selectedOption = CType(rptOptionLevel1.Items(repeaterItemIndex).FindControl("option2DDL"), DropDownList).SelectedValue
                        End If
                End Select
            Case Is = 3
                Select Case UCase(masterOptionDisplayType)
                    Case Is = "DDL"
                        selectedOption = ddlOptionLevel3.SelectedValue
                    Case Is = "TABLE"
                        If repeaterItemIndex > -1 Then
                            selectedOption = CType(rptOptionLevel1.Items(repeaterItemIndex).FindControl("option3DDL"), DropDownList).SelectedValue
                        End If
                End Select
        End Select

        For Each dr As DataRow In ProductOptions.Rows
            If Utilities.CheckForDBNull_String(dr("OPTION_TYPE")) = optionType _
                    AndAlso Utilities.CheckForDBNull_String(dr("PRODUCT_CODE")) = selectedOption Then
                newProductCode += Utilities.CheckForDBNull_String(dr("PRODUCT_CODE"))
                Exit For
            End If
        Next

        Return newProductCode
    End Function

#End Region

#Region "Private Functions"

    Private Function GetOptionTypes(ByVal dt As DataTable) As Generic.List(Of String)
        Dim l As New Generic.List(Of String)
        If dt.Rows.Count > 0 Then
            For Each item As DataRow In dt.Rows
                If Not String.IsNullOrEmpty(Utilities.CheckForDBNull_String(item("OPTION_TYPE"))) Then
                    If Not l.Contains(item("OPTION_TYPE")) Then
                        l.Add(item("OPTION_TYPE"))
                    End If
                End If
            Next
        End If
        Return l
    End Function

    Private Function GetOptionsLangs(ByVal da As SqlDataAdapter) As DataTable


        Dim selectSizesLangs As String = " SELECT * " & _
                                           "   FROM tbl_product_option_definitions_lang WITH (NOLOCK)  " & _
                                           " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                           " AND PARTNER = @PARTNER " & _
                                           " AND LANGUAGE_CODE = @LANGUAGE_CODE "
        Dim dt As New DataTable
        'da.Fill(dt)

        '-------------------------
        ' Size Language Variables
        '-------------------------
        With da.SelectCommand.Parameters
            .Clear()
            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
            .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
            .Add("@LANGUAGE_CODE", SqlDbType.NVarChar).Value = Utilities.GetDefaultLanguage
        End With
        da.SelectCommand.CommandText = selectSizesLangs
        da.Fill(dt)
        If Not dt.Rows.Count > 0 Then
            da.SelectCommand.Parameters("@PARTNER").Value = Utilities.GetAllString
            da.Fill(dt)
            If Not dt.Rows.Count > 0 Then
                da.SelectCommand.Parameters("@BUSINESS_UNIT").Value = Utilities.GetAllString
                da.SelectCommand.Parameters("@PARTNER").Value = TalentCache.GetPartner(Profile)
                da.Fill(dt)
                If Not dt.Rows.Count > 0 Then
                    da.SelectCommand.Parameters("@BUSINESS_UNIT").Value = Utilities.GetAllString
                    da.SelectCommand.Parameters("@PARTNER").Value = Utilities.GetAllString
                    da.Fill(dt)
                End If
            End If

        End If

        Return dt
    End Function

    Private Function GetProductOptions(ByVal da As SqlDataAdapter, _
                                        ByVal productCode As String) As DataTable

        Dim selectProductSizes As String = " SELECT o.*, (" & _
                                             "  SELECT ISNULL(t.PERSONALISABLE, 'FALSE') FROM TBL_PRODUCT t WHERE t.PRODUCT_CODE = o.MASTER_PRODUCT" & _
                                             " ) as PERSONALISABLE, " & _
                                             " (" & _
                                             " SELECT count(*) FROM tbl_price_list_detail p WITH (NOLOCK)  " & _
                                             " WHERE p.PRICE_LIST = @PRICE_LIST AND p.PRODUCT = o.PRODUCT_CODE " & _
                                             " ) as COUNT_OF_PRICES " & _
                                             " FROM tbl_product_options o WITH (NOLOCK)  " & _
                                             " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                             " AND PARTNER = @PARTNER " & _
                                             " AND MASTER_PRODUCT = @MASTER_PRODUCT " & _
                                             " ORDER BY OPTION_TYPE "


        'Dim selectProductSizes As String = " SELECT *, (" & _
        '                                     "  SELECT ISNULL(PERSONALISABLE, 'FALSE') FROM TBL_PRODUCT WHERE PRODUCT_CODE = MASTER_PRODUCT" & _
        '                                     " ) as PERSONALISABLE " & _
        '                                     " FROM tbl_product_options WITH (NOLOCK)  " & _
        '                                     " WHERE BUSINESS_UNIT = @BUSINESS_UNIT " & _
        '                                     " AND PARTNER = @PARTNER " & _
        '                                     " AND MASTER_PRODUCT = @MASTER_PRODUCT " & _
        '                                     " ORDER BY OPTION_TYPE "

        da.SelectCommand.CommandText = selectProductSizes
        Dim dt As New DataTable

        '-------------------------
        ' Product Sizes
        '-------------------------
        With da.SelectCommand.Parameters
            .Clear()
            .Add("@PRICE_LIST", SqlDbType.NVarChar).Value = ModuleDefaults.PriceList
            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
            .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(Profile)
            .Add("@MASTER_PRODUCT", SqlDbType.NVarChar).Value = productCode
        End With
        da.Fill(dt)
        If Not dt.Rows.Count > 0 Then
            da.SelectCommand.Parameters("@PARTNER").Value = Utilities.GetAllString
            da.Fill(dt)
            If Not dt.Rows.Count > 0 Then
                da.SelectCommand.Parameters("@BUSINESS_UNIT").Value = Utilities.GetAllString
                da.SelectCommand.Parameters("@PARTNER").Value = TalentCache.GetPartner(Profile)
                da.Fill(dt)
                If Not dt.Rows.Count > 0 Then
                    da.SelectCommand.Parameters("@BUSINESS_UNIT").Value = Utilities.GetAllString
                    da.SelectCommand.Parameters("@PARTNER").Value = Utilities.GetAllString
                    da.Fill(dt)
                End If
            End If
        End If

        Return dt
    End Function

    Private Function GetOptionDefaults(ByVal da As SqlDataAdapter, _
                                            ByVal optionTypes As Generic.List(Of String), _
                                            Optional ByVal orderBy As String = "") As DataTable

        ' Check if in B2B or B2C mode. If in B2B, must use '*ALL' as partner
        ' ''Dim dtDefaultPartners As Data.DataTable
        ' ''Dim appVars As New TalentApplicationVariablesTableAdapters.tbl_authorized_partnersTableAdapter 
        ' ''dtDefaultPartners = appVars.Get_Default_Partners()

        Dim partnerVal As String = TalentCache.GetPartner(Profile)
        With da.SelectCommand.Parameters
            .Clear()
            .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
            ' ''If dtDefaultPartners.Rows.Count = 0 Then
            ' ''    ' B2B
            ' ''    partnerVal = Talent.Common.Utilities.GetAllString
            ' ''Else
            ' ''    ' B2C
            ' ''    partnerVal = TalentCache.GetPartner(Profile)
            ' ''End If
            .Add("@PARTNER", SqlDbType.NVarChar).Value = partnerVal
            .Add("@MASTER_PRODUCT", SqlDbType.NVarChar).Value = Me.MasterProduct
        End With

        Dim selectDefaults As String = " SELECT * " & _
                                          "   FROM tbl_product_option_defaults WITH (NOLOCK)  " & _
                                          "   INNER JOIN tbl_product_option_types_lang WITH (NOLOCK)  " & _
                                          "   ON tbl_product_option_defaults.BUSINESS_UNIT = tbl_product_option_types_lang.BUSINESS_UNIT " & _
                                          "   AND tbl_product_option_defaults.PARTNER = tbl_product_option_types_lang.PARTNER " & _
                                          "   AND tbl_product_option_defaults.OPTION_TYPE = tbl_product_option_types_lang.OPTION_TYPE " & _
                                          " WHERE tbl_product_option_defaults.BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                          " AND tbl_product_option_defaults.PARTNER = @PARTNER " & _
                                          " AND tbl_product_option_defaults.MASTER_PRODUCT = @MASTER_PRODUCT " & _
                                          " AND ("

        Dim count As String = 0
        For Each str As String In optionTypes
            count += 1
            selectDefaults += " tbl_product_option_defaults.OPTION_TYPE = @OPTION_TYPE" & count.ToCharArray & " "
            If Not count = optionTypes.Count Then
                selectDefaults += " OR "
            End If
            da.SelectCommand.Parameters.Add("@OPTION_TYPE" & count.ToString, SqlDbType.NVarChar).Value = str
        Next

        selectDefaults += " )"
        If String.IsNullOrEmpty(orderBy) Then
            selectDefaults += " ORDER BY DISPLAY_SEQUENCE ASC "
        Else
            selectDefaults += orderBy
        End If


        da.SelectCommand.CommandText = selectDefaults
        Dim dt As New DataTable

        da.Fill(dt) ' BU, Partner, Master
        If Not dt.Rows.Count > 0 Then
            da.SelectCommand.Parameters("@PARTNER").Value = Utilities.GetAllString
            da.Fill(dt) ' BU, *ALL, Master
            If Not dt.Rows.Count > 0 Then
                da.SelectCommand.Parameters("@BUSINESS_UNIT").Value = Utilities.GetAllString
                da.Fill(dt) ' *ALL, *ALL, Master
                If Not dt.Rows.Count > 0 Then
                    da.SelectCommand.Parameters("@MASTER_PRODUCT").Value = Utilities.GetAllString
                    da.SelectCommand.Parameters("@PARTNER").Value = partnerVal
                    da.SelectCommand.Parameters("@BUSINESS_UNIT").Value = TalentCache.GetBusinessUnit
                    da.Fill(dt) ' BU, Partner, *ALL
                    If Not dt.Rows.Count > 0 Then
                        da.SelectCommand.Parameters("@PARTNER").Value = Utilities.GetAllString
                        da.Fill(dt) ' BU, *ALL, *ALL
                        If Not dt.Rows.Count > 0 Then
                            da.SelectCommand.Parameters("@BUSINESS_UNIT").Value = Utilities.GetAllString
                            da.Fill(dt) ' *ALL, *ALL, *ALL
                        End If
                    End If
                End If
            End If
        End If
        Return dt
    End Function

    Private Function GetProductCodeFromDDLOption() As String
        Dim productCode As String = ""
        If SingleOption Then

            If UCase(Utilities.CheckForDBNull_String(OptionDefaults.Rows(0)("MATCH_ACTION"))).ToUpper = "APPEND" Then
                'If the match action is append, then add the product code to the start of the tbi.product property
                productCode = Me.MasterProduct
            Else
                'Otherwise, blank the tbi.product property as the whole product code will be taken from the table
                productCode = ""
            End If

            'Append the rest of the product code
            productCode += GetOptionProductCode(Utilities.CheckForDBNull_String(ProductOptions.Rows(0)("OPTION_TYPE")), _
                                                 1, _
                                                 Utilities.CheckForDBNull_String(OptionDefaults.Rows(0)("DISPLAY_TYPE")))
        Else

            're-order the table rows by the APPEND_SEQUENCE value
            Dim drc As DataRow() = OptionDefaults.Select("1=1", "APPEND_SEQUENCE ASC")

            productCode = Me.MasterProduct
            Dim levelCount As Integer = 0

            'Loop through the re-ordered datarows
            For Each dr As DataRow In drc
                levelCount = 0

                'We need to know which display level the row is associated with
                'so loop through each row of the original results table, and take 
                'the row number of the row that matches the ID of the re-ordered 
                'row
                For Each opt As DataRow In OptionDefaults.Rows
                    levelCount += 1
                    If Utilities.CheckForDBNull_BigInt(dr("ID")) = Utilities.CheckForDBNull_BigInt(opt("ID")) Then
                        Exit For
                    End If
                Next

                productCode += GetOptionProductCode(Utilities.CheckForDBNull_String(dr("OPTION_TYPE")), _
                                                 levelCount, _
                                                 Utilities.CheckForDBNull_String(dr("DISPLAY_TYPE")))
            Next

        End If
        Return productCode
    End Function

#End Region

End Class