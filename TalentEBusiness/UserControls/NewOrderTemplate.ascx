<%@ Control Language="VB" AutoEventWireup="false" CodeFile="NewOrderTemplate.ascx.vb" Inherits="UserControls_NewOrderTemplate" %>

<asp:PlaceHolder ID="plhUpdateTemplate" runat="server">
<div id="overwrite-template" class="default-box">
    <div id="overwriteTemplate" runat="server">
        <h2><asp:Label ID="lblOverwriteLabel" runat="server" /></h2>
        <div class="update-template">
            <fieldset>
                <legend><asp:Literal ID="ltlUpdateTemplateFieldsetLegend" runat="server" /></legend>
                <ul>
                    <li class="select select-template">
                        <asp:Label ID="lblSelectTemplate" runat="server" AssociatedControlID="ddlSelectTemplate" />
                        <asp:DropDownList ID="ddlSelectTemplate" runat="server" CssClass="select" />
                    </li>
                    <li class="btn update-button">
                        <asp:Button ID="UpdateButton" runat="server" CssClass="button" />
                    </li>
                </ul>
            </fieldset>
        </div>
    </div>
</div>
</asp:PlaceHolder>

<div id="save-template" class="default-box">
    <h2><asp:Label ID="lblNewTemplate" runat="server" /></h2>

    <asp:PlaceHolder ID="plhErrorMessage" runat="server">
        <p class="error"><asp:Label ID="errLabel" runat="server" /></p>
    </asp:PlaceHolder>
    <asp:Label ID="InstructionsLabel" runat="Server" CssClass="intructions" />
    <asp:ValidationSummary ID="NewTemplateValidationSummary" ValidationGroup="NewTemplate" runat="server" />

    <div class="new-template">
        <fieldset>
            <legend><asp:Literal ID="ltlNewTemplateFieldetLegend" runat="server" /></legend>
            <ul>
                <li class="text template-name">
                    <asp:Label ID="lblTemplateName" runat="server" AssociatedControlID="txtTemplateName" />
                    <asp:TextBox ID="txtTemplateName" runat="server" CssClass="input-l" />
                    <asp:RequiredFieldValidator ID="rfvTemplateName" runat="server" 
                        ControlToValidate="txtTemplateName" Display="Static" Text="*" ValidationGroup="NewTemplate" />
                </li>
                <li class="textarea template-description">
                    <asp:Label ID="lblTemplateDescription" runat="server" AssociatedControlID="txtTemplateDescription" />
                    <asp:TextBox ID="txtTemplateDescription" runat="server" TextMode="MultiLine" />
                </li>
                <li class="checkbox leave-items-in-basket">
                    <asp:CheckBox ID="chkLeaveOrderInBasket" runat="server" TextAlign="Left" />
                </li>
                <asp:PlaceHolder ID="plhAllowFriendsAndFamilyToView" runat="server">
                <li class="checkbox allow-ff-to-view">
                    <asp:CheckBox ID="chkAllowFFToView" runat="server" TextAlign="Left" />
                </li>
                </asp:PlaceHolder>
                <li class="btn save-template">
                    <asp:Button ID="btnSaveTemplate" runat="server" CausesValidation="true" ValidationGroup="NewTemplate" CssClass="button" />
                </li>
            </ul>
        </fieldset>
    </div>
</div>