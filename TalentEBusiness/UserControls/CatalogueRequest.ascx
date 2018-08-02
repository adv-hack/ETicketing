<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CatalogueRequest.ascx.vb" Inherits="UserControls_CatalogueRequest" %>
<div id="catalogueRequest">
    <asp:Label ID="promoCodeLabel" runat="server"></asp:Label>
    <asp:TextBox ID="promoCodeTextBox" CssClass="text-box" runat="server"></asp:TextBox>
    <br /><br />
    <asp:Button ID="proceed" cssclass="button" runat="server" />
</div>
