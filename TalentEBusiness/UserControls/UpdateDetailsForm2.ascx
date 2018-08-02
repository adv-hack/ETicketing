<%@ Control Language="VB" AutoEventWireup="false" CodeFile="UpdateDetailsForm2.ascx.vb"
    Inherits="UserControls_UpdateDetailsForm2" %>
<%@ Register Src="ChangePassword.ascx" TagName="ChangePassword" TagPrefix="Talent" %>
<asp:Label ID="RegistrationHeaderLabel" runat="server" CssClass="titleLabel"></asp:Label>
<p class="instructions">
    <asp:Label ID="RegistrationInstructionsLabel" runat="server" CssClass="instructionsLabel"></asp:Label>
      <asp:Label ID="lblFromActivationNewContact" runat="server" CssClass="instructionsLabel"></asp:Label></p>
<p class="error">
    <asp:Label ID="ErrorLabel" runat="server" CssClass="error"></asp:Label>&nbsp;</p>
<p class="error">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList"
        ValidationGroup="Registration" />
</p>
<div id="registration">
    <div id="PersonalDetailsBox" class="PersonalDetailsBox box default-form" runat="server"  oninit="SetupDisplayType">
        <h2>
            <asp:Label ID="PersonalDetailsLabel" runat="server"></asp:Label>
        </h2>
        <div id="titleRow" class="titleRow" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="titleLabel" runat="server" AssociatedControlID="title"></asp:Label>
            <asp:DropDownList CssClass="select" ID="title" runat="server">
            </asp:DropDownList>
            <asp:RegularExpressionValidator ControlToValidate="title" ID="titleRegEx" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="fullNameRow" class="fullNameRow" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="fullNameLabel" runat="server" AssociatedControlID="fullName"></asp:Label>
            <asp:TextBox ID="fullName" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="fullName" ID="fullNameRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="fullName" ID="fullNameRegEx" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="forenameRow" class="forenameRow" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="forenameLabel" runat="server" AssociatedControlID="forename"></asp:Label>
            <asp:TextBox ID="forename" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="forename" ID="forenameRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="forename" ID="forenameRegEx" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="surnameRow" class="surnameRow" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="surnameLabel" runat="server" AssociatedControlID="surname"></asp:Label>
            <asp:TextBox ID="surname" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="surname" ID="surnameRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="surname" ID="surnameRegEx" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="positionRow" class="positionRow" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="positionLabel" runat="server" AssociatedControlID="position"></asp:Label>
            <asp:TextBox ID="position" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RegularExpressionValidator ControlToValidate="position" ID="positionRegEx" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="emailRow" class="emailRow" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="emailLabel" runat="server" AssociatedControlID="email"></asp:Label>
            <asp:TextBox ID="email" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="email" ID="emailRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="email" ID="emailRegEx" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="emailCompareRow" class="emailRow" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="emailConfirmLabel" runat="server" AssociatedControlID="emailConfirm"></asp:Label>
            <asp:TextBox ID="emailConfirm" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="emailConfirm" ID="emailConfirmRFV"
                runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                ValidationGroup="Registration" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="emailConfirmCompare" runat="server" OnInit="SetupCompareValidator"
                ControlToValidate="emailConfirm" ControlToCompare="email" SetFocusOnError="true"
                Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true"
                Text="*"></asp:CompareValidator>
        </div>
        <div id="phoneRow" class="phoneRow" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="phoneLabel" runat="server" AssociatedControlID="phone"></asp:Label>
            <asp:TextBox ID="phone" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="phone" ID="phoneRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="phone" ID="phoneRegEx" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="faxRow" class="faxRow" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="faxLabel" runat="server" AssociatedControlID="fax"></asp:Label>
            <asp:TextBox ID="fax" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RegularExpressionValidator ControlToValidate="fax" ID="faxRegEx" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
    </div>
    <div id="passwordRow" class="passwordRow" runat="server" oninit="SetupDisplayType">
        <Talent:ChangePassword ID="ChangeMyPassword1" runat="server"></Talent:ChangePassword>
    </div>
    <div id="AddressBox" class="AddressBox box default-form" runat="server"  oninit="SetupDisplayType">
        <div id="AddressTitleRow" runat="server">
            <h2>
                <asp:Label ID="AddressInfoLabel" runat="server"></asp:Label>
            </h2>
        </div>
        <% CreateAddressingJavascript()%>
        <% CreateAddressingHiddenFields()%>
        <div id="FindAddressButtonRow" class="FindAddressButtonRow" runat="server" oninit="SetupDisplayType">
            <label>
                &nbsp;</label>
            <a id="AddressingLinkButton" name="AddressingLinkButtton" class="AddressingLinkButton"
                href="Javascript:addressingPopup();">
                <%=GetAddressingLinkText()%>
            </a>
        </div>
        <div id="companyNameRow" class="companyNameRow" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="companyNameLabel" runat="server" AssociatedControlID="companyName"></asp:Label>
            <asp:TextBox ID="companyName" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="companyName" ID="companyNameRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="companyName" ID="companyNameRegEx"
                runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                ValidationGroup="Registration" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="AddressLine1Row" class="AddressLine1Row" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="lblAddress1" runat="server" AssociatedControlID="address1"></asp:Label>
            <asp:TextBox ID="address1" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="address1" ID="rfvAddress1" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="address1" ID="regExAddress1" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="AddressLine2Row" class="AddressLine2Row" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="lblAddress2" runat="server" AssociatedControlID="address2"></asp:Label>
            <asp:TextBox ID="address2" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="address2" ID="rfvAddress2" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="address2" ID="regExAddress2" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="AddressLine3Row" class="AddressLine3Row" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="lblAddress3" runat="server" AssociatedControlID="address3"></asp:Label>
            <asp:TextBox ID="address3" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="address3" ID="rfvAddress3" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="address3" ID="regExAddress3" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="AddressLine4Row" class="AddressLine4Row" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="lblAddress4" runat="server" AssociatedControlID="address4"></asp:Label>
            <asp:TextBox ID="address4" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="address4" ID="rfvAddress4" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="address4" ID="regExAddress4" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="AddressLine5Row" class="AddressLine5Row" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="lblAddress5" runat="server" AssociatedControlID="address5"></asp:Label>
            <asp:TextBox ID="address5" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="address5" ID="rfvAddress5" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="address5" ID="regExAddress5" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="AddressPostcodeRow" class="AddressPostcodeRow" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="postcodeLabel" runat="server" AssociatedControlID="postcode"></asp:Label>
            <asp:TextBox ID="postcode" CssClass="input-m" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="postcode" ID="postcodeRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="postcode" ID="postcodeRegEx" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="AddressCountryRow" class="AddressCountryRow" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="countryLabel" runat="server" AssociatedControlID="country"></asp:Label>
            <asp:DropDownList ID="country" CssClass="select" runat="server">
            </asp:DropDownList>
            <asp:RegularExpressionValidator ControlToValidate="country" ID="countryRegEx" runat="server"
                OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="accountNumberRow" class="AccountNumberRow" runat="server" oninit="SetupDisplayType">
            <asp:Label ID="accountNumberLabel" runat="server" AssociatedControlID="accountNumber"></asp:Label>
            <asp:TextBox ID="accountNumber" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="accountNumber" ID="accountNumberRFV"
                runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                ValidationGroup="Registration" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
        </div>
    </div>
    <div id="CompanyInfoBox" class="CompanyInfoBox box default-form" runat="server" oninit="SetupDisplayType">
        <div id="CompanyInfoTitleRow" runat="server">
            <h2>
                <asp:Label ID="CompanyInfoLabel" runat="server"></asp:Label>
            </h2>
            <div id="vatNoRow" class="vatNumberRow" runat="server" oninit="SetupDisplayType">
                <asp:Label ID="vatNumberLabel" runat="server" AssociatedControlID="vatNumber"></asp:Label>
                <asp:TextBox ID="vatNumber" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="vatNumber" ID="vatNumberRFV" runat="server"
                    OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                    Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            </div>
            <div id="costCenRow" class="costCenRow" runat="server" oninit="SetupDisplayType">
                <asp:Label ID="costCenLabel" runat="server" AssociatedControlID="costCen"></asp:Label>
                <asp:TextBox ID="costCen" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="costCen" ID="costCenRFV" runat="server"
                    OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                    Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="fullName" ID="costCenRegEx" runat="server"
                    OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                    Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
        </div>
    </div>
    <div id="newsletterRow" class="newsletterRow" runat="server" oninit="SetupDisplayType">
        <asp:CheckBox ID="Newsletter" runat="server" />
    </div>
    <div id="NewsletterChoiceRow" class="NewsletterChoiceRow" runat="server" oninit="SetupDisplayType">
        <asp:RadioButtonList ID="NewsletterChoice" runat="server" CssClass="newsletter-choice">
        </asp:RadioButtonList>
    </div>
    <div id="option1" class="option1" runat="server" oninit="SetupDisplayType">
        <asp:CheckBox ID="opt1" runat="server" />
    </div>
    <div id="option2" class="option2" runat="server" oninit="SetupDisplayType">
        <asp:CheckBox ID="opt2" runat="server" />
    </div>
    <div id="option3" class="option3" runat="server" oninit="SetupDisplayType">
        <asp:CheckBox ID="opt3" runat="server" />
    </div>
    <div id="option4" class="option4" runat="server" oninit="SetupDisplayType">
        <asp:CheckBox ID="opt4" runat="server" />
    </div>
    <div id="option5" class="option5" runat="server" oninit="SetupDisplayType">
        <asp:CheckBox ID="opt5" runat="server" />
    </div>
    <div id="registerBtn" class="registerBtn" runat="server" oninit="SetupDisplayType">
        <asp:Button ID="updateBtn" CssClass="button" runat="server" CausesValidation="true"
            ValidationGroup="Registration" />
    </div>
    <div id="supplementaryText" class="supplementaryText" runat="server" oninit="SetupDisplayType">
        <asp:Label ID="supplementaryTextLabel" runat="server" AssociatedControlID="accountNumber"></asp:Label>
    </div>
</div>
