<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PurchaseHistory.aspx.vb" Inherits="PagesLogin_Orders_PurchaseHistory" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder3" runat="Server">
	<script>
		function EnterKeyEvent(e) {
			if (e.keyCode == 13) {
				__doPostBack('<%=btnSearch.UniqueID%>', "");
			}
		}

        //Determin whether or not to show the ticketing options
	    function ticketOptions() {
	        if (document.getElementById('chkTicketingProducts').checked) {
	            $('.ebiz-filter-status').show();
	            $('.ebiz-product-type').show();
	            $('.ebiz-package-id').show();
	            $('.ebiz-booked-seats').show();
	            $('.ebiz-show-reservations').show();
	            $('.ebiz-loyalty-information').show();
	            $('.ebiz-buyback-products').show();
                $('.ebiz-corporate-products').show();
                $('.ebiz-prod-desc').show();
	        } else {
	            $('.ebiz-filter-status').hide();
	            $('.ebiz-product-type').hide();
	            $('.ebiz-package-id').hide();
	            $('.ebiz-booked-seats').hide();
	            $('.ebiz-show-reservations').hide();
	            $('.ebiz-loyalty-information').hide();
	            $('.ebiz-buyback-products').hide();
                $('.ebiz-corporate-products').hide();
                $('.ebiz-prod-desc').hide();
	        }
	    }
	</script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
	<Talent:HTMLInclude ID="uscHTMLInclude" runat="server" Usage="2" Sequence="1" />
	<input type="hidden" id="hdnCurrentPage" runat="server" value="0" />
	<input type="hidden" id="hdnLastRRN" runat="server" value="0" />
	<input type="hidden" id="hdnRRNOfFirstRecordOnPage" runat="server" value="0" />

	<div class="row ebiz-purchase-history-wrap">
		<div class="large-3 columns ebiz-purchase-history-filters-wrap">
			<div class="panel">
				
				<asp:PlaceHolder ID="plhRetailProducts" runat="server">
				<div class="row ebiz-retail-products">
					<div class="large-12 columns">
						<asp:CheckBox ID="chkRetailProducts" runat="server" Checked="true" ClientIDMode="Static" CssClass="ebiz-checkbox-label-wrap" />
						<asp:Label ID="lblRetailProducts" runat="server" AssociatedControlID="chkRetailProducts" />
					</div>
				</div>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="plhTicketingProducts" runat="server">
				<div class="row ebiz-ticketing-products">
					<div class="large-12 columns">
						<asp:CheckBox ID="chkTicketingProducts" runat="server" Checked="true" ClientIDMode="Static" CssClass="ebiz-checkbox-label-wrap" />
						<asp:Label ID="lblTicketingProducts" runat="server" AssociatedControlID="chkTicketingProducts" />
					</div>
				</div>
                </asp:PlaceHolder>
                
				<div class="row ebiz-from-date">
					<div class="large-12 medium-4 columns">
						<asp:Label ID="lblFromDate" runat="server" associatedcontrolid="txtFromDate" />
					</div>
					<div class="large-12 medium-8 columns">
						<asp:TextBox ID="txtFromDate" runat="server" CssClass="datepicker" onkeypress="return EnterKeyEvent(event)" validationgroup="PurchaseHistory" />
						<asp:RangeValidator runat="server" ID="rgvFromDate" controltovalidate="txtFromDate" type="Date" cssclass="error" validationgroup="PurchaseHistory" />
					</div>
				</div>
				<div class="row ebiz-to-date">
					<div class="large-12 medium-4 columns">
						<asp:Label ID="lblToDate" runat="server" associatedcontrolid="txtToDate" />
					</div>
					<div class="large-12 medium-8 columns">
						<asp:TextBox ID="txtToDate" runat="server" cssclass="datepicker" onkeypress="return EnterKeyEvent(event)" validationgroup="PurchaseHistory" />
						<asp:RangeValidator runat="server" ID="rgvToDate" controltovalidate="txtToDate" type="Date" cssclass="error" validationgroup="PurchaseHistory" />
						<asp:CompareValidator runat="server" ID="cmpToDate" controltovalidate="txtFromDate" controltocompare="txtToDate" cssclass="error" operator="LessThanEqual" type="Date" validationgroup="PurchaseHistory" />
					</div>
				</div>
                <asp:PlaceHolder ID="plhStatus" runat="server">
				<div class="row ebiz-filter-status">
					<div class="large-12 medium-4 columns">
						<asp:Label ID="lblStatus" runat="server" associatedcontrolid="ddlStatus" />
					</div>
					<div class="large-12 medium-8 columns">
						<asp:DropDownList ID="ddlStatus" runat="server" />
					</div>
				</div>
                </asp:PlaceHolder>
				<asp:PlaceHolder ID="plhProductType" runat="server">
				<div class="row ebiz-product-type">
					<div class="large-12 medium-4 columns">
						<asp:Label ID="lblProductType" runat="server" AssociatedControlID="ddlProductType" />
					</div>
					<div class="large-12 medium-8 columns">
						<asp:DropDownList ID="ddlProductType" runat="server" />
					</div>
				</div>
				</asp:PlaceHolder>
				<div class="row ebiz-pay-ref">
					<div class="large-12 medium-4 columns">
						<asp:Label ID="lblPaymentReference" runat="server" AssociatedControlID="txtPaymentReference" />
					</div>
					<div class="large-12 medium-8 columns">
						<asp:TextBox ID="txtPaymentReference" runat="server" onkeypress="return EnterKeyEvent(event)" />
						<asp:RegularExpressionValidator ID="revPaymentReference" runat="server" ControlToValidate="txtPaymentReference" ValidationGroup="PurchaseHistory" Display="Static" CssClass="error" ></asp:RegularExpressionValidator>
					</div>
				</div>
				<asp:PlaceHolder ID="plhProductOrDescription" runat="server">
				<div class="row ebiz-prod-desc">
					<div class="large-12 medium-4 columns">
						<asp:Label ID="lblProductOrDescription" runat="server" AssociatedControlID="txtProductOrDescription" />
					</div>
					<div class="large-12 medium-8 columns">
						<asp:TextBox ID="txtProductOrDescription" runat="server" onkeypress="return EnterKeyEvent(event)" />
					</div>
				</div>
                </asp:PlaceHolder>
				<asp:PlaceHolder ID="plhPackageID" runat="server">
				<div class="row ebiz-package-id">
					<div class="large-12 medium-4 columns">
						<asp:Label ID="lblPackageID" runat="server" AssociatedControlID="txtPackageID" />
					</div>
					<div class="large-12 medium-8 columns">
						<asp:TextBox ID="txtPackageID" runat="server" onkeypress="return EnterKeyEvent(event)" />
					</div>
				</div>
				</asp:PlaceHolder>
                <asp:PlaceHolder ID="plhCorporateProducts" runat="server">
				<div class="row ebiz-corporate-products">
					<div class="large-12 columns">
						<asp:CheckBox ID="chkCorporateProducts" runat="server" />
						<asp:Label ID="lblCorporateProducts" runat="server" AssociatedControlID="chkCorporateProducts" />
					</div>
				</div>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="plhBuybackProducts" runat="server">
				<div class="row ebiz-buyback-products">
					<div class="large-12 columns">
						<asp:CheckBox ID="chkShowBuybackProducts" runat="server" />
						<asp:Label ID="lblShowBuybackProducts" runat="server" AssociatedControlID="chkShowBuybackProducts" />
					</div>
				</div>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="plhLoyaltyInformation" runat="server">
				<div class="row ebiz-loyalty-information">
					<div class="large-12 columns">
						<asp:CheckBox ID="chkShowLoyaltyInformation" runat="server" />
						<asp:Label ID="lblShowLoyaltyInformation" runat="server" AssociatedControlID="chkShowLoyaltyInformation" />
					</div>
				</div>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="plhShowReservations" runat="server">
				<div class="row ebiz-show-reservations">
					<div class="large-12 columns">
						<asp:CheckBox ID="chkShowReservations" runat="server" />
						<asp:Label ID="lblReservations" runat="server" AssociatedControlID="chkShowReservations" />
					</div>
				</div>
				</asp:PlaceHolder>
				<asp:PlaceHolder ID="plhShowBooked" runat="server">
				<div class="row ebiz-booked-seats">
					<div class="large-12 columns">
						<asp:CheckBox ID="chkShowBooked" runat="server" />
						<asp:Label ID="lblBooked" runat="server" AssociatedControlID="chkShowBooked" />
					</div>
				</div>
				</asp:PlaceHolder>
				<div class="stacked-for-small button-group">
					<asp:Button ID="btnClear" runat="server" cssclass="button ebiz-muted-action ebiz-clear" />
					<asp:Button ID="btnSearch" runat="server" cssclass="button ebiz-primary-action ebiz-search" validationgroup="PurchaseHistory" />
				</div>
			</div>
		</div>

		<div class="large-9 columns ebiz-purchase-history-results-wrap">
			<div class="panel">
				<asp:placeholder id="plhPagerTop" runat="server">
					<div class="ebiz-pagination">
						<ul class="pagination">
							<asp:PlaceHolder ID="plhFirstPageTop" runat="server"><li><asp:LinkButton ID="lnkFirstTop" runat="server" CommandArgument="F" OnCommand="PagerButtonClick"></asp:LinkButton></li></asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPrevPageTop" runat="server"><li><asp:LinkButton ID="lnkPreviousTop" runat="server" CommandArgument="P" OnCommand="PagerButtonClick"></asp:LinkButton></li></asp:PlaceHolder>
							<li class="ebiz-current-page"><asp:Literal ID="ltlCurrentPageTop" runat="server" /></li>
							<asp:PlaceHolder ID="plhNextPageTop" runat="server"><li><asp:LinkButton ID="lnkNextTop" runat="server" CommandArgument="N" OnCommand="PagerButtonClick"></asp:LinkButton></li></asp:PlaceHolder>
							<asp:PlaceHolder ID="plhLastPageTop" runat="server"><li><asp:LinkButton ID="lnkLastTop" runat="server" CommandArgument="L" OnCommand="PagerButtonClick"></asp:LinkButton></li></asp:PlaceHolder>                                                
						</ul>
					</div>
				</asp:placeholder>

				<asp:repeater id="rptOrderHeaderHistory" runat="server">
					<HeaderTemplate>

							<table>
								<thead>
									<tr>
										<th scope="col" class="ebiz-product" id="productcolumn" runat="server"><%= ProductColumnHeading%></th>
										<th scope="col" class="ebiz-payment-ref"><%= PaymentRefColumnHeading%></th>
										<th scope="col" class="ebiz-quantity" id="quantitycolumn" runat="server"><%=QuantityColumnHeading %></th>
										<th scope="col" class="ebiz-date"><%=DateColumnHeading%></th>
										<th scope="col" class="ebiz-customer"><%=CustomerColumnHeading%></th>
                                        <asp:PlaceHolder ID="plhRetailPurchaseRef" runat="server">
                                        <th scope="col" class="ebiz-retail-purchase-ref"><%=RetailPurchaseRefColumnHeading%></th>
                                        </asp:PlaceHolder>
                                        <asp:PlaceHolder ID="plhPriceColumnHeader" runat="server" Visible="<%#ShowPrices%>">
										    <th scope="col" class="ebiz-price"><%=PriceColumnHeading%></th>
                                        </asp:PlaceHolder>
										<th scope="col" class="ebiz-status"><%=StatusColumnHeading%></th>
										<th scope="col" class="ebiz-details">&nbsp;</th>
									</tr>
								</thead>
								<tbody>
						</HeaderTemplate>
						<ItemTemplate>
								<tr class="ebiz-sale-type-<%# Container.DataItem("IS_TICKETING_OR_RETAIL") %>">
									<td class="ebiz-product" data-title="<%# ProductColumnHeading%>" id="productcolumn" runat="server">
										<asp:Literal ID="ltlRetailProduct" runat="server" />
										<%# Container.DataItem("TICKETING_PRODUCT")%>
									</td>
									<td class="ebiz-payment-ref" data-title="<%= PaymentRefColumnHeading%>">
									<asp:Literal ID="ltlPaymentReference" runat="server" />
										<asp:PlaceHolder ID="plhWebOrderNumber" runat="server">
											<div class="web-order-number">
												<asp:Literal ID="ltlWebOrderNumberLabel" runat="server" /><br />
												<%# Container.DataItem("RETAIL_WEB_ORDER_NUMBER")%>
											</div>
										</asp:PlaceHolder>
										<asp:PlaceHolder ID="plhBackOfficerOrderNumber" runat="server">
											<div class="back-office-order-number">
												<asp:Literal ID="ltlBackOfficerOrderNumberLabel" runat="server" /><br />
												<%# Container.DataItem("RETAIL_BACK_OFFICE_ORDER_NUMBER")%>
											</div>
										</asp:PlaceHolder>
									</td>
									<td class="ebiz-quantity" data-title="<%#QuantityColumnHeading%>"><%# Container.DataItem("BULK_QTY").ToString()%></td>
									<td class="ebiz-date" data-title="<%=DateColumnHeading%>"><asp:Literal ID="ltlSaleDate" runat="server" /></td>
									<td class="ebiz-customer" data-title="<%=CustomerColumnHeading%>">
                                        <span class="ebiz-customer-number"><%# Container.DataItem("CUSTOMER_NUMBER")%></span>
                                        <span class="ebiz-customer-name"><%# Container.DataItem("CUSTOMER_NAME")%></span>
									</td>
                                    <asp:PlaceHolder ID="plhRetailPurchaseRef" runat="server">
                                        <td class="ebiz-retail-purchase-ref" data-title="<%=RetailPurchaseRefColumnHeading%>"><%# Container.DataItem("RETAIL_PURCHASE_ORDER")%></td>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plhPriceColumnValue" runat="server" Visible="<%#ShowPrices%>">
									    <td class="ebiz-price" data-title="<%=PriceColumnHeading%>"><%# Container.DataItem("PRICE")%></td>
                                    </asp:PlaceHolder>
									<td class="ebiz-status" data-title="<%=StatusColumnHeading%>"><%# GetStatusText(Container.DataItem("STATUS"), Container.DataItem("IS_TICKETING_OR_RETAIL"))%></td>
									<td class="ebiz-details" data-title="&nbsp;"><asp:HyperLink ID="hplDetails" runat="server"><i class="fa fa-info-circle"></i></asp:HyperLink></td>
								</tr>
						</ItemTemplate>
						<FooterTemplate>
								</tbody>
							</table>
					</FooterTemplate>
				</asp:repeater>

				<asp:placeholder id="plhPagerBottom" runat="server">
					<div class="ebiz-pagination">
						<ul class="pagination">
							<asp:PlaceHolder ID="plhFirstPageBottom" runat="server"><li><asp:LinkButton ID="lnkFirstBottom" runat="server" CommandArgument="F" OnCommand="PagerButtonClick"></asp:LinkButton></li></asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPrevPageBottom" runat="server"><li><asp:LinkButton ID="lnkPreviousBottom" runat="server" CommandArgument="P" OnCommand="PagerButtonClick"></asp:LinkButton></li></asp:PlaceHolder>
							<li class="ebiz-current-page"><asp:Literal ID="ltlCurrentPageBottom" runat="server" /></li>
							<asp:PlaceHolder ID="plhNextPageBottom" runat="server"><li><asp:LinkButton ID="lnkNextBottom" runat="server" CommandArgument="N" OnCommand="PagerButtonClick"></asp:LinkButton></li></asp:PlaceHolder>
							<asp:PlaceHolder ID="plhLastPageBottom" runat="server"><li><asp:LinkButton ID="lnkLastBottom" runat="server" CommandArgument="L" OnCommand="PagerButtonClick"></asp:LinkButton></li></asp:PlaceHolder>
						</ul>
					</div>
				</asp:placeholder>

				<asp:placeholder id="plhNoPurchasesFound" runat="server">
					<div class="alert-box warning">
						<asp:Literal ID="ltlNoPurchaseFound" runat="server" />
					</div>
				</asp:placeholder>
				<asp:placeholder id="plhErrorList" runat="server" visible="false">
				<div class="alert-box alert">
					<asp:BulletedList ID="blErrorMessages" runat="server" />
				</div>
				</asp:placeholder>
			</div>
		</div>
	</div>

    <script>
        //This code handles the visibility when there is a post back during purchase history searching.
        $(window).load(function(){
            if (document.getElementById('chkTicketingProducts').checked) {
                $('.ebiz-filter-status').show();
                $('.ebiz-product-type').show();
                $('.ebiz-package-id').show();
                $('.ebiz-booked-seats').show();
                $('.ebiz-show-reservations').show();
                $('.ebiz-loyalty-information').show();
                $('.ebiz-buyback-products').show();
                $('.ebiz-corporate-products').show();
                $('.ebiz-prod-desc').show();
            } else {
                $('.ebiz-filter-status').hide();
                $('.ebiz-product-type').hide();
                $('.ebiz-package-id').hide();
                $('.ebiz-booked-seats').hide();
                $('.ebiz-show-reservations').hide();
                $('.ebiz-loyalty-information').hide();
                $('.ebiz-buyback-products').hide();
                $('.ebiz-corporate-products').hide();
                $('.ebiz-prod-desc').hide();
            }
        })

    </script>
</asp:Content>

