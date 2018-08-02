<%@ Page Language="VB" AutoEventWireup="false" CodeFile="checkoutDeliveryDetails.aspx.vb" Inherits="PagesLogin_checkoutDeliveryDetails" title="Untitled Page" %>
<%@ Register Src="../../UserControls/CheckoutDeliveryAddress.ascx" TagName="CheckoutDeliveryAddress" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="cphHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <script src="https://a248.e.akamai.net/images.talentarena.co.uk/Global/Lib/ticketing-products-date-picker.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
     <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:CheckoutDeliveryAddress ID="CheckoutDeliveryAddress1" runat="server" />
</asp:Content>