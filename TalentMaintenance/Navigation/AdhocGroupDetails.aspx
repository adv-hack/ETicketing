<%@ Page Language="VB" 
ValidateRequest="false" 
AutoEventWireup="false" 
CodeFile="AdhocGroupDetails.aspx.vb"
Inherits="Navigation_AdhocGroupDetails" 
MasterPageFile="~/MasterPages/MasterPage.master" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" Runat="Server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <div id="pageMaintenanceTopNavigation">
       <asp:BulletedList ID="navigationOptions" runat="server" DisplayMode="HyperLink">
            <asp:listItem Text="Maintenance Portal" Value="../MaintenancePortal.aspx" />
       </asp:BulletedList>
    </div>
    
    <div id="pageDetails1">
        <p class="title"><asp:Label ID="titleLabel" runat="server" /></p>
        <p class="instructions"><asp:Label ID="instructionsLabel" runat="server" /></p>
        <p class="error"><asp:Label ID="ErrorLabel" runat="server" /></p>
        <p class="validationSummary"><asp:ValidationSummary ID="PageValidationSummary" runat="server" /></p>
        
        <div class="pageDetails2">
            <table cellspacing="0" class="defaultTable">
                <tbody>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="GroupNameLabel" runat="server" Text="Group Name" />
                        </th>
                        <td class="element">
                            <asp:TextBox ID="GroupNameTextBox" CssClass="input-l" runat="server" MaxLength="50" ReadOnly="true" />
                            <asp:RequiredFieldValidator ID="GroupNameRequiredFieldValidator" runat="server" ControlToValidate="GroupNameTextBox"
                                ErrorMessage=" Group Name is Required !">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rgxGroupNameTextBox" runat="server" ControlToValidate="GroupNameTextBox" 
                                ValidationExpression="^[A-Za-z0-9\s-]+" ErrorMessage="Please enter a valid Group name" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="Description1Label" runat="server" Text="Description 1" />
                        </th>
                        <td class="element">
                            <asp:TextBox ID="Description1TextBox" runat="server" CssClass="input-l" MaxLength="255" />
                            <asp:RequiredFieldValidator ID="Description1RequiredFieldValidator" runat="server"
                                ControlToValidate="Description1TextBox" ErrorMessage="Group Description 1 is Required !">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="Description2Label" runat="server" Text="Description 2" />
                        </th>
                        <td class="element">
                            <asp:TextBox ID="Description2TextBox" runat="server" CssClass="input-l" MaxLength="255" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="Html1Label" runat="server" Text="HTML 1" />
                        </th>
                        <td class="element">
                            <asp:TextBox ID="Html1TextBox" runat="server" CssClass="input-l" MaxLength="255" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="Html2Label" runat="server" Text="HTML 2" />
                        </th>
                        <td class="element">
                            <asp:TextBox ID="Html2TextBox" runat="server" CssClass="input-l" MaxLength="255" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="Html3Label" runat="server" Text="HTML 3" />
                        </th>
                        <td class="element">
                            <asp:TextBox ID="Html3TextBox" runat="server" CssClass="input-l" MaxLength="255" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="PageTitleLabel" runat="server" Text="Page Title" />
                        </th>
                        <td class="element">
                            <asp:TextBox ID="PageTitleTextBox" runat="server" CssClass="input-l" MaxLength="255" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="MetaDescLabel" runat="server" Text="Meta Description" />
                        </th>
                        <td class="element">
                            <asp:TextBox ID="MetaDescTextBox" runat="server" CssClass="input-l" MaxLength="255" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="MetaKeysLabel" runat="server" Text="Meta Keywords" />
                        </th>
                        <td class="element">
                            <asp:TextBox ID="MetaKeysTextBox" runat="server" CssClass="input-l" MaxLength="255" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        
        <div class="pageDetailButtons">
            <asp:Button ID="ReturnToGroupsButton" runat="server" Text="Return To Groups" CssClass="buttonPage" CausesValidation="False" />
            <asp:Button ID="ConfirmButton" runat="server" CssClass="button" Text="Confirm" />
            <asp:Button ID="btnDeleteGroup" runat="server" CssClass="button" Text="Delete Group" />
        </div>
    </div>
</asp:Content>
