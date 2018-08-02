<%@ Page Language="VB" AutoEventWireup="false" CodeFile="searchResults.aspx.vb" Inherits="PagesPublic_searchResults" title="Untitled Page" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/ProductSearchResultsList.ascx" TagName="SearchResultsList" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/ProductSearchResultsList2.ascx" TagName="SearchResultsList2" TagPrefix="Talent" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<Talent:HTMLInclude ID="Basket_HTMLInclude1" runat="server" Usage="2" Sequence="1" />
   <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:SearchResultsList ID="ProductSearchResultsList" runat="server" />
    <Talent:SearchResultsList2 ID="ProductSearchResultsList2" runat="server" />
    <div id="search-again"><asp:Button ID="btnSearchAgain" CssClass="button" runat="server"/></div>

    
</asp:Content>

