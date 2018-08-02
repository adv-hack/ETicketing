Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Xml
Imports Talent.Common

Namespace Talent.TradingPortal
    Public Class XMLTicketBookerResponse
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

        Private _takePayment As Boolean
        Public Property TakePayments() As Boolean
            Get
                Return _takePayment
            End Get
            Set(ByVal value As Boolean)
                _takePayment = value
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

                Dim nRegCustomers, nContinue, nTakePayments, nSuccessCount, nFailCount, _
                    nFailRegCount, nHeader, nHeaderRoot As XmlNode
                ' Create the three xml nodes needed at the root level
                With MyBase.xmlDoc
                    If Me.XmlTransactionDetails Is Nothing Then
                        Me.XmlTransactionDetails = .CreateElement("TransactionDetails")
                    End If
                    nRegCustomers = .CreateElement("RegisterCustomers")
                    nContinue = .CreateElement("ContinueSaleOnFailedRegistration")
                    nTakePayments = .CreateElement("TakePayments")
                    nSuccessCount = .CreateElement("SuccessCount")
                    nFailCount = .CreateElement("FailureCount")
                    nFailRegCount = .CreateElement("FailedRegistrationsCount")
                End With

                nRegCustomers.InnerText = Me.RegisterCustomers.ToString
                nContinue.InnerText = Me.ContinueOnFailedRegistration.ToString
                nTakePayments.InnerText = Me.TakePayments.ToString
                nSuccessCount.InnerText = Me.SuccessCount.ToString
                nFailCount.InnerText = Me.FailureCount.ToString
                nFailRegCount.InnerText = Me.FailedRegistrationCount.ToString

                With Me.XmlTransactionDetails
                    If Me.XmlTransactionDetails.ChildNodes.Count > 0 Then
                        .InsertBefore(nRegCustomers, Me.XmlTransactionDetails.FirstChild)
                    Else
                        .AppendChild(nRegCustomers)
                    End If
                    .InsertAfter(nContinue, nRegCustomers)
                    .InsertAfter(nTakePayments, nContinue)
                    .InsertAfter(nSuccessCount, nTakePayments)
                    .InsertAfter(nFailCount, nSuccessCount)
                    If Me.RegisterCustomers Then .InsertAfter(nFailRegCount, nFailCount)
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

        Public Sub AddNewTicketBookerCustomerResponse(ByVal success As Boolean, _
                                                        ByVal deAddItems As DEAddTicketingItems, _
                                                        ByVal deCust As DECustomer, _
                                                        ByVal registerCustomerResultSet As DataSet, _
                                                        ByVal paymentResultSet As DataSet, _
                                                        ByVal registrationSuccess As Boolean, _
                                                        ByVal errorMessage As String)

            Dim paymentReturnCode As String = ""
            Try
                paymentReturnCode = Utilities.CheckForDBNull_String(paymentResultSet.Tables(0).Rows(0)("ReturnCode"))
            Catch ex As Exception
            End Try

            If Me.XmlTransactionDetails Is Nothing Then
                Me.XmlTransactionDetails = Me.xmlDoc.CreateElement("TransactionDetails")
            End If

            Me.XmlTicketBookerCustomerResponse = Me.xmlDoc.CreateElement("TicketBookerCustomerResponse")

            Dim nSuccess, nBasketID, nEmailAddress, nRegCustResp, _
                nAddToBasketResp, nPayResp As XmlNode

            With MyBase.xmlDoc
                nSuccess = .CreateElement("Success")
                nBasketID = .CreateElement("TicketingBasketID")
                nEmailAddress = .CreateElement("EmailAddress")
                nRegCustResp = .CreateElement("RegisterCustomerResponse")
                nAddToBasketResp = .CreateElement("AddToBasketResponse")
                nPayResp = .CreateElement("PaymentResponse")
            End With

            With Me.XmlTicketBookerCustomerResponse

                'Set Success status
                nSuccess.InnerText = success.ToString
                .AppendChild(nSuccess)

                'Set Basket ID (Session\Transaction ID)
                nBasketID.InnerText = deAddItems.SessionId
                .AppendChild(nBasketID)

                'Set email address so can match up to original file
                If Not deCust Is Nothing AndAlso deCust.EmailAddress Is Nothing Then
                    nEmailAddress.InnerText = deCust.EmailAddress
                    .AppendChild(nEmailAddress)
                End If

                'If customers are to be registered then populate the customer response
                If Me.RegisterCustomers Then
                    .AppendChild(nRegCustResp)
                    AddRegisterCustomerNodes(nRegCustResp, registerCustomerResultSet, registrationSuccess, errorMessage)
                End If

                '   If success OrElse paymentReturnCode = majorPaymentError Then
                'Populate the Basket Response
                .AppendChild(nAddToBasketResp)
                Add_AddToBasketNodes(nAddToBasketResp, deAddItems)
                '   End If

                'If payments are being taken then populate the payment response
                If Me.TakePayments Then
                    .AppendChild(nPayResp)
                    AddPaymentNodes(nPayResp, paymentResultSet, deAddItems.SessionId, deAddItems.CustomerNumber, errorMessage)
                End If



            End With

            'Add the Ticket Booker Response to the Response Collection
            Me.XmlTransactionDetails.AppendChild(Me.XmlTicketBookerCustomerResponse)

        End Sub

        Protected Sub AddRegisterCustomerNodes(ByVal nRegCustResp As XmlNode, _
                                                ByVal registerCustomerResultSet As DataSet, _
                                                ByVal registrationSuccess As Boolean, _
                                                ByVal errorMessage As String)

            'Register Customer - Response Nodes
            '-------------------------------------
            Dim nCustomerDetails, nReturnCode, nSuccess As XmlNode
            Dim dtStatusDetails As DataTable
            Dim hasError As Boolean = False

            With MyBase.xmlDoc
                nSuccess = .CreateElement("RegistrationSuccess")
                nReturnCode = .CreateElement("ReturnCode")
                nCustomerDetails = .CreateElement("CustomerDetails")
            End With

            If Not registerCustomerResultSet Is Nothing AndAlso registerCustomerResultSet.Tables.Count > 0 Then
                dtStatusDetails = registerCustomerResultSet.Tables(0)
                nReturnCode.InnerText = dtStatusDetails.Rows(0)("ReturnCode")
                If Not registrationSuccess AndAlso nReturnCode.InnerText = String.Empty Then
                    nReturnCode.InnerText = errorMessage
                End If
                nSuccess.InnerText = registrationSuccess.ToString
                If dtStatusDetails.Rows(0)("ErrorOccurred") = "E" Then
                    hasError = True
                End If
            Else
                nSuccess.InnerText = False
            End If

            With nRegCustResp
                .AppendChild(nReturnCode)
                .AppendChild(nSuccess)
                If hasError = False AndAlso registrationSuccess = True Then
                    AddCustomerDetailsNodes(nCustomerDetails, hasError, registerCustomerResultSet)
                    .AppendChild(nCustomerDetails)
                End If
            End With


        End Sub
        Protected Sub AddCustomerDetailsNodes(ByVal nCustomerDetails As XmlNode, _
                                                ByVal hasError As Boolean, _
                                                ByVal registerCustomerResultSet As DataSet)

            Dim dtCustomerDetails As DataTable

            Dim nCustomerNo, nContactTitle, nContactInitials, nContactForename, nContactSurname, _
                nSalutation, nCompanyName, nPositionInCompany, nAddressLine1, _
                nAddressLine2, nAddressLine3, nAddressLine4, nAddressLine5, _
                nPostCode, nGender, nHomeTelephoneNumber, nWorkTelephoneNumber, _
                nMobileNumber, nEmailAddress, nDateBirth, nContactViaMail, _
                nSubscription1, nSubscription2, nSubscription3, nContactViaMail1, _
                nContactViaMail2, nContactViaMail3, nContactViaMail4, nContactViaMail5, _
                nExternalId1, nExternalId2 As XmlNode


            If Not hasError Then

                'Create the customer xml nodes
                With MyBase.xmlDoc
                    nCustomerNo = .CreateElement("CustomerNo")
                    nCustomerDetails = .CreateElement("CustomerDetails")
                    nContactTitle = .CreateElement("ContactTitle")
                    nContactInitials = .CreateElement("ContactInitials")
                    nContactForename = .CreateElement("ContactForename")
                    nContactSurname = .CreateElement("ContactSurname")
                    nSalutation = .CreateElement("Salutation")
                    nCompanyName = .CreateElement("CompanyName")
                    nPositionInCompany = .CreateElement("PositionInCompany")
                    nAddressLine1 = .CreateElement("AddressLine1")
                    nAddressLine2 = .CreateElement("AddressLine2")
                    nAddressLine3 = .CreateElement("AddressLine3")
                    nAddressLine4 = .CreateElement("AddressLine4")
                    nAddressLine5 = .CreateElement("AddressLine5")
                    nPostCode = .CreateElement("PostCode")
                    nGender = .CreateElement("Gender")
                    nHomeTelephoneNumber = .CreateElement("HomeTelephoneNumber")
                    nWorkTelephoneNumber = .CreateElement("WorkTelephoneNumber")
                    nMobileNumber = .CreateElement("MobileNumber")
                    nEmailAddress = .CreateElement("EmailAddress")
                    nDateBirth = .CreateElement("DateOfBirth")
                    nContactViaMail = .CreateElement("ContactViaMail")
                    nSubscription1 = .CreateElement("Subscription1")
                    nSubscription2 = .CreateElement("Subscription2")
                    nSubscription3 = .CreateElement("Subscription3")
                    nContactViaMail1 = .CreateElement("ContactViaMail1")
                    nContactViaMail2 = .CreateElement("ContactViaMail2")
                    nContactViaMail3 = .CreateElement("ContactViaMail3")
                    nContactViaMail4 = .CreateElement("ContactViaMail4")
                    nContactViaMail5 = .CreateElement("ContactViaMail5")
                    nExternalId1 = .CreateElement("ExternalId1")
                    nExternalId2 = .CreateElement("ExternalId2")
                End With


                If Not registerCustomerResultSet Is Nothing _
                                    AndAlso registerCustomerResultSet.Tables.Count > 1 Then
                    dtCustomerDetails = registerCustomerResultSet.Tables(1)
                    If dtCustomerDetails.Rows.Count > 0 Then
                        Dim dr As DataRow = dtCustomerDetails.Rows(0)

                        'Populate the nodes
                        nCustomerNo.InnerText = dr("CustomerNo")
                        nContactTitle.InnerText = dr("ContactTitle")
                        nContactInitials.InnerText = dr("ContactInitials")
                        nContactForename.InnerText = dr("ContactForename")
                        nContactSurname.InnerText = dr("ContactSurname")
                        nSalutation.InnerText = dr("Salutation")
                        nCompanyName.InnerText = dr("CompanyName")
                        nPositionInCompany.InnerText = dr("PositionInCompany")
                        nAddressLine1.InnerText = dr("AddressLine1")
                        nAddressLine2.InnerText = dr("AddressLine2")
                        nAddressLine3.InnerText = dr("AddressLine3")
                        nAddressLine4.InnerText = dr("AddressLine4")
                        nAddressLine5.InnerText = dr("AddressLine5")
                        nPostCode.InnerText = dr("Postcode")
                        nGender.InnerText = dr("Gender")
                        nHomeTelephoneNumber.InnerText = dr("HomeTelephoneNumber")
                        nWorkTelephoneNumber.InnerText = dr("WorkTelephoneNumber")
                        nMobileNumber.InnerText = dr("MobileNumber")
                        nEmailAddress.InnerText = dr("EmailAddress")
                        nDateBirth.InnerText = dr("DateBirth")
                        nContactViaMail.InnerText = dr("ContactViaMail")
                        nSubscription1.InnerText = dr("Subscription1")
                        nSubscription2.InnerText = dr("Subscription2")
                        nSubscription3.InnerText = dr("Subscription3")
                        nContactViaMail1.InnerText = dr("ContactViaMail1")
                        nContactViaMail2.InnerText = dr("ContactViaMail2")
                        nContactViaMail3.InnerText = dr("ContactViaMail3")
                        nContactViaMail4.InnerText = dr("ContactViaMail4")
                        nContactViaMail5.InnerText = dr("ContactViaMail5")
                        nExternalId1.InnerText = dr("ExternalId1")
                        nExternalId2.InnerText = dr("ExternalId2")

                        'Set the xml nodes
                        With nCustomerDetails
                            .AppendChild(nCustomerNo)
                            .AppendChild(nContactTitle)
                            .AppendChild(nContactInitials)
                            .AppendChild(nContactForename)
                            .AppendChild(nContactSurname)
                            .AppendChild(nSalutation)
                            .AppendChild(nCompanyName)
                            .AppendChild(nPositionInCompany)
                            .AppendChild(nAddressLine1)
                            .AppendChild(nAddressLine2)
                            .AppendChild(nAddressLine3)
                            .AppendChild(nAddressLine4)
                            .AppendChild(nAddressLine5)
                            .AppendChild(nPostCode)
                            .AppendChild(nGender)
                            .AppendChild(nHomeTelephoneNumber)
                            .AppendChild(nWorkTelephoneNumber)
                            .AppendChild(nMobileNumber)
                            .AppendChild(nEmailAddress)
                            .AppendChild(nDateBirth)
                            .AppendChild(nContactViaMail)
                            .AppendChild(nSubscription1)
                            .AppendChild(nSubscription2)
                            .AppendChild(nSubscription3)
                            .AppendChild(nContactViaMail1)
                            .AppendChild(nContactViaMail2)
                            .AppendChild(nContactViaMail3)
                            .AppendChild(nContactViaMail4)
                            .AppendChild(nContactViaMail5)
                            .AppendChild(nExternalId1)
                            .AppendChild(nExternalId2)
                        End With
                    End If
                End If
            End If

        End Sub

        Protected Sub Add_AddToBasketNodes(ByVal nAddToBasketResp As XmlNode, _
                                            ByVal deAddItems As DEAddTicketingItems)

            Dim nReturnCode, nSessionID, nProductCode, _
                nStandCode, nAreaCode, nCustomerNumber, nPriceCategory As XmlNode

            'Create the response xml nodes
            With MyBase.xmlDoc
                nReturnCode = .CreateElement("ReturnCode")
                nSessionID = .CreateElement("SessionID")
                nProductCode = .CreateElement("ProductCode")
                nStandCode = .CreateElement("StandCode")
                nAreaCode = .CreateElement("AreaCode")
                nCustomerNumber = .CreateElement("CustomerNumber")
                nPriceCategory = .CreateElement("PriceCategory")
            End With

            'Populate the nodes
            nReturnCode.InnerText = deAddItems.ErrorCode
            nSessionID.InnerText = deAddItems.SessionId
            nProductCode.InnerText = deAddItems.ProductCode
            nStandCode.InnerText = deAddItems.StandCode
            nAreaCode.InnerText = deAddItems.AreaCode
            nCustomerNumber.InnerText = deAddItems.CustomerNumber
            nPriceCategory.InnerText = deAddItems.PriceBand01

            'Append the xml nodes to the parent node 
            With nAddToBasketResp
                .AppendChild(nReturnCode)
                .AppendChild(nSessionID)
                .AppendChild(nProductCode)
                .AppendChild(nStandCode)
                .AppendChild(nAreaCode)
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
                    Try
                        nTotalPrice.InnerText = paymentResultSet.Tables(1).Rows(0)("TotalPrice")
                    Catch ex As Exception
                        nTotalPrice.InnerText = String.Empty
                    End Try

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

                Try

                
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

                Catch ex As Exception

                End Try

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

    End Class
End Namespace

