<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TicketExchangeProducts.aspx.vb" Inherits="PagesLogin_Orders_TicketExchangeProducts" %>
<%@ Register Src="~/UserControls/CustomerProgressBar.ascx" TagName="CustomerProgressBar" TagPrefix="Talent" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server"> 
         <script language="javascript" type="text/javascript">
         $(document).ready(function () { $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy' }); });
    </script>

    <Talent:CustomerProgressBar ID="ProgressBar1" runat="server"></Talent:CustomerProgressBar>

    <asp:PlaceHolder ID="plhErrorList" runat="server"  visible="False">
        <div class="alert-box alert">
            <asp:BulletedList ID="blErrorList" runat="server" />
        </div>
    </asp:PlaceHolder>

    <div class="panel ebiz-ticket-exchange-summary-wrap">
        <h2><asp:Literal ID="ltlTicketingExchangeProductsHeader" runat="server" /></h2>
        <div class="row small-up-1 medium-up-1 large-up-3">
            <div class="column ebiz-ticket-exchange-summary ebiz-ticket-exchange-summary-line1">
                <div><asp:Literal runat="server" ID="ltlSummaryLine1" /></div>
            </div>
            <div class="column ebiz-ticket-exchange-summary ebiz-ticket-exchange-summary-line2">
                <div><asp:Literal runat="server" ID="ltlSummaryLine2" /></div>
            </div>
            <div class="column ebiz-ticket-exchange-summary ebiz-ticket-exchange-summary-line3">
                <div><asp:Literal runat="server" ID="ltlSummaryLine3" /></div>
            </div>
        </div>
    </div>
    <div class="panel ebiz-ticket-exchange-select-product-wrap">
        <h2><asp:Literal ID="ltlSellYourTickets" runat="server" /></h2>

        <div class="row ebiz-from-date-filter">
            <div class="columns">
                <asp:Label ID="FromDateLabel" runat="server" AssociatedControlID="FromDate" />
            </div>
            <div class="columns">
                <asp:TextBox ID="FromDate" CssClass="datepicker" runat="server" />
                <asp:CompareValidator id="cvdFromDate" CssClass="error" runat="server" Type="Date" Operator="DataTypeCheck" ControlToValidate="FromDate" />
                <asp:label ID="lblFromDateValue" runat="server" Visible="false" />
            </div>
            <div class="columns">
                 <asp:Button ID="btnFromDate" runat="server" CssClass="button" AssociatedControlID="FromDate"/>
             </div>
        </div>

            <asp:Repeater ID="rptTicketExchangeProducts" runat="server">
                <HeaderTemplate>
                    <table class="no-footer ebiz-responsive-table ebiz-ticket-exchange-products-results" role="grid">
                    <thead>
                        <tr>
                             <asp:PlaceHolder ID="plhproductDateHeader" runat="server"> 
                                <th scope="col" class="ebiz-product-date">
                                    <asp:PlaceHolder ID="plhproductDateInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="product-date-header-info" data-disable-hover="false"  title="<%=ProductDateHeaderinfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                    <span><%=ProductDateHeader%></span></th>
                             </asp:PlaceHolder>

                            <asp:PlaceHolder ID="plhProductDescriptionHeader" runat="server"> 
                                <th scope="col" class="ebiz-product-desc">
                                    <asp:PlaceHolder ID="plhProductDescriptionInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="product-date-header-info" data-disable-hover="false"  title="<%=ProductDescriptionHeaderinfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                    <span><%=ProductDescriptionHeader%></span></th>
                             </asp:PlaceHolder>

                             <asp:PlaceHolder ID="plhCompetitionDescriptionHeader" runat="server"> 
                                <th scope="col" class="ebiz-product-competition">
                                    <asp:PlaceHolder ID="plhCompetitionDescriptionInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="product-date-header-info" data-disable-hover="false"  title="<%=CompetitionDescriptionHeaderinfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                    <span><%=CompetitionDescriptionHeader%></span></th>
                             </asp:PlaceHolder>

                            <asp:PlaceHolder ID="plhTotalPurchasedHeader" runat="server"> 
                                <th scope="col" class="ebiz-product-tot-purch">
                                    <asp:PlaceHolder ID="plhTotalPurchasedInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="product-date-header-info" data-disable-hover="false"  title="<%=TotalPurchasedHeaderinfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                    <span><%=TotalPurchasedHeader%></span></th>
                            </asp:PlaceHolder>

                             <asp:PlaceHolder ID="plhTotalPurchasedPriceHeader" runat="server"> 
                                <th scope="col" class="ebiz-product-tot-purch-price">
                                    <asp:PlaceHolder ID="plhTotalPurchasedPriceInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="product-date-header-info" data-disable-hover="false"  title="<%=TotalPurchasedPriceHeaderinfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                    <span><%=TotalPurchasedPriceHeader%></span></th>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="plhTotalWithCustomerHeader" runat="server"> 
                                <th scope="col" class="ebiz-product-tot-with-cust">
                                    <asp:PlaceHolder ID="plhTotalWithCustomerInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="product-date-header-info" data-disable-hover="false"  title="<%=TotalWithCustomerHeaderinfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                    <span><%=TotalWithCustomerHeader%></span></th>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="plhTotalSoldOnTicketExchangeHeader" runat="server"> 
                                <th scope="col" class="ebiz-product-tot-sold">
                                    <asp:PlaceHolder ID="plhTotalSoldOnTicketExchangeInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="product-date-header-info" data-disable-hover="false"  title="<%=TotalSoldOnTicketExchangeHeaderinfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                    <span><%=TotalSoldOnTicketExchangeHeader%></span></th>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="plhTotalResalePriceSoldHeader" runat="server"> 
                                <th scope="col" class="ebiz-product-tot-sold-price">
                                    <asp:PlaceHolder ID="plhTotalResalePriceSoldInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="product-date-header-info" data-disable-hover="false"  title="<%=TotalResalePriceSoldHeaderinfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                    <span><%=TotalResalePriceSoldHeader%></span></th>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="plhTotalHandlingFeeSoldHeader" runat="server"> 
                                <th scope="col" class="ebiz-product-tot-sold-fee">
                                    <asp:PlaceHolder ID="plhTotalHandlingFeeSoldInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="product-date-header-info" data-disable-hover="false"  title="<%=TotalHandlingFeeSoldHeaderinfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                    <span><%=TotalHandlingFeeSoldHeader%></span></th>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="plhTotalPendingOnTicketExchangeHeader" runat="server"> 
                                <th scope="col" class="ebiz-product-tot-pend">
                                    <asp:PlaceHolder ID="plhTotalPendingOnTicketExchangeInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="product-date-header-info" data-disable-hover="false"  title="<%=TotalPendingOnTicketExchangeHeaderinfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                    <span><%=TotalPendingOnTicketExchangeHeader%></span></th>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="plhTotalResalePricePendingHeader" runat="server"> 
                                <th scope="col" class="ebiz-product-tot-pend-price">
                                    <asp:PlaceHolder ID="plhTotalResalePricePendingInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="product-date-header-info" data-disable-hover="false"  title="<%=TotalResalePricePendingHeaderinfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                    <span><%=TotalResalePricePendingHeader%></span></th>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="plhTotalHandlingFeePendingHeader" runat="server"> 
                                <th scope="col" class="ebiz-product-tot-pend-fee">
                                    <asp:PlaceHolder ID="plhTotalHandlingFeePendingInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="product-date-header-info" data-disable-hover="false"  title="<%=TotalHandlingFeePendingHeaderinfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                    <span><%=TotalHandlingFeePendingHeader%></span></th>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="plhActionHeader" runat="server"> 
                                <th scope="col" class="ebiz-ebiz-action-product-select">
                                    <asp:PlaceHolder ID="plhActionInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="product-date-header-info" data-disable-hover="false"  title="<%=ActionHeaderinfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                    <span><%=ActionHeader%></span></th>
                            </asp:PlaceHolder>
                        </tr>
                    </thead>
                    <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                        <tr>
                            <asp:PlaceHolder ID="plhproductDate" runat="server"> 
                                <td class="ebiz-product-date" data-title="<%=ProductDateHeader%>"><%#Convert.ToDateTime(Eval("ProductDate")).ToShortDateString() %></td>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhproductDescription" runat="server">
                                <td class="ebiz-product-desc" data-title="<%=ProductDescriptionHeader%>"><%# Eval("ProductDescription")%></td>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhCompetitionDescription" runat="server">
                                <td class="ebiz-product-competition" data-title="<%=CompetitionDescriptionHeader%>"><%# Eval("CompetitionDescription")%>&nbsp;</td>
                            </asp:PlaceHolder>
                             <asp:PlaceHolder ID="plhTotalPurchased" runat="server">
                                <td class="ebiz-product-tot-purch" data-title="<%=TotalPurchasedHeader%>"><%# Eval("TotalPurchased")%></td>
                            </asp:PlaceHolder>
                             <asp:PlaceHolder ID="plhTotalPurchasedPrice" runat="server">
                                <td class="ebiz-product-tot-purch-price" data-title="<%=TotalPurchasedPriceHeader%>"><%# FormatCurrency(Eval("TotalPurchasedPrice"))%></td>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhTotalWithCustomer" runat="server">
                                <td class="ebiz-product-tot-with-cust" data-title="<%=TotalWithCustomerHeader%>"><%# Eval("TotalWithCustomer")%></td>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhTotalSoldOnTicketExchange" runat="server">
                                <td class="ebiz-product-tot-sold" data-title="<%=TotalSoldOnTicketExchangeHeader%>"><%# Eval("TotalSoldOnTicketExchange")%></td>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhTotalResalePriceSold" runat="server">
                                <td class="ebiz-product-tot-sold-price" data-title="<%=TotalResalePriceSoldHeader%>"><%# FormatCurrency(Eval("TotalResalePriceSold"))%></td>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhTotalHandlingFeeSold" runat="server">
                                <td class="ebiz-product-tot-sold-fee" data-title="<%=TotalHandlingFeeSoldHeader%>"><%# FormatCurrency(Eval("TotalHandlingFeeSold"))%></td>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhTotalPendingOnTicketExchange" runat="server">
                                <td class="ebiz-product-tot-pend" data-title="<%=TotalPendingOnTicketExchangeHeader%>"><%# Eval("TotalPendingOnTicketExchange")%></td>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhTotalResalePricePending" runat="server">
                                <td class="ebiz-product-tot-pend-price" data-title="<%=TotalResalePricePendingHeader%>"><%# FormatCurrency(Eval("TotalResalePricePending"))%></td>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhTotalHandlingFeePending" runat="server">
                                <td class="ebiz-product-resale" data-title="<%=TotalHandlingFeePendingHeader%>"><%# FormatCurrency(Eval("TotalHandlingFeePending"))%></td>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhSelect" runat="server"> 
                                <td class="ebiz-ebiz-action-product-select" data-title="<%=ActionHeader%>"><asp:HyperLink ID="hplSelect" runat="server" CssClass="ebiz-action"/></td> 
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhnotAvailable" runat="server"> 
                                <td>   <span data-tooltip  aria-haspopup="true" class="product-not-available-info" data-disable-hover="false"  title="<%# getStatusDescription(Eval("StatusCode"))%>"><%=ltlnotAvailable%> <i class="fa fa-info-circle" aria-hidden="true"></i>
                                </span></td>
                             </asp:PlaceHolder>                      
                         </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                </FooterTemplate>
            </asp:Repeater>
        </table>
     </div>
</asp:content>
