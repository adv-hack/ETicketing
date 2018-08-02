Imports Talent.Common
Imports System.Data

Partial Class UserControls_QuickOrder
    Inherits ControlBase

#Region "Class Level Fields"

    Private _minRows As Integer = 10
    Private _ucr As New UserControlResource
    Private _languageCode As String = Utilities.GetDefaultLanguage
    Private _conTalent As System.Data.SqlClient.SqlConnection = Nothing
    Private _currentOrderIsHomeDelivery As Boolean = False

#End Region

#Region "Constants"

    Const ViewStateRowsCount As String = "ViewStateRowsCount"

#End Region

#Region "Public Properties"
    Public Property ViewStateRowCounter() As Integer
        Get
            Return ViewState(ViewStateRowsCount)
        End Get
        Set(ByVal value As Integer)
            ViewState(ViewStateRowsCount) = value
        End Set
    End Property

    Public Property ProductID() As String = String.Empty
    Public Property Quantity() As Integer = 0


#End Region

#Region "Protected Methods"

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "quickOrder.ascx"
        End With
        _minRows = Talent.eCommerce.Utilities.CheckForDBNull_Int(_ucr.Content("defaultRowsToAdd", _languageCode, True))
        SetUpQuickOrderFormText()
        If Not Page.IsPostBack Then
            If Talent.eCommerce.Utilities.IsPartnerHomeDeliveryType(Profile) Then
                plhPostCodeOptions.Visible = True
                setPostCodeTextValues()
                plhQuickOrderTable.Visible = False
                plhQuickOrderButtons.Visible = False
            Else
                plhPostCodeOptions.Visible = False
                plhMoreRows.Visible = True
                If Profile.PartnerInfo.Details.PARTNER_TYPE = "BOTH" Then
                    plhQuickOrderHomeDelivery.Visible = True
                    lblQuickOrderHomeDelivery.Text = _ucr.Content("HomeDeliveryOption", _languageCode, True)
                End If
                addRows(_minRows)
                ViewStateRowCounter = _minRows
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        plhDeliveryDate.Visible = False
        lblIntro.Text = _ucr.Content("lblIntro", _languageCode, True)
        ' Check if items in basket, if so add on client click to validate basket 
        If Profile.Basket.BasketItems.Count > 0 Then
            addBtn.Attributes.Add("onclick", "return CheckDeleteBasket();")
        Else
        End If
        If Talent.eCommerce.Utilities.IsPartnerHomeDeliveryType(Profile) Then
            _currentOrderIsHomeDelivery = True
        Else
            If chkQuickOrderHomeDelivery.Checked Then _currentOrderIsHomeDelivery = True
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        plhErrorMessage.Visible = (errorSummary.Text.Length > 0)
        plhSuccessMessage.Visible = (ltlSuccessMessage.Text.Length > 0)
    End Sub

    Protected Sub addBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles addBtn.Click
        If ViewStateRowCounter = 0 Then ViewStateRowCounter = _minRows
        addRows(ViewStateRowCounter)
        addItemsToBasket(ViewStateRowCounter)
    End Sub

    Protected Sub moreRowsBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles moreRowsBtn.Click
        errorSummary.Text = String.Empty
        ' If the ViewStateRowCounter = 0 then the  counter has been reset as the postback has fired, thus reset it to the default minimum rows value
        If ViewStateRowCounter = 0 Then ViewStateRowCounter = _minRows
        Try
            Dim i As Integer = CInt(noOfRowsBox.Text)
            _minRows = i
            ViewStateRowCounter += _minRows 'Inc the number of rows
        Catch ex As Exception
            errorSummary.Text = _ucr.Content("invalidNumberErrorText", _languageCode, True)
        End Try
        addRows(ViewStateRowCounter)
    End Sub

    Protected Sub updateBtn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles updateBtn.Click
        Dim rowsToBind As Integer = _minRows
        If _currentOrderIsHomeDelivery Then
            rowsToBind = Utilities.CheckForDBNull_Int(_ucr.Attribute("NumberOfHomeDeliveryRows"))
        End If
        If ViewStateRowCounter = 0 Then ViewStateRowCounter = rowsToBind
        addRows(ViewStateRowCounter)
        UpdateDetails()
    End Sub

    Protected Sub chkQuickOrderHomeDelivery_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkQuickOrderHomeDelivery.CheckedChanged
        If chkQuickOrderHomeDelivery.Checked Then
            plhPostCodeOptions.Visible = True
            setPostCodeTextValues()
            plhQuickOrderTable.Visible = False
            plhQuickOrderButtons.Visible = False
        Else
            plhQuickOrderTable.Visible = True
            plhQuickOrderButtons.Visible = True
            plhPostCodeOptions.Visible = False
            addRows(_minRows)
            ViewStateRowCounter = _minRows
            plhMoreRows.Visible = True
        End If
    End Sub

    Protected Sub btnPostCodeSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnPostCodeSearch.Click
        errorSummary.Text = String.Empty
        If txtPostCodeSearch.Text.Trim.Length > 0 Then
            Dim postCodeExists As Boolean = True
            Session("QASUnavailable") = Nothing
            Dim moduleDefaults As New Talent.eCommerce.ECommerceModuleDefaults
            Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults()
            If (def.AddressingProvider = "QAS" Or def.AddressingProvider = "QASONDEMAND") Then
                Try
                    postCodeExists = Talent.eCommerce.Utilities.QASPostCodeExists(txtPostCodeSearch.Text.Trim, def.AddressingProvider)
                Catch ex As Exception
                    ' If failed to connect then put error message but allow to continue
                    errorSummary.Text = _ucr.Content("QASUnavailable", _languageCode, True)
                    Session("QASUnavailable") = True
                    postCodeExists = True
                End Try
            End If

            If postCodeExists Then
                If Not Session("QASUnavailable") AndAlso Not CBool(Session("QASUnavailable")) Then
                    ltlSuccessMessage.Text = _ucr.Content("PostCodeIsValid", _languageCode, True)
                End If
                Dim rowsToBind As Integer = Utilities.CheckForDBNull_Int(_ucr.Attribute("NumberOfHomeDeliveryRows"))
                addRows(rowsToBind)
                ViewStateRowCounter = rowsToBind
                plhMoreRows.Visible = False
                Session("QuickOrderPostCode") = txtPostCodeSearch.Text
                plhQuickOrderTable.Visible = True
                plhQuickOrderButtons.Visible = True
            Else
                errorSummary.Text = _ucr.Content("PostCodeNotValid", _languageCode, True)
                Dim rowsToBind As Integer = Utilities.CheckForDBNull_Int(_ucr.Attribute("NumberOfHomeDeliveryRows"))
                addRows(rowsToBind)
                ViewStateRowCounter = rowsToBind
                plhMoreRows.Visible = False
                Session("QuickOrderPostCode") = txtPostCodeSearch.Text
                plhQuickOrderTable.Visible = True
                plhQuickOrderButtons.Visible = True
            End If
        Else
            errorSummary.Text = _ucr.Content("PostCodeNotValid", _languageCode, True)
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub SetUpQuickOrderFormText()
        CType(QuickOrderTable.Rows(0).FindControl("srNoHeader"), Label).Text = _ucr.Content("srNoHeaderText", _languageCode, False)
        CType(QuickOrderTable.Rows(0).FindControl("productCodeHeader"), Label).Text = _ucr.Content("productCodeHeaderText", _languageCode, True)
        CType(QuickOrderTable.Rows(0).FindControl("quantityHeader"), Label).Text = _ucr.Content("quantityHeaderText", _languageCode, True)
        CType(QuickOrderTable.Rows(0).FindControl("removeItemHeader"), Label).Text = _ucr.Content("removeItemHeaderText", _languageCode, True)
        CType(QuickOrderTable.Rows(0).FindControl("unitPriceHeader"), Label).Text = _ucr.Content("unitPriceHeaderText", _languageCode, True)
        CType(QuickOrderTable.Rows(0).FindControl("totalPriceHeader"), Label).Text = _ucr.Content("totalPriceHeaderText", _languageCode, True)
        CType(QuickOrderTable.Rows(0).FindControl("stockLevelHeader"), Label).Text = _ucr.Content("stockLevelHeaderText", _languageCode, True)
        CType(QuickOrderTable.Rows(0).FindControl("productDescriptionHeader"), Label).Text = _ucr.Content("productDescriptionHeaderText", _languageCode, True)
        addBtn.Text = _ucr.Content("addToBasketButtonText", _languageCode, True)
        addBtn.Enabled = False
        updateBtn.Text = _ucr.Content("updateButtonText", _languageCode, True)
        moreRowsBtn.Text = _ucr.Content("addRowsButtonText", _languageCode, True)
        noOfRowsBox.Text = _minRows
    End Sub

    Private Sub setPostCodeTextValues()
        ltlPostCodeSearch.Text = _ucr.Content("PostcodeSearchInstructions", _languageCode, True)
        lblPostCodeSearch.Text = _ucr.Content("PostcodeLabel", _languageCode, True)
        btnPostCodeSearch.Text = _ucr.Content("PostcodeSearchButton", _languageCode, True)
    End Sub

    Private Sub addRows(ByVal rowsToAdd As Integer)
        ' Set the onkeydown event to fire when the enter key is clicked
        Dim buttonID As String = String.Empty
        buttonID = updateBtn.ClientID
        Const onKeyDown As String = "onkeydown"
        Dim EventText As String = "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('" & buttonID & "').click();return false;}} else {return true}; "
        Dim row As TableRow
        Dim lineNo, product, qty, remove, unit, total, stock, desc As TableCell
        Dim prodBox, qtyBox As TextBox
        Dim aceProductSearch As AjaxControlToolkit.AutoCompleteExtender
        Dim removeCheck As CheckBox
        Dim errLbl, lineNoLbl, unitLbl, totalLbl, stockLbl, descLbl As Label

        Dim productsToAdd2 As New Generic.Dictionary(Of String, DeProductLines)
        If Not Session("QuickOrderProducts") Is Nothing Then
            productsToAdd2 = CType(Session("QuickOrderProducts"), Generic.Dictionary(Of String, DeProductLines))
            Session("QuickOrderProducts") = Nothing

        ElseIf Not String.IsNullOrEmpty(ProductID) Then
            Dim dt As DataTable
            dt = TDataObjects.ProductsSettings.TblProduct.GetProductByProductID(productID)


            Dim dep As New DeProductLines


            Dim quant As Decimal

            If Quantity <> 0 Then
                quant = Quantity
            Else
                quant = ModuleDefaults.Min_Add_Quantity
            End If


            dep.Quantity = quant
            dep.ProductDescription = dt.Rows(0)("PRODUCT_SEARCH_KEYWORDS").ToString()
            productsToAdd2.Add(dt.Rows(0)("PRODUCT_CODE").ToString(), dep)
        End If


        For i As Integer = 1 To rowsToAdd
            row = New TableRow
            lineNo = New TableCell
            product = New TableCell
            qty = New TableCell
            remove = New TableCell
            unit = New TableCell
            total = New TableCell
            stock = New TableCell
            desc = New TableCell
            errLbl = New Label
            lineNoLbl = New Label
            prodBox = New TextBox
            aceProductSearch = New AjaxControlToolkit.AutoCompleteExtender
            qtyBox = New TextBox
            removeCheck = New CheckBox
            unitLbl = New Label
            totalLbl = New Label
            stockLbl = New Label
            descLbl = New Label

            ' Check if home delivery and from browsing. If so default in the product:
            Dim count As Integer = 1
            If _currentOrderIsHomeDelivery AndAlso productsToAdd2.Count > 0 Then
                Try
                    For Each strProduct As String In productsToAdd2.Keys
                        If count = i Then
                            prodBox.Text = productsToAdd2.Item(strProduct).ProductDescription
                            qtyBox.Text = productsToAdd2.Item(strProduct).Quantity
                            Exit For
                        End If
                        count += 1
                    Next
                Catch ex As Exception

                End Try
            End If

            errLbl.CssClass = ""
            lineNoLbl.CssClass = ""
            prodBox.CssClass = "input-l"
            qtyBox.CssClass = "input-s"
            removeCheck.CssClass = ""
            unitLbl.CssClass = ""
            totalLbl.CssClass = ""
            stockLbl.CssClass = ""
            descLbl.CssClass = ""
            lineNo.CssClass = "lineNo"
            lineNo.HorizontalAlign = HorizontalAlign.Center
            product.CssClass = "product"
            qty.CssClass = "qty"
            qty.HorizontalAlign = HorizontalAlign.Center
            remove.CssClass = "remove"
            remove.HorizontalAlign = HorizontalAlign.Center
            unit.CssClass = "unit"
            total.CssClass = "total"
            stock.CssClass = "stock"
            desc.CssClass = "desc"

            prodBox.Attributes.Add(onKeyDown, EventText)
            qtyBox.Attributes.Add(onKeyDown, EventText)
            removeCheck.Attributes.Add(onKeyDown, EventText)

            lineNoLbl.ID = "lineNoLbl" & i.ToString
            lineNoLbl.Text = i.ToString
            errLbl.ID = "errLbl" & i.ToString
            errLbl.CssClass = "error"
            errLbl.Visible = False
            prodBox.ID = "prodBox" & i.ToString
            aceProductSearch.ID = "aceProductSearch" & i.ToString
            qtyBox.ID = "qtyBox" & i.ToString
            qtyBox.Width = Web.UI.WebControls.Unit.Pixel(40)
            removeCheck.ID = "removeCheck" & i.ToString
            unitLbl.ID = "unitLbl" & i.ToString
            totalLbl.ID = "totalLbl" & i.ToString
            stockLbl.ID = "stockLbl" & i.ToString
            descLbl.ID = "descLbl" & i.ToString

            If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(_ucr.Attribute("EnableACEProductSearch")) Then
                aceProductSearch.TargetControlID = prodBox.ID
                aceProductSearch.FirstRowSelected = False
                aceProductSearch.ServiceMethod = "GetProductCodeList"
                aceProductSearch.MinimumPrefixLength = 2
                aceProductSearch.CompletionInterval = 100
                aceProductSearch.EnableCaching = True
                aceProductSearch.CompletionSetCount = Talent.eCommerce.Utilities.CheckForDBNull_Int(_ucr.Attribute("ACEProductSearchCompletionSetCount"))
                aceProductSearch.Enabled = True
            Else
                aceProductSearch.TargetControlID = prodBox.ID
                aceProductSearch.Enabled = False
            End If

            ' Attempt to load the values from the viewstate
            For Each key As String In Request.Form.Keys
                Dim name() As String = key.Split("$")
                Dim ID As String = name(name.Length - 1)

                If ID = "prodBox" & i.ToString Then
                    prodBox.Text = Request.Form(key)
                ElseIf ID = "qtyBox" & i.ToString Then
                    qtyBox.Text = Request.Form(key)
                ElseIf ID = "removeCheck" & i.ToString Then
                    If Request.Form(key) = "on" Then
                        removeCheck.Checked = True
                    Else
                        removeCheck.Checked = False
                    End If
                ElseIf ID = "unitLbl" & i.ToString Then
                    unitLbl.Text = Request.Form(key)
                ElseIf ID = "totalLbl" & i.ToString Then
                    totalLbl.Text = Request.Form(key)
                ElseIf ID = "stockLbl" & i.ToString Then
                    stockLbl.Text = Request.Form(key)
                ElseIf ID = "descLbl" & i.ToString Then
                    descLbl.Text = Request.Form(key)
                End If
            Next

            If removeCheck.Checked Then
                prodBox.Text = String.Empty
                qtyBox.Text = String.Empty
                removeCheck.Checked = False
                unitLbl.Text = String.Empty
                totalLbl.Text = String.Empty
                stockLbl.Text = String.Empty
                descLbl.Text = String.Empty
            End If

            ' Add the controls to the cells
            lineNo.Controls.Add(lineNoLbl)
            lineNo.Controls.Add(errLbl)
            product.Controls.Add(prodBox)
            product.Controls.Add(aceProductSearch)
            qty.Controls.Add(qtyBox)
            remove.Controls.Add(removeCheck)
            unit.Controls.Add(unitLbl)
            total.Controls.Add(totalLbl)
            stock.Controls.Add(stockLbl)
            desc.Controls.Add(descLbl)

            ' Add the cells to the row
            row.Cells.Add(lineNo)
            row.Cells.Add(product)
            row.Cells.Add(qty)
            row.Cells.Add(remove)
            row.Cells.Add(unit)
            row.Cells.Add(total)
            row.Cells.Add(stock)
            row.Cells.Add(desc)

            ' Add the row to the table
            QuickOrderTable.Rows.Add(row)
        Next
    End Sub

    Private Sub addItemsToBasket(ByVal rowsToAdd As Integer)
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
        moduleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
        def = moduleDefaults.GetDefaults
        Dim detail As New DataTable
        Dim amendBasket As New Talent.Common.DEAmendBasket
        With amendBasket
            .AddToBasket = True
            .BasketId = Profile.Basket.Basket_Header_ID
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = Profile.PartnerInfo.Details.Partner
            .UserID = Profile.UserName
        End With

        Dim deaItem As Talent.Common.DEAlerts
        Dim prodCode As String = ""
        Dim qty As Decimal = 0
        Dim remove As Boolean = False
        Dim items As New Generic.List(Of TalentBasketItem)
        Dim item As TalentBasketItem

        For i As Integer = 1 To rowsToAdd
            ' Attempt to load the values from the viewstate
            For Each key As String In Request.Form.Keys
                Dim name() As String = key.Split("$")
                Dim ID As String = name(name.Length - 1)
                Select Case ID
                    Case Is = "prodBox" & i.ToString
                        If Not String.IsNullOrEmpty(Request.Form(key)) Then prodCode = Request.Form(key)
                    Case Is = "qtyBox" & i.ToString
                        If Not String.IsNullOrEmpty(Request.Form(key)) Then qty = CDec(Request.Form(key))
                        If (chkQuickOrderHomeDelivery.Checked And qty > 5) OrElse (qty > 25) Then
                            qty = 0
                        End If
                    Case Is = "removeCheck" & i.ToString
                        Select Case Request.Form(key)
                            Case Is = "on"
                                remove = True
                            Case Else
                                remove = False
                        End Select
                End Select
            Next

            If Not String.IsNullOrEmpty(prodCode) AndAlso qty > 0 AndAlso remove = False Then
                item = New TalentBasketItem
                Dim products As Data.DataTable = Talent.eCommerce.Utilities.GetProductInfo(prodCode)
                If products IsNot Nothing Then
                    If products.Rows.Count > 0 Then
                        item.ALTERNATE_SKU = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("ALTERNATE_SKU"))
                        item.PRODUCT_DESCRIPTION1 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_1"))
                        item.PRODUCT_DESCRIPTION2 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_2"))
                        item.PRODUCT_DESCRIPTION3 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_3"))
                        item.PRODUCT_DESCRIPTION4 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_4"))
                        item.PRODUCT_DESCRIPTION5 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_5"))
                        item.GROUP_LEVEL_01 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("GROUP_L01_GROUP"))
                        item.GROUP_LEVEL_02 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("GROUP_L02_GROUP"))
                        item.GROUP_LEVEL_03 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("GROUP_L03_GROUP"))
                        item.GROUP_LEVEL_04 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("GROUP_L04_GROUP"))
                        item.GROUP_LEVEL_05 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("GROUP_L05_GROUP"))
                        item.GROUP_LEVEL_06 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("GROUP_L06_GROUP"))
                        item.GROUP_LEVEL_07 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("GROUP_L07_GROUP"))
                        item.GROUP_LEVEL_08 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("GROUP_L08_GROUP"))
                        item.GROUP_LEVEL_09 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("GROUP_L09_GROUP"))
                        item.GROUP_LEVEL_10 = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("GROUP_L10_GROUP"))
                        item.Product = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_CODE"))
                        If Talent.eCommerce.Utilities.CheckForDBNull_Boolean_DefaultFalse(products.Rows(0)("PRODUCT_OPTION_MASTER")) Then
                            item.MASTER_PRODUCT = Talent.eCommerce.Utilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_CODE"))
                        Else
                            item.MASTER_PRODUCT = TDataObjects.ProductsSettings.TblProductOptions.GetMasterProductCodeByOptionCode(_ucr.BusinessUnit, item.Product)
                        End If
                        item.Xml_Config = String.Empty
                        If _currentOrderIsHomeDelivery Then item.PRODUCT_SUB_TYPE = "HOMEDEL"
                    End If
                End If
                item.Quantity = qty
                items.Add(item)
            End If
            prodCode = String.Empty
            qty = 0
        Next

        For Each itm As TalentBasketItem In items
            deaItem = New Talent.Common.DEAlerts
            deaItem.ProductCode = itm.Product
            deaItem.Quantity = itm.Quantity
            deaItem.ProductSubType = itm.PRODUCT_SUB_TYPE
            deaItem.MasterProduct = itm.MASTER_PRODUCT
            Select Case def.PricingType
                Case 2
                    Dim prices As Data.DataTable = Talent.eCommerce.Utilities.GetChorusPrice(itm.Product, itm.Quantity)
                    If prices.Rows.Count > 0 Then
                        If def.ShowPricesExVAT Then
                            deaItem.Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("NetPrice"))
                        Else
                            deaItem.Price = Talent.eCommerce.Utilities.CheckForDBNull_Decimal(prices.Rows(0)("GrossPrice"))
                        End If
                    End If
                Case Else
                    deaItem.NetPrice = Talent.eCommerce.Utilities.GetWebPrices(itm.Product, itm.Quantity, itm.MASTER_PRODUCT).NET_PRICE
                    deaItem.TaxPrice = Talent.eCommerce.Utilities.GetWebPrices(itm.Product, itm.Quantity, itm.MASTER_PRODUCT).Purchase_Price_Tax
                    deaItem.Price = Talent.eCommerce.Utilities.GetWebPrices(itm.Product, itm.Quantity, itm.MASTER_PRODUCT).DisplayPrice
            End Select
            deaItem.AlternateSKU = itm.ALTERNATE_SKU
            If Not Profile.IsAnonymous And Not Profile.PartnerInfo.Details Is Nothing Then
                deaItem.CostCentre = Profile.PartnerInfo.Details.COST_CENTRE
                deaItem.AccountCode = Talent.eCommerce.Order.GetLastAccountNo(Profile.User.Details.LoginID)
            End If
            deaItem.ProductDescription1 = itm.PRODUCT_DESCRIPTION1
            deaItem.ProductDescription2 = itm.PRODUCT_DESCRIPTION2
            deaItem.ProductDescription3 = itm.PRODUCT_DESCRIPTION3
            deaItem.ProductDescription4 = itm.PRODUCT_DESCRIPTION4
            deaItem.ProductDescription5 = itm.PRODUCT_DESCRIPTION5
            deaItem.GroupL01Group = itm.GROUP_LEVEL_01
            deaItem.GroupL02Group = itm.GROUP_LEVEL_02
            deaItem.GroupL03Group = itm.GROUP_LEVEL_03
            deaItem.GroupL04Group = itm.GROUP_LEVEL_04
            deaItem.GroupL05Group = itm.GROUP_LEVEL_05
            deaItem.GroupL06Group = itm.GROUP_LEVEL_06
            deaItem.GroupL07Group = itm.GROUP_LEVEL_07
            deaItem.GroupL08Group = itm.GROUP_LEVEL_08
            deaItem.GroupL09Group = itm.GROUP_LEVEL_09
            deaItem.GroupL10Group = itm.GROUP_LEVEL_10
            If itm.PRODUCT_DESCRIPTION1.Length > 50 Then
                deaItem.Size = itm.PRODUCT_DESCRIPTION1.Substring(0, 50)
            Else
                deaItem.Size = itm.PRODUCT_DESCRIPTION1
            End If
            deaItem.QuantityAvailable = 0
            amendBasket.CollDEAlerts.Add(deaItem)
        Next

        Dim DBAmend As New DBAmendBasket
        With DBAmend
            .Dep = amendBasket
            Profile.Basket.EmptyBasket()
            .Dep.DeleteFromBasket = False
            .Dep.AddToBasket = True
            .Settings = GetSettingsObject()
            .AccessDatabase()
        End With

        Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
    End Sub

    Private Sub UpdateDetails()
        Dim tempBasket As New TalentBasket
        GetProductDetails(tempBasket)
        AllInStock_BackEndCheck()
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        If tempBasket.BasketItems.Count > 0 AndAlso def.CalculateDeliveryDate Then
            plhDeliveryDate.Visible = True
            ltlProjectedDeliveryDateLabel.Text = _ucr.Content("ProjectedDeliveryDateLabel", _languageCode, True)
            ltlProjectedDeliveryDateValue.Text = Talent.eCommerce.Utilities.GetDeliveryDateQuickOrder(Profile, tempBasket, _currentOrderIsHomeDelivery).ToLongDateString()
        Else
            plhDeliveryDate.Visible = False
        End If
    End Sub

    Private Sub GetProductDetails(ByRef tempBasket As TalentBasket)
        Dim err As New ErrorObj
        Dim productCodes As New ArrayList
        errorSummary.Text = String.Empty
        For i As Integer = 0 To QuickOrderTable.Rows.Count - 1
            If i = 0 OrElse i >= QuickOrderTable.Rows.Count - 1 Then
            Else
                Dim prodBox As TextBox = CType(QuickOrderTable.Rows(i).FindControl("prodBox" & i.ToString), TextBox)
                Dim check As CheckBox = CType(QuickOrderTable.Rows(i).FindControl("removeCheck" & i.ToString), CheckBox)
                If Not String.IsNullOrEmpty(prodBox.Text) And Not check.Checked Then
                    Dim productCode As String = prodBox.Text
                    If Not String.IsNullOrEmpty(_ucr.Attribute("SearchSplitString")) Then
                        Dim keywordArray() As String = prodBox.Text.Split(_ucr.Attribute("SearchSplitString"))
                        productCode = keywordArray(0).Trim()
                        prodBox.Text = productCode
                    End If
                    productCodes.Add(productCode)
                End If
            End If
        Next

        Dim prodInfo As New DEProductInfo(TalentCache.GetBusinessUnit, Profile.PartnerInfo.Details.Partner, productCodes, _languageCode)
        Dim DBProdInfo As New DBProductInfo(prodInfo)
        DBProdInfo.Settings = GetSettingsObject()
        err = DBProdInfo.AccessDatabase()

        Dim disableAdd As Boolean = False
        Dim colProducts As New Collection
        Dim listHello As New DictionaryEntry

        If Not err.HasError Then
            Dim products As Data.DataTable = DBProdInfo.ResultDataSet.Tables("ProductInformation")
            If products.Rows.Count > 0 Then
                Dim productOutOfStock As Boolean = False
                For i As Integer = 0 To QuickOrderTable.Rows.Count - 1
                    If i = 0 OrElse i >= QuickOrderTable.Rows.Count - 1 Then
                    Else
                        Dim prodBox As TextBox = QuickOrderTable.Rows(i).FindControl("prodBox" & i.ToString)
                        Dim errLbl As Label = QuickOrderTable.Rows(i).FindControl("errLbl" & i.ToString)
                        errLbl.Text = String.Empty
                        Dim found As Boolean = False
                        Dim invalidQty As Boolean = False
                        Dim invalidProduct As Boolean = False
                        For Each product As Data.DataRow In products.Rows
                            If Not String.IsNullOrEmpty(prodBox.Text) AndAlso (prodBox.Text = product("PRODUCT_CODE") Or prodBox.Text = product("ALTERNATE_SKU")) Then

                                Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
                                Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
                                If Not def.AllowMasterProductsToBePurchased AndAlso Utilities.CheckForDBNull_Boolean_DefaultTrue(product("PRODUCT_OPTION_MASTER")) Then
                                    errLbl.Text = "*"
                                    errLbl.Visible = True
                                    If String.IsNullOrEmpty(errorSummary.Text) Then errorSummary.Text = _ucr.Content("CantBuyMasterProduct", _languageCode, True)
                                    QuickOrderTable.Rows(i).CssClass = "errorRow"
                                    addBtn.Enabled = False
                                Else
                                    found = True
                                    Dim qtyBox As TextBox = QuickOrderTable.Rows(i).FindControl("qtyBox" & i.ToString)
                                    Dim unitLbl As Label = QuickOrderTable.Rows(i).FindControl("unitLbl" & i.ToString)
                                    Dim totalLbl As Label = QuickOrderTable.Rows(i).FindControl("totalLbl" & i.ToString)
                                    Dim stockLbl As Label = QuickOrderTable.Rows(i).FindControl("stockLbl" & i.ToString)
                                    Dim descLbl As Label = QuickOrderTable.Rows(i).FindControl("descLbl" & i.ToString)

                                    Dim stockTime As String = String.Empty
                                    Dim reStockCode As String = String.Empty
                                    Talent.eCommerce.Stock.GetNoStockDescription(product("PRODUCT_CODE"), stockTime, reStockCode)
                                    If (reStockCode = "X") Then
                                        addBtn.Enabled = False
                                        productOutOfStock = True
                                    End If

                                    Try
                                        Dim d As Decimal = CDec(qtyBox.Text)
                                        Dim storedQty As Decimal

                                        If colProducts.Contains(prodBox.Text) Then
                                            storedQty = colProducts.Item(prodBox.Text) + d
                                            colProducts.Remove(prodBox.Text)
                                            colProducts.Add(storedQty, prodBox.Text)

                                        Else
                                            colProducts.Add(d, prodBox.Text)

                                        End If
                                        ' Check for home del qty > 5 then error
                                        If chkQuickOrderHomeDelivery.Checked AndAlso (d > 5 OrElse storedQty > 5) Then
                                            invalidQty = True
                                            Throw New Exception
                                        ElseIf (storedQty > 25 OrElse d > 25) Then

                                            invalidQty = True
                                            Throw New Exception
                                        End If

                                        Dim webPrice As New Talent.Common.DEWebPrice
                                        webPrice = Talent.eCommerce.Utilities.GetWebPrices(product("PRODUCT_CODE"), d, product("PRODUCT_CODE"))

                                        If webPrice.PriceFound Then
                                            unitLbl.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(webPrice.Purchase_Price_Gross, 0.01), _ucr.BusinessUnit, _ucr.PartnerCode)
                                            totalLbl.Text = TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(webPrice.Purchase_Price_Gross, 0.01), _ucr.BusinessUnit, _ucr.PartnerCode)
                                        Else
                                            If String.IsNullOrEmpty(errorSummary.Text) Then errorSummary.Text = _ucr.Content("productsNotFoundForCustomerErrorText", _languageCode, True)
                                            addBtn.Enabled = False
                                            invalidProduct = True
                                            Throw New Exception

                                        End If

                                        stockLbl.Text = SQLDatabaseStockCheck(product("PRODUCT_CODE"))
                                        descLbl.Text = product("PRODUCT_DESCRIPTION_1")

                                        If Not Profile.PartnerInfo.Details.Enable_Price_View Then
                                            unitLbl.Visible = False
                                            totalLbl.Visible = False
                                        End If

                                        Dim talBasketItem As New TalentBasketItem
                                        talBasketItem.ALTERNATE_SKU = Talent.eCommerce.Utilities.CheckForDBNull_String(product("ALTERNATE_SKU"))
                                        talBasketItem.PRODUCT_DESCRIPTION1 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_1"))
                                        talBasketItem.PRODUCT_DESCRIPTION2 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_2"))
                                        talBasketItem.PRODUCT_DESCRIPTION3 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_3"))
                                        talBasketItem.PRODUCT_DESCRIPTION4 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_4"))
                                        talBasketItem.PRODUCT_DESCRIPTION5 = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_5"))
                                        talBasketItem.Product = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_CODE"))
                                        talBasketItem.MASTER_PRODUCT = Talent.eCommerce.Utilities.CheckForDBNull_String(product("PRODUCT_CODE"))
                                        talBasketItem.Xml_Config = String.Empty
                                        talBasketItem.Quantity = CDec(qtyBox.Text.Trim)
                                        tempBasket.AddItem(talBasketItem)
                                    Catch ex As Exception
                                        addBtn.Enabled = False
                                        disableAdd = True
                                    End Try
                                End If

                            ElseIf String.IsNullOrEmpty(prodBox.Text) Then
                                Dim qtyBox As TextBox = QuickOrderTable.Rows(i).FindControl("qtyBox" & i.ToString)
                                Dim unitLbl As Label = QuickOrderTable.Rows(i).FindControl("unitLbl" & i.ToString)
                                Dim totalLbl As Label = QuickOrderTable.Rows(i).FindControl("totalLbl" & i.ToString)
                                Dim stockLbl As Label = QuickOrderTable.Rows(i).FindControl("stockLbl" & i.ToString)
                                Dim descLbl As Label = QuickOrderTable.Rows(i).FindControl("descLbl" & i.ToString)

                                unitLbl.Text = ""
                                totalLbl.Text = ""
                                stockLbl.Text = ""
                                descLbl.Text = ""
                            End If
                            If found Then Exit For
                        Next

                        If Not String.IsNullOrEmpty(prodBox.Text) AndAlso Not found Then
                            errLbl.Text = "*"
                            errLbl.Visible = True
                            If String.IsNullOrEmpty(errorSummary.Text) Then errorSummary.Text = _ucr.Content("productsNotFoundErrorText", _languageCode, True)
                            QuickOrderTable.Rows(i).CssClass = "errorRow"
                            disableAdd = True
                            addBtn.Enabled = False
                        ElseIf invalidQty Then
                            errLbl.Text = "*"
                            errLbl.Visible = True
                            If String.IsNullOrEmpty(errorSummary.Text) Then errorSummary.Text = _ucr.Content("invalidQuantityErrorText", _languageCode, True)
                            QuickOrderTable.Rows(i).CssClass = "errorRow"
                            disableAdd = True
                            addBtn.Enabled = False
                        Else
                            QuickOrderTable.Rows(i).CssClass = "alternativeRow"
                        End If
                    End If
                Next
                If Not disableAdd AndAlso Not productOutOfStock Then
                    addBtn.Enabled = True
                End If
            Else
                If String.IsNullOrEmpty(errorSummary.Text) Then errorSummary.Text = _ucr.Content("productsNotFoundErrorText", _languageCode, True)
                addBtn.Enabled = False
            End If
        Else
            For i As Integer = 0 To QuickOrderTable.Rows.Count - 1
                If i = 0 OrElse i >= QuickOrderTable.Rows.Count - 1 Then
                Else
                    Dim qtyBox As TextBox = QuickOrderTable.Rows(i).FindControl("qtyBox" & i.ToString)
                    Dim unitLbl As Label = QuickOrderTable.Rows(i).FindControl("unitLbl" & i.ToString)
                    Dim totalLbl As Label = QuickOrderTable.Rows(i).FindControl("totalLbl" & i.ToString)
                    Dim stockLbl As Label = QuickOrderTable.Rows(i).FindControl("stockLbl" & i.ToString)
                    Dim descLbl As Label = QuickOrderTable.Rows(i).FindControl("descLbl" & i.ToString)

                    qtyBox.Text = ""
                    unitLbl.Text = ""
                    totalLbl.Text = ""
                    stockLbl.Text = ""
                    descLbl.Text = ""
                End If
            Next
            addBtn.Enabled = False
        End If
    End Sub

    Private Function AllInStock_BackEndCheck() As Boolean
        errorSummary.Text = String.Empty
        Dim AllInStock As Boolean = True
        If ((ConfigurationManager.AppSettings("Check_Live_Stock") <> "") AndAlso CBool(ConfigurationManager.AppSettings("Check_Live_Stock"))) Then
            Dim dep As New Talent.Common.DePNA
            Dim des As New Talent.Common.DESettings
            Dim tls As New Talent.Common.TalentStock
            Dim err As New Talent.Common.ErrorObj
            Dim dt As Data.DataTable = Nothing
            Dim dRow As Data.DataRow = Nothing
            Dim strResults As New StringBuilder
            Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
            Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
            Dim stockLocation As String = def.StockLocation
            Dim productcodes As String = String.Empty
            Dim locations As String = String.Empty
            Dim alternateSKUs As String = String.Empty

            For i As Integer = 0 To QuickOrderTable.Rows.Count - 1
                If i = 0 OrElse i >= QuickOrderTable.Rows.Count - 1 Then
                Else
                    Dim prodBox As TextBox = QuickOrderTable.Rows(i).FindControl("prodBox" & i.ToString)
                    If Not String.IsNullOrEmpty(prodBox.Text) Then
                        productcodes += prodBox.Text
                        locations += stockLocation
                        If Not String.IsNullOrEmpty(productcodes) Then productcodes += ","
                        If Not String.IsNullOrEmpty(locations) Then locations += ","
                    End If
                End If
            Next

            If productcodes.EndsWith(",") Then productcodes = productcodes.TrimEnd(",")
            If locations.EndsWith(",") Then locations = locations.TrimEnd(",")
            dep.SKU = productcodes
            dep.Warehouse = locations

            With des
                .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("SYSTEM21").ToString
                .Cacheing = False
                .AccountNo3 = Profile.PartnerInfo.Details.Account_No_3
                .AccountNo4 = Profile.PartnerInfo.Details.Account_No_4
                .DestinationDatabase = "SYSTEM21"
                .StoredProcedureGroup = Talent.eCommerce.Utilities.GetStoredProcedureGroup()
                .RetryFailures = def.StockCheckRetry
                .RetryAttempts = def.StockCheckRetryAttempts
                .RetryWaitTime = def.StockCheckRetryWait
                .RetryErrorNumbers = def.StockCheckRetryErrors
                .IgnoreErrors = def.StockCheckIgnoreErrors
            End With
            With tls
                .Settings = des
                .Dep = dep
                err = .GetMutlipleStock
                If Not err.HasError Then dt = .ResultDataSet.Tables(0)
            End With

            If Not err.HasError Then
                ' We could have forced no error by ignoring the error code, therefore the datatable will be empty - just continue.
                If Not dt Is Nothing Then
                    If dt.Rows.Count > 0 Then
                        Dim productcode As String = String.Empty, quantity As String = String.Empty
                        For i As Integer = 0 To QuickOrderTable.Rows.Count - 1
                            If i = 0 OrElse i >= QuickOrderTable.Rows.Count - 1 Then
                            Else
                                Dim prodBox As TextBox = QuickOrderTable.Rows(i).FindControl("prodBox" & i.ToString)
                                Dim qtyBox As TextBox = QuickOrderTable.Rows(i).FindControl("qtyBox" & i.ToString)
                                Dim errLbl As Label = QuickOrderTable.Rows(i).FindControl("errLbl" & i.ToString)
                                Dim stockLbl As Label = QuickOrderTable.Rows(i).FindControl("stockLbl" & i.ToString)
                                productcode = prodBox.Text
                                quantity = qtyBox.Text
                                For Each row As Data.DataRow In dt.Rows
                                    If Utilities.CheckForDBNull_String(row("ProductNumber")).Trim = productcode Then
                                        stockLbl.Text = Utilities.CheckForDBNull_Decimal(row("Quantity")).ToString
                                        If Utilities.CheckForDBNull_Decimal(row("Quantity")) < Utilities.CheckForDBNull_Decimal(quantity) Then
                                            errLbl.Text = "*"
                                            errLbl.Visible = True
                                            AllInStock = False
                                        End If
                                        Exit For 'This product has been found so exit the loop
                                    End If
                                Next
                            End If
                        Next
                    Else
                    End If
                End If
            Else
                AllInStock = False
            End If

        End If
        If Not AllInStock AndAlso String.IsNullOrEmpty(errorSummary.Text) Then
            errorSummary.Text = _ucr.Content("stockErrorText", _languageCode, True)
        End If
        Return AllInStock
    End Function

    Private Function SQLDatabaseStockCheck(ByVal productCode As String) As String
        Dim stockDescription As String = String.Empty
        Dim stockTimeValue As String = String.Empty
        Dim moduleDefaults As Talent.eCommerce.ECommerceModuleDefaults = New Talent.eCommerce.ECommerceModuleDefaults
        Dim def As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults
        stockDescription = Talent.eCommerce.Stock.GetNoStockDescription(productCode, stockTimeValue)
        If String.IsNullOrWhiteSpace(stockDescription) Then stockDescription = _ucr.Content("InStockText", _languageCode, True)
        Return stockDescription
    End Function

#End Region

#Region "Private Functions"

    Private Function GetSettingsObject() As Talent.Common.DESettings
        Dim settings As New Talent.Common.DESettings
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
        settings.DestinationDatabase = "SQL2005"
        Return settings
    End Function

#End Region

End Class