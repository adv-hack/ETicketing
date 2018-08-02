<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AccountWindow.ascx.vb"
    Inherits="UserControls_AccountWindow" %>
<div id="divAccountWindow">
    <asp:PlaceHolder ID="plhMessage" runat="server" Visible="false">
        <div id="divMessage">
            <asp:Literal ID="ltlMessage" runat="server"></asp:Literal>
        </div>
    </asp:PlaceHolder>
        <asp:Literal ID="ltlAccountWindowList" runat="server"></asp:Literal>
</div>
