<%@ Page Language="VB" AutoEventWireup="false" CodeFile="forgottenPassword.aspx.vb" Inherits="PagesPublic_forgottenPassword" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhErrorList" runat="server">
        <div class="alert-box alert">
            <asp:BulletedList ID="blErrorMessages" runat="server" />
        </div>
    </asp:PlaceHolder>
    <asp:ValidationSummary ID="vlsForgottenPassword" runat="server" DisplayMode="BulletList" ValidationGroup="ForgottenPassword" CssClass="alert-box alert ebiz-forgotten-password-validation-summary" />

    <asp:PlaceHolder ID="plhInitialPage" runat="server">
        <div class="panel ebiz-forgotten-password">
            <h2><asp:Literal ID="ltlForgottenPasswordTitleLabel" runat="server" /></h2>
            <fieldset>
                <legend><asp:Literal ID="ltlForgottenPasswordLegend" runat="server" /></legend>
                <asp:Literal ID="ltlForgottenPasswordInstructionsLabel" runat="server" />
                <div class="row ebiz-password">
                    <div class="medium-3 columns">
                        <asp:Label ID="lblForgottenPasswordLabel" runat="server" AssociatedControlID="txtForgottenPasswordTextBox" CssClass="middle" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtForgottenPasswordTextBox" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvForgottenPassword" runat="server" ValidationGroup="ForgottenPassword" ControlToValidate="txtForgottenPasswordTextBox" Display="Static"
                            CssClass="error ebiz-validator-error" />
                        <asp:RegularExpressionValidator ID="rgxForgottenPassword" runat="server" ValidationGroup="ForgottenPassword" ControlToValidate="txtForgottenPasswordTextBox" SetFocusOnError="true" Visible="true"
                            Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                </div>
                <asp:PlaceHolder ID="plhAddtionalOptions" runat="server" Visible="false">
                    <div class="ebiz-additional-options">
                        <asp:RadioButtonList ID="rblAddtionalOptions" runat="server" RepeatLayout="UnorderedList" />
                    </div>
                </asp:PlaceHolder>
                <asp:Button ID="btnForgottenPassword" CssClass="button" runat="server" ValidationGroup="ForgottenPassword" CausesValidation="true" />
            </fieldset>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhSuccessPage" runat ="server">
        <div class="ebiz-forgotten-password-success">
            <h2><asp:Literal ID="ltlForgottenPasswordSuccessTitleLabel" runat="server" /></h2>
            <div class="alert-box success">
                <asp:Literal ID="ltlForgottenPasswordSuccess" runat="server" /> 
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhUserSignedIn" runat="server">
        <div class="ebiz-user-signed-in">
            <h2><asp:Literal ID="ltlUserSignedInTitleLabel" runat="server" /></h2>
            <div class="alert-box warning">
                <asp:Literal ID="ltlUserSignedIn" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>
</asp:content>
