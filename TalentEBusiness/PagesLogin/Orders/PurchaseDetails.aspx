<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PurchaseDetails.aspx.vb" Inherits="PagesLogin_Orders_PurchaseDetails" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/DirectDebitSummary.ascx" TagName="DirectDebitSummary" TagPrefix="Talent" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhErrorMessage" runat="server" Visible="false">
        <p class="alert-box alert">
            <asp:Literal ID="ltlError" runat="server" />
        </p>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhSuccessMessage" runat="server" Visible="false">
		<p class="alert-box success">
			<asp:Literal ID="ltlSuccessMessage" runat="server" />
		</p>
	</asp:PlaceHolder>

    <asp:PlaceHolder ID="plhPurchaseDetail" runat="server" Visible="true">

    <div class="panel ebiz-payment-owner-purchase-details">
        <div class="row">
            <div class="medium-6 columns ebiz-purchase-details">
                <h2><asp:Literal ID="ltlPaymentDetails" runat="server" /></h2>
                <asp:PlaceHolder ID="plhPackageID" runat="server" Visible="True">
                    <div class="row ebiz-package-id">
                        <div class="medium-6 columns"><asp:Literal ID="ltlPackageIdLabel" runat="server" /></div>
                        <div class="medium-6 columns"><asp:Literal ID="ltlPackageId" runat="server" /></div>
                    </div>
                    <asp:PlaceHolder ID="plhPackageStatus" runat="server" Visible="false">    
                        <div class="row ebiz-package-status">
                            <div class="medium-6 columns"><asp:Literal ID="ltlPackageStatusLabel" runat="server" /></div>
                            <div class="medium-6 columns"><asp:Literal ID="ltlPackageStatus" runat="server" /></div>
                        </div>
                    </asp:PlaceHolder>
                </asp:PlaceHolder>
                <div class="row ebiz-payment-reference">
                    <div class="medium-6 columns"><asp:Literal ID="ltlPaymentReferenceLabel" runat="server" /></div>
                    <div class="medium-6 columns"><%= PaymentReference.TrimStart("0")%></div>
                </div>
                <div class="row ebiz-transaction-dete">
                    <div class="medium-6 columns"><asp:Literal ID="ltlTransactionDeteLabel" runat="server" /></div>
                    <div class="medium-6 columns"><asp:Literal ID="ltlTransactionDate" runat="server" /></div>
                </div>
                <asp:PlaceHolder ID="plhPriceBand" runat="server" Visible="false">  
                    <div class="row ebiz-price-band">
                        <div class="medium-6 columns"><asp:Literal ID="ltlPriceBandLabel" runat="server" /></div>
                        <div class="medium-6 columns"><asp:Literal ID="ltlPriceBand" runat="server" /></div>
                    </div> 
                </asp:PlaceHolder>         
                <asp:PlaceHolder ID="plhPackageValueNet" runat="server" visible="false">
                    <div class="row ebiz-package-value-net">
                        <div class="medium-6 columns"><asp:Literal ID="ltlPackageValueNetLabel" runat="server" /></div>
                        <div class="medium-6 columns"><asp:Literal ID="ltlPackageValueNetValue" runat="server" /></div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhPackageValueVat" runat="server" visible="false">
                    <div class="row ebiz-package-value-vat">
                        <div class="medium-6 columns"><asp:Literal ID="ltlPackageValueVatLabel" runat="server" /></div>
                        <div class="medium-6 columns"><asp:Literal ID="ltlPackageValueVatValue" runat="server" /></div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhPackageValueGross" runat="server" visible="false">
                    <div class="row ebiz-package-value-gross">
                        <div class="medium-6 columns"><asp:Literal ID="ltlPackageValueGrossLabel" runat="server" /></div>
                        <div class="medium-6 columns"><asp:Literal ID="ltlPackageValueGrossValue" runat="server" /></div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhPackageValueDiscountComponent" runat="server" visible="false">
                    <div class="row ebiz-package-value-discount-component">
                        <div class="medium-6 columns"><asp:Literal ID="ltlPackageValueDiscountComponentLabel" runat="server" /></div>
                        <div class="medium-6 columns"><asp:Literal ID="ltlPackageValueDiscountComponentValue" runat="server" /></div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhPackageValueDiscountPackage" runat="server" visible="false">
                    <div class="row ebiz-package-value-discount-package">
                        <div class="medium-6 columns"><asp:Literal ID="ltlPackageValueDiscountPackageLabel" runat="server" /></div>
                        <div class="medium-6 columns"><asp:Literal ID="ltlPackageValueDiscountPackageValue" runat="server" /></div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhTicketTotal" runat="server" visible="false">
                    <div class="row ebiz-total-value">
                        <div class="medium-6 columns"><asp:Literal ID="ltlTicketsTotalLabel" runat="server" /></div>
                        <div class="medium-6 columns"><asp:Literal ID="ltlTicketsTotalValue" runat="server" /></div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhRetailTotal" runat="server" visible="false">
                    <div class="row ebiz-retail-total-value">
                        <div class="medium-6 columns"><asp:Literal ID="ltlRetailTotalLabel" runat="server" /></div>
                        <div class="medium-6 columns"><asp:Literal ID="ltlRetailTotalValue" runat="server" /></div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhAgent" runat="server">
                <div class="row ebiz-agent">
                    <div class="medium-6 columns"><asp:Literal ID="ltlAgentLabel" runat="server" /></div>
                    <div class="medium-6 columns"><asp:Literal ID="ltlAgentName" runat="server" /></div>
                </div>
                <div class="row ebiz-batch-reference">
                    <div class="medium-6 columns"><asp:Literal ID="ltlBatchReferenceLabel" runat="server" /></div>
                    <div class="medium-6 columns"><asp:Literal ID="ltlBatchReference" runat="server" /></div>
                </div>
                </asp:PlaceHolder>
                <asp:Repeater ID="rptFees" runat="server">
                    <ItemTemplate>
                    <div class="row ebiz-fee-price">
                        <div class="medium-6 columns"><%# Container.DataItem("ProductDescription").ToString().Trim()%></div>
                        <div class="medium-6 columns"><%# IsFeeCancelled(Container.DataItem("SalePrice"), Container.DataItem("StatusCode"))%></div>
                    </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Repeater ID="rptTrackingNumbers" runat="server" OnItemDataBound="rptTrackingNumbers_ItemDataBound">
                    <ItemTemplate>
                        <div class="row ebiz-tracking-number">
                            <div class="medium-6 columns"><asp:Literal ID="ltlOrderTrackingNumber" runat="server" /></div>
                            <div class="medium-6 columns">
                                <asp:HyperLink ID="hplOrderTrackingNumber" runat="server">
                                    <%# Container.DataItem("OrderTrackingNumber").ToString().Trim()%>
                                </asp:HyperLink>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <div class="medium-6 columns ebiz-payment-owner">
                <h2><asp:Literal ID="ltlPaymentOwner" runat="server" /></h2>
                <asp:PlaceHolder ID="plhName" runat="server">
                    <div class="row ebiz-name">
                        <div class="columns">
                            <asp:Literal ID="ltlName" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhAddressLine1" runat="server">
                    <div class="row ebiz-address-line-1">
                        <div class="columns">
                            <asp:Literal ID="ltlAddressLine1" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhAddressLine2" runat="server">
                    <div class="row ebiz-address-line-2">
                        <div class="columns">
                            <asp:Literal ID="ltlAddressLine2" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhAddressLine3" runat="server">
                    <div class="row ebiz-address-line-3">
                        <div class="columns">
                            <asp:Literal ID="ltlAddressLine3" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhAddressLine4" runat="server">
                    <div class="row ebiz-address-line-4">
                        <div class="columns">
                            <asp:Literal ID="ltlAddressLine4" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhPostCode" runat="server">
                    <div class="row ebiz-postcode">
                        <div class="columns">
                            <asp:Literal ID="ltlPostCode1" runat="server" />&nbsp;<asp:Literal ID="ltlPostCode2" runat="server" />
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>

        <div class="row ebiz-payment-details-wrap">
            <div class="columns">
                <h2><asp:Literal ID="ltlPaymentDetailsTitle" runat="server" /></h2>
                <asp:Repeater ID="rptMultiPaymentdetails" runat="server"> 
                    <HeaderTemplate>
                        <table class="ebiz-multiplepayments">
                            <thead>
                                <tr>
                                    <th scope="col" class="ebiz-agent-name"><%= PaymentTypeColumnHeading %></th>
                                    <th scope="col" class="ebiz-agent-name"><%= AmountPaidColumnHeading %></th>   
                                    <th scope="col" class="ebiz-agent-name"><%= DetailsColumnHeading %></th>                                                      
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                                <tr>
                                    <td><asp:Literal ID="ltlMultiplePayType" runat="server" /></td>
                                    <td><%# FormatCurrency(Container.DataItem("PayAmount").ToString().Trim())%></td>      
                                    <td>
                                        <asp:PlaceHolder ID="plhCC" runat="server">
                                            <ul class="inline-list">
                                                <li><%=EncryptedCardNumberPrefixString %><%# Container.DataItem("CardNumber").ToString().Trim()  %></li>
                                                <li><asp:Literal ID="ltlExpiryDate" runat="server" /></li>
                                                <li><%# Talent.eCommerce.Utilities.GetFormattedCardDateForDisplay(Container.DataItem("ExpiryDate").ToString().Trim())%></li>
                                                <li><asp:Literal ID="ltlStartDate" runat="server" /></li>
                                                <li><%# Talent.eCommerce.Utilities.GetFormattedCardDateForDisplay(Container.DataItem("StartDate").ToString().Trim())%></li>
                                                <li><asp:Literal ID="ltlIssueNumber" runat="server" /></li>
                                                <li><%# Container.DataItem("IssueDetail").ToString().Trim()%></li>
                                            </ul>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="plhDD" runat="server">
                                            <ul class="inline-list">
                                                <li><asp:Literal ID="ltlAccountName" runat="server" /></li>
                                                <li><%# Container.DataItem("AccountName").ToString().Trim()%></li>
                                                <li><asp:Literal ID="ltlAccountNumber" runat="server" /></li>
                                                <li><%# Container.DataItem("AccountNumber").ToString().Trim()%></li>
                                                <li><asp:Literal ID="ltlSortCode" runat="server" /></li>
                                                <li><%# Container.DataItem("SortCode").ToString().Trim()%></li>
                                            </ul>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="plhCF" runat="server">
                                            <ul class="inline-list">
                                                <li><asp:Literal ID="ltlCreditFinance" runat="server" /></li>
                                                <li><%# Container.DataItem("FinancePlanDescription").ToString().Trim()%></li>
                                                <li><asp:Literal ID="ltlCreditFinanceRequest" runat="server" /></li>
                                                <li><%# Container.DataItem("FinanceRequestID").ToString().Trim()%></li>
                                            </ul>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="plhEP" runat="server">
                                            <ul class="inline-list">
                                                <li><asp:Literal ID="ltlEPCardNumber" runat="server" /></li>
                                                <li><%# Container.DataItem("CardNumber").ToString().Trim()%></li>
                                            </ul>
                                        </asp:PlaceHolder>
                                    </td>
                                </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                </asp:Repeater> 

                <asp:Repeater ID="rptPaymentDetails" runat="server">
                    <ItemTemplate>
                        <asp:PlaceHolder ID="plhPayAmount" runat="server">
                            <div class="row ebiz-pay-amount">
                                <div class="medium-3 columns"><%= AmountPaidColumnHeading%></div>
                                <div class="medium-9 columns"><%# FormatCurrency(Container.DataItem("PayAmount").ToString().Trim())%></div>
                            </div>
                        </asp:PlaceHolder>
                        <div class="row ebiz-pay-type">
                            <div class="medium-3 columns"><%= PaymentTypeColumnHeading%></div>
                            <div class="medium-9 columns"><asp:Literal ID="ltlPayType" runat="server" />
                                <asp:HyperLink ID="hplDirectDebitSummary" runat="server">&nbsp;<i class="fa fa-table"></i></asp:HyperLink>
                            </div>
                        </div>
                        <asp:PlaceHolder ID="plhDetails" runat="server">
                            <asp:PlaceHolder ID="plhCreditCard" runat="server">
                            <div class="row ebiz-card-number">
                                <div class="medium-3 columns"><asp:Literal ID="ltlCreditCardNumberLabel" runat="server" /></div>
                                <div class="medium-9 columns"><%=EncryptedCardNumberPrefixString %><%# Container.DataItem("CardNumber").ToString().Trim()%></div>
                            </div>
                            <div class="row ebiz-expiry-date">
                                <div class="medium-3 columns"><asp:Literal ID="ltlExpiryDateLabel" runat="server" /></div>
                                <div class="medium-9 columns"><%# Talent.eCommerce.Utilities.GetFormattedCardDateForDisplay(Container.DataItem("ExpiryDate").ToString().Trim())%></div>
                            </div>
                            <div class="row ebiz-start-date">
                                <div class="medium-3 columns"><asp:Literal ID="ltlStartDateLabel" runat="server" /></div>
                                <div class="medium-9 columns"><%# Talent.eCommerce.Utilities.GetFormattedCardDateForDisplay(Container.DataItem("StartDate").ToString().Trim())%></div>
                            </div>
                            <div class="row ebiz-issue-number">
                                <div class="medium-3 columns"><asp:Literal ID="ltlIssueNumberLabel" runat="server" /></div>
                                <div class="medium-9 columns"><%# Container.DataItem("IssueDetail").ToString().Trim()%></div>
                            </div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhDirectDebit" runat="server">
                            <div class="row ebiz-account-name">
                                <div class="medium-3 columns"><asp:Literal ID="ltlAccountNameLabel" runat="server" /></div>
                                <div class="medium-9 columns"><%# Container.DataItem("AccountName").ToString().Trim()%></div>
                            </div>
                            <div class="row ebiz-account-number">
                                <div class="medium-3 columns"><asp:Literal ID="ltlAccountNumberLabel" runat="server" /></div>
                                <div class="medium-9 columns"><%# Container.DataItem("AccountNumber").ToString().Trim()%></div>
                            </div>
                            <div class="row ebiz-sort-code">
                                <div class="medium-3 columns"><asp:Literal ID="ltlSortCodeLabel" runat="server" /></div>
                                <div class="medium-9 columns"><%# Container.DataItem("SortCode").ToString().Trim()%></div>
                            </div>
                            <div id="ebiz-direct-debit-summary-reveal-<%# Container.ItemIndex %>" class="reveal large" data-reveal>
                                <Talent:DirectDebitSummary ID="uscDirectDebitSummary" runat="server" />
                                <button class="close-button" data-close aria-label="Close modal" type="button">
                                    <i class="fa fa-times" aria-hidden="true"></i>
                                 </button>
                            </div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhCreditFinance" runat="server">
                            <div class="row ebiz-plan-code">
                                <div class="medium-3 columns"><asp:Literal ID="ltlCreditFinancePlan" runat="server" /></div>
                                <div class="medium-9 columns"><%# Container.DataItem("FinancePlanDescription").ToString().Trim()%></div>
                            </div>
                            <div class="row ebiz-financeRequest-ID">
                                <div class="medium-3 columns"><asp:Literal ID="ltlCreditFinanceRequestID" runat="server" /></div>
                                <div class="medium-6 columns"><%# Container.DataItem("FinanceRequestID").ToString().Trim()%></div>
                            </div>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhEPurse" runat="server">
                                <div class="row ebiz-epurse">
                                    <div class="medium-3 columns"><asp:Literal ID="ltlEPurseCardNumber" runat="server" /></div>
                                    <div class="medium-6 columns"><%# Container.DataItem("CardNumber").ToString().Trim()%></div>
                                </div>
                            </asp:PlaceHolder>
                        </asp:PlaceHolder>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>

    <div class="ebiz-back-wrap">
        <asp:Button ID="btnBack" runat="server" CssClass="button ebiz-back" />
    </div>

    <asp:PlaceHolder ID="plhRetailOrdersRepeater" runat="server" visible="false">
        <div class="panel ebiz-retail-orders">
            <h2><asp:Literal ID="ltlRetailOrdersTitle" runat="server" /></h2>
            <asp:Repeater ID="rptRetailOrders" runat="server">
            <HeaderTemplate>
                <table class="ebiz-retail-orders">
                    <thead>
                        <tr>
                            <th scope="col" class="ebiz-retail-product"><%=RetailProductColumnHeading%></th>
                            <th scope="col" class="ebiz-retail-description"><%=RetailDescriptionColumnHeading%></th>
                            <th scope="col" class="ebiz-retail-quantity"><%=QuantityHeading%></th>
                            <th scope="col" class="ebiz-retail-price"><%=PriceColumnHeading%></th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                    <tr>
                        <td class="ebiz-retail-product" data-title="<%=RetailProductColumnHeading%>"><%# Container.DataItem("PRODUCT_CODE").ToString().Trim()%></td>
                        <td class="ebiz-retail-description" data-title="<%=RetailDescriptionColumnHeading%>"><%# Container.DataItem("PRODUCT_DESCRIPTION_1").ToString().Trim()%></td>
                        <td class="ebiz-retail-quantity" data-title="<%=QuantityHeading%>"><%# CType(Container.DataItem("QUANTITY"), Integer).ToString %></td>
                        <td class="ebiz-retail-price" data-title="<%=PriceColumnHeading%>"><%# GetPrice(Container.DataItem("PURCHASE_PRICE_GROSS").ToString().Trim(), False)%></td>
                    </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
            </asp:Repeater>
            <asp:PlaceHolder ID="plhRetailDiscount" runat="server" visible="false">
                <div class="panel ebiz-retail-Summary">
                    <div class="row ebiz-retail-discount-value">
                        <div class="medium-6 columns"><asp:Literal ID="ltlRetailGoodsValueLabel" runat="server" /></div>
                        <div class="medium-6 columns"><asp:Literal ID="ltlRetailGoodsValue" runat="server" /></div>
                    </div>
                    <div class="row ebiz-retail-discount-value">
                        <div class="medium-6 columns"><asp:Literal ID="ltlRetailDiscountLabel" runat="server" /></div>
                        <div class="medium-6 columns"><asp:Literal ID="ltlRetailDiscountValue" runat="server" /></div>
                    </div>
                    <div class="row ebiz-retail-total-value">
                        <div class="medium-6 columns"><asp:Literal ID="ltlRetailSummaryTotalLabel" runat="server" /></div>
                        <div class="medium-6 columns"><asp:Literal ID="ltlRetailSummaryTotalValue" runat="server" /></div>
                    </div>
                </div>
             </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhOrderDetailsRepeater" runat="server">
        <div class="panel ebiz-ticketing-orders">
            <h2><asp:Literal ID="ltlTicketingOrdersTitle" runat="server" /></h2>
            <asp:PlaceHolder ID="plhActions" runat="server">
                <div class="stacked-for-small button-group">                    
                    <asp:Button ID="btnPrintSelectedItem" CssClass="button ebiz-print-selected" runat="server" OnClientClick="if (validateButtonAction('print','.ebiz-item-select', '.ebiz-order-details') == false) return(false);" />
                    <asp:Button ID="btnCancelSelectedItem" runat="server" CssClass="button ebiz-cancel-selected" OnClientClick="if (validateButtonAction('cancel','.ebiz-item-select', '.ebiz-order-details') == false) return(false);" />                   
                    <asp:Button ID="btnTransferSelectedItem" runat="server" CssClass="button ebiz-transfer-selected" OnClientClick="if (validateButtonAction('transfer','.ebiz-item-select', '.ebiz-order-details') == false) return(false);" />                   
                    <asp:Button ID="btnAmendSelectedItem" runat="server" CssClass="button ebiz-amend-selected" OnClientClick="if (validateButtonAction('amend','.ebiz-item-select', '.ebiz-order-details')== false) return(false);" />                   
                    <asp:Button ID="btnSeatHistorySelectedItem" runat="server" CssClass="button ebiz-seat-hist-selected" OnClientClick="if (validateButtonAction('seatHist','.ebiz-item-select', '.ebiz-order-details') == false) return(false);" />                           
                    <asp:Button ID="btnSeatPrintHistorySelectedItem" runat="server" CssClass="button ebiz-seat-print-hist-selected" OnClientClick="if (validateButtonAction('seatPrintHist','.ebiz-item-select', '.ebiz-order-details') == false) return(false);" />                   
                    <asp:Button ID="btnResendEmail" runat="server" CssClass="button resend-email" OnClientClick="if (validateButtonAction('email','.ebiz-item-select', '.ebiz-order-details') == false) return(false);" />                   
                    <asp:Button ID="btnResendMessage" runat="server" CssClass="button resend-message" OnClientClick="if (validateButtonAction('number','.ebiz-item-select', '.ebiz-order-details') == false) return(false);" />                   
                </div>
            </asp:PlaceHolder>
            <asp:Repeater ID="rptOrderDetails" runat="server">
            <HeaderTemplate>
                <table class="ebiz-order-details">
                    <thead>
                        <tr>
                            <asp:PlaceHolder ID="plhchkSelectAll" runat="server">
                                <th scope="col" class="ebiz-select" >
                                    <asp:Checkbox ID="chkSelectAll"  runat="server" ClientIDMode="Static" OnClick="selectAll(this.checked, '.ebiz-item-select');" /> 
                                </th>
                            </asp:PlaceHolder>
                            <th scope="col" class="ebiz-product"><%=ProductColumnHeading%></th>
                            <asp:PlaceHolder ID="plhSeatsColumn" runat="server">
                                <th scope="col" class="ebiz-seat"><%=SeatColumnHeading%></th>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhQuantityColumn" runat="server">
                                <th scope="col" class="ebiz-quantity"><%=QuantityHeading%></th>
                            </asp:PlaceHolder>
                            <th scope="col" class="ebiz-date"><%=DateColumnHeading%></th>
                            <th scope="col" class="ebiz-customer"><%=CustomerColumnHeading%></th>
                            <th scope="col" class="ebiz-price"><%=PriceColumnHeading%></th>
                            <asp:PlaceHolder ID="plhPriceBand" runat="server">
                                <th scope="col" class="ebiz-price-band"><%=PriceBandColumnHeading%></th>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhFulfilmentMethodColumn" runat="server">
                                <th scope="col" class="ebiz-fulfilment-method"><%=FulfilmentMethodColumnHeading%></th>
                            </asp:PlaceHolder> 
                            <asp:PlaceHolder ID="plhLoyaltyPointsColumn" runat="server"><th scope="col" class="ebiz-loyalty-points"><%=LoyaltyPointsColumnHeading%></th></asp:PlaceHolder>
                                <th scope="col" class="ebiz-status"><%=StatusColumnHeading%></th>
                            <asp:PlaceHolder ID="plhDetailsOption1" runat="server">
                                <th scope="col" class="ebiz-details-1">&nbsp;</th>
                            </asp:PlaceHolder>                     
                            <asp:PlaceHolder ID="plhDespatchHeaders" runat="server">
                                <th scope="col" class="ebiz-ticket-number"><%=TicketNumberHeading%></th>
                                <th scope="col" class="ebiz-despatch-date"><%=DespatchDateHeading%></th>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhDetailsOption2" runat="server">
                                <th scope="col" class="ebiz-details-2">&nbsp;</th>
                            </asp:PlaceHolder>
                          </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                    <tr>
                        <asp:PlaceHolder ID="plhchkSelectedItem" runat="server">
                            <td  data-title="<%=ItemSelectHeading%>" class="ebiz-item-select">
                                <asp:CheckBox ID="chkSelectedItem" runat="server" onclick="validateSelChkBox(this.checked, '#chkSelectAll', '.ebiz-item-select', '.ebiz-order-details');"></asp:Checkbox>
                            </td>
                        </asp:PlaceHolder>
                        <td class="ebiz-product" data-title="<%=ProductColumnHeading %>"><%# Container.DataItem("ProductDescription").ToString().Trim()%></td>
                        <asp:PlaceHolder ID="plhSeatsColumn" runat="server">
                        <td class="ebiz-seat" data-title="<%=SeatColumnHeading%>"><%# FormatSeat(Container.DataItem("Seat"), Container.DataItem("Roving"), Container.DataItem("Unreserved"), Container.DataItem("ProductType"), Container.DataItem("AllocatedSeat"))%></asp:Literal></td>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhQuantityColumn" runat="server">
                        <td class="ebiz-quantity" data-title="<%=QuantityHeading%>">
                            <%# Container.DataItem("BulkQty").ToString()%>
                            <asp:HiddenField runat="server" ID="hdfBulkSalesID" Value='<%# Container.DataItem("BulkID").ToString().TrimStart(GlobalConstants.LEADING_ZEROS)%>' />
                        </td>
                        </asp:PlaceHolder>
                        <td class="ebiz-date" data-title="<%=DateColumnHeading%>"><%# getProductDate(Container.DataItem("ProductDate").ToString().Trim(), Container.DataItem("IsProductBundle").ToString().Trim(), Container.DataItem("BundleStartDate").ToString().Trim(), Container.DataItem("BundleEndDate").ToString().Trim(), Container.DataItem("HideDate"))%></td>
                        <td class="ebiz-customer" data-title="<%=CustomerColumnHeading%>"><%# Container.DataItem("CustomerNumber").ToString().TrimStart(GlobalConstants.LEADING_ZEROS)%> <%# Container.DataItem("CustomerName").ToString().Trim()%></td>
                        <td class="ebiz-price" data-title="<%=PriceColumnHeading%>"><asp:Literal ID="ltlPrice" runat="server" Text='<%# FormatCurrency(Container.DataItem("SalePrice"))%>'></asp:Literal>
                            <asp:HyperLink ID="hlpPromotion" runat="server" data-open="promotion-modal" CssClass="ebiz-open-modal" Visible='<%# ShowPromotion(Container.DataItem)%>' NavigateUrl='<%# GetNavigateUrl(Container.DataItem) %>' data-disable-hover="false" title="Offer" onclick="validatePrintIcon(this);">
                                <asp:Image ID="imgPromotion" runat="server" CssClass="promotion-image" style="display: none;" />
                                <span data-tooltip aria-haspopup="true" class="has-tip" data-disable-hover="false" title="Offer"><i class="fa fa-certificate"></i></span>
                            </asp:HyperLink>
                            <div id="promotion-modal" class="reveal ebiz-reveal-ajax" data-reveal></div>
                        </td>
                        <asp:PlaceHolder ID="plhPriceBand" runat="server">
                            <td class="ebiz-price-band" data-title="<%=PriceBandColumnHeading%>"><%# Container.DataItem("PriceBandDesc").ToString().Trim()%></td>
                         </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhFulfilmentMethodColumn" runat="server">
                            <td class="ebiz-fulfilment-method" data-title="<%=FulfilmentMethodColumnHeading%>"><%# GetFulfilmentTextFromCodes(Container.DataItem("FulfilmentMethod").ToString().Trim())%></td>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhLoyaltyPointsColumn" runat="server">
                        <td class="ebiz-loyalty-points" runat="server" id ="loyalty" data-title="<%=LoyaltyPointsColumnHeading%>"><%# Container.DataItem("LoyaltyPoints").ToString().Trim().ToString().TrimStart(GlobalConstants.LEADING_ZEROS)%></td>
                        </asp:PlaceHolder>
                        <td class="ebiz-status" data-title="<%=StatusColumnHeading%>"><%# GetStatusText(Container.DataItem("StatusCode").ToString().Trim()) %></td>
                        <asp:PlaceHolder ID="plhDetailsOption" runat="server">
                        <td class="ebiz-details-1"><a href="#" data-options="align: left" data-dropdown="<%# GetBundleKey(Container.DataItem("ProductCode"), Container.DataItem("Seat").ToString().Trim())%>"><i class="fa fa-info-circle"></i></a></td>
                        </asp:PlaceHolder>
                        
                        <asp:PlaceHolder ID="plhDespatchHeaders" runat="server">
                            <td class="ebiz-ticket-number" data-title="<%=TicketNumberHeading%>"><%# FormatTicketNumber(Container.DataItem("TicketNumber").ToString().Trim())%></td>
                            <td class="ebiz-despatch-date" data-title="<%=DespatchDateHeading%>"><%# FormatDespatchDate(Container.DataItem("DespatchDate").ToString().Trim(), Container.DataItem("TicketNumber").ToString().Trim())%></td>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhPackageDetailLink" runat="server" Visible="false">
                            <td class="ebiz-details-2">
                                <asp:HyperLink ID="hplDetails" runat="server"><i class="fa fa-info-circle"></i></asp:HyperLink>
                            </td>
                        </asp:PlaceHolder>
                      
                        <asp:HiddenField ID="hdfPaymentReference" runat="server" value='<%# Container.DataItem("PaymentReference").ToString().Trim()%>' />
                        <asp:HiddenField ID="hdfSeat" runat="server" value='<%# Container.DataItem("Seat").ToString().Trim()%>'  />
                        <asp:HiddenField ID="hdfProductCode" runat="server" value='<%# Container.DataItem("ProductCode").ToString().Trim()%>' />
                        <asp:HiddenField ID="hdfProductDate" runat="server" value='<%# Container.DataItem("ProductDate").ToString().Trim()%>' />
                        <asp:HiddenField ID="hdfCustomerNumber" runat="server" value='<%# Container.DataItem("CustomerNumber").ToString().Trim()%>' />
                        <asp:HiddenField ID="hdfProductType" runat="server" value='<%# Container.DataItem("ProductType").ToString().Trim()%>' />
                        <asp:HiddenField ID="hdfStadiumCode" runat="server" value='<%# Container.DataItem("StadiumCode").ToString().Trim()%>' />
                        <asp:HiddenField ID="hdfBulkID" runat="server" value='<%# Container.DataItem("BulkID").ToString().Trim()%>' />
                        <asp:HiddenField ID="hdfCallID" runat="server" value='<%# Container.DataItem("CallID").ToString().Trim()%>' />
                        <asp:HiddenField ID="hdfRRN" runat="server" value='<%# Container.DataItem("RRN").ToString().Trim()%>' />

                        <asp:HiddenField runat="server" ID="hdfPrintAllowed" Value='false' />
                        <asp:HiddenField runat="server" ID="hdfPrintAllowedError" />
                        <asp:HiddenField runat="server" ID="hdfCancelAllowed" Value='false' />
                        <asp:HiddenField runat="server" ID="hdfCancelAllowedError" />
                        <asp:HiddenField runat="server" ID="hdfCancelURL" />
                        <asp:HiddenField runat="server" ID="hdfTransferAllowed" Value='false' />
                        <asp:HiddenField runat="server" ID="hdfTransferAllowedError" />
                        <asp:HiddenField runat="server" ID="hdfTransferURL" />
                        <asp:HiddenField runat="server" ID="hdfAmendAllowed" Value='false' />
                        <asp:HiddenField runat="server" ID="hdfAmendAllowedError" />
                        <asp:HiddenField runat="server" ID="hdfAmendURL" />
                        <asp:HiddenField runat="server" ID="hdfSeatHistoryAllowed" Value='false' />
                        <asp:HiddenField runat="server" ID="hdfSeatHistoryAllowedError" />
                        <asp:HiddenField runat="server" ID="hdfSeatHistoryURL"/>
                        <asp:HiddenField runat="server" ID="hdfSeatPrintHistoryAllowed" Value='false' />
                        <asp:HiddenField runat="server" ID="hdfSeatPrintHistoryAllowedError" />
                        <asp:HiddenField runat="server" ID="hdfSeatPrintHistoryURL" />
                    </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
            </FooterTemplate>
            </asp:Repeater>
            <div class="reveal ebiz-reveal-ajax large" id="seat-history-modal" data-reveal ></div>  
            <div class="reveal ebiz-reveal-ajax large" id="seat-print-history-modal" data-reveal ></div>   
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhPackageDetailRepeater" runat="server">
        <div class="panel ebiz-package-detail">
            <h2><asp:Literal ID="ltlPackageBreakdown" runat="server" /></h2>
            <div class="stacked-for-small button-group">
                <asp:HyperLink ID="hlnkTransfer" runat="server" CssClass="button ebiz-transfer" />
                <asp:HyperLink ID="hlnkAmend" runat="server" CssClass="button ebiz-amend" />
                <asp:HyperLink ID="hlnkCancel" runat="server" CssClass="button ebiz-cancel" />
            </div>
            <asp:PlaceHolder ID="plhPackageDetail" runat="server">
                <div class="row ebiz-package-description-row">
                    <div class="large-12 columns ebiz-package-description-label"><asp:Literal ID="ltlPackageDescription" runat="server" /></div>
                    <div class="large-12 columns ebiz-product-description-label"><asp:Literal ID="ltlProductDescription" runat="server" /></div>
                </div>
            </asp:PlaceHolder>
            <asp:Repeater ID="rptPackageDetails" runat="server">
                <HeaderTemplate>
                    <table class="stack">
                        <thead>                           
                            <tr>
                                <th scope="col" class="ebiz-quantity"><%=QuantityHeading%></th>
                                <th scope="col" class="ebiz-description"><%=ComponentHeading%></th>
                                <asp:PlaceHolder ID="plhComponentNetPriceHdr" runat="server">
                                    <th scope="col" class="ebiz-unit-price"><%=ComponentNetPriceHeading%></th>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhComponentDiscountHdr" runat="server">
                                    <th scope="col" class="ebiz-component-discount"><%=ComponentDiscountHeading%></th>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhComponentTotalPriceHdr" runat="server">
                                    <th scope="col" class="ebiz-price"><%=PriceHeading%></th>
                                </asp:PlaceHolder>
                                <th scope="col" class="ebiz-detail"><%=DetailHeading%></th>
                            </tr>                           
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                     <asp:PlaceHolder ID="plhComponentDetails" runat="server">
                            <tr>
                                <td class="ebiz-quantity" data-title="<%=QuantityHeading %>"><%# Container.DataItem("Quantity").ToString().Trim()%></td>
                                <td class="ebiz-description" data-title="<%=ComponentHeading %>"><%# Container.DataItem("ComponentDescription").ToString().Trim()%></td>
                                <asp:PlaceHolder ID="plhComponentNetPrice" runat="server">
                                    <td class="ebiz-unit-price" data-title="<%=ComponentNetPriceHeading%>"><%# GetPrice(Container.DataItem("ComponentUnitPrice") * Container.DataItem("Quantity"), False)%></td>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhComponentDiscount" runat="server">
                                    <td class="ebiz-component-discount" data-title="<%=ComponentDiscountHeading%>"><%# Container.DataItem("ComponentDiscountPercentage")%></td>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhComponentTotalPrice" runat="server">
                                    <td class="ebiz-price" data-title="<%=PriceHeading%>"><%# GetPrice(Container.DataItem("TotalPrice"), False)%></td>
                                </asp:PlaceHolder>
                                <td class="ebiz-detail" data-title="<%=DetailHeading%>"><%# Container.DataItem("Details").ToString().Trim()%>
                                <asp:PlaceHolder ID="plhComponentBulkSeatSummary" runat="server">
                                    <asp:HyperLink ID="hlkSeatDetails" runat="server" data-open="seat-details-modal" CssClass="ebiz-open-modal fa fa-info-circle" NavigateUrl='<%# GetComponentBulkSeatURL(Container.DataItem("ComponentID").ToString().Trim())%>'></asp:HyperLink>
                                    <div class="large reveal ebiz-reveal-ajax" id="seat-details-modal" data-reveal=""></div> 
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhComponentSummary" runat="server">
                                    <asp:Repeater ID="rptComponentSummary" runat="server" OnItemDataBound="rptComponentSummary_ItemDataBound">
                                        <ItemTemplate>
                                                <asp:PlaceHolder ID="plhComponentStandDesc" runat="server">
                                                    <p><asp:Literal ID="ltlComponentStandDesc" runat="server" /></p>
                                                </asp:PlaceHolder>
                                                <p><asp:Literal ID="ltlComponentDetail" runat="server" /></p>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhComponentSeatSummary" runat="server">
                                    <a href="#" data-open="Comp_Seat_<%# Container.DataItem("ComponentID").ToString().Trim()%>"><i class="fa fa-info-circle"></i></a>
                                    <div class="large reveal" id="Comp_Seat_<%# Container.DataItem("ComponentID").ToString().Trim()%>" data-reveal>
                                    <asp:Repeater ID="rptComponentSeatSummary" runat="server">
                                        <HeaderTemplate>
                                            <table class="stack">
                                                <thead>
                                                    <tr>
                                                        <th scope="col" class="ebiz-product"><%=ProductColumnHeading%></th>
                                                        <th scope="col" class="ebiz-stand"><%= StandColumnHeading%></th>
                                                        <asp:PlaceHolder ID="plhSeatDetailsHeading" runat="server" visible='<%# Not HideSeatForPWS %>'>
                                                            <th scope="col" class="ebiz-seat"><%=SeatColumnHeading%></th>
                                                        </asp:PlaceHolder>
                                                        <th scope="col" class="ebiz-customer"><%=CustomerColumnHeading%></th>
                                                        <th scope="col" class="ebiz-price-band"><%=PriceBandColumnHeading%></th>
                                                        <asp:PlaceHolder ID="plhComponentSeatPriceHeading" runat="server" visible='<%# ShowComponentSeatPrices(Container.DataItem)%>'>
                                                            <th scope="col" class="ebiz-price-band"><%= PriceColumnHeading%></th>    
                                                        </asp:PlaceHolder>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                    </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td class="ebiz-product" data-title="<%=ProductColumnHeading %>"><%# Container.DataItem("ProductDescription").ToString().Trim()%></td>
                                                <td class="ebiz-stand" data-title="<%=StandColumnHeading%>"><%# Container.DataItem("StandDesc").ToString().Trim()%></td>
                                                <asp:PlaceHolder ID="plhSeatDetails" runat="server" visible='<%# Not HideSeatForPWS %>'>
                                                    <td class="ebiz-seat" data-title="<%=SeatColumnHeading%>"><%# FormatSeat(Container.DataItem("Seat"), Container.DataItem("Roving"), Container.DataItem("Unreserved"), Container.DataItem("ProductType"), String.Empty)%></td>
                                                </asp:PlaceHolder>
                                                <td class="ebiz-customer" data-title="<%=CustomerColumnHeading%>"><%# Container.DataItem("CustomerNumber").ToString().Trim().TrimStart(GlobalConstants.LEADING_ZEROS)%> <%# Container.DataItem("CustomerName").ToString().Trim()%></td>
                                                <td class="ebiz-price-band" data-title="<%=PriceBandColumnHeading%>"><%# Container.DataItem("PriceBandDesc").ToString().Trim()%></td>
                                                <asp:PlaceHolder ID="plhComponentSeatPrice" runat="server" visible='<%# ShowComponentSeatPrices(Container.DataItem)%>'>
                                                    <td class="ebiz-price-band" data-title="<%=PriceColumnHeading%>"><%# FormatCurrency(Container.DataItem("SalePrice"))%></td>
                                                </asp:PlaceHolder>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            </tbody>
                                            </table>
                                        </FooterTemplate>
                                    </asp:Repeater>
                                        <button class="close-button" data-close aria-label="Close modal" type="button">
                                            <i class="fa fa-times" aria-hidden="true"></i>
                                        </button>
                                    </div>
                                </asp:PlaceHolder>
                                </td>
                            </tr>
                          </asp:PlaceHolder>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhPackageHistoryRepeater" runat="server">
        <div class="panel ebiz-pakage-history">
            <h2><asp:Literal ID="ltlPackageHistory" runat="server" /></h2>
            <asp:Repeater ID="rptPackageHistory" runat="server">
                    <HeaderTemplate>
                    <table class="stack">
                        <thead>
                            <tr>
                                <th scope="col" class="ebiz-package-date"><%= PackageHistoryDateColHead%></th>
                                <th scope="col" class="ebiz-package-history"><%= PackageHistoryDescColHead%></th>
                                <th scope="col" class="ebiz-package-detail"></th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                    <ItemTemplate>
                    <tr>
                        <td class="ebiz-package-date" data-title="<%=PackageHistoryDateColHead %>"><%# CType(Container.DataItem("AmendmentDate"), Date).ToString("D")%></td>
                        <td class="ebiz-package-history" data-title="<%=PackageHistoryDescColHead %>"><asp:literal ID="ltlComment" runat="server"></asp:literal></td>
                        <asp:PlaceHolder ID="plhPackageDetailsButton" runat="server">
                        <td class="ebiz-package-detail">
                            <asp:Button ID="btnDetailPackageHistory" runat="server" Text='<%# DetailButtonText %>'  CssClass="button" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "PackageAmendmentId").ToString().Trim() %>' CommandName="PackageHistoryDetail" />
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
    </asp:PlaceHolder>

    <asp:Repeater ID="rptBundles" runat="server">
        <ItemTemplate>
        <div id="<%# GetCurrentBundleKey(Container.DataItem) %>" data-dropdown-content class="f-dropdown content large">
        <asp:Repeater ID="rptProductsInABundle" runat="server">
            <HeaderTemplate>
                <table class="ebiz-bundle">
                    <thead>
                        <tr>
                            <th scope="col" class="ebiz-product"><%=ProductColumnHeading%></th>
                            <th scope="col" class="ebiz-seat"><%=SeatColumnHeading%></th>
                            <th scope="col" class="ebiz-date"><%=DateColumnHeading%></th>
                            <th scope="col" class="ebiz-customer"><%=CustomerColumnHeading%></th>
                            <th scope="col" class="ebiz-price"><%=PriceColumnHeading%></th>
                            <th scope="col" class="ebiz-price-band"><%=PriceBandColumnHeading%></th>
                            <th scope="col" class="ebiz-status"><%=StatusColumnHeading%></th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                        <tr class="ebiz-sale-type-T">
                            <td class="ebiz-product" data-title="<%=ProductColumnHeading %>"><%# Container.DataItem("ProductDescription").ToString().Trim()%></td>
                            <td class="ebiz-seat" data-title="<%=SeatColumnHeading%>"><%# Container.DataItem("Seat").ToString().Trim()%></td>
                            <td class="ebiz-date" data-title="<%=DateColumnHeading%>"><%# Container.DataItem("ProductDate").ToString().Trim()%></td>
                            <td class="ebiz-customer" data-title="<%=CustomerColumnHeading%>"><%# Container.DataItem("CustomerNumber").ToString().Trim().TrimStart(GlobalConstants.LEADING_ZEROS)%> <%# Container.DataItem("CustomerName").ToString().Trim()%></td>
                            <td class="ebiz-price" data-title="<%=PriceColumnHeading%>"><%# FormatCurrency(Container.DataItem("SalePrice"))%>
                                <asp:HyperLink ID="hplPromotion" runat="server" CssClass="promotion-link" Visible='<%# ShowPromotion(Container.DataItem)%>' NavigateUrl='<%# GetNavigateUrl(Container.DataItem) %>'>
                                    <asp:Image ID="imgPromotion" runat="server" CssClass="ebiz-promotion-image" />
                                </asp:HyperLink>
                            </td>
                            <td class="ebiz-price-band" data-title="<%=PriceBandColumnHeading%>"><%# Container.DataItem("PriceBandDesc").ToString().Trim()%></td>
                            <td class="ebiz-status" data-title="<%=StatusColumnHeading%>"><%# GetStatusText(Container.DataItem("StatusCode").ToString().Trim()) %></td>
                        </tr>
            </ItemTemplate>
            <FooterTemplate>
                    </tbody>
                </table>
                </div>
            </FooterTemplate>
        </asp:Repeater>
        </ItemTemplate>
    </asp:Repeater>

    <asp:HiddenField ID="hdfIsBulk" runat="server" ClientIDMode="Static" />
    <asp:HiddenField runat="server" ID="hdfSingleSelectOnlyErr" ClientIDMode="Static" />
    <asp:HiddenField runat="server" ID="hdfSingleSelectOnlyBulkHdrErr" ClientIDMode="Static" />     
    <asp:HiddenField runat="server" ID="hdfsingleSelectOnlyCorpHdrErr" ClientIDMode="Static" />      
    <asp:HiddenField runat="server" ID="hdfSelectionMandatoryErr" ClientIDMode="Static" />
    <asp:HiddenField runat="server" ID="hdfFullCancOnly" ClientIDMode="Static" />
    <asp:HiddenField runat="server" ID="hdfFullCancOnlyErr" ClientIDMode="Static" />  
    <asp:HiddenField runat="server" ID="hdfMaxSelLimitErr" ClientIDMode="Static" />  
    <asp:HiddenField runat="server" ID="hdfCustomerIsOnStop" ClientIDMode="Static" />  
    <asp:HiddenField runat="server" ID="hdfStopCodeDiagTitle" ClientIDMode="Static" />  
    <asp:HiddenField runat="server" ID="hdfStopCodeDiagMessage" ClientIDMode="Static" />                
    <asp:HiddenField runat="server" ID="hdfStopCodeOkBtnText" ClientIDMode="Static" />  
    <asp:HiddenField runat="server" ID="hdfStopCodeCancBtnText" ClientIDMode="Static" />  
    </asp:PlaceHolder>
</asp:content>

