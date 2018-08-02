<%@ Page Language="VB" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeFile="HospitalityBooking.aspx.vb" Inherits="PagesPublic_Hospitality_HospitalityBooking" ViewStateMode="Disabled" %>

<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityFixturePackageHeader.ascx" TagName="HospitalityFixturePackageHeader" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityBookingExtras.ascx" TagName="HospitalityBookingExtras" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Hospitality/HospitalityBookingPackageDiscount.ascx" TagName="HospitalityBookingPackageDiscount" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ReserveTickets.ascx" TagPrefix="Talent" TagName="ReserveBooking" %>


<asp:content id="ContentHead" contentplaceholderid="ContentPlaceHolder2" runat="server">
</asp:content>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
<asp:Panel ID="pnlHospitalityBooking" runat="server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
     <Talent:ReserveBooking runat="server" id="uscReserveTickets" Visible="False" />
    <asp:PlaceHolder ID="plhBookingErrors" runat="server">
        <div class="alert-box alert"><asp:Literal ID="ltlBookingErrors" runat="server" /></div>
    </asp:PlaceHolder>   
	<asp:PlaceHolder ID="plhWarningMessage" runat="server">
	    <div class="alert-box warning"><asp:Literal ID="ltlWarningMessage" runat="server" /></div>
	</asp:PlaceHolder>
    <asp:PlaceHolder ID="plhSuccessMessage" runat="server">
        <div class="alert-box success"><asp:Literal ID="ltlSuccessMessage" runat="server" /></div>
    </asp:PlaceHolder>
    
    <div id="reserveValidationErrors" class="alert-box alert" style="display:none">
        <ul>
        </ul>
    </div>

    <asp:ValidationSummary ID="vlsHospitalityBookingErrors" runat="server" ValidationGroup="HospitalityBooking" ShowSummary="true" CssClass="alert-box alert ebiz-booking-errors"/>
    <asp:ValidationSummary ID="vlsHospitalityBookingExtrasErrors" runat="server" ValidationGroup="HospitalityBookingExtras" ShowSummary="true" CssClass="alert-box alert ebiz-booking-errors" />

    <asp:PlaceHolder ID="plhBookingDetails" runat="server">
        <Talent:HospitalityFixturePackageHeader ID="uscHospitalityFixturePackageHeader" runat="server" />

        <asp:Placeholder ID="plhBookingOptions" runat="server">
          <div class="panel c-hospitality-booking-options">
            <asp:Placeholder ID="plhLeadSource" runat="server">
              <div class="row c-hospitality-lead">
                  <div class="columns medium-3">                
                    <asp:Label ID="lblLeadSource" class="medium-middle" runat="server" AssociatedControlID="ddlLeadSource"/>
                  </div>            
                  <div class="columns medium-9">
                    <asp:DropDownList ID="ddlLeadSource" runat="server" ValidationGroup="HospitalityBooking" ClientIdMode="Static" />
                    <asp:RequiredFieldValidator ID="rfvLeadSource" runat="server" ControlToValidate="ddlLeadSource" ValidationGroup="HospitalityBooking" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" EnableClientScript="true" InitialValue="-1" />
                  </div>
              </div>
            </asp:Placeholder>
    
            <asp:Placeholder ID="plhBookingType" runat="server"> 
            <fieldset class="o-radio-group">
                <legend>             
                    <asp:Label ID="lblBookingType" runat="server" />
                </legend>   
                <asp:RadioButtonList runat="server" ID="rblMarkOrderFor" AutoPostBack="false" ValidationGroup="HospitalityBooking" RepeatLayout="Flow" ClientIdMode="Static">
                    <asp:ListItem Text="lblPersonal" Value="rdoPersonal" class="overflow-hidden" />
                    <asp:ListItem Text="lblBusiness" Value="rdoBusiness" class="overflow-hidden" />
                </asp:RadioButtonList>
                <asp:RequiredFieldValidator ID="rfvBookingOptions" runat="server" ControlToValidate="rblMarkOrderFor" ValidationGroup="HospitalityBooking" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" EnableClientScript="true"/>
            </fieldset>
            </asp:Placeholder> 
          </div>
        </asp:PlaceHolder>

        <%-- Box Office View --%>
        <asp:PlaceHolder ID="plhBoxOfficeMode" runat="server">
            <div class="c-package-components">
                <div class="row">
                    <div class="column">
                        <div class="panel">
                            <asp:PlaceHolder ID="plhBoxOfficeSeatedComponents" runat="server">
                                <div class="row">
                                    <div class="columns <%=BookingHeadingDynamicCSSClass%>">
                                        <div class="f2 h-title"><%=PackageDescription%></div>
                                    </div>
                                    <asp:PlaceHolder ID="plhViewFromArea1" runat="server">
                                        <div class="columns medium-3 medium-text-right">
                                            <div class="f2 h-title c-package-components__view-from-area"><asp:HyperLink ID="hlkView1" runat="server" /></div>
                                        </div>
                                    </asp:PlaceHolder>
                                </div>
                                <asp:PlaceHolder ID="plhBoxOfficeSTExceptionWarning" runat="server" Visible="false">
                                  <div class="alert-box warning">
                                    <asp:Literal ID="ltlSTExceptionWarningMessage" runat="server" />
                                  </div>
                                </asp:PlaceHolder>
                    
                                <ul class="accordion" data-accordion data-allow-all-closed="true" data-multi-expand="true">
                                    <asp:Repeater ID="rptBoxOfficeSeatedComponents" runat="server" OnItemCommand="rptBoxOfficeSeatedComponents_ItemCommand">
                                        <HeaderTemplate></HeaderTemplate>
                                        <ItemTemplate>
                                            <li class="accordion-item <%# If(Eval("ComponentType") = "A", "is-active", String.Empty) %> <%# Eval("ComponentType") %>" data-accordion-item onclick="CollapseAccordion(this)">

                                                <!-- Accordion tab title -->
                                                <a href="#" class="accordion-title pr5">
                                                    <div class="row">
                                                        <div class="medium-6 columns">
                                                            <asp:Literal ID="ltlComponentDescription" runat="server" />
                                                        </div>
                                                        <asp:PlaceHolder ID="plhComponentPriceTop" runat="server">
                                                            <div class="medium-6 columns medium-text-right">
                                                                <%# Eval("FormattedPriceBeforeVAT")%>
                                                            </div>
                                                        </asp:PlaceHolder>
                                                    </div>
                                                </a>

                                                <!-- Accordion tab content -->
                                                <div class="accordion-content" data-tab-content>
                                                    <asp:Repeater ID="rptBoxOfficeSeats" runat="server" OnItemDataBound="rptBoxOfficeSeats_ItemDataBound" OnItemCommand="rptBoxOfficeSeats_ItemCommand">
                                                        <HeaderTemplate>
                                                            <table class="ebiz-responsive-table o-admin-table js-admin-table c-package-components__table">
                                                                <thead>
                                                                    <tr>
                                                                        <th class="c-package-components__component"><asp:Literal ID="ltlComponentDescription" runat="server" /></th>
                                                                        <th class="c-package-components__customer"><%=CustomerHeaderText%></th>
                                                                        <th class="c-package-components__price-band"><%=PriceBandHeaderText%></th>
                                                                        <th class="c-package-components__price-code"><%=PriceCodeHeaderText%></th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                    <tr id="trBoxOfficeSeatsItemHeader" runat="server">                                                                     
                                                                        <td  id="tdBoxOfficeSeatsItemHeader" class="c-package-components__component" runat="server">
                                                                            <a href="#" class="o-admin-table__menu-trigger"><i class="fa fa-angle-down" aria-hidden="true"></i> <%# Eval("SeatedComponentFormattedDisplay")%></a>
                                                                            <asp:HiddenField ID="hdfSeatDetails" runat="server" Value='<%# Eval("SeatDetails")%>' />
                                                                            <asp:HiddenField ID="hdfAlphaSuffix" runat="server" Value='<%# Eval("AlphaSuffix")%>' />
                                                                            <asp:HiddenField ID="hdfFormattedSeatDetails" runat="server" Value='<%# Eval("FormattedSeatDetails")%>' />
                                                                            <asp:HiddenField ID="hdfStandCode" runat="server" Value='<%# Eval("StandCode").ToString().Trim()%>' />
                                                                            <asp:HiddenField ID="hdfAreaCode" runat="server" Value='<%# Eval("AreaCode").ToString().Trim()%>' />
                                                                            <asp:HiddenField ID="hdfComponentType" runat="server" Value='<%# Eval("ComponentType")%>' />
                                                                            <div class="js-admin-table__menu o-admin-table__menu">
                                                                                <ul class="menu">
                                                                                    
                                                                                    <asp:PlaceHolder ID="plhChangeSeat" runat="server">
                                                                                        <li><asp:LinkButton ID="lbtnChangeSeat" runat="server" CommandName="ChangeSeat" CommandArgument='<%# Eval("ComponentID")%>'><i class="fa fa-pencil" aria-hidden="true"></i> <%=ChangeComponentSeatText%></asp:LinkButton></li>
                                                                                    </asp:PlaceHolder>
                                                                                    <asp:PlaceHolder ID="plhSTException" runat="server" visible="false">
                                                                                        <li><a data-open="exampleModalSTExceptions<%# Eval("FormattedSeatDetails").ToString().Replace("/", "")%>"><i class="fa fa-exclamation-triangle"></i> <asp:Literal ID="ltlSTExceptionLink" runat="server" /></a></li>                                                 
                                                                                        <div class="reveal c-package-components__exceptions-reveal" id="exampleModalSTExceptions<%# Eval("FormattedSeatDetails").ToString().Replace("/", "")%>" data-reveal>
                                                                                        <h2><asp:Literal ID="ltlSTExceptionHeader" runat="server" /> </h2>                                                                                       
                                                                                            <asp:Repeater ID="rptBoxOfficeSTExceptionDetails" runat="server">
                                                                                                <HeaderTemplate>
                                                                                                    <table class="ebiz-responsive-table o-admin-table js-admin-table c-package-components__table">                                                                                                       
                                                                                                          <tbody>
                                                                                                </HeaderTemplate>
                                                                                                <ItemTemplate>
                                                                                                    <tr class="row mb3">
                                                                                                        <td class="medium-6 columns"><strong><%#Eval("ProductDescription1")%></strong></td>                            
                                                                                                        <td class="medium-6 columns"><i class="fa fa-calendar"></i> <%#Eval("ProductDate")%></td>                                                                                                    
                                                                                                    </tr>
                                                                                                </ItemTemplate>
                                                                                                <FooterTemplate>
                                                                                                       </tbody>
                                                                                                      </table>
                                                                                                </FooterTemplate>
                                                                                            </asp:Repeater>                                                                                           
                                                                                        <button class="close-button" data-close aria-label="Close modal" type="button">
                                                                                          <i class="fa fa-times"></i>
                                                                                        </button>
                                                                                      </div> 
                                                                                    </asp:PlaceHolder>                                                                             
                                                                                    <asp:PlaceHolder ID="plhRemoveForComponent" runat="server">
                                                                                        <li><asp:LinkButton ID="lbtnRemoveForComponent" runat="server" CommandName="RemoveForComponent" CommandArgument='<%# Eval("ComponentID")%>'><i class="fa fa-trash-o" aria-hidden="true"></i><%=RemoveForComponentText%></asp:LinkButton></li>
                                                                                    </asp:PlaceHolder>
                                                                                    <asp:PlaceHolder ID="plhCantDeleteComponent" runat="server">
                                                                                        <li><span class="disabled" title="<%=CantRemoveSeatWarningText%>"><i class="fa fa-trash-o" aria-hidden="true"></i> <%=RemoveForComponentText%></span></li>
                                                                                    </asp:PlaceHolder>
                                                                                    <asp:PlaceHolder ID="plhPrintSingleSeat" runat="server">
                                                                                        <li>   
                                                                                            <a href="#" OnClick="PrintSingleSeat('<%# Eval("SeatDetails")%>','<%# Eval("ComponentID")%>','<%# Eval("FormattedSeatDetails")%>'); return false;"><i class="fa fa-print" aria-hidden="true"></i> <%=PrintSingleSeatLinkText %></a>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
                                                                                        </li>  
                                                                                    </asp:PlaceHolder>                                                                                
                                                                                </ul>
                                                                            </div>
                                                                            <asp:HiddenField ID="hdfComponentID" runat="server" Value='<%# Eval("ComponentID")%>' />
                                                                        </td>
                                                                        <td class="c-package-components__customer" data-title="<%=CustomerHeaderText%>">
                                                                            <asp:DropDownList ID="ddlCustomer" runat="server" />
                                                                        </td>
                                                                        <td class="c-package-components__price-band" data-title="<%=PriceBandHeaderText%>">
                                                                            <asp:DropDownList ID="ddlPriceBands" runat="server" />
                                                                        </td>
                                                                        <td class="c-package-components__price-code" data-title="<%=PriceCodeHeaderText%>">
                                                                            <asp:DropDownList ID="ddlPriceCodes" runat="server" />
                                                                        </td>
                                                                    </tr>
                                                            </ItemTemplate>
                                                            <FooterTemplate>
                                                                </tbody>
                                                            </table>
                                                        </FooterTemplate>
                                                    </asp:Repeater>

                                                    <asp:HiddenField ID="hdfComponentID" runat="server" Value='<%# Eval("ComponentID")%>' />
                                                    <asp:HiddenField ID="hdfQuantity" runat="server" />
                                                    <asp:HiddenField ID="hdfStandCode" runat="server"/>
                                                    <asp:HiddenField ID="hdfAreaCode" runat="server"/>
                                                    <asp:HiddenField ID="hdfComponentType" runat="server" />   

                                                    <ul class="menu">                                                     
                                                        <asp:PlaceHolder ID="plhAddMoreSeats" runat="server">
                                                    	    <li><asp:LinkButton ID="lbtnAddSeat" runat="server" CommandName="AddSeatUsingBestAvailable" CommandArgument='<%# Eval("ComponentID")%>'><i class="fa fa-plus" aria-hidden="true"></i> <%=AddSeatUsingBestAvailableText%></asp:LinkButton></li>
                                                        </asp:PlaceHolder>                                                       
                                                        <asp:PlaceHolder ID="plhCantAddMoreSeats" runat="server">
                                                          <li><span class="disabled" title="<%=CantAddMoreSeatsWarningText%>"><i class="fa fa-plus" aria-hidden="true"></i> <%=AddSeatUsingBestAvailableText%></span></li>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="plhChangeAllSeats" runat="server">
                                                            <li><asp:LinkButton ID="lbtnChangeAllSeats" runat="server" CommandName="ChangeAllSeats" CommandArgument='<%# Eval("ComponentID")%>'><i class="fa fa-refresh" aria-hidden="true"></i> <%=ChangeAllComponentSeatsText%></asp:LinkButton></li>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="plhRemoveAllForComponent" runat="server">
                                                            <li><asp:LinkButton ID="lbtnRemoveAllForComponent" runat="server" CommandName="RemoveAllForComponent" CommandArgument='<%# Eval("ComponentID")%>'><i class="fa fa-trash-o" aria-hidden="true"></i> <%=RemoveAllForComponentText%></asp:LinkButton></li>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="plhCantRemoveWholeComponentAvailability" runat="server">
                                                            <li><span class="disabled" title="<%=CantRemoveWholeComponentAvailabilityWarningText%>"><i class="fa fa-trash-o" aria-hidden="true"></i> <%=RemoveAllForComponentText%></span></li>
                                                        </asp:PlaceHolder>
                                                        <asp:PlaceHolder ID="plhCantRemoveWholeComponentDueToMinQty" runat="server">
                                                            <li><span class="disabled" title="<%=CantRemoveWholeComponentDutToMinQtyWarningText%>"><i class="fa fa-trash-o" aria-hidden="true"></i> <%=RemoveAllForComponentText%></span></li>
                                                        </asp:PlaceHolder>
                                                    </ul>
                                                    <asp:PlaceHolder ID="plhComponentPriceBottom" runat="server">                                            
                                                        <div class="row c-package-components__price">
                                                            <div class="column text-right">
                                                                <asp:PlaceHolder ID="plhComponentDiscountedPrice" runat="server">
                                                                    <span class="f2 strike"><%# Eval("formattedPriceBeforeVATExclDisc")%></span>
                                                                    <span class="f2 discount-price"><%# Eval("FormattedPriceBeforeVAT")%></span>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder ID="plhComponentPrice" runat="server">
                                                                    <span class="f2"><%# Eval("FormattedPriceBeforeVAT")%></span>
                                                                </asp:PlaceHolder>
                                                            </div>
                                                        </div><!-- /.c-package-components__price -->
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="plhComponentDiscount" runat="server">
                                                        <div class="row c-package-components__discount">
                                                            <div class="small-6 columns">
                                                                <label class="large-middle"><%=PercentageDiscountText%></label>
                                                            </div>
                                                            <div class="small-6 columns">
                                                                <div class="input-group ml-auto w4">
                                                                    <span class="input-group-label">%</span>
                                                                    <asp:TextBox ID="txtComponentDiscountPercentage" type="number" runat="server" CssClass="text-right mb0 c-package-components__discount-textbox" />
                                                                    <asp:RangeValidator ID="rngComponentDiscountPercentage" runat="server" ControlToValidate="txtComponentDiscountPercentage" Display="None" CssClass="error" SetFocusOnError="true" ValidationGroup="HospitalityBooking" EnableClientScript="true" />
                                                                    <asp:RequiredFieldValidator ID="rfvComponentDiscountPercentage" runat="server" Display="None" SetFocusOnError="true" ControlToValidate="txtComponentDiscountPercentage" CssClass="error" ValidationGroup="HospitalityBooking" />
                                                                    <asp:HiddenField ID="hdfMaxComponentDiscountPercentage" runat="server" ClientIDMode="Static"/>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </asp:PlaceHolder>
                                                </div>
                                            </li>
                                        </ItemTemplate>
                                        <FooterTemplate></FooterTemplate>
                                    </asp:Repeater>
                                </ul>

                            </asp:PlaceHolder>
                     

                            <asp:PlaceHolder ID="plhBoxOfficePackageIncludedExtras" runat="server">
                                <div class="f2 h-title"><asp:Literal ID="ltlBoxOfficePackageIncludedExtras" runat="server" /></div>
                                <ul class="accordion" data-accordion  data-allow-all-closed="true">
                                   <li class="accordion-item" data-accordion-item onclick="CollapseAccordion(this)">
                                      <!-- Accordion tab title -->
                                        <a href="#" class="accordion-title">Included extras</a>
                                        <!-- Accordion tab content: it would start in the open state due to using the `is-active` state class. -->
                                          <div class="accordion-content" data-tab-content>
                                            <asp:Repeater ID="rptBoxOfficeNonSeatedComponents" runat="server">
                                                <HeaderTemplate>
                                                    <table id="" class="ebiz-responsive-table o-admin-table js-admin-table c-package-components__included-components">
                                                        <thead>
                                                            <tr>
                                                                <th class="c-package-components__component"><%=IncludedExtrasHeaderText%></th>
                                                                <th class="c-package-components__quanity"><%=QuantityHeaderText%></th>
                                                                <asp:PlaceHolder ID="plhPriceColumnHeader" runat="server" Visible="<%#IsPriceColumnVisible%>">
                                                                    <th class="c-package-components__price large-text-right"><%=PriceHeaderText%></th>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder ID="plhComponentDiscount" runat="server" Visible="<%#IsPriceColumnVisible%>">
                                                                <th class="c-package-components__discount"><%=PercentageDiscountText%></th>
                                                                </asp:PlaceHolder>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                            <tr>
                                                                <td class="c-package-components__component" data-title="<%=IncludedExtrasHeaderText%>">
                                                                    <a href="#" class="o-admin-table__menu-trigger"><i class="fa fa-angle-down" aria-hidden="true"></i> <%# Eval("ComponentDescription")%></a>
                                                                    <div class="js-admin-table__menu o-admin-table__menu">
                                                                        <ul class="menu">
                                                                            <asp:PlaceHolder ID="plhRemoveForComponent" runat="server">                                                                                 
                                                                                <li><asp:LinkButton ID="lbtnRemoveForComponent" runat="server" CommandName="RemoveForComponent" CommandArgument='<%# Eval("ComponentID")%>'><i class="fa fa-trash-o" aria-hidden="true"></i><%=RemoveForComponentText %></asp:LinkButton></li>
                                                                            </asp:PlaceHolder>
                                                                            <asp:PlaceHolder ID="plhCantDeleteComponent" runat="server">
	                                                                            <li><span class="disabled" title="<%=CantRemoveNonSeatedComponentDueToMinQtyWarningText%>"><i class="fa fa-trash-o" aria-hidden="true"></i> <%=RemoveForComponentText%></span></li>
                                                                            </asp:PlaceHolder>
                                                                        </ul>
                                                                    </div>
                                                                </td>
                                                                <td class="c-package-components__quantity" data-title="<%=QuantityHeaderText%>">
                                                                    <asp:DropDownList ID="ddlComponentQuantity" CssClass="w3" runat="server" />
                                                                    <asp:HiddenField ID="hdfComponentID" runat="server" Value='<%# Eval("ComponentID")%>' />
                                                                </td>
                                                                <asp:PlaceHolder ID="plhPriceColumn" runat="server" Visible="<%#IsPriceColumnVisible%>">
                                                                    <td class="c-package-components__price large-text-right" data-title="<%=PriceHeaderText%>">
                                                                        <asp:PlaceHolder ID="plhComponentDiscountedPrice" runat="server">
                                                                            <span class="strike pr3"><%#Eval("formattedPriceBeforeVATExclDisc")%></span>
                                                                            <span class="discount-price"><%# Eval("FormattedPriceBeforeVAT")%></span>
                                                                        </asp:PlaceHolder>
                                                                        <asp:PlaceHolder ID="plhComponentPrice" runat="server">
                                                                            <span class="full-price"><%# Eval("FormattedPriceBeforeVAT")%></span>
                                                                        </asp:PlaceHolder>
                                                                    </td>
                                                                </asp:PlaceHolder>
                                                                <asp:PlaceHolder ID="plhComponentDiscount" runat="server" Visible="<%#IsPriceColumnVisible%>">
                                                                    <td class="c-package-components__discount" data-title="<%=PercentageDiscountText %>">
                                                                        <div class="input-group ml-auto-l w4">
                                                                            <span class="input-group-label">%</span>
                                                                            <asp:TextBox ID="txtComponentDiscountPercentage" type="number" runat="server" CssClass="text-right c-package-components__discount-textbox" />
                                                                            <asp:RangeValidator ID="rngComponentDiscountPercentage" runat="server" ControlToValidate="txtComponentDiscountPercentage" Display="None" CssClass="error" SetFocusOnError="true" ValidationGroup="HospitalityBooking" EnableClientScript="true" />
                                                                            <asp:RequiredFieldValidator ID="rfvComponentDiscountPercentage" runat="server" Display="None" SetFocusOnError="true" ControlToValidate="txtComponentDiscountPercentage" CssClass="error" ValidationGroup="HospitalityBooking" />
                                                                            <asp:HiddenField ID="hdfMaxComponentDiscountPercentage" runat="server" ClientIDMode="Static"/>
                                                                        </div>
                                                                    </td>
                                                                </asp:PlaceHolder>
                                                            </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                        </tbody>
                                                    </table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                          </div>
                                     </li>
                                  </ul>
                              </asp:PlaceHolder>
                                                     
			                <asp:PlaceHolder ID="plhBoxOfficeExtraComponents" runat="server" Visible="False">
                                <div class="c-extra-components">
                                    <div class="f2 h-title">
                                        <asp:Literal ID="ltlBoxOfficePackageExtrasTitle" runat="server" />
                                    </div>
                                    <div class="row">
                                        <div class="columns medium-3">                
                                            <asp:Label ID="lblBoxOfficePackageExtrasDDLLabel" CssClass="medium-middle" runat="server" AssociatedControlID="ddlBoxOfficePackageExtras" />                                                
                                        </div>            
                                        <div class="columns medium-9">
                                            <asp:DropDownList ID="ddlBoxOfficePackageExtras" runat="server" ValidationGroup="HospitalityBookingExtras" />
                                            <asp:RequiredFieldValidator ID="rfvBoxOfficePackageExtras" runat="server" ControlToValidate="ddlBoxOfficePackageExtras" ValidationGroup="HospitalityBookingExtras" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" EnableClientScript="true" InitialValue="-1" />        
                                        </div>
                                    </div>
                                    <asp:LinkButton ID="lbtnBoxOfficeAddPackageExtras" runat="server" CssClass="button mb0" ValidationGroup="HospitalityBookingExtras"/>                                    
                                </div>
                            </asp:PlaceHolder>

                          </div>
                        </div>
                      </div>
                    </div>       
        </asp:PlaceHolder>

        <%-- PWS View --%>
        <asp:PlaceHolder ID="plhPublicWebSalesMode" runat="server">
            <div class="c-package-components">
                <div class="row">
                    <div class="column">
                        <div class="panel">

                            <asp:PlaceHolder ID="plhPWSSeatedComponents" runat="server">
                              <div class="c-package-components__components">
                                <div class="row">
                                    <div class="columns <%=BookingHeadingDynamicCSSClass%>">
                                        <div class="f2 h-title"><%=PackageDescription%></div>
                                    </div>
                                    <asp:PlaceHolder ID="plhViewFromArea2" runat="server">
                                        <div class="columns medium-3 medium-text-right">
                                            <div class="f2 h-title c-package-components__view-from-area"><asp:HyperLink ID="hlkView2" runat="server" /></div>
                                        </div>
                                    </asp:PlaceHolder>
                                </div>
                            
                                <asp:Repeater ID="rptPWSSeatedComponents"  OnItemCommand="rptPWSSeatedComponents_ItemCommand" runat="server">
                                    <HeaderTemplate></HeaderTemplate>
                                    <ItemTemplate>
                                      <div class="c-package-components__component">

                                        <asp:Repeater ID="rptPWSSeats" runat="server" OnItemDataBound="rptPWSSeats_ItemDataBound" OnItemCommand="rptPWSSeats_ItemCommand">
                                            <HeaderTemplate>
                                                <table class="ebiz-responsive-table">
                                                    <thead>
                                                        <tr>
                                                            <th class="w-8c-l c-package-components__components"><asp:Literal ID="ltlComponentDescription" runat="server" /></th>
                                                            <th class="w-2c-l c-package-components__customer"><%=CustomerHeaderText %></th>
                                                            <th class="w-2c-l c-package-components__price-band"><%=PriceBandHeaderText%></th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                        <tr>
                                                            <td class="w-8c-l c-package-components__components" id="tdPWSSeatsItemHeader" runat="server">
                                                                <span><%# Eval("SeatedComponentFormattedDisplay")%></span> 
                                                                 <ul class="menu">
                                                                    <asp:PlaceHolder ID="plhChangeSeat" runat="server">                
                                                                        <li><asp:LinkButton ID="lbtnChangeSeat" runat="server" CommandName="ChangeSeat" CommandArgument='<%# Eval("ComponentID")%>'><i class="fa fa-pencil" aria-hidden="true"></i> <%=ChangeComponentSeatText%></asp:LinkButton></li>
                                                                    </asp:PlaceHolder>
                                                                    <asp:PlaceHolder ID="plhRemoveForComponent" runat="server">
                                                                        <li><asp:LinkButton ID="lbtnRemoveForComponent" runat="server" CommandName="RemoveForComponent" CommandArgument='<%# Eval("ComponentID")%>'><i class="fa fa-trash-o" aria-hidden="true"></i><%=RemoveForComponentText%></asp:LinkButton></li>
                                                                    </asp:PlaceHolder>
                                                                 </ul>
                                                                <asp:HiddenField ID="hdfSeatDetails" runat="server" Value='<%# Eval("SeatDetails")%>' />
                                                                <asp:HiddenField ID="hdfAlphaSuffix" runat="server" Value='<%# Eval("AlphaSuffix")%>' />
                                                                <asp:HiddenField ID="hdfComponentID" runat="server" Value='<%# Eval("ComponentID")%>' />
                                                                <asp:HiddenField ID="hdfPriceCode" runat="server" Value='<%# Eval("PriceCode")%>' />
                                                                <asp:HiddenField ID="hdfFormattedSeatDetails" runat="server" Value='<%# Eval("FormattedSeatDetails")%>' />
                                                                <asp:HiddenField ID="hdfStandCode" runat="server" Value='<%# Eval("StandCode").ToString().Trim()%>' />
                                                                <asp:HiddenField ID="hdfAreaCode" runat="server" Value='<%# Eval("AreaCode").ToString().Trim()%>' />
                                                            </td>
                                                            <td class="w-2c-l c-package-components__customer" data-title="<%=CustomerHeaderText%>">
                                                                <asp:DropDownList ID="ddlCustomer" runat="server" />
                                                            </td>
                                                            <td class="w-2c-l c-package-components__price-band" data-title="<%=PriceBandHeaderText %>">
                                                                <asp:DropDownList ID="ddlPriceBands" runat="server" />
                                                                <asp:Literal ID="ltlPriceBand" runat="server" />
                                                            </td>
                                                        </tr>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                    </tbody>
                                                </table>
                                            </FooterTemplate>
                                        </asp:Repeater>

                                          <asp:HiddenField ID="hdfStandCode" runat="server" />
                                          <asp:HiddenField ID="hdfAreaCode" runat="server" />
                                        
                                          <ul class="menu">  
                                            <asp:PlaceHolder ID="plhChangeAllSeats" runat="server">                                            
                                                <li><asp:LinkButton ID="lbtnChangeAllSeats" runat="server" CommandName="ChangeAllSeats" CommandArgument='<%# Eval("ComponentID")%>'><i class="fa fa-refresh" aria-hidden="true"></i> <%=ChangeAllComponentSeatsText%></asp:LinkButton></li>                                          
                                            </asp:PlaceHolder>   
                                            <asp:PlaceHolder ID="plhRemoveAllForComponent" runat="server">
                                                <li><asp:LinkButton ID="lbtnRemoveAllForComponent" runat="server" CommandName="RemoveAllForComponent" CommandArgument='<%# Eval("ComponentID")%>'><i class="fa fa-trash-o" aria-hidden="true"></i> <%=RemoveAllForComponentText%></asp:LinkButton></li>
                                            </asp:PlaceHolder>
                                          </ul>
                                        <asp:PlaceHolder ID="plhComponentPrice" runat="server">
                                            <div class="row mb4 c-hosp-bkng-compo__desc-price">
                                                <div class="column text-right">
                                                    <div class="f2"><%# Eval("FormattedPriceBeforeVAT")%></div>
                                                </div>
                                            </div>
                                        </asp:PlaceHolder>
                                      </div>
                                    </ItemTemplate>
                                    <FooterTemplate></FooterTemplate>
                                </asp:Repeater>
                                </div>
                            </asp:PlaceHolder>


                            <asp:PlaceHolder ID="plhPWSPackageIncludedExtras" runat="server">
                                <div class="mb4 c-package-components__extras">
                                  <div class="f2 h-title"><asp:Literal ID="ltlPWSPackageIncludedExtras" runat="server" /></div>
                        
                                  <asp:Repeater ID="rptPWSNonSeatedComponents" runat="server">
                                      <HeaderTemplate>
                                          <table class="ebiz-responsive-table">
                                              <thead>
                                                  <tr>
                                                      <th><%=IncludedExtrasHeaderText%></th>
                                                      <th class="large-text-right"><%=QuantityHeaderText %></th>
                                                      <asp:PlaceHolder ID="plhPriceColumnHeader" runat="server" Visible="<%#IsPriceColumnVisible%>">
                                                          <th class="large-text-right"><%=PriceHeaderText %></th>
                                                      </asp:PlaceHolder>
                                                  </tr>
                                              </thead>
                                              <tbody>
                                      </HeaderTemplate>
                                      <ItemTemplate>
                                                  <tr>
                                                      <td data-title="<%=IncludedExtrasHeaderText%>"><%# Eval("ComponentDescription")%>
                                                          <ul class="menu">
                                                            <asp:PlaceHolder ID="plhRemoveForComponent" runat="server">                                                                                 
                                                                <li><asp:LinkButton ID="lbtnRemoveForComponent" runat="server" CommandName="RemoveForComponent" CommandArgument='<%# Eval("ComponentID")%>'><i class="fa fa-trash-o" aria-hidden="true"></i><%=RemoveForComponentText %></asp:LinkButton></li>
                                                            </asp:PlaceHolder>                                                                           
                                                          </ul>
                                                      </td>
                                                      <td  class="large-text-right" data-title="<%=QuantityHeaderText %>"><%# Eval("Quantity").ToString().TrimStart(GlobalConstants.LEADING_ZEROS)%></td>
                                                      <asp:PlaceHolder ID="plhPriceColumn" runat="server" Visible="<%#IsPriceColumnVisible%>">
                                                          <td data-title="<%=PriceHeaderText%>" class="large-text-right"><%# Eval("FormattedPriceBeforeVAT")%></td>
                                                      </asp:PlaceHolder>
                                                  </tr>
                                      </ItemTemplate>
                                      <FooterTemplate>
                                              </tbody>
                                          </table>
                                      </FooterTemplate>
                                  </asp:Repeater>
                                </div>
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID="plhPWSExtraComponents" runat="server" Visible="False">                            
                                <div class="c-package-components__add-extras">
                                    <div class="f2 h-title">
                                        <asp:Literal ID="ltlPWSPackageExtrasTitle" runat="server" />
                                    </div>
                                    <div class="row">
                                        <div class="columns medium-3">     
                                            <asp:Label ID="lblPWSPackageExtrasDDLLabel" CssClass="medium-middle" runat="server" AssociatedControlID="ddlPWSPackageExtras" />                                             
                                        </div>            
                                        <div class="columns medium-9">
                                            <asp:DropDownList ID="ddlPWSPackageExtras" runat="server" ValidationGroup="HospitalityBookingExtras"/>                                           
                                            <asp:RequiredFieldValidator ID="rfvPWSPackageExras" runat="server" ControlToValidate="ddlPWSPackageExtras" ValidationGroup="HospitalityBookingExtras" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" EnableClientScript="true" InitialValue="-1" />    
                                        </div>
                                    </div>
                                    <asp:LinkButton ID="lbtnPWSAddPackageExtras" runat="server" CssClass="button mb0" ValidationGroup="HospitalityBookingExtras"/>
                                </div>
                            </asp:PlaceHolder>                            

                        </div>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder id="plhQAndA" runat="server" Visible="False" ViewStateMode="Enabled">        
        <div id="accordioncontent" class="panel ebiz-content" runat="server">
            <div class="panel c-q-and-a">
                <div class="f2 h-title"><%=QuestionAndAnswerHeaderText%></div>
                    <ul class="accordion" data-accordion  data-allow-all-closed="true">
                        <li class="accordion-item <%=QAndAExpandDynamicClass%>" data-accordion-item onclick="CollapseAccordion(this)">        
                                <asp:Repeater ID="rptProductQuestions" runat="server" Visible="True" OnItemDataBound="rptProductQuestions_ItemDatabound">
                                   <HeaderTemplate>
                                       <!-- Accordion tab title -->
                                       <a href="#" class="accordion-title"><%=AdditionalQuestionText%></a>
                                       <!-- Accordion tab content: it would start in the open state due to using the `is-active` state class. -->                   
                                   <div class="accordion-content" data-tab-content>
                                   </HeaderTemplate>
                                   <ItemTemplate>
                                       <asp:PlaceHolder id="plhFreeTextField" runat="server" Visible="False" ViewStateMode="Enabled">
                                           <div class="row c-q-and-a__input">
                                               <div class="medium-3 columns">
                                                   <asp:Label runat="server" ID="lblQuestionText" AssociatedControlID="txtQuestionAnswerText" CssClass="middle" />
                                               </div>
                                               <div class="medium-9 columns">
                                                   <asp:TextBox runat="server" ID="txtQuestionAnswerText" onChange="updatePackage()" />
                                               </div>
                                           </div>
                                       </asp:PlaceHolder>
                                       <asp:PlaceHolder id="plhCheckbox" runat="server" Visible="False"  ViewStateMode="Enabled">
                                           <div class="row c-q-and-a__checkbox">
                                               <div class="medium-3 columns">
                                                   <asp:Label runat="server" ID="lblQuestionCheckText" AssociatedControlID="chkQuestionCheckText" CssClass="middle" />
                                               </div>
                                               <div class="medium-9 columns">
                                                   <asp:CheckBox runat="server" ID="chkQuestionCheckText" OnClick="updatePackage()" /> 
                                               </div>
                                           </div>
                                       </asp:PlaceHolder>
                                       <asp:PlaceHolder id="plhDate" runat="server" Visible="False"  ViewStateMode="Enabled">
                                           <div class="row c-q-and-a__input">
                                               <div class="medium-3 columns">
                                                   <asp:Label runat="server" ID="lblDate" AssociatedControlID="txtDate" CssClass="middle" />
                                               </div>
                                               <div class="medium-9 columns">
                                                   <asp:TextBox runat="server" ID="txtDate" CssClass="datepicker" onChange="updatePackage()"/>
                                               </div>
                                           </div>
                                       </asp:PlaceHolder>
                                       <asp:PlaceHolder id="plhListOfAnswers" runat="server" Visible="False">
                                           <div class="row c-q-and-a__select">
                                           <div class="medium-3 columns">
                                               <asp:Label runat="server" ID="lblListOfAnswers" AssociatedControlID="ddlAnswers" CssClass="middle" />
                                           </div>
                                           <div class="medium-9 columns">
                                               <asp:DropDownList ID="ddlAnswers" runat="server" onChange="updatePackage()" />
                                           </div>
                                       </div>
                                       </asp:PlaceHolder>
                                       <asp:HiddenField runat="server" ID="hdfAnswerType" />
                                       <asp:HiddenField runat="server" ID="hdfTemplateID" />
                                       <asp:HiddenField runat="server" ID="hdfQuestionID" />
                                       <asp:HiddenField runat="server" ID="hdfRememberedAnswer" />
                                       <asp:HiddenField runat="server" ID="hdfTotalNumberOfSeats" />
                                       <asp:HiddenField runat="server" ID="hdfNumberOfQuestion" />
                                       <asp:HiddenField runat="server" ID="hdfIsQuestionPerBooking" />
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </div>
                                    </FooterTemplate>
                                </asp:Repeater>                          
                        </li>
                    </ul>
                </div>
        </div>
        </asp:PlaceHolder>

        <div class="reveal" data-reveal id="view-area-1">
            <asp:Image ID="imgViewArea" runat="server" />
            <button class="close-button" data-close aria-label="Close modal" type="button">
                <span aria-hidden="true"><i class="fa fa-times" aria-hidden="true"></i></span>
            </button>
        </div>

        <div class="panel c-hospitality-totals">
            <div class="row">
                <div class="small-6 columns">
                    <span class="f2"><asp:Literal ID="ltlBookingTotalCostLabel" runat="server" /></span>
                </div>
                <div class="small-6 columns text-right">
                    <span class="f2"><asp:Literal ID="ltlBookingTotalCostExVAT" runat="server" /></span>
                </div>
            </div>
            <div class="row">
                <div class="small-6 columns">
                    <asp:Literal ID="ltlBookingVATLabel" runat="server" />
                </div>
                <div class="small-6 columns text-right">
                    <asp:Literal ID="ltlBookingVATValue" runat="server" />
                </div>
            </div>
            <asp:PlaceHolder ID="plhTotalOnly" runat="server">
                <div class="row">
                    <div class="small-6 columns">
                        <asp:Literal ID="ltlTOBookingTotalCostIncVATLabel" runat="server" />
                    </div>
                    <div class="small-6 columns text-right">
                        <asp:Literal ID="ltlTOBookingTotalCostIncVATValue" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhDiscountTotal" runat="server">
                <div class="row">
                    <div class="small-6 columns">
                        <asp:Literal ID="ltlDiscountLabel" runat="server" />
                    </div>
                    <div class="small-6 columns text-right">
                        <span class="discount-price"><asp:Literal ID="ltlDiscountValue" runat="server" /></span>
                    </div>
                </div>
                <div class="row">
                    <div class="small-6 columns">
                        <asp:Label ID="ltlBookingTotalCostIncVATLabel" runat="server" AssociatedControlID="txtBookingTotalCostIncVAT" CssClass="middle unstyled-label" />
                    </div>
                    <div class="small-6 columns">
                        <div class="input-group ml-auto w4">
                            <asp:PlaceHolder ID="plhLeftCurrencySymbol" runat="server"><span class="input-group-label input-group-label--prefix"><%=CurrencySymbol %></span></asp:PlaceHolder>
                            <span class="has-tip" runat="server" id="spanBookingTotalCostIncVAT">
                                <asp:TextBox ID="txtBookingTotalCostIncVAT" runat="server" ClientIDMode="Static" CssClass="text-right" />
                                <asp:RangeValidator ID="rngBookingTotalCostIncVAT" runat="server" ControlToValidate="txtBookingTotalCostIncVAT" Display="None" CssClass="error" SetFocusOnError="true" ValidationGroup="HospitalityBooking" EnableClientScript="true" />
                                <asp:RequiredFieldValidator ID="rfvBookingTotalCostIncVAT" runat="server" Display="None" SetFocusOnError="true" ControlToValidate="txtBookingTotalCostIncVAT" CssClass="error" ValidationGroup="HospitalityBooking" />
                            </span>
                            <asp:PlaceHolder ID="plhRightCurrencySymbol" runat="server"><span class="input-group-label input-group-label--suffixes"><%=CurrencySymbol %></span></asp:PlaceHolder>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div><!-- /.c-hospitality-totals -->


        <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />


        <div class="row c-hospitality-actions">
            <div class="medium-9 columns">
                <div class="button-group">
                    <asp:LinkButton ID="lbtnCancel" runat="server" CssClass="button ebiz-muted-action" OnClientClick="CancelButtonClick(); return false;" />
                    <asp:LinkButton ID="lbtnSaveAsEnquiry" runat="server" CssClass="button ebiz-primary-action" />                   
                    <asp:LinkButton ID="lbtnBackToBookingEnquiry" runat="server" CssClass="button ebiz-primary-action" />
                    <asp:HyperLink ID="hlkReserveBooking" CssClass="button ebiz-primary-action" runat="server" onClick="updateBookingAndOpenReserveModal();"></asp:HyperLink>
                    <asp:LinkButton ID="lbtnUpdate" runat="server" CssClass="button ebiz-primary-action" ValidationGroup="HospitalityBooking" />
                    <asp:LinkButton ID="lbtnContinue" runat="server" CssClass="button ebiz-primary-action" ValidationGroup="HospitalityBooking" />
                    <asp:LinkButton ID="lbtnRemoveDiscount" runat="server" CssClass="button ebiz-primary-action" />
                    <asp:LinkButton ID="lbtnUpdateForSoldBooking" runat="server" CssClass="button ebiz-primary-action" ValidationGroup="HospitalityBooking" />                                  
                </div>
            </div>
            <div class="medium-3 columns">              
                <div class="button-group">
                    <asp:placeholder ID="plhPrintBooking" runat="server">
                        <a href="#" class="button ebiz-primary-action" OnClick="PrintBookingClick();return false;"><i class="fa fa-print" aria-hidden="true"></i> <%=PrintBookingLinkText%></a>  
                    </asp:placeholder>
                    <asp:placeholder ID="plhGenerateDocument" runat="server">
                        <a href="#" class="button ebiz-primary-action" OnClick="GenerateDocumentForBookingClick();return false;"><i class="fa fa-file-word-o" aria-hidden="true"></i> <%=GenerateDocumentLinkText%></a>
                    </asp:placeholder>
                    <asp:PlaceHolder ID="plhCreatePDF" runat="server">
                        <asp:LinkButton ID="lbtnCreatePDF" runat="server" CssClass="button ebiz-primary-action" ValidationGroup="HospitalityBooking"><i class="fa fa-file-pdf-o" aria-hidden="true"></i> <%= CreatePDFLinkText%></asp:LinkButton>
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>

        <asp:HiddenField ID="hdfAlertifyTitle" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdfAlertifyMessage" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdfAlertifyOK" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdfAlertifyCancel" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdfCancelBooking" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdfMaxBookingTotalCostIncVAT" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="hdfFFRedirectURL" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdfNewCustomerRedirectText" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdfUpdatePackageMessageText" runat="server" ClientIDMode="Static" />
        <asp:HiddenField runat="server" ID="hdfDatePickerClearDateText"  ClientIDMode="Static" />

        <asp:HiddenField ID="hdfPrintSingleSeat" runat="server" ClientIDMode="Static"/> 
        <asp:HiddenField ID="hdfSeatToBePrinted" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="hdfFormattedSeatToBePrinted" runat="server" ClientIDMode="Static"/>  
        <asp:HiddenField ID="hdfComponentId" runat="server" ClientIDMode="Static"/>                 
        <asp:HiddenField ID="hdfAlertifyTitleForPrintSingleSeat" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="hdfAlertifyMessageForPrintSingleSeat" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="hdfAlertifyOKForPrintSingleSeat" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="hdfAlertifyCancelForPrintSingleSeat" runat="server" ClientIDMode="Static" />

        <asp:HiddenField ID="hdfAlertifyTitleForPrintBooking" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="hdfAlertifyMessageForPrintBooking" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="hdfAlertifyOKForPrintBooking" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="hdfAlertifyCancelForPrintBooking" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdfCallIdToBePrinted" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdfPrintBooking" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdfNumberOfTicketsToPrint" runat="server" ClientIDMode="Static" />

        <asp:HiddenField ID="hdfAlertifyTitleForSingleBookingDocument" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="hdfAlertifyMessageForSingleBookingDocument" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="hdfAlertifyOKForSingleBookingDocument" runat="server" ClientIDMode="Static"/>
        <asp:HiddenField ID="hdfAlertifyCancelForSingleBookingDocument" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdfCallIdForDocumentProduction" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdfGenerateBookingDocument" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdfMergedDocumentPath" runat="server" ClientIDMode="Static" />

        <asp:hiddenfield ID="hdfNoneReservationErrorCount" runat="server" ClientIDMode="Static"/>
        <asp:hiddenfield ID="hdfReserveClick" runat="server" ClientIDMode="Static"/>

    </asp:PlaceHolder>
    </asp:Panel>
</asp:content>
