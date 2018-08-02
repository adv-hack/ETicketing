Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal

    Public Class XmlAddTicketingItemsReturnBasketResponse
        Inherits XmlResponse

        Private errorOccurred As Boolean = False
        Private ndHeaderRoot, ndHeaderRootHeader, ndHeader As XmlNode
        Private ndAddTicketingItems, ndResponse, ndReturnCode, ndSessionID, ndProductCode, ndStandCode, ndAreaCode, ndCustomerNumber, ndAlternativeSeat As XmlNode
        Private _deAddItems As DEAddTicketingItems
        Private _alternativeSeatSelected As Boolean
        Private ndRetrieveTicketingItems, ndReservationDetails As XmlNode
        Private ndShoppingBasket, ndWSFee, ndCreditcardFee, ndCarriageFee, ndTotalPrice, ndReservedQuantity As XmlNode
        Private ndCustomerNo, ndProductDescription, ndSeat, ndPriceCode, ndPriceBand, ndPrice, ndReservedSeat, ndErrorCode, ndUserLimit, ndProductLimit, ndPreReqProduct, ndProductDetails As XmlNode
        Private dtReservationDetails As DataTable
        Private dtGlobalParameters As DataTable
        Private dtStatusDetails As DataTable

        Public Property DeAddItems() As DEAddTicketingItems
            Get
                Return _deAddItems
            End Get
            Set(ByVal value As DEAddTicketingItems)
                _deAddItems = value
            End Set
        End Property
        Public Property AlternativeSeatSelected() As Boolean
            Get
                Return _alternativeSeatSelected
            End Get
            Set(value As Boolean)
                _alternativeSeatSelected = value
            End Set
        End Property

        Protected Overrides Sub InsertBodyV1()
            Try
                With MyBase.xmlDoc
                    ndAddTicketingItems = .CreateElement("AddTicketingItemsReturnBasket")
                    ndResponse = .CreateElement("Response")
                    ndReservationDetails = .CreateElement("TicketingItemsDetails")
                End With

                CreateResponseSection()
                CreateReservationDetails()

                With ndAddTicketingItems
                    .AppendChild(ndResponse)
                    If Not errorOccurred Then
                        .AppendChild(ndReservationDetails)
                    End If
                End With

                Const c1 As String = "//"
                Const c2 As String = "/TransactionHeader"
                ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                ndHeaderRootHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                ndHeader.InsertAfter(ndAddTicketingItems, ndHeaderRootHeader)
            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSATI-01"
                    .HasError = True
                End With
            End Try
        End Sub

        Protected Sub CreateResponseSection()
            'Create the response xml nodes
            With MyBase.xmlDoc
                ndReturnCode = .CreateElement("ReturnCode")
                ndSessionID = .CreateElement("SessionID")
                ndProductCode = .CreateElement("ProductCode")
                ndStandCode = .CreateElement("StandCode")
                ndAreaCode = .CreateElement("AreaCode")
                ndCustomerNumber = .CreateElement("CustomerNumber")
                ndAlternativeSeat = .CreateElement("AlternativeSeat")
            End With

            'Populate the nodes
            ndReturnCode.InnerText = _deAddItems.ErrorCode
            ndSessionID.InnerText = _deAddItems.SessionId
            ndProductCode.InnerText = _deAddItems.ProductCode
            ndStandCode.InnerText = _deAddItems.StandCode
            ndAreaCode.InnerText = _deAddItems.AreaCode
            ndCustomerNumber.InnerText = _deAddItems.CustomerNumber
            ndAlternativeSeat.InnerText = _alternativeSeatSelected

            'Append the xml nodes to the parent node 
            With ndResponse
                .AppendChild(ndReturnCode)
                .AppendChild(ndSessionID)
                .AppendChild(ndProductCode)
                .AppendChild(ndStandCode)
                .AppendChild(ndAreaCode)
                .AppendChild(ndCustomerNumber)
                .AppendChild(ndAlternativeSeat)
            End With
        End Sub

        Protected Sub CreateReservationDetails()
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
                    ndCustomerNo.InnerText = Talent.Common.Utilities.CheckForDBNull_String(dr("CustomerNo"))
                    ndProductCode.InnerText = Talent.Common.Utilities.CheckForDBNull_String(dr("ProductCode"))
                    ndProductDescription.InnerText = Talent.Common.Utilities.CheckForDBNull_String(dr("ProductDescription"))
                    ndSeat.InnerText = Talent.Common.Utilities.CheckForDBNull_String(dr("Seat"))
                    ndPriceCode.InnerText = Talent.Common.Utilities.CheckForDBNull_String(dr("PriceCode"))
                    ndPriceBand.InnerText = Talent.Common.Utilities.CheckForDBNull_String(dr("PriceBand"))
                    ndPrice.InnerText = Talent.Common.Utilities.CheckForDBNull_String(dr("Price"))
                    ndReservedSeat.InnerText = Talent.Common.Utilities.CheckForDBNull_String(dr("ReservedSeat"))
                    ndErrorCode.InnerText = Talent.Common.Utilities.CheckForDBNull_String(dr("ErrorCode"))
                    ndUserLimit.InnerText = Talent.Common.Utilities.CheckForDBNull_String(dr("UserLimit"))
                    ndProductLimit.InnerText = Talent.Common.Utilities.CheckForDBNull_String(dr("ProductLimit"))
                    ndPreReqProduct.InnerText = Talent.Common.Utilities.CheckForDBNull_String(dr("ErrorInformation"))

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