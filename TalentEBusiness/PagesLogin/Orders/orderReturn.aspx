<%@ Page Language="VB" AutoEventWireup="false" CodeFile="orderReturn.aspx.vb" Inherits="PagesLogin_orderReturn" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/OrderReturn.ascx" TagName="OrderReturn" TagPrefix="Talent"%>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:OrderReturn ID="OrderReturn" runat="server" />
</asp:Content>