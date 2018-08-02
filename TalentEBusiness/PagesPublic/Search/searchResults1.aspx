<%@ Page Language="VB" AutoEventWireup="false" CodeFile="searchResults1.aspx.vb" Inherits="PagesPublic_searchResults1" title="Untitled Page" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
   <Talent:HTMLInclude ID="Basket_HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <iframe runat="server" id="iframeOcelluz" width="761" height="720"  style="border:none" frameborder="0" scrolling="auto"  >
    </iframe>  
    
</asp:Content>

