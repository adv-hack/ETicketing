<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OrderEnquiryDetails.ascx.vb" Inherits="UserControls_OrderEnquiryDetails" %>
<%@ Register Src="SummaryTotals.ascx" TagName="SummaryTotals" TagPrefix="Talent" %>

<asp:PlaceHolder ID="plhErrorList" runat="server">
    <asp:BulletedList ID="ErrorList" runat="server" CssClass="alert-box alert"></asp:BulletedList>
</asp:PlaceHolder>

<div id="order-info">
    <asp:FormView ID="OrderInfoView" runat="server" summary="Order details">
        <ItemTemplate>
            <table summary="Order details" cellspacing="0">
                <tr>
                    <th scope="col">
                        <asp:Label ID="WebOrderNoLabel" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <td>
                        <asp:Label ID="WebOrderNo" runat="server" Text='<%# Container.DataItem("PROCESSED_ORDER_ID") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th scope="col">
                        <asp:Label ID="OrderNoLabel" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <td>
                        <asp:Label ID="OrderNo" runat="server" Text='<%# Container.DataItem("BACK_OFFICE_ORDER_ID") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th scope="col">
                        <asp:Label ID="CustomerRefLabel" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <td>
                          <asp:Label ID="CustomerRef" runat="server" Text='<%# Container.DataItem("CONTACT_NAME") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th scope="col">
                        <asp:Label ID="DeliveryContactLabel" runat="server" OnPreRender="GetText"></asp:Label>
                    </td>
                        <td>
                            <asp:Label ID="DeliveryContact" runat="server" Text='<%# Container.DataItem("CONTACT_NAME") %>'></asp:Label>
                        </td>
                </tr>
                <tr>
                    <th scope="col">
                        <asp:Label ID="OrderDateLabel" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <td>
                        <asp:Label ID="OrderDate" runat="server" Text='<%# Container.DataItem("CREATED_DATE") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th scope="col">
                        <asp:Label ID="DeliverToLabel" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <td>
                        <asp:Label ID="Deliver1" runat="server" Text='<%# Container.DataItem("ADDRESS_LINE_1") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th scope="col">&nbsp;
                    </th>
                    <td>
                        <asp:Label ID="Deliver2" runat="server" Text='<%# Container.DataItem("ADDRESS_LINE_2") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th scope="col">&nbsp;
                    </th>
                    <td>
                        <asp:Label ID="Deliver3" runat="server" Text='<%# Container.DataItem("ADDRESS_LINE_3") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th scope="col">&nbsp;
                    </th>
                    <td>
                        <asp:Label ID="Deliver4" runat="server" Text='<%# Container.DataItem("ADDRESS_LINE_4") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th scope="col">&nbsp;
                    </th>
                    <td>
                        <asp:Label ID="Deliver5" runat="server" Text='<%# Container.DataItem("ADDRESS_LINE_5") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th scope="col">&nbsp;
                    </th>
                    <td>
                        <asp:Label ID="Deliver6" runat="server" Text='<%# Container.DataItem("POSTCODE") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th scope="col">&nbsp;
                    </th>
                    <td>
                        <asp:Label ID="Deliver7" runat="server" Text='<%# Container.DataItem("COUNTRY") %>'></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th scope="col">
                        <asp:Label ID="DeliveryTypeLabel" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <td>
                        <asp:Label ID="DeliveryType" runat="server" Text='<%# talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("DELIVERY_TYPE_DESCRIPTION")) %>'></asp:Label>
                    </td>
                </tr>
            </table>
        </ItemTemplate>
    </asp:FormView>
</div>
<div id="order-lines">
    <asp:Repeater ID="OrderLines" runat="server">
        <HeaderTemplate>
            <table summary="order lines" cellspacing="0">
                <tr>
                    <th class="line-no" scope="col">
                        <asp:Label ID="LineNoHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <th class="product-code" scope="col">
                        <asp:Label ID="ProductCodeHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <th class="description" scope="col">
                        <asp:Label ID="DescriptionHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <th class="shipment-no" scope="col">
                        <asp:Label ID="ShipmentNoHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <th class="despatch-date" scope="col">
                        <asp:Label ID="DespatchDateHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <th class="quantity-ordered" scope="col">
                        <asp:Label ID="QuantityOrderedHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <th class="quantity-shipped" scope="col">
                        <asp:Label ID="QuantityShippedHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                    <%--<th class="value" scope="col">
                        <asp:Label ID="ValueHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>--%>
                    <asp:PlaceHolder ID="plhShowValue" runat="server" Visible="false" OnPreRender="CanDisplayThisColumn">
                        <th class="value" scope="col">
                            <asp:Label ID="ValueHeader" runat="server" OnPreRender="GetText"></asp:Label>
                        </th>
                    </asp:PlaceHolder>

                    <th class="tracking-code" scope="col">
                        <asp:Label ID="DeliveryTrackingHeader" runat="server" OnPreRender="GetText"></asp:Label>
                    </th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="line-no">
                    <asp:Label ID="LineNo" runat="server" Text='<%# Container.DataItem("LINE_NUMBER") %>'></asp:Label>
                </td>
                <td class="product-code">
                    <asp:Label ID="ProductCode" runat="server" Text='<%# Container.DataItem("PRODUCT_CODE") %>'></asp:Label>
                </td>
                <td class="description">
                    <asp:Label ID="Description" runat="server" Text='<%# Container.DataItem("PRODUCT_DESCRIPTION_1") %>'></asp:Label>
                </td>
                <td class="shipment-no">
                    <asp:Label ID="ShipmentNo" runat="server" Text='<%# Container.DataItem("SHIPMENT_NUMBER") %>'></asp:Label>
                </td>
                <td class="despatch-date">
                    <asp:Label ID="DespatchDate" runat="server" Text=' <%# Talent.eCommerce.Utilities.SuppressDefaultDate(Container.DataItem("DATE_SHIPPED").tostring) %>'></asp:Label>
                </td>
                <td class="quantity-ordered">
                    <asp:Label ID="QuantityOrdered" runat="server" Text='<%# CInt(Container.DataItem("QUANTITY")) %>'></asp:Label>
                </td>
                <td class="quantity-shipped">
                    <asp:Label ID="QuantityShipped" runat="server" Text='<%# Container.DataItem("QUANTITY_SHIPPED") %>'></asp:Label>
                </td>
                <%--<td class="value">
                    <asp:Label ID="Value" runat="server" Text='<%# FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Container.DataItem("LINE_PRICE_GROSS"), 0.01, False))%>'></asp:Label>
                </td>--%>
                <asp:PlaceHolder ID="plhShowValue" runat="server" Visible="false" OnPreRender="CanDisplayThisColumn">
                    <td class="value">
                        <asp:Label ID="Value" runat="server" Text='<%# TDataObjects.PaymentSettings.FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Container.DataItem("LINE_PRICE_GROSS"), 0.01, False), Container.DataItem("CURRENCY"), Container.DataItem("BUSINESS_UNIT"))%>'></asp:Label>
                    </td>
                </asp:PlaceHolder>                
                <td class="tracking-code">
                    <asp:HyperLink runat="server" ID="hypTrack" NavigateUrl="" Text='<%# Container.DataItem("TRACKING_NO") %>'></asp:HyperLink>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>
<div id="summary-totals-wrap">
    <Talent:SummaryTotals ID="SummaryTotals1" runat="server" Usage="ORDERENQUIRY" />
</div>
<div>
    <asp:Button ID="ReOrderButton" runat="server" CssClass="button" />
</div>
