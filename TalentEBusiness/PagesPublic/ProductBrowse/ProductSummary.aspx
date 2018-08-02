<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPages/Shared/Blank.master" AutoEventWireup="false" CodeFile="ProductSummary.aspx.vb" Inherits="PagesPublic_ProductBrowse_ProductSummary" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProductSummary.ascx" TagName="ProductSummary" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:ProductSummary ID="ProductSummary1" runat="server" />
    <button class="close-button" data-close aria-label="Close modal" type="button">
    	<i class="fa fa-times"></i>
  	</button>
</asp:Content>
