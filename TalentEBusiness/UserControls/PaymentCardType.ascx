<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PaymentCardType.ascx.vb" Inherits="UserControls_PaymentCardType" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/CheckoutPartPayments.ascx" TagName="CheckoutPartPayments" TagPrefix="Talent" %>

<div class="row ebiz-card-type">
    <div class="medium-3 columns">
        <asp:Label ID="lblCardSelection" runat="server" AssociatedControlID="ddlCardType"  />
    </div>
    <div class="medium-4 columns">
        <asp:DropDownList ID="ddlCardType" runat="server" AutoPostBack="true" ViewStateMode="Enabled" />
        <asp:RequiredFieldValidator ID="rfvCardType" runat="server" ControlToValidate="ddlCardType" ValidationGroup="Checkout" InitialValue=" -- " Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" EnableClientScript="true" />
    </div>
    <div class="medium-5 columns">
        <asp:Image ID="imgMaestroCard" runat="server" SkinID="MaestroCard" />
        <asp:Image ID="imgMasterCard" runat="server" SkinID="MasterCard" />
        <asp:Image ID="imgVisaCard" runat="server" SkinID="VisaCard" />
        <asp:Image ID="imgVisaDebitCard" runat="server" SkinID="VisaDebitCard" />
        <asp:Image ID="imgVisaElectronCard" runat="server" SkinID="VisaElectronCard" />
        <asp:Image ID="imgAmexCard" runat="server" SkinID="AmexCard" />
    </div>
</div>
<asp:PlaceHolder ID="plhCaptureMethod" runat="server" Visible="true">
    <div class="row ebiz-capture-method">
        <div class="medium-3 columns">
            <asp:Label ID="lblCaptureMethod" runat="server" AssociatedControlID="ddlCaptureMethod"  />
        </div>
        <div class="medium-9 columns">
            <asp:DropDownList ID="ddlCaptureMethod" runat="server" AutoPostBack="false" ViewStateMode="Enabled" />
        </div>
        
    </div>
</asp:PlaceHolder>
<Talent:CheckoutPartPayments ID="uscCCPartPayment" runat="server" ViewStateMode="Enabled" />









