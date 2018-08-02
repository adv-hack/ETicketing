<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0"  xmlns:xsl="http://www.w3.org/1999/XSL/Transform"  xmlns:fo="http://www.w3.org/1999/XSL/Format">
  <xsl:output method="html"/>

  <xsl:template match="/">
	  <html>
	  <head>
	  <title>Season Ticket Renewals Report</title>
	  </head>
	  <link rel="stylesheet" type="text/css" media="screen" href="Supplynet_STRReport.css" />
	  <body> 

	  	<div class="Supplynet_STRReport_title">Season Ticket Renewal Report</div>
		<hr/>
	  	<div class="Supplynet_STRReport_summary">  	
			<table cellspacing="0" >	
				<tr ><td>Date:</td><td><xsl:value-of select="SeasonTicketRenewalsResponse/TransactionHeader/TimeStamp"/></td></tr>	
				<tr ><td>Transaction ID:</td><td><xsl:value-of select="SeasonTicketRenewalsResponse/TransactionHeader/TransactionID"/></td></tr>	
				<xsl:if test="SeasonTicketRenewalsResponse/TransactionHeader/ErrorStatus!=''">
					<tr ><td>Transaction Return Code:</td><td><xsl:value-of select="SeasonTicketRenewalsResponse/TransactionHeader/ErrorStatus/@ErrorNumber"/> - <xsl:value-of select="SeasonTicketRenewalsResponse/TransactionHeader/ErrorStatus"/></td></tr>	
				</xsl:if>
				<xsl:if test="SeasonTicketRenewalsResponse/TransactionHeader/ErrorStatus=''">
					<tr ><td>FailuresCount:</td><td><xsl:value-of select="SeasonTicketRenewalsResponse/TransactionDetails/FailureCount"/></td></tr>	
					<tr ><td>SuccessCount:</td><td><xsl:value-of select="SeasonTicketRenewalsResponse/TransactionDetails/SuccessCount"/></td></tr>
				</xsl:if>
		    	</table>
		</div>
		
		<xsl:if test="SeasonTicketRenewalsResponse/TransactionHeader/ErrorStatus=''">	

			<hr/>
			<div class="Supplynet_STRReport_failed_title">Failed Transactions</div>
			<div class="Supplynet_STRReport_failed_report">
				<table cellspacing="0"  >	
					<tr >
						<td>Customer Number</td>
						<td>Seat</td>
						<td>SessionID</td>
						<td>Error Code</td>
					</tr>	
					<xsl:for-each select="SeasonTicketRenewalsResponse/TransactionDetails/SeasonTicketRenewalResponse">
						<xsl:if test="Success='False'">
							<tr >
								<td><xsl:value-of select="AddToBasketResponse/CustomerNumber"/></td>
								<td><xsl:value-of select="AddToBasketResponse/Seat"/></td>
								<td><xsl:value-of select="PaymentResponse/SessionID"/></td>
								<td><xsl:value-of select="PaymentResponse/ReturnCode"/></td>
							</tr>
						</xsl:if>
					</xsl:for-each>	
			    	</table>
			</div>
		    
			<hr/>
			<div class="Supplynet_STRReport_success_title">Successful Transactions</div>
		    	<div class="Supplynet_STRReport_success_report">
				<table cellspacing="0" >	
					<tr >
						<td>Customer Number</td>
						<td>Seat</td>
						<td>Payment Reference</td>
						<td>Total Price</td>
					</tr>	
					<xsl:for-each select="SeasonTicketRenewalsResponse/TransactionDetails/SeasonTicketRenewalResponse">
						<xsl:if test="Success='True'">
						<tr >
							<td><xsl:value-of select="AddToBasketResponse/CustomerNumber"/></td>
							<td><xsl:value-of select="AddToBasketResponse/Seat"/></td>
							<td><xsl:value-of select="PaymentResponse/ConfirmedDetails/PaymentReference"/></td>
							<td><xsl:value-of select="PaymentResponse/ConfirmedDetails/TotalPrice"/></td>
						</tr>
						</xsl:if>
					</xsl:for-each>	
			    	</table>
			</div>
	
	    	</xsl:if>
		
		<hr/>

	  </body>
	  </html>
 </xsl:template>

</xsl:stylesheet>
