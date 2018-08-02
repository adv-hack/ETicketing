<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SeatHistory.aspx.vb" Inherits="PagesLogin_Orders_SeatHistory" MasterPageFile="~/MasterPages/Shared/Blank.master" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhSeatHistoryHeader" runat="server" Visible="false">
        <div class="row ebiz-product">
            <div class="large-4 columns"><asp:Literal ID="ltlProductLabel" runat="server" /></div>
            <div class="large-8 columns"><asp:Literal ID="ltlProductValue" runat="server" /></div>
        </div>
        <asp:PlaceHolder ID="plhCustomerDetails" runat="server">
        <div class="row ebiz-customer">
            <div class="large-4 columns"><asp:Literal ID="ltlCustomerLabel" runat="server" /></div>
            <div class="large-8 columns"><asp:Literal ID="ltlCustomerValue" runat="server" /></div>
        </div>
        </asp:PlaceHolder>
        <div class="row ebiz-seat">
            <div class="large-4 columns"><asp:Literal ID="ltlSeatLabel" runat="server" /></div>
            <div class="large-8 columns"><asp:Literal ID="ltlSeatValue" runat="server" /></div>
        </div>
        <div class="row ebiz-seat-status">
            <div class="large-4 columns"><asp:Literal ID="ltlSeatStatusLabel" runat="server" /></div>
            <div class="large-8 columns"><asp:Literal ID="ltlSeatStatusValue" runat="server" /></div>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhNoSeatHistory" runat="server">
        <div class="alert-box info ebiz-no-seat-history">
            <asp:Literal ID="ltlNoSeatHistory" runat="server" />
        </div>
    </asp:PlaceHolder>
    <asp:Repeater ID="rptSeatHistory" runat="server">
        <HeaderTemplate>
            <table class="stack">
                <thead>
                    <tr>
                        <th scope="col" class="ebiz-agent-name"><%# GetText("AgentColumn")%></th>
                        <th scope="col" class="ebiz-date"><%# GetText("DateColumn")%></th>
                        <th scope="col" class="ebiz-time"><%# GetText("TimeColumn")%></th>
                        <th scope="col" class="ebiz-customer-number"><%# GetText("CustomerNumberColumn")%></th>
                        <th scope="col" class="ebiz-action"><%# GetText("ActionColumn")%></th>
                        <th scope="col" class="ebiz-batch"><%# GetText("BatchColumn")%></th>
                        <th scope="col" class="ebiz-payment-ref"><%# GetText("PaymentReferenceColumn")%></th>
                    </tr>
                </thead>
                <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td data-title="" class="ebiz-agent-name"><%# DataBinder.Eval(Container.DataItem, "Agent").ToString()%></th>
                    <td data-title="" class="ebiz-date"><%# Talent.eCommerce.Utilities.GetAgentDate(DataBinder.Eval(Container.DataItem, "Date"))%></th>
                    <td data-title="" class="ebiz-time"><%# DataBinder.Eval(Container.DataItem, "Time").ToString()%></th>
                    <td data-title="" class="ebiz-customer-number"><%# DataBinder.Eval(Container.DataItem, "CustomerNumber").ToString().TrimStart(GlobalConstants.LEADING_ZEROS)%></th>
                    <td data-title="" class="ebiz-action"><%# GetTranslatedActionName(DataBinder.Eval(Container.DataItem, "Action").ToString())%></th>
                    <td data-title="" class="ebiz-batch"><%# DataBinder.Eval(Container.DataItem, "Batch").ToString()%></th>
                    <td data-title="" class="ebiz-payment-ref"><%# DataBinder.Eval(Container.DataItem, "PaymentReference").ToString()%></th>
                </tr>
                <tr class="ebiz-comment-line1">
                    <td colspan="7"><%# GetFormattedCommentLine(DataBinder.Eval(Container.DataItem, "CommentLine1").ToString())%></td>
                </tr>
                <tr class="ebiz-comment-line2">
                    <td colspan="7"><%# GetFormattedCommentLine(DataBinder.Eval(Container.DataItem, "CommentLine2").ToString().TrimStart(GlobalConstants.LEADING_ZEROS))%></span></td>
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
  </asp:Content>