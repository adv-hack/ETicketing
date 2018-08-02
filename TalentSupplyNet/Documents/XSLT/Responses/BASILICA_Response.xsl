<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns="http://172.16.10.81/TradingPortal">
	<xsl:output method="xml" encoding="utf-8" indent="yes"/>
	<xsl:template match="/">

		<xsl:variable name="year" select="substring-before(OrderResponse/TransactionHeader/TimeStamp,'-')"/>
		<xsl:variable name="month" select="substring-before(substring-after(OrderResponse/TransactionHeader/TimeStamp,'-'),'-')"/>
		<xsl:variable name="day" select="substring-before(substring-after(substring-after(OrderResponse/TransactionHeader/TimeStamp,'-'),'-'),'T')"/>
		<xsl:variable name="hour" select="substring-after(substring-before(OrderResponse/TransactionHeader/TimeStamp,':'),'T')"/>
		<xsl:variable name="minute" select="substring-before(substring-after(substring-after(OrderResponse/TransactionHeader/TimeStamp,'T'),':'),':')"/>
		<xsl:variable name="secs" select="substring-after(substring-after(substring-after(OrderResponse/TransactionHeader/TimeStamp,'T'),':'),':')"/>
		<xsl:variable name="time" select="concat($hour,$minute,$secs)"/>
		<xsl:variable name="date" select="concat($year,'-',$month,'-',$day)"/>
		<xsl:for-each select="OrderResponse/OrderInfo/OrderNumbers">
			<Basilica_POC_1>
				<General>
					<xsl:attribute name="Order_Reference_Number">
						<xsl:value-of select="CustomerPO"/>
					</xsl:attribute>
					<xsl:attribute name="Order_Date">
						<xsl:value-of select="$date"/>
					</xsl:attribute>
					<xsl:attribute name="Document_Creation_Date">
						<xsl:value-of select="$date"/>
					</xsl:attribute>
					<xsl:attribute name="Vendor_Order_Number">
						<xsl:value-of select="BranchOrderNumber"/>
					</xsl:attribute>
					<xsl:attribute name="Status">
						<xsl:for-each select="ErrorStatus">
							<xsl:if test="@ErrorNumber=''">
								<xsl:value-of select="'Accepted'"/>
							</xsl:if>
							<xsl:if test="@ErrorNumber!=''">
								<xsl:value-of select="'Rejected'"/>
							</xsl:if>
						</xsl:for-each>
					</xsl:attribute>
					<Buy_from_Vendor Number="WES001">
						<Name>Westcoast</Name>
					</Buy_from_Vendor>
					<xsl:for-each select="ErrorStatus">
						<xsl:if test="@ErrorNumber=''">
						</xsl:if>
						<xsl:if test="@ErrorNumber='TACDBOR-16'">
							<ControlMsg>
								<Msg>
									<xsl:attribute name="ID">
										<xsl:value-of select="'10'"/>
									</xsl:attribute>
									<xsl:value-of select="'Duplicate Customer PO '"/>
									<xsl:value-of select="../CustomerPO"/>
								</Msg>
							</ControlMsg>
						</xsl:if>
						<xsl:if test="@ErrorNumber='TACDBOR-17'">
							<ControlMsg>
								<Msg>
									<xsl:attribute name="ID">
										<xsl:value-of select="'20'"/>
									</xsl:attribute>
									<xsl:value-of select="'Invalid Item Code '"/>
									<xsl:value-of select="substring-before(substring-after(current(),'('),')')"/>
								</Msg>
							</ControlMsg>
						</xsl:if>
					</xsl:for-each>
				</General>
				<Invoicing>
					<Bill_to_Customer>
						<Bill_to_Name>Basilica Computing Ltd</Bill_to_Name>
						<Bill_to_Address>No1 Avenue One</Bill_to_Address>
						<Bill_to_Additional_Address_Info>Letchworth Business Park</Bill_to_Additional_Address_Info>
						<Bill_to_City>Letchworth</Bill_to_City>
						<Bill_to_Post_Code>SG6 2HB</Bill_to_Post_Code>
						<Bill_to_County>Herts</Bill_to_County>
						<Bill_to_Country>GB</Bill_to_Country>
						<Bill_to_Contact>Accounts Payable</Bill_to_Contact>
						<Currency>GBP</Currency>
					</Bill_to_Customer>
				</Invoicing>
				<Shipping Date_Required="" Line_Fill_Kill="" Part_Shipment="">
					<Ship_to_Customer Ship_Type="">
						<Ship_Reference></Ship_Reference>
						<Ship_to_Name>
							<xsl:value-of select="ShipToAttention"/>
						</Ship_to_Name>
						<Ship_to_Address>
							<xsl:value-of select="ShipToAddress1"/>
						</Ship_to_Address>
						<Ship_to_Additional_Address_Info>
							<xsl:value-of select="ShipToAddress2"/>
						</Ship_to_Additional_Address_Info>
						<Ship_to_City>
							<xsl:value-of select="ShipToAddress3"/>
						</Ship_to_City>
						<Ship_to_Post_Code>
							<xsl:value-of select="ShipToPostalCode"/>
						</Ship_to_Post_Code>
						<Ship_to_County>
							<xsl:value-of select="ShipToProvince"/>
						</Ship_to_County>
						<Ship_to_Country></Ship_to_Country>
						<Ship_to_Contact>
							<xsl:value-of select="ShipToAttention"/>
						</Ship_to_Contact>
					</Ship_to_Customer>
				</Shipping>
				<Company_info>
					<Company_Reference>BAS133</Company_Reference>
					<Company_Name>Basilica Computing Ltd</Company_Name>
				</Company_info>
				<xsl:for-each select="OrderSuffix/LineInformation/ProductLine">
					<Line>
						<Vendor_Line_Number>
							<xsl:value-of select="WestcoastLineNumber"/>
						</Vendor_Line_Number>
						<Expected_Delivery_Date></Expected_Delivery_Date>
						<Unit_of_Measure>EACH</Unit_of_Measure>
						<Quantity>
							<xsl:value-of select="OrderQuantity"/>
						</Quantity>
						<Direct_Unit_Cost>
							<xsl:value-of select="UnitPrice"/>
						</Direct_Unit_Cost>
						<Vendor_Item_Number>
							<xsl:value-of select="SKU"/>
						</Vendor_Item_Number>
						<Common_Item_Number>
							<xsl:value-of select="SKU"/>
						</Common_Item_Number>
						<Common_Line_Number>
							<xsl:value-of select="WestcoastLineNumber*10000"/>
						</Common_Line_Number>
						<Confirmed_Direct_Unit_Cost>
							<xsl:value-of select="UnitPrice"/>
						</Confirmed_Direct_Unit_Cost>
						<Confirmed_Quantity>
							<xsl:value-of select="OrderQuantity"/>
						</Confirmed_Quantity>
						<Available_Quantity></Available_Quantity>
							<Status>
						<xsl:if test="number(AllocatedQuantity + BackOrderedQuantity) = 0">Rejected</xsl:if>
						<xsl:if test="number(AllocatedQuantity + BackOrderedQuantity) > 0">Accepted</xsl:if>
							</Status>
					</Line>
				</xsl:for-each>
			</Basilica_POC_1>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet><!-- Stylus Studio meta-information - (c) 2004-2006. Progress Software Corporation. All rights reserved.
<metaInformation>
<scenarios ><scenario default="yes" name="Scenario1" userelativepaths="yes" externalpreview="no" url="Response\good.xml" htmlbaseurl="" outputurl="" processortype="internal" useresolver="yes" profilemode="0" profiledepth="" profilelength="" urlprofilexml="" commandline="" additionalpath="" additionalclasspath="" postprocessortype="none" postprocesscommandline="" postprocessadditionalpath="" postprocessgeneratedext="" validateoutput="no" validator="internal" customvalidator=""/></scenarios><MapperMetaTag><MapperInfo srcSchemaPathIsRelative="yes" srcSchemaInterpretAsXML="no" destSchemaPath="" destSchemaRoot="" destSchemaPathIsRelative="yes" destSchemaInterpretAsXML="no"/><MapperBlockPosition></MapperBlockPosition><TemplateContext></TemplateContext><MapperFilter side="source"></MapperFilter></MapperMetaTag>
</metaInformation>
-->