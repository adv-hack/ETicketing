<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductControls.ascx.vb" Inherits="UserControls_ProductControls" %>
<%@ Register Src="../UserControls/ProductOptions.ascx" TagName="ProductOptions" TagPrefix="Talent" %>

<asp:PlaceHolder ID="plhNormal" runat="server">
    <div class="panel ebiz-product-controls">
        <div class="row ebiz-sale-price">
            <div class="small-6 medium-4 columns">
                <asp:Label runat="server" ID="lblSalePriceText" Visible="false"></asp:Label>
            </div>
            <div class="small-6 medium-4 medium-pull-4 columns">
                <strong><asp:Label runat="server" ID="lblSalePrice" Visible="false"></asp:Label></strong>
            </div>
        </div>
        <div class="row ebiz-price">
            <div class="small-6 medium-4 columns">
                <asp:Label runat="server" ID="lblPriceText" CssClass="middle"></asp:Label>
            </div>
            <div class="small-6 medium-4 medium-pull-4 columns">
                <strong><asp:Label runat="server" ID="lblPrice"></asp:Label></strong>
            </div>
        </div>
        <p class="ebiz-out-of-stock">
            <asp:Literal ID="OutOfStockLabel" runat="server" Visible="false" />
        </p>
        <div runat="server" id="buyOptions">
            <div class="row ebiz-buy-options">
                <div class="small-6 medium-4 columns">
                    <asp:Label runat="server" ID="lblQuantity" AssociatedControlID="txtQuantity" CssClass="middle"></asp:Label>
                </div>
                <div class="small-6 medium-4 medium-pull-4 columns">
                    <asp:TextBox runat="server" ID="txtQuantity" OnPreRender="GetDefaultQuantity" type="number" min="0"></asp:TextBox>
                    <asp:CustomValidator ID="cvaldQuantityStock" Display="None" ControlToValidate="txtQuantity" runat="server" ValidationGroup="product" />
                    <asp:CustomValidator ID="cvaldNegative" Display="None" ControlToValidate="txtQuantity" runat="server" ValidationGroup="product" />
                    <asp:RegularExpressionValidator ID="QuantityValidator" runat="server" ControlToValidate="txtQuantity" ValidationExpression="^[0-9]{0,20}$" OnPreRender="GetRegExErrorText" ValidationGroup="product" Enabled="True" Display="None" />
                    <asp:RequiredFieldValidator ID="rfvQuantityValidator" runat="server" ControlToValidate="txtQuantity" OnPreRender="GetRfvErrorText" ValidationGroup="product" Enabled="true" Display="None" />
                </div>
            </div>
            <asp:Button ID="btnAddToBasket" runat="server" CssClass="button ebiz-add-to-basket" ValidationGroup="product" CausesValidation="true" />
        </div>
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plhOptions" runat="server">
    <Talent:ProductOptions ID="ProductOptions1" runat="server" />
</asp:PlaceHolder>
