<%@ Page Language="VB" 
MasterPageFile="~/MasterPages/masterpage.master" 
AutoEventWireup="false"
CodeFile="GroupProductMaintenance.aspx.vb" 
Inherits="Navigation_GroupProductMaintenance" Title="Group Navigation Maintenance - Maintain Group Products" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <div id="pageMaintenanceTopNavigation">
       <asp:BulletedList ID="navigationOptions" runat="server" DisplayMode="HyperLink">
            <asp:listItem Text="Maintenance Portal" Value="../MaintenancePortal.aspx" />
       </asp:BulletedList>
    </div>

    <div id="pageOverview1">
        <p class="title">
            <asp:Label ID="titleLabel" runat="server" Text="Maintain Group Products"></asp:Label></p>
        <p class="instructions">
            <asp:Label ID="pathLabel" runat="server" Text="Path:"></asp:Label>
            <asp:Label ID="path" runat="server" Text=""></asp:Label></p>
        <p class="error">
            <asp:Label ID="ErrorLabel" runat="server"></asp:Label></p>
        <div class="pageOverview3">
        </div>
        <table>
            <tr>
                <th>
                    <asp:Label runat="server" ID="lblHdr1" Text="Products In Group"></asp:Label>
                </th>
                <th>
                    <asp:Label runat="server" ID="lblHdr2" Text="Adhoc Products In Group"></asp:Label>
                </th>
                <th>
                    &nbsp</th>
                <th>
                    <asp:Label runat="server" ID="Label1" Text="Products Not In Group"></asp:Label>
                </th>
            </tr>
            <tr>
                <td>
                    <asp:ListBox ID="lbxProductsIn" runat="server" Rows="20"></asp:ListBox></td>
                <td>
                    <asp:ListBox ID="lbxAdhocProductsIn" runat="server" Rows="20" SelectionMode="Multiple">
                    </asp:ListBox></td>
                <td>
                    <asp:Button ID="btnAdd" runat="server" Text="<" />
                    <br />
                    <asp:Button ID="btnRemove" runat="server" Text=">" />
                </td>
                <td>
                    <asp:ListBox ID="lbxProductsOut" runat="server" Rows="20" SelectionMode="Multiple"></asp:ListBox></td>
            </tr>
        </table>
    </div>
    <asp:Button ID="btnReturnToNavigation" runat="server" Text="Return To Navigation" />
</asp:Content>
