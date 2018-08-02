Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
Imports System.Text.RegularExpressions
Imports Talent.Common
Imports Talent.Common.Utilities
Imports System.Linq

<Serializable()> _
Public Class DBVouchers
    Inherits DBAccess

#Region "Class Level Fields"

#End Region

#Region "Public Properties"

    Public Property DeVouch() As DEVouchers

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
            Case Is = "GetVoucherList" : err = AccessDatabaseWS138R()
            Case Is = "AddOrUpdateVoucherDetails" : err = AccessDatabaseWS139R()
            Case Is = "RedeemVoucher" : err = AccessDatabaseWS137R()
            Case Is = "GetCustomerVoucherDetails" : err = AccessDatabaseWS143R()
            Case Is = "ConvertVoucherToOnAccount" : err = AccessDatabaseWS137R()
        End Select

        Return err
    End Function

#End Region

#Region "Private Functions"

    Private Function AccessDatabaseWS138R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim returnCode As String = String.Empty
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
        Dim DtVoucherList As New DataTable("VoucherList")
        ResultDataSet.Tables.Add(DtVoucherList)
        With DtVoucherList.Columns
            .Add("VoucherId", GetType(Integer))
            .Add("Description", GetType(String))
            .Add("VoucherType", GetType(String))
            .Add("ExpiryInformation", GetType(String))
            .Add("Months", GetType(Integer))
            .Add("Days", GetType(Integer))
            .Add("ExpiryDate", GetType(Date))
            .Add("Active", GetType(Boolean))
        End With


        Try

            Do While moreRecords
                'Customers are returned in PARAM2 and PARAM3
                PARAMOUT = CallWS138R(lastId, moreRecords)

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
                        dRow = DtVoucherList.NewRow
                        dRow("VoucherId") = PARAMOUT.Substring(iPosition, 13)
                        dRow("Description") = PARAMOUT.Substring(iPosition + 13, 50).Trim()
                        dRow("VoucherType") = convertGETOGiftExperience(PARAMOUT.Substring(iPosition + 64, 1))
                        If String.IsNullOrEmpty(PARAMOUT.Substring(iPosition + 65, 7)) OrElse Int32.Parse(PARAMOUT.Substring(iPosition + 65, 7)) = 0 Then
                            If Not String.IsNullOrEmpty(PARAMOUT.Substring(iPosition + 72, 2)) Then
                                dRow("ExpiryInformation") = String.Concat("Expires after ", PARAMOUT.Substring(iPosition + 72, 2), " months and ", PARAMOUT.Substring(iPosition + 74, 3), " days.")
                                dRow("Months") = PARAMOUT.Substring(iPosition + 72, 2)
                                dRow("Days") = PARAMOUT.Substring(iPosition + 74, 3)
                            Else
                                dRow("ExpiryInformation") = String.Concat("Expires after ", PARAMOUT.Substring(iPosition + 74, 3), " days.")
                                dRow("Days") = PARAMOUT.Substring(iPosition + 74, 3)
                            End If

                        Else
                            dRow("ExpiryInformation") = String.Concat("Expires on ", ISeriesDate(PARAMOUT.Substring(iPosition + 65, 7)).ToString())
                            dRow("ExpiryDate") = ISeriesDate(PARAMOUT.Substring(iPosition + 65, 7))
                        End If
                        dRow("Active") = convertToBool(PARAMOUT.Substring(iPosition + 63, 1))

                        DtVoucherList.Rows.Add(dRow)
                        iPosition = iPosition + 100
                    Loop

                End If

                'Check if there's any more records
                If PARAMOUT.Substring(10222, 1) = "Y" Then
                    moreRecords = True
                    lastId = CType(PARAMOUT.Substring(10223, 13), Integer)
                Else
                    moreRecords = False
                End If
            Loop

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS138R-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS138R(ByVal lastId As Integer, ByVal more As Boolean) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS138R (@PARAM1)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 10240)
        parmIO.Value = WS138RParm(lastId, more)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString


        Return PARAMOUT
    End Function

    Private Function WS138RParm(ByVal lastId As Integer?, ByVal more As Boolean) As String
        Dim myString As New StringBuilder
        more = If(lastId <> 0, False)
        'Construct the parameter
        myString.Append(Utilities.FixStringLength(String.Empty, 10221))
        myString.Append(Utilities.FixStringLength(ConvertToYN(DeVouch.ShowActiveAndInActiveRecords), 1))
        myString.Append(Utilities.FixStringLength(Utilities.ConvertToYN(more), 1))
        myString.Append(Utilities.FixStringLength(Utilities.PadLeadingZeros(lastId, 13), 13))
        myString.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))

        Return myString.ToString()

    End Function

    Private Function AccessDatabaseWS139R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim returnCode As String = String.Empty
        Dim lastId As Integer = 0
        Dim moreRecords As Boolean = True

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        Dim DtVoucherDetails As New DataTable("VoucherDetails")
        ResultDataSet.Tables.Add(DtVoucherDetails)
        With DtVoucherDetails.Columns
            .Add("VoucherId", GetType(Integer))
        End With

        Try

            PARAMOUT = CallWS139R()

            dRow = Nothing
            dRow = DtVoucherDetails.NewRow
            dRow("VoucherId") = PARAMOUT.Substring(1, 13)
            DtVoucherDetails.Rows.Add(dRow)

            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" OrElse PARAMOUT.Substring(1021, 2).Trim <> String.Empty Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2).Trim
                moreRecords = False
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)


        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS138R-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS139R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS139R (@PARAM1)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS139RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString


        Return PARAMOUT
    End Function

    Private Function WS139RParm() As String
        Dim stringPassedToDatabase As New StringBuilder

        stringPassedToDatabase.Append(Utilities.FixStringLength(ConvertToAddUpdate(DeVouch.Mode), 1))
        stringPassedToDatabase.Append(Utilities.PadLeadingZeros(DeVouch.VoucherDefinitionId.ToString, 13))
        stringPassedToDatabase.Append(Utilities.FixStringLength(DeVouch.VoucherDescription, 40))
        stringPassedToDatabase.Append(Utilities.FixStringLength(ConvertToGiftExperience(DeVouch.VoucherType), 1))
        stringPassedToDatabase.Append(Utilities.FixStringLength(ConvertToYN(DeVouch.ShowActiveAndInActiveRecords), 1))
        stringPassedToDatabase.Append(Utilities.PadLeadingZeros(DateToIseriesFormat(DeVouch.ExpiryDate), 7))
        stringPassedToDatabase.Append(Utilities.PadLeadingZeros(DeVouch.ExpiryMonths.ToString(), 2))
        stringPassedToDatabase.Append(Utilities.PadLeadingZeros(DeVouch.ExpiryDays.ToString(), 3))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 952))
        stringPassedToDatabase.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 3))

        Return stringPassedToDatabase.ToString()
    End Function

    Private Function AccessDatabaseWS137R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim returnCode As String = String.Empty
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
        Dim DtVoucherList As New DataTable("GiftVoucherInfo")
        ResultDataSet.Tables.Add(DtVoucherList)
        With DtVoucherList.Columns
            .Add("VoucherCode", GetType(String))
            .Add("GiftVoucherPrice", GetType(Decimal))
            .Add("OnAccountTotal", GetType(Decimal))
        End With

        Try

            PARAMOUT = CallWS137R()

            'Set the response data on the first call to WS137R

            dRow = Nothing
            dRow = DtStatusResults.NewRow
            If PARAMOUT.Substring(1023, 1) = "E" OrElse PARAMOUT.Substring(1021, 2).Trim <> String.Empty Then
                dRow("ErrorOccurred") = "E"
                dRow("ReturnCode") = PARAMOUT.Substring(1021, 2).Trim
                moreRecords = False
            Else
                dRow("ErrorOccurred") = ""
                dRow("ReturnCode") = ""
            End If
            DtStatusResults.Rows.Add(dRow)

            Dim iPosition As Integer = 0
            dRow = Nothing
            dRow = DtVoucherList.NewRow
            dRow("VoucherCode") = Utilities.CheckForDBNull_String(PARAMOUT.Substring(iPosition, 30).Trim())
            dRow("GiftVoucherPrice") = Utilities.CheckForDBNull_Decimal(PARAMOUT.Substring(iPosition + 121, 15).ConvertStringToDecimal())
            dRow("OnAccountTotal") = Utilities.CheckForDBNull_Decimal(PARAMOUT.Substring(iPosition + 136, 15).ConvertStringToDecimal())
            DtVoucherList.Rows.Add(dRow)

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS137R-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS137R() As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS137R (@PARAM1)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 1024)
        parmIO.Value = WS137RParm()
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString


        Return PARAMOUT
    End Function

    Private Function WS137RParm() As String
        Dim stringPassedToDatabase As New StringBuilder

        'Construct the parameter
        stringPassedToDatabase.Append(Utilities.FixStringLength(DeVouch.VoucherCode, 30))
        stringPassedToDatabase.Append(Utilities.PadLeadingZeros(DeVouch.CustomerNumber, 12))
        stringPassedToDatabase.Append(Utilities.FixStringLength(ConvertRedeemModeToString(DeVouch.RedeemMode), 1))
        stringPassedToDatabase.Append(Utilities.FixStringLength(DeVouch.ExternalCompany, 50))
        stringPassedToDatabase.Append(Utilities.PadLeadingZeros(DeVouch.VoucherDefinitionId.ToString, 13))
        'The Price should be passed in pence and not in pounds. For ex. if the price is 5.99, it will be passed as (5.99 * 100) 599.
        stringPassedToDatabase.Append(Utilities.PadLeadingZeros((DeVouch.VoucherPrice.ConvertToISeriesIntegerValue()).ToString, 15))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 15))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 15))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 835))
        stringPassedToDatabase.Append(Utilities.PadLeadingZeros(DeVouch.UniqueVoucherId, 13))
        stringPassedToDatabase.Append(Utilities.FixStringLength(DeVouch.AgreementCode, 10))
        stringPassedToDatabase.Append(Utilities.FixStringLength(ConvertToYN(DeVouch.ExternalVoucherCodeFlag), 1))
        stringPassedToDatabase.Append(Utilities.FixStringLength(DeVouch.BoxOfficeUser, 10))
        stringPassedToDatabase.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 3))

        Return stringPassedToDatabase.ToString()

    End Function

    Private Function AccessDatabaseWS143R() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim returnCode As String = String.Empty
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
        Dim DtUnusedVoucherList As New DataTable("UnusedVoucherList")
        ResultDataSet.Tables.Add(DtUnusedVoucherList)
        With DtUnusedVoucherList.Columns
            .Add("VoucherId", GetType(Integer))
            .Add("Description", GetType(String))
            .Add("ExpiryDate", GetType(Date))
            .Add("SalePrice", GetType(String))
            .Add("VoucherCode", GetType(String))
            .Add("OnAccountBalance", GetType(String))
            .Add("UniqueVoucherId", GetType(Integer))
            .Add("VoucherSource", GetType(String))
            .Add("ExternalCompanyName", GetType(String))
        End With


        Try

            Do While moreRecords
                'Customers are returned in PARAM2 and PARAM3
                PARAMOUT = CallWS143R(lastId)

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
                    


                    If String.IsNullOrEmpty(PARAMOUT.Substring(iPosition, 13).Trim()) AndAlso Not String.IsNullOrEmpty(PARAMOUT.Substring(10195, 15).Trim()) Then
                        dRow = Nothing
                        dRow = DtUnusedVoucherList.NewRow
                        dRow("OnAccountBalance") = FormatPrice(PARAMOUT.Substring(10195, 15))
                        DtUnusedVoucherList.Rows.Add(dRow)
                    Else
                        iPosition = 0
                        Do While Not String.IsNullOrEmpty(PARAMOUT.Substring(iPosition, 13).Trim())
                            dRow = Nothing
                            dRow = DtUnusedVoucherList.NewRow
                            dRow("VoucherId") = PARAMOUT.Substring(iPosition, 13)
                            dRow("Description") = PARAMOUT.Substring(iPosition + 13, 50).Trim()
                            dRow("ExpiryDate") = ISeriesDate(PARAMOUT.Substring(iPosition + 63, 7))
                            dRow("SalePrice") = FormatPrice(PARAMOUT.Substring(iPosition + 70, 15))
                            dRow("VoucherCode") = PARAMOUT.Substring(iPosition + 85, 30).Trim()
                            dRow("UniqueVoucherId") = PARAMOUT.Substring(iPosition + 115, 13).Trim()
                            dRow("VoucherSource") = PARAMOUT.Substring(iPosition + 128, 1).Trim()
                            dRow("ExternalCompanyName") = PARAMOUT.Substring(iPosition + 129, 49).Trim()
                            dRow("OnAccountBalance") = FormatPrice(PARAMOUT.Substring(10195, 15))
                            DtUnusedVoucherList.Rows.Add(dRow)
                            iPosition = iPosition + 200
                        Loop
                    End If
                End If

                'Check if there's any more records
                If PARAMOUT.Substring(10222, 1) = "Y" Then
                    moreRecords = True
                    lastId = CType(PARAMOUT.Substring(10223, 13), Integer)
                Else
                    moreRecords = False
                End If
            Loop

        Catch ex As Exception
            Const strError As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACDBCC-WS143R-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    Private Function CallWS143R(ByVal lastId As Integer) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As New StringBuilder
        Dim parmIO As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        strHEADER.Append("CALL ")
        strHEADER.Append(Settings.StoredProcedureGroup.Trim)
        strHEADER.Append("/WS143R (@PARAM1)")
        cmdSELECT = New iDB2Command(strHEADER.ToString(), conTALENTTKT)

        parmIO = cmdSELECT.Parameters.Add("@PARAM1", iDB2DbType.iDB2Char, 10240)
        parmIO.Value = WS143RParm(lastId)
        parmIO.Direction = ParameterDirection.InputOutput

        'Execute
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters("@PARAM1").Value.ToString


        Return PARAMOUT
    End Function

    Private Function WS143RParm(ByVal lastId As Integer) As String
        Dim stringPassedToDatabase As New StringBuilder

        'Construct the parameter
        'stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 10210))
        'JIRA - MBSTS-4726
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 9600))
        stringPassedToDatabase.Append(Utilities.FixStringLength(Settings.OriginatingSource, 10))
        If DeVouch.RetrieveUsedVoucher Then
            stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 584))
            stringPassedToDatabase.Append(Utilities.ConvertToYN(DeVouch.RetrieveUsedVoucher, 1))
            stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 15))
        Else
            stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 600))
        End If
        stringPassedToDatabase.Append(Utilities.PadLeadingZeros(DeVouch.CustomerNumber, 12))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 1))
        stringPassedToDatabase.Append(Utilities.PadLeadingZeros(lastId, 13))
        stringPassedToDatabase.Append(Utilities.FixStringLength(Settings.OriginatingSourceCode, 1))
        stringPassedToDatabase.Append(Utilities.FixStringLength(String.Empty, 3))

        Return stringPassedToDatabase.ToString()

    End Function
#End Region

End Class
