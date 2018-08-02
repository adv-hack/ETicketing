<?xml version='1.0' ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" encoding="utf-8" indent="yes"/>
  <xsl:template match="/">
    <xsl:element name="XmlLoadRequest">

      <xsl:variable name="BusinessUnit">SJL</xsl:variable>

      <xsl:for-each select="XmlLoadRequest/ecrminput">

        <!-- Don't do anything if there's no email address.. -->
        <xsl:variable name="n227" select="xml/@id = '227'" />

        <!--<xsl:if test="not($n227)">-->

        <xsl:variable name="ecrmInputID" select="@id" />
        <xsl:element name="ecrminput">
          <xsl:attribute name="id">
            <xsl:value-of select="$ecrmInputID"/>
          </xsl:attribute>

          <!-- Output everything except the address lines -->
          <xsl:for-each select="xml">
            <xsl:if test="not(@id='100' or @id='101' or @id='102' or @id='103' or @id='104' or @id='229')">
              <xsl:variable name="currentValue" select="current()" />
              <xsl:if test="$currentValue != ''">
                <xsl:copy-of select="current()"/>
              </xsl:if>
              <xsl:if test="$currentValue = ''">
                <xsl:element name="xml">
                  <xsl:attribute name="id">
                    <xsl:value-of select="@id"/>
                  </xsl:attribute>
                  <xsl:attribute name="title">
                    <xsl:value-of select="@title"/>
                  </xsl:attribute>
                  <xsl:text> </xsl:text>
                </xsl:element>
              </xsl:if>
            </xsl:if>
          </xsl:for-each>

          <xsl:variable name="Partner" select="xml/@id = '118'" />

          <xsl:variable name="n001" select="xml/@id = '001'" />
          <!-- Customer ID -->
          <xsl:variable name="n002" select="xml/@id = '002'" />
          <!-- Contact ID -->
          <xsl:variable name="n004" select="xml/@id = '004'" />
          <!-- Branch -->

          <xsl:variable name="n100" select="xml/@id = '100'" />
          <!-- Address Line 1 -->
          <xsl:variable name="n101" select="xml/@id = '101'" />
          <!-- Address Line 2 -->
          <xsl:variable name="n102" select="xml/@id = '102'" />
          <!-- Address Line 3 -->
          <xsl:variable name="n103" select="xml/@id = '103'" />
          <!-- Address Line 4 -->
          <xsl:variable name="n104" select="xml/@id = '104'" />
          <!-- Address Line 5 -->
          <xsl:variable name="n105" select="xml/@id = '105'" />
          <!-- Postcode -->

          <xsl:variable name="n200" select="xml/@id = '200'" />
          <!-- Title -->
          <xsl:variable name="n201" select="xml/@id = '201'" />
          <!-- Initials -->
          <xsl:variable name="n202" select="xml/@id = '202'" />
          <!-- Forename -->
          <xsl:variable name="n203" select="xml/@id = '203'" />
          <!-- Surname -->
          <xsl:variable name="n204" select="xml/@id = '204'" />
          <!-- Home Phone -->
          <xsl:variable name="n206" select="xml/@id = '206'" />
          <!-- Work Phone -->
          <xsl:variable name="n208" select="xml/@id = '208'" />
          <!-- Mobile Phone -->
          <xsl:variable name="n210" select="xml/@id = '210'" />
          <!-- Fax -->
          <xsl:variable name="n212" select="xml/@id = '212'" />
          <!-- Other Number -->
          <!--<xsl:variable name="n227" select="xml/@id = '227'" />-->
          <!-- Email Address -->
          <xsl:variable name="n229" select="xml/@id = '229'" />
          <!-- Date Of Birth -->
          <xsl:variable name="n231" select="xml/@id = '231'" />
          <!-- Salutation -->
          <xsl:variable name="n232" select="xml/@id = '232'" />
          <!-- SL No. 1 -->
          <xsl:variable name="n233" select="xml/@id = '233'" />
          <!-- SL No. 2 -->

          <xsl:variable name="n10101" select="xml/@id = '10101'" />
          <!-- Address Type -->
          <xsl:variable name="n10102" select="xml/@id = '10102'" />
          <!-- Address Reference -->
          <xsl:variable name="n10103" select="xml/@id = '10103'" />
          <!-- Address Country -->
          <xsl:variable name="n10104" select="xml/@id = '10104'" />
          <!-- Address Sequence -->
          <xsl:variable name="n10105" select="xml/@id = '10105'" />
          <!-- Default Address Flag -->

          <xsl:variable name="n10201" select="xml/@id = '10201'" />
          <!-- Full Name -->
          <xsl:variable name="n10202" select="xml/@id = '10202'" />
          <!-- Messaging ID -->
          <xsl:variable name="n10203" select="xml/@id = '10203'" />
          <!-- Originating Business Unit -->
          <xsl:variable name="n10204" select="xml/@id = '10204'" />
          <!-- Account No. 3 -->
          <xsl:variable name="n10205" select="xml/@id = '10205'" />
          <!-- Account No. 4 -->
          <xsl:variable name="n10206" select="xml/@id = '10206'" />
          <!-- Account No. 5 -->
          <xsl:variable name="n10207" select="xml/@id = '10207'" />
          <!-- Newsletter Subscription -->
          <xsl:variable name="n10208" select="xml/@id = '10208'" />
          <!-- HTML Newsletter -->
          <xsl:variable name="n10209" select="xml/@id = '10209'" />
          <!-- Additional Boolean 1 -->
          <xsl:variable name="n10210" select="xml/@id = '10210'" />
          <!-- Additional Boolean 2 -->
          <xsl:variable name="n10211" select="xml/@id = '10211'" />
          <!-- Additional Boolean 3 -->
          <xsl:variable name="n10212" select="xml/@id = '10212'" />
          <!-- Additional Boolean 4 -->
          <xsl:variable name="n10213" select="xml/@id = '10213'" />
          <!-- Additional Boolean 5 -->
          <xsl:variable name="n10214" select="xml/@id = '10214'" />
          <!-- Position -->

          <!-- Customer Info  -->
          <xsl:if test="not($n001)">
            <xsl:element name="xml">
              <xsl:attribute name="id">001</xsl:attribute>
              <xsl:attribute name="title">CustomerID</xsl:attribute>
              <xsl:text> </xsl:text>
            </xsl:element>
          </xsl:if>

          <xsl:if test="not($n002)">
            <xsl:element name="xml">
              <xsl:attribute name="id">002</xsl:attribute>
              <xsl:attribute name="title">ContactID</xsl:attribute>
              <xsl:text> </xsl:text>
            </xsl:element>
          </xsl:if>

          <xsl:if test="not($n004)">
            <xsl:element name="xml">
              <xsl:attribute name="id">004</xsl:attribute>
              <xsl:attribute name="title">Branch</xsl:attribute>
              <xsl:text> </xsl:text>
            </xsl:element>
          </xsl:if>

          <!-- Select and Re-Organise the Address Info-->
          <xsl:choose>

            <!-- No Address Specified -->
            <xsl:when test="not($n100) and not($n101) and not($n102) and not($n103) and not($n104)">
              <xsl:element name="xml">
                <xsl:attribute name="id">100</xsl:attribute>
                <xsl:attribute name="title">AddressLine1</xsl:attribute>
                <xsl:text> </xsl:text>
              </xsl:element>
              <xsl:element name="xml">
                <xsl:attribute name="id">101</xsl:attribute>
                <xsl:attribute name="title">AddressLine2</xsl:attribute>
                <xsl:text> </xsl:text>
              </xsl:element>
              <xsl:element name="xml">
                <xsl:attribute name="id">102</xsl:attribute>
                <xsl:attribute name="title">AddressLine3</xsl:attribute>
                <xsl:text> </xsl:text>
              </xsl:element>
              <xsl:element name="xml">
                <xsl:attribute name="id">103</xsl:attribute>
                <xsl:attribute name="title">AddressLine4</xsl:attribute>
                <xsl:text> </xsl:text>
              </xsl:element>
              <xsl:element name="xml">
                <xsl:attribute name="id">104</xsl:attribute>
                <xsl:attribute name="title">AddressLine5</xsl:attribute>
                <xsl:text> </xsl:text>
              </xsl:element>
            </xsl:when>

            <xsl:when test="not($n104)">
              <xsl:call-template name="SortAddress-Line5Blank">
                <xsl:with-param name="n100" select="$n100" />
                <xsl:with-param name="n101" select="$n101" />
                <xsl:with-param name="n102" select="$n102" />
                <xsl:with-param name="n103" select="$n103" />
                <xsl:with-param name="n104" select="$n104" />
              </xsl:call-template>
            </xsl:when>

            <xsl:when test="not($n103)">
              <xsl:call-template name="SortAddress-Line4Blank">
                <xsl:with-param name="n100" select="$n100" />
                <xsl:with-param name="n101" select="$n101" />
                <xsl:with-param name="n102" select="$n102" />
                <xsl:with-param name="n103" select="$n103" />
                <xsl:with-param name="n104" select="$n104" />
              </xsl:call-template>
            </xsl:when>

            <xsl:when test="not($n102)">
              <xsl:call-template name="SortAddress-Line3Blank">
                <xsl:with-param name="n100" select="$n100" />
                <xsl:with-param name="n101" select="$n101" />
                <xsl:with-param name="n102" select="$n102" />
                <xsl:with-param name="n103" select="$n103" />
                <xsl:with-param name="n104" select="$n104" />
              </xsl:call-template>
            </xsl:when>

            <xsl:when test="not($n101)">
              <xsl:call-template name="SortAddress-Line2Blank">
                <xsl:with-param name="n100" select="$n100" />
                <xsl:with-param name="n101" select="$n101" />
                <xsl:with-param name="n102" select="$n102" />
                <xsl:with-param name="n103" select="$n103" />
                <xsl:with-param name="n104" select="$n104" />
              </xsl:call-template>
            </xsl:when>

            <xsl:when test="not($n100)">
              <xsl:call-template name="SortAddress-Line1Blank">
                <xsl:with-param name="n100" select="$n100" />
                <xsl:with-param name="n101" select="$n101" />
                <xsl:with-param name="n102" select="$n102" />
                <xsl:with-param name="n103" select="$n103" />
                <xsl:with-param name="n104" select="$n104" />
              </xsl:call-template>
            </xsl:when>

            <xsl:when test="$n100 and $n101 and $n102 and $n103 and $n104">
              <xsl:call-template name="SortAddress-AllLines">
                <xsl:with-param name="n100" select="$n100" />
                <xsl:with-param name="n101" select="$n101" />
                <xsl:with-param name="n102" select="$n102" />
                <xsl:with-param name="n103" select="$n103" />
                <xsl:with-param name="n104" select="$n104" />
              </xsl:call-template>
            </xsl:when>
          </xsl:choose>

          <xsl:if test="not($n105)">
            <xml id="105" title="PostCode">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>

          <!-- Contact Info -->
          <xsl:if test="not($n200)">
            <xml id="200" title="Title">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n201)">
            <xml id="201" title="Initials">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n202)">
            <xml id="202" title="Surname">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n203)">
            <xml id="203" title="Forename">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n204)">
            <xml id="204" title="TelephoneNumber">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n206)">
            <xml id="206" title="WorkNumber">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n208)">
            <xml id="208" title="MobileNumber">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n210)">
            <xml id="210" title="FaxNumber">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n212)">
            <xml id="212" title="OtherNumber">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n227)">
            <xml id="227" title="EmailAddress">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>

          <xsl:if test="$n229">
            <xml id="229" title="DOB">
              <xsl:for-each select="xml">
                <xsl:if test="@id=229">
                  <xsl:value-of select="substring(current(),5,4)"/>
                  <xsl:value-of select="substring(current(),3,2)"/>
                  <xsl:value-of select="substring(current(),1,2)"/>
                </xsl:if>
              </xsl:for-each>
            </xml>
          </xsl:if>

          <xsl:if test="not($n229)">
            <xml id="229" title="DOB">
              <xsl:text>19000101</xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n231)">
            <xml id="231" title="Salutation">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n232)">
            <xml id="232" title="SL Account No 1">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n233)">
            <xml id="233" title="SL Account No 2">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>

          <!-- Additional Info Needed for Customer DB -->
          <xsl:if test="not($n10101)">
            <xml id="10101" title="AddressType">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10102)">
            <xml id="10102" title="AddressReference">
              <xsl:if test="not($n100)">
                <xsl:for-each select="xml">
                  <xsl:if test="@id=101">
                    <xsl:value-of select="current()"/>
                  </xsl:if>
                </xsl:for-each>
              </xsl:if>
              <xsl:if test="$n100">
                <xsl:for-each select="xml">
                  <xsl:if test="@id=100">
                    <xsl:value-of select="current()"/>
                  </xsl:if>
                </xsl:for-each>
              </xsl:if>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10103)">
            <xml id="10103" title="Country">
              <xsl:for-each select="xml">
                <xsl:if test="@id=104">
                  <xsl:value-of select="current()"/>
                </xsl:if>
              </xsl:for-each>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10104)">
            <xml id="10104" title="AddressSequence">
              <xsl:text>0</xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10105)">
            <xml id="10105" title="DefaultAddress">
              <xsl:text>True</xsl:text>
            </xml>
          </xsl:if>

          <xsl:if test="not($n10201)">
            <xml id="10201" title="FullName">
              <xsl:for-each select="xml">
                <xsl:if test="@id=203">
                  <xsl:value-of select="current()"/>
                </xsl:if>
              </xsl:for-each>
              <xsl:text xml:space="preserve"> </xsl:text>
              <xsl:for-each select="xml">
                <xsl:if test="@id=202">
                  <xsl:value-of select="current()"/>
                </xsl:if>
              </xsl:for-each>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10202)">
            <xml id="10202" title="MessagingID">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10203)">
            <xml id="10203" title="OriginatingBusinessUnit">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10204)">
            <xml id="10204" title="AccountNumber3">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10205)">
            <xml id="10205" title="AccountNumber4">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10206)">
            <xml id="10206" title="AccountNumber5">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10207)">
            <xml id="10207" title="SubscribeNewsletter">
              <xsl:text>False</xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10208)">
            <xml id="10208" title="HTMLNewsletter">
              <xsl:text>False</xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10209)">
            <xml id="10209" title="Bit1">
              <xsl:text>False</xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10210)">
            <xml id="10210" title="Bit2">
              <xsl:text>False</xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10211)">
            <xml id="10211" title="Bit3">
              <xsl:text>False</xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10212)">
            <xml id="10212" title="Bit4">
              <xsl:text>False</xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10213)">
            <xml id="10213" title="Bit5">
              <xsl:text>False</xsl:text>
            </xml>
          </xsl:if>
          <xsl:if test="not($n10214)">
            <xml id="10214" title="Position">
              <xsl:text> </xsl:text>
            </xml>
          </xsl:if>

          <!-- Add the Business Unit -->
          <xsl:element name="xml">
            <xsl:attribute name="id">10301</xsl:attribute>
            <xsl:attribute name="title">BusinessUnit</xsl:attribute>
            <xsl:value-of select="$BusinessUnit"/>
          </xsl:element>

          <!-- Add the Partner -->
          <xsl:element name="xml">
            <xsl:attribute name="id">10302</xsl:attribute>
            <xsl:attribute name="title">Partner</xsl:attribute>
            <xsl:if test="$Partner">
              <xsl:for-each select="xml">
                <xsl:if test="@id=118">
                  <xsl:value-of select="current()"/>
                </xsl:if>
              </xsl:for-each>
            </xsl:if>
            <xsl:if test="not($Partner)">
              <xsl:text>EVERYONE</xsl:text>
            </xsl:if>
          </xsl:element>

        </xsl:element>
        <!--</xsl:if>-->
      </xsl:for-each>

    </xsl:element>

  </xsl:template>

  <xsl:template name="SortAddress-AllLines">
    <xsl:param name="n100"/>
    <xsl:param name="n101"/>
    <xsl:param name="n102"/>
    <xsl:param name="n103"/>
    <xsl:param name="n104"/>

    <xsl:call-template name="AddAddressLine1" />


    <xsl:element name="xml">
      <xsl:attribute name="id">101</xsl:attribute>
      <xsl:attribute name="title">AddressLine2</xsl:attribute>
      <xsl:for-each select="xml">
        <xsl:if test="@id=100">
          <xsl:value-of select="current()"/>
          <!--<xsl:text>, </xsl:text>-->
        </xsl:if>
      </xsl:for-each>
      <!--<xsl:for-each select="xml">
        <xsl:if test="@id=101">
          <xsl:value-of select="current()"/>
        </xsl:if>
      </xsl:for-each>-->
    </xsl:element>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">101</xsl:with-param>
      <xsl:with-param name="exists" select="$n102" />
      <xsl:with-param name="NewXmlNodeID">102</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine3</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">102</xsl:with-param>
      <xsl:with-param name="exists" select="$n103" />
      <xsl:with-param name="NewXmlNodeID">103</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine4</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">103</xsl:with-param>
      <xsl:with-param name="exists" select="$n104" />
      <xsl:with-param name="NewXmlNodeID">104</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine5</xsl:with-param>
    </xsl:call-template>

  </xsl:template>

  <xsl:template name="SortAddress-Line1Blank">
    <xsl:param name="n100"/>
    <xsl:param name="n101"/>
    <xsl:param name="n102"/>
    <xsl:param name="n103"/>
    <xsl:param name="n104"/>

    <xsl:call-template name="AddAddressLine1" />

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">101</xsl:with-param>
      <xsl:with-param name="exists" select="$n100" />
      <xsl:with-param name="NewXmlNodeID">101</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine2</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">102</xsl:with-param>
      <xsl:with-param name="exists" select="$n102" />
      <xsl:with-param name="NewXmlNodeID">102</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine3</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">103</xsl:with-param>
      <xsl:with-param name="exists" select="$n103" />
      <xsl:with-param name="NewXmlNodeID">103</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine4</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">104</xsl:with-param>
      <xsl:with-param name="exists" select="$n104" />
      <xsl:with-param name="NewXmlNodeID">104</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine5</xsl:with-param>
    </xsl:call-template>

  </xsl:template>

  <xsl:template name="SortAddress-Line2Blank">
    <xsl:param name="n100"/>
    <xsl:param name="n101"/>
    <xsl:param name="n102"/>
    <xsl:param name="n103"/>
    <xsl:param name="n104"/>

    <xsl:call-template name="AddAddressLine1" />

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">100</xsl:with-param>
      <xsl:with-param name="exists" select="$n100" />
      <xsl:with-param name="NewXmlNodeID">101</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine2</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">102</xsl:with-param>
      <xsl:with-param name="exists" select="$n102" />
      <xsl:with-param name="NewXmlNodeID">102</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine3</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">103</xsl:with-param>
      <xsl:with-param name="exists" select="$n103" />
      <xsl:with-param name="NewXmlNodeID">103</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine4</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">104</xsl:with-param>
      <xsl:with-param name="exists" select="$n104" />
      <xsl:with-param name="NewXmlNodeID">104</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine5</xsl:with-param>
    </xsl:call-template>

  </xsl:template>

  <xsl:template name="SortAddress-Line3Blank">
    <xsl:param name="n100"/>
    <xsl:param name="n101"/>
    <xsl:param name="n102"/>
    <xsl:param name="n103"/>
    <xsl:param name="n104"/>

    <xsl:call-template name="AddAddressLine1" />

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">100</xsl:with-param>
      <xsl:with-param name="exists" select="$n100" />
      <xsl:with-param name="NewXmlNodeID">101</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine2</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">101</xsl:with-param>
      <xsl:with-param name="exists" select="$n101" />
      <xsl:with-param name="NewXmlNodeID">102</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine3</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">103</xsl:with-param>
      <xsl:with-param name="exists" select="$n103" />
      <xsl:with-param name="NewXmlNodeID">103</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine4</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">104</xsl:with-param>
      <xsl:with-param name="exists" select="$n104" />
      <xsl:with-param name="NewXmlNodeID">104</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine5</xsl:with-param>
    </xsl:call-template>

  </xsl:template>

  <xsl:template name="SortAddress-Line4Blank">
    <xsl:param name="n100"/>
    <xsl:param name="n101"/>
    <xsl:param name="n102"/>
    <xsl:param name="n103"/>
    <xsl:param name="n104"/>

    <xsl:call-template name="AddAddressLine1" />

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">100</xsl:with-param>
      <xsl:with-param name="exists" select="$n100" />
      <xsl:with-param name="NewXmlNodeID">101</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine2</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">101</xsl:with-param>
      <xsl:with-param name="exists" select="$n101" />
      <xsl:with-param name="NewXmlNodeID">102</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine3</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">102</xsl:with-param>
      <xsl:with-param name="exists" select="$n102" />
      <xsl:with-param name="NewXmlNodeID">103</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine4</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">104</xsl:with-param>
      <xsl:with-param name="exists" select="$n104" />
      <xsl:with-param name="NewXmlNodeID">104</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine5</xsl:with-param>
    </xsl:call-template>

  </xsl:template>

  <xsl:template name="SortAddress-Line5Blank">
    <xsl:param name="n100"/>
    <xsl:param name="n101"/>
    <xsl:param name="n102"/>
    <xsl:param name="n103"/>
    <xsl:param name="n104"/>

    <xsl:call-template name="AddAddressLine1" />

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">100</xsl:with-param>
      <xsl:with-param name="exists" select="$n100" />
      <xsl:with-param name="NewXmlNodeID">101</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine2</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">101</xsl:with-param>
      <xsl:with-param name="exists" select="$n101" />
      <xsl:with-param name="NewXmlNodeID">102</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine3</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">102</xsl:with-param>
      <xsl:with-param name="exists" select="$n102" />
      <xsl:with-param name="NewXmlNodeID">103</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine4</xsl:with-param>
    </xsl:call-template>

    <xsl:call-template name="SortAddressNode">
      <xsl:with-param name="SelectNodeID">103</xsl:with-param>
      <xsl:with-param name="exists" select="$n103" />
      <xsl:with-param name="NewXmlNodeID">104</xsl:with-param>
      <xsl:with-param name="XmlTitle">AddressLine5</xsl:with-param>
    </xsl:call-template>

  </xsl:template>

  <xsl:template name="AddAddressLine1">
    <xsl:element name="xml">
      <xsl:attribute name="id">100</xsl:attribute>
      <xsl:attribute name="title">AddressLine1</xsl:attribute>
      <xsl:for-each select="xml">
        <xsl:if test="@id=117">
          <xsl:value-of select="current()"/>
        </xsl:if>
      </xsl:for-each>
    </xsl:element>
  </xsl:template>

  <!-- 
    Checks to see if the current node specified exists in 
    the original XML document, if it does, it is outputted 
    1 line down form its original position (i.e. address line 3
    would become address line 4), if it does not exist in the 
    original XML document then an empty element is created. 
    -->
  <xsl:template name="SortAddressNode">
    <xsl:param name="exists"/>
    <xsl:param name="SelectNodeID"/>
    <xsl:param name="NewXmlNodeID"/>
    <xsl:param name="XmlTitle"/>

    <!-- Create the element and its attributes -->
    <xsl:element name="xml">
      <xsl:attribute name="id">
        <xsl:value-of select="$NewXmlNodeID"/>
      </xsl:attribute>
      <xsl:attribute name="title">
        <xsl:value-of select="$XmlTitle"/>
      </xsl:attribute>

      <!-- If the original element does NOT exists
            add and empty text field -->
      <xsl:if test="not($exists)">
        <xsl:text> </xsl:text>
      </xsl:if>

      <!-- If the original element does exists
            find it and add its contents -->
      <xsl:if test="$exists">
        <xsl:for-each select="xml">
          <xsl:if test="@id=$SelectNodeID">
            <xsl:value-of select="current()"/>
          </xsl:if>
        </xsl:for-each>
      </xsl:if>

      <!-- Close the XML element -->
    </xsl:element>

    <!-- Close the template -->
  </xsl:template>

</xsl:stylesheet>