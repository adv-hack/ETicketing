<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PromotionsBox.ascx.vb"
    Inherits="UserControls_PromotionsBox" %>
<div id="promo-box">
    <asp:Label ID="promoLabel" runat="server" AssociatedControlID="promoBox"></asp:Label>
    <asp:TextBox ID="promoBox" runat="server" OnPreRender="SetEnterKeyAction"></asp:TextBox>
    <asp:Button ID="promoButton" CssClass="button" runat="server" />
</div>
