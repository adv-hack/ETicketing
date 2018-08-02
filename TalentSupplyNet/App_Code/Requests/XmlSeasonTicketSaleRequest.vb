Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used for season ticket sale
'
'       Date                        March 2009
'
'       Author                      Lars Jacobsson 
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
    Public Class XmlSeasonTicketSaleRequest
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
                Return True
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
        Private _retCustResultSet As Data.DataSet
        Public Property RetrieveCustomerResultSet() As Data.DataSet
            Get
                Return _retCustResultSet
            End Get
            Set(ByVal value As Data.DataSet)
                _retCustResultSet = value
            End Set
        End Property

        Private _ticketsHaveBeenReserved As Boolean
        Public Property TicketsHaveBeenReserved() As Boolean
            Get
                Return _ticketsHaveBeenReserved
            End Get
            Set(ByVal value As Boolean)
                _ticketsHaveBeenReserved = value
            End Set
        End Property

        Dim xmlLoopCount As Integer = 0


        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse

            Me.SuccessTally = 0
            Me.FailureTally = 0
            Me.FailedRegistrationTally = 0

            Settings.SupplyNetRequestName = "SeasonTicketSale"
            Dim xmlAction As XmlSeasonTicketSaleResponse = CType(xmlResp, XmlSeasonTicketSaleResponse)

            xmlAction.ResponseDirectory = Settings.ResponseDirectory

            Dim err As New ErrorObj
            Dim Row As String = ""
            Dim Seat As String = ""

            ' Save the setting for determining if we are logging requests
            Dim LogRequestsSav As New Boolean
            LogRequestsSav = Settings.LogRequests

            '
            ' Switch of logging property for settings as this call will 
            ' call a few different TalentCommon Functions and we do not wich to log each one.
            Settings.LogRequests = False

            Try
                '   bNode = Base Node
                For Each bNode As XmlNode In xmlDoc.SelectSingleNode("//SeasonTicketSaleRequest/SeasonTicketSale").ChildNodes

                    Select Case bNode.Name
                        Case Is = "Header"
                            For Each hNode As XmlNode In bNode.ChildNodes
                                Select Case hNode.Name
                                    Case Is = "RegisterCustomers"
                                        Me.RegisterCustomers = SetBooleanFromYN(hNode.InnerText, False)
                                    Case Is = "ContinueOnFailedRegistration"
                                        Me.ContinueOnFailedRegistration = SetBooleanFromYN(hNode.InnerText, False)
                                        'Case Is = "FeeDepartment"
                                        '    Me.FeeDepartment = hNode.InnerText

                                End Select
                            Next
                        Case Is = "Details"

                            Dim totalRecordsCount As Integer = 0
                            For Each cNode As XmlNode In bNode.ChildNodes
                                totalRecordsCount = totalRecordsCount + 1
                            Next
                            Settings.RequestCount = totalRecordsCount

                            ' Create the supplyNet Request
                            If LogRequestsSav Then
                                Settings.LogRequests = True
                                createSupplyNetRequest(Settings.Partner, Settings.LoginId, Settings.SupplyNetRequestName, Settings.TransactionID, Settings.RequestCount, 0, Now, Nothing, True)
                                Settings.LogRequests = False
                            End If


                            For Each hNode As XmlNode In bNode.ChildNodes
                                Select Case hNode.Name
                                    Case Is = "Sale"
                                        Me.DEGetTransID = New DETicketingItemDetails
                                        'Me.DeAddItems = New DEAddTicketingItems
                                        Me.RegisterCustomerResultSet = New Data.DataSet
                                        err = New ErrorObj
                                        Me.PaymentResultSet = New Data.DataSet
                                        Select Case MyBase.DocumentVersion
                                            Case Is = "1.0"
                                                err = Me.LoadXmlV1(hNode)
                                        End Select

                                        Dim registrationSuccess As Boolean = True

                                        'If customers are to be registered then do this first
                                        If Me.RegisterCustomers Then err = RegisterCustomer(err)

                                        'If registration failed, but we are to continue with
                                        'the sale regardless then reset the error object here
                                        If err.HasError Then
                                            '-----------------------------------------------------
                                            ' BF - 170509
                                            ' Currently this flag wont work because WS003R doesn't 
                                            ' return the Customer Number of the matched record!!
                                            '-----------------------------------------------------
                                            'If Me.ContinueOnFasiledRegistration Then
                                            '    err = New ErrorObj
                                            '    registrationSuccess = False
                                            '    Me.FailedRegistrationTally += 1
                                            'End If
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
                                                        'If Not Me.ContinueOnFailedRegistration Then
                                                        err.HasError = True
                                                        err.ErrorMessage = "SeasonTicketSale Error Registering User - " & Utilities.CheckForDBNull_String(dr("ReturnCode")) & " - " & DeCust.EmailAddress
                                                        err.ErrorNumber = "TTPRQTBR-03"
                                                        err.ErrorStatus = "Registration Error"
                                                        'End If
                                                    End If
                                                End If
                                            End If
                                        End If

                                        ' Retrieve customer if not already done so to get price band
                                        If Not err.HasError Then err = RetrieveCustomer(err)

                                        'If there is no error then get the Transaction ID
                                        If Not err.HasError Then err = GetTransactionID(err)

                                        'If there is no error then add the tickets to the basket
                                        If Not err.HasError Then err = AddTicketsToBasket(err)

                                        'If there has been no error and payment is to be takent then do this now
                                        If Not err.HasError AndAlso Me.TakePayments Then
                                            err = TakePayment(err)
                                            If Not err.HasError AndAlso Me.PaymentResultSet.Tables.Count > 0 Then
                                                Dim dtStatusDetails As Data.DataTable = Me.PaymentResultSet.Tables(0)
                                                Dim dr As Data.DataRow = dtStatusDetails.Rows(0)

                                                If Utilities.CheckForDBNull_String(dr("ErrorOccurred")) = "E" Then
                                                    err.HasError = True
                                                    err.ErrorMessage = "SeasonTicketSale Error Taking Payment - " & Utilities.CheckForDBNull_String(dr("ReturnCode"))
                                                    err.ErrorNumber = "TTPRQTBR-04b"
                                                    err.ErrorStatus = "Payment Error"
                                                End If
                                            End If
                                        End If

                                        'If tickets have been reserved but payment failed then release the tickets
                                        If err.HasError AndAlso TicketsHaveBeenReserved Then
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

                                        ' Update the supplyNet Request record
                                        If LogRequestsSav Then
                                            Settings.LogRequests = True
                                            incrementSupplyNetProgressCount(Settings.TransactionID)
                                            Settings.LogRequests = False
                                        End If

                                        'Add the result to the response nodes
                                        xmlAction.AddNewSeasonTicketSaleCustomerResponse(Not err.HasError, _
                                                                                        Me.DeAddItems, _
                                                                                        Me.DeCust, _
                                                                                        Me.RegisterCustomerResultSet, _
                                                                                        Me.PaymentResultSet, _
                                                                                        registrationSuccess, _
                                                                                        err.ErrorNumber)


                                End Select
                            Next

                            'Set the corresponding properties on the Response
                            xmlAction.RegisterCustomers = Me.RegisterCustomers
                            xmlAction.TakePayments = Me.TakePayments
                            xmlAction.ContinueOnFailedRegistration = Me.ContinueOnFailedRegistration
                    End Select
                Next

                'Reset the settings supplynet logging properties
                Settings.LogRequests = LogRequestsSav
                If Settings.LogRequests Then
                    markSupplyNetTransactionAsCompleted(TransactionID)
                End If

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

            If DeCust.PositionInCompany <> String.Empty OrElse DeCust.CompanyName <> String.Empty Then
                DeCust.UpdateCompanyInformation = "Y"
            End If

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
        Protected Function RetrieveCustomer(ByVal err As ErrorObj) As ErrorObj
            Me.RetrieveCustomerResultSet = New Data.DataSet

            Dim CUSTOMER As New TalentCustomer
            Dim deCustV11 As New DECustomerV11
            deCustV11.DECustomersV1.Add(DeCust)
            If Not err.HasError Then
                With CUSTOMER
                    .DeV11 = deCustV11
                    .Settings = Settings
                    err = .CustomerRetrieval
                End With
            End If

            Me.RetrieveCustomerResultSet = CUSTOMER.ResultDataSet

            If Not err.HasError Then
                If Me.RetrieveCustomerResultSet.Tables.Count > 1 Then
                    If Me.RetrieveCustomerResultSet.Tables(1).Rows.Count > 0 Then
                        Dim dr As Data.DataRow = Me.RetrieveCustomerResultSet.Tables(1).Rows(0)
                        Me.DeAddItems.PriceBand01 = Utilities.CheckForDBNull_String(dr("PriceBand"))
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
                    .Settings = Settings
                    err = .AddTicketingItemsReturnBasket
                End With
                '------------------------------------------------------------
                'If the add was successful then set the flag to indicate this
                '------------------------------------------------------------
                If Not err.HasError Then TicketsHaveBeenReserved = True

                '---------------------------------------------
                ' Check if there were any errors in the basket
                '---------------------------------------------
                Dim dtBasket As Data.DataTable = BASKET.ResultDataSet.Tables(2)
                For Each basketRow As Data.DataRow In dtBasket.Rows
                    If basketRow("ErrorCode").ToString.Trim <> String.Empty Then
                        With err
                            .ErrorMessage = "Error on basket item: " & basketRow("ErrorCode").ToString.Trim & _
                                            "-" & basketRow("ProductCode")
                            .ErrorStatus = "AddTicketingItemsReturnBasket Error"
                            .ErrorNumber = "TTPRQTBR-04"
                            .HasError = True
                        End With
                    End If
                Next
                '-------------------------
                ' Check if basket is empty
                '-------------------------
                If dtBasket.Rows.Count = 0 Then
                    With err
                        .ErrorMessage = "No items in basket"
                        .ErrorStatus = "AddTicketingItemsReturnBasket Error"
                        .ErrorNumber = "TTPRQTBR-05"
                        .HasError = True
                    End With

                End If
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
                    err = .TakePaymentReadOrder
                End With
            End If

            Me.PaymentResultSet = PAYMENT.ResultDataSet

            Return err
        End Function


        Private Function LoadXmlV1(ByVal cNode As XmlNode) As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            Dim Seat As String = ""
            Dim Row As String = ""
            '--------------------------------------------------------------------------
            Dim intItemCount As Integer = 0
            '-------------------------------------------------------------------------------------

            Try
                For Each sNode As XmlNode In cNode.ChildNodes
                    Select Case sNode.Name
                        Case Is = "CustomerNo"
                            If sNode.InnerText.Trim <> String.Empty Then
                                DeAddItems.CustomerNumber = sNode.InnerText
                            Else
                                DeAddItems.CustomerNumber = "000000000000"
                            End If
                        Case Is = "CustomerDetails"
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
                                        Case "MothersName"
                                            .MothersName = cdNode.InnerText
                                            .ProcessMothersName = "1"

                                        Case "FathersName"
                                            .FathersName = cdNode.InnerText
                                            .ProcessFathersName = "1"

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
                                        Case Is = "Password"
                                            .Password = cdNode.InnerText
                                        Case Is = "UserID1"
                                            .PassportNumber = cdNode.InnerText
                                        Case Is = "UserID2"
                                            .GreenCardNumber = cdNode.InnerText
                                        Case Is = "UserID3"
                                            .PIN_Number = cdNode.InnerText
                                        Case Is = "UserID4"
                                            .User_ID_4 = cdNode.InnerText
                                        Case Is = "UserID5"
                                            .User_ID_5 = cdNode.InnerText
                                        Case Is = "UserID6"
                                            .User_ID_6 = cdNode.InnerText
                                        Case Is = "UserID7"
                                            .User_ID_7 = cdNode.InnerText
                                        Case Is = "UserID8"
                                            .User_ID_8 = cdNode.InnerText
                                        Case Is = "UserID9"
                                            .User_ID_9 = cdNode.InnerText
                                        Case Is = "Attributes"
                                            Dim count As Integer = 1
                                            For Each Node3 As XmlNode In cdNode.ChildNodes

                                                Select Case count
                                                    Case Is = 1
                                                        .Attribute01 = Node3.InnerText
                                                        .Attribute01Action = SetAttributeAction(Node3.Attributes("action").Value)
                                                    Case Is = 2
                                                        .Attribute04 = Node3.InnerText
                                                        .Attribute04Action = SetAttributeAction(Node3.Attributes("action").Value)
                                                    Case Is = 3
                                                        .Attribute05 = Node3.InnerText
                                                        .Attribute05Action = SetAttributeAction(Node3.Attributes("action").Value)
                                                End Select
                                                count += 1
                                            Next

                                    End Select

                                Next
                            End With

                        Case Is = "ProductCode"
                            DeAddItems.ProductCode = sNode.InnerText
                        Case Is = "StadiumCode"
                            DeAddItems.Stadium1 = sNode.InnerText
                        Case Is = "Stand"
                            DeAddItems.StandCode = sNode.InnerText
                        Case Is = "Area"
                            DeAddItems.AreaCode = sNode.InnerText
                        Case Is = "CustomerNo"
                            DeAddItems.CustomerNumber = sNode.InnerText
                        Case Is = "Seat"
                            Seat = sNode.InnerText
                            DeAddItems.Seat = Seat
                        Case Is = "Row"
                            Row = sNode.InnerText
                            DeAddItems.Row = Row

                        Case Is = "Suffix"
                            DeAddItems.RowSeat01 = Row + Space(4 - Len(Row)) + Seat + Space(4 - Len(Seat)) + sNode.InnerText
                            DeAddItems.Suffix = sNode.InnerText

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
                'TODO - Remove hardcoded quantity and priceband below

                DeAddItems.Quantity01 = "1"

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


