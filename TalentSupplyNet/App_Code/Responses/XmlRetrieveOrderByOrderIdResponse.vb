Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal


    Public Class XmlRetrieveOrderByOrderIdResponse
        Inherits XmlResponse

#Region "Class Level Fields"

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndResponse, ndReturnCode, ndOrders As XmlNode

#End Region

#Region "Protected Methods"

        Protected Overrides Sub InsertBodyV1()
            Try
                With MyBase.xmlDoc
                    ndResponse = .CreateElement("Response")
                    ndOrders = .CreateElement("Orders")
                End With
                ndResponse.AppendChild(ndOrders)

                createOrderNodes()

                Const c1 As String = "//"
                Const c2 As String = "/TransactionHeader"
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndResponse, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "XmlRetrieveOrdersByIdResponse-01"
                    .HasError = True
                End With
            End Try

        End Sub
        Protected Overrides Sub InsertBodyV1_1()
            Try
                With MyBase.xmlDoc
                    ndResponse = .CreateElement("Response")
                    ndOrders = .CreateElement("Orders")
                End With
                ndResponse.AppendChild(ndOrders)

                createOrderNodes()

                Const c1 As String = "//"
                Const c2 As String = "/TransactionHeader"
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndResponse, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "XmlRetrieveOrdersByIdResponse-01"
                    .HasError = True
                End With
            End Try

        End Sub

#End Region

#Region "Private Methods"

        Private Sub createOrderNodes()
            Dim lastWeborderId As String = String.Empty
            For Each row As DataRow In ResultDataSet.Tables(0).Rows

                Dim settings As XmlWriterSettings = New XmlWriterSettings()
                settings.Indent = True
                settings.OmitXmlDeclaration = True
                Dim writer As XmlWriter = XmlWriter.Create(Console.Out, settings)

                'Declare and set node variables for each order (order, header, details)
                'Only bind the header nodes onces
                Try
                    Dim ndOrder, ndOrderHeaderInformation, ndOrderDetailsInformation As XmlNode
                    If row("PROCESSED_ORDER_ID").ToString <> lastWeborderId Then
                        lastWeborderId = row("PROCESSED_ORDER_ID").ToString
                        ndOrder = xmlDoc.CreateElement("Order")
                        ndOrderHeaderInformation = xmlDoc.CreateElement("OrderHeaderInformation")
                        ndOrderDetailsInformation = xmlDoc.CreateElement("OrderDetailsInformation")

                        createOrderHeaderInformation(ndOrderHeaderInformation, row)

                        ndOrder.AppendChild(ndOrderHeaderInformation)
                        ndOrder.AppendChild(ndOrderDetailsInformation)
                        ndOrders.AppendChild(ndOrder)
                    End If

                    'Item node variables
                    Dim ndItem As XmlNode
                    Dim ndLineNumber, ndSKU, ndAlternateSKU, ndProductDescription, ndQuantity, ndMasterProduct, ndItemPromotionValue, ndItemPromotionPercentage As XmlNode
                    Dim ndItemPaymentOptions, ndPurchasePriceGross, ndPurchasePriceNet, ndPurchasePriceTax, ndDeliveryGross, ndDeliveryNet, ndDeliveryTax As XmlNode
                    Dim ndTaxCode, ndLinePriceGross, ndLinePriceNet, ndLinePriceTax, ndGLCode1, ndGLCode2, ndGLCode3, ndGLCode4, ndGLCode5 As XmlNode
                    'My new Node
                    Dim ndInstructions As XmlNode


                    'Create the items nodes
                    ndItem = xmlDoc.CreateElement("Item")
                    ndLineNumber = xmlDoc.CreateElement("LineNumber")
                    ndSKU = xmlDoc.CreateElement("SKU")
                    ndAlternateSKU = xmlDoc.CreateElement("AlternateSKU")

                    ndProductDescription = xmlDoc.CreateElement("ProductDescription")
                    ndQuantity = xmlDoc.CreateElement("Quantity")
                    ndMasterProduct = xmlDoc.CreateElement("MasterProduct")
                    ndItemPromotionValue = xmlDoc.CreateElement("PromotionValue")
                    ndItemPromotionPercentage = xmlDoc.CreateElement("PromotionPercentage")
                    ndItemPaymentOptions = xmlDoc.CreateElement("PaymentOptions")

                    ndGLCode1 = xmlDoc.CreateElement("GLCode1")
                    ndGLCode2 = xmlDoc.CreateElement("GLCode2")
                    ndGLCode3 = xmlDoc.CreateElement("GLCode3")
                    ndGLCode4 = xmlDoc.CreateElement("GLCode4")
                    ndGLCode5 = xmlDoc.CreateElement("GLCode5")

                    ndPurchasePriceGross = xmlDoc.CreateElement("PurchasePriceGross")
                    ndPurchasePriceNet = xmlDoc.CreateElement("PurchasePriceNet")
                    ndPurchasePriceTax = xmlDoc.CreateElement("PurchasePriceTax")
                    ndDeliveryGross = xmlDoc.CreateElement("DeliveryGross")
                    ndDeliveryNet = xmlDoc.CreateElement("DeliveryNet")
                    ndDeliveryTax = xmlDoc.CreateElement("DeliveryTax")
                    ndTaxCode = xmlDoc.CreateElement("TaxCode")
                    ndLinePriceGross = xmlDoc.CreateElement("LinePriceGross")
                    ndLinePriceNet = xmlDoc.CreateElement("LinePriceNet")
                    ndLinePriceTax = xmlDoc.CreateElement("LinePriceTax")
                    'Create new node for Personalisation
                    ndInstructions = xmlDoc.CreateElement("Instructions")

                    'Put the item data into the nodes
                    ndLineNumber.InnerText = row("LINE_NUMBER").ToString
                    ndSKU.InnerText = row("PRODUCT_CODE").ToString
                    ndAlternateSKU.InnerText = row("ALTERNATE_SKU").ToString
                    ndProductDescription.InnerText = Replace(row("PRODUCT_DESCRIPTION_1").ToString, "&", "and")
                    ndQuantity.InnerText = row("QUANTITY").ToString
                    ndMasterProduct.InnerText = row("MASTER_PRODUCT").ToString
                    ndItemPromotionValue.InnerText = row("PROMOTION_VALUE").ToString
                    ndItemPromotionPercentage.InnerText = row("PROMOTION_PERCENTAGE").ToString
                    ndPurchasePriceGross.InnerText = row("PURCHASE_PRICE_GROSS").ToString
                    ndPurchasePriceNet.InnerText = row("PURCHASE_PRICE_NET").ToString
                    ndPurchasePriceTax.InnerText = row("PURCHASE_PRICE_TAX").ToString
                    ndDeliveryGross.InnerText = row("DELIVERY_GROSS").ToString
                    ndDeliveryNet.InnerText = row("DELIVERY_NET").ToString
                    ndDeliveryTax.InnerText = row("DELIVERY_TAX").ToString
                    ndTaxCode.InnerText = row("TAX_CODE").ToString
                    ndLinePriceGross.InnerText = row("LINE_PRICE_GROSS").ToString
                    ndLinePriceNet.InnerText = row("LINE_PRICE_NET").ToString
                    ndLinePriceTax.InnerText = row("LINE_PRICE_TAX").ToString
                    'Put the Personalisation into the node
                    ndInstructions.InnerText = row("INSTRUCTIONS").ToString

                    ndGLCode1.InnerText = row("PRODUCT_GLCODE_1").ToString
                    ndGLCode2.InnerText = row("PRODUCT_GLCODE_2").ToString
                    ndGLCode3.InnerText = row("PRODUCT_GLCODE_3").ToString
                    ndGLCode4.InnerText = row("PRODUCT_GLCODE_4").ToString
                    ndGLCode5.InnerText = row("PRODUCT_GLCODE_5").ToString

                    'Form the item section
                    ndItem.AppendChild(ndLineNumber)
                    ndItem.AppendChild(ndSKU)
                    ndItem.AppendChild(ndAlternateSKU)
                    ndItem.AppendChild(ndProductDescription)
                    ndItem.AppendChild(ndQuantity)
                    ndItem.AppendChild(ndMasterProduct)
                    ndItem.AppendChild(ndItemPromotionValue)
                    ndItem.AppendChild(ndItemPromotionPercentage)
                    ndItem.AppendChild(ndInstructions)
                    ndItemPaymentOptions.AppendChild(ndPurchasePriceGross)
                    ndItemPaymentOptions.AppendChild(ndPurchasePriceNet)
                    ndItemPaymentOptions.AppendChild(ndPurchasePriceTax)
                    ndItemPaymentOptions.AppendChild(ndDeliveryGross)
                    ndItemPaymentOptions.AppendChild(ndDeliveryNet)
                    ndItemPaymentOptions.AppendChild(ndDeliveryTax)
                    ndItemPaymentOptions.AppendChild(ndTaxCode)
                    ndItemPaymentOptions.AppendChild(ndGLCode1)
                    ndItemPaymentOptions.AppendChild(ndGLCode2)
                    ndItemPaymentOptions.AppendChild(ndGLCode3)
                    ndItemPaymentOptions.AppendChild(ndGLCode4)
                    ndItemPaymentOptions.AppendChild(ndGLCode5)
                    ndItemPaymentOptions.AppendChild(ndLinePriceGross)
                    ndItemPaymentOptions.AppendChild(ndLinePriceNet)
                    ndItemPaymentOptions.AppendChild(ndLinePriceTax)
                    ndItem.AppendChild(ndItemPaymentOptions)
                    'Now 'FORM' ? the Personalisation

                    ndOrderDetailsInformation.AppendChild(ndItem)
                Catch ex As Exception
                    Exit For
                    Throw
                End Try
            Next

        End Sub

        Private Sub createOrderHeaderInformation(ByRef ndOrderHeaderInformation As XmlNode, ByVal row As DataRow)
            'Header node variables
            Dim ndCustomerId, ndWebOrderNo, ndBusinessUnit, ndPartner, ndCreatedDate, ndLastActivityDate As XmlNode
            Dim ndAddressingInformation, ndContactName, ndAddressLine1, ndAddressLine2, ndAddressLine3, ndAddressLine4, ndAddressLine5 As XmlNode
            Dim ndPostCode, ndCountry, ndCountryCode, ndContactEmail, ndSpecialInstructions1, ndSpecialInstructions2, ndShippedDate, ndProjectedDeliveryDate As XmlNode
            Dim ndDeliveryType, ndDeliveryTypeDescription, ndTrackingNo, ndLanguage, ndWarehouse, ndCurrency, ndShippingCode, ndGiftMessage As XmlNode
            Dim ndPurchaseOrder, ndComment, ndPaymentOptions, ndPaymentType, ndTotalOrderItemsValueGross, ndTotalOrderItemsValueNet As XmlNode
            Dim ndTotalOrderItemsValueTax, ndTotalDeliveryGross, ndTotalDeliveryNet, ndTotalDeliveryTax, ndPromotionDescription As XmlNode
            Dim ndPromotionValue, ndPromotionPercentage, ndTotalOrderValue, ndTotalAmountCharged, ndCreditCardType As XmlNode
            Dim ndTaxOptions, ndTaxInclusive1, ndTaxInclusive2, ndTaxInclusive3, ndTaxInclusive4, ndTaxInclusive5 As XmlNode
            Dim ndTaxDisplay1, ndTaxDisplay2, ndTaxDisplay3, ndTaxDisplay4, ndTaxDisplay5 As XmlNode
            Dim ndTaxCode1, ndTaxCode2, ndTaxCode3, ndTaxCode4, ndTaxCode5 As XmlNode
            Dim ndTaxAmount1, ndTaxAmount2, ndTaxAmount3, ndTaxAmount4, ndTaxAmount5 As XmlNode
            'My new Node for SmartcardNumber 28.5.2013
            Dim ndSmartcardNumber As XmlNode

            'Create the nodes
            ndCustomerId = xmlDoc.CreateElement("CustomerId")
            ndWebOrderNo = xmlDoc.CreateElement("WebOrderNo")
            ndBusinessUnit = xmlDoc.CreateElement("BusinessUnit")
            ndPartner = xmlDoc.CreateElement("Partner")
            ndCreatedDate = xmlDoc.CreateElement("CreatedDate")
            ndLastActivityDate = xmlDoc.CreateElement("LastActivityDate")
            ndAddressingInformation = xmlDoc.CreateElement("AddressingInformation")
            ndContactName = xmlDoc.CreateElement("ContactName")
            ndAddressLine1 = xmlDoc.CreateElement("AddressLine1")
            ndAddressLine2 = xmlDoc.CreateElement("AddressLine2")
            ndAddressLine3 = xmlDoc.CreateElement("AddressLine3")
            ndAddressLine4 = xmlDoc.CreateElement("AddressLine4")
            ndAddressLine5 = xmlDoc.CreateElement("AddressLine5")
            ndPostCode = xmlDoc.CreateElement("PostCode")
            ndCountry = xmlDoc.CreateElement("Country")
            ndCountryCode = xmlDoc.CreateElement("CountryCode")
            ndContactEmail = xmlDoc.CreateElement("ContactEmail")
            ndSpecialInstructions1 = xmlDoc.CreateElement("SpecialInstructions1")
            ndSpecialInstructions2 = xmlDoc.CreateElement("SpecialInstructions2")
            ndShippedDate = xmlDoc.CreateElement("ShippedDate")
            ndProjectedDeliveryDate = xmlDoc.CreateElement("ProjectedDeliveryDate")
            ndDeliveryType = xmlDoc.CreateElement("DeliveryType")
            ndDeliveryTypeDescription = xmlDoc.CreateElement("DeliveryTypeDescription")
            ndTrackingNo = xmlDoc.CreateElement("TrackingNo")
            ndLanguage = xmlDoc.CreateElement("Language")
            ndWarehouse = xmlDoc.CreateElement("Warehouse")
            ndCurrency = xmlDoc.CreateElement("Currency")
            ndGiftMessage = xmlDoc.CreateElement("GiftMessage")
            ndShippingCode = xmlDoc.CreateElement("ShippingCode")
            ndPurchaseOrder = xmlDoc.CreateElement("PurchaseOrder")
            ndComment = xmlDoc.CreateElement("Comment")
            'Create new node for Smartcard 28.5.2013
            ndSmartcardNumber = xmlDoc.CreateElement("SmartcardNumber")

            ndPaymentOptions = xmlDoc.CreateElement("PaymentOptions")
            ndPaymentType = xmlDoc.CreateElement("PaymentType")
            ndTotalOrderItemsValueGross = xmlDoc.CreateElement("TotalOrderItemsValueGross")
            ndTotalOrderItemsValueNet = xmlDoc.CreateElement("TotalOrderItemsValueNet")
            ndTotalOrderItemsValueTax = xmlDoc.CreateElement("TotalorderItemsValueTax")
            ndTotalDeliveryGross = xmlDoc.CreateElement("TotalDeliveryGross")
            ndTotalDeliveryNet = xmlDoc.CreateElement("TotalDeliveryNet")
            ndTotalDeliveryTax = xmlDoc.CreateElement("TotalDeliveryTax")
            ndPromotionDescription = xmlDoc.CreateElement("PromotionDescription")
            ndPromotionValue = xmlDoc.CreateElement("PromotionValue")
            ndPromotionPercentage = xmlDoc.CreateElement("PromotionPercentage")
            ndTotalOrderValue = xmlDoc.CreateElement("TotalOrderValue")
            ndTotalAmountCharged = xmlDoc.CreateElement("TotalAmountCharged")
            ndCreditCardType = xmlDoc.CreateElement("CreditCardType")
            ndTaxOptions = xmlDoc.CreateElement("TaxOptions")
            ndTaxInclusive1 = xmlDoc.CreateElement("TaxInclusive1")
            ndTaxDisplay1 = xmlDoc.CreateElement("TaxDisplay1")
            ndTaxCode1 = xmlDoc.CreateElement("TaxCode1")
            ndTaxAmount1 = xmlDoc.CreateElement("TaxAmount1")
            ndTaxInclusive2 = xmlDoc.CreateElement("TaxInclusive2")
            ndTaxDisplay2 = xmlDoc.CreateElement("TaxDisplay2")
            ndTaxCode2 = xmlDoc.CreateElement("TaxCode2")
            ndTaxAmount2 = xmlDoc.CreateElement("TaxAmount2")
            ndTaxInclusive3 = xmlDoc.CreateElement("TaxInclusive3")
            ndTaxDisplay3 = xmlDoc.CreateElement("TaxDisplay3")
            ndTaxCode3 = xmlDoc.CreateElement("TaxCode3")
            ndTaxAmount3 = xmlDoc.CreateElement("TaxAmount3")
            ndTaxInclusive4 = xmlDoc.CreateElement("TaxInclusive4")
            ndTaxDisplay4 = xmlDoc.CreateElement("TaxDisplay4")
            ndTaxCode4 = xmlDoc.CreateElement("TaxCode4")
            ndTaxAmount4 = xmlDoc.CreateElement("TaxAmount4")
            ndTaxInclusive5 = xmlDoc.CreateElement("TaxInclusive5")
            ndTaxDisplay5 = xmlDoc.CreateElement("TaxDisplay5")
            ndTaxCode5 = xmlDoc.CreateElement("TaxCode5")
            ndTaxAmount5 = xmlDoc.CreateElement("TaxAmount5")

            'Put the header data in the nodes
            ndCustomerId.InnerText = row("LOGINID").ToString
            ndWebOrderNo.InnerText = row("PROCESSED_ORDER_ID").ToString
            ndBusinessUnit.InnerText = row("BUSINESS_UNIT").ToString
            ndPartner.InnerText = row("PARTNER").ToString
            ndCreatedDate.InnerText = row("CREATED_DATE").ToString
            ndLastActivityDate.InnerText = row("LAST_ACTIVITY_DATE").ToString
            'The settings.stadium property has been re-used here for ExcludeCustInfo flag
            If Settings.Stadium = String.Empty Then
                ndContactName.InnerText = EncodeXML(row("CONTACT_NAME").ToString)
                ndAddressLine1.InnerText = EncodeXML(row("ADDRESS_LINE_1").ToString)
                ndAddressLine2.InnerText = EncodeXML(row("ADDRESS_LINE_2").ToString)
                ndAddressLine3.InnerText = EncodeXML(row("ADDRESS_LINE_3").ToString)
                ndAddressLine4.InnerText = EncodeXML(row("ADDRESS_LINE_4").ToString)
                ndAddressLine5.InnerText = EncodeXML(row("ADDRESS_LINE_5").ToString)
                ndPostCode.InnerText = row("POSTCODE").ToString
                ndCountry.InnerText = row("COUNTRY").ToString
                ndCountryCode.InnerText = row("COUNTRY_CODE").ToString
                ndContactEmail.InnerText = EncodeXML(row("CONTACT_EMAIL").ToString)
                ndSpecialInstructions1.InnerText = row("SPECIAL_INSTRUCTIONS_1").ToString
                ndSpecialInstructions2.InnerText = row("SPECIAL_INSTRUCTIONS_2").ToString
            Else
                ndContactName.InnerText = String.Empty
                ndAddressLine1.InnerText = String.Empty
                ndAddressLine2.InnerText = String.Empty
                ndAddressLine3.InnerText = String.Empty
                ndAddressLine4.InnerText = String.Empty
                ndAddressLine5.InnerText = String.Empty
                ndPostCode.InnerText = String.Empty
                ndCountry.InnerText = String.Empty
                ndCountryCode.InnerText = String.Empty
                ndContactEmail.InnerText = String.Empty
                ndSpecialInstructions1.InnerText = String.Empty
                ndSpecialInstructions2.InnerText = String.Empty
            End If
            ndShippedDate.InnerText = row("SHIPPED_DATE").ToString
            ndProjectedDeliveryDate.InnerText = row("PROJECTED_DELIVERY_DATE").ToString
            ndDeliveryType.InnerText = row("DELIVERY_TYPE").ToString
            ndDeliveryTypeDescription.InnerText = row("DELIVERY_TYPE_DESCRIPTION").ToString
            ndTrackingNo.InnerText = row("TRACKING_NO").ToString
            ndLanguage.InnerText = row("LANGUAGE").ToString
            ndWarehouse.InnerText = row("WAREHOUSE").ToString
            ndCurrency.InnerText = row("CURRENCY").ToString
            ndGiftMessage.InnerText = row("GIFT_MESSAGE").ToString
            ndShippingCode.InnerText = row("SHIPPING_CODE").ToString
            ndPurchaseOrder.InnerText = row("PURCHASE_ORDER").ToString
            ndComment.InnerText = row("PURCHASE_ORDER").ToString
            'Put the SmartcardNumber into the node - 28.5.2013
            ndSmartcardNumber.InnerText = row("SMARTCARD_NUMBER").ToString

            ndPaymentType.InnerText = row("PAYMENT_TYPE").ToString
            ndTotalOrderItemsValueGross.InnerText = row("TOTAL_ORDER_ITEMS_VALUE_GROSS").ToString
            ndTotalOrderItemsValueNet.InnerText = row("TOTAL_ORDER_ITEMS_VALUE_NET").ToString
            ndTotalOrderItemsValueTax.InnerText = row("TOTAL_ORDER_ITEMS_TAX").ToString
            ndTotalDeliveryGross.InnerText = row("TOTAL_DELIVERY_GROSS").ToString
            ndTotalDeliveryNet.InnerText = row("TOTAL_DELIVERY_NET").ToString
            ndTotalDeliveryTax.InnerText = row("TOTAL_DELIVERY_TAX").ToString
            ndPromotionDescription.InnerText = row("PROMOTION_DESCRIPTION").ToString
            ndPromotionValue.InnerText = row("PROMOTION_VALUE").ToString
            ndPromotionPercentage.InnerText = row("PROMOTION_PERCENTAGE").ToString
            ndTotalOrderValue.InnerText = row("TOTAL_ORDER_VALUE").ToString
            ndTotalAmountCharged.InnerText = row("TOTAL_AMOUNT_CHARGED").ToString
            ndCreditCardType.InnerText = row("CREDIT_CARD_TYPE").ToString
            ndTaxInclusive1.InnerText = row("TAX_INCLUSIVE_1").ToString
            ndTaxDisplay1.InnerText = row("TAX_DISPLAY_1").ToString
            ndTaxCode1.InnerText = row("TAX_CODE_1").ToString
            ndTaxAmount1.InnerText = row("TAX_AMOUNT_1").ToString
            ndTaxInclusive2.InnerText = row("TAX_INCLUSIVE_2").ToString
            ndTaxDisplay2.InnerText = row("TAX_DISPLAY_2").ToString
            ndTaxCode2.InnerText = row("TAX_CODE_2").ToString
            ndTaxAmount2.InnerText = row("TAX_AMOUNT_2").ToString
            ndTaxInclusive3.InnerText = row("TAX_INCLUSIVE_3").ToString
            ndTaxDisplay3.InnerText = row("TAX_DISPLAY_3").ToString
            ndTaxCode3.InnerText = row("TAX_CODE_3").ToString
            ndTaxAmount3.InnerText = row("TAX_AMOUNT_3").ToString
            ndTaxInclusive4.InnerText = row("TAX_INCLUSIVE_4").ToString
            ndTaxDisplay4.InnerText = row("TAX_DISPLAY_4").ToString
            ndTaxCode4.InnerText = row("TAX_CODE_4").ToString
            ndTaxAmount4.InnerText = row("TAX_AMOUNT_4").ToString
            ndTaxInclusive5.InnerText = row("TAX_INCLUSIVE_5").ToString
            ndTaxDisplay5.InnerText = row("TAX_DISPLAY_5").ToString
            ndTaxCode5.InnerText = row("TAX_CODE_5").ToString
            ndTaxAmount5.InnerText = row("TAX_AMOUNT_5").ToString

            'Form the header section
            ndOrderHeaderInformation.AppendChild(ndCustomerId)
            ndOrderHeaderInformation.AppendChild(ndWebOrderNo)
            ndOrderHeaderInformation.AppendChild(ndBusinessUnit)
            ndOrderHeaderInformation.AppendChild(ndPartner)
            ndOrderHeaderInformation.AppendChild(ndCreatedDate)
            ndOrderHeaderInformation.AppendChild(ndLastActivityDate)
            ndAddressingInformation.AppendChild(ndContactName)
            ndAddressingInformation.AppendChild(ndAddressLine1)
            ndAddressingInformation.AppendChild(ndAddressLine2)
            ndAddressingInformation.AppendChild(ndAddressLine3)
            ndAddressingInformation.AppendChild(ndAddressLine4)
            ndAddressingInformation.AppendChild(ndAddressLine5)
            ndAddressingInformation.AppendChild(ndPostCode)
            ndAddressingInformation.AppendChild(ndCountry)
            ndAddressingInformation.AppendChild(ndCountryCode)
            ndAddressingInformation.AppendChild(ndContactEmail)
            ndAddressingInformation.AppendChild(ndSpecialInstructions1)
            ndAddressingInformation.AppendChild(ndSpecialInstructions2)
            ndAddressingInformation.AppendChild(ndShippedDate)
            ndAddressingInformation.AppendChild(ndProjectedDeliveryDate)
            ndAddressingInformation.AppendChild(ndDeliveryType)
            ndAddressingInformation.AppendChild(ndDeliveryTypeDescription)
            ndOrderHeaderInformation.AppendChild(ndAddressingInformation)
            ndOrderHeaderInformation.AppendChild(ndTrackingNo)
            ndOrderHeaderInformation.AppendChild(ndLanguage)
            ndOrderHeaderInformation.AppendChild(ndWarehouse)
            ndOrderHeaderInformation.AppendChild(ndCurrency)
            ndOrderHeaderInformation.AppendChild(ndShippingCode)
            ndOrderHeaderInformation.AppendChild(ndGiftMessage)
            ndOrderHeaderInformation.AppendChild(ndPurchaseOrder)
            ndOrderHeaderInformation.AppendChild(ndComment)
            ' Smartcard Number 28.5.2013
            ndOrderHeaderInformation.AppendChild(ndSmartcardNumber)

            ndPaymentOptions.AppendChild(ndPaymentType)
            ndPaymentOptions.AppendChild(ndTotalOrderItemsValueGross)
            ndPaymentOptions.AppendChild(ndTotalOrderItemsValueNet)
            ndPaymentOptions.AppendChild(ndTotalOrderItemsValueTax)
            ndPaymentOptions.AppendChild(ndTotalDeliveryGross)
            ndPaymentOptions.AppendChild(ndTotalDeliveryNet)
            ndPaymentOptions.AppendChild(ndTotalDeliveryTax)
            ndPaymentOptions.AppendChild(ndPromotionDescription)
            ndPaymentOptions.AppendChild(ndPromotionValue)
            ndPaymentOptions.AppendChild(ndPromotionPercentage)
            ndPaymentOptions.AppendChild(ndTotalOrderValue)
            ndPaymentOptions.AppendChild(ndTotalAmountCharged)
            ndPaymentOptions.AppendChild(ndCreditCardType)
            ndOrderHeaderInformation.AppendChild(ndPaymentOptions)
            ndTaxOptions.AppendChild(ndTaxInclusive1)
            ndTaxOptions.AppendChild(ndTaxDisplay1)
            ndTaxOptions.AppendChild(ndTaxCode1)
            ndTaxOptions.AppendChild(ndTaxAmount1)
            ndTaxOptions.AppendChild(ndTaxInclusive2)
            ndTaxOptions.AppendChild(ndTaxDisplay2)
            ndTaxOptions.AppendChild(ndTaxCode2)
            ndTaxOptions.AppendChild(ndTaxAmount2)
            ndTaxOptions.AppendChild(ndTaxInclusive3)
            ndTaxOptions.AppendChild(ndTaxDisplay3)
            ndTaxOptions.AppendChild(ndTaxCode3)
            ndTaxOptions.AppendChild(ndTaxAmount3)
            ndTaxOptions.AppendChild(ndTaxInclusive4)
            ndTaxOptions.AppendChild(ndTaxDisplay4)
            ndTaxOptions.AppendChild(ndTaxCode4)
            ndTaxOptions.AppendChild(ndTaxAmount4)
            ndTaxOptions.AppendChild(ndTaxInclusive5)
            ndTaxOptions.AppendChild(ndTaxDisplay5)
            ndTaxOptions.AppendChild(ndTaxCode5)
            ndTaxOptions.AppendChild(ndTaxAmount5)
            ndOrderHeaderInformation.AppendChild(ndTaxOptions)
        End Sub

        Private Function EncodeXML(ByVal nodeString As String) As String
            Static badAmpersand As New Regex("&(?![a-zA-Z]{2,6};|#[0-9]{2,4};)")

            nodeString = badAmpersand.Replace(nodeString, "&amp;")

            Return nodeString.Replace("<", "&lt;").Replace("""", "&quot;").Replace(">", "gt;")
        End Function

        'Luke added 05/07/2016
        Private Function CdataXML(ByVal nodeString As String) As String

            nodeString = "<![CDATA[" & nodeString & "]]>"

            Return nodeString
        End Function

#End Region
    End Class
End Namespace