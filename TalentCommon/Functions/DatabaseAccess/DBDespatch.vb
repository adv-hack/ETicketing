Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
Imports Talent.Common
Imports Talent.Common.Utilities

<Serializable()> _
Public Class DBDespatch
    Inherits DBAccess

#Region "Public Properties"

    Public Property DeDespatch As DEDespatch
    Public Property DespatchCollection As New List(Of DEDespatchItem)

#End Region

#Region "Constants"

    Private Const errorString As String = "Error during database Access"
    Private Const RetrieveDespatchTransactionItems As String = "RetrieveDespatchTransactionItems"
    Private Const RetrieveDespatchProcessItems As String = "RetrieveDespatchProcessItems"
    Private Const RetrieveDespatchNoteItems As String = "RetrieveDespatchNoteItems"
    Private Const RetrievePaymentReferenceFromTicketNumber As String = "RetrievePaymentReferenceFromTicketNumber"
    Private Const DespatchCompletion As String = "DespatchCompletion"
    Private Const CreateTrackingReferences As String = "CreateTrackingReferences"
    Private Const DespatchOrderTokenCheck As String = "DespatchOrderTokenCheck"
    Private Const RetrieveDespatchAddressLabelItems As String = "RetrieveDespatchAddressLabelItems"

#End Region

#Region "Class Level Fields"

    Private _cmdAdapter As iDB2DataAdapter = Nothing
    Private _cmd As iDB2Command = Nothing

#End Region

#Region "Protected Methods"

    Protected Overrides Function AccessDataBaseTALENTTKT() As ErrorObj
        Dim err As New ErrorObj

        Select Case _settings.ModuleName
            Case Is = RetrieveDespatchTransactionItems : err = AccessDatabasePY106S()
            Case Is = RetrieveDespatchProcessItems : err = AccessDatabasePY107S()
            Case Is = RetrieveDespatchNoteItems : err = AccessDatabasePY107S2()
            Case Is = RetrievePaymentReferenceFromTicketNumber : err = AccessDatabasePY107S3()
            Case Is = DespatchOrderTokenCheck : err = AccessDatabasePY107S4()
            Case Is = DespatchCompletion : err = AccessDatabaseWS273R()
            Case Is = CreateTrackingReferences : err = AccessDatabasePY015BS()
            Case Is = RetrieveDespatchAddressLabelItems : err = AccessDatabasePY107S5()
        End Select

        Return err
    End Function

#End Region

#Region "Private Functions"
    ''' <summary>
    ''' Build the parameters and call the stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallPY107S()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL PY107S(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6)"
        _cmd.CommandType = CommandType.Text
        If DeDespatch.CommandTimeout > 0 Then _cmd.CommandTimeout = DeDespatch.CommandTimeout

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pBatchNumber As iDB2Parameter
        Dim pPaymentReference As IDbDataParameter
        Dim pSessionID As IDbDataParameter
        Dim pRefreshBatch As New iDB2Parameter
        Dim pMode As New iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pErrorCode.Value = String.Empty
        pSource = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pSource.Direction = ParameterDirection.InputOutput
        pSource.Value = DeDespatch.Source
        pBatchNumber = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Decimal, 13)
        pBatchNumber.Direction = ParameterDirection.InputOutput
        pBatchNumber.Value = DeDespatch.BatchID
        pPaymentReference = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Decimal, 15)
        pPaymentReference.Direction = ParameterDirection.InputOutput
        pPaymentReference.Value = DeDespatch.PaymentRef
        pSessionID = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 36)
        pSessionID.Direction = ParameterDirection.InputOutput
        pSessionID.Value = DeDespatch.SessionID
        pRefreshBatch = _cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 1)
        pRefreshBatch.Direction = ParameterDirection.InputOutput
        pRefreshBatch.Value = DeDespatch.RefreshBatch
        pMode = _cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 1)
        pMode.Direction = ParameterDirection.InputOutput
        pMode.Value = DeDespatch.Type

        _cmdAdapter = New iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "DespatchProcessItems")

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = CStr(_cmd.Parameters(0).Value).Trim
            drStatus("ReturnCode") = GlobalConstants.ERRORFLAG
        Else
            drStatus("ErrorOccurred") = ""
            drStatus("ReturnCode") = ""
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub
    Private Function AccessDatabasePY107S() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Despatch Process Items data table
        Dim DtDespatchProcessItems As New DataTable("DespatchProcessItems")
        ResultDataSet.Tables.Add(DtDespatchProcessItems)

        Try
            CallPY107S()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = errorString
                .ErrorNumber = "DBDespatch-PY107S"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Returns tables DespatchNoteItems and DespatchNoteItemsSummary
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AccessDatabasePY107S2() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Despatch Note Items data table
        Dim DtDespatchNoteItems As New DataTable("DespatchNoteItems")
        ResultDataSet.Tables.Add(DtDespatchNoteItems)

        'Create the Despatch Process Items Summary data table
        Dim DtDespatchNoteItemsSummary As New DataTable("DespatchNoteItemsSummary")
        ResultDataSet.Tables.Add(DtDespatchNoteItemsSummary)

        Try
            CallPY107S2()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = errorString
                .ErrorNumber = "DBDespatch-PY107S2"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Build the parameters and call the stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallPY107S2()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL PY107S2(@PARAM0, @PARAM1, @PARAM2)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pBatchNumber As iDB2Parameter
        Dim pMode As New iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pErrorCode.Value = String.Empty
        pBatchNumber = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Decimal, 13)
        pBatchNumber.Direction = ParameterDirection.InputOutput
        pBatchNumber.Value = DeDespatch.BatchID

        pMode = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1)
        pMode.Direction = ParameterDirection.InputOutput
        ' Retrieve the summary records for payments (1) or reservations (3) or retail (5)
        pMode.Value = "1"
        If DeDespatch.DespatchReservations Then pMode.Value = "3"
        If DeDespatch.DespatchRetail Then pMode.Value = "5"

        _cmdAdapter = New iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "DespatchNoteItemsSummary")

        ' Retrieve the detail records for payments (2) or reservations (4) or retail (6)
        pMode.Value = "2"
        If DeDespatch.DespatchReservations Then pMode.Value = "4"
        If DeDespatch.DespatchRetail Then pMode.Value = "6"

        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "DespatchNoteItems")

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = CStr(_cmd.Parameters(0).Value).Trim
            drStatus("ReturnCode") = GlobalConstants.ERRORFLAG
        Else
            drStatus("ErrorOccurred") = ""
            drStatus("ReturnCode") = ""
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

    ''' <summary>
    ''' Returns payment reference from passed ticket number
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AccessDatabasePY107S3() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        'Create the TicketSearchResults data table
        Dim DtTicketSearchResults As New DataTable("TicketSearchResults")
        ResultDataSet.Tables.Add(DtTicketSearchResults)
        With DtTicketSearchResults.Columns
            .Add("ReferenceType", GetType(String))
            .Add("ReferenceValue", GetType(String))
        End With
        Try
            CallPY107S3()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = errorString
                .ErrorNumber = "DBDespatch-PY107S3"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Returns result (blank or 'E') of check against TC043TBL.CH0543
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AccessDatabasePY107S4() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the TicketSearchResults data table
        Dim DtTokenCheckResults As New DataTable("TokenCheckResults")
        ResultDataSet.Tables.Add(DtTokenCheckResults)
        With DtTokenCheckResults.Columns
            .Add("Result", GetType(String))
        End With
        Try
            CallPY107S4()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = errorString
                .ErrorNumber = "DBDespatch-PY107S4"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Functionality for populating order tracking references
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabasePY015BS() As ErrorObj

        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Tracking References data table
        Dim DtTrackingReferences As New DataTable("CreateTrackingReferences")
        ResultDataSet.Tables.Add(DtTrackingReferences)

        Try
            CallPY015BS()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = errorString
                .ErrorNumber = "DBDespatch-PY015BS"
                .HasError = True
            End With
        End Try
        Return err

    End Function
    ''' <summary>
    ''' Returns a list of distinct address information for the batch number
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AccessDatabasePY107S5() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Tracking References data table
        Dim DtDespatchAddressLabelItems As New DataTable("DespatchAddressLabelItems")
        ResultDataSet.Tables.Add(DtDespatchAddressLabelItems)

        Try
            CallPY107S5()
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = errorString
                .ErrorNumber = "DBDespatch-PY107S5"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Private Sub CallPY107S5()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL PY107S5(@PARAM0, @PARAM1)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pBatchNumber As iDB2Parameter
        Dim pMode As New iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pErrorCode.Value = String.Empty
        pBatchNumber = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Decimal, 13)
        pBatchNumber.Direction = ParameterDirection.InputOutput
        pBatchNumber.Value = DeDespatch.BatchID

        _cmdAdapter = New iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "DespatchAddressLabelItems")

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = CStr(_cmd.Parameters(0).Value).Trim
            drStatus("ReturnCode") = GlobalConstants.ERRORFLAG
        Else
            drStatus("ErrorOccurred") = ""
            drStatus("ReturnCode") = ""
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

    ''' <summary>
    ''' Build the parameters and call the stored procedure.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallPY015BS()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL PY015BS(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4)"
        _cmd.CommandType = CommandType.Text

        Dim pMode As iDB2Parameter
        Dim pPaymentReference As iDB2Parameter
        Dim pTrackingReference As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pErrorCode As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.Output
        pSource = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pMode = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1)
        pPaymentReference = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Decimal, 15)
        pTrackingReference = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 100)

        pMode.Value = DeDespatch.Mode
        pPaymentReference.Value = Utilities.PadLeadingZeros(CType(DeDespatch.PaymentRef, String), 15)
        pTrackingReference.Value = DeDespatch.TrackingReference
        pSource.Value = DeDespatch.Source
        pErrorCode.Value = String.Empty

        _cmdAdapter = New iDB2DataAdapter
        _cmdAdapter.SelectCommand = _cmd
        _cmdAdapter.Fill(ResultDataSet, "CreateTrackingReferences")

        Dim drStatus As DataRow = ResultDataSet.Tables("ErrorStatus").NewRow
        If CStr(_cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = CStr(_cmd.Parameters(0).Value).Trim
            drStatus("ReturnCode") = GlobalConstants.ERRORFLAG
        End If
        ResultDataSet.Tables("ErrorStatus").Rows.Add(drStatus)
    End Sub

    ''' <summary>
    ''' Build the parameters and call the stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallPY107S3()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL PY107S3(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pSource As iDB2Parameter
        Dim pTicketNumber As iDB2Parameter
        Dim pPaymentReference As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pErrorCode.Value = String.Empty
        pSource = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pSource.Direction = ParameterDirection.Input
        pSource.Value = DeDespatch.Source
        pTicketNumber = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 24)
        pTicketNumber.Direction = ParameterDirection.Input
        pTicketNumber.Value = DeDespatch.TicketNumber
        pPaymentReference = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Decimal, 15)
        pPaymentReference.Direction = ParameterDirection.InputOutput
        pPaymentReference.Value = 0
        pPaymentReference = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 1)
        pPaymentReference.Direction = ParameterDirection.InputOutput
        pPaymentReference.Value = ""

        _cmd.ExecuteNonQuery()

        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If CStr(_cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = CStr(_cmd.Parameters(0).Value).Trim
            drStatus("ReturnCode") = GlobalConstants.ERRORFLAG
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)

        Dim drTicketSearchResults As DataRow = ResultDataSet.Tables("TicketSearchResults").NewRow
        drTicketSearchResults("ReferenceType") = _cmd.Parameters(Param4).Value.ToString()
        drTicketSearchResults("ReferenceValue") = CLng(CheckForDBNull_Decimal(_cmd.Parameters(Param3).Value.ToString()))
        ResultDataSet.Tables("TicketSearchResults").Rows.Add(drTicketSearchResults)

    End Sub

    ''' <summary>
    ''' Returns table DespatchTransactionItems (transactions to be despatched) 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function AccessDatabasePY106S() As ErrorObj

        Dim batchNumber As String = String.Empty
        Dim PaymentRef As String = String.Empty
        Dim err As New ErrorObj
        ResultDataSet = New DataSet

        'Create the Status data table
        Dim DtStatusResults As New DataTable("StatusResults")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With

        'Create the Despatch Process Items data table
        Dim DtDespatchTransactionItems As New DataTable("DespatchTransactionsItems")
        ResultDataSet.Tables.Add(DtDespatchTransactionItems)

        Try
            Dim cmd As iDB2Command = conTALENTTKT.CreateCommand()

            cmd.CommandText = "Call PY106S(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4, @PARAM5, @PARAM6, @PARAM7, @PARAM8, @PARAM9, @PARAM10, @PARAM11, @PARAM12, @PARAM13, @PARAM14, @PARAM15, @PARAM16, @PARAM17, @PARAM18, @PARAM19, @ErrorCode)"
            cmd.CommandType = CommandType.Text
            If DeDespatch.CommandTimeout > 0 Then cmd.CommandTimeout = DeDespatch.CommandTimeout

            Dim parmIO_0 As iDB2Parameter
            parmIO_0 = cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
            parmIO_0.Value = DeDespatch.Type
            parmIO_0.Direction = ParameterDirection.Input

            Dim parmIO_1 As iDB2Parameter
            parmIO_1 = cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 7)
            parmIO_1.Value = DeDespatch.FromDate
            parmIO_1.Direction = ParameterDirection.Input

            Dim parmIO_2 As iDB2Parameter
            parmIO_2 = cmd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 7)
            parmIO_2.Value = DeDespatch.ToDate
            parmIO_2.Direction = ParameterDirection.Input

            Dim parmIO_3 As iDB2Parameter
            parmIO_3 = cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 4)
            parmIO_3.Value = DeDespatch.SuperType
            parmIO_3.Direction = ParameterDirection.Input

            Dim parmIO_4 As iDB2Parameter
            parmIO_4 = cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 4)
            parmIO_4.Value = DeDespatch.SubType
            parmIO_4.Direction = ParameterDirection.Input

            Dim parmIO_5 As iDB2Parameter
            parmIO_5 = cmd.Parameters.Add(Param5, iDB2DbType.iDB2Char, 6)
            parmIO_5.Value = DeDespatch.Product
            parmIO_5.Direction = ParameterDirection.Input

            Dim parmIO_6 As iDB2Parameter
            parmIO_6 = cmd.Parameters.Add(Param6, iDB2DbType.iDB2Char, 3)
            parmIO_6.Value = DeDespatch.SeatDetails.Stand
            parmIO_6.Direction = ParameterDirection.Input

            Dim parmIO_7 As iDB2Parameter
            parmIO_7 = cmd.Parameters.Add(Param7, iDB2DbType.iDB2Char, 4)
            parmIO_7.Value = DeDespatch.SeatDetails.Area
            parmIO_7.Direction = ParameterDirection.Input

            Dim parmIO_8 As iDB2Parameter
            parmIO_8 = cmd.Parameters.Add(Param8, iDB2DbType.iDB2Char, 15)
            parmIO_8.Value = Utilities.PadLeadingZeros(CType(DeDespatch.PaymentRef, String), 15)
            parmIO_8.Direction = ParameterDirection.Input

            Dim parmIO_9 As iDB2Parameter
            parmIO_9 = cmd.Parameters.Add(Param9, iDB2DbType.iDB2Char, 1)
            parmIO_9.Value = DeDespatch.DeliveryMethod
            parmIO_9.Direction = ParameterDirection.Input

            Dim parmIO_10 As iDB2Parameter
            parmIO_10 = cmd.Parameters.Add(Param10, iDB2DbType.iDB2Char, 1)
            parmIO_10.Value = DeDespatch.Country
            parmIO_10.Direction = ParameterDirection.Input

            Dim parmIO_11 As iDB2Parameter
            parmIO_11 = cmd.Parameters.Add(Param11, iDB2DbType.iDB2Char, 1)
            parmIO_11.Value = DeDespatch.Postcode
            parmIO_11.Direction = ParameterDirection.Input

            Dim parmIO_12 As iDB2Parameter
            parmIO_12 = cmd.Parameters.Add(Param12, iDB2DbType.iDB2Char, 1)
            parmIO_12.Value = DeDespatch.GiftWrap
            parmIO_12.Direction = ParameterDirection.Input

            Dim parmIO_13 As iDB2Parameter
            parmIO_13 = cmd.Parameters.Add(Param13, iDB2DbType.iDB2Char, 1)
            parmIO_13.Value = ""
            parmIO_13.Direction = ParameterDirection.Input

            Dim parmIO_14 As iDB2Parameter
            parmIO_14 = cmd.Parameters.Add(Param14, iDB2DbType.iDB2Char, 1)
            parmIO_14.Value = ""
            If Not DeDespatch.IncludeGenericTransactions Then parmIO_14.Value = "N"
            parmIO_14.Direction = ParameterDirection.InputOutput

            Dim parmIO_15 As iDB2Parameter
            parmIO_15 = cmd.Parameters.Add(Param15, iDB2DbType.iDB2Char, 3)
            If DeDespatch.AttributeCategory Is Nothing Then DeDespatch.AttributeCategory = ""
            parmIO_15.Value = DeDespatch.AttributeCategory
            parmIO_15.Direction = ParameterDirection.InputOutput

            Dim parmIO_16 As iDB2Parameter
            parmIO_16 = cmd.Parameters.Add(Param16, iDB2DbType.iDB2Char, 2)
            If DeDespatch.PaymentMethod Is Nothing Then DeDespatch.PaymentMethod = ""
            parmIO_16.Value = DeDespatch.PaymentMethod
            parmIO_16.Direction = ParameterDirection.Input

            Dim parmIO_17 As iDB2Parameter
            parmIO_17 = cmd.Parameters.Add(Param17, iDB2DbType.iDB2Char, 10)
            If DeDespatch.AgentName Is Nothing Then DeDespatch.AgentName = ""
            parmIO_17.Value = DeDespatch.AgentName
            parmIO_17.Direction = ParameterDirection.Input

            Dim parmIO_18 As iDB2Parameter
            parmIO_18 = cmd.Parameters.Add(Param18, iDB2DbType.iDB2Char, 1)
            parmIO_18.Value = DeDespatch.TransactionType
            parmIO_18.Direction = ParameterDirection.Input


            Dim parmIO_19 As iDB2Parameter
            parmIO_19 = cmd.Parameters.Add(Param19, iDB2DbType.iDB2Char, 1)
            If DeDespatch.StrictSearch Then
                parmIO_19.Value = "Y"
            Else
                parmIO_19.Value = "N"
            End If
            parmIO_19.Direction = ParameterDirection.Input

            Dim pErrorCode As iDB2Parameter
            pErrorCode = cmd.Parameters.Add(ErrorCode, iDB2DbType.iDB2Char, 10)
            pErrorCode.Value = ""
            pErrorCode.Direction = ParameterDirection.InputOutput

            Dim cmdAdapter As New iDB2DataAdapter
            cmdAdapter.SelectCommand = cmd
            cmdAdapter.Fill(ResultDataSet, "DespatchTransactionsItems")

            Dim drStatus As DataRow = DtStatusResults.NewRow
            If CStr(cmd.Parameters(20).Value).Trim.Length > 0 Then
                drStatus("ErrorOccurred") = CStr(cmd.Parameters(19).Value).Trim
                drStatus("ReturnCode") = "E"
            Else
                drStatus("ErrorOccurred") = String.Empty
                drStatus("ReturnCode") = String.Empty
            End If
            DtStatusResults.Rows.Add(drStatus)

        Catch sqlex As iDB2Exception
            With err
                .ErrorMessage = sqlex.Message
                .ErrorStatus = errorString
                .ErrorNumber = "DBDespatch-PY106S-a"
                .HasError = True
            End With
            Dim drStatus As DataRow = DtStatusResults.NewRow
            drStatus("ErrorOccurred") = sqlex.Message
            drStatus("ReturnCode") = "E"
            DtStatusResults.Rows.Add(drStatus)
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = errorString
                .ErrorNumber = "DBDespatch-PY106S-b"
                .HasError = True
            End With
            Dim drStatus As DataRow = DtStatusResults.NewRow
            drStatus("ErrorOccurred") = ex.Message
            drStatus("ReturnCode") = "E"
            DtStatusResults.Rows.Add(drStatus)
        End Try

        Return err
    End Function

    ''' <summary>
    ''' Build the parameters and call the stored procedure
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CallPY107S4()
        _cmd = conTALENTTKT.CreateCommand()
        _cmd.CommandText = "CALL PY107S4(@PARAM0, @PARAM1, @PARAM2, @PARAM3, @PARAM4)"
        _cmd.CommandType = CommandType.Text

        Dim pErrorCode As iDB2Parameter
        Dim pMode As iDB2Parameter
        Dim pPaymentOrResReference As iDB2Parameter
        Dim pTimeStampToken As iDB2Parameter
        Dim pResult As iDB2Parameter

        pErrorCode = _cmd.Parameters.Add(Param0, iDB2DbType.iDB2Char, 10)
        pErrorCode.Direction = ParameterDirection.InputOutput
        pErrorCode.Value = String.Empty
        pMode = _cmd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1)
        pMode.Direction = ParameterDirection.Input
        pMode.Value = DeDespatch.Mode
        pPaymentOrResReference = _cmd.Parameters.Add(Param2, iDB2DbType.iDB2Decimal, 15)
        pPaymentOrResReference.Direction = ParameterDirection.Input
        pPaymentOrResReference.Value = DeDespatch.PaymentRef
        pTimeStampToken = _cmd.Parameters.Add(Param3, iDB2DbType.iDB2Char, 50)
        pTimeStampToken.Direction = ParameterDirection.Input
        pTimeStampToken.Value = DeDespatch.TimeStampToken
        pResult = _cmd.Parameters.Add(Param4, iDB2DbType.iDB2Char, 1)
        pResult.Direction = ParameterDirection.InputOutput
        pResult.Value = ""

        _cmd.ExecuteNonQuery()

        Dim drStatus As DataRow = ResultDataSet.Tables("StatusResults").NewRow
        If CStr(_cmd.Parameters(0).Value).Trim.Length > 0 Then
            drStatus("ErrorOccurred") = CStr(_cmd.Parameters(0).Value).Trim
            drStatus("ReturnCode") = GlobalConstants.ERRORFLAG
        End If
        ResultDataSet.Tables("StatusResults").Rows.Add(drStatus)

        Dim drTokenCheckResults As DataRow = ResultDataSet.Tables("TokenCheckResults").NewRow
        drTokenCheckResults("Result") = _cmd.Parameters(Param4).Value.ToString()
        ResultDataSet.Tables("TokenCheckResults").Rows.Add(drTokenCheckResults)

    End Sub
    ''' <summary>
    ''' Functionality for completing a despatch order
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Private Function AccessDatabaseWS273R() As ErrorObj
        Dim err As New ErrorObj
        ResultDataSet = New DataSet
        Dim dRow As DataRow = Nothing
        Dim PARAMOUT As String = String.Empty
        Dim nextRecord As Integer = 0
        Dim moreRecords As Boolean = True
        Dim boolNoErrors As Boolean = True
        dRow = Nothing
        Dim DtStatusResults As New DataTable("ErrorStatus")
        ResultDataSet.Tables.Add(DtStatusResults)
        With DtStatusResults.Columns
            .Add("ErrorOccurred", GetType(String))
            .Add("ReturnCode", GetType(String))
        End With
        Try
            Do While moreRecords And boolNoErrors
                PARAMOUT = CallWS273R(nextRecord, moreRecords)
                If PARAMOUT.Substring(1023, 1) = GlobalConstants.ERRORFLAG Then
                    dRow = ResultDataSet.Tables("ErrorStatus").NewRow
                    dRow("ErrorOccurred") = GlobalConstants.ERRORFLAG
                    dRow("ReturnCode") = PARAMOUT.Substring(1021, 2)
                    ResultDataSet.Tables("ErrorStatus").Rows.Add(dRow)
                    boolNoErrors = False
                End If
            Loop
            If PARAMOUT.Substring(1023, 1) = GlobalConstants.SUCCESSFLAG Then
                dRow = ResultDataSet.Tables("ErrorStatus").NewRow
                dRow("ErrorOccurred") = GlobalConstants.SUCCESSFLAG
                dRow("ReturnCode") = String.Empty
                ResultDataSet.Tables("ErrorStatus").Rows.Add(dRow)
            End If
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = errorString
                .ErrorNumber = "DBDespatch-WS273R"
                .HasError = True
            End With
        End Try
        Return err
    End Function

    ''' <summary>
    ''' Call the RPG program with the given record counts
    ''' </summary>
    ''' <param name="nextRecord">The next record to start at</param>
    ''' <param name="moreRecords">The more records flag</param>
    ''' <returns>The first parameter value</returns>
    ''' <remarks></remarks>
    Private Function CallWS273R(ByRef nextRecord As Integer, ByRef moreRecords As Boolean) As String
        Dim cmdSELECT As iDB2Command = Nothing
        Dim strHEADER As String = "CALL " & Settings.StoredProcedureGroup.Trim & "/WS273R(@PARAM1, @PARAM2)"
        Dim parmIO1 As iDB2Parameter
        Dim parmIO2 As iDB2Parameter
        Dim PARAMOUT As String = String.Empty

        cmdSELECT = New iDB2Command(strHEADER, conTALENTTKT)
        parmIO1 = cmdSELECT.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
        parmIO1.Value = WS273RParm1()
        parmIO1.Direction = ParameterDirection.InputOutput

        parmIO2 = cmdSELECT.Parameters.Add(Param2, iDB2DbType.iDB2Char, 32765)
        parmIO2.Value = WS273RParm2(nextRecord, moreRecords)
        parmIO2.Direction = ParameterDirection.InputOutput

        TalentCommonLog("CallWS273R", DeDespatch.AgentName, "Backend Request: strHEADER=" & strHEADER & ", parmIO1.Value=" & parmIO1.Value)
        cmdSELECT.ExecuteNonQuery()
        PARAMOUT = cmdSELECT.Parameters(Param1).Value.ToString
        populateDespatchItemsWithErrorCodes(cmdSELECT.Parameters(Param2).Value.ToString, nextRecord - 300)
        Return PARAMOUT
    End Function
    ''' <summary>
    ''' Format the first parameter with the order details
    ''' </summary>
    ''' <returns>The formatted string</returns>
    ''' <remarks></remarks>
    Private Function WS273RParm1() As String
        Dim myString As New StringBuilder
        myString.Append(Utilities.FixStringLength(DeDespatch.SessionID, 36))
        myString.Append(Utilities.PadLeadingZeros(DeDespatch.PaymentRef, 15)) '51
        myString.Append(Utilities.PadLeadingZeros(DeDespatch.BatchID, 13)) '64
        myString.Append(Utilities.FixStringLength(DeDespatch.Comments, 800)) '864
        myString.Append(Utilities.FixStringLength(DeDespatch.Type, 1)) '865
        myString.Append(Utilities.FixStringLength(String.Empty, 90))
        myString.Append(Utilities.FixStringLength(DeDespatch.TimeStampToken, 50))
        myString.Append(Utilities.PadLeadingZeros(String.Empty, 3))
        myString.Append(Utilities.FixStringLength(ConvertToYN(DeDespatch.PrintSelectAll), 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 1))
        myString.Append(Utilities.FixStringLength(DeDespatch.AgentName, 10))
        myString.Append(Utilities.FixStringLength(DeDespatch.Source, 1))
        myString.Append(Utilities.FixStringLength(String.Empty, 3))
        Return myString.ToString()
    End Function

    ''' <summary>
    ''' Format the second parameter with the order lines
    ''' </summary>
    ''' <param name="nextRecord">The next record to start at</param>
    ''' <param name="moreRecords">The more records flag</param>
    ''' <returns>The formatted string</returns>
    ''' <remarks></remarks>
    Private Function WS273RParm2(ByRef nextRecord As Integer, ByRef moreRecords As Boolean) As String
        Dim myString As New StringBuilder
        Dim i As Integer = nextRecord
        nextRecord = nextRecord + 300
        Do While DespatchCollection.Count > i AndAlso i < (nextRecord)
            myString.Append(Utilities.FixStringLength(DespatchCollection.Item(i).Product, 6))
            myString.Append(Utilities.FixStringLength(DespatchCollection.Item(i).SeatDetails.Stand, 3))
            myString.Append(Utilities.FixStringLength(DespatchCollection.Item(i).SeatDetails.Area, 4))
            myString.Append(Utilities.FixStringLength(DespatchCollection.Item(i).SeatDetails.Row, 4))
            myString.Append(Utilities.FixStringLength(DespatchCollection.Item(i).SeatDetails.Seat, 4))
            myString.Append(Utilities.FixStringLength(DespatchCollection.Item(i).SeatDetails.AlphaSuffix, 1))
            myString.Append(Utilities.FixStringLength(DespatchCollection.Item(i).TicketNumber, 15))
            myString.Append(Utilities.FixStringLength(DespatchCollection.Item(i).MembershipRFID, 15))
            myString.Append(Utilities.FixStringLength(DespatchCollection.Item(i).MembershipMagScan, 18))
            myString.Append(Utilities.FixStringLength(DespatchCollection.Item(i).MembershipMetalBadge, 5))
            myString.Append(Utilities.FixStringLength(String.Empty, 2)) 'Error code
            myString.Append(Utilities.FixStringLength(DespatchCollection.Item(i).Status, 1)) 'Status
            myString.Append(Utilities.FixStringLength(DespatchCollection.Item(i).PrintTicket, 1)) 'Print Ticket
            myString.Append(Utilities.FixStringLength(String.Empty, 21))
            i += 1
        Loop
        Dim blank As Integer = 32765 - myString.Length
        myString.Append(Utilities.FixStringLength(String.Empty, blank))
        moreRecords = (DespatchCollection.Count > nextRecord)
        Return myString.ToString()
    End Function

    ''' <summary>
    ''' Add any error codes to the existing data entities collection
    ''' </summary>
    ''' <param name="paramString">The formatted return parameter string</param>
    ''' <remarks></remarks>
    Private Sub populateDespatchItemsWithErrorCodes(ByVal paramString As String, intStartIndex As Integer)
        Dim iPosition As Integer = 0
        Dim i As Integer = intStartIndex
        Do While paramString.Substring(iPosition + 1, 1).Trim().Length > 0
            Dim errorCode As String = String.Empty
            errorCode = paramString.Substring(iPosition + 75, 2).Trim()
            DespatchCollection.Item(i).ErrorCode = errorCode
            iPosition = iPosition + 100
            i += 1
        Loop
    End Sub

#End Region

End Class
