Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with amend ticketing items responses
'
'       Date                        June 2007
'
'       Author                       
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRSATI- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlAmendTicketingItemsResponse
        Inherits XmlResponse

        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndAmendTicketingItems, ndResponse, ndReservationDetails, ndReturnCode As XmlNode
        Private ndShoppingBasket, ndWSFee, ndCreditcardFee, ndCarriageFee, ndTotalPrice, _
                ndReservedQuantity As XmlNode
        Private ndCustomerNo, ndProductCode, ndProductDescription, ndSeat, ndPriceCode, _
                ndPriceBand, ndPrice, ndReservedSeat, ndErrorCode, ndUserLimit, _
                ndProductLimit, ndPreReqProduct, ndProductDetails As XmlNode
        Private dtReservationDetails As DataTable
        Private dtGlobalParameters As DataTable
        Private dtStatusDetails As DataTable
        Private errorOccurred As Boolean = False
        Private _deAmendTicketingItems As DEAmendTicketingItems
        Public Property DeAmendTicketingItems() As DEAmendTicketingItems
            Get
                Return _deAmendTicketingItems
            End Get
            Set(ByVal value As DEAmendTicketingItems)
                _deAmendTicketingItems = value
            End Set
        End Property

        Protected Overrides Sub InsertBodyV1()

            Try

                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    ndAmendTicketingItems = .CreateElement("AmendTicketingItems")
                    ndResponse = .CreateElement("Response")
                    ndReservationDetails = .CreateElement("TicketingItemsDetails")
                End With

                'Create the response xml section
                CreateResponseSection()

                'Create the customer detail section
                CreateBasketDetails()

                'Populate the xml document
                With ndAmendTicketingItems
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndReservationDetails)
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
                ndHeader.InsertAfter(ndAmendTicketingItems, ndHeaderRootHeader)

            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSFR-01"
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
            If Not (DeAmendTicketingItems.ErrorCode.Trim = "") Then
                ndReturnCode.InnerText = DeAmendTicketingItems.ErrorCode.Trim
                errorOccurred = True
            Else
                ndReturnCode.InnerText = dr("ReturnCode")
                If dr("ErrorOccurred") = "E" Then
                    errorOccurred = True
                End If
            End If

            'Set the xml nodes
            With ndResponse
                .AppendChild(ndReturnCode)
            End With


        End Sub

        Protected Sub CreateBasketDetails()

            Dim dr As DataRow
            Dim bWSFee As Boolean = False
            Dim bBKFee As Boolean = False
            Dim bCRFee As Boolean = False

            If Not errorOccurred Then

                'Read the global parameters
                dtGlobalParameters = ResultDataSet.Tables(1)
                dr = dtGlobalParameters.Rows(0)

                ' Create the xml nodes needed for the confirmed details
                With MyBase.xmlDoc
                    ndShoppingBasket = .CreateElement("ShoppingBasket")
                    ndWSFee = .CreateElement("WSFee")
                    ndCreditcardFee = .CreateElement("CreditcardFee")
                    ndCarriageFee = .CreateElement("CarriageFee")
                    ndTotalPrice = .CreateElement("TotalPrice")
                    ndReservedQuantity = .CreateElement("ReservedQuantity")
                End With

                'Populate the nodes
                ndReservedQuantity.InnerText = dr("ReservedQuantity")
                ndTotalPrice.InnerText = dr("TotalPrice")
                ndWSFee.InnerText = dr("WebServicesFee")
                ndCreditcardFee.InnerText = dr("BookingFee")
                ndCarriageFee.InnerText = dr("CarriageFee")
                If dr("ApplyWebServicesFee") = "Y" Then bWSFee = True
                If dr("ApplyBookingFee") = "Y" Then bBKFee = True
                If dr("ApplyCarriageFee") = "Y" Then bCRFee = True

                'Read the values for the reservation section
                dtReservationDetails = ResultDataSet.Tables(2)

                'Loop around the products
                For Each dr In dtReservationDetails.Rows

                    'Create the product xml nodes
                    With MyBase.xmlDoc
                        ndProductDetails = .CreateElement("ProductDetails")
                        ndCustomerNo = .CreateElement("CustomerNo")
                        ndProductCode = .CreateElement("ProductCode")
                        ndProductDescription = .CreateElement("ProductDescription")
                        ndSeat = .CreateElement("Seat")
                        ndPriceCode = .CreateElement("PriceCode")
                        ndPriceBand = .CreateElement("PriceBand")
                        ndPrice = .CreateElement("Price")
                        ndReservedSeat = .CreateElement("ReservedSeat")
                        ndErrorCode = .CreateElement("ErrorCode")
                        ndUserLimit = .CreateElement("UserLimit")
                        ndProductLimit = .CreateElement("ProductLimit")
                        ndPreReqProduct = .CreateElement("PreReqProduct")
                    End With

                    'Populate the nodes
                    ndCustomerNo.InnerText = dr("CustomerNo")
                    ndProductCode.InnerText = dr("ProductCode")
                    ndProductDescription.InnerText = dr("ProductDescription")
                    ndSeat.InnerText = dr("Seat")
                    ndPriceCode.InnerText = dr("PriceCode")
                    ndPriceBand.InnerText = dr("PriceBand")
                    ndPrice.InnerText = dr("Price")
                    ndReservedSeat.InnerText = dr("ReservedSeat")
                    ndErrorCode.InnerText = dr("ErrorCode")
                    ndUserLimit.InnerText = dr("UserLimit")
                    ndProductLimit.InnerText = dr("ProductLimit")
                    ndPreReqProduct.InnerText = dr("ErrorInformation")

                    'Set the xml nodes
                    With ndProductDetails
                        .AppendChild(ndCustomerNo)
                        .AppendChild(ndProductCode)
                        .AppendChild(ndProductDescription)
                        .AppendChild(ndSeat)
                        .AppendChild(ndPriceCode)
                        .AppendChild(ndPriceBand)
                        .AppendChild(ndPrice)
                        .AppendChild(ndReservedSeat)
                        .AppendChild(ndErrorCode)
                        .AppendChild(ndUserLimit)
                        .AppendChild(ndProductLimit)
                        .AppendChild(ndPreReqProduct)
                    End With

                    'Add the product to the shopping basket
                    With ndShoppingBasket
                        .AppendChild(ndProductDetails)
                    End With

                Next dr

                'Add the shopping basket to the confirmed details
                With ndReservationDetails
                    .AppendChild(ndReservedQuantity)
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