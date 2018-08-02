Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with payment responses
'
'       Date                        May 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSPY- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlPaymentResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndPayment, ndProductDetails, ndConfirmedDetails, ndReturnCode, ndResponse As XmlNode
        Private ndShoppingBasket, ndProductCode, ndCustomerNo, _
                ndSeat, ndPriceCode, ndPriceBand, ndPrice, ndCarriageMethod, ndCardPrint As XmlNode
        Private ndWSFee, ndCreditcardFee, ndCarriageFee, ndSeatRestriction As XmlNode
        Private ndPaymentReference, ndTotalPrice As XmlNode
        Private dtPaymentDetails As DataTable
        Private dtShoppingBasket As DataTable
        Private dtStatusDetails As DataTable
        Private errorOccurred As Boolean = False

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndPayment = .CreateElement("Payment")
                    ndResponse = .CreateElement("Response")
                    ndConfirmedDetails = .CreateElement("ConfirmedDetails")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Create the customer detail section
                CreatePaymentSEction()

                'Populate the xml document
                With ndPayment
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndConfirmedDetails)
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
                ndHeader.InsertAfter(ndPayment, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSPY-01"
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
            Else
                With MyBase.xmlDoc
                    ndPaymentReference = .CreateElement("PaymentReference")
                    ndTotalPrice = .CreateElement("TotalPrice")
                End With
                ndPaymentReference.InnerText = dr("PaymentReference")
                ndTotalPrice.InnerText = dr("TotalPrice")
            End If

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndReturnCode)
            End With


        End Sub

        Protected Sub CreatePaymentSection()

            Dim dr As DataRow
            Dim bWSFee As Boolean = False
            Dim bBKFee As Boolean = False
            Dim bCRFee As Boolean = False

            If Not errorOccurred Then

                ' Create the xml nodes needed for the confirmed details
                With MyBase.xmlDoc
                    ndShoppingBasket = .CreateElement("ShoppingBasket")
                    ndWSFee = .CreateElement("WSFee")
                    ndCreditcardFee = .CreateElement("CreditcardFee")
                    ndCarriageFee = .CreateElement("CarriageFee")
                End With

                'Read the values for the response section
                dtShoppingBasket = ResultDataSet.Tables(1)

                'Loop around the products
                For Each dr In dtShoppingBasket.Rows

                    If dr("FeeType") = "Y" Then

                        Select Case dr("ProductCode").ToString.Trim
                            Case Is = "BKFEE"
                                ndCreditcardFee.InnerText = dr("Price")
                                bBKFee = True
                            Case Is = "CRFEE"
                                ndCarriageFee.InnerText = dr("Price")
                                bCRFee = True
                            Case Is = "WSFEE"
                                ndWSFee.InnerText = dr("Price")
                                bWSFee = True
                            Case Is = "ATFEE"
                                ndWSFee.InnerText = dr("Price")
                                bWSFee = True
                        End Select

                    Else


                        'Create the product xml nodes
                        With MyBase.xmlDoc
                            ndProductDetails = .CreateElement("ProductDetails")
                            ndCustomerNo = .CreateElement("CustomerNo")
                            ndProductCode = .CreateElement("ProductCode")
                            ndSeat = .CreateElement("Seat")
                            ndPriceCode = .CreateElement("PriceCode")
                            ndPriceBand = .CreateElement("PriceBand")
                            ndPrice = .CreateElement("Price")
                            ndCarriageMethod = .CreateElement("CarriageMethod")
                            ndCardPrint = .CreateElement("CardPrint")
                            ndSeatRestriction = .CreateElement("SeatRestriction")
                        End With

                        'Populate the nodes
                        ndCustomerNo.InnerText = dr("CustomerNo")
                        ndProductCode.InnerText = dr("ProductCode")
                        ndSeat.InnerText = dr("Seat")
                        ndPriceCode.InnerText = dr("PriceCode")
                        ndPriceBand.InnerText = dr("PriceBand")
                        ndPrice.InnerText = dr("Price")
                        ndCarriageMethod.InnerText = dr("CarriageMethod")
                        ndCardPrint.InnerText = dr("CardPrint")
                        ndSeatRestriction.InnerText = dr("SeatRestriction")

                        'Set the xml nodes
                        With ndProductDetails
                            .AppendChild(ndCustomerNo)
                            .AppendChild(ndProductCode)
                            .AppendChild(ndSeat)
                            .AppendChild(ndPriceCode)
                            .AppendChild(ndPriceBand)
                            .AppendChild(ndPrice)
                            .AppendChild(ndCarriageMethod)
                            .AppendChild(ndCardPrint)
                            .AppendChild(ndSeatRestriction)
                        End With

                        'Add the product to the shopping basket
                        With ndShoppingBasket
                            .AppendChild(ndProductDetails)
                        End With
                    End If

                Next dr

                'Add the shopping basket to the confirmed details
                With ndConfirmedDetails
                    .AppendChild(ndPaymentReference)
                    If bWSFee = True Then .AppendChild(ndWSFee)
                    If bBKFee = True Then .AppendChild(ndCreditcardFee)
                    If bCRFee = True Then .AppendChild(ndCarriageFee)
                    .AppendChild(ndTotalPrice)
                    .AppendChild(ndShoppingBasket)
                End With


            End If

        End Sub

    End Class

End Namespace