Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with ticketing print request
'
'       Date                        September 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSPR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlOrderDetailTicketingResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndOrderDetail, ndResponse, ndOrderDetails, ndReturnCode As XmlNode
        Private ndQuantity, ndProductDetails, ndCustomerNo, ndProductCode, _
                ndProductDescription, ndProductDate, ndProductTime, ndSeat, _
                ndPriceCode, ndPriceBand, ndPrice, ndTurnstiles, _
                ndGates, ndBarcodeValue, ndTicketID As XmlNode
        Private dtOrderDetails As DataTable
        Private dtStatusDetails As DataTable
        Private errorOccurred As Boolean = False

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndOrderDetail = .CreateElement("OrderDetail")
                    ndResponse = .CreateElement("Response")
                    ndOrderDetails = .CreateElement("OrderDetails")
                    ndQuantity = .CreateElement("Quantity")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Create the customer detail section
                CreateOrderDetail()

                'Populate the xml document
                With ndOrderDetail
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndOrderDetails)
                    End If
                End With

                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndOrderDetail, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPR-01"
                    .HasError = True
                End With
            End Try

        End Sub

        Protected Sub CreateResponseSection()

            Dim dr As DataRow

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndReturnCode = .CreateElement("ReturnCode")
            End With

            'Read the values for the response section
            dtStatusDetails = ResultDataSet.Tables(0)
            dr = dtStatusDetails.Rows(0)

            'Populate the nodes
            ndReturnCode.InnerText = dr("ReturnCode")
            If dr("ErrorOccurred") = "E" Then
                errorOccurred = True
            End If

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndReturnCode)
            End With


        End Sub

        Protected Sub CreateOrderDetail()

            Dim dr As DataRow

            If Not errorOccurred Then

                'Read the values for the order section
                dtOrderDetails = ResultDataSet.Tables(1)
                dr = dtOrderDetails.Rows(0)

                'Add the quantity
                ndQuantity.InnerText = dtOrderDetails.Rows.Count.ToString
                ndOrderDetails.AppendChild(ndQuantity)

                'Loop around the order details
                For Each dr In dtOrderDetails.Rows

                    'Create the xml nodes
                    With MyBase.xmlDoc
                        ndProductDetails = .CreateElement("ProductDetails")
                        ndCustomerNo = .CreateElement("CustomerNo")
                        ndProductCode = .CreateElement("ProductCode")
                        ndProductDescription = .CreateElement("ProductDescription")
                        ndProductDate = .CreateElement("ProductDate")
                        ndProductTime = .CreateElement("ProductTime")
                        ndSeat = .CreateElement("Seat")
                        ndPriceCode = .CreateElement("PriceCode")
                        ndPriceBand = .CreateElement("PriceBand")
                        ndTurnstiles = .CreateElement("Turnstiles")
                        ndGates = .CreateElement("Gates")
                        ndBarcodeValue = .CreateElement("BarcodeValue")
                        ndTicketID = .CreateElement("TicketID")
                    End With

                    'Populate the nodes
                    ndCustomerNo.InnerText = dr("CustomerNo")
                    ndProductCode.InnerText = dr("ProductCode")
                    ndProductDescription.InnerText = dr("ProductDescription")
                    ndProductDate.InnerText = dr("ProductDate")
                    ndProductTime.InnerText = dr("ProductTime")
                    ndSeat.InnerText = dr("Seat")
                    ndPriceCode.InnerText = dr("PriceCode")
                    ndPriceBand.InnerText = dr("PriceBand")
                    ndTurnstiles.InnerText = dr("Turnstiles")
                    ndGates.InnerText = dr("Gates")
                    ndBarcodeValue.InnerText = dr("BarcodeValue")
                    ndTicketID.InnerText = dr("TicketId")

                    'Set the xml nodes
                    With ndProductDetails
                        .AppendChild(ndCustomerNo)
                        .AppendChild(ndProductCode)
                        .AppendChild(ndProductDescription)
                        .AppendChild(ndProductDate)
                        .AppendChild(ndProductTime)
                        .AppendChild(ndSeat)
                        .AppendChild(ndPriceCode)
                        .AppendChild(ndPriceBand)
                        .AppendChild(ndTurnstiles)
                        .AppendChild(ndGates)
                        .AppendChild(ndBarcodeValue)
                        .AppendChild(ndTicketID)
                    End With

                    'Add the product to the product list
                    With ndOrderDetails
                        .AppendChild(ndProductDetails)
                    End With

                Next dr

            End If

        End Sub

    End Class

End Namespace