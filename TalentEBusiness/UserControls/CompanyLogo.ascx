<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CompanyLogo.ascx.vb" Inherits="UserControls_CompanyLogo" ViewStateMode="Disabled" %>
<div id="logo">
    <%--<a href="../../PagesPublic/Home/Home.aspx"><asp:Image ID="logoImage" runat="server"  AlternateText="logo" /></a>--%>
    <asp:HyperLink runat="server" ID="logoHyperlink"><asp:Image ID="logoImage" runat="server"  AlternateText="logo" /></asp:HyperLink>
</div>