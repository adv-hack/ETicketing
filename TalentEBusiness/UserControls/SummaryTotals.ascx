<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SummaryTotals.ascx.vb" Inherits="UserControls_SummaryTotals" %>
<div class="panel ebiz-summary-totals">
    <h2>
        
            <asp:Label ID="TotalsHeaderLabel" runat="server">Merchandise Totals</asp:Label>
        
    </h2>
    <div class="row ebiz-sub-total">
        <div class="small-6 columns">
            <asp:Label ID="SubTotalLabel" runat="server"></asp:Label>
        </div>
        <div class="small-6 columns">
            <asp:Label ID="SubTotal" runat="server"></asp:Label>
        </div>
    </div>
    <asp:panel id="deliveryRow" runat="server" CssClass="row ebiz-delivery">
        <div class="small-6 columns">
            <asp:Label ID="DeliveryLabel" runat="server"></asp:Label>
        </div>
        <div class="small-6 columns">
            <asp:Label ID="Delivery" runat="server"></asp:Label>
        </div>
    </asp:panel>
    <div class="row ebiz-promotion">
        <div class="small-6 columns">
            <asp:Label ID="PromotionLabel" runat="server"></asp:Label>
        </div>
        <div class="small-6 columns">
            <asp:Label ID="Promotion" runat="server"></asp:Label>
        </div>
    </div>
    <asp:panel id="tax1Row" runat="server" CssClass="row ebiz-tax1">
        <div class="small-6 columns">
            <asp:Label ID="Tax1Label" runat="server"></asp:Label>
        </div>
        <div class="small-6 columns">
            <asp:Label ID="Tax1" runat="server"></asp:Label>
        </div>
    </asp:panel>
    <asp:panel id="tax2Row" runat="server" CssClass="row ebiz-tax2">
        <div class="small-6 columns">
            <asp:Label ID="Tax2Label" runat="server"></asp:Label>
        </div>
        <div class="small-6 columns">
            <asp:Label ID="Tax2" runat="server"></asp:Label>
        </div>
    </asp:panel>
    <asp:panel id="tax3Row" runat="server" CssClass="row ebiz-tax3">
        <div class="small-6 columns">
            <asp:Label ID="Tax3Label" runat="server"></asp:Label>
        </div>
        <div class="small-6 columns">
            <asp:Label ID="Tax3" runat="server"></asp:Label>
        </div>
    </asp:panel>
    <asp:panel id="tax4Row" runat="server" CssClass="row ebiz-tax4">
        <div class="small-6 columns">
            <asp:Label ID="Tax4Label" runat="server"></asp:Label>
        </div>
        <div class="small-6 columns">
            <asp:Label ID="Tax4" runat="server"></asp:Label>
        </div>
    </asp:panel>
    <asp:panel id="tax5Row" runat="server" CssClass="row ebiz-tax5">
        <div class="small-6 columns">
            <asp:Label ID="Tax5Label" runat="server"></asp:Label>
        </div>
        <div class="small-6 columns">
            <asp:Label ID="Tax5" runat="server"></asp:Label>
        </div>
    </asp:panel>
    <div class="row ebiz-total">
        <div class="small-6 columns">
            <asp:Label ID="TotalLabel" runat="server"></asp:Label>
        </div>
        <div class="small-6 columns">
            <asp:Label ID="Total" runat="server"></asp:Label>
        </div>
    </div>
</div>