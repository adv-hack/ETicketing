<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PaymentDetailsPopup.aspx.vb" Inherits="PagesLogin_Orders_PaymentDetailsPopup" title="Untitled Page" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
    <%@ Register Src="~/UserControls/TransactionDetails.ascx" TagName="TransactionDetails" TagPrefix="Talent" %>
    <%@Register Src="~/UserControls/OrderEnquiry2.ascx" TagName="OrderEnquiry2" TagPrefix="Talent"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
<Talent:TransactionDetails ID="TransactionDetails1" runat="server" />
<Talent:OrderEnquiry2 ID="OrderEnquiry21" runat="server" />
</asp:Content>

