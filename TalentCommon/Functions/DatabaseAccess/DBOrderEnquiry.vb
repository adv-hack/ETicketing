Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Text
Imports IBM.Data.DB2.iSeries

#Region "DBOrderEnquiryHeader"

<Serializable()> _
Public Class DBOrderEnquiryHeader
    Inherits DBAccess


End Class

#End Region

#Region "DBOrderEnquiryDetails"

<Serializable()> _
Public Class DBOrderEnquiryDetail
    Inherits DBAccess

    Private _dep As New DEOrder

    Private _dtDetails As New DataTable

    Private _parmTRAN As String

    Private _dicClubCode As New Dictionary(Of String, String)

    Public Property Dep() As DEOrder
        Get
            Return _dep
        End Get
        Set(ByVal value As DEOrder)
            _dep = value
        End Set
    End Property

    Private Property ParmTRAN() As String
        Get
            Return _parmTRAN
        End Get
        Set(ByVal value As String)
            _parmTRAN = value
        End Set
    End Property

    Public Property dtDetails() As DataTable
        Get
            Return _dtDetails
        End Get
        Set(ByVal value As DataTable)
            _dtDetails = value
        End Set
    End Property

    Public Function ReadOrderEnquiryDetails(Optional ByVal opendb As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------
        Select Case Settings.DestinationDatabase
            Case Is = SYSTEM21
                If opendb Then err = System21Open()
                If Not err.HasError Then err = ReadOrderEnquiryDetailsSystem21()
                If opendb Then System21Close()
            Case Is = TALENTTKT
                If opendb Then err = TALENTTKTOpen()
                If Not err.HasError Then err = AccessDatabase_WS005R()
                If opendb Then TALENTTKTClose()

        End Select
        Return err
    End Function

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj

        Dim err As ErrorObj
        Select Case _settings.ModuleName
            Case Is = "OrderEnquiryDetail"
                err = AccessDatabase_WS005R()
            Case Is = "TransactionEnquiryDetails"
                err = AccessDatabase_WS101R()
            Case Else
                err = AccessDatabase_WS005R()
        End Select


        Return err
    End Function

    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj

        Dim err As ErrorObj
        '---------------
        ' Read the order
        '---------------
        err = ReadOrderEnquiryDetailsSystem21()

        Return err
    End Function

    Private Function ReadOrderEnquiryDetailsSystem21() As ErrorObj
        Dim err As New ErrorObj
        Dim intPosition As Integer = 0
        '---------------------------------------------------------------------------
        dtDetails = New DataTable("OrderEnquiryDetails")
        ResultDataSet = New DataSet
        ResultDataSet.Tables.Add(dtDetails)

        If dtDetails.Columns.Count = 0 Then
            AddColumnsToDataTables()
        End If

        Return err
    End Function

    Private Sub AddColumnsToDataTables()
        '---------------------------------------------------------------------------
        With dtDetails.Columns
            .Add("SaleDate", GetType(String))
            .Add("ProductDescription", GetType(String))
            .Add("Seat", GetType(String))
            .Add("SalePrice", GetType(Decimal))
            .Add("BatchReference", GetType(String))
            .Add("PaymentReference", GetType(String))
            .Add("StatusCode", GetType(String))
            .Add("LoyaltyPointsExpired", GetType(Boolean))
            .Add("LoyaltyPoints", GetType(String))
            .Add("PromotionID", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("ProductDate", GetType(String))
            .Add("ProductType", GetType(String))
            .Add("OriginalPrice", GetType(Decimal))
            .Add("CatCancelWeb", GetType(Boolean))
            .Add("CatCancelAgent", GetType(Boolean))
            .Add("PriceBand", GetType(String))
            .Add("CustomerNumber", GetType(String))
            .Add("CustomerName", GetType(String))
            .Add("PriceBandDesc", GetType(String))
            .Add("StadiumCode", GetType(String))
            .Add("CatAmmendWeb", GetType(Boolean))
            .Add("CatAmmendAgent", GetType(Boolean))
            .Add("CatTransferWeb", GetType(Boolean))
            .Add("CatTransferAgent", GetType(Boolean))
            .Add("FeesCode", GetType(String))
            .Add("IsPrintable", GetType(Boolean))
            .Add("RRN", GetType(String))
            .Add("CallId", GetType(Long))
            .Add("IsProductBundle", GetType(Boolean))
            .Add("RelatingBundleProduct", GetType(String))
            .Add("RelatingBundleSeat", GetType(DESeatDetails))
            .Add("Unreserved", GetType(Boolean))
            .Add("Roving", GetType(Boolean))
            .Add("ComponentID", GetType(Long))
        End With
        '---------------------------------------------------------------------------
    End Sub

    Private Function AccessDatabase_WS101R() As ErrorObj
        Dim err As New ErrorObj
        Dim intPosition As Integer = 0
        Dim item1 As String = String.Empty
        Dim item2 As String = String.Empty
        Dim item3 As String = String.Empty
        Dim item4 As String = String.Empty
        Dim item5 As String = String.Empty
        Dim moreResults As Boolean = True
        Dim firstCall As Boolean = True
        Dim txs As String = String.Empty
        Dim tempClubCode As String = String.Empty

        Dim nextStartingNumber As Integer = 0
        Dim totalToReturn As Integer = 0
        Dim lastDetailReturned As Integer = 0
        '---------------------------------------------------------------------------
        dtDetails = New DataTable("TransactionHeaderDetails")
        ResultDataSet = New DataSet
        ResultDataSet.Tables.Add(dtDetails)

        If dtDetails.Columns.Count = 0 Then
            With dtDetails.Columns
                .Add("PaymentReference", GetType(String))
                .Add("Date", GetType(String))
                .Add("Member", GetType(String))
                .Add("Value", GetType(Decimal))
                .Add("NumberOfItems", GetType(String))
                .Add("PaymentType", GetType(String))
                .Add("ClubCode", GetType(String))
                .Add("CallId", GetType(Long))
            End With
        End If

        'Create the Status data table
        Dim dtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(dtStatusResults)
        With dtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("BankedTotal", GetType(String))
            .Add("RewardTotal", GetType(String))
            .Add("LoyaltyActive", GetType(String))
            .Add("LoyaltyPoints", GetType(String))
        End With

        'Create the ClubCode table
        Dim dtClubCode As New DataTable("ClubCode")
        ResultDataSet.Tables.Add(dtClubCode)
        With dtClubCode.Columns
            .Add("ClubCode", GetType(String))
        End With

        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                  "/WS101R(@PARAM1,@PARAM2)"
        Dim parmInput As iDB2Parameter
        Dim parmInput2 As iDB2Parameter
        Dim PARMOUT As String = String.Empty
        Dim PARMOUT2 As String = String.Empty

        If Not err.HasError Then
            While moreResults

                Try
                    Dim det As New DETicketingItemDetails
                    If Dep.CollDEOrders.Count > 0 Then
                        det = Dep.CollDEOrders.Item(1)
                    Else
                        det.ProductCode = ""
                        det.Type = ""
                    End If
                    Dim webUser As String = String.Empty
                    If Not Settings.OriginatingSource = String.Empty Then
                        webUser = Settings.OriginatingSource
                    Else
                        webUser = Settings.Company
                    End If

                    cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
                    parmInput = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
                    parmInput.Value = Utilities.FixStringLength(" ", 4938) & _
                                       Utilities.FixStringLength(txs, 36) & _
                                       Utilities.PadLeadingZeros(Dep.PaymentReference, 15) & _
                                       Utilities.FixStringLength(Dep.FromDate, 8) & _
                                       Utilities.FixStringLength(Dep.ToDate, 8) & _
                                       Utilities.FixStringLength(Settings.AccountNo1, 12) & _
                                       Utilities.FixStringLength(" ", 83) & _
                                       Utilities.FixStringLength(Settings.OriginatingSource.ToString, 10) & _
                                           Utilities.FixStringLength("W", 1) & _
                                       Utilities.FixStringLength("00", 2) & _
                                       Utilities.FixStringLength(" ", 3) & _
                                       Utilities.FixStringLength(" ", 1) & _
                                       Utilities.FixStringLength("  ", 2) & _
                                       Utilities.FixStringLength(" ", 1)
                    parmInput.Direction = ParameterDirection.InputOutput
                    parmInput2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 20000)
                    parmInput2.Value = Utilities.FixStringLength(" ", 20000)
                    parmInput2.Direction = ParameterDirection.InputOutput

                    cmdSELECT.ExecuteNonQuery()
                    PARMOUT = cmdSELECT.Parameters(Param1).Value.ToString
                    PARMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString
                Catch ex As Exception
                    Const strError2 As String = "Error during database access"
                    moreResults = False
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError2
                        .ErrorNumber = "TACDBOENQ04"
                        .HasError = True
                    End With


                End Try

                If Not err.HasError Then
                    '----------------
                    ' Process Results
                    '----------------
                    Try
                        txs = PARMOUT.Substring(4938, 36)
                        Dim iCounter As Integer
                        Dim iPosition As Integer = 0
                        Dim sWork As String = PARMOUT2
                        Dim dr As DataRow = Nothing
                        Dim strQty As String = String.Empty
                        Dim tempValue As Decimal = 0
                        For iCounter = 0 To 99
                            iPosition = iCounter * 200
                            If sWork.Substring(iPosition, 50).Trim > String.Empty Then
                                dr = Nothing
                                dr = dtDetails.NewRow()

                                If sWork.Substring(iPosition, 2) <> String.Empty Then
                                    dr("PaymentReference") = sWork.Substring(iPosition, 15).Trim
                                    dr("Date") = sWork.Substring(iPosition + 15, 10)
                                    dr("Member") = sWork.Substring(iPosition + 25, 12).Trim
                                    tempValue = Utilities.FormatPrice(sWork.Substring(iPosition + 37, 19))
                                    dr("NumberOfItems") = sWork.Substring(iPosition + 56, 5)
                                    dr("PaymentType") = sWork.Substring(iPosition + 61, 2).Trim()
                                    tempClubCode = (sWork.Substring(iPosition + 63, 50)).Trim
                                    dr("ClubCode") = tempClubCode
                                    dr("CallId") = Utilities.CheckForDBNull_BigInt(sWork.Substring(iPosition + 114, 13).Trim())
                                    AddClubCode(tempClubCode)
                                    'position 68 decide the value is -ve or +ve
                                    'P or N
                                    If sWork.Substring(iPosition + 113, 1).Trim = "N" Then
                                        tempValue = tempValue * (-1)
                                    End If
                                    dr("Value") = tempValue
                                    dtDetails.Rows.Add(dr)
                                End If

                            Else
                                Exit For
                            End If
                        Next

                        If PARMOUT.Substring(5116, 1) = "1" Or PARMOUT.Substring(5119, 1) <> " " Then
                            moreResults = False
                        End If

                        'Populate Status Table
                        If dtStatusResults.Rows.Count = 0 Then
                            Dim dsr As DataRow = Nothing
                            dsr = dtStatusResults.NewRow()
                            dsr("ErrorOccurred") = PARMOUT.Substring(5119, 1)
                            dsr("ReturnCode") = PARMOUT.Substring(5117, 2)

                            dtStatusResults.Rows.Add(dsr)
                        End If

                    Catch ex As Exception
                        Const strError2 As String = "Error processing results"
                        moreResults = False
                        With err
                            .ErrorMessage = ex.Message
                            .ErrorStatus = strError2
                            .ErrorNumber = "TACDBOENQ05"
                            .HasError = True
                        End With
                    End Try
                Else
                    moreResults = False
                End If
            End While

            'Populate clubcode table
            If _dicClubCode.Count > 0 Then
                Dim dcr As DataRow = Nothing
                For Each kvClubCode As KeyValuePair(Of String, String) In _dicClubCode
                    If kvClubCode.Key.Length > 0 Then
                        dcr = dtClubCode.NewRow()
                        dcr("ClubCode") = kvClubCode.Key
                        dtClubCode.Rows.Add(dcr)
                    End If
                Next
            End If

        End If

        Return err
    End Function

    ''' <summary>
    ''' Adds the club code to dictionary.
    ''' </summary>
    ''' <param name="clubCode">The club code.</param>
    Private Sub AddClubCode(ByVal clubCode As String)
        If Not _dicClubCode.ContainsKey(clubCode) Then
            _dicClubCode.Add(clubCode, clubCode)
        End If
    End Sub

    Private Function AccessDatabase_WS005R() As ErrorObj
        Dim err As New ErrorObj
        Dim intPosition As Integer = 0
        Dim item1 As String = String.Empty
        Dim item2 As String = String.Empty
        Dim item3 As String = String.Empty
        Dim item4 As String = String.Empty
        Dim item5 As String = String.Empty
        Dim moreResults As Boolean = True
        Dim firstCall As Boolean = True
        Dim nextStartingNumber As Integer = Dep.LastRecordNumber
        Dim totalToReturn As Integer = Dep.TotalRecords
        Dim lastDetailReturned As Integer = Dep.LastRecordNumber
        Dim PARMOUT As String = String.Empty
        Dim PARMOUT2 As String = String.Empty
        Dim PARMOUT3 As String = String.Empty
        Dim PARMOUT4 As String = String.Empty
        Dim oneCallOnly As Boolean = False
        Dim lastPageReturned As Boolean = False
        Dim purchaseHistoryMode As Boolean = False
        Dim hasMoreRecords As String = String.Empty


        'We will load the data one page at a time for purchase history
        If String.IsNullOrWhiteSpace(Dep.PaymentReference) AndAlso Dep.CallId = 0 Then
            purchaseHistoryMode = True
            If Settings.OriginatingSourceCode <> "S" Then
                oneCallOnly = True
            End If
        End If

        ResultDataSet = New DataSet
        CreateWS005DataSet(ResultDataSet)
        If Dep.OrderStatus = "ALL" Then Dep.OrderStatus = ""
        Dep.PaymentReference = Dep.PaymentReference.Trim()
        Dim paymentReferenceGiven As Long = Utilities.PadLeadingZeros(Dep.PaymentReference, 15)

        If Not err.HasError Then
            While moreResults And nextStartingNumber < 1000

                Try
                    CallWS005R(PARMOUT, PARMOUT2, PARMOUT3, PARMOUT4, nextStartingNumber, hasMoreRecords)
                Catch ex As Exception
                    Const strError2 As String = "Error during database access"
                    moreResults = False
                    With err
                        .ErrorMessage = ex.Message
                        .ErrorStatus = strError2
                        .ErrorNumber = "TACDBOENQ02"
                        .HasError = True
                    End With
                End Try

                'Populate Status Table
                If PARMOUT.Substring(5119, 1).ToString() = GlobalConstants.ERRORFLAG Then
                    Dim errorStatusRow As DataRow = Nothing
                    errorStatusRow = ResultDataSet.Tables("ErrorStatus").NewRow()
                    errorStatusRow("ErrorOccurred") = PARMOUT.Substring(5119, 1)
                    errorStatusRow("ReturnCode") = PARMOUT.Substring(5117, 2)
                    ResultDataSet.Tables("ErrorStatus").Rows.Add(errorStatusRow)
                    moreResults = False
                End If

                If Not err.HasError Then
                    Try
                        hasMoreRecords = PARMOUT.Substring(1991, 1)
                        PopulateStatusTable(ResultDataSet.Tables("StatusResults"), PARMOUT)
                        PopulatePaymentOwnerTable(ResultDataSet.Tables("PaymentOwnerDetails"), PARMOUT)
                        PopulateOrderDetailTable(ResultDataSet.Tables("OrderEnquiryDetails"), PARMOUT2)
                        PopulateOrderTrackingNumbersTable(ResultDataSet.Tables("OrderTrackingNumbers"), PARMOUT4)
                        lastDetailReturned = CInt(PARMOUT.Substring(4829, 6))
                        If firstCall Then
                            totalToReturn = CInt(PARMOUT.Substring(4823, 6))
                            firstCall = False
                        End If
                        If PARMOUT.Substring(4807, 1) <> "N" Then lastPageReturned = True

                        If (lastDetailReturned >= totalToReturn AndAlso Not purchaseHistoryMode AndAlso hasMoreRecords <> "Y") OrElse oneCallOnly OrElse (purchaseHistoryMode AndAlso lastPageReturned) Then
                            PopulatePaymentDetailTable(ResultDataSet.Tables("PaymentDetails"), PARMOUT)
                            'Only populate the package information on the last call as we need all of the seats to be returned first
                            PopulatePackageHistoryTable(ResultDataSet.Tables("PackageHistory"), PARMOUT)
                            PopulatePackageDetailTable(ResultDataSet.Tables("PackageDetail"), PARMOUT3)
                            PopulateComponentSummaryTable(ResultDataSet.Tables("ComponentSummary"), ResultDataSet.Tables("OrderEnquiryDetails"), ResultDataSet.Tables("PackageDetail"))
                            moreResults = False
                        Else
                            nextStartingNumber = lastDetailReturned
                            Dep.LastRelativeRecordNumber = ResultDataSet.Tables("StatusResults").Rows(0)("LastRRN")
                        End If

                    Catch ex As Exception
                        Const strError2 As String = "Error processing results"
                        moreResults = False
                        With err
                            .ErrorMessage = ex.Message
                            .ErrorStatus = strError2
                            .ErrorNumber = "TACDBOENQ04"
                            .HasError = True
                        End With
                    End Try
                Else
                    moreResults = False
                End If
            End While
        End If

        Return err
    End Function

    Private Sub CallWS005R(ByRef PARMOUT As String, ByRef PARMOUT2 As String, ByRef PARMOUT3 As String, ByRef PARMOUT4 As String, ByVal nextStartingNumber As Integer, ByVal hasMoreRecords As String)
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS005R(@PARAM1,@PARAM2,@PARAM3,@PARAM4)"
        Dim parmInput1 As iDB2Parameter
        Dim parmInput2 As iDB2Parameter
        Dim parmInput3 As iDB2Parameter
        Dim parmInput4 As iDB2Parameter
        Dim det As New DETicketingItemDetails
        Dim parameterString As New StringBuilder
        If Dep.CollDEOrders.Count > 0 Then
            det = Dep.CollDEOrders.Item(1)
        Else
            det.ProductCode = ""
            det.Type = ""
        End If

        Dim returnFees As String = String.Empty
        If Settings.OriginatingSource.Trim.Length > 0 Then
            returnFees = "Y"
        End If

        'parameterString.Append(Utilities.FixStringLength(String.Empty, 4700))
        parameterString.Append(Utilities.FixStringLength(String.Empty, 1992))
        parameterString.Append(Utilities.FixStringLength(Dep.ProductCode, 6))
        parameterString.Append(Utilities.FixStringLength(String.Empty, 2702))
        parameterString.Append(Utilities.FixStringLength(Dep.ProductOrDescription, 40))
        If Dep.ShowDespatchInformation Then
            'parameterString.Append(Utilities.FixStringLength(String.Empty, 4779))
            parameterString.Append(Utilities.FixStringLength(String.Empty, 39))
            parameterString.Append(Utilities.FixStringLength(Dep.PackageKey, 22))
            parameterString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(Dep.BulkSalesFlag), 1))
            parameterString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(Dep.ShowDespatchInformation), 1))
        Else
            'parameterString.Append(Utilities.FixStringLength(String.Empty, 4803))
            parameterString.Append(Utilities.FixStringLength(String.Empty, 63))
        End If

        parameterString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(Dep.ShowRetailItems), 1))
        parameterString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(Dep.ShowTicketingItems), 1))
        parameterString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(Dep.IncludeBooked), 1))
        parameterString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(Dep.RequestFirstPage), 1))
        parameterString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(Dep.IsLastPage), 1))
        parameterString.Append(Utilities.PadLeadingZeros(Dep.FirstRecordOnPageRelativeRecordNumber, 15))
        parameterString.Append(Utilities.PadLeadingZeros(Dep.TotalRecords, 6))
        ' parameterString.Append(Utilities.PadLeadingZeros(Dep.LastRecordNumber, 6))
        parameterString.Append(Utilities.PadLeadingZeros(nextStartingNumber, 6))
        parameterString.Append(Utilities.PadLeadingZeros(Dep.CurrentPage, 6))
        parameterString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(Dep.RequestLastPage), 1))
        parameterString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(Dep.RequestPreviousPage), 1))
        parameterString.Append(Utilities.PadLeadingZeros(Dep.LastRelativeRecordNumber, 15))
        parameterString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(Dep.IncludeBuybackSales), 1))
        parameterString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(Dep.IncludeRoyaltyInformation), 1))
        parameterString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(Dep.IncludeReservations), 1))

        parameterString.Append(Utilities.PadLeadingZeros(Dep.BulkSalesID, 13))
        parameterString.Append(Utilities.FixStringLength(String.Empty, 88))
        parameterString.Append(Utilities.PadLeadingZeros(Dep.CallId, 13))
        parameterString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(Dep.CorporateProductsOnly), 1))
        parameterString.Append(Utilities.FixStringLength(Dep.OrderStatus, 6))
        parameterString.Append(Utilities.FixStringLength(Dep.FromDate, 8))
        parameterString.Append(Utilities.FixStringLength(Dep.ToDate, 8))
        parameterString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(Dep.CustNumberPayRefShouldMatch), 1))
        parameterString.Append(Utilities.FixStringLength(returnFees, 1))
        parameterString.Append(Utilities.PadLeadingZeros(Dep.PaymentReference, 15))
        parameterString.Append(Utilities.FixStringLength("000000000000000000000000000000", 30))
        parameterString.Append(Utilities.FixStringLength(Settings.AuthorityUserProfile, 10))
        parameterString.Append(Utilities.FixStringLength(Settings.OriginatingSource, 10))
        parameterString.Append(Utilities.FixStringLength(det.ProductCode.ToString, 6))
        parameterString.Append(Utilities.FixStringLength(det.Type.ToString, 1))
        parameterString.Append(Utilities.FixStringLength("00000000000000000000", 20))
        parameterString.Append(Utilities.FixStringLength(String.Empty, 1))
        parameterString.Append(Utilities.FixStringLength("00000", 5))
        parameterString.Append(Utilities.PadLeadingZeros(Settings.AccountNo1, 12))
        parameterString.Append(Utilities.FixStringLength("000", 3))
        parameterString.Append(nextStartingNumber.ToString.PadLeft(3, "0"))
        parameterString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        parameterString.Append(Utilities.FixStringLength(String.Empty, 2))
        parameterString.Append(Utilities.FixStringLength(String.Empty, 1))

        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
        parmInput1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
        parmInput1.Value = parameterString.ToString()
        parmInput1.Direction = ParameterDirection.InputOutput
        parmInput2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 20000)
        parmInput2.Value = Utilities.FixStringLength(String.Empty, 20000)
        parmInput2.Direction = ParameterDirection.InputOutput
        parmInput3 = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 20000)
        parmInput3.Value = Utilities.FixStringLength(String.Empty, 20000)
        parmInput3.Direction = ParameterDirection.InputOutput
        parmInput4 = cmdSELECT.Parameters.Add(Param4, iDB2DbType.iDB2Char, 10240)
        parmInput4.Value = Utilities.FixStringLength(String.Empty, 10240)
        parmInput4.Direction = ParameterDirection.InputOutput
        cmdSELECT.ExecuteNonQuery()

        PARMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        PARMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString
        PARMOUT3 = cmdSELECT.Parameters(Param3).Value.ToString
        PARMOUT4 = cmdSELECT.Parameters(Param4).Value.ToString
    End Sub

    Private Sub CreateWS005DataSet(ByRef resultDataSet As DataSet)

        dtDetails = New DataTable("OrderEnquiryDetails")
        resultDataSet.Tables.Add(dtDetails)
        CreateOrderDetailTable(dtDetails)

        Dim dtPackageDetail As New DataTable("PackageDetail")
        resultDataSet.Tables.Add(dtPackageDetail)
        CreatePackageDetailTable(dtPackageDetail)

        Dim dtPackageHistory As New DataTable("PackageHistory")
        resultDataSet.Tables.Add(dtPackageHistory)
        CreatePackageHistoryTable(dtPackageHistory)

        Dim dtComponentSummary As New DataTable("ComponentSummary")
        resultDataSet.Tables.Add(dtComponentSummary)
        CreateComponentSummaryTable(dtComponentSummary)

        Dim dtStatusResults As New DataTable("StatusResults")
        resultDataSet.Tables.Add(dtStatusResults)
        CreateStatusTable(dtStatusResults)

        Dim dtPaymentOwnerDetails As New DataTable("PaymentOwnerDetails")
        resultDataSet.Tables.Add(dtPaymentOwnerDetails)
        CreatePaymentOwnerTable(dtPaymentOwnerDetails)

        Dim dtPaymentDetails As New DataTable("PaymentDetails")
        resultDataSet.Tables.Add(dtPaymentDetails)
        CreatePaymentDetailTable(dtPaymentDetails)

        Dim dtOrderTrackingNumbers As New DataTable("OrderTrackingNumbers")
        resultDataSet.Tables.Add(dtOrderTrackingNumbers)
        CreateOrderTrackingNumbersTable(dtOrderTrackingNumbers)

        Dim dtErrorStatus As New DataTable("ErrorStatus")
        resultDataSet.Tables.Add(dtErrorStatus)
        CreateErrorStatusTable(dtErrorStatus)
    End Sub

    Private Sub CreateErrorStatusTable(ByRef dtErrorStatus As DataTable)
        With dtErrorStatus.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
    End Sub
    Private Sub CreateStatusTable(ByRef dtStatus As DataTable)
        With dtStatus.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("CashbackBankedTotal", GetType(String))
            .Add("CashbackUnbankedTotal", GetType(String))
            .Add("CashbackSpentTotal", GetType(String))
            .Add("BankedTotal", GetType(String))
            .Add("RewardTotal", GetType(String))
            .Add("LoyaltyActive", GetType(String))
            .Add("LoyaltyPoints", GetType(String))
            .Add("LastReturnedRecordNumber", GetType(Integer))
            .Add("TotalRecordNumber", GetType(Integer))
            .Add("PackageStatus", GetType(String))
            .Add("PackageType", GetType(String))
            .Add("PackageCancelFlag", GetType(String))
            .Add("PackageAmendFlag", GetType(String))
            .Add("PackageTransferFlag", GetType(String))
            .Add("ProductDescription", GetType(String))
            .Add("PackageDescription", GetType(String))
            .Add("PackageValueNet", GetType(Decimal))
            .Add("PackageValueVAT", GetType(Decimal))
            .Add("PackageValueGross", GetType(Decimal))
            .Add("PackageComponentDiscount", GetType(Decimal))
            .Add("PackagePackageDiscount", GetType(Decimal))
            .Add("PackagePricePaid", GetType(Decimal))
            .Add("ProductCode", GetType(String))
            .Add("ProductType", GetType(String))
            .Add("StadiumCode", GetType(String))
            .Add("ProductSubType", GetType(String))
            .Add("PaymentReference", GetType(String))
            .Add("FullCancelOnly", GetType(Boolean))
            .Add("CurrentPage", GetType(Integer))
            .Add("IsLastPage", GetType(Boolean))
            .Add("LastRecord", GetType(Integer))
            .Add("LastRRN", GetType(Integer))
            .Add("TotalRecords", GetType(Integer))
            .Add("RRNOfFirstRecordOnPage", GetType(Integer))
            .Add("ProductDate", GetType(String))
            .Add("BundleStartDate", GetType(String))
            .Add("BundleEndDate", GetType(String))
            .Add("DDAmountAlreadyPaid", GetType(Decimal))
        End With
    End Sub

    Private Sub CreatePaymentOwnerTable(ByRef dtPaymentOwner As DataTable)
        If dtPaymentOwner.Columns.Count = 0 Then
            With dtPaymentOwner.Columns
                .Add("PayOwnerNumber", GetType(String))
                .Add("Name", GetType(String))
                .Add("AddressLine1", GetType(String))
                .Add("AddressLine2", GetType(String))
                .Add("AddressLine3", GetType(String))
                .Add("AddressLine4", GetType(String))
                .Add("PostCodePart1", GetType(String))
                .Add("PostCodePart2", GetType(String))
                .Add("OriginatedSource", GetType(String))
                .Add("TransactionDate", GetType(String))
                .Add("BatchDetail", GetType(String))
                .Add("RetailTempOrderId", GetType(String))
                .Add("IsCorporateLinkedHomeGame", GetType(String))
            End With
        End If
    End Sub

    Private Sub CreatePaymentDetailTable(ByRef dtPaymentDetail As DataTable)
        If dtPaymentDetail.Columns.Count = 0 Then
            With dtPaymentDetail.Columns
                .Add("PayType", GetType(String))
                .Add("PayAmount", GetType(Decimal))
                'Credit card, Google Wallet, Paypal
                .Add("CardNumber", GetType(String))
                .Add("ExpiryDate", GetType(String))
                .Add("IssueDetail", GetType(String))
                .Add("StartDate", GetType(String))
                'Direct Debit
                .Add("AccountNumber", GetType(String))
                .Add("SortCode", GetType(String))
                .Add("AccountName", GetType(String))
                'Credit Finance
                .Add("FinancePlan", GetType(String))
                .Add("FinanceRequestID", GetType(String))
                .Add("FinancePlanDescription", GetType(String))
            End With
        End If
    End Sub

    Private Sub CreateOrderDetailTable(ByRef dtOrderDetail As DataTable)
        If dtDetails.Columns.Count = 0 Then
            With dtOrderDetail.Columns
                .Add("SaleDate", GetType(String))
                .Add("ProductDescription", GetType(String))
                .Add("Seat", GetType(String))
                .Add("SalePrice", GetType(Decimal))
                .Add("BatchReference", GetType(String))
                .Add("PaymentReference", GetType(String))
                .Add("StatusCode", GetType(String))
                .Add("LoyaltyPointsExpired", GetType(Boolean))
                .Add("LoyaltyPoints", GetType(String))
                .Add("PromotionID1", GetType(String))
                .Add("PromotionID2", GetType(String))
                .Add("PromotionID3", GetType(String))
                .Add("ProductCode", GetType(String))
                .Add("ProductDate", GetType(String))
                .Add("ProductType", GetType(String))
                .Add("OriginalPrice", GetType(Decimal))
                .Add("CatCancelWeb", GetType(Boolean))
                .Add("CatCancelAgent", GetType(Boolean))
                .Add("PriceBand", GetType(String))
                .Add("CustomerNumber", GetType(String))
                .Add("CustomerName", GetType(String))
                .Add("PriceBandDesc", GetType(String))
                .Add("StadiumCode", GetType(String))
                .Add("CatAmmendWeb", GetType(Boolean))
                .Add("CatAmmendAgent", GetType(Boolean))
                .Add("CatTransferWeb", GetType(Boolean))
                .Add("CatTransferAgent", GetType(Boolean))
                .Add("FeesCode", GetType(String))
                .Add("IsPrintable", GetType(Boolean))
                .Add("RRN", GetType(String))
                .Add("CallId", GetType(Long))
                .Add("IsProductBundle", GetType(Boolean))
                .Add("RelatingBundleProduct", GetType(String))
                .Add("RelatingBundleSeat", GetType(DESeatDetails))
                .Add("Unreserved", GetType(Boolean))
                .Add("Roving", GetType(Boolean))
                .Add("ComponentID", GetType(Long))
                .Add("StandDesc", GetType(String))
                .Add("AreaDesc", GetType(String))
                .Add("SeatUse", GetType(String))
                .Add("PaymentOwner", GetType(Boolean))
                .Add("BulkQty", GetType(Integer))
                .Add("BulkID", GetType(Integer))
                .Add("IsPackageItem", GetType(Boolean))
                .Add("TicketNumber", GetType(String))
                .Add("DespatchDate", GetType(String))
                .Add("AllocatedSeat", GetType(String))
                .Add("BundleStartDate", GetType(String))
                .Add("BundleEndDate", GetType(String))
                .Add("FulfilmentMethod", GetType(String))
                .Add("HideDate", GetType(Boolean))
                .Add("HideTime", GetType(Boolean))
                .Add("ReasonCancelUnavailable", GetType(String))
                .Add("ReasonAmendUnavailable", GetType(String))
                .Add("ReasonTransferUnavailable", GetType(String))
                .Add("IsDDRefundProduct", GetType(Boolean))
                .Add("PackageId", GetType(Long))
                .Add("IsCorporateLinkedHomeGame", GetType(Boolean))
            End With
        End If
    End Sub

    Private Sub CreatePackageDetailTable(ByRef dtPackageDetail As DataTable)
        With dtPackageDetail.Columns
            .Add("Quantity", GetType(Integer))
            .Add("ComponentDescription", GetType(String))
            .Add("TotalPrice", GetType(Decimal))
            .Add("Negative", GetType(Boolean))
            .Add("Details", GetType(String))
            .Add("ComponentID", GetType(Long))
            .Add("ComponentGroupID", GetType(Long))
            .Add("ComponentGroupType", GetType(String))
            .Add("ComponentType", GetType(String))
            .Add("ComponentShowSeats", GetType(Boolean))
            .Add("PackagePriceMode", GetType(String))
            .Add("ComponentUnitPrice", GetType(Decimal))
            .Add("ComponentDiscountPercentage", GetType(Decimal))
            .Add("HideSeatForPWS", GetType(Boolean))
        End With
    End Sub

    Private Sub CreatePackageHistoryTable(ByRef dtPackageHistory As DataTable)
        With dtPackageHistory.Columns
            .Add("PackageAmendmentId", GetType(Long))
            .Add("Comment", GetType(String))
            .Add("AmendmentDate", GetType(Date))
            .Add("PackageDiscAmended", GetType(Boolean))
            .Add("ComponentDiscAmended", GetType(Boolean))
        End With
    End Sub

    Private Sub CreateComponentSummaryTable(ByRef dtComponentSummary As DataTable)
        With dtComponentSummary.Columns
            .Add("ComponentID", GetType(Long))
            .Add("Stand", GetType(String))
            .Add("Seat", GetType(String))
            .Add("StandDesc", GetType(String))
            .Add("AreaDesc", GetType(String))
            .Add("PriceBandDesc", GetType(String))
            .Add("DisplayDesc", GetType(String))
            .Add("Quantity", GetType(Integer))
            .Add("AvailabilityComponent", GetType(Boolean))
            .Add("BulkID", GetType(Integer))
        End With
    End Sub

    Private Sub CreateOrderTrackingNumbersTable(ByRef dtOrderTrackingNumbers As DataTable)
        With dtOrderTrackingNumbers.Columns
            .Add("OrderTrackingNumber", GetType(String))
        End With
    End Sub
    Private Sub PopulateStatusTable(ByRef dtStatusResults As DataTable, ByVal parmOut As String)
        If dtStatusResults.Rows.Count = 0 Then
            Dim dsr As DataRow = Nothing
            dsr = dtStatusResults.NewRow()
            dsr("PackageType") = Utilities.CheckForDBNull_String(parmOut.Substring(4874, 1))
            dsr("PackageStatus") = Utilities.CheckForDBNull_String(parmOut.Substring(4875, 1))
            dsr("PackageCancelFlag") = Utilities.CheckForDBNull_String(parmOut.Substring(4876, 1))
            dsr("PackageAmendFlag") = Utilities.CheckForDBNull_String(parmOut.Substring(4877, 1))
            dsr("PackageTransferFlag") = Utilities.CheckForDBNull_String(parmOut.Substring(4878, 1))
            dsr("ProductDescription") = Utilities.CheckForDBNull_String(parmOut.Substring(4879, 40))
            dsr("PackageDescription") = Utilities.CheckForDBNull_String(parmOut.Substring(4919, 30))
            dsr("PackageValueNet") = CDec(Utilities.FormatPrice(parmOut.Substring(4646, 9)))
            dsr("PackageValueVAT") = CDec(Utilities.FormatPrice(parmOut.Substring(4655, 9)))
            dsr("PackageValueGross") = CDec(Utilities.FormatPrice(parmOut.Substring(4664, 9)))
            dsr("PackagePackageDiscount") = CDec(Utilities.FormatPrice(parmOut.Substring(4673, 9)))
            dsr("PackageComponentDiscount") = CDec(Utilities.FormatPrice(parmOut.Substring(4682, 9)))
            dsr("PackagePricePaid") = CDec(Utilities.FormatPrice(parmOut.Substring(4691, 9)))
            dsr("ProductCode") = parmOut.Substring(4949, 6).Trim
            dsr("ProductType") = parmOut.Substring(4955, 1).Trim
            dsr("StadiumCode") = parmOut.Substring(4956, 2).Trim
            dsr("ProductSubType") = parmOut.Substring(4958, 4).Trim
            dsr("PaymentReference") = Utilities.PadLeadingZeros(parmOut.Substring(5000, 15).Trim, 15)
            dsr("ErrorOccurred") = parmOut.Substring(5119, 1)
            dsr("ReturnCode") = parmOut.Substring(5117, 2)
            dsr("LoyaltyActive") = parmOut.Substring(5092, 1)
            dsr("ProductDate") = parmOut.Substring(4758, 7)
            dsr("BundleStartDate") = parmOut.Substring(4765, 7)
            dsr("BundleEndDate") = parmOut.Substring(4772, 7)
            dsr("DDAmountAlreadyPaid") = CDec(Utilities.FormatPrice(parmOut.Substring(4747, 11)))

            Try
                dsr("CashbackBankedTotal") = CDec(parmOut.Substring(5015, 8) & "." & parmOut.Substring(5023, 2)).ToString("#######0.00")
            Catch ex As Exception
                dsr("CashbackBankedTotal") = "0.00"
            End Try
            Try
                dsr("CashbackUnbankedTotal") = CDec(parmOut.Substring(5025, 8) & "." & parmOut.Substring(5033, 2)).ToString("#######0.00")
            Catch ex As Exception
                dsr("CashbackUnbankedTotal") = "0.00"
            End Try
            Try
                dsr("CashbackSpentTotal") = CDec(parmOut.Substring(5035, 8) & "." & parmOut.Substring(5043, 2)).ToString("#######0.00")
            Catch ex As Exception
                dsr("CashbackSpentTotal") = "0.00"
            End Try

            Try
                dsr("RewardTotal") = CDec(parmOut.Substring(5072, 8) & "." & parmOut.Substring(5080, 2)).ToString("#######0.00")
            Catch ex As Exception
                dsr("RewardTotal") = "0.00"
            End Try

            Try
                dsr("BankedTotal") = CDec(parmOut.Substring(5082, 8) & "." & parmOut.Substring(5090, 2)).ToString("#######0.00")
            Catch ex As Exception
                dsr("BankedTotal") = "0.00"
            End Try

            Try
                dsr("LoyaltyPoints") = CDec(parmOut.Substring(5093, 5)).ToString
            Catch ex As Exception
                dsr("LoyaltyPoints") = "0"
            End Try

            dsr("LastReturnedRecordNumber") = CInt(parmOut.Substring(4829, 6))
            dsr("TotalRecordNumber") = CInt(parmOut.Substring(4823, 6))
            dsr("FullCancelOnly") = ConvertFromISeriesYesNoToBoolean(parmOut.Substring(1189, 1))
            dsr("RRNOfFirstRecordOnPage") = CInt(parmOut.Substring(4808, 15))
            dsr("CurrentPage") = CInt(parmOut.Substring(4835, 6))
            dsr("IsLastPage") = ConvertFromISeriesYesNoToBoolean(parmOut.Substring(4807, 1))
            dsr("LastRecord") = CInt(parmOut.Substring(4829, 6))
            dsr("LastRRN") = CInt(parmOut.Substring(4843, 15))
            dsr("TotalRecords") = CInt(parmOut.Substring(4823, 6))

            dtStatusResults.Rows.Add(dsr)
        Else
            dtStatusResults.Rows(0)("LastReturnedRecordNumber") = CInt(parmOut.Substring(4829, 6))
            dtStatusResults.Rows(0)("LastRRN") = CInt(parmOut.Substring(4843, 15))
        End If
    End Sub

    Private Sub PopulateOrderDetailTable(ByRef dtOrderDetail As DataTable, ByVal parmOut2 As String)
        Dim dr As DataRow = Nothing
        Dim iParmoutPosition As Integer = 0
        Dim iCounter As Integer = 0
        iCounter = 0
        Dim iPosition As Integer = 0
        Dim sWork As String = parmOut2
        Dim strQty As String = String.Empty
        For iCounter = 0 To 39
            iPosition = iCounter * 500
            If sWork.Substring(iPosition, 40).Trim > String.Empty Then
                dr = Nothing
                dr = dtOrderDetail.NewRow()

                If sWork.Substring(iPosition, 2) <> "00" Then
                    dr("SaleDate") = sWork.Substring(iPosition, 10)
                    dr("ProductDescription") = sWork.Substring(iPosition + 10, 40).Trim
                    If sWork.Substring(iPosition + 134, 1).Trim = GlobalConstants.TRAVELPRODUCTTYPE Then
                        dr("Seat") = sWork.Substring(iPosition + 50, 18).Trim.Replace("//", "/ /").TrimEnd("/")
                    Else
                        dr("Seat") = sWork.Substring(iPosition + 50, 18).Trim.Replace(" ", "").Replace("//", "/ /").TrimEnd("/")
                    End If
                    dr("SalePrice") = CDec(sWork.Substring(iPosition + 68, 12))
                    dr("BatchReference") = sWork.Substring(iPosition + 80, 9).Trim
                    dr("PaymentReference") = sWork.Substring(iPosition + 89, 15).Trim
                    ' dont set status for attendance 
                    If dr("Seat").ToString.Trim = "*AT/TEND" Then
                        dr("StatusCode") = String.Empty
                    Else
                        dr("StatusCode") = sWork.Substring(iPosition + 104, 6).Trim
                    End If

                    dr("LoyaltyPoints") = sWork.Substring(iPosition + 110, 5).Trim
                    dr("LoyaltyPointsExpired") = Utilities.convertToBool(sWork.Substring(iPosition + 145, 1).Trim)
                    dr("PromotionID1") = sWork.Substring(iPosition + 115, 13).Trim
                    dr("PromotionID2") = sWork.Substring(iPosition + 375, 13).Trim
                    dr("PromotionID3") = sWork.Substring(iPosition + 388, 13).Trim
                    dr("ProductCode") = sWork.Substring(iPosition + 128, 6).Trim
                    dr("ProductType") = sWork.Substring(iPosition + 134, 1).Trim
                    Try
                        If CDate(sWork.Substring(iPosition + 135, 10)).ToString("dd/MM/yyyy") = "00/00/00" Then
                            dr("ProductDate") = "01/01/0001"
                        Else
                            dr("ProductDate") = CDate(sWork.Substring(iPosition + 135, 10)).ToString("dd/MM/yyyy")
                        End If
                    Catch ex As Exception
                        dr("ProductDate") = "01/01/0001"
                    End Try
                    Try
                        dr("BundleStartDate") = Utilities.ISeriesDate(sWork.Substring(iPosition + 444, 7)).ToLongDateString()
                    Catch ex As Exception
                        dr("BundleStartDate") = "01/01/0001"
                    End Try
                    Try
                        dr("BundleEndDate") = Utilities.ISeriesDate(sWork.Substring(iPosition + 451, 7)).ToLongDateString()
                    Catch ex As Exception
                        dr("BundleEndDate") = "01/01/0001"
                    End Try
                    dr("CATCancelWeb") = False
                    dr("CATCancelAgent") = False
                    Select Case sWork.Substring(iPosition + 146, 1).Trim
                        Case Is = "A"
                            dr("CATCancelAgent") = True
                        Case Is = "Y"
                            dr("CATCancelWeb") = True
                            dr("CATCancelAgent") = True
                    End Select
                    dr("PriceBand") = sWork.Substring(iPosition + 147, 1).Trim
                    dr("CustomerNumber") = sWork.Substring(iPosition + 148, 12).Trim
                    dr("CustomerName") = sWork.Substring(iPosition + 160, 51).Trim
                    dr("PriceBandDesc") = sWork.Substring(iPosition + 211, 15).Trim
                    dr("StadiumCode") = sWork.Substring(iPosition + 226, 2).Trim
                    'If the original price is blank (eg. no promotion has been applied), set the original price to match the sale price
                    If String.IsNullOrEmpty(sWork.Substring(iPosition + 228, 12).Trim) Then
                        dr("OriginalPrice") = CDec(sWork.Substring(iPosition + 68, 12))
                    Else
                        dr("OriginalPrice") = Utilities.CheckForDBNull_Decimal(sWork.Substring(iPosition + 228, 12).Trim)
                    End If
                    dr("CATAmmendWeb") = False
                    dr("CATAmmendAgent") = False
                    Select Case sWork.Substring(iPosition + 240, 1).Trim
                        Case Is = "A"
                            dr("CATAmmendAgent") = True
                        Case Is = "Y"
                            dr("CATAmmendWeb") = True
                            dr("CATAmmendAgent") = True
                    End Select
                    dr("CATTransferWeb") = False
                    dr("CATTransferAgent") = False
                    Select Case sWork.Substring(iPosition + 241, 1).Trim
                        Case Is = "A"
                            dr("CATTransferAgent") = True
                        Case Is = "Y"
                            dr("CATTransferWeb") = True
                            dr("CATTransferAgent") = True
                    End Select
                    dr("FeesCode") = sWork.Substring(iPosition + 242, 6).Trim
                    If sWork.Substring(iPosition + 248, 1).Trim = "Y" Then
                        dr("IsPrintable") = True
                    Else
                        dr("IsPrintable") = False
                    End If
                    dr("RRN") = sWork.Substring(iPosition + 249, 15).Trim
                    If sWork.Substring(iPosition + 264, 13).Trim <> String.Empty Then
                        dr("CallId") = sWork.Substring(iPosition + 264, 13).Trim
                    Else
                        dr("CallId") = 0
                    End If
                    dr("RelatingBundleProduct") = sWork.Substring(iPosition + 279, 6).Trim
                    Dim seat As New DESeatDetails
                    seat.Stand = sWork.Substring(iPosition + 285, 3).Trim()
                    seat.Area = sWork.Substring(iPosition + 288, 4).Trim()
                    seat.Row = sWork.Substring(iPosition + 292, 4).Trim()
                    seat.Seat = sWork.Substring(iPosition + 296, 4).Trim()
                    seat.AlphaSuffix = sWork.Substring(iPosition + 300, 1).Trim()
                    dr("RelatingBundleSeat") = seat
                    dr("IsProductBundle") = (sWork.Substring(iPosition + 301, 1).Trim = "B")
                    If sWork.Substring(iPosition + 277, 1).Trim() = "Y" Then
                        dr("Unreserved") = True
                    Else
                        dr("Unreserved") = False
                    End If
                    If sWork.Substring(iPosition + 278, 1).Trim() = "Y" Then
                        dr("Roving") = True
                    Else
                        dr("Roving") = False
                    End If
                    dr("ComponentID") = sWork.Substring(iPosition + 302, 13).Trim.ConvertAndTrimStringToLong
                    dr("StandDesc") = sWork.Substring(iPosition + 315, 30).Trim
                    dr("AreaDesc") = sWork.Substring(iPosition + 345, 20).Trim
                    dr("SeatUse") = sWork.Substring(iPosition + 365, 1).Trim
                    dr("PaymentOwner") = sWork.Substring(iPosition + 366, 1).ConvertFromISeriesYesNoToBoolean
                    If sWork.Substring(iPosition + 401, 1).Trim = "Y" Then
                        dr("IsPackageItem") = True
                    Else
                        dr("IsPackageItem") = False
                    End If
                    dr("TicketNumber") = sWork.Substring(iPosition + 402, 14).Trim
                    dr("DespatchDate") = sWork.Substring(iPosition + 417, 7).Trim
                    dr("FulfilmentMethod") = sWork.Substring(iPosition + 458, 1).Trim
                    dr("AllocatedSeat") = sWork.Substring(iPosition + 424, 20).Trim.Replace(" ", "").Replace("//", "/ /").TrimEnd("/")
                    dr("BulkQty") = Utilities.CheckForDBNull_Int(sWork.Substring(iPosition + 459, 5).Trim)
                    dr("BulkID") = Utilities.CheckForDBNull_Int(sWork.Substring(iPosition + 464, 13).Trim)
                    dr("HideDate") = Utilities.convertToBool(sWork.Substring(iPosition + 477, 1))
                    dr("HideTime") = Utilities.convertToBool(sWork.Substring(iPosition + 478, 1))
                    dr("ReasonCancelUnavailable") = sWork.Substring(iPosition + 479, 2)
                    dr("ReasonAmendUnavailable") = sWork.Substring(iPosition + 481, 2)
                    dr("ReasonTransferUnavailable") = sWork.Substring(iPosition + 483, 2)
                    dr("IsDDRefundProduct") = Utilities.convertToBool(sWork.Substring(iPosition + 485, 1))
                    dr("PackageId") = Utilities.CheckForDBNull_BigInt(sWork.Substring(iPosition + 486, 13))
                    dr("IsCorporateLinkedHomeGame") = Utilities.convertToBool(sWork.Substring(iPosition + 499, 1))
                    dtOrderDetail.Rows.Add(dr)
                End If
            Else
                Exit For
            End If
        Next
    End Sub

    Private Sub PopulatePaymentOwnerTable(ByRef dtPaymentOwner As DataTable, ByVal parmOut As String)
        If dtPaymentOwner.Rows.Count = 0 AndAlso parmOut.Substring(5119, 1).Trim.Length = 0 AndAlso parmOut.Substring(5117, 2).Trim.Length = 0 Then
            Dim dr As DataRow = Nothing
            Dim iPosition As Integer = 0
            dr = Nothing
            dr = dtPaymentOwner.NewRow
            dr("Name") = parmOut.Substring(iPosition, 30).Trim + parmOut.Substring(iPosition + 1290, 30).Trim
            dr("AddressLine1") = parmOut.Substring(iPosition + 30, 30).Trim
            dr("AddressLine2") = parmOut.Substring(iPosition + 60, 30).Trim
            dr("AddressLine3") = parmOut.Substring(iPosition + 90, 25).Trim
            dr("AddressLine4") = parmOut.Substring(iPosition + 115, 25).Trim
            dr("PostCodePart1") = parmOut.Substring(iPosition + 140, 4).Trim
            dr("PostCodePart2") = parmOut.Substring(iPosition + 144, 4).Trim
            dr("OriginatedSource") = parmOut.Substring(iPosition + 148, 10).Trim
            dr("TransactionDate") = parmOut.Substring(iPosition + 158, 10).Trim
            dr("BatchDetail") = parmOut.Substring(iPosition + 168, 9).Trim
            dr("PayOwnerNumber") = parmOut.Substring(iPosition + 1177, 12).Trim
            dr("RetailTempOrderId") = parmOut.Substring(iPosition + 1190, 100).Trim
            dr("IsCorporateLinkedHomeGame") = parmOut.Substring(iPosition + 1998, 1)
            dtPaymentOwner.Rows.Add(dr)
        End If
    End Sub

    Private Sub PopulatePaymentDetailTable(ByRef dtPaymentDetail As DataTable, ByVal parmOut As String)
        'PaymentDetails
        'each array length 100 
        'number of array 10
        Dim iPosition As Integer = 0
        Dim iCounter As Integer = 0
        Dim dr As DataRow = Nothing
        Dim tempPayDetails As String = String.Empty
        Dim payDetailPosition As Integer = 0
        Dim tempPayType As String = String.Empty
        If parmOut.Substring(177, 1000).Trim.Length > 0 Then
            For iCounter = 0 To 9
                iPosition = 177 + (iCounter * 100)
                If parmOut.Substring(iPosition, 100).Trim.Length > 0 Then
                    payDetailPosition = 0
                    tempPayType = String.Empty
                    dr = Nothing
                    tempPayDetails = parmOut.Substring(iPosition, 100)
                    dr = dtPaymentDetail.NewRow
                    tempPayType = tempPayDetails.Substring(payDetailPosition, 2).Trim.ToUpper
                    dr("PayType") = tempPayType
                    dr("PayAmount") = Utilities.FormatPrice(tempPayDetails.Substring(payDetailPosition + 2, 15).Trim)
                    If parmOut.Substring(iPosition + 77, 1) = "Y" Then
                        dr("PayAmount") = dr("PayAmount") * -1
                    End If
                    If (tempPayType = GlobalConstants.CCPAYMENTTYPE) Then
                        dr("CardNumber") = tempPayDetails.Substring(payDetailPosition + 17, 4).Trim
                        dr("ExpiryDate") = tempPayDetails.Substring(payDetailPosition + 36, 4).Trim
                        dr("StartDate") = tempPayDetails.Substring(payDetailPosition + 40, 4).Trim
                        dr("IssueDetail") = tempPayDetails.Substring(payDetailPosition + 44, 2).Trim
                    ElseIf (tempPayType = GlobalConstants.PAYPALPAYMENTTYPE) Then
                        dr("CardNumber") = tempPayDetails.Substring(payDetailPosition + 17, 19).Trim
                    ElseIf (tempPayType = GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE) Then
                        dr("CardNumber") = tempPayDetails.Substring(payDetailPosition + 17, 19).Trim
                    ElseIf (tempPayType = GlobalConstants.CSPAYMENTTYPE) Then

                    ElseIf (tempPayType = GlobalConstants.DDPAYMENTTYPE) Then
                        dr("AccountNumber") = tempPayDetails.Substring(payDetailPosition + 17, 8).Trim
                        dr("SortCode") = tempPayDetails.Substring(payDetailPosition + 25, 6).Trim
                        dr("AccountName") = tempPayDetails.Substring(payDetailPosition + 31, 40).Trim
                    ElseIf (tempPayType = GlobalConstants.CFPAYMENTTYPE) Then
                        dr("FinancePlan") = tempPayDetails.Substring(payDetailPosition + 17, 3).Trim
                        dr("FinanceRequestID") = tempPayDetails.Substring(payDetailPosition + 20, 10).Trim
                        dr("FinancePlanDescription") = tempPayDetails.Substring(payDetailPosition + 30, 47).Trim
                    ElseIf (tempPayType = GlobalConstants.EPURSEPAYMENTTYPE) Then
                        dr("CardNumber") = tempPayDetails.Substring(payDetailPosition + 18, 19).Trim
                    End If
                    dtPaymentDetail.Rows.Add(dr)
                Else
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub PopulatePackageDetailTable(ByRef dtPackageDetail As DataTable, ByVal parmOut3 As String)
        If Not String.IsNullOrWhiteSpace(parmOut3) Then
            Dim iPosition As Integer = 0
            Dim iCounter As Integer = 0
            Dim dr As DataRow = Nothing
            Dim strRecord As String
            For iCounter = 0 To 199
                iPosition = iCounter * 100
                strRecord = parmOut3.Substring(iPosition, 5).Trim
                'If Not String.IsNullOrEmpty(strRecord) AndAlso Integer.Parse(strRecord) > 0 Then
                If Not String.IsNullOrEmpty(strRecord) Then
                    dr = Nothing
                    dr = dtPackageDetail.NewRow()
                    If Utilities.CheckForDBNull_String(parmOut3.Substring(iPosition + 74, 1)).ConvertFromISeriesYesNoToBoolean() Then
                        dr("Quantity") = CType(parmOut3.Substring(iPosition, 5), Integer) * (-1)
                    Else
                        dr("Quantity") = parmOut3.Substring(iPosition, 5)
                    End If
                    dr("ComponentDescription") = parmOut3.Substring(iPosition + 5, 30).Trim
                    If Utilities.CheckForDBNull_String(parmOut3.Substring(iPosition + 44, 1)).ConvertFromISeriesYesNoToBoolean() Then
                        dr("TotalPrice") = CDec(Utilities.FormatPrice(parmOut3.Substring(iPosition + 35, 9))) * (-1)
                    Else
                        dr("TotalPrice") = Utilities.FormatPrice(parmOut3.Substring(iPosition + 35, 9))
                    End If
                    dr("Negative") = Utilities.CheckForDBNull_String(parmOut3.Substring(iPosition + 44, 1)).ConvertFromISeriesYesNoToBoolean()
                    dr("ComponentID") = parmOut3.Substring(iPosition + 45, 13).Trim
                    dr("ComponentGroupID") = parmOut3.Substring(iPosition + 58, 13).Trim
                    dr("ComponentGroupType") = parmOut3.Substring(iPosition + 71, 2).Trim
                    dr("ComponentType") = parmOut3.Substring(iPosition + 73, 1).Trim
                    dr("ComponentShowSeats") = Utilities.CheckForDBNull_String(parmOut3.Substring(iPosition + 75, 1)).ConvertFromISeriesYesNoToBoolean()
                    dr("PackagePriceMode") = parmOut3.Substring(iPosition + 76, 1).Trim
                    dr("ComponentUnitPrice") = CDec(Utilities.FormatPrice(parmOut3.Substring(iPosition + 77, 9)))
                    dr("ComponentDiscountPercentage") = CDec(Utilities.FormatPrice(parmOut3.Substring(iPosition + 86, 5)))
                    dr("HideSeatForPWS") = Utilities.convertToBool(parmOut3.Substring(iPosition + 91, 1))
                    dtPackageDetail.Rows.Add(dr)
                Else
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub PopulatePackageHistoryTable(ByRef dtPackageHistory As DataTable, ByVal parmOut As String)
        If (Dep.CallId > 0) Then
            Dim iPosition As Integer = 0
            Dim iCounter As Integer = 0
            Dim dr As DataRow = Nothing
            Dim strRecord As String
            parmOut = parmOut.Substring(iPosition + 1999, 1000)
            For iCounter = 0 To 9
                iPosition = iCounter * 100
                strRecord = parmOut.Substring(iPosition, 15).Trim
                If Not String.IsNullOrEmpty(strRecord) AndAlso Long.Parse(strRecord) > 0 Then
                    dr = Nothing
                    dr = dtPackageHistory.NewRow()
                    dr("PackageAmendmentId") = Utilities.CheckForDBNull_BigInt(parmOut.Substring(iPosition, 15))
                    dr("Comment") = parmOut.Substring(iPosition + 15, 50).Trim
                    dr("AmendmentDate") = Utilities.ISeriesDate(parmOut.Substring(iPosition + 65, 7))
                    dr("PackageDiscAmended") = Utilities.convertToBool(parmOut.Substring(iPosition + 72, 1).Trim)
                    dr("ComponentDiscAmended") = Utilities.convertToBool(parmOut.Substring(iPosition + 73, 1).Trim)
                    dtPackageHistory.Rows.Add(dr)
                Else
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub PopulateComponentSummaryTable(ByRef dtComponentSummary As DataTable, ByVal dtOrderDetail As DataTable, ByVal dtPackageDetail As DataTable)
        If dtOrderDetail IsNot Nothing AndAlso dtOrderDetail.Rows.Count > 0 Then
            If dtPackageDetail IsNot Nothing AndAlso dtPackageDetail.Rows.Count > 0 Then
                Dim componentID As Long = 0
                Dim standCode As String = String.Empty
                Dim standDesc As String = String.Empty
                Dim areaDesc As String = String.Empty
                Dim priceBandDesc As String = String.Empty
                Dim quantity As Integer = 0
                Dim rowNotAddedQty As Integer = 0
                Dim rowNotAdded As Boolean = True
                Dim dr As DataRow = Nothing
                Dim tempPreviousSeatUse As String = String.Empty
                Dim tempCurrentSeatUse As String = String.Empty
                Dim tempPreviousStandCode As String = String.Empty
                Dim tempCurrentStandCode As String = String.Empty
                Dim tempPreviousPriceBand As String = String.Empty
                Dim tempCurrentPriceBand As String = String.Empty
                For packRowIndex As Integer = 0 To dtPackageDetail.Rows.Count - 1
                    tempPreviousSeatUse = String.Empty
                    tempPreviousStandCode = String.Empty
                    tempPreviousPriceBand = String.Empty
                    standCode = String.Empty
                    standDesc = String.Empty
                    areaDesc = String.Empty
                    priceBandDesc = String.Empty
                    rowNotAdded = False
                    quantity = 0
                    rowNotAddedQty = 0
                    dtOrderDetail.DefaultView.RowFilter = "FeesCode = '' AND ComponentID = " & dtPackageDetail.Rows(packRowIndex)("ComponentID")
                    dtOrderDetail.DefaultView.Sort = "SeatUse ASC, Seat ASC, PriceBand ASC"
                    For orderRowIndex As Integer = 0 To dtOrderDetail.DefaultView.Count - 1
                        If Utilities.CheckForDBNull_String(dtPackageDetail.Rows(packRowIndex)("ComponentType")).Trim = "A" Then
                            dr = Nothing
                            dr = dtComponentSummary.NewRow
                            dr("ComponentID") = dtPackageDetail.Rows(packRowIndex)("ComponentID")
                            dr("Seat") = Utilities.CheckForDBNull_String(dtOrderDetail.DefaultView(orderRowIndex)("Seat"))
                            dr("StandDesc") = Utilities.CheckForDBNull_String(dtOrderDetail.DefaultView(orderRowIndex)("StandDesc"))
                            dr("AreaDesc") = Utilities.CheckForDBNull_String(dtOrderDetail.DefaultView(orderRowIndex)("AreaDesc"))
                            dr("PriceBandDesc") = Utilities.CheckForDBNull_String(dtOrderDetail.DefaultView(orderRowIndex)("PriceBandDesc"))
                            dr("DisplayDesc") = Utilities.CheckForDBNull_String(dtOrderDetail.DefaultView(orderRowIndex)("AreaDesc"))
                            dr("Quantity") = quantity
                            dr("AvailabilityComponent") = True
                            dr("BulkID") = Utilities.CheckForDBNull_Int(dtOrderDetail.DefaultView(orderRowIndex)("BulkID"))

                            dtComponentSummary.Rows.Add(dr)
                            rowNotAdded = False
                        Else
                            tempCurrentSeatUse = Utilities.CheckForDBNull_String(dtOrderDetail.DefaultView(orderRowIndex)("SeatUse"))
                            tempCurrentStandCode = Utilities.CheckForDBNull_String(dtOrderDetail.DefaultView(orderRowIndex)("Seat"))
                            tempCurrentPriceBand = Utilities.CheckForDBNull_String(dtOrderDetail.DefaultView(orderRowIndex)("PriceBand"))
                            If tempCurrentStandCode.Trim.Length > 2 Then
                                tempCurrentStandCode = tempCurrentStandCode.Substring(0, 3)
                            Else
                                tempCurrentStandCode = String.Empty
                            End If
                            If tempCurrentSeatUse.Trim.ToUpper <> "T" Then
                                If (tempCurrentSeatUse <> tempPreviousSeatUse) OrElse (tempCurrentStandCode <> tempPreviousStandCode) OrElse (tempCurrentPriceBand <> tempPreviousPriceBand) Then
                                    If Not String.IsNullOrWhiteSpace(tempPreviousSeatUse) Then
                                        'move the value to dtComponentSummary
                                        dr = Nothing
                                        dr = dtComponentSummary.NewRow
                                        dr("ComponentID") = dtPackageDetail.Rows(packRowIndex)("ComponentID")
                                        dr("Seat") = ""
                                        dr("Stand") = tempPreviousStandCode
                                        dr("StandDesc") = standDesc
                                        dr("AreaDesc") = areaDesc
                                        dr("PriceBandDesc") = priceBandDesc

                                        'For non bulk mode,  quantity for T&A components is incremented 1 at a time as each seat is returned via the OrderDetails Call.
                                        'For nbulk, we retrieve 1 record per bulk ID, which will equate to 1 per stand/area breakdown. sS we dont need to increment, just use the quantity returned.
                                        dr("Quantity") = quantity
                                        If rowNotAddedQty > 0 Then
                                            dr("Quantity") = rowNotAddedQty
                                        End If
                                        If tempPreviousSeatUse = "P" Then
                                            dr("DisplayDesc") = priceBandDesc
                                        Else
                                            dr("DisplayDesc") = areaDesc
                                        End If
                                        dr("AvailabilityComponent") = False
                                        dr("BulkID") = 0
                                        dtComponentSummary.Rows.Add(dr)
                                        rowNotAdded = False
                                    End If
                                    tempPreviousSeatUse = tempCurrentSeatUse
                                    tempPreviousStandCode = tempCurrentStandCode
                                    tempPreviousPriceBand = tempCurrentPriceBand
                                    standCode = tempPreviousStandCode
                                    standDesc = Utilities.CheckForDBNull_String(dtOrderDetail.DefaultView(orderRowIndex)("StandDesc"))
                                    areaDesc = Utilities.CheckForDBNull_String(dtOrderDetail.DefaultView(orderRowIndex)("AreaDesc"))
                                    priceBandDesc = Utilities.CheckForDBNull_String(dtOrderDetail.DefaultView(orderRowIndex)("PriceBandDesc"))
                                    quantity = 1
                                    If Utilities.CheckForDBNull_Int(dtOrderDetail.DefaultView(orderRowIndex)("BulkID")) > 0 Then
                                        rowNotAddedQty = Utilities.CheckForDBNull_Int(dtOrderDetail.DefaultView(orderRowIndex)("BulkQty"))
                                    End If
                                    rowNotAdded = True
                                Else
                                    quantity += 1
                                End If
                            End If
                        End If

                    Next
                    If rowNotAdded Then
                        dr = Nothing
                        dr = dtComponentSummary.NewRow
                        dr("ComponentID") = dtPackageDetail.Rows(packRowIndex)("ComponentID")
                        dr("Seat") = ""
                        dr("Stand") = standCode
                        dr("StandDesc") = standDesc
                        dr("AreaDesc") = areaDesc
                        dr("PriceBandDesc") = priceBandDesc
                        dr("Quantity") = quantity
                        If rowNotAddedQty > 0 Then
                            dr("Quantity") = rowNotAddedQty
                        End If
                        If tempPreviousSeatUse = "P" Then
                            dr("DisplayDesc") = priceBandDesc
                        Else
                            dr("DisplayDesc") = areaDesc
                        End If
                        dr("AvailabilityComponent") = False
                        dr("BulkID") = 0
                        dtComponentSummary.Rows.Add(dr)
                        rowNotAdded = False
                    End If
                    dtOrderDetail.DefaultView.RowFilter = ""
                Next
                dtOrderDetail.DefaultView.RowFilter = ""
            End If
        End If


    End Sub

    Private Sub PopulateOrderTrackingNumbersTable(ByRef dtOrderTrackingNumbers As DataTable, ByVal parmOut4 As String)
        Dim dr As DataRow = Nothing
        Dim iPosition As Integer = 0
        Do While parmOut4.Substring(iPosition + 1, 1).Trim().Length > 0
            dr = dtOrderTrackingNumbers.NewRow
            dr("OrderTrackingNumber") = parmOut4.Substring(iPosition + 15, 100).Trim()
            dtOrderTrackingNumbers.Rows.Add(dr)
            iPosition = iPosition + 115
        Loop
    End Sub
#Region "OLD"
    'Private Function ReadOrderEnquiryDetailsTALENTTKT() As ErrorObj
    '    Dim err As New ErrorObj
    '    Dim intPosition As Integer = 0
    '    Dim item1 As String = String.Empty
    '    Dim item2 As String = String.Empty
    '    Dim item3 As String = String.Empty
    '    Dim item4 As String = String.Empty
    '    Dim item5 As String = String.Empty
    '    Dim moreResults As Boolean = True
    '    Dim firstCall As Boolean = True

    '    Dim nextStartingNumber As Integer = Dep.LastRecordNumber
    '    Dim totalToReturn As Integer = Dep.TotalRecords
    '    Dim lastDetailReturned As Integer = Dep.LastRecordNumber
    '    '---------------------------------------------------------------------------
    '    dtDetails = New DataTable("OrderEnquiryDetails")
    '    ResultDataSet = New DataSet
    '    ResultDataSet.Tables.Add(dtDetails)
    '    If Dep.OrderStatus = "ALL" Then Dep.OrderStatus = ""
    '    If dtDetails.Columns.Count = 0 Then
    '        AddColumnsToDataTables()
    '    End If

    '    Dim dtPackageDetail As New DataTable("PackageDetail")
    '    ResultDataSet.Tables.Add(dtPackageDetail)
    '    With dtPackageDetail.Columns
    '        .Add("Quantity", GetType(Integer))
    '        .Add("ComponentDescription", GetType(String))
    '        .Add("TotalPrice", GetType(Decimal))
    '        .Add("Negative", GetType(Boolean))
    '        .Add("Details", GetType(String))
    '        .Add("ComponentID", GetType(Long))
    '        .Add("ComponentGroupID", GetType(Long))
    '        .Add("ComponentGroupType", GetType(String))
    '        .Add("ComponentType", GetType(String))
    '    End With

    '    Dim dtPackageHistory As New DataTable("PackageHistory")
    '    ResultDataSet.Tables.Add(dtPackageHistory)
    '    With dtPackageHistory.Columns
    '        .Add("PackageAmendmentId", GetType(Long))
    '        .Add("Comment", GetType(String))
    '        .Add("AmendmentDate", GetType(Date))
    '    End With

    '    'Create the Status data table
    '    Dim dtStatusResults As New DataTable("StatusResults")
    '    ResultDataSet.Tables.Add(dtStatusResults)
    '    With dtStatusResults.Columns
    '        .Add("ErrorOccurred", GetType(String))
    '        .Add("ReturnCode", GetType(String))
    '        .Add("CashbackBankedTotal", GetType(String))
    '        .Add("CashbackUnbankedTotal", GetType(String))
    '        .Add("CashbackSpentTotal", GetType(String))
    '        .Add("BankedTotal", GetType(String))
    '        .Add("RewardTotal", GetType(String))
    '        .Add("LoyaltyActive", GetType(String))
    '        .Add("LoyaltyPoints", GetType(String))
    '        .Add("LastReturnedRecordNumber", GetType(Integer))
    '        .Add("TotalRecordNumber", GetType(Integer))
    '        .Add("PackageCancelFlag", GetType(Boolean))
    '        .Add("PackageAmendFlag", GetType(Boolean))
    '        .Add("PackageTransferFlag", GetType(Boolean))
    '        .Add("ProductDescription", GetType(String))
    '        .Add("PackageDescription", GetType(String))
    '        .Add("ProductCode", GetType(String))
    '        .Add("ProductType", GetType(String))
    '        .Add("StadiumCode", GetType(String))
    '        .Add("ProductSubType", GetType(String))
    '    End With

    '    'Create the payment details table
    '    Dim dtPaymentOwnerDetails As New DataTable("PaymentOwnerDetails")
    '    ResultDataSet.Tables.Add(dtPaymentOwnerDetails)
    '    If dtPaymentOwnerDetails.Columns.Count = 0 Then
    '        With dtPaymentOwnerDetails.Columns
    '            .Add("Name", GetType(String))
    '            .Add("AddressLine1", GetType(String))
    '            .Add("AddressLine2", GetType(String))
    '            .Add("AddressLine3", GetType(String))
    '            .Add("AddressLine4", GetType(String))
    '            .Add("PostCodePart1", GetType(String))
    '            .Add("PostCodePart2", GetType(String))
    '            .Add("OriginatedSource", GetType(String))
    '            .Add("TransactionDate", GetType(String))
    '            .Add("BatchDetail", GetType(String))
    '        End With
    '    End If


    '    'Create the payment details table
    '    _dtPaymentDetails = New DataTable("PaymentDetails")
    '    ResultDataSet.Tables.Add(_dtPaymentDetails)
    '    If _dtPaymentDetails.Columns.Count = 0 Then
    '        AddPaymentDetailsTableColumns()
    '    End If

    '    Dim cmdSELECT As iDB2Command = Nothing
    '    Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
    '                              "/WS005R(@PARAM1,@PARAM2,@PARAM3)"
    '    Dim parmInput As iDB2Parameter
    '    Dim parmInput2 As iDB2Parameter
    '    Dim parmInput3 As iDB2Parameter
    '    Dim PARMOUT As String = String.Empty
    '    Dim PARMOUT2 As String = String.Empty
    '    Dim PARMOUT3 As String = String.Empty
    '    Dep.PaymentReference = Dep.PaymentReference.Trim()
    '    Dim paymentReferenceGiven As Long = Utilities.PadLeadingZeros(Dep.PaymentReference, 15)
    '    If Not err.HasError Then
    '        While moreResults And nextStartingNumber < 1000

    '            Try
    '                Dim det As New DETicketingItemDetails
    '                If Dep.CollDEOrders.Count > 0 Then
    '                    det = Dep.CollDEOrders.Item(1)
    '                Else
    '                    det.ProductCode = ""
    '                    det.Type = ""
    '                End If

    '                Dim returnFees As String = String.Empty
    '                If Settings.OriginatingSource.Trim.Length > 0 Then
    '                    returnFees = "Y"
    '                End If
    '                Dim custNumPayRefMatchValidation As String = String.Empty
    '                If Dep.CustNumberPayRefShouldMatch Then
    '                    custNumPayRefMatchValidation = "Y"
    '                Else
    '                    custNumPayRefMatchValidation = "N"
    '                End If

    '                cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
    '                parmInput = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 5120)
    '                parmInput.Value = Utilities.FixStringLength(String.Empty, 4962) & _
    '                                    Utilities.PadLeadingZeros(Dep.CallId, 13) & _
    '                                    Utilities.FixStringLength(Utilities.ConvertToYN(Dep.CorporateProductsOnly), 1) & _
    '                                    Utilities.FixStringLength(Dep.OrderStatus, 6) & _
    '                                    Utilities.FixStringLength(Dep.FromDate, 8) & _
    '                                    Utilities.FixStringLength(Dep.ToDate, 8) & _
    '                                    Utilities.FixStringLength(custNumPayRefMatchValidation, 1) & _
    '                                    Utilities.FixStringLength(returnFees, 1) & _
    '                                    Utilities.PadLeadingZeros(Dep.PaymentReference, 15) & _
    '                                    Utilities.FixStringLength("000000000000000000000000000000", 30) & _
    '                                    Utilities.FixStringLength(Settings.AuthorityUserProfile, 10) & _
    '                                    Utilities.FixStringLength(Settings.OriginatingSource, 10) & _
    '                                    Utilities.FixStringLength(det.ProductCode.ToString, 6) & _
    '                                    Utilities.FixStringLength(det.Type.ToString, 1) & _
    '                                    Utilities.FixStringLength("00000000000000000000", 20) & _
    '                                    Utilities.FixStringLength(String.Empty, 1) & _
    '                                    Utilities.FixStringLength("00000", 5) & _
    '                                    Utilities.FixStringLength(Settings.AccountNo1, 12) & _
    '                                    Utilities.FixStringLength("000", 3) & _
    '                                    nextStartingNumber.ToString.PadLeft(3, "0") & _
    '                                    Utilities.FixStringLength("W", 1) & _
    '                                    Utilities.FixStringLength(String.Empty, 2) & _
    '                                    Utilities.FixStringLength(String.Empty, 1)

    '                parmInput.Direction = ParameterDirection.InputOutput
    '                parmInput2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 20000)
    '                parmInput2.Value = Utilities.FixStringLength(" ", 20000)
    '                parmInput2.Direction = ParameterDirection.InputOutput
    '                parmInput3 = cmdSELECT.Parameters.Add(Param3, iDB2DbType.iDB2Char, 20000)
    '                parmInput3.Value = Utilities.FixStringLength(" ", 20000)
    '                parmInput3.Direction = ParameterDirection.InputOutput
    '                cmdSELECT.ExecuteNonQuery()
    '                PARMOUT = cmdSELECT.Parameters(Param1).Value.ToString
    '                PARMOUT2 = cmdSELECT.Parameters(Param2).Value.ToString
    '                PARMOUT3 = cmdSELECT.Parameters(Param3).Value.ToString
    '            Catch ex As Exception
    '                Const strError2 As String = "Error during database access"
    '                moreResults = False
    '                With err
    '                    .ErrorMessage = ex.Message
    '                    .ErrorStatus = strError2
    '                    .ErrorNumber = "TACDBOENQ02"
    '                    .HasError = True
    '                End With
    '            End Try

    '            If Not err.HasError Then
    '                '----------------
    '                ' Process Results
    '                '----------------


    '                Try
    '                    Dim dr As DataRow = Nothing
    '                    Dim iParmoutPosition As Integer = 0
    '                    Dim iCounter As Integer = 0

    '                    'PARMOUT2 populate OrderEnquiryDetails table (dtDetails) and Packagedetail (dtPackageDetail) table
    '                    iCounter = 0
    '                    Dim iPosition As Integer = 0
    '                    Dim sWork As String = PARMOUT2

    '                    Dim strQty As String = String.Empty

    '                    'If Dep.CallId = 0 Then  ' This is not a package so retrieve the order details.

    '                    For iCounter = 0 To 49
    '                        iPosition = iCounter * 400
    '                        If sWork.Substring(iPosition, 50).Trim > String.Empty Then
    '                            dr = Nothing
    '                            dr = dtDetails.NewRow()

    '                            If sWork.Substring(iPosition, 2) <> "00" Then
    '                                'dr("SaleDate") = CDate(sWork.Substring(iPosition, 10)).ToString("dd/MM/yyyy")
    '                                dr("SaleDate") = sWork.Substring(iPosition, 10)
    '                                dr("ProductDescription") = sWork.Substring(iPosition + 10, 40).Trim
    '                                dr("Seat") = sWork.Substring(iPosition + 50, 18).Trim
    '                                dr("SalePrice") = CDec(sWork.Substring(iPosition + 68, 12))
    '                                dr("BatchReference") = sWork.Substring(iPosition + 80, 9).Trim
    '                                dr("PaymentReference") = sWork.Substring(iPosition + 89, 15).Trim
    '                                ' dont set status for attendance 
    '                                If dr("Seat").ToString.Trim = "*AT/TEND" Then
    '                                    dr("StatusCode") = String.Empty
    '                                Else
    '                                    dr("StatusCode") = sWork.Substring(iPosition + 104, 6).Trim
    '                                End If

    '                                dr("LoyaltyPoints") = sWork.Substring(iPosition + 110, 5).Trim
    '                                dr("LoyaltyPointsExpired") = Utilities.convertToBool(sWork.Substring(iPosition + 145, 1).Trim)
    '                                dr("PromotionID") = sWork.Substring(iPosition + 115, 13).Trim
    '                                dr("ProductCode") = sWork.Substring(iPosition + 128, 6).Trim
    '                                dr("ProductType") = sWork.Substring(iPosition + 134, 1).Trim
    '                                Try
    '                                    If CDate(sWork.Substring(iPosition + 135, 10)).ToString("dd/MM/yyyy") = "00/00/00" Then
    '                                        dr("ProductDate") = "00/00/0000"
    '                                    Else
    '                                        dr("ProductDate") = CDate(sWork.Substring(iPosition + 135, 10)).ToString("dd/MM/yyyy")
    '                                    End If
    '                                Catch ex As Exception
    '                                    dr("ProductDate") = "00/00/0000"
    '                                End Try
    '                                dr("CATCancelWeb") = False
    '                                dr("CATCancelAgent") = False
    '                                Select Case sWork.Substring(iPosition + 146, 1).Trim
    '                                    Case Is = "A"
    '                                        dr("CATCancelAgent") = True
    '                                    Case Is = "Y"
    '                                        dr("CATCancelWeb") = True
    '                                        dr("CATCancelAgent") = True
    '                                End Select
    '                                dr("PriceBand") = sWork.Substring(iPosition + 147, 1).Trim
    '                                dr("CustomerNumber") = sWork.Substring(iPosition + 148, 12).Trim
    '                                dr("CustomerName") = sWork.Substring(iPosition + 160, 51).Trim
    '                                dr("PriceBandDesc") = sWork.Substring(iPosition + 211, 15).Trim
    '                                dr("StadiumCode") = sWork.Substring(iPosition + 226, 2).Trim
    '                                'If the original price is blank (eg. no promotion has been applied), set the original price to match the sale price
    '                                If String.IsNullOrEmpty(sWork.Substring(iPosition + 228, 12).Trim) Then
    '                                    dr("OriginalPrice") = CDec(sWork.Substring(iPosition + 68, 12))
    '                                Else
    '                                    dr("OriginalPrice") = Utilities.CheckForDBNull_Decimal(sWork.Substring(iPosition + 228, 12).Trim)
    '                                End If
    '                                dr("CATAmmendWeb") = False
    '                                dr("CATAmmendAgent") = False
    '                                Select Case sWork.Substring(iPosition + 240, 1).Trim
    '                                    Case Is = "A"
    '                                        dr("CATAmmendAgent") = True
    '                                    Case Is = "Y"
    '                                        dr("CATAmmendWeb") = True
    '                                        dr("CATAmmendAgent") = True
    '                                End Select
    '                                dr("CATTransferWeb") = False
    '                                dr("CATTransferAgent") = False
    '                                Select Case sWork.Substring(iPosition + 241, 1).Trim
    '                                    Case Is = "A"
    '                                        dr("CATTransferAgent") = True
    '                                    Case Is = "Y"
    '                                        dr("CATTransferWeb") = True
    '                                        dr("CATTransferAgent") = True
    '                                End Select
    '                                dr("FeesCode") = sWork.Substring(iPosition + 242, 6).Trim
    '                                If sWork.Substring(iPosition + 248, 1).Trim = "Y" Then
    '                                    dr("IsPrintable") = True
    '                                Else
    '                                    dr("IsPrintable") = False
    '                                End If
    '                                dr("RRN") = sWork.Substring(iPosition + 249, 15).Trim
    '                                If sWork.Substring(iPosition + 264, 13).Trim <> String.Empty Then
    '                                    dr("CallId") = sWork.Substring(iPosition + 264, 13).Trim
    '                                Else
    '                                    dr("CallId") = 0
    '                                End If
    '                                dr("RelatingBundleProduct") = sWork.Substring(iPosition + 279, 6).Trim
    '                                Dim seat As New DESeatDetails
    '                                seat.Stand = sWork.Substring(iPosition + 285, 3).Trim()
    '                                seat.Area = sWork.Substring(iPosition + 288, 4).Trim()
    '                                seat.Row = sWork.Substring(iPosition + 292, 4).Trim()
    '                                seat.Seat = sWork.Substring(iPosition + 296, 4).Trim()
    '                                seat.AlphaSuffix = sWork.Substring(iPosition + 300, 1).Trim()
    '                                dr("RelatingBundleSeat") = seat
    '                                dr("IsProductBundle") = (sWork.Substring(iPosition + 301, 1).Trim = "B")
    '                                If sWork.Substring(iPosition + 277, 1).Trim() = "Y" Then
    '                                    dr("Unreserved") = True
    '                                Else
    '                                    dr("Unreserved") = False
    '                                End If
    '                                If sWork.Substring(iPosition + 278, 1).Trim() = "Y" Then
    '                                    dr("Roving") = True
    '                                Else
    '                                    dr("Roving") = False
    '                                End If
    '                                dr("ComponentID") = sWork.Substring(iPosition + 302, 13).Trim.ConvertAndTrimStringToLong
    '                                dtDetails.Rows.Add(dr)
    '                            End If

    '                        Else
    '                            Exit For
    '                        End If
    '                    Next
    '                    'ElseIf Dep.CallId > 0 Then
    '                    PopulatePackageDetailTable(PARMOUT3, dtPackageDetail)
    '                    'End If
    '                    '------------------------
    '                    ' Check if more to return
    '                    '------------------------ 
    '                    lastDetailReturned = CInt(PARMOUT.Substring(5113, 3))
    '                    If firstCall Then
    '                        totalToReturn = CInt(PARMOUT.Substring(5110, 3))
    '                        firstCall = False
    '                    End If

    '                    If lastDetailReturned >= totalToReturn Then
    '                        moreResults = False
    '                    Else
    '                        nextStartingNumber = lastDetailReturned
    '                    End If

    '                    'PARMOUT populates PaymentOwnerDetails, PaymentDetails table (dtPaymentOwnerDetails, dtPaymentDetails)
    '                    If dtPaymentOwnerDetails.Rows.Count = 0 AndAlso PARMOUT.Substring(5119, 1).Trim.Length = 0 AndAlso PARMOUT.Substring(5117, 2).Trim.Length = 0 Then
    '                        'PaymentOwnerDetails
    '                        dr = Nothing
    '                        dr = dtPaymentOwnerDetails.NewRow
    '                        dr("Name") = PARMOUT.Substring(iParmoutPosition, 30).Trim
    '                        dr("AddressLine1") = PARMOUT.Substring(iParmoutPosition + 30, 30).Trim
    '                        dr("AddressLine2") = PARMOUT.Substring(iParmoutPosition + 60, 30).Trim
    '                        dr("AddressLine3") = PARMOUT.Substring(iParmoutPosition + 90, 25).Trim
    '                        dr("AddressLine4") = PARMOUT.Substring(iParmoutPosition + 115, 25).Trim
    '                        dr("PostCodePart1") = PARMOUT.Substring(iParmoutPosition + 140, 4).Trim
    '                        dr("PostCodePart2") = PARMOUT.Substring(iParmoutPosition + 144, 4).Trim
    '                        dr("OriginatedSource") = PARMOUT.Substring(iParmoutPosition + 148, 10).Trim
    '                        dr("TransactionDate") = PARMOUT.Substring(iParmoutPosition + 158, 10).Trim
    '                        dr("BatchDetail") = PARMOUT.Substring(iParmoutPosition + 168, 9).Trim
    '                        dtPaymentOwnerDetails.Rows.Add(dr)
    '                        PopulatePaymentDetailsTable(PARMOUT)
    '                    End If

    '                    'Populate Status Table
    '                    If dtStatusResults.Rows.Count = 0 Then
    '                        Dim dsr As DataRow = Nothing
    '                        dsr = dtStatusResults.NewRow()

    '                        dsr("PackageCancelFlag") = Utilities.CheckForDBNull_String(PARMOUT.Substring(4876, 1)).ConvertFromISeriesYesNoToBoolean()
    '                        dsr("PackageAmendFlag") = Utilities.CheckForDBNull_String(PARMOUT.Substring(4877, 1)).ConvertFromISeriesYesNoToBoolean()
    '                        dsr("PackageTransferFlag") = Utilities.CheckForDBNull_String(PARMOUT.Substring(4878, 1)).ConvertFromISeriesYesNoToBoolean()
    '                        dsr("ProductDescription") = Utilities.CheckForDBNull_String(PARMOUT.Substring(4879, 40))
    '                        dsr("PackageDescription") = Utilities.CheckForDBNull_String(PARMOUT.Substring(4919, 30))
    '                        dsr("ProductCode") = PARMOUT.Substring(4949, 6).Trim
    '                        dsr("ProductType") = PARMOUT.Substring(4955, 1).Trim
    '                        dsr("StadiumCode") = PARMOUT.Substring(4956, 2).Trim
    '                        dsr("ProductSubType") = PARMOUT.Substring(4958, 4).Trim

    '                        dsr("ErrorOccurred") = PARMOUT.Substring(5119, 1)
    '                        dsr("ReturnCode") = PARMOUT.Substring(5117, 2)
    '                        dsr("LoyaltyActive") = PARMOUT.Substring(5092, 1)
    '                        Try
    '                            dsr("CashbackBankedTotal") = CDec(PARMOUT.Substring(5015, 8) & "." & PARMOUT.Substring(5023, 2)).ToString("#######0.00")
    '                        Catch ex As Exception
    '                            dsr("CashbackBankedTotal") = "0.00"
    '                        End Try
    '                        Try
    '                            dsr("CashbackUnbankedTotal") = CDec(PARMOUT.Substring(5025, 8) & "." & PARMOUT.Substring(5033, 2)).ToString("#######0.00")
    '                        Catch ex As Exception
    '                            dsr("CashbackUnbankedTotal") = "0.00"
    '                        End Try
    '                        Try
    '                            dsr("CashbackSpentTotal") = CDec(PARMOUT.Substring(5035, 8) & "." & PARMOUT.Substring(5043, 2)).ToString("#######0.00")
    '                        Catch ex As Exception
    '                            dsr("CashbackSpentTotal") = "0.00"
    '                        End Try

    '                        Try
    '                            dsr("RewardTotal") = CDec(PARMOUT.Substring(5072, 8) & "." & PARMOUT.Substring(5080, 2)).ToString("#######0.00")
    '                        Catch ex As Exception
    '                            dsr("RewardTotal") = "0.00"
    '                        End Try

    '                        Try
    '                            dsr("BankedTotal") = CDec(PARMOUT.Substring(5082, 8) & "." & PARMOUT.Substring(5090, 2)).ToString("#######0.00")
    '                        Catch ex As Exception
    '                            dsr("BankedTotal") = "0.00"
    '                        End Try

    '                        Try
    '                            dsr("LoyaltyPoints") = CDec(PARMOUT.Substring(5093, 5)).ToString
    '                        Catch ex As Exception
    '                            dsr("LoyaltyPoints") = "0"
    '                        End Try

    '                        dsr("LastReturnedRecordNumber") = lastDetailReturned
    '                        dsr("TotalRecordNumber") = totalToReturn

    '                        dtStatusResults.Rows.Add(dsr)
    '                    Else
    '                        dtStatusResults.Rows(0)("LastReturnedRecordNumber") = lastDetailReturned
    '                    End If

    '                    If (Dep.CallId > 0) Then
    '                        PopulatePackageHistoryTable(PARMOUT.Substring(iParmoutPosition + 2000, 1000), dtPackageHistory)
    '                    End If

    '                Catch ex As Exception
    '                    Const strError2 As String = "Error processing results"
    '                    moreResults = False
    '                    With err
    '                        .ErrorMessage = ex.Message
    '                        .ErrorStatus = strError2
    '                        .ErrorNumber = "TACDBOENQ04"
    '                        .HasError = True
    '                    End With
    '                End Try
    '                If moreResults AndAlso paymentReferenceGiven <= 0 Then
    '                    moreResults = False
    '                End If
    '            Else
    '                moreResults = False
    '            End If

    '        End While

    '    End If

    '    Return err
    'End Function

    'Private Sub PopulatePackageHistoryTable(ByVal sWork As String, ByVal dtPackageHistory As DataTable)
    '    Dim iPosition As Integer = 0
    '    Dim iCounter As Integer = 0
    '    Dim dr As DataRow = Nothing
    '    Dim strRecord As String
    '    For iCounter = 0 To 9
    '        iPosition = iCounter * 100
    '        strRecord = sWork.Substring(iPosition, 15).Trim
    '        If Not String.IsNullOrEmpty(strRecord) AndAlso Long.Parse(strRecord) > 0 Then
    '            dr = Nothing
    '            dr = dtPackageHistory.NewRow()
    '            dr("PackageAmendmentId") = Utilities.CheckForDBNull_BigInt(sWork.Substring(iPosition, 15))
    '            dr("Comment") = sWork.Substring(iPosition + 15, 50).Trim
    '            dr("AmendmentDate") = Utilities.ISeriesDate(sWork.Substring(iPosition + 65, 7))
    '            dtPackageHistory.Rows.Add(dr)
    '        Else
    '            Exit For
    '        End If
    '    Next

    'End Sub

    'Private Sub PopulatePaymentDetailsTable(ByVal parmOut As String)
    '    'PaymentDetails
    '    'each array length 100 
    '    'number of array 10
    '    Dim iPosition As Integer = 0
    '    Dim iCounter As Integer = 0
    '    Dim dr As DataRow = Nothing
    '    Dim tempPayDetails As String = String.Empty
    '    Dim payDetailPosition As Integer = 0
    '    Dim tempPayType As String = String.Empty
    '    If parmOut.Substring(177, 1000).Trim.Length > 0 Then
    '        For iCounter = 0 To 9
    '            iPosition = 177 + (iCounter * 100)
    '            If parmOut.Substring(iPosition, 100).Trim.Length > 0 Then
    '                payDetailPosition = 0
    '                tempPayType = String.Empty
    '                dr = Nothing
    '                tempPayDetails = parmOut.Substring(iPosition, 100)
    '                dr = _dtPaymentDetails.NewRow
    '                tempPayType = tempPayDetails.Substring(payDetailPosition, 2).Trim.ToUpper
    '                dr("PayType") = tempPayType
    '                dr("PayAmount") = Utilities.FormatPrice(tempPayDetails.Substring(payDetailPosition + 2, 15).Trim)
    '                If parmOut.Substring(iPosition + 77, 1) = "Y" Then
    '                    dr("PayAmount") = dr("PayAmount") * -1
    '                End If
    '                If (tempPayType = GlobalConstants.CCPAYMENTTYPE) Then
    '                    dr("CardNumber") = tempPayDetails.Substring(payDetailPosition + 17, 4).Trim
    '                    dr("ExpiryDate") = tempPayDetails.Substring(payDetailPosition + 36, 4).Trim
    '                    dr("IssueDetail") = tempPayDetails.Substring(payDetailPosition + 40, 2).Trim
    '                    dr("StartDate") = tempPayDetails.Substring(payDetailPosition + 42, 4).Trim
    '                ElseIf (tempPayType = GlobalConstants.PAYPALPAYMENTTYPE) Then
    '                    dr("CardNumber") = tempPayDetails.Substring(payDetailPosition + 17, 19).Trim
    '                ElseIf (tempPayType = GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE) Then
    '                    dr("CardNumber") = tempPayDetails.Substring(payDetailPosition + 17, 19).Trim
    '                ElseIf (tempPayType = GlobalConstants.CSPAYMENTTYPE) Then

    '                ElseIf (tempPayType = GlobalConstants.DDPAYMENTTYPE) Then
    '                    dr("AccountNumber") = tempPayDetails.Substring(payDetailPosition + 17, 8).Trim
    '                    dr("SortCode") = tempPayDetails.Substring(payDetailPosition + 25, 6).Trim
    '                    dr("AccountName") = tempPayDetails.Substring(payDetailPosition + 31, 40).Trim
    '                End If
    '                _dtPaymentDetails.Rows.Add(dr)
    '            Else
    '                Exit For
    '            End If
    '        Next
    '    End If
    'End Sub

    'Private Sub AddPaymentDetailsTableColumns()
    '    With _dtPaymentDetails.Columns
    '        .Add("PayType", GetType(String))
    '        .Add("PayAmount", GetType(Decimal))
    '        'Credit card, Google Wallet, Paypal
    '        .Add("CardNumber", GetType(String))
    '        .Add("ExpiryDate", GetType(String))
    '        .Add("IssueDetail", GetType(String))
    '        .Add("StartDate", GetType(String))
    '        'Direct Debit
    '        .Add("AccountNumber", GetType(String))
    '        .Add("SortCode", GetType(String))
    '        .Add("AccountName", GetType(String))
    '    End With
    'End Sub

    'Private Function ReadOrderEnquiryDetailsTALENTTKT_Dummy() As ErrorObj
    '    Dim err As New ErrorObj
    '    dtDetails = New DataTable("OrderEnquiryDetails")
    '    ResultDataSet = New DataSet
    '    ResultDataSet.Tables.Add(dtDetails)

    '    If dtDetails.Columns.Count = 0 Then
    '        AddColumnsToDataTables()
    '    End If

    '    'Create the Status data table
    '    Dim dtStatusResults As New DataTable("StatusResults")
    '    ResultDataSet.Tables.Add(dtStatusResults)
    '    With dtStatusResults.Columns
    '        .Add("ErrorOccurred", GetType(String))
    '        .Add("ReturnCode", GetType(String))
    '        .Add("CashbackBankedTotal", GetType(String))
    '        .Add("CashbackUnbankedTotal", GetType(String))
    '        .Add("CashbackSpentTotal", GetType(String))
    '        .Add("BankedTotal", GetType(String))
    '        .Add("RewardTotal", GetType(String))
    '        .Add("LoyaltyActive", GetType(String))
    '        .Add("LoyaltyPoints", GetType(String))
    '    End With

    '    'Create the payment details table
    '    Dim dtPaymentOwnerDetails As New DataTable("PaymentOwnerDetails")
    '    ResultDataSet.Tables.Add(dtPaymentOwnerDetails)
    '    If dtPaymentOwnerDetails.Columns.Count = 0 Then
    '        With dtPaymentOwnerDetails.Columns
    '            .Add("Name", GetType(String))
    '            .Add("AddressLine1", GetType(String))
    '            .Add("AddressLine2", GetType(String))
    '            .Add("AddressLine3", GetType(String))
    '            .Add("AddressLine4", GetType(String))
    '            .Add("PostCode", GetType(String))
    '            .Add("OriginatedSource", GetType(String))
    '            .Add("TransactionDate", GetType(String))
    '            .Add("BatchDetail", GetType(Decimal))
    '        End With
    '    End If

    '    'Create the payment details table
    '    _dtPaymentDetails = New DataTable("PaymentDetails")
    '    ResultDataSet.Tables.Add(_dtPaymentDetails)
    '    If _dtPaymentDetails.Columns.Count = 0 Then
    '        AddPaymentDetailsTableColumns()
    '    End If


    '    Try
    '        Dim dr As DataRow = Nothing

    '        dr = Nothing
    '        dr = dtPaymentOwnerDetails.NewRow
    '        dr("Name") = "Testing"
    '        dr("AddressLine1") = "Address Line 1 Test"
    '        dr("AddressLine2") = "Address Line 2 Test"
    '        dr("AddressLine3") = "Address Line 3 Test"
    '        dr("AddressLine4") = "Address Line 4 Test"
    '        dr("PostCode") = "TEST PSO"
    '        dr("OriginatedSource") = "INTERNET"
    '        dr("TransactionDate") = "20/02/2013"
    '        dr("BatchDetail") = "123456"
    '        dtPaymentOwnerDetails.Rows.Add(dr)


    '        'PopulatePaymentDetailsTable(PARMOUT)
    '        dr = Nothing
    '        dr = _dtPaymentDetails.NewRow
    '        Dim tempPayType As String = "CC"
    '        dr("PayType") = tempPayType
    '        dr("PayAmount") = "123.56"
    '        If (tempPayType = GlobalConstants.CCPAYMENTTYPE) Then
    '            dr("CardNumber") = "4929123123123"
    '            dr("ExpiryDate") = "0213"
    '            dr("IssueDetail") = ""
    '            dr("StartDate") = "0112"
    '        ElseIf (tempPayType = GlobalConstants.PAYPALPAYMENTTYPE) Then
    '            dr("CardNumber") = "9876543210"
    '        ElseIf (tempPayType = GlobalConstants.GOOGLECHECKOUTPAYMENTTYPE) Then
    '            dr("CardNumber") = "1234567890"
    '        ElseIf (tempPayType = GlobalConstants.CSPAYMENTTYPE) Then

    '        ElseIf (tempPayType = GlobalConstants.DDPAYMENTTYPE) Then
    '            dr("AccountNumber") = "345678901"
    '            dr("SortCode") = "123456"
    '            dr("AccountName") = "TEST CUST"
    '        End If
    '        _dtPaymentDetails.Rows.Add(dr)

    '        dr = Nothing
    '        dr = dtDetails.NewRow()

    '        dr("SaleDate") = "19/02/2013"
    '        dr("ProductDescription") = "TEST PROD"
    '        dr("Seat") = "EEAS0001"
    '        dr("SalePrice") = "123.56"
    '        dr("BatchReference") = "123456"
    '        dr("PaymentReference") = "210987"
    '        dr("StatusCode") = "PEND"
    '        dr("LoyaltyPoints") = "24680"
    '        dr("LoyaltyPointsExpired") = False
    '        dr("PromotionID") = ""
    '        dr("ProductCode") = "PRCODE"
    '        dr("ProductType") = "H"

    '        dr("ProductDate") = CDate("20/02/2013").ToString("dd/MM/yyyy")

    '        dr("CATCancelWeb") = False
    '        dr("CATCancelAgent") = False
    '        dr("PriceBand") = "A"
    '        dr("CustomerNumber") = "000000010740"
    '        dr("CustomerName") = "Testing Cust"
    '        dr("PriceBandDesc") = "Adult"
    '        dr("StadiumCode") = "NT"
    '        dr("OriginalPrice") = "123.56"
    '        dr("CATAmmendWeb") = False
    '        dr("CATAmmendAgent") = False
    '        dr("CATTransferWeb") = False
    '        dr("CATTransferAgent") = False
    '        dr("FeesCode") = ""
    '        dr("IsPrintable") = True
    '        dtDetails.Rows.Add(dr)

    '        'Populate Status Table

    '        Dim dsr As DataRow = Nothing
    '        dsr = dtStatusResults.NewRow()
    '        dsr("ErrorOccurred") = ""
    '        dsr("ReturnCode") = ""
    '        dsr("LoyaltyActive") = "N"
    '        dsr("CashbackBankedTotal") = "0.00"
    '        dsr("CashbackUnbankedTotal") = "0.00"
    '        dsr("CashbackSpentTotal") = "0.00"
    '        dsr("RewardTotal") = "0.00"
    '        dsr("BankedTotal") = "0.00"
    '        dsr("LoyaltyPoints") = "0"
    '        dtStatusResults.Rows.Add(dsr)
    '    Catch ex As Exception
    '        Const strError2 As String = "Error processing results"
    '        With err
    '            .ErrorMessage = ex.Message
    '            .ErrorStatus = strError2
    '            .ErrorNumber = "TACDBOENQ04"
    '            .HasError = True
    '        End With
    '    End Try

    '    Return err
    'End Function
#End Region


End Class

#End Region


