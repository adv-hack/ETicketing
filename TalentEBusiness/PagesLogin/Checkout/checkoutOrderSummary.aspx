<%@ Page Language="VB" AutoEventWireup="false" CodeFile="checkoutOrderSummary.aspx.vb" Inherits="PagesLogin_checkoutOrderSummary" title="Untitled Page" %>
<%@ Register Src="../../UserControls/CheckoutOrderSummary.ascx" TagName="CheckoutOrderSummary" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
       <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:Label ID="InstructionsLabel" runat="server"></asp:Label>
    <Talent:CheckoutOrderSummary ID="CheckoutOrderSummary1" runat="server" />
</asp:Content>

