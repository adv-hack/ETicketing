<%@ Page Language="VB" AutoEventWireup="false"
    CodeFile="CatalogueRequestConfirmation.aspx.vb" Inherits="PagesPublic_catalogueRequestConfirmation"
    Title="Untitled Page" %>

<%@ Register Src="../../UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <div id="divFailedMessage">
        <asp:Literal ID="ltlFailedMessage" runat="server" Visible="false"></asp:Literal>
    </div>
</asp:Content>
