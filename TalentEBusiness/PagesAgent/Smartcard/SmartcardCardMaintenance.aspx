<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SmartcardCardMaintenance.aspx.vb" Inherits="PagesAgent_Smartcard_SmartcardCardMaintenance" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/SmartcardCardMaintenance.ascx" TagName="SmartcardCardMaintenance" TagPrefix="Talent"%>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:SmartcardCardMaintenance ID="SmartcardCardMaintenance" runat="server" />
</asp:Content>