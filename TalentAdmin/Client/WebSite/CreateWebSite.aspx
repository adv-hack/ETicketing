<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false"
    CodeFile="CreateWebSite.aspx.vb" Inherits="_CreateWebSite" Title="Untitled Page" EnableSessionState="True" %>

<%@ Register Src="~/UserControls/WebSiteDetails.ascx" TagName="SiteDetails" TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <Talent:SiteDetails ID="SiteDetails1" runat="server" />
</asp:Content>

