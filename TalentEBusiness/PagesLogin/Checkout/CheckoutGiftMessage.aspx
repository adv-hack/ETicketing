<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CheckoutGiftMessage.aspx.vb" Inherits="PagesLogin_Checkout_CheckoutGiftMessage" title="Untitled Page" %>
<%@ Register Src="../../UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/GiftMessage.ascx" TagName="GiftMessage" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:GiftMessage id="GiftMessage1" runat="server">
    </uc1:GiftMessage>
     <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
</asp:Content>

