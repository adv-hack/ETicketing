Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
Imports System.Text.RegularExpressions
Imports Talent.Common
Imports Talent.Common.Utilities
Imports Talent.Common.UtilityExtension
Imports System.Linq

<Serializable()> _
Public Class DBPackage
    Inherits DBAccess

    Private Const CLASSNAME As String = "DBPACKAGE"

#Region "Class Level Fields"

    Private _cmdAdapter As iDB2DataAdapter = Nothing
    Private _cmd As iDB2Command = Nothing

#End Region

#Region "Public Properties"

    Public Property DePack() As DEPackages
    Public Property WS036RFirstParm() As String = String.Empty
    Const strError As String = "Error during database access"

#End Region

#Region "Protected Methods"

    Protected Overrides Function AccessDataBaseTALENTCRM() As ErrorObj
        Return New ErrorObj
    End Function

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Return New ErrorObj
    End Function

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = "UpdateCustomerComponentDetails" : err = AccessDatabaseWS622R()
            Case Is = "DeleteCustomerPackage" : err = AccessDatabaseWS622R()
            Case Is = "GetCustomerPackageInformation" : err = AccessDatabaseWS074R()
            Case Is = "GetComponentList" : err = AccessDatabaseWS076R()
            Case Is = "GetComponentGroupList" : err = AccessDatabaseWS077R()
            Case Is = "GetComponentDetails" : err = AccessDatabaseWS078R()
            Case Is = "GetComponentGroupDetails" : err = AccessDatabaseWS079R()
            Case Is = "AddEditDeleteComponentGroup" : err = AccessDatabaseWS080R()
            Case Is = "AddEditDeleteComponent" : err = AccessDatabaseWS081R()
            Case Is = "UpdateTravelAndAccomodationComponentDetails" : err = AccessDatabaseWS623R()
            Case Is = "GetComponentSeats" : err = AccessDatabaseWS099S()
            Case Is = "GetPackageSeatDetails" : err = AccessDatabaseWS074S()
            Case Is = "GetHospitalityBookings" : err = AccessDatabaseCS001S()
            Case Is = "UpdateHospitalityBookingStatus" : err = AccessDatabaseCS002S()
            Case Is = "GetSoldHospitalityBookingDetails" : err = AccessDatabaseCS003S()
            Case Is = "PrintHospitalityBookings" : err = AccessDatabaseWS696R()
            Case Is = "CreateHospitalityBookingDocument" : err = AccessDatabaseWS640R()
        End Select

        'logging for testing purpose
        If err.HasError Then

        End If
        Return err
    End Function

#End Region

    Private Function WS072RParm(ByRef amendComponentsItemNo As Integer, ByRef seatAllocationItemNo As Integer) As String
        Dim myString As New StringBuilder
        Dim counter As Integer = 0
        Dim iNoOfRecordsMissing As Integer = 0

        'Header Information
        Dim PackageDiscountedByValuePence As Integer = DePack.PackageDiscountedByValue * 100
        myString.Append(Utilities.PadLeadingZeros(DePack.PackageID, 13))
        myString.Append(Utilities.FixStringLength(DePack.ProductCode, 6))
        myString.Append(Utilities.FixStringLength(DePack.BasketId, 36))
        myString.Append(Utilities.FixStringLength(DePack.BoxOfficeUser, 10))
        myString.Append(Utilities.FixStringLength(DePack.Mode.GetISeriesOperationMode(), 1))
        myString.Append(Utilities.PadLeadingZeros(DePack.ComponentGroupID, 13))
        myString.Append(Utilities.PadLeadingZeros(DePack.ComponentID, 13))
        myString.Append(DePack.MarkAsCompleted.ConvertToISeriesYesNo, 1)
        myString.Append(Utilities.PadLeadingZerosDec(DePack.Discount, 5))
        myString.Append(DePack.UpdateDiscount.ConvertToISeriesYesNo, 1)
        myString.Append(Utilities.PadLeadingZeros(DePack.LeadSourceID, 13))
        myString.Append(Utilities.PadLeadingZeros(PackageDiscountedByValuePence, 9))
        myString.Append(DePack.RemoveAllDiscounts.ConvertToISeriesYesNo, 1)
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.FixStringLength(DePack.MarkOrderFor, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 375))

        'Component Items
        Do While counter < 100 AndAlso amendComponentsItemNo < DePack.AmendComponents.Count
            Dim item As AmendComponent = DePack.AmendComponents(amendComponentsItemNo)
            myString.Append(Utilities.PadLeadingZeros(item.ComponentId, 13))
            myString.Append(Utilities.PadLeadingZeros(item.ComponentGroupId, 13))
            myString.Append(Utilities.PadLeadingZeros(item.Quantity, 6))
            myString.Append(Utilities.PadLeadingZerosDec(item.Discount, 5))
            myString.Append(Utilities.FixStringLength(String.Empty, 3))
            counter += 1
            amendComponentsItemNo += 1
        Loop

        'If there are less than 100 ComponentArray items, then add zeroes to the integer type and spaces to string type
        'for each one of the non existent items.
        iNoOfRecordsMissing = 100 - counter
        For i As Integer = 1 To iNoOfRecordsMissing
            myString.Append(Utilities.PadLeadingZeros(String.Empty, 35))
            myString.Append(Utilities.FixStringLength(String.Empty, 5))
        Next

        'Seat Items
        counter = 0
        Do While counter < 100 AndAlso seatAllocationItemNo < DePack.SeatAllocations.Count
            Dim item As SeatAllocation = DePack.SeatAllocations(seatAllocationItemNo)
            myString.Append(Utilities.PadLeadingZeros(item.ComponentId, 13))
            myString.Append(Utilities.FixStringLength(item.Seat, 15))
            myString.Append(Utilities.FixStringLength(item.AlphaSuffix, 1))
            myString.Append(Utilities.FixStringLength(item.PriceBand, 1))
            myString.Append(Utilities.FixStringLength(item.PriceCode, 2))
            myString.Append(Utilities.PadLeadingZeros(item.CustomerNumber, 12))
            myString.Append(Utilities.FixStringLength(item.Action.GetISeriesOperationMode(), 1))
            myString.Append(Utilities.FixStringLength(String.Empty, 2))     'Seat Error
            myString.Append(Utilities.FixStringLength(String.Empty, 1))     'Roving
            myString.Append(Utilities.PadLeadingZeros(item.BulkId, 13))
            myString.Append(Utilities.FixStringLength(String.Empty, 39))     'Spare
            counter += 1
            seatAllocationItemNo += 1
        Loop

        'If there are less than 100 SeatArray items, then add zeroes to the integer type and spaces to string type
        'for each one of the non existent items.
        iNoOfRecordsMissing = 0
        iNoOfRecordsMissing = 100 - counter
        For i As Integer = 1 To iNoOfRecordsMissing
            myString.Append(Utilities.PadLeadingZeros(String.Empty, 13))
            myString.Append(Utilities.FixStringLength(String.Empty, 19))
            myString.Append(Utilities.PadLeadingZeros(String.Empty, 12))
            myString.Append(Utilities.FixStringLength(String.Empty, 4))
            myString.Append(Utilities.PadLeadingZeros(String.Empty, 13))
            myString.Append(Utilities.FixStringLength(String.Empty, 39))
        Next

        myString.Append(Utilities.FixStringLength(String.Empty, 725))
        myString.Append(Utilities.PadLeadingZeros(Settings.LoginId, 12))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))
        Return myString.ToString()
    End Function

    Protected Function AccessDatabaseWS622R() As ErrorObj
        Dim err As New ErrorObj
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        Dim PARAMOUT3 As String = String.Empty
        Dim PARAMOUT4 As String = String.Empty
        Dim PARAMOUT5 As String = String.Empty
        Dim PARAMOUT6 As String = String.Empty
        Dim amendComponentItemNo As Integer = 0
        Dim seatAllocationItemNo As Integer = 0
        Dim moreRecords As Boolean = True

        Try
            Do While moreRecords
                PARAMOUT = CallWS622R(PARAMOUT2, PARAMOUT3, PARAMOUT4, PARAMOUT5, PARAMOUT6, amendComponentItemNo, seatAllocationItemNo)
                If PARAMOUT.Substring(15239, 1) = GlobalConstants.ERRORFLAG OrElse PARAMOUT.Substring(15237, 2).Trim <> String.Empty Then
                    'if an error has occurred leave the loop and process the records
                    moreRecords = False
                Else
                    If amendComponentItemNo = DePack.AmendComponents.Count AndAlso seatAllocationItemNo = DePack.SeatAllocations.Count Then
                        moreRecords = False
                    End If
                End If
            Loop

            'Set the basket parm
            WS036RFirstParm = PARAMOUT4

            'Was the call t0 WS072R succesful? 
            If PARAMOUT.Substring(15239, 1) = GlobalConstants.ERRORFLAG OrElse PARAMOUT.Substring(15237, 2).Trim <> String.Empty Then
                ResultDataSet = New DataSet
                Dim DtStatusResults As New DataTable("StatusResults")
                ResultDataSet.Tables.Add(DtStatusResults)
                With DtStatusResults.Columns
                    .Add("ErrorOccurred", GetType(String))
                    .Add("ReturnCode", GetType(String))
                End With
                Dim dRow As DataRow = Nothing
                dRow = DtStatusResults.NewRow
                dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                dRow("ReturnCode") = PARAMOUT.Substring(15237, 2).Trim
                DtStatusResults.Rows.Add(dRow)

                Dim DtPackage As New DataTable("Package")
                Dim DtComponent As New DataTable("Component")
                Dim DtSeat As New DataTable("Seat")
                Dim DtComponentAmendments As New DataTable("ComponentAmendments")
                Dim DtExtraComponents As New DataTable("ExtraComponents")
                ResultDataSet.Tables.Add(DtPackage)
                ResultDataSet.Tables.Add(DtComponent)
                ResultDataSet.Tables.Add(DtSeat)
                ResultDataSet.Tables.Add(DtComponentAmendments)
                ResultDataSet.Tables.Add(DtExtraComponents)
            Else
                'Process the package
                AccessDatabaseWS074R(PARAMOUT2, PARAMOUT3, PARAMOUT5, PARAMOUT6)
            End If
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPK-WS622R"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS622R", strError, err, ex, LogTypeConstants.TCBMPACKAGELOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Function CallWS622R(ByRef PARAMOUT2 As String, ByRef PARAMOUT3 As String, ByRef PARAMOUT4 As String, ByRef PARAMOUT5 As String, ByRef PARAMOUT6 As String, ByRef amendComponentsItemNo As Integer, ByRef seatAllocationItemNo As Integer) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim parmIO3 As iDB2Parameter
        Dim parmIO4 As iDB2Parameter
        Dim parmIO5 As iDB2Parameter
        Dim parmIO6 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS622R (@PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5,@PARAM6)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 15240)
        parmIO.Value = WS072RParm(amendComponentsItemNo, seatAllocationItemNo)
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add("@PARAM2", iDB2DbType.iDB2Char, 15240)
        parmIO2.Value = WS074RParm(String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty)
        parmIO2.Direction = ParameterDirection.InputOutput

        parmIO3 = cmdSELECT.Parameters.Add("@PARAM3", iDB2DbType.iDB2Char, 30000)
        parmIO3.Value = String.Empty
        parmIO3.Direction = ParameterDirection.InputOutput

        parmIO4 = cmdSELECT.Parameters.Add("@PARAM4", iDB2DbType.iDB2Char, 5120)
        parmIO4.Value = String.Empty
        parmIO4.Direction = ParameterDirection.InputOutput

        parmIO5 = cmdSELECT.Parameters.Add("@PARAM5", iDB2DbType.iDB2Char, 5000)
        parmIO5.Value = String.Empty
        parmIO5.Direction = ParameterDirection.InputOutput

        parmIO6 = cmdSELECT.Parameters.Add("@PARAM6", iDB2DbType.iDB2Char, 5000)
        parmIO6.Value = String.Empty
        parmIO6.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters("@PARAM2").Value.ToString
        PARAMOUT3 = cmdSELECT.Parameters("@PARAM3").Value.ToString
        PARAMOUT4 = cmdSELECT.Parameters("@PARAM4").Value.ToString
        PARAMOUT5 = cmdSELECT.Parameters("@PARAM5").Value.ToString
        PARAMOUT6 = cmdSELECT.Parameters("@PARAM6").Value.ToString

        Return PARAMOUT
    End Function

    Protected Function AccessDatabaseWS623R() As ErrorObj

        Dim err As New ErrorObj
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        Dim PARAMOUT3 As String = String.Empty
        Dim PARAMOUT4 As String = String.Empty
        Dim PARAMOUT5 As String = String.Empty

        Try
            '
            'Call WS623R
            PARAMOUT = CallWS623R(PARAMOUT2, PARAMOUT3, PARAMOUT4, PARAMOUT5)

            'Set the basket parm
            WS036RFirstParm = PARAMOUT4

            'Was the call to WS073R succesful? 
            If PARAMOUT.Substring(1023, 1) = "E" OrElse PARAMOUT.Substring(1021, 2).Trim <> String.Empty Then

                'Create the Status data table
                ResultDataSet = New DataSet
                Dim DtStatusResults As New DataTable("StatusResults")
                ResultDataSet.Tables.Add(DtStatusResults)
                With DtStatusResults.Columns
                    .Add("ErrorOccurred", GetType(String))
                    .Add("ReturnCode", GetType(String))
                End With

                Dim dRow As DataRow = Nothing
                dRow = DtStatusResults.NewRow
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2).Trim
                DtStatusResults.Rows.Add(dRow)
            Else
                'Process the package
                AccessDatabaseWS074R(PARAMOUT2, PARAMOUT3, PARAMOUT5)
            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPK-WS622R"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS623R", strError, err, ex, LogTypeConstants.TCBMPACKAGELOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try

        Return err

    End Function

    Private Function CallWS623R(ByRef PARAMOUT2 As String, ByRef PARAMOUT3 As String, ByRef PARAMOUT4 As String, ByRef PARAMOUT5 As String) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim parmIO3 As iDB2Parameter
        Dim parmIO4 As iDB2Parameter
        Dim parmIO5 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS623R (@PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS073RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add("@PARAM2", iDB2DbType.iDB2Char, 15240)
        parmIO2.Value = WS074RParm("", "", "", "", "", "")
        parmIO2.Direction = ParameterDirection.InputOutput

        parmIO3 = cmdSELECT.Parameters.Add("@PARAM3", iDB2DbType.iDB2Char, 30000)
        parmIO3.Value = " "
        parmIO3.Direction = ParameterDirection.InputOutput

        parmIO4 = cmdSELECT.Parameters.Add("@PARAM4", iDB2DbType.iDB2Char, 5120)
        parmIO4.Value = " "
        parmIO4.Direction = ParameterDirection.InputOutput

        parmIO5 = cmdSELECT.Parameters.Add("@PARAM5", iDB2DbType.iDB2Char, 5000)
        parmIO5.Value = " "
        parmIO5.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters("@PARAM2").Value.ToString
        PARAMOUT3 = cmdSELECT.Parameters("@PARAM3").Value.ToString
        PARAMOUT4 = cmdSELECT.Parameters("@PARAM4").Value.ToString
        PARAMOUT5 = cmdSELECT.Parameters("@PARAM5").Value.ToString

        Return PARAMOUT
    End Function

    Private Function AccessDatabaseWS073R() As ErrorObj

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

            PARAMOUT = CallWS073R()
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" OrElse PARAMOUT.Substring(1021, 2).Trim <> String.Empty Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2).Trim
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)


        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS073R-01"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS073R", strError, err, ex, LogTypeConstants.TCBMPACKAGELOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Function CallWS073R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS073R (@PARAM1)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS073RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString


        Return PARAMOUT
    End Function

    Private Function WS073RParm() As String
        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.PadLeadingZeros(DePack.PackageID, 13))
        myString.Append(Utilities.PadLeadingZeros(DePack.ComponentID, 13))
        myString.Append(Utilities.FixStringLength(DePack.ProductCode, 6))
        myString.Append(Utilities.FixStringLength(DePack.BasketId, 36))
        myString.Append(Utilities.FixStringLength(DePack.BoxOfficeUser, 10))

        'We are not perfroming any functions in proceed mode
        If DePack.Proceed Then
            myString.Append(Utilities.FixStringLength("", 267))
        Else
            myString.Append(Utilities.PadLeadingZeros(DateToIseriesFormat(DePack.Fromdate), 7))
            myString.Append(Utilities.PadLeadingZeros(DateToIseriesFormat(DePack.ToDate), 7))
            myString.Append(Utilities.FixStringLength(DePack.StandCode, 3))

            'there would be 10 records for area- 9 for OtherTAComponentAreas and 1 for TAComponentArea2
            If DePack.OtherTAComponentAreas.Count > 0 Then
                myString.Append(Utilities.FixStringLength(DePack.OtherTAComponentAreas(0).Area, 4))
                myString.Append(Utilities.FixStringLength(DePack.OtherTAComponentAreas(0).PriceBandAndQuantity.PriceBand, 1))
                myString.Append(Utilities.PadLeadingZeros(DePack.OtherTAComponentAreas(0).PriceBandAndQuantity.Quantity, 5))
            Else
                myString.Append(Utilities.FixStringLength("", 5))
                myString.Append(Utilities.PadLeadingZeros(0, 5))
            End If

            'Price Bands
            myString.Append(Utilities.FixStringLength(DePack.TAComponentArea2.Area, 4))
            For Each item As PriceBandAndQuantity In DePack.TAComponentArea2.PriceBandAndQuantities
                myString.Append(Utilities.FixStringLength(item.PriceBand, 1))
                myString.Append(Utilities.PadLeadingZeros(item.Quantity, 5))
            Next
            'If there are less than 26 Price Band and Quantity items, then add zeroes to the integer type and spaces to string type
            'for each one of the non existent items.
            Dim iNoOfRecordsMissing As Integer = 0
            If DePack.TAComponentArea2.PriceBandAndQuantities.Count < 26 Then
                iNoOfRecordsMissing = 26 - DePack.TAComponentArea2.PriceBandAndQuantities.Count
                For i As Integer = 1 To iNoOfRecordsMissing
                    myString.Append(Utilities.FixStringLength(String.Empty, 1))
                    myString.Append(Utilities.PadLeadingZeros(String.Empty, 5))
                Next
            End If

            ' The first OtherTAComponentAreas list has already been added to the input string so loop through lists 2 to 9
            For Each item As OtherTAComponentArea In DePack.OtherTAComponentAreas.Where(Function(a, b) b <> 0)
                myString.Append(Utilities.FixStringLength(item.Area, 4))
                myString.Append(Utilities.FixStringLength(item.PriceBandAndQuantity.PriceBand, 1))
                myString.Append(Utilities.PadLeadingZeros(item.PriceBandAndQuantity.Quantity, 5))
            Next
            'If there are less than 9 OtherTAComponentArea items, then add zeroes to the integer type and spaces to string type
            'for each one of the non existent items.
            iNoOfRecordsMissing = 0
            If DePack.OtherTAComponentAreas.Count < 9 Then
                If DePack.OtherTAComponentAreas.Count = 0 Then
                    iNoOfRecordsMissing = 8
                Else
                    iNoOfRecordsMissing = 9 - DePack.OtherTAComponentAreas.Count
                End If
                For i As Integer = 1 To iNoOfRecordsMissing
                    myString.Append(Utilities.FixStringLength(String.Empty, 4))
                    myString.Append(Utilities.FixStringLength(String.Empty, 1))
                    myString.Append(Utilities.PadLeadingZeros(String.Empty, 5))
                Next
            End If
        End If


        myString.Append(Utilities.FixStringLength(DePack.MarkAsCompleted.ConvertToISeriesYesNo(), 1))
        myString.Append(Utilities.PadLeadingZeros(DePack.ComponentGroupID, 13))
        myString.Append(Utilities.FixStringLength(DePack.Proceed.ConvertToISeriesYesNo(), 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 648))
        myString.Append(Utilities.PadLeadingZeros(Settings.LoginId, 12))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))

        Return myString.ToString()

    End Function

    Private Function AccessDatabaseWS074R(Optional ByVal parm1 As String = "", Optional ByVal parm2 As String = "", Optional ByVal parm3 As String = "", Optional ByVal parm4 As String = "") As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim PARAMOUT2 As String = String.Empty
        Dim PARAMOUT3 As String = String.Empty
        Dim PARAMOUT4 As String = String.Empty
        Dim moreRecords As Boolean = True
        Dim firstCall As Boolean = True
        Dim NextChangedComponentId As String = String.Empty
        Dim NextComponentId As String = String.Empty
        Dim NextSeat As String = String.Empty
        Dim NextAlpha As String = String.Empty
        Dim NextProduct As String = String.Empty
        Dim NextComponentGroupId As String = String.Empty
        HospitalityBookingDataSet(ResultDataSet)

        Try

            Do While moreRecords

                If String.IsNullOrWhiteSpace(parm1) Then
                    PARAMOUT = CallWS074R(NextComponentId, NextChangedComponentId, NextSeat, NextAlpha, NextProduct, NextComponentGroupId, PARAMOUT2, PARAMOUT3, PARAMOUT4)
                Else
                    PARAMOUT = parm1
                    PARAMOUT2 = parm2
                    PARAMOUT3 = parm3
                    PARAMOUT4 = parm4
                    parm1 = String.Empty
                    parm2 = String.Empty
                End If

                If firstCall Then
                    dRow = Nothing
                    dRow = ResultDataSet.Tables("StatusResults").NewRow
                    If PARAMOUT.Substring(15239, 1) = "E" OrElse PARAMOUT.Substring(15237, 2).Trim <> String.Empty Then
                        If PARAMOUT.Substring(15237, 2).Trim = "WF" Then
                            dRow("ErrorOccurred") = ""
                            dRow("ReturnCode") = ""
                        Else
                            dRow("ErrorOccurred") = "E"
                            dRow("ReturnCode") = PARAMOUT.Substring(15237, 2).Trim
                        End If
                        moreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    ResultDataSet.Tables("StatusResults").Rows.Add(dRow)
                End If

                If moreRecords Then

                    'Loop for PARAM
                    Dim iPosition As Integer = 0

                    If firstCall Then
                        firstCall = False
                        dRow = Nothing
                        dRow = ResultDataSet.Tables("Package").NewRow
                        dRow("PackageID") = If(String.IsNullOrEmpty(PARAMOUT.Substring(iPosition, 13).Trim()), 0, Long.Parse(PARAMOUT.Substring(iPosition, 13).Trim()))
                        dRow("PackageDescription") = PARAMOUT.Substring(iPosition + 13, 30).Trim()
                        dRow("PackageType") = PARAMOUT.Substring(iPosition + 43, 1).Trim()
                        dRow("Discount") = Convert.ToDecimal(PARAMOUT.Substring(iPosition + 44, 3) & "." & PARAMOUT.Substring(iPosition + 47, 2))
                        dRow("Quantity") = PARAMOUT.Substring(iPosition + 49, 6).Trim()
                        dRow("PriceBeforeVAT") = Convert.ToDecimal(PARAMOUT.Substring(iPosition + 55, 15).Trim()) / 100
                        dRow("VATPrice") = Convert.ToDecimal(PARAMOUT.Substring(iPosition + 70, 15).Trim()) / 100
                        dRow("PriceIncludingVAT") = Convert.ToDecimal(PARAMOUT.Substring(iPosition + 85, 16).Trim()) / 100
                        dRow("Completed") = PARAMOUT.Substring(iPosition + 101, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("ErrorCode") = PARAMOUT.Substring(iPosition + 102, 2).Trim()
                        dRow("ErrorDescription") = PARAMOUT.Substring(iPosition + 104, 40).Trim()
                        dRow("ActiveFlag") = PARAMOUT.Substring(iPosition + 144, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("AllowComments") = PARAMOUT.Substring(iPosition + 147, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("PricingMethod") = PARAMOUT.Substring(iPosition + 148, 1).Trim()
                        dRow("LeadSourceID") = PARAMOUT.Substring(iPosition + 149, 13).Trim()
                        dRow("CallID") = PARAMOUT.Substring(iPosition + 162, 13).Trim()
                        dRow("PackageDiscountedByValue") = Utilities.FormatPrice(PARAMOUT.Substring(iPosition + 176, 9).Trim())
                        dRow("PackageDiscountRemovedDueToPriceRecalc") = PARAMOUT.Substring(iPosition + 185, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("PackageComponentLevelDiscountValue") = Utilities.FormatPrice(PARAMOUT.Substring(iPosition + 186, 9).Trim())
                        dRow("MarkOrderFor") = PARAMOUT.Substring(iPosition + 195, 1).Trim()
                        dRow("CatMode") = PARAMOUT.Substring(iPosition + 196, 1).Trim()
                        dRow("TemplateID") = CheckForDBNull_Decimal(PARAMOUT.Substring(iPosition + 197, 14).Trim())
                        dRow("ExpandAccordion") = PARAMOUT.Substring(iPosition + 211, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("BookingCustomerNumber") = PARAMOUT.Substring(iPosition + 212, 13).Trim()
                        dRow("BookingStatus") = PARAMOUT.Substring(iPosition + 226, 1).Trim()

                        'Are the prices negative?
                        If PARAMOUT.Substring(145, 1) = "Y" Then
                            If dRow("PriceBeforeVAT").Trim <> "0.00" Then dRow("PriceBeforeVAT") = "-" & dRow("PriceBeforeVAT").ToString.Trim
                            If dRow("VATPrice").Trim <> "0.00" Then dRow("VATPrice") = "-" & dRow("VATPrice").ToString.Trim
                            If dRow("PriceIncludingVAT").Trim <> "0.00" Then dRow("PriceIncludingVAT") = "-" & dRow("PriceIncludingVAT").ToString.Trim
                        End If

                        ResultDataSet.Tables("Package").Rows.Add(dRow)
                        iPosition = iPosition + 500
                    End If


                    iPosition = 500
                    Do While iPosition < 13000 AndAlso Not String.IsNullOrEmpty(PARAMOUT.Substring(iPosition, 13).Trim())

                        dRow = Nothing
                        dRow = ResultDataSet.Tables("Component").NewRow
                        dRow("ComponentID") = CLng(PARAMOUT.Substring(iPosition, 13))
                        dRow("ComponentDescription") = PARAMOUT.Substring(iPosition + 13, 30).Trim()
                        dRow("Type") = PARAMOUT.Substring(iPosition + 43, 1).Trim()
                        dRow("Discount") = Convert.ToDecimal(PARAMOUT.Substring(iPosition + 44, 3) & "." & PARAMOUT.Substring(iPosition + 47, 2))
                        dRow("Quantity") = PARAMOUT.Substring(iPosition + 49, 6).Trim()
                        dRow("PriceBeforeVAT") = Utilities.FormatPrice(PARAMOUT.Substring(iPosition + 55, 15).Trim())
                        dRow("VATPrice") = Utilities.FormatPrice(PARAMOUT.Substring(iPosition + 70, 15).Trim())
                        dRow("PriceIncludingVAT") = Utilities.FormatPrice(PARAMOUT.Substring(iPosition + 85, 16).Trim())
                        dRow("Completed") = PARAMOUT.Substring(iPosition + 101, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("ErrorCode") = PARAMOUT.Substring(iPosition + 102, 2).Trim()
                        dRow("ErrorDescription") = PARAMOUT.Substring(iPosition + 104, 40).Trim()
                        dRow("ActiveFlag") = PARAMOUT.Substring(iPosition + 144, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("AllocateAutomatically") = PARAMOUT.Substring(iPosition + 145, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("ComponentGroupSequence") = PARAMOUT.Substring(iPosition + 146, 3).Trim()
                        dRow("ComponentSequence") = PARAMOUT.Substring(iPosition + 149, 3).Trim()
                        dRow("ComponentGroupId") = PARAMOUT.Substring(iPosition + 152, 13).Trim()
                        dRow("ComponentType") = PARAMOUT.Substring(iPosition + 165, 1).Trim()
                        dRow("ComponentGroupType") = PARAMOUT.Substring(iPosition + 166, 2).Trim()
                        dRow("ProductCode") = PARAMOUT.Substring(iPosition + 168, 6).Trim()
                        dRow("AreaInError") = PARAMOUT.Substring(iPosition + 174, 4).Trim()
                        dRow("Proceed") = PARAMOUT.Substring(iPosition + 178, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("MinQty") = PARAMOUT.Substring(iPosition + 179, 7).Trim()
                        dRow("MaxQty") = PARAMOUT.Substring(iPosition + 186, 7).Trim()
                        dRow("FromDate") = ISeriesDate(PARAMOUT.Substring(iPosition + 193, 7).Trim())
                        dRow("ToDate") = ISeriesDate(PARAMOUT.Substring(iPosition + 200, 7).Trim())
                        dRow("CanAmendSeat") = Utilities.CheckForDBNull_Boolean_DefaultFalse(PARAMOUT.Substring(iPosition + 208, 1).Trim())
                        dRow("MaxDiscountPercent") = Utilities.ConvertStringToDecimal(PARAMOUT.Substring(iPosition + 209, 5).Trim()) / 100
                        dRow("DiscountValue") = Utilities.ConvertStringToDecimal(PARAMOUT.Substring(iPosition + 214, 9).Trim()) / 100
                        dRow("UnitPrice") = Utilities.FormatPrice(PARAMOUT.Substring(iPosition + 223, 15).Trim())
                        dRow("PriceBeforeVATExclDisc") = dRow("UnitPrice") * dRow("Quantity")
                        dRow("IsExtraComponent") = PARAMOUT.Substring(iPosition + 238, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("HideSeatForPWS") = convertToBool(PARAMOUT.Substring(iPosition + 239, 1).Trim())

                        'Are the prices negative?
                        If PARAMOUT.Substring(iPosition + 207, 1) = "Y" Then
                            If dRow("PriceBeforeVAT").Trim <> "0.00" Then dRow("PriceBeforeVAT") = "-" & dRow("PriceBeforeVAT").ToString.Trim
                            If dRow("PriceBeforeVATExclDisc").Trim <> "0.00" Then dRow("PriceBeforeVATExclDisc") = "-" & dRow("PriceBeforeVATExclDisc").ToString.Trim
                            If dRow("VATPrice").Trim <> "0.00" Then dRow("VATPrice") = "-" & dRow("VATPrice").ToString.Trim
                            If dRow("PriceIncludingVAT").Trim <> "0.00" Then dRow("PriceIncludingVAT") = "-" & dRow("PriceIncludingVAT").ToString.Trim
                        End If

                        ResultDataSet.Tables("Component").Rows.Add(dRow)
                        iPosition = iPosition + 250
                    Loop

                    iPosition = 0
                    Do While iPosition < 30000 AndAlso Not String.IsNullOrEmpty(PARAMOUT2.Substring(iPosition, 13).Trim())

                        dRow = Nothing
                        dRow = ResultDataSet.Tables("Seat").NewRow
                        dRow("ComponentID") = CLng(PARAMOUT2.Substring(iPosition, 13))
                        dRow("ProductCode") = PARAMOUT2.Substring(iPosition + 13, 6).Trim()
                        dRow("SeatDetails") = PARAMOUT2.Substring(iPosition + 19, 15)
                        dRow("AlphaSuffix") = PARAMOUT2.Substring(iPosition + 34, 1).Trim()
                        dRow("CustomerNumber") = PARAMOUT2.Substring(iPosition + 35, 12).Trim()
                        dRow("PriceBand") = PARAMOUT2.Substring(iPosition + 47, 1).Trim()
                        dRow("Price") = Utilities.FormatPrice(PARAMOUT2.Substring(iPosition + 48, 9).Trim())
                        dRow("ValidPriceBands") = PARAMOUT2.Substring(iPosition + 58, 36).Trim()
                        dRow("PriceCode") = PARAMOUT2.Substring(iPosition + 94, 2).Trim()
                        dRow("ErrorCode") = PARAMOUT2.Substring(iPosition + 97, 1).Trim()
                        dRow("MaxLimit") = PARAMOUT2.Substring(iPosition + 98, 5).Trim()
                        dRow("UserLimit") = PARAMOUT2.Substring(iPosition + 103, 5).Trim()
                        dRow("ErrorInformation") = PARAMOUT2.Substring(iPosition + 108, 40).Trim()
                        dRow("RovingOrUnreserved") = PARAMOUT2.Substring(iPosition + 148, 1).Trim()
                        dRow("BulkQuantity") = Utilities.CheckForDBNull_Int(PARAMOUT2.Substring(iPosition + 149, 5).Trim())
                        dRow("BulkID") = Utilities.CheckForDBNull_BigInt(PARAMOUT2.Substring(iPosition + 154, 13).Trim())
                        dRow("CanAmendSeat") = Utilities.CheckForDBNull_Boolean_DefaultFalse(PARAMOUT2.Substring(iPosition + 167, 1).Trim())
                        dRow("DefaultProductPriceBand") = PARAMOUT2.Substring(iPosition + 168, 1).Trim()

                        'Is the price negative?
                        If PARAMOUT2.Substring(iPosition + 96, 1) = "Y" Then
                            If dRow("Price").Trim <> "0.00" Then dRow("Price") = "-" & dRow("Price").ToString.Trim
                        End If

                        ResultDataSet.Tables("Seat").Rows.Add(dRow)
                        iPosition = iPosition + 200
                    Loop

                    iPosition = 0

                    Do While iPosition < 5000 AndAlso Not String.IsNullOrWhiteSpace(PARAMOUT3.Substring(iPosition + 40, 30).Trim())

                        dRow = Nothing
                        dRow = ResultDataSet.Tables("ComponentAmendments").NewRow

                        dRow("ComponentDescription") = PARAMOUT3.Substring(iPosition, 30).Trim()
                        dRow("TotalPrice") = Utilities.FormatPrice(PARAMOUT3.Substring(iPosition + 30, 10))
                        If PARAMOUT3.Substring(iPosition + 40, 1) = "Y" Then
                            If dRow("TotalPrice").Trim <> "0.00" Then dRow("TotalPrice") = "-" & dRow("TotalPrice").ToString.Trim
                        End If
                        dRow("QuantityAmended") = PARAMOUT3.Substring(iPosition + 41, 3)
                        dRow("QuantityAdded") = PARAMOUT3.Substring(iPosition + 44, 3)
                        dRow("QuantityDeleted") = PARAMOUT3.Substring(iPosition + 47, 3)

                        ResultDataSet.Tables("ComponentAmendments").Rows.Add(dRow)
                        iPosition = iPosition + 50
                    Loop

                    'Package Extras
                    iPosition = 0
                    Do While iPosition < 5000 AndAlso Not String.IsNullOrEmpty(PARAMOUT4) AndAlso Not String.IsNullOrWhiteSpace(PARAMOUT4.Substring(iPosition, 100).Trim())

                        dRow = Nothing
                        dRow = ResultDataSet.Tables("ExtraComponents").NewRow
                        dRow("ComponentID") = CLng(PARAMOUT4.Substring(iPosition, 13))
                        dRow("ComponentCode") = PARAMOUT4.Substring(iPosition + 13, 12).Trim()
                        dRow("ComponentDescription") = PARAMOUT4.Substring(iPosition + 25, 30).Trim()
                        dRow("AvailabilityFlag") = Utilities.convertToBool(PARAMOUT4.Substring(iPosition + 55, 1))
                        dRow("PWSFlag") = Utilities.convertToBool(PARAMOUT4.Substring(iPosition + 56, 1))
                        ResultDataSet.Tables("ExtraComponents").Rows.Add(dRow)
                        iPosition = iPosition + 100
                    Loop
                End If

                'Check if there's any more records
                If PARAMOUT.Substring(15231, 1) = "Y" Then
                    moreRecords = True
                    NextComponentGroupId = CType(PARAMOUT.Substring(15115, 13), String)
                    NextProduct = CType(PARAMOUT.Substring(15128, 6), String)
                    NextAlpha = CType(PARAMOUT.Substring(15134, 1), String)
                    NextSeat = CType(PARAMOUT.Substring(15135, 15), String)
                    NextChangedComponentId = CType(PARAMOUT.Substring(15150, 13), String)
                    NextComponentId = CType(PARAMOUT.Substring(15163, 13), String)
                Else
                    moreRecords = False
                End If
            Loop

        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS074R-01"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS074R", strError, err, ex, LogTypeConstants.TCBMPACKAGELOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Function CallWS074R(ByVal NextComponentId As String, ByVal NextChangedComponentId As String, ByVal NextSeat As String,
                                ByVal NextAlpha As String, ByVal NextProduct As String, ByVal NextComponentGroupId As String,
                                ByRef PARAMOUT2 As String, ByRef PARAMOUT3 As String, ByRef PARAMOUT4 As String) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim parmIO3 As iDB2Parameter
        Dim parmIO4 As iDB2Parameter
        Dim PARAMOUT1 As String = String.Empty

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS074R (@PARAM1,@PARAM2,@PARAM3,@PARAM4)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 15240)
        parmIO.Value = WS074RParm(NextComponentId, NextChangedComponentId, NextSeat, NextAlpha, NextProduct, NextComponentGroupId)
        parmIO.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add("@PARAM2", iDB2DbType.iDB2Char, 30000)
        parmIO2.Value = String.Empty
        parmIO2.Direction = ParameterDirection.InputOutput

        parmIO3 = cmdSELECT.Parameters.Add("@PARAM3", iDB2DbType.iDB2Char, 5000)
        parmIO3.Value = String.Empty
        parmIO3.Direction = ParameterDirection.InputOutput

        parmIO4 = cmdSELECT.Parameters.Add("@PARAM4", iDB2DbType.iDB2Char, 5000)
        parmIO4.Value = String.Empty
        parmIO4.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT1 = cmdSELECT.Parameters("@PARAM1").Value.ToString
        PARAMOUT2 = cmdSELECT.Parameters("@PARAM2").Value.ToString
        PARAMOUT3 = cmdSELECT.Parameters("@PARAM3").Value.ToString
        PARAMOUT4 = cmdSELECT.Parameters("@PARAM4").Value.ToString

        Return PARAMOUT1
    End Function

    Private Function WS074RParm(ByVal NextComponentId As String, ByVal NextChangedComponentId As String, ByVal NextSeat As String,
                                 ByVal NextAlpha As String, ByVal NextProduct As String, ByVal NextComponentGroupId As String) As String

        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(String.Empty, 15070))
        myString.Append(Utilities.FixStringLength(DePack.BusinessUnit, 20))
        myString.Append(Utilities.FixStringLength(String.Empty, 2))
        myString.Append(Utilities.FixStringLength(DePack.BoxOfficeUser, 10))
        myString.Append(Utilities.PadLeadingZeros(DePack.CallId, 13))
        myString.Append(Utilities.PadLeadingZeros(NextComponentGroupId, 13))
        myString.Append(Utilities.FixStringLength(NextProduct, 6))
        myString.Append(Utilities.FixStringLength(NextAlpha, 1))
        myString.Append(Utilities.FixStringLength(NextSeat, 15))
        myString.Append(Utilities.PadLeadingZeros(NextChangedComponentId, 13))
        myString.Append(Utilities.PadLeadingZeros(NextComponentId, 13))
        If String.IsNullOrWhiteSpace(DePack.TicketingProductCode) Then
            myString.Append(Utilities.FixStringLength(DePack.ProductCode, 6))
        Else
            myString.Append(Utilities.FixStringLength(DePack.TicketingProductCode, 6))
        End If
        myString.Append(Utilities.FixStringLength(DePack.BasketId, 36))
        myString.Append(Utilities.PadLeadingZeros(DePack.PackageID, 13))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.PadLeadingZeros("0000", 4))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))

        Return myString.ToString()

    End Function

    Private Function AccessDatabaseWS076R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim lastId As Integer = 0
        Dim moreRecords As Boolean = True

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the new voucher list table
        Dim DtComponentGroupList As New DataTable("ComponentList")
        ResultDataSet.Tables.Add(DtComponentGroupList)
        With DtComponentGroupList.Columns
            .Add("ComponentID", GetType(Int64))
            .Add("Description", GetType(String))
            .Add("ComponentStadiumCode", GetType(String))
        End With


        Try

            Do While moreRecords
                'Customers are returned in PARAM2 and PARAM3
                PARAMOUT = CallWS076R(lastId)

                'Set the response data on the first call to WS016R
                If lastId = 0 Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(10239, 1) = "E" OrElse PARAMOUT.Substring(10237, 2).Trim <> String.Empty Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(10237, 2).Trim
                        moreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                If moreRecords Then

                    'Loop for PARAM
                    Dim iPosition As Integer = 0
                    Do While Not String.IsNullOrEmpty(PARAMOUT.Substring(iPosition, 13).Trim())

                        dRow = Nothing
                        dRow = DtComponentGroupList.NewRow
                        dRow("ComponentID") = PARAMOUT.Substring(iPosition, 13)
                        dRow("Description") = PARAMOUT.Substring(iPosition + 13, 30).Trim()
                        dRow("ComponentStadiumCode") = PARAMOUT.Substring(iPosition + 43, 2).Trim()
                        DtComponentGroupList.Rows.Add(dRow)
                        iPosition = iPosition + 50
                    Loop

                End If

                'Check if there's any more records
                If PARAMOUT.Substring(10235, 1) = "Y" Then
                    moreRecords = True
                    lastId = CType(PARAMOUT.Substring(10222, 13), Integer)
                Else
                    moreRecords = False
                End If
            Loop

        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS076R-01"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS076R", strError, err, ex, LogTypeConstants.TCBMPACKAGELOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Function CallWS076R(ByVal lastId As Integer) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS076R (@PARAM1)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 10240)
        parmIO.Value = WS076RParm(lastId)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString


        Return PARAMOUT
    End Function

    Private Function WS076RParm(ByVal lastId As Integer) As String
        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(String.Empty, 10222))
        myString.Append(Utilities.PadLeadingZeros(lastId, 13))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))

        Return myString.ToString()

    End Function

    Private Function AccessDatabaseWS077R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim lastId As Integer = 0
        Dim moreRecords As Boolean = True

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the new voucher list table
        Dim DtComponentGroupList As New DataTable("ComponentGroupList")
        ResultDataSet.Tables.Add(DtComponentGroupList)
        With DtComponentGroupList.Columns
            .Add("ComponentGroupID", GetType(Int64))
            .Add("Description", GetType(String))
            .Add("ComponentGroupType", GetType(String))
        End With

        Try

            Do While moreRecords
                'Customers are returned in PARAM2 and PARAM3
                PARAMOUT = CallWS077R(lastId)

                'Set the response data on the first call to WS016R
                If lastId = 0 Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(10239, 1) = "E" OrElse PARAMOUT.Substring(10237, 2).Trim <> String.Empty Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(10237, 2).Trim
                        moreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                If moreRecords Then

                    'Loop for PARAM
                    Dim iPosition As Integer = 0
                    Do While Not String.IsNullOrEmpty(PARAMOUT.Substring(iPosition, 13).Trim())

                        dRow = Nothing
                        dRow = DtComponentGroupList.NewRow
                        dRow("ComponentGroupID") = PARAMOUT.Substring(iPosition, 13)
                        dRow("Description") = PARAMOUT.Substring(iPosition + 13, 30).Trim()
                        dRow("ComponentGroupType") = PARAMOUT.Substring(iPosition + 43, 2).Trim().ConvertDatabaseComponentGroupTypeToUIValues()
                        DtComponentGroupList.Rows.Add(dRow)
                        iPosition = iPosition + 50
                    Loop

                End If

                'Check if there's any more records
                If PARAMOUT.Substring(10235, 1) = "Y" Then
                    moreRecords = True
                    lastId = CType(PARAMOUT.Substring(10222, 13), Integer)
                Else
                    moreRecords = False
                End If
            Loop

        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS077R-01"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS077R", strError, err, ex, LogTypeConstants.TCBMPACKAGELOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Function CallWS077R(ByVal lastId As Integer) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS077R (@PARAM1)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 10240)
        parmIO.Value = WS077RParm(lastId)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString


        Return PARAMOUT
    End Function

    Private Function WS077RParm(ByVal lastId As Integer) As String
        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(String.Empty, 10222))
        myString.Append(Utilities.PadLeadingZeros(lastId, 13))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))

        Return myString.ToString()

    End Function

    Private Function AccessDatabaseWS078R() As ErrorObj

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
            .Add("DisplayDateFields", GetType(Boolean))
            .Add("TicketingFromDate", GetType(Date))
            .Add("TicketingToDate", GetType(Date))
        End With

        'Stand table
        Dim DtStand As New DataTable("Stand")
        ResultDataSet.Tables.Add(DtStand)
        With DtStand.Columns
            .Add("StandCode", GetType(String))
            .Add("StandDescription", GetType(String))
            .Add("Extra1Available", GetType(Boolean))
            .Add("Extra2Available", GetType(Boolean))
            .Add("Extra3Available", GetType(Boolean))
            .Add("Extra4Available", GetType(Boolean))
        End With

        'Area table when product code is not sent (Maintenance Mode)
        Dim DtArea As New DataTable("Area")
        'ResultDataSet.Tables.Add(DtArea)
        With DtArea.Columns
            .Add("AreaCode", GetType(String))
            .Add("AreaDescription", GetType(String))
        End With

        'Area table when product code is sent (Sales Mode)
        Dim DtArea1 As New DataTable("Area")
        'ResultDataSet.Tables.Add(DtArea1)
        With DtArea1.Columns
            .Add("AreaCode", GetType(String))
            .Add("FromQuantity", GetType(Integer))
            .Add("ToQuantity", GetType(Integer))
            .Add("Default", GetType(Boolean))
            .Add("DefaultType", GetType(String))
        End With

        If String.IsNullOrEmpty(DePack.ProductCode) Then
            ResultDataSet.Tables.Add(DtArea)
        Else
            ResultDataSet.Tables.Add(DtArea1)
        End If

        'Date table
        Dim DtDate As New DataTable("Date")
        ResultDataSet.Tables.Add(DtDate)
        With DtDate.Columns
            .Add("ProductDate", GetType(Date))
        End With

        'Price band table
        Dim DtPriceBand As New DataTable("PriceBand")
        ResultDataSet.Tables.Add(DtPriceBand)
        With DtPriceBand.Columns
            .Add("AreaCode", GetType(String))
            .Add("PriceBand", GetType(String))
            .Add("PriceBandText", GetType(String))
            .Add("Price", GetType(Decimal))
        End With

        Try

            PARAMOUT = CallWS078R()

            'Set the response data on the first call to WS078R
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(10239, 1) = "E" OrElse PARAMOUT.Substring(10237, 2).Trim <> String.Empty Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(10237, 2).Trim
                dRow("DisplayDateFields") = False
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                dRow("DisplayDateFields") = PARAMOUT.Substring(10203, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                If dRow("DisplayDateFields") Then
                    dRow("TicketingFromDate") = ISeriesDate(PARAMOUT.Substring(10196, 7))
                    dRow("TicketingToDate") = ISeriesDate(PARAMOUT.Substring(10189, 7))
                End If
            End If
            DtStatusResults.Rows.Add(dRow)

            'Loop for Stand
            Dim iPosition As Integer = 30
            Do While Not String.IsNullOrEmpty(PARAMOUT.Substring(iPosition, 50).Trim()) AndAlso iPosition < 5030

                dRow = Nothing
                dRow = DtStand.NewRow
                dRow("StandCode") = PARAMOUT.Substring(iPosition, 3).Trim()
                dRow("StandDescription") = PARAMOUT.Substring(iPosition + 3, 30).Trim()
                dRow("Extra1Available") = PARAMOUT.Substring(iPosition + 33, 1).ConvertFromISeriesYesNoToBoolean()
                dRow("Extra2Available") = PARAMOUT.Substring(iPosition + 34, 1).ConvertFromISeriesYesNoToBoolean()
                dRow("Extra3Available") = PARAMOUT.Substring(iPosition + 35, 1).ConvertFromISeriesYesNoToBoolean()
                dRow("Extra4Available") = PARAMOUT.Substring(iPosition + 36, 1).ConvertFromISeriesYesNoToBoolean()

                DtStand.Rows.Add(dRow)
                iPosition = iPosition + 50
            Loop

            'Loop for Area
            iPosition = 5030
            If String.IsNullOrEmpty(DePack.ProductCode) Then
                ' There can be no area returned by the program in between two areas since there is a tight coupling between
                ' what controls are visible on the UI and the areas fetched. Like if there is no third area and there exists
                ' a fourth area, the drop down control will not be displayed on the UI for third area but will be displayed
                ' for the fourth area.
                'Do While Not String.IsNullOrEmpty(PARAMOUT.Substring(iPosition, 50).Trim()) AndAlso iPosition < 5530
                Do While iPosition < 5530

                    dRow = Nothing
                    dRow = DtArea.NewRow
                    dRow("AreaCode") = PARAMOUT.Substring(iPosition, 4).Trim()
                    dRow("AreaDescription") = PARAMOUT.Substring(iPosition + 4, 30).Trim()
                    DtArea.Rows.Add(dRow)
                    iPosition = iPosition + 50
                Loop
            Else
                Do While Not String.IsNullOrEmpty(PARAMOUT.Substring(iPosition, 50).Trim()) AndAlso iPosition < 5530

                    dRow = Nothing
                    dRow = DtArea1.NewRow
                    dRow("AreaCode") = PARAMOUT.Substring(iPosition, 4).Trim()
                    dRow("FromQuantity") = Integer.Parse(PARAMOUT.Substring(iPosition + 4, 5).Trim())
                    dRow("ToQuantity") = Integer.Parse(PARAMOUT.Substring(iPosition + 9, 5).Trim())
                    dRow("Default") = PARAMOUT.Substring(iPosition + 14, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                    dRow("DefaultType") = PARAMOUT.Substring(iPosition + 15, 1).Trim()
                    DtArea1.Rows.Add(dRow)
                    iPosition = iPosition + 50
                Loop

            End If


            'Loop for Product Date
            iPosition = 5530
            Do While Not String.IsNullOrEmpty(PARAMOUT.Substring(iPosition, 7).Trim()) AndAlso iPosition < 6930

                dRow = Nothing
                dRow = DtDate.NewRow
                dRow("ProductDate") = ISeriesDate(PARAMOUT.Substring(iPosition, 7))
                DtDate.Rows.Add(dRow)
                iPosition = iPosition + 7
            Loop

            'Loop for Price Band
            iPosition = 6930
            Do While Not String.IsNullOrEmpty(PARAMOUT.Substring(iPosition, 50).Trim()) AndAlso iPosition < 8680

                dRow = Nothing
                dRow = DtPriceBand.NewRow
                dRow("AreaCode") = PARAMOUT.Substring(iPosition, 4).Trim()
                dRow("PriceBand") = PARAMOUT.Substring(iPosition + 4, 1).Trim()
                dRow("PriceBandText") = PARAMOUT.Substring(iPosition + 5, 15).Trim()
                dRow("Price") = FormatPrice(PARAMOUT.Substring(iPosition + 20, 9).Trim())
                DtPriceBand.Rows.Add(dRow)
                iPosition = iPosition + 50
            Loop

        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS078R-01"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS078R", strError, err, ex, LogTypeConstants.TCBMPACKAGELOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Function CallWS078R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS078R (@PARAM1)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 10240)
        parmIO.Value = WS078RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString


        Return PARAMOUT
    End Function

    Private Function WS078RParm() As String
        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(String.Empty, 10186))
        myString.Append(Utilities.FixStringLength(DePack.IsCATBasket.ConvertToISeriesYesNo, 1))
        myString.Append(Utilities.FixStringLength(DePack.AvailableToSellAvailableTickets.ConvertToISeriesYesNo, 1))
        myString.Append(Utilities.FixStringLength(DePack.AvailableToSell03.ConvertToISeriesYesNo, 1))
        myString.Append(Utilities.PadLeadingZeros(0, 14))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.PadLeadingZeros(DePack.ComponentGroupID, 13))
        myString.Append(Utilities.FixStringLength(DePack.ProductCode, 6))
        myString.Append(Utilities.PadLeadingZeros(DePack.ComponentID, 13))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))

        Return myString.ToString()

    End Function

    Private Function AccessDatabaseWS079R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim lastId As Integer = 0
        Dim moreRecords As Boolean = True

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtComponentGroupDetails As New DataTable("ComponentGroupDetails")
        ResultDataSet.Tables.Add(DtComponentGroupDetails)
        With DtComponentGroupDetails.Columns
            .Add("GroupCode", GetType(String))
            .Add("GroupDescription", GetType(String))
            .Add("Type", GetType(String))
            .Add("QuantityToSelect", GetType(Integer))
            .Add("Department", GetType(String))
            .Add("Default1", GetType(Boolean))
            .Add("Quantity1FromValue", GetType(Integer))
            .Add("Quantity1ToValue", GetType(Integer))
            .Add("Default1Type", GetType(String))
            .Add("Default2", GetType(Boolean))
            .Add("Quantity2FromValue", GetType(Integer))
            .Add("Quantity2ToValue", GetType(Integer))
            .Add("Default2Type", GetType(String))
            .Add("Default3", GetType(Boolean))
            .Add("Quantity3FromValue", GetType(Integer))
            .Add("Quantity3ToValue", GetType(Integer))
            .Add("Default3Type", GetType(String))
            .Add("Default4", GetType(Boolean))
            .Add("Quantity4FromValue", GetType(Integer))
            .Add("Quantity4ToValue", GetType(Integer))
            .Add("Default4Type", GetType(String))
            .Add("Default5", GetType(Boolean))
            .Add("Quantity5FromValue", GetType(Integer))
            .Add("Quantity5ToValue", GetType(Integer))
            .Add("Default5Type", GetType(String))
            .Add("Default6", GetType(Boolean))
            .Add("Quantity6FromValue", GetType(Integer))
            .Add("Quantity6ToValue", GetType(Integer))
            .Add("Default6Type", GetType(String))
            .Add("Default7", GetType(Boolean))
            .Add("Quantity7FromValue", GetType(Integer))
            .Add("Quantity7ToValue", GetType(Integer))
            .Add("Default7Type", GetType(String))
            .Add("Default8", GetType(Boolean))
            .Add("Quantity8FromValue", GetType(Integer))
            .Add("Quantity8ToValue", GetType(Integer))
            .Add("Default8Type", GetType(String))
            .Add("Default9", GetType(Boolean))
            .Add("Quantity9FromValue", GetType(Integer))
            .Add("Quantity9ToValue", GetType(Integer))
            .Add("Default9Type", GetType(String))
            .Add("Default10", GetType(Boolean))
            .Add("Quantity10FromValue", GetType(Integer))
            .Add("Quantity10ToValue", GetType(Integer))
            .Add("Default10Type", GetType(String))
            .Add("GroupStadiumCode", GetType(String))
            .Add("NoneReqOpt", GetType(String))
        End With

        Dim DtComponentDetails As New DataTable("ComponentDetails")
        ResultDataSet.Tables.Add(DtComponentDetails)
        With DtComponentDetails.Columns
            .Add("ComponentID", GetType(Int64))
            .Add("ComponentDescription", GetType(String))
            .Add("ComponentSequence", GetType(Integer))
            .Add("Area1", GetType(String))
            .Add("Area2", GetType(String))
            .Add("Area3", GetType(String))
            .Add("Area4", GetType(String))
            .Add("Area5", GetType(String))
            .Add("Area6", GetType(String))
            .Add("Area7", GetType(String))
            .Add("Area8", GetType(String))
            .Add("Area9", GetType(String))
            .Add("Area10", GetType(String))
            .Add("QuantityArea", GetType(String))

            .Add("Default1", GetType(Boolean))
            .Add("Default1FromValue", GetType(Integer))
            .Add("Default1ToValue", GetType(Integer))
            .Add("Default1Type", GetType(String))
            .Add("Default2", GetType(Boolean))
            .Add("Default2FromValue", GetType(Integer))
            .Add("Default2ToValue", GetType(Integer))
            .Add("Default2Type", GetType(String))
            .Add("Default3", GetType(Boolean))
            .Add("Default3FromValue", GetType(Integer))
            .Add("Default3ToValue", GetType(Integer))
            .Add("Default3Type", GetType(String))
            .Add("Default4", GetType(Boolean))
            .Add("Default4FromValue", GetType(Integer))
            .Add("Default4ToValue", GetType(Integer))
            .Add("Default4Type", GetType(String))
            .Add("Default5", GetType(Boolean))
            .Add("Default5FromValue", GetType(Integer))
            .Add("Default5ToValue", GetType(Integer))
            .Add("Default5Type", GetType(String))
            .Add("Default6", GetType(Boolean))
            .Add("Default6FromValue", GetType(Integer))
            .Add("Default6ToValue", GetType(Integer))
            .Add("Default6Type", GetType(String))
            .Add("Default7", GetType(Boolean))
            .Add("Default7FromValue", GetType(Integer))
            .Add("Default7ToValue", GetType(Integer))
            .Add("Default7Type", GetType(String))
            .Add("Default8", GetType(Boolean))
            .Add("Default8FromValue", GetType(Integer))
            .Add("Default8ToValue", GetType(Integer))
            .Add("Default8Type", GetType(String))
            .Add("Default9", GetType(Boolean))
            .Add("Default9FromValue", GetType(Integer))
            .Add("Default9ToValue", GetType(Integer))
            .Add("Default9Type", GetType(String))
            .Add("Default10", GetType(Boolean))
            .Add("Default10FromValue", GetType(Integer))
            .Add("Default10ToValue", GetType(Integer))
            .Add("Default10Type", GetType(String))
            .Add("Sequence", GetType(Integer))
            .Add("AllAvailableDates", GetType(Boolean))
            .Add("StartDateAdjustment", GetType(String))
            .Add("StartDateAdjustmentInDays", GetType(Integer))
            .Add("EndDateAdjustment", GetType(String))
            .Add("EndDateAdjustmentInDays", GetType(Integer))
            .Add("ExtraDaysChargeable", GetType(Boolean))
            .Add("ExtraDaysDiscount", GetType(Integer))
            .Add("IsAvailable", GetType(Boolean))
        End With


        Try

            Do While moreRecords
                PARAMOUT = CallWS079R(lastId)

                If lastId = 0 Then
                    dRow = Nothing
                    dRow = DtStatusResults.NewRow
                    If PARAMOUT.Substring(10239, 1) = "E" OrElse PARAMOUT.Substring(10237, 2).Trim <> String.Empty Then
                        dRow("ErrorOccurred") = "E"
                        dRow("ReturnCode") = PARAMOUT.Substring(10237, 2).Trim
                        moreRecords = False
                    Else
                        dRow("ErrorOccurred") = ""
                        dRow("ReturnCode") = ""
                    End If
                    DtStatusResults.Rows.Add(dRow)
                End If

                If moreRecords Then

                    'Loop for PARAM
                    Dim iPosition As Integer = 0

                    'Loop for Component Group Details
                    dRow = Nothing
                    dRow = DtComponentGroupDetails.NewRow
                    dRow("GroupCode") = PARAMOUT.Substring(iPosition, 12).Trim()
                    dRow("GroupDescription") = PARAMOUT.Substring(iPosition + 12, 30).Trim()
                    dRow("Type") = PARAMOUT.Substring(iPosition + 42, 2).Trim().ConvertDatabaseComponentGroupTypeToUIValues()
                    dRow("QuantityToSelect") = PARAMOUT.Substring(iPosition + 44, 5).Trim()
                    dRow("Department") = PARAMOUT.Substring(iPosition + 49, 12).Trim()
                    dRow("Default1") = PARAMOUT.Substring(iPosition + 61, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                    dRow("Quantity1FromValue") = PARAMOUT.Substring(iPosition + 62, 5).Trim()
                    dRow("Quantity1ToValue") = PARAMOUT.Substring(iPosition + 67, 5).Trim()
                    dRow("Default2") = PARAMOUT.Substring(iPosition + 72, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                    dRow("Quantity2FromValue") = PARAMOUT.Substring(iPosition + 73, 5).Trim()
                    dRow("Quantity2ToValue") = PARAMOUT.Substring(iPosition + 78, 5).Trim()
                    dRow("Default3") = PARAMOUT.Substring(iPosition + 83, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                    dRow("Quantity3FromValue") = PARAMOUT.Substring(iPosition + 84, 5).Trim()
                    dRow("Quantity3ToValue") = PARAMOUT.Substring(iPosition + 89, 5).Trim()
                    dRow("Default4") = PARAMOUT.Substring(iPosition + 94, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                    dRow("Quantity4FromValue") = PARAMOUT.Substring(iPosition + 95, 5).Trim()
                    dRow("Quantity4ToValue") = PARAMOUT.Substring(iPosition + 100, 5).Trim()
                    dRow("Default5") = PARAMOUT.Substring(iPosition + 105, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                    dRow("Quantity5FromValue") = PARAMOUT.Substring(iPosition + 106, 5).Trim()
                    dRow("Quantity5ToValue") = PARAMOUT.Substring(iPosition + 111, 5).Trim()
                    dRow("Default6") = PARAMOUT.Substring(iPosition + 116, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                    dRow("Quantity6FromValue") = PARAMOUT.Substring(iPosition + 117, 5).Trim()
                    dRow("Quantity6ToValue") = PARAMOUT.Substring(iPosition + 122, 5).Trim()
                    dRow("Default7") = PARAMOUT.Substring(iPosition + 127, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                    dRow("Quantity7FromValue") = PARAMOUT.Substring(iPosition + 128, 5).Trim()
                    dRow("Quantity7ToValue") = PARAMOUT.Substring(iPosition + 133, 5).Trim()
                    dRow("Default8") = PARAMOUT.Substring(iPosition + 138, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                    dRow("Quantity8FromValue") = PARAMOUT.Substring(iPosition + 139, 5).Trim()
                    dRow("Quantity8ToValue") = PARAMOUT.Substring(iPosition + 144, 5).Trim()
                    dRow("Default9") = PARAMOUT.Substring(iPosition + 149, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                    dRow("Quantity9FromValue") = PARAMOUT.Substring(iPosition + 150, 5).Trim()
                    dRow("Quantity9ToValue") = PARAMOUT.Substring(iPosition + 155, 5).Trim()
                    dRow("Default10") = PARAMOUT.Substring(iPosition + 160, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                    dRow("Quantity10FromValue") = PARAMOUT.Substring(iPosition + 161, 5).Trim()
                    dRow("Quantity10ToValue") = PARAMOUT.Substring(iPosition + 166, 5).Trim()
                    dRow("Default1Type") = PARAMOUT.Substring(iPosition + 171, 1).Trim()
                    dRow("Default2Type") = PARAMOUT.Substring(iPosition + 172, 1).Trim()
                    dRow("Default3Type") = PARAMOUT.Substring(iPosition + 173, 1).Trim()
                    dRow("Default4Type") = PARAMOUT.Substring(iPosition + 174, 1).Trim()
                    dRow("Default5Type") = PARAMOUT.Substring(iPosition + 175, 1).Trim()
                    dRow("Default6Type") = PARAMOUT.Substring(iPosition + 176, 1).Trim()
                    dRow("Default7Type") = PARAMOUT.Substring(iPosition + 177, 1).Trim()
                    dRow("Default8Type") = PARAMOUT.Substring(iPosition + 178, 1).Trim()
                    dRow("Default9Type") = PARAMOUT.Substring(iPosition + 179, 1).Trim()
                    dRow("Default10Type") = PARAMOUT.Substring(iPosition + 180, 1).Trim()
                    dRow("GroupStadiumCode") = PARAMOUT.Substring(iPosition + 181, 2).Trim()
                    dRow("NoneReqOpt") = PARAMOUT.Substring(iPosition + 183, 1).Trim()
                    DtComponentGroupDetails.Rows.Add(dRow)
                    iPosition = iPosition + 500

                    'If there are no components in the component group then do not retreive any data
                    Do While Not String.IsNullOrEmpty(PARAMOUT.Substring(iPosition, 13).Trim()) AndAlso iPosition < 9500

                        dRow = Nothing
                        dRow = DtComponentDetails.NewRow
                        dRow("ComponentID") = PARAMOUT.Substring(iPosition, 13).Trim()
                        dRow("ComponentDescription") = PARAMOUT.Substring(iPosition + 13, 30).Trim()
                        dRow("ComponentSequence") = PARAMOUT.Substring(iPosition + 203, 3).Trim()
                        dRow("Area1") = PARAMOUT.Substring(iPosition + 43, 4).Trim()
                        dRow("Area2") = PARAMOUT.Substring(iPosition + 47, 4).Trim()
                        dRow("Area3") = PARAMOUT.Substring(iPosition + 51, 4).Trim()
                        dRow("Area4") = PARAMOUT.Substring(iPosition + 55, 4).Trim()
                        dRow("Area5") = PARAMOUT.Substring(iPosition + 59, 4).Trim()
                        dRow("Area6") = PARAMOUT.Substring(iPosition + 63, 4).Trim()
                        dRow("Area7") = PARAMOUT.Substring(iPosition + 67, 4).Trim()
                        dRow("Area8") = PARAMOUT.Substring(iPosition + 71, 4).Trim()
                        dRow("Area9") = PARAMOUT.Substring(iPosition + 75, 4).Trim()
                        dRow("Area10") = PARAMOUT.Substring(iPosition + 79, 4).Trim()

                        dRow("Default1") = PARAMOUT.Substring(iPosition + 83, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("Default1FromValue") = PARAMOUT.Substring(iPosition + 84, 5).Trim()
                        dRow("Default1ToValue") = PARAMOUT.Substring(iPosition + 89, 5).Trim()
                        dRow("Default2") = PARAMOUT.Substring(iPosition + 94, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("Default2FromValue") = PARAMOUT.Substring(iPosition + 95, 5).Trim()
                        dRow("Default2ToValue") = PARAMOUT.Substring(iPosition + 100, 5).Trim()
                        dRow("Default3") = PARAMOUT.Substring(iPosition + 105, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("Default3FromValue") = PARAMOUT.Substring(iPosition + 106, 5).Trim()
                        dRow("Default3ToValue") = PARAMOUT.Substring(iPosition + 111, 5).Trim()
                        dRow("Default4") = PARAMOUT.Substring(iPosition + 116, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("Default4FromValue") = PARAMOUT.Substring(iPosition + 117, 5).Trim()
                        dRow("Default4ToValue") = PARAMOUT.Substring(iPosition + 122, 5).Trim()
                        dRow("Default5") = PARAMOUT.Substring(iPosition + 127, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("Default5FromValue") = PARAMOUT.Substring(iPosition + 128, 5).Trim()
                        dRow("Default5ToValue") = PARAMOUT.Substring(iPosition + 133, 5).Trim()
                        dRow("Default6") = PARAMOUT.Substring(iPosition + 138, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("Default6FromValue") = PARAMOUT.Substring(iPosition + 139, 5).Trim()
                        dRow("Default6ToValue") = PARAMOUT.Substring(iPosition + 144, 5).Trim()
                        dRow("Default7") = PARAMOUT.Substring(iPosition + 149, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("Default7FromValue") = PARAMOUT.Substring(iPosition + 150, 5).Trim()
                        dRow("Default7ToValue") = PARAMOUT.Substring(iPosition + 155, 5).Trim()
                        dRow("Default8") = PARAMOUT.Substring(iPosition + 160, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("Default8FromValue") = PARAMOUT.Substring(iPosition + 161, 5).Trim()
                        dRow("Default8ToValue") = PARAMOUT.Substring(iPosition + 166, 5).Trim()
                        dRow("Default9") = PARAMOUT.Substring(iPosition + 171, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("Default9FromValue") = PARAMOUT.Substring(iPosition + 172, 5).Trim()
                        dRow("Default9ToValue") = PARAMOUT.Substring(iPosition + 177, 5).Trim()
                        dRow("Default10") = PARAMOUT.Substring(iPosition + 182, 1).Trim().ConvertFromISeriesYesNoToBoolean()
                        dRow("Default10FromValue") = PARAMOUT.Substring(iPosition + 183, 5).Trim()
                        dRow("Default10ToValue") = PARAMOUT.Substring(iPosition + 188, 5).Trim()
                        dRow("Default1Type") = PARAMOUT.Substring(iPosition + 193, 1).Trim()
                        dRow("Default2Type") = PARAMOUT.Substring(iPosition + 194, 1).Trim()
                        dRow("Default3Type") = PARAMOUT.Substring(iPosition + 195, 1).Trim()
                        dRow("Default4Type") = PARAMOUT.Substring(iPosition + 196, 1).Trim()
                        dRow("Default5Type") = PARAMOUT.Substring(iPosition + 197, 1).Trim()
                        dRow("Default6Type") = PARAMOUT.Substring(iPosition + 198, 1).Trim()
                        dRow("Default7Type") = PARAMOUT.Substring(iPosition + 199, 1).Trim()
                        dRow("Default8Type") = PARAMOUT.Substring(iPosition + 200, 1).Trim()
                        dRow("Default9Type") = PARAMOUT.Substring(iPosition + 201, 1).Trim()
                        dRow("Default10Type") = PARAMOUT.Substring(iPosition + 202, 1).Trim()

                        dRow("Sequence") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(iPosition + 203, 3).Trim())  'This is not used in the frontend
                        dRow("AllAvailableDates") = Utilities.CheckForDBNull_Boolean_DefaultTrue(PARAMOUT.Substring(iPosition + 206, 1).Trim().ConvertFromISeriesYesNoToBoolean())
                        dRow("StartDateAdjustment") = Utilities.CheckForDBNull_String(PARAMOUT.Substring(iPosition + 207, 1).Trim())
                        dRow("StartDateAdjustmentInDays") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(iPosition + 208, 3).Trim())
                        dRow("EndDateAdjustment") = Utilities.CheckForDBNull_String(PARAMOUT.Substring(iPosition + 211, 1).Trim())
                        dRow("EndDateAdjustmentInDays") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(iPosition + 212, 3).Trim())
                        dRow("ExtraDaysChargeable") = Utilities.CheckForDBNull_Boolean_DefaultTrue(PARAMOUT.Substring(iPosition + 215, 1).Trim().ConvertFromISeriesYesNoToBoolean())
                        dRow("ExtraDaysDiscount") = Utilities.CheckForDBNull_Int(PARAMOUT.Substring(iPosition + 216, 3).Trim())
                        dRow("IsAvailable") = Utilities.CheckForDBNull_Boolean_DefaultFalse(PARAMOUT.Substring(iPosition + 219, 1).Trim().ConvertFromISeriesYesNoToBoolean())
                        dRow("QuantityArea") = PARAMOUT.Substring(iPosition + 220, 4).Trim()
                        DtComponentDetails.Rows.Add(dRow)
                        iPosition = iPosition + 300
                    Loop

                End If

                'Check if there's any more records
                If PARAMOUT.Substring(10222, 1) = "Y" Then
                    moreRecords = True
                    lastId = CType(PARAMOUT.Substring(10209, 13), Integer)
                Else
                    moreRecords = False
                End If
            Loop

        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS079R-01"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS079R", strError, err, ex, LogTypeConstants.TCBMPACKAGELOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Function CallWS079R(ByVal lastId As Integer) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS079R (@PARAM1)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 10240)
        parmIO.Value = WS079RParm(lastId)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString


        Return PARAMOUT
    End Function

    Private Function WS079RParm(ByVal lastId As Integer) As String
        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(String.Empty, 10209))
        myString.Append(Utilities.PadLeadingZeros(lastId, 13))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.PadLeadingZeros(DePack.ComponentGroupID, 13))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))

        Return myString.ToString()

    End Function

    Private Function AccessDatabaseWS080R() As ErrorObj

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
            .Add("ComponentGroupId", GetType(Long))
        End With

        Try

            PARAMOUT = CallWS080R()
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" OrElse PARAMOUT.Substring(1021, 2).Trim <> String.Empty Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2).Trim
                dRow("ComponentGroupId") = 0
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
                dRow("ComponentGroupId") = If(String.IsNullOrWhiteSpace(PARAMOUT.Substring(1, 13)), 0, CType(PARAMOUT.Substring(1, 13), Long))
            End If
            DtStatusResults.Rows.Add(dRow)


        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS080R-01"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS080R", strError, err, ex, LogTypeConstants.TCBMPACKAGELOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Function CallWS080R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS080R (@PARAM1)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS080RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString


        Return PARAMOUT
    End Function

    Private Function WS080RParm() As String
        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(DePack.Mode.GetISeriesOperationMode(), 1))
        myString.Append(Utilities.PadLeadingZeros(DePack.ComponentGroupID, 13))
        myString.Append(Utilities.FixStringLength(DePack.ComponentCode, 12))
        myString.Append(Utilities.FixStringLength(DePack.ComponentGroupDescription, 30))
        myString.Append(Utilities.FixStringLength(DePack.ComponentType.ConvertUIComponentGroupTypeToDatabaseValues(), 2))
        myString.Append(Utilities.PadLeadingZeros(DePack.QuantityToSelect, 5))

        For Each item As ComponentDefault In DePack.ComponentDefaults
            myString.Append(Utilities.FixStringLength(item.IsDefault.ConvertToISeriesYesNo(), 1))
            myString.Append(Utilities.PadLeadingZeros(item.DefaultFrom.ToString(), 5))
            myString.Append(Utilities.PadLeadingZeros(item.DefaultTo.ToString(), 5))
            myString.Append(Utilities.FixStringLength(item.Group.GetISeriesDefaultType(), 1))
        Next

        'If there are less than 10 default items, then add missing values
        Dim iNoOfItemsMissing As Integer = 0
        If DePack.ComponentDefaults.Count < 10 Then
            iNoOfItemsMissing = 10 - DePack.ComponentDefaults.Count
            For i As Integer = 1 To iNoOfItemsMissing
                myString.Append(Utilities.FixStringLength(False.ConvertToISeriesYesNo, 1))
                myString.Append(Utilities.PadLeadingZeros(String.Empty, 10))
                myString.Append(Utilities.FixStringLength(DefaultType.PerGroup.GetISeriesDefaultType(), 1))
            Next
        End If

        myString.Append(Utilities.FixStringLength(String.Empty, 836))
        myString.Append(Utilities.FixStringLength(DePack.NoneReqOpt, 1))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))

        Return myString.ToString()

    End Function

    Private Function AccessDatabaseWS081R() As ErrorObj

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

            PARAMOUT = CallWS081R()
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" OrElse PARAMOUT.Substring(1021, 2).Trim <> String.Empty Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2).Trim
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)


        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS081R-01"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS081R", strError, err, ex, LogTypeConstants.TCBMPACKAGELOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Function CallWS081R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS081R (@PARAM1)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS081RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString


        Return PARAMOUT
    End Function

    Private Function WS081RParm() As String
        Dim myString As New StringBuilder

        'Construct the parameter
        myString.Append(Utilities.FixStringLength(DePack.Mode.GetISeriesOperationMode(), 1))
        myString.Append(Utilities.PadLeadingZeros(DePack.ComponentGroupID, 13))
        myString.Append(Utilities.PadLeadingZeros(DePack.ComponentID, 13))
        myString.Append(Utilities.FixStringLength(DePack.ComponentType, 1))
        myString.Append(Utilities.PadLeadingZeros(DePack.ComponentSequence, 3))

        For Each item As AreaInComponent In DePack.AreaInComponents
            myString.Append(Utilities.FixStringLength(item.Area, 4))
        Next

        'If there are less than 10 Area items, then add spaces to the string for each one of the non existent items.
        Dim iNoOfItemsMissing As Integer = 0
        If DePack.AreaInComponents.Count < 10 Then
            iNoOfItemsMissing = 10 - DePack.AreaInComponents.Count
            myString.Append(Utilities.FixStringLength(String.Empty, iNoOfItemsMissing * 4))
        End If

        For Each item As ComponentDefault In DePack.ComponentDefaults
            myString.Append(Utilities.FixStringLength(item.IsDefault.ConvertToISeriesYesNo(), 1))
            myString.Append(Utilities.PadLeadingZeros(item.DefaultFrom, 5))
            myString.Append(Utilities.PadLeadingZeros(item.DefaultTo, 5))
            myString.Append(Utilities.FixStringLength(item.Group.GetISeriesDefaultType(), 1))
        Next

        'If there are less than 10 Default items, then add spaces to the string for each one of the non existent items.
        'Total 12 spaces are required for each missing record.
        iNoOfItemsMissing = 0
        If DePack.ComponentDefaults.Count < 10 Then
            iNoOfItemsMissing = 10 - DePack.ComponentDefaults.Count
            Dim IsDefault As Boolean = False
            For i As Integer = 1 To iNoOfItemsMissing
                myString.Append(Utilities.FixStringLength(IsDefault.ConvertToISeriesYesNo(), 1))
                myString.Append(Utilities.PadLeadingZeros(String.Empty, 10))
                myString.Append(Utilities.FixStringLength(DefaultType.PerGroup.GetISeriesDefaultType(), 1))
            Next
        End If

        myString.Append(Utilities.FixStringLength(DePack.Sequence.GetISeriesSequenceMode(), 1))

        myString.Append(Utilities.FixStringLength(DePack.AllAvailableDates.ConvertToISeriesYesNo(), 1))
        myString.Append(Utilities.FixStringLength(DePack.StartDayAdjustment.DayAdjustment, 1))
        myString.Append(Utilities.PadLeadingZeros(DePack.StartDayAdjustment.Days, 3))
        myString.Append(Utilities.FixStringLength(DePack.EndDayAdjustment.DayAdjustment, 1))
        myString.Append(Utilities.PadLeadingZeros(DePack.EndDayAdjustment.Days, 3))
        myString.Append(Utilities.FixStringLength(DePack.ExtraDaysChargeable.ConvertToISeriesYesNo(), 1))
        myString.Append(Utilities.PadLeadingZeros(DePack.DiscountOnDailyRate, 3))
        myString.Append(Utilities.FixStringLength(DePack.QuantityArea, 4))

        myString.Append(Utilities.FixStringLength(String.Empty, 811))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))

        Return myString.ToString()

    End Function

    Private Function AccessDatabaseWS099S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable(GlobalConstants.STATUS_RESULTS_TABLE_NAME)
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtSeat As New DataTable("Seat")
        ResultDataSet.Tables.Add(DtSeat)
        Try
            CallWS099S()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS099S-01"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS099S", strError, err, ex, LogTypeConstants.TCBMPACKAGELOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Sub CallWS099S()
        Dim cmdSelect As iDB2Command = Nothing
        Dim strHeader As New StringBuilder
        Dim cmdAdapter = New iDB2DataAdapter

        cmdSelect = conTALENTTKT.CreateCommand()
        strHeader.Append("CALL ")
        strHeader.Append("WS099S(@PARAM0, @PARAM1, @PARAM2)")
        cmdSelect = New iDB2Command(strHeader.ToString(), conTALENTTKT)

        Dim pPackID As iDB2Parameter
        Dim pProductCode As iDB2Parameter
        Dim pErrorCode As iDB2Parameter

        pPackID = cmdSelect.Parameters.Add(Param0, iDB2DbType.iDB2Decimal, 13)
        pProductCode = cmdSelect.Parameters.Add(Param1, iDB2DbType.iDB2Char, 6)
        pErrorCode = cmdSelect.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput

        pPackID.Value = Utilities.PadLeadingZeros(DePack.PackageID, 13)
        pProductCode.Value = Utilities.FixStringLength(DePack.ProductCode, 6)
        pErrorCode.Value = String.Empty

        cmdAdapter.SelectCommand = cmdSelect
        cmdAdapter.Fill(ResultDataSet, "Seat")

        Dim drStatus As DataRow = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).NewRow
        If CStr(cmdSelect.Parameters(2).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = CStr(cmdSelect.Parameters(2).Value).Trim
            drStatus("ReturnCode") = GlobalConstants.ERRORFLAG
        End If
        ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Add(drStatus)
    End Sub

#Region "Package Seat Details"
    Private Function AccessDatabaseWS074S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        Dim dtPackageSeatDetails As New DataTable("PackageSeatDetails")
        ResultDataSet.Tables.Add(dtPackageSeatDetails)

        Try
            CallWS074S()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Error during database Access"
                .ErrorNumber = "DBPackage-WS074S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Sub CallWS074S()
        Dim cmdSelect As iDB2Command = Nothing
        Dim strHeader As New StringBuilder
        Dim seatAllo As SeatAllocation = DePack.SeatAllocations(0)
        Dim cmdAdapter = New iDB2DataAdapter

        cmdSelect = conTALENTTKT.CreateCommand()
        strHeader.Append("CALL ")
        'strHeader.Append(Settings.StoredProcedureGroup.Trim)
        strHeader.Append("WS074S(@PARAM0, @PARAM1, @PARAM2)")
        cmdSelect = New iDB2Command(strHeader.ToString(), conTALENTTKT)

        Dim pBasketID As iDB2Parameter
        Dim pBulkID As iDB2Parameter
        Dim pErrorCode As iDB2Parameter

        pBasketID = cmdSelect.Parameters.Add(Param0, iDB2DbType.iDB2Char, 36)
        pBulkID = cmdSelect.Parameters.Add(Param1, iDB2DbType.iDB2Decimal, 13)
        pErrorCode = cmdSelect.Parameters.Add(Param2, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput

        pBasketID.Value = DePack.BasketId
        pBulkID.Value = seatAllo.BulkId
        pErrorCode.Value = String.Empty


        cmdAdapter.SelectCommand = cmdSelect
        cmdAdapter.Fill(ResultDataSet, "PackageSeatDetails")

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(cmdSelect.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = CStr(cmdSelect.Parameters(0).Value).Trim
            drStatus("ReturnCode") = GlobalConstants.ERRORFLAG
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub
#End Region

#Region "Hospitality Bookings List"
    Private Function AccessDatabaseCS001S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Hospitality Bookings data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        Dim dtHospitalityBookings As New DataTable("HospitalityBookings")
        Dim dtPrintDetails As New DataTable("HospitalityPrintDetails")
        With dtHospitalityBookings.Columns
            .Add("FormattedDate", GetType(String))
        End With
        ResultDataSet.Tables.Add(dtHospitalityBookings)
        ResultDataSet.Tables.Add(dtPrintDetails)

        Try
            CallCS001S()
            getFormattedDate(ResultDataSet.Tables("HospitalityBookings"))
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Error during database Access"
                .ErrorNumber = "DBPackage-CS001S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Sub CallCS001S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL CS001S(@ErrorCode, @RecordCount, @PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6, @PARAM7, @PARAM8, @PARAM9, @PARAM10,@PARAM11)"
        _cmd.CommandType = CommandType.Text

        Dim pAgent As iDB2Parameter
        Dim pCallID As iDB2Parameter
        Dim pFromDate As iDB2Parameter
        Dim pToDate As iDB2Parameter
        Dim pStatus As iDB2Parameter
        Dim pCustomer As iDB2Parameter
        Dim pPackage As iDB2Parameter
        Dim pProduct As iDB2Parameter
        Dim pMarkOrderFor As iDB2Parameter
        Dim pQandAStatus As iDB2Parameter
        Dim pMaxRecords As IDbDataParameter
        Dim pErrorCode As iDB2Parameter
        Dim pPrintStatus As iDB2Parameter
        Dim pPrintMode As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pMaxRecords = _cmd.Parameters.Add(RecordCount, iDB2DbType.iDB2Decimal, 5)
        pAgent = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pCallID = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Decimal, 13)
        pFromDate = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Decimal, 8)
        pToDate = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Decimal, 8)
        pStatus = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 1)
        pCustomer = _cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 12)
        pPackage = _cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 30)
        pProduct = _cmd.Parameters.Add(Param7, iDB2DbType.iDB2Char, 40)
        pMarkOrderFor = _cmd.Parameters.Add(Param8, iDB2DbType.iDB2Char, 1)
        pQandAStatus = _cmd.Parameters.Add(Param9, iDB2DbType.iDB2Char, 1)
        pPrintStatus = _cmd.Parameters.Add(Param10, iDB2DbType.iDB2Char, 1)
        pPrintMode = _cmd.Parameters.Add(Param10, iDB2DbType.iDB2Char, 1)

        pErrorCode.Value = String.Empty
        pMaxRecords.Value = DePack.HospitalityBookingFilters.MaxRecords
        pAgent.Value = DePack.HospitalityBookingFilters.Agent
        pCallID.Value = DePack.HospitalityBookingFilters.CallId
        pFromDate.Value = DePack.HospitalityBookingFilters.Fromdate
        pToDate.Value = DePack.HospitalityBookingFilters.ToDate
        pStatus.Value = DePack.HospitalityBookingFilters.Status
        pCustomer.Value = DePack.HospitalityBookingFilters.Customer
        pPackage.Value = DePack.HospitalityBookingFilters.PackageDescription
        pProduct.Value = DePack.HospitalityBookingFilters.ProductDescription
        pMarkOrderFor.Value = DePack.HospitalityBookingFilters.MarkOrderFor
        pQandAStatus.Value = DePack.HospitalityBookingFilters.QandAStatus
        pPrintStatus.Value = DePack.HospitalityBookingFilters.PrintStatus
        pPrintMode.Value = String.Empty

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.TableMappings.Add("Table", "HospitalityBookings")
        _cmdAdapter.TableMappings.Add("Table1", "HospitalityPrintDetails")
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet)
        Utilities.ConvertISeriesTables(ResultDataSet)

        Dim drStatus As DataRow = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).NewRow
        If CStr(_cmd.Parameters(ErrorCode).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(ErrorCode).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Add(drStatus)
    End Sub
#End Region

#Region "Update Hospitality Booking Status"
    Private Function AccessDatabaseCS002S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Hospitality Bookings data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        Try
            CallCS002S()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Error during database Access"
                .ErrorNumber = "DBPackage-CS002S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Sub CallCS002S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL CS002S(@ErrorCode, @Source, @PARAM2, @PARAM3, @PARAM4)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pCallID As iDB2Parameter
        Dim pStatus As iDB2Parameter
        Dim pUser As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pSource = _cmd.Parameters.Add(Source, iDB2DbType.iDB2Char, 1)
        pCallID = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Decimal, 13)
        pStatus = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 1)
        pUser = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 10)

        pErrorCode.Direction = ParameterDirection.InputOutput

        pErrorCode.Value = String.Empty
        pSource.Value = Utilities.FixStringLength(DePack.Source, 10)
        pCallID.Value = Utilities.PadLeadingZeros(DePack.CallId, 13)
        pStatus.Value = Utilities.FixStringLength(DePack.Status, 1)
        pUser.Value = Utilities.FixStringLength(DePack.BoxOfficeUser, 10)

        _cmd.ExecuteNonQuery()

        Dim drStatus As DataRow = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).NewRow
        If CStr(_cmd.Parameters(ErrorCode).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(ErrorCode).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Add(drStatus)
    End Sub

    ''' <summary>
    ''' Format the iSeries 7 char date CYYMMDD to a date object and then to a string based on the global date format
    ''' </summary>
    ''' <param name="dtHospitalityBookings">The hospitality bookings data table</param>
    ''' <remarks></remarks>
    Private Sub getFormattedDate(ByRef dtHospitalityBookings As DataTable)
        Dim processDate As Date = Nothing
        Dim formattedDate As String = String.Empty
        For Each row As DataRow In dtHospitalityBookings.Rows
            processDate = Utilities.ISeriesDate(row("ProcessDate"))
            formattedDate = processDate.ToString(Settings.EcommerceModuleDefaultsValues.GlobalDateFormat)
            row("FormattedDate") = formattedDate
        Next
    End Sub
#End Region

#Region "Sold Hospitality Booking Details"
    Private Function AccessDatabaseCS003S() As ErrorObj
        Dim err As New ErrorObj
        Dim dRow As DataRow = Nothing
        ResultDataSet = New DataSet
        HospitalityBookingDataSet(ResultDataSet)
        Try
            CallCS003S()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = "Error during database Access"
                .ErrorNumber = "DBPackage-CS003S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Sub CallCS003S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL CS003S(@ErrorCode, @Source, @CallId)"
        _cmd.CommandType = CommandType.Text

        Dim pCallID As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pErrorCode As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
        pSource = _cmd.Parameters.Add(Source, iDB2DbType.iDB2Char, 1)
        pCallID = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Decimal, 13)
        pErrorCode.Direction = ParameterDirection.InputOutput

        pErrorCode.Value = String.Empty
        pSource.Value = DePack.Source
        pCallID.Value = DePack.CallId

        _cmdAdapter = New IBM.Data.DB2.iSeries.iDB2DataAdapter
        _cmdAdapter.TableMappings.Add("Table", "Package")
        _cmdAdapter.TableMappings.Add("Table1", "Component")
        _cmdAdapter.TableMappings.Add("Table2", "Seat")
        _cmdAdapter.TableMappings.Add("Table3", "ExtraComponents")
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet)

        Dim drStatus As DataRow = ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).NewRow
        If CStr(_cmd.Parameters(ErrorCode).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = GlobalConstants.ERRORFLAG
            drStatus("ReturnCode") = CStr(_cmd.Parameters(ErrorCode).Value).Trim
        Else
            drStatus("ErrorOccurred") = String.Empty
            drStatus("ReturnCode") = String.Empty
        End If
        ResultDataSet.Tables(GlobalConstants.STATUS_RESULTS_TABLE_NAME).Rows.Add(drStatus)
    End Sub

    Private Sub HospitalityBookingDataSet(ByRef resultdataSet As DataSet)
        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        resultdataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtPackage As New DataTable("Package")
        resultdataSet.Tables.Add(DtPackage)
        With DtPackage.Columns
            .Add("PackageID", GetType(Long))
            .Add("PackageDescription", GetType(String))
            .Add("PackageType", GetType(String))
            .Add("Discount", GetType(Decimal))
            .Add("Quantity", GetType(String))
            .Add("PriceBeforeVAT", GetType(Decimal))
            .Add("VATPrice", GetType(Decimal))
            .Add("PriceIncludingVAT", GetType(Decimal))
            .Add("Completed", GetType(Boolean))
            .Add("ErrorCode", GetType(String))
            .Add("ErrorDescription", GetType(String))
            .Add("ActiveFlag", GetType(Boolean))
            .Add("AllowComments", GetType(Boolean))
            .Add("PricingMethod", GetType(String))
            .Add("LeadSourceID", GetType(String))
            .Add("CallID", GetType(Long))
            .Add("PackageDiscountRemovedDueToPriceRecalc", GetType(Boolean))
            .Add("PackageDiscountedByValue", GetType(Decimal))
            .Add("PackageComponentLevelDiscountValue", GetType(Decimal))
            .Add("MarkOrderFor", GetType(String))
            .Add("CatMode", GetType(String))
            .Add("TemplateID", GetType(Integer))
            .Add("ExpandAccordion", GetType(Boolean))
            .Add("BookingCustomerNumber", GetType(String))
            .Add("BookingStatus", GetType(String))
        End With

        Dim DtComponent As New DataTable("Component")
        resultdataSet.Tables.Add(DtComponent)
        With DtComponent.Columns
            .Add("ComponentID", GetType(Long))
            .Add("ComponentDescription", GetType(String))
            .Add("Type", GetType(String))
            .Add("Discount", GetType(Decimal))
            .Add("DiscountValue", GetType(Decimal))
            .Add("Quantity", GetType(String))
            .Add("PriceBeforeVAT", GetType(Decimal))
            .Add("PriceBeforeVATExclDisc", GetType(Decimal))
            .Add("VATPrice", GetType(Decimal))
            .Add("PriceIncludingVAT", GetType(Decimal))
            .Add("UnitPrice", GetType(Decimal))
            .Add("Completed", GetType(Boolean))
            .Add("ErrorCode", GetType(String))
            .Add("ErrorDescription", GetType(String))
            .Add("ActiveFlag", GetType(Boolean))
            .Add("AllocateAutomatically", GetType(Boolean))
            .Add("ComponentGroupSequence", GetType(Integer))
            .Add("ComponentSequence", GetType(Integer))
            .Add("ComponentGroupID", GetType(Long))
            .Add("ComponentType", GetType(String))
            .Add("ComponentGroupType", GetType(String))
            .Add("ProductCode", GetType(String))
            .Add("AreaInError", GetType(String))
            .Add("Proceed", GetType(Boolean))
            .Add("MaxQty", GetType(Integer))
            .Add("MinQty", GetType(Integer))
            .Add("FromDate", GetType(Date))
            .Add("ToDate", GetType(Date))
            .Add("CanAmendSeat", GetType(Boolean))
            .Add("MaxDiscountPercent", GetType(Decimal))
            .Add("HasPrintableTicket", GetType(Boolean))
            .Add("IsExtraComponent", GetType(Boolean))
            .Add("HideSeatForPWS", GetType(Boolean))
        End With

        Dim DtSeat As New DataTable("Seat")
        resultdataSet.Tables.Add(DtSeat)
        With DtSeat.Columns
            .Add("ComponentID", GetType(Long))
            .Add("ProductCode", GetType(String))
            .Add("SeatDetails", GetType(String))
            .Add("AlphaSuffix", GetType(String))
            .Add("CustomerNumber", GetType(String))
            .Add("PriceBand", GetType(String))
            .Add("Price", GetType(String))
            .Add("ValidPriceBands", GetType(String))
            .Add("PriceCode", GetType(String))
            .Add("ErrorCode", GetType(String))
            .Add("MaxLimit", GetType(Integer))
            .Add("UserLimit", GetType(Integer))
            .Add("ErrorInformation", GetType(String))
            .Add("RovingOrUnreserved", GetType(String))
            .Add("BulkQuantity", GetType(Integer))
            .Add("BulkID", GetType(Long))
            .Add("CanAmendSeat", GetType(Boolean))
            .Add("DefaultProductPriceBand", GetType(String))
        End With

        Dim DtComponentAmendments As New DataTable("ComponentAmendments")
        resultdataSet.Tables.Add(DtComponentAmendments)
        With DtComponentAmendments.Columns
            .Add("ComponentDescription", GetType(String))
            .Add("TotalPrice", GetType(String))
            .Add("QuantityAmended", GetType(Integer))
            .Add("QuantityDeleted", GetType(Integer))
            .Add("QuantityAdded", GetType(Integer))
        End With

        Dim DtExtraComponents As New DataTable("ExtraComponents")
        resultdataSet.Tables.Add(DtExtraComponents)
        With DtExtraComponents.Columns
            .Add("ComponentID", GetType(Long))
            .Add("ComponentCode", GetType(String))
            .Add("ComponentDescription", GetType(String))
            .Add("AvailabilityFlag", GetType(Boolean))
            .Add("PWSFlag", GetType(Boolean))
        End With
    End Sub

#End Region

#Region "Print Hospitality Bookings"
    Private Function AccessDatabaseWS696R() As ErrorObj
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
            PARAMOUT = CallWS696R()
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            ''Was the call to WS696R succesful? 
            If PARAMOUT.Length > 0 AndAlso PARAMOUT.Substring(0, 10).Trim <> String.Empty Then
                dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                dRow("ReturnCode") = "PE"  ' Generic printing error 
            Else
                dRow("ErrorOccurred") = String.Empty
                dRow("ReturnCode") = String.Empty
            End If
            DtStatusResults.Rows.Add(dRow)
        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPK-WS696R"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS696R", strError, err, ex, LogTypeConstants.TCBMPACKAGELOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Function CallWS696R()

        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty


        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS696R (@PARAM1)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 5000)
        parmIO.Value = WS696RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()

        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString
        Return PARAMOUT

    End Function

    Private Function WS696RParm() As String

        Dim myString As New StringBuilder
        'Dim componentId = String.Empty
        Dim callId As Decimal = If(DePack.HospitalityBookingFilters.CallId > 0, DePack.HospitalityBookingFilters.CallId, DePack.CallId)

        myString.Append(Utilities.FixStringLength(String.Empty, 10))
        myString.Append(Utilities.PadLeadingZeros(DePack.HospitalityBookingFilters.MaxRecords, 5))
        myString.Append(Utilities.FixStringLength(DePack.HospitalityBookingFilters.Agent, 10))
        myString.Append(Utilities.PadLeadingZeros(callId, 13))
        myString.Append(Utilities.FixStringLength(DePack.HospitalityBookingFilters.Fromdate, 8))
        myString.Append(Utilities.FixStringLength(DePack.HospitalityBookingFilters.ToDate, 8))
        myString.Append(Utilities.FixStringLength(DePack.HospitalityBookingFilters.Status, 1))
        myString.Append(Utilities.PadLeadingZeros(DePack.HospitalityBookingFilters.Customer, 12))
        myString.Append(Utilities.FixStringLength(DePack.HospitalityBookingFilters.PackageDescription, 30))
        myString.Append(Utilities.FixStringLength(DePack.HospitalityBookingFilters.ProductDescription, 40))
        myString.Append(Utilities.FixStringLength(DePack.HospitalityBookingFilters.MarkOrderFor, 1))
        myString.Append(Utilities.FixStringLength(DePack.HospitalityBookingFilters.QandAStatus, 1))
        myString.Append(Utilities.FixStringLength(DePack.HospitalityBookingFilters.PrintStatus, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 1)) 'printMode   
        myString.Append(Utilities.FixStringLength(DePack.SeatToBePrinted, 15)) 'seat
        myString.Append(Utilities.FixStringLength(String.Empty, 1)) ' suffix
        myString.Append(Utilities.FixStringLength(GlobalConstants.BACKEND_INTERACTIVE_MODE, 1)) 'p#mode
        myString.Append(Utilities.FixStringLength(DePack.ProductCodeToBePrinted, 6)) 'Product code for future print user stories
        myString.Append(Utilities.FixStringLength(DePack.BoxOfficeUser, 10)) 'loggedin boxoffice user

        Dim componentId = String.Empty
        If DePack.ComponentID > 0 Then
            componentId = DePack.ComponentID
        End If
        myString.Append(Utilities.FixStringLength(componentId, 13)) ' ComponentId
        myString.Append(Utilities.FixStringLength(String.Empty, 4813))
        Return myString.ToString()

    End Function
#End Region

#Region "Generate Document For Hospitality Booking"
    Private Function AccessDatabaseWS640R() As ErrorObj
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

        Dim DtDocumentInformation As New DataTable("DocumentInformation")
        ResultDataSet.Tables.Add(DtDocumentInformation)
        With DtDocumentInformation.Columns
            .Add("MergedPath", GetType(String))
            .Add("File", GetType(String))
            .Add("ActivityRecordId", GetType(String))
        End With

        Try
            PARAMOUT = CallWS640R()
            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Length > 0 AndAlso PARAMOUT.Substring(4999, 1).Trim <> String.Empty Then
                dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                dRow("ReturnCode") = "DE"  ' Generic Document Production Error Code                
            Else
                dRow("ErrorOccurred") = String.Empty
                dRow("ReturnCode") = String.Empty
            End If
            DtStatusResults.Rows.Add(dRow)

            If DtStatusResults.Rows(0).Item("ErrorOccurred") = String.Empty Then
                dRow = Nothing
                dRow = DtDocumentInformation.NewRow
                dRow("MergedPath") = PARAMOUT.Substring(23, 150).Trim
                dRow("File") = PARAMOUT.Substring(173, 12).Trim
                dRow("ActivityRecordId") = PARAMOUT.Substring(185, 13).Trim
                DtDocumentInformation.Rows.Add(dRow)
            End If

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBPK-WS640R"
                .HasError = True
            End With
            TalentLogger.Logging(CLASSNAME, "AccessDatabaseWS640R", strError, err, ex, LogTypeConstants.TCBMPACKAGELOG, Settings.BusinessUnit, Settings.Partner, Settings.LoginId)
        End Try
        Return err
    End Function

    Private Function CallWS640R()

        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty


        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS640R (@PARAM1)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 5000)
        parmIO.Value = WS640RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()

        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString
        Return PARAMOUT

    End Function

    Private Function WS640RParm() As String

        Dim myString As New StringBuilder

        myString.Append(Utilities.PadLeadingZeros(DePack.CallId, 13))
        myString.Append(Utilities.FixStringLength(DePack.BoxOfficeUser, 10))
        myString.Append(Utilities.FixStringLength(String.Empty, 4977))

        Return myString.ToString()

    End Function
#End Region
End Class
