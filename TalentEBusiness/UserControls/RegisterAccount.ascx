<%@ Control Language="VB" AutoEventWireup="false" CodeFile="RegisterAccount.ascx.vb"
    Inherits="UserControls_RegisterAccount" %>
<div class="panel ebiz-register-account">
    <h2>
    	
        	<asp:Label ID="headerLabel" runat="server"></asp:Label>
    	
    </h2>
    <div class="ebiz-ebiz-register-account-blurb">
        <asp:Label ID="infoLabel" runat="server"></asp:Label>
    </div>
    <asp:Button ID="registerButton" CssClass="button ebiz-register-account-button" runat="server" />
</div>
