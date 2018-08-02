<%@ Page Language="VB"  AutoEventWireup="false"
    CodeFile="advancedSearch.aspx.vb" Inherits="PagesPublic_advancedSearch" Title="Untitled Page" %>
<%@ Register Src="~/UserControls/advancedSearchForm.ascx" TagName="advancedSearchForm"
    TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
   <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <asp:Label ID="bodyLabel1" runat="server"></asp:Label>
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:advancedSearchForm ID="AdvancedSearchForm1" runat="server" />
</asp:Content>
