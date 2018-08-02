<%@ Control Language="VB" AutoEventWireup="false" CodeFile="StandAndAreaSelection.ascx.vb" Inherits="UserControls_StandAndAreaSelection" ViewStateMode="Enabled" %>

<asp:Panel runat="server" ID="pnlStandArea" CssClass="panel ebiz-best-available">
    <asp:PlaceHolder ID="plhHeader" runat="server">
        <h2><asp:Literal ID="ltlStandAndAreaHeader" runat="server" /></h2>
    </asp:PlaceHolder>
    <fieldset>
        <legend><asp:Literal ID="ltlStandAreaSelectionLegend" runat="server" /></legend>

        <div class="c-price-band-filter">
            <a class="button" data-toggle="js-price-band-filter" style="display:none;"><asp:Literal ID="ltlPricingFilterOption" runat="server" /></a>
            <div class="c-price-band-price-filter">
                <div id="js-price-band-filter">
                    <asp:PlaceHolder ID="plhMaximumAndMinimumPrice" runat="server">
                    <div class="c-price-band-filter__price">
                        <h2><asp:Literal ID="ltlMinMaxPricingHeader" runat="server" /></h2>
                        <div class="row">
                            <div class="medium-6 columns">
                                <asp:Label ID="lblMinimumPrice" runat="server" AssociatedControlID="ddlMinimumPrice" />
                                <asp:DropDownList ID="ddlMinimumPrice" runat="server" ClientIDMode="Static" />
                            </div>
                            <div class="medium-6 columns">
                                <asp:Label ID="lblMaximumPrice" runat="server" AssociatedControlID="ddlMaximumPrice" CssClass="c-price-band-filter__price__max-label" />
                                <asp:DropDownList ID="ddlMaximumPrice" runat="server" ClientIDMode="Static" />
                            </div>
                        </div>
                    </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="plhPriceBreaks" runat="server">
                    <div class="c-price-band-catagory-filter">
                        <h2><asp:Literal ID="ltlPriceBreakSelection" runat="server" /></h2>
                        <div class="row">
                            <div class="columns">
                                <asp:Label ID="lblPriceBreakSelection" runat="server" AssociatedControlID="ddlPriceBreakSelection" CssClass="c-price-band-catagory-filter__label" />
                                <asp:DropDownList ID="ddlPriceBreakSelection" runat="server" ClientIDMode="Static" />
                            </div>
                        </div>
                    </div>
                    </asp:PlaceHolder>
                </div>
            </div>

            <div class="c-price-band-best-available">
                <a class="button button--edited" data-toggle="js-price-band-best-available" style="display:none;"><asp:Literal ID="ltlSelectStandAreaOptions" runat="server" /></a>
                <div id="js-price-band-best-available">
                    <h2><asp:Literal ID="ltlSelectStandAreaHeader" runat="server" /></h2>
                    <asp:PlaceHolder ID="plhCombinedStandAndArea" runat="server">
                    <div class="row c-price-band-filter__price__combined-stand-area-container">
                        <div class="columns">
                            <asp:Label ID="lblCombinedStandAndArea" runat="server" AssociatedControlID="ddlCombinedStandAndArea" CssClass="c-price-band-filter__price__combined-stand-area-label" />
                            <asp:DropDownList ID="ddlCombinedStandAndArea" CssClass="ebiz-combined-stand-area-drop-down select" ClientIDMode="Static" runat="server" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhStandDropDownList" runat="server">
                    <div class="row c-price-band-filter__price__stand-container">
                        <div class="columns">
                            <asp:Label ID="standLabel" runat="server" AssociatedControlID="standDropDown" CssClass="c-price-band-filter__price__stand-label" />
                            <asp:DropDownList ID="standDropDown" CssClass="ebiz-stand-drop-down select" runat="server" ClientIDMode="Static" OnChange="<%# GetJavascriptStringStandChange()%>" />
                        </div>
                    </div>
                    <div class="row c-price-band-filter__price__area-container">
                        <div class="columns">
                            <asp:Label ID="areaLabel" runat="server" AssociatedControlID="areaDropDownList" CssClass="c-price-band-filter__price__area-label"  />
                            <asp:DropDownList ID="areaDropDownList" runat="server" CssClass="ebiz-area-drop-down select" ClientIDMode="Static" OnChange="<%# GetJavascriptStringAreaChange()%>" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                </div>
            </div>
            
            <div class="c-ticket-exchange" id="ticketExchangePanel" runat="server">
              <a class="button button--edited" data-toggle="js-ticket-exchange" style="display:none;"><asp:Literal ID="ltlTicketExchangeOptions" runat="server" /></a>
              <div id="js-ticket-exchange">
                <h2><asp:Literal ID="ltlTicketExchangeHeader" runat="server"/></h2>
                <div class="ebiz-switch-wrap">
                   <div class="ebiz-switch-wrap ebiz-switch-ticket-exchange-option">
                      <div class="ebiz-switch-label"><%= TicketExchangeOptionText %></div>
                      <div class="switch">
                          <input class="switch-input" id="ticketExchangeOption" type="checkbox" name="ticketExchangeOption" onchange="ReDrawStadium(false, false, false, false, false, false);" runat="server" ClientIDMode="Static" />
                          <label class="switch-paddle" for="ticketExchangeOption">
                            <span class="show-for-sr"><%= TicketExchangeOptionText %></span>
                            <span class="switch-active" aria-hidden="true">Yes</span>
                            <span class="switch-inactive" aria-hidden="true">No</span>
                          </label>
                      </div>
                    </div>
                    <div class="ebiz-slider-wrap ebiz-slider-ticket-exchange-option">
                      <div class="ebiz-slider-label"><%= TicketExchangeSliderText%></div>
                      <div class="slider" data-slider runat="server" id="divTicketExchangeSlider">
                        <span class="slider-handle" id="slider-handle-min" data-slider-handle role="slider" tabindex="1" >
                          <span class="slider-drop">£<input id="slider-handle-ticket-exchange-min"></span>
                        </span>
                        <span class="slider-fill" data-slider-fill></span>
                        <span class="slider-handle" id="slider-handle-max" data-slider-handle role="slider" tabindex="1">
                          <span class="slider-drop">£<input id="slider-handle-ticket-exchange-max"></span>
                        </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>  

            <asp:PlaceHolder ID="plhBuyingOptions" runat="server">
            <div class="c-price-band-concession">
                <asp:PlaceHolder ID="plhPriceBandList" runat="server">
                    <a class="button" data-toggle="js-price-band-concession" style="display:none;"><asp:Literal ID="ltlPriceBandOptions" runat="server" /></a>
                    <div id="js-price-band-concession">
                        <h2><asp:Literal ID="ltlPriceBandOptionsHeader" runat="server" /></h2>                    
                        <asp:Repeater ID="rptPriceBand" runat="server">
                            <ItemTemplate>
                                <div class="ebiz-priceband ebiz-priceband-<%# Eval("PriceBand").ToString()%> <%# GetLoggedInCustomerPriceBandCss(Eval("PriceBand").ToString())%> ">
                                    <div class="row ebiz-priceband-description-wrapper">
                                        <div class="columns ebiz-price-band-description-wrapper">
                                            <asp:Label ID="lblPriceBandDescription" runat="server" AssociatedControlID="txtPriceBandQuantity" />
                                        </div>
                                        <div class="columns ebiz-priceband-price-wrapper">
                                            <label class="ebiz-priceband-price ebiz-priceband-price-label"><%# Eval("PriceBandPrice").ToString()%></label>
                                        </div>
                                        <div class="columns ebiz-priceband-quantity-wrapper">
                                            <asp:Textbox ID="txtPriceBandQuantity" runat="server" type="number" CssClass="ebiz-price-band-quantity-box" ValidationGroup="quantity" autocomplete="off" />
                                            <asp:RangeValidator ID="rngPriceBandQuantity" runat="server" ControlToValidate="txtPriceBandQuantity" Display="None" CssClass="error" SetFocusOnError="true" ValidationGroup="Quantity" EnableClientScript="true" />
                                        </div>
                                    </div>
                                </div>
                                <asp:HiddenField ID="hdfPriceBand" runat="server"  />                
                            </ItemTemplate>
                        </asp:Repeater>
                        <div class="c-price-band-concession__buy-container">
                            <asp:Button CssClass="button ebiz-primary-action" ID="btnPriceBandListBuyButton" runat="server" OnKeyPress="<%# GetJavascriptStringBuyMouseUp() %>" OnMouseUp="<%# GetJavascriptStringBuyMouseUp() %>" OnClick="AddTicketingItems" ValidationGroup="Quantity" CausesValidation="true" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhDefaultPriceBand" runat="server">
                    <div class="row ebiz-quantity">
                        <div class="medium-3 columns">
                            <asp:Label ID="lblQuantity" runat="server" AssociatedControlID="txtQuantity" />
                        </div>
                        <div class="medium-9 columns">
                            <asp:Textbox ID="txtQuantity" runat="server" type="number" CssClass="ebiz-quantity-box" ValidationGroup="quantity" autocomplete="off" />
                            <asp:RangeValidator ID="rngQuantity" runat="server" ControlToValidate="txtQuantity" Display="None" CssClass="error" SetFocusOnError="true" ValidationGroup="Quantity" EnableClientScript="true" />
                        </div>
                    </div>
                    <div class="c-price-band-concession__buy-container">
                        <asp:Button CssClass="button ebiz-primary-action" ID="btnSingleQuantityBuyButton" runat="server" OnKeyPress="<%# GetJavascriptStringBuyMouseUp() %>" OnMouseUp="<%# GetJavascriptStringBuyMouseUp() %>" OnClick="AddTicketingItems" ValidationGroup="Quantity" CausesValidation="true" />
                    </div>
                </asp:PlaceHolder>
            </div>
            </asp:PlaceHolder>
            
            <asp:PlaceHolder ID="plhSelectSeatsButton" runat="server">
                <div class="ebiz-select-seats-wrap">
                    <asp:Button CssClass="button" ID="btnSelectSeats" runat="server" />
                </div>
            </asp:PlaceHolder>
            
            <asp:PlaceHolder ID="plhResetOption" runat="server">
                <div class="c-price-band__reset-container">
                    <a href="#" class="ebiz-muted-action button" onclick="ResetStadium();"><i class="fa fa-repeat" aria-hidden="true"></i> <%= ResetButtonText %></a>
                </div>
            </asp:PlaceHolder>
        </div>
    </fieldset>

    <asp:HiddenField ID="hdfProductType" runat="server" />
    <asp:HiddenField ID="hdfProductCode" runat="server" />
    <asp:HiddenField ID="hdfProductSubType" runat="server" />
    <asp:HiddenField ID="hdfProductStadium" runat="server" />
    <asp:HiddenField ID="hdfProductPriceBand" runat="server" />
    <asp:HiddenField ID="hdfSelectedMinimumPrice" runat="server" />
    <asp:HiddenField ID="hdfSelectedMaximumPrice" runat="server" />
    <asp:HiddenField ID="hdfSelectedPriceBreakId" runat="server" />
    <asp:HiddenField ID="hdfStandSelected" runat="server" />
    <asp:HiddenField ID="hdfAreaSelected" runat="server" />
    <asp:HiddenField ID="hdfCampaignCode" runat="server" />
    <asp:HiddenField ID="hdfProductHomeAsAway" runat="server" />
    <asp:HiddenField ID="hdfOldPrice" runat="server" />
    <asp:HiddenField ID="hdfAlternativeSeatSelection" runat="server" />
    <asp:HiddenField ID="hdfAlternativeSeatSelectionAcrossStands" runat="server" />
    <asp:HiddenField ID="hdfShowTicketExchangeAsDropDown" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfShowPricingOptionsAsDropDown" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfShowStandAreaOptionsAsDropDown" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfShowPriceBandListAsDropDown" runat="server" ClientIDMode="Static" />
    <asp:Literal ID="ltlJavascriptDDL" runat="server" />
</asp:Panel>

<asp:PlaceHolder ID="plhSoldOut" runat="server" Visible="False">
    <div class="alert-box alert"><asp:Literal ID="ltlError" runat="server" /></div>
</asp:PlaceHolder>

<script type="text/javascript">
    var renderStandAreaDDLOptions = true;
    var buyButton;
    $(function () {
        if (document.getElementById('<% = btnPriceBandListBuyButton.ClientID%>') == undefined) {
            if (document.getElementById('<% = btnSingleQuantityBuyButton.ClientID%>') != undefined) {
                buyButton = '#' + '<% = btnSingleQuantityBuyButton.ClientID%>';
            }
        } else {
            buyButton = '#' + '<% = btnPriceBandListBuyButton.ClientID%>';
        }
        $(buyButton).hide();
    });

    function onQuantityChanged() {
        var hasPriceBand = false;
        if ($(".ebiz-price-band-quantity-box") != undefined) {
            $(".ebiz-price-band-quantity-box").each(function () {
                if ($(this).val() != "") {
                    hasPriceBand = true;
                    return false;
                }
            });
        }
        if ($(".ebiz-quantity-box") != undefined) {
            if ($(".ebiz-quantity-box").val() != "") {
                hasPriceBand = true;
            }
        }

        if ("<%= IsPriceAndAreaSelection%>" == "False") {
            if (!hasPriceBand ||
                $('#<% = standDropDown.ClientID%> :selected').text() == "" ||
                $('#<% = standDropDown.ClientID%> :selected').text() == "<%= StandDDLPleaseSelectText%>" ||
                $('#<% = areaDropDownList.ClientID%> :selected').text() == "" ||
                $('#<% = areaDropDownList.ClientID%> :selected').text() == "<%= AreaDDLFirstOption%>") {
                $(buyButton).hide();
            }
            else {
                $(buyButton).show();
            }
        } else {
            if (!hasPriceBand || $('#<% = ddlCombinedStandAndArea.ClientID%> :selected').text() == "" ||
                $('#<% = ddlCombinedStandAndArea.ClientID%> :selected').text() == "<%= AreaDDLFirstOption%>") {
                $(buyButton).hide();
            }
            else {
                $(buyButton).show();
            }
        }
    }
</script>