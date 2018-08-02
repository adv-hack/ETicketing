<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductListGraphical.ascx.vb"
    Inherits="UserControls_ProductListGraphical" %>
<asp:Panel ID="pnlProductListGraphical" runat="server">

    <asp:Label ID="lblError" runat="server" Text=""></asp:Label>

    <div class="panel ebiz-pagination-top-wrap">
        <div class="row">
            <div runat="server" id="pagerDisplayString" class="medium-3 columns ebiz-pager-display-wrap">
                <%=NowShowingResultsString("TOP")%>
            </div>
            <div class="medium-6 columns ebiz-pagination-outer-wrap">
                <div runat="server" id="pagerDisplayString2" class="ebiz-pagination-inner-wrap">
                    <div class="ebiz-pagination">
                        <%=PagingString("TOP")%>
                    </div>
                </div>
                <asp:Button ID="ShowAllButton" runat="server" Text="<%ShowAllButtonText%>" CssClass="button" />
            </div>
            <div class="medium-3 columns ebiz-sort-by-wrap">
                <asp:Label ID="txtSortBy" runat="server" Text="Sort By:" AssociatedControlID="orderddl"></asp:Label>
                <asp:DropDownList ID="orderddl" runat="server" AutoPostBack="true">
                </asp:DropDownList>
            </div>
        </div>
    </div>

    <asp:ValidationSummary ID="AddToBasketSummary" runat="server" ValidationGroup="AddToBasket" CssClass="alert-box alert" />

    <asp:Repeater runat="server" ID="rptProductListGraphical">
        <HeaderTemplate>
            <div class="row ebiz-product-list-graphical-wrap">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="column ebiz-graphical-product-item">
                <div class="panel">
                    <asp:HyperLink ID="ImageHyperLink" runat="server" ToolTip="<%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%>"
                        NavigateUrl="<%# FormatUrl(CType(Container.DataItem, Talent.eCommerce.Product).NavigateURL)%>"
                        Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>">
                            <asp:Image runat="server" CssClass="ebiz-product-image" ImageUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).ImagePath.Trim%>" Text="<%# CType(Container.DataItem, Talent.eCommerce.Product).AltText.Trim%>"/>
                    </asp:HyperLink>
                    <h2>
                        <asp:HyperLink ID="hypProductName" runat="server" NavigateUrl="<%# FormatUrl(CType(Container.DataItem, Talent.eCommerce.Product).NavigateURL)%>"
                            Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>"><%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%></asp:HyperLink>
                    </h2>
                    <div class="ebiz-long-description">
                        <%#CType(Container.DataItem, Talent.eCommerce.Product).Description2 %>
                    </div>
                    <asp:PlaceHolder ID="plhPriceInfo" runat="server" Visible="<%#ShowPrices%>">
                        <div class="ebiz-price">
                            <span class="ebiz-price-label">
                                <asp:Literal runat="server" ID="lblPriceText"></asp:Literal></span>
                            <span class="ebiz-price-amount">
                                <asp:Literal runat="server" ID="lblGrossPrice" Visible="false"></asp:Literal>
                                <asp:Literal runat="server" ID="lblSalePrice" Visible="false"></asp:Literal>
                            </span>                                                
                        </div>            
                    </asp:PlaceHolder>
                    <div class="ebiz-product-more-info">
                        <asp:HyperLink ID="MoreInfoButton" runat="server" CssClass="button"></asp:HyperLink>                        
                    </div>        
                    <asp:PlaceHolder ID="plhNoStock" runat="server">
                        <div class="ebiz-stock">
                            <asp:Literal ID="NoStockLabel" runat="server"></asp:Literal>
                        </div>
                    </asp:PlaceHolder>                    
                    <asp:PlaceHolder ID="plhAction" runat="server">
                        <div class="ebiz-add-to-basket-wrap">
                            <asp:TextBox ID="txtQuantity" runat="server" type="number" min="0"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="QuantityValidator" runat="server" ControlToValidate="txtQuantity"
                                ValidationExpression="^[0-9]{0,20}$" OnInit="GetRegExErrorText" ValidationGroup="AddToBasket"
                                Enabled="True" Display="Static" CssClass="error"></asp:RegularExpressionValidator>
                            <div class="ebiz-buy-wrap">
                                <asp:Button ID="btnBuy" runat="server" Text="" CommandArgument='<%#CType(Container.DataItem, Talent.eCommerce.Product).Code%>'
                                    CssClass="button" ValidationGroup="AddToBasket" CausesValidation="true" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <asp:Image ID="promoImage" runat="server" CssClass="ebiz-promotion-image" />
                </div>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
    <div class="panel ebiz-pagination-bottom-wrap">
        <div class="row">
            <div runat="server" id="pagerDisplayString3" class="medium-6 columns ebiz-pager-display-wrap">
                <%=NowShowingResultsString("BOTTOM")%>
            </div>
            <div class="medium-6 columns ebiz-pagination-outer-wrap">
                <div runat="server" id="pagerDisplayString4" class="ebiz-pagination-inner-wrap">
                    <div class="ebiz-pagination">
                        <%=PagingString("BOTTOM")%>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Panel>
