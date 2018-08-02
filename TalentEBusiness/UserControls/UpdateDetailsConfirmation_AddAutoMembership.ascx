<%@ Control Language="VB" AutoEventWireup="false" CodeFile="UpdateDetailsConfirmation_AddAutoMembership.ascx.vb" Inherits="UserControls_UpdateDetailsConfirmation_AddAutoMembership" %>
<div class="add-auto-membership">
    <div class="title">
        <asp:Label ID="titleLabel" runat="server"></asp:Label>
    </div>
    <div class="content">
        <asp:Label ID="contentLabel" runat="server"></asp:Label>
        <div class="addmembership" runat="server" id="AddMembershipWrap">
            <asp:CheckBox ID="addMembershipCheck" runat="server" />
        </div>
        <div class="button-wrap">
            <asp:Button ID="returnButton" runat="server" CssClass="button return-button" />
            <asp:Button ID="addButton" runat="server" CssClass="button add-button" />
            <p class="clear"></p>
        </div>
    </div>
</div>