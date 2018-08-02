<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductDetail.ascx.vb" Inherits="TalentEBusiness_UserControls_ProductDetail" ViewStateMode="Disabled" %>
<%@ Register Src="TicketingPPS.ascx" TagName="TicketingPPS" TagPrefix="Talent" %>
<%@ Register Src="PriceBandSelection.ascx" TagName="PriceBandSelection" TagPrefix="Talent" %>
<%@ Register Src="MatchDayHospitality.ascx" TagName="MatchDayHospitality" TagPrefix="Talent" %>

<asp:PlaceHolder ID="plhProductSubType" runat="server" Visible="false">
    <asp:Literal ID="ltlProductSubTypeHtmlInclude" runat="server" ViewStateMode="Disabled" />
</asp:PlaceHolder>
<asp:PlaceHolder ID="plhErrorList" runat="server" Visible="False">
    <div class="alert-box alert">
        <asp:BulletedList ID="errorlist" runat="server" />
    </div>
</asp:PlaceHolder>
<asp:ValidationSummary ID="vlsBulkSalesQuantity" runat="server" ValidationGroup="Quantity" ShowSummary="true" CssClass="alert-box alert" />

<asp:PlaceHolder ID="plhAdvancedProductFilter" runat="server">
    <div class="panel ebiz-ticketing-filter-wrap">
        <h2>
            <asp:Literal ID="ltlLegend" runat="server" /></h2>
        <div class="row">
            <asp:PlaceHolder ID="plhAge" runat="server" Visible="False">
                <div class="column ebiz-ticketing-filter-item ebiz-age-filter">
                    <asp:Label ID="lblAge" runat="server" AssociatedControlID="ddlAge" />
                    <asp:DropDownList ID="ddlAge" runat="server" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhDescription" runat="server" Visible="False">
                <div class="column ebiz-ticketing-filter-item ebiz-description-filter">
                    <asp:Label ID="lblDescription" runat="server" AssociatedControlID="ddlDescription" />
                    <asp:DropDownList ID="ddlDescription" runat="server" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhLocation" runat="server" Visible="False">
                <div class="column ebiz-ticketing-filter-item ebiz-location-filter">
                    <asp:Label ID="lblLocation" runat="server" AssociatedControlID="ddlLocation" />
                    <asp:DropDownList ID="ddlLocation" runat="server" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhStadiumDescription" runat="server" Visible="False">
                <div class="column ebiz-ticketing-filter-item ebiz-stadium-filter">
                    <asp:Label ID="lblStadiumDescription" runat="server" AssociatedControlID="ddlStadiumDescription" />
                    <asp:DropDownList ID="ddlStadiumDescription" runat="server" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhDate" runat="server" Visible="False">
                <div class="column ebiz-ticketing-filter-item ebiz-date-filter">
                    <asp:Literal ID="ltlAvailableDaysScript" runat="server" />
                    <label for="datepicker">
                        <asp:Literal ID="ltlDate" runat="server" /></label>
                    <asp:TextBox ID="txtDate" runat="server" CssClass="datepicker" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhDuration" runat="server" Visible="False">
                <div class="column ebiz-ticketing-filter-item ebiz-duration-filter">
                    <asp:Label ID="lblDuration" runat="server" AssociatedControlID="ddlDuration" />
                    <asp:DropDownList ID="ddlDuration" runat="server" />
                </div>
            </asp:PlaceHolder>
        </div>
        <div class="stacked-for-small button-group ebiz-subtype-filter-buttons">
            <asp:PlaceHolder ID="plhClearButton" runat="server" Visible="False">
                <asp:Button ID="btnClear" runat="server" OnClick="btnClear_Click" CssClass="button ebiz-muted-action ebiz-clear" />
            </asp:PlaceHolder>
            <asp:Button ID="btnSearch" runat="server" OnClick="btnSearch_Click" CssClass="button ebiz-primary-action ebiz-search" />
        </div>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhNoProductsFound" runat="server" Visible="False">
    <div class="alert-box alert ebiz-no-products-found">
        <asp:Literal ID="ltlNoProductsFound" runat="server" /></div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhPagerTopWrapper" runat="server">
    <div class="panel ebiz-pager ebiz-top-pager" id="PagerTop" runat="server">
        <div class="row">
            <div class="large-4 columns ebiz-sort-by">
                <asp:PlaceHolder ID="plhSimpleFilterDropDown" runat="server">
                    <div class="row">
                        <div class="large-6 columns">
                            <asp:Label ID="sortByLabel" runat="server" Text="Sort by" AssociatedControlID="filterByDropDown" CssClass="middle" />
                        </div>
                        <div class="large-6 columns">
                            <asp:DropDownList ID="filterByDropDown" runat="server" AutoPostBack="True" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
            <asp:PlaceHolder ID="plhTopPager" runat="server">
                <div class="large-8 columns ebiz-product-details-pagination ebiz-top-pagination">
                    <span class="ebiz-pagination-displaying-blurb">
                        <asp:PlaceHolder ID="plhDisplayingLabelT" runat="server"><span class="ebiz-pagination-displaying">
                            <asp:Literal ID="displayingLabelT" runat="server" /></span></asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhDisplayingValueLabelT" runat="server"><span class="ebiz-pagination-value-first">
                            <asp:Literal ID="displayingValueLabelT" runat="server" /></span></asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhToLabelT" runat="server"><span class="ebiz-pageination-to">
                            <asp:Literal ID="toLabelT" runat="server" /></span></asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhToValueLabelT" runat="server"><span class="ebiz-pagination-value-last1">
                            <asp:Literal ID="toValueLabelT" runat="server" /></span></asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhOfLabelT" runat="server"><span class="ebiz-pagination-of">
                            <asp:Literal ID="ofLabelT" runat="server" /></span></asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhOfValueLabelT" runat="server"><span class="ebiz-pagination-value-last2">
                            <asp:Literal ID="ofValueLabelT" runat="server" /></span></asp:PlaceHolder>
                    </span>
                    <ul class="pagination">
                        <asp:PlaceHolder ID="plhLnkFirstT" runat="server">
                            <li class="ebiz-pagination-first">
                                <asp:HyperLink ID="LnkFirstT" runat="server" /></li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhLnkPrevT" runat="server">
                            <li class="ebiz-pagination-prev">
                                <asp:HyperLink ID="LnkPrevT" runat="server" /></li>
                        </asp:PlaceHolder>
                        <asp:Literal ID="LinksLabelT" runat="server" />
                        <asp:PlaceHolder ID="plhLnkNextT" runat="server">
                            <li class="ebiz-pagination-next">
                                <asp:HyperLink ID="LnkNextT" runat="server" /></li>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhLnkLastT" runat="server">
                            <li class="ebiz-pagination-last">
                                <asp:HyperLink ID="LnkLastT" runat="server" /></li>
                        </asp:PlaceHolder>
                    </ul>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhTicketingProducts" runat="server">
    <div id="ticketing-products" class="ebiz-accordion ebiz-ticketing-products">
        <asp:Repeater ID="ProductRepeater" runat="server" ViewStateMode="Disabled">
            <ItemTemplate>
                <asp:PlaceHolder ID="plhNormalView" runat="server" Visible="true">
                    <a id="anchorvar<%# DataBinder.Eval(Container.DataItem,"ProductCode").Trim%>" href="#<%# DataBinder.Eval(Container.DataItem,"ProductCode").Trim%>"></a>
                    <asp:PlaceHolder ID="plhProductNoAccordianTop" runat="server" Visible="false">
                        <a href="<%#setVisualSeatSelectionUrl(String.Empty, Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("ProductStadium")), Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("ProductCode")), _
                        Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("CampaignCode")), Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("ProductType")), _
                        Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("ProductSubType")), Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("ProductHomeAsAway")), Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("RestrictGraphical")))%>"></asp:PlaceHolder>

                    <asp:Panel runat="server" ID="pnlAccordian" CssClass="panel ebiz-header">

                        <%-- Hidden Field Data --%>
                        <asp:HiddenField ID="hfProductCode" runat="server" Value='<%# DataBinder.Eval(Container.DataItem,"ProductCode")%>' />
                        <asp:HiddenField ID="hfProductStadium" runat="server" Value='<%# DataBinder.Eval(Container.DataItem,"ProductStadium")%>' />
                        <asp:HiddenField ID="hfProductType" runat="server" Value='<%# DataBinder.Eval(Container.DataItem,"ProductType")%>' />
                        <asp:HiddenField ID="hfCampaignCode" runat="server" Value='<%# DataBinder.Eval(Container.DataItem,"CampaignCode")%>' />
                        <asp:HiddenField ID="hfProductHomeAsAway" runat="server" Value='<%# DataBinder.Eval(Container.DataItem,"ProductHomeAsAway")%>' />
                        <asp:HiddenField ID="hdfProductAvailability" runat="server" Value='<%# DataBinder.Eval(Container.DataItem,"ProductAvailForSale").Trim %>' />
                        <asp:HiddenField ID="hdfPriceCode" runat="server" Value='<%#Talent.eCommerce.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem,"PriceCode")).Trim %>' />

                        <asp:PlaceHolder ID="plhProductDescription" runat="server" Visible="False">
                            <div class="ebiz-product-header">
                                <h2>
                                    <asp:Literal ID="ltlProductDescription" runat="server" /></h2>
                            </div>
                        </asp:PlaceHolder>

                        <%-- Main Header --%>
                        <div class="ebiz-product-wrap">

                            <div class="ebiz-product-logos">
                                <asp:Image ID="OppositionImage" runat="server" ImageUrl='<%# ProductDetail.GetImageURL("PRODTICKETING", Container.DataItem("ProductOppositionCode"))%>' CssClass="ebiz-opposition" />
                                <asp:Image ID="CompetitionImage" runat="server" ImageUrl='<%# ProductDetail.GetImageURL("PRODCOMPETITION", Container.DataItem("ProductCompetitionCode"))%>' CssClass="ebiz-competition" />
                                <asp:PlaceHolder ID="plhSponsorImage" runat="server">
                                    <asp:Image ID="SponsorImage" runat="server" ImageUrl='<%# ProductDetail.GetImageURL("PRODSPONSOR", Container.DataItem("ProductSponsorCode"))%>' CssClass="ebiz-sponsor" />
                                </asp:PlaceHolder>
                            </div>

                            <div class="ebiz-product-details">

                                <asp:PlaceHolder ID="plhProductCompetition" runat="server" Visible="False">
                                    <div class="ebiz-product-detail ebiz-competition">

                                        <div class="ebiz-label">
                                            <asp:Literal ID="ltlProductCompetitionLabel" runat="server" /></div>
                                        <div class="ebiz-value">
                                            <asp:Literal ID="ltlProductCompetitionValue" runat="server" /></div>

                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhProductDate" runat="server" Visible="False">
                                    <div class="ebiz-product-detail ebiz-date">

                                        <div class="ebiz-label">
                                            <asp:Literal ID="ltlProductDateLabel" runat="server" /></div>
                                        <div class="ebiz-value">
                                            <asp:Literal ID="ltlProductDateValue" runat="server" /></div>

                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhProductTime" runat="server" Visible="False">
                                    <div class="ebiz-product-detail ebiz-time">

                                        <div class="ebiz-label">
                                            <asp:Literal ID="ltlProductTimeLabel" runat="server" /></div>
                                        <div class="ebiz-value">
                                            <asp:Literal ID="ltlProductTimeValue" runat="server" /></div>

                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhProductEntryTime" runat="server" Visible="False">
                                    <div class="ebiz-product-detail ebiz-entry-time">

                                        <div class="ebiz-label">
                                            <asp:Literal ID="ltlProductEntryTimeLabel" runat="server" /></div>
                                        <div class="ebiz-value">
                                            <asp:Literal ID="ltlProductEntryTimeValue" runat="server" /></div>

                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhLoyaltyPoints" runat="server" Visible="False">
                                    <div class="ebiz-product-detail ebiz-loyalty">

                                        <div class="ebiz-label">
                                            <asp:Literal ID="ltlLoyaltyPointsLabel" runat="server" /></div>
                                        <div class="ebiz-value">
                                            <asp:Literal ID="ltlLoyaltyPointsValue" runat="server" /></div>

                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhAvailability" runat="server" Visible="False">
                                    <div class="ebiz-product-detail ebiz-availability">

                                        <div class="ebiz-label">
                                            <asp:Literal ID="ltlAvailabilityLabel" runat="server" /></div>
                                        <div class="ebiz-value">
                                            <asp:Label ID="lblAvailabilityValue" runat="server" /></div>

                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhAge" runat="server" Visible="False">
                                    <div class="ebiz-product-detail ebiz-age">

                                        <div class="ebiz-label">
                                            <asp:Literal ID="ltlAgeLabel" runat="server" /></div>
                                        <div class="ebiz-value">
                                            <asp:Literal ID="ltlAgeValue" runat="server" /></div>

                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhDuration" runat="server" Visible="False">
                                    <div class="ebiz-product-detail ebiz-duration">

                                        <div class="ebiz-label">
                                            <asp:Literal ID="ltlDurationLabel" runat="server" /></div>
                                        <div class="ebiz-value">
                                            <asp:Literal ID="ltlDurationValue" runat="server" /></div>

                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhLocation" runat="server" Visible="False">
                                    <div class="ebiz-product-detail ebiz-location">

                                        <div class="ebiz-label">
                                            <asp:Literal ID="ltlLocationLabel" runat="server" /></div>
                                        <div class="ebiz-value">
                                            <asp:Literal ID="ltlLocationValue" runat="server" /></div>

                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhStadiumDescription" runat="server" Visible="False">
                                    <div class="ebiz-product-detail ebiz-stadium">

                                        <div class="ebiz-label">
                                            <asp:Literal ID="ltlStadiumDescriptionLabel" runat="server" /></div>
                                        <div class="ebiz-value">
                                            <asp:Literal ID="ltlStadiumDescriptionValue" runat="server" /></div>

                                    </div>
                                </asp:PlaceHolder>

                            </div>

                        </div>


                        <asp:PlaceHolder ID="plhSoldOutHeader" runat="server" Visible="False">
                            <div class="alert-box alert ebiz-sold-out">
                                <asp:Literal ID="ltlSoldOutHeader" runat="server" /></div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhEligibility" runat="server" Visible='<%# Container.DataItem("ProductReqdMemDesc").ToString().Trim().Length>0 %>'>
                            <div class="alert-box warning ebiz-eligibility"><%#GetProductEligibilityText(Container.DataItem("ProductReqdMemDesc").ToString().Trim())%></div>
                        </asp:PlaceHolder>

                        <%-- Extended text --%>
                        <asp:PlaceHolder ID="plhExtendedTextPanel" runat="server">
                            <div class="alert-box info ebiz-extended-text">
                                <asp:Literal ID="ltlExtendedText1" runat="server" />
                                <asp:Literal ID="ltlExtendedText2" runat="server" />
                                <asp:Literal ID="ltlExtendedText3" runat="server" />
                                <asp:Literal ID="ltlExtendedText4" runat="server" />
                                <asp:Literal ID="ltlExtendedText5" runat="server" />
                            </div>
                        </asp:PlaceHolder>

                        <%-- Specific content --%>
                        <asp:Literal ID="ltlSpecificContent1" runat="server" />
                        <asp:PlaceHolder ID="plhUpgradeToHospitality" runat="server" Visible="false">
                            <div class="ebiz-product__footer">
                                <div class="medium-text-right">
                                    <div class="di mr3">
                                        <asp:HyperLink ID="hplUpgradeToHospitality" runat="server" CssClass="button mb0-m" />
                                    </div>
                                    <div class="di">
                                        <asp:HyperLink ID="hplSelectSeats" runat="server" CssClass="button mb0"></asp:HyperLink>
                                    </div>
                                </div>
                            </div>
                            <!-- ./ebiz-product__footer -->
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhProductNoAccordianBottom" runat="server" Visible="false"></a>                                  
                        </asp:PlaceHolder>
                    </asp:Panel>

                    <asp:PlaceHolder ID="plhProductNoAccordianMain" runat="server">
                        <div class="panel ebiz-content <%# DataBinder.Eval(Container.DataItem, "ProductCode").Trim%>">

                            <%-- Multi Stadium Text --%>
                            <asp:PlaceHolder ID="multiStadiumText" runat="server" Visible="false">
                                <p class="ebiz-multi-stadium-text">
                                    <asp:Label ID="lblMultiStadiumPerProductText" runat="server" /></p>
                            </asp:PlaceHolder>
                            <%-- Instructions for PPS --%>
                            <asp:PlaceHolder ID="plhInstructionText" runat="server" Visible="false">
                                <p class="ebiz-instruction-text">
                                    <asp:Label ID="lblInstructionText" runat="server" /></p>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhFacebookLike" runat="server" Visible="false">
                                <div class="ebiz-facebook-like">
                                    <asp:Literal ID="ltlFacebookLike" runat="server" Visible="false" /></div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhAdditionalInformation" runat="server" Visible="false">
                                <div class="ebiz-more-info">
                                    <asp:HyperLink ID="hlkAdditionalInformation" runat="server" data-open="additional-information-modal" CssClass="ebiz-open-modal">
                                        <asp:Literal ID="ltlMoreInfoText" runat="server" /></asp:HyperLink>
                                    <div id="additional-information-modal" class="reveal ebiz-reveal-ajax" data-reveal></div>
                                </div>
                            </asp:PlaceHolder>

                            <Talent:TicketingPPS ID="TicketingPPS1" runat="server" ProductCode='<%# DataBinder.Eval(Container.DataItem,"ProductCode")%>' PriceCode='<%# DataBinder.Eval(Container.DataItem,"PriceCode")%>' AmendPPSEnrolmentInUse="<%# AmendPPSEnrolmentInUse %>" />
                            <Talent:MatchDayHospitality ID="MatchDayHospitality1" runat="server" ProductCode='<%# DataBinder.Eval(Container.DataItem,"ProductCode")%>' Visible="False" />
                            <Talent:PriceBandSelection ID="PriceBandSelection1" runat="server" Visible="False" ProductCode='<%# DataBinder.Eval(Container.DataItem,"ProductCode")%>' ProductDetailCode='<%# DataBinder.Eval(Container.DataItem,"ProductDetailCode") %>' HasAssociatedTravelProduct='<%# DataBinder.Eval(Container.DataItem, "HasAssociatedTravelProduct") %>' />

                            <asp:PlaceHolder ID="plhSoldOut" runat="server" Visible="False">
                                <div class="alert-box alert ebiz-sold-out">
                                    <asp:Literal ID="ltlSoldOut" runat="server" /></div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhUnavailableBox" runat="server" Visible="False">
                                <div class="alert-box alert ebiz-unavailable">
                                    <asp:Literal ID="ltlUnavailableBoxText" runat="server" /></div>
                            </asp:PlaceHolder>
                        </div>
                    </asp:PlaceHolder>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="plhKioskView" runat="server" Visible="false">
                    <div class="ebiz-match" onclick="location.href='<%# GetAreaSelectionUrl(Talent.Common.Utilities.CheckForDBNull_String(Container.DataItem("ProductStadium")),Container.DataItem("ProductCode"),Talent.Common.Utilities.CheckForDBNull_String(Container.DataItem("CampaignCode")),Container.DataItem("ProductType")) %>';">
                        <div class="ebiz-text">
                            <p class="ebiz-opposition">
                                <asp:Label ID="kioskOppositionLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ProductDescription")%>'></asp:Label></p>
                            <p class="ebiz-competition">
                                <asp:Label ID="kioskCompetitionLabel" runat="server" Text='<%# DataBinder.Eval(Container.DataItem,"ProductDescription2") %>'></asp:Label></p>
                            <p class="ebiz-time">
                                <asp:Label ID="kioskTimeLabel" runat="server" Text='<%# GetFormattedProductDateTime(Container.DataItem("ProductMDTE08"),Container.DataItem("ProductYear"),Container.DataItem("ProductTime"),Container.DataItem("ProductEntryTime")) %>'></asp:Label></p>
                        </div>
                        <div class="ebiz-images">
                            <div class="ebiz-opposition">
                                <asp:Image ID="kioskOppositionImage" runat="server" ImageUrl='<%# ProductDetail.GetImageURL("PRODTICKETING", Container.DataItem("ProductOppositionCode"))%>' /></div>
                            <div class="ebiz-competition">
                                <asp:Image ID="kioskCompetitionImage" runat="server" ImageUrl='<%# ProductDetail.GetImageURL("PRODCOMPETITION", Container.DataItem("ProductCompetitionCode"))%>' /></div>
                        </div>
                    </div>
                </asp:PlaceHolder>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:PlaceHolder>

<%--Pager Bottom section--%>
<asp:PlaceHolder ID="plhBottomPager" runat="server">
    <div class="panel ebiz-product-details-pagination ebiz-bottom-pagination" id="PagerBottom" runat="server">
        <span class="ebiz-pagination-displaying-blurb">
            <asp:PlaceHolder ID="plhDisplayingLabelB" runat="server"><span class="ebiz-pagination-displaying">
                <asp:Literal ID="displayingLabelB" runat="server" /></span></asp:PlaceHolder>
            <asp:PlaceHolder ID="plhDisplayingValueLabelB" runat="server"><span class="ebiz-pagination-value-first">
                <asp:Literal ID="displayingValueLabelB" runat="server" /></span></asp:PlaceHolder>
            <asp:PlaceHolder ID="plhToLabelB" runat="server"><span class="ebiz-pageination-to">
                <asp:Literal ID="toLabelB" runat="server" /></span></asp:PlaceHolder>
            <asp:PlaceHolder ID="plhToValueLabelB" runat="server"><span class="ebiz-pagination-value-last1">
                <asp:Literal ID="toValueLabelB" runat="server" /></span></asp:PlaceHolder>
            <asp:PlaceHolder ID="plhOfLabelB" runat="server"><span class="ebiz-pagination-of">
                <asp:Literal ID="ofLabelB" runat="server" /></span></asp:PlaceHolder>
            <asp:PlaceHolder ID="plhOfValueLabelB" runat="server"><span class="ebiz-pagination-value-last2">
                <asp:Literal ID="ofValueLabelB" runat="server" /></span></asp:PlaceHolder>
        </span>
        <ul class="pagination">
            <asp:PlaceHolder ID="plhLnkFirstB" runat="server">
                <li class="ebiz-pagination-first">
                    <asp:HyperLink ID="LnkFirstB" runat="server" /></li>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhLnkPrevB" runat="server">
                <li class="ebiz-pagination-prev">
                    <asp:HyperLink ID="LnkPrevB" runat="server" /></li>
            </asp:PlaceHolder>
            <asp:Literal ID="LinksLabelB" runat="server" />
            <asp:PlaceHolder ID="plhLnkNextB" runat="server">
                <li class="ebiz-pagination-next">
                    <asp:HyperLink ID="LnkNextB" runat="server" /></li>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhLnkLastB" runat="server">
                <li class="ebiz-pagination-last">
                    <asp:HyperLink ID="LnkLastB" runat="server" /></li>
            </asp:PlaceHolder>
        </ul>
    </div>
</asp:PlaceHolder>
<script type="text/javascript">
    function switchMenu(obj) {
        var el = document.getElementById(obj);
        if (el.style.display != "none") {
            el.style.display = 'none';
        }
        else {
            el.style.display = '';
        }
    }
</script>


