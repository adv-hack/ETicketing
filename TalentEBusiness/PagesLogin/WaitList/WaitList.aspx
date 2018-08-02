<%@ Page Language="VB" AutoEventWireup="false" CodeFile="WaitList.aspx.vb" Inherits="PagesLogin_WaitList_WaitList" EnableEventValidation="false" %>

<%@ Register Src="../../UserControls/SeasonTicketWaitList.ascx" TagName="SeasonTicketWaitList"
    TagPrefix="uc1" %>
<%@ Register Src="../../UserControls/SeasonTicketWaitListSummary.ascx" TagName="SeasonTicketWaitListSummary"
    TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="error-list">
        <asp:BulletedList ID="errorList" runat="server"></asp:BulletedList>
    </div>
    <uc2:SeasonTicketWaitListSummary ID="SeasonTicketWaitListSummary1" runat="server" Usage="WaitListPage" />
    <uc1:SeasonTicketWaitList ID="SeasonTicketWaitList1" runat="server" />
</asp:Content>

