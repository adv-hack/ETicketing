<%@ Control Language="VB" AutoEventWireup="false" CodeFile="FriendsAndFamilyOptions.ascx.vb" Inherits="UserControls_FriendsAndFamilyOptions" ViewStateMode="Disabled" %>

<asp:PlaceHolder ID="plhErrorMessage" runat="server">
    <p class="alert-box alert"><asp:Literal ID="ltlErrorLabel" runat="server" /></p>
</asp:PlaceHolder>
<asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="FandF" CssClass="alert-box alert" DisplayMode="BulletList" />

<asp:PlaceHolder ID="plhSuccessMessage" runat="server">
    <p class="alert-box success"><asp:Literal ID="ltlAddFFSuccessLabel" runat="server" /></p>
</asp:PlaceHolder>

<div class="panel ebiz-ff-options">
    <h2><asp:Literal ID="ltlSubHeader" runat="server" /></h2>
    <div class="row ebiz-ff-customer-number">
        <div class="medium-3 columns">
            <asp:Label ID="CustomerNumberLabel" runat="server" AssociatedControlID="CustomerNumberTextBox" />
        </div>
        <div class="medium-9 columns">
            <asp:TextBox ID="CustomerNumberTextBox" runat="server" type="number" min="1" />
            <asp:RequiredFieldValidator ID="CustomerNumberRFV" runat="server" ControlToValidate="CustomerNumberTextBox" ValidationGroup="FandF" 
                CssClass="error ebiz-validator-error" SetFocusOnError="true" Display="Static" />
        </div>
    </div>
    <div class="row ebiz-ff-customer-surname">
        <div class="medium-3 columns">
            <asp:Label ID="CustomerSurnameLabel" runat="server" AssociatedControlID="CustomerSurnameTextBox" />
        </div>
        <div class="medium-9 columns">
            <asp:TextBox ID="CustomerSurnameTextBox" runat="server" />
            <asp:RequiredFieldValidator ID="CustomerSurNameRFV" runat="server" ControlToValidate="CustomerSurnameTextBox" ValidationGroup="FandF" 
                CssClass="error ebiz-validator-error" SetFocusOnError="true" Display="Static" />
        </div>
    </div>
    <asp:PlaceHolder ID="plhPostCode" runat="server" Visible="true">
        <div class="row ebiz-ff-customer-postcode">
            <div class="medium-3 columns">
                <asp:Label ID="CustomerPostCodeLabel" runat="server" AssociatedControlID="CustomerPostCodeTextBox" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="CustomerPostCodeTextBox" runat="server" />
                <asp:RequiredFieldValidator ID="CustomerPostCodeRFV" runat="server" ControlToValidate="CustomerPostCodeTextBox" ValidationGroup="FandF" 
                    CssClass="error ebiz-validator-error" SetFocusOnError="true" Display="Static" />
            </div>
        </div>
    </asp:PlaceHolder>
    <div class="ebiz-ff-buttons">
        <div class="button-group ebiz-ff-buttons-wrap">
            <asp:PlaceHolder ID="plhAddMyFF" runat="server">
                <asp:Button ID="AddMyFFButton" CssClass="button ebiz-add-ff-to-me" runat="server" CausesValidation="true" ValidationGroup="FandF" />
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAddTheirsFF" runat="server">
                <asp:Button ID="AddTheirFFButton" CssClass="button ebiz-add-me-to-ff" runat="server" CausesValidation="true" ValidationGroup="FandF" />
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhRegisterCustomer" runat="server">
                <asp:Button ID="RegisterCustomerButton"  CausesValidation="false" CssClass="button ebiz-register-customer" runat="server" />
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhWishList" runat="server">
                <asp:HyperLink ID="hplWishList" runat="server" CssClass="button ebiz-ff-wishlist" NavigateUrl="~/PagesLogin/Template/ViewTemplates.aspx" />
            </asp:PlaceHolder>
        </div>
    </div>
</div>