<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SeatSelection.ascx.vb" Inherits="UserControls_SeatSelection" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/FavouriteSeatSelection.ascx" TagName="FavouriteSeatSelection" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/StandAndAreaSelection.ascx" TagName="StandAndAreaSelection" TagPrefix="Talent" %>

<div class="row ebiz-seat-selection ebiz-information">
    <div class="<%= InformationTextCSSClassname %> columns">
        <asp:PlaceHolder ID="plhInformationText" runat="server" Visible="false">
            <h1>
                <asp:Label ID="lblInformationText" runat="server" CssClass="ebiz-product-title" />
            </h1>
            <asp:PlaceHolder ID="plhAdditionalInformation" runat="server">
                <div class="alert-box info ebiz-more-info">
                    <asp:HyperLink ID="hlkAdditionalInformation" data-open="additional-information-modal" CssClass="ebiz-open-modal" runat="server">
                        <asp:Literal ID="ltlMoreInfoText" runat="server" />
                    </asp:HyperLink>
                    <div id="additional-information-modal" class="reveal ebiz-reveal-ajax" data-reveal></div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
    </div>
    <asp:PlaceHolder ID="plhFacebookLike" runat="server">
        <div class="large-2 columns">
            <asp:Literal ID="ltlFacebookLike" runat="server" />
        </div>
    </asp:PlaceHolder>
</div>

<asp:PlaceHolder ID="plhErrorList" runat="server" Visible="false">
    <div class="row">
        <div class="large-12 columns">
            <div class="alert-box alert">
                <asp:BulletedList ID="blErrorMessages" runat="server" />
            </div>
        </div>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhBackToLinkedProducts" runat="server">
    <div class="row ebiz-back-to-linked-products">
        <div class="large-12 columns">
            <asp:HyperLink ID="hplLinkedProductBack" runat="server" CssClass="button" />
        </div>
    </div>
</asp:PlaceHolder>

<div class="row ebiz-seat-selection ebiz-selection">
    <!-- second main column -->
    <div class="large-9 columns hide-for-small ebiz-svg-container ebiz-area-view ebiz-multiselect-off">
        <asp:PlaceHolder ID="plhExtendedText" runat="server" Visible="false">
            <div class="alert-box info ebiz-extended-text">
                <asp:Literal ID="lblExtendedText1" runat="server" />
                <asp:Literal ID="lblExtendedText2" runat="server" />
                <asp:Literal ID="lblExtendedText3" runat="server" />
                <asp:Literal ID="lblExtendedText4" runat="server" />
                <asp:Literal ID="lblExtendedText5" runat="server" />
            </div>
        </asp:PlaceHolder>
        <div class="panel">
            <div id="ticketing3DSeatView" style="display: none;"></div>
            <asp:PlaceHolder ID="plhChangeStandView" runat="server">
                <input type="button" onclick="changeStandView();" value="<%= ChangeStandViewButtonText %>" id="btnChangeStandView" class="button ebiz-view-price" />
            </asp:PlaceHolder>


            <asp:PlaceHolder ID="plhStandAvailabilityKey" runat="server">
                <div class="ebiz-legend ebiz-stand-key ebiz-stand-availability">
                    <h2>
                        <asp:Literal ID="ltlAreaAvailability" runat="server" />
                    </h2>
                    <asp:Repeater ID="rptAvailabilityKey" runat="server">
                        <HeaderTemplate>
                            <ul class="no-bullet">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="<%# DataBinder.Eval(Container.DataItem, "CSS_CLASS").ToString() %>">
                                <div style="background: <%# DataBinder.Eval(Container.DataItem, "COLOUR").ToString() %>;">&nbsp;</div>
                                <span><%#DataBinder.Eval(Container.DataItem, "TEXT").ToString()%></span>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plhPricingKey" runat="server">
                <div style="display: none;" class="ebiz-legend ebiz-stand-key ebiz-stand-pricing">
                    <h2>
                        <asp:Literal ID="ltlAreaPricing" runat="server" />
                    </h2>
                    <asp:Repeater ID="rptPricingKey" runat="server">
                        <HeaderTemplate>
                            <ul class="no-bullet">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <li class="<%# DataBinder.Eval(Container.DataItem, "CSS_CLASS").ToString() %>">
                                <div style="background: <%# DataBinder.Eval(Container.DataItem, "COLOUR").ToString() %>;">&nbsp;</div>
                                <span><%# GetPriceTextBasedOnCategory(DataBinder.Eval(Container.DataItem, "AREA_CATEGORY").ToString(), DataBinder.Eval(Container.DataItem, "TEXT").ToString())%></span>
                            </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </asp:PlaceHolder>

            <div class="row ebiz-svg-controls-wrap">
                <div class="large-12 columns" id="svg-controls">
                    <input type="button" onclick="backToStadium()" value="<%= BackToStadiumViewText %>" class="button ebiz-back-to-stadium-view" id="backButton" style="display: none" />
                    <input type="button" onclick="goReset();" id="btnResetSeating" class="button ebiz-reset-seating" value="<%= ResetButtonText %>" style="display: none" />
                    <input type="button" onclick="rowSeatCodesOnClick();" id="btnRowSeatCodes" class="button ebiz-row-seat-SVG" value="<%= RowSeatRowOnSVGText %>" style="display: none" />
                    <input type="button" onclick="multiSelectOnClick();" id="btnMultiSelect" class="button ebiz-multi-select" value="<%= MultiSelectOffText%>" style="display: none" />
                    <asp:PlaceHolder ID="plhViewFromArea" runat="server">
                        <input type="button" onclick="ViewFromAreaClick();" value="<%= ViewFromAreaButtonText %>" id="btnViewFromArea" class="button ebiz-view-from-area" style="display: none" />
                    </asp:PlaceHolder>
                    <div id="navigation-options" class="ebiz-navigation-options-wrap" style="display: none">
                        <a onclick="goUp()" class="ebiz-up ebiz-direction-enabled"><i class=" fa fa-chevron-up"></i></a>
                        <a onclick="goLeft()" class="ebiz-left ebiz-direction-enabled" id="btnGoLeft"><i class="fa fa-chevron-left"></i></a>
                        <a onclick="goRight()" class="ebiz-right ebiz-direction-enabled"><i class="fa fa-chevron-right"></i></a>
                        <a onclick="goDown()" class="ebiz-down"><i class="fa fa-chevron-down"></i></a>
                    </div>
                    <div class="ebiz-zoom-options" style="display: none">
                        <input type="hidden" id="sliderVal" value="2000" />
                        <input type="hidden" id="maxX" value="" />
                        <input type="hidden" id="maxY" value="" />
                        <div id="zoomOut">
                            <a onclick="goZoomIn()" class="ebiz-zoom-out"><i class="fa fa-minus"></i></a>
                        </div>
                        <div id="zoomSlider"></div>
                        <div id="zoomIn">
                            <a onclick="goZoomOut()" class="ebiz-zoom-in"><i class="fa fa-plus"></i></a>
                        </div>
                    </div>
                    <div id="display" class="ebiz-seat-navigator-wrap" style="display: none"></div>
                    <div class="ebiz-stadium">
                        <div id="stadium-canvas" class="ebiz-stadium-canvas-wrap">
                            <div class="ebiz-match-text">
                                <asp:PlaceHolder ID="plhTeamsWrapper" runat="server">
                                    <div class="ebiz-teams-wrapper">
                                        <asp:PlaceHolder ID="plhHomeTeam" runat="server">
                                            <div class="ebiz-home-team">
                                                <asp:Image ID="imgHomeTeam" runat="server" />
                                            </div>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="plhVersus" runat="server">
                                            <div class="ebiz-versus">
                                                <asp:Literal ID="ltlVersus" runat="server" />
                                            </div>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="plhAwayTeam" runat="server">
                                            <div class="ebiz-away-team">
                                                <asp:Image ID="imgAwayTeam" runat="server" />
                                            </div>
                                        </asp:PlaceHolder>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhMatchHeader" runat="server">
                                    <h2 id="matchHeader">
                                        <asp:Literal ID="ltlMatchHeader" runat="server" />
                                    </h2>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhCompetition" runat="server">
                                    <p class="ebiz-match-competition" id="matchCompetition">
                                        <asp:Literal ID="ltlCompetition" runat="server" />
                                    </p>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhDateAndTime" runat="server">
                                    <p class="ebiz-match-date" id="matchDate">
                                        <asp:Literal ID="ltlDateAndTime" runat="server" />
                                    </p>
                                </asp:PlaceHolder>
                            </div>
                            <div id="stand-tip" style="display: none" class="alert-box ebiz-callout ebiz-tooltip ebiz-stand-tip">
                                <ul class="no-bullet">
                                    <li id="stand" class="ebiz-stand"><span class="ebiz-label"><%= StandPrefixText%></span><span class="ebiz-data"></span></li>
                                    <li id="area" class="ebiz-area"><span class="ebiz-label"><%= AreaPrefixText%></span><span class="ebiz-data"></span></li>
                                    <li id="stand-tip-availibility" class="ebiz-availibility"><span class="ebiz-label"><%= AvailabilityPrefixText%></span><span class="ebiz-data"></span></li>
                                    <li id="stand-tip-price" class="ebiz-price"><span class="ebiz-label"></span><span class="ebiz-data"></span></li>
                                    <li id="stand-tip-price1" class="ebiz-price"><span class="ebiz-label"></span><span class="ebiz-data"></span></li>
                                </ul>
                            </div>
                            <div id="quick-buy-option" class="panel ebiz-quick-buy-option" style="display: none">
                                <h2>
                                    <asp:Literal ID="ltlQuickBuyTitle" runat="server" />
                                </h2>
                                <div class="ebiz-quick-buy-options-wrap">
                                    <div class="row">
                                        <div class="small-4 columns ebiz-quantity-minus">
                                            <a onclick="qBuyTicketQuant('minus');"><i class="fa fa-minus"></i></a>
                                        </div>
                                        <div class="small-4 columns ebiz-quantity-value">
                                            <span id="quick-buy-quantity"></span>
                                        </div>
                                        <div class="small-4 columns ebiz-quantity-plus">
                                            <a onclick="qBuyTicketQuant('add');"><i class="fa fa-plus"></i></a>
                                        </div>
                                    </div>
                                </div>
                                <asp:HiddenField ClientIDMode="Static" ID="hdfQuickBuyQuantity" runat="server" />
                                <asp:HiddenField ClientIDMode="Static" ID="hdfQuickBuyStandAreaCode" runat="server" />
                                <asp:HiddenField ClientIDMode="Static" ID="hdfTransferableBasketItems" runat="server" />
                                <div class="button-group">
                                    <input type="button" onclick="clearQuickBuy();" id="btnClearQuickBuy" class="button ebiz-muted-action ebiz-clear-basket" value="<%= ClearQuickBuyText %>" />
                                    <asp:Button ID="btnQuickBuy" runat="server" class="button ebiz-primary-action ebiz-qick-buy" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div style="display: none" id="svg-wrapper">
                    <asp:PlaceHolder ID="plhSeatingKey" runat="server" Visible="false">
                        <div style="display: none" class="ebiz-legend ebiz-seating-key">
                            <h2>
                                <asp:Literal ID="ltlSeatingKey" runat="server" />
                            </h2>
                            <asp:Repeater ID="rptSeatingKey" runat="server">
                                <HeaderTemplate>
                                    <ul class="no-bullet">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <li class="<%# DataBinder.Eval(Container.DataItem, "CSS_CLASS").ToString() %>">
                                        <div style="background: <%# DataBinder.Eval(Container.DataItem, "FILL_COLOUR").ToString() %>; border: solid 2px <%# DataBinder.Eval(Container.DataItem, "OUTLINE_COLOUR").ToString() %>">&nbsp;</div>
                                        <span><%#DataBinder.Eval(Container.DataItem, "TEXT").ToString()%></span> </li>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </ul>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </asp:PlaceHolder>
                    <div class="ebiz-seat-options">
                        <asp:PlaceHolder ID="plhPitchTop" runat="server">
                            <div class="ebiz-pitch ebiz-pitch-top">
                                <i class="fa fa-arrow-up"></i>
                                <asp:Literal ID="ltlPitchTop" runat="server" />
                                <i class="fa fa-arrow-up"></i>
                            </div>
                        </asp:PlaceHolder>
                        <div class="ebiz-seat-selection-outer-container">
                            <div class="ebiz-seat-selection-inner-container">
                                <div id="select" style="display: none; position: relative;">
                                    <div id="tip" class="alert-box ebiz-callout ebiz-tooltip ebiz-seat-tip" style="display: none">
                                        <ul class="no-bullet">
                                            <li class="ebiz-stand" id="tip-stand"><span class="ebiz-label"><%= StandPrefixText%></span><span class="ebiz-data"></span></li>
                                            <li class="ebiz-area" id="tip-area"><span class="ebiz-label"><%= AreaPrefixText%></span><span class="ebiz-data"></span></li>
                                            <li class="ebiz-seat-row" id="tip-row"><span class="ebiz-label"><%= RowPrefixText%></span><span class="ebiz-data"></span></li>
                                            <li class="ebiz-seat-number" id="tip-seat-number"><span class="ebiz-label"><%= SeatPrefixText%></span><span class="ebiz-data"></span></li>
                                            <li class="ebiz-seat-number" id="tip-seat-price"><span class="ebiz-label"><%= SeatPricePrefixText %></span><span class="ebiz-data"></span></li>
                                            <li class="ebiz-restriction-desc" id="tip-restriction-desc" style="display: none"></li>
                                            <li class="ebiz-reservation-desc" id="tip-reservation-desc" style="display: none"></li>
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <asp:PlaceHolder ID="plhPitchBottom" runat="server">
                            <div class="ebiz-pitch ebiz-pitch-bottom">
                                <i class="fa fa-arrow-down"></i>
                                <asp:Literal ID="ltlPitchBottom" runat="server" />
                                <i class="fa fa-arrow-down"></i>
                            </div>
                        </asp:PlaceHolder>
                    </div>
                    <div id="login-register-prompt" class="reveal" data-reveal>
                        <h2 id="firstModalTitle">
                            <asp:Literal ID="ltlLoginMessage" runat="server" /></h2>
                        <input type="button" onclick="registerLoginRedirect();" id="btnLoginOrRegister" class="button ebiz-login-register" value="<%= LoginMessageTitle %>" />
                        <input type="button" data-close id="btnCloseBox" class="button ebiz-close-modal" value="<%= CloseModalWindowText %>" />
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="large-12 columns">
                    <input id="hdfStadiumXML" type="hidden" value="<%= StadiumXml %>" />
                    <input id="hdfDescriptionsXML" type="hidden" value="<%= DescriptionsXml %>" />
                    <input id="hdfStandAndAreaCode" type="hidden" value="<%= StandAndAreaCode %>" />
                    <input id="hdfStadiumCode" type="hidden" value="<%= StadiumCode %>" />
                    <input id="hdfProductCode" type="hidden" value="<%= ProductCode %>" />
                    <input id="hdfProductType" type="hidden" value="<%= ProductType %>" />
                    <input id="hdfCATMode" type="hidden" value="<%= CATMode %>" />
                    <asp:PlaceHolder ID="plhSeatDetails" runat="server">
                        <div class="ebiz-seat-details" style="display: none">
                            <h2>
                                <asp:Literal ID="ltlSeatDeailsTitle" runat="server" />
                            </h2>
                            <ul class="no-bullet">
                                <li class="ebiz-customer-number">
                                    <span class="ebiz-label">
                                        <asp:Literal ID="ltlSeatDetailsCustomerNumber" runat="server" />
                                    </span>
                                    <span id="customer-number" class="ebiz-data"></span>
                                </li>
                                <li class="ebiz-customer-forename">
                                    <span class="ebiz-label">
                                        <asp:Literal ID="ltlSeatDetailsCustomerForename" runat="server" />
                                    </span>
                                    <span id="customer-forename" class="ebiz-data"></span>
                                </li>
                                <li class="ebiz-customer-surname">
                                    <span class="ebiz-label">
                                        <asp:Literal ID="ltlSeatDetailsCustomerSurname" runat="server" />
                                    </span>
                                    <span id="customer-surname" class="ebiz-data"></span>
                                </li>
                                <li class="ebiz-customer-address1">
                                    <span class="ebiz-label">
                                        <asp:Literal ID="ltlAddress1" runat="server" />
                                    </span>
                                    <span id="customer-address1" class="ebiz-data"></span>
                                </li>
                                <li class="ebiz-customer-address2">
                                    <span class="ebiz-label">
                                        <asp:Literal ID="ltlAddress2" runat="server" />
                                    </span>
                                    <span id="customer-address2" class="ebiz-data"></span>
                                </li>
                                <li class="ebiz-customer-address3">
                                    <span class="ebiz-label">
                                        <asp:Literal ID="ltlAddress3" runat="server" />
                                    </span>
                                    <span id="customer-address3" class="ebiz-data"></span>
                                </li>
                                <li class="ebiz-season-ticket">
                                    <span class="ebiz-label">
                                        <asp:Literal ID="ltlSeasonTicket" runat="server" />
                                    </span>
                                    <span id="season-ticket" class="ebiz-data"></span>
                                </li>
                                <li class="ebiz-book-number">
                                    <span class="ebiz-label">
                                        <asp:Literal ID="ltlBookNumber" runat="server" />
                                    </span>
                                    <span id="book-number" class="ebiz-data"></span>
                                </li>
                            </ul>
                        </div>
                        <div class="ebiz-restriction-details" style="display: none">
                            <h2>
                                <asp:Literal ID="ltlRestrictionDetails" runat="server" />
                            </h2>
                            <ul class="no-bullet">
                                <li class="ebiz-restriction-details">
                                    <span class="ebiz-label">
                                        <asp:Literal ID="ltlTextDescRestriction" runat="server" />
                                    </span>
                                    <span id="restriction-details" class="ebiz-data"></span>
                                </li>
                                <li class="ebiz-restriction-code">
                                    <span class="ebiz-label">
                                        <asp:Literal ID="ltlRestrictionCode" runat="server" />
                                    </span>
                                    <span id="restriction-code" class="ebiz-data"></span>
                                </li>
                                <li class="ebiz-restriction-description">
                                    <span class="ebiz-label">
                                        <asp:Literal ID="ltlRestrictionDescription" runat="server" />
                                    </span>
                                    <span id="restriction-description" class="ebiz-data"></span>
                                </li>
                            </ul>
                        </div>
                        <div class="ebiz-customer-reservation-time" style="display: none">
                            <h2>
                                <asp:Literal ID="ltlCustomerReserationTime" runat="server" />
                            </h2>
                            <ul class="no-bullet">
                                <li class="ebiz-reserved-date">
                                    <span class="ebiz-label">
                                        <asp:Literal ID="ltlReservedDate" runat="server" />
                                    </span>
                                    <span id="reserved-date" class="ebiz-data"></span>
                                </li>
                                <li class="ebiz-reserved-time">
                                    <span class="ebiz-label">
                                        <asp:Literal ID="ltlReservedTime" runat="server" />
                                    </span>
                                    <span id="reserved-time" class="ebiz-data"></span>
                                </li>
                            </ul>
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>

    <!-- first main column -->
    <div class="small-12 large-3 columns">

        <asp:PlaceHolder ID="plhUpgradeToHospitality" runat="server">
            <div class="panel c-hospitality-upgrade-blurb">
                <asp:Literal ID="ltlUpgradeToHospitality" runat="server"></asp:Literal>
                <div class="c-hospitality-upgrade-blurb__blurb">
                    <asp:Literal ID="ltlUpgradeToHospitalityDesc" runat="server"></asp:Literal>
                </div>
                <asp:HyperLink ID="hplUpgradeToHospitality" runat="server" CssClass="button mb0"></asp:HyperLink>
            </div>
        </asp:PlaceHolder>

        <div id="detailed-seat-list-panel">
            <div class="panel ebiz-selected-seats">
                <div class="c-price-band-selection">
                    <h2 class="c-price-band-selection__items-blurb"><span class="c-price-band-selection__items-total">0</span> <%=ItemsInBasketText %></h2>
                </div>
                <div class="button-group">
                    <input type="button" onclick="ClearSeatsClick()" value="<%= ClearSelectionButtonText %>" id="btnClearSeats" class="button ebiz-muted-action ebiz-clear-seats" />
                    <asp:Button ID="btnBuy" ClientIDMode="Static" runat="server" OnClientClick="return buyOnClick();" class="button ebiz-primary-action ebiz-buy" />
                </div>
                <asp:HiddenField ID="hdfSelectedSeats" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdfIsBuyButtonClicked" runat="server" ClientIDMode="Static" Value="False" />
                <asp:HiddenField ID="hdfRowSeat" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdfRowText" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="hdfSeatText" runat="server" ClientIDMode="Static" />
            </div>
        </div>
        <div class="row ebiz-buying-options">
            <asp:PlaceHolder ID="plhStandPriceSelectionBoxes" runat="server">
                <div class="large-12 medium-6 columns">
                    <Talent:StandAndAreaSelection ID="uscStandAndAreaSelection" runat="server" />
                </div>
            </asp:PlaceHolder>

            <div class="large-12 <%= MediumClass %> columns">
                <div class="panel">
                    <!--For 2.12.2-->
                    <asp:PlaceHolder ID="plhStandAndArea" runat="server">
                        <div class="row">
                            <div class="columns">
                                <asp:Label ID="lblChangeStandAndArea" runat="server" AssociatedControlID="ddlChangeStandAndArea" CssClass="c-price-band-filter__price__combined-stand-area-label" />
                                <asp:DropDownList ID="ddlChangeStandAndArea" runat="server" ClientIDMode="Static"/>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <asp:HyperLink ID="hplBackToSTExceptionsPage" runat="server" CssClass="ebiz-back-to-st-exceptions button" Visible="false" />
                    <asp:HyperLink ID="hplBackToHospitalityBookingPage" runat="server" CssClass="ebiz-back-to-hospitality-booking button" Visible="false" />
                    <Talent:FavouriteSeatSelection ID="uscFavouriteSeatSelection" runat="server" />
                    <asp:PlaceHolder ID="plhGameSelection" runat="server">
                        <div class="panel ebiz-selection-a-different-game-wrap">
                            <h2>
                                <asp:Literal ID="ltlGameSelectionTitle" runat="server" />
                            </h2>
                            <asp:Label ID="lblGameSelection" runat="server" AssociatedControlID="ddlGameSelection" Style="display: none;" />
                            <asp:DropDownList ID="ddlGameSelection" runat="server" AutoPostBack="true" ViewStateMode="Enabled" />
                        </div>
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
        <asp:HiddenField ID="hdfPriceBands" runat="server" ClientIDMode="Static" />
    </div>
