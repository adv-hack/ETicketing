<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OrderHistoryLinks.ascx.vb"
    Inherits="UserControls_OrderHistoryLinks" %>
<asp:PlaceHolder ID="plhHistoryLinks" runat="server">
    <div id="divLinkHeader" class="historylink">
        <ul id="ulHistoryLinks" runat="server" class="historylinkul">
            <li id="liPurchaseHistoryLink" runat="server" visible="false"><a href="orderEnquiry.aspx?OrderType=2&OrderTemplateSubType=1">
                <asp:Literal ID="PurchaseHistoryLinkText" runat="server"></asp:Literal></a></li>
            <li id="liTransactionHistoryLink" runat="server" visible="false"><a href="TransactionEnquiry.aspx">
                <asp:Literal ID="TransactionHistoryLinkText" runat="server"></asp:Literal></a></li>
            <li id="liRetailHistoryLink" runat="server" visible="false"><a href="orderEnquiry.aspx?OrderType=1&OrderTemplateSubType=3">
                <asp:Literal ID="RetailHistoryLinkText" runat="server"></asp:Literal></a></li>
        </ul>
    </div>
</asp:PlaceHolder>
<div id="divHeader" class="historyheader">
    <asp:Literal ID="litHistoryHeader" runat="server"></asp:Literal>
</div>
<div id="divInstruction" class="historyInstruction">
    <asp:Literal ID="ltlInstruction" runat="server"></asp:Literal>
</div>
