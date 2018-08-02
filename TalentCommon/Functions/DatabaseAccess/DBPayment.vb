Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Text
Imports IBM.Data.DB2.iSeries
Imports Talent.Common
Imports Talent.Common.Utilities

'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Payments 
'
'       Date                        June 2007
'
'       Author                      Des Webster
'
'       � CS Group 2006             All rights reserved.
'                                    
'       Error Number Code base      TACDBPY- 
'                                   
'       Modification Summary
'
'       dd/mm/yy    By              Description
'       --------    ---             -----------
'       31/08/07    A Green         Added Refund Payment functionality
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBPayment
    Inherits DBAccess

    Private _de As New DEPayments
    Private _dePED As New DEPaymentExternalDetails
    Private _deDelDetails As New DEDeliveryDetails
    Private _derpay As New DERefundPayment
    Private _payDetails As Generic.Dictionary(Of String, DEPayments)
    Private _saveCardFunctions As New DBSaveCardFunctions
    Private _moreThan40CashbackCustomers As Boolean = False
    Private _startNextCashbackCustomer As Integer = 1
    Private _cmdAdapter As iDB2DataAdapter = Nothing
    Private _cmd As iDB2Command = Nothing

    Private Const TakePayment As String = "TakePayment"
    Private Const RefundPayment As String = "RefundPayment"
    Private Const CancelAllPayments As String = "CancelAllPayments"
    Private Const TakePaymentReadOrder As String = "TakePaymentReadOrder"
    Private Const PaymentPending As String = "PaymentPending"
    Private Const TakePaymentViaBackend As String = "TakePaymentViaBackend"
    Private Const DirectDebitDDIRef As String = "DirectDebitDDIRef"
    Private Const DirectDebitPaymentDays As String = "DirectDebitPaymentDays"
    Private Const DirectDebitSummary As String = "DirectDebitSummary"
    Private Const GenerateTransactionID As String = "GenerateTransactionID"
    Private Const RetrieveCashback As String = "RetrieveCashback"
    Private Const UpdateCashback As String = "UpdateCashback"
    Private Const OnAccountAdjustment As String = "OnAccountAdjustment"
    Private Const UpdateAndRetrievePaymentCharges As String = "UpdateAndRetrievePaymentCharges"
    Private Const RetrieveAndUpdateAdHocFees As String = "RetrieveAndUpdateAdHocFees"
    Private Const TakePartPayment As String = "TakePartPayment"
    Private Const CancelPartPayment As String = "CancelPartPayment"
    Private Const RetrieveEPurseTotal As String = "RetrieveEPurseTotal"
    Private Const RetrievePartPayments As String = "RetrievePartPayments"
    Private Const RetrieveMySavedCards As String = "RetrieveMySavedCards"
    Private Const SaveMyCard As String = "SaveMyCard"
    Private Const DeleteMyCard As String = "DeleteMyCard"
    Private Const SetMyCardAsDefault As String = "SetMyCardAsDefault"
    Private Const RetrieveOnAccountDetails As String = "RetrieveOnAccountDetails"
    Private Const OrderCompletionUpdates As String = "OrderCompletionUpdates"
    Private Const RetrieveVariablePricedProducts As String = "RetrieveVariablePricedProducts"
    Private _orderFunctions As New DBOrderFunctions

    Public Property De() As DEPayments
        Get
            Return _de
        End Get
        Set(ByVal value As DEPayments)
            _de = value
        End Set
    End Property
    Public Property DePED() As DEPaymentExternalDetails
        Get
            Return _dePED
        End Get
        Set(ByVal value As DEPaymentExternalDetails)
            _dePED = value
        End Set
    End Property
    Public Property DeDelDetails() As DEDeliveryDetails
        Get
            Return _deDelDetails
        End Get
        Set(ByVal value As DEDeliveryDetails)
            _deDelDetails = value
        End Set
    End Property
    Public Property Derpay() As DERefundPayment
        Get
            Return _derpay
        End Get
        Set(ByVal value As DERefundPayment)
            _derpay = value
        End Set
    End Property
    Public Property DBOrderFunctions() As DBOrderFunctions
        Get
            Return _orderFunctions
        End Get
        Set(ByVal value As DBOrderFunctions)
            _orderFunctions = value
        End Set
    End Property
    Public Property PayDetails() As Generic.Dictionary(Of String, DEPayments)
        Get
            Return _payDetails
        End Get
        Set(ByVal value As Generic.Dictionary(Of String, DEPayments))
            _payDetails = value
        End Set
    End Property

    Public Property OrderLevelFulfilmentMethod() As String = String.Empty
    Public Property DDScheduleAdjusted() As Boolean = False
    Public Property SendEmail() As Boolean = True

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj

        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            ' This is full ticketing completion routine
            Case Is = TakePayment : err = AccessDatabaseWS608R(True)
            Case Is = RefundPayment : err = AccessDatabaseWS050R()
            Case Is = CancelAllPayments : err = AccessDatabaseWS650R()
            Case Is = TakePaymentReadOrder : err = AccessDatabaseWS608R()
            Case Is = PaymentPending : err = AccessDatabaseWS631R()
                ' This is just taking a payment
            Case Is = TakePaymentViaBackend : err = AccessDatabaseWS094R()
            Case Is = DirectDebitDDIRef : err = AccessDatabaseWS084R()
            Case Is = DirectDebitPaymentDays : err = AccessDatabaseWS084R()
            Case Is = DirectDebitSummary : err = AccessDatabaseWS086R()
            Case Is = GenerateTransactionID : err = AccessDatabaseWS097R()
                ' Cashback module
            Case Is = RetrieveCashback : err = AccessDatabaseWS057R()
            Case Is = UpdateCashback : err = AccessDatabaseWS057R()
            Case Is = OnAccountAdjustment : err = AccessDatabaseWS057R()
                ' Adhoc Fees
            Case Is = UpdateAndRetrievePaymentCharges : err = AccessDatabaseWS012R()
            Case Is = RetrieveAndUpdateAdHocFees : err = AccessDatabaseWS173R()
                ' Save My Card Functions
            Case Is = RetrieveMySavedCards, SaveMyCard, DeleteMyCard, SetMyCardAsDefault : err = AccessDatabaseWS060R()

            Case Is = UpdateAndRetrievePaymentCharges : err = AccessDatabaseWS012R()
            Case Is = RetrieveAndUpdateAdHocFees : err = AccessDatabaseWS173R()
                ' E-Purse
            Case Is = RetrieveEPurseTotal : err = AccessDatabaseFW035R()
                ' Partial Payment
            Case Is = TakePartPayment : err = AccessDatabaseWS618R()
            Case Is = CancelPartPayment : err = AccessDatabaseWS618R()
            Case Is = RetrievePartPayments : err = AccessDatabaseWS098R()
                ' On account details
            Case Is = RetrieveOnAccountDetails : err = AccessDatabaseWS067R()
                ' End of a transaction
            Case Is = OrderCompletionUpdates : err = AccessDatabaseWS001S()
            Case Is = RetrieveVariablePricedProducts : err = AccessDatabaseMD042S()
        End Select

        Return err

    End Function

    Private Function AccessDatabaseWS032R(Optional ByVal parmWS032R As String = "", Optional ByVal paymentOnly As Boolean = False) As ErrorObj

        Dim err As New ErrorObj
        If parmWS032R.Trim = "" Then
            ResultDataSet = New DataSet
        End If
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("AdditionalInfo", GetType(String))
            .Add("PaymentReference", GetType(String))
        End With

        'Create the payment Details data table
        Dim DtPaymentResults As New DataTable("PaymentResults")
        With DtPaymentResults.Columns
            .Add("PaymentReference", GetType(String))
            .Add("BatchReference", GetType(String))
            .Add("TotalPrice", GetType(String))
        End With

        'Payment table is not needed when called from the wrapper program
        If parmWS032R.Trim = "" Or paymentOnly Then
            ResultDataSet.Tables.Add(DtPaymentResults)
        End If

        Try

            'Has the parameter been passed in
            If parmWS032R.Trim <> "" Then
                PARAMOUT = parmWS032R
            Else
                'Call WS032R
                PARAMOUT = CallWS032R()
            End If

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(4999, 1) = "E" Or PARAMOUT.Substring(4997, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(4997, 2)
                dRow("Additionalinfo") = PARAMOUT.Substring(949, 30).Trim
                dRow("PaymentReference") = ""
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                dRow("AdditionalInfo") = ""
                dRow("PaymentReference") = PARAMOUT.Substring(53, 15).TrimStart("0")
            End If
            DtStatusResults.Rows.Add(dRow)

            'No errors 
            If PARAMOUT.Substring(4999, 1) <> "E" AndAlso PARAMOUT.Substring(4997, 2).Trim = "" Then

                'Create a new row
                dRow = Nothing
                dRow = DtPaymentResults.NewRow
                dRow("PaymentReference") = PARAMOUT.Substring(53, 15)
                dRow("BatchReference") = PARAMOUT.Substring(68, 9)
                dRow("TotalPrice") = Utilities.FormatPrice(PARAMOUT.Substring(77, 11))
                If PARAMOUT.Substring(51, 1) = "Y" Then
                    dRow("TotalPrice") = dRow("TotalPrice").ToString.Trim + "-"
                End If
                DtPaymentResults.Rows.Add(dRow)

            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPY-01"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS032R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS032R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5000)
        parmIO.Value = WS032RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS032R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS032R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function AccessDatabaseWS050R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim intCount As Integer = 0
        Dim intStart As Integer = 0
        Dim sngRefundTotal As Single = 0
        Dim sSeatString As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the refund payment Details data table
        Dim DtRefundPaymentDetails As New DataTable("RefundPaymentDetails")
        ResultDataSet.Tables.Add(DtRefundPaymentDetails)
        With DtRefundPaymentDetails.Columns
            .Add("RefundReference", GetType(String))
            .Add("RefundTotal", GetType(String))
            .Add("RefundFeeTotal", GetType(String))
            .Add("RefundCustomerNo", GetType(String))
            .Add("RefundCustomerName", GetType(String))
        End With

        'Create the refund payment Details data table
        Dim DtRefundProductDetails As New DataTable("RefundProductDetails")
        ResultDataSet.Tables.Add(DtRefundProductDetails)
        With DtRefundProductDetails.Columns
            .Add("CustomerNo", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("Seat", GetType(String))
            .Add("Price", GetType(String))
        End With

        Try

            'Call WS050R
            PARAMOUT = CallWS050R()

            Dim intLen As Integer = PARAMOUT.Length

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(3069, 1) = "E" Or PARAMOUT.Substring(3070, 2) <> "  " Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(3070, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            'No errors 
            If Not (PARAMOUT.Substring(3069, 1) = "E" Or PARAMOUT.Substring(3070, 2) <> "  ") Then

                Do While intCount <= 50
                    intStart = (intCount * 43)
                    sSeatString = PARAMOUT.Substring(intStart, 43)
                    If sSeatString.Trim <> "" Then
                        'Create a new row
                        dRow = Nothing
                        dRow = DtRefundProductDetails.NewRow
                        dRow("ProductCode") = PARAMOUT.Substring(intStart, 6)
                        If PARAMOUT.Substring(intStart + 21, 1).Trim = "" Then
                            dRow("Seat") = PARAMOUT.Substring(intStart + 6, 3).Trim & "/" & _
                                                    PARAMOUT.Substring(intStart + 9, 4).Trim & "/" & _
                                                    PARAMOUT.Substring(intStart + 13, 4).Trim & "/" & _
                                                    PARAMOUT.Substring(intStart + 17, 4).Trim
                        Else
                            dRow("Seat") = PARAMOUT.Substring(intStart + 6, 3).Trim & "/" & _
                                                    PARAMOUT.Substring(intStart + 9, 4).Trim & "/" & _
                                                    PARAMOUT.Substring(intStart + 13, 4).Trim & "/" & _
                                                    PARAMOUT.Substring(intStart + 17, 4).Trim & "/" & _
                                                    PARAMOUT.Substring(intStart + 21, 1).Trim
                        End If
                        dRow("Price") = PARAMOUT.Substring(intStart + 22, 9)
                        dRow("CustomerNo") = PARAMOUT.Substring(intStart + 31, 12)
                        If IsNumeric(dRow("Price")) Then
                            sngRefundTotal = sngRefundTotal + CType(dRow("Price"), Single)
                        End If
                        DtRefundProductDetails.Rows.Add(dRow)
                    Else
                        intCount = 51
                    End If
                    intCount = intCount + 1
                Loop

                dRow = Nothing
                dRow = DtRefundPaymentDetails.NewRow
                dRow("RefundCustomerName") = PARAMOUT.Substring(2906, 15)
                dRow("RefundTotal") = Utilities.FormatPrice(PARAMOUT.Substring(2958, 15))
                dRow("RefundFeeTotal") = Utilities.FormatPrice(PARAMOUT.Substring(2973, 15))
                dRow("RefundReference") = PARAMOUT.Substring(2988, 15)
                dRow("RefundCustomerNo") = PARAMOUT.Substring(3022, 15)
                DtRefundPaymentDetails.Rows.Add(dRow)

            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPY-02"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS050R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS050R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS050Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS050R", Derpay.RefundCustomerNo, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS050R", Derpay.RefundCustomerNo, "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS050Parm() As String

        Dim myString As String = String.Empty
        Dim intCount As Integer = 1
        Dim sCancelAll As String = String.Empty
        Dim paymentMode As String = String.Empty

        Dim DEri As New DERefundItem
        Dim DEsd As New DESeatDetails
        Dim DEtid As New DETicketingItemDetails

        Dim intLen As Integer = 0

        'Set the Cancel All flag
        Select Case Derpay.PaymentReference.Trim
            Case Is = "" : sCancelAll = "N"
            Case Is <> "" : sCancelAll = "Y"
        End Select

        'Set the payment Mode
        Select Case Derpay.PaymentDetails.PaymentMode
            Case Is = "Batch" : paymentMode = "B"
            Case Is = "Inline" : paymentMode = "I"
            Case Is = "None" : paymentMode = "N"
            Case Else : paymentMode = ""
        End Select

        'Construct the parameter
        Do While intCount <= 50
            If Derpay.CollRefundItems.Count > 0 And intCount <= Derpay.CollRefundItems.Count Then
                DEri = CType(Derpay.CollRefundItems(intCount), DERefundItem)
                DEtid.SeatDetails1.FormattedSeat = DEri.ProductDetails
                myString = myString & Utilities.FixStringLength(DEri.ProductCode, 6) & _
                                    Utilities.FixStringLength(DEtid.SeatDetails1.Stand, 3) & _
                                    Utilities.FixStringLength(DEtid.SeatDetails1.Area, 4) & _
                                    Utilities.FixStringLength(DEtid.SeatDetails1.Row, 4) & _
                                    Utilities.FixStringLength(DEtid.SeatDetails1.Seat, 4) & _
                                    Utilities.FixStringLength(DEtid.SeatDetails1.AlphaSuffix, 1) & _
                                    Utilities.FixStringLength("", 21)
            Else
                myString = myString & Utilities.FixStringLength("", 43)
            End If
            If True Then

            End If

            intCount = intCount + 1
        Loop
        intLen = myString.Length

        myString = myString & Utilities.FixStringLength("", 729) & _
                                    Utilities.FixStringLength(Derpay.PaymentDetails.RetainCustomerReservations, 1) & _
                                    Utilities.FixStringLength("", 77) & _
                                    Utilities.FixStringLength(Derpay.CancelMode, 1) & _
                                    Utilities.FixStringLength("", 15) & _
                                    Utilities.FixStringLength("", 15) & _
                                    "000000000000000" & _
                                    Utilities.PadLeadingZeros(Derpay.PaymentReference, 15) & _
                                    Utilities.FixStringLength(sCancelAll, 1) & _
                                    Utilities.FixStringLength(Derpay.PaymentDetails.PaymentMode, 1) & _
                                    Utilities.FixStringLength(Derpay.PaymentDetails.PaymentType, 2) & _
                                    Utilities.PadLeadingZeros(Derpay.RefundCustomerNo, 12) & _
                                    Utilities.FixStringLength(Derpay.PaymentDetails.CardNumber, 19) & _
                                    Utilities.FixStringLength(Derpay.PaymentDetails.ExpiryDate, 4) & _
                                    Utilities.FixStringLength(Derpay.PaymentDetails.StartDate, 4) & _
                                    Utilities.FixStringLength(Derpay.PaymentDetails.IssueNumber, 2) & _
                                    Utilities.FixStringLength(Derpay.PaymentDetails.CV2Number, 4) & "Y    " '
        'e#Mode              2958   2958    
        'e#SeatTotal         2959   2973  2 
        'e#FeeTotal          2974   2988  2 
        'e#NxtPyrf           2989   3003  0 
        'e#pyrf              3004   3018  0 
        'e#CnxAll            3019   3019    
        'e#RefundMode        3020   3020    
        'e#RefundType        3021   3022    
        'e#RefundMemb        3023   3034    
        intLen = myString.Length

        Return myString

    End Function

    Private Function AccessDatabaseWS650R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim intCount As Integer = 0
        Dim intStart As Integer = 0
        Dim sngRefundTotal As Single = 0
        Dim sSeatString As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the refund payment Details data table
        Dim DtRefundPaymentDetails As New DataTable("RefundPaymentDetails")
        ResultDataSet.Tables.Add(DtRefundPaymentDetails)
        With DtRefundPaymentDetails.Columns
            .Add("RefundTotal", GetType(String))
            .Add("RefundFeeTotal", GetType(String))
            .Add("RefundCustomerNo", GetType(String))
            .Add("RefundCustomerName", GetType(String))
        End With

        'Create the refund payment Details data table
        Dim DtRefundProductDetails As New DataTable("RefundProductDetails")
        ResultDataSet.Tables.Add(DtRefundProductDetails)
        With DtRefundProductDetails.Columns
            .Add("ProductCode", GetType(String))
            .Add("PriceBand", GetType(String))
            .Add("PriceCode", GetType(String))
            .Add("CustomerNo", GetType(String))
            .Add("NumberOfCancellations", GetType(String))
            .Add("RefundTotal", GetType(String))
            .Add("RefundReference", GetType(String))
        End With

        Try

            'Call WS650R
            PARAMOUT = CallWS650R()

            Dim intLen As Integer = PARAMOUT.Length

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(3069, 1) = "E" Or PARAMOUT.Substring(3070, 2) <> "  " Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(3070, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            'No errors 
            If Not (PARAMOUT.Substring(3069, 1) = "E" Or PARAMOUT.Substring(3070, 2) <> "  ") Then

                Do While intCount <= 50
                    intStart = (intCount * 54)
                    sSeatString = PARAMOUT.Substring(intStart, 54)
                    If sSeatString.Trim <> "" Then
                        'Create a new row
                        dRow = Nothing
                        dRow = DtRefundProductDetails.NewRow
                        dRow("ProductCode") = PARAMOUT.Substring(intStart, 6)
                        dRow("PriceBand") = PARAMOUT.Substring(intStart + 6, 2)
                        dRow("PriceCode") = PARAMOUT.Substring(intStart + 8, 1)
                        dRow("CustomerNo") = PARAMOUT.Substring(intStart + 9, 12)
                        dRow("NumberOfCancellations") = PARAMOUT.Substring(intStart + 21, 3)
                        dRow("RefundTotal") = Utilities.FormatPrice(PARAMOUT.Substring(intStart + 24, 15))
                        dRow("RefundReference") = PARAMOUT.Substring(intStart + 39, 15)
                        DtRefundProductDetails.Rows.Add(dRow)
                    Else
                        intCount = 51
                    End If
                    intCount = intCount + 1
                Loop

                dRow = Nothing
                dRow = DtRefundPaymentDetails.NewRow
                dRow("RefundCustomerName") = PARAMOUT.Substring(2906, 15)
                dRow("RefundTotal") = Utilities.FormatPrice(PARAMOUT.Substring(2958, 15))
                dRow("RefundFeeTotal") = Utilities.FormatPrice(PARAMOUT.Substring(2973, 15))
                dRow("RefundCustomerNo") = PARAMOUT.Substring(3022, 15)
                DtRefundPaymentDetails.Rows.Add(dRow)

            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPY-WS650R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS650R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS650R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS650Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS650R", Derpay.RefundCustomerNo, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS650R", Derpay.RefundCustomerNo, "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS650Parm() As String

        Dim myString As String = String.Empty
        Dim intCount As Integer = 1
        Dim sCancelAll As String = String.Empty
        Dim paymentMode As String = String.Empty

        Dim DEri As New DERefundItem
        Dim DEsd As New DESeatDetails
        Dim DEtid As New DETicketingItemDetails

        Dim intLen As Integer = 0

        'Set the Cancel All flag
        Select Case Derpay.PaymentReference.Trim
            Case Is = "" : sCancelAll = "N"
            Case Is <> "" : sCancelAll = "Y"
        End Select

        'Set the payment Mode
        Select Case Derpay.PaymentDetails.PaymentMode
            Case Is = "Batch" : paymentMode = "B"
            Case Is = "Inline" : paymentMode = "I"
            Case Is = "None" : paymentMode = "N"
            Case Else : paymentMode = ""
        End Select

        'Construct the parameter
        Do While intCount <= 50
            If Derpay.CollRefundItems.Count > 0 And intCount <= Derpay.CollRefundItems.Count Then
                DEri = CType(Derpay.CollRefundItems(intCount), DERefundItem)
                'DEtid.SeatDetails1.FormattedSeat = DEri.ProductDetails
                myString = myString & Utilities.PadLeadingZeros(DEri.CustomerNo, 12) &
                                    Utilities.FixStringLength(DEri.ProductCode, 6) &
                                    Utilities.FixStringLength(DEri.PriceBand, 2) &
                                    Utilities.FixStringLength(DEri.PriceCode, 1) &
                                    Utilities.FixStringLength(DEri.CancelAllMatching, 1) &
                                    Utilities.FixStringLength("", 32)
            Else
                myString = myString & Utilities.FixStringLength("", 54)
            End If
            If True Then

            End If

            intCount = intCount + 1
        Loop
        intLen = myString.Length

        myString = myString & Utilities.FixStringLength("", 179) &
                                    Utilities.FixStringLength(Derpay.PaymentDetails.RetainCustomerReservations, 1) &
                                    Utilities.FixStringLength("", 77) &
                                    Utilities.FixStringLength("", 1) &
                                    Utilities.FixStringLength("", 15) &
                                    Utilities.FixStringLength("", 15) &
                                    "000000000000000" &
                                    Utilities.PadLeadingZeros(Derpay.PaymentReference, 15) &
                                    Utilities.FixStringLength(sCancelAll, 1) &
                                    Utilities.FixStringLength(Derpay.PaymentDetails.PaymentMode, 1) &
                                    Utilities.FixStringLength(Derpay.PaymentDetails.PaymentType, 2) &
                                    Utilities.PadLeadingZeros(Derpay.RefundCustomerNo, 12) &
                                    Utilities.FixStringLength(Derpay.PaymentDetails.CardNumber, 19) &
                                    Utilities.FixStringLength(Derpay.PaymentDetails.ExpiryDate, 4) &
                                    Utilities.FixStringLength(Derpay.PaymentDetails.StartDate, 4) &
                                    Utilities.FixStringLength(Derpay.PaymentDetails.IssueNumber, 2) &
                                    Utilities.FixStringLength(Derpay.PaymentDetails.CV2Number, 4) & "Y    " '
        'e#Mode              2958   2958    
        'e#SeatTotal         2959   2973  2 
        'e#FeeTotal          2974   2988  2 
        'e#NxtPyrf           2989   3003  0 
        'e#pyrf              3004   3018  0 
        'e#CnxAll            3019   3019    
        'e#RefundMode        3020   3020    
        'e#RefundType        3021   3022    
        'e#RefundMemb        3023   3034    
        intLen = myString.Length

        Return myString

    End Function

    ''' <summary>
    ''' AccessDatabaseWS631R is an wrapper for WS032R
    ''' </summary>
    ''' <param name="parmWS631R"></param>
    ''' <param name="paymentOnly"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseWS631R(Optional ByVal parmWS631R As String = "", Optional ByVal paymentOnly As Boolean = False) As ErrorObj

        Dim err As New ErrorObj
        If parmWS631R.Trim = "" Then
            ResultDataSet = New DataSet
        End If
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("AdditionalInfo", GetType(String))
            .Add("PaymentReference", GetType(String))
        End With

        'Create the payment Details data table
        Dim DtPaymentResults As New DataTable("PaymentResults")
        With DtPaymentResults.Columns
            .Add("PaymentReference", GetType(String))
            .Add("BatchReference", GetType(String))
            .Add("TotalPrice", GetType(String))
        End With

        'Payment table is not needed when called from the wrapper program
        If parmWS631R.Trim = "" Or paymentOnly Then
            ResultDataSet.Tables.Add(DtPaymentResults)
        End If

        Try

            'Has the parameter been passed in
            If parmWS631R.Trim <> "" Then
                PARAMOUT = parmWS631R
            Else
                'Call WS631R
                PARAMOUT = CallWS631R()
            End If

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(4999, 1) = "E" Or PARAMOUT.Substring(4997, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(4997, 2)
                dRow("Additionalinfo") = PARAMOUT.Substring(949, 30).Trim
                dRow("PaymentReference") = ""
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                dRow("AdditionalInfo") = ""
                dRow("PaymentReference") = PARAMOUT.Substring(53, 15).TrimStart("0")
            End If
            DtStatusResults.Rows.Add(dRow)

            'No errors 
            If PARAMOUT.Substring(4999, 1) <> "E" AndAlso PARAMOUT.Substring(4997, 2).Trim = "" Then

                'Create a new row
                dRow = Nothing
                dRow = DtPaymentResults.NewRow
                dRow("PaymentReference") = PARAMOUT.Substring(53, 15)
                dRow("BatchReference") = PARAMOUT.Substring(68, 9)
                dRow("TotalPrice") = Utilities.FormatPrice(PARAMOUT.Substring(77, 11))
                If PARAMOUT.Substring(51, 1) = "Y" Then
                    dRow("TotalPrice") = dRow("TotalPrice").ToString.Trim + "-"
                End If
                DtPaymentResults.Rows.Add(dRow)

            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPY-01"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS631R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS631R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5000)
        parmIO.Value = WS631RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS631R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS631R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS631RParm() As String
        If String.IsNullOrWhiteSpace(De.BasketContentType) Then De.BasketContentType = GlobalConstants.TICKETINGBASKETCONTENTTYPE
        Dim sbParam5 As New StringBuilder
        sbParam5.Append(Utilities.FixStringLength(De.SessionId, 36)) 'e5uniq  1     36
        sbParam5.Append(Utilities.FixStringLength(GetProgramMode(), 1)) 'e5mode 37     37
        sbParam5.Append(Utilities.FixStringLength(De.exPayment, 1)) 'e5exPayMode 38     38
        sbParam5.Append(Utilities.FixStringLength(GetwebServiceUser(), 10)) 'e5WebSrvUsr 39     48
        sbParam5.Append(Utilities.FixStringLength(ConvertToYN(De.GiftAid), 1)) 'e5GiftAid 49     49
        sbParam5.Append(Utilities.FixStringLength("", 1)) 'e5SingleSeat 50     50
        sbParam5.Append(Utilities.FixStringLength(GetPaymentMode(), 1)) 'e5payMode 51     51
        sbParam5.Append(Utilities.FixStringLength("", 1)) 'e5totalIsNeg 52     52
        sbParam5.Append(Utilities.FixStringLength(De.CATMode, 1)) 'e5CAT 53     53
        sbParam5.Append(Utilities.PadLeadingZeros("0", 15)) 'e5pyrf 54     68  0
        sbParam5.Append(Utilities.PadLeadingZeros("0", 9)) 'e5bref 69     77  0
        sbParam5.Append(Utilities.PadLeadingZeros("0", 11)) 'e5ordrC 78     88
        sbParam5.Append(Utilities.FixStringLength("", 10)) 'e5dept 89     98
        sbParam5.Append(Utilities.FixStringLength(GetInteractivePrint(), 1)) 'e5interactPrt 99     99
        sbParam5.Append(Utilities.FixStringLength("", 375))
        sbParam5.Append(Utilities.FixStringLength(Get3dSecureDetail(), 75)) 'e53DSecDtl 475    549
        sbParam5.Append(Utilities.PadLeadingZeros(De.Amount.Replace(".", ""), 11)) 'e5merchTotalC 550    560
        Dim winbankXID_2 As String = String.Empty
        sbParam5.Append(Utilities.FixStringLength(Get3dSecureOtherDetail(winbankXID_2), 39)) 'e5xid 561    599
        sbParam5.Append(Utilities.FixStringLength(GetExPayDetailOrDelDetail(), 298)) ' e5ExtPayDtls 600    897
        sbParam5.Append(Utilities.FixStringLength("", 1)) 'e5mem 898    898
        sbParam5.Append(Utilities.FixStringLength("", 1)) 'e5tkt 899    899
        sbParam5.Append(Utilities.FixStringLength("", 1)) 'e5sea 900    900
        sbParam5.Append(Utilities.FixStringLength("", 3)) 'e5ExtPayCCode         901    903
        sbParam5.Append(Utilities.FixStringLength("", 1)) 'e5pps 904    904
        sbParam5.Append(Utilities.FixStringLength("", 3)) '905 - 907
        sbParam5.Append(Utilities.FixStringLength(De.MarketingCampaign, 5)) 'e5mCampaign 908    912
        sbParam5.Append(Utilities.FixStringLength(De.SessionCampaignCode, 20)) 'e5Campaign 913    932
        sbParam5.Append(Utilities.FixStringLength(De.SaveMyCardMode, 1)) 'e5cardaction 933    933
        sbParam5.Append(Utilities.FixStringLength("", 13)) '                           934    946
        sbParam5.Append(Utilities.FixStringLength(ConvertToYN(De.CustomerPresent), 1)) 'e5CustPresent 947    947
        sbParam5.Append(Utilities.FixStringLength(Utilities.ConvertToYN(De.AllowManualIntervention), 1)) 'e5ManInvent 948    948
        sbParam5.Append(Utilities.FixStringLength(GetAdditionalPayTypeDetail(winbankXID_2), 76)) '949 - 1024
        sbParam5.Append(Utilities.FixStringLength("", 5)) 'e5BulkQty 1025   1029
        sbParam5.Append(Utilities.FixStringLength("", 13)) 'e5BulkID 1030   1042
        sbParam5.Append(Utilities.FixStringLength("", 2949)) 'spare 1043 3991
        sbParam5.Append(Utilities.FixStringLength(De.BasketContentType, 1)) '3992 3992
        sbParam5.Append(Utilities.FixStringLength(ConvertToYN(De.CanResetPayProcess), 1)) 'spare 3993 3993
        sbParam5.Append(Utilities.FixStringLength("", 3)) 'spare 3994 3996
        sbParam5.Append(Utilities.FixStringLength(GetPaymentDetail(), 1000)) 'e5payDetails 3997   4996 (5) 5 X 200
        sbParam5.Append(Utilities.FixStringLength(De.Source, 1)) 'e5srce 4997   4997
        sbParam5.Append(Utilities.FixStringLength("", 2)) 'e5rtcd 4998   4999
        sbParam5.Append(Utilities.FixStringLength("", 1)) 'e5erfg 5000   5000
        Return sbParam5.ToString
    End Function

    '---------------------------------------------
    ' Take Payment and Retrieve Order Details
    '---------------------------------------------
    Private Function AccessDatabaseWS608R(Optional ByVal paymentOnly As Boolean = False) As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim parm5WS631R As String = String.Empty
        Dim parm2WS608R As String = String.Empty
        Dim dRow As DataRow = Nothing

        Try

            'The wrapper program will set payment reference on a success
            DBOrderFunctions.De.PaymentReference = 0
            DBOrderFunctions.De.EndOfSale = "Y"

            'Call WS608R
            CallWS608R(parm2WS608R, parm5WS631R, paymentOnly)
            DDScheduleAdjusted = convertToBool(parm5WS631R.Substring(3990, 1))

            'No errors 
            If parm5WS631R.Substring(4999, 1) = "E" Or parm5WS631R.Substring(4997, 2).Trim <> "" Or
                paymentOnly = True Then

                SendEmail = convertToBool(parm2WS608R.Substring(3070, 1))
                OrderLevelFulfilmentMethod = parm2WS608R.Substring(3071, 1)
                'Error With Payment or in payment only mode
                AccessDatabaseWS631R(parm5WS631R, paymentOnly)
            Else
                'Process the friends and families
                DBOrderFunctions.De.Src = De.Source
                DBOrderFunctions.De.PaymentReference = parm5WS631R.Substring(53, 15)
                DBOrderFunctions.StoredProcedureGroup = Settings.StoredProcedureGroup
                DBOrderFunctions.conTALENTTKT = conTALENTTKT
                err = DBOrderFunctions.AccessDatabaseWS030R()
                ResultDataSet = DBOrderFunctions.ResultDataSet
            End If

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPY-WS608R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Sub CallWS608R(ByRef parm2WS608R As String, ByRef parm5WS631R As String, ByVal paymentOnly As Boolean)

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS608R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5)"
        Dim parm1IO1 As iDB2Parameter
        Dim parm2IOWS608R As iDB2Parameter
        Dim parm3IOWS608R As iDB2Parameter
        Dim parm4IOWS608RFee As iDB2Parameter
        Dim parm5IOWS631R As iDB2Parameter

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        parm1IO1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parm1IO1.Value = Utilities.FixStringLength("", 1024)
        parm1IO1.Direction = ParameterDirection.InputOutput

        'Populate the WS030R parameter
        parm2IOWS608R = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 3072)
        parm2IOWS608R.Value = Utilities.FixStringLength("", 3072)
        parm2IOWS608R.Direction = ParameterDirection.InputOutput

        'Populate the WS608R parameter
        parm3IOWS608R = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1024)
        parm3IOWS608R.Value = WS608Parm(paymentOnly)
        parm3IOWS608R.Direction = ParameterDirection.Input

        'Populate for WS608R Fees parameter 4
        parm4IOWS608RFee = cmdSELECT.Parameters.Add(Param4, iDB2DbType.iDB2Char, 10240)
        parm4IOWS608RFee.Value = WS608RParm4()
        parm4IOWS608RFee.Direction = ParameterDirection.InputOutput

        'Populate the WS631R parameter
        parm5IOWS631R = cmdSELECT.Parameters.Add(Param5, iDB2DbType.iDB2Char, 5000)
        parm5IOWS631R.Value = WS631RParm()
        parm5IOWS631R.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS608R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIOWS631R.Value=" & parm5IOWS631R.Value)
        cmdSELECT.ExecuteNonQuery()
        parm2WS608R = cmdSELECT.Parameters(Param2).Value.ToString
        parm5WS631R = cmdSELECT.Parameters(Param5).Value.ToString
        TalentCommonLog("CallWS608R", "", "Backend Response: parmWS631R=" & parm5WS631R & ";parm2WS608R=" & parm2WS608R)

    End Sub

    Private Function WS608RParm4() As String

        Dim sbParam4 As New StringBuilder
        Dim sbFeesString As New StringBuilder
        ' allowed 400 fees of 25 char length
        If De.BasketPaymentFeesEntityList IsNot Nothing AndAlso De.BasketPaymentFeesEntityList.Count > 0 Then
            For itemIndex As Integer = 0 To De.BasketPaymentFeesEntityList.Count - 1
                sbFeesString.Append(Utilities.FixStringLength(De.BasketPaymentFeesEntityList.Item(itemIndex).FeeCode, 6))
                sbFeesString.Append(Utilities.PadLeadingZeros(De.BasketPaymentFeesEntityList.Item(itemIndex).FeeValue.ToString.Replace(".", ""), 9))
                sbFeesString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(De.BasketPaymentFeesEntityList.Item(itemIndex).IsChargeable), 1))
                sbFeesString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(De.BasketPaymentFeesEntityList.Item(itemIndex).IsTransactional), 1))
                sbFeesString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(De.BasketPaymentFeesEntityList.Item(itemIndex).FeeValue < 0), 1))
                sbFeesString.Append(Utilities.FixStringLength("", 7))
            Next
        Else
            sbFeesString.Append(Utilities.FixStringLength("*NONE*", 6))
            sbFeesString.Append(Utilities.PadLeadingZeros("", 9))
            sbFeesString.Append(Utilities.FixStringLength("", 1))
            sbFeesString.Append(Utilities.FixStringLength("", 1))
            sbFeesString.Append(Utilities.FixStringLength("", 8))
        End If

        'fix the fee array length to 10000
        sbParam4.Append(Utilities.FixStringLength(sbFeesString.ToString, 10000))
        sbParam4.Append(Utilities.FixStringLength("", 236))
        sbParam4.Append(Utilities.FixStringLength(De.PartPaymentApplyTypeFlag.ConvertPartPaymentFlagToISeriesType, 1))
        sbParam4.Append(Utilities.PadLeadingZeros(De.FeesCount.ToString, 3))

        Return sbParam4.ToString

    End Function

    Private Function WS032RParm() As String
        Dim sbParam5 As New StringBuilder
        sbParam5.Append(Utilities.FixStringLength(De.SessionId, 36)) 'e5uniq  1     36
        sbParam5.Append(Utilities.FixStringLength(GetProgramMode(), 1)) 'e5mode 37     37
        sbParam5.Append(Utilities.FixStringLength(De.exPayment, 1)) 'e5exPayMode 38     38
        sbParam5.Append(Utilities.FixStringLength(GetwebServiceUser(), 10)) 'e5WebSrvUsr 39     48
        sbParam5.Append(Utilities.FixStringLength(ConvertToYN(De.GiftAid), 1)) 'e5GiftAid 49     49
        sbParam5.Append(Utilities.FixStringLength("", 1)) 'e5SingleSeat 50     50
        sbParam5.Append(Utilities.FixStringLength(GetPaymentMode(), 1)) 'e5payMode 51     51
        sbParam5.Append(Utilities.FixStringLength("", 1)) 'e5totalIsNeg 52     52
        sbParam5.Append(Utilities.FixStringLength(De.CATMode, 1)) 'e5CAT 53     53
        sbParam5.Append(Utilities.PadLeadingZeros("0", 15)) 'e5pyrf 54     68  0
        sbParam5.Append(Utilities.PadLeadingZeros("0", 9)) 'e5bref 69     77  0
        sbParam5.Append(Utilities.PadLeadingZeros("0", 11)) 'e5ordrC 78     88
        sbParam5.Append(Utilities.FixStringLength("", 10)) 'e5dept 89     98
        sbParam5.Append(Utilities.FixStringLength(GetInteractivePrint(), 1)) 'e5interactPrt 99     99
        sbParam5.Append(Utilities.FixStringLength("", 375))
        sbParam5.Append(Utilities.FixStringLength(Get3dSecureDetail(), 75)) 'e53DSecDtl 475    549
        sbParam5.Append(Utilities.PadLeadingZeros(De.Amount.Replace(".", ""), 11)) 'e5merchTotalC 550    560
        Dim winbankXID_2 As String = String.Empty
        sbParam5.Append(Utilities.FixStringLength(Get3dSecureOtherDetail(winbankXID_2), 39)) 'e5xid 561    599
        sbParam5.Append(Utilities.FixStringLength(GetExPayDetailOrDelDetail(), 298)) ' e5ExtPayDtls 600    897
        sbParam5.Append(Utilities.FixStringLength("", 1)) 'e5mem 898    898
        sbParam5.Append(Utilities.FixStringLength("", 1)) 'e5tkt 899    899
        sbParam5.Append(Utilities.FixStringLength("", 1)) 'e5sea 900    900
        sbParam5.Append(Utilities.FixStringLength("", 3)) 'e5ExtPayCCode         901    903
        sbParam5.Append(Utilities.FixStringLength("", 1)) 'e5pps 904    904
        sbParam5.Append(Utilities.FixStringLength("", 3)) '905 - 907
        sbParam5.Append(Utilities.FixStringLength(De.MarketingCampaign, 5)) 'e5mCampaign 908    912
        sbParam5.Append(Utilities.FixStringLength(De.SessionCampaignCode, 20)) 'e5Campaign 913    932
        sbParam5.Append(Utilities.FixStringLength(De.SaveMyCardMode, 1)) 'e5cardaction 933    933
        sbParam5.Append(Utilities.FixStringLength(De.UniqueCardId, 13)) 'e5id04 934    946
        sbParam5.Append(Utilities.FixStringLength(ConvertToYN(De.CustomerPresent), 1)) 'e5CustPresent 947    947
        sbParam5.Append(Utilities.FixStringLength(Utilities.ConvertToYN(De.AllowManualIntervention), 1)) 'e5ManInvent 948    948
        sbParam5.Append(Utilities.FixStringLength(GetAdditionalPayTypeDetail(winbankXID_2), 76)) '949 - 1024
        sbParam5.Append(Utilities.FixStringLength("", 5)) 'e5BulkQty 1025   1029
        sbParam5.Append(Utilities.FixStringLength("", 13)) 'e5BulkID 1030   1042
        sbParam5.Append(Utilities.FixStringLength("", 2954)) 'spare 1043 3996
        sbParam5.Append(Utilities.FixStringLength(GetPaymentDetail(), 1000)) 'e5payDetails 3997   4996 (5) 5 X 200
        sbParam5.Append(Utilities.FixStringLength(De.Source, 1)) 'e5srce 4997   4997
        sbParam5.Append(Utilities.FixStringLength("", 2)) 'e5rtcd 4998   4999
        sbParam5.Append(Utilities.FixStringLength("", 1)) 'e5erfg 5000   5000
        Return sbParam5.ToString
    End Function

    Private Function GetProgramMode() As String
        Dim programMode As String = "C"
        If De.exPayment = "3" Then
            programMode = "X"
        End If
        Return programMode
    End Function

    Private Function GetwebServiceUser() As String
        Dim webServiceUser As String = String.Empty
        If Not Settings.OriginatingSource.Equals(String.Empty) Then
            webServiceUser = Settings.OriginatingSource
        Else
            webServiceUser = Settings.Company
        End If
        Return webServiceUser
    End Function

    Private Function GetPaymentMode() As String
        Dim paymentMode As String = String.Empty
        Select Case De.PaymentMode
            Case Is = "Batch" : paymentMode = "B"
            Case Is = "None" : paymentMode = "N"
        End Select
        Return paymentMode
    End Function

    Private Function GetInteractivePrint() As String
        Dim interactivePrint As String = String.Empty
        'Perform interactive printing for RAS agents
        interactivePrint = "N"
        If Not String.IsNullOrEmpty(Settings.AgentType) Then
            If Settings.AgentType.Equals("2") Then
                interactivePrint = DeDelDetails.PrintOption.Trim
            End If
        End If
        Return interactivePrint
    End Function

    Private Function Get3dSecureDetail() As String
        Dim secure3DString As String = String.Empty
        If De.ThreeDSecureMode = "WINBANK" Then
            secure3DString += Utilities.FixStringLength(De.ThreeDSecureCAVV, 48) & _
            Utilities.FixStringLength(De.ThreeDSecureECI, 2) & _
            Utilities.FixStringLength(De.ThreeDSecureEnrolled, 1) & _
            Utilities.FixStringLength(De.ThreeDSecurePAResStatus, 1) & _
            Utilities.FixStringLength(De.ThreeDSecureSignatureVerification, 1) & _
            Utilities.FixStringLength(De.ThreeDSecureTransactionId, 13) & _
            Utilities.FixStringLength("", 9)

        Else
            secure3DString += Utilities.FixStringLength(De.ThreeDSecureTransactionId, 28) & _
                 Utilities.FixStringLength(De.ThreeDSecureECI, 2) & _
                 Utilities.FixStringLength(De.ThreeDSecureCAVV, 32) & _
                 Utilities.FixStringLength(De.ThreeDSecureStatus, 2)

            If De.ThreeDSecureMode = "COMMIDEA" Then
                secure3DString += Utilities.FixStringLength(De.ThreeDSecureATSData, 6) & _
                                 Utilities.FixStringLength("", 5)
            Else
                secure3DString += Utilities.FixStringLength(De.ThreeDSecureCardScheme, 2) & _
                                 Utilities.FixStringLength("", 9)
            End If
        End If
        Return secure3DString
    End Function

    Private Function Get3dSecureOtherDetail(ByRef winbankXID_2 As String) As String
        Dim secure3DOtherString As String = String.Empty
        ' XID is 40 long and we don't have anywhere to put it all in one place - split to 
        ' 39 long and 1 long.
        Dim winbankXID_1 As String = String.Empty

        If De.ThreeDSecureMode = "WINBANK" Then
            winbankXID_1 = Utilities.FixStringLength(De.ThreeDSecureXid, 39)
            If De.ThreeDSecureXid.Length > 39 Then
                winbankXID_2 = De.ThreeDSecureXid.Substring(39, 1)
            End If
            secure3DOtherString += Utilities.FixStringLength(winbankXID_1, 39)
        Else
            secure3DOtherString += Utilities.FixStringLength("", 39) '599
        End If
        Return secure3DOtherString
    End Function

    Private Function GetExPayDetailOrDelDetail() As String
        Dim miscDetail As String = String.Empty
        If DePED.ExtPaymentReference.ToString.Trim <> "" Then
            miscDetail += Utilities.FixStringLength(DePED.ExtPaymentReference, 20) & _
                        Utilities.FixStringLength(DePED.ExtPaymentName, 30) & _
                        Utilities.FixStringLength(DePED.ExtPaymentAddress1, 30) & _
                        Utilities.FixStringLength(DePED.ExtPaymentAddress2, 30) & _
                        Utilities.FixStringLength(DePED.ExtPaymentAddress3, 25) & _
                        Utilities.FixStringLength(DePED.ExtPaymentAddress4, 25) & _
                        Utilities.FixStringLength(DePED.ExtPaymentCountry, 25) & _
                        Utilities.FixStringLength(DePED.ExtPaymentPostCode, 8) & _
                        Utilities.FixStringLength(DePED.ExtPaymentTel1, 15) & _
                        Utilities.FixStringLength(DePED.ExtPaymentTel2, 15) & _
                        Utilities.FixStringLength(DePED.ExtPaymentTel3, 15) & _
                        Utilities.FixStringLength(DePED.ExtPaymentEmail, 60)
        Else
            Dim sContactName As String = Utilities.FixStringLength(DeDelDetails.ContactName, 60)
            miscDetail += Utilities.FixStringLength("", 20) & _
                        Utilities.FixStringLength(sContactName.Substring(0, 30), 30) & _
                        Utilities.FixStringLength(DeDelDetails.Address1, 30) & _
                        Utilities.FixStringLength(DeDelDetails.Address2, 30) & _
                        Utilities.FixStringLength(DeDelDetails.Address3, 25) & _
                        Utilities.FixStringLength(DeDelDetails.Address4, 25) & _
                        Utilities.FixStringLength(DeDelDetails.Country, 25) & _
                        Utilities.FixStringLength(DeDelDetails.Postcode, 8) & _
                        Utilities.FixStringLength(sContactName.Substring(30, 15), 15) & _
                        Utilities.FixStringLength(sContactName.Substring(45, 15), 15) & _
                        Utilities.FixStringLength("", 15) & _
                        Utilities.FixStringLength(DeDelDetails.AddressMoniker, 60)
        End If
        Return miscDetail
    End Function

    Private Function GetAdditionalPayTypeDetail(ByVal winbankXID_2 As String) As String
        Dim payAdditionalDetail As String = String.Empty

        If De.PaymentType = "CC" Then

            If De.ThreeDSecureMode = "WINBANK" Then

                payAdditionalDetail += Utilities.FixStringLength("", 2) & _
                           Utilities.FixStringLength(winbankXID_2, 1) & _
                           Utilities.FixStringLength(De.CardHolderName, 50) & _
                           Utilities.PadLeadingZeros(De.Installments, 2) & _
                           Utilities.FixStringLength("", 17)
            Else
                payAdditionalDetail += Utilities.FixStringLength(String.Empty, 53)
                If String.IsNullOrEmpty(De.Installments) Then
                    payAdditionalDetail += Utilities.FixStringLength(String.Empty, 2)
                Else
                    payAdditionalDetail += Utilities.PadLeadingZeros(De.Installments, 2)
                End If
                payAdditionalDetail += Utilities.FixStringLength("", 17)
            End If

        ElseIf De.PaymentType = "DD" Then

            payAdditionalDetail += Utilities.FixStringLength("", 72)                       '949-1020

        ElseIf De.PaymentType = "CF" Then

            payAdditionalDetail += Utilities.FixStringLength("", 55)
            payAdditionalDetail += Utilities.FixStringLength(De.PaymentOptionCode, 3)      'credit finance payment option code
            payAdditionalDetail += Utilities.PadLeadingZeros(De.YearsAtAddress, 3)         'credit finance number of years at this address
            payAdditionalDetail += Utilities.PadLeadingZeros(De.clubProductCodeForFinance, 3) 'credit finance club/product code 
            payAdditionalDetail += Utilities.FixStringLength("", 2)
            payAdditionalDetail += Utilities.PadLeadingZeros(De.MonthsAtAddress, 2)
            payAdditionalDetail += Utilities.FixStringLength(De.HomeStatus, 1)
            payAdditionalDetail += Utilities.FixStringLength(De.EmploymentStatus, 1)
            payAdditionalDetail += Utilities.PadLeadingZeros(De.GrossIncome, 6)
            payAdditionalDetail += Utilities.FixStringLength("", 3972)
            payAdditionalDetail += Utilities.FixStringLength(De.Source, 1)

        Else
            payAdditionalDetail += Utilities.FixStringLength("", 72)                       '949-1020
        End If
        Return payAdditionalDetail
    End Function

    Private Function GetPaymentDetail() As String
        'can send 5 card detail 5 x 200 = 1000 (3997 - 4996)
        'todo check with Pete for structure
        Dim payCardDetails As String = String.Empty
        'first card detail will be the payment card and then pps1, pps2
        payCardDetails += GetPaymentDetail(De) '3997 - 4196  = 200
        'other 4 card details
        If PayDetails IsNot Nothing AndAlso PayDetails.Count > 0 Then
            payCardDetails += GetPaymentDetailByKey(GlobalConstants.PPS1STAGE) '4197 - 4396 
            payCardDetails += GetPaymentDetailByKey(GlobalConstants.PPS2STAGE) '4397 - 4596
            payCardDetails += Utilities.FixStringLength("", 400) '4597 - 4996
        Else
            payCardDetails += Utilities.FixStringLength("", 800) '4197 - 4996
        End If
        payCardDetails = Utilities.FixStringLength(payCardDetails, 1000)
        Return payCardDetails
    End Function

    Private Function GetPaymentDetailByKey(ByVal keyName As String) As String
        Dim payCardDetails As String = String.Empty
        If PayDetails.ContainsKey(UCase(keyName)) Then
            payCardDetails += GetPaymentDetail(PayDetails.Item(UCase(keyName)))
        Else
            payCardDetails += Utilities.FixStringLength("", 200)
        End If
        Return Utilities.FixStringLength(payCardDetails, 200)
    End Function

    Private Function GetPaymentDetail(ByVal pay As DEPayments) As String

        Dim parm As New StringBuilder

        Select Case pay.PaymentType
            Case Is = GlobalConstants.CCPAYMENTTYPE
                parm.Append(GlobalConstants.CCPAYMENTTYPE)
                parm.Append(Utilities.FixStringLength(pay.CardNumber, 19))
                parm.Append(Utilities.PadLeadingZeros(pay.ExpiryDate, 4))
                parm.Append(Utilities.PadLeadingZeros(pay.StartDate, 4))
                parm.Append(Utilities.FixStringLength(pay.IssueNumber, 2))
                parm.Append(Utilities.FixStringLength(pay.CV2Number, 4))
                parm.Append(Utilities.FixStringLength("", 30)) 'card holder name
                parm.Append(Utilities.FixStringLength(pay.TokenID, 18))
                parm.Append(Utilities.FixStringLength(pay.ProcessingDB, 15))
                parm.Append(Utilities.FixStringLength(pay.TransactionID, 15))
                parm.Append(Utilities.FixStringLength("", 10)) 'card type
                parm.Append(Utilities.PadLeadingZeros(pay.UniqueCardId, 13))
                parm.Append(Utilities.FixStringLength("", 64))
            Case Is = GlobalConstants.PAYMENTTYPE_VANGUARD
                parm.Append(GlobalConstants.PAYMENTTYPE_VANGUARD)
                parm.Append(Utilities.FixStringLength(pay.CardNumber, 19))
                parm.Append(Utilities.PadLeadingZeros(pay.ExpiryDate, 4))
                parm.Append(Utilities.PadLeadingZeros(pay.StartDate, 4))
                parm.Append(Utilities.FixStringLength(pay.IssueNumber, 2))
                parm.Append(Utilities.FixStringLength(pay.CV2Number, 4))
                parm.Append(Utilities.FixStringLength("", 30)) 'card holder name
                parm.Append(Utilities.FixStringLength(pay.TokenID, 18))
                parm.Append(Utilities.FixStringLength(pay.ProcessingDB, 15))
                parm.Append(Utilities.FixStringLength(pay.TransactionID, 15))
                parm.Append(Utilities.FixStringLength(pay.CardType, 10))
                parm.Append(Utilities.FixStringLength("", 77))
            Case Is = GlobalConstants.DDPAYMENTTYPE
                parm.Append(GlobalConstants.DDPAYMENTTYPE)
                parm.Append(Utilities.FixStringLength(pay.AccountName, 40))
                parm.Append(Utilities.PadLeadingZeros(pay.SortCode, 6))
                parm.Append(Utilities.PadLeadingZeros(pay.AccountNumber, 8))
                parm.Append(Utilities.PadLeadingZeros(pay.DDIReference, 10))
                parm.Append(Utilities.PadLeadingZeros(pay.PaymentDay, 2))
                parm.Append(Utilities.FixStringLength("", 132))

            Case Is = GlobalConstants.CQPAYMENTTYPE
                If (pay.ChequeAccount Is Nothing OrElse pay.ChequeAccount Is String.Empty) AndAlso (pay.ChequeNumber Is Nothing OrElse pay.ChequeNumber Is String.Empty) Then
                    If pay.OptionTokencode.Contains("/") Then
                        Dim div As Integer = pay.OptionTokencode.IndexOf("/")
                        pay.ChequeNumber = pay.OptionTokencode.Substring(0, div)
                        pay.ChequeAccount = pay.OptionTokencode.Substring((div + 1), (pay.OptionTokencode.Length - 1 - div))
                    End If
                End If
                parm.Append(GlobalConstants.CQPAYMENTTYPE)
                parm.Append(Utilities.FixStringLength(pay.ChequeNumber, 10))
                parm.Append(Utilities.FixStringLength(pay.ChequeAccount, 8))
                parm.Append(Utilities.FixStringLength(pay.ExpiryDate, 8))
                parm.Append(Utilities.FixStringLength("", 172))

            Case Is = GlobalConstants.CFPAYMENTTYPE
                parm.Append(GlobalConstants.CFPAYMENTTYPE)
                parm.Append(Utilities.FixStringLength(pay.AccountName, 40))
                parm.Append(Utilities.PadLeadingZeros(pay.SortCode, 6))
                parm.Append(Utilities.PadLeadingZeros(pay.AccountNumber, 8))
                parm.Append(Utilities.PadLeadingZeros("", 10))
                parm.Append(Utilities.PadLeadingZeros("", 2))
                parm.Append(Utilities.FixStringLength("", 132))

            Case Is = GlobalConstants.EPURSEPAYMENTTYPE
                parm.Append(GlobalConstants.EPURSEPAYMENTTYPE)
                If Not De.IsGiftCard Then
                    parm.Append(Utilities.FixStringLength(pay.CardNumber, 30))
                    parm.Append(Utilities.FixStringLength(pay.PIN, 4))
                    parm.Append(Utilities.FixStringLength(pay.Currency, 3))
                    parm.Append(Utilities.FixStringLength("", 161))
                Else
                    parm.Append(Utilities.FixStringLength(pay.CardNumber, 14))
                    parm.Append(Utilities.FixStringLength("", 184))
                End If
            Case Is = GlobalConstants.PAYPALPAYMENTTYPE
                parm.Append(GlobalConstants.PAYPALPAYMENTTYPE)
                parm.Append(Utilities.FixStringLength(pay.CardNumber, 19))
                parm.Append(Utilities.FixStringLength("", 179))

            Case Is = GlobalConstants.CHIPANDPINPAYMENTTYPE
                parm.Append(GlobalConstants.CHIPANDPINPAYMENTTYPE)
                parm.Append(Utilities.FixStringLength(pay.ChipAndPinIPAddress, 15))
                parm.Append(Utilities.FixStringLength("", 183))

            Case Is = GlobalConstants.POINTOFSALEPAYMENTTYPE
                parm.Append(GlobalConstants.POINTOFSALEPAYMENTTYPE)
                parm.Append(Utilities.FixStringLength(pay.PointOfSaleIPAddress, 15))
                parm.Append(Utilities.FixStringLength(pay.PointOfSaleTCPPort, 5))
                parm.Append(Utilities.FixStringLength("", 178))

            Case Else
                parm.Append(Utilities.FixStringLength(De.PaymentType, 2))
                parm.Append(Utilities.FixStringLength(pay.OptionTokencode, 30))
                parm.Append(Utilities.FixStringLength("", 168))

        End Select
        Return parm.ToString()
    End Function

    Private Function AccessDatabaseWS094R(Optional ByVal parmWS094R As String = "") As ErrorObj

        Dim err As New ErrorObj
        If parmWS094R.Trim = "" Then
            ResultDataSet = New DataSet
        End If
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("AdditionalInfo", GetType(String))
        End With

        'Create the payment Details data table
        Dim DtPaymentResults As New DataTable("PaymentResults")
        With DtPaymentResults.Columns
            .Add("PaymentReference", GetType(String))
            .Add("BatchReference", GetType(String))
            .Add("TotalPrice", GetType(String))
        End With

        'Payment table is not needed when called from the wrapper program
        If parmWS094R.Trim = "" Then
            ResultDataSet.Tables.Add(DtPaymentResults)
        End If

        Try

            'Has the parameter been passed in
            If parmWS094R.Trim <> "" Then
                PARAMOUT = parmWS094R
            Else
                'Call WS094R
                PARAMOUT = CallWS094R()
            End If

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                dRow("Additionalinfo") = PARAMOUT.Substring(949, 30).Trim
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                dRow("AdditionalInfo") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            'No errors 
            If PARAMOUT.Substring(1023, 1) <> "E" And PARAMOUT.Substring(1021, 2).Trim = "" And parmWS094R.Trim = "" Then

                'Create a new row
                dRow = Nothing
                dRow = DtPaymentResults.NewRow
                'dRow("PaymentReference") = PARAMOUT.Substring(53, 15)
                'dRow("BatchReference") = PARAMOUT.Substring(68, 9)
                'dRow("TotalPrice") = Utilities.FormatPrice(PARAMOUT.Substring(77, 11))
                dRow("PaymentReference") = ""
                dRow("BatchReference") = ""
                dRow("TotalPrice") = ""
                DtPaymentResults.Rows.Add(dRow)

            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPY-03"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS094R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS094R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS094Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Populate the return parameter
        parmO = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmO.Value = ""
        parmO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS094R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS094R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS094Parm() As String

        Dim myString As String = String.Empty

        Dim parmAmount As String = String.Empty
        parmAmount = (CDec(De.Amount * 100)).ToString

        'Construct the parameter
        myString = parmAmount.PadLeft(11, "0") & _
                 Utilities.FixStringLength(De.CardNumber, 20) & _
                 Utilities.PadLeadingZeros(De.ExpiryDate, 4) & _
                 Utilities.PadLeadingZeros(De.StartDate, 4) & _
                 Utilities.FixStringLength(De.IssueNumber, 2) & _
                 Utilities.FixStringLength(De.CV2Number, 4) & _
                 Utilities.FixStringLength(De.CustomerNumber, 12) & _
                 Utilities.FixStringLength("", 963) & _
                 Utilities.FixStringLength(De.Source, 1)

        Return myString

    End Function

    Private Function AccessDatabaseWS084R() As ErrorObj

        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the direct debit data table
        Dim DtDirectDebitResults As New DataTable("DirectDebitResults")
        Select Case _settings.ModuleName
            Case Is = DirectDebitDDIRef
                With DtDirectDebitResults.Columns
                    .Add("DirectDebitDDIRef", GetType(String))
                    .Add("Originator", GetType(String))
                End With
            Case Is = DirectDebitPaymentDays
                With DtDirectDebitResults.Columns
                    .Add("Originator", GetType(String))
                    .Add("PaymentDays", GetType(String))
                End With
        End Select

        Try

            'Call WS094R
            PARAMOUT = CallWS084R()

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            'No errors 
            If PARAMOUT.Substring(1023, 1) <> "E" And PARAMOUT.Substring(1021, 2).Trim = "" Then

                'Payment table is not needed when called from the wrapper program
                ResultDataSet.Tables.Add(DtDirectDebitResults)

                Select Case _settings.ModuleName
                    Case Is = DirectDebitDDIRef
                        'Create a new row
                        dRow = Nothing
                        dRow = DtDirectDebitResults.NewRow
                        dRow("DirectDebitDDIRef") = PARAMOUT.Substring(45, 10)
                        dRow("Originator") = PARAMOUT.Substring(39, 6)
                        DtDirectDebitResults.Rows.Add(dRow)
                    Case Is = DirectDebitPaymentDays
                        'Create a new row
                        dRow = Nothing
                        dRow = DtDirectDebitResults.NewRow
                        dRow("Originator") = PARAMOUT.Substring(39, 6)
                        dRow("PaymentDays") = PARAMOUT.Substring(55, 32)
                        DtDirectDebitResults.Rows.Add(dRow)
                End Select

            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPY-04"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS084R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS084R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS084Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS084R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS084R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS084Parm() As String

        Dim myString As String = String.Empty

        Dim stage As String = String.Empty
        Select Case UCase(De.PaymentStage)
            Case Is = UCase("checkoutPPS1Details.aspx") : stage = "01"
            Case Is = UCase("checkoutPPS2Details.aspx") : stage = "02"
            Case Is = UCase("checkoutDirectDebitDetails.aspx") : stage = "03"
        End Select

        Dim mode As String = String.Empty
        Select Case _settings.ModuleName
            Case Is = DirectDebitDDIRef : mode = "1"
            Case Is = DirectDebitPaymentDays : mode = "3"
        End Select

        'Construct the parameter
        myString = Utilities.FixStringLength(De.SessionId, 36) & _
                 Utilities.FixStringLength(mode, 1) & _
                 Utilities.FixStringLength(stage, 2) & _
                 Utilities.PadLeadingZeros("0", 16) & _
                 Utilities.FixStringLength("", 925)

        If De.ProductCodeForDD <> "" Then
            myString = myString & Utilities.FixStringLength(De.ProductCodeForDD, 6)
        Else
            myString = myString & Utilities.FixStringLength("", 6)
        End If

        myString = myString & Utilities.PadLeadingZeros("0", 15) & _
                        Utilities.FixStringLength("", 1) & _
                        Utilities.FixStringLength(De.CustomerNumber, 12) & _
                        Utilities.FixStringLength("", 6) & _
                        Utilities.FixStringLength(De.Source, 1) & _
                        Utilities.FixStringLength("", 3)

        Return myString

    End Function

    Private Function WS608Parm(ByVal paymentOnly As Boolean) As String

        Dim myString As String = String.Empty

        If De.AutoEnrol Then
            myString = "Y"
        Else
            myString = "N"
        End If

        If paymentOnly Then
            myString += "Y"
        Else
            myString += "N"
        End If

        'Construct the parameter
        myString += Utilities.FixStringLength("", 1018) & _
                 Utilities.FixStringLength(De.Source, 1) & _
                 Utilities.FixStringLength("", 3)

        Return myString

    End Function

    Private Function AccessDatabaseWS086R() As ErrorObj

        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("TotalAmount", GetType(String))
            .Add("TotalAmountPaid", GetType(String))
            .Add("ScheduledEntries", GetType(String))
            .Add("DirectDebitDDIRef", GetType(String))
            .Add("AccountName", GetType(String))
            .Add("AccountNumber", GetType(String))
            .Add("SortCode", GetType(String))
        End With

        'Create the direct debit summary header data table
        Dim DtDirectDebitSummary As New DataTable("DirectDebitSummary")
        With DtDirectDebitSummary.Columns
            .Add("PaymentDate", GetType(String))
            .Add("PaymentAmount", GetType(String))
            .Add("ScheduledPaymentStatus", GetType(String))
            .Add("ScheduledPaymentStatusDescription", GetType(String))
        End With

        Try

            'Call WS086R
            PARAMOUT = CallWS086R()

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                dRow("TotalAmount") = ""
                dRow("TotalAmountPaid") = ""
                dRow("ScheduledEntries") = ""
                dRow("DirectDebitDDIRef") = ""
                dRow("AccountName") = ""
                dRow("AccountNumber") = ""
                dRow("SortCode") = ""
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                dRow("TotalAmount") = Utilities.FormatPrice(PARAMOUT.Substring(79, 15))
                dRow("TotalAmountPaid") = Utilities.FormatPrice(PARAMOUT.Substring(987, 15))
                dRow("ScheduledEntries") = PARAMOUT.Substring(94, 2)
                dRow("DirectDebitDDIRef") = PARAMOUT.Substring(15, 10).ToString.TrimStart("0")
                dRow("AccountName") = PARAMOUT.Substring(39, 40)
                dRow("AccountNumber") = PARAMOUT.Substring(25, 8)
                dRow("SortCode") = PARAMOUT.Substring(33, 6)
            End If
            DtStatusResults.Rows.Add(dRow)

            'No errors 
            If PARAMOUT.Substring(1023, 1) <> "E" And PARAMOUT.Substring(1021, 2).Trim = "" Then

                'Add the direct debit summary table
                ResultDataSet.Tables.Add(DtDirectDebitSummary)

                'Populate the summary table
                Dim loopIndex As Integer = 0
                Dim startDateIndex As Integer = 96
                Dim startPayIndex As Integer = 192
                Dim startscheduleStatusIndex As Integer = 372
                Dim startscheduleStatusDescriptionIndex As Integer = 384
                For loopIndex = 0 To CType(PARAMOUT.Substring(94, 2), Integer) - 1

                    'Populate the row
                    dRow = Nothing
                    dRow = DtDirectDebitSummary.NewRow
                    dRow("PaymentDate") = PARAMOUT.Substring(startDateIndex, 8)
                    dRow("PaymentAmount") = Utilities.FormatPrice(PARAMOUT.Substring(startPayIndex, 15))
                    dRow("ScheduledPaymentStatus") = PARAMOUT.Substring(startscheduleStatusIndex, 1)
                    dRow("ScheduledPaymentStatusDescription") = PARAMOUT.Substring(startscheduleStatusDescriptionIndex, 20).Trim()
                    DtDirectDebitSummary.Rows.Add(dRow)

                    'Increment and go again
                    startDateIndex += 8
                    startPayIndex += 15
                    startscheduleStatusIndex += 1
                    startscheduleStatusDescriptionIndex += 20
                Next

            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPY-04"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS086R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS086R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS086Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS086R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS086R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS086Parm() As String

        Dim myString As String = String.Empty

        'Construct the parameter
        myString = Utilities.PadLeadingZeros(De.PaymentRef, 15) & _
                 Utilities.PadLeadingZeros("0", 10) & _
                 Utilities.PadLeadingZeros("0", 8) & _
                 Utilities.PadLeadingZeros("0", 6) & _
                 Utilities.FixStringLength("", 40) & _
                 Utilities.PadLeadingZeros("0", 17) & _
                 Utilities.FixStringLength("", 96) & _
                 Utilities.PadLeadingZeros("0", 180) & _
                 Utilities.FixStringLength("", 630) & _
                 Utilities.FixStringLength(De.CustomerNumber, 12) & _
                 Utilities.FixStringLength("", 6) & _
                 Utilities.FixStringLength(De.Source, 1) & _
                 Utilities.FixStringLength("", 3)

        Return myString

    End Function

    Private Function AccessDatabaseWS097R() As ErrorObj

        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("GeneratedID", GetType(String))
        End With

        Try

            'Call WS097R
            PARAMOUT = CallWS097R()

            'Set the response data
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                dRow("GeneratedID") = ""
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                dRow("GeneratedID") = PARAMOUT.Substring(1007, 13)

            End If
            DtStatusResults.Rows.Add(dRow)


        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPY-05"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS097R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS097R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS097Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS097R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS097R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS097Parm() As String

        Dim myString As String = String.Empty

        'Construct the parameter
        myString = Utilities.PadLeadingZeros(De.GenerateTransactionFileID, 5) & _
                 Utilities.FixStringLength(" ", 1002) & _
                 Utilities.FixStringLength(" ", 13) & _
                 Utilities.FixStringLength(De.Source, 1) & _
                 Utilities.FixStringLength("", 3)

        Return myString

    End Function

    Private Function AccessDatabaseWS057R() As ErrorObj
        Dim bMoreRecords As Boolean = True
        Dim lastCustomer As String = String.Empty
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the results data table
        Dim DtAvailableCashBackResults As New DataTable("AvailableCashBackResults")
        ResultDataSet.Tables.Add(DtAvailableCashBackResults)
        With DtAvailableCashBackResults.Columns
            .Add("CustomerNumber", GetType(String))
            .Add("TotalReward", GetType(Decimal))
            .Add("AvailableReward", GetType(Decimal))
            .Add("RewardSelected", GetType(Boolean))
            .Add("OnAccountEnabled", GetType(Boolean))
        End With

        Try
            Do While bMoreRecords Or _moreThan40CashbackCustomers
                PARAMOUT = CallWS057R(lastCustomer)

                'Set the response data
                If PARAMOUT.Substring(5119, 1) = "E" Or PARAMOUT.Substring(5117, 2).Trim <> "" Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(5117, 2)
                    DtStatusResults.Rows.Add(dRow)
                    bMoreRecords = False
                Else
                    If De.CashbackMode.Equals("R") Or De.CashbackMode.Equals("E") Then
                        Dim iPosition As Integer = 0
                        Do While PARAMOUT.Substring(iPosition, 1).Trim <> String.Empty
                            dRow = Nothing
                            dRow = DtAvailableCashBackResults.NewRow
                            dRow("CustomerNumber") = PARAMOUT.Substring(iPosition, 12)
                            dRow("TotalReward") = Convert.ToDecimal(PARAMOUT.Substring(iPosition + 12, 7) & "." & PARAMOUT.Substring(iPosition + 19, 2))
                            dRow("AvailableReward") = Convert.ToDecimal(PARAMOUT.Substring(iPosition + 21, 7) & "." & PARAMOUT.Substring(iPosition + 28, 2))
                            dRow("RewardSelected") = convertToBool(PARAMOUT.Substring(iPosition + 30, 1))
                            dRow("OnAccountEnabled") = Utilities.convertToBool(PARAMOUT.Substring(5026, 1))
                            ' dRow("OnAccountEnabled") = False
                            DtAvailableCashBackResults.Rows.Add(dRow)
                            iPosition = iPosition + 100
                        Loop
                    End If
                    bMoreRecords = Utilities.convertToBool(PARAMOUT.Substring(5115, 1))
                    If bMoreRecords Then
                        lastCustomer = PARAMOUT.Substring(5103, 12)
                    End If
                End If
            Loop
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPY-06"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS057R(ByVal lastCustomer As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS057R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS057Parm(lastCustomer)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS057R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS057R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS057Parm(ByVal lastCustomer As String) As String

        Dim myString As String = String.Empty

        'Construct the parameter
        If De.CashbackMode.Equals("R") Then
            myString = Utilities.FixStringLength(" ", 5017) & _
                                Utilities.PadLeadingZeros(De.Amount.Replace(".", ""), 9) & _
                                Utilities.FixStringLength(" ", 1) & _
                                Utilities.FixStringLength(De.CustomerNumber, 12) & _
                                Utilities.FixStringLength(" ", 27) & _
                                Utilities.FixStringLength(De.SessionId, 36) & _
                 Utilities.FixStringLength(De.CashbackMode, 1) & _
                 Utilities.FixStringLength(lastCustomer, 12) & _
                 Utilities.FixStringLength(" ", 1) & _
                 Utilities.FixStringLength(De.Source, 1)

        ElseIf De.CashbackMode.Equals("E") Then
            myString = Utilities.FixStringLength(" ", 5017) & _
                                Utilities.PadLeadingZeros(De.Amount.Replace(".", ""), 9) & _
                                Utilities.FixStringLength(" ", 1) & _
                                Utilities.FixStringLength(De.CustomerNumber, 12) & _
                                Utilities.FixStringLength(" ", 27) & _
                                Utilities.FixStringLength(De.SessionId, 36) & _
                 Utilities.FixStringLength(De.CashbackMode, 1) & _
                 Utilities.FixStringLength(lastCustomer, 12) & _
                 Utilities.FixStringLength(" ", 1) & _
                 Utilities.FixStringLength(De.Source, 1)

        ElseIf De.CashbackMode.Equals("D") Then
            myString = Utilities.FixStringLength(" ", 5066)
            myString += Utilities.FixStringLength(De.SessionId, 36) & _
                 Utilities.FixStringLength(De.CashbackMode, 1) & _
                 Utilities.FixStringLength(lastCustomer, 12) & _
                 Utilities.FixStringLength(" ", 1) & _
                 Utilities.FixStringLength(De.Source, 1)

        ElseIf De.CashbackMode.Equals("F") Then
            If _moreThan40CashbackCustomers Then
                Dim i As Integer = 1
                While i <= 40
                    ' 40 is the number of customers in one go that WS057R can take
                    If De.CashbackCustomersSelected.Count >= _startNextCashbackCustomer Then
                        myString += Utilities.FixStringLength(De.CashbackCustomersSelected(i), 12) & _
                                    Utilities.PadLeadingZeros(De.TotalRewardSelected(i), 9) & _
                                    Utilities.PadLeadingZeros(De.AvailableRewardSelected(i), 9) & _
                                    Utilities.FixStringLength("1", 1) & _
                                    Utilities.FixStringLength(" ", 69)
                    Else
                        Dim numberOfCharsToPad As Integer = 0
                        numberOfCharsToPad = 41 - i
                        numberOfCharsToPad = numberOfCharsToPad * 100
                        myString += Utilities.FixStringLength(" ", numberOfCharsToPad)
                        Exit While
                    End If
                    i += 1
                    _startNextCashbackCustomer += 1
                End While
                If De.CashbackCustomersSelected.Count >= _startNextCashbackCustomer Then
                    _moreThan40CashbackCustomers = True
                Else
                    _moreThan40CashbackCustomers = False
                End If
            Else
                Dim i As Integer = 1
                While i <= 40
                    ' 40 is the number of customers in one go that WS057R can take
                    If De.CashbackCustomersSelected.Count >= i Then
                        If De.CashbackCustomersSelected.Count > 40 Then
                            _moreThan40CashbackCustomers = True
                        End If

                        Dim strAvailableRewardSelected As String = (CInt(De.AvailableRewardSelected(i))).ToString
                        '  strAvailableRewardSelected = Utilities.PadLeadingZeros(strAvailableRewardSelected, 9)
                        myString += Utilities.FixStringLength(De.CashbackCustomersSelected(i), 12) & _
                                    Utilities.PadLeadingZeros(De.TotalRewardSelected(i), 9) & _
                                    Utilities.PadLeadingZeros(strAvailableRewardSelected, 9) & _
                                    Utilities.FixStringLength("1", 1) & _
                                    Utilities.FixStringLength(" ", 69)
                    Else
                        Dim numberOfCharsToPad As Integer = 0
                        numberOfCharsToPad = 41 - i
                        numberOfCharsToPad = numberOfCharsToPad * 100
                        myString += Utilities.FixStringLength(" ", numberOfCharsToPad)
                        Exit While
                    End If
                    i += 1
                    _startNextCashbackCustomer = i
                End While
            End If
            myString += Utilities.FixStringLength(" ", 1017) & _
                        Utilities.PadLeadingZeros(De.Amount.Replace(".", ""), 9) & _
                        Utilities.FixStringLength(" ", 1) & _
                        Utilities.FixStringLength(De.CustomerNumber, 12) & _
                        Utilities.FixStringLength(" ", 27) & _
                        Utilities.FixStringLength(De.SessionId, 36) & _
                 Utilities.FixStringLength(De.CashbackMode, 1) & _
                 Utilities.FixStringLength(lastCustomer, 12) & _
                 Utilities.FixStringLength(" ", 1) & _
                 Utilities.FixStringLength(De.Source, 1)

        ElseIf De.CashbackMode.Equals("A") Then
            Dim sb57Parm As New StringBuilder
            sb57Parm.Append(Utilities.FixStringLength(De.CustomerNumber, 12))
            sb57Parm.Append(Utilities.PadLeadingZeros(De.Amount.Replace(".", ""), 9))
            sb57Parm.Append(Utilities.FixStringLength("", 3993))
            sb57Parm.Append(Utilities.PadLeadingZeros(De.UniqueCardId, 13))
            sb57Parm.Append(Utilities.FixStringLength("Y", 1))
            sb57Parm.Append(Utilities.FixStringLength(De.AdjustmentType, 1))
            sb57Parm.Append(Utilities.FixStringLength(De.Reason, 50))
            sb57Parm.Append(Utilities.PadLeadingZeros(De.ActivationDate, 7))
            sb57Parm.Append(Utilities.FixStringLength(De.ExpiryOption, 1))
            sb57Parm.Append(Utilities.PadLeadingZeros(De.ExpiryDate, 7))
            sb57Parm.Append(Utilities.FixStringLength(De.ConfirmCarryOverAllowance, 1))
            sb57Parm.Append(Utilities.FixStringLength("", 971))
            sb57Parm.Append(Utilities.FixStringLength(De.SessionId, 36))
            sb57Parm.Append(Utilities.FixStringLength(De.CashbackMode, 1))
            sb57Parm.Append(Utilities.FixStringLength("", 13))
            sb57Parm.Append(Utilities.FixStringLength(De.Source, 1))
            sb57Parm.Append(Utilities.FixStringLength("", 3))
            myString = sb57Parm.ToString()
        End If

        Return myString

    End Function

    Private Function AccessDatabaseWS012R() As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(dtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the results data table
        Dim dtPaymentChargesResults As New DataTable("PaymentCharges")
        ResultDataSet.Tables.Add(dtPaymentChargesResults)
        With dtPaymentChargesResults.Columns
            .Add("FeeCode", GetType(String))
            .Add("FeePrice", GetType(Decimal))
            .Add("CustomerNumber", GetType(String))
        End With

        Try
            PARAMOUT = CallWS012R()

            'Set the response data
            If PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                dRow = Nothing
                dRow = dtStatusResults.NewRow
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1022, 2)
                dtStatusResults.Rows.Add(dRow)
            Else
                dRow = Nothing
                dRow = dtPaymentChargesResults.NewRow
                dRow("FeeCode") = PARAMOUT.Substring(118, 6).Trim
                If De.FeeMode = "A" Then
                    dRow("FeePrice") = Convert.ToDecimal(PARAMOUT.Substring(124, 7) & "." & PARAMOUT.Substring(131, 2))
                Else
                    dRow("FeePrice") = 0
                End If
                dRow("CustomerNumber") = PARAMOUT.Substring(1003, 12)
                dtPaymentChargesResults.Rows.Add(dRow)
            End If
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPY-07"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS012R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS012R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS012Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS012R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS012R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS012Parm() As String

        Dim myString As String = String.Empty

        'Construct the parameter
        myString += Utilities.FixStringLength("", 118) & _
                 Utilities.FixStringLength(De.FeeCode, 6) & _
                 Utilities.FixStringLength("", 9) & _
                 Utilities.FixStringLength(De.FeeMode, 1) & _
                 Utilities.FixStringLength("", 822) & _
                 Utilities.FixStringLength(De.SessionId, 36) & _
                 Utilities.FixStringLength("", 10) & _
                 Utilities.FixStringLength(De.CustomerNumber, 12) & _
                 Utilities.FixStringLength("", 6) & _
                 Utilities.FixStringLength(De.Source, 1)

        Return myString

    End Function

    Private Function AccessDatabaseWS173R() As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the results data table
        Dim dtAdhocFees As New DataTable("AdhocFees")
        ResultDataSet.Tables.Add(dtAdhocFees)
        With dtAdhocFees.Columns
            .Add("FeeCode", GetType(String))
            .Add("FeeDescription", GetType(String))
            .Add("FeePrice", GetType(Decimal))
        End With

        Try
            PARAMOUT = CallWS173R()

            'Set the response data
            If PARAMOUT.Substring(5119, 1) = "E" Or PARAMOUT.Substring(5117, 2).Trim <> "" Then
                dRow = Nothing
                dRow = dtStatusResults.NewRow
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(5118, 2)
                dtStatusResults.Rows.Add(dRow)
            Else
                Dim iPosition As Integer = 0
                Do While PARAMOUT.Substring(iPosition, 1).Trim <> String.Empty
                    dRow = Nothing
                    dRow = dtAdhocFees.NewRow
                    dRow("FeeCode") = PARAMOUT.Substring(iPosition, 6).Trim
                    dRow("FeeDescription") = PARAMOUT.Substring(iPosition + 6, 50).Trim
                    dRow("FeePrice") = Convert.ToDecimal(PARAMOUT.Substring(iPosition + 56, 7) & "." & PARAMOUT.Substring(iPosition + 63, 2))
                    dtAdhocFees.Rows.Add(dRow)
                    iPosition = iPosition + 65
                Loop
            End If
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPY-08"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS173R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS173R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS173Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS173R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS173R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS173Parm() As String

        Dim myString As String = String.Empty

        'Construct the parameter
        myString += Utilities.FixStringLength("", 120)
        myString += Utilities.FixStringLength(De.Source, 1)

        Return myString

    End Function

    Private Function AccessDatabaseWS060R() As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        ResultDataSet = New DataSet
        Dim dtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        _saveCardFunctions.CreateCardDetailsTable(ResultDataSet)

        Try
            PARAMOUT = CallWS060R()
            dRow = Nothing
            dRow = ResultDataSet.Tables(0).NewRow
            If PARAMOUT.Substring(1023, 1) = "E" Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                ResultDataSet.Tables(0).Rows.Add(dRow)
            Else
                dRow("ErrorOccurred") = String.Empty
                dRow("ReturnCode") = String.Empty
                ResultDataSet.Tables(0).Rows.Add(dRow)
                _saveCardFunctions.PopulateCardDetails(ResultDataSet, PARAMOUT, 97)
            End If
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBPayment.vb-AccessDatabaseWS060R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function AccessDatabaseWS067R() As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        ResultDataSet = New DataSet
        Dim LastRecord As String = "000"
        Dim RecordTotal As String = "000"
        Dim MoreRecords As Boolean = True
        Dim dtStatusResults As New DataTable("dtStatusResults")
        Dim dtOnAccountDetails As New DataTable("OnAccountDetails")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("TotalBalance", GetType(String))
            .Add("RefundableBalance", GetType(String))
        End With
        '  _saveCardFunctions.CreateCardDetailsTable(ResultDataSet)

        Try

            Do While MoreRecords

                PARAMOUT = CallWS067R(PARAMOUT2, LastRecord)
                dRow = Nothing
                dRow = ResultDataSet.Tables(0).NewRow
                'Check for errors
                If PARAMOUT.Substring(5119, 1) = "E" Or PARAMOUT.Substring(5117, 2).Trim <> "" Then
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(5117, 2)
                    ResultDataSet.Tables(0).Rows.Add(dRow)
                Else
                    dRow("ErrorOccurred") = String.Empty
                    dRow("ReturnCode") = String.Empty
                    'ResultDataSet.Tables(0).Rows.Add(dRow)
                End If
                'Need to create table if doesn't already exist
                If Not ResultDataSet.Tables.Contains("OnAccountDetails") Then
                    ResultDataSet.Tables.Add(dtOnAccountDetails)
                    With dtOnAccountDetails.Columns
                        .Add("Date", GetType(String))
                        .Add("Amount", GetType(String))
                        .Add("Sign", GetType(String))
                        .Add("product", GetType(String))
                        .Add("ProductDescription", GetType(String))
                        .Add("PaymentReference", GetType(String))
                        .Add("ActivtyType", GetType(String))
                        .Add("RunningBalance", GetType(String))
                        .Add("RunningBalanceSign", GetType(String))
                        .Add("Seat", GetType(String))
                        .Add("RefundFrom", GetType(String))
                    End With
                End If

                'Create the results data table
                If LastRecord = "000" Then
                    dRow = Nothing
                    dRow = dtStatusResults.NewRow
                    If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                        dRow("TotalBalance") = Talent.Common.Utilities.CheckForDBNull_Decimal(PARAMOUT.Substring(48, 9).Trim) / 100
                        If (PARAMOUT.Substring(76, 1) = "N") Then
                            dRow("TotalBalance") = dRow("TotalBalance") * -1
                        End If
                        dRow("RefundableBalance") = Talent.Common.Utilities.CheckForDBNull_Decimal(PARAMOUT.Substring(57, 9).Trim) / 100
                    End If
                    dtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(3071, 1) <> "E" AndAlso PARAMOUT.Substring(3069, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim Position As Integer = 0
                    Dim Counter As Integer = 0
                    Dim Sign As String

                    Do While Counter < 120

                        'Create a new row
                        If Not String.IsNullOrEmpty(PARAMOUT2.Substring(Position, 36).Trim) Then
                            dRow = Nothing
                            dRow = dtOnAccountDetails.NewRow
                            dRow("Date") = PARAMOUT2.Substring(Position, 7).Trim
                            dRow("Amount") = Utilities.FormatPrice(PARAMOUT2.Substring(Position + 7, 9))
                            dRow("Product") = PARAMOUT2.Substring(Position + 17, 6)
                            dRow("ProductDescription") = PARAMOUT2.Substring(Position + 23, 40).Trim
                            dRow("PaymentReference") = PARAMOUT2.Substring(Position + 63, 15).TrimStart("0")
                            dRow("ActivtyType") = PARAMOUT2.Substring(Position + 78, 2).Trim
                            dRow("RunningBalance") = Utilities.FormatPrice(PARAMOUT2.Substring(Position + 80, 9))
                            dRow("Seat") = PARAMOUT2.Substring(Position + 89, 18).Trim
                            dRow("RefundFrom") = PARAMOUT2.Substring(Position + 107, 2).Trim

                            ' If Negative make value -ve 
                            Sign = PARAMOUT2.Substring(Position + 16, 1).Trim
                            If Sign = "N" Then
                                dRow("Amount") = dRow("Amount") * -1
                            End If
                            Sign = PARAMOUT2.Substring(Position + 111, 1).Trim
                            If Sign = "N" Then
                                dRow("RunningBalance") = dRow("RunningBalance") * -1
                            End If



                            dtOnAccountDetails.Rows.Add(dRow)

                            'Increment
                            Position = Position + 200
                            Counter = Counter + 1
                        Else
                            Exit Do
                        End If
                    Loop

                    'Extract the footer information
                    MoreRecords = convertToBool(PARAMOUT.Substring(5110, 1))
                    LastRecord = PARAMOUT.Substring(5111, 5)
                    '  RecordTotal = PARAMOUT.Substring(3062, 3)
                Else
                    MoreRecords = False
                End If
            Loop

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBPayment.vb-AccessDatabaseWS067R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function CallWS060R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS060R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS060Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        TalentCommonLog("CallWS060R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        TalentCommonLog("CallWS060R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT
    End Function

    Private Function CallWS067R(ByRef PARAMOUT2 As String, ByVal lastRecord As String) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS067R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS067Parm(lastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 24000)
        parmIO2.Value = ""
        parmIO2.Direction = ParameterDirection.InputOutput

        TalentCommonLog("CallWS067R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString 'data in this one 
        TalentCommonLog("CallWS067R", "", "Backend Response: PARAMOUT=" & PARAMOUT & PARAMOUT2)

        Return PARAMOUT
    End Function

    Private Function WS060Parm() As String
        Dim myString As New Stringbuilder

        myString.Append(Utilities.FixStringLength(_de.SessionId, 36))
        myString.Append(Utilities.PadLeadingZeros(_de.CustomerNumber, 12))
        myString.Append(Utilities.FixStringLength(_de.SaveMyCardMode, 1))
        myString.Append(Utilities.FixStringLength("", 2))
        myString.Append(Utilities.FixStringLength(_de.CardNumber, 19))
        myString.Append(Utilities.FixStringLength(_de.ExpiryDate, 4))
        myString.Append(Utilities.FixStringLength(_de.StartDate, 4))
        myString.Append(Utilities.FixStringLength(_de.IssueNumber, 2))
        myString.Append(Utilities.FixStringLength(_de.CV2Number, 4))
        myString.Append(Utilities.FixStringLength(_de.UniqueCardId, 13))
        myString.Append(Utilities.FixStringLength("", 833)) '7 card detail retrieve
        myString.Append(Utilities.FixStringLength("", 34))  'spare 
        myString.Append(Utilities.FixStringLength(ConvertToYN(_de.DefaultCard), 1))
        myString.Append(Utilities.FixStringLength(_de.CardType, 10))
        myString.Append(Utilities.FixStringLength(_de.TokenDate, 7))
        myString.Append(Utilities.FixStringLength(_de.ProcessingDB, 15))
        myString.Append(Utilities.FixStringLength(_de.TokenID, 18))
        myString.Append(Utilities.FixStringLength("", 3))
        myString.Append(Utilities.FixStringLength(_de.PaymentType, 2))
        myString.Append(Utilities.FixStringLength(_de.Source, 1))
        myString.Append(Utilities.FixStringLength("", 3))
        Return myString.ToString
    End Function

    Private Function WS067Parm(ByVal lastRecord As String) As String
        Dim myString As New StringBuilder

        myString.Append(Utilities.FixStringLength(_de.SessionId, 36))
        myString.Append(Utilities.PadLeadingZeros(_de.CustomerNumber, 12))
        myString.Append(Utilities.FixStringLength("", 18))
        myString.Append(Utilities.FixStringLength(_de.AgentName, 10))
        myString.Append(Utilities.FixStringLength("", 5035))
        myString.Append(Utilities.PadLeadingZeros(lastRecord, 5))
        myString.Append(Utilities.FixStringLength(_de.Source, 1))
        myString.Append(Utilities.FixStringLength("", 3))
        Return myString.ToString
    End Function

    Private Function AccessDatabaseFW035R() As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the results data table
        Dim dtEPurse As New DataTable("EPurse")
        ResultDataSet.Tables.Add(dtEPurse)
        With dtEPurse.Columns
            .Add("EPurseValue", GetType(String))
            .Add("EPursePoints", GetType(String))
            'Points Worth Fields
            .Add("EPursePWPoints", GetType(String))
            .Add("EPursePWValue", GetType(String))
            .Add("EPurseAllowToSpendValue", GetType(String))
        End With

        Try
            PARAMOUT = CallFW035R()

            'Add the status row
            dRow = Nothing
            dRow = dtStatusResults.NewRow
            If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            dtStatusResults.Rows.Add(dRow)

            'Set the response data
            If PARAMOUT.Substring(3071, 1) <> "E" AndAlso PARAMOUT.Substring(3069, 2).Trim = "" Then
                dRow = Nothing
                dRow = dtEPurse.NewRow
                dRow("EPurseValue") = Utilities.FormatPrice(PARAMOUT.Substring(3047, 9).Trim)
                If String.IsNullOrEmpty(PARAMOUT.Substring(3038, 9).TrimStart("0")) Then
                    dRow("EPursePoints") = "0"
                Else
                    dRow("EPursePoints") = PARAMOUT.Substring(3038, 9).TrimStart("0")
                End If
                dRow("EPursePWPoints") = PARAMOUT.Substring(3005, 8)
                dRow("EPursePWValue") = Utilities.FormatPrice(PARAMOUT.Substring(2997, 8).Trim)
                dRow("EPurseAllowToSpendValue") = Utilities.FormatPrice(PARAMOUT.Substring(2978, 9).Trim())
                dtEPurse.Rows.Add(dRow)
                If String.IsNullOrEmpty(De.CardNumber) Then De.CardNumber = PARAMOUT.Substring(2, 13).Trim()
            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "DBPayment.vb-AccessDatabaseFW035R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallFW035R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "FW035R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = FW035Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallFW035R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallFW035R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function FW035Parm() As String
        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(" B")
        myString.Append(Utilities.FixStringLength(De.CardNumber, 14))
        myString.Append(Utilities.FixStringLength(String.Empty, 2924))
        myString.Append(Utilities.FixStringLength(ConvertToYN(De.IsGiftCard), 1))
        myString.Append(Utilities.FixStringLength(De.Currency, 3))
        myString.Append(Utilities.FixStringLength(De.PIN, 4))
        myString.Append(Utilities.FixStringLength(De.CardNumber, 30))
        myString.Append(Utilities.PadLeadingZeros(String.Empty, 9))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSource, 10))
        myString.Append(Utilities.PadLeadingZeros(String.Empty, 59))
        myString.Append(Utilities.FixStringLength(De.CustomerNumber, 12))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))

        Return myString.ToString()
    End Function

    '-------------------------------------------------------
    ' Take Partial Payment and Retrieve partial payment list
    '-------------------------------------------------------
    Private Function AccessDatabaseWS618R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim parmWS618R As String = String.Empty
        Dim parmWS098R As String = String.Empty
        Dim dRow As DataRow = Nothing

        Try

            'Call WS618R
            CallWS618R(parmWS618R, parmWS098R)

            'No errors 
            If parmWS618R.Substring(3071, 1) = "E" Or parmWS618R.Substring(3069, 2).Trim <> "" Then

                'Create the Status data table
                Dim dtStatusResults As New DataTable("StatusResults")
                ResultDataSet.Tables.Add(dtStatusResults)
                With dtStatusResults.Columns
                    .Add("ErrorOccurred", GetType(String))
                    .Add("ReturnCode", GetType(String))
                End With

                'Add the error message
                dRow = Nothing
                dRow = dtStatusResults.NewRow
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = parmWS618R.Substring(3069, 2)
                dtStatusResults.Rows.Add(dRow)
            Else
                AccessDatabaseWS098R(parmWS098R)
            End If

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPY-WS618R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Sub CallWS618R(ByRef parmWS618R As String, ByRef parmWS098R As String)

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS618R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIOWS618R As iDB2Parameter
        Dim parmIOWS098R As iDB2Parameter

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the WS618R parameter
        parmIOWS618R = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIOWS618R.Value = WS618Parm()
        parmIOWS618R.Direction = ParameterDirection.InputOutput

        'Populate the WS030R parameter
        parmIOWS098R = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 3072)
        parmIOWS098R.Value = WS098Parm("000", "000")
        parmIOWS098R.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS618R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIOWS618R.Value=" & parmIOWS618R.Value)
        cmdSELECT.ExecuteNonQuery()
        parmWS618R = cmdSELECT.Parameters(Param1).Value.ToString
        parmWS098R = cmdSELECT.Parameters(Param2).Value.ToString
        TalentCommonLog("CallWS618R", "", "Backend Response: parmWS618R=" & parmWS618R & ";parmWS098R=" & parmWS098R)

    End Sub

    Private Function WS618Parm() As String

        If String.IsNullOrWhiteSpace(De.CustomerNumber) Then De.CustomerNumber = GlobalConstants.GENERIC_CUSTOMER_NUMBER

        Dim sb618Parm As New StringBuilder
        sb618Parm.Append(Utilities.FixStringLength(De.PaymentType, 2))
        If De.PartPaymentId Is Nothing Then
            sb618Parm.Append(Utilities.PadLeadingZeros(0, 13))
        Else
            sb618Parm.Append(Utilities.PadLeadingZeros(De.PartPaymentId, 13))
        End If

        If De.CancelAll Is Nothing Then
            sb618Parm.Append(Utilities.FixStringLength("N", 1))
        Else
            sb618Parm.Append(Utilities.FixStringLength(De.CancelAll, 1))
        End If
        sb618Parm.Append(Utilities.FixStringLength(De.PartPaymentApplyTypeFlag.ConvertPartPaymentFlagToISeriesType, 1))
        sb618Parm.Append(Utilities.FixStringLength("", 32))
        'Construct the parameter

        'This te epurse action only
        Dim actionMode As String = String.Empty
        Select Case Settings.ModuleName
            Case Is = CancelPartPayment
                actionMode = "C"
            Case Is = TakePartPayment
                actionMode = "R"
            Case Else
                actionMode = ""
        End Select
        sb618Parm.Append(Utilities.FixStringLength(actionMode, 1))
        sb618Parm.Append(Utilities.FixStringLength(De.CardNumber, 14))
        sb618Parm.Append(Utilities.FixStringLength("", 35))
        sb618Parm.Append(Utilities.FixStringLength(De.ChipAndPinIPAddress, 15))
        sb618Parm.Append(Utilities.FixStringLength("", 35))
        sb618Parm.Append(Utilities.FixStringLength(De.CardNumber, 19))
        sb618Parm.Append(Utilities.PadLeadingZeros(De.ExpiryDate, 4))
        sb618Parm.Append(Utilities.PadLeadingZeros(De.StartDate, 4))
        sb618Parm.Append(Utilities.FixStringLength(De.IssueNumber, 2))
        sb618Parm.Append(Utilities.FixStringLength(De.CV2Number, 4))
        sb618Parm.Append(Utilities.FixStringLength(De.CardHolderName, 32))
        sb618Parm.Append(Utilities.PadLeadingZeros(De.UniqueCardId, 13))
        sb618Parm.Append(ConvertToYN(De.CustomerPresent))
        sb618Parm.Append(Utilities.FixStringLength(De.SaveMyCardMode, 1))
        sb618Parm.Append(Utilities.FixStringLength("", 70))
        sb618Parm.Append(Utilities.FixStringLength(De.Currency, 3))
        sb618Parm.Append(Utilities.FixStringLength(De.PIN, 4))
        sb618Parm.Append(Utilities.FixStringLength(De.CardNumber, 30))
        sb618Parm.Append(Utilities.FixStringLength("", 163))
        sb618Parm.Append(Utilities.FixStringLength(De.FeesPartPaymentEntity.FeeCode, 10))
        sb618Parm.Append(Utilities.FixStringLength(De.FeesPartPaymentEntity.FeeCategory, 10))
        sb618Parm.Append(Utilities.FixStringLength(De.FeesPartPaymentEntity.CardType, 10))
        sb618Parm.Append(Utilities.PadLeadingZeros((De.FeesPartPaymentEntity.FeeValue).ToString("F2").Replace(".", ""), 9))
        sb618Parm.Append(Utilities.PadLeadingZeros((De.FeesPartPaymentEntity.FeeValueActual).ToString("F2").Replace(".", ""), 9))
        sb618Parm.Append(Utilities.FixStringLength(De.TokenID, 18))
        sb618Parm.Append(Utilities.FixStringLength(De.ProcessingDB, 15))
        sb618Parm.Append(Utilities.FixStringLength(De.TransactionID, 15))
        sb618Parm.Append(Utilities.FixStringLength(De.CardType, 10))
        sb618Parm.Append(Utilities.FixStringLength("", 2413))
        sb618Parm.Append(Utilities.PadLeadingZeros(CDec(De.RetailAmount).ToString("F2").Replace(".", ""), 9))
        sb618Parm.Append(Utilities.FixStringLength("", 1))
        sb618Parm.Append(Utilities.FixStringLength(Settings.OriginatingSource, 10))
        sb618Parm.Append(Utilities.FixStringLength("", 9))
        sb618Parm.Append(Utilities.PadLeadingZeros(CDec(De.Amount).ToString("F2").Replace(".", ""), 9))
        sb618Parm.Append(Utilities.PadLeadingZeros(De.CustomerNumber, 12))
        sb618Parm.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        sb618Parm.Append(Utilities.FixStringLength("", 3))
        Return sb618Parm.ToString

    End Function

    '-------------------------------------------------------
    ' Retrieve partial payment list
    '-------------------------------------------------------
    Private Function AccessDatabaseWS098R(Optional ByVal parmWS098R As String = "") As ErrorObj

        Dim err As New ErrorObj
        If String.IsNullOrEmpty(parmWS098R.Trim) Then
            ResultDataSet = New DataSet
        End If
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim LastRecord As String = "000"
        Dim RecordTotal As String = "000"
        Dim MoreRecords As Boolean = True

        'Create the Status data table
        Dim dtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the results data table
        Dim dtPartPayments As New DataTable("PartPayments")
        ResultDataSet.Tables.Add(dtPartPayments)
        With dtPartPayments.Columns
            .Add("BasketId", GetType(String))
            .Add("CustomerNo", GetType(String))
            .Add("PaymentMethod", GetType(String))
            .Add("PaymentAmount", GetType(String))
            .Add("CardNumber", GetType(String))
            .Add("PartPaymentId", GetType(String))
            .Add("FeeCode", GetType(String))
            .Add("FeeCategory", GetType(String))
            .Add("FeeCard", GetType(String))
            .Add("FeeValue", GetType(Decimal))
            .Add("FeeValueActual", GetType(Decimal))
        End With

        Try

            Do While MoreRecords

                'Call WS098R
                If String.IsNullOrEmpty(parmWS098R.Trim) Then
                    PARAMOUT = CallWS098R(RecordTotal, LastRecord)
                Else
                    PARAMOUT = parmWS098R
                    parmWS098R = ""
                End If

                'Add the status row
                If LastRecord = "000" Then
                    dRow = Nothing
                    dRow = dtStatusResults.NewRow
                    If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    dtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(3071, 1) <> "E" AndAlso PARAMOUT.Substring(3069, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim Position As Integer = 0
                    Dim Counter As Integer = 1
                    Do While Counter <= 15

                        'Create a new row
                        If Not String.IsNullOrEmpty(PARAMOUT.Substring(Position, 36).Trim) Then
                            dRow = Nothing
                            dRow = dtPartPayments.NewRow
                            dRow("BasketId") = PARAMOUT.Substring(Position, 36).Trim
                            dRow("CustomerNo") = PARAMOUT.Substring(Position + 36, 12).Trim
                            dRow("PaymentMethod") = PARAMOUT.Substring(Position + 48, 2).Trim
                            dRow("PaymentAmount") = Utilities.FormatPrice(PARAMOUT.Substring(Position + 50, 15))
                            dRow("CardNumber") = PARAMOUT.Substring(Position + 65, 30).Trim
                            dRow("PartPaymentId") = PARAMOUT.Substring(Position + 95, 13).Trim
                            dRow("FeeCode") = PARAMOUT.Substring(Position + 108, 10).Trim
                            dRow("FeeCategory") = PARAMOUT.Substring(Position + 118, 10).Trim
                            dRow("FeeCard") = PARAMOUT.Substring(Position + 128, 10).Trim
                            dRow("FeeValue") = Utilities.FormatPrice(PARAMOUT.Substring(Position + 138, 9))
                            dRow("FeeValueActual") = Utilities.FormatPrice(PARAMOUT.Substring(Position + 147, 9))
                            dtPartPayments.Rows.Add(dRow)

                            'Increment
                            Position = Position + 200
                            Counter = Counter + 1
                        Else
                            Exit Do
                        End If
                    Loop

                    'Extract the footer information
                    LastRecord = PARAMOUT.Substring(3065, 3)
                    RecordTotal = PARAMOUT.Substring(3062, 3)
                    If CInt(LastRecord) >= CInt(RecordTotal) Then
                        MoreRecords = False
                    End If
                Else
                    MoreRecords = False
                End If
            Loop

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPY-WS098R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS098R(ByVal sRecordTotal As String, _
                                ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS098R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add("@Param1", iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS098Parm(sRecordTotal, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@Param1").Value.ToString

        Return PARAMOUT

    End Function

    Private Function WS098Parm(ByVal sRecordTotal As String, _
                                ByVal sLastRecord As String) As String

        Dim myString As String = String.Empty

        'Construct the parameter
        myString = Utilities.FixStringLength("", 3014)
        myString += Utilities.FixStringLength(De.SessionId, 36)
        myString += Utilities.FixStringLength(De.CustomerNumber, 12)
        myString += Utilities.FixStringLength(sRecordTotal, 3)
        myString += Utilities.FixStringLength(sLastRecord, 3)
        myString += Utilities.FixStringLength(De.Source, 1)
        myString += Utilities.FixStringLength("", 3)

        Return myString

    End Function

    ''' <summary>
    ''' Functionality for updating customer records when completing an order
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseWS001S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            CallWS001S()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Error during database Access"
                .ErrorNumber = "DBPayment-WS001S"
                .HasError = True
            End With
        End Try
        Return err

    End Function


    ''' <summary>
    ''' Functionality for updating customer records when completing an order
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseMD042S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            CallMD042S()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Error during database Access"
                .ErrorNumber = "DBPayment-MD042S"
                .HasError = True
            End With
        End Try
        Return err

    End Function

    ''' <summary>
    ''' Build the parameters and call the stored procedure.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallMD042S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL MD042S(@PARAM0, @PARAM1, @PARAM2)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pType As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.Output
        pSource = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pType = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 20)
        pErrorCode.Value = String.Empty
        pSource.Value = De.Source
        pType.Value = De.variablePricedProductType

        'Create the variable priced product table
        Dim dtVariablePricedProducts As New DataTable("VariablePricedProducts")
        ResultDataSet.Tables.Add(dtVariablePricedProducts)

        _cmdAdapter = New iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "VariablePricedProducts")

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = CStr(_cmd.Parameters(0).Value).Trim
            drStatus("ReturnCode") = GlobalConstants.ERRORFLAG
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

    ''' <summary>
    ''' Build the parameters and call the stored procedure.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallWS001S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL WS001S(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6, @PARAM7, @PARAM8)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pSelectedTALENTUser As iDB2Parameter
        Dim pCustomerNumber As iDB2Parameter
        Dim pMobileNumberField1 As iDB2Parameter
        Dim pMobileNumberField2 As iDB2Parameter
        Dim pMobilePrefFlag As iDB2Parameter
        Dim pEmailAddress As iDB2Parameter
        Dim pEmailPrefFlag As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.Output
        pSource = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pSelectedTALENTUser = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10)
        pCustomerNumber = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 12)
        pMobileNumberField1 = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 7)
        pMobileNumberField2 = _cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 15)
        pMobilePrefFlag = _cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 1)
        pEmailAddress = _cmd.Parameters.Add(Param7, iDB2DbType.iDB2Char, 60)
        pEmailPrefFlag = _cmd.Parameters.Add(Param8, iDB2DbType.iDB2Char, 1)

        pErrorCode.Value = String.Empty
        pSource.Value = De.Source
        pSelectedTALENTUser.Value = De.AgentName
        pCustomerNumber.Value = De.CustomerNumber
        pMobilePrefFlag.Value = De.CustomerDataEntity.ContactViaMobile
        pEmailAddress.Value = De.CustomerDataEntity.EmailAddress
        pEmailPrefFlag.Value = De.CustomerDataEntity.ContactViaEmail
        Utilities.FormatTelephone(De.CustomerDataEntity.MobileNumber, pMobileNumberField2.Value, pMobileNumberField1.Value)

        _cmdAdapter = New iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "ErrorStatus")

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = CStr(_cmd.Parameters(0).Value).Trim
            drStatus("ReturnCode") = GlobalConstants.ERRORFLAG
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

    Private Function AccessDatabaseVG300S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Vanguard Configuration data table
        Dim dtVanguardConfig As New DataTable("VanguardConfiguration")
        With dtVanguardConfig.Columns
            .Add("WebserviceURL", GetType(String))
            .Add("SoapActionURL", GetType(String))
            .Add("SoapHost", GetType(String))
            .Add("SystemID", GetType(String))
            .Add("SystemGUID", GetType(String))
            .Add("ClientPasscode", GetType(String))
            .Add("ReferralPhoneNo", GetType(String))
            .Add("WebserviceTimeout", GetType(String))
        End With
        ResultDataSet.Tables.Add(dtVanguardConfig)

        Dim dtPaymentAccountId As New DataTable("PaymentAccountID")
        With dtVanguardConfig.Columns
            .Add("AccountID", GetType(String))
            .Add("AccountDesc", GetType(String))
            .Add("TKTMerchNumMoto", GetType(String))
            .Add("TKTMerchNumMotoPwd", GetType(String))
            .Add("TKTMerchNumCustPresent", GetType(String))
            .Add("TKTMerchNumCustPresentPwd", GetType(String))
            .Add("PWSMerchNum", GetType(String))
            .Add("PWSMerchNumPwd", GetType(String))
            .Add("RASMerchNum", GetType(String))
            .Add("RASMerchNumPwd", GetType(String))
            .Add("PPSMerchNum", GetType(String))
            .Add("PPSMerchNumPwd", GetType(String))
            .Add("ATSMerchNum", GetType(String))
            .Add("ATSMerchNumPwd", GetType(String))

            .Add("CORPMerchNum", GetType(String))
            .Add("CORPMerchNumPwd", GetType(String))
            .Add("PWSCORPMerchNum", GetType(String))
            .Add("PWSCORPMerchNumPwd", GetType(String))
            .Add("RETAILMerchNum", GetType(String))
            .Add("RETAILMerchNumPwd", GetType(String))
            .Add("PWSRETAILMerchNum", GetType(String))
            .Add("PWSRETAILMerchNumPwd", GetType(String))
        End With

        Try
            ' CallVG300S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBPayment-VG300S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

End Class

