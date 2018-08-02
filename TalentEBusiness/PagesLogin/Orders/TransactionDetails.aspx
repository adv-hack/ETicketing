<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TransactionDetails.aspx.vb" Inherits="PagesLogin_Orders_TransactionDetails" title="Untitled Page" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
    <%@ Register Src="~/UserControls/TransactionDetails.ascx" TagName="TransactionDetails" TagPrefix="Talent" %>
    <%@Register Src="~/UserControls/OrderEnquiry2.ascx" TagName="OrderEnquiry2" TagPrefix="Talent"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div id="divTransactionLinkTop" class="transactionlink">
<asp:HyperLink ID="hlnkTransactionLinkTop" runat="server"></asp:HyperLink>
</div>
<Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
<Talent:TransactionDetails ID="TransactionDetails1" runat="server" />
<Talent:OrderEnquiry2 ID="OrderEnquiry21" runat="server" />
<div id="divTransactionLinkBottom" class="transactionlink">
<asp:HyperLink ID="hlnkTransactionLinkBottom" runat="server"></asp:HyperLink>
</div>
</asp:Content>

