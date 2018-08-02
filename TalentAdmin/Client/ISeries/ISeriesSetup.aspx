<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false"
    CodeFile="ISeriesSetup.aspx.vb" Inherits="_IseriesSetup" Title="Untitled Page" EnableSessionState="True" %>

<%@ Register Src="~/UserControls/IseriesSetup.ascx" TagName="ISeries" TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <Talent:ISeries ID="ISeries1" runat="server" />
</asp:Content>
