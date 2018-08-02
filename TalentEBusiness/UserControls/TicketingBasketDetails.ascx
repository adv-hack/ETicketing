<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TicketingBasketDetails.ascx.vb" Inherits="UserControls_TicketingBasketDetails" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/Package/PackageSummary.ascx" TagName="PackageSummary" TagPrefix="Talent" %>

<h2>
    <asp:Literal ID="ltlTicketingBasketHeaderLabel" runat="server" /></h2>

<asp:PlaceHolder ID="plhMultiStadiumText" runat="server">
    <p class="ebiz-multi-stadium-text">
        <asp:Literal ID="ltlMultiStadiumMainTextLabel" runat="server" />
    </p>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plhCustomerSearch" runat="server">
    <p class="ebiz-customer-search-link">
        <asp:HyperLink ID="hlkCustomerSearch" runat="server" Visible="false" class="button"></asp:HyperLink>
    </p>
</asp:PlaceHolder>
<script>

    var updateBasketUsed = false;


    $(document).ready(function () {
        //In BoxOffice enable dynamic option creation by customer number
        if ($("#hdfIsAgent").val() == "True") {
            $(".js-select2-tags").select2({
                tags: true
            });
        }
    });

    function EnablePriceCodeDropdown(priceCodeDDLID) {
        document.getElementById(priceCodeDDLID).disabled = false;
    }

    function highlightUpdateBasket() {
        var highlightUpdateBasketText = document.getElementById("hdfHighlightUpdateBasketText").value;
        var UpdateBasketPromptDuration = document.getElementById("hdfUpdateBasketPromptDuration").value;
        var ShowUpateBasketPrompt = (document.getElementById("hdfShowUpdateBasketPrompt").value.toLowerCase() == "true");
        var highlightUpdateBasketButtonWhenRequired = (document.getElementById("hdfHighlightUpdateBasketButtonWhenRequired").value.toLowerCase() == "true");
        if (ShowUpateBasketPrompt && !updateBasketUsed) {
            alertify.notify(highlightUpdateBasketText, '-basket-needs-updating', UpdateBasketPromptDuration);
            updateBasketUsed = true;
        }
        if (highlightUpdateBasketButtonWhenRequired) {
            $(".ebiz-basket-buttons-wrap").addClass("ebiz-basket-buttons-wrap--basket-needs-updating");
        }
    };

    function optionsUpdated() {
        var hdfOptionsUpdated = document.getElementById("hdfOptionsUpdated");
        hdfOptionsUpdated.value = 'Y';
        highlightUpdateBasket();
    }
    function CustomerDDLRedirect(ddlClientID, URLClientIDNC, URLClientIDFF) {
        var custDDL = document.getElementById(ddlClientID);
        if (custDDL != null) {
            var hdfOptionsUpdated = document.getElementById("hdfOptionsUpdated");
            var selectedIndex = custDDL.selectedIndex;
            var custDLLText = custDDL.options[selectedIndex].text;
            var isCustomerNumber = /^\d+$/.test(custDLLText);

            if (isCustomerNumber) {
                var basketDetailId = $('#' + ddlClientID).prev().val();
                document.getElementById("hdfSearchNewCustomer").value = "true";
                document.getElementById("hdfNewCustomerNumber").value = custDLLText;
                document.getElementById("hdfSelectedBasketDetailId").value = basketDetailId;
                $("#form1").submit();
            }
            else {
                highlightUpdateBasket();
                var doRedirect = false;
                if (custDLLText == '<%=NewCustomerOptionText%>' && hdfOptionsUpdated.value == 'Y') {
                    //show message
                    firstCall = true;
                    if (confirm('<%=NewCustomerConfirmation%>'))
                        doRedirect = true;
                    else
                        doRedirect = false;
                }
                if (custDLLText == '<%=NewCustomerOptionText%>') {
                    doRedirect = true;
                }
                if (custDLLText == '<%=FriendsAndFamilyRegOption%>') {
                    doRedirect = true;
                }
                if (doRedirect) {
                    if (URLClientIDFF.length > 0 && custDDL.options[custDDL.selectedIndex].value == URLClientIDFF) {
                        location = URLClientIDFF;
                    }
                    if (URLClientIDNC.length > 0 && custDDL.options[custDDL.selectedIndex].value == URLClientIDNC) {
                        location = URLClientIDNC;
                    }
                }

            }
        }
    }

</script>

<asp:HiddenField ClientIDMode="Static" ID="hdfHighlightUpdateBasketText" runat="server" />
<asp:HiddenField ClientIDMode="Static" ID="hdfUpdateBasketPromptDuration" runat="server" />
<asp:HiddenField ClientIDMode="Static" ID="hdfHighlightUpdateBasketButtonWhenRequired" runat="server" />
<asp:HiddenField ClientIDMode="Static" ID="hdfShowUpdateBasketPrompt" runat="server" />
<asp:HiddenField ClientIDMode="Static" ID="hdfIsAgent" runat="server" />

<asp:PlaceHolder ID="plhOrderLevel" runat="server">
    <div class="alert-box info ebiz-order-fulfilment">
        <div class="ebiz-order-fulfilment-inner-wrap">
            <asp:PlaceHolder ID="plhOrderFulfilment" runat="server">
                <div class="ebiz-order-fulfilment-label-wrap">
                    <asp:Label ID="lblOrderFulfilment" runat="server" AssociatedControlID="ddlOrderFulfilment" />
                </div>
                <div class="ebiz-order-fulfilment-select-wrap">
                    <asp:DropDownList ID="ddlOrderFulfilment" runat="server" onChange="optionsUpdated()" />
                    <asp:RequiredFieldValidator ID="rfvOrderFulfilment" runat="server" ValidationGroup="Basket" ControlToValidate="ddlOrderFulfilment" Display="None" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhManualIntervention" runat="server">
                <div class="ebiz-order-fulfilment-checkbox-wrap">
                    <asp:CheckBox ID="chkManualIntervention" runat="server" />
                    <asp:Label ID="lblManualIntervention" runat="server" AssociatedControlID="chkManualIntervention" />
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:PlaceHolder>

<asp:Repeater ID="EventRepeater" runat="server">
    <ItemTemplate>
        <div id="divEventRepeater" runat="server">
            <div class="ebiz-header">
                <div class="ebiz-product-detail">
                    <asp:Image ID="EventImage" runat="server" />
                    <div class="ebiz-basket-product-descriptions">
                        <asp:PlaceHolder ID="plhDescription1" runat="server">
                            <h2>
                                <asp:Literal ID="ltlDescription1" runat="server" /></h2>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhDescription2" runat="server">
                            <div class="ebiz-description-2">
                                <asp:Literal ID="ltlDescription2" runat="server" />
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhDescription3" runat="server">
                            <div class="ebiz-description-3">
                                <asp:Literal ID="ltlDescription3" runat="server" />
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhDescription4" runat="server">
                            <div class="ebiz-description-4">
                                <asp:Literal ID="ltlDescription4" runat="server" />
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhDescription5" runat="server">
                            <div class="ebiz-description-5">
                                <asp:Literal ID="ltlDescription5" runat="server" />
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhSpecificContent1" runat="server">
                            <div class="ebiz-specific-content-1">
                                <asp:Literal ID="ltlSpecificContent1" runat="server" />
                            </div>
                        </asp:PlaceHolder>
                    </div>
                </div>
            </div>
            <asp:PlaceHolder ID="plhMoreTickets" runat="server">
                <div class="ebiz-more-tickets">
                    <asp:HyperLink ID="MoreTicketsLink" runat="server" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phEditParticipantsLink" runat="server">
                <div class="ebiz-edit-participants">
                    <asp:HyperLink ID="lnkEditParticipantsLink" runat="server" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhSeasonTicketExceptionsLink" runat="server">
                <div class="ebiz-season-ticket-exceptions-link">
                    <asp:HyperLink ID="hplSeasonTicketExceptions" runat="server" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhFulfilment" runat="server">
                <div class="alert-box info ebiz-product-fulfilment">
                    <div class="row">
                        <div class="medium-3 columns">
                            <asp:Label ID="lblEventFulfilment" runat="server" AssociatedControlID="EventFulfilmentDDL" />
                        </div>
                        <div class="medium-9 columns">
                            <asp:DropDownList ID="EventFulfilmentDDL" runat="server" onChange="optionsUpdated()" />
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:Repeater ID="StandRepeater" runat="server" OnItemDataBound="DoStandItemDataBound">
                <ItemTemplate>
                    <asp:PlaceHolder ID="plhStandDetails" runat="server">
                        <div class="row ebiz-stand-information">
                            <div class="columns">
                                <asp:Literal ID="ltlStandPreLabel" runat="server" />
                            </div>
                            <div class="columns">
                                <asp:Literal ID="ltlStandLabel" runat="server" />
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="plhPackageDetails" runat="server">
                        <div class="row ebiz-package-information">
                            <div class="columns">
                                <asp:HyperLink ID="lnkpackageBooking" runat="server" />
                            </div>
                        </div>
                        <asp:PlaceHolder ID="plhPackageComments" runat="server">
                            <p class="ebiz-package-comments">
                                <asp:Literal ID="ltlPackageCommentsLabel" runat="server" />
                            </p>
                        </asp:PlaceHolder>
                    </asp:PlaceHolder>

                    <asp:Repeater ID="AreaRepeater" runat="server" OnItemDataBound="DoAreaItemDataBound">
                        <ItemTemplate>
                            <asp:PlaceHolder ID="plhAreaDetails" runat="server">
                                <div class="row ebiz-area-information">
                                    <div class="columns">
                                        <asp:Literal ID="ltlAreaPreLabel" runat="server" />
                                    </div>
                                    <div class="columns">
                                        <asp:Literal ID="ltlAreaLabel" runat="server" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>

                            <Talent:PackageSummary ID="uscPackageSummary" runat="server" Display="false" />

                            <asp:Repeater ID="SeatsRepeater" runat="server" OnItemDataBound="DoSeatsItemDataBound">
                                <HeaderTemplate>
                                    <table class="ebiz-ticketing-basket">
                                        <thead>
                                            <tr>
                                                <th scope="col" class="ebiz-customer"><%# GetTextTable("CustomerHeaderLabel")%></th>
                                                <th scope="col" id="priceCodeCol" runat="server" onprerender="PriceCodeColumnPreRender" class="ebiz-price-code">
                                                    <%# GetTextTable("PriceCodeHeaderLabel")%>
                                                    <asp:CheckBox ID="chkOverridePromotionPriceCode" runat="server" OnPreRender="OverridePromotionCheckBoxPreRender" />
                                                </th>
                                                <th scope="col" id="bandCol" runat="server" onprerender="PriceBandColumnPreRender" class="ebiz-price-band">
                                                    <%# GetTextTable("BandHeaderLabel")%>
                                                </th>
                                                <th scope="col" id="fulfilmentCol" runat="server" onprerender="FulfilmentColumnPreRender" class="ebiz-fulfilment">
                                                    <%# GetTextTable("FulfilmentHeaderLabel")%>
                                                </th>
                                                <th scope="col" id="seatCol" runat="server" onprerender="SeatColumnPreRender" class="ebiz-seat">
                                                    <asp:Literal ID="ltlSeatHeaderLabel" runat="server" OnLoad="GetText" />
                                                </th>
                                                <th scope="col" id="bulkSalesQuantityCol" runat="server" onprerender="BulkSalesQuantityColumnPreRender" class="ebiz-bulk-quantity">
                                                    <%# GetTextTable("BulkSalesQuantityHeaderLabel")%>
                                                </th>
                                                <th scope="col" id="packageQuantityCol" runat="server" onprerender="PackageColumnsPreRender" class="ebiz-package-quantity">
                                                    <%# GetTextTable("PackageQuantityHeaderLabel")%>
                                                </th>
                                                <th scope="col" id="netPriceCol" runat="server" onprerender="PackageColumnsPreRender" class="ebiz-net-price">
                                                    <%# GetTextTable("netPriceHeaderLabel")%>
                                                </th>
                                                <th scope="col" class="ebiz-price">
                                                    <%# GetTextTable("PriceHeaderLabel")%>
                                                </th>
                                                <th scope="col" id="removeCol" runat="server" class="ebiz-remove"></th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td data-title='<%#GetTextTable("CustomerHeaderLabel") %>' class="ebiz-customer" runat="server">
                                            <asp:HiddenField ID="hdfBasketDetailID" runat="server" />
                                            <asp:DropDownList ID="CustomerDDL" runat="server" class="js-select2-tags" />
                                            <asp:HiddenField ID="hidFFRegURL" runat="server" ClientIDMode="Static" />
                                            <asp:HiddenField ID="hdfNewCustomerURL" runat="server" ClientIDMode="Static" />
                                            <asp:Label ID="customerLabel" runat="server" Visible="false"></asp:Label>
                                            <asp:Label ID="ProductCode" runat="server" Visible="false"></asp:Label>
                                            <asp:Label ID="OriginalCustomer" runat="server" Visible="false"></asp:Label>
                                            <asp:Label ID="SeatMessage" runat="server" Visible="false"></asp:Label>
                                            <asp:Label ID="PriceCode" runat="server" Visible="false"></asp:Label>
                                            <asp:Label ID="ProductType" runat="server" Visible="false"></asp:Label>
                                            <asp:HyperLink ID="LoginLink1" runat="server"></asp:HyperLink>
                                            <asp:Label ID="NewParticipantLabel" runat="server" Visible="false"></asp:Label>
                                        </td>
                                        <td id="priceCodeCol" runat="server" data-title='<%#GetTextTable("PriceCodeHeaderLabel") %>' class="ebiz-price-code">
                                            <asp:DropDownList ID="PriceCodeDDL" runat="server">
                                            </asp:DropDownList>
                                            <asp:Label ID="OriginalPriceCode" runat="server" Visible="false"></asp:Label>
                                            <asp:Label ID="lblPriceCodeDescription" runat="server" Visible="false" />
                                        </td>
                                        <td id="bandCol" runat="server" data-title='<%#GetTextTable("BandHeaderLabel") %>' class="ebiz-price-band">
                                            <asp:DropDownList ID="PriceBandDDL" runat="server" />
                                            <asp:Label ID="BandMessage" runat="server" Visible="false"></asp:Label>
                                            <asp:Label ID="OriginalBand" runat="server" Visible="false"></asp:Label>
                                            <asp:HyperLink ID="LoginLink2" runat="server"></asp:HyperLink>
                                        </td>
                                        <td id="fulfilmentCol" runat="server" data-title='<%#GetTextTable("FulfilmentHeaderLabel") %>' class="ebiz-fulfilment">
                                            <asp:Label ID="ProductTypeMTTP" runat="server" Visible="false"></asp:Label>
                                            <asp:Label ID="OriginalFulfilmentCode" runat="server" Visible="false"></asp:Label>
                                            <asp:Label ID="OriginalFulfilment" runat="server" Visible="true"></asp:Label>
                                        </td>
                                        <td id="seatCol" runat="server" data-title='<%#GetTextTable("SeatHeaderLabel") %>' class="ebiz-seat">
                                            <asp:Literal ID="TravelProductLabel" runat="server" Visible="False" />
                                            <asp:Literal ID="SeatLabel" runat="server" />
                                            <asp:HiddenField ID="hdSeatLabel" runat="server"></asp:HiddenField>
                                            <asp:HiddenField ID="hdSeatLabel2" runat="server"></asp:HiddenField>
                                            <asp:Label ID="RestrictedSeat" runat="server"></asp:Label>
                                            <asp:PlaceHolder ID="plhAllocatedSeat" runat="server" Visible="false">
                                                <asp:TextBox ID="txtAllocatedStand" runat="server" CssClass="ebiz-allocated-stand" MaxLength="3" />
                                                <asp:TextBox ID="txtAllocatedArea" runat="server" CssClass="ebiz-allocated-area" MaxLength="4" />
                                                <asp:TextBox ID="txtAllocatedRow" runat="server" CssClass="ebiz-allocated-row" MaxLength="4" />
                                                <asp:TextBox ID="txtAllocatedSeatNumber" runat="server" CssClass="ebiz-allocated-seat-number" MaxLength="4" />
                                                <asp:TextBox ID="txtAllocatedSeatSuffix" runat="server" CssClass="ebiz-allocated-seat-suffix" MaxLength="1" />
                                                <asp:Literal ID="ltlAllocatedSeat" runat="server" />
                                                <asp:RegularExpressionValidator ID="rgxAllocatedStand" runat="server" ValidationGroup="Basket" ControlToValidate="txtAllocatedStand" ValidationExpression="^[a-zA-Z0-9\s]{0,3}$" Display="None" SetFocusOnError="true" />
                                                <asp:RegularExpressionValidator ID="rgxAllocatedArea" runat="server" ValidationGroup="Basket" ControlToValidate="txtAllocatedArea" ValidationExpression="^[a-zA-Z0-9\s]{0,4}$" Display="None" SetFocusOnError="true" />
                                                <asp:RegularExpressionValidator ID="rgxAllocatedRow" runat="server" ValidationGroup="Basket" ControlToValidate="txtAllocatedRow" ValidationExpression="^[a-zA-Z0-9\s]{0,4}$" Display="None" SetFocusOnError="true" />
                                                <asp:RegularExpressionValidator ID="rgxAllocatedSeatNumber" runat="server" ValidationGroup="Basket" ControlToValidate="txtAllocatedSeatNumber" ValidationExpression="^[a-zA-Z0-9\s]{0,4}$" Display="None" SetFocusOnError="true" />
                                                <asp:RegularExpressionValidator ID="rgxAllocatedSeatSuffix" runat="server" ValidationGroup="Basket" ControlToValidate="txtAllocatedSeatSuffix" ValidationExpression="^[a-zA-Z0-9\s]{0,1}$" Display="None" SetFocusOnError="true" />
                                                <asp:HiddenField ID="hdfNewAllocatedSeatString" runat="server" />
                                            </asp:PlaceHolder>
                                        </td>
                                        <td id="bulkSalesQuantityCol" runat="server" data-title='<%#GetTextTable("BulkSalesQuantityHeaderLabel") %>' class="ebiz-bulk-quantity">
                                            <asp:TextBox ID="txtBulkSalesQuantity" runat="server" />
                                            <asp:Label ID="lblBulkSalesQuantity" runat="server" />
                                            <asp:RequiredFieldValidator ID="rfvBulkSalesQuantity" runat="server" ValidationGroup="Basket" ControlToValidate="txtBulkSalesQuantity" Display="None" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                                            <asp:RegularExpressionValidator ID="rgxBulkSalesQuantity" runat="server" ValidationGroup="Basket" ControlToValidate="txtBulkSalesQuantity" Display="None" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                                            <asp:HiddenField ID="hdfBulkSalesID" runat="server" />
                                        </td>
                                        <td id="packageQuantityCol" runat="server" data-title='<%#GetTextTable("PackageQuantityHeaderLabel") %>' class="ebiz-package-quantity">
                                            <asp:Label ID="PackageQuantityLabel" runat="server"></asp:Label>
                                            <asp:Label ID="PackageIDForQuantity" runat="server" Visible="false"></asp:Label>
                                        </td>
                                        <td id="netPriceCol" runat="server" data-title='<%#GetTextTable("netPriceHeaderLabel") %>' class="ebiz-net-price">
                                            <asp:Label ID="NetPriceLabel" runat="server"></asp:Label>
                                        </td>
                                        <td data-title='<%#GetTextTable("PriceHeaderLabel") %>' class="ebiz-price" runat="server">
                                            <span data-tooltip aria-haspopup="true" class="has-tip" data-disable-hover="false" title="Offer">
                                                <asp:HyperLink ID="hplPromotions" data-open="promotions-modal" runat="server" CssClass="ebiz-open-modal">
                                                    <i class="fa fa-certificate" aria-hidden="true"></i>
                                                </asp:HyperLink>
                                            </span>
                                            <div id="promotions-modal" class="reveal ebiz-reveal-ajax" data-reveal></div>
                                            <asp:PlaceHolder ID="plhVoucherLink" runat="server">
                                                <asp:HyperLink ID="hplVouchers" runat="server" data-open="vouchers-modal" CssClass="ebiz-open-modal">
                                                    <i class="fa fa-tag" aria-hidden="true"></i>
                                                </asp:HyperLink>
                                                <div id="vouchers-modal" class="reveal ebiz-reveal-ajax" data-reveal></div>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="plhTicketExchangeSeat" runat="server">
                                                <i class="fa fa-exchange ebiz-ticket-exchange-seat" aria-hidden="true" runat="server" id="iconTicketExchange"></i>
                                            </asp:PlaceHolder>
                                            <asp:Label ID="PriceLabel" runat="server"></asp:Label>
                                        </td>
                                        <td id="removeCol" runat="server" data-title="" class="ebiz-remove">
                                            <span data-tooltip aria-haspopup="true" class="has-tip" data-disable-hover="false">
                                                <asp:Button ID="btnSaveFavouriteSeat" runat="server" OnClick="btnSaveFavouriteSeat_OnClick" CssClass="fa-input ebiz-fa-input ebiz-save-favourite-seat" />
                                            </span>
                                            <a id="hplComments" data-open="comments-modal" runat="server" class="fa-input ebiz-fa-input ebiz-open-modal ebiz-add-comment">
                                                <span data-tooltip aria-haspopup="true" class="has-tip" data-disable-hover="false" title="Add comment.">
                                                    <asp:Literal ID="ltlCommentsContent" runat="server" />
                                                </span>
                                            </a>
                                            <asp:Button ID="RemoveButton" runat="server" OnPreRender="GetText" OnClick="DoRemoveTicket" CssClass="fa-input ebiz-fa-input ebiz-remove" />
                                            <div id="comments-modal" class="reveal ebiz-reveal-ajax" data-reveal></div>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </tbody>
                                </table>
                                </FooterTemplate>
                            </asp:Repeater>
                        </ItemTemplate>
                    </asp:Repeater>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </ItemTemplate>
</asp:Repeater>
<input type="hidden" id="hdfOptionsUpdated" value="N" />
<asp:Repeater ID="FreeItemsRepeater" runat="server">
    <HeaderTemplate>
        <table class="stack ebiz-free-items">
            <thead>
                <tr>
                    <th class="ebiz-free-items-description">
                        <asp:Literal ID="ltlFreeItemsDescriptionHeader" runat="server" OnPreRender="GetText" />
                    </th>
                    <th class="ebiz-free-items-seat-details">
                        <asp:Literal ID="ltlFreeItemsSeatDetailsHeader" runat="server" OnPreRender="GetText" />
                    </th>
                    <th class="ebiz-free-items-price-band">
                        <asp:Literal ID="ltlFreeItemsPriceBandHeader" runat="server" OnPreRender="GetText" />
                    </th>
                    <th class="ebiz-free-items-member-number">
                        <asp:Literal ID="ltlFreeItemsMemberNumberHeader" runat="server" OnPreRender="GetText" />
                    </th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td data-title="" class="ebiz-free-items-description">
                <span class="ebiz-product-code">
                    <asp:Label ID="ProductCode" runat="server"></asp:Label></span><span class="ebiz-decription"><asp:Label ID="Description" runat="server"></asp:Label></span>
            </td>
            <td data-title="" class="ebiz-free-items-seat-details">
                <asp:Label ID="SeatDetails" runat="server"></asp:Label>
            </td>
            <td data-title="" class="ebiz-free-items-price-band">
                <asp:Label ID="PriceBand" runat="server"></asp:Label>
            </td>
            <td data-title="" class="ebiz-free-items-member-number">
                <asp:Label ID="MemberNumber" runat="server"></asp:Label>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody>
        </table>
        <asp:Label ID="FreeItemsDescriptionsLabel" runat="server" OnPreRender="GetText" CssClass="ebiz-free-items-descriptions" />
    </FooterTemplate>
</asp:Repeater>
