<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CheckoutDeliveryAddress.ascx.vb"
    Inherits="UserControls_CheckoutDeliveryAddress" %>
<div id="checkout-delivery-details" class="default-form">
<asp:Literal ID="ltlPreferredDatesScript" runat="server" Visible="false"></asp:Literal>
    <div class="title">
        <h2>
            <asp:Label ID="TitleText" runat="server"></asp:Label>
        </h2>
        <p class="instructions">
            <asp:Label ID="InstructionsText" runat="server"></asp:Label></p>
        <p class="error">
            <asp:Label ID="ErrorLabel" runat="server" CssClass="error" ForeColor="Red"></asp:Label></p>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="DeliveryDetails" />
    </div>
    <div class="address1">
        <asp:Label ID="SelectAddressLabel" runat="server" AssociatedControlID="SelectAddressDDL"></asp:Label>
        <asp:DropDownList ID="SelectAddressDDL" CssClass="select" runat="server" AutoPostBack="true">
        </asp:DropDownList>
        <asp:Label ID="SelectAddressHelpLabel" runat="server" CssClass="help selectaddress"></asp:Label>
    </div>
    <% CreateAddressingJavascript()%>
    <% CreateAddressingHiddenFields()%>
    <div id="FindAddressButtonRow" class="FindAddressButtonRow" runat="server">
        <label>
            &nbsp;</label>
        <a id="AddressingLinkButton" name="AddressingLinkButtton" class="AddressingLinkButton"
            href="Javascript:addressingPopup();">
            <%=GetAddressingLinkText()%></a>
    </div>
    <div id="AddressLine1Row" class="AddressLine1Row" runat="server">
        <div class="building">
            <asp:Label ID="BuildingLabel" runat="server" AssociatedControlID="building"></asp:Label>
            <asp:TextBox ID="building" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="BuildingRFV" runat="server" ValidationGroup="DeliveryDetails"
                Text="*" Display="Static" ControlToValidate="building"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="BuildingRegEx" runat="server" ValidationGroup="DeliveryDetails"
                Text="*" Display="Static" ControlToValidate="building"></asp:RegularExpressionValidator>
        </div>
    </div>
    <div id="AddressLine2Row" class="address2" runat="server">
        <asp:Label ID="AddressLabel2" runat="server" AssociatedControlID="Address2"></asp:Label>
        <asp:TextBox ID="Address2" CssClass="input-l" runat="server" MaxLength="30"></asp:TextBox>
        <asp:RequiredFieldValidator ID="Address2RFV" runat="server" ValidationGroup="DeliveryDetails"
            Text="*" Display="Static" ControlToValidate="Address2"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="Address2RegEx" ControlToValidate="Address2" runat="server"
            ValidationGroup="DeliveryDetails" Text="*" Display="Static"></asp:RegularExpressionValidator>
    </div>
    <div id="AddressLine3Row" class="address3" runat="server">
        <asp:Label ID="AddressLabel3" runat="server" AssociatedControlID="Address3"></asp:Label>
        <asp:TextBox ID="Address3" CssClass="input-l" runat="server" MaxLength="30"></asp:TextBox>
        <asp:RequiredFieldValidator ID="Address3RFV" runat="server" ValidationGroup="DeliveryDetails"
            Text="*" Display="Static" ControlToValidate="Address3"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="Address3RegEx" ControlToValidate="Address3" runat="server"
            ValidationGroup="DeliveryDetails" Text="*" Display="Static"></asp:RegularExpressionValidator>
    </div>
    <div id="AddressLine4Row" class="address4" runat="server">
        <asp:Label ID="AddressLabel4" runat="server" AssociatedControlID="Address4"></asp:Label>
        <asp:TextBox ID="Address4" CssClass="input-l" runat="server" MaxLength="25"></asp:TextBox>
        <asp:RequiredFieldValidator ID="Address4RFV" runat="server" ValidationGroup="DeliveryDetails"
            Text="*" Display="Static" ControlToValidate="Address4"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="Address4RegEx" ControlToValidate="Address4" runat="server"
            ValidationGroup="DeliveryDetails" Text="*" Display="Static"></asp:RegularExpressionValidator>
    </div>
    <div id="AddressLine5Row" class="address5" runat="server">
        <asp:Label ID="AddressLabel5" runat="server" AssociatedControlID="Address5"></asp:Label>
        <asp:TextBox ID="Address5" CssClass="input-l" runat="server" MaxLength="25"></asp:TextBox>
        <asp:RequiredFieldValidator ID="Address5RFV" runat="server" ValidationGroup="DeliveryDetails"
            Text="*" Display="Static" ControlToValidate="Address5" Enabled="false"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="Address5RegEx" ControlToValidate="Address5" runat="server"
            ValidationGroup="DeliveryDetails" Text="*" Display="Static"></asp:RegularExpressionValidator>
    </div>
    <div id="AddressPostcodeRow" class="postcode" runat="server">
        <asp:Label ID="PostcodeLabel" runat="server" AssociatedControlID="postcode"></asp:Label>
        <asp:TextBox ID="postcode" CssClass="input-m" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="postcodeRFV" runat="server" ValidationGroup="DeliveryDetails"
            Text="*" Display="Static" ControlToValidate="postcode"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="postcodeRegEx" ControlToValidate="postcode" runat="server"
            ValidationGroup="DeliveryDetails" Text="*" Display="Static"></asp:RegularExpressionValidator>
    </div>
    <div id="AddressCountryRow" class="country" runat="server">
        <asp:Label ID="countryLabel" runat="server" AssociatedControlID="CountryDDL"></asp:Label>
        <asp:DropDownList ID="CountryDDL" CssClass="select" runat="server">
        </asp:DropDownList>
        <asp:RegularExpressionValidator ID="CountryDDLRegEx" ControlToValidate="CountryDDL"
            runat="server" ValidationGroup="DeliveryDetails" Text="*" Display="Static"></asp:RegularExpressionValidator>
    </div>
    <div class="delivery-contact">
        <asp:Label ID="DeliveryContactLabel" runat="server" AssociatedControlID="DeliveryContact"></asp:Label>
        <asp:TextBox ID="DeliveryContact" CssClass="input-l" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="DeliveryContactRFV" runat="server" ValidationGroup="DeliveryDetails"
            Text="*" Display="Static" ControlToValidate="DeliveryContact"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="DeliveryContactRegEx" ControlToValidate="DeliveryContact"
            runat="server" ValidationGroup="DeliveryDetails" Text="*" Display="Static"></asp:RegularExpressionValidator>
    </div>
    <div class="delivery-instructions">
        <asp:Label ID="DeliveryInsructionsLabel" runat="server" AssociatedControlID="DeliveryInstructions"></asp:Label>
        <asp:TextBox ID="DeliveryInstructions" CssClass="input-l" runat="server" TextMode="multiLine"></asp:TextBox>
        <asp:RequiredFieldValidator ID="DeliveryInstructionsRFV" runat="server" ValidationGroup="DeliveryDetails"
            Text="*" Display="Static" ControlToValidate="DeliveryInstructions"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="DeliveryInstructionsRegEx" ControlToValidate="DeliveryInstructions"
            runat="server" ValidationGroup="DeliveryDetails" Text="*" Display="Static"></asp:RegularExpressionValidator>
        <asp:Label ID="DeliveryInsructionsHelpLabel" runat="server" CssClass="help deliveryinstruction"></asp:Label>
    </div>
    <div id="PurchaseOrderRow" class="PurchaseOrder" runat="server">
        <asp:Label ID="PurchaseOrderLabel" runat="server" AssociatedControlID="PurchaseOrder"></asp:Label>
        <asp:TextBox ID="PurchaseOrder" CssClass="input-l" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="PurchaseOrderRFV" runat="server" ValidationGroup="DeliveryDetails"
            Text="*" Display="Static" ControlToValidate="PurchaseOrder"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="PurchaseOrderRegEx" ControlToValidate="PurchaseOrder"
            runat="server" ValidationGroup="DeliveryDetails" Text="*" Display="Static"></asp:RegularExpressionValidator>
        <asp:Label ID="PurchaseOrderHelpLabel" runat="server" CssClass="help purchaseorder"></asp:Label>
    </div>
    <div id="DeliveryDayRow" class="DeliveryDay" runat="server">
        <asp:Label ID="DeliveryDayLabel" runat="server" CssClass="deliveryday label"></asp:Label>
        <asp:Label ID="DeliveryDay" runat="server" CssClass="deliveryday value"></asp:Label>
    </div>
    <div id="DeliveryDateRow" class="DeliveryDate" runat="server">
        <asp:Label ID="DeliveryDateLabel" runat="server" CssClass="deliverydate label"></asp:Label>
        <asp:Label ID="DeliveryDate" runat="server" CssClass="deliverydate value"></asp:Label>
        <asp:Label ID="DeliveryDateHelpLabel" runat="server" CssClass="help deliverydate"></asp:Label>
    </div>
    <div id="PreferredDateRow" class="PreferredDate" runat="server">
        <asp:Label ID="PreferredDateLabel" runat="server" CssClass="deliverydate label"></asp:Label>
        <asp:TextBox ID="PreferredDate" CssClass="input-l datepicker" runat="server"></asp:TextBox>
        <asp:RegularExpressionValidator ID="PreferredDateRegEx" ControlToValidate="PreferredDate"
            runat="server" ValidationGroup="DeliveryDetails" Text="*" Display="Static"></asp:RegularExpressionValidator>
        <asp:Label ID="PreferredDateHelpLabel" runat="server" CssClass="help preferreddate"></asp:Label>
    </div>
    <div class="save-address">
        <asp:CheckBox ID="SaveAddress" runat="server" /></div>
    <div class="tandc">
        <asp:CheckBox ID="tandc" runat="server" /></div>
    <div class="proceed-btn">
        <label>
            &nbsp;</label>
        <asp:Button ID="proceed" CssClass="button" runat="server" CausesValidation="true"
            ValidationGroup="DeliveryDetails" /></div>
    <asp:PlaceHolder ID="plhDeliveryMessage" runat="server" Visible="False">
        <div class="delivery-message">
            <asp:Literal ID="ltlDeliveryMessage" runat="server" />
        </div>
    </asp:PlaceHolder>
</div>
