<%@ Control Language="VB" AutoEventWireup="false" CodeFile="miniBasket.ascx.vb" Inherits="UserControls_miniBasket" ViewStateMode="Disabled" %>
<%@ Import Namespace="Talent.eCommerce" %>
<%@ Reference Control="~/UserControls/CATBasket.ascx" %>

<div id="mini-basket" class="panel ebiz-mini-basket">
    <div class="button-group">
        <asp:Button ID="clearBasketButton" runat="server" CssClass="button ebiz-muted-action ebiz-clear-basket" />
        <asp:HyperLink id="BasketLinkButton" runat="server" NavigateUrl="~/PagesPublic/Basket/Basket.aspx" OnPreRender="GetText" CssClass="button ebiz-basket" />
        <asp:HyperLink id="CheckoutLinkButton" runat="server" OnPreRender="GetText" CssClass="button ebiz-checkout" />
    </div>
    <asp:PlaceHolder ID="plhMiniBasketTextView" runat="server">
        <asp:PlaceHolder ID="TicketSubTotals_Text" runat="server">
            <div class="row ebiz-tickets-total">
                <div class="small-6 columns">
                    <asp:Literal ID="TicketTotalLabel_Text" runat="server" OnPreRender="GetText" />
                </div>
                <div class="small-6 columns ebiz-price-column">
                    <asp:Literal ID="TicketTotal_Text" runat="server" />
                </div>
            </div>
            <asp:PlaceHolder ID="pblBuyBackTotal_Text" runat="server" Visible="false">
                <div class="row ebiz-buy-back-total">
                    <div class="small-6 columns">
                        <asp:Literal ID="BuyBackTotalLabel_Text" runat="server" OnPreRender="GetText" />
                    </div>
                    <div class="small-6 columns ebiz-price-column">
                        <asp:Literal ID="BuyBackTotal_Text" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="row ebiz-ticket-fee-total">
                    <div class="small-6 columns">
                        <asp:Literal ID="TicketFeeTotalLabel_Text" runat="server" OnPreRender="GetText" />
                    </div>
                    <div class="small-6 columns ebiz-price-column">
                        <asp:Literal ID="TicketFeeTotal_Text" runat="server" />
                    </div>
                </div>
            <asp:PlaceHolder ID="plhCashbackTotal_Text" runat="server" Visible="false">
                <div class="row ebiz-cash-back-total">
                    <div class="small-6 columns">
                        <asp:Literal ID="ltlCashbackLabel_Text" runat="server" />
                    </div>
                    <div class="small-6 columns ebiz-price-column">
                        <asp:Literal ID="ltlCashbackValue_Text" runat="server" />
                    </div>
                </div> 
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAdhocFeesTotal_Text" runat="server" Visible="false">
                <asp:Repeater ID="rptAdhocFees_Text" runat="server">
                    <ItemTemplate>
                        <div class="row ebiz-adhoc-fee-item">
                            <div class="small-6 columns">
                                <%# Container.DataItem("FEE_DESCRIPTION").ToString%>
                            </div>
                            <div class="small-6 columns ebiz-price-column">
                                <%# ShowNegativeSymbol(Container.DataItem("IS_NEGATIVE"))%><%# FormatCurrency(Container.DataItem("FEE_VALUE"))%>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:PlaceHolder>
            <asp:placeholder id="plhOnAccountTotalSummary" runat="server" Visible="false">
                <div class="row ebiz-on-account-total">
                    <div class="small-6 columns">
                        <asp:Literal ID="ltlOnAccountTotal" runat="server" OnPreRender="GetText" />
                    </div>
                    <div class="small-6 columns ebiz-price-column">
                        <asp:Literal ID="ltlOnAccountValue" runat="server" />
                    </div>
                </div>
            </asp:placeholder>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhMerchandiseSubTotals_Text" runat="server">
            <div class="row ebiz-mechandise-total">
                <div class="small-6 columns">
                    <asp:Literal ID="MerchandiseTotalLabel_Text" runat="server" OnPreRender="GetText" />
                </div>
                <div class="small-6 columns ebiz-price-column">
                    <asp:Literal ID="MerchandiseTotal_Text" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhMinQtyPanel_Text" runat="server">
            <div class="row ebiz-mechandise-total">
                <div class="large-12 columns"><asp:Literal ID="MiniBasketMinimumQuantityLabel_Text" runat="server" OnPreRender="GetText" /></span>
            </li>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhMinValPanel_Text" runat="server">
            <div class="row ebiz-mechandise-total">
                <div class="large-12 columns"><asp:Literal ID="MiniBasketMinimumAmountLabel_Text" runat="server" OnPreRender="GetText" /></div>
            </div>
        </asp:PlaceHolder>
        <div class="row ebiz-total">
            <div class="large-12 columns">
                <asp:Literal ID="TotalLabel_Text" runat="server" OnPreRender="GetText" />
            </div>
            <div class="large-12 columns">
                <asp:Literal ID="Total_Text" runat="server" />
            </div>
        </div>
        <div class="row ebiz-items">
            <div class="large-12 columns">
                <asp:Literal ID="NoOfItemsLabel_Text" runat="server" OnPreRender="GetText" />
            </div>
            <div class="large-12 columns">
                <asp:Literal ID="NoOfItems_Text" runat="server" />
            </div>
        </li>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhMiniBasketGridView" runat="server">
        
        <asp:PlaceHolder ID="plhTicketSubTotals_Grid" runat="server">
            <div class="row ebiz-tickets-total">
                <div class="small-6 columns">
                    <asp:Label ID="TicketTotalLabel_Grid" runat="server" OnPreRender="GetText" />
                </div>
                <div class="small-6 columns ebiz-price-column">
                    <asp:Label ID="TicketTotal_Grid" runat="server" />
                </div>
            </div>
            <div class="row ebiz-ticket-fee-total">
                <div class="small-6 columns">
                    <asp:Label ID="TicketFeeTotalLabel_Grid" runat="server" OnPreRender="GetText" />
                </div>
                <div class="small-6 columns ebiz-price-column">
                    <asp:Label ID="TicketFeeTotal_Grid" runat="server" />
                </div>
            </div>
            <asp:PlaceHolder ID="plhBuyBackTotal_Grid" runat="server" Visible="false">
                <div class="row ebiz-buy-back-total">
                    <div class="small-6 columns">
                        <asp:Label ID="BuyBackTotalLabel_Grid" runat="server" OnPreRender="GetText" />
                    </div>
                    <div class="small-6 columns ebiz-price-column">
                        <asp:Label ID="BuyBackTotal_Grid" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhCashbackTotal_Grid" runat="server" Visible="false">
                <div class="row ebiz-cash-back-total">
                    <div class="small-6 columns">
                        <asp:Literal ID="ltlCashbackLabel_Grid" runat="server" />
                    </div>
                    <div class="small-6 columns ebiz-price-column">
                        <asp:Literal ID="ltlCashbackValue_Grid" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAdhocFees_Grid" runat="server" Visible="false">
                <asp:Repeater ID="rptAdhocFees_Grid" runat="server">
                    <ItemTemplate>
                    <div class="row ebiz-adhoc-fee-item">
                        <div class="small-6 columns">
                            <%# Container.DataItem("FEE_DESCRIPTION").ToString%>
                        </div>
                        <div class="small-6 columns ebiz-price-column">
                            <%# ShowNegativeSymbol(Container.DataItem("IS_NEGATIVE"))%><%# FormatCurrency(Container.DataItem("FEE_VALUE"))%>
                        </div>
                    </div>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:PlaceHolder>
            <asp:placeholder id="plhOnAccountTotalSummary_Grid" runat="server" Visible="false">
                <div class="row ebiz-on-account-total">
                    <div class="large-6 columns">
                        <asp:Literal ID="ltlOnAccountTotal_Grid" runat="server" OnPreRender="GetText" />
                    </div>
                    <div class="small-6 columns ebiz-price-column">
                        <asp:Literal ID="ltlOnAccountValue_Grid" runat="server" />
                    </div>
                </div>
            </asp:placeholder>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhMerchandiseSubTotals_Grid" runat="server">
            <div class="row ebiz-mechandise-total">
                <div class="small-6 columns">
                    <asp:Label ID="MerchandiseTotalLabel_Grid" runat="server" OnPreRender="GetText" />
                </div>
                <div class="small-6 columns ebiz-price-column">
                    <asp:Label ID="MerchandiseTotal_Grid" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhTaxLabel" runat="server">
        <div class="row ebiz-tax">
            <div class="small-6 columns">
                <asp:Label ID="TaxLabel_Grid" runat="server" OnPreRender="GetText" />
            </div>
            <div class="small-6 columns ebiz-price-column">
                <asp:Label ID="Tax_Grid" runat="server" />
            </div>
        </div>
        </asp:PlaceHolder>
        <div class="row ebiz-total">
            <div class="small-6 columns">
                <asp:Label ID="TotalLabel_Grid" runat="server" OnPreRender="GetText" />
            </div>
            <div class="small-6 columns ebiz-price-column">
                <asp:Label ID="Total_Grid" runat="server" />
            </div>
        </div>
       <asp:Repeater ID="miniBasketRepeater" runat="server">
            <ItemTemplate>
                <div class="panel ebiz-basket-repeater-wrap">
                    <div class="row">
                        <asp:HiddenField ID="hfProductCode" runat="server" />
                        <div class="columns ebiz-quantity-wrap">
                            <span id="qtyWrap" runat="server" class="ebiz-quantity">
                                <asp:Label ID="qtyLabel" runat="server" />
                            </span>
                            <span id="qtyDividerWrap" runat="server" class="ebiz-divider">
                                <asp:Label ID="qtyDividerLabel" runat="server" />
                            </span>
                        </div>
                        <div class="columns ebiz-code-name-wrap">
                            <span id="productCodeWrap" runat="server" class="ebiz-code">
                                <asp:Label ID="productCodeLabel" runat="server" />
                            </span>
                            <span id="productNameWrap" runat="server" class="ebiz-name">
                                <asp:Label ID="productNameLabel" runat="server" />
                            </span>
                        </div>
                        <div id="unitPriceWrap" runat="server" class="columns ebiz-unit-price-wrap ebiz-price-wrap" style="display: none;">
                            <asp:Label ID="unitPriceLabel" runat="server" />
                        </div>
                        <div id="linePriceWrap" runat="server" class="columns ebiz-line-price-wrap ebiz-price-wrap">
                            <asp:Label ID="linePriceLabel" runat="server" />
                        </div>
                    </div>
                    <!--<div id="deleteWrap" runat="server" class="action">
                        <asp:Button ID="deleteButton" runat="server" OnClick="doDeleteItem" CssClass="button" />
                    </div>-->
                </div>
            </ItemTemplate>
        </asp:Repeater>
        
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhMinQtyPanel_Grid" runat="server">
        <div class="alert-box warning  ebz-minimum-order-quantity">
            <asp:Label ID="MiniBasketMinimumQuantityLabel_Grid" runat="server" OnPreRender="GetText" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhMinValPanel_Grid" runat="server">
        <div class="alert-box warning  ebz-minimum-order-value">    
            <asp:Label ID="MiniBasketMinimumAmountLabel_Grid" runat="server" OnPreRender="GetText" />
        </div>
    </asp:PlaceHolder>
</div>