<%@ Control Language="VB" AutoEventWireup="false" CodeFile="WebSiteDetails.ascx.vb" Inherits="UserControls_WebSiteDetails" %>

<div id="SiteDetails"> 
    <div class="title">
        <asp:Label ID="lblSiteHeader" runat="server">Create Website</asp:Label>
    </div>
    <div class="details">
        <div class="item">
            <asp:ValidationSummary ID="SiteDetailsSummary" runat="server" ValidationGroup="SiteDetails" />
        </div>
        <div class="item">
            <asp:Label ID="lblSiteType" runat="server">Website Type:</asp:Label>
            <asp:DropDownList ID="ddlSiteType" runat="server" AutoPostBack="true"></asp:DropDownList>
        </div>
        <div class="item">
            <asp:Label ID="lblSiteFormat" runat="server">Website Format:</asp:Label>
            <asp:DropDownList ID="ddlSiteFormat" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSiteFormat_SelectedIndexChanged"></asp:DropDownList>
        </div>
        <div class="item">
            <asp:Label ID="lblSiteName" runat="server">Website Name:</asp:Label>
            <asp:DropDownList ID="ddlSiteName" runat="server"></asp:DropDownList>
        </div>
        <div class="item">
            <asp:Label ID="lblWebServer" runat="server">Web Server:</asp:Label>
            <asp:DropDownList ID="ddlWebServer" runat="server"></asp:DropDownList>
        </div>
        <div class="item">
            <asp:Label ID="lblUpgradePath" runat="server">Upgrade Path:</asp:Label>
            <asp:TextBox ID="txtUpgradePath" runat="server" CssClass="input-l"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvUpgradePath" runat="server" Text="*" Display="Dynamic"
            ValidationGroup="SiteDetails" ControlToValidate="txtUpgradePath"></asp:RequiredFieldValidator>
        </div>
        <div class="item">
            <asp:Label ID="lblSiteUrl" runat="server">Website Url:</asp:Label>
            <asp:TextBox ID="txtSiteUrl" runat="server" CssClass="input-l"></asp:TextBox>
            <asp:RequiredFieldValidator ID="rfvSiteUrl" runat="server" Text="*" Display="Dynamic"
            ValidationGroup="SiteDetails" ControlToValidate="txtSiteUrl"></asp:RequiredFieldValidator>
        </div>
        <div class="item">
            <asp:Label ID="lblSslCertPath" runat="server">SSL Certificate Path:</asp:Label>
            <asp:TextBox ID="txtSslCertPath" runat="server" CssClass="input-l"></asp:TextBox>
        </div>
        <div class="item">
            <asp:Button ID="btnCreateWebSite" runat="server" Text="Create Web Site" ValidationGroup="SiteDetails"/>
            <asp:Label ID="lblCreateWebSite" runat="server"></asp:Label>
        </div>
    </div>       
</div>
