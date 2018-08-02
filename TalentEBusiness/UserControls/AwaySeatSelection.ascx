<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AwaySeatSelection.ascx.vb" Inherits="UserControls_AwaySeatSelection" %>
<asp:Panel ID="pnlAwaySelection" runat="server" CssClass="row ebiz-away-quantity" DefaultButton="buyButton">
    <div class="large-2 small-3 columns">
        <asp:Label ID="qtyLabel" runat="server" AssociatedControlID="qtyTextBox" CssClass="middle"></asp:Label>
    </div>
    <div class="large-2 small-3 columns">
        <asp:TextBox ID="qtyTextBox" runat="server" type="number"></asp:TextBox>
    </div>
    <div class="large-8 small-6 columns">
        <asp:Button CssClass="button" ID="buyButton" runat="server" OnClick="AddTicketingItems" />
    </div>
</asp:Panel>
<asp:PlaceHolder ID="plhTravelProduct" runat="server" Visible="False">
    <p class="ebiz-include-travel">
        <asp:CheckBox ID="chkIncludeTravel" runat="server" /> <asp:Label ID="lblIncludeTravel" runat="server" AssociatedControlID="chkIncludeTravel"></asp:Label>
    </p>
</asp:PlaceHolder>
<asp:HiddenField ID="hfProductCode" runat="server" />
<asp:HiddenField ID="hfProductPriceBand" runat="server" />
<asp:HiddenField ID="hfPriceCode" runat="server" />