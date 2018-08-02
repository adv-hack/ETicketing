<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PageHeaderText.ascx.vb" Inherits="UserControls_PageHeaderText" ViewStateMode="Disabled" %>

<asp:Literal ID="ltlHeader" runat="server" />

<asp:placeholder id="plhProductBrowseIntro" runat="server" Visible="false">
    <div class="ebiz-browse-intro-text"><asp:Literal ID="ltlProductBrowseIntro" runat="server" /></div>
</asp:placeholder>

<asp:placeholder id="plhProductBrowseHTML" runat="server" Visible="false">
    <div class="ebiz-browse-html"><asp:Literal ID="ltlProductBrowseHTML" runat="server" /></div>
</asp:placeholder>
