<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CheckoutPartPayments.ascx.vb" Inherits="UserControls_CheckoutPartPayments" %>

<asp:PlaceHolder ID="plhPartPmt" runat="server" Visible="true">
    <div class="row ebiz-part-payment">  
        <div class="medium-3 columns">
            <asp:Label ID="lblPartPmt" AssociatedControlID="txtPartPmt"  runat="server" />
        </div>
        <div class="medium-9 columns">
            <asp:TextBox ID="txtPartPmt" runat="server" MaxLength="30" />
            <asp:RequiredFieldValidator ID="rfvPaymentAmount" runat="server"  Display="Static" CssClass="error" SetFocusOnError="true" ValidationGroup="Checkout" OnLoad="SetRequiredValidator" ControlToValidate="txtPartPmt" />
            <asp:RegularExpressionValidator ID="PartPaymentRegEx" runat="server" Display="Static" OnLoad="SetValidators" ControlToValidate="txtPartPmt" CssClass="error" ValidationGroup="Checkout"></asp:RegularExpressionValidator>
        </div>
    </div>
</asp:PlaceHolder>
