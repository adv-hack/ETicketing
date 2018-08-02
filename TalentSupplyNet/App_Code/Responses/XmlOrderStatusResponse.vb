Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Status Response
'
'       Date                        8th Nov 2006
'
'       Author                      Andy White
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSOS- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlOrderStatusResponse
        Inherits XmlResponse
        '--------------------------------------------------------------------------------------
        Private ndHeaderRoot, ndHeaderRootHeader As XmlNode
        Private atXmlNsXsi As XmlAttribute
        '
        Private ndHeader, ndOrderNumbers, ndBranchOrderNumber, _
                ndCustomerPO, ndErrorStatus, ndOrderEntryDate, ndOrderStatus, ndPackage As XmlNode

        Private ndShipFromBranch, ndLineStatus, ndTotalSales, ndInvoiceNumber, ndInvoiceDate, _
                ndOrderShipDate, ndOrderCreditMemo As XmlNode

        Private atErrorNumber, atSuffix, atOrderCreditMemoCode As XmlAttribute
        Private iCounter As Integer = 0

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
            '--------------------------------------------------------------------------
            Dim iCounter As Integer = 0
            Dim dr As DataRow
            '
            Try
                With MyBase.xmlDoc
                    '----------------------------------------------------------------------
                    If Not dtHeader Is Nothing AndAlso dtHeader.Rows.Count > 0 Then
                        For Each dr In dtHeader.Rows
                            '------------------------------------------------------
                            err2 = CreateHeader()
                            ndBranchOrderNumber.InnerText = dr("BranchOrderNumber").ToString
                            If dr("BranchOrderNumber") <> "Not Found" Then
                                OrderNumber = dr("OrderNumber").ToString
                                ndCustomerPO.InnerText = dr("CustomerPO").ToString
                                Select Case dr("OrderStatus").ToString
                                    Case Is = "C"
                                        ndOrderStatus.InnerText = "Complete"
                                    Case Is = "X"
                                        ndOrderStatus.InnerText = "Cancelled"
                                    Case Else
                                        ndOrderStatus.InnerText = "Open"
                                End Select
                                ndOrderEntryDate.InnerText = dr("OrderEntryDate").ToString
                            End If
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
                                err2 = InsertCarrier(dr("OrderEntryDate"))
                        Next dr
                    Else
                        '----------------------------------------------------------
                        '   No order header, write dummy
                        '
                        err2 = CreateHeader()
                        err2 = AppendHeader()
                        err2 = CreatePackage()
                        err2 = AppendPackage()
                    End If
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Insert Order Header Nodes"
                With err2
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOS-14"
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
                    ndOrderEntryDate = .CreateElement("OrderEntryDate")
                    ndOrderStatus = .CreateElement("OrderStatus")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOS-15"
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
                    .AppendChild(ndOrderEntryDate)
                    .AppendChild(ndOrderStatus)
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Append Order Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOS-16"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertCarrier(ByVal OrderEntryDate As String) As ErrorObj
            Dim err As ErrorObj = Nothing
            '---------------------------------------------------------------------------------------------
            Dim dc As DataRow
            Try
                ''--------------------------------------------------------------
                'For Each dc In dtCarrier.Rows
                '    '----------------------------------------------------------
                '    If dc("OrderNumber").Equals(OrderNumber) Then
                '        err = CreateCarrier()
                '        ndOrderEntryDate.InnerText = OrderEntryDate
                '        ndCarrier.InnerText = dc("CarrierCode")
                '        atSuffix.Value = dc("CarrierCodeDescription")
                '        ndCarrier.Attributes.Append(atSuffix)
                '        '------------------------------------------------------
                '        ndShipFromBranch.InnerText = dc("ShipFromBranch")
                '        ndOrderStatus.InnerText = dc("OrderStatus")
                '        ndTotalSales.InnerText = dc("TotalSales")
                '        ndInvoiceDate.InnerText = dc("InvoiceDate")
                '        ndOrderShipDate.InnerText = dc("OrderShipDate")
                '        ndOrderCreditMemo.InnerText = dc("OrderCreditMemo1")
                '        atOrderCreditMemoCode.Value = dc("OrderCreditMemoCode1")
                '        ndOrderCreditMemo.Attributes.Append(atOrderCreditMemoCode)
                '        err = AppendCarrier()
                '        '------------------------------------------------------
                '    End If
                'Next dc
                Dim currentPackage As String = String.Empty
                Dim totalSales As Decimal = 0
                For Each dc In dtPackage.Rows
                    If dc("OrderNumber").Equals(OrderNumber) Then
                        If dc("PackageID").ToString <> currentPackage Then
                            currentPackage = dc("PackageID")

                            err = CreatePackage()
                            atSuffix.Value = dc("PackageID")
                            ndPackage.Attributes.Append(atSuffix)
                            ndShipFromBranch.InnerText = dc("Stockroom")
                            ndTotalSales.InnerText = dc("InvoiceTotal").ToString
                            ndInvoiceNumber.InnerText = dc("InvoiceNumber").ToString.Trim
                            ndInvoiceDate.InnerText = dc("InvoiceDate").ToString
                            ndOrderShipDate.InnerText = dc("AllocationDate").ToString

                            err = AppendPackage()
                        End If
                    End If

                Next dc
            Catch ex As Exception
                Const strError As String = "Failed to Insert Carrier Node"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOS-02"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreatePackage() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndPackage = .CreateElement("OrderSuffix")
                    ndShipFromBranch = .CreateElement("ShipFromBranch")
                    '   ndOrderStatus = .CreateElement("OrderStatus")
                    ndTotalSales = .CreateElement("TotalSales")
                    ndInvoiceNumber = .CreateElement("InvoiceNumber")
                    ndInvoiceDate = .CreateElement("InvoiceDate")
                    ndOrderShipDate = .CreateElement("OrderShipDate")
                    ndOrderCreditMemo = .CreateElement("OrderCreditMemo")
                    atSuffix = .CreateAttribute("ID")
                    atOrderCreditMemoCode = .CreateAttribute("Code")
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Package Node"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOD-11"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendPackage() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                ndOrderNumbers.AppendChild(ndPackage)
                With ndPackage
                    .AppendChild(ndShipFromBranch)
                    '   .AppendChild(ndOrderStatus)
                    .AppendChild(ndTotalSales)
                    .AppendChild(ndInvoiceNumber)
                    .AppendChild(ndInvoiceDate)
                    .AppendChild(ndOrderShipDate)
                    .AppendChild(ndOrderCreditMemo)
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Append Package Node"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSOS-12"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        'Private Function CreateCarrier() As ErrorObj
        '    Dim err As ErrorObj = Nothing
        '    '--------------------------------------------------------------------------
        '    Try
        '        With MyBase.xmlDoc
        '            ndCarrier = .CreateElement("OrderSuffix")
        '            ndShipFromBranch = .CreateElement("ShipFromBranch")
        '            ndOrderStatus = .CreateElement("OrderStatus")
        '            ndTotalSales = .CreateElement("TotalSales")
        '            ndInvoiceDate = .CreateElement("InvoiceDate")
        '            ndOrderShipDate = .CreateElement("OrderShipDate")
        '            ndOrderCreditMemo = .CreateElement("OrderCreditMemo")
        '            atSuffix = .CreateAttribute("ID")
        '            atOrderCreditMemoCode = .CreateAttribute("Code")
        '        End With
        '    Catch ex As Exception
        '        Const strError As String = "Failed to Create Carrier Node"
        '        With err
        '            .ErrorMessage = ex.Message
        '            .ErrorStatus = strError
        '            .ErrorNumber = "TTPRSOD-11"
        '            .HasError = True
        '        End With
        '    End Try
        '    Return err
        'End Function
        'Private Function AppendCarrier() As ErrorObj
        '    Dim err As ErrorObj = Nothing
        '    '--------------------------------------------------------------------------
        '    Try
        '        ndOrderNumbers.AppendChild(ndCarrier)
        '        With ndCarrier
        '            .AppendChild(ndShipFromBranch)
        '            .AppendChild(ndOrderStatus)
        '            .AppendChild(ndTotalSales)
        '            .AppendChild(ndInvoiceDate)
        '            .AppendChild(ndOrderShipDate)
        '            .AppendChild(ndOrderCreditMemo)
        '        End With
        '    Catch ex As Exception
        '        Const strError As String = "Failed to Append Carrier Node"
        '        With err
        '            .ErrorMessage = ex.Message
        '            .ErrorStatus = strError
        '            .ErrorNumber = "TTPRSOS-12"
        '            .HasError = True
        '        End With
        '    End Try
        '    Return err
        'End Function


        Public Sub New()

        End Sub
    End Class
End Namespace