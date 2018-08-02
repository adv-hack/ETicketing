<%@ Page Language="VB" AutoEventWireup="false" CodeFile="InvoiceEnquiryDetail.aspx.vb" Inherits="PagesLogin_invoiceEnquiryDetail" title="Untitled Page" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/InvoiceEnquiryDetail.ascx" TagName="InvoiceEnquiryDetail" TagPrefix="uc1" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <uc1:InvoiceEnquiryDetail ID="InvoiceEnquiryDetail1" runat="server" />
</asp:Content>

