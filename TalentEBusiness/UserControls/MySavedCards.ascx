<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MySavedCards.ascx.vb" Inherits="UserControls_MySavedCards" %>
<%@ Reference Control="~/UserControls/MarketingCampaigns.ascx" %>
<%@ Reference Control="~/UserControls/CheckoutPartPayments.ascx" %>

<asp:PlaceHolder ID="plhMySavedCards" runat="server">
    <div class="panel">
        <asp:PlaceHolder ID="plhMySavedCardsIntroText" runat="server">
            <p class="ebiz-my-saved-cards-intro-text">
                <asp:Literal ID="ltlMySavedCardsIntroText" runat="server" /></p>
        </asp:PlaceHolder>
        <div class="row ebiz-saved-card-list">
            <div class="medium-3 columns">
                <asp:Label ID="lblSavedCardList" AssociatedControlID="ddlSavedCardList" runat="server" />
            </div>
            <div class="medium-9 columns">
                <asp:DropDownList ID="ddlSavedCardList" runat="server" ViewStateMode="Enabled" />
            </div>
        </div>
        <asp:PlaceHolder ID="plhButtonGroup" runat="server">
            <div class="ebiz-my-saved-cards-buttons button-group">
                <asp:PlaceHolder ID="plhDeleteButton" runat="server" Visible="false">
                    <asp:Button ID="btnDeleteThisCard" runat="server" CssClass="button" />
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhDefaultButton" runat="server" Visible="false">
                    <asp:Button ID="btnSetThisCardAsDefault" runat="server" CssClass="button" />
                </asp:PlaceHolder>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhSecurityNumber" runat="server" Visible="false">
            <div class="row ebiz-card-security-number">
                <div class="medium-3 columns">
                    <asp:Label ID="lblCardSecurityNumber" runat="server" AssociatedControlID="txtCardSecurityNumber" />
                </div>
                <div class="medium-2 columns">
                    <asp:TextBox ID="txtCardSecurityNumber" runat="server" SkinID="CreditCardCV2" ViewStateMode="Enabled" AutoCompleteType="Disabled" type="tel" min="0" />
                </div>
                <div class="medium-7 columns">
                    <asp:Image ID="imgSecurityNumber" runat="server" SkinID="CV2" MaxLength="4" />
                </div>
            </div>
            <div class="row ebiz-card-csv-error">
                <div class="columns large-12">
                    <asp:RequiredFieldValidator ID="rfvSecurityNumber" runat="server" SkinID="CreditCard" ValidationGroup="Checkout" ControlToValidate="txtCardSecurityNumber" CssClass="error" />
                    <asp:RegularExpressionValidator ID="rgxSecurityNumber" runat="server" SkinID="CreditCard" ValidationGroup="Checkout" ControlToValidate="txtCardSecurityNumber" CssClass="error" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhCaptureMethod" runat="server" Visible="false">
            <div class="row ebiz-capture-method">
                <div class="medium-3 columns">
                    <asp:Label ID="lblCaptureMethod" runat="server" AssociatedControlID="ddlCaptureMethod" />
                </div>
                <div class="medium-4 columns">
                    <asp:DropDownList ID="ddlCaptureMethod" runat="server" AutoPostBack="false" ViewStateMode="Enabled" />
                </div>
                <div class="medium-5 columns">&nbsp;</div>
            </div>
        </asp:PlaceHolder>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhNoSavedCards" runat="server">
    <div class="alert-box info">
        <asp:Literal ID="ltlNoSavedCards" runat="server" />
    </div>
</asp:PlaceHolder>
