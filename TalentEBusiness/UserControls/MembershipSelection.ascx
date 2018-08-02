<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MembershipSelection.ascx.vb" Inherits="UserControls_MembershipSelection" ViewStateMode="Disabled" %>
<asp:Panel ID="pnlAwaySelection" runat="server" CssClass="row ebiz-membership-quantity" DefaultButton="buyButton">
    <div class="large-2 small-3 columns">
        <asp:Label ID="qtyLabel" runat="server" AssociatedControlID="qtyTextBox" CssClass="middle"></asp:Label>
    </div>
    <div class="large-2 small-3 columns">
        <asp:TextBox CssClass="input-s" ID="qtyTextBox" runat="server" MaxLength="2" type="number" min="0" max="99"></asp:TextBox>
    </div>
    <div class="large-8 small-6 columns">
        <asp:Button CssClass="button" ID="buyButton" runat="server" OnClick="AddTicketingItems" />
    </div>
</asp:Panel>
<asp:HiddenField ID="hfPriceCode" runat="server" />
<asp:HiddenField ID="hfProductCode" runat="server" />