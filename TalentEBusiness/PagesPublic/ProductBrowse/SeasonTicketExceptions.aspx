<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SeasonTicketExceptions.aspx.vb" Inherits="PagesPublic_ProductBrowse_SeasonTicketExceptions" ViewStateMode="Disabled" MasterPageFile="~/MasterPages/PublicWebSales/TalentDev/1Column.master" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/BasketSummary.ascx" TagName="BasketSummary" TagPrefix="Talent" %>

<asp:Content ID="cphHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">

</asp:Content>

<asp:Content ID="cphBody" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
        <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
        <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />

        <asp:PlaceHolder ID="plhExceptionsDetails" runat="server">
            <div class="ebiz-exceptions-details alert-box warning">
                <asp:Literal ID="ltlExceptionsDetails" runat="server" />
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhErrorMessage" runat="server">
            <div class="alert-box alert"><asp:Literal ID="ltlErrorMessage" runat="server" /></div>
        </asp:PlaceHolder>

        <asp:Repeater ID="rptSeasonTicketSeats" runat="server">
            <HeaderTemplate>
                <div class="panel ebiz-season-ticket-product">
                    <div class="ebiz-header">
                        <div class="ebiz-product-detail">
                            <asp:Image ID="STEventImage" runat="server" />
                            <div class="ebiz-basket-product-descriptions">
                                <asp:PlaceHolder ID="plhSTDescription1" runat="server">
                                    <h2><asp:Literal ID="ltlSTDescription1" runat="server" /></h2>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhSTDescription2" runat="server">
                                    <div class="ebiz-description-2"><asp:Literal ID="ltlSTDescription2" runat="server" /></div>
                                </asp:PlaceHolder>
                            </div>
                        </div>
                    </div>

                    
                        <table class="stack">
                        <thead>
                            <tr>
                                <th scope="col" class="ebiz-season-ticket-seat"><%= SeasonTicketSeatColumnHeading%></th>
                                <th scope="col" class="ebiz-season-ticket-exceptions"><%= ExceptionsColumnHeading%></th>
                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                        <tr>
                            <td class="ebiz-season-ticket-seat" data-title="<%= SeasonTicketSeatColumnHeading%>">
                                <asp:Literal ID="ltlSeasonTicketSeat" runat="server" />
                                <asp:Literal ID="ltlSeasonTicketSeatRestriction" runat="server" />
                            </td>
                            <td class="ebiz-season-ticket-exceptions" data-title="<%= ExceptionsColumnHeading%>">
                                <asp:PlaceHolder ID="plhExceptionCount" runat="server">
                                    <a href="#" data-toggle="<%# GetDropDownID(Container.ItemIndex)%>"><asp:Literal ID="ltlExceptionCount" runat="server" /></a>
                                    <div id="<%# GetDropDownID(Container.ItemIndex)%>" class="dropdown-pane" data-dropdown data-hover="true" data-hover-pane="true" data-options="vOffset:14">
                                        <asp:Repeater ID="rptDropDownExceptionGames" runat="server">
                                            <HeaderTemplate><ul class="no-bullet"></HeaderTemplate>
                                            <ItemTemplate><li><%# DataBinder.Eval(Container.DataItem, "PRODUCT_DESCRIPTION1").ToString().Trim()%></li></ItemTemplate>
                                            <FooterTemplate></ul></FooterTemplate>
                                        </asp:Repeater>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:Literal ID="ltlNoExceptions" runat="server" />
                            </td>
                        </tr>
            </ItemTemplate>
            <FooterTemplate>
                        </tbody>
                        </table>
                    </div>
            </FooterTemplate>
        </asp:Repeater>
        
        <Talent:HTMLInclude ID="uscHTMLInclude2" runat="server" Usage="2" Sequence="2" />

        <asp:Repeater ID="rptExceptionProducts" runat="server">
            <HeaderTemplate>
                <div class="ebiz-exception-product-list">
            </HeaderTemplate>
            <ItemTemplate>
                    <div class="panel ebiz-exception-product">
                        <div class="ebiz-header">
                            <div class="ebiz-product-detail">
                                <asp:Image ID="EventImage" runat="server" />
                                <div class="ebiz-basket-product-descriptions">
                                    <asp:PlaceHolder ID="plhDescription1" runat="server">
                                        <h2><asp:Literal ID="ltlDescription1" runat="server" /></h2>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plhDescription2" runat="server">
                                        <div class="ebiz-description-2"><asp:Literal ID="ltlDescription2" runat="server" /></div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plhProductDate" runat="server">
                                        <div class="ebiz-description-3 ebiz-product-date"><asp:Literal ID="ltlProductDate" runat="server" /></div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plhProductTime" runat="server">
                                        <div class="ebiz-description-4 ebiz-product-time"><asp:Literal ID="ltlProductTime" runat="server" /></div>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                            <asp:PlaceHolder ID="plhNoAvailability" runat="server">
                                <div class="alert-box warning ebiz-no-availability"><asp:Literal ID="ltlNoAvailability" runat="server" /></div>
                            </asp:PlaceHolder>
                        </div>

                        <asp:Repeater ID="rptExceptionSeats" runat="server" OnItemDataBound="rptExceptionSeats_ItemDataBound" OnItemCommand="rptExceptionSeats_ItemCommand">
                            <HeaderTemplate>
                                <div class="ebiz-exception-seats">
                                    <table class="stack">
                                    <thead>
                                        <tr>
                                            <th scope="col" class="ebiz-season-ticket-seat"><%= SeasonTicketSeatColumnHeading%></th>
                                            <th scope="col" class="ebiz-exception-seat"><%= ExceptionSeatColumnHeading%></th>
                                            <th scope="col" class="ebiz-change-remove-button">&nbsp;</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                        <tr>
                                            <td class="ebiz-season-ticket-seat" data-title="<%= SeasonTicketSeatColumnHeading%>">
                                                <asp:Literal ID="ltlSeasonTicketSeatForExceptionProduct" runat="server" />
                                                <asp:Literal ID="ltlSTSeatRestrictionForExceptionProduct" runat="server" />
                                            </td>
                                            <td class="ebiz-exception-seat" data-title="<%= ExceptionSeatColumnHeading%>">
                                                <asp:Literal ID="ltlExceptionSeatForExceptionProduct" runat="server" />
                                                <asp:Literal ID="ltlExSeatRestrictionForExceptionProduct" runat="server" />
                                                <asp:HiddenField ID="hdfExceptionSeatForExceptionProduct" runat="server" />
                                            </td>
                                            <td class="ebiz-change-remove-button">
                                                <div class="button-group">
                                                    <asp:HyperLink ID="hplChangeSeat" runat="server" CssClass="button ebiz-change-seat" />
                                                    <asp:HyperLink ID="hplPickSeat" runat="server" CssClass="button ebiz-pick-seat" />
                                                    <asp:Button ID="btnRemove" runat="server" CommandName="RemoveSeat" CssClass="button ebiz-remove-seat" />
                                                </div>
                                            </td>
                                        </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                    </tbody>
                                    </table>
                                </div>
                            </FooterTemplate>
                        </asp:Repeater>

                        <asp:PlaceHolder ID="plhKeepSeatsTogether" runat="server">
                            <div class="ebiz-keep-seats-together">
                                <asp:Label ID="lblKeepSeatsTogether" runat="server" CssClass="alert-box info" />
                                <div>
                                    <asp:Button ID="btnKeepSeatsTogether" runat="server" CssClass="button ebiz-cta" CommandName="KeepSeatsTogether"/>
                                </div>
                                <asp:HiddenField ID="hdfKeepSeatsTogether" runat="server" />
                            </div>
                        </asp:PlaceHolder>
                    </div>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <asp:Button ID="btnBasket" runat="server" CssClass="button ebiz-cta" />

        <Talent:BasketSummary ID="uscBasketSummary" runat="server" />

</asp:Content>