Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Web
Imports Talent.Common.Utilities

<Serializable()> _
Public Class TalentBasket
    Inherits TalentBase

#Region "Public Properties"

    Public Property MultipleSQLConnectionStrings() As Generic.List(Of String)
    Public Property De_AddPPS() As New DEAddPPS
    Public Property Dep() As New DEAmendBasket
    Public Property DeAmendTicketingItems() As New DEAmendTicketingItems
    Public Property DeAddTicketingItems() As New DEAddTicketingItems
    Public Property De() As New DETicketingItemDetails
    Public Property DeTicketingBasket() As New DETicketingBasket
    Public Property ResultDataSet() As New DataSet
    Public Property OrphanSeatRemaining() As Boolean = False
    Public Property ListOfDEAddTicketingItems() As New List(Of DEAddTicketingItems)
    Public Property AlternativeSeatSelected() As Boolean = False
    Public Property ClearAvailableStandAreaCache() As Boolean = False
    Public Property BasketRequiresRedirectToBookingOrComponentPage() As String = String.Empty
    Public Property WS036RFirstParm() As String = String.Empty
    Public Property BasketHasExceptionSeats() As Boolean = False

#End Region

#Region "Public Functions"

    Public Function AddToBasket() As ErrorObj
        Const ModuleName As String = "AddToBasket"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Dim pa As New DBAmendBasket
        With pa
            .Dep = Dep
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
                Dep = .Dep
            End If
        End With
        Return err
    End Function

    Public Function AddRetailToTicketingBasket(ByVal BasketId As String, ByVal TempOrderId As String, ByVal paymentTaken As Boolean)
        Dim err As New ErrorObj
        Dim basket As New DBAmendBasket
        Dim retailTkt As New DERetailToTicketing
        With retailTkt
            .BasketId = BasketId
            .TempOrderId = TempOrderId
            .PaymentTaken = paymentTaken

            'Populate the retail information required for ticketing.  We only pass the minimum amount of information to 
            'iseries and we retrieve the other information via a web service call after the sale completes.  We use 
            'order id and line number to identify the basket item.
            Dim dt As DataTable = TDataObjects.OrderSettings.TblOrderDetail.GetOrderDetailRecordsByTempOrderID(TempOrderId)
            For Each dr As DataRow In dt.Rows
                Dim retailTktProd As New DERetailToTicketingProduct
                With retailTktProd
                    .LineNumber = dr("LINE_NUMBER")
                    .Price = dr("PURCHASE_PRICE_GROSS")
                    .Quantity = CType(dr("QUANTITY"), Integer)
                End With
                .ListOfDERetailToTicketingProducts.Add(retailTktProd)
            Next

            ' We need to add the delivery and promotion costs as well.
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                'Read the first line because all of the following values come from order header
                Dim dr As DataRow = dt.Rows(0)
                .DeliveryPrice = dr("TOTAL_DELIVERY_GROSS")
                .PromotionPrice += dr("PROMOTION_VALUE1")
            End If
        End With

        Const ModuleName As String = "AddRetailToTicketingBasket"
        Settings.ModuleName = ModuleName
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        With basket
            .DERetailToTicketing = retailTkt
            .Dep = Dep
            .Settings = Settings
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
            Dep = .Dep
        End With
        Return err
    End Function

    'Public Function DeleteFromBasket() As ErrorObj
    '    Const ModuleName As String = "DeleteFromBasket"
    '    Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
    '    Dim err As New ErrorObj
    '    Dim pa As New DBAmendBasket
    '    '-------------------------------------------------------------------------------------
    '    With pa
    '        .Dep = Dep
    '        .Settings = Settings
    '        err = .ValidateAgainstDatabase()
    '        Dep = .Dep
    '    End With
    '    Return err
    'End Function

    'Public Function ReplaceBasket() As ErrorObj
    '    Const ModuleName As String = "ReplaceBasket"
    '    Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
    '    Dim err As New ErrorObj
    '    Dim pa As New DBAmendBasket
    '    With pa
    '        .Dep = Dep
    '        .Settings = Settings
    '        err = .ValidateAgainstDatabase()
    '        Dep = .Dep
    '    End With
    '    Return err
    'End Function

    Public Function DeleteMultipleBaskets() As ErrorObj
        Const ModuleName As String = "DeleteMultipleBaskets"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Dim pa As New DBAmendBasket
        With pa
            .Dep = Dep
            .Dep.DeleteMultipleBaskets = True
            .Dep.BasketIdList = Dep.BasketIdList
            .Dep.DeleteModule = Dep.DeleteModule
            .Settings = Settings
            err = .AccessDatabase()
            Dep = .Dep
        End With
        Return err
    End Function

    Public Function DeleteMultipleBaskets_MultiDBs() As ErrorObj
        Dim err As New ErrorObj
        Dim pa As New DBAmendBasket
        For Each connStr As String In MultipleSQLConnectionStrings
            Settings.DatabaseType1 = "1"
            Settings.DestinationDatabase = "SQL2005"
            Settings.FrontEndConnectionString = connStr
            err = New ErrorObj
            With pa
                .Dep = Dep
                .Dep.DeleteMultipleBaskets = True
                .Dep.BasketIdList = Dep.BasketIdList
                .Dep.DeleteModule = Dep.DeleteModule
                .Settings = Settings
                err = .AccessDatabase()
                Dep = .Dep
            End With
        Next
        Return err
    End Function

    'Public Function AddTicketingItems() As ErrorObj

    '    Const ModuleName As String = "AddTicketingItems"
    '    TalentCommonLog(ModuleName, DeAddTicketingItems.CustomerNumber, "Talent.Common Request = DeAddTicketingItems=" & DeAddTicketingItems.LogString)
    '    Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
    '    Dim err As New ErrorObj
    '    '-------------------------------------------------------------------------------------
    '    '   No Cache possible due to way the process works
    '    '
    '    Settings.ModuleName = ModuleName
    '    Dim dbBasket As New DBAmendBasket
    '    With dbBasket
    '        '---------------------------------------------------------------
    '        ' if stand and area is blank, attempt to get stand and area from 
    '        ' table by pricing category
    '        '---------------------------------------------------------------
    '        If DeAddTicketingItems.StandCode = String.Empty Then
    '            Dim cmd As New Data.SqlClient.SqlCommand
    '            Dim availableSeats As Int32 = 0
    '            Dim foundSeats As Boolean = False
    '            cmd.Connection = New Data.SqlClient.SqlConnection(Settings.FrontEndConnectionString)

    '            Try
    '                cmd.Connection.Open()

    '                If cmd.Connection.State = ConnectionState.Open Then
    '                    cmd.CommandText = " SELECT * FROM TBL_TICKETING_CATEGORY_AREAS WITH (NOLOCK)  WHERE" & _
    '                                     "  STADIUM = @STADIUM AND PRICING_CATEGORY = @PRICING_CATEGORY"

    '                    With cmd.Parameters
    '                        .Clear()
    '                        .Add("@STADIUM", SqlDbType.NVarChar).Value = DeAddTicketingItems.Stadium1
    '                        .Add("@PRICING_CATEGORY", SqlDbType.NVarChar).Value = DeAddTicketingItems.PriceBand01
    '                    End With
    '                    Dim dr As Data.SqlClient.SqlDataReader = cmd.ExecuteReader
    '                    If dr.HasRows Then
    '                        While dr.Read AndAlso Not foundSeats
    '                            availableSeats = CInt(dr("TOTAL_SEATS_FOR_CATEGORY")) - CInt(dr("BOOKED_SEATS"))
    '                            If availableSeats >= DeAddTicketingItems.Quantity01 Then
    '                                '-----------------------------------------
    '                                ' attempt to book seats in this stand/area
    '                                '-----------------------------------------
    '                                DeAddTicketingItems.StandCode = dr("STAND").ToString
    '                                DeAddTicketingItems.AreaCode = dr("AREA").ToString
    '                                DeAddTicketingItems.ReservationCode = dr("RESERVATION_CODE").ToString
    '                                .DeAddTicketingItems = DeAddTicketingItems
    '                                .Settings = Settings
    '                                err = .AccessDatabase()
    '                                ResultDataSet = .ResultDataSet
    '                                If .DeAddTicketingItems.ErrorCode.Trim = String.Empty Then
    '                                    foundSeats = True
    '                                    '-------------------
    '                                    ' deduct from totals
    '                                    '-------------------
    '                                Else
    '                                    '-----------------
    '                                    ' reset error code
    '                                    '-----------------
    '                                    DeAddTicketingItems.ErrorCode = String.Empty
    '                                End If

    '                            End If
    '                        End While
    '                        If Not foundSeats Then
    '                            err.HasError = True
    '                            err.ErrorMessage = "No available seats for category"
    '                            err.ErrorNumber = "TACTABAS-03"
    '                            DeAddTicketingItems.ErrorCode = err.ErrorNumber
    '                        End If
    '                    Else
    '                        err.HasError = True
    '                        err.ErrorMessage = "No available seats for category"
    '                        err.ErrorNumber = "TACTABAS-01"
    '                        DeAddTicketingItems.ErrorCode = err.ErrorNumber
    '                    End If
    '                    dr.Close()

    '                End If
    '            Catch ex As Exception
    '                err.HasError = True
    '                err.ErrorMessage = "Failed to read tbl_ticketing_category WITH (NOLOCK) "
    '                err.ErrorNumber = "TACTABAS-02"
    '                DeAddTicketingItems.ErrorCode = err.ErrorNumber
    '            Finally
    '                Try
    '                    cmd.Connection.Close()
    '                Catch ex As Exception
    '                End Try
    '            End Try
    '        Else
    '            '---------------------------------------------
    '            ' already have stand and area, no need to loop
    '            '---------------------------------------------
    '            .DeAddTicketingItems = DeAddTicketingItems
    '            .Settings = Settings
    '            err = .AccessDatabase()
    '            ResultDataSet = .ResultDataSet
    '        End If

    '    End With

    '    TalentCommonLog(ModuleName, DeAddTicketingItems.CustomerNumber, ResultDataSet, err)
    '    Return err
    'End Function

    Public Function AddTicketingItemsReturnBasket() As ErrorObj
        Const ModuleName As String = "AddTicketingItemsReturnBasket"
        TalentCommonLog(ModuleName, DeAddTicketingItems.CustomerNumber, "Talent.Common Request = DeAddTicketingItems=" & DeAddTicketingItems.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbBasket As New DBAmendBasket
        With dbBasket
            .DeAddTicketingItems = DeAddTicketingItems
            .Settings = Settings
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
            OrphanSeatRemaining = .OrphanSeatRemaining
            AlternativeSeatSelected = .AlternativeSeatSelected
            ClearAvailableStandAreaCache = .ClearAvailableStandAreaCache
            BasketRequiresRedirectToBookingOrComponentPage = .BasketRequiresRedirectToBookingOrComponentPage
            BasketHasExceptionSeats = .BasketHasExceptionSeats
            If BasketHasExceptionSeats Then
                GetBasketExceptionSeats(dbBasket)
            End If
            err = ProcessNewBasket(DeAddTicketingItems.Source, DeAddTicketingItems.SessionId, ResultDataSet)
        End With
        TalentCommonLog(ModuleName, DeAddTicketingItems.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function AddMultipleTicketingItemsReturnBasket() As ErrorObj
        Const ModuleName As String = "AddMultipleTicketingItemsReturnBasket"
        Dim err As New ErrorObj
        Dim dbBasket As New DBAmendBasket
        TalentCommonLog(ModuleName, De.CustomerNo, "Talent.Common Request")
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Settings.ModuleName = ModuleName
        With dbBasket
            .De = De
            .ListOfDEAddTicketingItems = ListOfDEAddTicketingItems
            .Settings = Settings
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
            'This list object stores the error details returned from the WS621R call as well as the product information
            ListOfDEAddTicketingItems = .ListOfDEAddTicketingItems
            BasketHasExceptionSeats = .BasketHasExceptionSeats
            If BasketHasExceptionSeats Then
                GetBasketExceptionSeats(dbBasket)
            End If
            err = ProcessNewBasket(De.Src, De.SessionId, ResultDataSet)
        End With
        TalentCommonLog(ModuleName, De.CustomerNo, ResultDataSet, err)
        Return err
    End Function

    Public Function AddTicketingReservedItems() As ErrorObj
        Const ModuleName As String = "AddTicketingReservedItems"
        TalentCommonLog(ModuleName, DeAddTicketingItems.CustomerNumber, "Talent.Common Request = DeAddTicketingItems=" & DeAddTicketingItems.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbBasket As New DBAmendBasket
        With dbBasket
            .DeAddTicketingItems = DeAddTicketingItems
            .Settings = Settings
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
        End With
        TalentCommonLog(ModuleName, DeAddTicketingItems.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function AddTicketingReservedItemsReturnBasket() As ErrorObj
        Const ModuleName As String = "AddTicketingReservedItemsReturnBasket"
        TalentCommonLog(ModuleName, DeAddTicketingItems.CustomerNumber, "Talent.Common Request = DeAddTicketingItems=" & DeAddTicketingItems.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbBasket As New DBAmendBasket
        With dbBasket
            .DeAddTicketingItems = DeAddTicketingItems
            .Settings = Settings
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
            BasketHasExceptionSeats = .BasketHasExceptionSeats
            If BasketHasExceptionSeats Then
                GetBasketExceptionSeats(dbBasket)
            End If
            err = ProcessNewBasket(DeAddTicketingItems.Source, DeAddTicketingItems.SessionId, ResultDataSet)
        End With
        TalentCommonLog(ModuleName, DeAddTicketingItems.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function AddSeasonTicketRenewalsReturnBasket() As ErrorObj
        Const ModuleName As String = "AddSeasonTicketRenewalsReturnBasket"
        TalentCommonLog(ModuleName, DeAddTicketingItems.CustomerNumber, "Talent.Common Request = DeAddTicketingItems=" & DeAddTicketingItems.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbBasket As New DBAmendBasket
        With dbBasket
            .DeAddTicketingItems = DeAddTicketingItems
            .Settings = Settings
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
            BasketHasExceptionSeats = .BasketHasExceptionSeats
            If BasketHasExceptionSeats Then
                GetBasketExceptionSeats(dbBasket)
            End If
            err = ProcessNewBasket(DeAddTicketingItems.Source, DeAddTicketingItems.SessionId, ResultDataSet)
        End With
        TalentCommonLog(ModuleName, DeAddTicketingItems.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function RemoveTicketingItems() As ErrorObj
        Const ModuleName As String = "RemoveTicketingItems"
        TalentCommonLog(ModuleName, De.CustomerNo, "Talent.Common Request = De=" & De.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbBasket As New DBAmendBasket
        With dbBasket
            .Settings = Settings
            .De = De
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
        End With
        TalentCommonLog(ModuleName, De.CustomerNo, ResultDataSet, err)
        Return err
    End Function

    Public Function RemoveTicketingItemsReturnBasket() As ErrorObj
        Const ModuleName As String = "RemoveTicketingItemsReturnBasket"
        TalentCommonLog(ModuleName, De.CustomerNo, "Talent.Common Request = De=" & De.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbBasket As New DBAmendBasket
        With dbBasket
            .Settings = Settings
            .De = De
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
            OrphanSeatRemaining = .OrphanSeatRemaining
            BasketHasExceptionSeats = .BasketHasExceptionSeats
            If BasketHasExceptionSeats Then
                GetBasketExceptionSeats(dbBasket)
            End If
            err = ProcessNewBasket(De.Src, De.SessionId, ResultDataSet)
        End With
        TalentCommonLog(ModuleName, De.CustomerNo, ResultDataSet, err)
        Return err
    End Function

    Public Function RetrieveTicketingItems() As ErrorObj
        Const ModuleName As String = "RetrieveTicketingItems"
        TalentCommonLog(ModuleName, De.CustomerNo, "Talent.Common Request = De=" & De.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbBasket As New DBAmendBasket
        With dbBasket
            .Settings = Settings
            .De = De
            .WS036RFirstParm = WS036RFirstParm
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
            BasketHasExceptionSeats = .BasketHasExceptionSeats
            If BasketHasExceptionSeats Then
                GetBasketExceptionSeats(dbBasket)
            End If
            err = ProcessNewBasket(De.Src, De.SessionId, ResultDataSet)
        End With
        TalentCommonLog(ModuleName, De.CustomerNo, ResultDataSet, err)
        Return err
    End Function

    Public Function AmendTicketingItems() As ErrorObj
        Const ModuleName As String = "AmendTicketingItems"
        TalentCommonLog(ModuleName, DeAmendTicketingItems.CustomerNo, "Talent.Common Request = DeAmendTicketingItems=" & DeAmendTicketingItems.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbBasket As New DBAmendBasket
        With dbBasket
            .Settings = Settings
            .DeAmendTicketingItems = DeAmendTicketingItems
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
        End With
        TalentCommonLog(ModuleName, DeAmendTicketingItems.CustomerNo, ResultDataSet, err)
        Return err
    End Function

    Public Function AmendTicketingItemsReturnBasket() As ErrorObj
        Const ModuleName As String = "AmendTicketingItemsReturnBasket"
        TalentCommonLog(ModuleName, DeAmendTicketingItems.CustomerNo, "Talent.Common Request = DeAmendTicketingItems=" & DeAmendTicketingItems.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbBasket As New DBAmendBasket
        With dbBasket
            .Settings = Settings
            .DeAmendTicketingItems = DeAmendTicketingItems
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
            BasketHasExceptionSeats = .BasketHasExceptionSeats
            If BasketHasExceptionSeats Then
                GetBasketExceptionSeats(dbBasket)
            End If
            err = ProcessNewBasket(DeAmendTicketingItems.Src, DeAmendTicketingItems.SessionID, ResultDataSet)
        End With
        TalentCommonLog(ModuleName, DeAmendTicketingItems.CustomerNo, ResultDataSet, err)
        Return err
    End Function

    Public Function GenerateTicketingBasketID() As ErrorObj
        Const ModuleName As String = "GenerateTicketingBasketID"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Try
            Settings.ModuleName = ModuleName
            De.SessionId = ""

            ' Retrieve the current date and time
            Dim intMillisecs As Integer = System.DateTime.Now.Millisecond
            Dim intSeconds As Integer = System.DateTime.Now.Second
            Dim intMinute As Integer = System.DateTime.Now.Minute
            Dim intHour As Integer = System.DateTime.Now.Hour
            Dim intDay As Integer = System.DateTime.Now.Day
            Dim intMonth As Integer = System.DateTime.Now.Month
            Dim intYear As Integer = System.DateTime.Now.Year

            ' Determine the number of seconds and the number of minutes since midnight
            Dim lngSecsSince As Long = (intHour * 3600) + (intMinute * 60) + intSeconds
            Dim lngMinsSince As Long = (intHour * 60) + intMinute

            ' Jig 'em around
            Dim lngSecs As Long = 10000000 + lngSecsSince.ToString & Mid(intMillisecs.ToString, 1, 3)
            Dim lngDate As Long = 10000000 + (intYear * 10000) + (intMonth * 100) + intDay
            Dim lngMins As Long = 1000 + lngMinsSince

            ' Construct the session id 
            Dim sessionID As String = lngDate.ToString & (CType(De.CustomerNo.Trim, Int64) * 2) & lngMins.ToString & lngSecs.ToString

            'Pad the end of the session ID with 'random' digits up to 36 maximum.
            Dim padSpace As Integer = 36 - (sessionID.Trim.Length)
            If (padSpace > 0) Then
                Randomize()
                Dim rand As Long = CLng(((999999999999999999 * Rnd()) + 1))
                Dim randStr As String = Right("000000000000000000" & rand.ToString.Trim, padSpace)
                sessionID = sessionID.Trim & randStr.Trim
            End If

            'Set the session ID
            De.SessionId = sessionID

        Catch ex As Exception
            Const strError As String = "Failed to generate the session id"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACTABAS-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Public Function RemoveTicketingExpiredBaskets() As ErrorObj
        Const ModuleName As String = "RemoveTicketingExpiredBaskets"
        If String.IsNullOrEmpty(Settings.DestinationDatabase) OrElse _
            String.IsNullOrEmpty(Settings.BackOfficeConnectionString) Then
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        End If

        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbBasket As New DBAmendBasket
        With dbBasket
            .Settings = Settings
            .De = De
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
        End With
        Return err
    End Function

    Public Function RemoveExpiredBaskets() As ErrorObj
        Dim err As New ErrorObj

        'Remove the ticketing expired baskets
        err = RemoveTicketingExpiredBaskets()
        If Not ResultDataSet Is Nothing And Not err.HasError Then

            ' Was the call to the back end successful
            If ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> "E" Then

                'Construct the generic list
                Dim dtBasketIds As DataTable = ResultDataSet.Tables(1)
                Dim basketIds As New Generic.List(Of String)
                Dim intLoopCount As Integer = 0

                'Do we have any basket to remove
                If dtBasketIds.Rows.Count > 0 Then
                    Do While intLoopCount < dtBasketIds.Rows.Count
                        basketIds.Add(dtBasketIds.Rows(intLoopCount).Item("BasketId"))
                        intLoopCount = intLoopCount + 1
                    Loop

                    'Remove the front end baskets
                    Dep = New DEAmendBasket
                    Dep.BasketIdList = basketIds
                    Dep.DeleteModule = "Ticketing"
                    err = DeleteMultipleBaskets()
                End If
            End If
        End If
        Return err
    End Function

    Public Function RemoveExpiredBaskets_MultiDBs() As ErrorObj
        Dim err As New ErrorObj

        'Remove the ticketing expired baskets
        err = RemoveTicketingExpiredBaskets()
        If Not ResultDataSet Is Nothing And Not err.HasError Then

            ' Was the call to the back end successful
            If ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> "E" Then

                'Construct the generic list
                Dim dtBasketIds As DataTable = ResultDataSet.Tables(1)
                Dim basketIds As New Generic.List(Of String)
                Dim intLoopCount As Integer = 0

                'Do we have any basket to remove
                If dtBasketIds.Rows.Count > 0 Then
                    Do While intLoopCount < dtBasketIds.Rows.Count
                        basketIds.Add(dtBasketIds.Rows(intLoopCount).Item("BasketId"))
                        intLoopCount = intLoopCount + 1
                    Loop

                    'Remove the front end baskets
                    Dep = New DEAmendBasket
                    Dep.BasketIdList = basketIds
                    Dep.DeleteModule = "Ticketing"
                    err = DeleteMultipleBaskets_MultiDBs()
                End If

            End If
        End If
        Return err
    End Function

    Public Function AddPPSToBasket() As ErrorObj
        Const ModuleName As String = "AddPPSItemsToBasket"
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Dim dbBasket As New DBAmendBasket
        Settings.ModuleName = ModuleName
        With dbBasket
            .De_AddPPS = De_AddPPS
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase()
                De_AddPPS = dbBasket.De_AddPPS
                ResultDataSet = dbBasket.ResultDataSet
                BasketHasExceptionSeats = .BasketHasExceptionSeats
                If BasketHasExceptionSeats Then
                    GetBasketExceptionSeats(dbBasket)
                End If
                err = ProcessNewBasket(De_AddPPS.Source, De_AddPPS.SessionId, ResultDataSet)
            End If
        End With
        Return err
    End Function

    Public Function ClearAndAddTicketingItemsReturnBasket() As ErrorObj
        Const ModuleName As String = "ClearAndAddTicketingItemsReturnBasket"
        TalentCommonLog(ModuleName, DeAddTicketingItems.CustomerNumber, "Talent.Common Request = DeAddTicketingItems=" & DeAddTicketingItems.LogString)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbBasket As New DBAmendBasket
        With dbBasket
            .DeAddTicketingItems = DeAddTicketingItems
            .Settings = Settings
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
            OrphanSeatRemaining = .OrphanSeatRemaining
            BasketRequiresRedirectToBookingOrComponentPage = .BasketRequiresRedirectToBookingOrComponentPage
            BasketHasExceptionSeats = .BasketHasExceptionSeats
            If BasketHasExceptionSeats Then
                GetBasketExceptionSeats(dbBasket)
            End If
            err = ProcessNewBasket(DeAddTicketingItems.Source, DeAddTicketingItems.SessionId, ResultDataSet)
        End With
        TalentCommonLog(ModuleName, DeAddTicketingItems.CustomerNumber, ResultDataSet, err)
        Return err
    End Function

    Public Function VerifyAndUpdateExtOrderNumberToBasket() As ErrorObj
        Const ModuleName As String = "VerifyAndUpdateExtOrderNumberToBasket"
        TalentCommonLog(ModuleName, Dep.BasketId, Dep.BasketVerificationMode & ";" & Dep.ExternalOrderNumber)
        Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
        Dim err As New ErrorObj
        Settings.ModuleName = ModuleName
        Dim dbBasket As New DBAmendBasket
        With dbBasket
            .Dep = Dep
            .Settings = Settings
            err = .AccessDatabase()
            ResultDataSet = .ResultDataSet
        End With
        TalentCommonLog(ModuleName, Dep.BasketId, ResultDataSet, err)
        Return err
    End Function

    ''' <summary>
    ''' Get the season ticket exception seats (Call WS006S)
    ''' If there are any exception seats, add to a new table and insert the table into the existing basket ResultDataSet
    ''' </summary>
    ''' <param name="dbBasket">Pass the existing DBBasket object so that the properties are set correctly.</param>
    ''' <returns>An error object with detailed errors</returns>
    ''' <remarks></remarks>
    Public Function GetBasketExceptionSeats(ByRef dbBasket As DBAmendBasket) As ErrorObj
        Const moduleName As String = "GetBasketExceptionSeats"
        Me.GetConnectionDetails(Settings.BusinessUnit, String.Empty, moduleName)
        Dim err As New ErrorObj
        Settings.ModuleName = moduleName
        With dbBasket
            .Settings = Settings
            err = .AccessDatabase()
            If Not err.HasError AndAlso .ResultDataSet IsNot Nothing Then
                Dim dtBasketDetailExceptions As DataTable = .ResultDataSet.Tables("BasketDetailExceptions")
                BasketHasExceptionSeats = (dtBasketDetailExceptions.Rows.Count > 0)
                If BasketHasExceptionSeats Then
                    Dim tempTable As New DataTable
                    tempTable = .ResultDataSet.Tables("BasketDetailExceptions").Copy
                    ResultDataSet.Tables.Add(tempTable)
                End If
            Else
                BasketHasExceptionSeats = False
            End If
        End With
        Return err
    End Function

#End Region

End Class