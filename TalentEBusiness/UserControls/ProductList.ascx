<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductList.ascx.vb"
    Inherits="UserControls_ProductList" %>
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
    <div class="data-product-list">
        <asp:Repeater ID="rptProducts" runat="server">
            <HeaderTemplate>
                <table summary="Data Product List" cellspacing="0">
                    <tr>
                        <th scope="col" class="image">
                            &nbsp;</th>
                        <th scope="col" class="name">
                            <asp:Label runat="server" ID="colHeader1" /></th>
                        <th scope="col" class="style">
                            <asp:Label runat="server" ID="colHeader2" /></th>
                        <th scope="col" class="country">
                            <asp:Label runat="server" ID="colHeader3" /></th>
                        <th scope="col" class="price">
                            <asp:Label runat="server" ID="colHeader4" /></th>
                        <th scope="col" class="action">
                            &nbsp;</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr class="row">
                    <td class="image">
                        <asp:HyperLink ID="ImageHyperLink" runat="server"  
                            ToolTip="<%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%>"
                            NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                            ImageUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).ImagePath.Trim%>" 
                            Text="<%#CType(Container.DataItem, Talent.eCommerce.Product).altText.Trim%>"
                            Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>" >                                                       
                        </asp:HyperLink></td>
                    <td class="name">
                        <p class="name-name">
                        <asp:HyperLink ID="hypProductName" runat="server" 
                            NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                            Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>">
                            <%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%> </asp:HyperLink></p>
                        <p class="name-description">
                            <%#CType(Container.DataItem, Talent.eCommerce.Product).Description2%>
                        </p>
                    </td>
                    <td class="style">
                        <%#CType(Container.DataItem, Talent.eCommerce.Product).Colour%>
                    </td>
                    <td class="country">
                        <%#CType(Container.DataItem, Talent.eCommerce.Product).country%>
                    </td>
                    <td class="price">
                        <asp:Label ID="lblPrice" runat="server"></asp:Label>
                    </td>
                    <td class="action">
                        <asp:Label ID="NoStockLabel" runat="server"></asp:Label>
                        <asp:Button ID="btn_buy" runat="server" Text="Add to BasketHC" CommandArgument='<%#CType(Container.DataItem, Talent.eCommerce.Product).code %>'
                            CssClass="button" /></td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="alternativeRow">
                    <td class="image">
                        <asp:HyperLink ID="ImageHyperLink" runat="server" 
                            ToolTip="<%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%>"
                            NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                            ImageUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).ImagePath.Trim%>"
                             Text="<%#CType(Container.DataItem, Talent.eCommerce.Product).altText.Trim%>"
                             Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>">
                        </asp:HyperLink></td>
                    <td class="name">
                        <p class="name-name">
                         <asp:HyperLink ID="hypProductName" runat="server" 
                            NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.Product).navigateURL%>"
                            Enabled="<%# CType(Container.DataItem, Talent.eCommerce.Product).LinkEnabled%>"><%#CType(Container.DataItem, Talent.eCommerce.Product).Description1.Trim%></asp:HyperLink></p>
                        <p class="name-description">
                            <%#CType(Container.DataItem, Talent.eCommerce.Product).Description2%>
                        </p>
                    </td>
                    <td class="style">
                        <%#CType(Container.DataItem, Talent.eCommerce.Product).Colour%>
                    </td>
                    <td class="country">
                        <%#CType(Container.DataItem, Talent.eCommerce.Product).country%>
                    </td>
                    <td class="price">
                        <asp:Label ID="lblPrice" runat="server"></asp:Label>
                    </td>
                    <td class="action">
                        <asp:Label ID="NoStockLabel" runat="server"></asp:Label>
                        <asp:Button ID="btn_buy" runat="server" Text="Add to BasketHC" CommandArgument='<%#CType(Container.DataItem, Talent.eCommerce.Product).code%>'
                            CssClass="button" /></td>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
    </div>
      <div class="data-pager">
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
