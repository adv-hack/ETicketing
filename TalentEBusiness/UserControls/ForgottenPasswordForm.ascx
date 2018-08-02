<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ForgottenPasswordForm.ascx.vb" Inherits="UserControls_ForgottenPasswordForm" %>

<asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList" ValidationGroup="ForgottenPassword" CssClass="alert-box alert ebiz-forgotten-password-validation-summary" />

<div id="forgotten-password" class="panel ebiz-forgotten-password">
    <h2>
        <asp:Literal ID="ForgottenPasswordTitleLabel" runat="server" />
    </h2>
    <fieldset>
        <legend><asp:Literal ID="ltlForgottenPasswordLegend" runat="server" /></legend>
        <asp:Literal ID="ForgottenPasswordInstructionsLabel" runat="server" />
        <div class="row ebiz-password">
            <div class="medium-3 columns">
                <asp:Label ID="ForgottenPasswordLabel" runat="server" AssociatedControlID="ForgottenPasswordTextBox" CssClass="middle" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="ForgottenPasswordTextBox" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ForgottenPasswordRFV" runat="server" ValidationGroup="ForgottenPassword" ControlToValidate="ForgottenPasswordTextBox" Display="Static"
                    CssClass="error ebiz-validator-error"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="ForgottenPasswordRegEx" runat="server" ValidationGroup="ForgottenPassword" ControlToValidate="ForgottenPasswordTextBox" SetFocusOnError="true" Visible="true"
                    Display="Static" Enabled="true" CssClass="error ebiz-validator-error"></asp:RegularExpressionValidator>
                <asp:CustomValidator ID="CheckEmailAddress" Display="Static" ControlToValidate="ForgottenPasswordTextBox" runat="server" ValidationGroup="ForgottenPassword" Text="" CssClass="error ebiz-validator-error"></asp:CustomValidator>
            </div>
        </div>
        <asp:PlaceHolder ID="plhAddtionalOptions" runat="server" Visible="false">
            <div class="ebiz-additional-options">
                <asp:RadioButtonList ID="rblAddtionalOptions" runat="server" RepeatLayout="UnorderedList" />
            </div>
        </asp:PlaceHolder>
        <asp:Button ID="ForgottenPasswordButton" CssClass="button" runat="server" ValidationGroup="ForgottenPassword" CausesValidation="true" />
    </fieldset>
</div>
