Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Credit checks
'
'       Date                        July 2007
'
'       Author                      Ben
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBDD- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Public Class DBDeliveryDate
    Inherits DBAccess

    Private _deDelDate As DEDeliveryDate
    Private _dtHeader As New DataTable
    Private _dtItem As New DataTable
    Private _paramTRAN As String
    Private _deliveryZoneCode As String
    Private _deliveryZoneType As String
    Private Const GetDeliveryDate As String = "GetDeliveryDate"
    Private Const GetPreferredDeliveryDates As String = "GetPreferredDeliveryDates"

    Public Property deDelDate() As DEDeliveryDate
        Get
            Return _deDelDate
        End Get
        Set(ByVal value As DEDeliveryDate)
            _deDelDate = value
        End Set
    End Property

    Public Property dtHeader() As DataTable
        Get
            Return _dtHeader
        End Get
        Set(ByVal value As DataTable)
            _dtHeader = value
        End Set
    End Property

    Public Property dtItem() As DataTable
        Get
            Return _dtItem
        End Get
        Set(ByVal value As DataTable)
            _dtItem = value
        End Set
    End Property

    Public Property ParamTRAN() As String
        Get
            Return _paramTRAN
        End Get
        Set(ByVal value As String)
            _paramTRAN = value
        End Set
    End Property

    Public Property DeliveryZoneCode() As String
        Get
            Return _deliveryZoneCode
        End Get
        Set(ByVal value As String)
            _deliveryZoneCode = value
        End Set
    End Property

    Public Property DeliveryZoneType() As String
        Get
            Return _deliveryZoneType
        End Get
        Set(ByVal value As String)
            _deliveryZoneType = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj
        Dim err As New ErrorObj

        '--------------------------------------------------------------------
        If Not err.HasError Then
            Dim PARAMOUT As String = String.Empty
            Dim cmdSelect As iDB2Command = Nothing
            Dim dtrProducts As SqlDataReader = Nothing
            Dim paraminput As iDB2Parameter
            Dim paramoutput As iDB2Parameter
            Dim SQLString As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                      "/DELDATE(@PARAM1, @PARAM2)"

            CreateResultsTables()
            Try
                '-------------------------------------------------------------
                '   Execute
                '
                cmdSelect = New iDB2Command(SQLString, conSystem21)
                With cmdSelect
                    paraminput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                    paraminput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) &
                                        Utilities.FixStringLength(Settings.AccountNo1, 8) &
                                        Utilities.FixStringLength(Settings.AccountNo2, 3)
                    paraminput.Direction = ParameterDirection.Input
                    paramoutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                    paramoutput.Value = Param2
                    paramoutput.Direction = ParameterDirection.InputOutput
                    .ExecuteNonQuery()
                    '--------------------------------------------------------------------
                    PARAMOUT = cmdSelect.Parameters(Param2).Value.ToString
                    If PARAMOUT.Substring(1023, 1) = "Y" Then
                        With err
                            .ErrorMessage = PARAMOUT
                            .ErrorNumber = PARAMOUT.Substring(1019, 4)
                            .ErrorStatus = "Error retrieving delivery date - " &
                                        Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", err.ErrorNumber)
                            .HasError = True
                        End With
                    Else
                        ' Build the response DataSet
                        Dim iseriesDate As String = PARAMOUT.Substring(0, 10)
                        Dim currentDate As Date = deDelDate.CurrentDate

                        ResultDataSet = New DataSet
                        Dim newRow As DataRow = dtHeader.NewRow
                        newRow("CurrentDate") = currentDate
                        'newRow("CreditLimit") = 10
                        newRow("ProjectedDeliveryDate") = Utilities.ISeriesDate(iseriesDate)
                        dtHeader.Rows.Add(newRow)

                        Me.ResultDataSet.Tables.Add(Me.dtHeader)
                    End If
                    .Dispose()
                End With
                '--------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access "
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8 & "[" & PARAMOUT & "]"
                    .ErrorNumber = "TACDBDD-08"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------

        Return err
    End Function

    Protected Overrides Function AccessDataBaseChorus() As ErrorObj
        Dim err As New ErrorObj

        '--------------------------------------------------------------------
        If Not err.HasError Then
            Dim PARAMOUT As String = String.Empty
            Dim cmdSelect As iDB2Command = Nothing
            Dim dtrProducts As SqlDataReader = Nothing
            Dim paraminput As iDB2Parameter
            Dim paramoutput As iDB2Parameter
            Dim SQLString As String = "CALL " & Settings.StoredProcedureGroup.Trim &
                                      "/DELDATE(@PARAM1, @PARAM2)"

            CreateResultsTables()
            Try
                '-------------------------------------------------------------
                '   Execute
                '
                cmdSelect = New iDB2Command(SQLString, conChorus)
                With cmdSelect
                    paraminput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                    paraminput.Value = Utilities.FixStringLength(Settings.AccountNo3, 20) &
                                        Utilities.FixStringLength(Settings.AccountNo1, 15) &
                                        Utilities.FixStringLength(Settings.AccountNo2, 15)
                    paraminput.Direction = ParameterDirection.Input
                    paramoutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                    paramoutput.Value = Param2
                    paramoutput.Direction = ParameterDirection.InputOutput
                    .ExecuteNonQuery()
                    '--------------------------------------------------------------------
                    PARAMOUT = cmdSelect.Parameters(Param2).Value.ToString
                    If PARAMOUT.Substring(1023, 1) = "Y" Then
                        With err
                            .ErrorMessage = PARAMOUT
                            .ErrorNumber = PARAMOUT.Substring(1019, 4)
                            .ErrorStatus = "Error retrieving delivery date - " &
                                        Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", err.ErrorNumber)
                            .HasError = True
                        End With
                    Else
                        ' Build the response DataSet
                        Dim iseriesDate As String = PARAMOUT.Substring(0, 10)
                        Dim currentDate As Date = deDelDate.CurrentDate

                        ResultDataSet = New DataSet
                        Dim newRow As DataRow = dtHeader.NewRow
                        newRow("CurrentDate") = currentDate
                        'newRow("CreditLimit") = 10
                        'newRow("ProjectedDeliveryDate") = iseriesDate.Substring(6, 2) & "\" & iseriesDate.Substring(4, 2) & "\" & iseriesDate.Substring(0, 4)
                        newRow("ProjectedDeliveryDate") = iseriesDate.Substring(6, 2) & "/" & iseriesDate.Substring(4, 2) & "/" & iseriesDate.Substring(0, 4)

                        dtHeader.Rows.Add(newRow)

                        Me.ResultDataSet.Tables.Add(Me.dtHeader)
                    End If
                    .Dispose()
                End With
                '--------------------------------------------------------------------
            Catch ex As Exception
                Const strError8 As String = "Error during database access "
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError8 & "[" & PARAMOUT & "]"
                    .ErrorNumber = "TACDBDD-09"
                    .HasError = True
                End With
            End Try
        End If
        '--------------------------------------------------------------------

        Return err
    End Function

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj
        Select Case _settings.ModuleName
            Case Is = GetDeliveryDate : err = GetTheDeliveryDate()
            Case Is = GetPreferredDeliveryDates : err = GetThePreferredDeliveryDates()
        End Select
        Return err
    End Function

    Protected Sub CreateResultsTables()
        Me.ResultDataSet = New DataSet("DeliveryDateDataSet")
        Me.dtHeader = New DataTable("DeliveryDateHeader")
        Me.dtItem = New DataTable("DeliveryDateDetail")

        With dtHeader.Columns
            .Add(New DataColumn("CurrentDate", GetType(Date)))
            .Add(New DataColumn("ProjectedDeliveryDate", GetType(Date)))
            .Add(New DataColumn("DeliveryDateUsable", GetType(Boolean)))
        End With

        dtItem.Columns.Add(New DataColumn("ProjectedDeliveryDay", GetType(String)))

    End Sub

    Protected Function GetDeliveryCutOffTime() As String
        Dim deliveryCutOffTime As String = ""
        Dim sqlConn As New SqlConnection(Settings.FrontEndConnectionString)
        Dim cmd As SqlCommand = Nothing
        Const selectStr = "SELECT [VALUE] " &
                            "FROM tbl_ecommerce_module_defaults_bu " &
                            "WHERE [DEFAULT_NAME] = 'DELIVERY_CUT_OFF_TIME' " &
                            "AND [BUSINESS_UNIT] = @BUSINESS_UNIT " &
                            "AND [PARTNER] = @PARTNER"
        cmd = New SqlCommand(selectStr, sqlConn)
        Dim dtr As SqlDataReader

        'open
        Try
            sqlConn.Open()
        Catch ex As Exception
        End Try

        'Execute
        If sqlConn.State = ConnectionState.Open Then
            Try
                With cmd
                    .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = Settings.BusinessUnit
                    .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = Settings.Partner
                    dtr = .ExecuteReader()
                End With

                If dtr.Read Then
                    deliveryCutOffTime = dtr.Item("VALUE").ToString
                Else
                    dtr.Close()
                    cmd = New SqlCommand(selectStr, sqlConn)
                    With cmd
                        .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = Settings.BusinessUnit
                        .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = Utilities.GetAllString
                        dtr = .ExecuteReader()
                    End With

                    If dtr.Read Then
                        deliveryCutOffTime = dtr.Item("VALUE").ToString
                    Else
                        dtr.Close()
                        cmd = New SqlCommand(selectStr, sqlConn)
                        With cmd
                            .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = Utilities.GetAllString
                            .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = Settings.Partner
                            dtr = .ExecuteReader()
                        End With

                        If dtr.Read Then
                            deliveryCutOffTime = dtr.Item("VALUE").ToString
                        Else
                            dtr.Close()
                            cmd = New SqlCommand(selectStr, sqlConn)
                            With cmd
                                .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = Utilities.GetAllString
                                .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = Utilities.GetAllString
                                dtr = .ExecuteReader()
                            End With

                            If dtr.Read Then
                                deliveryCutOffTime = dtr.Item("VALUE").ToString
                            End If
                            dtr.Close()
                        End If
                    End If
                End If

            Catch ex As Exception
            End Try
        End If

        'Close
        Try
            If sqlConn.State = ConnectionState.Open Then
                sqlConn.Close()
            End If
        Catch ex As Exception
        End Try

        Return deliveryCutOffTime
    End Function

    Protected Function GetUnavailableDays() As DataTable
        Dim deliveryCutOffTime As String = ""
        Dim sqlConn As New SqlConnection(Settings.FrontEndConnectionString)
        Dim cmd As SqlCommand = Nothing
        Const selectStr = "SELECT [DATE] " &
                            "FROM tbl_delivery_unavailable_dates where CARRIER_CODE = '*ALL' or CARRIER_CODE = @carrierCode"

        cmd = New SqlCommand(selectStr, sqlConn)

        cmd.Parameters.Add("@carrierCode", SqlDbType.Char).Value = deDelDate.CarrierCode
        Dim dt As New Data.DataTable
        'open
        Try
            sqlConn.Open()
        Catch ex As Exception
        End Try

        Dim da As New SqlDataAdapter(cmd)

        'Execute
        If sqlConn.State = ConnectionState.Open Then
            Try
                da.Fill(dt)
            Catch ex As Exception
            End Try
        End If

        'Close
        Try
            If sqlConn.State = ConnectionState.Open Then
                sqlConn.Close()
            End If
        Catch ex As Exception
        End Try

        Return dt
    End Function

    Protected Function GetDeliveryLeadTime() As String
        Dim deliveryLeadTime As String = ""
        Dim sqlConn As New SqlConnection(Settings.FrontEndConnectionString)
        Dim cmd As SqlCommand = Nothing
        Dim selectStr As String = String.Empty
        Const selectStrTrade As String = "SELECT [VALUE] FROM tbl_ecommerce_module_defaults_bu WHERE [DEFAULT_NAME] = 'DELIVERY_LEAD_TIME' AND [BUSINESS_UNIT] = @BUSINESS_UNIT AND [PARTNER] = @PARTNER"
        Const selectStrHome As String = "SELECT [VALUE] FROM tbl_ecommerce_module_defaults_bu WHERE [DEFAULT_NAME] = 'DELIVERY_LEAD_TIME_HOME' AND [BUSINESS_UNIT] = @BUSINESS_UNIT AND [PARTNER] = @PARTNER"
        If deDelDate.HomeDelivery Then
            selectStr = selectStrHome
        Else
            selectStr = selectStrTrade
        End If
        cmd = New SqlCommand(selectStr, sqlConn)
        Dim dtr As SqlDataReader

        'open
        Try
            sqlConn.Open()
        Catch ex As Exception
        End Try

        'Execute
        If sqlConn.State = ConnectionState.Open Then
            Try
                With cmd
                    .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = Settings.BusinessUnit
                    .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = Settings.Partner
                    dtr = .ExecuteReader()
                End With

                If dtr.Read Then
                    deliveryLeadTime = dtr.Item("VALUE").ToString
                Else
                    dtr.Close()
                    cmd = New SqlCommand(selectStr, sqlConn)
                    With cmd
                        .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = Settings.BusinessUnit
                        .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = Utilities.GetAllString
                        dtr = .ExecuteReader()
                    End With

                    If dtr.Read Then
                        deliveryLeadTime = dtr.Item("VALUE").ToString
                    Else
                        dtr.Close()
                        cmd = New SqlCommand(selectStr, sqlConn)
                        With cmd
                            .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = Utilities.GetAllString
                            .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = Settings.Partner
                            dtr = .ExecuteReader()
                        End With

                        If dtr.Read Then
                            deliveryLeadTime = dtr.Item("VALUE").ToString
                        Else
                            dtr.Close()
                            cmd = New SqlCommand(selectStr, sqlConn)
                            With cmd
                                .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = Utilities.GetAllString
                                .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = Utilities.GetAllString
                                dtr = .ExecuteReader()
                            End With

                            If dtr.Read Then
                                deliveryLeadTime = dtr.Item("VALUE").ToString
                            End If
                            dtr.Close()
                        End If
                    End If
                End If

            Catch ex As Exception
            End Try
        End If

        'Close
        Try
            If sqlConn.State = ConnectionState.Open Then
                sqlConn.Close()
            End If
        Catch ex As Exception
        End Try

        Return deliveryLeadTime
    End Function

    Protected Function GetDeliveryZoneData() As DataTable
        Dim dtDeliveryZone As New DataTable
        Dim sqlConn As New SqlConnection(Settings.FrontEndConnectionString)
        Dim cmd As SqlCommand = Nothing
        Const selectStr = "SELECT TOP 1 [MONDAY], [TUESDAY], [WEDNESDAY], [THURSDAY], [FRIDAY], [SATURDAY], [SUNDAY] " &
                            "FROM tbl_delivery_zone " &
                            "WHERE [DELIVERY_ZONE_CODE] = @DELIVERY_ZONE_CODE " &
                            "AND [BUSINESS_UNIT] = @BUSINESS_UNIT " &
                            "AND [PARTNER] = @PARTNER"
        cmd = New SqlCommand(selectStr, sqlConn)
        With cmd
            .Parameters.Add(New SqlParameter("@DELIVERY_ZONE_CODE", SqlDbType.NVarChar, 50)).Value = DeliveryZoneCode
            .Parameters.Add(New SqlParameter("@BUSINESS_UNIT", SqlDbType.NVarChar, 50)).Value = Settings.BusinessUnit
            .Parameters.Add(New SqlParameter("@PARTNER", SqlDbType.NVarChar, 50)).Value = Settings.Partner
        End With
        'Dim dtr As SqlDataReader

        'open
        Try
            sqlConn.Open()
        Catch ex As Exception
        End Try

        'Execute
        If sqlConn.State = ConnectionState.Open Then
            Try
                Dim da As New SqlDataAdapter(cmd)
                da.Fill(dtDeliveryZone)
                If dtDeliveryZone.Rows.Count = 0 Then
                    cmd.Parameters("@PARTNER").Value = Utilities.GetAllString
                    da.Fill(dtDeliveryZone)
                    If dtDeliveryZone.Rows.Count = 0 Then
                        With cmd
                            .Parameters("@BUSINESS_UNIT").Value = Utilities.GetAllString
                            .Parameters("@PARTNER").Value = Settings.Partner
                        End With
                        da.Fill(dtDeliveryZone)
                        If dtDeliveryZone.Rows.Count = 0 Then
                            With cmd
                                .Parameters("@BUSINESS_UNIT").Value = Utilities.GetAllString
                                .Parameters("@PARTNER").Value = Utilities.GetAllString
                            End With
                            da.Fill(dtDeliveryZone)
                        End If
                    End If
                End If
                da.Dispose()
            Catch ex As Exception
            End Try
        End If

        'Close
        Try
            If sqlConn.State = ConnectionState.Open Then
                sqlConn.Close()
            End If
        Catch ex As Exception
        End Try

        Return dtDeliveryZone
    End Function

    Private Function GetTheDeliveryDate() As ErrorObj
        Dim err As New ErrorObj

        Dim d As Date = deDelDate.CurrentDate
        Dim expectedDeliveryDate As Date = d

        'Following table is used to get the unavailable days from tbl_delivery_unavailable_dates
        Dim dtUnavailableDays As Data.DataTable = GetUnavailableDays()
        CreateResultsTables()

        If String.IsNullOrEmpty(DeliveryZoneCode) And String.IsNullOrEmpty(DeliveryZoneType) Then

            'Process the delivery date as normal
            Dim newRow As DataRow = dtHeader.NewRow
            newRow("CurrentDate") = d
            newRow("ProjectedDeliveryDate") = d.AddDays(5)
            dtHeader.Rows.Add(newRow)
            Me.ResultDataSet.Tables.Add(Me.dtHeader)

        Else

            'Process the delivery date using tbl_delivery_zone

            'Step 1: Get Todays date and current time. 
            '-----------------------------------------
            'If it is a Saturday or a Sunday then treat as if it is the following Monday and move onto next step. 
            If d.DayOfWeek = DayOfWeek.Sunday Then
                expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
            ElseIf d.DayOfWeek = DayOfWeek.Saturday Then
                expectedDeliveryDate = expectedDeliveryDate.AddDays(2)
            Else
                'Compare current time to DELIVERY_CUT_OFF_TIME.
                'If the current time is after the cut off time then add 1 day to todays date. 
                'If the result of this is Saturday or Sunday then continue on to Monday. 
                'If the result is on a blackout day, then add 1 day to it
                Try
                    Dim deliveryCutOffTime As String = DateTime.Parse(GetDeliveryCutOffTime()).ToString("t")
                    If d.ToString("t") > deliveryCutOffTime Then
                        expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                        If expectedDeliveryDate.DayOfWeek = DayOfWeek.Sunday Then
                            expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                        ElseIf expectedDeliveryDate.DayOfWeek = DayOfWeek.Saturday Then
                            expectedDeliveryDate = expectedDeliveryDate.AddDays(2)
                        ElseIf IsUnavailableDay(expectedDeliveryDate, dtUnavailableDays) Then
                            expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                        End If
                    End If
                Catch ex As Exception
                    Const strError10 As String = "Error during calculation of delivery cut off time"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError10
                        .ErrorNumber = "TACDBDD-10"
                        .HasError = True
                    End With
                End Try
            End If


            'Step 2: Add lead time.
            '----------------------
            'Saturdays and Sundays are to be ignored in this calculation. 
            'This will be done by adding 1 day to the date and keeping a counter. 
            'Each time the day is MON-FRI the counter in incremented if the weekday is not a blackout day. 
            'Do this until counter is equal to DELIVERY_LEAD_TIME
            If Not err.HasError Then
                Try
                    Dim deliveryLeadTime As Integer = Integer.Parse(GetDeliveryLeadTime())
                    If deDelDate.StockLeadTime > 0 Then
                        deliveryLeadTime += deDelDate.StockLeadTime
                    End If
                    Dim i As Integer = 0
                    While i < deliveryLeadTime
                        If expectedDeliveryDate.DayOfWeek = DayOfWeek.Sunday Then
                            expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                        ElseIf expectedDeliveryDate.DayOfWeek = DayOfWeek.Saturday Then
                            expectedDeliveryDate = expectedDeliveryDate.AddDays(2)
                        ElseIf IsUnavailableDay(expectedDeliveryDate, dtUnavailableDays) Then
                            expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                        Else
                            expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                            i = i + 1
                        End If
                    End While
                Catch ex As Exception
                    Const strError11 As String = "Error during calculation of delivery lead time"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError11
                        .ErrorNumber = "TACDBDD-11"
                        .HasError = True
                    End With
                End Try
            End If


            'Step 3: Calculate delivery day from Delivery Zone table
            '-------------------------------------------------------
            'Read records off TBL_DELIVERY_ZONE and calculate the Zone’s next available delivery slot 
            'by checking what days are set to True (i.e. deliveries can be made on that day). 
            'This will be done by taking the date from step 2, working out the day, and seeing if 
            'it’s set to True for this zone. If so then this is the delivery date. 
            'If not then increment the date, check the day and keep going until one is found.

            Dim foundADeliveryDay As Boolean = False
            If Not err.HasError Then
                Try
                    Dim dtDeliveryZone As New DataTable
                    Dim j, k As Integer
                    If deDelDate.HomeDelivery Then
                        ' HOME DELIVERY - check valid days passed down (from Carrier file)
                        ' Try for 28 days
                        j = 0
                        Do Until j = 28 OrElse foundADeliveryDay
                            If IsHomeDeliveryDay(expectedDeliveryDate.DayOfWeek.ToString().ToUpper()) AndAlso (Not IsUnavailableDay(expectedDeliveryDate, dtUnavailableDays)) Then
                                foundADeliveryDay = True
                            Else
                                expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                            End If
                            j += 1
                        Loop
                    Else
                        ' TRADE ORDER - get valid days from Delivery Zone against trade address
                        dtDeliveryZone = GetDeliveryZoneData()
                        For k = 0 To 3
                            For j = 0 To dtDeliveryZone.Columns.Count - 1
                                If dtDeliveryZone.Columns(j).ColumnName = expectedDeliveryDate.DayOfWeek.ToString.ToUpper Then
                                    If (dtDeliveryZone.Rows(0)(j).ToString.ToUpper = "TRUE") AndAlso (Not IsUnavailableDay(expectedDeliveryDate, dtUnavailableDays)) Then
                                        foundADeliveryDay = True
                                        Exit For
                                    Else
                                        expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                                    End If
                                End If
                            Next
                            j = 0
                        Next
                    End If
                Catch ex As Exception
                    Const strError12 As String = "Error during data access with tbl_delivery_zone"
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError12
                        .ErrorNumber = "TACDBDD-12"
                        .HasError = True
                    End With
                End Try
            End If

            'pass the day name on if one has been found
            Dim newRow As DataRow = dtHeader.NewRow
            newRow("CurrentDate") = d
            newRow("ProjectedDeliveryDate") = expectedDeliveryDate
            newRow("DeliveryDateUsable") = foundADeliveryDay
            dtHeader.Rows.Add(newRow)
            Me.ResultDataSet.Tables.Add(Me.dtHeader)

        End If

        Return err
    End Function

    Private Function IsHomeDeliveryDay(ByVal expectedDeliveryDay As String) As Boolean
        Dim blIsHomeDeliveryDay As Boolean = False
        If deDelDate.HomeDeliveryDays <> String.Empty Then
            Select Case expectedDeliveryDay
                Case Is = "MONDAY"
                    If deDelDate.HomeDeliveryDays.Substring(0, 1) = "Y" Then
                        blIsHomeDeliveryDay = True
                    End If
                Case Is = "TUESDAY"
                    If deDelDate.HomeDeliveryDays.Substring(1, 1) = "Y" Then
                        blIsHomeDeliveryDay = True
                    End If
                Case Is = "WEDNESDAY"
                    If deDelDate.HomeDeliveryDays.Substring(2, 1) = "Y" Then
                        blIsHomeDeliveryDay = True
                    End If
                Case Is = "THURSDAY"
                    If deDelDate.HomeDeliveryDays.Substring(3, 1) = "Y" Then
                        blIsHomeDeliveryDay = True
                    End If
                Case Is = "FRIDAY"
                    If deDelDate.HomeDeliveryDays.Substring(4, 1) = "Y" Then
                        blIsHomeDeliveryDay = True
                    End If
                Case Is = "SATURDAY"
                    If deDelDate.HomeDeliveryDays.Substring(5, 1) = "Y" Then
                        blIsHomeDeliveryDay = True
                    End If
                Case Is = "SUNDAY"
                    If deDelDate.HomeDeliveryDays.Substring(6, 1) = "Y" Then
                        blIsHomeDeliveryDay = True
                    End If

            End Select
        Else
            blIsHomeDeliveryDay = True
        End If
        Return blIsHomeDeliveryDay
    End Function

    Private Function IsUnavailableDay(ByVal checkDate As Date, ByVal dtUnavailableDays As Data.DataTable) As Boolean
        Dim isUnavailable As Boolean = False
        Try
            For Each row As DataRow In dtUnavailableDays.Rows
                Dim unavailableDate As New Date
                unavailableDate = CDate(row("DATE"))
                If unavailableDate.ToShortDateString() = checkDate.ToShortDateString() Then
                    isUnavailable = True
                    Exit For
                End If
            Next
        Catch ex As Exception
            isUnavailable = False
        End Try
        Return isUnavailable
    End Function

    Private Function GetThePreferredDeliveryDates() As ErrorObj
        Dim err As New ErrorObj
        Me.ResultDataSet = New DataSet("PreferredDeliveryDates")
        Dim dtAllowedDeliveryDates As New DataTable("PreferredDeliveryDates")
        dtAllowedDeliveryDates.Columns.Add(New DataColumn("DeliveryDates", GetType(Date)))

        Dim d As Date = deDelDate.CurrentDate
        Dim expectedDeliveryDate As Date = d

        Dim dtUnavailableDays As Data.DataTable = GetUnavailableDays()

        'Step 1: Get Todays date and current time. 
        '-----------------------------------------
        'If it is a Saturday or a Sunday then treat as if it is the following Monday and move onto next step. 
        If d.DayOfWeek = DayOfWeek.Sunday Then
            expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
        ElseIf d.DayOfWeek = DayOfWeek.Saturday Then
            expectedDeliveryDate = expectedDeliveryDate.AddDays(2)
        Else
            'Compare current time to DELIVERY_CUT_OFF_TIME.
            'If the current time is after the cut off time then add 1 day to todays date. 
            'If the result of this is Saturday or Sunday then continue on to Monday. 
            Try
                Dim deliveryCutOffTime As String = DateTime.Parse(GetDeliveryCutOffTime()).ToString("t")
                If d.ToString("t") > deliveryCutOffTime Then
                    expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                    If expectedDeliveryDate.DayOfWeek = DayOfWeek.Sunday Then
                        expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                    ElseIf expectedDeliveryDate.DayOfWeek = DayOfWeek.Saturday Then
                        expectedDeliveryDate = expectedDeliveryDate.AddDays(2)
                    End If
                End If
            Catch ex As Exception
                Const strError10 As String = "Error during calculation of delivery cut off time"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError10
                    .ErrorNumber = "TACDBDD-13"
                    .HasError = True
                End With
            End Try
        End If

        If Not err.HasError Then
            Try
                Dim deliveryLeadTime As Integer = Integer.Parse(GetDeliveryLeadTime())
                If deDelDate.StockLeadTime > 0 Then
                    deliveryLeadTime += deDelDate.StockLeadTime
                End If
                Dim i As Integer = 0
                While i < deliveryLeadTime
                    If expectedDeliveryDate.DayOfWeek = DayOfWeek.Sunday Then
                        expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                    ElseIf expectedDeliveryDate.DayOfWeek = DayOfWeek.Saturday Then
                        expectedDeliveryDate = expectedDeliveryDate.AddDays(2)
                    ElseIf IsUnavailableDay(expectedDeliveryDate, dtUnavailableDays) Then
                        expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                    Else
                        expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                        i = i + 1
                    End If
                End While
            Catch ex As Exception
                Const strError11 As String = "Error during calculation of delivery lead time"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError11
                    .ErrorNumber = "TACDBDD-14"
                    .HasError = True
                End With
            End Try
        End If
        If Not err.HasError Then
            Try
                Dim dtDeliveryZone As New DataTable
                dtDeliveryZone = GetDeliveryZoneData()
                Dim maximumDays As Integer = deDelDate.DaysWithinPreferredDate
                Dim tempMaxDays As Integer = 1
                Dim tempIsDeliveryDay As Boolean = False
                While tempMaxDays <= maximumDays
                    tempIsDeliveryDay = False
                    ' Check for home delivery - if so this will go by Carrier delivery days
                    If deDelDate.HomeDelivery Then
                        If IsHomeDeliveryDay(expectedDeliveryDate.DayOfWeek.ToString.ToUpper) AndAlso
                                   Not IsUnavailableDay(expectedDeliveryDate, dtUnavailableDays) Then
                            Dim newRow As DataRow = dtAllowedDeliveryDates.NewRow
                            newRow("DeliveryDates") = expectedDeliveryDate.Date
                            dtAllowedDeliveryDates.Rows.Add(newRow)
                            tempIsDeliveryDay = True

                        End If
                    Else

                        'Trade order - go by delivery zone days
                        For colIndex As Integer = 0 To dtDeliveryZone.Columns.Count - 1
                            If dtDeliveryZone.Columns(colIndex).ColumnName = expectedDeliveryDate.DayOfWeek.ToString.ToUpper AndAlso
                                   Not IsUnavailableDay(expectedDeliveryDate, dtUnavailableDays) Then
                                If dtDeliveryZone.Rows(0)(colIndex).ToString.ToUpper = "TRUE" Then
                                    Dim newRow As DataRow = dtAllowedDeliveryDates.NewRow
                                    newRow("DeliveryDates") = expectedDeliveryDate.Date
                                    dtAllowedDeliveryDates.Rows.Add(newRow)
                                    tempIsDeliveryDay = True
                                    Exit For
                                End If
                            End If
                        Next
                    End If
                    If expectedDeliveryDate.DayOfWeek = DayOfWeek.Sunday Then
                        expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                        If tempIsDeliveryDay Then
                            tempMaxDays = tempMaxDays + 1
                        End If
                    ElseIf expectedDeliveryDate.DayOfWeek = DayOfWeek.Saturday Then
                        expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                        If tempIsDeliveryDay Then
                            tempMaxDays = tempMaxDays + 1
                        End If
                    Else
                        expectedDeliveryDate = expectedDeliveryDate.AddDays(1)
                        tempMaxDays = tempMaxDays + 1
                    End If
                End While

                Me.ResultDataSet.Tables.Add(dtAllowedDeliveryDates)
            Catch ex As Exception
                Const strError12 As String = "Error during data access with tbl_delivery_zone"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError12
                    .ErrorNumber = "TACDBDD-15"
                    .HasError = True
                End With
            End Try
        End If
        Return err
    End Function
End Class
