<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="GeneratePasswordHash.aspx.vb" Inherits="Login_GeneratePasswordHash" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="server">
    <div id="pageMaintenanceTopNavigation">
        <ul>
            <li><a href="../Default.aspx"><asp:Literal ID="ltlHomeLink" runat="server" /></a></li>
        </ul>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">
    <div>
        Pasword to Encode:<asp:TextBox ID="PasswordBox" runat="server"  Width="200px"></asp:TextBox>
    </div>
    <div>
        Select Encryption Type:
        <asp:RadioButtonList ID="EncryptionTypeList" runat="server">
            <asp:ListItem Text="MD5" Value="MD5" Selected="True"></asp:ListItem>
            <asp:ListItem Text="SHA-1" Value="SHA1"></asp:ListItem>
        </asp:RadioButtonList>
    </div>
    <div>
        <asp:Button ID="GoButton" runat="server" Text="Create Hash" />
    </div>
    <div>
        <h3><asp:Label ID="HashTitleLabel" runat="server"></asp:Label></h3>
        <asp:Label ID="HashLabel" runat="server"></asp:Label>
    </div>
</asp:Content>