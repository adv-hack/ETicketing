<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductList2.ascx.vb" Inherits="UserControls_ProductList2" %>
<%@ Register Src="~/usercontrols/productoptions.ascx" TagName="ProductOptions" TagPrefix="Talent" %>
<asp:Panel ID="pnlProductList" runat="server">
    <p class="error">
        <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
    </p>
    <div class="data-pager">
        <p class="pager-filter">
            <asp:Label ID="txtSortBy" runat="server" Text="Sort ByHC:"></asp:Label>
            <asp:DropDownList ID="ddlSortBy" runat="server" AutoPostBack="true">
            </asp:DropDownList>
        </p>
       <%-- <p class="pager-display-nav">
            <span class="display">
                <%=nowShowingResultsString("TOP")%>
            </span><span class="nav">
                <%=PagingString("TOP")%>
            </span>
        </p>--%>
        <p class="clear">
        </p>
    </div>
    <div class="data-product-list">
        <asp:Repeater ID="rptProducts" runat="server">
            <HeaderTemplate>
            </HeaderTemplate>
            <ItemTemplate>
                <div class="row">
                    <div class="image">
                        <asp:HyperLink ID="ImageHyperLink" runat="server"  
                            ToolTip="<%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%>"
                            NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                            ImageUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).ImagePath.Trim%>" 
                            Text="<%#CType(Container.DataItem, Talent.eCommerce.Product).altText.Trim%>"
                            Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>">
                        </asp:HyperLink></div>
                    <div class="name">
                        <p class="name-name">
                        <asp:HyperLink ID="productName" runat="server"  
                            NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                            Text="<%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%>"
                            Enabled="<%#CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>">
                        </asp:HyperLink>
                            </p>
                        <p class="name-description">
                            <%#CType(Container.DataItem, Talent.eCommerce.Product).Description2%>
                        </p>
                    </div>
                    <div id="buyButtonPanel" runat="server" class="action">
                        <asp:Label ID="NoStockLabel" runat="server"></asp:Label>
                        <asp:Button ID="btn_buy" runat="server" Text="Add to BasketHC" CommandArgument='<%#CType(Container.DataItem, Talent.eCommerce.Product).code%>'
                            CssClass="button" /></div>
                          
                    <div>
                        <asp:HiddenField  runat="server" ID="productCodeHidden" Value='<%#CType(Container.DataItem, Talent.eCommerce.Product).code%>'/>
                        <Talent:ProductOptions ID="ProductOptions1" runat="server" AutoPopulate="false" MasterProduct="<%# CType(Container.DataItem, Talent.eCommerce.Product).Code%>" />
                    </div>
                </div>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <div class="alternativeRow">
                    <div class="image">
                        <asp:HyperLink ID="ImageHyperLink" runat="server"  
                            ToolTip="<%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%>"
                            NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                            ImageUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).ImagePath.Trim%>" 
                            Text="<%#CType(Container.DataItem, Talent.eCommerce.Product).altText.Trim%>"
                            Enabled="<%#CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>">
                        </asp:HyperLink></div>
                    <div class="name">
                        <p class="name-name">
                        <asp:HyperLink ID="productName" runat="server"  
                            NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                            Text="<%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%>"
                            Enabled="<%#CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>">
                        </asp:HyperLink>
                        </p>
                        <p class="name-description">
                            <%#CType(Container.DataItem, Talent.eCommerce.Product).Description2%>
                        </p>
                    </div>
                    <div id="buyButtonPanel" runat="server" class="action">
                        <asp:Label ID="NoStockLabel" runat="server"></asp:Label>
                        <asp:Button ID="btn_buy" runat="server" Text="Add to BasketHC" CommandArgument='<%#CType(Container.DataItem, Talent.eCommerce.Product).code%>'
                            CssClass="button" /></div>
                    <div>
                        <asp:HiddenField  runat="server" ID="productCodeHidden" Value='<%#CType(Container.DataItem, Talent.eCommerce.Product).code%>' />
                        <Talent:ProductOptions ID="ProductOptions1" runat="server" AutoPopulate="false" MasterProduct="<%# CType(Container.DataItem, Talent.eCommerce.Product).Code%>" />
                    </div>
                </div>
            </AlternatingItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:Repeater>
    </div>
    <%--<div class="data-pager">
        <p class="pager-display-nav">
            <span class="display">
                <%=nowShowingResultsString("BOTTOM")%>
            </span><span class="nav">
                <%=PagingString("BOTTOM")%>
            </span>
        </p>
        <p class="clear">
        </p>
    </div>--%>
</asp:Panel>