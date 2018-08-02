<%@ Page Language="VB" AutoEventWireup="false" CodeFile="StatementEnquiry.aspx.vb" Inherits="PagesLogin_StatementEnquiry" title="Untitled Page" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/StatementEnquiry.ascx" TagName="StatementEnquiry" TagPrefix="uc1" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <uc1:StatementEnquiry ID="StatementEnquiry1" runat="server" />
</asp:Content>

