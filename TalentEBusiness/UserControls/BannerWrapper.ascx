<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BannerWrapper.ascx.vb" Inherits="UserControls_BannerWrapper" %>
<%@ Register Src="Banners.ascx" TagName="Banners1" TagPrefix="Talent" %>

<asp:Panel ID="pnlBannerWrapper" runat="server">
    <div class="banners">
    <Talent:Banners1 ID="bannersUC" runat="server"></Talent:Banners1>
    </div>
</asp:Panel>