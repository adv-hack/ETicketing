Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Detail Response
'
'       Date                        8th Nov 2006
'
'       Author                      Andy White
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSOD- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlOrderDetailResponse
        Inherits XmlResponse
        ' 
        Private ndHeaderRoot, ndHeaderRootHeader As XmlNode
        '---------------------------------------------------------------------------------------------
        '   Order Header <OrderInfo> <OrderNumbers>
        '
        Private ndHeader, ndOrderNumbers, ndBranchOrderNumber, ndCustomerPO, ndErrorStatus, ndEndUserPO, _
                ndOrderWeight, ndOrderEntryDate, ndInvoiceDate, ndPromiseDate, ndOrderType, ndFulfillmentFlag, _
                ndShipComplete, ndHoldReason, ndTermsCode, ndResellerNBR, ndNumberOfCartons, _
                ndCreditMemoReasonCode, ndConfigFlag, ndSplitBillToSwitch, ndSplitFromOrderNumber, _
                ndRMACode, ndCreditCardSW, ndGovEndUserType, ndEntryMethod, ndTermID, ndBackOrderStatus, _
                ndShippableSW, ndConfigTimeStamp, ndSelSrcSlsHdr, ndSelSrcAcctnoHdr, ndOECarrier, _
                ndFrtOutCode, ndProNbrSW, ndProNbr As XmlNode

        Private ndOrderTotals, ndSalesTotal, ndFreightTotal, ndTaxTotal, ndSalePlusTax, ndGrandTotal, _
                ndCODAmount, ndDiscountAmount, ndCurrencyCode, ndCompanyCurrency, ndCurrencyRate As XmlNode

        Private ndShipToAddress, ndShipToName, ndShipToAttention, ndShipToAddress1, _
                ndShipToAddress2, ndShipToAddress3, ndShipToAddress4, _
                ndShipToCity, ndShipToProvince, ndShipToPostalCode, ndShipToCountry As XmlNode

        Private ndBillToAddress, ndBillToName, ndBillToAttention, ndBillToAddress1, _
                ndBillToAddress2, ndBillToAddress3, ndBillToAddress4, _
                ndBillToCity, ndBillToProvince, ndBillToPostalCode, ndBillToCountry As XmlNode

        Private ndShipFromBranch, ndOrderStatus, ndCarrier, ndOrderShipDate, ndOrderSuffix, _
                ndLineInformation As XmlNode
        '---------------------------------------------------------------------------------------------
        '   Order Details  <LineInformation> <ProductLine>
        '
        Private ndProductLine, ndSKU, ndUnitPrice, ndWestCoastLineNumber, ndCustomerLineNumber, ndOrderQuantity, _
                ndBackOrderQuantity, ndBackOrderETADate, ndSKUDescription, ndShipQuantity, _
                ndComponentQty, ndNonWayPromiseDate, ndUnitOfMeasure, ndResellerUnitPrice, _
                ndExtendedLineSales, ndLineSalesTotal, ndLineTerms, ndReserveSequenceNbr, _
                ndVendorPartNumber, ndVendorName, ndSelSrcSls, ndSelSrcAcctno, _
                ndFreeItemSwitch, ndSystemComponentSwitch, ndCustomerSKU As XmlNode

        Private ndConfigInformation, ndConfigIndicator, ndConfigStatus, ndConfigAssemblyCode, _
                ndConfigLabCode, ndConfigOnHoldSw, ndConfigPcrCnt, ndConfigPchCnt, ndConfigStgCnt, _
                ndConfigSthCnt, ndConfigWipCnt, ndConfigQaaCnt, ndConfigQahCnt, ndConfigBinCnt, _
                ndConfigOshCnt, ndConfigHoldReasonText As XmlNode

        Private ndSkuSerialNumber, ndSerialNumber As XmlNode

        Private atErrorNumber, atShipTo, atBillTo, atShipFromBranch, atCarrier, atXmlNsXsi As XmlAttribute
        '---------------------------------------------------------------------------------------------
        Private CarrierCode As String
        '---------------------------------------------------------------------------------------------

        Protected Overrides Sub InsertBodyV1()
            '--------------------------------------------------------------------------------------
            '   Seperate the tables out of the ResultSet  
            ' 
            Try
                '--------------------------------------------------------------------------------------
                With MyBase.xmlDoc
                    ndHeader = .CreateElement("OrderInfo")
                    If Not Err.HasError Then

                        dtHeader = ResultDataSet.Tables.Item(0)
                        dtDetail = ResultDataSet.Tables.Item(1)
                        dtText = ResultDataSet.Tables.Item(2)
                        dtCarrier = ResultDataSet.Tables.Item(3)
                        dtPackage = ResultDataSet.Tables.Item(4)
                        dtProduct = ResultDataSet.Tables.Item(5)

                        Err2 = InsertHeader()
                    End If
                    '--------------------------------------------------------------------------------------
                    '   Insert the fragment into the XML document
                    '
                    Const c1 As String = "//"                               ' Constants are faster at run time
                    Const c2 As String = "/TransactionHeader"
                    '
                    ndHeaderRoot = .SelectSingleNode(c1 & RootElement())
                    ndHeaderRootHeader = .SelectSingleNode(c1 & RootElement() & c2)
                    ndHeaderRoot.InsertAfter(ndHeader, ndHeaderRootHeader)

                    'Insert the XSD reference & namespace as an attribute within the root node
                    atXmlNsXsi = CreateNamespaceAttribute()
                    ndHeaderRoot.Attributes.Append(atXmlNsXsi)
                End With
            Catch ex As Exception
            End Try
        End Sub

        Private Function InsertHeader() As ErrorObj
            Dim err2 As ErrorObj = Nothing
            '----------------------------------------------------------------------------------
            Dim iCounter As Integer = 0
            Dim dr As DataRow
            Try
                With MyBase.xmlDoc
                    '-----------------------------------------------------------------------------
                    If Not dtHeader Is Nothing AndAlso dtHeader.Rows.Count > 0 Then
                        For Each dr In dtHeader.Rows
                            '------------------------------------------------------
                            err2 = CreateHeader()
                            With dr
                                ndBranchOrderNumber.InnerText = .Item("BranchOrderNumber").ToString
                                If dr("BranchOrderNumber") <> "Not Found" Then
                                    OrderNumber = .Item("OrderNumber").ToString
                                    ndCustomerPO.InnerText = .Item("CustomerPO").ToString
                                    '
                                    ndEndUserPO.InnerText = .Item("EndUserPO").ToString
                                    ndOrderWeight.InnerText = .Item("OrderWeight").ToString
                                    ndOrderEntryDate.InnerText = .Item("OrderEntryDate").ToString
                                    ndInvoiceDate.InnerText = .Item("InvoiceDate").ToString
                                    ndPromiseDate.InnerText = .Item("PromiseDate").ToString
                                    ndOrderType.InnerText = .Item("OrderType").ToString
                                    ndFulfillmentFlag.InnerText = .Item("FulfillmentFlag").ToString
                                    ndShipComplete.InnerText = .Item("ShipComplete").ToString
                                    ndHoldReason.InnerText = .Item("HoldReason").ToString
                                    ndTermsCode.InnerText = .Item("TermsCode").ToString
                                    ndResellerNBR.InnerText = .Item("ResellerNBR").ToString
                                    '    ndNumberOfCartons.InnerText = .Item("NumberOfCartons").ToString
                                    ndNumberOfCartons.InnerText = String.Empty
                                    ndCreditMemoReasonCode.InnerText = .Item("CreditMemoReasonCode").ToString
                                    ndConfigFlag.InnerText = .Item("ConfigFlag").ToString
                                    ndSplitBillToSwitch.InnerText = .Item("SplitBillToSwitch").ToString
                                    ndSplitFromOrderNumber.InnerText = .Item("SplitFromOrderNumber").ToString
                                    ndRMACode.InnerText = .Item("RMACode").ToString
                                    ndCreditCardSW.InnerText = .Item("CreditCardSW").ToString
                                    ndGovEndUserType.InnerText = .Item("GovEndUserType").ToString
                                    ndEntryMethod.InnerText = .Item("EntryMethod").ToString
                                    ndTermID.InnerText = .Item("TermID").ToString
                                    ndBackOrderStatus.InnerText = .Item("BackOrderStatus").ToString
                                    ndShippableSW.InnerText = .Item("ShippableSW").ToString
                                    ndConfigTimeStamp.InnerText = .Item("ConfigTimeStamp").ToString
                                    ndSelSrcSlsHdr.InnerText = .Item("SelSrcSlsHdr").ToString
                                    ndSelSrcAcctnoHdr.InnerText = .Item("SelSrcAcctnoHdr").ToString
                                    ndOECarrier.InnerText = .Item("OECarrier").ToString
                                    ndFrtOutCode.InnerText = .Item("FrtOutCode").ToString
                                    ndProNbrSW.InnerText = .Item("ProNbrSW").ToString
                                    ndProNbr.InnerText = .Item("ProNbr").ToString
                                    '-------------------------------
                                    '   OrderTotals
                                    '
                                    ndSalesTotal.InnerText = .Item("SalesTotal").ToString
                                    ndFreightTotal.InnerText = .Item("FreightTotal").ToString
                                    ndTaxTotal.InnerText = .Item("TaxTotal").ToString
                                    ndSalePlusTax.InnerText = .Item("SalePlusTax").ToString
                                    ndGrandTotal.InnerText = .Item("GrandTotal").ToString
                                    ndCODAmount.InnerText = .Item("CODAmount").ToString
                                    ndDiscountAmount.InnerText = .Item("DiscountAmount").ToString
                                    ndCurrencyCode.InnerText = .Item("CurrencyCode").ToString
                                    ndCompanyCurrency.InnerText = .Item("CompanyCurrency").ToString
                                    ndCurrencyRate.InnerText = .Item("CurrencyRate").ToString
                                    '-------------------------------
                                    '   Address Type = "ShipTo" 
                                    '
                                    ndShipToName.InnerText = .Item("ShipToName").ToString
                                    ndShipToAttention.InnerText = .Item("ShipToAttention").ToString
                                    ndShipToAddress1.InnerText = .Item("ShipToAddress1").ToString
                                    ndShipToAddress2.InnerText = .Item("ShipToAddress2").ToString
                                    ndShipToAddress3.InnerText = .Item("ShipToAddress3").ToString
                                    ndShipToAddress4.InnerText = .Item("ShipToAddress4").ToString
                                    ndShipToCity.InnerText = .Item("ShipToCity").ToString
                                    ndShipToProvince.InnerText = .Item("ShipToProvince").ToString
                                    ndShipToPostalCode.InnerText = .Item("ShipToPostalCode").ToString
                                    ndShipToCountry.InnerText = .Item("ShipToCountry").ToString
                                    '-------------------------------
                                    '   Address Type = "BillTo" 
                                    '
                                    ndBillToName.InnerText = .Item("BillToName").ToString
                                    ndBillToAttention.InnerText = .Item("BillToAttention").ToString
                                    ndBillToAddress1.InnerText = .Item("BillToAddress1").ToString
                                    ndBillToAddress2.InnerText = .Item("BillToAddress2").ToString
                                    ndBillToAddress3.InnerText = .Item("BillToAddress3").ToString
                                    ndBillToAddress4.InnerText = .Item("BillToAddress4").ToString
                                    ndBillToCity.InnerText = .Item("BillToCity").ToString
                                    ndBillToProvince.InnerText = .Item("BillToProvince").ToString
                                    ndBillToPostalCode.InnerText = .Item("BillToPostalCode").ToString
                                    ndBillToCountry.InnerText = .Item("BillToCountry").ToString
                                    '-------------------------------
                                    ndShipFromBranch.InnerText = .Item("ShipFromBranch").ToString
                                    atShipFromBranch.Value = .Item("ShipFromBranchNumber").ToString
                                    Select Case dr("OrderStatus").ToString
                                        Case Is = "C"
                                            ndOrderStatus.InnerText = "Complete"
                                        Case Is = "X"
                                            ndOrderStatus.InnerText = "Cancelled"
                                        Case Else
                                            ndOrderStatus.InnerText = "Open"
                                    End Select
                                    '      ndOrderStatus.InnerText = .Item("OrderStatus").ToString
                                    ndCarrier.InnerText = .Item("CarrierCodeDescription").ToString
                                    atCarrier.Value = .Item("CarrierCode").ToString
                                    If CType(.Item("ItemsShipped"), Boolean) Then
                                        ndOrderShipDate.InnerText = .Item("ShippedDate").ToString
                                    Else
                                        ndOrderShipDate.InnerText = String.Empty
                                    End If

                                End If
                            End With
                            '-----------------------------------------------------------
                            iCounter += 1
                            ndErrorStatus = .CreateElement("ErrorStatus")
                            If Not err2 Is Nothing Then
                                ndErrorStatus.InnerText = err2.ItemErrorStatus(iCounter)
                                atErrorNumber = .CreateAttribute("ErrorNumber")
                                atErrorNumber.Value = err2.ItemErrorCode(iCounter)
                                ndErrorStatus.Attributes.Append(atErrorNumber)
                            ElseIf Not Err Is Nothing Then
                                ndErrorStatus.InnerText = Err.ItemErrorStatus(iCounter)
                                atErrorNumber = .CreateAttribute("ErrorNumber")
                                atErrorNumber.Value = Err.ItemErrorCode(iCounter)
                                ndErrorStatus.Attributes.Append(atErrorNumber)
                            End If
                            '-----------------------------------------------------------
                            err2 = AppendHeader()
                            If dr("BranchOrderNumber") <> "Not Found" Then _
                                err2 = InsertDetails()
                        Next dr
                    Else
                        '----------------------------------------------------------
                        '   No order header, write dummy
                        '
                        err2 = CreateHeader()
                        err2 = AppendHeader()
                        err2 = CreateDetails()
                        err2 = AppendDetails()
                    End If
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Insert Order Header Nodes"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOD-14"
                    .HasError = True
                End With
            End Try
            Return err2
        End Function
        Private Function CreateHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndOrderNumbers = .CreateElement("OrderNumbers")
                    ndBranchOrderNumber = .CreateElement("BranchOrderNumber")
                    ndCustomerPO = .CreateElement("CustomerPO")
                    '
                    ndEndUserPO = .CreateElement("EndUserPO")
                    ndOrderWeight = .CreateElement("OrderWeight")
                    ndOrderEntryDate = .CreateElement("OrderEntryDate")
                    ndInvoiceDate = .CreateElement("InvoiceDate")
                    ndPromiseDate = .CreateElement("PromiseDate")
                    ndOrderType = .CreateElement("OrderType")
                    ndFulfillmentFlag = .CreateElement("FulfillmentFlag")
                    ndShipComplete = .CreateElement("ShipComplete")
                    ndHoldReason = .CreateElement("HoldReason")
                    ndTermsCode = .CreateElement("TermsCode")
                    ndResellerNBR = .CreateElement("ResellerNBR")
                    ndNumberOfCartons = .CreateElement("NumberOfCartons")
                    ndCreditMemoReasonCode = .CreateElement("CreditMemoReasonCode")
                    ndConfigFlag = .CreateElement("ConfigFlag")
                    ndSplitBillToSwitch = .CreateElement("SplitBillToSwitch")
                    ndSplitFromOrderNumber = .CreateElement("SplitFromOrderNumber")
                    ndRMACode = .CreateElement("RMACode")
                    ndCreditCardSW = .CreateElement("CreditCardSW")
                    ndGovEndUserType = .CreateElement("GovEndUserType")
                    ndEntryMethod = .CreateElement("EntryMethod")
                    ndTermID = .CreateElement("TermID")
                    ndBackOrderStatus = .CreateElement("BackOrderStatus")
                    ndShippableSW = .CreateElement("ShippableSW")
                    ndConfigTimeStamp = .CreateElement("ConfigTimeStamp")
                    ndSelSrcSlsHdr = .CreateElement("SelSrcSlsHdr")
                    ndSelSrcAcctnoHdr = .CreateElement("SelSrcAcctnoHdr")
                    ndOECarrier = .CreateElement("OECarrier")
                    ndFrtOutCode = .CreateElement("FrtOutCode")
                    ndProNbrSW = .CreateElement("ProNbrSW")
                    ndProNbr = .CreateElement("ProNbr")
                    '-------------------------------
                    ndOrderTotals = .CreateElement("OrderTotals")
                    ndSalesTotal = .CreateElement("SalesTotal")
                    ndFreightTotal = .CreateElement("FreightTotal")
                    ndTaxTotal = .CreateElement("TaxTotal")
                    ndSalePlusTax = .CreateElement("SalePlusTax")
                    ndGrandTotal = .CreateElement("GrandTotal")
                    ndCODAmount = .CreateElement("CODAmount")
                    ndDiscountAmount = .CreateElement("DiscountAmount")
                    ndCurrencyCode = .CreateElement("CurrencyCode")
                    ndCompanyCurrency = .CreateElement("CompanyCurrency")
                    ndCurrencyRate = .CreateElement("CurrencyRate")
                    '-------------------------------
                    ' Address Type = "ShipTo" 
                    '
                    ndShipToAddress = .CreateElement("Address")
                    atShipTo = .CreateAttribute("Type")
                    atShipTo.Value = "ShipTo"
                    ndShipToAddress.Attributes.Append(atShipTo)
                    ndShipToName = .CreateElement("ShipToName")
                    ndShipToAttention = .CreateElement("ShipToAttention")
                    ndShipToAddress1 = .CreateElement("ShipToAddressLine1")
                    ndShipToAddress2 = .CreateElement("ShipToAddressLine2")
                    ndShipToAddress3 = .CreateElement("ShipToAddressLine3")
                    ndShipToAddress4 = .CreateElement("ShipToAddressLine4")
                    ndShipToCity = .CreateElement("ShipToCity")
                    ndShipToProvince = .CreateElement("ShipToProvince")
                    ndShipToPostalCode = .CreateElement("ShipToPostalCode")
                    ndShipToCountry = .CreateElement("ShipToCountryCode")
                    '-------------------------------
                    ' Address Type = "BillTo" 
                    '
                    ndBillToAddress = .CreateElement("Address")
                    atBillTo = .CreateAttribute("Type")
                    atBillTo.Value = "BillTo"
                    ndBillToAddress.Attributes.Append(atBillTo)
                    ndBillToName = .CreateElement("BillToName")
                    ndBillToAttention = .CreateElement("BillToAttention")
                    ndBillToAddress1 = .CreateElement("BillToAddressLine1")
                    ndBillToAddress2 = .CreateElement("BillToAddressLine2")
                    ndBillToAddress3 = .CreateElement("BillToAddressLine3")
                    ndBillToAddress4 = .CreateElement("BillToAddressLine4")
                    ndBillToCity = .CreateElement("BillToCity")
                    ndBillToProvince = .CreateElement("BillToProvince")
                    ndBillToPostalCode = .CreateElement("BillToPostalCode")
                    ndBillToCountry = .CreateElement("BillToCountryCode")
                    '-------------------------------
                    ndShipFromBranch = .CreateElement("ShipFromBranch")
                    atShipFromBranch = .CreateAttribute("Number")
                    ndShipFromBranch.Attributes.Append(atShipFromBranch)
                    '-------------------------------
                    ndOrderStatus = .CreateElement("OrderStatus")
                    ndCarrier = .CreateElement("Carrier")
                    atCarrier = .CreateAttribute("Code")
                    ndCarrier.Attributes.Append(atCarrier)
                    ndOrderShipDate = .CreateElement("OrderShipDate")
                    '----------------------------------------------------------
                    ndOrderSuffix = .CreateElement("OrderSuffix")
                    ndLineInformation = .CreateElement("LineInformation")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOD-15"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                ndHeader.AppendChild(ndOrderNumbers)
                With ndOrderNumbers
                    .AppendChild(ndBranchOrderNumber)
                    .AppendChild(ndCustomerPO)
                    .AppendChild(ndErrorStatus)
                    '
                    .AppendChild(ndEndUserPO)
                    .AppendChild(ndOrderWeight)
                    .AppendChild(ndOrderEntryDate)
                    .AppendChild(ndInvoiceDate)
                    .AppendChild(ndPromiseDate)
                    .AppendChild(ndOrderType)
                    .AppendChild(ndFulfillmentFlag)
                    .AppendChild(ndShipComplete)
                    .AppendChild(ndHoldReason)
                    .AppendChild(ndTermsCode)
                    .AppendChild(ndResellerNBR)
                    .AppendChild(ndNumberOfCartons)
                    .AppendChild(ndCreditMemoReasonCode)
                    .AppendChild(ndConfigFlag)
                    .AppendChild(ndSplitBillToSwitch)
                    .AppendChild(ndSplitFromOrderNumber)
                    .AppendChild(ndRMACode)
                    .AppendChild(ndCreditCardSW)
                    .AppendChild(ndGovEndUserType)
                    .AppendChild(ndEntryMethod)
                    .AppendChild(ndTermID)
                    .AppendChild(ndBackOrderStatus)
                    .AppendChild(ndShippableSW)
                    .AppendChild(ndConfigTimeStamp)
                    .AppendChild(ndSelSrcSlsHdr)
                    .AppendChild(ndSelSrcAcctnoHdr)
                    .AppendChild(ndOECarrier)
                    .AppendChild(ndFrtOutCode)
                    .AppendChild(ndProNbrSW)
                    .AppendChild(ndProNbr)
                    '-------------------------------
                    .AppendChild(ndOrderTotals)
                    With ndOrderTotals
                        .AppendChild(ndSalesTotal)
                        .AppendChild(ndFreightTotal)
                        .AppendChild(ndTaxTotal)
                        .AppendChild(ndSalePlusTax)
                        .AppendChild(ndGrandTotal)
                        .AppendChild(ndCODAmount)
                        .AppendChild(ndDiscountAmount)
                        .AppendChild(ndCurrencyCode)
                        .AppendChild(ndCompanyCurrency)
                        .AppendChild(ndCurrencyRate)
                    End With
                    '-------------------------------
                    ' Address Type  ShipTo 
                    '
                    .AppendChild(ndShipToAddress)
                    With ndShipToAddress
                        .AppendChild(ndShipToName)
                        .AppendChild(ndShipToAttention)
                        .AppendChild(ndShipToAddress1)
                        .AppendChild(ndShipToAddress2)
                        .AppendChild(ndShipToAddress3)
                        .AppendChild(ndShipToAddress4)
                        .AppendChild(ndShipToCity)
                        .AppendChild(ndShipToProvince)
                        .AppendChild(ndShipToPostalCode)
                        .AppendChild(ndShipToCountry)
                    End With
                    '-------------------------------
                    ' Address Type  BillTo 
                    '
                    .AppendChild(ndBillToAddress)
                    With ndBillToAddress
                        .Attributes.Append(atBillTo)
                        .AppendChild(ndBillToName)
                        .AppendChild(ndBillToAttention)
                        .AppendChild(ndBillToAddress1)
                        .AppendChild(ndBillToAddress2)
                        .AppendChild(ndBillToAddress3)
                        .AppendChild(ndBillToAddress4)
                        .AppendChild(ndBillToCity)
                        .AppendChild(ndBillToProvince)
                        .AppendChild(ndBillToPostalCode)
                        .AppendChild(ndBillToCountry)
                    End With
                    '-------------------------------
                    .AppendChild(ndShipFromBranch)
                    .AppendChild(ndOrderStatus)
                    .AppendChild(ndCarrier)
                    .AppendChild(ndOrderShipDate)
                    '----------------------------------------------------------
                    .AppendChild(ndOrderSuffix)
                    ndOrderSuffix.AppendChild(ndLineInformation)
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Append Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOD-16"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            '
            Dim dr As DataRow
            '---------------------------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    If Not dtDetail Is Nothing AndAlso dtDetail.Rows.Count > 0 Then
                        For Each dr In dtDetail.Rows
                            If dr("OrderNumber").Equals(OrderNumber) Then
                                err = CreateDetails()
                                '-----------------------------------------------------------------------------
                                ndSKU.InnerText = dr("SKU").ToString.Trim
                                ndUnitPrice.InnerText = dr("UnitPrice").ToString
                                ndWestCoastLineNumber.InnerText = dr("WestCoastLineNumber").ToString
                                ndCustomerLineNumber.InnerText = dr("CustomerLineNumber").ToString
                                ndOrderQuantity.InnerText = dr("OrderQuantity").ToString
                                ndBackOrderQuantity.InnerText = dr("BackOrderQuantity").ToString
                                ndBackOrderETADate.InnerText = dr("BackOrderETADate").ToString
                                ndSKUDescription.InnerText = dr("SKUDescription").ToString.Trim
                                ndShipQuantity.InnerText = dr("ShipQuantity").ToString
                                ndComponentQty.InnerText = dr("ComponentQty").ToString
                                ndNonWayPromiseDate.InnerText = dr("NonWayPromiseDate").ToString
                                ndUnitOfMeasure.InnerText = dr("UnitOfMeasure").ToString
                                ndResellerUnitPrice.InnerText = dr("ResellerUnitPrice").ToString
                                ndExtendedLineSales.InnerText = dr("ExtendedLineSales").ToString
                                ndLineSalesTotal.InnerText = dr("LineSalesTotal").ToString
                                ndLineTerms.InnerText = dr("LineTerms").ToString
                                ndReserveSequenceNbr.InnerText = dr("ReserveSequenceNbr").ToString
                                ndVendorPartNumber.InnerText = dr("VendorPartNumber").ToString
                                ndVendorName.InnerText = dr("VendorName").ToString
                                ndSelSrcSls.InnerText = dr("SelSrcSls").ToString
                                ndSelSrcAcctno.InnerText = dr("SelSrcAcctno").ToString
                                ndFreeItemSwitch.InnerText = dr("FreeItemSwitch").ToString
                                ndSystemComponentSwitch.InnerText = dr("SystemComponentSwitch").ToString
                                ndCustomerSKU.InnerText = dr("CustomerSKU").ToString
                                '-----------------------------------------------------------------------------
                                ndConfigIndicator.InnerText = dr("ConfigIndicator").ToString
                                ndConfigStatus.InnerText = dr("ConfigStatus").ToString
                                ndConfigAssemblyCode.InnerText = dr("ConfigAssemblyCode").ToString
                                ndConfigLabCode.InnerText = dr("ConfigLabCode").ToString
                                ndConfigOnHoldSw.InnerText = dr("ConfigOnHoldSw").ToString
                                ndConfigPcrCnt.InnerText = dr("ConfigPcrCnt").ToString
                                ndConfigPchCnt.InnerText = dr("ConfigPchCnt").ToString
                                ndConfigStgCnt.InnerText = dr("ConfigStgCnt").ToString
                                ndConfigSthCnt.InnerText = dr("ConfigSthCnt").ToString
                                ndConfigWipCnt.InnerText = dr("ConfigWipCnt").ToString
                                ndConfigQaaCnt.InnerText = dr("ConfigQaaCnt").ToString
                                ndConfigQahCnt.InnerText = dr("ConfigQahCnt").ToString
                                ndConfigBinCnt.InnerText = dr("ConfigBinCnt").ToString
                                ndConfigOshCnt.InnerText = dr("ConfigOshCnt").ToString
                                ndConfigHoldReasonText.InnerText = dr("ConfigHoldReasonText").ToString
                                '-----------------------------------------------------------------------------
                                err = AppendDetails()
                                '-----------------------------------------------------------------------------
                                Dim dr2 As DataRow
                                For Each dr2 In dtProduct.Rows
                                    If dr2("OrderNumber").Equals(OrderNumber) _
                                    And dr2("OrderLine").Equals(dr("LineNumber").ToString) Then
                                        ndSerialNumber = MyBase.xmlDoc.CreateElement("SerialNumber")
                                        ndSerialNumber.InnerText = dr2("SerialNumber")
                                        ndSkuSerialNumber.AppendChild(ndSerialNumber)
                                    End If
                                Next
                            End If
                        Next dr
                        err = InsertComments()
                    Else
                        '---------------------------------------------------------------------------------
                        '  No data so write dummy
                        '
                        err = CreateDetails()
                        err = AppendDetails()
                        '-----------------------------------------------------------------------------
                    End If
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Insert Order Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOD-17"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreateDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndProductLine = .CreateElement("ProductLine")
                    ndSKU = .CreateElement("SKU")
                    ndUnitPrice = .CreateElement("UnitPrice")
                    ndWestCoastLineNumber = .CreateElement("WestCoastLineNumber")
                    ndCustomerLineNumber = .CreateElement("CustomerLineNumber")
                    ndOrderQuantity = .CreateElement("OrderQuantity")
                    ndBackOrderQuantity = .CreateElement("BackOrderedQuantity")
                    ndBackOrderETADate = .CreateElement("BackOrderETADate")
                    ndSKUDescription = .CreateElement("SKUDescription")
                    ndShipQuantity = .CreateElement("ShipQuantity")
                    ndComponentQty = .CreateElement("ComponentQty")
                    ndNonWayPromiseDate = .CreateElement("NonWayPromiseDate")
                    ndUnitOfMeasure = .CreateElement("UnitOfMeasure")
                    ndResellerUnitPrice = .CreateElement("ResellerUnitPrice")
                    ndExtendedLineSales = .CreateElement("ExtendedLineSales")
                    ndLineSalesTotal = .CreateElement("LineSalesTotal")
                    ndLineTerms = .CreateElement("LineTerms")
                    ndReserveSequenceNbr = .CreateElement("ReserveSequenceNbr")
                    ndVendorPartNumber = .CreateElement("VendorPartNumber")
                    ndVendorName = .CreateElement("VendorName")
                    ndSelSrcSls = .CreateElement("SelSrcSls")
                    ndSelSrcAcctno = .CreateElement("SelSrcAcctno")
                    ndFreeItemSwitch = .CreateElement("FreeItemSwitch")
                    ndSystemComponentSwitch = .CreateElement("SystemComponentSwitch")
                    ndCustomerSKU = .CreateElement("CustomerSKU")
                    '-----------------------------------------------------------------------------
                    ndConfigInformation = .CreateElement("ConfigInformation")
                    ndConfigIndicator = .CreateElement("ConfigIndicator")
                    ndConfigStatus = .CreateElement("ConfigStatus")
                    ndConfigAssemblyCode = .CreateElement("ConfigAssemblyCode")
                    ndConfigLabCode = .CreateElement("ConfigLabCode")
                    ndConfigOnHoldSw = .CreateElement("ConfigOnHoldSw")
                    ndConfigPcrCnt = .CreateElement("ConfigPcrCnt")
                    ndConfigPchCnt = .CreateElement("ConfigPchCnt")
                    ndConfigStgCnt = .CreateElement("ConfigStgCnt")
                    ndConfigSthCnt = .CreateElement("ConfigSthCnt")
                    ndConfigWipCnt = .CreateElement("ConfigWipCnt")
                    ndConfigQaaCnt = .CreateElement("ConfigQaaCnt")
                    ndConfigQahCnt = .CreateElement("ConfigQahCnt")
                    ndConfigBinCnt = .CreateElement("ConfigBinCnt")
                    ndConfigOshCnt = .CreateElement("ConfigOshCnt")
                    ndConfigHoldReasonText = .CreateElement("ConfigHoldReasonText")
                    '-----------------------------------------------------------------------------
                    ndSkuSerialNumber = .CreateElement("SkuSerialNumber")
                    '-----------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOD-18"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With ndProductLine
                    .AppendChild(ndSKU)
                    .AppendChild(ndUnitPrice)
                    .AppendChild(ndWestCoastLineNumber)
                    .AppendChild(ndCustomerLineNumber)
                    .AppendChild(ndOrderQuantity)
                    .AppendChild(ndBackOrderQuantity)
                    .AppendChild(ndBackOrderETADate)
                    .AppendChild(ndSKUDescription)
                    .AppendChild(ndShipQuantity)
                    .AppendChild(ndComponentQty)
                    .AppendChild(ndNonWayPromiseDate)
                    .AppendChild(ndUnitOfMeasure)
                    .AppendChild(ndResellerUnitPrice)
                    .AppendChild(ndExtendedLineSales)
                    .AppendChild(ndLineSalesTotal)
                    .AppendChild(ndLineTerms)
                    .AppendChild(ndReserveSequenceNbr)
                    .AppendChild(ndVendorPartNumber)
                    .AppendChild(ndVendorName)
                    .AppendChild(ndSelSrcSls)
                    .AppendChild(ndSelSrcAcctno)
                    .AppendChild(ndFreeItemSwitch)
                    .AppendChild(ndSystemComponentSwitch)
                    .AppendChild(ndCustomerSKU)
                    '------------------------------------------------------------------
                    .AppendChild(ndConfigInformation)
                    With ndConfigInformation
                        .AppendChild(ndConfigIndicator)
                        .AppendChild(ndConfigStatus)
                        .AppendChild(ndConfigAssemblyCode)
                        .AppendChild(ndConfigLabCode)
                        .AppendChild(ndConfigOnHoldSw)
                        .AppendChild(ndConfigPcrCnt)
                        .AppendChild(ndConfigPchCnt)
                        .AppendChild(ndConfigStgCnt)
                        .AppendChild(ndConfigSthCnt)
                        .AppendChild(ndConfigWipCnt)
                        .AppendChild(ndConfigQaaCnt)
                        .AppendChild(ndConfigQahCnt)
                        .AppendChild(ndConfigBinCnt)
                        .AppendChild(ndConfigOshCnt)
                        .AppendChild(ndConfigHoldReasonText)
                    End With
                    .AppendChild(ndSkuSerialNumber)
                End With
                ndLineInformation.AppendChild(ndProductLine)
            Catch ex As Exception
                Const strError As String = "Failed to Append Order Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOD-19"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertComments() As ErrorObj
            Dim err As ErrorObj = Nothing
            '
            Dim ndCommentLine, ndCommentText, ndCommentLineNumber As XmlNode
            Dim dr As DataRow
            '---------------------------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    If Not dtText Is Nothing AndAlso dtText.Rows.Count > 0 Then
                        For Each dr In dtText.Rows
                            If dr("OrderNumber").Equals(OrderNumber) Then
                                ndCommentLine = .CreateElement("CommentLine")
                                ndCommentText = .CreateElement("CommentText")
                                ndCommentLineNumber = .CreateElement("CommentLineNumber")
                                '
                                ndCommentLineNumber.InnerText = dr("TextLineNumber")
                                ndCommentText.InnerText = dr("Text")
                                '
                                ndCommentLine.AppendChild(ndCommentText)
                                ndCommentLine.AppendChild(ndCommentLineNumber)
                                ndLineInformation.AppendChild(ndCommentLine)
                            End If
                        Next
                    Else
                        ndCommentLine = .CreateElement("CommentLine")
                        ndCommentText = .CreateElement("CommentText")
                        ndCommentLineNumber = .CreateElement("CommentLineNumber")
                        '
                        ndCommentLine.AppendChild(ndCommentText)
                        ndCommentLine.AppendChild(ndCommentLineNumber)
                        ndLineInformation.AppendChild(ndCommentLine)
                    End If
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Insert Order Comment Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOD-13"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace