Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports Talent.Common
Imports Talent.Common.Utilities
Imports System.Text

<Serializable()> _
Public Class DBSmartcard
    Inherits DBAccess

    Private _de As DESmartcard
    Public Property DE() As DESmartcard
        Get
            Return _de
        End Get
        Set(ByVal value As DESmartcard)
            _de = value
        End Set
    End Property


    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case "RetrieveSmartcardCardDetails"
                err = AccessDatabase_WS170R()
            Case "CallSmartcardFunction"
                err = AccessDatabase_WS171R()
            Case "RetrieveSmartcardCards"
                err = AccessDatabase_WS174R()
            Case "RetrieveDEXList"
                err = AccessDatabase_TC521R()
            Case "RetrieveAvailableFanIdList"
                err = AccessDatabase_WS102R()
            Case "UpdateCurrentFanId"
                err = AccessDatabase_WS103R()
            Case "RequestPrintCard"
                err = AccessDatabase_WS632R()
            Case "ExternalSmartcardReprintRequest"
                err = AccessDatabase_FW060R()
        End Select

        Return err
    End Function
    Protected Function AccessDatabase_WS632R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        Dim PARAMOUT As String = String.Empty
        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorFlag", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtSmartcardHeaderDataResults As New DataTable("TeamCardResults")
        ResultDataSet.Tables.Add(DtSmartcardHeaderDataResults)
        With DtSmartcardHeaderDataResults.Columns
            .Add("SessionID", GetType(String))
            .Add("Mode", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("Stand", GetType(String))
            .Add("Area", GetType(String))
            .Add("Row", GetType(String))
            .Add("Seat", GetType(String))
            .Add("Alpha", GetType(String))
            .Add("PaymentReference", GetType(String))
            .Add("NewCardNumberID", GetType(String))
            .Add("ExistingCardNumberID", GetType(String))

        End With

        Try
            PARAMOUT = CallWS632R()

            Dim TeamCardResult As DataRow = DtSmartcardHeaderDataResults.NewRow
            TeamCardResult("SessionID") = PARAMOUT.Substring(0, 36)
            TeamCardResult("Mode") = PARAMOUT.Substring(36, 1)
            TeamCardResult("ProductCode") = PARAMOUT.Substring(37, 6)
            TeamCardResult("Stand") = PARAMOUT.Substring(43, 3)
            TeamCardResult("Area") = PARAMOUT.Substring(46, 4)
            TeamCardResult("Row") = PARAMOUT.Substring(50, 4)
            TeamCardResult("Seat") = PARAMOUT.Substring(54, 4)
            TeamCardResult("Alpha") = PARAMOUT.Substring(58, 1)
            TeamCardResult("PaymentReference") = PARAMOUT.Substring(59, 15)
            TeamCardResult("NewCardNumberID") = PARAMOUT.Substring(74, 14)
            TeamCardResult("ExistingCardNumberID") = PARAMOUT.Substring(88, 14)
            DtSmartcardHeaderDataResults.Rows.Add(TeamCardResult)

            Dim ErrorStatusResult As DataRow = DtStatusResults.NewRow
            ErrorStatusResult("ReturnCode") = PARAMOUT.Substring(1021, 1)
            ErrorStatusResult("ErrorFlag") = PARAMOUT.Substring(1022, 2)
            DtStatusResults.Rows.Add(ErrorStatusResult)

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBSC-01-WS632R"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Protected Function AccessDatabase_WS170R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("DtStatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Smartcard Data Header Data Table
        Dim DtSmartcardHeaderDataResults As New DataTable("DtSmartcardHeaderDataResults")
        ResultDataSet.Tables.Add(DtSmartcardHeaderDataResults)
        With DtSmartcardHeaderDataResults.Columns
            .Add("ProduceCardOptionAvailable", GetType(Boolean))
            .Add("ReprintCardOptionAvailable", GetType(Boolean))
            .Add("CancelCardOptionAvailable", GetType(Boolean))
            .Add("Printers", GetType(ArrayList))
        End With

        'Create the Ticketing Sales Data Table
        Dim DtTicketingSalesResults As New DataTable("DtTicketingSalesResults")
        ResultDataSet.Tables.Add(DtTicketingSalesResults)
        With DtTicketingSalesResults.Columns
            .Add("ActiveSale", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("SoldDate", GetType(String))
            .Add("CancelledDate", GetType(String))
            .Add("SoldBy", GetType(String))
            .Add("CancelledBy", GetType(String))
            .Add("SaleRecordID", GetType(String))
            .Add("SCRecordID", GetType(String))
            .Add("SCUploadType", GetType(String))
            .Add("SCUploadEnabled", GetType(Boolean))
            .Add("SCPrintEnabled", GetType(Boolean))
        End With

        'Create the Smartcard Card Data Table
        Dim DtSmartcardCardDataResults As New DataTable("DtSmartcardCardDataResults")
        ResultDataSet.Tables.Add(DtSmartcardCardDataResults)
        With DtSmartcardCardDataResults.Columns
            .Add("ActiveCard", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("CardNumber", GetType(String))
            .Add("Card_ActivatedDate", GetType(String))
            .Add("Card_DeactivatedDate", GetType(String))
            .Add("CardRecordID", GetType(String))
        End With

        'Create the Smartcard Error Log Data Table
        Dim DtSmartcardErrorLogResults As New DataTable("DtSmartcardErrorLogResults")
        ResultDataSet.Tables.Add(DtSmartcardErrorLogResults)
        With DtSmartcardErrorLogResults.Columns
            .Add("ErrorDate", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("CardNumber", GetType(String))
            .Add("RequestAction", GetType(String))
            .Add("ErrorNumber", GetType(String))
            .Add("ErrorDescription", GetType(String))
        End With

        Try
            PARAMOUT = CallWS170R()

            '
            'Set the status data table from the program call
            '
            Dim sRow As DataRow
            sRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(5119, 1) = "E" Then
                sRow("ErrorOccurred") = "E"
            Else
                sRow("ErrorOccurred") = ""
            End If
            sRow("ReturnCode") = PARAMOUT.Substring(5117, 2)
            DtStatusResults.Rows.Add(sRow)

            '
            'If no errors then populate data tables with results
            '
            If PARAMOUT.Substring(5119, 1) <> "E" Then

                '
                'Create the Smartcard Header Data Row
                '
                Dim hRow As DataRow
                hRow = DtSmartcardHeaderDataResults.NewRow

                'Retrieve available printers
                Dim printers As ArrayList = New ArrayList
                Dim iStart As Integer = 4000
                Dim iInc As Integer = 10
                Dim iCounter As Integer = 0

                Do While iCounter <= 49
                    If PARAMOUT.Substring(iStart, iInc).Trim <> "" Then
                        printers.Insert(iCounter, PARAMOUT.Substring(iStart, iInc))
                    Else
                        Exit Do
                    End If

                    'Increment
                    iStart += iInc
                    iCounter += 1
                Loop

                hRow("Printers") = printers
                hRow("ProduceCardOptionAvailable") = False
                hRow("ReprintCardOptionAvailable") = False
                hRow("CancelCardOptionAvailable") = False

                If PARAMOUT.Substring(5091, 1) = "Y" Then
                    hRow("ProduceCardOptionAvailable") = True
                End If
                If PARAMOUT.Substring(5092, 1) = "Y" Then
                    hRow("ReprintCardOptionAvailable") = True
                End If
                If PARAMOUT.Substring(5093, 1) = "Y" Then
                    hRow("CancelCardOptionAvailable") = True
                End If

                DtSmartcardHeaderDataResults.Rows.Add(hRow)

                '
                ' Create Talent Sales data rows
                '
                iStart = 0
                iInc = 100
                iCounter = 1
                Do While iCounter <= 10

                    ' Has a record been returned
                    If PARAMOUT.Substring(iStart, 1).Trim = "" Then
                        Exit Do
                    Else
                        Dim dRow As DataRow = DtTicketingSalesResults.NewRow

                        dRow("ActiveSale") = PARAMOUT.Substring(iStart, 1)
                        dRow("ProductCode") = PARAMOUT.Substring(iStart + 1, 6)
                        dRow("SoldDate") = PARAMOUT.Substring(iStart + 7, 10)
                        dRow("CancelledDate") = PARAMOUT.Substring(iStart + 17, 10)
                        dRow("SoldBy") = PARAMOUT.Substring(iStart + 27, 10)
                        dRow("CancelledBy") = PARAMOUT.Substring(iStart + 37, 10)
                        dRow("SaleRecordID") = PARAMOUT.Substring(iStart + 47, 15)
                        dRow("SCRecordID") = PARAMOUT.Substring(iStart + 62, 15)
                        dRow("SCUploadType") = PARAMOUT.Substring(iStart + 77, 1)
                        If PARAMOUT.Substring(iStart + 78, 1) = "Y" Then
                            dRow("SCUploadEnabled") = True
                        End If
                        If PARAMOUT.Substring(iStart + 79, 1) = "Y" Then
                            dRow("SCPrintEnabled") = True
                        End If

                        DtTicketingSalesResults.Rows.Add(dRow)

                        'Increment
                        iStart += iInc
                        iCounter += 1
                    End If
                Loop

                '
                ' Create Smartcard Card Data rows
                '
                iStart = 1000
                iInc = 100
                iCounter = 1
                Do While iCounter <= 10

                    ' Has a record been returned
                    If PARAMOUT.Substring(iStart, 1).Trim = "" Then
                        Exit Do
                    Else
                        Dim dRow As DataRow = DtSmartcardCardDataResults.NewRow

                        dRow("ActiveCard") = PARAMOUT.Substring(iStart, 1)
                        dRow("ProductCode") = PARAMOUT.Substring(iStart + 1, 6)
                        dRow("CardNumber") = PARAMOUT.Substring(iStart + 7, 14)
                        dRow("Card_ActivatedDate") = PARAMOUT.Substring(iStart + 21, 10)
                        dRow("Card_DeactivatedDate") = PARAMOUT.Substring(iStart + 31, 10)
                        dRow("CardRecordID") = PARAMOUT.Substring(iStart + 41, 15)

                        DtSmartcardCardDataResults.Rows.Add(dRow)

                        'Increment
                        iStart += iInc
                        iCounter += 1
                    End If
                Loop

                '
                ' Create Smartcard Error Log Data rows
                '
                iStart = 2000
                iInc = 200
                iCounter = 1
                Do While iCounter <= 10

                    ' Has a record been returned
                    If PARAMOUT.Substring(iStart, 1).Trim = "" Then
                        Exit Do
                    Else
                        Dim dRow As DataRow = DtSmartcardErrorLogResults.NewRow

                        dRow("ErrorDate") = PARAMOUT.Substring(iStart, 10)
                        dRow("ProductCode") = PARAMOUT.Substring(iStart + 10, 6)
                        dRow("CardNumber") = PARAMOUT.Substring(iStart + 16, 14)
                        dRow("RequestAction") = PARAMOUT.Substring(iStart + 30, 1)
                        dRow("ErrorNumber") = PARAMOUT.Substring(iStart + 31, 10)
                        dRow("ErrorDescription") = PARAMOUT.Substring(iStart + 41, 159)

                        DtSmartcardErrorLogResults.Rows.Add(dRow)

                        'Increment
                        iStart += iInc
                        iCounter += 1
                    End If
                Loop
            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBSC-01-WS170R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function CallWS632R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS632R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"

        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS632Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS632R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS632R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT
    End Function
    Private Function CallWS170R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS170R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS170Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS170R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS170R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function
    Private Function WS632Parm() As String
        Dim parameter As New StringBuilder
        parameter.Append(Utilities.FixStringLength(_de.SessionID, 36))
        parameter.Append(Utilities.FixStringLength(_de.Mode, 1))
        parameter.Append(Utilities.FixStringLength(_de.ProductCode, 6))
        parameter.Append(Utilities.FixStringLength(_de.Stand, 3))
        parameter.Append(Utilities.FixStringLength(_de.Area, 4))
        parameter.Append(Utilities.FixStringLength(_de.Row, 4))
        parameter.Append(Utilities.FixStringLength(_de.Seat, 4))
        parameter.Append(Utilities.FixStringLength(_de.Alpha, 1))
        parameter.Append(Utilities.PadLeadingZeros(CStr(_de.PaymentReference), 15))
        parameter.Append(Utilities.FixStringLength(String.Empty, 14))
        parameter.Append(Utilities.FixStringLength(_de.ExistingCardNumber, 14))
        parameter.Append(Utilities.FixStringLength(String.Empty, 920))
        parameter.Append(Utilities.FixStringLength(_de.ErrorFlag, 1))
        parameter.Append(Utilities.FixStringLength(_de.ErrorCode, 1))
        Return parameter.ToString()
    End Function
    Private Function WS170Parm() As String

        Dim myString As String

        Dim webServiceUser As String = String.Empty
        If Not Settings.OriginatingSource.Equals(String.Empty) Then
            webServiceUser = Settings.OriginatingSource
        Else
            webServiceUser = Settings.Company
        End If

        Dim AgentFilter, InactiveProducts, ActiveProducts, EventProducts, CardProducts As String
        If DE.AgentFilter Then AgentFilter = "Y" Else AgentFilter = "N"
        If DE.InactiveProducts Then InactiveProducts = "Y" Else InactiveProducts = "N"
        If DE.ActiveProducts Then ActiveProducts = "Y" Else ActiveProducts = "N"
        If DE.EventProducts Then EventProducts = "Y" Else EventProducts = "N"
        If DE.CardProducts Then CardProducts = "Y" Else CardProducts = "N"

        myString = Utilities.FixStringLength("", 5086) & _
                 Utilities.PadLeadingZeros(AgentFilter, 1) & _
                 Utilities.PadLeadingZeros(InactiveProducts, 1) & _
                 Utilities.PadLeadingZeros(ActiveProducts, 1) & _
                 Utilities.PadLeadingZeros(EventProducts, 1) & _
                 Utilities.PadLeadingZeros(CardProducts, 1) & _
                 Utilities.FixStringLength("", 3) & _
                 Utilities.PadLeadingZeros(DE.CustomerNumber, 12) & _
                 Utilities.FixStringLength(webServiceUser, 10) & _
                 Utilities.FixStringLength(DE.Src, 1) & _
                 Utilities.FixStringLength("", 3)

        Return myString

    End Function

    Protected Function AccessDatabase_WS171R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            PARAMOUT = CallWS171R()

            'Set the response data
            Dim sRow As DataRow
            sRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" Then
                sRow("ErrorOccurred") = "E"
            Else
                sRow("ErrorOccurred") = ""
            End If
            sRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
            DtStatusResults.Rows.Add(sRow)

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBSC-02-WS171R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Protected Function AccessDatabase_FW060R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtResults As New DataTable("SmartcardExternalReprintResults")
        ResultDataSet.Tables.Add(DtResults)
        With DtResults.Columns
            .Add("CustomerNumber", GetType(String))
            .Add("CustomerNumberWithPrefix", GetType(String))
            .Add("SmartcardNumber", GetType(String))
            .Add("ReprintStatusCode", GetType(String))
            .Add("ReprintStatusDescription", GetType(String))
        End With

        Try
            PARAMOUT = CallFW060R()

            'Set the response data
            Dim sRow As DataRow
            sRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(32764, 1) = "E" Then
                sRow("ErrorOccurred") = "E"
            Else
                sRow("ErrorOccurred") = ""
                populateExternalReprintResults(PARAMOUT, DtResults)
            End If
            sRow("ReturnCode") = PARAMOUT.Substring(32762, 2)
            DtStatusResults.Rows.Add(sRow)

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBSC-01-FW060R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function CallFW060R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "FW060R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 32765)
        parmIO.Value = FW060RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallFW060R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallFW060R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function FW060RParm() As String

        Dim membArrayPos As Integer = 0
        Dim membPrefixArrayPos As Integer = 3000
        Dim oldCardArrayPos As Integer = 6750
        Dim newCardArrayPos As Integer = 10250
        Dim productCodeArrayPos As Integer = 13750
        Dim stadiumCodeArrayPos As Integer = 15250
        Dim standCodeArrayPos As Integer = 15750
        Dim areaCodeArrayPos As Integer = 16500
        Dim rowCodeArrayPos As Integer = 17500
        Dim seatNumberArrayPos As Integer = 18500
        Dim seatSuffixArrayPos As Integer = 19500

        Dim sb As StringBuilder = New StringBuilder(Utilities.FixStringLength(String.Empty, 32765))

        For Each externalReprintRequest As DESmartcardExternalReprint In DE.ExternalReprintRequests
            sb.Remove(membArrayPos, 12).Insert(membArrayPos, Utilities.PadLeadingZeros(externalReprintRequest.CustomerNumber, 12))
            sb.Remove(membPrefixArrayPos, 15).Insert(membPrefixArrayPos, Utilities.FixStringLength(externalReprintRequest.PrefixedCustomerNumber, 15))
            sb.Remove(oldCardArrayPos, 14).Insert(oldCardArrayPos, Utilities.FixStringLength(externalReprintRequest.OldSmartcardNumber, 14))
            sb.Remove(newCardArrayPos, 14).Insert(newCardArrayPos, Utilities.FixStringLength(externalReprintRequest.SmartcardNumber, 14))
            sb.Remove(productCodeArrayPos, 6).Insert(productCodeArrayPos, Utilities.FixStringLength(externalReprintRequest.ProductCode, 6))
            sb.Remove(stadiumCodeArrayPos, 2).Insert(stadiumCodeArrayPos, Utilities.FixStringLength(externalReprintRequest.StadiumCode, 2))
            sb.Remove(standCodeArrayPos, 3).Insert(standCodeArrayPos, Utilities.FixStringLength(externalReprintRequest.StandCode, 3))
            sb.Remove(areaCodeArrayPos, 4).Insert(areaCodeArrayPos, Utilities.FixStringLength(externalReprintRequest.AreaCode, 4))
            sb.Remove(rowCodeArrayPos, 4).Insert(rowCodeArrayPos, Utilities.FixStringLength(externalReprintRequest.Row, 4))
            sb.Remove(seatNumberArrayPos, 4).Insert(seatNumberArrayPos, Utilities.PadLeadingZeros(externalReprintRequest.Seat, 4))
            sb.Remove(seatSuffixArrayPos, 1).Insert(seatSuffixArrayPos, Utilities.FixStringLength(externalReprintRequest.AlphaSeat, 1))

            membArrayPos += 12
            membPrefixArrayPos += 15
            oldCardArrayPos += 14
            newCardArrayPos += 14
            productCodeArrayPos += 6
            stadiumCodeArrayPos += 2
            standCodeArrayPos += 3
            areaCodeArrayPos += 4
            rowCodeArrayPos += 4
            seatNumberArrayPos += 4
            seatSuffixArrayPos += 1
        Next
        sb.Remove(32761, 1).Insert(32761, Utilities.FixStringLength(Settings.OriginatingSource, 1))
        Return sb.ToString()
    End Function

    Private Sub populateExternalReprintResults(ByVal FW060ROutPutParm As String, ByRef resultsTable As DataTable)

        Dim membArrayPos As Integer = 0
        Dim membPrefixArrayPos As Integer = 3000
        Dim newCardArrayPos As Integer = 10250
        Dim reprintReturnCodeArrayPos As Integer = 19750
        Dim reprintReturnDescArrayPos As Integer = 20250

        For Each externalReprintRequest As DESmartcardExternalReprint In DE.ExternalReprintRequests
            Dim sRow As DataRow = resultsTable.NewRow
            resultsTable.Rows.Add(sRow)
            sRow("CustomerNumber") = FW060ROutPutParm.Substring(membArrayPos, 12)
            sRow("CustomerNumberWithPrefix") = FW060ROutPutParm.Substring(membPrefixArrayPos, 15)
            sRow("SmartcardNumber") = FW060ROutPutParm.Substring(newCardArrayPos, 14)
            sRow("ReprintStatusCode") = FW060ROutPutParm.Substring(reprintReturnCodeArrayPos, 2)
            sRow("ReprintStatusDescription") = FW060ROutPutParm.Substring(reprintReturnDescArrayPos, 25)

            membArrayPos += 12
            membPrefixArrayPos += 15
            newCardArrayPos += 14
            reprintReturnCodeArrayPos += 2
            reprintReturnDescArrayPos += 25
        Next
    End Sub

    Private Function CallWS171R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS171R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS171Parm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        TalentCommonLog("CallWS171R", "", "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        TalentCommonLog("CallWS171R", "", "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS171Parm() As String

        Dim webServiceUser As String = String.Empty
        If Not Settings.OriginatingSource.Equals(String.Empty) Then
            webServiceUser = Settings.OriginatingSource
        Else
            webServiceUser = Settings.Company
        End If

        Dim myString As String = ""

        Dim MediaType As String = DE.MediaType
        ' If DE.MediaType = String.Empty Then MediaType = "Y"

        myString = Utilities.FixStringLength(DE.RequestType, 1) & _
        Utilities.FixStringLength(DE.ProductCode, 6) & _
        Utilities.FixStringLength(DE.Printer, 10) & _
        Utilities.FixStringLength(DE.CardNumber, 14) & _
        Utilities.FixStringLength(DE.SaleRecordID, 15) & _
        Utilities.FixStringLength(DE.CardRecordID, 15) & _
        Utilities.FixStringLength(DE.CustomerNumber, 12) & _
        Utilities.FixStringLength(MediaType, 1) & _
        Utilities.FixStringLength("", 936) & _
        Utilities.FixStringLength(webServiceUser, 10) & _
        Utilities.FixStringLength(DE.Src, 1) & _
        Utilities.FixStringLength("", 3)

        Return myString

    End Function

    Private Function AccessDatabase_WS174R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
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
        Dim dtPartPayments As New DataTable("Smartcards")
        ResultDataSet.Tables.Add(dtPartPayments)
        With dtPartPayments.Columns
            .Add("CustomerNo", GetType(String))
            .Add("FirstName", GetType(String))
            .Add("Surname", GetType(String))
            .Add("CardNumber", GetType(String))
            .Add("FanId", GetType(String))
            .Add("Last4Digits", GetType(String))
            .Add("IssueNumber", GetType(String))
            .Add("FanIdActive", GetType(String))
            .Add("SeasonCard", GetType(String))
        End With

        Try

            Do While MoreRecords

                'Call WS174R
                PARAMOUT = CallWS174R(LastRecord, RecordTotal)

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
                    Do While Counter <= 30

                        'Create a new row
                        If Not String.IsNullOrEmpty(PARAMOUT.Substring(Position, 12).Trim) Then
                            dRow = Nothing
                            dRow = dtPartPayments.NewRow
                            dRow("CustomerNo") = PARAMOUT.Substring(Position, 12).Trim
                            dRow("FirstName") = PARAMOUT.Substring(Position + 12, 20).Trim
                            dRow("Surname") = PARAMOUT.Substring(Position + 32, 30).Trim
                            dRow("CardNumber") = PARAMOUT.Substring(Position + 62, 14).Trim
                            dRow("FanId") = PARAMOUT.Substring(Position + 76, 16).Trim
                            dRow("Last4Digits") = PARAMOUT.Substring(Position + 92, 4).Trim
                            dRow("IssueNumber") = PARAMOUT.Substring(Position + 96, 2).Trim
                            dRow("FanIdActive") = PARAMOUT.Substring(Position + 98, 1).Trim
                            dRow("SeasonCard") = PARAMOUT.Substring(Position + 99, 1).Trim
                            dtPartPayments.Rows.Add(dRow)

                            'Increment
                            Position = Position + 100
                            Counter = Counter + 1
                        Else
                            MoreRecords = False
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
                .ErrorNumber = "TACDBSC-03-WS174R"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS174R(ByVal sRecordTotal As String, _
                                ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS174R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add("@Param1", iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS174Parm(sRecordTotal, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@Param1").Value.ToString

        Return PARAMOUT

    End Function

    Private Function WS174Parm(ByVal sRecordTotal As String, _
                                ByVal sLastRecord As String) As String

        Dim myString As String = String.Empty

        'Construct the parameter
        myString = Utilities.FixStringLength("", 3050)
        myString += Utilities.FixStringLength(DE.CustomerNumber, 12)
        myString += Utilities.FixStringLength(sRecordTotal, 3)
        myString += Utilities.FixStringLength(sLastRecord, 3)
        myString += Utilities.FixStringLength(DE.Src, 1)
        myString += Utilities.FixStringLength("", 3)

        Return myString

    End Function

    Private Function AccessDatabase_TC521R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim MoreRecords As Boolean = True
        Dim txs As String = String.Empty

        'Create the Status data table
        Dim dtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the results data table
        Dim dtResultsList As New DataTable("ListOfResults")
        ResultDataSet.Tables.Add(dtResultsList)
        With dtResultsList.Columns
            .Add("EventName", GetType(String))
            .Add("EventDate", GetType(String))
            .Add("EventID", GetType(String))
            .Add("VenueName", GetType(String))
            .Add("VenueID", GetType(String))
            .Add("CustomerName", GetType(String))
            .Add("BasketRef", GetType(String))
            .Add("TicketID", GetType(String))
            .Add("TicketSaleDate", GetType(String))
            .Add("TicketPrintDate", GetType(String))
            .Add("TicketSaleTerminal", GetType(String))
            .Add("Barcode", GetType(String))
            .Add("BarcodeType", GetType(String))
            .Add("AreaName", GetType(String))
            .Add("AreaID", GetType(String))
            .Add("SectionName", GetType(String))
            .Add("SectionID", GetType(String))
            .Add("RowNo", GetType(String))
            .Add("SeatNo", GetType(String))
            .Add("EntranceDesc", GetType(String))
            .Add("GateDesc", GetType(String))
            .Add("PriceType", GetType(String))
        End With

        Try
            Do While MoreRecords
                PARAMOUT = CallTC521R(txs)

                'Add the status row
                If String.IsNullOrEmpty(txs) Then
                    dRow = Nothing
                    dRow = dtStatusResults.NewRow
                    If PARAMOUT.Substring(5119, 1) = "E" Or PARAMOUT.Substring(5117, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(5117, 2)
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    dtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(5119, 1) <> "E" AndAlso PARAMOUT.Substring(5117, 2).Trim = "" Then
                    Dim Position As Integer = 17
                    Dim Counter As Integer = 1
                    Do While Counter <= 10
                        If Not String.IsNullOrEmpty(PARAMOUT.Substring(Position, 12).Trim) Then
                            dRow = Nothing
                            dRow = dtResultsList.NewRow
                            dRow("EventName") = PARAMOUT.Substring(Position, 40).Trim
                            dRow("EventDate") = PARAMOUT.Substring(Position + 40, 12).Trim
                            dRow("EventID") = PARAMOUT.Substring(Position + 52, 6).Trim
                            dRow("VenueName") = PARAMOUT.Substring(Position + 58, 30).Trim
                            dRow("VenueID") = PARAMOUT.Substring(Position + 88, 4).Trim
                            dRow("CustomerName") = PARAMOUT.Substring(Position + 92, 58).Trim
                            dRow("BasketRef") = PARAMOUT.Substring(Position + 150, 13).Trim
                            dRow("TicketID") = PARAMOUT.Substring(Position + 163, 13).Trim
                            dRow("TicketSaleDate") = PARAMOUT.Substring(Position + 176, 12).Trim
                            dRow("TicketPrintDate") = PARAMOUT.Substring(Position + 188, 12).Trim
                            dRow("TicketSaleTerminal") = PARAMOUT.Substring(Position + 200, 30).Trim
                            dRow("Barcode") = PARAMOUT.Substring(Position + 230, 120).Trim
                            dRow("BarcodeType") = PARAMOUT.Substring(Position + 350, 30).Trim
                            dRow("AreaName") = PARAMOUT.Substring(Position + 380, 30).Trim
                            dRow("AreaID") = PARAMOUT.Substring(Position + 410, 4).Trim
                            dRow("SectionName") = PARAMOUT.Substring(Position + 414, 30).Trim
                            dRow("SectionID") = PARAMOUT.Substring(Position + 444, 4).Trim
                            dRow("RowNo") = PARAMOUT.Substring(Position + 448, 4).Trim
                            dRow("SeatNo") = PARAMOUT.Substring(Position + 452, 4).Trim
                            dRow("EntranceDesc") = PARAMOUT.Substring(Position + 456, 12).Trim
                            dRow("GateDesc") = PARAMOUT.Substring(Position + 468, 12).Trim
                            dRow("PriceType") = PARAMOUT.Substring(Position + 480, 15).Trim
                            dtResultsList.Rows.Add(dRow)
                            Position = Position + 500
                            Counter = Counter + 1
                        Else
                            MoreRecords = False
                            Exit Do
                        End If
                    Loop
                    txs = PARAMOUT.Substring(5065, 49)
                    MoreRecords = (PARAMOUT.Substring(5116, 1) <> "1")
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
                .ErrorNumber = "TACDBSC-04-TC521R"
                .HasError = True
            End With
        End Try

        Return err
    End Function

    Private Function CallTC521R(ByVal txs As String) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "TC521R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add("@Param1", iDB2DbType.iDB2Char, 5120)
        parmIO.Value = TC521RParm(txs)
        parmIO.Direction = ParameterDirection.InputOutput
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@Param1").Value.ToString
        Return PARAMOUT
    End Function

    Private Function TC521RParm(ByVal txs As String) As String
        Dim webUser As String = String.Empty
        If String.IsNullOrEmpty(Settings.OriginatingSource) Then
            webUser = Settings.Company
        Else
            webUser = Settings.OriginatingSource
        End If
        Dim myString As New StringBuilder
        myString.Append(Utilities.FixStringLength(webUser, 10))
        myString.Append(Utilities.FixStringLength(DE.ProductCode, 6))
        myString.Append(Utilities.FixStringLength(DE.DEXListMode, 1))
        myString.Append(Utilities.FixStringLength("", 5000))
        myString.Append(Utilities.FixStringLength("", 48))
        myString.Append(Utilities.FixStringLength(txs, 49))
        myString.Append(Utilities.FixStringLength("", 6))
        Return myString.ToString()
    End Function

    Private Function AccessDatabase_WS102R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim dtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the results data table
        Dim dtFanIdList As New DataTable("ListOfFanIds")
        ResultDataSet.Tables.Add(dtFanIdList)
        With dtFanIdList.Columns
            .Add("AvailableFanId", GetType(String))
            .Add("LastFourCardDigits", GetType(String))
            .Add("CardIssueNumber", GetType(String))
        End With


        Try
            PARAMOUT = CallWS102R()

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

            'No errors 
            If PARAMOUT.Substring(3071, 1) <> "E" AndAlso PARAMOUT.Substring(3069, 2).Trim = "" Then
                Dim iPosition As Integer = 0
                Do While Not String.IsNullOrEmpty(PARAMOUT.Substring(iPosition, 1).Trim)
                    dRow = Nothing
                    dRow = dtFanIdList.NewRow
                    dRow("AvailableFanId") = PARAMOUT.Substring(iPosition, 16).Trim
                    dRow("LastFourCardDigits") = PARAMOUT.Substring(iPosition + 17, 4).Trim
                    dRow("CardIssueNumber") = PARAMOUT.Substring(iPosition + 22, 2).Trim
                    dtFanIdList.Rows.Add(dRow)

                    iPosition += 40
                    If iPosition > 3000 Then Exit Do
                Loop
            End If

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBSC-05-WS102R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS102R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS102R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add("@Param1", iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS102RParm()
        parmIO.Direction = ParameterDirection.InputOutput
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@Param1").Value.ToString
        Return PARAMOUT
    End Function

    Private Function WS102RParm() As String
        Dim myString As New System.Text.StringBuilder
        myString.Append(Utilities.FixStringLength("", 3050))
        myString.Append(Utilities.PadLeadingZeros(DE.CustomerNumber, 12))
        myString.Append(Utilities.FixStringLength("", 6))
        myString.Append(Utilities.FixStringLength(DE.Src, 1))
        myString.Append(Utilities.FixStringLength("", 3))
        Return myString.ToString()
    End Function

    Private Function AccessDatabase_WS103R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim dtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Try
            PARAMOUT = CallWS103R()

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

        Catch ex As Exception
            ResultDataSet = Nothing
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBSC-06-WS103R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS103R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS103R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
        parmIO = cmdSELECT.Parameters.Add("@Param1", iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS103RParm()
        parmIO.Direction = ParameterDirection.InputOutput
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@Param1").Value.ToString
        Return PARAMOUT
    End Function

    Private Function WS103RParm() As String
        Dim myString As New System.Text.StringBuilder

        'Populate the mode
        If String.IsNullOrEmpty(DE.OldFanId) Then
            myString.Append("A")
            myString.Append(Utilities.FixStringLength(DE.NewFanId, 16))
            myString.Append(Utilities.FixStringLength(DE.New4Digits, 4))
            myString.Append(Utilities.FixStringLength(DE.OldFanId, 16))
            myString.Append(Utilities.FixStringLength(DE.Old4Digits, 4))
            myString.Append(Utilities.FixStringLength("", 5))
            myString.Append(Utilities.FixStringLength(DE.NewIssueNumber, 2))
            myString.Append(Utilities.FixStringLength(DE.OldIssueNumber, 2))
        Else
            myString.Append("T")
            myString.Append(Utilities.FixStringLength(DE.OldFanId, 16))
            myString.Append(Utilities.FixStringLength(DE.Old4Digits, 4))
            myString.Append(Utilities.FixStringLength(DE.NewFanId, 16))
            myString.Append(Utilities.FixStringLength(DE.New4Digits, 4))
            myString.Append(Utilities.FixStringLength("", 5))
            myString.Append(Utilities.FixStringLength(DE.OldIssueNumber, 2))
            myString.Append(Utilities.FixStringLength(DE.NewIssueNumber, 2))
        End If

        myString.Append(Utilities.FixStringLength(DE.CardNumber, 14))
        myString.Append(Utilities.FixStringLength("", 2986))
        myString.Append(Utilities.PadLeadingZeros(DE.CustomerNumber, 12))
        myString.Append(Utilities.FixStringLength("", 6))
        myString.Append(Utilities.FixStringLength(DE.Src, 1))
        myString.Append(Utilities.FixStringLength("", 3))
        Return myString.ToString()
    End Function
End Class
