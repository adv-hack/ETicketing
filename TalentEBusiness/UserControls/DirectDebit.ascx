<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DirectDebit.ascx.vb" Inherits="UserControls_DirectDebit" %>

<asp:PlaceHolder ID="plhDirectDebit" runat="server" Visible="false">
    <div class="row ebiz-dd-account-name">
        <div class="medium-3 columns">
            <asp:Label ID="AccountNameLabel" runat="server" AssociatedControlID="AccountName" />
        </div>
        <div class="medium-9 columns">
            <asp:TextBox ID="AccountName" runat="server" />
            <asp:RequiredFieldValidator ID="AccountNameRFV" runat="server"  Display="Static" CssClass="error" SetFocusOnError="true"
                ValidationGroup="Checkout" ControlToValidate="AccountName" />
            <asp:RegularExpressionValidator ID="AccountNameRegEx" runat="server"  Display="Static" CssClass="error" SetFocusOnError="true"
                ValidationGroup="Checkout" ControlToValidate="AccountName" />
        </div>
    </div>
    <div class="row ebiz-dd-sort-code">
        <div class="medium-3 columns">
            <asp:Label ID="SortCodeLabel" runat="server" AssociatedControlID="SortCode1" />
        </div>
        <div class="small-4 medium-3 large-1 columns">
            <asp:TextBox ID="SortCode1" runat="server" MaxLength="2" CssClass="sort-code-1" />
        </div>
        <div class="small-4 medium-3 large-1 columns">
            <asp:TextBox ID="SortCode2" runat="server" MaxLength="2" CssClass="sort-code-2" />
        </div>
        <div class="small-4 medium-3 large-1 end columns">
            <asp:TextBox ID="SortCode3" runat="server" MaxLength="2" CssClass="sort-code-3" />
        </div>
    </div>
    <div class="ebiz-dd-sort-code-error">
        <asp:RegularExpressionValidator ID="SortCode1RegEx" runat="server"  Display="Static" CssClass="error" SetFocusOnError="true"
            ValidationGroup="Checkout" ControlToValidate="SortCode1" />
        <asp:RegularExpressionValidator ID="SortCode2RegEx" runat="server"  Display="Static" CssClass="error" SetFocusOnError="true"
            ValidationGroup="Checkout" ControlToValidate="SortCode2" />
        <asp:RegularExpressionValidator ID="SortCode3RegEx" runat="server"  Display="Static" CssClass="error" SetFocusOnError="true"
            ValidationGroup="Checkout" ControlToValidate="SortCode3" />
        <asp:CustomValidator ID="csvSortCode" runat="server" ValidationGroup="Checkout" SetFocusOnError="true" Display="Static" CssClass="error"
            OnServerValidate="ValidateSortCode" ClientValidationFunction="ValidateSortCode" EnableClientScript="true" />
    </div>
    <div class="row ebiz-dd-account-number">
        <div class="medium-3 columns">
            <asp:Label ID="AccountNumberLabel" runat="server" AssociatedControlID="AccountNumber" />
        </div>
        <div class="medium-9 columns">
            <asp:TextBox ID="AccountNumber" runat="server" MaxLength="8" />
            <asp:RequiredFieldValidator ID="AccountNumberRFV" runat="server"  Display="Static" CssClass="error" SetFocusOnError="true"
                ValidationGroup="Checkout" ControlToValidate="AccountNumber" />
            <asp:RegularExpressionValidator ID="AccountNumberRegEx" runat="server"  Display="Static" CssClass="error" SetFocusOnError="true"
                ValidationGroup="Checkout" ControlToValidate="AccountNumber" />
        </div>
    </div>
    <asp:PlaceHolder ID="plhPaymentDayDDL" runat="server" Visible="false">
    <div class="row ebiz-dd-payment-days">
        <div class="medium-3 columns">
            <asp:Label ID="PaymentDayLabel" runat="server" AssociatedControlID="PaymentDayDDL" />
        </div>
        <div class="medium-9 columns">
            <asp:DropDownList ID="PaymentDayDDL" runat="server" />
        </div>
    </div>
    </asp:PlaceHolder>
</asp:PlaceHolder>