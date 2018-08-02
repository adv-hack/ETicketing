<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PriceBreakTable.ascx.vb"
    Inherits="UserControls_PriceBreakTable" %>
<asp:PlaceHolder ID="pnlPriceBreakTable" runat="server">
    <div class="multibuy">
        <p class="header">
            <asp:Label runat="server" ID="lblPriceBreakHeader"> </asp:Label></p>
        <asp:Label runat="server" ID="lblPriceBreakTable" Text=""></asp:Label>
        <p class="disclaimer">
            <span>
                <asp:Label runat="server" ID="lblDisclaimerHeader"></asp:Label>
            </span>
            <asp:Label runat="server" ID="lblDisclaimerText"></asp:Label>
        </p>
    </div>
</asp:PlaceHolder>
