<%@ Control Language="VB" AutoEventWireup="false" CodeFile="GroupGraphical.ascx.vb"
    Inherits="UserControls_GroupGraphical" %>
<asp:Panel ID="pnlGroupGraphical" runat="server">
    <div class="graphical-pager">
        <p class="pager-display">
            <%=nowShowingResultsString()%>
        </p>
        <p class="pager-nav">
            <%=PagingString()%>
        </p>
        <p class="clear">
        </p>
    </div>
    <div class="group-graphical">
        <asp:DataList runat="server" ID="dlsGroupGraphical" RepeatDirection="Horizontal" summary="graphical group display">
            <ItemTemplate>
                <div class="wrapper">
                    <div class="container">
                        <div class="image">
                            <asp:Hyperlink ID="ImageHyperLink" runat="server" 
                                ToolTip="<%#CType(Container.DataItem, Talent.eCommerce.GroupLevelDetails.GroupDetails).GroupDescription1%>"
                                NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.groupleveldetails.groupdetails).GROUPnavigateURL%>"
                                ImageUrl="<%# CType(Container.DataItem, Talent.eCommerce.GroupLevelDetails.GroupDetails).GROUPImagePath%>"
                                Text="<%# CType(Container.DataItem, Talent.eCommerce.GroupLevelDetails.GroupDetails).GroupAltText %>">
                            </asp:Hyperlink>
                        </div>
                        <div class="copy">
                            <p class="name">
                                <asp:HyperLink ID="hypProductName" runat="server" 
                                NavigateUrl="<%# CType(Container.DataItem, Talent.eCommerce.groupleveldetails.groupdetails).groupnavigateURL%>"><%#CType(Container.DataItem, Talent.eCommerce.GroupLevelDetails.GroupDetails).GroupDescription1%></asp:HyperLink>
                            </p>
                            <p class="description">
                                <asp:Label ID="lblDescription2" runat="server" Text="<%# CType(Container.DataItem, Talent.eCommerce.GroupLevelDetails.GroupDetails).GroupDescription2%>"></asp:Label></p>
                        </div>
                    </div>
                    <p class="clearing">
                    </p>
                </div>
            </ItemTemplate>
        </asp:DataList>
    </div>
</asp:Panel>
