<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductListGraphical2.ascx.vb" Inherits="UserControls_ProductListGraphical2" %>


    <div class="panel ebiz-pager ebiz-top-pager ebiz-retail-pager">
        <div class="row">
            <div class="large-8 columns">
                <asp:PlaceHolder ID="plhPagerTop" runat="server">
                    <ul class="pagination">
                        <asp:Literal ID="ltlNowShowingResultsTop" runat="server" />
                        <asp:Literal ID="ltlPagingTop" runat="server" />
                        <li>    
                            <asp:PlaceHolder ID="plhPagerTopShowAll" runat="server">
                                <asp:Button ID="ShowAllButton" runat="server" Text="<%ShowAllButtonText%>" CssClass="button ebiz-show-all" />
                            </asp:PlaceHolder>
                        </li>
                    </ul>
                </asp:PlaceHolder> 
            </div>
            <div class="small-3 large-1 columns">
                <asp:Label ID="txtSortBy" runat="server" AssociatedControlID="orderddl" CssClass="inline ebiz-sort-by-label" />
            </div>
            <div class="small-9 large-3 columns">
                <asp:DropDownList ID="orderddl" runat="server" AutoPostBack="true" CssClass="ebiz-sort-by-options" />
            </div>
        </div>
    </div>


    
        <asp:PlaceHolder ID="template1" runat="server">
            <asp:Repeater runat="server" ID="rptProductListGraphical">
                <HeaderTemplate>
                    <ul class="<%=BlockGridStyleClass %> ebiz-product-list-graphical">
                </HeaderTemplate>
                <ItemTemplate>
                    <li>
                        <div class="panel">
                            <asp:HyperLink ID="HyperLink1" runat="server" ToolTip="<%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%>"
                                NavigateUrl="<%# FormatUrl(CType(Container.DataItem, Talent.eCommerce.Product).navigateURL)%>"
                                Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>">
                                <asp:Image runat="server" CssClass="ebiz-image" ImageUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).ImagePath.Trim%>" Text="<%# CType(Container.DataItem, Talent.eCommerce.Product).AltText.Trim%>"/>
                            </asp:HyperLink>
                            <h2>
                                
                                    <asp:HyperLink ID="hypProductName" runat="server" NavigateUrl="<%# FormatUrl(CType(Container.DataItem, Talent.eCommerce.Product).navigateURL)%>"
                                        Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>"><%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%></asp:HyperLink>
                                
                            </h2>
                            <p class="ebiz-long-description">
                                <%#CType(Container.DataItem, Talent.eCommerce.Product).Description2 %>
                            </p>
                            <p class="ebiz-price">
                                <asp:Literal runat="server" ID="lblPriceText" />
                                <strong><asp:Literal runat="server" ID="lblPrice" /></strong>
                            </p>
                            <asp:PlaceHolder ID="plhNoStock" runat="server">
                                <p class="ebiz-stock"><asp:Literal ID="NoStockLabel" runat="server" /></p>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhAction" runat="server">
                                <div class="ebiz-action">
                                    <asp:TextBox ID="txtQuantity" runat="server" type="number" min="0" />
                                    <asp:RegularExpressionValidator ID="QuantityValidator" runat="server" ControlToValidate="txtQuantity" CssClass="error"
                                        ValidationExpression="^[0-9]{0,20}$" OnInit="GetRegExErrorText" ValidationGroup="AddToBasket" Enabled="True" Display="Static" />
                                    <asp:Button ID="btnBuy" runat="server" CommandArgument='<%#CType(Container.DataItem, Talent.eCommerce.Product).code%>'
                                        CssClass="button" ValidationGroup="AddToBasket" CausesValidation="true" />
                                </div>
                            </asp:PlaceHolder>
                            <asp:Image ID="promoImage" runat="server" CssClass="ebiz-promotion" />
                        </div>
                    </li>
                </ItemTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
            </asp:Repeater>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="template3" runat="server">
            <asp:Repeater runat="server" ID="rptProductListGraphical2">
                <HeaderTemplate>
                    <p class="image-header">
                        <%#GetText("ProductImageHeaderText")%>
                    </p>
                    <p class="name-header">
                        <%#GetText("ProductNameHeaderText")%>
                    </p>
                    <p class="brand-header">
                        <%#GetText("ProductBrandHeaderText")%>
                    </p>
                    <p class="pack-size-header">
                        <%#GetText("ProductPackSizeHeaderText")%>
                    </p>
                    <p class="qty-header">
                        <%#GetText("ProductQuantityHeaderText")%>
                    </p>
                    <p class="price-header">
                        <%#GetText("ProductPriceHeaderText")%>
                    </p>
                    <p class="buy-header">
                        <%#GetText("ProductBuyHeaderText")%>
                    </p>
                    <p class="promo-image-header">
                        <%#GetText("ProductPromoHeaderText")%>
                    </p>
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="wrapper">
                        <div class="container">
                            <div class="image">
                                <asp:HyperLink ID="ImageHyperLink" runat="server" ToolTip="<%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%>"
                                    NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                                    ImageUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).ImagePath.Trim%>"
                                    Text="<%# CType(Container.DataItem, Talent.eCommerce.Product).AltText.Trim%>"
                                    Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>">
                                </asp:HyperLink>
                            </div>
                            <div class="copy">
                                <p class="name">
                                    <asp:HyperLink ID="hypProductCode" runat="server" NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
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
                                    <asp:TextBox ID="txtQuantity" CssClass="input-s" runat="server"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="QuantityValidator" runat="server" ControlToValidate="txtQuantity"
                                        ValidationExpression="^[0-9]{0,20}$" OnInit="GetRegExErrorText" ValidationGroup="AddToBasket"
                                        Text="*" Enabled="True" Display="Static"></asp:RegularExpressionValidator>
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
            </asp:Repeater>
        </asp:PlaceHolder>

    
    <asp:PlaceHolder ID="plhPagerBottom" runat="server">
        <div class="panel pagination-centered ebiz-pager ebiz-bottom-pager">
            <ul class="pagination">
                <asp:Literal ID="ltlNowShowingResultsBottom" runat="server" />
                <asp:Literal ID="ltlPagingBottom" runat="server" />
            </ul>
        </div>
    </asp:PlaceHolder>

<script>
    $(document).ready(function() {

        var w = document.body.clientWidth;
            if (w > 641) {

                $(window).load(function() {

                    // set image height
                    subheaders = $('.ebiz-product-list-graphical .panel > a'); 
                    maxHeight = Math.max.apply(
                    Math, subheaders.map(function() {
                      return $(this).height();
                    }).get());
                    subheaders.height(maxHeight);

                    // set panel height
                    boxes = $('.ebiz-product-list-graphical .panel');
                    maxHeight = Math.max.apply(
                    Math, boxes.map(function() {
                      return $(this).height();
                    }).get());
                    boxes.height(maxHeight);
 

            });

        };

    });
</script>