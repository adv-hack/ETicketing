Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.Sql
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports Talent.Common
Imports System.Text

<Serializable()> _
Public Class DBBasketFunctions

#Region "Public Properties"

    Public Property Source() As String
    Public Property SessionId() As String
    Public Property CustomerNo() As String
    Public Property conTALENTTKT() As iDB2Connection
    Public Property StoredProcedureGroup() As String
    Public Property OriginatingSource() As String
    Public Property LinkedProductId() As Integer = 0
    Public Property BulkSalesMode() As Boolean

#End Region

#Region "Private Functions"

    Private Function CallWS036R(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String

        'Create command object
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strProgram As String = "WS036R"
        Dim strHEADER As String = "CALL " & StoredProcedureGroup & _
                                    "/" & strProgram & "(@PARAM1)"
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        'Set the connection string
        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)

        'Populate the parameter
        parmIO = cmdSELECT.Parameters.Add("Param1", iDB2DbType.iDB2Char, 5120)
        parmIO.Value = WS036Parm(sRecordTotal, sLastRecord)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        Utilities.TalentCommonLog("CallWS036R", CustomerNo, "Backend Request: strHEADER=" & strHEADER & ", parmIO.Value=" & parmIO.Value)

        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("Param1").Value.ToString

        Utilities.TalentCommonLog("CallWS036R", CustomerNo, "Backend Response: PARAMOUT=" & PARAMOUT)

        Return PARAMOUT

    End Function

    Private Function WS036Parm(ByVal sRecordTotal As String, ByVal sLastRecord As String) As String
        Dim myString As New StringBuilder
        Dim department As String = String.Empty

        'Error Handling for objects that are not set.
        If _customerNo Is Nothing Then _customerNo = GlobalConstants.GENERIC_CUSTOMER_NUMBER
        If _sessionId Is Nothing Then _sessionId = String.Empty

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(String.Empty, 4947))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.FixStringLength(Me.OriginatingSource, 10))
        myString.Append(Utilities.FixStringLength(String.Empty, 80))
        myString.Append(Utilities.FixStringLength(department, 10))
        myString.Append(Utilities.FixStringLength(String.Empty, 14))
        myString.Append(Utilities.FixStringLength(_sessionId, 36))
        myString.Append(Utilities.PadLeadingZeros(_customerNo, 12))
        myString.Append(Utilities.FixStringLength(sRecordTotal, 3))
        myString.Append(Utilities.FixStringLength(sLastRecord, 3))
        myString.Append(Utilities.FixStringLength(Source, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))

        Return myString.ToString
    End Function

    Private Sub AddColumnsToBasketStatusTable(ByRef dt As DataTable)
        With dt.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
            .Add("ValidationError", GetType(String))
        End With
    End Sub

    Private Sub AddColumnsToBasketHeaderTable(ByRef dt As DataTable)
        With dt.Columns
            .Add("MarketingCampaign", GetType(String))
            .Add("UserSelectFulfilment", GetType(String))
            .Add("ReservedQuantity", GetType(String))
            .Add("ApplyCarriageFee", GetType(String))
            .Add("ApplyBookingFee", GetType(String))
            .Add("ApplyWebSalesFee", GetType(String))
            .Add("ApplyWebServicesFee", GetType(String))
            .Add("CarriageFee", GetType(String))
            .Add("BookingFee", GetType(String))
            .Add("WebSalesFee", GetType(String))
            .Add("WebServicesFee", GetType(String))
            .Add("TotalPrice", GetType(String))
            .Add("PaymentOptions", GetType(String))
            .Add("RestrictPaymentOptions", GetType(String))
            .Add("PaymentAccountId", GetType(String))
            .Add("PaymentExternalToken", GetType(String))
            .Add("CATMode", GetType(String))
            .Add("CATPrice", GetType(String))
            .Add("FeesExcluded", GetType(String))
            .Add("OriginalSalePaidWithCF", GetType(Boolean))
            .Add("DeliveryCountryCode", GetType(String))
        End With
    End Sub

    Private Sub AddColumnsToBasketDetailsTable(ByRef Dt As DataTable)
        With Dt.Columns
            .Add("CustomerNo", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("PriceCode", GetType(String))
            .Add("PriceBand", GetType(String))
            .Add("Seat", GetType(String))
            .Add("ProductType", GetType(String))
            .Add("Price", GetType(String))
            .Add("ProductDescription", GetType(String))
            .Add("StandDescription", GetType(String))
            .Add("AreaDescription", GetType(String))
            .Add("BandDescription", GetType(String))
            .Add("ReservedSeat", GetType(String))
            .Add("CurrentFulfilSlctn", GetType(String))
            .Add("ErrorCode", GetType(String))
            .Add("ProductLimit", GetType(String))
            .Add("UserLimit", GetType(String))
            .Add("ErrorInformation", GetType(String))
            .Add("ProductTime", GetType(String))
            .Add("ProductOppositionCode", GetType(String))
            .Add("ProductCompetitionCode", GetType(String))
            .Add("ProductCompetitionText", GetType(String))
            .Add("ProductDate", GetType(String))
            .Add("SeatRestriction", GetType(String))
            .Add("IsFree", GetType(String))
            .Add("ProductSubType", GetType(String))
            .Add("OptPostFul", GetType(String))
            .Add("OptCollFul", GetType(String))
            .Add("OptP@HFul", GetType(String))
            .Add("OptSCUploadFul", GetType(String))
            .Add("OptRegPostFul", GetType(String))
            .Add("OptPrintFul", GetType(String))
            .Add("PackageID", GetType(String))
            .Add("Quantity", GetType(String))
            .Add("VatValue", GetType(String))
            .Add("IncludeTravel", GetType(String))
            .Add("CanSaveAsFavouriteSeat", GetType(Boolean))
            .Add("ReservedForCurrentLoginid", GetType(Boolean))
            .Add("PromotionId1", GetType(String))
            .Add("PromotionId2", GetType(String))
            .Add("PromotionId3", GetType(String))
            .Add("OriginalPrice", GetType(String))
            .Add("CATSeatDetails", GetType(String))
            .Add("PricedBnds", GetType(String))
            .Add("LinkedProductId", GetType(Integer))
            .Add("FeeCategory", GetType(String))
            .Add("ProductTypeActual", GetType(String))
            .Add("IsNegativeFee", GetType(Boolean))
            .Add("IsSystemFee", GetType(Boolean))
            .Add("IsExternal", GetType(Boolean))
            .Add("IsIncluded", GetType(Boolean))
            .Add("CustomerAllocation", GetType(String))
            .Add("VoucherDefinitionId", GetType(Long))
            .Add("VoucherCode", GetType(String))
            .Add("Unreserved", GetType(String))
            .Add("Roving", GetType(String))
            .Add("CannotApplyFees", GetType(Boolean))
            .Add("CATQuantity", GetType(String))
            .Add("CATFulfilment", GetType(String))
            .Add("financeClubProductID", GetType(String))
            .Add("financePlanID", GetType(String))
            .Add("BulkSalesID", GetType(Integer))
            .Add("PackageType", GetType(String))
            .Add("AllocatedSeat", GetType(String))
            .Add("ReservationCode", GetType(String))
            .Add("BundleStartDate", GetType(String))
            .Add("BundleEndDate", GetType(String))
            .Add("RestrictedBasketOptions", GetType(Boolean))
            .Add("DisplayInACancelBasket", GetType(String))
            .Add("CallId", GetType(Long))
            .Add("BasketHasExceptionSeats", GetType(Boolean))
        End With
    End Sub

    Private Sub AddValuesToBasketDetailsTableRow(ByRef dRow As DataRow, ByVal PARAMOUT As String, ByVal iPosition As Integer, ByVal backendProgramName As String)
        dRow("CustomerNo") = PARAMOUT.Substring(iPosition, 12)
        dRow("ProductCode") = PARAMOUT.Substring(iPosition + 12, 6).Trim
        dRow("PriceCode") = PARAMOUT.Substring(iPosition + 18, 2).Trim()
        dRow("PriceBand") = PARAMOUT.Substring(iPosition + 20, 1).Trim()
        dRow("Seat") = PARAMOUT.Substring(iPosition + 21, 15) & PARAMOUT.Substring(iPosition + 161, 1)
        dRow("ProductType") = PARAMOUT.Substring(iPosition + 36, 1).Trim()
        dRow("ProductDescription") = PARAMOUT.Substring(iPosition + 46, 40).Trim()
        dRow("StandDescription") = PARAMOUT.Substring(iPosition + 86, 30).Trim()
        'area description will be overridden by roving seat
        dRow("AreaDescription") = PARAMOUT.Substring(iPosition + 116, 30).Trim()
        dRow("BandDescription") = PARAMOUT.Substring(iPosition + 146, 15).Trim()
        dRow("ReservedSeat") = PARAMOUT.Substring(iPosition + 162, 1).Trim()
        dRow("CurrentFulfilSlctn") = PARAMOUT.Substring(iPosition + 163, 1).Trim()
        dRow("ErrorCode") = PARAMOUT.Substring(iPosition + 165, 1)
        dRow("ProductLimit") = PARAMOUT.Substring(iPosition + 166, 5).Trim()
        dRow("UserLimit") = PARAMOUT.Substring(iPosition + 171, 5).Trim()
        dRow("ErrorInformation") = PARAMOUT.Substring(iPosition + 176, 40).Trim()
        If Utilities.convertToBool(PARAMOUT.Substring(iPosition + 602, 1)) Or PARAMOUT.Substring(iPosition + 36, 1).Trim() = GlobalConstants.SEASONTICKETPRODUCTTYPE Then
            dRow("ProductTime") = String.Empty
        Else
            dRow("ProductTime") = PARAMOUT.Substring(iPosition + 216, 7).Trim()
        End If
        dRow("ProductOppositionCode") = PARAMOUT.Substring(iPosition + 223, 4).Trim()
        dRow("ProductCompetitionCode") = PARAMOUT.Substring(iPosition + 227, 1).Trim()
        dRow("ProductCompetitionText") = PARAMOUT.Substring(iPosition + 228, 40).Trim()
        Try
            If Utilities.convertToBool(PARAMOUT.Substring(iPosition + 601, 1)) Or PARAMOUT.Substring(iPosition + 36, 1).Trim() = GlobalConstants.SEASONTICKETPRODUCTTYPE Then
                dRow("ProductDate") = String.Empty
            Else
                dRow("ProductDate") = Utilities.ISeriesDate(PARAMOUT.Substring(iPosition + 268, 7)).ToLongDateString
            End If
        Catch ex As Exception
            dRow("ProductDate") = "01\01\1900"
        End Try

        'Show date range for bundles
        If Not Utilities.CheckForDBNull_String(PARAMOUT.Substring(iPosition + 580, 7)).Trim Is String.Empty Then
            Try
                dRow("BundleStartDate") = Utilities.ISeriesDate(PARAMOUT.Substring(iPosition + 580, 7)).ToLongDateString
            Catch ex As Exception
                dRow("BundleStartDate") = ""
            End Try
            Try
                dRow("BundleEndDate") = Utilities.ISeriesDate(PARAMOUT.Substring(iPosition + 587, 7)).ToLongDateString
            Catch ex As Exception
                dRow("BundleEndDate") = ""
            End Try
        End If

        dRow("SeatRestriction") = PARAMOUT.Substring(iPosition + 275, 2)

        If Not String.IsNullOrEmpty(PARAMOUT.Substring(iPosition + 277, 1)) Then
            If UCase(PARAMOUT.Substring(iPosition + 277, 1)) = "Y" Then
                dRow("IsFree") = "True"
            Else
                dRow("IsFree") = "False"
            End If
        Else
            dRow("IsFree") = "False"
        End If

        'Product Sub Type
        If dRow("ProductType").ToString.Equals("P") Then
            dRow("ProductSubType") = PARAMOUT.Substring(iPosition + 278, 1).Trim()
        ElseIf backendProgramName = "WS609R" Then
            dRow("ProductSubType") = PARAMOUT.Substring(iPosition + 221, 4).Trim()
        ElseIf PARAMOUT.Length > 312 Then
            dRow("ProductSubType") = PARAMOUT.Substring(iPosition + 309, 4).Trim()
        Else
            dRow("ProductSubType") = ""
        End If

        dRow("OptPostFul") = PARAMOUT.Substring(iPosition + 279, 1).Trim()
        dRow("OptCollFul") = PARAMOUT.Substring(iPosition + 280, 1).Trim()
        dRow("OptPrintFul") = PARAMOUT.Substring(iPosition + 425, 1).Trim()
        dRow("OptP@HFul") = PARAMOUT.Substring(iPosition + 281, 1).Trim()
        dRow("OptSCUploadFul") = PARAMOUT.Substring(iPosition + 282, 1).Trim()
        dRow("ProductTypeActual") = PARAMOUT.Substring(iPosition + 283, 1).Trim()
        dRow("PackageID") = PARAMOUT.Substring(iPosition + 284, 13).Trim()
        dRow("VatValue") = Utilities.FormatPrice(PARAMOUT.Substring(iPosition + 300, 9)).Trim()
        dRow("IncludeTravel") = PARAMOUT.Substring(iPosition + 313, 1).Trim()
        dRow("OptRegPostFul") = PARAMOUT.Substring(iPosition + 314, 1).Trim()
        dRow("CanSaveAsFavouriteSeat") = Utilities.convertToBool(PARAMOUT.Substring(iPosition + 315, 1))
        If PARAMOUT.Substring(iPosition + 318, 1) = "Y" Then
            dRow("ReservedForCurrentLoginid") = True
        Else
            dRow("ReservedForCurrentLoginid") = False
        End If
        dRow("PromotionId1") = PARAMOUT.Substring(iPosition + 319, 13).Trim()
        dRow("PromotionId2") = PARAMOUT.Substring(iPosition + 535, 13).Trim()
        dRow("PromotionId3") = PARAMOUT.Substring(iPosition + 548, 13).Trim()
        dRow("OriginalPrice") = Utilities.FormatPrice(PARAMOUT.Substring(iPosition + 332, 15))
        'never trim cat seat details as it has an format includes spaces
        dRow("CATSeatDetails") = PARAMOUT.Substring(iPosition + 347, 40).Trim()
        dRow("PricedBnds") = PARAMOUT.Substring(iPosition + 387, 36).Trim()
        dRow("LinkedProductId") = LinkedProductId

        'charge fee 424
        If PARAMOUT.Substring(iPosition + 424, 1) = "Y" Then
            dRow("IsIncluded") = True
        Else
            dRow("IsIncluded") = False
        End If

        '425 print
        Dim feeCategory As String = PARAMOUT.Substring(iPosition + 426, 10).Trim
        dRow("FeeCategory") = feeCategory
        If feeCategory.Length > 0 Then
            dRow("IsSystemFee") = True
        Else
            dRow("IsSystemFee") = False
        End If
        dRow("IsExternal") = True

        dRow("CustomerAllocation") = PARAMOUT.Substring(iPosition + 479, 1).Trim()
        dRow("VoucherDefinitionId") = If(PARAMOUT.Substring(iPosition + 436, 13).Trim = String.Empty, 0, CType(PARAMOUT.Substring(iPosition + 436, 13), Long))
        dRow("VoucherCode") = PARAMOUT.Substring(iPosition + 449, 30).Trim()
        dRow("Unreserved") = PARAMOUT.Substring(iPosition + 480, 1).Trim()
        dRow("Roving") = PARAMOUT.Substring(iPosition + 481, 1).Trim()
        'override area description as per roving
        If dRow("Roving").ToString() = "Y" Then
            dRow("AreaDescription") = "ROVING"
        End If
        dRow("CannotApplyFees") = Not (PARAMOUT.Substring(iPosition + 482, 1).ConvertFromISeriesYesNoToBoolean)
        If (PARAMOUT.Substring(iPosition + 483, 1).Trim = "") OrElse (Not IsNumeric(PARAMOUT.Substring(iPosition + 483, 1).Trim)) Then
            dRow("CATQuantity") = "0"
        Else
            dRow("CATQuantity") = PARAMOUT.Substring(iPosition + 483, 7).Trim
        End If
        dRow("CATFulfilment") = PARAMOUT.Substring(iPosition + 490, 1).Trim
        dRow("financeClubProductID") = PARAMOUT.Substring(iPosition + 491, 3).Trim
        dRow("financePlanID") = PARAMOUT.Substring(iPosition + 494, 2).Trim
        dRow("BulkSalesID") = PARAMOUT.Substring(iPosition + 512, 13).Trim
        dRow("PackageType") = PARAMOUT.Substring(iPosition + 561, 1).Trim
        Dim allocatedSeat As New DESeatDetails
        allocatedSeat.UnFormattedSeat = PARAMOUT.Substring(iPosition + 562, 16) 'This field is not trimmed for a reason!
        dRow("AllocatedSeat") = allocatedSeat.FormattedSeat
        dRow("ReservationCode") = PARAMOUT.Substring(iPosition + 578, 2).Trim

        Dim price As String = String.Empty
        If dRow("BulkSalesID") > 0 Then
            dRow("Quantity") = PARAMOUT.Substring(iPosition + 496, 5).Trim
            price = PARAMOUT.Substring(iPosition + 501, 11).Trim
        Else
            dRow("Quantity") = PARAMOUT.Substring(iPosition + 596, 5).Trim
            price = PARAMOUT.Substring(iPosition + 37, 9)
        End If
        If PARAMOUT.Substring(iPosition + 423, 1) = "Y" Then
            dRow("IsNegativeFee") = True
            dRow("Price") = Utilities.FormatPrice(Utilities.ConvertStringToDecimal(price) * -1)
        Else
            dRow("IsNegativeFee") = False
            dRow("Price") = Utilities.FormatPrice(price)
        End If
        dRow("RestrictedBasketOptions") = Utilities.convertToBool(PARAMOUT.Substring(iPosition + 594, 1))
        dRow("DisplayInACancelBasket") = Utilities.convertToBool(PARAMOUT.Substring(iPosition + 595, 1))
        dRow("CallId") = Utilities.CheckForDBNull_BigInt(PARAMOUT.Substring(iPosition + 603, 13).Trim.TrimStart("0"))
        dRow("BasketHasExceptionSeats") = Utilities.convertToBool(PARAMOUT.Substring(4893, 1))
    End Sub

    Private Sub AddValuesToBasketHeaderTableRow(ByRef dRow As DataRow, ByVal PARAMOUT As String, ByVal sRecordTotal As String)
        dRow("MarketingCampaign") = PARAMOUT.Substring(4958, 1).Trim()
        dRow("UserSelectFulfilment") = PARAMOUT.Substring(4979, 1)
        dRow("ReservedQuantity") = sRecordTotal

        Dim PaymentOptions As New StringBuilder("")
        If PARAMOUT.Substring(5030, 1).ToUpper() = "Y" Then
            PaymentOptions.Append("CC")
            PaymentOptions.Append(",PP")
            PaymentOptions.Append(",GC")
        End If

        If PARAMOUT.Substring(5031, 1).ToUpper() = "Y" Then
            If PaymentOptions.Length > 0 Then
                PaymentOptions.Append(",")
            End If
            PaymentOptions.Append("DD")
        End If

        If PARAMOUT.Substring(5032, 1).ToUpper() = "Y" Then
            If PaymentOptions.Length > 0 Then
                PaymentOptions.Append(",")
            End If
            PaymentOptions.Append("CF")
        End If

        If PARAMOUT.Substring(5034, 1).ToUpper() = "Y" Then
            If PaymentOptions.Length > 0 Then
                PaymentOptions.Append(",")
            End If
            PaymentOptions.Append("EP")
        End If

        If PARAMOUT.Substring(5029, 1).ToUpper() = "Y" Then
            If PaymentOptions.Length > 0 Then
                PaymentOptions.Append(",")
            End If
            PaymentOptions.Append("IV")
        End If

        dRow("PaymentOptions") = PaymentOptions.ToString()

        If PARAMOUT.Substring(5033, 1).ToUpper() = "Y" Then
            dRow("RestrictPaymentOptions") = "Y"
        Else
            dRow("RestrictPaymentOptions") = "N"
        End If

        dRow("PaymentAccountId") = PARAMOUT.Substring(4918, 12).Trim 'Merchant ID
        dRow("PaymentExternalToken") = PARAMOUT.Substring(4980, 25).Trim 'Token or Transaction ID from external gateway such as PayPal
        dRow("CATMode") = PARAMOUT.Substring(4900, 1).Trim 'Cancel, Amend or Transfer
        dRow("CATPrice") = Utilities.FormatPrice(PARAMOUT.Substring(4901, 15))
        If PARAMOUT.Substring(4916, 1) = "Y" Then
            dRow("CATPrice") = dRow("CATPrice").ToString.Trim + "-"
        End If
        dRow("FeesExcluded") = PARAMOUT.Substring(4800, 90)
        dRow("OriginalSalePaidWithCF") = Utilities.convertToBool(PARAMOUT.Substring(5036, 1))
    End Sub

    Private Sub AddFeesToBasketDetailsTable(ByRef DtProductListResults As DataTable, ByVal PARAMOUTBasket As String)

        Dim dRow As DataRow = Nothing

        ' BKFEE
        If PARAMOUTBasket.Substring(4980, 1) = "Y" Then
            dRow = Nothing
            dRow = DtProductListResults.NewRow
            dRow("ProductCode") = GlobalConstants.BKFEE
            dRow("Price") = Utilities.FormatPrice(PARAMOUTBasket.Substring(4984, 9))
            If PARAMOUTBasket.Substring(4800, 1) = "Y" Then
                dRow("Price") = CDec(dRow("Price")) * (-1)
            End If
            DtProductListResults.Rows.Add(dRow)
        End If

        ' WSFEE
        If PARAMOUTBasket.Substring(4981, 1) = "Y" Then
            dRow = Nothing
            dRow = DtProductListResults.NewRow
            dRow("ProductCode") = GlobalConstants.WSFEE
            dRow("Price") = Utilities.FormatPrice(PARAMOUTBasket.Substring(4993, 9))
            If PARAMOUTBasket.Substring(4801, 1) = "Y" Then
                dRow("Price") = CDec(dRow("Price")) * (-1)
            End If
            DtProductListResults.Rows.Add(dRow)
        End If

        ' CRFEE
        If PARAMOUTBasket.Substring(4982, 1) = "Y" Then
            dRow = Nothing
            dRow = DtProductListResults.NewRow
            dRow("ProductCode") = GlobalConstants.CRFEE
            dRow("Price") = Utilities.FormatPrice(PARAMOUTBasket.Substring(5002, 9))
            If PARAMOUTBasket.Substring(4802, 1) = "Y" Then
                dRow("Price") = CDec(dRow("Price")) * (-1)
            End If
            DtProductListResults.Rows.Add(dRow)
        End If

        ' ATFEE
        If PARAMOUTBasket.Substring(4983, 1) = "Y" Then
            dRow = Nothing
            dRow = DtProductListResults.NewRow
            dRow("ProductCode") = GlobalConstants.ATFEE
            dRow("Price") = Utilities.FormatPrice(PARAMOUTBasket.Substring(5011, 9))
            If PARAMOUTBasket.Substring(4803, 1) = "Y" Then
                dRow("Price") = CDec(dRow("Price")) * (-1)
            End If
            DtProductListResults.Rows.Add(dRow)
        End If

        ' DDFEE
        If PARAMOUTBasket.Substring(4969, 1) = "Y" Then
            dRow = Nothing
            dRow = DtProductListResults.NewRow
            dRow("ProductCode") = GlobalConstants.DDFEE
            dRow("Price") = Utilities.FormatPrice(PARAMOUTBasket.Substring(4970, 9))
            If PARAMOUTBasket.Substring(4804, 1) = "Y" Then
                dRow("Price") = CDec(dRow("Price")) * (-1)
            End If
            DtProductListResults.Rows.Add(dRow)
        End If

        ' Buyback Reward
        If PARAMOUTBasket.Substring(4959, 1) = "Y" Then
            dRow = Nothing
            dRow = DtProductListResults.NewRow
            dRow("ProductCode") = GlobalConstants.BBFEE
            dRow("Price") = Utilities.FormatPrice(PARAMOUTBasket.Substring(4960, 9))
            DtProductListResults.Rows.Add(dRow)
        End If

        ' Registered Post Fees
        If PARAMOUTBasket.Substring(4936, 1) = "Y" Then
            dRow = Nothing
            dRow = DtProductListResults.NewRow
            dRow("ProductCode") = PARAMOUTBasket.Substring(4930, 6)
            dRow("Price") = Utilities.FormatPrice(PARAMOUTBasket.Substring(4937, 9))
            If PARAMOUTBasket.Substring(4805, 1) = "Y" Then
                dRow("Price") = CDec(dRow("Price")) * (-1)
            End If
            DtProductListResults.Rows.Add(dRow)
        End If

        ' CAT Fees
        If PARAMOUTBasket.Substring(4907, 1) = "Y" Then
            dRow = Nothing
            dRow = DtProductListResults.NewRow
            dRow("ProductCode") = PARAMOUTBasket.Substring(4901, 6)
            dRow("Price") = Utilities.FormatPrice(PARAMOUTBasket.Substring(4908, 9))
            DtProductListResults.Rows.Add(dRow)
        End If

        'CAT Refund Fee on cancellation
        If PARAMOUTBasket.Substring(4874, 1) = "Y" Then
            dRow = Nothing
            dRow = DtProductListResults.NewRow
            dRow("ProductCode") = PARAMOUTBasket.Substring(4868, 6)
            dRow("Price") = Utilities.FormatPrice(PARAMOUTBasket.Substring(4875, 9))
            If PARAMOUTBasket.Substring(4806, 1) = "Y" Then
                dRow("Price") = CDec(dRow("Price")) * (-1)
            End If
            DtProductListResults.Rows.Add(dRow)
        End If

    End Sub

#End Region

#Region "Public Functions"

    Public Sub RetrieveTicketingShoppingBasketValues(ByVal PARAMOUT As String, ByRef ResultDataSet As DataSet, ByRef err As ErrorObj, ByVal backendProgramName As String)
        Dim dRow As DataRow = Nothing
        Dim sLastRecord As String = "000"
        Dim sRecordTotal As String = "000"
        Dim bMoreRecords As Boolean = True

        'Loop until all the items reserved have been retrieved
        Do While bMoreRecords = True
            '
            'Call WS036R
            If String.IsNullOrEmpty(PARAMOUT) Then
                PARAMOUT = CallWS036R(sRecordTotal, sLastRecord)
            End If

            If PARAMOUT.ToString.Trim.Length < 274 AndAlso sLastRecord <> "000" Then
                bMoreRecords = False
            Else
                '
                ' Set the Status table indicating any errors
                dRow = Nothing
                dRow = ResultDataSet.Tables("BasketStatus").NewRow
                If PARAMOUT.TrimEnd.Length >= 5119 Then
                    dRow("ErrorOccurred") = "E"
                    dRow("ReturnCode") = PARAMOUT.Substring(5117, 2)
                    dRow("ValidationError") = ""
                    err.ErrorNumber = PARAMOUT.Substring(5117, 2)
                    err.HasError = True
                Else
                    dRow("ErrorOccurred") = ""
                    dRow("ReturnCode") = ""
                    dRow("ValidationError") = ""
                End If
                ResultDataSet.Tables("BasketStatus").Rows.Add(dRow)
                '
                'No errors 
                If Not err.HasError Then

                    'Extract the data from the parameter
                    Dim iPosition As Integer = 0
                    Dim iCounter As Integer = 1
                    Do While iCounter <= 6

                        ' Has a product been returned
                        If PARAMOUT.Substring(iPosition + 12, 6).Trim = "" Then
                            Exit Do
                        Else

                            'Create a new row
                            dRow = Nothing
                            dRow = ResultDataSet.Tables("BasketDetail").NewRow
                            AddValuesToBasketDetailsTableRow(dRow, PARAMOUT, iPosition, backendProgramName)
                            ResultDataSet.Tables("BasketDetail").Rows.Add(dRow)

                            'Increment
                            iPosition = iPosition + 800
                            iCounter = iCounter + 1

                        End If
                    Loop

                    'Extract the footer information
                    sLastRecord = PARAMOUT.Substring(5113, 3)
                    sRecordTotal = PARAMOUT.Substring(5110, 3)
                    ' If basket is empty these will be blank
                    If (Not sLastRecord.Trim = String.Empty) AndAlso (Not sRecordTotal.Trim = String.Empty) Then
                        If CInt(sLastRecord) >= CInt(sRecordTotal) Then
                            bMoreRecords = False

                            'Set the Header record
                            dRow = Nothing
                            dRow = ResultDataSet.Tables("BasketHeader").NewRow
                            AddValuesToBasketHeaderTableRow(dRow, PARAMOUT, sRecordTotal)
                            ResultDataSet.Tables("BasketHeader").Rows.Add(dRow)

                            ' add the fees as new records on the basket details table
                            AddFeesToBasketDetailsTable(ResultDataSet.Tables("BasketDetail"), PARAMOUT)

                        End If
                    Else
                        bMoreRecords = False
                    End If
                Else
                    bMoreRecords = False
                End If

            End If

            'We always want to call WS036R on the second call
            PARAMOUT = ""
        Loop

    End Sub

    Public Sub CreateBasketTables(ByRef ResultDataSet As DataSet)
        '
        'Create the status data table
        Dim DtStatusResults As New DataTable
        ResultDataSet.Tables.Add(DtStatusResults)
        AddColumnsToBasketStatusTable(DtStatusResults)
        '
        'Create the global variables data table
        Dim DtGlobalResults As New DataTable
        ResultDataSet.Tables.Add(DtGlobalResults)
        AddColumnsToBasketHeaderTable(DtGlobalResults)
        '
        'Create the Product List data table
        Dim DtProductListResults As New DataTable
        ResultDataSet.Tables.Add(DtProductListResults)
        AddColumnsToBasketDetailsTable(DtProductListResults)

        'Name the tables
        ResultDataSet.Tables(0).TableName = "BasketStatus"
        ResultDataSet.Tables(1).TableName = "BasketHeader"
        ResultDataSet.Tables(2).TableName = "BasketDetail"

    End Sub

#End Region

End Class