<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductSearchResultsList.ascx.vb"
    Inherits="UserControls_ProductSearchResultsList" %>
<asp:Panel ID="pnlProductSearchResultsList" runat="server">
    <div class="ebiz-search-results-label">
        <asp:Literal ID="lblSearchResults" runat="server" Text=""></asp:Literal>
    </div>
    <asp:Label ID="txtSortBy" runat="server" Text="Sort By:" Visible="false"></asp:Label>
    <asp:Literal ID="lblError" runat="server" Text=""></asp:Literal>
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
                <asp:Button ID="ShowAllButton" runat="server" Text="<%ShowAllButtonText%>"  CssClass="button"/>
            </div>
            <div class="medium-3 columns ebiz-sort-by-wrap">
                <asp:Label ID="Label1" runat="server" Text="Sort By:" AssociatedControlID="orderddl"></asp:Label>
                <asp:DropDownList ID="orderddl" runat="server" AutoPostBack="true">
                </asp:DropDownList>
            </div>
        </div>
    </div>
    <asp:Repeater ID="rptProducts" runat="server" >
        <HeaderTemplate>
            <div class="row ebiz-product-list-graphical-wrap">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="column ebiz-graphical-product-item">
                    <div class="panel">
                        <asp:HyperLink ID="HyperLink1" runat="server" ToolTip="<%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%>"
                            NavigateUrl="<%# FormatUrl(CType(Container.DataItem, Talent.eCommerce.Product).navigateURL)%>"
                            Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>">
                            <asp:Image ID="Image1" runat="server" CssClass="ebiz-image" ImageUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).ImagePath.Trim%>" Text="<%# CType(Container.DataItem, Talent.eCommerce.Product).AltText.Trim%>"/>
                        </asp:HyperLink>
                        <h2>
                            <asp:HyperLink ID="hypProductName" runat="server" NavigateUrl="<%# FormatUrl(CType(Container.DataItem, Talent.eCommerce.Product).navigateURL)%>"
                                Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>"><%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%></asp:HyperLink>
                        </h2>
                        <div class="ebiz-long-description">
                            <%#CType(Container.DataItem, Talent.eCommerce.Product).Description2 %>
                        </div>
                        <div class="ebiz-price">
                            <asp:Literal runat="server" ID="lblPriceText" />
                            <asp:Literal runat="server" ID="lblPrice" />
                        </div>
                        <asp:PlaceHolder ID="plhNoStock" runat="server">
                            <div class="ebiz-stock">
                                <asp:Literal ID="NoStockLabel" runat="server" />
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhAction" runat="server">
                            <div class="ebiz-add-to-basket-wrap">
                                <asp:TextBox ID="txtQuantity" runat="server" type="number" min="0"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="QuantityValidator" runat="server" ControlToValidate="txtQuantity"
                                    ValidationExpression="^[0-9]{0,20}$" OnInit="GetRegExErrorText" ValidationGroup="AddToBasket"
                                    Enabled="True" Display="Static" CssClass="error"></asp:RegularExpressionValidator>
                                <div class="ebiz-buy-wrap">
                                    <asp:Button ID="btnBuy" runat="server" Text="" CommandArgument='<%#CType(Container.DataItem, Talent.eCommerce.Product).code%>'
                                        CssClass="button" ValidationGroup="AddToBasket" CausesValidation="true" />
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <asp:Image ID="promoImage" runat="server" CssClass="ebiz-promotion" />
                    </div>
                </div>
        </ItemTemplate>
          
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>

    <div class="panel ebiz-pagination-bottom-wrap">
        <div class="row">
            <div runat="server" id="pagerDisplayString3"  class="medium-6 columns ebiz-pager-display-wrap">
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
