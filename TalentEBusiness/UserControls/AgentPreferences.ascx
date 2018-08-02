<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AgentPreferences.ascx.vb" Inherits="UserControls_AgentPreferences" ViewStateMode="Enabled" %>
<%@ Register Src="~/UserControls/AgentsList.ascx" TagName="AgentsList" TagPrefix="Talent" %>

<script>
    function ValidateBulkSalesAndPrintAlways(chkboxBulkSales, chkboxPrintAlways, allowChanges) {
        if (allowChanges) {
            if ($("#" + chkboxBulkSales).is(':checked'))
                $("#" + chkboxPrintAlways).prop('disabled', true);
            if ($("#" + chkboxPrintAlways).is(':checked'))
                $("#" + chkboxBulkSales).prop('disabled', true);
            if ($("#" + chkboxPrintAlways).is(':checked') || $("#" + chkboxBulkSales).is(':checked')) {
                $(".ebiz-bulk-vs-print").show();
            }
            else {
                $(".ebiz-bulk-vs-print").hide();
                $("#" + chkboxPrintAlways).prop('disabled', false);
                $("#" + chkboxBulkSales).prop('disabled', false);
            }
        } else {
            $(".ebiz-bulk-vs-print").hide();
            $("#" + chkboxPrintAlways).prop('disabled', true);
            $("#" + chkboxBulkSales).prop('disabled', true);
        }
    }
</script>

<asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList" CssClass="alert-box alert" ValidationGroup="AgentPreferences" />

<asp:PlaceHolder ID="plhErrorList" runat="server">
    <div class="alert-box alert">
        <asp:BulletedList ID="blErrList" runat="server" />
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhSuccessList" runat="server">
    <div class="alert-box success">
        <asp:BulletedList ID="blSuccessList" runat="server" />
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhAgentCopySuccessMessage" runat="server">
    <div class="success alert-box">
        <asp:Literal ID="ltlAgentCopySuccessMessage" runat="server" /></div>
</asp:PlaceHolder>

<div id="agentPreferenceHeader" class="panel ebiz-agent-preferences-agent" runat="server">
    <h2>
        <asp:Label ID="lblAgentPreferences" runat="server" />
    </h2>
    <Talent:AgentsList ID="uscAgentList" runat="server" />
</div>

<asp:PlaceHolder ID="plhAgentPreferences" runat="server">
    <div class="panel ebiz-agent-preferences">
        <h2>
            <asp:Literal ID="ltlAgentPreferencesInstructionsLabel" runat="server" /></h2>
        <fieldset>
            <legend>
                <asp:Literal ID="ltlAgentPreferencesLegend" runat="server" /></legend>
            <asp:PlaceHolder ID="plhDepartment" runat="server">
                <div class="row ebiz-department">
                    <div class="medium-3 columns">
                        <asp:Literal ID="ltlDepartment" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:Literal ID="ltlAgentDepartment" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="row ebiz-authority-group">
                <div class="medium-3 columns">
                    <asp:Label ID="lblAuthorityGroup" runat="server" AssociatedControlID="ddlAuthorityGroups" />
                </div>
                <div class="medium-9 columns">
                    <asp:DropDownList ID="ddlAuthorityGroups" runat="server" AutoPostBack="false" />
                    <asp:RequiredFieldValidator ID="rfvAuthorityGroup" runat="server" ValidationGroup="AgentPreferences" ControlToValidate="ddlAuthorityGroups" Display="Static" CssClass="error" />
                    <asp:CustomValidator ID="csvAuthorityGroup" Display="Static" ControlToValidate="ddlAuthorityGroups" runat="server" ValidationGroup="AgentPreferences" CssClass="error" />
                </div>
            </div>
            <div class="row ebiz-printer-group">
                <div class="medium-3 columns">
                    <asp:Label ID="lblPrinterGroup" runat="server" AssociatedControlID="ddlPrinterGroup" />
                </div>
                <div class="medium-9 columns">
                    <asp:DropDownList ID="ddlPrinterGroup" runat="server" AutoPostBack="true" />
                    <asp:RequiredFieldValidator ID="rfvPrinterGroup" runat="server" ValidationGroup="AgentPrintingPreferences" ControlToValidate="ddlPrinterGroup" Display="Static" CssClass="error" />
                    <asp:CustomValidator ID="csvPrinterGroup" Display="Static" ControlToValidate="ddlPrinterGroup" runat="server" ValidationGroup="AgentPreferences" CssClass="error" />
                </div>
            </div>
            <div class="row ebiz-ticket-printer">
                <div class="medium-3 columns">
                    <asp:Label ID="lblTicketPrinterDefault" runat="server" AssociatedControlID="ddlTicketPrinterDefault" />
                </div>
                <div class="medium-9 columns">
                    <asp:DropDownList ID="ddlTicketPrinterDefault" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvTicketPrinterDefault" runat="server" ValidationGroup="AgentPrintingPreferences" ControlToValidate="ddlTicketPrinterDefault" Display="Static" CssClass="error" />
                    <asp:CustomValidator ID="csvTicketPrinterDefault" Display="Static" ControlToValidate="ddlTicketPrinterDefault" runat="server" ValidationGroup="AgentPreferences" CssClass="error" />
                </div>
            </div>
            <asp:PlaceHolder ID="plhSmartPrinterDefault" runat="server">
                <div class="row ebiz-smartcard-printer">
                    <div class="medium-3 columns">
                        <asp:Label ID="lblSmartcardPrinterDefault" runat="server" AssociatedControlID="ddlSmartcardPrinterDefault" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:DropDownList ID="ddlSmartcardPrinterDefault" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvSmartcardPrinterDefault" runat="server" ValidationGroup="AgentPrintingPreferences" ControlToValidate="ddlSmartcardPrinterDefault" Display="Static" CssClass="error" />
                        <asp:CustomValidator ID="csvSmartcardPrinterDefault" Display="Static" ControlToValidate="ddlSmartcardPrinterDefault" runat="server" ValidationGroup="AgentPreferences" CssClass="error" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="row ebiz-printer-home">
                <div class="medium-3 columns">
                    <asp:Label ID="lblOverridePrinterHome" runat="server" AssociatedControlID="txtOverridePrinterHome" />
                </div>
                <div class="medium-9 columns">
                    <asp:TextBox ID="txtOverridePrinterHome" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvOverridePrinterHome" runat="server" ValidationGroup="AgentPreferences" ControlToValidate="txtOverridePrinterHome" Display="Static" CssClass="error" />
                </div>
            </div>
            <div class="row ebiz-printer-event">
                <div class="medium-3 columns">
                    <asp:Label ID="lblOverridePrinterEvent" runat="server" AssociatedControlID="txtOverridePrinterEvent" />
                </div>
                <div class="medium-9 columns">
                    <asp:TextBox ID="txtOverridePrinterEvent" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvOverridePrinterEvent" runat="server" ValidationGroup="AgentPreferences" ControlToValidate="txtOverridePrinterEvent" Display="Static" CssClass="error" />
                </div>
            </div>
            <div class="row ebiz-printer-travel">
                <div class="medium-3 columns">
                    <asp:Label ID="lblOverridePrinterTravel" runat="server" AssociatedControlID="txtOverridePrinterTravel" />
                </div>
                <div class="medium-9 columns">
                    <asp:TextBox ID="txtOverridePrinterTravel" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvOverridePrinterTravel" runat="server" ValidationGroup="AgentPreferences" ControlToValidate="txtOverridePrinterTravel" Display="Static" CssClass="error" />
                </div>
            </div>
            <div class="row ebiz-printer-receipts">
                <div class="medium-3 columns">
                    <asp:Label ID="lblOverridePrinterSTReceipts" runat="server" AssociatedControlID="txtOverridePrinterSTReceipts" />
                </div>
                <div class="medium-9 columns">
                    <asp:TextBox ID="txtOverridePrinterSTReceipts" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvOverridePrinterSTReceipts" runat="server" ValidationGroup="AgentPreferences" ControlToValidate="txtOverridePrinterSTReceipts" Display="Static" CssClass="error" />
                </div>
            </div>
            <div class="row ebiz-override-printer">
                <div class="medium-3 columns">
                    <asp:Label ID="lblOverridePrinterAddress" runat="server" AssociatedControlID="txtOverridePrinterAddress" />
                </div>
                <div class="medium-9 columns">
                    <asp:TextBox ID="txtOverridePrinterAddress" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvOverridePrinterAddress" runat="server" ValidationGroup="AgentPreferences" ControlToValidate="txtOverridePrinterAddress" Display="Static" CssClass="error" />
                </div>
            </div>
            <div class="row ebiz-capture-method">
                <div class="medium-3 columns">
                    <asp:Label ID="lblDefaultCaptureMethod" runat="server" AssociatedControlID="ddlCaptureMethod" />
                </div>
                <div class="medium-9 columns">
                    <asp:DropDownList ID="ddlCaptureMethod" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvDefaultCaptureMethod" runat="server" ValidationGroup="AgentPrintingPreferences" ControlToValidate="ddlCaptureMethod" Display="Static" CssClass="error" />
                </div>
            </div>
            <div class="ebiz-print-address-label">
                <asp:CheckBox ID="chkPrintAddressLabelsDefault" runat="server" />
                <asp:Label ID="lblPrintAddressLabelsDefault" runat="server" AssociatedControlID="chkPrintAddressLabelsDefault" />
            </div>
            <div class="ebiz-print-transaction-receipt">
                <asp:CheckBox ID="chkPrintTransactionReceiptDefault" runat="server" />
                <asp:Label ID="lblPrintTransactionReceiptDefault" runat="server" AssociatedControlID="chkPrintTransactionReceiptDefault" />
            </div>
            <div class="ebiz-bulk-sales-mode">
                <asp:CheckBox ID="chkBulkSalesMode" runat="server" />
                <asp:Label ID="lblBulkSalesMode" runat="server" AssociatedControlID="chkBulkSalesMode" />
            </div>
            <div class="ebiz-print-now">
                <asp:CheckBox ID="chkPrintAlways" runat="server" Checked="false" />
                <asp:Label ID="lblPrintAlways" runat="server" AssociatedControlID="chkPrintAlways" />
            </div>
            <div class="ebiz-bulk-vs-print alert-box info">
                <asp:Literal ID="ltlBulkSalesVsPrintAlways" runat="server"></asp:Literal>
            </div>
            <div class="ebiz-hospitality-mode">
                <asp:CheckBox ID="chkCorporateHospitalityMode" runat="server" Checked="false" />
                <asp:Label ID="lblCorporateHospitalityMode" runat="server" AssociatedControlID="chkCorporateHospitalityMode" />
            </div>
            <div class="ebiz-save-agent-preferences-wrap">
                <asp:Button ID="btnSave" CssClass="button" runat="server" ValidationGroup="AgentPreferences" CausesValidation="true" />
            </div>
        </fieldset>
    </div>

</asp:PlaceHolder>
