<%@ Control Language="VB" AutoEventWireup="false" CodeFile="NotificationOptions.ascx.vb" Inherits="UserControls_NotificationOptions" %>

<div class="alert-box info ebiz-notification-options">
    <div class="ebiz-notification-options-email">
        <asp:PlaceHolder ID="plhEmailID" runat="server">
        <div class="row ebiz-notification-options-email-address">
            <div class="medium-3 columns">
                <asp:Label ID="lblEmailID" AssociatedControlID="txtEmailID" runat="server" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="txtEmailID" runat="server" />
                <asp:RequiredFieldValidator ID="rfvEmailID" runat="server" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" ControlToValidate="txtEmailID" />
                <asp:RegularExpressionValidator ID="regEmailID" runat="server" Display="Static" ControlToValidate="txtEmailID" CssClass="error ebiz-validator-error"></asp:RegularExpressionValidator>
            </div>
        </div>
        </asp:PlaceHolder>
        <div class="row">
            <asp:PlaceHolder ID="plhSendEmail" runat="server">
                <div class="column ebiz-notification-options-item ebiz-notification-options-email-send">
                    <asp:CheckBox ID="chkSendEmail" runat="server" Checked="true" />
                    <asp:Label ID="lblSendEmail" AssociatedControlID="chkSendEmail" runat="server" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhSaveEmail" runat="server">
                <div class="column ebiz-notification-options-item ebiz-notification-options-email-save">
                    <asp:CheckBox ID="chkSaveEmail" runat="server" Checked="true" />
                    <asp:Label ID="lblSaveEmail" AssociatedControlID="chkSaveEmail" runat="server" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhEMailPrefFlag" runat="server">
                <div class="column ebiz-notification-options-item ebiz-notification-options-email-contact">
                    <asp:CheckBox ID="chkEMailPrefFlag" runat="server" />
                    <asp:Label ID="lblEMailPrefFlag" AssociatedControlID="chkEMailPrefFlag" runat="server" />
                </div>
            </asp:PlaceHolder>
        </div>
    </div>

    <div class="ebiz-notification-options-mobile">
        <asp:PlaceHolder ID="plhMobileNumber" runat="server">
            <div class="row ebiz-notification-options-mobile-number">
                <div class="medium-3 columns">
                    <asp:Label ID="lblMobileNumber" AssociatedControlID="txtEmailID" runat="server" />
                </div>
                <div class="medium-9 columns">
                    <asp:TextBox ID="txtMobileNumber" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvMobileNumber" runat="server" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" ControlToValidate="txtMobileNumber" />
                    <asp:RegularExpressionValidator ID="regMobileNumber" runat="server" Display="Static" ControlToValidate="txtMobileNumber" CssClass="error ebiz-validator-error"></asp:RegularExpressionValidator>
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="row">
            <asp:PlaceHolder ID="plhSaveMobileNumber" runat="server">
                <div class="column ebiz-notification-options-item ebiz-notification-options-mobile-number-save">
                    <asp:CheckBox ID="chkSaveNumber" runat="server" Checked="true" />
                    <asp:Label ID="lblSaveNumber" AssociatedControlID="chkSaveNumber" CssClass="middle" runat="server" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhMobilePrefFlag" runat="server">
                <div class="column ebiz-notification-options-item ebiz-notification-options-mobile-number-contact">
                    <asp:CheckBox ID="chkMobilePrefFlag" runat="server" />
                    <asp:Label ID="lblMobilePrefFlag" AssociatedControlID="chkMobilePrefFlag" CssClass="middle" runat="server" />
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</div>
<asp:HiddenField ID="hdfCATMode" runat="server" />