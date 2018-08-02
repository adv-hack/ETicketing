<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="no" />
  <xsl:template match="/">
    <xsl:for-each select="OrderRequest">
      <OrderRequest>
        <Version>
          <xsl:value-of select="Version" />
        </Version>
        <TransactionHeader>
          <SenderID>
            <xsl:value-of select="TransactionHeader/SenderID" />
          </SenderID>
          <ReceiverID>
            <xsl:value-of select="TransactionHeader/ReceiverID" />
          </ReceiverID>
          <CountryCode>
            <xsl:value-of select="TransactionHeader/CountryCode" />
          </CountryCode>
          <LoginID>
            <xsl:value-of select="TransactionHeader/LoginID" />
          </LoginID>
          <Password>
            <xsl:value-of select="TransactionHeader/Password" />
          </Password>
          <Company>
            <xsl:value-of select="TransactionHeader/Company" />
          </Company>
          <TransactionID>
            <xsl:value-of select="TransactionHeader/TransactionID" />
          </TransactionID>
        </TransactionHeader>
        <OrderHeaderInformation>
          <BillToSuffix>
            <xsl:value-of select="OrderHeaderInformation/BillToSuffix" />
          </BillToSuffix>
          <AddressingInformation>
            <CustomerPO>
              <xsl:value-of select="OrderHeaderInformation/AddressingInformation/CustomerPO" />
            </CustomerPO>
            <ShipToAttention>
              <xsl:value-of select="OrderHeaderInformation/AddressingInformation/ShipToAttention" />
            </ShipToAttention>
            <EndUserPO>
              <xsl:value-of select="OrderHeaderInformation/AddressingInformation/EndUserPO" />
            </EndUserPO>
            <ShipTo>
              <Address>
                <ShipToAddress1>
                  <xsl:value-of select="OrderHeaderInformation/AddressingInformation/ShipTo/Address/ShipToAddress1" />
                </ShipToAddress1>
                <ShipToAddress2>
                  <xsl:value-of select="OrderHeaderInformation/AddressingInformation/ShipTo/Address/ShipToAddress2" />
                </ShipToAddress2>
                <ShipToAddress3>
                  <xsl:value-of select="OrderHeaderInformation/AddressingInformation/ShipTo/Address/ShipToAddress3" />
                </ShipToAddress3>
                <ShipToCity>
                  <xsl:value-of select="OrderHeaderInformation/AddressingInformation/ShipTo/Address/ShipToCity" />
                </ShipToCity>
                <ShipToProvince>
                  <xsl:value-of select="OrderHeaderInformation/AddressingInformation/ShipTo/Address/ShipToProvince" />
                </ShipToProvince>
                <ShipToPostalCode>
                  <xsl:value-of select="OrderHeaderInformation/AddressingInformation/ShipTo/Address/ShipToPostalCode" />
                </ShipToPostalCode>
              </Address>
            </ShipTo>
          </AddressingInformation>
          <ProcessingOptions>
            <CarrierCode>
              <xsl:value-of select="OrderHeaderInformation/ProcessingOptions/CarrierCode" />
            </CarrierCode>
            <AutoRelease>
              <xsl:value-of select="OrderHeaderInformation/ProcessingOptions/AutoRelease" />
            </AutoRelease>
            <ShipmentOptions>
              <BackOrderFlag>
                <xsl:value-of select="OrderHeaderInformation/ProcessingOptions/ShipmentOptions/BackOrderFlag" />
              </BackOrderFlag>
              <SplitShipmentFlag>
                <xsl:value-of select="OrderHeaderInformation/ProcessingOptions/ShipmentOptions/SplitShipmentFlag" />
              </SplitShipmentFlag>
              <SplitLine>
                <xsl:value-of select="OrderHeaderInformation/ProcessingOptions/ShipmentOptions/SplitLine" />
              </SplitLine>
              <ShipFromBranches>
                <xsl:value-of select="OrderHeaderInformation/ProcessingOptions/ShipmentOptions/ShipFromBranches" />
              </ShipFromBranches>
            </ShipmentOptions>
          </ProcessingOptions>
          <DynamicMessage>
            <MessageLines>
              <xsl:value-of select="OrderHeaderInformation/DynamicMessage/MessageLines" />
            </MessageLines>
          </DynamicMessage>
        </OrderHeaderInformation>
        <OrderLineInformation>
          <ProductLine>
            <SKU>
              <xsl:value-of select="OrderLineInformation/ProductLine/SKU" />
            </SKU>
            <Quantity>
              <xsl:value-of select="OrderLineInformation/ProductLine/Quantity" />
            </Quantity>
            <CustomerLineNumber>
              <xsl:value-of select="OrderLineInformation/ProductLine/CustomerLineNumber" />
            </CustomerLineNumber>
            <ReservedInventory>
              <ReserveCode>
                <xsl:value-of select="OrderLineInformation/ProductLine/ReservedInventory/ReserveCode" />
              </ReserveCode>
              <ReserveSequence>
                <xsl:value-of select="OrderLineInformation/ProductLine/ReservedInventory/ReserveSequence" />
              </ReserveSequence>
            </ReservedInventory>
          </ProductLine>
          <CommentLine>
            <CommentText>
              <xsl:value-of select="OrderLineInformation/CommentLine/CommentText" />
            </CommentText>
          </CommentLine>
        </OrderLineInformation>
        <ShowDetail>
          <xsl:value-of select="ShowDetail" />
        </ShowDetail>
      </OrderRequest>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>