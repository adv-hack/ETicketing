<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SeatPrintHistory.aspx.vb" Inherits="PagesLogin_Orders_SeatPrintHistory" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />

    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <asp:PlaceHolder ID="plhSeatPrintHistoryHeader" runat="server" Visible="false">
    <div class="panel ebiz-seat-print-history-header">
        <div class="row ebiz-product">
            <div class="large-3 columns">
                <asp:Literal ID="ltlProductLabel" runat="server" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="ltlProductValue" runat="server" />
            </div>
        </div>
        <asp:PlaceHolder ID="plhCustomerDetails" runat="server">
        <div class="row ebiz-customer">
            <div class="large-3 columns">
                <asp:Literal ID="ltlCustomerLabel" runat="server" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="ltlCustomerValue" runat="server" />
            </div>
        </div>
        </asp:PlaceHolder>
        <div class="row ebiz-seat">
            <div class="large-3 columns">
                <asp:Literal ID="ltlSeatLabel" runat="server" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="ltlSeatValue" runat="server" />
            </div>
        </div>
        <div class="row ebiz-tickets-printed">
            <div class="large-3 columns">
                <asp:Literal ID="ltlTicketsPrintedLabel" runat="server" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="ltlTicketsPrintedValue" runat="server" />
            </div>
        </div>
    </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhNoSeatPrintHistory" runat="server">
    <div class="alert-box info ebiz-no-seat-print-history">
        <asp:Literal ID="ltlNoSeatPrintHistory" runat="server" />
    </div>
    </asp:PlaceHolder>

    <asp:Repeater ID="rptSeatPrintHistory" runat="server">
        <HeaderTemplate>
            <table class="ebiz-seat-print-history">
                <thead>
                    <tr>
                        <th scope="col" class="ebiz-agent-name"><%# GetText("AgentColumn")%></th>
                        <th scope="col" class="ebiz-date"><%# GetText("DateColumn")%></th>
                        <th scope="col" class="ebiz-time"><%# GetText("TimeColumn")%></th>
                        <th scope="col" class="ebiz-customer-number"><%# GetText("CustomerNumberColumn")%></th>
                        <th scope="col" class="ebiz-customer-name"><%# GetText("CustomerNameColumn")%></th>
                        <th scope="col" class="ebiz-program"><%# GetText("ProgramNameColumn")%></th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
                    <tr>
                        <td class="ebiz-agent-name"><%# DataBinder.Eval(Container.DataItem, "Agent").ToString()%></td>
                        <td class="ebiz-date"><%# Talent.eCommerce.Utilities.GetAgentDate(DataBinder.Eval(Container.DataItem, "Date"))%></td>
                        <td class="ebiz-time"><%# DataBinder.Eval(Container.DataItem, "Time").ToString()%></td>
                        <td class="ebiz-customer-number"><%# DataBinder.Eval(Container.DataItem, "CustomerNumber").ToString().TrimStart(GlobalConstants.LEADING_ZEROS)%></td>
                        <td class="ebiz-customer-name"><%# DataBinder.Eval(Container.DataItem, "CustomerForename").ToString()%>&nbsp;<%# DataBinder.Eval(Container.DataItem, "CustomerSurname").ToString()%></td>
                        <td class="ebiz-program"><%# GetTranslatedProgramName(DataBinder.Eval(Container.DataItem, "Program").ToString())%></td>
                    </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
 
    <button class="close-button" data-close aria-label="Close modal" type="button">
      <span aria-hidden="true"><i class="fa fa-times"></i></span>
    </button>
     <script>
        $(document).ready(function () {
            setTimeout(function () {
                $(".ebiz-close").focus();
            }, 1);
        });
    </script>

</asp:Content>