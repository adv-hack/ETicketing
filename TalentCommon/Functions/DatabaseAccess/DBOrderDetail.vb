Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Order Detail Requests
'
'       Date                        5th Feb 2007
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved. 
'
'       Error Number Code base      TACDBOD- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBOrderDetail
    Inherits DBAccess
    '---------------------------------------------------------------------------------------------
    Private _system21CompanyPo As String = String.Empty
    Private _de As New DETicketingItemDetails
    Private Const OrderDetails As String = "OrderDetails"
    Private Const RetrieveSCOrderDetails As String = "RetrieveSCOrderDetails"
    Private Const OrderProductsSpecificText As String = "OrderProductsSpecificText"
    Private Const GetBulkSeats As String = "GetBulkSeats"
    Private Const GetComponentBulkSeats As String = "GetComponentBulkSeats"
    Private _dep As New DEOrder
    Private DB As New DBOrder
    Private _orderFunctions As New DBOrderFunctions
   

    Public Property System21CompanyPo() As String
        Get
            Return _system21CompanyPo
        End Get
        Set(ByVal value As String)
            _system21CompanyPo = value
        End Set
    End Property
    Public Property Dep() As DEOrder
        Get
            Return _dep
        End Get
        Set(ByVal value As DEOrder)
            _dep = value
        End Set
    End Property
    Public Property De() As DETicketingItemDetails
        Get
            Return _de
        End Get
        Set(ByVal value As DETicketingItemDetails)
            _de = value
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


    Public Function ReadOrder(Optional ByVal opendb As Boolean = True) As ErrorObj
        Dim err As New ErrorObj
        '-------------------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                err = ReadOrderSystem21()
            Case Is = TALENTTKT
                err = AccessDatabase()
        End Select
        Return err
    End Function

    Public Function ReadOrderSystem21() As ErrorObj
        Dim err As ErrorObj = Nothing
        '-----------------------------------------------------------------------------
        '   by customer purchase order number
        '
        Const sqlSelect1 As String = "SELECT ORDN40 FROM OEP40 " & _
                                          " WHERE CONO40 = @PARAM1 AND CUSO40 = @PARAM2 AND " & _
                                           " CUSN40 = @CUSN40 " & _
                                          " ORDER BY ORDN40 "
        '-----------------------------------------------------------------------------
        '   by Branch order number
        '
        Const sqlSelect2 As String = "SELECT ORDN40 FROM OEP40 " & _
                                          " WHERE CONO40 = @PARAM1 AND ORDN40 = @PARAM2 AND " & _
                                           " CUSN40 = @CUSN40 " & _
                                          " ORDER BY ORDN40 "
        '-----------------------------------------------------------------------------
        '   by both
        '
        Const sqlSelect3 As String = "SELECT ORDN40 FROM OEP40 " & _
                                          " WHERE CONO40 = @PARAM1 AND CUSO40 = @PARAM2 AND " & _
                                           " ORDN40 = @ORDN40 AND CUSN40 = @CUSN40 " & _
                                          " ORDER BY ORDN40 "
        Dim cmdSelect As iDB2Command = Nothing
        Dim dtrReader As iDB2DataReader = Nothing
        Dim param1 As String = "@PARAM1"
        Dim param2 As String = "@PARAM2"
        Dim iOrder As Integer = 0
        '----------------------------------------------------------------------------------------
        Try
            err = System21Open()
            Dim dr As DataRow
            Dim deoh As New DeOrderHeader                       ' Items  as Collection
            Dim OrderNumber As String = String.Empty
            With DB
                .System21CompanyNo = Settings.AccountNo3
                Settings.Authorised = True                      ' to prevent ReadOrderSystem21 being abused
                '-----------------------------------------------------------------------------
                If .ResultDataSet Is Nothing Then
                    .ResultDataSet = New DataSet
                    .ResultDataSet.Tables.Add(.DtHeader)        ' 0
                    .ResultDataSet.Tables.Add(.DtDetail)        ' 1
                    .ResultDataSet.Tables.Add(.DtText)          ' 2
                    .ResultDataSet.Tables.Add(.DtCarrier)       ' 3
                    .ResultDataSet.Tables.Add(.DtPackage)       ' 4
                    .ResultDataSet.Tables.Add(.DtProduct)       ' 5
                    '
                    .ResultDataSet.Tables.Item(0).TableName = "DtHeader"
                    .ResultDataSet.Tables.Item(1).TableName = "DtDetail"
                    .ResultDataSet.Tables.Item(2).TableName = "DtText"
                    .ResultDataSet.Tables.Item(3).TableName = "DtCarrier"
                    .ResultDataSet.Tables.Item(4).TableName = "DtPackage"
                    .ResultDataSet.Tables.Item(5).TableName = "DtProduct"
                End If
                '-----------------------------------------------------------------------------
                Dim system21OrderNo As String
                For iOrder = 1 To Dep.CollDEOrders.Count
                    deoh = Dep.CollDEOrders.Item(iOrder)
                    .System21OrderNo = String.Empty
                    system21OrderNo = deoh.BranchOrderNumber
                    System21CompanyPo = deoh.CustomerPO
                    '-------------------------------------------------------------------------
                    If System21CompanyPo <> String.Empty And system21OrderNo = String.Empty Then
                        '-----------------------------------------------------------------------------
                        '   by customer purchase order number
                        '
                        cmdSelect = New iDB2Command(sqlSelect1, conSystem21)
                        Select Case Settings.DatabaseType1
                            Case Is = T65535
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 20).Value = System21CompanyPo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2CharBitData, 8).Value = Settings.AccountNo1.Trim
                            Case Is = T285
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 20).Value = System21CompanyPo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                            Case Else
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 20).Value = System21CompanyPo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                        End Select
                    ElseIf System21CompanyPo = String.Empty And system21OrderNo <> String.Empty Then
                        '-----------------------------------------------------------------------------
                        '   by Branch order number
                        '
                        cmdSelect = New iDB2Command(sqlSelect2, conSystem21)
                        Select Case Settings.DatabaseType1
                            Case Is = T65535
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 7).Value = system21OrderNo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2CharBitData, 8).Value = Settings.AccountNo1.Trim
                            Case Is = T285
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = system21OrderNo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                            Case Else
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = system21OrderNo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                        End Select
                    Else
                        '-----------------------------------------------------------------------------
                        '   by both
                        '
                        cmdSelect = New iDB2Command(sqlSelect3, conSystem21)
                        Select Case Settings.DatabaseType1
                            Case Is = T65535
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2CharBitData, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2CharBitData, 20).Value = System21CompanyPo
                                cmdSelect.Parameters.Add("@ORDN40", iDB2DbType.iDB2CharBitData, 7).Value = system21OrderNo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2CharBitData, 8).Value = Settings.AccountNo1.Trim
                            Case Is = T285
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 20).Value = System21CompanyPo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = system21OrderNo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                            Case Else
                                cmdSelect.Parameters.Add(param1, iDB2DbType.iDB2Char, 2).Value = .System21CompanyNo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 20).Value = System21CompanyPo
                                cmdSelect.Parameters.Add(param2, iDB2DbType.iDB2Char, 7).Value = system21OrderNo
                                cmdSelect.Parameters.Add("@CUSN40", iDB2DbType.iDB2Char, 8).Value = Settings.AccountNo1.Trim
                        End Select
                    End If
                    '------------------------------------
                    dtrReader = cmdSelect.ExecuteReader()
                    If Not err.HasError Then
                        .Settings = Settings
                        .Dep = Dep
                        If dtrReader.HasRows Then
                            While dtrReader.Read
                                OrderNumber = dtrReader.GetString(dtrReader.GetOrdinal("ORDN40"))
                                .System21OrderNo = OrderNumber
                                err = .ReadOrder(True)
                            End While
                        Else
                            dr = Nothing
                            dr = DB.DtHeader.NewRow()
                            dr("BranchOrderNumber") = "Not Found"
                            dr("CustomerPO") = System21CompanyPo
                            DB.DtHeader.Rows.Add(dr)              ' Add to Header Table
                            '
                            With err
                                .ItemErrorMessage(iOrder) = String.Empty
                                .ItemErrorCode(iOrder) = "TACDBOD-036"
                                .ItemErrorStatus(iOrder) = "Error Purchase Order number does not exist on System21- " & System21CompanyPo
                            End With

                        End If
                    End If
                    '-------------------------------------------------------------------------
                Next iOrder
                ResultDataSet = DB.ResultDataSet
            End With
            System21Close()
            '
        Catch ex As Exception
            Const strError As String = "Error during Read By Purchase Order Number"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBOD-032"
                .HasError = True
                Return err
            End With
        End Try
        Return err
    End Function
    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj

        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = OrderDetails
                If CType(Dep.CollDEOrders.Item(1), DETicketingItemDetails).Type = "FutureBooked" Then
                    err = AccessDatabaseWS053R()
                Else
                    err = AccessDatabaseWS030R()
                End If
            Case Is = RetrieveSCOrderDetails
                err = AccessDatabaseWS172R()
            Case Is = OrderProductsSpecificText
                err = AccessDatabaseMD08KS()
            Case Is = GetBulkSeats
                err = AccessDatabaseWS030S()
            Case Is = GetComponentBulkSeats
                err = AccessDatabaseWS005S()
        End Select

        Return err

    End Function

    Private Function AccessDatabaseWS030R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        DBOrderFunctions.De = Dep.CollDEOrders.Item(1)
        DBOrderFunctions.StoredProcedureGroup = Settings.StoredProcedureGroup
        DBOrderFunctions.conTALENTTKT = conTALENTTKT
        err = DBOrderFunctions.AccessDatabaseWS030R()
        ResultDataSet = DBOrderFunctions.ResultDataSet

        Return err

    End Function
    Private Function AccessDatabaseWS030S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        DBOrderFunctions.De = Dep.CollDEOrders.Item(1)
        DBOrderFunctions.StoredProcedureGroup = Settings.StoredProcedureGroup
        DBOrderFunctions.conTALENTTKT = conTALENTTKT
        err = DBOrderFunctions.AccessDatabaseWS030S()
        ResultDataSet = DBOrderFunctions.ResultDataSet
        Return err
    End Function

    Private Function AccessDatabaseWS005S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        Dim dtComponentBulkSeats As New DataTable("ComponentBulkSeatDetails")
        ResultDataSet.Tables.Add(dtComponentBulkSeats)

        Try
            CallWS005S()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Error during database Access"
                .ErrorNumber = "DBOrderEnquiry-WS005S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Public Function AccessDatabaseWS053R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim sStartPos As String = "000"
        Dim sReturnedTotal As String = "000"
        Dim PARAMOUT As String = String.Empty
        Dim bMoreRecords As Boolean = True
        Dim parmWS053R As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("ProductDescription", GetType(String))
        End With

        'Create the Order data table
        Dim DtOrderResults As New DataTable("OrderResults")
        ResultDataSet.Tables.Add(DtOrderResults)
        With DtOrderResults.Columns
            .Add("CustomerNo", GetType(String))
            .Add("ContactSurname", GetType(String))
            .Add("ProductDescription", GetType(String))
            .Add("ProductDate", GetType(String))
            .Add("Seat", GetType(String))
            .Add("Status", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("ContactInitial", GetType(String))
            .Add("Unreserved", GetType(String))
        End With

        Try

            'Loop until all the Orders have been retrieved
            Do While bMoreRecords = True

                If ParmWS053R.Trim <> "" And sStartPos = "000" Then
                    PARAMOUT = ParmWS053R
                Else
                    'Call WS053R
                    PARAMOUT = CallWS053R(sReturnedTotal, sStartPos)
                End If

                If sStartPos = "000" Then

                    'Set the response data
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(3071, 1) = "E" Or PARAMOUT.Substring(3069, 2).Trim <> "" Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(3069, 2)
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    dRow("ProductCode") = PARAMOUT.Substring(3000, 6)
                    dRow("ProductDescription") = PARAMOUT.Substring(3006, 40)
                    DtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(3071, 1) <> "E" And PARAMOUT.Substring(3069, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 20

                        ' Has a order been returned (check member)
                        If PARAMOUT.Substring(iPosition, 12).Trim = "" Then
                            bMoreRecords = False
                            Exit Do
                        Else

                            'Create a new row
                            dRow = Nothing
                            dRow = DtOrderResults.NewRow
                            dRow("CustomerNo") = PARAMOUT.Substring(iPosition, 12)
                            dRow("ContactSurname") = PARAMOUT.Substring(iPosition + 12, 30)
                            dRow("ProductDescription") = PARAMOUT.Substring(iPosition + 42, 40)
                            dRow("ProductDate") = PARAMOUT.Substring(iPosition + 82, 8)
                            dRow("Seat") = PARAMOUT.Substring(iPosition + 90, 20)
                            dRow("Status") = PARAMOUT.Substring(iPosition + 110, 20)
                            dRow("ProductCode") = PARAMOUT.Substring(iPosition + 130, 6)
                            dRow("ContactInitial") = PARAMOUT.Substring(iPosition + 136, 1)
                            dRow("Unreserved") = Utilities.convertToBool(PARAMOUT.Substring(iPosition + 137, 1))

                            DtOrderResults.Rows.Add(dRow)

                            'Increment
                            iPosition = iPosition + 150
                            iCounter = iCounter + 1

                        End If
                    Loop

                    'Extract the footer information
                    sStartPos = PARAMOUT.Substring(3065, 3)
                    sReturnedTotal = PARAMOUT.Substring(3062, 3)
                    ' If all 20 records have been populated from the calling program, 
                    ' the program will need to be called again to check and return any
                    ' further records
                    If CInt(sReturnedTotal) < 20 Then
                        bMoreRecords = False
                    End If
                Else
                    bMoreRecords = False
                End If
            Loop

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBOD-01"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Public Function AccessDatabaseWS172R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim parmWS172R As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("ProductDescription", GetType(String))
        End With

        'Create the Order data table
        Dim DtSCOrderDetails As New DataTable("SCOrderDetails")
        ResultDataSet.Tables.Add(DtSCOrderDetails)
        With DtSCOrderDetails.Columns
            .Add("NoOfTickets", GetType(Integer))
            .Add("NoOfCardsProduced", GetType(Integer))
            .Add("NoOfCardsUploaded", GetType(Integer))
            .Add("NoOfSeasonTicketsUploaded", GetType(Integer))
            .Add("NoOfRFIDTickets", GetType(Integer))
            .Add("NoOfBarcodeRequests", GetType(Integer))
            .Add("NoOfSCErrors", GetType(Integer))
            .Add("NoOfNonSCProducts", GetType(Integer))
        End With

        Try

            PARAMOUT = CallWS172R()
            'End If

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

                'Create a new row
                dRow = Nothing
                dRow = DtSCOrderDetails.NewRow
                dRow("NoOfTickets") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(15, 3).Trim())
                dRow("NoOfCardsProduced") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(18, 3).Trim())
                dRow("NoOfCardsUploaded") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(21, 3).Trim())
                dRow("NoOfSeasonTicketsUploaded") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(24, 3).Trim())
                dRow("NoOfRFIDTickets") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(27, 3).Trim())
                dRow("NoOfBarcodeRequests") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(30, 3).Trim())
                dRow("NoOfSCErrors") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(33, 3).Trim())
                dRow("NoOfNonSCProducts") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(36, 3).Trim())

                DtSCOrderDetails.Rows.Add(dRow)

            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBOD-02"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS053R(ByVal sRecordTotal As String, _
                                ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS053R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add("@Param1", iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS053Parm(sRecordTotal, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@Param1").Value.ToString

        Return PARAMOUT

    End Function

    Private Function CallWS172R() As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS172R"
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add("@Param1", iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS172RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@Param1").Value.ToString

        Return PARAMOUT

    End Function

    Private Sub CallWS005S()
        Dim cmdSelect As iDB2Command = Nothing
        Dim strHeader As New StringBuilder
        Dim cmdAdapter = New iDB2DataAdapter

        cmdSelect = conTALENTTKT.CreateCommand()
        strHeader.Append("CALL ")
        strHeader.Append("WS005S(@PARAM0, @PARAM1, @PARAM2)")
        cmdSelect = New iDB2Command(strHeader.ToString(), conTALENTTKT)

        Dim pCallID As iDB2Parameter
        Dim pComponentID As iDB2Parameter
        Dim pErrorCode As iDB2Parameter

        pCallID = cmdSelect.Parameters.Add(Param0, iDB2DbType.iDB2Decimal, 13)
        pComponentID = cmdSelect.Parameters.Add(Param1, iDB2DbType.iDB2Decimal, 13)
        pErrorCode = cmdSelect.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput

        pCallID.Value = Dep.CallId
        pComponentID.Value = Dep.ComponentId
        pErrorCode.Value = String.Empty

        cmdAdapter.SelectCommand = cmdSelect
        cmdAdapter.Fill(ResultDataSet, "ComponentBulkSeatDetails")

        Dim drStatus As DataRow = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).NewRow
        If CStr(cmdSelect.Parameters(2).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = CStr(cmdSelect.Parameters(2).Value).Trim
            drStatus("ReturnCode") = GlobalConstants.ERRORFLAG
        End If
        ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Add(drStatus)
    End Sub


    Public Function WS053Parm(ByVal sReturnedTotal As String, _
                                ByVal sStartPos As String) As String

        Dim myString As String

        myString = Utilities.FixStringLength("", 3048) & _
                   Utilities.FixStringLength(CType(Dep.CollDEOrders.Item(1), DETicketingItemDetails).StadiumCode, 2) & _
                   Utilities.FixStringLength(CType(Dep.CollDEOrders.Item(1), DETicketingItemDetails).CustomerNo, 12) & _
                   Utilities.FixStringLength(sReturnedTotal, 3) & _
                   Utilities.FixStringLength(sStartPos, 3) & "W   "

        Dim iLength As Integer
        iLength = myString.Length

        Return myString

    End Function

    Public Function WS172RParm() As String

        Dim myString As String
        Dim ticketingItemDtls As DETicketingItemDetails
        Dim paymentRef As String = "000000000000000"
        Dim src As String = " "

        If Not Dep.CollDEOrders.Item(1) Is Nothing Then
            ticketingItemDtls = CType(Dep.CollDEOrders.Item(1), DETicketingItemDetails)
            paymentRef = ticketingItemDtls.PaymentReference
            src = ticketingItemDtls.Src
        End If


        myString = Utilities.PadLeadingZeros(paymentRef, 15) & _
                    Utilities.FixStringLength(" ", 1005) & _
                    Utilities.FixStringLength(src, 1) & _
                    Utilities.FixStringLength(" ", 3)

        Dim iLength As Integer
        iLength = myString.Length

        Return myString

    End Function

    Private Function AccessDatabaseMD08KS() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Try

            'Create the Status data table
            Dim DtStatusResults As New DataTable("StatusResults")
            ResultDataSet.Tables.Add(DtStatusResults)
            With DtStatusResults.Columns
                .Add("ErrorOccurred", GetType(String))
                .Add("ReturnCode", GetType(String))
            End With
            Dim DtProductSpecificText As New DataTable("ProductSpecificText")
            ResultDataSet.Tables.Add(DtProductSpecificText)
            With DtProductSpecificText.Columns
                .Add("ProductCode", GetType(String))
                .Add("Sequence", GetType(String))
                .Add("SpecificText", GetType(String))
            End With
            If Dep IsNot Nothing AndAlso Dep.ProductCodes IsNot Nothing AndAlso Dep.ProductCodes.Length > 0 Then
                Dim startIndex As Integer = 0
                While (startIndex < Dep.ProductCodes.Length)
                    Call_MD08KS(ResultDataSet, startIndex)
                End While
            Else
                Dim dr As DataRow = DtStatusResults.NewRow
                dr("ErrorOccurred") = "E"
                dr("ReturnCode") = "PM"
                DtStatusResults.Rows.Add(dr)
            End If

        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBOD-MD08KS"
                .HasError = True
            End With
        End Try


        Return err

    End Function

    Private Sub Call_MD08KS(ByRef pdtSpecificDataSet As DataSet, ByRef startIndex As Integer)
        Try
            Dim pdtSpecificCommand As iDB2Command = conTALENTTKT.CreateCommand()
            pdtSpecificCommand.CommandText = "Call MD08KS(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)"
            pdtSpecificCommand.CommandType = CommandType.Text
            iDB2CommandBuilder.DeriveParameters(pdtSpecificCommand)

            pdtSpecificCommand.Parameters(0).Value = ""
            pdtSpecificCommand.Parameters(1).Value = GetProductCode(startIndex)
            pdtSpecificCommand.Parameters(2).Value = GetProductCode(startIndex + 1)
            pdtSpecificCommand.Parameters(3).Value = GetProductCode(startIndex + 2)
            pdtSpecificCommand.Parameters(4).Value = GetProductCode(startIndex + 3)
            pdtSpecificCommand.Parameters(5).Value = GetProductCode(startIndex + 4)
            pdtSpecificCommand.Parameters(6).Value = GetProductCode(startIndex + 5)
            pdtSpecificCommand.Parameters(7).Value = GetProductCode(startIndex + 6)
            pdtSpecificCommand.Parameters(8).Value = GetProductCode(startIndex + 7)
            pdtSpecificCommand.Parameters(9).Value = GetProductCode(startIndex + 8)
            pdtSpecificCommand.Parameters(10).Value = GetProductCode(startIndex + 9)

            'for next call the star index will be
            startIndex = startIndex + 10

            Dim pdtSpecificCmdReader As iDB2DataReader = pdtSpecificCommand.ExecuteReader()

            Dim dr As DataRow = pdtSpecificDataSet.Tables("StatusResults").NewRow
            If pdtSpecificCommand.Parameters(0).Value.ToString().Trim.Length > 0 Then
                dr("ErrorOccurred") = "E"
                dr("ReturnCode") = pdtSpecificCommand.Parameters(0).Value
            Else
                dr("ErrorOccurred") = ""
                dr("ReturnCode") = ""
            End If
            pdtSpecificDataSet.Tables("StatusResults").Rows.Add(dr)

            If pdtSpecificDataSet.Tables("StatusResults").Rows(0)("ReturnCode").ToString().Trim.Length = 0 Then
                While pdtSpecificCmdReader.Read()
                    dr = Nothing
                    dr = pdtSpecificDataSet.Tables("ProductSpecificText").NewRow
                    dr("ProductCode") = pdtSpecificCmdReader.Item("MTCD8K").ToString().Trim()
                    dr("Sequence") = pdtSpecificCmdReader.Item("SEQN8K").ToString().Trim()
                    dr("SpecificText") = pdtSpecificCmdReader.Item("TEXT8K").ToString().Trim()
                    pdtSpecificDataSet.Tables("ProductSpecificText").Rows.Add(dr)
                End While
            End If
        Catch ex As Exception
            Throw
        End Try


    End Sub

    Private Function GetProductCode(ByVal arrIndex As Integer)
        If arrIndex < Dep.ProductCodes.Length Then
            Return Dep.ProductCodes(arrIndex)
        Else
            Return ""
        End If
    End Function

End Class
