<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TicketingPPS.ascx.vb" Inherits="UserControls_TicketingPPS" ViewStateMode="Enabled" %>

<asp:PlaceHolder ID="plhPPSErrors" runat="server">
    <div id="alert-box alert">
        <asp:BulletedList ID="ErrorList" runat="server" CssClass="error" />
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhSchemeLockedMessage" runat="server">
    <div class="alert-box warning ebiz-scheme-locked">
        <asp:Literal ID="SchemeLockedMessage" runat="server" />
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhPPSList" runat="server">
    <div class="ebiz-pps-list">
        <asp:CheckBoxList ID="PPSList" runat="server" RepeatColumns="4" RepeatDirection="Horizontal" CellSpacing="0" />
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhNoSchemes" runat="server">
    <div class="ebiz-no-schemes">
        <asp:CheckBox ID="noSchemes" runat="server" />
    </div>
</asp:PlaceHolder>

<asp:HiddenField ID="hfProductCode" runat="server" />
<asp:HiddenField ID="hfPriceCode" runat="server" />

<asp:PlaceHolder ID="plhAmendPPSDetails" runat="server" Visible="false">
    <asp:Repeater ID="rptAmendPPSDetails" runat="server">
        <HeaderTemplate>
            <div class="ebiz-amendpps-item">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="<%# DataBinder.Eval(Container.DataItem, "CSSClass").ToString.Trim%>">
                <div class="ebiz-amendpps-customer-details">
                    <asp:HiddenField ID="hdfAmendPPSSeatDetails" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "SeatDetails")%>' />
                    <asp:HiddenField ID="hdfAmendPPSCustomerNumber" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "CustomerNumber")%>' />
                    <asp:CheckBox ID="chkAmendPPSEnrol" runat="server" Checked='<%# CBool(DataBinder.Eval(Container.DataItem, "Enrolled"))%>' Enabled='<%# SetAmendPPSEnrolEnabled(CBool(DataBinder.Eval(Container.DataItem, "Enrolled")))%>' />
                    <asp:Label ID="lblAmendPPSEnrol" runat="server" AssociatedControlID="chkAmendPPSEnrol">
                        <span class="ebiz-customer-number"><%# DataBinder.Eval(Container.DataItem, "CustomerNumber").ToString.TrimStart("0")%></span>
                        <span class="ebiz-customer-name"><%# DataBinder.Eval(Container.DataItem, "Name").ToString.Trim%></span>
                    </asp:Label>
                </div>
                <div class="row small-up-1 medium-up-4">
                    <div class="column ebiz-amendpps-stand">
                        <span class="ebiz-label"><%= StandLabel%></span>
                        <span class="ebiz-data"><%# Talent.eCommerce.Utilities.GetStandFromSeatDetails(DataBinder.Eval(Container.DataItem, "SeatDetails")).Trim%></span>
                    </div>
                    <div class="column ebiz-amendpps-area">
                        <span class="ebiz-label"><%= AreaLabel%></span>
                        <span class="ebiz-data"><%# Talent.eCommerce.Utilities.GetAreaFromSeatDetails(DataBinder.Eval(Container.DataItem, "SeatDetails")).Trim%></span>
                    </div>
                    <div class="column ebiz-amendpps-row">
                        <span class="ebiz-label"><%= RowLabel%></span>
                        <span class="ebiz-data"><%# Talent.eCommerce.Utilities.GetRowFromSeatDetails(DataBinder.Eval(Container.DataItem, "SeatDetails")).Trim%></span>
                    </div>
                    <div class="column ebiz-amendpps-seat">
                        <span class="ebiz-label"><%= SeatLabel %></span>
                        <span class="ebiz-data"><%# Talent.eCommerce.Utilities.GetSeatFromSeatDetails(DataBinder.Eval(Container.DataItem, "SeatDetails")).Trim%></span>
                    </div>
                </div>
                <asp:PlaceHolder ID="plhButtonGroup" runat="server">
                <div class="stacked-for-small button-group">
                    <asp:PlaceHolder ID="plhCancelPPSEnrollment" runat="server">
                        <asp:Button ID="btnCancelPPSEnrollment" runat="server" CssClass="button ebiz-cancel-pps-enrollment" CommandName="CancelPPSEnrollment" Text='<%# CancelPPSEnrollmentButtonText%>' />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhAmendPPSUpdate" runat="server" Visible='<%# CBool(DataBinder.Eval(Container.DataItem, "Enrolled"))%>'>
                        <asp:Button ID="btnUpdatePaymentDetails" runat="server" CssClass="button ebiz-primary-action ebiz-update-payments" CommandName="Update" Text='<%# UpdatePaymentsButtonText %>' />
                    </asp:PlaceHolder>
                </div>
                </asp:PlaceHolder>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
</asp:PlaceHolder>