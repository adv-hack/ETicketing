<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EndOfDay.aspx.vb" Inherits="PagesLogin_Orders_EndOfDay" title="Untitled Page" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PrintPage.ascx" TagName="PrintPage" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/TransactionEnquiry.ascx" TagName="TransactionEnquiry" TagPrefix="uc1" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
        <uc1:TransactionEnquiry ID="uclTransactionEnquiry" runat="server" EndOfDay="true" />
        <Talent:PrintPage ID="PrintPage1" runat="server" />
</asp:Content>

