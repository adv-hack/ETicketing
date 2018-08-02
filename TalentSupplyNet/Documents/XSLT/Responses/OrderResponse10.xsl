<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="no" />
  <xsl:template match="/">
    <xsl:for-each select="OrderResponse">
      <OrderResponse>
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
          <ErrorStatus>
            <xsl:attribute name="ErrorNumber">
              <xsl:value-of select="TransactionHeader/ErrorStatus/@ErrorNumber" />
            </xsl:attribute>
          </ErrorStatus>
          <DocumentID>
            <xsl:value-of select="TransactionHeader/DocumentID" />
          </DocumentID>
          <TransactionID>
            <xsl:value-of select="TransactionHeader/TransactionID" />
          </TransactionID>
          <TimeStamp>
            <xsl:value-of select="TransactionHeader/TimeStamp" />
          </TimeStamp>
        </TransactionHeader>
        <OrderInfo>
          <OrderNumbers>
            <BranchOrderNumber>
              <xsl:value-of select="OrderInfo/OrderNumbers/BranchOrderNumber" />
            </BranchOrderNumber>
            <CustomerPO>
              <xsl:value-of select="OrderInfo/OrderNumbers/CustomerPO" />
            </CustomerPO>
            <ThirdPartyFreight>
              <xsl:value-of select="OrderInfo/OrderNumbers/ThirdPartyFreight" />
            </ThirdPartyFreight>
            <ShipToAttention>
              <xsl:value-of select="OrderInfo/OrderNumbers/ShipToAttention" />
            </ShipToAttention>
            <ShipToAddress1>
              <xsl:value-of select="OrderInfo/OrderNumbers/ShipToAddress1" />
            </ShipToAddress1>
            <ShipToAddress2>
              <xsl:value-of select="OrderInfo/OrderNumbers/ShipToAddress2" />
            </ShipToAddress2>
            <ShipToAddress3>
              <xsl:value-of select="OrderInfo/OrderNumbers/ShipToAddress3" />
            </ShipToAddress3>
            <ShipToCity>
              <xsl:value-of select="OrderInfo/OrderNumbers/ShipToCity" />
            </ShipToCity>
            <ShipToProvince>
              <xsl:value-of select="OrderInfo/OrderNumbers/ShipToProvince" />
            </ShipToProvince>
            <ShipToPostalCode>
              <xsl:value-of select="OrderInfo/OrderNumbers/ShipToPostalCode" />
            </ShipToPostalCode>
            <ShipToSuffix>
              <xsl:value-of select="OrderInfo/OrderNumbers/ShipToSuffix" />
            </ShipToSuffix>
            <AddressErrorMessage>
              <xsl:attribute name="AddressErrorType">
                <xsl:value-of select="OrderInfo/OrderNumbers/AddressErrorMessage/@AddressErrorType" />
              </xsl:attribute>
            </AddressErrorMessage>
            <ContractNumber>
              <xsl:value-of select="OrderInfo/OrderNumbers/ContractNumber" />
            </ContractNumber>
            <OrderSuffix>
              <xsl:attribute name="Suffix">
                <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/@Suffix" />
              </xsl:attribute>
              <DistributionWeight>
                <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/DistributionWeight" />
              </DistributionWeight>
              <SuffixErrorResponse>
                <xsl:attribute name="SuffixErrorType">
                  <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/SuffixErrorResponse/@SuffixErrorType" />
                </xsl:attribute>
              </SuffixErrorResponse>
              <Carrier>
                <xsl:attribute name="CarrierCode">
                  <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/Carrier/@CarrierCode" />
                </xsl:attribute>
                <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/Carrier" />
              </Carrier>
              <LineInformation>
                <ProductLine>
                  <LineError>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/LineError" />
                  </LineError>
                  <SKU>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/SKU" />
                  </SKU>
                  <UnitPrice>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/UnitPrice" />
                  </UnitPrice>
                  <WestCoastLineNumber>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/WestCoastLineNumber" />
                  </WestCoastLineNumber>
                  <CustomerLineNumber>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/CustomerLineNumber" />
                  </CustomerLineNumber>
                  <ShipFromBranch>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/ShipFromBranch" />
                  </ShipFromBranch>
                  <OrderQuantity>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/OrderQuantity" />
                  </OrderQuantity>
                  <AllocatedQuantity>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/AllocatedQuantity" />
                  </AllocatedQuantity>
                  <BackOrderedQuantity>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/BackOrderedQuantity" />
                  </BackOrderedQuantity>
                  <BackOrderETADate>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/BackOrderETADate" />
                  </BackOrderETADate>
                  <PriceDerivedFlag>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/PriceDerivedFlag" />
                  </PriceDerivedFlag>
                  <ForeignCurrency>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/ForeignCurrency" />
                  </ForeignCurrency>
                  <FreightRate>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/FreightRate" />
                  </FreightRate>
                  <TransitDays>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/TransitDays" />
                  </TransitDays>
                  <LineBillToSuffix>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/ProductLine/LineBillToSuffix" />
                  </LineBillToSuffix>
                </ProductLine>
                <CommentLine>
                  <CommentText>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/CommentLine/CommentText" />
                  </CommentText>
                  <CommentLineNumber>
                    <xsl:value-of select="OrderInfo/OrderNumbers/OrderSuffix/LineInformation/CommentLine/CommentLineNumber" />
                  </CommentLineNumber>
                </CommentLine>
              </LineInformation>
            </OrderSuffix>
          </OrderNumbers>
        </OrderInfo>
      </OrderResponse>
    </xsl:for-each>
  </xsl:template>
</xsl:stylesheet>