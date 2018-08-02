Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text


<Serializable()> _
Public Class DBOrderFunctions
    'Inherits DBAccess

    Protected Const Source As String = "@Source"
    Protected Const ErrorCode As String = "@Error"
    Protected Const RecordCount As String = "@RecordCount"
    Protected Const Param0 As String = "@Param0"
    Protected Const Param1 As String = "@Param1"
    Protected Const Param2 As String = "@Param2"
    Protected Const Param3 As String = "@Param3"

    Private _de As New DETicketingItemDetails
    Private _resultDataSet As DataSet
    Private _conTALENTTKT As iDB2Connection = Nothing
    Private _storedProcedureGroup As String = String.Empty
    Private _parm2WS030R As String = String.Empty
    Private _parmWS030R As String = String.Empty

    Public Property De() As DETicketingItemDetails
        Get
            Return _de
        End Get
        Set(ByVal value As DETicketingItemDetails)
            _de = value
        End Set
    End Property
    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property
    Public Property conTALENTTKT() As iDB2Connection
        Get
            Return _conTALENTTKT
        End Get
        Set(ByVal value As iDB2Connection)
            _conTALENTTKT = value
        End Set
    End Property
    Public Property StoredProcedureGroup() As String
        Get
            Return _storedProcedureGroup
        End Get
        Set(ByVal value As String)
            _storedProcedureGroup = value
        End Set
    End Property
    Public Property ParmWS030R() As String
        Get
            Return _parmWS030R
        End Get
        Set(ByVal value As String)
            _parmWS030R = value
        End Set
    End Property

    Public Function AccessDatabaseWS030R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim sLastRecord As String = "00000"
        Dim sRecordTotal As String = "00000"
        Dim MD009rrn As Long = 0
        Dim MD008rrn As Long = 0
        Dim CD051rrn As Long = 0
        Dim PARAMOUT As String = String.Empty
        Dim bMoreRecords As Boolean = True
        Dim tempGoodsValue As String = String.Empty
        Dim tempNumberOfUnits As String = String.Empty

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("PaymentReference", GetType(String))
            .Add("TotalPrice", GetType(String))
            .Add("PaymentMethod", GetType(String))
            .Add("RefundAmount", GetType(String))
            .Add("OnAccountAmount", GetType(Decimal))
            .Add("onAccountRefunded", GetType(Boolean))
            .Add("deliveryContactName", GetType(String))
            .Add("deliveryAddress1", GetType(String))
            .Add("deliveryAddress2", GetType(String))
            .Add("deliveryAddress3", GetType(String))
            .Add("deliveryAddress4", GetType(String))
            .Add("deliveryAddress5", GetType(String))
            .Add("deliveryPostCode", GetType(String))
            .Add("SaleDate", GetType(Date))
            .Add("TempOrderId", GetType(String))
            .Add("EMailTemplateID", GetType(Integer))
        End With

        'Create the payment Details data table
        Dim DtPurchaseResults As New DataTable("PurchaseResults")
        ResultDataSet.Tables.Add(DtPurchaseResults)
        With DtPurchaseResults.Columns
            .Add("ProductCode", GetType(String))
            .Add("ProductDescription", GetType(String))
            .Add("Seat", GetType(String))
            .Add("CustomerNo", GetType(String))
            .Add("ContactForename", GetType(String))
            .Add("ContactSurname", GetType(String))
            .Add("Price", GetType(String))
            .Add("ProductDate", GetType(String))
            .Add("ProductTime", GetType(String))
            .Add("Turnstiles", GetType(String))
            .Add("Gates", GetType(String))
            .Add("SmartcardUploaded", GetType(String))
            .Add("FeeType", GetType(String))
            .Add("PriceCode", GetType(String))
            .Add("PriceBand", GetType(String))
            .Add("BarcodeValue", GetType(String))
            .Add("CarriageMethod", GetType(String))
            .Add("CardPrint", GetType(String))
            .Add("SeatRestriction", GetType(String))
            .Add("TicketId", GetType(String))
            .Add("ProductType", GetType(String))
            .Add("PrintLayout", GetType(String))
            .Add("SeatSuffix", GetType(String))
            .Add("TicketText", GetType(String))
            .Add("PayRef", GetType(String))
            .Add("PayType", GetType(String))
            .Add("PayTypeDesc", GetType(String))
            .Add("PaymentAmnt", GetType(String))
            .Add("Stand", GetType(String))
            .Add("Area", GetType(String))
            .Add("RowN", GetType(String))
            .Add("SeatN", GetType(String))
            .Add("SeatText", GetType(String))
            .Add("RestText", GetType(String))
            .Add("PriceCDesc", GetType(String))
            .Add("PriceBDesc", GetType(String))
            .Add("Title", GetType(String))
            .Add("StandDesc", GetType(String))
            .Add("AreaText", GetType(String))
            .Add("AddressLine1", GetType(String))
            .Add("AddressLine2", GetType(String))
            .Add("AddressLine3", GetType(String))
            .Add("AddressLine4", GetType(String))
            .Add("AddressLine5", GetType(String))
            .Add("PostCode", GetType(String))
            .Add("ClientReferenceName", GetType(String))
            .Add("CallReference", GetType(Decimal))
            .Add("PackageDescription", GetType(String))
            .Add("NumberOfUnits", GetType(Integer))
            .Add("GoodsValue", GetType(Decimal))
            .Add("TotalVATValue", GetType(Decimal))
            .Add("CorporateLayout", GetType(String))
            .Add("PackageID", GetType(String))
            .Add("CostPerPackage", GetType(Decimal))
            .Add("CancelledSeat", GetType(Boolean))
            .Add("VoucherCode", GetType(String))
            .Add("ComponentDescription", GetType(String))
            .Add("Unreserved", GetType(String))
            .Add("Roving", GetType(String))
            .Add("Bundle", GetType(Boolean))
            .Add("PackagePart", GetType(Boolean))
            .Add("RelatingBundleProduct", GetType(String))
            .Add("RelatingBundleSeat", GetType(String))
            .Add("Quantity", GetType(Integer))
            .Add("BulkID", GetType(Integer))
            .Add("VoucherExpiryDate", GetType(String))
            .Add("BundleStartDate", GetType(String))
            .Add("BundleEndDate", GetType(String))
            .Add("HideDate", GetType(Boolean))
            .Add("HideTime", GetType(Boolean))
            .Add("BuybackOrTicketExchange", GetType(String))
            .Add("SummariseRecordsOnEmail", GetType(Boolean))
        End With

        Dim tempNumericHolder As String = String.Empty

        Try

            'Loop until all the products purchased have been retrieved
            Do While bMoreRecords = True

                If ParmWS030R.Trim <> "" And sLastRecord = "00000" Then
                    PARAMOUT = ParmWS030R
                Else
                    'Call WS030R
                    PARAMOUT = CallWS030R(sRecordTotal, sLastRecord, MD009rrn, MD008rrn, CD051rrn)
                End If

                If sLastRecord = "00000" Then

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
                    dRow("PaymentReference") = De.PaymentReference.ToString

                    If String.IsNullOrEmpty((PARAMOUT.Substring(2806, 13).Trim)) Then
                        dRow("EMailTemplateID") = 0
                    Else
                        dRow("EMailTemplateID") = PARAMOUT.Substring(2806, 13).Trim
                    End If

                    'If money is being refunded populate "RefundAmount"  
                    If PARAMOUT.Substring(2946, 1) = "Y" Then
                        dRow("RefundAmount") = Utilities.FormatPrice(PARAMOUT.Substring(2980, 15))
                        dRow("TotalPrice") = "0"
                    Else
                        dRow("TotalPrice") = Utilities.FormatPrice(PARAMOUT.Substring(2980, 15))
                        dRow("RefundAmount") = "0"
                    End If

                    dRow("OnAccountAmount") = Decimal.Zero
                    If Not String.IsNullOrWhiteSpace(PARAMOUT.Substring(2977, 9)) Then
                        dRow("OnAccountAmount") = Utilities.FormatPrice(PARAMOUT.Substring(2937, 9))
                        dRow("onAccountRefunded") = Utilities.convertToBool(PARAMOUT.Substring(2936, 1))
                    End If

                    dRow("PaymentMethod") = PARAMOUT.Substring(2977, 2)
                    dRow("deliveryContactName") = PARAMOUT.Substring(0, 30).Trim() + PARAMOUT.Substring(178, 30).Trim()
                    dRow("deliveryAddress1") = PARAMOUT.Substring(30, 30).Trim()
                    dRow("deliveryAddress2") = PARAMOUT.Substring(60, 30).Trim()
                    dRow("deliveryAddress3") = PARAMOUT.Substring(90, 25).Trim()
                    dRow("deliveryAddress4") = PARAMOUT.Substring(115, 25).Trim()
                    dRow("deliveryAddress5") = PARAMOUT.Substring(140, 30).Trim()
                    dRow("deliveryPostCode") = PARAMOUT.Substring(170, 4).Trim() + " " + PARAMOUT.Substring(174, 4).Trim()
                    dRow("TempOrderId") = PARAMOUT.Substring(2829, 100).Trim
                    dRow("SaleDate") = Talent.Common.Utilities.ISeriesDate(PARAMOUT.Substring(178, 7))
                    DtStatusResults.Rows.Add(dRow)
                End If

                'No errors 
                If PARAMOUT.Substring(3071, 1) <> "E" And PARAMOUT.Substring(3069, 2).Trim = "" Then

                    'Extract the data from the parameter
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 20

                        ' For DD Season Ticket Renewals then parm2 of WS030 is always zero length
                        If _parm2WS030R.Trim.Length = 0 Then
                            bMoreRecords = False
                            Exit Do
                        End If

                        ' Has a product been returned
                        If _parm2WS030R.Substring(iPosition, 3).Trim = "" Then
                            bMoreRecords = False
                            Exit Do
                        Else

                            'Create a new row
                            tempNumericHolder = String.Empty
                            dRow = Nothing
                            dRow = DtPurchaseResults.NewRow
                            dRow("ProductCode") = _parm2WS030R.Substring(iPosition, 6).Trim()
                            dRow("ProductDescription") = _parm2WS030R.Substring(iPosition + 6, 40).Trim()
                            dRow("Seat") = _parm2WS030R.Substring(iPosition + 46, 18).Trim & _parm2WS030R.Substring(iPosition + 142, 1)
                            dRow("CustomerNo") = _parm2WS030R.Substring(iPosition + 64, 12).Trim()
                            dRow("ContactForename") = _parm2WS030R.Substring(iPosition + 76, 20).Trim()
                            dRow("ContactSurname") = _parm2WS030R.Substring(iPosition + 96, 30).Trim()
                            dRow("Price") = Utilities.FormatPrice(_parm2WS030R.Substring(iPosition + 126, 9)).Trim()
                            dRow("ProductDate") = _parm2WS030R.Substring(iPosition + 135, 7).Trim()
                            dRow("BundleStartDate") = _parm2WS030R.Substring(iPosition + 1175, 7).Trim()
                            dRow("BundleEndDate") = _parm2WS030R.Substring(iPosition + 1182, 7).Trim()
                            dRow("HideDate") = Utilities.convertToBool(_parm2WS030R.Substring(iPosition + 1194, 1).Trim())
                            dRow("HideTime") = Utilities.convertToBool(_parm2WS030R.Substring(iPosition + 1195, 1).Trim())
                            dRow("Turnstiles") = _parm2WS030R.Substring(iPosition + 143, 12).Trim()
                            dRow("Gates") = _parm2WS030R.Substring(iPosition + 155, 12).Trim()
                            If _parm2WS030R.Substring(iPosition + 167, 1) = "0" Then
                                dRow("SmartcardUploaded") = "Y"
                            Else
                                dRow("SmartcardUploaded") = "N"
                            End If
                            dRow("FeeType") = _parm2WS030R.Substring(iPosition + 168, 1)
                            dRow("PriceCode") = _parm2WS030R.Substring(iPosition + 169, 2)
                            dRow("PriceBand") = _parm2WS030R.Substring(iPosition + 171, 1)
                            dRow("ProductTime") = _parm2WS030R.Substring(iPosition + 172, 7).Trim()

                            'Barcode Field has been extended from 25 chars to 50 chars. 
                            dRow("CardPrint") = _parm2WS030R.Substring(iPosition + 167, 1).Trim
                            dRow("BarcodeValue") = _parm2WS030R.Substring(iPosition + 179, 50).Trim
                            dRow("CarriageMethod") = _parm2WS030R.Substring(iPosition + 229, 1).Trim
                            dRow("SeatRestriction") = _parm2WS030R.Substring(iPosition + 230, 2).Trim
                            dRow("TicketId") = _parm2WS030R.Substring(iPosition + 232, 10).Trim
                            dRow("ProductType") = _parm2WS030R.Substring(iPosition + 242, 1).Trim
                            dRow("PrintLayout") = _parm2WS030R.Substring(iPosition + 243, 8).Trim
                            dRow("SeatSuffix") = _parm2WS030R.Substring(iPosition + 142, 1).Trim
                            dRow("TicketText") = _parm2WS030R.Substring(iPosition + 251, 40).Trim
                            dRow("PayRef") = GetZeroIfEmpty(_parm2WS030R.Substring(iPosition + 291, 15).Trim)
                            dRow("PayType") = _parm2WS030R.Substring(iPosition + 306, 2).Trim
                            dRow("PayTypeDesc") = _parm2WS030R.Substring(iPosition + 308, 13).Trim
                            dRow("PaymentAmnt") = Utilities.FormatPrice(GetZeroIfEmpty(_parm2WS030R.Substring(iPosition + 321, 15).Trim))
                            dRow("Stand") = _parm2WS030R.Substring(iPosition + 336, 3).Trim
                            dRow("Area") = _parm2WS030R.Substring(iPosition + 339, 4).Trim
                            dRow("RowN") = _parm2WS030R.Substring(iPosition + 343, 4).Trim
                            dRow("SeatN") = _parm2WS030R.Substring(iPosition + 347, 4).Trim
                            dRow("SeatText") = _parm2WS030R.Substring(iPosition + 351, 20).Trim
                            dRow("RestText") = _parm2WS030R.Substring(iPosition + 371, 20).Trim
                            dRow("PriceCDesc") = _parm2WS030R.Substring(iPosition + 391, 40).Trim
                            dRow("PriceBDesc") = _parm2WS030R.Substring(iPosition + 769, 30).Trim
                            dRow("VoucherExpiryDate") = _parm2WS030R.Substring(iPosition + 431, 7).Trim
                            dRow("Title") = _parm2WS030R.Substring(iPosition + 446, 6).Trim
                            dRow("StandDesc") = _parm2WS030R.Substring(iPosition + 452, 30).Trim
                            dRow("AreaText") = _parm2WS030R.Substring(iPosition + 482, 30).Trim
                            dRow("AddressLine1") = _parm2WS030R.Substring(iPosition + 512, 30).Trim
                            dRow("AddressLine2") = _parm2WS030R.Substring(iPosition + 542, 30).Trim
                            dRow("AddressLine3") = _parm2WS030R.Substring(iPosition + 572, 25).Trim
                            dRow("AddressLine4") = _parm2WS030R.Substring(iPosition + 597, 25).Trim
                            dRow("AddressLine5") = _parm2WS030R.Substring(iPosition + 622, 20).Trim
                            dRow("PostCode") = (_parm2WS030R.Substring(iPosition + 642, 4).Trim & " " & _parm2WS030R.Substring(iPosition + 646, 4).Trim).Trim
                            dRow("ClientReferenceName") = _parm2WS030R.Substring(iPosition + 650, 30).Trim
                            dRow("CallReference") = GetZeroIfEmpty(_parm2WS030R.Substring(iPosition + 680, 13).Trim)
                            dRow("PackageDescription") = _parm2WS030R.Substring(iPosition + 693, 30).Trim

                            tempNumberOfUnits = GetZeroIfEmpty(_parm2WS030R.Substring(iPosition + 1189, 5).Trim)
                            tempGoodsValue = Utilities.FormatPrice(GetZeroIfEmpty(_parm2WS030R.Substring(iPosition + 726, 10).Trim))
                            dRow("NumberOfUnits") = tempNumberOfUnits
                            dRow("GoodsValue") = tempGoodsValue
                            If CInt(tempNumberOfUnits) > 0 Then
                                dRow("CostPerPackage") = (CDec(tempGoodsValue) / CInt(tempNumberOfUnits))
                            Else
                                dRow("CostPerPackage") = "0.00"
                            End If

                            dRow("TotalVATValue") = Utilities.FormatPrice(GetZeroIfEmpty(_parm2WS030R.Substring(iPosition + 736, 10).Trim))
                            dRow("CorporateLayout") = _parm2WS030R.Substring(iPosition + 746, 10).Trim
                            dRow("PackageID") = _parm2WS030R.Substring(iPosition + 756, 13).Trim
                            If _parm2WS030R.Substring(iPosition + 799, 1).Trim = "D" Then
                                dRow("CancelledSeat") = True
                            Else
                                dRow("CancelledSeat") = False
                            End If
                            dRow("VoucherCode") = _parm2WS030R.Substring(iPosition + 1069, 30).Trim
                            dRow("ComponentDescription") = _parm2WS030R.Substring(iPosition + 1101, 30).Trim
                            dRow("Unreserved") = _parm2WS030R.Substring(iPosition + 1099, 1).Trim
                            dRow("Roving") = _parm2WS030R.Substring(iPosition + 1100, 1).Trim
                            dRow("Bundle") = (_parm2WS030R.Substring(iPosition + 1154, 1).Trim = "Y")
                            dRow("PackagePart") = (_parm2WS030R.Substring(iPosition + 1155, 1).Trim = "Y")
                            dRow("RelatingBundleProduct") = _parm2WS030R.Substring(iPosition + 1101, 6).Trim
                            Dim seat As New DESeatDetails
                            seat.Stand = _parm2WS030R.Substring(iPosition + 1107, 3).Trim
                            seat.Area = _parm2WS030R.Substring(iPosition + 1110, 4).Trim
                            seat.Row = _parm2WS030R.Substring(iPosition + 1114, 4).Trim
                            seat.Seat = _parm2WS030R.Substring(iPosition + 1118, 4).Trim
                            seat.AlphaSuffix = _parm2WS030R.Substring(iPosition + 1122, 1).Trim
                            dRow("RelatingBundleSeat") = seat.FormattedSeat
                            dRow("Quantity") = Utilities.CheckForDBNull_Int(_parm2WS030R.Substring(iPosition + 1156, 5).Trim)
                            dRow("BulkID") = Utilities.CheckForDBNull_Int(_parm2WS030R.Substring(iPosition + 1161, 13).Trim)
                            dRow("BuybackOrTicketExchange") = _parm2WS030R.Substring(iPosition + 1174, 1).Trim
                            dRow("SummariseRecordsOnEmail") = Utilities.convertToBool(_parm2WS030R.Substring(iPosition + 1196, 1).Trim)
                            DtPurchaseResults.Rows.Add(dRow)

                            'Increment
                            'Each slot 1200 long
                            iPosition = iPosition + 1200
                            iCounter = iCounter + 1

                        End If
                    Loop

                    'Extract the footer information
                    sLastRecord = PARAMOUT.Substring(2824, 5)
                    sRecordTotal = PARAMOUT.Substring(2819, 5)
                    MD009rrn = PARAMOUT.Substring(3033, 15)
                    MD008rrn = PARAMOUT.Substring(3018, 15)
                    CD051rrn = PARAMOUT.Substring(2962, 15)
                    If CInt(sLastRecord) >= CInt(sRecordTotal) Then
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
                .ErrorNumber = "TACDBOF-01"
                .HasError = True
            End With
        End Try

        Return err

    End Function

    Private Function CallWS030R(ByVal sRecordTotal As String, _
                                ByVal sLastRecord As String, _
                                ByVal MD009rrn As Long, _
                                ByVal MD008rrn As Long, _
                                ByVal CD051rrn As Long) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS030R"
        Dim strHEADER As String = "CALL " & StoredProcedureGroup.Trim & _
                                    "/" & strProgram & "(@PARAM1,@PARAM2)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty
        'Dim PARAMOUT2 As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add("@Param1", iDB2DbType.iDB2Char, 3072)
        parmIO.Value = WS030Parm(sRecordTotal, sLastRecord, MD009rrn, MD008rrn, CD051rrn)
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO = cmdSELECT.Parameters.Add("@Param2", iDB2DbType.iDB2Char, 24000)
        parmIO.Value = ""
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@Param1").Value.ToString
        _parm2WS030R = cmdSELECT.Parameters("@Param2").Value.ToString

        Return PARAMOUT

    End Function

    Public Function WS030Parm(ByVal sRecordTotal As String, ByVal sLastRecord As String, ByVal MD009rrn As Long, ByVal MD008rrn As Long, ByVal CD051rrn As Long) As String
        Dim myString As New StringBuilder
        Dim request As String = String.Empty
        If De.PaymentReference > 0 Then
            request = "1"
        Else
            request = "2"
        End If

        ' If retrieving information for upgrade, transfer, cancellation email then request will be 4,5 or 6
        If De.RequestType IsNot Nothing AndAlso De.RequestType <> String.Empty Then
            request = De.RequestType
        End If

        myString.Append(Utilities.FixStringLength("", 2786))
        myString.Append(Utilities.FixStringLength(De.BusinessUnit, 20))
        myString.Append(Utilities.FixStringLength("", 13))
        myString.Append(Utilities.FixStringLength(sRecordTotal, 5))
        myString.Append(Utilities.FixStringLength(sLastRecord, 5))
        myString.Append(Utilities.FixStringLength("", 132))
        myString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(De.RetryFailure), 1))
        myString.Append(Utilities.PadLeadingZeros(CD051rrn, 15))
        myString.Append(Utilities.FixStringLength("", 2))
        myString.Append(Utilities.PadLeadingZeros(De.SetAsPrinted, 1))
        myString.Append(Utilities.PadLeadingZeros(0, 15))
        myString.Append(Utilities.PadLeadingZeros(De.UnprintedRecords, 1))
        myString.Append(Utilities.FixStringLength("", 1))
        myString.Append(Utilities.PadLeadingZeros(De.PaymentReference, 15))
        myString.Append(Utilities.FixStringLength(De.ProductCode, 6))
        myString.Append(Utilities.PadLeadingZeros(MD008rrn, 15))
        myString.Append(Utilities.PadLeadingZeros(MD009rrn, 15))
        myString.Append(Utilities.FixStringLength(De.EndOfSale, 1))
        myString.Append(Utilities.FixStringLength(request, 1))
        myString.Append(Utilities.FixStringLength(De.CustomerNo, 12))
        myString.Append(Utilities.FixStringLength("", 6))
        myString.Append(Utilities.FixStringLength(De.Src, 1))
        myString.Append("   ")

        Return myString.ToString()
    End Function
#Region "WS030S"

    Public Function AccessDatabaseWS030S() As ErrorObj
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
            CallWS030S()
        Catch ex As Exception
            Const strError8 As String = "Error during database Access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError8
                .ErrorNumber = "TACDBOF-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Sub CallWS030S()
        Dim _cmd As iDB2Command = Nothing
        Dim _cmdAdapter As iDB2DataAdapter = Nothing
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL WS030S( @PARAM1, @ERROR )"
        _cmd.CommandType = CommandType.Text

        Dim pPayref As iDB2Parameter
        Dim pErrorCode As iDB2Parameter
       

        pPayref = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Decimal, 15)
        pPayref.Value = De.PaymentReference

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pErrorCode.Value = String.Empty

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet)
        Utilities.ConvertISeriesTables(ResultDataSet)
        If ResultDataSet.Tables.Count = 2 Then
            ResultDataSet.Tables(1).TableName = "BulkSeats"
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

    Private Function GetZeroIfEmpty(ByVal valueToVerify As String) As String
        Dim tempReturnValue As String = String.Empty
        If IsNumeric(valueToVerify) Then
            tempReturnValue = valueToVerify
        Else
            tempReturnValue = "0"
        End If
        Return tempReturnValue
    End Function

End Class
