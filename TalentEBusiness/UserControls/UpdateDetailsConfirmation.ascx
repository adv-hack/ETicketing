<%@ Control Language="VB" AutoEventWireup="false" CodeFile="UpdateDetailsConfirmation.ascx.vb" Inherits="UserControls_UpdateDetailsConfirmation" %>
<div class="ebiz-update-details-confirmation">
    <asp:Panel runat="server" ID="pnlAddMembership">
        <asp:Literal ID="contentLabel" runat="server" />
        <div class="ebiz-add-membership" runat="server" id="AddMembershipWrap">
            <asp:CheckBox ID="addMembershipCheck" runat="server" />
        </div>
        <asp:Button ID="returnButton" runat="server" CssClass="button return-button" />
        <asp:Button ID="addButton" runat="server" CssClass="button add-button" />
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlCapturePhoto">
        <input type="button" class="button" runat="server" id="capturePhotoBtn" />
    </asp:Panel>
</div>
