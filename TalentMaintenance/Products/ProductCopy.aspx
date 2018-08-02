<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="ProductCopy.aspx.vb" Inherits="Products_ProductCopy" %>
<%@ Register Src="../UserControls/ProductMaintenanceTopNavigation.ascx" TagName="ProductMaintenanceTopNavigation"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" Runat="Server">
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">
    <uc1:ProductMaintenanceTopNavigation ID="ProductMaintenanceTopNavigation1" runat="server" />
    <p class="title">
        <asp:Label ID="titleLabel" runat="server" />
    </p>

    <div id="pageAddHtmlInclude1">
        <p class="error">
            <asp:Label ID="ErrLabel" runat="server" Text="" CssClass="error"></asp:Label>
        </p>
        <div class="pageAddHtmlInclude2">
            <table cellspacing="0" class="defaultTable">
                <tbody>
                    <tr>
                        <th class="label defaultTable_CopyProduct_col1" scope="row">
                            <asp:Label ID="productLabel" runat="server" Text="Product Code" />
                        </th>
                        <td class="element" colspan="2">
                            <asp:Label ID="product" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label defaultTable_CopyProduct_col1" scope="row">
                            <asp:Label ID="description1Label" runat="server" Text="Product Description #1" />
                        </th>
                        <td class="defaultTable_CopyProduct_col2">
                            <asp:CheckBox ID="description1Check" runat="server" />
                        </td>
                        <td class="defaultTable_CopyProduct_col3">
                            <asp:Label ID="textDescription1" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label defaultTable_CopyProduct_col1" scope="row">
                            <asp:Label ID="description2Label" runat="server" Text="Product Description #2" />
                        </th>
                        <td class="defaultTable_CopyProduct_col2">
                            <asp:CheckBox ID="description2Check" runat="server" />
                        </td>
                        <td class="defaultTable_CopyProduct_col3">
                            <asp:Label ID="textDescription2" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label defaultTable_CopyProduct_col1" scope="row">
                            <asp:Label ID="description3Label" runat="server" Text="Product Description #3" />
                        </th>
                        <td class="defaultTable_CopyProduct_col2">
                            <asp:CheckBox ID="description3Check" runat="server" />
                        </td>    
                        <td class="defaultTable_CopyProduct_col3">
                            <asp:Label ID="textDescription3" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label defaultTable_CopyProduct_col1" scope="row">
                            <asp:Label ID="description4Label" runat="server" Text="Product Description #4" />
                        </th>
                        <td class="defaultTable_CopyProduct_col2">
                            <asp:CheckBox ID="description4Check" runat="server" />
                        </td>    
                        <td class="defaultTable_CopyProduct_col3">
                            <asp:Label ID="textDescription4" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label defaultTable_CopyProduct_col1" scope="row">
                            <asp:Label ID="description5Label" runat="server" Text="Product Description #5" />
                        </th>
                        <td class="defaultTable_CopyProduct_col2">
                            <asp:CheckBox ID="description5Check" runat="server" />
                        </td>    
                        <td class="defaultTable_CopyProduct_col3">
                            <asp:Label ID="textDescription5" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label defaultTable_CopyProduct_col1" scope="row">
                            <asp:Label ID="HTML1Label" runat="server" Text="Product HTML #1" />
                        </th>
                        <td class="defaultTable_CopyProduct_col2">
                            <asp:CheckBox ID="HTML1Check" runat="server" />
                        </td>    
                        <td class="defaultTable_CopyProduct_col3">
                            <asp:Label ID="textHTML1" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label defaultTable_CopyProduct_col1" scope="row">
                            <asp:Label ID="HTML2Label" runat="server" Text="Product HTML #2" />
                        </th>
                        <td class="defaultTable_CopyProduct_col2">
                            <asp:CheckBox ID="HTML2Check" runat="server" />
                        </td>    
                        <td class="defaultTable_CopyProduct_col3">
                            <asp:Label ID="textHTML2" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label defaultTable_CopyProduct_col1" scope="row">
                            <asp:Label ID="HTML3Label" runat="server" Text="Product HTML #3" />
                        </th>
                        <td class="defaultTable_CopyProduct_col2">
                            <asp:CheckBox ID="HTML3Check" runat="server" />
                        </td>    
                        <td class="defaultTable_CopyProduct_col3">
                            <asp:Label ID="textHTML3" runat="server" Width="650" />
                        </td>
                    </tr>
                    <tr>
                        <th class="label" scope="row" style="vertical-align: top;">
                            <asp:Label ID="labelProductsToCopy" runat="server" Text="Copy to product(s), comma separated:" />
                        </th>
                        <td class="label" colspan="2">
                            <asp:TextBox ID="textProductsToCopy" runat="server" CssClass="textarea" Width="550" Height="100" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        
        <div class="pageAddHtmlIncludeButtons">
            <asp:Button ID="ConfirmButton" runat="server" Text="Confirm" CssClass="button" />
            <asp:Button ID="CancelButton" runat="server" Text="Cancel" CausesValidation="False" CssClass="button" />
            <asp:Button ID="btnClearForm" runat="Server" Text="Clear Selections" />
        </div>
    </div>
</asp:Content>

