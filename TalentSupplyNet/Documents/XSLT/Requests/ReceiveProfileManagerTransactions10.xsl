<?xml version='1.0' ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes" />
  <xsl:template match="/">
  <TransactionInterface>
    <Version>1.0</Version>
    <TransactionHeader>

      <SenderID></SenderID>
      <ReceiverID></ReceiverID>
      <CountryCode>UK</CountryCode>
      <LoginID></LoginID>
      <Password></Password>
      <Company></Company>
      <TransactionID></TransactionID>
    </TransactionHeader>
    <xsl:copy-of select="TransactionInterface/ListOfTransactions" />

  </TransactionInterface>

  </xsl:template>
</xsl:stylesheet>
