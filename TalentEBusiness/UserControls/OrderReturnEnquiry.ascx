<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OrderReturnEnquiry.ascx.vb" Inherits="UserControls_OrderReturnEnquiry" %>

<asp:PlaceHolder ID="plhErrorList" runat="server">
    <div class="alert-box alert">
        <asp:BulletedList ID="errorlist" runat="server" />
    </div>
</asp:PlaceHolder>

<asp:Repeater ID="CustomerRepeater" runat="server">
    <ItemTemplate>
        <div class="panel ebiz-buyback">
            <div class="row ebiz-buyback-customer">
                <div class="large-3 columns"><asp:Literal ID="ltlCustomerLabel" runat="server" /></div>
                <div class="large-9 columns"><asp:Literal ID="ltlCustomerValue" runat="server" /><asp:HiddenField ID="hdfCustomerNumber" runat="server" /></div>
            </div>
            <asp:Repeater ID="OrderRepeater" runat="server" OnItemDataBound="DoOrdersRepeaterItemDatabound">
                <HeaderTemplate>
                    <table>
                        <thead>
                            <tr>
                                <th class="ebiz-buyback-membership-match" scope="col"><%#GetText("membershipMatchLabel")%></th>
                                <th class="ebiz-buyback-date" scope="col"><%#GetText("dateLabel")%></th>
                                <th class="ebiz-buyback-seat" scope="col"><%#GetText("seatLabel")%></th>
                                <th class="ebiz-buyback-status" scope="col"><%#GetText("statusLabel")%></th>
                                <th class="ebiz-buyback-selection" scope="col">&nbsp;</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                            <tr>
                                <td class="ebiz-buyback-membership-match" data-title="<%#GetText("membershipMatchLabel")%>"><asp:Literal ID="matchLabel" runat="server" /></td>
                                <td class="ebiz-buyback-date" data-title="<%#GetText("dateLabel")%>"><asp:Literal ID="dateLabel" runat="server" /></td>
                                <td class="ebiz-buyback-seat" data-title="<%#GetText("seatLabel")%>"><asp:Literal ID="seatLabel" runat="server" /></td>
                                <td class="ebiz-buyback-status" data-title="<%#GetText("statusLabel")%>">
                                    <asp:Literal ID="statusLabel" runat="server" />
                                    <asp:HiddenField ID="hfProductCode" runat="server" />
                                    <asp:HiddenField ID="hfStatus" runat="server" />
                                    <asp:HiddenField ID="hfSeat" runat="server" />
                                </td>
                                <td class="ebiz-buyback-selection"><asp:CheckBox ID="OrderSelectionCBox" runat="server" /></td>
                            </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </ItemTemplate>
</asp:Repeater>

<asp:Panel ID="ButtonsPanel" runat="server" CssClass="ebiz-buyback-continue-button">
    <asp:Button ID="ContinueButton" CssClass="button ebiz-primary-action" runat="server" />
</asp:Panel>