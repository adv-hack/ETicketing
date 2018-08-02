Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports Talent.Common.Utilities
Imports System.Text

<Serializable()> _
Public Class DBReservations
    Inherits DBAccess

#Region "Class Level Fields"

    Private _cmd As iDB2Command = Nothing
    Private _cmdAdapter As iDB2DataAdapter = Nothing
    Private _basketFunctions As New DBBasketFunctions

#End Region

#Region "Public Properties"
    Public Property DeReservations() As DEReservations
#End Region

#Region "Constants"
    Private Const RetrieveCustomerReservations As String = "RetrieveCustomerReservations"
    Private Const ReserveAllBasketItems As String = "ReserveAllBasketItems"
    Private Const ReserveAllBasketItemsReturnBasket As String = "ReserveAllBasketItemsReturnBasket"
    Private Const UnreserveAllBasketItems As String = "UnreserveAllBasketItems"
    Private Const UnreserveAllBasketItemsReturnBasket As String = "UnreserveAllBasketItemsReturnBasket"
    Private Const UnreserveSeparateBasketItems As String = "UnreserveSeparateBasketItems"
    Private Const UnreserveSeparateBasketItemsReturnBasket As String = "UnreserveSeparateBasketItemsReturnBasket"
    Private Const AddReservationToBasketReturnBasket As String = "AddReservationToBasketReturnBasket"
#End Region

#Region "TALENTTKT"
    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        'Set up the basket functions object
        _basketFunctions.conTALENTTKT = conTALENTTKT
        _basketFunctions.StoredProcedureGroup = Settings.StoredProcedureGroup
        _basketFunctions.OriginatingSource = Settings.OriginatingSource
        _basketFunctions.CustomerNo = Settings.LoginId
        _basketFunctions.SessionId = DeReservations.SessionID
        _basketFunctions.Source = DeReservations.Source
        If Settings.AgentEntity IsNot Nothing Then
            _basketFunctions.BulkSalesMode = Settings.AgentEntity.BulkSalesMode
        End If

        Select Case _settings.ModuleName
            Case Is = ReserveAllBasketItems : err = AccessDatabaseWS270R()
            Case Is = ReserveAllBasketItemsReturnBasket : err = AccessDatabaseWS627R()
            Case Is = RetrieveCustomerReservations : err = AccessDatabaseWS018R()
            Case Is = AddReservationToBasketReturnBasket : err = AccessDatabaseWS626R()
            Case Is = UnreserveAllBasketItems, UnreserveSeparateBasketItems : err = AccessDatabaseWS271R()
            Case Is = UnreserveAllBasketItemsReturnBasket, UnreserveSeparateBasketItemsReturnBasket : err = AccessDatabaseWS628R()
        End Select

        Return err

    End Function
#End Region

#Region "Protected Functions"
    ''' <summary>
    ''' Adds reservations to basket or retrieves reservations
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function AccessDatabaseWS018R() As ErrorObj

        Dim err As New ErrorObj
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing

        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim dtBasketDetails As New DataTable("BasketStatus")
        ResultDataSet.Tables.Add(dtBasketDetails)
        With dtBasketDetails.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("ValidationError", GetType(String))
        End With

        Dim dtCustomerReservations As New DataTable("CustomerReservations")
        With dtCustomerReservations.Columns
            .Add("ProductCode", GetType(String))
            .Add("ProductCodeDescription", GetType(String))
            .Add("CustomerNumber", GetType(String))
            .Add("StandCode", GetType(String))
            .Add("AreaCode", GetType(String))
            .Add("RowNumber", GetType(String))
            .Add("SeatNumber", GetType(String))
            .Add("AlphaSuffix", GetType(String))
            .Add("PriceCode", GetType(String))
            .Add("PriceCodeDescription", GetType(String))
            .Add("ReservationReference", GetType(String))
            .Add("ReservedUntilDate", GetType(String))
            .Add("ReservedUntilTime", GetType(String))
            .Add("PriceBand", GetType(String))
            .Add("PriceBandDescription", GetType(String))
            .Add("TicketNumber", GetType(String))
            .Add("ReservationCode", GetType(String))
            .Add("Quantity", GetType(Integer))
            .Add("ProductType", GetType(String))
            .Add("LinkedID", GetType(Long))
            .Add("LinkMandatory", GetType(String))
            .Add("DespatchDate", GetType(String))
        End With

        ResultDataSet.Tables.Add(dtCustomerReservations)
        Try

            Dim boolCallWS018R As Boolean = True
            Dim sMore As String = ""
            Dim sLastRec As String = ""
            Do While boolCallWS018R

                'Call WS018R
                CallWS018R(PARAMOUT, PARAMOUT2, sLastRec)

                Dim position As Integer = 0
                Do While PARAMOUT2.Substring(position, 250).Trim <> ""
                    dRow = dtCustomerReservations.NewRow
                    dRow("CustomerNumber") = PARAMOUT2.Substring(position, 12).TrimStart("0")
                    dRow("PriceCode") = PARAMOUT2.Substring(position + 12, 2)
                    dRow("PriceCodeDescription") = PARAMOUT2.Substring(position + 14, 40).Trim
                    dRow("PriceBand") = PARAMOUT2.Substring(position + 54, 1)
                    dRow("PriceBandDescription") = PARAMOUT2.Substring(position + 55, 15).Trim
                    dRow("ProductCode") = PARAMOUT2.Substring(position + 70, 6).Trim
                    dRow("ProductCodeDescription") = PARAMOUT2.Substring(position + 76, 40).Trim
                    dRow("StandCode") = PARAMOUT2.Substring(position + 116, 3)
                    dRow("AreaCode") = PARAMOUT2.Substring(position + 119, 4)
                    dRow("RowNumber") = PARAMOUT2.Substring(position + 123, 4)
                    dRow("SeatNumber") = PARAMOUT2.Substring(position + 127, 4)
                    dRow("AlphaSuffix") = PARAMOUT2.Substring(position + 131, 1)
                    ' gap here  
                    dRow("ReservationCode") = PARAMOUT2.Substring(position + 146, 2).Trim
                    dRow("ReservationReference") = PARAMOUT2.Substring(position + 148, 15).TrimStart("0")
                    dRow("ReservedUntilDate") = PARAMOUT2.Substring(position + 163, 7).Trim
                    dRow("ReservedUntilTime") = PARAMOUT2.Substring(position + 170, 6).Trim
                    dRow("ProductType") = PARAMOUT2.Substring(position + 176, 1).Trim
                    dRow("LinkedID") = Utilities.PadLeadingZeros(PARAMOUT2.Substring(position + 177, 13).Trim, 13)
                    dRow("LinkMandatory") = PARAMOUT2.Substring(position + 190, 1).Trim
                    dRow("TicketNumber") = PARAMOUT2.Substring(position + 191, 24).Trim
                    dRow("DespatchDate") = Utilities.ISeriesDate(PARAMOUT2.Substring(position + 215, 7)).ToString("dd/MM/yy")
                    dRow("Quantity") = 1
                    dtCustomerReservations.Rows.Add(dRow)
                    position = position + 250
                Loop
                sMore = PARAMOUT2.Substring(32718, 1).Trim
                sLastRec = dtCustomerReservations.Rows.Count.ToString
                If sMore <> "Y" Then boolCallWS018R = False
                If PARAMOUT.Substring(1023, 1) = GlobalConstants.ERRORFLAG Or PARAMOUT.Substring(1021, 2).Trim <> "" Then boolCallWS018R = False
            Loop

            If PARAMOUT.Substring(1023, 1) = GlobalConstants.ERRORFLAG Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                dRow = dtStatusResults.NewRow
                dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                dtStatusResults.Rows.Add(dRow)
            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBReservations-WS018R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    ''' <summary>
    ''' Unreserves seperate reservations or collections of reservations
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function AccessDatabaseWS628R() As ErrorObj

        Dim err As New ErrorObj
        Dim PARAMOUT1 As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing

        'Create the basket data tables
        _basketFunctions.CreateBasketTables(ResultDataSet)

        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Status data table
        Dim dtReservationResults As New DataTable("ReservationsInfo")
        ResultDataSet.Tables.Add(dtReservationResults)
        With dtReservationResults.Columns
            .Add("ReservationReference", GetType(String))
        End With


        Try

            CallWS628R(PARAMOUT1, PARAMOUT2, PARAMOUTBasket)

            dRow = Nothing
            If Not (PARAMOUT1.Substring(1021, 1) = GlobalConstants.ERRORFLAG Or PARAMOUT1.Substring(1022, 2).Trim <> "") Then
                dRow = dtReservationResults.NewRow
                dRow("ReservationReference") = PARAMOUT1.Substring(995, 5).TrimStart("0")
                dtReservationResults.Rows.Add(dRow)
            Else
                dRow = dtStatusResults.NewRow
                dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                dRow("ReturnCode") = PARAMOUT1.Substring(1022, 2)
                dtStatusResults.Rows.Add(dRow)
            End If

            ' Retrieve the ticketing shopping basket
            _basketFunctions.RetrieveTicketingShoppingBasketValues(PARAMOUTBasket, ResultDataSet, err, "WS628R")

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBReservations-WS628R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    ''' <summary>
    ''' Reserves baskets
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Function AccessDatabaseWS627R() As ErrorObj

        Dim err As New ErrorObj
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing

        'Create the basket data tables
        _basketFunctions.CreateBasketTables(ResultDataSet)

        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Status data table
        Dim dtReservationResults As New DataTable("ReservationsInfo")
        ResultDataSet.Tables.Add(dtReservationResults)
        With dtReservationResults.Columns
            .Add("ReservationReference", GetType(String))
        End With

        Try

            CallWS627R(PARAMOUT, PARAMOUTBasket)

            dRow = Nothing
            If Not (PARAMOUT.Substring(1021, 1) = GlobalConstants.ERRORFLAG Or PARAMOUT.Substring(1021, 2).Trim <> "") Then
                dRow = dtReservationResults.NewRow
                dRow("ReservationReference") = PARAMOUT.Substring(995, 15).TrimStart("0")
                dtReservationResults.Rows.Add(dRow)
            Else
                dRow = dtStatusResults.NewRow
                dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                dRow("ReturnCode") = PARAMOUT.Substring(1022, 2)
                dtStatusResults.Rows.Add(dRow)
            End If

            ' Retrieve the ticketing shopping basket
            _basketFunctions.RetrieveTicketingShoppingBasketValues(PARAMOUTBasket, ResultDataSet, err, "WS627R")

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBReservations-WS627R"
                .HasError = True
            End With
        End Try

        Return err

    End Function
#End Region

#Region "Private Functions"
    'Add to basket functions
    Private Sub CallWS018R(ByRef PARAMOUT As String, ByRef PARAMOUT2 As String, ByVal sLastRec As String)

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS018R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS018Parm1()
        parmIO.Direction = ParameterDirection.InputOutput

        'Populate the parameter
        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 32765)
        parmIO2.Value = WS018Parm2(sLastRec)
        parmIO2.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS018R", DeReservations.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString

        TalentCommonLog("CallWS018R", DeReservations.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT)
    End Sub

    Protected Function AccessDatabaseWS626R() As ErrorObj

        Dim err As New ErrorObj
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        Dim PARAMOUTBasket As String = String.Empty
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing

        'Create the basket data tables
        _basketFunctions.CreateBasketTables(ResultDataSet)

        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try

            CallWS626R(PARAMOUT, PARAMOUT2, PARAMOUTBasket)

            If PARAMOUT.Substring(1023, 1) = GlobalConstants.ERRORFLAG Or PARAMOUT.Substring(1021, 2).Trim <> "" Then
                dRow = dtStatusResults.NewRow
                dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                dtStatusResults.Rows.Add(dRow)
            End If

            _basketFunctions.RetrieveTicketingShoppingBasketValues(PARAMOUTBasket, ResultDataSet, err, "WS626R")

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBReservations-WS626R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Sub CallWS626R(ByRef PARAMOUT As String, ByRef PARAMOUT2 As String, ByRef PARAMOUTBasket As String)

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS626R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2, @PARAM3)"
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim parmIOBasket As iDB2Parameter

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS018Parm1()
        parmIO.Direction = ParameterDirection.InputOutput

        'Populate the parameter
        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 32765)
        parmIO2.Value = WS018Parm2("")
        parmIO2.Direction = ParameterDirection.InputOutput

        parmIOBasket = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 5120)
        parmIOBasket.Value = Utilities.FixStringLength("", 5120)
        parmIOBasket.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS626R", DeReservations.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString
        PARAMOUTBasket = cmdSELECT.Parameters(Param3).Value.ToString

        TalentCommonLog("CallWS626R", DeReservations.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT)
    End Sub

    Private Function WS018Parm1() As String
        Dim stringPassedToDatabase As New StringBuilder
        stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.SessionID, 36))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 1))
        stringPassedToDatabase.Append(Utilities.PadLeadingZeros(DeReservations.CustomerNumber, 12))
        stringPassedToDatabase.Append(Utilities.FixStringLength(Settings.OriginatingSource, 10))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 11))
        stringPassedToDatabase.Append(Utilities.FixStringLength(ConvertToYN(DeReservations.ReturnSeatDetails), 1))
        stringPassedToDatabase.Append(Utilities.FixStringLength(ConvertToYN(DeReservations.AddToBasket), 1))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 916))
        stringPassedToDatabase.Append(Utilities.FixStringLength(ConvertToYN(DeReservations.ByPassPreReqCheck), 1))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 13))
        stringPassedToDatabase.Append(Utilities.PadLeadingZeros(DeReservations.CustomerNumber, 12))
        stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.ProductCode, 6))
        stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.Source, 1))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 3))

        Return stringPassedToDatabase.ToString()
    End Function

    Private Function WS018Parm2(ByVal sLastRec As String) As String
        If DeReservations.Seats.Count = 0 Then
            Dim ret As New StringBuilder
            ret.Append(Utilities.FixStringLength("", 32719))
            ret.Append(Utilities.PadLeadingZeros(sLastRec, 7))
            ret.Append(Utilities.FixStringLength("", 39))
            Return ret.ToString()
        End If
        Dim seats As List(Of DESeatDetails) = DeReservations.Seats
        Dim stringPassedToDatabase As New StringBuilder
        For Each seat As DESeatDetails In DeReservations.Seats
            stringPassedToDatabase.Append(Utilities.FixStringLength(seat.ProductCode, 6))
            stringPassedToDatabase.Append(Utilities.FixStringLength(seat.Stand, 3))
            stringPassedToDatabase.Append(Utilities.FixStringLength(seat.Area, 4))
            stringPassedToDatabase.Append(Utilities.FixStringLength(seat.Row, 4))
            stringPassedToDatabase.Append(Utilities.FixStringLength(seat.Seat, 4))
            stringPassedToDatabase.Append(Utilities.FixStringLength(seat.AlphaSuffix, 1))
            If stringPassedToDatabase.ToString().Length = 31375 Then
                Exit For
            End If
        Next
        Dim emptyString As String = 32765 - stringPassedToDatabase.ToString().Length
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, emptyString))
        Return stringPassedToDatabase.ToString()
    End Function

    'Unreserve functions
    Private Sub CallWS628R(ByRef PARAMOUT1 As String, ByRef PARAMOUT2 As String, ByRef PARAMOUTBasket As String)

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS628R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2, @PARAM3)"
        Dim parmIO1 As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim parmIOBasket As iDB2Parameter

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO1.Value = WS271RParm1()
        parmIO1.Direction = ParameterDirection.InputOutput

        'Populate the parameter
        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 32765)
        parmIO2.Value = WS271RParm2()
        parmIO2.Direction = ParameterDirection.InputOutput

        parmIOBasket = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 5120)
        parmIOBasket.Value = Utilities.FixStringLength("", 5120)
        parmIOBasket.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS628R", DeReservations.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO1.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT1 = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString
        PARAMOUTBasket = cmdSELECT.Parameters(Param3).Value.ToString

        TalentCommonLog("CallWS628R", DeReservations.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT1)
    End Sub

    Public Function AccessDatabaseWS271R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        Dim dRow As DataRow = Nothing

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Status data table
        Dim dtReservationResults As New DataTable("ReservationsInfo")
        ResultDataSet.Tables.Add(dtReservationResults)
        With dtReservationResults.Columns
            .Add("ReservationReference", GetType(String))
        End With

        Try
            CallWS271R(PARAMOUT, PARAMOUT2)
            dRow = Nothing
            If Not (PARAMOUT.Substring(1021, 1) = GlobalConstants.ERRORFLAG Or PARAMOUT.Substring(1022, 2).Trim <> "") Then
                dRow = dtReservationResults.NewRow
                dRow("ReservationReference") = PARAMOUT.Substring(995, 5).TrimStart("0")
                dtReservationResults.Rows.Add(dRow)
            Else
                dRow = dtStatusResults.NewRow
                dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                dRow("ReturnCode") = PARAMOUT.Substring(1022, 2)
                dtStatusResults.Rows.Add(dRow)
            End If
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBReservations-WS271R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Sub CallWS271R(ByRef PARAMOUT As String, ByRef PARAMOUT2 As String)
        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS271R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS271RParm1()
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 32765)
        parmIO2.Value = WS271RParm2()
        parmIO2.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS271R", DeReservations.Agent, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()

        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString

        TalentCommonLog("CallWS271R", DeReservations.Agent, "Backend Response: PARAMOUT=" & PARAMOUT)
    End Sub

    Private Function WS271RParm1() As String
        If DeReservations.UnreserveAll Then
            Dim stringPassedToDatabase As New StringBuilder
            stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.SessionID, 36))
            stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.Agent, 10))
            stringPassedToDatabase.Append(Utilities.ConvertToYN(DeReservations.UnreserveAll))
            stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.ProductCode, 6))
            stringPassedToDatabase.Append(Utilities.PadLeadingZeros(DeReservations.CustomerNumber, 12))
            stringPassedToDatabase.Append(Utilities.PadLeadingZeros(String.Empty, 15))
            stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.Comment, 500))
            stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 424))
            stringPassedToDatabase.Append(Utilities.FixStringLength(ConvertToYN(DeReservations.ByPassPreReqCheck), 1))
            stringPassedToDatabase.Append(Utilities.PadLeadingZeros(DeReservations.NumberOfSeatsReserved, 5))
            stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.Agent, 10))
            stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.Source, 1))
            stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 3))
            Return stringPassedToDatabase.ToString()
        Else
            Dim stringPassedToDatabase As New StringBuilder
            stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 1010))
            stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.Agent, 10))
            stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.Source, 1))
            stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 3))
            Return stringPassedToDatabase.ToString()
        End If
    End Function

    Private Function WS271RParm2() As String
        If DeReservations.UnreserveAll Then
            Return String.Empty
        Else
            Dim seats As List(Of DESeatDetails) = DeReservations.Seats
            Dim stringPassedToDatabase As New StringBuilder
            For Each seat As DESeatDetails In DeReservations.Seats
                stringPassedToDatabase.Append(Utilities.FixStringLength(seat.ProductCode, 6))
                stringPassedToDatabase.Append(Utilities.FixStringLength(seat.Stand, 3))
                stringPassedToDatabase.Append(Utilities.FixStringLength(seat.Area, 4))
                stringPassedToDatabase.Append(Utilities.FixStringLength(seat.Row, 4))
                stringPassedToDatabase.Append(Utilities.FixStringLength(seat.Seat, 4))
                stringPassedToDatabase.Append(Utilities.FixStringLength(seat.AlphaSuffix, 1))
                If stringPassedToDatabase.ToString().Length = 32538 Then
                    Exit For
                End If
            Next
            Dim emptyString As String = 32765 - stringPassedToDatabase.ToString().Length
            stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, emptyString))
            Return stringPassedToDatabase.ToString()
        End If
    End Function

    ' Reserve functions

    Private Sub CallWS627R(ByRef PARAMOUT As String, ByRef PARAMOUTBasket As String)

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS627R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1, @PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim parmIOBasket As iDB2Parameter

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS270RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        parmIOBasket = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 5120)
        parmIOBasket.Value = Utilities.FixStringLength("", 5120)
        parmIOBasket.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS627R", DeReservations.CustomerNumber, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARAMOUTBasket = cmdSELECT.Parameters(Param2).Value.ToString

        TalentCommonLog("CallWS627R", DeReservations.CustomerNumber, "Backend Response: PARAMOUT=" & PARAMOUT)
    End Sub

    Public Function AccessDatabaseWS270R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim PARAMOUT As String = String.Empty
        Dim dRow As DataRow = Nothing

        'Create the Status data table
        Dim dtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Status data table
        Dim dtReservationResults As New DataTable("ReservationsInfo")
        ResultDataSet.Tables.Add(dtReservationResults)
        With dtReservationResults.Columns
            .Add("ReservationReference", GetType(String))
        End With

        Try
            CallWS270R(PARAMOUT)
            dRow = Nothing
            If Not (PARAMOUT.Substring(1022, 1) = GlobalConstants.ERRORFLAG Or PARAMOUT.Substring(1021, 2).Trim <> "") Then
                dRow = dtReservationResults.NewRow
                dRow("ReservationReference") = PARAMOUT.Substring(995, 15).TrimStart("0")
                dtReservationResults.Rows.Add(dRow)
            Else
                dRow = dtStatusResults.NewRow
                dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                dtStatusResults.Rows.Add(dRow)
            End If


        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBReservations-WS270R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS270R(ByRef PARAMOUT As String) As String
        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS270R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter


        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS270RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS270R", DeReservations.Agent, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()

        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS270R", DeReservations.Agent, "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT
    End Function

    Private Function WS270RParm() As String
        Dim stringPassedToDatabase As New StringBuilder

        stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.SessionID, 36))
        stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.Agent, 10))
        stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.ExpiryDate, 8))
        stringPassedToDatabase.Append(Utilities.TimeToIseriesFormat(DeReservations.ExpiryTime))
        stringPassedToDatabase.Append(ConvertToYN(DeReservations.SaleOrReturn))
        stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.Comment, 500))
        stringPassedToDatabase.Append(Utilities.PadLeadingZeros(DeReservations.CallId, 13))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 415))
        stringPassedToDatabase.Append(Utilities.FixStringLength(ConvertToYN(DeReservations.ByPassPreReqCheck), 1))
        stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.NumberOfSeatsReserved, 5))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 15))
        stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.Agent, 10))
        stringPassedToDatabase.Append(Utilities.FixStringLength(DeReservations.Source, 1))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 3))
        Return stringPassedToDatabase.ToString()

    End Function


#End Region

End Class
