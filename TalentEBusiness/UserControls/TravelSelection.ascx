<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TravelSelection.ascx.vb" Inherits="UserControls_TravelSelection" %>
<asp:DropDownList ID="TransportDropDown" CssClass="select" runat="server"></asp:DropDownList>
<strong><asp:Label ID="qtyLabel" runat="server"></asp:Label></strong>
<asp:TextBox CssClass ="input-s" ID="qtyTextBox" runat="server" MaxLength="2"></asp:TextBox>
<asp:Button CssClass="button" id="buyButton" runat="server" />
