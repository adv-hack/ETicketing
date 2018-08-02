<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductRelationsGraphical2.ascx.vb" Inherits="UserControls_ProductRelationsGraphical2" %>
<asp:Panel ID="pnlProductRelationsGraphical" runat="server">
    <div class="ProductRelationsGraphical2_ascx product-relations-graphical graphical-product-list">
        <div class="graphical-relationship-header">
            <h2>
                <asp:Label ID="lblHeaderText" runat="server"></asp:Label>
            </h2>
        </div>
        <div>
            <asp:ValidationSummary ID="AddToBasketSummary" runat="server" ValidationGroup="AddToBasket" />
            <br />
            <asp:Label ID="lblError" runat="server" CssClass="error"></asp:Label></div>
            <span>
                <asp:Button ID="ShowAllButton" runat="server" Text="<%ShowAllButtonText%>"/>
            </span>
        <asp:DataList runat="server" ID="dlsProductRelationsGraphical" RepeatDirection="Horizontal">
            <ItemTemplate>
                <div class="wrapper">
                    <div class="container">
                        <div class="image">
                            <asp:HyperLink ID="ImageHyperLink" runat="server" 
                                ToolTip="<%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%>"
                                NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                                ImageUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).ImagePath.Trim%>"
                                Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>">
                            </asp:HyperLink>
                        </div>
                        <div class="copy">
                            <p class="name">
                                <asp:HyperLink ID="hypProductName" runat="server" 
                                NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                                Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>"><%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%></asp:HyperLink>
                            </p>
                            <p class="description">
                                <asp:Label ID="lblProductDescription" runat="server" Text="<%#CType(Container.DataItem, Talent.eCommerce.Product).Description2 %>"></asp:Label> 
                            </p>
                            <p class="price">
                                <asp:Label ID="lblPriceText" runat="server"></asp:Label>
                                <asp:Label ID="lblPrice" runat="server"></asp:Label>
                            </p>
                            <p class="buy">
                                <span class="qty">
                                    <asp:TextBox ID="txtQuantity" CssClass="input-s" runat="server" OnPreRender="GetDefaultQuantity" min="0"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="QuantityValidator" runat="server"
                                     ControlToValidate="txtQuantity" ValidationExpression="^[0-9]{0,20}$" OnPreRender="GetRegExErrorText"
                                     ValidationGroup="AddToBasket" Text="*" Enabled="True" Display="Static"></asp:RegularExpressionValidator>
                                </span>
                                <span class="action">
                                    <asp:Label ID="NoStockLabel" runat="server"></asp:Label>
                                    <asp:Button ID="btnBuy" runat="server" CausesValidation="true" ValidationGroup="AddToBasket" Text="" CommandArgument='<%#CType(Container.DataItem, Talent.eCommerce.Product).code%>' CssClass="button"/>
                                </span>
                            </p>
                            <div class="promo-image">
                            <asp:Image ID="promoImage" runat="server" />
                        </div>
                        </div>
                    </div>
                    <p class="clearing"></p>
                </div>
            </ItemTemplate>
        </asp:DataList>
    </div>
</asp:Panel>