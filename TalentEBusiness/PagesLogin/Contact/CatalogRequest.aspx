<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CatalogRequest.aspx.vb" Inherits="PagesPublic_catalogRequest" Title="Untitled Page" %>
<%@ Register Src="../../UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/CatalogRequest.ascx" TagName="CatalogRequest" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />   
    <Talent:CatalogRequest ID="CatalogRequest" runat="server" />
</asp:Content>
