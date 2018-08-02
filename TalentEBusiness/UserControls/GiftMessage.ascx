<%@ Control Language="VB" AutoEventWireup="false" CodeFile="GiftMessage.ascx.vb" Inherits="UserControls_GiftMessage" %>
<div id="gift-message" class="default-form">
    <div>
        <h2><asp:Label ID="titleLabel" runat="server"></asp:Label></h2>
        <p><asp:Label ID="instructionsLabel" runat="server"></asp:Label></p>
    </div>
    <div>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="GiftMessage"/>
    </div>
    <div>
        <asp:Label ID="toLabel" runat="server" AssociatedControlID="toBox"></asp:Label>
        <asp:TextBox ID="toBox" runat="server" CssClass="input-l"></asp:TextBox>
        <asp:RequiredFieldValidator ID="toRequired" runat="server" ControlToValidate="toBox" Text="*" ValidationGroup="GiftMessage"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="toRegEx" runat="server" ControlToValidate="toBox" Text="*" ValidationGroup="GiftMessage"></asp:RegularExpressionValidator>
    </div>
    <div>
        <asp:Label ID="msgLabel" runat="server" AssociatedControlID="msgBox"></asp:Label>
        <asp:TextBox ID="msgBox" runat="server" TextMode="MultiLine" CssClass="input-l"></asp:TextBox>
        <asp:RequiredFieldValidator ID="msgRequired" runat="server" ControlToValidate="msgBox" Text="*" ValidationGroup="GiftMessage"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="msgRegEx" runat="server" ControlToValidate="msgBox" Text="*" ValidationGroup="GiftMessage"></asp:RegularExpressionValidator>
    </div>
    <div>
        <label>&nbsp;</label>
        <asp:Button ID="submitBtn" runat="server" ValidationGroup="GiftMessage" CausesValidation="true" CssClass="button" />
    </div>
</div>