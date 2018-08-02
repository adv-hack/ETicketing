<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PaymentSummaries.ascx.vb" Inherits="UserControls_PaymentSummaries" %>
<%@ Register Src="SummaryTotals.ascx" TagName="SummaryTotals" TagPrefix="Talent" %>
<%@ Register Src="BasketSummary.ascx" TagName="BasketSummary" TagPrefix="Talent" %>
<div class="payment-totals" >
    <Talent:SummaryTotals ID="Payment_SummaryTotals1" runat="server" Usage="PAYMENT" />
    <Talent:BasketSummary ID="BasketSummary1" runat="server" Usage="PAYMENT" />
</div>
