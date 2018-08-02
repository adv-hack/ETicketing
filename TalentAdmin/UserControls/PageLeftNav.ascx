<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PageLeftNav.ascx.vb" Inherits="UserControls_PageLeftNav" %>
<asp:BulletedList ID="navigationOptions" runat="server" DisplayMode="HyperLink">
    <asp:listItem Text="Home" Value="~/Default.aspx"  />
    <asp:listItem Text="Create Web Site" Value="~/Client/WebSite/CreateWebSite.aspx"  />
    <asp:listItem Text="Database Upgrade" Value="~/Client/WebSite/DatabaseAdmin.aspx"  />
    <asp:listItem Text="Copy BU" Value="~/Server/WebSite/CopyBU/CopyBUSelection.aspx"  />
</asp:BulletedList>

