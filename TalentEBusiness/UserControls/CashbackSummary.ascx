<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CashbackSummary.ascx.vb" Inherits="UserControls_CashbackSummary" %>
<asp:PlaceHolder ID="plhCashBackSummary" runat="server" Visible="True">
    <div class="panel ebiz-cashback-summary">
        <p class="ebiz-cashback-summary-available"><asp:Literal ID="ltlCashbackAvailable" runat="server" /></p>
        <asp:PlaceHolder ID="plhCashbackApplied" runat="server" Visible="false">
            <p class="ebiz-cashback-summary-applied"><asp:Literal ID="ltlCashbackApplied" runat="server" /></p>
        </asp:PlaceHolder>
        <asp:Literal ID="ltlInformationText" runat="server" />
    </div>
</asp:PlaceHolder>