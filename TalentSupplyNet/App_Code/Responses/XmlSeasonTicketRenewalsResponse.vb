Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal
    Public Class XmlSeasonTicketRenewalsResponse
        Inherits XmlResponse

        Private _XmlTransactiondetails As XmlNode
        Public Property XmlTransactionDetails() As XmlNode
            Get
                Return _XmlTransactiondetails
            End Get
            Set(ByVal value As XmlNode)
                _XmlTransactiondetails = value
            End Set
        End Property

        Private _xmlTicketBookerCustomerResponse As XmlNode
        Public Property XmlTicketBookerCustomerResponse() As XmlNode
            Get
                Return _xmlTicketBookerCustomerResponse
            End Get
            Set(ByVal value As XmlNode)
                _xmlTicketBookerCustomerResponse = value
            End Set
        End Property

        Private _regUsers As Boolean
        Public Property RegisterCustomers() As Boolean
            Get
                Return _regUsers
            End Get
            Set(ByVal value As Boolean)
                _regUsers = value
            End Set
        End Property

        Private _continueOnFailedReg As Boolean
        Public Property ContinueOnFailedRegistration() As Boolean
            Get
                Return _continueOnFailedReg
            End Get
            Set(ByVal value As Boolean)
                _continueOnFailedReg = value
            End Set
        End Property


        Private _successCount As Integer
        Public Property SuccessCount() As Integer
            Get
                Return _successCount
            End Get
            Set(ByVal value As Integer)
                _successCount = value
            End Set
        End Property

        Private _failCount As Integer
        Public Property FailureCount() As Integer
            Get
                Return _failCount
            End Get
            Set(ByVal value As Integer)
                _failCount = value
            End Set
        End Property

        Private _failedRegCount As Integer
        Public Property FailedRegistrationCount() As Integer
            Get
                Return _failedRegCount
            End Get
            Set(ByVal value As Integer)
                _failedRegCount = value
            End Set
        End Property

        Const majorPaymentError As String = "**"


        Protected Overrides Sub InsertBodyV1()
            Try

                Dim nSuccessCount, nFailCount, _
                     nHeader, nHeaderRoot As XmlNode
                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    If Me.XmlTransactionDetails Is Nothing Then
                        Me.XmlTransactionDetails = .CreateElement("TransactionDetails")
                    End If
                    nSuccessCount = .CreateElement("SuccessCount")
                    nFailCount = .CreateElement("FailureCount")
                End With

                nSuccessCount.InnerText = Me.SuccessCount.ToString
                nFailCount.InnerText = Me.FailureCount.ToString

                With Me.XmlTransactionDetails
                    If Me.XmlTransactionDetails.ChildNodes.Count > 0 Then
                        .InsertBefore(nSuccessCount, Me.XmlTransactionDetails.FirstChild)
                    Else
                        .AppendChild(nSuccessCount)
                    End If
                    .InsertAfter(nFailCount, nSuccessCount)
                End With

                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '
                nHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                nHeaderRoot = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                nHeader.InsertAfter(Me.XmlTransactionDetails, nHeaderRoot)

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

        Protected Overrides Sub InsertBodyV1_1()
            Try

                Dim nSuccessCount, nFailCount, _
                     nHeader, nHeaderRoot As XmlNode
                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    If Me.XmlTransactionDetails Is Nothing Then
                        Me.XmlTransactionDetails = .CreateElement("TransactionDetails")
                    End If
                    nSuccessCount = .CreateElement("SuccessCount")
                    nFailCount = .CreateElement("FailureCount")
                End With

                nSuccessCount.InnerText = Me.SuccessCount.ToString
                nFailCount.InnerText = Me.FailureCount.ToString

                With Me.XmlTransactionDetails
                    If Me.XmlTransactionDetails.ChildNodes.Count > 0 Then
                        .InsertBefore(nSuccessCount, Me.XmlTransactionDetails.FirstChild)
                    Else
                        .AppendChild(nSuccessCount)
                    End If
                    .InsertAfter(nFailCount, nSuccessCount)
                End With

                '--------------------------------------------------------------------------------------
                '   Insert the fragment into the XML document
                '
                Const c1 As String = "//"                               ' Constants are faster at run time
                Const c2 As String = "/TransactionHeader"
                '
                nHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                nHeaderRoot = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                nHeader.InsertAfter(Me.XmlTransactionDetails, nHeaderRoot)

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

        Public Sub AddNewSeasonTicketRenewalsResponse(ByVal success As Boolean, _
                                                        ByVal deAddItems As DEAddTicketingItems, _
                                                        ByVal paymentResultSet As DataSet, _
                                                        ByVal errorMessage As String)

            Dim paymentReturnCode As String = ""
            If Not paymentResultSet Is Nothing AndAlso paymentResultSet.Tables.Count > 0 Then
                Try
                    paymentReturnCode = Utilities.CheckForDBNull_String(paymentResultSet.Tables(0).Rows(0)("ReturnCode"))
                Catch ex As Exception
                End Try
            End If

            If Me.XmlTransactionDetails Is Nothing Then
                Me.XmlTransactionDetails = Me.xmlDoc.CreateElement("TransactionDetails")
            End If

            Me.XmlTicketBookerCustomerResponse = Me.xmlDoc.CreateElement("SeasonTicketRenewalResponse")

            Dim nSuccess, nBasketID, nEmailAddress, nRegCustResp, _
                nAddToBasketResp, nPayResp, nDirectDebitResp As XmlNode

            With MyBase.xmlDoc
                nSuccess = .CreateElement("Success")
                nBasketID = .CreateElement("TicketingBasketID")
                nEmailAddress = .CreateElement("EmailAddress")
                nRegCustResp = .CreateElement("RegisterCustomerResponse")
                nAddToBasketResp = .CreateElement("AddToBasketResponse")
                nPayResp = .CreateElement("PaymentResponse")
                nDirectDebitResp = .CreateElement("DirectDebitResponse")
            End With

            With Me.XmlTicketBookerCustomerResponse

                'Set Success status
                nSuccess.InnerText = success.ToString
                .AppendChild(nSuccess)

                'Set Basket ID (Session\Transaction ID)
                nBasketID.InnerText = deAddItems.SessionId
                .AppendChild(nBasketID)



                '   If success OrElse paymentReturnCode = majorPaymentError Then
                'Populate the Basket Response
                .AppendChild(nAddToBasketResp)
                Add_AddToBasketNodes(nAddToBasketResp, deAddItems)
                '   End If

                'If payments are being taken then populate the payment response
                .AppendChild(nPayResp)
                AddPaymentNodes(nPayResp, paymentResultSet, deAddItems.SessionId, deAddItems.CustomerNumber, errorMessage)

                'If Direct Debits are being taken then populate the direct debit response
                .AppendChild(nDirectDebitResp)
                AddDirectDebitNodes(nDirectDebitResp, paymentResultSet, deAddItems.CustomerNumber, errorMessage)


            End With

            'Add the Ticket Booker Response to the Response Collection
            Me.XmlTransactionDetails.AppendChild(Me.XmlTicketBookerCustomerResponse)

        End Sub


    

        Protected Sub Add_AddToBasketNodes(ByVal nAddToBasketResp As XmlNode, _
                                            ByVal deAddItems As DEAddTicketingItems)

            Dim nReturnCode, nSessionID, nProductCode, _
                nSeat, nCustomerNumber, nPriceCategory As XmlNode

            'Create the response xml nodes
            With MyBase.xmlDoc
                nReturnCode = .CreateElement("ReturnCode")
                nSessionID = .CreateElement("SessionID")
                nProductCode = .CreateElement("ProductCode")
                nSeat = .CreateElement("Seat")
                nCustomerNumber = .CreateElement("CustomerNumber")
                nPriceCategory = .CreateElement("PriceCategory")
            End With

            'Populate the nodes
            nReturnCode.InnerText = deAddItems.ErrorCode
            nSessionID.InnerText = deAddItems.SessionId
            nProductCode.InnerText = deAddItems.ProductCode
            nSeat.InnerText = deAddItems.StandCode.Trim & "/" & _
                              deAddItems.AreaCode.Trim & "/" & _
                              deAddItems.Row.Trim & "/" & _
                              deAddItems.Seat.Trim 
            If deAddItems.Suffix.Trim <> "" Then nSeat.InnerText = nSeat.InnerText & "/" & deAddItems.Suffix
            nCustomerNumber.InnerText = deAddItems.CustomerNumber
            nPriceCategory.InnerText = deAddItems.PriceBand01

            'Append the xml nodes to the parent node 
            With nAddToBasketResp
                .AppendChild(nReturnCode)
                .AppendChild(nSessionID)
                .AppendChild(nProductCode)
                .AppendChild(nSeat)
                .AppendChild(nCustomerNumber)
                .AppendChild(nPriceCategory)
            End With
        End Sub

        Protected Sub AddPaymentNodes(ByVal nPayResp As XmlNode, _
                                        ByVal paymentResultSet As DataSet, _
                                        ByVal sessionID As String, _
                                        ByVal customerNumber As String, _
                                        ByVal errorMessage As String)

            Dim nConfirmedDetails, nReturnCode, nSessionID, nAdditionalInfo As XmlNode
            Dim nCustomerNo As XmlNode
            Dim nPaymentReference, nTotalPrice As XmlNode
            Dim dtStatusResults As DataTable
            Dim hasError As Boolean = False

            Dim dr As DataRow

            'Create the response xml nodes
            With MyBase.xmlDoc
                nReturnCode = .CreateElement("ReturnCode")
                nSessionID = .CreateElement("SessionID")
                nCustomerNo = .CreateElement("CustomerNo")
                nConfirmedDetails = .CreateElement("ConfirmedDetails")
                nAdditionalInfo = .CreateElement("AdditionalInfo")
            End With

            'Read the values for the response section
            If paymentResultSet.Tables.Count > 0 Then
                dtStatusResults = paymentResultSet.Tables("StatusResults")

                dr = dtStatusResults.Rows(0)

                'Populate the nodes
                nReturnCode.InnerText = dr("ReturnCode")
                nSessionID.InnerText = sessionID
                nCustomerNo.InnerText = customerNumber

                Try
                    nAdditionalInfo.InnerText = dr("AdditionalInfo")
                Catch ex As Exception
                End Try

                If dr("ErrorOccurred") = "E" Then
                    hasError = True
                Else
                    With MyBase.xmlDoc
                        nPaymentReference = .CreateElement("PaymentReference")
                        nTotalPrice = .CreateElement("TotalPrice")
                    End With
                    '  dtPaymentResults = paymentResultSet.Tables("PaymentResults")
                    '  dr2 = dtPaymentResults.Rows(0)
                    nPaymentReference.InnerText = dr("PaymentReference")
                    nTotalPrice.InnerText = dr("TotalPrice")
                    AddPaymentDetailNodes(nConfirmedDetails, nPaymentReference, nTotalPrice, paymentResultSet)
                End If

                'Set the xml nodes
                With nPayResp
                    .AppendChild(nReturnCode)
                    .AppendChild(nSessionID)
                    .AppendChild(nCustomerNo)
                    .AppendChild(nAdditionalInfo)
                    If Not hasError Then .AppendChild(nConfirmedDetails)
                End With
            Else
                '--------------------------------------------
                ' Unexpected error during payment - report it
                '--------------------------------------------
                'Populate the nodes
                nReturnCode.InnerText = errorMessage
                nSessionID.InnerText = sessionID
                nCustomerNo.InnerText = customerNumber
                nAdditionalInfo.InnerText = String.Empty

                'Set the xml nodes
                With nPayResp
                    .AppendChild(nReturnCode)
                    .AppendChild(nSessionID)
                    .AppendChild(nCustomerNo)
                    .AppendChild(nAdditionalInfo)
                End With
            End If


        End Sub

        Protected Sub AddPaymentDetailNodes(ByVal nConfirmedDetails As XmlNode, _
                                            ByVal nPaymentReference As XmlNode, _
                                            ByVal nTotalPrice As XmlNode, _
                                            ByVal paymentResultSet As DataSet)
            If Not paymentResultSet Is Nothing AndAlso paymentResultSet.Tables.Count > 1 Then
                Dim nProductDetails As XmlNode
                Dim nShoppingBasket, nProductCode, nCustomerNo, _
                        nSeat, nPriceCode, nPriceBand, nPrice As XmlNode
                Dim nWSFee, nCreditcardFee, nCarriageFee As XmlNode
                Dim dtShoppingBasket As DataTable

                Dim dr As DataRow
                Dim bWSFee As Boolean = False
                Dim bBKFee As Boolean = False
                Dim bCRFee As Boolean = False


                ' Create the xml nodes needed for the confirmed details
                With MyBase.xmlDoc
                    nShoppingBasket = .CreateElement("ShoppingBasket")
                    nWSFee = .CreateElement("WSFee")
                    nCreditcardFee = .CreateElement("CreditcardFee")
                    nCarriageFee = .CreateElement("CarriageFee")
                End With

                'Read the values for the response section
                dtShoppingBasket = paymentResultSet.Tables(1)

                'Loop around the products
                For Each dr In dtShoppingBasket.Rows

                    If dr("FeeType") = "Y" Then

                        Select Case dr("ProductCode").ToString.Trim
                            Case Is = "BKFEE"
                                nCreditcardFee.InnerText = dr("Price")
                                bBKFee = True
                            Case Is = "CRFEE"
                                nCarriageFee.InnerText = dr("Price")
                                bCRFee = True
                            Case Is = "WSFEE"
                                nWSFee.InnerText = dr("Price")
                                bWSFee = True
                            Case Is = "ATFEE"
                                nWSFee.InnerText = dr("Price")
                                bWSFee = True
                        End Select

                    Else

                        'Create the product xml nodes
                        With MyBase.xmlDoc
                            nProductDetails = .CreateElement("ProductDetails")
                            nCustomerNo = .CreateElement("CustomerNo")
                            nProductCode = .CreateElement("ProductCode")
                            nSeat = .CreateElement("Seat")
                            nPriceCode = .CreateElement("PriceCode")
                            nPriceBand = .CreateElement("PriceBand")
                            nPrice = .CreateElement("Price")
                        End With

                        'Populate the nodes
                        nCustomerNo.InnerText = dr("CustomerNo")
                        nProductCode.InnerText = dr("ProductCode")
                        nSeat.InnerText = dr("Seat")
                        nPriceCode.InnerText = dr("PriceCode")
                        nPriceBand.InnerText = dr("PriceBand")
                        nPrice.InnerText = dr("Price")

                        'Set the xml nodes
                        With nProductDetails
                            .AppendChild(nCustomerNo)
                            .AppendChild(nProductCode)
                            .AppendChild(nSeat)
                            .AppendChild(nPriceCode)
                            .AppendChild(nPriceBand)
                            .AppendChild(nPrice)
                        End With

                        'Add the product to the shopping basket
                        With nShoppingBasket
                            .AppendChild(nProductDetails)
                        End With
                    End If

                Next dr

                'Add the shopping basket to the confirmed details
                With nConfirmedDetails
                    .AppendChild(nPaymentReference)
                    If bWSFee = True Then .AppendChild(nWSFee)
                    If bBKFee = True Then .AppendChild(nCreditcardFee)
                    If bCRFee = True Then .AppendChild(nCarriageFee)
                    .AppendChild(nTotalPrice)
                    .AppendChild(nShoppingBasket)
                End With
            End If


        End Sub

        Protected Sub AddDirectDebitNodes(ByVal nDirectDebitResp As XmlNode, _
                                        ByVal paymentResultSet As DataSet, _
                                        ByVal customerNumber As String, _
                                        ByVal errorMessage As String)

            Dim nReturnCode, nCustomerNo As XmlNode
            Dim nAccountName, nAccountNumber, nSortCode As XmlNode
            Dim nConfirmedDetails As XmlNode = Nothing
            Dim dtStatusResultsDirectDebit As DataTable
            Dim hasError As Boolean = False

            Dim dr As DataRow

            Try
                'Create the response xml nodes
                With MyBase.xmlDoc
                    nReturnCode = .CreateElement("ReturnCode")
                    nCustomerNo = .CreateElement("CustomerNo")
                    nAccountName = .CreateElement("AccountName")
                    nAccountNumber = .CreateElement("AccountNumber")
                    nSortCode = .CreateElement("SortCode")
                End With

                'Read the values for the response section
                If paymentResultSet.Tables.Count > 0 Then
                    If paymentResultSet.Tables.Count > 2 Then

                        dtStatusResultsDirectDebit = paymentResultSet.Tables("DirectDebitStatusResults")
                        dr = dtStatusResultsDirectDebit.Rows(0)

                        'Populate the nodes
                        nReturnCode.InnerText = dr("ReturnCode")
                        nCustomerNo.InnerText = customerNumber
                        nAccountName.InnerText = dr("AccountName")
                        nAccountNumber.InnerText = dr("AccountNumber")
                        nSortCode.InnerText = dr("SortCode")

                        If dr("ErrorOccurred") = "E" Then
                            hasError = True
                        Else
                            With MyBase.xmlDoc
                                nConfirmedDetails = .CreateElement("ConfirmedDetails")
                            End With
                            AddDirectDebitDetailNodes(nConfirmedDetails, paymentResultSet)
                        End If

                        'Set the xml nodes
                        With nDirectDebitResp
                            .AppendChild(nReturnCode)
                            .AppendChild(nCustomerNo)
                            .AppendChild(nAccountName)
                            .AppendChild(nAccountNumber)
                            .AppendChild(nSortCode)
                            If Not hasError Then .AppendChild(nConfirmedDetails)
                        End With
                    End If

                Else
                    '--------------------------------------------
                    ' Unexpected error - report it
                    '--------------------------------------------
                    'Populate the nodes
                    nReturnCode.InnerText = errorMessage
                    nCustomerNo.InnerText = customerNumber

                    'Set the xml nodes
                    With nDirectDebitResp
                        .AppendChild(nReturnCode)
                        .AppendChild(nCustomerNo)
                    End With
                End If

            Catch ex As Exception

            End Try

        End Sub

        Protected Sub AddDirectDebitDetailNodes(ByVal nConfirmedDetails As XmlNode, ByVal paymentResultSet As DataSet)

            If Not paymentResultSet Is Nothing AndAlso paymentResultSet.Tables.Count > 2 Then

                Dim nPaymentReference, nTotalAmount, nScheduledEntries As XmlNode
                Dim nDDIReference, nPaymentSchedule As XmlNode
                Dim nPayment, nPaymentDate, nPaymentAmount As XmlNode
                Dim dtStatusResults As DataTable
                Dim dtStatusResultsDirectDebit As DataTable
                Dim dtDirectDebitSummary As DataTable

                Dim dr1, dr3, dr4 As DataRow

                dtStatusResults = paymentResultSet.Tables("StatusResults")
                dtStatusResultsDirectDebit = paymentResultSet.Tables("DirectDebitStatusResults")
                dtDirectDebitSummary = paymentResultSet.Tables("DirectDebitSummary")

                dr1 = dtStatusResults.Rows(0)
                dr3 = dtStatusResultsDirectDebit.Rows(0)

                ' Create the xml nodes needed for the confirmed details
                With MyBase.xmlDoc
                    nDDIReference = .CreateElement("DDIReference")
                    nPaymentReference = .CreateElement("PaymentReference")
                    nTotalAmount = .CreateElement("TotalAmount")
                    nScheduledEntries = .CreateElement("ScheduledEntries")
                    nPaymentSchedule = .CreateElement("PaymentSchedule")
                End With


                nDDIReference.InnerText = dr3("DirectDebitDDIRef")
                nPaymentReference.InnerText = dr1("PaymentReference")
                nTotalAmount.InnerText = dr3("TotalAmount")
                nScheduledEntries.InnerText = dr3("ScheduledEntries")

                'Loop around the products
                For Each dr4 In dtDirectDebitSummary.Rows

                    'Create the product xml nodes
                    With MyBase.xmlDoc
                        nPayment = .CreateElement("Payment")
                        nPaymentDate = .CreateElement("PaymentDate")
                        nPaymentAmount = .CreateElement("PaymentAmount")
                    End With

                    'Populate the nodes
                    nPaymentDate.InnerText = dr4("PaymentDate")
                    nPaymentAmount.InnerText = dr4("PaymentAmount")

                    'Set the xml nodes
                    With nPayment
                        .AppendChild(nPaymentDate)
                        .AppendChild(nPaymentAmount)
                    End With
                    With nPaymentSchedule
                        .AppendChild(nPayment)
                    End With

                Next dr4

                'Add the shopping basket to the confirmed details
                With nConfirmedDetails
                    .AppendChild(nDDIReference)
                    .AppendChild(nPaymentReference)
                    .AppendChild(nTotalAmount)
                    .AppendChild(nScheduledEntries)
                    .AppendChild(nPaymentSchedule)
                End With
            End If


        End Sub

    End Class
End Namespace

