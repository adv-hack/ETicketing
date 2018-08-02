<%@ Page Language="VB" AutoEventWireup="false" CodeFile="editTemplate.aspx.vb" Inherits="PagesLogin_editTemplate" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/OrderTemplateDetails.ascx" TagName="OrderTemplateDetails" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:OrderTemplateDetails id="OrderTemplateDetails1" runat="server" Usage="TEMPLATES" />
</asp:Content>