<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ActivateAccount.ascx.vb"
    Inherits="UserControls_ActivateAccount" %>
<div class="login-page-activate-box">
    <div id="activate-account" class="box default-form">
        <h2>
            <asp:Label ID="headerLabel" runat="server"></asp:Label>
        </h2>
       
        <div class="instructions">
            <asp:Label ID="infoLabel" runat="server"></asp:Label>
        </div> 
        <div class="error">
            <asp:BulletedList ID="errorList" runat="server" CssClass="error">
            </asp:BulletedList>
        </div>
        <div class="account-number">
            <asp:Label ID="accountNoLabel" runat="server" AssociatedControlID="accountNo1"></asp:Label>
            <asp:TextBox ID="accountNo1" runat="server" CssClass="input-l"></asp:TextBox>
            <asp:TextBox ID="accountNo2" runat="server" CssClass="input-l"></asp:TextBox>
        </div>
        <div class="forename">
            <asp:Label ID="lblForename" runat="server" AssociatedControlID="txtForename" />
            <asp:TextBox ID="txtForename" runat="server" CssClass="input-l" />
        </div>
        <div class="surname">
            <asp:Label ID="surnameLabel" runat="server" AssociatedControlID="surname"></asp:Label>
            <asp:TextBox ID="surname" runat="server" CssClass="input-l"></asp:TextBox>
        </div>
        <div class="postcode">
            <asp:Label ID="postcodeLabel" runat="server" AssociatedControlID="postcode"></asp:Label>
            <asp:TextBox ID="postcode" runat="server" CssClass="input-l"></asp:TextBox>
        </div>
        <p class="activate">
            <asp:Button ID="activateButton" runat="server" CssClass="button" />
        </p>
    </div>
</div>
