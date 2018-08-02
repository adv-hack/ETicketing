Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with order change responses
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSOC- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlOrderChangeResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode


        Private ndOrderInfo, ndOrderNumbers, ndBranchOrderNumber, ndCustomerPO, ndOrderHeaderError As XmlNode

        Private ndOrderSuffix, ndLineInformation, ndOrderHeaderInformation As XmlNode

        Private atSuffix, atXmlNsXsi As XmlAttribute

        Private ndAddLine, ndLineError, ndSKU, ndQuantity, ndShippedQuantity, _
                 ndBackOrderQuantity, ndBackOrderETA, ndWestCoastLineNumber, ndCustomerLineNumber, _
                 ndUnitPrice, ndShipFromWarehouse, ndLocalCurrencyPrice, ndPriceDerivedFlag, _
                 ndForeignCurrency, ndCarrierCode, ndFreightRate, ndTransitDays, ndComment As XmlNode

        Private ndChangeLine, ndOldWestCoastLineNumber, ndNewWestCoastLineNumber,  ndSuffix As XmlNode
        '                              
        Private ndDeleteLine As XmlNode
        '                     
        Private ndAddComment As XmlNode
        '
        Private dtChangeShip As DataTable
        Private dtAddLine As DataTable
        Private dtChangeLine As DataTable
        Private dtDeleteLine As DataTable
        Private dtComment As DataTable
        Private dtHeaderErrors As DataTable
        Protected Overrides Sub InsertBodyV1()
            '------------------------------------------------------------------------------
            '   Seperate the tables out of the ResultSet    
            '
            Try
                With MyBase.xmlDoc
                    ndHeader = .CreateElement("OrderInfo")
                    ndOrderInfo = .CreateElement("OrderInfo")
                    If Not Err.HasError Then

                        dtHeader = ResultDataSet.Tables.Item(0)
                        dtDetail = ResultDataSet.Tables.Item(1)
                        dtText = ResultDataSet.Tables.Item(2)
                        dtCarrier = ResultDataSet.Tables.Item(3)
                        dtPackage = ResultDataSet.Tables.Item(4)
                        dtProduct = ResultDataSet.Tables.Item(5)

                        dtChangeShip = ResultDataSet.Tables.Item(6)
                        dtAddLine = ResultDataSet.Tables.Item(7)
                        dtChangeLine = ResultDataSet.Tables.Item(8)
                        dtDeleteLine = ResultDataSet.Tables.Item(9)
                        dtComment = ResultDataSet.Tables.Item(10)
                        dtHeaderErrors = ResultDataSet.Tables.Item(11)
                    End If
                    ndOrderNumbers = .CreateElement("OrderNumbers")
                    Err = InsertHeader()
                    '--------------------------------------------------------------------------------------
                    '   Insert the fragment into the XML document
                    '
                    Const c1 As String = "//"                               ' Constants are faster at run time
                    Const c2 As String = "/TransactionHeader"
                    '
                    ndHeaderRoot = .SelectSingleNode(c1 & RootElement())
                    ndHeaderRootHeader = .SelectSingleNode(c1 & RootElement() & c2)
                    ' ndHeaderRoot.InsertAfter(ndHeader, ndHeaderRootHeader)

                    ndHeaderRoot.InsertAfter(ndOrderInfo, ndHeaderRootHeader)

                    'Insert the XSD reference & namespace as an attribute within the root node
                    atXmlNsXsi = CreateNamespaceAttribute()
                    ndHeaderRoot.Attributes.Append(atXmlNsXsi)
                End With
            Catch ex As Exception
            End Try
        End Sub

        Private Function InsertHeader() As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Try
                Dim dr As DataRow
                Dim OrderNumber As String = String.Empty
                If (Not dtDetail Is Nothing AndAlso dtDetail.Rows.Count > 0) Or _
                    (Not dtHeaderErrors Is Nothing AndAlso dtHeaderErrors.Rows.Count > 0) Then
                    For Each dr In dtHeader.Rows
                        err = CreateHeader()
                        '-----------------------------------------------------------
                        OrderNumber = dr("OrderNumber")
                        ndBranchOrderNumber.InnerText = OrderNumber
                        ndCustomerPO.InnerText = dr("CustomerPO")
                        '    ndOrderHeaderError.InnerText = dr("OrderHeaderError")
                        '    atSuffix.Value = dr("OrderSuffix")
                        '-----------------------------------------------------------
                        If dtAddLine.Rows.Count > 0 Then _
                                err = InsertAddLine(OrderNumber)
                        '
                        If dtChangeLine.Rows.Count > 0 Then _
                                err = InsertChangeLine(OrderNumber)
                        '
                        If dtDeleteLine.Rows.Count > 0 Then _
                                err = InsertDelete(OrderNumber)
                        '
                        If dtComment.Rows.Count > 0 Then _
                                err = InsertComment(OrderNumber)
                        '
                        err = AppendHeader()

                        ndOrderInfo.AppendChild(ndOrderNumbers)
                    Next
                    '-------------------------------------------------------------------------
                    ' Loop through header errors to report on cases where order not found, etc
                    '-------------------------------------------------------------------------
                    If Not dtHeaderErrors Is Nothing AndAlso dtHeaderErrors.Rows.Count > 0 Then
                        For Each dr In dtHeaderErrors.Rows
                            err = CreateHeader()
                            OrderNumber = dr("OrderNo")
                            ndBranchOrderNumber.InnerText = OrderNumber
                            ndCustomerPO.InnerText = dr("CustomerPO")
                            ndOrderHeaderError.InnerText = dr("ErrorMessage")
                            err = AppendHeader()
                            ndOrderInfo.AppendChild(ndOrderNumbers)
                        Next
                    End If
                Else
                    '-----------------------------------------
                    '   No Items so create Dummy
                    '
                    err = CreateHeader()
                    err = AppendHeader()
                    ndOrderInfo.AppendChild(ndOrderNumbers)
                    '
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-18"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreateHeader() As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndOrderHeaderInformation = .CreateElement("OrderHeaderInformation")
                    ndBranchOrderNumber = .CreateElement("BranchOrderNumber")
                    ndCustomerPO = .CreateElement("CustomerPO")
                    ndOrderHeaderError = .CreateElement("OrderHeaderError")
                    ndOrderSuffix = .CreateElement("OrderSuffix")
                    atSuffix = .CreateAttribute("Suffix")
                    ndLineInformation = .CreateElement("LineInformation")
                    '-----------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-19"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendHeader() As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Try
                With ndOrderNumbers
                    .AppendChild(ndBranchOrderNumber)
                    .AppendChild(ndCustomerPO)
                    .AppendChild(ndOrderHeaderError)
                    .AppendChild(ndOrderSuffix)
                End With
                With ndOrderSuffix
                    .Attributes.Append(atSuffix)
                    .AppendChild(ndLineInformation)
                End With
                '-----------------------------------------------------------------
            Catch ex As Exception
                Const strError As String = "Failed to Append Header Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-20"
                    .HasError = True
                End With
            End Try
            Return err
        End Function


        Private Function InsertDelete(ByVal OrderNumber As String) As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Try
                Dim dr As DataRow
                If Not dtDetail Is Nothing AndAlso dtDetail.Rows.Count > 0 Then
                    For Each dr In dtDeleteLine.Rows
                        If dr("OrderNo").Equals(OrderNumber) Then
                            err = CreateDelete()
                            '-----------------------------------------------------------
                            If dr("ErrorCode").ToString <> String.Empty Then
                                ndLineError.InnerText = dr("ErrorCode") & " - " & dr("ErrorMessage")
                            End If
                            ndWestCoastLineNumber.InnerText = dr("LineNo")
                            '-----------------------------------------------------------
                            err = AppendDelete()
                        End If
                    Next
                Else
                    '---------------------------------------------------------------------------
                    '   No Items so create Dummy
                    '
                    err = CreateDelete()
                    err = AppendDelete()
                    '
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert Deleted Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-18"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreateDelete() As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndDeleteLine = .CreateElement("DeleteLine")
                    ndLineError = .CreateElement("LineError")
                    ndWestCoastLineNumber = .CreateElement("WestCoastLineNumber")
                    '-----------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Deleted Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-19"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendDelete() As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Try
                With ndDeleteLine
                    .AppendChild(ndLineError)
                    .AppendChild(ndWestCoastLineNumber)
                End With
                '-----------------------------------------------------------------
                ndLineInformation.AppendChild(ndDeleteLine)
            Catch ex As Exception
                Const strError As String = "Failed to Append Deleted Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-20"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertComment(ByVal OrderNumber As String) As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Try
                Dim dr As DataRow
                If Not dtDetail Is Nothing AndAlso dtDetail.Rows.Count > 0 Then
                    For Each dr In dtComment.Rows
                        If dr("OrderNo").Equals(OrderNumber) Then
                            err = CreateComment()
                            '-----------------------------------------------------------
                            If dr("ErrorCode").ToString <> String.Empty Then
                                ndLineError.InnerText = dr("ErrorCode") & " - " & dr("ErrorMessage")
                            End If

                            ndComment.InnerText = dr("Comment")
                            'ndWestCoastLineNumber.InnerText = dr("WestCoastLineNumber")
                            'ndSuffix.InnerText = dr("Suffix")
                            '-----------------------------------------------------------
                            err = AppendComment()
                        End If
                    Next
                Else
                    '---------------------------------------------------------------------------
                    '   No Items so create Dummy
                    '
                    err = CreateComment()
                    err = AppendComment()
                    '
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert Comment Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-18"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreateComment() As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndAddComment = .CreateElement("AddComment")
                    ndLineError = .CreateElement("LineError")
                    ndComment = .CreateElement("Comment")
                    ndWestCoastLineNumber = .CreateElement("WestCoastLineNumber")
                    ndSuffix = .CreateElement("Suffix")
                    '-----------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create Commentd Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-19"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendComment() As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                With ndAddComment
                    .AppendChild(ndLineError)
                    .AppendChild(ndComment)
                    .AppendChild(ndWestCoastLineNumber)
                    .AppendChild(ndSuffix)
                End With
                '-----------------------------------------------------------------
                ndLineInformation.AppendChild(ndAddComment)
            Catch ex As Exception
                Const strError As String = "Failed to Append Comment Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-20"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertAddLine(ByVal OrderNumber As String) As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Try
                Dim dr As DataRow
                If Not dtDetail Is Nothing AndAlso dtDetail.Rows.Count > 0 Then
                    For Each dr In dtAddLine.Rows
                        If dr("OrderNo").Equals(OrderNumber) Then
                            err = CreateAddLine()
                            If dr("ErrorCode").ToString <> String.Empty Then
                                ndLineError.InnerText = dr("ErrorCode") & " - " & dr("ErrorMessage")
                            End If

                            ndSKU.InnerText = dr("SKU")
                            ndQuantity.InnerText = dr("Quantity")
                            ndUnitPrice.InnerText = dr("Price")
                            ndWestCoastLineNumber.InnerText = dr("LineNo")
                            ndLocalCurrencyPrice.InnerText = dr("Price")
                            '---------------------
                            ' Retrieve Line detail
                            '---------------------
                            Dim foundRows As DataRow()
                            If dr("LineNo").ToString <> String.Empty Then
                                foundRows = dtDetail.Select("LineNumber = '" & CInt(dr("LineNo")) & "'")
                                If Not foundRows(0) Is Nothing Then
                                    ' ndShippedQuantity.InnerText = dr("ShippedQuantity")
                                    ndBackOrderQuantity.InnerText = foundRows(0)("BackOrderQuantity")
                                    ndBackOrderETA.InnerText = Utilities.CheckForDBNull_String(foundRows(0)("BackOrderETADate"))
                                    '    ndCustomerLineNumber.InnerText = dr("CustomerLineNumber")
                                    ndShipFromWarehouse.InnerText = foundRows(0)("ShipFromBranch")
                                    'ndPriceDerivedFlag.InnerText = dr("PriceDerivedFlag")
                                    ' ndForeignCurrency.InnerText = dr("ForeignCurrency")
                                    ' ndCarrierCode.InnerText = dr("CarrierCode")
                                    ' ndFreightRate.InnerText = dr("FreightRate")
                                    ' ndTransitDays.InnerText = dr("TransitDays")
                                End If
                            End If
                            '---------------------------------------------------------------------------
                            err = AppendAddLine()
                        End If
                    Next
                Else
                    '---------------------------------------------------------------------------
                    '   No Items so create Dummy
                    '
                    err = CreateAddLine()
                    err = AppendAddLine()
                    '
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert AddLine Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-18"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreateAddLine() As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndAddLine = .CreateElement("AddLine")
                    ndLineError = .CreateElement("LineError")
                    ndSKU = .CreateElement("SKU")
                    ndQuantity = .CreateElement("Quantity")
                    ndShippedQuantity = .CreateElement("ShippedQuantity")
                    ndBackOrderQuantity = .CreateElement("BackOrderQuantity")
                    ndBackOrderETA = .CreateElement("BackOrderETA")
                    ndWestCoastLineNumber = .CreateElement("WestCoastLineNumber")
                    ndCustomerLineNumber = .CreateElement("CustomerLineNumber")
                    ndUnitPrice = .CreateElement("UnitPrice")
                    ndShipFromWarehouse = .CreateElement("ShipFromWarehouse")
                    ndLocalCurrencyPrice = .CreateElement("LocalCurrencyPrice")
                    ndPriceDerivedFlag = .CreateElement("PriceDerivedFlag")
                    ndForeignCurrency = .CreateElement("ForeignCurrency")
                    ndCarrierCode = .CreateElement("CarrierCode")
                    ndFreightRate = .CreateElement("FreightRate")
                    ndTransitDays = .CreateElement("TransitDays")
                    '-----------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create AddLine Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-19"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendAddLine() As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Try
                With ndAddLine
                    .AppendChild(ndLineError)
                    .AppendChild(ndSKU)
                    .AppendChild(ndQuantity)
                    .AppendChild(ndShippedQuantity)
                    .AppendChild(ndBackOrderQuantity)
                    .AppendChild(ndBackOrderETA)
                    .AppendChild(ndWestCoastLineNumber)
                    .AppendChild(ndCustomerLineNumber)
                    .AppendChild(ndUnitPrice)
                    .AppendChild(ndShipFromWarehouse)
                    .AppendChild(ndLocalCurrencyPrice)
                    .AppendChild(ndPriceDerivedFlag)
                    .AppendChild(ndForeignCurrency)
                    .AppendChild(ndCarrierCode)
                    .AppendChild(ndFreightRate)
                    .AppendChild(ndTransitDays)
                End With
                '-----------------------------------------------------------------
                ndLineInformation.AppendChild(ndAddLine)
            Catch ex As Exception
                Const strError As String = "Failed to Append AddLine Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-20"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function InsertChangeLine(ByVal OrderNumber As String) As ErrorObj
            Dim err As ErrorObj = Nothing
            '--------------------------------------------------------------------------
            Try
                Dim dr As DataRow
                If Not dtDetail Is Nothing AndAlso dtDetail.Rows.Count > 0 Then
                    For Each dr In dtChangeLine.Rows
                        If dr("OrderNo").Equals(OrderNumber) Then
                            err = CreateChangeLine()
                            '-----------------------------------------------------------
                            If dr("ErrorCode").ToString <> String.Empty Then
                                ndLineError.InnerText = dr("ErrorCode") & " - " & dr("ErrorMessage")
                            End If
                            ndSKU.InnerText = dr("SKU")
                            ndQuantity.InnerText = dr("Quantity")
                            'ndShippedQuantity.InnerText = dr("ShippedQuantity")
                            ndOldWestCoastLineNumber.InnerText = dr("OldLineNo")
                            ndNewWestCoastLineNumber.InnerText = dr("LineNo")
                            'ndCustomerLineNumber.InnerText = dr("CustomerLineNumber")
                            ndUnitPrice.InnerText = dr("Price")

                            'ndSuffix.InnerText = dr("Suffix")
                            'ndLocalCurrencyPrice.InnerText = dr("LocalCurrencyPrice")
                            'ndPriceDerivedFlag.InnerText = dr("PriceDerivedFlag")
                            'ndForeignCurrency.InnerText = dr("ForeignCurrency ")
                            'ndCarrierCode.InnerText = dr("CarrierCode")
                            'ndFreightRate.InnerText = dr("FreightRate")
                            'ndTransitDays = dr("TransitDays")
                            '---------------------
                            ' Retrieve Line detail
                            '---------------------
                            Dim foundRows As DataRow()
                            If dr("LineNo").ToString <> String.Empty Then
                                foundRows = dtDetail.Select("LineNumber = '" & CInt(dr("LineNo")) & "'")
                                If Not foundRows(0) Is Nothing Then
                                    ndBackOrderQuantity.InnerText = foundRows(0)("BackOrderQuantity")
                                    ndBackOrderETA.InnerText = Utilities.CheckForDBNull_String(foundRows(0)("BackOrderETADate"))
                                    ' ndShipFromWarehouse.InnerText = foundRows(0)("ShipFromBranch")
                                End If
                            End If
                            err = AppendChangeLine()
                        End If
                    Next
                Else
                    '---------------------------------------------------------------------------
                    '   No Items so create Dummy
                    '
                    err = CreateChangeLine()
                    err = AppendChangeLine()
                    '
                End If
            Catch ex As Exception
                Const strError As String = "Failed to Insert ChangeLine Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-18"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function CreateChangeLine() As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Try
                With MyBase.xmlDoc
                    ndChangeLine = .CreateElement("ChangeLine")
                    ndLineError = .CreateElement("LineError")
                    ndSKU = .CreateElement("SKU")
                    ndQuantity = .CreateElement("Quantity")
                    ndShippedQuantity = .CreateElement("ShippedQuantity")
                    ndBackOrderQuantity = .CreateElement("BackOrderQuantity")
                    ndBackOrderETA = .CreateElement("BackOrderETA")
                    ndOldWestCoastLineNumber = .CreateElement("OldWestCoastLineNumber")
                    ndNewWestCoastLineNumber = .CreateElement("NewWestCoastLineNumber")
                    ndCustomerLineNumber = .CreateElement("CustomerLineNumber")
                    ndUnitPrice = .CreateElement("UnitPrice")
                    ndSuffix = .CreateElement("Suffix")
                    ndLocalCurrencyPrice = .CreateElement("LocalCurrencyPrice")
                    ndPriceDerivedFlag = .CreateElement("PriceDerivedFlag")
                    ndForeignCurrency = .CreateElement("ForeignCurrency")
                    ndCarrierCode = .CreateElement("CarrierCode")
                    ndFreightRate = .CreateElement("FreightRate")
                    ndTransitDays = .CreateElement("TransitDays")
                    '-----------------------------------------------------------------
                End With
            Catch ex As Exception
                Const strError As String = "Failed to Create ChangeLine Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-19"
                    .HasError = True
                End With
            End Try
            Return err
        End Function
        Private Function AppendChangeLine() As ErrorObj
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Try
                With ndChangeLine
                    .AppendChild(ndLineError)
                    .AppendChild(ndSKU)
                    .AppendChild(ndQuantity)
                    .AppendChild(ndShippedQuantity)
                    .AppendChild(ndBackOrderQuantity)
                    .AppendChild(ndBackOrderETA)
                    .AppendChild(ndOldWestCoastLineNumber)
                    .AppendChild(ndNewWestCoastLineNumber)
                    .AppendChild(ndCustomerLineNumber)
                    .AppendChild(ndUnitPrice)
                    .AppendChild(ndSuffix)
                    .AppendChild(ndLocalCurrencyPrice)
                    .AppendChild(ndPriceDerivedFlag)
                    .AppendChild(ndForeignCurrency)
                    .AppendChild(ndCarrierCode)
                    .AppendChild(ndFreightRate)
                    .AppendChild(ndTransitDays)
                End With
                ndLineInformation.AppendChild(ndChangeLine)
                '-----------------------------------------------------------------
            Catch ex As Exception
                Const strError As String = "Failed to Append Change Line Nodes"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPO-20"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace