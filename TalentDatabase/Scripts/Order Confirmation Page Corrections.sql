-- ===========================================================
-- Change the order confirmation payment details messages
-- ===========================================================

UPDATE [tbl_page_text_lang]
   SET [TEXT_CONTENT] = '<div class="ebiz-ticketing-payment-details">
	<h2 class="subheader"><small>Your Payment Details&hellip;</small></h2>
	<ul class="small-block-grid-1 medium-block-grid-2">
		<li>
			<div class="ebiz-label">
				Your Payment Reference
			</div>
			<div class="ebiz-value">
				<strong><<PAYMENT_REFERENCE>></strong>
			</div>
		</li>
		<li>
			<div class="ebiz-label">
				Your Payment Amount
			</div>
			<div class="ebiz-value">
				<strong><<PAYMENT_AMOUNT>></strong>
			</div>
		</li>
	</ul>
</div>'

WHERE [PAGE_CODE] = 'CheckoutOrderConfirmation.aspx' AND [TEXT_CODE] = 'TicketingPaymentDetails'
UPDATE [tbl_page_text_lang]
   SET [TEXT_CONTENT] = '<div class="ebiz-ticketing-payment-details">
	<h2 class="subheader"><small>Your Payment Details&hellip;</small></h2>
	<ul class="small-block-grid-1 medium-block-grid-2">
		<li>
			<div class="ebiz-label">
				Your Payment Reference
			</div>
			<div class="ebiz-value">
				<strong><<PAYMENT_REFERENCE>></strong>
			</div>
		</li>
		<li>
			<div class="ebiz-label">
				Your Payment Amount
			</div>
			<div class="ebiz-value">
				<strong><<PAYMENT_AMOUNT>></strong>
			</div>
		</li>
	</ul>
</div>'
WHERE [PAGE_CODE] = 'CheckoutOrderConfirmation.aspx' AND [TEXT_CODE] = 'SingleBasketPaymentDetails'

UPDATE [tbl_page_text_lang]
   SET [TEXT_CONTENT] = '<div class="ebiz-merchandise-payment-details">
	<h2 class="subheader"><small>Your Payment Details&hellip;</small></h2>
	<ul class="small-block-grid-1 medium-block-grid-3">
		<li>
			<div class="ebiz-label">
				Your Order Reference
			</div>
			<div class="ebiz-value">
				<strong><<ORDER_REFERENCE>><<BACK_OFFICE_ORDER_ID>></strong>
			</div>
		</li>
		<li>
			<div class="ebiz-label">
				Your Payment Type
			</div>
			<div class="ebiz-value">
				<strong><<PAYMENT_TYPE>></strong>
			</div>
		</li>
		<li>
			<div class="ebiz-label">
				Your Payment Amount
			</div>
			<div class="ebiz-value">
				<strong><<PAYMENT_AMOUNT>></strong>
			</div>
		</li>
	</ul>
</div>'
WHERE [PAGE_CODE] = 'CheckoutOrderConfirmation.aspx' AND [TEXT_CODE] = 'MerchandisePaymentDetails'

UPDATE [tbl_page_text_lang]
   SET [TEXT_CONTENT] = '<div class="ebiz-ticketing-refund-details">
	<h2 class="subheader"><small>Your Refund Details&hellip;</small></h2>
	<ul class="small-block-grid-1 medium-block-grid-3">
		<li>
			<div class="ebiz-label">
				Your Refund Reference
			</div>
			<div class="ebiz-value">
				<strong><<ORDER_REFERENCE>></strong>
			</div>
		</li>
		<li>
			<div class="ebiz-label">
				Your Refund Type
			</div>
			<div class="ebiz-value">
				<strong><<PAYMENT_TYPE>></strong>
			</div>
		</li>
		<li>
			<div class="ebiz-label">
				Your Refund Amount
			</div>
			<div class="ebiz-value">
				<strong><<PAYMENT_AMOUNT>></strong>
			</div>
		</li>
	</ul>
</div>'
WHERE [PAGE_CODE] = 'CheckoutOrderConfirmation.aspx' AND [TEXT_CODE] = 'TicketingRefundDetails'

UPDATE [tbl_page_text_lang]
   SET [TEXT_CONTENT] = '<div class="ebiz-ticketing-payment-message-for-generic-sale">
	<h2 class="subheader"><small>The Payment Has Been Processed Successfully</small></h2>
</div>'
WHERE [PAGE_CODE] = 'CheckoutOrderConfirmation.aspx' AND [TEXT_CODE] = 'TicketingRefundDetails'
 
GO


