<%@ Page Language="VB" AutoEventWireup="false"
    CodeFile="CompetitionQuestions.aspx.vb" Inherits="PagesLogin_competitionQuestions"
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
    <div class="error">
        <asp:BulletedList ID="errorlist" runat="server">
        </asp:BulletedList>
    </div>
    <div class="competition-question">
        <asp:Label ID="QuestionLabel" runat="server"></asp:Label>
    </div>
    <div class="competition-answers">
        <asp:RadioButtonList ID="CompetitionAnswers" runat="server" CssClass="competition-answers">
        </asp:RadioButtonList>
    </div>
    <div id="buttons">
        <asp:Panel ID="ButtonsPanel" runat="server">
            <asp:Button ID="SubmitButton" CssClass="button submit" runat="server" Text="Submit" />
        </asp:Panel>
    </div>
</asp:Content>
