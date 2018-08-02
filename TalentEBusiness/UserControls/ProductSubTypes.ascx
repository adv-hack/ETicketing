<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductSubTypes.ascx.vb" Inherits="UserControls_ProductSubTypes" ViewStateMode="Disabled" %>
<%@ Register Src="ProductSubTypesFilter.ascx" TagName="ProductSubTypesFilter" TagPrefix="Talent" %>

<asp:PlaceHolder ID="plhErrorList" runat="server" Visible="False">
    <div class="alert-box alert">
        <asp:BulletedList ID="errorlist" runat="server" />
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plProductSubTypesFilter" runat="server" Visible="True">
    <Talent:ProductSubTypesFilter ID="ProductSubTypesFilter" runat="server" />
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhNoProductsFound" runat="server" Visible="False">
    <div class="alert-box warning ebiz-no-products-found">
        <asp:Literal ID="ltlNoProductsFound" runat="server" /></div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhPagerTopWrapper" runat="server">
    <div class="panel pagination-centered ebiz-pager ebiz-top-pager" id="PagerTop" runat="server">
        <asp:PlaceHolder ID="plhTopPager" runat="server">
            <ul class="pagination">
                <li class="ebiz-displaying"><asp:Literal ID="displayingLabelT" runat="server" /></li>
                <li class="ebiz-current-item"><asp:Literal ID="displayingValueLabelT" runat="server" /></li>
                <li class="ebiz-to"><asp:Literal ID="toLabelT" runat="server" /></li>
                <li class="ebiz-next-item"><asp:Literal ID="toValueLabelT" runat="server" /></li>
                <li class="ebiz-of"><asp:Literal ID="ofLabelT" runat="server" /></li>
                <li class="ebiz-total-pages"><asp:Literal ID="ofValueLabelT" runat="server" /></li>
                <li class="ebiz-first"><asp:HyperLink ID="LnkFirstT" runat="server" /></li>
                <li class="ebiz-prev"><asp:HyperLink ID="LnkPrevT" runat="server" /></li>
                <asp:Literal ID="LinksLabelT" runat="server" />
                <li class="ebiz-next"><asp:HyperLink ID="LnkNextT" runat="server" /></li>
                <li class="ebiz-last"><asp:HyperLink ID="LnkLastT" runat="server" /></li>
            </ul>
        </asp:PlaceHolder>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhTicketingResults" runat="server">
    <asp:Repeater ID="rptTicketingResults" runat="server" Visible="false">
        <HeaderTemplate>
            <div class="row ebiz-product-sub-types">
        </HeaderTemplate>
        <ItemTemplate>
            <div id="divResultItem" class="column ebiz-product-sub-type-item" runat="server">
                <asp:HyperLink ID="hlkTicketingResult" runat="server">
                    <div class="panel">
                        <div class="ebiz-subtype-header">
                            <h2><asp:Literal ID="ltlDescription" runat="server" /></h2>
                        </div>
                        <div class="ebiz-subtype-image">
                            <asp:Image ID="imgSubType" runat="server" />
                            <asp:Image ID="imgOpposition" runat="server" />
                            <asp:Image ID="imgCompetition" runat="server" />
                            <asp:Image ID="imgSponsor" runat="server" />
                        </div>
                        <div class="ebiz-subtype-dates">
                            <div class="row">
                                <asp:PlaceHolder ID="plhStartDate" runat="server">
                                    <div class="columns ebiz-column-label ebiz-start-date"><asp:Literal ID="ltlStartDateLabel" runat="server" /></div>
                                    <div class="columns ebiz-column-data ebiz-start-date"><asp:Literal ID="ltlStartDateValue" runat="server" /></div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhEndDate" runat="server">
                                    <div class="columns ebiz-column-label ebiz-end-date"><asp:Literal ID="ltlEndDateLabel" runat="server" /></div>
                                    <div class="columns ebiz-column-data ebiz-end-date"><asp:Literal ID="ltlEndDateValue" runat="server" /></div>
                                </asp:PlaceHolder>
                            </div>
                        </div>
                    </div>
                </asp:HyperLink>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhBottomPager" runat="server">
    <div class="panel pagination-centered ebiz-pager ebiz-bottom-pager" id="PagerBottom" runat="server">
        <ul class="pagination">
            <li class="ebiz-displaying"><asp:Literal ID="displayingLabelB" runat="server" /></li>
            <li class="ebiz-current-item"><asp:Literal ID="displayingValueLabelB" runat="server" /></li>
            <li class="ebiz-to"><asp:Literal ID="toLabelB" runat="server" /></li>
            <li class="ebiz-next-item"><asp:Literal ID="toValueLabelB" runat="server" /></li>
            <li class="ebiz-of"><asp:Literal ID="ofLabelB" runat="server" /></li>
            <li class="ebiz-total-pages"><asp:Literal ID="ofValueLabelB" runat="server" /></li>
            <li class="ebiz-first"><asp:HyperLink ID="LnkFirstB" runat="server" /></li>
            <li class="ebiz-prev"><asp:HyperLink ID="LnkPrevB" runat="server" /></li>
            <asp:Literal ID="LinksLabelB" runat="server" />
            <li class="ebiz-next"><asp:HyperLink ID="LnkNextB" runat="server" /></li>
            <li class="ebiz-last"><asp:HyperLink ID="LnkLastB" runat="server" /></li>
        </ul>
    </div>
</asp:PlaceHolder>

