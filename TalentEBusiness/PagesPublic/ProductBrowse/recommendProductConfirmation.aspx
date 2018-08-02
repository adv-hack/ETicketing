<%@ Page Language="VB" AutoEventWireup="false"
    CodeFile="recommendProductConfirmation.aspx.vb" Inherits="PagesPublic_recommendProductConfirmation"
    Title="Untitled Page" %>

<%@ Register Src="../../UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <asp:Label ID="MessageText" runat="server"></asp:Label>
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <p class="Home-Link">
        <asp:HyperLink ID="HomeLink" runat="server" OnPreRender="GetText"></asp:HyperLink></p>
</asp:Content>
