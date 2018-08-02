Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common

'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to handle season ticket renewals
'
'       Date                        17/02/09
'
'       Author                      Ben Ford
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQSTRR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal
    Public Class XMLSeasonTicketRenewalsRequest
        Inherits XmlRequest
        'Invoke constructor on base, passing web service name
        Public Sub New(ByVal webserviceName As String)
            MyBase.new(webserviceName)
        End Sub
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

            Dim xmlAction As XmlSeasonTicketRenewalsResponse = CType(xmlResp, XmlSeasonTicketRenewalsResponse)

            xmlAction.ResponseDirectory = Settings.ResponseDirectory
            Settings.SupplyNetRequestName = "SeasonTicketRenewals"

            Dim err As New ErrorObj

            Try
                '   bNode = Base Node
                For Each bNode As XmlNode In xmlDoc.SelectSingleNode("//SeasonTicketRenewalsRequest").ChildNodes
                    Select Case bNode.Name
                        Case Is = "TransactionHeader"

                        Case Is = "SeasonTicketRenewals"

                            Dim totalRecordsCount As Integer = 0
                            For Each cNode As XmlNode In bNode.ChildNodes
                                totalRecordsCount = totalRecordsCount + 1
                            Next
                            Settings.RequestCount = totalRecordsCount

                            '   cNode = Customer Request Node
                            For Each cNode As XmlNode In bNode.ChildNodes
                                xmlLoopCount += 1
                                ' If one round trip fails we don't want all the
                                ' rest fail so wrap in a Try-Catch
                                Try
                                    err = New ErrorObj
                                    Me.DEGetTransID = New DETicketingItemDetails
                                    Me.DeAddItems = New DEAddTicketingItems

                                    Me.PaymentResultSet = New Data.DataSet

                                    Select Case MyBase.DocumentVersion
                                        Case Is = "1.0"
                                            err = Me.LoadXmlV1(cNode)
                                        Case Is = "1.1"
                                            err = Me.LoadXmlV11(cNode)
                                    End Select

                                    TicketsHaveBeenReserved = False

                                    ' Get the Transaction ID
                                    If Not err.HasError Then err = GetTransactionID(err)

                                    'If there is no error then add the tickets to the basket
                                    If Not err.HasError Then err = AddSeasonTicketRenewalsToBasket(err)

                                    'If the add was successful then set the flag to indicate this
                                    If Not err.HasError Then TicketsHaveBeenReserved = True

                                    'If there has been no error and payment is to be takent then do this now
                                    If Not err.HasError AndAlso TicketsHaveBeenReserved Then
                                        err = TakePayment(err)
                                        If Not err.HasError AndAlso Me.PaymentResultSet.Tables.Count > 0 Then
                                            Dim dtStatusDetails As Data.DataTable = Me.PaymentResultSet.Tables(0)
                                            Dim dr As Data.DataRow = dtStatusDetails.Rows(0)

                                            If Utilities.CheckForDBNull_String(dr("ErrorOccurred")) = "E" Then
                                                err.HasError = True
                                                err.ErrorMessage = "TicketBooker Error Taking Payment - " & Utilities.CheckForDBNull_String(dr("ReturnCode"))
                                                err.ErrorNumber = "TTPRQSTR-04"
                                                err.ErrorStatus = "Payment Error"
                                            Else
                                                If Me.DePay.PaymentType = "DD" Then
                                                    err = GetDirectDebitSummary(err)
                                                End If
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

                                    If Me.PaymentResultSet Is Nothing Then
                                        Me.PaymentResultSet = New Data.DataSet
                                    End If

                                    xmlAction.SuccessCount = Me.SuccessTally
                                    xmlAction.FailureCount = Me.FailureTally


                                    'Add the result to the response nodes
                                    xmlAction.AddNewSeasonTicketRenewalsResponse(Not err.HasError, _
                                                                                    Me.DeAddItems, _
                                                                                    Me.PaymentResultSet, _
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
                    .ErrorNumber = "TTPRQSTR-01"
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


        Protected Function AddSeasonTicketRenewalsToBasket(ByVal err As ErrorObj) As ErrorObj
            Dim BASKET As New TalentBasket

            DeAddItems.IgnoreFriendsAndFamily = "Y"
            If Not err.HasError Then
                With BASKET
                    .DeAddTicketingItems = DeAddItems
                    .DeAddTicketingItems.Source = "S"
                    .Settings = Settings
                    err = .AddSeasonTicketRenewalsReturnBasket
                End With

            End If
            '--------------------------------------
            ' Check if there are any basket entries
            '--------------------------------------
            If Not err.HasError Then
                Try
                    Dim dt As Data.DataTable = BASKET.ResultDataSet.Tables("BasketDetail")
                    If dt.Rows.Count = 0 Then
                        err.HasError = True
                        err.ErrorNumber = "TTPRQSTR-07"
                        err.ErrorMessage = "No results"
                    Else
                        '------------------------------
                        ' Check it's for the right seat
                        '------------------------------
                        Dim seat As String = Utilities.CheckForDBNull_String(dt.Rows(0)("Seat").ToString)
                        If seat = String.Empty Then
                            err.HasError = True
                            err.ErrorNumber = "TTPRQSTR-05"
                            err.ErrorMessage = "No results"
                        Else
                            TicketsHaveBeenReserved = True
                            Dim stand As String = seat.Substring(0, 3).Trim
                            Dim area As String = seat.Substring(3, 4).Trim
                            Dim row As String = seat.Substring(7, 4).Trim
                            Dim seatNo As String = seat.Substring(11, 4).Trim
                            Dim suffix As String = seat.Substring(15, 1).Trim

                            If stand <> DeAddItems.StandCode Or _
                                area <> DeAddItems.AreaCode Or _
                                row <> DeAddItems.Row Or _
                                seatNo <> DeAddItems.Seat Or _
                                suffix <> DeAddItems.Suffix Then
                                err.HasError = True
                                err.ErrorNumber = "TTPRQSTR-06"
                                err.ErrorMessage = "Wrong seat"
                            End If
                        End If
                    End If
                Catch ex As Exception
                    err.HasError = True
                    err.ErrorNumber = "TTPRQSTR-03"
                    err.ErrorMessage = "No results"
                    ' Tickets may have been reserved so set just in case
                    TicketsHaveBeenReserved = True
                End Try

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

        Protected Function GetDirectDebitSummary(ByVal err As ErrorObj) As ErrorObj
            Try
                'Retrieve the payment schedule
                Dim returnErrorCode As String = String.Empty

                'Create the payment object
                Dim payment As New Talent.Common.TalentPayment
                Dim dePayment As New Talent.Common.DEPayments
                With dePayment
                    .PaymentRef = Me.PaymentResultSet.Tables(0).Rows(0)("PaymentReference")
                    .Source = "S"
                    .CustomerNumber = Me.DeAddItems.CustomerNumber
                End With

                payment.De = dePayment
                payment.Settings = Settings

                'Retrieve the direct debit summary
                err = payment.DirectDebitSummary

                'Populate the ddi reference 
                If Not err.HasError _
                        AndAlso Not payment.ResultDataSet Is Nothing Then


                    Dim dt1 As New Data.DataTable()
                    Dim dt2 As New Data.DataTable()

                    dt1 = payment.ResultDataSet.Tables("StatusResults").Copy()
                    dt1.TableName = "DirectDebitStatusResults"
                    dt2 = payment.ResultDataSet.Tables("DirectDebitSummary").Copy()

                    Me.PaymentResultSet.Tables.Add(dt1)
                    Me.PaymentResultSet.Tables.Add(dt2)

                End If

            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = "Error retrieving DD payment schedule"
                    .ErrorNumber = "TTPRQSTR-08"
                    .HasError = True
                End With
            End Try

            Return err

        End Function

        Private Function LoadXmlV1(ByVal cNode As XmlNode) As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim intItemCount As Integer = 0
            '-------------------------------------------------------------------------------------
            '   cNode = Renewal Node
            '   sNode = Customer Sub Node
            '   cdNode = Customer Details Node
            '   prNode = Product Node
            '   iNode = Item Node
            '   pNode = Payment Node
            '   pdNode = Payment Details Node
            Try
                For Each sNode As XmlNode In cNode.ChildNodes
                    Select Case sNode.Name
                        Case Is = "CustomerNo"
                            DeAddItems.CustomerNumber = sNode.InnerText
                        Case Is = "StadiumCode"
                            DeAddItems.Stadium1 = sNode.InnerText
                        Case Is = "Stand"
                            DeAddItems.StandCode = sNode.InnerText
                        Case Is = "Area"
                            DeAddItems.AreaCode = sNode.InnerText
                        Case Is = "Row"
                            DeAddItems.Row = sNode.InnerText
                        Case Is = "Seat"
                            DeAddItems.Seat = sNode.InnerText
                        Case Is = "Suffix"
                            DeAddItems.Suffix = sNode.InnerText

                        Case Is = "Payment"
                            intItemCount = intItemCount + 1
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
                    .ErrorNumber = "TTPRQSTR-02"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

        Private Function LoadXmlV11(ByVal cNode As XmlNode) As ErrorObj
            Const ModuleName As String = "LoadXmlV11"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            Dim intItemCount As Integer = 0
            '-------------------------------------------------------------------------------------
            '   cNode = Renewal Node
            '   sNode = Customer Sub Node
            '   cdNode = Customer Details Node
            '   prNode = Product Node
            '   iNode = Item Node
            '   pNode = Payment Node
            '   pdNode = Payment Details Node
            Try
                For Each sNode As XmlNode In cNode.ChildNodes
                    Select Case sNode.Name
                        Case Is = "CustomerNo"
                            DeAddItems.CustomerNumber = sNode.InnerText
                        Case Is = "AllocatedCustNo"
                            DeAddItems.AllocatedCustNo = sNode.InnerText
                        Case Is = "StadiumCode"
                            DeAddItems.Stadium1 = sNode.InnerText
                        Case Is = "Stand"
                            DeAddItems.StandCode = sNode.InnerText
                        Case Is = "Area"
                            DeAddItems.AreaCode = sNode.InnerText
                        Case Is = "Row"
                            DeAddItems.Row = sNode.InnerText
                        Case Is = "Seat"
                            DeAddItems.Seat = sNode.InnerText
                        Case Is = "Suffix"
                            DeAddItems.Suffix = sNode.InnerText

                        Case Is = "Payment"
                            intItemCount = intItemCount + 1
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
                                                Case Is = "DirectDebit" : .PaymentType = "DD"
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

                                        Case Is = "AccountName"
                                            .AccountName = pNode.InnerText

                                        Case Is = "SortCode"
                                            .SortCode = pNode.InnerText

                                        Case Is = "AccountCode"
                                            .AccountNumber = pNode.InnerText

                                        Case Is = "PaymentDay"
                                            .PaymentDay = pNode.InnerText

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
                    .ErrorNumber = "TTPRQSTR-02"
                    .HasError = True
                End With
            End Try
            Return err
        End Function


    End Class
End Namespace


