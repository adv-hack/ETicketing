<%@ Page Language="VB" EnableEventValidation="false" AutoEventWireup="false" 
    CodeFile="LinkedProducts.aspx.vb" Inherits="PagesPublic_ProductBrowse_LinkedProducts" ViewStateMode="Disabled"  %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/StandAndAreaSelection.ascx" TagName="StandAndAreaSelection" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PriceBandSelection.ascx" TagName="PriceBandSelection" TagPrefix="Talent" %>

<asp:Content ID="cphBody" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <asp:PlaceHolder ID="plhErrorMessages" runat="server" Visible="false">
        <div class="alert-box alert"><asp:Literal ID="ltlErrorMessage" runat="server" /></div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhListOfErrorMessages" runat="server" Visible="false">
        <div class="alert-box alert"><asp:BulletedList ID="blErrorMessages" runat="server" /></div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhListOfSuccessMessages" runat="server" Visible="false">
        <div class="alert-box success"><asp:BulletedList ID="blSuccessMessages" runat="server" /></div>
    </asp:PlaceHolder>
    <asp:ValidationSummary ID="vlsErrorMessages" runat="server" DisplayMode="BulletList" ShowSummary="true" CssClass="alert-box alert" ValidationGroup="LinkedProducts" />

    <div class="stacked-for-small button-group ebiz-linked-products-buttons-top-wrap">
        <asp:Button ID="btnBasketTop" CssClass="button ebiz-go-to-basket" runat="server" ValidationGroup="LinkedProducts" />
        <asp:Button ID="btnBuyTop" CssClass="button ebiz-primary-action ebiz-buy" runat="server" ValidationGroup="LinkedProducts" CausesValidation="true" />
    </div>
    
    <div class="panel ebiz-main-product-intro-wrap">
        <a data-open="exampleModal1"><asp:Literal ID="ltlMainProductIntro" runat="server" /></a>
    </div>

    <div class="reveal" id="exampleModal1" data-reveal>
        <div class="panel ebiz-main-product">
            <div class="ebiz-product-detail">
                <asp:Image ID="imgOpposition" runat="server" CssClass="ebiz-opposition" />
                
               
                    <h2><asp:Literal ID="ltlProductDescription1" runat="server" /></h2>
                    <asp:PlaceHolder ID="plhProductDescription2" runat="server"><div class="ebiz-description2"><asp:Literal ID="ltlProductDescription2" runat="server" /></div></asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhDateAndTime" runat="server">
                        <div class="ebiz-date-time">
                            <span class="ebiz-date"><asp:Literal ID="ltlDate" runat="server" /></span>
                            <span class="ebiz-time"><asp:Literal ID="ltlTime" runat="server" /></span>
                        </div>
                    </asp:PlaceHolder>
                
            </div>

            <asp:PlaceHolder ID="plhSeatDetails" runat="server">
                <asp:Repeater ID="rptSeatDetails" runat="server">
                    <HeaderTemplate>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="panel ebiz-seat-details">
                            <div class="row">
                                <div class="large-3 columns">
                                    <asp:Literal ID="ltlSeatDetailsStandLabel" runat="server" />
                                </div>
                                <div class="large-9 columns">
                                    <asp:Literal ID="ltlSeatDetailsStandValue" runat="server" />
                                </div>
                                <div class="large-3 columns">
                                    <asp:Literal ID="ltlSeatDetailsAreaLabel" runat="server" />
                                </div>
                                <div class="large-9 columns">
                                    <asp:Literal ID="ltlSeatDetailsAreaValue" runat="server" />
                                </div>
                                <div class="large-3 columns">
                                    <asp:Literal ID="ltlSeatDetailsRowLabel" runat="server" />
                                </div>
                                <div class="large-9 columns">
                                    <asp:Literal ID="ltlSeatDetailsRowValue" runat="server" />
                                </div>
                                <div class="large-3 columns">
                                    <asp:Literal ID="ltlSeatDetailsSeatLabel" runat="server" />
                                </div>
                                <div class="large-9 columns">
                                    <asp:Literal ID="ltlSeatDetailsSeatValue" runat="server" />
                                </div>
                            </div>
                        </div>
                        </ItemTemplate>
                        <FooterTemplate></FooterTemplate>
                </asp:Repeater>
            </asp:PlaceHolder>

            <div class="alert-box info">
                <div class="row ebiz-quantity-requested">
                    <div class="medium-3 columns">
                        <asp:Literal ID="ltlQuantityRequestedLabel" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:Literal ID="ltlQuantityRequestedValue" runat="server" />
                    </div>
                </div>
            </div>

        </div>
        <button class="close-button" data-close aria-label="Close modal" type="button">
            <span aria-hidden="true"><i class="fa fa-times"></i></span>
        </button>
    </div>
    <Talent:HTMLInclude ID="uscHTMLInclude2" runat="server" Usage="2" Sequence="2" />

    <div id="ticketing-products" class="ebiz-accordion ebiz-ticketing-products">
        <asp:Repeater ID="rptLinkedProducts" runat="server" ViewStateMode="Enabled">
            <ItemTemplate>
                <div class="ebiz-linked-products <%# DataBinder.Eval(Container.DataItem, "RELATED_CSS_CLASS").ToString()%><%# GetMandatoryClassName(DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT_MANDATORY"))%>">
                    <asp:HiddenField ID="hdfProductCode" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT").ToString() %>' />
                    <asp:HiddenField ID="hdfProductType" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_TYPE").ToString() %>' />
                    <asp:HiddenField ID="hdfProductSubType" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_SUB_TYPE").ToString() %>' />
                    <asp:HiddenField ID="hdfPriceCode" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_PRICE_CODE").ToString() %>' />
                    <asp:HiddenField ID="hdfCampaignCode" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_CAMPAIGN_CODE").ToString() %>' />
                    <asp:HiddenField ID="hdfProductIsMandatory" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "RELATED_PRODUCT_MANDATORY").ToString() %>' />
                    <asp:HiddenField ID="hdfStadiumCode" runat="server" />
                    <asp:HiddenField ID="hdfDefaultPriceCode" runat="server" />
                    <asp:HiddenField ID="hdfDefaultPriceBand" runat="server" />
                    <asp:HiddenField ID="hdfProductDetailCode" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_STAND").ToString() & DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_AREA").ToString() %>' />
                    <asp:HiddenField ID="hdfQuantity" runat="server" />
                    <div runat="server" id="divAccordionHeader">
                        
                        <div class="ebiz-product-header">
                            <h2><div runat="server" style="display:none" id="divPurchasedIcon"><asp:Literal ID="ltlItemsPurchasedIndicator" runat="server"/></div> <asp:Literal ID="ltlProductDescription1" runat="server" /></h2>
                        </div>
                        <div class="ebiz-product-wrap">
                            <div class="ebiz-product-logos">
                                <asp:Image ID="imgOpposition" runat="server" CssClass="ebiz-opposition" />
                            </div>
                            <div class="ebiz-product-details">
                                <asp:PlaceHolder ID="plhProductDescription2" runat="server">
                                    <div class="ebiz-description-2"><asp:Literal ID="ltlProductDescription2" runat="server" /></div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhDateAndTime" runat="server">
                                    <div class="ebiz-date-time">
                                        <span class="ebiz-date"><asp:Literal ID="ltlDate" runat="server" /></span>
                                        <span class="ebiz-time"><asp:Literal ID="ltlTime" runat="server" /></span>
                                    </div>
                                </asp:PlaceHolder>
                            </div>
                        </div>

                         

                        
                        
                        <asp:PlaceHolder ID="plhInstructions" runat="server">
                            <div class="alert-box info ebiz-instructions"><asp:Literal ID="ltlInstructions" runat="server" /></div>
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
                        
                    </div>
                    <div runat="server" id="divAccordionContent">
                        <asp:PlaceHolder ID="plhAdditionalInformation" runat="server">
                            <p class="ebiz-additional-information"><asp:HyperLink ID="hlkAdditionalInformation" data-open="additional-information-modal" CssClass="button ebiz-open-modal" runat="server" /></p>
                            <div id="additional-information-modal" class="reveal ebiz-reveal-ajax" data-reveal></div>                            
                        </asp:PlaceHolder>

                        <asp:PlaceHolder ID="plhQuantity" runat="server">
                            <asp:PlaceHolder ID="plhPriceCodeDropdown" runat="server">
                                <div class="row ebiz-product-dropdown">
                                    <div class="large-3 columns">
                                        <asp:Label ID="lblPriceCodeDropdownDescription" runat="server" AssociatedControlID="ddlPriceCodes" CssClass="middle" />
                                    </div>
                                    <div class="large-9 columns">
                                        <asp:DropDownList ID="ddlPriceCodes" runat="server" />
                                    </div>
                                </div>
                            </asp:PlaceHolder>
                            <div class="row">
                                <asp:placeholder id="plhSeatSelectionLink" runat="server">
                                    <div class="medium-6 columns hide-for-small ebiz-seat-selection-wrap">
                                        <asp:HyperLink ID="hlkSeatSelection" CssClass="button" runat="server" />
                                        <asp:Button ID="btnSeatSelection" CssClass="button" runat="server" CommandName="SelectSeats" />
                                    </div>
                                </asp:placeholder>
                                <div class="<%# GetCSSColumnClass(DataBinder.Eval(Container.DataItem, "RELATED_TICKETING_PRODUCT_TYPE").ToString()) %> small-12 columns ebiz-linked-products-sas-wrap">
                                    <Talent:StandAndAreaSelection ID="uscStandAndAreaSelection" runat="server" HideBuyingOptions="true" />
                                    <Talent:PriceBandSelection ID="uscPriceBandSelection" runat="server" IsLinkedProduct="True" Visible="false" />
                                    <asp:placeholder id="plhTicketQuantity" runat="server">
                                        <div class="row ebiz-quantity">
                                            <div class="medium-3 columns">
                                                <asp:Label ID="lblQuantity" runat="server" AssociatedControlID="txtQuantity" />
                                            </div>
                                            <div class="medium-3 columns">
                                                <asp:TextBox ID="txtQuantity" runat="server" type="number"  />
                                                <asp:RegularExpressionValidator ID="rgxQuantity" runat="server" ValidationExpression="^[0-9]{0,2}$" ControlToValidate="txtQuantity" Display="None" CssClass="error" SetFocusOnError="true" ValidationGroup="LinkedProducts" />
                                                <asp:RangeValidator ID="rngQuantity" runat="server" ControlToValidate="txtQuantity" Display="None" CssClass="error" SetFocusOnError="true" ValidationGroup="LinkedProducts" EnableClientScript="true" />
                                                <asp:RequiredFieldValidator ID="rfvQuantity" runat="server" ControlToValidate="txtQuantity" Display="None" CssClass="error" SetFocusOnError="true" ValidationGroup="LinkedProducts" />
                                            </div>
                                            <div class="medium-6 columns">
                                                <asp:placeholder id="plhNumberOfTickets" runat="server">    
                                                    <div class="alert-box info">
                                                        <asp:Literal ID="ltlNumberOfTickets" runat="server" />
                                                    </div>
                                                </asp:placeholder>
                                            </div>
                                        </div>
                                    </asp:placeholder>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhSoldOut" runat="server">
                            <div class="alert-box warning"><asp:Literal ID="ltlSoldOut" runat="server" /></div>
                        </asp:PlaceHolder>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    
    <script type="text/javascript">
        function removeAllOptions(ddl) {
            var i;
            for (i = ddl.options.length - 1; i >= 0; i--) {
                ddl.remove(i);
            }
        }
        function addOption(selectbox, value, text) {
            var optn = document.createElement("OPTION");
            optn.text = text;
            optn.value = value;
            selectbox.options.add(optn);
        }
    
        function addCSSOnQuantityChange(obj,accItem) {
            var quantity = obj.value;
            var accord = ".accordion_item_no_" + accItem;
            if (quantity > 0) {
                $(accord).show();
            }
            else {
                //$(accord).hide();
            }
        }
    </script>
    <div class="stacked-for-small button-group ebiz-linked-products-buttons-top-wrap">
        <asp:Button ID="btnBasket" CssClass="button ebiz-go-to-basket" runat="server" ValidationGroup="LinkedProducts" />
        <asp:Button ID="btnBuy" CssClass="button ebiz-primary-action ebiz-buy" runat="server" ValidationGroup="LinkedProducts" CausesValidation="true" />
    </div>
    <Talent:HTMLInclude ID="uscHTMLInclude3" runat="server" Usage="2" Sequence="3" />
</asp:Content>