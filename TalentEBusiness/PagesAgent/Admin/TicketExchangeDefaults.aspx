<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TicketExchangeDefaults.aspx.vb" Inherits="PagesAgent_Admin_TicketExchangeDefaults" %>
    <asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
        <asp:PlaceHolder ID="plhErrorList" runat="server" visible="False">
            <div class="alert-box alert">
                <asp:BulletedList ID="blErrorList" runat="server" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhSuccessList" runat="server">
            <div class="alert-box success">
                <asp:BulletedList ID="blSuccessList" runat="server" />
            </div>
        </asp:PlaceHolder>
        <div class="panel ebiz-ticket-exchange-defaults-wrap">
            <h2>
         <asp:Literal ID="ltlTicketingExchangeDefaultHeader" runat="server" />
      </h2>
        </div>
        <div class="panel ebiz-ticket-exchange-defaults-wrap">
            <h2><asp:Literal ID="ltlTicketingExchangeDefaultHeader2" runat="server" /></h2>
            <div class="ebiz-product-code">
                <asp:Label ID="ProductCodeLabel" runat="server" AssociatedControlID="ddlProductCode" />
                <asp:DropDownList ID="ddlProductCode" runat="server" viewStateMode="Enabled" AutoPostback="True" />
                <asp:HiddenField ID="hdfProductCode" runat="server" />
            </div>
            <div class="ebiz-ticket-exchange-summary-wrap">
                <h2><asp:Label ID="ltlTicketingExchangeSummaryHeader" runat="server"/></h2>
                <div class="row small-up-1 medium-up-1 large-up-3">
                    <div class="column ebiz-ticket-exchange-summary ebiz-ticket-exchange-summary-line1">
                        <div>
                            <h3><asp:Label ID="ltlSummarySoldHeader" runat="server"/></h3>
                            <div class="row ebiz-total-expired">
                                <div class="columns small-6">
                                    <asp:Label ID="lblSumOfTicketsSoldOnTicketExchange" runat="server" AssociatedControlID="SumOfTicketsSoldOnTicketExchange" />
                                </div>
                                <div class="columns small-6"><strong><asp:Label ID="SumOfTicketsSoldOnTicketExchange" runat="server"/></strong></div>
                            </div>
                            <div class="row ebiz-value-sold">
                                <div class="columns small-6">
                                    <asp:Label ID="lblValueOfTicketsSoldOnTicketExchange" runat="server" AssociatedControlID="ValueOfTicketsSoldOnTicketExchange" />
                                </div>
                                <div class="columns small-6"><strong><asp:Label ID="ValueOfTicketsSoldOnTicketExchange" runat="server"/></strong></div>
                            </div>
                            <div class="row ebiz-value-fees-sold">
                                <div class="columns small-6">
                                    <asp:Label ID="lblValueOfTEFeesSoldOnTicketExchange" runat="server" AssociatedControlID="ValueOfTEFeesSoldOnTicketExchange" />
                                </div>
                                <div class="columns small-6"><strong><asp:Label ID="ValueOfTEFeesSoldOnTicketExchange" runat="server"/></strong></div>
                            </div>
                        </div>
                    </div>
                    <div class="column ebiz-ticket-exchange-summary ebiz-ticket-exchange-summary-line2">
                        <div>
                            <h3><asp:Label ID="ltlSummaryPendingHeader" runat="server"/></h3>
                            <div class="row ebiz-total-pending">
                                <div class="columns small-6">
                                    <asp:Label ID="lblSumOfTicketsPendingOnTicketExchange" runat="server" AssociatedControlID="SumOfTicketsPendingOnTicketExchange" />
                                </div>
                                <div class="columns small-6"><strong><asp:Label ID="SumOfTicketsPendingOnTicketExchange" runat="server"/></strong></div>
                            </div>
                            <div class="row ebiz-value-pending">
                                <div class="columns small-6">
                                    <asp:Label ID="lblValueOfTicketsPendingOnTicketExchange" runat="server" AssociatedControlID="ValueOfTicketsPendingOnTicketExchange" />
                                </div>
                                <div class="columns small-6"><strong><asp:Label ID="ValueOfTicketsPendingOnTicketExchange" runat="server"/></strong></div>
                            </div>
                            <div class="row ebiz-value-fees-expired">
                                <div class="columns small-6">
                                    <asp:Label ID="lblValueOfTEFeesExpiredOnTicketExchange" runat="server" AssociatedControlID="ValueOfTEFeesExpiredOnTicketExchange" />
                                </div>
                                <div class="columns small-6"><strong><asp:Label ID="ValueOfTEFeesExpiredOnTicketExchange" runat="server"/></strong></div>
                            </div>
                        </div>
                    </div>
                    <div class="column ebiz-ticket-exchange-summary ebiz-ticket-exchange-summary-line3">
                        <div>
                            <h3><asp:Label ID="ltlSummaryExpiredHeader" runat="server"/></h3>
                            <div class="row ebiz-total-expired">
                                <div class="columns small-6">
                                    <asp:Label ID="lblSumOfTicketsExpiredOnTicketExchange" runat="server" AssociatedControlID="SumOfTicketsExpiredOnTicketExchange" />
                                </div>
                                <div class="columns small-6"><strong><asp:Label ID="SumOfTicketsExpiredOnTicketExchange" runat="server"/></strong></div>
                            </div>
                            <div class="row ebiz-value-expired">
                                <div class="columns small-6">
                                    <asp:Label ID="lblValueOfTicketsExpiredOnTicketExchange" runat="server" AssociatedControlID="ValueOfTicketsExpiredOnTicketExchange" />
                                </div>
                                <div class="columns small-6"><strong><asp:Label ID="ValueOfTicketsExpiredOnTicketExchange" runat="server"/></strong></div>
                            </div>
                            <div class="row ebiz-value-fees-pending">
                                <div class="columns small-6">
                                    <asp:Label ID="lblValueOfTEFeesPendingOnTicketExchange" runat="server" AssociatedControlID="ValueOfTEFeesPendingOnTicketExchange" />
                                </div>
                                <div class="columns small-6"><strong><asp:Label ID="ValueOfTEFeesPendingOnTicketExchange" runat="server"/></strong></div>
                            </div>
                        </div>
                    </div>
                </div>
                <asp:PlaceHolder ID="plhProductLevelFields" runat="server" visible="False">
                    <div class="ebiz-product-level-fields">
                        <h2><asp:Label ID="lblDefaults" runat="server" /></h2>
                        <div class="row small-up-1 medium-up-3 large-up-3">
                           <!-- <div class="column">
                                <div class="row ebiz-total-allocated-sold">
                                    <div class="large-8 columns">
                                        <asp:Label ID="lblSumOfTicketsAllocatedSold" runat="server" AssociatedControlID="SumOfTicketsAllocatedSold" />
                                    </div>
                                    <div class="large-4 columns">
                                        <strong><asp:Label ID="SumOfTicketsAllocatedSold" runat="server" /></strong>
                                    </div>
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-total-booked">
                                    <div class="large-8 columns">
                                        <asp:Label ID="lblSumOfTicketsBooked" runat="server" AssociatedControlID="SumOfTicketsBooked" />
                                    </div>
                                    <div class="large-4 columns">
                                        <strong><asp:Label ID="SumOfTicketsBooked" runat="server" /></strong>
                                    </div>
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>-->
                            <div class="column">
                                <div class="row ebiz-allow-place-onsale">
                                    <div class="large-8 columns">
                                        <asp:Label ID="lblAllowPlaceOnSale" runat="server" AssociatedControlID="cbAllowPlaceOnSale" />
                                    </div>
                                    <div class="large-4 columns">
                                        <asp:Checkbox ID="cbAllowPlaceOnSale" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-allow-purchase">
                                    <div class="large-8 columns">
                                        <asp:Label ID="lblAllowPurchase" runat="server" AssociatedControlID="cbAllowPurchase" />
                                    </div>
                                    <div class="large-4 columns">
                                        <asp:Checkbox ID="cbAllowPurchase" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-product-min-price">
                                    <div class="large-8 columns">
                                        <asp:Label ID="lblProductMinPrice" runat="server" AssociatedControlID="txtProductMinPrice" />
                                    </div>
                                    <div class="large-4 columns">
                                        <asp:textbox ID="txtProductMinPrice" runat="server" ClientIDMode="Static" />
                                        <asp:CustomValidator ClientIDMode="Static" ID="cvValidateProductMinPrice" runat="server" EnableClientScript="true" ValidationGroup="TicketExchangeDefaults" ClientValidationFunction="validateProductMinMaxPrice" ControlToValidate="txtProductMinPrice" Display="Dynamic"></asp:CustomValidator>
                                        <asp:HiddenField ClientIDMode="Static" ID="hdfValidateProductMinPriceMsg" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-product-max-price">
                                    <div class="large-8 columns">
                                        <asp:Label ID="lblProductmaxPrice" runat="server" AssociatedControlID="txtProductmaxPrice" />
                                    </div>
                                    <div class="large-4 columns">
                                        <asp:textbox ID="txtProductmaxPrice" runat="server" ClientIDMode="Static" />
                                        <asp:CustomValidator ClientIDMode="Static" ID="cvValidateProductMaxPrice" runat="server" EnableClientScript="true" ValidationGroup="TicketExchangeDefaults" ClientValidationFunction="validateProductMinMaxPrice" ControlToValidate="txtProductMaxPrice" Display="Dynamic"></asp:CustomValidator>
                                        <asp:HiddenField ClientIDMode="Static" ID="hdfValidateProductMaxPriceMsg" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-Min-Max-Boundary-type">
                                    <div class="large-8 columns">
                                        <asp:Label ID="lblMinMaxBoundaryType" runat="server" AssociatedControlID="ddlMinMaxBoundaryType" />
                                    </div>
                                    <div class="large-4 columns">
                                        <asp:DropDownList ID="ddlMinMaxBoundaryType" runat="server" viewStateMode="Enabled" />
                                    </div>
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-club-fee-price">
                                    <div class="large-8 columns">
                                        <asp:Label ID="lblClubFee" runat="server" AssociatedControlID="txtClubFee" />
                                    </div>
                                    <div class="large-4 columns">
                                        <asp:textbox ID="txtClubFee" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-club-fee-type">
                                    <div class="large-8 columns">
                                        <asp:Label ID="lblClubFeeType" runat="server" AssociatedControlID="ddlClubFeeType" />
                                    </div>
                                    <div class="large-4 columns">
                                        <asp:DropDownList ID="ddlClubFeeType" runat="server" viewStateMode="Enabled" />
                                    </div>
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-allow-sales">
                                    <div class="large-8 columns">
                                        <asp:Label ID="lblCustomerRetainsPrerequisite" runat="server" AssociatedControlID="cbCustomerRetainsPrerequisite" />
                                    </div>
                                    <div class="large-4 columns">
                                        <asp:Checkbox ID="cbCustomerRetainsPrerequisite" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-allow-sales">
                                    <div class="large-8 columns">
                                        <asp:Label ID="lblCustomerRetainsMaximumLimit" runat="server" AssociatedControlID="cbCustomerRetainsMaximumLimit" />
                                    </div>
                                    <div class="large-4 columns">
                                        <asp:Checkbox ID="cbCustomerRetainsMaximumLimit" runat="server" />
                                    </div>
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                            <div class="column">
                                <div class="row ebiz-empty-row">
                                </div>
                            </div>
                        </div>
                        <div class="ebiz-button-wrap">
                            <asp:Button ID="btnUpdate" runat="server" CssClass="button" ValidationGroup="TicketExchangeDefaults" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
            <asp:Repeater ID="rptTicketExchangeDefaults" runat="server">
                <HeaderTemplate>
                    <table id="tblTicketExchangeDefaults" class="no-footer ebiz-responsive-table ebiz-ticket-exchange-defaults" role="grid">
                        <thead>
                            <tr>
                                <th scope="col" class="ebiz-stand-area">
                                    <%=StandAreaMaskHeader%>
                                </th>
                                <th scope="col" class="ebiz-tickets-allocated">
                                    <%=SumOfTicketsAllocatedSoldHeader%>
                                </th>
                                <th scope="col" class="ebiz-tickets-booked">
                                    <%=SumOfTicketsBookedHeader%>
                                </th>
                                <th scope="col" class="ebiz-tickets-pending">
                                    <%=SumOfTicketsPendingOnTicketExchangeHeader%>
                                </th>
                                <th scope="col" class="ebiz-tickets-expired">
                                    <%=SumOfTicketsExpiredOnTicketExchangeHeader%>
                                </th>
                                <th scope="col" class="ebiz-tickets-sold">
                                    <%=SumOfTicketsSoldOnTicketExchangeHeader%>
                                </th>
                                <th scope="col" class="ebiz-value-pending">
                                    <%=ValueOfTicketsPendingOnTicketExchangeHeader%>
                                </th>
                                <th scope="col" class="ebiz-value-expired">
                                    <%=ValueOfTicketsExpiredOnTicketExchangeHeader%>
                                </th>
                                <th scope="col" class="ebiz-value-sold">
                                    <%=ValueOfTicketsSoldOnTicketExchangeHeader%>
                                </th>
                                <th scope="col" class="ebiz-value-fees-pending">
                                    <%=ValueOfTEFeesPendingOnTicketExchangeHeader%>
                                </th>
                                <th scope="col" class="ebiz-value-fees-expired">
                                    <%=ValueOfTEFeesExpiredOnTicketExchangeHeader%>
                                </th>
                                <th scope="col" class="ebiz-value-fees-sold">
                                    <%=ValueOfTEFeesSoldOnTicketExchangeHeader%>
                                </th>
                                <th scope="col" class="ebiz-allow-place-onsale">
                                    <%=AllowPlaceOnTEHeader%> <br />
                                        <asp:Checkbox ID="chkAllAllowTEReturns" runat="server" OnClick="selectAll(this.checked, '.ebiz-select-te-returns');" />
                                </th>
                                <th scope="col" class="ebiz-allow-purchase">
                                    <%=AllowPurchaseTEHeader%> <br />
                                        <asp:Checkbox ID="chkAllAllowTESales" runat="server" OnClick="selectAll(this.checked, '.ebiz-select-te-sales');" />
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Label ID="lblStandAreaMask" runat="server" /> </td>
                        <td class="ebiz-product-pending" data-title="<%=SumOfTicketsAllocatedSoldHeader%>">
                            <%# Eval("SumOfTicketsAllocatedSold")%>
                        </td>
                        <td class="ebiz-product-pending" data-title="<%=SumOfTicketsBookedHeader%>">
                            <%# Eval("SumOfTicketsBooked")%>
                        </td>
                        <td class="ebiz-product-pending" data-title="<%=SumOfTicketsPendingOnTicketExchangeHeader%>">
                            <%# Eval("SumOfTicketsPendingOnTicketExchange")%>
                        </td>
                        <td class="ebiz-product-pending" data-title="<%=SumOfTicketsExpiredOnTicketExchangeHeader%>">
                            <%# Eval("SumOfTicketsExpiredOnTicketExchange")%>
                        </td>
                        <td class="ebiz-product-pending" data-title="<%=SumOfTicketsSoldOnTicketExchangeHeader%>">
                            <%# Eval("SumOfTicketsSoldOnTicketExchange")%>
                        </td>
                        <td class="ebiz-product-pending" data-title="<%=ValueOfTicketsPendingOnTicketExchangeHeader%>">
                            <%# Eval("ValueOfTicketsPendingOnTicketExchange")%>
                        </td>
                        <td class="ebiz-product-pending" data-title="<%=ValueOfTicketsExpiredOnTicketExchangeHeader%>">
                            <%# Eval("ValueOfTicketsExpiredOnTicketExchange")%>
                        </td>
                        <td class="ebiz-product-pending" data-title="<%=ValueOfTicketsSoldOnTicketExchangeHeader%>">
                            <%# Eval("ValueOfTicketsSoldOnTicketExchange")%>
                        </td>
                        <td class="ebiz-product-pending" data-title="<%=ValueOfTEFeesPendingOnTicketExchangeHeader%>">
                            <%# Eval("ValueOfTEFeesPendingOnTicketExchange")%>
                        </td>
                        <td class="ebiz-product-pending" data-title="<%=ValueOfTEFeesExpiredOnTicketExchangeHeader%>">
                            <%# Eval("ValueOfTEFeesExpiredOnTicketExchange")%>
                        </td>
                        <td class="ebiz-product-pending" data-title="<%=ValueOfTEFeesSoldOnTicketExchangeHeader%>">
                            <%# Eval("ValueOfTEFeesSoldOnTicketExchange")%>
                        </td>
                        <td>
                            <asp:Checkbox ID="cbAllowTEReturns" runat="server" CssClass="ebiz-select-te-returns" onclick="validateSelAllChkBox(this.checked, '#SiteMasterBody_ContentPlaceHolder1_rptTicketExchangeDefaults_chkAllAllowTEReturns', '.ebiz-select-te-returns', '');" /> 
                        </td>
                        <td>
                            <asp:Checkbox ID="cbAllowTESales" runat="server" CssClass="ebiz-select-te-sales" onclick="validateSelAllChkBox(this.checked, '#SiteMasterBody_ContentPlaceHolder1_rptTicketExchangeDefaults_chkAllAllowTESales', '.ebiz-select-te-sales', '');" /> </td>
                        <asp:HiddenField ID="hdfStand" runat="server" />
                        <asp:HiddenField ID="hdfArea" runat="server" />
                        <asp:HiddenField ID="hdfAllowTESales" runat="server" />
                        <asp:HiddenField ID="hdfAllowTEReturns" runat="server" />
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <script type="text/javascript">
            $(document).ready(function () {
                // On load validate & set SelectAll checkbox
                validateSelAllChkBox('true', '#SiteMasterBody_ContentPlaceHolder1_rptTicketExchangeDefaults_chkAllAllowTEReturns', '.ebiz-select-te-returns', '');
                validateSelAllChkBox('true', '#SiteMasterBody_ContentPlaceHolder1_rptTicketExchangeDefaults_chkAllAllowTESales', '.ebiz-select-te-sales', '');
            });
        </script>
    </asp:content>


