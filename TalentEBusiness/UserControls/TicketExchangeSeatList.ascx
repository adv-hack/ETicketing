<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TicketExchangeSeatList.ascx.vb" Inherits="TalentEBusiness_UserControls_TicketExchangeSeatList" ViewStateMode="Disabled" %>

 <asp:PlaceHolder ID="plhTicketExchangeSeatList" runat="server" >
     <asp:PlaceHolder ID="plhMasterSlider" runat="server"  visible="False">
        <div class="panel ebiz-ticket-exchange-set-all-tickets-wrap">
            <h2><asp:Literal ID="ltlSetAllTicketsToThisPrice" runat="server" /> </h2>
            <div class="row ebiz-price-slider-wrap">
                <div class="small-10 columns">
                    <div class="slider ebiz-main-slider" data-slider data-start="<%=MainSliderMinPrice%>" data-end="<%=MainSliderMaxPrice%>" data-initial-start="<%=MainSliderInitialStart %>" data-step="<%=SliderStepNumber %>">
                        <span class="slider-handle" data-slider-handle role="slider" tabindex="1" aria-controls="ebiz-main-slider-value"></span>
                        <span class="slider-fill" data-slider-fill></span>
                    </div>
                </div>
                <div class="small-2 columns">
                    <input type="number" id="ebiz-main-slider-value" class="ebiz-price-slider-value">
                </div>
            </div>
        </div>
     </asp:PlaceHolder>

        <div class="panel ebiz-ticket-exchange-select-tickets-wrap">
            <h2><asp:Literal ID="ltlHeader" runat="server" /></h2>
            <asp:Repeater ID="rptTicketExchangeSeatList" runat="server" ViewStateMode="Enabled">
                <HeaderTemplate>
                    <table class="dataTable no-footer ebiz-responsive-table" role="grid">
                    <thead>
                        <tr>
                             <asp:PlaceHolder ID="plhStatusHeader" runat="server"> 
                                <th scope="col" class="ebiz-status">
                                    <asp:PlaceHolder ID="plhStatusHeaderInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="status-header-info" data-disable-hover="false"  title="<%=StatusHeaderInfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                     <%=StatusHeader%></span></th>
                             </asp:PlaceHolder>
                             <asp:PlaceHolder ID="plhSelectHeader" runat="server"> 
                                <th scope="col" class="ebiz-select">
                                    <asp:PlaceHolder ID="plhSelectHeaderInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="select-header-info" data-disable-hover="false"  title="<%=SelectHeaderInfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                     <%=SelectHeader%></span></th>
                             </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhProductHeader" runat="server"> 
                                <th scope="col" class="ebiz-product">
                                    <asp:PlaceHolder ID="plhProductHeaderInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="generic-col-2-info" data-disable-hover="false"  title="<%=ProductHeaderInfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                     <%=ProductHeader%></span></th>
                             </asp:PlaceHolder>
                             <asp:PlaceHolder ID="plhseatHeader" runat="server"> 
                                <th scope="col" class="ebiz-seat">
                                    <asp:PlaceHolder ID="plhSeatHeaderInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="seat-header-info" data-disable-hover="false"  title="<%=SeatHeaderInfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                     <%=SeatHeader%></span></th>
                             </asp:PlaceHolder>
                             <asp:PlaceHolder ID="plhPaymentOwnerHeader" runat="server"> 
                                <th scope="col" class="ebiz-owner">
                                    <asp:PlaceHolder ID="plhOwnerHeaderInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="owner-header-info" data-disable-hover="false"  title="<%=PaymentOwnerHeaderInfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                     <%=PaymentOwnerHeader%></span></th>
                             </asp:PlaceHolder>
                             <asp:PlaceHolder ID="plhPaymentRefHeader" runat="server">   
                                <th scope="col" class="ebiz-paymentRef">
                                    <asp:PlaceHolder ID="plhPaymentRefHeaderInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class=paymentRef-header-info" data-disable-hover="false"  title="<%=PaymentRefHeaderInfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                     <%=PaymentRefHeader%></span></th>
                             </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhSeatedCustomerHeader" runat="server"> 
                                <th scope="col" class="ebiz-Customer">
                                    <asp:PlaceHolder ID="plhCustomerHeaderInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="customer-header-info" data-disable-hover="false"  title="<%=SeatedCustomerHeaderInfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                     <%=SeatedCustomerHeader%></span></th>
                             </asp:PlaceHolder>
                             <asp:PlaceHolder ID="plhSalepriceHeader" runat="server"> 
                                <th scope="col" class="ebiz-SalepriceHeader">
                                    <asp:PlaceHolder ID="plhSalePriceHeaderInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="salePrice-header-info" data-disable-hover="false"  title="<%=SalepriceHeaderInfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                     <%=SalepriceHeader%></span></th>
                             </asp:PlaceHolder>
                             <asp:PlaceHolder ID="plhFaceValueHeader" runat="server"> 
                                <th scope="col" class="ebiz-fac-evalue">
                                    <asp:PlaceHolder ID="plhFaceValueHeaderInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="faceValue-header-info" data-disable-hover="false"  title="<%=FaceValueHeaderInfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                     <%=FaceValueHeader%></span></th>
                             </asp:PlaceHolder>
                             <asp:PlaceHolder ID="plhPriceHeader" runat="server"> 
                                <th scope="col" class="ebiz-price">
                                    <asp:PlaceHolder ID="plhPriceHeaderInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="price-header-info" data-disable-hover="false"  title="<%=PriceHeaderInfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                     <%=PriceHeader%></span></th>
                             </asp:PlaceHolder>
                             <asp:PlaceHolder ID="plhClubFeeHeader" runat="server">  
                                <th scope="col" class="ebiz-club-fee">
                                    <asp:PlaceHolder ID="plhClubFeeHeaderInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="clubfee-header-info" data-disable-hover="false"  title="<%=clubfeeHeaderInfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                     <%=ClubFeeHeader%></span></th>
                             </asp:PlaceHolder>
                             <asp:PlaceHolder ID="plhYouWillEarnHeader" runat="server">
                                <th scope="col" class="ebiz-you-will-earn">
                                    <asp:PlaceHolder ID="plhYouWillEarnHeaderInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="you-will-earn-header-info" data-disable-hover="false"  title="<%=youwillearnHeaderInfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                     <%=YouWillEarnHeader%></span></th>
                             </asp:PlaceHolder>
                             <asp:PlaceHolder ID="plhTicketExchangeIdHeader" runat="server">  
                                <th scope="col" class="ebiz-ticket-exchange-id">
                                    <asp:PlaceHolder ID="plhTicketExchangeIdHeaderInfo" runat="server"> 
                                         <span data-tooltip  aria-haspopup="true" class="TicketExchangeIdHeader-header-info" data-disable-hover="false"  title="<%=TicketExchangeIdHeaderInfo%>"><i class="fa fa-info-circle" aria-hidden="true"> </i>
                                    </asp:PlaceHolder>
                                     <%=TicketExchangeIdHeader%></span></th>
                             </asp:PlaceHolder>
                        </tr>
                    </thead>
                  <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                  <tr>
                    <td class="ebiz-status" data-title="<%=StatusHeader%>"> 
                        <asp:Label id="lblStatus" runat="server" CssClass="ebiz-status-label"  />
                        <asp:PlaceHolder ID="plhStatusInfo" runat="server"> 
                            <span data-tooltip  aria-haspopup="true" class="status-info" data-disable-hover="false"  title="<%# Eval("AdditionalInfo") %>"><i class="fa fa-info-circle" aria-hidden="true"></i></span>
                        </asp:PlaceHolder>
                        <asp:HiddenField ID="hdfCheckChangeFlag" runat="server" />
                    </td>
                    <asp:PlaceHolder ID="plhSelect" runat="server"> 
                      <td class="ebiz-checkbox" data-title="<%=SelectHeader%>">
                        <asp:CheckBox ID="chkStatus" runat="server" CssClass="ebiz-exchange-checkbox" />
                        <asp:CustomValidator ID="cvCheckbox" runat="server" ValidationGroup="TicketExchangeSelection" Display="Static" CssClass="error" ClientValidationFunction="validateCheckbox"></asp:CustomValidator>
                      </td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhProduct" runat="server"> 
                        <td class="ebiz-product" data-title="<%=ProductHeader%>">                   
                            <asp:Label id="lblProduct" runat="server"/>
                        </td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhSeat" runat="server"> 
                        <td class="ebiz-seat" data-title="<%=SeatHeader%>">                   
                            <asp:Label id="lblSeat" runat="server"/>
                        </td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhOwner" runat="server">
                        <td class="ebiz-owner" data-title="<%=PaymentOwnerHeader%>">          
                            <asp:Label ID="lblPaymentOwner" runat="server"/>
                        </td>
                    </asp:PlaceHolder> 
                    <asp:PlaceHolder ID="plhPaymentRef" runat="server"> 
                        <td class="ebiz-PaymentRef" data-title="<%=PaymentRefHeader%>">     
                            <asp:Label id="lblPaymentRef" runat="server"/>
                        </td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhCustomer" runat="server">
                        <td class="ebiz-customer" data-title="<%=SeatedCustomerHeader%>">  
                            <asp:Label ID="lblSeatedCustomer" runat="server"/>
                        </td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhSaleprice" runat="server"> 
                        <td class="ebiz-sale-price" data-title=" <%=SalepriceHeader%>">                       
                            <asp:Label id="lblSalePrice" runat="server"/>
                        </td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhfacevalue" runat="server">
                        <td class="ebiz-face-value" data-title="<%=FaceValueHeader%>">
                            <asp:Label ID="lblFaceValuePrice" runat="server"/>
                        </td>
                    </asp:PlaceHolder> 
                    <asp:PlaceHolder ID="plhPriceSelection" runat="server">
                      <td class="ebiz-price" data-title="<%=PriceHeader%>">    
                         <div class="row ebiz-price-slider-wrap">   
                            <div class="small-8 columns">
                               <div runat="server" id="divPriceSlider" data-slider>
                                  <span class="slider-handle" data-slider-handle role="slider" tabindex="1" runat="server" id="spanSliderHandle"></span>
                                  <span class="slider-fill" data-slider-fill></span>
                               </div>
                            </div>
                            <div class="small-8 columns">
                               <asp:TextBox ID="txtResaleSlider" type="number" runat="server" />
                               <asp:Label ID="lblResaleValue" runat="server" CssClass="ebiz-resale-label" />
                            </div>  
                         </div>
                      </td>           
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhClubFee" runat="server">
                        <td class="ebiz-club-fee" data-title=" <%=ClubFeeHeader%>">
                           <asp:Label id="lblClubFee" runat="server" />
                        </td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhYouWillEarn" runat="server">
                        <td class="ebiz-you-will-earn" data-title="<%=YouWillEarnHeader%>">
                           <asp:Label id="lblYouWillEarn" runat="server" />
                        </td>
                    </asp:PlaceHolder>                           
                    <asp:PlaceHolder ID="plhTicketExchangeId" runat="server">                          
                        <td class="ebiz-exchangeid" data-title="<%=TicketExchangeIdHeader%>">
                            <asp:Label id="lblTicketExchangeId" runat="server"  />
                        </td>
                    </asp:PlaceHolder>
                       <asp:HiddenField ID="hdfOriginalResaleValue" runat="server" /> 
                       <asp:HiddenField ID="hdfOriginalStatus" runat="server" />
                       <asp:HiddenField ID="hdfOriginalChecked" runat="server" />
                       <asp:HiddenField ID="hdfOriginalPrice" runat="server" />  
                       <asp:HiddenField ID="hdfFaceValue" runat="server" />  
                       <asp:HiddenField ID="hdfProductFeeValue" runat="server" />  
                       <asp:HiddenField ID="hdfProductFeeType" runat="server" />  
                  </tr>
                </ItemTemplate>
                <FooterTemplate>
                   </tbody></table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <asp:HiddenField runat="server" ID="hdfCurrencySymbol" ClientIDMode="Static" />       
        <asp:HiddenField runat="server" ID="hdfCurrentlyOffSaleText" ClientIDMode="Static" />      
        <asp:HiddenField runat="server" ID="hdfCurrentlyOnSaleText" ClientIDMode="Static" />      
        <asp:HiddenField runat="server" ID="hdfTakingOffSaleText" ClientIDMode="Static" />      
        <asp:HiddenField runat="server" ID="hdfPlacingOnSaleText" ClientIDMode="Static" />      
        <asp:HiddenField runat="server" ID="hdfPriceChangeText" ClientIDMode="Static" />      
        <asp:HiddenField runat="server" ID="hdfSoldText" ClientIDMode="Static" />          
 </asp:PlaceHolder>