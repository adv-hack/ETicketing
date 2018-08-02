Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Tracking Response
'
'       Date                        8th Nov 2006
'
'       Author                      Andy White
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSOT- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlOrderTrackingResponse
        Inherits XmlResponse
        Private ndHeaderRoot, ndHeaderRootHeader As XmlNode
        Private atXmlNsXsi As XmlAttribute
        '
        Private ndHeader, ndOrderNumbers, ndBranchOrderNumber, _
                ndCustomerPO, ndErrorStatus, ndOrderEntryDate, ndCarrier, _
                ndTotalWeight, ndCartonCount As XmlNode

        Private ndOrderSuffix, ndDistributionWeight, ndSuffixErrorResponse, ndPackage As XmlNode

        Private ndShipDate, ndBoxNumber, ndBoxWeight, ndTrackingURL, _
                ndInvoiceNumber, ndCarrierName, ndConsignmentNumber, _
                ndContents, ndSKU, ndQuantity, ndCustomerSKU As XmlNode

        Private ndSkuSerialNumber, ndSerialNumber As XmlNode

        Private atErrorNumber, atSuffix, atSuffixErrorType, atCarrierCode, _
                atPackageID As XmlAttribute

        Private noShipmentsFound As Boolean = False
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

                        'dtHeader = ResultDataSet.Tables.Item(0)
                        'dtDetail = ResultDataSet.Tables.Item(1)
                        'dtText = ResultDataSet.Tables.Item(2)
                        'dtCarrier = ResultDataSet.Tables.Item(3)
                        'dtPackage = ResultDataSet.Tables.Item(4)
                        'dtProduct = ResultDataSet.Tables.Item(5)

                        dtHeader = ResultDataSet.Tables.Item("DtHeader")
                        dtDetail = ResultDataSet.Tables.Item("DtDetail")
                        dtText = ResultDataSet.Tables.Item("DtText")
                        dtCarrier = ResultDataSet.Tables.Item("DtCarrier")
                        dtPackage = ResultDataSet.Tables.Item("DtPackage")
                        dtProduct = ResultDataSet.Tables.Item("DtProduct")

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
            '
            Try
                With MyBase.xmlDoc
                    Dim dr As DataRow = Nothing
                    If Not dtHeader Is Nothing AndAlso dtHeader.Rows.Count > 0 Then
                        For Each dr In dtHeader.Rows
                            '--------------------------------------------------------------------
                            err2 = CreateHeader()
                            ndBranchOrderNumber.InnerText = dr("BranchOrderNumber").ToString
                            If dr("BranchOrderNumber") <> "Not Found" Then
                                OrderNumber = dr("OrderNumber").ToString
                                ndCustomerPO.InnerText = dr("CustomerPO").ToString
                                ndTotalWeight.InnerText = dr("TotalWeight").ToString
                                ndCartonCount.InnerText = dr("CartonCount").ToString
                                '--------------------------------------------
                                ' Check for no shipments found for this order
                                '--------------------------------------------
                                iCounter += 1

                                Dim foundRows As DataRow()
                                foundRows = dtPackage.Select("OrderNumber = '" & OrderNumber & "'")
                                If foundRows.Length = 0 Then
                                    noShipmentsFound = True
                                    If Err.ItemErrorCode(iCounter) = String.Empty Then
                                        Err.ItemErrorCode(iCounter) = "TTPRSOT-21"
                                        Err.ItemErrorStatus(iCounter) = "No shipments found for order"

                                    End If
                                End If
                            End If

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
                            err2 = InsertCarrier()
                        Next dr
                    Else
                        '--------------------------------------------------------------------
                        '   No order header, write dummy
                        '
                        err2 = CreateHeader()
                        err2 = AppendHeader()
                        err2 = CreateCarrier()
                        err2 = AppendCarrier()
                    End If
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Insert Order Header Nodes"
                With err2
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOT-14"
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
                    '--------------------------------------------------------------------
                    ndOrderNumbers = .CreateElement("OrderNumbers")
                    ndBranchOrderNumber = .CreateElement("BranchOrderNumber")
                    ndCustomerPO = .CreateElement("CustomerPO")
                    ndTotalWeight = .CreateElement("TotalWeight")
                    ndCartonCount = .CreateElement("CartonCount")
                    '--------------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Header Nodes"

                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOT-15"
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
                    .AppendChild(ndTotalWeight)
                    .AppendChild(ndCartonCount)
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Append  Order Header Nodes"

                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOT-16"
                    .HasError = True
                End With
    
            End Try
            Return err
        End Function

        Private Function InsertCarrier() As ErrorObj
            Dim err As ErrorObj = Nothing
            '---------------------------------------------------------------------------------------------
            Dim dc As DataRow = Nothing
            Try
                For Each dc In dtCarrier.Rows
                    If dc("OrderNumber").Equals(OrderNumber) Then
                        err = CreateCarrier()
                        ndDistributionWeight.InnerText = dc("DistributionWeight").ToString
                        ndSuffixErrorResponse.InnerText = dc("SuffixErrorResponse").ToString
                        ndCarrier.InnerText = dc("CarrierCodeDescription").ToString
                        atCarrierCode.Value = dc("CarrierCode").ToString
                        '------------------------------------------------------
                        atSuffixErrorType.Value = dc("SuffixErrorType").ToString
                        err = InsertPackageDetails()
                        err = AppendCarrier()
                        '------------------------------------------------------
                    End If
                Next dc
            Catch ex As Exception
                Const strError As String = "Failed to Insert Carrier Nodes"

                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOT-02"
                    .HasError = True
                End With

            End Try
            Return err
        End Function
        Private Function CreateCarrier() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    '--------------------------------------------------------------------
                    ndOrderSuffix = .CreateElement("OrderDetails")
                    ' atSuffix = .CreateAttribute("Detail") '
                    'ndOrderSuffix.Attributes.Append(atSuffix)
                    '--------------------------------------------------------------------
                    ndDistributionWeight = .CreateElement("DistributionWeight")
                    '--------------------------------------------------------------------
                    ndSuffixErrorResponse = .CreateElement("SuffixErrorResponse")
                    atSuffixErrorType = .CreateAttribute("SuffixErrorType")
                    ndSuffixErrorResponse.Attributes.Append(atSuffixErrorType)
                    '--------------------------------------------------------------------
                    ndCarrier = .CreateElement("Carrier")
                    atCarrierCode = .CreateAttribute("CarrierCode")
                    ndCarrier.Attributes.Append(atCarrierCode)
                    '--------------------------------------------------------------------
                    ndPackage = .CreateElement("Package")
                    atPackageID = .CreateAttribute("ID")
                    ndPackage.Attributes.Append(atPackageID)
                    '--------------------------------------------------------------------
                    ndOrderNumbers.AppendChild(ndOrderSuffix)
                End With
            Catch ex As Exception


                Const strError As String = "Failed to Create Carrier Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOT-11"
                    .HasError = True
                End With

            End Try
            Return err
        End Function
        Private Function AppendCarrier() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                ndSuffixErrorResponse.Attributes.Append(atSuffixErrorType)
                With ndOrderSuffix
                    .AppendChild(ndDistributionWeight)
                    .AppendChild(ndSuffixErrorResponse)
                    .AppendChild(ndCarrier)
                End With
                '----------------------------------------------------------------
            Catch ex As Exception

                Const strError As String = "Failed to Append Carrier Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOT-12"
                    .HasError = True
                End With
    
            End Try
            Return err
        End Function

        Private Function InsertPackageDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            Dim foundRowsDetail As DataRow()
            Dim sku As String = String.Empty
            '---------------------------------------------------------------------------------------------
            Try
                Dim dp As DataRow = Nothing
                Dim currentPackage As String = String.Empty
                For Each dp In dtPackage.Rows
                    If dp("OrderNumber").Equals(OrderNumber) Then
                        If dp("PackageID").ToString <> currentPackage Then

                            If currentPackage <> String.Empty Then
                                err = AppendPackageDetails()
                            End If
                            currentPackage = dp("PackageID")
                            '---------------------------------------------------------
                            err = CreatePackageDetails()
                            atPackageID.Value = dp("PackageID").ToString.Trim
                            ndBoxNumber.InnerText = dp("BoxNumber").ToString.Trim
                            ndBoxWeight.InnerText = dp("BoxWeight").ToString.Trim
                            ndShipDate.InnerText = dp("ShipDate").ToString.Trim
                            ndTrackingURL.InnerText = dp("TrackingURL").ToString.Trim
                            ndInvoiceNumber.InnerText = dp("InvoiceNumber").ToString.Trim
                            ndCarrierName.InnerText = dp("CarrierName").ToString.Trim
                            ndConsignmentNumber.InnerText = dp("ConsignmentNumber").ToString.Trim
                            '---------------------------------------------------------
                            '   Insert Contents SKU numbers

                            ndSKU = MyBase.xmlDoc.CreateElement("SKU")
                            ndSKU.InnerText = dp("PartNumber")
                            sku = dp("PartNumber").ToString.Trim
                            ndContents.AppendChild(ndSKU)

                            ndQuantity = MyBase.xmlDoc.CreateElement("Quantity")
                            ndQuantity.InnerText = dp("Quantity")
                            ndContents.AppendChild(ndQuantity)
                            '-------------------------------------------------------------
                            ' Check for corresponding CustomerSKU in line detail for order
                            '-------------------------------------------------------------
                            ndCustomerSKU = MyBase.xmlDoc.CreateElement("CustomerSKU")
                            ndCustomerSKU.InnerText = String.Empty
                            foundRowsDetail = dtDetail.Select("Sku = '" & sku & "'")
                            If foundRowsDetail.Length > 0 Then
                                ndCustomerSKU.InnerText = foundRowsDetail(0).Item("CustomerSKU").ToString.Trim
                            End If
                            ndContents.AppendChild(ndCustomerSKU)

                            ndSkuSerialNumber = MyBase.xmlDoc.CreateElement("SkuSerialNumber")
                            ndContents.AppendChild(ndSkuSerialNumber)

                            AppendSerialNumbers(currentPackage, dp("OrderNumber").ToString)
                           
                            '
                            'Dim dt As DataRow = Nothing
                            'For Each dt In dtProduct.Rows
                            '    If dt("OrderNumber").Equals(OrderNumber) And dt("PackageID").Equals(dt("PackageID")) Then
                            '        ndSKU = MyBase.xmlDoc.CreateElement("SKU")
                            '        ndSKU.InnerText = dt("SKU")
                            '        ndContents.AppendChild(ndSKU)
                            '    End If
                            'Next dt
                            ' err = AppendPackageDetails()
                        Else
                            ' Same package - just greate a new Part No
                            ndSKU = MyBase.xmlDoc.CreateElement("SKU")
                            ndSKU.InnerText = dp("PartNumber")
                            ndContents.AppendChild(ndSKU)
                            sku = dp("PartNumber").ToString.Trim

                            ndQuantity = MyBase.xmlDoc.CreateElement("Quantity")
                            ndQuantity.InnerText = dp("Quantity")
                            ndContents.AppendChild(ndQuantity)

                            '-------------------------------------------------------------
                            ' Check for corresponding CustomerSKU in line detail for order
                            '-------------------------------------------------------------
                            ndCustomerSKU = MyBase.xmlDoc.CreateElement("CustomerSKU")
                            ndCustomerSKU.InnerText = String.Empty
                            foundRowsDetail = dtDetail.Select("Sku = '" & sku & "'")
                            If foundRowsDetail.Length > 0 Then
                                ndCustomerSKU.InnerText = foundRowsDetail(0).Item("CustomerSKU").ToString.Trim
                            End If
                            ndContents.AppendChild(ndCustomerSKU)

                            ndSkuSerialNumber = MyBase.xmlDoc.CreateElement("SkuSerialNumber")
                            ndContents.AppendChild(ndSkuSerialNumber)

                            AppendSerialNumbers(dp("OrderLine").ToString, dp("OrderNumber").ToString)

                        End If
                    Else
                        '--------------------------------------------------------
                        '   no record create blanks
                        '
                        'err = CreatePackageDetails()
                        'err = AppendPackageDetails()
                        '
                    End If

                    ndCarrier.AppendChild(ndPackage)
                Next dp
                ' Create last one..
                If dtPackage.Rows.Count > 0 Then
                    err = AppendPackageDetails()
                End If
            Catch ex As Exception

                Const strError As String = "Failed to Insert Order Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOT-17"
                    .HasError = True
                End With
                
            End Try
            Return err
        End Function
        Private Function CreatePackageDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndPackage = .CreateElement("OrderSuffix")
                    atPackageID = .CreateAttribute("ID")
                    ndPackage.Attributes.Append(atPackageID)
                    '---------------------------------------------------------
                    ndBoxNumber = .CreateElement("BoxNumber")
                    ndBoxWeight = .CreateElement("BoxWeight")
                    ndShipDate = .CreateElement("ShipDate")
                    ndTrackingURL = .CreateElement("TrackingURL")
                    ndInvoiceNumber = .CreateElement("InvoiceNumber")
                    ndCarrierName = .CreateElement("CarrierName")
                    ndConsignmentNumber = .CreateElement("ConsignmentNumber")
                    ndContents = .CreateElement("Contents")
                    '   ndSkuSerialNumber = .CreateElement("SkuSerialNumber")
                    '---------------------------------------------------------
                End With
            Catch ex As Exception

                Const strError As String = "Failed to Create Order Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOT-18"
                    .HasError = True
                End With

            End Try
            Return err
        End Function
        Private Function AppendPackageDetails() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                ndOrderSuffix.AppendChild(ndPackage)
                '---------------------------------------------------------
                With ndPackage
                    .AppendChild(ndBoxNumber)
                    .AppendChild(ndBoxWeight)
                    .AppendChild(ndShipDate)
                    .AppendChild(ndTrackingURL)
                    .AppendChild(ndInvoiceNumber)
                    .AppendChild(ndCarrierName)
                    .AppendChild(ndConsignmentNumber)
                    .AppendChild(ndContents)
                    '     .AppendChild(ndSkuSerialNumber)
                End With
            Catch ex As Exception

                Const strError As String = "Failed to Append Order Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOT-19"
                    .HasError = True
                End With
           

            End Try
            Return err
        End Function
        Private Function AppendSerialNumbers(ByVal packageNo As String, ByVal orderNo As String) As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                Dim dr2 As DataRow
                For Each dr2 In dtProduct.Rows
                    If dr2("OrderNumber").Equals(OrderNumber) _
                    And dr2("PackageID").Equals(packageNo) Then
                        ndSerialNumber = MyBase.xmlDoc.CreateElement("SerialNumber")
                        ndSerialNumber.InnerText = dr2("SerialNumber")
                        ndSkuSerialNumber.AppendChild(ndSerialNumber)
                    End If
                Next

            Catch ex As Exception

                Const strError As String = "Failed to Append Order Detail Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOT-20"
                    .HasError = True
                End With


            End Try
            Return err
        End Function
    End Class
End Namespace