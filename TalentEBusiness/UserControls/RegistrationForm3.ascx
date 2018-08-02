<%@ Control Language="VB" AutoEventWireup="false" CodeFile="RegistrationForm3.ascx.vb"
    Inherits="UserControls_RegistrationForm3" %>
<asp:Label ID="RegistrationHeaderLabel" runat="server" CssClass="titleLabel"></asp:Label>
<p class="instructions">
    <asp:Label ID="RegistrationInstructionsLabel" runat="server" CssClass="instructionsLabel"></asp:Label></p>
<p class="error">
    <asp:Label ID="ErrorLabel" runat="server" CssClass="error"></asp:Label>&nbsp;</p>
<p class="error">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList"
        ValidationGroup="Registration1" />
    <asp:ValidationSummary ID="ValidationSummary2" runat="server" DisplayMode="BulletList"
        ValidationGroup="Registration2" />
    <asp:ValidationSummary ID="ValidationSummary3" runat="server" DisplayMode="BulletList"
        ValidationGroup="Registration3" />
    <asp:ValidationSummary ID="ValidationSummary4" runat="server" DisplayMode="BulletList"
        ValidationGroup="Registration4" />
</p>
<div id="registration">

    <asp:Panel ID="Panel1" runat="server" CssClass="registration-section-one">
        <div id="CompanyDtlsBox" class="CompanyDtlsBox box default-form" runat="server">
            <div id="CompanyDtlsBoxTitle" runat="server">
                <h2>
                    <asp:Label ID="companyDtlsTitleLabel" runat="server"></asp:Label>
                </h2>
            </div>
            <div id="companyNameRow" class="companyNameRow" runat="server">
                <asp:Label ID="companyNameLabel" runat="server" AssociatedControlID="companyName"></asp:Label>
                <asp:TextBox ID="companyName" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="companyName" ID="companyNameRFV" runat="server"
                    OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                    Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="companyName" ID="companyNameRegEx"
                    runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="deliveryAddDtlsBox" class="deliveryAddDtlsBox box default-form" runat="server">
                <asp:Panel ID="pnlDeliveryAddress" runat="server">
                    <div id="deliveryAddressTitle" runat="server">
                        <asp:Label ID="deliveryAddressTitleLabel" runat="server"></asp:Label>
                    </div>
                    <div id="add1Row" class="add1Row" runat="server">
                        <asp:Label ID="add1Label" runat="server" AssociatedControlID="add1"></asp:Label>
                        <asp:TextBox ID="add1" CssClass="input-l" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="add1" ID="add1RFV" runat="server"
                            OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                            Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ControlToValidate="add1" ID="add1RegEx" runat="server"
                            OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                            Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                    </div>
                    <div id="add2Row" class="add2Row" runat="server">
                        <asp:Label ID="add2Label" runat="server" AssociatedControlID="add2"></asp:Label>
                        <asp:TextBox ID="add2" CssClass="input-l" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="add2" ID="add2RFV" runat="server"
                            OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                            Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ControlToValidate="add2" ID="add2RegEx" runat="server"
                            OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                            Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                    </div>
                    <div id="add3Row" class="add3Row" runat="server">
                        <asp:Label ID="add3Label" runat="server" AssociatedControlID="add3"></asp:Label>
                        <asp:TextBox ID="add3" CssClass="input-l" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="add3" ID="add3RFV" runat="server"
                            OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                            Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ControlToValidate="add3" ID="add3RegEx" runat="server"
                            OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                            Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                    </div>
                    <div id="add4Row" class="add4Row" runat="server">
                        <asp:Label ID="add4Label" runat="server" AssociatedControlID="add4"></asp:Label>
                        <asp:TextBox ID="add4" CssClass="input-l" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="add4" ID="add4RFV" runat="server"
                            OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                            Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ControlToValidate="add4" ID="add4RegEx" runat="server"
                            OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                            Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                    </div>
                    <div id="add5Row" class="add5Row" runat="server">
                        <asp:Label ID="add5Label" runat="server" AssociatedControlID="add5"></asp:Label>
                        <asp:TextBox ID="add5" CssClass="input-l" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="add5" ID="add5RFV" runat="server"
                            OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                            Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ControlToValidate="add5" ID="add5RegEx" runat="server"
                            OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                            Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                    </div>
                    <div id="postcodeRow" class="postcodeRow" runat="server">
                        <asp:Label ID="postcodeLabel" runat="server" AssociatedControlID="postcode"></asp:Label>
                        <asp:TextBox ID="postcode" CssClass="input-m" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="postcode" ID="postcodeRFV" runat="server"
                            OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                            Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ControlToValidate="postcode" ID="postcodeRegEx" runat="server"
                            OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                            Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                    </div>
                    <div id="countryRow" class="countryRow" runat="server">
                        <asp:Label ID="countryLabel" runat="server" AssociatedControlID="country"></asp:Label>
                        <asp:DropDownList ID="country" CssClass="select" runat="server">
                        </asp:DropDownList>
                        <asp:RegularExpressionValidator ControlToValidate="country" ID="countryRegEx" runat="server"
                            OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                            Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                    </div>
                </asp:Panel>
            </div>
            <div id="phoneRow" class="phoneRow" runat="server">
                <asp:Label ID="phoneLabel" runat="server" AssociatedControlID="phone"></asp:Label>
                <asp:TextBox ID="phone" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="phone" ID="phoneRFV" runat="server"
                    OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                    Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="phone" ID="phoneRegEx" runat="server"
                    OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                    Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="faxRow" class="faxRow" runat="server">
                <asp:Label ID="faxLabel" runat="server" AssociatedControlID="fax"></asp:Label>
                <asp:TextBox ID="fax" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="fax" ID="faxRFV" runat="server" OnInit="SetupRequiredValidator"
                    SetFocusOnError="true" Visible="true" ValidationGroup="Registration1" Display="Static"
                    Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="fax" ID="faxRegEx" runat="server"
                    OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                    Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="emailRow" class="emailRow" runat="server">
                <asp:Label ID="emailLabel" runat="server" AssociatedControlID="email"></asp:Label>
                <asp:TextBox ID="email" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="email" ID="emailRFV" runat="server"
                    OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                    Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="email" ID="emailRegEx" runat="server"
                    OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                    Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="emailCompareRow" class="emailRow" runat="server">
                <asp:Label ID="emailConfirmLabel" runat="server" AssociatedControlID="emailConfirm"></asp:Label>
                <asp:TextBox ID="emailConfirm" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:CompareValidator ID="emailConfirmCompare" runat="server" OnInit="SetupCompareValidator"
                    ControlToValidate="emailConfirm" ControlToCompare="email" SetFocusOnError="true"
                    Visible="true" ValidationGroup="Registration1" Display="Static" Enabled="true"
                    Text="*"></asp:CompareValidator>
                <asp:RequiredFieldValidator ControlToValidate="emailConfirm" ID="emailConfirmRFV"
                    runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            </div>
            <div id="openingTimesRow" class="openingTimesRow" runat="server">
                <asp:Label ID="openingTimesLabel" runat="server" AssociatedControlID="openingTimes"></asp:Label>
                <asp:TextBox ID="openingTimes" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="openingTimes" ID="openingTimesRFV"
                    runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="openingTimes" ID="openingTimesRegEx"
                    runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="estAnnExpRow" class="estAnnExpRow" runat="server">
                <asp:Label ID="estAnnExpLabel" runat="server" AssociatedControlID="estAnnExp"></asp:Label>
                <asp:TextBox ID="estAnnExp" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="estAnnExp" ID="estAnnExpRFV" runat="server"
                    OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                    Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="estAnnExp" ID="estAnnExpRegEx"
                    runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="credLimRow" class="credLimRow" runat="server">
                <asp:Label ID="credLimLabel" runat="server" AssociatedControlID="credLim"></asp:Label>
                <asp:TextBox ID="credLim" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="credLim" ID="credLimRFV" runat="server"
                    OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                    Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="credLim" ID="credLimRegEx" runat="server"
                    OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                    Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="dlvryInstRow" class="dlvryInstRow" runat="server">
                <asp:Label ID="dlvryInstLabel" runat="server" AssociatedControlID="dlvryInst"></asp:Label>
                <asp:TextBox ID="dlvryInst" CssClass="input-l" runat="server" TextMode="MultiLine"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="dlvryInst" ID="dlvryInstRFV" runat="server"
                    OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                    Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="dlvryInst" ID="dlvryInstRegEx"
                    runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="partNCCRow" class="partNCCRow" runat="server">
                <asp:Label ID="partNCCLabel" runat="server" AssociatedControlID="partNCC"></asp:Label>
                <asp:CheckBox ID="partNCC" runat="server" />
            </div>
        </div>
        <div id="expCodeBox" class="expCodeBox box default-form" runat="server">
            <div id="expCodeBoxTitle" runat="server">
                <asp:Label ID="expCodeLabel" runat="server"></asp:Label>
            </div>
            <div id="costCenRow" class="costCenRow" runat="server">
                <asp:Label ID="costCenLabel" runat="server" AssociatedControlID="costCen"></asp:Label>
                <asp:TextBox ID="costCen" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="costCen" ID="costCenRFV" runat="server"
                    OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                    Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="costCen" ID="costCenRegEx" runat="server"
                    OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration1"
                    Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
        </div>
        <div id="contactDtlsBox" class="contactDtlsBox box default-form" runat="server">
            <div id="contactDtlsBoxTitle" runat="server">
                <asp:Label ID="contactDtlsTitleLabel" runat="server"></asp:Label>
            </div>
            <div id="mainContactNameRow" class="mainContactNameRow" runat="server">
                <asp:Label ID="mainContactNameLabel" runat="server" AssociatedControlID="mainContactName"></asp:Label>
                <asp:TextBox ID="mainContactName" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="mainContactName" ID="mainContactNameRFV"
                    runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="mainContactName" ID="mainContactNameRegEx"
                    runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="mainContactPosRow" class="mainContactPosRow" runat="server">
                <asp:Label ID="mainContactPosLabel" runat="server" AssociatedControlID="mainContactPos"></asp:Label>
                <asp:TextBox ID="mainContactPos" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="mainContactPos" ID="mainContactPosRFV"
                    runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="mainContactPos" ID="mainContactPosRegEx"
                    runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="mainContactTelRow" class="mainContactTel" runat="server">
                <asp:Label ID="mainContactTelLabel" runat="server" AssociatedControlID="mainContactTel"></asp:Label>
                <asp:TextBox ID="mainContactTel" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="mainContactTel" ID="mainContactTelRFV"
                    runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="mainContactTel" ID="mainContactTelRegEx"
                    runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="mainContactExtRow" class="mainContactExtRow" runat="server">
                <asp:Label ID="mainContactExtLabel" runat="server" AssociatedControlID="mainContactExt"></asp:Label>
                <asp:TextBox ID="mainContactExt" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="mainContactExt" ID="mainContactExtRFV"
                    runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="mainContactExt" ID="mainContactExtRegEx"
                    runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="scndContactNameRow" class="scndContactNameRow" runat="server">
                <asp:Label ID="scndContactNameLabel" runat="server" AssociatedControlID="scndContactName"></asp:Label>
                <asp:TextBox ID="scndContactName" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="scndContactName" ID="scndContactNameRFV"
                    runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="mainContactName" ID="scndContactNameRegEx"
                    runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="scndContactPosRow" class="scndContactPosRow" runat="server">
                <asp:Label ID="scndContactPosLabel" runat="server" AssociatedControlID="scndContactPos"></asp:Label>
                <asp:TextBox ID="scndContactPos" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="scndContactPos" ID="scndContactPosRFV"
                    runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="scndContactPos" ID="scndContactPosRegEx"
                    runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="scndContactTelRow" class="scndContactTel" runat="server">
                <asp:Label ID="scndContactTelLabel" runat="server" AssociatedControlID="scndContactTel"></asp:Label>
                <asp:TextBox ID="scndContactTel" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="scndContactTel" ID="scndContactTelRFV"
                    runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="scndContactTel" ID="scndContactTelRegEx"
                    runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="scndContactExtRow" class="scndContactExtRow" runat="server">
                <asp:Label ID="scndContactExtLabel" runat="server" AssociatedControlID="scndContactExt"></asp:Label>
                <asp:TextBox ID="scndContactExt" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="scndContactExt" ID="scndContactExtRFV"
                    runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="scndContactExt" ID="scndContactExtRegEx"
                    runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Registration1" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
        </div>
        <div class="next-prev-section">
            <asp:Button ID="nextButton1" CssClass="button" runat="server" CausesValidation="true"
                ValidationGroup="Registration1" />
        </div>
    </asp:Panel>
    
    <asp:Panel ID="Panel2" runat="server" CssClass="registration-section-two">
        <div class="bank-section">
            <h2>
                <asp:Label ID="bankTitleLabel" runat="server"></asp:Label>
            </h2>
            <div class="section-fields">
                <div id="nameOfBankRow" class="nameOfBankRow" runat="server">
                    <asp:Label ID="nameOfBankLabel" runat="server" AssociatedControlID="nameOfBankBox"></asp:Label>
                    <asp:TextBox ID="nameOfBankBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="nameOfBankBox" ID="nameOfBankRFV"
                        runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                        ValidationGroup="Registration2" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="nameOfBankBox" ID="nameOfBankRegEx"
                        runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                        ValidationGroup="Registration2" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="addressOfBankRow" class="addressOfBankRow" runat="server">
                    <asp:Label ID="addressOfBankLabel" runat="server" AssociatedControlID="addressOfBankBox"></asp:Label>
                    <asp:TextBox ID="addressOfBankBox" CssClass="input-l" runat="server" TextMode="MultiLine"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="addressOfBankBox" ID="addressOfBankRFV"
                        runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                        ValidationGroup="Registration2" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="addressOfBankBox" ID="addressOfBankRegEx"
                        runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                        ValidationGroup="Registration2" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="bankPostcodeRow" class="bankPostcodeRow" runat="server">
                    <asp:Label ID="bankPostcodeLabel" runat="server" AssociatedControlID="bankPostcodeBox"></asp:Label>
                    <asp:TextBox ID="bankPostcodeBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="bankPostcodeBox" ID="bankPostcodeRFV" runat="server"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration2"
                        Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="bankPostcodeBox" ID="bankPostcodeRegEx" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration2"
                        Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="accNameRow" class="accNameRow" runat="server">
                    <asp:Label ID="accNameLabel" runat="server" AssociatedControlID="accNameBox"></asp:Label>
                    <asp:TextBox ID="accNameBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="accNameBox" ID="accNameRFV" runat="server"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration2"
                        Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="accNameBox" ID="accNameRegEx" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration2"
                        Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="sortCodeRow" class="sortCodeRow" runat="server">
                    <asp:Label ID="sortCodeLabel" runat="server" AssociatedControlID="sortCodeBox"></asp:Label>
                    <asp:TextBox ID="sortCodeBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="sortCodeBox" ID="sortCodeRFV" runat="server"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration2"
                        Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="sortCodeBox" ID="sortCodeRegEx" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration2"
                        Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="accNoRow" class="accNoRow" runat="server">
                    <asp:Label ID="accNoLabel" runat="server" AssociatedControlID="accNoBox"></asp:Label>
                    <asp:TextBox ID="accNoBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="accNoBox" ID="accNoRFV" runat="server"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration2"
                        Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="accNoBox" ID="accNoRegEx" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration2"
                        Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="bankCheckRow" class="bankCheckRow" runat="server">
                    <asp:CheckBox ID="bankCheckAgree" runat="server" />
                </div>
            </div>
        </div>
        <div class="next-prev-section">
            <asp:Button ID="prevButton2" CssClass="button" runat="server" />
            <p class="next-prev-spacer"></p>
            <asp:Button ID="nextButton2" CssClass="button" runat="server" CausesValidation="true"
                ValidationGroup="Registration2" />
        </div>
    </asp:Panel>
    
    <asp:Panel ID="Panel3" runat="server" CssClass="registration-section-three">
        <div class="trade-section">
            <h2>
                <asp:Label ID="tradeTitleLabel" runat="server"></asp:Label>
            </h2>
            <div class="section-fields">
                <div id="supplier1NameRow" class="supplier1NameRow" runat="server">
                    <asp:Label ID="supplier1NameLabel" runat="server" AssociatedControlID="supplier1NameBox"></asp:Label>
                    <asp:TextBox ID="supplier1NameBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="supplier1NameBox" ID="supplier1NameBoxRFV"
                        runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                        ValidationGroup="Registration3" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="supplier1NameBox" ID="supplier1NameRegEx"
                        runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                        ValidationGroup="Registration3" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="supplier1AddressRow" class="supplier1AddressRow" runat="server">
                    <asp:Label ID="supplier1AddressLabel" runat="server" AssociatedControlID="supplier1AddressBox"></asp:Label>
                    <asp:TextBox ID="supplier1AddressBox" CssClass="input-l" runat="server" TextMode="MultiLine"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="supplier1AddressBox" ID="supplier1AddressRFV"
                        runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                        ValidationGroup="Registration3" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="supplier1AddressBox" ID="supplier1AddressRegEx"
                        runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                        ValidationGroup="Registration3" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="supplier1PostcodeRow" class="supplier1PostcodeRow" runat="server">
                    <asp:Label ID="supplier1PostcodeLabel" runat="server" AssociatedControlID="supplier1PostcodeBox"></asp:Label>
                    <asp:TextBox ID="supplier1PostcodeBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="supplier1PostcodeBox" ID="supplier1PostcodeRFV" runat="server"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="supplier1PostcodeBox" ID="supplier1PostcodeRegEx" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="supplier1TelRow" class="supplier1TelRow" runat="server">
                    <asp:Label ID="supplier1TelLabel" runat="server" AssociatedControlID="supplier1TelBox"></asp:Label>
                    <asp:TextBox ID="supplier1TelBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="supplier1TelBox" ID="supplier1TelRFV" runat="server"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="supplier1TelBox" ID="supplier1TelRegEx" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="supplier1FaxRow" class="supplier1FaxRow" runat="server">
                    <asp:Label ID="supplier1FaxLabel" runat="server" AssociatedControlID="supplier1FaxBox"></asp:Label>
                    <asp:TextBox ID="supplier1FaxBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="supplier1FaxBox" ID="supplier1FaxRFV" runat="server"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="supplier1FaxBox" ID="supplier1FaxRegEx" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="supplier1AccNoRow" class="supplier1AccNoRow" runat="server">
                    <asp:Label ID="supplier1AccNoLabel" runat="server" AssociatedControlID="supplier1AccNoBox"></asp:Label>
                    <asp:TextBox ID="supplier1AccNoBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="supplier1AccNoBox" ID="supplier1AccNoRFV" runat="server"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="supplier1AccNoBox" ID="supplier1AccNoRegEx" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="supplier2NameRow" class="supplier2NameRow" runat="server">
                    <asp:Label ID="supplier2NameLabel" runat="server" AssociatedControlID="supplier2NameBox"></asp:Label>
                    <asp:TextBox ID="supplier2NameBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="supplier2NameBox" ID="supplier2NameBoxRFV"
                        runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                        ValidationGroup="Registration3" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="supplier2NameBox" ID="supplier2NameRegEx"
                        runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                        ValidationGroup="Registration3" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="supplier2AddressRow" class="supplier2AddressRow" runat="server">
                    <asp:Label ID="supplier2AddressLabel" runat="server" AssociatedControlID="supplier2AddressBox"></asp:Label>
                    <asp:TextBox ID="supplier2AddressBox" CssClass="input-l" runat="server" TextMode="MultiLine"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="supplier2AddressBox" ID="supplier2AddressRFV"
                        runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                        ValidationGroup="Registration3" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="supplier2AddressBox" ID="supplier2AddressRegEx"
                        runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                        ValidationGroup="Registration3" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="supplier2PostcodeRow" class="supplier2PostcodeRow" runat="server">
                    <asp:Label ID="supplier2PostcodeLabel" runat="server" AssociatedControlID="supplier2PostcodeBox"></asp:Label>
                    <asp:TextBox ID="supplier2PostcodeBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="supplier2PostcodeBox" ID="supplier2PostcodeRFV" runat="server"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="supplier2PostcodeBox" ID="supplier2PostcodeRegEx" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="supplier2TelRow" class="supplier2TelRow" runat="server">
                    <asp:Label ID="supplier2TelLabel" runat="server" AssociatedControlID="supplier2TelBox"></asp:Label>
                    <asp:TextBox ID="supplier2TelBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="supplier2TelBox" ID="supplier2TelRFV" runat="server"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="supplier2TelBox" ID="supplier2TelRegEx" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="supplier2FaxRow" class="supplier2FaxRow" runat="server">
                    <asp:Label ID="supplier2FaxLabel" runat="server" AssociatedControlID="supplier2FaxBox"></asp:Label>
                    <asp:TextBox ID="supplier2FaxBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="supplier2FaxBox" ID="supplier2FaxRFV" runat="server"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="supplier2FaxBox" ID="supplier2FaxRegEx" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="supplier2AccNoRow" class="supplier2AccNoRow" runat="server">
                    <asp:Label ID="supplier2AccNoLabel" runat="server" AssociatedControlID="supplier2AccNoBox"></asp:Label>
                    <asp:TextBox ID="supplier2AccNoBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="supplier2AccNoBox" ID="supplier2AccNoRFV" runat="server"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="supplier2AccNoBox" ID="supplier2AccNoRegEx" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration3"
                        Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
            </div>
        </div>
        <div class="next-prev-section">
            <asp:Button ID="prevButton3" CssClass="button" runat="server" />
            <p class="next-prev-spacer"></p>
            <asp:Button ID="nextButton3" CssClass="button" runat="server" CausesValidation="true"
                ValidationGroup="Registration3" />
        </div>
    </asp:Panel>
    
    <asp:Panel ID="Panel4" runat="server" CssClass="registration-section-four">
        <div class="org-section">
            <h2>
                <asp:Label ID="orgTitleLabel" runat="server"></asp:Label>
            </h2>
            <div class="section-fields">
                <div id="q1Wrap" class="q1Wrap" runat="server">
                    <div id="q1Row1" class="q1Row" runat="server">
                        <asp:Label ID="q1Label" runat="server" CssClass="reg-question"></asp:Label>
                    </div>
                    <div id="q1Row2" class="q1Row" runat="server">
                        <asp:RadioButtonList ID="q1RadioList" runat="server">
                        </asp:RadioButtonList>
                    </div>
                    <div id="q1Row7" class="q1Row" runat="server">
                        <asp:Label ID="q1CharityLabel" runat="server" AssociatedControlID="q1CharityBox"></asp:Label>
                        <asp:TextBox ID="q1CharityBox" CssClass="input-l" runat="server"></asp:TextBox>
                        <asp:RegularExpressionValidator ControlToValidate="q1CharityBox" ID="q1CharityRegEx" runat="server"
                            OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration4"
                            Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                    </div>
                </div>
                <div id="q2Wrap" class="q2Wrap" runat="server">
                    <div id="q2Row1" class="q2Row" runat="server">
                        <asp:Label ID="q2Label" runat="server" CssClass="reg-question"></asp:Label>
                    </div>
                    <div id="q2Row2" class="q2Row" runat="server">
                        <asp:RadioButtonList ID="q2RadioList" runat="server">
                        </asp:RadioButtonList>
                    </div>
                </div>
                <div id="q3Wrap" class="q3Wrap" runat="server">
                    <div id="q3Row1" class="q3Row" runat="server">
                        <asp:Label ID="q3Label" runat="server" CssClass="reg-question"></asp:Label>
                    </div>
                    <div id="q3Row2" class="q3Row" runat="server">
                        <asp:CheckBoxList ID="q3CheckBoxList" runat="server">
                        </asp:CheckBoxList>
                    </div>
                </div>
                <div id="q4Wrap" class="q4Wrap" runat="server">
                    <div id="q4Row1" class="q4Row1" runat="server">
                        <asp:Label ID="q4Label" runat="server" CssClass="reg-question"></asp:Label>
                    </div>
                    <div id="q4OrgWorkRow" class="q4OrgWorkRow" runat="server">
                        <asp:Label ID="q4OrgWorkLabel" runat="server" AssociatedControlID="q4OrgWorkBox"></asp:Label>
                        <asp:TextBox ID="q4OrgWorkBox" CssClass="input-l" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="q4OrgWorkBox" ID="q4OrgWorkRFV" runat="server"
                            OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration4"
                            Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ControlToValidate="q4OrgWorkBox" ID="q4OrgWorkRegEx" runat="server"
                            OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration4"
                            Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                    </div>
                    <div id="q4HearAboutUsRow" class="q4HearAboutUsRow" runat="server">
                        <asp:Label ID="q4HearAboutUsLabel" runat="server" AssociatedControlID="q4HearAboutUsBox"></asp:Label>
                        <asp:TextBox ID="q4HearAboutUsBox" CssClass="input-l" runat="server"></asp:TextBox>
                        <asp:RequiredFieldValidator ControlToValidate="q4HearAboutUsBox" ID="q4HearAboutUsRFV" runat="server"
                            OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration4"
                            Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ControlToValidate="q4HearAboutUsBox" ID="q4HearAboutUsRegEx" runat="server"
                            OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration4"
                            Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                    </div>
                </div>
                <div id="appNameRow" class="appNameRow" runat="server">
                    <asp:Label ID="appNameLabel" runat="server" AssociatedControlID="appNameBox"></asp:Label>
                    <asp:TextBox ID="appNameBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="appNameBox" ID="appNameRFV" runat="server"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration4"
                        Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="appNameBox" ID="appNameRegEx" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration4"
                        Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="appPosRow" class="appPosRow" runat="server">
                    <asp:Label ID="appPosLabel" runat="server" AssociatedControlID="appPosBox"></asp:Label>
                    <asp:TextBox ID="appPosBox" CssClass="input-l" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ControlToValidate="appPosBox" ID="appPosRFV" runat="server"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration4"
                        Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ControlToValidate="appPosBox" ID="appPosRegEx" runat="server"
                        OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration4"
                        Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
                </div>
                <div id="q4Row2" class="q4Row2" runat="server">
                    <asp:CheckBox ID="q4CatalogueCheck" runat="server" />
                </div>
                <div id="tsandcsRow" class="tsandcsRow" runat="server">
                    <asp:CheckBox ID="tsandcsCheck" runat="server" />
                </div>
            </div>
        </div>
        <div class="next-prev-section registerBtn">
            <asp:Button ID="prevButton4" CssClass="button" runat="server" />
            <p class="next-prev-spacer"></p>
            <asp:Button ID="registerBtn" CssClass="button" runat="server" CausesValidation="true"
            ValidationGroup="Registration4" />
        </div>
    </asp:Panel>

    
   
</div>
