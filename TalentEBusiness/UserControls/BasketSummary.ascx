<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BasketSummary.ascx.vb" Inherits="UserControls_BasketSummary" %>
<%@ Register Src="~/UserControls/PartPayments.ascx" TagName="PartPayments" TagPrefix="Talent" %>
<asp:PlaceHolder ID="plhBasketSummary" runat="server" Visible="true">
    <div class="panel ebiz-basket-summary">
        <h2>
            
                <asp:Literal ID="ltlBasketSummaryTitle" runat="server" /></h2>
        <asp:Repeater ID="rptBasketSummary" runat="server" Visible="true">
            <HeaderTemplate>
            </HeaderTemplate>
            <ItemTemplate>
                <div class="row">
                    <div class="medium-6 columns">
                        <asp:Literal ID="ltlSummaryItemLabel" runat="server" />
                    </div>
                    <div class="medium-6 columns ebiz-values">
                        <span class="ebiz-fee-wrap">
                            <asp:HyperLink ID="hplMerchandisePromotions" data-open="merchandise-promotions-modal" runat="server" CssClass="ebiz-open-modal">
                                <asp:Image ID="imgMerchandisePromotions" runat="server" />
                            </asp:HyperLink>
                            <asp:Literal ID="ltlSummaryItemValue" runat="server" />
                        <span>
                        <span class="ebiz-remove-fee-wrap">
                            <asp:LinkButton ID="lnkButtonForInclude" ValidationGroup="BasketSummary" Visible="true" runat="server" CommandName="ProcessInclude" CssClass="fa-input ebiz-fa-input ebiz-remove">
                                <i id="iconForIncludeButton" runat="server" class="fa fa-times"></i>
                            </asp:LinkButton>
                        </span>
                        <asp:HiddenField ID="hidSummaryProduct" runat="server" Visible="false" />
                        <asp:HiddenField ID="hidFeeCategory" runat="server" Visible="false" />
                        <asp:HiddenField ID="hidIsIncluded" runat="server" Visible="false" />
                        <div id="merchandise-promotions-modal" class="reveal ebiz-reveal-ajax" data-reveal></div>
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:Repeater>
        <Talent:PartPayments CheckoutSummary="true" ID="uscPartPayments" runat="server" />
        <div class="row ebiz-basket-summary-total">
            <div class="medium-6 columns">
                <asp:Literal ID="ltlBasketSummaryTotalLabel" runat="server" />
            </div>
            <div class="medium-6 columns ebiz-values">
                <asp:Label ID="ltlBasketSummaryTotalValue" runat="server" />
            </div>
        </div>
        <asp:PlaceHolder ID="plhRefundAll" runat="server" Visible="false">
            <div class="ebiz-basket-buttons">
                <asp:Button ID="btnRefundAll" CssClass="button ebiz-basket-summary-refund-all" runat="server" OnClick="btnRefundAll_Click" />
            </div>
        </asp:PlaceHolder>
    </div>
    
</asp:PlaceHolder>
