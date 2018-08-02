<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ISeriesSetup.ascx.vb" Inherits="UserControls_IseriesSetup" %>

<div id="SiteDetails"> 
    <div class="title">
        <asp:Label ID="lblSiteHeader" runat="server">Configure I-Series</asp:Label>
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
            <asp:Button ID="btnConfigureISeries" runat="server" Text="Configure I-Series" ValidationGroup="SiteDetails"/>
            <asp:Label ID="lblConfigureISeries" runat="server"></asp:Label>
        </div>
    </div>       
</div>
