Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal

    Public Class XmlRetrieveOrdersByStatusResponse
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

                If MarkOrdersAsComplete Then
                    Dim listOfOrderIds As New Collection
                    For Each row As DataRow In ResultDataSet.Tables(0).Rows
                        Dim lastWebOrderId As String = String.Empty
                        If row("TEMP_ORDER_ID").ToString <> lastWebOrderId Then
                            lastWebOrderId = row("TEMP_ORDER_ID").ToString
                            listOfOrderIds.Add(lastWebOrderId)
                        End If
                    Next
                    Dim tDataObjects As New TalentDataObjects
                    tDataObjects.Settings = Settings
                    tDataObjects.OrderSettings.MarkOrdersAsComplete(Settings.BusinessUnit, Settings.Partner, OrderCompleteStatus, listOfOrderIds)
                End If
            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "XmlRetrieveOrdersByStatusResponse-01"
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

                createOrderNodesV1_1()

                Const c1 As String = "//"
                Const c2 As String = "/TransactionHeader"
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndResponse, ndHeaderRootHeader)

                If MarkOrdersAsComplete Then
                    Dim listOfOrderIds As New Collection
                    For Each row As DataRow In ResultDataSet.Tables(0).Rows
                        Dim lastWebOrderId As String = String.Empty
                        If row("TEMP_ORDER_ID").ToString <> lastWebOrderId Then
                            lastWebOrderId = row("TEMP_ORDER_ID").ToString
                            listOfOrderIds.Add(lastWebOrderId)
                        End If
                    Next
                    Dim tDataObjects As New TalentDataObjects
                    tDataObjects.Settings = Settings
                    tDataObjects.OrderSettings.MarkOrdersAsComplete(Settings.BusinessUnit, Settings.Partner, OrderCompleteStatus, listOfOrderIds)
                End If
            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "XmlRetrieveOrdersByStatusResponse-02"
                    .HasError = True
                End With
            End Try

        End Sub
#End Region

#Region "Private Methods"

        Private Sub createOrderNodes()
            Dim lastWeborderId As String = String.Empty
            For Each row As DataRow In ResultDataSet.Tables(0).Rows

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
                    Dim ndLineNumber, ndSKU, ndProductDescription, ndQuantity, ndMasterProduct, ndItemPromotionValue, ndItemPromotionPercentage As XmlNode
                    Dim ndItemPaymentOptions, ndPurchasePriceGross, ndPurchasePriceNet, ndPurchasePriceTax, ndDeliveryGross, ndDeliveryNet, ndDeliveryTax As XmlNode
                    Dim ndTaxCode, ndLinePriceGross, ndLinePriceNet, ndLinePriceTax As XmlNode
                    'My new Node
                    Dim ndInstructions As XmlNode


                    'Create the items nodes
                    ndItem = xmlDoc.CreateElement("Item")
                    ndLineNumber = xmlDoc.CreateElement("LineNumber")
                    ndSKU = xmlDoc.CreateElement("SKU")
                    ndProductDescription = xmlDoc.CreateElement("ProductDescription")
                    ndQuantity = xmlDoc.CreateElement("Quantity")
                    ndMasterProduct = xmlDoc.CreateElement("MasterProduct")
                    ndItemPromotionValue = xmlDoc.CreateElement("PromotionValue")
                    ndItemPromotionPercentage = xmlDoc.CreateElement("PromotionPercentage")
                    ndItemPaymentOptions = xmlDoc.CreateElement("PaymentOptions")
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
                    ndProductDescription.InnerText = row("PRODUCT_DESCRIPTION_1").ToString
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
                    

                    'Form the item section
                    ndItem.AppendChild(ndLineNumber)
                    ndItem.AppendChild(ndSKU)
                    ndItem.AppendChild(ndProductDescription)
                    ndItem.AppendChild(ndQuantity)
                    ndItem.AppendChild(ndMasterProduct)
                    ndItem.AppendChild(ndItemPromotionValue)
                    ndItem.AppendChild(ndItemPromotionPercentage)
                    ndItem.AppendChild(ndItemPromotionPercentage)
                    ndItemPaymentOptions.AppendChild(ndPurchasePriceGross)
                    ndItemPaymentOptions.AppendChild(ndPurchasePriceNet)
                    ndItemPaymentOptions.AppendChild(ndPurchasePriceTax)
                    ndItemPaymentOptions.AppendChild(ndDeliveryGross)
                    ndItemPaymentOptions.AppendChild(ndDeliveryNet)
                    ndItemPaymentOptions.AppendChild(ndDeliveryTax)
                    ndItemPaymentOptions.AppendChild(ndTaxCode)
                    ndItemPaymentOptions.AppendChild(ndLinePriceGross)
                    ndItemPaymentOptions.AppendChild(ndLinePriceNet)
                    ndItemPaymentOptions.AppendChild(ndLinePriceTax)
                    ndItem.AppendChild(ndItemPaymentOptions)
                    'Now 'FORM' ? the Personalisation
                    ndItem.AppendChild(ndInstructions)
                    ndOrderDetailsInformation.AppendChild(ndItem)
                Catch ex As Exception
                    Exit For
                    Throw
                End Try
            Next

        End Sub

        Private Sub createOrderNodesV1_1()
            Dim lastWeborderId As String = String.Empty
            For Each row As DataRow In ResultDataSet.Tables(0).Rows
                'Declare and set node variables for each order (order, header, details)
                'Only bind the header nodes onces
                Try
                    Dim ndOrder, ndOrderHeaderInformation, ndOrderDetailsInformation As XmlNode
                    If row("PROCESSED_ORDER_ID").ToString <> lastWeborderId Then
                        lastWeborderId = row("PROCESSED_ORDER_ID").ToString
                        ndOrder = xmlDoc.CreateElement("Order")
                        ndOrderHeaderInformation = xmlDoc.CreateElement("OrderHeaderInformation")
                        ndOrderDetailsInformation = xmlDoc.CreateElement("OrderDetailsInformation")

                        createOrderHeaderInformationV1_1(ndOrderHeaderInformation, row)

                        ndOrder.AppendChild(ndOrderHeaderInformation)
                        ndOrder.AppendChild(ndOrderDetailsInformation)
                        ndOrders.AppendChild(ndOrder)
                    End If

                    'Item node variables
                    Dim ndItem As XmlNode
                    Dim ndLineNumber, ndSKU, ndProductDescription, ndQuantity, ndQuantityShipped, ndMasterProduct, ndItemPromotionValue, ndItemPromotionPercentage As XmlNode
                    Dim ndItemPaymentOptions, ndPurchasePriceGross, ndPurchasePriceNet, ndPurchasePriceTax, ndDeliveryGross, ndDeliveryNet, ndDeliveryTax As XmlNode
                    Dim ndTaxCode, ndLinePriceGross, ndLinePriceNet, ndLinePriceTax As XmlNode
                    'My new Node
                    Dim ndInstructions As XmlNode


                    'Create the items nodes
                    ndItem = xmlDoc.CreateElement("Item")
                    ndLineNumber = xmlDoc.CreateElement("LineNumber")
                    ndSKU = xmlDoc.CreateElement("SKU")
                    ndProductDescription = xmlDoc.CreateElement("ProductDescription")
                    ndQuantity = xmlDoc.CreateElement("Quantity")
                    ndQuantityShipped = xmlDoc.CreateElement("QuantityShipped")
                    ndMasterProduct = xmlDoc.CreateElement("MasterProduct")
                    ndItemPromotionValue = xmlDoc.CreateElement("PromotionValue")
                    ndItemPromotionPercentage = xmlDoc.CreateElement("PromotionPercentage")
                    ndItemPaymentOptions = xmlDoc.CreateElement("PaymentOptions")
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
                    ndProductDescription.InnerText = row("PRODUCT_DESCRIPTION_1").ToString
                    ndQuantity.InnerText = row("QUANTITY").ToString
                    ndQuantityShipped.InnerText = row("QUANTITY_SHIPPED").ToString
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


                    'Form the item section
                    ndItem.AppendChild(ndLineNumber)
                    ndItem.AppendChild(ndSKU)
                    ndItem.AppendChild(ndProductDescription)
                    ndItem.AppendChild(ndQuantity)
                    ndItem.AppendChild(ndQuantityShipped)
                    ndItem.AppendChild(ndMasterProduct)
                    ndItem.AppendChild(ndItemPromotionValue)
                    ndItem.AppendChild(ndItemPromotionPercentage)
                    ndItem.AppendChild(ndItemPromotionPercentage)
                    ndItemPaymentOptions.AppendChild(ndPurchasePriceGross)
                    ndItemPaymentOptions.AppendChild(ndPurchasePriceNet)
                    ndItemPaymentOptions.AppendChild(ndPurchasePriceTax)
                    ndItemPaymentOptions.AppendChild(ndDeliveryGross)
                    ndItemPaymentOptions.AppendChild(ndDeliveryNet)
                    ndItemPaymentOptions.AppendChild(ndDeliveryTax)
                    ndItemPaymentOptions.AppendChild(ndTaxCode)
                    ndItemPaymentOptions.AppendChild(ndLinePriceGross)
                    ndItemPaymentOptions.AppendChild(ndLinePriceNet)
                    ndItemPaymentOptions.AppendChild(ndLinePriceTax)
                    ndItem.AppendChild(ndItemPaymentOptions)
                    'Now 'FORM' ? the Personalisation
                    ndItem.AppendChild(ndInstructions)
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
            Dim ndPostCode, ndCountry, ndContactEmail, ndSpecialInstructions1, ndSpecialInstructions2, ndShippedDate, ndProjectedDeliveryDate As XmlNode
            Dim ndDeliveryType, ndDeliveryTypeDescription, ndTrackingNo, ndLanguage, ndWarehouse, ndCurrency, ndShippingCode, ndGiftMessage As XmlNode
            Dim ndPurchaseOrder, ndPaymentOptions, ndPaymentType, ndTotalOrderItemsValueGross, ndTotalOrderItemsValueNet As XmlNode
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
            ndContactName.InnerText = row("CONTACT_NAME").ToString
            ndAddressLine1.InnerText = row("ADDRESS_LINE_1").ToString
            ndAddressLine2.InnerText = row("ADDRESS_LINE_2").ToString
            ndAddressLine3.InnerText = row("ADDRESS_LINE_3").ToString
            ndAddressLine4.InnerText = row("ADDRESS_LINE_4").ToString
            ndAddressLine5.InnerText = row("ADDRESS_LINE_5").ToString
            ndPostCode.InnerText = row("POSTCODE").ToString
            ndCountry.InnerText = row("COUNTRY").ToString
            ndContactEmail.InnerText = row("CONTACT_EMAIL").ToString
            ndSpecialInstructions1.InnerText = row("SPECIAL_INSTRUCTIONS_1").ToString
            ndSpecialInstructions2.InnerText = row("SPECIAL_INSTRUCTIONS_2").ToString
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

        Private Sub createOrderHeaderInformationV1_1(ByRef ndOrderHeaderInformation As XmlNode, ByVal row As DataRow)
            'Header node variables
            Dim ndCustomerId, ndWebOrderNo, ndBusinessUnit, ndPartner, ndCreatedDate, ndLastActivityDate As XmlNode
            Dim ndAddressingInformation, ndContactName, ndAddressLine1, ndAddressLine2, ndAddressLine3, ndAddressLine4, ndAddressLine5 As XmlNode
            Dim ndBillingInformation, ndBillingContactName, ndBillingAddressLine1, ndBillingAddressLine2, ndBillingAddressLine3, ndBillingAddressLine4 As XmlNode
            Dim ndBillingAddressLine5, ndBillingPostCode, ndBillingCountry, ndMobileNumber, ndTelephoneNumber, ndWorkNumber, ndFaxNumber, ndOtherNumber As XmlNode
            Dim ndPostCode, ndCountry, ndContactEmail, ndSpecialInstructions1, ndSpecialInstructions2, ndShippedDate, ndProjectedDeliveryDate As XmlNode
            Dim ndDeliveryType, ndDeliveryTypeDescription, ndTrackingNo, ndLanguage, ndWarehouse, ndCurrency, ndShippingCode, ndGiftMessage, ndIsGift, ndRecipientName, ndGiftText As XmlNode
            Dim ndDeliveryNotes, ndPurchaseOrder, ndPaymentOptions, ndPaymentType, ndTotalOrderItemsValueGross, ndTotalOrderItemsValueNet As XmlNode
            Dim ndTotalOrderItemsValueTax, ndTotalDeliveryGross, ndTotalDeliveryNet, ndTotalDeliveryTax, ndPromotionDescription As XmlNode
            Dim ndPromotionValue, ndPromotionPercentage, ndTotalOrderValue, ndTotalAmountCharged, ndCreditCardType As XmlNode
            Dim ndTaxOptions, ndTaxInclusive1, ndTaxInclusive2, ndTaxInclusive3, ndTaxInclusive4, ndTaxInclusive5 As XmlNode
            Dim ndTaxDisplay1, ndTaxDisplay2, ndTaxDisplay3, ndTaxDisplay4, ndTaxDisplay5 As XmlNode
            Dim ndTaxCode1, ndTaxCode2, ndTaxCode3, ndTaxCode4, ndTaxCode5 As XmlNode
            Dim ndTaxAmount1, ndTaxAmount2, ndTaxAmount3, ndTaxAmount4, ndTaxAmount5 As XmlNode
            'My new Node for SmartcardNumber 28.5.2013
            Dim ndSmartcardNumber As XmlNode
            Dim ndBillingContactTitle, ndBillingContactInitials, ndBillingContactForename, ndBillingContactSurname, ndBillingContactCompanyName, ndBillingContactEmail As XmlNode


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
            ndContactEmail = xmlDoc.CreateElement("ContactEmail")
            ndSpecialInstructions1 = xmlDoc.CreateElement("SpecialInstructions1")
            ndSpecialInstructions2 = xmlDoc.CreateElement("SpecialInstructions2")
            ndShippedDate = xmlDoc.CreateElement("ShippedDate")
            ndProjectedDeliveryDate = xmlDoc.CreateElement("ProjectedDeliveryDate")
            ndDeliveryType = xmlDoc.CreateElement("DeliveryType")
            ndDeliveryTypeDescription = xmlDoc.CreateElement("DeliveryTypeDescription")
            ndBillingInformation = xmlDoc.CreateElement("BillingAddressInformation")
            ndBillingContactName = xmlDoc.CreateElement("ContactName")
            ndBillingContactTitle = xmlDoc.CreateElement("ContactTitle")
            ndBillingContactInitials = xmlDoc.CreateElement("ContactInitials")
            ndBillingContactForename = xmlDoc.CreateElement("ContactForename")
            ndBillingContactSurname = xmlDoc.CreateElement("ContactSurname")
            ndBillingContactCompanyName = xmlDoc.CreateElement("ContactCompanyName")
            ndBillingContactEmail = xmlDoc.CreateElement("ContactEmail")
            ndBillingAddressLine1 = xmlDoc.CreateElement("AddressLine1")
            ndBillingAddressLine2 = xmlDoc.CreateElement("AddressLine2")
            ndBillingAddressLine3 = xmlDoc.CreateElement("AddressLine3")
            ndBillingAddressLine4 = xmlDoc.CreateElement("AddressLine4")
            ndBillingAddressLine5 = xmlDoc.CreateElement("AddressLine5")
            ndBillingPostCode = xmlDoc.CreateElement("PostCode")
            ndBillingCountry = xmlDoc.CreateElement("Country")
            ndMobileNumber = xmlDoc.CreateElement("MobileNumber")
            ndTelephoneNumber = xmlDoc.CreateElement("TelephoneNumber")
            ndWorkNumber = xmlDoc.CreateElement("WorkNumber")
            ndFaxNumber = xmlDoc.CreateElement("FaxNumber")
            ndOtherNumber = xmlDoc.CreateElement("OtherNumber")
            ndTrackingNo = xmlDoc.CreateElement("TrackingNo")
            ndLanguage = xmlDoc.CreateElement("Language")
            ndWarehouse = xmlDoc.CreateElement("Warehouse")
            ndCurrency = xmlDoc.CreateElement("Currency")
            ndGiftMessage = xmlDoc.CreateElement("GiftMessage")
            ndIsGift = xmlDoc.CreateElement("IsGift")
            ndRecipientName = xmlDoc.CreateElement("RecipientName")
            ndGiftText = xmlDoc.CreateElement("GiftText")
            ndDeliveryNotes = xmlDoc.CreateElement("DeliveryNotes")
            ndShippingCode = xmlDoc.CreateElement("ShippingCode")
            ndPurchaseOrder = xmlDoc.CreateElement("PurchaseOrder")
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
            ndContactName.InnerText = row("CONTACT_NAME").ToString

            ndAddressLine1.InnerText = row("ADDRESS_LINE_1").ToString
            ndAddressLine2.InnerText = row("ADDRESS_LINE_2").ToString
            ndAddressLine3.InnerText = row("ADDRESS_LINE_3").ToString
            ndAddressLine4.InnerText = row("ADDRESS_LINE_4").ToString
            ndAddressLine5.InnerText = row("ADDRESS_LINE_5").ToString
            ndPostCode.InnerText = row("POSTCODE").ToString
            ndCountry.InnerText = row("COUNTRY").ToString
            ndContactEmail.InnerText = row("CONTACT_EMAIL").ToString
            ndSpecialInstructions1.InnerText = row("SPECIAL_INSTRUCTIONS_1").ToString
            ndSpecialInstructions2.InnerText = row("SPECIAL_INSTRUCTIONS_2").ToString
            ndShippedDate.InnerText = row("SHIPPED_DATE").ToString
            ndProjectedDeliveryDate.InnerText = row("PROJECTED_DELIVERY_DATE").ToString
            ndDeliveryType.InnerText = row("DELIVERY_TYPE").ToString
            ndDeliveryTypeDescription.InnerText = row("DELIVERY_TYPE_DESCRIPTION").ToString
            ndBillingContactName.InnerText = row("BILLING_FULL_NAME").ToString
            ndBillingContactTitle.InnerText = row("BILLING_CONTACT_TITLE").ToString
            ndBillingContactInitials.InnerText = row("BILLING_CONTACT_INITIALS").ToString
            ndBillingContactForename.InnerText = row("BILLING_CONTACT_FORENAME").ToString
            ndBillingContactSurname.InnerText = row("BILLING_CONTACT_SURNAME").ToString
            ndBillingContactCompanyName.InnerText = row("BILLING_CONTACT_COMPANYNAME").ToString
            ndBillingContactEmail.InnerText = row("BILLING_CONTACT_EMAIL").ToString
            ndBillingAddressLine1.InnerText = row("BILLING_ADDRESS_LINE_1").ToString
            ndBillingAddressLine2.InnerText = row("BILLING_ADDRESS_LINE_2").ToString
            ndBillingAddressLine3.InnerText = row("BILLING_ADDRESS_LINE_3").ToString
            ndBillingAddressLine4.InnerText = row("BILLING_ADDRESS_LINE_4").ToString
            ndBillingAddressLine5.InnerText = row("BILLING_ADDRESS_LINE_5").ToString
            ndBillingPostCode.InnerText = row("BILLING_POST_CODE").ToString
            ndBillingCountry.InnerText = row("BILLING_COUNTRY").ToString
            ndMobileNumber.InnerText = row("MOBILE_NUMBER").ToString
            ndTelephoneNumber.InnerText = row("TELEPHONE_NUMBER").ToString
            ndWorkNumber.InnerText = row("WORK_NUMBER").ToString
            ndFaxNumber.InnerText = row("FAX_NUMBER").ToString
            ndOtherNumber.InnerText = row("OTHER_NUMBER").ToString

            ndTrackingNo.InnerText = row("TRACKING_NO").ToString
            ndLanguage.InnerText = row("LANGUAGE").ToString
            ndWarehouse.InnerText = row("WAREHOUSE").ToString
            ndCurrency.InnerText = row("CURRENCY").ToString
            ndIsGift.InnerText = row("GIFT_MESSAGE").ToString
            ndRecipientName.InnerText = row("RECIPIENT_NAME").ToString
            ndGiftText.InnerText = row("MESSAGE").ToString
            ndDeliveryNotes.InnerText = row("COMMENT").ToString
            ndShippingCode.InnerText = row("SHIPPING_CODE").ToString
            ndPurchaseOrder.InnerText = row("PURCHASE_ORDER").ToString
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
            ndAddressingInformation.AppendChild(ndContactEmail)
            ndAddressingInformation.AppendChild(ndSpecialInstructions1)
            ndAddressingInformation.AppendChild(ndSpecialInstructions2)
            ndAddressingInformation.AppendChild(ndShippedDate)
            ndAddressingInformation.AppendChild(ndProjectedDeliveryDate)
            ndAddressingInformation.AppendChild(ndDeliveryType)
            ndAddressingInformation.AppendChild(ndDeliveryTypeDescription)
            ndBillingInformation.AppendChild(ndBillingContactName)
            ndBillingInformation.AppendChild(ndBillingContactTitle)
            ndBillingInformation.AppendChild(ndBillingContactInitials)
            ndBillingInformation.AppendChild(ndBillingContactForename)
            ndBillingInformation.AppendChild(ndBillingContactSurname)
            ndBillingInformation.AppendChild(ndBillingContactCompanyName)
            ndBillingInformation.AppendChild(ndBillingContactEmail)
            ndBillingInformation.AppendChild(ndBillingAddressLine1)
            ndBillingInformation.AppendChild(ndBillingAddressLine2)
            ndBillingInformation.AppendChild(ndBillingAddressLine3)
            ndBillingInformation.AppendChild(ndBillingAddressLine4)
            ndBillingInformation.AppendChild(ndBillingAddressLine5)
            ndBillingInformation.AppendChild(ndBillingPostCode)
            ndBillingInformation.AppendChild(ndBillingCountry)
            ndBillingInformation.AppendChild(ndMobileNumber)
            ndBillingInformation.AppendChild(ndTelephoneNumber)
            ndBillingInformation.AppendChild(ndWorkNumber)
            ndBillingInformation.AppendChild(ndFaxNumber)
            ndBillingInformation.AppendChild(ndOtherNumber)
            ndOrderHeaderInformation.AppendChild(ndAddressingInformation)
            ndOrderHeaderInformation.AppendChild(ndBillingInformation)
            ndOrderHeaderInformation.AppendChild(ndTrackingNo)
            ndOrderHeaderInformation.AppendChild(ndLanguage)
            ndOrderHeaderInformation.AppendChild(ndWarehouse)
            ndOrderHeaderInformation.AppendChild(ndCurrency)
            ndOrderHeaderInformation.AppendChild(ndShippingCode)
            ndOrderHeaderInformation.AppendChild(ndGiftMessage)
            ndGiftMessage.AppendChild(ndIsGift)
            ndGiftMessage.AppendChild(ndRecipientName)
            ndGiftMessage.AppendChild(ndGiftText)
            ndOrderHeaderInformation.AppendChild(ndDeliveryNotes)
            ndOrderHeaderInformation.AppendChild(ndPurchaseOrder)
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

#End Region

    End Class

End Namespace