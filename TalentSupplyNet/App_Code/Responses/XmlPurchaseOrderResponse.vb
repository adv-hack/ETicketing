Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.Data
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with PurchaseOrder responses
'
'       Date                        Feb 2007
'
'       Author                      *** THIS IS WORK-IN-PROGRESS **** 
'
'       © CS Group 2006             All rights reserved.
'
'       Error Number Code base      TTPRSPO- 
'                                   
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlPurchaseOrderResponse
        Inherits XmlResponse

        Private ndDocRoot, ndDocHeaderRoot As XmlNode

        Private ndPurchaseOrders, ndPurchaseOrder, ndPurchaseOrderHeader, ndLOBCode, _
                ndDataStructureName, ndPurchaseOrderNumber, ndVendorNumber, ndVendorType, _
                ndSendThisSession, ndSentOK, ndXMLDocumentName, ndReleaseNumber, _
                ndOrderStatus, ndBuyerID, ndWarehouseID, ndShipToNumber, ndCarrierCode, _
                ndOrderDate, ndExpectedDate, ndRequestedReceiptDate, _
                ndNumberOfLinesOnOrder, ndTotalQuantity, ndTotalEstimatedCost, ndTaxTotal, _
                ndDiscountPercent, ndDiscountAmount, ndTransactionCurrencyCode, _
                ndBaseCurrencyCode, ndTransactionExchangeRate, ndMultyDivideFlag, _
                ndInvoiceNumber, ndSpecialInstructions, ndComments, ndInternal, ndExternal, _
                ndUDF01, ndUDF02, ndUDF03, ndUDF04, ndUDF05, _
                ndUDF06, ndUDF07, ndUDF08, ndUDF09, ndUDF10 As XmlNode

        Private ndDeliveryAddress, ndDeliveryName, ndDeliveryAddress1, _
                ndDeliveryAddress2, ndDeliveryAddress3, ndDeliveryAddress4, _
                ndDeliveryAddress5, ndShipToPostalCode As XmlNode

        Private ndPurchaseOrderLines, ndPurchaseOrderLine, ndLineActionCode, _
                ndLine_OrderStatus, ndLine_BuyerID, ndLine_WarehouseID, _
                ndProductNumber, ndProductDescription, ndUserProductNumber, _
                ndUserProductDescription, ndMeasureCode, ndDeliveryDate, _
                ndQuantityOrdered, ndQuantityReceived, ndQuantityOutstanding, ndEstimatedCost, _
                ndActualSellingPrice, ndSpecialSellingPrice, ndLine_DiscountAmount, _
                ndLine_DiscountPercent, ndLineInstructions As XmlNode

        Private atLineNumber, atDelivery As XmlAttribute
        Protected dtComments As New DataTable
        Const str13 As String = "yyyy-MM-dd"
        Const str14 As String = "HH:mm:ss"
        Protected Overrides Sub InsertBodyV1()
            '------------------------------------------------------------------------------
            '   Seperate the tables out of the ResultSet    
            '
            Try
                With MyBase.xmlDoc
                    ndPurchaseOrders = .CreateElement("PurchaseOrders")
                    If Not Err.HasError Then
                        dtHeader = ResultDataSet.Tables("Header")       ' Header
                        dtDetail = ResultDataSet.Tables("Details")      ' Item
                        dtComments = ResultDataSet.Tables("Comments")   ' Comments
                        Err2 = InsertHeader()
                    End If
                    '--------------------------------------------------------------------------------------
                    '   Insert the fragment into the XML document
                    '
                    Const c1 As String = "//"                               ' Constants are faster at run time
                    Const c2 As String = "/TransactionHeader"
                    '
                    ndDocRoot = .SelectSingleNode(c1 & RootElement())
                    ndDocHeaderRoot = .SelectSingleNode(c1 & RootElement() & c2)
                    ndDocRoot.InsertAfter(ndPurchaseOrders, ndDocHeaderRoot)
                    'Insert the XSD reference & namespace as an attribute within the root node
                    Dim atXmlNsXsi As XmlAttribute = CreateNamespaceAttribute()
                    ndDocRoot.Attributes.Append(atXmlNsXsi)
                End With

            Catch ex As Exception
            End Try

        End Sub

        Private Function InsertHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Dim dRow As DataRow
            Dim sWork As String = String.Empty
            Try
                If Not dtHeader Is Nothing AndAlso dtHeader.Rows.Count > 0 Then
                    For Each dRow In dtHeader.Rows
                        '--------------------------------------------------------------------------
                        err = CreateHeader()
                        '
                        Dim PurchaseOrderNumber As String = dRow("PurchaseOrderNumber")
                        ndPurchaseOrderNumber.InnerText = PurchaseOrderNumber
                        '--------------------------------------------------------------------------
                        ndLOBCode.InnerText = dRow("LOBCode").ToString
                        ndDataStructureName.InnerText = dRow("DataStructureName")
                        ndVendorNumber.InnerText = dRow("VendorNumber")
                        ndVendorType.InnerText = dRow("VendorType")
                        ndSendThisSession.InnerText = dRow("SendThisSession")
                        ndSentOK.InnerText = dRow("SentOK")
                        ndXMLDocumentName.InnerText = dRow("XMLDocumentName")
                        ndReleaseNumber.InnerText = dRow("ReleaseNumber")
                        ndOrderStatus.InnerText = dRow("OrderStatus")
                        ndBuyerID.InnerText = dRow("BuyerID")
                        ndWarehouseID.InnerText = dRow("WarehouseID")
                        ndShipToNumber.InnerText = dRow("ShipToNumber")
                        ndCarrierCode.InnerText = dRow("CarrierCode")


                        '-------------------------------
                        '   Address Type = "ShipTo" 
                        '
                        ndDeliveryName.InnerText = dRow("DeliveryName").ToString
                        ndDeliveryAddress1.InnerText = dRow("DeliveryAddress1").ToString
                        ndDeliveryAddress2.InnerText = dRow("DeliveryAddress2").ToString
                        ndDeliveryAddress3.InnerText = dRow("DeliveryAddress3").ToString
                        ndDeliveryAddress4.InnerText = dRow("DeliveryAddress4").ToString
                        ndDeliveryAddress5.InnerText = dRow("DeliveryAddress5").ToString
                        ndShipToPostalCode.InnerText = dRow("DeliveryPostcode").ToString

                        Try
                            Dim d1 As Date = dRow("OrderDateTime")
                            ndOrderDate.InnerText = d1.ToString("yyyyMMddThh:nn.ss")
                            Dim d2 As Date = dRow("ExpectedDate") '.ToString(str13)
                            ndExpectedDate.InnerText = d2.ToString("yyyyMMdd")
                            Dim d3 As Date = dRow("RequestedReceiptDate") '.ToString(str13)
                            ndRequestedReceiptDate.InnerText = d3.ToString("yyyyMMdd")

                        Catch ex As Exception
                        End Try
                        ndNumberOfLinesOnOrder.InnerText = dRow("NumberOfLinesOnOrder").ToString
                        ndTotalQuantity.InnerText = dRow("TotalQuantity").ToString
                        ndTotalEstimatedCost.InnerText = dRow("TotalEstimatedCost").ToString
                        ndTaxTotal.InnerText = dRow("TaxTotal").ToString
                        ndDiscountPercent.InnerText = dRow("DiscountPercent").ToString
                        ndDiscountAmount.InnerText = dRow("DiscountAmount").ToString
                        ndTransactionCurrencyCode.InnerText = dRow("TransactionCurrencyCode").ToString
                        ndBaseCurrencyCode.InnerText = dRow("BaseCurrencyCode").ToString
                        ndTransactionExchangeRate.InnerText = dRow("TransactionExchangeRate").ToString
                        ndMultyDivideFlag.InnerText = dRow("MultyDivideFlag").ToString
                        ndInvoiceNumber.InnerText = dRow("InvoiceNumber").ToString
                        ndSpecialInstructions.InnerText = dRow("SpecialInstructions").ToString
                        ndUDF01.InnerText = dRow("UDF01").ToString
                        ndUDF02.InnerText = dRow("UDF02").ToString
                        ndUDF03.InnerText = dRow("UDF03").ToString
                        ndUDF04.InnerText = dRow("UDF04").ToString
                        ndUDF05.InnerText = dRow("UDF05").ToString
                        ndUDF06.InnerText = dRow("UDF06").ToString
                        ndUDF07.InnerText = dRow("UDF07").ToString
                        ndUDF08.InnerText = dRow("UDF08").ToString
                        ndUDF09.InnerText = dRow("UDF09").ToString
                        ndUDF10.InnerText = dRow("UDF10").ToString
                        '---------------------------------------------------------------------------
                        err = InsertComments(PurchaseOrderNumber)
                        '---------------------------------------------------------------------------
                        err = InsertDetails(PurchaseOrderNumber)
                        '---------------------------------------------------------------------------
                        err = AppendHeader()
                     Next
                Else
                    '------------------------------------------------------------------------------
                    '   No records - Create dummy
                    '
                    err = CreateHeader()
                    err = CreateDetails()
                    err = AppendDetails()
                    err = AppendHeader()
                    '------------------------------------------------------------------------------
                End If
                ndPurchaseOrders.AppendChild(ndPurchaseOrder)
                '---------------------------------------------------------------------------
            Catch ex As Exception
                Const strError As String = "Failed to Insert Purchase Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-11"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreateHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndPurchaseOrder = .CreateElement("PurchaseOrder")
                    ndPurchaseOrderHeader = .CreateElement("PurchaseOrderHeader")
                    ndPurchaseOrderLines = .CreateElement("PurchaseOrderLines")
                    '
                    ndLOBCode = .CreateElement("LOBCode")
                    ndDataStructureName = .CreateElement("DataStructureName")
                    ndPurchaseOrderNumber = .CreateElement("PurchaseOrderNumber")
                    ndVendorNumber = .CreateElement("VendorNumber")
                    ndVendorType = .CreateElement("VendorType")
                    ndSendThisSession = .CreateElement("SendThisSession")
                    ndSentOK = .CreateElement("SentOK")
                    ndXMLDocumentName = .CreateElement("XMLDocumentName")
                    ndReleaseNumber = .CreateElement("ReleaseNumber")
                    ndOrderStatus = .CreateElement("OrderStatus")
                    ndBuyerID = .CreateElement("BuyerID")
                    ndWarehouseID = .CreateElement("WarehouseID")
                    ndShipToNumber = .CreateElement("ShipToNumber")
                    ndCarrierCode = .CreateElement("CarrierCode")

                    '-------------------------------
                    ' Address Type = "Delivery" 
                    '
                    ndDeliveryAddress = .CreateElement("Address")
                    atDelivery = .CreateAttribute("Type")
                    atDelivery.Value = "Delivery"
                    ndDeliveryAddress.Attributes.Append(atDelivery)
                    ndDeliveryName = .CreateElement("DeliveryName")
                    ndDeliveryAddress1 = .CreateElement("DeliveryAddressLine1")
                    ndDeliveryAddress2 = .CreateElement("DeliveryAddressLine2")
                    ndDeliveryAddress3 = .CreateElement("DeliveryAddressLine3")
                    ndDeliveryAddress4 = .CreateElement("DeliveryAddressLine4")
                    ndDeliveryAddress5 = .CreateElement("DeliveryAddressLine5")
                    ndShipToPostalCode = .CreateElement("DeliveryPostalCode")

                    ndOrderDate = .CreateElement("OrderDate")
                    ndExpectedDate = .CreateElement("ExpectedDate")
                    ndRequestedReceiptDate = .CreateElement("RequestedReceiptDate")
                    ndNumberOfLinesOnOrder = .CreateElement("NumberOfLinesOnOrder")
                    ndTotalQuantity = .CreateElement("TotalQuantity")
                    ndTotalEstimatedCost = .CreateElement("TotalEstimatedCost")
                    ndTaxTotal = .CreateElement("TaxTotal")
                    ndDiscountPercent = .CreateElement("DiscountPercent")
                    ndDiscountAmount = .CreateElement("DiscountAmount")
                    ndTransactionCurrencyCode = .CreateElement("TransactionCurrencyCode")
                    ndBaseCurrencyCode = .CreateElement("BaseCurrencyCode")
                    ndTransactionExchangeRate = .CreateElement("TransactionExchangeRate")
                    ndMultyDivideFlag = .CreateElement("MultyDivideFlag")
                    ndInvoiceNumber = .CreateElement("InvoiceNumber")
                    ndSpecialInstructions = .CreateElement("SpecialInstructions")
                    ndComments = .CreateElement("Comments")
                    ndInternal = .CreateElement("Internal")
                    ndExternal = .CreateElement("External")

                    ndUDF01 = .CreateElement("UDF01")
                    ndUDF02 = .CreateElement("UDF02")
                    ndUDF03 = .CreateElement("UDF03")
                    ndUDF04 = .CreateElement("UDF04")
                    ndUDF05 = .CreateElement("UDF05")
                    ndUDF06 = .CreateElement("UDF06")
                    ndUDF07 = .CreateElement("UDF07")
                    ndUDF08 = .CreateElement("UDF08")
                    ndUDF09 = .CreateElement("UDF09")
                    ndUDF10 = .CreateElement("UDF10")
                 End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Purchase Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-12"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendHeader() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With ndPurchaseOrderHeader
                    .AppendChild(ndLOBCode)
                    .AppendChild(ndDataStructureName)
                    .AppendChild(ndPurchaseOrderNumber)
                    .AppendChild(ndVendorNumber)
                    .AppendChild(ndVendorType)
                    .AppendChild(ndSendThisSession)
                    .AppendChild(ndSentOK)
                    .AppendChild(ndXMLDocumentName)
                    .AppendChild(ndReleaseNumber)
                    .AppendChild(ndOrderStatus)
                    .AppendChild(ndBuyerID)
                    .AppendChild(ndWarehouseID)
                    .AppendChild(ndShipToNumber)
                    .AppendChild(ndCarrierCode)
                    '-------------------------------
                    ' Address Type  ShipTo 
                    '
                    .AppendChild(ndDeliveryAddress)
                    With ndDeliveryAddress
                        .AppendChild(ndDeliveryName)
                        .AppendChild(ndDeliveryAddress1)
                        .AppendChild(ndDeliveryAddress2)
                        .AppendChild(ndDeliveryAddress3)
                        .AppendChild(ndDeliveryAddress4)
                        .AppendChild(ndDeliveryAddress5)
                        .AppendChild(ndShipToPostalCode)
                    End With
                    .AppendChild(ndOrderDate)
                    .AppendChild(ndExpectedDate)
                    .AppendChild(ndRequestedReceiptDate)
                    .AppendChild(ndNumberOfLinesOnOrder)
                    .AppendChild(ndTotalQuantity)
                    .AppendChild(ndTotalEstimatedCost)
                    .AppendChild(ndTaxTotal)
                    .AppendChild(ndDiscountPercent)
                    .AppendChild(ndDiscountAmount)
                    .AppendChild(ndTransactionCurrencyCode)
                    .AppendChild(ndBaseCurrencyCode)
                    .AppendChild(ndTransactionExchangeRate)
                    .AppendChild(ndMultyDivideFlag)
                    .AppendChild(ndInvoiceNumber)
                    .AppendChild(ndSpecialInstructions)
                    .AppendChild(ndComments)

                    ndComments.AppendChild(ndInternal)
                    ndComments.AppendChild(ndExternal)

                    .AppendChild(ndUDF01)
                    .AppendChild(ndUDF02)
                    .AppendChild(ndUDF03)
                    .AppendChild(ndUDF04)
                    .AppendChild(ndUDF05)
                    .AppendChild(ndUDF06)
                    .AppendChild(ndUDF07)
                    .AppendChild(ndUDF08)
                    .AppendChild(ndUDF09)
                    .AppendChild(ndUDF10)
                End With
                '---------------------------------------------------------------------------
                ndPurchaseOrder.AppendChild(ndPurchaseOrderHeader)
                ndPurchaseOrder.AppendChild(ndPurchaseOrderLines)
                ndPurchaseOrders.AppendChild(ndPurchaseOrder)
                '---------------------------------------------------------------------------
            Catch ex As Exception
                Const strError As String = "Failed to Append Purchase Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-14"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertDetails(ByVal PurchaseOrderNumber As String) As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
             Try
                Dim dr As DataRow
                If Not dtDetail Is Nothing AndAlso dtDetail.Rows.Count > 0 Then
                    For Each dr In dtDetail.Rows
                        If dr("PurchaseOrderNumber").ToString.Trim = PurchaseOrderNumber.Trim Then
                            '-----------------------------------------------------------
                            err = CreateDetails()
                            atLineNumber.Value = dr("LineNumber").ToString
                            '------------------------------------------------------------
                            ndLineActionCode.InnerText = dr("LineActionCode").ToString
                            ndLine_OrderStatus.InnerText = dr("Line_OrderStatus").ToString
                            ndLine_BuyerID.InnerText = dr("Line_BuyerID").ToString
                            ndLine_WarehouseID.InnerText = dr("Line_WarehouseID").ToString
                            ndProductNumber.InnerText = dr("ProductNumber").ToString
                            ndProductDescription.InnerText = dr("ProductDescription").ToString
                            ndUserProductNumber.InnerText = dr("UserProductNumber").ToString
                            ndUserProductDescription.InnerText = dr("UserProductDescription").ToString
                            ndMeasureCode.InnerText = dr("MeasureCode").ToString

                            Try
                                Dim d1 As Date = dr("DeliveryDate")
                                ndDeliveryDate.InnerText = d1.ToString("yyyyMMddThh:nn.ss")
                             Catch ex As Exception
                            End Try

                            ndQuantityOrdered.InnerText = dr("QuantityOrdered").ToString
                            ndQuantityReceived.InnerText = dr("QuantityReceived").ToString
                            ndQuantityOutstanding.InnerText = dr("QuantityOutstanding").ToString
                            ndEstimatedCost.InnerText = dr("EstimatedCost").ToString
                            ndActualSellingPrice.InnerText = dr("ActualSellingPrice").ToString
                            ndSpecialSellingPrice.InnerText = dr("SpecialSellingPrice").ToString
                            ndLine_DiscountAmount.InnerText = dr("Line_DiscountAmount").ToString
                            ndLine_DiscountPercent.InnerText = dr("Line_DiscountPercent").ToString
                            ndLineInstructions.InnerText = dr("LineInstructions").ToString
                            err = AppendDetails()
                        End If
                    Next
                Else
                    '---------------------------------------------------------------------------
                    '   No Items so create Dummy
                    '
                    err = CreateDetails()
                    err = AppendDetails()
                    '
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert Purchase Order Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-18"
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
                    ndPurchaseOrderLine = .CreateElement("PurchaseOrderLine")
                    atLineNumber = .CreateAttribute("LineNumber")
                    ndLineActionCode = .CreateElement("LineActionCode")
                    ndLine_OrderStatus = .CreateElement("Line_OrderStatus")
                    ndLine_BuyerID = .CreateElement("Line_BuyerID")
                    ndLine_WarehouseID = .CreateElement("Line_WarehouseID")
                    ndProductNumber = .CreateElement("ProductNumber")
                    ndProductDescription = .CreateElement("ProductDescription")
                    ndUserProductNumber = .CreateElement("UserProductNumber")
                    ndUserProductDescription = .CreateElement("UserProductDescription")
                    ndMeasureCode = .CreateElement("MeasureCode")
                    ndDeliveryDate = .CreateElement("DeliveryDate")
                    ndQuantityOrdered = .CreateElement("QuantityOrdered")
                    ndQuantityReceived = .CreateElement("QuantityReceived")
                    ndQuantityOutstanding = .CreateElement("QuantityOutstanding")
                    ndEstimatedCost = .CreateElement("EstimatedCost")
                    ndActualSellingPrice = .CreateElement("ActualSellingPrice")
                    ndSpecialSellingPrice = .CreateElement("SpecialSellingPrice")
                    ndLine_DiscountAmount = .CreateElement("Line_DiscountAmount")
                    ndLine_DiscountPercent = .CreateElement("Line_DiscountPercent")
                    ndLineInstructions = .CreateElement("LineInstructions")
                    '-----------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Purchase Order Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-19"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With ndPurchaseOrderLine
                    .Attributes.Append(atLineNumber)
                    .AppendChild(ndLineActionCode)
                    .AppendChild(ndLine_OrderStatus)
                    .AppendChild(ndLine_BuyerID)
                    .AppendChild(ndLine_WarehouseID)
                    .AppendChild(ndProductNumber)
                    .AppendChild(ndProductDescription)
                    .AppendChild(ndUserProductNumber)
                    .AppendChild(ndUserProductDescription)
                    .AppendChild(ndMeasureCode)
                    .AppendChild(ndDeliveryDate)
                    .AppendChild(ndQuantityOrdered)
                    .AppendChild(ndQuantityReceived)
                    .AppendChild(ndQuantityOutstanding)
                    .AppendChild(ndEstimatedCost)
                    .AppendChild(ndActualSellingPrice)
                    .AppendChild(ndSpecialSellingPrice)
                    .AppendChild(ndLine_DiscountAmount)
                    .AppendChild(ndLine_DiscountPercent)
                    .AppendChild(ndLineInstructions)
                End With
                '-----------------------------------------------------------------
                ndPurchaseOrderLines.AppendChild(ndPurchaseOrderLine)
            Catch ex As Exception
                Const strError As String = "Failed to Append Purchase Order Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-20"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertComments(ByVal purchaseOrderNumber As String) As ErrorObj
            Dim err As ErrorObj = Nothing
            '
            Dim ndCommentLine, ndCommentText, ndCommentLineNumber As XmlNode
            Dim dr As DataRow
            '---------------------------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    If Not dtComments Is Nothing AndAlso dtComments.Rows.Count > 0 Then
                        For Each dr In dtComments.Rows
                            If dr("PurchaseOrderNumber").Equals(purchaseOrderNumber) Then
                                ndCommentLine = .CreateElement("CommentLine")
                                ndCommentText = .CreateElement("CommentText")
                                ndCommentLineNumber = .CreateElement("CommentLineNumber")
                                '
                                ndCommentLineNumber.InnerText = dr("LineNumber")
                                ndCommentText.InnerText = dr("Text")
                                '
                                ndCommentLine.AppendChild(ndCommentText)
                                ndCommentLine.AppendChild(ndCommentLineNumber)
                                If dr("CommentType").Equals("INTERNAL") Then
                                    ndInternal.AppendChild(ndCommentLine)
                                Else
                                    ndExternal.AppendChild(ndCommentLine)
                                End If
                            End If
                        Next
                    Else
                        ndCommentLine = .CreateElement("CommentLine")
                        ndCommentText = .CreateElement("CommentText")
                        ndCommentLineNumber = .CreateElement("CommentLineNumber")
                        '
                        ndCommentLine.AppendChild(ndCommentText)
                        ndCommentLine.AppendChild(ndCommentLineNumber)
                        ndInternal.AppendChild(ndCommentLine)
                    End If
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Insert Order Comment Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-13"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace