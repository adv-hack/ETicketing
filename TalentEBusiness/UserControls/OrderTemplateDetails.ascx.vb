Imports Talent.Common
Imports TCUtilites = Talent.Common.Utilities
Imports Talent.eCommerce
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports System.Data
Imports System.Data.SqlClient

Partial Class UserControls_OrderTemplateDetails
    Inherits ControlBase

#Region "Class Level Fields"

    Private _conTalent As SqlConnection = Nothing
    Private _languageCode As String = TCUtilites.GetDefaultLanguage
    Private _pds As New PagedDataSource
    Private _ucr As New UserControlResource
    Private _cmdSelect As SqlCommand = Nothing
    Private _dtrProduct As SqlDataReader = Nothing
    Private _completeProductsList As ProductListGen
    Private _usage As String

#End Region

#Region "Public Properties"

    Public ReadOnly Property QueryString_HeaderID() As String
        Get
            Return Request("hid")
        End Get
    End Property

    Public Property Usage() As String
        Get
            Return _usage
        End Get
        Set(ByVal value As String)
            _usage = value
        End Set
    End Property

    Public Property ShowOptions() As Boolean

#End Region

#Region "Protected Methods"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _ucr
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .PageCode = TEBUtilities.GetCurrentPageName()
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            Select Case UCase(Usage)
                Case Is = "TEMPLATES"
                    .KeyCode = "OrderTemplatesDetail.ascx"
                Case Is = "SAVEDORDERS"
                    .KeyCode = "SavedOrdersDetail.ascx"
            End Select
            btnKeepItems.Text = .Content("KeepBasketButtonText", _languageCode, True)
            btnReplaceItems.Text = .Content("ReplaceBasketButtonText", _languageCode, True)
            lblReplaceBasketQuestion.Text = .Content("EmptyTheBasketContentsQuestion", _languageCode, True)
            ShowAllButton.Text = .Content("ShowAllButtonText", _languageCode, True)
        End With

        Dim defaultheader As String = Nothing
        Dim templates As New OrderTemplatesDataSetTableAdapters.tbl_order_template_headerTableAdapter
        Dim dv As DataView
        If ModuleDefaults.OrderTemplatesPerPartner Then
            dv = templates.Get_All_By_Partner(TalentCache.GetBusinessUnit, Profile.PartnerInfo.Details.Partner).DefaultView
        Else
            dv = templates.Get_All_By_LoginID(TalentCache.GetBusinessUnit, Profile.PartnerInfo.Details.Partner, Profile.UserName).DefaultView
        End If

        For Each row As System.Data.DataRow In dv.Table.Rows
            If Boolean.Parse(row("IS_DEFAULT")) Then
                defaultheader = row("TEMPLATE_HEADER_ID")
            End If
        Next

        If UCase(Usage) = "TEMPLATES" Then
            If (Request.QueryString("source") Is Nothing OrElse Not Request.QueryString("source").Equals("edit")) Then
                If Not defaultheader Is Nothing Then
                    Response.Redirect("EditTemplate.aspx?source=edit&hid=" + defaultheader)
                Else
                    Response.Redirect("ViewTemplates.aspx")
                End If
            End If
        End If

        If Not Page.IsPostBack Then
            BindDetailView()
            DisplayPagerNav(_pds)
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        Select Case ModuleDefaults.OrderTemplateLayoutType
            Case "2"
                Repeater1PlaceHolder.Visible = False
                Repeater2PlaceHolder.Visible = True
            Case Else
                Repeater1PlaceHolder.Visible = True
                Repeater2PlaceHolder.Visible = False
        End Select
        If String.IsNullOrEmpty(errlabel.Text.Trim) Then
            plhErrorMessage.Visible = False
        Else
            plhErrorMessage.Visible = True
        End If
    End Sub

    Protected Sub Repeater1_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles Repeater1.ItemDataBound
        Try
            If Not e.Item.ItemIndex = -1 Then
                Dim ri As RepeaterItem = e.Item
                Dim check As CheckBox = CType(ri.FindControl("column1CheckBox"), CheckBox)
                Dim detailID As Label = CType(ri.FindControl("column1Label"), Label)
                Dim link As HyperLink = CType(ri.FindControl("column2HyperLink"), HyperLink)
                Dim productID As Label = CType(ri.FindControl("column2Label"), Label)
                Dim price As Label = CType(ri.FindControl("column3Label"), Label)
                Dim quantity As TextBox = CType(ri.FindControl("column4TextBox"), TextBox)
                Dim linePrice As Label = CType(ri.FindControl("column5Label"), Label)
                Dim masterProductLabel As Label = CType(ri.FindControl("lblMasterProduct"), Label)

                Select Case UCase(Usage)
                    Case Is = "TEMPLATES"
                        Dim err As ErrorObj
                        Dim products As DataTable
                        Dim templateDetail As DataRow = CType(e.Item.DataItem, DataRowView).Row
                        Dim productCode As String = TEBUtilities.CheckForDBNull_String(templateDetail("PRODUCT_CODE"))
                        detailID.Text = TEBUtilities.CheckForDBNull_String(templateDetail("TEMPLATE_DETAIL_ID"))
                        detailID.Visible = False
                        Dim prodInfo As New DEProductInfo(TalentCache.GetBusinessUnit, Profile.PartnerInfo.Details.Partner, productCode, _languageCode)
                        Dim DBProdInfo As New DBProductInfo(prodInfo)
                        DBProdInfo.Settings = TEBUtilities.GetSettingsObject()
                        err = DBProdInfo.AccessDatabase()

                        If Not err.HasError Then
                            products = DBProdInfo.ResultDataSet.Tables("ProductInformation")
                            If products.Rows.Count > 0 Then
                                Dim product As DataRow = products.Rows(0)
                                Dim masterProduct As String = String.Empty
                                masterProduct = templateDetail("MASTER_PRODUCT").ToString.Trim
                                If String.IsNullOrEmpty(masterProduct.Trim) Then masterProduct = productCode
                                link.Text = product("PRODUCT_DESCRIPTION_1")
                                link.NavigateUrl = GetProductURL(product, masterProduct)
                                link.Visible = Not ModuleDefaults.SuppressProductLinks
                                productID.Text = product("PRODUCT_CODE")
                                productID.Visible = False
                                Dim deWp As Talent.Common.DEWebPrice = TEBUtilities.GetWebPrices(productCode, CDec(templateDetail("QUANTITY")), masterProduct)
                                If ModuleDefaults.ShowPricesExVAT Then
                                    price.Text = CDec(TEBUtilities.RoundToValue(deWp.Purchase_Price_Net, 0.01, False))
                                    price.Text = TDataObjects.PaymentSettings.FormatCurrency(price.Text, _ucr.BusinessUnit, _ucr.PartnerCode)
                                Else
                                    price.Text = CDec(TEBUtilities.RoundToValue(deWp.Purchase_Price_Gross, 0.01, False))
                                    price.Text = TDataObjects.PaymentSettings.FormatCurrency(price.Text, _ucr.BusinessUnit, _ucr.PartnerCode)
                                End If
                                quantity.Text = TEBUtilities.RoundToValue(CDec(templateDetail("QUANTITY")), 1)
                                Dim linePriceDecimal As Decimal = 0
                                linePriceDecimal = CDec(TEBUtilities.RoundToValue(deWp.Purchase_Price_Gross, 0.01, False))
                                linePriceDecimal = linePriceDecimal * CDec(templateDetail("QUANTITY"))
                                linePrice.Text = TDataObjects.PaymentSettings.FormatCurrency(linePriceDecimal, _ucr.BusinessUnit, _ucr.PartnerCode)
                                masterProductLabel.Text = masterProduct
                            End If
                        End If

                    Case Is = "SAVEDORDERS"
                        Dim p As Product
                        p = CType(e.Item.DataItem, Product)
                        link.Text = p.Description1
                        link.NavigateUrl = p.NavigateURL
                        productID.Text = p.Code
                        productID.Visible = False
                        detailID.Text = p.ID
                        detailID.Visible = False
                        price.Text = TDataObjects.PaymentSettings.FormatCurrency(TEBUtilities.RoundToValue(p.PriceForSorting, 0.01), _ucr.BusinessUnit, _ucr.PartnerCode)
                        quantity.Text = TEBUtilities.RoundToValue(p.Quantity, 1)
                        linePrice.Text = TDataObjects.PaymentSettings.FormatCurrency(TEBUtilities.RoundToValue(p.PriceForSorting * p.Quantity, 0.01), _ucr.BusinessUnit, _ucr.PartnerCode)
                        masterProductLabel.Text = p.MasterProduct
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub Repeater2_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles Repeater2.ItemDataBound
        Try
            If Not e.Item.ItemIndex = -1 Then
                Dim ri As RepeaterItem = e.Item
                Dim check As CheckBox = CType(ri.FindControl("column1CheckBox"), CheckBox)
                Dim productImage As System.Web.UI.WebControls.Image = CType(ri.FindControl("img"), System.Web.UI.WebControls.Image)
                Dim productCodeLabel As Label = CType(ri.FindControl("productCodeLabel"), Label)
                Dim link As HyperLink = CType(ri.FindControl("productCodeLink"), HyperLink)
                Dim productDescriptionLabel As Label = CType(ri.FindControl("ProductDescription"), Label)
                Dim productBrandLabel As Label = CType(ri.FindControl("productBrandLabel"), Label)
                Dim html1 As Label = CType(ri.FindControl("html1"), Label)
                Dim html2 As Label = CType(ri.FindControl("html2"), Label)
                Dim packsize As Label = CType(ri.FindControl("packSizeLabel"), Label)
                Dim quantity As TextBox = CType(ri.FindControl("qtyBox"), TextBox)
                Dim linePrice As Label = CType(ri.FindControl("priceLabel"), Label)
                Dim hdfProductCode As HiddenField = CType(ri.FindControl("hdfProductCode"), HiddenField)
                Dim hdfMasterProduct As HiddenField = CType(ri.FindControl("hdfMasterProduct"), HiddenField)

                Select Case UCase(Usage)
                    Case Is = "TEMPLATES"
                        Dim err As ErrorObj
                        Dim products As DataTable
                        Dim templateDetail As DataRow = CType(e.Item.DataItem, DataRowView).Row
                        Dim productCode As String = TCUtilites.CheckForDBNull_String(templateDetail("PRODUCT_CODE"))
                        productImage.ImageUrl = ImagePath.getImagePath("PRODLIST", productCode, _ucr.BusinessUnit, _ucr.PartnerCode)
                        Dim prodInfo As New DEProductInfo(TalentCache.GetBusinessUnit, Profile.PartnerInfo.Details.Partner, productCode, _languageCode)
                        Dim DBProdInfo As New DBProductInfo(prodInfo)
                        DBProdInfo.Settings = TEBUtilities.GetSettingsObject()
                        err = DBProdInfo.AccessDatabase()

                        If Not err.HasError Then
                            products = DBProdInfo.ResultDataSet.Tables("ProductInformation")
                            If products.Rows.Count > 0 Then
                                Dim product As DataRow = products.Rows(0)
                                Dim mastProd As String = TEBUtilities.CheckForDBNull_String(templateDetail("MASTER_PRODUCT"))
                                link.Text = productCode
                                link.NavigateUrl = GetProductURL(product, mastProd)
                                If ModuleDefaults.SuppressProductLinks Then
                                    link.Enabled = False
                                End If

                                productDescriptionLabel.Text = product("PRODUCT_DESCRIPTION_1")
                                productDescriptionLabel.Visible = True
                                productCodeLabel.Text = productCode
                                productCodeLabel.Visible = True
                                html1.Text = TEBUtilities.CheckForDBNull_String(product("PRODUCT_HTML_1"))
                                html2.Text = TEBUtilities.CheckForDBNull_String(product("PRODUCT_HTML_2"))
                                packsize.Text = TEBUtilities.CheckForDBNull_String(product("PRODUCT_PACK_SIZE"))
                                html1.Visible = True
                                html2.Visible = True
                                packsize.Visible = True
                                productBrandLabel.Text = TEBUtilities.CheckForDBNull_String(product("PRODUCT_DESCRIPTION_5"))
                                If String.IsNullOrEmpty(mastProd) Then mastProd = productCode
                                Dim deWp As DEWebPrice = TEBUtilities.GetWebPrices(product("PRODUCT_CODE"), CDec(templateDetail("QUANTITY")), mastProd)
                                If ModuleDefaults.ShowPricesExVAT Then
                                    linePrice.Text = CDec(TEBUtilities.RoundToValue(deWp.Purchase_Price_Net, 0.01, False) * CDec(templateDetail("QUANTITY")))
                                    linePrice.Text = TDataObjects.PaymentSettings.FormatCurrency(linePrice.Text, _ucr.BusinessUnit, _ucr.PartnerCode)
                                Else
                                    linePrice.Text = CDec(TEBUtilities.RoundToValue(deWp.Purchase_Price_Gross, 0.01, False) * CDec(templateDetail("QUANTITY")))
                                    linePrice.Text = TDataObjects.PaymentSettings.FormatCurrency(linePrice.Text, _ucr.BusinessUnit, _ucr.PartnerCode)
                                End If
                                quantity.Text = TEBUtilities.RoundToValue(CDec(templateDetail("QUANTITY")), 1)
                                hdfProductCode.Value = productCode
                                hdfMasterProduct.Value = mastProd
                            End If
                        End If

                    Case Is = "SAVEDORDERS"
                        Dim p As Product
                        p = CType(e.Item.DataItem, Product)
                        link.Text = p.Description1
                        link.NavigateUrl = p.NavigateURL
                        quantity.Text = p.Quantity
                        linePrice.Text = TDataObjects.PaymentSettings.FormatCurrency(TEBUtilities.RoundToValue(p.PriceForSorting * p.Quantity, 0.01), _ucr.BusinessUnit, _ucr.PartnerCode)
                End Select
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub ReplaceButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReplaceItems.Click
        plhReplaceBasketQuestion.Visible = False
        Profile.Basket.EmptyBasket()
        AddToBasket()
    End Sub

    Protected Sub KeepButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnKeepItems.Click
        plhReplaceBasketQuestion.Visible = False
        AddToBasket()
    End Sub

    Protected Sub AddToBasketButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles AddToBasketButton.Click
        If Profile.Basket.IsEmpty Then
            plhReplaceBasketQuestion.Visible = False
            AddToBasket()
        Else
            plhReplaceBasketQuestion.Visible = True
        End If
    End Sub

    Protected Sub UpdateButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles UpdateButton.Click
        Select Case UCase(Usage)
            Case Is = "TEMPLATES"
                'Retrieve the information on the currently selected template
                Dim DE_OrdTemplates As New Talent.Common.DEOrderTemplates("SELECT")
                Dim ordTemplate As New Talent.Common.DEOrderTemplate(CType(QueryString_HeaderID, Long))
                DE_OrdTemplates.OrderTemplates.Add(ordTemplate)
                Dim DB_OrdTemplates As New Talent.Common.DBOrderTemplates(DE_OrdTemplates)
                DB_OrdTemplates.Settings = TEBUtilities.GetSettingsObject()
                DB_OrdTemplates.AccessDatabase()

                'Re-create the template, with the header info retrieved, add the items to the template, and save the deftails
                Dim header As Data.DataTable = DB_OrdTemplates.ResultDataSet.Tables("OrderTemplatesHeader")
                If header.Rows.Count > 0 Then
                    Dim row As Data.DataRow = header.Rows(0)
                    ordTemplate = New Talent.Common.DEOrderTemplate(row("LOGINID"), row("BUSINESS_UNIT"), row("PARTNER"), CDate(row("CREATED_DATE")), CDate(row("LAST_USED_DATE")), _
                                                                    Now, row("NAME"), row("DESCRIPTION"), row("IS_DEFAULT"), CType(row("TEMPLATE_HEADER_ID"), Long))

                    'Populate each item in the basket into a OrderTemplateDetail Object and add each into the order template object's items collection
                    Dim productCode As String = String.Empty
                    Dim qty As String = String.Empty
                    Dim masterProduct As String = String.Empty
                    Select Case ModuleDefaults.OrderTemplateLayoutType
                        Case "2"
                            For Each ri As RepeaterItem In Repeater2.Items
                                productCode = CType(ri.FindControl("hdfProductCode"), HiddenField).Value
                                qty = CType(ri.FindControl("qtyBox"), TextBox).Text
                                masterProduct = CType(ri.FindControl("hdfMasterProduct"), HiddenField).Value
                                ordTemplate.OrderTemplateItems.Add(New Talent.Common.OrderTemplateDetail(productCode, qty, masterProduct))
                            Next
                        Case Else
                            For Each ri As RepeaterItem In Repeater1.Items
                                productCode = CType(ri.FindControl("column2Label"), Label).Text
                                qty = CType(ri.FindControl("column4TextBox"), TextBox).Text
                                masterProduct = CType(ri.FindControl("lblMasterProduct"), Label).Text
                                ordTemplate.OrderTemplateItems.Add(New Talent.Common.OrderTemplateDetail(productCode, qty, masterProduct))
                            Next
                    End Select

                    DE_OrdTemplates.OrderTemplates.Clear()
                    DE_OrdTemplates.Purpose = "UPDATE"
                    DE_OrdTemplates.OrderTemplates.Add(ordTemplate)
                    DB_OrdTemplates = New Talent.Common.DBOrderTemplates(DE_OrdTemplates)
                    DB_OrdTemplates.Settings = TEBUtilities.GetSettingsObject()
                    DB_OrdTemplates.AccessDatabase()

                    'Update the Summary Totals as the quantity of items might of changed
                    SummaryTotals1.SetUpTotals()
                End If

            Case Is = "SAVEDORDERS"
                'Update the order
                Dim details As New SavedOrdersDataSetTableAdapters.tbl_order_saved_detailTableAdapter
                Dim updateSuccessful As Boolean = True

                For Each item As RepeaterItem In Repeater1.Items
                    'Find the UPDATED data in the repeater
                    Dim detailID As Label = DirectCast(item.FindControl("column1Label"), Label)
                    Dim productCode As Label = DirectCast(item.FindControl("column2Label"), Label)
                    Dim quantity As TextBox = DirectCast(item.FindControl("column4TextBox"), TextBox)
                    Dim masterProduct As Label = DirectCast(item.FindControl("lblMasterProduct"), Label)
                    Try
                        'Convert to the correct data type
                        Dim savedDetailID As Long = CType(detailID.Text, Long)
                        Dim savedHeaderID As Long = CType(QueryString_HeaderID, Long)
                        Dim quantityDecimal As Decimal = CType(quantity.Text, Decimal)

                        'Send the rows to the database for update
                        details.UpdateSavedOrder(savedDetailID, savedHeaderID, productCode.Text, quantityDecimal, masterProduct.Text)
                    Catch ex As Exception
                        'Update has failed
                        updateSuccessful = False
                    End Try
                Next

                'Check if the update has been successful, display correct message
                If updateSuccessful Then
                    errlabel.Text = _ucr.Content("SavedOrderUpdated", _languageCode, True)
                Else
                    errlabel.Text = _ucr.Content("SavedOrderNotUpdated", _languageCode, True)
                End If
            Case Else
        End Select
        BindDetailView()
    End Sub

    Protected Sub ShowAllButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ShowAllButton.Click
        Session("ShowAll_Selected") = True
        Response.Redirect(Request.Url.AbsoluteUri)
    End Sub

    Protected Sub SetLayout2_Visibility(ByVal sender As Object, ByVal e As EventArgs)
        CType(sender, HtmlTableCell).Visible = Not TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("LayoutType2_SpecifyQtyToAdd"))
    End Sub

    Protected Sub ChangePage(ByVal sender As Object, ByVal e As EventArgs)
        Dim lb As LinkButton = CType(sender, LinkButton)
        Dim pageIndex As Integer = 0
        Try
            pageIndex = CInt(lb.Text)
            pageIndex -= 1
        Catch ex As Exception
            If lb.Text = _ucr.Content("FirstPageNavigationText", _languageCode, True) Then
                pageIndex = 0
            ElseIf lb.Text = _ucr.Content("LastPageNavigationText", _languageCode, True) Then
                pageIndex = _pds.PageCount - 1
            End If
        End Try

        _pds.CurrentPageIndex = pageIndex
        BindDetailView()
        DisplayPagerNav(_pds)
    End Sub

    Protected Sub CheckBoxChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim cb As CheckBox = CType(sender, CheckBox)
        Dim ri As RepeaterItem = CType(cb.Parent, RepeaterItem)
    End Sub

    Protected Sub GetText(ByVal sender As Object, ByVal e As EventArgs)
        Try
            Dim ctrl As Control = CType(sender, Control)
            With _ucr
                Select Case ctrl.ID
                    ' Labels
                    Case Is = "column1Header"
                        CType(ctrl, Label).Text = ""
                    Case Is = "column2Header"
                        CType(ctrl, Label).Text = .Content("ProductNameHeader", _languageCode, True)
                    Case Is = "column3Header"
                        CType(ctrl, Label).Text = .Content("UnitHeader", _languageCode, True)
                    Case Is = "column4Header"
                        CType(ctrl, Label).Text = .Content("QuantityHeader", _languageCode, True)
                    Case Is = "column5Header"
                        CType(ctrl, Label).Text = .Content("LineValueHeader", _languageCode, True)
                    Case Is = "buttonsHeader"
                        CType(ctrl, Label).Text = .Content("ButtonsColumnHeader", _languageCode, True)
                    Case Is = "productImageColumnHeader"
                        CType(ctrl, Label).Text = .Content("productImageColumnHeader", _languageCode, True)
                    Case Is = "productCodeColumnHeader"
                        CType(ctrl, Label).Text = .Content("productCodeColumnHeader", _languageCode, True)
                    Case Is = "descriptionColumnHeader"
                        CType(ctrl, Label).Text = .Content("descriptionColumnHeader", _languageCode, True)
                    Case Is = "packsizeColumnHeader"
                        CType(ctrl, Label).Text = .Content("packsizeColumnHeader", _languageCode, True)
                    Case Is = "quantityColumnHeader"
                        CType(ctrl, Label).Text = .Content("QuantityHeader", _languageCode, True)
                    Case Is = "ValueColumnHeader"
                        CType(ctrl, Label).Text = .Content("LineValueHeader", _languageCode, True)

                        ' Link Buttons
                    Case Is = "ViewDetailsLink"
                        CType(ctrl, Button).Text = "" '.Content("", _languageCode, True)
                        CType(ctrl, Button).Visible = False
                    Case Is = "AddLink"
                        CType(ctrl, Button).Text = "" '.Content("", _languageCode, True)
                        CType(ctrl, Button).Visible = False
                    Case Is = "DeleteLink"
                        CType(ctrl, Button).Text = .Content("DeleteProductButtonText", _languageCode, True)
                    Case Is = "SelectAll"
                        CType(ctrl, Button).Text = .Content("SelectAllProductsButtonText", _languageCode, True)
                    Case Is = "DeSelectAll"
                        CType(ctrl, Button).Text = .Content("DeSelectAllProductsButtonText", _languageCode, True)

                        ' Buttons
                    Case Is = "AddToBasketButton"
                        CType(ctrl, Button).Text = .Content("AddToBasketButtonText", _languageCode, True)
                    Case Is = "UpdateButton"
                        CType(ctrl, Button).Text = .Content("UpdateButtonText", _languageCode, True)
                    Case Is = "productCodeLabel"
                        CType(ctrl, Label).Text = _ucr.Content("ProductCodeText", _languageCode, True)
                End Select
            End With
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub DeleteItem(ByVal sender As Object, ByVal e As EventArgs)
        Dim ri As RepeaterItem = CType(CType(sender, Button).Parent, RepeaterItem)
        Dim DetailID As String = CType(ri.FindControl("column1Label"), Label).Text
        Select Case UCase(Usage)
            Case Is = "TEMPLATES"
                Dim DE_OrdTemplates As New Talent.Common.DEOrderTemplates("DELETE-LINE")
                Dim ordTemplate As New Talent.Common.DEOrderTemplate(CType(QueryString_HeaderID, Long))
                ordTemplate.OrderTemplateItems.Add(New Talent.Common.OrderTemplateDetail(CType(DetailID, Long)))

                DE_OrdTemplates.OrderTemplates.Add(ordTemplate)
                Dim DB_OrdTemplates As New Talent.Common.DBOrderTemplates(DE_OrdTemplates)
                DB_OrdTemplates.Settings = TEBUtilities.GetSettingsObject()
                DB_OrdTemplates.AccessDatabase()
            Case Is = "SAVEDORDERS"
                Dim details As New SavedOrdersDataSetTableAdapters.tbl_order_saved_detailTableAdapter
                details.DeleteBy_DetailID(DetailID)
        End Select
        BindDetailView()
        SummaryTotals1.SetUpTotals()
    End Sub

    Protected Sub SelectAll_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim cb As CheckBox
        For Each ri As RepeaterItem In Repeater1.Items
            cb = CType(ri.FindControl("column1CheckBox"), CheckBox)
            cb.Checked = True
        Next
    End Sub

    Protected Sub DeSelectAll_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim cb As CheckBox
        For Each ri As RepeaterItem In Repeater1.Items
            cb = CType(ri.FindControl("column1CheckBox"), CheckBox)
            cb.Checked = False
        Next
    End Sub

#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Bind the details into the repeater
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub BindDetailView()
        Dim hasItems As Boolean = False
        Select Case ModuleDefaults.OrderTemplateLayoutType
            Case "2"
                Repeater2.DataSource = SetUpPagedView()
                Repeater2.DataBind()
                If Repeater2.Items.Count > 0 Then hasItems = True
            Case Else
                Repeater1.DataSource = SetUpPagedView()
                Repeater1.DataBind()
                If Repeater1.Items.Count > 0 Then hasItems = True
        End Select
        If Not hasItems Then
            plhResults.Visible = False
            errlabel.Text = _ucr.Content("NoItemsInTheTemplate", _languageCode, True)
        Else
            UpdateButton.Visible = ShowOptions
        End If
    End Sub

    ''' <summary>
    ''' Setup the pager
    ''' </summary>
    ''' <param name="pds">The paged data source object</param>
    ''' <remarks></remarks>
    Private Sub DisplayPagerNav(ByVal pds As PagedDataSource)
        If pds.PageCount > 1 Then
            Dim startPage As Integer = 0
            Dim loopToPage As Integer = 0
            Dim currentPage As Integer = 0
            currentPage = pds.CurrentPageIndex + 1

            'If there are more than 10 pages we need to sort out which 10 pages either side
            'of the current page to display in the navigation:
            'E.g. if page = 8 and ther are 15 pages, the navigation should be:
            'First 3 4 5 6 7 8 9 10 11 12 Last
            If pds.PageCount > 10 Then
                If Not pds.CurrentPageIndex = 0 Then
                    For i As Integer = 3 To 0 Step -1
                        If currentPage + i <= pds.PageCount Then
                            startPage = currentPage - (9 - i)
                            If startPage < 0 Then
                                startPage = 0
                            End If
                            Exit For
                        End If
                    Next
                End If
                loopToPage = startPage + 9
            Else
                loopToPage = pds.PageCount
            End If

            If loopToPage = pds.PageCount Then
                If pds.PageCount > 10 Then
                    startPage -= 1
                    loopToPage -= 1
                Else
                    loopToPage -= 1
                End If
            End If

            If pds.PageCount > 10 Then
                FirstTop.Text = _ucr.Content("FirstItemNavigationText", _languageCode, True)
                FirstBottom.Text = _ucr.Content("FirstItemNavigationText", _languageCode, True)
                LastTop.Text = _ucr.Content("LastItemNavigationText", _languageCode, True)
                LastBottom.Text = _ucr.Content("LastItemNavigationText", _languageCode, True)
            End If

            Dim count As Integer = 0
            For i As Integer = startPage To loopToPage
                count += 1
                CType(Me.FindControl("Nav" & count.ToString & "Top"), LinkButton).Text = (i + 1).ToString
                CType(Me.FindControl("Nav" & count.ToString & "Bottom"), LinkButton).Text = (i + 1).ToString

                'Set the font to bold if is the selected page otherwise set it back to normal
                If pds.CurrentPageIndex = i Then
                    CType(Me.FindControl("Nav" & count.ToString & "Top"), LinkButton).Font.Bold = True
                    CType(Me.FindControl("Nav" & count.ToString & "Bottom"), LinkButton).Font.Bold = True
                Else
                    CType(Me.FindControl("Nav" & count.ToString & "Top"), LinkButton).Font.Bold = False
                    CType(Me.FindControl("Nav" & count.ToString & "Bottom"), LinkButton).Font.Bold = False
                End If
            Next

            count += 1
            For i As Integer = count To 10
                CType(Me.FindControl("Nav" & i.ToString & "Top"), LinkButton).Text = String.Empty
                CType(Me.FindControl("Nav" & i.ToString & "Bottom"), LinkButton).Text = String.Empty
            Next

            Dim arg0, arg1, arg2 As Integer
            arg0 = pds.PageSize * pds.CurrentPageIndex + 1
            If pds.IsLastPage Then
                arg1 = pds.DataSourceCount
            Else
                arg1 = (arg0 - 1) + pds.PageSize
            End If
            arg2 = pds.DataSourceCount

            CurrentResultsDisplaying.Text = String.Format(_ucr.Content("ResultsCurrentViewText", _languageCode, True), arg0.ToString, arg1.ToString, arg2.ToString)
        End If
    End Sub

    ''' <summary>
    ''' Set the paged data source object
    ''' </summary>
    ''' <returns>Paged data source object</returns>
    ''' <remarks></remarks>
    Private Function SetUpPagedView() As PagedDataSource
        _pds.PageSize = CInt(_ucr.Content("ResultsViewPageSize", _languageCode, True))
        Dim al As New ArrayList
        Dim productsToRemove As New ArrayList
        Dim exists As Boolean = False

        Select Case UCase(Usage)
            Case Is = "TEMPLATES"
                'Retrieve the information on the currently selected template
                Dim DE_OrdTemplates As New DEOrderTemplates("SELECT")
                Dim ordTemplate As New DEOrderTemplate(CType(QueryString_HeaderID, Long))
                DE_OrdTemplates.OrderTemplates.Add(ordTemplate)

                Dim DB_OrdTemplates As New DBOrderTemplates(DE_OrdTemplates)
                DB_OrdTemplates.Settings = TEBUtilities.GetSettingsObject()
                Dim err As ErrorObj = DB_OrdTemplates.AccessDatabase()
                If Not err.HasError Then
                    Dim detail As DataTable = DB_OrdTemplates.ResultDataSet.Tables("OrderTemplatesDetail")
                    _pds.DataSource = detail.DefaultView
                End If

            Case Is = "SAVEDORDERS"
                Dim err As New ErrorObj
                Try
                    Const SqlServer2005 As String = "SqlServer2005"
                    _conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SqlServer2005).ConnectionString)
                    _conTalent.Open()
                Catch ex As Exception
                    Const strError1 As String = "Could not establish connection to the database"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError1
                        .ErrorNumber = "TEAPUIL-01"
                        .HasError = True
                    End With
                End Try

                Dim pageProductsList As IList
                Try
                    Dim intMaxNoOfGroupLevels As Integer = ModuleDefaults.NumberOfGroupLevels
                    _completeProductsList = New ProductListGen
                    Dim strSelect As String = "SELECT DISTINCT " & _
                    "  tbl_order_saved_detail.PRODUCT_CODE, tbl_product.PRODUCT_DESCRIPTION_1, tbl_order_saved_detail.QUANTITY, " & _
                    "  tbl_order_saved_detail.MASTER_PRODUCT, tbl_group_product.GROUP_L01_GROUP, tbl_group_product.GROUP_L02_GROUP, " & _
                    "          tbl_group_product.GROUP_L03_GROUP, tbl_group_product.GROUP_L04_GROUP, " & _
                    "  tbl_group_product.GROUP_L05_GROUP, tbl_group_product.GROUP_L06_GROUP, tbl_group_product.GROUP_L07_GROUP, " & _
                    "  tbl_group_product.GROUP_L08_GROUP, tbl_group_product.GROUP_L09_GROUP, tbl_group_product.GROUP_L10_GROUP, " & _
                    "  tbl_order_saved_detail.SAVED_HEADER_ID,  " & _
                    "tbl_order_saved_detail.SAVED_DETAIL_ID  " & _
                    "  FROM  " & _
                    "    tbl_product INNER JOIN " & _
                    "   tbl_order_saved_detail ON tbl_product.PRODUCT_CODE = tbl_order_saved_detail.PRODUCT_CODE INNER JOIN  " & _
                    "   tbl_group_product ON tbl_product.PRODUCT_CODE = tbl_group_product.PRODUCT " & _
                    "  WHERE    tbl_order_saved_detail.SAVED_HEADER_ID = @SAVED_HEADER_ID "

                    _cmdSelect = New SqlCommand(strSelect, _conTalent)
                    _cmdSelect.Parameters.Add(New SqlParameter("@SAVED_HEADER_ID", SqlDbType.BigInt)).Value = QueryString_HeaderID
                    _dtrProduct = _cmdSelect.ExecuteReader()

                    If _dtrProduct.HasRows Then
                        Dim productCodes As New Generic.Dictionary(Of String, WebPriceProduct)
                        While _dtrProduct.Read
                            Dim p As New Product
                            p.Code = _dtrProduct("PRODUCT_CODE")
                            p.Description1 = _dtrProduct("PRODUCT_DESCRIPTION_1")
                            p.Quantity = _dtrProduct("QUANTITY")
                            p.ID = _dtrProduct("SAVED_DETAIL_ID")
                            p.MasterProduct = _dtrProduct("MASTER_PRODUCT")

                            ' Build navigate query string
                            Dim sbQry As New StringBuilder
                            With sbQry
                                .Append("../PagesPublic/ProductBrowse/product.aspx?group1=").Append(p.Group1)
                                If intMaxNoOfGroupLevels > 1 Then .Append("&group2=").Append(p.Group2)
                                If intMaxNoOfGroupLevels > 2 Then .Append("&group3=").Append(p.Group3)
                                If intMaxNoOfGroupLevels > 3 Then .Append("&group4=").Append(p.Group4)
                                If intMaxNoOfGroupLevels > 4 Then .Append("&group5=").Append(p.Group5)
                                If intMaxNoOfGroupLevels > 5 Then .Append("&group6=").Append(p.Group6)
                                If intMaxNoOfGroupLevels > 6 Then .Append("&group7=").Append(p.Group7)
                                If intMaxNoOfGroupLevels > 7 Then .Append("&group8=").Append(p.Group8)
                                If intMaxNoOfGroupLevels > 8 Then .Append("&group9=").Append(p.Group9)
                                If intMaxNoOfGroupLevels > 9 Then .Append("&group10=").Append(p.Group10)
                                .Append("&product=").Append(p.Code)
                            End With
                            p.NavigateURL = sbQry.ToString

                            Dim prodPrice As DEWebPrice
                            Dim inList As Boolean = False
                            For Each prod As Product In _completeProductsList.Products
                                If prod.Code = p.Code Then
                                    inList = True
                                End If
                            Next
                            If Not inList Then
                                prodPrice = TEBUtilities.GetWebPrices_WithPromoDetails(p.Code, CDec(_dtrProduct("QUANTITY")), _dtrProduct("MASTER_PRODUCT").ToString)
                                p.PriceForSorting = prodPrice.DisplayPrice
                                _completeProductsList.Add(p)
                            End If
                        End While
                    End If

                    If Not _completeProductsList Is Nothing Then
                        'order the product list
                        'BF -Don't order the list as there is no seq no and it will fall over completeProductsList.SortProductsByBestSeller("A")
                        pageProductsList = _completeProductsList.GetPageProducts(1)
                        _pds.DataSource = pageProductsList
                    End If
                Catch ex As Exception
                    Const strError8 As String = "Error during database access"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError8
                        .ErrorNumber = "TEAPUIL-06"
                        .HasError = True
                    End With
                End Try
                Try
                    _conTalent.Close()
                Catch ex As Exception
                    Const strError9 As String = "Failed to close database connection"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError9
                        .ErrorNumber = "TEAPUIL-07"
                        .HasError = True
                    End With
                End Try
        End Select

        If _pds.DataSourceCount <= _pds.PageSize Then
            ShowAllButton.Visible = False
        End If
        Return _pds
    End Function

    ''' <summary>
    ''' Get the URL of the product that the user will click
    ''' </summary>
    ''' <param name="product">The product code</param>
    ''' <param name="masterProduct">The master product code</param>
    ''' <returns>A url as a string</returns>
    ''' <remarks></remarks>
    Private Function GetProductURL(ByVal product As DataRow, ByVal masterProduct As String) As String
        Dim url As String = String.Empty
        url = "~/PagesPublic/ProductBrowse/product.aspx?"
        Try
            url += "group1=" & product("GROUP_L01_GROUP")
        Catch ex As Exception
            'If there is no Group Level 01 then the link will not work anyway
            Return String.Empty
        End Try
        Try
            url += "&group2=" & product("GROUP_L02_GROUP")
        Catch ex As Exception
        End Try
        Try
            url += "&group3=" & product("GROUP_L03_GROUP")
        Catch ex As Exception
        End Try
        Try
            url += "&group4=" & product("GROUP_L04_GROUP")
        Catch ex As Exception
        End Try
        Try
            url += "&group5=" & product("GROUP_L05_GROUP")
        Catch ex As Exception
        End Try
        Try
            url += "&group6=" & product("GROUP_L06_GROUP")
        Catch ex As Exception
        End Try
        Try
            url += "&group7=" & product("GROUP_L07_GROUP")
        Catch ex As Exception
        End Try
        Try
            url += "&group8=" & product("GROUP_L08_GROUP")
        Catch ex As Exception
        End Try
        Try
            url += "&group9=" & product("GROUP_L09_GROUP")
        Catch ex As Exception
        End Try
        Try
            url += "&group10=" & product("GROUP_L10_GROUP")
        Catch ex As Exception
        End Try
        If String.IsNullOrEmpty(masterProduct) Then
            url += "&product=" & product("PRODUCT_CODE")
        Else
            url += "&product=" & masterProduct
        End If
        Return url
    End Function

    ''' <summary>
    ''' Add the items selected, to the basket
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AddToBasket()
        errlabel.Text = ""
        Dim amendBasket As New Talent.Common.DEAmendBasket
        With amendBasket
            .AddToBasket = True
            .BasketId = Profile.Basket.Basket_Header_ID
            .BusinessUnit = TalentCache.GetBusinessUnit
            .PartnerCode = Profile.PartnerInfo.Details.Partner
            .UserID = Profile.UserName
        End With
        Dim deaItem As Talent.Common.DEAlerts
        Dim itemsAdded As Boolean = False
        Dim repeaterInUse As New Repeater

        Select Case ModuleDefaults.OrderTemplateLayoutType
            Case "2"
                repeaterInUse = Repeater2
            Case Else
                repeaterInUse = Repeater1
        End Select

        For Each ri As RepeaterItem In repeaterInUse.Items
            Dim qty As TextBox
            Select Case ModuleDefaults.OrderTemplateLayoutType
                Case 2
                    qty = CType(ri.FindControl("qtyBox"), TextBox)
                Case Else
                    qty = CType(ri.FindControl("column4TextBox"), TextBox)
            End Select
            Try
                qty.Text = CType(qty.Text, Integer)
            Catch ex As Exception
                errlabel.Text = _ucr.Content("invalidQuantityErrorText", _languageCode, True)
                Exit Sub
            End Try
        Next

        For Each ri As RepeaterItem In repeaterInUse.Items
            Dim qty As TextBox
            Dim check As CheckBox = CType(ri.FindControl("column1CheckBox"), CheckBox)
            Select Case ModuleDefaults.OrderTemplateLayoutType
                Case 2
                    qty = CType(ri.FindControl("qtyBox"), TextBox)
                Case Else
                    qty = CType(ri.FindControl("column4TextBox"), TextBox)
            End Select

            Dim itemToAdd As Boolean = False
            If TEBUtilities.CheckForDBNullOrBlank_Boolean_DefaultFalse(_ucr.Attribute("LayoutType2_SpecifyQtyToAdd")) Then
                itemToAdd = Not String.IsNullOrEmpty(qty.Text)
            Else
                itemToAdd = check.Checked
            End If

            If itemToAdd Then
                Try
                    Select Case ModuleDefaults.OrderTemplateLayoutType
                        Case 2
                            Dim productCode As HyperLink = CType(ri.FindControl("productCodeLink"), HyperLink)
                            Dim hdfMasterProduct As HiddenField = CType(ri.FindControl("hdfMasterProduct"), HiddenField)
                            Dim quant As Integer = ModuleDefaults.Default_Add_Quantity
                            Try
                                quant = CDec(qty.Text)
                            Catch ex As Exception
                            End Try
                            If Not String.IsNullOrWhiteSpace(hdfMasterProduct.Value) Then
                                Dim tbi As New TalentBasketItem
                                With tbi
                                    .Product = productCode.Text
                                    .Quantity = quant
                                    Dim products As Data.DataTable = TEBUtilities.GetProductInfo(productCode.Text)
                                    If products IsNot Nothing Then
                                        If products.Rows.Count > 0 Then
                                            .ALTERNATE_SKU = TEBUtilities.CheckForDBNull_String(products.Rows(0)("ALTERNATE_SKU"))
                                            .PRODUCT_DESCRIPTION1 = TEBUtilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_1"))
                                            .PRODUCT_DESCRIPTION2 = TEBUtilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_2"))
                                            .PRODUCT_DESCRIPTION3 = TEBUtilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_3"))
                                            .PRODUCT_DESCRIPTION4 = TEBUtilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_4"))
                                            .PRODUCT_DESCRIPTION5 = TEBUtilities.CheckForDBNull_String(products.Rows(0)("PRODUCT_DESCRIPTION_5"))
                                        End If
                                    End If

                                    Dim deWp As DEWebPrice = TEBUtilities.GetWebPrices(productCode.Text, quant, hdfMasterProduct.Value)
                                    .Gross_Price = CDec(TEBUtilities.RoundToValue(deWp.Purchase_Price_Gross, 0.01, False) * 1)
                                    .Net_Price = CDec(TEBUtilities.RoundToValue(deWp.Purchase_Price_Net, 0.01, False) * 1)
                                    .Tax_Price = CDec(TEBUtilities.RoundToValue(deWp.Purchase_Price_Tax, 0.01, False) * 1)
                                    If Not Profile.IsAnonymous And Not Profile.PartnerInfo.Details Is Nothing Then
                                        .Cost_Centre = Profile.PartnerInfo.Details.COST_CENTRE
                                        .Account_Code = Order.GetLastAccountNo(Profile.User.Details.LoginID)
                                    End If
                                End With
                                Profile.Basket.AddItem(tbi)
                                itemsAdded = True
                            End If

                        Case Else
                            Dim productCode As Label
                            Dim masterProduct As Label
                            productCode = CType(ri.FindControl("column2Label"), Label)
                            masterProduct = CType(ri.FindControl("lblMasterProduct"), Label)
                            If Not String.IsNullOrWhiteSpace(masterProduct.Text) Then
                                deaItem = New Talent.Common.DEAlerts
                                deaItem.ProductCode = productCode.Text
                                deaItem.MasterProduct = masterProduct.Text
                                Dim products As Data.DataTable = TEBUtilities.GetProductInfo(productCode.Text)
                                If products IsNot Nothing Then
                                    If products.Rows.Count > 0 Then
                                        deaItem.AlternateSKU = TEBUtilities.CheckForDBNull_String(products.Rows(0)("ALTERNATE_SKU"))
                                    End If
                                End If
                                Try
                                    deaItem.Quantity = CDec(qty.Text)
                                Catch ex As Exception
                                    deaItem.Quantity = 1
                                End Try
                                deaItem.Price = TEBUtilities.GetWebPrices(deaItem.ProductCode, deaItem.Quantity, deaItem.MasterProduct).Purchase_Price_Gross
                                If Not Profile.IsAnonymous And Not Profile.PartnerInfo.Details Is Nothing Then
                                    deaItem.CostCentre = Profile.PartnerInfo.Details.COST_CENTRE
                                    deaItem.AccountCode = Order.GetLastAccountNo(Profile.User.Details.LoginID)
                                End If
                                amendBasket.CollDEAlerts.Add(deaItem)
                                itemsAdded = True
                            End If
                    End Select
                Catch ex As Exception
                End Try
            End If
        Next

        If amendBasket.CollDEAlerts.Count > 0 Then
            Dim DBAmend As New DBAmendBasket
            With DBAmend
                .Dep = amendBasket
                .Settings = TEBUtilities.GetSettingsObject()
                .AccessDatabase()
            End With
        End If

        If itemsAdded Then
            Dim forwardToBasketOnAdd As Boolean = False
            If _ucr.Attribute("forwardToBasketOnAdd").ToUpper = "TRUE" Then forwardToBasketOnAdd = True
            If forwardToBasketOnAdd Then
                Response.Redirect("~/PagesPublic/Basket/Basket.aspx")
            Else
                Response.Redirect(Request.Url.ToString)
            End If
        Else
            errlabel.Text = _ucr.Content("NoItemsToAddErrorText", _languageCode, True)
        End If
    End Sub

#End Region

End Class