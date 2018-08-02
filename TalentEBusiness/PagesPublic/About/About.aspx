<%@ Page Language="VB" AutoEventWireup="false" CodeFile="About.aspx.vb" Inherits="PagesPublic_About_About" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
<div>
    <h1>About Page</h1>
    <table>
        <tbody>
            <tr>
                <td colspan="2"><asp:Label ID="AboutPageLabel" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td><b>Process Type:</b></td>
                <td><asp:Label ID="ProcessTypeLabel" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td><b>Application Version:</b></td>
                <td><asp:Label ID="VersionLabel" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td><b>Upgrade Date:</b></td>
                <td><asp:Label ID="UpgradeDateLabel" runat="server"></asp:Label></td>
            </tr>
        </tbody>
    </table>
</div>
</asp:Content>
