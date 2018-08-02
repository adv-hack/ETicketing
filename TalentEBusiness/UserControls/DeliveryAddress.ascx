<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DeliveryAddress.ascx.vb" Inherits="UserControls_DeliveryAddress" %>
<%@ Register Src="~/UserControls/DeliverySelection.ascx" TagName="DeliverySelection" TagPrefix="Talent" %>

<div class="ebiz-delivery-address-wrapper">

    <asp:PlaceHolder ID="plhDeliveryAddressInstructions" runat="server">
        <p><asp:Literal ID="ltlDeliveryAddressInstructions" runat="server" /></p>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhRadioButtons" runat="server"  ClientIDMode="Static">
        <div class="row ebiz-address-type">
            <div class="large-12 columns">
                <asp:Label ID="lblUseSameAddressAddressInstructions" runat="server" class="inline" />
                <asp:Label ID="lblRetailAddressInstructions" runat="server" class="inline" />
                <asp:Label ID="lblTicketingAddressInstructions" runat="server" class="inline" />
                <asp:CheckBox ID="chkUseSameAddress" runat="server" ClientIDMode="Static" Visible="True" Checked="true" AutoPostBack="true" />
                <asp:RadioButtonList ID="rdolDisplayOption" runat="server" ClientIDMode="Static" Visible="false" AutoPostBack="True">
                    <asp:ListItem Value="0" Selected="False" />
                    <asp:ListItem Value="1" Selected="false" />
                </asp:RadioButtonList>
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhDeliveryAddress" runat="server">
        <asp:PlaceHolder ID="plhAddressContactNameRow" runat="server">
            <div class="row ebiz-contact-name">
                <div class="medium-3 columns">
                    <asp:Label ID="lblContactName" runat="server" ClientIDMode="Static" AssociatedControlID="txtContactName" CssClass="inline" />
                </div>
                <div class="medium-9 columns">
                    <asp:TextBox ID="txtContactName" CssClass="input-l" runat="server" ClientIDMode="Static" />
                    <asp:RequiredFieldValidator ID="rfvContactName" runat="server" ControlToValidate="txtContactName"
                        SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" ClientIDMode="Static" Enabled="true" CssClass="error" />
                    <asp:RegularExpressionValidator ControlToValidate="txtContactName" ID="regexContactName" runat="server"
                        SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" Enabled="true" CssClass="error" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhSelectAddressRow" runat="server">
        <div class="row ebiz-contact-name">
                <div class="medium-3 columns">
                    <asp:Label ID="lblSelectAddress" runat="server" ClientIDMode="Static" AssociatedControlID="ddlSelectAddress" CssClass="inline"/>
                </div>
                <div class="medium-9 columns">
                    <asp:DropDownList ID="ddlSelectAddress" runat="server" ClientIDMode="Static" AutoPostBack="True" />
                    <asp:CheckBox ID="chkSaveTheAddress" runat="server" ClientIDMode="Static" Visible="false" />
                    <asp:HyperLink ID="hplFindAddress" runat="server" ClientIDMode="Static" />
                    <asp:Label ID="lblSelectAddressHelpLabel" runat="server" CssClass="help ebiz-select-address" />
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhAddressMonikerRow" runat="server">
            <div class="row ebiz-address-moniker-row">
                <div class="medium-3 columns">
                    <asp:Label ID="lblMonikerAddress" runat="server" ClientIDMode="Static" AssociatedControlID="txtMonikerAddress" CssClass="inline" />
                </div>
                <div class="medium-9 columns">
                    <asp:TextBox ID="txtMonikerAddress" CssClass="input-l" runat="server" ClientIDMode="Static" />
                    <asp:RequiredFieldValidator ID="rfvMonikerAddress" runat="server" ControlToValidate="txtMonikerAddress"
                            SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" ClientIDMode="Static" Enabled="true" CssClass="error" />
                    <asp:RegularExpressionValidator ControlToValidate="txtMonikerAddress" ID="regexMonikerAddress" runat="server"
                            SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" Enabled="true" CssClass="error" />
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhFindAddressButtonRow" runat="server" Visible="false">
            <asp:Literal ID="ltlAddressingHiddenFields" runat="server" />
            <div class="row ebiz-find-address">
                <div class="large-12 columns">
                    <a id="AddressingLinkButton" name="AddressingLinkButtton" class="AddressingLinkButton" href="Javascript:addressingPopup();"><%=GetAddressingLinkText()%></a>
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhAddressLine1Row" runat="server">
            <div class="row ebiz-address-line1">
                <div class="medium-3 columns">
                    <asp:Label ID="lblAddress1" runat="server" ClientIDMode="Static" AssociatedControlID="txtAddress1" CssClass="inline" />
                </div>
                <div class="medium-9 columns">
                    <asp:TextBox ID="txtAddress1" CssClass="input-l" runat="server" ClientIDMode="Static"  />
                    <asp:RequiredFieldValidator ID="rfvAddress1" runat="server" ControlToValidate="txtAddress1"
                            SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" ClientIDMode="Static" Enabled="true" CssClass="error" />
                    <asp:RegularExpressionValidator ControlToValidate="txtAddress1" ID="regexAddress1" runat="server"
                            SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" Enabled="true" CssClass="error" />
                </div>
            </div>        
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhAddressLine2Row" runat="server">
            <div class="row ebiz-address-line2">
                <div class="medium-3 columns">
                    <asp:Label ID="lblAddress2" runat="server" ClientIDMode="Static" AssociatedControlID="txtAddress2" CssClass="inline" />
                </div>
                <div class="medium-9 columns">
                    <asp:TextBox ID="txtAddress2" CssClass="input-l" runat="server" ClientIDMode="Static" />
                    <asp:RequiredFieldValidator ID="rfvAddress2" runat="server" ControlToValidate="txtAddress2"
                            SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" ClientIDMode="Static" Enabled="true" CssClass="error" />
                    <asp:RegularExpressionValidator ControlToValidate="txtAddress2" ID="regexAddress2" runat="server"
                            SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" Enabled="true" CssClass="error" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhAddressLine3Row" runat="server">
        <div class="row ebiz-address-line3">
            <div class="medium-3 columns">
                <asp:Label ID="lblTownCity" runat="server" ClientIDMode="Static" AssociatedControlID="txtTownCity" CssClass="inline" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="txtTownCity" CssClass="input-l" runat="server" ClientIDMode="Static"  />
                <asp:RequiredFieldValidator ID="rfvTownCity" runat="server" ControlToValidate="txtTownCity"
                        SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" ClientIDMode="Static" Enabled="true" CssClass="error" />
                <asp:RegularExpressionValidator ControlToValidate="txtTownCity" ID="regexTownCity" runat="server"
                        SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" Enabled="true" CssClass="error" />
            </div>
        </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhAddressLine4Row" runat="server">
        <div class="row ebiz-address-line4">
            <div class="medium-3 columns">
                <asp:Label ID="lblCounty" runat="server" ClientIDMode="Static" AssociatedControlID="txtCounty" CssClass="inline" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="txtCounty" CssClass="input-l" runat="server" ClientIDMode="Static" />
                <asp:RequiredFieldValidator ID="rfvCounty" runat="server" ControlToValidate="txtCounty"
                        SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" ClientIDMode="Static" Enabled="true" CssClass="error" />
                <asp:RegularExpressionValidator ControlToValidate="txtCounty" ID="regexCounty" runat="server"
                        SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" Enabled="true" CssClass="error" />
            </div>
        </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhAddressLine5Row" runat="server">
        <div class="row ebiz-address-line5">
            <div class="medium-3 columns">
                <asp:Label ID="lblAddress5" runat="server" ClientIDMode="Static" AssociatedControlID="txtAddress5" CssClass="inline" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="txtAddress5" CssClass="input-l" runat="server" ClientIDMode="Static" />
                <asp:RequiredFieldValidator ID="rfvAddress5" runat="server" ControlToValidate="txtAddress5"
                        SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" ClientIDMode="Static" Enabled="true" CssClass="error" />
                <asp:RegularExpressionValidator ControlToValidate="txtAddress5" ID="regexAddress5" runat="server"
                        SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" Enabled="true" CssClass="error" />
            </div>
        </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhAddressPostCodeRow" runat="server">
        <div class="row ebiz-address-postcode">
            <div class="medium-3 columns">
                <asp:Label ID="lblPostCode" runat="server" ClientIDMode="Static" AssociatedControlID="txtPostCode" CssClass="inline" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="txtPostCode" CssClass="input-l" runat="server" ClientIDMode="Static" />
                <asp:RequiredFieldValidator ID="rfvPostCode" runat="server" ControlToValidate="txtPostCode"
                        SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" ClientIDMode="Static" Enabled="true" CssClass="error" />
                <asp:RegularExpressionValidator ControlToValidate="txtPostCode" ID="regexPostCode" runat="server"
                        SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" Enabled="true" CssClass="error" />
            </div>
        </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhAddressCountryRow" runat="server">
        <div class="row ebiz-address-country">
            <div class="medium-3 columns">
                <asp:Label ID="lblCountry" runat="server" ClientIDMode="Static" AssociatedControlID="ddlCountries" CssClass="inline" />
            </div>
            <div class="medium-9 columns">
                <asp:DropDownList ID="ddlCountries" runat="server" ClientIDMode="Static" AutoPostBack="<%# GetCountryDropDownPostbackOption()%>"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvCountry" runat="server" ControlToValidate="ddlCountries"
                        InitialValue=" -- " SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" ClientIDMode="Static" Enabled="true" CssClass="error" />
            </div>
        </div>
        </asp:PlaceHolder>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhRetail" runat="server">
        <Talent:DeliverySelection id="DeliverySelection1" runat="server" />
        <asp:PlaceHolder ID="plhDeliveryInstructions" runat="server">
            <div class="row ebiz-delivery-instructions">
                <div class="medium-3 columns">
                    <asp:Label ID="DeliveryInsructionsLabel" runat="server" AssociatedControlID="DeliveryInstructions" />
                </div>
                <div class="medium-9 columns">
                    <asp:TextBox ID="DeliveryInstructions" CssClass="input-l" runat="server" TextMode="multiLine" />
                    <asp:RequiredFieldValidator ID="DeliveryInstructionsRFV" runat="server" ControlToValidate="DeliveryInstructions"
                            SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" ClientIDMode="Static" Enabled="true" CssClass="error" />
                    <asp:RegularExpressionValidator ID="DeliveryInstructionsRegEx" ControlToValidate="DeliveryInstructions"
                        runat="server" ValidationGroup="DeliveryDetails" Display="Static" CssClass="error" />
                    <asp:Label ID="DeliveryInsructionsHelpLabel" runat="server" CssClass="alert-box info ebiz-delivery-instructions-help" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhPurchaseOrderRow" runat="server">
        <div class="row ebiz-purchase-order">
            <div class="medium-3 columns">
                <asp:Label ID="PurchaseOrderLabel" runat="server" AssociatedControlID="PurchaseOrder" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="PurchaseOrder" runat="server" />
                <asp:RequiredFieldValidator ID="PurchaseOrderRFV" runat="server" ControlToValidate="PurchaseOrder" SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" ClientIDMode="Static" Enabled="true" CssClass="error" />
                <asp:RegularExpressionValidator ID="PurchaseOrderRegEx" ControlToValidate="PurchaseOrder" runat="server" ValidationGroup="DeliveryDetails" Display="Static" CssClass="error" />
                <asp:Label ID="PurchaseOrderHelpLabel" runat="server" CssClass="alert-box info" />
            </div>
        </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhDeliveryDayRow" runat="server">
        <div class="row ebiz-delivery-day">
            <div class="medium-3 columns">
                <asp:Label ID="DeliveryDayLabel" runat="server" />
            </div>
            <div class="medium-9 columns">
                <asp:Label ID="DeliveryDay" runat="server" />
            </div>
        </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhDeliveryDateRow" runat="server">
        <div class="row ebiz-delivery-date">
            <div class="medium-3 columns">
                <asp:Label ID="DeliveryDateLabel" runat="server" />
            </div>
            <div class="medium-9 columns">
                <asp:Label ID="DeliveryDate" runat="server" />
                <asp:Label ID="DeliveryDateHelpLabel" runat="server" CssClass="alert-box info" />
            </div>
        </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhPreferredDateRow" runat="server">
        <div class="row ebiz-preferred-date">
            <div class="medium-3 columns">
                <asp:Label ID="PreferredDateLabel" runat="server" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="PreferredDate" CssClass="datepicker" runat="server" />
                <asp:RegularExpressionValidator ID="PreferredDateRegEx" ControlToValidate="PreferredDate" runat="server" ValidationGroup="DeliveryDetails" Display="Static" CssClass="error" />
                <asp:Label ID="PreferredDateHelpLabel" runat="server" CssClass="alert-box info" />
            </div>
        </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhRetailHomeDeliveryOptions" runat="server">
            <asp:Label ID="lblDeliveryOptions" runat="server" CssClass="deliveryoptions help" />
            <asp:PlaceHolder ID="plhInstallationOption" runat="server">
            <div class="row ebiz-installation-option">
                <div class="medium-3 columns">
                    <asp:Label ID="lblInstallationOption" runat="server" AssociatedControlID="chkInstallationOption" />
                </div>
                <div class="medium-9 columns">
                    <asp:CheckBox ID="chkInstallationOption" runat="server" />
                </div>
            </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhCollectionOption" runat="server">
            <div class="row ebiz-collect-option">
                <div class="medium-3 columns">
                    <asp:Label ID="lblCollectOption" runat="server" AssociatedControlID="chkCollectOption" />
                </div>
                <div class="medium-9 columns">
                    <asp:CheckBox ID="chkCollectOption" runat="server" />
                </div>
            </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhGiftMessageRow" runat="server">
        <div class="row ebiz-check-gift-message">
            <div class="medium-12 columns">
                <asp:CheckBox ID="chkGiftMessage" runat="server" Checked="false" AutoPostBack="true" />
            </div>

         </div>
         <asp:PlaceHolder ID="plhGiftMessage" runat="server" Visible="false">
            <div class="ebiz-gift-message-wrap">
            <h2><asp:Literal ID="giftMessageInstructionsLabel" runat="server" /></h2>
                <div class="row ebiz-gift-message-to">
                    <div class="medium-3 columns">
                        <asp:Label ID="giftMessageToLabel"  runat="server" AssociatedControlID="toBox" ClientIDMode ="Static" CssClass ="inline"/>
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="toBox" runat="server" CssClass="input-l" ClientIDMode="Static"/>
                        <asp:RequiredFieldValidator ID="toRequired" runat="server" ControlToValidate="toBox" SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" ClientIDMode="Static" Enabled="true" CssClass="error" />
                        <asp:RegularExpressionValidator ID="toRegEx" ControlToValidate="toBox" runat="server" ValidationGroup="DeliveryDetails" Display="Static" CssClass="error" />
                    </div>
                </div>
                <div class="row ebiz-gift-message">
                    <div class="medium-3 columns">
                        <asp:Label ID="giftMessageLabel" runat="server" AssociatedControlID="msgBox" ClientIDMode ="Static" CssClass ="inline"/>
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="msgBox" runat="server" TextMode="MultiLine" />
                        <asp:RequiredFieldValidator ID="msgRequired" runat="server" ControlToValidate="msgBox" SetFocusOnError="true" Visible="true" ValidationGroup="DeliveryAddress" Display="Static" ClientIDMode="Static" Enabled="true" CssClass="error" />
                        <asp:RegularExpressionValidator ID="msgRegEx" ControlToValidate="msgBox" runat="server" ValidationGroup="DeliveryDetails" Display="Static" CssClass="error" />
                    </div>
                </div>
            </div>
            </asp:PlaceHolder>
        
        </asp:PlaceHolder>
    </asp:PlaceHolder>
</div>