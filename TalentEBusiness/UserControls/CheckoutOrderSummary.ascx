<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CheckoutOrderSummary.ascx.vb" Inherits="UserControls_CheckoutOrderSummary" %>
<%@ Register Src="BasketDetails.ascx" TagName="BasketDetails" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/DeliverySelection.ascx" TagName="DeliverySelection" TagPrefix="Talent" %>
<%@ Reference Control="~/UserControls/summaryTotals.ascx" %>
<asp:BulletedList ID="ErrorList" runat="server" CssClass="error">
</asp:BulletedList>
<Talent:BasketDetails ID="BasketDetails1" runat="server" Usage="ORDERSUMMARY" />
<div>
    <Talent:DeliverySelection id="DeliverySelection1" runat="server" />
</div>
<div class="summary-delivery-details" id="summaryDeliveryDetails" runat="server">
    <asp:Label ID="DeliveryDetailsTitle" runat="server"></asp:Label>
    <table summary="deleivery details" cellspacing="0">
        <tr>
            <th scope="row">
                <asp:Label ID="BuildingLabel" runat="server"></asp:Label>
            </th>
            <td>
                <asp:Label ID="building" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <th scope="row">
                <asp:Label ID="AddressLabel2" runat="server"></asp:Label>
            </th>
            <td>
                <asp:Label ID="Address2" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <th scope="row">
                <asp:Label ID="AddressLabel3" runat="server"></asp:Label>
            </th>
            <td>
                <asp:Label ID="Address3" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <th scope="row">
                <asp:Label ID="AddressLabel4" runat="server"></asp:Label>
            </th>
            <td>
                <asp:Label ID="Address4" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <th scope="row">
                <asp:Label ID="PostcodeLabel" runat="server"></asp:Label>
            </th>
            <td>
                <asp:Label ID="postcode" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <th scope="row">
                <asp:Label ID="AddressLabel5" runat="server"></asp:Label>
            </th>
            <td>
                <asp:Label ID="Address5" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <th scope="row">
                <asp:Label ID="countryLabel" runat="server"></asp:Label>
            </th>
            <td>
                <asp:Label ID="country" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <th scope="row">
                <asp:Label ID="DeliveryContactLabel" runat="server"></asp:Label>
            </th>
            <td>
                <asp:Label ID="DeliveryContact" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <th scope="row">
                <asp:Label ID="DeliveryInsructionsLabel" runat="server"></asp:Label>
            </th>
            <td>
                <asp:Label ID="DeliveryInstructions" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <asp:Panel ID="DeliveryDateTextPanel" runat="server" Visible="False">            
            <th scope="row">
            <asp:Label ID="DeliveryProjectedDateLabel" runat="server"></asp:Label>
            </th>
            <td>
            <asp:Label ID="DeliveryProjectedDate" runat="server"></asp:Label>
            </td>
            </asp:Panel>
            <asp:Panel ID="DeliveryDateCalendarPanel" runat="server">
            <th></th>
            <td>
            <asp:Calendar ID="DeliveryDateCalendar" runat="server"></asp:Calendar>            
            </td>
            </asp:Panel>            
        </tr>
    </table>
</div>
<div id="paymentMethodWrap" class="payment-method default-form" runat="server">
    <h2>
      <asp:Label ID="lblSelectPayMethod" runat="server"></asp:Label>
    </h2>
    <div>
        <asp:Label ID="lblPayBy" runat="server" AssociatedControlID="PaymentTypeDDL"></asp:Label>
        <asp:DropDownList ID="PaymentTypeDDL" runat="server" AutoPostBack="True" CssClass="select">
        </asp:DropDownList>
    </div>
</div>
<div id="summary-gift-message">
    <asp:CheckBox ID="GiftMessage" CssClass="input" runat="server" />
</div>
<div id="summary-proceed">
    <asp:Button ID="proceed" CssClass="button" runat="server" />
</div>
