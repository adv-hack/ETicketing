<%@ Control Language="VB" AutoEventWireup="false" CodeFile="UpdateDetailsForm.ascx.vb" Inherits="UserControls_UpdateDetailsForm" %>
  <%@ Register Src="~/UserControls/ChangePassword.ascx" TagName="ChangePassword" TagPrefix="Talent" %>
    <%@ Register Src="~/UserControls/AddressFormat1Form.ascx" TagName="AddressFormat1Form" TagPrefix="Talent" %>

      <script type="text/javascript">
        function ShowOrHide(value) {

          // Get the current DOB from the form 
          dobDay = document.getElementsByClassName("js--dob-Day")[0].value;
          dobMonth = document.getElementsByClassName("js--dob-Month")[0].value;
          dobYear = document.getElementsByClassName("js--dob-Year")[0].value;

          // Get latest date of birth that makes this person under the threshold and so a junior   
          ParentalConsentCeilingDay = document.getElementsByClassName("js--Parental-Consent-Ceiling-Day")[0].textContent;
          ParentalConsentCeilingMonth = document.getElementsByClassName("js--Parental-Consent-Ceiling-Month")[0].textContent;
          ParentalConsentCeilingYear = document.getElementsByClassName("js--Parental-Consent-Ceiling-Year")[0].textContent;
          ParentalEmail = document.getElementsByClassName("js--Parental-Email")[0].value;
          ParentalPhone = document.getElementsByClassName("js--Parental-Phone")[0].value;
          ShowParentalConsentAlert = document.getElementsByClassName("js--Show-Parental-Consent-Alert")[0].textContent;

          // Get database settings for parental email/phone RFV
          parentEmailRFVStatus = document.getElementsByClassName("js--Parent-Email-RFV-Status")[0].textContent;
          parentPhoneRFVStatus = document.getElementsByClassName("js--Parent-Phone-RFV-Status")[0].textContent;
          // We only want the RFVs to be on if the fields are shown so disable
          DisableParentRFV()

          document.getElementsByClassName("js--show-parental-consent")[0].style.display = 'none';
          if (dobDay == " -- " || dobMonth == " -- " || dobYear == " -- ") {
          }
          else {

            Dob = new Date(dobYear, dobMonth, dobDay)
            Ceiling = new Date(ParentalConsentCeilingYear, ParentalConsentCeilingMonth, ParentalConsentCeilingDay)
            if (Dob > Ceiling) {
              document.getElementsByClassName("js--show-parental-consent")[0].style.display = 'block';
              SetParentRFV()
              if ((ShowParentalConsentAlert == "True") && ParentalEmail.trim() == "" && ParentalPhone.trim() == "") {
                alertHeading = document.getElementsByClassName("js--Parental-Consent-Alert-Heading")[0].textContent;
                alertDetails = document.getElementsByClassName("js--Parental-Consent-Alert-Details")[0].textContent;
                alertify.alert(alertHeading, alertDetails, function () { });
              }
            }
          }
        }

        function DisableParentRFV() {
          var parentemailRFV = document.getElementById("<%=parentemailRFV.ClientID%>");
          if (parentemailRFV != null) {
            ValidatorEnable(parentemailRFV, false);
          }
          var parentphoneRFV = document.getElementById("<%=parentphoneRFV.ClientID%>");
          if (parentphoneRFV != null) {
            ValidatorEnable(parentphoneRFV, false);
          }
        }
        function SetParentRFV() {
          // Sets RFV to status set in database
          var parentemailRFV = document.getElementById("<%=parentemailRFV.ClientID%>");
          if (parentEmailRFVStatus == "true" && parentemailRFV != null) {
            ValidatorEnable(parentemailRFV, true);
          }
          var parentPhoneRFV = document.getElementById("<%=parentphoneRFV.ClientID%>");
          if (parentPhoneRFVStatus == "true" && parentPhoneRFV != null) {
            ValidatorEnable(parentPhoneRFV, true);
          }
        }

        $(document).ready(function () {
          ShowOrHide();
        });
      </script>


      <asp:Literal ID="RegistrationHeaderLabel" runat="server" />
      <asp:Label ID="ErrorLabel" runat="server" CssClass="alert-box success ebiz-successfully-printed-address-label" />
      <asp:Label ID="ErrorLabelID" runat="server" CssClass="alert-box alert" />
      <asp:PlaceHolder ID="plhPrintAddressLabelTop" runat="server">


        <div class="ebiz-update-profile">
          <asp:Button ID="btnPrintAddressLabelTop" runat="server" CausesValidation="true" ValidationGroup="Registration" CssClass="button"
          />
        </div>
      </asp:PlaceHolder>

      <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList" ValidationGroup="Registration" CssClass="alert-box alert"
      />
      <asp:PlaceHolder ID="plhPersonalDetailsBox" runat="server">
        <div class="panel ebiz-personal-details">
          <h2>

            <asp:Literal ID="PersonalDetailsLabel" runat="server" />

          </h2>
          <asp:PlaceHolder ID="plhTitleRow" runat="server">
            <div class="row ebiz-title">
              <div class="medium-3 columns">
                <asp:Label ID="titleLabel" runat="server" AssociatedControlID="title" />
              </div>
              <div class="medium-9 columns">
                <asp:DropDownList ID="title" runat="server" />
                <asp:RegularExpressionValidator ControlToValidate="title" ID="titleRegEx" runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhForenameRow" runat="server">
            <div class="row ebiz-first-name">
              <div class="medium-3 columns">
                <asp:Label ID="forenameLabel" runat="server" AssociatedControlID="forename" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="forename" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="forename" ID="forenameRFV" runat="server" OnInit="SetupRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="forename" ID="forenameRegEx" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhInitialsRow" runat="server">
            <div class="row ebiz-initials">
              <div class="medium-3 columns">
                <asp:Label ID="initialsLabel" runat="server" AssociatedControlID="initials" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="initials" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="initials" ID="initialsRFV" runat="server" OnInit="SetupRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="initials" ID="initialsRegEx" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhSurnameRow" runat="server">
            <div class="row ebiz-last-name">
              <div class="medium-3 columns">
                <asp:Label ID="surnameLabel" runat="server" AssociatedControlID="surname" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="surname" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="surname" ID="surnameRFV" runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
                <asp:RegularExpressionValidator ControlToValidate="surname" ID="surnameRegEx" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhCustomersuffixRow" runat="server">
            <div class="row ebiz-customersuffix">
              <div class="medium-3 columns">
                <asp:Label ID="customersuffixLabel" runat="server" AssociatedControlID="customersuffix" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="customersuffix" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="customersuffix" ID="customersuffixRFV" runat="server" OnInit="SetupRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="customersuffix" ID="customersuffixRegEx" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhNicknameRow" runat="server">
            <div class="row ebiz-nickname">
              <div class="medium-3 columns">
                <asp:Label ID="nicknameLabel" runat="server" AssociatedControlID="nickname" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="nickname" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="nickname" ID="nicknameRFV" runat="server" OnInit="SetupRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="nickname" ID="nicknameRegEx" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhAltusernameRow" runat="server">
            <div class="row ebiz-altusername">
              <div class="medium-3 columns">
                <asp:Label ID="altusernameLabel" runat="server" AssociatedControlID="altusername" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="altusername" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="altusername" ID="altusernameRFV" runat="server" OnInit="SetupRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="altusername" ID="altusernameRegEx" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhContactSLRow" runat="server">
            <div class="row ebiz-contactsl">
              <div class="medium-3 columns">
                <asp:Label ID="contactslLabel" runat="server" AssociatedControlID="altusername" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="contactsl" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="contactsl" ID="contactslRFV" runat="server" OnInit="SetupRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="contactsl" ID="contactslRegEx" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhSalutationRow" runat="server">
            <div class="row ebiz-salutation">
              <div class="medium-3 columns">
                <asp:Label ID="salutationLabel" runat="server" AssociatedControlID="salutation" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="salutation" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="salutation" ID="salutationRFV" runat="server" OnInit="SetupRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="salutation" ID="salutationRegEx" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhCompanyRow" runat="server">
            <div class="row ebiz-company">
              <div class="medium-3 columns">
                <asp:Label ID="lblCompanyName" runat="server" AssociatedControlID="txtCompanyName" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="txtCompanyName" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="txtCompanyName" ID="companyNameRFV" runat="server" OnInit="SetupRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="txtCompanyName" ID="companyNameRegEx" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhPositionRow" runat="server">
            <div class="row ebiz-position">
              <div class="medium-3 columns">
                <asp:Label ID="positionLabel" runat="server" AssociatedControlID="position" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="position" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="position" ID="positionRFV" runat="server" OnInit="SetupRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="position" ID="positionRegEx" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>

          <div class="js--show-parental-consent-data" runat="server" id="showparentalconsentdata" style=" display: none; ">
            <asp:Label ID="lblParentalConsentCeilingYear" class="js--Parental-Consent-Ceiling-Year" runat="server" />
            <asp:Label ID="lblParentalConsentCeilingMonth" class="js--Parental-Consent-Ceiling-Month" runat="server" />
            <asp:Label ID="lblParentalConsentCeilingDay" class="js--Parental-Consent-Ceiling-Day" runat="server" />
            <asp:Label ID="lblParentalConsentAlertHeading" class="js--Parental-Consent-Alert-Heading" runat="server" />
            <asp:Label ID="lblParentalConsentAlertDetails" class="js--Parental-Consent-Alert-Details" runat="server" />
            <asp:Label ID="lblShowAParentalConsentAlert" class="js--Show-Parental-Consent-Alert" runat="server" />
            <asp:Label ID="lblparentEmailRFVStatus" class="js--Parent-Email-RFV-Status" runat="server" />
            <asp:Label ID="lblparentPhoneRFVStatus" class="js--Parent-Phone-RFV-Status" runat="server" />
          </div>

          <asp:PlaceHolder ID="plhDOBRow" runat="server">
            <div class="row ebiz-dob">
              <div class="medium-3 columns ebiz-dob-label">
                <asp:Label ID="dobLabel" runat="server" AssociatedControlID="dobDay" />
              </div>
              <div class="medium-3 columns ebiz-dob-day">
                <asp:DropDownList ID="dobDay" runat="server" class="js--dob-Day" onchange="ShowOrHide(this.value)" />
                <asp:RegularExpressionValidator ControlToValidate="dobDay" ID="dobDayRegEx" runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
              </div>
              <div class="medium-3 columns ebiz-dob-month">
                <asp:DropDownList ID="dobMonth" runat="server" class="js--dob-Month" onchange="ShowOrHide(this.value)" />
                <asp:RegularExpressionValidator ControlToValidate="dobMonth" ID="dobMonthRegEx" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
              <div class="medium-3 columns ebiz-dob-year">
                <asp:DropDownList ID="dobYear" runat="server" class="js--dob-Year" onchange="ShowOrHide(this.value)" />
                <asp:RegularExpressionValidator ControlToValidate="dobYear" ID="dobYearRegEx" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhSexRow" runat="server">
            <div class="row ebiz-sex">
              <div class="medium-3 columns">
                <asp:Label ID="sexLabel" runat="server" AssociatedControlID="sex" />
              </div>
              <div class="medium-9 columns">
                <asp:DropDownList ID="sex" runat="server" />
                <asp:RegularExpressionValidator ControlToValidate="sex" ID="sexRegEx" runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhMothersnameRow" runat="server">
            <div class="row ebiz-mothers-name">
              <div class="medium-3 columns">
                <asp:Label ID="mothersnameLabel" runat="server" AssociatedControlID="mothersname" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="mothersname" runat="server" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhFathersnameRow" runat="server">
            <div class="row ebiz-fathers-name">
              <div class="medium-3 columns">
                <asp:Label ID="fathersnameLabel" runat="server" AssociatedControlID="fathersname" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="fathersname" runat="server" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhEmailRow" runat="server">
            <div class="row ebiz-email ebiz-contact-preference">
              <div class="medium-3 columns">
                <asp:Label ID="emailLabel" runat="server" AssociatedControlID="email" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="email" runat="server" type="email" />
                <asp:RequiredFieldValidator ControlToValidate="email" ID="emailRFV" runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
                <asp:RegularExpressionValidator ControlToValidate="email" ID="emailRegEx" runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
              </div>
              <div class="medium-9 large-offset-3 columns">
                <asp:CheckBox ID="cbContactbyEmail" Visible="true" runat="server"></asp:CheckBox>
                <asp:Label ID="ContactbyEmailLabel" runat="server" AssociatedControlID="cbContactbyEmail" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhEmailCompare" runat="server" Visible="True">
            <div class="row ebiz-confirm-email">
              <div class="medium-3 columns">
                <asp:Label ID="emailConfirmLabel" runat="server" AssociatedControlID="emailConfirm" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="emailConfirm" runat="server" type="email" />
                <asp:RequiredFieldValidator ControlToValidate="emailConfirm" ID="emailConfirmRFV" runat="server" OnInit="SetupRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:CompareValidator ID="emailConfirmCompare" runat="server" OnInit="SetupCompareValidator" ControlToValidate="emailConfirm"
                  ControlToCompare="email" SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static"
                  Enabled="true" Text="" CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhPhoneRow" runat="server">
            <div class="row ebiz-home-number ebiz-contact-preference">
              <div class="medium-3 columns">
                <asp:Label ID="phoneLabel" runat="server" AssociatedControlID="phone" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="phone" runat="server" type="tel" />
                <asp:RequiredFieldValidator ControlToValidate="phone" ID="phoneRFV" runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
                <asp:RegularExpressionValidator ControlToValidate="phone" ID="phoneRegEx" runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
              </div>
              <div class="medium-9 large-offset-3 columns">
                <asp:CheckBox ID="cbContactbyTelephoneHome" Visible="true" runat="server"></asp:CheckBox>
                <asp:Label ID="ContactbyTelephoneHomeLabel" runat="server" AssociatedControlID="cbContactbyTelephoneHome" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhWorkRow" runat="server">
            <div class="row ebiz-work-number ebiz-contact-preference">
              <div class="medium-3 columns">
                <asp:Label ID="workLabel" runat="server" AssociatedControlID="work" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="work" runat="server" type="tel" />
                <asp:RequiredFieldValidator ControlToValidate="work" ID="workRFV" runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
                <asp:RegularExpressionValidator ControlToValidate="work" ID="workRegEx" runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
              </div>
              <div class="medium-9 large-offset-3 columns">
                <asp:CheckBox ID="cbContactbyTelephoneWork" Visible="true" runat="server"></asp:CheckBox>
                <asp:Label ID="ContactbyTelephoneWorkLabel" runat="server" AssociatedControlID="cbContactbyTelephoneWork" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhMobileRow" runat="server">
            <div class="row ebiz-mobile-number ebiz-contact-preference">
              <div class="medium-3 columns">
                <asp:Label ID="mobileLabel" runat="server" AssociatedControlID="mobile" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="mobile" runat="server" type="tel" />
                <asp:RequiredFieldValidator ControlToValidate="mobile" ID="mobileRFV" runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
                <asp:RegularExpressionValidator ControlToValidate="mobile" ID="mobileRegEx" runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
              </div>
              <div class="medium-9 large-offset-3 columns">
                <asp:CheckBox ID="cbContactbyMobile" Visible="true" runat="server"></asp:CheckBox>
                <asp:Label ID="ContactbyMobileLabel" runat="server" AssociatedControlID="cbContactbyMobile" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhFaxRow" runat="server">
            <div class="row ebiz-fax-number">
              <div class="medium-3 columns">
                <asp:Label ID="faxLabel" runat="server" AssociatedControlID="fax" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="fax" runat="server" type="tel" />
                <asp:RequiredFieldValidator ControlToValidate="fax" ID="faxRFV" runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
                <asp:RegularExpressionValidator ControlToValidate="fax" ID="faxRegEx" runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhAgentOnlyFields" runat="server">
            <div class="row ebiz-price-band">
              <div class="medium-3 columns">
                <asp:Label ID="pricebandlabel" runat="server" AssociatedControlID="ddlpriceband" />
              </div>
              <div class="medium-9 columns">
                <asp:DropDownList ID="ddlpriceband" runat="server" />
              </div>
            </div>
            <div class="row ebiz-stop-code">
              <div class="medium-3 columns ebiz-stop-code-label">
                <asp:Label ID="Stopcodelabel" runat="server" AssociatedControlID="ddlStopcode" />
              </div>
              <div class="medium-6 columns ebiz-stop-code-list">
                <asp:DropDownList ID="ddlStopcode" runat="server" />
              </div>
              <div class="medium-3 columns ebiz-stop-code-button">
                <asp:HyperLink ID="hplStopCodeAudit" runat="server" data-open="stop-code-audit-modal" CssClass="ebiz-open-modal" NavigateUrl="~/PagesAgent/Profile/StopCodeAudit.aspx"
                />
                <div id="stop-code-audit-modal" class="reveal ebiz-reveal-ajax" data-reveal></div>
              </div>
              <asp:PlaceHolder ID="plhBookNumber" runat="server" Visible="false">
                <div class="row ebiz-book-number">
                  <div class="medium-3 columns">
                    <asp:Label ID="BookNumberLabel" runat="server" AssociatedControlID="BookNumber" />
                  </div>
                  <div class="medium-9 columns">
                    <asp:Label ID="BookNumber" runat="server" />
                  </div>
                </div>
              </asp:PlaceHolder>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhotherRow" runat="server">
            <div class="row ebiz-agent-only-other">
              <div class="medium-3 columns">
                <asp:Label ID="otherLabel" runat="server" AssociatedControlID="other" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="other" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="other" ID="otherRFV" runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
                <asp:RegularExpressionValidator ControlToValidate="other" ID="otherRegEx" runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true"
                  Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                />
              </div>
            </div>
          </asp:PlaceHolder>
        </div>
      </asp:PlaceHolder>
      <Talent:ChangePassword ID="ChangeMyPassword1" runat="server" />
      <Talent:AddressFormat1Form ID="AddressFormat1Form1" runat="server" />
      <asp:PlaceHolder ID="plhAddressBox" runat="server">
        <div class="panel ebiz-address-information">
          <asp:PlaceHolder ID="plhAddressTitleRow" runat="server">
            <h2>

              <asp:Literal ID="AddressInfoLabel" runat="server" />

            </h2>
          </asp:PlaceHolder>
          <% CreateAddressingJavascript()%>
            <% CreateAddressingHiddenFields()%>
              <asp:PlaceHolder ID="plhFindAddressButtonRow" runat="server">
                <div class="ebiz-find-address">
                  <a id="AddressingLinkButton" name="AddressingLinkButtton" href="Javascript:addressingPopup();">
                    <%=GetAddressingLinkText()%>
                  </a>
                </div>
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="plhAddressTypeRow" runat="server">
                <div class="row ebiz-address-type">
                  <div class="medium-3 columns">
                    <asp:Label ID="typeLabel" runat="server" AssociatedControlID="type" />
                  </div>
                  <div class="medium-9 columns">
                    <asp:TextBox ID="type" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="type" ID="typeRFV" runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true"
                      Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                    />
                    <asp:RegularExpressionValidator ControlToValidate="type" ID="typeRegEx" runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true"
                      Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                    />
                  </div>
                </div>
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="plhAddressReferenceRow" runat="server">
                <div class="row ebiz-address-reference">
                  <div class="medium-3 columns">
                    <asp:Label ID="referenceLabel" runat="server" AssociatedControlID="reference" />
                  </div>
                  <div class="medium-9 columns">
                    <asp:TextBox ID="reference" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="reference" ID="referenceRFV" runat="server" OnInit="SetupRequiredValidator"
                      SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true"
                      Text="" CssClass="error ebiz-validator-error" />
                    <asp:RegularExpressionValidator ControlToValidate="reference" ID="referenceRegEx" runat="server" OnInit="SetupRegExValidator"
                      SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true"
                      Text="" CssClass="error ebiz-validator-error" />
                  </div>
                </div>
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="plhAddressSequenceRow" runat="server">
                <div class="row ebiz-address-sequence">
                  <div class="medium-3 columns">
                    <asp:Label ID="sequenceLabel" runat="server" AssociatedControlID="sequence" />
                  </div>
                  <div class="medium-9 columns">
                    <asp:TextBox ID="sequence" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="sequence" ID="sequenceRFV" runat="server" OnInit="SetupRequiredValidator"
                      SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true"
                      Text="" CssClass="error ebiz-validator-error" />
                    <asp:RegularExpressionValidator ControlToValidate="sequence" ID="sequenceRegEx" runat="server" OnInit="SetupRegExValidator"
                      SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true"
                      Text="" CssClass="error ebiz-validator-error" />
                  </div>
                </div>
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="plhAddressLine1Row" runat="server">
                <div class="row ebiz-address-building">
                  <div class="medium-3 columns">
                    <asp:Label ID="buildingLabel" runat="server" AssociatedControlID="building" />
                  </div>
                  <div class="medium-9 columns">
                    <asp:TextBox ID="building" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="building" ID="buildingRFV" runat="server" OnInit="SetupRequiredValidator"
                      SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true"
                      Text="" CssClass="error ebiz-validator-error" />
                    <asp:RegularExpressionValidator ControlToValidate="building" ID="buildingRegEx" runat="server" OnInit="SetupRegExValidator"
                      SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true"
                      Text="" CssClass="error ebiz-validator-error" />
                  </div>
                </div>
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="plhAddressLine2Row" runat="server">
                <div class="row ebiz-address-street">
                  <div class="medium-3 columns">
                    <asp:Label ID="streetLabel" runat="server" AssociatedControlID="street" />
                  </div>
                  <div class="medium-9 columns">
                    <asp:TextBox ID="street" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="street" ID="streetRFV" runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true"
                      Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                    />
                    <asp:RegularExpressionValidator ControlToValidate="street" ID="streetRegEx" runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true"
                      Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                    />
                  </div>
                </div>
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="plhAddressLine3Row" runat="server">
                <div class="row ebiz-address-town">
                  <div class="medium-3 columns">
                    <asp:Label ID="townLabel" runat="server" AssociatedControlID="town" />
                  </div>
                  <div class="medium-9 columns">
                    <asp:TextBox ID="town" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="town" ID="townRFV" runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true"
                      Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                    />
                    <asp:RegularExpressionValidator ControlToValidate="town" ID="townRegEx" runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true"
                      Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                    />
                  </div>
                </div>
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="AddressLine4Row" runat="server">
                <div class="row ebiz-address-city">
                  <div class="medium-3 columns">
                    <asp:Label ID="cityLabel" runat="server" AssociatedControlID="city" />
                  </div>
                  <div class="medium-9 columns">
                    <asp:TextBox ID="city" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="city" ID="cityRFV" runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true"
                      Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                    />
                    <asp:RegularExpressionValidator ControlToValidate="city" ID="cityRegEx" runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true"
                      Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                    />
                  </div>
                </div>
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="plhAddressLine5Row" runat="server">
                <div class="row ebiz-address-county">
                  <div class="medium-3 columns">
                    <asp:Label ID="countyLabel" runat="server" AssociatedControlID="county" />
                  </div>
                  <div class="medium-9 columns">
                    <asp:TextBox ID="county" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="county" ID="countyRFV" runat="server" OnInit="SetupRequiredValidator" SetFocusOnError="true"
                      Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                    />
                    <asp:RegularExpressionValidator ControlToValidate="county" ID="countyRegEx" runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true"
                      Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text="" CssClass="error ebiz-validator-error"
                    />
                  </div>
                </div>
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="plhAddressPostcodeRow" runat="server">
                <div class="row ebiz-address-postcode">
                  <div class="medium-3 columns">
                    <asp:Label ID="postcodeLabel" runat="server" AssociatedControlID="postcode" />
                  </div>
                  <div class="medium-9 columns">
                    <asp:TextBox ID="postcode" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="postcode" ID="postcodeRFV" runat="server" OnInit="SetupRequiredValidator"
                      SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true"
                      Text="" CssClass="error ebiz-validator-error" />
                    <asp:RegularExpressionValidator ControlToValidate="postcode" ID="postcodeRegEx" runat="server" OnInit="SetupRegExValidator"
                      SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true"
                      Text="" CssClass="error ebiz-validator-error" />
                  </div>
                </div>
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="plhAddressCountryRow" runat="server">
                <div class="row ebiz-address-country">
                  <div class="medium-3 columns">
                    <asp:Label ID="countryLabel" runat="server" AssociatedControlID="country" />
                  </div>
                  <div class="medium-9 columns">
                    <asp:DropDownList ID="country" runat="server" />
                    <asp:RegularExpressionValidator ControlToValidate="country" ID="countryRegEx" runat="server" OnInit="SetupRegExValidator"
                      SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true"
                      Text="" CssClass="error ebiz-validator-error" />
                  </div>
                </div>
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="plhContactByPost" runat="server">
                <div class="row ebiz-contact-me-by-post">
                  <div class="medium-9 medium-offset-3 columns">
                    <asp:CheckBox ID="cbContactbyPost" runat="server"></asp:CheckBox>
                    <asp:Label ID="ContactbyPostLabel" runat="server" AssociatedControlID="cbContactbyPost" />
                  </div>
                </div>
              </asp:PlaceHolder>
        </div>
      </asp:PlaceHolder>
      <asp:PlaceHolder ID="plhSupporter" runat="server">
        <div class="panel ebiz-registration-supporter-selection">
          <div class="row registration-supporter-selection-one">
            <div class="medium-3 columns">
              <asp:Label ID="supporterSelectLabel1" runat="server" AssociatedControlID="supporterSelectDDL1" />
            </div>
            <div class="medium-6 columns">
              <asp:DropDownList ID="supporterSelectDDL1" runat="server" OnChange="Javascript: LoadSupporterSelectDDL2(this.form);PopulateDDL1Hidden();"
              />
              <asp:HiddenField ID="supporterSelect1Hidden" runat="server" />
            </div>
            <div class="medium-3 columns">
              <asp:Button ID="supporterSelectButton1" runat="server" CssClass="button" />
            </div>
          </div>
          <div class="row registration-supporter-selection-two">
            <div class="medium-3 columns">
              <asp:Label ID="supporterSelectLabel2" runat="server" AssociatedControlID="supporterSelectDDL2" />
            </div>
            <div class="medium-6 columns">
              <asp:DropDownList ID="supporterSelectDDL2" runat="server" OnChange="Javascript: LoadSupporterSelectDDL3(this.form);PopulateDDL2Hidden();"
              />
              <asp:HiddenField ID="supporterSelect2Hidden" runat="server" />
            </div>
            <div class="medium-3 columns">
              <asp:Button ID="supporterSelectButton2" runat="server" CssClass="button" />
            </div>
          </div>
          <div class="row registration-supporter-selection-three">
            <div class="medium-3 columns">
              <asp:Label ID="supporterSelectLabel3" runat="server" AssociatedControlID="supporterSelectDDL3" />
            </div>
            <div class="medium-9 columns">
              <asp:DropDownList ID="supporterSelectDDL3" runat="server" OnChange="Javascript: PopulateDDL3Hidden();" />
              <asp:HiddenField ID="supporterSelect3Hidden" runat="server" />
            </div>
          </div>
        </div>
      </asp:PlaceHolder>
      <asp:PlaceHolder ID="plhContactMethod" runat="server">
        <div class="panel ebiz-contact">
          <asp:Repeater ID="contactRepeater" runat="server">
            <HeaderTemplate>
              <h2>

                <asp:Literal ID="TeamContactHeaderLabel" runat="server" />

              </h2>
            </HeaderTemplate>
            <ItemTemplate>
              <div class="row">
                <div class="medium-3 columns">
                  <asp:Label ID="sportLabel" runat="server" AssociatedControlID="teamDDL" />
                </div>
                <div class="medium-9 columns">
                  <asp:DropDownList ID="teamDDL" runat="server" />
                </div>
              </div>
            </ItemTemplate>
            <AlternatingItemTemplate>
              <div class="row ebiz-alternative">
                <div class="medium-3 columns">
                  <asp:Label ID="sportLabel" runat="server" AssociatedControlID="teamDDL" />
                </div>
                <div class="medium-9 columns">
                  <asp:DropDownList ID="teamDDL" runat="server" />
                </div>
              </div>
            </AlternatingItemTemplate>
          </asp:Repeater>
          <div class="row ebiz-contact-method">
            <div class="medium-3 columns">
              <asp:Label ID="contactMethodLabel" runat="server" AssociatedControlID="contactMethodDDL" />
            </div>
            <div class="medium-9 columns">
              <asp:DropDownList ID="contactMethodDDL" runat="server" />
            </div>
          </div>
        </div>
      </asp:PlaceHolder>
      <asp:PlaceHolder ID="plhContactPreferences" runat="server">
        <div class="panel ebiz-contact-preferences">
          <h2>

            <asp:Literal ID="ltlContactPrefsH2" runat="server" />

          </h2>

          <asp:PlaceHolder ID="plhNewsletter" runat="server">
            <div class="ebiz-newsletter">
              <asp:CheckBox ID="Newsletter" runat="server" />
            </div>
            <div class="ebiz-newsletter-options">
              <asp:RadioButtonList ID="NewsletterChoice" runat="server" CssClass="no-bullet ebiz-newsletter-choice" RepeatLayout="UnorderedList"
              />
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhSubscribe2" runat="server">
            <div class="ebiz-subscribe-two">
              <asp:CheckBox ID="Subscribe2" runat="server" />
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhSubscribe3" runat="server">
            <div class="ebiz-subscribe-three">
              <asp:CheckBox ID="Subscribe3" runat="server" />
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhMailOption" runat="server">
            <div class="ebiz-mail-option">
              <asp:CheckBox ID="mailOption" runat="server" />
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhOpt1" runat="server">
            <div class="ebiz-option-one">
              <asp:CheckBox ID="opt1" runat="server" />
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhOpt2" runat="server">
            <div class="ebiz-option-two">
              <asp:CheckBox ID="opt2" runat="server" />
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhOpt3" runat="server">
            <div class="ebiz-option-three">
              <asp:CheckBox ID="opt3" runat="server" />
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhOpt4" runat="server">
            <div class="ebiz-option-four">
              <asp:CheckBox ID="opt4" runat="server" />
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhOpt5" runat="server">
            <div class="ebiz-option-five">
              <asp:CheckBox ID="opt5" runat="server" />
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhOpt5Radio" runat="server">
            <div class="row">
              <div class="column">
                <fieldset class="js-no-icheck o-radio-group-faux-buttons o-radio-group-faux-buttons--gdpr">
                  <legend><asp:Literal ID="lit" runat="server" Text="Option 5" /></legend>
                  <asp:RadioButton ID="rdoopt5No" runat="server" GroupName="opt5" Text="no" CssClass="o-radio-group-faux-buttons__radio  o-radio-group-faux-buttons__opt-out" />
                  <asp:RadioButton ID="rdoopt5Yes" runat="server" GroupName="opt5" Text="Yes" CssClass="o-radio-group-faux-buttons__radio  o-radio-group-faux-buttons__opt-in" />
                </fieldset>
              </div>
            </div>
          </asp:PlaceHolder>

        </div>
      </asp:PlaceHolder>
      <asp:PlaceHolder ID="plhAddMembership" runat="server">
        <div class="panel ebiz-registration-add-membership">
          <asp:CheckBox ID="addMembershipCheck" runat="server" />
        </div>
      </asp:PlaceHolder>
      <asp:PlaceHolder ID="plhRegisteredAddress" runat="server">
        <div class="panel ebiz-delivery-address">
          <h2>

            <asp:Literal ID="ltlRegAddressHdr" runat="server" />

          </h2>
          <div class="row ebiz-registration-address-same">
            <div class="large-12 columns">
              <asp:CheckBox ID="chkRegAddressSame" runat="server" AutoPostBack="True" />
            </div>
          </div>
          <asp:PlaceHolder ID="plhRegAddressLine1Row" runat="server">
            <div class="row ebiz-address-line-one">
              <div class="medium-3 columns">
                <asp:Label ID="lblRegAddress1" runat="server" AssociatedControlID="txtRegAddress1" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="txtRegAddress1" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="txtRegAddress1" ID="rfvTxtRegAddress1" runat="server" OnInit="SetupRegRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="txtRegAddress1" ID="regexTxtRegAddress1" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhRegAddressLine2Row" runat="server">
            <div class="row ebiz-address-line-two">
              <div class="medium-3 columns">
                <asp:Label ID="lblRegAddress2" runat="server" AssociatedControlID="lblRegAddress2" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="txtRegAddress2" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="txtRegAddress2" ID="rfvTxtRegAddress2" runat="server" OnInit="SetupRegRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="txtRegAddress2" ID="regexTxtRegAddress2" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhRegAddressLine3Row" runat="server">
            <div class="row ebiz-address-line-three">
              <div class="medium-3 columns">
                <asp:Label ID="lblRegAddress3" runat="server" AssociatedControlID="txtRegAddress3" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="txtRegAddress3" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="txtRegAddress3" ID="rfvTxtRegAddress3" runat="server" OnInit="SetupRegRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="txtRegAddress3" ID="regexTxtRegAddress3" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhRegAddressLine4Row" runat="server">
            <div class="row ebiz-address-line-four">
              <div class="medium-3 columns">
                <asp:Label ID="lblRegAddress4" runat="server" AssociatedControlID="txtRegAddress4" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="txtRegAddress4" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="txtRegAddress4" ID="rfvTxtRegAddress4" runat="server" OnInit="SetupRegRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="txtRegAddress4" ID="regexTxtRegAddress4" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhRegAddressLine5Row" runat="server">
            <div class="row ebiz-address-line-five">
              <div class="medium-3 columns">
                <asp:Label ID="lblRegAddress5" runat="server" AssociatedControlID="txtRegAddress5" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="txtRegAddress5" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="txtRegAddress5" ID="rfvTxtRegAddress5" runat="server" OnInit="SetupRegRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="txtRegAddress5" ID="regexTxtRegAddress5" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhRegAddressPostcodeRow" runat="server">
            <div class="row ebiz-postcode">
              <div class="medium-3 columns">
                <asp:Label ID="lblRegPostcode" runat="server" AssociatedControlID="txtRegPostcode" />
              </div>
              <div class="medium-9 columns">
                <asp:TextBox ID="txtRegPostcode" runat="server" />
                <asp:RequiredFieldValidator ControlToValidate="txtRegPostcode" ID="rfvTxtRegPostcode" runat="server" OnInit="SetupRegRequiredValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
                <asp:RegularExpressionValidator ControlToValidate="txtRegPostcode" ID="regexTxtRegPostcode" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="plhRegAddressCountryRow" runat="server">
            <div class="row ebiz-country">
              <div class="medium-3 columns">
                <asp:Label ID="lblRegCountry" runat="server" AssociatedControlID="country" />
              </div>
              <div class="medium-9 columns">
                <asp:DropDownList ID="ddlRegCountry" runat="server" />
                <asp:RegularExpressionValidator ControlToValidate="ddlRegCountry" ID="regexDdlRegCountry" runat="server" OnInit="SetupRegExValidator"
                  SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                  CssClass="error ebiz-validator-error" />
              </div>
            </div>
          </asp:PlaceHolder>
        </div>
      </asp:PlaceHolder>
      <asp:PlaceHolder ID="plhUserIDs" runat="server" Visible="false">
        <div class="panel ebiz-user-id">
          <h2>

            <asp:Literal ID="ltlUserIDsHeaderLabel" runat="server" />

          </h2>
          <div class="row ebiz-passport" runat="server" id="passportDiv">
            <div class="medium-3 columns">
              <asp:Label ID="lblPassport" runat="server" AssociatedControlID="txtPassport" />
            </div>
            <div class="medium-9 columns">
              <asp:TextBox ID="txtPassport" runat="server" />
              <asp:Label ID="lblPassportMsg" runat="server" />
            </div>
          </div>
          <div class="row ebiz-green-card" runat="server" id="greencardDiv">
            <div class="medium-3 columns">
              <asp:Label ID="lblGreenCard" runat="server" AssociatedControlID="txtGreenCard" />
            </div>
            <div class="medium-9 columns">
              <asp:TextBox ID="txtGreenCard" runat="server" />
            </div>
          </div>
          <div class="row ebiz-pin" runat="server" id="pinDiv">
            <div class="medium-3 columns">
              <asp:Label ID="lblPin" runat="server" AssociatedControlID="txtPin" />
            </div>
            <div class="medium-9 columns">
              <asp:TextBox ID="txtPin" runat="server" />
              <asp:Label ID="lblGreenCardMsg" runat="server" />
            </div>
          </div>
          <div class="row ebiz-id-four" runat="server" id="userID4Div">
            <div class="medium-3 columns">
              <asp:Label ID="lblUserID4" runat="server" AssociatedControlID="txtUserID4" />
            </div>
            <div class="medium-9 columns">
              <asp:TextBox ID="txtUserID4" runat="server" />
            </div>
          </div>
          <div class="row ebiz-id-five" runat="server" id="userID5Div">
            <div class="medium-3 columns">
              <asp:Label ID="lblUserID5" runat="server" AssociatedControlID="txtUserID5" />
            </div>
            <div class="medium-9 columns">
              <asp:TextBox ID="txtUserID5" runat="server" />
            </div>
          </div>
          <div class="row ebiz-id-six" runat="server" id="userID6Div">
            <div class="medium-3 columns">
              <asp:Label ID="lblUserID6" runat="server" AssociatedControlID="txtUserID6" />
            </div>
            <div class="medium-9 columns">
              <asp:TextBox ID="txtUserID6" runat="server" />
            </div>
          </div>
          <div class="row ebiz-id-seven" runat="server" id="userID7Div">
            <div class="medium-3 columns">
              <asp:Label ID="lblUserID7" runat="server" AssociatedControlID="txtUserID7" />
            </div>
            <div class="medium-9 columns">
              <asp:TextBox ID="txtUserID7" runat="server" />
            </div>
          </div>
          <div class="row ebiz-id-eight" runat="server" id="userID8Div">
            <div class="medium-3 columns">
              <asp:Label ID="lblUserID8" runat="server" AssociatedControlID="txtUserID8" />
            </div>
            <div class="medium-9 columns">
              <asp:TextBox ID="txtUserID8" runat="server" />
            </div>
          </div>
          <div class="row ebiz-id-nine" runat="server" id="userID9Div">
            <div class="medium-3 columns">
              <asp:Label ID="lblUserID9" runat="server" AssociatedControlID="txtUserID9" />
            </div>
            <div class="medium-9 columns">
              <asp:TextBox ID="txtUserID9" runat="server" />
            </div>
          </div>
        </div>
      </asp:PlaceHolder>

      <div class="js--show-parental-consent" runat="server" id="showparentalconsent" style=" display: none; ">
        <div class="panel ebiz-parent-consent">
          <h2>
            <asp:Literal ID="NeedAdultConsentForContactPermissionsHeading" runat="server" />
          </h2>
          <div class="row">
            <div class="panel ebiz-permission-instructions">
              <asp:Label ID="NeedAdultConsentForContactPermisionsInsructions" runat="server" />
            </div>
          </div>

          <asp:PlaceHolder ID="PlhShowPermissionCheckBox" runat="server">
            <div class="ebiz-option-five">
              <asp:CheckBox ID="cbParentalPermissionGranted" runat="server" />
            </div>
          </asp:PlaceHolder>


          <div class="row">
            <div class="medium-3 columns">
              <asp:Label ID="parentemailLabel" runat="server" AssociatedControlID="parentemail" />
            </div>
            <div class="large-3 columns">
              <asp:TextBox ID="parentemail" runat="server" type="email" class="js--Parental-Email" />
              <asp:RequiredFieldValidator ControlToValidate="parentemail" ID="parentemailRFV" runat="server" OnInit="SetupRequiredValidator"
                SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                CssClass="error ebiz-validator-error" class="js--Parents-Email-RFV" />
              <asp:RegularExpressionValidator ControlToValidate="parentemail" ID="parentemailRegEx" runat="server" OnLoad="SetupRegExValidator"
                SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" CssClass="error ebiz-validator-error"
              />
            </div>
          </div>
          <div class="row">
            <div class="medium-3 columns">
              <asp:Label ID="parentphonelabel" runat="server" AssociatedControlID="parentphone" />
            </div>
            <div class="large-3 columns">
              <asp:TextBox ID="parentphone" runat="server" type="tel" class="js--Parental-Phone" />
              <asp:RequiredFieldValidator ControlToValidate="parentphone" ID="parentphoneRFV" runat="server" OnInit="SetupRequiredValidator"
                SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" Text=""
                CssClass="error ebiz-validator-error" class="js--Parents-Phone-RFV" />
              <asp:RegularExpressionValidator ControlToValidate="parentphone" ID="parentphoneRegEx" runat="server" OnLoad="SetupRegExValidator"
                SetFocusOnError="true" Visible="true" ValidationGroup="Registration" Display="Static" Enabled="true" CssClass="error ebiz-validator-error"
              />
            </div>
          </div>
        </div>
      </div>

      <div class="stacked-for-small button-group ebiz-update-form-wrap">
        <asp:PlaceHolder ID="plhPrintAddressLabelBottom" runat="server">
          <asp:Button ID="btnPrintAddressLabelBottom" runat="server" CausesValidation="true" ValidationGroup="Registration" CssClass="button"
          />
        </asp:PlaceHolder>
        <asp:Button ID="updateBtn" runat="server" CausesValidation="true" ValidationGroup="Registration" CssClass="button ebiz-primary-action"
        />
        <asp:Panel runat="server" ID="pnlCapturePhoto">
          <input type="button" class="button ebiz-capture-photo" runat="server" id="capturePhotoBtn" />
        </asp:Panel>
      </div>
      <%  WriteDDLJavascript()%>