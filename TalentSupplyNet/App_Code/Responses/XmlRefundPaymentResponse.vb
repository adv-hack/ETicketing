Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with refund payment responses
'
'       Date                        May 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSRPR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlRefundPaymentResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndRefundPaymentResponse, ndRefundPayment As XmlNode
        Private ndResponse, ndReturnCode, ndRefundReference, ndRefundTotal As XmlNode
        Private ndRefundDetails, ndProductDetails, ndProductCode, ndCustomerNo, ndSeat, ndPrice,
                ndPriceBand, ndPriceCode, ndNumberofCancellations As XmlNode
        Private DtRefundProductDetails As DataTable
        Private DtRefundPaymentDetails As DataTable
        Private DtStatusResults As DataTable
        Private errorOccurred As Boolean = False

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndRefundPaymentResponse = .CreateElement("RefundPaymentResponse")
                    ndRefundPayment = .CreateElement("RefundPayment")
                End With

                'Create the response xml section
                CreateResponseSection(True)

                'Create the customer detail section
                CreateRefundPaymentSection(False)

                'Populate the xml document
                With ndRefundPayment
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndRefundDetails)
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
                ndHeader.InsertAfter(ndRefundPayment, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSRPY-01"
                    .HasError = True
                End With
            End Try

        End Sub

        Protected Overrides Sub InsertBodyV1_1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndRefundPaymentResponse = .CreateElement("RefundPaymentResponse")
                    ndRefundPayment = .CreateElement("RefundPayment")
                End With

                'Create the response xml section
                CreateResponseSection(False)

                'Create the customer detail section
                CreateRefundPaymentSection(True)

                'Populate the xml document
                With ndRefundPayment
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndRefundDetails)
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
                ndHeader.InsertAfter(ndRefundPayment, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSRPY-02"
                    .HasError = True
                End With
            End Try

        End Sub

        Protected Sub CreateResponseSection(ByVal incRefundReference As Boolean)

            Dim dr0 As DataRow
            Dim dr1 As DataRow

            'Create the response xml nodes
            With MyBase.xmlDoc
                ndResponse = .CreateElement("Response")
                ndReturnCode = .CreateElement("ReturnCode")
                If incRefundReference Then
                    ndRefundReference = .CreateElement("RefundReference")
                End If
                ndRefundTotal = .CreateElement("RefundTotal")
            End With

            'Read the values for the response section
            DtStatusResults = ResultDataSet.Tables(0)
            dr0 = DtStatusResults.Rows(0)

            'Populate the nodes
            With ndResponse

                ndReturnCode.InnerText = dr0("ReturnCode")
                .AppendChild(ndReturnCode)
                If dr0("ErrorOccurred") = "E" Then
                    errorOccurred = True
                Else
                    DtRefundPaymentDetails = ResultDataSet.Tables(1)
                    dr1 = DtRefundPaymentDetails.Rows(0)
                    If incRefundReference Then
                        ndRefundReference.InnerText = dr1("RefundReference")
                    End If
                    'refund Total already received in the correct format  ndRefundTotal.InnerText = Utilities.FormatPrice(dr1("RefundTotal"))
                    ndRefundTotal.InnerText = dr1("RefundTotal")
                    If incRefundReference Then
                        .AppendChild(ndRefundReference)
                    End If
                    .AppendChild(ndRefundTotal)
                    End If

            End With


        End Sub

        Protected Sub CreateRefundPaymentSection(ByVal incCancelAllMatching As Boolean)

            Dim dr2 As DataRow

            If Not errorOccurred Then

                'Read the values for the response section
                DtRefundProductDetails = ResultDataSet.Tables(2)

                ' Create and populate the xml nodes for RefundReference and RefundTotal
                With MyBase.xmlDoc
                    ndRefundDetails = .CreateElement("RefundDetails")
                End With

                'Loop around the refunded products
                For Each dr2 In DtRefundProductDetails.Rows

                    'Create the product xml nodes
                    With MyBase.xmlDoc
                        ndProductDetails = .CreateElement("ProductDetails")
                        ndCustomerNo = .CreateElement("CustomerNo")
                        ndProductCode = .CreateElement("ProductCode")
                        If incCancelAllMatching Then
                            ndPriceBand = .CreateElement("PriceBand")
                            ndPriceCode = .CreateElement("PriceCode")
                            ndNumberofCancellations = .CreateElement("NumberOfCancellations")
                            ndRefundTotal = .CreateElement("RefundTotal")
                            ndRefundReference = .CreateElement("RefundReference")
                        Else
                            ndSeat = .CreateElement("Seat")
                            ndPrice = .CreateElement("Price")
                        End If
                    End With

                    'Populate the nodes
                    ndCustomerNo.InnerText = dr2("CustomerNo")
                    ndProductCode.InnerText = dr2("ProductCode")
                    If incCancelAllMatching Then
                        ndPriceBand.InnerText = dr2("PriceBand")
                        ndPriceCode.InnerText = dr2("PriceCode")
                        ndNumberofCancellations.InnerText = dr2("NumberOfCancellations")
                        If dr2("RefundTotal").ToString.Trim = String.Empty Then
                            ndRefundTotal.InnerText = String.Empty
                        Else
                            ndRefundTotal.InnerText = dr2("RefundTotal")
                        End If
                        ndRefundReference.InnerText = dr2("RefundReference")
                    Else
                        ndSeat.InnerText = dr2("Seat")
                        If dr2("Price").ToString.Trim = String.Empty Then
                            ndPrice.InnerText = String.Empty
                        Else
                            ndPrice.InnerText = Utilities.FormatPrice(dr2("Price"))
                        End If
                    End If

                    'Set the xml nodes
                    With ndProductDetails
                        .AppendChild(ndCustomerNo)
                        .AppendChild(ndProductCode)
                        If incCancelAllMatching Then
                            .AppendChild(ndPriceBand)
                            .AppendChild(ndPriceCode)
                            .AppendChild(ndNumberofCancellations)
                            .AppendChild(ndRefundTotal)
                            .AppendChild(ndRefundReference)
                        Else
                            .AppendChild(ndSeat)
                            .AppendChild(ndPrice)
                        End If
                    End With

                    'Add the product to the shopping basket
                    With ndRefundDetails
                        .AppendChild(ndProductDetails)
                    End With

                Next dr2

            End If

        End Sub

    End Class

End Namespace