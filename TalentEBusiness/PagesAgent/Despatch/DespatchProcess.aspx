<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DespatchProcess.aspx.vb" Inherits="PagesAgent_Despatch_DespatchProcess" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/DespatchProcess.ascx" TagName="DespatchProcess" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:DespatchProcess ID="uscDespatchProcess" runat="server" Usage="DESPATCH" />
</asp:Content>

