<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" EnableEventValidation="false"
    CodeFile="ProductDetails.aspx.vb" Inherits="Products_ProductDetails" validateRequest="false"%>
    <%@ Register Src="../UserControls/ProductMaintenanceTopNavigation.ascx" TagName="ProductMaintenanceTopNavigation"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <uc1:ProductMaintenanceTopNavigation ID="ProductMaintenanceTopNavigation1" runat="server" />
    <!-- tinyMCE -->

    <script language="javascript" type="text/javascript" src="mce/jscripts/tiny_mce/tiny_mce.js"></script>

    <script language="javascript" type="text/javascript">
        // Notice: The simple theme does not use all options some of them are limited to the advanced theme
        tinyMCE.init({
            theme: "advanced",
            mode: "textareas",
            plugins: "preview",
            theme_advanced_buttons3_add: "preview"
        });
    </script>

    <!-- /tinyMCE -->
    
    
    
    <p class="maint-title">
        <asp:Label ID="titleLabel" runat="server" />
    </p>
    
    <div id="change-product-descriptions">
        <p class="error">
            <asp:Label ID="ErrLabel" runat="server" Text="" CssClass="error"></asp:Label>
        </p>
        <div class="change-product-descriptions-summary-wrapper">
            <table cellspacing="0" class="change-product-descriptions-summary vertical">
                <tbody>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="IDLabel" runat="server" Text="Product ID" /></th>
                        <td class="element">
                            <asp:Label ID="ID" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="productLabel" runat="server" Text="Product Code" /></th>
                        <td class="element">
                            <asp:Label ID="product" runat="server" /></td>
                    </tr>                 
                </tbody>
            </table>
        </div>
        <div class="change-product-descriptions-data-summary">
            <table cellspacing="0" class="change-product-descriptions-data vertical">
                <tbody>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="description1Label" runat="server" Text="Product Description #1" />
                        </th>
                        <td class="element">
                            <asp:TextBox ID="textDescription1" runat="server" CssClass="input-s" MaxLength="100" Width="650" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="description2Label" runat="server" Text="Product Description #2" />
                        </th>
                        <td class="element">
                            <asp:TextBox ID="textDescription2" runat="server" CssClass="input-s" MaxLength="1000" Width="650" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="description3Label" runat="server" Text="Product Description #3" />
                        </th>
                        <td class="element">
                            <asp:TextBox ID="textDescription3" runat="server" CssClass="input-s" MaxLength="1000" Width="650" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="description4Label" runat="server" Text="Product Description #4" />
                        </th>
                        <td class="element">
                            <asp:TextBox ID="textDescription4" runat="server" CssClass="input-s" MaxLength="1000" Width="650" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="description5Label" runat="server" Text="Product Description #5" />
                        </th>
                        <td class="element">
                            <asp:TextBox ID="textDescription5" runat="server" CssClass="input-s" MaxLength="1000" Width="650" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="HTML1Label" runat="server" Text="Product HTML #1" />
                        </th>
                        <td class="element ftbControl">
                            <asp:TextBox ID="TextHTML1" runat="server" CssClass="input-l" TextMode="SingleLine" />
                            <asp:Button ID="btnEditHtml1" runat="server" CssClass="button" CommandName='<%#Eval("PRODUCT_ID") %>' Text="Edit" />
                            <div>
                                <textarea id="FreeTextBox1" runat="server" name="elm1" rows="30" cols="120"></textarea>
                                <asp:Button ID="SaveButton" runat="server" Text="Save Changes" CssClass="button" />
                                <asp:Button ID="CancelEditButton" runat="server" Text="Cancel" CssClass="button" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="HTML2Label" runat="server" Text="Product HTML #2" />
                        </th>
                        <td class="element ftbControl">
                            <asp:TextBox ID="TextHTML2" runat="server" CssClass="input-l" TextMode="SingleLine"></asp:TextBox>
                            <asp:Button ID="btnEditHtml2" runat="server" CssClass="button" CommandName='<%#Eval("PRODUCT_ID") %>' Text="Edit" />
                            <div>
                                <textarea id="FreeTextBox2" runat="server" name="elm2" rows="30" cols="120"></textarea><p>
                                <asp:Button ID="SaveButton2" runat="server" Text="Save Changes" CssClass="button" />
                                <asp:Button ID="CancelEditButton2" runat="server" Text="Cancel" CssClass="button" /></p>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="HTML3Label" runat="server" Text="Product HTML #3" />
                        </th>
                        <td class="element ftbControl">
                            <asp:TextBox ID="TextHTML3" runat="server" CssClass="input-l" TextMode="SingleLine"></asp:TextBox>
                            <asp:Button ID="btnEditHtml3" runat="server" CssClass="button" CommandName='<%#Eval("PRODUCT_ID") %>' Text="Edit" />
                            <div>
                                <textarea id="FreeTextBox3" runat="server" name="elm3" rows="30" cols="120"></textarea>
                                <asp:Button ID="SaveButton3" runat="server" Text="Save Changes" CssClass="button" />
                                <asp:Button ID="CancelEditButton3" runat="server" Text="Cancel" CssClass="button" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="weightLabel" runat="server" Text="Weight" />
                        </th>
                        <td class="element">
                            <table class="change-product-descriptions-data-glcodes">
                                <tr>
                                    <td>
                                        <asp:TextBox ID="textWeight" runat="server" CssClass="input-s" Width="150" />
                                        <asp:TextBox ID="textWeightUnit" runat="server" CssClass="input-s" Width="50" MaxLength="20" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="glcodeLabel" runat="server" Text="G/L Code" />
                        </th>
                        <td class="element">      
                            <table class="change-product-descriptions-data-glcodes">
                                <tr>
                                    <td><asp:TextBox ID="textGLCode1" runat="server" CssClass="input-s" MaxLength="4" /></td>
                                    <td><asp:TextBox ID="textGLCode2" runat="server" CssClass="input-s" MaxLength="4" /></td>
                                    <td><asp:TextBox ID="textGLCode3" runat="server" CssClass="input-s" MaxLength="4" /></td>
                                    <td><asp:TextBox ID="textGLCode4" runat="server" CssClass="input-s" MaxLength="4" /></td>
                                    <td><asp:TextBox ID="textGLCode5" runat="server" CssClass="input-s" MaxLength="4" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="Label1" runat="server" Text="Keywords" /></th>
                        <td class="element ftbControl">
                            <asp:TextBox ID="TextHTML4" runat="server" CssClass="input-l" TextMode="SingleLine"></asp:TextBox>
                            <asp:Button ID="btnEditHtml4" runat="server" CssClass="button" CommandName='<%#Eval("PRODUCT_ID") %>' Text="Edit" />
                            <div>
                                <textarea id="FreeTextBox4" runat="server" name="elm3" rows="30" cols="120"></textarea>
                                <asp:Button ID="SaveButton4" runat="server" Text="Save Changes" CssClass="button" />
                                <asp:Button ID="CancelEditButton4" runat="server" Text="Cancel" CssClass="button" />
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <asp:PlaceHolder ID="plhChangeProductStock" runat="server" Visible="False">
        <p class="title">Change Product Stock</p>
        <div id="change_product_stock">
            <asp:Repeater ID="rptChangeProductStock" runat="server">
                <HeaderTemplate>
                    <table cellspacing="0" class="change-product-stock horizontal">
                    <tbody>
                        <tr>
                            <th class="label" scope="col"><span>Stock Code</span></th>
                            <th class="label" scope="col"><span>Stock Location</span></th>
                            <th class="label" scope="col"><span>Quantity</span></th>
                            <th class="label" scope="col"><span>Available Quantity</span></th>
                            <th class="label" scope="col"><span>Allocated Quantity</span></th>
                            <th class="label" scope="col"><span>Restock Code</span></th>
                            <th class="label" scope="col"><span>Warehouse</span></th>
                            <th class="label" scope="col"><span>Update</span></th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                        </tr>
                            <td class="element" scope="col">
                                <span><%# DataBinder.Eval(Container.DataItem, "STOCK_LOCATION")%></span>
                            </td>
                            <td class="element" scope="col">
                                <span><%# DataBinder.Eval(Container.DataItem, "STOCK_LOCATION_DESC")%></span>
                            </td>
                            <td class="element" scope="col">
                                <span><asp:TextBox ID="txtQuantity" runat="server" CssClass="input-s" Text='<%# CDec(DataBinder.Eval(Container.DataItem, "QUANTITY"))%>' /></span>
                            </td>
                            <td class="element" scope="col">
                                <span><asp:TextBox ID="txtAvailableQuantity" runat="server" CssClass="input-s" Text='<%# CDec(DataBinder.Eval(Container.DataItem, "AVAILABLE_QUANTITY"))%>' /></span>
                            </td>
                            <td class="element" scope="col">
                                <span><asp:TextBox ID="txtAllocatedQuantity" runat="server" CssClass="input-s" Text='<%# CDec(DataBinder.Eval(Container.DataItem, "ALLOCATED_QUANTITY"))%>' /></span>
                            </td>
                            <td class="element" scope="col">
                                <span><asp:TextBox ID="txtRestockCode" runat="server" MaxLength="20" CssClass="input-s" Text='<%# DataBinder.Eval(Container.DataItem, "RESTOCK_CODE")%>' /></span>
                            </td>
                            <td class="element" scope="col">
                                <span><asp:TextBox ID="txtWarehouse" runat="server" MaxLength="20" CssClass="input-s" Text='<%# DataBinder.Eval(Container.DataItem, "WAREHOUSE")%>' /></span>
                            </td>
                            <td class="element" scope="col">
                                <span><asp:Button ID="btnUpdateStock" runat="server" Text="Update" CommandName="Update" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PRODUCT_STOCK_ID")%>' /></span>
                            </td>
                        </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                        </tr>
                            <td class="element" scope="col">
                                <span><%# DataBinder.Eval(Container.DataItem, "STOCK_LOCATION")%></span>
                            </td>
                            <td class="element" scope="col">
                                <span><%# DataBinder.Eval(Container.DataItem, "STOCK_LOCATION_DESC")%></span>
                            </td>
                            <td class="element" scope="col">
                                <span><asp:TextBox ID="txtQuantity" runat="server" CssClass="input-s" Text='<%# CDec(DataBinder.Eval(Container.DataItem, "QUANTITY"))%>' /></span>
                            </td>
                            <td class="element" scope="col">
                                <span><asp:TextBox ID="txtAvailableQuantity" runat="server" CssClass="input-s" Text='<%# CDec(DataBinder.Eval(Container.DataItem, "AVAILABLE_QUANTITY"))%>' /></span>
                            </td>
                            <td class="element" scope="col">
                                <span><asp:TextBox ID="txtAllocatedQuantity" runat="server" CssClass="input-s" Text='<%# CDec(DataBinder.Eval(Container.DataItem, "ALLOCATED_QUANTITY"))%>' /></span>
                            </td>
                            <td class="element" scope="col">
                                <span><asp:TextBox ID="txtRestockCode" runat="server" MaxLength="20" CssClass="input-s" Text='<%# DataBinder.Eval(Container.DataItem, "RESTOCK_CODE")%>' /></span>
                            </td>
                            <td class="element" scope="col">
                                <span><asp:TextBox ID="txtWarehouse" runat="server" MaxLength="20" CssClass="input-s" Text='<%# DataBinder.Eval(Container.DataItem, "WAREHOUSE")%>' /></span>
                            </td>
                            <td class="element" scope="col">
                                <span><asp:Button ID="btnUpdateStock" runat="server" Text="Update" CommandName="Update" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PRODUCT_STOCK_ID")%>' /></span>
                            </td>
                        </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                    </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhChangeProductPrices" runat="server" Visible="False">
        <p class="title">Change Product Prices</p>
        <div id="change_product_prices">
            <table cellspacing="0" class="change-product-prices vertical">
                <tbody>
                    <tr>
                        <th class="label" scope="row">Price List</th>
                        <td class="element"><asp:Literal ID="ltlPriceList" runat="server" /></td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">Net Price</th>
                        <td class="element">
                            <asp:TextBox ID="txtNetPrice" runat="server" CssClass="input-s" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">Gross Price</th>
                        <td class="element">
                            <asp:TextBox ID="txtGrossPrice" runat="server" CssClass="input-s" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">Tax Amount</th>
                        <td class="element">
                            <asp:TextBox ID="txtTaxAmount" runat="server" CssClass="input-s" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">Sale Net Price</th>
                        <td class="element">
                            <asp:TextBox ID="txtSaleNetPrice" runat="server" CssClass="input-s" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">Sale Gross Price</th>
                        <td class="element">
                            <asp:TextBox ID="txtSaleGrossPrice" runat="server" CssClass="input-s" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">Sale Tax Amount</th>
                        <td class="element">
                            <asp:TextBox ID="txtSaleTaxAmount" runat="server" CssClass="input-s" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">Delivery Net Price</th>
                        <td class="element">
                            <asp:TextBox ID="txtDeliveryNetPrice" runat="server" CssClass="input-s" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">Delivery Gross Price</th>
                        <td class="element">
                            <asp:TextBox ID="txtDeliveryGrossPrice" runat="server" CssClass="input-s" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">Delivery Tax Amount</th>
                        <td class="element">
                            <asp:TextBox ID="txtDeliveryTaxAmount" runat="server" CssClass="input-s" />
                        </td>
                    </tr>
                                        <tr>
                        <th class="label" scope="row">VAT Codes</th>
                        <td>
                            <asp:DropDownList ID ="ddlVATCodes" runat="server" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div id="change_product_price_confirm_button">
            <asp:PlaceHolder ID="plhDeleteProducts" runat="server" Visible="false">
                <asp:Button ID="btnDeleteProduct" runat="server" Text="Delete Product" />
            </asp:PlaceHolder>
            <asp:Button ID="btnUpdateProduct" runat="server" Text="Update Product" />&nbsp;&nbsp;
            <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="False" CssClass="button" />
            <asp:HiddenField ID="hdfPriceListDetailID" runat="server" />
        </div>
    </asp:PlaceHolder>
    
</asp:Content>