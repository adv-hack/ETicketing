<%@ Control Language="VB" AutoEventWireup="false" CodeFile="RegistrationForm2.ascx.vb"
    Inherits="UserControls_RegistrationForm2" %>
<asp:Label ID="RegistrationHeaderLabel" runat="server" CssClass="titleLabel"></asp:Label>
<p class="instructions">
    <asp:Label ID="RegistrationInstructionsLabel" runat="server" CssClass="instructionsLabel"></asp:Label></p>
<p class="error">
    <asp:Label ID="ErrorLabel" runat="server" CssClass="error"></asp:Label>&nbsp;</p>
<p class="error">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList"
        ValidationGroup="Registration" />
</p>
<div id="registration">
    <div id="PersonalDetailsBox" class="PersonalDetailsBox box default-form" runat="server">
        <h2>
            <asp:Label ID="PersonalDetailsLabel" runat="server"></asp:Label>
        </h2>
        <div id="titleRow" class="titleRow" runat="server">
            <asp:Label ID="titleLabel" runat="server" AssociatedControlID="title"></asp:Label>
            <asp:DropDownList CssClass="select" ID="title" runat="server">
            </asp:DropDownList>
            <asp:RegularExpressionValidator ControlToValidate="title" ID="titleRegEx" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="forenameRow" class="forenameRow" runat="server">
            <asp:Label ID="forenameLabel" runat="server" AssociatedControlID="forename"></asp:Label>
            <asp:TextBox ID="forename" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="forename" ID="forenameRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="forename" ID="forenameRegEx" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="surnameRow" class="surnameRow" runat="server">
            <asp:Label ID="surnameLabel" runat="server" AssociatedControlID="surname"></asp:Label>
            <asp:TextBox ID="surname" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="surname" ID="surnameRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="surname" ID="surnameRegEx" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="positionRow" class="positionRow" runat="server">
            <asp:Label ID="positionLabel" runat="server" AssociatedControlID="position"></asp:Label>
            <asp:TextBox ID="position" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="position" ID="positionRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="position" ID="positionRegEx" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="emailRow" class="emailRow" runat="server">
            <asp:Label ID="emailLabel" runat="server" AssociatedControlID="email"></asp:Label>
            <asp:TextBox ID="email" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="email" ID="emailRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="email" ID="emailRegEx" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="emailCompareRow" class="emailRow" runat="server">
            <asp:Label ID="emailConfirmLabel" runat="server" AssociatedControlID="emailConfirm"></asp:Label>
            <asp:TextBox ID="emailConfirm" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:CompareValidator ID="emailConfirmCompare" runat="server" OnInit="SetupCompareValidator"
                ControlToValidate="emailConfirm" ControlToCompare="email" SetFocusOnError="true"
                Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true"
                Text="*"></asp:CompareValidator>
         <asp:RequiredFieldValidator ControlToValidate="emailConfirm" ID="emailConfirmRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
        </div>
        <div id="phoneRow" class="phoneRow" runat="server">
            <asp:Label ID="phoneLabel" runat="server" AssociatedControlID="phone"></asp:Label>
            <asp:TextBox ID="phone" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="phone" ID="phoneRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="phone" ID="phoneRegEx" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="faxRow" class="faxRow" runat="server">
            <asp:Label ID="faxLabel" runat="server" AssociatedControlID="fax"></asp:Label>
            <asp:TextBox ID="fax" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="fax" ID="faxRFV" runat="server" OnInit="SetupRequiredValidator"
                SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static"
                Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="fax" ID="faxRegEx" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
    </div>
    <div id="CredentialsBox" class="CredentialsBox box default-form" runat="server">
        <div id="CredentialsBoxTitle" runat="server">
            <h2>
                <asp:Label ID="CredentialsLabel" runat="server"></asp:Label>
            </h2>
        </div>
        <div id="PasswordInstructionsRow" runat="server">
            <p class="password-instructions">
                <asp:Label ID="passwordInstructionsLabel" runat="server"></asp:Label>
            </p>
        </div>
        <div id="usernameRow" class="usernameRow" runat="server">
            <asp:Label ID="loginIDLabel" runat="server" AssociatedControlID="loginid"></asp:Label>
            <asp:RequiredFieldValidator ControlToValidate="loginid" ID="loginidRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:TextBox ID="loginid" CssClass="input-l" runat="server"></asp:TextBox>
        </div>
        <div id="Password1Row" class="Password1Row" runat="server">
            <asp:Label ID="password1Label" runat="server" AssociatedControlID="password1"></asp:Label>
            <asp:TextBox ID="password1" CssClass="input-l" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="password1" ID="password1RFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="password1" ID="password1RegEx"
                runat="server" OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                ValidationGroup="Registration" Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="Password2Row" class="Password2Row" runat="server">
            <asp:Label ID="password2Label" runat="server" AssociatedControlID="password2"></asp:Label>
            <asp:TextBox ID="password2" CssClass="input-l" runat="server" TextMode="Password"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="password2" ID="password2RFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="passwordCompare" runat="server" OnInit="SetupCompareValidator"
                ControlToValidate="password2" ControlToCompare="password1" SetFocusOnError="true"
                Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true"
                Text="*"></asp:CompareValidator>
        </div>
    </div>
    <div id="AddressBox" class="AddressBox box default-form" runat="server">
        <div id="AddressTitleRow" runat="server">
            <h2>
                <asp:Label ID="AddressInfoLabel" runat="server"></asp:Label>
            </h2>
        </div>
        <% CreateAddressingJavascript()%>
        <% CreateAddressingHiddenFields()%>
        
        <%--  <div id="companyNameRow" class="companyNameRow" runat="server">
            <asp:Label ID="companyNameLabel" runat="server" AssociatedControlID="companyName"></asp:Label>
            <asp:TextBox ID="companyName" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="companyName" ID="companyNameRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="companyName" ID="companyNameRegEx" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="organisationRow" class="organisationRow" runat="server">
            <asp:Label ID="organisationLabel" runat="server" AssociatedControlID="organisation"></asp:Label>
            <asp:TextBox ID="organisation" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="organisation" ID="organisationRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="organisation" ID="organisationRegEx" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>--%>   
        <div id="AddressCountryRow" class="AddressCountryRow" runat="server">
            <asp:Label ID="countryLabel" runat="server" AssociatedControlID="country"></asp:Label>
            <asp:DropDownList ID="country" CssClass="select" runat="server">
            </asp:DropDownList>
            <asp:RegularExpressionValidator ControlToValidate="country" ID="countryRegEx" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="AddressPostcodeRow" class="AddressPostcodeRow" runat="server">
            <asp:Label ID="postcodeLabel" runat="server" AssociatedControlID="postcode"></asp:Label>
            <asp:TextBox ID="postcode" CssClass="input-m" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="postcode" ID="postcodeRFV" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="postcode" ID="postcodeRegEx" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="FindAddressButtonRow" class="FindAddressButtonRow" runat="server">
            <label>
                &nbsp;</label>
            <a id="AddressingLinkButton" name="AddressingLinkButtton" class="AddressingLinkButton"
                href="Javascript:addressingPopup();">
                <%=GetAddressingLinkText()%>
            </a>
        </div>
        <div id="AddressLine1Row" class="AddressLine1Row" runat="server">
            <asp:Label ID="lblAddress1" runat="server" AssociatedControlID="Address1"></asp:Label>
            <asp:TextBox ID="Address1" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="Address1" ID="rfvAddress1" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="Address1" ID="regExAddress1" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="AddressLine2Row" class="AddressLine2Row" runat="server">
            <asp:Label ID="lblAddress2" runat="server" AssociatedControlID="Address2"></asp:Label>
            <asp:TextBox ID="Address2" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="Address2" ID="rfvAddress2" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="Address2" ID="regExAddress2" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="AddressLine3Row" class="AddressLine3Row" runat="server">
            <asp:Label ID="lblAddress3" runat="server" AssociatedControlID="Address3"></asp:Label>
            <asp:TextBox ID="Address3" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="Address3" ID="rfvAddress3" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="Address3" ID="regExAddress3" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="AddressLine4Row" class="AddressLine4Row" runat="server">
            <asp:Label ID="lblAddress4" runat="server" AssociatedControlID="Address4"></asp:Label>
            <asp:TextBox ID="Address4" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="Address4" ID="rfvAddress4" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="Address4" ID="regExAddress4" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
        <div id="AddressLine5Row" class="AddressLine5Row" runat="server">
            <asp:Label ID="lblAddress5" runat="server" AssociatedControlID="Address5"></asp:Label>
            <asp:TextBox ID="Address5" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="Address5" ID="rfvAddress5" runat="server"
                OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ControlToValidate="Address5" ID="regExAddress5" runat="server"
                OnLoad="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                Display="Static" Enabled="true" Text="*"></asp:RegularExpressionValidator>
        </div>
       
     
        <div id="accountNumberRow" class="AccountNumberRow" runat="server">
            <asp:Label ID="accountNumberLabel" runat="server" AssociatedControlID="accountNumber"></asp:Label>
            <asp:TextBox ID="accountNumber" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="accountNumber" ID="accountNumberRFV"
                runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                ValidationGroup="Registration" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
        </div>
        <div id="accountNumberRow2" class="AccountNumberRow2" runat="server">
            <asp:Label ID="accountNumberLabel2" runat="server" AssociatedControlID="accountNumber2"></asp:Label>
            <asp:TextBox ID="accountNumber2" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="accountNumber2" ID="accountNumber2RFV"
                runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                ValidationGroup="Registration" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
        </div>
        <div id="accountNumberRow3" class="AccountNumberRow3" runat="server">
            <asp:Label ID="accountNumberLabel3" runat="server" AssociatedControlID="accountNumber3"></asp:Label>
            <asp:TextBox ID="accountNumber3" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="accountNumber3" ID="accountNumber3RFV"
                runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                ValidationGroup="Registration" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
        </div>
        <div id="accountNumberRow4" class="AccountNumberRow4" runat="server">
            <asp:Label ID="accountNumberLabel4" runat="server" AssociatedControlID="accountNumber4"></asp:Label>
            <asp:TextBox ID="accountNumber4" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="accountNumber4" ID="accountNumber4RFV"
                runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                ValidationGroup="Registration" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
        </div>
        <div id="accountNumberRow5" class="AccountNumberRow5" runat="server">
            <asp:Label ID="accountNumberLabel5" runat="server" AssociatedControlID="accountNumber5"></asp:Label>
            <asp:TextBox ID="accountNumber5" CssClass="input-l" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ControlToValidate="accountNumber5" ID="accountNumber5RFV"
                runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true"
                ValidationGroup="Registration" Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
        </div>
    </div>
   <div id="CompanyInfoBox" class="CompanyInfoBox box default-form" runat="server">
        <div id="CompanyInfoTitleRow" runat="server">
            <h2>
                <asp:Label ID="CompanyInfoLabel" runat="server" ></asp:Label>
            </h2>
            <div id="CompanyDetails1Row" class="CompanyDetails1Row" runat="server">
                <asp:Label ID="lblCompanyDetails1" runat="server" AssociatedControlID="ddlcompanyDetails1"></asp:Label>
         
                <asp:DropDownList  ID="ddlCompanyDetails1" runat="server">
                </asp:DropDownList>
            </div>
            <div id="CompanyDetails2Row" class="CompanyDetails2Row" runat="server">
                <asp:Label ID="lblCompanyDetails2" runat="server" AssociatedControlID="ddlcompanyDetails2"></asp:Label>
                <asp:DropDownList  ID="ddlCompanyDetails2" runat="server">
                </asp:DropDownList>
            </div>
            <div id="vatNoRow" class="vatNumberRow" runat="server">
                <asp:Label ID="vatNumberLabel" runat="server" AssociatedControlID="vatNumber"></asp:Label>
                <asp:TextBox ID="vatNumber" CssClass="input-l" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="vatNumber" ID="vatNumberRFV" runat="server"
                    OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                    Display="Static" Enabled="true" Text="*"></asp:RequiredFieldValidator>
            </div>
        </div>
    </div>
    <div class="tandcs">
        <asp:CheckBox ID="TandCs" runat="server" />
    </div>
     <div class="newsletter">
            <asp:CheckBox ID="Newsletter" runat="server" />
        </div>
        <div class="newsletter-choice">
            <asp:RadioButtonList ID="NewsletterChoice" runat="server" CssClass="newsletter-choice">
            </asp:RadioButtonList>
        </div>
        <div class="option1">
            <asp:CheckBox ID="opt1" runat="server" />
        </div>
        <div class="option2">
            <asp:CheckBox ID="opt2" runat="server" />
        </div>
        <div class="option3">
            <asp:CheckBox ID="opt3" runat="server" />
        </div>
        <div class="option4">
            <asp:CheckBox ID="opt4" runat="server" />
        </div>
        <div class="option5">
            <asp:CheckBox ID="opt5" runat="server" />
        </div>
    <div class="registerBtn">
        <label>
            &nbsp;</label>
        <asp:Button ID="registerBtn" CssClass="button" runat="server" CausesValidation="true"
            ValidationGroup="Registration" />
    </div>
    <div class="supplementaryText">
        <label>
            &nbsp;</label>
        <asp:Label ID="supplementaryTextLabel" runat="server" AssociatedControlID="accountNumber"></asp:Label>
    </div>
</div>
