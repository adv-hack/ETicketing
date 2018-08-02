<%@ Page Language="VB" AutoEventWireup="false" CodeFile="orderDetails.aspx.vb" Inherits="PagesLogin_orderDetails" title="Untitled Page" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/OrderEnquiryDetails.ascx" TagName="OrderEnquiryDetails"
    TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
   <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:OrderEnquiryDetails ID="OrderEnquiryDetails1" runat="server" />
</asp:Content>

