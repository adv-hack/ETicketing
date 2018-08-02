<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CATBasket.ascx.vb" Inherits="UserControls_CATBasket" %>
<%@ Register Src="~/UserControls/Package/PackageSummary.ascx" TagName="PackageSummary" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/DirectDebitSummary.ascx" TagName="DirectDebitSummary" TagPrefix="Talent" %>

<asp:PlaceHolder ID="plhCATDetails" runat="server">
    
    <div class="panel ebiz-cat-basket">
        <h2><asp:Label ID="lblCATHeader" runat="server"></asp:Label></h2>
        <p class="ebiz-instructions-top">
            <asp:Label ID="lblCATInstructionTop" runat="server"></asp:Label>
        </p>

        <div class="row ebiz-transaction-ref">
            <div class="large-3 columns">
                <asp:Literal ID="ltltransactionRef" runat="server" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="ltltransactionRefValue" runat="server" />
            </div>
        </div>
        <div class="row ebiz-transaction-date">
            <div class="large-3 columns">
                <asp:Literal ID="ltltransactionDate" runat="server" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="ltltransactionDateValue" runat="server" />
            </div>
        </div>
        <div class="row ebiz-booking-ref">
            <div class="large-3 columns">
                <asp:Literal ID="ltlCallID" runat="server" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="ltlCallIDValue" runat="server" />
            </div>
        </div>

        <asp:PlaceHolder ID="plhPackageDescription" runat="server" Visible="false">
            <div class="row ebiz-package-description">
                <div class="large-3 columns">
                <asp:Literal ID="ltlPackageDescription" runat="server" />
                </div>
                <div class="large-9 columns">
                <asp:Literal ID="ltlPackageDescriptionValue" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhDirectDebitPaymentsMade" runat="server" Visible="false">
        <div class="row ebiz-ddpayments-value">
            <div class="large-3 columns">
                 <asp:Literal ID="ltlDirectDebitPaymentsMade" runat="server" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="ltlDirectDebitPaymentsMadeValue" runat="server" />
                <asp:HyperLink ID="hplDirectDebitSummary" runat="server">&nbsp;<i class="fa fa-table"></i></asp:HyperLink>
                <div id="ebiz-direct-debit-summary-reveal" class="reveal large" data-reveal>
                    <Talent:DirectDebitSummary ID="uscDirectDebitSummary" runat="server" />
                    <button class="close-button" data-close aria-label="Close modal" type="button">
                        <i class="fa fa-times" aria-hidden="true"></i>
                    </button>
                </div>
            </div>
        </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhCATRepeater" runat="server">
            <asp:Repeater ID="rptOrderDetails" runat="server">
                <HeaderTemplate>
                    <table class="stack ebiz-cat">
                        <thead>
                            <tr>
                                <th class="ebiz-customer-no" scope="col"><%# GetText("CustomerNoLabel")%></th>
                                <th class="ebiz-customer-name" scope="col"><%# GetText("CustomerNameLabel")%></th>
                                <th class="ebiz-membership-match" scope="col"><%# GetText("MembershipMatchLabel")%></th>
                                <asp:PlaceHolder ID="plhSeatColumn" runat="server">
                                <th class="ebiz-seat" scope="col"><%# GetText("SeatLabel")%></th>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhQuantityColumn" runat="server">
                                <th class="ebiz-quantity" scope="col"><%# GetText("QuantityLabel")%></th>
                                </asp:PlaceHolder>
                                <th class="ebiz-price-band" scope="col"><%# GetText("PriceBandLabel")%></th>
                                <th class="ebiz-price" scope="col"><%# GetText("PriceLabel")%></th>
                                <th class="ebiz-status" scope="col"><%# GetText("StatusLabel")%></th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td data-title="<%# GetText("CustomerNoLabel")%>" class="ebiz-customer-no">
                            <%# DataBinder.Eval(Container.DataItem, "CustomerNumber").ToString.TrimStart(GlobalConstants.LEADING_ZEROS)%>
                        </td>
                        <td data-title="<%# GetText("CustomerNameLabel")%>" class="ebiz-customer-name">
                            <%# DataBinder.Eval(Container.DataItem, "CustomerName")%>
                        </td>
                        <td data-title="<%# GetText("MembershipMatchLabel")%>" class="ebiz-membership-match">
                            <%#DataBinder.Eval(Container.DataItem, "ProductDescription")%>
                        </td>
                        <asp:PlaceHolder ID="plhSeatColumn" runat="server">
                        <td data-title="<%# GetText("SeatLabel")%>" class="ebiz-seat">
                            <%# CheckSeatDetailsForAttendance(DataBinder.Eval(Container.DataItem, "Seat").ToString, DataBinder.Eval(Container.DataItem, "ProductType").ToString, DataBinder.Eval(Container.DataItem, "AllocatedSeat").ToString)%>
                        </td>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhQuantityColumn" runat="server">
                        <td data-title="<%# GetText("QuantityLabel")%>" class="ebiz-quantity">
                            <%#DataBinder.Eval(Container.DataItem, "BulkQty")%>                            
                        </td>
                        </asp:PlaceHolder>
                        <td data-title="<%# GetText("PriceBandLabel")%>" class="ebiz-price-band">
                            <%# DataBinder.Eval(Container.DataItem, "PriceBandDesc")%>
                        </td>
                        <td data-title="<%# GetText("PriceLabel")%>" class="ebiz-price">
                            <%# DataBinder.Eval(Container.DataItem, "SalePrice")%>
                        </td>
                        <td data-title="<%# GetText("StatusLabel")%>" class="ebiz-status">
                            <%# GetStatusText(Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "StatusCode")))%>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </asp:PlaceHolder>

        <Talent:PackageSummary ID="uscPackageSummaryCancel" runat="server" display="false" />
        <asp:PlaceHolder ID="plhPackageSummaryAmendOrTransfer" runat="server" Visible="false">
        <div class="row ebiz-package-id">
            <div class="large-3 columns">
                <asp:Literal ID="ltlPackageIDLabel" runat="server" />
            </div>
            <div class="large-9 columns">
                <asp:Literal ID="ltlPackageIDValue" runat="server" />
            </div>
        </div>
        <asp:Repeater ID="rptPackageSummaryAmendOrTransfer" runat="server">
                <HeaderTemplate>
                    <table class="stack ebiz-cat">
                        <thead>
                            <tr>
                                <th class="ebiz-component-qunatity-add" scope="col">
                                    <%= ComponentQuantityAddColHeader%>
                                </th>
                                <th class="ebiz-component-qunatity-del" scope="col">
                                    <%= ComponentQuantityDelColHeader%>
                                </th>
                                <th class="ebiz-component-qunatity-amd" scope="col">
                                    <%=ComponentQuantityAmdColHeader%>
                                </th>
                                <th class="ebiz-component" scope="col">
                                    <%= ComponentColHeader%>
                                </th>
                                <th class="ebiz-component-price" scope="col">
                                    <%= ComponentPriceColHeader%>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td data-title="<%=ComponentQuantityAddColHeader%>" class="ebiz-component-qunatity-add">
                            <%# DataBinder.Eval(Container.DataItem, "QuantityAdded")%>
                        </td>
                        <td data-title="<%=ComponentQuantityDelColHeader%>" class="ebiz-component-qunatity-del">
                            <%# DataBinder.Eval(Container.DataItem, "QuantityDeleted")%>
                        </td>
                        <td data-title="<%=ComponentQuantityAmdColHeader%>" class="ebiz-component-qunatity-amd">
                            <%# DataBinder.Eval(Container.DataItem, "QuantityAmended")%>
                        </td>
                        <td data-title="<%= ComponentColHeader%>" class="ebiz-component">
                            <%# DataBinder.Eval(Container.DataItem, "ComponentDescription")%>
                        </td>
                        <td data-title="<%= ComponentPriceColHeader%>" class="ebiz-component-price">
                            <%# FormatCurrency(DataBinder.Eval(Container.DataItem, "TotalPrice"))%>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </asp:PlaceHolder>

         <asp:PlaceHolder ID="plhYouWillBeTakenOutOfBulkMode" runat="server" Visible="false">
            <p class="ebiz-instructions-bottom ebiz-instructions-multiple-cat-bulk-warning">
                <asp:Label ID="lblYouWillBeTakenOutOfBulkMode" runat="server" Visible="false"></asp:Label>
            </p>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhInstructionBottom" runat="server" Visible="false">
            <p class="ebiz-instructions-bottom">
                <asp:Label ID="lblCATInstructionBottom" runat="server" Visible="false"></asp:Label>
            </p>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhMessage" runat="server">
            <div class="alert-box ebiz-message-to-user">
                <asp:Label ID="lblMessageToUser" runat="server"></asp:Label>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhYesOrNoOrOk" runat="server">
            <asp:Button ID="btnYes" runat="server" class="button ebiz-yes" />
            <asp:Button ID="btnNo" runat="server" class="button ebiz-no" />
        </asp:PlaceHolder>
    </div>
</asp:PlaceHolder>