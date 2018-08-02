<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DespatchProcess.ascx.vb" Inherits="UserControls_DespatchProcess" ViewStateMode="Enabled" %>

<div class="ebiz-despatch-process">

    <div class="row">
        <div class="large-12 columns">
            <span id="dummy"></span>
            <asp:PlaceHolder ID="plhErrorMessage" runat="server">
                <div class="alert-box alert">
                    <asp:Literal ID="ltlErrorDetails" runat="server"></asp:Literal>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhSuccessMessage" runat="server">
                <div class="alert-box success">
                    <asp:Literal ID="ltlSuccessDetails" runat="server"></asp:Literal>
                </div>
            </asp:PlaceHolder>
            <asp:ValidationSummary ID="vlsDespatchSearch" runat="server" ValidationGroup="DespatchSearch" ShowSummary="true" CssClass="alert-box alert" ShowValidationErrors="true" />
            <asp:ValidationSummary ID="vlsDespatchProcess" runat="server" ValidationGroup="DespatchProcess" ShowSummary="true" CssClass="alert-box alert" ShowValidationErrors="true" />
            <asp:CustomValidator ID="csvDuplicateTickets" runat="server" ValidationGroup="DespatchProcess" ValidateEmptyText="false" Display="None" OnServerValidate="csvDuplicateTickets_ServerValidate" />
        </div>
    </div>

    <input type="hidden" id="hdfMembershipLabel" value='<%=MembershipLabel %>' />

    <asp:Panel ID="plhDespatchDetailsWrapper" runat="server" DefaultButton="btnFinishOrder">

        <asp:PlaceHolder ID="plhSearchAndProcessInfoWrapper" runat="server">
            <div class="row ebiz-search-and-process-info-wrap" data-equalizer="hSearchAndProcessInfoWrapper" data-options="equalizeOnStack:false;">
                <div class="large-6 columns">
                    <asp:Panel runat="server" ID="pnlSearchOptions" CssClass="panel ebiz-despatch-search" DefaultButton="btnSearch" data-equalizer-watch="hSearchAndProcessInfoWrapper">
                        <div class="row ebiz-despatch-note-id">
                            <div class="large-3 columns">
                                <asp:Label ID="lblDespatchNoteID" runat="server" AssociatedControlID="txtDespatchNoteID" />
                            </div>
                            <div class="large-9 columns">
                                <asp:TextBox ID="txtDespatchNoteID" runat="server" ClientIDMode="Static" MaxLength="31" TabIndex="1" AccessKey="n" />
                                <asp:RegularExpressionValidator ID="rgxDespatchNoteID" runat="server" ControlToValidate="txtDespatchNoteID" ValidationExpression="^[P|R|p|r][-][0-9]{1,13}[-][0-9 ]{1,15}$" ValidationGroup="DespatchSearch"
                                    Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                                <asp:CustomValidator ID="csvSearchOptions" runat="server" ValidateEmptyText="true" ControlToValidate="txtDespatchNoteID" ClientValidationFunction="validateSearchOptions" ValidationGroup="DespatchSearch"
                                    Display="None" SetFocusOnError="true" OnServerValidate="csvSearchOptions_ServerValidate" />
                                <asp:HiddenField ID="hdfBatchID" runat="server" />
                                <asp:HiddenField ID="hdfTimeStampToken" runat="server" />
                                <asp:HiddenField ID="hdfPaymentReservationType" runat="server" />
                            </div>
                        </div>
                        <div class="row ebiz-payment-reference">
                            <div class="large-3 columns">
                                <asp:Label ID="lblPaymentReference" runat="server" AssociatedControlID="txtPaymentReference" />
                            </div>
                            <div class="large-9 columns">
                                <asp:TextBox ID="txtPaymentReference" runat="server" ClientIDMode="Static" MaxLength="15" TabIndex="2" AccessKey="p" />
                                <asp:RegularExpressionValidator ID="rgxPaymentReference" runat="server" ControlToValidate="txtPaymentReference" ValidationExpression="[0-9 ]{1,15}" ValidationGroup="DespatchSearch"
                                    Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                            </div>
                        </div>
                        <div class="row ebiz-ticket-number-search">
                            <div class="large-3 columns">
                                <asp:Label ID="lblTicketNumber" runat="server" AssociatedControlID="txtTicketNumber" />
                            </div>
                            <div class="large-9 columns">
                                <asp:TextBox ID="txtTicketNumber" runat="server" ClientIDMode="Static" MaxLength="24" TabIndex="3" AccessKey="t" />
                            </div>
                        </div>
                        <div class="ebiz-despatch-search-button-wrap">
                            <asp:Button ID="btnSearch" runat="server" CssClass="button ebiz-primary-action" CausesValidation="true" ValidationGroup="DespatchSearch" TabIndex="4" AccessKey="s"></asp:Button>
                        </div>
                    </asp:Panel>
                </div>

                <div class="large-6 columns">
                    <asp:PlaceHolder ID="plhDespatchProcessInfo" runat="server">
                        <div class="alert-box info ebiz-despatch-process-information" data-equalizer-watch="hSearchAndProcessInfoWrapper">
                            <div class="row">
                                <div class="large-3 columns">
                                    <asp:Literal ID="ltlPostageLabel" runat="server" />
                                </div>
                                <div class="large-9 columns">
                                    <asp:Literal ID="ltlPostageValue" runat="server" />
                                </div>
                            </div>
                            <asp:PlaceHolder ID="plhCharityFeeAdded" runat="server">
                                <div class="row">
                                    <div class="large-9 large-push-3 columns">
                                        <asp:Literal ID="ltlCharityFeeAdded" runat="server" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhGiftWrappingAdded" runat="server">
                                <div class="row">
                                    <div class="large-9 large-push-3 columns">
                                        <asp:Literal ID="ltlGiftWrappingAdded" runat="server" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <div class="row">
                                <div class="large-3 columns">
                                    <asp:HyperLink ID="hplPaymentReference" runat="server" />
                                    <asp:Literal ID="ltlReservationReference" runat="server" />
                                </div>
                                <div class="large-9 columns">
                                    <asp:Literal ID="ltlDespatchStatus" runat="server" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-3 columns">
                                    <asp:Literal ID="ltlCustomerLabel" runat="server" />
                                    <asp:HiddenField ID="hdfCustomerNumber" runat="server" />
                                </div>
                                <div class="large-9 columns">
                                    <asp:HyperLink ID="hplCustomerDetails" runat="server" />
                                    <asp:Literal ID="ltlGenericCustomer" runat="server" />
                                </div>
                            </div>
                            <div class="row">
                                <div class="large-3 columns">
                                    <asp:Literal ID="ltlCATpayrefsLabel" runat="server" />
                                </div>
                                <div class="large-9 columns">
                                    <asp:Literal ID="ltlCATpayrefsValue" runat="server" />
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:Repeater ID="rptItemstoDespatch" runat="server">
            <HeaderTemplate>
                <div class="panel ebiz-items-to-despatch-wrap">
                    <table class="ebiz-responsive-table">
                        <thead>
                            <tr>
                                <asp:PlaceHolder ID="plhShowPrintHeader" runat="server">
                                    <th scope="col" class="ebiz-th-checkbox-wrap ebiz-print">
                                        <asp:CheckBox ID="chkPrintAll" runat="server" ClientIDMode="Static" OnClick="selectPrintAll(this.checked);" /></th>
                                </asp:PlaceHolder>
                                <th scope="col" class="ebiz-event-name"><%=EventHeaderText%></th>
                                <th scope="col" class="ebiz-ticket-type"><%=TicketTypeHeaderText%></th>
                                <asp:PlaceHolder ID="plhPrefixHeader" runat="server">
                                    <th scope="col" class="ebiz-prefix"><%=PrefixHeaderText%></th>
                                </asp:PlaceHolder>

                                <th scope="col" class="ebiz-seat"><%=SeatHeaderText%></th>
                                <asp:PlaceHolder ID="plhShowTicketNumberHeader" runat="server">
                                    <th scope="col" class="ebiz-ticket-number"><%=TicketNoHeaderText%></th>
                                </asp:PlaceHolder>
                                <th scope="col" class="ebiz-status"><%=StatusHeaderText%></th>

                            </tr>
                        </thead>
                        <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <asp:PlaceHolder ID="plhShowPrint" runat="server">
                        <td class="ebiz-print" data-title="<%=PrintAllHeaderText%>">
                            <asp:CheckBox ID="chkPrint" runat="server" CssClass="ebiz-print-checkbox" />
                        </td>
                    </asp:PlaceHolder>
                    <td class="ebiz-event-name" data-title="<%=EventHeaderText%>">
                        <%# DataBinder.Eval(Container.DataItem, "ProductDescription").ToString().Trim()%>&nbsp;<%# DataBinder.Eval(Container.DataItem, "PriceCodeDescription").ToString().Trim()%>
                        <asp:HiddenField ID="hdfProductCode" runat="server" />
                        <asp:PlaceHolder ID="plhGiftMessage" runat="server">
                            <a class="button fa-input ebiz-show-gift-message" data-open="<%#GetGiftMessageRevealID(Container.ItemIndex)%>"><%=GiftMessageIcon%></a>
                            <div id="<%#GetGiftMessageRevealID(Container.ItemIndex)%>" class="reveal" data-reveal>
                                <%#DataBinder.Eval(Container.DataItem, "GiftMessage").ToString().Trim()%>
                                <button class="close-button" data-close aria-label="Close modal" type="button">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                        </asp:PlaceHolder>
                    </td>
                    <td class="ebiz-ticket-type" data-title="<%=TicketTypeHeaderText%>"><%# DataBinder.Eval(Container.DataItem, "PriceBandDescription").ToString().Trim()%></td>
                    <asp:PlaceHolder ID="plhPrefix" runat="server">
                        <td class="ebiz-prefix" data-title="<%=PrefixHeaderText%>">
                            <asp:Label ID="lblBarcodePrefix" runat="server" Text='<%# GetBarcodePrefix(DataBinder.Eval(Container.DataItem, "BarcodePrefix").ToString().Trim())%>' /></td>
                    </asp:PlaceHolder>

                    <td class="ebiz-seat" data-title="<%=SeatHeaderText%>">
                        <asp:Label ID="lblSeat" runat="server" CssClass="ebiz-seat-label" />
                        <asp:HiddenField ID="hdfStand" runat="server" />
                        <asp:HiddenField ID="hdfStandDesc" runat="server" />
                        <asp:HiddenField ID="hdfArea" runat="server" />
                        <asp:HiddenField ID="hdfAreaDesc" runat="server" />
                        <asp:HiddenField ID="hdfRowNumber" runat="server" />
                        <asp:HiddenField ID="hdfSeat" runat="server" />
                        <asp:HiddenField ID="hdfAlphaSuffix" runat="server" />
                    </td>
                    <asp:PlaceHolder ID="plhShowTicketNumber" runat="server">
                        <td class="ebiz-ticket-number" data-title="<%=TicketNoHeaderText%>">
                            <asp:TextBox ID="txtTicketNumber" runat="server" CssClass="ebiz-ticket-number-field" />

                            <asp:PlaceHolder ID="plhScanSeries" runat="server">
                                <asp:HyperLink ID="hplScanSeries" runat="server" CssClass="fa-input ebiz-fa-input" title="Scan Series"><%=ScanSeriesIcon %></asp:HyperLink>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhPrint" runat="server" Visible="false">
                                <asp:HyperLink ID="hplPrintLabelRenew" runat="server" CssClass="ebiz-print-label-renew" title="Print Label Renew" Style="display: none"><i class="fa fa-refresh"></i></asp:HyperLink>
                                <asp:HyperLink ID="hplPrintLabelRequest" runat="server" CssClass="ebiz-print-label-request" title="Print Label Request" Style="display: none"><i class="fa fa-print"></i></asp:HyperLink>
                                <asp:HyperLink ID="hplPrintLabelConfirm" runat="server" CssClass="ebiz-print-label-confirm" title="Print Confirm" Style="display: none"><i class="fa fa-question-circle"></i></asp:HyperLink>
                                <asp:HyperLink ID="hplCompleteLabelIcon" runat="server" CssClass="button fa-input print-button" title="Print Complete" Style="display: none"><%=CompleteLabelIcon%></asp:HyperLink>
                                <asp:HyperLink ID="hplRestrictedPrintLabelIcon" runat="server" CssClass="button fa-input print-button"><%=RestrictedLabelIcon%></asp:HyperLink>
                                <asp:Literal ID="ltlLoadingImg" runat="server" />
                                <asp:Literal ID="ltlPrintCardError" runat="server" />
                            </asp:PlaceHolder>
                            <asp:CustomValidator ID="csvTicketNumber" runat="server" ControlToValidate="txtTicketNumber" ValidateEmptyText="false" ValidationGroup="DespatchProcess"
                                Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" ClientValidationFunction="validateTicketNumber" />
                            <div class="ebiz-membership-rfid">
                                <asp:HiddenField ID="hdfMembershipRFID" runat="server" />
                            </div>
                            <asp:HiddenField ID="hdfMembershipMagScan" runat="server" />
                            <asp:HiddenField ID="hdfMembershipMetalBadge" runat="server" />
                            <asp:HiddenField ID="hdfSeatValidationCode" runat="server" Value='<%# GetSeatValidationCode(DataBinder.Eval(Container.DataItem, "BarcodePrefix").ToString()) %>' />
                        </td>
                    </asp:PlaceHolder>
                    <td class="ebiz-status" data-title="<%=StatusHeaderText%>">
                        <%# GetStatusText(DataBinder.Eval(Container.DataItem, "SeatStatus").ToString(), DataBinder.Eval(Container.DataItem, "Barcode").ToString(), DataBinder.Eval(Container.DataItem, "RFID").ToString(), DataBinder.Eval(Container.DataItem, "PrintedCount").ToString(), DataBinder.Eval(Container.DataItem, "IsMembershipRenewal").ToString()  , DataBinder.Eval(Container.DataItem, "NoPrintFlag").ToString())%>
                        <asp:HiddenField ID="hdfSeatStatus" runat="server" Value='<%# GetStatusText( DataBinder.Eval(Container.DataItem, "SeatStatus").ToString(), DataBinder.Eval(Container.DataItem, "Barcode").ToString(), DataBinder.Eval(Container.DataItem, "RFID").ToString(), DataBinder.Eval(Container.DataItem, "PrintedCount").ToString(), DataBinder.Eval(Container.DataItem, "IsMembershipRenewal").ToString() , DataBinder.Eval(Container.DataItem, "NoPrintFlag").ToString())%>' />
                    </td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </tbody>
                    </table>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <div id="scan-series" class="reveal" data-reveal>
            <script>
                $('#scan-series').bind('opened', function () {
                    document.getElementById('txtFirst').focus();
                });
            </script>
            <div class="row ebiz-scan-series-first">
                <div class="large-3 columns">
                    <asp:Label ID="lblFirst" runat="server" CssClass="middle" AssociatedControlID="txtFirst" />
                </div>
                <div class="large-9 columns">
                    <asp:TextBox ID="txtFirst" runat="server" ClientIDMode="Static" TabIndex="30001" />
                    <asp:CustomValidator ID="csvFirstTicketNumber" ClientIDMode="Static" runat="server" ControlToValidate="txtFirst" ValidateEmptyText="false" ValidationGroup="DespatchProcess"
                        Display="Dynamic" CssClass="error ebiz-validator-error" SetFocusOnError="true" ClientValidationFunction="validateScanSeriesTicketNumber" />
                </div>
            </div>
            <div class="row ebiz-scan-series-last">
                <div class="large-3 columns">
                    <asp:Label ID="lblLast" runat="server" CssClass="middle" AssociatedControlID="txtLast" />
                </div>
                <div class="large-9 columns">
                    <asp:TextBox ID="txtLast" runat="server" ClientIDMode="Static" TabIndex="30002" />
                    <asp:CustomValidator ID="csvLastTicketNumber" ClientIDMode="Static" runat="server" ControlToValidate="txtLast" ValidateEmptyText="false" ValidationGroup="DespatchProcess"
                        Display="Dynamic" CssClass="error ebiz-validator-error" SetFocusOnError="true" ClientValidationFunction="validateScanSeriesTicketNumber" />
                    <span id="ticketLimitError" class="error ebiz-validator-error" style="display: none;"><%=TicketLimitError %></span>
                </div>
            </div>
            <button class="close-button" data-close aria-label="Close modal" type="button">
                <span aria-hidden="true"><i class="fa fa-times"></i></span>
            </button>
        </div>

        <asp:PlaceHolder ID="plhDespatchOptions" runat="server">
            <div class="panel ebiz-ebiz-despatch-options-actions-wrap">
                <div class="row ebiz-despatch-options-wrap">
                    <div class="large-6 columns ebiz-despatch-options-blurb-wrap">
                        <div class="ebiz-despatch-membership-scan" style="display: none;" id="membership-options">
                            <div class="row ebiz-rfid">
                                <div class="large-3 columns">
                                    <asp:Label ID="lblMembershipRFID" runat="server" AssociatedControlID="txtMembershipRFID" />
                                </div>
                                <div class="large-9 columns">
                                    <asp:TextBox ID="txtMembershipRFID" runat="server" ClientIDMode="Static" />
                                    <asp:RegularExpressionValidator ID="rgxMembershipRFID" runat="server" ControlToValidate="txtMembershipRFID" ValidationGroup="DespatchProcess"
                                        Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                                </div>
                            </div>
                            <asp:PlaceHolder ID="plhMagScan" runat="server">
                                <div class="row ebiz-scan-magscan">
                                    <div class="large-3 columns">
                                        <asp:Label ID="lblMembershipMagScan" runat="server" AssociatedControlID="txtMembershipMagScan" />
                                    </div>
                                    <div class="large-9 columns">
                                        <asp:TextBox ID="txtMembershipMagScan" runat="server" ClientIDMode="Static" />
                                        <asp:RegularExpressionValidator ID="rgxMembershipMagScan" runat="server" ControlToValidate="txtMembershipMagScan" ValidationGroup="DespatchProcess"
                                            Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <div class="row ebiz-metal-badge-number">
                                <div class="large-3 columns">
                                    <asp:Label ID="lblMetalBadgeNumber" runat="server" AssociatedControlID="txtMetalBadgeNumber" />
                                </div>
                                <div class="large-9 columns">
                                    <asp:TextBox ID="txtMetalBadgeNumber" runat="server" ClientIDMode="Static" />
                                    <asp:RegularExpressionValidator ID="rgxMetalBadgeNumber" runat="server" ControlToValidate="txtMetalBadgeNumber" ValidationGroup="DespatchProcess"
                                        Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="large-6 columns ebiz-despatch-options-actions-wrap">
                        <div class="button-group">
                            <asp:PlaceHolder ID="plhDespatchOptionsButtons" runat="server">
                                <input type="button" id="btnNotForDespatch" runat="server" class="button ebiz-not-for-despatch" />
                                <input type="button" id="btnSent" runat="server" class="button ebiz-sent" />
                                <input type="button" id="btnSkipBack" runat="server" class="button ebiz-clear" onclick="skipBack();" />
                                <input type="button" id="btnSkipNext" runat="server" class="button ebiz-clear" onclick="skipNext();" />
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row ebiz-courier-options-despatch-buttons-wrap" data-equalizer="courierDespatchButtons" data-options="equalizeOnStack:false;">
                <div class="large-6 columns ebiz-courier-options-wrap">
                    <asp:PlaceHolder ID="plhCourierOptions" runat="server">
                        <div class="panel" data-equalizer-watch="courierDespatchButtons">
                            <div class="ebiz-courier-service-wrap">
                                <asp:DropDownList ID="ddlCourierService" runat="server">
                                    <asp:ListItem Text="232-EXPAK5 Next Day" />
                                    <asp:ListItem Text="242-EXPAK1 Next Day" />
                                </asp:DropDownList>
                            </div>
                            <div class="ebiz-courier-types-wrap">
                                <asp:RadioButtonList ID="rdoCourierType" runat="server" RepeatLayout="UnorderedList" CssClass="ebiz-courier-types">
                                    <asp:ListItem Text="Hospitality" />
                                    <asp:ListItem Text="Exhibition" />
                                    <asp:ListItem Text="Driver" />
                                    <asp:ListItem Text="Ticket Office" />
                                    <asp:ListItem Text="Sponsor" />
                                </asp:RadioButtonList>
                            </div>
                            <div class="row">
                                <div class="large-6 columns ebiz-courier-csv-file-wrap">
                                    <asp:HyperLink ID="hplCourierCSVFile" runat="server" Visible="false" download />
                                </div>
                                <div class="large-6 columns ebiz-courier-wrap">
                                    <asp:Button ID="btnCourier" runat="server" CssClass="button" AccessKey="c"></asp:Button>
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                </div>
                <div class="large-6 columns">
                    <asp:PlaceHolder ID="plhNotes" runat="server" Visible="false">
                        <div class="panel ebiz-despatch-process-notes" data-equalizer-watch="courierDespatchButtons">
                            <asp:Label ID="lblNotes" runat="server" AssociatedControlID="txtNotes" />
                            <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" ClientIDMode="Static" />
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
            <asp:PlaceHolder ID="plhPrintButton" runat="server">
                <div class="ebiz-despatch-print-buttons-wrap">
                    <asp:Button ID="btnFinishOrder" ClientIDMode="Static" runat="server" CssClass="button ebiz-primary-action ebiz-finish-order" CausesValidation="true" ValidationGroup="DespatchProcess" AccessKey="f"></asp:Button>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhProcessButtons" runat="server">
                <div class="ebiz-despatch-process-buttons-wrap">
                    <div class="stacked-for-small button-group">
                        <asp:Button ID="btnBack" runat="server" CssClass="button ebiz-back"></asp:Button>
                        <asp:Button ID="btnDespatchNoteGeneration" runat="server" CssClass="button ebiz-despatch-note-generation"></asp:Button>
                        <asp:Button ID="btnSkipOrder" runat="server" CssClass="button ebiz-skip-order" AccessKey="x"></asp:Button>
                        <asp:Button ID="btnClear" runat="server" CssClass="button ebiz-clear" CausesValidation="false" AccessKey="r" />
                        <%--<asp:Button ID="btnFinishOrder" ClientIDMode="Static" runat="server" CssClass="button ebiz-finish-order" CausesValidation="true" ValidationGroup="DespatchProcess" AccessKey="f"></asp:Button>--%>
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>

    </asp:Panel>

</div>
