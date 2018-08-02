<%@ Page Language="VB" AutoEventWireup="false"
    CodeFile="CompetitionQuestionsConfirmation.aspx.vb" Inherits="PagesLogin_competitionQuestionsConfirmation"
    Title="Untitled Page" %>

<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <div id="PageHeaderText">
        <asp:Label ID="PageHeaderTextLabel" runat="server"></asp:Label>
    </div>
    <div id="ConfirmationText">
        <asp:Label ID="ConfirmationTextLabel" runat="server"></asp:Label>
    </div>
</asp:Content>
