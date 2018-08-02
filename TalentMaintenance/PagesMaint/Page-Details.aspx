<%@ Page Language="VB" ValidateRequest="false" AutoEventWireup="false" CodeFile="Page-Details.aspx.vb"
    Inherits="PagesMaint_Page_Details" MasterPageFile="~/MasterPages/PageMaintenanceMasterPage.master"
    Title="Amend Page Details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div id="page-details">
        <h1><asp:Label ID="titleLabel" runat="server" /></h1>
        <p class="instructions"><asp:Label ID="instructionsLabel" runat="server" /></p>
        <asp:PlaceHolder ID="plhErrorMessage" runat="server">
            <p class="error"><asp:Label ID="ErrorLabel" runat="server" /></p>
        </asp:PlaceHolder>
        <asp:ValidationSummary ID="PageValidationSummary" CssClass="error" runat="server" />

        <div class="page-details-wrapper">
            <table cellspacing="0" class="page-details vertical">
                <tbody>
                    <tr class="bu">
                        <th scope="row"><asp:Label ID="BULabel" runat="server" /></th>
                        <td><asp:Label ID="BULabel1" runat="server" /></td>
                    </tr>
                    <tr class="partner">
                        <th scope="row"><asp:Label ID="PartnerLabel" runat="server" /></th>
                        <td><asp:Label ID="PartnerLabel1" runat="server" /></td>
                    </tr>
                    <tr class="page-name">
                        <th scope="row"><asp:Label ID="PageNameLabel" runat="server" /></th>
                        <td>
                            <asp:TextBox ID="PageNameTextBox" runat="server" MaxLength="100" ReadOnly="true" />
                            <asp:RequiredFieldValidator ID="PageNameRequiredFieldValidator" runat="server" ControlToValidate="PageNameTextBox" ErrorMessage=" Page Name is Required !" Text="*" />
                        </td>
                    </tr>
                    <tr class="description">
                        <th scope="row"><asp:Label ID="DescriptionLabel" runat="server" /></th>
                        <td>
                            <asp:TextBox ID="DescriptionTextBox" runat="server" CssClass="input-l" MaxLength="255" />
                            <asp:RequiredFieldValidator ID="DescriptionRequiredFieldValidator" runat="server" ControlToValidate="DescriptionTextBox" ErrorMessage="Page Description is Required !" Text="*" />
                        </td>
                    </tr>
                    <tr class="page-query">
                        <th scope="row"><asp:Label ID="PageQuerystringLabel" runat="server" /></th>
                        <td><asp:TextBox ID="PageQuerystringTextBox" runat="server" CssClass="input-l input-readonly" MaxLength="255" ReadOnly="True" /></td>
                    </tr>
                    <tr class="page-in-use">
                        <th scope="row"><asp:Label ID="PageInUseLabel" runat="server" /></th>
                        <td><asp:CheckBox ID="PageInUseCheckBox" runat="server" /></td>
                    </tr>
                    <tr class="use-secure-url">
                        <th scope="row"><asp:Label ID="UseSecureURLLabel" runat="server" /></th>
                        <td><asp:CheckBox ID="UseSecureURLCheckBox" runat="server" /></td>
                    </tr>
                    <tr class="html-in-use">
                        <th scope="row"><asp:Label ID="HTMLInUseLabel" runat="server" /></th>
                        <td><asp:CheckBox ID="HTMLInUseCheckBox" runat="server" /></td>
                    </tr>
                    <tr class="show-page-header">
                        <th scope="row"><asp:Label ID="ShowPageHeaderLabel" runat="server" /></th>
                        <td><asp:CheckBox ID="ShowPageHeaderCheckBox" runat="server" /></td>
                    </tr>
                    <tr class="force-login">
                        <th scope="row"><asp:Label ID="ForceLoginLabel" runat="server" /></th>
                        <td><asp:CheckBox ID="ForceLoginCheckBox" runat="server" /></td>
                    </tr>
                    <tr class="page-type">
                        <th scope="row"><asp:Label ID="PageTypeLabel" runat="server" /></th>
                        <td>
                            <asp:DropDownList ID="PageTpeDropDownList" runat="server" CssClass="select" Enabled="false">
                                <asp:ListItem Selected="True">STANDARD</asp:ListItem>
                                <asp:ListItem>PRODUCT</asp:ListItem>
                                <asp:ListItem>BROWSE</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr class="breadcrumb-trail-url">
                        <th scope="row"><asp:Label ID="BreadcrumbTrailURLLabel" runat="server" MaxLength="200" /></th>
                        <td><asp:TextBox ID="BreadcrumbTrailURLTextBox" runat="server" CssClass="input-l" MaxLength="200" /></td>
                    </tr>
                    <tr class="breadcrumb-trail-parent">
                        <th scope="row"><asp:Label ID="BreadcrumbTrailParentLabel" runat="server" /></th>
                        <td><asp:DropDownList ID="BreadcrumbTrailParentDropDownList" runat="server" CssClass="select" DataTextField="PAGE_CODE" DataValueField="PAGE_CODE" /></td>
                    </tr>
                    <tr class="template">
                        <th scope="row"><asp:Label ID="TemplateLabel" runat="server" /></th>
                        <td>
                            <p>
                                <asp:DropDownList ID="TemplateDropDownList" runat="server" CssClass="select" AutoPostBack="True" enabled="False" />
                                <a runat="server" id="lnkTemplate" href="#" onclick="MM_openBrWindow('Templateselection.aspx','','scrollbars=yes,width=900,height=600')">Visual Template Selection</a>
                            </p>
                            <p><img runat="server" alt="" id="TemplateImage" width="240" height="180" class="templateImage" /></p>
                        </td>
                    </tr>
                    <tr class="buttons">
                        <th>&nbsp;</th>
                        <td>
                            <asp:Button ID="ConfirmButton" runat="server" CssClass="confirm button" />
                            <asp:Button ID="CancelButton" runat="server" CausesValidation="False" CssClass="cancel button" />
                            <asp:Button ID="btnHideInMaintenance" runat="server" CssClass="hide-in-maintenance button" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <div class="page-detail-buttons button-bar">
            <asp:Button ID="ReturToPageButton" runat="server" CssClass="return-to-pages button" CausesValidation="False" />
            <asp:Button ID="PageHTButton" runat="server" CssClass="page-headers-and-titles button" CausesValidation="False" />
            <asp:Button ID="DefineHTMLButton" runat="server" CssClass="define-html-includes button" CausesValidation="False" />
            <asp:Button ID="EditControlsButton" runat="server" CssClass="edit-controls button" CausesValidation="False" />
        </div>
    </div>
</asp:Content>
