Imports System.Data
Imports Talent.Common

Partial Class Products_ProductDetails
    Inherits PageControlBase

    Dim productID As String = String.Empty
    Dim products As New ProductsTableAdapters.tbl_productTableAdapter

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        productID = Request.QueryString("ID")

        If Page.IsPostBack Then
            ErrLabel.Text = ""
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
        master.HeaderText = "Change Product Descriptions"
        titleLabel.Text = master.HeaderText

        If Not String.IsNullOrEmpty(productID) Then
            If IsPostBack = False Then

                setLabel()

                FreeTextBox1.Visible = False
                SaveButton.Visible = False
                CancelEditButton.Visible = False

                FreeTextBox2.Visible = False
                SaveButton2.Visible = False
                CancelEditButton2.Visible = False

                FreeTextBox3.Visible = False
                SaveButton3.Visible = False
                CancelEditButton3.Visible = False

                FreeTextBox4.Visible = False
                SaveButton4.Visible = False
                CancelEditButton4.Visible = False

                RetrieveVATCodes()
                bindStockRepeater()
                bindPricesTable()
            End If
        Else
            ErrLabel.Text = "There is no product ID Querystring value. Go back and choose another product."
        End If
    End Sub

    Public Sub setLabel()
        Dim dt As DataTable

        dt = TDataObjects.ProductsSettings.TblProduct.GetProductByProductID(productID)

        ID.Text = dt.Rows(0)("PRODUCT_ID").ToString
        product.Text = dt.Rows(0)("PRODUCT_CODE").ToString
        textDescription1.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_1").ToString
        textDescription2.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_2").ToString
        textDescription3.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_3").ToString
        textDescription4.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_4").ToString
        textDescription5.Text = dt.Rows(0)("PRODUCT_DESCRIPTION_5").ToString
        TextHTML1.Text = dt.Rows(0)("PRODUCT_HTML_1").ToString
        TextHTML2.Text = dt.Rows(0)("PRODUCT_HTML_2").ToString
        TextHTML3.Text = dt.Rows(0)("PRODUCT_HTML_3").ToString
        textWeight.Text = dt.Rows(0)("PRODUCT_WEIGHT").ToString
        textWeightUnit.Text = dt.Rows(0)("PRODUCT_WEIGHT_UOM").ToString
        textGLCode1.Text = dt.Rows(0)("PRODUCT_GLCODE_1").ToString
        textGLCode2.Text = dt.Rows(0)("PRODUCT_GLCODE_2").ToString
        textGLCode3.Text = dt.Rows(0)("PRODUCT_GLCODE_3").ToString
        textGLCode4.Text = dt.Rows(0)("PRODUCT_GLCODE_4").ToString
        textGLCode5.Text = dt.Rows(0)("PRODUCT_GLCODE_5").ToString
        TextHTML4.Text = dt.Rows(0)("PRODUCT_SEARCH_KEYWORDS").ToString
        plhDeleteProducts.Visible = Utilities.CheckForDBNull_Boolean_DefaultFalse(dt.Rows(0)("PRODUCT_OPTION_MASTER").ToString())
    End Sub

    Protected Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelButton.Click
        If Request.QueryString("find") Is Nothing Then
            Response.Redirect("ProductAdmin.aspx?" & GetQuerystrings())
        Else
            Response.Redirect("ProductAdmin.aspx?find=" & Request.QueryString("find") & "&" & GetQuerystrings())
        End If
    End Sub

    Public Sub Editdata(ByVal source As Object, ByVal e As EventArgs) Handles btnEditHtml1.Click
        btnEditHtml1.Visible = False

        FreeTextBox1.Value = TextHTML1.Text
        FreeTextBox1.Visible = True
        SaveButton.Visible = True
        CancelEditButton.Visible = True
        TextHTML1.Enabled = False
    End Sub

    Protected Sub SaveButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveButton.Click
        btnEditHtml1.Visible = True

        CancelEditButton.Visible = False
        TextHTML1.Text = FreeTextBox1.Value
        FreeTextBox1.Visible = False
        SaveButton.Visible = False
        TextHTML1.Enabled = True
        ErrLabel.Text = ("Don't forget to click the Confirm button to save changes!")
    End Sub

    Protected Sub CancelEditButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelEditButton.Click
        btnEditHtml1.Visible = True

        CancelEditButton.Visible = False
        FreeTextBox1.Value = ""
        FreeTextBox1.Visible = False
        SaveButton.Visible = False
        TextHTML1.Enabled = True
    End Sub

    Public Sub Editdata2(ByVal source As Object, ByVal e As EventArgs) Handles btnEditHtml2.Click
        btnEditHtml2.Visible = False

        FreeTextBox2.Value = TextHTML2.Text
        FreeTextBox2.Visible = True
        SaveButton2.Visible = True
        CancelEditButton2.Visible = True
        TextHTML2.Enabled = False
    End Sub

    Public Sub Editdata4(ByVal source As Object, ByVal e As EventArgs) Handles btnEditHtml4.Click
        btnEditHtml4.Visible = False

        FreeTextBox4.Value = TextHTML4.Text
        FreeTextBox4.Visible = True
        SaveButton4.Visible = True
        CancelEditButton4.Visible = True
        TextHTML4.Enabled = False
    End Sub

    Protected Sub SaveButton2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveButton2.Click
        btnEditHtml2.Visible = True

        CancelEditButton2.Visible = False
        TextHTML2.Text = FreeTextBox2.Value
        FreeTextBox2.Visible = False
        SaveButton2.Visible = False
        TextHTML2.Enabled = True
        ErrLabel.Text = ("Don't forget to click the Confirm button to save changes!")
    End Sub

    Protected Sub CancelEditButton2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelEditButton2.Click
        btnEditHtml2.Visible = True

        CancelEditButton2.Visible = False
        FreeTextBox2.Value = ""
        FreeTextBox2.Visible = False
        SaveButton2.Visible = False
        TextHTML2.Enabled = True
    End Sub

    Public Sub Editdata3(ByVal source As Object, ByVal e As EventArgs) Handles btnEditHtml3.Click
        btnEditHtml3.Visible = False

        FreeTextBox3.Value = TextHTML3.Text
        FreeTextBox3.Visible = True
        SaveButton3.Visible = True
        CancelEditButton3.Visible = True
        TextHTML3.Enabled = False
    End Sub

    Protected Sub SaveButton3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveButton3.Click
        btnEditHtml3.Visible = True

        CancelEditButton3.Visible = False
        TextHTML3.Text = FreeTextBox3.Value
        FreeTextBox3.Visible = False
        SaveButton3.Visible = False
        TextHTML3.Enabled = True

        ErrLabel.Text = ("Don't forget to click the Confirm button to save changes!")
    End Sub

    Protected Sub SaveButton4_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveButton4.Click
        btnEditHtml4.Visible = True

        CancelEditButton4.Visible = False
        TextHTML4.Text = FreeTextBox4.Value
        FreeTextBox4.Visible = False
        SaveButton4.Visible = False
        TextHTML4.Enabled = True

        ErrLabel.Text = ("Don't forget to click the Confirm button to save changes!")
    End Sub
    Protected Sub CancelEditButton3_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles CancelEditButton3.Click
        btnEditHtml3.Visible = True

        CancelEditButton3.Visible = False
        FreeTextBox3.Value = ""
        FreeTextBox3.Visible = False
        SaveButton3.Visible = False
        TextHTML3.Enabled = True
    End Sub

    Protected Sub rptChangeProductStock_OnItemCommand(ByVal sender As Object, ByVal e As RepeaterCommandEventArgs) Handles rptChangeProductStock.ItemCommand
        If e.CommandName = "Update" Then
            Try
                Dim txtQuantity As TextBox = CType(e.Item.FindControl("txtQuantity"), TextBox), _
                    txtAvailableQuantity As TextBox = CType(e.Item.FindControl("txtAvailableQuantity"), TextBox), _
                    txtAllocatedQuantity As TextBox = CType(e.Item.FindControl("txtAllocatedQuantity"), TextBox), _
                    txtRestockCode As TextBox = CType(e.Item.FindControl("txtRestockCode"), TextBox), _
                    txtWarehouse As TextBox = CType(e.Item.FindControl("txtWarehouse"), TextBox)

                Dim quantity As Decimal = 0, _
                    availableQuantity As Decimal = 0, _
                    allocatedQuantity As Decimal = 0, _
                    restockCode As String = String.Empty, _
                    warehouse As String = String.Empty

                If Not String.IsNullOrEmpty(txtQuantity.Text) Then quantity = CDec(txtQuantity.Text)
                If Not String.IsNullOrEmpty(txtAvailableQuantity.Text) Then availableQuantity = CDec(txtAvailableQuantity.Text)
                If Not String.IsNullOrEmpty(txtAllocatedQuantity.Text) Then allocatedQuantity = CDec(txtAllocatedQuantity.Text)
                If Not String.IsNullOrEmpty(txtRestockCode.Text) Then restockCode = txtRestockCode.Text
                If Not String.IsNullOrEmpty(txtWarehouse.Text) Then warehouse = txtWarehouse.Text

                Dim affectedRows As Integer = 0
                affectedRows = TDataObjects.ProductsSettings.TblProductStock.Update(e.CommandArgument, quantity, availableQuantity, allocatedQuantity, restockCode, warehouse)
                If affectedRows = 1 Then
                    ErrLabel.Text = affectedRows & " product updated"
                Else
                    ErrLabel.Text = affectedRows & " products updated"
                End If
            Catch ex As Exception
                ErrLabel.Text = "There has been a problem updating the product stock. (" & ex.Message & ")"
            Finally
                bindStockRepeater()
            End Try
        End If
    End Sub

    Protected Sub btnUpdateProductPrice_OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdateProduct.Click
        Dim netPrice, grossPrice, taxAmount, _
                saleNetPrice, saleGrossPrice, saleTaxAmount, _
                deliveryNetPrice, deliveryGrossPrice, deliveryTaxAmount As Decimal
        Dim VATCodes As String = String.Empty
        Dim bError As Boolean = False
        Dim sErrorMessage As String = ""
        Dim ans As Integer = 0
        Dim affectedRows As Integer = 0

        ' 1. UPDATE THE PRODUCT DETAILS 
        If (Not IsWeightValid()) Then
            ErrLabel.Text = "Invalid value for weight. It supports decimal 18,3 value."
            Return
        End If

        ans = TDataObjects.ProductsSettings.TblProduct.UpdateProduct(product.Text.Trim, textDescription1.Text.Trim, textDescription2.Text.Trim, textDescription3.Text.Trim, textDescription4.Text.Trim, textDescription5.Text.Trim, TextHTML1.Text.Trim, TextHTML2.Text.Trim, TextHTML3.Text.Trim, TextHTML4.Text.Trim, textWeight.Text.Trim, textWeightUnit.Text.Trim, textGLCode1.Text.Trim, textGLCode2.Text.Trim, textGLCode3.Text.Trim, textGLCode4.Text.Trim, textGLCode5.Text.Trim, productID)

        If ans > 0 Then
            ErrLabel.Text = "Product details have been updated successfully"
        Else
            ErrLabel.Text = ("An error occurred when updating the product details. Please see your system administrator.")
            Return
        End If

        Try
            ' 2. UPDATE THE PRODUCT PRICE DETAILS 
            If Not String.IsNullOrEmpty(txtNetPrice.Text) Then netPrice = CDec(txtNetPrice.Text)
            If Not String.IsNullOrEmpty(txtGrossPrice.Text) Then grossPrice = CDec(txtGrossPrice.Text)
            If Not String.IsNullOrEmpty(txtTaxAmount.Text) Then taxAmount = CDec(txtTaxAmount.Text)
            If Not String.IsNullOrEmpty(txtSaleNetPrice.Text) Then saleNetPrice = CDec(txtSaleNetPrice.Text)
            If Not String.IsNullOrEmpty(txtSaleGrossPrice.Text) Then saleGrossPrice = CDec(txtSaleGrossPrice.Text)
            If Not String.IsNullOrEmpty(txtSaleTaxAmount.Text) Then saleTaxAmount = CDec(txtSaleTaxAmount.Text)
            If Not String.IsNullOrEmpty(txtDeliveryNetPrice.Text) Then deliveryNetPrice = CDec(txtDeliveryNetPrice.Text)
            If Not String.IsNullOrEmpty(txtDeliveryGrossPrice.Text) Then deliveryGrossPrice = CDec(txtDeliveryGrossPrice.Text)
            If Not String.IsNullOrEmpty(txtDeliveryTaxAmount.Text) Then deliveryTaxAmount = CDec(txtDeliveryTaxAmount.Text)
            If Not String.IsNullOrEmpty(ddlVATCodes.SelectedValue) Then VATCodes = ddlVATCodes.SelectedValue

            If Not IsNoPriceChanged() Then
                affectedRows = TDataObjects.ProductsSettings.TblPriceListDetail.Update(hdfPriceListDetailID.Value, _
                                netPrice, grossPrice, taxAmount, saleNetPrice, saleGrossPrice, saleTaxAmount, _
                                deliveryNetPrice, deliveryGrossPrice, deliveryTaxAmount, VATCodes)
                If affectedRows > 0 Then
                    ErrLabel.Text = ErrLabel.Text & "<br/>" & affectedRows & " product prices updated"
                Else
                    ErrLabel.Text = ErrLabel.Text & "<br/>" & ("An error occurred when updating the product prices. Please see your system administrator.")
                    Return
                End If
            End If
            ' redirect only of all went out successful !!
            Response.Redirect("ProductAdmin.aspx?updated=" + product.Text & "&" & GetQuerystrings())

        Catch ex As Exception
            ErrLabel.Text = IIf(ErrLabel.Text <> "", ErrLabel.Text & "<br/>", "") & "There has been a problem updating the product prices (" & ex.Message & ")"
        Finally
            bindPricesTable()
            'cleanup 
            netPrice = Nothing
            grossPrice = Nothing
            taxAmount = Nothing
            saleNetPrice = Nothing
            saleGrossPrice = Nothing
            saleTaxAmount = Nothing
            deliveryNetPrice = Nothing
            deliveryGrossPrice = Nothing
            deliveryTaxAmount = Nothing
            VATCodes = Nothing
            bError = Nothing
            sErrorMessage = Nothing
            ans = Nothing
            affectedRows = Nothing
        End Try
    End Sub

    Private Sub bindStockRepeater()
        Try
            plhChangeProductStock.Visible = CBool(ConfigurationManager.AppSettings("AmendProductStock"))
        Catch ex As Exception
            plhChangeProductStock.Visible = False
        End Try
        If plhChangeProductStock.Visible Then
            If Not String.IsNullOrEmpty(product.Text) Then
                Dim productStockTable As DataTable = TDataObjects.ProductsSettings.TblProductStock.GetStockByProductCode(product.Text, False)
                If productStockTable.Rows.Count > 0 Then
                    rptChangeProductStock.DataSource = productStockTable
                    rptChangeProductStock.DataBind()
                Else
                    plhChangeProductStock.Visible = False
                End If
            Else
                plhChangeProductStock.Visible = False
            End If
        End If
    End Sub

    Private Sub bindPricesTable()
        Try
            plhChangeProductPrices.Visible = CBool(ConfigurationManager.AppSettings("AmendProductPrices"))
        Catch ex As Exception
            plhChangeProductPrices.Visible = False
        End Try
        If plhChangeProductPrices.Visible Then
            If Not String.IsNullOrEmpty(product.Text) Then
                Dim productPricesTable As DataTable = TDataObjects.ProductsSettings.TblPriceListDetail.GetPricesByProductCode(product.Text, False)
                If productPricesTable.Rows.Count = 1 Then
                    Try
                        hdfPriceListDetailID.Value = productPricesTable.Rows(0)("PRICE_LIST_DETAIL_ID").ToString
                        ltlPriceList.Text = productPricesTable.Rows(0)("PRICE_LIST").ToString
                        txtNetPrice.Text = CDec(productPricesTable.Rows(0)("NET_PRICE").ToString)
                        txtGrossPrice.Text = CDec(productPricesTable.Rows(0)("GROSS_PRICE").ToString)
                        txtTaxAmount.Text = CDec(productPricesTable.Rows(0)("TAX_AMOUNT").ToString)
                        txtSaleNetPrice.Text = CDec(productPricesTable.Rows(0)("SALE_NET_PRICE").ToString)
                        txtSaleGrossPrice.Text = CDec(productPricesTable.Rows(0)("SALE_GROSS_PRICE").ToString)
                        txtSaleTaxAmount.Text = CDec(productPricesTable.Rows(0)("SALE_TAX_AMOUNT").ToString)
                        txtDeliveryNetPrice.Text = CDec(productPricesTable.Rows(0)("DELIVERY_NET_PRICE").ToString)
                        txtDeliveryGrossPrice.Text = CDec(productPricesTable.Rows(0)("DELIVERY_GROSS_PRICE").ToString)
                        txtDeliveryTaxAmount.Text = CDec(productPricesTable.Rows(0)("DELIVERY_TAX_AMOUNT").ToString)
                        If String.IsNullOrEmpty(Utilities.CheckForDBNull_String(productPricesTable.Rows(0)("TAX_CODE").ToString)) Then
                            ddlVATCodes.SelectedIndex = 0
                        Else
                            ddlVATCodes.SelectedValue = productPricesTable.Rows(0)("TAX_CODE").ToString
                        End If
                        AddPricesToViewState()
                    Catch ex As Exception
                        ErrLabel.Text = "An error has occurred retrieving price information. (" & ex.Message & ")"
                    End Try
                Else
                    plhChangeProductPrices.Visible = False
                End If
            Else
                plhChangeProductPrices.Visible = False
            End If
        End If
    End Sub
    Private Function GetQuerystrings() As String
        Dim linkQuerystring As String = String.Empty
        If Not String.IsNullOrWhiteSpace(Request.QueryString("BU")) Then
            linkQuerystring = "BU=" & Request.QueryString("BU").Trim.ToUpper
        Else
            linkQuerystring = "BU=*ALL"
        End If
        If Not String.IsNullOrWhiteSpace(Request.QueryString("Partner")) Then
            linkQuerystring = linkQuerystring & "&Partner=" & Request.QueryString("Partner").Trim.ToUpper
        Else
            linkQuerystring = linkQuerystring & "&Partner=*ALL"
        End If
        Return linkQuerystring
    End Function

    Private Sub AddPricesToViewState()
        ViewState.Add("txtNetPrice", txtNetPrice.Text)
        ViewState.Add("txtGrossPrice", txtGrossPrice.Text)
        ViewState.Add("txtTaxAmount", txtTaxAmount.Text)
        ViewState.Add("txtSaleNetPrice", txtSaleNetPrice.Text)
        ViewState.Add("txtSaleGrossPrice", txtSaleGrossPrice.Text)
        ViewState.Add("txtSaleTaxAmount", txtSaleTaxAmount.Text)
        ViewState.Add("txtDeliveryNetPrice", txtDeliveryNetPrice.Text)
        ViewState.Add("txtDeliveryGrossPrice", txtDeliveryGrossPrice.Text)
        ViewState.Add("txtDeliveryTaxAmount", txtDeliveryTaxAmount.Text)
        ViewState.Add("ddlVATCodes", ddlVATCodes.SelectedValue)
    End Sub

    Private Function IsNoPriceChanged() As Boolean
        Dim isPriceNotChanged As Boolean = True
        Try
            If isPriceNotChanged AndAlso (CDec(ViewState("txtNetPrice")) <> CDec(txtNetPrice.Text)) Then isPriceNotChanged = False
            If isPriceNotChanged AndAlso (CDec(ViewState("txtGrossPrice")) <> CDec(txtGrossPrice.Text)) Then isPriceNotChanged = False
            If isPriceNotChanged AndAlso (CDec(ViewState("txtTaxAmount")) <> CDec(txtTaxAmount.Text)) Then isPriceNotChanged = False
            If isPriceNotChanged AndAlso (CDec(ViewState("txtSaleNetPrice")) <> CDec(txtSaleNetPrice.Text)) Then isPriceNotChanged = False
            If isPriceNotChanged AndAlso (CDec(ViewState("txtSaleGrossPrice")) <> CDec(txtSaleGrossPrice.Text)) Then isPriceNotChanged = False
            If isPriceNotChanged AndAlso (CDec(ViewState("txtSaleTaxAmount")) <> CDec(txtSaleTaxAmount.Text)) Then isPriceNotChanged = False
            If isPriceNotChanged AndAlso (CDec(ViewState("txtDeliveryNetPrice")) <> CDec(txtDeliveryNetPrice.Text)) Then isPriceNotChanged = False
            If isPriceNotChanged AndAlso (CDec(ViewState("txtDeliveryGrossPrice")) <> CDec(txtDeliveryGrossPrice.Text)) Then isPriceNotChanged = False
            If isPriceNotChanged AndAlso (CDec(ViewState("txtDeliveryTaxAmount")) <> CDec(txtDeliveryTaxAmount.Text)) Then isPriceNotChanged = False
            If isPriceNotChanged AndAlso ViewState("ddlVATCodes") <> ddlVATCodes.SelectedValue Then isPriceNotChanged = False
        Catch ex As Exception
            isPriceNotChanged = True
        End Try
        Return isPriceNotChanged
    End Function

    Private Function IsWeightValid() As Boolean
        Dim value As Decimal = Nothing
        Return Decimal.TryParse(textWeight.Text.Trim, value)
    End Function

    Private Sub RetrieveVATCodes()
        Dim err As New ErrorObj
        Dim dt As New DataTable
        Dim utilitites As New TalentUtiltities
        Dim settings As DESettings = Utilities.GetSettingsObject()
        utilitites.DescriptionKey = "VATP"
        utilitites.Settings = settings
        err = utilitites.RetrieveDescriptionEntries()
        If utilitites.ResultDataSet Is Nothing OrElse utilitites.ResultDataSet.Tables("ErrorStatus").Rows(0)("ErrorOccurred") = GlobalConstants.ERRORFLAG Then
        Else
            dt = utilitites.ResultDataSet.Tables("DescriptionsEntries")
        End If
        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            dt.DefaultView.Sort = "CODE ASC"
            dt = dt.DefaultView.ToTable
            AddListOption(" ", " ")
            For Each row As DataRow In dt.Rows
                AddListOption(row("DESCRIPTION"), row("CODE"))
            Next
        End If

        ddlVATCodes.DataBind()

    End Sub
    Private Sub AddListOption(ByVal optionText As String, ByVal optionValue As String)
        Dim listItem As New ListItem
        listItem.Text = optionText.Trim
        listItem.Value = optionValue.Trim
        ddlVATCodes.Items.Add(listItem)
        listItem = Nothing
    End Sub
    Protected Sub btnDeleteProduct_Click(sender As Object, e As EventArgs) Handles btnDeleteProduct.Click
        Dim productOptions As DataTable = TDataObjects.ProductsSettings.TblProductOptions.GetProductOptionByMasterProduct(Request.QueryString("BU"), product.Text, False)
        If productOptions IsNot Nothing Then
            If productOptions.Rows.Count > 0 Then
                'product contains options
                ErrLabel.Text = TDataObjects.ProductsSettings.RemoveProductWithOptions(Request.QueryString("BU"), Request.QueryString("Partner"), product.Text)
            Else
                'product without options
                ErrLabel.Text = TDataObjects.ProductsSettings.RemoveProductWithoutOptions(Request.QueryString("BU"), Request.QueryString("Partner"), product.Text)
            End If

            If ErrLabel.Text.ToString.Equals("SUCCESS") Then
                Response.Redirect("~/Products/productAdmin.aspx?BU=" & Request.QueryString("BU") & "&Partner=" & Request.QueryString("Partner"))
            End If

        End If
    End Sub
End Class



