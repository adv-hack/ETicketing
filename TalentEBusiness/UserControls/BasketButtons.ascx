<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BasketButtons.ascx.vb" Inherits="UserControls_BasketButtons" %>
<%@ Reference Control="~/UserControls/BasketDetails.ascx" %>
<%@ Reference Control="~/UserControls/ticketingBasketDetails.ascx" %>
<%@ Reference Control="~/UserControls/summaryTotals.ascx" %>
<%@ Reference Control="~/usercontrols/htmlinclude.ascx" %>

<script>
   
    $(document).ready(function () {
        if ($("#hdfIsReserveButtonEnabled").val() == "False") {
            $(".ebiz-basket-reserve-all-button").removeClass("has-tip top").addClass("ebiz-muted-action");
            $(".ebiz-basket-reserve-all-button").attr("title", document.getElementById("hdfCantReserveBasketText").value);
            $(".ebiz-basket-reserve-all-button").removeAttr("data-open");
            var elem = new Foundation.Tooltip($(".ebiz-basket-reserve-all-button"));
        }
    });
</script>
<div class="stacked-for-small button-group ebiz-basket-buttons-wrap">    
    <asp:HyperLink ID="ReserveAllButton" CssClass="button ebiz-basket-reserve-all-button" runat="server" OnPreRender="GetText" Visible="false" data-open="reserve-button" />
    <asp:HyperLink ID="UnReserveAllButton" CssClass="button ebiz-basket-unreserve-all-button" runat="server" OnPreRender="GetText" Visible="false" data-open="unreserve-button"  />
    <asp:Button ID="ClearBasketButton" CssClass="button ebiz-basket-clear-button" runat="server" OnPreRender="GetText" />
    <asp:Button ID="OtherMatchesButton" CssClass="button ebiz-basket-other-matches-button" runat="server" OnPreRender="GetText" />
    <asp:Button ID="AddLinkRegButton" CssClass="button ebiz-basket-ff-button" runat="server" OnPreRender="GetText" />
    <asp:Button ID="ContinueShoppingButton" CssClass="button ebiz-basket-continue-shopping" runat="server" OnPreRender="GetText" />
    <asp:Button ID="AddAnotherFixtureButton" CssClass="button ebiz-basket-another-fixture" runat="server" OnPreRender="GetText" />
    <asp:PlaceHolder ID="plhSaveOrders" runat="server">
        <asp:Button ID="SaveOrderButton" CssClass="button ebiz-basket-save-order" runat="server" OnPreRender="GetText" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhOrderTemplates" runat="server">
        <asp:Button ID="SaveTemplateButton" CssClass="button ebiz-basket-save-template" runat="server" OnPreRender="GetText" />
    </asp:PlaceHolder>
    <asp:Button ID="UpdateBasketButton" CssClass="button ebiz-basket-update-button" runat="server" OnPreRender="GetText" CausesValidation="true" ValidationGroup="Basket" />
    <asp:Button ID="CheckoutButton" CssClass="button ebiz-primary-action ebiz-basket-checkout-button" runat="server" OnPreRender="GetText" CausesValidation="true" ValidationGroup="Basket" />
    <asp:Button ID="FastCashButton" CssClass="button ebiz-basket-fastcash-button" runat="server" OnPreRender="GetText" CausesValidation="true" ValidationGroup="Basket" />

    <asp:HiddenField ID="hdfIsReserveButtonEnabled" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfCantReserveBasketText" runat="server" ClientIDMode="Static" />
</div>
