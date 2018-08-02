<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ContactUsForm.ascx.vb" Inherits="UserControls_ContactUsForm" %>

<asp:PlaceHolder ID="plhInstructions" runat="server">
    <p class="ebiz-contact-us-instructions"><asp:Literal ID="ContactUsInstructionsLabel" runat="server" /></p>
</asp:PlaceHolder>

<asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList" ValidationGroup="ContactUs" CssClass="alert-box alert" />
<asp:PlaceHolder ID="plhErrorMessage" runat="server">
    <p class="alert-box alert"><asp:Literal ID="ltlErrorMessage" runat="server" /></p>
</asp:PlaceHolder>

<div class="panel ebiz-contact-box">
    <h2><asp:Literal ID="PersonalDetailsLabel" runat="server" /></h2>
    <asp:PlaceHolder ID="plhTitleRow" runat="server">
    <div class="row ebiz-contact-title">
        <div class="large-3 columns">
            <asp:Label ID="titleLabel" runat="server" AssociatedControlID="title" CssClass="middle" />
        </div>
        <div class="large-9 columns">
            <asp:DropDownList ID="title" runat="server" />
            <asp:RegularExpressionValidator ControlToValidate="title" ID="titleRegEx" runat="server" CssClass="error"
                SetFocusOnError="true" Visible="true" ValidationGroup="ContactUs" Display="Static" Enabled="true" />
        </div>
    </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhForenameRow" runat="server">
    <div class="row ebiz-contact-forename">
        <div class="large-3 columns">
            <asp:Label ID="forenameLabel" runat="server" AssociatedControlID="forename" CssClass="middle" />
        </div>
        <div class="large-9 columns">
            <asp:TextBox ID="forename" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="forename" ID="forenameRFV" runat="server" CssClass="error"
                SetFocusOnError="true" Visible="true" ValidationGroup="ContactUs" Display="Static" Enabled="true" />
            <asp:RegularExpressionValidator ControlToValidate="forename" ID="forenameRegEx" runat="server" CssClass="error"
                SetFocusOnError="true" Visible="true" ValidationGroup="ContactUs" Display="Static" Enabled="true" />
        </div>
    </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhSurnameRow" runat="server">
    <div class="row ebiz-contact-surname">
        <div class="large-3 columns">
            <asp:Label ID="surnameLabel" runat="server" AssociatedControlID="surname" CssClass="middle" />
        </div>
        <div class="large-9 columns">
            <asp:TextBox ID="surname" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="surname" ID="surnameRFV" runat="server" CssClass="error"
                SetFocusOnError="true" Visible="true" ValidationGroup="ContactUs" Display="Static" Enabled="true"/>
            <asp:RegularExpressionValidator ControlToValidate="surname" ID="surnameRegEx" runat="server" CssClass="error"
                SetFocusOnError="true" Visible="true" ValidationGroup="ContactUs" Display="Static" Enabled="true" />
        </div>
    </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhEmailRow" runat="server">
    <div class="row ebiz-contact-email">
        <div class="large-3 columns">
            <asp:Label ID="emailLabel" runat="server" AssociatedControlID="email" CssClass="middle" />
        </div>
        <div class="large-9 columns">
            <asp:TextBox ID="email" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="email" ID="emailRFV" runat="server" CssClass="error"
                SetFocusOnError="true" Visible="true" ValidationGroup="ContactUs" Display="Static" Enabled="true" />
            <asp:RegularExpressionValidator ControlToValidate="email" ID="emailRegEx" runat="server" CssClass="error"
                SetFocusOnError="true" Visible="true" ValidationGroup="ContactUs" Display="Static" Enabled="true" />
        </div>
    </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhPhoneRow" runat="server">
    <div class="row ebiz-contact-phone">
        <div class="large-3 columns">
            <asp:Label ID="phoneLabel" runat="server" AssociatedControlID="phone" CssClass="middle" />
        </div>
        <div class="large-9 columns">
            <asp:TextBox ID="phone" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="phone" ID="phoneRFV" runat="server" CssClass="error"
                SetFocusOnError="true" Visible="true" ValidationGroup="ContactUs" Display="Static" Enabled="true" />
            <asp:RegularExpressionValidator ControlToValidate="phone" ID="phoneRegEx" runat="server" CssClass="error"
                SetFocusOnError="true" Visible="true" ValidationGroup="ContactUs" Display="Static" Enabled="true" />
        </div>
    </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhWorkRow" runat="server">
    <div class="row ebiz-contact-work">
        <div class="large-3 columns">
            <asp:Label ID="workLabel" runat="server" AssociatedControlID="work" CssClass="middle" />
        </div>
        <div class="large-9 columns">
            <asp:TextBox ID="work" runat="server" />
            <asp:RegularExpressionValidator ControlToValidate="work" ID="workRegEx" runat="server" CssClass="error"
                SetFocusOnError="true" Visible="true" ValidationGroup="ContactUs" Display="Static" Enabled="true" />
        </div>
    </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhMobileRow" runat="server">
    <div class="row ebiz-contact-mobile">
        <div class="large-3 columns">
            <asp:Label ID="mobileLabel" runat="server" AssociatedControlID="mobile" CssClass="middle" />
        </div>
        <div class="large-9 columns">
            <asp:TextBox ID="mobile" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="phone" ID="mobileRFV" runat="server"
                SetFocusOnError="true" Visible="true" ValidationGroup="ContactUs" Display="Static" CssClass="error" Enabled="true" />
            <asp:RegularExpressionValidator ControlToValidate="mobile" ID="mobileRegEx" runat="server" CssClass="error"
                SetFocusOnError="true" Visible="true" ValidationGroup="ContactUs" Display="Static" Enabled="true" />
        </div>
    </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhCategoryRow" runat="server">
    <div class="row ebiz-contact-category">
        <div class="large-3 columns">
            <asp:Label ID="categoryLabel" runat="server" AssociatedControlID="category" CssClass="middle" />
        </div>
        <div class="large-9 columns">
            <asp:DropDownList ID="category" CssClass="select" runat="server" />
            <asp:RegularExpressionValidator ControlToValidate="category" ID="categoryRegEx" runat="server" CssClass="error"
                SetFocusOnError="false" Visible="true" ValidationGroup="ContactUs" Display="Static" Enabled="true"/>
        </div>
    </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhMessageRow" runat="server">
    <div class="row ebiz-contact-message">
        <div class="large-3 columns">
            <asp:Label ID="messageLabel" runat="server" AssociatedControlID="message" CssClass="middle" />
        </div>
        <div class="large-9 columns">
            <asp:TextBox ID="message" runat="server" TextMode="MultiLine" CssClass="ebiz-contact-us-message-box" />
            <asp:RequiredFieldValidator ControlToValidate="message" ID="messageRFV" runat="server" CssClass="error"
                SetFocusOnError="true" Visible="true" ValidationGroup="ContactUs" Display="Static" Enabled="true" />
        </div>
    </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhFlagRow" runat="server">
    <div class="row ebiz-contact-flag">
        <div class="large-12 columns">
            <asp:CheckBox ID="flagCheckbox" runat="server" />
        </div>
    </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhPrivacyRow" runat="server">
    <div class="row ebiz-contact-privacy">
        <div class="large-12 columns">
            <asp:Literal ID="privacyLabel1" runat="server" />
            <asp:HyperLink ID="privacyLink" runat="server" />
            <asp:Literal ID="privacyLabel2" runat="server"  />
        </div>
    </div>
    </asp:PlaceHolder>
</div>

<asp:Button ID="submitBtn" CssClass="button" runat="server" CausesValidation="true" ValidationGroup="ContactUs" />