<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AgentPersonalisation.ascx.vb" Inherits="UserControls_AgentPersonalisation" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/Alerts.ascx" TagName="Alerts" TagPrefix="Talent" %>

<asp:PlaceHolder ID="plhNotLoggedIn" runat="server">
    <div class="top-bar">
        <div class="top-bar-left">
            <ul class="dropdown menu" data-dropdown-menu data-close-on-click-inside="false" data-hover-delay="200" data-closing-time="200">
                <li class="menu-text ebiz-TALENT">TALENT</li>
                <li class="ebiz-agent-log-in">
                    <asp:HyperLink ID="hplLogin" runat="server" NavigateUrl="~/PagesPublic/Agent/AgentLogin.aspx">
                        <asp:Literal ID="ltlLogin" runat="server" />
                    </asp:HyperLink>
                </li>
            </ul>
        </div>
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plhLoggedIn" runat="server">
    <div class="top-bar ebiz-customer-agent-nav-wrap">
        <div class="top-bar-left">
            <nav>
                <ul class="dropdown menu" data-dropdown-menu data-close-on-click-inside="false" data-hover-delay="200" data-closing-time="200">
                    <li class="menu-text ebiz-TALENT"><img src="../../App_Themes/BUI/img/Advanced.icon.svg" alt="Advanced" title="Advanced" class="ebiz-advanced-icon"></li>
                    <li class="ebiz-show-off-canvas-left"><a href="#" data-toggle="offCanvasLeft"><i class="fa fa-indent fa-fw"></i></a></li>
                    <%= GetStaticHTMLInclude("extra-links-left.htm")%>
                    <asp:PlaceHolder ID="plhCustomerDetails" runat="server">
                        <li class="has-submenu ebiz-customer-menu">
                            <a href="#">
                                <i class="fa fa-user fa-lg"></i> 
                                <asp:Literal ID="ltlCustomerDetails" runat="server" />
                            </a>
                            <%= GetStaticHTMLInclude("customer-menu.htm")%>
                        </li>                        
                        <li class="ebiz-customer-menu ebiz-stop-code">
                            <asp:Literal ID="ltlStopCodeDetails" runat="server" />
                        </li>
                    </asp:PlaceHolder>
                    <Talent:Alerts ID="uscAlerts" runat="server" />
                    <%= GetStaticHTMLInclude("agent-links.htm")%>
                    <%= GetStaticHTMLInclude("extra-links-right.htm")%>
                </ul>
            </nav>
        </div>
        <div class="top-bar-right">
            <nav>
                <ul class="dropdown menu align-right" data-dropdown-menu data-close-on-click-inside="false" data-hover-delay="200" data-closing-time="200">
                    <li class="ebiz-show-off-canvas-right"><a href="#" data-toggle="offCanvasRight"><i class="fa fa-outdent fa-fw"></i></a></li>
                    <li class="has-submenu ebiz-agent-menu">
                        <a href="#">
                            <i class="fa fa-users" aria-hidden="true"></i> 
                            <asp:Literal ID="ltlLoggedInAgent" runat="server" />
                        </a>
                        <%= GetStaticHTMLInclude("agent-menu.htm")%>
                    </li>

                    <li class="ebiz-default-printer">
                        <a runat="server" data-open="printerSelectionWindow">
                            <i class="fa fa-print" aria-hidden="true"></i><%=ddlPrinterList.SelectedItem %>
                        </a>
                    </li>

                    <li class="ebiz-basket-items">
                        <asp:HyperLink ID="hplBasket" runat="server" NavigateUrl="~/PagesPublic/Basket/Basket.aspx">
                            <i class="fa fa-shopping-basket" aria-hidden="true"></i>
                            <span class="info badge">
                                <asp:Literal ID="ltlBasketItems" runat="server" />
                            </span>
                        </asp:HyperLink>
                    </li>
                    <li class="ebiz-user-company">
                        <asp:HyperLink ID="hplUserCompany" runat="server" Visible="false">
                            <i class="fa fa-building" aria-hidden="true"></i>
                            <span><asp:Literal ID="ltlCompanyName" runat="server" /></span>
                        </asp:HyperLink>
                    </li>
                </ul>
            </nav>
        </div>
    </div>

    <div id="printerSelectionWindow" class="reveal" data-reveal>
        <asp:PlaceHolder ID="plhErrorList" runat="server">
            <div class="alert-box alert">
                <asp:BulletedList ID="blErrList" runat="server" />
            </div>
        </asp:PlaceHolder>
        <div class="medium-12 columns">
            <h3>
                <b><asp:Literal ID="ltlSelectPrinter" runat="server" /></b>
            </h3>
        </div>
        <div class="medium-4 columns">
            <asp:Label ID="lblPrinterListLabel" runat="server" AssociatedControlID="ddlPrinterList" />
        </div>
        <div class="medium-8 columns">
            <asp:DropDownList ID="ddlPrinterList" runat="server" ClientIDMode="Static" />
        </div>
        <br />
        <br />
        <div>
            <button class="close-button" data-close aria-label="Close modal" type="button" onclick="setDefaultSelectedPrinter()">
                <span aria-hidden="true">&times;</span>
            </button>
        </div>

        <br />
        <div class="button-group medium-12 columns" align="right">
            <asp:Button ID="btnOkPrinterSelection" runat="server" CssClass="button" />
            <asp:Button ID="btnCancelPrinterSelection" runat="server" data-close OnClientClick="setDefaultSelectedPrinter(); return false;" CssClass="button" />
        </div>
        <asp:HiddenField ID="hdfDefaultSelectedPrinter" runat="server" />
    </div>

    <div class="top-bar ebiz-product-nav-wrap">
        <div class="top-bar-left">
            <nav>
                <ul class="dropdown menu" data-dropdown-menu data-close-on-click-inside="false" data-hover-delay="200" data-closing-time="200">
                    <%= GetStaticHTMLInclude("product-menu.htm")%>
                    <asp:PlaceHolder ID="plhSavedAgentSearch" runat="server">
                        <li class="ebiz-search-menu">
                            <a href="../../PagesPublic/ProductBrowse/ProductSubTypes.aspx"><i class="fa fa-bookmark fa-fw"></i> History &amp; Favourites</a>
                            <ul class="menu">
                                <asp:PlaceHolder ID="plhLastSearches" runat="server" Visible="false">
                                    <li class="ebiz-last-search-label">
                                        <h2>
                                            <asp:Literal ID="ltlLastSearchLabel" runat="server" />
                                        </h2>
                                    </li>
                                    <li class="has-submenu ebiz-customer-menuebiz-last-search" >
                                        <div class="row">
                                            <div class="small-10 columns">
                                                <asp:HyperLink ID="hlkLastSavedSearch" runat="server">
                                                    <i class="fa fa-star fa-lg fa-fw"></i>
                                                    <asp:PlaceHolder ID="plhSearch" runat="server">
                                                        <asp:Label ID="lblSearch" runat="server" CssClass="ebiz-last-saved-search-type" />
                                                    </asp:PlaceHolder>
                                                </asp:HyperLink>
                                            </div>
                                        </div>
                                    </li>
                                </asp:PlaceHolder>
                                <asp:Repeater ID="rptSavedSearches" runat="server" OnItemDataBound="rptSavedSearches_ItemDataBound">
                                    <HeaderTemplate>
                                        <li class="ebiz-saved-search-label">
                                            <h2>
                                                <asp:Literal ID="ltlSavedSearchLabel" runat="server" />
                                            </h2>
                                        </li>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li class="has-submenu ebiz-saved-search">
                                            <div class="row">
                                                <div class="small-10 columns">
                                                    <asp:HyperLink ID="hlkSavedSearch" runat="server">
                                                        <i class="fa fa-star fa-lg fa-fw"></i>
                                                        <asp:PlaceHolder ID="plhSearch" runat="server">
                                                            <asp:Label ID="lblSearch" runat="server" CssClass="ebiz-saved-search-type" />
                                                        </asp:PlaceHolder>
                                                    </asp:HyperLink>
                                                </div>
                                                <div class="small-2 columns">
                                                    <asp:HiddenField ID="hdfSearchID" runat="server" Value='<%# Container.DataItem("SAVEDSEARCHID").ToString().Trim()%>' />
                                                    <asp:HyperLink ID="hlkDeleteSavedSearch" runat="server"><i class="fa fa-times"></i></asp:HyperLink>
                                                </div>
                                            </div>
                                        </li>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </ul>
                        </li>
                    </asp:PlaceHolder>
                </ul>
            </nav>
        </div>
    </div>
</asp:PlaceHolder>
<script language="javascript" type="text/javascript">
    function setDefaultSelectedPrinter() {
        document.getElementById("ddlPrinterList").selectedIndex = document.getElementById('<%= hdfDefaultSelectedPrinter.ClientID%>').value;
    }
</script>