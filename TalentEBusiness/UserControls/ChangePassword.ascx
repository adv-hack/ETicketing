<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ChangePassword.ascx.vb" Inherits="UserControls_ChangePassword" %>

<asp:Panel ID="pnlPasswordBox" Visible="True" runat="server">
    <div id="ChangePasswordBox" class="panel ebiz-change-password" runat="server">
        <h2><asp:Literal ID="TitleLabel" runat="server" /></h2>
        <asp:PlaceHolder ID="plhInstructionsLabel" runat="server">
            <p class="ebiz-change-password-instructions"><asp:Literal ID="InstructionsLabel" runat="server" /></p>
        </asp:PlaceHolder>
        <a class="dummy_scroll_position_maintainer"></a>
        <asp:PlaceHolder ID="plhError" runat="server" Visible="false">
            <div class="alert-box alert"><asp:Literal ID="ErrorText" runat="server" EnableViewState="False" /></div>
        </asp:PlaceHolder>
        <script type="text/javascript" language="javascript">
            $(document).ready(
                    function () {
                        var $valSum = $(".dummy_scroll_position_maintainer");
                        if ($valSum.size() > 0) {
                            var backup_ValidationSummaryOnSubmit = ValidationSummaryOnSubmit;
                            ValidationSummaryOnSubmit = function (validationGroup) {
                                if (validationGroup == "ChangePasswordSummary") {
                                    var backup_ScrollTo = window.scrollTo;
                                    window.scrollTo = function () { };
                                    backup_ValidationSummaryOnSubmit(validationGroup);
                                    window.scrollTo = backup_ScrollTo;
                                    setTimeout(function () { $("body").scrollTo($valSum); }, 1);
                                }
                                else {
                                    backup_ValidationSummaryOnSubmit(validationGroup);
                                }
                            };
                        }
                    }

            );
        </script>
        <div class="row ebiz-current-password">
            <div class="medium-3 columns">
                <asp:Label ID="CurrentPasswordLabel" runat="server" AssociatedControlID="CurrentPassword" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="CurrentPassword" runat="server" TextMode="Password" />
                <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" ControlToValidate="CurrentPassword" SetFocusOnError="true" Visible="true" ValidationGroup="ChangePasswordSummary"
                    Display="Static" Enabled="true" CssClass="error" />
            </div>
        </div>
        <div class="row ebiz-new-password">
            <div class="medium-3 columns">
                <asp:Label ID="NewPasswordLabel" runat="server" AssociatedControlID="NewPassword" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="NewPassword" runat="server" TextMode="Password" />
                <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" ControlToValidate="NewPassword" SetFocusOnError="true" Visible="true" ValidationGroup="ChangePasswordSummary"
                    Enabled="true" Display="Static" CssClass="error" />
                <asp:RegularExpressionValidator ControlToValidate="NewPassword" ID="NewPasswordRegEx" runat="server" SetFocusOnError="true" Visible="true" ValidationGroup="ChangePasswordSummary"
                    Display="Static" Enabled="true" CssClass="error" />
            </div>
        </div>
        <div class="row ebiz-confirm-password">
            <div class="medium-3 columns">
                <asp:Label ID="ConfirmNewPasswordLabel" runat="server" AssociatedControlID="ConfirmNewPassword" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="ConfirmNewPassword" runat="server" TextMode="Password" />
                <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" ControlToValidate="ConfirmNewPassword" SetFocusOnError="true" Visible="true" ValidationGroup="ChangePasswordSummary"
                    Display="Static" Enabled="true" CssClass="error" />
                <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword" SetFocusOnError="true" Visible="true"
                    ValidationGroup="ChangePasswordSummary" Display="Static" Enabled="true" CssClass="error" />
            </div>
        </div>
        <div class="ebiz-change-password-button">
            <asp:Button ID="ChangeButton" runat="server" CommandName="ChangePassword" ValidationGroup="ChangePasswordSummary" CssClass="button ebiz-primary-action" CausesValidation="true" />
        </div>
    </div>
</asp:Panel>
