<%@ Page Language="VB" AutoEventWireup="false" ViewStateMode="Disabled" CodeFile="Checkout.aspx.vb" Inherits="PagesLogin_Checkout_Checkout" MaintainScrollPositionOnPostback="true" %>

<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PaymentDetails.ascx" TagName="PaymentDetails" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/MarketingCampaigns.ascx" TagName="MarketingCampaigns" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/MySavedCards.ascx" TagName="MySavedCards" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/DirectDebit.ascx" TagName="DirectDebit" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/DirectDebitMandate.ascx" TagName="DirectDebitMandate" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ApplyCashback.ascx" TagName="ApplyCashback" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/EPurse.ascx" TagName="EPurse" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/CreditFinance.ascx" TagName="CreditFinance" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/CreditFinanceMandate.ascx" TagName="CreditFinanceMandate" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/BasketDetails.ascx" TagName="BasketDetails" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/SessionStatus.ascx" TagName="SessionStatus" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/DeliveryAddress.ascx" TagName="DeliveryAddress" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/BasketSummary.ascx" TagName="BasketSummary" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/BasketFees.ascx" TagName="BasketFees" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/CheckoutPartPayments.ascx" TagName="CheckoutPartPayments" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PaymentCardType.ascx" TagPrefix="Talent" TagName="PaymentCardType" %>
<%@ Register Src="~/UserControls/PaymentCardDetails.ascx" TagPrefix="Talent" TagName="PaymentCardDetails" %>
<%@ Register Src="~/UserControls/CustomerProgressBar.ascx" TagPrefix="Talent" TagName="CustomerProgressBar" %>


<asp:content id="cphHead" contentplaceholderid="ContentPlaceHolder2" runat="server">
	<asp:Literal ID="ltlPayPalObjectFile" runat="server" />
	<script type="text/javascript">
	    function ValidateTerms(sender, e) {
	        e.IsValid = jQuery(".ebiz-tandc input:checkbox").is(':checked');
	    }
	    function OpenNextItem(item) {
	        $(".ebiz-checkout-accordion").accordion("option", "active", item);
	    }

	    function ValidateOthersText(sender, e) {
	        var arr = $('#<%=hdnOthers.ClientID%>').val();
	        var str_array = arr.split(';');
	        for (var i = 0; i < str_array.length - 1; i++) {
	            var sub_str_array = str_array[i].split(':');
	            if ($('#<%=ddlOthersType.ClientID%> option:selected').val() == sub_str_array[0]) {
		            if (sub_str_array[1] == 'True') {
		                if (!$('#<%=txtOthersConfigurableValue.ClientID %>').val()) {
				            if (!(e == null)) { e.IsValid = false; }
				            $('#spOthersMandatoryError').text($('#<%=hdnOthersErrorMessage.ClientID%>').val());
					        document.getElementById('spOthersMandatoryError').style.display = '';
                        }
                        else {
                            document.getElementById('spOthersMandatoryError').style.display = 'none';
                        }
                    }
                }
            }
        }

        $('#<%=txtOthersConfigurableValue.ClientID%>').focusout(function () {
	        ValidateOthersText(null, null);
	    });

	    function ValidateSortCode(sender, e) {
	        var sortCodeValid = false;
	        if (jQuery(".sort-code-1").val().length == 2) {
	            if (jQuery(".sort-code-2").val().length == 2) {
	                if (jQuery(".sort-code-3").val().length == 2) { sortCodeValid = true; }
	            }
	        }
	        e.IsValid = sortCodeValid;
	    }
	</script>
</asp:content>

<asp:content id="CheckoutContent" contentplaceholderid="ContentPlaceHolder1" runat="server">

<Talent:CustomerProgressBar ID="ProgressBar1" runat="server"></Talent:CustomerProgressBar>
<Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
<Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />
	<asp:UpdatePanel ID="updCheckout" runat="server" ViewStateMode="Enabled">
		<Triggers>
			<asp:PostBackTrigger ControlID="btnConfirmCCPayment" />
			<asp:PostBackTrigger ControlID="btnConfirmSavedCardPayment" />
			<asp:PostBackTrigger ControlID="imgBtnConfirmPayPalPayment" />
			<asp:PostBackTrigger ControlID="btnConfirmDDMandate" />
			<asp:PostBackTrigger ControlID="btnConfirmCFMandate" />
			<asp:PostBackTrigger ControlID="btnConfirmCashPayment" />
			<asp:PostBackTrigger ControlID="btnConfirmEPursePayment" />
			<asp:PostBackTrigger ControlID="btnPromotions" />
			<asp:PostBackTrigger ControlID="btnZeroPricedBasketPayment" />
			<asp:PostBackTrigger ControlID="btnConfirmOnAccountPayment" />
			<asp:PostBackTrigger ControlID="btnConfirmPDQPayment" />
			<asp:PostBackTrigger ControlID="btnConfirmOthersPayment" />
			<asp:PostBackTrigger ControlID="uscBasketSummary" />
			<asp:PostBackTrigger ControlID="btnConfirmInvoicePayment" />
			<asp:PostBackTrigger ControlID="btnPaymentOptionSelected" />
            <asp:PostBackTrigger ControlID="btnConfirmCCType" />
			<asp:AsyncPostBackTrigger ControlID="btnConfirmChipAndPinPayment" />
			<asp:AsyncPostBackTrigger ControlID="btnConfirmDDPayment" />
			<asp:AsyncPostBackTrigger ControlID="btnCancelDDMandate" />
			<asp:AsyncPostBackTrigger ControlID="btnConfirmCFPayment" />
			<asp:AsyncPostBackTrigger ControlID="btnCancelCFMandate" />
			<asp:AsyncPostBackTrigger ControlID="btnConfirmPointOfSalePayment" />
			<asp:AsyncPostBackTrigger ControlID="uscApplyCashback" />
			<asp:AsyncPostBackTrigger ControlID="btnConfirmDeliveryAddress" />
			<asp:AsyncPostBackTrigger ControlID="uscDeliveryAddress" />
		</Triggers>
		<ContentTemplate>
				
		<div id="vlsPaymentCardDetailsErrors" class="alert-box alert ebiz-payment-card-details-errors" style="display:none;">
			<ul id="PaymentCardDetailsErrors">
			</ul>
		</div>
		<asp:ValidationSummary ID="vlsCheckoutErrors" runat="server" ValidationGroup="Checkout" ShowSummary="true" CssClass="alert-box alert ebiz-checkout-eErrors" />
		<asp:ValidationSummary ID="vlsTicketingPromotionErrors" runat="server" ValidationGroup="TicketingPromotions" ShowSummary="true" CssClass="alert-box alert ebiz-ticketing-promotion-errors" />
		<asp:CustomValidator ID="csvIsOthersTextMandatory" runat="server" ValidationGroup="Checkout"
			OnServerValidate="ValidateOthersText" ClientValidationFunction="ValidateOthersText" EnableClientScript="true"
			Display="Dynamic" CssClass="alert-box alert ebiz-validate-others-text" SetFocusOnError="true"  />
		<asp:ValidationSummary ID="vlsDeliveryAddressErrors" runat="server" CssClass="alert-box alert" ValidationGroup="DeliveryAddress" ShowSummary="true" />
		<asp:PlaceHolder ID="plhErrorList" runat="server" Visible="false">
			<div class="alert-box alert ebiz-error-messages">
				<asp:BulletedList ID="blErrorMessages" runat="server" />
			</div>
		</asp:PlaceHolder>
		<asp:PlaceHolder ID="plhSuccessMessage" runat="server" Visible="false">
			<div class="alert-box success">
				<asp:Literal ID="ltlSuccessMessages" runat="server" />
			</div>
		</asp:PlaceHolder>

		 <asp:PlaceHolder ID="plhWarningMessage" runat="server" Visible="false">
			<div class="alert-box warning">
				<asp:Literal ID="ltlWarningMessage" runat="server" />
			</div>
		</asp:PlaceHolder>

		<div class="row ebiz-checkout-wrap">
			<div class="ebiz-checkout-accordion ebiz-accordion large-9 columns">
				<asp:PlaceHolder ID="plhPPSList" runat="server" Visible="false">
					<div class="checkout-pps-list">
						<ul>
							<asp:PlaceHolder ID="plhPPSListPPS1" runat="server" Visible="false">
								<li class="checkout-pps-list-pps1 <%=CSSClassPPS1 %>">
									<asp:BulletedList ID="blPPS1List" runat="server" />
									<div class="pay-type">
										<strong><asp:Literal ID="ltlPPS1PaidUsingLabel" runat="server" /></strong>
										<span><asp:Literal ID="ltlPPS1PaidUsing" runat="server" /></span>
									</div>
								</li>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPPSListPPS2" runat="server" Visible="false">
								<li class="checkout-pps-list-pps2 <%=CSSClassPPS2 %>">
									<asp:BulletedList ID="blPPS2List" runat="server" />
									<div class="pay-type">
										<strong><asp:Literal ID="ltlPPS2PaidUsingLabel" runat="server" /></strong>
										<span><asp:Literal ID="ltlPPS2PaidUsing" runat="server" /></span>
									</div>
								</li>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPPSListSeasonTicket" runat="server" Visible="false">
								<li class="checkout-pps-list-season-ticket not-paid">
									<ul>
										<li><asp:Literal ID="ltlSeasonTicket" runat="server" /></li>
									</ul>
									<div class="pay-type">
										<strong><asp:Literal ID="ltlSeasonTicketPaidUsingLabel" runat="server" /></strong>
										<span><asp:Literal ID="ltlSeasonTicketPaidUsing" runat="server" /></span>
									</div>
								</li>
							</asp:PlaceHolder>
						</ul>
						<asp:PlaceHolder ID="plhResetPayments" runat="server" Visible="false">
							<div class="checkout-pps-list-reset">
								<a href="Checkout.aspx?clearpps=true" class="hyperlink-button button"><asp:Literal ID="ltlResetPayments" runat="server" /></a>
							</div>
						</asp:PlaceHolder>
					</div>
				</asp:PlaceHolder>
					
				<asp:PlaceHolder ID="plhPPSPaymentOptionsMessage" runat="server">
					<div class="pps-payment-messages message">
						<p><asp:Literal ID="ltlPPSPaymentOptionsMessage" runat="server" /></p>
						<asp:BulletedList ID="blProductList" runat="server" CssClass="product-list" />
					</div>
				</asp:PlaceHolder>

				<!-- Marketing Campaigns -->
				<asp:PlaceHolder ID="plhMarketingCampaigns" runat="server">
					<div class="panel header ebiz-checkout-marketing-option">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhMarketingCampaignTitle" runat="server">
									<h2><asp:Literal ID="ltlMarketingCampaignTitle" runat="server" /></h2></p>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhMarketingCampaignSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlMarketingCampaignSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhMarketingCampaignNumber" runat="server">
									<asp:Literal ID="ltlMarketingCampaignNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-marketing">
						<Talent:MarketingCampaigns ID="uscMarketingCampaigns" runat="server" Display="true" />
					</div>
				</asp:PlaceHolder>

				<!-- Auto enrol membership renewals -->
				<asp:PlaceHolder ID="plhAutoEnrolPPS" runat="server">
					<div class="panel ebiz-auto-enrol-pps">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhAutoEnrolPPSTitle" runat="server">
									<h2><asp:Literal ID="ltlAutoEnrolPPSTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:CheckBox ID="chkAutoEnrolPPS" runat="server" ViewStateMode="Enabled" />
								<asp:Label ID="lblAutoEnrolPPS" runat="server" AssociatedControlID="chkAutoEnrolPPS" />
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhAutoEnrolPPSNumber" runat="server">
									<asp:Literal ID="ltlAutoEnrolPPSNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
				</asp:PlaceHolder>

				<!-- Delivery Address -->
				<asp:PlaceHolder ID="plhDeliveryAddress" runat="server" Visible="False">
					<div class="panel header ebiz-delivery-address-option">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhDeliveryAddressTitle" runat="server">
									<h2><asp:Literal ID="ltlDeliveryAddressTitle" runat="server" ClientIDMode="Static" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhDeliveryAddressSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlDeliveryAddressSubTitle" runat="server" ClientIDMode="Static" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhDeliveryAddressNumber" runat="server">
									<asp:Literal ID="ltlDeliveryAddressNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-delivery-address">
						<Talent:DeliveryAddress ID="uscDeliveryAddress" runat="server" ViewStateMode="Enabled" />
						<div class="ebiz-submit-button-wrap ebiz-confirm-delivery-address-button-wrap">
							<asp:Button ID="btnConfirmDeliveryAddress" CssClass="button continue" runat="server" ClientIDMode="Static" ValidationGroup="DeliveryAddress" CausesValidation="true" />
						</div>
					</div>
				</asp:PlaceHolder>
				
				<!-- Please Choose A Payment Option -->
				<asp:PlaceHolder ID="plhSelectPaymentMethod" runat="server">
					<div class="panel header ebiz-checkout-payment-method">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								 <asp:PlaceHolder ID="plhPaymentOptionTitle" runat="server">
									<h2><asp:Literal ID="ltlPaymentOptionTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<p class="ebiz-subheader"><asp:Literal ID="ltlPaymentOption" runat="server" /></p>
							</div>
							<div class="columns ebiz-sequence">
								 <asp:PlaceHolder ID="plhPaymentOptionNumber" runat="server">
									<asp:Literal ID="ltlPaymentOptionNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-payment-method">
						<asp:PlaceHolder ID="plhPaymentOptionsMessage" runat="server">
							<p class="ebiz-message"><asp:Literal ID="ltlPaymentOptionsMessage" runat="server" /></p>
						</asp:PlaceHolder>
						<fieldset>
							<legend><asp:Literal ID="ltlPaymentOptionsLegend" runat="server" /></legend>
							
							<asp:PlaceHolder ID="plhPaymentMethodCreditCard" runat="server">
								<div class="row ebiz-credit-card">
									<div class="medium-6 large-4 columns">
										<asp:RadioButton runat="server" ID="rdoCreditCard" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblCreditCard" runat="server" AssociatedControlID="rdoCreditCard" />
									</div>
									<div class="medium-6 large-8 columns">
										<asp:Image ID="imgMaestroCard" runat="server" SkinID="MaestroCard" />
										<asp:Image ID="imgMasterCard" runat="server" SkinID="MasterCard" />
										<asp:Image ID="imgVisaCard" runat="server" SkinID="VisaCard" />
										<asp:Image ID="imgVisaDebitCard" runat="server" SkinID="VisaDebitCard" />
										<asp:Image ID="imgVisaElectronCard" runat="server" SkinID="VisaElectronCard" />
										<asp:Image ID="imgAmexCard" runat="server" SkinID="AmexCard" />
									</div>
								</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPaymentMethodSavedCard" runat="server">
								<div class="row ebiz-saved-card">
									<div class="medium-6 large-4 columns">
										<asp:RadioButton runat="server" ID="rdoSavedCard" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblSavedCard" runat="server" AssociatedControlID="rdoSavedCard" />
									</div>
									<div class="medium-6 large-8 columns">
										<asp:Image ID="imgSavedMastroCard" runat="server" SkinID="MaestroCard" />
										<asp:Image ID="imgSavedMasterCard" runat="server" SkinID="MasterCard" />
										<asp:Image ID="imgSavedVisaCard" runat="server" SkinID="VisaCard" />
										<asp:Image ID="imgSavedVisaDebitCard" runat="server" SkinID="VisaDebitCard" />
										<asp:Image ID="imgSavedVisaElectronCard" runat="server" SkinID="VisaElectronCard" />
										<asp:Image ID="imgSavedAmexCard" runat="server" SkinID="AmexCard" />
									</div>
								</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPaymentMethodPayPal" runat="server">
							<asp:HiddenField ID="hdfPayPalAccountID" runat="server" ClientIDMode="Static" />
							<asp:HiddenField ID="hdfPayPalEnvironment" runat="server" ClientIDMode="Static" />
								<div class="row ebiz-paypal">
									<div class="medium-6 large-4 columns">
										<asp:RadioButton runat="server" ID="rdoPayPal" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblPayPal" runat="server" AssociatedControlID="rdoPayPal" />
									</div>
									<div class="medium-6 large-8 columns">
										 <asp:Image ID="imgPayPal" runat="server" SkinID="PayPal" />
									</div>
								</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPaymentMethodDirectDebit" runat="server">
							<div class="row ebiz-direct-debit">
								<div class="medium-6 large-4 columns">
									<asp:RadioButton runat="server" ID="rdoDirectDebit" GroupName="PaymentMethod" ViewStateMode="Enabled" />
									<asp:Label ID="lblDirectDebit" runat="server" AssociatedControlID="rdoDirectDebit" />
								</div>
								<div class="medium-6 large-8 columns">
									<asp:Image ID="imgDirectDebit" runat="server" SkinID="DirectDebit" />
								</div>
							</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPaymentMethodCashback" runat="server">
								<div class="row ebiz-cashback">
									<div class="medium-6 large-4 columns">
										<asp:RadioButton runat="server" ID="rdoCashback" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblCashback" runat="server" AssociatedControlID="rdoCashback" />
									</div>
									<div class="medium-6 large-8 columns">
										<asp:Image ID="imgCashback" runat="server" SkinID="Cashback" />
									</div>
								</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPaymentMethodEPurse" runat="server">
								<div class="row ebiz-e-purse">
									<div class="medium-6 large-4 columns">
										<asp:RadioButton runat="server" ID="rdoEPurse" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblEPurse" runat="server" AssociatedControlID="rdoEPurse" />
									</div>
									<div class="medium-6 large-8 columns">
										<asp:Image ID="imgEPurse" runat="server" SkinID="EPurse" />
									</div>
								</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPaymentMethodCreditFinance" runat="server">
								<div class="row ebiz-credit-finance">
									<div class="medium-6 large-4 columns">
										<asp:RadioButton runat="server" ID="rdoCreditFinance" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblCreditFinance" runat="server" AssociatedControlID="rdoCreditFinance" />
									</div>
									<div class="medium-6 large-8 columns">
										<asp:Image ID="imgCreditFinance" runat="server" SkinID="CreditFinance" />
									</div>
								</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPaymentMethodCash" runat="server">
								<div class="row ebiz-cash">
									<div class="large-12 columns">
										<asp:RadioButton runat="server" ID="rdoCash" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblCash" runat="server" AssociatedControlID="rdoCash" />
									</div>
								</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPaymentMethodOnAccount" runat="server">
								<div class="row ebiz-onaccount">
									<div class="large-12 columns">
										<asp:RadioButton runat="server" ID="rdoOnAccount" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblOnAccount" runat="server" AssociatedControlID="rdoOnAccount" />
									</div>
								</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPaymentMethodChipAndPin" runat="server">
								<div class="row ebiz-chip-and-pin">
									<div class="large-12 columns">
										<asp:RadioButton runat="server" ID="rdoChipAndPin" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblChipAndPin" runat="server" AssociatedControlID="rdoChipAndPin" />
									</div>
								</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPaymentMethodPointOfSale" runat="server">
								<div class="row ebiz-point-of-sale">
									<div class="large-12 columns">
										<asp:RadioButton runat="server" ID="rdoPointOfSale" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblPointOfSale" runat="server" AssociatedControlID="rdoPointOfSale" />
									</div>
								</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPaymentMethodZeroPricedBasket" runat="server">
								<div class="row ebiz-zero-price">
									<div class="large-12 columns">
										<asp:RadioButton runat="server" ID="rdoZeroPricedBasket" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblZeroPricedBasket" runat="server" AssociatedControlID="rdoZeroPricedBasket" />
									</div>
								</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPaymentMethodPDQ" runat="server">
								<div class="row ebiz-pdq">
									<div class="large-12 columns">
										<asp:RadioButton runat="server" ID="rdoPDQ" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblPDQ" runat="server" AssociatedControlID="rdoPDQ" />
									</div>
								</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPaymentMethodGoogleCheckout" runat="server">
								<div class="row ebiz-googlecheckout">
									<div class="medium-6 large-4 columns">
										<asp:RadioButton runat="server" ID="rdoGoogleCheckout" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblGoogleCheckout" runat="server" AssociatedControlID="rdoGoogleCheckout" />
									</div>
									<div class="medium-6 large-8 columns">
										<asp:Image ID="imgGoogleCheckout" runat="server" SkinID="GoogleCheckout" />
									</div>
								</div>
							</asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhPaymentMethodInvoice" runat="server">
								<div class="row ebiz-others">
									<div class="large-12 columns">
										<asp:RadioButton runat="server" ID="rdoInvoice" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblInvoice" runat="server" AssociatedControlID="rdoInvoice" />
									</div>
								</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPaymentMethodOthers" runat="server">
								<div class="row ebiz-others">
									<div class="large-12 columns">
										<asp:RadioButton runat="server" ID="rdoOthers" GroupName="PaymentMethod" ViewStateMode="Enabled" />
										<asp:Label ID="lblOthers" runat="server" AssociatedControlID="rdoOthers" />
									</div>
								</div>
							</asp:PlaceHolder>
							<div class="ebiz-payment-option-selected-wrap  ebiz-submit-button-wrap">
								<asp:Button ID="btnPaymentOptionSelected" runat="server" ViewStateMode="Enabled" CssClass="button" />
							</div>
						</fieldset>
					</div>
				</asp:PlaceHolder>

				<!-- Credit/Debit Card Type -->   
				<asp:PlaceHolder ID="plhCCType" runat="server">   
					<div class="panel header ebiz-checkout-cc-type">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								 <asp:PlaceHolder ID="plhCCTypeTitle" runat="server">
									<h2><asp:Literal ID="ltlCCTypeTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhCCTypeSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlCCTypeSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhCCTypeNumber" runat="server">
									<asp:Literal ID="ltlCCTypeNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-cc-type">
						<asp:Panel ID="pnlCCTypeWrapper" runat="server" CssClass="cc-type-wrapper" DefaultButton="btnConfirmCCType">
							<asp:PlaceHolder ID="plhCCTypeMessage" runat="server">
								<p class="ebiz-message"><asp:Literal ID="ltlCCTypeMessage" runat="server" /></p>
							</asp:PlaceHolder>
							<fieldset>
								<legend><asp:Literal ID="ltlCCTypeLegend" runat="server" /></legend>
								<Talent:PaymentCardType ID="uscPaymentCardType" runat="server" ViewStateMode="Enabled" />     
								<div class="ebiz-button-confirm-cc-type-wrap ebiz-submit-button-wrap">                        
									<asp:Button ID="btnConfirmCCType" runat="server" CssClass="button" CausesValidation="true" ValidationGroup="Checkout" />
								</div>
							</fieldset>
						</asp:Panel>
					</div>
				</asp:PlaceHolder>

				<!-- Credit/Debit Card Posting To External-->   
				<asp:PlaceHolder ID="plhCCExternal" runat="server">   
					<div class="panel header ebiz-checkout-cc-external">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								 <asp:PlaceHolder ID="plhCCExternalTitle" runat="server">
									<h2><asp:Literal ID="ltlCCExternalTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhCCExternalSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlCCExternalSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhCCExternalNumber" runat="server">
									<asp:Literal ID="ltlCCExternalNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-cc-external">
						<asp:Panel ID="pnlCCExternalWrapper" runat="server" CssClass="cc-external-wrapper" DefaultButton="btnConfirmCCExternalPayment">
							<asp:PlaceHolder ID="plhCCExternalMessage" runat="server">
								<p class="ebiz-message"><asp:Literal ID="ltlCCExternalMessage" runat="server" /></p>
							</asp:PlaceHolder>
							<fieldset>
								<legend><asp:Literal ID="ltlCCExternalLegend" runat="server" /></legend>
								<Talent:PaymentCardDetails ID="uscPaymentCardDetails" runat="server" Usage="CHECKOUT" ViewStateMode="Enabled" />                             
								<Talent:CheckoutPartPayments ID="uscCCExternalPartPayments" visible="false"  runat="server" ViewStateMode="Enabled" />   
								<div class="ebiz-confirm-cc-external-payment-wrap ebiz-submit-button-wrap">                                   
									<asp:Button ID="btnConfirmCCExternalPayment" runat="server" CssClass="button" CausesValidation="true" ValidationGroup="Checkout" OnClientClick="submitToVanguard(); return false;" />
								</div>
							</fieldset>
						</asp:Panel>
					</div>
				</asp:PlaceHolder>
				 
				<!-- Credit/Debit Card -->   
				<asp:PlaceHolder ID="plhCreditCard" runat="server">   
					<div class="panel header ebiz-checkout-credit-card">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								 <asp:PlaceHolder ID="plhCreditCardTitle" runat="server">
									<h2><asp:Literal ID="ltlCreditCardTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhCreditCardSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlCreditCardSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhCreditCardNumber" runat="server">
									<asp:Literal ID="ltlCreditCardNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-credit-card">
						<asp:Panel ID="pnlCreditCardWrapper" runat="server" CssClass="credit-cards-wrapper" DefaultButton="btnConfirmCCPayment">
							<asp:PlaceHolder ID="plhCreditCardMessage" runat="server">
								<p class="ebiz-message"><asp:Literal ID="ltlCreditCardMessage" runat="server" /></p>
							</asp:PlaceHolder>
							<fieldset>
								<legend><asp:Literal ID="ltlCreditCardLegend" runat="server" /></legend>
								<Talent:PaymentDetails ID="uscPaymentDetails" runat="server" ViewStateMode="Enabled" />                             
								   
								<Talent:CheckoutPartPayments ID="uscCCPartPayment"  runat="server" ViewStateMode="Enabled" />                                      
								

								<asp:PlaceHolder ID="plhCreditCardTAndC" runat="server">
									<div class="ebiz-terms-and-conditions-wrap ebiz-checkbox-row-wrap">
										<asp:CheckBox ID="chkCardTAndC" runat="server" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" CausesValidation="true" ValidationGroup="Checkout" />
										<asp:Label ID="lblCardTAndC" runat="server" AssociatedControlID="chkCardTAndC" CssClass="tandc-label" />
										<asp:CustomValidator ID="csvCardTAndC" runat="server" ValidationGroup="Checkout" OnServerValidate="ValidateTerms" ClientValidationFunction="ValidateTerms" EnableClientScript="true" SetFocusOnError="true" Display="Static" CssClass="error" />
									</div>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhCustomerPresentCC" runat="server" Visible="False">
									<div class="ebiz-customer-present ebiz-checkbox-row-wrap">
										<asp:CheckBox ID="chkCustomerPresentCC" runat="server" CssClass="ebiz-custpres ebiz-checkbox-label-wrap"/>
										<asp:Label ID="lblCustomerPresentCC" runat="server" AssociatedControlID="chkCustomerPresentCC" CssClass="custpres-label" />
									</div>
								</asp:PlaceHolder>  
								<asp:Button ID="btnConfirmCCPayment" runat="server" CssClass="button" CausesValidation="true" ValidationGroup="Checkout" />
							</fieldset>
						</asp:Panel>
					</div>
				</asp:PlaceHolder>

				   
			   <!--  Saved Credit/Debit Card --> 
				<asp:PlaceHolder ID="plhSavedCard" runat="server">
					<div class="panel header ebiz-checkout-saved-credit-cards">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhSavedCardTitle" runat="server">
									<h2><asp:Literal ID="ltlSavedCardTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhSavedCardSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlSavedCardSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhSavedCardNumber" runat="server">
									<asp:Literal ID="ltlSavedCardNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div> 
					</div>
					<div class="panel ebiz-content ebiz-checkout-saved-credit-cards">
						<asp:Panel ID="pnlSavedCardsWrapper" runat="server" DefaultButton="btnConfirmSavedCardPayment">
							<fieldset>
								<legend><asp:Literal ID="ltlSavedCardsLegend" runat="server" /></legend>
								<Talent:MySavedCards ID="uscMySavedCards" runat="server" ShowDeleteButton="false" ShowDefaultButton="false" ShowSecurityNumber="true" />
								<Talent:CheckoutPartPayments ID="uscSCPartPayment"   runat="server" />  
							   
								<asp:PlaceHolder ID="plhSavedCardsTAndC" runat="server">
									<div class="ebiz-terms-and-conditions-wrap ebiz-checkbox-row-wrap">
										<asp:CheckBox ID="chkSavedCardTAndC" runat="server" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" CausesValidation="true" ValidationGroup="Checkout" />
										<asp:Label ID="lblSavedCardTAndC" runat="server" AssociatedControlID="chkSavedCardTAndC" CssClass="tandc-label" />
										<asp:CustomValidator ID="csvSavedCardTAndC" runat="server" ValidationGroup="Checkout" OnServerValidate="ValidateTerms" ClientValidationFunction="ValidateTerms" EnableClientScript="true" Display="Static" CssClass="error" SetFocusOnError="true" />
									</div>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhCustomerPresentSC" runat="server" Visible="False">
									<div class="ebiz-customer-present">
										<asp:CheckBox ID="chkCustomerPresentSC" runat="server" CssClass="ebiz-custpres ebiz-checkbox-label-wrap"/>
										<asp:Label ID="lblCustomerPresentSC" runat="server" AssociatedControlID="chkCustomerPresentSC" CssClass="custpres-label" />
									</div>
								</asp:PlaceHolder>  
								<asp:Button ID="btnConfirmSavedCardPayment" runat="server" CssClass="button" CausesValidation="true" ValidationGroup="Checkout" />
							</fieldset>
						</asp:Panel>
					</div>
				</asp:PlaceHolder>

				 <!-- PayPal -->
				<asp:PlaceHolder ID="plhPayPal" runat="server">
					<div class="panel header ebiz-checkout-paypal">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhPayPalTitle" runat="server">
									<h2><asp:Literal ID="ltlPayPalTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhPayPalSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlPayPalSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhPayPalNumber" runat="server">
									<asp:Literal ID="ltlPayPalNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-paypal">
						<fieldset>
							<legend><asp:Literal ID="ltlPayPalLegend" runat="server" /></legend>
							<asp:PlaceHolder ID="plhPayPalMessage" runat="server">
								<p class="message"><asp:Literal ID="ltlPayPalMessage" runat="server" /></p>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPayPalTAndC" runat="server">
								<div class="ebiz-terms-and-conditions-wrap ebiz-checkbox-row-wrap">
									<asp:CheckBox ID="chkPayPalTAndC" runat="server" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" CausesValidation="true" ValidationGroup="Checkout" />
									<asp:Label ID="lblPayPalTAndC" runat="server" AssociatedControlID="chkPayPalTAndC" />
									<asp:CustomValidator ID="csvPayPalTAndC" runat="server" ValidationGroup="Checkout"
										OnServerValidate="ValidateTerms" ClientValidationFunction="ValidateTerms" EnableClientScript="true"
										Display="Static" CssClass="error" SetFocusOnError="true" />
								</div>
							</asp:PlaceHolder>
							<asp:ImageButton ID="imgBtnConfirmPayPalPayment" runat="server" SkinID="PayPalCheckout" CausesValidation="true" ValidationGroup="Checkout" ClientIDMode="Static"/>
						</fieldset>
					</div>
				</asp:PlaceHolder>
					
				<!-- Direct Debit  -->	
				<asp:PlaceHolder ID="plhDirectDebit" runat="server">
					<div class="panel header checkout-direct-debit">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhDirectDebitTitle" runat="server">
									<h2><asp:Literal ID="ltlDirectDebitTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhDirectDebitSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlDirectDebitSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhDirectDebitNumber" runat="server">
									<asp:Literal ID="ltlDirectDebitNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<asp:Panel ID="pnlDirectDebitWrapper" runat="server" CssClass="panel checkout-direct-debit content" DefaultButton="btnConfirmDDPayment">
						<asp:PlaceHolder ID="plhDirectDebitWrapper" runat="server">
							<asp:PlaceHolder ID="plhDirectDebitH2" runat="server">
								<h2><asp:Literal ID="ltlDirectDebitH2" runat="server" /></h2>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhDirectDebitMessage" runat="server">
								<p class="message"><asp:Literal ID="ltlDirectDebitMessage" runat="server" /></p>
							</asp:PlaceHolder>
							<fieldset>
								<legend><asp:Literal ID="ltlDirectDebitLegend" runat="server" /></legend>
								<Talent:DirectDebit ID="uscDirectDebit" runat="server" UsePaymentDaysDDL="false" />
								<asp:HiddenField ID="hdfProductForDD" runat="server" />
								<asp:PlaceHolder ID="plhDirectDebitTAndC" runat="server">
									<div class="ebiz-terms-and-conditions-wrap ebiz-checkbox-row-wrap">
										<asp:CheckBox ID="chkDDTAndC" runat="server" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" CausesValidation="true" ValidationGroup="Checkout" />
										<asp:Label ID="lblDDTAndC" runat="server" AssociatedControlID="chkDDTAndC" />
										<asp:CustomValidator ID="csvDDTAndC" runat="server" ValidationGroup="Checkout" OnServerValidate="ValidateTerms"
											ClientValidationFunction="ValidateTerms" EnableClientScript="true" Display="Static"
											CssClass="error" SetFocusOnError="true" />
									</div>
								</asp:PlaceHolder>
								<asp:Button ID="btnConfirmDDPayment" runat="server" CssClass="button ebiz-primary-action ebiz-confirm-dd-payment" CausesValidation="true" ValidationGroup="Checkout" />
							</fieldset>
						</asp:PlaceHolder>
						<asp:PlaceHolder ID="plhDirectDebitMandateWrapper" runat="server" Visible="false">
							<Talent:DirectDebitMandate ID="uscDirectDebitMandate" runat="server" Visible="false" />
								<div class="button-group">
									<asp:Button ID="btnCancelDDMandate" runat="server" CssClass="button ebiz-muted-action ebiz-cancel-direct-debit-mandate" CausesValidation="true" ValidationGroup="Checkout" />
									<asp:Button ID="btnConfirmDDMandate" runat="server" CssClass="button ebiz-primary-action ebiz-confirm-direct-debit-mandate" CausesValidation="true" ValidationGroup="Checkout" />
								</div>
						</asp:PlaceHolder>
					</asp:Panel>
				</asp:PlaceHolder>

				<!-- Cashback  -->
				<asp:PlaceHolder ID="plhCashback" runat="server">
					<div class="panel header ebiz-checkout-cashback">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhCashbackTitle" runat="server">
									<h2><asp:Literal ID="ltlCashbackTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhCashbackSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlCashbackSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhCashbackNumber" runat="server">
									<asp:Literal ID="ltlCashbackNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-cashback">
						<fieldset>
							<legend><asp:Literal ID="ltlCashbackLegend" runat="server" /></legend>
							<asp:PlaceHolder ID="plhCashbackMessage" runat="server">
								<p class="message"><asp:Literal ID="ltlCashbackMessage" runat="server" /></p>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhCashbackTAndC" runat="server">
								<div class="ebiz-terms-and-conditions-wrap ebiz-checkbox-row-wrap">
									<asp:CheckBox ID="chkCashbackTAndC" runat="server" CausesValidation="true" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" ValidationGroup="Checkout" ViewStateMode="Enabled"/>
									<asp:Label ID="lblCashbackTAndC" runat="server" AssociatedControlID="chkCashbackTAndC" />
									<asp:CustomValidator ID="csvCashbackTAndC" runat="server" ValidationGroup="Checkout" OnServerValidate="ValidateTerms" ClientValidationFunction="ValidateTerms" EnableClientScript="true" Display="Static" CssClass="error" SetFocusOnError="true" />
								</div>
							</asp:PlaceHolder>
							<Talent:ApplyCashback ID="uscApplyCashback" runat="server" />
						</fieldset>
					</div>
				</asp:PlaceHolder>

				<!-- ePurse -->
				<asp:PlaceHolder ID="plhEPurse" runat="server">
					<div class="panel header ebiz-checkout-epurse">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhEPurseTitle" runat="server">
									<h2><asp:Literal ID="ltlEPurseTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhEPurseSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlEPurseSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhEPurseNumber" runat="server">
									<asp:Literal ID="ltlEPurseNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-epurse">
						<asp:PlaceHolder ID="plhEPurseH2" runat="server">
							<h2><asp:Literal ID="ltlEPurseH2" runat="server" /></h2>
						</asp:PlaceHolder>
						<asp:PlaceHolder ID="plhEPurseMessage" runat="server">
							<p class="message"><asp:Literal ID="ltlEPurseMessage" runat="server" /></p>
						</asp:PlaceHolder>
						<fieldset>
							<legend><asp:Literal ID="ltlEPurseLegend" runat="server" /></legend>
							<Talent:EPurse ID="uscEPurse" runat="server" ViewStateMode="Enabled" />
							<asp:PlaceHolder ID="plhEPPartPmt" runat="server">
								<div class="row ebiz-epurse-part-payment">
									<div class="medium-3 columns">
										<asp:Label ID="lblEPPartPmt" runat="server" AssociatedControlID="txtEPPartPmt" CssClass="middle" />
									</div>
									<div class="medium-9 columns">
										<asp:TextBox ID="txtEPPartPmt" runat="server" MaxLength="30" />
									</div>
								</div>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhEPurseTAndC" runat="server">
								<div class="ebiz-terms-and-conditions-wrap ebiz-checkbox-row-wrap">
									<asp:CheckBox ID="chkEPurseTAndC" runat="server" CausesValidation="true" ValidationGroup="Checkout" ViewStateMode="Enabled" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" />
									<asp:Label ID="lblEPurseTAndC" runat="server" AssociatedControlID="chkEPurseTAndC" />
									<asp:CustomValidator ID="csvEPurseTAndC" runat="server" ValidationGroup="Checkout" OnServerValidate="ValidateTerms" ClientValidationFunction="ValidateTerms" EnableClientScript="true" Display="Static" CssClass="error" SetFocusOnError="true" />
								</div>
							</asp:PlaceHolder>
							<asp:Button ID="btnConfirmEPursePayment" runat="server" CssClass="button" CausesValidation="true" ValidationGroup="Checkout" ViewStateMode="Enabled" />
						</fieldset>
					</div>
				</asp:PlaceHolder>

                <!-- Invoice -->
                <asp:PlaceHolder ID="plhInvoice" runat="server">
                    <div class="panel header checkout-invoice">
                        <div class="row">
                            <div class="ebiz-checkout-option-header-wrap columns">
                                <asp:PlaceHolder ID="plhInvoiceTitle" runat="server">
                                    <h2><asp:Literal ID="ltlInvoiceTitle" runat="server"/></h2>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plhInvoiceSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlInvoiceSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
                            </div>
                            <div class="columns ebiz-sequence">
                                <asp:PlaceHolder ID="plhInvoiceNumber" runat="server">
								   <asp:Literal ID="ltlInvoiceNumber" runat="server" />
								</asp:PlaceHolder>
                            </div>
                        </div>
                    </div>
                    <div class="panel ebiz-content checkout-invoice">
                        <fieldset>
                            <legend><asp:Literal ID="ltlInvoiceLegend" runat="server" /></legend>
                            <asp:PlaceHolder ID="plhInvoiceMessage" runat="server">
                                <p class="message"><asp:Literal ID="ltlInvoiceMessage" runat="server" /></p>                   
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhInvoiceTAndC" runat="server">
                                <div class="ebiz-terms-and-conditions-wrap ebiz-checkbox-row-wrap">
                                    <asp:CheckBox ID="chkInvoiceTAndC" runat="server" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" CausesValidation="true" ValidationGroup="Checkout" />
					                <asp:Label ID="lblInvoiceTAndC" runat="server" AssociatedControlID="chkInvoiceTAndC" CssClass="tandc-label" />
                                    <asp:CustomValidator ID="csvInvoice" runat="server" ValidationGroup="Checkout" OnServerValidate="ValidateTerms" ClientValidationFunction="ValidateTerms" EnableClientScript="true"
						                Display="Static" CssClass="error" SetFocusOnError="true" />
                                </div>
                            </asp:PlaceHolder>
                            <div class="ebiz-confirm-invoice-payment-wrap">
                                <asp:Button ID="btnConfirmInvoicePayment" runat="server" CssClass="button" CausesValidation="true" ValidationGroup="Checkout" ViewStateMode="Enabled" />   
                            </div>
                        </fieldset>
                    </div>
                </asp:PlaceHolder>

                <!-- Credit Finance -->
				<asp:PlaceHolder ID="plhCreditFinance" runat="server">
					<div class="panel header checkout-credit-finance">
						  <div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhCreditFinanceTitle" runat="server">
									<h2><asp:Literal ID="ltlCreditFinanceTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhCreditFinanceSubTitle" runat="server">
									  <p class="ebiz-subheader"><asp:Literal ID="ltlCreditFinanceSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhCreditFinanceNumber" runat="server">
								   <asp:Literal ID="ltlCreditFinanceNumber" runat="server" />
								</asp:PlaceHolder>
							 </div> 
						 </div>
					</div>
					<div class="panel ebiz-content checkout-credit-finance">
						<asp:Panel ID="pnlCreditFinanceWrapper" runat="server" CssClass="credit-finance-wrapper" DefaultButton="btnConfirmCFPayment">
							<asp:PlaceHolder ID="plhCreditFinanceWrapper" runat="server">
								<asp:PlaceHolder ID="plhCreditFinanceH2" runat="server">
									<h2><asp:Literal ID="ltlCreditFinanceH2" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhCreditFinanceMessage" runat="server">
									<p class="message"><asp:Literal ID="ltlCreditFinanceMessage" runat="server" /></p>
								</asp:PlaceHolder>
								<fieldset>
									<legend><asp:Literal ID="ltlCreditFinanceLegend" runat="server" /></legend>
									<Talent:CreditFinance ID="uscCreditFinance" runat="server" />
									<div class="terms-and-contitions checkbox">
										<asp:PlaceHolder ID="plhCFTAndC" runat="server">
											<asp:CustomValidator ID="csvCFTAndC" runat="server" ValidationGroup="Checkout" OnServerValidate="ValidateTerms"
												ClientValidationFunction="ValidateTerms" EnableClientScript="true" Display="Static"
												CssClass="error" SetFocusOnError="true" />
											<asp:CheckBox ID="chkCFTAndC" runat="server" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" CausesValidation="true" ValidationGroup="Checkout" />
											<asp:Label ID="lblCFTAndC" runat="server" AssociatedControlID="chkCFTAndC" />
										</asp:PlaceHolder>
										<asp:Button ID="btnConfirmCFPayment" runat="server" CssClass="confirm button" CausesValidation="true" ValidationGroup="Checkout" />
									</div>
								</fieldset>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhCreditFinanceMandateWrapper" runat="server" Visible="false">
								<ul class="credit-finance-mandate-buttons">
									<Talent:CreditFinanceMandate ID="uscCreditFinanceMandate" runat="server" />
									<li class="cancel-button">
										<asp:Button ID="btnCancelCFMandate" runat="server" CssClass="confirm button" CausesValidation="true" ValidationGroup="Checkout" />
									</li>
									<li class="confirm-button">
										<asp:Button ID="btnConfirmCFMandate" runat="server" CssClass="confirm button" CausesValidation="true" ValidationGroup="Checkout" />
									</li>
								</ul>
							</asp:PlaceHolder>
						</asp:Panel>
					</div>
				</asp:PlaceHolder>

				<!-- Cash -->
				<asp:PlaceHolder ID="plhCash" runat="server">
					<div class="panel header ebiz-checkout-cash">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhCashTitle" runat="server">
									<h2><asp:Literal ID="ltlCashTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhCashSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlCashSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhCashNumber" runat="server">
									<asp:Literal ID="ltlCashNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-cash">
						<fieldset>
							<legend><asp:Literal ID="ltlCashLegend" runat="server" /></legend>
							<asp:PlaceHolder ID="plhCashMessage" runat="server">
								<p class="message"><asp:Literal ID="ltlCashMessage" runat="server" /></p>
							</asp:PlaceHolder>

							<Talent:CheckoutPartPayments ID="uscCashPartPayment" runat="server" />    
							
							<asp:PlaceHolder ID="plhCashTAndC" runat="server">
								<div class="ebiz-terms-and-conditions-wrap ebiz-checkbox-row-wrap">
									<asp:CheckBox ID="chkCashTAndC" runat="server" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" CausesValidation="true" ValidationGroup="Checkout" />
									<asp:Label ID="lblCashTAndC" runat="server" AssociatedControlID="chkCashTAndC" CssClass="tandc-label" />
									<asp:CustomValidator ID="csvCashTAndC" runat="server" ValidationGroup="Checkout"
										OnServerValidate="ValidateTerms" ClientValidationFunction="ValidateTerms" EnableClientScript="true"
										Display="Static" CssClass="error" SetFocusOnError="true" />
								</div>
							</asp:PlaceHolder>
							<div class="ebiz-confirm-cash-payment-wrap">
								<asp:Button ID="btnConfirmCashPayment" runat="server" CssClass="button" CausesValidation="true" ValidationGroup="Checkout" />
							</div>
						</fieldset>
					</div>
				</asp:PlaceHolder>

				<!-- Other Payment Types -->
				<asp:PlaceHolder ID="plhOthers" runat="server">
					<div class="panel header ebiz-checkout-others">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhOthersTitle" runat="server">
									<h2><asp:Literal ID="ltlOthersTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhOthersSubtitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlOthersSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhOthersNumber" runat="server">
									<asp:Literal ID="ltlOthersNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-others">
						<fieldset>
							<legend><asp:Literal ID="ltlOthersLegend" runat="server" /></legend>
							<asp:PlaceHolder ID="plhOthersMessage" runat="server">
								<p class="message"><asp:Literal ID="ltlOthersMessage" runat="server" /></p>
							</asp:PlaceHolder>
							<div class="row ebiz-others-type">
								<div class="large-3 columns">
									<asp:Label ID="lblSelectOthersType" runat="server" AssociatedControlID="ddlOthersType" />
								</div>
								<div class="large-9 columns">
									<asp:DropDownList ID="ddlOthersType" runat="server" />
								</div>
							</div>
							<div class="row ebiz-others-value">
								<div class="large-3 columns">
									<asp:Label ID="lblOthersConfigurableValue" runat="server" AssociatedControlID="txtOthersConfigurableValue" CssClass="middle" />
								</div>
								<div class="large-9 columns">
									<asp:TextBox ID="txtOthersConfigurableValue" runat="server" />
									<span id="spOthersMandatoryError" class="error" style="display:none;"></span>
									<asp:HiddenField ID="hdnOthers" runat="server" />
									<asp:HiddenField ID="hdnOthersErrorMessage" runat="server" />
								</div>
							</div>

							<Talent:CheckoutPartPayments ID="uscOthersPartPayment" runat="server" /> 

							<asp:PlaceHolder ID="plhOthersTAndC" runat="server">
								<div class="ebiz-terms-and-conditions-wrap ebiz-checkbox-row-wrap">
									<asp:CheckBox ID="chkOthersTAndC" runat="server" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" CausesValidation="true" ValidationGroup="Checkout" />
									<asp:Label ID="lblOthersTAndC" runat="server" AssociatedControlID="chkOthersTAndC" CssClass="tandc-label" />
									<asp:CustomValidator ID="csvOthersTAndC" runat="server" ValidationGroup="Checkout"
										OnServerValidate="ValidateTerms" ClientValidationFunction="ValidateTerms" EnableClientScript="true"
										Display="Static" CssClass="error" SetFocusOnError="true" />
								</div>
							</asp:PlaceHolder>
							<asp:Button ID="btnConfirmOthersPayment" runat="server" CssClass="confirm button" CausesValidation="true" ValidationGroup="Checkout" />
						</fieldset>
					</div>
				</asp:PlaceHolder>
					
				<!-- On Account -->
				<asp:PlaceHolder ID="plhOnAccount" runat="server">
					<div class="panel header ebiz-onaccount">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhOnAccountTitle" runat="server">
									<h2><asp:Literal ID="ltlOnAccountTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhOnAccountSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlOnAccountSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhOnAccountNumber" runat="server">
									<asp:Literal ID="ltlOnAccountNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>  
					</div>
					<div class="panel ebiz-content ebiz-onaccount">                     
						<asp:PlaceHolder ID="plhOnAccountH2" runat="server">
							<h2><asp:Literal ID="ltlOnAccountH2" runat="server" /></h2>
						</asp:PlaceHolder>
						<asp:PlaceHolder ID="plhOnAccountMessage" runat="server">
							<p class="ebiz-message"><asp:Literal ID="ltlOnAccountMessage" runat="server" /></p>
						</asp:PlaceHolder>
						<fieldset>
							<legend><asp:Literal ID="ltlOnAccountLegend" runat="server" /></legend>
								<asp:PlaceHolder ID="plhOnAccountTAndC" runat="server">
									<div class="ebiz-terms-and-conditions-wrap ebiz-checkbox-row-wrap">
										<asp:CheckBox ID="chkOnAccountTAndC" runat="server" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" CausesValidation="true" ValidationGroup="Checkout" />
										<asp:Label ID="lblOnAccountTAndC" runat="server" AssociatedControlID="chkOnAccountTAndC" CssClass="tandc-label" />
										<asp:CustomValidator ID="csvOnAccountTAndC" runat="server" ValidationGroup="Checkout" OnServerValidate="ValidateTerms" ClientValidationFunction="ValidateTerms" EnableClientScript="true" Display="Static" CssClass="error" SetFocusOnError="true" />
									</div>
								</asp:PlaceHolder>
								<asp:Button ID="btnConfirmOnAccountPayment" runat="server" CssClass="confirm button" CausesValidation="true" ValidationGroup="Checkout" />
						</fieldset>
					</div>
				</asp:PlaceHolder>
					
				<!-- Chip and Pin -->
				<asp:PlaceHolder ID="plhChipAndPin" runat="server">
					<div class="panel header ebiz-checkout-chip-and-pin">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhChipAndPinTitle" runat="server">
									<h2><asp:Literal ID="ltlChipAndPinTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhChipAndPinSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlChipAndPinSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhChipAndPinNumber" runat="server">
									<asp:Literal ID="ltlChipAndPinNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-chip-and-pin">
						<fieldset>
							<legend><asp:Literal ID="ltlChipAndPinLegend" runat="server" /></legend>
							<asp:PlaceHolder ID="plhChipAndPinMessage" runat="server">
								<p class="message"><asp:Literal ID="ltlChipAndPinMessage" runat="server" /></p>
							</asp:PlaceHolder>
							<div class="row ebiz-chip-and-pin-devices">
								<div class="medium-3 columns">
									<asp:Label ID="lblChipAndPinDevices" runat="server" AssociatedControlID="ddlChipAndPinDevices" />
								</div>
								<div class="medium-9 columns">
									<asp:DropDownList ID="ddlChipAndPinDevices" runat="server" />
								</div>
							</div>
							<Talent:CheckoutPartPayments ID="uscChipAndPinPartPayment"   runat="server" />      
							
							
							<asp:PlaceHolder ID="plhChipAndPinTAndC" runat="server">
								<div class="ebiz-terms-and-conditions-wrap ebiz-checkbox-row-wrap">
									<asp:CheckBox ID="chkChipAndPinTAndC" runat="server" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" CausesValidation="true" ValidationGroup="Checkout" />
									<asp:Label ID="lblChipAndPinTAndC" runat="server" AssociatedControlID="chkChipAndPinTAndC" />
									<asp:CustomValidator ID="csvChipAndPin" runat="server" ValidationGroup="Checkout"
										OnServerValidate="ValidateTerms" ClientValidationFunction="ValidateTerms" EnableClientScript="true"
										Display="Static" CssClass="error" SetFocusOnError="true" />
								</div>
							</asp:PlaceHolder>
							<div class="ebiz-button-confirm-chip-and-pin-payment-wrap ebiz-submit-button-wrap">
								<asp:Button ID="btnConfirmChipAndPinPayment" runat="server" CssClass="button" CausesValidation="true" ValidationGroup="Checkout" />
							</div>
						</fieldset>
					</div>
				</asp:PlaceHolder>

				<!-- zero Priced Basket -->
				<asp:PlaceHolder ID="plhZeroPricedBasket" runat="server">
					<div class="panel header ebiz-checkout-zero-basket">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhZeroPricedBasketTitle" runat="server">
									<h2><asp:Literal ID="ltlZeroPricedBasketTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhZeroPricedBasketSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlZeroPricedBasketSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhZeroPricedBasketNumber" runat="server">
									<asp:Literal ID="ltlZeroPricedBasketNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-zero-basket">
						<fieldset>
							<legend><asp:Literal ID="ltlZeroPricedBasketLegend" runat="server" /></legend>
							<asp:PlaceHolder ID="plhZeroPricedBasketMessage" runat="server">
								<p class="message"><asp:Literal ID="ltlZeroPricedBasketMessage" runat="server" /></p>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhZeroPricedBasketTAndC" runat="server">
								<div class="ebiz-terms-and-conditions-wrap ebiz-checkbox-row-wrap">
									<asp:CheckBox ID="chkZeroPricedBasketTAndC" runat="server" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" CausesValidation="true" ValidationGroup="Checkout" />
									<asp:Label ID="lblZeroPricedBasketTAndC" runat="server" AssociatedControlID="chkZeroPricedBasketTAndC" CssClass="tandc-label" />
									<asp:CustomValidator ID="csvZeroPricedBasketTAndC" runat="server" ValidationGroup="Checkout"
										OnServerValidate="ValidateTerms" ClientValidationFunction="ValidateTerms" EnableClientScript="true"
										Display="Static" CssClass="error" SetFocusOnError="true" />
								</div>
							</asp:PlaceHolder>
							<asp:Button ID="btnZeroPricedBasketPayment" runat="server" CssClass="confirm button" CausesValidation="true" ValidationGroup="Checkout" />
						</fieldset>
					</div>
				</asp:PlaceHolder>

				<!-- Pont Of Sale -->
				<asp:PlaceHolder ID="plhPointOfSale" runat="server">
					<div class="panel header ebiz-checkout-point-of-sale">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhPointOfSaleTitle" runat="server">
									<h2><asp:Literal ID="ltlPointOfSaleTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhPointOfSaleSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlPointOfSaleSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhPointOfSaleNumber" runat="server">
									<asp:Literal ID="ltlPointOfSaleNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-point-of-sale">
						<fieldset>
							<legend><asp:Literal ID="ltlPointOfSaleLegend" runat="server" /></legend>
							<asp:PlaceHolder ID="plhPointOfSaleMessage" runat="server">
								<p class="message"><asp:Literal ID="ltlPointOfSaleMessage" runat="server" /></p>
							</asp:PlaceHolder>
							<div class="row ebiz-point-of-sale-terminals">
								<div class="large-3 columns">
									<asp:Label ID="lblPointOfSaleTerminals" runat="server" AssociatedControlID="ddlPointOfSaleTerminals" CssClass="middle" />
								</div>
								<div class="large-9 columns">
									<asp:DropDownList ID="ddlPointOfSaleTerminals" runat="server" />
								</div>
							</div>
							<asp:PlaceHolder ID="plhPointOfSaleTAndC" runat="server">
								<div class="ebiz-terms-and-conditions-wrap ebiz-checkbox-row-wrap">
									<asp:CheckBox ID="chkPointOfSaleTAndC" runat="server" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" CausesValidation="true" ValidationGroup="Checkout" />
									<asp:Label ID="lblPointOfSaleTAndC" runat="server" AssociatedControlID="chkPointOfSaleTAndC" />
									<asp:CustomValidator ID="csvPointOfSale" runat="server" ValidationGroup="Checkout"
										OnServerValidate="ValidateTerms" ClientValidationFunction="ValidateTerms" EnableClientScript="true"
										Display="Static" CssClass="error" SetFocusOnError="true" />
								</div>
							</asp:PlaceHolder>
							<asp:Button ID="btnConfirmPointOfSalePayment" runat="server" CssClass="confirm button" CausesValidation="true" ValidationGroup="Checkout" />
						</fieldset>
					</div>
				</asp:PlaceHolder>

				<!-- PDQ -->
				<asp:PlaceHolder ID="plhPDQ" runat="server">
					<div class="panel header ebiz-checkout-pdq">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhPDQTitle" runat="server">
									<h2><asp:Literal ID="ltlPDQTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhPDQSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlPDQSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhPDQNumber" runat="server">
									<asp:Literal ID="ltlPDQNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-point-of-sale">
						<fieldset>
							<legend><asp:Literal ID="ltlPDQLegend" runat="server" /></legend>
							<asp:PlaceHolder ID="plhPDQMessage" runat="server">
								<p class="message"><asp:Literal ID="ltlPDQMessage" runat="server" /></p>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPDQH2" runat="server">
								<h2><asp:Literal ID="ltlPDQH2" runat="server" /></h2>
							</asp:PlaceHolder>                             
							 <Talent:CheckoutPartPayments ID="uscPDQPartPayment"   runat="server" />                                   
							<asp:PlaceHolder ID="plhPDQTAndC" runat="server">                                
								<div class="terms-and-contitions checkbox">
									<asp:CheckBox ID="chkPDQTAndC" runat="server" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" CausesValidation="true" ValidationGroup="Checkout" />
									<asp:Label ID="lblPDQTAndC" runat="server" AssociatedControlID="chkPDQTAndC" />
									<asp:CustomValidator ID="csvPDQTAndC" runat="server" ValidationGroup="Checkout"
										OnServerValidate="ValidateTerms" ClientValidationFunction="ValidateTerms" EnableClientScript="true"
										Display="Static" CssClass="error" SetFocusOnError="true" />
								</div>
							</asp:PlaceHolder>
							<asp:Button ID="btnConfirmPDQPayment" runat="server" CssClass="confirm button" CausesValidation="true" ValidationGroup="Checkout" />
						</fieldset>
					</div>
				</asp:PlaceHolder>

				<!-- Google Checkout -->
				<asp:PlaceHolder ID="plhGoogleCheckout" runat="server">
					<div class="panel header ebiz-checkout-google">
						<div class="row">
							<div class="ebiz-checkout-option-header-wrap columns">
								<asp:PlaceHolder ID="plhGoogleCheckoutTitle" runat="server">
									<h2><asp:Literal ID="ltlGoogleCheckoutTitle" runat="server" /></h2>
								</asp:PlaceHolder>
								<asp:PlaceHolder ID="plhGoogleCheckoutSubTitle" runat="server">
									<p class="ebiz-subheader"><asp:Literal ID="ltlGoogleCheckoutSubTitle" runat="server" /></p>
								</asp:PlaceHolder>
							</div>
							<div class="columns ebiz-sequence">
								<asp:PlaceHolder ID="plhGoogleCheckoutNumber" runat="server">
									<asp:Literal ID="ltlGoogleCheckoutNumber" runat="server" />
								</asp:PlaceHolder>
							</div>
						</div>
					</div>
					<div class="panel ebiz-content ebiz-checkout-google">
						<fieldset>
							<legend><asp:Literal ID="ltlGoogleCheckoutLegend" runat="server" /></legend>
							<asp:PlaceHolder ID="plhGoogleCheckoutMessage" runat="server">
								<p class="message"><asp:Literal ID="ltlGoogleCheckoutMessage" runat="server" /></p>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhGoogleCheckoutTAndC" runat="server">
								<div class="terms-and-contitions checkbox">
									<asp:CheckBox ID="chkGoogleCheckoutTAndC" runat="server" CssClass="ebiz-tandc ebiz-checkbox-label-wrap" CausesValidation="true" ValidationGroup="Checkout" />
									<asp:Label ID="lblGoogleCheckoutTAndC" runat="server" AssociatedControlID="chkGoogleCheckoutTAndC" />
									<asp:CustomValidator ID="csvGoogleCheckoutTAndC" runat="server" ValidationGroup="Checkout"
										OnServerValidate="ValidateTerms" ClientValidationFunction="ValidateTerms" EnableClientScript="true"
										Display="Static" CssClass="error" SetFocusOnError="true" />
								</div>
							</asp:PlaceHolder>
							<asp:ImageButton ID="imgBtnConfirmGoogleCheckoutPayment" runat="server" SkinID="GoogleCheckoutCheckout" CausesValidation="true" ValidationGroup="Checkout" />
						</fieldset>
					</div>
				</asp:PlaceHolder>
					
				<asp:UpdateProgress ID="updProgressCheckout" AssociatedUpdatePanelID="updCheckout" runat="server" DisplayAfter="0">
					<ProgressTemplate>
						<div class="ebiz-loading-option">
							<span><%=LoadingText %></span>
						</div>
					</ProgressTemplate>
				</asp:UpdateProgress>
			</div>

			<div class="large-3 columns ebiz-checkout-sidebar">
				<Talent:SessionStatus ID="uscSessionStatus" runat="server" />
				<asp:PlaceHolder ID="plhPromotions" runat="server">
					<asp:Panel ID="pnlPromotions" runat="server" CssClass="panel ebiz-promotions" DefaultButton="btnPromotions">
						<h2><asp:Label ID="lblPromotions" runat="server" /></h2>
						<div class="input-group">
						<asp:TextBox ID="txtPromotions" runat="server" MaxLength="30" CssClass="input-group-field" />
						<div class="input-group-button">
							<asp:Button ID="btnPromotions" runat="server" ValidationGroup="Promotions" CssClass="button" />
						</div>
						</div>
						<asp:RequiredFieldValidator ID="rfvPromotions" Display="Static" runat="server" ControlToValidate="txtPromotions" SetFocusOnError="true" SkinID="Promotions" ValidationGroup="Promotions" CssClass="error" />
					</asp:Panel>
				</asp:PlaceHolder>
				<Talent:BasketFees ID="uscBasketFees" runat="server" />
				<Talent:BasketSummary ID="uscBasketSummary" runat="server" RemovePartPaymentAllowed="True" />
			</div>
		</div>

		
		 <Talent:BasketDetails ID="uscBasketDetails" runat="server" Usage="PAYMENT" DisplaySummaryTotals="False" />
			

		</ContentTemplate>
	</asp:UpdatePanel>
<Talent:HTMLInclude ID="uscHTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:content>
