<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CreditFinanceSummary.ascx.vb" Inherits="UserControls_CreditFinanceSummary" %>

<asp:PlaceHolder ID="plhErrorList" runat="server">
    <div class="alert-box alert">
        <asp:BulletedList ID="ErrorList" runat="server" CssClass="error" />
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="pnlCreditFinanceSummary" runat="server" Visible="false">
    <div class="panel ebiz-cf-summary">
        <h2><asp:Literal ID="TitleLabel" runat="server" /></h2>

        <div class="row ebiz-cf-summary-pay-ref">
            <div class="large-3 columns">
                <asp:Label ID="PaymentReferenceLabel" runat="server" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="PaymentReferenceDetailLabel" runat="server" />
            </div>
        </div>
        <div class="row ebiz-cf-summary-account-name">
            <div class="large-3 columns">
                <asp:Label ID="AccountNameLabel" runat="server" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="AccountNameDetailLabel" runat="server" />
            </div>
        </div>
        <div class="row ebiz-cf-summary-sortcode">
            <div class="large-3 columns">
                <asp:Label ID="SortCodeLabel" runat="server" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="SortCodeDetailLabel" runat="server" />
            </div>
        </div>
        <div class="row ebiz-cf-summary-account-number">
            <div class="large-3 columns">
                <asp:Label ID="AccountNumberLabel" runat="server" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="AccountNumberDetailLabel" runat="server" />
            </div>
        </div>
        <div class="row ebiz-cf-summary-address-line1">
            <div class="large-3 columns">
                <asp:Label ID="Address1Label" runat="server" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="Address1DetailLabel" runat="server" />
            </div>
        </div>
        <div class="row ebiz-cf-summary-address-line2">
            <div class="large-3 columns">
                <asp:Label ID="Address2Label" runat="server" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="Address2DetailLabel" runat="server" />
            </div>
        </div>
        <div class="row ebiz-cf-summary-address-line3">
            <div class="large-3 columns">
                <asp:Label ID="Address3Label" runat="server" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="Address3DetailLabel" runat="server" />
            </div>
        </div>
        <div class="row ebiz-cf-summary-address-line4">
            <div class="large-3 columns">
                <asp:Label ID="Address4Label" runat="server" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="Address4DetailLabel" runat="server" />
            </div>
        </div>
        <div class="row ebiz-cf-summary-address-line5">
            <div class="large-3 columns">
                <asp:Label ID="Address5Label" runat="server" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="Address5DetailLabel" runat="server" />
            </div>
        </div>
        <div class="row ebiz-cf-summary-address-postcode">
            <div class="large-3 columns">
                <asp:Label ID="PostCodeLabel" runat="server" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="PostCodeDetailLabel" runat="server" />
            </div>
        </div>
        <div class="row ebiz-cf-summary-years-at-address">
            <div class="large-3 columns">
                <asp:Label ID="AddressYearsLabel" runat="server" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="AddressYearsDetailsLabel" runat="server" />
            </div>
        </div>
        <div class="row ebiz-cf-summary-installment-plan">
            <div class="large-3 columns">
                <asp:Label ID="InstallmentPlanLabel" runat="server" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="InstallmentPlanDetailLabel" runat="server" />
            </div>
        </div>
        <div class="row ebiz-cf-summary-installment-example">
            <div class="large-3 columns">
                <asp:Label ID="InstallmentExampleLabel" runat="server" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="InstallmentExampleDetailLabel" runat="server" />
            </div>
        </div>
    </div>
</asp:PlaceHolder>