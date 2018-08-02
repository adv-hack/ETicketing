<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SmartcardEventMaintenance.aspx.vb" Inherits="PagesAgent_Smartcard_SmartcardEventMaintenance" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/SmartcardEventMaintenance.ascx" TagName="SmartcardEventMaintenance" TagPrefix="Talent"%>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:SmartcardEventMaintenance ID="SmartcardEventMaintenance" runat="server" />
</asp:Content>