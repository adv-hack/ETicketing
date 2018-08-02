Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
Imports Talent.Common

<Serializable()> _
Public Class DBPPS
    Inherits DBAccess
    Private _dePPS As DEPPS
    Private _dePPSEnrolmentScheme As DEPPSEnrolmentScheme
    Private Const AddPPSRequest As String = "AddPPSRequest"
    Private Const AmendPPS As String = "AmendPPS"
    Private Const RetrievePPSPaymentDetails As String = "RetrievePPSPaymentDetails"
    Private Const CancelPPSEnrollment As String = "CancelPPSEnrollment"
   

   

    Public Property dePPS() As DEPPS
        Get
            Return _dePPS
        End Get
        Set(ByVal value As DEPPS)
            _dePPS = value
        End Set
    End Property

    Public Property dePPSEnrolmentScheme() As DEPPSEnrolmentScheme
        Get
            Return _dePPSEnrolmentScheme
        End Get
        Set(ByVal value As DEPPSEnrolmentScheme)
            _dePPSEnrolmentScheme = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj

        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = AddPPSRequest : err = AccessDatabaseWS614R()
            Case Is = AmendPPS : err = AccessDatabaseWS617R()
            Case Is = CancelPPSEnrollment : err = AccessDatabaseWS614R()
        End Select

        Return err

    End Function

    Private Function AccessDatabaseWS614R() As ErrorObj
        Dim err As New ErrorObj

        If dePPS.CancelEnrolMode Then
            err = AccessDatabaseWS614R_cancelPPSEnrolmentMode()
        ElseIf dePPS.UpdateMode Or dePPS.RetrieveMode Then
            err = AccessDatabaseWS614R_UpdateOrRetrievePaymentDetailsMode()
        Else
            err = AccessDatabaseWS614R_AddPPSMode()
        End If
            Return err
    End Function

    Private Function AccessDatabaseWS614R_AddPPSMode() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim enrolmentCount As Integer = 0
        Dim schemeCount As Integer = 0
        Dim bMoreRecords As Boolean = True
        Dim PARAMOUT As String = String.Empty
        Dim successCount As Integer = 0
        Dim errorCount As Integer = 0

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("SuccessCount", GetType(Integer))
            .Add("ErrorCount", GetType(Integer))
            .Add("PaymentDetails", GetType(String))
            .Add("PaymentType", GetType(String))
        End With

        'Create the failed requests data table
        Dim DtFailedRequests As New DataTable("FailedRequests")
        ResultDataSet.Tables.Add(DtFailedRequests)
        With DtFailedRequests.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("PPSEnrolment", GetType(DEPPSEnrolment))
        End With

        Try

            'Loop until no more products available
            Do While enrolmentCount < dePPS.Enrolments.Count

                'Enrol the customers
                PARAMOUT = CallWS614R_AddPPSMode(enrolmentCount, schemeCount)

                'Set the response data
                If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then

                    'Construct an object with just the failures
                    Dim enrol As New DEPPSEnrolment
                    enrol.CustomerNumber = dePPS.Enrolments.Item(enrolmentCount).CustomerNumber
                    enrol.PaymentDetails = dePPS.Enrolments.Item(enrolmentCount).PaymentDetails
                    If Not PARAMOUT.Substring(3069, 2).Equals("PE") Then

                        'Add each failed item and set the return code
                        For i As Integer = schemeCount To dePPS.Enrolments.Item(enrolmentCount).EnrolmentSchemes.Count - 1
                            enrol.EnrolmentSchemes.Add(dePPS.Enrolments.Item(enrolmentCount).EnrolmentSchemes.Item(i))
                            enrol.EnrolmentSchemes.Item(i - schemeCount).ErrorCode = PARAMOUT.Substring(3069, 2).Trim
                        Next

                        'Set the failure total
                        errorCount += dePPS.Enrolments.Item(enrolmentCount).EnrolmentSchemes.Count - schemeCount
                    Else

                        'Add each failed item and set the return code
                        enrol.EnrolmentSchemes = FailedSchemes(PARAMOUT)

                        'Set the total fields
                        successCount += dePPS.Enrolments.Item(enrolmentCount).EnrolmentSchemes.Count - enrol.EnrolmentSchemes.Count
                        errorCount += enrol.EnrolmentSchemes.Count
                    End If

                    'We need to return the failed requests
                    dRow = Nothing
                    dRow = DtFailedRequests.NewRow
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
                    dRow("PPSEnrolment") = enrol
                    DtFailedRequests.Rows.Add(dRow)

                Else
                    'Set the success total
                    successCount += dePPS.Enrolments.Item(enrolmentCount).EnrolmentSchemes.Count
                End If

                'Move onto the next scheme enrolment if the backend request failed
                If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                    schemeCount = 0
                    enrolmentCount += 1
                Else
                    'The backend can only process 50 at a time.
                    schemeCount += 50
                    If dePPS.Enrolments.Item(enrolmentCount).EnrolmentSchemes.Count < schemeCount Then
                        schemeCount = 0
                        enrolmentCount += 1
                    End If
                End If

            Loop

            'Add the status record
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            dRow("SuccessCount") = successCount
            dRow("ErrorCount") = errorCount
            DtStatusResults.Rows.Add(dRow)

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPP-01"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS614R_AddPPSMode(ByVal enrolmentCount As Integer, ByVal schemeCount As Integer) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS614R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS614Parm_AddPPSMode(enrolmentCount, schemeCount)
        parmIO.Direction = ParameterDirection.InputOutput

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        Return PARAMOUT

    End Function

    Private Function WS614Parm_AddPPSMode(ByVal enrolmentCount As Integer, ByVal schemeCount As Integer) As String

        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(dePPS.SessionId, 36))
        myString.Append(Utilities.FixStringLength(dePPS.Enrolments.Item(enrolmentCount).PaymentDetails.PaymentType, 2))
        myString.Append(Utilities.FixStringLength(PaymentDetails(dePPS.Enrolments(enrolmentCount).PaymentDetails), 200))
        myString.Append(Utilities.FixStringLength(SchemeDetails(dePPS.Enrolments(enrolmentCount).EnrolmentSchemes, schemeCount, _
                                    dePPS.Enrolments(enrolmentCount).CustomerNumber), 2500)) '2613
        myString.Append(Utilities.FixStringLength("", 37))
        myString.Append(Utilities.FixStringLength("N", 1)) 'Retrieve Mode
        myString.Append(Utilities.FixStringLength("N", 1)) 'Update Mode
        myString.Append(Utilities.FixStringLength("", 291))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append("   ")

        Return myString.ToString

    End Function

    Private Function AccessDatabaseWS614R_UpdateOrRetrievePaymentDetailsMode() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the payment details data table
        Dim DtPaymentDetails As New DataTable("PaymentDetails")
        ResultDataSet.Tables.Add(DtPaymentDetails)
        With DtPaymentDetails.Columns
            .Add("PaymentType", GetType(String))
            .Add("PaymentDetails", GetType(String))
            .Add("CustomerNumber", GetType(String))
            .Add("SeatDetails", GetType(String))
            .Add("ProductCode", GetType(String))
        End With

        Try
            PARAMOUT = CallWS614R_UpdateOrRetrievePaymentDetailsMode()

            'Set the response data
            If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                dRow = Nothing
                dRow = DtStatusResults.NewRow
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
                DtStatusResults.Rows.Add(dRow)
            Else
                dRow = Nothing
                dRow = DtPaymentDetails.NewRow
                dRow("PaymentType") = PARAMOUT.Substring(36, 2).Trim
                dRow("PaymentDetails") = PARAMOUT.Substring(40, 19).Trim
                dRow("CustomerNumber") = PARAMOUT.Substring(113, 12).Trim
                dRow("SeatDetails") = PARAMOUT.Substring(131, 16).Trim
                dRow("ProductCode") = PARAMOUT.Substring(125, 6)
                DtPaymentDetails.Rows.Add(dRow)
            End If
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPP-02"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS614R_UpdateOrRetrievePaymentDetailsMode() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS614R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS614Parm_UpdateOrRetrievePaymentDetailsMode()
        parmIO.Direction = ParameterDirection.InputOutput

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        Return PARAMOUT

    End Function

    Private Function WS614Parm_UpdateOrRetrievePaymentDetailsMode() As String

        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(dePPS.SessionId, 36))
        If dePPS.UpdateMode Then
            myString.Append(Utilities.FixStringLength(dePPS.Enrolments(0).PaymentDetails.PaymentType, 2)) 'CC or DD or something else (if external CC)
            myString.Append(Utilities.FixStringLength(dePPS.Enrolments(0).PaymentDetails.PaymentType, 2)) 'CC or DD - intentionally in here twice.
            myString.Append(Utilities.FixStringLength(dePPS.Enrolments(0).PaymentDetails.CardNumber, 19))
            myString.Append(Utilities.FixStringLength(dePPS.Enrolments(0).PaymentDetails.ExpiryDate, 4))
            myString.Append(Utilities.FixStringLength(dePPS.Enrolments(0).PaymentDetails.StartDate, 4))
            myString.Append(Utilities.FixStringLength(dePPS.Enrolments(0).PaymentDetails.IssueNumber, 2))
            myString.Append(Utilities.FixStringLength(dePPS.Enrolments(0).PaymentDetails.CV2Number, 4))
            myString.Append(Utilities.FixStringLength(dePPS.Enrolments(0).PaymentDetails.CardHolderName, 30))
            myString.Append(Utilities.FixStringLength(dePPS.Enrolments(0).PaymentDetails.TokenID, 18))
            myString.Append(Utilities.FixStringLength(dePPS.Enrolments(0).PaymentDetails.ProcessingDB, 15))
            myString.Append(Utilities.FixStringLength(dePPS.Enrolments(0).PaymentDetails.TransactionID, 15))
            myString.Append(Utilities.FixStringLength(dePPS.Enrolments(0).PaymentDetails.CardType, 10))
            myString.Append(Utilities.FixStringLength("", 77))
        Else
            myString.Append(Utilities.FixStringLength("", 202))
        End If
        myString.Append(Utilities.PadLeadingZeros(dePPS.CustomerNumber, 12))
        myString.Append(Utilities.FixStringLength(dePPS.ProductCode, 6))
        myString.Append(Utilities.FixStringLength(dePPS.SeatDetails, 16))
        myString.Append(Utilities.FixStringLength("", 2378)) '2650
        myString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(dePPS.RetrieveMode), 1)) 'Retrieve Mode 
        myString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(dePPS.UpdateMode), 1)) 'Update Mode
        myString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(dePPS.CancelEnrolMode), 1)) 'Cancel Mode
        myString.Append(Utilities.FixStringLength("", 415))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, "1"))
        myString.Append(Utilities.FixStringLength("", 3))

        Return myString.ToString
    End Function

    Private Function AccessDatabaseWS614R_cancelPPSEnrolmentMode() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With


        Try
            PARAMOUT = CallWS614R_cancelPPSEnrolmentMode()

            'Set the response data
            If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                dRow = Nothing
                dRow = DtStatusResults.NewRow
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
                DtStatusResults.Rows.Add(dRow)
            End If
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPP-02"
                .HasError = True
            End With
        End Try
        Return err
    End Function


    Private Function CallWS614R_cancelPPSEnrolmentMode() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS614R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS614Parm_UpdateOrRetrievePaymentDetailsMode()
        parmIO.Direction = ParameterDirection.InputOutput

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        Return PARAMOUT

    End Function



    Private Function AccessDatabaseWS617R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim bMoreRecords As Boolean = True
        Dim PARAMOUT As String = String.Empty
        Dim lastSeat As String = String.Empty
        Dim errorCount As Integer = 0
        Dim CSSClassLoggedInCustomer As String = "ebiz-logged-in"
        Dim CSSClassFriendsAndFamily As String = "ebiz-friends"
        Dim loggedInCustomerNumber As String = Utilities.PadLeadingZeros(dePPSEnrolmentScheme.CustomerNumber, 12)
        Dim tempCustomerNumber As String = ""

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the PPS results data table
        Dim DtPPSResults As New DataTable("PPSResults")
        ResultDataSet.Tables.Add(DtPPSResults)
        With DtPPSResults.Columns
            .Add("Product", GetType(String))
            .Add("CustomerNumber", GetType(String))
            .Add("SeatDetails", GetType(String))
            .Add("Name", GetType(String))
            .Add("Enrolled", GetType(Boolean))
            .Add("CSSClass", GetType(String))
            .Add("SchemeLocked", GetType(Boolean))
        End With

        Try
            'Loop until no more seats left to populate
            Do While bMoreRecords
                PARAMOUT = CallWS617R(lastSeat)

                'Set the response data
                If PARAMOUT.Substring(5117, 1) = "E" Or PARAMOUT.Substring(5118, 2).Trim <> "" Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(5118, 2)
                    DtStatusResults.Rows.Add(dRow)
                    bMoreRecords = False
                Else
                    Dim seatCounter As Integer = 0
                    Dim i As Integer = 1
                    Do While i <= 20 AndAlso PARAMOUT.Substring(seatCounter, 100).Trim <> String.Empty
                        dRow = Nothing
                        dRow = DtPPSResults.NewRow
                        dRow("Product") = PARAMOUT.Substring(13, 6).Trim
                        tempCustomerNumber = PARAMOUT.Substring(seatCounter + 36, 12)
                        dRow("CustomerNumber") = tempCustomerNumber
                        dRow("SeatDetails") = PARAMOUT.Substring(seatCounter + 19, 16)
                        dRow("Name") = PARAMOUT.Substring(seatCounter + 48, 50).Trim
                        dRow("Enrolled") = Utilities.convertToBool(PARAMOUT.Substring(seatCounter + 35, 1))
                        If tempCustomerNumber = loggedInCustomerNumber Then
                            dRow("CSSClass") = CSSClassLoggedInCustomer
                        Else
                            dRow("CSSClass") = CSSClassFriendsAndFamily
                        End If
                        dRow("SchemeLocked") = Utilities.convertToBool(PARAMOUT.Substring(seatCounter + 98, 1))
                        DtPPSResults.Rows.Add(dRow)
                        seatCounter = seatCounter + 100
                        i += 1
                    Loop
                    lastSeat = PARAMOUT.Substring(2019, 16)
                    bMoreRecords = Utilities.convertToBool(PARAMOUT.Substring(5113, 1))
                End If
            Loop
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPP-03"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS617R(ByVal lastSeat As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS617R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS617Parm(lastSeat)
        parmIO.Direction = ParameterDirection.InputOutput

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString

        Return PARAMOUT

    End Function

    Private Function WS617Parm(ByVal lastSeat As String) As String

        Dim myString As String

        'Construct the parameter
        myString = Utilities.FixStringLength(Utilities.ConvertToYN(dePPSEnrolmentScheme.AmendPPSEnrolmentIgnoreFF), 1) & _
                    Utilities.PadLeadingZeros(dePPSEnrolmentScheme.CustomerNumber, 12) & _
                    Utilities.FixStringLength(dePPSEnrolmentScheme.ProductCode, 6) & _
                    Utilities.FixStringLength("", 2000) & _
                    Utilities.FixStringLength(lastSeat, 26) & _
                    Utilities.FixStringLength("", 4) & _
                    Utilities.FixStringLength(Settings.OriginatingSourceCode, "1") & "   "

        Return myString

    End Function

    Private Function PaymentDetails(ByVal pay As DEPayments) As String

        Dim parm As String = String.Empty

        Select Case pay.PaymentType
            'Credit Card Format
            Case Is = "CC"
                parm = "CC" & Utilities.FixStringLength(pay.CardNumber, 19) & _
                        Utilities.PadLeadingZeros(pay.ExpiryDate, 4) & _
                        Utilities.PadLeadingZeros(pay.StartDate, 4) & _
                        Utilities.FixStringLength(pay.IssueNumber, 2) & _
                        Utilities.FixStringLength(pay.CV2Number, 4) & _
                        Utilities.FixStringLength("", 30) & _
                        Utilities.FixStringLength("", 10)
                'Direct Debit Format
            Case Is = "DD"
                parm = "DD" & Utilities.FixStringLength(pay.AccountName, 40) & _
                        Utilities.PadLeadingZeros(pay.SortCode, 6) & _
                        Utilities.PadLeadingZeros(pay.AccountNumber, 8) & _
                        Utilities.PadLeadingZeros(pay.DDIReference, 10) & _
                        Utilities.FixStringLength("", 9)
        End Select


        Return parm

    End Function

    Private Function SchemeDetails(ByVal enrolment As Generic.List(Of DEPPSEnrolmentScheme), _
                                    ByVal schemeCount As Integer, _
                                    ByVal customerNumber As String) As String

        Dim parm As String = String.Empty

        For i As Integer = schemeCount To enrolment.Count - 1

            Dim seatDetails As New DESeatDetails
            seatDetails.FormattedSeat = enrolment(i).SeasonTicket

            'Format the scheme enrolments
            If String.IsNullOrEmpty(enrolment(i).CustomerNumber) Then
                parm += Utilities.PadLeadingZeros(customerNumber, 12)
            Else
                parm += Utilities.PadLeadingZeros(enrolment(i).CustomerNumber, 12)
            End If
            parm += Utilities.FixStringLength(enrolment(i).ProductCode, 6)
            parm += Utilities.FixStringLength(seatDetails.Stand, 3)
            parm += Utilities.FixStringLength(seatDetails.Area, 4)
            parm += Utilities.FixStringLength(seatDetails.Row, 4)
            parm += Utilities.FixStringLength(seatDetails.Seat, 4)
            parm += Utilities.FixStringLength(seatDetails.AlphaSuffix, 1)
            parm += Utilities.FixStringLength(enrolment(i).RegisteredPost, 1)
            parm += Utilities.FixStringLength("", 15)

            'Exit when 50 schemes have been processed.
            If parm.Length = 2500 Then
                Exit For
            End If
        Next

        Return parm

    End Function

    Private Function FailedSchemes(ByVal parm As String) As Generic.List(Of DEPPSEnrolmentScheme)

        Dim schemes As New Generic.List(Of DEPPSEnrolmentScheme)
        Dim i As Integer = 0

        'We are only interested in the scheme information
        parm = parm.Substring(113, 2500)
        Do While i < 2500

            'Have we procesed all of the schemes
            If String.IsNullOrEmpty(parm.Substring(i, 50).Trim) Then
                Exit Do
            Else
                'Extract schemes that are in error from the parameter
                If Not String.IsNullOrEmpty(parm.Substring(i + 48, 2).Trim) Then

                    'Construct the seat
                    Dim seatDetails As New DESeatDetails
                    seatDetails.Stand = parm.Substring(i + 18, 3)
                    seatDetails.Area = parm.Substring(i + 21, 4)
                    seatDetails.Row = parm.Substring(i + 25, 4)
                    seatDetails.Seat = parm.Substring(i + 29, 4)
                    seatDetails.AlphaSuffix = parm.Substring(i + 33, 1) 

                    'Add the enrolment
                    Dim enrol As New DEPPSEnrolmentScheme
                    enrol.CustomerNumber = parm.Substring(i, 12)
                    enrol.ProductCode = parm.Substring(i + 12, 6)
                    enrol.SeasonTicket = seatDetails.FormattedSeat
                    enrol.RegisteredPost = parm.Substring(i + 34, 1)
                    enrol.ErrorCode = parm.Substring(i + 48, 2)
                    schemes.Add(enrol)
                End If
            End If

            'Incremenet and loop again
            i += 50
        Loop

        Return schemes

    End Function
End Class
