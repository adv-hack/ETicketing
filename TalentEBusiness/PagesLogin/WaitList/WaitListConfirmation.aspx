<%@ Page Language="VB" AutoEventWireup="false" CodeFile="WaitListConfirmation.aspx.vb" Inherits="PagesLogin_WaitList_WaitListConfirmation" title="Untitled Page" %>

<%@ Register Src="../../UserControls/SeasonTicketWaitListSummary.ascx" TagName="SeasonTicketWaitListSummary"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:SeasonTicketWaitListSummary ID="SeasonTicketWaitListSummary1" runat="server" Usage="ConfirmationPage" />
</asp:Content>

