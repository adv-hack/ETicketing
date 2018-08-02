<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PaymentDetails.ascx.vb" Inherits="UserControls_PaymentDetails" ViewStateMode="Disabled" %>
    
    <div class="row ebiz-card-type">
        <div class="medium-3 columns">
            <asp:Label ID="CardSelectionLabel" runat="server" AssociatedControlID="CardSelector" />
        </div>
        <div class="medium-4 columns">
            <asp:DropDownList ID="CardSelector" runat="server" ViewStateMode="Enabled" />
            <asp:RequiredFieldValidator ID="rfvCardType" runat="server" ControlToValidate="CardSelector" ValidationGroup="Checkout" InitialValue=" -- "  Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" EnableClientScript="true" />
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

    <div class="row ebiz-card-number">
        <div class="medium-3 columns">
            <asp:Label ID="CardNumberLabel" runat="server" AssociatedControlID="CardNumber" />
        </div>
        <div class="medium-9 columns">
            <asp:TextBox ID="CardNumber" runat="server" ViewStateMode="Enabled" autocomplete="off" type="tel" min="0" />
            <asp:RequiredFieldValidator ID="CardNumberRFV" runat="server" ValidationGroup="Checkout" ControlToValidate="CardNumber" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
            <asp:RegularExpressionValidator ID="CardNumberRegEx" runat="server" ValidationGroup="Checkout" ControlToValidate="CardNumber" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
        </div>
    </div>


    <div class="row ebiz-card-expiry-date">
        <div class="medium-3 columns">
            <asp:Label ID="ExpiryDateLabel" runat="server" AssociatedControlID="ExpiryMonth" />
        </div>
        <div class="medium-2 columns">
            <asp:DropDownList ID="ExpiryMonth" runat="server" ViewStateMode="Enabled" />
            <asp:RequiredFieldValidator ID="rfvCardValidToMM" runat="server" ControlToValidate="ExpiryMonth" ValidationGroup="Checkout" InitialValue=" -- "  Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
        </div>
        <div class="medium-2 columns end">
            <asp:DropDownList ID="ExpiryYear" runat="server" ViewStateMode="Enabled" />
            <asp:RequiredFieldValidator ID="rfvCardValidToYYYY" runat="server" ControlToValidate="ExpiryYear" ValidationGroup="Checkout" InitialValue=" -- "  Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
        </div>
    </div>

    <div class="row ebiz-card-start-date">
        <div class="medium-3 columns">
            <asp:Label ID="StartDateLabel" runat="server" AssociatedControlID="StartMonth" />
        </div>
        <div class="medium-2 columns">
            <asp:DropDownList ID="StartMonth" runat="server" ViewStateMode="Enabled" />
        </div>
        <div class="medium-2 columns end">
            <asp:DropDownList ID="StartYear" runat="server" ViewStateMode="Enabled" />
        </div>
    </div>

    <asp:PlaceHolder ID="plhIssueNumber" runat="server">
        <div class="row ebiz-issue-number">
            <div class="medium-3 columns">
                <asp:Label ID="IssueNumberLabel" runat="server" AssociatedControlID="IssueNumber" />
            </div>
            <div class="medium-2 columns end">
                <asp:TextBox ID="IssueNumber" runat="server" ViewStateMode="Enabled" type="tel" min="0" />
            </div>
        </div>
        <div class="row ebiz-issue-number-error">
            <div class="large-12 columns">
                <asp:RequiredFieldValidator ID="IssueNumberRFV" runat="server" ValidationGroup="Checkout" ControlToValidate="IssueNumber" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                <asp:RegularExpressionValidator ID="IssueNumberRegEx" runat="server" ValidationGroup="Checkout" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" ControlToValidate="IssueNumber" />
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhSecurityNumber" runat="server">
        <div class="row ebiz-security-number">
            <div class="medium-3 columns">
                <asp:Label ID="SecurityNumberLabel" runat="server" AssociatedControlID="SecurityNumber" />
            </div>
            <div class="medium-2 columns">
                <asp:TextBox ID="SecurityNumber" runat="server" ViewStateMode="Enabled" MaxLength="4" autocomplete="off" type="tel" min="0" />
            </div>
            <div class="medium-7 columns">
                <asp:Image ID="SecurityImage" SkinID="CV2" runat="server" />
            </div>
        </div>
        <div class="row ebiz-card-csv-error">
            <div class="large-12 columns">
                <asp:RequiredFieldValidator ID="SecurityNumberRFV" runat="server" ValidationGroup="Checkout" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" ControlToValidate="SecurityNumber" />
                <asp:RegularExpressionValidator ID="SecurityNumberRegEx" runat="server" ValidationGroup="Checkout" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" ControlToValidate="SecurityNumber" />
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhCardHolderName" runat="server">
        <div class="row ebiz-card-holder-name">
            <div class="medium-3 columns">
                <asp:Label ID="CardHolderNameLabel" runat="server" AssociatedControlID="CardHolderName" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="CardHolderName" runat="server" ViewStateMode="Enabled" MaxLength="50" />
                <asp:RequiredFieldValidator ID="CardHolderNameRFV" runat="server" ValidationGroup="Checkout" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" ControlToValidate="CardHolderName" />
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhSetAsDefault" runat="server">
        <div class="row ebiz-set-as-default">
            <div class="medium-3 columns">
                <asp:Checkbox ID="chkSetAsDefault" runat="server" ViewStateMode="Enabled" />
                <asp:Label ID="lblSetAsDefault" runat="server" AssociatedControlID="chkSetAsDefault" />
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhInstallments" runat="server">
        <asp:PlaceHolder ID="plhInstallmentsIntro" runat="server">
            <p class="ebiz-installments-intro"><asp:Literal ID="ltlInstallmentsIntro" runat="server" /></p>
        </asp:PlaceHolder>

        <div class="row installments text">
            <div class="medium-3 columns">
                <asp:Label ID="InstallmentsLabel" runat="server" AssociatedControlID="txtInstallments" />
            </div>
            <div class="medium-2 columns">
                <asp:TextBox ID="txtInstallments" runat="server" ViewStateMode="Enabled" MaxLength="2" />
            </div>
            <div class="medium-7 columns">
                <asp:DropDownList ID="ddlInstallments" runat="server" />
                <asp:RequiredFieldValidator ID="rfvInstallments" runat="server" ControlToValidate="ddlInstallments" ValidationGroup="Checkout" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
            </div>
        </div>  
        <asp:Label ID="lblInstallments" runat="server" CssClass="alert-box alert" />
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhSaveTheseCardDetails" runat="server" Visible="false">
        <p class="ebiz-save-these-card-details">
            <asp:CheckBox ID="chkSaveTheseCardDetails" runat="server" ViewStateMode="Enabled" />
            <asp:Label ID="lblSaveTheseCardDetails" runat="server" AssociatedControlID="chkSaveTheseCardDetails" />
        </p>
    </asp:PlaceHolder>
