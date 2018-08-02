<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PaymentExternal.ascx.vb"
    Inherits="UserControls_PaymentExternal" %>
<div id="payment-external" class="default-form">
<h2><asp:Label ID="TitleLabel" runat="server"></asp:Label></h2>
    <asp:PlaceHolder ID="plhPayPal" runat="server">
        <div id="divPayPal" class="paypal">
            <asp:ImageButton ID="imgBtnPayPal" runat="server" CssClass="button-paypal" AlternateText="PayPal CheckOut" />
        </div>
    </asp:PlaceHolder>
</div>
