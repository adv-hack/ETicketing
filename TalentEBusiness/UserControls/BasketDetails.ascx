<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BasketDetails.ascx.vb" Inherits="UserControls_BasketDetails" %>
<%@ Register Src="SummaryTotals.ascx" TagName="SummaryTotals" TagPrefix="Talent" %>
<%@ Reference Control="~/UserControls/TicketingBasketDetails.ascx" %>
<%@ Register Src="~/UserControls/TicketingBasketDetails.ascx" TagName="TicketingBasketDetails" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/AlternativeProducts.ascx" TagName="AlternativeProducts" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/CashbackSummary.ascx" TagName="CashbackSummary" TagPrefix="Talent" %>
<%@ Reference Control="~/UserControls/AlternativeProducts.ascx" %>
<%@ Register Src="~/UserControls/CATBasket.ascx" TagName="CATBasket" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/BasketSummary.ascx" TagName="BasketSummary" TagPrefix="Talent" %>

<asp:ValidationSummary ID="vlsBasketSummary" runat="server" ValidationGroup="Basket" ShowSummary="true" CssClass="alert-box alert ebiz-validation-basket-summary" />
<asp:PlaceHolder ID="plhErrorList" runat="server">
	<div class="alert-box alert">
		<asp:BulletedList ID="ErrorList" runat="server" />
		<asp:Repeater ID="rptErrorList" runat="server">
			<HeaderTemplate><ul class="ebiz-override-prerequisite"></HeaderTemplate>
			<ItemTemplate>
				<li>
					<asp:Literal ID="ltlErrorMessage" runat="server" />
					<asp:LinkButton ID="lbtnOverrideOption" runat="server" CssClass="close" CommandName="OVERRIDE">
						<span><asp:Literal ID="ltlOverrideOption" runat="server" /></span> <i class="fa fa-times"></i>
					</asp:LinkButton>
					<asp:HiddenField ID="hdfProductCodeInError" runat="server" />
					<asp:HiddenField ID="hdfStandCodeInError" runat="server" />
				</li>
			</ItemTemplate>
			<FooterTemplate></ul></FooterTemplate>
		</asp:Repeater>
	</div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhSuccessMessages" runat="server">
    <div class="alert-box success">
		<asp:BulletedList ID="blSuccessList" runat="server" />
	</div>    
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhFavouriteSeatMessage" runat="server" Visible="false">
	<div class="alert-box success ebiz-favourite-seat-message">
		<asp:Literal ID="ltlFavouriteSeatMessage" runat="server" />
	</div>
</asp:PlaceHolder>

<div class="row basket-details-wrapper">
	<div class="columns">
		<div id="ecommerceBasketWrapper" runat="server">
			<asp:PlaceHolder ID="plhMerchandiseBasket" runat="server">
				<div class="panel ebiz-merchandising-Basket">
					<h2><asp:Label ID="merchandisingBasketHeaderLabel" runat="server" /></h2>
						<asp:Repeater ID="BasketRepeater" runat="server">
							<HeaderTemplate>
								<table class="stack">
									<thead>
									<tr>
										<th scope="col" id="ImageColumn" class="ebiz-image" runat="server" onprerender="SetVisibility"></th>
										<th scope="col" class="product-code" id="ProductCodeColumn" runat="server" onprerender="SetVisibility">
											<asp:Label ID="ProductCodeLabel" runat="server" OnPreRender="GetText"></asp:Label>
										</th>
										<th scope="col" class="ebiz-name" id="ProductNameColumn" runat="server" onprerender="SetVisibility">
											<asp:Label ID="ProductNameLabel" runat="server" OnPreRender="GetText"></asp:Label>
										</th>
										<th scope="col" class="ebiz-colour" id="ColourColumn" runat="server" onprerender="SetVisibility">
											<asp:Label ID="ColourLabel" runat="server" OnPreRender="GetText"></asp:Label>
										</th>
										<th scope="col" class="ebiz-size" id="SizeColumn" runat="server" onprerender="SetVisibility">
											<asp:Label ID="SizeLabel" runat="server" OnPreRender="GetText"></asp:Label>
										</th>
										<th scope="col" class="ebiz-unit" id="UnitPriceColumn" runat="server" onprerender="SetVisibility">
											<asp:Label ID="UnitPriceLabel" runat="server" OnPreRender="GetText"></asp:Label>
										</th>
										<th scope="col" class="ebiz-quantity" id="QuantityColumn" runat="server" onprerender="SetVisibility">
											<asp:Label ID="QuantityLabel" runat="server" OnPreRender="GetText"></asp:Label>
										</th>
										<th scope="col" class="ebiz-stock" id="StockColumn" runat="server" onprerender="SetVisibility">
											<asp:Label ID="StockLabel" runat="server" OnPreRender="GetText"></asp:Label>
										</th>
										<th scope="col" class="ebiz-value" id="ValueColumn" runat="server" onprerender="SetVisibility">
											<asp:Label ID="ValueLabel" runat="server" OnPreRender="GetText"></asp:Label>
										</th>
										<th scope="col" class="ebiz-cost-centre" id="CostCentreColumn" runat="server" onprerender="SetVisibility">
											<asp:Label ID="CostCentreLabel" runat="server" OnPreRender="GetText"></asp:Label>
										</th>
										<th scope="col" class="ebiz-account-number" id="AccountNoColumn" runat="server" onprerender="SetVisibility">
											<asp:Label ID="AccountNoLabel" runat="server" OnPreRender="GetText"></asp:Label>
										</th>
										<th id="UpdateColumn" class="ebiz-update" runat="server" onprerender="SetVisibility">
										</th>
									</tr>
									</thead>
									<tbody>
							</HeaderTemplate>
							<ItemTemplate>
								<tr>
									<td id="ImageColumn" class="ebiz-image" runat="server" onprerender="SetVisibility">
										<asp:Image ID="ProductImage" runat="server" />
									</td>
									<td class="ebiz-product-code" id="ProductCodeColumn" runat="server" onprerender="SetVisibility">
										<asp:Label ID="ProductCodeLabelVisible" runat="server"></asp:Label>
										<asp:Label ID="ProductCodeLabel" runat="server" Visible="false" Text='<%# Container.DataItem("PRODUCT") %>'></asp:Label>
										<asp:Label ID="BasketDetailIdLabel" runat="server" Visible="false" Text='<%# Container.DataItem("BASKET_DETAIL_ID") %>'></asp:Label>
										<asp:Label ID="XmlConfigLabel" runat="server" Visible="false" Text='<%# Container.DataItem("XML_CONFIG") %>'></asp:Label>
										<asp:Label ID="ProductMasterLabel" runat="server" Visible="false" Text='<%# Container.DataItem("MASTER_PRODUCT") %>'></asp:Label>
									</td>
									<td class="ebiz-name" id="ProductNameColumn" runat="server" onprerender="SetVisibility">
										<asp:CheckBox ID="isFree" runat="server" Checked='<%# Container.DataItem("IS_FREE") %>' Visible="false" />
										<asp:Label ID="ItemErrorLabel" runat="server" CssClass="alert-box alert" Visible="false"></asp:Label>
										<asp:HyperLink CssClass="productLink disabled" ID="ProductHyperLink" runat="server" OnDataBinding="GetURL"></asp:HyperLink>
										<asp:Label CssClass="ProductDescription" ID="ProductDescription" runat="server" OnDataBinding="SetVisibility"></asp:Label>
										<asp:Label CssClass="StockIssue" ID="StockIssueLabel" runat="server" Visible="false"></asp:Label>
										<asp:Label ID="PromoLabel" runat="server" Visible="false"></asp:Label>
									</td>
									<td class="ebiz-colour" id="ColourColumn" runat="server" onprerender="SetVisibility">
										<asp:Label ID="ColourLabel" runat="server"></asp:Label>
									</td>
									<td class="ebiz-size" id="SizeColumn" runat="server" onprerender="SetVisibility">
										<asp:Label ID="SizeLabel" runat="server" Text='<%# Container.DataItem("SIZE")%>'></asp:Label>
									</td>
									<td class="ebiz-unit" id="UnitPriceColumn" runat="server" onprerender="SetVisibility">
										<asp:Label ID="UnitLabel" runat="server"></asp:Label>
									</td>
									<td class="ebiz-quantity" id="QuantityColumn" runat="server" onprerender="SetVisibility">
										<asp:TextBox ID="QuantityBox" runat="server" Text='<%# CInt(Container.DataItem("QUANTITY")) %>' OnDataBinding="SetVisibility" MaxLength="50"></asp:TextBox>
										<asp:RegularExpressionValidator ID="QuantityRegEx" runat="server" Display="Static"
											ControlToValidate="QuantityBox" ValidationGroup="Quantity" OnInit="GetRegExSettings"
											OnDataBinding="SetVisibility" CssClass="error"></asp:RegularExpressionValidator>
										<asp:Label ID="QuantityLabel" runat="server" Text='<%# CInt(Container.DataItem("QUANTITY")) %>' OnDataBinding="SetVisibility"></asp:Label>
									</td>
									<td class="ebiz-stock" id="StockColumn" runat="server" onprerender="SetVisibility">
										<asp:Label ID="StockLabel" runat="server"></asp:Label>
									</td>
									<td class="ebiz-value" id="ValueColumn" runat="server" onprerender="SetVisibility">
										<asp:Label ID="ValueLabel" runat="server"></asp:Label>
									</td>
									<td class="ebiz-cost-centre" id="CostCentreColumn" runat="server" onprerender="SetVisibility">
										<asp:TextBox ID="CostCentreTextBox" runat="server" OnDataBinding="SetVisibility" MaxLength="20"></asp:TextBox>
										<asp:Label ID="CostCentreLabel" runat="server" OnDataBinding="SetVisibility"></asp:Label>
									</td>
									<td class="ebiz-account-no" id="AccountNoColumn" runat="server" onprerender="SetVisibility">
										<asp:TextBox ID="AccountNoTextBox" runat="server" OnDataBinding="SetVisibility" MaxLength="20"></asp:TextBox>
										<asp:Label ID="AccountNoLabel" runat="server" OnDataBinding="SetVisibility"></asp:Label>
									</td>
									<td id="UpdateColumn" class="ebiz-update" runat="server" onprerender="SetVisibility">
										<span data-tooltip aria-haspopup="true" class="has-tip" data-disable-hover="false" title="Add a comment.">
										<a id="hplComments" runat="server" class="fa-input ebiz-fa-input ebiz-open-modal ebiz-add-comment" data-open="comments-modal"><asp:Literal ID="ltlCommentsContent" runat="server" /></a>
											<asp:Button ID="PersonalisationButton" runat="server" OnClick="PersonaliseItem" CssClass="fa-input ebiz-fa-input ebiz-personalise" Visible="false"></asp:Button>
										</span>
										<div id="comments-modal" class="reveal ebiz-reveal-ajax" data-reveal></div>
										<asp:Button ID="DeleteButton" runat="server" OnDataBinding="GetText" OnClick="DeleteItem" CssClass="fa-input ebiz-fa-input ebiz-remove"></asp:Button> 
									</td>
								</tr>
								<asp:PlaceHolder ID="plhTransactionLabel" runat="server">
								<tr>
									<td colspan="12" class="ebiz-transaction-label">
										<asp:Label ID="TransactionLabel" runat="server"></asp:Label>
									</td>
								</tr>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhAlternativeProduct" runat="server">
								<tr>
									<td colspan="12" class="ebiz-alternative-products">
										<Talent:AlternativeProducts ID="alternativeProducts" runat="server" Visible="True" />
									</td>
								</tr>
								</asp:PlaceHolder>
							</ItemTemplate>
							<FooterTemplate>
							</tbody>
								</table>
							</FooterTemplate>
						</asp:Repeater>
					

					<asp:PlaceHolder ID="plhPromotionsSummary" runat="server">
						<div class="panel ebiz-promotions-summary">
							<h2><asp:Label ID="promoSummaryTitle" runat="server" OnPreRender="GetText"></asp:Label></h2>
							<asp:BulletedList ID="PromotionsSummary" runat="server" />
						</div>
					</asp:PlaceHolder>

					<Talent:SummaryTotals ID="SummaryTotals1" runat="server" />
				</div>
			</asp:PlaceHolder>

			<asp:PlaceHolder ID="plhTicketingBasket" runat="server">
				<div class="panel ebiz-ticketing-basket <%= BulkSalesModeCssClass%>">
					<Talent:CATBasket ID="CATBasket1" runat="server" />
					<Talent:HTMLInclude ID="Basket_HTMLInclude2" runat="server" Usage="2" Sequence="2" />
					<Talent:TicketingBasketDetails ID="TicketingBasketDetails1" runat="server" />
				</div>
			</asp:PlaceHolder>

		</div>
	</div>
	<div class="columns">
		<Talent:CashbackSummary ID="CashBackSummary" runat="server" />
		<Talent:BasketSummary ID="BasketSummary1" runat="server"></Talent:BasketSummary>
	</div>
</div>