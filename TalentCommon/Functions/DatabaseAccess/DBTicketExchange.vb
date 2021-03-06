Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
<Serializable()>
Public Class DBTicketExchange
    Inherits DBAccess
    '---------------------------------------------------------------------------------------------
    Private _system21CompanyPo As String = String.Empty

    ' Old Buybacks modules have been integrated in here (TE will replace this in the future)
    Private Const OrderReturn As String = "OrderReturn"
    Private Const OrderReturnRebook As String = "OrderReturnRebook"
    Private Const OrderReturnEnquiry As String = "OrderReturnEnquiry"

    Private Const TicketExchangeEnquiry As String = "TicketExchangeEnquiry"
    Private Const GetTicketExchangeProductsListForCustomer As String = "GetTicketExchangeProductsListForCustomer"
    Private Const GetTicketExchangeSeatSelectionForProduct As String = "GetTicketExchangeSeatSelectionForProduct"
    Private Const SubmitTicketExchangeAction As String = "SubmitTicketExchangeAction"
    Private Const GetTicketExchangeDefaults As String = "GetTicketExchangeDefaults"
    Private Const UpdateTicketExchangeDefaults As String = "UpdateTicketExchangeDefaults"

    Private _dep As New DETicketExchange
    Private _cmdAdapter As iDB2DataAdapter = Nothing
    Private _cmd As iDB2Command = Nothing

    Public Property Dep() As DETicketExchange
        Get
            Return _dep
        End Get
        Set(ByVal value As DETicketExchange)
            _dep = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj

        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = SubmitTicketExchangeAction, OrderReturn, OrderReturnRebook
                err = AccessDatabaseWS055R()
            Case TicketExchangeEnquiry, OrderReturnEnquiry
                err = AccessDatabaseWS056R()
            Case GetTicketExchangeProductsListForCustomer
                err = AccessDatabaseWS053S()
            Case GetTicketExchangeSeatSelectionForProduct
                err = AccessDatabaseWS054S()
            Case GetTicketExchangeDefaults
                err = AccessDatabaseWS055S()
            Case UpdateTicketExchangeDefaults
                err = AccessDatabaseWS056S()
        End Select

        Return err

    End Function

#Region "WS055R"

    Public Function AccessDatabaseWS055R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim sTicketExchangeRef As String = String.Empty
        Dim sErrorOccurred As String = String.Empty
        Dim sReturnCode As String = String.Empty
        Dim PARAMOUT As String = String.Empty
        Dim iReturnedOrders As Int32
        Dim iSystemCalls As Int32

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Status data table
        Dim DtTicketExchange As New DataTable("TicketExchange")
        ResultDataSet.Tables.Add(DtTicketExchange)
        With DtTicketExchange.Columns
            .Add("TicketExchangeReference", GetType(String))
        End With

        Try
            iReturnedOrders = Dep.TicketExchangeItems.Count
            iSystemCalls = 0

            'Loop until all the Orders Returns have been retrieved.
            'Call Order Return Api 20 OrdersReturns at a time. 
            Do While iReturnedOrders > (20 * iSystemCalls)

                Dim iStrIndex As Int32 = (20 * iSystemCalls) + 1
                Dim iEndIndex As Int32 = iReturnedOrders

                If iEndIndex > iStrIndex + 19 Then
                    iEndIndex = iStrIndex + 19
                End If

                PARAMOUT = CallWS055R(sTicketExchangeRef, iStrIndex, iEndIndex)

                If (PARAMOUT.Substring(5119, 1) = "E" Or PARAMOUT.Substring(5117, 2).Trim <> "") Then
                    sErrorOccurred = "E"
                    sReturnCode = PARAMOUT.Substring(5117, 2)
                    sTicketExchangeRef = ""
                    'Stop processing after an error.
                    Exit Do
                Else
                    sErrorOccurred = ""
                    sReturnCode = ""
                    sTicketExchangeRef = PARAMOUT.Substring(4985, 13)
                End If

                'increment the count of calls made to the back end (groups of 20)
                iSystemCalls += 1
            Loop

            'After all calls are made, set the status of the call in the status table.
            'Set the response data
            dRow = DtStatusResults.NewRow
            dRow("ErrorOccurred") = sErrorOccurred
            dRow("ReturnCode") = sReturnCode
            DtStatusResults.Rows.Add(dRow)

            dRow = DtTicketExchange.NewRow
            dRow("TicketExchangeReference") = sTicketExchangeRef
            DtTicketExchange.Rows.Add(dRow)


        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBORTN-01"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS055R(ByVal sTicketExchangeRef As String,
                                ByVal iStrIndex As Int32,
                                ByVal iEndIndex As Int32) As String
        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS055R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add("@Param1", iDB2DbType.iDB2Char, 1028)
        parmIO.Value = String.Empty
        parmIO.Direction = ParameterDirection.InputOutput


        parmIO = cmdSELECT.Parameters.Add("@Param2", iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS055Parm(sTicketExchangeRef, iStrIndex, iEndIndex)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@Param2").Value.ToString

        Return PARAMOUT

    End Function

    Public Function WS055Parm(ByVal sTicketExchangeRef As String,
                          ByVal iStrIndex As Int32,
                          ByVal iEndIndex As Int32) As String

        'Arrays of Ticket Exchange Items
        Dim sTicketExchangeItems As New StringBuilder()
        Dim sModeArray As New StringBuilder()
        Dim PaymentOwnerArray As New StringBuilder()
        Dim ticketExchangeRequestedPriceArray As New StringBuilder()
        Dim ticketExchangeOriginalPriceArray As New StringBuilder()
        Dim ticketExchangeFaceValuePriceArray As New StringBuilder()
        Dim PotentialEarningArray As New StringBuilder()
        Dim FeeArray As New StringBuilder()
        Dim FeeTypeArray As New StringBuilder()

        Dim priceInPence As Integer = 0

        iStrIndex = iStrIndex - 1
        iEndIndex = iEndIndex - 1

        For index As Int32 = iStrIndex To iEndIndex
            sTicketExchangeItems.Append(Utilities.FixStringLength((Dep.TicketExchangeItems.Item(index).ProductCode), 6))
            sTicketExchangeItems.Append(Utilities.FixStringLength((Dep.TicketExchangeItems.Item(index).SeatDetails.Stand), 3))
            sTicketExchangeItems.Append(Utilities.FixStringLength((Dep.TicketExchangeItems.Item(index).SeatDetails.Area), 4))
            sTicketExchangeItems.Append(Utilities.FixStringLength((Dep.TicketExchangeItems.Item(index).SeatDetails.Row), 4))
            sTicketExchangeItems.Append(Utilities.FixStringLength((Dep.TicketExchangeItems.Item(index).SeatDetails.Seat), 4))
            sTicketExchangeItems.Append(Utilities.FixStringLength((Dep.TicketExchangeItems.Item(index).SeatDetails.AlphaSuffix), 1))

            ' 0=Not For Sale, 1=For Sale, 2=Sold, 3=Placing On Sale, 4=Taking Off Sale, 5=Pricing Change 
            If Dep.TicketExchangeItems.Item(index).Status = GlobalConstants.TicketExchangeItemStatus.PriceChanged Then
                sModeArray.Append("P")
            ElseIf Dep.TicketExchangeItems.Item(index).Status = GlobalConstants.TicketExchangeItemStatus.TakingOffSale Then
                sModeArray.Append("Y")
            ElseIf Dep.TicketExchangeItems.Item(index).Status = GlobalConstants.TicketExchangeItemStatus.PlacingOnSale Then
                sModeArray.Append(" ")
            End If

            PaymentOwnerArray.Append(Utilities.FixStringLength((Dep.TicketExchangeItems.Item(index).PaymentOwnerCustomerNo), 12))

            ' Store the original Price, face value price and selected resale price.
            priceInPence = (Dep.TicketExchangeItems.Item(index).RequestedPrice * 100)
            ticketExchangeRequestedPriceArray.Append(Utilities.PadLeadingZeros(priceInPence, 9))
            priceInPence = (Dep.TicketExchangeItems.Item(index).OriginalPrice * 100)
            ticketExchangeOriginalPriceArray.Append(Utilities.PadLeadingZeros(priceInPence, 9))
            priceInPence = (Dep.TicketExchangeItems.Item(index).FaceValuePrice * 100)
            ticketExchangeFaceValuePriceArray.Append(Utilities.PadLeadingZeros(priceInPence, 9))
            priceInPence = (Dep.TicketExchangeItems.Item(index).PotentialEarning * 100)
            PotentialEarningArray.Append(Utilities.PadLeadingZeros(priceInPence, 9))
            priceInPence = (Dep.TicketExchangeItems.Item(index).Fee * 100)
            FeeArray.Append(Utilities.PadLeadingZeros(priceInPence, 9))
            FeeTypeArray.Append(Utilities.FixStringLength(Dep.TicketExchangeItems.Item(index).FeeType, 1))
        Next

        'Pad total records value with leading zeros. (Range is 1- 20)
        Dim sTotalRecs As String = "0" & CType((iEndIndex - iStrIndex + 1), String)
        If sTotalRecs.Trim.Length < 3 Then
            sTotalRecs = "0" + sTotalRecs.Trim
        End If

        Dim sFinalTicketExchangeString As New StringBuilder()
        sFinalTicketExchangeString.Append(Utilities.FixStringLength(sTicketExchangeItems.ToString, 440))                    '      1    440
        sFinalTicketExchangeString.Append(Utilities.FixStringLength(sModeArray.ToString, 20))                               '    441    460
        sFinalTicketExchangeString.Append(Utilities.FixStringLength(PaymentOwnerArray.ToString, 240))                       '    461    700
        sFinalTicketExchangeString.Append(Utilities.FixStringLength(ticketExchangeRequestedPriceArray.ToString, 180))       '    701    880
        sFinalTicketExchangeString.Append(Utilities.FixStringLength(ticketExchangeOriginalPriceArray.ToString, 180))        '    881   1060
        sFinalTicketExchangeString.Append(Utilities.FixStringLength(ticketExchangeFaceValuePriceArray.ToString, 180))       '   1061   1240
        sFinalTicketExchangeString.Append(Utilities.FixStringLength(PotentialEarningArray.ToString, 180))                   '   1241   1420
        sFinalTicketExchangeString.Append(Utilities.FixStringLength(FeeArray.ToString, 180))                                '   1421   1600
        sFinalTicketExchangeString.Append(Utilities.FixStringLength(FeeTypeArray.ToString, 20))                             '   1601   1620
        sFinalTicketExchangeString.Append(Utilities.FixStringLength(String.Empty, 3354))                                    '   1621   4974   Spare Flags
        If _settings.ModuleName = SubmitTicketExchangeAction Then                                                            '   4975   4975
            sFinalTicketExchangeString.Append(Utilities.FixStringLength("Y", 1))
        Else
            sFinalTicketExchangeString.Append(Utilities.FixStringLength("N", 1))
        End If

        If Settings.AgentEntity IsNot Nothing Then
            sFinalTicketExchangeString.Append(Utilities.FixStringLength(Settings.AgentEntity.AgentUsername, 10))                '   4976   4985  
        Else
            sFinalTicketExchangeString.Append(Utilities.FixStringLength(String.Empty, 10))                                      '   4976   4985  
        End If

        sFinalTicketExchangeString.Append(Utilities.FixStringLength(sTicketExchangeRef, 13))                                    '   4986   4998  
        sFinalTicketExchangeString.Append(Utilities.FixStringLength(Dep.Comment1, 50))                                          '   4999   5048  
        sFinalTicketExchangeString.Append(Utilities.FixStringLength(Dep.Comment2, 50))                                          '   5048   5098  
        sFinalTicketExchangeString.Append(Utilities.FixStringLength(Dep.ListingCustomer, 12))                                   '   5099   5110  
        sFinalTicketExchangeString.Append(Utilities.FixStringLength(sTotalRecs, 3))                                             '   5111   5113  
        sFinalTicketExchangeString.Append(Utilities.FixStringLength("000", 3) & "W   ")                                         '   5114   5120 

        Return sFinalTicketExchangeString.ToString

    End Function
#End Region

#Region "WS056R"

    Public Function AccessDatabaseWS056R() As ErrorObj
        Dim err As New ErrorObj
        Const ITEMS_PER_CALL As Integer = 20
        Dim dRow As DataRow = Nothing
        Dim ErrorOccurred As String = ""
        Dim ReturnCode As String = ""
        Dim PARAMOUT As String = String.Empty
        Dim NoOfCallsDone As Integer = 0
        Dim MoreRecords As Boolean = True

        ResultDataSet = New DataSet()
        Dim DtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        '
        ' Create the Order Return Enquiry data table
        '
        Dim dtTicketExchangeEnquiry As New DataTable("TicketExchangeEnquiry")
        ResultDataSet.Tables.Add(dtTicketExchangeEnquiry)
        With dtTicketExchangeEnquiry.Columns
            .Add("MemberNumber", GetType(String))
            .Add("Title", GetType(String))
            .Add("ContactForename", GetType(String))
            .Add("ContactSurname", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("ProductDescription", GetType(String))
            .Add("BuyBackDate", GetType(String))
            .Add("BuyBackTime", GetType(String))
            .Add("Stand", GetType(String))
            .Add("Area", GetType(String))
            .Add("Row", GetType(String))
            .Add("Seat", GetType(String))
            .Add("AlphaSuffix", GetType(String))
            .Add("Turnstile", GetType(String))
            .Add("Gate", GetType(String))
            .Add("SeatRestriction", GetType(String))
            .Add("RestText", GetType(String))
            .Add("RequestedPrice", GetType(Decimal))
            .Add("ClubHandlingFee", GetType(Decimal))
            .Add("OnSale", GetType(Boolean))
            .Add("TicketExchangeorBuyback", GetType(String))
            .Add("PreviousRequestedPrice", GetType(Decimal))
            .Add("FaceValue", GetType(Decimal))
            .Add("OriginalSalePayref", GetType(Decimal))
            .Add("Earnings", GetType(Decimal))
            .Add("TicketExchangeRef", GetType(Decimal))
            .Add("Unreserved", GetType(Boolean))
        End With

        Try
            '
            ' Loop until all the Orders Returns have been retrieved.
            ' Call Order Return Api ITEMS_PER_CALL OrdersReturns at a time.
            '
            While MoreRecords
                Dim LastRecord As Integer = (ITEMS_PER_CALL * NoOfCallsDone)

                PARAMOUT = CallWS056R(LastRecord)

                'Set the response data
                If PARAMOUT.Substring(5117, 1) = "E" Or PARAMOUT.Substring(5118, 2).Trim() <> "" Then
                    ErrorOccurred = "E"
                    ReturnCode = PARAMOUT.Substring(5118, 2)
                    Exit While                              'Stop processing after an error.
                Else
                    '
                    ' We can have up to ITEMS_PER_CALL returned so loop through them
                    '
                    For i As Integer = 0 To ITEMS_PER_CALL - 1

                        Dim Start As Integer = i * 250
                        If PARAMOUT.Substring(Start, 250).Trim() <> "" Then
                            '
                            ' Create a new row for each of the records returned
                            '
                            dRow = dtTicketExchangeEnquiry.NewRow()

                            dRow("MemberNumber") = PARAMOUT.Substring(Start, 12).Trim()
                            dRow("ProductCode") = PARAMOUT.Substring(Start + 12, 6).Trim()
                            dRow("ProductDescription") = PARAMOUT.Substring(Start + 18, 40).Trim()
                            dRow("BuyBackDate") = PARAMOUT.Substring(Start + 58, 7).Trim()
                            dRow("BuyBackTime") = PARAMOUT.Substring(Start + 65, 6).Trim()
                            dRow("Stand") = PARAMOUT.Substring(Start + 71, 3).Trim()
                            dRow("Area") = PARAMOUT.Substring(Start + 74, 4).Trim()
                            dRow("Row") = PARAMOUT.Substring(Start + 78, 4).Trim()
                            dRow("Seat") = PARAMOUT.Substring(Start + 82, 4).Trim().TrimStart("0")
                            dRow("AlphaSuffix") = PARAMOUT.Substring(Start + 86, 1).Trim()
                            dRow("Turnstile") = PARAMOUT.Substring(Start + 87, 12).Trim()
                            dRow("Gate") = PARAMOUT.Substring(Start + 99, 12).Trim()
                            dRow("Title") = PARAMOUT.Substring(Start + 111, 6).Trim()
                            dRow("ContactForename") = PARAMOUT.Substring(Start + 117, 20).Trim()
                            dRow("ContactSurname") = PARAMOUT.Substring(Start + 137, 30).Trim()
                            dRow("RequestedPrice") = Utilities.FormatPrice(PARAMOUT.Substring(Start + 167, 9).Trim())
                            dRow("ClubHandlingFee") = Utilities.FormatPrice(PARAMOUT.Substring(Start + 176, 9).Trim())
                            dRow("TicketExchangeorBuyback") = PARAMOUT.Substring(Start + 185, 1).Trim()
                            dRow("OnSale") = Utilities.CheckForDBNull_Boolean_DefaultTrue(PARAMOUT.Substring(Start + 186, 1).Trim())
                            dRow("PreviousRequestedPrice") = Utilities.FormatPrice(PARAMOUT.Substring(Start + 187, 9).Trim())
                            dRow("FaceValue") = Utilities.FormatPrice(PARAMOUT.Substring(Start + 196, 9).Trim())
                            dRow("OriginalSalePayref") = Utilities.CheckForDBNull_Decimal(PARAMOUT.Substring(Start + 205, 15).Trim())
                            dRow("Earnings") = Utilities.FormatPrice(PARAMOUT.Substring(Start + 220, 9).Trim())
                            dRow("TicketExchangeRef") = Utilities.CheckForDBNull_Decimal(PARAMOUT.Substring(Start + 229, 13).Trim())
                            dRow("Unreserved") = Utilities.convertToBool(PARAMOUT.Substring(Start + 242, 1).Trim())
                            dRow("SeatRestriction") = ""
                            dRow("RestText") = ""

                            dtTicketExchangeEnquiry.Rows.Add(dRow)
                        Else
                            '
                            ' No more results to process, so exit this loop
                            '
                            Exit For
                        End If

                    Next

                End If
                '
                ' Increment the count of calls made to the back end (groups of ITEMS_PER_CALL)
                ' if there's more information
                '
                If PARAMOUT.Substring(5112, 1) = "Y" Then
                    NoOfCallsDone += 1
                Else
                    MoreRecords = False
                End If

            End While


            'After all calls are made, set the status of the call in the status table.
            dRow = Nothing
            dRow = DtStatusResults.NewRow()
            dRow("ErrorOccurred") = ErrorOccurred
            dRow("ReturnCode") = ReturnCode
            DtStatusResults.Rows.Add(dRow)

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBORTN-02"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS056R(ByVal LastRecord As Integer) As String
        '
        ' Create command object
        '
        Dim cmdSELECT As iDB2Command = Nothing
        Dim HEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim() & "/WS056R(@PARAM1)"
        Dim parmIO As iDB2Parameter
        '
        ' Set the connection string
        '
        cmdSELECT = New iDB2Command(HEADER, conTALENTTKT)
        '
        ' Populate the parameter
        '
        Dim ParamValue As New StringBuilder("")
        ParamValue.Append(Utilities.FixStringLength("", 5072))
        ParamValue.Append(Utilities.PadLeadingZeros(Dep.ResoldPayref, 15))
        ParamValue.Append(Utilities.PadLeadingZeros(Dep.Customer, 12))
        ParamValue.Append(Utilities.PadLeadingZeros(Dep.TicketExchangeReference, 13))
        ParamValue.Append(Utilities.FixStringLength("", 1))
        ParamValue.Append(Utilities.PadLeadingZeros(LastRecord.ToString(), 3))
        ParamValue.Append(Utilities.FixStringLength(Dep.OriginatingSourceCode, 1))
        ParamValue.Append(Utilities.FixStringLength("", 1))
        ParamValue.Append(Utilities.FixStringLength("", 2))

        parmIO = cmdSELECT.Parameters.Add("@Param1", iDB2DbType.iDB2Char, 5120)
        parmIO.Value = ParamValue.ToString()
        parmIO.Direction = ParameterDirection.InputOutput
        '
        ' Execute
        '
        cmdSELECT.ExecuteNonQuery()
        Dim PARAMOUT As String = cmdSELECT.Parameters("@Param1").Value.ToString()

        Return PARAMOUT

    End Function

#End Region

#Region "WS053S"

    Private Function AccessDatabaseWS053S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            CallWS053S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBTicketExchangeSummary-WS053S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Sub CallWS053S()
        Dim _cmd As iDB2Command = Nothing
        Dim _cmdAdapter As iDB2DataAdapter = Nothing
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL WS053S( @ERROR, @SOURCE, @PARAM3, @PARAM4)"
        _cmd.CommandType = CommandType.Text

        Dim pCustomerNo As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pErrorCode As iDB2Parameter
        Dim pDate As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pSource = _cmd.Parameters.Add(Source, iDB2DbType.iDB2Char, 1)
        pCustomerNo = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 12)
        pDate = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Decimal, 7)
        pCustomerNo.Value = Dep.Customer
        pDate.Value = Dep.FromDate
        pSource.Value = GlobalConstants.SOURCE
        pErrorCode.Value = String.Empty

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet)
        Utilities.ConvertISeriesTables(ResultDataSet)
        If ResultDataSet.Tables.Count = 3 Then
            ResultDataSet.Tables(1).TableName = "TicketExchangeProductSummary"
            ResultDataSet.Tables(2).TableName = "TicketExchangeSummary"
        End If

        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If CStr(_cmd.Parameters(ErrorCode).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param2).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)
    End Sub
#End Region

#Region "WS054R"

    Private Function AccessDatabaseWS054S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            CallWS054S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBTicketExchangeSummary-WS054S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Sub CallWS054S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL WS054S( @ERROR, @SOURCE, @PARAM3, @PARAM4)"
        _cmd.CommandType = CommandType.Text

        Dim pCustomerNo As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pErrorCode As iDB2Parameter
        Dim pProductCode As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pSource = _cmd.Parameters.Add(Source, iDB2DbType.iDB2Char, 1)
        pCustomerNo = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 12)
        pProductCode = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 6)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pCustomerNo.Value = Dep.Customer
        pProductCode.Value = Dep.ProductCode
        pSource.Value = GlobalConstants.SOURCE
        pErrorCode.Value = String.Empty

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet)
        Utilities.ConvertISeriesTables(ResultDataSet)
        If ResultDataSet.Tables.Count = 3 Then
            ResultDataSet.Tables(1).TableName = "TicketExchangeProductInfomation"
            ResultDataSet.Tables(2).TableName = "TicketExchangeCustomerSeats"
        End If

        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If CStr(_cmd.Parameters(ErrorCode).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(Param2).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)
    End Sub
#End Region

#Region "WS055R Get TE Defaults"

    Private Function AccessDatabaseWS055S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            CallWS055S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBTicketExchangeDefaults-WS055S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Sub CallWS055S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL WS055S( @ERROR, @SOURCE, @PARAM3, @PARAM4)"
        _cmd.CommandType = CommandType.Text

        Dim pReturnAllProducts As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pErrorCode As iDB2Parameter
        Dim pProductCode As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pSource = _cmd.Parameters.Add(Source, iDB2DbType.iDB2Char, 1)
        pProductCode = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 6)
        pReturnAllProducts = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 1)
        pErrorCode.Direction = ParameterDirection.InputOutput

        pErrorCode.Value = " "
        pSource.Value = GlobalConstants.SOURCE
        pProductCode.Value = Dep.ProductCode
        pReturnAllProducts.Value = "N"
        If Dep.ReturnAllProducts Then
            pReturnAllProducts.Value = "1"
        End If
        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet)
        Utilities.ConvertISeriesTables(ResultDataSet)
        If ResultDataSet.Tables.Count = 3 Then
            ResultDataSet.Tables(1).TableName = "ProductTESummary"
            ResultDataSet.Tables(2).TableName = "StandAreaTESummary"
        End If

        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If CStr(_cmd.Parameters(ErrorCode).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(ErrorCode).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)
    End Sub
#End Region

#Region "WS056R Update TE Defaults"

    Private Function AccessDatabaseWS056S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim dtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            CallWS056S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBTicketExchangeDefaultsUpdate-WS056S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Sub CallWS056S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL WS056S( @ERROR, @SOURCE, @PARAM3, @PARAM4, @PARAM5, @PARAM6, @PARAM7, @PARAM8, @PARAM9, @PARAM10, @PARAM11, @PARAM12, @PARAM13, @PARAM14, @PARAM15, @PARAM16, @PARAM17, @PARAM18, @PARAM19)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pSelectedTALENTUser As iDB2Parameter
        Dim pProductCode As iDB2Parameter
        Dim pAllowTicketExchangeReturn As iDB2Parameter
        Dim pAllowTicketExchangePurchase As iDB2Parameter
        Dim pClubFeePercentageOrFixed As iDB2Parameter
        Dim pMinMaxBoundaryPercentageOrFixed As iDB2Parameter
        Dim pCustomerRetainsPrerequisite As iDB2Parameter
        Dim pCustomerRetainsMaxLimit As iDB2Parameter
        Dim pMinimumResalePrice As iDB2Parameter
        Dim pMaximumResalePrice As iDB2Parameter
        Dim pClubFee As iDB2Parameter
        Dim pProdMaximumTEPerCustomer As iDB2Parameter
        Dim pNumberOfStandAreas As iDB2Parameter
        Dim pStandAreaStandCodes As iDB2Parameter
        Dim pStandAreaAreaCodes As iDB2Parameter
        Dim pStandAreaAllowTicketExchangeReturnFlags As iDB2Parameter
        Dim pStandAreaAllowTicketExchangePurchaseFlags As iDB2Parameter

        Dim sBuilderStandCodes As New StringBuilder
        Dim sBuilderAreaCodes As New StringBuilder
        Dim sBuilderAllowTicketExchangeReturnFlags As New StringBuilder
        Dim sBuilderAllowTicketExchangePurchaseFlags As New StringBuilder
        Dim TESad As DETicketExchangeStandAreaDefaults

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pSource = _cmd.Parameters.Add(Source, iDB2DbType.iDB2Char, 1)
        pSelectedTALENTUser = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 10)
        pProductCode = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 6)
        pAllowTicketExchangeReturn = _cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 1)
        pAllowTicketExchangePurchase = _cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 1)
        pClubFeePercentageOrFixed = _cmd.Parameters.Add(Param7, iDB2DbType.iDB2Char, 1)
        pCustomerRetainsPrerequisite = _cmd.Parameters.Add(Param8, iDB2DbType.iDB2Char, 1)
        pCustomerRetainsMaxLimit = _cmd.Parameters.Add(Param9, iDB2DbType.iDB2Char, 1)
        pMinMaxBoundaryPercentageOrFixed = _cmd.Parameters.Add(Param10, iDB2DbType.iDB2Char, 1)
        pMinimumResalePrice = _cmd.Parameters.Add(Param11, iDB2DbType.iDB2Decimal, 15, 2)
        pMaximumResalePrice = _cmd.Parameters.Add(Param12, iDB2DbType.iDB2Decimal, 15, 2)
        pClubFee = _cmd.Parameters.Add(Param13, iDB2DbType.iDB2Decimal, 15, 2)
        pProdMaximumTEPerCustomer = _cmd.Parameters.Add(Param14, iDB2DbType.iDB2Decimal, 3, 0)
        pNumberOfStandAreas = _cmd.Parameters.Add(Param15, iDB2DbType.iDB2Decimal, 3, 0)
        pStandAreaStandCodes = _cmd.Parameters.Add(Param16, iDB2DbType.iDB2VarChar, 600)
        pStandAreaAreaCodes = _cmd.Parameters.Add(Param17, iDB2DbType.iDB2VarChar, 800)
        pStandAreaAllowTicketExchangeReturnFlags = _cmd.Parameters.Add(Param18, iDB2DbType.iDB2VarChar, 200)
        pStandAreaAllowTicketExchangePurchaseFlags = _cmd.Parameters.Add(Param19, iDB2DbType.iDB2VarChar, 200)

        pErrorCode.Direction = ParameterDirection.InputOutput
        pErrorCode.Value = String.Empty
        pSource.Value = GlobalConstants.SOURCE
        pSelectedTALENTUser.Value = Settings.AgentEntity.AgentUsername
        pProductCode.Value = Dep.ProductCode
        pAllowTicketExchangeReturn.Value = ConvertToISeriesYesNo(Dep.AllowTicketExchangeReturn)
        pAllowTicketExchangePurchase.Value = ConvertToISeriesYesNo(Dep.AllowTicketExchangePurchase)
        pClubFeePercentageOrFixed.Value = Dep.ClubFeePercentageOrFixed
        pCustomerRetainsPrerequisite.Value = ConvertToISeriesYesNo(Dep.CustomerRetainsPrerequisite)
        pCustomerRetainsMaxLimit.Value = ConvertToISeriesYesNo(Dep.CustomerRetainsMaxLimit)
        pMinMaxBoundaryPercentageOrFixed.Value = Dep.MinMaxBoundaryPercentageOrFixed
        pMinimumResalePrice.Value = Dep.MinimumResalePrice
        pMaximumResalePrice.Value = Dep.MaximumResalePrice
        pClubFee.Value = Dep.ClubFee
        pProdMaximumTEPerCustomer.Value = Dep.ProdMaximumTEPerCustomer
        pNumberOfStandAreas.Value = Dep.NumberOfStandAreas
        
        For s As Integer = 0 To pNumberOfStandAreas.Value - 1
            TESad = Dep.StandAreaDefaults.Item(s)
            sBuilderStandCodes.Append(TESad.StandAreaStandCode.PadRight(3))
            sBuilderAreaCodes.Append(TESad.StandAreaAreaCode.PadRight(4))
            sBuilderAllowTicketExchangePurchaseFlags.Append(ConvertToISeriesYesNo(TESad.StandAreaAllowTicketExchangePurchaseFlag))
            sBuilderAllowTicketExchangeReturnFlags.Append(ConvertToISeriesYesNo(TESad.StandAreaAllowTicketExchangeReturnFlag))
        Next
        pStandAreaStandCodes.Value = sBuilderStandCodes.ToString
        pStandAreaAreaCodes.Value = sBuilderAreaCodes.ToString
        pStandAreaAllowTicketExchangePurchaseFlags.Value = sBuilderAllowTicketExchangePurchaseFlags.ToString
        pStandAreaAllowTicketExchangeReturnFlags.Value = sBuilderAllowTicketExchangeReturnFlags.ToString

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet)


        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If CStr(_cmd.Parameters(ErrorCode).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(ErrorCode).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)
    End Sub
#End Region

End Class
