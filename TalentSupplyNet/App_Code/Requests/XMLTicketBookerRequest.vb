Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to book tickets
'
'       Date                        Feb 2008
'
'       Author                      Jonathan Williamson  
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQTBR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal
    Public Class XMLTicketBookerRequest
        Inherits XmlRequest


        Private _deTicketItemDetails As DETicketingItemDetails
        Public Property DEGetTransID() As DETicketingItemDetails
            Get
                Return _deTicketItemDetails
            End Get
            Set(ByVal value As DETicketingItemDetails)
                _deTicketItemDetails = value
            End Set
        End Property

        Private _deAddItems As New DEAddTicketingItems
        Public Property DeAddItems() As DEAddTicketingItems
            Get
                Return _deAddItems
            End Get
            Set(ByVal value As DEAddTicketingItems)
                _deAddItems = value
            End Set
        End Property

        Private _deCust As DECustomer
        Public Property DeCust() As DECustomer
            Get
                Return _deCust
            End Get
            Set(ByVal value As DECustomer)
                _deCust = value
            End Set
        End Property

        Private _dePay As DEPayments
        Public Property DePay() As DEPayments
            Get
                Return _dePay
            End Get
            Set(ByVal value As DEPayments)
                _dePay = value
            End Set
        End Property

        Private _dePayExternal As DEPaymentExternalDetails
        Public Property DePaymentExternal() As DEPaymentExternalDetails
            Get
                Return _dePayExternal
            End Get
            Set(ByVal value As DEPaymentExternalDetails)
                _dePayExternal = value
            End Set
        End Property


        Private _successTally As Integer
        Public Property SuccessTally() As Integer
            Get
                Return _successTally
            End Get
            Set(ByVal value As Integer)
                _successTally = value
            End Set
        End Property

        Private _errorTally As Integer
        Public Property FailureTally() As Integer
            Get
                Return _errorTally
            End Get
            Set(ByVal value As Integer)
                _errorTally = value
            End Set
        End Property

        Private _failedRegCount As Integer
        Public Property FailedRegistrationTally() As Integer
            Get
                Return _failedRegCount
            End Get
            Set(ByVal value As Integer)
                _failedRegCount = value
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
        Private _feeDepartment As String
        Public Property FeeDepartment() As String
            Get
                Return _feeDepartment
            End Get
            Set(ByVal value As String)
                _feeDepartment = value
            End Set
        End Property

        Private _paymentResultSet As Data.DataSet
        Public Property PaymentResultSet() As Data.DataSet
            Get
                Return _paymentResultSet
            End Get
            Set(ByVal value As Data.DataSet)
                _paymentResultSet = value
            End Set
        End Property
        Private _regCustResultSet As Data.DataSet
        Public Property RegisterCustomerResultSet() As Data.DataSet
            Get
                Return _regCustResultSet
            End Get
            Set(ByVal value As Data.DataSet)
                _regCustResultSet = value
            End Set
        End Property
        Private _basketResultSet As Data.DataSet
        Public Property BasketResultSet() As Data.DataSet
            Get
                Return _basketResultSet
            End Get
            Set(ByVal value As Data.DataSet)
                _basketResultSet = value
            End Set
        End Property

        Property ticketsHaveBeenReserved As Boolean = False

        Dim xmlLoopCount As Integer = 0



        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Me.SuccessTally = 0
            Me.FailureTally = 0
            Me.FailedRegistrationTally = 0

            Dim xmlAction As XMLTicketBookerResponse = CType(xmlResp, XMLTicketBookerResponse)
            Dim err As New ErrorObj

            Try
                '   bNode = Base Node
                For Each bNode As XmlNode In xmlDoc.SelectSingleNode("//TicketBookerRequest").ChildNodes

                    Select Case bNode.Name
                        Case Is = "Header"

                            For Each hNode As XmlNode In bNode.ChildNodes
                                Select Case hNode.Name
                                    Case Is = "RegisterCustomers"
                                        Me.RegisterCustomers = CBool(hNode.InnerText)
                                    Case Is = "TakePayments"
                                        Me.TakePayments = CBool(hNode.InnerText)
                                    Case Is = "ContinueSaleOnFailedRegistration"
                                        Me.ContinueOnFailedRegistration = CBool(hNode.InnerText)
                                    Case Is = "FeeDepartment"
                                        Me.FeeDepartment = hNode.InnerText

                                End Select
                            Next

                            'Set the corresponding properties on the Response
                            xmlAction.RegisterCustomers = Me.RegisterCustomers
                            xmlAction.TakePayments = Me.TakePayments
                            xmlAction.ContinueOnFailedRegistration = Me.ContinueOnFailedRegistration


                        Case Is = "Details"

                            '   cNode = Customer Request Node
                            For Each cNode As XmlNode In bNode.ChildNodes
                                xmlLoopCount += 1
                                ' If one round trip fails we don't want all the
                                ' rest fail so wrap in a Try-Catch
                                Try
                                    err = New ErrorObj
                                    Me.DEGetTransID = New DETicketingItemDetails
                                    Me.DeAddItems = New DEAddTicketingItems
                                    Me.RegisterCustomerResultSet = New Data.DataSet
                                    Me.PaymentResultSet = New Data.DataSet

                                    Select Case MyBase.DocumentVersion
                                        Case Is = "1.0"
                                            err = Me.LoadXmlV1(cNode)
                                    End Select



                                    Dim registrationSuccess As Boolean = True

                                    'If customers are to be registered then do this first
                                    If Me.RegisterCustomers Then err = RegisterCustomer(err)

                                    'If registration failed, but we are to continue with
                                    'the sale regardless then reset the error object here
                                    If err.HasError Then
                                        If Me.ContinueOnFailedRegistration Then
                                            err = New ErrorObj
                                            registrationSuccess = False
                                            Me.FailedRegistrationTally += 1
                                        End If
                                    Else
                                        'If there was no error attempting to register 
                                        'the user then check the results table
                                        If Me.RegisterCustomerResultSet.Tables.Count > 0 Then
                                            Dim dtStatusDetails As Data.DataTable = Me.RegisterCustomerResultSet.Tables(0)
                                            If dtStatusDetails.Rows.Count > 0 Then
                                                Dim dr As Data.DataRow = dtStatusDetails.Rows(0)
                                                If Utilities.CheckForDBNull_String(dr("ErrorOccurred")) = "E" Then
                                                    Me.FailedRegistrationTally += 1
                                                    registrationSuccess = False
                                                    If Not Me.ContinueOnFailedRegistration Then
                                                        err.HasError = True
                                                        err.ErrorMessage = "TicketBooker Error Registering User - " & Utilities.CheckForDBNull_String(dr("ReturnCode")) & " - " & DeCust.EmailAddress
                                                        err.ErrorNumber = "TTPRQTBR-03"
                                                        err.ErrorStatus = "Registration Error"
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If

                                    'If there is no error then get the Transaction ID
                                    If Not err.HasError Then err = GetTransactionID(err)

                                    'If there is no error then add the tickets to the basket
                                    If Not err.HasError Then err = AddTicketsToBasket(err)

                                    'If the add was successful then set the flag to indicate this
                                    If Not err.HasError Then ticketsHaveBeenReserved = True

                                    'If there has been no error and payment is to be takent then do this now
                                    If Not err.HasError AndAlso Me.TakePayments Then
                                        err = TakePayment(err)
                                        If Not err.HasError AndAlso Me.PaymentResultSet.Tables.Count > 0 Then
                                            Dim dtStatusDetails As Data.DataTable = Me.PaymentResultSet.Tables(0)
                                            Dim dr As Data.DataRow = dtStatusDetails.Rows(0)

                                            If Utilities.CheckForDBNull_String(dr("ErrorOccurred")) = "E" Then
                                                err.HasError = True
                                                err.ErrorMessage = "TicketBooker Error Taking Payment - " & Utilities.CheckForDBNull_String(dr("ReturnCode"))
                                                err.ErrorNumber = "TTPRQTBR-04"
                                                err.ErrorStatus = "Payment Error"
                                            End If
                                        End If
                                    End If

                                    'If tickets have been reserved but payment failed then release the tickets
                                    If ticketsHaveBeenReserved And err.HasError Then
                                        RemoveTicketsFromBasket()
                                    End If

                                    'Update the success/failure counts
                                    If err.HasError Then
                                        Me.FailureTally += 1
                                    Else
                                        Me.SuccessTally += 1
                                    End If


                                    If Me.RegisterCustomerResultSet Is Nothing Then
                                        Me.RegisterCustomerResultSet = New Data.DataSet
                                    End If
                                    If Me.PaymentResultSet Is Nothing Then
                                        Me.PaymentResultSet = New Data.DataSet
                                    End If

                                    xmlAction.SuccessCount = Me.SuccessTally
                                    xmlAction.FailureCount = Me.FailureTally
                                    xmlAction.FailedRegistrationCount = Me.FailedRegistrationTally

                                    'Add the result to the response nodes
                                    xmlAction.AddNewTicketBookerCustomerResponse(Not err.HasError, _
                                                                                    Me.DeAddItems, _
                                                                                    Me.DeCust, _
                                                                                    Me.RegisterCustomerResultSet, _
                                                                                    Me.PaymentResultSet, _
                                                                                    registrationSuccess, _
                                                                                    err.ErrorNumber)

                                Catch ex As Exception
                                End Try
                            Next

                    End Select


                Next
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = "Error reading XML Document"
                    .ErrorNumber = "TTPRQTBR-01"
                    .HasError = True
                End With
            End Try


            xmlAction.SenderID = Settings.SenderID
            xmlAction.CreateResponse()
            Return CType(xmlAction, XmlResponse)


        End Function

        Protected Function GetTransactionID(ByVal err As ErrorObj) As ErrorObj

            'If we have registered a new user, this will contain the new user number,
            'otherwise it will be set to 000000000000
            Me.DEGetTransID.CustomerNo = Me.DeAddItems.CustomerNumber

            Dim BASKET As New TalentBasket
            If Not err.HasError Then
                'Retrieve the items in the shopping basket
                With BASKET
                    .Settings = Settings
                    .De = Me.DEGetTransID
                    .ResultDataSet = Nothing
                    err = .GenerateTicketingBasketID
                End With
            End If

            'If there is no error add the Session ID to the New DEAddItems object
            If Not err.HasError Then
                Try
                    Me.DeAddItems.SessionId = BASKET.De.SessionId
                    Me.DePay.SessionId = BASKET.De.SessionId
                Catch ex As Exception
                End Try
            End If


            Return err
        End Function

        Protected Function RegisterCustomer(ByVal err As ErrorObj) As ErrorObj
            Me.RegisterCustomerResultSet = New Data.DataSet

            Dim CUSTOMER As New TalentCustomer
            Dim deCustV11 As New DECustomerV11
            deCustV11.DECustomersV1.Add(DeCust)
            If Not err.HasError Then
                With CUSTOMER
                    .DeV11 = deCustV11
                    .Settings = Settings
                    err = .SetCustomer
                End With
            End If

            Me.RegisterCustomerResultSet = CUSTOMER.ResultDataSet

            If Not err.HasError Then
                If Me.RegisterCustomerResultSet.Tables.Count > 1 Then
                    If Me.RegisterCustomerResultSet.Tables(1).Rows.Count > 0 Then
                        Dim dr As Data.DataRow = Me.RegisterCustomerResultSet.Tables(1).Rows(0)
                        Me.DeAddItems.CustomerNumber = Utilities.CheckForDBNull_String(dr("CustomerNo"))
                    End If
                End If
            End If

            Return err
        End Function



        Protected Function AddTicketsToBasket(ByVal err As ErrorObj) As ErrorObj

            If Not err.HasError Then
                Dim BASKET As New TalentBasket

                With BASKET
                    .DeAddTicketingItems = DeAddItems
                    .DeAddTicketingItems.Source = "S"
                    '.DeAddTicketingItems.PriceCode = "P1"
                    .Settings = Settings
                    err = .AddTicketingItemsReturnBasket
                    BasketResultSet = BASKET.ResultDataSet
                    If Not err.HasError Then
                        ' check for error code in basket status results
                        If BasketResultSet IsNot Nothing Then
                            If BasketResultSet.Tables.Count > 0 Then

                                If BasketResultSet.Tables("BasketStatus") IsNot Nothing Then
                                    For Each row As Data.DataRow In BasketResultSet.Tables("BasketStatus").Rows
                                        If Not row("ReturnCode").Equals(String.Empty) Then
                                            err.HasError = True
                                            err.ErrorNumber = "AAC" & "-" & row("ReturnCode")
                                            Exit For
                                        End If
                                    Next

                                    ' check for error at detail level
                                    If BasketResultSet.Tables("BasketDetail") IsNot Nothing Then
                                        For Each row As Data.DataRow In BasketResultSet.Tables("BasketDetail").Rows
                                            If Not row("ErrorCode").ToString.Trim.Equals(String.Empty) Then
                                                err.HasError = True
                                                err.ErrorNumber = "AAE" & "-" & row("ErrorCode")
                                                ticketsHaveBeenReserved = True
                                                Exit For
                                            End If
                                        Next
                                    End If


                                Else
                                    err.HasError = True
                                    err.ErrorNumber = "AAD"
                                End If
                            Else
                                err.HasError = True
                                err.ErrorNumber = "AAC"
                            End If
                        End If



                    End If

                End With

            End If

            Return err
        End Function


        Protected Sub RemoveTicketsFromBasket()
            Try
                Dim BASKET As New TalentBasket
                Dim DeRemove As New DETicketingItemDetails

                With DeRemove
                    .Src = "S"
                    .SessionId = Me.DeAddItems.SessionId
                End With

                ' Remove the Ticketing Items from the ticketing shopping basket
                With BASKET
                    .Settings = Settings
                    .De = DeRemove
                    .RemoveTicketingItems()
                End With
            Catch ex As Exception

            End Try

        End Sub

        Protected Function TakePayment(ByVal err As ErrorObj) As ErrorObj
            Me.PaymentResultSet = New Data.DataSet

            Dim PAYMENT As New TalentPayment
            If Not err.HasError Then

                With PAYMENT
                    .Settings = Settings
                    DePay.Source = "S"
                    Me.DePay.FeeDepartment = Me.FeeDepartment
                    .De = Me.DePay
                    .DePED = Me.DePaymentExternal
                    err = .TakePayment
                End With
            End If

            Me.PaymentResultSet = PAYMENT.ResultDataSet

            Return err
        End Function


        Private Function LoadXmlV1(ByVal cNode As XmlNode) As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim intItemCount As Integer = 0
            '-------------------------------------------------------------------------------------
            '   cNode = Customer Request Node
            '   sNode = Customer Sub Node
            '   cdNode = Customer Details Node
            '   prNode = Product Node
            '   iNode = Item Node
            '   pNode = Payment Node
            '   pdNode = Payment Details Node
            Try
                For Each sNode As XmlNode In cNode.ChildNodes
                    Select Case sNode.Name
                        Case Is = "CustomerAdd"
                            DeCust = New DECustomer
                            With DeCust
                                For Each cdNode As XmlNode In sNode.ChildNodes
                                    Select Case cdNode.Name
                                        '-----------------------------------------------------------
                                        '   Contact details
                                        '
                                        Case Is = "ContactTitle"
                                            .ContactTitle = cdNode.InnerText
                                            .ProcessContactTitle = "1"
                                        Case Is = "ContactInitials"
                                            .ContactInitials = cdNode.InnerText
                                            .ProcessContactInitials = "1"
                                        Case Is = "ContactForename"
                                            .ContactForename = cdNode.InnerText
                                            .ProcessContactForename = "1"
                                        Case Is = "ContactSurname"
                                            .ContactSurname = cdNode.InnerText
                                            .ProcessContactSurname = "1"
                                        Case Is = "Salutation"
                                            .Salutation = cdNode.InnerText
                                            .ProcessSalutation = "1"
                                        Case Is = "CompanyName"
                                            .CompanyName = cdNode.InnerText
                                            .ProcessCompanyName = "1"
                                        Case Is = "PositionInCompany"
                                            .PositionInCompany = cdNode.InnerText
                                            .ProcessPositionInCompany = "1"
                                        Case Is = "AddressLine1"
                                            .AddressLine1 = cdNode.InnerText
                                            .ProcessAddressLine1 = "1"
                                        Case Is = "AddressLine2"
                                            .AddressLine2 = cdNode.InnerText
                                            .ProcessAddressLine2 = "1"
                                        Case Is = "AddressLine3"
                                            .AddressLine3 = cdNode.InnerText
                                            .ProcessAddressLine3 = "1"
                                        Case Is = "AddressLine4"
                                            .AddressLine4 = cdNode.InnerText
                                            .ProcessAddressLine4 = "1"
                                        Case Is = "AddressLine5"
                                            .AddressLine5 = cdNode.InnerText
                                            .ProcessAddressLine5 = "1"
                                        Case Is = "PostCode"
                                            .PostCode = cdNode.InnerText
                                            .ProcessPostCode = "1"
                                        Case Is = "Gender"
                                            .Gender = cdNode.InnerText
                                            .ProcessGender = "1"
                                        Case Is = "HomeTelephoneNumber"
                                            .HomeTelephoneNumber = cdNode.InnerText
                                            .ProcessHomeTelephoneNumber = "1"
                                        Case Is = "WorkTelephoneNumber"
                                            .WorkTelephoneNumber = cdNode.InnerText
                                            .ProcessWorkTelephoneNumber = "1"
                                        Case Is = "MobileNumber"
                                            .MobileNumber = cdNode.InnerText
                                            .ProcessMobileNumber = "1"
                                        Case Is = "EmailAddress"
                                            .EmailAddress = cdNode.InnerText
                                            .ProcessEmailAddress = "1"
                                        Case Is = "DateOfBirth"
                                            .DateBirth = cdNode.InnerText
                                            .ProcessDateBirth = "1"
                                        Case Is = "ContactViaMail"
                                            .ContactViaMail = cdNode.InnerText
                                            .ProcessContactViaMail = "1"
                                        Case Is = "Subscription1"
                                            .Subscription1 = cdNode.InnerText
                                            .ProcessSubscription1 = "1"
                                        Case Is = "Subscription2"
                                            .Subscription2 = cdNode.InnerText
                                            .ProcessSubscription2 = "1"
                                        Case Is = "Subscription3"
                                            .Subscription3 = cdNode.InnerText
                                            .ProcessSubscription3 = "1"
                                        Case Is = "ContactViaMail1"
                                            .ContactViaMail1 = cdNode.InnerText
                                            .ProcessContactViaMail1 = "1"
                                        Case Is = "ContactViaMail2"
                                            .ContactViaMail2 = cdNode.InnerText
                                            .ProcessContactViaMail2 = "1"
                                        Case Is = "ContactViaMail3"
                                            .ContactViaMail3 = cdNode.InnerText
                                            .ProcessContactViaMail3 = "1"
                                        Case Is = "ContactViaMail4"
                                            .ContactViaMail4 = cdNode.InnerText
                                            .ProcessContactViaMail4 = "1"
                                        Case Is = "ContactViaMail5"
                                            .ContactViaMail5 = cdNode.InnerText
                                            .ProcessContactViaMail5 = "1"
                                        Case Is = "ExternalId1"
                                            .SLNumber1 = cdNode.InnerText
                                            .ProcessSLNumber1 = "1"
                                        Case Is = "ExternalId2"
                                            .SLNumber2 = cdNode.InnerText
                                            .ProcessSLNumber2 = "1"
                                    End Select

                                Next
                            End With

                        Case Is = "Product"

                            For Each prNode As XmlNode In sNode.ChildNodes
                                Select Case prNode.Name
                                    Case Is = "ProductCode"
                                        DeAddItems.ProductCode = prNode.InnerText
                                    Case Is = "StadiumCode"
                                        DeAddItems.Stadium1 = prNode.InnerText
                                    Case Is = "StandCode"
                                        DeAddItems.StandCode = prNode.InnerText
                                    Case Is = "AreaCode"
                                        DeAddItems.AreaCode = prNode.InnerText
                                    Case Is = "CustomerNumber"
                                        DeAddItems.CustomerNumber = prNode.InnerText
                                    Case Is = "PriceCode"
                                        DeAddItems.PriceCode = prNode.InnerText
                                End Select
                            Next

                        Case Is = "Item"
                            intItemCount = intItemCount + 1
                            With DeAddItems
                                For Each iNode As XmlNode In sNode.ChildNodes
                                    If iNode.Name = "Quantity" Then
                                        Select Case intItemCount
                                            Case Is = 1 : .Quantity01 = iNode.InnerText
                                            Case Is = 2 : .Quantity02 = iNode.InnerText
                                            Case Is = 3 : .Quantity03 = iNode.InnerText
                                            Case Is = 4 : .Quantity04 = iNode.InnerText
                                            Case Is = 5 : .Quantity05 = iNode.InnerText
                                            Case Is = 6 : .Quantity06 = iNode.InnerText
                                            Case Is = 7 : .Quantity07 = iNode.InnerText
                                            Case Is = 8 : .Quantity08 = iNode.InnerText
                                            Case Is = 9 : .Quantity09 = iNode.InnerText
                                            Case Is = 10 : .Quantity10 = iNode.InnerText
                                            Case Is = 11 : .Quantity11 = iNode.InnerText
                                            Case Is = 12 : .Quantity12 = iNode.InnerText
                                            Case Is = 13 : .Quantity13 = iNode.InnerText
                                            Case Is = 14 : .Quantity14 = iNode.InnerText
                                            Case Is = 15 : .Quantity15 = iNode.InnerText
                                            Case Is = 16 : .Quantity16 = iNode.InnerText
                                            Case Is = 17 : .Quantity17 = iNode.InnerText
                                            Case Is = 18 : .Quantity18 = iNode.InnerText
                                            Case Is = 19 : .Quantity19 = iNode.InnerText
                                            Case Is = 20 : .Quantity20 = iNode.InnerText
                                            Case Is = 21 : .Quantity21 = iNode.InnerText
                                            Case Is = 22 : .Quantity22 = iNode.InnerText
                                            Case Is = 23 : .Quantity23 = iNode.InnerText
                                            Case Is = 24 : .Quantity24 = iNode.InnerText
                                            Case Is = 25 : .Quantity25 = iNode.InnerText
                                            Case Is = 26 : .Quantity26 = iNode.InnerText
                                        End Select
                                    End If
                                    If iNode.Name = "PriceBand" Then
                                        Select Case intItemCount
                                            Case Is = 1 : .PriceBand01 = iNode.InnerText
                                            Case Is = 2 : .PriceBand02 = iNode.InnerText
                                            Case Is = 3 : .PriceBand03 = iNode.InnerText
                                            Case Is = 4 : .PriceBand04 = iNode.InnerText
                                            Case Is = 5 : .PriceBand05 = iNode.InnerText
                                            Case Is = 6 : .PriceBand06 = iNode.InnerText
                                            Case Is = 7 : .PriceBand07 = iNode.InnerText
                                            Case Is = 8 : .PriceBand08 = iNode.InnerText
                                            Case Is = 9 : .PriceBand09 = iNode.InnerText
                                            Case Is = 10 : .PriceBand10 = iNode.InnerText
                                            Case Is = 11 : .PriceBand11 = iNode.InnerText
                                            Case Is = 12 : .PriceBand12 = iNode.InnerText
                                            Case Is = 13 : .PriceBand13 = iNode.InnerText
                                            Case Is = 14 : .PriceBand14 = iNode.InnerText
                                            Case Is = 15 : .PriceBand15 = iNode.InnerText
                                            Case Is = 16 : .PriceBand16 = iNode.InnerText
                                            Case Is = 17 : .PriceBand17 = iNode.InnerText
                                            Case Is = 18 : .PriceBand18 = iNode.InnerText
                                            Case Is = 19 : .PriceBand19 = iNode.InnerText
                                            Case Is = 20 : .PriceBand20 = iNode.InnerText
                                            Case Is = 21 : .PriceBand21 = iNode.InnerText
                                            Case Is = 22 : .PriceBand22 = iNode.InnerText
                                            Case Is = 23 : .PriceBand23 = iNode.InnerText
                                            Case Is = 24 : .PriceBand24 = iNode.InnerText
                                            Case Is = 25 : .PriceBand25 = iNode.InnerText
                                            Case Is = 26 : .PriceBand26 = iNode.InnerText
                                        End Select
                                    End If

                                Next
                            End With

                        Case Is = "Payment"

                            Me.DePay = New DEPayments
                            Me.DePaymentExternal = New DEPaymentExternalDetails
                            With Me.DePay
                                For Each pNode As XmlNode In sNode.ChildNodes
                                    Select Case pNode.Name
                                        '-----------------------------------------------------------
                                        '   Payment details
                                        '
                                        Case Is = "PaymentMode"
                                            .PaymentMode = pNode.InnerText

                                        Case Is = "PaymentMethod"
                                            Select Case pNode.InnerText
                                                Case Is = "Cash" : .PaymentType = "CS"
                                                Case Is = "Cheque" : .PaymentType = "CQ"
                                                Case Is = "CreditCard" : .PaymentType = "CC"
                                            End Select

                                        Case Is = "ChequeNumber"
                                            .ChequeNumber = pNode.InnerText

                                        Case Is = "ChequeAccount"
                                            .ChequeAccount = pNode.InnerText

                                        Case Is = "CardNumber"
                                            .CardNumber = pNode.InnerText

                                        Case Is = "ExpiryDate"
                                            .ExpiryDate = pNode.InnerText

                                        Case Is = "StartDate"
                                            .StartDate = pNode.InnerText

                                        Case Is = "IssueNumber"
                                            .IssueNumber = pNode.InnerText

                                        Case Is = "CV2Number"
                                            .CV2Number = pNode.InnerText

                                        Case Is = "PaymentDetails"
                                            For Each pdNode As XmlNode In pNode.ChildNodes
                                                With Me.DePaymentExternal
                                                    Select Case pdNode.Name
                                                        Case Is = "Reference"
                                                            .ExtPaymentReference = pdNode.InnerText
                                                        Case Is = "Name"
                                                            .ExtPaymentName = pdNode.InnerText
                                                        Case Is = "Address1"
                                                            .ExtPaymentAddress1 = pdNode.InnerText
                                                        Case Is = "Address2"
                                                            .ExtPaymentAddress2 = pdNode.InnerText
                                                        Case Is = "Address3"
                                                            .ExtPaymentAddress3 = pdNode.InnerText
                                                        Case Is = "Address4"
                                                            .ExtPaymentAddress4 = pdNode.InnerText
                                                        Case Is = "Country"
                                                            .ExtPaymentCountry = pdNode.InnerText
                                                        Case Is = "PostCode"
                                                            .ExtPaymentPostCode = pdNode.InnerText
                                                        Case Is = "Telephone1"
                                                            .ExtPaymentTel1 = pdNode.InnerText
                                                        Case Is = "Telephone2"
                                                            .ExtPaymentTel2 = pdNode.InnerText
                                                        Case Is = "Telephone3"
                                                            .ExtPaymentTel3 = pdNode.InnerText
                                                        Case Is = "EmailAddress"
                                                            .ExtPaymentEmail = pdNode.InnerText
                                                    End Select
                                                End With

                                            Next
                                    End Select
                                Next
                            End With
                    End Select

                Next
                DeAddItems.Source = "S"
                DeAddItems.ErrorCode = String.Empty
                DeAddItems.ErrorFlag = String.Empty

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message & " - Last XML Record Read = Record " & xmlLoopCount.ToString
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQTBR-02"
                    .HasError = True
                End With
            End Try
            Return err
        End Function



    End Class
End Namespace


