Imports System.Data
Imports Talent.Common
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities

Partial Class UserControls_ProductRelationsGraphical3
    Inherits ControlBase

#Region "Constants"

    Const SQL2005DESTINATIONDATABASE As String = "SQL2005"
    Const STARALLPARTNER As String = "*ALL"
    Const BUYCOMMAND As String = "BUY"

#End Region

#Region "Class Level Fields"

    Private _productCode As String = String.Empty
    Private _ticketingProductType As String = String.Empty
    Private _ticketingProductSubType As String = String.Empty
    Private _businessUnit As String = String.Empty
    Private _partner As String = String.Empty
    Private _pageCode As String = String.Empty
    Private _fromPriceLabel As String = String.Empty
    Private _forwardToBasket As Boolean = False
    Private _ucr As New UserControlResource
    Private _languageCode As String = String.Empty
    Private _productDescriptionsTable As New DataTable
    Private _numberOfListItems As Integer = 0

#End Region

#Region "Public Properties"

    Public Property PagePosition() As Integer
    Public Property TemplateType() As String
    Public Property PriceLabel() As String
    Public Property BuyButtonText() As String
    Public Property PageSize() As Integer
    Public Property ShowImage() As Boolean
    Public Property ShowText() As Boolean
    Public Property ShowPrice() As Boolean
    Public Property ShowBuy() As Boolean
    Public Property ShowLink() As Boolean
    Public Property QuantityBoxMaxLength() As Integer
    Public Property RegularExpressionErrorText() As String
    Public Property DefaultQuantity() As String
    Public Property ListItemClassName() As String

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Visible Then
            plhProductRelationsGraphical.Visible = False
            _pageCode = TEBUtilities.GetCurrentPageName()
            _languageCode = TEBUtilities.GetCurrentLanguage()
            _partner = TalentCache.GetPartner(Profile)
            _businessUnit = TalentCache.GetBusinessUnit()
            With _ucr
                .BusinessUnit = _businessUnit
                .PageCode = _pageCode
                .PartnerCode = _partner
                .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                .KeyCode = "productRelationsGraphical.ascx"
            End With

            populateProductProperties()
            If Not String.IsNullOrWhiteSpace(_productCode) Then
                If Not Page.IsPostBack Then
                    Dim productRelationshipsTable As New DataTable
                    productRelationshipsTable = getAllProductsToBind()
                    If productRelationshipsTable.Rows.Count > 0 Then
                        If populateTextAndAttributes() Then
                            Dim outputTable As DataTable = productRelationshipsTable.Clone()
                            If PageSize > productRelationshipsTable.Rows.Count Then PageSize = productRelationshipsTable.Rows.Count
                            For i As Integer = 0 To PageSize - 1
                                outputTable.ImportRow(productRelationshipsTable.Rows(i))
                            Next
                            Try
                                _numberOfListItems = outputTable.Rows.Count
                                rptProductRelationsGraphical.DataSource = outputTable
                                rptProductRelationsGraphical.DataBind()
                                plhProductRelationsGraphical.Visible = True
                            Catch ex As Exception
                                plhProductRelationsGraphical.Visible = False
                            End Try
                        End If
                    End If
                Else
                    populateTextAndAttributes()
                End If
            End If
        End If
    End Sub

    Protected Sub rptProductRelationsGraphical_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptProductRelationsGraphical.ItemCommand
        If e.CommandName = BUYCOMMAND Then
            Dim productCode As String = e.CommandArgument.ToString()
            Dim availableQuantity As Integer = Stock.GetStockBalance(productCode)
            Dim quantitySelected As TextBox = CType(e.Item.FindControl("txtQuantity"), TextBox)
            Dim quantity As Integer = TEBUtilities.CheckForDBNull_Int(quantitySelected.Text)
            If quantity > availableQuantity And ModuleDefaults.FrontEndStockRequiredToAddToBasket Then
                lblError.Text = _ucr.Content("stockError", _languageCode, True)
            Else
                If quantity >= ModuleDefaults.Min_Add_Quantity Then
                    Dim tbi As New TalentBasketItem
                    With tbi
                        .Product = productCode
                        .Quantity = quantity
                        Dim products As Data.DataTable = Talent.eCommerce.Utilities.GetProductInfo(productCode)
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
                                Dim prices As DataTable = Talent.eCommerce.Utilities.GetChorusPrice(productCode, quantity)
                                If prices.Rows.Count > 0 Then
                                    .Gross_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("GrossPrice"))
                                    .Net_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("NetPrice"))
                                    .Tax_Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("TaxPrice"))
                                End If
                            Case Else
                                Dim deWp As DEWebPrice = Talent.eCommerce.Utilities.GetWebPrices(productCode, quantity, productCode)
                                .Gross_Price = deWp.Purchase_Price_Gross
                                .Net_Price = deWp.Purchase_Price_Net
                                .Tax_Price = deWp.Purchase_Price_Tax
                        End Select
                        If Not Profile.IsAnonymous And Not Profile.PartnerInfo.Details Is Nothing Then
                            .Cost_Centre = Profile.PartnerInfo.Details.COST_CENTRE
                            .Account_Code = Order.GetLastAccountNo(Profile.User.Details.LoginID)
                        End If
                    End With
                    Profile.Basket.AddItem(tbi)
                    If _forwardToBasket OrElse _pageCode.Contains("basket.aspx") Then
                        Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
                    Else
                        Response.Redirect(Request.Url.ToString)
                    End If
                Else
                    lblError.Text = String.Format(_ucr.Content("MinQuantityNotMetError", _languageCode, True), ModuleDefaults.Min_Add_Quantity.ToString("0"))
                End If
            End If
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender
        plhErrorMessage.Visible = (lblError.Text.Length > 0)
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Populate the product code, type and sub type properties
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub populateProductProperties()
        If Request.QueryString("product") IsNot Nothing Then
            _productCode = Request.QueryString("product")
        Else
            _productCode = getLastItemAddedToBasket()
        End If
        For Each item As TalentBasketItem In Profile.Basket.BasketItems
            If item.Product = _productCode AndAlso Not String.IsNullOrWhiteSpace(item.PRODUCT_TYPE) Then
                Dim _talentErrObj As New ErrorObj
                Dim _talentProduct As New TalentProduct
                Dim _deSettings As DESettings = TEBUtilities.GetSettingsObject()
                _deSettings.Cacheing = True
                _deSettings.CacheTimeMinutes = TEBUtilities.CheckForDBNull_Int(_ucr.Attribute("CacheTimeMinutes"))
                _talentProduct.Settings() = _deSettings
                _talentProduct.De.ProductCode = _productCode
                _talentProduct.De.Src = "W"
                _talentProduct.De.PriceCode = item.PRICE_CODE
                _talentErrObj = _talentProduct.ProductDetails
                Dim productDataSet As DataSet = _talentProduct.ResultDataSet
                If Not _talentErrObj.HasError Then
                    If productDataSet IsNot Nothing AndAlso productDataSet.Tables.Count = 3 AndAlso productDataSet.Tables(2).Rows.Count > 0 Then
                        _ticketingProductType = TEBUtilities.CheckForDBNull_String(productDataSet.Tables(2).Rows(0)("ProductType"))
                    End If
                End If
                _ticketingProductSubType = item.PRODUCT_SUB_TYPE.Trim
            End If
        Next
    End Sub

#End Region

#Region "Private Functions"

    ''' <summary>
    ''' Populate the text and attribute properties
    ''' </summary>
    ''' <returns>True if properties successfully populated</returns>
    ''' <remarks></remarks>
    Private Function populateTextAndAttributes() As Boolean
        Dim textAndAttributesPopulated As Boolean = False
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        Dim productRelationsTextTable As New DataTable
        Dim productRelationsAttributesTable As New DataTable
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = _businessUnit
        tDataObjects.Settings = settings

        productRelationsTextTable = tDataObjects.ProductsSettings.TblProductRelationsTextLang.GetTextByBUPartnerPageCodeQualifier(_businessUnit, _partner, _pageCode, 1)
        If productRelationsTextTable.Rows.Count > 0 Then
            For Each dr As DataRow In productRelationsTextTable.Rows
                Select Case dr("TEXT_CODE")
                    Case Is = "HEADER_TEXT"
                        lblHeaderText.Text = TEBUtilities.CheckForDBNull_String(dr("TEXT_VALUE"))
                    Case Is = "PRICE_LABEL"
                        PriceLabel = dr("TEXT_VALUE")
                    Case Is = "BUY_BUTTON_TEXT"
                        BuyButtonText = dr("TEXT_VALUE")
                    Case Is = "FROM_PRICE_LABEL"
                        _fromPriceLabel = dr("TEXT_VALUE")
                End Select
            Next
            textAndAttributesPopulated = True
            If ModuleDefaults.ShowFromPrices Then PriceLabel = _fromPriceLabel
        End If

        productRelationsAttributesTable = tDataObjects.ProductsSettings.TblProductRelationsAttributeValues.GetProductRelationsAttributes(_businessUnit, _partner, _pageCode, 1, PagePosition, TemplateType)
        If productRelationsAttributesTable.Rows.Count > 0 Then
            For Each dr As DataRow In productRelationsAttributesTable.Rows
                Select Case dr("ATTRIBUTE_CODE")
                    Case Is = "PAGE_SIZE"
                        PageSize = TEBUtilities.CheckForDBNull_Int(dr("ATTRIBUTE_VALUE"))
                    Case Is = "SHOW_IMAGE"
                        ShowImage = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(dr("ATTRIBUTE_VALUE"))
                    Case Is = "SHOW_TEXT"
                        ShowText = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(dr("ATTRIBUTE_VALUE"))
                    Case Is = "SHOW_PRICE"
                        ShowPrice = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(dr("ATTRIBUTE_VALUE"))
                    Case Is = "SHOW_BUY"
                        ShowBuy = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(dr("ATTRIBUTE_VALUE"))
                    Case Is = "SHOW_LINK"
                        ShowLink = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(dr("ATTRIBUTE_VALUE"))
                    Case Is = "QUANTITY_BOX_MAX_LENGTH"
                        QuantityBoxMaxLength = TEBUtilities.CheckForDBNull_Int(dr("ATTRIBUTE_VALUE"))
                        DefaultQuantity = ModuleDefaults.Min_Add_Quantity
                    Case Is = "FORWARD_TO_BASKET"
                        _forwardToBasket = TEBUtilities.CheckForDBNull_Boolean_DefaultTrue(dr("ATTRIBUTE_VALUE"))
                End Select
            Next
            If ShowBuy Then
                RegularExpressionErrorText = _ucr.Content("QuantityValidatorErrorText", _languageCode, True)
            End If
        Else
            textAndAttributesPopulated = False
        End If
        Return textAndAttributesPopulated
    End Function

    ''' <summary>
    ''' Return all the combined relationships - both retail and ticketing
    ''' </summary>
    ''' <returns>A datatable of all ticketing and retail data</returns>
    ''' <remarks></remarks>
    Private Function getAllProductsToBind() As DataTable
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        Dim productRelationsTable As New DataTable
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = _businessUnit
        tDataObjects.Settings = settings

        productRelationsTable = tDataObjects.ProductsSettings.TblProductRelations.GetType1ProductRelationsByBUPartnerAndProductInfo(_businessUnit, _partner, _productCode, _ticketingProductType, _ticketingProductSubType)
        Dim editedProductRelationsTable As DataTable = removeDuplicates(productRelationsTable)
        Dim newProductRelationsTable As DataTable = removeProductsAlreadyInBasket(editedProductRelationsTable)
        If newProductRelationsTable.Rows.Count > 0 Then
            Dim hasRelatedRetailProducts As Boolean = False
            Dim hasRelatedTicketingProducts As Boolean = False
            For Each row As DataRow In newProductRelationsTable.Rows
                If row("RELATED_PRODUCT").ToString().Length > 0 AndAlso _
                    row("RELATED_TICKETING_PRODUCT_TYPE").ToString().Length = 0 AndAlso _
                    row("RELATED_TICKETING_PRODUCT_SUB_TYPE").ToString().Length = 0 Then
                    hasRelatedRetailProducts = True
                End If
                If row("RELATED_TICKETING_PRODUCT_TYPE").ToString().Length > 0 OrElse _
                    row("RELATED_TICKETING_PRODUCT_SUB_TYPE").ToString().Length > 0 Then
                    hasRelatedTicketingProducts = True
                End If
            Next
            Dim retailTable As New DataTable
            Dim ticketingTable As New DataTable
            If hasRelatedRetailProducts Then
                retailTable = getRetailData()
            End If
            If hasRelatedTicketingProducts Then
                ticketingTable = getTicketingData()
            End If
            _productDescriptionsTable = combineTicketingAndRetailTables(retailTable, ticketingTable)
            Return removeProductsNoLongerAvailable(newProductRelationsTable, ticketingTable, retailTable)
        Else
            Return New DataTable
        End If
    End Function

    ''' <summary>
    ''' Get the retail data (product codes and descriptions)
    ''' </summary>
    ''' <returns>A data table of product codes and descriptions</returns>
    ''' <remarks></remarks>
    Private Function getRetailData() As DataTable
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = _businessUnit
        tDataObjects.Settings = settings
        Dim productRelationsWithDescriptionsTable As DataTable = tDataObjects.ProductsSettings.TblProductRelations.GetProductDescriptionsByBUAndPartner(_businessUnit, _partner)
        tDataObjects = Nothing
        Return productRelationsWithDescriptionsTable
    End Function

    ''' <summary>
    ''' Get the ticketing data (product codes and descriptions)
    ''' </summary>
    ''' <returns>A data table of product codes and descriptions</returns>
    ''' <remarks></remarks>
    Private Function getTicketingData() As DataTable
        Dim tDataObjects = New TalentDataObjects()
        Dim settings As DESettings = New DESettings()
        Dim feeds As New TalentFeeds
        Dim deFeeds As New DEFeeds
        Dim err As New ErrorObj
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = SQL2005DESTINATIONDATABASE
        settings.BusinessUnit = _businessUnit
        tDataObjects.Settings = settings
        settings.OriginatingSource = "W"
        settings.StoredProcedureGroup = ModuleDefaults.StoredProcedureGroup
        deFeeds.Corporate_Stadium = ModuleDefaults.CorporateStadium
        deFeeds.Ticketing_Stadium = ModuleDefaults.TicketingStadium
        deFeeds.Product_Type = STARALLPARTNER
        feeds.FeedsEntity = deFeeds
        feeds.Settings = settings
        tDataObjects = Nothing
        err = feeds.GetXMLFeed
        If err.HasError Then
            Return New DataTable
        Else
            If feeds.ProductsDataView IsNot Nothing Then
                Return feeds.ProductsDataView.Table
            Else
                Return New DataTable
            End If
        End If
    End Function

    ''' <summary>
    ''' Combine the ticketing and retail data tables together
    ''' </summary>
    ''' <param name="retailTable">retail data table</param>
    ''' <param name="ticketingTable">ticketing data table</param>
    ''' <returns>combined data table</returns>
    ''' <remarks></remarks>
    Private Function combineTicketingAndRetailTables(ByVal retailTable As DataTable, ByVal ticketingTable As DataTable) As DataTable
        Dim allProductsTable As New DataTable
        Dim dRow As DataRow = Nothing
        Dim dePrice As DEWebPrice = Nothing
        Dim price As Decimal = 0
        With allProductsTable.Columns
            .Add("PRODUCT_CODE", GetType(String))
            .Add("PRODUCT_DESCRIPTION_1", GetType(String))
            .Add("PRODUCT_DESCRIPTION_2", GetType(String))
            .Add("PRICE", GetType(Decimal))
            .Add("PRODUCT_TYPE", GetType(String))
            .Add("PRODUCT_SUB_TYPE", GetType(String))
            .Add("OPPOSITION_CODE", GetType(String))
        End With
        For Each row As DataRow In retailTable.Rows
            dRow = allProductsTable.NewRow
            dRow("PRODUCT_CODE") = row("PRODUCT_CODE").ToString()
            dRow("PRODUCT_DESCRIPTION_1") = row("PRODUCT_DESCRIPTION_1").ToString()
            dRow("PRODUCT_DESCRIPTION_2") = row("PRODUCT_DESCRIPTION_2").ToString()
            dePrice = TEBUtilities.GetWebPrices(row("PRODUCT_CODE").ToString(), 1, row("PRODUCT_CODE").ToString())
            If ModuleDefaults.ShowFromPrices Then
                If ModuleDefaults.ShowPricesExVAT Then
                    price = dePrice.From_Price_Net
                Else
                    price = dePrice.From_Price_Gross
                End If
            Else
                If ModuleDefaults.ShowPricesExVAT Then
                    price = dePrice.Purchase_Price_Net
                Else
                    price = dePrice.Purchase_Price_Gross
                End If
            End If
            dRow("PRICE") = price
            dRow("PRODUCT_TYPE") = String.Empty
            dRow("PRODUCT_SUB_TYPE") = String.Empty
            dRow("OPPOSITION_CODE") = String.Empty
            allProductsTable.Rows.Add(dRow)
        Next
        For Each row As DataRow In ticketingTable.Rows
            dRow = allProductsTable.NewRow
            dRow("PRODUCT_CODE") = row("ProductCode").ToString()
            dRow("PRODUCT_DESCRIPTION_1") = row("ProductDescription").ToString()
            dRow("PRODUCT_DESCRIPTION_2") = row("ProductCompetitionDesc").ToString()
            dRow("PRICE") = 0
            dRow("PRODUCT_TYPE") = row("ProductType").ToString()
            dRow("PRODUCT_SUB_TYPE") = row("ProductSubType").ToString()
            dRow("OPPOSITION_CODE") = row("ProductOppositionCode").ToString()
            allProductsTable.Rows.Add(dRow)
        Next
        Return allProductsTable
    End Function

    ''' <summary>
    ''' Get the full ticketing URL by the product type, sub type and code
    ''' </summary>
    ''' <param name="productCode">The given product code</param>
    ''' <param name="productType">The given product type</param>
    ''' <param name="productSubType">The given product sub type</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function getTicketingUrlByProductCode(ByVal productCode As String, ByVal productType As String, ByVal productSubType As String) As String
        Dim ticketingUrl As New StringBuilder
        Select Case productType
            Case Is = "H"
                ticketingUrl.Append("~/PagesPublic/ProductBrowse/ProductHome.aspx?ProductType=H")
            Case Is = "A"
                ticketingUrl.Append("~/PagesPublic/ProductBrowse/ProductAway.aspx?ProductType=A")
            Case Is = "S"
                ticketingUrl.Append("~/PagesPublic/ProductBrowse/ProductSeason.aspx?ProductType=S")
            Case Is = "C"
                ticketingUrl.Append("~/PagesPublic/ProductBrowse/ProductMembership.aspx?ProductType=C")
            Case Is = "E"
                ticketingUrl.Append("~/PagesPublic/ProductBrowse/ProductEvent.aspx?ProductType=E")
            Case Is = "T"
                ticketingUrl.Append("~/PagesPublic/ProductBrowse/ProductTravel.aspx?ProductType=T")
        End Select
        ticketingUrl.Append("&IsSingleProduct=TRUE&ProductCode=")
        ticketingUrl.Append(productCode)
        If Not String.IsNullOrWhiteSpace(productSubType) Then
            ticketingUrl.Append("&ProductSubType=")
            ticketingUrl.Append(productSubType)
        End If
        Return ticketingUrl.ToString()
    End Function

    ''' <summary>
    ''' Get the product opposition code based on the given product code
    ''' </summary>
    ''' <param name="productCode">The given product code</param>
    ''' <returns>The opposition code</returns>
    ''' <remarks></remarks>
    Private Function getOppositionCode(ByVal productCode As String) As String
        Dim oppositionCode As String = String.Empty
        For Each row As DataRow In _productDescriptionsTable.Rows
            If row("PRODUCT_CODE").ToString() = productCode Then
                oppositionCode = row("OPPOSITION_CODE")
                Exit For
            End If
        Next
        Return oppositionCode
    End Function

    ''' <summary>
    ''' Return the product code of the last item added to the basket
    ''' </summary>
    ''' <returns>Product code as a string</returns>
    ''' <remarks></remarks>
    Private Function getLastItemAddedToBasket() As String
        Dim productCode As String = String.Empty
        Dim productList As New Collection
        If Session("LastItemAddedToBasket") Is Nothing Then
            For Each item As TalentBasketItem In Profile.Basket.BasketItems
                If Not TEBUtilities.IsTicketingFee("TICKETING", item.Product, item.FEE_CATEGORY) Then
                    If String.IsNullOrWhiteSpace(item.MASTER_PRODUCT) Then
                        productCode = item.Product
                    Else
                        productCode = item.MASTER_PRODUCT
                    End If
                    productList.Add(productCode)
                    Session("LastItemAddedToBasket") = productList
                    Exit For
                End If
            Next
        Else
            If Profile.Basket.BasketItems.Count > 0 Then
                productList = removeSessionItemsNoLongerInBasket(Session("LastItemAddedToBasket"))
                For Each item As TalentBasketItem In Profile.Basket.BasketItems
                    If Not TEBUtilities.IsTicketingFee("TICKETING", item.Product, item.FEE_CATEGORY) Then
                        If String.IsNullOrWhiteSpace(item.MASTER_PRODUCT) Then
                            productCode = item.Product
                        Else
                            productCode = item.MASTER_PRODUCT
                        End If
                        Dim productAlreadyAdded As Boolean = False
                        For Each collectionItem As String In productList
                            If collectionItem = productCode Then
                                productAlreadyAdded = True
                                Exit For
                            End If
                        Next
                        If Not productAlreadyAdded Then productList.Add(productCode)
                        Session("LastItemAddedToBasket") = productList
                    End If
                Next
            Else
                Session("LastItemAddedToBasket") = Nothing
            End If
        End If
        If productList.Count > 0 Then productCode = productList(productList.Count)
        Return productCode
    End Function

    ''' <summary>
    ''' Rebind the collection based on what is in the current basket as it may have changed
    ''' </summary>
    ''' <param name="productList">The current product list session object</param>
    ''' <returns>an amended collection of products</returns>
    ''' <remarks></remarks>
    Private Function removeSessionItemsNoLongerInBasket(ByVal productList As Collection) As Collection
        Dim newProductList As New Collection
        For Each collectionItem1 As String In productList
            For Each item As TalentBasketItem In Profile.Basket.BasketItems
                If Not TEBUtilities.IsTicketingFee("TICKETING", item.Product, item.FEE_CATEGORY) Then
                    Dim productAlreadyAdded As Boolean = False
                    For Each collectionItem2 As String In productList
                        If collectionItem2 = item.Product Then
                            productAlreadyAdded = True
                            Exit For
                        End If
                    Next
                    If Not productAlreadyAdded Then
                        newProductList.Add(collectionItem1)
                        Exit For
                    End If
                End If
            Next
        Next
        Return newProductList
    End Function

    ''' <summary>
    ''' Check to see if this product is already in the basket, remove records that are already in the basket
    ''' </summary>
    ''' <param name="productRelationsTable">The current product relations</param>
    ''' <returns>A data table of edited results</returns>
    ''' <remarks></remarks>
    Private Function removeProductsAlreadyInBasket(ByVal productRelationsTable As DataTable) As DataTable
        For Each item As TalentBasketItem In Profile.Basket.BasketItems
            For Each row As DataRow In productRelationsTable.Rows
                If item.Product = row("RELATED_PRODUCT") Then
                    row.Delete()
                End If
            Next
            productRelationsTable.AcceptChanges()
        Next
        Return productRelationsTable
    End Function

    ''' <summary>
    ''' Remove any product codes that are linked more than once. Product can be setup in this way but shouldn't be shown more than once on the page
    ''' </summary>
    ''' <param name="productRelationsTable">The current product relations</param>
    ''' <returns>A data table of edited results</returns>
    ''' <remarks></remarks>
    Private Function removeDuplicates(ByVal productRelationsTable As DataTable) As DataTable
        Dim listOfProducts As New Collection
        For Each row As DataRow In productRelationsTable.Rows
            Dim productAlreadyListed As Boolean = False
            For Each item As String In listOfProducts
                If item = row("RELATED_PRODUCT").ToString() Then productAlreadyListed = True
            Next
            If productAlreadyListed Then
                row.Delete()
            Else
                listOfProducts.Add(row("RELATED_PRODUCT").ToString())
            End If
        Next
        productRelationsTable.AcceptChanges()
        Return productRelationsTable
    End Function

    ''' <summary>
    ''' Remove any product relations to products that are no longer on sale
    ''' </summary>
    ''' <param name="productRelationsTable">The product relations table</param>
    ''' <param name="ticketingTable">The current available ticketing product table</param>
    ''' <param name="retailTable">The current available retail product table</param>
    ''' <returns>A modified product relations table</returns>
    ''' <remarks></remarks>
    Private Function removeProductsNoLongerAvailable(ByVal productRelationsTable As DataTable, ByRef ticketingTable As DataTable, ByRef retailTable As DataTable) As DataTable
        For Each row As DataRow In productRelationsTable.Rows
            Dim productFound As Boolean = False
            For Each ticketingRow As DataRow In ticketingTable.Rows
                If row("RELATED_PRODUCT") = ticketingRow("ProductCode") Then
                    productFound = True
                    Exit For
                End If
            Next
            If Not productFound Then
                For Each retailRow As DataRow In retailTable.Rows
                    If row("RELATED_PRODUCT") = retailRow("PRODUCT_CODE") Then
                        productFound = True
                        Exit For
                    End If
                Next
            End If
            If Not productFound Then row.Delete()
        Next
        productRelationsTable.AcceptChanges()
        Return productRelationsTable
    End Function

#End Region

#Region "Public Functions"

    Public Function GetImagePath(ByVal productCode As String, ByVal productType As String, ByVal productSubType As String) As String
        Dim imagePathString As String = String.Empty
        If String.IsNullOrWhiteSpace(productType) AndAlso String.IsNullOrWhiteSpace(productSubType) Then
            imagePathString = ImagePath.getImagePath("PRODASSOC", productCode, _businessUnit, _partner)
        Else
            imagePathString = ImagePath.getImagePath("PRODTICKETING", getOppositionCode(productCode), _businessUnit, _partner)
        End If
        Return imagePathString
    End Function

    Public Function GetProductPath(ByVal productCode As String, ByVal productType As String, ByVal productSubType As String) As String
        Dim productPath As String = String.Empty
        If String.IsNullOrEmpty(ModuleDefaults.EPurseTopUpProductCode) Then
            If String.IsNullOrWhiteSpace(productType) AndAlso String.IsNullOrWhiteSpace(productSubType) Then
                productPath = "~/PagesPublic/ProductBrowse/product.aspx?product=" & productCode
            Else
                productPath = getTicketingUrlByProductCode(productCode, productType, productSubType)
            End If
        Else
            productPath = "~/PagesLogin/Smartcard/EPurse.aspx"
        End If
        Return ResolveUrl(productPath)
    End Function

    Public Function DisplayRetailOptions(ByVal productType As String, ByVal productSubType As String) As Boolean
        Dim isRetail As Boolean = False
        If String.IsNullOrWhiteSpace(productType) AndAlso String.IsNullOrWhiteSpace(productSubType) Then isRetail = True
        Return isRetail
    End Function

    Public Function GetProductDescription1ByProductCode(ByVal productCode As String) As String
        Dim productDescription As String = String.Empty
        For Each row As DataRow In _productDescriptionsTable.Rows
            If row("PRODUCT_CODE").ToString() = productCode Then
                productDescription = row("PRODUCT_DESCRIPTION_1").ToString()
                Exit For
            End If
        Next
        Return productDescription
    End Function

    Public Function GetProductDescription2ByProductCode(ByVal productCode As String) As String
        Dim productDescription As String = String.Empty
        For Each row As DataRow In _productDescriptionsTable.Rows
            If row("PRODUCT_CODE").ToString() = productCode Then
                productDescription = row("PRODUCT_DESCRIPTION_2").ToString()
                Exit For
            End If
        Next
        Return productDescription
    End Function

    Public Function GetProductPrice(ByVal productCode As String) As String
        Dim price As String = String.Empty
        For Each row As DataRow In _productDescriptionsTable.Rows
            If row("PRODUCT_CODE").ToString() = productCode Then
                price = TDataObjects.PaymentSettings.FormatCurrency(row("PRICE"), _ucr.BusinessUnit, _ucr.PartnerCode)
                Exit For
            End If
        Next
        Return price
    End Function

    Public Function GetClassName(ByVal ticketingProductType As String, ByVal itemIndex As Integer) As String
        Dim className As String = String.Empty
        If ticketingProductType.Length > 0 Then
            className = " class=""ticketing-product"
        Else
            className = " class=""retail-product"
        End If
        If itemIndex = 0 Then
            className = className & " first"""
        Else
            If itemIndex = _numberOfListItems - 1 Then
                className = className & " last"""
            Else
                className = className & """"
            End If
        End If
        Return className
    End Function

#End Region

End Class