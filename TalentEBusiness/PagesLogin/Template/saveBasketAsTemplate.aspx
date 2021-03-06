<%@ Page Language="VB" AutoEventWireup="false" CodeFile="saveBasketAsTemplate.aspx.vb" Inherits="PagesLogin_saveBasketAsTemplate" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/NewOrderTemplate.ascx" TagName="NewOrderTemplate" TagPrefix="uc1" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <uc1:NewOrderTemplate id="NewOrderTemplate1" runat="server" />
</asp:Content>