<%@ Page Language="VB" MasterPageFile="~/MasterPages/Shared/Blank.master" AutoEventWireup="false" CodeFile="AgentPreferences.aspx.vb" Inherits="PagesAgent_Profile_AgentPreferences" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/AgentPreferences.ascx" TagName="AgentPreferences" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/AgentsList.ascx" TagName="AgentsList" TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:AgentPreferences ID="uscAgentPreferences" runat="server" />
</asp:Content>