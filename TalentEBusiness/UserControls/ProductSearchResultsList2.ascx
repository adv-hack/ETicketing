<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductSearchResultsList2.ascx.vb" Inherits="UserControls_ProductSearchResultsList2" %>
<asp:Panel ID="pnlProductSearchResultsList" runat="server">
        <asp:Label ID="lblSearchResults" runat="server" Text="" CssClass="results-string"></asp:Label>
    <p class="error">
        <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
    </p>
    <div class="sortddl-wrap">
        <asp:Label ID="txtSortBy" runat="server" Text="Sort By:"></asp:Label>
        <asp:DropDownList ID="orderddl" runat="server" AutoPostBack="true"></asp:DropDownList>
   </div>
    <div class="data-pager">
        <p class="pager-filter">

        </p>
        <p class="pager-display-nav">
            <span runat="server" id="pagerDisplayString" class="display">
                <%=nowShowingResultsString("TOP")%>
            </span><span runat="server" id="pagerDisplayString2" class="nav">
                <%=PagingString("TOP")%>                
            </span>            
            <span>
                <asp:Button ID="ShowAllButton" runat="server" Text="<%ShowAllButtonText%>" CssClass="Button"/>
            </span>
        </p>
        <p class="clear">
        </p>
    </div>
    <div class="graphical-product-list" id="graphicalView" runat="server">
        <asp:Panel ID="template1" runat="server">
        <asp:DataList runat="server" ID="dlsProductListGraphical" RepeatDirection="Horizontal">
            <ItemTemplate>
                <div class="wrapper">
                    <div class="container">
                        <div class="image">
                            <asp:HyperLink ID="ImageHyperLink" runat="server" 
                                ToolTip="<%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%>"
                                NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                                ImageUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).ImagePath.Trim%>"
                                Text="<%# CType(Container.DataItem, Talent.eCommerce.Product).AltText.Trim%>"
                                Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>">
                            </asp:HyperLink>
                        </div>
                        <div class="copy">
                            <p class="name">
                                <asp:HyperLink ID="hypProductName" runat="server" 
                                NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                                Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>"><%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%></asp:HyperLink></p>
                            <p class="description">
                                <%#CType(Container.DataItem, Talent.eCommerce.Product).Description2 %>
                            </p>
                            <p class="price">
                                <asp:Label runat="server" ID="lblPriceText"></asp:Label>
                                <asp:Label runat="server" ID="lblPrice"></asp:Label>
                            </p>
                            <p class="buy">
                                <span class="qty">
                                    <asp:TextBox ID="txtQuantity" CssClass="input-s" runat="server" min="0"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="QuantityValidator" runat="server"
                                     ControlToValidate="txtQuantity" ValidationExpression="^[0-9]{0,20}$" OnInit="GetRegExErrorText"
                                     ValidationGroup="AddToBasket" Text="*" Enabled="True" Display="Static"></asp:RegularExpressionValidator>
                                </span><span class="action">
                                    <asp:Label ID="NoStockLabel" runat="server"></asp:Label>
                                    <asp:Button ID="btnBuy" runat="server" Text="" CommandArgument='<%#CType(Container.DataItem, Talent.eCommerce.Product).code%>'
                                        CssClass="button" ValidationGroup="AddToBasket" CausesValidation="true" /></span></p>
                        </div>
                        <div class="promo-image">
                            <asp:Image ID="promoImage" runat="server" />
                        </div>
                    </div>
                    <p class="clearing">
                    </p>
                </div>
            </ItemTemplate>
        </asp:DataList>
        </asp:Panel>
        <asp:Panel ID="template3" runat="server">
        <asp:DataList runat="server" ID="dlsProductListGraphical2" RepeatDirection="Horizontal">
            <HeaderTemplate>
                <p class="image-header"><%#GetText("ProductImageHeaderText")%></p>
                <p class="name-header"><%#GetText("ProductNameHeaderText")%></p>
                <p class="brand-header"><%#GetText("ProductBrandHeaderText")%></p>
                <p class="pack-size-header"><%#GetText("ProductPackSizeHeaderText")%></p>
                <p class="qty-header"><%#GetText("ProductQuantityHeaderText")%></p>
                <p class="price-header"><%#GetText("ProductPriceHeaderText")%></p>
                <p class="buy-header"><%#GetText("ProductBuyHeaderText")%></p>
                <p class="promo-image-header"><%#GetText("ProductPromoHeaderText")%></p>
            </HeaderTemplate>
            <ItemTemplate>
                <div class="wrapper">
                    <div class="container">
                        <div class="image">
                            <asp:HyperLink ID="ImageHyperLink" runat="server" CssClass="code-link"
                                ToolTip="<%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%>"
                                NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                                ImageUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).ImagePath.Trim%>"
                                Text="<%# CType(Container.DataItem, Talent.eCommerce.Product).AltText.Trim%>"
                                Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>">
                            </asp:HyperLink>
                        </div>
                        <div class="copy">
                            <p class="name">
                                <asp:HyperLink ID="hypProductCode" runat="server" 
                                NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                                Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>"><%#CType(Container.DataItem, Talent.eCommerce.Product).Code.Trim%></asp:HyperLink></p>
                            <p class="description">
                                <%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%>
                            </p>
                            <div class="brand-col">
                                 <p class="brand">
                                    <%# CType(Container.DataItem, Talent.eCommerce.Product).Brand%>
                                </p>
                                <p class="html1">
                                    <%#CType(Container.DataItem, Talent.eCommerce.Product).HTML_1%>
                                </p>
                                <p class="html2">
                                    <%#CType(Container.DataItem, Talent.eCommerce.Product).HTML_2%>
                                </p>
                            </div>
                            <p class="pack-size">
                                <%#CType(Container.DataItem, Talent.eCommerce.Product).PackSize%>
                            </p>
                            <p class="qty">
                                <asp:TextBox ID="txtQuantity" CssClass="input-s" runat="server" min="0"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="QuantityValidator" runat="server" ControlToValidate="txtQuantity" 
                                    ValidationExpression="^[0-9]{0,20}$" OnInit="GetRegExErrorText" ValidationGroup="AddToBasket" Text="*" 
                                    Enabled="True" Display="Static"></asp:RegularExpressionValidator>
                            </p>
                            <p class="price">
                                <asp:Label runat="server" ID="lblPriceText"></asp:Label>
                                <asp:Label runat="server" ID="lblPrice"></asp:Label>
                            </p>
                            <p class="buy">
                                <span class="action">
                                    <asp:Label ID="NoStockLabel" runat="server"></asp:Label>
                                    <asp:Button ID="btnBuy" runat="server" Text="" CommandArgument='<%#CType(Container.DataItem, Talent.eCommerce.Product).code%>'
                                        CssClass="button" ValidationGroup="AddToBasket" CausesValidation="true" /></span></p>
                        </div>
                        <div class="promo-image">
                            <asp:Image ID="promoImage" runat="server" />
                        </div>
                    </div>
                    <p class="clearing">
                    </p>
                </div>
            </ItemTemplate>
        </asp:DataList>
        </asp:Panel>
    </div>
    <div class="data-pager">
        <p class="pager-filter">

        </p>
        <p class="pager-display-nav">
            <span runat="server" class="display" id="pagerDisplayString3">
                <%=nowShowingResultsString("BOTTOM")%>
            </span><span runat="server" class="nav" id="pagerDisplayString4">
                <%=PagingString("BOTTOM")%>
            </span>
        </p>
        <p class="clear">
        </p>
    </div>
</asp:Panel>