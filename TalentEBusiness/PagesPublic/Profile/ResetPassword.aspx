<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ResetPassword.aspx.vb" Inherits="PagesPublic_ResetPassword" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhErrorList" runat="server">
        <div class="alert-box alert">
            <asp:BulletedList ID="blErrorMessages" runat="server"/>
        </div>
    </asp:PlaceHolder>
    <asp:ValidationSummary ID="vlsResetPassword" runat="server" DisplayMode="BulletList" ValidationGroup="ResetPassword" CssClass="alert-box alert ebiz-reset-password-validation-summary" />

    <asp:PlaceHolder ID="plhInitialPage" runat="server">
        <div class="panel ebiz-Reset-password">
            <h2><asp:Literal ID="ltlResetPasswordTitle" runat="server" /></h2>
            <fieldset>
                <legend><asp:Literal ID="ltlResetPasswordLegend" runat="server" /></legend>
                <asp:Literal ID="ResetPasswordInstructionsLabel" runat="server" />
                <!--Password Box-->
                <div class="row ebiz-password">
                    <div class="medium-3 columns">
                        <asp:Label ID="lblResetPassword" runat="server" AssociatedControlID="txtResetPassword" CssClass="middle" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtResetPassword" runat="server" TextMode="Password" />
                        <asp:RequiredFieldValidator ID="rfvResetPassword" runat="server" ValidationGroup="ResetPassword" ControlToValidate="txtResetPassword" Display="Static"
                            CssClass="error ebiz-validator-error" />
                        <asp:RegularExpressionValidator ID="rgxResetPassword" runat="server" ValidationGroup="ResetPassword" ControlToValidate="txtResetPassword" SetFocusOnError="true" Visible="true"
                            Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                </div>
                <!--Confirm Password Box-->
                <div class="row ebiz-confirm-password">
                    <div class="medium-3 columns">
                        <asp:Label ID="lblResetPasswordConfirm" runat="server" AssociatedControlID="txtResetPasswordConfirm" CssClass="middle" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtResetPasswordConfirm" runat="server" TextMode="Password" />
                        <asp:RequiredFieldValidator ID="rfvResetPasswordConfirm" runat="server" ValidationGroup="ResetPassword" ControlToValidate="txtResetPasswordConfirm" Display="Static"
                            CssClass="error ebiz-validator-error" />
                        <asp:RegularExpressionValidator ID="rgxResetPasswordConfirm" runat="server" ValidationGroup="ResetPassword" ControlToValidate="txtResetPasswordConfirm" SetFocusOnError="true" Visible="true"
                            Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                        <asp:CompareValidator ID="cvNewPasswordCompare" runat="server" ControlToCompare="txtResetPassword" ControlToValidate="txtResetPasswordConfirm" SetFocusOnError="true" Visible="true"
                            ValidationGroup="ChangePasswordSummary" Display="Static" Enabled="true" CssClass="error" />
                    </div>
                </div>
                <!--Go! Button-->
                <asp:Button ID="btnResetPassword" CssClass="button" runat="server" ValidationGroup="ResetPassword" CausesValidation="true" />
            </fieldset>
        </div>
    </asp:PlaceHolder>

    <!--Success Page-->
    <asp:PlaceHolder ID="plhSuccessPage" runat="server">
        <div class="ebiz-reset-password-success">
            <h2><asp:Literal ID="ltlResetPasswordSuccessTitle" runat="server" /></h2>
            <div class="alert-box success">
                <asp:Literal ID="ltlResetPasswordSuccess" runat="server" /> 
            </div>
        </div>
    </asp:PlaceHolder>

    <!--User Signed In Page-->
    <asp:PlaceHolder ID="plhUserSignedIn" runat="server">
        <div class="ebiz-user-Signed-in">
            <h2><asp:Literal ID="ltlUserSignedInTitle" runat="server" /></h2>
            <div class="alert-box warning">
                <asp:Literal ID="ltlUserSignedIn" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>