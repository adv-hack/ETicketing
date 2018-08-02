Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with order responses
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSOR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlOrderResponse
        Inherits XmlResponse

        Protected Overrides Sub InsertBodyV1()

            Dim ndHeader, ndHeaderHeader, ndrOrderInfo, ndrOrderNumbers, _
                ndBranchOrderNumber, ndCustomerPO, ndErrorStatus, ndThirdPartyFreight, _
                ndShipToAttention, ndShipToAddress1, ndShipToAddress2, ndShipToAddress3, _
                ndShipToCity, ndShipToProvince, ndShipToPostalCode, ndShipToSuffix, _
                ndAddressErrorMessage, ndContractNumber, ndrOrderSuffix, ndDistributionWeight, _
                ndSuffixErrorResponse, ndCarrier, ndFreightRate, ndLineInformation, _
                ndLineError, ndSku, ndUnitPrice, ndWestcoastLineNumber, _
                ndCustomerLineNumber, ndShipFromBranch, ndrOrderQuantity, ndAllocatedQuantity, _
                ndBackOrderQuantity, ndBackOrderETADate, ndPriceDerivedFlag, _
                ndForeignCurrency, ndLineFreightRate, ndTransitDays, ndLineBillToSuffix, _
                ndrCommentLine, ndrCommentText, ndrCommentLineNumber, ndCustomerSKU As XmlNode

            Dim atErrorNumber, atXmlNsXsi, atAddressErrorType, atSuffix, atCarrierCode As XmlAttribute

            '--------------------------------------------------------------------------------------
            '   Prepare all elements & attributes
            '
            '   Seperate the tables out of the ResultSet  
            ' 
            Dim dtHeader As DataTable = Nothing
            Dim dtDetail As DataTable = Nothing
            Dim dtText As DataTable = Nothing

            Dim orderNo As String = String.Empty
            Dim iCounter As Integer = 0

            Try
                dtHeader = ResultDataSet.Tables(0)     ' Header
                dtDetail = ResultDataSet.Tables(1)     ' Detail
                dtText = ResultDataSet.Tables(2)       ' Text
            Catch ex As Exception
            End Try
            '--------------------------------------------------------------------------------------
            Dim drOrder, drProduct, drComment As DataRow
            Dim ndrProductLine As XmlNode
            '
            With MyBase.xmlDoc
                ndrOrderInfo = .CreateElement("OrderInfo")
                '----------------------------------------------------------------------------------
                If Not dtHeader Is Nothing AndAlso dtHeader.Rows.Count > 0 Then
                    For Each drOrder In dtHeader.Rows
                        ndrOrderNumbers = .CreateElement("OrderNumbers")
                        '-----------------------------------------------------------
                        ' Add the delivery info from table 1
                        ' 
                        ndBranchOrderNumber = .CreateElement("BranchOrderNumber")
                        orderNo = drOrder("OrderNumber")
                        ndBranchOrderNumber.InnerText = drOrder("BranchOrderNumber")

                        ndCustomerPO = .CreateElement("CustomerPO")
                        ndCustomerPO.InnerText = drOrder("CustomerPO")
                        '-----------------------------------------------------------
                        iCounter += 1
                        ndErrorStatus = .CreateElement("ErrorStatus")
                        If Not Err Is Nothing Then _
                            ndErrorStatus.InnerText = Err.ItemErrorStatus(iCounter)
                        atErrorNumber = .CreateAttribute("ErrorNumber")
                        If Not Err Is Nothing Then _
                            atErrorNumber.Value = Err.ItemErrorCode(iCounter)
                        ndErrorStatus.Attributes.Append(atErrorNumber)
                        '-----------------------------------------------------------                        
                        ndThirdPartyFreight = .CreateElement("ThirdPartyFreight")
                        ndThirdPartyFreight.InnerText = String.Empty

                        ndShipToAttention = .CreateElement("ShipToAttention")
                        ndShipToAddress1 = .CreateElement("ShipToAddress1")
                        ndShipToAddress2 = .CreateElement("ShipToAddress2")
                        ndShipToAddress3 = .CreateElement("ShipToAddress3")
                        ndShipToCity = .CreateElement("ShipToCity")
                        ndShipToProvince = .CreateElement("ShipToProvince")
                        ndShipToPostalCode = .CreateElement("ShipToPostalCode")

                        ndShipToAttention.InnerText = drOrder("ShipToAttention")
                        ndShipToAddress1.InnerText = drOrder("ShipToAddress1")
                        ndShipToAddress2.InnerText = drOrder("ShipToAddress2")
                        ndShipToAddress3.InnerText = drOrder("ShipToAddress3")
                        ndShipToCity.InnerText = drOrder("ShipToCity")
                        ndShipToProvince.InnerText = drOrder("ShipToProvince")
                        ndShipToPostalCode.InnerText = drOrder("ShipToPostalCode")
                        '-----------------------------------------------------------                        
                        ndCarrier = .CreateElement("Carrier")
                        atCarrierCode = .CreateAttribute("CarrierCode")
                        ndFreightRate = .CreateElement("FreightRate")
                        ndCarrier.InnerText = drOrder("CarrierCodeDescription")
                        atCarrierCode.Value = drOrder("CarrierCode")
                        ndCarrier.Attributes.Append(atCarrierCode)
                        ndFreightRate.InnerText = String.Format("{0:F2}", drOrder("FreightRate"))

                        ndShipToSuffix = .CreateElement("ShipToSuffix")
                        Try
                            ndShipToSuffix.InnerText = drOrder("ShipToSuffix")
                        Catch ex As Exception

                        End Try
                        '-----------------------------------------------------------                        
                        ndAddressErrorMessage = .CreateElement("AddressErrorMessage")
                        ndAddressErrorMessage.InnerText = String.Empty
                        atAddressErrorType = .CreateAttribute("AddressErrorType")
                        atAddressErrorType.Value = String.Empty
                        ndAddressErrorMessage.Attributes.Append(atAddressErrorType)

                        ndContractNumber = .CreateElement("ContractNumber")
                        ndContractNumber.InnerText = String.Empty

                        ndrOrderSuffix = .CreateElement("OrderSuffix")
                        atSuffix = .CreateAttribute("Suffix")
                        atSuffix.Value = String.Empty
                        ndrOrderSuffix.Attributes.Append(atSuffix)

                        ndDistributionWeight = .CreateElement("DistributionWeight")
                        ndDistributionWeight.InnerText = String.Empty

                        ndSuffixErrorResponse = .CreateElement("SuffixErrorResponse")
                        ndSuffixErrorResponse.InnerText = String.Empty

                        ndLineInformation = .CreateElement("LineInformation")
                        '------------------------------------------------------------------------------
                        For Each drProduct In dtDetail.Rows
                            If drProduct("OrderNumber").Equals(orderNo) Then

                                ndrProductLine = .CreateElement("ProductLine")

                                ndSku = .CreateElement("SKU")
                                ndUnitPrice = .CreateElement("UnitPrice")
                                ndWestcoastLineNumber = .CreateElement("WestcoastLineNumber")
                                ndShipFromBranch = .CreateElement("ShipFromBranch")
                                ndrOrderQuantity = .CreateElement("OrderQuantity")
                                ndAllocatedQuantity = .CreateElement("AllocatedQuantity")
                                ndBackOrderQuantity = .CreateElement("BackOrderedQuantity")
                                ndLineError = .CreateElement("LineError")
                                ndCustomerLineNumber = .CreateElement("CustomerLineNumber")
                                ndBackOrderETADate = .CreateElement("BackOrderETADate")
                                ndPriceDerivedFlag = .CreateElement("PriceDerivedFlag")
                                ndForeignCurrency = .CreateElement("ForeignCurrency")
                                ndLineFreightRate = .CreateElement("LineFreightRate")
                                ndTransitDays = .CreateElement("TransitDays")
                                ndLineBillToSuffix = .CreateElement("LineBillToSuffix")
                                ndCustomerSKU = .CreateElement("CustomerSKU")

                                ndSku.InnerText = String.Empty
                                ndUnitPrice.InnerText = String.Empty
                                ndWestcoastLineNumber.InnerText = String.Empty
                                ndShipFromBranch.InnerText = String.Empty
                                ndrOrderQuantity.InnerText = String.Empty
                                ndAllocatedQuantity.InnerText = String.Empty
                                ndBackOrderQuantity.InnerText = String.Empty
                                ndLineError.InnerText = String.Empty
                                ndCustomerLineNumber.InnerText = String.Empty
                                ndBackOrderETADate.InnerText = String.Empty
                                ndPriceDerivedFlag.InnerText = String.Empty
                                ndForeignCurrency.InnerText = String.Empty
                                ndLineFreightRate.InnerText = String.Empty
                                ndTransitDays.InnerText = String.Empty
                                ndLineBillToSuffix.InnerText = String.Empty
                                ndCustomerSKU.InnerText = String.Empty

                                ndSku.InnerText = drProduct("Sku").ToString.Trim
                                ndUnitPrice.InnerText = String.Format("{0:F2}", drProduct("UnitPrice"))
                                ndWestcoastLineNumber.InnerText = drProduct("LineNumber")
                                ndShipFromBranch.InnerText = drProduct("ShipFromBranch")
                                ndrOrderQuantity.InnerText = drProduct("OrderQuantity")
                                ndAllocatedQuantity.InnerText = drProduct("AllocatedQuantity")
                                ndBackOrderQuantity.InnerText = drProduct("BackOrderQuantity")
                                ndCustomerSKU.InnerText = drProduct("CustomerSKU")

                                ndLineInformation.AppendChild(ndrProductLine)

                                With ndrProductLine
                                    .AppendChild(ndLineError)
                                    .AppendChild(ndSku)
                                    .AppendChild(ndUnitPrice)
                                    .AppendChild(ndWestcoastLineNumber)
                                    .AppendChild(ndCustomerLineNumber)
                                    .AppendChild(ndShipFromBranch)
                                    .AppendChild(ndrOrderQuantity)
                                    .AppendChild(ndAllocatedQuantity)
                                    .AppendChild(ndBackOrderQuantity)
                                    .AppendChild(ndBackOrderETADate)
                                    .AppendChild(ndPriceDerivedFlag)
                                    .AppendChild(ndForeignCurrency)
                                    .AppendChild(ndFreightRate)
                                    .AppendChild(ndTransitDays)
                                    .AppendChild(ndLineBillToSuffix)
                                    .AppendChild(ndCustomerSKU)
                                End With
                            End If
                        Next drProduct
                        '------------------------------------------------------------------------------
                        '   Add comment lines (recursive) from table 3
                        ' 
                        ndrCommentText = .CreateElement("CommentText")
                        ndrCommentLineNumber = .CreateElement("CommentLineNumber")
                        If Not dtText Is Nothing AndAlso dtText.Rows.Count > 0 Then
                            For Each drComment In dtText.Rows
                                If drComment("OrderNumber").Equals(orderNo) Then
                                    ndrCommentLine = .CreateElement("CommentLine")
                                    ndrCommentText = .CreateElement("CommentText")
                                    ndrCommentLineNumber = .CreateElement("CommentLineNumber")
                                    ndrCommentText.InnerText = drComment("Text")
                                    ndrCommentLineNumber.InnerText = drComment("TextLineNumber")
                                    ndLineInformation.AppendChild(ndrCommentLine)
                                    With ndrCommentLine
                                        .AppendChild(ndrCommentText)
                                        .AppendChild(ndrCommentLineNumber)
                                    End With
                                End If
                            Next drComment
                        Else
                            '--------------------------------------------------
                            '   No comments, write dummy
                            '
                            ndrCommentLine = .CreateElement("CommentLine")
                            ndrCommentText = .CreateElement("CommentText")
                            ndrCommentLineNumber = .CreateElement("CommentLineNumber")
                            ndrCommentText.InnerText = String.Empty
                            ndrCommentLineNumber.InnerText = String.Empty
                            ndLineInformation.AppendChild(ndrCommentLine)
                            With ndrCommentLine
                                .AppendChild(ndrCommentText)
                                .AppendChild(ndrCommentLineNumber)
                            End With
                        End If

                        With ndrOrderNumbers
                            .AppendChild(ndBranchOrderNumber)
                            .AppendChild(ndCustomerPO)
                            .AppendChild(ndErrorStatus)
                            .AppendChild(ndThirdPartyFreight)
                            .AppendChild(ndShipToAttention)
                            .AppendChild(ndShipToAddress1)
                            .AppendChild(ndShipToAddress2)
                            .AppendChild(ndShipToAddress3)
                            .AppendChild(ndShipToCity)
                            .AppendChild(ndShipToProvince)
                            .AppendChild(ndShipToPostalCode)
                            .AppendChild(ndShipToSuffix)
                            .AppendChild(ndAddressErrorMessage)
                            .AppendChild(ndContractNumber)
                            .AppendChild(ndrOrderSuffix)
                        End With
                        With ndrOrderSuffix
                            .AppendChild(ndDistributionWeight)
                            .AppendChild(ndSuffixErrorResponse)
                            .AppendChild(ndCarrier)
                            .AppendChild(ndFreightRate)
                            .AppendChild(ndLineInformation)
                        End With
                        ndrOrderInfo.AppendChild(ndrOrderNumbers)
                    Next drOrder
                    '------------------------------------------------------------------------------
                Else
                    '----------------------------------------------------------
                    '   No order header, write dummy
                    '
                    ndrOrderNumbers = .CreateElement("OrderNumbers")
                    ndBranchOrderNumber = .CreateElement("BranchOrderNumber")
                    ndCustomerPO = .CreateElement("CustomerPO")
                    ndErrorStatus = .CreateElement("ErrorStatus")

                    '-----------------------------------------------------------    
                    ndThirdPartyFreight = .CreateElement("ThirdPartyFreight")
                    ndShipToAttention = .CreateElement("ShipToAttention")
                    ndShipToAddress1 = .CreateElement("ShipToAddress1")
                    ndShipToAddress2 = .CreateElement("ShipToAddress2")
                    ndShipToAddress3 = .CreateElement("ShipToAddress3")
                    ndShipToCity = .CreateElement("ShipToCity")
                    ndShipToProvince = .CreateElement("ShipToProvince")
                    ndShipToPostalCode = .CreateElement("ShipToPostalCode")
                    ndCarrier = .CreateElement("Carrier")
                    atCarrierCode = .CreateAttribute("CarrierCode")
                    ndFreightRate = .CreateElement("FreightRate")

                    ndBranchOrderNumber.InnerText = String.Empty
                    ndCustomerPO.InnerText = String.Empty

                    ' If only one order header created then check for an error..
                    If Not Err Is Nothing Then _
                      ndErrorStatus.InnerText = Err.ItemErrorStatus(1)
                    atErrorNumber = .CreateAttribute("ErrorNumber")
                    If Not Err Is Nothing Then _
                        atErrorNumber.Value = Err.ItemErrorCode(1)
                    ndErrorStatus.Attributes.Append(atErrorNumber)
                    ndThirdPartyFreight.InnerText = String.Empty
                    ndShipToAttention.InnerText = String.Empty
                    ndShipToAddress1.InnerText = String.Empty
                    ndShipToAddress2.InnerText = String.Empty
                    ndShipToAddress3.InnerText = String.Empty
                    ndShipToCity.InnerText = String.Empty
                    ndShipToProvince.InnerText = String.Empty
                    ndShipToPostalCode.InnerText = String.Empty
                    ndCarrier.InnerText = String.Empty
                    atCarrierCode.Value = String.Empty
                    ndCarrier.Attributes.Append(atCarrierCode)
                    ndFreightRate.InnerText = String.Empty
                    ndrProductLine = .CreateElement("ProductLine")

                    ndSku = .CreateElement("SKU")
                    ndUnitPrice = .CreateElement("UnitPrice")
                    ndWestcoastLineNumber = .CreateElement("WestcoastLineNumber")
                    ndShipFromBranch = .CreateElement("ShipFromBranch")
                    ndrOrderQuantity = .CreateElement("OrderQuantity")
                    ndAllocatedQuantity = .CreateElement("AllocatedQuantity")
                    ndBackOrderQuantity = .CreateElement("BackOrderedQuantity")
                    ndLineError = .CreateElement("LineError")
                    ndCustomerLineNumber = .CreateElement("CustomerLineNumber")
                    ndBackOrderETADate = .CreateElement("BackOrderETADate")
                    ndPriceDerivedFlag = .CreateElement("PriceDerivedFlag")
                    ndForeignCurrency = .CreateElement("ForeignCurrency")
                    ndLineFreightRate = .CreateElement("LineFreightRate")
                    ndTransitDays = .CreateElement("TransitDays")
                    ndLineBillToSuffix = .CreateElement("LineBillToSuffix")
                    ndCustomerSKU = .CreateElement("CustomerSKU")

                    ndSku.InnerText = String.Empty
                    ndUnitPrice.InnerText = String.Empty
                    ndWestcoastLineNumber.InnerText = String.Empty
                    ndShipFromBranch.InnerText = String.Empty
                    ndrOrderQuantity.InnerText = String.Empty
                    ndAllocatedQuantity.InnerText = String.Empty
                    ndBackOrderQuantity.InnerText = String.Empty
                    ndLineError.InnerText = String.Empty
                    ndCustomerLineNumber.InnerText = String.Empty
                    ndBackOrderETADate.InnerText = String.Empty
                    ndPriceDerivedFlag.InnerText = String.Empty
                    ndForeignCurrency.InnerText = String.Empty
                    ndLineFreightRate.InnerText = String.Empty
                    ndTransitDays.InnerText = String.Empty
                    ndLineBillToSuffix.InnerText = String.Empty
                    ndCustomerSKU.InnerText = String.Empty

                    ndLineInformation = .CreateElement("LineInformation")
                    ndLineInformation.AppendChild(ndrProductLine)

                    With ndrProductLine
                        .AppendChild(ndLineError)
                        .AppendChild(ndSku)
                        .AppendChild(ndUnitPrice)
                        .AppendChild(ndWestcoastLineNumber)
                        .AppendChild(ndCustomerLineNumber)
                        .AppendChild(ndShipFromBranch)
                        .AppendChild(ndrOrderQuantity)
                        .AppendChild(ndAllocatedQuantity)
                        .AppendChild(ndBackOrderQuantity)
                        .AppendChild(ndBackOrderETADate)
                        .AppendChild(ndPriceDerivedFlag)
                        .AppendChild(ndForeignCurrency)
                        .AppendChild(ndFreightRate)
                        .AppendChild(ndTransitDays)
                        .AppendChild(ndLineBillToSuffix)
                        .AppendChild(ndCustomerSKU)
                    End With

                    With ndrOrderNumbers
                        .AppendChild(ndBranchOrderNumber)
                        .AppendChild(ndCustomerPO)
                        .AppendChild(ndErrorStatus)
                        .AppendChild(ndThirdPartyFreight)
                        .AppendChild(ndShipToAttention)
                        .AppendChild(ndShipToAddress1)
                        .AppendChild(ndShipToAddress2)
                        .AppendChild(ndShipToAddress3)
                        .AppendChild(ndShipToCity)
                        .AppendChild(ndShipToProvince)
                        .AppendChild(ndShipToPostalCode)
                        '.AppendChild(ndShipToSuffix)
                        '.AppendChild(ndAddressErrorMessage)
                        '.AppendChild(ndContractNumber)
                        '.AppendChild(ndrOrderSuffix)
                    End With

                    'With ndrOrderSuffix
                    '    .AppendChild(ndDistributionWeight)
                    '    .AppendChild(ndSuffixErrorResponse)
                    '    .AppendChild(ndCarrier)
                    '    .AppendChild(ndFreightRate)
                    '    .AppendChild(ndLineInformation)
                    'End With
                    With ndrOrderInfo
                        .AppendChild(ndrOrderNumbers)
                    End With
                End If
            End With
            '--------------------------------------------------------------------------------------
            '   Build XML fragment
            '
            'ndrOrderInfo.AppendChild(ndrOrderNumbers)

            'With ndrOrderNumbers
            '    .AppendChild(ndBranchOrderNumber)
            '    .AppendChild(ndCustomerPO)
            '    .AppendChild(ndErrorStatus)
            '    .AppendChild(ndThirdPartyFreight)
            '    .AppendChild(ndShipToAttention)
            '    .AppendChild(ndShipToAddress1)
            '    .AppendChild(ndShipToAddress2)
            '    .AppendChild(ndShipToAddress3)
            '    .AppendChild(ndShipToCity)
            '    .AppendChild(ndShipToProvince)
            '    .AppendChild(ndShipToPostalCode)
            '    .AppendChild(ndShipToSuffix)
            '    .AppendChild(ndAddressErrorMessage)
            '    .AppendChild(ndContractNumber)
            '    .AppendChild(ndrOrderSuffix)
            'End With

            'With ndrOrderSuffix
            '    .AppendChild(ndDistributionWeight)
            '    .AppendChild(ndSuffixErrorResponse)
            '    .AppendChild(ndCarrier)
            '    .AppendChild(ndFreightRate)
            '    .AppendChild(ndLineInformation)
            'End With

            '--------------------------------------------------------------------------------------
            '   Insert the fragment into the XML document
            '
            Const c1 As String = "//"                               ' Constants are faster at run time
            Const c2 As String = "/TransactionHeader"
            '
            ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
            ndHeaderHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
            ndHeader.InsertAfter(ndrOrderInfo, ndHeaderHeader)
            '--------------------------------------------------------------------------------------
            '   Insert the XSD reference & namespace as an attribute within the root node
            '
            atXmlNsXsi = CreateNamespaceAttribute()
            ndHeader.Attributes.Append(atXmlNsXsi)

        End Sub

    End Class

End Namespace