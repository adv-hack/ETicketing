<%@ Control Language="VB" AutoEventWireup="false" CodeFile="LoginBox.ascx.vb" Inherits="UserControls_LoginBox" ViewStateMode="Disabled" %>
<div class="panel ebiz-sign-in">
    <asp:PlaceHolder ID="plhLogin" runat="server">
        <script type="text/javascript">
            function AppendUsername(canAppend, txtUsername) {
                if (canAppend == "true") {
                    if ($.isNumeric(document.getElementById(txtUsername).value)) {
                        if (parseInt(document.getElementById(txtUsername).value) == (document.getElementById(txtUsername).value)) {
                            document.getElementById(txtUsername).value = $.strPad(document.getElementById(txtUsername).value, 12);
                        }
                    }
                }
            }
            $.strPad = function (i, l, s) {
                var o = i.toString();
                if (!s) { s = '0'; }
                while (o.length < l) {
                    o = s + o;
                }
                return o;
            };
        </script>
        <asp:Login ID="Login1" runat="server" summary="login box" RenderOuterTable="false">
            <LayoutTemplate>
                <asp:PlaceHolder ID="plhTitle" runat="server">
                    <h2>
                        
                            <asp:Literal ID="TitleLabel" runat="server" OnPreRender="GetText" /></h2>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhLoginFailure" runat="server" Visible="false">
                    <div class="alert-box alert ebiz-login-failure">
                        <asp:Label ID="SingularErrorLabel" runat="server" Visible="false" EnableViewState="false" />
                        <asp:Label ID="ErrorLabel" runat="server" EnableViewState="False" Visible="false" />
                        <asp:Literal ID="PasswordFailureText" runat="server" EnableViewState="False" OnPreRender="GetText" Visible="false" />
                        <asp:Literal ID="UsernameFailureText" runat="server" EnableViewState="False" OnPreRender="GetText" Visible="false" />
                        <asp:Literal ID="DuplicateEmailFailureText" runat="server" EnableViewState="False" OnPreRender="GetText" Visible="false" />
                        <asp:ValidationSummary ID="LoginBoxValidationSummary" runat="server" ValidationGroup="ctl00$Login1" DisplayMode="List" CssClass="LoginBoxValidationSummary" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhInstructions" runat="server">
                    <p class="ebiz-instructions">
                        <asp:Literal ID="LoginInstructionsLabel" runat="server" OnPreRender="GetText" />
                    </p>
                </asp:PlaceHolder>
                <fieldset>
                    <legend><asp:Literal ID="ltlLoginLegend" runat="server" /></legend>
                    <div class="row ebiz-user-name">
                        <div class="medium-3 columns">
                            <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" OnPreRender="GetText" CssClass="middle" />
                        </div>
                        <div class="medium-9 columns">
                            <asp:TextBox ID="UserName" runat="server" OnPreRender="SetEnterKeyAction" />
                            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" OnPreRender="GetText" ValidationGroup="ctl00$Login1" Display="Static" CssClass="error"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row ebiz-password">
                        <div class="medium-3 columns">
                            <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password" OnPreRender="GetText" CssClass="middle" />
                        </div>
                        <div class="medium-9 columns">
                            <asp:TextBox ID="Password" runat="server" TextMode="Password" OnPreRender="SetEnterKeyAction" />
                            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" OnPreRender="GetText" ValidationGroup="ctl00$Login1" Display="Static" CssClass="error"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <asp:PlaceHolder ID="plhPasswordLink" runat="server">
                        <div class="row ebiz-password-link">
                            <div class="medium-9 columns push-3">
                                <asp:Literal ID="ltlPasswordLink" runat="server" OnPreRender="GetText" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhAddtionalOptions" runat="server" Visible="false">
                        <div class="ebiz-additional-options">
                            <asp:RadioButtonList ID="rblAddtionalOptions" runat="server" RepeatLayout="UnorderedList" />
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhRemeberMe" runat="server">
                        <div class="ebiz-remember-me">
                            <asp:CheckBox ID="RememberMe" runat="server" OnPreRender="GetText" />
                            <asp:PlaceHolder ID="plhSecurityMessage" runat="server">
                                <span class="ebiz-security-message">
                                    <asp:HyperLink ID="hplSecurityMessage" data-open="security-message-modal" CssClass="ebiz-open-modal" runat="server" NavigateUrl="~/PagesPublic/Login/RememberMeWindow.aspx" />
                                    <div id="security-message-modal" class="reveal ebiz-reveal-ajax" data-reveal></div>
                                </span>
                            </asp:PlaceHolder>
                        </div>
                    </asp:PlaceHolder>
                    <div class="button-group">
                        <asp:Button ID="LoginButton" runat="server" CommandName="Login" OnPreRender="GetText" ValidationGroup="ctl00$Login1" CssClass="button ebiz-login" />
                        <asp:PlaceHolder ID="plhRegisterLink" runat="server" OnPreRender="GetText">
                            <asp:HyperLink ID="RegisterLink" runat="server" OnPreRender="GetText" CssClass="button ebiz-register" />
                        </asp:PlaceHolder>
                    </div>
                    <asp:PlaceHolder ID="plhForgottenPassword" runat="server">
                        <div class="ebiz-forgotten-password-link">
                            <asp:HyperLink ID="ForgottenPasswordLink" runat="server" OnPreRender="GetText" />
                        </div>
                    </asp:PlaceHolder>
                </fieldset>
            </LayoutTemplate>
        </asp:Login>
        <asp:Login ID="Login2" runat="server" summary="login box">
            <LayoutTemplate>
                <div class="instructions">
                    <asp:Label ID="LoginInstructionsLabel" runat="server" OnPreRender="GetText" />
                    <asp:TextBox ID="UserName" runat="server" OnPreRender="SetEnterKeyAction" />
                    <asp:TextBox ID="Password" runat="server" OnPreRender="SetEnterKeyAction" TextMode="Password" />
                    <asp:Button ID="LoginButton" runat="server" CommandName="Login" OnPreRender="GetText" ValidationGroup="ctl00$Login2" CssClass="button" />
                </div>
            </LayoutTemplate>
        </asp:Login>
        <asp:LoginStatus ID="LoginStatus1" runat="server" />
    </asp:PlaceHolder>
</div>

