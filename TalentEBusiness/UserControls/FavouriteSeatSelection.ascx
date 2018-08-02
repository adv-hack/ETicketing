<%@ Control Language="VB" AutoEventWireup="false" CodeFile="FavouriteSeatSelection.ascx.vb" Inherits="UserControls_FavouriteSeatSelection" ViewStateMode="Disabled" %>

<asp:Panel ID="pnlFavouriteSeat" runat="server" ViewStateMode="Disabled" DefaultButton="btnFavouriteSeatBuy">
    <div class="panel ebiz-favourite-seat-selection <% =CSSClassName %>">
        <asp:PlaceHolder ID="plhHeader" runat="server">
            <h2><asp:Literal ID="ltlFavouriteSeatHeader" runat="server" /></h2>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhFavouriteSeatAvailable" runat="server">
            <asp:PlaceHolder ID="plhInstructions" runat="server">
                <p class="ebiz-favourite-seat-intructions"><asp:Literal ID="ltlFavouriteSeatInstructions" runat="server" /></p>
            </asp:PlaceHolder>
            <fieldset>
                <legend><asp:Literal ID="ltlFavouriteSeatLegend" runat="server" /></legend>
                <div class="row ebiz-favourite-seat-quantity">
                    <div class="medium-3 columns">
                        <asp:Label ID="lblFavouriteSeatQuantity" runat="server" AssociatedControlID="txtFavouriteSeatQuantity" CssClass="middle" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtFavouriteSeatQuantity" runat="server" type="number" />
                        <asp:RangeValidator ID="rngQuantity" runat="server" ControlToValidate="txtFavouriteSeatQuantity" Display="None" CssClass="error" SetFocusOnError="true" ValidationGroup="FavSeatQuantity" EnableClientScript="true" />
                        <asp:RequiredFieldValidator ID="rfvQuantity" runat="server" Display="None" SetFocusOnError="true" ControlToValidate="txtFavouriteSeatQuantity" CssClass="error" ValidationGroup="FavSeatQuantity" />
                    </div>
                </div>
                <div class="ebiz-favourite-seat-buy-wrap">
                    <asp:Button ID="btnFavouriteSeatBuy" runat="server" CssClass="button ebiz-primary-action" CausesValidation="true" ValidationGroup="FavSeatQuantity" />
                </div>
            </fieldset>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhFavouriteSeatNotAvailable" runat="server">
            <p class="ebiz-no-favourite-seat"><asp:Literal ID="ltlFavouriteSeatNotAvailable" runat="server" /></p>
        </asp:PlaceHolder>
    </div>
    <asp:HiddenField ID="hfProductType" runat="server" />
    <asp:HiddenField ID="hfProductCode" runat="server" />
    <asp:HiddenField ID="hfProductSubType" runat="server" />
    <asp:HiddenField ID="hfProductStadium" runat="server" />
    <asp:HiddenField ID="hfProductPriceBand" runat="server" />
    <asp:HiddenField ID="hfCampaignCode" runat="server" />
    <asp:HiddenField ID="hfProductHomeAsAway" runat="server" />
</asp:Panel>
