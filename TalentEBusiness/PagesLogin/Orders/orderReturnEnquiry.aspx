<%@ Page Language="VB" AutoEventWireup="false" CodeFile="orderReturnEnquiry.aspx.vb" Inherits="PagesLogin_orderReturnEnquiry" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/OrderReturnEnquiry.ascx" TagName="OrderReturnEnquiry" TagPrefix="Talent"%>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:OrderReturnEnquiry ID="OrderReturnEnquiry" runat="server" />
</asp:Content>