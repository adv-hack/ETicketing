<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AmendPPSPayments.aspx.vb" Inherits="PagesPublic_ProductBrowse_AmendPPSPayments" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PaymentDetails.ascx" TagName="PaymentDetails" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PaymentCardDetails.ascx" TagName="PaymentCardDetails" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <asp:PlaceHolder ID="plhNoPaymentDetails" runat="server">
        <div class="alert-box warning"><asp:Literal ID="ltlNoPaymentDetails" runat="server" /></div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhErrorMessage" runat="server" Visible="false">
        <div class="alert-box alert"><asp:Literal ID="ltlErrorMessage" runat="server" /></div>
    </asp:PlaceHolder>
    <div id="vlsPaymentCardDetailsErrors" class="alert-box alert" style="display:none;">
            <ul id="PaymentCardDetailsErrors">
            </ul>
	</div>
    <asp:PlaceHolder ID="plhSuccessMessage" runat="server" Visible="false">
        <div class="alert-box success"><asp:Literal ID="ltlSuccessMessage" runat="server" /></div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhShowPaymentDetails" runat="server" visible="false">
        <div class="panel ebiz-pps-payment-details-wrap">
            <div class="row ebiz-paymentDetails">
                <div class="large-3 columns"><asp:Literal ID="ltlPaymentDetailsLabel" runat="server" /></div>
                <div class="large-9 columns"><asp:Literal ID="ltlPaymentDetailsValue" runat="server" /></div>
            </div>
        
    
            <asp:PlaceHolder ID="plhDirectDebitMessage" runat="server" visible="false">
                <div class="alert-box warning"><asp:Literal ID="ltlDirectDebitMessage" runat="server" /></div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plhCCForm" runat="server" visible="false">
                <Talent:PaymentDetails ID="PaymentDetailsCreditCardForm" runat="server" />
                <div class="stacked-for-small button-group ebiz-cc-form-wrap">
                    <asp:HyperLink ID="hplCancelPayment" runat="server" CssClass="button ebiz-cancel-payment" />
                    <asp:Button ID="btnUpdatePayment" runat="server" CssClass="button ebiz-primary-action ebiz-update-payment" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhCCFormExternal" runat="server" visible="false">
                <Talent:PaymentCardDetails ID="uscPaymentCardDetails" runat="server" Usage="AMENDPPSCARD"/>
                <div class="stacked-for-small button-group ebiz-cc-form-external-wrap">
                    <asp:HyperLink ID="hplCancelPaymentExternal" runat="server" CssClass="button ebiz-cancel-payment-external" />
                    <asp:Button ID="btnAmendPPSCard" runat="server" CssClass="button ebiz-primary-action ebiz-amend-pps-card" CausesValidation="true" ValidationGroup="Checkout" OnClientClick="submitToVanguard(); return false;" />
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>
</asp:Content>